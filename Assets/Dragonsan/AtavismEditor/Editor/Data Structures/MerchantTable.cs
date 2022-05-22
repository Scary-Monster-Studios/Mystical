using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
namespace Atavism
{
    public class MerchantTableItemEntry : DataStructure
    {
        public MerchantTableItemEntry() : this(-1, -1, -1, 0)
        {
        }

        public MerchantTableItemEntry(int count, int itemID) : this(count, itemID, -1, 0)
        {
        }

        public MerchantTableItemEntry(int count, int itemID, int entryID, int refreshTime)
        {
            this.id = entryID;
            this.count = count;
            this.itemID = itemID;
            this.refreshTime = refreshTime;

            fields = new Dictionary<string, string>() {
            {"tableID", "int"},
            {"count", "int"},
            {"itemID", "int"},
            {"refreshTime", "int"},
        };
        }

        public int tableID;
        public int count = -1;
        public int itemID=-1;
        public int refreshTime = 0;
        public string itemSearch = "";
        public MerchantTableItemEntry Clone()
        {
            return (MerchantTableItemEntry)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            if (fieldKey == "id")
            {
                return id.ToString();
            }
            else if (fieldKey == "tableID")
            {
                return tableID.ToString();
            }
            else if (fieldKey == "count")
            {
                return count.ToString();
            }
            else if (fieldKey == "itemID")
            {
                return itemID.ToString();
            }
            else if (fieldKey == "refreshTime")
            {
                return refreshTime.ToString();
            }
            return "";
        }
    }

    public class MerchantTable : DataStructure
    {
     
        public List<MerchantTableItemEntry> tableItems = new List<MerchantTableItemEntry>();

        public List<int> itemsToBeDeleted = new List<int>();

        public MerchantTable()
        {
            id = 1;
            Name = "name";
            // Database fields
            fields = new Dictionary<string, string>() {
            {"name", "string"},
        };

        }

        public MerchantTable Clone()
        {
            return (MerchantTable)this.MemberwiseClone();
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