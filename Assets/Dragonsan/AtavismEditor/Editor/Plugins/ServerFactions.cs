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
    // Handles the Faction Configuration
    public class ServerFactions : AtavismDatabaseFunction
    {

        public new Dictionary<int, FactionData> dataRegister;
        public new FactionData editingDisplay;
        public new FactionData originalDisplay;

        public static int[] factionIds = new int[] { -1 };
        public static string[] factionOptions = new string[] { "~ none ~" };
        public static GUIContent[] GuiFactionOptions = new GUIContent[] { new GUIContent("~ none ~") };



        // Use this for initialization
        public ServerFactions()
        {
            functionName = "Factions";
            // Database tables name
            tableName = "factions";
            functionTitle = "Factions Configuration";
            loadButtonLabel = "Load Factions";
            notLoadedText = "No Faction loaded.";
            // Init
            dataRegister = new Dictionary<int, FactionData>();

            editingDisplay = new FactionData();
            originalDisplay = new FactionData();
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

        public static void LoadFactionOptions()
        {
            string query = "SELECT id, name FROM factions where isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
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

        public static void LoadFactionOptions(bool gui)
        {
            if (!gui)
            {
                LoadFactionOptions();
                return;
            }

            string query = "SELECT id, name FROM factions where isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                GuiFactionOptions = new GUIContent[rows.Count + 1];
                GuiFactionOptions[optionsId] = new GUIContent("~ none ~");
                factionIds = new int[rows.Count + 1];
                factionIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    GuiFactionOptions[optionsId] = new GUIContent(data["id"] + ":" + data["name"]);
                    factionIds[optionsId] = int.Parse(data["id"]);
                }
            }
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

        FactionData LoadEntity(int id)
        {
            // Read all entries from the table
            string query = "SELECT " + originalDisplay.GetFieldsString() + " FROM " + tableName + " where isactive = 1 AND id=" + id;

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            FactionData display = new FactionData();
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    //foreach(string key in data.Keys)
                    //	Debug.Log("Name[" + key + "]:" + data[key]);
                    //return;

                    display.id = int.Parse(data["id"]);
                    display.Name = data["name"];
                    display.group = data["factionGroup"];
                    display.modifyable = bool.Parse(data["public"]);
                    display.defaultStance = int.Parse(data["defaultStance"]);

                    display.isLoaded = true;
                    //Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
                }
            }
            LoadFactionStances(display);
            return display;
        }

        void LoadFactionStances(FactionData factionData)
        {
            // Read all entries from the table
            string query = "SELECT " + new FactionStanceEntry().GetFieldsString() + " FROM faction_stances where factionID = "
                + factionData.id + " AND isactive = 1";

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
                    FactionStanceEntry entry = new FactionStanceEntry();

                    entry.id = int.Parse(data["id"]);
                    entry.factionID = int.Parse(data["factionID"]);
                    entry.otherFactionID = int.Parse(data["otherFaction"]);
                    entry.defaultStance = int.Parse(data["defaultStance"]);
                    factionData.factionStances.Add(entry);
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create a Faction before editing it."));
                return;
            }

            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Faction"));


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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Faction Properties")+":");
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
            editingDisplay = new FactionData();
            originalDisplay = new FactionData();
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
                LoadFactionOptions();
                linkedTablesLoaded = true;
            }

            // Draw the content database info
            //pos.y += ImagePack.fieldHeight;

            if (newEntity)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new Faction"));
                pos.y += ImagePack.fieldHeight;
            }

            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.75f);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.group = ImagePack.DrawField(pos, Lang.GetTranslate("Group")+":", editingDisplay.group, 0.75f);
            pos.y += ImagePack.fieldHeight;
            pos.width /= 2;
            editingDisplay.modifyable = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Dynamic")+":", editingDisplay.modifyable);
            pos.y += ImagePack.fieldHeight;
            int selectedStance = GetPositionOfStance(editingDisplay.defaultStance);
            selectedStance = ImagePack.DrawSelector(pos, Lang.GetTranslate("Default Stance")+":", selectedStance, FactionData.stanceOptions);
            editingDisplay.defaultStance = FactionData.stanceValues[selectedStance];
            pos.y += 1.5f * ImagePack.fieldHeight;
            if (editingDisplay.factionStances.Count == 0)
            {
                editingDisplay.factionStances.Add(new FactionStanceEntry(0, 0));
            }
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Faction Stance")+":");
            pos.y += ImagePack.fieldHeight;

            for (int i = 0; i < editingDisplay.factionStances.Count; i++)
            {
                 int otherFactionID = GetPositionOfFaction(editingDisplay.factionStances[i].otherFactionID);
                otherFactionID = ImagePack.DrawSelector(pos, Lang.GetTranslate("Other Faction")+":", otherFactionID, factionOptions);
                editingDisplay.factionStances[i].otherFactionID = factionIds[otherFactionID];
                pos.x += pos.width;
                selectedStance = GetPositionOfStance(editingDisplay.factionStances[i].defaultStance);
                selectedStance = ImagePack.DrawSelector(pos, Lang.GetTranslate("Default Stance")+":", selectedStance, FactionData.stanceOptions);
                editingDisplay.factionStances[i].defaultStance = FactionData.stanceValues[selectedStance];
                pos.y += ImagePack.fieldHeight;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Stance")))
                {
                    if (editingDisplay.factionStances[i].id > 0)
                        editingDisplay.stancesToBeDeleted.Add(editingDisplay.factionStances[i].id);
                    editingDisplay.factionStances.RemoveAt(i);
                }
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
            }
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Stance")))
            {
                editingDisplay.factionStances.Add(new FactionStanceEntry(0, 0));
            }

            pos.width *= 2;
            pos.y += 1.4f * ImagePack.fieldHeight;
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
                // Insert the abilities
                foreach (FactionStanceEntry entry in editingDisplay.factionStances)
                {
                    if (entry.otherFactionID != -1)
                    {
                        entry.factionID = itemID;
                        InsertStance(entry);
                    }
                }

                // Update online table to avoid access the database again			
                editingDisplay.id = itemID;
                editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
                dataLoaded = false;
                 newItemCreated = true;
                NewResult(Lang.GetTranslate("New entry inserted"));
              Load();
                 LoadFactionOptions();
               
            }
            else
            {
                NewResult("Error:"+Lang.GetTranslate("Error occurred, please check the Console"));
            }
        }

        void InsertStance(FactionStanceEntry entry)
        {
            string query = "INSERT INTO faction_stances";
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

            // Insert/Update the abilities
            foreach (FactionStanceEntry entry in editingDisplay.factionStances)
            {
                if (entry.otherFactionID != -1)
                {
                    if (entry.id == 0)
                    {
                        // This is a new entry, insert it
                        entry.factionID = editingDisplay.id;
                        InsertStance(entry);
                    }
                    else
                    {
                        // This is an existing entry, update it
                        entry.factionID = editingDisplay.id;
                        UpdateStance(entry);
                    }
                }
            }
            // Delete any stances that are tagged for deletion
            foreach (int stanceID in editingDisplay.stancesToBeDeleted)
            {
                DeleteStance(stanceID);
            }

            // Update online table to avoid access the database again			
            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " "+ Lang.GetTranslate("updated"));
             Load();
       }

        void UpdateStance(FactionStanceEntry entry)
        {
            string query = "UPDATE faction_stances";
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

        void DeleteStance(int stanceID)
        {
            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, stanceID.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "faction_stances", delete);
            string query = "UPDATE faction_stances SET isactive = 0 where id = " + stanceID;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
        }

        // Delete entries from the table
        void DeleteEntry()
        {


            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, tableName, delete);
            string query = "UPDATE " + tableName + " SET isactive = 0 where id = " + editingDisplay.id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            // Delete the stance links
            //delete = new Register("factionID", "?factionID", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "faction_stances", delete);
            query = "UPDATE faction_stances SET isactive = 0 where factionID = " + editingDisplay.id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            // Update online table to avoid access the database again		
            selectedDisplay = -1;
            newSelectedDisplay = 0;
            dataLoaded = false;
             NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " "+ Lang.GetTranslate("Deleted"));
            Load();
           LoadFactionOptions();
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
                where.Add(new Register("factionID", "?factionID", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "faction_stances", where);

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

            query = "UPDATE faction_stances SET isactive = 1 where factionID = " + id;
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

        private int GetPositionOfStance(int stanceValue)
        {
            for (int i = 0; i < FactionData.stanceValues.Length; i++)
            {
                if (FactionData.stanceValues[i] == stanceValue)
                    return i;
            }
            return 0;
        }

        private int GetPositionOfFaction(int factionID)
        {
            for (int i = 0; i < factionIds.Length; i++)
            {
                if (factionIds[i] == factionID)
                    return i;
            }
            return 0;
        }
    }
}