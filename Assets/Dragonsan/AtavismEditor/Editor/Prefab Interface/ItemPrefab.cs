using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Atavism
{
    public class ItemPrefab
    {

        // Prefab Parameters
        public ItemData itemData;

        // Prefab file information
        private string prefabName;
        private string prefabPath;
        // Common Prefab Prefix and Sufix
        private string itemPrefix = "Item";
        private string itemSufix = ".prefab";
        // Base path
        private string basePath = "";
        // Example Item Prefab Information
        private string basePrefab = "Example Item Prefab.prefab";
        private string basePrefabPath;

        private int[] requirementIds = new int[] { -1 };
        private string[] requirementOptions = new string[] { "~ none ~" };

        private int[] skillIds = new int[] { -1 };
        private string[] skillOptions = new string[] { "~ none ~" };
        private int[] classIds = new int[] { -1 };
        private string[] classOptions = new string[] { "~ none ~" };

        private int[] raceIds = new int[] { -1 };
        private string[] raceOptions = new string[] { "~ none ~" };
        private string[] statOptions = new string[] { "~ none ~" };

        public ItemPrefab()
        {
            basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
            basePrefabPath = basePath + basePrefab;
        }

        public ItemPrefab(ItemData itemData)
        {
            this.itemData = itemData;
          //  LoadSkillOptions();
            basePath = AtavismUnityUtility.GetAssetPath(basePrefab);
            prefabName = itemPrefix + itemData.Name + itemSufix;
            prefabPath = basePath + prefabName;
            basePrefabPath = basePath + basePrefab;
        }

      /*  public void Save(ItemData itemData)
        {
            this.itemData = itemData;
            this.Save();
        }

        // Save data from the class to the new prefab, creating one if it doesnt exist
        public void Save()
        {

            ServerOptionChoices.LoadAtavismChoiceOptions("Requirement", false, out requirementIds, out requirementOptions);
            ServerOptionChoices.LoadAtavismChoiceOptions("Race", false, out raceIds, out raceOptions);
            ServerOptionChoices.LoadAtavismChoiceOptions("Class", false, out classIds, out classOptions);
            DeletePrefabWithIDAndDifferingName(itemData.id, itemData.Name);
            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

            // If this is a new prefab
            if (item == null)
            {
                AssetDatabase.CopyAsset(basePrefabPath, prefabPath);
                AssetDatabase.Refresh();
                item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            }

            item.GetComponent<AtavismInventoryItem>().templateId = itemData.id;
            item.GetComponent<AtavismInventoryItem>().name = itemData.Name;
            Sprite icon = (Sprite)AssetDatabase.LoadAssetAtPath(itemData.icon, typeof(Sprite));
            if (icon != null)
                item.GetComponent<AtavismInventoryItem>().icon = icon;
            item.GetComponent<AtavismInventoryItem>().tooltip = itemData.toolTip;
            item.GetComponent<AtavismInventoryItem>().itemType = itemData.itemType;
            item.GetComponent<AtavismInventoryItem>().subType = itemData.subType;
            item.GetComponent<AtavismInventoryItem>().slot = itemData.slot;
            item.GetComponent<AtavismInventoryItem>().quality = itemData.itemQuality;
            item.GetComponent<AtavismInventoryItem>().currencyType = itemData.purchaseCurrency;
            item.GetComponent<AtavismInventoryItem>().cost = itemData.purchaseCost;
            item.GetComponent<AtavismInventoryItem>().binding = itemData.binding;
            item.GetComponent<AtavismInventoryItem>().sellable = itemData.sellable;
            item.GetComponent<AtavismInventoryItem>().DamageValue = itemData.damage;
            item.GetComponent<AtavismInventoryItem>().DamageMaxValue = itemData.damageMax;
            item.GetComponent<AtavismInventoryItem>().SetId = itemData.setId;
            item.GetComponent<AtavismInventoryItem>().EnchantId = itemData.enchant_profile_id;
            item.GetComponent<AtavismInventoryItem>().WeaponSpeed = (int)(itemData.delay * 1000);
            item.GetComponent<AtavismInventoryItem>().StackLimit = itemData.stackLimit;
            item.GetComponent<AtavismInventoryItem>().auctionHouse = itemData.auctionHouse;
            item.GetComponent<AtavismInventoryItem>().Unique = itemData.isUnique;
            item.GetComponent<AtavismInventoryItem>().gear_score = itemData.gear_score;

            item.GetComponent<AtavismInventoryItem>().ClearEffects();
            foreach (ItemEffectEntry effect in itemData.effects)
            {
                item.GetComponent<AtavismInventoryItem>().itemEffectTypes.Add(effect.itemEffectType);
                item.GetComponent<AtavismInventoryItem>().itemEffectNames.Add(effect.itemEffectName);
                item.GetComponent<AtavismInventoryItem>().itemEffectValues.Add(effect.itemEffectValue);
            }
            item.GetComponent<AtavismInventoryItem>().ClearRequirements();
            foreach (ItemTemplateOptionEntry option in itemData.itemTemplateOptions)
            {
                int selectedRequirement = GetOptionPosition(option.editor_option_type_id, requirementIds);


                item.GetComponent<AtavismInventoryItem>().itemReqTypes.Add(requirementOptions[selectedRequirement]);
                item.GetComponent<AtavismInventoryItem>().itemReqValues.Add(option.required_value.ToString());
                if (requirementOptions[selectedRequirement] == "Race")
                {
                    int raceID = 0;
                    int.TryParse(option.editor_option_choice_type_id, out raceID);
                    int selectedRace = GetOptionPosition(raceID, raceIds);
                    item.GetComponent<AtavismInventoryItem>().itemReqNames.Add(raceOptions[selectedRace]);
                    item.GetComponent<AtavismInventoryItem>().itemReqValues[item.GetComponent<AtavismInventoryItem>().itemReqValues.Count - 1] = option.editor_option_choice_type_id.ToString();

                }
                else if (requirementOptions[selectedRequirement] == "Class")
                {
                    int classID = 0;
                    int.TryParse(option.editor_option_choice_type_id, out classID);
                    int selectedClass = GetOptionPosition(classID, classIds);
                    item.GetComponent<AtavismInventoryItem>().itemReqNames.Add(classOptions[selectedClass]);
                    item.GetComponent<AtavismInventoryItem>().itemReqValues[item.GetComponent<AtavismInventoryItem>().itemReqValues.Count - 1] = option.editor_option_choice_type_id.ToString();

                }
                else if (requirementOptions[selectedRequirement] == "Level")
                {
                    item.GetComponent<AtavismInventoryItem>().itemReqNames.Add(option.editor_option_choice_type_id);
                }
                else if (requirementOptions[selectedRequirement] == "Skill Level")
                {
                    int skillID = 0;
                    int.TryParse(option.editor_option_choice_type_id, out skillID);
                    int selectedSkill = GetOptionPosition(skillID, skillIds);
                    item.GetComponent<AtavismInventoryItem>().itemReqNames.Add(skillOptions[selectedSkill]);
                }
                else if (requirementOptions[selectedRequirement] == "Stat")
                {
                    //   int statID = 0;
                    //   int.TryParse(option.editor_option_choice_type_id, out statID);

                    //  item.GetComponent<AtavismInventoryItem>().itemReqNames.Add(statOptions[statID]);
                    item.GetComponent<AtavismInventoryItem>().itemReqNames.Add(option.editor_option_choice_type_id);

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

        // Load data from the prefab base on its name
        // return true if the prefab exist and false if there is no prefab
        public bool Load()
        {

            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

            // If this is a new prefab
            if (item == null)
                return false;

            itemData = new ItemData();
            itemData.id = item.GetComponent<AtavismInventoryItem>().templateId;
            itemData.Name = item.GetComponent<AtavismInventoryItem>().name;
            //itemData.icon = item.GetComponent<AtavismInventoryItem>().icon;
            itemData.toolTip = item.GetComponent<AtavismInventoryItem>().tooltip;
            itemData.itemType = item.GetComponent<AtavismInventoryItem>().itemType;
            itemData.subType = item.GetComponent<AtavismInventoryItem>().subType;
            itemData.slot = item.GetComponent<AtavismInventoryItem>().slot;
            itemData.itemQuality = item.GetComponent<AtavismInventoryItem>().quality;
            itemData.purchaseCurrency = item.GetComponent<AtavismInventoryItem>().currencyType;
            itemData.purchaseCost = item.GetComponent<AtavismInventoryItem>().cost;
            itemData.sellable = item.GetComponent<AtavismInventoryItem>().sellable;
            itemData.setId = item.GetComponent<AtavismInventoryItem>().setId;

            return true;
        }

        public string LoadIcon()
        {
            GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            if (item == null)
                return "";
            if (item.GetComponent<AtavismInventoryItem>().icon != null)
            {
                return AssetDatabase.GetAssetPath(item.GetComponent<AtavismInventoryItem>().icon);
            }
            return "";

        }

        public static void DeleteAllPrefabs()
        {
            ItemPrefab temp = new ItemPrefab();
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
            ItemPrefab temp = new ItemPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                if (prefabPath != temp.basePrefabPath)
                {
                    GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                    if (!ids.Contains(item.GetComponent<AtavismInventoryItem>().TemplateId))
                    {
                        AssetDatabase.DeleteAsset(prefabPath);
                    }
                }
            }
            AssetDatabase.Refresh();
        }


        public static void DeletePrefabWithID(int id)
        {
            ItemPrefab temp = new ItemPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                if (item.GetComponent<AtavismInventoryItem>().TemplateId == id)
                    AssetDatabase.DeleteAsset(prefabPath);
            }
            AssetDatabase.Refresh();
        }

        public static void DeletePrefabWithIDAndDifferingName(int id, string name)
        {
            ItemPrefab temp = new ItemPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                if (item.GetComponent<AtavismInventoryItem>() != null && item.GetComponent<AtavismInventoryItem>().TemplateId == id && item.GetComponent<AtavismInventoryItem>().name != name)
                    AssetDatabase.DeleteAsset(prefabPath);
            }
            AssetDatabase.Refresh();
        }
        public static void AssignItemSet(int id, int setId)
        {
            ItemPrefab temp = new ItemPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                if (item.GetComponent<AtavismInventoryItem>() != null && item.GetComponent<AtavismInventoryItem>().TemplateId == id)
                {
                    item.GetComponent<AtavismInventoryItem>().SetId = setId;
                    EditorUtility.SetDirty(item);
                    AssetDatabase.SaveAssets();
                }
            }

            AssetDatabase.Refresh();
        }
        public static void UnassignItemSet(int id, int setId)
        {
            ItemPrefab temp = new ItemPrefab();
            string[] prefabPaths = Directory.GetFiles(temp.basePath, "*.prefab", SearchOption.AllDirectories);
            foreach (string prefabPath in prefabPaths)
            {
                GameObject item = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                if (item.GetComponent<AtavismInventoryItem>() != null && item.GetComponent<AtavismInventoryItem>().TemplateId == id)
                {
                    if (item.GetComponent<AtavismInventoryItem>().SetId == setId)
                        item.GetComponent<AtavismInventoryItem>().SetId = 0;
                    EditorUtility.SetDirty(item);
                    AssetDatabase.SaveAssets();
               }
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