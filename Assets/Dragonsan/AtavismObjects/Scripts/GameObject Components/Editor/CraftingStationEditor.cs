using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Atavism
{
    [CustomEditor(typeof(CraftingStation))]
    public class CraftingStationEditor : Editor
    {

        //private bool effectsLoaded = false;
       // private bool questsLoaded = false;
       // private bool tasksLoaded = false;
       // private bool instancesLoaded = false;
        string[] stationType;
        bool help = false;

        public override void OnInspectorGUI()
        {
          //  var indentOffset = EditorGUI.indentLevel * 5f;
            var lineRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(lineRect.x, lineRect.y, EditorGUIUtility.currentViewWidth - 60f, lineRect.height);
            var fieldRect = new Rect(labelRect.xMax, lineRect.y, lineRect.width - labelRect.width - 60f, lineRect.height);
            var buttonRect = new Rect(fieldRect.xMax, lineRect.y, 60f, lineRect.height);

            CraftingStation obj = target as CraftingStation;
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
            GUILayout.BeginVertical("Atavism Craft Station Configuration", topStyle);
            GUILayout.Space(20);
            if (stationType == null)
            {
                stationType = new string[] { "none" };
                stationType = ServerOptionChoices.LoadAtavismChoiceOptions("Crafting Station", true);
            }
            int selectedIndex = System.Array.IndexOf(stationType, obj.stationType);
            if (selectedIndex == -1) selectedIndex = 0;

            content = new GUIContent("Stattion Type");
            content.tooltip = "Stattion Type";
            selectedIndex = EditorGUILayout.Popup(content, selectedIndex, stationType);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            obj.stationType = stationType[selectedIndex];

        /*    content = new GUIContent("Icon");
            content.tooltip = "Select Icon";
            obj.icon = (Sprite)EditorGUILayout.ObjectField(content, obj.icon, typeof(Sprite), false);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                */
            content = new GUIContent("Cursor Icon");
            content.tooltip = "Select Cursor Icon";
            obj.cursorIcon = (Texture2D)EditorGUILayout.ObjectField(content, obj.cursorIcon, typeof(Texture2D), false);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Coord Effect");
            content.tooltip = "Select Coord Effect";
            obj.coordEffect = (CoordinatedEffect)EditorGUILayout.ObjectField(content, obj.coordEffect, typeof(CoordinatedEffect), false);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            GUILayout.EndVertical();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(obj);
                EditorUtility.SetDirty(target);
            }
        }
    }
}