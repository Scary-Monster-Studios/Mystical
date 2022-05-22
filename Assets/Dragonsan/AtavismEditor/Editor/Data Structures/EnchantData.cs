using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class EnchantData : DataStructure
    {
        public EnchantData() : this(-1, "")
        {
        }

        public EnchantData(int id, string Name)
        {
            this.id = id;
            this.Name = Name;
        }
        public int lower_by = 0;
        public int lower_to = -1;
        public string[] failedOptions = new string[] { "Lower by", "Lower to" };
        public string failed = "Lower by";
        public List<EnchantLevelData> enchantLevels = new List<EnchantLevelData>();
        public EnchantData Clone()
        {
            return (EnchantData)this.MemberwiseClone();
        }

    }

    public class StatEntry
    {
        public StatEntry(string itemStatName, int itemStatValue, int itemStatValuePercentage)
        {
            this.itemStatValue = itemStatValue;
            this.itemStatName = itemStatName;
            this.itemStatValuePercentage = itemStatValuePercentage;
        }
        public StatEntry(string itemStatName, int itemStatValue, float itemStatValuePercentagef)
        {
            this.itemStatValue = itemStatValue;
            this.itemStatName = itemStatName;
            this.itemStatValuePercentagef = itemStatValuePercentagef;
        }

        public int itemStatValuePercentage = 0;
        public float itemStatValuePercentagef = 0f;
        public string itemStatName;
        public int itemStatValue = 0;
    }

    public class EnchantLevelData : DataStructure
    {
                                            // General Parameters
        public int level = 1;               // The level to
        public int cost = 1000;             // The enchant cost 
        public float chance = 100f;         // Chance to enchante
        public bool allStats = false;       // All on item modified by stat_value and percentage
        public bool percentage = false;     // Make percentage modifie all stats by stat_value
        public int stat_value = 0;
        public bool add_not_exist = false;  // Add defined stat to item if not exist in base stats
        public int damage = 0;
        public int damagep = 0;
        public int gear_score = 0;
        public int gear_scorep = 0;
        public int lower_by = 0;
        public int lower_to = -1;
        public int currency = 1;
        public string[] failedOptions = new string[] { "Lower by", "Lower to" };

        // Fields common to weapons and armor
        public string failed = "";

        public int maxStatsEntries = 32;
        public List<StatEntry> stats = new List<StatEntry>();
        public EnchantLevelData() : this("")
        {

        }

        public EnchantLevelData(string name)
        {
            this.Name = name;
            // Database fields
            fields = new Dictionary<string, string>() {
     //   {"id", "int"},
        {"Name", "string"},
        {"level", "int"},
        {"cost", "int"},
        {"currency", "int"},
        {"chance", "float"},
        {"all_stats", "bool"},
        {"percentage", "bool"},
        {"stat_value", "int"},
        {"add_not_exist", "bool"},
        {"damage", "int"},
        {"damagep", "int"},
        {"lower_by", "int"},
        {"lower_to", "int"},
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
        {"gear_score", "int"},
        {"gear_scorep", "int"},

};
        }

        public EnchantLevelData Clone()
        {
            return (EnchantLevelData)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "id":
                    return id.ToString();
                case "Name":
                    return Name;
                case "level":
                    return level.ToString();
                case "chance":
                    return chance.ToString();
                case "cost":
                    return cost.ToString();
                case "currency":
                    return currency.ToString();
                case "all_stats":
                    return allStats.ToString();
                case "percentage":
                    return percentage.ToString();
                case "stat_value":
                    return stat_value.ToString();
                case "add_not_exist":
                    return add_not_exist.ToString();
                case "damage":
                    return damage.ToString();
                case "damagep":
                    return damagep.ToString();
                case "lower_by":
                    return lower_by.ToString();
                case "lower_to":
                    return lower_to.ToString();
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
                case "gear_score":
                    return gear_score.ToString();
                case "gear_scorep":
                    return gear_scorep.ToString();

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

    public class QualityData : DataStructure
    {
        public QualityData()
        {
        }
        public List<QualityEntry> list = new List<QualityEntry>();

    }

    public class QualityEntry
    {
        public QualityEntry()
        {
        }

        public QualityEntry(string Name) { this.Name = Name; }
        public QualityEntry(int id, string Name, float Cost, float Chance)
        {
            this.id = id;
            this.Name = Name;
            this.cost = Cost;
            this.chance = Chance;
        }

        public int id = -1;
        public string Name = "";
        public float cost = 100f;
        public float chance = 100f;
    }
}