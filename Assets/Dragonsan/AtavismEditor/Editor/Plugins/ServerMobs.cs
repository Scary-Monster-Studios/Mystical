using UnityEngine;
//using UnityEditor;
//using MySql.Data;
using MySql.Data.MySqlClient;
//using System;
//using System.Data;
//using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Atavism
{
    // Handles the Mob Configuration
    public class ServerMobs : AtavismDatabaseFunction
    {

        public new Dictionary<int, Mob> dataRegister;
        public new Mob editingDisplay;
        public new Mob originalDisplay;

        public int[] itemIds = new int[] { -1 };
        public string[] itemsList = new string[] { "~ none ~" };

        public int[] factionIds = new int[] { -1 };
        public string[] factionOptions = new string[] { "~ none ~" };

        public int[] mobTypeIds = new int[] { -1 };
        public string[] mobTypeOptions = new string[] { "~ none ~" };
        public string[] speciesOptions = new string[] { "~ none ~" };

        public int[] abilityIds = new int[] { -1 };
        public string[] abilityOptions = new string[] { "~ none ~" };

        public int[] skillIds = new int[] { -1 };
        public string[] skillOptions = new string[] { "~ none ~" };

        public static int[] mobIds = new int[] { -1 };
        public static string[] mobOptions = new string[] { "~ none ~" };
        public static GUIContent[] GuiMobOptions = new GUIContent[] { new GUIContent("~ none ~") };
        public string[] weaponTypeOptions = new string[] { "~ none ~" };

        // Damage Types
        public string[] damageOptions = new string[] { "~ none ~" };

        public string[] statsList = null;
        string weaponSearch1 = "";
        string weaponSearch2 = "";
        string abilitySearchAuto = "";
        string abilitySearch1 = "";
        string abilitySearch2 = "";
        string abilitySearch3 = "";
        string skinLootSearch = "";
        List<string> lootListSearch = new List<string>();
        bool _duplicate = false;
        // Use this for initialization
        public ServerMobs()
        {
            functionName = "Mobs";
            // Database tables name
            tableName = "mob_templates";
            functionTitle = "Mobs Configuration";
            loadButtonLabel = "Load Mobs";
            notLoadedText = "No Mob loaded.";
            // Init
            dataRegister = new Dictionary<int, Mob>();

            editingDisplay = new Mob();
            originalDisplay = new Mob();
            lootListSearch.Clear();
            weaponSearch1 = "";
            weaponSearch2 = "";
            abilitySearchAuto = "";
            abilitySearch1 = "";
            abilitySearch2 = "";
            abilitySearch3 = "";
            skinLootSearch = "";
        }
        void resetSearch(bool fokus)
        {
            lootListSearch.Clear();
            weaponSearch1 = "";
            weaponSearch2 = "";
            abilitySearchAuto = "";
            abilitySearch1 = "";
            abilitySearch2 = "";
            abilitySearch3 = "";
            skinLootSearch = "";
           if(fokus) GUI.FocusControl(null);
        }
        public override void Activate()
        {
            
            linkedTablesLoaded = false;
            resetSearch(true);

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

        public void LoadDamageOptions()
        {
          
                // Read all entries from the table
                string query = "SELECT name FROM damage_type where isactive = 1";

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
                    damageOptions = new string[rows.Count];
                    foreach (Dictionary<string, string> data in rows)
                    {
                        optionsId++;
                        damageOptions[optionsId - 1] = data["name"];
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
                skillIds[optionsId] = 0;
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
                    statsList = new string[rows.Count + 1];
                    statsList[optionsId] = "~ none ~";
                    foreach (Dictionary<string, string> data in rows)
                    {
                        optionsId++;
                        statsList[optionsId] = data["name"];
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

        public static void LoadMobOptions()
        {
            string query = "SELECT id, name FROM mob_templates where isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                mobOptions = new string[rows.Count + 1];
                mobOptions[optionsId] = "~ none ~";
                mobIds = new int[rows.Count + 1];
                mobIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    mobOptions[optionsId] = data["id"] + ":" + data["name"];
                    mobIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }
        public static void LoadMobOptions(bool gui)
        {
            if (!gui)
            {
                LoadMobOptions();
                return;
            }
            string query = "SELECT id, name FROM mob_templates where isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                GuiMobOptions = new GUIContent[rows.Count + 1];
                GuiMobOptions[optionsId] = new GUIContent("~ none ~");
                mobIds = new int[rows.Count + 1];
                mobIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    GuiMobOptions[optionsId] = new GUIContent(data["id"] + ":" + data["name"]);
                    mobIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }


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

        public static Mob LoadMobTemplateModel(int id)
        {
            // Read all entries from the table
            string query = "SELECT display1,aggro_radius,name,displayName FROM mob_templates where isactive = 1 AND id =" + id;

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read all the data
            Mob display = new Mob();
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                     display.display1 = data["display1"];
                     display.aggro_range = int.Parse(data["aggro_radius"]);
                     display.Name = data["name"];
                     display.displayName = data["displayName"];
                }
            }

            return display;
        }

        public static int LoadMobTemplateAggroRadius(int id)
        {
            // Read all entries from the table
            string query = "SELECT aggro_radius FROM mob_templates where isactive = 1 AND id =" + id;

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read all the data
            Mob display = new Mob();
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    return  int.Parse(data["aggro_radius"]);
                }
            }

            return 17;
        }



        // Load Database Data
        Mob LoadEntity(int id)
        {
           // Read all entries from the table
            string query = "SELECT " + originalDisplay.GetFieldsString() + " FROM " + tableName + " where isactive = 1 AND id =" + id;

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read all the data
            Mob display = new Mob();
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    //foreach(string key in data.Keys)
                    //	Debug.Log("Name[" + key + "]:" + data[key]);
                    //return;
                    display.id = int.Parse(data["id"]);
                    display.category = int.Parse(data["category"]);
                    display.Name = data["name"];
                    display.displayName = data["displayName"];
                    display.subTitle = data["subTitle"];
                    display.mobType = int.Parse(data["mobType"]);
                    display.species = data["species"];
                    display.subspecies = data["subSpecies"];
                    display.faction = int.Parse(data["faction"]);
                    display.display1 = data["display1"];
                    display.scale = float.Parse(data["scale"]);
                    display.hitBox = int.Parse(data["hitbox"]);
                    display.baseAnimationState = int.Parse(data["baseAnimationState"]);
                    display.speedWalk = float.Parse(data["speed_walk"]);
                    display.speedRun = float.Parse(data["speed_run"]);
                    display.primaryWeapon = int.Parse(data["primaryWeapon"]);
                    display.secondaryWeapon = int.Parse(data["secondaryWeapon"]);
                    display.attackable = bool.Parse(data["attackable"]);
                    display.minLevel = int.Parse(data["minLevel"]);
                    display.maxLevel = int.Parse(data["maxLevel"]);
                    display.minDamage = int.Parse(data["minDmg"]);
                    display.maxDamage = int.Parse(data["maxDmg"]);
                    display.damageType = data["dmgType"];
                    display.attackSpeed = float.Parse(data["attackSpeed"]);
                    display.questCategory = data["questCategory"];
                    display.autoAttack = int.Parse(data["autoAttack"]);
                    display.attackDistance = float.Parse(data["attackDistance"]);
                    display.ability0 = int.Parse(data["ability0"]);
                    display.abilityStatReq0 = data["abilityStatReq0"];
                    display.abilityStatPercent0 = int.Parse(data["abilityStatPercent0"]);
                    display.ability1 = int.Parse(data["ability1"]);
                    display.abilityStatReq1 = data["abilityStatReq1"];
                    display.abilityStatPercent1 = int.Parse(data["abilityStatPercent1"]);
                    display.ability2 = int.Parse(data["ability2"]);
                    display.abilityStatReq2 = data["abilityStatReq2"];
                    display.abilityStatPercent2 = int.Parse(data["abilityStatPercent2"]);

                    display.exp = int.Parse(data["exp"]);
                    display.addexplev = int.Parse(data["addExplev"]);
                    display.skinningLootTable = int.Parse(data["skinningLootTable"]);
                    display.skinningLevelReq = int.Parse(data["skinningLevelReq"]);
                    display.skinningLevelMax = int.Parse(data["skinningLevelMax"]);
                    display.skinningSkillId = int.Parse(data["skinningSkillId"]);
                    display.skinningSkillExp = int.Parse(data["skinningSkillExp"]);
                    display.skinningWeaponReq = data["skinningWeaponReq"];

                    display.aggro_range = int.Parse(data["aggro_radius"]);
                    display.linked_aggro_range = int.Parse(data["link_aggro_range"]);
                    display.linked_aggro_get = bool.Parse(data["get_link_aggro"]);
                    display.linked_aggro_send = bool.Parse(data["send_link_aggro"]);
                    display.chasing_distance = int.Parse(data["chasing_distance"]);

                    display.isLoaded = true;
                    //Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
                 }
            }
              LoadMobStat(display);
            LoadMobStatRestore(display);
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



        public void LoadMobStatRestore(Mob display)
        {

            string tableName = "mob_stat";
            // Read all entries from the table
            string query = "SELECT " + new MobStat().GetFieldsString() + " FROM " + tableName + " where mobTemplate = " + display.id + " AND isactive = 0";

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
                    MobStat statEntry = new MobStat();
                    statEntry.mobTemplate = display.id;
                    statEntry.id = int.Parse(data["id"]);
                    statEntry.stat = data["stat"];
                    statEntry.value = int.Parse(data["value"]);
                    display.statsRestore.Add(statEntry);
                }
            }
        }


        public void LoadMobStat(Mob display)
        {

            string tableName = "mob_stat";
            // Read all entries from the table
            string query = "SELECT " + new MobStat().GetFieldsString() + " FROM " + tableName + " where mobTemplate = " + display.id + " AND isactive = 1";

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
                    MobStat statEntry = new MobStat();
                    statEntry.mobTemplate = display.id;
                    statEntry.id = int.Parse(data["id"]);
                    statEntry.stat = data["stat"];
                    statEntry.value = int.Parse(data["value"]);
                    display.stats.Add(statEntry);
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create a Mob Template before editing it."));
                return;
            }


            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Mob Template"));

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

                pos.width *= 2;
                pos.y += ImagePack.fieldHeight * 1.5f;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Duplicate (Except Loot)")))
                {
                    Duplicate();
                }


                pos.y += ImagePack.fieldHeight * 1.5f;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Mob Properties:"));
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You dont have deleted Mob Templates ."));
                return;
            }

            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Restore Mob Template"));
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
            editingDisplay = new Mob();
            originalDisplay = new Mob();
            selectedDisplay = -1;
        }
        // Edit or Create
        public override void DrawEditor(Rect box, bool newItem)
        {
            newEntity = newItem;
            // Intercept the drawing of the editor if editing mob loot is true
            if (editingLoot)
            {
                DrawMobLootEditor(box);
                return;
            }

            if (!linkedTablesLoaded)
            {
                // Load items
                LoadItemList();
                LoadFactionOptions();
                LoadDamageOptions();
                LoadAbilityOptions();
                LoadSkillOptions();
                LoadStatOptions();
                LoadTableList();
                ServerOptionChoices.LoadAtavismChoiceOptions("Mob Type", false, out mobTypeIds, out mobTypeOptions);
                speciesOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Species", true);
                weaponTypeOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Weapon Type", true);
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new mob"));
                pos.y += 1.5f * ImagePack.fieldHeight;
            }

            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name") + ":", editingDisplay.Name, 0.75f);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.displayName = ImagePack.DrawField(pos, Lang.GetTranslate("Display name") + ":", editingDisplay.displayName, 0.75f);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.subTitle = ImagePack.DrawField(pos, Lang.GetTranslate("Subtitle")+":", editingDisplay.subTitle, 0.75f);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.display1 = ImagePack.DrawGameObject(pos, Lang.GetTranslate("Game Object")+":", editingDisplay.display1, 0.75f);
            pos.y += ImagePack.fieldHeight;
            pos.width /= 2;
            editingDisplay.species = ImagePack.DrawSelector(pos, Lang.GetTranslate("Species")+":", editingDisplay.species, speciesOptions);
            pos.x += pos.width;
            editingDisplay.subspecies = ImagePack.DrawField(pos, Lang.GetTranslate("Subspecies")+ ":", editingDisplay.subspecies);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;


           // editingDisplay.mobType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Mob Type")+":", editingDisplay.mobType, mobTypeOptions);
            int selected = GetOptionPosition(editingDisplay.mobType, mobTypeIds);
            selected = ImagePack.DrawSelector(pos, Lang.GetTranslate("Mob Type") + ":", selected, mobTypeOptions);
            editingDisplay.mobType = mobTypeIds[selected];

            pos.x += pos.width;
            int otherFactionID = GetPositionOfFaction(editingDisplay.faction);
            otherFactionID = ImagePack.DrawSelector(pos, Lang.GetTranslate("Faction")+":", otherFactionID, factionOptions);
            editingDisplay.faction = factionIds[otherFactionID];
            pos.x -= pos.width;
            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Mob Display"));
            pos.y += 1.5f * ImagePack.fieldHeight;
            //	editingDisplay.scale = ImagePack.DrawField (pos, "Scale:", editingDisplay.scale);
            //	pos.x += pos.width;
            editingDisplay.hitBox = ImagePack.DrawField(pos, Lang.GetTranslate("Hit Range")+":", editingDisplay.hitBox);
            //	pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.speedWalk = ImagePack.DrawField(pos, Lang.GetTranslate("Walk Speed")+":", editingDisplay.speedWalk);
            pos.x += pos.width;
            editingDisplay.speedRun = ImagePack.DrawField(pos, Lang.GetTranslate("Run Speed")+":", editingDisplay.speedRun);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            int selectedItem = GetPositionOfItem(editingDisplay.primaryWeapon);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Prim. Weapon")+":", ref weaponSearch1, selectedItem, itemsList);
            editingDisplay.primaryWeapon = itemIds[selectedItem];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            int selectedItem2 = GetPositionOfItem(editingDisplay.secondaryWeapon);
            selectedItem2 = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Sec. Weapon")+":", ref weaponSearch2, selectedItem2, itemsList);
            editingDisplay.secondaryWeapon = itemIds[selectedItem2];
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.baseAnimationState = ImagePack.DrawSelector(pos, Lang.GetTranslate("Base Animation")+":", editingDisplay.baseAnimationState - 1, editingDisplay.baseAnimationStateOptions) + 1;
            pos.x += pos.width;
            //	editingDisplay.category = ImagePack.DrawField (pos, "Category:", editingDisplay.category);
            pos.x -= pos.width;
            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Mob Combat Settings"));
            pos.y += 1.5f * ImagePack.fieldHeight;
            editingDisplay.exp = ImagePack.DrawField(pos, Lang.GetTranslate("Expirience min Lev") + ":", editingDisplay.exp);
            pos.x += pos.width;
            editingDisplay.addexplev = ImagePack.DrawField(pos, Lang.GetTranslate("Additional Exp / Lev") + ":", editingDisplay.addexplev);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.minLevel = ImagePack.DrawField(pos, Lang.GetTranslate("Min. Level") + ":", editingDisplay.minLevel);
            pos.x += pos.width;
            editingDisplay.maxLevel = ImagePack.DrawField(pos, Lang.GetTranslate("Max. Level") + ":", editingDisplay.maxLevel);
            pos.y += ImagePack.fieldHeight;
            pos.x -= pos.width;
            editingDisplay.minDamage = ImagePack.DrawField(pos, Lang.GetTranslate("Min. Damage")+":", editingDisplay.minDamage);
            pos.x += pos.width;
            editingDisplay.maxDamage = ImagePack.DrawField(pos, Lang.GetTranslate("Max. Damage")+":", editingDisplay.maxDamage);
            pos.y += ImagePack.fieldHeight;
            pos.x -= pos.width;
            editingDisplay.attackable = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Is Mob Attackable")+"?", editingDisplay.attackable);
            pos.x += pos.width;
            editingDisplay.aggro_range = ImagePack.DrawField(pos, "Aggro Rafius:", editingDisplay.aggro_range);
            pos.y += ImagePack.fieldHeight;
            pos.x -= pos.width;
            editingDisplay.chasing_distance = ImagePack.DrawField(pos, "Chasing Distance:", editingDisplay.chasing_distance);
            pos.x += pos.width;
            editingDisplay.linked_aggro_range = ImagePack.DrawField(pos, "Linked Aggro Radius:", editingDisplay.linked_aggro_range);
            pos.y += ImagePack.fieldHeight;
            pos.x -= pos.width;
            editingDisplay.linked_aggro_send = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Linked Aggro Can Send") + "?", editingDisplay.linked_aggro_send);
            pos.x += pos.width;
            editingDisplay.linked_aggro_get = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Linked Aggro Can Get") + "?", editingDisplay.linked_aggro_get);
            //    editingDisplay.attackSpeed = ImagePack.DrawField(pos, "Attack Speed:", editingDisplay.attackSpeed);
            pos.x -= pos.width;
            //	pos.y += ImagePack.fieldHeight;
            //	editingDisplay.damageType = ImagePack.DrawSelector (pos, "Damage Type:", editingDisplay.damageType, damageOptions); 
         //   pos.x += pos.width;
            //	editingDisplay.questCategory = ImagePack.DrawField (pos, "Quest Category:", editingDisplay.questCategory);
           // pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            int selectedAbility = GetPositionOfAbility(editingDisplay.autoAttack);
            selectedAbility = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Auto Attack")+":", ref abilitySearchAuto, selectedAbility, abilityOptions);
            editingDisplay.autoAttack = abilityIds[selectedAbility];
            pos.y += ImagePack.fieldHeight;
            //pos.x += pos.width;
            editingDisplay.attackDistance = ImagePack.DrawField(pos, Lang.GetTranslate("Attack Distance")+":", editingDisplay.attackDistance);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedAbility = GetPositionOfAbility(editingDisplay.ability0);
            selectedAbility = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Ability")+" 1:", ref abilitySearch1, selectedAbility, abilityOptions);
            editingDisplay.ability0 = abilityIds[selectedAbility];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.abilityStatReq0 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Use when")+":", editingDisplay.abilityStatReq0, statsList);
            pos.x += pos.width;
            editingDisplay.abilityStatPercent0 = ImagePack.DrawField(pos, Lang.GetTranslate("Is less than %")+":", editingDisplay.abilityStatPercent0);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedAbility = GetPositionOfAbility(editingDisplay.ability1);
            selectedAbility = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Ability")+" 2:", ref abilitySearch2, selectedAbility, abilityOptions);
            editingDisplay.ability1 = abilityIds[selectedAbility];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.abilityStatReq1 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Use when")+":", editingDisplay.abilityStatReq1, statsList);
            pos.x += pos.width;
            editingDisplay.abilityStatPercent1 = ImagePack.DrawField(pos, Lang.GetTranslate("Is less than %")+":", editingDisplay.abilityStatPercent1);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedAbility = GetPositionOfAbility(editingDisplay.ability2);
            selectedAbility = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Ability")+" 3:", ref abilitySearch3, selectedAbility, abilityOptions);
            editingDisplay.ability2 = abilityIds[selectedAbility];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.abilityStatReq2 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Use when")+":", editingDisplay.abilityStatReq2, statsList);
            pos.x += pos.width;
            editingDisplay.abilityStatPercent2 = ImagePack.DrawField(pos, Lang.GetTranslate("Is less than %")+":", editingDisplay.abilityStatPercent2);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;

            pos.y += ImagePack.fieldHeight;
            int selectedLootTable = GetPositionOfTable(editingDisplay.skinningLootTable);
            selectedLootTable = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Skinning Loot Table")+":", ref skinLootSearch, selectedLootTable, tablesList);
            editingDisplay.skinningLootTable = tableIds[selectedLootTable];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.skinningLevelReq = ImagePack.DrawField(pos, Lang.GetTranslate("Skinning Level Req") + ":", editingDisplay.skinningLevelReq);
            pos.x += pos.width;
            editingDisplay.skinningLevelMax = ImagePack.DrawField(pos, Lang.GetTranslate("Skinning Level Max") + ":", editingDisplay.skinningLevelMax);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
         
            int selectedSkill = GetOptionPosition(editingDisplay.skinningSkillId, skillIds);
            selectedSkill = ImagePack.DrawSelector(pos, Lang.GetTranslate("Skinning Skill") + ":", selectedSkill, skillOptions);
            editingDisplay.skinningSkillId = skillIds[selectedSkill];
            pos.x += pos.width;

            editingDisplay.skinningSkillExp = ImagePack.DrawField(pos, Lang.GetTranslate("Skinning Skill Exp") + ":", editingDisplay.skinningSkillExp);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;

            editingDisplay.skinningWeaponReq = ImagePack.DrawSelector(pos, Lang.GetTranslate("Skinning Weapon Req") + ":", editingDisplay.skinningWeaponReq, weaponTypeOptions);
            pos.y += ImagePack.fieldHeight;




            // Mob Individual Stats
            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Mob Individual Stats"));
            pos.y += 1.5f * ImagePack.fieldHeight;

            for (int i = 0; i < editingDisplay.stats.Count; i++)
            {
                editingDisplay.stats[i].stat = ImagePack.DrawSelector(pos, Lang.GetTranslate("Stat")+":", editingDisplay.stats[i].stat, statsList);
                //mobLoot[i].tableId = ImagePack.DrawField (pos, "Loot Table:", mobLoot[i].tableId);
                //pos.y += ImagePack.fieldHeight;
                pos.x += pos.width;
                editingDisplay.stats[i].value = ImagePack.DrawField(pos, Lang.GetTranslate("Value")+":", editingDisplay.stats[i].value);
                pos.y += ImagePack.fieldHeight;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Entry")))
                {
                    if (editingDisplay.stats[i].id > 0)
                        editingDisplay.mobStatToBeDeleted.Add(editingDisplay.stats[i].id);
                    editingDisplay.stats.RemoveAt(i);
                }
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
            }
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Mob Stat")))
            {
                MobStat statEntry = new MobStat();
                statEntry.mobTemplate = editingDisplay.id;
                editingDisplay.stats.Add(statEntry);
            }
            pos.width *= 2;


            // Mob loot
            pos.x -= ImagePack.innerMargin;
            pos.width /= 3;
            if (!newEntity)
            {
                pos.y += 1.5f * ImagePack.fieldHeight;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Edit Mob Loot")))
                {
                    inspectorScrollPosition.y = 0;
                    EditMobLoot();
                }
            }

            // Save data		
            //pos.x -= ImagePack.innerMargin;
            pos.y += 1.5f * ImagePack.fieldHeight;
            //pos.width /= 3;
             showSave = true;
       
            // Delete data
            if (!newEntity)
            {
                showDelete = true;
            }
            else
            {
                showDelete = false;
            }

            // Cancel editing
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
            if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete") + " " + Lang.GetTranslate(functionName) + " " + Lang.GetTranslate("entry") + " ?", Lang.GetTranslate("Are you sure you want to delete") + " " + editingDisplay.Name + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
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
            editingDisplay = originalDisplay.Clone();
            if (newEntity)
                state = State.New;
            else
                state = State.Loaded;
        }

        // Insert new entries into the table
        void InsertEntry()
        {
            NewResult(Lang.GetTranslate("Inserting..."));

            string sqlmob = "SELECT id FROM "+ tableName+ "  WHERE name ='" + editingDisplay.Name + "'";
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlmob);
        
            if ((rows != null) && (rows.Count > 0))
            {
                if (_duplicate)
                {
                    editingDisplay.Name = editingDisplay.Name+"|";
                    InsertEntry();
                    return;
                }
                NewResult("Error:"+Lang.GetTranslate("Can not be saved because the name already exists"));
                return;
            }


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

            // If the insert failed, don't insert the spawn marker
            if (mobID != -1)
            {
                editingDisplay.id = mobID;
                foreach (MobStat mobStatEntry in editingDisplay.stats)
                {
                    mobStatEntry.mobTemplate = mobID;
                }

                SaveStatChanges(editingDisplay);
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

            SaveStatChanges(editingDisplay);

            // Update online table to avoid access the database again			
            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry")+"  " + editingDisplay.Name + " " + Lang.GetTranslate("updated"));
               Load();
     }

        // Delete entries from the table
        void DeleteEntry()
        {
            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, tableName, delete);
            string query = "UPDATE " + tableName + " SET isactive = 0 where id = " + editingDisplay.id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            // Delete all stats
            //delete = new Register ("mobTemplate", "?mobTemplate", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "mob_stat", delete);
            query = "UPDATE mob_stat SET isactive = 0 where mobTemplate = " + editingDisplay.id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            // Now delete all spawns related to delete mob
            //delete = new Register ("mobTemplate", "?mobTemplate", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "spawn_data", delete);
            query = "UPDATE spawn_data SET isactive = 0 where mobTemplate = " + editingDisplay.id;
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
                where.Add(new Register("mobTemplate", "?mobTemplate", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "mob_stat", where);
                where.Clear();
                where.Add(new Register("mobTemplate", "?mobTemplate", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "mob_loot", where);

                where.Clear();
                where.Add(new Register("mobTemplate", "?mobTemplate", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "spawn_data", where);

                dataLoaded = false;
                dataRestoreLoaded = false;
                // Load();
                NewResult(Lang.GetTranslate("Entry Permanent Deleted"));
            }
        }

        void SaveStatChanges(Mob editingDisplay)
        {
            NewResult(Lang.GetTranslate("Saving Changes..."));
            string tableName = "mob_stat";
            foreach (MobStat mobStatEntry in editingDisplay.stats)
            {
                mobStatEntry.mobTemplate = editingDisplay.id;
                if (mobStatEntry.id < 1)
                {
                    // Setup the update query
                    string query = "INSERT INTO " + tableName;
                    query += " (" + mobStatEntry.FieldList("", ", ") + ") ";
                    query += "VALUES ";
                    query += " (" + mobStatEntry.FieldList("?", ", ") + ") ";

                    int mobStatID = -1;

                    // Setup the register data		
                    List<Register> update = new List<Register>();
                    foreach (string field in mobStatEntry.fields.Keys)
                    {
                        update.Add(mobStatEntry.fieldToRegister(field));
                    }

                    // Update the database
                    mobStatID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);
                    if (mobStatID != -1)
                    {
                        mobStatEntry.id = mobStatID;
                        NewResult(Lang.GetTranslate("Update successful"));
                    }
                }
                else
                {
                    // Setup the update query
                    string query = "UPDATE " + tableName;
                    query += " SET ";
                    query += mobStatEntry.UpdateList();
                    query += " WHERE id=?id";

                    // Setup the register data		
                    List<Register> update = new List<Register>();
                    foreach (string field in mobStatEntry.fields.Keys)
                    {
                        update.Add(mobStatEntry.fieldToRegister(field));
                    }
                    update.Add(new Register("id", "?id", MySqlDbType.Int32, mobStatEntry.id.ToString(), Register.TypesOfField.Int));

                    // Update the database
                    DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);
                    NewResult(Lang.GetTranslate("Update successful"));
                }
            }

            // And now delete any Objectives that are tagged for deletion
            foreach (int mobStatID in editingDisplay.mobStatToBeDeleted)
            {
                DeleteMobStat(mobStatID);
            }
        }

        void DeleteMobStat(int mobStatID)
        {
            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, mobStatID.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "mob_stat", delete);
            string query = "UPDATE mob_stat SET isactive = 0 where id = " + mobStatID;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
        }

        void Duplicate()
        {
            // Update name of current entry by adding "1" to the end
            editingDisplay.Name = editingDisplay.Name + " (Clone)";
            foreach (MobStat mobStatEntry in editingDisplay.stats)
            {
                mobStatEntry.id = -1;

            }
            _duplicate = true;
            InsertEntry();
            state = State.Loaded;
        }

        private int GetPositionOfItem(int itemId)
        {
            for (int i = 0; i < itemIds.Length; i++)
            {
                if (itemIds[i] == itemId)
                    return i;
            }
            return 0;
        }

        private int GetPositionOfFaction(int factionID)
        {
            for (int i = 0; i < factionIds.Length; i++)
            {
                if (factionIds[i] == factionID)
                    return i;
            }
            return 0;
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

        #region Mob Loot
        public List<MobLoot> mobLoot = new List<MobLoot>();
        public List<int> mobLootToBeDeleted = new List<int>();
        int maxLootTables = 10;
        bool editingLoot = false;

        public int[] tableIds = new int[] { -1 };
        public string[] tablesList = new string[] { "~ none ~" };
        //bool tablesLoaded = false;

        void EditMobLoot()
        {
            lootListSearch.Clear();
            editingLoot = true;
            LoadMobLoot();
            LoadTableList();
           
        }

        public void LoadMobLoot()
        {
            mobLoot.Clear();

            string tableName = "mob_loot";
            // Read all entries from the table
            string query = "SELECT " + new MobLoot().GetFieldsString() + " FROM " + tableName + " where mobTemplate = " + editingDisplay.id + " AND isactive = 1";

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
                    MobLoot lootEntry = new MobLoot();
                    lootEntry.id = int.Parse(data["id"]);
                    lootEntry.category = int.Parse(data["category"]);
                    lootEntry.tableId = int.Parse(data["lootTable"]);
                    lootEntry.chance = float.Parse(data["dropChance"]);
                    lootEntry.count = int.Parse(data["count"]);
                    lootEntry.mobTemplate = editingDisplay.id;
                    mobLoot.Add(lootEntry);
                }
            }
        }

        public void LoadMobLootRestore()
        {
            mobLoot.Clear();

            string tableName = "mob_loot";
            // Read all entries from the table
            string query = "SELECT " + new MobLoot().GetFieldsString() + " FROM " + tableName + " where mobTemplate = " + editingDisplay.id + " AND isactive = 0";

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
                    MobLoot lootEntry = new MobLoot();
                    lootEntry.id = int.Parse(data["id"]);
                    lootEntry.category = int.Parse(data["category"]);
                    lootEntry.tableId = int.Parse(data["lootTable"]);
                    lootEntry.chance = float.Parse(data["dropChance"]);
                    lootEntry.count = int.Parse(data["count"]);
                    lootEntry.mobTemplate = editingDisplay.id;
                    mobLoot.Add(lootEntry);
                }
            }
        }

        void DrawMobLootEditor(Rect box)
        {
            // First check if the selected mob has been changed
            if (mobLoot.Count > 0 && editingDisplay.id != mobLoot[0].mobTemplate)
            {
                LoadMobLoot();
            }
            // Setup the layout
            Rect pos = box;
            pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;

            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Set mob loot tables for mob")+": " + editingDisplay.Name);
            pos.y += ImagePack.fieldHeight;

            pos.width = pos.width / 2;
            for (int i = 0; i < maxLootTables; i++)
            {
                if (mobLoot.Count > i)
                {
                    if (lootListSearch.Count <= i)
                        lootListSearch.Add("");
                    string search = lootListSearch[i];
                    int selectedItem = GetPositionOfTable(mobLoot[i].tableId);
                    selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Loot Table") + ":", ref search, selectedItem, tablesList);
                    //selectedItem = ImagePack.DrawSelector(pos, Lang.GetTranslate("Loot Table")+":", selectedItem, tablesList);
                    mobLoot[i].tableId = tableIds[selectedItem];
                    lootListSearch[i] = search;
                    //mobLoot[i].tableId = ImagePack.DrawField (pos, "Loot Table:", mobLoot[i].tableId);
                    //pos.y += ImagePack.fieldHeight;
                   // pos.width = pos.width / 2;
                    pos.y += ImagePack.fieldHeight;
                  //  pos.x += pos.width;
                    mobLoot[i].count = ImagePack.DrawField(pos, Lang.GetTranslate("Drop Count") + ":", mobLoot[i].count);
                    pos.x += pos.width;
                   // pos.y += ImagePack.fieldHeight;
                    mobLoot[i].chance = ImagePack.DrawField(pos, Lang.GetTranslate("Drop Chance") + ":", mobLoot[i].chance);
                  //  pos.x -= pos.width;
                    pos.y += ImagePack.fieldHeight;
                    if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Entry")))
                    {
                        if (mobLoot[i].id > 0)
                            mobLootToBeDeleted.Add(mobLoot[i].id);
                        mobLoot.RemoveAt(i);
                    }
                    pos.x -= pos.width;
                   // pos.width = pos.width * 2;
                    pos.y += ImagePack.fieldHeight;
                }
            }
         //   pos.width = pos.width / 2;
            if (mobLoot.Count < maxLootTables)
            {
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Loot Table")))
                {
                    MobLoot lootEntry = new MobLoot();
                    lootEntry.mobTemplate = editingDisplay.id;
                    mobLoot.Add(lootEntry);
                }
            }
            pos.width = pos.width * 2;

            // Save data
            //pos.x -= ImagePack.innerMargin;
            pos.y += 1.4f * ImagePack.fieldHeight;
            pos.width /= 3;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Save Data")))
            {
                SaveChanges();
            }

            // Cancel editing
            pos.x += pos.width;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Back")))
            {
                editingLoot = false;
            }

            if (resultTimeout != -1 && resultTimeout > Time.realtimeSinceStartup)
            {
                pos.y += ImagePack.fieldHeight;
                ImagePack.DrawText(pos, result);
            }
        }

        void SaveChanges()
        {
            NewResult(Lang.GetTranslate("Saving Changes..."));
            string tableName = "mob_loot";
            foreach (MobLoot mobLootEntry in mobLoot)
            {
                if (mobLootEntry.id < 1)
                {
                    // Setup the update query
                    string query = "INSERT INTO " + tableName;
                    query += " (" + mobLootEntry.FieldList("", ", ") + ") ";
                    query += "VALUES ";
                    query += " (" + mobLootEntry.FieldList("?", ", ") + ") ";

                    int moblootID = -1;

                    // Setup the register data		
                    List<Register> update = new List<Register>();
                    foreach (string field in mobLootEntry.fields.Keys)
                    {
                        update.Add(mobLootEntry.fieldToRegister(field));
                    }

                    // Update the database
                    moblootID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);
                    if (moblootID != -1)
                        NewResult(Lang.GetTranslate("Update successful"));
                }
                else
                {
                    // Setup the update query
                    string query = "UPDATE " + tableName;
                    query += " SET ";
                    query += mobLootEntry.UpdateList();
                    query += " WHERE id=?id";

                    // Setup the register data		
                    List<Register> update = new List<Register>();
                    foreach (string field in mobLootEntry.fields.Keys)
                    {
                        update.Add(mobLootEntry.fieldToRegister(field));
                    }
                    update.Add(new Register("id", "?id", MySqlDbType.Int32, mobLootEntry.id.ToString(), Register.TypesOfField.Int));

                    // Update the database
                    DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);
                    NewResult(Lang.GetTranslate("Update successful"));
                }
            }

            // And now delete any Objectives that are tagged for deletion
            foreach (int mobLootID in mobLootToBeDeleted)
            {
                DeleteMobLoot(mobLootID);
            }
            NewResult(Lang.GetTranslate("Update successful"));
        }

        void DeleteMobLoot(int mobLootID)
        {
            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, mobLootID.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "mob_loot", delete);
            string query = "UPDATE mob_loot SET isactive = 0 where id = " + mobLootID;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
            NewResult("Loot Deleted");

        }
        void RestoreMobLoot(int mobLootID)
        {
            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, mobLootID.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "mob_loot", delete);
            string query = "UPDATE mob_loot SET isactive = 1 where id = " + mobLootID;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
            NewResult(Lang.GetTranslate("Loot Restored"));

        }

        private int GetPositionOfTable(int tableId)
        {
            for (int i = 0; i < tableIds.Length; i++)
            {
                if (tableIds[i] == tableId)
                    return i;
            }
            return 0;
        }

        #endregion Mob Loot
        void RestoreEntry(int id)
        {
            NewResult(Lang.GetTranslate("Restoring..."));
            string query = "UPDATE " + tableName + " SET isactive = 1 where id = " + id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            query = "UPDATE mob_stat SET isactive = 1 where mobTemplate = " + id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            query = "UPDATE spawn_data SET isactive = 1 where mobTemplate = " + editingDisplay.id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            dataLoaded = false;
            dataRestoreLoaded = false;
            //Load();
            NewResult(Lang.GetTranslate("Entry Restored"));
        }
    }
}