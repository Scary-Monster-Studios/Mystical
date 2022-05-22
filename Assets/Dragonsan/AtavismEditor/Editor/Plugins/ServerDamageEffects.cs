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
    public class ServerDamageEffects : ServerEffectType
    {

        public new string effectType = "Damage";
        public new string[] effectTypeOptions = new string[] { "MeleeStrikeEffect", "MagicalStrikeEffect", "PhysicalDotEffect", "MagicalDotEffect", "FlatDamageEffect" };


        public string[] damageOptions = new string[] { "~ none ~" };
        public string[] vitalityStatOptions = new string[] { "~ none ~" };

        // Use this for initialization
        public ServerDamageEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
            LoadDamageOptions();
            LoadVitalityStatOptions();
            if (newItem)
            {
                editingDisplay.floatValue1 = 1f;
            }
        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            showSkillModFields = true;
            editingDisplay.stringValue1 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Damage Property")+":", editingDisplay.stringValue1, vitalityStatOptions);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.stringValue2 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Damage Type")+":", editingDisplay.stringValue2, damageOptions);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.intValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Damage Amount")+":", editingDisplay.intValue1);
            pos.x += pos.width;
            if (!editingDisplay.effectType.Equals("FlatDamageEffect"))
            {

                editingDisplay.floatValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Damage Modifier") + ":", editingDisplay.floatValue1);
               
            }
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.floatValue2 = ImagePack.DrawField(pos, Lang.GetTranslate("Transfer Rate")+":", editingDisplay.floatValue2);
            pos.y += ImagePack.fieldHeight;
            if (editingDisplay.effectType.Equals("MeleeStrikeEffect")|| editingDisplay.effectType.Equals("MagicalStrikeEffect"))
            {
                int selectedEffect = ServerEffects.Instance.GetPositionOfEffect(editingDisplay.intValue2);
                selectedEffect = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Bonus Dmg Effect Req") + ":", ref editingDisplay.stringValueSearch2, selectedEffect, ServerEffects.effectOptions);
                editingDisplay.intValue2 = ServerEffects.effectIds[selectedEffect];
                pos.y += ImagePack.fieldHeight;
                //pos.x += pos.width;
                editingDisplay.intValue3 = ImagePack.DrawField(pos, Lang.GetTranslate("Bonus Damage Amount") + ":", editingDisplay.intValue3);
                //pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
             //   showSkillModFields = true;
            }
            if (!editingDisplay.effectType.Contains("Dot"))
            {
                editingDisplay.duration = ImagePack.DrawField(pos, Lang.GetTranslate("Damage Delay")+":", editingDisplay.duration);
                pos.y += ImagePack.fieldHeight;
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


            if (editingDisplay.effectType.Equals("FlatDamageEffect"))
                showSkillModFields = false;
            return pos;
        }

        public void LoadDamageOptions()
        {
            // Read all entries from the table
            string query = "SELECT name FROM damage_type where isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                damageOptions = new string[rows.Count];
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    damageOptions[optionsId - 1] = data["name"];
                }
            }
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