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
    // Handles the Crafting Recipes Configuration
    public class ServerCraftingRecipes : AtavismDatabaseFunction
    {

        public new Dictionary<int, CraftingRecipe> dataRegister;
        public new CraftingRecipe editingDisplay;
        public new CraftingRecipe originalDisplay;

        public int[] itemIds = new int[] { -1 };
        public string[] itemsList = new string[] { "~ none ~" };
        public int[] skillIds = new int[] { -1 };
        public string[] skillOptions = new string[] { "~ none ~" };

        public string[] stationOptions = new string[] { "~ none ~" };

        // Filter/Search inputs
        //private string recipeSearchInput = "";
        private string resultSearchInput1 = "";
        private string resultSearchInput2 = "";
        private string resultSearchInput3 = "";
        private string resultSearchInput4 = "";
        private string resultSearchInput5 = "";
        private string resultSearchInput6 = "";
        private string resultSearchInput7 = "";
        private string resultSearchInput8 = "";
        private string resultSearchInput9 = "";
        private string resultSearchInput10 = "";
        private string resultSearchInput11 = "";
        private string resultSearchInput12 = "";
        private string resultSearchInput13 = "";
        private string resultSearchInput14 = "";
        private string resultSearchInput15 = "";
        private string resultSearchInput16 = "";
        private List<string> effectSearchInput = new List<string>();

        // Handles the prefab creation, editing and save
        private CraftingRecipePrefab prefab = null;

        // Use this for initialization
        public ServerCraftingRecipes()
        {
            functionName = "Crafting Recipes";
            // Database tables name
            tableName = "crafting_recipes";
            functionTitle = "Crafting Recipe Configuration";
            loadButtonLabel = "Load Crafting Recipes";
            notLoadedText = "No Crafting Recipe loaded.";
            // Init
            dataRegister = new Dictionary<int, CraftingRecipe>();

            editingDisplay = new CraftingRecipe();
            originalDisplay = new CraftingRecipe();
        }
        void resetSearch(bool fokus)
        {
            resultSearchInput1 = "";
            resultSearchInput2 = "";
            resultSearchInput3 = "";
            resultSearchInput4 = "";
            resultSearchInput5 = "";
            resultSearchInput6 = "";
            resultSearchInput7 = "";
            resultSearchInput8 = "";
            resultSearchInput9 = "";
            resultSearchInput10 = "";
            resultSearchInput11 = "";
            resultSearchInput12 = "";
            resultSearchInput13 = "";
            resultSearchInput14 = "";
            resultSearchInput15 = "";
            resultSearchInput16 = "";
            effectSearchInput.Clear();
           if(fokus) GUI.FocusControl(null);
        }

        public override void Activate()
        {
            linkedTablesLoaded = false;
            resetSearch(true);
        }

        private void LoadItemList()
        {
            string query = "SELECT id, name FROM item_templates where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
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

        public void LoadSkillOptions()
        {
            string query = "SELECT id, name FROM skills where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
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

        // Load Database Data
        public override void Load()
        {
            if (!dataLoaded)
            {
                // Clean old data
                // dataRegister.Clear();
                displayKeys.Clear();

                // Read all entries from the table
                string query = "SELECT id,name FROM " + tableName + " where isactive = 1";

                // If there is a row, clear it.
                if (rows != null)
                    rows.Clear();

                // Load data
                rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
                //Debug.Log("#Rows:"+rows.Count);
                // Read all the data
                displayList.Clear();
                if ((rows != null) && (rows.Count > 0))
                {
                    foreach (Dictionary<string, string> data in rows)
                    {
                        displayList.Add(data["id"] + ". " + data["name"]);
                        displayKeys.Add(int.Parse(data["id"]));

                    }
                }
                dataLoaded = true;
                editingDisplay = null;
            }
        }

        CraftingRecipe LoadEntity(int id)
        {
            // Read all entries from the table
            string query = "SELECT " + originalDisplay.GetFieldsString() + " FROM " + tableName + " where isactive = 1 AND id="+id;

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            CraftingRecipe display = new CraftingRecipe();
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    //foreach(string key in data.Keys)
                    //	Debug.Log("Name[" + key + "]:" + data[key]);
                    //return;
                    display.id = int.Parse(data["id"]);
                    display.Name = data["name"];
                    display.icon = data["icon"];
                    display.resultItemID = int.Parse(data["resultItemID"]);
                    display.resultItemCount = int.Parse(data["resultItemCount"]);
                    display.resultItem2ID = int.Parse(data["resultItem2ID"]);
                    display.resultItem2Count = int.Parse(data["resultItem2Count"]);
                    display.resultItem3ID = int.Parse(data["resultItem3ID"]);
                    display.resultItem3Count = int.Parse(data["resultItem3Count"]);
                    display.resultItem4ID = int.Parse(data["resultItem4ID"]);
                    display.resultItem4Count = int.Parse(data["resultItem4Count"]);
                    display.chance = float.Parse(data["chance"]);
                    display.resultItem5ID = int.Parse(data["resultItem5ID"]);
                    display.resultItem5Count = int.Parse(data["resultItem5Count"]);
                    display.resultItem6ID = int.Parse(data["resultItem6ID"]);
                    display.resultItem6Count = int.Parse(data["resultItem6Count"]);
                    display.resultItem7ID = int.Parse(data["resultItem7ID"]);
                    display.resultItem7Count = int.Parse(data["resultItem7Count"]);
                    display.resultItem8ID = int.Parse(data["resultItem8ID"]);
                    display.resultItem8Count = int.Parse(data["resultItem8Count"]);
                    display.chance2 = float.Parse(data["chance2"]);
                    display.resultItem9ID = int.Parse(data["resultItem9ID"]);
                    display.resultItem9Count = int.Parse(data["resultItem9Count"]);
                    display.resultItem10ID = int.Parse(data["resultItem10ID"]);
                    display.resultItem10Count = int.Parse(data["resultItem10Count"]);
                    display.resultItem11ID = int.Parse(data["resultItem11ID"]);
                    display.resultItem11Count = int.Parse(data["resultItem11Count"]);
                    display.resultItem12ID = int.Parse(data["resultItem12ID"]);
                    display.resultItem12Count = int.Parse(data["resultItem12Count"]);
                    display.chance3 = float.Parse(data["chance3"]);
                    display.resultItem13ID = int.Parse(data["resultItem13ID"]);
                    display.resultItem13Count = int.Parse(data["resultItem13Count"]);
                    display.resultItem14ID = int.Parse(data["resultItem14ID"]);
                    display.resultItem14Count = int.Parse(data["resultItem14Count"]);
                    display.resultItem15ID = int.Parse(data["resultItem15ID"]);
                    display.resultItem15Count = int.Parse(data["resultItem15Count"]);
                    display.resultItem16ID = int.Parse(data["resultItem16ID"]);
                    display.resultItem16Count = int.Parse(data["resultItem16Count"]);
                    display.chance4 = float.Parse(data["chance4"]);
                    display.skillID = int.Parse(data["skillID"]);
                    display.skillLevelReq = int.Parse(data["skillLevelReq"]);
                    display.stationReq = data["stationReq"];
                    display.recipeItemID = int.Parse(data["recipeItemID"]);
                    display.creationTime = int.Parse(data["creationTime"]);
                    display.layoutReq = bool.Parse(data["layoutReq"]);
                    display.allowDyes = bool.Parse(data["allowDyes"]);
                    display.allowEssences = bool.Parse(data["allowEssences"]);
                    display.crafting_xp = int.Parse(data["crafting_xp"]);

                    for (int i = 1; i <= display.maxEntries; i++)
                    {
                        int itemId = int.Parse(data["component" + i]);
                        int count = int.Parse(data["component" + i + "count"]);
                        RecipeComponentEntry entry = new RecipeComponentEntry(itemId, count);
                        display.entries.Add(entry);
                    }

                    display.isLoaded = true;
                    //Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
                }
            }
             return display;
        }

        public override void LoadRestore()
        {
            if (!dataRestoreLoaded)
            {
                // Clean old data
                dataRegister.Clear();
                displayKeys.Clear();

                // Read all entries from the table
                string query = "SELECT id,name FROM " + tableName + " where isactive = 0";

                // If there is a row, clear it.
                if (rows != null)
                    rows.Clear();

                // Load data
                rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
                //Debug.Log("#Rows:"+rows.Count);
                // Read all the data
                displayList.Clear();
                if ((rows != null) && (rows.Count > 0))
                {
                    foreach (Dictionary<string, string> data in rows)
                    {
                        displayList.Add(data["id"] + ". " + data["name"]);
                        displayKeys.Add(int.Parse(data["id"]));

                    }
                }
                dataRestoreLoaded = true;
            }
        }


        // Draw the loaded list
        public override void DrawLoaded(Rect box)
        {
            // Setup the layout
            Rect pos = box;
            pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;

            if (displayList.Count <= 0)
            {
                pos.y += ImagePack.fieldHeight;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create a Crafting Recipe before edit it."));
                return;
            }


            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Crafting Recipe"));

            if (newItemCreated)
            {
                newItemCreated = false;
                newSelectedDisplay = displayKeys.Count - 1;
            }
            // Draw data Editor
            if (newSelectedDisplay != selectedDisplay)
            {
                selectedDisplay = newSelectedDisplay;
                int displayKey = -1;
                if (selectedDisplay >= 0)
                    displayKey = displayKeys[selectedDisplay];
                editingDisplay = LoadEntity(displayKey);
                resetSearch(false);
            }
            if (editingDisplay == null)
            {
                int displayKey = -1;
                if (selectedDisplay >= 0)
                    displayKey = displayKeys[selectedDisplay];
                editingDisplay = LoadEntity(displayKey);
            }

            pos.y += ImagePack.fieldHeight * 1.5f;
            pos.x -= ImagePack.innerMargin;
            pos.y -= ImagePack.innerMargin;
            pos.width += ImagePack.innerMargin;

            if (state != State.Loaded)
            {
                pos.x += ImagePack.innerMargin;
                pos.width /= 2;
                //Draw super magical compound object.
                newSelectedDisplay = ImagePack.DrawDynamicPartialListSelector(pos, Lang.GetTranslate("Search Filter")+": ", ref entryFilterInput, selectedDisplay, displayList);

                pos.y += ImagePack.fieldHeight * 1.5f;
              /*  if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Generate Prefabs")))
                {
                    GenerateAllPrefabs();
                }*/
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Duplicate")))
                {
                    Duplicate();
                }
                pos.x -= pos.width;

                pos.width *= 2;
                pos.y += ImagePack.fieldHeight * 1.5f;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Recipe Properties")+":");
                pos.y += ImagePack.fieldHeight * 0.75f;
            }

            DrawEditor(pos, false);

            pos.y -= ImagePack.fieldHeight;
            //pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
        }

        public override void DrawRestore(Rect box)
        {
            // Setup the layout
            Rect pos = box;
            pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;

            if (displayList.Count <= 0)
            {
                pos.y += ImagePack.fieldHeight;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You dont have deleted")+" " + Lang.GetTranslate(functionName) + ".");
                return;
            }

            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Restore")+" " + Lang.GetTranslate(functionName) + " "+ Lang.GetTranslate("Configuration"));
            pos.y += ImagePack.fieldHeight;

            pos.width -= 140 + 155;
            for (int i = 0; i < displayList.Count; i++)
            {
                ImagePack.DrawText(pos, displayList[i]);
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos, Lang.GetTranslate("Delete Permanently")))
                {
                    DeleteForever(displayKeys[i]);
                }
                pos.x += 155;
                if (ImagePack.DrawButton(pos, Lang.GetTranslate("Restore")))
                {
                    RestoreEntry(displayKeys[i]);
                }

                pos.x -= pos.width + 155;
                pos.y += ImagePack.fieldHeight;
            }
            pos.width += 140+ 155;
            showCancel = false;
            showDelete = false;
            showSave = false;
            if (resultTimeout != -1 && resultTimeout < Time.realtimeSinceStartup && result != "")
            {
                result = "";
            }
            EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight);
        }

        public override void CreateNewData()
        {
            editingDisplay = new CraftingRecipe();
            originalDisplay = new CraftingRecipe();
            selectedDisplay = -1;
        }
        // Edit or Create
        public override void DrawEditor(Rect box, bool nItem)
        {
            newEntity = nItem;
            if (!linkedTablesLoaded)
            {
                // Load items
                LoadItemList();
                LoadSkillOptions();
                stationOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Crafting Station", true);
                linkedTablesLoaded = true;
            }
            // Setup the layout
            Rect pos = box;
            pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;

            // Draw the content database info		
            if (newEntity)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new crafting recipe"));
                pos.y += ImagePack.fieldHeight;
            }
            if (editingDisplay == null)
                return;
            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.5f);
            pos.y += ImagePack.fieldHeight;
            pos.width /= 2;
            editingDisplay.crafting_xp = ImagePack.DrawField(pos, Lang.GetTranslate("Experience")+":", editingDisplay.crafting_xp);

            //pos.y += ImagePack.fieldHeight;
            int selectedItem = GetOptionPosition(editingDisplay.recipeItemID, itemIds);
            //selectedItem = ImagePack.DrawSelector (pos, "Recipe:", selectedItem, itemsList);
            //	selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, "Recipe: ", ref recipeSearchInput, selectedItem, itemsList);
            //	editingDisplay.recipeItemID = itemIds[selectedItem];
            pos.x += pos.width;
            string icon = ImagePack.DrawSpriteAsset(pos, Lang.GetTranslate("Icon") + ":", editingDisplay.icon);
        /*    if (editingDisplay.icon.Length > 3 && icon == " ")
            {
                CraftingRecipePrefab item = new CraftingRecipePrefab(editingDisplay);
                editingDisplay.icon = item.LoadIcon();
            }
            else
            {*/
                editingDisplay.icon = icon;
           // }
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight * 3;
            // Item 1

            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Group 1 of Creates Item"));
            pos.y += 1.5f * ImagePack.fieldHeight;
            editingDisplay.chance = ImagePack.DrawField(pos, Lang.GetTranslate("Chance")+":", editingDisplay.chance);
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.resultItemID, itemIds);
            //selectedItem = ImagePack.DrawSelector (pos, "Creates Item:", selectedItem, itemsList);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Creates Item")+":", ref resultSearchInput1, selectedItem, itemsList);
            editingDisplay.resultItemID = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.resultItemCount = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.resultItemCount);
            pos.y += ImagePack.fieldHeight;
            // Item 2
            selectedItem = GetOptionPosition(editingDisplay.resultItem2ID, itemIds);
            //selectedItem = ImagePack.DrawSelector (pos, "And Item:", selectedItem, itemsList);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("And Item")+":", ref resultSearchInput2, selectedItem, itemsList);
            editingDisplay.resultItem2ID = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.resultItem2Count = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.resultItem2Count);
            pos.y += ImagePack.fieldHeight;
            // Item 3
            selectedItem = GetOptionPosition(editingDisplay.resultItem3ID, itemIds);
            //selectedItem = ImagePack.DrawSelector (pos, "And Item:", selectedItem, itemsList);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("And Item")+": ", ref resultSearchInput3, selectedItem, itemsList);
            editingDisplay.resultItem3ID = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.resultItem3Count = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.resultItem3Count);
            pos.y += ImagePack.fieldHeight;
            // Item 4
            selectedItem = GetOptionPosition(editingDisplay.resultItem4ID, itemIds); ;
            //selectedItem = ImagePack.DrawSelector (pos, "And Item:", selectedItem, itemsList);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("And Item")+": ", ref resultSearchInput4, selectedItem, itemsList);
            editingDisplay.resultItem4ID = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.resultItem4Count = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.resultItem4Count);
            pos.y += 1.5f * ImagePack.fieldHeight;

            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Group 2 of Creates Item"));
            pos.y += 1.5f * ImagePack.fieldHeight;
            editingDisplay.chance2 = ImagePack.DrawField(pos, Lang.GetTranslate("Chance")+":", editingDisplay.chance2);
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.resultItem5ID, itemIds);
            //selectedItem = ImagePack.DrawSelector (pos, "Creates Item:", selectedItem, itemsList);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Creates Item")+": ", ref resultSearchInput5, selectedItem, itemsList);
            editingDisplay.resultItem5ID = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.resultItem5Count = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.resultItem5Count);
            pos.y += ImagePack.fieldHeight;
            // Item 2
            selectedItem = GetOptionPosition(editingDisplay.resultItem6ID, itemIds);
            //selectedItem = ImagePack.DrawSelector (pos, "And Item:", selectedItem, itemsList);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("And Item")+": ", ref resultSearchInput6, selectedItem, itemsList);
            editingDisplay.resultItem6ID = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.resultItem6Count = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.resultItem6Count);
            pos.y += ImagePack.fieldHeight;
            // Item 3
            selectedItem = GetOptionPosition(editingDisplay.resultItem7ID, itemIds);
            //selectedItem = ImagePack.DrawSelector (pos, "And Item:", selectedItem, itemsList);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("And Item")+": ", ref resultSearchInput7, selectedItem, itemsList);
            editingDisplay.resultItem7ID = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.resultItem3Count = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.resultItem3Count);
            pos.y += ImagePack.fieldHeight;
            // Item 4
            selectedItem = GetOptionPosition(editingDisplay.resultItem8ID, itemIds); ;
            //selectedItem = ImagePack.DrawSelector (pos, "And Item:", selectedItem, itemsList);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("And Item")+": ", ref resultSearchInput8, selectedItem, itemsList);
            editingDisplay.resultItem8ID = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.resultItem8Count = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.resultItem8Count);
            pos.y += 1.5f * ImagePack.fieldHeight;

            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Group 3 of Creates Item"));
            pos.y += 1.5f * ImagePack.fieldHeight;
            editingDisplay.chance3 = ImagePack.DrawField(pos, Lang.GetTranslate("Chance")+":", editingDisplay.chance3);
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.resultItem9ID, itemIds);
            //selectedItem = ImagePack.DrawSelector (pos, "Creates Item:", selectedItem, itemsList);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Creates Item")+": ", ref resultSearchInput9, selectedItem, itemsList);
            editingDisplay.resultItem9ID = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.resultItem9Count = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.resultItem9Count);
            pos.y += ImagePack.fieldHeight;
            // Item 2
            selectedItem = GetOptionPosition(editingDisplay.resultItem10ID, itemIds);
            //selectedItem = ImagePack.DrawSelector (pos, "And Item:", selectedItem, itemsList);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("And Item")+": ", ref resultSearchInput10, selectedItem, itemsList);
            editingDisplay.resultItem10ID = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.resultItem10Count = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.resultItem10Count);
            pos.y += ImagePack.fieldHeight;
            // Item 3
            selectedItem = GetOptionPosition(editingDisplay.resultItem11ID, itemIds);
            //selectedItem = ImagePack.DrawSelector (pos, "And Item:", selectedItem, itemsList);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("And Item")+": ", ref resultSearchInput11, selectedItem, itemsList);
            editingDisplay.resultItem11ID = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.resultItem11Count = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.resultItem11Count);
            pos.y += ImagePack.fieldHeight;
            // Item 4
            selectedItem = GetOptionPosition(editingDisplay.resultItem12ID, itemIds); ;
            //selectedItem = ImagePack.DrawSelector (pos, "And Item:", selectedItem, itemsList);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("And Item")+": ", ref resultSearchInput12, selectedItem, itemsList);
            editingDisplay.resultItem12ID = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.resultItem12Count = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.resultItem12Count);
            pos.y += 1.5f * ImagePack.fieldHeight;

            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Group 4 of Creates Item"));
            pos.y += 1.5f * ImagePack.fieldHeight;
            editingDisplay.chance4 = ImagePack.DrawField(pos, Lang.GetTranslate("Chance")+":", editingDisplay.chance4);
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.resultItem13ID, itemIds);
            //selectedItem = ImagePack.DrawSelector (pos, "Creates Item:", selectedItem, itemsList);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Creates Item")+": ", ref resultSearchInput13, selectedItem, itemsList);
            editingDisplay.resultItem13ID = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.resultItem13Count = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.resultItem13Count);
            pos.y += ImagePack.fieldHeight;
            // Item 2
            selectedItem = GetOptionPosition(editingDisplay.resultItem14ID, itemIds);
            //selectedItem = ImagePack.DrawSelector (pos, "And Item:", selectedItem, itemsList);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("And Item")+": ", ref resultSearchInput14, selectedItem, itemsList);
            editingDisplay.resultItem14ID = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.resultItem14Count = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.resultItem14Count);
            pos.y += ImagePack.fieldHeight;
            // Item 3
            selectedItem = GetOptionPosition(editingDisplay.resultItem15ID, itemIds);
            //selectedItem = ImagePack.DrawSelector (pos, "And Item:", selectedItem, itemsList);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("And Item")+": ", ref resultSearchInput15, selectedItem, itemsList);
            editingDisplay.resultItem15ID = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.resultItem15Count = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.resultItem15Count);
            pos.y += ImagePack.fieldHeight;
            // Item 4
            selectedItem = GetOptionPosition(editingDisplay.resultItem16ID, itemIds); ;
            //selectedItem = ImagePack.DrawSelector (pos, "And Item:", selectedItem, itemsList);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("And Item")+": ", ref resultSearchInput16, selectedItem, itemsList);
            editingDisplay.resultItem16ID = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.resultItem16Count = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.resultItem16Count);
            pos.y += 1.5f * ImagePack.fieldHeight;




            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Requirements"));
            pos.y += 1.5f * ImagePack.fieldHeight;
            int selectedSkill = GetOptionPosition(editingDisplay.skillID, skillIds);
            selectedSkill = ImagePack.DrawSelector(pos, Lang.GetTranslate("Skill")+":", selectedSkill, skillOptions);
            editingDisplay.skillID = skillIds[selectedSkill];
            pos.x += pos.width;
            editingDisplay.skillLevelReq = ImagePack.DrawField(pos, Lang.GetTranslate("Skill Level")+":", editingDisplay.skillLevelReq);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.stationReq = ImagePack.DrawSelector(pos, Lang.GetTranslate("Station Req")+":", editingDisplay.stationReq, stationOptions);
            pos.x += pos.width;
            editingDisplay.creationTime = ImagePack.DrawField(pos, Lang.GetTranslate("Creation Time")+":", editingDisplay.creationTime);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            //	editingDisplay.qualityChangeable = ImagePack.DrawToggleBox (pos, "Changes Quality:", editingDisplay.qualityChangeable);
            //	pos.y += ImagePack.fieldHeight;
            /*editingDisplay.allowDyes = ImagePack.DrawToggleBox (pos, "Allows Dyes:", editingDisplay.allowDyes);
            pos.x += pos.width;
            editingDisplay.allowEssences = ImagePack.DrawToggleBox (pos, "Allows Essences:", editingDisplay.allowEssences);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;*/
            editingDisplay.layoutReq = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Must Match Layout")+":", editingDisplay.layoutReq);
            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Items Required")+":");
            pos.y += 1.5f * ImagePack.fieldHeight;
            for (int i = 0; i < editingDisplay.maxEntries; i++)
            {
                // Generate search string if none exists
                if (effectSearchInput.Count <= i)
                {
                    effectSearchInput.Add("");
                }
                string searchString = effectSearchInput[i];

                if (editingDisplay.entries.Count <= i)
                    editingDisplay.entries.Add(new RecipeComponentEntry(-1, 1));
                if (i == 0)
                {
                    ImagePack.DrawText(pos, Lang.GetTranslate("Row 1"));
                    pos.y += ImagePack.fieldHeight;
                }
                else if (i == 4)
                {
                    ImagePack.DrawText(pos, Lang.GetTranslate("Row 2"));
                    pos.y += ImagePack.fieldHeight;
                }
                else if (i == 8)
                {
                    ImagePack.DrawText(pos, Lang.GetTranslate("Row 3"));
                    pos.y += ImagePack.fieldHeight;
                }
                else if (i == 12)
                {
                    ImagePack.DrawText(pos, Lang.GetTranslate("Row 4"));
                    pos.y += ImagePack.fieldHeight;
                }
                selectedItem = GetOptionPosition(editingDisplay.entries[i].itemId, itemIds);
                //selectedItem = ImagePack.DrawSelector (pos, "Item " + (i+1) + ":", selectedItem, itemsList);
                selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item")+" " + (i + 1) + ":", ref searchString, selectedItem, itemsList);
                editingDisplay.entries[i].itemId = itemIds[selectedItem];
                pos.y += ImagePack.fieldHeight;
                editingDisplay.entries[i].count = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.entries[i].count);
                pos.y += ImagePack.fieldHeight;

                // Save back the search string
                effectSearchInput[i] = searchString;
            }
            pos.width = pos.width * 2;

            // Save data		
            pos.x -= ImagePack.innerMargin;
            pos.y += 1.4f * ImagePack.fieldHeight;
            pos.width /= 3;
            showSave = true;
            if (!newEntity)
            {
                showDelete = true;
            }
            else
            {
                showDelete = false;
            }
            showCancel = true;
            if (resultTimeout != -1 && resultTimeout < Time.realtimeSinceStartup && result != "")
            {
                result = "";
            }

            if (!newEntity)
                EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight + 140);
            else
                EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight);
        }

        public override void save()
        {
            if (newEntity)
                InsertEntry();
            else
                UpdateEntry();

            state = State.Loaded;
            resetSearch(true);
        }

        public override void delete()
        {
            if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete")+" " + Lang.GetTranslate(functionName) + " "+ Lang.GetTranslate("entry")+" ?", Lang.GetTranslate("Are you sure you want to delete")+" " + editingDisplay.Name + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
            {
                DeleteEntry();
                newSelectedDisplay = 0;
                state = State.Loaded;
            }
            resetSearch(true);
        }

        public override void cancel()
        {
            resetSearch(true);
            if (selectedDisplay > -1)
                editingDisplay = LoadEntity(displayKeys[selectedDisplay]);
            else
                editingDisplay = LoadEntity(selectedDisplay);
            if (newEntity)
                state = State.New;
            else
                state = State.Loaded;
        }

        // Insert new entries into the table
        void InsertEntry()
        {
            NewResult(Lang.GetTranslate("Inserting..."));
            // Setup the update query
            string query = "INSERT INTO " + tableName;
            query += " (" + editingDisplay.FieldList("", ", ") + ") ";
            query += "VALUES ";
            query += " (" + editingDisplay.FieldList("?", ", ") + ") ";

            int mobID = -1;

            // Setup the register data		
            List<Register> update = new List<Register>();
            foreach (string field in editingDisplay.fields.Keys)
            {
                update.Add(editingDisplay.fieldToRegister(field));
            }

            // Update the database
            mobID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);

            // If the insert failed, don't insert the spawn marker
            if (mobID != -1)
            {
                // Update online table to avoid access the database again			
                editingDisplay.id = mobID;
                editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + mobID + "ID2:" + editingDisplay.id);
                CreatePrefab();
                dataLoaded = false;
                Load();
                newItemCreated = true;
                // Configure the correponding prefab
                NewResult(Lang.GetTranslate("New entry inserted"));
            }
            else
            {
                NewResult(Lang.GetTranslate("Error occurred, please check the Console"));
            }
        }

        // Update existing entries in the table based on the iddemo_table
        void UpdateEntry()
        {
            NewResult(Lang.GetTranslate("Updating..."));
            // Setup the update query
            string query = "UPDATE " + tableName;
            query += " SET ";
            query += editingDisplay.UpdateList();
            query += " WHERE id=?id";

            // Setup the register data		
            List<Register> update = new List<Register>();
            foreach (string field in editingDisplay.fields.Keys)
            {
                update.Add(editingDisplay.fieldToRegister(field));
            }
            update.Add(new Register("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString(), Register.TypesOfField.Int));

            // Update the database
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);
  
            // Configure the correponding prefab
            CreatePrefab();
            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry")+"  " + editingDisplay.Name + " "+ Lang.GetTranslate("updated"));
           Load();
         }

        // Delete entries from the table
        void DeleteEntry()
        {
            // Remove the prefab
            DeletePrefab();

            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, tableName, delete);
            string query = "UPDATE " + tableName + " SET isactive = 0 where id = " + editingDisplay.id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            selectedDisplay = -1;
            newSelectedDisplay = 0;
            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " "+ Lang.GetTranslate("Deleted"));
  Load();
          
        }


        void DeleteForever(int id)
        {
            if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete") + " " + Lang.GetTranslate(functionName) + " " + Lang.GetTranslate("entry") + " ?", Lang.GetTranslate("Are you sure you want to permanent delete") + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
            {
                NewResult(Lang.GetTranslate("Deleting..."));
                List<Register> where = new List<Register>();
                where.Add(new Register("id", "?id", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, tableName, where);

                dataLoaded = false;
                dataRestoreLoaded = false;
                // Load();
                NewResult(Lang.GetTranslate("Entry Permanent Deleted"));
            }
        }

        void RestoreEntry(int id)
        {
            NewResult(Lang.GetTranslate("Restoring..."));
            string query = "UPDATE " + tableName + " SET isactive = 1 where id = " + id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            editingDisplay = LoadEntity(id);
            CreatePrefab();

            dataLoaded = false;
            dataRestoreLoaded = false;
            //Load();
            NewResult(Lang.GetTranslate("Entry Restored"));
        }

        void Duplicate()
        {
            // Update name of current entry by adding "1" to the end
            editingDisplay.Name = editingDisplay.Name + " (Clone)";

            InsertEntry();
            state = State.Loaded;
        }

    /*    void GenerateAllPrefabs()
        {
            if (!dataLoaded)
            {
                Load();
            }

            // Delete all existing prefabs?
            CraftingRecipePrefab.DeleteAllPrefabs();

            foreach (CraftingRecipe recipeData in dataRegister.Values)
            {
                prefab = new CraftingRecipePrefab(recipeData);
                prefab.Save(recipeData);
            }

        }
        */

        void GenerateAllPrefabs()
        {
         /*   if (!dataLoaded)
            {
                Load();
            }

            // Delete all existing prefabs?
            //  ItemPrefab.DeleteAllPrefabs();

            foreach (int id in displayKeys)
            {
                CraftingRecipe recipeData = LoadEntity(id);
                prefab = new CraftingRecipePrefab(recipeData);
                prefab.Save(recipeData);
            }
            CraftingRecipePrefab.DeletePrefabWithoutIDs(displayKeys);*/
        }

        void CreatePrefab()
        {
            // Configure the correponding prefab
           // prefab = new CraftingRecipePrefab(editingDisplay);
           // prefab.Save(editingDisplay);
        }

        void DeletePrefab()
        {
         /*   prefab = new CraftingRecipePrefab(editingDisplay);

            if (prefab.Load())
                prefab.Delete();*/
        }
    }
}