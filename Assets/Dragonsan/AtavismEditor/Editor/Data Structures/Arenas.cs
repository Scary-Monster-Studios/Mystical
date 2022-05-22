using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class ArenaTeamEntry : DataStructure
    {
        public ArenaTeamEntry() : this("", Vector3.zero, "")
        {
        }

        public ArenaTeamEntry(string name, Vector3 loc) : this(name, loc, "")
        {
        }

        public ArenaTeamEntry(string name, Vector3 loc, string gameObject)
        {
            this.Name = name;
            this.loc = loc;
            //	this.orient = orient;
            this.gameObject = gameObject;

            fields = new Dictionary<string, string>() {
            {"id", "int"},
            {"arenaID", "int"},
            {"name", "string"},
            {"size", "int"},
            {"race", "int"},
            {"goal", "int"},
            {"spawnX", "float"},
            {"spawnY", "float"},
            {"spawnZ", "float"},
        };
        }

        public int arenaID;
        public int size = 1;
        public int race = 0;
        public int goal = 1;
        public Vector3 loc;
        //public Quaternion orient;
        public string gameObject;

        public ArenaTeamEntry Clone()
        {
            return (ArenaTeamEntry)this.MemberwiseClone();
        }

        public string MarkerObject
        {
            get
            {
                return gameObject;
            }
            set
            {
                gameObject = value;
                if (gameObject != null)
                {
                    // Try to get as Scene Object
                    GameObject tempObject = GameObject.Find(gameObject);
                    // If the object is at the Scene
                    if (tempObject != null)
                    {
                        // Get object transform
                        loc = tempObject.transform.position;
                        //	orient = tempObject.transform.rotation;
                    }
                }
            }
        }

        public override string GetValue(string fieldKey)
        {
            if (fieldKey == "id")
            {
                return id.ToString();
            }
            else if (fieldKey == "arenaID")
            {
                return arenaID.ToString();
            }
            else if (fieldKey == "name")
            {
                return Name;
            }
            else if (fieldKey == "size")
            {
                return size.ToString();
            }
            else if (fieldKey == "race")
            {
                return race.ToString();
            }
            else if (fieldKey == "goal")
            {
                return goal.ToString();
            }
            else if (fieldKey == "spawnX")
            {
                return loc.x.ToString();
            }
            else if (fieldKey == "spawnY")
            {
                return loc.y.ToString();
            }
            else if (fieldKey == "spawnZ")
            {
                return loc.z.ToString();
                /*} else if (fieldKey == "orientX") {
                    return orient.x.ToString();
                } else if (fieldKey == "orientY") {
                    return orient.y.ToString();
                } else if (fieldKey == "orientZ") {
                    return orient.z.ToString();
                } else if (fieldKey == "orientW") {
                    return orient.w.ToString();*/
            }
            return "";
        }
    }

    // Structure of a Atavism Instance
    public class Arena : DataStructure
    {
        public int arenaType = 1;                           // Type of island
        public static string[] arenaTypes = new string[] { "", "Deathmatch" };
        public int arenaCategory = 1;
        public int arenaInstanceID = 0;     // Id Instance for arena
        public int length = 300;            // Time length for arena
        public int defaultWinner = 1;       //Default winner team
        public int team1 = -1;
        public int team2 = -1;
        public int team3 = -1;
        public int team4 = -1;
        public int levelReq = 1;            //Min Level to enter
        public int levelMax = 99;           //Max level to enter
        public int victoryCurrency = 1;
        public int victoryPayment = 1;
        public int defeatCurrency = 1;
        public int defeatPayment = 1;
        public int victoryExp = 1;
        public int defeatExp = 1;
        public int start_hour = 0;
        public int start_minute = 0;
        public int end_hour = 23;
        public int end_minute = 59;
        public string description = "";

        /*   public float globalWaterHeight = 0f;
           public int administrator;						// The account that has administration privileges of the island
           public int populationLimit = -1;	
           */
        public List<ArenaTeamEntry> arenaTeams = new List<ArenaTeamEntry>();

        public List<int> itemsToBeDeleted = new List<int>();

        public Arena()
        {
            Name = "name";
            // Database fields
            fields = new Dictionary<string, string>() {
        {"id", "int"},
        {"arenaType", "int"},
        {"name", "string"},
        {"arenaCategory", "int"},
        {"arenaInstanceID", "int"},
        {"length", "int"},
        {"defaultWinner", "int"},
        {"team1", "int"},
        {"team2", "int"},
        {"team3", "int"},
        {"team4", "int"},
        {"levelReq", "int"},
        {"levelMax", "int"},
        {"victoryCurrency", "int"},
        {"victoryPayment", "int"},
        {"defeatCurrency", "int"},
        {"defeatPayment", "int"},
        {"victoryExp", "int"},
        {"defeatExp", "int"},
        {"isactive", "bool"},
        {"start_hour", "int"},
        {"start_minute", "int"},
        {"end_hour", "int"},
        {"end_minute", "int"},
        {"description", "string"},
    };
        }

        public Arena Clone()
        {
            return (Arena)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "id":
                    return id.ToString();
                case "name":
                    return Name;
                case "arenaType":
                    return arenaType.ToString();
                case "arenaCategory":
                    return arenaCategory.ToString();
                case "arenaInstanceID":
                    return arenaInstanceID.ToString();
                case "length":
                    return length.ToString();
                case "defaultWinner":
                    return defaultWinner.ToString();
                case "team1":
                    return team1.ToString();
                case "team2":
                    return team2.ToString();
                case "team3":
                    return team3.ToString();
                case "team4":
                    return team4.ToString();
                case "levelReq":
                    return levelReq.ToString();
                case "levelMax":
                    return levelMax.ToString();
                case "victoryCurrency":
                    return victoryCurrency.ToString();
                case "victoryPayment":
                    return victoryPayment.ToString();
                case "defeatCurrency":
                    return defeatCurrency.ToString();
                case "defeatPayment":
                    return defeatPayment.ToString();
                case "victoryExp":
                    return victoryExp.ToString();
                case "defeatExp":
                    return defeatExp.ToString();
                case "start_hour":
                    return start_hour.ToString();
                case "start_minute":
                    return start_minute.ToString();
                case "end_hour":
                    return end_hour.ToString();
                case "end_minute":
                    return end_minute.ToString();
                case "description":
                    return description;


            }
            return "";
        }
    }
}