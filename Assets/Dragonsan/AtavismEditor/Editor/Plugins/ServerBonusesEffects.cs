using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
namespace Atavism
{
    // Handles the Effects Configuration
    public class ServerBonusesEffects : ServerEffectType
    {

        public new string effectType = "Bonus";
        public new string[] effectTypeOptions = new string[] { "BonusEffect" };

        public int[] bonusIds = new int[] { -1 };
        public string[] bonusOptions = new string[] { "~ none ~" };
        public string[] bonusOptionsParam = new string[] { "" };
        public string[] bonusOptionsCode = new string[] { "" };
        string searchString1 = "";
        string searchString2 = "";
        string searchString3 = "";
        string searchString4 = "";
        string searchString5 = "";

        // Use this for initialization
        public ServerBonusesEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
            LoadBonusSettings();
        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            editingDisplay.boolValue2 = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Effect Stays after Logout") + ":", editingDisplay.boolValue2);
            pos.y += ImagePack.fieldHeight*1.5f;
            int selectedBonus = 0;
            try
            {
                selectedBonus = GetOptionPosition(editingDisplay.stringValue1, bonusOptionsCode);
            }
            catch (Exception)
            {
            }
            selectedBonus = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Bonus") + " 1: ", ref searchString1, selectedBonus, bonusOptions);
            editingDisplay.stringValue1 = bonusOptionsCode[selectedBonus].ToString();
            pos.y += ImagePack.fieldHeight;
            editingDisplay.intValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Value") + ":", editingDisplay.intValue1);
            pos.x += pos.width;
            editingDisplay.floatValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Value Percentage") + ":", editingDisplay.floatValue1);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedBonus = 0;
            try
            {
                selectedBonus = GetOptionPosition(editingDisplay.stringValue2, bonusOptionsCode);
            }
            catch (Exception)
            {
            }
            selectedBonus = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Bonus") + " 2: ", ref searchString2, selectedBonus, bonusOptions);
            editingDisplay.stringValue2 = bonusOptionsCode[selectedBonus].ToString();
            pos.y += ImagePack.fieldHeight;
            editingDisplay.intValue2 = ImagePack.DrawField(pos, Lang.GetTranslate("Value") + ":", editingDisplay.intValue2);
            pos.x += pos.width;
            editingDisplay.floatValue2 = ImagePack.DrawField(pos, Lang.GetTranslate("Value Percentage") + ":", editingDisplay.floatValue2);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedBonus = 0;
            try
            {
                selectedBonus = GetOptionPosition(editingDisplay.stringValue3, bonusOptionsCode);
            }
            catch (Exception)
            {
            }
            selectedBonus = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Bonus") + " 3: ", ref searchString3, selectedBonus, bonusOptions);
            editingDisplay.stringValue3 = bonusOptionsCode[selectedBonus].ToString();
            pos.y += ImagePack.fieldHeight;
            editingDisplay.intValue3 = ImagePack.DrawField(pos, Lang.GetTranslate("Value") + ":", editingDisplay.intValue3);
            pos.x += pos.width;
            editingDisplay.floatValue3 = ImagePack.DrawField(pos, Lang.GetTranslate("Value Percentage") + ":", editingDisplay.floatValue3);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedBonus = 0;
            try
            {
                selectedBonus = GetOptionPosition(editingDisplay.stringValue4, bonusOptionsCode);
            }
            catch (Exception)
            {
            }
            selectedBonus = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Bonus") + " 4: ", ref searchString4, selectedBonus, bonusOptions);
            editingDisplay.stringValue4 = bonusOptionsCode[selectedBonus].ToString();
            pos.y += ImagePack.fieldHeight;
            editingDisplay.intValue4 = ImagePack.DrawField(pos, Lang.GetTranslate("Value") + ":", editingDisplay.intValue4);
            pos.x += pos.width;
            editingDisplay.floatValue4 = ImagePack.DrawField(pos, Lang.GetTranslate("Value Percentage") + ":", editingDisplay.floatValue4);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedBonus = 0;
            try
            {
                selectedBonus = GetOptionPosition(editingDisplay.stringValue5, bonusOptionsCode);
            }
            catch (Exception)
            {
            }
            selectedBonus = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Bonus") + " 5: ", ref searchString5, selectedBonus, bonusOptions);
            editingDisplay.stringValue5 = bonusOptionsCode[selectedBonus].ToString();
            pos.y += ImagePack.fieldHeight;
            editingDisplay.intValue5 = ImagePack.DrawField(pos, Lang.GetTranslate("Value") + ":", editingDisplay.intValue5);
            pos.x += pos.width;
            editingDisplay.floatValue5 = ImagePack.DrawField(pos, Lang.GetTranslate("Value Percentage") + ":", editingDisplay.floatValue5);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            showTimeFields = true;
            showSkillModFields = true;
            return pos;
        }

        public void LoadBonusSettings()
        {
            // Read all entries from the table
            string query = "SELECT * FROM bonuses_settings where isactive = 1 order by id ";
            //  Debug.LogError(query);
            // If there is a row, clear it.
            
            //     optionslist.Clear();
            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data

            if ((rows != null) && (rows.Count > 0))
            {
                int optionsId = 0;
                bonusOptions = new string[rows.Count + 1];
                bonusOptionsParam = new string[rows.Count + 1];
                bonusOptionsCode = new string[rows.Count + 1];
                bonusOptions[optionsId] = "~ none ~";
                bonusOptionsParam[optionsId] = "";
                bonusOptionsCode[optionsId] = "";
                bonusIds = new int[rows.Count + 1];
                bonusIds[optionsId] = -1;

                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    bonusOptions[optionsId] = data["name"];
                    bonusIds[optionsId] = int.Parse(data["id"]);
                    bonusOptionsParam[optionsId] = data["param"];
                    bonusOptionsCode[optionsId] = data["code"];
                    //    Debug.LogError(int.Parse(data["id"])+", "+data["name"]+", "+float.Parse(data["cost"])+", "+float.Parse(data["chance"]));
                    //     optionslist.Add(new BonusOptionData(int.Parse(data["id"]), data["name"], data["code"], data["param"]));
                }
            }
        }

        public override string EffectType
        {
            get
            {
                return effectType;
            }
        }

        public override string[] EffectTypeOptions
        {
            get
            {
                return effectTypeOptions;
            }
        }
        public int GetOptionPosition(string id, string[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                if (ids[i] == id)
                    return i;
            }
            return 0;
        }

    }
}