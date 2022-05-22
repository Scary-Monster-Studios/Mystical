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
    public class ServerItemSets : AtavismDatabaseFunction
    {

        public new Dictionary<int, SetData> dataRegister;
        public new SetData editingDisplay;
        public new SetData originalDisplay;
        public int[] itemIds = new int[] { -1 };
        public string[] itemsList = new string[] { "~ none ~" };


        public string[] damageOptions = new string[] { "~ none ~" };

        public string[] itemEffectTypeOptions = new string[] { "~ none ~" };
        public string[] statOptions = new string[] { "~ none ~" };
        public string[] accountEffectTypeOptions = new string[] { "~ none ~" };



        // Handles the prefab creation, editing and save
        private ItemSetPrefab item = null;
      
        // Filter/Search inputs
        // private string currencySearchInput = "";
        //  private string enchantSearchInput = "";
        private List<string> effectSearchInput = new List<string>();
        private List<string> itemSearchInput = new List<string>();
        // Use this for initialization
        public ServerItemSets()
        {
            functionName = "Item Sets";
            // Database tables name
            tableName = "item_set_profile";
            functionTitle = "Item Sets Configuration";
            loadButtonLabel = "Load Item Sets";
            notLoadedText = "No Item Sets loaded.";
            // Init
            dataRegister = new Dictionary<int, SetData>();

            editingDisplay = new SetData();
            originalDisplay = new SetData();
       }

        public override void Activate()
        {
            linkedTablesLoaded = false;
            resetSearch(true);
        }

        void resetSearch(bool fokus)
        {
            effectSearchInput.Clear();
            itemSearchInput.Clear();
           if(fokus) GUI.FocusControl(null);
        }
         

        public void LoadStatOptions()
        {  // Read all entries from the table
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
        private void LoadItemList()
        {
            string query = "SELECT id, name FROM item_templates where isactive = 1 AND (itemType like 'Weapon' OR itemType like 'Armor')";

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

        SetData LoadEntity(int id)
        {
            // Read all entries from the table
            string query = "SELECT id,name FROM " + tableName + " WHERE isactive = 1 AND id=" + id;
            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            SetData display = new SetData();
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    //foreach(string key in data.Keys)
                    //	Debug.Log("Name[" + key + "]:" + data[key]);
                    //return;
                    display.id = int.Parse(data["id"]);
                    display.Name = data["name"];
                    display.isLoaded = true;
                }
            }
             LoadItemSetItems(display);
            LoadItemSetLevels(display);
            return display;
        }

        void LoadItemSetItems(SetData setData)
        {
            // Read all entries from the table
            string query = "SELECT * FROM item_set_items where isactive = 1 AND set_id = " + setData.id + " ";

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
                    setData.itemList.Add(int.Parse(data["template_id"]));
                }
            }
        }

        void LoadItemSetLevels(SetData setData)
        {

            // Read all entries from the table
            string query = "SELECT * FROM item_set_level where set_id = " + setData.id;

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
                    SetLevelData display = new SetLevelData();
                    display.id = int.Parse(data["id"]);
                    display.set_id = int.Parse(data["set_id"]);
                    display.number_of_parts = int.Parse(data["number_of_parts"]);
                    display.damage = int.Parse(data["damage"]);
                    display.damagep = int.Parse(data["damagep"]);
                    for (int i = 1; i <= display.maxStatsEntries; i++)
                    {
                        string effectName = data["effect" + i + "name"];
                        if (!string.IsNullOrEmpty(effectName))
                        {
                            int effectValue = int.Parse(data["effect" + i + "value"]);
                            int effectValuep = int.Parse(data["effect" + i + "valuep"]);
                            StatEntry entry = new StatEntry(effectName, effectValue, effectValuep);
                            display.stats.Add(entry);
                        }
                    }
                    display.isLoaded = true;
                    setData.levelList.Add(display);
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
              /*  if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Generate Prefabs")))
                {
                    GenerateAllPrefabs();
                }*/
                pos.x += pos.width;
                /*	if (ImagePack.DrawButton (pos.x, pos.y, "Duplicate")) {
                        Duplicate();
                    }*/
                pos.x -= pos.width;

                pos.width *= 2;
                pos.y += ImagePack.fieldHeight * 1.5f;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Item Set Properties")+":");
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
            editingDisplay = new SetData();
            originalDisplay = new SetData();
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

                LoadStatOptions();
                LoadItemList();
                linkedTablesLoaded = true;
                itemSearchInput = new List<string>();
            }

            // Draw the content database info
            if (newEntity)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new item Set"));
                pos.y += ImagePack.fieldHeight;
            }

            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.8f);
            pos.y += ImagePack.fieldHeight;
            pos.width /= 2;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Set Items")+":");
            pos.y += ImagePack.fieldHeight;
            pos.width *= 2;
            for (int j = 0; j < editingDisplay.itemList.Count; j++)
            {
                if (itemSearchInput.Count <= j)
                {
                    itemSearchInput.Add("");
                }
                string searchString = itemSearchInput[j];
                int selectedItem = GetOptionPosition(editingDisplay.itemList[j], itemIds);
                pos.width /= 2;
                selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item")+" " + (j + 1) + ":", ref searchString, selectedItem, itemsList);
                pos.width *= 2;
                editingDisplay.itemList[j] = itemIds[selectedItem];
                itemSearchInput[j] = searchString;
                pos.width = pos.width / 2;
                pos.x += pos.width;
                pos.y += ImagePack.fieldHeight;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove Item")+" " + (j + 1)))
                {
                    editingDisplay.itemListDeleted.Add(editingDisplay.itemList[j]);
                    editingDisplay.itemList.RemoveAt(j);

                }
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                pos.width = pos.width * 2;
            }
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Item")))
            {
                //StatEntry statEntry = new StatEntry("", 0, 0);
                editingDisplay.itemList.Add(-1);
            }
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Set levels"));
            pos.y += ImagePack.fieldHeight;
            for (int j = 0; j < editingDisplay.levelList.Count; j++)
            {
                ImagePack.DrawLabel(pos.x, pos.y, "#" + (j + 1));
                pos.y += ImagePack.fieldHeight;
                editingDisplay.levelList[j].set_id = editingDisplay.id;
                int from = 1;
                if (j > 0)
                    from = editingDisplay.levelList[j - 1].number_of_parts + 1;
                if (editingDisplay.levelList[j].number_of_parts < from)
                    editingDisplay.levelList[j].number_of_parts = from;
                editingDisplay.levelList[j].number_of_parts = ImagePack.DrawField(pos, Lang.GetTranslate("Number of parts")+":", editingDisplay.levelList[j].number_of_parts);
                pos.y += 1f * ImagePack.fieldHeight;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Stats")+":");
                pos.y += ImagePack.fieldHeight;
                pos.width = pos.width / 2;
                editingDisplay.levelList[j].damage = ImagePack.DrawField(pos, Lang.GetTranslate("Damage Value")+":", editingDisplay.levelList[j].damage);
                pos.x += pos.width;
                editingDisplay.levelList[j].damagep = ImagePack.DrawField(pos, Lang.GetTranslate("Damage Value")+" %: ", editingDisplay.levelList[j].damagep);
                pos.x -= pos.width;
                pos.width = pos.width * 2;
                pos.y += ImagePack.fieldHeight;

                for (int i = 0; i < editingDisplay.levelList[j].maxStatsEntries; i++)
                {
                    if (editingDisplay.levelList[j].stats.Count > i)
                    {
                        pos.width = pos.width / 2;
                        // Generate search string if none exists
                        if (effectSearchInput.Count <= i)
                        {
                            effectSearchInput.Add("");
                        }
                        string searchString = effectSearchInput[i];
                        //    pos.y += ImagePack.fieldHeight;
                        editingDisplay.levelList[j].stats[i].itemStatName = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Stat Name")+": ", ref searchString, editingDisplay.levelList[j].stats[i].itemStatName, statOptions);
                        pos.y += ImagePack.fieldHeight;
                        editingDisplay.levelList[j].stats[i].itemStatValue = ImagePack.DrawField(pos, Lang.GetTranslate("Value")+":", editingDisplay.levelList[j].stats[i].itemStatValue);
                        pos.x += pos.width;
                        editingDisplay.levelList[j].stats[i].itemStatValuePercentage = ImagePack.DrawField(pos, Lang.GetTranslate("Value")+" %: ", editingDisplay.levelList[j].stats[i].itemStatValuePercentage);
                        pos.x -= pos.width;

                        // Save back the search string
                        effectSearchInput[i] = searchString;

                        pos.x += pos.width;
                        pos.y += ImagePack.fieldHeight;
                        if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove stat")))
                        {
                            editingDisplay.levelList[j].stats.RemoveAt(i);
                        }
                        pos.x -= pos.width;
                        pos.y += ImagePack.fieldHeight;
                        pos.width = pos.width * 2;
                    }
                }
                if (editingDisplay.levelList[j].stats.Count < editingDisplay.levelList[j].maxStatsEntries)
                {
                    if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add stat")))
                    {
                        StatEntry statEntry = new StatEntry("", 0, 0);
                        editingDisplay.levelList[j].stats.Add(statEntry);
                    }
                }

                pos.width = pos.width / 2;
                pos.x += pos.width;
                pos.y += ImagePack.fieldHeight;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove Set level")+" #" + (j + 1)))
                {
                    editingDisplay.levelList.RemoveAt(j);
                }
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                pos.width = pos.width * 2;

            }
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Set level")))
            {
                SetLevelData setLevel = new SetLevelData();
                editingDisplay.levelList.Add(setLevel);
            }


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
            query += " (name) ";
            query += "VALUES ";
            query += " (?name) ";

            int itemID = -1;

            // Setup the register data		
            List<Register> update = new List<Register>();
            update.Add(new Register("name", "?name", MySqlDbType.VarChar, editingDisplay.Name, Register.TypesOfField.String));

            // Update the database
            itemID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);

            // If the insert failed, don't insert the spawn marker
            if (itemID != -1)
            {
                // Update online table to avoid access the database again			
                editingDisplay.id = itemID;
                // Insert the Requirements
                foreach (SetLevelData entry in editingDisplay.levelList)
                {
                    if (entry.id != -1)
                    {
                        entry.set_id = itemID;
                        InsertLevel(entry);
                    }
                    else
                    {
                        UpdateLevel(entry);
                    }
                }
                InsertItems(editingDisplay.itemList, itemID);

                editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
                CreatePrefab();
                dataLoaded = false;
                Load();
                newItemCreated = true;
                // Configure the correponding prefab
                NewResult(Lang.GetTranslate("New entry inserted"));
            }
            else
            {
                NewResult("Error:" + Lang.GetTranslate("Error occurred, please check the Console"));
            }
        }
        void deleteSetFromItems(List<int> itemList, int setid)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
           //     ItemPrefab.UnassignItemSet(itemList[i], setid);
            }
        }

        void InsertItems(List<int> itemList, int setid)
        {
            string query2 = "DELETE FROM item_set_items WHERE set_id=?set_id";
            // Setup the register data		
            List<Register> update1 = new List<Register>();
            update1.Add(new Register("set_id", "?set_id", MySqlDbType.Int32, setid.ToString(), Register.TypesOfField.Int));
            // Update the database
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query2, update1);

            if (itemList.Count > 0)
            {
                string query = "INSERT INTO item_set_items";
                query += " (`template_id`, `set_id`) ";
                query += " VALUES ";
                for (int i = 0; i < itemList.Count - 1; i++)
                {
                    query += " (" + itemList[i] + ", " + setid + "), ";
                }
                query += " (" + itemList[itemList.Count - 1] + ", " + setid + ") ";

                // Setup the register data		
                List<Register> update = new List<Register>();
               
                DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);
                for (int i = 0; i < itemList.Count; i++)
                {
               //     ItemPrefab.AssignItemSet(itemList[i], setid);
                }
            }
        }
        void InsertLevel(SetLevelData entry)
        {
            string query = "INSERT INTO item_set_level ";
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

        void UpdateLevel(SetLevelData entry)
        {
            string query = "UPDATE item_set_level";
            query += " SET ";
            query += entry.UpdateList();
            query += " WHERE id=?id";
            // Debug.LogError(query);
            // Setup the register data		
            List<Register> update = new List<Register>();
            foreach (string field in entry.fields.Keys)
            {
                update.Add(entry.fieldToRegister(field));
            }
            update.Add(new Register("id", "?id", MySqlDbType.Int32, entry.id.ToString(), Register.TypesOfField.Int));

            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);
        }


        // Update existing entries in the table based on the iddemo_table
        void UpdateEntry()
        {
            NewResult("Updating...");
            // Setup the update query
            string query = "UPDATE " + tableName;
            query += " SET ";
            query += " name=?name";

            query += " WHERE id=?id";

            // Setup the register data		
            List<Register> update = new List<Register>();

            update.Add(new Register("name", "?name", MySqlDbType.VarChar, editingDisplay.Name, Register.TypesOfField.String));
            update.Add(new Register("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString(), Register.TypesOfField.Int));

            // Update the database
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);

            // Insert/Update the requirements
            foreach (SetLevelData entry in editingDisplay.levelList)
            {
                if (entry.id == -1)
                {
                    entry.set_id = editingDisplay.id;
                    InsertLevel(entry);
                }
                else
                {
                    UpdateLevel(entry);
                }
            }
            InsertItems(editingDisplay.itemList, editingDisplay.id);
            deleteSetFromItems(editingDisplay.itemListDeleted, editingDisplay.id);
         
            // Remove the prefab
            DeletePrefab();
            // Configure the correponding prefab
            CreatePrefab();
            dataLoaded = false;
             NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " " + Lang.GetTranslate("updated"));
              Load();
     }

        // Delete entries from the table
        void DeleteEntry()
        {
            string query2 = "UPDATE  item_set_items SET isactive = 0 WHERE set_id=?set_id";
            // Setup the register data		
            List<Register> update1 = new List<Register>();
            update1.Add(new Register("set_id", "?set_id", MySqlDbType.Int32, editingDisplay.id.ToString(), Register.TypesOfField.Int));
            // Update the database
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query2, update1);

            for (int i = 0; i < editingDisplay.itemList.Count; i++)
            {
             //   ItemPrefab.AssignItemSet(editingDisplay.itemList[i], 0);
            }
            // Remove the prefab
            DeletePrefab();
            // Delete the database entry
            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, tableName, delete);
            string query = "UPDATE " + tableName + " SET isactive = 0 where id = " + editingDisplay.id;
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
                where.Add(new Register("set_id", "?set_id", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "item_set_level", where);
                where.Clear();
                where.Add(new Register("set_id", "?set_id", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "item_set_items", where);

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
            string query2 = "UPDATE  item_set_items SET isactive = 1 WHERE set_id=?set_id";
            // Setup the register data		
            List<Register> update1 = new List<Register>();
            update1.Add(new Register("set_id", "?set_id", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
            // Update the database
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query2, update1);

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

            //// Delete all existing prefabs?
            //ItemSetPrefab.DeleteAllPrefabs();

            //foreach (SetData itemData in dataRegister.Values)
            //{
            //    item = new ItemSetPrefab(itemData);
            //    item.Save(itemData);
            //}
            foreach (int id in displayKeys)
            {
                SetData itemData = LoadEntity(id);
                item = new ItemSetPrefab(itemData);
                item.Save(itemData);
            }
            ItemSetPrefab.DeletePrefabWithoutIDs(displayKeys);*/
        }

        void CreatePrefab()
        {
            // Configure the correponding prefab
         /*   item = new ItemSetPrefab(editingDisplay);
            item.Save(editingDisplay);*/
        }

        void DeletePrefab()
        {
          /*  item = new ItemSetPrefab(editingDisplay);

            if (item.Load())
                item.Delete();*/
        }

    }
}