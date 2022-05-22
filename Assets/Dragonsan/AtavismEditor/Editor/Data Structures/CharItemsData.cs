using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Structure of a Atavism Stats
/*
/* Table structure for tables
/*
CREATE TABLE `character_create_items` (
  `id` int(11) NOT NULL,
  `character_create_id` int(11) NOT NULL,
  `item_id` int(11) NOT NULL,
  `count` int(11) NOT NULL DEFAULT '1',
  `equipped` tinyint(4) DEFAULT NULL,
  */
namespace Atavism
{
    public class CharItemsData : DataStructure
    {
                                            // General Parameters
        public int charId = -1;                 // Database Index
        public int itemId = -1;
        public int count = 1;
        public bool equipped = false;
        public string itemSearch = "";
        public CharItemsData()
        {
            id = -1;
            // Database fields
            fields = new Dictionary<string, string>() {
        {"character_create_id", "int"},
        {"item_id", "int"},
        {"count", "int"},
        {"equipped", "bool"},
    };
        }

        public CharItemsData Clone()
        {
            return (CharItemsData)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "character_create_id":
                    return charId.ToString();
                case "item_id":
                    return itemId.ToString();
                case "count":
                    return count.ToString();
                case "equipped":
                    return equipped.ToString();
            }
            return "";
        }

    }
}