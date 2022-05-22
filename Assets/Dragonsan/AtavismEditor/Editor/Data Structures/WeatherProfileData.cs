using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class WeatherProfile : DataStructure
    {
        public WeatherProfile() : this("", Vector3.zero, "")
        {
        }

        public WeatherProfile(string name, Vector3 loc) : this(name, loc, "")
        {
        }

        public WeatherProfile(string name, Vector3 loc, string gameObject)
        {
            this.Name = name;
            //	this.loc = loc;
            //	this.gameObject = gameObject;

            fields = new Dictionary<string, string>() {
             { "id", "int"},
             {"name", "string" },
             {"temperature_min", "float"},
             {"temperature_max", "float"},
             {"humidity_min", "float"},
             {"humidity_max", "float"},
             {"wind_direction_min", "float"},
             {"wind_direction_max", "float"},
             {"wind_speed_min", "float"},
             {"wind_speed_max", "float"},
             {"wind_turbulence_min", "float"},
             {"wind_turbulence_max", "float"},
             {"fog_height_power_min", "float"},
             {"fog_height_power_max", "float"},
             {"fog_height_max", "float"},
             {"fog_distance_power_min", "float"},
             {"fog_distance_power_max", "float"},
             {"fog_distance_max", "float"},
             {"rain_power_min", "float"},
             {"rain_power_max", "float"},
             {"rain_power_terrein_min", "float"},
             {"rain_power_terrein_max", "float"},
             {"rain_min_height", "float"},
             {"rain_max_height", "float"},
             {"hail_power_min", "float"},
             {"hail_power_max", "float"},
             {"hail_power_terrein_min", "float"},
             {"hail_power_terrein_max", "float"},
             {"hail_min_height", "float"},
             {"hail_max_height", "float"},
             {"snow_power_min", "float"},
             {"snow_power_max", "float"},
             {"snow_power_terrein_min", "float"},
             {"snow_power_terrein_max", "float"},
             {"snow_max_height", "float"},
             {"snow_age_min", "float"},
             {"snow_age_max", "float"},
             {"thunder_power_min", "float"},
             {"thunder_power_max", "float"},
             {"cloud_power_min", "float"},
             {"cloud_power_max", "float"},
             {"cloud_min_height", "float"},
             {"cloud_max_height", "float"},
             {"cloud_speed_min", "float"},
             {"cloud_speed_max", "float"},
             {"moon_phase_min", "float"},
             {"moon_phase_max", "float"},
             { "isactive", "bool"},
        };
        }

         public float temperature_min = 0f;
        public float temperature_max = 20f;
        public float humidity_min = 0f;
        public float humidity_max = 1f;
        public float wind_direction_min = 0f;
        public float wind_direction_max = 1f;
        public float wind_speed_min = 0f;
        public float wind_speed_max = 0f;
        public float wind_turbulence_min = 0f;
        public float wind_turbulence_max = 0f;
        public float fog_height_power_min = 0f;
        public float fog_height_power_max = 0f;
        public float fog_height_max = 0f;
        public float fog_distance_power_min = 0f;
        public float fog_distance_power_max = 0f;
        public float fog_distance_max = 0f;
        public float rain_power_min = 0f;
        public float rain_power_max = 0f;
        public float rain_power_terrain_min = 0f;
        public float rain_power_terrain_max = 0f;
        public float rain_min_height = 0f;
        public float rain_max_height = 0f;
        public float hail_power_min = 0f;
        public float hail_power_max = 0f;
        public float hail_power_terrain_min = 0f;
        public float hail_power_terrain_max = 0f;
        public float hail_min_height = 0f;
        public float hail_max_height = 0f;
        public float snow_power_min = 0f;
        public float snow_power_max = 0f;
        public float snow_power_terrain_min = 0f;
        public float snow_power_terrain_max = 0f;
        public float snow_min_height = 0f;
        public float snow_age_min = 0f;
        public float snow_age_max = 0f;
        public float thunder_power_min = 0f;
        public float thunder_power_max = 0f;
        public float cloud_power_min = 0f;
        public float cloud_power_max = 0f;
        public float cloud_min_height = 0f;
        public float cloud_max_height = 0f;
        public float cloud_speed_min = 0f;
        public float cloud_speed_max = 0f;
        public float moon_phase_min = 0f;
        public float moon_phase_max = 0f;

        public List<WeatherInstace> watherInstaces = new List<WeatherInstace>();
        public List<int> itemsToBeDeleted = new List<int>();

        public WeatherProfile Clone()
        {
            return (WeatherProfile)this.MemberwiseClone();
        }


        public override string GetValue(string fieldKey)
        {
            if (fieldKey == "id")
            {
                return id.ToString();
            }
            else if (fieldKey == "id")
            {
                return id.ToString();
            }
            else if (fieldKey == "name")
            {
                return Name;
            }
            else if (fieldKey == "temperature_min")
            {
                return temperature_min.ToString();
            }
            else if (fieldKey == "temperature_max")
            {
                return temperature_max.ToString();
            }
            else if (fieldKey == "humidity_min")
            {
                return humidity_min.ToString();
            }
            else if (fieldKey == "humidity_max")
            {
                return humidity_max.ToString();
            }
            else if (fieldKey == "wind_direction_min")
            {
                return wind_direction_min.ToString();
            }
            else if (fieldKey == "wind_direction_max")
            {
                return wind_direction_max.ToString();
            }
            else if (fieldKey == "wind_speed_min")
            {
                return wind_speed_min.ToString();
            }
            else if (fieldKey == "wind_speed_max")
            {
                return wind_speed_max.ToString();
            }
            else if (fieldKey == "wind_turbulence_min")
            {
                return wind_turbulence_min.ToString();
            }
            else if (fieldKey == "wind_turbulence_max")
            {
                return wind_turbulence_max.ToString();
            }
            else if (fieldKey == "fog_height_power_min")
            {
                return fog_height_power_min.ToString();
            }
            else if (fieldKey == "fog_height_power_max")
            {
                return fog_height_power_max.ToString();
            }
            else if (fieldKey == "fog_height_max")
            {
                return fog_height_max.ToString();
            }
            else if (fieldKey == "fog_distance_power_min")
            {
                return fog_distance_power_min.ToString();
            }
            else if (fieldKey == "fog_distance_power_max")
            {
                return fog_distance_power_max.ToString();
            }
            else if (fieldKey == "fog_distance_max")
            {
                return fog_distance_max.ToString();
            }
            else if (fieldKey == "rain_power_min")
            {
                return rain_power_min.ToString();
            }
            else if (fieldKey == "rain_power_max")
            {
                return rain_power_max.ToString();
            }
            else if (fieldKey == "rain_power_terrain_min")
            {
                return rain_power_terrain_min.ToString();
            }
            else if (fieldKey == "rain_power_terrain_max")
            {
                return rain_power_terrain_max.ToString();
            }
            else if (fieldKey == "rain_min_height")
            {
                return rain_min_height.ToString();
            }
            else if (fieldKey == "rain_max_height")
            {
                return rain_max_height.ToString();
            }
            else if (fieldKey == "hail_power_min")
            {
                return hail_power_min.ToString();
            }
            else if (fieldKey == "hail_power_max")
            {
                return hail_power_max.ToString();
            }
            else if (fieldKey == "hail_power_terrain_min")
            {
                return hail_power_terrain_min.ToString();
            }
            else if (fieldKey == "hail_power_terrain_max")
            {
                return hail_power_terrain_max.ToString();
            }
            else if (fieldKey == "hail_min_height")
            {
                return hail_min_height.ToString();
            }
            else if (fieldKey == "snow_power_min")
            {
                return snow_power_min.ToString();
            }
            else if (fieldKey == "snow_power_max")
            {
                return snow_power_max.ToString();
            }
            else if (fieldKey == "snow_power_terrain_min")
            {
                return snow_power_terrain_min.ToString();
            }
            else if (fieldKey == "snow_power_terrain_max")
            {
                return snow_power_terrain_max.ToString();
            }
            else if (fieldKey == "snow_min_height")
            {
                return snow_min_height.ToString();
            }
            else if (fieldKey == "snow_age_min")
            {
                return snow_age_min.ToString();
            }
            else if (fieldKey == "snow_age_max")
            {
                return snow_age_max.ToString();
            }
            else if (fieldKey == "thunder_power_min")
            {
                return thunder_power_min.ToString();
            }
            else if (fieldKey == "thunder_power_max")
            {
                return thunder_power_max.ToString();
            }
            else if (fieldKey == "cloud_power_min")
            {
                return cloud_power_min.ToString();
            }
            else if (fieldKey == "cloud_power_max")
            {
                return cloud_power_max.ToString();
            }
            else if (fieldKey == "cloud_min_height")
            {
                return cloud_min_height.ToString();
            }
            else if (fieldKey == "cloud_max_height")
            {
                return cloud_max_height.ToString();
            }
            else if (fieldKey == "cloud_speed_min")
            {
                return cloud_speed_min.ToString();
            }
            else if (fieldKey == "cloud_speed_max")
            {
                return cloud_speed_max.ToString();
            }
            else if (fieldKey == "moon_phase_min")
            {
                return moon_phase_min.ToString();
            }
            else if (fieldKey == "moon_phase_max")
            {
                return moon_phase_max.ToString();
            }
            return "";
        }
    }

    // Structure of a Atavism Instance
    public class WeatherInstace : DataStructure
    {
        public int instance_id = 0;              // Database index
                                                 // General Parameters
        public int weather_profile_id = 0;
        public string search = "";
        public bool month1 = false;
        public bool month2 = false;
        public bool month3 = false;
        public bool month4 = false;
        public bool month5 = false;
        public bool month6 = false;
        public bool month7 = false;
        public bool month8 = false;
        public bool month9 = false;
        public bool month10 = false;
        public bool month11 = false;
        public bool month12 = false;
        public int priority = 1;

        public WeatherInstace()
        {

            // Database fields
            fields = new Dictionary<string, string>() {
           {"id", "int"},
            { "instance_id", "int"},
        {"weather_profile_id", "int"},
        {"month1", "bool"},
        {"month2", "bool"},
        {"month3", "bool"},
        {"month4", "bool"},
        {"month5", "bool"},
        {"month6", "bool"},
        {"month7", "bool"},
        {"month8", "bool"},
        {"month9", "bool"},
        {"month10", "bool"},
        {"month11", "bool"},
        {"month12", "bool"},
        {"priority","int" }
    };
        }

        public WeatherInstace Clone()
        {
            return (WeatherInstace)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "id":
                    return id.ToString();
                case "instance_id":
                    return instance_id.ToString();
                case "weather_profile_id":
                    return weather_profile_id.ToString();
                case "month1":
                    return month1.ToString();
                case "month2":
                    return month2.ToString();
                case "month3":
                    return month3.ToString();
                case "month4":
                    return month4.ToString();
                case "month5":
                    return month5.ToString();
                case "month6":
                    return month6.ToString();
                case "month7":
                    return month7.ToString();
                case "month8":
                    return month8.ToString();
                case "month9":
                    return month9.ToString();
                case "month10":
                    return month10.ToString();
                case "month11":
                    return month11.ToString();
                case "month12":
                    return month12.ToString();
                case "priority":
                    return priority.ToString();
            }
            return "";
        }
    }


    public class WeatherSeasion : DataStructure
    {
        public int instance_id = 0;              // Database index
                                                 // General Parameters
        public int season = 0;
        public static string[] seasonTypes = new string[] { "Winter", "Spring", "Summer", "Autumn" };
        public bool month1 = false;
        public bool month2 = false;
        public bool month3 = false;
        public bool month4 = false;
        public bool month5 = false;
        public bool month6 = false;
        public bool month7 = false;
        public bool month8 = false;
        public bool month9 = false;
        public bool month10 = false;
        public bool month11 = false;
        public bool month12 = false;

        public WeatherSeasion()
        {

            // Database fields
            fields = new Dictionary<string, string>() {
                    {"id", "int"},
                { "instance_id", "int"},
        {"season", "int"},
        {"month1", "bool"},
        {"month2", "bool"},
        {"month3", "bool"},
        {"month4", "bool"},
        {"month5", "bool"},
        {"month6", "bool"},
        {"month7", "bool"},
        {"month8", "bool"},
        {"month9", "bool"},
        {"month10", "bool"},
        {"month11", "bool"},
        {"month12", "bool"},
    };
        }

        public WeatherSeasion Clone()
        {
            return (WeatherSeasion)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "id":
                    return id.ToString();
                case "instance_id":
                    return instance_id.ToString();
                case "season":
                    return season.ToString();
                case "month1":
                    return month1.ToString();
                case "month2":
                    return month2.ToString();
                case "month3":
                    return month3.ToString();
                case "month4":
                    return month4.ToString();
                case "month5":
                    return month5.ToString();
                case "month6":
                    return month6.ToString();
                case "month7":
                    return month7.ToString();
                case "month8":
                    return month8.ToString();
                case "month9":
                    return month9.ToString();
                case "month10":
                    return month10.ToString();
                case "month11":
                    return month11.ToString();
                case "month12":
                    return month12.ToString();
            }
            return "";
        }

    }

}