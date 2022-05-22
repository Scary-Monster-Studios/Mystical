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
    public class ServerTaskEffects : ServerEffectType
    {

        public new string effectType = "Task";
        public new string[] effectTypeOptions = new string[] { "TaskCompleteEffect" };

        public int[] taskIds = new int[] { -1 };
        public string[] taskOptions = new string[] { "~ none ~" };

        // Use this for initialization
        public ServerTaskEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
            LoadTaskOptions();
        }

        public void LoadTaskOptions()
        {
            string query = "SELECT id, name FROM task where isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                taskOptions = new string[rows.Count + 1];
                taskOptions[optionsId] = "~ none ~";
                taskIds = new int[rows.Count + 1];
                taskIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    taskOptions[optionsId] = data["id"] + ":" + data["name"];
                    taskIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            int selectedTask = GetOptionPosition(editingDisplay.intValue1, taskIds);
            selectedTask = ImagePack.DrawSelector(pos, Lang.GetTranslate("Task")+":", selectedTask, taskOptions);
            editingDisplay.intValue1 = taskIds[selectedTask];

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
    }
}