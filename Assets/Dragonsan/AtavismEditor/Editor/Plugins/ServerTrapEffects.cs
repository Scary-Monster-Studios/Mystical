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
    public class ServerTrapEffects : ServerEffectType
    {

        public new string effectType = "Trap";
        public new string[] effectTypeOptions = new string[] { "TrapEffect" };
        public new string[] targetType = new string[] { "Friendly", "Enemy" };
        public int[] abilityIds = new int[] { -1 };
        public string[] abilityOptions = new string[] { "~ none ~" };
        string abilitySearchAuto = "";
        // Use this for initialization
        public ServerTrapEffects()
        {
            abilitySearchAuto = "";
        }
        public void LoadAbilityOptions()
        {
            string query = "SELECT id, name FROM abilities where isactive = 1";
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                abilityOptions = new string[rows.Count + 1];
                abilityOptions[optionsId] = "~ none ~";
                abilityIds = new int[rows.Count + 1];
                abilityIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    abilityOptions[optionsId] = data["id"] + ":" + data["name"];
                    abilityIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }
        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
            LoadAbilityOptions();
        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {

            if (editingDisplay.floatValue1 < 1f)
                editingDisplay.floatValue1 = 1f;

            editingDisplay.floatValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Size") + ":", editingDisplay.floatValue1);
            pos.y += ImagePack.fieldHeight;
            //pos.x += pos.width;
            editingDisplay.floatValue2 = ImagePack.DrawField(pos, Lang.GetTranslate("Duration (s)") + ":", editingDisplay.floatValue2);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.floatValue3 = ImagePack.DrawField(pos, Lang.GetTranslate("Activation Time (s)") + ":", editingDisplay.floatValue3);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.intValue2 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Target Type") + ":", editingDisplay.intValue2, targetType);
            pos.y += ImagePack.fieldHeight;
            //   editingDisplay.intValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Time (s)") + ":", editingDisplay.intValue1);
            int selectedAbility = GetPositionOfAbility(editingDisplay.intValue1);
            selectedAbility = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Ability") + ":", ref abilitySearchAuto, selectedAbility, abilityOptions);
            editingDisplay.intValue1 = abilityIds[selectedAbility];
            // pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            pos.width *= 2;
            editingDisplay.stringValue1 = ImagePack.DrawGameObject(pos, Lang.GetTranslate("Model") + ":", editingDisplay.stringValue1, 0.75f);
            pos.width /= 2;
            pos.y += ImagePack.fieldHeight ;
            showSkillModFields = true;
            showTimeFields = false;
            return pos;
        }

        private int GetPositionOfAbility(int abilityID)
        {
            for (int i = 0; i < abilityIds.Length; i++)
            {
                if (abilityIds[i] == abilityID)
                    return i;
            }
            return 0;
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