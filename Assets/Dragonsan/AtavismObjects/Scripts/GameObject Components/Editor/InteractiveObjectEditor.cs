using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Atavism
{
    [CustomEditor(typeof(InteractiveObject))]
    public class InteractiveObjectEditor : Editor
    {

        private bool effectsLoaded = false;
        private bool questsLoaded = false;
        private bool tasksLoaded = false;
        private bool instancesLoaded = false;
        string[] interactionTypes;
        bool help = false;

        public override void OnInspectorGUI()
        {
            InteractiveObject obj = target as InteractiveObject;
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
            GUILayout.BeginVertical("Atavism Interactive Object Configuration", topStyle);
            GUILayout.Space(20);
            //EditorGUILayout.LabelField("ID: " + obj.id);

             var lineResetRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var buttonResetRect = new Rect(fieldRect.xMax, lineResetRect.y, 60f, lineResetRect.height);
            GUIContent reset = new GUIContent("Reset ID");
            reset.tooltip = "Click to reset ID ";
            if (GUI.Button(buttonResetRect, reset, EditorStyles.miniButton))
                obj.id = -1;
            content = new GUIContent("ID");
            content.tooltip = "Id";
            EditorGUI.BeginDisabledGroup(true);
            obj.id = EditorGUILayout.IntField("ID:", obj.id);
            EditorGUI.EndDisabledGroup();
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            // Read in Interaction options from Database and display a drop down list
            if (interactionTypes == null)
            {
                interactionTypes = new string[] { "~ none ~" };
                interactionTypes = ServerOptionChoices.LoadAtavismChoiceOptions("Interaction Type", false);
            }
            if (interactionTypes.Length == 0)
            {
                EditorGUILayout.LabelField("!! Interaction Type is not loaded check database configuration in Atavism Editor !!");
                GUILayout.EndVertical();
                return;
            }
            int selectedOption = GetPositionOfInteraction(obj.interactionType);

            content = new GUIContent("Interaction Type");
            content.tooltip = "Interaction Type";
            selectedOption = EditorGUILayout.Popup(content, selectedOption, interactionTypes);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            obj.interactionType = interactionTypes[selectedOption];

            if (!questsLoaded)
                ServerQuests.LoadQuestOptions(true);
            questsLoaded = true;

            if (obj.interactionType.Contains("ApplyEffect"))
            {
                if (!effectsLoaded)
                    ServerEffects.LoadEffectOptions(true);
                effectsLoaded = true;


                content = new GUIContent("Effect");
                content.tooltip = "Effect";
                obj.interactionID = EditorGUILayout.IntPopup(content, obj.interactionID, ServerEffects.GuiEffectOptions, ServerEffects.effectIds);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }
            else if (obj.interactionType.Contains("Quest"))
            {

                content = new GUIContent("Quest");
                content.tooltip = "Quest";
                obj.interactionID = EditorGUILayout.IntPopup(content, obj.interactionID, ServerQuests.GuiQuestOptions, ServerQuests.questIds);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }
            else if (obj.interactionType.Contains("Task"))
            {
                if (!tasksLoaded)
                    ServerTask.LoadTaskOptions(true);
                tasksLoaded = true;


                content = new GUIContent("Task");
                content.tooltip = "Task";
                obj.interactionID = EditorGUILayout.IntPopup(content, obj.interactionID, ServerTask.GuiTaskOptions, ServerTask.taskIds);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }
            else if (obj.interactionType.Equals("InstancePortal"))
            {
                if (!instancesLoaded)
                    ServerInstances.LoadInstanceOptions(true);
                instancesLoaded = true;


                content = new GUIContent("Instance");
                content.tooltip = "Instance";
                obj.interactionID = EditorGUILayout.IntPopup(content, obj.interactionID, ServerInstances.GuiInstanceList, ServerInstances.instanceIds);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                // Need to get a location to teleport to
                Vector3 position = new Vector3();
                float.TryParse(obj.interactionData1, out position.x);
                float.TryParse(obj.interactionData2, out position.y);
                float.TryParse(obj.interactionData3, out position.z);

                content = new GUIContent("Position");
                content.tooltip = "Position";
                position = EditorGUILayout.Vector3Field(content, position);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                obj.interactionData1 = position.x.ToString();
                obj.interactionData2 = position.y.ToString();
                obj.interactionData3 = position.z.ToString();

                content = new GUIContent("Min Level");
                content.tooltip = "Min Level";
                obj.minLevel = EditorGUILayout.IntField(content, obj.minLevel);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                content = new GUIContent("Max Level");
                content.tooltip = "Max Level";
                obj.maxLevel = EditorGUILayout.IntField(content, obj.maxLevel);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }
            else if (obj.interactionType.Contains("CoordEffect"))
            {

                content = new GUIContent("Coord Effects");
                content.tooltip = "Coord Effects";
                EditorGUILayout.LabelField(content);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                if (obj.activateCoordEffects == null)
                    obj.activateCoordEffects = new System.Collections.Generic.List<GameObject>();
                //obj.activateCoordEffects = (GameObject) EditorGUILayout.ObjectField("Coord Effect:", obj.activateCoordEffect, typeof(GameObject), false);
                for (int i = 0; i < obj.activateCoordEffects.Count; i++)
                {
                    obj.activateCoordEffects[i] = (GameObject)EditorGUILayout.ObjectField(obj.activateCoordEffects[i], typeof(GameObject), false);
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add"))
                {
                    obj.activateCoordEffects.Add(null);
                }
                if (GUILayout.Button("Remove"))
                {
                    obj.activateCoordEffects.RemoveAt(obj.activateCoordEffects.Count - 1);
                }
                EditorGUILayout.EndHorizontal();
            }
            else if (obj.interactionType.Contains("CraftingStation"))
            {

            }

                // Quest Required ID

                content = new GUIContent("Quest Required");
            content.tooltip = "Quest Required";
            obj.questReqID = EditorGUILayout.IntPopup(content, obj.questReqID, ServerQuests.GuiQuestOptions, ServerQuests.questIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            // Coord Effect

            content = new GUIContent("Interact Coord Effect");
            content.tooltip = "Interact Coord Effect";
            obj.interactCoordEffect = (GameObject)EditorGUILayout.ObjectField(content, obj.interactCoordEffect, typeof(GameObject), false);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            // Interaction Time

            content = new GUIContent("Interaction Duration");
            content.tooltip = "Interaction Duration";
            obj.interactTimeReq = EditorGUILayout.FloatField(content, obj.interactTimeReq);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            // Respawn Time

            content = new GUIContent("Respawn Time");
            content.tooltip = "Respawn Time";
            obj.refreshDuration = EditorGUILayout.IntField(content, obj.refreshDuration);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            // Cursor Icon

            content = new GUIContent("Mouse Over Cursor");
            content.tooltip = "Mouse Over Cursor";
            obj.cursorIcon = (Texture2D)EditorGUILayout.ObjectField(content, obj.cursorIcon, typeof(Texture2D), false);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            // highlight colour
            content = new GUIContent("Highlight");
            content.tooltip = "Turn On/Off Highlight ";
            obj.highlight = EditorGUILayout.Toggle(content, obj.highlight);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.highlight)
            {
                content = new GUIContent("Highlight Colour");
                content.tooltip = "Highlight Colour";
                obj.highlightColour = EditorGUILayout.ColorField(content, obj.highlightColour);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }
            GUILayout.EndVertical();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(obj);
                EditorUtility.SetDirty(target);
            }
        }

        private int GetPositionOfInteraction(string interactionType)
        {
            for (int i = 0; i < interactionTypes.Length; i++)
            {
                if (interactionTypes[i] == interactionType)
                    return i;
            }
            return 0;
        }
    }
}