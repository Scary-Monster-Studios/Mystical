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
    // Handles the Skills Configuration
    public class ServerPatrolPaths : AtavismDatabaseFunction
    {

        public new Dictionary<int, PatrolPathData> dataRegister;
        public new PatrolPathData editingDisplay;
        public new PatrolPathData originalDisplay;

        // Use this for initialization
        public ServerPatrolPaths()
        {
            functionName = "Patrol Paths";
            // Database tables name
            tableName = "patrol_path";
            functionTitle = "Patrol Path Configuration";
            loadButtonLabel = "Load Patrol Paths";
            notLoadedText = "No Tables loaded.";
            // Init
            dataRegister = new Dictionary<int, PatrolPathData>();

            editingDisplay = new PatrolPathData();
            originalDisplay = new PatrolPathData();
            showCreate = false;
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

        PatrolPathData LoadEntity(int id)
        {
            // Read all entries from the table
            string query = "SELECT " + originalDisplay.GetFieldsString() + " FROM " + tableName + " where isactive = 1 AND id=" + id;

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            AtavismLogger.LogInfoMessage("#Rows:" + rows.Count);
            // Read all the data
            PatrolPathData display = new PatrolPathData();
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    foreach (string key in data.Keys)
                    {
                        AtavismLogger.LogInfoMessage("Name[" + key + "]:" + data[key]);
                    }
                    //return;

                    display.id = int.Parse(data["id"]);
                    display.Name = data["name"];
                    display.travelReverse = bool.Parse(data["travelReverse"]);
                    display.locX = float.Parse(data["locX"]);
                    display.locY = float.Parse(data["locY"]);
                    display.locZ = float.Parse(data["locZ"]);
                    display.lingerTime = float.Parse(data["lingerTime"]);
                    display.nextPoint = int.Parse(data["nextPoint"]);

                    display.isLoaded = true;
                    //Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
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

        /*void LoadMerchantTableItems (PatrolPathData merchantTable)
        {
            // Read all entries from the table
            string query = "SELECT * FROM " + "merchant_item" + " where tableID = " + merchantTable.id;

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear ();

            // Load data
            rows = DatabasePack.LoadData (DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            if ((rows != null) && (rows.Count > 0)) {
                foreach (Dictionary<string,string> data in rows) {
                    MerchantTableItemEntry entry = new MerchantTableItemEntry ();

                    entry.id = int.Parse (data ["id"]);
                    entry.count = int.Parse (data ["count"]); 
                    entry.itemID = int.Parse (data ["itemID"]);
                    entry.refreshTime = int.Parse (data ["refreshTime"]);
                    merchantTable.tableItems.Add (entry);
                }
            }
        }*/



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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create a Patrol Path before editing it."));
                return;
            }

            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Patrol Path"));


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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Patrol Path Properties")+":");
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You dont have deleted")+" " + functionName + ".");
                return;
            }

            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Restore")+" " + Lang.GetTranslate(functionName) + " "+ Lang.GetTranslate("Configuration"));
            pos.y += ImagePack.fieldHeight;

            pos.width -= 140+ 155;
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

                pos.x -= pos.width+ 155;
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
            editingDisplay = new PatrolPathData();
            originalDisplay = new PatrolPathData();
            selectedDisplay = -1;
        }

        // Edit or Create
        public override void DrawEditor(Rect box, bool newItem)
        {
            newEntity = newItem;
            // Setup the layout
            Rect pos = box;
            pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;

            if (!linkedTablesLoaded)
            {
                linkedTablesLoaded = true;
            }

            // Draw the content database info
            //pos.y += ImagePack.fieldHeight;

            if (newEntity)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new Patrol Path"));
                pos.y += ImagePack.fieldHeight;
            }

            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.75f);
            pos.width /= 2;
            pos.y += 1.5f * ImagePack.fieldHeight;
            /*ImagePack.DrawLabel (pos.x, pos.y, "Table Items");
            pos.y += 1.5f * ImagePack.fieldHeight;
            if (editingDisplay.tableItems.Count == 0) {
                editingDisplay.tableItems.Add (new MerchantTableItemEntry (-1, -1));
            }
            for (int i = 0; i < editingDisplay.tableItems.Count; i++) {
                editingDisplay.tableItems [i].count = ImagePack.DrawField (pos, "Count:", editingDisplay.tableItems [i].count);
                pos.x += pos.width;
                int selectedItem = GetPositionOfItem (editingDisplay.tableItems [i].itemID);
                selectedItem = ImagePack.DrawSelector (pos, "Item " + (i + 1) + ":", selectedItem, itemsList);
                editingDisplay.tableItems [i].itemID = itemIds [selectedItem];
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                editingDisplay.tableItems [i].refreshTime = ImagePack.DrawField (pos, "Refresh Time:", editingDisplay.tableItems [i].refreshTime);
                pos.x += pos.width;
                if (ImagePack.DrawButton (pos.x, pos.y, "Remove Item")) {
                    if (editingDisplay.tableItems[i].id > 0)
                        editingDisplay.itemsToBeDeleted.Add(editingDisplay.tableItems[i].id);
                    editingDisplay.tableItems.RemoveAt(i);
                }
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
            }
            if (ImagePack.DrawButton (pos.x, pos.y, "Add Item")) {
                editingDisplay.tableItems.Add (new MerchantTableItemEntry (-1, -1));
            }*/

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
            if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete")+" " + functionName + " "+ Lang.GetTranslate("entry")+" ?", Lang.GetTranslate("Are you sure you want to delete")+" " + editingDisplay.Name + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
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
                editingDisplay.id = itemID;
                // Insert the abilities
                foreach (PatrolPointEntry entry in editingDisplay.tableItems)
                {
                    /*if (entry.itemID != -1) {
                        entry.tableID = itemID;
                        InsertItem (entry);
                    }*/
                }

                // Update online table to avoid access the database again			
                //editingDisplay.id = itemID;
                editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
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

        void InsertItem(MerchantTableItemEntry entry)
        {
            string query = "INSERT INTO merchant_item";
            query += " (tableID, count, itemID, refreshTime) ";
            query += "VALUES ";
            query += " (" + entry.tableID + "," + entry.count + "," + entry.itemID + "," + entry.refreshTime + ") ";

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
            foreach (PatrolPointEntry entry in editingDisplay.tableItems)
            {
                /*if (entry.itemID != -1) {
                    if (entry.id < 1) {
                        // This is a new entry, insert it
                        entry.tableID = editingDisplay.id;
                        InsertItem (entry);
                    } else {
                        // This is an existing entry, update it
                        entry.tableID = editingDisplay.id;
                        UpdateItem (entry);
                    }
                }*/
            }

            // Delete any abilities that are tagged for deletion
            foreach (int itemID in editingDisplay.itemsToBeDeleted)
            {
                DeleteItem(itemID);
            }

            // Update online table to avoid access the database again			
            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " "+ Lang.GetTranslate("updated"));
              Load();
      }

        void UpdateItem(MerchantTableItemEntry entry)
        {
            string query = "UPDATE merchant_item";
            query += " SET ";
            query += entry.UpdateList();
            query += " WHERE id=?id";

            // Setup the register data		
            List<Register> update = new List<Register>();
            foreach (string field in entry.fields.Keys)
            {
                update.Add(entry.fieldToRegister(field));
            }

            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);
        }

        void DeleteItem(int itemID)
        {
            Register delete = new Register("id", "?id", MySqlDbType.Int32, itemID.ToString(), Register.TypesOfField.Int);
            DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "merchant_item", delete);
        }

        // Delete entries from the table
        void DeleteEntry()
        {
            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, tableName, delete);
            string query = "UPDATE " + tableName + " SET isactive = 0 where id = " + editingDisplay.id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            // Update online table to avoid access the database again		
            selectedDisplay = -1;
            newSelectedDisplay = 0;
            dataLoaded = false;
           NewResult("Entry " + editingDisplay.Name + " Deleted");
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

            InsertEntry();
            state = State.Loaded;
        }

        public static void SavePatrolPath(PatrolPathMarker marker)
        {
            // Verify the user is logged in and the Content Database is connected

            // If the gameObject already has an id, run an update
            if (marker.id < 1)
            {
                InsertMarker(marker);
            }
            else
            {
                UpdateMarker(marker);
            }
        }

        public static int InsertMarker(PatrolPathMarker marker)
        {
            string query = "INSERT INTO patrol_path (name, startingPoint, travelReverse, locX, locY, locZ, lingerTime) VALUES ";
            query += "('" + marker.name + "'," + marker.startingPoint + "," + marker.travelReverse + "," + marker.transform.position.x + ",";
            query += marker.transform.position.y + "," + marker.transform.position.z + "," + marker.lingerTime + ")";

            // Setup the register data		
            List<Register> update = new List<Register>();
            int itemID = -1;
            itemID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);
            marker.id = itemID;

            // If there is a next point, insert that and get the id, then save that as the nextPoint in the database
            if (marker.nextPoint != null)
            {
                if (marker.nextPoint.GetComponent<PatrolPathMarker>().id < 1)
                {
                    int nextPointID = InsertMarker(marker.nextPoint.GetComponent<PatrolPathMarker>());
                    query = "UPDATE patrol_path set nextPoint = " + nextPointID + " where id = " + marker.id;
                    DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);
                }
                else
                {
                    UpdateMarker(marker.nextPoint.GetComponent<PatrolPathMarker>());
                }
            }
            return itemID;
        }

        public static int UpdateMarker(PatrolPathMarker marker)
        {
            string query = "";
            int nextPointID = -1;
            // If there is a next point, insert that and get the id, then save that as the nextPoint in the database
            if (marker.nextPoint != null)
            {
                if (marker.nextPoint.GetComponent<PatrolPathMarker>().id < 1)
                {
                    nextPointID = InsertMarker(marker.nextPoint.GetComponent<PatrolPathMarker>());
                }
                else
                {
                    UpdateMarker(marker.nextPoint.GetComponent<PatrolPathMarker>());
                }
            }

            query = "UPDATE patrol_path";
            query += " SET travelReverse = " + marker.travelReverse + ", locX = " + marker.transform.position.x + ", locY = " + marker.transform.position.y;
            query += ", locZ = " + marker.transform.position.z + ", lingerTime = " + marker.lingerTime + ", nextPoint = " + nextPointID;
            query += " WHERE id=" + marker.id;

            // Setup the register data		
            List<Register> update = new List<Register>();
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);

            return marker.id;
        }

        public static void DeleteMarker(PatrolPathMarker marker)
        {
            string query = "UPDATE patrol_path";
            query += " SET isactive = 0";
            query += " WHERE id=" + marker.id;

            // Setup the register data		
            List<Register> update = new List<Register>();
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);

            // Update any spawns that are using this path
            query = "UPDATE spawn_data";
            query += " SET patrolPath = -1";
            query += " WHERE patrolPath=" + marker.id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);
        }
    }
}