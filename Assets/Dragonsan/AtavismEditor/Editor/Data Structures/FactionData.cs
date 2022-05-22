using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class FactionStanceEntry : DataStructure
    {
        public FactionStanceEntry() : this(-1, 0, -1)
        {
        }

        public FactionStanceEntry(int otherFactionID, int defaultStance) : this(otherFactionID, defaultStance, -1)
        {
        }

        public FactionStanceEntry(int otherFactionID, int defaultStance, int entryID)
        {
            this.factionID = entryID;
            this.otherFactionID = otherFactionID;
            this.defaultStance = defaultStance;

            fields = new Dictionary<string, string>() {
            {"factionID", "int"},
            {"otherFaction", "int"},
            {"defaultStance", "int"},
        };
        }

        public int factionID;
        public int otherFactionID;
        public int defaultStance;

        public FactionStanceEntry Clone()
        {
            return (FactionStanceEntry)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            if (fieldKey == "id")
            {
                return id.ToString();
            }
            else if (fieldKey == "factionID")
            {
                return factionID.ToString();
            }
            else if (fieldKey == "otherFaction")
            {
                return otherFactionID.ToString();
            }
            else if (fieldKey == "defaultStance")
            {
                return defaultStance.ToString();
            }
            return "";
        }
    }

    public class FactionData : DataStructure
    {
                                             // General Parameters
   
        public int category = 1;
        public string group = "";               // Id of the aspect this skill belongs to
        public bool modifyable = false;         // Id of the opposite aspect for this skill
        public int defaultStance = 0;       // Stat that gets the most gains from this skill
        public static string[] stanceOptions = new string[] { "Hated", "Disliked", "Neutral", "Friendly", "Honoured", "Exalted" };
        public static int[] stanceValues = new int[] { -2, -1, 0, 1, 2, 3 };
        public List<FactionStanceEntry> factionStances = new List<FactionStanceEntry>();

        public List<int> stancesToBeDeleted = new List<int>();

        public FactionData()
        {
            id = 1;
            Name = "name";
            // Database fields
            fields = new Dictionary<string, string>() {
            {"name", "string"},
            {"category", "int"},
            {"factionGroup", "string"},
            {"public", "bool"},
            {"defaultStance", "int"},
        };
        }

        public FactionData Clone()
        {
            return (FactionData)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            if (fieldKey == "name")
            {
                return Name;
            }
            else if (fieldKey == "category")
            {
                return category.ToString();
            }
            else if (fieldKey == "factionGroup")
            {
                return group;
            }
            else if (fieldKey == "public")
            {
                return modifyable.ToString();
            }
            else if (fieldKey == "defaultStance")
            {
                return defaultStance.ToString();
            }
            return "";
        }

        public static int GetPositionOfStance(int stanceValue)
        {
            for (int i = 0; i < FactionData.stanceValues.Length; i++)
            {
                if (FactionData.stanceValues[i] == stanceValue)
                    return i;
            }
            return 0;
        }

    }
}