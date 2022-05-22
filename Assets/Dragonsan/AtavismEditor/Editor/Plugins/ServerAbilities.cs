using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
//using System.Data;
using System.Collections;
using System.Collections.Generic;
namespace Atavism
{
    // Handles the Skills Configuration
    public class ServerAbilities : AtavismDatabaseFunction
    {

        public new Dictionary<int, AbilitiesData> dataRegister;
        public new AbilitiesData editingDisplay;
        public new AbilitiesData originalDisplay;
        public int[] skillIds = new int[] { -1 };
        public string[] skillOptions = new string[] { "~ none ~" };
        public int[] effectIds = new int[] { -1 };
        public string[] effectOptions = new string[] { "~ none ~" };
        public int[] itemIds = new int[] { -1 };
        public string[] itemsList = new string[] { "~ none ~" };
        public int[] coordIds = new int[] { -1 };
        public string[] coordList = new string[] { "~ none ~" };

        public string[] vitalityStatOptions = new string[] { "~ none ~" };

        public string[] weaponTypeOptions = new string[] { "~ none ~" };
        public string[] speciesOptions = new string[] { "~ none ~" };
        public string[] targetOptions = new string[] { "~ none ~" };

        Vector2 tooltipScroll = new Vector2();
        public int[] tagIds = new int[] { -1 };
        public string[] tagOptions = new string[] { "~ none ~" };

        // Handles the prefab creation, editing and save
        private AbilityPrefab prefab = null;
        // Filter/Search inputs
        private string casterEffectSearchInput = "";
        private string targetEffectSearchInput = "";
        private string reagentSearchInput = "";
        private string reagentSearchInput2 = "";
        private string reagentSearchInput3 = "";
        private string pulsCasterEffectSearchInput = "";
        private string pulsTargetEffectSearchInput = "";
        private string pulsReagentSearchInput = "";
        private string pulsReagentSearchInput2 = "";
        private string pulsReagentSearchInput3 = "";
        private string effect1SearchInput = "";
        private string effect2SearchInput = "";
        private string effect3SearchInput = "";
        private string effect4SearchInput = "";
        private string effect5SearchInput = "";
        private string effect6SearchInput = "";
        private string aoeCordSearch = "";
        private string cordSearch1 = "";
        private string cordSearch2 = "";
        private string cordSearch3 = "";
        private string cordSearch4 = "";
        private string cordSearch5 = "";
        private bool justLoad = false;
        // Use this for initialization
        public ServerAbilities()
        {
            functionName = "Abilities";
            // Database tables name
            tableName = "abilities";
            functionTitle = "Abilities Configuration";
            loadButtonLabel = "Load Abilities";
            notLoadedText = "No Abilitie loaded.";
            // Init
            dataRegister = new Dictionary<int, AbilitiesData>();

            editingDisplay = new AbilitiesData();
            originalDisplay = new AbilitiesData();
            
        }

        void resetSearch(bool fokus)
        {
            casterEffectSearchInput = "";
            targetEffectSearchInput = "";
            reagentSearchInput = "";
            reagentSearchInput2 = "";
            reagentSearchInput3 = "";
            effect1SearchInput = "";
            effect2SearchInput = "";
            effect3SearchInput = "";
            effect4SearchInput = "";
            effect5SearchInput = "";
            effect6SearchInput = "";
            cordSearch1 = "";
            cordSearch2 = "";
            cordSearch3 = "";
            cordSearch4 = "";
            cordSearch5 = "";
            aoeCordSearch = "";
            if (fokus) GUI.FocusControl(null);
        }

        public override void Activate()
        {
            linkedTablesLoaded = false;
            resetSearch(true);
          //  Debug.LogError("Ability Server Activation");
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
                skillIds[optionsId] = 0;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    skillOptions[optionsId] = data["id"] + ":" + data["name"];
                    skillIds[optionsId] = int.Parse(data["id"]);
                }
            }
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
                        effectOptions[optionsId] = data["id"] + ":" + data["name"];
                        effectIds[optionsId] = int.Parse(data["id"]);
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
                itemIds[optionsId] = 0;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    itemsList[optionsId] = data["id"] + ":" + data["name"];
                    itemIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        private void LoadCoordList()
        {
            string query = "SELECT id, name FROM coordinated_effects where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                coordList = new string[rows.Count + 1];
                coordList[optionsId] = "~ none ~";
                coordIds = new int[rows.Count + 1];
                coordIds[optionsId] = 0;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    coordList[optionsId] = data["name"];
                    coordIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }

        public void LoadVitalityStatOptions()
        {
            // Read all entries from the table
            string query = "SELECT name FROM stat where type = 2 AND isactive = 1";

            // Load data
            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                vitalityStatOptions = new string[rows.Count];
                //vitalityStatOptions [optionsId] = "~ none ~"; 
                foreach (Dictionary<string, string> data in rows)
                {
                    vitalityStatOptions[optionsId] = data["name"];
                    optionsId++;
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

        AbilitiesData LoadEntity(int id)
        {
            // Read all entries from the table
            string query = "SELECT " + originalDisplay.GetFieldsString() + " FROM " + tableName + " where isactive = 1 AND id=" + id;


            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            AbilitiesData display = new AbilitiesData();
            Char delimiter = ';';
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

                    display.abilityType = data["abilityType"];
                    display.skill = int.Parse(data["skill"]);
                    display.passive = bool.Parse(data["passive"]);
                    display.activationCost = int.Parse(data["activationCost"]);
                    display.activationCostPercentage = float.Parse(data["activationCostPercentage"]);
                    display.activationCostType = data["activationCostType"];
                    display.activationLength = float.Parse(data["activationLength"]);
                    display.activationAnimation = data["activationAnimation"];
                    display.activationParticles = data["activationParticles"];
                    display.casterEffectRequired = int.Parse(data["casterEffectRequired"]);
                    display.casterEffectConsumed = bool.Parse(data["casterEffectConsumed"]);
                    display.targetEffectRequired = int.Parse(data["targetEffectRequired"]);
                    display.targetEffectConsumed = bool.Parse(data["targetEffectConsumed"]);
                    display.weaponRequired = data["weaponRequired"];
                    display.reagentRequired = int.Parse(data["reagentRequired"]);
                    display.reagentCount = int.Parse(data["reagentCount"]);
                    display.reagentConsumed = bool.Parse(data["reagentConsumed"]);
                    display.reagent2Required = int.Parse(data["reagent2Required"]);
                    display.reagent2Count = int.Parse(data["reagent2Count"]);
                    display.reagent2Consumed = bool.Parse(data["reagent2Consumed"]);
                    display.reagent3Required = int.Parse(data["reagent3Required"]);
                    display.reagent3Count = int.Parse(data["reagent3Count"]);
                    display.reagent3Consumed = bool.Parse(data["reagent3Consumed"]);
                    display.ammoUsed = int.Parse(data["ammoUsed"]);
                    display.maxRange = int.Parse(data["maxRange"]);
                    display.minRange = int.Parse(data["minRange"]);
                    display.aoeRadius = int.Parse(data["aoeRadius"]);
                    display.aoeAngle = float.Parse(data["aoeAngle"]);
                    display.aoeType = data["aoeType"];
                    display.aoePrefab = data["aoePrefab"];
                    display.reqTarget = bool.Parse(data["reqTarget"]);
                    display.reqFacingTarget = bool.Parse(data["reqFacingTarget"]);
                    display.autoRotateToTarget = bool.Parse(data["autoRotateToTarget"]);
                    display.castingInRun = bool.Parse(data["castingInRun"]);
                    display.relativePositionReq = int.Parse(data["relativePositionReq"]);
                    display.targetType = data["targetType"];
                    display.targetState = int.Parse(data["targetState"]);
                    display.speciesTargetReq = data["speciesTargetReq"];
                    display.specificTargetReq = data["specificTargetReq"];
                    display.globalCooldown = bool.Parse(data["globalCooldown"]);
                    display.cooldown1Type = data["cooldown1Type"];
                    display.cooldown1Duration = float.Parse(data["cooldown1Duration"]);
                    display.weaponCooldown = bool.Parse(data["weaponCooldown"]);
                    display.startCooldownsOnActivation = bool.Parse(data["startCooldownsOnActivation"]);
                    display.activationEffect1 = int.Parse(data["activationEffect1"]);
                    display.activationTarget1 = data["activationTarget1"];
                    display.activationEffect2 = int.Parse(data["activationEffect2"]);
                    display.activationTarget2 = data["activationTarget2"];
                    display.activationEffect3 = int.Parse(data["activationEffect3"]);
                    display.activationTarget3 = data["activationTarget3"];
                    display.activationEffect4 = int.Parse(data["activationEffect4"]);
                    display.activationTarget4 = data["activationTarget4"];
                    display.activationEffect5 = int.Parse(data["activationEffect5"]);
                    display.activationTarget5 = data["activationTarget5"];
                    display.activationEffect6 = int.Parse(data["activationEffect6"]);
                    display.activationTarget6 = data["activationTarget6"];
                    display.coordEffect1Event = data["coordEffect1event"];
                    display.coordEffect1 = data["coordEffect1"];
                    display.coordEffect2Event = data["coordEffect2event"];
                    display.coordEffect2 = data["coordEffect2"];
                    display.coordEffect3Event = data["coordEffect3event"];
                    display.coordEffect3 = data["coordEffect3"];
                    display.coordEffect4Event = data["coordEffect4event"];
                    display.coordEffect4 = data["coordEffect4"];
                    display.coordEffect5Event = data["coordEffect5event"];
                    display.coordEffect5 = data["coordEffect5"];
                    display.tooltip = data["tooltip"];
                    display.chance = float.Parse(data["chance"]);
                    display.exp = int.Parse(data["exp"]);
                    display.consumeOnActivation = bool.Parse(data["consumeOnActivation"]);
                   // display.aoeCoordEffect = data["aoeCoordEffect"];
                    display.channelling_pulse_time = float.Parse(data["channelling_pulse_time"]);
                    display.channelling_pulse_num = int.Parse(data["channelling_pulse_num"]);
                  //  display.channelling_cost = int.Parse(data["channelling_cost"]);
                    display.channelling = bool.Parse(data["channelling"]);
                    display.channelling_in_run = bool.Parse(data["channelling_in_run"]);
                    display.activationDelay = float.Parse(data["activationDelay"]);

                    display.pulseCost = int.Parse(data["pulseCost"]);
                    display.pulseCostPercentage = float.Parse(data["pulseCostPercentage"]);
                    display.pulseCostType = data["pulseCostType"];
                    display.pulseCasterEffectRequired = int.Parse(data["pulseCasterEffectRequired"]);
                    display.pulseCasterEffectConsumed = bool.Parse(data["pulseCasterEffectConsumed"]);
                    display.pulseTargetEffectRequired = int.Parse(data["pulseTargetEffectRequired"]);
                    display.pulseTargetEffectConsumed = bool.Parse(data["pulseTargetEffectConsumed"]);
                    display.pulseReagentRequired = int.Parse(data["pulseReagentRequired"]);
                    display.pulseReagentCount = int.Parse(data["pulseReagentCount"]);
                    display.pulseReagentConsumed = bool.Parse(data["pulseReagentConsumed"]);
                    display.pulseReagent2Required = int.Parse(data["pulseReagent2Required"]);
                    display.pulseReagent2Count = int.Parse(data["pulseReagent2Count"]);
                    display.pulseReagent2Consumed = bool.Parse(data["pulseReagent2Consumed"]);
                    display.pulseReagent3Required = int.Parse(data["pulseReagent3Required"]);
                    display.pulseReagent3Count = int.Parse(data["pulseReagent3Count"]);
                    display.pulseReagent3Consumed = bool.Parse(data["pulseReagent3Consumed"]);

                    display.skipChecks = bool.Parse(data["skipChecks"]);
                    display.stealthReduction = bool.Parse(data["stealth_reduce"]);
                    display.interruptible = bool.Parse(data["interruptible"]);
                    display.interruption_chance = float.Parse(data["interruption_chance"]);
                    display.toggle = bool.Parse(data["toggle"]);

                    display.tagToDisable = int.Parse(data["tag_disable"]);
                    display.maxCountWithTag = int.Parse(data["tag_count"]);

                    display.chunk_length = float.Parse(data["chunk_length"]);
                    display.speed = float.Parse(data["speed"]);
                    display.aoePrediction = int.Parse(data["prediction"]);
                    display.aoeCountType = int.Parse(data["aoe_target_count_type"]);
                    display.aoeCountTargets = int.Parse(data["aoe_target_count"]);

                    if (data["tags"].Length > 0)
                    {
                        string[] splitted = data["tags"].Split(delimiter);
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create an Ability before edit it."));
                return;
            }

            // Draw the content database info
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Ability"));


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
                justLoad = true;
            }
            if (editingDisplay == null)
            {
                int displayKey = -1;
                if (selectedDisplay >= 0)
                    displayKey = displayKeys[selectedDisplay];
                editingDisplay = LoadEntity(displayKey);
                if (editingDisplay == null)
                    editingDisplay = new AbilitiesData();
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
             /*   if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Generate Prefabs")))
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Ability Properties")+":");
                pos.y += ImagePack.fieldHeight * 0.75f;
            }

            try
            {
                DrawEditor(pos, false);
            }
            catch (Exception e)
            {
                DatabasePack.SaveLog("ServerAbilities Exception "+e.Message+e.StackTrace);
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
            editingDisplay = new AbilitiesData();
            originalDisplay = new AbilitiesData();
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
                LoadEffectOptions();
                LoadSkillOptions();
                LoadItemList();
                LoadCoordList();
                LoadVitalityStatOptions();
                weaponTypeOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Weapon Type", true);
                speciesOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Species", true);
                targetOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Target Type", false);
                ServerOptionChoices.LoadAtavismChoiceOptions("Ability Tags", true, out tagIds, out tagOptions);

                linkedTablesLoaded = true;
            }

            // Draw the content database info		
            if (newEntity)
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new Ability"));
                pos.y += 1.5f * ImagePack.fieldHeight;
            }
            editingDisplay.Name = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.Name, 0.75f);
            pos.y += ImagePack.fieldHeight;
            pos.width /= 2;
            string abilityTypePrev = editingDisplay.abilityType;
            editingDisplay.abilityType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Ability Type")+":", editingDisplay.abilityType, editingDisplay.abilityTypeOptions);
            if (abilityTypePrev != editingDisplay.abilityType || justLoad)
            {
                //{ "CombatMeleeAbility", "MagicalAttackAbility", "EffectAbility", "FriendlyEffectAbility" };
                targetOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Target Type", false);
                List<string> abilityTypeList = new List<string>(targetOptions);
                if (editingDisplay.abilityType == "CombatMeleeAbility" || editingDisplay.abilityType == "MagicalAttackAbility")
                {
                        abilityTypeList.RemoveAll(delegate (string v) {
                            return !v.Contains("Enemy");
                        });
                }
                if (editingDisplay.abilityType == "EffectAbility")
                {
                    abilityTypeList.RemoveAll(delegate (string v) {
                        return v.Contains("Group");
                    });

                }
                if (editingDisplay.abilityType == "FriendlyEffectAbility")
                {
                    abilityTypeList.RemoveAll(delegate (string v) {
                        return v.Contains("Enemy");
                    });
                }

                targetOptions = abilityTypeList.ToArray();
                justLoad = false;
            }
          //  targetOptions = targetOptions.Except(new int[] { 4 }).ToArray();

          //  int numToRemove = 4;
           // targetOptions = targetOptions.Where(val => val != "Group").ToArray();


            pos.x += pos.width;
            string icon = ImagePack.DrawSpriteAsset(pos, Lang.GetTranslate("Icon") + ":", editingDisplay.icon);
           /* if (editingDisplay.icon.Length > 3 && icon == " ")
            {
                AbilityPrefab item = new AbilityPrefab(editingDisplay);
                editingDisplay.icon = item.LoadIcon();
            }
            else
            {*/
                editingDisplay.icon = icon;
           // }
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            int selectedSkill = GetOptionPosition(editingDisplay.skill, skillIds);
            selectedSkill = ImagePack.DrawSelector(pos, Lang.GetTranslate("Skill")+":", selectedSkill, skillOptions);
            editingDisplay.skill = skillIds[selectedSkill];
            pos.y += ImagePack.fieldHeight;
          
            editingDisplay.exp = ImagePack.DrawField(pos, Lang.GetTranslate("Experience")+":", editingDisplay.exp);
            if (editingDisplay.abilityType == "FriendlyEffectAbility")
            {
                pos.y += ImagePack.fieldHeight;
                editingDisplay.passive = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Passive") + ":", editingDisplay.passive);
            //    pos.y += ImagePack.fieldHeight;
            }
            else
            {
                editingDisplay.passive = false;
            }
            if (editingDisplay.abilityType == "EffectAbility")
            {
                pos.y += ImagePack.fieldHeight;
                //    pos.x += pos.width;
                editingDisplay.chance = ImagePack.DrawField(pos, Lang.GetTranslate("Chance") + ":", editingDisplay.chance);
                //     pos.x -= pos.width;
           }
            pos.y += ImagePack.fieldHeight;
            editingDisplay.channelling = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Channelling") + "?", editingDisplay.channelling);
           if (!editingDisplay.channelling)
            {
                pos.y += ImagePack.fieldHeight;
                editingDisplay.channelling_pulse_num = ImagePack.DrawField(pos, Lang.GetTranslate("Ability Pulse Num") + ":", editingDisplay.channelling_pulse_num);
                //   pos.y += ImagePack.fieldHeight;
                pos.x += pos.width;
                editingDisplay.channelling_pulse_time = ImagePack.DrawField(pos, Lang.GetTranslate("Ability Pulse Time (s)") + ":", editingDisplay.channelling_pulse_time);
                  pos.x -= pos.width;
            }
             if (editingDisplay.channelling)
            {
                pos.y += ImagePack.fieldHeight;
                // pos.x += pos.width;
                editingDisplay.channelling_in_run = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Channelling In Run") + "?", editingDisplay.channelling_in_run);
               // pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                //   editingDisplay.channelling_cost = ImagePack.DrawField(pos, Lang.GetTranslate("Channelling Puls Cost") + ":", editingDisplay.channelling_cost);
                editingDisplay.channelling_pulse_num = ImagePack.DrawField(pos, Lang.GetTranslate("Channelling Pulse Num") + ":", editingDisplay.channelling_pulse_num);
                //      pos.x += pos.width;
                pos.x += pos.width;
                //pos.y += ImagePack.fieldHeight;
                editingDisplay.channelling_pulse_time = ImagePack.DrawField(pos, Lang.GetTranslate("Channelling Pulse Time (s)") + ":", editingDisplay.channelling_pulse_time);
                pos.x -= pos.width;

            }
            pos.y += ImagePack.fieldHeight;
            editingDisplay.activationLength = ImagePack.DrawField(pos, Lang.GetTranslate("Cast Time (s)") + ":", editingDisplay.activationLength);
            pos.x += pos.width;
            editingDisplay.castingInRun = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Casting In Run") + "?", editingDisplay.castingInRun);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.skipChecks = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Skip part of the checking") + "?", editingDisplay.skipChecks);
            pos.x += pos.width;
            editingDisplay.stealthReduction = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Stealth Reduce") + "?", editingDisplay.stealthReduction);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.interruptible = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Interruptible") + "?", editingDisplay.interruptible);
            pos.x += pos.width;
         
            if (editingDisplay.interruptible)
            {
               editingDisplay.interruption_chance = ImagePack.DrawField(pos, Lang.GetTranslate("Interruption Chance %") + ":", editingDisplay.interruption_chance);

            }
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;

            editingDisplay.toggle = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Is Toggle") + "?", editingDisplay.toggle);
            pos.x += pos.width;
            if (editingDisplay.toggle)
                editingDisplay.maxCountWithTag = ImagePack.DrawField(pos, Lang.GetTranslate("Count of Abilities") + ":", editingDisplay.maxCountWithTag);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            
            if (editingDisplay.toggle)
            {
                int selectedTag = GetOptionPosition(editingDisplay.tagToDisable, tagIds);
                selectedTag = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Disable Ability with tag")  + ":", ref editingDisplay.tagToDisableSearch, selectedTag, tagOptions);
                editingDisplay.tagToDisable = tagIds[selectedTag];
                pos.y += ImagePack.fieldHeight;
            }
            editingDisplay.speed = ImagePack.DrawField(pos, Lang.GetTranslate("Projectile Speed (m/s)") + ":", editingDisplay.speed);


            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Activation Requirements"));
            pos.y += 1.5f * ImagePack.fieldHeight;
        
            editingDisplay.activationCostType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Cost Type") + ":", editingDisplay.activationCostType, vitalityStatOptions);
            pos.x += pos.width;
            editingDisplay.activationCost = ImagePack.DrawField(pos, Lang.GetTranslate("Cost")+":", editingDisplay.activationCost);
             pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.activationCostPercentage = ImagePack.DrawField(pos, Lang.GetTranslate("Cost Percentage") + ":", editingDisplay.activationCostPercentage);


            pos.y += ImagePack.fieldHeight;
            int selectedEffect = GetOptionPosition(editingDisplay.casterEffectRequired, effectIds);
            selectedEffect = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Caster Effect")+":", ref casterEffectSearchInput, selectedEffect, effectOptions);
            editingDisplay.casterEffectRequired = effectIds[selectedEffect];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.casterEffectConsumed = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("is Consumed")+"?", editingDisplay.casterEffectConsumed);
            pos.y += ImagePack.fieldHeight;
            selectedEffect = GetOptionPosition(editingDisplay.targetEffectRequired, effectIds);
            selectedEffect = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Target Effect")+":", ref targetEffectSearchInput, selectedEffect, effectOptions);
            editingDisplay.targetEffectRequired = effectIds[selectedEffect];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.targetEffectConsumed = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("is Consumed")+"?", editingDisplay.targetEffectConsumed);
            pos.y += ImagePack.fieldHeight;
            int selectedItem = GetOptionPosition(editingDisplay.reagentRequired, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Reagent")+":", ref reagentSearchInput, selectedItem, itemsList);
            editingDisplay.reagentRequired = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.reagentCount = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.reagentCount);
            pos.x += pos.width;
            editingDisplay.reagentConsumed = ImagePack.DrawToggleBox(pos, "is Consumed?", editingDisplay.reagentConsumed);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.reagent2Required, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Reagent")+":", ref reagentSearchInput2, selectedItem, itemsList);
            editingDisplay.reagent2Required = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.reagent2Count = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.reagent2Count);
            pos.x += pos.width;
            editingDisplay.reagent2Consumed = ImagePack.DrawToggleBox(pos, "is Consumed?", editingDisplay.reagent2Consumed);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.reagent3Required, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Reagent")+":", ref reagentSearchInput3, selectedItem, itemsList);
            editingDisplay.reagent3Required = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.reagent3Count = ImagePack.DrawField(pos, Lang.GetTranslate("Count")+":", editingDisplay.reagent3Count);
            pos.x += pos.width;
            editingDisplay.reagent3Consumed = ImagePack.DrawToggleBox(pos, "is Consumed?", editingDisplay.reagent3Consumed);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
      /*      editingDisplay.consumeAllreagent = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Consume all Reagents") + "?", editingDisplay.consumeAllreagent);
            if (editingDisplay.consumeAllreagent)
            {
                editingDisplay.reagentConsumed = true;
                editingDisplay.reagent2Consumed = true;
                editingDisplay.reagent3Consumed = true;
            }*/
            pos.x += pos.width;
            editingDisplay.consumeOnActivation = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Consume cost/reagent On Activation") + "?", editingDisplay.consumeOnActivation);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;

            editingDisplay.ammoUsed = ImagePack.DrawField(pos, Lang.GetTranslate("Ammo Used")+":", editingDisplay.ammoUsed);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.weaponRequired = ImagePack.DrawSelector(pos, Lang.GetTranslate("Weapon")+":", editingDisplay.weaponRequired, weaponTypeOptions);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.targetType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Target Type") + ":", editingDisplay.targetType, targetOptions);
            pos.x += pos.width;
            editingDisplay.targetState = ImagePack.DrawSelector(pos, Lang.GetTranslate("Target State") + ":", editingDisplay.targetState, editingDisplay.targetStateOptions);
            pos.x -= pos.width;


            /*pos.x += pos.width;
            editingDisplay.activationAnimation = ImagePack.DrawField (pos, "Animation:", editingDisplay.activationAnimation);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.activationParticles = ImagePack.DrawGameObject (pos, "Particles:", editingDisplay.activationParticles, 0.75f);*/
            if (editingDisplay.targetType == "AoE Enemy" || editingDisplay.targetType == "AoE Friendly" || editingDisplay.targetType == "Group" || editingDisplay.targetType == "Self Location")
            {

                pos.y += ImagePack.fieldHeight;
                if (editingDisplay.targetType == "Group")
                {
                    editingDisplay.aoeType = ImagePack.DrawSelector(pos, Lang.GetTranslate("AoE Type") + ":", editingDisplay.aoeType, editingDisplay.groupTypeOptions);
                }
                else if(editingDisplay.targetType == "Self Location")
                {
                    editingDisplay.aoeType = ImagePack.DrawSelector(pos, Lang.GetTranslate("AoE Type") + ":", editingDisplay.aoeType, editingDisplay.selfAoeTypeOptions);
                }
                else
                {
                    editingDisplay.aoeType = ImagePack.DrawSelector(pos, Lang.GetTranslate("AoE Type") + ":", editingDisplay.aoeType, editingDisplay.aoeTypeOptions);
                }
                pos.x += pos.width;
                editingDisplay.chunk_length = ImagePack.DrawField(pos, Lang.GetTranslate("Chunk Size (m)") + ":", editingDisplay.chunk_length);

                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                editingDisplay.aoePrediction = ImagePack.DrawSelector(pos, Lang.GetTranslate("AoE Hit Calculation") + ":", editingDisplay.aoePrediction, editingDisplay.aoePredictionOptions);
                pos.y += ImagePack.fieldHeight;
                editingDisplay.aoeCountType = ImagePack.DrawSelector(pos, Lang.GetTranslate("AoE Count Type") + ":", editingDisplay.aoeCountType, editingDisplay.aoeCountOptions);
                pos.x += pos.width;
                if(editingDisplay.aoeCountType==1|| editingDisplay.aoeCountType==2)
                editingDisplay.aoeCountTargets = ImagePack.DrawField(pos, Lang.GetTranslate("Aoe Targets Count") + ":", editingDisplay.aoeCountTargets);
                pos.x -= pos.width;
                pos.y += ImagePack.fieldHeight;
                editingDisplay.aoeRadius = ImagePack.DrawField(pos, Lang.GetTranslate("AoE Radius") + ":", editingDisplay.aoeRadius);
                pos.x += pos.width;
                if (editingDisplay.targetType != "Self Location")
                    editingDisplay.aoeAngle = ImagePack.DrawField(pos, Lang.GetTranslate("AoE Angle") + ":", editingDisplay.aoeAngle);
                
                pos.x -= pos.width;
                if (editingDisplay.aoeType == "LocationRadius")
                {
                    pos.y += ImagePack.fieldHeight;
                    pos.width *= 2;
                    editingDisplay.aoePrefab = ImagePack.DrawGameObject(pos, Lang.GetTranslate("AoE Marker") + ":", editingDisplay.aoePrefab, 0.75f);
                    pos.width /= 2;
                    //   pos.width *= 2;
                    //    pos.y += ImagePack.fieldHeight;
                    //   editingDisplay.aoeCoordEffect = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Aoe Coord. Effect"), ref aoeCordSearch, editingDisplay.aoeCoordEffect, coordList);
                    pos.y += ImagePack.fieldHeight;
                    editingDisplay.activationDelay = ImagePack.DrawField(pos, Lang.GetTranslate("AoE Activation Delay (s)") + ":", editingDisplay.activationDelay);
               //     pos.y += ImagePack.fieldHeight;
                    editingDisplay.reqTarget = false;
                    editingDisplay.reqFacingTarget = false;
                    // pos.width /= 2;
                }
                if (editingDisplay.aoeType == "PlayerRadius")
                {
                    editingDisplay.reqTarget = false;
                    editingDisplay.reqFacingTarget = false;
                }
                else if (editingDisplay.aoeType == "TargetRadius")
                {
                    editingDisplay.reqTarget = true;
                }
                else if (editingDisplay.targetType == "Self Location")
                {
                    editingDisplay.reqTarget = false;
                    editingDisplay.reqFacingTarget = false;
                }
            }
            else
            {
               editingDisplay.aoeType = "None";
               editingDisplay.reqTarget = true;
            }
            pos.y += ImagePack.fieldHeight;
            editingDisplay.speciesTargetReq = ImagePack.DrawSelector(pos, Lang.GetTranslate("Target Species") + ":", editingDisplay.speciesTargetReq, speciesOptions);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.minRange = ImagePack.DrawField(pos, Lang.GetTranslate("Min. Range") + ":", editingDisplay.minRange);
            pos.x += pos.width;
            editingDisplay.maxRange = ImagePack.DrawField(pos, Lang.GetTranslate("Max. Range") + ":", editingDisplay.maxRange);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
           
            editingDisplay.reqTarget = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Req Target")+"?", editingDisplay.reqTarget);
            pos.x += pos.width;
            editingDisplay.reqFacingTarget = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Req Facing Target")+"?", editingDisplay.reqFacingTarget);
            pos.x -= pos.width;
            //     pos.y += ImagePack.fieldHeight;
            //   editingDisplay.relativePositionReq = ImagePack.DrawSelector(pos, Lang.GetTranslate("Relative Position") + ":", editingDisplay.relativePositionReq, editingDisplay.relativePositionOptions);
            //     pos.y += ImagePack.fieldHeight;

            //	pos.x += pos.width;
            //editingDisplay.autoRotateToTarget = ImagePack.DrawToggleBox (pos, "Auto Rotate To Target?", editingDisplay.autoRotateToTarget);
            //	pos.x -= pos.width;
            /*pos.x += pos.width;
           editingDisplay.specificTargetReq = ImagePack.DrawField (pos, "Specific Target:", editingDisplay.specificTargetReq);
           pos.x -= pos.width;*/
            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Pulse Requirements"));
            pos.y += ImagePack.fieldHeight;
            editingDisplay.pulseCostType = ImagePack.DrawSelector(pos, Lang.GetTranslate("Cost Type") + ":", editingDisplay.pulseCostType, vitalityStatOptions);
            pos.x += pos.width;
            editingDisplay.pulseCost = ImagePack.DrawField(pos, Lang.GetTranslate("Cost") + ":", editingDisplay.pulseCost);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.pulseCostPercentage = ImagePack.DrawField(pos, Lang.GetTranslate("Cost Percentage") + ":", editingDisplay.pulseCostPercentage);

            pos.y += ImagePack.fieldHeight;
            selectedEffect = GetOptionPosition(editingDisplay.pulseCasterEffectRequired, effectIds);
            selectedEffect = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Caster Effect") + ":", ref pulsCasterEffectSearchInput, selectedEffect, effectOptions);
            editingDisplay.pulseCasterEffectRequired = effectIds[selectedEffect];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.pulseCasterEffectConsumed = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("is Consumed") + "?", editingDisplay.pulseCasterEffectConsumed);
            pos.y += ImagePack.fieldHeight;
            selectedEffect = GetOptionPosition(editingDisplay.pulseTargetEffectRequired, effectIds);
            selectedEffect = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Target Effect") + ":", ref pulsTargetEffectSearchInput, selectedEffect, effectOptions);
            editingDisplay.pulseTargetEffectRequired = effectIds[selectedEffect];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.pulseTargetEffectConsumed = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("is Consumed") + "?", editingDisplay.pulseTargetEffectConsumed);

            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.pulseReagentRequired, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Reagent") + ":", ref pulsReagentSearchInput, selectedItem, itemsList);
            editingDisplay.pulseReagentRequired = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.pulseReagentCount = ImagePack.DrawField(pos, Lang.GetTranslate("Count") + ":", editingDisplay.pulseReagentCount);
            pos.x += pos.width;
            editingDisplay.pulseReagentConsumed = ImagePack.DrawToggleBox(pos, "is Consumed?", editingDisplay.pulseReagentConsumed);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.pulseReagent2Required, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Reagent") + ":", ref pulsReagentSearchInput2, selectedItem, itemsList);
            editingDisplay.pulseReagent2Required = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.pulseReagent2Count = ImagePack.DrawField(pos, Lang.GetTranslate("Count") + ":", editingDisplay.pulseReagent2Count);
            pos.x += pos.width;
             editingDisplay.pulseReagent2Consumed = ImagePack.DrawToggleBox(pos, "is Consumed?", editingDisplay.pulseReagent2Consumed);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            selectedItem = GetOptionPosition(editingDisplay.pulseReagent3Required, itemIds);
            selectedItem = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Reagent") + ":", ref pulsReagentSearchInput3, selectedItem, itemsList);
            editingDisplay.pulseReagent3Required = itemIds[selectedItem];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.pulseReagent3Count = ImagePack.DrawField(pos, Lang.GetTranslate("Count") + ":", editingDisplay.pulseReagent3Count);
            pos.x += pos.width;
            editingDisplay.pulseReagent3Consumed = ImagePack.DrawToggleBox(pos, "is Consumed?", editingDisplay.pulseReagent3Consumed);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            
         /*   editingDisplay.pulseConsumeAllreagent = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Consume all Reagents") + "?", editingDisplay.pulseConsumeAllreagent);
            if (editingDisplay.pulseConsumeAllreagent)
            {
                editingDisplay.pulseReagentConsumed = true;
                editingDisplay.pulseReagent2Consumed = true;
                editingDisplay.pulseReagent3Consumed = true;
            }*/
         //   pos.x += pos.width;


            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Cooldown Attributes"));
            pos.y += 1.5f * ImagePack.fieldHeight;
            editingDisplay.globalCooldown = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Global Cooldown")+"?:", editingDisplay.globalCooldown);
            pos.x += pos.width;
            editingDisplay.weaponCooldown = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Weapon Cooldown")+"?:", editingDisplay.weaponCooldown);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.cooldown1Type = ImagePack.DrawField(pos, Lang.GetTranslate("Cooldown Type")+":", editingDisplay.cooldown1Type);
            pos.x += pos.width;
            editingDisplay.cooldown1Duration = ImagePack.DrawField(pos, Lang.GetTranslate("Duration")+":", editingDisplay.cooldown1Duration);
            pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.startCooldownsOnActivation = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Start Cooldown on Activation")+"?:", editingDisplay.startCooldownsOnActivation);
            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Effects"));
            pos.y += 1.5f * ImagePack.fieldHeight;
            editingDisplay.activationTarget1 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Act. Target")+" 1:", editingDisplay.activationTarget1, editingDisplay.activationTargetOptions);
            pos.y += ImagePack.fieldHeight;
            selectedEffect = GetOptionPosition(editingDisplay.activationEffect1, effectIds);
            selectedEffect = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Act. Effect")+" 1:", ref effect1SearchInput, selectedEffect, effectOptions);
            editingDisplay.activationEffect1 = effectIds[selectedEffect];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.activationTarget2 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Act. Target")+" 2:", editingDisplay.activationTarget2, editingDisplay.activationTargetOptions);
            pos.y += ImagePack.fieldHeight;
            selectedEffect = GetOptionPosition(editingDisplay.activationEffect2, effectIds);
            selectedEffect = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Act. Effect")+" 2:", ref effect2SearchInput, selectedEffect, effectOptions);
            editingDisplay.activationEffect2 = effectIds[selectedEffect];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.activationTarget3 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Act. Target")+" 3:", editingDisplay.activationTarget3, editingDisplay.activationTargetOptions);
            pos.y += ImagePack.fieldHeight;
            selectedEffect = GetOptionPosition(editingDisplay.activationEffect3, effectIds);
            selectedEffect = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Act. Effect")+" 3:", ref effect3SearchInput, selectedEffect, effectOptions);
            editingDisplay.activationEffect3 = effectIds[selectedEffect];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.activationTarget4 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Act. Target")+" 4:", editingDisplay.activationTarget4, editingDisplay.activationTargetOptions);
            pos.y += ImagePack.fieldHeight;
            selectedEffect = GetOptionPosition(editingDisplay.activationEffect4, effectIds);
            selectedEffect = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Act. Effect")+" 4:", ref effect4SearchInput, selectedEffect, effectOptions);
            editingDisplay.activationEffect4 = effectIds[selectedEffect];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.activationTarget5 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Act. Target")+" 5:", editingDisplay.activationTarget5, editingDisplay.activationTargetOptions);
            pos.y += ImagePack.fieldHeight;
            selectedEffect = GetOptionPosition(editingDisplay.activationEffect5, effectIds);
            selectedEffect = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Act. Effect")+" 5:", ref effect5SearchInput, selectedEffect, effectOptions);
            editingDisplay.activationEffect5 = effectIds[selectedEffect];
            pos.y += ImagePack.fieldHeight;
            editingDisplay.activationTarget6 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Act. Target")+" 6:", editingDisplay.activationTarget6, editingDisplay.activationTargetOptions);
            pos.y += ImagePack.fieldHeight;
            selectedEffect = GetOptionPosition(editingDisplay.activationEffect6, effectIds);
            selectedEffect = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Act. Effect")+" 6:", ref effect6SearchInput, selectedEffect, effectOptions);
            editingDisplay.activationEffect6 = effectIds[selectedEffect];
            pos.y += 1.5f * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Coordinated Effects"));
            pos.y += 1.5f * ImagePack.fieldHeight;
            if (editingDisplay.aoeType == "LocationRadius")
            {
                //   pos.width *= 2;
                pos.y += ImagePack.fieldHeight;
           //     editingDisplay.aoeCoordEffect = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Aoe Coord. Effect"), ref aoeCordSearch, editingDisplay.aoeCoordEffect, coordList);
                pos.y += ImagePack.fieldHeight;
            //    editingDisplay.activationDelay = ImagePack.DrawField(pos, Lang.GetTranslate("AoE Activation Delay (s)") + ":", editingDisplay.activationDelay);
            //    pos.y += ImagePack.fieldHeight;
                // pos.width /= 2;
            }
            editingDisplay.coordEffect1Event = ImagePack.DrawSelector(pos, Lang.GetTranslate("Effect Event")+" 1:", editingDisplay.coordEffect1Event, editingDisplay.coordEffect1EventOptions);
            //pos.x += pos.width;
            //editingDisplay.coordEffect1 = ImagePack.DrawField (pos, "Coord. Effect1:", editingDisplay.coordEffect1);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.coordEffect1 = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Coord. Effect")+" 1:", ref cordSearch1, editingDisplay.coordEffect1, coordList);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.coordEffect2Event = ImagePack.DrawSelector(pos, Lang.GetTranslate("Effect Event")+" 2:", editingDisplay.coordEffect2Event, editingDisplay.coordEffect2EventOptions);
            //pos.x += pos.width;
            //editingDisplay.coordEffect2 = ImagePack.DrawField (pos, "Coord. Effect2:", editingDisplay.coordEffect2);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.coordEffect2 = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Coord. Effect")+" 2:", ref cordSearch2, editingDisplay.coordEffect2, coordList);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.coordEffect3Event = ImagePack.DrawSelector(pos, Lang.GetTranslate("Effect Event") + " 3:", editingDisplay.coordEffect3Event, editingDisplay.coordEffect3EventOptions);
            //pos.x += pos.width;
            //editingDisplay.coordEffect1 = ImagePack.DrawField (pos, "Coord. Effect1:", editingDisplay.coordEffect1);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.coordEffect3 = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Coord. Effect") + " 3:", ref cordSearch3, editingDisplay.coordEffect3, coordList);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.coordEffect4Event = ImagePack.DrawSelector(pos, Lang.GetTranslate("Effect Event") + " 4:", editingDisplay.coordEffect4Event, editingDisplay.coordEffect3EventOptions);
            //pos.x += pos.width;
            //editingDisplay.coordEffect1 = ImagePack.DrawField (pos, "Coord. Effect1:", editingDisplay.coordEffect1);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.coordEffect4 = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Coord. Effect") + " 4:", ref cordSearch4, editingDisplay.coordEffect4, coordList);
            //pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.coordEffect5Event = ImagePack.DrawSelector(pos, Lang.GetTranslate("Effect Event") + " 5:", editingDisplay.coordEffect5Event, editingDisplay.coordEffect3EventOptions);
            //pos.x += pos.width;
            //editingDisplay.coordEffect1 = ImagePack.DrawField (pos, "Coord. Effect1:", editingDisplay.coordEffect1);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.coordEffect5 = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Coord. Effect") + " 5:", ref cordSearch5, editingDisplay.coordEffect5, coordList);
          


            pos.width *= 2;
            pos.y += 1.5f * ImagePack.fieldHeight;
            GUI.Label(pos, Lang.GetTranslate("Description")+":", ImagePack.FieldStyle());
            pos.height *= 2;
            tooltipScroll = GUI.BeginScrollView(pos, tooltipScroll, new Rect(0, 0, pos.width * 0.75f, 100));
            editingDisplay.tooltip = EditorGUI.TextArea(new Rect(115, 0, pos.width * 0.75f, 100), editingDisplay.tooltip, ImagePack.TextAreaStyle());
            GUI.EndScrollView();
            pos.height /= 2;
            pos.width /= 2;
            pos.y += ImagePack.fieldHeight * 2.2F;

            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Ability Tags"));
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
            pos.y += 2.2f * ImagePack.fieldHeight;
            pos.width *= 2;
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
                EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight + 150);
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
                // Update online table to avoid access the database again			
                editingDisplay.id = itemID;
                editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
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

            // Update online table to avoid access the database again			
        
            // Configure the correponding prefab
            CreatePrefab();
            dataLoaded = false;
             NewResult(Lang.GetTranslate("Entry")+"  " + editingDisplay.Name + " "+ Lang.GetTranslate("updated"));
            Load();
        }

        // Delete entries from the table
        void DeleteEntry()
        {

            //Check Effects
            string sqlSets = "SELECT skillID FROM skill_ability_gain WHERE abilityID=" + editingDisplay.id + " AND isactive=1";
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlSets);
            int abilitiesCount = 0;
            string abilitiesIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                abilitiesIds = "\n"+ Lang.GetTranslate("Skills Ids")+":";
                abilitiesCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    abilitiesIds += " " + data["skillID"] + ", ";
                }
            }

            //Check Items Effects
            string sqlItems = "SELECT id FROM item_templates WHERE (effect1type like 'UseAbility' AND effect1value =" + editingDisplay.id + " )";
            for (int ii = 2; ii <= 16; ii++)
            {
                sqlItems += " OR (effect"+ii+"type like 'UseAbility' AND effect"+ii+"value =" + editingDisplay.id + " )";
            }
            
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, sqlItems);
            int itemsCount = 0;
            string itemsIds = "";
            if ((rows != null) && (rows.Count > 0))
            {
                itemsIds = "\n"+ Lang.GetTranslate("Items Ids")+":";
                itemsCount = rows.Count;
                foreach (Dictionary<string, string> data in rows)
                {
                    itemsIds += " " + data["id"] + ",";
                }
            }

            DatabasePack.SaveLog("Delete Instance id =" + editingDisplay.id + " assigned to " + abilitiesIds + itemsIds );

            if (abilitiesCount == 0 && itemsCount == 0)
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
                NewResult(Lang.GetTranslate("Entry")+" " + editingDisplay.Name + " "+ Lang.GetTranslate("Deleted"));
            }
            else
            {
                EditorUtility.DisplayDialog("", Lang.GetTranslate("You can not delete Ability because it is assigned in")+":" + abilitiesIds + itemsIds, Lang.GetTranslate("OK"), "");
            }
            dataLoaded = false;
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
            dataLoaded = false;
            Load();
            state = State.Loaded;
        }

        void GenerateAllPrefabs()
        {
           /* if (!dataLoaded)
            {
                Load();
            }

            // Delete all existing prefabs?
            AbilityPrefab.DeleteAllPrefabs();

            foreach (int id in displayKeys)
            {
                AbilitiesData abilityData = LoadEntity(id);
                prefab = new AbilityPrefab(abilityData);
                prefab.Save(abilityData);
            }*/
           
        }

        void CreatePrefab()
        {
            // Configure the correponding prefab
        //    prefab = new AbilityPrefab(editingDisplay);
         //   prefab.Save(editingDisplay);
        }

        void DeletePrefab()
        {
         /*   prefab = new AbilityPrefab(editingDisplay);

            if (prefab.Load())
                prefab.Delete();*/
        }
    }
}