using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Atavism
{
   //  [CustomEditor(typeof(Region))]
    public class RegionEditor : Editor
    {

    //    private bool effectsLoaded = false;
    //    private bool questsLoaded = false;
//private bool tasksLoaded = false;
   //     private bool instancesLoaded = false;
     //   string[] interactionTypes;
        bool help = false;
   //     int type = 0;
        public override void OnInspectorGUI()
        {
          //  var indentOffset = EditorGUI.indentLevel * 5f;
            var lineRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(lineRect.x, lineRect.y, EditorGUIUtility.currentViewWidth - 60f, lineRect.height);
            var fieldRect = new Rect(labelRect.xMax, lineRect.y, lineRect.width - labelRect.width - 60f, lineRect.height);
            var buttonRect = new Rect(fieldRect.xMax, lineRect.y, 60f, lineRect.height);
            Region obj = target as Region;

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
            GUILayout.BeginVertical("Atavism CoordEffect Configuration", topStyle);
            GUILayout.Space(20);
            GUILayout.BeginVertical("Animation Configuration", boxStyle);
            GUILayout.Space(20);
            /*
                content = new GUIContent("Target");
                content.tooltip = "Select Target";
                obj.target = (CoordinatedEffectTarget)EditorGUILayout.EnumPopup(content, obj.target);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
           
            content = new GUIContent("Activation Delay");
            content.tooltip = "put activation delay";
            obj.activationDelay = EditorGUILayout.FloatField(content, obj.activationDelay);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Duration");
            content.tooltip = "put dutation";
            obj.duration = EditorGUILayout.FloatField(content, obj.duration);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Destroy When Finished");
            content.tooltip = "Destroy this object When Finished";
            obj.destroyWhenFinished = EditorGUILayout.Toggle(content, obj.destroyWhenFinished);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


            content = new GUIContent("Target Lock Move Time");
            content.tooltip = "Time for lock";
            obj.lockMove = EditorGUILayout.FloatField(content, obj.lockMove);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            GUILayout.EndVertical();
            GUILayout.Space(2);

            GUILayout.BeginVertical("Move Player Configuration", boxStyle);
            GUILayout.Space(20);
            content = new GUIContent("Move Character");
            content.tooltip = "Move Character";
            obj.moveCharacter = EditorGUILayout.Toggle(content, obj.moveCharacter);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            if (obj.moveCharacter)
            {
                content = new GUIContent("Activation Move Delay");
                content.tooltip = "Activation Move Delay";
                obj.activationMoveDelay = EditorGUILayout.FloatField(content, obj.activationMoveDelay);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


                content = new GUIContent("Move Speed");
                content.tooltip = "Move Speed";
                obj.moveSpeed = EditorGUILayout.FloatField(content, obj.moveSpeed);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                content = new GUIContent("During Time");
                content.tooltip = "During Time";
                obj.duringTime = EditorGUILayout.FloatField(content, obj.duringTime);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

          
            }

                content = new GUIContent("Caster Jump Character");
                content.tooltip = "Caster Jump Character";
                obj.jumpCharacter = EditorGUILayout.Toggle(content, obj.jumpCharacter);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            if (obj.jumpCharacter)
            {
                content = new GUIContent("Activation Caster Jump Character");
                content.tooltip = "Activation Caster Jump Character";
                obj.activationJumpDelay = EditorGUILayout.FloatField(content, obj.activationJumpDelay);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }*/
            GUILayout.EndVertical();
        
            GUILayout.EndVertical();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(obj);
                EditorUtility.SetDirty(target);
            }
        }
    }
}