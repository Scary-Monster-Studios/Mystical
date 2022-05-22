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
    // Handles the Skills Configuration
    public class ServerSkills : AtavismDatabaseFunction
    {

        public new Dictionary<int, SkillsData> dataRegister;
        public new SkillsData editingDisplay;
        public new SkillsData originalDisplay;
        public int[] abilityIds = new int[] { -1 };
        public string[] abilityOptions = new string[] { "~ none ~" };
        public static int[] skillIds = new int[] { -1 };
        public static string[] skillOptions = new string[] { "~ none ~" };
        public string[] statOptions = new string[] { "~ none ~" };
        public int[] skillProfileIds = new int[] { -1 };
        public string[] skillProfileOptions = new string[] { "~ none ~" };

        public int[] classIds = new int[] { -1 };
        public string[] classOptions = new string[] { "~ none ~" };
        string skillPlrofileSearch = "";
        // Handles the prefab creation, editing and save
        private SkillPrefab prefab = null;
        public static string[] skillType = new string[] { "Crafting", "Combat", "Gathering" };
        int type = 0;
        // Simulated the auto-increment on tables without it - Not used
        //private int autoID = 1;

        // Use this for initialization
        public ServerSkills()
        {
            functionName = "Skills";
            // Database tables name
            tableName = "skills";
            functionTitle = "Skills Configuration";
            loadButtonLabel ="Load Skills";
            notLoadedText = "No Skill loaded.";
            // Init
            dataRegister = new Dictionary<int, SkillsData>();

            editingDisplay = new SkillsData();
            originalDisplay = new SkillsData();
            skillPlrofileSearch = "";

        }
        void resetSearch(bool fokus)
        {
            skillPlrofileSearch = "";
           if(fokus) GUI.FocusControl(null);
        }
        public override void Activate()
        {
            linkedTablesLoaded = false;
            resetSearch(true);
        }

        public void LoadAbilityOptions()
        {
            string query = "SELECT id, name FROM abilities where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                abilityOptions = new string[rows.Count + 1];
                abilityOptions[optionsId] = "~ none ~";
                abilityIds = new int[rows.Count + 1];
                abilityIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    abilityOptions[optionsId] = data["id"] + ":" + data["name"];
                    abilityIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        public static void LoadSkillOptions()
        {
            string query = "SELECT id, name FROM skills where isactive = 1";



            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
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

        public void LoadStatOptions()
        {
            
                // Read all entries from the table
                string query = "SELECT name FROM stat where isactive = 1";

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
                    statOptions = new string[rows.Count + 1];
                    statOptions[optionsId] = "~ none ~";
                    foreach (Dictionary<string, string> data in rows)
                    {
                        optionsId++;
                        statOptions[optionsId] = data["name"];
                    }
                }
           
        }


        public void LoadProfileOptions(int type)
        {

            // Read all entries from the table
            string query = "SELECT id , profile_name FROM skill_profile where isactive = 1 AND type = " + type;
            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();
            skillProfileOptions = new string[1];
            skillProfileOptions[0] = "~ none ~";
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                skillProfileOptions = new string[rows.Count + 1];
                skillProfileOptions[optionsId] = "~ none ~";
                skillProfileIds = new int[rows.Count + 1];
                skillProfileIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    skillProfileOptions[optionsId] = data["id"] + ":" + data["profile_name"];
                    skillProfileIds[optionsId] = int.Parse(data["id"]);
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

        SkillsData LoadEntity(int id)
        {

            // Read all entries from the table
            string query = "SELECT " + originalDisplay.GetFieldsString() + " FROM " + tableName + " where isactive = 1 AND id=" + id;

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            SkillsData display = new SkillsData();
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
                    display.type = int.Parse(data["type"]);
                    display.aspect = int.Parse(data["aspect"]);
                    display.oppositeAspect = int.Parse(data["oppositeAspect"]);
                    display.mainAspectOnly = bool.Parse(data["mainAspectOnly"]);
                    display.primaryStat = data["primaryStat"];
                    display.secondaryStat = data["secondaryStat"];
                    display.thirdStat = data["thirdStat"];
                    display.fourthStat = data["fourthStat"];
                    display.maxLevel = int.Parse(data["maxLevel"]);
                    display.automaticallyLearn = bool.Parse(data["automaticallyLearn"]);
                    display.skillPointCost = int.Parse(data["skillPointCost"]);
                    display.parentSkill = int.Parse(data["parentSkill"]);
                    display.parentSkillLevelReq = int.Parse(data["parentSkillLevelReq"]);
                    display.prereqSkill1 = int.Parse(data["prereqSkill1"]);
                    display.prereqSkill1Level = int.Parse(data["prereqSkill1Level"]);
                    display.prereqSkill2 = int.Parse(data["prereqSkill2"]);
                    display.prereqSkill2Level = int.Parse(data["prereqSkill2Level"]);
                    display.prereqSkill3 = int.Parse(data["prereqSkill3"]);
                    display.prereqSkill3Level = int.Parse(data["prereqSkill3Level"]);
                    display.playerLevelReq = int.Parse(data["playerLevelReq"]);
                    display.skillProfile = int.Parse(data["skill_profile_id"]);
                    display.talent = bool.Parse(data["talent"]);

                    display.isLoaded = true;
                    //Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
                }
            }
            LoadSkillAbilities(display);
            return display;
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

        void LoadSkillAbilities(SkillsData skillData)
        {
            // Read all entries from the table
            string query = "SELECT " + new SkillAbilityEntry().GetFieldsString() + " FROM skill_ability_gain where skillID = " + skillData.id + " AND isactive = 1";

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
                    SkillAbilityEntry entry = new SkillAbilityEntry();

                    entry.id = int.Parse(data["id"]);
                    entry.skillLevelReq = int.Parse(data["skillLevelReq"]);
                    entry.abilityID = int.Parse(data["abilityID"]);
                    entry.automaticallyLearn = bool.Parse(data["automaticallyLearn"]);
                    skillData.skillAbilities.Add(entry);
                }
            }
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create a Skill before editing it."));
                return;
            }

            // Draw the content database info
           // Debug.LogError(Lang.GetTranslate("Edit Skill"));
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Skill"));

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
                resetSearch(false);
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
           /*     if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Generate Prefabs")))
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Skill Properties")+":");
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You dont have deleted")+" " + Lang.GetTranslate(functionName) + ".");
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
            editingDisplay = new SkillsData();
            originalDisplay = new SkillsData();
            selectedDisplay = -1;
        }

        // Edit or Create
        public override void DrawEditor(Rect box, bool nItem)
        {
            newEntity = nItem;
            // Setup the layout
            Rect pos = box;
            pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;

            if (!linkedTablesLoaded)
            {
                LoadAbilityOptions();
                LoadSkillOptions();
                LoadProfileOptions(type);
                LoadStatOptions();
                ServerOptionChoices.LoadAtavismChoiceOptions("Class", true, out classIds, out classOptions);
                linkedTablesLoaded = true;
            }

            // Draw the content database info
            //pos.y += ImagePack.fieldHeight;

            if (newEntity)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new Skill"));
                pos.y += ImagePack.fieldHeight;
            }
            if (editingDisplay == null)
                return;
            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name") +":", editingDisplay.Name, 0.75f);
            pos.y += ImagePack.fieldHeight;
            pos.width /= 2;
            editingDisplay.maxLevel = ImagePack.DrawField(pos, Lang.GetTranslate("Max Level")+":", editingDisplay.maxLevel);
            pos.x += pos.width;
            string icon = ImagePack.DrawSpriteAsset(pos, Lang.GetTranslate("Icon") + ":", editingDisplay.icon);
         /*   if (editingDisplay.icon.Length > 3 && icon == " ")
            {
                SkillPrefab item = new SkillPrefab(editingDisplay);
                editingDisplay.icon = item.LoadIcon();
            }
            else
            {*/
                editingDisplay.icon = icon;
      //      }
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.skillPointCost = ImagePack.DrawField(pos, Lang.GetTranslate("Skill Point Cost")+":", editingDisplay.skillPointCost);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.automaticallyLearn = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Automatically Learn") + ":", editingDisplay.automaticallyLearn);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.talent = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Talent") + ":", editingDisplay.talent);
            pos.y += ImagePack.fieldHeight;
            if (type != editingDisplay.type)
                LoadProfileOptions(editingDisplay.type);
            type = editingDisplay.type;
            editingDisplay.type = ImagePack.DrawSelector(pos, Lang.GetTranslate("Skill Type")+":", editingDisplay.type, skillType);
            if (type != editingDisplay.type)
                LoadProfileOptions(editingDisplay.type);
            pos.y += ImagePack.fieldHeight;
            int selectedProfile = GetPositionOfSkillProfile(editingDisplay.skillProfile);
            selectedProfile = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Skill Profile")+":", ref skillPlrofileSearch, selectedProfile, skillProfileOptions);
            editingDisplay.skillProfile = skillProfileIds[selectedProfile];
            pos.y += ImagePack.fieldHeight;

            int selectedClass = GetOptionPosition(editingDisplay.aspect, classIds);
            selectedClass = ImagePack.DrawSelector(pos, Lang.GetTranslate("Skill Class")+":", selectedClass, classOptions);
            editingDisplay.aspect = classIds[selectedClass];
            pos.x += pos.width;
            editingDisplay.mainAspectOnly = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Only this class")+"?", editingDisplay.mainAspectOnly);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedClass = GetOptionPosition(editingDisplay.oppositeAspect, classIds);
            selectedClass = ImagePack.DrawSelector(pos, Lang.GetTranslate("Opposite Class")+":", selectedClass, classOptions);
            editingDisplay.oppositeAspect = classIds[selectedClass];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.primaryStat = ImagePack.DrawSelector(pos, Lang.GetTranslate("Primary Stat")+":", editingDisplay.primaryStat, statOptions);
            pos.x += pos.width;
            editingDisplay.secondaryStat = ImagePack.DrawSelector(pos, Lang.GetTranslate("Secondary Stat")+":", editingDisplay.secondaryStat, statOptions);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.thirdStat = ImagePack.DrawSelector(pos, Lang.GetTranslate("Third Stat")+":", editingDisplay.thirdStat, statOptions);
            pos.x += pos.width;
            editingDisplay.fourthStat = ImagePack.DrawSelector(pos, Lang.GetTranslate("Fourth Stat")+":", editingDisplay.fourthStat, statOptions);
            pos.x -= pos.width;
            pos.y += 1.5f * ImagePack.fieldHeight;
            // Requirements
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Requirements"));
            pos.y += ImagePack.fieldHeight;
            int selectedSkill = GetPositionOfSkill(editingDisplay.parentSkill);
            selectedSkill = ImagePack.DrawSelector(pos, Lang.GetTranslate("Parent Skill")+":", selectedSkill, skillOptions);
            editingDisplay.parentSkill = skillIds[selectedSkill];
            pos.x += pos.width;
            editingDisplay.parentSkillLevelReq = ImagePack.DrawField(pos, Lang.GetTranslate("Skill Level Req")+":", editingDisplay.parentSkillLevelReq);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            // prereq 1
            selectedSkill = GetPositionOfSkill(editingDisplay.prereqSkill1);
            selectedSkill = ImagePack.DrawSelector(pos, Lang.GetTranslate("Prereq Skill")+" 1:", selectedSkill, skillOptions);
            editingDisplay.prereqSkill1 = skillIds[selectedSkill];
            pos.x += pos.width;
            editingDisplay.prereqSkill1Level = ImagePack.DrawField(pos, Lang.GetTranslate("Skill Level Req")+":", editingDisplay.prereqSkill1Level);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            // prereq 2
            selectedSkill = GetPositionOfSkill(editingDisplay.prereqSkill2);
            selectedSkill = ImagePack.DrawSelector(pos, Lang.GetTranslate("Prereq Skill")+" 2:", selectedSkill, skillOptions);
            editingDisplay.prereqSkill2 = skillIds[selectedSkill];
            pos.x += pos.width;
            editingDisplay.prereqSkill2Level = ImagePack.DrawField(pos, Lang.GetTranslate("Skill Level Req")+":", editingDisplay.prereqSkill2Level);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            // prereq 3
            selectedSkill = GetPositionOfSkill(editingDisplay.prereqSkill3);
            selectedSkill = ImagePack.DrawSelector(pos, Lang.GetTranslate("Prereq Skill")+" 3:", selectedSkill, skillOptions);
            editingDisplay.prereqSkill3 = skillIds[selectedSkill];
            pos.x += pos.width;
            editingDisplay.prereqSkill3Level = ImagePack.DrawField(pos, Lang.GetTranslate("Skill Level Req")+":", editingDisplay.prereqSkill3Level);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.playerLevelReq = ImagePack.DrawField(pos, Lang.GetTranslate("Player Level Req")+":", editingDisplay.playerLevelReq);
            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Skill Abilities"));
            pos.y += 1.5f * ImagePack.fieldHeight;
          /*  if (editingDisplay.skillAbilities.Count == 0)
            {
                editingDisplay.skillAbilities.Add(new SkillAbilityEntry(0, -1));
            }*/
            for (int i = 0; i < editingDisplay.skillAbilities.Count; i++)
            {
                editingDisplay.skillAbilities[i].skillLevelReq = ImagePack.DrawField(pos, Lang.GetTranslate("Skill Level")+":", editingDisplay.skillAbilities[i].skillLevelReq);
                pos.x += pos.width;
                editingDisplay.skillAbilities[i].automaticallyLearn = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Automatically Learn")+":", editingDisplay.skillAbilities[i].automaticallyLearn);
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                int selectedAbility = GetPositionOfAbility(editingDisplay.skillAbilities[i].abilityID);
                selectedAbility = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Ability")+" " + (i + 1) + ":", ref editingDisplay.skillAbilities[i].abilitySearch, selectedAbility, abilityOptions);
                editingDisplay.skillAbilities[i].abilityID = abilityIds[selectedAbility];
                pos.y += ImagePack.fieldHeight;
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove Ability")))
                {
                    if (editingDisplay.skillAbilities[i].id > 0)
                        editingDisplay.abilitiesToBeDeleted.Add(editingDisplay.skillAbilities[i].id);
                    editingDisplay.skillAbilities.RemoveAt(i);
                }
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
            }
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Ability")))
            {
                editingDisplay.skillAbilities.Add(new SkillAbilityEntry(0, -1));
            }

            pos.width *= 2;
            pos.y += 1.4f * ImagePack.fieldHeight;
            // Save data
            pos.x -= ImagePack.innerMargin;
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
            // Setup the update query
            string query = "INSERT INTO " + tableName;
            query += " (" + editingDisplay.FieldList("", ", ") + ") ";
            query += "VALUES ";
            query += " (" + editingDisplay.FieldList("?", ", ") + ") ";

            int itemID = -1;

            // Setup the register data		
            List<Register> update = new List<Register>();
            foreach (string field in editingDisplay.fields.Keys)
            {
                update.Add(editingDisplay.fieldToRegister(field));
            }

            // Update the database
            itemID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);

            // If the insert failed, don't insert the spawn marker
            if (itemID != -1)
            {
                editingDisplay.id = itemID;
                // Insert the abilities
                foreach (SkillAbilityEntry entry in editingDisplay.skillAbilities)
                {
                    if (entry.abilityID != -1)
                    {
                        entry.skillID = itemID;
                        InsertAbility(entry);
                    }
                }

                // Update online table to avoid access the database again			
                //editingDisplay.id = itemID;
                editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
                CreatePrefab();

                dataLoaded = false;
                Load();
                newItemCreated = true;
                // Configure the correponding prefab
                LoadSkillOptions();
               // LoadProfileOptions();

                NewResult(Lang.GetTranslate("New entry inserted"));
            }
            else
            {
                NewResult("Error:"+Lang.GetTranslate("Error occurred, please check the Console"));
            }
        }

        void InsertAbility(SkillAbilityEntry entry)
        {
            string query = "INSERT INTO skill_ability_gain";
            query += " (" + entry.FieldList("", ", ") + ") ";
            query += "VALUES ";
            query += " (" + entry.FieldList("?", ", ") + ") ";

            // Setup the register data		
            List<Register> update = new List<Register>();
            foreach (string field in entry.fields.Keys)
            {
                update.Add(entry.fieldToRegister(field));
            }

            int itemID = -1;
            itemID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);

            entry.id = itemID;
        }

        // Update existing entries in the table based on the iddemo_table
        void UpdateEntry()
        {
            NewResult(Lang.GetTranslate("Updating..."));
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

            // Insert/Update the abilities
            foreach (SkillAbilityEntry entry in editingDisplay.skillAbilities)
            {
                if (entry.abilityID != -1)
                {
                    if (entry.id < 1)
                    {
                        // This is a new entry, insert it
                        entry.skillID = editingDisplay.id;
                        InsertAbility(entry);
                    }
                    else
                    {
                        // This is an existing entry, update it
                        entry.skillID = editingDisplay.id;
                        UpdateAbility(entry);
                    }
                }
            }

            // Delete any abilities that are tagged for deletion
            foreach (int abilityID in editingDisplay.abilitiesToBeDeleted)
            {
                DeleteAbility(abilityID);
            }

        
            // Configure the correponding prefab
            CreatePrefab();
            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry")+"  " + editingDisplay.Name + " "+ Lang.GetTranslate("updated"));
            Load();
        }

        void UpdateAbility(SkillAbilityEntry entry)
        {
            string query = "UPDATE skill_ability_gain";
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

        void DeleteAbility(int abilityID)
        {
            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, abilityID.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "skill_ability_gain", delete);
            string query = "UPDATE skill_ability_gain SET isactive = 0 where id = " + abilityID;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
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

            // Delete the ability links
            //delete = new Register ("skillID", "?skillID", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "skill_ability_gain", delete);
            query = "UPDATE skill_ability_gain SET isactive = 0 where skillID = " + editingDisplay.id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            // Update online table to avoid access the database again		
            selectedDisplay = -1;
            newSelectedDisplay = 0;
            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " "+ Lang.GetTranslate("Deleted"));
            Load();

            LoadSkillOptions();
          //  LoadProfileOptions();

        }
        void DeleteForever(int id)
        {
            if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete") + " " + Lang.GetTranslate(functionName) + " " + Lang.GetTranslate("entry") + " ?", Lang.GetTranslate("Are you sure you want to permanent delete") + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
            {
                NewResult(Lang.GetTranslate("Deleting..."));
                List<Register> where = new List<Register>();
                where.Add(new Register("id", "?id", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, tableName, where);

                where.Clear();
                where.Add(new Register("skillID", "?skillID", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "skill_ability_gain", where);

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
            query = "UPDATE skill_ability_gain SET isactive = 1 where skillID = " + id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
            editingDisplay = LoadEntity(id);
            CreatePrefab();
            dataLoaded = false;
            dataRestoreLoaded = false;
            //Load();
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
         /*   if (!dataLoaded)
            {
                Load();
            }

            // Delete all existing prefabs?
        //    SkillPrefab.DeleteAllPrefabs();

            foreach (int id in displayKeys)
            {
                SkillsData skillData = LoadEntity(id);
                prefab = new SkillPrefab(skillData);
                prefab.Save(skillData);
            }
            SkillPrefab.DeletePrefabWithoutIDs(displayKeys);*/
        }

        void CreatePrefab()
        {
            // Configure the correponding prefab
         //   prefab = new SkillPrefab(editingDisplay);
         //   prefab.Save(editingDisplay);
        }

        void DeletePrefab()
        {
         /*   prefab = new SkillPrefab(editingDisplay);

            if (prefab.Load())
                prefab.Delete();*/
        }

        private int GetPositionOfAbility(int abilityID)
        {
            for (int i = 0; i < abilityIds.Length; i++)
            {
                if (abilityIds[i] == abilityID)
                    return i;
            }
            return 0;
        }

        private int GetPositionOfSkill(int skillID)
        {
            for (int i = 0; i < skillIds.Length; i++)
            {
                if (skillIds[i] == skillID)
                    return i;
            }
            return 0;
        }
        private int GetPositionOfSkillProfile(int profileID)
        {
            for (int i = 0; i < skillProfileIds.Length; i++)
            {
                if (skillProfileIds[i] == profileID)
                    return i;
            }
            return 0;
        }
    }
}