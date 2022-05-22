using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Atavism
{
    [CustomEditor(typeof(ModularCharacterDnaPanel))]
    public class ModularCharacterDnaPanelEditor : Editor
    {
        public string[] parts = new[] {"Hair", "Beard", "Face", "Eyebrow", "Scar", "Hands", "LowerArms", "UpperArms", "Torso", "Hips", "LowerLegs", "Eyes", "Mouth", "Head", "Feet"};
        public string[] partsColor = new[] {"HairColor", "StubbleColor", "SkinColor", "EyeColor", "ScarColor", "BodyArtColor", "BeardColor", "EyebrowColor", "MouthColor"};
        bool help = false;

        public override void OnInspectorGUI()
        {
            ModularCharacterDnaPanel obj = target as ModularCharacterDnaPanel;
            //   var indentOffset = EditorGUI.indentLevel * 5f;
            var lineRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(lineRect.x, lineRect.y, EditorGUIUtility.currentViewWidth - 60f, lineRect.height);
            var fieldRect = new Rect(labelRect.xMax, lineRect.y, lineRect.width - labelRect.width - 60f, lineRect.height);
            var buttonRect = new Rect(fieldRect.xMax, lineRect.y, 60f, lineRect.height);
            GUIContent content = new GUIContent("Help");
            content.tooltip = "Click to show or hide help informations";
            if (GUI.Button(buttonRect, content, EditorStyles.miniButton))
                help = !help;
            GUIStyle topStyle = new GUIStyle(GUI.skin.box);
            topStyle.normal.textColor = Color.white;
            topStyle.fontStyle = FontStyle.Bold;
            topStyle.alignment = TextAnchor.UpperLeft;
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.textColor = Color.cyan;
            boxStyle.fontStyle = FontStyle.Bold;
            boxStyle.alignment = TextAnchor.UpperLeft;
            GUILayout.BeginVertical("Atavism Non UMA Modular Character Panel Configuration", topStyle);
            GUILayout.Space(20);
            content = new GUIContent("Slider Prefab");
            content.tooltip = "Select Slider Prefab";
            obj.dnaSliderPrefab = (GameObject) EditorGUILayout.ObjectField(content, obj.dnaSliderPrefab,typeof(GameObject), true);
            content = new GUIContent("Color Prefab");
            content.tooltip = "Select Color Prefab";
            obj.dnaColorPrefab = (GameObject) EditorGUILayout.ObjectField(content, obj.dnaColorPrefab,typeof(GameObject), true);
            content = new GUIContent("Picker Prefab");
            content.tooltip = "Select Picker Prefab";
            obj.ColorPickerPrefab = (GameObject) EditorGUILayout.ObjectField(content, obj.ColorPickerPrefab,typeof(GameObject), true);

            content = new GUIContent("Picker Location Transform");
            content.tooltip = "Select  Location Transform";
            obj.pickerLoc = (RectTransform) EditorGUILayout.ObjectField(content, obj.pickerLoc,typeof(RectTransform), true);

            
            GUILayout.BeginVertical("Sliders Definitions", boxStyle);
            GUILayout.Space(20);
            for (int r = 0; r < obj.dnaSliders.Count; r++)
            {
                GUI.backgroundColor = Color.blue;
                GUILayout.BeginVertical("", topStyle);
                GUI.backgroundColor = Color.white;
                content = new GUIContent("Slider Type");
                content.tooltip = "Select Slider function";
                int ii = 0;
                int j = 0;
                foreach (string c in parts)
                {
                    if (c.Equals(obj.dnaSliders[r]))
                        ii = j;
                    j++;
                }

                ii = EditorGUILayout.Popup(content, ii, parts);
                obj.dnaSliders[r] = parts[ii];
                content = new GUIContent("Display Name");
                content.tooltip = "Set Display Name";
                obj.dnaSlidersNames[r] = EditorGUILayout.TextField(content, obj.dnaSlidersNames[r]);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                if (GUILayout.Button("Remove Slider"))
                {
                    if (obj.dnaSlidersNames.Count > 0)
                        obj.dnaSlidersNames.RemoveAt(r);
                    if (obj.dnaSliders.Count > 0)
                        obj.dnaSliders.RemoveAt(r);
                }
                EditorGUILayout.EndVertical();
                GUILayout.Space(5);
            }
            if (GUILayout.Button("Add Slider"))
                {
                    obj.dnaSlidersNames.Add("");
                    obj.dnaSliders.Add("");
                }
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
            GUILayout.BeginVertical("Colors Definitions", boxStyle);
            GUILayout.Space(20);
            for (int r = 0; r < obj.dnaColors.Count; r++)
            {
                GUI.backgroundColor = Color.red;
                GUILayout.BeginVertical("", topStyle);
                GUI.backgroundColor = Color.white;
                content = new GUIContent("Color Type");
                content.tooltip = "Select Color function";
                int ii = 0;
                int j = 0;
                foreach (string c in partsColor)
                {
                    if (c.Equals(obj.dnaColors[r]))
                        ii = j;
                    j++;
                }

                ii = EditorGUILayout.Popup(content, ii, partsColor);
                obj.dnaColors[r] = partsColor[ii];
                content = new GUIContent("Display Name");
                content.tooltip = "Set Display Name";
                obj.dnaColorNames[r] = EditorGUILayout.TextField(content, obj.dnaColorNames[r]);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                if (GUILayout.Button("Remove Color"))
                {
                    if (obj.dnaColorNames.Count > 0)
                        obj.dnaColorNames.RemoveAt(r);
                    if (obj.dnaColors.Count > 0)
                        obj.dnaColors.RemoveAt(r);
                }
                EditorGUILayout.EndVertical();
                GUILayout.Space(5);
            }
            if (GUILayout.Button("Add Color"))
            {
                obj.dnaColorNames.Add("");
                obj.dnaColors.Add("");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(obj);
                EditorUtility.SetDirty(target);
            }
        }
    }
}