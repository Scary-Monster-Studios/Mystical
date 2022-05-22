using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Atavism
{
    // Handles the Effects Configuration
    public class ServerResourceNodes : AtavismFunction
    {
        // Result text
       // public new string result = "";
       // public new float resultTimeout = -1;
       // public new float resultDuration = 5;

        bool showConfirmDelete = false;
        int bulkEntryCount = 100;
        Dictionary<string, int> instanceList = new Dictionary<string, int>();

        // Use this for initialization
        public ServerResourceNodes()
        {
            functionName = "Resource Nodes";

            showConfirmDelete = false;
        }

        public override void Activate()
        {
        }

        public void LoadSelectList()
        {
            //string[] selectList = new string[dataRegister.Count];
            /*displayList =  new string[dataRegister.Count];
			int i = 0;
			foreach (int displayID in dataRegister.Keys) {
				//selectList [i] = displayID + ". " + dataRegister [displayID].name;
				displayList [i] = displayID + ". " + dataRegister [displayID].name;
				i++;
			}*/
            //displayList = new Combobox(selectList);
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

            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Save Resource Nodes"));
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("Save the Resource Nodes in the current scene by clicking below."));
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("When the Resource Nodes have been saved, save your scene again."));
            pos.y += ImagePack.fieldHeight;
            bulkEntryCount = ImagePack.DrawField(pos, Lang.GetTranslate("Bulk Entry Count")+":", bulkEntryCount);
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Save Nodes")))
            {
                GetSceneResourceNodes();
                showConfirmDelete = false;
            }

            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("To delete all node data from all open Scenes in the Database, click"));
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawText(pos, Lang.GetTranslate("Delete Node Data."));
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Node Data")))
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

        void GetSceneResourceNodes()
        {
            instanceList.Clear();
            AtavismLogger.LogInfoMessage("Resource saving start time: " + System.DateTime.Now);
            NewResult(Lang.GetTranslate("Saving..."));
            //string instance = EditorApplication.currentScene;
          /*  string instance = EditorSceneManager.GetActiveScene().name;
            string[] split = instance.Split(char.Parse("/"));
            instance = split[split.Length - 1];
            split = instance.Split(char.Parse("."));
            instance = split[0];

            int instanceID = ServerInstances.GetInstanceID(instance);*/

            List<ResourceNode> preexistingNodes = new List<ResourceNode>();
            List<ResourceNode> newNodes = new List<ResourceNode>();

            // Find all resource nodes in the scene
            int itemID = -1;
            UnityEngine.Object[] resourceNodes = FindObjectsOfType<ResourceNode>();
            string query = "";
            foreach (UnityEngine.Object resourceNodeObj in resourceNodes)
            {
                ResourceNode resourceNode = (ResourceNode)resourceNodeObj;
                if (resourceNode.id > 0)
                {
                    preexistingNodes.Add(resourceNode);
                }
                else
                {
                    newNodes.Add(resourceNode);
                }
            }
            AtavismLogger.LogDebugMessage("Resource Node found new =" + newNodes.Count + " exist=" + preexistingNodes.Count);

            string insertNodeQuery = "INSERT INTO resource_node_template (name,profileId"
            //" skill, skillLevel, skillLevelMax,skillExp ,weaponReq, equipped, gameObject, coordEffect"
                + ", instance," +
            //" respawnTime, " +
            "locX, locY, locZ" +
           // ", harvestCount, harvestTimeReq" +
            ")" + " values ";

            query = insertNodeQuery;
            int insertCount = 0;

            AtavismLogger.LogInfoMessage("Before Insert time: " + System.DateTime.Now);
            foreach (ResourceNode resourceNode in newNodes)
            {
                int instanceId = ServerInstances.GetInstanceID(resourceNode.gameObject.scene.name);
                if (!instanceList.ContainsKey(resourceNode.gameObject.scene.name))
                {
                    instanceList.Add(resourceNode.gameObject.scene.name, instanceId);
                    AtavismLogger.LogDebugMessage("Resource Node saving found from instances =" + resourceNode.gameObject.scene.name + " " + instanceId);
                }
                // Insert the new resource node
                string coordEffect = "";
             //   if (resourceNode.harvestCoordEffect != null)
             //       coordEffect = resourceNode.harvestCoordEffect.name;
                /*query = "INSERT INTO resource_node_template (name, skill, skillLevel, skillLevelMax, weaponReq, equipped, gameObject, coordEffect" 
                    + ", instance, respawnTime, locX, locY, locZ, harvestCount, harvestTimeReq)" + " values ('" + resourceNode.name + "'," + resourceNode.skillType + "," 
                    + resourceNode.reqSkillLevel + "," + resourceNode.skillLevelMax + "," + "'" + resourceNode.harvestTool + "'," 
                    + resourceNode.ToolMustBeEquipped + ",'','" + coordEffect + "','" + instance + "',"
                    + resourceNode.refreshDuration + "," + resourceNode.transform.position.x + "," + resourceNode.transform.position.y + "," 
                    + resourceNode.transform.position.z + "," + resourceNode.resourceCount + "," + resourceNode.timeToHarvest + ")";*/
                if (insertCount > 0)
                    query += ",";
                query += "('" + resourceNode.name + "',"+resourceNode.profileId+","/* + resourceNode.skillType + ","
                
                        + resourceNode.reqSkillLevel + "," + resourceNode.skillLevelMax + "," +resourceNode.skillExp+","+ "'" + resourceNode.harvestTool + "',"
                        + resourceNode.ToolMustBeEquipped + ",'','" + coordEffect + "'," */+ instanceId + ","
                       /* + resourceNode.refreshDuration + ","*/ + resourceNode.transform.position.x + "," + resourceNode.transform.position.y + ","
                        + resourceNode.transform.position.z /*+ "," + resourceNode.resourceCount + "," + resourceNode.timeToHarvest */+ ")";

                insertCount++;
                if (insertCount == bulkEntryCount)
                {
                    DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, query);
                    insertCount = 0;
                    query = insertNodeQuery;
                }
                /*itemID = DatabasePack.Insert (DatabasePack.contentDatabasePrefix, query, new List<Register>(), true);

                if (itemID == -1) {
                    // Insert failed, set message
                    return;
                }*/
                resourceNode.id = itemID;
                EditorUtility.SetDirty(resourceNode);

                /*foreach (ResourceDrop drop in resourceNode.resources) {
                    // Insert the resource drops
                    if (drop.item == null) {
                        Debug.LogWarning("ResourceDrop skipped for resourceNode " + itemID + " as it has no item");
                        continue;
                    }
                    query = "INSERT INTO resource_drop (resource_template, item, min, max, chance)"
                        + " values(" + itemID + "," + drop.item.TemplateId + "," + drop.min + "," + drop.max + "," + drop.chance + ")";
                    DatabasePack.Insert (DatabasePack.contentDatabasePrefix, query, new List<Register>(), true);
                }*/
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
                query = "Select id, locX, locY, locZ from resource_node_template where instance = " + instId;
                try
                {
                    //	List<int> dropsToDelete = new List<int>();
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
                                    foreach (ResourceNode resourceNode in newNodes)
                                    {
                                        if (instId == instanceList[resourceNode.gameObject.scene.name] && resourceNode.id < 0)
                                        {
                                            if (Math.Abs(resourceNode.transform.position.x - loc.x) < 0.1f && Math.Abs(resourceNode.transform.position.y - loc.y) < 0.1f
                                            && Math.Abs(resourceNode.transform.position.z - loc.z) < 0.1f)
                                            {
                                                resourceNode.id = data.GetInt32("id");
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
        /*    insertNodeQuery = "INSERT INTO resource_drop (resource_template, item, min, max, chance) values ";
            query = insertNodeQuery;
            AtavismLogger.LogInfoMessage("Before drop insert time: " + System.DateTime.Now);
           /* foreach (ResourceNode resourceNode in newNodes)
            {
                foreach (ResourceDrop drop in resourceNode.resources)
                {
                    // Insert the resource drops
                    if (drop == null || drop.itemId < 1 )
                    {
                        AtavismLogger.LogWarning("ResourceDrop skipped for resourceNode " + itemID + " as it has no item");
                        continue;
                    }
                    if (insertCount > 0)
                        query += ",";
                    query += "(" + resourceNode.id + "," + drop.itemId + "," + drop.min + "," + drop.max + "," + drop.chance + ")";
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
            }*/
            AtavismLogger.LogInfoMessage("After drop insert time: " + System.DateTime.Now);

            // Now deal with preexisting nodes
            foreach (ResourceNode resourceNode in preexistingNodes)
            {
                int instanceId = ServerInstances.GetInstanceID(resourceNode.gameObject.scene.name);
                if (!instanceList.ContainsKey(resourceNode.gameObject.scene.name))
                {
                    instanceList.Add(resourceNode.gameObject.scene.name, instanceId);
                    AtavismLogger.LogDebugMessage("Resource Node saving found from instances =" + resourceNode.gameObject.scene.name + " " + instanceId);
                }
                // Update the existing resource node
                string coordEffect = "";
              //  if (resourceNode.harvestCoordEffect != null)
              //      coordEffect = resourceNode.harvestCoordEffect.name;
                query = "UPDATE resource_node_template set name = '" + resourceNode.name + "', profileId=" +resourceNode.profileId +
                      //  ", skill = " + resourceNode.skillType + ", skillLevel = " + resourceNode.reqSkillLevel + ", skillLevelMax = " + resourceNode.skillLevelMax + ", skillExp="+resourceNode.skillExp+", weaponReq = '" + resourceNode.harvestTool
                   // + "', equipped = " + resourceNode.ToolMustBeEquipped + ", gameObject = '', coordEffect = '" + coordEffect + "'" +
                        ", instance = " + instanceId +// ", respawnTime = " + resourceNode.refreshDuration + "" +
                      ", locX = " + resourceNode.transform.position.x + ", locY = " + resourceNode.transform.position.y + ", locZ = " + resourceNode.transform.position.z //+ ", harvestCount = "
                    //+ resourceNode.resourceCount + ", harvestTimeReq = " + resourceNode.timeToHarvest 
                    + " where id = " + resourceNode.id;
                DatabasePack.Update(DatabasePack.contentDatabasePrefix, query, new List<Register>());

                // Delete the existing resource drops for this node - not the most efficient, but easier to do
             /*   Register delete = new Register("resource_template", "?resource_template", MySqlDbType.Int32, resourceNode.id.ToString(), Register.TypesOfField.Int);
                DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "resource_drop", delete, false);
*/
                // And re-insert them
              /*  foreach (ResourceDrop drop in resourceNode.resources)
                {
                    // Insert the resource drops
                    if (drop == null || drop.itemId < 1)
                    {
                        AtavismLogger.LogWarning("ResourceDrop skipped for resourceNode " + itemID + " as it has no item");
                        continue;
                    }
                    query = "INSERT INTO resource_drop (resource_template, item, min, max, chance)"
                        + " values(" + resourceNode.id + "," + drop.itemId + "," + drop.min + "," + drop.max + "," + drop.chance + ")";
                    DatabasePack.Insert(DatabasePack.contentDatabasePrefix, query, new List<Register>());
                }*/
            }

            NewResult(Lang.GetTranslate("Nodes saved"));
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
                Scene s = SceneManager.GetSceneAt(i);
                if (s.isLoaded)
                {
                    int instanceID = ServerInstances.GetInstanceID(s.name);

                    DatabasePack.con = DatabasePack.Connect(DatabasePack.contentDatabasePrefix);

                    //    string query = "delete from resource_drop where resource_template IN (Select id from resource_node_template where instance = " + instanceID + ")";
                    //     DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, query);

                    // Now delete all resource nodes in this instance
                    Register delete2 = new Register("instance", "?instance", MySqlDbType.Int32, instanceID.ToString(), Register.TypesOfField.Int);
                    DatabasePack.Delete(DatabasePack.contentDatabasePrefix, "resource_node_template", delete2, true);
                }
            }
            // Finally, reset the id for each node in the scene
            UnityEngine.Object[] resourceNodes = FindObjectsOfType<ResourceNode>();
            foreach (UnityEngine.Object resourceNodeObj in resourceNodes)
            {
                ResourceNode resourceNode = (ResourceNode)resourceNodeObj;
                resourceNode.id = -1;
                EditorUtility.SetDirty(resourceNode);
            }

            NewResult(Lang.GetTranslate("Node data deleted from all Opens Scenes")); 
        }

        public void NewResult(string resultMessage)
        {
            result = resultMessage;
            resultTimeout = Time.realtimeSinceStartup + resultDuration;
        }

    }
}