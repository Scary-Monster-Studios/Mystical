using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Structure of a Atavism Stats
/*
/* Table structure for tables
/*

CREATE TABLE `character_create_skills` (
  `id` int(11) NOT NULL,
  `character_create_id` int(11) NOT NULL,
  `skill` int(11) NOT NULL,
*/
namespace Atavism
{
    public class CharSkillsData : DataStructure
    {
                                            // General Parameters
        public int charId = -1;                 // Database Index
        public int skill = -1;

        public string skillSearch = "";

        public CharSkillsData()
        {
            id = -1;
            // Database fields
            fields = new Dictionary<string, string>() {
        {"character_create_id", "int"},
        {"skill", "int"},
    };
        }

        public CharSkillsData Clone()
        {
            return (CharSkillsData)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "character_create_id":
                    return charId.ToString();
                case "skill":
                    return skill.ToString();
            }
            return "";
        }

    }
}