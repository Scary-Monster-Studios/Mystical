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
    // Handles the Item Configuration
    public class ServerSkillProfile : AtavismDatabaseFunction
    {

        enum SkillProfileView
        {
            Home,
            CraftProfile,
            GatheringProgile,
            CombatProfile
        }

        public new Dictionary<int, SkillLevelProfileData> dataRegister;
        public new SkillLevelProfileData editingDisplay;
        public new SkillLevelProfileData originalDisplay;
        public QualityData quality;

        public int[] currencyIds = new int[] { -1 };
        public string[] currencyOptions = new string[] { "~ none ~" };

        public string[] damageOptions = new string[] { "~ none ~" };

        public string[] itemEffectTypeOptions = new string[] { "~ none ~" };
        public string[] statOptions = new string[] { "~ none ~" };
        public string[] accountEffectTypeOptions = new string[] { "~ none ~" };
        SkillProfileView view = SkillProfileView.Home;
        public string[] itemQualityOptions = new string[] { "~ none ~" };

        // Handles the prefab creation, editing and save
       // private ItemPrefab item = null;

        // Filter/Search inputs
        //private string currencySearchInput = "";
        //private List<string> effectSearchInput = new List<string>();

        // Use this for initialization
        public ServerSkillProfile()
        {
            functionName = "Skill Profiles";
            // Database tables name
            tableName = "skill_profile";
            functionTitle = "Skill Profile Configuration";
            loadButtonLabel = "Load Skill Profile";
            notLoadedText = "No Skill Profile loaded.";
            // Init
            dataRegister = new Dictionary<int, SkillLevelProfileData>();

            editingDisplay = new SkillLevelProfileData();
            originalDisplay = new SkillLevelProfileData();
        }

        public override void Activate()
        {
            linkedTablesLoaded = false;
            resetSearch(true);
        }
        void resetSearch(bool fokus)
        {
           if(fokus) GUI.FocusControl(null);
        }



        public void LoadCurrencyOptions()
        {
            string query = "SELECT id, name FROM currencies where isactive = 1";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                currencyOptions = new string[rows.Count + 1];
                //currencyOptions [optionsId] = "~ none ~"; 
                currencyIds = new int[rows.Count + 1];
                //currencyIds [optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    currencyOptions[optionsId] = data["id"] + ":" + data["name"];
                    currencyIds[optionsId] = int.Parse(data["id"]);
                }
            }
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
                    statOptions = new string[rows.Count + 1];
                    statOptions[optionsId] = "~ none ~";
                    foreach (Dictionary<string, string> data in rows)
                    {
                        optionsId++;
                        statOptions[optionsId] = data["name"];
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
                int type = 0;
                if (view.Equals(SkillProfileView.CraftProfile))
                    type = 0;
                if (view.Equals(SkillProfileView.CombatProfile))
                    type = 1;
                if (view.Equals(SkillProfileView.GatheringProgile))
                    type = 2;

                // Read all entries from the table
                string query = "SELECT id,profile_name FROM " + tableName + " where isactive = 1 AND type ="+type;

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
                        displayList.Add(data["id"] + ". " + data["profile_name"]);
                        displayKeys.Add(int.Parse(data["id"]));

                    }
                }
                dataLoaded = true;
            }
        }

      
        SkillLevelProfileData LoadEntity(int id)
        {
            
         //   SkillLevelProfileData slpd = new SkillLevelProfileData();
            // Read all entries from the table
            string query = "SELECT * FROM " + tableName + " where isactive = 1 AND  id =" + id;
            //  Debug.LogError(query);
            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            SkillLevelProfileData display = new SkillLevelProfileData();

            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    //  foreach(string key in data.Keys)
                    //  	Debug.Log("Name[" + key + "]:" + data[key]);
                    //return;

                    display.id = int.Parse(data["id"]);
                    display.profileName = data["profile_name"];
                    display.type = int.Parse(data["type"]);
                    //display = int.Parse(data ["level"]);
                    string levelDiff = data["level_diff"];
                    if (levelDiff.Length > 0)
                    {
                        string[] levs = levelDiff.Split(';');
                        for (int i = 0; i < levs.Length; i++)
                        {
                            SkillLevelDiffData entry = new SkillLevelDiffData(float.Parse(levs[i]));
                            display.levelDiffExp.Add(entry);

                        }
                    }
                    display.isLoaded = true;
                    //Debug.Log("Name:" + display.name  + "=[" +  display.id  + "]");
               }
            }
          LoadProfileLevels(display);
         return display;
        }

        void LoadProfileLevels(SkillLevelProfileData profile)
        {
            string query = "SELECT * FROM skill_profile_levels where profile_id = " + profile.id + " AND isactive = 1";
            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    SkillLevelData entry = new SkillLevelData();
                    entry.id = int.Parse(data["id"]);
                    entry.profile_id = int.Parse(data["profile_id"]);
                    entry.level = int.Parse(data["level"]);
                    entry.xpRequired = int.Parse(data["required_xp"]);
                    profile.levelExp.Add(entry);
                }
            }
        }


        public override void LoadRestore()
        {
            if (!dataRestoreLoaded)
            {
                // Clean old data
                dataRegister.Clear();
                displayKeys.Clear();
                /*  int type = 0;
                  if (view.Equals(SkillProfileView.CraftProfile))
                      type = 0;
                  if (view.Equals(SkillProfileView.CombatProfile))
                      type = 1;
                  if (view.Equals(SkillProfileView.GatheringProgile))
                      type = 2;
                 */
                // Read all entries from the table
                string query = "SELECT id,profile_name FROM " + tableName + " where isactive = 0 " ;

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
                        displayList.Add(data["id"] + ". " + data["profile_name"]);
                        displayKeys.Add(int.Parse(data["id"]));

                    }
                }
                dataRestoreLoaded = true;
            }
        }


/*
        public void LoadSelectList()
        {
          //  displayList = new string[dataRegister.Count];
            int i = 0;
            foreach (int displayID in dataRegister.Keys)
            {
                displayList[i] = displayID + ". " + dataRegister[displayID].profileName;
                i++;
            }
        }
        */

        private int SelectTab(Rect pos, int sel)
        {
            pos.y += ImagePack.tabTop;
            pos.x += ImagePack.tabLeft;
            bool create = false;
            bool edit = false;
            bool doc = false;
            bool restore = false;

            switch (sel)
            {
                case 0:
                    //   create = true;
                    break;
                case 1:
                    edit = true;
                    break;
                case 2:
                    doc = true;
                    break;
                case 3:
                    restore = true;
                    break;
            }
          //  if (view == SkillProfileView.CraftProfile)
          //  {
                if (create)
                    pos.y += ImagePack.tabSpace;
                if (ImagePack.DrawTabCreate(pos, create))
                    return 0;
                if (create)
                    pos.y -= ImagePack.tabSpace;
                pos.x += ImagePack.tabMargin;
         //   }
            pos.x += ImagePack.tabMargin;
            if (edit)
                pos.y += ImagePack.tabSpace;
            if (ImagePack.DrawTabEdit(pos, edit))
                return 1;
            if (edit)
                pos.y -= ImagePack.tabSpace;
            pos.x += ImagePack.tabMargin;
            if (doc)
                pos.y += ImagePack.tabSpace;
            if (ImagePack.DrawTabDoc(pos, doc))
                return 2;
            if (doc)
                pos.y -= ImagePack.tabSpace;
            if (showRecovery)
            {
                pos.x += ImagePack.tabMargin;
                if (restore)
                    pos.y += ImagePack.tabSpace;
                if (ImagePack.DrawTabRestore(pos, restore))
                    return 3;
                if (restore)
                    pos.y -= ImagePack.tabSpace;
            }
            return sel;
        }


        public override void Draw(Rect box)
        {

            // Setup the layout
            Rect pos = box;


            if (view == SkillProfileView.Home)
            {
                pos.x += ImagePack.innerMargin;
                pos.y += ImagePack.innerMargin;
                pos.width -= ImagePack.innerMargin;
                pos.height = ImagePack.fieldHeight;
                DrawHome(pos);
                dataLoaded = false;
            }
            else if (view == SkillProfileView.CraftProfile)
            {
                base.Draw(pos);

                //  DrawLoaded(pos);
            }
            else if (view == SkillProfileView.GatheringProgile)
            {
                base.Draw(pos);
            }
            else if (view == SkillProfileView.CombatProfile)
            {
                base.Draw(pos);
            }
            /*
            else if (view == EnchantView.Graveyards)
            {
                DrawGraveyards(pos);
            }
            else if (view == EnchantView.Claims)
            {
                DrawClaims(pos);
            }*/
        }

        void DrawHome(Rect pos)
        {
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Skill Profiles"));
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Crafting Profile")))
            {
                view = SkillProfileView.CraftProfile;
                linkedTablesLoaded = false;
                dataLoaded = false;
                Load();
            }
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Combat Profile")))
            {
                view = SkillProfileView.CombatProfile;
                linkedTablesLoaded = false;
                dataLoaded = false;
                Load();
            }
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Gathering Profile")))
            {
                view = SkillProfileView.GatheringProgile;
                linkedTablesLoaded = false;
                dataLoaded = false;
                Load();
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
            pos.width /= 2;
            pos.x += pos.width;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Back")))
            {
                view = SkillProfileView.Home;
            }
            pos.x -= pos.width;
            pos.width *= 2;

            //if (view.Equals(SkillProfileView.CraftProfile))
            //  {
            if (displayKeys.Count <= 0)
            {
                pos.y += ImagePack.fieldHeight;
                if (view.Equals(SkillProfileView.CraftProfile))
                    ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create an Crafting Profile before edit it."));
                if (view.Equals(SkillProfileView.CombatProfile))
                    ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create an Combat Profile before edit it."));
                if (view.Equals(SkillProfileView.GatheringProgile))
                    ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("You must create an Gathering Profile before edit it."));
               
                return;
            }
            if (view.Equals(SkillProfileView.CraftProfile))
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Crafting Profile"));
            }
            if (view.Equals(SkillProfileView.CombatProfile))
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Combat Profile"));
            }
            if (view.Equals(SkillProfileView.GatheringProgile))
            {
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Edit Gathering Profile"));
            }

            // Draw the content database info
            if (newItemCreated)
                {
                    newItemCreated = false;
                    newSelectedDisplay = displayKeys.Count - 1;
                }

                // Draw data Editor
                if (newSelectedDisplay != selectedDisplay)
                {
                    selectedDisplay = newSelectedDisplay;
                    int displayKey = displayKeys[selectedDisplay];
                    editingDisplay = LoadEntity(displayKey);
                    //originalDisplay = editingDisplay.Clone();
                }
                pos.y += ImagePack.fieldHeight * 1.5f;
                pos.x -= ImagePack.innerMargin;
                pos.y -= ImagePack.innerMargin;
                pos.width += ImagePack.innerMargin;
         //   }



            if (state != State.Loaded /*&& view.Equals(SkillProfileView.CraftProfile)*/)
            {
                pos.x += ImagePack.innerMargin;
                pos.width /= 2;
                //Draw super magical compound object.
                newSelectedDisplay = ImagePack.DrawDynamicPartialListSelector(pos, Lang.GetTranslate("Search Filter")+": ", ref entryFilterInput, selectedDisplay, displayList);

                pos.y += ImagePack.fieldHeight;
                //Build prefabs button
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Duplicate")))
                {
                    Duplicate();
                }
                pos.x -= pos.width;

                pos.width *= 2;
                pos.y += ImagePack.fieldHeight * 1.5f;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Profile Properties:"));
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
            if (view.Equals(SkillProfileView.CraftProfile))
            {
                editingDisplay = new SkillLevelProfileData();
                originalDisplay = new SkillLevelProfileData();
            }
            selectedDisplay = -1;
        }

        bool hideLevels = false;
        // Edit or Create
        public override void DrawEditor(Rect box, bool newItem)
        {
            newEntity = newItem;
            // Setup the layout
            Rect pos = box;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;
            if (state.Equals(State.New))
            {
                pos.width /= 2;
                pos.x += pos.width;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Back")))
                {
                    view = SkillProfileView.Home;
                }
                pos.x -= pos.width;
                pos.width *= 2;
                pos.y += 2f * ImagePack.innerMargin;
            }
            if (view.Equals(SkillProfileView.CraftProfile) || view.Equals(SkillProfileView.CombatProfile) || view.Equals(SkillProfileView.GatheringProgile))
            {
                if (!linkedTablesLoaded)
                {
                    LoadCurrencyOptions();
                    LoadStatOptions();
                    itemQualityOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Item Quality", false);
                    linkedTablesLoaded = true;
                }

                // Draw the content database info
                if (newEntity)
                {
                    if (view.Equals(SkillProfileView.CraftProfile))
                        ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new crafting profile"));
                    if (view.Equals(SkillProfileView.CombatProfile))
                        ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new combat profile"));
                    if (view.Equals(SkillProfileView.GatheringProgile))
                        ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Create a new gathering profile"));
                    pos.y += ImagePack.fieldHeight;
                }
                int type = 0;
                if (view.Equals(SkillProfileView.CraftProfile))
                    type = 0;
                if (view.Equals(SkillProfileView.CombatProfile))
                    type = 1;
                if (view.Equals(SkillProfileView.GatheringProgile))
                    type = 2;

                editingDisplay.type = type;

                editingDisplay.profileName = ImagePack.DrawField(pos, Lang.GetTranslate("Name")+":", editingDisplay.profileName, 0.8f);
                pos.y += ImagePack.fieldHeight;
                editingDisplay.generate_to_level = ImagePack.DrawField(pos, Lang.GetTranslate("Generate XP levels")+":", editingDisplay.generate_to_level);
                pos.y += ImagePack.fieldHeight;
                editingDisplay.generate_base_value = ImagePack.DrawField(pos, Lang.GetTranslate("Base XP value")+":", editingDisplay.generate_base_value);
                pos.y += ImagePack.fieldHeight;
                editingDisplay.generate_percentage_value = ImagePack.DrawField(pos, Lang.GetTranslate("Percentage XP/lev (%)")+":", editingDisplay.generate_percentage_value);
                pos.y += ImagePack.fieldHeight;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Generate levels")))
                {
                    editingDisplay.levelExp.Clear();
                    for (int ii = 0; ii < editingDisplay.generate_to_level; ii++)
                    {
                        SkillLevelData lev = new SkillLevelData();
                        lev.level = ii + 1;
                        if (ii == 0)
                        {
                            lev.xpRequired = editingDisplay.generate_base_value;
                        }
                        else
                        {
                            int xp = ((editingDisplay.levelExp[ii - 1].xpRequired + Mathf.RoundToInt(editingDisplay.levelExp[ii - 1].xpRequired * (editingDisplay.generate_percentage_value / 100f))));
                            if (xp < editingDisplay.levelExp[ii - 1].xpRequired) xp = 2000000000;
                            lev.xpRequired = xp;
                        }
                        editingDisplay.levelExp.Add(lev);
                    }
                }
                if (editingDisplay.levelExp.Count > 0)
                {
                    pos.width /= 3;
                    pos.x += pos.width * 2;
                    if (hideLevels)
                    {
                        if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Show levels")+" (" + editingDisplay.levelExp.Count + ")"))
                        {
                            hideLevels = false;
                        }
                    }
                    else
                    {
                        if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Hide Levels")))
                        {
                            hideLevels = true;
                        }
                    }
                    pos.x -= pos.width * 2;
                    pos.width *= 3;
                }
                pos.y += ImagePack.fieldHeight;
                if (!hideLevels)
                    for (int j = 0; j < editingDisplay.levelExp.Count; j++)
                    {
                        editingDisplay.levelExp[j].id = editingDisplay.id;
                        editingDisplay.levelExp[j].xpRequired = ImagePack.DrawField(pos, Lang.GetTranslate("Xp for level")+" " + (j + 2) + ":", editingDisplay.levelExp[j].xpRequired);
                        pos.y += ImagePack.fieldHeight;
                        pos.width /= 3;
                        if (editingDisplay.levelExp.Count - 1 == j)
                        {
                            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add XP level")))
                            {
                                SkillLevelData lev = new SkillLevelData();
                                lev.level = editingDisplay.levelExp.Count;
                                editingDisplay.levelExp.Add(lev);
                            }
                            pos.x += pos.width;
                            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove last level")))
                            {
                                editingDisplay.levelExp.RemoveAt(j);
                            }
                            pos.x += pos.width;
                            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Hide Levels")))
                            {
                                hideLevels = true;
                            }
                            pos.x -= pos.width * 2;
                        }
                        pos.width *= 3;
                    }
                // pos.y += ImagePack.fieldHeight;
                if (editingDisplay.levelExp.Count == 0)
                {
                    //    pos.y += ImagePack.fieldHeight;
                    pos.width /= 3;
                    if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add XP level")))
                    {
                        SkillLevelData lev = new SkillLevelData();
                        lev.level = editingDisplay.levelExp.Count;
                        editingDisplay.levelExp.Add(lev);
                    }
                    pos.width *= 3;
                }
                pos.y += ImagePack.fieldHeight;
               
                if (view.Equals(SkillProfileView.CraftProfile)|| view.Equals(SkillProfileView.GatheringProgile))
                {
                    if (view.Equals(SkillProfileView.CraftProfile))
                        ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Crafting Level diff % XP"));
                    if (view.Equals(SkillProfileView.GatheringProgile))
                        ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Gathering Level diff % XP"));

                    for (int j = 0; j < editingDisplay.levelDiffExp.Count; j++)
                    {
                        pos.y += ImagePack.fieldHeight;
                        editingDisplay.levelDiffExp[j].id = editingDisplay.id;
                        editingDisplay.levelDiffExp[j].xpPercantage = ImagePack.DrawField(pos, Lang.GetTranslate("level diff")+" " + (j + 1) + " "+ Lang.GetTranslate("XP")+" % :", editingDisplay.levelDiffExp[j].xpPercantage);
                        pos.width /= 2;
                        if (editingDisplay.levelDiffExp.Count - 1 == j)
                        {   //  pos.y += ImagePack.fieldHeight;
                            pos.y += ImagePack.fieldHeight;
                            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add level diff")))
                            {
                                SkillLevelDiffData lev = new SkillLevelDiffData();
                                /// lev.level = editingDisplay.levelDiffExp.Count;
                                editingDisplay.levelDiffExp.Add(lev);
                            }
                            pos.x += pos.width;
                            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Remove level diff")))
                            {
                                editingDisplay.levelDiffExp.RemoveAt(j);
                            }
                            pos.x -= pos.width;
                        }
                        pos.width *= 2;
                    }
                    if (editingDisplay.levelDiffExp.Count == 0)
                    {
                        pos.width /= 2;
                        pos.y += ImagePack.fieldHeight;
                        if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add level diff")))
                        {
                            SkillLevelDiffData lev = new SkillLevelDiffData();
                            /// lev.level = editingDisplay.levelDiffExp.Count;
                            editingDisplay.levelDiffExp.Add(lev);
                        }
                        pos.width *= 2;
                    }
                }
                // Save data		
                pos.x -= ImagePack.innerMargin;
                pos.y += ImagePack.fieldHeight;
                if (!newEntity)
                {
                    Color temp = GUI.color;
                    GUI.color = Color.red;
                    //  ImagePack.DrawText(pos, "      Attention, erasing is permanent !!! ");
                    GUI.color = temp;
                    pos.y += ImagePack.fieldHeight;
                }
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
         /*   if (view.Equals(SkillProfileView.CombatProfile) && !newEntity)
            {
                if (!linkedTablesLoaded)
                {
                    itemQualityOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Item Quality", false);
                    linkedTablesLoaded = true;
                }
                // Draw the content database info
                ImagePack.DrawLabel(pos.x, pos.y, "Item Quality Settings");
                pos.y += ImagePack.fieldHeight;
                for (int j = 0; j < itemQualityOptions.Length; j++)
                {
                    if (quality.list.Count < j + 1)
                    {
                        quality.list.Add(new QualityEntry(itemQualityOptions[j]));
                    }
                    ImagePack.DrawLabel(pos.x, pos.y, itemQualityOptions[j]);
                    pos.y += ImagePack.fieldHeight;
                    pos.width = pos.width / 2;
                    quality.list[j].chance = ImagePack.DrawField(pos, "Chance (%):", quality.list[j].chance);
                    pos.x += pos.width;
                    quality.list[j].cost = ImagePack.DrawField(pos, "Cost (%): ", quality.list[j].cost);
                    pos.x -= pos.width;
                    pos.width = pos.width * 2;
                    pos.y += ImagePack.fieldHeight;
                }
                // Save data		
                pos.x -= ImagePack.innerMargin;
                pos.y += ImagePack.fieldHeight;
                pos.width /= 3;
                showSave = true;
                showDelete = false;
                showCancel = false;

                if (resultTimeout != -1 && resultTimeout < Time.realtimeSinceStartup && result != "")
                {
                    result = "";
                }
                EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight);
            }*/
        }


        public override void save()
        {
            if (newEntity)
                InsertEntry();
            else
                UpdateEntry();

            state = State.Loaded;
            linkedTablesLoaded = false;

            resetSearch(true);
        }

        public override void delete()
        {
            if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete")+" " + Lang.GetTranslate(functionName) + " "+ Lang.GetTranslate("entry")+" ?", Lang.GetTranslate("Are you sure you want to delete")+" " + editingDisplay.Name + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
            {
                DeleteEntry();
                newSelectedDisplay = 0;
                state = State.Loaded;
                linkedTablesLoaded = false;

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
      
            int itemID = -1;
         
            NewResult(Lang.GetTranslate("Inserting..."));
            // Setup the update query
            string query = "INSERT INTO " + tableName;
            query += " (" + editingDisplay.FieldList("", ", ") + ") ";
            query += "VALUES ";
            query += " (" + editingDisplay.FieldList("?", ", ") + ") ";
            //          Debug.LogError("sql:  " + query);

            // Setup the register data		
            List<Register> update = new List<Register>();
            foreach (string field in editingDisplay.fields.Keys)
            {
                update.Add(editingDisplay.fieldToRegister(field));
            }
            //update.Add(new Register("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString(), Register.TypesOfField.Int));

            // Update the database
            itemID = DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);
            // editingDisplay.enchantLevels.
            // If the insert failed, don't insert the spawn marker
            //   }
            if (itemID != -1)
            {
                // Update online table to avoid access the database again			
                editingDisplay.id = itemID;
                // Insert the Requirements
                foreach (SkillLevelData entry in editingDisplay.levelExp)
                {
                    entry.profile_id = itemID;
                    InsertRequirement(entry);

                }
                editingDisplay.isLoaded = true;
                //Debug.Log("ID:" + itemID + "ID2:" + editingDisplay.id);
                dataLoaded = false;
                Load();
                newItemCreated = true;
                NewResult(Lang.GetTranslate("New entry inserted"));
            }
            else
            {
                NewResult("Error:"+Lang.GetTranslate("Error occurred, please check the Console"));
            }

        }

        void InsertRequirement(SkillLevelData entry)
        {
            string query = "INSERT INTO skill_profile_levels";
            query += " (" + entry.FieldList("", ", ") + ") ";
            query += "VALUES ";
            query += " (" + entry.FieldList("?", ", ") + ") ";
            // Debug.Log(query);
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

            string query2 = "DELETE FROM skill_profile_levels WHERE profile_id=?id";
            // Setup the register data		
            List<Register> update1 = new List<Register>();
            update1.Add(new Register("id", "?id", MySqlDbType.Int32, editingDisplay.id.ToString(), Register.TypesOfField.Int));
            // Update the database
            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query2, update1);

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

            foreach (SkillLevelData entry in editingDisplay.levelExp)
            {
                entry.profile_id = editingDisplay.id;
                InsertRequirement(entry);
            }

            dataLoaded = false;
            NewResult(Lang.GetTranslate("Entry")+"  " + editingDisplay.Name + " "+ Lang.GetTranslate("updated"));
              Load();
      }


        void UpdateGrade()
        {
            NewResult(Lang.GetTranslate("Updating grade..."));
            List<Register> update = new List<Register>();
            //      Debug.LogError(quality.list.Count);
            foreach (QualityEntry qe in quality.list)
            {
                if (qe.id.Equals(-1))
                {
                    string query1 = "SELECT MAX(id) as id FROM item_quality";
                    if (rows != null)
                        rows.Clear();
                    rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query1);
                    //         Debug.Log("#Rows:" + rows.Count);
                    //         Debug.Log("#Rows:" + rows[0]);

                    int plid = 0;
                    if (rows != null && rows.Count > 0 && rows[0].ContainsKey("id") && !String.IsNullOrEmpty(rows[0]["id"]))
                    {
                        plid = int.Parse(rows[0]["id"]);
                    }
                    //       Debug.Log("#plid:" + plid);
                    string query = "INSERT INTO item_quality (`id`,`name`,`cost`,`chance`)  VALUES (" + (plid + 1) + ",'" + qe.Name + "'," + qe.cost + "," + qe.chance + ") ";
                    //       Debug.Log("#query:" + query);
                    DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, update);
                }
                else
                {
                    string query = "UPDATE item_quality  SET cost=" + qe.cost + ", chance=" + qe.chance + " WHERE id=" + qe.id;
                    //      Debug.Log("#query:" + query);
                    DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, update);
                }
            }

            NewResult(Lang.GetTranslate("Grades updated"));
        }


        // Delete entries from the table
        void DeleteEntry()
        {

            //string query = "DELETE FROM " + tableName + " WHERE id="+ editingDisplay.id;
            string query = "UPDATE " + tableName + " SET isactive = 0 where id=" + editingDisplay.id;

            DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

            /* string query2 = "DELETE FROM skill_profile_levels WHERE profile_id=" + editingDisplay.id.ToString();
             DatabasePack.Update(DatabasePack.contentDatabasePrefix, query2, new List<Register>());
             */

            // Update online table to avoid access the database again		
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

                where.Clear();
                where.Add(new Register("profile_id", "?profile_id", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "skill_profile_levels", where);

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
            editingDisplay.profileName = editingDisplay.profileName + " (Clone)";
            editingDisplay.id = -1;
            InsertEntry();
            state = State.Loaded;
            linkedTablesLoaded = false;
            dataLoaded = false;
            Load();
        }



    }
}