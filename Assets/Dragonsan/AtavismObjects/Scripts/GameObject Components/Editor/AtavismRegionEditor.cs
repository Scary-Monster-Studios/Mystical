using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Atavism
{
    [CustomEditor(typeof(AtavismRegion))]
    public class AtavismRegionEditor : Editor
    {

        private bool effectsLoaded = false;
        private bool questsLoaded = false;
        private bool tasksLoaded = false;
        private bool instancesLoaded = false;
        string[] interactionTypes;
        bool help = false;

        public override void OnInspectorGUI()
        {
          //  var indentOffset = EditorGUI.indentLevel * 5f;
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
            GUILayout.BeginVertical("Atavism Region Configuration", topStyle);
            GUILayout.Space(20);
            AtavismRegion obj = target as AtavismRegion;

            //EditorGUILayout.LabelField("ID: " + obj.id);
            var lineResetRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var buttonResetRect = new Rect(fieldRect.xMax, lineResetRect.y, 60f, lineResetRect.height);
            GUIContent reset = new GUIContent("Reset ID");
            reset.tooltip = "Click to reset ID";
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

            /*int selectedOption = GetPositionOfInteraction(obj.regionType);
            selectedOption = EditorGUILayout.Popup("Region Type:", selectedOption, interactionTypes); 
            obj.regionType = interactionTypes[selectedOption];*/
            content = new GUIContent("Region Type");
            content.tooltip = "Region Type";
            obj.regionType = (AtavismRegionType)EditorGUILayout.EnumPopup(content, obj.regionType);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            if (!questsLoaded)
                ServerQuests.LoadQuestOptions(true);
            questsLoaded = true;

            if (obj.regionType == AtavismRegionType.Water)
            {
                if (!effectsLoaded)
                {
                    ServerEffects.LoadEffectOptions(true);
                    effectsLoaded = true;
                }
                content = new GUIContent("Breath Effect:");
                content.tooltip = "Breath Effect:";
                obj.actionID = EditorGUILayout.IntPopup(content, obj.actionID, ServerEffects.GuiEffectOptions, ServerEffects.effectIds);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }
            else if (obj.regionType == AtavismRegionType.ApplyEffect)
            {
                if (!effectsLoaded)
                {
                    ServerEffects.LoadEffectOptions(true);
                    effectsLoaded = true;
                }
                content = new GUIContent("Effect:");
                content.tooltip = "Effect:";
                obj.actionID = EditorGUILayout.IntPopup(content, obj.actionID, ServerEffects.GuiEffectOptions, ServerEffects.effectIds);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }
            else if (obj.regionType == AtavismRegionType.StartQuest)
            {
                content = new GUIContent("Quest:");
                content.tooltip = "Quest:";
                obj.actionID = EditorGUILayout.IntPopup(content, obj.actionID, ServerQuests.GuiQuestOptions, ServerQuests.questIds);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }
            else if (obj.regionType == AtavismRegionType.CompleteTask)
            {
                if (!tasksLoaded)
                    ServerTask.LoadTaskOptions(true);
                tasksLoaded = true;
                content = new GUIContent("Task:");
                content.tooltip = "Task:";
                obj.actionID = EditorGUILayout.IntPopup(content, obj.actionID, ServerTask.GuiTaskOptions, ServerTask.taskIds);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }
            else if (obj.regionType == AtavismRegionType.Teleport)
            {
                if (!instancesLoaded)
                    ServerInstances.LoadInstanceOptions(true);
                instancesLoaded = true;
                content = new GUIContent("Instance:");
                content.tooltip = "Instance:";
                obj.actionID = EditorGUILayout.IntPopup(content, obj.actionID, ServerInstances.GuiInstanceList, ServerInstances.instanceIds);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                // Need to get a location to teleport to
                Vector3 position = new Vector3();
                float.TryParse(obj.actionData1, out position.x);
                float.TryParse(obj.actionData2, out position.y);
                float.TryParse(obj.actionData3, out position.z);
                content = new GUIContent("Position:");
                content.tooltip = "Position:";
                position = EditorGUILayout.Vector3Field(content, position);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                obj.actionData1 = position.x.ToString();
                obj.actionData2 = position.y.ToString();
                obj.actionData3 = position.z.ToString();
            }

            // Quest Required ID
            //obj.questReqID = EditorGUILayout.IntPopup("Quest Required:", obj.questReqID, ServerQuests.questOptions, ServerQuests.questIds); 

            // Coord Effect
            //obj.interactCoordEffect = (GameObject) EditorGUILayout.ObjectField("Interact Coord Effect:", obj.interactCoordEffect, typeof(GameObject), false);

            // Interaction Time
            //obj.interactTimeReq = EditorGUILayout.FloatField("Interaction Duration:", obj.interactTimeReq);

            // Respawn Time
            //obj.refreshDuration = EditorGUILayout.IntField("Respawn Time:", obj.refreshDuration);

            // Cursor Icon
            //obj.cursorIcon = (Texture2D) EditorGUILayout.ObjectField("Mouse Over Cursor:", obj.cursorIcon, typeof(Texture2D), false);

            // highlight colour
            //obj.highlightColour = EditorGUILayout.ColorField("Highlight Colour:", obj.highlightColour);
            GUILayout.EndVertical();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(obj);
                EditorUtility.SetDirty(target);
            }
        }
    }
}