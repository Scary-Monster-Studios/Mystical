using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Atavism
{
    // Handles the Character Setup Configuration
    public class ServerCharacter : AtavismDatabaseFunction
    {

        public new Dictionary<int, CharData> dataRegister;
        public new CharData editingDisplay;
        public new CharData originalDisplay;

        public int[] classIds = new int[] { -1 };
        public string[] classOptions = new string[] { "~ none ~" };

        public int[] raceIds = new int[] { -1 };
        public string[] raceOptions = new string[] { "~ none ~" };

        public int[] instanceIds = new int[] { -1 };
        public string[] instanceOptions = new string[] { "~ none ~" };

        public int[] factionIds = new int[] { -1 };
        public string[] factionOptions = new string[] { "~ none ~" };

        public int[] abilityIds = new int[] { -1 };
        public string[] abilityOptions = new string[] { "~ none ~" };

        public int[] abilityIds2 = new int[] { -1 };
        public string[] abilityOptions2 = new string[] { "~ none ~" };

        // Character Stats, Skills and Items to Display
        public string[] statsList = null;

        public int[] skillIds = new int[] { -1 };
        public string[] skillsList = null;

        public static int[] itemIds = new int[] { -1 };
        public static string[] itemsList = null;
        public static GUIContent[] GuiItemsList = new GUIContent[] { new GUIContent("~ none ~") };
        public static GUIContent[] RNProfileList = new GUIContent[] { new GUIContent("~ none ~") };
        public static int[] RNProfileIds = new int[] { -1 };
        string autoAtackSearch = "";
        string sprintSearch = "";
        public  Dictionary<string, StatsData> stats;
        public  Dictionary<string, string> maxStats;

        // Use this for initialization
        public ServerCharacter()
        {
            functionName = "Player Character Setup";
            // Database tables name
            tableName = "character_create_template";
            functionTitle = "Character Configuration";
            loadButtonLabel = "Load Character";
            notLoadedText = "No Character loaded.";
            // Init
            stats = new Dictionary<string, StatsData>();
            maxStats = new Dictionary<string, string>();
            dataRegister = new Dictionary<int, CharData>();

            editingDisplay = new CharData();
            originalDisplay = new CharData();
            autoAtackSearch = "";
            sprintSearch = "";
        }
        void LoadStatsData()
        {
            // Read all entries from the table
            string query = "SELECT * FROM stat where isactive = 1";
           // if ()
            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();
            stats.Clear();
            maxStats.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
         //  int fakeId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    //foreach(string key in data.Keys)
                    //	Debug.Log("Name[" + key + "]:" + data[key]);
                    //return;
                    // As we don have a primary key ID field
                    StatsData display = new StatsData();
                    display.Name = data["name"];
                    display.originalName = display.Name;
                    display.type = int.Parse(data["type"]);
                    display.statFunction = data["stat_function"];
                    display.mobBase = int.Parse(data["mob_base"]);
                    display.mobLevelIncrease = int.Parse(data["mob_level_increase"]);
                    display.mobLevelPercentIncrease = float.Parse(data["mob_level_percent_increase"]);
                    display.min = int.Parse(data["min"]);
                    display.maxstat = data["maxstat"];
                    display.canExceedMax = bool.Parse(data["canExceedMax"]);
                    display.sharedWithGroup = bool.Parse(data["sharedWithGroup"]);
                    byte shiftPlayerOnly = byte.Parse(data["shiftTarget"]);
                    display.shiftTarget = (int)shiftPlayerOnly;
                    display.shiftValue = int.Parse(data["shiftValue"]);
                    display.shiftReverseValue = int.Parse(data["shiftReverseValue"]);
                    display.shiftInterval = int.Parse(data["shiftInterval"]);
                    display.isShiftPercent = bool.Parse(data["isShiftPercent"]);
                    display.onMaxHit = data["onMaxHit"];
                    display.onMinHit = data["onMinHit"];
                    display.shiftReq1 = data["shiftReq1"];
                    display.shiftReq1State = bool.Parse(data["shiftReq1State"]);
                    display.shiftReq1SetReverse = bool.Parse(data["shiftReq1SetReverse"]);
                    display.shiftReq2 = data["shiftReq2"];
                    display.shiftReq2State = bool.Parse(data["shiftReq2State"]);
                    display.shiftReq2SetReverse = bool.Parse(data["shiftReq2SetReverse"]);
                    display.shiftReq3 = data["shiftReq3"];
                    display.shiftReq3State = bool.Parse(data["shiftReq3State"]);
                    display.shiftReq3SetReverse = bool.Parse(data["shiftReq3SetReverse"]);
                    display.startPercent = int.Parse(data["startPercent"]);
                    display.deathResetPercent = int.Parse(data["deathResetPercent"]);
                    display.releaseResetPercent = int.Parse(data["releaseResetPercent"]);
                    
                    display.isLoaded = true;
                    //Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
                    stats.Add(display.Name, display);
                }
            }
            foreach(string sd in stats.Keys)
            {
                if (stats[sd].type == 2)
                {
                    if (!string.IsNullOrEmpty(stats[sd].maxstat))
                    {
                        maxStats.Add(stats[sd].maxstat, sd);
                    }
                }
            }
        }



        public void LoadInstanceOptions()
        {
            // Read all entries from the table
            string query = "SELECT id, island_name FROM instance_template";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.adminDatabasePrefix, query);
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

        public void LoadFactionOptions()
        {
            string query = "SELECT id, name FROM factions ";

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
        public void LoadAbilityOptions2()
        {
            string query = "SELECT id, name FROM abilities where isactive = 1 and toggle = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                abilityOptions2 = new string[rows.Count + 1];
                abilityOptions2[optionsId] = "~ none ~";
                abilityIds2 = new int[rows.Count + 1];
                abilityIds2[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    abilityOptions2[optionsId] = data["id"] + ":" + data["name"];
                    abilityIds2[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        private void LoadStatList()
        {
            List<CharStatsData> charStats = new List<CharStatsData>();
            foreach (string stat in statsList)
            {
                CharStatsData charStat = new CharStatsData();
                charStat.stat = stat;
                charStat.statValue = 0;
                charStats.Add(charStat);
            }
            editingDisplay.charStats = charStats;
        }

        public override void Activate()
        {
            if (statsList == null)
            {
                LoadStatOptions();
            }
            if (state == State.New)
                  LoadStatList();
            linkedTablesLoaded = false;
            LoadStatsData();
            resetSearch(true);
        }
        void resetSearch(bool fokus)
        {
           if(fokus) GUI.FocusControl(null);
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
                    statsList = new string[rows.Count];
                    foreach (Dictionary<string, string> data in rows)
                    {
                        optionsId++;
                        statsList[optionsId - 1] = data["name"];
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
            skillsList = new string[rows.Count + 1];
            skillIds = new int[rows.Count + 1];
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                skillsList[optionsId] = "~ none ~";
                skillIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    skillsList[optionsId] = data["id"] + ":" + data["name"];
                    skillIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        public static void LoadItemOptions()
        {
            string query = "SELECT id, name FROM item_templates where isactive = 1";
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);


            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            itemsList = new string[rows.Count + 1];
            itemIds = new int[rows.Count + 1];
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                itemsList[optionsId] = "~ none ~";
                itemIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    itemsList[optionsId] = data["id"] + ":" + data["name"];
                    itemIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }


        public static void LoadItemOptions(bool gui)
        {
            if (!gui)
            {
                LoadItemOptions();
                return;
            }
            string query = "SELECT id, name FROM item_templates where isactive = 1";
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);


            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            GuiItemsList = new GUIContent[rows.Count + 1];
            itemIds = new int[rows.Count + 1];
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                GuiItemsList[optionsId] = new GUIContent("~ none ~");
                itemIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    GuiItemsList[optionsId] = new GUIContent(data["id"] + ":" + data["name"]);
                    itemIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        public static void LoadResourceNodeProfileOptions(bool gui)
        {
            if (!gui)
            {
                LoadItemOptions();
                return;
            }
            string query = "SELECT id, name FROM resource_node_profile where isactive = 1";
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);


            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            RNProfileList = new GUIContent[rows.Count + 1];
            RNProfileIds = new int[rows.Count + 1];
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                RNProfileList[optionsId] = new GUIContent("~ none ~");
                RNProfileIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    RNProfileList[optionsId] = new GUIContent(data["id"] + ":" + data["name"]);
                    RNProfileIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        public static int GetFilteredListSelector(GUIContent content, ref string name, int selected, GUIContent[] list, int[] ids)
        {
            string wSearched = name.ToLower();
            List<int> itList = new List<int>();
            List<GUIContent> ItList = new List<GUIContent>();
            int wSelectedinSublist = 0;
            List<GUIContent> wFilteredItems = new List<GUIContent>(list);
            if (name.Length > 1)
            {
                int i = 0;
               foreach(GUIContent c in list)
                {
                    if (c.text.ToLower().Contains(wSearched))
                    {
                        itList.Add(ids[i]);
                        ItList.Add(c);
                    }
                    i++;
                }
                wSelectedinSublist = (int)EditorGUILayout.IntPopup(content, selected, ItList.ToArray(), itList.ToArray());
            }
            else
            {
                wSelectedinSublist = (int)EditorGUILayout.IntPopup(content, selected, list, ids);
            }
                return wSelectedinSublist;
        }

        // Load Database Data
        public override void Load()
        {
            if (!dataLoaded)
            {
                ServerOptionChoices.LoadAtavismChoiceOptions("Race", false, out raceIds, out raceOptions);
                ServerOptionChoices.LoadAtavismChoiceOptions("Class", false, out classIds, out classOptions);

                // Clean old data
                // dataRegister.Clear();
                displayKeys.Clear();

                // Read all entries from the table
                string query = "SELECT id,race,aspect FROM " + tableName + " where isactive = 1";

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
                        int selectedRace = GetOptionPosition(int.Parse(data["race"]), raceIds);
                        string raceName = raceOptions[selectedRace];
                        int selectedClass = GetOptionPosition(int.Parse(data["aspect"]), classIds);
                        string className = classOptions[selectedClass];

                        displayList.Add(data["id"] + ". " + raceName + " " + className);
                        displayKeys.Add(int.Parse(data["id"]));

                    }
                }
                dataLoaded = true;
            }
        }

        CharData LoadEntity(int id)
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
            CharData display = new CharData();
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    //foreach(string key in data.Keys)
                    //	Debug.Log("Name[" + key + "]:" + data[key]);
                    //return;
                    display.id = int.Parse(data["id"]);
                    display.race = int.Parse(data["race"]);
                    display.aspect = int.Parse(data["aspect"]);
                    display.faction = int.Parse(data["faction"]);
                    display.instance = int.Parse(data["instance"]);
                    display.pos_x = float.Parse(data["pos_x"]);
                    display.pos_y = float.Parse(data["pos_y"]);
                    display.pos_z = float.Parse(data["pos_z"]);
                    display.orientation = float.Parse(data["orientation"]);
                    display.respawnInstance = int.Parse(data["respawnInstance"]);
                    display.respawnPosX = float.Parse(data["respawnPosX"]);
                    display.respawnPosY = float.Parse(data["respawnPosY"]);
                    display.respawnPosZ = float.Parse(data["respawnPosZ"]);
                    display.autoAttack = int.Parse(data["autoAttack"]);
                    display.startingLevel = int.Parse(data["startingLevel"]);
                    display.sprint = int.Parse(data["sprint"]);

                    display.isLoaded = true;
                    //Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
                }
            }
           // Load character starting stats, skills and items
            LoadCharacterStats(display);
            LoadCharacterSkills(display);
            LoadCharacterItems(display);
            return display;
        }

        public override void LoadRestore()
        {
            if (!dataRestoreLoaded)
            {


                ServerOptionChoices.LoadAtavismChoiceOptions("Race", false, out raceIds, out raceOptions);
                ServerOptionChoices.LoadAtavismChoiceOptions("Class", false, out classIds, out classOptions);


                // Clean old data
                dataRegister.Clear();
                displayKeys.Clear();

                // Read all entries from the table
                string query = "SELECT id,race,aspect FROM " + tableName + " where isactive = 0";

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
                        int selectedRace = GetOptionPosition(int.Parse(data["race"]), raceIds);
                        string raceName = raceOptions[selectedRace];
                        int selectedClass = GetOptionPosition(int.Parse(data["aspect"]), classIds);
                        string className = classOptions[selectedClass];

                        displayList.Add(data["id"] + ". " + raceName + " " + className);
                        displayKeys.Add(int.Parse(data["id"]));
                    }
                }
                dataRestoreLoaded = true;
            }
        }

        // Load Stats
        public void LoadCharacterStats(CharData charData)
        {
            List<CharStatsData> charStats = new List<CharStatsData>();
            // Read all entries from the table
            string query = "SELECT " + new CharStatsData().GetFieldsString() + " FROM character_create_stats where character_create_id = "
                + charData.id + " AND isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read all the data
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    CharStatsData display = new CharStatsData();
                    display.id = int.Parse(data["id"]);
                    display.charId = int.Parse(data["character_create_id"]);
                    display.stat = data["stat"];
                    display.statValue = int.Parse(data["value"]);
                    display.levelIncrease = float.Parse(data["levelIncrease"]);
                    display.levelPercentIncrease = float.Parse(data["levelPercentIncrease"]);
                    charStats.Add(display);
                }
            }
            // Check for any stats the template may not have
            foreach (string stat in statsList)
            {
                bool statExists = false;
                foreach (CharStatsData charStat in charStats)
                {
                    if (stat == charStat.stat)
                    {
                        statExists = true;
                    }
                }

                if (!statExists)
                {
                    CharStatsData statData = new CharStatsData();
                    statData.stat = stat;
                    statData.statValue = 0;
                    charStats.Add(statData);
                }
            }
            charData.charStats = charStats;
        }

        // Load Stats
        public void LoadCharacterSkills(CharData charData)
        {
            List<CharSkillsData> charSkills = new List<CharSkillsData>();
            // Read all entries from the table
            string query = "SELECT " + new CharSkillsData().GetFieldsString() + " FROM character_create_skills WHERE character_create_id = "
                        + charData.id + " AND isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read all the data
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    CharSkillsData display = new CharSkillsData();
                    display.id = int.Parse(data["id"]);
                    display.charId = int.Parse(data["character_create_id"]);
                    display.skill = int.Parse(data["skill"]);
                    charSkills.Add(display);
                }
            }
            charData.charSkills = charSkills;
        }

        // Load Stats
        public void LoadCharacterItems(CharData charData)
        {
            List<CharItemsData> charItems = new List<CharItemsData>();
            // Read all entries from the table
            string query = "SELECT " + new CharItemsData().GetFieldsString() + " FROM character_create_items WHERE character_create_id = "
                + charData.id + " AND isactive = 1"; ;

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read all the data
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    CharItemsData display = new CharItemsData();
                    display.id = int.Parse(data["id"]);
                    display.charId = int.Parse(data["character_create_id"]);
                    display.itemId = int.Parse(data["item_id"]);
                    display.count = int.Parse(data["count"]);
                    display.equipped = bool.Parse(data["equipped"]);
                    charItems.Add(display);
                }
            }
            charData.charItems = charItems;
        }

        public void LoadSelectList()
        {
            //string[] selectList = new string[dataRegister.Count];
        //    displayList = new string[dataRegister.Count];
            int i = 0;
            if (!linkedTablesLoaded)
            {
                ServerOptionChoices.LoadAtavismChoiceOptions("Race", false, out raceIds, out raceOptions);
                ServerOptionChoices.LoadAtavismChoiceOptions("Class", false, out classIds, out classOptions);
            }
            foreach (int displayID in dataRegister.Keys)
            {
                //selectList [i] = displayID + ". " + dataRegister [displayID].name;
                int selectedRace = GetOptionPosition(dataRegister[displayID].race, raceIds);
                string raceName = raceOptions[selectedRace];
                int selectedClass = GetOptionPosition(dataRegister[displayID].aspect, classIds);
                string className = classOptions[selectedClass];
                displayList[i] = displayID + ". " + raceName + " " + className;
                i++;
            }
            //displayList = new Combobox(selectList);
        }

        bool showDuplicate = false;
        int copyClass = -1;
        int copyRace = -1;
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create a Character Template before editing it."));
                return;
            }

            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Character Template"));

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
                pos.y += ImagePack.fieldHeight;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Copy to")))
                {
                    showDuplicate = !showDuplicate;
                }
                if (showDuplicate) {
                    pos.y += ImagePack.fieldHeight;
                    int selectedRace = GetOptionPosition(copyRace, raceIds);
                    selectedRace = ImagePack.DrawSelector(pos, Lang.GetTranslate("Character Race")+":", selectedRace, raceOptions);
                    copyRace = raceIds[selectedRace];
                    pos.x += pos.width;
                    int selectedClass = GetOptionPosition(copyClass, classIds);
                    selectedClass = ImagePack.DrawSelector(pos, Lang.GetTranslate("Character Class")+":", selectedClass, classOptions);
                    copyClass = classIds[selectedClass];
                    pos.x -= pos.width;
                    pos.y += ImagePack.fieldHeight;
                    if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Copy")))
                    {
                        if (checkRaceAv())
                        {
                            Duplicate();
                            showDuplicate = false;
                        }
                    }
                }
                pos.width *= 2;
                pos.y += ImagePack.fieldHeight * 1.5f;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Template Properties:"));
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
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Restore")+" " + Lang.GetTranslate(functionName) + " " + Lang.GetTranslate("Configuration"));
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
            editingDisplay = new CharData();
            originalDisplay = new CharData();
            selectedDisplay = -1;
            LoadStatOptions();
            LoadStatList();
            
        }

        // Edit or Create
        public override void DrawEditor(Rect box, bool newI)
        {
            newEntity = newI;
            // Setup the layout
            Rect pos = box;
            pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;
            if (stats.Count == 0)
            {
                LoadStatsData();
            }

            if (!linkedTablesLoaded)
            {
                LoadInstanceOptions();
                LoadFactionOptions();
                LoadAbilityOptions();
                LoadAbilityOptions2();
                ServerOptionChoices.LoadAtavismChoiceOptions("Race", false, out raceIds, out raceOptions);
                ServerOptionChoices.LoadAtavismChoiceOptions("Class", false, out classIds, out classOptions);
                linkedTablesLoaded = true;
            }

            if (newEntity)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new Character"));
                pos.y += ImagePack.fieldHeight;
            }

            pos.width /= 2;
            int selectedRace = GetOptionPosition(editingDisplay.race, raceIds);
            selectedRace = ImagePack.DrawSelector(pos, Lang.GetTranslate("Character Race")+":", selectedRace, raceOptions);
            editingDisplay.race = raceIds[selectedRace];
            pos.x += pos.width;
            int selectedClass = GetOptionPosition(editingDisplay.aspect, classIds);
            selectedClass = ImagePack.DrawSelector(pos, Lang.GetTranslate("Character Class")+":", selectedClass, classOptions);
            editingDisplay.aspect = classIds[selectedClass];
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            int otherFactionID = GetOptionPosition(editingDisplay.faction, factionIds);
            otherFactionID = ImagePack.DrawSelector(pos, Lang.GetTranslate("Faction")+":", otherFactionID, factionOptions);
            editingDisplay.faction = factionIds[otherFactionID];
            pos.width *= 2;
            pos.y += ImagePack.fieldHeight;
            int instanceID = GetOptionPosition(editingDisplay.instance, instanceIds);
            instanceID = ImagePack.DrawSelector(pos, Lang.GetTranslate("Instance")+":", instanceID, instanceOptions);
            editingDisplay.instance = instanceIds[instanceID];
            pos.y += 1.5f * ImagePack.fieldHeight;
            editingDisplay.Spawn = ImagePack.DrawGameObject(pos, Lang.GetTranslate("Drag a Game Object to get its Position")+":", editingDisplay.Spawn, 0.5f);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.Position = ImagePack.Draw3DPosition(pos, Lang.GetTranslate("Or insert manually a Spawn Location")+":", editingDisplay.Position);
            pos.y += ImagePack.fieldHeight;
            pos.width /= 3;
            editingDisplay.orientation = ImagePack.DrawField(pos, Lang.GetTranslate("Orientation")+":", editingDisplay.orientation);
            //pos.x += pos.width;
            pos.width *= 3;
            pos.y += ImagePack.fieldHeight;

            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Starting Respawn Data")+":");
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("Only used if your game does not use the Graveyard System"));
            pos.y += ImagePack.fieldHeight;
            instanceID = GetOptionPosition(editingDisplay.respawnInstance, instanceIds);
            instanceID = ImagePack.DrawSelector(pos, Lang.GetTranslate("Respawn Instance")+":", instanceID, instanceOptions);
            editingDisplay.respawnInstance = instanceIds[instanceID];
            pos.y += 1.5f * ImagePack.fieldHeight;
            editingDisplay.ReSpawn = ImagePack.DrawGameObject(pos, Lang.GetTranslate("Drag a Game Object to get its Position")+":", editingDisplay.ReSpawn, 0.5f);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.RespawnPosition = ImagePack.Draw3DPosition(pos, Lang.GetTranslate("Or insert manually a Spawn Location")+":", editingDisplay.RespawnPosition);

            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Starting Stats"));
            pos.y += 1.5f * ImagePack.fieldHeight;
            editingDisplay.startingLevel = ImagePack.DrawField(pos, Lang.GetTranslate("Starting Level")+":", editingDisplay.startingLevel);
            pos.y += ImagePack.fieldHeight;
            pos.width /= 2;
            int selectedAbility = GetOptionPosition(editingDisplay.autoAttack, abilityIds);
            selectedAbility = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Auto Attack")+":", ref autoAtackSearch, selectedAbility, abilityOptions);
            editingDisplay.autoAttack = abilityIds[selectedAbility];

            pos.y += ImagePack.fieldHeight;

            selectedAbility = GetOptionPosition(editingDisplay.sprint, abilityIds2);
            selectedAbility = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Sprint Ability") + ":", ref sprintSearch, selectedAbility, abilityOptions2);
            editingDisplay.sprint = abilityIds2[selectedAbility];
            pos.width *= 2;
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("Enter starting stats as if starting level is 1"));
            pos.y += ImagePack.fieldHeight;
            // Stats - show a map of all stats
            pos.width /= 2;
            foreach (CharStatsData charStat in editingDisplay.charStats)
            {
                if (stats.ContainsKey(charStat.stat))
                {
                    if (stats[charStat.stat].type == 2)
                    {
                        if (charStat.statValue <= stats[charStat.stat].min)
                        {
                            if (stats[charStat.stat].onMinHit.Equals("death") || stats[charStat.stat].onMinHit.Contains("effect"))
                            {
                                charStat.statValue = stats[charStat.stat].min+1;
                            }
                        }
                    }
                    else if(stats[charStat.stat].type == 0)
                    {
                        if (maxStats.ContainsKey(charStat.stat))
                        {
                            foreach (CharStatsData cs in editingDisplay.charStats)
                            {
                                if (cs.stat.Equals(maxStats[charStat.stat]))
                                {
                                    if (charStat.statValue < cs.statValue)
                                    {
                                        charStat.statValue = cs.statValue;
                                        if (charStat.statValue == 0)
                                            charStat.statValue = 1;
                                    }
                                }
                            }
                        }
                    }
                }

                charStat.statValue = ImagePack.DrawField(pos, charStat.stat, charStat.statValue);
                pos.y += ImagePack.fieldHeight;
                charStat.levelIncrease = ImagePack.DrawField(pos, Lang.GetTranslate("Increases by")+":", charStat.levelIncrease);
                pos.x += pos.width;
                charStat.levelPercentIncrease = ImagePack.DrawField(pos, Lang.GetTranslate("And Percent")+":", charStat.levelPercentIncrease);
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight * 1.5f;
            }
            pos.width *= 2;

            if (!newEntity)
            {
                if (skillsList == null)
                    LoadSkillOptions();
                if (itemsList == null)
                    LoadItemOptions();

                pos.y += 1.5f * ImagePack.fieldHeight;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Starting Skills"));
                pos.y += 1.5f * ImagePack.fieldHeight;

                pos.width /= 3;
                /*if (editingDisplay.charSkills.Count == 0) {
                    editingDisplay.charSkills.Add(new CharSkillsData());
                }*/
                for (int i = 0; i < editingDisplay.charSkills.Count; i++)
                {
                    int selectedSkill = GetOptionPosition(editingDisplay.charSkills[i].skill, skillIds);
                    selectedSkill = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Skill")+" " + (i + 1) + ":", ref editingDisplay.charSkills[i].skillSearch, selectedSkill, skillsList);
                    editingDisplay.charSkills[i].skill = skillIds[selectedSkill];
                    pos.x += 2.1f * pos.width;
                    if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Skill")))
                    {
                        if (editingDisplay.charSkills[i].id > 0)
                            editingDisplay.skillsToBeDeleted.Add(editingDisplay.charSkills[i].id);
                        editingDisplay.charSkills.RemoveAt(i);
                    }
                    pos.x -= 2.1f * pos.width;
                    pos.y += ImagePack.fieldHeight;

                }
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Skill")))
                {
                    editingDisplay.charSkills.Add(new CharSkillsData());
                }
                pos.width *= 3;
                pos.width /= 2;
                pos.y += 1.5f * ImagePack.fieldHeight;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Starting Items"));
                pos.y += 1.5f * ImagePack.fieldHeight;

                /*if (editingDisplay.charItems.Count == 0) {
                    editingDisplay.charItems.Add(new CharItemsData());
                }*/
                for (int i = 0; i < editingDisplay.charItems.Count; i++)
                {
                    int selectedItem = GetOptionPosition(editingDisplay.charItems[i].itemId, itemIds);
                    selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item")+" " + (i + 1) + ":", ref editingDisplay.charItems[i].itemSearch, selectedItem, itemsList);
                    editingDisplay.charItems[i].itemId = itemIds[selectedItem];
                    //	pos.x += pos.width;
                    pos.y += ImagePack.fieldHeight;
                    editingDisplay.charItems[i].count = ImagePack.DrawField(pos, Lang.GetTranslate("Count") + ":", editingDisplay.charItems[i].count);
                    //	pos.x -= pos.width;
                    pos.x += pos.width;
                    editingDisplay.charItems[i].equipped = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Equipped"), editingDisplay.charItems[i].equipped);
                    // pos.x += pos.width;
                    pos.y += ImagePack.fieldHeight;
                    if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Item")))
                    {
                        if (editingDisplay.charItems[i].id > 0)
                            editingDisplay.itemsToBeDeleted.Add(editingDisplay.charItems[i].id);
                        editingDisplay.charItems.RemoveAt(i);
                    }
                    pos.x -= pos.width;
                    pos.y += ImagePack.fieldHeight;
                }
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Item")))
                {
                    editingDisplay.charItems.Add(new CharItemsData());
                }

                pos.width *= 2;
                pos.x += ImagePack.innerMargin;
            }

            pos.y += 2.5f * ImagePack.fieldHeight;
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
                EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight + 100);
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
            if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete")+" " + Lang.GetTranslate(functionName) + " " + Lang.GetTranslate("entry")+" ?", Lang.GetTranslate("Are you sure you want to delete")+" " + editingDisplay.Name + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
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
                // Insert the stats
                foreach (CharStatsData entry in editingDisplay.charStats)
                {
                    entry.charId = itemID;
                    InsertStat(entry);
                }
               
                foreach (CharSkillsData entry in editingDisplay.charSkills)
                {
                    entry.charId = itemID;
                    InsertSkill(entry);
                }
                foreach (CharItemsData entry in editingDisplay.charItems)
                {
                    entry.charId = itemID;
                    InsertItem(entry);
                }

                // Update online table to avoid access the database again			
                editingDisplay.id = itemID;
                editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
                dataLoaded = false;
                Load();
                newItemCreated = true;
                NewResult(Lang.GetTranslate("New entry inserted"));
            }
            else
            {
                NewResult(Lang.GetTranslate("Error occurred, please check the Console"));
            }
        }

        public static void InsertStat(CharStatsData entry)
        {
            // Check if the stat already exists for this character entry
            string query = "Select * from character_create_stats where character_create_id = " + entry.charId + " AND stat = '" + entry.stat + "'";
            try
            {
                // Open the connection
                DatabasePack.Connect(DatabasePack.contentDatabasePrefix);
                if (DatabasePack.con.State.ToString() != "Open")
                    DatabasePack.con.Open();
                // Use the connections to fetch data
                using (DatabasePack.con)
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, DatabasePack.con))
                    {
                        // Execute the query
                        MySqlDataReader data = cmd.ExecuteReader();
                        // If there are columns
                        if (data.HasRows)
                        {
                            return;
                        }
                        data.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
            finally
            {
            }

            query = "INSERT INTO character_create_stats";
            query += " (character_create_id, stat, value, levelIncrease, levelPercentIncrease) ";
            query += "VALUES ";
            query += " (" + entry.charId + ",'" + entry.stat + "'," + entry.statValue + "," + entry.levelIncrease + "," + entry.levelPercentIncrease + ") ";

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

        void InsertSkill(CharSkillsData entry)
        {
            string query = "INSERT INTO character_create_skills";
            query += " (character_create_id, skill) ";
            query += "VALUES ";
            query += " (" + entry.charId + "," + entry.skill + ") ";

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

        void InsertItem(CharItemsData entry)
        {
            string query = "INSERT INTO character_create_items";
            query += " (character_create_id, item_id, count, equipped) ";
            query += "VALUES ";
            query += " (" + entry.charId + "," + entry.itemId + "," + entry.count + "," + entry.equipped + ") ";

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

            // Update stats, skills and items
            foreach (CharStatsData entry in editingDisplay.charStats)
            {
                entry.charId = editingDisplay.id;
                if (entry.id < 1)
                    InsertStat(entry);
                else
                    UpdateStat(entry);
            }
            foreach (CharSkillsData entry in editingDisplay.charSkills)
            {
                if (entry.skill == -1)
                    continue;
                entry.charId = editingDisplay.id;
                if (entry.id < 1)
                    InsertSkill(entry);
                else
                    UpdateSkill(entry);
            }
            foreach (CharItemsData entry in editingDisplay.charItems)
            {
                if (entry.itemId == -1)
                    continue;
                entry.charId = editingDisplay.id;
                if (entry.id < 1)
                    InsertItem(entry);
                else
                    UpdateItem(entry);
            }

            // Delete any items that are tagged for deletion
            foreach (int itemID in editingDisplay.itemsToBeDeleted)
            {
                DeleteItem(itemID);
            }
            // Delete any skills that are tagged for deletion
            foreach (int skillID in editingDisplay.skillsToBeDeleted)
            {
                DeleteSkill(skillID);
            }

            // Update online table to avoid access the database again			
            dataLoaded = false;
            int selectedRace = GetOptionPosition(editingDisplay.race, raceIds);
            string raceName = raceOptions[selectedRace];
            int selectedClass = GetOptionPosition(editingDisplay.aspect, classIds);
            string className = classOptions[selectedClass];
            NewResult(Lang.GetTranslate("Entry")+"  " + raceName + " "+ className + " " + Lang.GetTranslate("updated"));
           Load();
         }

        // Update existing entries in the table based on the iddemo_table
        void UpdateStat(CharStatsData entry)
        {
            // Setup the update query
            string query = "UPDATE character_create_stats";
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

            // Update the database
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);
        }

        // Update existing entries in the table based on the iddemo_table
        void UpdateSkill(CharSkillsData entry)
        {
            // Setup the update query
            string query = "UPDATE character_create_skills";
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

            // Update the database
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);
        }

        // Update existing entries in the table based on the iddemo_table
        void UpdateItem(CharItemsData entry)
        {
            // Setup the update query
            string query = "UPDATE character_create_items";
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

            // Update the database
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);
        }

        void DeleteSkill(int skillID)
        {
            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, skillID.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "character_create_skills", delete);
            string query = "UPDATE character_create_skills SET isactive = 0 where id = " + skillID;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
        }

        void DeleteItem(int itemID)
        {
            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, itemID.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "character_create_items", delete);
            string query = "UPDATE character_create_items SET isactive = 0 where id = " + itemID;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
        }

        // Delete entries from the table
        void DeleteEntry()
        {
            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, tableName, delete, true);
            string query = "UPDATE " + tableName + " SET isactive = 0 where id = " + editingDisplay.id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            //delete = new Register ("character_create_id", "?character_create_id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "character_create_stats", delete, true);
            query = "UPDATE character_create_stats SET isactive = 0 where character_create_id = " + editingDisplay.id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            // Update online table to avoid access the database again			
            selectedDisplay = -1;
            newSelectedDisplay = 0;
            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " " + Lang.GetTranslate("Deleted"));
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
                where.Add(new Register("character_create_id", "?character_create_id", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "character_create_stats", where);

                where.Clear();
                where.Add(new Register("character_create_id", "?character_create_id", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "character_create_items", where);

                where.Clear();
                where.Add(new Register("character_create_id", "?character_create_id", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "character_create_skills", where);

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
            query = "UPDATE character_create_stats SET isactive = 1 where character_create_id = " + id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
            dataLoaded = false;
            dataRestoreLoaded = false;
            //Load();
            NewResult(Lang.GetTranslate("Entry Restored"));
        }


        bool checkRaceAv()
        {
            string sql = "SELECT id From " + tableName + " WHERE race=" + copyRace + " AND aspect=" + copyClass;
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sql);
            if (rows != null)
            {
                if (rows.Count > 0)
                {
                    int selectedRace = GetOptionPosition(copyRace, raceIds);
                    string raceName = raceOptions[selectedRace];
                    int selectedClass = GetOptionPosition(copyClass, classIds);
                    string className = classOptions[selectedClass];
                    NewResult(Lang.GetTranslate("Race")+" " + raceName + " " + Lang.GetTranslate("Class")+" "+className+ " " +Lang.GetTranslate("cannot be created, because it already exists in the database"));
                    return false;
                }
                else
                    return true;
            }
            else
                return false; ;

        }
        void Duplicate()
        {
            // Update name of current entry by adding "1" to the end
            editingDisplay.id = 0;
            editingDisplay.aspect = copyClass;
            editingDisplay.race = copyRace;
            foreach (CharStatsData entry in editingDisplay.charStats)
            {
                entry.charId = editingDisplay.id;
            }
            foreach (CharSkillsData entry in editingDisplay.charSkills)
            {
                entry.charId = editingDisplay.id;
            }
            foreach (CharItemsData entry in editingDisplay.charItems)
            {
                entry.charId = editingDisplay.id;
                entry.id = -1;
            }

            InsertEntry();
            state = State.Loaded;
            linkedTablesLoaded = false;
            dataLoaded = false;
            Load();
        }

    }
}