using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class SetData : DataStructure
    {
        public SetData() : this(-1, "")
        {
        }

        public SetData(int id, string Name)
        {
            this.id = id;
            this.Name = Name;
        }
        public List<SetLevelData> levelList = new List<SetLevelData>();
        public List<int> itemList = new List<int>();
        public List<int> itemListDeleted = new List<int>();
        public SetData Clone()
        {
            return (SetData)this.MemberwiseClone();
        }

    }


    public class SetLevelData : DataStructure
    {
                                            // General Parameters
        public int number_of_parts = 1;               // The level to
        public int set_id = 0;             // The enchant cost 
        public int damage = 0;
        public int damagep = 0;
        public int maxStatsEntries = 32;
        public List<StatEntry> stats = new List<StatEntry>();
        public SetLevelData() : this(0, 1, 0)
        {

        }

        public SetLevelData(int id, int parts, int setid)
        {
            this.id = id;
            this.set_id = setid;
            this.number_of_parts = parts;
            // Database fields
            fields = new Dictionary<string, string>() {
     //   {"id", "int"},
        {"number_of_parts", "int"},
        {"set_id", "int"},
         {"damage", "int"},
        {"damagep", "int"},
        {"effect1valuep", "int"},
        {"effect1name", "string"},
        {"effect1value", "int"},
        {"effect2valuep", "int"},
        {"effect2name", "string"},
        {"effect2value", "int"},
        {"effect3valuep", "int"},
        {"effect3name", "string"},
        {"effect3value", "int"},
        {"effect4valuep", "int"},
        {"effect4name", "string"},
        {"effect4value", "int"},
        {"effect5valuep", "int"},
        {"effect5name", "string"},
        {"effect5value", "int"},
        {"effect6valuep", "int"},
        {"effect6name", "string"},
        {"effect6value", "int"},
        {"effect7valuep", "int"},
        {"effect7name", "string"},
        {"effect7value", "int"},
        {"effect8valuep", "int"},
        {"effect8name", "string"},
        {"effect8value", "int"},
        {"effect9valuep", "int"},
        {"effect9name", "string"},
        {"effect9value", "int"},
        {"effect10valuep", "int"},
        {"effect10name", "string"},
        {"effect10value", "int"},
        {"effect11valuep", "int"},
        {"effect11name", "string"},
        {"effect11value", "int"},
        {"effect12valuep", "int"},
        {"effect12name", "string"},
        {"effect12value", "int"},
        {"effect13valuep", "int"},
        {"effect13name", "string"},
        {"effect13value", "int"},
        {"effect14valuep", "int"},
        {"effect14name", "string"},
        {"effect14value", "int"},
        {"effect15valuep", "int"},
        {"effect15name", "string"},
        {"effect15value", "int"},
        {"effect16valuep", "int"},
        {"effect16name", "string"},
        {"effect16value", "int"},
        {"effect17valuep", "int"},
        {"effect17name", "string"},
        {"effect17value", "int"},
        {"effect18valuep", "int"},
        {"effect18name", "string"},
        {"effect18value", "int"},
        {"effect19valuep", "int"},
        {"effect19name", "string"},
        {"effect19value", "int"},
        {"effect20valuep", "int"},
        {"effect20name", "string"},
        {"effect20value", "int"},
        {"effect21valuep", "int"},
        {"effect21name", "string"},
        {"effect21value", "int"},
        {"effect22valuep", "int"},
        {"effect22name", "string"},
        {"effect22value", "int"},
        {"effect23valuep", "int"},
        {"effect23name", "string"},
        {"effect23value", "int"},
        {"effect24valuep", "int"},
        {"effect24name", "string"},
        {"effect24value", "int"},
        {"effect25valuep", "int"},
        {"effect25name", "string"},
        {"effect25value", "int"},
        {"effect26valuep", "int"},
        {"effect26name", "string"},
        {"effect26value", "int" },
        {"effect27valuep", "int"},
        {"effect27name", "string"},
        {"effect27value", "int"},
        {"effect28valuep", "int"},
        {"effect28name", "string"},
        {"effect28value", "int"},
        {"effect29valuep", "int"},
        {"effect29name", "string"},
        {"effect29value", "int"},
        {"effect30valuep", "int"},
        {"effect30name", "string"},
        {"effect30value", "int"},
        {"effect31valuep", "int"},
        {"effect31name", "string"},
        {"effect31value", "int"},
        {"effect32valuep", "int"},
        {"effect32name", "string"},
        {"effect32value", "int"},
};
        }

        public SetLevelData Clone()
        {
            return (SetLevelData)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "id":
                    return id.ToString();
                case "set_id":
                    return set_id.ToString();
                case "number_of_parts":
                    return number_of_parts.ToString();
                case "damage":
                    return damage.ToString();
                case "damagep":
                    return damagep.ToString();
                case "effect1valuep":
                    return getEffectData(0, "percentage");
                case "effect1name":
                    return getEffectData(0, "name");
                case "effect1value":
                    return getEffectData(0, "value");
                case "effect2valuep":
                    return getEffectData(1, "percentage");
                case "effect2name":
                    return getEffectData(1, "name");
                case "effect2value":
                    return getEffectData(1, "value");
                case "effect3valuep":
                    return getEffectData(2, "percentage");
                case "effect3name":
                    return getEffectData(2, "name");
                case "effect3value":
                    return getEffectData(2, "value");
                case "effect4valuep":
                    return getEffectData(3, "percentage");
                case "effect4name":
                    return getEffectData(3, "name");
                case "effect4value":
                    return getEffectData(3, "value");
                case "effect5valuep":
                    return getEffectData(4, "percentage");
                case "effect5name":
                    return getEffectData(4, "name");
                case "effect5value":
                    return getEffectData(4, "value");
                case "effect6valuep":
                    return getEffectData(5, "percentage");
                case "effect6name":
                    return getEffectData(5, "name");
                case "effect6value":
                    return getEffectData(5, "value");
                case "effect7valuep":
                    return getEffectData(6, "percentage");
                case "effect7name":
                    return getEffectData(6, "name");
                case "effect7value":
                    return getEffectData(6, "value");
                case "effect8valuep":
                    return getEffectData(7, "percentage");
                case "effect8name":
                    return getEffectData(7, "name");
                case "effect8value":
                    return getEffectData(7, "value");
                case "effect9valuep":
                    return getEffectData(8, "percentage");
                case "effect9name":
                    return getEffectData(8, "name");
                case "effect9value":
                    return getEffectData(8, "value");
                case "effect10valuep":
                    return getEffectData(9, "percentage");
                case "effect10name":
                    return getEffectData(9, "name");
                case "effect10value":
                    return getEffectData(9, "value");
                case "effect11valuep":
                    return getEffectData(10, "percentage");
                case "effect11name":
                    return getEffectData(10, "name");
                case "effect11value":
                    return getEffectData(10, "value");
                case "effect12valuep":
                    return getEffectData(11, "percentage");
                case "effect12name":
                    return getEffectData(11, "name");
                case "effect12value":
                    return getEffectData(11, "value");

                case "effect13valuep":
                    return getEffectData(12, "percentage");
                case "effect13name":
                    return getEffectData(12, "name");
                case "effect13value":
                    return getEffectData(12, "value");
                case "effect14valuep":
                    return getEffectData(13, "percentage");
                case "effect14name":
                    return getEffectData(13, "name");
                case "effect14value":
                    return getEffectData(13, "value");
                case "effect15valuep":
                    return getEffectData(14, "percentage");
                case "effect15name":
                    return getEffectData(14, "name");
                case "effect15value":
                    return getEffectData(14, "value");
                case "effect16valuep":
                    return getEffectData(15, "percentage");
                case "effect16name":
                    return getEffectData(15, "name");
                case "effect16value":
                    return getEffectData(15, "value");
                case "effect17valuep":
                    return getEffectData(16, "percentage");
                case "effect17name":
                    return getEffectData(16, "name");
                case "effect17value":
                    return getEffectData(16, "value");
                case "effect18valuep":
                    return getEffectData(17, "percentage");
                case "effect18name":
                    return getEffectData(17, "name");
                case "effect18value":
                    return getEffectData(17, "value");
                case "effect19valuep":
                    return getEffectData(18, "percentage");
                case "effect19name":
                    return getEffectData(18, "name");
                case "effect19value":
                    return getEffectData(18, "value");
                case "effect20valuep":
                    return getEffectData(19, "percentage");
                case "effect20name":
                    return getEffectData(19, "name");
                case "effect20value":
                    return getEffectData(19, "value");
                case "effect21valuep":
                    return getEffectData(20, "percentage");
                case "effect21name":
                    return getEffectData(20, "name");
                case "effect21value":
                    return getEffectData(20, "value");
                case "effect22valuep":
                    return getEffectData(21, "percentage");
                case "effect22name":
                    return getEffectData(21, "name");
                case "effect22value":
                    return getEffectData(21, "value");
                case "effect23valuep":
                    return getEffectData(22, "percentage");
                case "effect23name":
                    return getEffectData(22, "name");
                case "effect23value":
                    return getEffectData(22, "value");
                case "effect24valuep":
                    return getEffectData(23, "percentage");
                case "effect24name":
                    return getEffectData(23, "name");
                case "effect24value":
                    return getEffectData(23, "value");
                case "effect25valuep":
                    return getEffectData(24, "percentage");
                case "effect25name":
                    return getEffectData(24, "name");
                case "effect25value":
                    return getEffectData(24, "value");
                case "effect26valuep":
                    return getEffectData(25, "percentage");
                case "effect26name":
                    return getEffectData(25, "name");
                case "effect26value":
                    return getEffectData(25, "value");
                case "effect27valuep":
                    return getEffectData(26, "percentage");
                case "effect27name":
                    return getEffectData(26, "name");
                case "effect27value":
                    return getEffectData(26, "value");
                case "effect28valuep":
                    return getEffectData(27, "percentage");
                case "effect28name":
                    return getEffectData(27, "name");
                case "effect28value":
                    return getEffectData(27, "value");
                case "effect29valuep":
                    return getEffectData(28, "percentage");
                case "effect29name":
                    return getEffectData(28, "name");
                case "effect29value":
                    return getEffectData(28, "value");
                case "effect30valuep":
                    return getEffectData(29, "percentage");
                case "effect30name":
                    return getEffectData(29, "name");
                case "effect30value":
                    return getEffectData(29, "value");
                case "effect31valuep":
                    return getEffectData(30, "percentage");
                case "effect31name":
                    return getEffectData(30, "name");
                case "effect31value":
                    return getEffectData(30, "value");
                case "effect32valuep":
                    return getEffectData(31, "percentage");
                case "effect32name":
                    return getEffectData(31, "name");
                case "effect32value":
                    return getEffectData(31, "value");

            }
            return "";
        }

        string getEffectData(int effectNum, string field)
        {
            if (stats.Count > effectNum)
            {
                if (field == "percentage")
                {
                    return stats[effectNum].itemStatValuePercentage.ToString();
                }
                else if (field == "name")
                {
                    return stats[effectNum].itemStatName;
                }
                else if (field == "value")
                {
                    return stats[effectNum].itemStatValue.ToString();
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

    }
}