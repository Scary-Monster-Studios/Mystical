using UnityEngine;
using UnityEditor;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Atavism
{
    // Class that implements the Instances configuration
    public class ServerBonusSettings : AtavismDatabaseFunction
    {
        public new Dictionary<int, BonusOptionData> dataRegister;
        public new BonusOptionData editingDisplay;
        public new BonusOptionData originalDisplay;

        public int[] accountIds = new int[] { 1 };
        public string[] accountList = new string[] { "~ First Account ~" };

        public static int[] arenaIds = new int[] { 1 };
        public static string[] arenaList = new string[] { "~ none ~" };

        public int[] currencyIds = new int[] { -1 };
        public string[] currencyOptions = new string[] { "~ none ~" };
       
        // Use this for initialization
        public ServerBonusSettings()
        {
            functionName = "Bonus Settings";
            // Database tables name
            tableName = "bonuses_settings";
            functionTitle = "Bonus Setting";
            loadButtonLabel = "Load Bonus";
            notLoadedText = "No Bonus Setting loaded.";
            // Init
            dataRegister = new Dictionary<int, BonusOptionData>();

            editingDisplay = new BonusOptionData();
            originalDisplay = new BonusOptionData();
        }
        void resetSearch(bool fokus)
        {
           if(fokus) GUI.FocusControl(null);

        }
        public override void Activate()
        {
            linkedTablesLoaded = false;
            resetSearch(true);
            //showCreate = false;
            //showDelete = false;
        }

   
        

        // Load Database Data
        public override void Load()
        {
            if (!dataLoaded)
            {
               
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

        BonusOptionData LoadEntity(int id)
        {
            // Read all entries from the table
            string query = "SELECT * FROM " + tableName+ " WHERE isactive = 1 AND id=" + id;

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read all the data
            BonusOptionData display = new BonusOptionData();
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    display.id = int.Parse(data["id"]);
                    display.Name = data["name"];
                    display.code = data["code"];
                    display.param = data["param"];

               //     display.isLoaded = false;
                }
            }
        //    LoadWaetherInstaces(display);
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

        // Draw the Instance list
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create an Bonus Setting before editing it."));
                return;
            }


            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Bonus Setting"));

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
                pos.x += pos.width;
                pos.y += ImagePack.fieldHeight * 1.5f;
             /*   if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Duplicate")))
                {
                    Duplicate();
                }*/
                pos.x -= pos.width;
                pos.width *= 2;
                pos.y += ImagePack.fieldHeight * 1.5f;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Bonus Settings:"));
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
            editingDisplay = new BonusOptionData();
            originalDisplay = new BonusOptionData();
            selectedDisplay = -1;
        }
        // Edit or Create a new instance
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
              //  LoadInstanceOptions();
                linkedTablesLoaded = true;
            }

            // Draw the content database info
            if (newEntity)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new Bonus Setting"));
                pos.y += ImagePack.fieldHeight;
            }
            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.75f);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.code = ImagePack.DrawField(pos, Lang.GetTranslate("Code") + ":", editingDisplay.code);
            pos.y += ImagePack.fieldHeight;
            pos.width /= 2;
            bool val = true;
            bool valp = true;
            if(editingDisplay.param.Contains("v"))
                val = true;
            else
                val = false;
            if (editingDisplay.param.Contains("p"))
                valp = true;
            else
                valp = false;
            val = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Value") + "?", val);
            pos.x += pos.width;
            valp = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Percentage") + "?", valp);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
                editingDisplay.param = "";
            if (val)
                editingDisplay.param = "v";
            if (valp)
                editingDisplay.param += "p";

            // Save Instance data
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
            EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight+50);
            /*
            if (!newEntity)
                EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight + 100);
            else
                EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight);*/

        }

        public override void save()
        {
            if (newEntity)
                InsertEntry();
            else
                UpdateEntry();

            resetSearch(true);
            state = State.Loaded;
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
            query += " (name ,code,param ,isactive) ";
            query += "VALUES ";
            query += "(?name ,?code,?param ,?isactive) ";

            int arenaID = -1;

            // Setup the register data		
            List<Register> update = new List<Register>();
            update.Add(new Register("name", "?name", MySqlDbType.VarChar, editingDisplay.Name.ToString(), Register.TypesOfField.String));
            update.Add(new Register("code", "?code", MySqlDbType.VarChar, editingDisplay.code.ToString(), Register.TypesOfField.String));
            update.Add(new Register("param", "?param", MySqlDbType.VarChar, editingDisplay.param.ToString(), Register.TypesOfField.String));
            update.Add(new Register("isactive", "?isactive", MySqlDbType.Int32, "1", Register.TypesOfField.Int));

            // Update the database
            arenaID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);

            // If the insert failed, don't insert the spawn marker
            if (arenaID != -1)
            {
                //int islandID = arenaID;
                //    int i = 1;

                // Update online table to avoid access the database again			
                editingDisplay.id = arenaID;
               // editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + instanceID + "ID2:" + editingDisplay.id);
                dataLoaded = false;
                Load();
                newItemCreated = true;
                NewResult(Lang.GetTranslate("New entry inserted"));
            }
            else
            {
                NewResult("Error:"+ Lang.GetTranslate("Error occurred, please check the Console"));
            }
        }



        // Update existing entries in the table based on the iddemo_table
        void UpdateEntry()
        {
            NewResult(Lang.GetTranslate("Updating..."));
            // Setup the update query
            string query = "UPDATE " + tableName;
            query += " SET name=?name,";
            query += " code=?code,";
            query += " param=?param,";
            query += " isactive=?isactive ";
            query += " WHERE id=?id ";
            // Setup the register data		
            List<Register> update = new List<Register>();
            update.Add(new Register("name", "?name", MySqlDbType.VarChar, editingDisplay.Name.ToString(), Register.TypesOfField.String));
            update.Add(new Register("code", "?code", MySqlDbType.VarChar, editingDisplay.code, Register.TypesOfField.String));
            update.Add(new Register("param", "?param", MySqlDbType.VarChar, editingDisplay.param, Register.TypesOfField.String));
            update.Add(new Register("isactive", "?isactive", MySqlDbType.Int32, "1", Register.TypesOfField.Int));
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
            //Check weather_instance
            string sql = "SELECT vip_level_id FROM vip_level_bonuses where bonus_settings_id = " + editingDisplay.id;
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sql);
            int Count = 0;
            string Ids = "";
            if ((rows != null) && (rows.Count > 0))
            {
                Ids = "\n"+ Lang.GetTranslate("Vip level Ids")+":";
                Count = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    Ids += " " + data["vip_level_id"] + ",";
                }
            }

            sql = "SELECT vip_level_id FROM vip_level_bonuses where bonus_settings_id = " + editingDisplay.id;
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sql);
            Count = 0;
            //string Ids = "";
            if ((rows != null) && (rows.Count > 0))
            {
                Ids = "\n" + Lang.GetTranslate("Vip level Ids") + ":";
                Count = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    Ids += " " + data["vip_level_id"] + ",";
                }
            }
            DatabasePack.SaveLog("Delete Bonus Settings id =" + editingDisplay.id + " assigned to " + Ids );

            if (Count == 0)
            {
                string query = "UPDATE " + tableName + " SET isactive = 0 where id = " + editingDisplay.id;
                DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
                // Update online table to avoid access the database again			
                selectedDisplay = -1;
                newSelectedDisplay = 0;
                NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " "+ Lang.GetTranslate("Deleted"));
            }
            else
            {

                EditorUtility.DisplayDialog(Lang.GetTranslate("Can't Dalete Bonus Setting"), Lang.GetTranslate("You can not delete this Bonus Setting because it is assigned in:") + Ids , Lang.GetTranslate("OK"), "");


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
            NewResult(Lang.GetTranslate("Entry Restored"));
        }
        void Duplicate()
        {
            // Update name of current entry by adding "1" to the end
            editingDisplay.Name = editingDisplay.Name + " (Clone)";
            InsertEntry();
            state = State.Loaded;
        }
   

    }
}