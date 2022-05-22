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
    public class ServerDataBase : AtavismFunction
    {

        private string databaseHost;            // Database Host Address
     //   private string databasePort;            // Database Host port
        private string databaseUser;            // Database User Name
       // private string databasePassword;        // Database Password
        private string databaseName;            // Database Name

        private string testType = "none";       // Connection test type (Content or Admin)

        // Tab selection
        public int selected = 1;

        public Vector2 inspectorScrollPosition = Vector2.zero;
        public float inspectorHeight = 0;
        private bool sameCredential = false;
        // Use this for initialization
        public ServerDataBase()
        {

        }

        void Awake()
        {
            functionName = "Data Base";     // Set the function name
            string prefix = DatabasePack.contentDatabasePrefix;
            databaseHost = EditorPrefs.GetString("databaseHost" + prefix + DatabasePack.GetProjectName());
            if (databaseHost == null || databaseHost == "")
            {
                EditorPrefs.SetString("databaseHost" + prefix + DatabasePack.GetProjectName(), "localhost");
            }
            databaseName = EditorPrefs.GetString("databaseName" + prefix + DatabasePack.GetProjectName());
            if (databaseName == null || databaseName == "")
            {
                EditorPrefs.SetString("databaseName" + prefix + DatabasePack.GetProjectName(), "world_content");
            }
            databaseUser = EditorPrefs.GetString("databaseUser" + prefix + DatabasePack.GetProjectName());
            if (databaseUser == null || databaseUser == "")
            {
                EditorPrefs.SetString("databaseUser" + prefix + DatabasePack.GetProjectName(), "root");
            }

            prefix = DatabasePack.adminDatabasePrefix;
            databaseHost = EditorPrefs.GetString("databaseHost" + prefix + DatabasePack.GetProjectName());
            if (databaseHost == null || databaseHost == "")
            {
                EditorPrefs.SetString("databaseHost" + prefix + DatabasePack.GetProjectName(), "localhost");
            }
            databaseName = EditorPrefs.GetString("databaseName" + prefix + DatabasePack.GetProjectName());
            if (databaseName == null || databaseName == "")
            {
                EditorPrefs.SetString("databaseName" + prefix + DatabasePack.GetProjectName(), "admin");
            }
            databaseUser = EditorPrefs.GetString("databaseUser" + prefix + DatabasePack.GetProjectName());
            if (databaseUser == null || databaseUser == "")
            {
                EditorPrefs.SetString("databaseUser" + prefix + DatabasePack.GetProjectName(), "root");
            }

            prefix = DatabasePack.masterDatabasePrefix;
            databaseHost = EditorPrefs.GetString("databaseHost" + prefix + DatabasePack.GetProjectName());
            if (databaseHost == null || databaseHost == "")
            {
                EditorPrefs.SetString("databaseHost" + prefix + DatabasePack.GetProjectName(), "localhost");
            }
            databaseName = EditorPrefs.GetString("databaseName" + prefix + DatabasePack.GetProjectName());
            if (databaseName == null || databaseName == "")
            {
                EditorPrefs.SetString("databaseName" + prefix + DatabasePack.GetProjectName(), "master");
            }
            databaseUser = EditorPrefs.GetString("databaseUser" + prefix + DatabasePack.GetProjectName());
            if (databaseUser == null || databaseUser == "")
            {
                EditorPrefs.SetString("databaseUser" + prefix + DatabasePack.GetProjectName(), "root");
            }
            prefix = DatabasePack.atavismDatabasePrefix;
            databaseHost = EditorPrefs.GetString("databaseHost" + prefix + DatabasePack.GetProjectName());
            if (databaseHost == null || databaseHost == "")
            {
                EditorPrefs.SetString("databaseHost" + prefix + DatabasePack.GetProjectName(), "localhost");
            }
            databaseName = EditorPrefs.GetString("databaseName" + prefix + DatabasePack.GetProjectName());
            if (databaseName == null || databaseName == "")
            {
                EditorPrefs.SetString("databaseName" + prefix + DatabasePack.GetProjectName(), "atavism");
            }
            databaseUser = EditorPrefs.GetString("databaseUser" + prefix + DatabasePack.GetProjectName());
            if (databaseUser == null || databaseUser == "")
            {
                EditorPrefs.SetString("databaseUser" + prefix + DatabasePack.GetProjectName(), "root");
            }


        }

        // Update is called once per frame
        void Update()
        {
        }

        private int SelectTab(Rect pos, int sel)
        {
            pos.y += ImagePack.tabTop;
            pos.x += pos.width - ImagePack.tabMargin*3;
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
        /*    pos.x += ImagePack.tabMargin;
            ImagePack.DrawTabDoc(pos, doc);
            pos.x += ImagePack.tabMargin;
            ImagePack.DrawTabDoc(pos, doc);
            */
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
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Content Database Configuration"));
                pos.y += ImagePack.fieldHeight;
                sameCredential = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Same Database credentials?"), sameCredential);
                DrawDataBase(pos, "content");
                if (sameCredential)
                    SetSameCredential();
                // Draw the admin database info
                pos.y += 230;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Administrator Database Configuration"));
                DrawDataBase(pos, "admin");
                pos.y += 230;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Master Database Configuration"));
                DrawDataBase(pos, "master");
                pos.y += 230;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Atavism Database Configuration"));
                DrawDataBase(pos, "atavism");
                pos.y += 210;
                ImagePack.DrawScrollBar(box.x + box.width, box.y, box.height - 14);
             

            }
            else if (selected == 2)
            {
                DrawHelp(box);
            }

            GUI.EndScrollView();
            EnableScrollBar(pos.y - box.y + ImagePack.fieldHeight);
        }

        private void SetSameCredential()
        {
            string prefixc = DatabasePack.contentDatabasePrefix;
            string prefix = DatabasePack.adminDatabasePrefix;
            EditorPrefs.SetString("databaseHost" + prefix + DatabasePack.GetProjectName(), EditorPrefs.GetString("databaseHost" + prefixc + DatabasePack.GetProjectName()));
            EditorPrefs.SetString("databasePort" + prefix + DatabasePack.GetProjectName(), EditorPrefs.GetString("databasePort" + prefixc + DatabasePack.GetProjectName()));
            EditorPrefs.SetString("databaseUser" + prefix + DatabasePack.GetProjectName(), EditorPrefs.GetString("databaseUser" + prefixc + DatabasePack.GetProjectName()));
            EditorPrefs.SetString("databasePassword" + prefix + DatabasePack.GetProjectName(), EditorPrefs.GetString("databasePassword" + prefixc + DatabasePack.GetProjectName()));
            prefix = DatabasePack.masterDatabasePrefix;
            EditorPrefs.SetString("databaseHost" + prefix + DatabasePack.GetProjectName(), EditorPrefs.GetString("databaseHost" + prefixc + DatabasePack.GetProjectName()));
            EditorPrefs.SetString("databasePort" + prefix + DatabasePack.GetProjectName(), EditorPrefs.GetString("databasePort" + prefixc + DatabasePack.GetProjectName()));
            EditorPrefs.SetString("databaseUser" + prefix + DatabasePack.GetProjectName(), EditorPrefs.GetString("databaseUser" + prefixc + DatabasePack.GetProjectName()));
            EditorPrefs.SetString("databasePassword" + prefix + DatabasePack.GetProjectName(), EditorPrefs.GetString("databasePassword" + prefixc + DatabasePack.GetProjectName()));
            prefix = DatabasePack.atavismDatabasePrefix;
            EditorPrefs.SetString("databaseHost" + prefix + DatabasePack.GetProjectName(), EditorPrefs.GetString("databaseHost" + prefixc + DatabasePack.GetProjectName()));
            EditorPrefs.SetString("databasePort" + prefix + DatabasePack.GetProjectName(), EditorPrefs.GetString("databasePort" + prefixc + DatabasePack.GetProjectName()));
            EditorPrefs.SetString("databaseUser" + prefix + DatabasePack.GetProjectName(), EditorPrefs.GetString("databaseUser" + prefixc + DatabasePack.GetProjectName()));
            EditorPrefs.SetString("databasePassword" + prefix + DatabasePack.GetProjectName(), EditorPrefs.GetString("databasePassword" + prefixc + DatabasePack.GetProjectName()));

        }

        // Basic Database Draw
        void DrawDataBase(Rect box, string type)
        {
            // Layout
            float posX = box.x + ImagePack.innerMargin;
            float posY = box.y;
            float width = box.width - 2 * ImagePack.innerMargin;
            float height = ImagePack.fieldHeight;

            // Fields prefix
            string prefix = "";
            if (type == "admin")
                prefix = DatabasePack.adminDatabasePrefix;
            if (type == "master")
                prefix = DatabasePack.masterDatabasePrefix;
            if (type == "atavism")
                prefix = DatabasePack.atavismDatabasePrefix;

            // Database Fields		
            posY += height;
             ImagePack.DrawSavedData(new Rect(posX, posY, width, height), Lang.GetTranslate("Database Host")+":", "databaseHost" + prefix + DatabasePack.GetProjectName(), false);
            posY += height;
             ImagePack.DrawSavedData(new Rect(posX, posY, width, height), Lang.GetTranslate("Database Port")+":", "databasePort" + prefix + DatabasePack.GetProjectName(), false);
            posY += height;
             ImagePack.DrawSavedData(new Rect(posX, posY, width, height), Lang.GetTranslate("Database Name")+":", "databaseName" + prefix + DatabasePack.GetProjectName(), false);
            posY += height;
             ImagePack.DrawSavedData(new Rect(posX, posY, width, height), Lang.GetTranslate("Database User")+":", "databaseUser" + prefix + DatabasePack.GetProjectName(), false);
            posY += height;
             ImagePack.DrawSavedData(new Rect(posX, posY, width, height), Lang.GetTranslate("Database Password")+":", "databasePassword" + prefix + DatabasePack.GetProjectName(), true);
            posY += 1.4f * height;

            // Test the connection
            if (ImagePack.DrawButton(posX, posY, Lang.GetTranslate("Test Connection")))
            {
                testType = type;
                DatabasePack.TestConnection(prefix, true);
            }
            posY += 0.8f * height;
            if (type == testType)
                GUI.Label(new Rect(posX, posY, width, height), DatabasePack.connectionResult, ImagePack.FieldStyle());
        }

    }

}