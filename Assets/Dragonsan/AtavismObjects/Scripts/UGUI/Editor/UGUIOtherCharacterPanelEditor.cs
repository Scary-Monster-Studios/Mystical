using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine.UI;

namespace Atavism
{
   [CustomEditor(typeof(UGUIOtherCharacterPanel))]
    public class UGUIOtherCharacterPanelEditor : Editor
    {
        bool help = false;
        GUIContent[] stats;
        GUIContent[] slots;
         public override void OnInspectorGUI()
        {
            UGUIOtherCharacterPanel obj = target as UGUIOtherCharacterPanel;
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
            GUILayout.BeginVertical("Atavism Other Character Panel Configuration", topStyle);
            GUILayout.Space(20);
            //EditorGUILayout.LabelField("ID: " + obj.id);
            GUILayout.BeginVertical("", boxStyle);
           // GUILayout.Space(20);

            

            content = new GUIContent("Target Name");
            content.tooltip = "Select Target Name UI element to attach";
            obj.targetName = (TextMeshProUGUI)EditorGUILayout.ObjectField(content, obj.targetName, typeof(TextMeshProUGUI), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if(stats==null)
                stats =  ServerStats.LoadStatOptionsForGui();
            
            GUILayout.BeginVertical("Stat Definitions", boxStyle);
            GUILayout.Space(20);
            /*content = new GUIContent("Stat Definitions");
            content.tooltip = "";

            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                */
            if (obj.StatsText == null)
                obj.StatsText = new  List<TextMeshProUGUI>();

            if (obj.StatsName == null)
                obj.StatsName = new  List<string>();


            for (int i=0;i< obj.StatsName.Count;i++)
            {
                
                GUILayout.BeginVertical("", boxStyle);
              //  DamageType dt = obj.damageTypeColor[key];

                content = new GUIContent("Stat");
                content.tooltip = "Select Stat for field";
                
                
                int ii=0;
                int j = 0;
                foreach (GUIContent c in stats)
                {
                    if (c.text.Equals( obj.StatsName[i]))
                        ii = j;
                    j++;
                }
                ii = EditorGUILayout.Popup(content,ii,stats);
                obj.StatsName[i] = stats[ii].text;
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
              //  editingDisplay.stringValue2 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Damage Type")+":", editingDisplay.stringValue2, damageOptions);

              //int i = Array.IndexOf(damageTypes, dt.Type);
             
              content = new GUIContent("Stat field");
              content.tooltip = "Select Stat field UI element to attach";
              obj.StatsText[i] = (TextMeshProUGUI)EditorGUILayout.ObjectField(content, obj.StatsText[i], typeof(TextMeshProUGUI), true);
              if (help)
                  EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                
                GUILayout.EndVertical();
                GUILayout.Space(5);
            }


         
          EditorGUILayout.BeginHorizontal();
          if (GUILayout.Button("Add"))
          {
              obj.StatsName.Add("");
              obj.StatsText.Add(null);
          }
          if (GUILayout.Button("Remove"))
          {
              if (obj.StatsName.Count > 0)
                  obj.StatsName.RemoveAt(obj.StatsName.Count-1);
              if (obj.StatsText.Count > 0)
                  obj.StatsText.RemoveAt(obj.StatsText.Count-1);
          }
          EditorGUILayout.EndHorizontal();
            
          GUILayout.EndVertical();

        
          if(slots==null)
           slots =  ServerItems.LoadSlotsOptions();
            
            GUILayout.BeginVertical("Slots Definitions", boxStyle);
            GUILayout.Space(20);
          
            if (obj.slots == null)
                obj.slots = new  List<UGUIItemDisplay>();

            if (obj.slotName == null)
                obj.slotName = new  List<string>();


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
                    if (c.text.Equals( obj.slotName[i]))
                        ii = j;
                    j++;
                }
                ii = EditorGUILayout.Popup(content,ii,slots);
                obj.slotName[i] = slots[ii].text;
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
              //  editingDisplay.stringValue2 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Damage Type")+":", editingDisplay.stringValue2, damageOptions);

              //int i = Array.IndexOf(damageTypes, dt.Type);
             
              content = new GUIContent("Slot field");
              content.tooltip = "Select Slot field UI element to attach";
              obj.slots[i] = (UGUIItemDisplay) EditorGUILayout.ObjectField(content, obj.slots[i], typeof(UGUIItemDisplay), true);
              if (help)
                  EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                GUILayout.EndVertical();
                GUILayout.Space(5);
            }
         
          EditorGUILayout.BeginHorizontal();
          if (GUILayout.Button("Add"))
          {
              obj.slotName.Add("");
              obj.slots.Add(null);
          }
          if (GUILayout.Button("Remove"))
          {
              if (obj.slots.Count > 0)
                  obj.slots.RemoveAt(obj.slots.Count-1);
              if (obj.slotName.Count > 0)
                  obj.slotName.RemoveAt(obj.slotName.Count-1);
          }
          EditorGUILayout.EndHorizontal();
            
          GUILayout.EndVertical();
          
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