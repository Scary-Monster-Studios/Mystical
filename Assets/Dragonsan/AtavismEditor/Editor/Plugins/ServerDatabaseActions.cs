using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
namespace Atavism
{
     public partial class ServerDatabaseActions : AtavismFunction
    {
        bool showQueryForm = false;
        Vector2 descriptionScroll = new Vector2();
        string query = "";
        int databaseId = 0;
        string[] databaseOptions = new string[] { "Master", "Admin", "Atavism", "Content" };
        // Use this for initialization
        public ServerDatabaseActions()
        {
            functionName = "Database Actions";
        }

        public override void Activate()
        {
            query = "";
        }

        public void LoadSelectList()
        {
        }

        // Edit or Create
        public override void Draw(Rect box)
        {
            // Setup the layout
            Rect pos = box;
            pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;
            DrawHome(pos);
        }

        void DrawHome(Rect pos)
        {
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate(functionName));
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Wipe Characters")))
            {
                if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete Characters")  + " ?", Lang.GetTranslate("Are you sure you want to delete all Characters") + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
                {
                    WipeCharacter();
                }
            }
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Wipe Characters & Accounts")))
            {
                if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete Characters & Accounts") + " ?", Lang.GetTranslate("Are you sure you want to delete all Characters & Accounts") + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
                {
                    WipeAccounts();
                }
            }
            pos.y += ImagePack.fieldHeight*2f;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Disable All Spawned Mobs")))
            {
                if (EditorUtility.DisplayDialog(Lang.GetTranslate("Disable All Spawned Mobs") + " ?", Lang.GetTranslate("Are you sure you want to disable all Spawned Mobs") + " ?", Lang.GetTranslate("Disable"), Lang.GetTranslate("Do Not disable")))
                {
                    DisableSpawnedMobs();
                }
            }
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Enable All Spawned Mobs")))
            {
                if (EditorUtility.DisplayDialog(Lang.GetTranslate("Enable All Spawned Mobs") + " ?", Lang.GetTranslate("Are you sure you want to enable all Spawned Mobs") + " ?", Lang.GetTranslate("Enable"), Lang.GetTranslate("Do Not enable")))
                {
                    EnableSpawnedMobs();
                }
            }
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete All Spawned Mobs")))
            {
                if (EditorUtility.DisplayDialog(Lang.GetTranslate("Delete All Spawned Mobs") + " ?", Lang.GetTranslate("Are you sure you want to delete all Spawned Mobs") + " ?", Lang.GetTranslate("Delete"), Lang.GetTranslate("Do Not delete")))
                {
                    DeleteSpawnedMobs();
                }
            }
            pos.y += ImagePack.fieldHeight*2f;
            if (ImagePack.DrawButton(pos.x, pos.y, showQueryForm ? Lang.GetTranslate("Hide Query Form") : Lang.GetTranslate("Show Query Form")))
            {
                showQueryForm = !showQueryForm;
            }
            if (showQueryForm)
            {

                pos.y += ImagePack.fieldHeight;
               databaseId = ImagePack.DrawSelector(pos, Lang.GetTranslate("Database") + ":", databaseId, databaseOptions);
                pos.y += ImagePack.fieldHeight;
                GUI.Label(pos, Lang.GetTranslate("Query") + ":", ImagePack.FieldStyle());
                pos.height *= 4;
                descriptionScroll = GUI.BeginScrollView(pos, descriptionScroll, new Rect(0, 0, pos.width * 0.75f, 200));
                query = EditorGUI.TextArea(new Rect(115, 0, pos.width * 0.75f, 200), query, ImagePack.TextAreaStyle());
                GUI.EndScrollView();
                pos.height /= 4;
                pos.y += 4.2f * ImagePack.fieldHeight;
                Color temp = GUI.color;
                GUI.color = Color.red;
                ImagePack.DrawText(pos, "      " + Lang.GetTranslate("Attention, Only Inserts or updates can be made  !!! "));
                GUI.color = temp;
                pos.y += ImagePack.fieldHeight;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Run the query")))
                {
                    if (EditorUtility.DisplayDialog(Lang.GetTranslate("Run the query") + " ?", Lang.GetTranslate("Are you sure you want to run the query") + " ?", Lang.GetTranslate("Run"), Lang.GetTranslate("Do Not Run")))
                    {
                        RunDatabaseQuery();
                    }
                }
            }
        }

        void RunDatabaseQuery()
        {
            switch (databaseId)
            {
                //"Master", "Admin", "Atavism", "Content" 
                case 0:
                    string[] querys = query.Split(';');
                    foreach(string s in querys)
                        if (!string.IsNullOrEmpty(s))
                            DatabasePack.ExecuteNonQuery(DatabasePack.masterDatabasePrefix, s, 600);
                    break;
                case 1:
                    querys = query.Split(';');
                    foreach (string s in querys)
                        if(!string.IsNullOrEmpty(s))
                        DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, s, 600);
                    break;
                case 2:
                     querys = query.Split(';');
                    foreach (string s in querys)
                        if (!string.IsNullOrEmpty(s))
                            DatabasePack.ExecuteNonQuery(DatabasePack.atavismDatabasePrefix, s, 600);
                    break;
                case 3:
                    querys = query.Split(';');
                    foreach (string s in querys)
                        if (!string.IsNullOrEmpty(s))
                            DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, s, 600);
                    break;
            }

           // NewResult(Lang.GetTranslate("Query has been done"));


        }



        void DisableSpawnedMobs()
        {
            string sql = "update spawn_data set isactive = 0 where instance is not null;";
            DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, sql, 600);
        }

        void DeleteSpawnedMobs()
        {
            string sql = "Delete from spawn_data where instance is not null;";
            DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, sql, 600);
        }

        void EnableSpawnedMobs()
        {
            string sql = "update spawn_data set isactive = 1 where instance is not null;";
            DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, sql, 600);
        }



        void WipeCharacter()
        {
            string sql = "delete from player_character";
            DatabasePack.ExecuteNonQuery(DatabasePack.atavismDatabasePrefix, sql, 600);
            sql = "delete from objstore";
            DatabasePack.ExecuteNonQuery(DatabasePack.atavismDatabasePrefix, sql,300);
            sql = "delete from history_objstore";
            DatabasePack.ExecuteNonQuery(DatabasePack.atavismDatabasePrefix, sql,600);
            sql = "delete from player_items";
            DatabasePack.ExecuteNonQuery(DatabasePack.atavismDatabasePrefix, sql,600);
            sql = "delete from player_item_sockets";
            DatabasePack.ExecuteNonQuery(DatabasePack.atavismDatabasePrefix, sql,600);

            sql = "delete from account_character";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);
            sql = "delete from character_purchases";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);
            sql = "delete from auction_house";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);
            sql = "delete from auction_house_ended";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);
            sql = "delete from history_auction_house_ended";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);
            sql = "delete from character_block_list";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);
            sql = "delete from character_friends";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);
            sql = "delete from character_mail";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);
            sql = "delete from character_purchases";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);
            sql = "delete from cooldowns";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);
            sql = "delete from data_logs";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);
            sql = "delete from claim where permanent <> 1";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);
            sql = "update claim set cost = org_cost, currency = org_currency, instanceOwner=0, instanceGuild=-1,owner = -1, forSale = 1, sellerName ='' where parent > -1";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);
            sql = "delete from claim_action";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);
            sql = "delete from claim_object";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);
            sql = "delete from claim_permission";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);
             sql = "delete from guild";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);
            sql = "delete from guild_member";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);
            sql = "delete from guild_rank";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);

            sql = "delete from account_character";
            DatabasePack.ExecuteNonQuery(DatabasePack.masterDatabasePrefix, sql,600);
            sql = "delete from plugin_status";
            DatabasePack.ExecuteNonQuery(DatabasePack.atavismDatabasePrefix, sql,600);
            sql = "delete from world";
            DatabasePack.ExecuteNonQuery(DatabasePack.masterDatabasePrefix, sql,600);
        }

        void WipeAccounts()
        {
            WipeCharacter();
            string sql = "delete from account";
            DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, sql,600);
       
            sql = "delete from account";
            DatabasePack.ExecuteNonQuery(DatabasePack.masterDatabasePrefix, sql,600);
   
        }


        public void NewResult(string resultMessage)
        {
            result = resultMessage;
            resultTimeout = Time.realtimeSinceStartup + resultDuration;
        }
    }
}