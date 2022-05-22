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
    public class ServerCreateItemFromLootEffects : ServerEffectType
    {

        public new string effectType = "CreateItemFromLoot";
        public new string[] effectTypeOptions = new string[] { "Create Item From Loot" };

        public int[] itemIds = new int[] { -1 };
        public string[] itemsList = new string[] { "~ none ~" };

        public int[] tableIds = new int[] { -1 };
        public string[] tablesList = new string[] { "~ none ~" };

        // Filter/Search inputs
        private string itemSearchInput = "";
        private string itemSearchInput2 = "";
        private string itemSearchInput3 = "";
        private string itemSearchInput4 = "";
        private string itemSearchInput5 = "";


        // Use this for initialization
        public ServerCreateItemFromLootEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
            LoadItemList();
            LoadTableList();

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

        private void LoadTableList()
        {
            string query = "SELECT id, name FROM loot_tables where isactive = 1";

            // If there is a row, clear it.
            /*     if (rows != null)
                     rows.Clear();
                     */
            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                tablesList = new string[rows.Count + 1];
                tablesList[optionsId] = "~ none ~";
                tableIds = new int[rows.Count + 1];
                tableIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    tablesList[optionsId] = data["id"] + ":" + data["name"];
                    tableIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }


        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            pos.y += ImagePack.fieldHeight;
            int selectedTask = GetOptionPosition(editingDisplay.intValue1, tableIds);
            //selectedTask = ImagePack.DrawSelector (pos, "Item:", selectedTask, itemsList);
            selectedTask = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Loot Table")+" #1: ", ref itemSearchInput, selectedTask, tablesList);
            editingDisplay.intValue1 = tableIds[selectedTask];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.floatValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Chance")+":", editingDisplay.floatValue1);
            pos.y += ImagePack.fieldHeight;
            selectedTask = GetOptionPosition(editingDisplay.intValue2, tableIds);
            //selectedTask = ImagePack.DrawSelector (pos, "Item:", selectedTask, itemsList);
            selectedTask = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Loot Table")+" #2: ", ref itemSearchInput2, selectedTask, tablesList);
            editingDisplay.intValue2 = tableIds[selectedTask];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.floatValue2 = ImagePack.DrawField(pos, Lang.GetTranslate("Chance")+":", editingDisplay.floatValue2);
            pos.y += ImagePack.fieldHeight;
            selectedTask = GetOptionPosition(editingDisplay.intValue3, tableIds);
            //selectedTask = ImagePack.DrawSelector (pos, "Item:", selectedTask, itemsList);
            selectedTask = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Loot Table")+" #3: ", ref itemSearchInput3, selectedTask, tablesList);
            editingDisplay.intValue3 = tableIds[selectedTask];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.floatValue3 = ImagePack.DrawField(pos, Lang.GetTranslate("Chance")+":", editingDisplay.floatValue3);
            pos.y += ImagePack.fieldHeight;
            selectedTask = GetOptionPosition(editingDisplay.intValue4, tableIds);
            //selectedTask = ImagePack.DrawSelector (pos, "Item:", selectedTask, itemsList);
            selectedTask = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Loot Table")+" #4: ", ref itemSearchInput4, selectedTask, tablesList);
            editingDisplay.intValue4 = tableIds[selectedTask];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.floatValue4 = ImagePack.DrawField(pos, Lang.GetTranslate("Chance")+":", editingDisplay.floatValue4);
            pos.y += ImagePack.fieldHeight;
            selectedTask = GetOptionPosition(editingDisplay.intValue5, tableIds);
            //selectedTask = ImagePack.DrawSelector (pos, "Item:", selectedTask, itemsList);
            selectedTask = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Loot Table")+" #5: ", ref itemSearchInput5, selectedTask, tablesList);
            editingDisplay.intValue5 = tableIds[selectedTask];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.floatValue5 = ImagePack.DrawField(pos, Lang.GetTranslate("Chance")+":", editingDisplay.floatValue5);

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