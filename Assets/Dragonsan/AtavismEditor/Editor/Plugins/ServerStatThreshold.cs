using UnityEngine;
using UnityEditor;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
namespace Atavism
{
    // Handles the Effects Configuration
    public class ServerStatThreshold : AtavismDatabaseFunction
    {

        public new Dictionary<int, StatThresholdData> dataRegister;
        public new StatThresholdData editingDisplay;
        public new StatThresholdData originalDisplay;
        public static string[] dataTypeOptions = new string[] { "bool", "float", "int", "string" };
        public static string[] boolValueOptions = new string[] { "true", "false" };

        public string[] statFunctionOptions = new string[] { "~ none ~" };

        // Use this for initialization
        public ServerStatThreshold()
        {
            functionName = "Stat Threshold";
            // Database tables name
            tableName = "stat_thresholds";
            functionTitle = "Stat Threshold Configuration";
            loadButtonLabel = "Load Stat Threshold";
            notLoadedText = "No Stat Threshold loaded.";
            // Init
            dataRegister = new Dictionary<int, StatThresholdData>();
            editingDisplay = new StatThresholdData();
            originalDisplay = new StatThresholdData();
        }

        public override void Activate()
        {
            linkedTablesLoaded = false;
            resetSearch(true);
            showRecovery = false;
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
                string query = "SELECT distinct stat_function FROM " + tableName;
                // + " where isactive = 1";

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
                        displayList.Add(data["stat_function"]);
                        //displayKeys.Add(int.Parse(data["id"]));
                        
                    }
                }
                dataLoaded = true;
            }
        }

        StatThresholdData LoadEntity(string id)
        {
            // Read entry from the table
            string query = "SELECT * FROM " + tableName + " where stat_function = '" + id + "'";//" AND isactive = 1";
            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();
            StatThresholdData display = new StatThresholdData();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    display.function = data["stat_function"];
                    StatThresholdEntry ste = new StatThresholdEntry(data["stat_function"], int.Parse(data["threshold"]), int.Parse(data["threshold"]), int.Parse(data["num_per_point"]));
                    display.thresholds.Add(ste);
                    display.isLoaded = true;
                }
            }
            return display;
        }


      /*  public override void LoadRestore()
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create a Stat Threshold before edit it."));
                return;
            }

            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Stat Threshold Configuration"));

            if (newItemCreated)
            {
                newItemCreated = false;
                newSelectedDisplay = displayKeys.Count - 1;
            }
            // Draw data Editor
            if (newSelectedDisplay != selectedDisplay)
            {
                selectedDisplay = newSelectedDisplay;
                string displayKey = "";
                if(selectedDisplay>=0)
                    displayKey = displayList[selectedDisplay];
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

      /*  public override void DrawRestore(Rect box)
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
                   // DeleteForever(displayKeys[i]);
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
        */
        public override void CreateNewData()
        {
            editingDisplay = new StatThresholdData();
            originalDisplay = new StatThresholdData();
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
                statFunctionOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Stat Functions", true);

                linkedTablesLoaded = true;
            }

            if (newEntity)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new Stat Threshold"));
                pos.y += ImagePack.fieldHeight;
            }
            pos.y += ImagePack.fieldHeight * 1.5f;
            editingDisplay.function = ImagePack.DrawSelector(pos, Lang.GetTranslate("Stat Funstion") + ":", editingDisplay.function, statFunctionOptions);
            pos.y += ImagePack.fieldHeight * 1.5f;

            for (int i = 0; i < editingDisplay.thresholds.Count; i++)
            {
                pos.width = pos.width / 2;
                editingDisplay.thresholds[i].threshold = ImagePack.DrawField(pos, Lang.GetTranslate("Threshold") + ":", editingDisplay.thresholds[i].threshold);
                pos.x += pos.width;
                editingDisplay.thresholds[i].points = ImagePack.DrawField(pos, Lang.GetTranslate("Number per point") + ":", editingDisplay.thresholds[i].points);
                pos.x -= pos.width;

                pos.x += pos.width;
                pos.y += ImagePack.fieldHeight;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove Threshold")))
                {
                    editingDisplay.thresholdsToDelete.Add(editingDisplay.thresholds[i]);
                    editingDisplay.thresholds.RemoveAt(i);
                }
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                pos.width = pos.width * 2;
            }

         //   if (editingDisplay.thresholds.Count < vipOptions.Length)
          //  {
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Threshold")))
                {
                    StatThresholdEntry statEntry = new StatThresholdEntry(editingDisplay.function,0, 0, 0);
                    editingDisplay.thresholds.Add(statEntry);
                }
          //  }
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
                editingDisplay = LoadEntity(displayList[selectedDisplay]);
           /* else
                editingDisplay = LoadEntity(selectedDisplay);*/
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
            query += " (stat_function ,threshold ,num_per_point) ";
            query += "VALUES ";
            query += "(?stat_function ,?threshold ,?num_per_point) ";
            int achivID = -1;

            foreach (StatThresholdEntry ste in editingDisplay.thresholds)
            {
                List<Register> update = new List<Register>();
                update.Add(new Register("stat_function", "?stat_function", MySqlDbType.VarChar, ste.function, Register.TypesOfField.String));
                update.Add(new Register("threshold", "?threshold", MySqlDbType.Int32, ste.threshold.ToString(), Register.TypesOfField.Int));
                update.Add(new Register("num_per_point", "?num_per_point", MySqlDbType.Int32, ste.points.ToString(), Register.TypesOfField.Int));
                achivID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);
            }
          

            // If the insert failed, don't insert the spawn marker
            if (achivID != -1)
            {
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

            foreach (StatThresholdEntry ste in editingDisplay.thresholdsToDelete)
            {
                DeleteTreshold(ste);
            }
            string query = "UPDATE " + tableName;
            query += " SET ";
            query += " num_per_point =?num_per_point, threshold=?threshold";
            query += " WHERE stat_function=?stat_function AND threshold=?threshold_old";
            foreach (StatThresholdEntry ste in editingDisplay.thresholds)
            {


                if (ste.threshold_org == 0)
                {
                    string query2 = "INSERT INTO " + tableName;
                    query2 += " (stat_function ,threshold ,num_per_point) ";
                    query2 += "VALUES ";
                    query2 += "(?stat_function ,?threshold ,?num_per_point) ";
                    List<Register> update = new List<Register>();
                    update.Add(new Register("stat_function", "?stat_function", MySqlDbType.VarChar, editingDisplay.function, Register.TypesOfField.String));
                    update.Add(new Register("threshold", "?threshold", MySqlDbType.Int32, ste.threshold.ToString(), Register.TypesOfField.Int));
                    update.Add(new Register("num_per_point", "?num_per_point", MySqlDbType.Int32, ste.points.ToString(), Register.TypesOfField.Int));
                     DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query2, update);
                }
                else
                {
                    List<Register> update = new List<Register>();
                    update.Add(new Register("stat_function", "?stat_function", MySqlDbType.VarChar, ste.function, Register.TypesOfField.String));
                    update.Add(new Register("threshold", "?threshold", MySqlDbType.Int32, ste.threshold.ToString(), Register.TypesOfField.Int));
                    update.Add(new Register("threshold", "?threshold_old", MySqlDbType.Int32, ste.threshold_org.ToString(), Register.TypesOfField.Int));
                    update.Add(new Register("num_per_point", "?num_per_point", MySqlDbType.Int32, ste.points.ToString(), Register.TypesOfField.Int));

                    // Update the database
                    DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);
                }
            }
            // Update online table to avoid access the database again		
            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " "+ Lang.GetTranslate("updated"));
               Load();
     }

        // Delete entries from the table
        void DeleteEntry()
        {
            NewResult(Lang.GetTranslate("Deleting..."));
            //string query = "UPDATE " + tableName + " SET isactive = 0 where id = " + editingDisplay.id;
            //DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
            List<Register> where = new List<Register>();
            where.Add(new Register("stat_function", "?stat_function", MySqlDbType.String, editingDisplay.function, Register.TypesOfField.String));
            DatabasePack.Delete(DatabasePack.contentDatabasePrefix, tableName, where);

            // Update online table to avoid access the database again		
            selectedDisplay = -1;
            newSelectedDisplay = 0;
            dataLoaded = false;
             NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name+ " "+ Lang.GetTranslate("Deleted"));
            Load();
       }

        void DeleteTreshold(StatThresholdEntry ste)
        {
           // if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete") + " " + Lang.GetTranslate(functionName) + " " + Lang.GetTranslate("entry") + " ?", Lang.GetTranslate("Are you sure you want to permanent delete") + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
          //  {
                NewResult(Lang.GetTranslate("Deleting..."));
                List<Register> where = new List<Register>();
                where.Add(new Register("stat_function", "?stat_function", MySqlDbType.String, ste.function, Register.TypesOfField.String));
                where.Add(new Register("threshold", "?threshold", MySqlDbType.Int32, ste.threshold_org.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, tableName, where);

              //  dataLoaded = false;
               // dataRestoreLoaded = false;
                // Load();
                NewResult(Lang.GetTranslate("Entry Deleted"));
           // }
        }
      /*  void DeleteEntry(string function, int threshold)
        {
            if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete") + " " + Lang.GetTranslate(functionName) + " " + Lang.GetTranslate("entry") + " ?", Lang.GetTranslate("Are you sure you want to permanent delete") + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
            {
                NewResult(Lang.GetTranslate("Deleting..."));
                List<Register> where = new List<Register>();
                where.Add(new Register("stat_function", "?stat_function", MySqlDbType.String, function, Register.TypesOfField.String));
                where.Add(new Register("threshold", "?threshold", MySqlDbType.Int32, threshold.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, tableName, where);

                //  dataLoaded = false;
                // dataRestoreLoaded = false;
                // Load();
                NewResult(Lang.GetTranslate("Entry Deleted"));
            }
        }*/
        /*void RestoreEntry(int id)
        {
            NewResult(Lang.GetTranslate("Restoring..."));
            string query = "UPDATE " + tableName + " SET isactive = 1 where id = " + id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
            dataLoaded = false;
            dataRestoreLoaded = false;
            //Load();
            NewResult(Lang.GetTranslate("Entry Restored"));
        }
        */
    }
}