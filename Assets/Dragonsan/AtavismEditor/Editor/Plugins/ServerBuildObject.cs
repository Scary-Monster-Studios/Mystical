using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
namespace Atavism
{
    // Handles the Build Object Template Configuration
    public class ServerBuildObject : AtavismDatabaseFunction
    {

        public new Dictionary<int, BuildObjectData> dataRegister;
        public new BuildObjectData editingDisplay;
        public new BuildObjectData originalDisplay;

        public int[] categoryIds = new int[] { -1 };
        public string[] categoryOptions = new string[] { "~ none ~" };

        public int[] itemIds = new int[] { -1 };
        public string[] itemsList = new string[] { "~ none ~" };
        public int[] skillIds = new int[] { -1 };
        public string[] skillOptions = new string[] { "~ none ~" };
        public string[] weaponTypeOptions = new string[] { "~ none ~" };
        public string[] claimObjectInteractionTypeOptions = new string[] { "~ none ~" };

        public int[] tableIds = new int[] { -1 };
        public string[] tablesList = new string[] { "~ none ~" };

        public int[] mobIds = new int[] { -1 };
        public string[] mobList = new string[] { "~ none ~" };

        public int[] effectIds = new int[] { -1 };
        public string[] effectOptions = new string[] { "~ none ~" };

        public int[] instanceIds = new int[] { -1 };
        public string[] instanceOptions = new string[] { "~ none ~" };

        // Handles the prefab creation, editing and save
        private BuildObjectPrefab item = null;
        // Use this for initialization
        public ServerBuildObject()
        {
            functionName = "Build Object";
            // Database tables name
            tableName = "build_object_template";
            functionTitle = "Build Object Configuration";
            loadButtonLabel = "Load Build Object";
            notLoadedText = "No Build Objects loaded.";
            // Init
            dataRegister = new Dictionary<int, BuildObjectData>();

            editingDisplay = new BuildObjectData();
            originalDisplay = new BuildObjectData();
        }

        public override void Activate()
        {
            linkedTablesLoaded = false;
            resetSearch(true);
        }
        void resetSearch(bool fokus)
        {
           if(fokus) GUI.FocusControl(null);
        }
        private void LoadItemList()
        {
            string query = "SELECT id, name FROM item_templates where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                itemsList = new string[rows.Count + 1];
                itemsList[optionsId] = "~ none ~";
                itemIds = new int[rows.Count + 1];
                itemIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    itemsList[optionsId] = data["id"] + ":" + data["name"];
                    itemIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        public void LoadSkillOptions()
        {
            string query = "SELECT id, name FROM skills where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                skillOptions = new string[rows.Count + 1];
                skillOptions[optionsId] = "~ none ~";
                skillIds = new int[rows.Count + 1];
                skillIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    skillOptions[optionsId] = data["id"] + ":" + data["name"];
                    skillIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        private void LoadTableList()
        {
            string query = "SELECT id, name FROM loot_tables where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                tablesList = new string[rows.Count + 1];
                tablesList[optionsId] = "~ none ~";
                tableIds = new int[rows.Count + 1];
                tableIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    tablesList[optionsId] = data["id"] + ":" + data["name"];
                    tableIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        private void LoadMobList()
        {
            string query = "SELECT id, name FROM spawn_data where isactive = 1 AND instance is NULL";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                mobList = new string[rows.Count + 1];
                mobList[optionsId] = "~ none ~";
                mobIds = new int[rows.Count + 1];
                mobIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    mobList[optionsId] = data["id"] + ":" + data["name"];
                    mobIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        public void LoadEffectOptions()
        {
           
                // Read all entries from the table
                string query = "SELECT id, name FROM effects where isactive = 1";

                // If there is a row, clear it.
                if (rows != null)
                    rows.Clear();

                // Load data
                rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
                //Debug.Log("#Rows:"+rows.Count);
                // Read all the data
                int optionsId = 0;
                if ((rows != null) && (rows.Count > 0))
                {
                    effectOptions = new string[rows.Count + 1];
                    effectOptions[optionsId] = "~ none ~";
                    effectIds = new int[rows.Count + 1];
                    effectIds[optionsId] = 0;
                    foreach (Dictionary<string, string> data in rows)
                    {
                        optionsId++;
                        effectOptions[optionsId] = data["id"] + ":" + data["name"];
                        effectIds[optionsId] = int.Parse(data["id"]);
                    }
                }
           
        }

        public void LoadInstanceOptions()
        {
            // Read all entries from the table
            string query = "SELECT id, island_name FROM instance_template";

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.adminDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                instanceOptions = new string[rows.Count + 1];
                instanceOptions[optionsId] = "~ none ~";
                instanceIds = new int[rows.Count + 1];
                instanceIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    instanceOptions[optionsId] = data["id"] + ":" + data["island_name"];
                    instanceIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        // Load Database Data
        public override void Load()
        {
            if (!dataLoaded)
            {
                // Clean old data
                // dataRegister.Clear();
                displayKeys.Clear();

                // Read all entries from the table
                string query = "SELECT id,name FROM " + tableName + " where isactive = 1";

                // If there is a row, clear it.
                if (rows != null)
                    rows.Clear();

                // Load data
                rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
                //Debug.Log("#Rows:"+rows.Count);
                // Read all the data
                displayList.Clear();
                if ((rows != null) && (rows.Count > 0))
                {
                    foreach (Dictionary<string, string> data in rows)
                    {
                        displayList.Add(data["id"] + ". " + data["name"]);
                        displayKeys.Add(int.Parse(data["id"]));

                    }
                }
                dataLoaded = true;
                editingDisplay = null;
            }
        }

        public override void LoadRestore()
        {
            if (!dataRestoreLoaded)
            {
                // Clean old data
                dataRegister.Clear();
                displayKeys.Clear();

                // Read all entries from the table
                string query = "SELECT id,name FROM " + tableName + " where isactive = 0";

                // If there is a row, clear it.
                if (rows != null)
                    rows.Clear();

                // Load data
                rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
                //Debug.Log("#Rows:"+rows.Count);
                // Read all the data
                displayList.Clear();
                if ((rows != null) && (rows.Count > 0))
                {
                    foreach (Dictionary<string, string> data in rows)
                    {
                        displayList.Add(data["id"] + ". " + data["name"]);
                        displayKeys.Add(int.Parse(data["id"]));

                    }
                }
                dataRestoreLoaded = true;
            }
        }

        BuildObjectData LoadEntity(int id)
        {


            // Load in stages first
            Dictionary<int, BuildObjectStage> stages = LoadBuildObjectStages();

            // Read all entries from the table
            string query = "SELECT " + originalDisplay.GetFieldsString() + " FROM " + tableName + " where  id="+id;

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            BuildObjectData display = new BuildObjectData();
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    //foreach(string key in data.Keys)
                    //	Debug.Log("Name[" + key + "]:" + data[key]);
                    //return;
                    display.id = int.Parse(data["id"]);
                    display.Name = data["name"];
                    display.icon = data["icon"];
                    display.skill = int.Parse(data["skill"]);
                    display.skillLevelReq = int.Parse(data["skillLevelReq"]);
                    display.category = int.Parse(data["category"]);
                    display.weaponReq = data["weaponReq"];
                    display.distanceReq = float.Parse(data["distanceReq"]);
                    display.buildTaskReqPlayer = bool.Parse(data["buildTaskReqPlayer"]);
                    display.validClaimType = int.Parse(data["validClaimType"]);
                    display.firstStageID = int.Parse(data["firstStageID"]);
                    display.availableFromItemOnly = bool.Parse(data["availableFromItemOnly"]);
                    display.interactionType = data["interactionType"];
                    display.interactionID = int.Parse(data["interactionID"]);
                    display.interactionData1 = data["interactionData1"];

                    int stageID = display.firstStageID;
                    // Load in stages
                    while (stageID > 0)
                    {
                        if (stages.ContainsKey(stageID))
                        {
                            display.stages.Add(stages[stageID]);
                            stageID = stages[stageID].nextStage;
                        }
                        else
                        {
                            stageID = -1;
                        }
                    }
                    display.isLoaded = true;
                }
            }
           return display;
        }

        Dictionary<int, BuildObjectStage> LoadBuildObjectStages()
        {
            Dictionary<int, BuildObjectStage> stages = new Dictionary<int, BuildObjectStage>();
            // Read all entries from the table
            string query = "SELECT " + new BuildObjectStage().GetFieldsString() + " FROM build_object_stage where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    BuildObjectStage stage = new BuildObjectStage();
                    stage.id = int.Parse(data["id"]);
                    stage.gameObject = data["gameObject"];
                    stage.buildTimeReq = float.Parse(data["buildTimeReq"]);
                    stage.nextStage = int.Parse(data["nextStage"]);
                    for (int i = 1; i <= stage.maxEntries; i++)
                    {
                        int itemId = int.Parse(data["itemReq" + i]);
                        int count = int.Parse(data["itemReq" + i + "Count"]);
                        BuildObjectItemEntry entry = new BuildObjectItemEntry(itemId, count);
                        stage.entries.Add(entry);
                    }
                    stages.Add(stage.id, stage);
                }
            }
            return stages;
        }

      
        // Draw the loaded list
        public override void DrawLoaded(Rect box)
        {
            // Setup the layout
            Rect pos = box;
            pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;

            if (displayList.Count <= 0)
            {
                pos.y += ImagePack.fieldHeight;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create a Build Object before editing it."));
                return;
            }


            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Build Object"));

            if (newItemCreated)
            {
                newItemCreated = false;
                newSelectedDisplay = displayKeys.Count - 1;
            }
            // Draw data Editor
            if (newSelectedDisplay != selectedDisplay)
            {
                selectedDisplay = newSelectedDisplay;
                int displayKey = -1;
                if (selectedDisplay >= 0)
                    displayKey = displayKeys[selectedDisplay];
                editingDisplay = LoadEntity(displayKey);
            }
            if (editingDisplay == null)
            {
                int displayKey = -1;
                if (selectedDisplay >= 0)
                    displayKey = displayKeys[selectedDisplay];
                editingDisplay = LoadEntity(displayKey);
            }

            pos.y += ImagePack.fieldHeight * 1.5f;
            pos.x -= ImagePack.innerMargin;
            pos.y -= ImagePack.innerMargin;
            pos.width += ImagePack.innerMargin;

            if (state != State.Loaded)
            {
                pos.x += ImagePack.innerMargin;
                pos.width /= 2;
                //Draw super magical compound object.
                newSelectedDisplay = ImagePack.DrawDynamicPartialListSelector(pos, Lang.GetTranslate("Search Filter")+": ", ref entryFilterInput, selectedDisplay, displayList);

                pos.y += ImagePack.fieldHeight * 1.5f;
               /* if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Generate Prefabs")))
                {
                    GenerateAllPrefabs();
                }*/
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Duplicate")))
                {
                    Duplicate();
                }
                pos.x -= pos.width;

                pos.width *= 2;
                pos.y += ImagePack.fieldHeight * 1.5f;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Build Object Properties:"));
                pos.y += ImagePack.fieldHeight * 0.75f;
            }

            DrawEditor(pos, false);

            pos.y -= ImagePack.fieldHeight;
            //pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
        }

        public override void DrawRestore(Rect box)
        {
            // Setup the layout
            Rect pos = box;
            pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;

            if (displayList.Count <= 0)
            {
                pos.y += ImagePack.fieldHeight;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You dont have deleted")+" " + Lang.GetTranslate(functionName) + " .");
                return;
            }

            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Restore")+" " + Lang.GetTranslate(functionName) + " "+ Lang.GetTranslate("Configuration"));
            pos.y += ImagePack.fieldHeight;

            pos.width -= 140 + 155;
            for (int i = 0; i < displayList.Count; i++)
            {
                ImagePack.DrawText(pos, displayList[i]);
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos, Lang.GetTranslate("Delete Permanently")))
                {
                    DeleteForever(displayKeys[i]);
                }
                pos.x += 155;
                if (ImagePack.DrawButton(pos, Lang.GetTranslate("Restore")))
                {
                    RestoreEntry(displayKeys[i]);
                }

                pos.x -= pos.width + 155;
                pos.y += ImagePack.fieldHeight;
            }
            pos.width += 140+ 155;
            showCancel = false;
            showDelete = false;
            showSave = false;
            if (resultTimeout != -1 && resultTimeout < Time.realtimeSinceStartup && result != "")
            {
                result = "";
            }
            EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight);
        }
        public override void CreateNewData()
        {
            editingDisplay = new BuildObjectData();
            originalDisplay = new BuildObjectData();
            selectedDisplay = -1;
        }

        // Edit or Create
        public override void DrawEditor(Rect box, bool newItem)
        {
            newEntity = newItem;
            if (!linkedTablesLoaded)
            {
                // Load items
                LoadItemList();
                LoadSkillOptions();
                LoadTableList();
                LoadMobList();
                LoadEffectOptions();
                LoadInstanceOptions();
                ServerOptionChoices.LoadAtavismChoiceOptions("Building Category", true, out categoryIds, out categoryOptions);
                weaponTypeOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Weapon Type", true);
                claimObjectInteractionTypeOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Claim Object Interaction Type", true);
                linkedTablesLoaded = true;
            }
            // Setup the layout
            Rect pos = box;
            pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;

            // Draw the content database info		
            if (newEntity)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new build object template"));
                pos.y += ImagePack.fieldHeight;
            }

            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.5f);
            //pos.x += pos.width;
            //editingDisplay.icon = ImagePack.DrawTextureAsset (pos, "Icon:", editingDisplay.icon);	
            //pos.x -= pos.width;
            pos.width /= 2;
            pos.y += ImagePack.fieldHeight;
            int selectedSkill = GetOptionPosition(editingDisplay.skill, skillIds);
            selectedSkill = ImagePack.DrawSelector(pos, Lang.GetTranslate("Skill")+":", selectedSkill, skillOptions);
            editingDisplay.skill = skillIds[selectedSkill];
            pos.x += pos.width;
           // editingDisplay.icon = ImagePack.DrawSpriteAsset(pos, Lang.GetTranslate("Icon")+":", editingDisplay.icon);
            string icon = ImagePack.DrawSpriteAsset(pos, Lang.GetTranslate("Icon") + ":", editingDisplay.icon);
            if (editingDisplay.icon.Length > 3 && icon == " ")
            {
                item = new BuildObjectPrefab(editingDisplay);
                editingDisplay.icon = item.LoadIcon();
            }
            else
            {
                editingDisplay.icon = icon;
            }
             pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.skillLevelReq = ImagePack.DrawField(pos, Lang.GetTranslate("Skill Level Req")+":", editingDisplay.skillLevelReq);
            pos.y += ImagePack.fieldHeight;
            int selectedCategory = GetOptionPosition(editingDisplay.category, categoryIds);
            selectedCategory = ImagePack.DrawSelector(pos, Lang.GetTranslate("Category")+":", selectedCategory, categoryOptions);
            editingDisplay.category = categoryIds[selectedCategory];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.weaponReq = ImagePack.DrawSelector(pos, Lang.GetTranslate("Weapon Req")+":", editingDisplay.weaponReq, weaponTypeOptions);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.distanceReq = ImagePack.DrawField(pos, Lang.GetTranslate("Max Distance")+":", editingDisplay.distanceReq);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.buildTaskReqPlayer = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Build Task Req Player")+"?", editingDisplay.buildTaskReqPlayer);
            pos.y += ImagePack.fieldHeight;
           GUI.Label(pos, Lang.GetTranslate("Valid Claim Types")+":", ImagePack.FieldStyle());
          /*  Rect tempPosition = new Rect(pos.x + pos.width / 2, pos.y, pos.width / 2 - ImagePack.fieldMargin, pos.height - ImagePack.lineSpace);
      //   *   ClaimType currentClaimType = (ClaimType)editingDisplay.validClaimType;
         editingDisplay.validClaimType = EditorGUI.DrawField(tempPosition, editingDisplay.validClaimType, ImagePack.SelectorStyle());
           // editingDisplay.validClaimType = (int)currentClaimType;*/
            pos.y += ImagePack.fieldHeight;
            editingDisplay.availableFromItemOnly = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Available as Item Only")+"?", editingDisplay.availableFromItemOnly);

            // Interaction
            pos.y += ImagePack.fieldHeight;
            editingDisplay.interactionType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Interaction Type")+":", editingDisplay.interactionType, claimObjectInteractionTypeOptions);
            if (editingDisplay.interactionType == "Resource")
            {
                pos.y += ImagePack.fieldHeight;
                int selectedItem = GetOptionPosition(editingDisplay.interactionID, tableIds);
                selectedItem = ImagePack.DrawSelector(pos, Lang.GetTranslate("Loot Table")+":", selectedItem, tablesList);
                editingDisplay.interactionID = tableIds[selectedItem];
            }
            else if (editingDisplay.interactionType == "NPC")
            {
                pos.y += ImagePack.fieldHeight;
                int selectedMob = GetOptionPosition(editingDisplay.interactionID, mobIds);
                selectedMob = ImagePack.DrawSelector(pos, Lang.GetTranslate("Mob Template")+":", selectedMob, mobList);
                editingDisplay.interactionID = mobIds[selectedMob];
            }
            else if (editingDisplay.interactionType == "Chest")
            {
                pos.y += ImagePack.fieldHeight;
                editingDisplay.interactionID = ImagePack.DrawField(pos, Lang.GetTranslate("Number of Slots")+":", editingDisplay.interactionID);
            }
            else if (editingDisplay.interactionType == "Effect")
            {
                pos.y += ImagePack.fieldHeight;
                int selectedEffect = GetOptionPosition(editingDisplay.interactionID, effectIds);
                selectedEffect = ImagePack.DrawSelector(pos, Lang.GetTranslate("Effect")+":", selectedEffect, effectOptions);
                editingDisplay.interactionID = effectIds[selectedEffect];
            }
            else if (editingDisplay.interactionType == "Instance")
            {
                pos.y += ImagePack.fieldHeight;
                int selectedInstance = GetOptionPosition(editingDisplay.interactionID, instanceIds);
                selectedInstance = ImagePack.DrawSelector(pos, "Instance:", selectedInstance, instanceOptions);
                editingDisplay.interactionID = instanceIds[selectedInstance];
            }

            // Loop through stages
            if (editingDisplay.stages.Count == 0)
            {
                editingDisplay.stages.Add(new BuildObjectStage());
            }
            for (int i = 0; i < editingDisplay.stages.Count; i++)
            {
                pos.y += 1.5f * ImagePack.fieldHeight;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Stage")+" " + (i + 1) + ":");
                pos.y += ImagePack.fieldHeight;
                pos.width = pos.width * 2;
                editingDisplay.stages[i].gameObject = ImagePack.DrawGameObject(pos, Lang.GetTranslate("Prefab")+": ", editingDisplay.stages[i].gameObject, 0.75f);
                pos.width /= 2;
                pos.y += ImagePack.fieldHeight;
                editingDisplay.stages[i].buildTimeReq = ImagePack.DrawField(pos, Lang.GetTranslate("Time to Build")+":", editingDisplay.stages[i].buildTimeReq);
                pos.y += 1.5f * ImagePack.fieldHeight;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Items Required")+":");
                pos.y += ImagePack.fieldHeight;
                for (int j = 0; j < editingDisplay.stages[i].maxEntries; j++)
                {
                    if (editingDisplay.stages[i].entries.Count <= j)
                        editingDisplay.stages[i].entries.Add(new BuildObjectItemEntry(-1, 0));
                    int selectedItem = GetOptionPosition(editingDisplay.stages[i].entries[j].itemId, itemIds);
                    selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item")+" " + (j + 1) + ":", ref editingDisplay.stages[i].entries[j].itemSearch, selectedItem, itemsList);
                    editingDisplay.stages[i].entries[j].itemId = itemIds[selectedItem];
                    //pos.x += pos.width;
                    pos.y += ImagePack.fieldHeight;
                    editingDisplay.stages[i].entries[j].count = ImagePack.DrawField(pos, Lang.GetTranslate("Item")+" " + (j + 1) + " "+ Lang.GetTranslate("Count")+":", editingDisplay.stages[i].entries[j].count);
                    //pos.x -= pos.width;
                    pos.y += ImagePack.fieldHeight;
                }

                if (editingDisplay.stages.Count > 1)
                {
                    pos.x += pos.width;
                    if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove Stage")))
                    {
                        if (editingDisplay.stages[i].id > 0)
                            editingDisplay.stagesToBeDeleted.Add(editingDisplay.stages[i].id);
                        editingDisplay.stages.RemoveAt(i);
                    }
                    pos.x -= pos.width;
                }
            }
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Stage")))
            {
                editingDisplay.stages.Add(new BuildObjectStage());
            }

            pos.width = pos.width * 2;

            // Save data		
            pos.x -= ImagePack.innerMargin;
            pos.y += 1.4f * ImagePack.fieldHeight;
            pos.width /= 3;
            showSave = true;
            if (!newEntity)
            {
                showDelete = true;
            }
            else
            {
                showDelete = false;
            }
            showCancel = true;
            if (resultTimeout != -1 && resultTimeout < Time.realtimeSinceStartup && result != "")
            {
                result = "";
            }

            if (!newEntity)
                EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight + 140);
            else
                EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight);
        }


        public override void save()
        {
            if (newEntity)
                InsertEntry();
            else
                UpdateEntry();

            state = State.Loaded;
            resetSearch(true);
        }

        public override void delete()
        {
            if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete")+" " + Lang.GetTranslate(functionName) + " "+ Lang.GetTranslate("entry")+" ?", Lang.GetTranslate("Are you sure you want to delete")+" " + editingDisplay.Name + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
            {
                DeleteEntry();
                newSelectedDisplay = 0;
                state = State.Loaded;
            }
            resetSearch(true);
        }

        public override void cancel()
        {
            resetSearch(true);
            if (selectedDisplay > -1)
                editingDisplay = LoadEntity(displayKeys[selectedDisplay]);
            else
                editingDisplay = LoadEntity(selectedDisplay);
            if (newEntity)
                state = State.New;
            else
                state = State.Loaded;
        }

        // Insert new entries into the table
        void InsertEntry()
        {
            NewResult(Lang.GetTranslate("Inserting..."));
            // Insert the items - reverse the order so the next Ids can be set
            for (int i = editingDisplay.stages.Count - 1; i >= 0; i--)
            {
                InsertStage(editingDisplay.stages[i]);
                if (i > 0)
                {
                    editingDisplay.stages[i - 1].nextStage = editingDisplay.stages[i].id;
                }
            }
            // Set the first stage ID 
            editingDisplay.firstStageID = editingDisplay.stages[0].id;

            // Setup the update query
            string query = "INSERT INTO " + tableName;
            query += " (" + editingDisplay.FieldList("", ", ") + ") ";
            query += "VALUES ";
            query += " (" + editingDisplay.FieldList("?", ", ") + ") ";

            int mobID = -1;

            // Setup the register data
            List<Register> update = new List<Register>();
            foreach (string field in editingDisplay.fields.Keys)
            {
                update.Add(editingDisplay.fieldToRegister(field));
            }

            // Update the database
            mobID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);

            // If the insert failed, don't insert the stages
            if (mobID != -1)
            {
                // Update online table to avoid access the database again			
                editingDisplay.id = mobID;
                editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + mobID + "ID2:" + editingDisplay.id);
                CreatePrefab();
                dataLoaded = false;
                Load();
                newItemCreated = true;

                // Configure the correponding prefab
                 NewResult(Lang.GetTranslate("New entry inserted"));
            }
            else
            {
                NewResult(Lang.GetTranslate("Error occurred, please check the Console"));
            }
        }

        void InsertStage(BuildObjectStage entry)
        {
            string query = "INSERT INTO build_object_stage";
            query += " (" + entry.FieldList("", ", ") + ") ";
            query += "VALUES ";
            query += " (" + entry.FieldList("?", ", ") + ") ";

            // Setup the register data
            List<Register> update = new List<Register>();
            foreach (string field in entry.fields.Keys)
            {
                update.Add(entry.fieldToRegister(field));
            }

            entry.id = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);
        }

        // Update existing entries in the table based on the iddemo_table
        void UpdateEntry()
        {
            NewResult(Lang.GetTranslate("Updating..."));
            // Insert/Update the items - reverse the order so the next Ids can be set
            for (int i = editingDisplay.stages.Count - 1; i >= 0; i--)
            {
                if (editingDisplay.stages[i].id < 1)
                {
                    InsertStage(editingDisplay.stages[i]);
                }
                else
                {
                    UpdateStage(editingDisplay.stages[i]);
                }
                if (i > 0)
                {
                    editingDisplay.stages[i - 1].nextStage = editingDisplay.stages[i].id;
                }
            }

            // Set the first stage ID 
            editingDisplay.firstStageID = editingDisplay.stages[0].id;

            // Setup the update query
            string query = "UPDATE " + tableName;
            query += " SET ";
            query += editingDisplay.UpdateList();
            query += " WHERE id=?id";

            // Setup the register data		
            List<Register> update = new List<Register>();
            foreach (string field in editingDisplay.fields.Keys)
            {
                update.Add(editingDisplay.fieldToRegister(field));
            }
            update.Add(new Register("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString(), Register.TypesOfField.Int));

            // Update the database
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);

            // Update online table to avoid access the database again			
          
            // Delete any stages that are tagged for deletion
            foreach (int stageID in editingDisplay.stagesToBeDeleted)
            {
                DeleteStage(stageID);
            }

            // Configure the correponding prefab
            CreatePrefab();
            dataLoaded = false;
             NewResult(Lang.GetTranslate("Entry")+"  " + editingDisplay.Name + " "+ Lang.GetTranslate("updated"));
            Load();
       }

        void UpdateStage(BuildObjectStage entry)
        {
            string query = "UPDATE build_object_stage";
            query += " SET ";
            query += entry.UpdateList();
            query += " WHERE id=?id";

            // Setup the register data
            List<Register> update = new List<Register>();
            foreach (string field in entry.fields.Keys)
            {
                update.Add(entry.fieldToRegister(field));
            }
            update.Add(new Register("id", "?id", MySqlDbType.Int32, entry.id.ToString(), Register.TypesOfField.Int));

            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);
        }

        // Delete entries from the table
        void DeleteEntry()
        {
            // Remove the prefab
            DeletePrefab();

            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, tableName, delete);
            string query = "UPDATE " + tableName + " SET isactive = 0 where id = " + editingDisplay.id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            // Update online table to avoid access the database again			
            // Update online table to avoid access the database again		
            selectedDisplay = -1;
            newSelectedDisplay = 0;
            dataLoaded = false;
              NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " "+ Lang.GetTranslate("Deleted"));
          Load();
        }

        void DeleteStage(int stageID)
        {
            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, stageID.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "build_object_stage", delete);
            string query = "UPDATE build_object_stage SET isactive = 0 where id = " + stageID;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
        }

        void DeleteForever(int id)
        {
            if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete") + " " + Lang.GetTranslate(functionName) + " " + Lang.GetTranslate("entry") + " ?", Lang.GetTranslate("Are you sure you want to permanent delete") + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
            {
                NewResult(Lang.GetTranslate("Deleting..."));

               editingDisplay = LoadEntity(id);
                List<Register> where = new List<Register>();
                for (int i = editingDisplay.stages.Count - 1; i >= 0; i--)
                {
                    where.Clear();
                    where.Add(new Register("id", "?id", MySqlDbType.Int32, editingDisplay.stages[i].id.ToString(), Register.TypesOfField.Int));
                    DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "build_object_stage", where);
                }


                where.Clear();
                where.Add(new Register("id", "?id", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, tableName, where);
                editingDisplay = null;
                dataLoaded = false;
                dataRestoreLoaded = false;
                // Load();
                NewResult(Lang.GetTranslate("Entry Permanent Deleted"));
            }
        }


        void RestoreEntry(int id)
        {
            NewResult(Lang.GetTranslate("Restoring..."));
            string query = "UPDATE " + tableName + " SET isactive = 1 where id = " + id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());


            dataLoaded = false;
            dataRestoreLoaded = false;
            //Load();
            editingDisplay = LoadEntity(id);
            CreatePrefab();
            NewResult(Lang.GetTranslate("Entry Restored"));
        }
        void Duplicate()
        {
            // Update name of current entry by adding "1" to the end
            editingDisplay.Name = editingDisplay.Name + " (Clone)";

            InsertEntry();
            state = State.Loaded;
        }

        void GenerateAllPrefabs()
        {
       /*     if (!dataLoaded)
            {
                Load();
            }

            // Delete all existing prefabs?
            BuildObjectPrefab.DeleteAllPrefabs();
            foreach (int id in displayKeys)
            {
                BuildObjectData boData = LoadEntity(id);
                BuildObjectPrefab prefab = new BuildObjectPrefab(boData);
                prefab.Save(boData);
           
            }*/
        }

        void CreatePrefab()
        {
       /*     // Configure the correponding prefab
            item = new BuildObjectPrefab(editingDisplay);
            item.Save(editingDisplay);*/
        }

        void DeletePrefab()
        {
         /*   item = new BuildObjectPrefab(editingDisplay);

            if (item.Load())
                item.Delete();*/
        }

    }
}