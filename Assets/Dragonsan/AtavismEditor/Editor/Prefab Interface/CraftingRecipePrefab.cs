using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace Atavism
{
    public class CraftingRecipePrefab
    {

        // Prefab Parameters
        public CraftingRecipe craftingRecipeData;

        // Prefab file information
        private string prefabName;
        private string prefabPath;
        // Common Prefab Prefix and Sufix
        private string itemPrefix = "Recipe";
        private string itemSufix = ".prefab";
        // Base path
        private string basePath = "";
        // Example Item Prefab Information
        private string basePrefab = "Example Recipe Prefab.prefab";
        private string basePrefabPath;

        public CraftingRecipePrefab()
        {
            basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
            basePrefabPath = basePath + basePrefab;
        }

        public CraftingRecipePrefab(CraftingRecipe craftingRecipeData)
        {
            this.craftingRecipeData = craftingRecipeData;

            basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
            prefabName = itemPrefix + craftingRecipeData.Name + itemSufix;
            prefabPath = basePath + prefabName;
            basePrefabPath = basePath + basePrefab;
        }
     /*   public string LoadIcon()
        {
            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            if (item == null)
                return "";
            if (item.GetComponent<AtavismCraftingRecipe>().icon != null)
            {
                return AssetDatabase.GetAssetPath(item.GetComponent<AtavismCraftingRecipe>().icon);
            }
            return "";

        }*/
      /*  public void Save(CraftingRecipe craftingRecipeData)
        {
            this.craftingRecipeData = craftingRecipeData;
            this.Save();
        }*/

        // Save data from the class to the new prefab, creating one if it doesnt exist
     /*   public void Save()
        {
            DeletePrefabWithIDAndDifferingName(craftingRecipeData.id, craftingRecipeData.Name);
            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

            // If this is a new prefab
            if (item == null)
            {
                AssetDatabase.CopyAsset(basePrefabPath, prefabPath);
                AssetDatabase.Refresh();
                item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            }

            item.GetComponent<AtavismCraftingRecipe>().recipeID = craftingRecipeData.id;
            item.GetComponent<AtavismCraftingRecipe>().recipeName = craftingRecipeData.Name;
            Sprite icon = (Sprite)AssetDatabase.LoadAssetAtPath(craftingRecipeData.icon, typeof(Sprite));
            if (icon != null)
                item.GetComponent<AtavismCraftingRecipe>().icon = icon;
            item.GetComponent<AtavismCraftingRecipe>().stationReq = craftingRecipeData.stationReq;
            item.GetComponent<AtavismCraftingRecipe>().skillID = craftingRecipeData.skillID;
            item.GetComponent<AtavismCraftingRecipe>().skillLevelReq = craftingRecipeData.skillLevelReq;
            item.GetComponent<AtavismCraftingRecipe>().creationTime = craftingRecipeData.creationTime;

            // Get Item reqs for first stage
            foreach (RecipeComponentEntry entry in craftingRecipeData.entries)
            {
                if (entry.itemId > 0)
                {
                    item.GetComponent<AtavismCraftingRecipe>().itemsReq.Add(entry.itemId);
                    item.GetComponent<AtavismCraftingRecipe>().itemsReqCounts.Add(entry.count);
                }
            }

            if (craftingRecipeData.resultItemID > 0)
            {
                item.GetComponent<AtavismCraftingRecipe>().createsItems.Add(craftingRecipeData.resultItemID);
                item.GetComponent<AtavismCraftingRecipe>().createsItemsCounts.Add(craftingRecipeData.resultItemCount);
            }
            if (craftingRecipeData.resultItem2ID > 0)
            {
                item.GetComponent<AtavismCraftingRecipe>().createsItems.Add(craftingRecipeData.resultItem2ID);
                item.GetComponent<AtavismCraftingRecipe>().createsItemsCounts.Add(craftingRecipeData.resultItem2Count);
            }
            if (craftingRecipeData.resultItem3ID > 0)
            {
                item.GetComponent<AtavismCraftingRecipe>().createsItems.Add(craftingRecipeData.resultItem3ID);
                item.GetComponent<AtavismCraftingRecipe>().createsItemsCounts.Add(craftingRecipeData.resultItem3Count);
            }
            if (craftingRecipeData.resultItem4ID > 0)
            {
                item.GetComponent<AtavismCraftingRecipe>().createsItems.Add(craftingRecipeData.resultItem4ID);
                item.GetComponent<AtavismCraftingRecipe>().createsItemsCounts.Add(craftingRecipeData.resultItem4Count);
            }

            if (craftingRecipeData.resultItem5ID > 0)
            {
                item.GetComponent<AtavismCraftingRecipe>().createsItems2.Add(craftingRecipeData.resultItem5ID);
                item.GetComponent<AtavismCraftingRecipe>().createsItemsCounts2.Add(craftingRecipeData.resultItem5Count);
            }
            if (craftingRecipeData.resultItem6ID > 0)
            {
                item.GetComponent<AtavismCraftingRecipe>().createsItems2.Add(craftingRecipeData.resultItem6ID);
                item.GetComponent<AtavismCraftingRecipe>().createsItemsCounts2.Add(craftingRecipeData.resultItem6Count);
            }
            if (craftingRecipeData.resultItem7ID > 0)
            {
                item.GetComponent<AtavismCraftingRecipe>().createsItems2.Add(craftingRecipeData.resultItem7ID);
                item.GetComponent<AtavismCraftingRecipe>().createsItemsCounts2.Add(craftingRecipeData.resultItem7Count);
            }
            if (craftingRecipeData.resultItem8ID > 0)
            {
                item.GetComponent<AtavismCraftingRecipe>().createsItems2.Add(craftingRecipeData.resultItem8ID);
                item.GetComponent<AtavismCraftingRecipe>().createsItemsCounts2.Add(craftingRecipeData.resultItem8Count);
            }

            if (craftingRecipeData.resultItem9ID > 0)
            {
                item.GetComponent<AtavismCraftingRecipe>().createsItems3.Add(craftingRecipeData.resultItem9ID);
                item.GetComponent<AtavismCraftingRecipe>().createsItemsCounts3.Add(craftingRecipeData.resultItem9Count);
            }
            if (craftingRecipeData.resultItem10ID > 0)
            {
                item.GetComponent<AtavismCraftingRecipe>().createsItems3.Add(craftingRecipeData.resultItem10ID);
                item.GetComponent<AtavismCraftingRecipe>().createsItemsCounts3.Add(craftingRecipeData.resultItem10Count);
            }
            if (craftingRecipeData.resultItem11ID > 0)
            {
                item.GetComponent<AtavismCraftingRecipe>().createsItems3.Add(craftingRecipeData.resultItem11ID);
                item.GetComponent<AtavismCraftingRecipe>().createsItemsCounts3.Add(craftingRecipeData.resultItem11Count);
            }
            if (craftingRecipeData.resultItem12ID > 0)
            {
                item.GetComponent<AtavismCraftingRecipe>().createsItems3.Add(craftingRecipeData.resultItem12ID);
                item.GetComponent<AtavismCraftingRecipe>().createsItemsCounts3.Add(craftingRecipeData.resultItem12Count);
            }

            if (craftingRecipeData.resultItem13ID > 0)
            {
                item.GetComponent<AtavismCraftingRecipe>().createsItems4.Add(craftingRecipeData.resultItem13ID);
                item.GetComponent<AtavismCraftingRecipe>().createsItemsCounts4.Add(craftingRecipeData.resultItem13Count);
            }
            if (craftingRecipeData.resultItem14ID > 0)
            {
                item.GetComponent<AtavismCraftingRecipe>().createsItems4.Add(craftingRecipeData.resultItem14ID);
                item.GetComponent<AtavismCraftingRecipe>().createsItemsCounts4.Add(craftingRecipeData.resultItem14Count);
            }
            if (craftingRecipeData.resultItem15ID > 0)
            {
                item.GetComponent<AtavismCraftingRecipe>().createsItems4.Add(craftingRecipeData.resultItem15ID);
                item.GetComponent<AtavismCraftingRecipe>().createsItemsCounts4.Add(craftingRecipeData.resultItem15Count);
            }
            if (craftingRecipeData.resultItem16ID > 0)
            {
                item.GetComponent<AtavismCraftingRecipe>().createsItems4.Add(craftingRecipeData.resultItem16ID);
                item.GetComponent<AtavismCraftingRecipe>().createsItemsCounts4.Add(craftingRecipeData.resultItem16Count);
            }


            /*for (int i = 1; i < craftingRecipeData.stages.Count; i++) {
                foreach (BuildObjectItemEntry entry in craftingRecipeData.stages[i].entries) {
                    if (entry.itemId > 0)
                        item.GetComponent<AtavismCraftingRecipe>().upgradeItemsReq.Add(entry.itemId);
                }
            }*/
/*
            EditorUtility.SetDirty(item);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }*/

      /*  public void Delete()
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

            craftingRecipeData = new CraftingRecipe();
            craftingRecipeData.id = item.GetComponent<AtavismCraftingRecipe>().recipeID;
            craftingRecipeData.Name = item.GetComponent<AtavismCraftingRecipe>().recipeName;
            //craftingRecipeData.icon = item.GetComponent<AtavismBuildObject>().icon;
            //craftingRecipeData.gameObject = item.GetComponent<AtavismCraftingRecipe>().gameObject;
            craftingRecipeData.skillID = item.GetComponent<AtavismCraftingRecipe>().skillID;
            craftingRecipeData.skillLevelReq = item.GetComponent<AtavismCraftingRecipe>().skillLevelReq;

            return true;
        }
        */
      /*  public static void DeleteAllPrefabs()
        {
            CraftingRecipePrefab temp = new CraftingRecipePrefab();
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
            CraftingRecipePrefab temp = new CraftingRecipePrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                if (prefabPath != temp.basePrefabPath)
                {
                    GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                    if (!ids.Contains(item.GetComponent<AtavismCraftingRecipe>().recipeID))
                    {
                        AssetDatabase.DeleteAsset(prefabPath);
                    }
                }
            }
            AssetDatabase.Refresh();
        }

        public static void DeletePrefabWithIDAndDifferingName(int id, string name)
        {
            CraftingRecipePrefab temp = new CraftingRecipePrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
          //      if (item.GetComponent<AtavismCraftingRecipe>() != null && item.GetComponent<AtavismCraftingRecipe>().recipeID == id && item.GetComponent<AtavismCraftingRecipe>().name != name)
            //        AssetDatabase.DeleteAsset(prefabPath);
            }
            AssetDatabase.Refresh();
        }*/

    }
}