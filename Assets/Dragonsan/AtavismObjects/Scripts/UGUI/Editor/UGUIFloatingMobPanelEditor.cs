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
   [CustomEditor(typeof(UGUIFloatingMobPanel))]
    public class UGUIFloatingMobPanelEditor : Editor
    {
        bool help = false;
        GUIContent[] damageTypes;
         public override void OnInspectorGUI()
        {
            UGUIFloatingMobPanel obj = target as UGUIFloatingMobPanel;
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
            GUILayout.BeginVertical("Atavism Floating Mob Panel Configuration", topStyle);
            GUILayout.Space(20);
            //EditorGUILayout.LabelField("ID: " + obj.id);
            GUILayout.BeginVertical("", boxStyle);
           // GUILayout.Space(20);

            content = new GUIContent("Name Text");
            content.tooltip = "Select Name UI element to attach";
            obj.nameText = (Text)EditorGUILayout.ObjectField(content, obj.nameText, typeof(Text), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("TMP Name");
            content.tooltip = "Select TMP Name UI element to attach";
            obj.nameTextTMP = (TextMeshProUGUI)EditorGUILayout.ObjectField(content, obj.nameTextTMP, typeof(TextMeshProUGUI), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


            content = new GUIContent("Tag Text");
            content.tooltip = "Select Level UI element to attach";
            obj.tagText = (Text)EditorGUILayout.ObjectField(content, obj.tagText, typeof(Text), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("TMP Tag Text");
            content.tooltip = "Select TMP tag text UI element to attach";
            obj.tagTextTMP = (TextMeshProUGUI)EditorGUILayout.ObjectField(content, obj.tagTextTMP, typeof(TextMeshProUGUI), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
          
            
            content = new GUIContent("Level Text");
            content.tooltip = "Select Level UI element to attach";
            obj.levelText = (Text)EditorGUILayout.ObjectField(content, obj.levelText, typeof(Text), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

           /* content = new GUIContent("TMP Level");
            content.tooltip = "Select TMP Level UI element to attach";
            obj.TMPLevelText = (TextMeshProUGUI)EditorGUILayout.ObjectField(content, obj.TMPLevelText, typeof(TextMeshProUGUI), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
*/
            content = new GUIContent("combat Text");
            content.tooltip = "Select combat text UI element to attach";
            obj.combatText = (Text)EditorGUILayout.ObjectField(content, obj.combatText, typeof(Text), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("TMP Level");
            content.tooltip = "Select TMP Level UI element to attach";
            obj.combatTextTMP = (TextMeshProUGUI)EditorGUILayout.ObjectField(content, obj.combatTextTMP, typeof(TextMeshProUGUI), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

         //fonts
            content = new GUIContent("Combat Text Font");
            content.tooltip ="Select Font Asset to attach";
            obj.CombatTextFont = (TMP_FontAsset)EditorGUILayout.ObjectField(content, obj.CombatTextFont, typeof(TMP_FontAsset), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Message Font");
            content.tooltip = "Select Font Asset to attach";
            obj.MessageFont = (TMP_FontAsset)EditorGUILayout.ObjectField(content, obj.MessageFont, typeof(TMP_FontAsset), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Xp Font");
            content.tooltip = "Select Font Asset to attach";
            obj.XPFont = (TMP_FontAsset)EditorGUILayout.ObjectField(content, obj.XPFont, typeof(TMP_FontAsset), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Combat Heal Font");
            content.tooltip = "Select Font Asset to attach";
            obj.CombatHealFont = (TMP_FontAsset)EditorGUILayout.ObjectField(content, obj.CombatHealFont, typeof(TMP_FontAsset), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Combat Critical Heal Font");
            content.tooltip = "Select Font Asset to attach";
            obj.CombatHealCritFont = (TMP_FontAsset)EditorGUILayout.ObjectField(content, obj.CombatHealCritFont, typeof(TMP_FontAsset), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Buff Gained Font");
            content.tooltip = "Select Font Asset to attach";;
            obj.BuffGainedFont = (TMP_FontAsset)EditorGUILayout.ObjectField(content, obj.BuffGainedFont, typeof(TMP_FontAsset), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Debuff Gained Font");
            content.tooltip ="Select Font Asset to attach";
            obj.DebuffGainedFont = (TMP_FontAsset)EditorGUILayout.ObjectField(content, obj.DebuffGainedFont, typeof(TMP_FontAsset), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Self Damage Font");
            content.tooltip = "Select Font Asset to attach";
            obj.SelfDamageFont = (TMP_FontAsset)EditorGUILayout.ObjectField(content, obj.SelfDamageFont, typeof(TMP_FontAsset), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Self Damage Critical Font");
            content.tooltip = "Select Font Asset to attach";
            obj.SelfDamageCritFont = (TMP_FontAsset)EditorGUILayout.ObjectField(content, obj.SelfDamageCritFont, typeof(TMP_FontAsset), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            
            content = new GUIContent("Enemy Damage Critic Font");
            content.tooltip = "Select Font Asset to attach";
            obj.EnemyDamageCritFont = (TMP_FontAsset)EditorGUILayout.ObjectField(content, obj.EnemyDamageCritFont, typeof(TMP_FontAsset), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);



            
            content = new GUIContent("Chat Panel");
            content.tooltip = "Select Description UI element to attach";
            obj.chatPanel = (RectTransform)EditorGUILayout.ObjectField(content, obj.chatPanel, typeof(RectTransform), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            
            content = new GUIContent("Chat Text");
            content.tooltip = "Select chat text UI element to attach";
            obj.chatText = (Text)EditorGUILayout.ObjectField(content, obj.chatText, typeof(Text), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("TMP Description Text");
            content.tooltip = "Select TMP chat text UI element to attach";
            obj.chatTextTMP = (TextMeshProUGUI)EditorGUILayout.ObjectField(content, obj.chatTextTMP, typeof(TextMeshProUGUI), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

         
            
            content = new GUIContent("Using World Space Canvas");
            content.tooltip = "Using World Space Canvas";
            obj.usingWorldSpaceCanvas = EditorGUILayout.Toggle(content, obj.usingWorldSpaceCanvas);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            content = new GUIContent("Free Camera");
            content.tooltip = "Free Camera";
            obj.faceCamera = EditorGUILayout.Toggle(content, obj.faceCamera);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Render Distance");
            content.tooltip = "Render Distance";
            obj.renderDistance = EditorGUILayout.FloatField(content, obj.renderDistance);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            
            content = new GUIContent("Chat Display Time");
            content.tooltip = "Chat Display Time";
            obj.chatDisplayTime = EditorGUILayout.FloatField(content, obj.chatDisplayTime);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Friendly Name Color");
            content.tooltip = "Friendly Name Color";
            obj.friendlyNameColour = EditorGUILayout.ColorField(content, obj.friendlyNameColour);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Friendly Name Color");
            content.tooltip = "Friendly Name Color";
            obj.neutralNameColour = EditorGUILayout.ColorField(content, obj.neutralNameColour);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Friendly Name Color");
            content.tooltip = "Friendly Name Color";
            obj.enemyNameColour = EditorGUILayout.ColorField(content, obj.enemyNameColour);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            
            
            content = new GUIContent("My Message Color");
            content.tooltip = "My Message Color";
            obj.myMessageColour = EditorGUILayout.ColorField(content, obj.myMessageColour);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Target Message Color");
            content.tooltip = "Target Message Color";
            obj.targetMessageColour = EditorGUILayout.ColorField(content, obj.targetMessageColour);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
          
            content = new GUIContent("XP Text Color");
            content.tooltip = "XP Text Color";
            obj.XPTextColor = EditorGUILayout.ColorField(content, obj.XPTextColor);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Heal Color");
            content.tooltip = "Heal Color";
            obj.HealColor = EditorGUILayout.ColorField(content, obj.HealColor);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            
            content = new GUIContent("Critical Heal Color");
            content.tooltip = "Critical Heal Color";
            obj.CriticalHealColor = EditorGUILayout.ColorField(content, obj.CriticalHealColor);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Buff Gained Color");
            content.tooltip = "Buff Gained Color";
            obj.BuffGainedColor = EditorGUILayout.ColorField(content, obj.BuffGainedColor);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Debuff Gained Color");
            content.tooltip = "Debuff Gained Color";
            obj.DebuffGainedColor = EditorGUILayout.ColorField(content, obj.DebuffGainedColor);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


            
            content = new GUIContent("Self Damage Color");
            content.tooltip = "Self Damage Color";
            obj.SelfDamageColor = EditorGUILayout.ColorField(content, obj.SelfDamageColor);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Default Damage Color");
            content.tooltip = "Default Damage Color";
            obj.DamageColor = EditorGUILayout.ColorField(content, obj.DamageColor);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Default Critical Damage Color");
            content.tooltip = "Default Critical Damage Color";
            obj.CriticalDamageColor = EditorGUILayout.ColorField(content, obj.CriticalDamageColor);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            
            damageTypes =  ServerDamage.LoadDamageTypeOptions();

            
            content = new GUIContent("Mob Type Definitions");
            content.tooltip = "";

            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.damageTypeColor == null)
                obj.damageTypeColor = new  List<DamageType>();
            string k = "";
            var keys = obj.damageTypeColor.ToArray();
            foreach (DamageType dt in obj.damageTypeColor)
            {
              //  k = key;
                GUILayout.BeginVertical("", boxStyle);
              //  DamageType dt = obj.damageTypeColor[key];

                content = new GUIContent("Damage Type");
                content.tooltip = "Damage Type";
              //  editingDisplay.stringValue2 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Damage Type")+":", editingDisplay.stringValue2, damageOptions);

              //int i = Array.IndexOf(damageTypes, dt.Type);
              int i=0;
              int j = 0;
              foreach (GUIContent c in damageTypes)
              {
                  if (c.text.Equals(dt.type))
                      i = j;
                  j++;
              }
                i = EditorGUILayout.Popup(content,i,damageTypes);
                dt.type = damageTypes[i].text;
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


                content = new GUIContent("Damage Color");
                content.tooltip = "Default Damage Color";
                dt.damageColor = EditorGUILayout.ColorField(content, dt.damageColor);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                content = new GUIContent("Critical Damage Color");
                content.tooltip = "Critical Damage Color";
                dt.criticDamageColor = EditorGUILayout.ColorField(content, dt.criticDamageColor);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
              /*  if (key != dt.Type)
                    obj.damageTypeColor.Remove(key);
                obj.damageTypeColor[dt.Type] = dt;*/
                GUILayout.EndVertical();
            }


         
          EditorGUILayout.BeginHorizontal();
          if (GUILayout.Button("Add"))
          {
              obj.damageTypeColor.Add(new DamageType());
          }
          if (GUILayout.Button("Remove"))
          {
              if (obj.damageTypeColor.Count > 0)
                  obj.damageTypeColor.RemoveAt(obj.damageTypeColor.Count-1);
          }
          EditorGUILayout.EndHorizontal();
            
            
            content = new GUIContent("Critical Size Up Rate");
            content.tooltip = "Critical Size Up Rate";
            obj.CriticalSizeUpRate = EditorGUILayout.FloatField(content, obj.CriticalSizeUpRate);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Critical Size Down Rate");
            content.tooltip = "Critical Size Down Rate";
            obj.CriticalSizeDownRate = EditorGUILayout.FloatField(content, obj.CriticalSizeDownRate);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Normal Damage Display Time");
            content.tooltip = "Normal Damage Display Time";
            obj.NormalDamageDisplayTime = EditorGUILayout.FloatField(content, obj.NormalDamageDisplayTime);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Critical Damage Display Time");
            content.tooltip = "Critical Damage Display Time";
            obj.CriticalDmgDisplayTime = EditorGUILayout.FloatField(content, obj.CriticalDmgDisplayTime);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


            
            
            
            content = new GUIContent("Initial Alpha Factor");
            content.tooltip = "Initial Alpha Factor";
            obj.initialAlphaFactor = EditorGUILayout.FloatField(content, obj.initialAlphaFactor);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Combat Text Speed");
            content.tooltip = "Combat Text Speed";
            obj.combatTextSpeed = EditorGUILayout.FloatField(content, obj.combatTextSpeed);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Combat Color Speed");
            content.tooltip = "Combat Color Speed";
            obj.combatColorSpeed = EditorGUILayout.FloatField(content, obj.combatColorSpeed);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Combat Text Scale Down Speed");
            content.tooltip = "Combat Text Scale Down Speed";
            obj.combatTextScaleDownSpeed = EditorGUILayout.FloatField(content, obj.combatTextScaleDownSpeed);
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