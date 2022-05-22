using UnityEngine;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Atavism {
    // This is the base class for Atavism Database Access Functions
    public class AtavismDatabaseFunction : AtavismFunction
    {
        // Database Configuration States
        public enum State
        {
            Loading,
            Loaded,
            Edit,
            New,
            Doc,
            Restore
        }

        // State of editing
        public State state = State.New;

        // command object
        public MySqlCommand cmd = null;

        // Store database row fetchs
        public List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();

        // Database tables name
        public string tableName = "";
        public string functionTitle = "Configuration";
        public string loadButtonLabel = "Load";
        public string notLoadedText = "Not loaded.";

        // Dictionary to handle data fields
        public Dictionary<int, DataStructure> dataRegister;
        public List<int> displayKeys = new List<int>();

        // Stores the selected register
        public int selectedDisplay = -1;
        public int newSelectedDisplay = 0;

        public DataStructure editingDisplay;
        public DataStructure originalDisplay;

        public bool dataLoaded = false;
        public bool dataRestoreLoaded = false;
        public bool newItemCreated = false;
        public bool dataSaved = false;
        public bool showRecovery = true;
        public bool showCreate = true;
        // Used to load in data from other tables if set to false
        public bool linkedTablesLoaded = false;

        public Vector2 inspectorScrollPosition = Vector2.zero;
        public float inspectorHeight = 0;

        //public Combobox displayList;
        public List<string> displayList = new List<string>();

        // Tab selection
        public int selected = 0;

        // Filter input
        protected string entryFilterInput = "";

        protected bool newEntity = false;

        /// <summary>
        /// Enables the scroll bar and sets total window height
        /// </summary>
        /// <param name="windowHeight">Window height.</param>
        public void EnableScrollBar(float windowHeight)
        {
            inspectorHeight = windowHeight;
        }

        private int SelectTab(Rect pos, int sel)
        {
            pos.y += ImagePack.tabTop;
            // pos.x += ImagePack.tabLeft;
            if (showRecovery)
            {
                if(showCreate)
                    pos.x += pos.width - ImagePack.tabMargin * 4;
                else
                    pos.x += pos.width - ImagePack.tabMargin * 3;
            }
            else
            {
                if (showCreate)
                    pos.x += pos.width - ImagePack.tabMargin * 3;
                else
                    pos.x += pos.width - ImagePack.tabMargin * 2;
            }
            bool create = false;
            bool edit = false;
            bool doc = false;
            bool restore = false;
            if (!showCreate && sel == 0)
                sel = 1;
                
            switch (sel)
            {
                case 0:
                    create = true;
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
            if (showCreate)
            {
                if (create)
                    pos.y += ImagePack.tabSpace;
                if (ImagePack.DrawTabCreate(pos, create))
                    return 0;
                if (create)
                    pos.y -= ImagePack.tabSpace;
                pos.x += ImagePack.tabMargin;
            }
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

        // Draw the user interface
        public override void Draw(Rect box)
        {
            //ImagePack.DrawScrollBar (box.x + box.width, box.y, box.height - 14);

            // Draw the Control Tabs
            int newSelected = SelectTab(box, selected);

            if (newSelected != selected)
            {
                selected = newSelected;
                switch (selected)
                {
                    case 0: // Create New
                        CreateNewData();
                        state = State.New;
                        break;
                    case 1: // Edit
                        state = State.Loading;
                        break;
                    case 2: // Documentation
                        state = State.Doc;
                        break;
                    case 3: // Restore
                        state = State.Restore;
                        break;
                }
            }

            Rect inspectorScrollWindow = box;
            Rect inspectorWindow = box;
            inspectorWindow.width -= 2;
            inspectorScrollWindow.width += 14;
            inspectorWindow.height = Mathf.Max(box.height, inspectorHeight);

            // Switch between editing states
            switch (state)
            {
                case State.Loading:
                    // Load database information
                    selected = 1;
                    state = State.Loaded;
                    Load();
                    dataSaved = false;
                    break;
                case State.Loaded:
                    // After loading, shows
                    if (dataLoaded)
                    {
                        state = State.Edit;
                    }
                    else
                    {
                        state = State.Loading;
                    }
                    break;
                case State.Edit:
                    // Editing register
                    selected = 1;
                    dataRestoreLoaded = false;
                    inspectorScrollPosition = GUI.BeginScrollView(inspectorScrollWindow, inspectorScrollPosition, inspectorWindow);
                    DrawLoaded(box);
                    GUI.EndScrollView();
                    break;
                case State.New:
                    // Create a new register
                    selected = 0;
                    dataRestoreLoaded = false;
                    inspectorScrollPosition = GUI.BeginScrollView(inspectorScrollWindow, inspectorScrollPosition, inspectorWindow);
                    DrawEditor(box, true);
                    GUI.EndScrollView();
                    break;
                case State.Doc:
                    // Create a new register
                    selected = 2;
                    dataRestoreLoaded = false;
                    inspectorScrollPosition = GUI.BeginScrollView(inspectorScrollWindow, inspectorScrollPosition, inspectorWindow);
                    inspectorHeight = DrawHelp(box);
                    GUI.EndScrollView();
                    break;
                case State.Restore:
                    // Create a new register
                    selected = 3;
                    dataLoaded = false;
                    LoadRestore();
                    inspectorScrollPosition = GUI.BeginScrollView(inspectorScrollWindow, inspectorScrollPosition, inspectorWindow);
                    DrawRestore(box);
                    GUI.EndScrollView();
                    break;
            }

        }

        // Draw the Instance list
        public virtual bool DrawWaitingLoading(Rect box)
        {
            // Setup the layout
            Rect pos = box;
            pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;

            // Draw the content database info
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, functionTitle);

            // Load Instances Button
            pos.y += 2 * ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, loadButtonLabel))
                return true;

            // Show current instances
            pos.y += 2 * ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, notLoadedText);

            return false;
        }

        public virtual void NewResult(string resultMessage)
        {
            result = resultMessage;
            resultTimeout = Time.realtimeSinceStartup + resultDuration;
        }

        public int GetOptionPosition(int id, int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                if (ids[i] == id)
                    return i;
            }
            return 0;
        }
        public int GetOptionPosition(string id, string[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                if (ids[i] == id)
                    return i;
            }
            return 0;
        }


        public virtual void Load()
        {

        }
        public virtual void LoadRestore()
        {

        }

        public virtual void DrawLoaded(Rect box)
        {

        }
        public virtual void DrawRestore(Rect box)
        {

        }

        public virtual void DrawEditor(Rect box, bool nItem)
        {

        }

        public virtual void CreateNewData()
        {
        }

    }
}