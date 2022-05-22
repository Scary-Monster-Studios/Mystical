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
    public class ServerEffects : AtavismDatabaseFunction
    {

        private static ServerEffects instance;

        public new Dictionary<int, EffectsData> dataRegister;
        public new EffectsData editingDisplay;
        public new EffectsData originalDisplay;
        public int[] skillIds = new int[] { -1 };
        public string[] skillOptions = new string[] { "~ none ~" };
        public static int[] effectIds = new int[] { -1 };
        public static string[] effectOptions = new string[] { "~ none ~" };
        public static GUIContent[] GuiEffectOptions = new GUIContent[] { new GUIContent("~ none ~") };
        private string reqEffectSearch = "";
        private string bonusEffectSearch = "";
        public int[] tagIds = new int[] { -1 };
        public string[] tagOptions = new string[] { "~ none ~" };

        // Handles the prefab creation, editing and save
        private EffectPrefab prefab = null;
       
        // Use this for initialization
        public ServerEffects()
        {
            instance = this;
            functionName = "Effects";
            // Database tables name
            tableName = "effects";
            functionTitle = "Effects Configuration";
            loadButtonLabel = "Load Effects";
            notLoadedText = "No Effect loaded.";
            // Init
            dataRegister = new Dictionary<int, EffectsData>();

            editingDisplay = new EffectsData();
            originalDisplay = new EffectsData();
            reqEffectSearch = "";
            bonusEffectSearch = "";
        }
        void resetSearch(bool fokus)
        {
            reqEffectSearch = "";
            bonusEffectSearch = "";
           if(fokus) GUI.FocusControl(null);

        }

        public override void Activate()
        {
            linkedTablesLoaded = false;
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

        public static void LoadEffectOptions()
        {
            // Read all entries from the table
            string query = "SELECT id, name FROM effects where isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                effectOptions = new string[rows.Count + 1];
                effectOptions[optionsId] = "~ none ~";
                effectIds = new int[rows.Count + 1];
                effectIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    effectOptions[optionsId] = data["id"] + ":" + data["name"];
                    effectIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        public static void LoadEffectOptions(bool gui)
        {
            if (!gui)
            {
                LoadEffectOptions();
                return;
            }
            // Read all entries from the table
            string query = "SELECT id, name FROM effects where isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                GuiEffectOptions = new GUIContent[rows.Count + 1];
                GuiEffectOptions[optionsId] = new GUIContent("~ none ~");
                effectIds = new int[rows.Count + 1];
                effectIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    GuiEffectOptions[optionsId] = new GUIContent(data["id"] + ":" + data["name"]);
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

        EffectsData LoadEntity(int id)
        {
            // Read all entries from the table
            string query = "SELECT " + originalDisplay.GetFieldsString() + " FROM " + tableName + " where isactive = 1 AND id =" + id;

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            Char delimiter = ';';
            EffectsData display = new EffectsData();
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    //foreach(string key in data.Keys)
                    //	Debug.Log("Name[" + key + "]:" + data[key]);
                    //return;

                    display.id = int.Parse(data["id"]);
                    display.Name = data["name"];

                    //display.displayName = data ["displayName"];
                    display.icon = data["icon"];
                    display.effectMainType = data["effectMainType"];
                    // Set the effect class
                    if (editingDisplay == null)
                        editingDisplay = new EffectsData();
                    foreach (ServerEffectType effectClass in editingDisplay.effectTypes)
                    {
                        if (effectClass.EffectType == display.effectMainType)
                        {
                            display.effectClass = effectClass;
                            display.effectMainType = effectClass.EffectType;
                        }
                    }
                    display.effectType = data["effectType"];
                    //display.effectFamily = int.Parse (data ["effectFamily"]);
                    display.isBuff = bool.Parse(data["isBuff"]);
                    display.skillType = int.Parse(data["skillType"]);
                    display.skillLevelMod = float.Parse(data["skillLevelMod"]);
                    display.passive = bool.Parse(data["passive"]);
                    display.stackLimit = int.Parse(data["stackLimit"]);
                    display.allowMultiple = bool.Parse(data["allowMultiple"]);
                    display.duration = float.Parse(data["duration"]);
                    display.pulseCount = int.Parse(data["pulseCount"]);
                    display.tooltip = data["tooltip"];
                    display.bonusEffectReq = int.Parse(data["bonusEffectReq"]);
                    display.bonusEffectReqConsumed = bool.Parse(data["bonusEffectReqConsumed"]);
                    display.bonusEffect = int.Parse(data["bonusEffect"]);
                    display.removeBonusWhenEffectRemoved = bool.Parse(data["removeBonusWhenEffectRemoved"]);
                    display.pulseCoordEffect = data["pulseCoordEffect"];

                    display.interruption_chance = float.Parse(data["interruption_chance"]);
                    display.interruption_chance_max = float.Parse(data["interruption_chance_max"]);
                    display.interruptionAll =      bool.Parse(data["interruption_all"]);

                    display.stackTime = bool.Parse(data["stackTime"]);

                    display.intValue1 = int.Parse(data["intValue1"]);
                    display.intValue2 = int.Parse(data["intValue2"]);
                    display.intValue3 = int.Parse(data["intValue3"]);
                    display.intValue4 = int.Parse(data["intValue4"]);
                    display.intValue5 = int.Parse(data["intValue5"]);

                    display.floatValue1 = float.Parse(data["floatValue1"]);
                    display.floatValue2 = float.Parse(data["floatValue2"]);
                    display.floatValue3 = float.Parse(data["floatValue3"]);
                    display.floatValue4 = float.Parse(data["floatValue4"]);
                    display.floatValue5 = float.Parse(data["floatValue5"]);

                    display.stringValue1 = data["stringValue1"];
                    display.stringValue2 = data["stringValue2"];
                    display.stringValue3 = data["stringValue3"];
                    display.stringValue4 = data["stringValue4"];
                    display.stringValue5 = data["stringValue5"];

                    display.boolValue1 = bool.Parse(data["boolValue1"]);
                    display.boolValue2 = bool.Parse(data["boolValue2"]);
                    display.boolValue3 = bool.Parse(data["boolValue3"]);
                    display.boolValue4 = bool.Parse(data["boolValue4"]);
                    display.boolValue5 = bool.Parse(data["boolValue5"]);
                    if (data["group_tags"].Length > 0)
                    {
                        string[] splitted = data["group_tags"].Split(delimiter);
                        for (int i = 0; i < splitted.Length; i++)
                        {
                            display.tags.Add(int.Parse(splitted[i]));
                            display.tagSearch.Add("");

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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create an Effect before editing it."));
                return;
            }

            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Effect"));

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
                if (editingDisplay.effectClass != null)
                    editingDisplay.effectClass.LoadOptions(editingDisplay, false);
                resetSearch(false);
            }
            if (editingDisplay == null)
            {
                int displayKey = -1;
                if (selectedDisplay >= 0)
                    displayKey = displayKeys[selectedDisplay];
                editingDisplay = LoadEntity(displayKey);
                if (editingDisplay.effectClass != null)
                    editingDisplay.effectClass.LoadOptions(editingDisplay, false);
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
            /*    if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Generate Prefabs")))
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Effect Properties:"));
                pos.y += ImagePack.fieldHeight * 0.75f;
            }

            try
            {
                DrawEditor(pos, false);
            }
            catch (Exception e)
            {
                DatabasePack.SaveLog("ServerEffects Exception " + e.Message + e.StackTrace);
            }

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
            editingDisplay = new EffectsData();
            originalDisplay = new EffectsData();
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

            // Draw the content database info
            if (!linkedTablesLoaded)
            {
                LoadSkillOptions();
                LoadEffectOptions();
                ServerOptionChoices.LoadAtavismChoiceOptions("Effects Tags", false, out tagIds, out tagOptions);
                linkedTablesLoaded = true;
            }

            if (newEntity)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new Effect"));
                pos.y += ImagePack.fieldHeight;

                if (editingDisplay.effectClass == null)
                {
                    foreach (ServerEffectType effectClass in editingDisplay.effectTypes)
                    {
                        if (ImagePack.DrawButton(pos.x, pos.y, effectClass.EffectType))
                        {
                            editingDisplay.effectClass = effectClass;
                            editingDisplay.effectMainType = effectClass.EffectType;
                            effectClass.LoadOptions(editingDisplay, true);
                        }
                        pos.y += ImagePack.fieldHeight;
                    }
                    EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight);
                    return;
                }
            }

            int selectedEffect = -1;

            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.75f);
            pos.y += ImagePack.fieldHeight;
            //editingDisplay.displayName = ImagePack.DrawField (pos, "Display Name:", editingDisplay.displayName, 0.75f);
            //pos.y += ImagePack.fieldHeight;
            //editingDisplay.icon = ImagePack.DrawField (pos, "Icon Name:", editingDisplay.icon, 0.75f);
            pos.width /= 2;
            // We only allowing changing of the effect main type when it is a new item
            ImagePack.DrawText(pos, Lang.GetTranslate("Effect Type")+": " + editingDisplay.effectMainType);
            pos.x += pos.width;
            string icon = ImagePack.DrawSpriteAsset(pos, Lang.GetTranslate("Icon") + ":", editingDisplay.icon);
         /*   if (editingDisplay.icon.Length > 3 && icon == " ")
            {
                EffectPrefab item = new EffectPrefab(editingDisplay);
                editingDisplay.icon = item.LoadIcon();
            }
            else
            {*/
                editingDisplay.icon = icon;
           // }
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight * 1.5f;
            // Allow selection of effect type from the effect sub class
            editingDisplay.effectType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Effect Subtype") + ":", editingDisplay.effectType, editingDisplay.effectClass.EffectTypeOptions);
            pos.y += ImagePack.fieldHeight;
            // Draw items from the effectClass
            bool showTimeFields;
            bool showSkillModFields = true;
            pos = editingDisplay.effectClass.DrawEditor(pos, newEntity, editingDisplay, out showTimeFields, out showSkillModFields);

            //editingDisplay.effectFamily = ImagePack.DrawField (pos, "Family:", editingDisplay.effectFamily);
            //
            if (editingDisplay.effectClass.GetType() != typeof(ServerReviveEffects) && editingDisplay.effectClass.GetType() != typeof(ServerVipEffects) && editingDisplay.effectClass.GetType() != typeof(ServerTrapEffects) && editingDisplay.effectClass.GetType() != typeof(ServerTrapEffects))
            {
                if (editingDisplay.effectClass.GetType() != typeof(ServerDamageEffects) &&
                    editingDisplay.effectClass.GetType() != typeof(ServerRestoreEffects) &&
                    editingDisplay.effectClass.GetType() != typeof(ServerReviveEffects) &&
                    editingDisplay.effectClass.GetType() != typeof(ServerStunEffects)
                    && editingDisplay.effectClass.GetType() != typeof(ServerSleepEffects)
                    && editingDisplay.effectClass.GetType() != typeof(ServerDispelEffect)
                    && editingDisplay.effectClass.GetType() != typeof(ServerTeleportEffects)
                    && editingDisplay.effectClass.GetType() != typeof(ServerBuildObjectEffects)
                    && editingDisplay.effectClass.GetType() != typeof(ServerTeachAbilityEffects)
                     && editingDisplay.effectClass.GetType() != typeof(ServerTeachSkillEffects)
                     && editingDisplay.effectClass.GetType() != typeof(ServerTaskEffects)
                     && editingDisplay.effectClass.GetType() != typeof(ServerThreatEffects)
                     && editingDisplay.effectClass.GetType() != typeof(ServerCreateItemEffects)
                     && editingDisplay.effectClass.GetType() != typeof(ServerCreateItemFromLootEffects)
                     && editingDisplay.effectClass.GetType() != typeof(ServerSpawnEffect)
                     && editingDisplay.effectClass.GetType() != typeof(ServerSetRespawnLocationEffects)
                   // && editingDisplay.effectClass.GetType() != typeof(ServerTriggerEffects)
                    )
                {

                    editingDisplay.isBuff = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Removable by Player") + "?", editingDisplay.isBuff);
                    pos.x += pos.width;
                    if (editingDisplay.effectClass.GetType() != typeof(ServerMountEffects) 
                        && editingDisplay.effectClass.GetType() != typeof(ServerMorphEffects)
                        && editingDisplay.effectClass.GetType() != typeof(ServerStateEffects)
                        && editingDisplay.effectClass.GetType() != typeof(ServerTriggerEffects)
                        )
                    {
                        editingDisplay.passive = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Is Passive") + "?", editingDisplay.passive);

                    }
                    pos.x -= pos.width;
                    pos.y += ImagePack.fieldHeight;
                }
                if (showSkillModFields)
                if (editingDisplay.effectClass.GetType() != typeof(ServerBonusesEffects) &&
                        editingDisplay.effectClass.GetType() != typeof(ServerMountEffects)
                        )
                {

                    int selectedSkill = GetPositionOfSkill(editingDisplay.skillType);
                    selectedSkill = ImagePack.DrawSelector(pos, Lang.GetTranslate("Skill Type") + ":", selectedSkill, skillOptions);
                    editingDisplay.skillType = skillIds[selectedSkill];
                    pos.x += pos.width;
                    editingDisplay.skillLevelMod = ImagePack.DrawField(pos, Lang.GetTranslate("Skill Mod") + ":", editingDisplay.skillLevelMod);
                    pos.x -= pos.width;
                    pos.y += ImagePack.fieldHeight;
                }
                if (showTimeFields)
                {
                    if (editingDisplay.effectClass.GetType() != typeof(ServerBonusesEffects)&&
                        editingDisplay.effectClass.GetType() != typeof(ServerStunEffects) &&
                        editingDisplay.effectClass.GetType() != typeof(ServerImmuneEffects) &&
                            editingDisplay.effectClass.GetType() != typeof(ServerSleepEffects)
                       && editingDisplay.effectClass.GetType() != typeof(ServerMorphEffects)
                          && editingDisplay.effectClass.GetType() != typeof(ServerStateEffects)
                         && editingDisplay.effectClass.GetType() != typeof(ServerShieldEffects)
                           && editingDisplay.effectClass.GetType() != typeof(ServerTriggerEffects)
                  )
                    {

                        editingDisplay.stackLimit = ImagePack.DrawField(pos, Lang.GetTranslate("Stack Limit") + ":", editingDisplay.stackLimit);
                        pos.x += pos.width;
                        editingDisplay.stackTime = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Stack Time") + "?", editingDisplay.stackTime);
                        pos.x -= pos.width;
                        pos.y += ImagePack.fieldHeight;
                        editingDisplay.allowMultiple = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Allow Multiple") + "?", editingDisplay.allowMultiple);
                        //pos.x -= pos.width;
                        pos.y += ImagePack.fieldHeight;
                    }
                    editingDisplay.duration = ImagePack.DrawField(pos, Lang.GetTranslate("Duration (secs)") + ":", editingDisplay.duration);
                    if (editingDisplay.effectClass.GetType() != typeof(ServerBonusesEffects)
                        && editingDisplay.effectClass.GetType() != typeof(ServerImmuneEffects)
                       && editingDisplay.effectClass.GetType() != typeof(ServerMorphEffects)
                             && editingDisplay.effectClass.GetType() != typeof(ServerStateEffects)
                             && editingDisplay.effectClass.GetType() != typeof(ServerStealthEffects)
                              && editingDisplay.effectClass.GetType() != typeof(ServerTriggerEffects)
                   )
                    {

                        pos.x += pos.width;
                        if (editingDisplay.effectClass.GetType() != typeof(ServerStunEffects)&&
                            editingDisplay.effectClass.GetType() != typeof(ServerSleepEffects) && editingDisplay.effectClass.GetType() != typeof(ServerShieldEffects))
                            editingDisplay.pulseCount = ImagePack.DrawField(pos, Lang.GetTranslate("Num. Pulses"), editingDisplay.pulseCount);
                        pos.x -= pos.width;

                        pos.y += ImagePack.fieldHeight;
                        pos.width *= 2;
                        pos.width /= 3;
                        pos.width *= 2;
                      //  if (editingDisplay.effectClass.GetType() != typeof(ServerImmuneEffects))
                            editingDisplay.pulseCoordEffect = ImagePack.DrawGameObject(pos, Lang.GetTranslate("Pulse CoordEffect") + ":", editingDisplay.pulseCoordEffect, 0.6f);
                        pos.width /= 2;
                        //pos.x -= pos.width;
                        pos.width *= 3;
                    }
                    else
                    {
                        pos.width *= 2;
                    }
                    pos.y += ImagePack.fieldHeight;

                }
                else
                {
                    pos.width *= 2;
                }

                editingDisplay.tooltip = ImagePack.DrawField(pos, Lang.GetTranslate("Tooltip") + ":", editingDisplay.tooltip, 0.75f, 60);
                pos.y += ImagePack.fieldHeight;
            }
            else
            {
                pos.width *= 2;
            }
            pos.y += 1.5f * ImagePack.fieldHeight;

            pos.width /= 2;

            if (editingDisplay.effectClass.GetType() == typeof(ServerDamageEffects) ||
                             editingDisplay.effectClass.GetType() == typeof(ServerStunEffects)
                             || editingDisplay.effectClass.GetType() == typeof(ServerSleepEffects)
                              || editingDisplay.effectClass.GetType() == typeof(ServerMorphEffects)
                             )
            {
                editingDisplay.interruption_chance = ImagePack.DrawField(pos, Lang.GetTranslate("Interruption Chance") + ":", editingDisplay.interruption_chance);
                pos.x += pos.width;
                editingDisplay.interruption_chance_max = ImagePack.DrawField(pos, Lang.GetTranslate("Interruption Chance Max") + ":", editingDisplay.interruption_chance_max);
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                editingDisplay.interruptionAll = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Force Interruption") + "?", editingDisplay.interruptionAll);
                //  pos.x -= pos.width;
            }

            pos.y += 1.5f * ImagePack.fieldHeight;

            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Effect Tags"));
         //   pos.y += ImagePack.fieldHeight;
            for (int j = 0; j < editingDisplay.tags.Count; j++)
            {
                pos.y += ImagePack.fieldHeight;
                int selectedTag = GetOptionPosition(editingDisplay.tags[j], tagIds);
                string search = editingDisplay.tagSearch[j];
                selectedTag = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Tag ") + (j + 1) + ":", ref search, selectedTag, tagOptions);
                editingDisplay.tagSearch[j] = search;
                editingDisplay.tags[j] = tagIds[selectedTag];
                pos.x += pos.width;
                pos.y += ImagePack.fieldHeight;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Tag")))
                {

                    editingDisplay.tags.RemoveAt(j);
                    editingDisplay.tagSearch.RemoveAt(j);
                }
                pos.x -= pos.width;
            }
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Tag")))
            {
                editingDisplay.tags.Add(0);
                editingDisplay.tagSearch.Add("");
            }

            pos.width *= 2;

            pos.y += ImagePack.fieldHeight;


            if (editingDisplay.effectClass.GetType() != typeof(ServerVipEffects) && editingDisplay.effectClass.GetType() != typeof(ServerBonusesEffects) && editingDisplay.effectClass.GetType() != typeof(ServerTrapEffects) && editingDisplay.effectClass.GetType() != typeof(ServerTriggerEffects))
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Bonus Effects"));
                pos.y += ImagePack.fieldHeight;
                pos.width /= 2;
                selectedEffect = GetPositionOfEffect(editingDisplay.bonusEffectReq);
                selectedEffect = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Required Effect") + ":", ref reqEffectSearch, selectedEffect, effectOptions);
                editingDisplay.bonusEffectReq = effectIds[selectedEffect];
                //pos.x += pos.width;
                pos.y += ImagePack.fieldHeight;
                selectedEffect = GetPositionOfEffect(editingDisplay.bonusEffect);
                selectedEffect = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Bonus Effect") + ":", ref bonusEffectSearch, selectedEffect, effectOptions);
                editingDisplay.bonusEffect = effectIds[selectedEffect];
                //pos.x -= pos.width;		
                pos.y += ImagePack.fieldHeight;
                editingDisplay.bonusEffectReqConsumed = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Is Consumed") + "?", editingDisplay.bonusEffectReqConsumed);
                pos.x += pos.width;
                editingDisplay.removeBonusWhenEffectRemoved = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Removed when Effect Removed") + "?", editingDisplay.removeBonusWhenEffectRemoved);
                pos.x -= pos.width;

                pos.width *= 2;
            }
            pos.y += ImagePack.fieldHeight;
            // Save data
            pos.x -= ImagePack.innerMargin;
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
            if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete")+" " + Lang.GetTranslate(functionName )+ " "+ Lang.GetTranslate("entry")+" ?", Lang.GetTranslate("Are you sure you want to delete")+" " + editingDisplay.Name + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
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

            int itemID = -1;

            // Setup the register data		
            List<Register> update = new List<Register>();
            foreach (string field in editingDisplay.fields.Keys)
            {
                update.Add(editingDisplay.fieldToRegister(field));
            }

            // Update the database
            itemID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);

            // If the insert failed, don't insert the spawn marker
            if (itemID != -1)
            {
                editingDisplay.id = itemID;
                // Update online table to avoid access the database again			
                editingDisplay.id = itemID;
                editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
                LoadEffectOptions();
                // Configure the correponding prefab
                CreatePrefab();
                dataLoaded = false;
                Load();
                newItemCreated = true;
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
            NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " "+Lang.GetTranslate("updated"));
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

            // Update online table to avoid access the database again		
            selectedDisplay = -1;
            newSelectedDisplay = 0;
            dataLoaded = false;
             NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " "+ Lang.GetTranslate("Deleted"));
             Load();
          LoadEffectOptions();
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
            linkedTablesLoaded = false;
        }

        void GenerateAllPrefabs()
        {
         /*   if (!dataLoaded)
            {
                Load();
            }

            // Delete all existing prefabs?
            EffectPrefab.DeleteAllPrefabs();
           // Debug.LogError("Generate Prefabs displayKeys=" + displayKeys);

            foreach (int id in displayKeys)
            {
               // Debug.LogError("Generate Prefabs id =" + id);
                EffectsData effectData = LoadEntity(id);
                // Only create prefab is duration > 0 or passive
                if (effectData.duration < 1 && !effectData.passive && !effectData.createPrefab)
                    continue;
                prefab = new EffectPrefab(effectData);
                prefab.Save(effectData);
            }*/
        }

        void CreatePrefab()
        {
            // Only create prefab is duration > 0 or passive
        /*    if (editingDisplay.duration < 1 && !editingDisplay.passive && !editingDisplay.createPrefab)
                return;
            // Configure the correponding prefab
            prefab = new EffectPrefab(editingDisplay);
         //   prefab.Save(editingDisplay);*/
        }

        void DeletePrefab()
        {
        /*    prefab = new EffectPrefab(editingDisplay);

            if (prefab.Load())
                prefab.Delete();*/
        }

        public int GetPositionOfSkill(int skillID)
        {
            for (int i = 0; i < skillIds.Length; i++)
            {
                if (skillIds[i] == skillID)
                    return i;
            }
            return 0;
        }

        public int GetPositionOfEffect(int effectID)
        {
            for (int i = 0; i < effectIds.Length; i++)
            {
                if (effectIds[i] == effectID)
                    return i;
            }
            return 0;
        }

        public static ServerEffects Instance
        {
            get
            {
                return instance;
            }
        }
    }
}