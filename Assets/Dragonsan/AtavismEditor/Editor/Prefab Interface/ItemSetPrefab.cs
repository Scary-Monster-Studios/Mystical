using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Atavism
{
    public class ItemSetPrefab
    {

        // Prefab Parameters
        public SetData itemSetData;

        // Prefab file information
        private string prefabName;
        private string prefabPath;
        // Common Prefab Prefix and Sufix
        private string itemPrefix = "ItemSet";
        private string itemSufix = ".prefab";
        // Base path
        private string basePath = "";
        // Example Item Prefab Information
        private string basePrefab = "Example ItemSet Prefab.prefab";
        private string basePrefabPath;

       // private int[] requirementIds = new int[] { -1 };
      //  private string[] requirementOptions = new string[] { "~ none ~" };

        private int[] skillIds = new int[] { -1 };
        private string[] skillOptions = new string[] { "~ none ~" };
       // private int[] classIds = new int[] { -1 };
       // private string[] classOptions = new string[] { "~ none ~" };

        //private int[] raceIds = new int[] { -1 };
       // private string[] raceOptions = new string[] { "~ none ~" };
        private string[] statOptions = new string[] { "~ none ~" };

        public ItemSetPrefab()
        {
            basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
            basePrefabPath = basePath + basePrefab;
        }

        public ItemSetPrefab(SetData itemSetData)
        {
            this.itemSetData = itemSetData;
        //    LoadSkillOptions();
            basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
            prefabName = itemPrefix + itemSetData.Name + itemSufix;
            prefabPath = basePath + prefabName;
            basePrefabPath = basePath + basePrefab;
        }
/*
        public void Save(SetData itemSetData)
        {
            this.itemSetData = itemSetData;
            this.Save();
        }

        // Save data from the class to the new prefab, creating one if it doesnt exist
        public void Save()
        {

            DeletePrefabWithIDAndDifferingName(itemSetData.id, itemSetData.Name);
            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

            // If this is a new prefab
            if (item == null)
            {
                AssetDatabase.CopyAsset(basePrefabPath, prefabPath);
                AssetDatabase.Refresh();
                item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            }

            item.GetComponent<AtavismInventoryItemSet>().Setid = itemSetData.id;
            item.GetComponent<AtavismInventoryItemSet>().Name = itemSetData.Name;
            item.GetComponent<AtavismInventoryItemSet>().itemList = itemSetData.itemList;

            item.GetComponent<AtavismInventoryItemSet>().ClearLevels();
            foreach (SetLevelData level in itemSetData.levelList)
            {
                AtavismInventoryItemSetLevel aiisl = new AtavismInventoryItemSetLevel();
                aiisl.DamageValue = level.damage;
                aiisl.DamageValuePercentage = level.damagep;
                aiisl.NumerOfParts = level.number_of_parts;

                foreach (StatEntry stat in level.stats)
                {
                    aiisl.itemStatName.Add(stat.itemStatName);
                    aiisl.itemStatValues.Add(stat.itemStatValue);
                    aiisl.itemStatValuesPercentage.Add(stat.itemStatValuePercentage);
                }

                item.GetComponent<AtavismInventoryItemSet>().levelList.Add(aiisl);
            }


            EditorUtility.SetDirty(item);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void Delete()
        {
            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

            // If this is a new prefab
            if (item != null)
            {
                AssetDatabase.DeleteAsset(prefabPath);
                AssetDatabase.Refresh();
            }
        }

        // Load data from the prefab base on its name
        // return true if the prefab exist and false if there is no prefab
        public bool Load()
        {

            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

            // If this is a new prefab
            if (item == null)
                return false;

            itemSetData = new SetData();
            itemSetData.id = item.GetComponent<AtavismInventoryItemSet>().Setid;
            itemSetData.Name = item.GetComponent<AtavismInventoryItemSet>().Name;
            //itemData.icon = item.GetComponent<AtavismInventoryItem>().icon;
            //itemSetData.levelList = item.GetComponent<AtavismInventoryItemSet>().levelList;
            itemSetData.itemList = item.GetComponent<AtavismInventoryItemSet>().itemList;


            return true;
        }

        public static void DeleteAllPrefabs()
        {
            ItemSetPrefab temp = new ItemSetPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                if (prefabPath != temp.basePrefabPath)
                    AssetDatabase.DeleteAsset(prefabPath);
            }
            AssetDatabase.Refresh();
        }


        public static void DeletePrefabWithoutIDs(List<int> ids)
        {
            ItemSetPrefab temp = new ItemSetPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                if (prefabPath != temp.basePrefabPath)
                {
                    GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                    if (!ids.Contains(item.GetComponent<AtavismInventoryItemSet>().Setid))
                    {
                        AssetDatabase.DeleteAsset(prefabPath);
                    }
                }
            }
            AssetDatabase.Refresh();
        }

        public static void DeletePrefabWithID(int id)
        {
            ItemSetPrefab temp = new ItemSetPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                if (item.GetComponent<AtavismInventoryItemSet>().Setid == id)
                    AssetDatabase.DeleteAsset(prefabPath);
            }
            AssetDatabase.Refresh();
        }

        public static void DeletePrefabWithIDAndDifferingName(int id, string name)
        {
            ItemSetPrefab temp = new ItemSetPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
             //   if (item.GetComponent<AtavismInventoryItemSet>() != null && item.GetComponent<AtavismInventoryItemSet>().Setid == id && item.GetComponent<AtavismInventoryItemSet>().name != name)
              //      AssetDatabase.DeleteAsset(prefabPath);
            }
            AssetDatabase.Refresh();
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
        public void LoadSkillOptions()
        {
            string query = "SELECT id, name FROM skills where isactive = 1";
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            // Read data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                skillOptions = new string[rows.Count + 1];
                skillOptions[optionsId] = "~ none ~";
                skillIds = new int[rows.Count + 1];
                skillIds[optionsId] = -1;
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    skillOptions[optionsId] = data["name"];
                    skillIds[optionsId] = int.Parse(data["id"]);
                }
            }
        }
        public void LoadStatOptions()
        {

            // Read all entries from the table
            string query = "SELECT name FROM stat where isactive = 1";
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();

            // If there is a row, clear it.
            if (rows != null)
                rows.Clear();

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                statOptions = new string[rows.Count + 1];
                statOptions[optionsId] = "~ none ~";
                foreach (Dictionary<string, string> data in rows)
                {
                    optionsId++;
                    statOptions[optionsId] = data["name"];
                }
            }
            
        }*/
    }
}