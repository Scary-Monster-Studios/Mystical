using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Structure of a Atavism Character

/*
/* Table structure for tables
/*

CREATE TABLE `character_create_template` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `race` varchar(45) NOT NULL,
  `aspect` varchar(45) NOT NULL,
  `instanceName` varchar(45) NOT NULL,
  `pos_x` float NOT NULL,
  `pos_y` float NOT NULL,
  `pos_z` float NOT NULL,
  `orientation` float NOT NULL,
  `health` int(11) NOT NULL,
  `mana` int(11) NOT NULL,

*/
namespace Atavism
{
    public class CharData : DataStructure
    {
                                             // General Parameters
        public int race = -1;
        public int aspect = -1;
        public int faction = 1;
        public int instance;
        public float pos_x = 0;
        public float pos_y = 0;
        public float pos_z = 0;
        string spawn = null;                            // Prefab Object to spawn 
        public int respawnInstance;
        public float respawnPosX = 0;
        public float respawnPosY = 0;
        public float respawnPosZ = 0;
        string respawn = null;
        public float orientation = 0;
        public int autoAttack = 0;
        public int startingLevel = 1;
        public int sprint = -1;

        public List<CharStatsData> charStats = new List<CharStatsData>();
        public List<CharSkillsData> charSkills = new List<CharSkillsData>();
        public List<CharItemsData> charItems = new List<CharItemsData>();

        public List<int> skillsToBeDeleted = new List<int>();
        public List<int> itemsToBeDeleted = new List<int>();

        public Vector3 Position
        {
            get
            {
                return new Vector3(pos_x, pos_y, pos_z);
            }
            set
            {
                Vector3 spawnLoc = value;
                pos_x = spawnLoc.x;
                pos_y = spawnLoc.y;
                pos_z = spawnLoc.z;
            }
        }

        public string Spawn
        {
            get
            {
                return spawn;
            }
            set
            {
                spawn = value;
                if (spawn != null)
                {
                    // Try to get as Scene Object
                    GameObject tempObject = GameObject.Find(spawn);
                    // If the object is at the Scene
                    if (tempObject != null)
                    {
                        // Get object transform
                        Vector3 spawnLoc = tempObject.transform.position;
                        pos_x = spawnLoc.x;
                        pos_y = spawnLoc.y;
                        pos_z = spawnLoc.z;
                    }
                }
            }
        }

        public Vector3 RespawnPosition
        {
            get
            {
                return new Vector3(respawnPosX, respawnPosY, respawnPosZ);
            }
            set
            {
                Vector3 spawnLoc = value;
                respawnPosX = spawnLoc.x;
                respawnPosY = spawnLoc.y;
                respawnPosZ = spawnLoc.z;
            }
        }

        public string ReSpawn
        {
            get
            {
                return respawn;
            }
            set
            {
                respawn = value;
                if (respawn != null)
                {
                    // Try to get as Scene Object
                    GameObject tempObject = GameObject.Find(respawn);
                    // If the object is at the Scene
                    if (tempObject != null)
                    {
                        // Get object transform
                        Vector3 spawnLoc = tempObject.transform.position;
                        respawnPosX = spawnLoc.x;
                        respawnPosY = spawnLoc.y;
                        respawnPosZ = spawnLoc.z;
                    }
                }
            }
        }

        public CharData()
        {
            // Database fields
            fields = new Dictionary<string, string>() {
        {"race", "int"},
        {"aspect", "int"},
        {"faction", "int"},
        {"instance", "int"},
        {"pos_x", "float"},
        {"pos_y", "float"},
        {"pos_z", "float"},
        {"respawnInstance", "int"},
        {"respawnPosX", "float"},
        {"respawnPosY", "float"},
        {"respawnPosZ", "float"},
        {"orientation", "float"},
        {"autoAttack", "int"},
        {"startingLevel", "int"},
        {"sprint", "int" },
    };
        }

        public CharData Clone()
        {
            return (CharData)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "race":
                    return race.ToString();
                case "aspect":
                    return aspect.ToString();
                case "faction":
                    return faction.ToString();
                case "instance":
                    return instance.ToString();
                case "pos_x":
                    return pos_x.ToString();
                case "pos_y":
                    return pos_y.ToString();
                case "pos_z":
                    return pos_z.ToString();
                case "respawnInstance":
                    return respawnInstance.ToString();
                case "respawnPosX":
                    return respawnPosX.ToString();
                case "respawnPosY":
                    return respawnPosY.ToString();
                case "respawnPosZ":
                    return respawnPosZ.ToString();
                case "orientation":
                    return orientation.ToString();
                case "autoAttack":
                    return autoAttack.ToString();
                case "startingLevel":
                    return startingLevel.ToString();
                case "sprint":
                    return sprint.ToString();
            }
            return "";
        }

    }
}