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
    public class ServerBuildObjectEffects : ServerEffectType
    {

        public new string effectType = "Build Object";
        public new string[] effectTypeOptions = new string[] { "BuildObjectEffect" };

        public int[] buildObjectIds = new int[] { -1 };
        public string[] buildObjectOptions = new string[] { "~ none ~" };

        // Use this for initialization
        public ServerBuildObjectEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
            LoadBuildObjectOptions();
        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            pos.y += ImagePack.fieldHeight;
            int selectedBuildObject = GetOptionPosition(editingDisplay.intValue1, buildObjectIds);
            selectedBuildObject = ImagePack.DrawSelector(pos, Lang.GetTranslate("Build Object")+":", selectedBuildObject, buildObjectOptions);
            editingDisplay.intValue1 = buildObjectIds[selectedBuildObject];
            pos.y += ImagePack.fieldHeight;

            showTimeFields = false;
            showSkillModFields = false;
            return pos;
        }

        public void LoadBuildObjectOptions()
        {
            string query = "SELECT id, name FROM build_object_template";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                buildObjectOptions = new string[rows.Count + 1];
                buildObjectOptions[optionsId] = "~ none ~";
                buildObjectIds = new int[rows.Count + 1];
                buildObjectIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    buildObjectOptions[optionsId] = data["id"] + ":" + data["name"];
                    buildObjectIds[optionsId] = int.Parse(data["id"]);
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