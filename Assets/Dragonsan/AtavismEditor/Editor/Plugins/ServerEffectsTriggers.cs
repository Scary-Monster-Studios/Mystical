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
    public class ServerEffectsTriggers : AtavismDatabaseFunction
    {

        public new Dictionary<int, EffectsTriggerData> dataRegister;
        public new EffectsTriggerData editingDisplay;
        public new EffectsTriggerData originalDisplay;

        public string[] raceOptions = new string[] { "~ none ~" };
        public string[] classOptions = new string[] { "~ none ~" };
        public int[] raceIds = new int[] { -1 };
        public int[] classIds = new int[] { -1 };

        public int[] abilityIds = new int[] { -1 };
        public string[] abilityOptions = new string[] { "~ none ~" };

        public int[] tagIds = new int[] { -1 };
        public string[] tagOptions = new string[] { "~ none ~" };

        public int[] abilityTagIds = new int[] { -1 };
        public string[] abilityTagOptions = new string[] { "~ none ~" };

        public int[] effectIds = new int[] { -1 };
        public string[] effectOptions = new string[] { "~ none ~" };
        

        // Use this for initialization
        public ServerEffectsTriggers()
        {
            functionName = "Effects Triggers Profile ";
            // Database tables name
            tableName = "effects_triggers";
            functionTitle = "Effects Triggers Profile Configuration";
            loadButtonLabel = "Load Effects Triggers Profile";
            notLoadedText = "No Effects Triggers Profile loaded.";
            // Init
            dataRegister = new Dictionary<int, EffectsTriggerData>();

            editingDisplay = new EffectsTriggerData();
            originalDisplay = new EffectsTriggerData();
        }
        void resetSearch(bool fokus)
        {
           
            if (fokus)
                GUI.FocusControl(null);
        }
        public override void Activate()
        {
            linkedTablesLoaded = false;
            resetSearch(true);
        }



        public void LoadEffectOptions()
        {

            // Read all entries from the table
            string query = "SELECT id, name FROM effects where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            int optionsId = -1;
            if ((rows != null) && (rows.Count > 0))
            {
                effectOptions = new string[rows.Count ];
                //effectOptions[optionsId] = "~ none ~";
                effectIds = new int[rows.Count ];
                //effectIds[optionsId] = 0;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    effectOptions[optionsId] = data["name"];
                    effectIds[optionsId] = int.Parse(data["id"]);
                }
            }

        }

        public void LoadAbilityOptions()
        {
            string query = "SELECT id, name FROM abilities where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = -1;
            if ((rows != null) && (rows.Count > 0))
            {
                abilityOptions = new string[rows.Count ];
                //abilityOptions[optionsId] = "~ none ~";
                abilityIds = new int[rows.Count ];
                //abilityIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    abilityOptions[optionsId] = data["id"] + ":" + data["name"];
                    abilityIds[optionsId] = int.Parse(data["id"]);
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
                        displayList.Add(data["name"]);
                        displayKeys.Add(int.Parse(data["id"]));

                    }
                }
                dataLoaded = true;
            }
        }

        EffectsTriggerData LoadEntity(int id)
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
            EffectsTriggerData display = new EffectsTriggerData();
            int fakeId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    //foreach(string key in data.Keys)
                    //	Debug.Log("Name[" + key + "]:" + data[key]);
                    //return;
                    // As we don have a primary key ID field
                    //  fakeId++;
                    display.id = id;
                    display.Name = data["name"];
                    display.eventType = int.Parse(data["event_type"]);

                    display.Race = int.Parse(data["race"]);
                    display.Class = int.Parse(data["class"]);
                    display.chance_min = float.Parse(data["chance_min"]);
                    display.chance_max = float.Parse(data["chance_max"]);
                    display.actionType = int.Parse(data["action_type"]);
                    //                    display.target = int.Parse(data["target"]);
                    if (data["tags"].Length > 0)
                    {
                        string[] splitted = data["tags"].Split(';');
                        for (int i = 0; i < splitted.Length; i++)
                        {
                            display.tags.Add(int.Parse(splitted[i]));
                            display.tagSearch.Add("");

                        }
                    }
                    display.isLoaded = true;
                    //Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
                }
            }
            LoadActions(display);
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
                        displayList.Add(data["name"]);
                        displayKeys.Add(int.Parse(data["id"]));

                    }
                }
                dataRestoreLoaded = true;
            }
        }

        void LoadActions(EffectsTriggerData etdata)
        {
            // Read all entries from the table
            string query = "SELECT * FROM effects_triggers_actions where effects_triggers_id = " + etdata.id;
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
                    TriggerActionEntry display = new TriggerActionEntry();
                    display.id = int.Parse(data["id"]);
                    display.effects_triggers_id = int.Parse(data["effects_triggers_id"]);
                    display.ability = int.Parse(data["ability"]);

                    display.effect = int.Parse(data["effect"]);
                    if (display.ability > 0)
                    {
                        display.actionType = 0;
                    }
                    else if (display.effect > 0)
                    {
                        display.actionType = 1;
                    }
                    else
                    {
                        display.actionType = 2;
                    }
                    display.mod_v = int.Parse(data["mod_v"]);
                    display.mod_p = float.Parse(data["mod_p"]);
                    display.chance_min = float.Parse(data["chance_min"]);
                    display.chance_max = float.Parse(data["chance_max"]);
                    display.target = int.Parse(data["target"]);

                    etdata.actions.Add(display);
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create an Effect Trigger before editing it."));
                return;
            }

            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Effect Trigger"));

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
                if (selectedDisplay >= 0 /*&& displayKeys.Contains(selectedDisplay)*/)
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
                newSelectedDisplay = ImagePack.DrawDynamicPartialListSelector(pos, Lang.GetTranslate("Search Filter") + ": ", ref entryFilterInput, selectedDisplay, displayList);

                pos.width *= 2;
                pos.y += ImagePack.fieldHeight * 1.5f;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Effect Trigger Properties") + ":");
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You dont have deleted") + " " + Lang.GetTranslate(functionName) + ".");
                return;
            }

            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Restore") + " " + Lang.GetTranslate(functionName) + " " + Lang.GetTranslate("Configuration"));
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
            editingDisplay = new EffectsTriggerData();
            originalDisplay = new EffectsTriggerData();
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
                LoadEffectOptions();
                LoadAbilityOptions();
                ServerOptionChoices.LoadAtavismChoiceOptions("Race", true, out raceIds, out raceOptions);
                ServerOptionChoices.LoadAtavismChoiceOptions("Class", true, out classIds, out classOptions);
                ServerOptionChoices.LoadAtavismChoiceOptions("Effects Tags", false, out tagIds, out tagOptions);
                ServerOptionChoices.LoadAtavismChoiceOptions("Ability Tags", false, out abilityTagIds, out abilityTagOptions);

                linkedTablesLoaded = true;
            }

            // Draw the content database info

            if (newEntity)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new Effects Trigger Profile"));
                pos.y += ImagePack.fieldHeight;
            }

            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name") + ":", editingDisplay.Name, 0.75f);
            pos.y += ImagePack.fieldHeight;
            pos.width /= 2;
            editingDisplay.eventType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Event Type") + ":", editingDisplay.eventType, editingDisplay.eventTypeOptions);
            pos.x += pos.width;
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.chance_min = ImagePack.DrawField(pos, Lang.GetTranslate("Chance Min %") + ":", editingDisplay.chance_min);
            pos.x += pos.width;
            editingDisplay.chance_max = ImagePack.DrawField(pos, Lang.GetTranslate("Chance Max %") + ":", editingDisplay.chance_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            int selectedRace = GetOptionPosition(editingDisplay.Race, raceIds);
            selectedRace = ImagePack.DrawSelector(pos, Lang.GetTranslate("Race") + ":", selectedRace, raceOptions);
            editingDisplay.Race = raceIds[selectedRace];
            pos.x += pos.width;
            int selectedClass = GetOptionPosition(editingDisplay.Class, classIds);
            selectedClass = ImagePack.DrawSelector(pos, Lang.GetTranslate("Class") + ":", selectedClass, classOptions);
            editingDisplay.Class = classIds[selectedClass];
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.actionType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Action Type") + ":", editingDisplay.actionType, editingDisplay.actionTypeOptions);
            pos.x += pos.width;
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            if (editingDisplay.eventType == 0 || editingDisplay.eventType == 1 || editingDisplay.eventType == 6)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Ability Tags"));
            }
            else
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Effect Tags"));
            }
            //   pos.y += ImagePack.fieldHeight;
            for (int j = 0; j < editingDisplay.tags.Count; j++)
            {
                pos.y += ImagePack.fieldHeight;
                if (editingDisplay.eventType == 0 || editingDisplay.eventType == 1 || editingDisplay.eventType == 6)
                {
                    int selectedTag = GetOptionPosition(editingDisplay.tags[j], abilityTagIds);
                    string search = editingDisplay.tagSearch[j];
                    selectedTag = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Ability Tag ") + (j + 1) + ":", ref search, selectedTag, abilityTagOptions);
                    editingDisplay.tagSearch[j] = search;
                    editingDisplay.tags[j] = abilityTagIds[selectedTag];

                }
                else
                {
                    int selectedTag = GetOptionPosition(editingDisplay.tags[j], tagIds);
                    string search = editingDisplay.tagSearch[j];
                    selectedTag = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Effects Tag ") + (j + 1) + ":", ref search, selectedTag, tagOptions);
                    editingDisplay.tagSearch[j] = search;
                    editingDisplay.tags[j] = tagIds[selectedTag];
                }

                pos.x += pos.width;
                pos.y += ImagePack.fieldHeight;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Tag")))
                {

                    editingDisplay.tags.RemoveAt(j);
                    editingDisplay.tagSearch.RemoveAt(j);
                }
                pos.x -= pos.width;
            }
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Tag")))
            {
                editingDisplay.tags.Add(0);
                editingDisplay.tagSearch.Add("");
            }

            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Actions"));
            pos.y += 1.5f * ImagePack.fieldHeight;
            if (editingDisplay.actions.Count == 0)
            {
                editingDisplay.actions.Add(new TriggerActionEntry(0, 0, 0, 100));
            }
            for (int i = 0; i < editingDisplay.actions.Count; i++)
            {
                ImagePack.DrawText(pos.x, pos.y, Lang.GetTranslate("Action") + " " + (i + 1));
                pos.y += ImagePack.fieldHeight;
                if (editingDisplay.eventType != 2 && editingDisplay.eventType != 3)
                    editingDisplay.actions[i].actionType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Action Type") + ":", editingDisplay.actions[i].actionType, editingDisplay.abilityActionTypeOptions);
                else
                    editingDisplay.actions[i].actionType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Action Type") + ":", editingDisplay.actions[i].actionType, editingDisplay.effectActionTypeOptions);
                pos.y += ImagePack.fieldHeight;
                if (editingDisplay.actions[i].actionType != 2)
                {
                    editingDisplay.actions[i].target = ImagePack.DrawSelector(pos, Lang.GetTranslate("Target") + ":", editingDisplay.actions[i].target, editingDisplay.targetOptions);
                    pos.y += ImagePack.fieldHeight;
                }

                if (editingDisplay.actions[i].actionType == 0)
                {
                    int selectedAbility = GetOptionPosition(editingDisplay.actions[i].ability, abilityIds);
                    selectedAbility = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Ability") + ": ", ref editingDisplay.actions[i].abilitySearch, selectedAbility, abilityOptions);
                    editingDisplay.actions[i].ability = abilityIds[selectedAbility];
                    editingDisplay.actions[i].effect = -1;
                }
                else if (editingDisplay.actions[i].actionType == 1)
                {

                    int selectedEffect = GetOptionPosition(editingDisplay.actions[i].effect, effectIds);
                    selectedEffect = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Effect") + ": ", ref editingDisplay.actions[i].effectSearch, selectedEffect, effectOptions);
                    editingDisplay.actions[i].effect = effectIds[selectedEffect];
                    editingDisplay.actions[i].ability = -1;
                }
                else
                {
                    editingDisplay.actions[i].ability = -1;
                    editingDisplay.actions[i].effect = -1;
                    editingDisplay.actions[i].mod_v = ImagePack.DrawField(pos, Lang.GetTranslate("Modifier Value") + ":", editingDisplay.actions[i].mod_v);
                    pos.x += pos.width;
                    editingDisplay.actions[i].mod_p = ImagePack.DrawField(pos, Lang.GetTranslate("Modifier %") + ":", editingDisplay.actions[i].mod_p);
                    pos.x -= pos.width;
                }
                pos.y += ImagePack.fieldHeight;

                editingDisplay.actions[i].chance_min = ImagePack.DrawField(pos, Lang.GetTranslate("Chance Min") + ":", editingDisplay.actions[i].chance_min);
                pos.x += pos.width;
                editingDisplay.actions[i].chance_max = ImagePack.DrawField(pos, Lang.GetTranslate("Chance Max") + ":", editingDisplay.actions[i].chance_max);
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove Action")))
                {
                    if (editingDisplay.actions[i].id > 0)
                        editingDisplay.actionsToBeDeleted.Add(editingDisplay.actions[i].id);
                    editingDisplay.actions.RemoveAt(i);
                }
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
            }
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Action")))
            {
                editingDisplay.actions.Add(new TriggerActionEntry(0, 0, 0, 100));
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
                EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight + 100);
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

            // If the insert succeeded
            if (itemID != -1)
            {
                // Update online table to avoid access the database again			
                editingDisplay.id = dataRegister.Count + 1; // Set the highest free index ;
                                                            // Insert the Links
                foreach (TriggerActionEntry entry in editingDisplay.actions)
                {
                    entry.effects_triggers_id = itemID;
                    InsertAction(entry);
                }
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


        void InsertAction(TriggerActionEntry entry)
        {
            string query = "INSERT INTO effects_triggers_actions";
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
            query += " WHERE id=" + editingDisplay.id;

            // Setup the register data		
            List<Register> update = new List<Register>();
            foreach (string field in editingDisplay.fields.Keys)
            {
                update.Add(editingDisplay.fieldToRegister(field));
            }

            // Update the database
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);

            foreach (TriggerActionEntry entry in editingDisplay.actions)
            {
                {
                    if (entry.id < 1)
                    {
                        // This is a new entry, insert it
                        entry.effects_triggers_id = editingDisplay.id;
                        InsertAction(entry);
                    }
                    else
                    {
                        // This is an existing entry, update it
                        UpdateAction(entry);
                    }
                }
            }

            // Delete any links that are tagged for deletion
            foreach (int linkID in editingDisplay.actionsToBeDeleted)
            {
                DeleteAction(linkID);
            }

            // Update online table to avoid access the database again			

            dataLoaded = false;

            NewResult(Lang.GetTranslate("Entry") + " " + editingDisplay.Name + " " + Lang.GetTranslate("updated"));
            Load();
        }

        void UpdateAction(TriggerActionEntry entry)
        {
            string query = "UPDATE effects_triggers_actions";
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

        void DeleteAction(int linkID)
        {
            string query = "UPDATE effects_triggers_actions SET isactive = 0 where id = " + linkID;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
        }

        // Delete entries from the table
        void DeleteEntry()
        {
            //Check Loot Table
            string sqlEffects = "SELECT id FROM `effects` WHERE effectMainType  like 'Trigger' AND ";
                sqlEffects += " stringValue1" + " like '%" + editingDisplay.id + "%' ";
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

            DatabasePack.SaveLog("Delete Effect Trigger id =" + editingDisplay.id + " assigned to " + effectsIds);

            if (effectsCount == 0 )
            {
                string query = "UPDATE " + tableName + " SET isactive = 0 where id = " + editingDisplay.id;
                DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

                // Update online table to avoid access the database again		
                selectedDisplay = -1;
                newSelectedDisplay = 0;
                dataLoaded = false;
                NewResult(Lang.GetTranslate("Entry") + " " + editingDisplay.Name + " " + Lang.GetTranslate("Deleted"));
                Load();
            }
            else
            {
                EditorUtility.DisplayDialog("", Lang.GetTranslate("You can not delete Effect Trigger") + " " + editingDisplay.Name + " " + Lang.GetTranslate("because it is assigned in") + ":" + effectsIds , Lang.GetTranslate("OK"), "");
            }
        }

        void DeleteForever(int id)
        {
            if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete") + " " + Lang.GetTranslate(functionName) + " " + Lang.GetTranslate("entry") + " ?", Lang.GetTranslate("Are you sure you want to permanent delete") + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
            {
                NewResult(Lang.GetTranslate("Deleting..."));
                List<Register> where = new List<Register>();
                where.Add(new Register("id", "?id", MySqlDbType.String, id.ToString(), Register.TypesOfField.String));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, tableName, where);

                where.Clear();
                where.Add(new Register("effects_triggers_id", "?effects_triggers_id", MySqlDbType.String, id.ToString(), Register.TypesOfField.String));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "effects_triggers_actions", where);

                dataLoaded = false;
                dataRestoreLoaded = false;
                // Load();
                NewResult(Lang.GetTranslate("Entry Permanent Deleted"));
            }
        }


        void RestoreEntry(int id)
        {
            NewResult(Lang.GetTranslate("Restoring..."));
            string query = "UPDATE " + tableName + " SET isactive = 1 where id = " + id ;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            dataLoaded = false;
            dataRestoreLoaded = false;
            //Load();
            NewResult(Lang.GetTranslate("Entry Restored"));
        }

        private int GetPositionOfEffect(int effectID)
        {
            for (int i = 0; i < effectIds.Length; i++)
            {
                if (effectIds[i] == effectID)
                    return i;
            }
            return 0;
        }
    }
}