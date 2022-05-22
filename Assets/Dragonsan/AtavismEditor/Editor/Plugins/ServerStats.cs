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
    public class ServerStats : AtavismDatabaseFunction
    {

        public new Dictionary<int, StatsData> dataRegister;
        public new StatsData editingDisplay;
        public new StatsData originalDisplay;

        public string[] statOptions = new string[] { "~ none ~" };
        public string[] statOptions2 = new string[] { "~ none ~" };
        public string[] statFunctionOptions = new string[] { "~ none ~" };
        public string[] statShiftActionOptions = new string[] { "~ none ~" };
        public string[] statShiftStateOptions = new string[] { "~ none ~" };

        public int[] effectIds = new int[] { -1 };
        public string[] effectOptions = new string[] { "~ none ~" };
        string reqEffectSearch = "";
        string reqEffectSearch1 = "";
        string reqEffectSearch2 = "";
        string reqEffectSearch3 = "";
        string reqEffectSearch4 = "";
        string reqEffectSearch5 = "";
        string reqEffectSearch6 = "";
        // Use this for initialization
        public ServerStats()
        {
            functionName = "Stats";
            // Database tables name
            tableName = "stat";
            functionTitle = "Stats Configuration";
            loadButtonLabel = "Load Stats";
            notLoadedText = "No Stats loaded.";
            // Init
            dataRegister = new Dictionary<int, StatsData>();

            editingDisplay = new StatsData();
            originalDisplay = new StatsData();
        }
        void resetSearch(bool fokus)
        {
            reqEffectSearch = "";
            reqEffectSearch1 = "";
            reqEffectSearch2 = "";
            reqEffectSearch3 = "";
            reqEffectSearch4 = "";
            reqEffectSearch5 = "";
            reqEffectSearch6 = "";
            if (fokus) GUI.FocusControl(null);
        }
        public override void Activate()
        {
            linkedTablesLoaded = false;
            resetSearch(true);
        }

        public void LoadStatOptions()
        {
          
                // Read all entries from the table
                string query = "SELECT name FROM stat where isactive = 1";

                // If there is a row, clear it.
                if (rows != null)
                    rows.Clear();

                // Load data
                rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
                //Debug.Log("#Rows:"+rows.Count);
                // Read all the data
                int optionsId = 0;
                if ((rows != null) && (rows.Count > 0))
                {
                    statOptions = new string[rows.Count];
                    statOptions2 = new string[rows.Count+1];
                    statOptions2[0] ="~ none ~";
                foreach (Dictionary<string, string> data in rows)
                    {
                    statOptions[optionsId] = data["name"];
                    statOptions2[optionsId+1] = data["name"];
                    optionsId++;
                    }
                }
           
        }

        public static GUIContent[] LoadStatOptionsForGui()
        {

            GUIContent[] options = new GUIContent[1];
            // Read all entries from the table
            string query = "SELECT name FROM stat where isactive = 1";

            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                options = new GUIContent[rows.Count ];
                foreach (Dictionary<string, string> data in rows)
                {
                    options[optionsId] = new GUIContent(data["name"]);
                    optionsId++;
                }
            }
            return options;
        }
        public void LoadEffectOptions()
        {
           
                // Read all entries from the table
                string query = "SELECT id, name FROM effects where isactive = 1";

                // If there is a row, clear it.
                if (rows != null)
                    rows.Clear();

                // Load data
                rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
                //Debug.Log("#Rows:"+rows.Count);
                // Read all the data
                int optionsId = 0;
                if ((rows != null) && (rows.Count > 0))
                {
                    effectOptions = new string[rows.Count + 1];
                    effectOptions[optionsId] = "~ none ~";
                    effectIds = new int[rows.Count + 1];
                    effectIds[optionsId] = 0;
                    foreach (Dictionary<string, string> data in rows)
                    {
                        optionsId++;
                        effectOptions[optionsId] = data["name"];
                        effectIds[optionsId] = int.Parse(data["id"]);
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
                string query = "SELECT name FROM " + tableName + " where isactive = 1";

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
                        displayList.Add( data["name"]);
                      //  displayKeys.Add(int.Parse(data["id"]));

                    }
                }
                dataLoaded = true;
            }
        }

        StatsData LoadEntity(string id)
        {
            // Read all entries from the table
            string query = "SELECT " + originalDisplay.GetFieldsString() + " FROM " + tableName + " where isactive = 1 AND name like '"+id+"'";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            StatsData display = new StatsData();
            int fakeId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    //foreach(string key in data.Keys)
                    //	Debug.Log("Name[" + key + "]:" + data[key]);
                    //return;
                    // As we don have a primary key ID field
                    fakeId++;
                    display.id = fakeId;
                    display.Name = data["name"];
                    display.originalName = display.Name;
                    display.type = int.Parse(data["type"]);
                    display.statFunction = data["stat_function"];
                    display.mobBase = int.Parse(data["mob_base"]);
                    display.mobLevelIncrease = int.Parse(data["mob_level_increase"]);
                    display.mobLevelPercentIncrease = float.Parse(data["mob_level_percent_increase"]);
                    display.min = int.Parse(data["min"]);
                    display.maxstat = data["maxstat"];
                    display.canExceedMax = bool.Parse(data["canExceedMax"]);
                    display.sharedWithGroup = bool.Parse(data["sharedWithGroup"]);
                    byte shiftPlayerOnly = byte.Parse(data["shiftTarget"]);
                    display.shiftTarget = (int)shiftPlayerOnly;
                    display.shiftValue = int.Parse(data["shiftValue"]);
                    display.shiftReverseValue = int.Parse(data["shiftReverseValue"]);
                    display.shiftInterval = int.Parse(data["shiftInterval"]);
                    display.isShiftPercent = bool.Parse(data["isShiftPercent"]);
                    display.onMaxHit = data["onMaxHit"];
                    display.onMinHit = data["onMinHit"];
                    display.shiftReq1 = data["shiftReq1"];
                    display.shiftReq1State = bool.Parse(data["shiftReq1State"]);
                    display.shiftReq1SetReverse = bool.Parse(data["shiftReq1SetReverse"]);
                    display.shiftReq2 = data["shiftReq2"];
                    display.shiftReq2State = bool.Parse(data["shiftReq2State"]);
                    display.shiftReq2SetReverse = bool.Parse(data["shiftReq2SetReverse"]);
                    display.shiftReq3 = data["shiftReq3"];
                    display.shiftReq3State = bool.Parse(data["shiftReq3State"]);
                    display.shiftReq3SetReverse = bool.Parse(data["shiftReq3SetReverse"]);
                    display.startPercent = int.Parse(data["startPercent"]);
                    display.deathResetPercent = int.Parse(data["deathResetPercent"]);
                    display.releaseResetPercent = int.Parse(data["releaseResetPercent"]);
                    display.onThresholdHit = data["onThreshold"];
                    display.onThreshold2Hit = data["onThreshold2"];
                    display.onThreshold3Hit = data["onThreshold3"];
                    display.onThreshold4Hit = data["onThreshold4"];
                    display.onThreshold5Hit = data["onThreshold5"];
                    display.threshold = float.Parse(data["threshold"]);
                    display.threshold2 = float.Parse(data["threshold2"]);
                    display.threshold3 = float.Parse(data["threshold3"]);
                    display.threshold4 = float.Parse(data["threshold4"]);
                    display.shiftModStat = data["shiftModStat"];
                   

                    display.isLoaded = true;
                    //Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
                }
            }
            LoadStatLinks(display);
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
                string query = "SELECT name FROM " + tableName + " where isactive = 0";

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
                        displayList.Add( data["name"]);
                      //  displayKeys.Add(int.Parse(data["id"]));

                    }
                }
                dataRestoreLoaded = true;
            }
        }

        void LoadStatLinks(StatsData statData)
        {
            // Read all entries from the table
            string query = "SELECT " + new StatLinkEntry().GetFieldsString() + " FROM stat_link where stat = '" + statData.Name + "' AND isactive = 1";
            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    StatLinkEntry display = new StatLinkEntry();
                    display.id = int.Parse(data["id"]);
                    display.statTo = data["statTo"];
                    display.changePerPoint = int.Parse(data["changePerPoint"]);
                    statData.statLinks.Add(display);
                }
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create an Stat before editing it."));
                return;
            }

            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Stat"));

            if (newItemCreated)
            {
                newItemCreated = false;
                newSelectedDisplay = displayKeys.Count - 1;
            }

            // Draw data Editor
            if (newSelectedDisplay != selectedDisplay)
            {
                selectedDisplay = newSelectedDisplay;
                editingDisplay = LoadEntity(displayList[selectedDisplay]);
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

                pos.width *= 2;
                pos.y += ImagePack.fieldHeight * 1.5f;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Stat Properties")+":");
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
                    DeleteForever(displayList[i]);
                }
                pos.x += 155;
                if (ImagePack.DrawButton(pos, Lang.GetTranslate("Restore")))
                {
                    RestoreEntry(displayList[i]);
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
            editingDisplay = new StatsData();
            originalDisplay = new StatsData();
            selectedDisplay = -1;
        }

        // Edit or Create
        public override void DrawEditor(Rect box, bool nItem)
        {
            newEntity = nItem;
            // Setup the layout
            Rect pos = box;
            pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;

            if (!linkedTablesLoaded)
            {
                LoadStatOptions();
                LoadEffectOptions();
                statFunctionOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Stat Functions", true);
                statShiftActionOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Stat Shift Action", true);
                string[] stateOptions1 = ServerOptionChoices.LoadAtavismChoiceOptions("Stat Shift Requirement", true);
                string[] stateOptions = ServerOptionChoices.LoadAtavismChoiceOptions("State", false);

                statShiftStateOptions = new string[stateOptions.Length + stateOptions1.Length];
                /*statShiftStateOptions[0] = "~ none ~";
                statShiftStateOptions[1] = "combatstate";
                statShiftStateOptions[2] = "deadstate";
                */
                for (int i = 0; i < stateOptions1.Length; i++)
                {
                    statShiftStateOptions[i] = stateOptions1[i];
                }
                for (int i = 0; i < stateOptions.Length; i++)
                {
                    statShiftStateOptions[i + stateOptions1.Length] = stateOptions[i];
                }
                linkedTablesLoaded = true;
            }

            // Draw the content database info
            //pos.y += ImagePack.fieldHeight;

            if (newEntity)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new Stat"));
                pos.y += ImagePack.fieldHeight;
            }

            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.75f);
            pos.y += ImagePack.fieldHeight;
            pos.width /= 2;
            editingDisplay.type = ImagePack.DrawSelector(pos, Lang.GetTranslate("Stat Type")+":", editingDisplay.type, editingDisplay.typeOptions);
            pos.x += pos.width;
            editingDisplay.statFunction = ImagePack.DrawSelector(pos, Lang.GetTranslate("Stat Function")+":", editingDisplay.statFunction, statFunctionOptions);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.sharedWithGroup = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Visible to Group")+"?", editingDisplay.sharedWithGroup);
            pos.y += ImagePack.fieldHeight * 1.5f;
            ImagePack.DrawLabel(pos, Lang.GetTranslate("Mob Values and Progression for this Stat")+":");
            pos.y += ImagePack.fieldHeight;
            editingDisplay.mobBase = ImagePack.DrawField(pos, Lang.GetTranslate("Base Value")+":", editingDisplay.mobBase);
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("Each level it..."));
            pos.y += ImagePack.fieldHeight;
            editingDisplay.mobLevelIncrease = ImagePack.DrawField(pos, Lang.GetTranslate("Increases by")+":", editingDisplay.mobLevelIncrease);
            pos.x += pos.width;
            editingDisplay.mobLevelPercentIncrease = ImagePack.DrawField(pos, Lang.GetTranslate("And Percentage")+":", editingDisplay.mobLevelPercentIncrease);
            pos.x -= pos.width;
            pos.width *= 2;

            if (editingDisplay.type == 2)
            {
                // Draw additional vitality stat fields
                pos.y += 2.5f * ImagePack.fieldHeight;
                ImagePack.DrawLabel(pos, Lang.GetTranslate("Vitality Stat Settings")+":");
                pos.y += ImagePack.fieldHeight;
                pos.width /= 2;

                editingDisplay.min = ImagePack.DrawField(pos, Lang.GetTranslate("Minimum")+":", editingDisplay.min);
                pos.x += pos.width;
                editingDisplay.maxstat = ImagePack.DrawSelector(pos, Lang.GetTranslate("Max Stat")+":", editingDisplay.maxstat, statOptions);
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                editingDisplay.canExceedMax = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Can Exceed Max")+"?", editingDisplay.canExceedMax);
                pos.y += ImagePack.fieldHeight;
                editingDisplay.shiftTarget = ImagePack.DrawSelector(pos, Lang.GetTranslate("Shift Target")+":", editingDisplay.shiftTarget, editingDisplay.targetOptions);
                pos.y += ImagePack.fieldHeight;
                editingDisplay.shiftValue = ImagePack.DrawField(pos, Lang.GetTranslate("Shift Value")+":", editingDisplay.shiftValue);
                pos.x += pos.width;
                editingDisplay.shiftReverseValue = ImagePack.DrawField(pos, Lang.GetTranslate("Reverse Value")+":", editingDisplay.shiftReverseValue);
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                editingDisplay.shiftInterval = ImagePack.DrawField(pos, Lang.GetTranslate("Shift Interval")+":", editingDisplay.shiftInterval);
                pos.x += pos.width;
                editingDisplay.isShiftPercent = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Is Shift Percentage")+"?", editingDisplay.isShiftPercent);
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                editingDisplay.shiftModStat = ImagePack.DrawSelector(pos, Lang.GetTranslate("Shift Mod Stat") + ":", editingDisplay.shiftModStat, statOptions2);
                pos.y += ImagePack.fieldHeight;

                if (editingDisplay.onMinHit.StartsWith("effect"))
                {
                    string[] vals = editingDisplay.onMinHit.Split(':');
                    editingDisplay.onMinHit = ImagePack.DrawSelector(pos, Lang.GetTranslate("On Min Hit")+":", vals[0], statShiftActionOptions);
                    if (editingDisplay.onMinHit.StartsWith("effect"))
                    {
                        pos.y += ImagePack.fieldHeight;
                        //pos.x += pos.width;
                        int effectID = -1;
                        if (vals.Length > 1)
                            effectID = int.Parse(vals[1]);
                        int selectedEffect = GetPositionOfEffect(effectID);
                        selectedEffect = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Effect")+":", ref reqEffectSearch, selectedEffect, effectOptions);
                        //  selectedEffect = ImagePack.DrawSelector (pos, "Effect:", selectedEffect, effectOptions);
                        effectID = effectIds[selectedEffect];
                        editingDisplay.onMinHit = "effect:" + effectID;
                        //	pos.x -= pos.width;
                    }
                }
                else
                {
                    editingDisplay.onMinHit = ImagePack.DrawSelector(pos, Lang.GetTranslate("On Min Hit")+":", editingDisplay.onMinHit, statShiftActionOptions);
                }
                pos.y += ImagePack.fieldHeight*1.5f;

                if (editingDisplay.threshold > 0)
                {

                    int effectID1 = -1;
                    string[] vals1 = editingDisplay.onThresholdHit.Split(':');
                    if (vals1.Length > 1)
                        effectID1 = int.Parse(vals1[1]);
                    int selectedEffect1 = GetPositionOfEffect(effectID1);
                    selectedEffect1 = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Effect ") + ":", ref reqEffectSearch2, selectedEffect1, effectOptions);
                    //   selectedEffect = ImagePack.DrawSelector (pos, "Effect:", selectedEffect, effectOptions);
                    effectID1 = effectIds[selectedEffect1];
                    editingDisplay.onThresholdHit = "effect:" + effectID1;
                    pos.y += ImagePack.fieldHeight;
                } editingDisplay.threshold = ImagePack.DrawField(pos, Lang.GetTranslate("Threshold (%)") + ":", editingDisplay.threshold);
                pos.y += ImagePack.fieldHeight;
                if (editingDisplay.threshold2 > 0 && editingDisplay.threshold2 <= editingDisplay.threshold)
                    editingDisplay.threshold2 = editingDisplay.threshold + 0.1f;
                if (editingDisplay.threshold2 > 0)
                {

                    int effectID2 = -1;
                    string[] vals2 = editingDisplay.onThreshold2Hit.Split(':');
                    if (vals2.Length > 1)
                        effectID2 = int.Parse(vals2[1]);
                    int selectedEffect2 = GetPositionOfEffect(effectID2);
                    selectedEffect2 = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Effect ") + ":", ref reqEffectSearch3, selectedEffect2, effectOptions);
                    //   selectedEffect = ImagePack.DrawSelector (pos, "Effect:", selectedEffect, effectOptions);
                    effectID2 = effectIds[selectedEffect2];
                    editingDisplay.onThreshold2Hit = "effect:" + effectID2;
                    pos.y += ImagePack.fieldHeight;
                }
                editingDisplay.threshold2 = ImagePack.DrawField(pos, Lang.GetTranslate("Threshold (%)") + ":", editingDisplay.threshold2);
                pos.y += ImagePack.fieldHeight;
                if (editingDisplay.threshold3 > 0 && editingDisplay.threshold3 <= editingDisplay.threshold)
                    editingDisplay.threshold3 = editingDisplay.threshold + 0.1f;
                if (editingDisplay.threshold3 > 0 && editingDisplay.threshold3 <= editingDisplay.threshold2)
                    editingDisplay.threshold3 = editingDisplay.threshold2 + 0.1f;

                if (editingDisplay.threshold3 > 0)
                {

                    int effectID3 = -1;
                    string[] vals3 = editingDisplay.onThreshold3Hit.Split(':');
                    if (vals3.Length > 1)
                        effectID3 = int.Parse(vals3[1]);
                    int selectedEffect3 = GetPositionOfEffect(effectID3);
                    selectedEffect3 = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Effect ") + ":", ref reqEffectSearch4, selectedEffect3, effectOptions);
                    //   selectedEffect = ImagePack.DrawSelector (pos, "Effect:", selectedEffect, effectOptions);
                    effectID3 = effectIds[selectedEffect3];
                    editingDisplay.onThreshold3Hit = "effect:" + effectID3;
                    pos.y += ImagePack.fieldHeight;
                }
                editingDisplay.threshold3 = ImagePack.DrawField(pos, Lang.GetTranslate("Threshold (%)") + ":", editingDisplay.threshold3);
                pos.y += ImagePack.fieldHeight;
                if (editingDisplay.threshold4 > 0 && editingDisplay.threshold4 <= editingDisplay.threshold)
                    editingDisplay.threshold4 = editingDisplay.threshold + 0.1f;
                if (editingDisplay.threshold4 > 0 && editingDisplay.threshold4 <= editingDisplay.threshold2)
                    editingDisplay.threshold4 = editingDisplay.threshold2 + 0.1f;
                if (editingDisplay.threshold4 > 0 && editingDisplay.threshold4 <= editingDisplay.threshold3)
                    editingDisplay.threshold4 = editingDisplay.threshold3 + 0.1f;
                if (editingDisplay.threshold4 > 0)
                {

                    int effectID4 = -1;
                    string[] vals4 = editingDisplay.onThreshold4Hit.Split(':');
                    if (vals4.Length > 1)
                        effectID4 = int.Parse(vals4[1]);
                    int selectedEffect4 = GetPositionOfEffect(effectID4);
                    selectedEffect4 = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Effect ") + ":", ref reqEffectSearch5, selectedEffect4, effectOptions);
                    //   selectedEffect = ImagePack.DrawSelector (pos, "Effect:", selectedEffect, effectOptions);
                    effectID4 = effectIds[selectedEffect4];
                    editingDisplay.onThreshold4Hit = "effect:" + effectID4;
                    pos.y += ImagePack.fieldHeight;
                }
                editingDisplay.threshold4 = ImagePack.DrawField(pos, Lang.GetTranslate("Threshold (%)") + ":", editingDisplay.threshold4);
                pos.y += ImagePack.fieldHeight;

                int effectID5 = -1;
                string[] vals5 = editingDisplay.onThreshold5Hit.Split(':');
                if (vals5.Length > 1)
                    effectID5 = int.Parse(vals5[1]);
                int selectedEffect5 = GetPositionOfEffect(effectID5);
                selectedEffect5 = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Effect ") + ":", ref reqEffectSearch6, selectedEffect5, effectOptions);
                //   selectedEffect = ImagePack.DrawSelector (pos, "Effect:", selectedEffect, effectOptions);
                effectID5 = effectIds[selectedEffect5];
                editingDisplay.onThreshold5Hit = "effect:" + effectID5;
                pos.y += ImagePack.fieldHeight*1.5f;




                if (editingDisplay.onMaxHit.StartsWith("effect"))
                {
                    string[] vals = editingDisplay.onMaxHit.Split(':');
                    editingDisplay.onMaxHit = ImagePack.DrawSelector(pos, Lang.GetTranslate("On max Hit")+":", vals[0], statShiftActionOptions);
                    if (editingDisplay.onMaxHit.StartsWith("effect"))
                    {
                        pos.y += ImagePack.fieldHeight;
                        // pos.x += pos.width;
                        int effectID = -1;
                        if (vals.Length > 1)
                            effectID = int.Parse(vals[1]);
                        int selectedEffect = GetPositionOfEffect(effectID);
                        selectedEffect = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Effect")+":", ref reqEffectSearch1, selectedEffect, effectOptions);
                        //   selectedEffect = ImagePack.DrawSelector (pos, "Effect:", selectedEffect, effectOptions);
                        effectID = effectIds[selectedEffect];
                        editingDisplay.onMaxHit = "effect:" + effectID;
                        //	pos.x -= pos.width;
                    }
                }
                else
                {
                    editingDisplay.onMaxHit = ImagePack.DrawSelector(pos, Lang.GetTranslate("On Max Hit")+":", editingDisplay.onMaxHit, statShiftActionOptions);
                }
                pos.y += ImagePack.fieldHeight;
                editingDisplay.startPercent = ImagePack.DrawField(pos, Lang.GetTranslate("Start") + " %:", editingDisplay.startPercent);
                pos.y += ImagePack.fieldHeight;
                editingDisplay.releaseResetPercent = ImagePack.DrawField(pos, Lang.GetTranslate("Release") + " %:", editingDisplay.releaseResetPercent);
                pos.y += ImagePack.fieldHeight;
                editingDisplay.shiftReq1 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Requirement") + " 1:", editingDisplay.shiftReq1, statShiftStateOptions);
                pos.y += ImagePack.fieldHeight;
                editingDisplay.shiftReq1State = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Req True") + "?", editingDisplay.shiftReq1State);
                pos.x += pos.width;
                editingDisplay.shiftReq1SetReverse = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Reverse if Fail") + "?", editingDisplay.shiftReq1SetReverse);
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                editingDisplay.shiftReq2 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Requirement") + " 2:", editingDisplay.shiftReq2, statShiftStateOptions);
                pos.y += ImagePack.fieldHeight;
                editingDisplay.shiftReq2State = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Req True") + "?", editingDisplay.shiftReq2State);
                pos.x += pos.width;
                editingDisplay.shiftReq2SetReverse = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Reverse if Fail") + "?", editingDisplay.shiftReq2SetReverse);
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                editingDisplay.shiftReq3 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Requirement") + " 3:", editingDisplay.shiftReq3, statShiftStateOptions);
                pos.y += ImagePack.fieldHeight;
                editingDisplay.shiftReq3State = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Req True") + "?", editingDisplay.shiftReq3State);
                pos.x += pos.width;
                editingDisplay.shiftReq3SetReverse = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Reverse if Fail") + "?", editingDisplay.shiftReq3SetReverse);
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;

                pos.width *= 2;
            }

            pos.width /= 2;

            // Stat Link area
            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Stat Links"));
            pos.y += 1.5f * ImagePack.fieldHeight;
            for (int i = 0; i < editingDisplay.statLinks.Count; i++)
            {
                editingDisplay.statLinks[i].statTo = ImagePack.DrawSelector(pos, Lang.GetTranslate("Stat to change"), editingDisplay.statLinks[i].statTo, statOptions);
                pos.x += pos.width;
                editingDisplay.statLinks[i].changePerPoint = ImagePack.DrawField(pos, Lang.GetTranslate("Amount")+":", editingDisplay.statLinks[i].changePerPoint);
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove Link")))
                {
                    if (editingDisplay.statLinks[i].id > 0)
                        editingDisplay.linksToBeDeleted.Add(editingDisplay.statLinks[i].id);
                    editingDisplay.statLinks.RemoveAt(i);
                }
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
            }
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Conversion")))
            {
                editingDisplay.statLinks.Add(new StatLinkEntry("", "", 1));
            }

            pos.width *= 2;

            pos.y += 2.5f * ImagePack.fieldHeight;
            // Save data
            pos.x -= ImagePack.innerMargin;
            pos.width /= 3;
            showSave = true;
            // Make sure there is a max stat if this is a vitality stat

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
                EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight + 100);
            else
                EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight);

        }

        public override void save()
        {
            if (editingDisplay.type == 2 && (editingDisplay.maxstat == "" || editingDisplay.maxstat == "~ none ~"))
            {
                NewResult("Error:" + Lang.GetTranslate("You must set a Max Stat before"));
            }
            else
            {
                if (newEntity)
                    InsertEntry();
                else
                    UpdateEntry();

                state = State.Loaded;
            }
            resetSearch(true);
        }

        public override void delete()
        {
            if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete")+" " + Lang.GetTranslate(functionName) + " " + Lang.GetTranslate("entry")+" ?", Lang.GetTranslate("Are you sure you want to delete")+" " + editingDisplay.Name + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
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
            editingDisplay = LoadEntity(displayList[selectedDisplay]);
           
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

            int itemID = -1;

            // Setup the register data		
            List<Register> update = new List<Register>();
            foreach (string field in editingDisplay.fields.Keys)
            {
                update.Add(editingDisplay.fieldToRegister(field));
            }

            // Update the database
            itemID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);

            // If the insert succeeded
            if (itemID != -1)
            {
                // Update online table to avoid access the database again			
                editingDisplay.id = dataRegister.Count + 1; // Set the highest free index ;
                                                            // Insert the Links
                foreach (StatLinkEntry entry in editingDisplay.statLinks)
                {
                    if (entry.statTo != "" && !entry.statTo.Contains("none"))
                    {
                        entry.stat = editingDisplay.Name;
                        InsertLink(entry);
                    }
                }
                editingDisplay.isLoaded = true;
                InsertNewCharacterStat(editingDisplay);
                //Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
                dataLoaded = false;
                Load();
                newItemCreated = true;
                LoadStatOptions();
                NewResult(Lang.GetTranslate("New entry inserted"));
            }
            else
            {
                NewResult("Error:" + Lang.GetTranslate("Error occurred, please check the Console"));
            }
        }

        /// <summary>
        /// Inserts the new stat into every existing character template entry
        /// </summary>
        /// <param name="statData">Stat data.</param>
        void InsertNewCharacterStat(StatsData statData)
        {
            //string characterStatsTable = "";
            //string query = "INSERT INTO " + characterStatsTable + " (character_create_id, stat, value, levelIncr;
            //DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, query);

            List<int> characterTemplateIDs = new List<int>();
            string query = "Select id from character_create_template where isactive = 1";
            try
            {
                // Open the connection
                DatabasePack.Connect(DatabasePack.contentDatabasePrefix);
                if (DatabasePack.con.State.ToString() != "Open")
                    DatabasePack.con.Open();
                // Use the connections to fetch data
                using (DatabasePack.con)
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, DatabasePack.con))
                    {
                        // Execute the query
                        MySqlDataReader data = cmd.ExecuteReader();
                        // If there are columns
                        if (data.HasRows)
                        {
                            int fieldsCount = data.FieldCount;
                            while (data.Read())
                            {
                                characterTemplateIDs.Add(data.GetInt32("id"));
                            }
                        }
                        data.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                NewResult("Error" + Lang.GetTranslate("Error occurred deleting old entries"));
            }
            finally
            {
            }

            foreach (int characterTemplateID in characterTemplateIDs)
            {
                CharStatsData charStat = new CharStatsData();
                charStat.charId = characterTemplateID;
                charStat.stat = statData.Name;
                charStat.statValue = statData.mobBase;
                charStat.levelIncrease = 0;
                charStat.levelPercentIncrease = 0;
                ServerCharacter.InsertStat(charStat);
            }
        }

        void InsertLink(StatLinkEntry entry)
        {
            string query = "INSERT INTO stat_link";
            query += " (" + entry.FieldList("", ", ") + ") ";
            query += "VALUES ";
            query += " (" + entry.FieldList("?", ", ") + ") ";

            // Setup the register data		
            List<Register> update = new List<Register>();
            foreach (string field in entry.fields.Keys)
            {
                update.Add(entry.fieldToRegister(field));
            }

            int itemID = -1;
            itemID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);

            entry.id = itemID;
        }

        // Update existing entries in the table based on the iddemo_table
        void UpdateEntry()
        {
            NewResult(Lang.GetTranslate("Updating..."));
            // Setup the update query
            string query = "UPDATE " + tableName;
            query += " SET ";
            query += editingDisplay.UpdateList();
            query += " WHERE name='" + editingDisplay.originalName + "'";

            // Setup the register data		
            List<Register> update = new List<Register>();
            foreach (string field in editingDisplay.fields.Keys)
            {
                update.Add(editingDisplay.fieldToRegister(field));
            }
            //update.Add (new Register ("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString (), Register.TypesOfField.Int));

            // Update the database
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);

            // Insert/Update the stat links
            foreach (StatLinkEntry entry in editingDisplay.statLinks)
            {
                if (entry.statTo != "" && !entry.statTo.Contains("none"))
                {
                    if (entry.id < 1)
                    {
                        // This is a new entry, insert it
                        entry.stat = editingDisplay.Name;
                        InsertLink(entry);
                    }
                    else
                    {
                        // This is an existing entry, update it
                        entry.stat = editingDisplay.Name;
                        UpdateLink(entry);
                    }
                }
            }

            // Delete any links that are tagged for deletion
            foreach (int linkID in editingDisplay.linksToBeDeleted)
            {
                DeleteLink(linkID);
            }

            // Update online table to avoid access the database again			
           

            // Need to update other stats that refer to this stat
            query = "UPDATE stat set maxStat = '" + editingDisplay.Name + "' where maxStat = '" + editingDisplay.originalName + "'";
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            // Need to update all entries in the character_create_stats if the statname changed
            query = "UPDATE character_create_stats set stat = '" + editingDisplay.Name + "' where stat = '" + editingDisplay.originalName + "'";
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            //TODO: update all items and effects that reference this stat
            dataLoaded = false;
         
            NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " " + Lang.GetTranslate("updated"));
          Load();
            LoadStatOptions();
 }

        void UpdateLink(StatLinkEntry entry)
        {
            string query = "UPDATE stat_link";
            query += " SET ";
            query += entry.UpdateList();
            query += " WHERE id=?id";

            // Setup the register data		
            List<Register> update = new List<Register>();
            foreach (string field in entry.fields.Keys)
            {
                update.Add(entry.fieldToRegister(field));
            }
            update.Add(new Register("id", "?id", MySqlDbType.Int32, entry.id.ToString(), Register.TypesOfField.Int));

            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);
        }

        void DeleteLink(int linkID)
        {
            string query = "UPDATE stat_link SET isactive = 0 where id = " + linkID;
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
        }

        // Delete entries from the table
        void DeleteEntry()
        {
            //Check Loot Table
            string sqlEffects = "SELECT id FROM `effects` WHERE effectMainType  like 'Stat' AND (";
            for (int ii = 1; ii <= 5; ii++)
            {
                sqlEffects += " stringValue" + ii + " like '" + editingDisplay.Name+"' ";
                if (ii < 5) sqlEffects += " OR ";
            }
            sqlEffects += ")";
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlEffects);
            int effectsCount = 0;
            string effectsIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                effectsIds = "\n" + Lang.GetTranslate("Effects Ids") + ":";
                effectsCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    effectsIds += " " + data["id"] + ",";
                }
            }

            string sqlQuestReq = "SELECT quest_id FROM `quest_requirement` WHERE editor_option_choice_type_id  like '" + editingDisplay.Name + "' ";
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlQuestReq);
            int questReqCount = 0;
            string questReqIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                questReqIds = "\n" + Lang.GetTranslate("Quest Ids") + ":";
                questReqCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    questReqIds += " " + data["quest_id"] + ",";
                }
            }

            DatabasePack.SaveLog("Delete Stats id =" + editingDisplay.id + " assigned to " + effectsIds + questReqIds);

            if (effectsCount == 0 && questReqCount ==0)
            {
                //Register delete = new Register ("name", "?name", MySqlDbType.String, editingDisplay.name, Register.TypesOfField.String);
                //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, tableName, delete, true);
                string query = "UPDATE " + tableName + " SET isactive = 0 where name = '" + editingDisplay.originalName + "'";
                DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

                // Delete from character stats
                //delete = new Register ("stat", "?stat", MySqlDbType.String, editingDisplay.name, Register.TypesOfField.String);
                //DatabasePack.Delete (DatabasePack.contentDatabasePrefix, "character_create_stats", delete, true);
                query = "UPDATE character_create_stats SET isactive = 0 where stat = '" + editingDisplay.originalName + "'";
                DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

                // Update online table to avoid access the database again		
                selectedDisplay = -1;
                newSelectedDisplay = 0;
                dataLoaded = false;
                NewResult(Lang.GetTranslate("Entry") + " " + editingDisplay.Name + " " + Lang.GetTranslate("Deleted"));
   Load();
             
                LoadStatOptions();

            }
            else
            {
                EditorUtility.DisplayDialog("", Lang.GetTranslate("You can not delete Stat") + " " + editingDisplay.Name + " " + Lang.GetTranslate("because it is assigned in") + ":" + effectsIds + questReqIds , Lang.GetTranslate("OK"), "");
            }
        }

        void DeleteForever(string id)
        {
            if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete") + " " + Lang.GetTranslate(functionName) + " " + Lang.GetTranslate("entry") + " ?", Lang.GetTranslate("Are you sure you want to permanent delete") + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
            {
                NewResult(Lang.GetTranslate("Deleting..."));
                List<Register> where = new List<Register>();
                where.Add(new Register("name", "?name", MySqlDbType.String, id.ToString(), Register.TypesOfField.String));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, tableName, where);

                where.Clear();
                where.Add(new Register("stat", "?stat", MySqlDbType.String, id.ToString(), Register.TypesOfField.String));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "character_create_stats", where);

                where.Clear();
                where.Add(new Register("stat", "?stat", MySqlDbType.String, id.ToString(), Register.TypesOfField.String));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "stat_link", where);

                where.Clear();
                where.Add(new Register("statTo", "?statTo", MySqlDbType.String, id.ToString(), Register.TypesOfField.String));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "stat_link", where);
                string query = "UPDATE damage_type SET resistance_stat = '' where resistance_stat = '" + editingDisplay.originalName + "'";
                DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

                dataLoaded = false;
                 dataRestoreLoaded = false;
                // Load();
                NewResult(Lang.GetTranslate("Entry Permanent Deleted"));
            }
        }


        void RestoreEntry(string id)
        {
            NewResult(Lang.GetTranslate("Restoring..."));
            string query = "UPDATE " + tableName + " SET isactive = 1 where name = '" + id+"'";
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
            query = "UPDATE character_create_stats SET isactive = 1 where stat = '" + id + "'";
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            dataLoaded = false;
            dataRestoreLoaded = false;
            //Load();
            NewResult(Lang.GetTranslate("Entry Restored"));
        }

        private int GetPositionOfEffect(int effectID)
        {
            for (int i = 0; i < effectIds.Length; i++)
            {
                if (effectIds[i] == effectID)
                    return i;
            }
            return 0;
        }
    }
}