using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class MobSpawnData : DataStructure
    {
        // General Parameters
        public int mobTemplate = -1;
        public int mobTemplate2 = -1;
        public int mobTemplate3 = -1;
        public int mobTemplate4 = -1;
        public int mobTemplate5 = -1;
        public int numSpawns = 1;
        public int spawnRadius = 0;
        public int respawnTime = 60000; // Milliseconds
        public int respawnTimeMax = 60000; // Milliseconds
        public int corpseDespawnTime = 50000; // Milliseconds
        public int spawnActiveStartHour = -1;
        public int spawnActiveEndHour = -1;
        public int alternateSpawnMobTemplate = -1;
        public int alternateSpawnMobTemplate2 = -1;
        public int alternateSpawnMobTemplate3 = -1;
        public int alternateSpawnMobTemplate4 = -1;
        public int alternateSpawnMobTemplate5 = -1;
        public bool combat = true;
        public int roamRadius = 0; // Metres
        public int patrolPath = -1;
        public string startsQuests = "";
        public string endsQuests = "";
        public string startsDialogues = "";
        public string otherActions = "";
        public string baseAction = "";
        public bool weaponSheathed = false;
        public int merchantTable = -1;
        public int questOpenLootTable = -1;
        public bool isChest = false;
        public int pickupItem = -1;
        public Vector3 position= Vector3.zero;
        public Quaternion rotation = Quaternion.identity; 
            
        public MobSpawnData()
        {
            Name = "name";
            // Database fields
            fields = new Dictionary<string, string>() {
        {"name", "string"},
        {"mobTemplate", "int"},
        {"numSpawns", "int"},
        {"spawnRadius", "int"},
        {"respawnTime", "int"},
        {"corpseDespawnTime", "int"},
        {"spawnActiveStartHour", "int"},
        {"spawnActiveEndHour", "int"},
        {"alternateSpawnMobTemplate", "int"},
        {"combat", "bool"},
        {"roamRadius", "int"},
        {"patrolPath", "int"},
        {"startsQuests", "string"},
        {"endsQuests", "string"},
        {"startsDialogues", "string"},
        {"otherActions", "string"},
        {"baseAction", "string"},
        {"weaponSheathed", "bool"},
        {"merchantTable", "int"},
        {"questOpenLootTable", "int"},
        {"isChest", "bool"},
        {"pickupItem", "int"},
    };
        }

        public MobSpawnData Clone()
        {
            return (MobSpawnData)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "id":
                    return id.ToString();
                case "name":
                    return Name;
                case "mobTemplate":
                    return mobTemplate.ToString();
                case "numSpawns":
                    return numSpawns.ToString();
                case "spawnRadius":
                    return spawnRadius.ToString();
                case "respawnTime":
                    return respawnTime.ToString();
                case "corpseDespawnTime":
                    return corpseDespawnTime.ToString();
                case "spawnActiveStartHour":
                    return spawnActiveStartHour.ToString();
                case "spawnActiveEndHour":
                    return spawnActiveEndHour.ToString();
                case "alternateSpawnMobTemplate":
                    return alternateSpawnMobTemplate.ToString();
                case "combat":
                    return combat.ToString();
                case "roamRadius":
                    return roamRadius.ToString();
                case "patrolPath":
                    return patrolPath.ToString();
                case "startsQuests":
                    return startsQuests;
                case "endsQuests":
                    return endsQuests;
                case "startsDialogues":
                    return startsDialogues;
                case "otherActions":
                    return otherActions;
                case "baseAction":
                    return baseAction;
                case "weaponSheathed":
                    return weaponSheathed.ToString();
                case "merchantTable":
                    return merchantTable.ToString();
                case "questOpenLootTable":
                    return questOpenLootTable.ToString();
                case "isChest":
                    return isChest.ToString();
                case "pickupItem":
                    return pickupItem.ToString();
            }
            return "";
        }
    }
}