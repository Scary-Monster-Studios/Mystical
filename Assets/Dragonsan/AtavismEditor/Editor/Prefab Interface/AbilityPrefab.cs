using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace Atavism
{
    public class AbilityPrefab
    {

        // Prefab Parameters
        public AbilitiesData abilityData;

        // Prefab file information
        private string prefabName;
        private string prefabPath;
        // Common Prefab Prefix and Sufix
        private string itemPrefix = "Ability";
        private string itemSufix = ".prefab";
        // Base path
        private string basePath = "";
        // Example Item Prefab Information
        private string basePrefab = "Example Ability Prefab.prefab";
        private string basePrefabPath;

        public AbilityPrefab()
        {
            basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
            basePrefabPath = basePath + basePrefab;
        }

        public AbilityPrefab(AbilitiesData abilityData)
        {
            this.abilityData = abilityData;

            basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
            prefabName = itemPrefix + abilityData.Name + itemSufix;
            prefabPath = basePath + prefabName;
            basePrefabPath = basePath + basePrefab;
        }

    /*    public void Save(AbilitiesData abilityData)
        {
            this.abilityData = abilityData;

            this.Save();
        }*/

        // Save data from the class to the new prefab, creating one if it doesnt exist
    /*    public void Save()
        {
            DeletePrefabWithIDAndDifferingName(abilityData.id, abilityData.Name);
            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

            // If this is a new prefab
            if (item == null)
            {
                AssetDatabase.CopyAsset(basePrefabPath, prefabPath);
                AssetDatabase.Refresh();
                item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            }

            item.GetComponent<AtavismAbility>().id = abilityData.id;
            item.GetComponent<AtavismAbility>().name = abilityData.Name;
            Sprite icon = (Sprite)AssetDatabase.LoadAssetAtPath(abilityData.icon, typeof(Sprite));
            if (icon != null)
                item.GetComponent<AtavismAbility>().icon = icon;
            item.GetComponent<AtavismAbility>().tooltip = abilityData.tooltip;
            item.GetComponent<AtavismAbility>().cost = abilityData.activationCost;
            item.GetComponent<AtavismAbility>().costProperty = abilityData.activationCostType;
            item.GetComponent<AtavismAbility>().globalcd = abilityData.globalCooldown;
            item.GetComponent<AtavismAbility>().weaponcd = abilityData.weaponCooldown;
            item.GetComponent<AtavismAbility>().cooldownType = abilityData.cooldown1Type;
            item.GetComponent<AtavismAbility>().cooldownLength = abilityData.cooldown1Duration;
            item.GetComponent<AtavismAbility>().weaponReq = abilityData.weaponRequired;
            item.GetComponent<AtavismAbility>().reagentReq = abilityData.reagentRequired;
            item.GetComponent<AtavismAbility>().distance = abilityData.maxRange;
            item.GetComponent<AtavismAbility>().castingInRun = abilityData.castingInRun;
            if (abilityData.targetType == "Enemy")
                item.GetComponent<AtavismAbility>().targetType = TargetType.Enemy;
            else if (abilityData.targetType == "Self")
                item.GetComponent<AtavismAbility>().targetType = TargetType.Self;
            else if (abilityData.targetType.Contains("Friend"))
                item.GetComponent<AtavismAbility>().targetType = TargetType.Friendly;
            else if (abilityData.targetType.Contains("none"))
                item.GetComponent<AtavismAbility>().targetType = TargetType.All;
            if (abilityData.targetType == "AoE Enemy")
                item.GetComponent<AtavismAbility>().targetType = TargetType.AoE_Enemy;
            else if (abilityData.targetType == "AoE Frendly")
                item.GetComponent<AtavismAbility>().targetType = TargetType.AoE_Friendly;
            else if (abilityData.targetType == "Group")
                item.GetComponent<AtavismAbility>().targetType = TargetType.Group;
            item.GetComponent<AtavismAbility>().passive = abilityData.passive;
            item.GetComponent<AtavismAbility>().castTime = abilityData.activationLength;
           // item.GetComponent<AtavismAbility>().reqLevel = abilityData.;

            EditorUtility.SetDirty(item);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }*/
     /*   public string LoadIcon()
        {
            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            if (item == null)
                return "";
            if (item.GetComponent<AtavismAbility>().icon != null)
            {
                return AssetDatabase.GetAssetPath(item.GetComponent<AtavismAbility>().icon);
            }
            return "";

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
    /*    public bool Load()
        {

            GameObject ability = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

            // If this is a new prefab
            if (ability == null)
                return false;

            abilityData = new AbilitiesData();
            abilityData.id = ability.GetComponent<AtavismAbility>().id;
            abilityData.Name = ability.GetComponent<AtavismAbility>().name;
            //abilityData.icon = ability.GetComponent<AtavismAbility>().icon;
            abilityData.tooltip = ability.GetComponent<AtavismAbility>().tooltip;
            abilityData.activationCost = ability.GetComponent<AtavismAbility>().cost;
            abilityData.activationCostType = ability.GetComponent<AtavismAbility>().costProperty;
            abilityData.maxRange = ability.GetComponent<AtavismAbility>().distance;
            abilityData.activationLength = ability.GetComponent<AtavismAbility>().castTime;
            abilityData.passive = ability.GetComponent<AtavismAbility>().passive;
            abilityData.castingInRun = ability.GetComponent<AtavismAbility>().castingInRun;

            return true;
        }*/

   /*     public static void DeleteAllPrefabs()
        {
            AbilityPrefab temp = new AbilityPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                if (prefabPath != temp.basePrefabPath)
                    AssetDatabase.DeleteAsset(prefabPath);
            }
            AssetDatabase.Refresh();
        }*/

      /*  public static void DeletePrefabWithoutIDs(List<int> ids)
        {
            AbilityPrefab temp = new AbilityPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                if (prefabPath != temp.basePrefabPath)
                {
                    GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                    if (!ids.Contains(item.GetComponent<AtavismAbility>().id))
                    {
                        AssetDatabase.DeleteAsset(prefabPath);
                    }
                }
            }
            AssetDatabase.Refresh();
        }
        */
     /*   public static void DeletePrefabWithIDAndDifferingName(int id, string name)
        {
            AbilityPrefab temp = new AbilityPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                if (item.GetComponent<AtavismAbility>() != null && item.GetComponent<AtavismAbility>().id == id
                        && item.GetComponent<AtavismAbility>().name != name)
                    AssetDatabase.DeleteAsset(prefabPath);
            }
            AssetDatabase.Refresh();
        }*/

    }
}