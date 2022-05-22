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
    public class ServerMountEffects : ServerEffectType
    {

        public new string effectType = "Mount";
        public new string[] effectTypeOptions = new string[] { "MountEffect" };
        public string[] mountTypeOptions = new string[] { "Ground", "Swimming", "Flying" };

        public string[] statOptions = new string[] { "~ none ~" };

        // Use this for initialization
        public ServerMountEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
            LoadStatOptions();
        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            editingDisplay.stringValue1 = ImagePack.DrawGameObject(pos, Lang.GetTranslate("Model")+": ", editingDisplay.stringValue1, 0.75f);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.intValue1 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Mount Type"), editingDisplay.intValue1, mountTypeOptions);
            //editingDisplay.intValue1 = ImagePack.DrawField (pos, "Mount Type:", editingDisplay.intValue1);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.intValue2 = ImagePack.DrawField(pos, Lang.GetTranslate("Speed Increase")+" %:", editingDisplay.intValue2);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.stringValue2 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Stat change")+":", editingDisplay.stringValue2, statOptions);
            pos.x += pos.width;
            editingDisplay.floatValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Modification")+":", editingDisplay.floatValue1);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            showTimeFields = false;
            editingDisplay.createPrefab = true;
            showSkillModFields = true;
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