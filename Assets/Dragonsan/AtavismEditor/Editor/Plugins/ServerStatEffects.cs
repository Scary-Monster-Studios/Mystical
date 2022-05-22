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
    public class ServerStatEffects : ServerEffectType
    {

        public new string effectType = "Stat";
        public new string[] effectTypeOptions = new string[] { "StatEffect" };

        public string[] statOptions = new string[] { "~ none ~" };

        // Use this for initialization
        public ServerStatEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
            LoadStatOptions();
        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            editingDisplay.boolValue2 = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Effect Stays after Logout")+":", editingDisplay.boolValue2);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.boolValue1 = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Modify By Percent")+":", editingDisplay.boolValue1);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.stringValue1 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Stat")+" 1:", editingDisplay.stringValue1, statOptions);
            pos.x += pos.width;
            editingDisplay.floatValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Modification")+":", editingDisplay.floatValue1);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.stringValue2 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Stat")+" 2:", editingDisplay.stringValue2, statOptions);
            pos.x += pos.width;
            editingDisplay.floatValue2 = ImagePack.DrawField(pos, Lang.GetTranslate("Modification")+":", editingDisplay.floatValue2);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.stringValue3 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Stat")+" 3:", editingDisplay.stringValue3, statOptions);
            pos.x += pos.width;
            editingDisplay.floatValue3 = ImagePack.DrawField(pos, Lang.GetTranslate("Modification")+":", editingDisplay.floatValue3);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.stringValue4 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Stat")+" 4:", editingDisplay.stringValue4, statOptions);
            pos.x += pos.width;
            editingDisplay.floatValue4 = ImagePack.DrawField(pos, Lang.GetTranslate("Modification")+":", editingDisplay.floatValue4);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.stringValue5 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Stat")+" 5:", editingDisplay.stringValue5, statOptions);
            pos.x += pos.width;
            editingDisplay.floatValue5 = ImagePack.DrawField(pos, Lang.GetTranslate("Modification")+":", editingDisplay.floatValue5);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            showSkillModFields = true;
            showTimeFields = true;
            return pos;
        }

        public void LoadStatOptions()
        {
            // Read all entries from the table
            string query = "SELECT name FROM stat where isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                statOptions = new string[rows.Count + 1];
                statOptions[optionsId] = "~ none ~";
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    statOptions[optionsId] = data["name"];
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
    }
}