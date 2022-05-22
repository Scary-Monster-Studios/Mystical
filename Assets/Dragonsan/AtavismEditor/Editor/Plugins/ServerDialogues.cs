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
    // Handles the Mob Configuration
    public class ServerDialogues : AtavismDatabaseFunction
    {

        public new Dictionary<int, DialogueData> dataRegister;
        public new DialogueData editingDisplay;
        public new DialogueData originalDisplay;

        int requestedDisplay = -1;

        public static int[] dialogueIds = new int[] { -1 };
        public static string[] dialogueList = new string[] { "~ none ~" };
        public static GUIContent[] GuiDialogueList = new GUIContent[] { new GUIContent("~ none ~") };

        public int[] questIds = new int[] { -1 };
        public string[] questList = new string[] { "~ none ~" };

        public int[] factionIds = new int[] { -1 };
        public string[] factionList = new string[] { "~ none ~" };

        public int[] abilityIds = new int[] { -1 };
        public string[] abilityOptions = new string[] { "~ none ~" };

        public string[] actionOptions = new string[] { "~ none ~" };

        public int[] itemIds = new int[] { -1 };
        public string[] itemsList = new string[] { "~ none ~" };

        Vector2 descriptionScroll = new Vector2();

        private string[] itemReqSearchInput = new string[] { "", "", "", "", "", "", "", "" };
        string questSearch = "";

        // Use this for initialization
        public ServerDialogues()
        {
            functionName = "Dialogue";
            // Database tables name
            tableName = "dialogue";
            functionTitle = "Dialogue Configuration";
            loadButtonLabel = "Load Dialogues";
            notLoadedText = "No Dialogue loaded.";
            // Init
            dataRegister = new Dictionary<int, DialogueData>();

            editingDisplay = new DialogueData();
            originalDisplay = new DialogueData();
            questSearch = "";
        }
        void resetSearch(bool fokus)
        {
            questSearch = "";
            itemReqSearchInput = new string[] { "", "", "", "", "", "", "", "" };
           if(fokus) GUI.FocusControl(null);
        }

        public override void Activate()
        {
            linkedTablesLoaded = false;
            resetSearch(true);
        }

        public static void LoadDialogueList()
        {
            string query = "SELECT id, name FROM dialogue where isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                dialogueList = new string[rows.Count + 1];
                dialogueList[optionsId] = "~ none ~";
                dialogueIds = new int[rows.Count + 1];
                dialogueIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    dialogueList[optionsId] = data["id"] + ":" + data["name"];
                    dialogueIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        public static void LoadDialogueList(bool gui)
        {
            if (!gui)
            {
                LoadDialogueList();
                return;
            }
            string query = "SELECT id, name FROM dialogue where isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                GuiDialogueList = new GUIContent[rows.Count + 1];
                GuiDialogueList[optionsId] = new GUIContent("~ none ~");
                dialogueIds = new int[rows.Count + 1];
                dialogueIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    GuiDialogueList[optionsId] = new GUIContent(data["id"] + ":" + data["name"]);
                    dialogueIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        public static void LoadOpeningDialogueList(bool gui)
        {
            if (!gui)
            {
                LoadDialogueList();
                return;
            }
            string query = "SELECT id, name FROM dialogue where openingDialogue = 1 and isactive = 1 ";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                GuiDialogueList = new GUIContent[rows.Count + 1];
                GuiDialogueList[optionsId] = new GUIContent("~ none ~");
                dialogueIds = new int[rows.Count + 1];
                dialogueIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    GuiDialogueList[optionsId] = new GUIContent(data["id"] + ":" + data["name"]);
                    dialogueIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }


        private void LoadQuestList()
        {
            string query = "SELECT id, name FROM quests where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                questList = new string[rows.Count + 1];
                questList[optionsId] = "~ none ~";
                questIds = new int[rows.Count + 1];
                questIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    questList[optionsId] = data["id"] + ":" + data["name"];
                    questIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        private void LoadFactionList()
        {
            string query = "SELECT id, name FROM factions where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                factionList = new string[rows.Count + 1];
                factionList[optionsId] = "~ none ~";
                factionIds = new int[rows.Count + 1];
                factionIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    factionList[optionsId] = data["id"] + ":" + data["name"];
                    factionIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        public void LoadAbilityOptions()
        {
            string query = "SELECT id, name FROM abilities where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
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
            }
        }

        DialogueData LoadEntity(int id)
        {

            // Read all entries from the table
            string query = "SELECT " + originalDisplay.GetFieldsString() + " FROM " + tableName + " where isactive = 1 AND id ="+id;

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            DialogueData display = new DialogueData();
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    //foreach(string key in data.Keys)
                    //	Debug.Log("Name[" + key + "]:" + data[key]);
                    //return;
                    display.id = int.Parse(data["id"]);
                    display.Name = data["name"];
                    display.openingDialogue = bool.Parse(data["openingDialogue"]);
                    display.repeatable = bool.Parse(data["repeatable"]);
                    display.prereqDialogue = int.Parse(data["prereqDialogue"]);
                    display.prereqQuest = int.Parse(data["prereqQuest"]);
                    display.prereqFaction = int.Parse(data["prereqFaction"]);
                    display.prereqFactionStance = int.Parse(data["prereqFactionStance"]);
                    display.reactionAutoStart = bool.Parse(data["reactionAutoStart"]);
                    display.text = data["text"];
                    for (int i = 1; i <= display.maxEntries; i++)
                    {
                        string text = data["option" + i + "text"];
                        if (text != null && text.Length != 0)
                        {
                            string action = data["option" + i + "action"];
                            int actionID = int.Parse(data["option" + i + "actionID"]);
                            int itemReq = int.Parse(data["option" + i + "itemReq"]);
                            DialogueActionEntry entry = new DialogueActionEntry(text, action, actionID, itemReq);
                            display.entries.Add(entry);
                        }
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create a Dialogue before editing it."));
                return;
            }


            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Dialogue"));

            if (newItemCreated)
            {
                newItemCreated = false;
                 newSelectedDisplay = displayKeys.Count - 1;
            }

            // Draw data Editor
             /*   if (newSelectedDisplay != selectedDisplay)
                 {
                     selectedDisplay = newSelectedDisplay;
                     if (displayKeys.Count <= selectedDisplay)
                         selectedDisplay = 0;
                     int displayKey = displayKeys[selectedDisplay];
                     editingDisplay = dataRegister[displayKey];
                     originalDisplay = editingDisplay.Clone();
                     requestedDisplay = selectedDisplay;
                     resetSearch(true);
                 }*/
            if (newSelectedDisplay != selectedDisplay)
            {
                int prevId = -1;
                if (requestedDisplay== newSelectedDisplay)
                    prevId = editingDisplay.id;

                    selectedDisplay = newSelectedDisplay;
               
                int displayKey = -1;
                if (selectedDisplay >= 0)
                    displayKey = displayKeys[selectedDisplay];
              
                editingDisplay = LoadEntity(displayKey);
                editingDisplay.previousDialogueID = prevId;
                requestedDisplay = selectedDisplay;
                resetSearch(false);
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
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Duplicate")))
                {
                    Duplicate();
                }

                pos.width *= 2;
                pos.y += ImagePack.fieldHeight * 1.5f;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Dialogue Properties")+":");
                pos.y += ImagePack.fieldHeight * 0.75f;
            }

            DrawEditor(pos, false);

            pos.y -= ImagePack.fieldHeight;
            //pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;

            if (requestedDisplay != -1 && requestedDisplay != selectedDisplay)
                newSelectedDisplay = requestedDisplay;
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
            editingDisplay = new DialogueData();
           // originalDisplay = new DialogueData();
            selectedDisplay = -1;
        }
        DialogueData dd = null;
        // Edit or Create
        public override void DrawEditor(Rect box, bool nItem)
        {
            newEntity = nItem;
            if (!linkedTablesLoaded)
            {
                LoadDialogueList();
                LoadQuestList();
                LoadFactionList();
                LoadAbilityOptions();
                LoadItemList();
                actionOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Dialogue Action", false);
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new Dialogue"));
                pos.y += ImagePack.fieldHeight;
            }

            // Draw a button to go back to the previous dialogue
            if (editingDisplay.previousDialogueID != -1)
            {
                if (dd==null || dd.id != editingDisplay.previousDialogueID)
                    dd = LoadEntity(editingDisplay.previousDialogueID);
                pos.width = pos.width * 2;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Go back to")+" " + dd.id + "." + dd.Name))
                {
                    //editingDisplay = GetDialogue(editingDisplay.previousDialogueID);
                    requestedDisplay = GetPositionOfPreviousDialogue(editingDisplay.previousDialogueID);
                    state = State.Edit;
                    return;
                }
                pos.y += ImagePack.fieldHeight;
                pos.width = pos.width / 2;
            }

            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.5f);
            pos.y += ImagePack.fieldHeight;
            pos.width = pos.width / 2;
            editingDisplay.openingDialogue = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Opening Dialogue")+":", editingDisplay.openingDialogue);
            pos.x += pos.width;
            editingDisplay.repeatable = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Repeatable")+":", editingDisplay.repeatable);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            int dialogueID = GetPositionOfDialogue(editingDisplay.prereqDialogue);
            dialogueID = ImagePack.DrawSelector(pos, Lang.GetTranslate("Prereq Dialogue")+":", dialogueID, dialogueList);
            editingDisplay.prereqDialogue = dialogueIds[dialogueID];
            //pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
        //    Debug.LogWarning("Prereq przed " + editingDisplay.prereqQuest);
            int questID = GetOptionPosition(editingDisplay.prereqQuest, questIds);
            questID = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Prereq Quest")+":", ref questSearch, questID, questList);
         //   Debug.LogWarning("Prereq po " + questID);
            editingDisplay.prereqQuest = questIds[questID];
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            int factionID = GetPositionOfFaction(editingDisplay.prereqFaction);
            factionID = ImagePack.DrawSelector(pos, Lang.GetTranslate("Prereq Faction")+":", factionID, factionList);
            editingDisplay.prereqFaction = factionIds[factionID];
            pos.x += pos.width;
            int factionStance = GetPositionOfStance(editingDisplay.prereqFactionStance);
            factionStance = ImagePack.DrawSelector(pos, Lang.GetTranslate("Prereq Stance")+":", factionStance, FactionData.stanceOptions);
            editingDisplay.prereqFactionStance = FactionData.stanceValues[factionStance];
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            pos.width = pos.width * 2;
            GUI.Label(pos, Lang.GetTranslate("Text")+":", ImagePack.FieldStyle());
            pos.height *= 3;
            descriptionScroll = GUI.BeginScrollView(pos, descriptionScroll, new Rect(0, 0, pos.width * 0.75f, 150));
            editingDisplay.text = EditorGUI.TextArea(new Rect(115, 0, pos.width * 0.75f, 150), editingDisplay.text, ImagePack.TextAreaStyle());
            //editingDisplay.text = ImagePack.DrawField (pos, "Text:", editingDisplay.text, 0.75f, 60);
            GUI.EndScrollView();
            pos.height /= 3;
            pos.y += 3f * ImagePack.fieldHeight;

            // Only show actions if it has been saved due to complications of moving forward/back
            if (!newEntity)
            {
                if (editingDisplay.entries.Count == 0)
                {
                    editingDisplay.entries.Add(new DialogueActionEntry("", actionOptions[0], -1, -1));
                }
                for (int i = 0; i < editingDisplay.maxEntries; i++)
                {
                    if (editingDisplay.entries.Count > i)
                    {
                        ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Action")+" " + (i + 1));
                        pos.y += ImagePack.fieldHeight;
                        editingDisplay.entries[i].text = ImagePack.DrawField(pos, Lang.GetTranslate("Text")+":", editingDisplay.entries[i].text, 1.4f);
                        pos.width = pos.width / 2;
                        pos.y += ImagePack.fieldHeight;
                        string previousAction = editingDisplay.entries[i].action;
                        editingDisplay.entries[i].action = ImagePack.DrawSelector(pos, Lang.GetTranslate("Action Type")+":", editingDisplay.entries[i].action, actionOptions);
                        if (previousAction != editingDisplay.entries[i].action)
                        {
                            editingDisplay.entries[i].actionID = -1;
                        }
                        pos.x += pos.width;
                        if (editingDisplay.entries[i].action == "Dialogue")
                        {
                            // Treat dialogues differently as the developer will be able to click to move onto the next one
                            if (editingDisplay.entries[i].actionID == -1)
                            {
                                // No dialogue yet, click button to create a new one
                                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Create Dialogue")))
                                {
                                    if (string.IsNullOrEmpty(editingDisplay.entries[i].text))
                                    {
                                        NewResult("Error: "+ Lang.GetTranslate("Set Action Text before Create Dialogue"));
                                    }
                                    else
                                    {
                                        // Save dialogue first then lets go create a new one
                                        UpdateEntry();
                                        int previousDialogueID = editingDisplay.id;
                                        string dialogname = editingDisplay.entries[i].text;
                                        CreateNewData();
                                        editingDisplay.Name = dialogname;
                                        editingDisplay.previousDialogueID = previousDialogueID;
                                        editingDisplay.previousActionPosition = i;
                                        editingDisplay.openingDialogue = false;
                                        this.state = State.New;
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                // Show dialogue with option to move forward
                                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Edit Dialogue")))
                                {
                                    // Save dialogue first then lets go create a new one
                                    UpdateEntry();
                                   // dataRegister[editingDisplay.entries[i].actionID].previousDialogueID = editingDisplay.id;
                                    requestedDisplay = GetPositionOfDialogue(editingDisplay.entries[i].actionID) - 1;
                                    state = State.Edit;
                                }
                            }
                        }
                        else if (editingDisplay.entries[i].action == "Ability")
                        {
                            int selectedAbility = GetOptionPosition(editingDisplay.entries[i].actionID, abilityIds);
                            selectedAbility = ImagePack.DrawSelector(pos, Lang.GetTranslate("Ability")+":", selectedAbility, abilityOptions);
                            editingDisplay.entries[i].actionID = abilityIds[selectedAbility];
                        }
                        else if (editingDisplay.entries[i].action == "Quest")
                        {
                            int selectedQuest = GetOptionPosition(editingDisplay.entries[i].actionID, questIds);
                            selectedQuest = ImagePack.DrawSelector(pos, Lang.GetTranslate("Quest")+":", selectedQuest, questList);
                            editingDisplay.entries[i].actionID = questIds[selectedQuest];
                        }
                        else if (editingDisplay.entries[i].action == "Repair")
                        {
                            editingDisplay.entries[i].actionID = 1;
                        }
                        pos.x -= pos.width;
                        pos.y += ImagePack.fieldHeight;
                        int selectedItem = GetOptionPosition(editingDisplay.entries[i].itemReq, itemIds);
                        selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Item Req")+": ", ref itemReqSearchInput[i], selectedItem, itemsList);
                        editingDisplay.entries[i].itemReq = itemIds[selectedItem];
                        pos.y += ImagePack.fieldHeight;
                          pos.x += pos.width;
                        if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove Action")))
                        {
                            editingDisplay.entries.Remove(editingDisplay.entries[i]);
                        }
                            pos.x -= pos.width;
                        pos.y += ImagePack.fieldHeight;
                        pos.width = pos.width * 2;

                    }


                }
                if (editingDisplay.entries.Count < editingDisplay.maxEntries)
                {
                    if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Action")))
                    {
                        DialogueActionEntry actionEntry = new DialogueActionEntry("", actionOptions[0], -1, -1);
                        editingDisplay.entries.Add(actionEntry);
                    }
                }
            }
            //pos.width = pos.width * 2;

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

            // If the insert succeeded - update other fields
            if (mobID != -1)
            {
                // Update online table to avoid access the database again			
                editingDisplay.id = mobID;
                editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + mobID + "ID2:" + editingDisplay.id);
             //   dataRegister.Add(editingDisplay.id, editingDisplay);
              //  displayKeys.Add(editingDisplay.id);
                newItemCreated = true;
                // If this has a previous id - update the actionID field for that dialogue
                if (editingDisplay.previousDialogueID != -1)
                {
                    int tempSelectedDisplay = selectedDisplay;
                    selectedDisplay = GetPositionOfPreviousDialogue(editingDisplay.previousDialogueID);
                    int actionPos = editingDisplay.previousActionPosition;

                    editingDisplay = LoadEntity(editingDisplay.previousDialogueID);
                    //dataRegister[editingDisplay.previousDialogueID];
                //    Debug.LogError("InsertEntry actionPos:" + actionPos + " Count:" + editingDisplay.entries.Count);
                    editingDisplay.entries[actionPos].actionID = mobID;
                    UpdateEntry();
                  //  editingDisplay = originalDisplay;
                    selectedDisplay = tempSelectedDisplay;
                }
                LoadDialogueList();
                NewResult(Lang.GetTranslate("New entry inserted"));
            }
            else
            {
                NewResult(Lang.GetTranslate("Error: Error occurred, please check the Console"));
            }
            dataLoaded = false;
            Load();

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

            // Update online table to avoid access the database again			
            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry")+"  " + editingDisplay.Name + " "+ Lang.GetTranslate("updated"));
             Load();
       }

        // Delete entries from the table
        void DeleteEntry()
        {
            //Register delete = new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int);
            //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, tableName, delete);
            string query = "UPDATE " + tableName + " SET isactive = 0 where id = " + editingDisplay.id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            // Update any entries that have this as its next step
            query = "UPDATE " + tableName + " SET option1text = '', option1action = '', option1actionID = 0 where"
                + " option1action = 'Dialogue' AND option1actionID = " + editingDisplay.id;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            // Update online table to avoid access the database again		
            selectedDisplay = -1;
            newSelectedDisplay = 0;
            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " "+ Lang.GetTranslate("Deleted"));
             Load();
           LoadDialogueList();
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

        private int GetPositionOfDialogue(int dialogueID)
        {
            for (int i = 0; i < dialogueIds.Length; i++)
            {
                if (dialogueIds[i] == dialogueID)
                    return i;
            }
            return 0;
        }

        private int GetPositionOfFaction(int factionID)
        {
            for (int i = 0; i < factionIds.Length; i++)
            {
                if (factionIds[i] == factionID)
                    return i;
            }
            return 0;
        }

        private int GetPositionOfStance(int stanceValue)
        {
            for (int i = 0; i < FactionData.stanceValues.Length; i++)
            {
                if (FactionData.stanceValues[i] == stanceValue)
                    return i;
            }
            return 0;
        }

        private int GetPositionOfPreviousDialogue(int dialogueID)
        {
            for (int i = 0; i < displayKeys.Count; i++)
            {
                if (displayKeys[i] == dialogueID)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}