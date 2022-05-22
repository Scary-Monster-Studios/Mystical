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
    public class ServerRestoreEffects : ServerEffectType
    {

        public new string effectType = "Restore";
        public new string[] effectTypeOptions = new string[] { "HealInstantEffect", "HealOverTimeEffect", "HealthTransferEffect" };

        public string[] vitalityStatOptions = new string[] { "~ none ~" };

        // Use this for initialization
        public ServerRestoreEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
            LoadVitalityStatOptions();
        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            pos.y += ImagePack.fieldHeight;
            editingDisplay.intValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Restore Amount")+":", editingDisplay.intValue1);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.stringValue1 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Restore Property")+":", editingDisplay.stringValue1, vitalityStatOptions);
            pos.y += ImagePack.fieldHeight;
            if (editingDisplay.effectType.Contains("Transfer"))
            {
                  editingDisplay.floatValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Transfer Rate")+":", editingDisplay.floatValue1);
                pos.y += ImagePack.fieldHeight;
            }
            if (editingDisplay.effectType.Contains("Instant"))
            {
                pos.width *= 2;
                editingDisplay.pulseCoordEffect = ImagePack.DrawGameObject(pos, Lang.GetTranslate("Pulse CoordEffect")+":", editingDisplay.pulseCoordEffect, 0.6f);
                pos.width /= 2;
                pos.y += ImagePack.fieldHeight;
                showTimeFields = false;
            }
            else
            {
                showTimeFields = true;
            }
            if (editingDisplay.effectType.Contains("Transfer"))
                showTimeFields = false;

            showSkillModFields = true;
            return pos;
        }

        public void LoadVitalityStatOptions()
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
                vitalityStatOptions = new string[rows.Count];
                //vitalityStatOptions [optionsId] = "~ none ~"; 
                foreach (Dictionary<string, string> data in rows)
                {
                    vitalityStatOptions[optionsId] = data["name"];
                    optionsId++;
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