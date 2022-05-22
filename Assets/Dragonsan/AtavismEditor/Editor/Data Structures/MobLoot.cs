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

    public class MobLoot : DataStructure
    {
                                            // General Parameters
        public int category = 0;
        public int mobTemplate;
        public int tableId = -1;
        public float chance = 0;
        public int count = 1;

        public MobLoot()
        {
            // Database fields
            fields = new Dictionary<string, string>() {
        {"category", "int"},
        {"mobTemplate", "int"},
        {"lootTable", "int"},
        {"dropChance", "float"},
        {"count", "int"},
    };
        }

        public MobLoot Clone()
        {
            return (MobLoot)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "category":
                    return category.ToString();
                case "mobTemplate":
                    return mobTemplate.ToString();
                case "lootTable":
                    return tableId.ToString();
                case "dropChance":
                    return chance.ToString();
                case "count":
                    return count.ToString();
            }
            return "";
        }

    }
}