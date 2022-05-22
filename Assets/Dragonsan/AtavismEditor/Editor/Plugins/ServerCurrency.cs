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
    // Handles the Effects Configuration
    public class ServerCurrency : AtavismDatabaseFunction
    {

        public new Dictionary<int, CurrencyData> dataRegister;
        public new CurrencyData editingDisplay;
        public new CurrencyData originalDisplay;

        // Handles the prefab creation, editing and save
        private CurrencyPrefab prefab = null;

        public static int[] currencyIds = new int[] { -1 };
        public static string[] currencyOptions = new string[] { "~ none ~" };

        public static GUIContent[] GuiCurrencyOptions = new GUIContent[] { new GUIContent("~ none ~") };

        public int[] currencyGroupIds = new int[] { -1 };
        public string[] currencyGroupOptions = new string[] { "~ none ~" };

        // Use this for initialization
        public ServerCurrency()
        {
            functionName = "Currencies";
            // Database tables name
            tableName = "currencies";
            functionTitle = "Currency Configuration";
            loadButtonLabel = "Load Currencies";
            notLoadedText = "No Currencies loaded.";
            // Init
            dataRegister = new Dictionary<int, CurrencyData>();

            editingDisplay = new CurrencyData();
            originalDisplay = new CurrencyData();
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

        public static void LoadCurrencyOptions()
        {
            // Read all entries from the table
            string query = "SELECT id, name FROM currencies where isactive = 1";

            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
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
                    currencyOptions[optionsId] = data["name"];
                    currencyIds[optionsId] = int.Parse(data["id"]);
                }
            }

        }

        public static void LoadCurrencyOptions(bool gui)
        {
            if (!gui)
            {
                LoadCurrencyOptions();
                return;
            }
            // Read all entries from the table
            string query = "SELECT id, name FROM currencies where isactive = 1";

            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                GuiCurrencyOptions = new GUIContent[rows.Count + 1];
                GuiCurrencyOptions[optionsId] = new GUIContent("~ none ~");
                currencyIds = new int[rows.Count + 1];
                currencyIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    GuiCurrencyOptions[optionsId] = new GUIContent(data["name"]);
                    currencyIds[optionsId] = int.Parse(data["id"]);
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

        CurrencyData LoadEntity(int id)
        {

            // Read all entries from the table
            string query = "SELECT " + originalDisplay.GetFieldsString() + " FROM " + tableName + " where isactive = 1 AND id = " + id;

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            CurrencyData display = new CurrencyData();
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
                    display.description = data["description"];
                    display.maximum = long.Parse(data["maximum"]);
                    display.currencyGroup = int.Parse(data["currencyGroup"]);
                    display.currencyPosition = int.Parse(data["currencyPosition"]);
                    display.external = bool.Parse(data["external"]);

                    display.isLoaded = true;
                }
            }
            LoadCurrencyConversions(display);
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


        void LoadCurrencyConversions(CurrencyData currencyData)
        {
            // Read all entries from the table
            string query = "SELECT " + new CurrencyConversionEntry().GetFieldsString() + " FROM currency_conversion where currencyID = "+ currencyData.id + " AND isactive = 1";

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
                    CurrencyConversionEntry display = new CurrencyConversionEntry();
                    display.id = int.Parse(data["id"]);
                    display.currencyToID = int.Parse(data["currencyToID"]);
                    display.amount = int.Parse(data["amount"]);
                    display.autoConverts = bool.Parse(data["autoConverts"]);
                    currencyData.currencyConversion.Add(display);
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create a Currency before editing it."));
                return;
            }

            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Currency"));

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

                pos.width *= 2;
                pos.y += ImagePack.fieldHeight * 1.5f;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Currency Properties:"));
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
            editingDisplay = new CurrencyData();
            originalDisplay = new CurrencyData();
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
                linkedTablesLoaded = true;
                LoadCurrencyOptions();
                 ServerOptionChoices.LoadAtavismChoiceOptions("Currency Group", true, out currencyGroupIds, out currencyGroupOptions);
            }

            // Draw the content database info
            //pos.y += ImagePack.fieldHeight;

            if (newEntity)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new Currency"));
                pos.y += ImagePack.fieldHeight;
            }

            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.75f);
            pos.y += ImagePack.fieldHeight;
            pos.width /= 2;
            editingDisplay.maximum = ImagePack.DrawField(pos, Lang.GetTranslate("Max")+":", editingDisplay.maximum);
            pos.x += pos.width;
            string icon = ImagePack.DrawSpriteAsset(pos, Lang.GetTranslate("Icon") + ":", editingDisplay.icon);
       /*     if (editingDisplay.icon.Length > 3 && icon == " ")
            {
                CurrencyPrefab item = new CurrencyPrefab(editingDisplay);
                editingDisplay.icon = item.LoadIcon();
            }
            else
            {*/
                editingDisplay.icon = icon;
           // }
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;

          //  editingDisplay.currencyGroup = ImagePack.DrawSelector(pos, Lang.GetTranslate("Group")+":", editingDisplay.currencyGroup, currencyGroupOptions);
            int selected = GetOptionPosition(editingDisplay.currencyGroup, currencyGroupIds);
            selected = ImagePack.DrawSelector(pos, Lang.GetTranslate("Group") + ":", selected, currencyGroupOptions);
            editingDisplay.currencyGroup = currencyGroupIds[selected];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.currencyPosition = ImagePack.DrawSelector(pos, Lang.GetTranslate("Position")+":", editingDisplay.currencyPosition, editingDisplay.positionOptions);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.external = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("External")+":", editingDisplay.external);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.description = ImagePack.DrawField(pos, Lang.GetTranslate("Description")+":", editingDisplay.description);

            // Conversion area
            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Conversion Options"));
            pos.y += 1.5f * ImagePack.fieldHeight;
            if (editingDisplay.currencyConversion.Count == 0)
            {
                editingDisplay.currencyConversion.Add(new CurrencyConversionEntry(-1, -1, 0));
            }
            for (int i = 0; i < editingDisplay.currencyConversion.Count; i++)
            {
                editingDisplay.currencyConversion[i].amount = ImagePack.DrawField(pos, Lang.GetTranslate("Amount")+":", editingDisplay.currencyConversion[i].amount);
                pos.x += pos.width;
                int selectedCurrency = GetPositionOfCurrency(editingDisplay.currencyConversion[i].currencyToID);
                selectedCurrency = ImagePack.DrawSelector(pos, Lang.GetTranslate("Converts To 1"), selectedCurrency, currencyOptions);
                editingDisplay.currencyConversion[i].currencyToID = currencyIds[selectedCurrency];
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                editingDisplay.currencyConversion[i].autoConverts = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Auto Converts")+":", editingDisplay.currencyConversion[i].autoConverts);
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove Conversion")))
                {
                    if (editingDisplay.currencyConversion[i].id > 0)
                        editingDisplay.conversionsToBeDeleted.Add(editingDisplay.currencyConversion[i].id);
                    editingDisplay.currencyConversion.RemoveAt(i);
                }
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
            }
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Conversion")))
            {
                editingDisplay.currencyConversion.Add(new CurrencyConversionEntry(-1, -1, 0));
            }

            pos.width *= 2;

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
                // Update online table to avoid access the database again			
                editingDisplay.id = itemID;
                // Insert the Conversions
                foreach (CurrencyConversionEntry entry in editingDisplay.currencyConversion)
                {
                    if (entry.currencyToID != -1)
                    {
                        entry.currencyID = itemID;
                        InsertConversion(entry);
                    }
                }
                editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
                CreatePrefab();
                dataLoaded = false;
                Load();
                newItemCreated = true;
                // Configure the correponding prefab
                LoadCurrencyOptions();
                NewResult(Lang.GetTranslate("New entry inserted"));
            }
            else
            {
                NewResult(Lang.GetTranslate("Error occurred, please check the Console"));
            }
        }

        void InsertConversion(CurrencyConversionEntry entry)
        {
            string query = "INSERT INTO currency_conversion";
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

            // Insert/Update the currency conversions
            foreach (CurrencyConversionEntry entry in editingDisplay.currencyConversion)
            {
                if (entry.currencyToID != -1)
                {
                    if (entry.id < 1)
                    {
                        // This is a new entry, insert it
                        entry.currencyID = editingDisplay.id;
                        InsertConversion(entry);
                    }
                    else
                    {
                        // This is an existing entry, update it
                        entry.currencyID = editingDisplay.id;
                        UpdateConversion(entry);
                    }
                }
            }

            // Delete any conversions that are tagged for deletion
            foreach (int conversionID in editingDisplay.conversionsToBeDeleted)
            {
                DeleteConversion(conversionID);
            }

           // Configure the correponding prefab
            CreatePrefab();
            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry")+"  " + editingDisplay.Name + " "+ Lang.GetTranslate("updated"));
              Load();
      }

        void UpdateConversion(CurrencyConversionEntry entry)
        {
            string query = "UPDATE currency_conversion";
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

        void DeleteConversion(int conversionID)
        {
            string query = "UPDATE currency_conversion SET isactive = 0 where id = " + conversionID;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
        }

        // Delete entries from the table
        void DeleteEntry()
        {

            //Check Arena
            string sqlArena = "SELECT id FROM arena_templates WHERE victoryCurrency =" + editingDisplay.id + " OR defeatCurrency = " + editingDisplay.id;
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlArena);
            int arenaCount = 0;
            string arenaIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                arenaIds = "\n" + Lang.GetTranslate("Arena Ids") + ":";
                arenaCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    arenaIds += " " + data["id"] + ",";
                }
            }

            //Check Quests
            string sqlquests = "SELECT id FROM quests  WHERE currency1 =" + editingDisplay.id + " OR currency2 = " + editingDisplay.id;

            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlquests);
            int questCount = 0;
            string questIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                questIds = "\n" + Lang.GetTranslate("Quests Ids") + ":";
                questCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    questIds += " " + data["id"] + ",";
                }
            }

            DatabasePack.SaveLog("Delete Currency  id =" + editingDisplay.id + " assigned to " + arenaIds + questIds );

            if (arenaCount == 0 && questCount == 0)
            {
                // Remove the prefab
                DeletePrefab();

                //Register delete = new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
                //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, tableName, delete);
                string query = "UPDATE " + tableName + " SET isactive = 0 where id = " + editingDisplay.id;
                DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

                // Delete the item links;
                query = "UPDATE currency_conversion SET isactive = 0 where currencyID = " + editingDisplay.id;
                DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

                // Update online table to avoid access the database again		
                selectedDisplay = -1;
                newSelectedDisplay = 0;
                dataLoaded = false;
              
                NewResult(Lang.GetTranslate("Entry") + " " + editingDisplay.Name + " " + Lang.GetTranslate("Deleted"));
                Load();
                LoadCurrencyOptions();
            }
            else
            {

                EditorUtility.DisplayDialog("", Lang.GetTranslate("You can not delete Instance because it is assigned in") + ":" + arenaIds + questIds , Lang.GetTranslate("OK"), "");


            }
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
                where.Add(new Register("currencyID", "?currencyID", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "currency_conversion", where);

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

            query = "UPDATE currency_conversion SET isactive = 1 where currencyID = " + id;
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
          /*  if (!dataLoaded)
            {
                Load();
            }

            // Delete all existing prefabs?
            CurrencyPrefab.DeleteAllPrefabs();

            foreach (int id  in displayKeys)
            {
                CurrencyData currencyData = LoadEntity(id);
                prefab = new CurrencyPrefab(currencyData, -1, 1);
                foreach (CurrencyConversionEntry conversionEntry in currencyData.currencyConversion)
                {
                    if (conversionEntry.autoConverts)
                    {
                        prefab.convertsTo = conversionEntry.currencyToID;
                        prefab.conversionAmountReq = conversionEntry.amount;
                        break;
                    }
                }
                prefab.Save(currencyData);
            }*/
        }

        void CreatePrefab()
        {
            // Configure the correponding prefab
          /*  prefab = new CurrencyPrefab(editingDisplay, -1, 1);

            foreach (CurrencyConversionEntry conversionEntry in editingDisplay.currencyConversion)
            {
                if (conversionEntry.autoConverts)
                {
                    prefab.convertsTo = conversionEntry.currencyToID;
                    prefab.conversionAmountReq = conversionEntry.amount;
                    break;
                }
            }*/
         //   prefab.Save();
        }

        void DeletePrefab()
        {
         /*   prefab = new CurrencyPrefab(editingDisplay, -1, 1);

            if (prefab.Load())
                prefab.Delete();*/
        }

        public int GetPositionOfCurrency(int currencyID)
        {
            for (int i = 0; i < currencyIds.Length; i++)
            {
                if (currencyIds[i] == currencyID)
                    return i;
            }
            return 0;
        }
    }
}