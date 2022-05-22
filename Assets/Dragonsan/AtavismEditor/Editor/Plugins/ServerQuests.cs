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
    // Handles the Quests Configuration
    public class ServerQuests : AtavismDatabaseFunction
    {

        public new Dictionary<int, QuestsData> dataRegister;
        public new QuestsData editingDisplay;
        public new QuestsData originalDisplay;

        public int[] factionIds = new int[] { -1 };
        public string[] factionOptions = new string[] { "~ none ~" };

        public int[] itemIds = new int[] { -1 };
        public string[] itemsList = new string[] { "~ none ~" };

        public int[] currencyIds = new int[] { -1 };
        public string[] currencyOptions = new string[] { "~ none ~" };

        public static int[] questIds = new int[] { -1 };
        public static string[] questOptions = new string[] { "~ none ~" };
        public static GUIContent[] GuiQuestOptions = new GUIContent[] { new GUIContent("~ none ~") };

        public int[] raceIds = new int[] { -1 };
        public string[] raceOptions = new string[] { "~ none ~" };

        public int[] classIds = new int[] { -1 };
        public string[] classOptions = new string[] { "~ none ~" };

        public int[] skillIds = new int[] { -1 };
        public string[] skillOptions = new string[] { "~ none ~" };

        public int[] mobIds = new int[] { -1 };
        public string[] mobList = new string[] { "~ none ~" };

        public int[] taskIds = new int[] { -1 };
        public string[] taskList = new string[] { "~ none ~" };

        public int[] requirementIds = new int[] { -1 };
        public string[] requirementOptions = new string[] { "~ none ~" };

        public string[] statOptions = new string[] { "~ none ~" };

        public string[] objectiveTypeOptions = new string[] { "~ none ~" };
        public List<string> mobSearch = new List<string>();
        public List<string> itemSearch = new List<string>();
        public List<string> cItemSearch = new List<string>();
        string itemGivenSearch1 = "";
        string itemGivenSearch2 = "";
        string itemGivenSearch3 = "";
        string itemSearch1 = "";
        string itemSearch2 = "";
        string itemSearch3 = "";
        string itemSearch4 = "";
        string itemSearch5 = "";
        string itemSearch6 = "";
        string itemSearch7 = "";
        string itemSearch8 = "";
        string itemChoisSearch1 = "";
        string itemChoisSearch2 = "";
        string itemChoisSearch3 = "";
        string itemChoisSearch4 = "";
        string itemChoisSearch5 = "";
        string itemChoisSearch6 = "";
        string itemChoisSearch7 = "";
        string itemChoisSearch8 = "";
        string questCompletedSearch = "";
        string questStartedSearch = "";

        Vector2 descriptionScroll = new Vector2();
        Vector2 objectiveScroll = new Vector2();
        Vector2 progressScroll = new Vector2();
        Vector2 completionScroll = new Vector2();


        // Use this for initialization
        public ServerQuests()
        {
            functionName = "Quests";
            // Database tables name
            tableName = "quests";
            functionTitle = "Quests Configuration";
            loadButtonLabel = "Load Quests";
            notLoadedText = "No Quest loaded.";
            // Init
            dataRegister = new Dictionary<int, QuestsData>();

            editingDisplay = new QuestsData();
            originalDisplay = new QuestsData();
            mobSearch = new List<string>();
            itemSearch = new List<string>();
            cItemSearch = new List<string>();
            itemGivenSearch1 = "";
            itemGivenSearch2 = "";
            itemGivenSearch3 = "";
            itemSearch1 = "";
            itemSearch2 = "";
            itemSearch3 = "";
            itemSearch4 = "";
            itemSearch5 = "";
            itemSearch6 = "";
            itemSearch7 = "";
            itemSearch8 = "";
            itemChoisSearch1 = "";
            itemChoisSearch2 = "";
            itemChoisSearch3 = "";
            itemChoisSearch4 = "";
            itemChoisSearch5 = "";
            itemChoisSearch6 = "";
            itemChoisSearch7 = "";
            itemChoisSearch8 = "";
            questCompletedSearch = "";
            questStartedSearch = "";
        }
        void resetSearch(bool fokus)
        {
            mobSearch.Clear();
            itemSearch.Clear();
            cItemSearch.Clear();
            itemGivenSearch1 = "";
            itemGivenSearch2 = "";
            itemGivenSearch3 = "";
            itemSearch1 = "";
            itemSearch2 = "";
            itemSearch3 = "";
            itemSearch4 = "";
            itemSearch5 = "";
            itemSearch6 = "";
            itemSearch7 = "";
            itemSearch8 = "";
            itemChoisSearch1 = "";
            itemChoisSearch2 = "";
            itemChoisSearch3 = "";
            itemChoisSearch4 = "";
            itemChoisSearch5 = "";
            itemChoisSearch6 = "";
            itemChoisSearch7 = "";
            itemChoisSearch8 = "";
            questCompletedSearch = "";
            questStartedSearch = "";
           if(fokus) GUI.FocusControl(null);
        }
        public override void Activate()
        {
            linkedTablesLoaded = false;
            resetSearch(true);
        }

        public void LoadFactionOptions()
        {
            string query = "SELECT id, name FROM factions where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                factionOptions = new string[rows.Count + 1];
                factionOptions[optionsId] = "~ none ~";
                factionIds = new int[rows.Count + 1];
                factionIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    factionOptions[optionsId] = data["id"] + ":" + data["name"];
                    factionIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        public static void LoadQuestOptions()
        {
            string query = "SELECT id, name FROM quests where isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                questOptions = new string[rows.Count + 1];
                questOptions[optionsId] = "~ none ~";
                questIds = new int[rows.Count + 1];
                questIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    questOptions[optionsId] = data["id"] + ":" + data["name"];
                    questIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        public static void LoadQuestOptions(bool gui)
        {
            if (!gui)
            {
                LoadQuestOptions();
                return;
            }
            string query = "SELECT id, name FROM quests where isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                GuiQuestOptions = new  GUIContent[rows.Count + 1];
                GuiQuestOptions[optionsId] = new GUIContent("~ none ~");
                questIds = new int[rows.Count + 1];
                questIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    GuiQuestOptions[optionsId] = new GUIContent(data["id"] + ":" + data["name"]);
                    questIds[optionsId] = int.Parse(data["id"]);
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

        public void LoadCurrencyOptions()
        {
            string query = "SELECT id, name FROM currencies where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                currencyOptions = new string[rows.Count + 1];
                currencyOptions[optionsId] = "~ none ~";
                currencyIds = new int[rows.Count + 1];
                currencyIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    currencyOptions[optionsId] = data["id"] + ":" + data["name"];
                    currencyIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        private void LoadMobList()
        {
            string query = "SELECT id, name FROM mob_templates where isactive = 1";

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

        private void LoadTaskList()
        {
            string query = "SELECT id, name FROM task where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                taskList = new string[rows.Count + 1];
                taskList[optionsId] = "~ none ~";
                taskIds = new int[rows.Count + 1];
                taskIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    taskList[optionsId] = data["id"] + ":" + data["name"];
                    taskIds[optionsId] = int.Parse(data["id"]);
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
            }
        }

        QuestsData LoadEntity(int id)
        {
            
                // Read all entries from the table
                string query = "SELECT " + originalDisplay.GetFieldsString() + " FROM " + tableName + " where isactive = 1 AND id ="+id;

                // If there is a row, clear it.
                if (rows != null)
                    rows.Clear();

                // Load data
                rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            QuestsData display = new QuestsData();
            if ((rows != null) && (rows.Count > 0))
                {
                    foreach (Dictionary<string, string> data in rows)
                    {
                        //foreach(string key in data.Keys)
                        //	Debug.Log("Name[" + key + "]:" + data[key]);
                        //return;
                      
                        display.id = int.Parse(data["id"]);
                        display.Name = data["name"];

                        display.category = int.Parse(data["category"]);
                        display.faction = int.Parse(data["faction"]);
                        display.chain = data["chain"];
                        display.level = int.Parse(data["level"]);
                        display.zone = data["zone"];
                        display.numGrades = int.Parse(data["numGrades"]);
                        display.repeatable = bool.Parse(data["repeatable"]);
                        display.description = data["description"];
                        display.objectiveText = data["objectiveText"];
                        display.progressText = data["progressText"];
                        display.deliveryItem1 = int.Parse(data["deliveryItem1"]);
                        display.deliveryItem2 = int.Parse(data["deliveryItem2"]);
                        display.deliveryItem3 = int.Parse(data["deliveryItem3"]);
                        display.questPrereq = int.Parse(data["questPrereq"]);
                        display.questStartedReq = int.Parse(data["questStartedReq"]);
                        display.completionText = data["completionText"];
                        display.experience = int.Parse(data["experience"]);
                    /*    display.item1 = int.Parse(data["item1"]);
                        display.item1count = int.Parse(data["item1count"]);
                        display.item2 = int.Parse(data["item2"]);
                        display.item2count = int.Parse(data["item2count"]);
                        display.item3 = int.Parse(data["item3"]);
                        display.item3count = int.Parse(data["item3count"]);
                        display.item4 = int.Parse(data["item4"]);
                        display.item4count = int.Parse(data["item4count"]);
                        display.chooseItem1 = int.Parse(data["chooseItem1"]);
                        display.chooseItem1count = int.Parse(data["chooseItem1count"]);
                        display.chooseItem2 = int.Parse(data["chooseItem2"]);
                        display.chooseItem2count = int.Parse(data["chooseItem2count"]);
                        display.chooseItem3 = int.Parse(data["chooseItem3"]);
                        display.chooseItem3count = int.Parse(data["chooseItem3count"]);
                        display.chooseItem4 = int.Parse(data["chooseItem4"]);
                        display.chooseItem4count = int.Parse(data["chooseItem4count"]);*/
                        display.currency = int.Parse(data["currency1"]);
                        display.currencyCount = int.Parse(data["currency1count"]);
                        display.currency2 = int.Parse(data["currency2"]);
                        display.currency2count = int.Parse(data["currency2count"]);
                        display.rep1 = int.Parse(data["rep1"]);
                        display.rep1gain = int.Parse(data["rep1gain"]);
                        display.rep2 = int.Parse(data["rep2"]);
                        display.rep2gain = int.Parse(data["rep2gain"]);

                        display.isLoaded = true;
                        //Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
                         }
                   
                }
                LoadQuestItems(display);
                LoadQuestObjectives(display);
                LoadQuestRequirements(display);
            return display;
        }

        void LoadQuestItems(QuestsData questData)
        {
            // Read all entries from the table
            string query = "SELECT * FROM quest_items where quest_id = " + questData.id;

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            Char delimiter = ';';
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    QuestItemEntry entry = new QuestItemEntry();
                    entry.id = int.Parse(data["id"]);
                    entry.questID = int.Parse(data["quest_id"]);
                    entry.itemId = int.Parse(data["item"]);
                    entry.count = int.Parse(data["count"]);
                    bool choose = bool.Parse(data["choose"]);
                    if (choose)
                    {
                        questData.chooseItems.Add(entry);
                    }
                    else
                    {
                        questData.items.Add(entry);
                    }
                }
            }
        }

        void LoadQuestObjectives(QuestsData questData)
        {
            // Read all entries from the table
            string query = "SELECT " + new QuestsObjectivesData().GetFieldsString() + " FROM quest_objectives where questID = " + questData.id + " AND isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            Char delimiter = ';';
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    QuestsObjectivesData entry = new QuestsObjectivesData();

                    entry.id = int.Parse(data["id"]);
                    entry.objectiveType = data["objectiveType"];
                    entry.target = int.Parse(data["target"]);
                    entry.targetCount = int.Parse(data["targetCount"]);
                    entry.targetText = data["targetText"];
                    if (data["targets"].Length > 0)
                    {
                        string[] splitted = data["targets"].Split(delimiter);
                        for (int i = 0; i < splitted.Length; i++)
                        {
                            entry.targetsList.Add(int.Parse(splitted[i]));
                            entry.targetsSearch.Add("");

                        }
                    }
                    questData.questObjectives.Add(entry);
                }
            }
        }

        void LoadQuestRequirements(QuestsData questData)
        {
            // Read all entries from the table
            string query = "SELECT " + new QuestRequirementEntry().GetFieldsString() + " FROM quest_requirement where quest_id = "
                + questData.id + " AND isactive = 1";

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
                    QuestRequirementEntry display = new QuestRequirementEntry();
                    display.id = int.Parse(data["id"]);
                    display.editor_option_type_id = int.Parse(data["editor_option_type_id"]);
                    display.editor_option_choice_type_id = data["editor_option_choice_type_id"];
                    display.required_value = int.Parse(data["required_value"]);
                    questData.questRequirements.Add(display);
                }
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


        // Draw the loaded list
        public override void DrawLoaded(Rect box)
        {

            breadCrumb = "";

            // Setup the layout
            Rect pos = box;
            pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;

            if (displayList.Count <= 0)
            {
                pos.y += ImagePack.fieldHeight;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create a Quest before edit it."));
                return;
            }

            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Quest"));

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
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Duplicate")))
                {
                    Duplicate();
                }

                pos.width *= 2;
                pos.y += ImagePack.fieldHeight * 1.5f;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Quest Properties")+":");
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
            editingDisplay = new QuestsData();
            originalDisplay = new QuestsData();
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
                //AtavismLogger.LogInfoMessage("Quest param loading");
                LoadFactionOptions();
                LoadQuestOptions();
                LoadSkillOptions();
                LoadItemList();
                LoadCurrencyOptions();
                LoadMobList();
                LoadTaskList();
                LoadStatOptions();
                objectiveTypeOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Quest Objective Type", false);
                ServerOptionChoices.LoadAtavismChoiceOptions("Requirement", false, out requirementIds, out requirementOptions);
                ServerOptionChoices.LoadAtavismChoiceOptions("Race", false, out raceIds, out raceOptions);
                ServerOptionChoices.LoadAtavismChoiceOptions("Class", false, out classIds, out classOptions);
                linkedTablesLoaded = true;
            }

            // Draw the content database info

            if (newEntity)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new Quest"));
                pos.y += 1f * ImagePack.fieldHeight;
            }
            pos.y += 0.5f * ImagePack.fieldHeight;

            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.75f);
            pos.y += ImagePack.fieldHeight;
            GUI.Label(pos, Lang.GetTranslate("Description")+":", ImagePack.FieldStyle());
            pos.height *= 2;
            descriptionScroll = GUI.BeginScrollView(pos, descriptionScroll, new Rect(0, 0, pos.width * 0.75f, 100));
            editingDisplay.description = EditorGUI.TextArea(new Rect(115, 0, pos.width * 0.75f, 100), editingDisplay.description, ImagePack.TextAreaStyle());
            //editingDisplay.description = ImagePack.DrawField (new Rect(0, 0, pos.width * 0.75f, 200), "Description:", editingDisplay.description, 0.75f, 50);
            GUI.EndScrollView();
            pos.y += 2.2f * ImagePack.fieldHeight;
            pos.height /= 2;
            GUI.Label(pos, Lang.GetTranslate("Objective Text")+":", ImagePack.FieldStyle());
            pos.height *= 2;
            objectiveScroll = GUI.BeginScrollView(pos, objectiveScroll, new Rect(0, 0, pos.width * 0.75f, 100));
            editingDisplay.objectiveText = EditorGUI.TextArea(new Rect(115, 0, pos.width * 0.75f, 100), editingDisplay.objectiveText, ImagePack.TextAreaStyle());
            GUI.EndScrollView();
            pos.height /= 2;
            pos.y += 2.2f * ImagePack.fieldHeight;
            GUI.Label(pos, Lang.GetTranslate("Progress Text")+":", ImagePack.FieldStyle());
            pos.height *= 2;
            progressScroll = GUI.BeginScrollView(pos, progressScroll, new Rect(0, 0, pos.width * 0.75f, 100));
            editingDisplay.progressText = EditorGUI.TextArea(new Rect(115, 0, pos.width * 0.75f, 100), editingDisplay.progressText, ImagePack.TextAreaStyle());
            GUI.EndScrollView();
            pos.height /= 2;
            pos.y += 2.2f * ImagePack.fieldHeight;
            GUI.Label(pos, Lang.GetTranslate("Completion Text")+":", ImagePack.FieldStyle());
            pos.height *= 2;
            completionScroll = GUI.BeginScrollView(pos, completionScroll, new Rect(0, 0, pos.width * 0.75f, 100));
            editingDisplay.completionText = EditorGUI.TextArea(new Rect(115, 0, pos.width * 0.75f, 100), editingDisplay.completionText, ImagePack.TextAreaStyle());
            GUI.EndScrollView();
            pos.height /= 2;
            pos.y += 2.2f * ImagePack.fieldHeight;
            pos.width /= 2;
            //editingDisplay.category = ImagePack.DrawField (pos, "Category:", editingDisplay.category);
            //pos.x += pos.width;
            //editingDisplay.faction = ImagePack.DrawField (pos, "Faction:", editingDisplay.faction);
            int otherFactionID = GetOptionPosition(editingDisplay.faction, factionIds);
            otherFactionID = ImagePack.DrawSelector(pos, Lang.GetTranslate("Faction")+":", otherFactionID, factionOptions);
            editingDisplay.faction = factionIds[otherFactionID];

            pos.x += pos.width;
            editingDisplay.level = ImagePack.DrawField(pos, Lang.GetTranslate("Level")+":", editingDisplay.level);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.chain = ImagePack.DrawField(pos, Lang.GetTranslate("Chain")+":", editingDisplay.chain);
            pos.x += pos.width;
            //editingDisplay.zone = ImagePack.DrawField (pos, "Zone:", editingDisplay.zone);
            //pos.x += pos.width;
            //editingDisplay.numGrades = ImagePack.DrawField (pos, "Num. Grades:", editingDisplay.numGrades);
            //pos.x += pos.width;
            editingDisplay.repeatable = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Is Repeatable")+"?", editingDisplay.repeatable);
            pos.x -= pos.width;
            //pos.x -= 2*pos.width;	
            pos.width *= 2;
            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Items Given"));
            pos.y += 1.5f * ImagePack.fieldHeight;
            pos.width /= 2;
            int selectedItem = GetOptionPosition(editingDisplay.deliveryItem1, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item")+" 1:", ref itemGivenSearch1, selectedItem, itemsList);
            editingDisplay.deliveryItem1 = itemIds[selectedItem];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.deliveryItem2, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item")+" 2:", ref itemGivenSearch2, selectedItem, itemsList);
            editingDisplay.deliveryItem2 = itemIds[selectedItem];
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.deliveryItem3, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item")+" 3:", ref itemGivenSearch3, selectedItem, itemsList);
            editingDisplay.deliveryItem3 = itemIds[selectedItem];
            pos.width *= 2;
            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Quest Prerequisites"));
            pos.y += 1.5f * ImagePack.fieldHeight;
            //pos.width *= 3;
            pos.width /= 2;
            int selectedQuest = GetOptionPosition(editingDisplay.questPrereq, questIds);
            selectedQuest = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Quest Completed")+":", ref questCompletedSearch, selectedQuest, questOptions);
            editingDisplay.questPrereq = questIds[selectedQuest];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedQuest = GetOptionPosition(editingDisplay.questStartedReq, questIds);
            selectedQuest = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Quest Started")+":", ref questStartedSearch, selectedQuest, questOptions);
            editingDisplay.questStartedReq = questIds[selectedQuest];
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            pos.width *= 2;

            // Requirements area
            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Requirements"));
            pos.y += 1.5f * ImagePack.fieldHeight;
            pos.width /= 2;
            for (int i = 0; i < editingDisplay.questRequirements.Count; i++)
            {
                int selectedRequirement = GetOptionPosition(editingDisplay.questRequirements[i].editor_option_type_id, requirementIds);
                selectedRequirement = ImagePack.DrawSelector(pos, Lang.GetTranslate("Type")+":", selectedRequirement, requirementOptions);
                editingDisplay.questRequirements[i].editor_option_type_id = requirementIds[selectedRequirement];
                pos.x += pos.width;
                if (requirementOptions[selectedRequirement] == "Race")
                {
                    int raceID = 0;
                    int.TryParse(editingDisplay.questRequirements[i].editor_option_choice_type_id, out raceID);
                    int selectedRace = GetOptionPosition(raceID, raceIds);
                    selectedRace = ImagePack.DrawSelector(pos, Lang.GetTranslate("Race")+":", selectedRace, raceOptions);
                    editingDisplay.questRequirements[i].editor_option_choice_type_id = raceIds[selectedRace].ToString();
                }
                else if (requirementOptions[selectedRequirement] == "Class")
                {
                    int classID = 0;
                    int.TryParse(editingDisplay.questRequirements[i].editor_option_choice_type_id, out classID);
                    int selectedClass = GetOptionPosition(classID, classIds);
                    selectedClass = ImagePack.DrawSelector(pos, Lang.GetTranslate("Class")+":", selectedClass, classOptions);
                    editingDisplay.questRequirements[i].editor_option_choice_type_id = classIds[selectedClass].ToString();
                }
                else if (requirementOptions[selectedRequirement] == "Level")
                {
                    editingDisplay.questRequirements[i].required_value = ImagePack.DrawField(pos, Lang.GetTranslate("Level")+":", editingDisplay.questRequirements[i].required_value);
                }
                else if (requirementOptions[selectedRequirement] == "Skill Level")
                {
                    int skillID = 0;
                    int.TryParse(editingDisplay.questRequirements[i].editor_option_choice_type_id, out skillID);
                    int selectedSkill = GetOptionPosition(skillID, skillIds);
                    selectedSkill = ImagePack.DrawSelector(pos, Lang.GetTranslate("Skill")+":", selectedSkill, skillOptions);
                    editingDisplay.questRequirements[i].editor_option_choice_type_id = skillIds[selectedSkill].ToString();
                    pos.y += ImagePack.fieldHeight;
                    editingDisplay.questRequirements[i].required_value = ImagePack.DrawField(pos, Lang.GetTranslate("Level")+":", editingDisplay.questRequirements[i].required_value);
                }
                else if (requirementOptions[selectedRequirement] == "Stat")
                {
                    editingDisplay.questRequirements[i].editor_option_choice_type_id = ImagePack.DrawSelector(pos, Lang.GetTranslate("Stat")+":", editingDisplay.questRequirements[i].editor_option_choice_type_id, statOptions);
                    pos.y += ImagePack.fieldHeight;
                    editingDisplay.questRequirements[i].required_value = ImagePack.DrawField(pos, Lang.GetTranslate("Value")+":", editingDisplay.questRequirements[i].required_value);
                }
                else if (requirementOptions[selectedRequirement] == "Faction")
                {
                    int factionID = 0;
                    int.TryParse(editingDisplay.questRequirements[i].editor_option_choice_type_id, out factionID);
                    int selectedFaction = GetOptionPosition(factionID, factionIds);
                    selectedFaction = ImagePack.DrawSelector(pos, Lang.GetTranslate("Faction")+":", selectedFaction, factionOptions);
                    editingDisplay.questRequirements[i].editor_option_choice_type_id = factionIds[selectedFaction].ToString();
                    pos.y += ImagePack.fieldHeight;
                    //editingDisplay.questRequirements[i].required_value = ImagePack.DrawField (pos, "Reputation:", editingDisplay.questRequirements[i].required_value);
                    int selectedStance = FactionData.GetPositionOfStance(editingDisplay.questRequirements[i].required_value);
                    selectedStance = ImagePack.DrawSelector(pos, Lang.GetTranslate("Stance")+":", selectedStance, FactionData.stanceOptions);
                    editingDisplay.questRequirements[i].required_value = FactionData.stanceValues[selectedStance];
                }
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove Requirement")))
                {
                    if (editingDisplay.questRequirements[i].id > 0)
                        editingDisplay.questRequirementsToBeDeleted.Add(editingDisplay.questRequirements[i].id);
                    editingDisplay.questRequirements.RemoveAt(i);
                }
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
            }
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Requirement")))
            {
                editingDisplay.questRequirements.Add(new QuestRequirementEntry(-1, -1, ""));
            }

            pos.width *= 2;

            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Objectives"));
            pos.y += ImagePack.fieldHeight;
            /*if (editingDisplay.questObjectives.Count == 0) {
                editingDisplay.questObjectives.Add(new QuestsObjectivesData());
            }*/
            for (int i = 0; i < editingDisplay.questObjectives.Count; i++)
            {
                pos.width /= 2;
                editingDisplay.questObjectives[i].objectiveType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Type")+" " + (i + 1) + ":",
                    editingDisplay.questObjectives[i].objectiveType, objectiveTypeOptions);
                //pos.x += pos.width;
                pos.y += ImagePack.fieldHeight;
                if (editingDisplay.questObjectives[i].objectiveType == "item")
                {
                    selectedItem = GetOptionPosition(editingDisplay.questObjectives[i].target, itemIds);
                    selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Target")+" " + (i + 1) + ":", ref editingDisplay.questObjectives[i].targetSearch, selectedItem, itemsList);
                    editingDisplay.questObjectives[i].target = itemIds[selectedItem];
                }
                else if (editingDisplay.questObjectives[i].objectiveType == "mob")
                {
                    int selectedMob = GetOptionPosition(editingDisplay.questObjectives[i].target, mobIds);
                    selectedMob = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Target")+" " + (i + 1) + ":", ref editingDisplay.questObjectives[i].targetSearch, selectedMob, mobList);
                    editingDisplay.questObjectives[i].target = mobIds[selectedMob];
                }
                else if (editingDisplay.questObjectives[i].objectiveType == "task")
                {
                    int selectedTask = GetOptionPosition(editingDisplay.questObjectives[i].target, taskIds);
                    selectedTask = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Target")+" " + (i + 1) + ":", ref editingDisplay.questObjectives[i].targetSearch, selectedTask, taskList);
                    editingDisplay.questObjectives[i].target = taskIds[selectedTask];
                }
                // pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                editingDisplay.questObjectives[i].targetCount = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.questObjectives[i].targetCount);
                pos.y += ImagePack.fieldHeight;
                pos.width *= 2;
                editingDisplay.questObjectives[i].targetText = ImagePack.DrawField(pos, Lang.GetTranslate("Text")+":", editingDisplay.questObjectives[i].targetText, 1.4f);
                pos.width /= 2;

                if (editingDisplay.questObjectives[i].objectiveType == "mobCategory")
                {
                    if (editingDisplay.questObjectives[i].targetsList.Count == 0)
                    {
                        editingDisplay.questObjectives[i].targetsList.Add(0);
                        editingDisplay.questObjectives[i].targetsSearch.Add("");
                    }
                    for (int j = 0; j < editingDisplay.questObjectives[i].targetsList.Count; j++)
                    {
                        pos.y += ImagePack.fieldHeight;
                        int selectedMob = GetOptionPosition(editingDisplay.questObjectives[i].targetsList[j], mobIds);
                        string search = editingDisplay.questObjectives[i].targetsSearch[j];
                        selectedMob = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Target Mob") + (j + 1) + ":", ref search, selectedMob, mobList);
                        editingDisplay.questObjectives[i].targetsSearch[j] = search;
                        editingDisplay.questObjectives[i].targetsList[j] = mobIds[selectedMob];
                        editingDisplay.questObjectives[i].target = 0;
                      pos.x += pos.width;
                        pos.y += ImagePack.fieldHeight;
                        if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Mob")))
                        {

                            editingDisplay.questObjectives[i].targetsList.RemoveAt(j);
                            editingDisplay.questObjectives[i].targetsSearch.RemoveAt(j);
                        }
                        pos.x -= pos.width;
                    }
                    pos.y += ImagePack.fieldHeight;
                    if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Mob")))
                    {
                        editingDisplay.questObjectives[i].targetsList.Add(0);
                        editingDisplay.questObjectives[i].targetsSearch.Add("");
                    }
                }



                pos.y += ImagePack.fieldHeight;
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Objective")))
                {
                    if (editingDisplay.questObjectives[i].id > 0)
                        editingDisplay.objectivesToBeDeleted.Add(editingDisplay.questObjectives[i].id);
                    editingDisplay.questObjectives.RemoveAt(i);
                }
                pos.x -= pos.width;
                pos.width *= 2;
                pos.y += ImagePack.fieldHeight;
            }
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Objective")))
            {
                editingDisplay.questObjectives.Add(new QuestsObjectivesData());
            }

            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Rewards"));
            pos.y += ImagePack.fieldHeight;

            pos.width /= 2;
            editingDisplay.experience = ImagePack.DrawField(pos, Lang.GetTranslate("Experience")+":", editingDisplay.experience);
            pos.y += ImagePack.fieldHeight;   //pos.x -= pos.width;

            int selectedCurrency = GetOptionPosition(editingDisplay.currency, currencyIds);
            selectedCurrency = ImagePack.DrawSelector(pos, Lang.GetTranslate("Currency") + ":", selectedCurrency, currencyOptions);
            editingDisplay.currency = currencyIds[selectedCurrency];
            pos.x += pos.width;
            //  pos.y += ImagePack.fieldHeight;
            editingDisplay.currencyCount = ImagePack.DrawField(pos, Lang.GetTranslate("Count") + ":", editingDisplay.currencyCount);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedCurrency = GetOptionPosition(editingDisplay.currency2, currencyIds);
            selectedCurrency = ImagePack.DrawSelector(pos, Lang.GetTranslate("Currency") + " 2:", selectedCurrency, currencyOptions);
            editingDisplay.currency2 = currencyIds[selectedCurrency];
            pos.x += pos.width;
            //   pos.y += ImagePack.fieldHeight;
            editingDisplay.currency2count = ImagePack.DrawField(pos, Lang.GetTranslate("Count") + ":", editingDisplay.currency2count);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            int repID = GetOptionPosition(editingDisplay.rep1, factionIds);
            repID = ImagePack.DrawSelector(pos, Lang.GetTranslate("Faction") + " 1:", repID, factionOptions);
            editingDisplay.rep1 = factionIds[repID];
            pos.x += pos.width;
            editingDisplay.rep1gain = ImagePack.DrawField(pos, Lang.GetTranslate("Rep") + ":", editingDisplay.rep1gain);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            repID = GetOptionPosition(editingDisplay.rep2, factionIds);
            repID = ImagePack.DrawSelector(pos, Lang.GetTranslate("Faction") + " 2:", repID, factionOptions);
            editingDisplay.rep2 = factionIds[repID];
            pos.x += pos.width;
            editingDisplay.rep2gain = ImagePack.DrawField(pos, Lang.GetTranslate("Rep") + ":", editingDisplay.rep2gain);
            pos.x -= pos.width;

            pos.y += ImagePack.fieldHeight;

            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Reward Items"));
            pos.y += ImagePack.fieldHeight;

            for (int i = 0; i < editingDisplay.items.Count; i++)
            {
                
                //  pos.width /= 2;
                //   pos.y += ImagePack.fieldHeight;
                selectedItem = GetOptionPosition(editingDisplay.items[i].itemId, itemIds);
                selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item") + " "+(i+1)+":", ref editingDisplay.items[i].searche, selectedItem, itemsList);
                editingDisplay.items[i].itemId = itemIds[selectedItem];
                pos.y += ImagePack.fieldHeight;
                editingDisplay.items[i].count = ImagePack.DrawField(pos, Lang.GetTranslate("Item") + " " + (i + 1) + " " + Lang.GetTranslate("Count") + ":", editingDisplay.items[i].count);

                pos.y += ImagePack.fieldHeight;
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Item")))
                {
                    if (editingDisplay.items[i].id > 0)
                        editingDisplay.itemsToBeDeleted.Add(editingDisplay.items[i].id);
                    editingDisplay.items.RemoveAt(i);
                }
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
            }
           // pos.width *= 2;

            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Item")))
            {
                editingDisplay.items.Add(new QuestItemEntry());
            }
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Reward Items To Choose"));
            pos.y += ImagePack.fieldHeight;

            for (int i = 0; i < editingDisplay.chooseItems.Count; i++)
            {

                //  pos.width /= 2;
                //   pos.y += ImagePack.fieldHeight;
                selectedItem = GetOptionPosition(editingDisplay.chooseItems[i].itemId, itemIds);
                selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item") + " " + (i + 1) + ":", ref editingDisplay.chooseItems[i].searche, selectedItem, itemsList);
                editingDisplay.chooseItems[i].itemId = itemIds[selectedItem];
                pos.y += ImagePack.fieldHeight;
                editingDisplay.chooseItems[i].count = ImagePack.DrawField(pos, Lang.GetTranslate("Item") + " " + (i + 1) + " " + Lang.GetTranslate("Count") + ":", editingDisplay.chooseItems[i].count);
                pos.y += ImagePack.fieldHeight;
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Item")))
                {
                    if (editingDisplay.chooseItems[i].id > 0)
                        editingDisplay.chooseItemsToBeDeleted.Add(editingDisplay.chooseItems[i].id);
                    editingDisplay.chooseItems.RemoveAt(i);
                }
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
            }
            // pos.width *= 2;

            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Item")))
            {
                editingDisplay.chooseItems.Add(new QuestItemEntry());
            }


/*
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.item1, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item")+" 1:", ref itemSearch1, selectedItem, itemsList);
            editingDisplay.item1 = itemIds[selectedItem];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.item1count = ImagePack.DrawField(pos, Lang.GetTranslate("Item")+" 1 "+ Lang.GetTranslate("Count")+":", editingDisplay.item1count);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.item2, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item")+" 2:", ref itemSearch2, selectedItem, itemsList);
            editingDisplay.item2 = itemIds[selectedItem];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.item2count = ImagePack.DrawField(pos, Lang.GetTranslate("Item")+" 2 "+ Lang.GetTranslate("Count")+":", editingDisplay.item2count);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.item3, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item")+" 3:", ref itemSearch3, selectedItem, itemsList);
            editingDisplay.item3 = itemIds[selectedItem];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.item3count = ImagePack.DrawField(pos, Lang.GetTranslate("Item")+" 3 "+ Lang.GetTranslate("Count")+":", editingDisplay.item3count);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.item4, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item")+" 4:", ref itemSearch4, selectedItem, itemsList);
            editingDisplay.item4 = itemIds[selectedItem];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.item4count = ImagePack.DrawField(pos, Lang.GetTranslate("Item")+" 4 "+ Lang.GetTranslate("Count")+":", editingDisplay.item4count);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.item5, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item")+" 5:", ref itemSearch5, selectedItem, itemsList);
            editingDisplay.item5 = itemIds[selectedItem];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.item5count = ImagePack.DrawField(pos, Lang.GetTranslate("Item")+" 5 "+ Lang.GetTranslate("Count")+":", editingDisplay.item5count);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.item6, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item")+" 6:", ref itemSearch6, selectedItem, itemsList);
            editingDisplay.item6 = itemIds[selectedItem];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.item6count = ImagePack.DrawField(pos, Lang.GetTranslate("Item")+" 6 "+ Lang.GetTranslate("Count")+":", editingDisplay.item6count);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.item7, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item")+" 7:", ref itemSearch7, selectedItem, itemsList);
            editingDisplay.item7 = itemIds[selectedItem];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.item7count = ImagePack.DrawField(pos, Lang.GetTranslate("Item")+" 7 "+ Lang.GetTranslate("Count")+":", editingDisplay.item7count);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.item8, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item")+" 8:", ref itemSearch8, selectedItem, itemsList);
            editingDisplay.item8 = itemIds[selectedItem];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.item8count = ImagePack.DrawField(pos, Lang.GetTranslate("Item")+" 8 "+ Lang.GetTranslate("Count")+":", editingDisplay.item8count);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;

            selectedItem = GetOptionPosition(editingDisplay.chooseItem1, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item Choice")+" 1:", ref itemChoisSearch1, selectedItem, itemsList);
            editingDisplay.chooseItem1 = itemIds[selectedItem];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.chooseItem1count = ImagePack.DrawField(pos, Lang.GetTranslate("Choice") + " 1 " + Lang.GetTranslate("Count") + ":", editingDisplay.chooseItem1count);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.chooseItem2, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item Choice") + " 2:", ref itemChoisSearch2, selectedItem, itemsList);
            editingDisplay.chooseItem2 = itemIds[selectedItem];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.chooseItem2count = ImagePack.DrawField(pos, Lang.GetTranslate("Choice") + " 2 " + Lang.GetTranslate("Count") + ":", editingDisplay.chooseItem2count);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.chooseItem3, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item Choice") + " 3:", ref itemChoisSearch3, selectedItem, itemsList);
            editingDisplay.chooseItem3 = itemIds[selectedItem];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.chooseItem3count = ImagePack.DrawField(pos, Lang.GetTranslate("Choice") + " 3 " + Lang.GetTranslate("Count") + ":", editingDisplay.chooseItem3count);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.chooseItem4, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item Choice") + " 4:", ref itemChoisSearch4, selectedItem, itemsList);
            editingDisplay.chooseItem4 = itemIds[selectedItem];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.chooseItem4count = ImagePack.DrawField(pos, Lang.GetTranslate("Choice") + " 4 " + Lang.GetTranslate("Count") + ":", editingDisplay.chooseItem4count);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;

            selectedItem = GetOptionPosition(editingDisplay.chooseItem5, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item Choice") + " 5:", ref itemChoisSearch5, selectedItem, itemsList);
            editingDisplay.chooseItem5 = itemIds[selectedItem];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.chooseItem5count = ImagePack.DrawField(pos, Lang.GetTranslate("Choice") + " 5 " + Lang.GetTranslate("Count") + ":", editingDisplay.chooseItem5count);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.chooseItem6, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item Choice") + " 6:", ref itemChoisSearch6, selectedItem, itemsList);
            editingDisplay.chooseItem6 = itemIds[selectedItem];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.chooseItem6count = ImagePack.DrawField(pos, Lang.GetTranslate("Choice") + " 6 " + Lang.GetTranslate("Count") + ":", editingDisplay.chooseItem6count);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.chooseItem7, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item Choice") + " 7:", ref itemChoisSearch7, selectedItem, itemsList);
            editingDisplay.chooseItem7 = itemIds[selectedItem];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.chooseItem7count = ImagePack.DrawField(pos, Lang.GetTranslate("Choice") + " 7 " + Lang.GetTranslate("Count") + ":", editingDisplay.chooseItem7count);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.chooseItem8, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item Choice") + " 8:", ref itemChoisSearch8, selectedItem, itemsList);
            editingDisplay.chooseItem8 = itemIds[selectedItem];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.chooseItem8count = ImagePack.DrawField(pos, Lang.GetTranslate("Choice") + " 8 " + Lang.GetTranslate("Count") + ":", editingDisplay.chooseItem8count);
            //pos.x -= pos.width;*/
            pos.y += ImagePack.fieldHeight;

          
            pos.width *= 2;

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
            if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete") + " " + functionName + " " + Lang.GetTranslate("entry") + " ?", Lang.GetTranslate("Are you sure you want to delete") + " " + editingDisplay.Name + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
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
                // Insert the Requirements
                foreach (QuestRequirementEntry entry in editingDisplay.questRequirements)
                {
                    if (entry.editor_option_type_id != -1)
                    {
                        entry.questID = itemID;
                        InsertRequirement(entry);
                    }
                }
                // Insert the objectives
                foreach (QuestsObjectivesData entry in editingDisplay.questObjectives)
                {
                    if (entry.target != -1 || entry.targetsList.Count > 0)
                    {
                        entry.questID = itemID;
                        InsertObjective(entry);
                    }
                }

                foreach (QuestItemEntry entry in editingDisplay.items)
                {
                    if (entry.itemId > 0 && entry.count > 0)
                    {
                        entry.questID = itemID;
                        InsertItems(entry,false);
                    }
                }
                foreach (QuestItemEntry entry in editingDisplay.chooseItems)
                {
                    if (entry.itemId > 0 && entry.count > 0)
                    {
                        entry.questID = itemID;
                        InsertItems(entry, true);
                    }
                }


                // Update online table to avoid access the database again			
                editingDisplay.id = itemID;
                editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
                dataLoaded = false;
                Load();
                LoadQuestOptions();
                newItemCreated = true;
                NewResult(Lang.GetTranslate("New entry inserted"));
            }
            else
            {
                NewResult("Error"+ Lang.GetTranslate("Error occurred, please check the Console"));
            }
        }

        void InsertRequirement(QuestRequirementEntry entry)
        {
            string query = "INSERT INTO quest_requirement";
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

        void InsertObjective(QuestsObjectivesData entry)
        {
            string query = "INSERT INTO quest_objectives";
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

        void InsertItems(QuestItemEntry entry,bool choose)
        {
            string query = "INSERT INTO quest_items";
            query += " (" + entry.FieldList("", ", ") + ", choose) ";
            query += "VALUES ";
            query += " (" + entry.FieldList("?", ", ") + ", ?choose) ";

            // Setup the register data		
            List<Register> update = new List<Register>();

            foreach (string field in entry.fields.Keys)
            {
                update.Add(entry.fieldToRegister(field));
            }
            update.Add(new Register("choose", "?choose", MySqlDbType.Int16, choose.ToString(), Register.TypesOfField.Bool));

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

            // Insert/Update the requirements
            foreach (QuestRequirementEntry entry in editingDisplay.questRequirements)
            {
                if (entry.editor_option_type_id != -1)
                {
                    if (entry.id < 1)
                    {
                        // This is a new entry, insert it
                        entry.questID = editingDisplay.id;
                        InsertRequirement(entry);
                    }
                    else
                    {
                        // This is an existing entry, update it
                        entry.questID = editingDisplay.id;
                        UpdateRequirement(entry);
                    }
                }
            }

            // Delete any requirements that are tagged for deletion
            foreach (int requirementID in editingDisplay.questRequirementsToBeDeleted)
            {
                DeleteRequirement(requirementID);
            }

            // Insert/Update the objectives
            foreach (QuestsObjectivesData entry in editingDisplay.questObjectives)
            {
                if (entry.target != -1|| entry.targetsList.Count > 0)
                {
                    if (entry.id == 0)
                    {
                        // This is a new entry, insert it
                        entry.questID = editingDisplay.id;
                        InsertObjective(entry);
                    }
                    else
                    {
                        // This is an existing entry, update it
                        entry.questID = editingDisplay.id;
                        UpdateObjective(entry);
                    }
                }
            }
            // And now delete any Objectives that are tagged for deletion
            foreach (int objectiveID in editingDisplay.objectivesToBeDeleted)
            {
                DeleteObjective(objectiveID);
            }
            // Insert/Update the items
            foreach (QuestItemEntry entry in editingDisplay.items)
            {
             //   if (entry.target != -1 || entry.targetsList.Count > 0)
             //   {
                    if (entry.id == 0)
                    {
                        // This is a new entry, insert it
                        entry.questID = editingDisplay.id;
                        InsertItems(entry,false);
                    }
                    else
                    {
                        // This is an existing entry, update it
                        entry.questID = editingDisplay.id;
                        UpdateItems(entry);
                    }
              //  }
            }
            // And now delete any Item that are tagged for deletion
            foreach (int itemID in editingDisplay.itemsToBeDeleted)
            {
                DeleteItem(itemID);
            }
            // Insert/Update the items
            foreach (QuestItemEntry entry in editingDisplay.chooseItems)
            {
                //   if (entry.target != -1 || entry.targetsList.Count > 0)
                //   {
                if (entry.id == 0)
                {
                    // This is a new entry, insert it
                    entry.questID = editingDisplay.id;
                    InsertItems(entry, false);
                }
                else
                {
                    // This is an existing entry, update it
                    entry.questID = editingDisplay.id;
                    UpdateItems(entry);
                }
                //  }
            }
            // And now delete any Item that are tagged for deletion
            foreach (int itemID in editingDisplay.chooseItemsToBeDeleted)
            {
                DeleteItem(itemID);
            }

            // Update online table to avoid access the database again			
            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " "+ Lang.GetTranslate("updated"));
              Load();
            LoadQuestOptions();
            linkedTablesLoaded = false;
         }

        void UpdateRequirement(QuestRequirementEntry entry)
        {
            string query = "UPDATE quest_requirement";
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

        void DeleteRequirement(int requirementID)
        {
            string query = "UPDATE quest_requirement SET isactive = 0 where id = " + requirementID;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
        }

        void UpdateObjective(QuestsObjectivesData entry)
        {
            string query = "UPDATE quest_objectives";
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

        void UpdateItems(QuestItemEntry entry)
        {
            string query = "UPDATE quest_items";
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

        void DeleteItem(int itemId)
        {
            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, objectiveID.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "quest_objectives", delete);
            List<Register> where = new List<Register>();
            where.Add(new Register("id", "?id", MySqlDbType.Int32, itemId.ToString(), Register.TypesOfField.Int));
            DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "quest_items", where);
        }


        void DeleteObjective(int objectiveID)
        {
            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, objectiveID.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "quest_objectives", delete);
            string query = "UPDATE quest_objectives SET isactive = 0 where id = " + objectiveID;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
        }

        // Delete entries from the table
        void DeleteEntry()
        {
            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, tableName, delete);
            string query = "UPDATE " + tableName + " SET isactive = 0 where id = " + editingDisplay.id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            query = "UPDATE quest_objectives SET isactive = 0 where questID = " + editingDisplay.id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            // Update online table to avoid access the database again		
            selectedDisplay = -1;
            newSelectedDisplay = 0;
            dataLoaded = false;
             NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " "+ Lang.GetTranslate("Deleted"));
           Load();
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
                where.Add(new Register("questID", "?questID", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "quest_objectives", where);

                where.Clear();
                where.Add(new Register("quest_id", "?quest_id", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "quest_requirement", where);

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
            query = "UPDATE quest_objectives SET isactive = 1 where questID = " + id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

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

    }
}