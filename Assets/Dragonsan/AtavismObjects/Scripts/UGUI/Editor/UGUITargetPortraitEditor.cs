using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Atavism
{
    [CustomEditor(typeof(UGUITargetPortrait))]
    public class UGUITargetPortraitEditor : Editor
    {
        bool help = false;
        string[] mobTypes;
        int[] mobTypesIds;

        public override void OnInspectorGUI()
        {
            UGUITargetPortrait obj = target as UGUITargetPortrait;
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
            GUILayout.BeginVertical("Atavism Target Portrait Configuration", topStyle);
            GUILayout.Space(20);
            //EditorGUILayout.LabelField("ID: " + obj.id);
            GUILayout.BeginVertical("", boxStyle);
           // GUILayout.Space(20);

            content = new GUIContent("Name");
            content.tooltip = "Select Name UI element to attach";
            obj.name = (Text)EditorGUILayout.ObjectField(content, obj.name, typeof(Text), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("TMP Name");
            content.tooltip = "Select TMP Name UI element to attach";
            obj.TMPName = (TextMeshProUGUI)EditorGUILayout.ObjectField(content, obj.TMPName, typeof(TextMeshProUGUI), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


            content = new GUIContent("Level");
            content.tooltip = "Select Level UI element to attach";
            obj.levelText = (Text)EditorGUILayout.ObjectField(content, obj.levelText, typeof(Text), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("TMP Level");
            content.tooltip = "Select TMP Level UI element to attach";
            obj.TMPLevelText = (TextMeshProUGUI)EditorGUILayout.ObjectField(content, obj.TMPLevelText, typeof(TextMeshProUGUI), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("TMP Description Text");
            content.tooltip = "Select Description UI element to attach";
            obj.TMPDescriptionText = (TextMeshProUGUI)EditorGUILayout.ObjectField(content, obj.TMPDescriptionText, typeof(TextMeshProUGUI), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("TMP Species Text");
            content.tooltip = "Select Species UI element to attach";
            obj.TMPSpeciesText = (TextMeshProUGUI)EditorGUILayout.ObjectField(content, obj.TMPSpeciesText, typeof(TextMeshProUGUI), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Portrait");
            content.tooltip = "Select Portrait UI element to attach";
            obj.portrait = (Image)EditorGUILayout.ObjectField(content, obj.portrait, typeof(Image), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Aggro Image");
            content.tooltip = "Select Aggro Image UI element to attach";
            obj.agroImage= (Image)EditorGUILayout.ObjectField(content, obj.agroImage, typeof(Image), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            GUILayout.EndVertical();

            GUILayout.BeginVertical("Popup Configuration", boxStyle);
            GUILayout.Space(20);

            content = new GUIContent("Show Popup On Right Click");
            content.tooltip = "Select to show popup on right click";
            obj.showPopupOnRightClick = EditorGUILayout.Toggle(content, obj.showPopupOnRightClick);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Menu Popup button");
            content.tooltip = "Select Button UI element to attach";
            obj.menuPopupButton = (Button)EditorGUILayout.ObjectField(content, obj.menuPopupButton, typeof(Button), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Menu Popup");
            content.tooltip = "Select menu object to attach";
            obj.popupMenu = (RectTransform)EditorGUILayout.ObjectField(content, obj.popupMenu, typeof(RectTransform), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);



            content = new GUIContent("Whisper button");
            content.tooltip = "Select Whisper button";
            obj.WhisperButton = (Button)EditorGUILayout.ObjectField(content, obj.WhisperButton, typeof(Button), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Invite Group button");
            content.tooltip = "Select Invite Group button";
            obj.InviteGroupButton = (Button)EditorGUILayout.ObjectField(content, obj.InviteGroupButton, typeof(Button), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Trade button");
            content.tooltip = "Select Trade button";
            obj.TradeButton = (Button)EditorGUILayout.ObjectField(content, obj.TradeButton, typeof(Button), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Duel button");
            content.tooltip = "Select Duel button";
            obj.DuelButton = (Button)EditorGUILayout.ObjectField(content, obj.DuelButton, typeof(Button), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Info button");
            content.tooltip = "Select Info button";
            obj.InfoButton = (Button)EditorGUILayout.ObjectField(content, obj.InfoButton, typeof(Button), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Pet Despawn button");
            content.tooltip = "Select Pet Despawn button";
            obj.petDespawnButton = (Button)EditorGUILayout.ObjectField(content, obj.petDespawnButton, typeof(Button), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Friend button");
            content.tooltip = "Select Friend button";
            obj.friendButton = (Button)EditorGUILayout.ObjectField(content, obj.friendButton, typeof(Button), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Guild button");
            content.tooltip = "Select Guild button";
            obj.guildButton = (Button)EditorGUILayout.ObjectField(content, obj.guildButton, typeof(Button), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                
            content = new GUIContent("Build Help button");
            content.tooltip = "Select Build Help button";
            obj.buildHelpButton = (Button)EditorGUILayout.ObjectField(content, obj.buildHelpButton, typeof(Button), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
          
            content = new GUIContent("Build Repair button");
            content.tooltip = "Select Build Repair button";
            obj.buildRepairButton = (Button)EditorGUILayout.ObjectField(content, obj.buildRepairButton, typeof(Button), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            GUILayout.EndVertical();
            GUILayout.BeginVertical("Mob Type Configuration", boxStyle);
            GUILayout.Space(20);
            content = new GUIContent("Mob Type add Marks");
            content.tooltip = "Select to show stars next to the target name";
            obj.mobTypeAddStars = EditorGUILayout.Toggle(content, obj.mobTypeAddStars);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Mob Type Image");
            content.tooltip = "Select mob type image UI element to attach";
            obj.mobTypeImage = (Image)EditorGUILayout.ObjectField(content, obj.mobTypeImage, typeof(Image), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

           
            if (mobTypes == null)
            {
              //  mobTypes = new string[] { "~ none ~" };
                ServerOptionChoices.LoadAtavismChoiceOptions("Mob Type", false, out mobTypesIds,out mobTypes);
            }
            if (mobTypes.Length == 0)
            {
                EditorGUILayout.LabelField("!! Mob Type is not loaded check database configuration in Atavism Editor !!");
                GUILayout.EndVertical();
                GUILayout.EndVertical();
                return;
            }
            content = new GUIContent("Mob Type Definitions");
            content.tooltip = "";

            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.mobtypeDefinition == null)
                obj.mobtypeDefinition = new System.Collections.Generic.List<TargetMobTyp>();
            for (int i = 0; i < obj.mobtypeDefinition.Count; i++)
            {
                GUILayout.BeginVertical("", boxStyle);

                content = new GUIContent("Mob Type");
                content.tooltip = "Select Mob Type";
                int selectedClass = GetOptionPosition(obj.mobtypeDefinition[i].Mobtype, mobTypesIds);
                selectedClass = EditorGUILayout.Popup(content, selectedClass, mobTypes);
                obj.mobtypeDefinition[i].Mobtype = mobTypesIds[selectedClass];
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                content = new GUIContent("Mark Text");
                content.tooltip = "Set mark that will appear next to the target name";
                obj.mobtypeDefinition[i].mark = EditorGUILayout.TextField(content, obj.mobtypeDefinition[i].mark);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                content = new GUIContent("Target Type Image");
                content.tooltip = "Select mob type image UI element to attach";
                obj.mobtypeDefinition[i].additionalImage = (Sprite)EditorGUILayout.ObjectField(content, obj.mobtypeDefinition[i].additionalImage, typeof(Sprite), true);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
               

              
                GUILayout.EndVertical();
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.mobtypeDefinition.Add(new TargetMobTyp());
            }
            if (GUILayout.Button("Remove"))
            {
                if (obj.mobtypeDefinition.Count > 0)
                    obj.mobtypeDefinition.RemoveAt(obj.mobtypeDefinition.Count - 1);
            }
            EditorGUILayout.EndHorizontal();


            GUILayout.EndVertical();

        
            GUILayout.BeginVertical("Effect Configuration", boxStyle);
            GUILayout.Space(20);
            content = new GUIContent("Active effect");
            content.tooltip = "Select to show effects";
            obj.activeEffect = EditorGUILayout.Toggle(content, obj.activeEffect);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


            content = new GUIContent("Effect Buttons");
            content.tooltip = "";

            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.effectButtons == null)
                obj.effectButtons = new System.Collections.Generic.List<UGUIEffect>();
            for (int i = 0; i < obj.effectButtons.Count; i++)
            {
                obj.effectButtons[i] = (UGUIEffect)EditorGUILayout.ObjectField(obj.effectButtons[i], typeof(UGUIEffect), true);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.effectButtons.Add(null);
            }
            if (GUILayout.Button("Remove"))
            {
                if (obj.effectButtons.Count > 0)
                    obj.effectButtons.RemoveAt(obj.effectButtons.Count - 1);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.BeginVertical("Stance Configuration", boxStyle);
            GUILayout.Space(20);
            content = new GUIContent("Light Image");
            content.tooltip = "Select mob type image UI element to attach";
            obj.LightImage = (Image)EditorGUILayout.ObjectField(content, obj.LightImage, typeof(Image), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Friendly Light Image");
            content.tooltip = "Select friendly light image";
            obj.friendlyImage = (Sprite)EditorGUILayout.ObjectField(content, obj.friendlyImage, typeof(Sprite), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            content = new GUIContent("Neutral Light Image");
            content.tooltip = "Select neutral light image";
            obj.neutralImage = (Sprite)EditorGUILayout.ObjectField(content, obj.neutralImage, typeof(Sprite), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            content = new GUIContent("Enemy Light Image");
            content.tooltip = "Select enemy light image";
            obj.enemyImage = (Sprite)EditorGUILayout.ObjectField(content, obj.enemyImage, typeof(Sprite), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


            content = new GUIContent("Friendly Name Colour");
            content.tooltip = "Select friendly colour for the name";
            obj.friendlyNameColour = EditorGUILayout.ColorField(content, obj.friendlyNameColour);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Neutral Name Colour");
            content.tooltip = "Select neutral colour for the name";
            obj.neutralNameColour = EditorGUILayout.ColorField(content, obj.neutralNameColour);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            content = new GUIContent("Enemy Name Colour");
            content.tooltip = "Select enemy colour for the name";
            obj.enemyNameColour = EditorGUILayout.ColorField(content, obj.enemyNameColour);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            GUILayout.EndVertical();

            content = new GUIContent("Chat Controller");
            content.tooltip = "Select mob type image UI element to attach";
            obj.chatController = (UGUIChatController)EditorGUILayout.ObjectField(content, obj.chatController, typeof(UGUIChatController), true);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            GUILayout.Space(2);

            GUILayout.EndVertical();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(obj);
                EditorUtility.SetDirty(target);
            }
        }
        public int GetOptionPosition(int id, int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                if (ids[i] == id)
                    return i;
            }
            return 0;
        }

    }
}