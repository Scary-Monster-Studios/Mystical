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
    // Handles the Mob Configuration
    public class ServerLootTables : AtavismDatabaseFunction
    {

        public new Dictionary<int, LootTable> dataRegister;
        public new LootTable editingDisplay;
        public new LootTable originalDisplay;

        public int[] itemIds = new int[] { -1 };
        public string[] itemsList = new string[] { "~ none ~" };

        // Use this for initialization
        public ServerLootTables()
        {
            functionName = "Loot Tables";
            // Database tables name
            tableName = "loot_tables";
            functionTitle = "Loot Table Configuration";
            loadButtonLabel = "Load Loot Tables";
            notLoadedText = "No Loot Table loaded.";
            // Init
            dataRegister = new Dictionary<int, LootTable>();

            editingDisplay = new LootTable();
            originalDisplay = new LootTable();
        }
        void resetSearch(bool fokus)
        {
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
        LootTable LoadEntity(int id)
        {
            // Read all entries from the table
            string query = "SELECT * FROM " + tableName + " where isactive = 1 AND id="+id;
            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            LootTable display = new LootTable();

            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    //foreach(string key in data.Keys)
                    //	Debug.Log("Name[" + key + "]:" + data[key]);
                    //return;
                    display.id = int.Parse(data["id"]);
                    display.Name = data["name"];
                 /*   for (int i = 1; i <= display.maxEntries; i++)
                    {
                        int itemId = int.Parse(data["item" + i]);
                        if (itemId != -1)
                        {
                            float chance = float.Parse(data["item" + i + "chance"]);
                            int count = int.Parse(data["item" + i + "count"]);
                            LootTableEntry entry = new LootTableEntry(itemId, chance, count);
                            display.entries.Add(entry);
                        }
                    }*/
                    display.isLoaded = true;
                }
            }
            LoadLootItems(display);
            return display;
        }

        void LoadLootItems(LootTable lootdata)
        {
            // Read all entries from the table
            string query = "SELECT * FROM loot_table_items where loot_table_id = " + lootdata.id;

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
                    LootTableEntry entry = new LootTableEntry();
                    entry.id = int.Parse(data["id"]);
                   // entry.questID = int.Parse(data["quest_id"]);
                    entry.itemId = int.Parse(data["item"]);
                    entry.count = int.Parse(data["count"]);
                    entry.countMax = int.Parse(data["count_max"]);
                    entry.chance = float.Parse(data["chance"]);
                    lootdata.entries.Add(entry);
                 
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create a Loot Table before editing it."));
                return;
            }


            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Loot Table"));
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Loot Table Properties:"));
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
            editingDisplay = new LootTable();
            originalDisplay = new LootTable();
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new loot table"));
                pos.y += ImagePack.fieldHeight;
            }

            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.5f);
            pos.y += ImagePack.fieldHeight;

            if (editingDisplay.entries.Count == 0)
            {
                editingDisplay.entries.Add(new LootTableEntry(-1, 100, 1));
            }
            for (int i = 0; i < editingDisplay.maxEntries; i++)
            {
                if (editingDisplay.entries.Count > i)
                {
                    pos.width = pos.width / 2;
                    int selectedItem = GetPositionOfItem(editingDisplay.entries[i].itemId);
                    selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item")+" " + (i + 1) + ":", ref editingDisplay.entries[i].itemSearch, selectedItem, itemsList);
                    editingDisplay.entries[i].itemId = itemIds[selectedItem];
                    //pos.x += pos.width;
                    pos.y += ImagePack.fieldHeight;
                    editingDisplay.entries[i].count = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.entries[i].count);
                    pos.x += pos.width;
                    editingDisplay.entries[i].countMax = ImagePack.DrawField(pos, Lang.GetTranslate("Count Max") + ":", editingDisplay.entries[i].countMax);
                    pos.x -= pos.width;
                    pos.y += ImagePack.fieldHeight;
                    editingDisplay.entries[i].chance = ImagePack.DrawField(pos, Lang.GetTranslate("Chance")+":", editingDisplay.entries[i].chance);
                    pos.y += ImagePack.fieldHeight;
                    pos.x += pos.width;
                    if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove Item")))
                    {
                        if (editingDisplay.entries[i].id > 0)
                            editingDisplay.entriesToBeDeleted.Add(editingDisplay.entries[i].id);
                        editingDisplay.entries.RemoveAt(i);
                    }
                    pos.x -= pos.width;
                    pos.y += ImagePack.fieldHeight;
                    pos.width = pos.width * 2;
                }
            }
            if (editingDisplay.entries.Count < editingDisplay.maxEntries)
            {
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Item")))
                {
                    LootTableEntry lootTableEntry = new LootTableEntry(-1, 0, 1);
                    editingDisplay.entries.Add(lootTableEntry);
                }
            }
            //pos.width = pos.width * 2;

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

            string sqlmob = "SELECT id FROM " + tableName + "  WHERE name ='" + editingDisplay.Name + "'";
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlmob);

            if ((rows != null) && (rows.Count > 0))
            {
                NewResult("Error:" + Lang.GetTranslate("Can not be saved because the name already exists"));
                return;
            }


            // Setup the update query
            string query = "INSERT INTO " + tableName;
            query += " (" + editingDisplay.FieldList("", ", ") + ") ";
            query += "VALUES ";
            query += " (" + editingDisplay.FieldList("?", ", ") + ") ";

            int lootId = -1;

            // Setup the register data		
            List<Register> update = new List<Register>();
            foreach (string field in editingDisplay.fields.Keys)
            {
                update.Add(editingDisplay.fieldToRegister(field));
            }

            // Update the database
            lootId = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);

            // If the insert failed, don't insert the spawn marker
            if (lootId != -1)
            {
                foreach (LootTableEntry entry in editingDisplay.entries)
                {
                    if (entry.itemId > 0 && entry.count > 0)
                    {
                      //  entry.questID = itemID;
                        InsertItems(entry, lootId);
                    }
                }

                // Update online table to avoid access the database again			
                editingDisplay.id = lootId;
                editingDisplay.isLoaded = true;
                dataLoaded = false;
                Load();
                newItemCreated = true;
                NewResult(Lang.GetTranslate("New entry inserted"));
            }
            else
            {
                NewResult("Error:" + Lang.GetTranslate("Error occurred, please check the Console"));
            }
        }

        void InsertItems(LootTableEntry entry, int  lootId)
        {
            string query = "INSERT INTO loot_table_items";
            query += " (loot_table_id, item, count, chance, count_max) ";
            query += "VALUES ";
            query += " ( ?loot_table_id, ?item, ?count, ?chance, ?count_max) ";

            // Setup the register data		
            List<Register> update = new List<Register>();

            update.Add(new Register("loot_table_id", "?loot_table_id", MySqlDbType.Int32, lootId.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("item", "?item", MySqlDbType.Int32, entry.itemId.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("count", "?count", MySqlDbType.Int32, entry.count.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("chance", "?chance", MySqlDbType.Float, entry.chance.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("count_max", "?count_max", MySqlDbType.Float, entry.count.ToString(), Register.TypesOfField.Float));

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

            // Insert/Update the items
            foreach (LootTableEntry entry in editingDisplay.entries)
            {
                //   if (entry.target != -1 || entry.targetsList.Count > 0)
                //   {
                if (entry.id < 1)
                {
                    // This is a new entry, insert it
                    InsertItems(entry, editingDisplay.id);
                }
                else
                {
                    // This is an existing entry, update it
                    UpdateItems(entry);
                }
                //  }
            }
            // And now delete any Item that are tagged for deletion
            foreach (int itemID in editingDisplay.entriesToBeDeleted)
            {
                DeleteItem(itemID);
            }

            // Update online table to avoid access the database again			
            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " "+ Lang.GetTranslate("updated"));
             Load();
       }

        // Delete entries from the table
        void DeleteEntry()
        {
            //Check Mobs
            string sqlMob = "SELECT mobTemplate FROM mob_loot WHERE lootTable  =" + editingDisplay.id + " AND isactive = 1";
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlMob);
            int mobCount = 0;
            string mobIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                mobIds = "\n" + Lang.GetTranslate("Mobs Ids") + ":";
                mobCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    mobIds += " " + data["mobTemplate"] + ",";
                }
            }
            //Check Loot Table
            string sqlEffects = "SELECT id FROM `effects` WHERE effectMainType  like 'CreateItemFromLoot' AND (";
            for (int ii = 1; ii <= 5; ii++)
            {
                sqlEffects += " intValue" + ii + " = '" + editingDisplay.id + "' ";
                if (ii < 5) sqlEffects += " OR ";
            }
            sqlEffects += ") AND isactive =1 ";
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlEffects);
            int effectsCount = 0;
            string effectsIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                effectsIds = "\n" + Lang.GetTranslate("Effects Ids") + ":";
                effectsCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    effectsIds += " " + data["id"] + ",";
                }
            }

            DatabasePack.SaveLog("Delete Item id =" + editingDisplay.id + " assigned to " + mobIds );

            if ( mobCount == 0 )
            {
                //Delete not active effect
                sqlEffects = "Delete From `effects` WHERE effectMainType  like 'CreateItemFromLoot' AND (";
                for (int ii = 1; ii <= 5; ii++)
                {
                    sqlEffects += " intValue" + ii + " = '" + editingDisplay.id + "' ";
                    if (ii < 5) sqlEffects += " OR ";
                }
                sqlEffects += ") AND isactive = 0 ";
                DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, sqlEffects);
             
                //Delete mob_loot from not active
                List <Register> delete = new List<Register>();
                delete.Add(new Register("lootTable", "?lootTable", MySqlDbType.Int32, editingDisplay.id.ToString(), Register.TypesOfField.Int));
                delete.Add(new Register("isactive", "?isactive", MySqlDbType.Int32, "0", Register.TypesOfField.Int));
                DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "mob_loot", delete);

                string query = "UPDATE " + tableName + " SET isactive = 0 where id = " + editingDisplay.id;
                DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            // Update online table to avoid access the database again		
            selectedDisplay = -1;
            newSelectedDisplay = 0;
            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " " + Lang.GetTranslate("Deleted"));
               Load();
         }
            else
            {
                EditorUtility.DisplayDialog("", Lang.GetTranslate("You can not delete Item") + " " + editingDisplay.Name + " " + Lang.GetTranslate("because it is assigned in") + ":"  + mobIds , Lang.GetTranslate("OK"), "");
            }
        }

        void UpdateItems(LootTableEntry entry)
        {
            string query = "UPDATE loot_table_items";
            query += " SET ";
            query += " item=?item, count=?count, chance=?chance, count_max=?count_max ";
            query += " WHERE id=?id";

            // Setup the register data		
            List<Register> update = new List<Register>();
            update.Add(new Register("id", "?id", MySqlDbType.Int32, entry.id.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("item", "?item", MySqlDbType.Int32, entry.itemId.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("count", "?count", MySqlDbType.Int32, entry.count.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("count_max", "?count_max", MySqlDbType.Int32, entry.countMax.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("chance", "?chance", MySqlDbType.Float, entry.chance.ToString(), Register.TypesOfField.Float));

            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);
        }

        void DeleteItem(int itemId)
        {
            List<Register> where = new List<Register>();
            where.Add(new Register("id", "?id", MySqlDbType.Int32, itemId.ToString(), Register.TypesOfField.Int));
            DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "loot_table_items", where);
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

    }
}