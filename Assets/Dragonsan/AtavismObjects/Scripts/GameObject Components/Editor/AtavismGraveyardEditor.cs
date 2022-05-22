using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Atavism
{
    [CustomEditor(typeof(AtavismGraveyard))]
    public class AtavismGraveyardEditor : Editor
    {

        private bool factionLoaded = false;
        bool help = false;


        public override void OnInspectorGUI()
        {
          //  var indentOffset = EditorGUI.indentLevel * 5f;
            var lineRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(lineRect.x, lineRect.y, EditorGUIUtility.currentViewWidth - 60f, lineRect.height);
            var fieldRect = new Rect(labelRect.xMax, lineRect.y, lineRect.width - labelRect.width - 60f, lineRect.height);
            var buttonRect = new Rect(fieldRect.xMax, lineRect.y, 60f, lineRect.height);
            AtavismGraveyard obj = target as AtavismGraveyard;

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
            GUILayout.BeginVertical("Atavism Graveyard Configuration", topStyle);
            GUILayout.Space(20);

            content = new GUIContent("ID");
            content.tooltip = "Unique Id of Graveyard";
            var lineResetRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var buttonResetRect = new Rect(fieldRect.xMax, lineResetRect.y, 60f, lineResetRect.height);
            GUIContent reset = new GUIContent("Reset ID");
            reset.tooltip = "Click to reset ID";
            if (GUI.Button(buttonResetRect, reset, EditorStyles.miniButton))
                obj.id = -1;
            EditorGUI.BeginDisabledGroup(true);
            obj.id = EditorGUILayout.IntField("ID:", obj.id);
            EditorGUI.EndDisabledGroup();
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Faction");
            content.tooltip = "Select Faction";
            if (!factionLoaded)
            {
                ServerFactions.LoadFactionOptions(true);
                factionLoaded = true;
            }
            obj.factionID = EditorGUILayout.IntPopup(content, obj.factionID, ServerFactions.GuiFactionOptions, ServerFactions.factionIds);

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