using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Atavism
{
    public class AtavismMigration : EditorWindow
    {

        //  [MenuItem("Window/Atavism Migration to X.2")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(AtavismMigration));
        }

        Vector2 scrollPos;

        public void OnGUI()
        {
            GUILayout.Label("");
            if (GUILayout.Button("Migrate"))
            {
                if (DatabasePack.TestConnection(DatabasePack.contentDatabasePrefix, true))
                {
                    if (EditorUtility.DisplayDialog("",
                        Lang.GetTranslate("Are you sure you want to migrate data to version Atavism X.5") + " ?",
                        Lang.GetTranslate("OK"), Lang.GetTranslate("Cancel")))
                    {
                        List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
                        string query = "SELECT * FROM `server_version` ";
                        rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
                        if ((rows != null) && (rows.Count > 0))
                        {
                            foreach (Dictionary<string, string> data in rows)
                            {
                                string sql4 = "UPDATE `abilities` SET `damageType`='"+data["name"]+"' WHERE damageType = ''";
                                DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, sql4, 600);
                            }
                        }
                        
                        
                        
                        query = "SELECT * FROM abilities  WHERE abilityType ='AtackAbility'";
                        rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
                        if ((rows != null) && (rows.Count == 0))
                        {
                            Run();
                        }
                        else if ((rows != null) && (rows.Count > 0))
                        {
                            if (EditorUtility.DisplayDialog("",
                                Lang.GetTranslate("Data has been detected in one of the new tables are you sure you want to start data migration") + " ?", Lang.GetTranslate("OK"), Lang.GetTranslate("Cancel")))
                            {
                                Run();
                            }
                        }
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("", "Setup the connection to the database in the editor ",
                        Lang.GetTranslate("OK"), "");
                }
            }
        }


       // [MenuItem("Window/Atavism/Atavism Migration to X.5")]
        public static void Migrate()
        {
            if (DatabasePack.TestConnection(DatabasePack.contentDatabasePrefix, true))
            {
                if (EditorUtility.DisplayDialog("Atavism migration data to version X.4",
                    Lang.GetTranslate("Are you sure you want to migrate data to version Atavism X.5") + " ?",
                    Lang.GetTranslate("OK"), Lang.GetTranslate("Cancel")))
                {
                    List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
                    string query = "SELECT * FROM abilities  WHERE abilityType ='AttackAbility'";
                    rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
                    if ((rows != null) && (rows.Count == 0))
                    {
                        Run();
                    }
                    else if ((rows != null) && (rows.Count > 0))
                    {
                        if (EditorUtility.DisplayDialog("Atavism migration data to version X.5",
                            Lang.GetTranslate(
                                "Data has been detected in one of the new tables are you sure you want to start data migration") +
                            " ?", Lang.GetTranslate("OK"), Lang.GetTranslate("Cancel")))
                        {
                            Run();
                        }
                    }
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Atavism migration data to version X.5",
                    "Setup the connection to the basebase in the Atavism Editor", Lang.GetTranslate("OK"), "");
            }
        }

        static void Run()
        {
            
            //Abilities
            string sql1 = "UPDATE `abilities` SET `abilityType`='AttackAbility' WHERE abilityType ='CombatMeleeAbility'";
            DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, sql1, 600);
            string sql2 = "UPDATE `abilities` SET `abilityType`='AttackAbility' WHERE abilityType ='MagicalAttackAbility'";
            DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, sql2, 600);
            string sql3 = "UPDATE `abilities` SET `weaponRequired`='' WHERE weaponRequired = '~ none ~'";
            DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, sql3, 600);
         
                
            //Update Weapon Type
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            string query = "SELECT * FROM editor_option_choice where isactive = 1 and optionTypeID in (Select id from editor_option where optionType like 'Weapon Type')";
            if (rows != null)
                rows.Clear();
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    int id = int.Parse(data["id"]);
                    int type = 0;
                    string csql = "UPDATE `abilities` SET `weaponRequired`='"+id+"' WHERE weaponRequired = '"+data["choice"]+"'";
                    DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, csql, 600);
                }
            }
            
            query = "SELECT * FROM `damage_type` LIMIT 1 ";
            if (rows != null)
                rows.Clear();
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    string sql4 = "UPDATE `abilities` SET `damageType`='"+data["name"]+"' WHERE damageType = ''";
                    DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, sql4, 600);
                }
            }
            
            string sql5 = "UPDATE `effects` SET `effectType`='AttackEffect' WHERE effectType ='MeleeStrikeEffect'";
            DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, sql5, 600);
            string sql6 = "UPDATE `effects` SET `effectType`='AttackEffect' WHERE effectType ='MagicalStrikeEffect'";
            DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, sql6, 600);
            
            string sql7 = "UPDATE `effects` SET `effectType`='AttackDotEffect' WHERE effectType ='MagicalDotEffect'";
            DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, sql7, 600);
            string sql8 = "UPDATE `effects` SET `effectType`='AttackDotEffect' WHERE effectType ='PhysicalDotEffect'";
            DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, sql8, 600);

            string sql9 = "UPDATE `stat` SET `stat_function` = '' WHERE "+
                          "`stat_function` = 'Magical Accuracy' OR `stat_function` = 'Physical Accuracy' OR "+
                          "`stat_function` = 'Magical Critic' OR `stat_function` = 'Magical Critic Power' OR "+
                          "`stat_function` = 'Physical Critic' OR `stat_function` = 'Physical Critic Power' OR "+
                          "`stat_function` = 'Physical Evasion' OR `stat_function` = 'Magical Evasion' OR "+
                          "`stat_function` = 'Magical Power' OR `stat_function` = 'Physical Power'";
            DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, sql9, 600);

            EditorUtility.DisplayDialog("Atavism migration data to version X.5", "Migration was successful",
                Lang.GetTranslate("OK"), "");
            Debug.Log("Migration was successful");

        }

    }
}