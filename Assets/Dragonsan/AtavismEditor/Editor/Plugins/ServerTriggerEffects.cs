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
    public class ServerTriggerEffects : ServerEffectType
    {

        public new string effectType = "Trigger";
        public new string[] effectTypeOptions = new string[] { "TriggerEffect" };

        public int[] triggerIds = new int[] { -1 };
        public string[] triggerOptions = new string[] { "~ none ~" };
        string searchString = "";
        List<int> profiles = new List<int>();
        List<string> profilesearch = new List<string>();
        Char delimiter = ';';

        // string searchPassiveString = "";
        // Use this for initialization
        public ServerTriggerEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
            LoadTriggersOptions();
           
        }

        public void LoadTriggersOptions()
        {
            string query = "SELECT id, name FROM effects_triggers where isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                triggerOptions = new string[rows.Count + 1];
                triggerOptions[optionsId] = "~ none ~";
                triggerIds = new int[rows.Count + 1];
                triggerIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    triggerOptions[optionsId] = data["id"] + ":" + data["name"];
                    triggerIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }



        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {

            //	editingDisplay.intValue2 = 0;
            pos.y += ImagePack.fieldHeight;
            if (profiles.Count == 0 && editingDisplay.stringValue1.Length > 0)
            {
                if (editingDisplay.stringValue1.Length > 0)
                {
                    string[] splitted = editingDisplay.stringValue1.Split(delimiter);
                    for (int i = 0; i < splitted.Length; i++)
                    {
                        profiles.Add(int.Parse(splitted[i]));
                        profilesearch.Add("");

                    }
                }
            }

            for (int j = 0; j < profiles.Count; j++)
            {
                pos.y += ImagePack.fieldHeight;
                int selectedTag = GetOptionPosition(profiles[j], triggerIds);
                string search = profilesearch[j];
                selectedTag = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Trigger Profile ") + (j + 1) + ":", ref search, selectedTag, triggerOptions);
                profilesearch[j] = search;
                profiles[j] = triggerIds[selectedTag];


                pos.x += pos.width;
                pos.y += ImagePack.fieldHeight;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Trigger Profile")))
                {

                    profiles.RemoveAt(j);
                    profilesearch.RemoveAt(j);
                }
                pos.x -= pos.width;
            }
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Trigger Profile")))
            {
                profiles.Add(0);
                profilesearch.Add("");
            }
            if (profiles.Count > 0)
                editingDisplay.stringValue1 = string.Join(";", profiles.ConvertAll(i => i.ToString()).ToArray());


            pos.y += ImagePack.fieldHeight;
            showSkillModFields = false;
            showTimeFields = true;
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