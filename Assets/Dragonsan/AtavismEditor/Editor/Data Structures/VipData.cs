using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class VipData : DataStructure
    {
        public VipData() : this(-1, "")
        {
        }

        public VipData(int id, string Name)
        {
            this.id = id;
            this.Name = Name;
        }
        public int max_points = 0;
        public int level = 1;
        public string description = "";
        public List<BonusEntry> bonuses = new List<BonusEntry>();

        public VipData Clone()
        {
            return (VipData)this.MemberwiseClone();
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

    public class BonusEntry
    {
        public BonusEntry(int id ,int BonusType, int BonusValue, float BonusValuePercentage)
        {
            this.id = id;
            this.BonusValue = BonusValue;
            this.BonusType = BonusType;
            this.BonusValuePercentage = BonusValuePercentage;
        }
        public int id = -1;
        public float BonusValuePercentage = 0;
        public int BonusType;
        public int BonusValue = 0;
    }

  
    public class BonusOptionData
    {
        public BonusOptionData()
        {
        }

        public BonusOptionData(int id, string Name, string code, string param)
        {
            this.id = id;
            this.Name = Name;
            this.code = code;
            this.param = param;
        }

        public int id = -1;
        public string Name = "";
        public string code = "";
        public string param = "";
    }
}