using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Atavism
{
    public class SkillPrefab
    {

        // Prefab Parameters
        public SkillsData skillData;

        // Prefab file information
        private string prefabName;
        private string prefabPath;
        // Common Prefab Prefix and Sufix
        private string itemPrefix = "Skill";
        private string itemSufix = ".prefab";
        // Base path
        private string basePath = "";
        // Example Item Prefab Information
        private string basePrefab = "Example Skill Prefab.prefab";
        private string basePrefabPath;

        public SkillPrefab()
        {
            basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
            basePrefabPath = basePath + basePrefab;
        }

        public SkillPrefab(SkillsData skillData)
        {
            this.skillData = skillData;

            basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
            prefabName = itemPrefix + skillData.Name + itemSufix;
            prefabPath = basePath + prefabName;
            basePrefabPath = basePath + basePrefab;
        }

      /*  public void Save(SkillsData skillData)
        {
            this.skillData = skillData;
            this.Save();
        }

        // Save data from the class to the new prefab, creating one if it doesnt exist
        public void Save()
        {
            DeletePrefabWithIDAndDifferingName(skillData.id, skillData.Name);
            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

            // If this is a new prefab
            if (item == null)
            {
                AssetDatabase.CopyAsset(basePrefabPath, prefabPath);
                AssetDatabase.Refresh();
                item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            }

            item.GetComponent<Skill>().id = skillData.id;
            item.GetComponent<Skill>().skillname = skillData.Name;
            Sprite icon = (Sprite)AssetDatabase.LoadAssetAtPath(skillData.icon, typeof(Sprite));
            if (icon != null)
                item.GetComponent<Skill>().icon = icon;
            item.GetComponent<Skill>().mainAspect = skillData.aspect;
            item.GetComponent<Skill>().type = skillData.type;

            item.GetComponent<Skill>().oppositeAspect = skillData.oppositeAspect;
            item.GetComponent<Skill>().mainAspectOnly = skillData.mainAspectOnly;
            item.GetComponent<Skill>().parentSkill = skillData.parentSkill;
            item.GetComponent<Skill>().parentSkillLevelReq = skillData.parentSkillLevelReq;
            item.GetComponent<Skill>().playerLevelReq = skillData.playerLevelReq;
            item.GetComponent<Skill>().pcost = skillData.skillPointCost;
            item.GetComponent<Skill>().talent = skillData.talent;
            // Save abilities
            item.GetComponent<Skill>().abilities = new List<int>();
            item.GetComponent<Skill>().abilityLevelReqs = new List<int>();
            foreach (SkillAbilityEntry skillAbility in skillData.skillAbilities)
            {
                if (skillAbility.abilityID > 0)
                {
                    item.GetComponent<Skill>().abilities.Add(skillAbility.abilityID);
                    item.GetComponent<Skill>().abilityLevelReqs.Add(skillAbility.skillLevelReq);
                }
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
        public string LoadIcon()
        {
            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            if (item == null)
                return "";
            if (item.GetComponent<Skill>().icon != null)
            {
                return AssetDatabase.GetAssetPath(item.GetComponent<Skill>().icon);
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

            skillData.id = item.GetComponent<Skill>().id;
            skillData.Name = item.GetComponent<Skill>().skillname;
            //skillData.icon = item.GetComponent<Skill>().icon;
            skillData.parentSkill = item.GetComponent<Skill>().parentSkill;
            skillData.parentSkillLevelReq = item.GetComponent<Skill>().parentSkillLevelReq;
            skillData.playerLevelReq = item.GetComponent<Skill>().playerLevelReq;

            return true;
        }

        public static void DeleteAllPrefabs()
        {
            SkillPrefab temp = new SkillPrefab();
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
            SkillPrefab temp = new SkillPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                if (prefabPath != temp.basePrefabPath)
                {
                    GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                    if (!ids.Contains(item.GetComponent<Skill>().id))
                    {
                        AssetDatabase.DeleteAsset(prefabPath);
                    }
                }
            }
            AssetDatabase.Refresh();
        }

        public static void DeletePrefabWithIDAndDifferingName(int id, string name)
        {
            SkillPrefab temp = new SkillPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                if (item.GetComponent<Skill>() != null && item.GetComponent<Skill>().id == id && item.GetComponent<Skill>().skillname != name)
                    AssetDatabase.DeleteAsset(prefabPath);
            }
            AssetDatabase.Refresh();
        }*/

    }
}