using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace Atavism
{
    public class BuildObjectPrefab
    {

        // Prefab Parameters
        public BuildObjectData buildObjectData;

        // Prefab file information
        private string prefabName;
        private string prefabPath;
        // Common Prefab Prefix and Sufix
        private string itemPrefix = "BuildObject";
        private string itemSufix = ".prefab";
        // Base path
        private string basePath = "";
        // Example Item Prefab Information
        private string basePrefab = "Example BuildObject Prefab.prefab";
        private string basePrefabPath;

        public BuildObjectPrefab()
        {
            basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
            basePrefabPath = basePath + basePrefab;
        }

        public BuildObjectPrefab(BuildObjectData buildObjectData)
        {
            this.buildObjectData = buildObjectData;

            basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
            prefabName = itemPrefix + buildObjectData.Name + itemSufix;
            prefabPath = basePath + prefabName;
            basePrefabPath = basePath + basePrefab;
        }

        public string LoadIcon()
        {
            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            if (item == null)
                return "";
            if (item.GetComponent<AtavismBuildObjectTemplate>().icon != null)
            {
                return AssetDatabase.GetAssetPath(item.GetComponent<AtavismBuildObjectTemplate>().icon);
            }
            return "";

        }
      /*  public void Save(BuildObjectData buildObjectData)
        {
            this.buildObjectData = buildObjectData;
            this.Save();
        }*/

        // Save data from the class to the new prefab, creating one if it doesnt exist
      /*  public void Save()
        {
       //     DeletePrefabWithIDAndDifferingName(buildObjectData.id, buildObjectData.Name);
            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

            // If this is a new prefab
            if (item == null)
            {
                AssetDatabase.CopyAsset(basePrefabPath, prefabPath);
                AssetDatabase.Refresh();
                item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            }

            item.GetComponent<AtavismBuildObjectTemplate>().id = buildObjectData.id;
            item.GetComponent<AtavismBuildObjectTemplate>().buildObjectName = buildObjectData.Name;
            Sprite icon = (Sprite)AssetDatabase.LoadAssetAtPath(buildObjectData.icon, typeof(Sprite));
            if (icon != null)
                item.GetComponent<AtavismBuildObjectTemplate>().icon = icon;
            item.GetComponent<AtavismBuildObjectTemplate>().category = buildObjectData.category;
            item.GetComponent<AtavismBuildObjectTemplate>().skill = buildObjectData.skill;
            item.GetComponent<AtavismBuildObjectTemplate>().skillLevelReq = buildObjectData.skillLevelReq;
            item.GetComponent<AtavismBuildObjectTemplate>().distanceReq = buildObjectData.distanceReq;
            item.GetComponent<AtavismBuildObjectTemplate>().buildTaskReqPlayer = buildObjectData.buildTaskReqPlayer;
            item.GetComponent<AtavismBuildObjectTemplate>().validClaimTypes = (ClaimType)buildObjectData.validClaimType;
            item.GetComponent<AtavismBuildObjectTemplate>().onlyAvailableFromItem = buildObjectData.availableFromItemOnly;
            item.GetComponent<AtavismBuildObjectTemplate>().reqWeapon = buildObjectData.weaponReq;

            // Read in last stage to get gameObject
            BuildObjectStage lastStage = buildObjectData.stages[buildObjectData.stages.Count - 1];
            if (lastStage != null)
            {
                item.GetComponent<AtavismBuildObjectTemplate>().gameObject = lastStage.gameObject;
            }


            // Get Item reqs for first stage
            foreach (BuildObjectItemEntry entry in buildObjectData.stages[0].entries)
            {
                if (entry.itemId > 0)
                {
                    item.GetComponent<AtavismBuildObjectTemplate>().itemsReq.Add(entry.itemId);
                    item.GetComponent<AtavismBuildObjectTemplate>().itemsReqCount.Add(entry.count);
                }
            }

            for (int i = 1; i < buildObjectData.stages.Count; i++)
            {
                foreach (BuildObjectItemEntry entry in buildObjectData.stages[i].entries)
                {
                    if (entry.itemId > 0)
                        item.GetComponent<AtavismBuildObjectTemplate>().upgradeItemsReq.Add(entry.itemId);
                }
            }

            EditorUtility.SetDirty(item);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }*/

     /*   public void Delete()
        {
            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

            // If this is a new prefab
            if (item != null)
            {
                AssetDatabase.DeleteAsset(prefabPath);
                AssetDatabase.Refresh();
            }
        }*/

        // Load data from the prefab base on its name
        // return true if the prefab exist and false if there is no prefab
      /*  public bool Load()
        {

            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

            // If this is a new prefab
            if (item == null)
                return false;

            buildObjectData = new BuildObjectData();
            buildObjectData.id = item.GetComponent<AtavismBuildObjectTemplate>().id;
            buildObjectData.Name = item.GetComponent<AtavismBuildObjectTemplate>().buildObjectName;
            //buildObjectData.icon = item.GetComponent<AtavismBuildObject>().icon;
            //buildObjectData.gameObject = item.GetComponent<AtavismBuildObjectTemplate>().gameObject;
            buildObjectData.skill = item.GetComponent<AtavismBuildObjectTemplate>().skill;
            buildObjectData.skillLevelReq = item.GetComponent<AtavismBuildObjectTemplate>().skillLevelReq;
            buildObjectData.distanceReq = item.GetComponent<AtavismBuildObjectTemplate>().distanceReq;
            buildObjectData.availableFromItemOnly = item.GetComponent<AtavismBuildObjectTemplate>().onlyAvailableFromItem;

            return true;
        }*/

      /*  public static void DeleteAllPrefabs()
        {
            BuildObjectPrefab temp = new BuildObjectPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                if (prefabPath != temp.basePrefabPath)
                    AssetDatabase.DeleteAsset(prefabPath);
            }
            AssetDatabase.Refresh();
        }
        */
      /*  public static void DeletePrefabWithoutIDs(List<int> ids)
        {
            BuildObjectPrefab temp = new BuildObjectPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                if (prefabPath != temp.basePrefabPath)
                {
                    GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                    if (!ids.Contains(item.GetComponent<AtavismBuildObjectTemplate>().id))
                    {
                        AssetDatabase.DeleteAsset(prefabPath);
                    }
                }
            }
            AssetDatabase.Refresh();
        }
        */
    /*    public static void DeletePrefabWithIDAndDifferingName(int id, string name)
        {
            BuildObjectPrefab temp = new BuildObjectPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                if (item.GetComponent<AtavismBuildObjectTemplate>() != null && item.GetComponent<AtavismBuildObjectTemplate>().id == id
                        && item.GetComponent<AtavismBuildObjectTemplate>().name != name)
                    AssetDatabase.DeleteAsset(prefabPath);
            }
            AssetDatabase.Refresh();
        }
        */
    }
}