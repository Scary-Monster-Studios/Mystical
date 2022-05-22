using UnityEngine;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using UnityEditor;

namespace Atavism
{
    // Handles the Mob Configuration
    public class ServerOptionChoices : AtavismDatabaseFunction
    {

        //  public Dictionary<int, AtavismEditorOption> dataRegister;
        public new AtavismEditorOption editingDisplay;
        public new AtavismEditorOption originalDisplay;
       
        // Use this for initialization
        public ServerOptionChoices()
        {
            functionName = "Option Choices";
            // Database tables name
            tableName = "editor_option";
            functionTitle = "Option Type Configuration";
            loadButtonLabel = "Load Option Choices";
            notLoadedText = "No Option Type loaded.";
            // Init
         //   dataRegister = new Dictionary<int, AtavismEditorOption>();

            editingDisplay = new AtavismEditorOption();
            originalDisplay = new AtavismEditorOption();
        }

        public override void Activate()
        {
            linkedTablesLoaded = false;
            //   showRecovery = false;
            resetSearch(true);
        }
        void resetSearch(bool fokus)
        {
           if(fokus) GUI.FocusControl(null);
        }

        public override void Load()
        {
            if (!dataLoaded)
            {
                // Clean old data
                displayKeys.Clear();

                // Read all entries from the table
                string query = "SELECT id,optionType FROM " + tableName + " where isactive = 1";

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
                        displayList.Add(data["id"] + ". " + data["optionType"]);
                        displayKeys.Add(int.Parse(data["id"]));
                    }
                }
                dataLoaded = true;
            }
        }

        // Load Database Data
        AtavismEditorOption LoadEntity(int id,bool restore)
        {
                // Read all entries from the table
                string query = "SELECT " + originalDisplay.GetFieldsString() + " FROM " + tableName + " where id =" + id + " AND isactive = 1";

                // If there is a row, clear it.
                if (rows != null)
                    rows.Clear();

                // Load data
                rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
                //Debug.Log("#Rows:"+rows.Count);
                // Read all the data
                AtavismEditorOption display = new AtavismEditorOption();
                if ((rows != null) && (rows.Count > 0))
                {
                    foreach (Dictionary<string, string> data in rows)
                    {
                        //foreach(string key in data.Keys)
                        //	Debug.Log("Name[" + key + "]:" + data[key]);
                        //return;
                        display.id = int.Parse(data["id"]);
                        display.Name = data["optionType"];
                        display.deletable = bool.Parse(data["deletable"]);
                        display.isLoaded = true;
                    }
                    LoadOptionChoices(display,restore);
                }
                return display;
        }
        public override void LoadRestore()
        {
            if (!dataRestoreLoaded)
            {
                // Clean old data
            //    dataRegister.Clear();
                displayKeys.Clear();

                // Read all entries from the table
                string query = "SELECT id,optionType FROM " + tableName + " where isactive = 0";

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
                        displayList.Add(data["id"] + ". " + data["optionType"]);
                        displayKeys.Add(int.Parse(data["id"]));

                    }
                }
                dataRestoreLoaded = true;
            }
        }
        void LoadOptionChoices(AtavismEditorOption optionData,bool restore)
        {
            // Read all entries from the table
            string query = "SELECT " + new AtavismEditorOptionChoice().GetFieldsString() + " FROM "
                    + "editor_option_choice" + " where optionTypeID = " + optionData.id + " AND isactive = ";
            if (restore)
                query += "0";
            else
                query += "1";
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
                    AtavismEditorOptionChoice entry = new AtavismEditorOptionChoice();

                    entry.id = int.Parse(data["id"]);
                    entry.choice = data["choice"];
                    optionData.optionChoices.Add(entry);
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create an Option Type before edit it."));
                return;
            }

            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Option Type"));

            if (newItemCreated)
            {
                newItemCreated = false;
          //      LoadSelectList();
                newSelectedDisplay = displayKeys.Count - 1;
            }

            // Draw data Editor
            if (newSelectedDisplay != selectedDisplay || restore)
            {
                restore = false;
                selectedDisplay = newSelectedDisplay;
                int displayKey = -1;
                if (selectedDisplay >= 0)
                    displayKey = displayKeys[selectedDisplay];
                editingDisplay = LoadEntity(displayKey,false);
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Option Type Properties")+":");
                pos.y += ImagePack.fieldHeight * 0.75f;
            }

            DrawEditor(pos, false);

            pos.y -= ImagePack.fieldHeight;
            //pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            restoreOptions = false;
        }

        bool restoreOptions = false;
        bool loadRestoreOptions= false;
        bool restore = false;
        public override void DrawRestore(Rect box)
        {
            restore = true;
            // Setup the layout
            Rect pos = box;
            pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;

           

       
            Color color = Color.white;
            if (restoreOptions)
            {
                color = Color.green;
            }
          //  pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos, Lang.GetTranslate("Restore Options"),color))
            {
                restoreOptions = !restoreOptions;

            }
                if (restoreOptions)
            {
                if (!loadRestoreOptions)
                {
                    Load();
                    loadRestoreOptions = true;
                    selectedDisplay = -1;
                }
            }
            else
            {
                if (loadRestoreOptions)
                {
                    loadRestoreOptions = false;
                    dataRestoreLoaded = false;
                    LoadRestore();
                    selectedDisplay = -1;
                }
            }
            pos.y += ImagePack.fieldHeight;
            if (!restoreOptions)
            {
                // Draw the content database info
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Restore Option Type"));
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
            }
            else
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Restore Option Choices"));
                pos.y += ImagePack.fieldHeight;
                pos.x += ImagePack.innerMargin;
                pos.width /= 2;
                if (newSelectedDisplay != selectedDisplay)
                {
                    selectedDisplay = newSelectedDisplay;
                    int displayKey = -1;
                    if (selectedDisplay >= 0)
                        displayKey = displayKeys[selectedDisplay];
                    editingDisplay = LoadEntity(displayKey,true);
                }

                //Draw super magical compound object.
                newSelectedDisplay = ImagePack.DrawDynamicPartialListSelector(pos, Lang.GetTranslate("Search Filter")+": ", ref entryFilterInput, selectedDisplay, displayList);

                pos.width *= 2;
                pos.y += ImagePack.fieldHeight * 1.5f;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Option Type Properties to restore")+":");
                pos.y += ImagePack.fieldHeight * 0.75f;
                if (editingDisplay.optionChoices.Count == 0)
                {
                    pos.y += ImagePack.fieldHeight;
                    ImagePack.DrawText(pos, Lang.GetTranslate("You dont have deleted Choice Options for this Type."),Color.red);

                }
                 pos.width -= 140 + 155;
                   for (int i = 0; i < editingDisplay.optionChoices.Count; i++)
                {
                    ImagePack.DrawText(pos, Lang.GetTranslate("Choice")+": " + editingDisplay.optionChoices[i].id+". "+ editingDisplay.optionChoices[i].choice);
                    pos.x += pos.width;
                    if (ImagePack.DrawButton(pos, Lang.GetTranslate("Delete Permanently")))
                    {
                        DeleteForeverChoice(editingDisplay.optionChoices[i].id);
                    }
                    pos.x += 155;
                    if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Restore Choice")))
                    {
                            RestoreEntryChoice(editingDisplay.optionChoices[i].id);
                    }
                    pos.x -= pos.width + 155;
                    
                    pos.y += ImagePack.fieldHeight;
                }
                pos.width += 140+ 155;

            }
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
            editingDisplay = new AtavismEditorOption();
            originalDisplay = new AtavismEditorOption();
            selectedDisplay = -1;
        }
        // Edit or Create
        public override void DrawEditor(Rect box, bool newItem)
        {
            newEntity = newItem;
            if (!linkedTablesLoaded)
            {
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new Option Type"));
                pos.y += ImagePack.fieldHeight;
            }

            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Option Type")+":", editingDisplay.Name, 0.5f);
            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Option Choices"));
            pos.y += ImagePack.fieldHeight;

            if (editingDisplay.optionChoices.Count == 0)
            {
                editingDisplay.optionChoices.Add(new AtavismEditorOptionChoice(0, -1, ""));
            }
            for (int i = 0; i < editingDisplay.optionChoices.Count; i++)
            {
                pos.width /= 1.5f;
                editingDisplay.optionChoices[i].choice = ImagePack.DrawField(pos, Lang.GetTranslate("Choice")+":", editingDisplay.optionChoices[i].choice);
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Choice")))
                {
                    if (editingDisplay.optionChoices[i].id > 0)
                        editingDisplay.choicesToBeDeleted.Add(editingDisplay.optionChoices[i].id);
                    editingDisplay.optionChoices.RemoveAt(i);
                }
                pos.x -= pos.width;
                pos.width *= 1.5f;
                pos.y += ImagePack.fieldHeight;
            }
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Choice")))
            {
                editingDisplay.optionChoices.Add(new AtavismEditorOptionChoice(0, -1, ""));
            }

            //pos.width = pos.width * 2;
         
   showSave = true;

            if (!newEntity && editingDisplay.deletable)
            {
                showDelete = true;
            }
            else
            {
                showDelete = false;
            }

            showCancel = true;
            // Cancel editing

            if (resultTimeout != -1 && resultTimeout < Time.realtimeSinceStartup && result != "")
            {
                result = "";
            }
            //EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight + 100);

                if (!newEntity)
                    EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight+ 110);
                else
                    EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight+ 10);
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
                editingDisplay = LoadEntity(displayKeys[selectedDisplay],false);
            else
                editingDisplay = LoadEntity(selectedDisplay,false);
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

            // If the insert failed, don't insert the choices
            if (itemID != -1)
            {
                // Update online table to avoid access the database again			
                editingDisplay.id = itemID;
                // Insert the choices
                foreach (AtavismEditorOptionChoice entry in editingDisplay.optionChoices)
                {
                    if (entry.choice != "")
                    {
                        entry.optionTypeID = itemID;
                        InsertChoice(entry);
                    }
                }
                editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + mobID + "ID2:" + editingDisplay.id);
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

        void InsertChoice(AtavismEditorOptionChoice entry)
        {
            string query = "INSERT INTO editor_option_choice";
            //query += " (optionTypeID, choice) ";
            //query += "VALUES ";
            //query += " (" + entry.optionTypeID + ",'" + entry.choice + "') ";
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

            // Insert/Update the choices
            foreach (AtavismEditorOptionChoice entry in editingDisplay.optionChoices)
            {
                if (entry.choice != "")
                {
                    if (entry.id < 1)
                    {
                        // This is a new entry, insert it
                        entry.optionTypeID = editingDisplay.id;
                        InsertChoice(entry);
                    }
                    else
                    {
                        // This is an existing entry, update it
                        entry.optionTypeID = editingDisplay.id;
                        UpdateChoice(entry);
                    }
                }
            }
            // And now delete any choices that are tagged for deletion
            foreach (int choiceID in editingDisplay.choicesToBeDeleted)
            {
                DeleteChoice(choiceID);
            }

            // Update online table to avoid access the database again			
            dataLoaded = false;
             NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " " + Lang.GetTranslate("updated"));
             Load();
      }

        void UpdateChoice(AtavismEditorOptionChoice entry)
        {
            string query = "UPDATE editor_option_choice";
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
            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " " + Lang.GetTranslate("updated"));
            Load();
        }

        void DeleteChoice(int objectiveID)
        {
            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, objectiveID.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "editor_option_choice", delete);
            string query = "UPDATE editor_option_choice SET isactive = 0 where id = " + objectiveID;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
        }

        // Delete entries from the table
        void DeleteEntry()
        {
            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, tableName, delete);
            string query = "UPDATE " + tableName + " SET isactive = 0 where id = " + editingDisplay.id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            // Update online table to avoid access the database again			
            //  dataRegister.Remove(displayKeys[selectedDisplay]);
            //   displayKeys.Remove(selectedDisplay);
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
                where.Add(new Register("optionTypeID", "?optionTypeID", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "editor_option_choice", where);

                dataLoaded = false;
                dataRestoreLoaded = false;
                LoadRestore();
                NewResult(Lang.GetTranslate("Entry Permanent Deleted"));
            }
        }

        void DeleteForeverChoice(int id)
        {
            if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete") + " " + Lang.GetTranslate(functionName) + " " + Lang.GetTranslate("entry") + " ?", Lang.GetTranslate("Are you sure you want to permanent delete choice") + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
            {
                NewResult(Lang.GetTranslate("Deleting..."));
                List<Register> where = new List<Register>();
                where.Add(new Register("id", "?id", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "editor_option_choice", where);


                editingDisplay = LoadEntity(displayKeys[selectedDisplay], true);
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
            LoadRestore();
            NewResult(Lang.GetTranslate("Entry Restored"));
        }
        void RestoreEntryChoice(int id)
        {
            NewResult(Lang.GetTranslate("Restoring..."));
            string query = "UPDATE editor_option_choice SET isactive = 1 where id = " + id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());


           // dataLoaded = false;
           // dataRestoreLoaded = false;
            //Load();
            editingDisplay = LoadEntity(displayKeys[selectedDisplay], true);
            NewResult(Lang.GetTranslate("Entry Restored"));
        }


        /// <summary>
        /// Loads the atavism choice options for the specified OptionType
        /// </summary>
        /// <returns>The atavism choice options.</returns>
        /// <param name="optionType">Option type.</param>
        public static string[] LoadAtavismChoiceOptions(string optionType, bool allowNone)
        {
            string[] options = new string[] { };
            if (allowNone)
            {
                options = new string[1];
                options[0] = "~ none ~";
            }

            // First need to get the ID for the optionType
            int optionTypeID = -1;

            string query = "SELECT id FROM editor_option where optionType = '" + optionType + "' AND isactive = 1";
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    optionTypeID = int.Parse(data["id"]);
                }
            }

            // If we have an ID, load in the options
            if (optionTypeID != -1)
            {
                // Read all entries from the table
                query = "SELECT optionTypeID, choice FROM editor_option_choice where optionTypeID = " + optionTypeID + " AND isactive = 1";

                rows.Clear();
                // Load data
                rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
                //Debug.Log("#Rows:"+rows.Count);
                // Read all the data
                int optionsId = 0;
                if ((rows != null) && (rows.Count > 0))
                {
                    if (allowNone)
                    {
                        options = new string[rows.Count + 1];
                        options[0] = "~ none ~";
                        optionsId++;
                    }
                    else
                    {
                        options = new string[rows.Count];
                    }
                    foreach (Dictionary<string, string> data in rows)
                    {
                        options[optionsId] = data["choice"];
                        optionsId++;
                    }
                }
            }
            return options;
        }

        /// <summary>
        /// Loads the atavism choice options for the specified OptionType
        /// </summary>
        /// <returns>The atavism choice options.</returns>
        /// <param name="optionType">Option type.</param>
        public static void LoadAtavismChoiceOptions(string optionType, bool allowNone, out int[] ids, out string[] options)
        {
            string query = "SELECT id, choice FROM editor_option_choice where optionTypeID = (SELECT id from editor_option where "
                + "optionType = '" + optionType + "') AND isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            options = new string[rows.Count];
            ids = new int[rows.Count];

            int optionsId = 0;
            if (allowNone)
            {
                options = new string[rows.Count + 1];
                options[optionsId] = "~ none ~";
                ids = new int[rows.Count + 1];
                ids[optionsId] = -1;
                optionsId++;
            }

            // Read data
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    options[optionsId] = data["choice"];
                    ids[optionsId] = int.Parse(data["id"]);
                    optionsId++;
                }
            }
        }
        /// <summary>
        /// Loads the atavism choice options for the specified OptionType
        /// </summary>
        /// <param name="optionType"></param>
        /// <param name="allowNone"></param>
        /// <param name="ids"></param>
        /// <param name="options"></param>
        /// <param name="addAll"></param>
        public static void LoadAtavismChoiceOptions(string optionType, bool allowNone, out int[] ids, out string[] options,bool addAll)
        {
            string query = "SELECT id, choice FROM editor_option_choice where optionTypeID = (SELECT id from editor_option where "
                           + "optionType = '" + optionType + "') AND isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            options = new string[rows.Count];
            ids = new int[rows.Count];

            int optionsId = 0;
            if (addAll)
            {
                if (allowNone)
                {
                    options = new string[rows.Count + 2];
                    options[optionsId] = "~ none ~";
                    ids = new int[rows.Count + 2];
                    ids[optionsId] = -1;
                    optionsId++;
                    options[optionsId] = "Any";
                    ids[optionsId] = 0;
                    
                    
                }
                else
                {
                    options = new string[rows.Count + 1];
                    options[optionsId] = "Any";
                    ids = new int[rows.Count + 1];
                    ids[optionsId] = 0;
                    optionsId++;
                }
            }
            else
            {
                if (allowNone)
                {
                    options = new string[rows.Count + 1];
                    options[optionsId] = "~ none ~";
                    ids = new int[rows.Count + 1];
                    ids[optionsId] = -1;
                    optionsId++;
                }
            }

            // Read data
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    options[optionsId] = data["choice"];
                    ids[optionsId] = int.Parse(data["id"]);
                    optionsId++;
                }
            }
        }
    }
}