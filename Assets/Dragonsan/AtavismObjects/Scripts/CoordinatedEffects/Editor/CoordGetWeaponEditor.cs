using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Atavism
{
    [CustomEditor(typeof(CoordGetWeapon))]
    public class CoordGetWeaponEditor : Editor
    {

        bool help = false;
        GUIContent[] slots;
        public override void OnInspectorGUI()
        {
          //  var indentOffset = EditorGUI.indentLevel * 5f;
            var lineRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(lineRect.x, lineRect.y, EditorGUIUtility.currentViewWidth - 60f, lineRect.height);
            var fieldRect = new Rect(labelRect.xMax, lineRect.y, lineRect.width - labelRect.width - 60f, lineRect.height);
            var buttonRect = new Rect(fieldRect.xMax, lineRect.y, 60f, lineRect.height);
            CoordGetWeapon obj = target as CoordGetWeapon;

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
            GUILayout.BeginVertical("Get Weapon Configuration", boxStyle);
            GUILayout.Space(20);
            
            if(slots==null)
                slots =  ServerItems.LoadSlotsOptions(false,true);
            
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

            content = new GUIContent("Weapon Rest Time");
            content.tooltip = "Time afte which weapon will be moved to the rest slot";
            obj.restTime = EditorGUILayout.FloatField(content, obj.restTime);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            
            content = new GUIContent("Move All Weapons To Rest Slots");
            content.tooltip = "Move All Weapons To Rest Slots";
            obj.moveAllWeaponSlotToRest = EditorGUILayout.Toggle(content, obj.moveAllWeaponSlotToRest);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Move From Main Slot To Rest Slot");
            content.tooltip = "Select the slot from which the object will be transferred to the rest slot";
            int ii=0;
            int j = 0;
            foreach (GUIContent c in slots)
            {
                if (c.text.Equals( obj.mainSlotMoveToRest))
                    ii = j;
                j++;
            }
            ii = EditorGUILayout.Popup(content,ii,slots);
            obj.mainSlotMoveToRest = slots[ii].text;
            //   obj.targetSlot = (AttachmentSocket)EditorGUILayout.EnumPopup(content, obj.targetSlot);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            
            content = new GUIContent("Move From Rest Slot To Main Slot");
            content.tooltip = "Select the slot from which the object will be transferred to the Main slot";
             ii=0;
             j = 0;
            foreach (GUIContent c in slots)
            {
                if (c.text.Equals( obj.slotMoveFromRestToMain))
                    ii = j;
                j++;
            }
            ii = EditorGUILayout.Popup(content,ii,slots);
            obj.slotMoveFromRestToMain = slots[ii].text;
            //   obj.targetSlot = (AttachmentSocket)EditorGUILayout.EnumPopup(content, obj.targetSlot);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


            
            
            
        /*    content = new GUIContent("Show Trail");
            content.tooltip = "Show Trail";
            obj.showTrail = EditorGUILayout.Toggle(content, obj.showTrail);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.showTrail)
            {
                content = new GUIContent("Hide Trail");
                content.tooltip = "Move Character";
                obj.hideTrail = EditorGUILayout.Toggle(content, obj.hideTrail);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                if (obj.hideTrail)
                {
                    content = new GUIContent("Hide Time");
                    content.tooltip = "Time for lock";
                    obj.restTime = EditorGUILayout.FloatField(content, obj.restTime);
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                }
            }*/

            /*  content = new GUIContent("DucTrail");
              content.tooltip = "DucTrail";
              obj.DucTrail = EditorGUILayout.Toggle(content, obj.DucTrail);
              if (help)
                  EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
  */
            content = new GUIContent("Caster Look At Target");
            content.tooltip = "Rotate Caster to Target";
            obj.lookAtTarget = EditorGUILayout.Toggle(content, obj.lookAtTarget);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

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