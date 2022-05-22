using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class RankingData : DataStructure
    {
        public RankingData() : this(-1, "")
        {
        }

        public RankingData(int id, string Name)
        {
            this.id = id;
            this.Name = Name;
        }
        public int count = 0;
        public int type = 1;
        public string[] types = new string[] { "", "Kill","Expirience","Harvesting","Crafting","Looting","Use Ability","Final blow","Gear Score"};
        public string description = "";
       // public string objects = "";
        public List<int> objects = new List<int>();
        public List<string> objectsSearch = new List<string>();

       // public List<BonusEntry> bonuses = new List<BonusEntry>();

        public RankingData Clone()
        {
            return (RankingData)this.MemberwiseClone();
        }

     /*  string getBonusesData(int option, string field)
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
        }*/

    }

   
}