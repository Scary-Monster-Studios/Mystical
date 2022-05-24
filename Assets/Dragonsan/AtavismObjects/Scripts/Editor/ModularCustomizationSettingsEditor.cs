using System;
using System.Collections;
using System.Collections.Generic;
using HNGamers.Atavism;
using UnityEditor;
using UnityEngine;

namespace Atavism
{
  
    [CustomEditor(typeof(ModularCustomizationSettings))]
   public class ModularCustomizationSettingsEditor : Editor
   {  
       bool help = false;
       GUIContent[] slots;

       public override void OnInspectorGUI()
       {
           ModularCustomizationSettings obj = target as ModularCustomizationSettings;
           //   var indentOffset = EditorGUI.indentLevel * 5f;
           var lineRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
           var labelRect = new Rect(lineRect.x, lineRect.y, EditorGUIUtility.currentViewWidth - 60f, lineRect.height);
           var fieldRect = new Rect(labelRect.xMax, lineRect.y, lineRect.width - labelRect.width - 60f, lineRect.height);
           var buttonRect = new Rect(fieldRect.xMax, lineRect.y, 60f, lineRect.height);
           GUIContent content = new GUIContent("Help");
           content.tooltip = "Click to show or hide help informations";
           if (GUI.Button(buttonRect, content, EditorStyles.miniButton))
               help = !help;
         
           if(slots==null)
               slots =  ServerItems.LoadSlotsOptions(true, true);

           GUIStyle topStyle = new GUIStyle(GUI.skin.box);
           topStyle.normal.textColor = Color.white;
           topStyle.fontStyle = FontStyle.Bold;
           topStyle.alignment = TextAnchor.UpperLeft;
           GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
           boxStyle.normal.textColor = Color.cyan;
           boxStyle.fontStyle = FontStyle.Bold;
           boxStyle.alignment = TextAnchor.UpperLeft;
           GUILayout.BeginVertical("Atavism Modular Customization Settings", topStyle);
           GUILayout.Space(20);
           
           GUILayout.BeginVertical("Sockets Definitions", boxStyle);
           GUILayout.Space(20);

           
           if (slots == null || slots.Length == 2)
           {
               GUI.color = Color.red;
               GUILayout.Space(20);
               GUILayout.Label("Not found definition for item slots !!!");
               GUI.color = Color.white;
               GUILayout.EndVertical();
               GUILayout.EndVertical();
               return;
           }
           
           
           foreach (AttachmentSocket soc in Enum.GetValues(typeof(AttachmentSocket)))
           {
               bool found = false;
               foreach (var mss in obj.settings)
               {
                   if (mss.socket == soc)
                   {
                       found = true;
                   }
               }
               if (!found)
               {
                   ModularSlotSetting mss = new ModularSlotSetting();
                   mss.socket = soc;
                   obj.settings.Add(mss);
               }
           }

         
           
           for (int s = 0; s < obj.settings.Count; s++)
           {
               
               GUI.backgroundColor = Color.green;
               GUILayout.BeginVertical("", topStyle);
               GUI.backgroundColor = Color.white;
               content = new GUIContent("Socket Type");
               content.tooltip = "Select Slot Name for field";
               EditorGUILayout.LabelField( obj.settings[s].socket.ToString());
               if (help)
                   EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

               content = new GUIContent("Slot Name");
               content.tooltip = "Select Slot Name for field";
               int ii = 0;
               int j = 0;
               foreach (GUIContent c in slots)
               {
                   if (c.text.Equals(obj.settings[s].slot))
                       ii = j;
                   j++;
               }

               ii = EditorGUILayout.Popup(content, ii, slots);
               obj.settings[s].slot = slots[ii].text;
               if (help)
                   EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

               
               
               
             /*  for (int i = 0; i < obj.settings[s].slots.Count; i++)
               {

                   GUILayout.BeginVertical("", boxStyle);
                   //  DamageType dt = obj.damageTypeColor[key];

                   content = new GUIContent("Slot Name");
                   content.tooltip = "Select Slot Name for field";
                   int ii = 0;
                   int j = 0;
                   foreach (GUIContent c in slots)
                   {
                       if (c.text.Equals(obj.settings[s].slots[i]))
                           ii = j;
                       j++;
                   }

                   ii = EditorGUILayout.Popup(content, ii, slots);
                   obj.settings[s].slots[i] = slots[ii].text;
                   if (help)
                       EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                   //  editingDisplay.stringValue2 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Damage Type")+":", editingDisplay.stringValue2, damageOptions);

                   //int i = Array.IndexOf(damageTypes, dt.Type);

                   GUILayout.EndVertical();
                   GUILayout.Space(5);
               }

               EditorGUILayout.BeginHorizontal();
               if (GUILayout.Button("Add Slot"))
               {
                   obj.settings[s].slots.Add("");
               }

               if (GUILayout.Button("Remove Slot"))
               {
                   if (obj.settings[s].slots.Count > 0)
                       obj.settings[s].slots.RemoveAt(obj.settings[s].slots.Count - 1);
               }
               EditorGUILayout.EndHorizontal();*/
               GUILayout.EndVertical();
               GUILayout.Space(5);
           }
         /*  EditorGUILayout.BeginHorizontal();
           if (GUILayout.Button("Add"))
           {
               obj.settings.Add(new ModularSlotSetting());
           }

           if (GUILayout.Button("Remove"))
           {
               if (obj.settings.Count > 0)
                   obj.settings.RemoveAt(obj.settings.Count - 1);
           }

           EditorGUILayout.EndHorizontal();*/
           GUILayout.EndVertical();
           GUILayout.EndVertical();

       }
   }
}