using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Structure of a Atavism Stats
/*
/* Table structure for tables
/*
 CREATE TABLE `stat` (
  `name` varchar(45) NOT NULL,
  `type` int(11) DEFAULT '0',
  `stat_function` varchar(45) DEFAULT NULL,
  
  Stat table:

name - String - The name of the stat.
type - Integer - Use a drop down with the following: 
(0: Base stat - this is for like strength, agility etc; and 1: Resistance stat - for armour etc)
stat_function - String - What function the stat serves, only used for base stats. 
It only wants the following options: 
health_mod, mana_mod (these two effect the character's health/mana). physical_power, magical_power (these two effect the damage done by the characters abilities). physical_accuracy, magical_accuracy (these two effect the chance of hitting with an ability). See my attached sql file to see examples.  
*/
namespace Atavism
{
    public class TriggerActionEntry : DataStructure
    {
        public TriggerActionEntry() : this(0, 0, 0, 100)
        {
        }

        public TriggerActionEntry(int ability, int effect, float chance_min, float chance_max)
        {
            this.ability = ability;
            this.effect = effect;
            this.chance_min = chance_min;
            this.chance_max = chance_max;

            fields = new Dictionary<string, string>() {
            {"ability", "int"},
            {"effect", "int"},
            {"chance_min", "float"},
            {"chance_max", "float"},
            {"mod_v", "int"},
            {"mod_p", "float"},
                {"effects_triggers_id", "int" },
                 {"target", "int"},

        };
        }

        public float chance_min = 0F;
        public float chance_max = 100F;
        public int ability = 0;
        public string abilitySearch = "";
        public int effect = 0;
        public string effectSearch = "";
        public int actionType = 0;
        public int mod_v = 0;
        public float mod_p = 0;
        public int effects_triggers_id = -1;
        public int target = 0;

        public TriggerActionEntry Clone()
        {
            return (TriggerActionEntry)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            if (fieldKey == "id")
            {
                return id.ToString();
            }
            else if (fieldKey == "ability")
            {
                return ability.ToString();
            }
            else if (fieldKey == "effect")
            {
                return effect.ToString();
            }
            else if (fieldKey == "chance_min")
            {
                return chance_min.ToString();
            }
            else if (fieldKey == "chance_max")
            {
                return chance_max.ToString();
            }
            else if (fieldKey == "mod_v")
            {
                return mod_v.ToString();
            }
            else if (fieldKey == "mod_p")
            {
                return mod_p.ToString();
            }
            else if (fieldKey == "effects_triggers_id")
            {
                return effects_triggers_id.ToString();
            }
            else if (fieldKey == "target")
                return target.ToString();
            return "";
        }
    }

    public class EffectsTriggerData : DataStructure
    {
                                             // General Parameters
        public string originalName = "";
        public int eventType = 0;
        public string[] eventTypeOptions = new string[] {
        "Dodge",
        "Miss",
        "Damage",
        "Heal",
        "Critical",
        "Kill",
        "Parry",
        "Sleep",
        "Stun"
        };
        public string[] targetOptions = new string[] {
        "All",
        "Caster",
        "Target"
        };
        public string[] actionTypeOptions = new string[] {
        "Dealt",
        "Received"
        };
        public string[] abilityActionTypeOptions = new string[] {
        "Ability",
        "Effect"
        };
        public string[] effectActionTypeOptions = new string[] {
        "Ability",
        "Effect",
        "Modifier"
        };


        public float chance_min = 0;
        public float chance_max = 0;
        public List<int> tags = new List<int>();
        public List<string> tagSearch = new List<string>();
        public int Race = 0;
        public int Class = 0;
        public int actionType = 0;
     //   public int target = 0;



        public List<TriggerActionEntry> actions = new List<TriggerActionEntry>();

        public List<int> actionsToBeDeleted = new List<int>();

        public EffectsTriggerData()
        {
            usesID = false;
            // Database fields
            fields = new Dictionary<string, string>() {
        {"name", "string"},
        {"event_type", "int"},
        {"tags", "string"},
        {"race", "int"},
        {"class", "int"},
        {"action_type", "int"},
       // {"target", "int"},
        {"chance_min", "float"},
        {"chance_max", "float"},
        };
        }

        public StatsData Clone()
        {
            return (StatsData)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "name":
                    return Name;
                case "event_type":
                    return eventType.ToString();
                case "tags":
                    return string.Join(";", tags.ConvertAll(i => i.ToString()).ToArray());
                case "race":
                    return Race.ToString();
                case "class":
                    return Class.ToString();
                case "action_type":
                    return actionType.ToString();
              //  case "target":
              //      return target.ToString();
                case "chance_min":
                    return chance_min.ToString();
                case "chance_max":
                    return chance_max.ToString();
            }
            return "";
        }
    }
}