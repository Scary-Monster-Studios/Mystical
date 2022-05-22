using UnityEngine;
using UnityEditor;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Atavism
{
    // Class that implements the Instances configuration
    public class ServerWeather : AtavismDatabaseFunction
    {
        public new Dictionary<int, WeatherProfile> dataRegister;
        public new WeatherProfile editingDisplay;
        public new WeatherProfile originalDisplay;

        public int[] accountIds = new int[] { 1 };
        public string[] accountList = new string[] { "~ First Account ~" };

        public static int[] arenaIds = new int[] { 1 };
        public static string[] arenaList = new string[] { "~ none ~" };

        public int[] currencyIds = new int[] { -1 };
        public string[] currencyOptions = new string[] { "~ none ~" };
       
        // Use this for initialization
        public ServerWeather()
        {
            functionName = "Weather Profiles";
            // Database tables name
            tableName = "weather_profile";
            functionTitle = "Weather Configuration";
            loadButtonLabel = "Load Weather";
            notLoadedText = "No Weather Profiles loaded.";
            // Init
            dataRegister = new Dictionary<int, WeatherProfile>();

            editingDisplay = new WeatherProfile();
            originalDisplay = new WeatherProfile();
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

        WeatherProfile LoadEntity(int id)
        {
            // Read all entries from the table
            string query = "SELECT * FROM " + tableName+ " WHERE isactive = 1 AND id=" + id;

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read all the data
            WeatherProfile display = new WeatherProfile();
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    display.id = int.Parse(data["id"]);
                    display.Name = data["name"];
                    display.temperature_min = float.Parse(data["temperature_min"]);
                    display.temperature_max = float.Parse(data["temperature_max"]);
                    display.humidity_min = float.Parse(data["humidity_min"]);
                    display.humidity_max = float.Parse(data["humidity_max"]);
                    display.wind_direction_min = float.Parse(data["wind_direction_min"]);
                    display.wind_direction_max = float.Parse(data["wind_direction_max"]);
                    display.wind_speed_min = float.Parse(data["wind_speed_min"]);
                    display.wind_speed_max = float.Parse(data["wind_speed_max"]);
                    display.wind_turbulence_min = float.Parse(data["wind_turbulence_min"]);
                    display.wind_turbulence_max = float.Parse(data["wind_turbulence_max"]);
                    display.fog_height_power_min = float.Parse(data["fog_height_power_min"]);
                    display.fog_height_power_max = float.Parse(data["fog_height_power_max"]);
                    display.fog_height_max = float.Parse(data["fog_height_max"]);
                    display.fog_distance_power_min = float.Parse(data["fog_distance_power_min"]);
                    display.fog_distance_power_max = float.Parse(data["fog_distance_power_max"]);
                    display.fog_distance_max = float.Parse(data["fog_distance_max"]);
                    display.rain_power_min = float.Parse(data["rain_power_min"]);
                    display.rain_power_max = float.Parse(data["rain_power_max"]);
                    display.rain_power_terrain_min = float.Parse(data["rain_power_terrain_min"]);
                    display.rain_power_terrain_max = float.Parse(data["rain_power_terrain_max"]);
                    display.rain_min_height = float.Parse(data["rain_min_height"]);
                    display.rain_max_height = float.Parse(data["rain_max_height"]);
                    display.hail_power_min = float.Parse(data["hail_power_min"]);
                    display.hail_power_max = float.Parse(data["hail_power_max"]);
                    display.hail_power_terrain_min = float.Parse(data["hail_power_terrain_min"]);
                    display.hail_power_terrain_max = float.Parse(data["hail_power_terrain_max"]);
                    display.hail_min_height = float.Parse(data["hail_min_height"]);
                    display.hail_max_height = float.Parse(data["hail_max_height"]);
                    display.snow_power_min = float.Parse(data["snow_power_min"]);
                    display.snow_power_max = float.Parse(data["snow_power_max"]);
                    display.snow_power_terrain_min = float.Parse(data["snow_power_terrain_min"]);
                    display.snow_power_terrain_max = float.Parse(data["snow_power_terrain_max"]);
                    display.snow_min_height = float.Parse(data["snow_min_height"]);
                    display.snow_age_min = float.Parse(data["snow_age_min"]);
                    display.snow_age_max = float.Parse(data["snow_age_max"]);
                    display.thunder_power_min = float.Parse(data["thunder_power_min"]);
                    display.thunder_power_max = float.Parse(data["thunder_power_max"]);
                    display.cloud_power_min = float.Parse(data["cloud_power_min"]);
                    display.cloud_power_max = float.Parse(data["cloud_power_max"]);
                    display.cloud_min_height = float.Parse(data["cloud_min_height"]);
                    display.cloud_max_height = float.Parse(data["cloud_max_height"]);
                    display.cloud_speed_min = float.Parse(data["cloud_speed_min"]);
                    display.cloud_speed_max = float.Parse(data["cloud_speed_max"]);
                    display.moon_phase_min = float.Parse(data["moon_phase_min"]);
                    display.moon_phase_max = float.Parse(data["moon_phase_max"]);
                    display.isLoaded = false;
                }
            }
            LoadWaetherInstaces(display);
        return display;
        }

        void LoadWaetherInstaces(WeatherProfile weatherProfile)
        {
            // Read all entries from the table
            string query = "SELECT * FROM  weather_instance  where weather_profile_id = " + weatherProfile.id;

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
                    entry.id = int.Parse(data["instance_id"]);
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
                    weatherProfile.watherInstaces.Add(entry);
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create an Waether Profile before editing it."));
                return;
            }


            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Warther Profile"));

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
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Duplicate")))
                {
                    Duplicate();
                }
                pos.x -= pos.width;
                pos.width *= 2;
                pos.y += ImagePack.fieldHeight * 1.5f;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Weather Profile Properties:"));
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
            editingDisplay = new WeatherProfile();
            originalDisplay = new WeatherProfile();
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new Weather Profile"));
                pos.y += ImagePack.fieldHeight;
            }
            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.75f);
            pos.y += ImagePack.fieldHeight;
            pos.width /= 2;
            //Temperature
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Temperature"));
            pos.y += ImagePack.fieldHeight;
            editingDisplay.temperature_min = ImagePack.DrawField(pos, Lang.GetTranslate("Temperature Min") + ":", editingDisplay.temperature_min);
            pos.x += pos.width;
            editingDisplay.temperature_max = ImagePack.DrawField(pos, Lang.GetTranslate("Temperature Max") + ":", editingDisplay.temperature_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            //Humidity
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Humidity"));
            pos.y += ImagePack.fieldHeight;
            editingDisplay.humidity_min = ImagePack.DrawField(pos, Lang.GetTranslate("Humidity Min") + ":", editingDisplay.humidity_min);
            pos.x += pos.width;
            editingDisplay.humidity_max = ImagePack.DrawField(pos, Lang.GetTranslate("Humidity Max") + ":", editingDisplay.humidity_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            //Wind
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Wind"));
            pos.y += ImagePack.fieldHeight;
            editingDisplay.wind_direction_min = ImagePack.DrawField(pos, Lang.GetTranslate("Direction Min") + ":", editingDisplay.wind_direction_min);
            pos.x += pos.width;
            editingDisplay.wind_direction_max = ImagePack.DrawField(pos, Lang.GetTranslate("Direction Max") + ":", editingDisplay.wind_direction_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;

            editingDisplay.wind_speed_min = ImagePack.DrawField(pos, Lang.GetTranslate("Speed Min") + ":", editingDisplay.wind_speed_min);
            pos.x += pos.width;
            editingDisplay.wind_speed_max = ImagePack.DrawField(pos, Lang.GetTranslate("Wind Speed Max") + ":", editingDisplay.wind_speed_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;

            editingDisplay.wind_turbulence_min = ImagePack.DrawField(pos, Lang.GetTranslate("Turbulence Min") + ":", editingDisplay.wind_turbulence_min);
            pos.x += pos.width;
            editingDisplay.wind_turbulence_max = ImagePack.DrawField(pos, Lang.GetTranslate("Turbulence Max") + ":", editingDisplay.wind_turbulence_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            //Fog
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Fog"));
            pos.y += ImagePack.fieldHeight;
            editingDisplay.fog_height_power_min = ImagePack.DrawField(pos, Lang.GetTranslate("Height Power Min") + ":", editingDisplay.fog_height_power_min);
            pos.x += pos.width;
            editingDisplay.fog_height_power_max = ImagePack.DrawField(pos, Lang.GetTranslate("Fog Height Power Max") + ":", editingDisplay.fog_height_power_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;

            editingDisplay.fog_height_max = ImagePack.DrawField(pos, Lang.GetTranslate("Height Max") + ":", editingDisplay.fog_height_max);
            pos.x += pos.width;
            editingDisplay.fog_distance_max = ImagePack.DrawField(pos, Lang.GetTranslate("Distance Max") + ":", editingDisplay.fog_distance_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;

            editingDisplay.fog_distance_power_min = ImagePack.DrawField(pos, Lang.GetTranslate("Distance Power Min") + ":", editingDisplay.fog_distance_power_min);
            pos.x += pos.width;
            editingDisplay.fog_distance_power_max = ImagePack.DrawField(pos, Lang.GetTranslate("Distance Power Max") + ":", editingDisplay.fog_distance_power_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            //Rain
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Rain"));
            pos.y += ImagePack.fieldHeight;
            editingDisplay.rain_power_min = ImagePack.DrawField(pos, Lang.GetTranslate("Power Min") + ":", editingDisplay.rain_power_min);
            pos.x += pos.width;
            editingDisplay.rain_power_max = ImagePack.DrawField(pos, Lang.GetTranslate("Power Max") + ":", editingDisplay.rain_power_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;

            editingDisplay.rain_power_terrain_min = ImagePack.DrawField(pos, Lang.GetTranslate("Power Terrain Min") + ":", editingDisplay.rain_power_terrain_min);
            pos.x += pos.width;
            editingDisplay.rain_power_terrain_max = ImagePack.DrawField(pos, Lang.GetTranslate("Power Terrain Max") + ":", editingDisplay.rain_power_terrain_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;

            editingDisplay.rain_min_height = ImagePack.DrawField(pos, Lang.GetTranslate("Min Height") + ":", editingDisplay.rain_min_height);
            pos.x += pos.width;
            editingDisplay.rain_max_height = ImagePack.DrawField(pos, Lang.GetTranslate("Max Height") + ":", editingDisplay.rain_max_height);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            //Hail
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Hail"));
            pos.y += ImagePack.fieldHeight;
            editingDisplay.hail_power_min = ImagePack.DrawField(pos, Lang.GetTranslate("Power Min") + ":", editingDisplay.hail_power_min);
            pos.x += pos.width;
            editingDisplay.hail_power_max = ImagePack.DrawField(pos, Lang.GetTranslate("Power Max") + ":", editingDisplay.hail_power_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;

            editingDisplay.hail_power_terrain_min = ImagePack.DrawField(pos, Lang.GetTranslate("Power Terrain Min") + ":", editingDisplay.hail_power_terrain_min);
            pos.x += pos.width;
            editingDisplay.hail_power_terrain_max = ImagePack.DrawField(pos, Lang.GetTranslate("Power Terrain Max") + ":", editingDisplay.hail_power_terrain_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;

            editingDisplay.hail_min_height = ImagePack.DrawField(pos, Lang.GetTranslate("Min Height") + ":", editingDisplay.hail_min_height);
            pos.x += pos.width;
            editingDisplay.hail_max_height = ImagePack.DrawField(pos, Lang.GetTranslate("Max Height") + ":", editingDisplay.hail_max_height);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            //Snow
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Snow"));
            pos.y += ImagePack.fieldHeight;
            editingDisplay.snow_power_min = ImagePack.DrawField(pos, Lang.GetTranslate("Power Min") + ":", editingDisplay.snow_power_min);
            pos.x += pos.width;
            editingDisplay.snow_power_max = ImagePack.DrawField(pos, Lang.GetTranslate("Power Max") + ":", editingDisplay.snow_power_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;

            editingDisplay.snow_power_terrain_min = ImagePack.DrawField(pos, Lang.GetTranslate("Power Terrain Min") + ":", editingDisplay.snow_power_terrain_min);
            pos.x += pos.width;
            editingDisplay.snow_power_terrain_max = ImagePack.DrawField(pos, Lang.GetTranslate("Power Terrain Max") + ":", editingDisplay.snow_power_terrain_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;

            editingDisplay.snow_age_min = ImagePack.DrawField(pos, Lang.GetTranslate("Age Min") + ":", editingDisplay.snow_age_min);
            pos.x += pos.width;
            editingDisplay.snow_age_max = ImagePack.DrawField(pos, Lang.GetTranslate("Age Max") + ":", editingDisplay.snow_age_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.snow_min_height = ImagePack.DrawField(pos, Lang.GetTranslate("Min Height") + ":", editingDisplay.snow_min_height);
            pos.y += ImagePack.fieldHeight;
            //Thunder
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Thunder"));
            pos.y += ImagePack.fieldHeight;
            editingDisplay.thunder_power_min = ImagePack.DrawField(pos, Lang.GetTranslate("Power Min") + ":", editingDisplay.thunder_power_min);
            pos.x += pos.width;
            editingDisplay.thunder_power_max = ImagePack.DrawField(pos, Lang.GetTranslate("Power Max") + ":", editingDisplay.thunder_power_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            //Cloud
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Cloud"));
            pos.y += ImagePack.fieldHeight;
            editingDisplay.cloud_power_min = ImagePack.DrawField(pos, Lang.GetTranslate("Power Min") + ":", editingDisplay.cloud_power_min);
            pos.x += pos.width;
            editingDisplay.cloud_power_max = ImagePack.DrawField(pos, Lang.GetTranslate("Power Max") + ":", editingDisplay.cloud_power_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;

            editingDisplay.cloud_speed_min = ImagePack.DrawField(pos, Lang.GetTranslate("Speed Min") + ":", editingDisplay.cloud_speed_min);
            pos.x += pos.width;
            editingDisplay.cloud_speed_max = ImagePack.DrawField(pos, Lang.GetTranslate("Speed Max") + ":", editingDisplay.cloud_speed_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;

            editingDisplay.cloud_min_height = ImagePack.DrawField(pos, Lang.GetTranslate("Min Height") + ":", editingDisplay.cloud_min_height);
            pos.x += pos.width;
            editingDisplay.cloud_max_height = ImagePack.DrawField(pos, Lang.GetTranslate("Max Height") + ":", editingDisplay.cloud_max_height);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            //Moon
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Moon"));
            pos.y += ImagePack.fieldHeight;
            editingDisplay.moon_phase_min = ImagePack.DrawField(pos, Lang.GetTranslate("Phase Min") + ":", editingDisplay.moon_phase_min);
            pos.x += pos.width;
            editingDisplay.moon_phase_max = ImagePack.DrawField(pos, Lang.GetTranslate("Phase Max") + ":", editingDisplay.moon_phase_max);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            pos.width *= 2;


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
            query += " (name ,temperature_min ,temperature_max ,humidity_min ,humidity_max ,wind_direction_min ,wind_direction_max ,wind_speed_min ," +
                "wind_speed_max ,wind_turbulence_min ,wind_turbulence_max ,fog_height_power_min ,fog_height_power_max ,fog_height_max ,fog_distance_power_min ," +
                "fog_distance_power_max ,fog_distance_max ,rain_power_min ,rain_power_max ,rain_power_terrain_min ,rain_power_terrain_max ,rain_min_height ," +
                "rain_max_height ,hail_power_min ,hail_power_max ,hail_power_terrain_min ,hail_power_terrain_max ,hail_min_height ,hail_max_height ,snow_power_min ," +
                "snow_power_max ,snow_power_terrain_min ,snow_power_terrain_max ,snow_min_height ,snow_age_min ,snow_age_max ,thunder_power_min ,thunder_power_max ," +
                "cloud_power_min ,cloud_power_max ,cloud_min_height ,cloud_max_height ,cloud_speed_min ,cloud_speed_max ,moon_phase_min ,moon_phase_max ,isactive) ";
            query += "VALUES ";
            query += "(?name ,?temperature_min ,?temperature_max ,?humidity_min ,?humidity_max ,?wind_direction_min ,?wind_direction_max ,?wind_speed_min ,?wind_speed_max " +
                ",?wind_turbulence_min ,?wind_turbulence_max ,?fog_height_power_min ,?fog_height_power_max ,?fog_height_max ,?fog_distance_power_min ,?fog_distance_power_max ," +
                "?fog_distance_max ,?rain_power_min ,?rain_power_max ,?rain_power_terrain_min ,?rain_power_terrain_max ,?rain_min_height ,?rain_max_height ,?hail_power_min ," +
                "?hail_power_max ,?hail_power_terrain_min ,?hail_power_terrain_max ,?hail_min_height ,?hail_max_height ,?snow_power_min ,?snow_power_max ,?snow_power_terrain_min " +
                ",?snow_power_terrain_max ,?snow_min_height ,?snow_age_min ,?snow_age_max ,?thunder_power_min ,?thunder_power_max ,?cloud_power_min ,?cloud_power_max ,?cloud_min_height" +
                " ,?cloud_max_height ,?cloud_speed_min ,?cloud_speed_max ,?moon_phase_min ,?moon_phase_max ,?isactive) ";

            int arenaID = -1;

            // Setup the register data		
            List<Register> update = new List<Register>();
            update.Add(new Register("name", "?name", MySqlDbType.VarChar, editingDisplay.Name.ToString(), Register.TypesOfField.String));
            update.Add(new Register("temperature_min", "?temperature_min", MySqlDbType.Float, editingDisplay.temperature_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("temperature_max", "?temperature_max", MySqlDbType.Float, editingDisplay.temperature_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("humidity_min", "?humidity_min", MySqlDbType.Float, editingDisplay.humidity_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("humidity_max", "?humidity_max", MySqlDbType.Float, editingDisplay.humidity_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("wind_direction_min", "?wind_direction_min", MySqlDbType.Float, editingDisplay.wind_direction_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("wind_direction_max", "?wind_direction_max", MySqlDbType.Float, editingDisplay.wind_direction_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("wind_speed_min", "?wind_speed_min", MySqlDbType.Float, editingDisplay.wind_speed_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("wind_speed_max", "?wind_speed_max", MySqlDbType.Float, editingDisplay.wind_speed_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("wind_turbulence_min", "?wind_turbulence_min", MySqlDbType.Float, editingDisplay.wind_turbulence_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("wind_turbulence_max", "?wind_turbulence_max", MySqlDbType.Float, editingDisplay.wind_turbulence_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("fog_height_power_min", "?fog_height_power_min", MySqlDbType.Float, editingDisplay.fog_height_power_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("fog_height_power_max", "?fog_height_power_max", MySqlDbType.Float, editingDisplay.fog_height_power_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("fog_height_max", "?fog_height_max", MySqlDbType.Float, editingDisplay.fog_height_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("fog_distance_power_min", "?fog_distance_power_min", MySqlDbType.Float, editingDisplay.fog_distance_power_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("fog_distance_power_max", "?fog_distance_power_max", MySqlDbType.Float, editingDisplay.fog_distance_power_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("fog_distance_max", "?fog_distance_max", MySqlDbType.Float, editingDisplay.fog_distance_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("rain_power_min", "?rain_power_min", MySqlDbType.Float, editingDisplay.rain_power_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("rain_power_max", "?rain_power_max", MySqlDbType.Float, editingDisplay.rain_power_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("rain_power_terrain_min", "?rain_power_terrain_min", MySqlDbType.Float, editingDisplay.rain_power_terrain_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("rain_power_terrain_max", "?rain_power_terrain_max", MySqlDbType.Float, editingDisplay.rain_power_terrain_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("rain_min_height", "?rain_min_height", MySqlDbType.Float, editingDisplay.rain_min_height.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("rain_max_height", "?rain_max_height", MySqlDbType.Float, editingDisplay.rain_max_height.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("hail_power_min", "?hail_power_min", MySqlDbType.Float, editingDisplay.hail_power_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("hail_power_max", "?hail_power_max", MySqlDbType.Float, editingDisplay.hail_power_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("hail_power_terrain_min", "?hail_power_terrain_min", MySqlDbType.Float, editingDisplay.hail_power_terrain_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("hail_power_terrain_max", "?hail_power_terrain_max", MySqlDbType.Float, editingDisplay.hail_power_terrain_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("hail_min_height", "?hail_min_height", MySqlDbType.Float, editingDisplay.hail_min_height.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("hail_max_height", "?hail_max_height", MySqlDbType.Float, editingDisplay.hail_max_height.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("snow_power_min", "?snow_power_min", MySqlDbType.Float, editingDisplay.snow_power_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("snow_power_max", "?snow_power_max", MySqlDbType.Float, editingDisplay.snow_power_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("snow_power_terrain_min", "?snow_power_terrain_min", MySqlDbType.Float, editingDisplay.snow_power_terrain_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("snow_power_terrain_max", "?snow_power_terrain_max", MySqlDbType.Float, editingDisplay.snow_power_terrain_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("snow_min_height", "?snow_min_height", MySqlDbType.Float, editingDisplay.snow_min_height.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("snow_age_min", "?snow_age_min", MySqlDbType.Float, editingDisplay.snow_age_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("snow_age_max", "?snow_age_max", MySqlDbType.Float, editingDisplay.snow_age_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("thunder_power_min", "?thunder_power_min", MySqlDbType.Float, editingDisplay.thunder_power_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("thunder_power_max", "?thunder_power_max", MySqlDbType.Float, editingDisplay.thunder_power_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("cloud_power_min", "?cloud_power_min", MySqlDbType.Float, editingDisplay.cloud_power_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("cloud_power_max", "?cloud_power_max", MySqlDbType.Float, editingDisplay.cloud_power_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("cloud_min_height", "?cloud_min_height", MySqlDbType.Float, editingDisplay.cloud_min_height.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("cloud_max_height", "?cloud_max_height", MySqlDbType.Float, editingDisplay.cloud_max_height.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("cloud_speed_min", "?cloud_speed_min", MySqlDbType.Float, editingDisplay.cloud_speed_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("cloud_speed_max", "?cloud_speed_max", MySqlDbType.Float, editingDisplay.cloud_speed_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("moon_phase_min", "?moon_phase_min", MySqlDbType.Float, editingDisplay.moon_phase_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("moon_phase_max", "?moon_phase_max", MySqlDbType.Float, editingDisplay.moon_phase_max.ToString(), Register.TypesOfField.Float));
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
                editingDisplay.isLoaded = true;
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
            query += " temperature_min=?temperature_min,";
            query += " temperature_max=?temperature_max,";
            query += " humidity_min=?humidity_min,";
            query += " humidity_max=?humidity_max,";
            query += " wind_direction_min=?wind_direction_min,";
            query += " wind_direction_max=?wind_direction_max,";
            query += " wind_speed_min=?wind_speed_min,";
            query += " wind_speed_max=?wind_speed_max,";
            query += " wind_turbulence_min=?wind_turbulence_min,";
            query += " wind_turbulence_max=?wind_turbulence_max,";
            query += " fog_height_power_min=?fog_height_power_min,";
            query += " fog_height_power_max=?fog_height_power_max,";
            query += " fog_height_max=?fog_height_max,";
            query += " fog_distance_power_min=?fog_distance_power_min,";
            query += " fog_distance_power_max=?fog_distance_power_max,";
            query += " fog_distance_max=?fog_distance_max,";
            query += " rain_power_min=?rain_power_min,";
            query += " rain_power_max=?rain_power_max,";
            query += " rain_power_terrain_min=?rain_power_terrain_min,";
            query += " rain_power_terrain_max=?rain_power_terrain_max,";
            query += " rain_min_height=?rain_min_height,";
            query += " rain_max_height=?rain_max_height,";
            query += " hail_power_min=?hail_power_min,";
            query += " hail_power_max=?hail_power_max,";
            query += " hail_power_terrain_min=?hail_power_terrain_min,";
            query += " hail_power_terrain_max=?hail_power_terrain_max,";
            query += " hail_min_height=?hail_min_height,";
            query += " hail_max_height=?hail_max_height,";
            query += " snow_power_min=?snow_power_min,";
            query += " snow_power_max=?snow_power_max,";
            query += " snow_power_terrain_min=?snow_power_terrain_min,";
            query += " snow_power_terrain_max=?snow_power_terrain_max,";
            query += " snow_min_height=?snow_min_height,";
            query += " snow_age_min=?snow_age_min,";
            query += " snow_age_max=?snow_age_max,";
            query += " thunder_power_min=?thunder_power_min,";
            query += " thunder_power_max=?thunder_power_max,";
            query += " cloud_power_min=?cloud_power_min,";
            query += " cloud_power_max=?cloud_power_max,";
            query += " cloud_min_height=?cloud_min_height,";
            query += " cloud_max_height=?cloud_max_height,";
            query += " cloud_speed_min=?cloud_speed_min,";
            query += " cloud_speed_max=?cloud_speed_max,";
            query += " moon_phase_min=?moon_phase_min,";
            query += " moon_phase_max=?moon_phase_max,";
            query += " isactive=?isactive ";
            query += " WHERE id=?id ";
            // Setup the register data		
            List<Register> update = new List<Register>();
            update.Add(new Register("name", "?name", MySqlDbType.VarChar, editingDisplay.Name.ToString(), Register.TypesOfField.String));
            update.Add(new Register("temperature_min", "?temperature_min", MySqlDbType.Float, editingDisplay.temperature_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("temperature_max", "?temperature_max", MySqlDbType.Float, editingDisplay.temperature_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("humidity_min", "?humidity_min", MySqlDbType.Float, editingDisplay.humidity_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("humidity_max", "?humidity_max", MySqlDbType.Float, editingDisplay.humidity_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("wind_direction_min", "?wind_direction_min", MySqlDbType.Float, editingDisplay.wind_direction_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("wind_direction_max", "?wind_direction_max", MySqlDbType.Float, editingDisplay.wind_direction_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("wind_speed_min", "?wind_speed_min", MySqlDbType.Float, editingDisplay.wind_speed_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("wind_speed_max", "?wind_speed_max", MySqlDbType.Float, editingDisplay.wind_speed_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("wind_turbulence_min", "?wind_turbulence_min", MySqlDbType.Float, editingDisplay.wind_turbulence_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("wind_turbulence_max", "?wind_turbulence_max", MySqlDbType.Float, editingDisplay.wind_turbulence_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("fog_height_power_min", "?fog_height_power_min", MySqlDbType.Float, editingDisplay.fog_height_power_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("fog_height_power_max", "?fog_height_power_max", MySqlDbType.Float, editingDisplay.fog_height_power_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("fog_height_max", "?fog_height_max", MySqlDbType.Float, editingDisplay.fog_height_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("fog_distance_power_min", "?fog_distance_power_min", MySqlDbType.Float, editingDisplay.fog_distance_power_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("fog_distance_power_max", "?fog_distance_power_max", MySqlDbType.Float, editingDisplay.fog_distance_power_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("fog_distance_max", "?fog_distance_max", MySqlDbType.Float, editingDisplay.fog_distance_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("rain_power_min", "?rain_power_min", MySqlDbType.Float, editingDisplay.rain_power_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("rain_power_max", "?rain_power_max", MySqlDbType.Float, editingDisplay.rain_power_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("rain_power_terrain_min", "?rain_power_terrain_min", MySqlDbType.Float, editingDisplay.rain_power_terrain_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("rain_power_terrain_max", "?rain_power_terrain_max", MySqlDbType.Float, editingDisplay.rain_power_terrain_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("rain_min_height", "?rain_min_height", MySqlDbType.Float, editingDisplay.rain_min_height.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("rain_max_height", "?rain_max_height", MySqlDbType.Float, editingDisplay.rain_max_height.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("hail_power_min", "?hail_power_min", MySqlDbType.Float, editingDisplay.hail_power_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("hail_power_max", "?hail_power_max", MySqlDbType.Float, editingDisplay.hail_power_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("hail_power_terrain_min", "?hail_power_terrain_min", MySqlDbType.Float, editingDisplay.hail_power_terrain_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("hail_power_terrain_max", "?hail_power_terrain_max", MySqlDbType.Float, editingDisplay.hail_power_terrain_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("hail_min_height", "?hail_min_height", MySqlDbType.Float, editingDisplay.hail_min_height.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("hail_max_height", "?hail_max_height", MySqlDbType.Float, editingDisplay.hail_max_height.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("snow_power_min", "?snow_power_min", MySqlDbType.Float, editingDisplay.snow_power_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("snow_power_max", "?snow_power_max", MySqlDbType.Float, editingDisplay.snow_power_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("snow_power_terrain_min", "?snow_power_terrain_min", MySqlDbType.Float, editingDisplay.snow_power_terrain_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("snow_power_terrain_max", "?snow_power_terrain_max", MySqlDbType.Float, editingDisplay.snow_power_terrain_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("snow_min_height", "?snow_min_height", MySqlDbType.Float, editingDisplay.snow_min_height.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("snow_age_min", "?snow_age_min", MySqlDbType.Float, editingDisplay.snow_age_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("snow_age_max", "?snow_age_max", MySqlDbType.Float, editingDisplay.snow_age_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("thunder_power_min", "?thunder_power_min", MySqlDbType.Float, editingDisplay.thunder_power_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("thunder_power_max", "?thunder_power_max", MySqlDbType.Float, editingDisplay.thunder_power_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("cloud_power_min", "?cloud_power_min", MySqlDbType.Float, editingDisplay.cloud_power_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("cloud_power_max", "?cloud_power_max", MySqlDbType.Float, editingDisplay.cloud_power_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("cloud_min_height", "?cloud_min_height", MySqlDbType.Float, editingDisplay.cloud_min_height.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("cloud_max_height", "?cloud_max_height", MySqlDbType.Float, editingDisplay.cloud_max_height.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("cloud_speed_min", "?cloud_speed_min", MySqlDbType.Float, editingDisplay.cloud_speed_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("cloud_speed_max", "?cloud_speed_max", MySqlDbType.Float, editingDisplay.cloud_speed_max.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("moon_phase_min", "?moon_phase_min", MySqlDbType.Float, editingDisplay.moon_phase_min.ToString(), Register.TypesOfField.Float));
            update.Add(new Register("moon_phase_max", "?moon_phase_max", MySqlDbType.Float, editingDisplay.moon_phase_max.ToString(), Register.TypesOfField.Float));
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
            string sql = "SELECT id FROM weather_instance where weather_profile_id = " + editingDisplay.id;
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sql);
            int Count = 0;
            string Ids = "";
            if ((rows != null) && (rows.Count > 0))
            {
                Ids = "\n"+ Lang.GetTranslate("Instance Template Ids")+":";
                Count = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    Ids += " " + data["id"] + ",";
                }
            }

            DatabasePack.SaveLog("Delete WeatherProfile id =" + editingDisplay.id + " assigned to " + Ids );

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

                EditorUtility.DisplayDialog(Lang.GetTranslate("Can't Dalete Wather Profile"), Lang.GetTranslate("You can not delete this Weather Profile because it is assigned in:") + Ids , Lang.GetTranslate("OK"), "");


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