using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class SkillLevelProfileData : DataStructure
    {
         public string profileName = "";
        public int type = 0;
        public int generate_to_level = 100;
        public int generate_base_value = 100;
        public float generate_percentage_value = 100f;
        public List<SkillLevelData> levelExp = new List<SkillLevelData>();
        public List<SkillLevelDiffData> levelDiffExp = new List<SkillLevelDiffData>();

        public SkillLevelProfileData()
        {
            id = -1;
            usesID = false;
            // Database fields
            fields = new Dictionary<string, string>() {
        {"profile_name", "string"},
        {"level_diff", "string"},
        {"type", "int"},
         };
        }
        public SkillLevelProfileData Clone()
        {
            return (SkillLevelProfileData)this.MemberwiseClone();
        }
        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "profile_name":
                    return profileName;
                case "type":
                    return type.ToString();
                case "level_diff":
                    string s = "";
                    foreach (SkillLevelDiffData sldd in levelDiffExp)
                    {
                        s += sldd.xpPercantage + ";";
                        //    Debug.LogWarning(sldd.xpPercantage + " s:" + s);
                    }
                    if (s.Length > 0)
                        return s.Remove(s.Length - 1);
                    else
                        return s;
            }
            return "";
        }

    }
    public class SkillLevelDiffData
    {
        public int id = -1;
        public float xpPercantage = 100f;
        public SkillLevelDiffData() : this(100f) { }
        public SkillLevelDiffData(float xpPercantage)
        {
            this.xpPercantage = xpPercantage;
        }
    }


    public class SkillLevelData : DataStructure
    {
                                             // General Parameters
        public int profile_id = -1;
        public int level = 1;
        public int xpRequired = 100;

        public SkillLevelData()
        {
            id = -1;
            usesID = false;
            // Database fields
            fields = new Dictionary<string, string>() {
        {"profile_id", "int"},
        {"level", "int"},
        {"required_xp", "int"},
    };
        }

        public SkillLevelData Clone()
        {
            return (SkillLevelData)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "profile_id":
                    return profile_id.ToString();
                case "level":
                    return level.ToString();
                case "required_xp":
                    return xpRequired.ToString();
            }
            return "";
        }
    }
}