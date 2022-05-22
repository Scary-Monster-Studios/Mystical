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
    public class ServerEnchanteProfile : AtavismDatabaseFunction
    {

        enum EnchantView
        {
            Home,
            EnchantProfile,
            Grade
        }

        public new Dictionary<int, EnchantData> dataRegister;
        public new EnchantData editingDisplay;
        public new EnchantData originalDisplay;
        public QualityData quality;

        public int[] currencyIds = new int[] { -1 };
        public string[] currencyOptions = new string[] { "~ none ~" };

        public string[] damageOptions = new string[] { "~ none ~" };

        public string[] itemEffectTypeOptions = new string[] { "~ none ~" };
        public string[] statOptions = new string[] { "~ none ~" };
        public string[] accountEffectTypeOptions = new string[] { "~ none ~" };
        EnchantView view = EnchantView.Home;
        public string[] itemQualityOptions = new string[] { "~ none ~" };

        // Handles the prefab creation, editing and save
       // private ItemPrefab item = null;

        // Filter/Search inputs
        private string currencySearchInput = "";
        private List<string> effectSearchInput = new List<string>();

        // Use this for initialization
        public ServerEnchanteProfile()
        {
            functionName = "Enchanting";
            // Database tables name
            tableName = "item_enchant_profile";
            functionTitle = "Enchant Profile Configuration";
            loadButtonLabel = "Load Enchant Profile";
            notLoadedText = "No Enchant Profile loaded.";
            // Init
            dataRegister = new Dictionary<int, EnchantData>();

            editingDisplay = new EnchantData();
            originalDisplay = new EnchantData();
            showRecovery = false;
         }

        void resetSearch(bool fokus)
        {
            currencySearchInput = "";
            effectSearchInput.Clear();
           if(fokus) GUI.FocusControl(null);
        }

        public override void Activate()
        {
            linkedTablesLoaded = false;
            resetSearch(true);
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
                //currencyOptions [optionsId] = "~ none ~"; 
                currencyIds = new int[rows.Count + 1];
                //currencyIds [optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    currencyOptions[optionsId] = data["id"] + ":" + data["name"];
                    currencyIds[optionsId] = int.Parse(data["id"]);
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
                string query = "SELECT distinct id,name FROM " + tableName + " where isactive = 1";

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

        EnchantData LoadEntity(int id)
        {
            EnchantLevelData eld = new EnchantLevelData();
            // Read all entries from the table
            string query = "SELECT " + eld.GetFieldsString() + " FROM " + tableName + " where isactive = 1 AND id=" + id;
            //  Debug.LogError(query);
            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            EnchantData ad = new EnchantData(id, "");

            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    //foreach(string key in data.Keys)
                    //	Debug.Log("Name[" + key + "]:" + data[key]);
                    //return;
                    EnchantLevelData display = new EnchantLevelData();
                    display.id = int.Parse(data["id"]);
                    display.Name = data["Name"];
                    ad.Name = data["Name"];
                    display.level = int.Parse(data["level"]);
                    display.cost = int.Parse(data["cost"]);
                    display.chance = float.Parse(data["chance"]);
                    display.allStats = bool.Parse(data["all_stats"]);
                    display.percentage = bool.Parse(data["percentage"]);
                    display.stat_value = int.Parse(data["stat_value"]);
                    display.add_not_exist = bool.Parse(data["add_not_exist"]);
                    display.lower_by = int.Parse(data["lower_by"]);
                    display.lower_to = int.Parse(data["lower_to"]);
                    display.damage = int.Parse(data["damage"]);
                    display.damagep = int.Parse(data["damagep"]);
                    display.currency = int.Parse(data["currency"]);
                    for (int i = 1; i <= display.maxStatsEntries; i++)
                    {
                        string effectName = data["effect" + i + "name"];
                        if (effectName != null && effectName != "")
                        {
                            int effectValuep = int.Parse(data["effect" + i + "valuep"]);
                            int effectValue = int.Parse(data["effect" + i + "value"]);
                            StatEntry entry = new StatEntry(effectName, effectValue, effectValuep);
                            display.stats.Add(entry);
                        }
                    }
                    display.isLoaded = true;
                    ad.enchantLevels.Add(display);
                }
            }
           return ad;
        }

        public override void LoadRestore()
        {
            if (!dataRestoreLoaded)
            {
                // Clean old data
                dataRegister.Clear();
                displayKeys.Clear();

                // Read all entries from the table
                string query = "SELECT distinct id,name FROM " + tableName + " where isactive = 0";

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


        public void LoadQuality()
        {
            if (!dataLoaded)
            {
                // Clean old data
                dataRegister.Clear();
                displayKeys.Clear();
               // EnchantLevelData eld = new EnchantLevelData();
                // Read all entries from the table
                string query = "SELECT * FROM item_quality order by id";
                //  Debug.LogError(query);
                // If there is a row, clear it.
                if (rows != null)
                    rows.Clear();
                quality = new QualityData();
                // Load data
                rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
                //Debug.Log("#Rows:"+rows.Count);
                // Read all the data

                if ((rows != null) && (rows.Count > 0))
                {
                    foreach (Dictionary<string, string> data in rows)
                    {
                        //    Debug.LogError(int.Parse(data["id"])+", "+data["name"]+", "+float.Parse(data["cost"])+", "+float.Parse(data["chance"]));
                        quality.list.Add(new QualityEntry(int.Parse(data["id"]), data["name"], float.Parse(data["cost"]), float.Parse(data["chance"])));
                    }
                }
                dataLoaded = true;

            }
        }


     /*   private int SelectTab(Rect pos, int sel)
        {
            pos.y += ImagePack.tabTop;
            pos.x += ImagePack.tabLeft;
            bool create = false;
            bool edit = false;
            bool doc = false;

            switch (sel)
            {
                case 0:
                    //   create = true;
                    break;
                case 1:
                    edit = true;
                    break;
                case 2:
                    doc = true;
                    break;
            }
            if (view == EnchantView.EnchantProfile)
            {
                if (create)
                    pos.y += ImagePack.tabSpace;
                if (ImagePack.DrawTabCreate(pos, create))
                    return 0;
                if (create)
                    pos.y -= ImagePack.tabSpace;
                pos.x += ImagePack.tabMargin;
            }
            pos.x += ImagePack.tabMargin;
            if (edit)
                pos.y += ImagePack.tabSpace;
            if (ImagePack.DrawTabEdit(pos, edit))
                return 1;
            if (edit)
                pos.y -= ImagePack.tabSpace;
            pos.x += ImagePack.tabMargin;
            if (doc)
                pos.y += ImagePack.tabSpace;
            if (ImagePack.DrawTabDoc(pos, doc))
                return 2;
            if (doc)
                pos.y -= ImagePack.tabSpace;

            return sel;
        }
        */

        public override void Draw(Rect box)
        {

            // Setup the layout
            Rect pos = box;


            if (view == EnchantView.Home)
            {
                pos.x += ImagePack.innerMargin;
                pos.y += ImagePack.innerMargin;
                pos.width -= ImagePack.innerMargin;
                pos.height = ImagePack.fieldHeight;
                DrawHome(pos);
                dataLoaded = false;
            }
            else if (view == EnchantView.EnchantProfile)
            {
                base.Draw(pos);

                //  DrawLoaded(pos);
            }
            else if (view == EnchantView.Grade)
            {
                base.Draw(pos);
            }
            
        }
        
        void DrawHome(Rect pos)
        {
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Enchanting"));
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Enchant Profile")))
            {
                view = EnchantView.EnchantProfile;
                linkedTablesLoaded = false;
                showCreate = true;
            }
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Enchant cost & chance per grade")))
            {
                view = EnchantView.Grade;
                LoadQuality();
                linkedTablesLoaded = false;
                showCreate = false;
            }
            pos.y += ImagePack.fieldHeight;
            /*if (ImagePack.DrawButton(pos.x, pos.y, "Manage Graveyards"))
              {
                  view = EnchantView.Graveyards;
              }
              pos.y += ImagePack.fieldHeight;
              if (ImagePack.DrawButton(pos.x, pos.y, "Manage Claims"))
              {
                  view = EnchantView.Claims;
              }
              pos.y += ImagePack.fieldHeight;*/
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
            pos.width /= 2;
            pos.x += pos.width;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Back to Enchanting")))
            {
                view = EnchantView.Home;
            }
            pos.x -= pos.width;
            pos.width *= 2;

            if (view.Equals(EnchantView.EnchantProfile))
            {
                if (displayList.Count <= 0)
                {
                    pos.y += ImagePack.fieldHeight;
                    ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create an Profile before edit it."));
                    return;
                }
                // Draw the content database info
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Enchant Profile"));
                

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
            }

            if (state != State.Loaded && view.Equals(EnchantView.EnchantProfile))
            {
                pos.x += ImagePack.innerMargin;
                pos.width /= 2;
                //Draw super magical compound object.
                newSelectedDisplay = ImagePack.DrawDynamicPartialListSelector(pos, Lang.GetTranslate("Search Filter")+": ", ref entryFilterInput, selectedDisplay, displayList);

                pos.y += ImagePack.fieldHeight;
                //Build prefabs button
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Duplicate")))
                {
                    Duplicate();
                }
                pos.x -= pos.width;

                pos.width *= 2;
                pos.y += ImagePack.fieldHeight * 1.5f;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Profile Properties")+":");
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
            if (view.Equals(EnchantView.EnchantProfile))
            {
                editingDisplay = new EnchantData();
                originalDisplay = new EnchantData();
            }
            selectedDisplay = -1;
        }

        // Edit or Create
        public override void DrawEditor(Rect box, bool newItem)
        {
            newEntity = newItem;
            // Setup the layout
            Rect pos = box;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;
            if (state.Equals(State.New))
            {
                pos.width /= 2;
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Back to Enchanting")))
                {
                    view = EnchantView.Home;
                }
                pos.x -= pos.width;
                pos.width *= 2;
                pos.y += 2f * ImagePack.innerMargin;
            }
            if (view.Equals(EnchantView.EnchantProfile))
            {
                if (!linkedTablesLoaded)
                {
                    LoadCurrencyOptions();
                    LoadStatOptions();
                    linkedTablesLoaded = true;
                }

                // Draw the content database info
                if (newEntity)
                {
                    ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new enchant profile"));
                    pos.y += ImagePack.fieldHeight;
                }

                editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.8f);
                pos.y += ImagePack.fieldHeight;
                if (editingDisplay.enchantLevels.Count == 0)
                    editingDisplay.enchantLevels.Add(new EnchantLevelData(editingDisplay.Name));

                for (int j = 0; j < editingDisplay.enchantLevels.Count; j++)
                {
                    ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Enchant level Settings #") + (j + 1));
                    pos.y += ImagePack.fieldHeight;
                    editingDisplay.enchantLevels[j].Name = editingDisplay.Name;
                    //  editingDisplay.enchantLevels[j].lower_by = editingDisplay.lower_by;
                    //   editingDisplay.enchantLevels[j].lower_to = editingDisplay.lower_to;
                    editingDisplay.enchantLevels[j].id = editingDisplay.id;
                    int from = 1;
                    if (j > 0)
                        from = editingDisplay.enchantLevels[j - 1].level + 1;
                    if (editingDisplay.enchantLevels[j].level < from)
                        editingDisplay.enchantLevels[j].level = from;
                    editingDisplay.enchantLevels[j].level = ImagePack.DrawField(pos, Lang.GetTranslate("Level from")+" " + from + " "+ Lang.GetTranslate("to")+":", editingDisplay.enchantLevels[j].level);
                    pos.y += ImagePack.fieldHeight;
                    editingDisplay.enchantLevels[j].lower_by = ImagePack.DrawField(pos, Lang.GetTranslate("On fail level lower by")+":", editingDisplay.enchantLevels[j].lower_by);
                    pos.y += ImagePack.fieldHeight;
                    editingDisplay.enchantLevels[j].lower_to = ImagePack.DrawField(pos, Lang.GetTranslate("On fail level lower to")+":", editingDisplay.enchantLevels[j].lower_to);
                    pos.y += ImagePack.fieldHeight;
                    editingDisplay.enchantLevels[j].chance = ImagePack.DrawField(pos, Lang.GetTranslate("Chance")+":", editingDisplay.enchantLevels[j].chance);
                    pos.y += ImagePack.fieldHeight;
                    editingDisplay.enchantLevels[j].cost = ImagePack.DrawField(pos, Lang.GetTranslate("Cost")+":", editingDisplay.enchantLevels[j].cost);
                    pos.y += ImagePack.fieldHeight;
                    pos.width = pos.width / 2;
                    //  editingDisplay.enchantLevels[j].currency = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Currency")+": ", ref currencySearchInput, editingDisplay.enchantLevels[j].currency, currencyOptions);
                    int selectedCurrency = GetOptionPosition(editingDisplay.enchantLevels[j].currency, currencyIds);
                    selectedCurrency = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Currency") + ": ", ref currencySearchInput, selectedCurrency, currencyOptions);
                    editingDisplay.enchantLevels[j].currency = currencyIds[selectedCurrency];
                    pos.width = pos.width * 2;
                    pos.y += ImagePack.fieldHeight;
                    pos.width = pos.width / 2;
                    editingDisplay.enchantLevels[j].gear_score = ImagePack.DrawField(pos, Lang.GetTranslate("Gear Score") + ":", editingDisplay.enchantLevels[j].gear_score);
                    pos.x += pos.width;
                    editingDisplay.enchantLevels[j].gear_scorep = ImagePack.DrawField(pos, Lang.GetTranslate("Gear Score percentage") + ":", editingDisplay.enchantLevels[j].gear_scorep);
                    pos.y += ImagePack.fieldHeight;
                    pos.x -= pos.width;
                    pos.width = pos.width * 2;
                    editingDisplay.enchantLevels[j].allStats = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("All stats on item")+":", editingDisplay.enchantLevels[j].allStats);
                    if (editingDisplay.enchantLevels[j].allStats)
                    {
                        pos.y += ImagePack.fieldHeight;
                        editingDisplay.enchantLevels[j].percentage = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Percentage")+":", editingDisplay.enchantLevels[j].percentage);
                        pos.y += ImagePack.fieldHeight;
                        editingDisplay.enchantLevels[j].stat_value = ImagePack.DrawField(pos, Lang.GetTranslate("Stat value")+":", editingDisplay.enchantLevels[j].stat_value);
                    }
                    else
                    {
                        pos.y += ImagePack.fieldHeight;
                        editingDisplay.enchantLevels[j].add_not_exist = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Add not exist")+":", editingDisplay.enchantLevels[j].add_not_exist);

                        pos.y += 1f * ImagePack.fieldHeight;
                        ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Stats")+":");
                        pos.y += ImagePack.fieldHeight;
                        pos.width = pos.width / 2;
                        editingDisplay.enchantLevels[j].damage = ImagePack.DrawField(pos, Lang.GetTranslate("Damage")+":", editingDisplay.enchantLevels[j].damage);
                            
                        pos.x += pos.width;
                        editingDisplay.enchantLevels[j].damagep = ImagePack.DrawField(pos, Lang.GetTranslate("Damage percentage")+":", editingDisplay.enchantLevels[j].damagep);
                        pos.x -= pos.width;
                        pos.width = pos.width * 2;
                        pos.y += ImagePack.fieldHeight;
                        for (int i = 0; i < editingDisplay.enchantLevels[j].maxStatsEntries; i++)
                        {
                            if (editingDisplay.enchantLevels[j].stats.Count > i)
                            {
                                pos.width = pos.width / 2;
                                // Generate search string if none exists
                                if (effectSearchInput.Count <= i)
                                {
                                    effectSearchInput.Add("");
                                }
                                string searchString = effectSearchInput[i];
                                //    pos.y += ImagePack.fieldHeight;
                                editingDisplay.enchantLevels[j].stats[i].itemStatName = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Stat Name")+": ", ref searchString, editingDisplay.enchantLevels[j].stats[i].itemStatName, statOptions);
                                pos.y += ImagePack.fieldHeight;
                                editingDisplay.enchantLevels[j].stats[i].itemStatValue = ImagePack.DrawField(pos, Lang.GetTranslate("Value")+":", editingDisplay.enchantLevels[j].stats[i].itemStatValue);
                                pos.x += pos.width;
                                editingDisplay.enchantLevels[j].stats[i].itemStatValuePercentage = ImagePack.DrawField(pos, Lang.GetTranslate("Value percentage")+":", editingDisplay.enchantLevels[j].stats[i].itemStatValuePercentage);
                                pos.x -= pos.width;

                                // Save back the search string
                                effectSearchInput[i] = searchString;

                                pos.x += pos.width;
                                pos.y += ImagePack.fieldHeight;
                                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove stat")))
                                {
                                    editingDisplay.enchantLevels[j].stats.RemoveAt(i);
                                }
                                pos.x -= pos.width;
                                pos.y += ImagePack.fieldHeight;
                                pos.width = pos.width * 2;
                            }
                        }
                        if (editingDisplay.enchantLevels[j].stats.Count < editingDisplay.enchantLevels[j].maxStatsEntries)
                        {
                            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add stat")))
                            {
                                StatEntry statEntry = new StatEntry("", 0, 0);
                                editingDisplay.enchantLevels[j].stats.Add(statEntry);
                            }
                        }
                    }
                    pos.width = pos.width / 2;
                    pos.x += pos.width;
                    pos.y += ImagePack.fieldHeight;
                    if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove profile level #") + (j + 1)))
                    {
                        editingDisplay.enchantLevels.RemoveAt(j);
                    }
                    pos.x -= pos.width;
                    pos.y += ImagePack.fieldHeight;
                    pos.width = pos.width * 2;

                }

                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add profile level")))
                {
                    EnchantLevelData enchantLevel = new EnchantLevelData(editingDisplay.Name);
                    editingDisplay.enchantLevels.Add(enchantLevel);
                }

                // Save data		
                pos.x -= ImagePack.innerMargin;
                pos.y += 3f * ImagePack.fieldHeight;
                if (!newEntity)
                {
                    Color temp = GUI.color;
                    GUI.color = Color.red;
                    ImagePack.DrawText(pos, "      "+Lang.GetTranslate("Attention, erasing is permanent !!! "));
                    GUI.color = temp;
                    pos.y += ImagePack.fieldHeight;
                }
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
            if (view.Equals(EnchantView.Grade) && !newItem)
            {
                if (!linkedTablesLoaded)
                {
                    itemQualityOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Item Quality", false);
                    linkedTablesLoaded = true;
                }
                // Draw the content database info
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Item Quality Settings"));
                pos.y += ImagePack.fieldHeight;
                for (int j = 0; j < itemQualityOptions.Length; j++)
                {
                    if (quality.list.Count < j + 1)
                    {
                        quality.list.Add(new QualityEntry(itemQualityOptions[j]));
                    }
                    ImagePack.DrawLabel(pos.x, pos.y, itemQualityOptions[j]);
                    pos.y += ImagePack.fieldHeight;
                    pos.width = pos.width / 2;
                    quality.list[j].chance = ImagePack.DrawField(pos, Lang.GetTranslate("Chance (%)")+":", quality.list[j].chance);
                    pos.x += pos.width;
                    quality.list[j].cost = ImagePack.DrawField(pos, Lang.GetTranslate("Cost (%)")+": ", quality.list[j].cost);
                    pos.x -= pos.width;
                    pos.width = pos.width * 2;
                    pos.y += ImagePack.fieldHeight;
                }
                // Save data		
                pos.x -= ImagePack.innerMargin;
                pos.y += ImagePack.fieldHeight;
                pos.width /= 3;
                showSave = true;

                if (resultTimeout != -1 && resultTimeout < Time.realtimeSinceStartup && result != "")
                {
                    result = "";
                }

                EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight);

            }

        }

        public override void save()
        {
            if (view.Equals(EnchantView.Grade) && !newEntity)
            {
                UpdateGrade();
            }
            else if (view.Equals(EnchantView.EnchantProfile))
            {
                if (newEntity)
                    InsertEntry();
                else
                    UpdateEntry();
            }
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
            string query1 = "SELECT MAX(id) as id FROM " + tableName;
            if (rows != null)
                rows.Clear();
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query1);
            //       Debug.Log("#Rows:" + rows.Count);
            //       Debug.Log("#Rows:" + rows[0]);

            int plid = 0;
            if (rows != null && rows.Count > 0 && rows[0].ContainsKey("id") && !String.IsNullOrEmpty(rows[0]["id"]))
            {
                plid = int.Parse(rows[0]["id"]);
            }
            int itemID = -1;
            for (int i = 0; i < editingDisplay.enchantLevels.Count; i++)
            {
                editingDisplay.enchantLevels[i].id = plid + 1;
                editingDisplay.enchantLevels[i].Name = editingDisplay.Name;
                NewResult(Lang.GetTranslate("Inserting..."));
                // Setup the update query
                string query = "INSERT INTO " + tableName;
                query += " (id," + editingDisplay.enchantLevels[i].FieldList("", ", ") + ") ";
                query += "VALUES ";
                query += " (?id," + editingDisplay.enchantLevels[i].FieldList("?", ", ") + ") ";
                //          Debug.LogError("sql:  " + query);

                // Setup the register data		
                List<Register> update = new List<Register>();
                foreach (string field in editingDisplay.enchantLevels[i].fields.Keys)
                {
                    update.Add(editingDisplay.enchantLevels[i].fieldToRegister(field));
                }
                update.Add(new Register("id", "?id", MySqlDbType.Int32, editingDisplay.enchantLevels[i].id.ToString(), Register.TypesOfField.Int));

                // Update the database
                itemID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);
                // editingDisplay.enchantLevels.
                // If the insert failed, don't insert the spawn marker
            }
            if (itemID != -1)
            {
                // Update online table to avoid access the database again			
                editingDisplay.id = plid + 1;
                // Insert the Requirements

                editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
              //  dataRegister.Add(editingDisplay.id, editingDisplay);
              //  displayKeys.Add(editingDisplay.id);
                newItemCreated = true;
                linkedTablesLoaded = false;
                dataLoaded = false;
                Load();
                // Configure the correponding prefab
                NewResult(Lang.GetTranslate("New entry inserted"));
            }
            else
            {
                NewResult("Error" + Lang.GetTranslate("Error occurred, please check the Console"));
            }

        }


        // Update existing entries in the table based on the iddemo_table
        void UpdateEntry()
        {
            NewResult(Lang.GetTranslate("Updating..."));

            string query2 = "DELETE FROM " + tableName + " WHERE id=?id";

            // Setup the register data		
            List<Register> update1 = new List<Register>();
            update1.Add(new Register("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString(), Register.TypesOfField.Int));
            // Update the database
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query2, update1);
            //int itemID = -1;
            for (int i = 0; i < editingDisplay.enchantLevels.Count; i++)
            {
                string query = "INSERT INTO " + tableName;
                query += " (id," + editingDisplay.enchantLevels[i].FieldList("", ", ") + ") ";
                query += "VALUES ";
                query += " (?id," + editingDisplay.enchantLevels[i].FieldList("?", ", ") + ") ";
                //     Debug.LogError("sql:  " + query);

                // Setup the register data		
                List<Register> update = new List<Register>();
                foreach (string field in editingDisplay.enchantLevels[i].fields.Keys)
                {
                    update.Add(editingDisplay.enchantLevels[i].fieldToRegister(field));
                }
                update.Add(new Register("id", "?id", MySqlDbType.Int32, editingDisplay.enchantLevels[i].id.ToString(), Register.TypesOfField.Int));

                // Update the database
                DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);

            }

            // Update online table to avoid access the database again			
            // dataRegister[displayKeys[selectedDisplay]] = editingDisplay;
            linkedTablesLoaded = false;
            dataLoaded = false;
             // Remove the prefab
            // Configure the correponding prefab
            NewResult(Lang.GetTranslate("Entry") + "  " + editingDisplay.Name + " " + Lang.GetTranslate("updated"));
          Load();
         }


        void UpdateGrade()
        {
            NewResult(Lang.GetTranslate("Updating grade..."));
            List<Register> update = new List<Register>();
            //      Debug.LogError(quality.list.Count);
            foreach (QualityEntry qe in quality.list)
            {
                if (qe.id.Equals(-1))
                {
                    string query1 = "SELECT MAX(id) as id FROM item_quality";
                    if (rows != null)
                        rows.Clear();
                    rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query1);
                    //         Debug.Log("#Rows:" + rows.Count);
                    //         Debug.Log("#Rows:" + rows[0]);

                    int plid = 0;
                    if (rows != null && rows.Count > 0 && rows[0].ContainsKey("id") && !String.IsNullOrEmpty(rows[0]["id"]))
                    {
                        plid = int.Parse(rows[0]["id"]);
                    }
                    //       Debug.Log("#plid:" + plid);
                    string query = "INSERT INTO item_quality (`id`,`name`,`cost`,`chance`)  VALUES (" + (plid + 1) + ",'" + qe.Name + "'," + qe.cost + "," + qe.chance + ") ";
                    //       Debug.Log("#query:" + query);
                    DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);
                }
                else
                {
                    string query = "UPDATE item_quality  SET cost=" + qe.cost + ", chance=" + qe.chance + " WHERE id=" + qe.id;
                    //      Debug.Log("#query:" + query);
                    DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);
                }
            }

            NewResult(Lang.GetTranslate("Grades updated"));
        }


        // Delete entries from the table
        void DeleteEntry()
        {

            string query = "DELETE FROM " + tableName + " WHERE id=" + editingDisplay.id;
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
            NewResult(Lang.GetTranslate("Entry Restored"));
        }

        void Duplicate()
        {
            // Update name of current entry by adding "1" to the end
            editingDisplay.Name = editingDisplay.Name + " (Clone)";
            editingDisplay.id = -1;
            InsertEntry();
            state = State.Loaded;
            linkedTablesLoaded = false;
            dataLoaded = false;
            Load();
        }



    }
}