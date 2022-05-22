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
    public class StatLinkEntry : DataStructure
    {
        public StatLinkEntry() : this("", "", 0)
        {
        }

        public StatLinkEntry(string stat, string statTo, int changePerPoint)
        {
            this.stat = stat;
            this.statTo = statTo;
            this.changePerPoint = changePerPoint;

            fields = new Dictionary<string, string>() {
            {"stat", "string"},
            {"statTo", "string"},
            {"changePerPoint", "int"},
        };
        }

        public string stat;
        public string statTo;
        public int changePerPoint = 1;

        public StatLinkEntry Clone()
        {
            return (StatLinkEntry)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            if (fieldKey == "id")
            {
                return id.ToString();
            }
            else if (fieldKey == "stat")
            {
                return stat;
            }
            else if (fieldKey == "statTo")
            {
                return statTo;
            }
            else if (fieldKey == "changePerPoint")
            {
                return changePerPoint.ToString();
            }
            return "";
        }
    }

    public class StatsData : DataStructure
    {
                                             // General Parameters
        public string originalName = "";
        public int type = 0;
        public string[] typeOptions = new string[] {
        "Base stat",
        "Resistance stat",
        "Vitality stat"
    };
        public string statFunction = "~ none ~";
        public int mobBase = 0;
        public int mobLevelIncrease = 0;
        public float mobLevelPercentIncrease = 0;
        public int min = 0;
        public string maxstat = "";
        public bool canExceedMax = false;
        public bool sharedWithGroup = false;
        public string[] targetOptions = new string[] {
        "All",
        "Player Only",
        "Mob Only"
    };
        public string shiftModStat = "";
        public int shiftTarget = 2;
        public int shiftValue = 0;
        public int shiftReverseValue = 0;
        public int shiftInterval = 2;
        public bool isShiftPercent = true;
        public string onMaxHit = "";
        public string onMinHit = "";
        public string onThresholdHit = "";
        public string onThreshold2Hit = "";
        public string onThreshold3Hit = "";
        public string onThreshold4Hit = "";
        public string onThreshold5Hit = "";
        public float threshold = -1f;
        public float threshold2 = -1f;
        public float threshold3 = -1f;
        public float threshold4 = -1f;
        public string shiftReq1 = "";
        public bool shiftReq1State = false;
        public bool shiftReq1SetReverse = false;
        public string shiftReq2 = "";
        public bool shiftReq2State = false;
        public bool shiftReq2SetReverse = false;
        public string shiftReq3 = "";
        public bool shiftReq3State = false;
        public bool shiftReq3SetReverse = false;
        public int startPercent = 50;
        public int deathResetPercent = -1;
        public int releaseResetPercent = -1;

        public List<StatLinkEntry> statLinks = new List<StatLinkEntry>();

        public List<int> linksToBeDeleted = new List<int>();

        public StatsData()
        {
            usesID = false;
            // Database fields
            fields = new Dictionary<string, string>() {
        {"name", "string"},
        {"type", "int"},
        {"stat_function", "string"},
        {"mob_base", "int"},
        {"mob_level_increase", "int"},
        {"mob_level_percent_increase", "float"},
        {"min", "int"},
        {"maxstat", "string"},
        {"canExceedMax", "bool"},
        {"sharedWithGroup", "bool"},
        {"shiftTarget", "int"},
        {"shiftValue", "int"},
        {"shiftReverseValue", "int"},
        {"shiftInterval", "int"},
        {"isShiftPercent", "bool"},
        {"onMaxHit", "string"},
        {"onMinHit", "string"},
        {"shiftReq1", "string"},
        {"shiftReq1State", "bool"},
        {"shiftReq1SetReverse", "bool"},
        {"shiftReq2", "string"},
        {"shiftReq2State", "bool"},
        {"shiftReq2SetReverse", "bool"},
        {"shiftReq3", "string"},
        {"shiftReq3State", "bool"},
        {"shiftReq3SetReverse", "bool"},
        {"startPercent", "int"},
        {"deathResetPercent", "int"},
        {"releaseResetPercent", "int"},
        {"onThreshold", "string"},
        {"onThreshold2", "string"},
        {"onThreshold3", "string"},
        {"onThreshold4", "string"},
        {"onThreshold5", "string"},
        {"threshold", "float"},
        {"threshold2", "float"},
        {"threshold3", "float"},
        {"threshold4", "float"},
        {"shiftModStat","string" },

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
                case "type":
                    return type.ToString();
                case "stat_function":
                    return statFunction;
                case "mob_base":
                    return mobBase.ToString();
                case "mob_level_increase":
                    return mobLevelIncrease.ToString();
                case "mob_level_percent_increase":
                    return mobLevelPercentIncrease.ToString();
                case "min":
                    return min.ToString();
                case "maxstat":
                    return maxstat;
                case "canExceedMax":
                    return canExceedMax.ToString();
                case "sharedWithGroup":
                    return sharedWithGroup.ToString();
                case "shiftTarget":
                    return shiftTarget.ToString();
                case "shiftValue":
                    return shiftValue.ToString();
                case "shiftReverseValue":
                    return shiftReverseValue.ToString();
                case "shiftInterval":
                    return shiftInterval.ToString();
                case "isShiftPercent":
                    return isShiftPercent.ToString();
                case "onMaxHit":
                    return onMaxHit;
                case "onMinHit":
                    return onMinHit;
                case "shiftReq1":
                    return shiftReq1;
                case "shiftReq1State":
                    return shiftReq1State.ToString();
                case "shiftReq1SetReverse":
                    return shiftReq1SetReverse.ToString();
                case "shiftReq2":
                    return shiftReq2;
                case "shiftReq2State":
                    return shiftReq2State.ToString();
                case "shiftReq2SetReverse":
                    return shiftReq2SetReverse.ToString();
                case "shiftReq3":
                    return shiftReq3;
                case "shiftReq3State":
                    return shiftReq3State.ToString();
                case "shiftReq3SetReverse":
                    return shiftReq3SetReverse.ToString();
                case "startPercent":
                    return startPercent.ToString();
                case "deathResetPercent":
                    return deathResetPercent.ToString();
                case "releaseResetPercent":
                    return releaseResetPercent.ToString();
                case "threshold":
                    return threshold.ToString();
                case "threshold2":
                    return threshold2.ToString();
                case "threshold3":
                    return threshold3.ToString();
                case "threshold4":
                    return threshold4.ToString();
                case "onThreshold":
                    return onThresholdHit;
                case "onThreshold2":
                    return onThreshold2Hit;
                case "onThreshold3":
                    return onThreshold3Hit;
                case "onThreshold4":
                    return onThreshold4Hit;
                case "onThreshold5":
                    return onThreshold5Hit;
                case "shiftModStat":
                    return shiftModStat;

            }
            return "";
        }
    }
}