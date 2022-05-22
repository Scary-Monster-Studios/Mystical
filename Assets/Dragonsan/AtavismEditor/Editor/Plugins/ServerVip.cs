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
    public class ServerVip : AtavismDatabaseFunction
    {

        enum VipView
        {
            Home,
            Level,
            Settings
        }

        public new Dictionary<int, VipData> dataRegister;
        public new VipData editingDisplay;
        public new VipData originalDisplay;
        public List<BonusOptionData> optionslist = new List<BonusOptionData>();

        public int[] bonusIds = new int[] { -1 };
        public string[] bonusOptions = new string[] { "~ none ~" };
        public string[] bonusOptionsParam = new string[] { "" };
        public string[] bonusOptionsCode = new string[] { "" };

        public string[] damageOptions = new string[] { "~ none ~" };

        public string[] itemEffectTypeOptions = new string[] { "~ none ~" };
        public string[] vipOptions = new string[] { "~ none ~" };
        public string[] accountEffectTypeOptions = new string[] { "~ none ~" };
        VipView view = VipView.Level;
        public string[] itemQualityOptions = new string[] { "~ none ~" };

        // Handles the prefab creation, editing and save
       // private ItemPrefab item = null;

        // Filter/Search inputs
        private string currencySearchInput = "";
        private List<string> effectSearchInput = new List<string>();

        // Use this for initialization
        public ServerVip()
        {
            functionName = "Vip";
            // Database tables name
            tableName = "vip_level";
            functionTitle = "Vip Level Configuration";
            loadButtonLabel = "Load Vip Levels";
            notLoadedText = "No Vip Levels loaded.";
            // Init
            dataRegister = new Dictionary<int, VipData>();

            editingDisplay = new VipData();
            originalDisplay = new VipData();
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



     /*   public void LoadCurrencyOptions()
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
        }**/



        public void LoadStatOptions()
        {
          
                // Read all entries from the table
                string query = "SELECT name FROM bonuses_settings where isactive = 1";

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
                    vipOptions = new string[rows.Count + 1];
                vipOptions[optionsId] = "~ none ~";
                    foreach (Dictionary<string, string> data in rows)
                    {
                        optionsId++;
                    vipOptions[optionsId] = data["name"];
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
                string query = "SELECT * FROM " + tableName + " where isactive = 1 ";

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

        VipData LoadEntity(int id)
        {
            // Read all entries from the table
            string query = "SELECT * FROM " + tableName + " where isactive = 1 AND id=" + id;
            //  Debug.LogError(query);
            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            VipData display = new VipData();

            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {   //foreach(string key in data.Keys)
                    //	Debug.Log("Name[" + key + "]:" + data[key]);
                    //return;
                    display.id = int.Parse(data["id"]);
                    display.Name = data["name"];
                    display.level = int.Parse(data["level"]);
                    display.description = data["description"];
                    display.max_points = int.Parse(data["max_points"]);
                    display.isLoaded = true;
                    //   ad.enchantLevels.Add(display);
                }
            }
            query = "SELECT * FROM vip_level_bonuses where vip_level_id=" + id;

            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {   //foreach(string key in data.Keys)
                    //	Debug.Log("Name[" + key + "]:" + data[key]);

                    display.bonuses.Add(new BonusEntry(int.Parse(data["vip_level_id"]), int.Parse(data["bonus_settings_id"]), int.Parse(data["value"]), float.Parse(data["valuep"])));
                }
            }


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

        public void LoadBonusSettings()
        {
            // Clean old data
            // Read all entries from the table
            string query = "SELECT * FROM bonuses_settings where isactive = 1 order by id ";
            // Debug.LogError(query);
            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();
            optionslist.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data

            if ((rows != null) && (rows.Count > 0))
            {
                int optionsId = 0;
                bonusOptions = new string[rows.Count + 1];
                bonusOptions[optionsId] = "~ none ~"; 
                bonusIds = new int[rows.Count + 1];
                bonusIds[optionsId] = -1;
                bonusOptionsParam = new string[rows.Count + 1];
                bonusOptionsCode = new string[rows.Count + 1];
                bonusOptionsParam[optionsId] = "";
                bonusOptionsCode[optionsId] = "";
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    bonusOptions[optionsId] = data["name"];
                    bonusIds[optionsId] = int.Parse(data["id"]);
                    bonusOptionsParam[optionsId] = data["param"];
                    bonusOptionsCode[optionsId] = data["code"];
                    //    Debug.LogError(int.Parse(data["id"])+", "+data["name"]+", "+float.Parse(data["cost"])+", "+float.Parse(data["chance"]));
                    optionslist.Add(new BonusOptionData(int.Parse(data["id"]), data["name"], data["code"], data["param"]));
                }
            }
            //     dataLoaded = true;

            //  }
        }

        public void LoadBonusSettings2()
        {
         /*   if (!dataLoaded)
            {*/
                // Clean old data
                dataRegister.Clear();
                displayKeys.Clear();
               // EnchantLevelData eld = new EnchantLevelData();
                // Read all entries from the table
                string query = "SELECT * FROM vip_settings where isactive = 1 order by id ";
                //  Debug.LogError(query);
                // If there is a row, clear it.
                if (rows != null)
                    rows.Clear();
                optionslist .Clear();
                // Load data
                rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
                //Debug.Log("#Rows:"+rows.Count);
                // Read all the data

                if ((rows != null) && (rows.Count > 0))
                {
                int optionsId = 0;
                bonusOptions = new string[rows.Count + 1];
                bonusOptions[optionsId] = "~ none ~"; 
                bonusIds = new int[rows.Count + 1];
                bonusIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                   
                }
                foreach (Dictionary<string, string> data in rows)
                    {
                    optionsId++;
                    bonusOptions[optionsId] =  data["name"];
                    bonusIds[optionsId] = int.Parse(data["id"]);
                    //    Debug.LogError(int.Parse(data["id"])+", "+data["name"]+", "+float.Parse(data["cost"])+", "+float.Parse(data["chance"]));
                    optionslist.Add(new BonusOptionData(int.Parse(data["id"]), data["name"], data["code"], data["params"]));
                    }
                }
           //     dataLoaded = true;

          //  }
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


            if (view == VipView.Home)
            {
                pos.x += ImagePack.innerMargin;
                pos.y += ImagePack.innerMargin;
                pos.width -= ImagePack.innerMargin;
                pos.height = ImagePack.fieldHeight;
                DrawHome(pos);
                dataLoaded = false;
            }
            else if (view == VipView.Level)
            {
                base.Draw(pos);

                //  DrawLoaded(pos);
            }
            else if (view == VipView.Settings)
            {
                base.Draw(pos);
            }
            
        }
        
        void DrawHome(Rect pos)
        {
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Vip"));
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Levels")))
            {
                LoadBonusSettings();
                view = VipView.Level;
                linkedTablesLoaded = false;
                showCreate = true;
            }
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Option Settings")))
            {
                view = VipView.Settings;
                LoadBonusSettings();
                linkedTablesLoaded = false;
                showCreate = false;
            }
            pos.y += ImagePack.fieldHeight;
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
         /*   if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Back to Vip")))
            {
                view = VipView.Home;
            }*/
            pos.x -= pos.width;
            pos.width *= 2;

            if (view.Equals(VipView.Level))
            {
                if (displayList.Count <= 0)
                {
                    pos.y += ImagePack.fieldHeight;
                    ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create an Profile before edit it."));
                    return;
                }
                // Draw the content database info
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Vip Level"));
                

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

            if (state != State.Loaded && view.Equals(VipView.Level))
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Level Properties")+":");
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
            if (view.Equals(VipView.Level))
            {
                editingDisplay = new VipData();
                originalDisplay = new VipData();
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
            /*    if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Back to Vip")))
                {
                    view = VipView.Home;
                }*/
                pos.x -= pos.width;
                pos.width *= 2;
                pos.y += 2f * ImagePack.innerMargin;
            }
           /* if (view.Equals(VipView.Level))
            {*/
                if (!linkedTablesLoaded)
                {
                // LoadCurrencyOptions();
                LoadBonusSettings();

                LoadStatOptions();
                    linkedTablesLoaded = true;
                }

                // Draw the content database info
                if (newEntity)
                {
                    ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new Vip Level"));
                    pos.y += ImagePack.fieldHeight;
                }

                editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.8f);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.level = ImagePack.DrawField(pos, Lang.GetTranslate("Level") + ":", editingDisplay.level);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.max_points = ImagePack.DrawField(pos, Lang.GetTranslate("Points to Next Level") + ":", editingDisplay.max_points);
            pos.y += ImagePack.fieldHeight;
                editingDisplay.description = ImagePack.DrawField(pos, Lang.GetTranslate("Description") + ":", editingDisplay.description, 0.75f, 60);
                pos.y += 3*ImagePack.fieldHeight;

             
                    for (int i = 0; i < editingDisplay.bonuses.Count; i++)
                        {
                                pos.width = pos.width / 2;
                                // Generate search string if none exists
                                if (effectSearchInput.Count <= i)
                                {
                                    effectSearchInput.Add("");
                                }
                                string searchString = effectSearchInput[i];
                    //    pos.y += ImagePack.fieldHeight;
                    int selectedBonus = GetOptionPosition(editingDisplay.bonuses[i].BonusType, bonusIds);
                // selectedAbility = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Ability") + ": ", ref searchString, selectedAbility, abilityOptions);

                selectedBonus = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Option Name")+": ", ref searchString, selectedBonus, bonusOptions);
                    editingDisplay.bonuses[i].BonusType = bonusIds[selectedBonus];
                    pos.y += ImagePack.fieldHeight;
                if (bonusOptionsParam[selectedBonus].Contains("v"))
                {
                    editingDisplay.bonuses[i].BonusValue = ImagePack.DrawField(pos, Lang.GetTranslate("Value") + ":", editingDisplay.bonuses[i].BonusValue);
                    pos.x += pos.width;
                }
                if (bonusOptionsParam[selectedBonus].Contains("p"))
                    editingDisplay.bonuses[i].BonusValuePercentage = ImagePack.DrawField(pos, Lang.GetTranslate("Value percentage")+":", editingDisplay.bonuses[i].BonusValuePercentage);
                if (bonusOptionsParam[selectedBonus].Contains("v"))
                    pos.x -= pos.width;

                                // Save back the search string
                                effectSearchInput[i] = searchString;

                                pos.x += pos.width;
                                pos.y += ImagePack.fieldHeight;
                                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove Bonus")))
                                {
                                    editingDisplay.bonuses.RemoveAt(i);
                                }
                                pos.x -= pos.width;
                                pos.y += ImagePack.fieldHeight;
                                pos.width = pos.width * 2;
                            }
                        
                        if (editingDisplay.bonuses.Count < vipOptions.Length)
                        {
                            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Bonus")))
                            {
                             BonusEntry statEntry = new BonusEntry(-1,0, 0, 0);
                                editingDisplay.bonuses.Add(statEntry);
                            }
                        }
                    
                    pos.width = pos.width / 2;
                    pos.x += pos.width;
                    pos.y += ImagePack.fieldHeight;
               

                // Save data		
                pos.x -= ImagePack.innerMargin;
                pos.y += 3f * ImagePack.fieldHeight;
                if (!newEntity)
                {
                    Color temp = GUI.color;
                    GUI.color = Color.red;
                 //   ImagePack.DrawText(pos, "   "+Lang.GetTranslate("Attention, erasing is permanent !!! "));
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

           /* }
            if (view.Equals(VipView.Settings) && !newItem)
            {
                // Draw the content database info
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Vip Option Settings"));
                pos.y += ImagePack.fieldHeight;
                for (int j = 0; j < optionslist.Count; j++)
                {
                    ImagePack.DrawLabel(pos.x, pos.y, itemQualityOptions[j]);
                    pos.y += ImagePack.fieldHeight;
                    pos.width = pos.width / 2;
                    optionslist[j].Name = ImagePack.DrawField(pos, Lang.GetTranslate("name")+":", optionslist[j].Name);
                    pos.x += pos.width;
                    optionslist[j].code = ImagePack.DrawField(pos, Lang.GetTranslate("code")+": ", optionslist[j].code);
                    pos.x -= pos.width;
                    //pos.width = pos.width * 2;
                    pos.y += ImagePack.fieldHeight;
                    
                    bool percentage = optionslist[j].param.Contains("p");
                    bool value = optionslist[j].param.Contains("v");
                    percentage = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("percentage") + "?", percentage);
                    pos.x += pos.width;
                    value = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("value") + "?", value);
                    optionslist[j].param = "";
                    if (percentage)
                        optionslist[j]. param += "p";
                    if (value)
                        optionslist[j]. param += "v";
                    pos.x -= pos.width;
                    pos.width = pos.width * 2;
                    pos.y += ImagePack.fieldHeight;

                    if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove Option")))
                    {
                        optionslist.RemoveAt(j);
                    }
                    pos.x -= pos.width;
                    pos.y += ImagePack.fieldHeight;
                    pos.width = pos.width * 2;
                }

                if (editingDisplay.bonuses.Count < vipOptions.Length)
                {
                    if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Option")))
                    {
                        BonusOptionData statEntry = new BonusOptionData(-1,"", "", "");
                        optionslist.Add(statEntry);
                    }
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

            }*/

        }

        public override void save()
        {
         /*   if (view.Equals(VipView.Settings) && !newEntity)
            {
                UpdateGrade();
            }
            else if (view.Equals(VipView.Level))
            {*/
                if (newEntity)
                    InsertEntry();
                else
                    UpdateEntry();
          //  }
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
            if (rows != null)
                rows.Clear();
            string query = "INSERT INTO " + tableName;
            query += " (name ,level ,description ,max_points ) ";
            query += "VALUES ";
            query += "(?name ,?level ,?description ,?max_points) ";
            int vipID = -1;
            List<Register> update = new List<Register>();
            update.Add(new Register("name", "?name", MySqlDbType.VarChar, editingDisplay.Name.ToString(), Register.TypesOfField.String));
            update.Add(new Register("level", "?level", MySqlDbType.Int32, editingDisplay.level.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("description", "?description", MySqlDbType.VarChar, editingDisplay.description, Register.TypesOfField.String));
            update.Add(new Register("max_points", "?max_points", MySqlDbType.Int32, editingDisplay.max_points.ToString(), Register.TypesOfField.Int));
            vipID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);
            if (vipID != -1)
            {
                // Update online table to avoid access the database again			
                editingDisplay.id = vipID;
              
                editingDisplay.isLoaded = true;

                foreach (BonusEntry entry in editingDisplay.bonuses)
                {
                    if (entry.BonusType>0)
                    {
                        InsertVipOption(entry,vipID);
                    }
                }

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

        void InsertVipOption(BonusEntry entry,int id)
        {
            string query = "INSERT INTO vip_level_bonuses " ;
            query += " (vip_level_id ,bonus_settings_id ,value ,valuep ) ";
            query += "VALUES ";
            query += "("+id+ " ," +entry.BonusType+" ,"+entry.BonusValue + " ,"+ entry.BonusValuePercentage + ") ";
            int vipID = -1;
            List<Register> update = new List<Register>();
        //    update.Add(new Register("vip_level_id", "?vip_level_id", MySqlDbType.Int32, editingDisplay.level.ToString(), Register.TypesOfField.Int));
            //update.Add(new Register("vip_settings_id", "?vip_settings_id", MySqlDbType.Int32, entry.BonusType.ToString(), Register.TypesOfField.Int));
        //    update.Add(new Register("value", "?value", MySqlDbType.Int32, editingDisplay.level.ToString(), Register.TypesOfField.Int));
         //   update.Add(new Register("valuep", "?valuep", MySqlDbType.Float, editingDisplay.level.ToString(), Register.TypesOfField.Float));
            vipID = DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, query);

        //    int itemID = -1;
       //     itemID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);

            entry.id = vipID;
        }



        // Update existing entries in the table based on the iddemo_table
        void UpdateEntry()
        {
            NewResult(Lang.GetTranslate("Updating..."));
            if (rows != null)
                rows.Clear();
            string query = "UPDATE "+ tableName;
            query += " SET ";
            query += " name =?name,level=?level ,description =?description,max_points=?max_points  ";
            query += " WHERE id=?id";
            int vipID = -1;
            List<Register> update = new List<Register>();
            update.Add(new Register("name", "?name", MySqlDbType.VarChar, editingDisplay.Name.ToString(), Register.TypesOfField.String));
            update.Add(new Register("level", "?level", MySqlDbType.Int32, editingDisplay.level.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("description", "?description", MySqlDbType.VarChar, editingDisplay.description, Register.TypesOfField.String));
            update.Add(new Register("max_points", "?max_points", MySqlDbType.Int32, editingDisplay.max_points.ToString(), Register.TypesOfField.Int));
          //  vipID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);
            update.Add(new Register("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString(), Register.TypesOfField.Int));
            // Update the database
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);

            string query2 = "DELETE FROM vip_level_bonuses WHERE vip_level_id="+ editingDisplay.id;

            // Setup the register data		
            List<Register> update1 = new List<Register>();
            update1.Add(new Register("vip_level_id", "?vip_level_id", MySqlDbType.Int32, editingDisplay.id.ToString(), Register.TypesOfField.Int));
            // Update the database
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query2, update1);
            foreach (BonusEntry entry in editingDisplay.bonuses)
            {
                if (entry.BonusType > 0)
                {
                    InsertVipOption(entry, editingDisplay.id);
                }
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





        // Delete entries from the table
        void DeleteEntry()
        {

            string query = "UPDATE " + tableName;
            query += " SET  isactive = 0 ";
            query += " WHERE id=" + editingDisplay.id;
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
            if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete") + " " + Lang.GetTranslate(functionName) + " " + Lang.GetTranslate("entry") + " ?", Lang.GetTranslate("Are you sure you want to delete") + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
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