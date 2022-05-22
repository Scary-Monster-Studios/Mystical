using UnityEngine;
using UnityEditor;
using System.Collections;


namespace Atavism
{
  //  [CustomEditor(typeof(InstancePortal))]
    public class InstancePortalEditor : Editor
    {

   /**     private bool effectsLoaded = false;
        private bool questsLoaded = false;
        private bool tasksLoaded = false;
        private bool instancesLoaded = false;
        string[] interactionTypes;
        bool help = false;
        int instance = 0;
        public override void OnInspectorGUI()
        {
            InstancePortal obj = target as InstancePortal;
            var indentOffset = EditorGUI.indentLevel * 5f;
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
            GUILayout.BeginVertical("Atavism Instance Portal Configuration", topStyle);
            GUILayout.Space(20);
            //EditorGUILayout.LabelField("ID: " + obj.id);

            content = new GUIContent("Trigger");
            content.tooltip = "Select trigger";
            obj.trigger = (InstancePortal.Trigger)EditorGUILayout.EnumPopup(content, obj.trigger);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


            if (!instancesLoaded)
            {
                ServerInstances.LoadInstanceOptions(true);
                instancesLoaded = true;
            }
            content = new GUIContent("Instance");
            content.tooltip = "Instance";
            obj.instanceID = EditorGUILayout.IntPopup(content, obj.instanceID, ServerInstances.GuiInstanceList, ServerInstances.instanceIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
          
            content = new GUIContent("Spawn Marker Name");
            content.tooltip = "Spawn Marker Name";
            obj.markerName = EditorGUILayout.TextField(content, obj.markerName);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Localization");
                content.tooltip = "Localization";
                obj.loc = EditorGUILayout.Vector3Field(content, obj.loc);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            GUILayout.EndVertical();
        }
    */
       
    }
}