using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    [CustomEditor(typeof(AtavismMobSpawnMarker))]
    [CanEditMultipleObjects]
    public class AtavismMobSpawnMarkerEditor : Editor
    {

        private bool mobTemplatesLoaded = false;
        private bool questsLoaded = false;
        private bool dialoguesLoaded = false;
        private bool merchantTablesLoaded = false;
        //  string[] interactionTypes;
        bool help = false;
        SerializedObject so;
        SerializedProperty mobTemplateID;
        SerializedProperty mobTemplateID2;
        SerializedProperty mobTemplateID3;
        SerializedProperty mobTemplateID4;
        SerializedProperty mobTemplateID5;
        SerializedProperty alternateMobTemplateID;
        SerializedProperty alternateMobTemplateID2;
        SerializedProperty alternateMobTemplateID3;
        SerializedProperty alternateMobTemplateID4;
        SerializedProperty alternateMobTemplateID5;
        SerializedProperty respawnTime;
        SerializedProperty respawnTimeMax;
        SerializedProperty despawnTime;
        SerializedProperty spawnActiveStartHour;
        SerializedProperty spawnActiveEndHour;
        SerializedProperty roamRadius;
        SerializedProperty show_aggro;
        SerializedProperty show_roam;
        SerializedProperty merchantTable;

        private void OnEnable()
        {
            mobTemplateID = serializedObject.FindProperty("mobTemplateID");
            mobTemplateID2 = serializedObject.FindProperty("mobTemplateID2");
            mobTemplateID3 = serializedObject.FindProperty("mobTemplateID3");
            mobTemplateID4 = serializedObject.FindProperty("mobTemplateID4");
            mobTemplateID5 = serializedObject.FindProperty("mobTemplateID5");
            alternateMobTemplateID = serializedObject.FindProperty("alternateMobTemplateID");
            alternateMobTemplateID2 = serializedObject.FindProperty("alternateMobTemplateID2");
            alternateMobTemplateID3 = serializedObject.FindProperty("alternateMobTemplateID3");
            alternateMobTemplateID4 = serializedObject.FindProperty("alternateMobTemplateID4");
            alternateMobTemplateID5 = serializedObject.FindProperty("alternateMobTemplateID5");
            respawnTime = serializedObject.FindProperty("respawnTime");
            respawnTimeMax = serializedObject.FindProperty("respawnTimeMax");
            despawnTime = serializedObject.FindProperty("despawnTime");
            spawnActiveStartHour = serializedObject.FindProperty("spawnActiveStartHour");
            spawnActiveEndHour = serializedObject.FindProperty("spawnActiveEndHour");
            roamRadius = serializedObject.FindProperty("roamRadius");
           show_aggro = serializedObject.FindProperty("show_aggro");
            show_roam = serializedObject.FindProperty("show_roam");
            merchantTable = serializedObject.FindProperty("merchantTable");
         //   roamRadius = serializedObject.FindProperty("roamRadius");
          //  roamRadius = serializedObject.FindProperty("roamRadius");

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            AtavismMobSpawnMarker obj = target as AtavismMobSpawnMarker;
            so = new SerializedObject(target);
            EditorGUI.BeginChangeCheck();
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
            GUILayout.BeginVertical("Atavism Spawner Object Configuration", topStyle);
            GUILayout.Space(20);

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

            if (!mobTemplatesLoaded)
            {
                ServerMobs.LoadMobOptions(true);
                mobTemplatesLoaded = true;
            }
            content = new GUIContent("Search Template:");
            content.tooltip = "Search Template ";
            obj.mobTemplateSearch = EditorGUILayout.TextField(content, obj.mobTemplateSearch);

            content = new GUIContent("Mob Template");
            content.tooltip = "Select Mob Template";
            mobTemplateID.intValue = ServerCharacter.GetFilteredListSelector(content, ref obj.mobTemplateSearch, mobTemplateID.intValue, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            content = new GUIContent("Search Template 2:");
            content.tooltip = "Search Template ";
            obj.mobTemplateSearch2 = EditorGUILayout.TextField(content, obj.mobTemplateSearch2);

            content = new GUIContent("Mob Template 2");
            content.tooltip = "Select Mob Template";
            mobTemplateID2.intValue = ServerCharacter.GetFilteredListSelector(content, ref obj.mobTemplateSearch2, mobTemplateID2.intValue, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            content = new GUIContent("Search Template 3:");
            content.tooltip = "Search Template ";
            obj.mobTemplateSearch3 = EditorGUILayout.TextField(content, obj.mobTemplateSearch3);

            content = new GUIContent("Mob Template 3");
            content.tooltip = "Select Mob Template";
            mobTemplateID3.intValue = ServerCharacter.GetFilteredListSelector(content, ref obj.mobTemplateSearch3, mobTemplateID3.intValue, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            content = new GUIContent("Search Template 4:");
            content.tooltip = "Search Template ";
            obj.mobTemplateSearch4 = EditorGUILayout.TextField(content, obj.mobTemplateSearch4);

            content = new GUIContent("Mob Template 4");
            content.tooltip = "Select Mob Template";
            mobTemplateID4.intValue = ServerCharacter.GetFilteredListSelector(content, ref obj.mobTemplateSearch4, mobTemplateID4.intValue, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            content = new GUIContent("Search Template 5:");
            content.tooltip = "Search Template ";
            obj.mobTemplateSearch5 = EditorGUILayout.TextField(content, obj.mobTemplateSearch5);

            content = new GUIContent("Mob Template 5");
            content.tooltip = "Select Mob Template";
            mobTemplateID5.intValue = ServerCharacter.GetFilteredListSelector(content, ref obj.mobTemplateSearch5, mobTemplateID5.intValue, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


       /*     content = new GUIContent("Spawn Radius");
            content.tooltip = "Spawn Radius";
            obj.spawnRadius = EditorGUILayout.IntField(content, obj.spawnRadius);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Spawn Count");
            content.tooltip = "Spawn Count";
            obj.numSpawns = EditorGUILayout.IntField(content, obj.numSpawns);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
*/
            content = new GUIContent("Respawn Time (in seconds)");
            content.tooltip = "Respawn Time (in seconds)";
            respawnTime.intValue = EditorGUILayout.IntField(content, respawnTime.intValue);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            
            content = new GUIContent("Respawn Time Max(in seconds)");
            content.tooltip = "Respawn Time (in seconds)";
            respawnTimeMax.intValue = EditorGUILayout.IntField(content, respawnTimeMax.intValue);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Corpse Despawn Time (in seconds)");
            content.tooltip = "Corpse Despawn Time (in seconds)";
            despawnTime.intValue = EditorGUILayout.IntField(content, despawnTime.intValue);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

           GUILayout.Space(2);
            GUILayout.BeginVertical("Time Based Spawn Rules", boxStyle);
           // EditorGUILayout.LabelField("Time Based Spawn Rules");
            GUILayout.Space(20);

            content = new GUIContent("Spawn Activate Hour");
            content.tooltip = "Spawn Activate Hour";
            spawnActiveStartHour.intValue = EditorGUILayout.IntSlider(content, spawnActiveStartHour.intValue, -1, 23);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Spawn Deactivate Hour");
            content.tooltip = "Spawn Deactivate Hour";
            spawnActiveEndHour.intValue = EditorGUILayout.IntSlider(content, spawnActiveEndHour.intValue, -1, 23);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            EditorGUILayout.Space(5);
            
            content = new GUIContent("Search Template:");
            content.tooltip = "Search Template ";
            obj.alternateMobTemplateSearch = EditorGUILayout.TextField(content, obj.alternateMobTemplateSearch);

            content = new GUIContent("Alternate Spawn Mob Template");
            content.tooltip = "Select Alternate Spawn Mob Template";
            alternateMobTemplateID.intValue = ServerCharacter.GetFilteredListSelector(content, ref obj.alternateMobTemplateSearch, alternateMobTemplateID.intValue, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Search Template 2:");
            content.tooltip = "Search Template ";
            obj.alternateMobTemplateSearch2 = EditorGUILayout.TextField(content, obj.alternateMobTemplateSearch2);

            content = new GUIContent("Alternate Spawn Mob Template 2");
            content.tooltip = "Select Alternate Spawn Mob Template";
            alternateMobTemplateID2.intValue = ServerCharacter.GetFilteredListSelector(content, ref obj.alternateMobTemplateSearch2, alternateMobTemplateID2.intValue, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Search Template 3:");
            content.tooltip = "Search Template ";
            obj.alternateMobTemplateSearch3 = EditorGUILayout.TextField(content, obj.alternateMobTemplateSearch3);

            content = new GUIContent("Alternate Spawn Mob Template 3");
            content.tooltip = "Select Alternate Spawn Mob Template";
            alternateMobTemplateID3.intValue = ServerCharacter.GetFilteredListSelector(content, ref obj.alternateMobTemplateSearch3, alternateMobTemplateID3.intValue, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Search Template 4:");
            content.tooltip = "Search Template ";
            obj.alternateMobTemplateSearch4 = EditorGUILayout.TextField(content, obj.alternateMobTemplateSearch4);

            content = new GUIContent("Alternate Spawn Mob Template 4");
            content.tooltip = "Select Alternate Spawn Mob Template";
            alternateMobTemplateID4.intValue = ServerCharacter.GetFilteredListSelector(content, ref obj.alternateMobTemplateSearch4, alternateMobTemplateID4.intValue, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Search Template 5:");
            content.tooltip = "Search Template ";
            obj.alternateMobTemplateSearch5 = EditorGUILayout.TextField(content, obj.alternateMobTemplateSearch5);

            content = new GUIContent("Alternate Spawn Mob Template 5");
            content.tooltip = "Select Alternate Spawn Mob Template";
            alternateMobTemplateID5.intValue = ServerCharacter.GetFilteredListSelector(content, ref obj.alternateMobTemplateSearch5, alternateMobTemplateID5.intValue, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            GUILayout.EndVertical();

            GUILayout.Space(2);
            GUILayout.BeginVertical("Spawn Behaviour", boxStyle);
            GUILayout.Space(20);

            content = new GUIContent("Roam Radius");
            content.tooltip = "Roam radius";
            roamRadius.floatValue = EditorGUILayout.FloatField(content, roamRadius.floatValue);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            if (!questsLoaded)
                ServerQuests.LoadQuestOptions(true);
            questsLoaded = true;
            EditorGUILayout.Space(5);

            content = new GUIContent("Starts Quests");
            content.tooltip = "Starts Quests";
            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            List<int> startsQuests = new List<int>(obj.startsQuests);
            for (int i = 0; i < startsQuests.Count; i++)
            {
                startsQuests[i] = EditorGUILayout.IntPopup(new GUIContent(), startsQuests[i], ServerQuests.GuiQuestOptions, ServerQuests.questIds);
            }
            startsQuests.Remove(-1);
            
            
            content = new GUIContent("Search Start Quest:");
            content.tooltip = "Search Start Quest ";
            obj.startsQuestsSearch = EditorGUILayout.TextField(content, obj.startsQuestsSearch);

            int newQuest = ServerCharacter.GetFilteredListSelector(new GUIContent(), ref obj.startsQuestsSearch, -1, ServerQuests.GuiQuestOptions, ServerQuests.questIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (newQuest > 0)
            {
                startsQuests.Add(newQuest);
                obj.startsQuestsSearch = "";
            }
            obj.startsQuests = startsQuests;
            EditorGUILayout.Space(5);

           content = new GUIContent("Ends Quests");
            content.tooltip = "Ends Quests";
            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            List<int> endsQuests = new List<int>(obj.endsQuests);
            for (int i = 0; i < endsQuests.Count; i++)
            {
                endsQuests[i] = EditorGUILayout.IntPopup(new GUIContent(), endsQuests[i], ServerQuests.GuiQuestOptions, ServerQuests.questIds);
            }
            endsQuests.Remove(-1);
            content = new GUIContent("Search End Quest:");
            content.tooltip = "Search End Quest ";
            obj.endsQuestsSearch = EditorGUILayout.TextField(content, obj.endsQuestsSearch);

             newQuest = ServerCharacter.GetFilteredListSelector(new GUIContent(), ref obj.endsQuestsSearch, -1,  ServerQuests.GuiQuestOptions, ServerQuests.questIds);
            if (newQuest > 0)
            {
                endsQuests.Add(newQuest);
                obj.endsQuestsSearch = "";
            }
            obj.endsQuests = endsQuests;

            if (!dialoguesLoaded)
                ServerDialogues.LoadOpeningDialogueList(true);
            dialoguesLoaded = true;
            EditorGUILayout.Space(5);

            content = new GUIContent("Offers Dialogues");
            content.tooltip = "Offers Dialogues";
            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            List<int> dialogues = new List<int>(obj.startsDialogues);
            for (int i = 0; i < dialogues.Count; i++)
            {
                dialogues[i] = EditorGUILayout.IntPopup(new GUIContent(), dialogues[i], ServerDialogues.GuiDialogueList, ServerDialogues.dialogueIds);
            } 
            dialogues.Remove(-1);
            content = new GUIContent("Search Dialogue:");
            content.tooltip = "Search Dialogue";
            obj.startsDialoguesSearch = EditorGUILayout.TextField(content, obj.startsDialoguesSearch);

             newQuest = ServerCharacter.GetFilteredListSelector(new GUIContent(), ref obj.startsDialoguesSearch, -1,  ServerDialogues.GuiDialogueList, ServerDialogues.dialogueIds);
            if (newQuest > 0)
            {
                dialogues.Add(newQuest);
                obj.startsDialoguesSearch = "";
            }
            obj.startsDialogues = dialogues;

            EditorGUILayout.Separator();

            if (!merchantTablesLoaded)
                ServerMerchantTables.LoadMerchantTableList(true);
            merchantTablesLoaded = true;

            content = new GUIContent("Merchant Table");
            content.tooltip = "Merchant Table";
            merchantTable.intValue = EditorGUILayout.IntPopup(content, merchantTable.intValue, ServerMerchantTables.GuiMerchantTableList, ServerMerchantTables.merchantTableIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            if (obj.otherActions.Contains("Bank"))
            {
                content = new GUIContent("Offers Bank");
                content.tooltip = "Offers Bank";
                if (!EditorGUILayout.Toggle(content, true))
                {
                    obj.otherActions.Remove("Bank");
                }
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }
            else
            {
                content = new GUIContent("Offers Bank");
                content.tooltip = "Offers Bank";
                if (EditorGUILayout.Toggle(content, false))
                {
                    obj.otherActions.Add("Bank");
                }
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }
            GUILayout.EndVertical();
            GUILayout.Space(20);
        
            GUILayout.BeginVertical("Spawner Actions", boxStyle);
            // EditorGUILayout.LabelField("Time Based Spawn Rules");
            GUILayout.Space(20);

            content = new GUIContent("Show Aggro Radius");
            content.tooltip = "Show Aggro Radius";
            show_aggro.boolValue = EditorGUILayout.Toggle(content, show_aggro.boolValue);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            
            content = new GUIContent("Show Roam Radius");
            content.tooltip = "Show Roam Radius";
            show_roam.boolValue = EditorGUILayout.Toggle(content, show_roam.boolValue);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            if (!obj.position.Equals(obj.gameObject.transform.position))
            {
                EditorGUILayout.PropertyField(so.FindProperty("layers"));
                content = new GUIContent("Layers");
                content.tooltip = "Select Layers of the ground";
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                
                GUI.color = Color.red;
                if (GUILayout.Button("Raycast Check Distance"))
                {
                    bool found = false;
                    int count = 0;
                    do
                    {
                        count++;
                        RaycastHit HitDown;
                        if (Physics.Raycast(new Vector3(obj.gameObject.transform.position.x, obj.gameObject.transform.position.y + 1 * count, obj.gameObject.transform.position.z), -Vector3.up, out HitDown, 2*count, obj.layers))
                        {
                           Vector3 v = HitDown.point + new Vector3(0f, 0.1f, 0f);
                           obj.gameObject.transform.position = v;
                           found = true;
                           obj.position = v;
                        }
                    } while (!found && count < 100);
                }
                GUI.color = Color.white;
            }
            
            /*   
             
   
               if (GUILayout.Button("Save"))
               {
                    obj.Clear();
               }*/
           

            GUILayout.EndVertical();
      
             
            GUILayout.EndVertical();
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(obj);
                EditorUtility.SetDirty(target);
            }
            if (EditorGUI.EndChangeCheck())
            {
                so.ApplyModifiedProperties();
              //  GUI.FocusControl(null);
            }
            if (GUILayout.Button("Save"))
            {
               
                ServerMobSpawner.SaveSpawner(obj);
               
            }
            if (GUILayout.Button("Delete"))
            {
                if (EditorUtility.DisplayDialog("Delete Mob Spawner", "Are you sure you want to delete spawner permanently" , "Delete", "Cancel"))
                {
                    ServerMobSpawner.DeleteSpawner(obj);
                    DestroyImmediate(obj.gameObject);
                }
                
               
            }
            serializedObject.ApplyModifiedProperties();
        }
        /*  private int GetPositionOfInteraction(string interactionType)
          {
              for (int i = 0; i < interactionTypes.Length; i++)
              {
                  if (interactionTypes[i] == interactionType)
                      return i;
              }
              return 0;
          }*/
    }
}