using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Atavism
{
     [CustomEditor(typeof(CoordLockMove))]
    public class CoordLockMoveEditor : Editor
    {

      //  private bool effectsLoaded = false;
      //  private bool questsLoaded = false;
     //   private bool tasksLoaded = false;
     //   private bool instancesLoaded = false;
    //    string[] interactionTypes;
        bool help = false;
     //   int type = 0;
        public override void OnInspectorGUI()
        {
         //   var indentOffset = EditorGUI.indentLevel * 5f;
            var lineRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(lineRect.x, lineRect.y, EditorGUIUtility.currentViewWidth - 60f, lineRect.height);
            var fieldRect = new Rect(labelRect.xMax, lineRect.y, lineRect.width - labelRect.width - 60f, lineRect.height);
            var buttonRect = new Rect(fieldRect.xMax, lineRect.y, 60f, lineRect.height);
            CoordLockMove obj = target as CoordLockMove;

            GUIContent content = new GUIContent("Help");
            content.tooltip = "Click to show or hide help information";
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

                content = new GUIContent("Target");
            content.tooltip = "Select target, you can choose between caster or target values, depending who is invoking this coordinated effect";
            obj.target = (CoordinatedEffectTarget)EditorGUILayout.EnumPopup(content, obj.target);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
           
            content = new GUIContent("Activation Delay");
            content.tooltip = "Define activation delay (in seconds)";
            obj.activationDelay = EditorGUILayout.FloatField(content, obj.activationDelay);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Duration");
            content.tooltip = "Define duration (in seconds)";
            obj.duration = EditorGUILayout.FloatField(content, obj.duration);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Destroy When Finished");
            content.tooltip = "Is this object should be destroyed when its duration time + activation delay finish. In case of multiple Coordinated scripts on the object, keep only one Destroy When Finished ticked";
            obj.destroyWhenFinished = EditorGUILayout.Toggle(content, obj.destroyWhenFinished);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Interruption Can Terminate CoordEffect");
            content.tooltip = "Interruption of the ability should stop this effect and destroy the object";
            obj.interruptCanTerminateCoordEffect = EditorGUILayout.Toggle(content, obj.interruptCanTerminateCoordEffect);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Target Lock Move Time");
            content.tooltip = "For how long the target should be locked (unavailable to move)";
            obj.lockMove = EditorGUILayout.FloatField(content, obj.lockMove);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            GUILayout.EndVertical();
            GUILayout.Space(2);

            GUILayout.BeginVertical("Move Player Configuration", boxStyle);
            GUILayout.Space(20);
            content = new GUIContent("Move Character");
            content.tooltip = "Should target be moved";
            obj.moveCharacter = EditorGUILayout.Toggle(content, obj.moveCharacter);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            if (obj.moveCharacter)
            {
                content = new GUIContent("Activation Move Delay");
                content.tooltip = "Define activation delay for character movement by this coordinated effect (in seconds)";
                obj.activationMoveDelay = EditorGUILayout.FloatField(content, obj.activationMoveDelay);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


                content = new GUIContent("Move Speed");
                content.tooltip = "Define move speed with which the target will be moved with";
                obj.moveSpeed = EditorGUILayout.FloatField(content, obj.moveSpeed);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                content = new GUIContent("During Time");
                content.tooltip = "Define duration how long the target will be moved";
                obj.duringTime = EditorGUILayout.FloatField(content, obj.duringTime);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

          
            }

            content = new GUIContent("Use Jump On Caster");
            content.tooltip = "Define if the Caster uses jump";
            obj.jumpCharacter = EditorGUILayout.Toggle(content, obj.jumpCharacter);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            if (obj.jumpCharacter)
            {
                content = new GUIContent("Jump Activation Delay");
                content.tooltip = "Define activation delay for jump (in seconds)";
                obj.activationJumpDelay = EditorGUILayout.FloatField(content, obj.activationJumpDelay);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }
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