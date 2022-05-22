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
    public class ServerCreateItemEffects : ServerEffectType
    {

        public new string effectType = "CreateItem";
        public new string[] effectTypeOptions = new string[] { "Create Item" };

        public int[] itemIds = new int[] { -1 };
        public string[] itemsList = new string[] { "~ none ~" };

        // Filter/Search inputs
        private string itemSearchInput = "";

        // Use this for initialization
        public ServerCreateItemEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
            LoadItemList();
        }

        public void LoadItemList()
        {
            string query = "SELECT id, name FROM item_templates where isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                itemsList = new string[rows.Count + 1];
                itemsList[optionsId] = "~ none ~";
                itemIds = new int[rows.Count + 1];
                itemIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    itemsList[optionsId] = data["id"] + ":" + data["name"];
                    itemIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            pos.y += ImagePack.fieldHeight;
            int selectedTask = GetOptionPosition(editingDisplay.intValue1, itemIds);
            //selectedTask = ImagePack.DrawSelector (pos, "Item:", selectedTask, itemsList);
            selectedTask = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item")+": ", ref itemSearchInput, selectedTask, itemsList);
            editingDisplay.intValue1 = itemIds[selectedTask];
          //  pos.y += ImagePack.fieldHeight;
           // editingDisplay.intValue2 = ImagePack.DrawField(pos, Lang.GetTranslate("Count") + ":", editingDisplay.intValue2);

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