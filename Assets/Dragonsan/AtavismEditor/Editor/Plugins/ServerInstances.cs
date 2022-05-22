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
    // Class that implements the Instances configuration
    public class ServerInstances : AtavismDatabaseFunction
    {
        public new Dictionary<int, Instance> dataRegister;
        public new Instance editingDisplay;
        public new Instance originalDisplay;

        public int[] accountIds = new int[] { 1 };
        public string[] accountList = new string[] { "~ First Account ~" };

        public static int[] instanceIds = new int[] { 1 };
        public static string[] instanceList = new string[] { "~ none ~" };
        public static GUIContent[] GuiInstanceList = new GUIContent[] { new GUIContent("~ none ~") };

        public static int[] GuiInstanceSpawnIds = new int[] { 1 };
        public static GUIContent[] GuiInstanceSpawnList = new GUIContent[] { new GUIContent("~ none ~") };
        public static Vector3[] GuiInstanceSpawnLoc = new Vector3[] { Vector3.zero };


        // Database auxiliar table name
        private string portalTableName = "island_portals";
        private string weatherInstanceTableName = "weather_instance";
        private string weatherSeasonTableName = "weather_season";
        //  private string accountSearch = "";

        public static int[] instanceWeatherProfileIds = new int[] { 1 };
        public static string[] instanceWeatherProfileList = new string[] { "~ none ~" };


        // Use this for initialization
        public ServerInstances()
        {
            functionName = "Instances";
            // Database tables name
            tableName = "instance_template";
            functionTitle = "Instance Configuration";
            loadButtonLabel = "Load Instances";
            notLoadedText = "No Instance loaded.";
            // Init
            dataRegister = new Dictionary<int, Instance>();

            editingDisplay = new Instance();
            originalDisplay = new Instance();
            //    accountSearch = "";
            showRecovery = false;
        }

        public override void Activate()
        {
            linkedTablesLoaded = false;
            showRecovery = false;
            resetSearch(true);
        }
        void resetSearch(bool fokus)
        {
           if(fokus) GUI.FocusControl(null);
        }
        private void LoadAccountList()

        {
            string query = "SELECT id, username FROM account";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.adminDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                accountList = new string[rows.Count];
                accountIds = new int[rows.Count];
                foreach (Dictionary<string, string> data in rows)
                {
                    accountList[optionsId] = data["id"] + ":" + data["username"];
                    accountIds[optionsId] = int.Parse(data["id"]);
                    optionsId++;
                }
            }
            else
            {
                accountList = new string[1];
                accountList[optionsId] = "~ First Account ~";
                accountIds = new int[1];
                accountIds[optionsId] = 1;
            }
        }

        public static void LoadInstanceOptions()
        {
            string query = "SELECT id, island_name FROM instance_template";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.adminDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                instanceList = new string[rows.Count + 1];
                instanceList[optionsId] = "~ none ~";
                instanceIds = new int[rows.Count + 1];
                instanceIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    instanceList[optionsId] = data["id"] + ":" + data["island_name"];
                    instanceIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }
        public static void LoadInstanceOptions(bool gui)
        {
            if (!gui)
            {
                LoadInstanceOptions();
                return;
            }
            string query = "SELECT id, island_name FROM instance_template";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.adminDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                GuiInstanceList = new GUIContent[rows.Count + 1];
                GuiInstanceList[optionsId] = new GUIContent("~ none ~");
                instanceIds = new int[rows.Count + 1];
                instanceIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    GuiInstanceList[optionsId] = new GUIContent(data["id"] + ":" + data["island_name"]);
                    instanceIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }


        public static void LoadInstancePortals(int instanceId)
        {
            // Read all entries from the table
            string query = "SELECT * FROM " + "island_portals" + " where island = " + instanceId;

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.adminDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                GuiInstanceSpawnList = new GUIContent[rows.Count + 1];
                GuiInstanceSpawnList[optionsId] = new GUIContent("~ none ~");
                GuiInstanceSpawnIds = new int[rows.Count + 1];
                GuiInstanceSpawnIds[optionsId] = -1;
                GuiInstanceSpawnLoc = new Vector3[rows.Count + 1] ;
                GuiInstanceSpawnLoc[optionsId] = Vector3.zero;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;

                    GuiInstanceSpawnIds[optionsId] = int.Parse(data["id"]);
                    GuiInstanceSpawnList[optionsId] = new GUIContent(data["name"]);
                    GuiInstanceSpawnLoc[optionsId] = new Vector3(float.Parse(data["locX"]), float.Parse(data["locY"]), float.Parse(data["locZ"]));
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
                string query = "SELECT id, island_name FROM " + tableName;// + " where isactive = 1";

                // If there is a row, clear it.
                if (rows != null)
                    rows.Clear();

                // Load data
                rows = DatabasePack.LoadData(DatabasePack.adminDatabasePrefix, query);
                //Debug.Log("#Rows:"+rows.Count);
                // Read all the data
                displayList.Clear();
                if ((rows != null) && (rows.Count > 0))
                {
                    foreach (Dictionary<string, string> data in rows)
                    {
                        displayList.Add(data["id"] + ". " + data["island_name"]);
                        displayKeys.Add(int.Parse(data["id"]));

                    }
                }
                dataLoaded = true;
            }
        }

        // Load Database Data
        Instance LoadEntity(int id)
        {
         
                // Read all entries from the table
                string query = "SELECT id, island_name, islandType, createOnStartup, globalWaterHeight, administrator, populationLimit FROM " + tableName+" WHERE id="+id;

                // If there is a row, clear it.
                if (rows != null)
                    rows.Clear();

                // Load data
                rows = DatabasePack.LoadData(DatabasePack.adminDatabasePrefix, query);
            Instance display = new Instance();

            // Read all the data
            if ((rows != null) && (rows.Count > 0))
                {
                    foreach (Dictionary<string, string> data in rows)
                    {
                     //   Instance display = new Instance();
                        display.id = int.Parse(data["id"]);
                        display.Name = data["island_name"];
                        display.islandType = int.Parse(data["islandType"]);
                        display.createOnStartup = bool.Parse(data["createOnStartup"]);
                        display.globalWaterHeight = float.Parse(data["globalWaterHeight"]);
                        display.administrator = int.Parse(data["administrator"]);
                        display.populationLimit = int.Parse(data["populationLimit"]);
                        display.isLoaded = false;
                      }
                }
                      LoadInstancePortals(display);
                    LoadInstanceWeatherPrfiles(display);
                    LoadWeatherSeasonsPrfiles(display);
              return display;
        }

        void LoadInstancePortals(Instance instanceEntry)
        {
            // Read all entries from the table
            string query = "SELECT * FROM " + "island_portals" + " where island = " + instanceEntry.id;

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.adminDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    InstancePortalEntry entry = new InstancePortalEntry();

                    entry.id = int.Parse(data["id"]);
                    entry.Name = data["name"];
                    entry.loc = new Vector3(float.Parse(data["locX"]), float.Parse(data["locY"]), float.Parse(data["locZ"]));
                    entry.orient = new Quaternion(int.Parse(data["orientX"]), int.Parse(data["orientY"]), int.Parse(data["orientZ"]), int.Parse(data["orientW"]));
                    instanceEntry.instancePortals.Add(entry);
                }
            }
        }

        void LoadInstanceWeatherPrfiles(Instance instanceEntry)
        {
            // Read all entries from the table
            string query = "SELECT * FROM " + weatherInstanceTableName + " where instance_id = " + instanceEntry.id;

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
                    WeatherInstace entry = new WeatherInstace();

                    entry.id = int.Parse(data["id"]);
                    entry.instance_id = int.Parse(data["instance_id"]);
                    entry.weather_profile_id = int.Parse(data["weather_profile_id"]);
                    entry.month1 = bool.Parse(data["month1"]);
                    entry.month2 = bool.Parse(data["month2"]);
                    entry.month3 = bool.Parse(data["month3"]);
                    entry.month4 = bool.Parse(data["month4"]);
                    entry.month5 = bool.Parse(data["month5"]);
                    entry.month6 = bool.Parse(data["month6"]);
                    entry.month7 = bool.Parse(data["month7"]);
                    entry.month8 = bool.Parse(data["month8"]);
                    entry.month9 = bool.Parse(data["month9"]);
                    entry.month10 = bool.Parse(data["month10"]);
                    entry.month11 = bool.Parse(data["month11"]);
                    entry.month12 = bool.Parse(data["month12"]);
                    entry.priority = int.Parse(data["priority"]);
                    instanceEntry.instanceWeatherProfiles.Add(entry);
                }
            }
        }
        void LoadWeatherSeasonsPrfiles(Instance instanceEntry)
        {
            // Read all entries from the table
            string query = "SELECT * FROM " + weatherSeasonTableName + " where instance_id = " + instanceEntry.id;

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
                    WeatherSeasion entry = new WeatherSeasion();

                    entry.id = int.Parse(data["id"]);
                    entry.instance_id = int.Parse(data["instance_id"]);
                    entry.season = int.Parse(data["season"]);
                    entry.month1 = bool.Parse(data["month1"]);
                    entry.month2 = bool.Parse(data["month2"]);
                    entry.month3 = bool.Parse(data["month3"]);
                    entry.month4 = bool.Parse(data["month4"]);
                    entry.month5 = bool.Parse(data["month5"]);
                    entry.month6 = bool.Parse(data["month6"]);
                    entry.month7 = bool.Parse(data["month7"]);
                    entry.month8 = bool.Parse(data["month8"]);
                    entry.month9 = bool.Parse(data["month9"]);
                    entry.month10 = bool.Parse(data["month10"]);
                    entry.month11 = bool.Parse(data["month11"]);
                    entry.month12 = bool.Parse(data["month12"]);
                    instanceEntry.instanceWeatherSeasons.Add(entry);
                }
            }
        }


        public void LoadInstanceWeatherProfilesOptions()
        {
            // Read all entries from the table
            string query = "SELECT id, name FROM weather_profile ";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //   Debug.LogError("#Rows:"+rows.Count);
            // Read all the data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                instanceWeatherProfileList = new string[rows.Count + 1];
                instanceWeatherProfileList[optionsId] = "~ none ~";
                instanceWeatherProfileIds = new int[rows.Count + 1];
                instanceWeatherProfileIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    instanceWeatherProfileList[optionsId] = data["id"] + ":" + data["name"];
                    instanceWeatherProfileIds[optionsId] = int.Parse(data["id"]);
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create an Instance before editing it."));
                return;
            }


            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Instance"));

            if (newItemCreated)
            {
                newItemCreated = false;
              //  LoadSelectList();
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Instance Properties")+":");
                pos.y += ImagePack.fieldHeight * 0.75f;
            }

            DrawEditor(pos, false);

            pos.y -= ImagePack.fieldHeight;
            //pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
        }

        public override void CreateNewData()
        {
            editingDisplay = new Instance();
            originalDisplay = new Instance();
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
                LoadAccountList();
                LoadInstanceWeatherProfilesOptions();
                linkedTablesLoaded = true;
            }

            // Draw the content database info
            if (newEntity)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new instance"));
                pos.y += ImagePack.fieldHeight;
            }
            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.75f);

            pos.y += ImagePack.fieldHeight;
            editingDisplay.islandType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Instance Type")+":", editingDisplay.islandType, Instance.islandTypes);
            pos.y += ImagePack.fieldHeight;
            if (editingDisplay.islandType == 0)
                editingDisplay.createOnStartup = true;

            editingDisplay.createOnStartup = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Create On Startup")+":", editingDisplay.createOnStartup);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.globalWaterHeight = ImagePack.DrawField(pos, Lang.GetTranslate("Water Height")+":", editingDisplay.globalWaterHeight);
            pos.y += ImagePack.fieldHeight;
            pos.width /= 2;
            //   int selectedAccount = GetPositionOfAccount (editingDisplay.administrator);
            //	selectedAccount = ImagePack.DrawDynamicFilteredListSelector(pos, "Admin Account:",ref accountSearch, selectedAccount, accountList);
            //	editingDisplay.administrator = accountIds [selectedAccount];
            pos.width *= 2;
            //   pos.y += ImagePack.fieldHeight;
            editingDisplay.populationLimit = ImagePack.DrawField(pos, Lang.GetTranslate("Population Limit")+":", editingDisplay.populationLimit);

            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Markers"));
            pos.y += 1.5f * ImagePack.fieldHeight;
            if (editingDisplay.instancePortals.Count == 0)
            {
                editingDisplay.instancePortals.Add(new InstancePortalEntry("spawn", Vector3.zero));
            }
            for (int i = 0; i < editingDisplay.instancePortals.Count; i++)
            {
                editingDisplay.instancePortals[i].Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.instancePortals[i].Name);
                pos.y += ImagePack.fieldHeight;
                editingDisplay.instancePortals[i].loc = ImagePack.Draw3DPosition(pos, Lang.GetTranslate("Location")+":", editingDisplay.instancePortals[i].loc);
                pos.y += ImagePack.fieldHeight * 1.5f;
                pos.width /= 2;
                float yaw = editingDisplay.instancePortals[i].orient.eulerAngles.y;
                yaw = ImagePack.DrawField(pos, Lang.GetTranslate("Rotation")+":", yaw);
                Vector3 pitchYawRoll = editingDisplay.instancePortals[i].orient.eulerAngles;
                pitchYawRoll.y = yaw;
                editingDisplay.instancePortals[i].orient.eulerAngles = pitchYawRoll;
                pos.y += ImagePack.fieldHeight;
                pos.width *= 2;
                editingDisplay.instancePortals[i].MarkerObject = ImagePack.DrawGameObject(pos, Lang.GetTranslate("Game Object")+":", editingDisplay.instancePortals[i].gameObject, 0.75f);
                pos.y += ImagePack.fieldHeight;
                if (i > 0)
                {
                    pos.width /= 2;
                    pos.x += pos.width;
                    if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove Marker")))
                    {
                        if (editingDisplay.instancePortals[i].id > 0)
                            editingDisplay.itemsToBeDeleted.Add(editingDisplay.instancePortals[i].id);
                        editingDisplay.instancePortals.RemoveAt(i);
                    }
                    pos.x -= pos.width;
                    pos.width *= 2;
                }
                pos.y += ImagePack.fieldHeight;
            }

            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Marker")))
            {
                editingDisplay.instancePortals.Add(new InstancePortalEntry());
            }

            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Weather Profiles"));
            pos.y += 1.5f * ImagePack.fieldHeight;

            for (int i = 0; i < editingDisplay.instanceWeatherProfiles.Count; i++)
            {

                int selectedprofile = GetOptionPosition(editingDisplay.instanceWeatherProfiles[i].weather_profile_id, instanceWeatherProfileIds);
                selectedprofile = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Weather Profile")+" #" + i, ref editingDisplay.instanceWeatherProfiles[i].search, selectedprofile, instanceWeatherProfileList);
                editingDisplay.instanceWeatherProfiles[i].weather_profile_id = instanceWeatherProfileIds[selectedprofile];
                pos.y += ImagePack.fieldHeight * 1f;
                editingDisplay.instanceWeatherProfiles[i].priority = ImagePack.DrawField(pos, Lang.GetTranslate("Prolfile priority")+":", editingDisplay.instanceWeatherProfiles[i].priority);
                pos.y += ImagePack.fieldHeight * 1f;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Months"));
                pos.y += 1f * ImagePack.fieldHeight;
                pos.width /= 13;
                editingDisplay.instanceWeatherProfiles[i].month1 = ImagePack.DrawToggleBox(pos, "1:", editingDisplay.instanceWeatherProfiles[i].month1);
                pos.x += pos.width;
                editingDisplay.instanceWeatherProfiles[i].month2 = ImagePack.DrawToggleBox(pos, "2:", editingDisplay.instanceWeatherProfiles[i].month2);
                pos.x += pos.width;
                editingDisplay.instanceWeatherProfiles[i].month3 = ImagePack.DrawToggleBox(pos, "3:", editingDisplay.instanceWeatherProfiles[i].month3);
                pos.x += pos.width;
                editingDisplay.instanceWeatherProfiles[i].month4 = ImagePack.DrawToggleBox(pos, "4:", editingDisplay.instanceWeatherProfiles[i].month4);
                pos.x += pos.width;
                editingDisplay.instanceWeatherProfiles[i].month5 = ImagePack.DrawToggleBox(pos, "5:", editingDisplay.instanceWeatherProfiles[i].month5);
                pos.x += pos.width;
                editingDisplay.instanceWeatherProfiles[i].month6 = ImagePack.DrawToggleBox(pos, "6:", editingDisplay.instanceWeatherProfiles[i].month6);
                pos.x += pos.width;
                editingDisplay.instanceWeatherProfiles[i].month7 = ImagePack.DrawToggleBox(pos, "7:", editingDisplay.instanceWeatherProfiles[i].month7);
                pos.x += pos.width;
                editingDisplay.instanceWeatherProfiles[i].month8 = ImagePack.DrawToggleBox(pos, "8:", editingDisplay.instanceWeatherProfiles[i].month8);
                pos.x += pos.width;
                editingDisplay.instanceWeatherProfiles[i].month9 = ImagePack.DrawToggleBox(pos, "9:", editingDisplay.instanceWeatherProfiles[i].month9);
                pos.x += pos.width;
                editingDisplay.instanceWeatherProfiles[i].month10 = ImagePack.DrawToggleBox(pos, "10:", editingDisplay.instanceWeatherProfiles[i].month10);
                pos.x += pos.width;
                editingDisplay.instanceWeatherProfiles[i].month11 = ImagePack.DrawToggleBox(pos, "11:", editingDisplay.instanceWeatherProfiles[i].month11);
                pos.x += pos.width;
                editingDisplay.instanceWeatherProfiles[i].month12 = ImagePack.DrawToggleBox(pos, "12:", editingDisplay.instanceWeatherProfiles[i].month12);
                pos.x -= pos.width * 11;
                pos.width *= 13;
                //  pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;

                pos.width /= 2;
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove Profile")))
                {
                    if (editingDisplay.instanceWeatherProfiles[i].id > 0)
                        editingDisplay.profileToBeDeleted.Add(editingDisplay.instanceWeatherProfiles[i].id);
                    editingDisplay.instanceWeatherProfiles.RemoveAt(i);
                }
                pos.x -= pos.width;
                pos.width *= 2;

                pos.y += ImagePack.fieldHeight;
            }

            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Profile")))
            {
                editingDisplay.instanceWeatherProfiles.Add(new WeatherInstace());
            }


            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Weather Seasons"));
            pos.y += 1.5f * ImagePack.fieldHeight;

            for (int i = 0; i < editingDisplay.instanceWeatherSeasons.Count; i++)
            {

                editingDisplay.instanceWeatherSeasons[i].season = ImagePack.DrawSelector(pos, Lang.GetTranslate("Weather Season")+" #" + i, editingDisplay.instanceWeatherSeasons[i].season, WeatherSeasion.seasonTypes);
                pos.y += ImagePack.fieldHeight * 1f;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Months"));
                pos.y += 1f * ImagePack.fieldHeight;
                pos.width /= 13;
                editingDisplay.instanceWeatherSeasons[i].month1 = ImagePack.DrawToggleBox(pos, "1:", editingDisplay.instanceWeatherSeasons[i].month1);
                pos.x += pos.width;
                editingDisplay.instanceWeatherSeasons[i].month2 = ImagePack.DrawToggleBox(pos, "2:", editingDisplay.instanceWeatherSeasons[i].month2);
                pos.x += pos.width;
                editingDisplay.instanceWeatherSeasons[i].month3 = ImagePack.DrawToggleBox(pos, "3:", editingDisplay.instanceWeatherSeasons[i].month3);
                pos.x += pos.width;
                editingDisplay.instanceWeatherSeasons[i].month4 = ImagePack.DrawToggleBox(pos, "4:", editingDisplay.instanceWeatherSeasons[i].month4);
                pos.x += pos.width;
                editingDisplay.instanceWeatherSeasons[i].month5 = ImagePack.DrawToggleBox(pos, "5:", editingDisplay.instanceWeatherSeasons[i].month5);
                pos.x += pos.width;
                editingDisplay.instanceWeatherSeasons[i].month6 = ImagePack.DrawToggleBox(pos, "6:", editingDisplay.instanceWeatherSeasons[i].month6);
                pos.x += pos.width;
                editingDisplay.instanceWeatherSeasons[i].month7 = ImagePack.DrawToggleBox(pos, "7:", editingDisplay.instanceWeatherSeasons[i].month7);
                pos.x += pos.width;
                editingDisplay.instanceWeatherSeasons[i].month8 = ImagePack.DrawToggleBox(pos, "8:", editingDisplay.instanceWeatherSeasons[i].month8);
                pos.x += pos.width;
                editingDisplay.instanceWeatherSeasons[i].month9 = ImagePack.DrawToggleBox(pos, "9:", editingDisplay.instanceWeatherSeasons[i].month9);
                pos.x += pos.width;
                editingDisplay.instanceWeatherSeasons[i].month10 = ImagePack.DrawToggleBox(pos, "10:", editingDisplay.instanceWeatherSeasons[i].month10);
                pos.x += pos.width;
                editingDisplay.instanceWeatherSeasons[i].month11 = ImagePack.DrawToggleBox(pos, "11:", editingDisplay.instanceWeatherSeasons[i].month11);
                pos.x += pos.width;
                editingDisplay.instanceWeatherSeasons[i].month12 = ImagePack.DrawToggleBox(pos, "12:", editingDisplay.instanceWeatherSeasons[i].month12);
                pos.x -= pos.width * 11;
                pos.width *= 13;
                //  pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;

                pos.width /= 2;
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove Season")))
                {
                    if (editingDisplay.instanceWeatherSeasons[i].id > 0)
                        editingDisplay.seasonsToBeDeleted.Add(editingDisplay.instanceWeatherSeasons[i].id);
                    editingDisplay.instanceWeatherSeasons.RemoveAt(i);
                }
                pos.x -= pos.width;
                pos.width *= 2;

                pos.y += ImagePack.fieldHeight;
            }

            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Season")))
            {
                editingDisplay.instanceWeatherSeasons.Add(new WeatherSeasion());
            }

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

            if (!newEntity)
                EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight + 100);
            else
                EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight);

        }

        public override void save()
        {
            if (editingDisplay.islandType == 0)
                editingDisplay.createOnStartup = true;
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
            query += " (island_name, template, administrator, category, status, createOnStartup, islandType, globalWaterHeight, public, password, style, recommendedLevel, description, size, populationLimit) ";
            query += "VALUES ";
            query += "(?island_name, ?template, ?administrator, ?category, ?status, ?createOnStartup, ?islandType, ?globalWaterHeight, ?public, ?password, ?style, ?recommendedLevel, ?description, ?size, ?populationLimit)";

            int instanceID = -1;

            // Setup the register data		
            List<Register> update = new List<Register>();
            update.Add(new Register("island_name", "?island_name", MySqlDbType.VarChar, editingDisplay.Name.ToString(), Register.TypesOfField.String));
            update.Add(new Register("template", "?template", MySqlDbType.VarChar, "", Register.TypesOfField.String));
            update.Add(new Register("administrator", "?administrator", MySqlDbType.Int32, editingDisplay.administrator.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("category", "?category", MySqlDbType.Int32, "1", Register.TypesOfField.Int));
            update.Add(new Register("status", "?status", MySqlDbType.VarChar, "Active", Register.TypesOfField.String));
            update.Add(new Register("createOnStartup", "?createOnStartup", MySqlDbType.Byte, editingDisplay.createOnStartup.ToString(), Register.TypesOfField.Bool));
            update.Add(new Register("islandType", "?islandType", MySqlDbType.Int32, editingDisplay.islandType.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("globalWaterHeight", "?globalWaterHeight", MySqlDbType.Float, editingDisplay.globalWaterHeight.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("public", "?public", MySqlDbType.Byte, "true", Register.TypesOfField.Bool));
            update.Add(new Register("password", "?password", MySqlDbType.VarChar, "", Register.TypesOfField.String));
            update.Add(new Register("style", "?style", MySqlDbType.VarChar, "", Register.TypesOfField.String));
            update.Add(new Register("recommendedLevel", "?recommendedLevel", MySqlDbType.Int32, "0", Register.TypesOfField.Int));
            update.Add(new Register("description", "?description", MySqlDbType.VarChar, "", Register.TypesOfField.String));
            update.Add(new Register("size", "?size", MySqlDbType.Int32, "-1", Register.TypesOfField.Int));
            update.Add(new Register("populationLimit", "?populationLimit", MySqlDbType.Int32, editingDisplay.populationLimit.ToString(), Register.TypesOfField.Int));

            // Update the database
            instanceID = DatabasePack.Insert(DatabasePack.adminDatabasePrefix, query, update);

            // If the insert failed, don't insert the spawn marker
            if (instanceID != -1)
            {
                //int islandID = instanceID;

                foreach (InstancePortalEntry entry in editingDisplay.instancePortals)
                {
                    if (entry.Name != "")
                    {
                        entry.instanceID = instanceID;
                        InsertItem(entry);
                    }
                }

                foreach (WeatherInstace entry in editingDisplay.instanceWeatherProfiles)
                {
                    entry.instance_id = instanceID;
                    InseetWeatherProfile(entry);
                }

                foreach (WeatherSeasion entry in editingDisplay.instanceWeatherSeasons)
                {
                    entry.instance_id = instanceID;
                    InseetWeatherSeason(entry);
                }
                // Update online table to avoid access the database again			
                editingDisplay.id = instanceID;
                editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + instanceID + "ID2:" + editingDisplay.id);
                // dataRegister.Add(editingDisplay.id, editingDisplay);
                // displayKeys.Add(editingDisplay.id);
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

        void InsertItem(InstancePortalEntry entry)
        {
            string query = "INSERT INTO " + portalTableName;
            query += " (island, portalType, faction, displayID, locX, locY, locZ, orientX, orientY, orientZ, orientW, name) ";
            query += "VALUES ";
            query += " (" + entry.instanceID + "," + entry.portalType + "," + entry.faction + "," + entry.displayID + "," + entry.loc.x + ","
                + entry.loc.y + "," + entry.loc.z + "," + entry.orient.x + "," + entry.orient.y
                + "," + entry.orient.z + "," + entry.orient.w + ",'" + entry.Name + "') ";

            // Setup the register data
            List<Register> update = new List<Register>();
            foreach (string field in entry.fields.Keys)
            {
                update.Add(entry.fieldToRegister(field));
            }

            int itemID = -1;
            itemID = DatabasePack.Insert(DatabasePack.adminDatabasePrefix, query, update);

            entry.id = itemID;
        }

        void InseetWeatherProfile(WeatherInstace entry)
        {

            string query = "INSERT INTO " + weatherInstanceTableName;
            query += " (instance_id, weather_profile_id, month1, month2, month3, month4, month5, month6, month7, month8, month9, month10,month11,month12,priority) ";
            query += "VALUES ";
            query += " (" + entry.instance_id + "," + entry.weather_profile_id + "," + entry.month1 + "," + entry.month2 + "," + entry.month3 + ","
                + entry.month4 + "," + entry.month5 + "," + entry.month6 + "," + entry.month7
                + "," + entry.month8 + "," + entry.month9 + "," + entry.month10 + "," + entry.month11 + "," + entry.month12 + "," + entry.priority + ") ";

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

        void InseetWeatherSeason(WeatherSeasion entry)
        {

            string query = "INSERT INTO " + weatherSeasonTableName;
            query += " (instance_id, season, month1, month2, month3, month4, month5, month6, month7, month8, month9, month10,month11,month12) ";
            query += "VALUES ";
            query += " (" + entry.instance_id + "," + entry.season + "," + entry.month1 + "," + entry.month2 + "," + entry.month3 + ","
                + entry.month4 + "," + entry.month5 + "," + entry.month6 + "," + entry.month7
                + "," + entry.month8 + "," + entry.month9 + "," + entry.month10 + "," + entry.month11 + "," + entry.month12 + ") ";

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
            query += " SET island_name=?island_name,";
            query += " createOnStartup=?createOnStartup,";
            query += " islandType=?islandType,";
            query += " globalWaterHeight=?globalWaterHeight,";
            query += " administrator=?administrator,";
            query += " populationLimit=?populationLimit WHERE id=?id";
            // Setup the register data		
            List<Register> update = new List<Register>();
            update.Add(new Register("island_name", "?island_name", MySqlDbType.VarChar, editingDisplay.Name.ToString(), Register.TypesOfField.String));
            update.Add(new Register("createOnStartup", "?createOnStartup", MySqlDbType.Byte, editingDisplay.createOnStartup.ToString(), Register.TypesOfField.Bool));
            update.Add(new Register("islandType", "?islandType", MySqlDbType.Int32, editingDisplay.islandType.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("globalWaterHeight", "?globalWaterHeight", MySqlDbType.Float, editingDisplay.globalWaterHeight.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("administrator", "?administrator", MySqlDbType.Int32, editingDisplay.administrator.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("populationLimit", "?populationLimit", MySqlDbType.Int32, editingDisplay.populationLimit.ToString(), Register.TypesOfField.Int));
            update.Add(new Register("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString(), Register.TypesOfField.Int));

            // Update the database
            DatabasePack.Update(DatabasePack.adminDatabasePrefix, query, update);

            // Insert/Update the abilities
            foreach (InstancePortalEntry entry in editingDisplay.instancePortals)
            {
                if (entry.Name != "")
                {
                    if (entry.id < 1)
                    {
                        // This is a new entry, insert it
                        entry.instanceID = editingDisplay.id;
                        InsertItem(entry);
                    }
                    else
                    {
                        // This is an existing entry, update it
                        entry.instanceID = editingDisplay.id;
                        UpdateItem(entry);
                    }
                }
            }

            foreach (WeatherInstace entry in editingDisplay.instanceWeatherProfiles)
            {
                if (entry.id < 1)
                {
                    entry.instance_id = editingDisplay.id;
                    InseetWeatherProfile(entry);
                }
                else
                {
                    entry.instance_id = editingDisplay.id;
                    UpdateWeatherProfile(entry);
                }
            }
            foreach (WeatherSeasion entry in editingDisplay.instanceWeatherSeasons)
            {
                if (entry.id < 1)
                {
                    entry.instance_id = editingDisplay.id;
                    InseetWeatherSeason(entry);
                }
                else
                {
                    entry.instance_id = editingDisplay.id;
                    UpdateWeatherSeason(entry);
                }
            }
            // Delete any abilities that are tagged for deletion
            foreach (int itemID in editingDisplay.itemsToBeDeleted)
            {
                DeleteItem(itemID);
            }
            foreach (int itemID in editingDisplay.profileToBeDeleted)
            {
                DeleteProfile(itemID);
            }
            foreach (int itemID in editingDisplay.seasonsToBeDeleted)
            {
                DeleteSeason(itemID);
            }

            // Update online table to avoid access the database again			
            // dataRegister[displayKeys[selectedDisplay]] = editingDisplay;
            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry") + "  " + editingDisplay.Name + " " + Lang.GetTranslate("updated"));
              Load();
      }

        void UpdateItem(InstancePortalEntry entry)
        {
            string query = "UPDATE " + portalTableName;
            query += " SET ";
            query += entry.UpdateList();
            query += " WHERE id=?id";

            // Setup the register data
            List<Register> update = new List<Register>();
            foreach (string field in entry.fields.Keys)
            {
                update.Add(entry.fieldToRegister(field));
            }

            DatabasePack.Update(DatabasePack.adminDatabasePrefix, query, update);
        }

        void UpdateWeatherProfile(WeatherInstace entry)
        {

            string query = "UPDATE " + weatherInstanceTableName;
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
        void UpdateWeatherSeason(WeatherSeasion entry)
        {

            string query = "UPDATE " + weatherSeasonTableName;
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


        void DeleteItem(int portalID)
        {
            Register delete = new Register("id", "?id", MySqlDbType.Int32, portalID.ToString(), Register.TypesOfField.Int);
            DatabasePack.Delete(DatabasePack.adminDatabasePrefix, portalTableName, delete);
        }
        void DeleteProfile(int portalID)
        {
            Register delete = new Register("id", "?id", MySqlDbType.Int32, portalID.ToString(), Register.TypesOfField.Int);
            DatabasePack.Delete(DatabasePack.contentDatabasePrefix, weatherInstanceTableName, delete);
        }

        void DeleteSeason(int portalID)
        {
            Register delete = new Register("id", "?id", MySqlDbType.Int32, portalID.ToString(), Register.TypesOfField.Int);
            DatabasePack.Delete(DatabasePack.contentDatabasePrefix, weatherSeasonTableName, delete);
        }

        void DeleteWeatherProfile(int instanceID, int weatherProfileID)
        {
            List<Register> delete = new List<Register>();
            delete.Add(new Register("instance_id", "?instance_id", MySqlDbType.Int32, instanceID.ToString(), Register.TypesOfField.Int));
            delete.Add(new Register("weather_profile_id", "?weather_profile_id", MySqlDbType.Int32, weatherProfileID.ToString(), Register.TypesOfField.Int));
            //  Register delete = new Register("id", "?id", MySqlDbType.Int32, instanceID.ToString(), Register.TypesOfField.Int);
            DatabasePack.Delete(DatabasePack.contentDatabasePrefix, portalTableName, delete);
        }


        // Delete entries from the table
        void DeleteEntry()
        {
            //Check instance_template
            string sql = "SELECT id FROM character_create_template WHERE instance=" + editingDisplay.id;
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sql);
            int charCount = 0;
            string charIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                charIds = "\n"+ Lang.GetTranslate("Character Template Ids")+":";
                charCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    charIds += " " + data["id"] + ",";
                }
            }
            //Check Arena
            string sqlArena = "SELECT id FROM arena_templates WHERE arenaInstanceID=" + editingDisplay.id;
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlArena);
            int arenaCount = 0;
            string arenaIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                arenaIds = "\n"+ Lang.GetTranslate("Arena Ids")+":";
                arenaCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    arenaIds += " " + data["id"] + ",";
                }
            }

            //Check Buildings
            string sqlBuildings = "SELECT id FROM build_object_template WHERE interactionType like 'Instance' AND interactionID=" + editingDisplay.id;
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlBuildings);
            int buildingCount = 0;
            string buildingIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                buildingIds = "\n"+ Lang.GetTranslate("Building Objects Ids")+":";
                buildingCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    buildingIds += " " + data["id"]+",";
                }
            }
            string sqlEffects = "SELECT id FROM effects WHERE (effectType like 'TeleportEffect' AND  intValue1=" + editingDisplay.id+ " ) OR (effectType like 'SetRespawnLocationEffect' AND  intValue1=" + editingDisplay.id +")";
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlEffects);
            int effectsCount = 0;
            string effectsIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                effectsIds = "\n"+ Lang.GetTranslate("Effects Ids")+":";
                effectsCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    effectsIds += " " + data["id"] + ",";
                }
            }

            DatabasePack.SaveLog("Delete Instance id =" + editingDisplay.id + " assigned to " + charIds + arenaIds + buildingIds + effectsIds);

            if (charCount == 0 && arenaCount == 0 && buildingCount == 0 && effectsCount == 0)
            {
                // List<Register> update = new List<Register>();
                Register delete = new Register("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString(), Register.TypesOfField.Int);
                DatabasePack.Delete(DatabasePack.adminDatabasePrefix, tableName, delete);
                delete = new Register("island", "?island", MySqlDbType.Int32, editingDisplay.id.ToString(), Register.TypesOfField.Int);
                DatabasePack.Delete(DatabasePack.adminDatabasePrefix, portalTableName, delete);
            }
            else
            {
                
                EditorUtility.DisplayDialog("", Lang.GetTranslate("You can not delete Instance because it is assigned in")+":" +charIds+arenaIds+buildingIds+effectsIds , Lang.GetTranslate("OK"), "");


            }
            // Update online table to avoid access the database again			
            dataLoaded = false;
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
        public new int  GetOptionPosition(int id, int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                if (ids[i] == id)
                    return i;
            }
            return 0;
        }

        public static int GetInstanceID(string instanceName)
        {
            string query = "SELECT id FROM instance_template where island_name = '" + instanceName + "'";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.adminDatabasePrefix, query);
            // Read data
            //	int optionsId = 0;
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