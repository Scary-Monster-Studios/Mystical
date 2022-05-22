using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class GameSettingData : DataStructure
    {
          public string dataType = "";
        public string val = "";

        public GameSettingData()
        {
            id = 0;
            // Database fields
            fields = new Dictionary<string, string>() {
        {"name", "string"},
        {"dataType", "string"},
        {"value", "string"}
    };
        }

        public GameSettingData Clone()
        {
            return (GameSettingData)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "id":
                    return id.ToString();
                case "name":
                    return Name;
                case "dataType":
                    return dataType;
                case "value":
                    return val;
            }
            return "";
        }
    }
}