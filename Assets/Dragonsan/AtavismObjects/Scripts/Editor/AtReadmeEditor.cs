﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;

namespace Atavism
{


	[CustomEditor(typeof(AtReadme))]
	[InitializeOnLoad]
	public class AtReadmeEditor : Editor
	{

		static string sessionName = "Atavims.showedReadme";

		static float kSpace = 16f;

		static AtReadmeEditor()
		{
			EditorApplication.delayCall += AutoSelectReadme;
		}

		static void AutoSelectReadme()
		{
			if (!SessionState.GetBool(sessionName, false))
			{
				var readme = SelectReadme();
				SessionState.SetBool(sessionName, true);

				/*if (readme && !readme.loadedLayout)
				{
					LoadLayout();
					readme.loadedLayout = true;
				}*/
			}
		}

		/*static void LoadLayout()
		{
			var assembly = typeof(EditorApplication).Assembly;
			var windowLayoutType = assembly.GetType("UnityEditor.WindowLayout", true);
			var method = windowLayoutType.GetMethod("LoadWindowLayout", BindingFlags.Public | BindingFlags.Static);
			method.Invoke(null, new object[] {Path.Combine(Application.dataPath, "TutorialInfo/Layout.wlt"), false});
		}*/

		[MenuItem("Window/Atavism/Show Read Me")]
		static AtReadme SelectReadme()
		{
			var ids = AssetDatabase.FindAssets("Readme t:AtReadme");
			if (ids.Length == 1)
			{
				var obj = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(ids[0]));

				Selection.objects = new UnityEngine.Object[] {obj};

				return (AtReadme) obj;
			}
			else
			{
				Debug.Log("Couldn't find a readme");
				return null;
			}
		}

		protected override void OnHeaderGUI()
		{
			var readme = (AtReadme) target;
			Init();

			var iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth / 3f - 20f, 128f);

			GUILayout.BeginHorizontal("In BigTitle");
			{
				GUILayout.Label(readme.icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
				GUILayout.Label(readme.title, TitleStyle);
			}
			GUILayout.EndHorizontal();
		}

		public override void OnInspectorGUI()
		{
			var readme = (AtReadme) target;
			Init();

			foreach (var section in readme.sections)
			{
				if (!string.IsNullOrEmpty(section.title))
				{
					GUILayout.Label(section.title, HeadingStyle);
				}

				if (!string.IsNullOrEmpty(section.text))
				{
					GUILayout.Label(section.text, BodyStyle);
				}

				if (!string.IsNullOrEmpty(section.linkText))
				{
					if (LinkLabel(new GUIContent(section.linkText)))
					{
						Application.OpenURL(section.url);
					}
				}

				GUILayout.Space(kSpace);
			}
		}


		bool m_Initialized;

		GUIStyle LinkStyle
		{
			get { return m_LinkStyle; }
		}

		[SerializeField] GUIStyle m_LinkStyle;

		GUIStyle TitleStyle
		{
			get { return m_TitleStyle; }
		}

		[SerializeField] GUIStyle m_TitleStyle;

		GUIStyle HeadingStyle
		{
			get { return m_HeadingStyle; }
		}

		[SerializeField] GUIStyle m_HeadingStyle;

		GUIStyle BodyStyle
		{
			get { return m_BodyStyle; }
		}

		[SerializeField] GUIStyle m_BodyStyle;

		void Init()
		{
			if (m_Initialized)
				return;
			m_BodyStyle = new GUIStyle(EditorStyles.label);
			m_BodyStyle.wordWrap = true;
			m_BodyStyle.fontSize = 14;

			m_TitleStyle = new GUIStyle(m_BodyStyle);
			m_TitleStyle.fontSize = 26;

			m_HeadingStyle = new GUIStyle(m_BodyStyle);
			m_HeadingStyle.fontSize = 18;

			m_LinkStyle = new GUIStyle(m_BodyStyle);
			m_LinkStyle.wordWrap = false;
			// Match selection color which works nicely for both light and dark skins
			m_LinkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
			m_LinkStyle.stretchWidth = false;

			m_Initialized = true;
		}

		bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
		{
			var position = GUILayoutUtility.GetRect(label, LinkStyle, options);

			Handles.BeginGUI();
			Handles.color = LinkStyle.normal.textColor;
			Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
			Handles.color = Color.white;
			Handles.EndGUI();
			

			EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

			return GUI.Button(position, label, LinkStyle);
		}
	}

}