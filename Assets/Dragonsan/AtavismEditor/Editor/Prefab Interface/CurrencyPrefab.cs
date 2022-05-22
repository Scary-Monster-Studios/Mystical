using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace Atavism
{
    public class CurrencyPrefab
    {

        // Prefab Parameters
        public CurrencyData currencyData;
        public int convertsTo = -1;
        public int conversionAmountReq = 1;

        // Prefab file information
        private string prefabName;
        private string prefabPath;
        // Common Prefab Prefix and Sufix
        private string itemPrefix = "Currency";
        private string itemSufix = ".prefab";
        // Base path
        private string basePath = "";
        // Example Item Prefab Information
        private string basePrefab = "Example Currency Prefab.prefab";
        private string basePrefabPath;

        public CurrencyPrefab()
        {
            basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
            basePrefabPath = basePath + basePrefab;
        }

        public CurrencyPrefab(CurrencyData currencyData, int convertsTo, int conversionAmountReq)
        {
            this.currencyData = currencyData;
            this.convertsTo = convertsTo;
            this.conversionAmountReq = conversionAmountReq;

            basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
            prefabName = itemPrefix + currencyData.Name + itemSufix;
            prefabPath = basePath + prefabName;
            basePrefabPath = basePath + basePrefab;
        }
        public CurrencyPrefab(CurrencyData currencyData)
        {
            this.currencyData = currencyData;
            this.convertsTo = currencyData.currencyConversion[0].currencyToID;
            this.conversionAmountReq = currencyData.currencyConversion[0].amount;

            basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
            prefabName = itemPrefix + currencyData.Name + itemSufix;
            prefabPath = basePath + prefabName;
            basePrefabPath = basePath + basePrefab;
        }

      /*  public void Save(CurrencyData currencyData)
        {
            this.currencyData = currencyData;
            this.Save();
        }

        // Save data from the class to the new prefab, creating one if it doesnt exist
        public void Save()
        {
            DeletePrefabWithIDAndDifferingName(currencyData.id, currencyData.Name);
            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

            // If this is a new prefab
            if (item == null)
            {
                AssetDatabase.CopyAsset(basePrefabPath, prefabPath);
                AssetDatabase.Refresh();
                item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            }

            item.GetComponent<Currency>().id = currencyData.id;
            item.GetComponent<Currency>().name = currencyData.Name;
            Sprite icon = (Sprite)AssetDatabase.LoadAssetAtPath(currencyData.icon, typeof(Sprite));
            if (icon != null)
                item.GetComponent<Currency>().icon = icon;
            item.GetComponent<Currency>().group = currencyData.currencyGroup;
            item.GetComponent<Currency>().position = currencyData.currencyPosition;

            item.GetComponent<Currency>().convertsTo = convertsTo;
            item.GetComponent<Currency>().conversionAmountReq = conversionAmountReq;
            item.GetComponent<Currency>().max = currencyData.maximum;

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
        public string LoadIcon()
        {
            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            if (item == null)
                return "";
            if (item.GetComponent<Currency>().icon != null)
            {
                return AssetDatabase.GetAssetPath(item.GetComponent<Currency>().icon);
            }
            return "";

        }
        // Load data from the prefab base on its name
        // return true if the prefab exist and false if there is no prefab
        public bool Load()
        {

            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

            // If this is a new prefab
            if (item == null)
                return false;

            currencyData = new CurrencyData();
            currencyData.id = item.GetComponent<Currency>().id;
            currencyData.Name = item.GetComponent<Currency>().name;
            currencyData.currencyGroup = item.GetComponent<Currency>().group;
            currencyData.currencyPosition = item.GetComponent<Currency>().position;

            return true;
        }

        public static void DeleteAllPrefabs()
        {
            CurrencyPrefab temp = new CurrencyPrefab();
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
            CurrencyPrefab temp = new CurrencyPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                if (prefabPath != temp.basePrefabPath)
                {
                    GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                    if (!ids.Contains(item.GetComponent<Currency>().id))
                    {
                        AssetDatabase.DeleteAsset(prefabPath);
                    }
                }
            }
            AssetDatabase.Refresh();
        }

        public static void DeletePrefabWithIDAndDifferingName(int id, string name)
        {
            CurrencyPrefab temp = new CurrencyPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                if (item.GetComponent<Currency>() != null && item.GetComponent<Currency>().id == id
                        && item.GetComponent<Currency>().name != name)
                    AssetDatabase.DeleteAsset(prefabPath);
            }
            AssetDatabase.Refresh();
        }*/
    }
}