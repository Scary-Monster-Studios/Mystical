using UnityEngine;
using UnityEditor;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
namespace Atavism
{
    // Handles the Effects Configuration
    public class ServerGameSetting : AtavismDatabaseFunction
    {

        public new Dictionary<int, GameSettingData> dataRegister;
        public new GameSettingData editingDisplay;
        public new GameSettingData originalDisplay;
        public static string[] dataTypeOptions = new string[] { "bool", "float", "int", "string" };
        public static string[] boolValueOptions = new string[] { "true", "false" };

        // Use this for initialization
        public ServerGameSetting()
        {
            functionName = "Game Setting";
            // Database tables name
            tableName = "game_setting";
            functionTitle = "Game Setting Configuration";
            loadButtonLabel = "Load Game Setting";
            notLoadedText = "No Game Setting loaded.";
            // Init
            dataRegister = new Dictionary<int, GameSettingData>();
            editingDisplay = new GameSettingData();
            originalDisplay = new GameSettingData();
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

        GameSettingData LoadEntity(int id)
        {
            // Read entry from the table
            string query = "SELECT "+originalDisplay.GetFieldsString()+" FROM " + tableName + " where id ="+id+" AND isactive = 1";
            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();
            GameSettingData display = new GameSettingData();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    display.id = int.Parse(data["id"]);
                    display.Name = data["name"];
                    display.dataType = data["dataType"];
                    display.val = data["value"];
                    display.isLoaded = true;
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create a Game Setting before edit it."));
                return;
            }

            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Game Setting Configuration"));

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
                if(selectedDisplay>=0)
                    displayKey =  displayKeys[selectedDisplay];
                editingDisplay = LoadEntity(displayKey);
            }

            pos.y += ImagePack.fieldHeight;
            pos.x -= ImagePack.innerMargin;
            pos.y -= ImagePack.innerMargin;
            pos.width += ImagePack.innerMargin;
            DrawEditor(pos, false);
            pos.y -= ImagePack.fieldHeight;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;

            if (state != State.Loaded)
            {
                pos.y += ImagePack.fieldHeight;
                pos.width /= 2;
                newSelectedDisplay = ImagePack.DrawDynamicPartialListSelector(pos, Lang.GetTranslate("Search Filter")+": ", ref entryFilterInput, selectedDisplay, displayList);
                pos.x += pos.width;
                pos.x -= pos.width;
                pos.width *= 2;
            }
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You dont have deleted")+" " + Lang.GetTranslate(functionName) +".");
                return;
            }

            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Restore")+" " + Lang.GetTranslate(functionName) +" "+ Lang.GetTranslate("Configuration"));
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
            pos.width += 140 + 155;
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
            editingDisplay = new GameSettingData();
            originalDisplay = new GameSettingData();
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

            // Draw the content database info
            //pos.y += ImagePack.fieldHeight;
            if (!linkedTablesLoaded)
            {
                linkedTablesLoaded = true;
            }

            if (newEntity)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new Game Setting"));
                pos.y += ImagePack.fieldHeight;
            }
            pos.y += ImagePack.fieldHeight;

            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.75f);
            pos.y += ImagePack.fieldHeight;
            //editingDisplay.val = ImagePack.DrawCombobox (pos, "Resistance Stat:", editingDisplay.resistanceStat, statOptions);
            editingDisplay.dataType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Data Type") + ":", editingDisplay.dataType, dataTypeOptions);
            pos.y += ImagePack.fieldHeight;
           // if(editingDisplay.dataType.Equals("bool"))
         //       editingDisplay.val = ImagePack.DrawSelector(pos, Lang.GetTranslate("Value") + ":", editingDisplay.val, boolValueOptions);
         //   else
                editingDisplay.val = ImagePack.DrawField(pos, Lang.GetTranslate("Value")+":", editingDisplay.val, 0.75f);

            if (editingDisplay.dataType.Equals("bool"))
            {
                bool a;
                if (!bool.TryParse(editingDisplay.val, out a))
                {
                    pos.y += ImagePack.fieldHeight;
                    Color temp = GUI.color;
                    GUI.color = Color.red;
                    ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("value is not bool"));
                    GUI.color = temp;
                }
            }
            else if(editingDisplay.dataType.Equals("float"))
            { float a;
                if (!float.TryParse(editingDisplay.val,out a))
                {
                    pos.y += ImagePack.fieldHeight;
                    Color temp = GUI.color;
                    GUI.color = Color.red;
                    ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("value is not float"));
                    GUI.color = temp;
                }
            }
            else if (editingDisplay.dataType.Equals("int"))
            {
                int a;
                if (!int.TryParse(editingDisplay.val,out a))
                {
                    pos.y += ImagePack.fieldHeight;
                    Color temp = GUI.color;
                    GUI.color = Color.red;
                    ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("value is not int"));
                    GUI.color = temp;
                }
            }
                pos.y += ImagePack.fieldHeight;
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
            if (selectedDisplay>-1)
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
                // Update online table to avoid access the database again			
                //   editingDisplay.id = mobID;
                //  editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + mobID + "ID2:" + editingDisplay.id);
                //  dataRegister.Add(editingDisplay.id, editingDisplay);
                //  displayKeys.Add(editingDisplay.id);
                dataLoaded = false;
                Load();
                newItemCreated = true;
                NewResult(Lang.GetTranslate("New entry inserted"));
            }
            else
            {
                NewResult("Error:"+Lang.GetTranslate("Error occurred, please check the Console"));
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

            // Update online table to avoid access the database again		
            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " "+ Lang.GetTranslate("updated"));
               Load();
     }

        // Delete entries from the table
        void DeleteEntry()
        {
            NewResult(Lang.GetTranslate("Deleting..."));
            string query = "UPDATE " + tableName + " SET isactive = 0 where id = " + editingDisplay.id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
            // Update online table to avoid access the database again		
            selectedDisplay = -1;
            newSelectedDisplay = 0;
            dataLoaded = false;
             NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name+ " "+ Lang.GetTranslate("Deleted"));
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

    }
}