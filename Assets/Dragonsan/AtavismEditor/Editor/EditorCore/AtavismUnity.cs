using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Threading;

namespace Atavism
{
    enum AtavismEditorState
    {
        Intro,
        Login,
        UnityQuestion,
        RegisterUnity,
        WaitingResponse,
        RegisterComplete,
        LoadGUI,
        Home
    }

    // Main Atavism Unity Editor Window
    public class AtavismUnity : EditorWindow
    {
        static AtavismUnity instance;
        readonly string version1 = "X";
        readonly string version = ".6.0";

        // Store plugin list	
        private List<string> pluginList = new List<string>();
        // Store plugins
        public List<AtavismPlugin> plugins = new List<AtavismPlugin>();

        // Stores the selected plugin
        private AtavismPlugin currentPlugin = null;
        // Stores the selected plugin function
        private AtavismFunction currentFunction;

        public Vector2 categoryScrollPosition = Vector2.zero;
        public Vector2 functionScrollPosition = Vector2.zero;

        public float categoryHeight = 0;
        public float functionHeight = 0;
    
        // Editor State
        private AtavismEditorState editorState;
      
        private string currentFilter = "all";

        private bool showDatabaseSettings = false;

        private string searchString = "";
       private bool searchOption = false;

        int counting = 0;
        bool restatring = false;
        bool stoping = false;

        // Create the main window at the Unity Window Menu
        [MenuItem("Window/Atavism/Atavism Editor (Atavism Online)", priority = -1000)]
        public static void ShowWindow()
        {
            CultureInfo ci = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            // Basic window parameters		
            int width = ImagePack.layoutWidth+80;
            int height = ImagePack.layoutHeight;
             // Window Title
            string title = "Atavism Editor";
            // Create the main window
            AtavismUnity atavism = (AtavismUnity)EditorWindow.GetWindow(typeof(AtavismUnity), false, title);
            instance = atavism;
            atavism.minSize = new Vector2(width, height);
             // We need the mouse movement events
            atavism.wantsMouseMove = true;
             atavism.editorState = AtavismEditorState.LoadGUI;
         
        }
        private void OnFocus()
        {
#if UNITY_EDITOR_WIN
            if (instance == null)
                ShowWindow();
          /*  if (editorState == AtavismEditorState.Intro)
            {
                ShowWindow();
            }
            */
                // Load Atavism Plugins
            LoadPlugins();
            // Run plugins initialization
            Installplugins();
#endif
        }
      
        // Main GUI code
        void OnGUI()
        {
           if (editorState == AtavismEditorState.LoadGUI)
            {
               
                // Load Atavism Plugins
                LoadPlugins();
                // Run plugins initialization
                Installplugins();
                // Starts the GUI
                editorState = AtavismEditorState.Home;

                if (!DatabasePack.TestConnection(""))
                {
                    showDatabaseSettings = true;
                    UnityEngine.Debug.Log("Connection details not correct, showing Database plugin");
                }
            }

            if (editorState == AtavismEditorState.Home)
            {
                float x = position.x;
                float y = position.y;
                GUI.skin = ImagePack.atavismSkin;
                // Initial position to draw		
                float pos = ImagePack.layoutTopMargin;
                // Draw background
                GUI.DrawTexture(new Rect(0, 0, position.width, position.height), ImagePack.mainWindow);
                GUI.DrawTexture(new Rect(0, 0, position.width, ImagePack.mainWindow_topbar.height), ImagePack.mainWindow_topbar);
                GUI.DrawTexture(new Rect(18, 3, ImagePack.mainWindow_logo.width, ImagePack.mainWindow_logo.height), ImagePack.mainWindow_logo);
                GUI.DrawTexture(new Rect(3, 53, ImagePack.mainWindow_category_top.width, ImagePack.mainWindow_category_top.height), ImagePack.mainWindow_category_top);
                GUI.DrawTexture(new Rect(3, 68, ImagePack.mainWindow_category_mid.width, position.height - 16), ImagePack.mainWindow_category_mid);
                GUI.DrawTexture(new Rect(3, position.height - 16, ImagePack.mainWindow_category_bottom.width, ImagePack.mainWindow_category_bottom.height), ImagePack.mainWindow_category_bottom);
                GUI.DrawTexture(new Rect(140, 125, ImagePack.mainWindow_plugin_top.width, ImagePack.mainWindow_plugin_top.height), ImagePack.mainWindow_plugin_top);
                GUI.DrawTexture(new Rect(140, 140, ImagePack.mainWindow_plugin_mid.width, position.height - 21), ImagePack.mainWindow_plugin_mid);
                GUI.DrawTexture(new Rect(140, position.height - 21, ImagePack.mainWindow_plugin_bottom.width, ImagePack.mainWindow_plugin_bottom.height), ImagePack.mainWindow_plugin_bottom);

                GUI.DrawTexture(new Rect(ImagePack.layoutInspectorMargin - 20, 53, ImagePack.mainWindow_inspec_top_l.width, ImagePack.mainWindow_inspec_top_l.height), ImagePack.mainWindow_inspec_top_l);
                GUI.DrawTexture(new Rect(ImagePack.layoutInspectorMargin - 20 + ImagePack.mainWindow_inspec_top_l.width, 53, position.width - ImagePack.mainWindow_inspec_top_r.width, ImagePack.mainWindow_inspec_top.height), ImagePack.mainWindow_inspec_top);
                GUI.DrawTexture(new Rect(position.width - ImagePack.mainWindow_inspec_top_r.width, 53, ImagePack.mainWindow_inspec_top_r.width, ImagePack.mainWindow_inspec_top_r.height), ImagePack.mainWindow_inspec_top_r);

                GUI.DrawTexture(new Rect(ImagePack.layoutInspectorMargin - 20, 53 + ImagePack.mainWindow_inspec_top_l.height, ImagePack.mainWindow_inspec_mid_l.width, position.height - 53 - ImagePack.mainWindow_inspec_top.height - ImagePack.mainWindow_inspec_bottom_l.height - 30), ImagePack.mainWindow_inspec_mid_l);
                GUI.DrawTexture(new Rect(ImagePack.layoutInspectorMargin - 20 + ImagePack.mainWindow_inspec_mid_l.width, 53 + ImagePack.mainWindow_inspec_top.height, position.width - ImagePack.mainWindow_inspec_mid_r.width, position.height - 53 - ImagePack.mainWindow_inspec_top.height - ImagePack.mainWindow_inspec_bottom.height - 30), ImagePack.mainWindow_inspec_mid);
                GUI.DrawTexture(new Rect(position.width - ImagePack.mainWindow_inspec_mid_r.width, 53 + ImagePack.mainWindow_inspec_top_r.height, ImagePack.mainWindow_inspec_mid_r.width, position.height - 53 - ImagePack.mainWindow_inspec_top.height - ImagePack.mainWindow_inspec_bottom_r.height - 30), ImagePack.mainWindow_inspec_mid_r);

                GUI.DrawTexture(new Rect(ImagePack.layoutInspectorMargin - 20, position.height - ImagePack.mainWindow_inspec_bottom_l.height - 30, ImagePack.mainWindow_inspec_bottom_l.width, ImagePack.mainWindow_inspec_bottom_l.height), ImagePack.mainWindow_inspec_bottom_l);
                GUI.DrawTexture(new Rect(ImagePack.layoutInspectorMargin - 20 + ImagePack.mainWindow_inspec_bottom_l.width, position.height - ImagePack.mainWindow_inspec_bottom.height - 30, position.width - ImagePack.mainWindow_inspec_bottom_r.width, ImagePack.mainWindow_inspec_bottom.height), ImagePack.mainWindow_inspec_bottom);
                GUI.DrawTexture(new Rect(position.width - ImagePack.mainWindow_inspec_bottom_r.width, position.height - ImagePack.mainWindow_inspec_bottom_r.height - 30, ImagePack.mainWindow_inspec_bottom_r.width, ImagePack.mainWindow_inspec_bottom_r.height), ImagePack.mainWindow_inspec_bottom_r);

                ImagePack.DrawText(new Rect(176, 7, 50, 20), version1);
                int fs = ImagePack.textFontSize;
                ImagePack.textFontSize = (int) (ImagePack.textFontSize * 0.8f);
                ImagePack.DrawText(new Rect(188, 8, 50, 20), version);
                ImagePack.textFontSize = fs;

                Color c = ImagePack.fieldFontColor;
                ImagePack.fieldFontColor = Color.white;
                Lang.Language = ImagePack.DrawSelector(new Rect(270, 11, 200, 30), Lang.GetTranslate("Language") + ": ", Lang.Language, Lang.LangOptions);
                ImagePack.fieldFontColor = c;
                if (ImagePack.DrawButton(position.width - 310, 5, Lang.GetTranslate("Manage Account"), 30))
                {
                    Application.OpenURL("http://apanel.atavismonline.com");
                }
                if (ImagePack.DrawButton(position.width - 170, 5, Lang.GetTranslate("AO Log File"), 30))
                {
                    if (File.Exists("AtavismEditor.log"))
                        System.Diagnostics.Process.Start("AtavismEditor.log");
                }
                if (ImagePack.DrawButton(position.width - 310, 25, Lang.GetTranslate("Atavism Online Store"), 30))
                {
                    Application.OpenURL("http://www.atavismonline.com");
                }
                if (ImagePack.DrawButton(position.width - 170, 25, Lang.GetTranslate("Documentation"), 30))
                {
                    Application.OpenURL("http://unity.wiki.atavismonline.com/");
                }
                Color color = Color.white;
                Color color2 = Color.white;
                if (ImagePack.DrawButton(new Rect(position.width - 430, 5, 80, 20), Lang.GetTranslate("Reload config"), color, Lang.GetTranslate("Reload config from Atavism Editor Standalone")))
                {
                    DatabasePack.ReadConfig();
                }
             /*   if (restatring)
                {
                    if ((counting > 100 && counting < 200) || (counting > 300 && counting < 400) || (counting > 500 && counting < 600) || (counting > 700 && counting < 800) || (counting > 900 && counting < 1000) ||
                        (counting > 1100 && counting < 1200) || (counting > 1300 && counting < 1400) || (counting > 1500 && counting < 1600) || (counting > 1700 && counting < 1800) || (counting > 1900 && counting < 2000))
                        color = Color.green;
                }
                if (stoping)
                {
                    if ((counting > 100 && counting < 200) || (counting > 300 && counting < 400) || (counting > 500 && counting < 600) || (counting > 700 && counting < 800) || (counting > 900 && counting < 1000) ||
                        (counting > 1100 && counting < 1200) || (counting > 1300 && counting < 1400) || (counting > 1500 && counting < 1600) || (counting > 1700 && counting < 1800) || (counting > 1900 && counting < 2000))
                        color2 = Color.green;
                }
                ImagePack.DrawText(new Rect(position.width - 520, 2, 50, 20), Lang.GetTranslate("Server Status"), Lang.GetTranslate("Only for Atavism Virtual Machine"));

                if (ImagePack.DrawButton(new Rect(position.width - 430, 5, 80, 20), Lang.GetTranslate("Start/Restart"), color, Lang.GetTranslate("Only for Atavism Virtual Machine")))
                {
                    if (DatabasePack.TestConnection(DatabasePack.adminDatabasePrefix, true))
                        RestartServer();
                    else
                        UnityEngine.Debug.LogError("No connection");
                }
                if (ImagePack.DrawButton(new Rect(position.width - 430, 25, 80, 20), Lang.GetTranslate("Stop"), color2, Lang.GetTranslate("Only for Atavism Virtual Machine")))
                {
                    if (DatabasePack.TestConnection(DatabasePack.adminDatabasePrefix, true))
                        StopServer();
                    else
                        UnityEngine.Debug.LogError("No connection");
                }
                ImagePack.DrawText(new Rect(position.width - 510, 25, 10, 20), "A:", Lang.GetTranslate("Only for Atavism Virtual Machine"));
                ImagePack.DrawText(new Rect(position.width - 468, 25, 10, 20), "W:", Lang.GetTranslate("Only for Atavism Virtual Machine"));
                if (counting > 2000)
                {
                    checkServer();
                    counting = 0;
                }
                else
                {
                    counting++;
                }

                if (authServer == 0)
                {
                    GUI.DrawTexture(new Rect(position.width - 494, 28, ImagePack.status.width, ImagePack.status.height), ImagePack.status);
                }
                else
                {
                    if (authServer == 2)
                        GUI.DrawTexture(new Rect(position.width - 494, 28, ImagePack.status.width, ImagePack.status.height), ImagePack.statusBlue);
                    else
                        GUI.DrawTexture(new Rect(position.width - 494, 28, ImagePack.status.width, ImagePack.status.height), ImagePack.statusRed);
                }

                if (worldServer == 0)
                {
                    GUI.DrawTexture(new Rect(position.width - 450, 28, ImagePack.status.width, ImagePack.status.height), ImagePack.status);
                }
                else
                {
                    if (worldServer == 2)
                        GUI.DrawTexture(new Rect(position.width - 450, 28, ImagePack.status.width, ImagePack.status.height), ImagePack.statusBlue);
                    else
                        GUI.DrawTexture(new Rect(position.width - 450, 28, ImagePack.status.width, ImagePack.status.height), ImagePack.statusRed);
                }*/
                // Draw all button
                Rect all = new Rect(ImagePack.layoutLeftMargin, pos, ImagePack.allButton.width, ImagePack.allButton.height);
                bool allSelected = false;
                if (currentFilter == "all")
                    allSelected = true;
                float initialPos = ImagePack.layoutTop;
                Rect categoryScrollWindow = new Rect(ImagePack.layoutLeftMargin, ImagePack.layoutTop, ImagePack.layoutCategoryWidth, ImagePack.layoutCategoryHeight);
                Rect categoryWindow = new Rect(ImagePack.layoutLeftMargin, ImagePack.layoutTop, ImagePack.layoutCategoryWidth, categoryHeight);

                categoryScrollPosition = GUI.BeginScrollView(categoryScrollWindow, categoryScrollPosition, categoryWindow);

                if (GUI.Button(all, "", ImagePack.AllButtonStyle(allSelected)))
                {
                    currentFilter = "all";
                    currentPlugin = null;
                }

                pos += ImagePack.allButton.height + ImagePack.layoutSpace;

                // Draw the Categories Installed
                foreach (AtavismPlugin plugin in plugins)
                {
                    // For each category get the plugin information
                    Rect position = new Rect(ImagePack.layoutLeftMargin, pos, plugin.icon.width, plugin.icon.height);
                    // Check if has an catagory selected
                    Texture pluginIcon = plugin.icon; 
                    if ((currentPlugin != null) && (currentPlugin.pluginName == plugin.pluginName))
                        pluginIcon = plugin.iconSelected;

                    if (GUI.Button(position, "", ImagePack.CategoryButtonStyle(pluginIcon, plugin.iconOver)))
                    {
                        // If clicked at Category, load the plugin
                        currentPlugin = plugin;
                        currentFilter = plugin.pluginName;
                        allSelected = false;
                        currentFunction = null;
                        //   mouseClicked = false;
                        return;
                    }
                    int s = ImagePack.textFontSize;
                    ImagePack.textFontSize = 9;
                    GUIStyle style = new GUIStyle();
                    style.fontSize = ImagePack.textFontSize;
                    Vector2 size = style.CalcSize(new GUIContent(Lang.GetTranslate(plugin.pluginName)));
                    ImagePack.DrawText(new Rect(ImagePack.layoutLeftMargin+ plugin.icon.width/2- size.x/2, pos + plugin.icon.height - size.y - 6, size.x, size.y), Lang.GetTranslate(plugin.pluginName));
                    ImagePack.textFontSize = s;

                    // Get the next position to draw category icon
                    pos += plugin.icon.height + ImagePack.layoutSpace;
                }

                GUI.EndScrollView();
                categoryHeight = pos - initialPos;
                // Draw search box
                SearchBox();

                // Setup the function initial draw position
                pos = ImagePack.layoutFunctionTopMargin;
                int functionCount = 0;
                Rect functionScrollWindow = new Rect(ImagePack.layoutFunctionMargin, ImagePack.layoutFunctionTop, ImagePack.layoutFunctionWidth + 16, position.height - ImagePack.layoutFunctionTopMargin - 12);
                Rect functionWindow = new Rect(ImagePack.layoutFunctionMargin, pos, ImagePack.layoutFunctionWidth, functionHeight);
                functionScrollPosition = GUI.BeginScrollView(functionScrollWindow, functionScrollPosition, functionWindow);
                pos += ImagePack.layoutSpace;
                // For each function in the selected plugin
                foreach (AtavismFunction function in plugins[0].ListFunctions())
                {
                    // Check if the Pluign belongs to this category, or if show all is selected
                    if ((function.functionCategory == currentFilter) || allSelected)
                    {
                        // Perform search operation before display plugin
                        // Test if plugin name match search criteria 
                        if (CheckSearch(function.functionName))
                        {
                            functionCount++;
                            // Get the initial position to draw the button
                            Rect position = new Rect(ImagePack.layoutFunctionMargin, pos, ImagePack.button.width, ImagePack.button.height);

                            if (showDatabaseSettings)
                            {
                                if (function.functionName == "Data Base")
                                {
                                    currentFunction = function;
                                    currentFunction.Activate();
                                    showDatabaseSettings = false;
                                    break;
                                }
                            }
                            // Is there a function selected?
                            bool thisFunctionSelected = false;
                            if ((currentFunction != null) && (currentFunction.functionName == function.functionName))
                                thisFunctionSelected = true;
                            if (GUI.Button(position, "", ImagePack.PluginButtonStyle(thisFunctionSelected)))
                            {
                                currentFunction = function;
                                currentFunction.Activate();
                                return;
                            }
                            // After draw the button, draw the info in front of the button
                            // Set the initial Atavism logo position
                            int iconMargin = (ImagePack.button.height - ImagePack.atLogo.height) / 2;
                            Rect iconLeft = new Rect(ImagePack.layoutFunctionMargin + iconMargin, pos + iconMargin, ImagePack.atLogo.width, ImagePack.atLogo.height);
                            GUI.DrawTexture(iconLeft, ImagePack.atLogo);
                            // Set the initial Lock position
                            Rect iconRight = new Rect(ImagePack.layoutFunctionMargin + ImagePack.button.width - ImagePack.lockGreen.width - iconMargin - 3, pos + iconMargin, ImagePack.lockGreen.width, ImagePack.lockGreen.height);
                            GUI.DrawTexture(iconRight, ImagePack.lockGreen);
                            // Set the function name position
                            Rect textPosition = new Rect(ImagePack.layoutFunctionMargin + ImagePack.button.width / 5, pos + ImagePack.button.height / 4, ImagePack.button.width, ImagePack.button.height);
                            GUI.Label(textPosition, Lang.GetTranslate(function.functionName));
                            // Go to the next button drtaw position
                            pos += ImagePack.button.height + ImagePack.layoutSpace;
                        }
                    }
                }

                GUI.EndScrollView();

                functionHeight = functionCount * (ImagePack.button.height + ImagePack.layoutSpace);

                // Setup the Inspector Display position		
                // If selected a function shows its inspector
                if (currentFunction != null)
                {
                    // get the function initial position
                    Rect inspectorTitle = new Rect(ImagePack.layoutInspectorMargin, ImagePack.layoutTopMargin, ImagePack.layoutInspectorBoxWidth, 30);
                    GUI.Label(inspectorTitle, Lang.GetTranslate(currentFunction.functionName) + Lang.GetTranslate(currentFunction.breadCrumb), ImagePack.HeaderStyle());
                    // Get the inspector box area
                    pos += ImagePack.button.height + ImagePack.layoutSpace;
                    Rect inspectorBox = new Rect(ImagePack.layoutInspectorMargin, ImagePack.layoutInspectorTopMargin, position.width - ImagePack.layoutInspectorMargin - 26, position.height - ImagePack.layoutInspectorTopMargin - 80);
                    // Ask the Function to draw its own inspector
                     currentFunction.Draw(inspectorBox);
                    if(currentFunction.showSave|| currentFunction.showDelete|| currentFunction.showCancel)
                        GUI.DrawTexture(new Rect(ImagePack.layoutInspectorMargin - 20 +7, position.height - ImagePack.mainWindow_inspec_bottom_li.height - 73, position.width - ImagePack.layoutInspectorMargin+4, ImagePack.mainWindow_inspec_bottom_li.height), ImagePack.mainWindow_inspec_bottom_li);

                    if (currentFunction.showSave)
                        if (ImagePack.DrawButton(400, position.height - 65, Lang.GetTranslate("Save Data")))
                        {
                            currentFunction.save();
                        }
                    if (currentFunction.showDelete)
                        if (ImagePack.DrawButton(550, position.height - 65, Lang.GetTranslate("Delete Data")))
                        {
                            currentFunction.delete();
                        }
                    if (currentFunction.showCancel)
                        if (ImagePack.DrawButton(700, position.height - 65, Lang.GetTranslate("Cancel")))
                        {
                            currentFunction.cancel();
                        }


                    Rect inspectorInfo = new Rect(ImagePack.layoutInspectorMargin - 10, position.height - 33, position.width - ImagePack.layoutInspectorMargin - 26, 30);

                    string message = currentFunction.result;
                    if (message.StartsWith("Error:"))
                    {
                        GUI.color = Color.red;
                        message = message.Remove(0, 6);
                    }
                    ImagePack.DrawText(inspectorInfo, message);
                    GUI.color = Color.white;

                }
                //}
                Repaint();
            }
        
        }

        void RestartServer()
        {
            string query = "INSERT INTO server";
            query += " (action) ";
            query += "VALUES ";
            query += " (?action) ";


            // Setup the register data		
            List<Register> update = new List<Register>();
            update.Add(new Register("action", "?action", MySqlDbType.VarChar, "restart", Register.TypesOfField.String));

            DatabasePack.Insert(DatabasePack.adminDatabasePrefix, query, update);
            restatring = true;
            worldServer = 0;
            authServer = 0;
            //  loginMessage = "Send Restart Command";
        }


        bool checkStatus(string server)
        {
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            string query = "SELECT server,status FROM server_status WHERE server like '" + server + "'";
            string query2 = "SELECT action FROM server";

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();
            rows = DatabasePack.LoadData(DatabasePack.adminDatabasePrefix, query2);
            if (rows.Count == 0)
            {
                restatring = false;
                stoping = false;
            }
            if (rows != null)
                rows.Clear();
            // Load data
            rows = DatabasePack.LoadData(DatabasePack.adminDatabasePrefix, query);
            // Read all the data
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    if (bool.Parse(data["status"]))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return false;
        }

        void StopServer()
        {
            string query = "INSERT INTO server ";
            query += " (action) ";
            query += "VALUES ";
            query += " (?action) ";
            // Setup the register data		
            List<Register> update = new List<Register>();
            update.Add(new Register("action", "?action", MySqlDbType.VarChar, "stop", Register.TypesOfField.String));

            DatabasePack.Insert(DatabasePack.adminDatabasePrefix, query, update);
            stoping = true;
        }

        void OnDestroy()
        {

        }

        // Check if name match search criteria
        bool CheckSearch(string name)
        {
            // Search is NOT case sensitive
            string lowCaseName = name.ToLower();
            // Check if searching terms
            if (searchOption)
            {
                // Match full initial word
                return lowCaseName.StartsWith(searchString.ToLower());
            }
            else
            {
                // Match letter sequence
                return lowCaseName.Contains(searchString.ToLower());
            }
        }

        // Draw search box
        void SearchBox()
        {
            Rect searchPos = new Rect(ImagePack.layoutFunctionMargin, ImagePack.layoutTopMargin, 200, ImagePack.fieldHeight - ImagePack.lineSpace);
            // Save search string to test if something was type
            string previousSearchString = searchString;
            searchString = GUI.TextField(searchPos, searchString, ImagePack.TextFieldStyle());
            if (previousSearchString != searchString)
            {
                searchOption = false;
            }
            if (GUI.Button(new Rect(ImagePack.layoutFunctionRightMargin - 10, searchPos.y, ImagePack.buttonSearch.width, ImagePack.buttonSearch.height), "", ImagePack.ButtonSearch()))
            {
                searchOption = true;
            }
            searchPos.y += ImagePack.fieldHeight;
            searchPos.width /= 3;
            searchPos.x += ImagePack.button.width / 3;
            if (GUI.Button(searchPos, Lang.GetTranslate("Reset"), ImagePack.ButtonStyle()))
            {
                searchOption = false;
                searchString = "";
            }
        }

        // Load the plugins/categories
        void LoadPlugins()
        {
            pluginList.Clear();
            pluginList.Add("Server");
          //  pluginList.Add("Mobs");
          //  pluginList.Add("Items");
          //  pluginList.Add("Combat");
         //   pluginList.Add("Character");
            pluginList.Add("Integration");
        }

        // Install/Load a plugin
        void Installplugins()
        {
            plugins.Clear();
            // Create the plugin Instances
            for (int i = 0; i < pluginList.Count; i++)
            {
                // The class name is the plugin name plus Plugin prefix
                string className = pluginList[i] + "Plugin";
                plugins.Add((AtavismPlugin)AtavismPlugin.CreateInstance(className));
            }
        }
        int authServer = 0;
        int worldServer = 0;

        private void checkServer()
        {
            if (!DatabasePack.TestConnection(DatabasePack.adminDatabasePrefix))
            {
                authServer = 0;
                worldServer = 0;
            }
            else
            {
                if (checkStatus("auth"))
                    authServer = 2;
                else
                    authServer = 1;
                if (checkStatus("world"))
                    worldServer = 2;
                else
                    worldServer = 1;
            }
        }
        public static AtavismUnity Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
