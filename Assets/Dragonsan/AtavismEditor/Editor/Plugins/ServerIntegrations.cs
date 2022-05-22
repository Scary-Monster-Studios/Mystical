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
    // Server connection and setup function
    public class ServerIntegrations : AtavismFunction
    {

       // private string databaseHost;            // Database Host Address
       // private string databasePort;            // Database Host port
       // private string databaseUser;            // Database User Name
      //  private string databasePassword;        // Database Password
     //  private string databaseName;            // Database Name

        //private string testType = "none";		// Connection test type (Content or Admin)

        // Tab selection
        public int selected = 1;

        public Vector2 inspectorScrollPosition = Vector2.zero;
        public float inspectorHeight = 0;
        bool i2loc = false;
        bool masterAudio = false;
        bool streamer = false;
        bool streamer2 = false;
        bool pps2 = false;
        string i2loc_Integration = "AT_I2LOC_PRESET";
        string masterAudio_Integration = "AT_MASTERAUDIO_PRESET";
        string pps2_Integration = "AT_PPS2_PRESET";
        string streamer_Integration = "AT_STREAMER";
        string streamer2_Integration = "AT_STREAMER2";

        // Use this for initialization
        public ServerIntegrations()
        {

        }

        void Awake()
        {
            functionName = "Integrations";      

            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (symbols.Contains(i2loc_Integration))
                i2loc = true;
            else
                i2loc = false;
            if (symbols.Contains(masterAudio_Integration))
                masterAudio = true;
            else
                masterAudio = false;
            if (symbols.Contains(pps2_Integration))
                pps2 = true;
            else
                pps2 = false;

            if (symbols.Contains(streamer2_Integration))
                streamer2 = true;
            else
            {
                streamer2 = false;
                if (symbols.Contains(streamer_Integration))
                    streamer = true;
                else
                    streamer = false;
            }

        }

        // Update is called once per frame
        void Update()
        {
        }

        private int SelectTab(Rect pos, int sel)
        {
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (symbols.Contains(i2loc_Integration))
                i2loc = true;
            else
                i2loc = false;
            if (symbols.Contains(masterAudio_Integration))
                masterAudio = true;
            else
                masterAudio = false;

            if (symbols.Contains(streamer2_Integration))
                streamer2 = true;
            else
            {
                streamer2 = false;
                if (symbols.Contains(streamer_Integration))
                    streamer = true;
                else
                    streamer = false;
            }

            pos.y += ImagePack.tabTop;
            pos.x += pos.width - ImagePack.tabMargin * 3;

            bool edit = false;
            bool doc = false;

            switch (sel)
            {
                case 1:
                    edit = true;
                    break;
                case 2:
                    doc = true;
                    break;
            }

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

            return sel;
        }

        /// <summary>
        /// Enables the scroll bar and sets total window height
        /// </summary>
        /// <param name="windowHeight">Window height.</param>
        public void EnableScrollBar(float windowHeight)
        {
            inspectorHeight = windowHeight;
        }
        // Draw the function inspector
        // box: Rect representing the inspector area
        public override void Draw(Rect box)
        {

            // Draw the Control Tabs
            selected = SelectTab(box, selected);
            Rect inspectorScrollWindow = box;
            Rect inspectorWindow = box;
            inspectorWindow.width -= 2;
            inspectorScrollWindow.width += 14;
            inspectorWindow.height = Mathf.Max(box.height, inspectorHeight); Rect pos = box;
            inspectorScrollPosition = GUI.BeginScrollView(inspectorScrollWindow, inspectorScrollPosition, inspectorWindow);
            if (selected == 1)
            {
                // Set the drawing layout
                
                pos.x += ImagePack.innerMargin;
                pos.y += ImagePack.innerMargin;
                pos.width -= ImagePack.innerMargin;
                pos.height = ImagePack.fieldHeight;
                // Draw the content database info
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Content Integrations"));
                pos.y += ImagePack.fieldHeight * 2f;
                pos.width /= 2;
                i2loc = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("I2 Localization"), i2loc);
                pos.y += ImagePack.fieldHeight;
                masterAudio = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Master Audio"), masterAudio);
                pos.y += ImagePack.fieldHeight;
                streamer = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("World Streamer"), streamer);
                pos.y += ImagePack.fieldHeight;
                streamer2 = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("World Streamer 2"), streamer2);
                pos.y += ImagePack.fieldHeight;
                pps2 = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Post-process Stack 2"), pps2);
                pos.width *= 2;
                string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                if (!symbols.Contains(i2loc_Integration) && i2loc)
                {
                    symbols += ";" + i2loc_Integration;
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
                }

                if (!symbols.Contains(masterAudio_Integration) && masterAudio)
                {
                    symbols += ";" + masterAudio_Integration;
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
                }
                if (!symbols.Contains(pps2_Integration) && pps2)
                {
                    symbols += ";" + pps2_Integration;
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
                }
                if (!symbols.Contains(streamer_Integration) && streamer)
                {
                    symbols += ";" + streamer_Integration;
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
                }
                if (!symbols.Contains(streamer2_Integration) && streamer2)
                {
                    symbols += ";" + streamer2_Integration;
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
                }

                if (symbols.Contains(i2loc_Integration) && !i2loc)
                {
                    if (symbols.IndexOf(i2loc_Integration) > 0)
                        symbols = symbols.Remove(symbols.IndexOf(i2loc_Integration) - 1, i2loc_Integration.Length + 1);
                    if (symbols.IndexOf(i2loc_Integration) == 0)
                        symbols = symbols.Remove(0, i2loc_Integration.Length);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
                }

                if (symbols.Contains(masterAudio_Integration) && !masterAudio)
                {
                    if (symbols.IndexOf(masterAudio_Integration) > 0)
                        symbols = symbols.Remove(symbols.IndexOf(masterAudio_Integration) - 1, masterAudio_Integration.Length + 1);
                    if (symbols.IndexOf(masterAudio_Integration) == 0)
                        symbols = symbols.Remove(0, masterAudio_Integration.Length);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
                }
                if (symbols.Contains(pps2_Integration) && !pps2)
                {
                    if (symbols.IndexOf(pps2_Integration) > 0)
                        symbols = symbols.Remove(symbols.IndexOf(pps2_Integration) - 1, pps2_Integration.Length + 1);
                    if (symbols.IndexOf(pps2_Integration) == 0)
                        symbols = symbols.Remove(0, pps2_Integration.Length);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
                }
                if (symbols.Contains(streamer_Integration) && !streamer )
                {
                    
                    if (symbols.IndexOf(streamer_Integration) > 0 && symbols.IndexOf(streamer_Integration) != symbols.IndexOf(streamer2_Integration))
                        symbols = symbols.Remove(symbols.IndexOf(streamer_Integration) - 1, streamer_Integration.Length + 1);
                    if (symbols.IndexOf(streamer_Integration) == 0 && symbols.IndexOf(streamer_Integration) != symbols.IndexOf(streamer2_Integration))
                        symbols = symbols.Remove(0, streamer_Integration.Length);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
                }
                if (symbols.Contains(streamer2_Integration) && !streamer2)
                {
                    if (symbols.IndexOf(streamer2_Integration) > 0)
                        symbols = symbols.Remove(symbols.IndexOf(streamer2_Integration) - 1, streamer2_Integration.Length + 1);
                    if (symbols.IndexOf(streamer2_Integration) == 0)
                        symbols = symbols.Remove(0, streamer2_Integration.Length);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
                }

            }
            else if (selected == 2)
            {
                DrawHelp(box);
            }

            GUI.EndScrollView();
            EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight);
        }

    }

}