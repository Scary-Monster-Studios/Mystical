using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace Atavism
{
    public class EffectPrefab
    {

        // Prefab Parameters
        public EffectsData effectData;

        // Prefab file information
        private string prefabName;
        private string prefabPath;
        // Common Prefab Prefix and Sufix
        private string itemPrefix = "Effect";
        private string itemSufix = ".prefab";
        // Base path
        private string basePath = "";
        // Example Item Prefab Information
        private string basePrefab = "Example Effect Prefab.prefab";
        private string basePrefabPath;

        public EffectPrefab()
        {
            basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
            basePrefabPath = basePath + basePrefab;
        }

        public EffectPrefab(EffectsData effectData)
        {
            this.effectData = effectData;

            basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
            prefabName = itemPrefix + effectData.Name + itemSufix;
            prefabPath = basePath + prefabName;
            basePrefabPath = basePath + basePrefab;
        }

     /*   public void Save(EffectsData effectData)
        {
            this.effectData = effectData;

            this.Save();
        }

        // Save data from the class to the new prefab, creating one if it doesnt exist
        public void Save()
        {
            DeletePrefabWithIDAndDifferingName(effectData.id, effectData.Name);
            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

            // If this is a new prefab
            if (item == null)
            {
                AssetDatabase.CopyAsset(basePrefabPath, prefabPath);
                AssetDatabase.Refresh();
                item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            }

            item.GetComponent<AtavismEffect>().id = effectData.id;
            item.GetComponent<AtavismEffect>().name = effectData.Name;
            Sprite icon = (Sprite)AssetDatabase.LoadAssetAtPath(effectData.icon, typeof(Sprite));
            if (icon != null)
                item.GetComponent<AtavismEffect>().icon = icon;
            item.GetComponent<AtavismEffect>().tooltip = effectData.tooltip;

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

            GameObject effect = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

            // If this is a new prefab
            if (effect == null)
                return false;

            return true;
        }
        public string LoadIcon()
        {
            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            if (item == null)
                return "";
            if (item.GetComponent<AtavismEffect>().icon != null)
            {
                return AssetDatabase.GetAssetPath(item.GetComponent<AtavismEffect>().icon);
            }
            return "";

        }

        public static void DeleteAllPrefabs()
        {
            EffectPrefab temp = new EffectPrefab();
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
            EffectPrefab temp = new EffectPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                if (prefabPath != temp.basePrefabPath)
                {
                    GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                    if (!ids.Contains(item.GetComponent<AtavismEffect>().id))
                    {
                        AssetDatabase.DeleteAsset(prefabPath);
                    }
                }
            }
            AssetDatabase.Refresh();
        }

        public static void DeletePrefabWithIDAndDifferingName(int id, string name)
        {
            EffectPrefab temp = new EffectPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                if (item.GetComponent<AtavismEffect>() != null && item.GetComponent<AtavismEffect>().id == id
                        && item.GetComponent<AtavismEffect>().name != name)
                    AssetDatabase.DeleteAsset(prefabPath);
            }
            AssetDatabase.Refresh();
        }*/
    }
}