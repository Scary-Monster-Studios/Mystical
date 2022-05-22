using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Atavism
{
    // Handles the Spawn Configuration
    public class ServerMobSpawner : AtavismFunction
    {
        bool showConfirmDelete = false;
        int bulkEntryCount = 100;
        Dictionary<string, int> instanceList = new Dictionary<string, int>();

        private bool show_aggro = false;
        private bool show_roam = false;

        // Use this for initialization
        public ServerMobSpawner()
        {
            functionName = "Mob Spawner";

            showConfirmDelete = false;
        }

        public override void Activate()
        {
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

            // Draw the content database info
            //pos.y += ImagePack.fieldHeight;

            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Mob Spawn Marker"));
            pos.y += ImagePack.fieldHeight ;
            ImagePack.DrawText(pos, Lang.GetTranslate("Generate Mobs in the Mob Spawn Marker in the current scene by clicking below."));
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Generate")))
            {
                GenerateSceneMobSpawnMarker();
            }
            pos.y += ImagePack.fieldHeight * 1.1F ;
            ImagePack.DrawText(pos, Lang.GetTranslate("Reset Generated Mobs in the Mob Spawn Marker in the current scene by clicking below."));
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Reset")))
            {
                ResetSceneMobSpawnMarker();
            }
            pos.y += ImagePack.fieldHeight * 1.1F ;
            ImagePack.DrawText(pos, Lang.GetTranslate("Load Mobs from database for active scenes by clicking below."));
            pos.y += ImagePack.fieldHeight;

            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Load")))
            {
               
                Dictionary<int, GameObject> scenes = new Dictionary<int, GameObject>(); 
                    
                for(int i=0;i<SceneManager.sceneCount;i++ )
                {
                    Scene s = SceneManager.GetSceneAt(i);
                    if (s.isLoaded)
                    {
                        int instanceID = ServerInstances.GetInstanceID(s.name);
                        if (instanceID > 0)
                        {
                            GameObject g = GameObject.Find("SpawnData_" + instanceID);
                            if(g)
                                GameObject.DestroyImmediate(g);
                            g = new GameObject("SpawnData_" + instanceID);
                            SceneManager.MoveGameObjectToScene(g, s);
                            List<MobSpawnData> list =  ServerMobSpawnData.LoadEntities(instanceID);
                        //    Debug.LogError("Loaded Mobs For instance "+instanceID+" "+s.name+" count "+list.Count);
                            foreach (var msd in list)
                            {
                                Mob mob = ServerMobs.LoadMobTemplateModel(msd.mobTemplate);
                                if (mob.display1.Contains(".prefab"))
                                {
                                    int resourcePathPos = mob.display1.IndexOf("Resources/");
                                    mob.display1 = mob.display1.Substring(resourcePathPos + 10);
                                    mob.display1 = mob.display1.Remove(mob.display1.Length - 7);
                                }
                                GameObject prefab = (GameObject)UnityEngine.Resources.Load(mob.display1);
                                GameObject go = Instantiate(prefab, msd.position, msd.rotation);
                                
                                go.transform.SetParent(g.transform);
                                go.name = mob.Name + " (" + msd.mobTemplate + ") " + msd.id;
                                AtavismMobSpawnMarker amsm =  go.AddComponent<AtavismMobSpawnMarker>();
                                amsm.id = msd.id;
                                amsm.position = msd.position;
                                amsm.mobTemplateID = msd.mobTemplate;
                                amsm.mobTemplateID2 = msd.mobTemplate2;
                                amsm.mobTemplateID3 = msd.mobTemplate3;
                                amsm.mobTemplateID4 = msd.mobTemplate4;
                                amsm.mobTemplateID5 = msd.mobTemplate5;
                                amsm.respawnTime = msd.respawnTime/1000;
                                amsm.respawnTimeMax = msd.respawnTimeMax/1000;
                                amsm.despawnTime = msd.corpseDespawnTime/1000;

                                amsm.spawnActiveStartHour = msd.spawnActiveStartHour;
                                amsm.spawnActiveEndHour = msd.spawnActiveEndHour;
                                amsm.alternateMobTemplateID = msd.alternateSpawnMobTemplate;
                                amsm.alternateMobTemplateID2 = msd.alternateSpawnMobTemplate2;
                                amsm.alternateMobTemplateID3 = msd.alternateSpawnMobTemplate3;
                                amsm.alternateMobTemplateID4 = msd.alternateSpawnMobTemplate4;
                                amsm.alternateMobTemplateID5 = msd.alternateSpawnMobTemplate5;
                                amsm.roamRadius = msd.roamRadius;
                                amsm.patrolPath = msd.patrolPath;
                                amsm.isChest = msd.isChest;
                                
                                //amsm.pickupItemID = msd.pickupItemID;
                                amsm.merchantTable = msd.merchantTable;
                                string[] sq = msd.startsQuests.Split(',');
                                List<int> l = new List<int>();
                                foreach (var st in sq)
                                {
                                    if (st.Length > 0)
                                    {
                                        l.Add(int.Parse(st));
                                    }
                                }
                                amsm.startsQuests = l;
                                sq = msd.endsQuests.Split(',');
                                l = new List<int>();
                                foreach (var st in sq)
                                {
                                    if (st.Length > 0)
                                    {
                                        l.Add(int.Parse(st));
                                    }
                                }

                                amsm.endsQuests = l;
                                sq = msd.startsDialogues.Split(',');
                                l = new List<int>();
                                foreach (var st in sq)
                                {
                                    if (st.Length > 0)
                                    {
                                        l.Add(int.Parse(st));
                                    }
                                }

                                amsm.startsDialogues = l;

                                sq = msd.otherActions.Split(',');
                                List<string> sl = new List<string>();
                                foreach (var st in sq)
                                {
                                    sl.Add(st);
                                }

                                amsm.otherActions = sl;
                                amsm.hasCombat = msd.combat;
                                amsm.weaponSheathed = msd.weaponSheathed;
                               /* AtavismGetModel agm = go.GetComponent<AtavismGetModel>();
                                if (agm)
                                    agm.spawn();*/
                            }
                            
                            
                            scenes.Add(instanceID, g);
                        }
                    }
                }
         //       Debug.LogError("Spawner loading End "+scenes.Count);
               // SceneManager.sceneLoaded
                
            }
            pos.y += ImagePack.fieldHeight * 1.1F ;
            ImagePack.DrawText(pos, Lang.GetTranslate("Show Aggro Radius for all spawned Mob Spawn Marker in scenes."));
            pos.y += ImagePack.fieldHeight;

            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Show/Hide Aggro Radius")))
            {
                show_aggro = !show_aggro;
                UnityEngine.Object[] spawners = FindObjectsOfType<AtavismMobSpawnMarker>();
                foreach (UnityEngine.Object spawnersspawnerObj in spawners)
                {
                    AtavismMobSpawnMarker spawner = (AtavismMobSpawnMarker) spawnersspawnerObj;
                    spawner.show_aggro = show_aggro;
                }
            }
            pos.y += ImagePack.fieldHeight * 1.1F ;
            ImagePack.DrawText(pos, Lang.GetTranslate("Show Roam Radius for all spawned Mob Spawn Marker in scenes."));
            pos.y += ImagePack.fieldHeight;

            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Show/Hide Roam Radius")))
            {
                show_roam = !show_roam;
                UnityEngine.Object[] spawners2 = FindObjectsOfType<AtavismMobSpawnMarker>();
                foreach (UnityEngine.Object spawnersspawnerObj in spawners2)
                {
                    AtavismMobSpawnMarker spawner = (AtavismMobSpawnMarker)spawnersspawnerObj;
                    spawner.show_roam = show_roam;                
                }
            }
            
            
            pos.y += ImagePack.fieldHeight*1.2f;
            ImagePack.DrawText(pos, Lang.GetTranslate("Save the Mob Spawn Marker in the current scene by clicking below."));
        //    ImagePack.DrawText(pos, Lang.GetTranslate("When the Mob Spawn Marker have been saved, save your scene again."));
            pos.y += ImagePack.fieldHeight;
            bulkEntryCount = ImagePack.DrawField(pos, Lang.GetTranslate("Bulk Entry Count")+":", bulkEntryCount);
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Save")))
            {
                GetSceneMobSpawnMarker();
                showConfirmDelete = false;
            }

            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("To delete all spawn data from all open Scenes in the Database, click"));
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Spawn Data")))
            {
                showConfirmDelete = true;
            }

            if (showConfirmDelete)
            {
                pos.y += ImagePack.fieldHeight;
                ImagePack.DrawText(pos, Lang.GetTranslate("Are you sure")+"?");
                pos.y += ImagePack.fieldHeight;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Yes, delete")))
                {
                    ClearSavedInstanceNodeData();
                    showConfirmDelete = false;
                }
            }


            if (resultTimeout != -1 && resultTimeout > Time.realtimeSinceStartup)
            {
                pos.y += ImagePack.fieldHeight;
                ImagePack.DrawText(pos, result);
            }

        }

        void GenerateSceneMobSpawnMarker()
        {
            UnityEngine.Object[] spawners = FindObjectsOfType<AtavismMobSpawnMarkerGenerator>();
            int count = 0;
            int id = 1;
            foreach (UnityEngine.Object spawnersspawnerObj in spawners)
            {
                AtavismMobSpawnMarkerGenerator spawner = (AtavismMobSpawnMarkerGenerator) spawnersspawnerObj;
                if (spawner != null)
                {
                    Mob mob = ServerMobs.LoadMobTemplateModel(spawner.mobTemplateID);
                    count += spawner.Spawn(mob.display1, mob.aggro_range,id,spawners.Length);
                }

                id++;
            }
            EditorUtility.DisplayDialog("Atavism Spawn Ganarated", "Genarated "+count+" spawns", "OK", "");
        }

        void ResetSceneMobSpawnMarker()
        {
            UnityEngine.Object[] spawners = FindObjectsOfType<AtavismMobSpawnMarkerGenerator>();
            foreach (UnityEngine.Object spawnersspawnerObj in spawners)
            {
                AtavismMobSpawnMarkerGenerator spawner = (AtavismMobSpawnMarkerGenerator) spawnersspawnerObj;
                if (spawner != null)
                {
                    spawner.Clear();
                }
            }
        }

        
        
        
        void GetSceneMobSpawnMarker()
        {
            Dictionary<string,int> instanceList = new Dictionary<string, int>();

            AtavismLogger.LogInfoMessage("Mob Spawn Marker saving start time: " + System.DateTime.Now);
            NewResult(Lang.GetTranslate("Saving..."));
            List<AtavismMobSpawnMarker> newSpawners = new List<AtavismMobSpawnMarker>();
            List<AtavismMobSpawnMarker> preexistingSpawners = new List<AtavismMobSpawnMarker>();
            // Find all resource nodes in the scene
            int itemID = -1;
            UnityEngine.Object[] spawners = FindObjectsOfType<AtavismMobSpawnMarker>();
            string query = "";
            foreach (UnityEngine.Object spawnersspawnerObj in spawners)
            {
                AtavismMobSpawnMarker spawner = (AtavismMobSpawnMarker)spawnersspawnerObj;
            //   newSpawners.Add(spawner);
               if (spawner.id > 0)
               {
                   preexistingSpawners.Add(spawner);
               }
               else
               {
                   newSpawners.Add(spawner);
               }
            }
            
            AtavismLogger.LogDebugMessage("Mob Spawn Marker found new =" + newSpawners.Count + " exist=" + preexistingSpawners.Count );

            string insertNodeQuery = "INSERT INTO spawn_data (category,name,mobTemplate,mobTemplate2,mobTemplate3,mobTemplate4,mobTemplate5,markerName,locX,locY,locZ,orientX,orientY,orientZ,orientW,instance,numSpawns,"
                                     + "spawnRadius,respawnTime,respawnTimeMax,corpseDespawnTime,spawnActiveStartHour,spawnActiveEndHour,alternateSpawnMobTemplate,alternateSpawnMobTemplate2,alternateSpawnMobTemplate3,alternateSpawnMobTemplate4"
                                     +",alternateSpawnMobTemplate5,combat,roamRadius,startsQuests,endsQuests,startsDialogues,otherActions,baseAction,weaponSheathed,merchantTable,patrolPath,questOpenLootTable,isChest,pickupItem) values ";

            query = insertNodeQuery;
            int insertCount = 0;

            AtavismLogger.LogInfoMessage("Before Insert time: " + System.DateTime.Now);
            foreach (AtavismMobSpawnMarker sm in newSpawners)
            {
                int instanceID = ServerInstances.GetInstanceID(sm.gameObject.scene.name);
                if (!instanceList.ContainsKey(sm.gameObject.scene.name))
                {
                    instanceList.Add(sm.gameObject.scene.name, instanceID);
                 //   AtavismLogger.LogDebugMessage("Interactive Objects saving found from instances =" + sm.gameObject.scene.name + " " + instanceID);
                }
                string startsQuests = "";
                foreach (int questID in sm.startsQuests)
                {
                    startsQuests += questID + ",";
                }

                string endsQuests = "";
                foreach (int questID in sm.endsQuests)
                {
                    endsQuests += questID + ",";
                }

                string startsDialogues = "";
                foreach (int questID in sm.startsDialogues)
                {
                    startsDialogues += questID + ",";
                }

                string otherActions = "";
                foreach (string questID in sm.otherActions)
                {
                    otherActions += questID + ",";
                }

                string baseAction = "";
                var go = sm.gameObject;
                
                    if (go != null)
                    {
                        if (insertCount > 0)
                            query += ",";
                        query += "(1,'" + sm.name + "'," + sm.mobTemplateID + "," + sm.mobTemplateID2 + "," + sm.mobTemplateID3 + "," + sm.mobTemplateID4 + "," + sm.mobTemplateID5
                                 + ",''," + go.transform.position.x + "," + go.transform.position.y + "," + go.transform.position.z
                                 + "," + go.transform.rotation.x + "," + go.transform.rotation.y + "," + go.transform.rotation.z + "," + go.transform.rotation.w
                                 + "," + instanceID + "," + sm.numSpawns + "," + sm.spawnRadius + "," + sm.respawnTime *1000+ "," + sm.respawnTimeMax *1000+ ","+ sm.despawnTime *1000+ "," + sm.spawnActiveStartHour + "," + sm.spawnActiveEndHour
                                 + "," + sm.alternateMobTemplateID + "," + sm.alternateMobTemplateID2 + "," + sm.alternateMobTemplateID3 + "," + sm.alternateMobTemplateID4 + "," + sm.alternateMobTemplateID5 + ",1,"
                                 + sm.roamRadius + ",'" + startsQuests + "','" + endsQuests + "','" + startsDialogues + "','" + otherActions + "','"
                                 + baseAction + "',0," + sm.merchantTable + "," + sm.patrolPath + ",-1," + sm.isChest + "," + sm.pickupItemID + ")";
                        insertCount++;
                        if (insertCount == bulkEntryCount)
                        {
                            DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, query);
                            insertCount = 0;
                            query = insertNodeQuery;
                        }
                    }
                

            }



            if (insertCount != 0)
            {
                DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, query);
                insertCount = 0;
            }
            
             foreach (int instId in instanceList.Values)
            {

                // Now run a select of all id's in the database for this scene
                query = "Select id, locX, locY, locZ from spawn_data where instance = " + instId;
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
                                    Vector3 loc = new Vector3(data.GetFloat("locX"), data.GetFloat("locY"), data.GetFloat("locZ"));
                                    foreach (AtavismMobSpawnMarker obj in newSpawners)
                                    {
                                        if (instId == instanceList[obj.gameObject.scene.name] && obj.id < 0)
                                        {
                                            if (Math.Abs(obj.transform.position.x - loc.x) < 0.1f && Math.Abs(obj.transform.position.y - loc.y) < 0.1f
                                            && Math.Abs(obj.transform.position.z - loc.z) < 0.1f)
                                            {
                                                obj.id = data.GetInt32("id");
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            data.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.ToString());
                    NewResult("Error:"+Lang.GetTranslate("Error occurred deleting old entries"));
                }
                finally
                {
                }
            }
            
            
            foreach (AtavismMobSpawnMarker obj in preexistingSpawners)
            {
                int instanceId = ServerInstances.GetInstanceID(obj.gameObject.scene.name);
                if (!instanceList.ContainsKey(obj.gameObject.scene.name))
                {
                    instanceList.Add(obj.gameObject.scene.name, instanceId);
                    AtavismLogger.LogDebugMessage("Interactive Objects saving found from instances =" + obj.gameObject.scene.name + " " + instanceId);
                }

                string coordEffect = "";
                // Update the existing resource node
                string startsQuests = "";
                foreach (int questID in obj.startsQuests)
                {
                    startsQuests += questID + ",";
                }

                string endsQuests = "";
                foreach (int questID in obj.endsQuests)
                {
                    endsQuests += questID + ",";
                }

                string startsDialogues = "";
                foreach (int questID in obj.startsDialogues)
                {
                    startsDialogues += questID + ",";
                }

                string otherActions = "";
                foreach (string questID in obj.otherActions)
                {
                    otherActions += questID + ",";
                }

                string baseAction = "";

                    query = "UPDATE `spawn_data` SET `mobTemplate`=" + obj.mobTemplateID + ",`mobTemplate2`=" + obj.mobTemplateID2 + ",`mobTemplate3`=" + obj.mobTemplateID3 + ",`mobTemplate4`=" + obj.mobTemplateID4 + ",`mobTemplate5`=" + obj.mobTemplateID5 + "," +
                            "`locX`=" + obj.transform.position.x + ",`locY`=" + obj.transform.position.y + ",`locZ`=" + obj.transform.position.z + ",`orientX`=" + obj.transform.rotation.x + ",`orientY`=" + obj.transform.rotation.y + ",`orientZ`=" + obj.transform.rotation.z +
                            ",`orientW`=" + obj.transform.rotation.w + ",`respawnTime`=" + obj.respawnTime*1000 + ",`respawnTimeMax`=" + obj.respawnTimeMax*1000 + ",`corpseDespawnTime`=" + obj.despawnTime *1000+ ",`spawnActiveStartHour`=" + obj.spawnActiveStartHour + "," +
                            "`spawnActiveEndHour`=" + obj.spawnActiveEndHour + ",`alternateSpawnMobTemplate`=" + obj.alternateMobTemplateID + ",`alternateSpawnMobTemplate2`=" + obj.alternateMobTemplateID2 + ",`alternateSpawnMobTemplate3`=" + obj.alternateMobTemplateID3 + "," +
                            "`alternateSpawnMobTemplate4`=" + obj.alternateMobTemplateID4 + ",`alternateSpawnMobTemplate5`=" + obj.alternateMobTemplateID5 + ",`combat`=" + obj.hasCombat + ",`roamRadius`=" + obj.roamRadius + ",`startsQuests`='" + startsQuests + "',`endsQuests`='" 
                            + endsQuests + "',`startsDialogues`='" + startsDialogues + "',`otherActions`='" + otherActions + "',`baseAction`='" + baseAction + "',`weaponSheathed`=" + obj.weaponSheathed + ",`merchantTable`=" + obj.merchantTable + "," +
                            "`patrolPath`=" + obj.patrolPath + ",`isChest`=" + obj.isChest + " WHERE id = " + obj.id;
                    DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
               
                
            }

            NewResult(Lang.GetTranslate("Mob Spawn Markers saved"));
        }

        public static void SaveSpawner(AtavismMobSpawnMarker obj)
        {
            CultureInfo ci = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
             int instanceId = ServerInstances.GetInstanceID(obj.gameObject.scene.name);
               /* if (!instanceList.ContainsKey(obj.gameObject.scene.name))
                {
                    instanceList.Add(obj.gameObject.scene.name, instanceId);
                    AtavismLogger.LogDebugMessage("Interactive Objects saving found from instances =" + obj.gameObject.scene.name + " " + instanceId);
                }*/
                string coordEffect = "";
                // Update the existing resource node
                string startsQuests = "";
                foreach (int questID in obj.startsQuests)
                {
                    startsQuests += questID + ",";
                }

                string endsQuests = "";
                foreach (int questID in obj.endsQuests)
                {
                    endsQuests += questID + ",";
                }

                string startsDialogues = "";
                foreach (int questID in obj.startsDialogues)
                {
                    startsDialogues += questID + ",";
                }

                string otherActions = "";
                foreach (string questID in obj.otherActions)
                {
                    otherActions += questID + ",";
                }

                string baseAction = "";
                if (obj.id > 0)
                {
                    string query = "UPDATE `spawn_data` SET `mobTemplate`=" + obj.mobTemplateID + ",`mobTemplate2`=" + obj.mobTemplateID2 + ",`mobTemplate3`=" + obj.mobTemplateID3 + ",`mobTemplate4`=" + obj.mobTemplateID4 + ",`mobTemplate5`=" + obj.mobTemplateID5 + "," +
                                   "`locX`=" + obj.transform.position.x + ",`locY`=" + obj.transform.position.y + ",`locZ`=" + obj.transform.position.z + ",`orientX`=" + obj.transform.rotation.x + ",`orientY`=" + obj.transform.rotation.y + ",`orientZ`=" +
                                   obj.transform.rotation.z + ",`orientW`=" + obj.transform.rotation.w + "," +
                                   "`respawnTime`=" + obj.respawnTime*1000 + ",`respawnTimeMax`=" + obj.respawnTimeMax *1000+ ",`corpseDespawnTime`=" + obj.despawnTime *1000+ ",`spawnActiveStartHour`=" + obj.spawnActiveStartHour + ",`spawnActiveEndHour`=" + obj.spawnActiveEndHour + "," +
                                   "`alternateSpawnMobTemplate`=" + obj.alternateMobTemplateID + ",`alternateSpawnMobTemplate2`=" + obj.alternateMobTemplateID2 + ",`alternateSpawnMobTemplate3`=" + obj.alternateMobTemplateID3 + ",`alternateSpawnMobTemplate4`=" +
                                   obj.alternateMobTemplateID4 + "," +
                                   "`alternateSpawnMobTemplate5`=" + obj.alternateMobTemplateID5 + ",`combat`=" + obj.hasCombat + ",`roamRadius`=" + obj.roamRadius + ",`startsQuests`='" + startsQuests + "',`endsQuests`='" + endsQuests + "',`startsDialogues`='" + startsDialogues +
                                   "',`otherActions`='" + otherActions + "'," +
                                   "`baseAction`='" + baseAction + "',`weaponSheathed`=" + obj.weaponSheathed + ",`merchantTable`=" + obj.merchantTable + ",`patrolPath`=" + obj.patrolPath + ",`isChest`=" + obj.isChest + " WHERE id = " + obj.id;
                  //  Debug.LogError("sql "+query);
                    DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
                }
                else
                {
                    string insertQuery = "INSERT INTO spawn_data (category,name,mobTemplate,mobTemplate2,mobTemplate3,mobTemplate4,mobTemplate5,markerName,locX,locY,locZ,orientX,orientY,orientZ,orientW,instance,numSpawns,"
                                         + "spawnRadius,respawnTime,respawnTimeMax,corpseDespawnTime,spawnActiveStartHour,spawnActiveEndHour,alternateSpawnMobTemplate,alternateSpawnMobTemplate2,alternateSpawnMobTemplate3,alternateSpawnMobTemplate4"
                                         +",alternateSpawnMobTemplate5,combat,roamRadius,startsQuests,endsQuests,startsDialogues,otherActions,baseAction,weaponSheathed,merchantTable,patrolPath,questOpenLootTable,isChest,pickupItem) values ";
                    insertQuery += "(1,'" + obj.name + "'," + obj.mobTemplateID + "," + obj.mobTemplateID2 + "," + obj.mobTemplateID3 + "," + obj.mobTemplateID4 + "," + obj.mobTemplateID5 + ",''," + obj.transform.position.x + "," + obj.transform.position.y + "," + obj.transform.position.z
                                   + "," + obj.transform.rotation.x + "," + obj.transform.rotation.y + "," + obj.transform.rotation.z + "," + obj.transform.rotation.w + "," + instanceId + "," + obj.numSpawns + "," + obj.spawnRadius + "," + obj.respawnTime*1000 +"," + obj.respawnTimeMax *1000+ "," 
                                   + obj.despawnTime*1000 + "," + obj.spawnActiveStartHour + "," + obj.spawnActiveEndHour + "," + obj.alternateMobTemplateID + "," + obj.alternateMobTemplateID2 + "," + obj.alternateMobTemplateID3 + "," + obj.alternateMobTemplateID4 + "," + obj.alternateMobTemplateID5 
                                   + ",1," + obj.roamRadius + ",'" + startsQuests + "','" + endsQuests + "','" + startsDialogues + "','" + otherActions + "','" + baseAction + "',0," + obj.merchantTable + "," + obj.patrolPath + ",-1," + obj.isChest + "," + obj.pickupItemID + ")";
                  //  Debug.LogError("sql "+insertQuery);
                    int id =   DatabasePack.Insert(DatabasePack.contentDatabasePrefix, insertQuery,new List<Register>());
                    obj.id = id;
                }
        }

        public static void DeleteSpawner(AtavismMobSpawnMarker obj)
        {
            if (obj.id > 0)
            {
                Register delete2 = new Register("id", "?id", MySqlDbType.Int32, obj.id.ToString(), Register.TypesOfField.Int);
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "spawn_data", delete2, true);
            }


        }



        void ClearSavedInstanceNodeData()
        {
            NewResult(Lang.GetTranslate("Deleting..."));
            //string instance = EditorApplication.currentScene;
            string instance = EditorSceneManager.GetActiveScene().name;


            string[] split = instance.Split(char.Parse("/"));
            instance = split[split.Length - 1];
            split = instance.Split(char.Parse("."));
            instance = split[0];
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                int instanceID = ServerInstances.GetInstanceID(EditorSceneManager.GetSceneAt(i).name);

                DatabasePack.con = DatabasePack.Connect(DatabasePack.contentDatabasePrefix);

                // Now delete all resource nodes in this instance
                Register delete2 = new Register("instance", "?instance", MySqlDbType.Int32, instanceID.ToString(), Register.TypesOfField.Int);
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "spawn_data", delete2, true);
            }
           
            NewResult(Lang.GetTranslate("Spawn data deleted from all Opens Scenes")); 
        }

        public void NewResult(string resultMessage)
        {
            result = resultMessage;
            resultTimeout = Time.realtimeSinceStartup + resultDuration;
        }

    }
}