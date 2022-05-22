using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Structure of a Loot Table
/*
/* Table structure for tables
/*
CREATE TABLE `mob_loot` (
`id` int(11) NOT NULL AUTO_INCREMENT,
  `category` int(11) NOT NULL DEFAULT '1',
  `mobTemplate` int(11) NOT NULL,
  `lootTable` int(11) DEFAULT NULL,
  `dropChance` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mobTemplate` (`mobTemplate`)
)
*/
namespace Atavism
{
    public class CoordEffectData : DataStructure
    {
                                            // General Parameters
         public string prefab = "";

        public CoordEffectData()
        {
            // Database fields
            fields = new Dictionary<string, string>() {
            {"name", "string"},
            {"prefab", "string"},
    };
        }

        public CoordEffectData Clone()
        {
            return (CoordEffectData)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "id":
                    return id.ToString();
                case "name":
                    return Name;
                case "prefab":
                    return prefab;
            }
            return "";
        }

    }
}