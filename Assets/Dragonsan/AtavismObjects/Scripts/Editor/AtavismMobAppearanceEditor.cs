using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Atavism
{
    [CustomEditor(typeof(AtavismMobAppearance))]
    public class AtavismMobAppearanceEditor : Editor
    {
        bool help = false;
        GUIContent[] slots;

        public override void OnInspectorGUI()
        {
            AtavismMobAppearance obj = target as AtavismMobAppearance;
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
            GUILayout.BeginVertical("Atavism Mob Appearance Configuration", topStyle);
            GUILayout.Space(20);
            //EditorGUILayout.LabelField("ID: " + obj.id);

            content = new GUIContent("Portrait Icon");
            content.tooltip = "Select Icon Sprite element to attach";
            obj.portraitIcon = (Sprite)EditorGUILayout.ObjectField(content, obj.portraitIcon, typeof(Sprite), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            
            content = new GUIContent("Combat Close Delay");
            content.tooltip = "Combat Close Delay";
            obj.combatCloseDelay = EditorGUILayout.FloatField(content, obj.combatCloseDelay);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Unsheathe all weapons without default rest slot in combat");
            content.tooltip = "Unsheathe all weapons without default rest slot in combat";
            obj.unsheatheAllWeaponsNotDefaultRestOnEnterInCombat = EditorGUILayout.Toggle(content, obj.unsheatheAllWeaponsNotDefaultRestOnEnterInCombat);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

           
          if(slots==null)
           slots =  ServerItems.LoadSlotsOptions();
            
            GUILayout.BeginVertical("Sockets Definitions", boxStyle);
            GUILayout.Space(20);
          
            if (slots ==null || slots.Length == 0)
            {
                EditorGUILayout.LabelField("!! Slots is not loaded check database configuration in Old Atavism Editor !!");
                GUILayout.EndVertical();
                GUILayout.EndVertical();
                return;
            }
            
            if (obj.sockets == null)
                obj.sockets = new  List<Transform>();

            if (obj.slots == null)
                obj.slots = new  List<string>();


            for (int i=0;i< obj.slots.Count;i++)
            {
                
                GUILayout.BeginVertical("", boxStyle);
              //  DamageType dt = obj.damageTypeColor[key];

                content = new GUIContent("Slot Name");
                content.tooltip = "Select Slot Name for field";
                
                
                int ii=0;
                int j = 0;
                foreach (GUIContent c in slots)
                {
                    if (c.text.Equals( obj.slots[i]))
                        ii = j;
                    j++;
                }
                ii = EditorGUILayout.Popup(content,ii,slots);
                obj.slots[i] = slots[ii].text;
                
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
              //  editingDisplay.stringValue2 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Damage Type")+":", editingDisplay.stringValue2, damageOptions);

              //int i = Array.IndexOf(damageTypes, dt.Type);
             
              content = new GUIContent("Slot field");
              content.tooltip = "Select Slot field UI element to attach";
              obj.sockets[i] = (Transform) EditorGUILayout.ObjectField(content, obj.sockets[i], typeof(Transform), true);
              if (help)
                  EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
        
              content = new GUIContent("Rest Slot field");
              content.tooltip = "Select Rest Slot field UI element to attach";
              obj.restsockets[i] = (Transform) EditorGUILayout.ObjectField(content, obj.restsockets[i], typeof(Transform), true);
              if (help)
                  EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
              
              GUILayout.EndVertical();
                GUILayout.Space(5);
            }
         
          EditorGUILayout.BeginHorizontal();
          if (GUILayout.Button("Add"))
          {
              obj.slots.Add("");
              obj.sockets.Add(null);
              obj.restsockets.Add(null);
          }
          if (GUILayout.Button("Remove"))
          {
              if (obj.slots.Count > 0)
                  obj.slots.RemoveAt(obj.slots.Count-1);
              if (obj.sockets.Count > 0)
                  obj.sockets.RemoveAt(obj.sockets.Count-1);
              if (obj.restsockets.Count > 0)
                  obj.restsockets.RemoveAt(obj.restsockets.Count-1);
          }
          EditorGUILayout.EndHorizontal();
            
          GUILayout.EndVertical();
            
            
            
            GUILayout.Space(2);

            GUILayout.EndVertical();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(obj);
                EditorUtility.SetDirty(target);
            }
        }

    }
}