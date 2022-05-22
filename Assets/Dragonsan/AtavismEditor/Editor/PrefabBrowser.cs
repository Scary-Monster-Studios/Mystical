using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Atavism
{
    public class PrefabBrowser : EditorWindow
    {
        [MenuItem("Window/Atavism/Atavism Prefab Browser")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(PrefabBrowser));
        }
        int size = 64;
        bool item = false;
        bool itemSet = false;
        bool skills = false;
        bool abilities = false;
        bool effects = false;
        bool craftRecipes = false;
        bool currencies = false;
        bool buildObjects = false;
        bool resourceNode = false;
        bool globalEvents = false;
        bool reces = false;
        Vector2 scrollPos;
        Vector2 scrollPos2;
        Vector2 scrollPos3;
        Vector2 scrollPos4;
        Vector2 scrollPos5;
        Vector2 scrollPos6;
        Vector2 scrollPos7;
        Vector2 scrollPos8;
        Vector2 scrollPos9;
        Vector2 scrollPos10;
        private Vector2 scrollPos11;
        string itemSearche = "";
        string itemSetSearche = "";
        string skillSearche = "";
        string abilitiesSearche = "";
        string effectsSearche = "";
        string currencySearche = "";
        string craftingSearche = "";
        string buildObjSearche = "";

        int limit = 100;
        public void OnGUI()
        {
            EditorGUILayout.LabelField("Select Only In Run");
            EditorGUILayout.Space();
            limit = EditorGUILayout.IntField("Display Limit", limit);
            EditorGUILayout.Space();
            currencies = EditorGUILayout.Toggle("Show Currencies", currencies);
            item = EditorGUILayout.Toggle("Show Items", item);
            itemSet = EditorGUILayout.Toggle("Show Item Sets", itemSet);
            craftRecipes = EditorGUILayout.Toggle("Show Crafting Recipes", craftRecipes);
            skills = EditorGUILayout.Toggle("Show Skills", skills);
            abilities = EditorGUILayout.Toggle("Show Abilities", abilities);
            effects = EditorGUILayout.Toggle("Show Effects", effects);
            buildObjects = EditorGUILayout.Toggle("Show Build Objects", buildObjects);
            resourceNode = EditorGUILayout.Toggle("Show ResouceNodes Profiles", resourceNode);
            globalEvents = EditorGUILayout.Toggle("Show Global Events", globalEvents);
            reces = EditorGUILayout.Toggle("Show Races", reces);
            EditorGUILayout.Space();
            if (!Application.isPlaying)
                return;
            if (currencies)
            {
                showCurrencies();
            }
            if (item)
            {
                showItems();
            }
            if (craftRecipes)
            {
                showCraftRecipes();
            }
            if (itemSet)
            {
                showItemSets();
            }
            if (skills)
            {
                showSkills();
            }
            if (abilities)
            {
                showAbilities();
            }
            if (effects)
            {
                showEffects();
            }
            if (buildObjects)
            {
                showBuildObjects();
            }

            if (resourceNode)
            {
                showResourceNodeProfiles();
            }
            
            if (globalEvents)
            {
                showGlobalEvents();
            }  
            if (reces)
            {
                showRaces();
            }
        }

        void showCurrencies()
        {
            EditorGUILayout.LabelField("List of Currencies " + (AtavismPrefabManager.Instance != null && AtavismPrefabManager.Instance.GetCurrencyPrefabData() != null ? AtavismPrefabManager.Instance.GetCurrencyPrefabData().Count : 0));
            currencySearche = EditorGUILayout.TextField("Search", currencySearche);
            EditorGUILayout.Space();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
            if (AtavismPrefabManager.Instance != null)
            {
                int i = 0;
                foreach (CurrencyPrefabData obj in AtavismPrefabManager.Instance.GetCurrencyPrefabData())
                {
                    if (obj.name.ToLower().Contains(currencySearche.ToLower()) || currencySearche.Equals(""))
                        if (i < limit || limit == -1)
                        {
                            EditorGUILayout.IntField("id", obj.id);
                            EditorGUILayout.TextField("name", obj.name);
                            //  EditorGUILayout.TextField("icon name", obj.iconData);
                            if (obj.icon == null)
                            {
                                if (obj.iconData.Length > 0)
                                {
                                    Texture2D tex = new Texture2D(2, 2);
                                    bool wyn = tex.LoadImage(System.Convert.FromBase64String(obj.iconData));
                                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                                    obj.icon = sprite;
                                }
                            }
                            if (obj.icon != null)
                                EditorGUILayout.TextField("Icon size", obj.icon.texture.width + "x" + obj.icon.texture.height);
                            EditorGUILayout.ObjectField("Icon", obj.icon, typeof(Sprite));

                            EditorGUILayout.Space();
                            i++;
                        }
                        else
                        {
                            EditorGUILayout.LabelField("The result display limit has been reached");
                            break;
                        }
                }
                if (i == 0)
                    EditorGUILayout.LabelField("There are no result to display");
            }
            else
            {
                EditorGUILayout.LabelField("Enter play mode to load data ");
            }
            EditorGUILayout.EndScrollView();
        }

        void showItems()
        {
            EditorGUILayout.LabelField("List of Items " + (AtavismPrefabManager.Instance != null && AtavismPrefabManager.Instance.GetItemPrefabData() != null ? AtavismPrefabManager.Instance.GetItemPrefabData().Count : 0));
            itemSearche = EditorGUILayout.TextField("Search", itemSearche);
            EditorGUILayout.Space();
            scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2, GUILayout.ExpandHeight(true));
            if (AtavismPrefabManager.Instance != null)
            {
                int i = 0;
                foreach (ItemPrefabData obj in AtavismPrefabManager.Instance.GetItemPrefabData())
                {

                    if (obj.name.ToLower().Contains(itemSearche.ToLower()) || itemSearche.Equals(""))
                        if (i < limit || limit == -1)
                        {

                            EditorGUILayout.IntField("id", obj.templateId);
                            EditorGUILayout.TextField("name", obj.name);
                            //  EditorGUILayout.TextField("icon name", obj.iconData);
                            if (obj.icon == null)
                            {
                                if (obj.iconData.Length > 0)
                                {
                                    Texture2D tex = new Texture2D(2, 2);
                                    bool wyn = tex.LoadImage(System.Convert.FromBase64String(obj.iconData));
                                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                                    obj.icon = sprite;
                                }
                            }
                            if (obj.icon != null)
                                EditorGUILayout.TextField("Icon size", obj.icon.texture.width + "x" + obj.icon.texture.height);
                            EditorGUILayout.ObjectField("Icon", obj.icon, typeof(Sprite));

                            EditorGUILayout.IntField("id", obj.setId);
                            EditorGUILayout.IntField("Durability", obj.durability);
                            EditorGUILayout.Toggle("Repairable", obj.repairable);

                            /*    EditorGUILayout.TextField("Item Type", obj.itemType);
                                EditorGUILayout.TextField("Tooltip", obj.tooltip);
                                EditorGUILayout.TextField("Sub Type", obj.subType);
                                EditorGUILayout.TextField("Slot", obj.slot);
                                EditorGUILayout.IntField("Quality", obj.quality);
                                EditorGUILayout.IntField("CurrencyType", obj.currencyType);
                                EditorGUILayout.LongField("Cost", obj.cost);
                                EditorGUILayout.IntField("Binding", obj.binding);
                                EditorGUILayout.Toggle("Sellable", obj.sellable);
                                EditorGUILayout.IntField("Damage Value", obj.damageValue);
                                EditorGUILayout.IntField("Damage Max Value", obj.damageMaxValue);
                                EditorGUILayout.IntField("SetId", obj.setId);
                                EditorGUILayout.IntField("Enchant Id", obj.enchantId);
                                EditorGUILayout.IntField("Weapon Speed", obj.weaponSpeed);
                                EditorGUILayout.IntField("Stack Limit", obj.stackLimit);
                                EditorGUILayout.Toggle("Auction House", obj.auctionHouse);
                                EditorGUILayout.Toggle("Unique", obj.unique);
                                EditorGUILayout.IntField("Gear Score", obj.gear_score);

                                EditorGUILayout.TextField("Socket Type", obj.sockettype);
                                EditorGUILayout.IntField("Durability", obj.durability);
                                EditorGUILayout.IntField("Weight", obj.weight);
                                EditorGUILayout.IntField("Auto Attack", obj.autoattack);
                                EditorGUILayout.IntField("Ammo Type", obj.ammoType);
                                EditorGUILayout.IntField("Death Loss", obj.deathLoss);
                                EditorGUILayout.Toggle("Parry", obj.parry);*/
                            EditorGUILayout.Space();
                            i++;
                        }
                        else
                        {
                            EditorGUILayout.LabelField("The result display limit has been reached");
                            break;
                        }
                }
                if (i == 0)
                    EditorGUILayout.LabelField("There are no result to display");
            }
            else
            {
                EditorGUILayout.LabelField("Enter play mode to load data ");
            }
            EditorGUILayout.EndScrollView();
        }

        void showCraftRecipes()
        {
            EditorGUILayout.LabelField("List of Crafting Recipes " + (AtavismPrefabManager.Instance != null && AtavismPrefabManager.Instance.GetCraftingRecipesPrefabData() != null ? AtavismPrefabManager.Instance.GetCraftingRecipesPrefabData().Count : 0));
            craftingSearche = EditorGUILayout.TextField("Search", craftingSearche);
            EditorGUILayout.Space();
            scrollPos3 = EditorGUILayout.BeginScrollView(scrollPos3, GUILayout.ExpandHeight(true));
            if (AtavismPrefabManager.Instance != null)
            {
                int i = 0;

                foreach (CraftingRecipePrefabData obj in AtavismPrefabManager.Instance.GetCraftingRecipesPrefabData())
                {
                    if (obj.recipeName.ToLower().Contains(craftingSearche.ToLower()) || craftingSearche.Equals(""))
                        if (i < limit || limit == -1)
                        {
                            EditorGUILayout.IntField("Id", obj.recipeID);
                            EditorGUILayout.TextField("Name", obj.recipeName);
                            EditorGUILayout.TextField("Required station", obj.stationReq);
                            EditorGUILayout.IntField("Skill Id", obj.skillID);
                            EditorGUILayout.IntField("Skill Level", obj.skillLevelReq);
                            EditorGUILayout.LongField("date", obj.date);

                            //  EditorGUILayout.TextField("icon name", obj.iconData);
                            if (obj.icon == null)
                            {
                                if (obj.iconData.Length > 0)
                                {
                                    Texture2D tex = new Texture2D(2, 2);
                                    bool wyn = tex.LoadImage(System.Convert.FromBase64String(obj.iconData));
                                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                                    obj.icon = sprite;
                                }
                            }
                            if (obj.icon != null)
                                EditorGUILayout.TextField("Icon size", obj.icon.texture.width + "x" + obj.icon.texture.height);
                            EditorGUILayout.ObjectField("Icon", obj.icon, typeof(Sprite));
                            EditorGUILayout.Space();
                            i++;
                        }
                        else
                        {
                            EditorGUILayout.LabelField("The result display limit has been reached");
                            break;
                        }
                }
                if (i == 0)
                    EditorGUILayout.LabelField("There are no result to display");
            }
            else
            {
                EditorGUILayout.LabelField("Enter play mode to load data ");
            }
            EditorGUILayout.EndScrollView();
        }

        void showItemSets()
        {
            EditorGUILayout.LabelField("List of Item Sets " + (AtavismPrefabManager.Instance != null && AtavismPrefabManager.Instance.GetItemSetPrefabData() != null ? AtavismPrefabManager.Instance.GetItemSetPrefabData().Count : 0));
            itemSetSearche = EditorGUILayout.TextField("Search", itemSetSearche);
            EditorGUILayout.Space();
            scrollPos4 = EditorGUILayout.BeginScrollView(scrollPos4, GUILayout.ExpandHeight(true));
            if (AtavismPrefabManager.Instance != null)
            {
                int i = 0;

                foreach (ItemSetPrefabData obj in AtavismPrefabManager.Instance.GetItemSetPrefabData())
                {
                    if (obj.Name.ToLower().Contains(itemSetSearche.ToLower()) || itemSetSearche.Equals(""))
                        if (i < limit || limit == -1)
                        {
                            EditorGUILayout.IntField("id", obj.Setid);
                            EditorGUILayout.TextField("name", obj.Name);
                            //  EditorGUILayout.TextField("icon name", obj.iconData);
                            //   EditorGUILayout.ObjectField("Icon", obj.icon, typeof(Sprite));
                            EditorGUILayout.Space();
                            i++;
                        }
                        else
                        {
                            EditorGUILayout.LabelField("The result display limit has been reached");
                            break;
                        }

                }
                if (i == 0)
                    EditorGUILayout.LabelField("There are no result to display");
            }
            else
            {
                EditorGUILayout.LabelField("Enter play mode to load data ");
            }
            EditorGUILayout.EndScrollView();
        }

        void showSkills()
        {
            EditorGUILayout.LabelField("List of Skills " + (AtavismPrefabManager.Instance != null && AtavismPrefabManager.Instance.GetSkillPrefabData() != null ? AtavismPrefabManager.Instance.GetSkillPrefabData().Count : 0));
            skillSearche = EditorGUILayout.TextField("Search", skillSearche);
            EditorGUILayout.Space();
            scrollPos5 = EditorGUILayout.BeginScrollView(scrollPos5, GUILayout.ExpandHeight(true));
            if (AtavismPrefabManager.Instance != null)
            {
                int i = 0;

                foreach (SkillPrefabData obj in AtavismPrefabManager.Instance.GetSkillPrefabData())
                {
                    if (obj.skillname.ToLower().Contains(skillSearche.ToLower()) || skillSearche.Equals(""))
                        if (i < limit || limit == -1)
                        {
                            EditorGUILayout.IntField("id", obj.id);
                            EditorGUILayout.TextField("name", obj.skillname);
                            EditorGUILayout.LongField("date", obj.date);
                            //  EditorGUILayout.TextField("icon name", obj.iconData);
                            if (obj.icon == null)
                            {
                                if (obj.iconData.Length > 0)
                                {
                                    Texture2D tex = new Texture2D(2, 2);
                                    bool wyn = tex.LoadImage(System.Convert.FromBase64String(obj.iconData));
                                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                                    obj.icon = sprite;
                                }
                            }
                            if (obj.icon != null)
                                EditorGUILayout.TextField("Icon size", obj.icon.texture.width + "x" + obj.icon.texture.height);
                            EditorGUILayout.ObjectField("Icon", obj.icon, typeof(Sprite));

                            foreach (var abi in obj.abilities)
                            {
                                EditorGUILayout.IntField("Ability ", abi);
                            }
                            
                            EditorGUILayout.Space(5);
                            i++;
                        }
                        else
                        {
                            EditorGUILayout.LabelField("The result display limit has been reached");
                            break;
                        }

                }
                if (i == 0)
                    EditorGUILayout.LabelField("There are no result to display");
            }
            else
            {
                EditorGUILayout.LabelField("Enter play mode to load data ");
            }
            EditorGUILayout.EndScrollView();
        }

        void showAbilities()
        {
            EditorGUILayout.LabelField("List of Abilities " + (AtavismPrefabManager.Instance != null && AtavismPrefabManager.Instance.GetAbilityPrefabData() != null ? AtavismPrefabManager.Instance.GetAbilityPrefabData().Count : 0));
            abilitiesSearche = EditorGUILayout.TextField("Search", abilitiesSearche);
            EditorGUILayout.Space();
            scrollPos6 = EditorGUILayout.BeginScrollView(scrollPos6, GUILayout.ExpandHeight(true));
            if (AtavismPrefabManager.Instance != null)
            {
                int i = 0;

                foreach (AbilityPrefabData obj in AtavismPrefabManager.Instance.GetAbilityPrefabData())
                {
                    if (obj.name.ToLower().Contains(abilitiesSearche.ToLower()) || abilitiesSearche.Equals(""))
                        if (i < limit || limit == -1)
                        {
                            EditorGUILayout.IntField("id", obj.id);
                            EditorGUILayout.TextField("name", obj.name);
                            // EditorGUILayout.TextField("icon name", obj.iconData);
                            if (obj.icon == null)
                            {
                                if (obj.iconData.Length > 0)
                                {
                                    Texture2D tex = new Texture2D(2, 2);
                                    bool wyn = tex.LoadImage(System.Convert.FromBase64String(obj.iconData));
                                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                                    obj.icon = sprite;
                                }
                            }
                            if (obj.icon != null)
                                EditorGUILayout.TextField("Icon size", obj.icon.texture.width + "x" + obj.icon.texture.height);
                            EditorGUILayout.ObjectField("Icon", obj.icon, typeof(Sprite));
                            EditorGUILayout.Space();
                            i++;
                        }
                        else
                        {
                            EditorGUILayout.LabelField("The result display limit has been reached");
                            break;
                        }

                }
                if (i == 0)
                    EditorGUILayout.LabelField("There are no result to display");
            }
            else
            {
                EditorGUILayout.LabelField("Enter play mode to load data ");
            }
            EditorGUILayout.EndScrollView();
        }

        void showEffects()
        {
            EditorGUILayout.LabelField("List of Effects " + (AtavismPrefabManager.Instance != null && AtavismPrefabManager.Instance.GetEffectPrefabData() != null ? AtavismPrefabManager.Instance.GetEffectPrefabData().Count : 0));
            effectsSearche = EditorGUILayout.TextField("Search", effectsSearche);
            EditorGUILayout.Space();
            scrollPos7 = EditorGUILayout.BeginScrollView(scrollPos7, GUILayout.ExpandHeight(true));
            if (AtavismPrefabManager.Instance != null)
            {
                int i = 0;

                foreach (EffectsPrefabData obj in AtavismPrefabManager.Instance.GetEffectPrefabData())
                {
                    if (obj.name.ToLower().Contains(effectsSearche.ToLower()) || effectsSearche.Equals(""))
                        if (i < limit || limit == -1)
                        {
                            EditorGUILayout.IntField("id", obj.id);
                            EditorGUILayout.TextField("name", obj.name);
                            // EditorGUILayout.TextField("icon name", obj.iconData);
                            if (obj.icon == null)
                            {
                                if (obj.iconData.Length > 0)
                                {
                                    Texture2D tex = new Texture2D(2, 2);
                                    bool wyn = tex.LoadImage(System.Convert.FromBase64String(obj.iconData));
                                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                                    obj.icon = sprite;
                                }
                            }
                            if (obj.icon != null)
                                EditorGUILayout.TextField("Icon size", obj.icon.texture.width + "x" + obj.icon.texture.height);
                            EditorGUILayout.ObjectField("Icon", obj.icon, typeof(Sprite));
                            EditorGUILayout.Space();
                            i++;
                        }
                        else
                        {
                            EditorGUILayout.LabelField("The result display limit has been reached");
                            break;
                        }

                }
                if (i == 0)
                    EditorGUILayout.LabelField("There are no result to display");
            }
            else
            {
                EditorGUILayout.LabelField("Enter play mode to load data ");
            }
            EditorGUILayout.EndScrollView();
        }

        void showBuildObjects()
        {
            EditorGUILayout.LabelField("List of Build Objects " + (AtavismPrefabManager.Instance.GetBuildObjPrefabData() != null ? AtavismPrefabManager.Instance.GetBuildObjPrefabData().Count : 0));
            buildObjSearche = EditorGUILayout.TextField("Search", buildObjSearche);
            EditorGUILayout.Space();
            scrollPos8 = EditorGUILayout.BeginScrollView(scrollPos8, GUILayout.ExpandHeight(true));
            if (AtavismPrefabManager.Instance != null)
            {
                int i = 0;

                foreach (BuildObjPrefabData obj in AtavismPrefabManager.Instance.GetBuildObjPrefabData())
                {
                    if (obj.buildObjectName.ToLower().Contains(buildObjSearche.ToLower()) || buildObjSearche.Equals(""))
                        if (i < limit || limit == -1)
                        {
                            EditorGUILayout.BeginVertical();
                            EditorGUILayout.IntField("id", obj.id);
                            EditorGUILayout.TextField("name", obj.buildObjectName);
                            EditorGUILayout.TextField("prefab", obj.gameObject);
                            // EditorGUILayout.TextField("icon name", obj.iconData);
                            if (obj.icon == null)
                            {
                                if (obj.iconData.Length > 0)
                                {
                                    Texture2D tex = new Texture2D(2, 2);
                                    bool wyn = tex.LoadImage(System.Convert.FromBase64String(obj.iconData));
                                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                                    obj.icon = sprite;
                                }
                            }
                            if (obj.icon != null)
                                EditorGUILayout.TextField("Icon size", obj.icon.texture.width + "x" + obj.icon.texture.height);
                            EditorGUILayout.ObjectField("Icon", obj.icon, typeof(Sprite));
                            EditorGUILayout.IntField("Category", obj.category);
                            EditorGUILayout.LongField("Date", obj.date);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.Space();
                            i++;
                        }
                        else
                        {
                            EditorGUILayout.LabelField("The result display limit has been reached");
                            break;
                        }

                }
                if (i == 0)
                    EditorGUILayout.LabelField("There are no result to display");
            }
            else
            {
                EditorGUILayout.LabelField("Enter play mode to load data ");
            }
            EditorGUILayout.EndScrollView();
        }
          void showResourceNodeProfiles()
        {
            EditorGUILayout.LabelField("List of Global Events " + (AtavismPrefabManager.Instance.GetResourceNodePrefabData() != null ? AtavismPrefabManager.Instance.GetResourceNodePrefabData().Count : 0));
          //  buildObjSearche = EditorGUILayout.TextField("Search", buildObjSearche);
            EditorGUILayout.Space();
            scrollPos9 = EditorGUILayout.BeginScrollView(scrollPos9, GUILayout.ExpandHeight(true));
            if (AtavismPrefabManager.Instance != null)
            {
                int i = 0;
                EditorGUILayout.BeginVertical();
                foreach (RecourceNodeProfileData obj in AtavismPrefabManager.Instance.GetResourceNodePrefabData())
                {
                  //  if (obj.buildObjectName.ToLower().Contains(buildObjSearche.ToLower()) || buildObjSearche.Equals(""))
                        if (i < limit || limit == -1)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.BeginVertical();
                            EditorGUILayout.IntField("profile id", obj.id);
                            EditorGUILayout.LongField("date", obj.date);
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.BeginVertical();
                            for (int s = 0; s < obj.settingList.Count; s++ )
                            {
                               
                                var setting = obj.settingList[s];
                                EditorGUILayout.Space();
                                EditorGUILayout.IntField("Settings Id", setting.id);
                                if (setting.cursorIcon == null)
                                {
                                    if (setting.cursorIconData.Length > 0)
                                    {
                                        Texture2D tex = new Texture2D(2, 2);
                                        bool wyn = tex.LoadImage(System.Convert.FromBase64String(setting.cursorIconData));
                                       // Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                                        setting.cursorIcon = tex;
                                    }
                                }
                                if (setting.cursorIcon != null)
                                    EditorGUILayout.TextField("Cursor Icon size", setting.cursorIcon.width + "x" + setting.cursorIcon.height);
                                EditorGUILayout.ObjectField("Cursor Icon", setting.cursorIcon, typeof(Texture2D));
                                if (setting.selectedIcon == null)
                                {
                                    if (setting.selectedIconData.Length > 0)
                                    {
                                        Texture2D tex = new Texture2D(2, 2);
                                        bool wyn = tex.LoadImage(System.Convert.FromBase64String(setting.selectedIconData));
                                        Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                                        setting.selectedIcon = sprite;
                                    }
                                }
                                if (setting.selectedIcon != null)
                                    EditorGUILayout.TextField("Selected Icon size", setting.selectedIcon.texture.width + "x" + setting.selectedIcon.texture.height);
                                EditorGUILayout.ObjectField("Selected Icon", setting.selectedIcon, typeof(Sprite));
                               
                            }
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.Space();

                            EditorGUILayout.Space();
                            EditorGUILayout.EndHorizontal();
                            i++;
                        }
                        else
                        {
                            EditorGUILayout.LabelField("The result display limit has been reached");
                            break;
                        }

                }
                if (i == 0)
                    EditorGUILayout.LabelField("There are no result to display");
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.LabelField("Enter play mode to load data ");
            }
            EditorGUILayout.EndScrollView();
        }
          
        void showGlobalEvents()
        {
            EditorGUILayout.LabelField("List of Global Events " + (AtavismPrefabManager.Instance.GetGlobalEventPrefabData() != null ? AtavismPrefabManager.Instance.GetGlobalEventPrefabData().Count : 0));
          //  buildObjSearche = EditorGUILayout.TextField("Search", buildObjSearche);
            EditorGUILayout.Space();
            scrollPos10 = EditorGUILayout.BeginScrollView(scrollPos10, GUILayout.ExpandHeight(true));
            if (AtavismPrefabManager.Instance != null)
            {
                int i = 0;

                foreach (GlobalEventData obj in AtavismPrefabManager.Instance.GetGlobalEventPrefabData())
                {
                  //  if (obj.buildObjectName.ToLower().Contains(buildObjSearche.ToLower()) || buildObjSearche.Equals(""))
                        if (i < limit || limit == -1)
                        {
                            EditorGUILayout.IntField("id", obj.id);
                          //  EditorGUILayout.TextField("name", obj.buildObjectName);
                          //  EditorGUILayout.TextField("prefab", obj.gameObject);
                            // EditorGUILayout.TextField("icon name", obj.iconData);
                            if (obj.icon == null)
                            {
                                if (obj.iconData.Length > 0)
                                {
                                    Texture2D tex = new Texture2D(2, 2);
                                    bool wyn = tex.LoadImage(System.Convert.FromBase64String(obj.iconData));
                                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                                    obj.icon = sprite;
                                }
                            }
                            if (obj.icon != null)
                                EditorGUILayout.TextField("Icon size", obj.icon.texture.width + "x" + obj.icon.texture.height);
                            EditorGUILayout.ObjectField("Icon", obj.icon, typeof(Sprite));
                            EditorGUILayout.LongField("Date", obj.date);

                            EditorGUILayout.Space();
                            i++;
                        }
                        else
                        {
                            EditorGUILayout.LabelField("The result display limit has been reached");
                            break;
                        }

                }
                if (i == 0)
                    EditorGUILayout.LabelField("There are no result to display");
            }
            else
            {
                EditorGUILayout.LabelField("Enter play mode to load data ");
            }
            EditorGUILayout.EndScrollView();
        }
        
          void showRaces()
        {
            EditorGUILayout.LabelField("List of Racess " + (AtavismPrefabManager.Instance.GetRacesPrefabData() != null ? AtavismPrefabManager.Instance.GetRacesPrefabData().Count : 0));
          //  buildObjSearche = EditorGUILayout.TextField("Search", buildObjSearche);
            EditorGUILayout.Space();
            scrollPos11 = EditorGUILayout.BeginScrollView(scrollPos11, GUILayout.ExpandHeight(true));
            if (AtavismPrefabManager.Instance != null)
            {
                int i = 0;
                EditorGUILayout.BeginVertical();
                foreach (RaceData obj in AtavismPrefabManager.Instance.GetRacesPrefabData())
                {
                  //  if (obj.buildObjectName.ToLower().Contains(buildObjSearche.ToLower()) || buildObjSearche.Equals(""))
                        if (i < limit || limit == -1)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.BeginVertical();
                            EditorGUILayout.IntField("Race id", obj.id);
                            EditorGUILayout.TextField("Race Name", obj.name);
                            EditorGUILayout.TextField("Race Desc", obj.description);
                           // EditorGUILayout.LongField("date", obj.date);
                            if (obj.icon == null)
                            {
                                if (obj.iconData.Length > 0)
                                {
                                    Texture2D tex = new Texture2D(2, 2);
                                    bool wyn = tex.LoadImage(System.Convert.FromBase64String(obj.iconData));
                                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                                    obj.icon = sprite;
                                }
                            }
                            if (obj.icon != null)
                                EditorGUILayout.TextField("Race Icon size", obj.icon.texture.width + "x" + obj.icon.texture.height);
                            EditorGUILayout.ObjectField("Race Icon", obj.icon, typeof(Texture2D));
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.BeginVertical();
                            foreach (var cl in obj.classList.Values )
                            {
                                EditorGUILayout.Space();
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.BeginVertical();
                                EditorGUILayout.IntField("Class Id", cl.id);
                                EditorGUILayout.TextField("Class Name", cl.name);
                                EditorGUILayout.TextField("Class Desc", cl.description);

                                if (cl.icon == null)
                                {
                                    if (cl.iconData.Length > 0)
                                    {
                                        Texture2D tex = new Texture2D(2, 2);
                                        bool wyn = tex.LoadImage(System.Convert.FromBase64String(cl.iconData));
                                        Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                                       cl.icon = sprite;
                                    }
                                }
                                if (cl.icon != null)
                                    EditorGUILayout.TextField("Class Icon size", cl.icon.texture.width + "x" + cl.icon.texture.height);
                                EditorGUILayout.ObjectField("Class Icon", cl.icon, typeof(Texture2D));
                                EditorGUILayout.EndVertical();
                                EditorGUILayout.BeginVertical();
                                foreach (var gl in cl.genderList.Values)
                                {
                                    EditorGUILayout.BeginVertical();
                                    EditorGUILayout.IntField("Gender Id", gl.id);
                                    EditorGUILayout.TextField("Gender Name", gl.name);
                                    EditorGUILayout.TextField("Gender Prefab", gl.prefab);

                                    if (gl.icon == null)
                                    {
                                        if (gl.iconData.Length > 0)
                                        {
                                            Texture2D tex = new Texture2D(2, 2);
                                            bool wyn = tex.LoadImage(System.Convert.FromBase64String(gl.iconData));
                                            Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                                            gl.icon = sprite;
                                        }
                                    }
                                    if (gl.icon != null)
                                        EditorGUILayout.TextField("Gender Icon size", gl.icon.texture.width + "x" + cl.icon.texture.height);
                                    EditorGUILayout.ObjectField("Gender Icon", gl.icon, typeof(Texture2D));
                                    EditorGUILayout.EndVertical();
                                }
                                EditorGUILayout.EndVertical();



                                EditorGUILayout.EndHorizontal();
                               
                            }
                            EditorGUILayout.EndVertical();
                          
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space();
                            EditorGUILayout.Space();
                            i++;
                        }
                        else
                        {
                            EditorGUILayout.LabelField("The result display limit has been reached");
                            break;
                        }

                }
                if (i == 0)
                    EditorGUILayout.LabelField("There are no result to display");
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.LabelField("Enter play mode to load data ");
            }
            EditorGUILayout.EndScrollView();
        }
    }
}