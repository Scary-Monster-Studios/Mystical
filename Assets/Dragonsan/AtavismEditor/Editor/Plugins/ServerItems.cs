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
    // Handles the Item Configuration
    public class ServerItems : AtavismDatabaseFunction
    {

        public new Dictionary<int, ItemData> dataRegister;
        public new ItemData editingDisplay;
        public new ItemData originalDisplay;

        public int[] abilityIds = new int[] { -1 };
        public string[] abilityOptions = new string[] { "~ none ~" };
        public int[] abilityPassIds = new int[] { -1 };
        public string[] abilityPassOptions = new string[] { "~ none ~" };

        public int[] currencyIds = new int[] { -1 };
        public string[] currencyOptions = new string[] { "~ none ~" };

        public int[] recipeIds = new int[] { -1 };
        public string[] recipeList = new string[] { "~ none ~" };

        public string[] itemTypeOptions = new string[] { "~ none ~" };
        public string[] weaponTypeOptions = new string[] { "~ none ~" };
        public string[] armorTypeOptions = new string[] { "~ none ~" };
        public string[] itemSocketTypeOptions = new string[] { "~ none ~" };
        public string[] itemAchievementOptions = new string[] { "~ none ~" };
        public int[] itemAchievementIds = new int[] { -1 };

        public string[] damageOptions = new string[] { "~ none ~" };

        public string[] itemEffectTypeOptions = new string[] { "~ none ~" };
        public string[] statOptions = new string[] { "~ none ~" };
        public string[] accountEffectTypeOptions = new string[] { "~ none ~" };

        public int[] buildObjectIds = new int[] { -1 };
        public string[] buildObjectOptions = new string[] { "~ none ~" };

        public int[] questIds = new int[] { -1 };
        public string[] questOptions = new string[] { "~ none ~" };

        public int[] requirementIds = new int[] { -1 };
        public string[] requirementOptions = new string[] { "~ none ~" };

        public int[] classIds = new int[] { -1 };
        public string[] classOptions = new string[] { "~ none ~" };

        public int[] raceIds = new int[] { -1 };
        public string[] raceOptions = new string[] { "~ none ~" };

        public int[] skillIds = new int[] { -1 };
        public string[] skillOptions = new string[] { "~ none ~" };

        public int[] factionIds = new int[] { -1 };
        public string[] factionOptions = new string[] { "~ none ~" };

        public int[] ammoIds = new int[] { -1 };
        public string[] ammoOptions = new string[] { "~ none ~" };

        public int[] mobIds = new int[] { -1 };
        public string[] mobList = new string[] { "~ none ~" };

        public int[] bonusIds = new int[] { -1 };
        public string[] bonusOptions = new string[] { "~ none ~" };
        public string[] bonusOptionsParam = new string[] { "" };
        public string[] bonusOptionsCode = new string[] { "" };


        public string[] stationOptions = new string[] { "~ none ~" };

        public int[] enchantProfileIds = new int[] { -1 };
        public string[] enchantProfileOptions = new string[] { "~ none ~" };
        string[] itemQualityOptions = new string[] { "~ none ~" };
        bool qualityloaded = false;
        // Handles the prefab creation, editing and save
        private ItemPrefab item = null;
        
        // Filter/Search inputs
        private string currencySearchInput = "";
        private string enchantSearchInput = "";
        private string searcheAchievement="";
        private string searcheaa = "";
        private string passiveAbilijtySerach = "";
        private List<string> effectSearchInput = new List<string>();
        
        // Use this for initialization
        public ServerItems()
        {
            functionName = "Items";
            // Database tables name
            tableName = "item_templates";
            functionTitle = "Item Configuration";
            loadButtonLabel = "Load Items";
            notLoadedText = "No Item loaded.";
            // Init
            dataRegister = new Dictionary<int, ItemData>();

            editingDisplay = new ItemData();
            originalDisplay = new ItemData();
            effectSearchInput.Clear();
            qualityloaded = false;
        }
        void resetSearch(bool fokus)
        {
            currencySearchInput = "";
            enchantSearchInput = "";
            searcheAchievement = "";
            searcheaa = "";
            passiveAbilijtySerach = "";
            effectSearchInput.Clear();
           if(fokus) GUI.FocusControl(null);
        }

        public override void Activate()
        {
            linkedTablesLoaded = false;
            resetSearch(true);
        }

        public static GUIContent[] LoadSlotsOptions(bool addRoot = false, bool addNone=false)
        {
            GUIContent[] options = new GUIContent[0];
            // Read all entries from the table
            string query = "SELECT name FROM item_slots where isactive = 1";

            List<Dictionary<string, string>> rows = null;//DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                options = new GUIContent[rows.Count+ (addRoot?1:0)+(addNone?1:0) ];
                if (addNone )
                {
                    
                    options[optionsId] = new GUIContent("None");
                    optionsId++;
                }
                if (addRoot )
                {
                    
                    options[optionsId] = new GUIContent("Root");
                    optionsId++;
                }
                

                foreach (Dictionary<string, string> data in rows)
                {
                    options[optionsId] = new GUIContent(data["name"]);
                    optionsId++;
                }
            }
            else
            {
                options = new GUIContent[rows.Count+ (addRoot?1:0)+(addNone?1:0) ];
                if (addNone )
                {
                    
                    options[optionsId] = new GUIContent("None");
                    optionsId++;
                }
                if (addRoot )
                {
                    
                    options[optionsId] = new GUIContent("Root");
                    optionsId++;
                }
            }

            return options;
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
        public void LoadPassiveAbilityOptions()
        {
            string query = "SELECT id, name FROM abilities where isactive = 1 and passive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                abilityPassOptions = new string[rows.Count + 1];
                abilityPassOptions[optionsId] = "~ none ~";
                abilityPassIds = new int[rows.Count + 1];
                abilityPassIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    abilityPassOptions[optionsId] = data["id"] + ":" + data["name"];
                    abilityPassIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        public void LoadAchievementOptions()
        {
            string query = "SELECT id, name FROM achievement_settings where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                itemAchievementOptions = new string[rows.Count ];
               // itemAchievementOptions[optionsId] = "~ none ~";
                itemAchievementIds = new int[rows.Count];
              //  itemAchievementIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                  
                    itemAchievementOptions[optionsId] = data["id"] + ":" + data["name"];
                    itemAchievementIds[optionsId] = int.Parse(data["id"]);
                    optionsId++;
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

        public void LoadBonusSettings()
        {
            // Read all entries from the table
            string query = "SELECT * FROM bonuses_settings where isactive = 1 order by id ";
            //  Debug.LogError(query);
            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();
       //     optionslist.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data

            if ((rows != null) && (rows.Count > 0))
            {
                int optionsId = 0;
                bonusOptions = new string[rows.Count + 1];
                bonusOptionsParam = new string[rows.Count + 1];
                bonusOptionsCode = new string[rows.Count + 1];
                bonusOptions[optionsId] = "~ none ~";
                bonusOptionsParam[optionsId] = "";
                bonusOptionsCode[optionsId] = "";
                bonusIds = new int[rows.Count + 1];
                bonusIds[optionsId] = -1;
              
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    bonusOptions[optionsId] = data["name"];
                    bonusIds[optionsId] = int.Parse(data["id"]);
                    bonusOptionsParam[optionsId] =data["param"];
                    bonusOptionsCode[optionsId] = data["code"];
                    //    Debug.LogError(int.Parse(data["id"])+", "+data["name"]+", "+float.Parse(data["cost"])+", "+float.Parse(data["chance"]));
                    //     optionslist.Add(new BonusOptionData(int.Parse(data["id"]), data["name"], data["code"], data["param"]));
                }
            }
        }

        private void LoadRecipeList()
        {
            string query = "SELECT id, name FROM crafting_recipes where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                recipeList = new string[rows.Count + 1];
                recipeList[optionsId] = "~ none ~";
                recipeIds = new int[rows.Count + 1];
                recipeIds[optionsId] = 0;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    recipeList[optionsId] = data["id"] + ":" + data["name"];
                    recipeIds[optionsId] = int.Parse(data["id"]);
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

        public void LoadBuildObjectOptions()
        {
            string query = "SELECT id, name FROM build_object_template where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                buildObjectOptions = new string[rows.Count + 1];
                buildObjectOptions[optionsId] = "~ none ~";
                buildObjectIds = new int[rows.Count + 1];
                buildObjectIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    buildObjectOptions[optionsId] = data["id"] + ":" + data["name"];
                    buildObjectIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        public void LoadQuestOptions()
        {
            string query = "SELECT id, name FROM quests where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
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

        public void LoadEnchantProfileOptions()
        {
            string query = "SELECT DISTINCT id, name FROM item_enchant_profile ";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                enchantProfileOptions = new string[rows.Count + 1];
                enchantProfileOptions[optionsId] = "~ none ~";
                enchantProfileIds = new int[rows.Count + 1];
                enchantProfileIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    enchantProfileOptions[optionsId] = data["id"] + ":" + data["name"];
                    enchantProfileIds[optionsId] = int.Parse(data["id"]);
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

        ItemData LoadEntity(int id)
        {

            // Read all entries from the table
            string query = "SELECT " + originalDisplay.GetFieldsString() + " FROM " + tableName + " where isactive = 1 AND id =" + id;

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            ItemData display = new ItemData();

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
                    display.category = data["category"];
                    display.subcategory = data["subcategory"];
                    display.itemType = data["itemType"];
                    display.skillExp = int.Parse(data["skillExp"]);
                    display.subType = data["subType"];
                    display.slot = data["slot"];
                    display.display = data["display"];
                    display.itemQuality = int.Parse(data["itemQuality"]);
                    display.binding = int.Parse(data["binding"]);
                    display.isUnique = bool.Parse(data["isUnique"]);
                    display.auctionHouse = bool.Parse(data["auctionHouse"]);
                    display.stackLimit = int.Parse(data["stackLimit"]);
                    display.purchaseCurrency = int.Parse(data["purchaseCurrency"]);
                    display.purchaseCost = long.Parse(data["purchaseCost"]);
                    display.sellable = bool.Parse(data["sellable"]);
                    display.damage = int.Parse(data["damage"]);
                    display.damageMax = int.Parse(data["damageMax"]);
                    display.damageType = data["damageType"];
                    display.delay = float.Parse(data["delay"]);
                    display.toolTip = data["toolTip"];
                    display.enchant_profile_id = int.Parse(data["enchant_profile_id"]);
                    display.gear_score = int.Parse(data["gear_score"]);
                    display.passive_ability = int.Parse(data["passive_ability"]);

                    display.weight = int.Parse(data["weight"]);
                    display.durability = int.Parse(data["durability"]);
                    display.autoattack = int.Parse(data["autoattack"]);
                    display.socketType = data["socket_type"];
                    display.ammotype = int.Parse(data["ammotype"]);
                    display.deathLossChance = int.Parse(data["death_loss"]);
                    display.parry = bool.Parse(data["parry"]);
                    display.oadelete = bool.Parse(data["oadelete"]);

                    for (int i = 1; i <= display.maxEffectEntries; i++)
                    {
                        string effectType = data["effect" + i + "type"];
                        if (effectType != null && effectType != "")
                        {
                            string effectName = data["effect" + i + "name"];
                            string effectValue = data["effect" + i + "value"];
                            ItemEffectEntry entry = new ItemEffectEntry(effectType, effectName, effectValue);
                            display.effects.Add(entry);
                        }
                    }

                    display.isLoaded = true;

                }
            }

            LoadItemTemplateOptions(display);
            LoadItemSetProfile(display);

            return display;
        }

        void LoadItemSetProfile(ItemData itemData)
        {
            // Read all entries from the table
            string query = "SELECT * FROM item_set_items where template_id = " + itemData.id;

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
                    itemData.setId = int.Parse(data["set_id"]);
                }
            }
        }

        void LoadItemTemplateOptions(ItemData itemData)
        {
            // Read all entries from the table
            string query = "SELECT " + new ItemTemplateOptionEntry().GetFieldsString() + " FROM item_templates_options where item_id = "
                + itemData.id + " AND isactive = 1";

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
                    ItemTemplateOptionEntry display = new ItemTemplateOptionEntry();
                    display.id = int.Parse(data["id"]);
                    display.editor_option_type_id = int.Parse(data["editor_option_type_id"]);
                    display.editor_option_choice_type_id = data["editor_option_choice_type_id"];
                    display.required_value = int.Parse(data["required_value"]);
                    itemData.itemTemplateOptions.Add(display);
                }
            }
        }

        public void LoadMobOptions()
        {
            string query = "SELECT id, name FROM mob_templates where isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            //   Debug.LogError("Rows " + rows.Count);
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

   /*     public void LoadSelectList()
        {
            //string[] selectList = new string[dataRegister.Count];
         //   displayList = new string[dataRegister.Count];
            int i = 0;
            foreach (int displayID in dataRegister.Keys)
            {
                //selectList [i] = displayID + ". " + dataRegister [displayID].name;
                displayList[i] = displayID + ". " + dataRegister[displayID].name;
                i++;
            }
            //displayList = new Combobox(selectList);
        }
        */

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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create an Item before edit it."));
                return;
            }


            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Item Template"));

            //****
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

                pos.y += ImagePack.fieldHeight;
                //Build prefabs button
             /*   if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Generate Prefabs")))
                {
                    GenerateAllPrefabs();
                }*/
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Duplicate")))
                {
                    Duplicate();
                }
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight * 1f;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Save All Items")))
                {
                    SaveAll();
                }

                pos.width *= 2;

                pos.y += ImagePack.fieldHeight * 1.5f;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Template Properties")+":");
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

            pos.width -= 140+ 155;
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

                pos.x -= pos.width+ 155;
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
            editingDisplay = new ItemData();
            originalDisplay = new ItemData();
            selectedDisplay = -1;
        }

        // Edit or Create
        public override void DrawEditor(Rect box, bool nItem)
        {
            newEntity = nItem;
            // Setup the layout
            Rect pos = box;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;

            if (!linkedTablesLoaded)
            {
                LoadBonusSettings();
                LoadAbilityOptions();
                LoadCurrencyOptions();
                LoadAchievementOptions();
                LoadRecipeList();
                LoadDamageOptions();
                LoadStatOptions();
                LoadBuildObjectOptions();
                LoadQuestOptions();
                LoadSkillOptions();
                LoadFactionOptions();
                LoadMobOptions();
                LoadEnchantProfileOptions();
                LoadPassiveAbilityOptions();
                //LoadRequirementOptions();
                itemSocketTypeOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Sockets Type", true);
                itemTypeOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Item Type", false);
                weaponTypeOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Weapon Type", false);
                armorTypeOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Armor Type", false);
                itemEffectTypeOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Item Effect Type", true);
                accountEffectTypeOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Account Effect Type", true);
                ServerOptionChoices.LoadAtavismChoiceOptions("Requirement", false, out requirementIds, out requirementOptions);
                ServerOptionChoices.LoadAtavismChoiceOptions("Race", false, out raceIds, out raceOptions);
                ServerOptionChoices.LoadAtavismChoiceOptions("Class", false, out classIds, out classOptions);
                ServerOptionChoices.LoadAtavismChoiceOptions("Ammo Type", true, out ammoIds, out ammoOptions);
                stationOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Crafting Station", true);
                linkedTablesLoaded = true;
            }
            if (editingDisplay == null)
                return;
            // Draw the content database info
            if (newEntity)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new item"));
                pos.y += ImagePack.fieldHeight;

                if (editingDisplay.itemType == "")
                {
                    foreach (string itemTypeOption in itemTypeOptions)
                    {
                        if (ImagePack.DrawButton(pos.x, pos.y, itemTypeOption))
                        {
                            editingDisplay.itemType = itemTypeOption;
                        }
                        pos.y += ImagePack.fieldHeight;
                    }
                    return;
                }
            }

            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.8f);
            pos.y += ImagePack.fieldHeight;
            pos.width /= 2;
            if (editingDisplay.itemType != "Weapon" && editingDisplay.itemType != "Armor")
            {
                string[] options = new string[itemTypeOptions.Length - 2];
                int optionPos = 0;
                foreach (string option in itemTypeOptions)
                {
                    if (option != "Weapon" && option != "Armor")
                    {
                        options[optionPos++] = option;
                    }
                }
                editingDisplay.itemType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Item Type")+":", editingDisplay.itemType, options);
              
            }
            else
            {
                ImagePack.DrawText(pos, Lang.GetTranslate("Item Type")+": " + editingDisplay.itemType);
                if (editingDisplay.itemType == "Weapon")
                {
                    pos.y += ImagePack.fieldHeight;
                    editingDisplay.skillExp = ImagePack.DrawField(pos, Lang.GetTranslate("Skill Exp") + ":", editingDisplay.skillExp);
                }
            }
            pos.x += pos.width;
            string icon =    ImagePack.DrawSpriteAsset(pos, Lang.GetTranslate("Icon")+":", editingDisplay.icon);
        /*    if (editingDisplay.icon.Length > 3 && icon == " ")
            {
                item = new ItemPrefab(editingDisplay);
                editingDisplay.icon = item.LoadIcon();
            }
            else
            {*/
                editingDisplay.icon = icon;
       //     }
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            if (editingDisplay.itemType == "Weapon")
                editingDisplay.subType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Sub-Type")+":", editingDisplay.subType, weaponTypeOptions);
            else if (editingDisplay.itemType == "Armor")
                editingDisplay.subType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Sub-Type")+":", editingDisplay.subType, armorTypeOptions);
            pos.width *= 2;
            pos.y += 1.3f * ImagePack.fieldHeight;
            if ((editingDisplay.itemType == "Weapon") || (editingDisplay.itemType == "Armor"))
            {
                pos.width *= 0.7f;
                editingDisplay.display = ImagePack.DrawGameObject(pos, Lang.GetTranslate("Equipment Display")+":", editingDisplay.display, 0.6f);
                pos.width /= 0.7f;
            }
            pos.width /= 2;
            pos.y += 1.5f * ImagePack.fieldHeight;
            /*editingDisplay.category = ImagePack.DrawField (pos, "Category:", editingDisplay.category);
            pos.x += pos.width;
            editingDisplay.subcategory = ImagePack.DrawField (pos, "Sub-category:", editingDisplay.subcategory);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;*/
            if ((editingDisplay.itemType == "Weapon") || (editingDisplay.itemType == "Armor"))
            {
                if (editingDisplay.itemType == "Weapon")
                    editingDisplay.slot = ImagePack.DrawSelector(pos, Lang.GetTranslate("Slot")+":", editingDisplay.slot, editingDisplay.slotWeaponOptions);
                else
                    editingDisplay.slot = ImagePack.DrawSelector(pos, Lang.GetTranslate("Slot")+":", editingDisplay.slot, editingDisplay.slotArmorOptions);

                pos.x += pos.width;
                editingDisplay.gear_score = ImagePack.DrawField(pos, Lang.GetTranslate("Gear Score") + ":", editingDisplay.gear_score);
                pos.x -= pos.width;

                if (editingDisplay.itemType == "Weapon")
                {
                    pos.y += ImagePack.fieldHeight;
                    //	editingDisplay.damageType = ImagePack.DrawSelector (pos, "Damage Type:", editingDisplay.damageType, damageOptions); 
                    //	pos.x += pos.width;
                    editingDisplay.damage = ImagePack.DrawField(pos, Lang.GetTranslate("Damage Min")+":", editingDisplay.damage);
                    pos.x += pos.width;
                    editingDisplay.damageMax = ImagePack.DrawField(pos, Lang.GetTranslate("Damage Max")+":", editingDisplay.damageMax);
                    if (editingDisplay.damage > editingDisplay.damageMax) editingDisplay.damageMax = editingDisplay.damage;
                    pos.x -= pos.width;
                    pos.y += ImagePack.fieldHeight;
                    editingDisplay.delay = ImagePack.DrawField(pos, Lang.GetTranslate("Delay")+":", editingDisplay.delay);
                    //pos.x -= pos.width;

                }
                pos.y += ImagePack.fieldHeight;
                int selectedEnchantProfile = GetOptionPosition(editingDisplay.enchant_profile_id, enchantProfileIds);
                //selectedCurrency = ImagePack.DrawSelector (pos, "Currency:", selectedCurrency, currencyOptions);
                selectedEnchantProfile = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Enchant Profile")+": ", ref enchantSearchInput, selectedEnchantProfile, enchantProfileOptions);
                editingDisplay.enchant_profile_id = enchantProfileIds[selectedEnchantProfile];
                pos.y += ImagePack.fieldHeight;
            }
            else if (editingDisplay.itemType == "Ammo")
            {
                int ammoID = 0;
                int.TryParse(editingDisplay.slot, out ammoID);
                int selectedAmmo = GetOptionPosition(ammoID, ammoIds);
                selectedAmmo = ImagePack.DrawSelector(pos, Lang.GetTranslate("Ammo Type")+":", selectedAmmo, ammoOptions);
                editingDisplay.slot = ammoIds[selectedAmmo].ToString();
                pos.y += ImagePack.fieldHeight;
                editingDisplay.damage = ImagePack.DrawField(pos, Lang.GetTranslate("Damage")+":", editingDisplay.damage);
                pos.x += pos.width;
                //    editingDisplay.damageMax = ImagePack.DrawField(pos, "Damage Max:", editingDisplay.damageMax);
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;

            }
            if (!qualityloaded)
            {
                itemQualityOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Item Quality", false);
                qualityloaded = true;
            }

            editingDisplay.itemQuality = ImagePack.DrawSelector(pos, Lang.GetTranslate("Item Quality")+":", editingDisplay.itemQuality - 1, itemQualityOptions) + 1;

            pos.x += pos.width;
            editingDisplay.binding = ImagePack.DrawSelector(pos, Lang.GetTranslate("Binding")+":", editingDisplay.binding, editingDisplay.bindingOptions);
            pos.x -= pos.width;
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            int selectedCurrency = GetOptionPosition(editingDisplay.purchaseCurrency, currencyIds);
            //selectedCurrency = ImagePack.DrawSelector (pos, "Currency:", selectedCurrency, currencyOptions);
            selectedCurrency = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Currency")+": ", ref currencySearchInput, selectedCurrency, currencyOptions);
            editingDisplay.purchaseCurrency = currencyIds[selectedCurrency];
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.purchaseCost = ImagePack.DrawField(pos, Lang.GetTranslate("Purchase Cost") + ":", editingDisplay.purchaseCost);
            //pos.x += pos.width;
            //editingDisplay.passive_ability = ImagePack.DrawField(pos, Lang.GetTranslate("Passive Ability") + ":", editingDisplay.passive_ability);
            pos.y += ImagePack.fieldHeight;
            int selectedPAbility = GetOptionPosition(editingDisplay.passive_ability, abilityPassIds);
            //selectedAbility = ImagePack.DrawSelector (pos, "Ability:", selectedAbility, abilityOptions);
            selectedPAbility = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Passive Ability") + ": ", ref passiveAbilijtySerach, selectedPAbility, abilityPassOptions);
            editingDisplay.passive_ability = abilityPassIds[selectedPAbility];
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.sellable = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Is Item Sellable")+"?", editingDisplay.sellable);
            pos.x += pos.width;
            editingDisplay.isUnique = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Is Item Unique")+"?", editingDisplay.isUnique);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.auctionHouse = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Auction Sellable")+"?", editingDisplay.auctionHouse);

            if ((editingDisplay.itemType == "Weapon") || (editingDisplay.itemType == "Armor"))
            {
                pos.x += pos.width;
                editingDisplay.parry = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Parry") + "?", editingDisplay.parry);
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                editingDisplay.durability = ImagePack.DrawField(pos, Lang.GetTranslate("Durability"), editingDisplay.durability);
            }

                pos.x += pos.width;
                //pos.y += ImagePack.fieldHeight;
                editingDisplay.socketType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Socket Type") + ":", editingDisplay.socketType, itemSocketTypeOptions);
                pos.x -= pos.width;
            
            
            pos.y += ImagePack.fieldHeight;
            editingDisplay.deathLossChance = ImagePack.DrawField(pos, Lang.GetTranslate("Death Loss Chance "), editingDisplay.deathLossChance);
            pos.x += pos.width;
             editingDisplay.weight = ImagePack.DrawField(pos, Lang.GetTranslate("Weight"), editingDisplay.weight);
              pos.x -= pos.width; 
          
            if (editingDisplay.itemType == "Weapon")
            {
                pos.y += ImagePack.fieldHeight;

                int selectedAbility = GetOptionPosition(editingDisplay.autoattack, abilityIds);
                //selectedAbility = ImagePack.DrawSelector (pos, "Ability:", selectedAbility, abilityOptions);
                selectedAbility = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Auto Attack") + ": ", ref searcheaa, selectedAbility, abilityOptions);
                editingDisplay.autoattack = abilityIds[selectedAbility];
                pos.y += ImagePack.fieldHeight;

                int selectedAmmo = GetOptionPosition(editingDisplay.ammotype, ammoIds);
                selectedAmmo = ImagePack.DrawSelector(pos, Lang.GetTranslate("Ammo Type") + ":", selectedAmmo, ammoOptions);
                editingDisplay.ammotype = ammoIds[selectedAmmo];
               
            }

            pos.y += ImagePack.fieldHeight;
            if (editingDisplay.itemType == "Bag" || editingDisplay.itemType == "Container")
            {
                editingDisplay.stackLimit = ImagePack.DrawField(pos, Lang.GetTranslate("Number of Slots")+":", editingDisplay.stackLimit);
            }
            else
            {
                editingDisplay.stackLimit = ImagePack.DrawField(pos, Lang.GetTranslate("Stack Limit")+":", editingDisplay.stackLimit);
            }

            pos.x += pos.width; 
            if ((editingDisplay.itemType != "Weapon") && (editingDisplay.itemType != "Armor"))
                editingDisplay.oadelete = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Delete item on Activate") + "?", editingDisplay.oadelete);
            pos.x -= pos.width;

            pos.y += ImagePack.fieldHeight;
            pos.width *= 2;
            editingDisplay.toolTip = ImagePack.DrawField(pos, Lang.GetTranslate("Tool Tip")+":", editingDisplay.toolTip, 0.75f, 60);

            pos.y += 2.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Item Effects"));
            pos.y += ImagePack.fieldHeight;

            for (int i = 0; i < editingDisplay.maxEffectEntries; i++)
            {
                if (editingDisplay.effects.Count > i)
                {
                    pos.width = pos.width / 2;
                    editingDisplay.effects[i].itemEffectType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Effect")+" " + (i + 1)+ " "+Lang.GetTranslate("Type")+":", editingDisplay.effects[i].itemEffectType, itemEffectTypeOptions);

                    // Generate search string if none exists
                    if (effectSearchInput.Count <= i)
                    {
                        effectSearchInput.Add("");
                    }
                    string searchString = effectSearchInput[i];

                    if (editingDisplay.effects[i].itemEffectType == "Stat")
                    {
                        //pos.x -= pos.width;
                        pos.y += ImagePack.fieldHeight;
                        //editingDisplay.effects[i].itemEffectName = ImagePack.DrawSelector (pos, "Stat:", editingDisplay.effects[i].itemEffectName, statOptions);
                        editingDisplay.effects[i].itemEffectName = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Stat")+": ", ref searchString, editingDisplay.effects[i].itemEffectName, statOptions);
                        //pos.x += pos.width;
                        pos.y += ImagePack.fieldHeight;
                        if (editingDisplay.effects[i].itemEffectValue == "")
                            editingDisplay.effects[i].itemEffectValue = "0";
                        editingDisplay.effects[i].itemEffectValue = ImagePack.DrawField(pos, Lang.GetTranslate("Value")+":", editingDisplay.effects[i].itemEffectValue);
                    }
                    else if (editingDisplay.effects[i].itemEffectType == "UseAbility")
                    {
                        if (editingDisplay.effects[i].itemEffectValue == "")
                            editingDisplay.effects[i].itemEffectValue = "0";
                        //pos.x -= pos.width;
                        pos.y += ImagePack.fieldHeight;
                        int selectedAbility = GetOptionPosition(int.Parse(editingDisplay.effects[i].itemEffectValue), abilityIds);
                        //selectedAbility = ImagePack.DrawSelector (pos, "Ability:", selectedAbility, abilityOptions);
                        selectedAbility = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Ability") + ": ", ref searchString, selectedAbility, abilityOptions);
                        editingDisplay.effects[i].itemEffectValue = abilityIds[selectedAbility].ToString();
                    }
                  /*  else if (editingDisplay.effects[i].itemEffectType == "PassiveAbility")
                    {
                        if (editingDisplay.effects[i].itemEffectValue == "")
                            editingDisplay.effects[i].itemEffectValue = "0";
                        //pos.x -= pos.width;
                        pos.y += ImagePack.fieldHeight;
                        int selectedAbility = GetOptionPosition(int.Parse(editingDisplay.effects[i].itemEffectValue), abilityIds);
                        //selectedAbility = ImagePack.DrawSelector (pos, "Ability:", selectedAbility, abilityOptions);
                        selectedAbility = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Ability") + ": ", ref searchString, selectedAbility, abilityOptions);
                        editingDisplay.effects[i].itemEffectValue = abilityIds[selectedAbility].ToString();
                    }*/
                    /* else if (editingDisplay.effects[i].itemEffectType == "AutoAttack")
                     {
                         if (editingDisplay.effects[i].itemEffectValue == "")
                             editingDisplay.effects[i].itemEffectValue = "0";
                         pos.y += ImagePack.fieldHeight;
                         int selectedAbility = GetOptionPosition(int.Parse(editingDisplay.effects[i].itemEffectValue), abilityIds);
                         //selectedAbility = ImagePack.DrawSelector (pos, "Ability:", selectedAbility, abilityOptions);
                         selectedAbility = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Ability")+": ", ref searchString, selectedAbility, abilityOptions);
                         editingDisplay.effects[i].itemEffectValue = abilityIds[selectedAbility].ToString();
                     }*/
                    else if (editingDisplay.effects[i].itemEffectType == "Currency")
                    {
                        if (editingDisplay.effects[i].itemEffectValue == "")
                            editingDisplay.effects[i].itemEffectValue = "0";
                        pos.y += ImagePack.fieldHeight;
                        selectedCurrency = GetOptionPosition(int.Parse(editingDisplay.effects[i].itemEffectValue), currencyIds);
                        //selectedCurrency = ImagePack.DrawSelector (pos, "Currency:", selectedCurrency, currencyOptions);
                        selectedCurrency = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Currency")+": ", ref searchString, selectedCurrency, currencyOptions);
                        editingDisplay.effects[i].itemEffectValue = currencyIds[selectedCurrency].ToString();
                    }
                    else if (editingDisplay.effects[i].itemEffectType == "ClaimObject")
                    {
                        if (editingDisplay.effects[i].itemEffectValue == "")
                            editingDisplay.effects[i].itemEffectValue = "0";
                        pos.y += ImagePack.fieldHeight;

                        int selectedBuildObject = GetOptionPosition(int.Parse(editingDisplay.effects[i].itemEffectValue), buildObjectIds);
                        //selectedBuildObject = ImagePack.DrawSelector (pos, "Build Object:", selectedBuildObject, buildObjectOptions);
                        selectedBuildObject = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Build Object")+": ", ref searchString, selectedBuildObject, buildObjectOptions);
                        editingDisplay.effects[i].itemEffectValue = "" + buildObjectIds[selectedBuildObject];
                    }
                    else if (editingDisplay.effects[i].itemEffectType == "CreateClaim")
                    {
                        //editingDisplay.effects[i].itemEffectName = "";
                        pos.x += pos.width;
                        editingDisplay.effects[i].itemEffectValue = ImagePack.DrawField(pos, Lang.GetTranslate("Size")+":", editingDisplay.effects[i].itemEffectValue);

                        if (editingDisplay.effects[i].itemEffectValue == "" || editingDisplay.effects[i].itemEffectValue == " ")
                        {
                            editingDisplay.effects[i].itemEffectValue = "5";
                        }
                        if (int.Parse(editingDisplay.effects[i].itemEffectValue) < 5)
                        {
                            editingDisplay.effects[i].itemEffectValue = "5";
                        }
                        pos.x -= pos.width;
                        pos.y += ImagePack.fieldHeight;
                        GUI.Label(pos, Lang.GetTranslate("Valid Claim Types")+":", ImagePack.FieldStyle());
                        Rect tempPosition = new Rect(pos.x + pos.width / 2, pos.y, pos.width / 2 - ImagePack.fieldMargin, pos.height - ImagePack.lineSpace);
                        if (String.IsNullOrEmpty(editingDisplay.effects[i].itemEffectName))
                            editingDisplay.effects[i].itemEffectName = "" + 1;
                   /*     ClaimType currentClaimType = (ClaimType)int.Parse(editingDisplay.effects[i].itemEffectName);
                        currentClaimType = (ClaimType)EditorGUI.EnumPopup(tempPosition, currentClaimType, ImagePack.SelectorStyle());
                        editingDisplay.effects[i].itemEffectName = ((int)currentClaimType).ToString();*/
                    }
                    else if (editingDisplay.effects[i].itemEffectType == "StartQuest")
                    {
                        if (editingDisplay.effects[i].itemEffectValue == "")
                            editingDisplay.effects[i].itemEffectValue = "0";
                        pos.y += ImagePack.fieldHeight;
                        int selectedQuest = GetOptionPosition(int.Parse(editingDisplay.effects[i].itemEffectValue), questIds);
                        //selectedQuest = ImagePack.DrawSelector (pos, "Quest:", selectedQuest, questOptions);
                        selectedQuest = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Quest")+": ", ref searchString, selectedQuest, questOptions);
                        editingDisplay.effects[i].itemEffectValue = questIds[selectedQuest].ToString();
                    }
                  /*  else if (editingDisplay.effects[i].itemEffectType == "UseAmmo")
                    {
                        pos.y += ImagePack.fieldHeight;
                        int ammoID = 0;
                        int.TryParse(editingDisplay.effects[i].itemEffectName, out ammoID);
                        int selectedAmmo = GetOptionPosition(ammoID, ammoIds);
                        selectedAmmo = ImagePack.DrawSelector(pos, Lang.GetTranslate("Ammo Type")+":", selectedAmmo, ammoOptions);
                        editingDisplay.effects[i].itemEffectName = ammoIds[selectedAmmo].ToString();
                    }*/
                    else if (editingDisplay.effects[i].itemEffectType == "CraftsItem")
                    {
                        if (editingDisplay.effects[i].itemEffectValue == "")
                            editingDisplay.effects[i].itemEffectValue = "0";
                        pos.y += ImagePack.fieldHeight;
                        int craftedItem = GetOptionPosition(int.Parse(editingDisplay.effects[i].itemEffectValue), recipeIds);
                        craftedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Crafts Recipe")+": ", ref searchString, craftedItem, recipeList);
                        editingDisplay.effects[i].itemEffectValue = "" + recipeIds[craftedItem];
                    }
                    else if (editingDisplay.effects[i].itemEffectType == "AccountEffect")
                    {
                        if (editingDisplay.effects[i].itemEffectValue == "")
                            editingDisplay.effects[i].itemEffectValue = "0";
                        pos.y += ImagePack.fieldHeight;
                        editingDisplay.effects[i].itemEffectName = ImagePack.DrawSelector(pos, Lang.GetTranslate("Effect")+" " + (i + 1) + " "+ Lang.GetTranslate("Type")+":", editingDisplay.effects[i].itemEffectName, accountEffectTypeOptions);
                        pos.y += ImagePack.fieldHeight;
                        editingDisplay.effects[i].itemEffectValue = ImagePack.DrawField(pos, Lang.GetTranslate("Value")+":", editingDisplay.effects[i].itemEffectValue);
                    }
                    else if (editingDisplay.effects[i].itemEffectType == "SpawnMob")
                    {
                        if (editingDisplay.effects[i].itemEffectValue == "")
                            editingDisplay.effects[i].itemEffectValue = "0";
                        pos.y += ImagePack.fieldHeight;
                        int craftedItem = GetOptionPosition(int.Parse(editingDisplay.effects[i].itemEffectValue), mobIds);
                        craftedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Mob Template")+": ", ref searchString, craftedItem, mobList);
                        editingDisplay.effects[i].itemEffectValue = "" + mobIds[craftedItem];
                    }
                    else if (editingDisplay.effects[i].itemEffectType == "Sockets")
                    {
                        pos.y += ImagePack.fieldHeight;
                        editingDisplay.effects[i].itemEffectName = ImagePack.DrawSelector(pos, Lang.GetTranslate("Sockets Type")+":", editingDisplay.effects[i].itemEffectName, itemSocketTypeOptions);

                        if (editingDisplay.effects[i].itemEffectValue == "")
                            editingDisplay.effects[i].itemEffectValue = "1";
                        pos.x += pos.width;
                        editingDisplay.effects[i].itemEffectValue = ImagePack.DrawField(pos, Lang.GetTranslate("Number of sockets")+":", editingDisplay.effects[i].itemEffectValue);
                        pos.x -= pos.width;
                    }
                  /*  else if (editingDisplay.effects[i].itemEffectType == "SocketsEffect")
                    {
                        pos.y += ImagePack.fieldHeight;
                        editingDisplay.effects[i].itemEffectName = ImagePack.DrawSelector(pos, Lang.GetTranslate("Socket Type")+":", editingDisplay.effects[i].itemEffectName, itemSocketTypeOptions);

                        if (editingDisplay.effects[i].itemEffectValue == "")
                            editingDisplay.effects[i].itemEffectValue = "1";
                        pos.x += pos.width;
                        //    editingDisplay.effects[i].itemEffectValue = ImagePack.DrawField(pos, "Numer:", editingDisplay.effects[i].itemEffectValue);
                        pos.x -= pos.width;
                    }*/
                    else if (editingDisplay.effects[i].itemEffectType == "Blueprint")
                    {
                        if (editingDisplay.effects[i].itemEffectValue == "")
                            editingDisplay.effects[i].itemEffectValue = "0";
                        pos.y += ImagePack.fieldHeight;
                        int craftedItem = GetOptionPosition(int.Parse(editingDisplay.effects[i].itemEffectValue), recipeIds);
                        craftedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Crafts Recipe")+": ", ref searchString, craftedItem, recipeList);
                        editingDisplay.effects[i].itemEffectValue = "" + recipeIds[craftedItem];
                    }
                 /*   else if (editingDisplay.effects[i].itemEffectType == "Parry")
                    {
                      //  pos.y += ImagePack.fieldHeight;
                        if (editingDisplay.itemType == "Weapon")
                        {
                           // if (editingDisplay.effects[i].itemEffectValue == "")
                                editingDisplay.effects[i].itemEffectValue = editingDisplay.subType;
                         //   editingDisplay.effects[i].itemEffectValue = ImagePack.DrawSelector(pos, Lang.GetTranslate("Weapon-Type") + ":", editingDisplay.effects[i].itemEffectValue, weaponTypeOptions);
                        }
                        else
                        {
                            editingDisplay.effects[i].itemEffectValue = "";
                            Color temp = GUI.color;
                            GUI.color = Color.red;
                            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Parry Effects can be only on Weapon item !!"));
                            GUI.color = temp;
                        }
                    }*/
                    else if (editingDisplay.effects[i].itemEffectType == "Bonus")
                    {
                        pos.y += ImagePack.fieldHeight;
                        if (editingDisplay.itemType == "Weapon" || editingDisplay.itemType == "Armor")
                        {
                            int selectedBonus = 0;
                            try
                            {
                                selectedBonus = GetOptionPosition(editingDisplay.effects[i].itemEffectName, bonusOptionsCode);
                            }
                            catch (Exception)
                            {
                            }
                            selectedBonus = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Option Name") + ": ", ref searchString, selectedBonus, bonusOptions);
                            pos.y += ImagePack.fieldHeight;
                            editingDisplay.effects[i].itemEffectName = bonusOptionsCode[selectedBonus].ToString();
                            if (!editingDisplay.effects[i].itemEffectValue.Contains("|"))
                                editingDisplay.effects[i].itemEffectValue = "0|" + 0f;
                            string[] sa = editingDisplay.effects[i].itemEffectValue.Split('|');
                            int value = int.Parse(sa[0]);
                            float valuep = float.Parse(sa[1]);
                            if (bonusOptionsParam[selectedBonus].Contains("v"))
                            {

                                value = ImagePack.DrawField(pos, Lang.GetTranslate("Value") + ":", value);
                                pos.x += pos.width;
                            }
                            if (bonusOptionsParam[selectedBonus].Contains("p"))
                                valuep = ImagePack.DrawField(pos, Lang.GetTranslate("Percentage") + ":", valuep);
                            editingDisplay.effects[i].itemEffectValue = value + "|" + valuep+"|"+ bonusOptions[selectedBonus];
                            if (bonusOptionsParam[selectedBonus].Contains("v"))
                                pos.x -= pos.width;
                        }
                        else
                        {
                            Color temp = GUI.color;
                            GUI.color = Color.red;
                            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Bonus Effects can be only on Weapon or Armor !!"));
                            GUI.color = temp;
                        }
                    }
                    else if (editingDisplay.effects[i].itemEffectType == "SkillReset")
                    {

                    }
                    else if (editingDisplay.effects[i].itemEffectType == "TalentReset")
                    {

                    }
                    else if (editingDisplay.effects[i].itemEffectType == "Achievement")
                    {
                        if (editingDisplay.effects[i].itemEffectValue == "")
                            editingDisplay.effects[i].itemEffectValue = "1";
                        pos.y += ImagePack.fieldHeight;
                        int selected = GetOptionPosition(int.Parse(editingDisplay.effects[i].itemEffectValue), itemAchievementIds);
                        //selectedCurrency = ImagePack.DrawSelector (pos, "Currency:", selectedCurrency, currencyOptions);
                        selected = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Achievement") + ": ", ref searcheAchievement , selected, itemAchievementOptions);
                        editingDisplay.effects[i].itemEffectValue = itemAchievementIds[selected]+"";
                    }
                    else
                    {
                        pos.x += pos.width;
                        editingDisplay.effects[i].itemEffectValue = ImagePack.DrawField(pos, Lang.GetTranslate("Value")+":", editingDisplay.effects[i].itemEffectValue);
                        pos.x -= pos.width;
                    }


                    // Save back the search string
                    effectSearchInput[i] = searchString;

                    pos.x += pos.width;
                    pos.y += ImagePack.fieldHeight;
                    if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove Effect")))
                    {
                        editingDisplay.effects.RemoveAt(i);
                    }
                    pos.x -= pos.width;
                    //pos.x -= pos.width;
                    pos.y += ImagePack.fieldHeight;
                    pos.width = pos.width * 2;
                }
            }
            if (editingDisplay.effects.Count < editingDisplay.maxEffectEntries)
            {
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Item Effect")))
                {
                    ItemEffectEntry itemEffectEntry = new ItemEffectEntry("", "", "");
                    editingDisplay.effects.Add(itemEffectEntry);
                }
            }

            // Requirements area
            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Requirements"));
            pos.y += 1.5f * ImagePack.fieldHeight;
            pos.width /= 2;
            for (int i = 0; i < editingDisplay.itemTemplateOptions.Count; i++)
            {
                int selectedRequirement = GetOptionPosition(editingDisplay.itemTemplateOptions[i].editor_option_type_id, requirementIds);
                selectedRequirement = ImagePack.DrawSelector(pos, Lang.GetTranslate("Type")+":", selectedRequirement, requirementOptions);
                editingDisplay.itemTemplateOptions[i].editor_option_type_id = requirementIds[selectedRequirement];
                pos.x += pos.width;
                if (requirementOptions[selectedRequirement] == "Race")
                {
                    int raceID = 0;
                    int.TryParse(editingDisplay.itemTemplateOptions[i].editor_option_choice_type_id, out raceID);
                    int selectedRace = GetOptionPosition(raceID, raceIds);
                    selectedRace = ImagePack.DrawSelector(pos, Lang.GetTranslate("Race")+":", selectedRace, raceOptions);
                    editingDisplay.itemTemplateOptions[i].editor_option_choice_type_id = raceIds[selectedRace].ToString();
                }
                else if (requirementOptions[selectedRequirement] == "Class")
                {
                    int classID = 0;
                    int.TryParse(editingDisplay.itemTemplateOptions[i].editor_option_choice_type_id, out classID);
                    int selectedClass = GetOptionPosition(classID, classIds);
                    selectedClass = ImagePack.DrawSelector(pos, Lang.GetTranslate("Class")+":", selectedClass, classOptions);
                    editingDisplay.itemTemplateOptions[i].editor_option_choice_type_id = classIds[selectedClass].ToString();
                }
                else if (requirementOptions[selectedRequirement] == "Level")
                {
                    editingDisplay.itemTemplateOptions[i].required_value = ImagePack.DrawField(pos, Lang.GetTranslate("Level")+":", editingDisplay.itemTemplateOptions[i].required_value);
                }
                else if (requirementOptions[selectedRequirement] == "Skill Level")
                {
                    int skillID = 0;
                    int.TryParse(editingDisplay.itemTemplateOptions[i].editor_option_choice_type_id, out skillID);
                    int selectedSkill = GetOptionPosition(skillID, skillIds);
                    selectedSkill = ImagePack.DrawSelector(pos, Lang.GetTranslate("Skill")+":", selectedSkill, skillOptions);
                    editingDisplay.itemTemplateOptions[i].editor_option_choice_type_id = skillIds[selectedSkill].ToString();
                    pos.y += ImagePack.fieldHeight;
                    editingDisplay.itemTemplateOptions[i].required_value = ImagePack.DrawField(pos, Lang.GetTranslate("Level")+":", editingDisplay.itemTemplateOptions[i].required_value);
                }
                else if (requirementOptions[selectedRequirement] == "Stat")
                {
                    editingDisplay.itemTemplateOptions[i].editor_option_choice_type_id = ImagePack.DrawSelector(pos, Lang.GetTranslate("Stat")+":", editingDisplay.itemTemplateOptions[i].editor_option_choice_type_id, statOptions);
                    pos.y += ImagePack.fieldHeight;
                    editingDisplay.itemTemplateOptions[i].required_value = ImagePack.DrawField(pos, Lang.GetTranslate("Value")+":", editingDisplay.itemTemplateOptions[i].required_value);
                }
                else if (requirementOptions[selectedRequirement] == "Faction")
                {
                    int factionID = 0;
                    int.TryParse(editingDisplay.itemTemplateOptions[i].editor_option_choice_type_id, out factionID);
                    int selectedFaction = GetOptionPosition(factionID, factionIds);
                    selectedFaction = ImagePack.DrawSelector(pos, Lang.GetTranslate("Faction")+":", selectedFaction, factionOptions);
                    editingDisplay.itemTemplateOptions[i].editor_option_choice_type_id = factionIds[selectedFaction].ToString();
                    pos.y += ImagePack.fieldHeight;
                    //editingDisplay.itemTemplateOptions[i].required_value = ImagePack.DrawField (pos, "Reputation:", editingDisplay.itemTemplateOptions[i].required_value);
                    int selectedStance = FactionData.GetPositionOfStance(editingDisplay.itemTemplateOptions[i].required_value);
                    selectedStance = ImagePack.DrawSelector(pos, Lang.GetTranslate("Stance")+":", selectedStance, FactionData.stanceOptions);
                    editingDisplay.itemTemplateOptions[i].required_value = FactionData.stanceValues[selectedStance];
                }
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove Requirement")))
                {
                    if (editingDisplay.itemTemplateOptions[i].id > 0)
                        editingDisplay.itemOptionsToBeDeleted.Add(editingDisplay.itemTemplateOptions[i].id);
                    editingDisplay.itemTemplateOptions.RemoveAt(i);
                }
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
            }
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Requirement")))
            {
                editingDisplay.itemTemplateOptions.Add(new ItemTemplateOptionEntry(-1, -1, ""));
            }
            pos.width *= 2;

            // Save data		
            pos.x -= ImagePack.innerMargin;
            pos.y += 3f * ImagePack.fieldHeight;
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
                UpdateEntry(true);

            state = State.Loaded;
            linkedTablesLoaded = false;

            resetSearch(true);
        }

        public override void delete()
        {
            if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete")+" " + Lang.GetTranslate(functionName) + " "+ Lang.GetTranslate("entry")+" ?", Lang.GetTranslate("Are you sure you want to delete")+" " + editingDisplay.Name + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
            {
                DeleteEntry();
                newSelectedDisplay = 0;
                state = State.Loaded;
                linkedTablesLoaded = false;

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
                // Update online table to avoid access the database again			
                editingDisplay.id = itemID;
                // Insert the Requirements
                foreach (ItemTemplateOptionEntry entry in editingDisplay.itemTemplateOptions)
                {
                    if (entry.editor_option_type_id != -1)
                    {
                        entry.itemID = itemID;
                        InsertRequirement(entry);
                    }
                }
                editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
             //   dataRegister.Add(editingDisplay.id, editingDisplay);
             //   displayKeys.Add(editingDisplay.id);
                newItemCreated = true;

                // Configure the correponding prefab
                CreatePrefab();
                dataLoaded = false;
                Load();
                NewResult(Lang.GetTranslate("New entry inserted"));
            }
            else
            {
                NewResult(Lang.GetTranslate("Error occurred, please check the Console"));
            }
        }

        void InsertRequirement(ItemTemplateOptionEntry entry)
        {
            string query = "INSERT INTO item_templates_options";
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
        void UpdateEntry(bool prefab)
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
            foreach (ItemTemplateOptionEntry entry in editingDisplay.itemTemplateOptions)
            {
                if (entry.editor_option_type_id != -1)
                {
                    if (entry.id < 1)
                    {
                        // This is a new entry, insert it
                        entry.itemID = editingDisplay.id;
                        InsertRequirement(entry);
                    }
                    else
                    {
                        // This is an existing entry, update it
                        entry.itemID = editingDisplay.id;
                        UpdateRequirement(entry);
                    }
                }
            }

            // Delete any requirements that are tagged for deletion
            foreach (int requirementID in editingDisplay.itemOptionsToBeDeleted)
            {
                DeleteRequirement(requirementID);
            }

            // Update online table to avoid access the database again			
            //  dataRegister[displayKeys[selectedDisplay]] = editingDisplay;

            // Remove the prefab
            // DeletePrefab();
            // Configure the correponding prefab
            if (prefab)
            {
                CreatePrefab();
                dataLoaded = false;
            }
            NewResult(Lang.GetTranslate("Entry") + "  " + editingDisplay.Name + " " + Lang.GetTranslate("updated"));
          Load();
 }

        void UpdateRequirement(ItemTemplateOptionEntry entry)
        {
            string query = "UPDATE item_templates_options";
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
            string query = "UPDATE item_templates_options SET isactive = 0 where id = " + requirementID;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
        }

        // Delete entries from the table
        void DeleteEntry()
        {

            //Check instance_template
            string sql = "SELECT character_create_id FROM character_create_items WHERE isactive = 1 AND item_id=" + editingDisplay.id;
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sql);
            int charCount = 0;
            string charIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                charIds = "\n"+ Lang.GetTranslate("Character Template Ids")+":";
                charCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    charIds += " " + data["character_create_id"] + ",";
                }
            }

            //Check Merchant Table
            string sqlMerchantTable = "SELECT tableID FROM merchant_item WHERE itemID=" + editingDisplay.id +" AND isactive = 1";
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlMerchantTable);
            int merchantTableCount = 0;
            string merchantTableIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                merchantTableIds = "\n"+Lang.GetTranslate("Merchant Table Ids")+":";
                merchantTableCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    merchantTableIds += " " + data["tableID"] + ",";
                }
            }
            //Check Loot Table
            string sqlLootTable = "SELECT id FROM loot_tables WHERE ";
            for (int ii = 1; ii <= 10; ii++)
            {
                sqlLootTable += " item" + ii + " =" + editingDisplay.id;
                if (ii < 10) sqlLootTable += " OR ";
            }
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlLootTable);
            int lootTableCount = 0;
            string lootTableIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                lootTableIds = "\n"+ Lang.GetTranslate("Loot Table Ids")+":";
                lootTableCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    lootTableIds += " " + data["id"] + ",";
                }
            }

            //Check Quests
            string sqlquests = "SELECT quests.id as id FROM quests LEFT JOIN quest_objectives on quests.id = quest_objectives.questID WHERE (quest_objectives.objectiveType like 'item' AND quest_objectives.target=" + editingDisplay.id + ") ";
            for (int ii = 1; ii <= 3; ii++)
            {
                sqlquests += " OR deliveryItem" + ii + " =" + editingDisplay.id ;
            }
            for (int ii = 1; ii <= 4; ii++)
            {
                sqlquests += " OR item" + ii + " =" + editingDisplay.id;
            }
            for (int ii = 1; ii <= 4; ii++)
            {
                sqlquests += " OR chooseItem" + ii + " =" + editingDisplay.id;
            }
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlquests);
            int questCount = 0;
            string questIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                questIds = "\n"+ Lang.GetTranslate("Quests Ids")+":";
                questCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    questIds += " " + data["id"] + ",";
                }
            }
         //   Debug.Log(sqlquests);

            //Check Sets
            string sqlSets = "SELECT set_id FROM item_set_items WHERE isactive=1 AND template_id=" + editingDisplay.id;
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlSets);
            int setsCount = 0;
            string setsIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                setsIds = "\n"+ Lang.GetTranslate("Item Sets Ids")+":";
                setsCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    setsIds += " " + data["id"] + ",";
                }
            }

            //Check Crafting
            string sqlCrafting = "SELECT id FROM crafting_recipes WHERE recipeItemID =" + editingDisplay.id+ " OR resultItemID=" + editingDisplay.id;
            for (int ii = 2; ii <= 16; ii++)
            {
                sqlCrafting += " OR resultItem"+ii+"ID =" + editingDisplay.id;
            }
            for (int ii = 1; ii <= 16; ii++)
            {
                sqlCrafting += " OR component" + ii + " =" + editingDisplay.id;
            }
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlCrafting);
            int craftingCount = 0;
            string craftingIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                craftingIds = "\n"+ Lang.GetTranslate("Crafting recipe Ids")+":";
                craftingCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    craftingIds += " " + data["id"] + ",";
                }
            }
          //  Debug.Log(sqlquests);

            //Check Crafting
            string sqlBuilding = "SELECT id FROM build_object_stage WHERE itemReq1 =" + editingDisplay.id ;
            for (int ii = 2; ii <= 6; ii++)
            {
                sqlBuilding += " OR itemReq" + ii + " =" + editingDisplay.id;
            }
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlBuilding);
            int buildingCount = 0;
            string buildingIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                buildingIds = "\n"+ Lang.GetTranslate("Buildings Stage Ids")+":";
                buildingCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    buildingIds += " " + data["id"] + ",";
                }
            }
          //  Debug.Log(sqlBuilding);

            //Check Ability
            string sqlAbility = "SELECT id FROM abilities WHERE reagentRequired =" + editingDisplay.id+ " OR reagent2Required =" + editingDisplay.id+ " OR reagent3Required =" + editingDisplay.id;
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlAbility);
            int abilityCount = 0;
            string abilityIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                abilityIds = "\n"+ Lang.GetTranslate("Abilities Ids")+":";
                buildingCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    abilityIds += " " + data["id"] + ",";
                }
            }
          //  Debug.Log(sqlBuilding);

            //Check Mobs
            string sqlMob = "SELECT id FROM mob_templates WHERE primaryWeapon =" + editingDisplay.id + " OR secondaryWeapon =" + editingDisplay.id ;
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlMob);
            int mobCount = 0;
            string mobIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                mobIds = "\n"+ Lang.GetTranslate("Mobs Ids")+":";
                mobCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    mobIds += " " + data["id"] + ",";
                }
            }
          //  Debug.Log(sqlMob);

            //Check Dialogs
            string sqlDialog = "SELECT id FROM dialogue WHERE option1itemReq =" + editingDisplay.id + " OR option2itemReq =" + editingDisplay.id + " OR option3itemReq =" + editingDisplay.id;
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlDialog);
            int dialogCount = 0;
            string dialogIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                dialogIds = "\n"+ Lang.GetTranslate("Dialogs Ids")+":";
                dialogCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    dialogIds += " " + data["id"] + ",";
                }
            }
        //    Debug.Log(sqlDialog);

            //Check Resource dromp
            string sqlResource = "SELECT resource_template FROM resource_drop WHERE item =" + editingDisplay.id ;
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlResource);
            int resourceCount = 0;
            string resourceIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                resourceIds = "\n"+ Lang.GetTranslate("Rresource Node Ids")+":";
                resourceCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    resourceIds += " " + data["resource_template"] + ",";
                }
            }
        //    Debug.Log(sqlResource);

            //Check Resource dromp
            string sqlPlyItems = "SELECT obj_id FROM player_items WHERE templateID  =" + editingDisplay.id;
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.atavismDatabasePrefix, sqlPlyItems);
            int plyItemsCount = 0;
            string plyItemsIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                plyItemsIds = "\n" + Lang.GetTranslate("Inventory of Player Id") + ":";
                plyItemsCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    plyItemsIds += " " + data["obj_id"] + ",";
                }
            }
            //   Debug.Log(sqlPlyItems);

            //Check Auctions 
            string sqlAucItems = "SELECT id FROM auction_house WHERE item_template_id  =" + editingDisplay.id;
           // Debug.Log(sqlAucItems);
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.adminDatabasePrefix, sqlAucItems);
         
            int aucItemsCount = 0;
            string aucItemsIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                aucItemsIds = "\n" + Lang.GetTranslate("Auctions Id") + ":";
                aucItemsCount = rows.Count;
                 foreach (Dictionary<string, string> data in rows)
                {
                    aucItemsIds += " " + data["id"] + ",";
                }
            }
            //Check Auctions ended
            string sqlAucHistItems = "SELECT id FROM auction_house_ended WHERE item_template_id  =" + editingDisplay.id;
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.adminDatabasePrefix, sqlAucHistItems);
            int aucHistItemsCount = 0;
            string aucHistItemsIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                aucHistItemsIds = "\n" + Lang.GetTranslate("Auctions ended Id") + ":";
                aucHistItemsCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    aucHistItemsIds += " " + data["id"] + ",";
                }
            }



            DatabasePack.SaveLog("Delete Item id =" + editingDisplay.id + " assigned to " + charIds + merchantTableIds + lootTableIds + questIds+ setsIds+ craftingIds+ buildingIds+ abilityIds+ mobIds+ dialogIds+ resourceIds+ plyItemsIds+aucItemsIds+ aucHistItemsIds);

            if (charCount == 0 && merchantTableCount == 0 && buildingCount == 0 && lootTableCount == 0 && questCount == 0 && setsCount == 0 && craftingCount == 0 && abilityCount == 0 && mobCount == 0 && dialogCount == 0 && resourceCount==0 && plyItemsCount == 0 && aucHistItemsCount == 0 && aucItemsCount == 0)
            {
                // Remove the prefab
                DeletePrefab();
                Register delete = new Register("item_id", "?item_id", MySqlDbType.Int32, editingDisplay.id.ToString(), Register.TypesOfField.Int);
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "character_create_items", delete);

                delete = new Register("itemID", "?itemID", MySqlDbType.Int32, editingDisplay.id.ToString(), Register.TypesOfField.Int);
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "merchant_item", delete);

                delete = new Register("template_id", "?template_id", MySqlDbType.Int32, editingDisplay.id.ToString(), Register.TypesOfField.Int);
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "item_set_items", delete);
                
                    
                // Delete the database entry
                string  query = "UPDATE " + tableName + " SET isactive = 0 where id = " + editingDisplay.id;
                DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            // Update online table to avoid access the database again		
            selectedDisplay = -1;
            newSelectedDisplay = 0;
            NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " "+ Lang.GetTranslate("Deleted"));
            }
            else
            {
                EditorUtility.DisplayDialog("", Lang.GetTranslate("You can not delete Item")+" " + editingDisplay.Name+" "+ Lang.GetTranslate("because it is assigned in")+":" + charIds + merchantTableIds + lootTableIds + questIds + setsIds + craftingIds + buildingIds + abilityIds + mobIds + dialogIds + resourceIds+ plyItemsIds + aucItemsIds + aucHistItemsIds, Lang.GetTranslate("OK"), "");
            }
            dataLoaded = false;
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
                where.Add(new Register("item_id", "?item_id", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "item_templates_options", where);

                dataLoaded = false;
                dataRestoreLoaded = false;
                // Load();
                NewResult(Lang.GetTranslate("Entry Permanent Deleted"));
            }
        }
        void SaveAll()
        {
            if (!dataLoaded)
            {
                Load();
            }

            foreach (int id in displayKeys)
            {
                editingDisplay = LoadEntity(id);
                UpdateEntry(false);
            }
        }

        void RestoreEntry(int id)
        {
            NewResult(Lang.GetTranslate("Restoring..."));
            string query = "UPDATE " + tableName + " SET isactive = 1 where id = " + id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            editingDisplay = LoadEntity(id);
            CreatePrefab();

            dataLoaded = false;
            dataRestoreLoaded = false;
           // Load();
            NewResult(Lang.GetTranslate("Entry Restored"));
        }

        void Duplicate()
        {
            // Update name of current entry by adding "1" to the end
            editingDisplay.Name = editingDisplay.Name + " (Clone)";
            editingDisplay.id = 0;
            InsertEntry();
            state = State.Loaded;
            linkedTablesLoaded = false;
            dataLoaded = false;
            Load();
        }

        void GenerateAllPrefabs()
        {
       /*     if (!dataLoaded)
            {
                Load();
            }

            // Delete all existing prefabs?
          //  ItemPrefab.DeleteAllPrefabs();

            foreach (int id in displayKeys)
            {
                ItemData itemData = LoadEntity(id);
                item = new ItemPrefab(itemData);
                item.Save(itemData);
            }
            ItemPrefab.DeletePrefabWithoutIDs(displayKeys);*/
        }

        void CreatePrefab()
        {
            // Configure the correponding prefab
         /*   item = new ItemPrefab(editingDisplay);
            item.Save(editingDisplay);*/
        }

        void DeletePrefab()
        {
          /*  item = new ItemPrefab(editingDisplay);

            if (item.Load())
                item.Delete();*/
        }

    }
}