using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class PatrolPointEntry : DataStructure
    {
        public PatrolPointEntry() : this(-1, 0, 0, 0, 0, -1)
        {

        }

        public PatrolPointEntry(int entryID, float locX, float locY, float locZ, float lingerTime, int nextPoint)
        {
            this.id = entryID;
            this.locX = locX;
            this.locY = locY;
            this.locZ = locZ;
            this.lingerTime = lingerTime;
            this.nextPoint = nextPoint;

            fields = new Dictionary<string, string>() {
            {"id", "int"},
            {"locX", "float"},
            {"locY", "float"},
            {"locZ", "float"},
            {"lingerTime", "float"},
            {"nextPoint", "int"},
        };
        }

        public float locX;
        public float locY;
        public float locZ;
        public float lingerTime = 0;
        public int nextPoint = -1;

        public PatrolPointEntry Clone()
        {
            return (PatrolPointEntry)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            if (fieldKey == "id")
            {
                return id.ToString();
            }
            else if (fieldKey == "locX")
            {
                return locX.ToString();
            }
            else if (fieldKey == "locY")
            {
                return locY.ToString();
            }
            else if (fieldKey == "locZ")
            {
                return locZ.ToString();
            }
            else if (fieldKey == "lingerTime")
            {
                return lingerTime.ToString();
            }
            else if (fieldKey == "nextPoint")
            {
                return nextPoint.ToString();
            }
            return "";
        }
    }

    public class PatrolPathData : DataStructure
    {
         public bool travelReverse = false;
        public float locX;
        public float locY;
        public float locZ;
        public float lingerTime = 0;
        public int nextPoint = -1;

        public List<PatrolPointEntry> tableItems = new List<PatrolPointEntry>();

        public List<int> itemsToBeDeleted = new List<int>();

        public PatrolPathData()
        {
            id = 1;
            Name = "name";
            // Database fields
            fields = new Dictionary<string, string>() {
            {"name", "string"},
    {"startingPoint", "int"},
    {"travelReverse", "int"},
    {"locX", "float"},
    {"locY", "float"},
    {"locZ", "float"},
    {"lingerTime", "int"},
    {"nextPoint", "int"},
                                    };

        }

        public PatrolPathData Clone()
        {
            return (PatrolPathData)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "id":
                    return id.ToString();
                case "name":
                    return Name;
            }
            return "";
        }
    }
}