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
    public class ServerTeachSkillEffects : ServerEffectType
    {

        public new string effectType = "Teach Skill";
        public new string[] effectTypeOptions = new string[] { "TeachSkillEffect" };

        public int[] skillIds = new int[] { -1 };
        public string[] skillOptions = new string[] { "~ none ~" };

        // Use this for initialization
        public ServerTeachSkillEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
            LoadSkillOptions();
        }

        public void LoadSkillOptions()
        {
            string query = "SELECT id, name FROM skills where isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                skillOptions = new string[rows.Count + 1];
                skillOptions[optionsId] = "~ none ~";
                skillIds = new int[rows.Count + 1];
                skillIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    skillOptions[optionsId] = data["id"] + ":" + data["name"];
                    skillIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            int selectedSkill = GetPositionOfSkill(editingDisplay.intValue1);
            selectedSkill = ImagePack.DrawSelector(pos, Lang.GetTranslate("Skill To Learn")+":", selectedSkill, skillOptions);
            editingDisplay.intValue1 = skillIds[selectedSkill];

            pos.y += ImagePack.fieldHeight;
            showSkillModFields = false;
            showTimeFields = false;
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

        private int GetPositionOfSkill(int skillID)
        {
            for (int i = 0; i < skillIds.Length; i++)
            {
                if (skillIds[i] == skillID)
                    return i;
            }
            return 0;
        }
    }
}