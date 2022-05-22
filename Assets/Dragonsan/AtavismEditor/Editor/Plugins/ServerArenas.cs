using UnityEngine;
using UnityEditor;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Atavism
{
    // Class that implements the Instances configuration
    public class ServerArenas : AtavismDatabaseFunction
    {
        //   public Dictionary<int, Arena> dataRegister;
        public new Arena editingDisplay;
        public new Arena originalDisplay;

        public int[] accountIds = new int[] { 1 };
        public string[] accountList = new string[] { "~ First Account ~" };

        public static int[] arenaIds = new int[] { 1 };
        public static string[] arenaList = new string[] { "~ none ~" };

        public int[] instanceIds = new int[] { -1 };
        public string[] instanceOptions = new string[] { "~ none ~" };

        public int[] currencyIds = new int[] { -1 };
        public string[] currencyOptions = new string[] { "~ none ~" };
        // Database auxiliar table name
        //  private string portalTableName = "island_portals";

        private string arenaTeamsTableName = "arena_teams";
        //  private string victoryCurrencySearchInput = "";
        //  private string defeatCurrencySearchInput = "";

        // Use this for initialization
        public ServerArenas()
        {
            functionName = "Arena";
            // Database tables name
            tableName = "arena_templates";
            functionTitle = "Arena Configuration";
            loadButtonLabel = "Load Arenas";
            notLoadedText = "No Arena loaded.";
            // Init
        //    dataRegister = new Dictionary<int, Arena>();

            editingDisplay = new Arena();
            //   originalDisplay = new Arena();
            showRecovery = false;
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

        public void LoadInstanceOptions()
        {
            // Read all entries from the table
            string query = "SELECT id, island_name FROM instance_template WHERE islandType = 4";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.adminDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                instanceOptions = new string[rows.Count + 1];
                instanceOptions[optionsId] = "~ none ~";
                instanceIds = new int[rows.Count + 1];
                instanceIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    instanceOptions[optionsId] = data["id"] + ":" + data["island_name"];
                    instanceIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }
        
        public void LoadCurrencyOptions()
        {
            string query = "SELECT id, name FROM currencies where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
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
                    currencyOptions[optionsId] = data["id"] + ":" + data["name"];
                    currencyIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }
        public static void LoadArenaOptions()
        {
            string query = "SELECT id, name FROM arena_templates";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                arenaList = new string[rows.Count + 1];
                arenaList[optionsId] = "~ none ~";
                arenaIds = new int[rows.Count + 1];
                arenaIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    arenaList[optionsId] = data["id"] + ":" + data["name"];
                    arenaIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }



        // Load Database Data
        public override void Load()
        {
            if (!dataLoaded)
            {
                // Clean old data
             //   dataRegister.Clear();
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
               // dataRegister.Clear();
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


        // Load Database Data
        Arena LoadEntity(int id)
        {

            // Clean old data
         //   displayKeys.Clear();

            // Read all entries from the table
            string query = "SELECT `id`, `arenaType`, `name`, `arenaCategory`, `arenaInstanceID`, `length`, `defaultWinner`, `team1`, `team2`, `team3`, `team4`, `levelReq`, `levelMax`, `victoryCurrency`, `victoryPayment`, `defeatCurrency`, `defeatPayment`, `victoryExp`, `defeatExp`, `isactive`, `start_hour`, `start_minute`, `end_hour`, `end_minute`, `description` FROM " + tableName
                + " WHERE id = " + id;

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read all the data
            Arena display = new Arena();

            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    display.id = int.Parse(data["id"]);
                    display.Name = data["name"];
                    display.arenaType = int.Parse(data["arenaType"]);
                    display.arenaInstanceID = int.Parse(data["arenaInstanceID"]);
                    display.length = int.Parse(data["length"]);
                    display.defaultWinner = int.Parse(data["defaultWinner"]);
                    display.team1 = int.Parse(data["team1"]);
                    display.team2 = int.Parse(data["team2"]);
                    display.team3 = int.Parse(data["team3"]);
                    display.team4 = int.Parse(data["team4"]);
                    display.levelReq = int.Parse(data["levelReq"]);
                    display.levelMax = int.Parse(data["levelMax"]);
                    display.victoryCurrency = int.Parse(data["victoryCurrency"]);
                    display.victoryPayment = int.Parse(data["victoryPayment"]);
                    display.defeatCurrency = int.Parse(data["defeatCurrency"]);
                    display.defeatPayment = int.Parse(data["defeatPayment"]);
                    display.victoryExp = int.Parse(data["victoryExp"]);
                    display.defeatExp = int.Parse(data["defeatExp"]);
                    display.start_hour = int.Parse(data["start_hour"]);
                    display.start_minute = int.Parse(data["start_minute"]);
                    display.end_hour = int.Parse(data["end_hour"]);
                    display.end_minute = int.Parse(data["end_minute"]);
                    display.description = data["description"];
                    display.isLoaded = false;
                    //Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
                }
            }
            LoadArenaTeams(display);
            return display;
        }

        void LoadArenaTeams(Arena arenaEntry)
        {
            // Read all entries from the table
            string query = "SELECT id, name, size, goal, spawnX, spawnY, spawnZ FROM " + "arena_teams" + " where arenaID = " + arenaEntry.id;

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
                    ArenaTeamEntry entry = new ArenaTeamEntry();

                    entry.id = int.Parse(data["id"]);
                    entry.Name = data["name"];
                    entry.size = int.Parse(data["size"]);
                    entry.goal = int.Parse(data["goal"]);
                    entry.loc = new Vector3(float.Parse(data["spawnX"]), float.Parse(data["spawnY"]), float.Parse(data["spawnZ"]));
                    //	entry.orient = new Quaternion(int.Parse (data ["orientX"]), int.Parse (data ["orientY"]), int.Parse (data ["orientZ"]), int.Parse (data ["orientW"]));
                    arenaEntry.arenaTeams.Add(entry);
                }
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create an Arena before editing it."));
                return;
            }


            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Arena"));
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

                pos.width *= 2;
                pos.y += ImagePack.fieldHeight * 1.5f;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Arena Properties")+":");
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You dont have deleted arena."));
                return;
            }

            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Restore Arenas Configuration"));
            pos.y += ImagePack.fieldHeight;

            pos.width -= 140 + 155;
            for (int i = 0; i < displayList.Count; i++)
            {
                ImagePack.DrawText(pos, displayList[i]);
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos, Lang.GetTranslate("Delete Permanently")))
                {
                    //DeleteForever(displayKeys[i]);
                }
                pos.x += 155;
                if (ImagePack.DrawButton(pos, Lang.GetTranslate("Restore")))
                {
                    //RestoreEntry(displayKeys[i]);
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
            editingDisplay = new Arena();
            originalDisplay = new Arena();
            selectedDisplay = -1;
        }
        // Edit or Create a new instance
        public override void DrawEditor(Rect box, bool newInstance)
        {
            newEntity = newInstance;
            // Setup the layout
            Rect pos = box;
            pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;

            if (!linkedTablesLoaded)
            {
                LoadInstanceOptions();
                LoadCurrencyOptions();
                linkedTablesLoaded = true;
            }

            // Draw the content database info
            if (newEntity)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new arena"));
                pos.y += ImagePack.fieldHeight;
            }
            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.75f);

            pos.y += ImagePack.fieldHeight;
            editingDisplay.arenaType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Arena Type")+":", editingDisplay.arenaType, Arena.arenaTypes);
            pos.y += ImagePack.fieldHeight;
            int instanceID = GetOptionPosition(editingDisplay.arenaInstanceID, instanceIds);
            instanceID = ImagePack.DrawSelector(pos, Lang.GetTranslate("Instance")+":", instanceID, instanceOptions);
            editingDisplay.arenaInstanceID = instanceIds[instanceID];
            // editingDisplay.createOnStartup = ImagePack.DrawToggleBox (pos, "Create On Startup:", editingDisplay.createOnStartup);
            pos.y += 1.5f * ImagePack.fieldHeight;
            editingDisplay.length = ImagePack.DrawField(pos, Lang.GetTranslate("Arena Time Length")+":", editingDisplay.length);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.start_hour = ImagePack.DrawField(pos, Lang.GetTranslate("Arena Start Hour")+":", editingDisplay.start_hour);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.start_minute = ImagePack.DrawField(pos, Lang.GetTranslate("Arena Start Minute")+":", editingDisplay.start_minute);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.end_hour = ImagePack.DrawField(pos, Lang.GetTranslate("Arena End Hour")+":", editingDisplay.end_hour);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.end_minute = ImagePack.DrawField(pos, Lang.GetTranslate("Arena End Minute")+":", editingDisplay.end_minute);



            //	pos.y += ImagePack.fieldHeight;
            //	int selectedAccount = GetPositionOfAccount (editingDisplay.administrator);
            //	selectedAccount = ImagePack.DrawSelector (pos, "Admin Account:", selectedAccount, accountList);
            //	editingDisplay.administrator = accountIds [selectedAccount];
            pos.y += 1.5f * ImagePack.fieldHeight;
            editingDisplay.levelReq = ImagePack.DrawField(pos, Lang.GetTranslate("Minimum Level To Enter") +":", editingDisplay.levelReq);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.levelMax = ImagePack.DrawField(pos, Lang.GetTranslate("Maximum Level To Enter") +":", editingDisplay.levelMax);
            pos.y += 1.5f * ImagePack.fieldHeight;
            // pos.width /= 2;
            int selectedCurrency = GetOptionPosition(editingDisplay.victoryCurrency, currencyIds);
            //selectedCurrency = ImagePack.DrawSelector (pos, "Currency:", selectedCurrency, currencyOptions);
            selectedCurrency = ImagePack.DrawSelector(pos, Lang.GetTranslate("Victory Currency")+": ", selectedCurrency, currencyOptions);
            editingDisplay.victoryCurrency = currencyIds[selectedCurrency];
            //pos.x -= pos.width;
            //        pos.width *= 2;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.victoryPayment = ImagePack.DrawField(pos, Lang.GetTranslate("Victory Payment")+":", editingDisplay.victoryPayment);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.victoryExp = ImagePack.DrawField(pos, Lang.GetTranslate("Victory Experience")+":", editingDisplay.victoryExp);
            //pos.width /= 2;

            pos.y += 1.5f * ImagePack.fieldHeight;
            selectedCurrency = GetOptionPosition(editingDisplay.defeatCurrency, currencyIds);
            //selectedCurrency = ImagePack.DrawSelector (pos, "Currency:", selectedCurrency, currencyOptions);
            selectedCurrency = ImagePack.DrawSelector(pos, Lang.GetTranslate("Defeat Currency")+": ", selectedCurrency, currencyOptions);
            editingDisplay.defeatCurrency = currencyIds[selectedCurrency];
            //pos.x -= pos.width;
            //pos.width *= 2;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.defeatPayment = ImagePack.DrawField(pos, Lang.GetTranslate("Defeat Payment")+":", editingDisplay.defeatPayment);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.defeatExp = ImagePack.DrawField(pos, Lang.GetTranslate("Defeat Experience")+":", editingDisplay.defeatExp);
            pos.y += 1.5f * ImagePack.fieldHeight;
            //pos.width *= 2;

            editingDisplay.description = ImagePack.DrawField(pos, Lang.GetTranslate("Description")+":", editingDisplay.description, 0.75f, 60);
            pos.y += 2.5f * ImagePack.fieldHeight;
            //pos.width /= 2;
            ImagePack.DrawLabel(pos.x, pos.y, "Teams");
            pos.y += 1.5f * ImagePack.fieldHeight;
            if (editingDisplay.arenaTeams.Count == 0)
            {
                editingDisplay.arenaTeams.Add(new ArenaTeamEntry("TeamA", Vector3.zero));
                editingDisplay.arenaTeams.Add(new ArenaTeamEntry("TeamB", Vector3.zero));
            }
            for (int i = 0; i < editingDisplay.arenaTeams.Count; i++)
            {
                editingDisplay.arenaTeams[i].Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.arenaTeams[i].Name);
                pos.y += ImagePack.fieldHeight;
                editingDisplay.arenaTeams[i].size = ImagePack.DrawField(pos, Lang.GetTranslate("Team Size")+":", editingDisplay.arenaTeams[i].size);
                pos.y += ImagePack.fieldHeight;
                editingDisplay.arenaTeams[i].goal = ImagePack.DrawField(pos, Lang.GetTranslate("Team Goal")+":", editingDisplay.arenaTeams[i].goal);
                pos.y += ImagePack.fieldHeight;
                editingDisplay.arenaTeams[i].loc = ImagePack.Draw3DPosition(pos, Lang.GetTranslate("Location")+":", editingDisplay.arenaTeams[i].loc);
                pos.y += ImagePack.fieldHeight * 1.5f;

                editingDisplay.arenaTeams[i].MarkerObject = ImagePack.DrawGameObject(pos, Lang.GetTranslate("Game Object")+":", editingDisplay.arenaTeams[i].gameObject, 0.75f);
                if (i > 1)
                {
                    pos.y += ImagePack.fieldHeight;
                    pos.width /= 2;
                    pos.x += pos.width;
                    if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove Team")))
                    {
                        if (editingDisplay.arenaTeams[i].id > 0)
                            editingDisplay.itemsToBeDeleted.Add(editingDisplay.arenaTeams[i].id);
                        editingDisplay.arenaTeams.RemoveAt(i);
                    }
                    pos.x -= pos.width;
                    pos.width *= 2;
                }
                pos.y += 1.5f * ImagePack.fieldHeight;
            }

            if (editingDisplay.arenaTeams.Count < 2)
            {
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Team")))
                {
                    editingDisplay.arenaTeams.Add(new ArenaTeamEntry());
                }
            }

            // Save Instance data
            showSave = true;
      
            // Delete Instance data
            if (!newEntity)
            {
                showDelete = true;
            }
            else
            {
                showDelete = false;
            }

            // Cancel editing
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
            query += " (arenaType,  name,  arenaCategory,  arenaInstanceID,  length,  defaultWinner,  levelReq,  levelMax,  victoryCurrency,  victoryPayment,  defeatCurrency,  defeatPayment,  victoryExp, defeatExp, start_hour, start_minute, end_hour, end_minute, description) ";
            query += "VALUES ";
            query += "(?arenaType, ?name, ?arenaCategory, ?arenaInstanceID, ?length, ?defaultWinner, ?levelReq, ?levelMax, ?victoryCurrency, ?victoryPayment, ?defeatCurrency, ?defeatPayment, ?victoryExp, ?defeatExp, ?start_hour, ?start_minute, ?end_hour, ?end_minute, ?description) ";

            int arenaID = -1;

            // Setup the register data		
            List<Register> update = new List<Register>();
            update.Add(new Register("arenaType", "?arenaType", MySqlDbType.Int32, editingDisplay.arenaType.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("name", "?name", MySqlDbType.VarChar, editingDisplay.Name.ToString(), Register.TypesOfField.String));
            update.Add(new Register("arenaCategory", "?arenaCategory", MySqlDbType.Int32, editingDisplay.arenaCategory.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("arenaInstanceID", "?arenaInstanceID", MySqlDbType.Int32, editingDisplay.arenaInstanceID.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("length", "?length", MySqlDbType.Int32, editingDisplay.length.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("defaultWinner", "?defaultWinner", MySqlDbType.Int32, editingDisplay.defaultWinner.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("levelReq", "?levelReq", MySqlDbType.Int32, editingDisplay.levelReq.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("levelMax", "?levelMax", MySqlDbType.Int32, editingDisplay.levelMax.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("victoryCurrency", "?victoryCurrency", MySqlDbType.Int32, editingDisplay.victoryCurrency.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("victoryPayment", "?victoryPayment", MySqlDbType.Int32, editingDisplay.victoryPayment.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("defeatCurrency", "?defeatCurrency", MySqlDbType.Int32, editingDisplay.defeatCurrency.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("defeatPayment", "?defeatPayment", MySqlDbType.Int32, editingDisplay.defeatPayment.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("victoryExp", "?victoryExp", MySqlDbType.Int32, editingDisplay.victoryExp.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("defeatExp", "?defeatExp", MySqlDbType.Int32, editingDisplay.defeatExp.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("start_hour", "?start_hour", MySqlDbType.Int32, editingDisplay.start_hour.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("start_minute", "?start_minute", MySqlDbType.Int32, editingDisplay.start_minute.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("end_hour", "?end_hour", MySqlDbType.Int32, editingDisplay.end_hour.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("end_minute", "?end_minute", MySqlDbType.Int32, editingDisplay.end_minute.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("description", "?description", MySqlDbType.VarChar, editingDisplay.description.ToString(), Register.TypesOfField.String));

            // Update the database
            arenaID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);

            // If the insert failed, don't insert the spawn marker
            if (arenaID != -1)
            {
                //	int islandID = arenaID;
                int i = 1;
                foreach (ArenaTeamEntry entry in editingDisplay.arenaTeams)
                {
                    if (entry.Name != "")
                    {
                        entry.arenaID = arenaID;
                        InsertTeam(entry, i++);
                    }
                }

                // Update online table to avoid access the database again			
                editingDisplay.id = arenaID;
                editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + instanceID + "ID2:" + editingDisplay.id);
                // dataRegister.Add(editingDisplay.id, editingDisplay);
                //   displayKeys.Add(editingDisplay.id);
                dataLoaded = false;
                Load();
                newItemCreated = true;
                NewResult(Lang.GetTranslate("New entry inserted"));
            }
            else
            {
                NewResult(Lang.GetTranslate("Error occurred, please check the Console"));
            }
        }

        void InsertTeam(ArenaTeamEntry entry, int i)
        {
            string query = "INSERT INTO " + arenaTeamsTableName;
            query += " (arenaID, name, size, race, goal, spawnX, spawnY, spawnZ) ";
            query += "VALUES ";
            query += " (" + entry.arenaID + ",'" + entry.Name + "'," + entry.size + "," + entry.race + "," + entry.goal + "," + entry.loc.x + ","
                + entry.loc.y + "," + entry.loc.z + ") ";

            // Setup the register data
            List<Register> update = new List<Register>();
            foreach (string field in entry.fields.Keys)
            {
                update.Add(entry.fieldToRegister(field));
            }

            int teamID = -1;
            teamID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);

            entry.id = teamID;
            string query2 = "UPDATE " + tableName;
            query2 += " SET team" + i + " = ?team" + i + " WHERE id = ?id ";
            List<Register> update2 = new List<Register>();
            update2.Add(new Register("team" + i, "?team" + i, MySqlDbType.Int32, teamID.ToString(), Register.TypesOfField.Int));
            update2.Add(new Register("id", "?id", MySqlDbType.Int32, entry.arenaID.ToString(), Register.TypesOfField.Int));
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query2, update2);
        }

        // Update existing entries in the table based on the iddemo_table
        void UpdateEntry()
        {
            NewResult(Lang.GetTranslate("Updating..."));
            // Setup the update query
            string query = "UPDATE " + tableName;
            query += " SET name=?name,";
            query += " arenaType=?arenaType,";
            query += " arenaCategory=?arenaCategory,";
            query += " arenaInstanceID=?arenaInstanceID,";
            query += " length=?length,";
            query += " defaultWinner=?defaultWinner,";
            query += " levelReq=?levelReq,";
            query += " levelMax=?levelMax,";
            query += " victoryCurrency=?victoryCurrency,";
            query += " victoryPayment=?victoryPayment,";
            query += " defeatCurrency=?defeatCurrency,";
            query += " defeatPayment=?defeatPayment,";
            query += " victoryExp=?victoryExp,";
            query += " defeatExp=?defeatExp,";
            query += " start_hour=?start_hour,";
            query += " start_minute=?start_minute,";
            query += " end_hour=?end_hour,";
            query += " end_minute=?end_minute,";
            query += " description=?description";
            query += " WHERE id=?id";
            // Setup the register data		
            List<Register> update = new List<Register>();
            update.Add(new Register("arenaType", "?arenaType", MySqlDbType.Int16, editingDisplay.arenaType.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("name", "?name", MySqlDbType.VarChar, editingDisplay.Name.ToString(), Register.TypesOfField.String));
            update.Add(new Register("arenaCategory", "?arenaCategory", MySqlDbType.Int32, editingDisplay.arenaCategory.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("arenaInstanceID", "?arenaInstanceID", MySqlDbType.Int32, editingDisplay.arenaInstanceID.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("length", "?length", MySqlDbType.Int32, editingDisplay.length.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("defaultWinner", "?defaultWinner", MySqlDbType.Int32, editingDisplay.defaultWinner.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("levelReq", "?levelReq", MySqlDbType.Int32, editingDisplay.levelReq.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("levelMax", "?levelMax", MySqlDbType.Int32, editingDisplay.levelMax.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("victoryCurrency", "?victoryCurrency", MySqlDbType.Int32, editingDisplay.victoryCurrency.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("victoryPayment", "?victoryPayment", MySqlDbType.Int32, editingDisplay.victoryPayment.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("defeatCurrency", "?defeatCurrency", MySqlDbType.Int32, editingDisplay.defeatCurrency.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("defeatPayment", "?defeatPayment", MySqlDbType.Int32, editingDisplay.defeatPayment.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("victoryExp", "?victoryExp", MySqlDbType.Int32, editingDisplay.victoryExp.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("defeatExp", "?defeatExp", MySqlDbType.Int32, editingDisplay.defeatExp.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("start_hour", "?start_hour", MySqlDbType.Int32, editingDisplay.start_hour.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("start_minute", "?start_minute", MySqlDbType.Int32, editingDisplay.start_minute.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("end_hour", "?end_hour", MySqlDbType.Int32, editingDisplay.end_hour.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("end_minute", "?end_minute", MySqlDbType.Int32, editingDisplay.end_minute.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("description", "?description", MySqlDbType.VarChar, editingDisplay.description.ToString(), Register.TypesOfField.String));
            update.Add(new Register("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString(), Register.TypesOfField.Int));

            // Update the database
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);

            // Insert/Update the abilities
            int i = 1;
            foreach (ArenaTeamEntry entry in editingDisplay.arenaTeams)
            {
                if (entry.Name != "")
                {
                    if (entry.id < 1)
                    {
                        // This is a new entry, insert it
                        entry.arenaID = editingDisplay.id;
                        InsertTeam(entry, i++);
                    }
                    else
                    {
                        // This is an existing entry, update it
                        entry.arenaID = editingDisplay.id;
                        UpdateTeam(entry, i++);
                    }
                }
            }

            // Delete any abilities that are tagged for deletion
            foreach (int teamID in editingDisplay.itemsToBeDeleted)
            {
                DeleteTeam(teamID);
            }

            // Update online table to avoid access the database again			
            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry") + "  " + editingDisplay.Name + " " + Lang.GetTranslate("updated"));
           Load();
         }

        void UpdateTeam(ArenaTeamEntry entry, int i)
        {
            string query = "UPDATE " + arenaTeamsTableName;
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

            /*   string query2 = "UPDATE " + tableName;
               query2 += " SET team" + i + "=?team" + i + " WHERE id=?id";
               List<Register> update2 = new List<Register>();
               update2.Add(new Register("team" + i, "?team" + i, MySqlDbType.Int24, editingDisplay.name.ToString(), Register.TypesOfField.Int));
               update2.Add(new Register("id", "?id", MySqlDbType.Int32, entry.arenaID.ToString(), Register.TypesOfField.Int));
               DatabasePack.Update(DatabasePack.contentDatabasePrefix, query2, update2);*/
        }

        void DeleteTeam(int portalID)
        {
            Register delete = new Register("id", "?id", MySqlDbType.Int32, portalID.ToString(), Register.TypesOfField.Int);
            DatabasePack.Delete(DatabasePack.contentDatabasePrefix, arenaTeamsTableName, delete);
        }

        // Delete entries from the table
        void DeleteEntry()
        {
            Register delete = new Register("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString(), Register.TypesOfField.Int);
            DatabasePack.Delete(DatabasePack.contentDatabasePrefix, tableName, delete);
            delete = new Register("arenaID", "?arenaID", MySqlDbType.Int32, editingDisplay.id.ToString(), Register.TypesOfField.Int);
            DatabasePack.Delete(DatabasePack.contentDatabasePrefix, arenaTeamsTableName, delete);

            // Update online table to avoid access the database again			
            selectedDisplay = -1;
            newSelectedDisplay = 0;

            dataLoaded = false;
             NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " "+ Lang.GetTranslate("Deleted"));
           Load();
        }

        private int GetPositionOfAccount(int accountId)
        {
            for (int i = 0; i < accountIds.Length; i++)
            {
                if (accountIds[i] == accountId)
                    return i;
            }
            return 0;
        }

        public static int GetArenaID(string arenaName)
        {
            string query = "SELECT id FROM arena_template where name = '" + arenaName + "'";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            //int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    return int.Parse(data["id"]);
                }
            }
            return -1;
        }

    }
}