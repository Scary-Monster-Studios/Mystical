using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    [CustomEditor(typeof(AtavismMobSpawnMarkerGenerator))]
    public class AtavismMobSpawnMarkerGeneratorEditor : Editor
    {

        private bool mobTemplatesLoaded = false;
        private bool questsLoaded = false;
        private bool dialoguesLoaded = false;
        private bool merchantTablesLoaded = false;
        //  string[] interactionTypes;
        bool help = false;
        SerializedObject so;
        
        public override void OnInspectorGUI()
        {
            AtavismMobSpawnMarkerGenerator obj = target as AtavismMobSpawnMarkerGenerator;
            so = new SerializedObject(target);
           
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
          /*  GUIContent reset = new GUIContent("Reset ID");
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
*/
          EditorGUI.BeginChangeCheck();
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
            obj.mobTemplateID = ServerCharacter.GetFilteredListSelector(content, ref obj.mobTemplateSearch, obj.mobTemplateID, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            content = new GUIContent("Search Template 2:");
            content.tooltip = "Search Template ";
            obj.mobTemplateSearch2 = EditorGUILayout.TextField(content, obj.mobTemplateSearch2);

            content = new GUIContent("Mob Template 2");
            content.tooltip = "Select Mob Template";
            obj.mobTemplateID2 = ServerCharacter.GetFilteredListSelector(content, ref obj.mobTemplateSearch2, obj.mobTemplateID2, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            content = new GUIContent("Search Template 3:");
            content.tooltip = "Search Template ";
            obj.mobTemplateSearch3 = EditorGUILayout.TextField(content, obj.mobTemplateSearch3);

            content = new GUIContent("Mob Template 3");
            content.tooltip = "Select Mob Template";
            obj.mobTemplateID3 = ServerCharacter.GetFilteredListSelector(content, ref obj.mobTemplateSearch3, obj.mobTemplateID3, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            content = new GUIContent("Search Template 4:");
            content.tooltip = "Search Template ";
            obj.mobTemplateSearch4 = EditorGUILayout.TextField(content, obj.mobTemplateSearch4);

            content = new GUIContent("Mob Template 4");
            content.tooltip = "Select Mob Template";
            obj.mobTemplateID4 = ServerCharacter.GetFilteredListSelector(content, ref obj.mobTemplateSearch4, obj.mobTemplateID4, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            content = new GUIContent("Search Template 5:");
            content.tooltip = "Search Template ";
            obj.mobTemplateSearch5 = EditorGUILayout.TextField(content, obj.mobTemplateSearch5);

            content = new GUIContent("Mob Template 5");
            content.tooltip = "Select Mob Template";
            obj.mobTemplateID5 = ServerCharacter.GetFilteredListSelector(content, ref obj.mobTemplateSearch5, obj.mobTemplateID5, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
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
            obj.respawnTime = EditorGUILayout.IntField(content, obj.respawnTime);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            
            content = new GUIContent("Respawn Time Max(in seconds)");
            content.tooltip = "Respawn Time (in seconds)";
            obj.respawnTimeMax = EditorGUILayout.IntField(content, obj.respawnTimeMax);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Corpse Despawn Time (in seconds)");
            content.tooltip = "Corpse Despawn Time (in seconds)";
            obj.despawnTime = EditorGUILayout.IntField(content, obj.despawnTime);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            GUILayout.Space(2);
            GUILayout.BeginVertical("Time Based Spawn Rules", boxStyle);
           // EditorGUILayout.LabelField("Time Based Spawn Rules");
            GUILayout.Space(20);

            content = new GUIContent("Spawn Activate Hour");
            content.tooltip = "Spawn Activate Hour";
            obj.spawnActiveStartHour = EditorGUILayout.IntSlider(content, obj.spawnActiveStartHour, -1, 23);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Spawn Deactivate Hour");
            content.tooltip = "Spawn Deactivate Hour";
            obj.spawnActiveEndHour = EditorGUILayout.IntSlider(content, obj.spawnActiveEndHour, -1, 23);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            EditorGUILayout.Space(5);
            
            content = new GUIContent("Search Template:");
            content.tooltip = "Search Template ";
            obj.alternateMobTemplateSearch = EditorGUILayout.TextField(content, obj.alternateMobTemplateSearch);

            content = new GUIContent("Alternate Spawn Mob Template");
            content.tooltip = "Select Alternate Spawn Mob Template";
            obj.alternateMobTemplateID = ServerCharacter.GetFilteredListSelector(content, ref obj.alternateMobTemplateSearch, obj.alternateMobTemplateID, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Search Template 2:");
            content.tooltip = "Search Template ";
            obj.alternateMobTemplateSearch2 = EditorGUILayout.TextField(content, obj.alternateMobTemplateSearch2);

            content = new GUIContent("Alternate Spawn Mob Template 2");
            content.tooltip = "Select Alternate Spawn Mob Template";
            obj.alternateMobTemplateID2 = ServerCharacter.GetFilteredListSelector(content, ref obj.alternateMobTemplateSearch2, obj.alternateMobTemplateID2, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Search Template 3:");
            content.tooltip = "Search Template ";
            obj.alternateMobTemplateSearch3 = EditorGUILayout.TextField(content, obj.alternateMobTemplateSearch3);

            content = new GUIContent("Alternate Spawn Mob Template 3");
            content.tooltip = "Select Alternate Spawn Mob Template";
            obj.alternateMobTemplateID3 = ServerCharacter.GetFilteredListSelector(content, ref obj.alternateMobTemplateSearch3, obj.alternateMobTemplateID3, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Search Template 4:");
            content.tooltip = "Search Template ";
            obj.alternateMobTemplateSearch4 = EditorGUILayout.TextField(content, obj.alternateMobTemplateSearch4);

            content = new GUIContent("Alternate Spawn Mob Template 4");
            content.tooltip = "Select Alternate Spawn Mob Template";
            obj.alternateMobTemplateID4 = ServerCharacter.GetFilteredListSelector(content, ref obj.alternateMobTemplateSearch4, obj.alternateMobTemplateID4, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Search Template 5:");
            content.tooltip = "Search Template ";
            obj.alternateMobTemplateSearch5 = EditorGUILayout.TextField(content, obj.alternateMobTemplateSearch5);

            content = new GUIContent("Alternate Spawn Mob Template 5");
            content.tooltip = "Select Alternate Spawn Mob Template";
            obj.alternateMobTemplateID5 = ServerCharacter.GetFilteredListSelector(content, ref obj.alternateMobTemplateSearch5, obj.alternateMobTemplateID5, ServerMobs.GuiMobOptions, ServerMobs.mobIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            GUILayout.EndVertical();

            GUILayout.Space(2);
            GUILayout.BeginVertical("Spawn Behaviour", boxStyle);
            GUILayout.Space(20);

            content = new GUIContent("Roam Radius");
            content.tooltip = "Roam radius";
            obj.roamRadius = EditorGUILayout.FloatField(content, obj.roamRadius);
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
            obj.merchantTable = EditorGUILayout.IntPopup(content, obj.merchantTable, ServerMerchantTables.GuiMerchantTableList, ServerMerchantTables.merchantTableIds);
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
            
            
            GUILayout.BeginVertical("Ganerate Spawns", boxStyle);
            // EditorGUILayout.LabelField("Time Based Spawn Rules");
            GUILayout.Space(20);

            
            
            content = new GUIContent("Spawn Radius");
            content.tooltip = "Spawn Radius";
          //  obj.mesh = (Mesh) EditorGUILayout.ObjectField(content, obj.mesh,typeof(Mesh));
            
            content = new GUIContent("Spawn Radius");
            content.tooltip = "Spawn Radius";
            obj.spawn_radius = EditorGUILayout.FloatField(content, obj.spawn_radius);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            content = new GUIContent("Spawn Height");
            content.tooltip = "Spawn Height";
            obj.spawn_height = EditorGUILayout.FloatField(content, obj.spawn_height);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Spawn Count");
            content.tooltip = "Spawn Count";
            obj.spawn_count = EditorGUILayout.IntField(content, obj.spawn_count);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Spawn Min Distance");
            content.tooltip = "Spawn Min Distance";
            obj.spawn_min_distance = EditorGUILayout.FloatField(content, obj.spawn_min_distance);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
           
            
            content = new GUIContent("Search Position Iteration Count");
            content.tooltip = "Search Position Iteration Count";
            obj.iterationCount = EditorGUILayout.IntField(content, obj.iterationCount);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            
            content = new GUIContent("Gizmo Line Count");
            content.tooltip = "Gizmo Line Count";
            obj.lineDrawCount = EditorGUILayout.IntSlider(content, obj.lineDrawCount,18,1440);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            
            EditorGUILayout.PropertyField(so.FindProperty("layers"));
            content = new GUIContent("Layers");
            content.tooltip = "Select Layer of the ground";
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (EditorGUI.EndChangeCheck())
            {
                so.ApplyModifiedProperties();
                //  GUI.FocusControl(null);
            }
            GUILayout.EndVertical();
             
            GUILayout.EndVertical();
            if (GUILayout.Button("Generate"))
            {
                obj.Clear();
                Mob mob = ServerMobs.LoadMobTemplateModel(obj.mobTemplateID);
                int count = obj.Spawn(mob.display1, mob.aggro_range);
        
                EditorUtility.DisplayDialog("Atavism Spawn Ganarated", "Genarated "+count+" spawns", "OK", "");
                GUIUtility.ExitGUI();
            }

            if (GUILayout.Button("Reset"))
            {
               obj.Clear();
            }

            GUILayout.Space(20);
            if (!obj.show_aggro)
            {
                if (GUILayout.Button("Show Aggro Radius"))
                {
                    foreach (var mob in obj.mobs)
                    {
                        AtavismMobSpawnMarker amsm = mob.GetComponent<AtavismMobSpawnMarker>();
                        if (amsm)
                        {
                            amsm.show_aggro = true;
                        }
                    }

                    obj.show_aggro = true;
                }
            }
            else
            {
                if (GUILayout.Button("Hide Aggro Radius"))
                {
                    foreach (var mob in obj.mobs)
                    {
                        AtavismMobSpawnMarker amsm = mob.GetComponent<AtavismMobSpawnMarker>();
                        if (amsm)
                        {
                            amsm.show_aggro = false;
                        }
                    }
                    obj.show_aggro = false;
                }
               
            }
            if (!obj.show_roam)
            {
                if (GUILayout.Button("Show Roam Radius"))
                {
                    foreach (var mob in obj.mobs)
                    {
                        AtavismMobSpawnMarker amsm = mob.GetComponent<AtavismMobSpawnMarker>();
                        if (amsm)
                        {
                            amsm.show_roam = true;
                        }
                    }

                    obj.show_roam = true;
                }
            }
            else
            {
                if (GUILayout.Button("Hide Roam Radius"))
                {
                    foreach (var mob in obj.mobs)
                    {
                        AtavismMobSpawnMarker amsm = mob.GetComponent<AtavismMobSpawnMarker>();
                        if (amsm)
                        {
                            amsm.show_roam = false;
                        }
                    }

                    obj.show_roam = false;
                }

            }
          
            if (GUI.changed)
            {
                EditorUtility.SetDirty(obj);
                EditorUtility.SetDirty(target);
            }
           
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