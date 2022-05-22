using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class AchievementData : DataStructure
    {
        public AchievementData() : this(-1, "")
        {
        }

        public AchievementData(int id, string Name)
        {
            this.id = id;
            this.Name = Name;
        }
        public int value = 0;
        public int type = 1;
        public string[] types = new string[] { "", "Kill", "Experience", "Harvesting","Crafting","Looting","Use Ability","Final Blow","Gear Score"};
        public string description = "";
       // public string objects = "";
        public List<int> objects = new List<int>();
        public List<string> objectsSearch = new List<string>();

        public List<BonusEntry> bonuses = new List<BonusEntry>();
        public List<StatEntry> stats = new List<StatEntry>();

        public AchievementData Clone()
        {
            return (AchievementData)this.MemberwiseClone();
        }

        string getBonusesData(int option, string field)
        {
            if (bonuses.Count > option)
            {
                if (field == "percentage")
                {
                    return bonuses[option].BonusValuePercentage.ToString();
                }
                else if (field == "type")
                {
                    return bonuses[option].BonusType.ToString();
                }
                else if (field == "value")
                {
                    return bonuses[option].BonusValue.ToString();
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