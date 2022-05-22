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
    public class ServerTeachAbilityEffects : ServerEffectType
    {

        public new string effectType = "Teach Ability";
        public new string[] effectTypeOptions = new string[] { "TeachAbilityEffect" };

        public int[] abilityIds = new int[] { -1 };
        public string[] abilityOptions = new string[] { "~ none ~" };

        // Use this for initialization
        public ServerTeachAbilityEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
            LoadAbilityOptions();
        }

        public void LoadAbilityOptions()
        {
            string query = "SELECT id, name FROM abilities where isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
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

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            pos.y += ImagePack.fieldHeight;
            int selectedAbility = GetPositionOfAbility(editingDisplay.intValue1);
            selectedAbility = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Ability")+":", ref editingDisplay.stringValue1, selectedAbility, abilityOptions);
            editingDisplay.intValue1 = abilityIds[selectedAbility];

            pos.y += ImagePack.fieldHeight;
            showTimeFields = false;
            showSkillModFields = false;
            return pos;
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

        private int GetPositionOfAbility(int abilityID)
        {
            for (int i = 0; i < abilityIds.Length; i++)
            {
                if (abilityIds[i] == abilityID)
                    return i;
            }
            return 0;
        }
    }
}