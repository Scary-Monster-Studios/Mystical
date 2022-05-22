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
    public class ServerSetRespawnLocationEffects : ServerEffectType
    {

        public new string effectType = "Set Respawn Location";
        public new string[] effectTypeOptions = new string[] { "SetRespawnLocationEffect" };

        public int[] instanceIds = new int[] { -1 };
        public string[] instanceOptions = new string[] { "~ none ~" };

        // Use this for initialization
        public ServerSetRespawnLocationEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
            LoadInstanceOptions();
        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            int instanceID = GetOptionPosition(editingDisplay.intValue1, instanceIds);
            instanceID = ImagePack.DrawSelector(pos, Lang.GetTranslate("Instance")+":", instanceID, instanceOptions);
            editingDisplay.intValue1 = instanceIds[instanceID];
            pos.y += ImagePack.fieldHeight*1.5f;
            pos.width *= 2;
            GameObject target = (GameObject)ImagePack.DrawObject(pos, Lang.GetTranslate("Drag a Game Object to get its Position")+":", null);
            pos.width /= 2;
            if (target != null)
            {
                editingDisplay.floatValue1 = target.transform.position.x;
                editingDisplay.floatValue2 = target.transform.position.y;
                editingDisplay.floatValue3 = target.transform.position.z;
            }
            pos.y += ImagePack.fieldHeight;
            Vector3 position = new Vector3(editingDisplay.floatValue1, editingDisplay.floatValue2, editingDisplay.floatValue3);
            pos.width *= 2;
            position = ImagePack.Draw3DPosition(pos, Lang.GetTranslate("Location")+":", position);
            pos.width /= 2;
            editingDisplay.floatValue1 = position.x;
            editingDisplay.floatValue2 = position.y;
            editingDisplay.floatValue3 = position.z;
            pos.y += ImagePack.fieldHeight * 1.5f;

            showTimeFields = false;
            showSkillModFields = false;
            return pos;
        }

        public void LoadInstanceOptions()
        {
            // Read all entries from the table
            string query = "SELECT id, island_name FROM instance_template";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.adminDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                instanceOptions = new string[rows.Count + 1];
                instanceOptions[optionsId] = "~ none ~";
                instanceIds = new int[rows.Count + 1];
                instanceIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    instanceOptions[optionsId] = data["id"] + ":" + data["island_name"];
                    instanceIds[optionsId] = int.Parse(data["id"]);
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