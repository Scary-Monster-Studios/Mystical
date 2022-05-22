using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class StatThresholdData : DataStructure
    {
        public StatThresholdData() : this( "")
        {
        }

        public StatThresholdData(string function)
        {
        
            this.function = function;
        }
    
        public string function = "";

        public List<StatThresholdEntry> thresholds = new List<StatThresholdEntry>();
        public List<StatThresholdEntry> thresholdsToDelete = new List<StatThresholdEntry>();


        public StatThresholdData Clone()
        {
            return (StatThresholdData)this.MemberwiseClone();
        }

        string getBonusesData(int option, string field)
        {
            if (thresholds.Count > option)
            {
                if (field == "stat_function")
                {
                    return thresholds[option].function.ToString();
                }
                else if (field == "threshold")
                {
                    return thresholds[option].threshold.ToString();
                }
                else if (field == "num_per_point")
                {
                    return thresholds[option].points.ToString();
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

    public class StatThresholdEntry
    {
        public StatThresholdEntry(string function, int threshold_org, int threshold, int points)
        {
            this.function = function;
            this.threshold_org = threshold_org;
            this.threshold = threshold;
            this.points = points;
        }
        public string function = "";
        public int threshold_org = 0;
        public int threshold = 0;
        public int points = 0;
    }

}