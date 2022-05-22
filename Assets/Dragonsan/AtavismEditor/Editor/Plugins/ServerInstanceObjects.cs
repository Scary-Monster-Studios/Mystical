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
    // Handles the Effects Configuration
    public class ServerInstanceObjects : AtavismFunction
    {

        enum InstanceObjectsView
        {
            Home,
            InteractiveObjects,
            Regions,
            Graveyards,
            Claims
        } 
         
        // Result text
       // public new string result = "";
      //  public new float resultTimeout = -1;
     //   public new float resultDuration = 5;

        InstanceObjectsView view = InstanceObjectsView.Home;
        bool showConfirmDelete = false;
        int bulkEntryCount = 100;
        bool overideOwner =false;
        Dictionary<string,int> instanceList = new Dictionary<string, int>();
        // Use this for initialization
        public ServerInstanceObjects()
        {
            functionName = "Instance Objects";

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

            if (view == InstanceObjectsView.Home)
            {
                DrawHome(pos);
            }
            else if (view == InstanceObjectsView.InteractiveObjects)
            {
                DrawInteractiveObjects(pos);
            }
            else if (view == InstanceObjectsView.Regions)
            {
                DrawRegions(pos);
            }
            else if (view == InstanceObjectsView.Graveyards)
            {
                DrawGraveyards(pos);
            }
            else if (view == InstanceObjectsView.Claims)
            {
                DrawClaims(pos);
            }
        }

        void DrawHome(Rect pos)
        {
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Instance Objects"));
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Manage Interactive Objects")))
            {
                view = InstanceObjectsView.InteractiveObjects;
            }
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Manage Regions")))
            {
                view = InstanceObjectsView.Regions;
            }
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Manage Graveyards")))
            {
                view = InstanceObjectsView.Graveyards;
            }
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Manage Claims")))
            {
                view = InstanceObjectsView.Claims;
            }
            pos.y += ImagePack.fieldHeight;
        }

        void DrawInteractiveObjects(Rect pos)
        {
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Save Interactive Objects"));
            pos.width /= 2;
            pos.x += pos.width;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Back")))
            {
                view = InstanceObjectsView.Home;
            }
            pos.x -= pos.width;
            pos.width *= 2;
            pos.y += ImagePack.fieldHeight * 2;
            ImagePack.DrawText(pos, Lang.GetTranslate("Save the Interactive Objects in the current scene by clicking below."));
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("When the Interactive Objects have been saved, save your scene again."));
            pos.y += ImagePack.fieldHeight;
            bulkEntryCount = ImagePack.DrawField(pos, Lang.GetTranslate("Bulk Entry Count")+":", bulkEntryCount);
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Save Interactive Objects")))
            {
                GetSceneInteractiveObjects();
                showConfirmDelete = false;
            }

            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("To delete all Interactive Objects data from all open Scenes in the Database, click"));
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("Delete Interactive Objects Data."));
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Interactive Objects Data")))
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
                    ClearSavedInstanceInteractiveObjectsData();
                    showConfirmDelete = false;
                }
            }

            if (resultTimeout != -1 && resultTimeout > Time.realtimeSinceStartup)
            {
                pos.y += ImagePack.fieldHeight;
                ImagePack.DrawText(pos, result);
            }
        }

        void DrawRegions(Rect pos)
        {
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Save Regions"));
            pos.width /= 2;
            pos.x += pos.width;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Back")))
            {
                view = InstanceObjectsView.Home;
            }
            pos.x -= pos.width;
            pos.width *= 2;
            pos.y += ImagePack.fieldHeight * 2;
            ImagePack.DrawText(pos, Lang.GetTranslate("Save the Regions in the current scene by clicking below."));
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("When the Regions have been saved, save your scene again."));
            pos.y += ImagePack.fieldHeight;
            bulkEntryCount = ImagePack.DrawField(pos, Lang.GetTranslate("Bulk Entry Count")+":", bulkEntryCount);
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Save Regions")))
            {
                GetSceneRegions();
                showConfirmDelete = false;
            }

            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("To delete all region data from all open Scenes in the Database, click"));
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("Delete Region Data."));
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Region Data")))
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
                    ClearSavedInstanceRegionData();
                    showConfirmDelete = false;
                }
            }

            if (resultTimeout != -1 && resultTimeout > Time.realtimeSinceStartup)
            {
                pos.y += ImagePack.fieldHeight;
                ImagePack.DrawText(pos, result);
            }
        }

        void DrawGraveyards(Rect pos)
        {
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Save Graveyards"));
            pos.width /= 2;
            pos.x += pos.width;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Back")))
            {
                view = InstanceObjectsView.Home;
            }
            pos.x -= pos.width;
            pos.width *= 2;
            pos.y += ImagePack.fieldHeight * 2;
            ImagePack.DrawText(pos, Lang.GetTranslate("Save the Graveyards in the current scene by clicking below."));
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("When the Graveyards have been saved, save your scene again."));
            pos.y += ImagePack.fieldHeight;
            bulkEntryCount = ImagePack.DrawField(pos, Lang.GetTranslate("Bulk Entry Count")+":", bulkEntryCount);
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Save Graveyards")))
            {
                GetSceneGraveyards();
                showConfirmDelete = false;
            }

            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("To delete all graveyard data from all open Scenes in the Database, click"));
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("Delete Graveyard Data."));
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Graveyard Data")))
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
                    ClearSavedInstanceGraveyardData();
                    showConfirmDelete = false;
                }
            }

            if (resultTimeout != -1 && resultTimeout > Time.realtimeSinceStartup)
            {
                pos.y += ImagePack.fieldHeight;
                ImagePack.DrawText(pos, result);
            }
        }

        void DrawClaims(Rect pos)
        {
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Save Claims"));
            pos.width /= 2;
            pos.x += pos.width;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Back")))
            {
                view = InstanceObjectsView.Home;
            }
            pos.x -= pos.width;
            pos.width *= 2;
            pos.y += ImagePack.fieldHeight * 2;
            ImagePack.DrawText(pos, Lang.GetTranslate("Save the Claims in the current scene by clicking below."));
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("When the Claims have been saved, save your scene again."));
            pos.y += ImagePack.fieldHeight;
            bulkEntryCount = ImagePack.DrawField(pos, Lang.GetTranslate("Bulk Entry Count")+":", bulkEntryCount);
            pos.y += ImagePack.fieldHeight;
            overideOwner = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Overide owner, price")+":", overideOwner);

            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Save Claims")))
            {
                GetSceneClaims();
                showConfirmDelete = false;
            }

            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("To delete all claim data from all open Scenes in the Database, click"));
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("Delete Claim Data."));
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Claim Data")))
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
                    ClearSavedInstanceClaimData();
                    showConfirmDelete = false;
                }
            }

            if (resultTimeout != -1 && resultTimeout > Time.realtimeSinceStartup)
            {
                pos.y += ImagePack.fieldHeight;
                ImagePack.DrawText(pos, result);
            }
        }

        #region Interactive Objects Functions
        void GetSceneInteractiveObjects()
        {
            instanceList.Clear();
            AtavismLogger.LogInfoMessage("Interactive Object saving start time: " + System.DateTime.Now);
            NewResult(Lang.GetTranslate("Saving..."));
            //string instance = EditorApplication.currentScene;
            string instance = EditorSceneManager.GetActiveScene().name;

            string[] split = instance.Split(char.Parse("/"));
            instance = split[split.Length - 1];
            split = instance.Split(char.Parse("."));
            instance = split[0];

          //  int instanceID = ServerInstances.GetInstanceID(instance);

            List<InteractiveObject> preexistingNodes = new List<InteractiveObject>();
            List<InteractiveObject> newNodes = new List<InteractiveObject>();

            // Find all resource nodes in the scene
            int itemID = -1;
            InteractiveObject[] interactiveObjects = FindObjectsOfType<InteractiveObject>();
            string query = "";
            foreach (InteractiveObject iObj in interactiveObjects)
            {
                if (iObj.id > 0)
                {
                    preexistingNodes.Add(iObj);
                }
                else
                {
                    newNodes.Add(iObj);
                }
            }
            AtavismLogger.LogDebugMessage("Iteractive Objects saving found new=" + newNodes.Count + " exist=" + preexistingNodes.Count);


            string insertNodeQuery = "INSERT INTO interactive_object (name, instance, locX, locY, locZ, interactionType, interactionID, interactionData1,"
                    + "interactionData2, interactionData3, questReqID, interactTimeReq, coordEffect, respawnTime)" + " values ";

            query = insertNodeQuery;
            int insertCount = 0;

            AtavismLogger.LogInfoMessage("Before Insert time: " + System.DateTime.Now);
            foreach (InteractiveObject obj in newNodes)
            {
                int instanceId = ServerInstances.GetInstanceID(obj.gameObject.scene.name);
                if (!instanceList.ContainsKey(obj.gameObject.scene.name))
                {
                    instanceList.Add(obj.gameObject.scene.name, instanceId);
                    AtavismLogger.LogDebugMessage("Interactive Objects saving found from instances =" + obj.gameObject.scene.name + " " + instanceId);
                }
                string coordEffect = "";
                if (obj.interactCoordEffect != null)
                    coordEffect = obj.interactCoordEffect.name;
                // Insert the new resource node
                if (insertCount > 0)
                    query += ",";
                query += "('" + obj.name + "'," + instanceId + "," + obj.transform.position.x + "," + obj.transform.position.y
                    + "," + obj.transform.position.z + ",'" + obj.interactionType + "'," + obj.interactionID + ",'" + obj.interactionData1
                        + "','" + obj.interactionData2 + "','" + obj.interactionData3 + "'," + obj.questReqID
                        + "," + obj.interactTimeReq + ",'" + coordEffect + "'," + obj.refreshDuration + ")";

                insertCount++;
                if (insertCount == bulkEntryCount)
                {
                    DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, query);
                    insertCount = 0;
                    query = insertNodeQuery;
                }

                obj.id = itemID;
                EditorUtility.SetDirty(obj);

                AtavismLogger.LogInfoMessage("Finished inserting node at: " + System.DateTime.Now);
            }

            if (insertCount != 0)
            {
                DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, query);
                insertCount = 0;
            }
            foreach (int instId in instanceList.Values)
            {

                // Now run a select of all id's in the database for this scene
                query = "Select id, locX, locY, locZ from interactive_object where instance = " + instId;
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
                                    foreach (InteractiveObject obj in newNodes)
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
            // Now deal with preexisting nodes
            foreach (InteractiveObject obj in preexistingNodes)
            {
                int instanceId = ServerInstances.GetInstanceID(obj.gameObject.scene.name);
                if (!instanceList.ContainsKey(obj.gameObject.scene.name))
                {
                    instanceList.Add(obj.gameObject.scene.name, instanceId);
                    AtavismLogger.LogDebugMessage("Interactive Objects saving found from instances =" + obj.gameObject.scene.name + " " + instanceId);
                }
                string coordEffect = "";
                if (obj.interactCoordEffect != null)
                    coordEffect = obj.interactCoordEffect.name;
                // Update the existing resource node
                query = "UPDATE interactive_object set name = '" + obj.name + "', locX = " + obj.transform.position.x
                    + ", locY = " + obj.transform.position.y + ", locZ = " + obj.transform.position.z + ", interactionType = '"
                        + obj.interactionType + "', interactionID = " + obj.interactionID + ", interactionData1 = '" + obj.interactionData1
                        + "', interactionData2 = '" + obj.interactionData2 + "', interactionData3 = '" + obj.interactionData3 + "', questReqID = "
                        + obj.questReqID + ", interactTimeReq = " + obj.interactTimeReq + ", coordEffect = '" + coordEffect
                        + "', respawnTime = " + obj.refreshDuration + " where id = " + obj.id;
                DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
            }

            NewResult(Lang.GetTranslate("Nodes saved"));
        }

        void ClearSavedInstanceInteractiveObjectsData()
        {
            NewResult(Lang.GetTranslate("Deleting..."));
            //string instance = EditorApplication.currentScene;
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                int instanceID = ServerInstances.GetInstanceID(EditorSceneManager.GetSceneAt(i).name);

                // Now delete all resource nodes in this instance
                Register delete2 = new Register("instance", "?instance", MySqlDbType.Int32, instanceID.ToString(), Register.TypesOfField.Int);
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "interactive_object", delete2, true);
            }
            // Finally, reset the id for each node in the scene
            UnityEngine.Object[] resourceNodes = FindObjectsOfType<InteractiveObject>();
            foreach (UnityEngine.Object resourceNodeObj in resourceNodes)
            {
                InteractiveObject obj = (InteractiveObject)resourceNodeObj;
                obj.id = -1;
                EditorUtility.SetDirty(obj);
            }
            NewResult(Lang.GetTranslate("Node data deleted"));
        }

        #endregion Interactive Objects Functions

        #region Region Functions
        void GetSceneRegions()
        {
            instanceList.Clear();
            AtavismLogger.LogInfoMessage("Region saving start time: " + System.DateTime.Now);
            NewResult(Lang.GetTranslate("Saving..."));
            //string instance = EditorApplication.currentScene;
            string instance = EditorSceneManager.GetActiveScene().name;
            string[] split = instance.Split(char.Parse("/"));
            instance = split[split.Length - 1];
            split = instance.Split(char.Parse("."));
            instance = split[0];

           // int instanceID = ServerInstances.GetInstanceID(instance);

            List<AtavismRegion> preexistingNodes = new List<AtavismRegion>();
            List<AtavismRegion> newNodes = new List<AtavismRegion>();

            // Find all resource nodes in the scene
            int itemID = -1;
            AtavismRegion[] regions = FindObjectsOfType<AtavismRegion>();
            string query = "";
            foreach (AtavismRegion region in regions)
            {
                if (region.id > 0)
                {
                    preexistingNodes.Add(region);
                }
                else
                {
                    newNodes.Add(region);
                }
            }
            AtavismLogger.LogDebugMessage("Regions saving found new=" + newNodes.Count + " exist=" + preexistingNodes.Count);

            string insertNodeQuery = "INSERT INTO region (name, instance, locX, locY, locZ, regionType, actionID, actionData1, actionData2, actionData3)" + " values ";

            query = insertNodeQuery;
            int insertCount = 0;

            AtavismLogger.LogInfoMessage("Before Insert time: " + System.DateTime.Now);
            foreach (AtavismRegion region in newNodes)
            {
                int instanceId = ServerInstances.GetInstanceID(region.gameObject.scene.name);
                if (!instanceList.ContainsKey(region.gameObject.scene.name))
                {
                    instanceList.Add(region.gameObject.scene.name, instanceId);
                    AtavismLogger.LogDebugMessage("Region saving found from instances =" + region.gameObject.scene.name + " " + instanceId);
                }
                // Insert the new resource node
                if (insertCount > 0)
                    query += ",";
                query += "('" + region.name + "'," + instanceId + "," + region.transform.position.x + "," + region.transform.position.y
                    + "," + region.transform.position.z + ",'" + region.regionType.ToString() + "'," + region.actionID
                        + ",'" + region.actionData1 + "','" + region.actionData2 + "','" + region.actionData3 + "')";
                //string select_query = "Select id from region where instance = " + instanceId+ " AND locX=" + region.transform.position.x + " AND locY=" + region.transform.position.y+ " AND locZ=" + region.transform.position.z;

                insertCount++;
                if (insertCount == bulkEntryCount)
                {
                    DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, query);
                    insertCount = 0;
                    query = insertNodeQuery;
                }

                region.id = itemID;
                EditorUtility.SetDirty(region);

                AtavismLogger.LogInfoMessage("Finished inserting node at: " + System.DateTime.Now);
            }

            if (insertCount != 0)
            {
                DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, query);
                insertCount = 0;
            }
            foreach (int instId in instanceList.Values)
            {

                // Now run a select of all id's in the database for this scene
                query = "Select id, locX, locY, locZ from region where instance = " + instId;
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
                                    foreach (AtavismRegion region in newNodes)
                                    {
                                        if (instId == instanceList[region.gameObject.scene.name] && region.id < 0)
                                        {
                                            if (Math.Abs(region.transform.position.x - loc.x) < 0.1f && Math.Abs(region.transform.position.y - loc.y) < 0.1f
                                            && Math.Abs(region.transform.position.z - loc.z) < 0.1f)
                                            {
                                                region.id = data.GetInt32("id");
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
                    NewResult("Error:"+ Lang.GetTranslate("Error occurred deleting old entries"));
                }
                finally
                {
                }
            }
            // Now go through each node again and save the drops
            insertNodeQuery = "INSERT INTO region_shape (regionID, locX, locY, locZ, shape, size1, size2, size3, loc2X, loc2Y, loc2Z, orientX, orientY"
                + ", orientZ, orientW) values ";
            query = insertNodeQuery;
            AtavismLogger.LogInfoMessage("Before drop insert time: " + System.DateTime.Now);
            foreach (AtavismRegion region in newNodes)
            {
                foreach (Collider collider in region.GetComponentsInChildren<Collider>())
                {
                    // Insert the resource drops
                    if (!(collider is SphereCollider) && !(collider is BoxCollider) && !(collider is CapsuleCollider))
                    {
                       AtavismLogger.LogWarning("Collider skipped for region " + itemID + " as it is the wrong type");
                        continue;
                    }

                    if (collider is SphereCollider)
                    {
                        if (insertCount > 0)
                            query += ",";
                        SphereCollider sphereCollider = (SphereCollider)collider;
                        query += GetSphereColliderInsertString(region.id, sphereCollider);
                    }
                    else if (collider is CapsuleCollider)
                    {
                        if (insertCount > 0)
                            query += ",";
                        CapsuleCollider capsuleCollider = (CapsuleCollider)collider;
                        query += GetCapsuleColliderInsertString(region.id, capsuleCollider);
                    }
                    else if (collider is BoxCollider)
                    {
                        if (insertCount > 0)
                            query += ",";
                        BoxCollider boxCollider = (BoxCollider)collider;
                        query += GetBoxColliderInsertString(region.id, boxCollider);
                    }
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
            AtavismLogger.LogInfoMessage("After drop insert time: " + System.DateTime.Now);

            // Now deal with preexisting nodes
            foreach (AtavismRegion region in preexistingNodes)
            {
                // Update the existing resource node
                query = "UPDATE region set name = '" + region.name + "', locX = " + region.transform.position.x
                    + ", locY = " + region.transform.position.y + ", locZ = " + region.transform.position.z + ", regionType = '"
                        + region.regionType.ToString() + "', actionID = " + region.actionID + ", actionData1 = '" + region.actionData1
                        + "', actionData2 = '" + region.actionData2 + "', actionData3 = '" + region.actionData3 + "' where id = " + region.id;
                DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

                // Delete the existing resource drops for this node - not the most efficient, but easier to do
                Register delete = new Register("regionID", "?regionID", MySqlDbType.Int32, region.id.ToString(), Register.TypesOfField.Int);
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "region_shape", delete, false);

                // And re-insert them
                foreach (Collider collider in region.GetComponentsInChildren<Collider>())
                {
                    // Insert the resource drops
                    if (!(collider is SphereCollider) && !(collider is BoxCollider) && !(collider is CapsuleCollider))
                    {
                        AtavismLogger.LogWarning("Collider skipped for region " + itemID + " as it is the wrong type");
                        continue;
                    }

                    query = "INSERT INTO region_shape (regionID, locX, locY, locZ, shape, size1, size2, size3, loc2X, loc2Y, loc2Z, orientX, orientY"
                        + ", orientZ, orientW) values ";

                    if (collider is SphereCollider)
                    {
                        SphereCollider sphereCollider = (SphereCollider)collider;
                        query += GetSphereColliderInsertString(region.id, sphereCollider);
                    }
                    else if (collider is CapsuleCollider)
                    {
                        CapsuleCollider capsuleCollider = (CapsuleCollider)collider;
                        query += GetCapsuleColliderInsertString(region.id, capsuleCollider);
                    }
                    else if (collider is BoxCollider)
                    {
                        BoxCollider boxCollider = (BoxCollider)collider;
                        query += GetBoxColliderInsertString(region.id, boxCollider);
                    }
                    DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, new List<Register>());
                }
            }

            NewResult(Lang.GetTranslate("Nodes saved"));
        }

        string GetSphereColliderInsertString(int id, SphereCollider collider)
        {
            
            Vector3 pos1 = collider.transform.position + collider.transform.rotation * collider.center;
            float scale = collider.transform.lossyScale.x;
            if (scale < collider.transform.lossyScale.y)
                scale = collider.transform.lossyScale.y;
            if (scale < collider.transform.lossyScale.z)
                scale = collider.transform.lossyScale.z;
            return "(" + id + "," + pos1.x + "," + pos1.y + "," + pos1.z
                + ",'sphere'," + (collider.radius * scale) + ",0,0,0,0,0,0,0,0,1)";
        }

        string GetCapsuleColliderInsertString(int id, CapsuleCollider collider)
        {
            Vector3 pos1 = collider.transform.position + collider.transform.rotation * collider.center;
            Vector3 pos2 = collider.transform.position + collider.transform.rotation * collider.center;
            float scale = 1f;
            if (collider.direction == 0)
            {
                pos1 += collider.transform.rotation * Vector3.right * (collider.height / 2);
                pos2 -= collider.transform.rotation * Vector3.right * (collider.height / 2);
                scale = collider.transform.lossyScale.y;
                if (scale < collider.transform.lossyScale.z)
                    scale = collider.transform.lossyScale.z;
            }
            else if (collider.direction == 1)
            {
                pos1 += collider.transform.rotation * Vector3.up * (collider.height / 2);
                pos2 -= collider.transform.rotation * Vector3.up * (collider.height / 2);
                scale = collider.transform.lossyScale.x;
                if (scale < collider.transform.lossyScale.z)
                    scale = collider.transform.lossyScale.z;
            }
            else if (collider.direction == 2)
            {
                pos1 += collider.transform.rotation * Vector3.forward * (collider.height / 2);
                pos2 -= collider.transform.rotation * Vector3.forward * (collider.height / 2);
                scale = collider.transform.lossyScale.x;
                if (scale < collider.transform.lossyScale.y)
                    scale = collider.transform.lossyScale.y;
            }
            return "(" + id + "," + pos1.x + "," + pos1.y + "," + pos1.z
                + ",'capsule'," + (collider.radius*scale) + ",0,0," + pos2.x + "," + pos2.y + "," + pos2.z + ",0,0,0,1)";
        }

        string GetBoxColliderInsertString(int id, BoxCollider collider)
        {
            Vector3 pos1 = collider.transform.position + collider.transform.rotation * collider.center;
           // collider.bounds.size.
           
            return "(" + id + ", " + pos1.x + ", " + pos1.y + ", " + pos1.z
            + ",'box'," + collider.bounds.size.x + "," + collider.bounds.size.y + "," + collider.bounds.size.z + ",0,0,0," + collider.transform.rotation.x
                + "," + collider.transform.rotation.y + "," + collider.transform.rotation.z + "," + collider.transform.rotation.w + ")";
            
        }

        void ClearSavedInstanceRegionData()
        {
            NewResult(Lang.GetTranslate("Deleting..."));
            //string instance = EditorApplication.currentScene;
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                int instanceID = ServerInstances.GetInstanceID(EditorSceneManager.GetSceneAt(i).name);

                DatabasePack.con = DatabasePack.Connect(DatabasePack.contentDatabasePrefix);

                string query = "delete from region_shape where regionID IN (Select id from region where instance = " + instanceID + ")";
                DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, query);

                // Now delete all resource nodes in this instance
                Register delete2 = new Register("instance", "?instance", MySqlDbType.Int32, instanceID.ToString(), Register.TypesOfField.Int);
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "region", delete2, true);
            }
            // Finally, reset the id for each node in the scene
            UnityEngine.Object[] resourceNodes = FindObjectsOfType<AtavismRegion>();
            foreach (UnityEngine.Object resourceNodeObj in resourceNodes)
            {
                AtavismRegion resourceNode = (AtavismRegion)resourceNodeObj;
                resourceNode.id = -1;
                EditorUtility.SetDirty(resourceNode);
            }
            NewResult(Lang.GetTranslate("Node data deleted"));
        }

        #endregion Region Functions

        #region Graveyard Functions
        void GetSceneGraveyards()
        {
            AtavismLogger.LogInfoMessage("Graveyard saving start time: " + System.DateTime.Now);
            NewResult(Lang.GetTranslate("Saving..."));
            //string instance = EditorApplication.currentScene;
            string instance = EditorSceneManager.GetActiveScene().name;
            string[] split = instance.Split(char.Parse("/"));
            instance = split[split.Length - 1];
            split = instance.Split(char.Parse("."));
            instance = split[0];

          //  int instanceID = ServerInstances.GetInstanceID(instance);

            List<AtavismGraveyard> preexistingNodes = new List<AtavismGraveyard>();
            List<AtavismGraveyard> newNodes = new List<AtavismGraveyard>();

            // Find all resource nodes in the scene
            int itemID = -1;
            AtavismGraveyard[] graveyards = FindObjectsOfType<AtavismGraveyard>();
            string query = "";
            foreach (AtavismGraveyard gy in graveyards)
            {
                if (gy.id > 0)
                {
                    preexistingNodes.Add(gy);
                }
                else
                {
                    newNodes.Add(gy);
                }
            }
            AtavismLogger.LogDebugMessage("Graveyards saving found new=" + newNodes.Count + " exist=" + preexistingNodes.Count);

            string insertNodeQuery = "INSERT INTO graveyard (name, instance, locX, locY, locZ, factionReq, factionRepReq)" + " values ";

            query = insertNodeQuery;
            int insertCount = 0;

            AtavismLogger.LogInfoMessage("Before Insert time: " + System.DateTime.Now);
            foreach (AtavismGraveyard gy in newNodes)
            {
                int instanceId = ServerInstances.GetInstanceID(gy.gameObject.scene.name);
                if (!instanceList.ContainsKey(gy.gameObject.scene.name))
                {
                    instanceList.Add(gy.gameObject.scene.name, instanceId);
                    AtavismLogger.LogDebugMessage("Graveyards saving found from instances =" + gy.gameObject.scene.name + " " + instanceId);
                }
                // Insert the new resource node
                if (insertCount > 0)
                    query += ",";
                query += "('" + gy.name + "'," + instanceId + "," + gy.transform.position.x + "," + gy.transform.position.y
                    + "," + gy.transform.position.z + "," + gy.factionID + ",1)";

                insertCount++;
                if (insertCount == bulkEntryCount)
                {
                    DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, query);
                    insertCount = 0;
                    query = insertNodeQuery;
                }

                gy.id = itemID;
                EditorUtility.SetDirty(gy);

                AtavismLogger.LogInfoMessage("Finished inserting node at: " + System.DateTime.Now);
            }

            if (insertCount != 0)
            {
                DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, query);
                insertCount = 0;
            }
            foreach (int instId in instanceList.Values)
            {
                // Now run a select of all id's in the database for this scene
                query = "Select id, locX, locY, locZ from graveyard where instance = " + instId;
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
                                    foreach (AtavismGraveyard gy in newNodes)
                                    {
                                        if (instId == instanceList[gy.gameObject.scene.name] && gy.id < 0)
                                        {
                                            if (Math.Abs(gy.transform.position.x - loc.x) < 0.1f && Math.Abs(gy.transform.position.y - loc.y) < 0.1f
                                            && Math.Abs(gy.transform.position.z - loc.z) < 0.1f)
                                            {
                                                gy.id = data.GetInt32("id");
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
                    NewResult("Error:"+ Lang.GetTranslate("Error occurred deleting old entries"));
                }
                finally
                {
                }
            }
            // Now deal with preexisting nodes
            foreach (AtavismGraveyard gy in preexistingNodes)
            {
                // Update the existing resource node
                query = "UPDATE graveyard set name = '" + gy.name + "', locX = " + gy.transform.position.x
                    + ", locY = " + gy.transform.position.y + ", locZ = " + gy.transform.position.z + ", factionReq = "
                        + gy.factionID + ", factionRepReq = 1 where id = " + gy.id;
                DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());
            }

            NewResult(Lang.GetTranslate("Nodes saved"));
        }

        void ClearSavedInstanceGraveyardData()
        {
            NewResult(Lang.GetTranslate("Deleting..."));
            //string instance = EditorApplication.currentScene;
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                int instanceID = ServerInstances.GetInstanceID(EditorSceneManager.GetSceneAt(i).name);

                // Now delete all resource nodes in this instance
                Register delete2 = new Register("instance", "?instance", MySqlDbType.Int32, instanceID.ToString(), Register.TypesOfField.Int);
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "graveyard", delete2, true);
            }
            // Finally, reset the id for each node in the scene
            UnityEngine.Object[] resourceNodes = FindObjectsOfType<AtavismGraveyard>();
            foreach (UnityEngine.Object resourceNodeObj in resourceNodes)
            {
                AtavismGraveyard gy = (AtavismGraveyard)resourceNodeObj;
                gy.id = -1;
                EditorUtility.SetDirty(gy);
            }
            NewResult(Lang.GetTranslate("Node data deleted"));
        }

        #endregion Graveyard Functions

        #region Claim Functions
        void GetSceneClaims()
        {

            AtavismLogger.LogInfoMessage("Claim saving start time: " + System.DateTime.Now);
            NewResult(Lang.GetTranslate("Saving..."));
            instanceList.Clear();
            //string instance = EditorApplication.currentScene;
            string instance = EditorSceneManager.GetActiveScene().name;
            string[] split = instance.Split(char.Parse("/"));
            instance = split[split.Length - 1];
            split = instance.Split(char.Parse("."));
            instance = split[0];

            //  int instanceID = ServerInstances.GetInstanceID(instance);

            List<AtavismClaimRegion> preexistingNodes = new List<AtavismClaimRegion>();
            List<AtavismClaimRegion> newNodes = new List<AtavismClaimRegion>();

            // Find all resource nodes in the scene
            int itemID = -1;
            AtavismClaimRegion[] claims = FindObjectsOfType<AtavismClaimRegion>();
            string query = "";
            foreach (AtavismClaimRegion gy in claims)
            {
                if (gy.id > 0)
                {
                    preexistingNodes.Add(gy);
                }
                else
                {
                    newNodes.Add(gy);
                }
            }
            AtavismLogger.LogDebugMessage("Claim saving found new =" + newNodes.Count + " exist=" + preexistingNodes.Count);
            string insertNodeQuery = "INSERT INTO claim (name, instance, locX, locY, locZ, claimType, owner, size, sizeZ, sizeY, forSale, sellerName,"
                + " cost, currency, purchaseItemReq, priority,permanent,org_cost ,org_currency ,object_limit_profile, taxCurrency, taxAmount, taxInterval, taxPeriodPay, taxPeriodSell) values ";

            query = insertNodeQuery;
            int insertCount = 0;

            AtavismLogger.LogInfoMessage("Before Insert time: " + System.DateTime.Now);
            foreach (AtavismClaimRegion gy in newNodes)
            {
                //  gy.gameObject.scene.name
                int instanceId = ServerInstances.GetInstanceID(gy.gameObject.scene.name);
                if (!instanceList.ContainsKey(gy.gameObject.scene.name))
                {
                    instanceList.Add(gy.gameObject.scene.name,instanceId);
                    AtavismLogger.LogDebugMessage("Claim saving found from instances =" + gy.gameObject.scene.name+" "+instanceId);
                }
                // Insert the new resource node
                if (insertCount > 0)
                    query += ",";
                //BoxCollider boxCollider = gy.GetComponent<BoxCollider>();
                string purchaseItemReqs = "";
                foreach (int purchaseItemReq in gy.deedItemIDs)
                {
                    purchaseItemReqs += purchaseItemReq + ",";
                }
                query += "('" + gy.name + "'," + instanceId + "," + gy.transform.position.x.ToString("0.00") + "," + gy.transform.position.y.ToString("0.00") + ","
                    + gy.transform.position.z.ToString("0.00") + "," + gy.claimType + ",-1," + gy.size.x.ToString("0.00") + "," + gy.size.z.ToString("0.00") + "," + gy.size.y.ToString("0.00") + ",1,'',"
                    + gy.cost + "," + gy.purchaseCurrency + ",'" + purchaseItemReqs + "',1,1," + gy.cost + "," + gy.purchaseCurrency + ","+gy.object_limit_profile+ ","+gy.taxCurrency+ ","+gy.taxCurrencyAmount+ ","+gy.taxInterval+ ","+gy.taxMaxTimePay+ ","+gy.taxMinTimeSell+")";

                insertCount++;
                if (insertCount == bulkEntryCount)
                {
                    AtavismLogger.LogWarning("Claim bulkEntryCount query: " + query);
                    DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, query);
                    insertCount = 0;
                    query = insertNodeQuery;
                }

                gy.id = itemID;
                EditorUtility.SetDirty(gy);

                AtavismLogger.LogInfoMessage("Finished inserting node at: " + System.DateTime.Now);
            }
          
            if (insertCount != 0)
            {
                AtavismLogger.LogWarning("Claim query: " + query);
                DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, query);
                insertCount = 0;
            }
            foreach (int instId in instanceList.Values)
            {
                // Now run a select of all id's in the database for this scene
                query = "Select id, locX, locY, locZ from claim where instance = " + instId;
                try
                {
                    // Open the connection
                    DatabasePack.Connect(DatabasePack.adminDatabasePrefix);
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
                                    foreach (AtavismClaimRegion gy in newNodes)
                                    {
                                        if (instId == instanceList[gy.gameObject.scene.name] && gy.id < 0)
                                        {
                                            if (Math.Abs(gy.transform.position.x - loc.x) < 0.1f && Math.Abs(gy.transform.position.y - loc.y) < 0.1f
                                                && Math.Abs(gy.transform.position.z - loc.z) < 0.1f)
                                            {
                                                gy.id = data.GetInt32("id");
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
                    NewResult("Error:"+ Lang.GetTranslate("Error occurred deleting old entries"));
                }
                finally
                {
                }
            }
            
            string inserUpgradetNodeQuery = "INSERT INTO claim_upgrade (claimID, locX, locY, locZ, sizeX, sizeZ, sizeY, purchaseItemReq, cost, currency,object_limit_profile,taxCurrency,taxAmount,taxInterval,taxPeriodPay,taxPeriodSell) values ";

            query = inserUpgradetNodeQuery;
            insertCount = 0;
            foreach (AtavismClaimRegion gy in newNodes)
            {
                for (int i = 0; i < gy.upgrades.Count; i++)
                {
                    if (insertCount > 0)
                        query += ",";
                    string purchaseItemReqs = "";
                    foreach (int purchaseItemReq in gy.upgrades[i].deedItemIDs)
                    {
                        purchaseItemReqs += purchaseItemReq + ",";
                    }

                    query += "(" + gy.id + "," + gy.upgrades[i].position.x.ToString("0.00") + "," + gy.upgrades[i].position.y.ToString("0.00") + "," + gy.upgrades[i].position.z.ToString("0.00")
                             + "," + gy.upgrades[i].size.x.ToString("0.00") + "," + gy.upgrades[i].size.z.ToString("0.00") + "," + gy.upgrades[i].size.y.ToString("0.00") + ",'"
                             + purchaseItemReqs + "'," + gy.upgrades[i].currencyAmount + "," + gy.upgrades[i].currency + ","+gy.upgrades[i].object_limit_profile+ ","+gy.upgrades[i].taxCurrency+ ","+gy.upgrades[i].taxCurrencyAmount+ ","+gy.upgrades[i].taxInterval+ ","+gy.upgrades[i].taxMaxTimePay+ ","+gy.upgrades[i].taxMinTimeSell+")";
                    insertCount++;
                    if (insertCount == bulkEntryCount)
                    {
                        AtavismLogger.LogWarning("Claim bulkEntryCount query: " + query);
                        DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, query);
                        insertCount = 0;
                        query = inserUpgradetNodeQuery;
                    }
                }
            }
            if (insertCount != 0)
            {
                AtavismLogger.LogWarning("Claim query: " + query);
                DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, query);
                insertCount = 0;
            }
           // instanceList.Clear();
            // Now deal with preexisting nodes
            string deleteUpdate = "";
            
            foreach (AtavismClaimRegion gy in preexistingNodes)
            {
                int instanceId = ServerInstances.GetInstanceID(gy.gameObject.scene.name);
                if (!instanceList.ContainsKey(gy.gameObject.scene.name))
                {
                    instanceList.Add(gy.gameObject.scene.name, instanceId);
                    AtavismLogger.LogDebugMessage("Claim saving found from instances =" + gy.gameObject.scene.name + " " + instanceId);
                }
                // Update the existing resource node
                query = "UPDATE claim set name = '" + gy.name + "', locX = " + gy.transform.position.x
                    + ", locY = " + gy.transform.position.y + ", locZ = " + gy.transform.position.z
                    + ", claimType = " + (int)gy.claimType+", object_limit_profile="+gy.object_limit_profile+ ",taxCurrency="+gy.taxCurrency+ ",taxAmount="+gy.taxCurrencyAmount+ ",taxInterval="+gy.taxInterval+ ",taxPeriodPay="+gy.taxMaxTimePay+ ",taxPeriodSell="+gy.taxMinTimeSell;

                if (overideOwner)
                {
                  //  BoxCollider boxCollider = gy.GetComponent<BoxCollider>();
                    string purchaseItemReqs = "";
                    foreach (int purchaseItemReq in gy.deedItemIDs)
                    {
                        purchaseItemReqs += purchaseItemReq + ",";
                    }
                    query += ", owner=-1, size=" + gy.size.x + ", sizeY=" + gy.size.y+ ", sizeZ=" + gy.size.z + ",forSale=1,sellerName='', cost=" + gy.cost +
                        ", currency=" + gy.purchaseCurrency + ",purchaseItemReq='" + purchaseItemReqs + "' , org_cost=" + gy.cost+ ",org_currency =" + gy.purchaseCurrency;
                }
                query += " where id = " + gy.id;
                DatabasePack.Update(DatabasePack.adminDatabasePrefix, query, new List<Register>());
                if (insertCount > 0)
                    deleteUpdate += ",";
                deleteUpdate += gy.id + "";
                insertCount++;
            }
            if (insertCount != 0)
            {
                query = "DELETE FROM `claim_upgrade` WHERE claimID in (" + deleteUpdate + ")";
                AtavismLogger.LogWarning("Delete Claim upgrade query: " + query);
                DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, query);
                insertCount = 0;
            }
            query = inserUpgradetNodeQuery;
            insertCount = 0;
            foreach (AtavismClaimRegion gy in preexistingNodes)
            {
                for (int i = 0; i < gy.upgrades.Count; i++)
                {
                    if (insertCount > 0)
                        query += ",";
                    string purchaseItemReqs = "";
                    foreach (int purchaseItemReq in gy.upgrades[i].deedItemIDs)
                    {
                        purchaseItemReqs += purchaseItemReq + ",";
                    }

                    query += "(" + gy.id + "," + gy.upgrades[i].position.x.ToString("0.00") + "," + gy.upgrades[i].position.y.ToString("0.00") + "," + gy.upgrades[i].position.z.ToString("0.00")
                             + "," + gy.upgrades[i].size.x.ToString("0.00") + "," + gy.upgrades[i].size.z.ToString("0.00") + "," + gy.upgrades[i].size.y.ToString("0.00") + ",'"
                             + purchaseItemReqs + "'," + gy.upgrades[i].currencyAmount + "," + gy.upgrades[i].currency + ","+gy.upgrades[i].object_limit_profile+ ","+gy.upgrades[i].taxCurrency+ ","+gy.upgrades[i].taxCurrencyAmount+ ","+gy.upgrades[i].taxInterval+ ","+gy.upgrades[i].taxMaxTimePay+ ","+gy.upgrades[i].taxMinTimeSell+")";
                    insertCount++;
                    if (insertCount == bulkEntryCount)
                    {
                        AtavismLogger.LogWarning("Claim bulkEntryCount query: " + query);
                        DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, query);
                        insertCount = 0;
                        query = inserUpgradetNodeQuery;
                    }
                }
            }
            if (insertCount != 0)
            {
                AtavismLogger.LogWarning("Claim query: " + query);
                DatabasePack.ExecuteNonQuery(DatabasePack.adminDatabasePrefix, query);
                insertCount = 0;
            }
            
            NewResult(Lang.GetTranslate("Claims saved"));
        }

        void ClearSavedInstanceClaimData()
        {
            NewResult(Lang.GetTranslate("Deleting..."));
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                int instanceID = ServerInstances.GetInstanceID(EditorSceneManager.GetSceneAt(i).name);
                // Now delete all resource nodes in this instance
                Register delete2 = new Register("instance", "?instance", MySqlDbType.Int32, instanceID.ToString(), Register.TypesOfField.Int);
                DatabasePack.Delete(DatabasePack.adminDatabasePrefix, "claim", delete2, true);
            }
            // Finally, reset the id for each node in the scene
            UnityEngine.Object[] resourceNodes = FindObjectsOfType<AtavismClaimRegion>();
            foreach (UnityEngine.Object resourceNodeObj in resourceNodes)
            {
                AtavismClaimRegion gy = (AtavismClaimRegion)resourceNodeObj;
                gy.id = -1;
                EditorUtility.SetDirty(gy);
            }
            NewResult(Lang.GetTranslate("Node data deleted"));
        }

        public static int[] claimProfileIds = new int[] { -1 };
        
        public static GUIContent[] claimProfileOptions = new GUIContent[] { new GUIContent("~ none ~") };

        
        public static void LoadClaimObjectLimitProfile()
        {
            // Read all entries from the table
            string query = "SELECT id, name FROM claim_profile where isactive = 1";

            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                claimProfileOptions = new GUIContent[rows.Count + 1];
                claimProfileOptions[optionsId] = new GUIContent("~ none ~");
                claimProfileIds = new int[rows.Count + 1];
                claimProfileIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    claimProfileOptions[optionsId] = new GUIContent(data["name"]);
                    claimProfileIds[optionsId] = int.Parse(data["id"]);
                }
            }

        }
        
        #endregion Claim Functions

        public void NewResult(string resultMessage)
        {
            result = resultMessage;
            resultTimeout = Time.realtimeSinceStartup + resultDuration;
        }
    }
}