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
    public class ServerReviveEffects : ServerEffectType
    {

        public new string effectType = "Revive";
        public new string[] effectTypeOptions = new string[] { "ReviveEffect" };

        public string[] statOptions = new string[] { "~ none ~" };

        // Use this for initialization
        public ServerReviveEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
            LoadStatOptions();
        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            editingDisplay.stringValue1 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Vitality stat") +":", editingDisplay.stringValue1, statOptions);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.intValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Health % Given")+":", editingDisplay.intValue1);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.stringValue2 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Vitality stat")+" 2:", editingDisplay.stringValue2, statOptions);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.intValue2 = ImagePack.DrawField(pos, Lang.GetTranslate("% Given")+":", editingDisplay.intValue2);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.stringValue3 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Vitality stat")+" 3:", editingDisplay.stringValue3, statOptions);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.intValue3 = ImagePack.DrawField(pos, Lang.GetTranslate("% Given")+":", editingDisplay.intValue3);
            pos.y += ImagePack.fieldHeight;
            showTimeFields = false;
            showSkillModFields = true;
            return pos;
        }

        public void LoadStatOptions()
        {
            // Read all entries from the table
            string query = "SELECT name FROM stat where type = 2 AND isactive = 1";

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