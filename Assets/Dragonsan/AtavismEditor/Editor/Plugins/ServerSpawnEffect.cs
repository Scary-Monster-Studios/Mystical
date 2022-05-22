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
    public class ServerSpawnEffect : ServerEffectType
    {

        public new string effectType = "Spawn";
        public new string[] effectTypeOptions = new string[] { "SpawnEffect" };

        public int[] spawnIds = new int[] { -1 };
        public string[] spawnOptions = new string[] { "~ none ~" };
        public int[] spawnPetIds = new int[] { -1 };
        public string[] spawnPetOptions = new string[] { "~ none ~" };
        public int[] spawnPassiveIds = new int[] { -1 };
        public string[] spawnPassiveOptions = new string[] { "~ none ~" };
        string[] spawnType = new string[] { "Wild",/* "Quest Follower", */"Non Combat pet", "Combat pet" };//CapturedCombatPet=4
        public int[] spawnTypeIds = new int[] { 0/*,1*/, 2, 3 };
        string searchString = "";
        string searchPassiveString = "";
        // Use this for initialization
        public ServerSpawnEffect()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
            LoadSpawnOptions();
            LoadSpawnPetOptions();
            LoadSpawnPassivetOptions();
        }

        public void LoadSpawnOptions()
        {
            string query = "SELECT id, name FROM spawn_data where isactive = 1 AND instance is NULL";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                spawnOptions = new string[rows.Count + 1];
                spawnOptions[optionsId] = "~ none ~";
                spawnIds = new int[rows.Count + 1];
                spawnIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    spawnOptions[optionsId] = data["id"] + ":" + data["name"];
                    spawnIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        public void LoadSpawnPetOptions()
        {
            string query = "SELECT id, name FROM mob_templates where isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            //   Debug.LogError("Rows " + rows.Count);
            if ((rows != null) && (rows.Count > 0))
            {
                spawnPetOptions = new string[rows.Count + 1];
                spawnPetOptions[optionsId] = "~ none ~";
                spawnPetIds = new int[rows.Count + 1];
                spawnPetIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    spawnPetOptions[optionsId] = data["id"] + ":" + data["name"];
                    spawnPetIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        public void LoadSpawnPassivetOptions()
        {
            string query = "SELECT id, name FROM effects where isactive = 1 AND passive = 1 ";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            //    Debug.LogError("Rows " + rows.Count);
            if ((rows != null) && (rows.Count > 0))
            {
                spawnPassiveOptions = new string[rows.Count + 1];
                spawnPassiveOptions[optionsId] = "~ none ~";
                spawnPassiveIds = new int[rows.Count + 1];
                spawnPassiveIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    spawnPassiveOptions[optionsId] = data["id"] + ":" + data["name"];
                    spawnPassiveIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }


        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {

            //	editingDisplay.intValue2 = 0;
            int selectedTypeSpawn = GetOptionPosition(editingDisplay.intValue2, spawnTypeIds);
            selectedTypeSpawn = ImagePack.DrawSelector(pos, Lang.GetTranslate("Spawn Type")+":", selectedTypeSpawn, spawnType);
            editingDisplay.intValue2 = spawnTypeIds[selectedTypeSpawn];

            pos.y += ImagePack.fieldHeight;
            if (editingDisplay.intValue2 == 0)
            {
                int selectedSpawn = GetOptionPosition(editingDisplay.intValue1, spawnIds);
                selectedSpawn = ImagePack.DrawSelector(pos, Lang.GetTranslate("Spawn Data")+":", selectedSpawn, spawnOptions);
                editingDisplay.intValue1 = spawnIds[selectedSpawn];
            }
            if (editingDisplay.intValue2 > 1)
            {
                int selectedSpawn = GetOptionPosition(editingDisplay.intValue1, spawnPetIds);
                selectedSpawn = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Pet Model")+":", ref searchString, selectedSpawn, spawnPetOptions);
                editingDisplay.intValue1 = spawnPetIds[selectedSpawn];
                pos.y += ImagePack.fieldHeight;
                editingDisplay.duration = ImagePack.DrawField(pos, Lang.GetTranslate("Duration")+":", editingDisplay.duration);

            }
            pos.y += ImagePack.fieldHeight;

            int selectedEffect = GetOptionPosition(editingDisplay.intValue3, spawnPassiveIds);
            selectedEffect = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Passive Effect")+":", ref searchPassiveString, selectedEffect, spawnPassiveOptions);
            editingDisplay.intValue3 = spawnPassiveIds[selectedEffect];

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
    }
}