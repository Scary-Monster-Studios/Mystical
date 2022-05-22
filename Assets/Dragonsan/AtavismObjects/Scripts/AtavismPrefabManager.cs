using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Atavism
{
    [Serializable]
    public class GlobalEventData
    {
        public int id = -1;
        [NonSerialized] public  Sprite icon;
        public string iconData = "";
        [NonSerialized] public bool iconloaded = false;
        public long date = 0L;
    }
    
    [Serializable]
    public class RecourceNodeProfileSettingData
    {
        public int id = -1;
        [NonSerialized] public Texture2D cursorIcon;
        public string cursorIconData = "";
        [NonSerialized] public  Sprite selectedIcon;
        public string selectedIconData = "";
        [NonSerialized] public bool iconloaded = false;
        public long date = 0L;
    }
    
    [Serializable]
    public class RecourceNodeProfileData
    {
        public int id = -1;
        public long date = 0L;
        public Dictionary<int,RecourceNodeProfileSettingData> settingList = new Dictionary<int,RecourceNodeProfileSettingData>();
    }

  
    [Serializable]
    public class RaceData
    {
        public int id = -1;
        public string name = "";
        public string description = "";
        [NonSerialized] public Sprite icon;
        [NonSerialized] public bool iconloaded = false;
        public string iconData = "";
        public Dictionary<int,ClassData> classList = new Dictionary<int,ClassData>();
        public long date = 0L;
    }
    
    
    
    
    [Serializable]
    public class ClassData
    {
        public int id = -1;
        public string name = "";
        public string description = "";
        [NonSerialized] public Sprite icon;
        public string iconData = "";
        [NonSerialized] public bool iconloaded = false;
        public  Dictionary<int,GenderData> genderList = new Dictionary<int,GenderData>();
    }
    
    [Serializable]
    public class GenderData
    {
        public int id = -1;
        public string name = "";
        public string prefab = "";
        [NonSerialized] public Sprite icon;
        public string iconData = "";
        [NonSerialized] public bool iconloaded = false;
    }

    [Serializable]
    public class ItemPrefabData
    {
        public int templateId = -1;
        public string name;
        [NonSerialized] public Sprite icon;
        [NonSerialized] public bool iconloaded = false;
        public string iconData = "";
        public string tooltip;
        public string itemType = "";
        public string subType = "";
        public string slot = "";
        public int quality = 0;
        public int currencyType = 0;
        public long cost = 0;
        public int binding = 0;
        public bool sellable = true;
        public int damageValue = 0;
        public int damageMaxValue = 0;
        public int setId = 0;
        public int enchantId = 0;
        public int weaponSpeed = 2000;
        public int stackLimit = 1;
        public bool auctionHouse = false;
        public bool unique = false;
        public int gear_score = 0;

        public string sockettype = "";
        public int durability = 0;
        public int weight = 0;
        public int autoattack = -1;
        public int ammoType = -1;
        public int deathLoss = 0;
        public bool parry = false;
        public bool repairable = true;
        public List<string> itemEffectTypes = new List<string>();
        public List<string> itemEffectNames = new List<string>();
        public List<string> itemEffectValues = new List<string>();
        public List<string> itemReqTypes = new List<string>();
        public List<string> itemReqNames = new List<string>();
        public List<string> itemReqValues = new List<string>();
        public long date = 0L;
    }
    [Serializable]
    public class ItemSetPrefabData
    {
        public int Setid = 0;
        public string Name = "name";        // The enchant profile name
        public List<int> itemList = new List<int>();
        public List<AtavismInventoryItemSetLevel> levelList = new List<AtavismInventoryItemSetLevel>();
        public long date = 0L;
    }

    [Serializable]
    public class AbilityPrefabData
    {
        public int id = 0;
        public string name;
        [NonSerialized] public Sprite icon;
        public string iconData = "";
        [NonSerialized] public bool iconloaded = false;
        public string tooltip;
        //public string rank = "";
        public int cost = 0;
        public int pulseCost = 0;
        public float costPercentage = 0;
        public float pulseCostPercentage = 0;
        public string costProperty = "mana";
        public string pulseCostProperty = "mana";
        public bool globalcd = false;
        public bool weaponcd = false;
        public string cooldownType = "";
        public float cooldownLength = 0;
        public string weaponReq = "";
        public int reagentReq = -1;
        public int distance = 0;
        public bool castingInRun = false;
        public TargetType targetType;
        public float castTime = 0;
        public bool passive = false;
        public string aoeType = "";
        public int maxRange = 4;                // How far away the target can be (in metres)
        public int minRange = 0;                // How close the target can be (in metres)
        public int aoeRadius = 1;
        public string aoePrefab = "";
        //public int reqLevel = 1;
        public bool toggle = false;

        public long date = 0L;
    }

    [Serializable]
    public class EffectsPrefabData
    {
        public int id;
        public string name;
        [NonSerialized] public Sprite icon;
        public string iconData = "";
        [NonSerialized] public bool iconloaded = false;
        public string tooltip;
        public long date = 0L;
        
        public bool isBuff;
        public int stackLimit = 1;
        public bool stackTime = false;
        public bool allowMultiple = false;
     /*   float length;
        float expiration = -1;
        bool active = false;
        bool passive = false;
        */
    }

    [Serializable]
    public class CurrencyPrefabData
    {
        public int id = -1;
        public string name = "";
        public string description = "";
        [NonSerialized] public Sprite icon;
        public string iconData = "";
        [NonSerialized] public bool iconloaded = false;
        public int group = 1;
        public int position = 1;
        public int convertsTo = -1;
        public int conversionAmountReq = 1;
        public long max = 999999;
        public long date = 0L;
    }

    [Serializable]
    public class SkillPrefabData
    {
        public int id = 0;
        public string skillname = "";
        [NonSerialized] public Sprite icon;
        public string iconData = "";
        [NonSerialized] public bool iconloaded = false;
        public int mainAspect = -1;
        public int type = -1;
        public int oppositeAspect = -1;
        public bool mainAspectOnly = false;
        public int parentSkill = -1;
        public int parentSkillLevelReq = -1;
        public int playerLevelReq = 1;
        public int pcost = 0;
        public bool talent = false;
        public List<int> abilities = new List<int>();
        public List<int> abilityLevelReqs = new List<int>();
        public long date = 0L;
    }


    [Serializable]
    public class BuildObjPrefabData
    {
        public int id = 0;
        public string buildObjectName = "";
        [NonSerialized] public Sprite icon;
        public string iconData = "";
        [NonSerialized] public bool iconloaded = false;
        public int category = 0;
        public int skill = -1;
        public int skillLevelReq = 0;
        public float distanceReq = 1f;
        public float buildTimeReq = 1f;
        public bool buildTaskReqPlayer = true;
//        public ClaimType validClaimTypes = ClaimType.Any;
        public List<int> validClaimTypes = new List<int>();
        public bool onlyAvailableFromItem = false;
        public string reqWeapon = "";
        public string gameObject = "";

        public List<int> itemsReq = new List<int>();
        public List<int> itemsReqCount = new List<int>();

        public List<int> upgradeItemsReq = new List<int>();
        public long date = 0L;
     //   public Dictionary<int, int> itemReqs = new Dictionary<int, int>();
    }

    [Serializable]
    public class CraftingRecipePrefabData
    {
        public int recipeID;
        public string recipeName;
        [NonSerialized] public Sprite icon;
        public string iconData = "";
        [NonSerialized] public bool iconloaded = false;
        public int skillID = -1;
        public int skillLevelReq = -1;
        public string stationReq = "";
        public int creationTime = 0;

        public List<int> createsItems =new List<int>();
        public List<int> createsItemsCounts = new List<int>();
        public List<int> createsItems2 = new List<int>();
        public List<int> createsItemsCounts2 = new List<int>();
        public List<int> createsItems3 = new List<int>();
        public List<int> createsItemsCounts3 = new List<int>();
        public List<int> createsItems4 = new List<int>();
        public List<int> createsItemsCounts4 = new List<int>();
        public List<int> itemsReq = new List<int>();
        public List<int> itemsReqCounts = new List<int>();
        public long date = 0L;
    }



    [Serializable]
    public class AtavismPefabData
    {
        public Dictionary<int, ItemPrefabData> items = new Dictionary<int, ItemPrefabData>();
        public Dictionary<int, ItemSetPrefabData> itemSets = new Dictionary<int, ItemSetPrefabData>();
        public Dictionary<int, CraftingRecipePrefabData> craftRecipes = new Dictionary<int, CraftingRecipePrefabData>();
        public Dictionary<int, CurrencyPrefabData> currencies = new Dictionary<int, CurrencyPrefabData>();

        public Dictionary<int, BuildObjPrefabData> buildObjects = new Dictionary<int, BuildObjPrefabData>();

        public Dictionary<int, SkillPrefabData> skills = new Dictionary<int, SkillPrefabData>();
        public Dictionary<int, AbilityPrefabData> abilities = new Dictionary<int, AbilityPrefabData>();
        public Dictionary<int, EffectsPrefabData> effects = new Dictionary<int, EffectsPrefabData>();
        public Dictionary<int, RaceData> races = new Dictionary<int, RaceData>();

        public Dictionary<int, RecourceNodeProfileData> recourceNode = new Dictionary<int, RecourceNodeProfileData>();   
        public Dictionary<int, GlobalEventData> globalEvents = new Dictionary<int, GlobalEventData>();   

    }

    public class AtavismPrefabManager : MonoBehaviour
    {
        static AtavismPrefabManager instance;
        // Start is called before the first frame update

        [HideInInspector]  public AtavismPefabData prefabsdatadata = new AtavismPefabData();
        Dictionary<int, float> itemIconGet = new Dictionary<int, float>();
        Dictionary<int, float> skillIconGet = new Dictionary<int, float>();
        Dictionary<int, float> currencyIconGet = new Dictionary<int, float>();
        Dictionary<int, float> effectsIconGet = new Dictionary<int, float>();

        Dictionary<string, float> resourceIconGet = new Dictionary<string, float>();
        //  Dictionary<int, float> craftRecipesIconGet = new Dictionary<int, float>();
        //  Dictionary<int, float> buildObjectsIconGet = new Dictionary<int, float>();
        //  Dictionary<int, float> effectsIconGet = new Dictionary<int, float>();
        protected bool prefabReloading = false;
        protected int toReload = 11;
        [HideInInspector] public int reloaded = 0;
        [SerializeField] private bool loadOnStartup = false;
        public bool PrefabReloading
        {
            get
            {
                return prefabReloading;
            }
        } public int ToReload
        {
            get
            {
                return toReload;
            }
        }
        void Start()
        {

            if (instance != null)
            {
                return;
            }

            instance = this;
            SceneManager.sceneLoaded += sceneLoaded;
#if UNITY_ANDROID || UNITY_IOS
            if (!Directory.Exists(Application.persistentDataPath+"/" + Application.productName))
                Directory.CreateDirectory(Application.persistentDataPath+"/" + Application.productName);

            
            if (File.Exists( Application.persistentDataPath+"/" + Application.productName + "/game.data"))
            {
                FileStream file = File.Open( Application.persistentDataPath+"/" + Application.productName + "/game.data", FileMode.Open);
#else
            if (!Directory.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "/" + Application.productName))
                Directory.CreateDirectory(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "/" + Application.productName);
//Debug.LogError(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "/" + Application.productName);
            if (File.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "/" + Application.productName + "/game.data"))
            {
                FileStream file = File.Open(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "/" + Application.productName + "/game.data", FileMode.Open);
/*#else
            if (!Directory.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/" + Application.productName))
                Directory.CreateDirectory(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/" + Application.productName);

            if (File.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/" + Application.productName + "/game.data"))
            {
                FileStream file = File.Open(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/" + Application.productName + "/game.data", FileMode.Open);*/
#endif
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();

                    prefabsdatadata = (AtavismPefabData) bf.Deserialize(file);
                    if (prefabsdatadata.races == null)
                        prefabsdatadata.races = new Dictionary<int, RaceData>();

                    if (prefabsdatadata.recourceNode == null)
                        prefabsdatadata.recourceNode = new Dictionary<int, RecourceNodeProfileData>();
                    if (prefabsdatadata.globalEvents == null)
                        prefabsdatadata.globalEvents = new Dictionary<int, GlobalEventData>();

                    foreach (RaceData race in prefabsdatadata.races.Values)
                    {
                        if (AtavismLogger.logLevel <= LogLevel.Debug)
                            Debug.Log("Loading Race " + race.id);
                        if (race.iconData.Length > 0)
                        {
                            Texture2D tex = new Texture2D(2, 2);
                            bool wyn = tex.LoadImage(System.Convert.FromBase64String(race.iconData));
                            Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                            race.icon = sprite;
                        }

                        foreach (ClassData _class in race.classList.Values)
                        {
                            if (AtavismLogger.logLevel <= LogLevel.Debug)
                                Debug.Log("Loading Race " + _class.id);
                            if (_class.iconData.Length > 0)
                            {
                                Texture2D tex = new Texture2D(2, 2);
                                bool wyn = tex.LoadImage(System.Convert.FromBase64String(_class.iconData));
                                Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                                _class.icon = sprite;
                            }

                            foreach (GenderData gender in _class.genderList.Values)
                            {
                                if (AtavismLogger.logLevel <= LogLevel.Debug)
                                    Debug.Log("Loading Race " + gender.id);
                                if (gender.iconData.Length > 0)
                                {
                                    Texture2D tex = new Texture2D(2, 2);
                                    bool wyn = tex.LoadImage(System.Convert.FromBase64String(gender.iconData));
                                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                                    gender.icon = sprite;
                                }


                            }

                        }

                    }



                    foreach (BuildObjPrefabData item in prefabsdatadata.buildObjects.Values)
                    {
                        if (AtavismLogger.logLevel <= LogLevel.Debug)
                            Debug.Log("Loading Build Object " + item.buildObjectName);
                        if (item.iconData.Length > 0)
                        {
                            Texture2D tex = new Texture2D(2, 2);
                            bool wyn = tex.LoadImage(System.Convert.FromBase64String(item.iconData));
                            Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                            item.icon = sprite;
                        }

                    }

                    foreach (ItemPrefabData item in prefabsdatadata.items.Values)
                    {
                        if (AtavismLogger.logLevel <= LogLevel.Debug)
                            Debug.Log("Loading Item " + item.name);
                        if (item.iconData.Length > 0)
                        {
                            Texture2D tex = new Texture2D(2, 2);
                            bool wyn = tex.LoadImage(System.Convert.FromBase64String(item.iconData));
                            Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                            item.icon = sprite;
                        }
                    }

                    foreach (CraftingRecipePrefabData item in prefabsdatadata.craftRecipes.Values)
                    {
                        if (AtavismLogger.logLevel <= LogLevel.Debug)
                            Debug.Log("Loading crafting recipe  " + item.recipeName);
                        if (item.iconData.Length > 0)
                        {
                            Texture2D tex = new Texture2D(2, 2);
                            bool wyn = tex.LoadImage(System.Convert.FromBase64String(item.iconData));
                            Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                            item.icon = sprite;
                        }
                    }


                    foreach (CurrencyPrefabData item in prefabsdatadata.currencies.Values)
                    {
                        if (AtavismLogger.logLevel <= LogLevel.Debug)
                            Debug.Log("Loading currency " + item.name);
                        if (item.iconData.Length > 0)
                        {
                            Texture2D tex = new Texture2D(2, 2);
                            bool wyn = tex.LoadImage(System.Convert.FromBase64String(item.iconData));
                            Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                            item.icon = sprite;
                        }
                    }

                    foreach (AbilityPrefabData item in prefabsdatadata.abilities.Values)
                    {
                        if (AtavismLogger.logLevel <= LogLevel.Debug)
                            Debug.Log("Loading ability " + item.name);
                        if (item.iconData.Length > 0)
                        {
                            Texture2D tex = new Texture2D(2, 2);
                            bool wyn = tex.LoadImage(System.Convert.FromBase64String(item.iconData));
                            Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                            item.icon = sprite;
                        }
                    }

                    foreach (SkillPrefabData item in prefabsdatadata.skills.Values)
                    {
                        if (AtavismLogger.logLevel <= LogLevel.Debug)
                            Debug.Log("Loading Skill " + item.skillname);
                        if (item.iconData.Length > 0)
                        {
                            Texture2D tex = new Texture2D(2, 2);
                            bool wyn = tex.LoadImage(System.Convert.FromBase64String(item.iconData));
                            Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                            item.icon = sprite;
                        }
                    }

                    foreach (EffectsPrefabData item in prefabsdatadata.effects.Values)
                    {
                        if (AtavismLogger.logLevel <= LogLevel.Debug)
                            Debug.Log("Loading effect " + item.name);
                        if (item.iconData.Length > 0)
                        {
                            Texture2D tex = new Texture2D(2, 2);
                            bool wyn = tex.LoadImage(System.Convert.FromBase64String(item.iconData));
                            Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                            item.icon = sprite;
                        }
                    }

                    foreach (ItemSetPrefabData item in prefabsdatadata.itemSets.Values)
                    {
                        //  Debug.Log("Loading item set " + item.Name);

                    }

                    foreach (RecourceNodeProfileData node in prefabsdatadata.recourceNode.Values)
                    {
                        foreach (RecourceNodeProfileSettingData set in node.settingList.Values)
                        {
                            if (set.cursorIconData.Length > 0)
                            {
                                Texture2D tex = new Texture2D(2, 2);
                                bool wyn = tex.LoadImage(System.Convert.FromBase64String(set.cursorIconData));
                                //Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                                set.cursorIcon = tex;
                            }

                            if (set.selectedIconData.Length > 0)
                            {
                                Texture2D tex = new Texture2D(2, 2);
                                bool wyn = tex.LoadImage(System.Convert.FromBase64String(set.selectedIconData));
                                Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                                set.selectedIcon = sprite;
                            }
                        }
                    }

                    foreach (GlobalEventData gv in prefabsdatadata.globalEvents.Values)
                    {
                        if (AtavismLogger.logLevel <= LogLevel.Debug)
                            Debug.Log("Loading Global Event " + gv.id);
                        if (gv.iconData.Length > 0)
                        {
                            Texture2D tex = new Texture2D(2, 2);
                            bool wyn = tex.LoadImage(System.Convert.FromBase64String(gv.iconData));
                            Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                            gv.icon = sprite;
                        }
                    }

                }
                catch (SerializationException e)
                {
                    Debug.LogError("Failed to deserialize. Reason: " + e.Message + "\n\n" + e.StackTrace);
                    prefabsdatadata = new AtavismPefabData();
                    // throw;
                }
                finally
                {
                    file.Close();
                }

            }
            else
            {
                prefabsdatadata = new AtavismPefabData();
            }
            AtavismClient.Instance.NetworkHelper.RegisterPrefabMessageHandler("GetCountData", HandleGetCountData);

            AtavismEventSystem.RegisterEvent("ITEM_RELOAD", this);
            AtavismEventSystem.RegisterEvent("ITEM_ICON_UPDATE", this);
            AtavismEventSystem.RegisterEvent("CURRENCY_ICON_UPDATE", this);
            AtavismEventSystem.RegisterEvent("SKILL_ICON_UPDATE", this);
            AtavismEventSystem.RegisterEvent("EFFECT_ICON_UPDATE", this);
            if (loadOnStartup)
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                AtavismClient.Instance.NetworkHelper.GetPrefabs(props, "GetCountData");
                string[] arg = new string[1];
                AtavismEventSystem.DispatchEvent("LOADING_PREFAB_SHOW", arg);

            }
            LoadRaceData();
            LoadItemPrefabData();
            LoadItemSetPrefabData();

            LoadCurrencyPrefabData();
            LoadSkillsPrefabData();
            LoadAbilitiesPrefabData();
            LoadEffectsPrefabData();
            LoadResourceNodePrefabData();
            LoadBuldingObjectsPrefabData();
            LoadCraftingRecipePrefabData();

            LoadGlobaEventsData();
            NetworkAPI.RegisterExtensionMessageHandler("ao.reloaded", HandleReloaded);
            NetworkAPI.RegisterExtensionMessageHandler("combatSettings", HandleCombatSettings);

            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("LOAD_PREFAB", args);
            toReload = 11;
            
            
        }

        private int items, craft, curr, itset, skill, ability, effects, building, resource, race, global = 0;
        public void HandleGetCountData(Dictionary<string, object> props)
        {
             items = (int) props["Item"];
             craft = (int) props["CraftingRecipe"];
             curr = (int) props["Currency"];
             itset = (int) props["ItemSet"];
             skill = (int) props["Skill"];
             ability = (int) props["Ability"];
             effects = (int) props["Effect"];
             building = (int) props["BuildingObject"];
             resource = (int) props["ResourceNode"];
             race = (int) props["Race"];
             global = (int) props["GlobalEvents"];
            prefabCount = items + craft + curr + itset + skill + ability + effects + building + resource + race + global;
            iconsCount = items + craft + curr + skill + ability + effects + building + resource ;
            StartCoroutine(PrefabLoading());
        }
        private int prefabCount = 0;
        private int iconsCount = 0;

        private IEnumerator PrefabLoading()
        {
            WaitForSeconds delay =new WaitForSeconds(0.5f);
            int loadedprefab = 0;
            int loadedicons = 0;
            while (reloaded < toReload || loadedprefab < prefabCount || loadedicons < iconsCount)
            {
                loadedprefab = GetCountItems() + GetCountRecipe() + GetCountCurrencies() + GetCountItemSets() + GetCountSkills() + GetCountAbilities() + GetCountEffects() + GetCountBuildings()
                               + GetCountResourceNodes() + GetCountRaces() + GetCountGlobalEvents();
                loadedicons = GetCountLoadedItemIcons() + GetCountLoadedRecipeIcons() + GetCountLoadedCurrencyIcons() + GetCountLoadedSkillIcons() + GetCountLoadedAbilityIcons()
                              + GetCountLoadedEffectIcons() + GetCountLoadedBuildingIcons() + GetCountLoadedResourceNodeIcons();

          /*      Debug.LogError("AtavismPrefabManager: Loaded prefabs " + loadedprefab + " / " + prefabCount + " Loaded icons " + loadedicons + " / " + iconsCount+"\n"+
                               GetCountItems()+"/"+items+"\n"+
                               GetCountRecipe()+"/"+craft+"\n"+
                               GetCountCurrencies()+"/"+curr+"\n"+
                               GetCountItemSets()+"/"+itset+"\n"+
                               GetCountSkills()+"/"+skill+"\n"+
                               GetCountAbilities()+"/"+ability+"\n"+
                               GetCountEffects()+"/"+effects+"\n"+
                               GetCountBuildings()+"/"+building+"\n"+
                               GetCountResourceNodes()+"/"+resource+"\n"+
                               GetCountRaces()+"/"+race+"\n"+
                               GetCountGlobalEvents()+"/"+global+"\n\n"+
                               
                              GetCountLoadedItemIcons() +"/"+items+"\n"+
                               GetCountLoadedRecipeIcons()+"/"+craft+"\n" +
                               GetCountLoadedCurrencyIcons()+"/"+curr+"\n" +
                               GetCountLoadedSkillIcons()+"/"+skill+"\n" +
                               GetCountLoadedAbilityIcons()+"/"+ability+"\n" + 
                               GetCountLoadedEffectIcons()+"/"+effects+"\n" + 
                               GetCountLoadedBuildingIcons()+"/"+building+"\n" + 
                               GetCountLoadedResourceNodeIcons()+"/"+resource);
 /**/
                string[] args = new string[2];
                args[0] = (loadedicons + loadedprefab).ToString();
                args[1] = (prefabCount + iconsCount).ToString();
                AtavismEventSystem.DispatchEvent("LOADING_PREFAB_UPDATE", args);  
                
                yield return delay;
            }
            string[] arg = new string[1];
            AtavismEventSystem.DispatchEvent("LOADING_PREFAB_HIDE", arg);  
        }
        public void HandleReloaded(Dictionary<string, object> props)
        {
          //  Debug.LogError("HandleReloaded");
          //  reloaded = 0;
            LoadRaceData();
            LoadItemPrefabData();
            LoadItemSetPrefabData();
            LoadCurrencyPrefabData();
            LoadSkillsPrefabData();
            LoadAbilitiesPrefabData();
            LoadEffectsPrefabData();
            LoadResourceNodePrefabData();
            LoadBuldingObjectsPrefabData();
            LoadCraftingRecipePrefabData();
            LoadGlobaEventsData();
           
          //  Debug.LogError("HandleReloaded End");
        }
        
        public void HandleCombatSettings(Dictionary<string, object> props)
        {
          //  Debug.LogError("HandleCombatSettings");
            prefabReloading = (bool)props["DevMode"];
            
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("SETTINGS", args);
            
          //  Debug.LogError("HandleCombatSettings En prefabReloading="+prefabReloading);
        }

        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("ITEM_RELOAD", this);
            AtavismEventSystem.UnregisterEvent("ITEM_ICON_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("CURRENCY_ICON_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("SKILL_ICON_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("EFFECT_ICON_UPDATE", this);
            
            NetworkAPI.RemoveExtensionMessageHandler("ao.reloaded", HandleReloaded);
#if UNITY_ANDROID || UNITY_IOS
            if (!Directory.Exists(Application.persistentDataPath+"/" + Application.productName))
                Directory.CreateDirectory(Application.persistentDataPath+"/" + Application.productName);
            FileStream file = File.Create( Application.persistentDataPath+"/" + Application.productName + "/game.data");
#else
            if (!Directory.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "/" + Application.productName))
                Directory.CreateDirectory(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "/" + Application.productName);

            FileStream file = File.Create(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + "/" + Application.productName + "/game.data");
/*#else
            if (!Directory.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/" + Application.productName))
                Directory.CreateDirectory(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/" + Application.productName);

            FileStream file = File.Create(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/" + Application.productName + "/game.data");*/
#endif
            try
            {
                List<int> todatete = new List<int>();
                foreach (BuildObjPrefabData item in prefabsdatadata.buildObjects.Values)
                {
                    /*  if (item.icon == null)
                          todatete.Add(item.id);
                      else if (item.iconData.Length == 0)
                          todatete.Add(item.id);*/
                }

                foreach (int id in todatete)
                    prefabsdatadata.buildObjects.Remove(id);
                todatete.Clear();
                foreach (ItemPrefabData item in prefabsdatadata.items.Values)
                {
                    /* if (item.icon == null)
                         todatete.Add(item.templateId);
                     else if (item.iconData.Length == 0)
                         todatete.Add(item.templateId);*/
                }

                foreach (int id in todatete)
                    prefabsdatadata.items.Remove(id);
                todatete.Clear();
                foreach (CraftingRecipePrefabData item in prefabsdatadata.craftRecipes.Values)
                {
                    /*  if (item.icon == null)
                          todatete.Add(item.recipeID);
                      else if (item.iconData.Length == 0)
                          todatete.Add(item.recipeID);*/
                }

                foreach (int id in todatete)
                    prefabsdatadata.craftRecipes.Remove(id);
                todatete.Clear();


                foreach (CurrencyPrefabData item in prefabsdatadata.currencies.Values)
                {
                    /*   if (item.icon == null)
                           todatete.Add(item.id);
                       else if (item.iconData.Length == 0)
                           todatete.Add(item.id);*/
                }

                foreach (int id in todatete)
                    prefabsdatadata.currencies.Remove(id);
                todatete.Clear();

                foreach (AbilityPrefabData item in prefabsdatadata.abilities.Values)
                {
                    /*  if (item.icon == null)
                          todatete.Add(item.id);
                      else if(item.iconData.Length==0)
                          todatete.Add(item.id);*/
                }

                foreach (int id in todatete)
                    prefabsdatadata.abilities.Remove(id);
                todatete.Clear();

                foreach (SkillPrefabData item in prefabsdatadata.skills.Values)
                {
                    /* if (item.icon == null)
                         todatete.Add(item.id);
                     else if (item.iconData.Length == 0)
                         todatete.Add(item.id);*/
                }

                foreach (int id in todatete)
                    prefabsdatadata.skills.Remove(id);
                todatete.Clear();
                foreach (EffectsPrefabData item in prefabsdatadata.effects.Values)
                {
                    /*   if (item.icon == null)
                           todatete.Add(item.id);
                       else if (item.iconData.Length == 0)
                           todatete.Add(item.id);*/
                }

                foreach (int id in todatete)
                    prefabsdatadata.effects.Remove(id);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, prefabsdatadata);
            }
            catch (SerializationException e)
            {
                Debug.LogError("Failed to serialize. Reason: " + e.Message + "\n\n" + e.StackTrace);
            }
            finally
            {
                file.Close();
            }
        }


        void ClientReady()
        {
            // Debug.LogError(" Connect Prefab Server ClientReady");
        }

        public void OnEvent(AtavismEventData eData)
        {
       //  Debug.LogError("AtavismPrefabManager: "+eData.eventType);
           if (eData.eventType == "ITEM_ICON_UPDATE")
            {
                if (loadOnStartup)
                {
                    if (GetCountItems() - GetCountLoadedItemIcons() > 0)
                    {
                            LoadItemIcons(10);
                    }
                }
            }else if (eData.eventType == "Skill_ICON_UPDATE")
            {
                if (loadOnStartup)
                {
                    if (GetCountSkills() - GetCountLoadedSkillIcons() > 0)
                    {
                            LoadSkillIcons(10);
                    }
                }
            }else
           if (eData.eventType == "CURRENCY_ICON_UPDATE")
           {
               if (loadOnStartup)
               {
                   if (GetCountCurrencies() - GetCountLoadedCurrencyIcons() > 0)
                   {
                       LoadCurrencyIcons(10);
                   }
               }
           }else
           if (eData.eventType == "EFFECT_ICON_UPDATE")
           {
               if (loadOnStartup)
               {
                   if (GetCountEffects() - GetCountLoadedEffectIcons() > 0)
                   {
                       LoadEffectsIcons();
                   }
               }
           }
           else if (eData.eventType == "ITEM_RELOAD")
            {
                if (loadOnStartup)
                {
                    if (GetCountItems() - GetCountLoadedItemIcons() > 0)
                    {
                        LoadItemIcons();
                    }

                    if (GetCountSkills() - GetCountLoadedSkillIcons() > 0)
                    {
                        LoadSkillIcons();
                    }

                    if (GetCountCurrencies() - GetCountLoadedCurrencyIcons() > 0)
                    {
                        LoadCurrencyIcons();
                    }
                    if (GetCountEffects() - GetCountLoadedEffectIcons() > 0)
                    {
                        LoadEffectsIcons();
                    }
                }
            }
        }
        
        
        private void sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (!arg0.name.Equals("Login") && !arg0.name.Equals(ClientAPI.Instance.characterSceneName))
            {

            }
            else if (arg0.name.Equals("Login"))
            {
                LoadRaceData();
                LoadItemPrefabData();
                LoadItemSetPrefabData();

                LoadCurrencyPrefabData();
                LoadSkillsPrefabData();
                LoadAbilitiesPrefabData();
                LoadEffectsPrefabData();
                LoadResourceNodePrefabData();
                LoadBuldingObjectsPrefabData();
                LoadCraftingRecipePrefabData();

                LoadGlobaEventsData();
            }
        }

        public void SaveCurrency(CurrencyPrefabData cpd)
        {
            //  Debug.LogError("Save CurrencyPrefabData id=" + cpd.id + " name=" + cpd.name);
            if (prefabsdatadata.currencies.ContainsKey(cpd.id))
                prefabsdatadata.currencies.Remove(cpd.id);
            prefabsdatadata.currencies.Add(cpd.id, cpd);
        }

        public void SaveCurrencyIcon(int id, Sprite icon, string iconData)
        {
            // Debug.LogError("Save SaveCurrencyIcon id=" + id);
            if (prefabsdatadata.currencies.ContainsKey(id))
            {
                prefabsdatadata.currencies[id].icon = icon;
                prefabsdatadata.currencies[id].iconData = iconData;
                prefabsdatadata.currencies[id].iconloaded = true;
            }
        }

        public void DeleteCurrency(int id)
        {
            if (prefabsdatadata.currencies.ContainsKey(id))
                prefabsdatadata.currencies.Remove(id);
        }

        public void SaveSkill(SkillPrefabData cpd)
        {
            //   Debug.LogError("Save SkillPrefabData id=" + cpd.id + " name=" + cpd.skillname);
            if (prefabsdatadata.skills.ContainsKey(cpd.id))
                prefabsdatadata.skills.Remove(cpd.id);
            prefabsdatadata.skills.Add(cpd.id, cpd);
        }

        public void SaveSkillIcon(int id, Sprite icon, string iconData)
        {
            //  Debug.LogError("Save SaveSkillIcon id=" + id);
            if (prefabsdatadata.skills.ContainsKey(id))
            {
                prefabsdatadata.skills[id].icon = icon;
                prefabsdatadata.skills[id].iconData = iconData;
                prefabsdatadata.skills[id].iconloaded = true;
            }
        }

        public void DeleteSkill(int id)
        {
            if (prefabsdatadata.skills.ContainsKey(id))
                prefabsdatadata.skills.Remove(id);
        }

        public void SaveAbility(AbilityPrefabData cpd)
        {
            //    Debug.LogError("Save AbilityPrefabData id=" + cpd.id + " name=" + cpd.name);
            if (prefabsdatadata.abilities.ContainsKey(cpd.id))
                prefabsdatadata.abilities.Remove(cpd.id);
            prefabsdatadata.abilities.Add(cpd.id, cpd);
        }

        public void SaveAbilityIcon(int id, Sprite icon, string iconData)
        {
            //  Debug.LogError("Save SaveAbilityIcon id=" + id);
            if (prefabsdatadata.abilities.ContainsKey(id))
            {
                prefabsdatadata.abilities[id].icon = icon;
                prefabsdatadata.abilities[id].iconData = iconData;
                prefabsdatadata.abilities[id].iconloaded = true;
            }
        }

        public void DeleteAbility(int id)
        {
            if (prefabsdatadata.abilities.ContainsKey(id))
                prefabsdatadata.abilities.Remove(id);
        }

        public void SaveEffects(EffectsPrefabData cpd)
        {
            //   Debug.LogError("Save EffectsPrefabData id=" + cpd.id + " name=" + cpd.name);
            if (prefabsdatadata.effects.ContainsKey(cpd.id))
                prefabsdatadata.effects.Remove(cpd.id);
            prefabsdatadata.effects.Add(cpd.id, cpd);
        }

        public void SaveEffectsIcon(int id, Sprite icon, string iconData)
        {
            //  Debug.LogError("Save SaveEffectsIcon id=" + id);
            if (prefabsdatadata.effects.ContainsKey(id))
            {
                prefabsdatadata.effects[id].icon = icon;
                prefabsdatadata.effects[id].iconData = iconData;
                prefabsdatadata.effects[id].iconloaded = true;
            }
        }

        public void DeleteEffect(int id)
        {
            if (prefabsdatadata.effects.ContainsKey(id))
                prefabsdatadata.effects.Remove(id);
        }

        public void SaveItem(ItemPrefabData ipd)
        {
            //  Debug.LogError("Save ItemPrefabData id=" + ipd.templateId + " name=" + ipd.name);
            if (prefabsdatadata.items.ContainsKey(ipd.templateId))
                prefabsdatadata.items.Remove(ipd.templateId);
            prefabsdatadata.items.Add(ipd.templateId, ipd);
        }

        public void SaveItemsIcon(int id, Sprite icon, string iconData)
        {
            //  Debug.LogError("Save SaveItemsIcon id=" + id);
            if (prefabsdatadata.items.ContainsKey(id))
            {
                prefabsdatadata.items[id].icon = icon;
                prefabsdatadata.items[id].iconData = iconData;
                prefabsdatadata.items[id].iconloaded = true;
            }
        }

        public void DeleteItem(int id)
        {
            if (prefabsdatadata.items.ContainsKey(id))
                prefabsdatadata.items.Remove(id);
        }

        public void SaveItemSet(ItemSetPrefabData ipd)
        {
            //   Debug.LogError("Save ItemSetPrefabData id=" + ipd.Setid + " name=" + ipd.Name);
            if (prefabsdatadata.itemSets.ContainsKey(ipd.Setid))
                prefabsdatadata.itemSets.Remove(ipd.Setid);
            prefabsdatadata.itemSets.Add(ipd.Setid, ipd);

            foreach (var itemId in ipd.itemList)
            {
                if (prefabsdatadata.items.ContainsKey(itemId))
                {
                    prefabsdatadata.items[itemId].setId = ipd.Setid;
                }
            }
        }


        public void DeleteItemSet(int id)
        {
            if (prefabsdatadata.itemSets.ContainsKey(id))
            {
                foreach (var itemId in prefabsdatadata.itemSets[id].itemList)
                {
                    if (prefabsdatadata.items.ContainsKey(itemId))
                    {
                        prefabsdatadata.items[itemId].setId = 0;
                    }
                }

                prefabsdatadata.itemSets.Remove(id);
            }
        }

        public void SaveCrafringRecipe(CraftingRecipePrefabData ipd)
        {
            //   Debug.LogError("Save SaveCrafringRecipe id=" + ipd.recipeID + " name=" + ipd.recipeName);
            if (prefabsdatadata.craftRecipes.ContainsKey(ipd.recipeID))
                prefabsdatadata.craftRecipes.Remove(ipd.recipeID);
            prefabsdatadata.craftRecipes.Add(ipd.recipeID, ipd);
        }

        public void SaveCrafringRecipeIcon(int id, Sprite icon, string iconData)
        {
            //   Debug.LogError("Save SaveCrafringRecipeIcon id=" + id);
            if (prefabsdatadata.craftRecipes.ContainsKey(id))
            {
                prefabsdatadata.craftRecipes[id].icon = icon;
                prefabsdatadata.craftRecipes[id].iconData = iconData;
                prefabsdatadata.craftRecipes[id].iconloaded = true;
            }
        }

        public void DeleteCrafringRecipe(int id)
        {
            if (prefabsdatadata.craftRecipes.ContainsKey(id))
                prefabsdatadata.craftRecipes.Remove(id);
        }

        public void SaveBuildObject(BuildObjPrefabData ipd)
        {
            // Debug.LogError("Save BuildObjPrefabData id=" + ipd.id + " name=" + ipd.buildObjectName);
            if (prefabsdatadata.buildObjects.ContainsKey(ipd.id))
                prefabsdatadata.buildObjects.Remove(ipd.id);
            prefabsdatadata.buildObjects.Add(ipd.id, ipd);
        }

        public void SaveBuildObjectIcon(int id, Sprite icon, string iconData)
        {
            //   Debug.LogError("Save SaveItemsIcon id=" + id);
            if (prefabsdatadata.buildObjects.ContainsKey(id))
            {
                prefabsdatadata.buildObjects[id].icon = icon;
                prefabsdatadata.buildObjects[id].iconData = iconData;
                prefabsdatadata.buildObjects[id].iconloaded = true;
            }
        }

        public void DeleteBuildObject(int id)
        {
            if (prefabsdatadata.buildObjects.ContainsKey(id))
                prefabsdatadata.buildObjects.Remove(id);
        }

        public void DeleteGlobalEvent(int id)
        {
            if (prefabsdatadata.globalEvents.ContainsKey(id))
                prefabsdatadata.globalEvents.Remove(id);
        }

        public ItemPrefabData GetItemTemplateByID(int id)
        {
            if (prefabsdatadata.items.ContainsKey(id))
                return prefabsdatadata.items[id];
            return null;
        }

        public Sprite GetItemIconByID(int id)
        {
            if (prefabsdatadata.items.ContainsKey(id))
                return prefabsdatadata.items[id].icon;
            return null;
        }
//Item
        public int GetCountLoadedItemIcons()
        {
            int count = 0;
            foreach (var item in prefabsdatadata.items.Values)
            {
                if (item.iconData.Length > 0 || item.iconloaded)
                    count++;
            }

            return count;
        }
        
        public int GetCountItems()
        {
            return prefabsdatadata.items.Count;
        }
//Recipe        
        public int GetCountLoadedRecipeIcons()
        {
            int count = 0;
            foreach (var item in prefabsdatadata.craftRecipes.Values)
            {
                if (item.iconData.Length > 0|| item.iconloaded)
                    count++;
            }

            return count;
        }

        public int GetCountRecipe()
        {
            return prefabsdatadata.craftRecipes.Count;
        }
//Skill
        public int GetCountLoadedSkillIcons()
        {
            int count = 0;
            foreach (var skill in prefabsdatadata.skills.Values)
            {
                if (skill.iconData.Length > 0|| skill.iconloaded)
                    count++;
            }

            return count;
        }

        public int GetCountSkills()
        {
            return prefabsdatadata.skills.Count;
        }
//currency
        public int GetCountLoadedCurrencyIcons()
        {
            int count = 0;
            foreach (var currency in prefabsdatadata.currencies.Values)
            {
                if (currency.iconData.Length > 0|| currency.iconloaded)
                    count++;
            }

            return count;
        }

        public int GetCountCurrencies()
        {
            return prefabsdatadata.currencies.Count;
        }

//ability
        public int GetCountLoadedAbilityIcons()
        {
            int count = 0;
            foreach (var ability in prefabsdatadata.abilities.Values)
            {
                if (ability.iconData.Length > 0|| ability.iconloaded)
                    count++;
            }

            return count;
        }

        public int GetCountAbilities()
        {
            return prefabsdatadata.abilities.Count;
        } 
//effect
        public int GetCountLoadedEffectIcons()
        {
            int count = 0;
            foreach (var effect in prefabsdatadata.effects.Values)
            {
                if (effect.iconData.Length > 0|| effect.iconloaded)
                    count++;
            }

            return count;
        }

        public int GetCountEffects()
        {
            return prefabsdatadata.effects.Count;
        }

//Building
        public int GetCountLoadedBuildingIcons()
        {
            int count = 0;
            foreach (var Building in prefabsdatadata.buildObjects.Values)
            {
                if (Building.iconData.Length > 0|| Building.iconloaded)
                    count++;
            }

            return count;
        }

        public int GetCountBuildings()
        {
            return prefabsdatadata.buildObjects.Count;
        }

//ResourceNode
        public int GetCountLoadedResourceNodeIcons()
        {
            int count = 0;
            foreach (var rn in prefabsdatadata.recourceNode.Values)
            {
                bool ok = true;
                foreach (var rnsl in rn.settingList.Values)
                {
                    if (rnsl.cursorIconData.Length==0 || rnsl.selectedIconData.Length==0)
                    {
                        ok = false;
                    }
                }
                if (ok)
                    count++;
            }

            return count;
        }
//Races
        public int GetCountRaces()
        {
            int count = 0;
            foreach (var race in prefabsdatadata.races.Values)
            {
                count += race.classList.Count;
            }
            
            return count;
        }
//sets
        public int GetCountItemSets()
        {
            return prefabsdatadata.itemSets.Count;
        }
//Global ebents
        public int GetCountGlobalEvents()
        {
            return prefabsdatadata.globalEvents.Count;
        }

        public int GetCountResourceNodes()
        {
            return prefabsdatadata.recourceNode.Count;
        }
        
        
        public Dictionary<int, RaceData> GetRaceData()
        {
            return prefabsdatadata.races;
        }

        public Dictionary<int, ItemSetPrefabData>.ValueCollection GetItemSetPrefabData()
        {
            return prefabsdatadata.itemSets.Values;
        }

        public Dictionary<int, ItemPrefabData>.ValueCollection GetItemPrefabData()
        {
            return prefabsdatadata.items.Values;
        }

        public Dictionary<int, CraftingRecipePrefabData>.ValueCollection GetCraftingRecipesPrefabData()
        {
            return prefabsdatadata.craftRecipes.Values;
        }

        public Dictionary<int, CurrencyPrefabData>.ValueCollection GetCurrencyPrefabData()
        {
            return prefabsdatadata.currencies.Values;
        }

        public Dictionary<int, SkillPrefabData>.ValueCollection GetSkillPrefabData()
        {
            return prefabsdatadata.skills.Values;
        }

        public Dictionary<int, AbilityPrefabData>.ValueCollection GetAbilityPrefabData()
        {
            return prefabsdatadata.abilities.Values;
        }

        public Dictionary<int, EffectsPrefabData>.ValueCollection GetEffectPrefabData()
        {
            return prefabsdatadata.effects.Values;
        }

        public Dictionary<int, BuildObjPrefabData>.ValueCollection GetBuildObjPrefabData()
        {
            return prefabsdatadata.buildObjects.Values;
        }

        public Dictionary<int, RecourceNodeProfileData>.ValueCollection GetResourceNodePrefabData()
        {
            return prefabsdatadata.recourceNode.Values;
        }

        public Dictionary<int, GlobalEventData>.ValueCollection GetGlobalEventPrefabData()
        {
            return prefabsdatadata.globalEvents.Values;
        }
        public Dictionary<int, RaceData>.ValueCollection GetRacesPrefabData()
        {
            return prefabsdatadata.races.Values;
        }

        public AtavismInventoryItem LoadItem(string Name)
        {
            foreach (ItemPrefabData ipd in prefabsdatadata.items.Values)
            {
                //  Debug.LogWarning("LoadItem by name "+ Name+" prefName="+ipd.name);
                if (ipd != null && ipd.name.Equals(Name))
                {
                    AtavismInventoryItem item = new AtavismInventoryItem();
                    item.templateId = ipd.templateId;
                    item.name = ipd.name;
                    item.icon = ipd.icon;
                    item.tooltip = ipd.tooltip;
                    item.itemType = ipd.itemType;
                    item.subType = ipd.subType;
                    item.slot = ipd.slot;
                    item.quality = ipd.quality;
                    item.currencyType = ipd.currencyType;
                    item.cost = ipd.cost;
                    item.binding = ipd.binding;
                    item.sellable = ipd.sellable;
                    item.DamageValue = ipd.damageValue;
                    item.DamageMaxValue = ipd.damageMaxValue;
                    item.SetId = ipd.setId;
                    item.EnchantId = ipd.enchantId;
                    item.WeaponSpeed = ipd.weaponSpeed;
                    item.StackLimit = ipd.stackLimit;
                    item.auctionHouse = ipd.auctionHouse;
                    item.Unique = ipd.unique;
                    item.gear_score = ipd.gear_score;

                    item.sockettype = ipd.sockettype;
                    item.Durability = ipd.durability;
                    item.MaxDurability = ipd.durability;
                    item.weight = ipd.weight;
                    item.autoattack = ipd.autoattack;
                    item.ammoType = ipd.ammoType;
                    item.deathLoss = ipd.deathLoss;
                    item.parry = ipd.parry;
                    item.repairable = ipd.repairable; 
                        
                    item.itemEffectTypes = ipd.itemEffectTypes;
                    item.itemEffectNames = ipd.itemEffectNames;
                    item.itemEffectValues = ipd.itemEffectValues;

                    item.itemReqTypes = ipd.itemReqTypes;
                    item.itemReqNames = ipd.itemReqNames;
                    item.itemReqValues = ipd.itemReqValues;
                    return item;
                }
            }

            return null;
        }

        public AtavismInventoryItem LoadItem(string Name, AtavismInventoryItem item)
        {
            foreach (ItemPrefabData ipd in prefabsdatadata.items.Values)
            {
                //  Debug.LogWarning("LoadItem by name "+ Name+" prefName="+ipd.name);
                if (ipd != null && ipd.name.Equals(Name))
                {
                    item.templateId = ipd.templateId;
                    item.name = ipd.name;
                    item.icon = ipd.icon;
                    item.tooltip = ipd.tooltip;
                    item.itemType = ipd.itemType;
                    item.subType = ipd.subType;
                    item.slot = ipd.slot;
                    item.quality = ipd.quality;
                    item.currencyType = ipd.currencyType;
                    item.cost = ipd.cost;
                    item.binding = ipd.binding;
                    item.sellable = ipd.sellable;
                    item.DamageValue = ipd.damageValue;
                    item.DamageMaxValue = ipd.damageMaxValue;
                    item.SetId = ipd.setId;
                    item.EnchantId = ipd.enchantId;
                    item.WeaponSpeed = ipd.weaponSpeed;
                    item.StackLimit = ipd.stackLimit;
                    item.auctionHouse = ipd.auctionHouse;
                    item.Unique = ipd.unique;
                    item.gear_score = ipd.gear_score;

                    item.sockettype = ipd.sockettype;
                    item.Durability = ipd.durability;
                    item.MaxDurability = ipd.durability;
                    item.weight = ipd.weight;
                    item.autoattack = ipd.autoattack;
                    item.ammoType = ipd.ammoType;
                    item.deathLoss = ipd.deathLoss;
                    item.parry = ipd.parry;
                    item.repairable = ipd.repairable; 

                    item.itemEffectTypes = ipd.itemEffectTypes;
                    item.itemEffectNames = ipd.itemEffectNames;
                    item.itemEffectValues = ipd.itemEffectValues;

                    item.itemReqTypes = ipd.itemReqTypes;
                    item.itemReqNames = ipd.itemReqNames;
                    item.itemReqValues = ipd.itemReqValues;
                    return item;
                }
            }

            return null;
        }

        public AtavismInventoryItem LoadItem(int id)
        {
            if (prefabsdatadata.items.ContainsKey(id))
            {
                AtavismInventoryItem item = new AtavismInventoryItem();

                ItemPrefabData ipd = prefabsdatadata.items[id];
                if (ipd != null)
                {

                    if (ipd.icon == null)
                    {
                        if (!itemIconGet.ContainsKey(id))
                        {
                            Dictionary<string, object> ps = new Dictionary<string, object>();
                            ps.Add("objs", id + ";");
                            //  Debug.LogWarning("!!!!!!!!!!!!    Get  Item Icon for id " + id);
                            AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "ItemIcon");
                            itemIconGet.Add(id, Time.time);
                        }
                        else
                        {
                            if (itemIconGet[id] + 2f < Time.time)
                            {
                                Dictionary<string, object> ps = new Dictionary<string, object>();
                                ps.Add("objs", id + ";");
                                //    Debug.LogWarning("!!!!!!!!!!!!    Get  Item Icon for id " + id);
                                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "ItemIcon");
                                itemIconGet[id] = Time.time;
                            }
                        }
                    }

                    item.templateId = ipd.templateId;
                    item.name = ipd.name;
                    item.icon = ipd.icon;
                    item.tooltip = ipd.tooltip;
                    item.itemType = ipd.itemType;
                    item.subType = ipd.subType;
                    item.slot = ipd.slot;
                    item.quality = ipd.quality;
                    item.currencyType = ipd.currencyType;
                    item.cost = ipd.cost;
                    item.binding = ipd.binding;
                    item.sellable = ipd.sellable;
                    item.DamageValue = ipd.damageValue;
                    item.DamageMaxValue = ipd.damageMaxValue;
                    item.SetId = ipd.setId;
                    item.EnchantId = ipd.enchantId;
                    item.WeaponSpeed = ipd.weaponSpeed;
                    item.StackLimit = ipd.stackLimit;
                    item.auctionHouse = ipd.auctionHouse;
                    item.Unique = ipd.unique;
                    item.gear_score = ipd.gear_score;

                    item.Durability = ipd.durability;
                    item.MaxDurability = ipd.durability;
                    item.sockettype = ipd.sockettype;
                    item.weight = ipd.weight;
                    item.autoattack = ipd.autoattack;
                    item.ammoType = ipd.ammoType;
                    item.deathLoss = ipd.deathLoss;
                    item.parry = ipd.parry;
                    item.repairable = ipd.repairable; 

                    item.itemEffectTypes = ipd.itemEffectTypes;
                    item.itemEffectNames = ipd.itemEffectNames;
                    item.itemEffectValues = ipd.itemEffectValues;

                    item.itemReqTypes = ipd.itemReqTypes;
                    item.itemReqNames = ipd.itemReqNames;
                    item.itemReqValues = ipd.itemReqValues;

                }
                else
                {
                    AtavismLogger.LogError("Item definiction id=" + id + " is null");
                    return null;
                }

                return item;
            }
            else
            {
                if (id > 0)
                {

                    AtavismLogger.LogError("Storage doesn't contain item definition id=" + id);
                    //  LoadItemPrefabData();
                }

            }

            return null;
        }

        public AtavismInventoryItem LoadItem(AtavismInventoryItem item)
        {
            //Debug.LogError("LoadItem  "+item);
            if(item!=null)
            if (prefabsdatadata.items.ContainsKey(item.templateId))
            {
                ItemPrefabData ipd = prefabsdatadata.items[item.templateId];
                if (ipd != null)
                {

                    if (ipd.icon == null)
                    {
                        //  Debug.LogWarning("!!!!!!!!!!!!      Item " + id + " | icon is null");
                        if (!itemIconGet.ContainsKey(item.templateId))
                        {
                            Dictionary<string, object> ps = new Dictionary<string, object>();
                            ps.Add("objs", item.templateId + ";");
                            //   Debug.LogWarning("!!!!!!!!!!!!    Get  Item Icon for id " + id);
                            AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "ItemIcon");
                            itemIconGet.Add(item.templateId, Time.time);
                        }
                        else
                        {
                            if (itemIconGet[item.templateId] + 2f < Time.time)
                            {
                                Dictionary<string, object> ps = new Dictionary<string, object>();
                                ps.Add("objs", item.templateId + ";");
                                // Debug.LogWarning("!!!!!!!!!!!!    Get  Item Icon for id " + id);
                                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "ItemIcon");
                                itemIconGet[item.templateId] = Time.time;
                            }
                        }
                    }

                    item.templateId = ipd.templateId;
                    item.name = ipd.name;
                    item.icon = ipd.icon;
                    item.tooltip = ipd.tooltip;
                    item.itemType = ipd.itemType;
                    item.subType = ipd.subType;
                    item.slot = ipd.slot;
                    item.quality = ipd.quality;
                    item.currencyType = ipd.currencyType;
                    item.cost = ipd.cost;
                    item.binding = ipd.binding;
                    item.sellable = ipd.sellable;
                    item.DamageValue = ipd.damageValue;
                    item.DamageMaxValue = ipd.damageMaxValue;
                    item.SetId = ipd.setId;
                    item.EnchantId = ipd.enchantId;
                    item.WeaponSpeed = ipd.weaponSpeed;
                    item.StackLimit = ipd.stackLimit;
                    item.auctionHouse = ipd.auctionHouse;
                    item.Unique = ipd.unique;
                    item.gear_score = ipd.gear_score;

                   // item.Durability = ipd.durability;
                    item.MaxDurability = ipd.durability;
                    item.sockettype = ipd.sockettype;
                    item.weight = ipd.weight;
                    item.autoattack = ipd.autoattack;
                    item.ammoType = ipd.ammoType;
                    item.deathLoss = ipd.deathLoss;
                    item.parry = ipd.parry;
                    item.repairable = ipd.repairable; 

                    item.itemEffectTypes = ipd.itemEffectTypes;
                    item.itemEffectNames = ipd.itemEffectNames;
                    item.itemEffectValues = ipd.itemEffectValues;

                    item.itemReqTypes = ipd.itemReqTypes;
                    item.itemReqNames = ipd.itemReqNames;
                    item.itemReqValues = ipd.itemReqValues;

                }
                else
                {
                    AtavismLogger.LogError("Item definiction id=" + item.templateId + " is null");
                    return null;
                }

                return item;
            }
            return null;
        }

        public void LoadItemIcons(int limit = 5)
        {
            List<int> iconLack = new List<int>();
            foreach (var item in prefabsdatadata.items.Values)
            {
                if (item.iconData.Length == 0)
                {
                    if (itemIconGet.ContainsKey(item.templateId))
                    {
                        if (itemIconGet[item.templateId] + 2f < Time.time)
                            iconLack.Add(item.templateId);
                    }
                    else
                    {
                        iconLack.Add(item.templateId);
                    }
                }

                if (iconLack.Count >= limit)
                    break;
            }

            if (iconLack.Count > 0)
            {
                string s = "";
                foreach (int id in iconLack)
                {
                    s += id + ";";
                    itemIconGet[id] = Time.time;

                }
                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("objs", s);
                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "ItemIcon");
            }
        }
        public void LoadSkillIcons(int limit = 5)
        {
            List<int> iconLack = new List<int>();
            foreach (var skill in prefabsdatadata.skills.Values)
            {
                if (skill.iconData.Length == 0)
                {
                    if (skillIconGet.ContainsKey(skill.id))
                    {
                        if (skillIconGet[skill.id] + 2f < Time.time)
                            iconLack.Add(skill.id);
                    }
                    else
                    {
                        iconLack.Add(skill.id);
                    }
                }

                if (iconLack.Count >= limit)
                    break;
            }

            if (iconLack.Count > 0)
            {
                string s = "";
                foreach (int id in iconLack)
                {
                    s += id + ";";
                    skillIconGet[id] = Time.time;

                }
                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("objs", s);
                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "SkillIcon");
            }
        }
        
        public void LoadCurrencyIcons(int limit = 5)
        {
            List<int> iconLack = new List<int>();
            foreach (var currency in prefabsdatadata.currencies.Values)
            {
                if (currency.iconData.Length == 0)
                {
                    if (currencyIconGet.ContainsKey(currency.id))
                    {
                        if (currencyIconGet[currency.id] + 2f < Time.time)
                            iconLack.Add(currency.id);
                    }
                    else
                    {
                        iconLack.Add(currency.id);
                    }
                }

                if (iconLack.Count >= limit)
                    break;
            }

            if (iconLack.Count > 0)
            {
                string s = "";
                foreach (int id in iconLack)
                {
                    s += id + ";";
                    currencyIconGet[id] = Time.time;

                }
                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("objs", s);
                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "CurrencyIcon");
            }
        }

        public void LoadEffectsIcons(int limit = 5)
        {
            List<int> iconLack = new List<int>();
            foreach (var eff in prefabsdatadata.effects.Values)
            {
                if (eff.iconData.Length == 0)
                {
                    if (effectsIconGet.ContainsKey(eff.id))
                    {
                        if (effectsIconGet[eff.id] + 2f < Time.time)
                            iconLack.Add(eff.id);
                    }
                    else
                    {
                        iconLack.Add(eff.id);
                    }
                }

                if (iconLack.Count >= limit)
                    break;
            }

            if (iconLack.Count > 0)
            {
                string s = "";
                foreach (int id in iconLack)
                {
                    s += id + ";";
                    effectsIconGet[id] = Time.time;

                }
                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("objs", s);
                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "EffectIcon");
            }
        }

        public AtavismInventoryItemSet LoadItemSet(int id)
        {
            AtavismInventoryItemSet aiis = new AtavismInventoryItemSet();
            if (prefabsdatadata.itemSets.ContainsKey(id))
            {
                ItemSetPrefabData ipd = prefabsdatadata.itemSets[id];
                if (ipd != null)
                {
                    aiis.Setid = ipd.Setid;
                    aiis.Name = ipd.Name;
                    aiis.itemList = ipd.itemList;
                    aiis.levelList = ipd.levelList;
                }
                else
                {
                    AtavismLogger.LogError("Item set definiction id=" + id + " is null");
                    return null;
                }
            }
            else
            {
                if (id > 0)
                {
                    AtavismLogger.LogError("Storage doesn't contain item set definition id=" + id);
                   // LoadItemSetPrefabData();
                }
                return null;
            }
            return aiis;
        }
        public List<AtavismInventoryItemSet> LoadAllItemSet()
        {
            List<AtavismInventoryItemSet> list = new List<AtavismInventoryItemSet>();
            foreach (ItemSetPrefabData ipd in prefabsdatadata.itemSets.Values)
            {
                AtavismInventoryItemSet aiis = new AtavismInventoryItemSet();
                if (ipd != null)
                {
                    aiis.Setid = ipd.Setid;
                    aiis.Name = ipd.Name;
                    aiis.itemList = ipd.itemList;
                    aiis.levelList = ipd.levelList;
                    list.Add(aiis);
                }
            }
            return list;
        }

        public AtavismBuildObjectTemplate LoadBuildObject(int id)
        {
            AtavismBuildObjectTemplate aiis = new AtavismBuildObjectTemplate();
            if (prefabsdatadata.buildObjects.ContainsKey(id))
            {
                BuildObjPrefabData ipd = prefabsdatadata.buildObjects[id];
                if (ipd != null)
                {
                    aiis.id = ipd.id;
                    aiis.buildObjectName = ipd.buildObjectName;
                    aiis.icon = ipd.icon;
                    aiis.category = ipd.category;
                    aiis.skill = ipd.skill;
                    aiis.skillLevelReq = ipd.skillLevelReq;
                    aiis.distanceReq = ipd.distanceReq;
                    aiis.buildTaskReqPlayer = ipd.buildTaskReqPlayer;
                    aiis.validClaimTypes = ipd.validClaimTypes;
                    aiis.onlyAvailableFromItem = ipd.onlyAvailableFromItem;
                    aiis.reqWeapon = ipd.reqWeapon;
                    aiis.gameObject = ipd.gameObject;
                    aiis.itemsReq = ipd.itemsReq;
                    aiis.itemsReqCount = ipd.itemsReqCount;
                    aiis.upgradeItemsReq = ipd.upgradeItemsReq;
                    aiis.buildTimeReq = ipd.buildTimeReq;
                }
                else
                {
                    AtavismLogger.LogError("BuildObject definiction id=" + id + " is null");
                    return null;
                }
            }
            else
            {
                if (id > 0)
                {

                    AtavismLogger.LogError("Storage doesn't contain BuildObject definition id=" + id);
                 //   LoadBuldingObjectsPrefabData();
                }
                return null;
            }
            return aiis;
        }
        public List<AtavismBuildObjectTemplate> LoadAllBuildObject()
        {
            List<AtavismBuildObjectTemplate> list = new List<AtavismBuildObjectTemplate>();
            List<int> iconLack = new List<int>();
            foreach (BuildObjPrefabData ipd in prefabsdatadata.buildObjects.Values)
            {
                AtavismBuildObjectTemplate aiis = new AtavismBuildObjectTemplate();
                    //go.AddComponent<AtavismBuildObjectTemplate>();
                if (ipd != null)
                {

                    aiis.id = ipd.id;
                    aiis.buildObjectName = ipd.buildObjectName;
                    aiis.icon = ipd.icon;
                    aiis.category = ipd.category;
                    aiis.skill = ipd.skill;
                    aiis.skillLevelReq = ipd.skillLevelReq;
                    aiis.distanceReq = ipd.distanceReq;
                    aiis.buildTaskReqPlayer = ipd.buildTaskReqPlayer;
                    aiis.validClaimTypes = ipd.validClaimTypes;
                    aiis.onlyAvailableFromItem = ipd.onlyAvailableFromItem;
                    aiis.reqWeapon = ipd.reqWeapon;
                    aiis.gameObject = ipd.gameObject;
                    aiis.itemsReq = ipd.itemsReq;
                    aiis.itemsReqCount = ipd.itemsReqCount;
                    aiis.upgradeItemsReq = ipd.upgradeItemsReq;
                    aiis.buildTimeReq = ipd.buildTimeReq;
                    list.Add(aiis);
                    if (aiis.icon == null)
                    {
                        iconLack.Add(aiis.id);
                    }
                }
            }
            if (iconLack.Count > 0)
            {
                string s = "";
                foreach (int id in iconLack)
                {
                    s += id + ";";
                }

                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("objs", s);
                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "BuildingObjectIcon");

            }
            return list;
        }


        public AtavismCraftingRecipe LoadCraftingRecipe(int id)
        {
            AtavismCraftingRecipe aiis = new AtavismCraftingRecipe();
            if (prefabsdatadata.craftRecipes.ContainsKey(id))
            {
                CraftingRecipePrefabData ipd = prefabsdatadata.craftRecipes[id];
                if (ipd != null)
                {
                    aiis.recipeID = ipd.recipeID;
                    aiis.recipeName = ipd.recipeName;
                    aiis.icon = ipd.icon;
                    aiis.skillID = ipd.skillID;
                    aiis.skillLevelReq = ipd.skillLevelReq;
                    aiis.stationReq = ipd.stationReq;
                    aiis.creationTime = ipd.creationTime;

                    aiis.createsItems = ipd.createsItems;
                    aiis.createsItemsCounts = ipd.createsItemsCounts;
                    aiis.createsItems2 = ipd.createsItems2;
                    aiis.createsItemsCounts2 = ipd.createsItemsCounts2;
                    aiis.createsItems3 = ipd.createsItems3;
                    aiis.createsItemsCounts3 = ipd.createsItemsCounts3;
                    aiis.createsItems4 = ipd.createsItems4;
                    aiis.createsItemsCounts4 = ipd.createsItemsCounts4;
                    aiis.itemsReq = ipd.itemsReq;
                    aiis.itemsReqCounts = ipd.itemsReqCounts;
                }
                else
                {
                    AtavismLogger.LogError("CraftingRecipe definiction id=" + id + " is null");
                    return null;
                }
            }
            else
            {
                if (id > 0)
                {
                    AtavismLogger.LogError("Storage doesn't contain CraftingRecipe definition id=" + id);
                  //  LoadCraftingRecipePrefabData();
                }
                return null;
            }
            return aiis;
        }
        public List<AtavismCraftingRecipe> LoadAllCraftingRecipe()
        {
            List<AtavismCraftingRecipe> list = new List<AtavismCraftingRecipe>();
            List<int> iconLack = new List<int>();
            foreach (CraftingRecipePrefabData ipd in prefabsdatadata.craftRecipes.Values)
            {
                AtavismCraftingRecipe aiis = new AtavismCraftingRecipe();
                if (ipd != null)
                {
                    aiis.recipeID = ipd.recipeID;
                    aiis.recipeName = ipd.recipeName;
                    aiis.icon = ipd.icon;
                    aiis.skillID = ipd.skillID;
                    aiis.skillLevelReq = ipd.skillLevelReq;
                    aiis.stationReq = ipd.stationReq;
                    aiis.creationTime = ipd.creationTime;

                    aiis.createsItems = ipd.createsItems;
                    aiis.createsItemsCounts = ipd.createsItemsCounts;
                    aiis.createsItems2 = ipd.createsItems2;
                    aiis.createsItemsCounts2 = ipd.createsItemsCounts2;
                    aiis.createsItems3 = ipd.createsItems3;
                    aiis.createsItemsCounts3 = ipd.createsItemsCounts3;
                    aiis.createsItems4 = ipd.createsItems4;
                    aiis.createsItemsCounts4 = ipd.createsItemsCounts4;
                    aiis.itemsReq = ipd.itemsReq;
                    aiis.itemsReqCounts = ipd.itemsReqCounts;
                    list.Add(aiis);
                    if (aiis.icon == null)
                    {
                        iconLack.Add(aiis.recipeID);
                    }
                }
            }
            if (iconLack.Count > 0)
            {
                string s = "";
                foreach (int id in iconLack)
                {
                    s += id + ";";
                }

                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("objs", s);
                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "CraftingRecipeIcon");
            }
            return list;
        }



        public Currency LoadCurrency(int id)
        {
            if (prefabsdatadata.currencies.ContainsKey(id))
            {
                Currency aiis = new Currency();

                CurrencyPrefabData ipd = prefabsdatadata.currencies[id];
                if (ipd != null)
                {
                    aiis.id = ipd.id;
                    aiis.name = ipd.name;
                    aiis.icon = ipd.icon;
                    aiis.convertsTo = ipd.convertsTo;
                    aiis.conversionAmountReq = ipd.conversionAmountReq;
                  //  aiis.description = ipd.description;
                    aiis.max = ipd.max;
                    aiis.position = ipd.position;
                  //  aiis.group = ipd.;
                    aiis.group = ipd.group;
                }
                else
                {
                    AtavismLogger.LogError("Currency definiction id=" + id + " is null");
                    return null;
                }
                return aiis;
            }
            else
            {
                if (id > 0)
                {

                    AtavismLogger.LogError("Storage doesn't contain Currency definition id=" + id);
                //    LoadCurrencyPrefabData();
                }

            }
            return null;
        }

        public List<Currency> LoadAllCurrency()
        {
            List<Currency> list = new List<Currency>();
            List<int> iconLack = new List<int>();
            foreach (CurrencyPrefabData ipd in prefabsdatadata.currencies.Values)
            {
                Currency aiis = new Currency();
                if (ipd != null)
                {
                    aiis.id = ipd.id;
                    aiis.name = ipd.name;
                    aiis.icon = ipd.icon;
                    aiis.convertsTo = ipd.convertsTo;
                    aiis.conversionAmountReq = ipd.conversionAmountReq;
                    //  aiis.description = ipd.description;
                    aiis.max = ipd.max;
                    aiis.position = ipd.position;
                    //  aiis.group = ipd.;
                    aiis.group = ipd.group;
                    list.Add(aiis);
                    if (aiis.icon == null)
                    {
                        iconLack.Add(aiis.id);
                    }
                }
            }
            if (iconLack.Count > 0)
            {
                string s = "";
                foreach (int id in iconLack)
                {
                    s += id + ";";
                }
                
                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("objs", s);
                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "CurrencyIcon");
            }
            return list;
        }



        public AtavismEffect LoadEffect(int id)
        {
          
            if (prefabsdatadata.effects.ContainsKey(id))
            {
                AtavismEffect item = new AtavismEffect();
                EffectsPrefabData ipd = prefabsdatadata.effects[id];
                if (ipd != null)
                {
                    item.id = ipd.id;
                    item.name = ipd.name;
                    item.icon = ipd.icon;
                    item.tooltip = ipd.tooltip;
                    item.isBuff = ipd.isBuff;
                    item.stackLimit = ipd.stackLimit;
                    item.stackTime = ipd.stackTime;
                    item.allowMultiple = ipd.allowMultiple;
                }
                else
                {
                    AtavismLogger.LogError("effect definiction id=" + id + " is null");
                    return null;
                }
                return item;
            }
            else
            {
                if (id > 0)
                {

                    AtavismLogger.LogError("Storage doesn't contain effect definition id=" + id);
                  //  LoadEffectsPrefabData();
                }
               
            }
            return null;
        }

        public AbilityPrefabData GetAbilityPrefab(int id)
        {
           return prefabsdatadata.abilities[id];
        }

        public AtavismAbility LoadAbility(int id)
        {
          
            if (prefabsdatadata.abilities.ContainsKey(id))
            {
                AtavismAbility aa = new AtavismAbility();
                AbilityPrefabData ipd = prefabsdatadata.abilities[id];
                if (ipd != null)
                {
                    aa.id = ipd.id;
                    aa.name = ipd.name;
                    aa.icon = ipd.icon;
                    aa.tooltip = ipd.tooltip;
                    aa.cost = ipd.cost;
                    aa.costPercentage = ipd.costPercentage;
                    aa.costProperty = ipd.costProperty;
                    aa.pulseCost = ipd.pulseCost;
                    aa.pulseCostPercentage = ipd.pulseCostPercentage;
                    aa.pulseCostProperty = ipd.pulseCostProperty;
                    aa.globalcd = ipd.globalcd;
                    aa.weaponcd = ipd.weaponcd;
                    aa.cooldownType = ipd.cooldownType;
                    aa.cooldownLength = ipd.cooldownLength;
                    aa.reagentReq = ipd.reagentReq;
                    aa.weaponReq = ipd.weaponReq;
                    aa.distance = ipd.distance;
                    aa.castingInRun = ipd.castingInRun;
                    aa.targetType = ipd.targetType;
                    aa.castTime = ipd.castTime;
                    aa.passive = ipd.passive;
                    aa.toggle = ipd.toggle;
                    aa.aoeType = ipd.aoeType;
                    aa.aoeRadius = ipd.aoeRadius;
                    aa.minRange = ipd.minRange;
                    aa.maxRange = ipd.maxRange;
                }
                else
                {
                    AtavismLogger.LogError("ability definiction id=" + id + " is null");
                    return null;
                }
                return aa;

            }
            else
            {
                if (id > 0)
                {

                    AtavismLogger.LogError("Storage doesn't contain ability definition id=" + id);
                  //  LoadAbilitiesPrefabData();
                }
               
            }
            return null;
        }
        
            public AtavismAbility LoadAbility(AtavismAbility aa)
        {
          if(aa!=null)
            if (prefabsdatadata.abilities.ContainsKey(aa.id))
            {
                AbilityPrefabData ipd = prefabsdatadata.abilities[aa.id];
                if (ipd != null)
                {
                    aa.id = ipd.id;
                    aa.name = ipd.name;
                    aa.icon = ipd.icon;
                    aa.tooltip = ipd.tooltip;
                    aa.cost = ipd.cost;
                    aa.costPercentage = ipd.costPercentage;
                    aa.costProperty = ipd.costProperty;
                    aa.pulseCost = ipd.pulseCost;
                    aa.pulseCostPercentage = ipd.pulseCostPercentage;
                    aa.pulseCostProperty = ipd.pulseCostProperty;
                    aa.globalcd = ipd.globalcd;
                    aa.weaponcd = ipd.weaponcd;
                    aa.cooldownType = ipd.cooldownType;
                    aa.cooldownLength = ipd.cooldownLength;
                    aa.reagentReq = ipd.reagentReq;
                    aa.weaponReq = ipd.weaponReq;
                    aa.distance = ipd.distance;
                    aa.castingInRun = ipd.castingInRun;
                    aa.targetType = ipd.targetType;
                    aa.castTime = ipd.castTime;
                    aa.passive = ipd.passive;
                    aa.toggle = ipd.toggle;
                    aa.aoeType = ipd.aoeType;
                    aa.aoeRadius = ipd.aoeRadius;
                    aa.minRange = ipd.minRange;
                    aa.maxRange = ipd.maxRange;
                }
                else
                {
                    AtavismLogger.LogError("ability definiction id=" + aa.id + " is null");
                    return null;
                }
                return aa;

            }
            return null;
        }
        
        public List<AtavismAbility> LoadAllAbilities()
        {
            List<AtavismAbility> list = new List<AtavismAbility>();
            List<int> iconLack = new List<int>();
            foreach (AbilityPrefabData ipd in prefabsdatadata.abilities.Values)
            {
                AtavismAbility aa = new AtavismAbility();
                if (ipd != null)
                {
                    aa.id = ipd.id;
                    aa.name = ipd.name;
                    aa.icon = ipd.icon;
                    aa.tooltip = ipd.tooltip;
                    aa.cost = ipd.cost;
                    aa.costPercentage = ipd.costPercentage;
                    aa.costProperty = ipd.costProperty;
                    aa.pulseCost = ipd.pulseCost;
                    aa.pulseCostPercentage = ipd.pulseCostPercentage;
                    aa.pulseCostProperty = ipd.pulseCostProperty;
                    aa.globalcd = ipd.globalcd;
                    aa.weaponcd = ipd.weaponcd;
                    aa.cooldownType = ipd.cooldownType;
                    aa.cooldownLength = ipd.cooldownLength;
                    aa.reagentReq = ipd.reagentReq;
                    aa.weaponReq = ipd.weaponReq;
                    aa.distance = ipd.distance;
                    aa.castingInRun = ipd.castingInRun;
                    aa.targetType = ipd.targetType;
                    aa.castTime = ipd.castTime;
                    aa.passive = ipd.passive;
                    aa.toggle = ipd.toggle;
                    aa.aoeType = ipd.aoeType;
                    aa.aoeRadius = ipd.aoeRadius;
                    aa.minRange = ipd.minRange;
                    aa.maxRange = ipd.maxRange;
                    list.Add(aa);
                    if (aa.icon == null)
                    {
                        iconLack.Add(aa.id);
                    }
                }
            }
            if (iconLack.Count > 0)
            {

                string s = "";
                foreach (int id in iconLack)
                {
                    s += id + ";";
                }
              //  Debug.LogError(s);
                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("objs", s);
                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "AbilityIcon");
            }
            return list;
        }

        public Skill LoadSkill(int id)
        {
            Skill aa = new Skill();
            if (prefabsdatadata.skills.ContainsKey(id))
            {
                SkillPrefabData ipd = prefabsdatadata.skills[id];
                if (ipd != null)
                {
                    aa.id = ipd.id;
                    aa.skillname = ipd.skillname;
                    aa.icon = ipd.icon;
                    aa.mainAspect = ipd.mainAspect;
                    aa.oppositeAspect = ipd.oppositeAspect;
                    aa.mainAspectOnly = ipd.mainAspectOnly;
                    aa.parentSkill = ipd.parentSkill;
                    aa.parentSkillLevelReq = ipd.parentSkillLevelReq;
                    aa.playerLevelReq = ipd.playerLevelReq;
                    aa.pcost = ipd.pcost;
                    aa.talent = ipd.talent;
                    aa.abilities = ipd.abilities;
                    aa.abilityLevelReqs = ipd.abilityLevelReqs;
                    aa.type = ipd.type;
                }
                else
                {
                    AtavismLogger.LogError("skill definiction id=" + id + " is null");
                    return null;
                }
            }
            else
            {
                if (id > 0)
                {

                    AtavismLogger.LogError("Storage doesn't contain skill definition id=" + id);
                //    LoadSkillsPrefabData();
                }
                return null;
            }
            return aa;
        }

        public List<Skill> LoadAllSkill()
        {
            List<Skill> list = new List<Skill>();
            List<int> iconLack = new List<int>();
            foreach (SkillPrefabData ipd in prefabsdatadata.skills.Values)
            {
                Skill aa = new Skill();
                if (ipd != null)
                {
                    aa.id = ipd.id;
                    aa.skillname = ipd.skillname;
                    aa.icon = ipd.icon;
                    aa.mainAspect = ipd.mainAspect;
                    aa.oppositeAspect = ipd.oppositeAspect;
                    aa.mainAspectOnly = ipd.mainAspectOnly;
                    aa.parentSkill = ipd.parentSkill;
                    aa.parentSkillLevelReq = ipd.parentSkillLevelReq;
                    aa.playerLevelReq = ipd.playerLevelReq;
                    aa.pcost = ipd.pcost;
                    aa.talent = ipd.talent;
                    aa.abilities = ipd.abilities;
                    aa.abilityLevelReqs = ipd.abilityLevelReqs;
                    aa.type = ipd.type;
                    list.Add(aa);
                    if (aa.icon == null)
                    {
                        iconLack.Add(aa.id);
                    }
                }
            }
            if (iconLack.Count > 0)
            {
                string s = "";
                foreach (int id in iconLack)
                {
                    s += id + ";";
                }

                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("objs", s);
                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "SkillIcon");
            }
            return list;
        }


        /// <summary>
        /// Function sending list saved Races to serwer to get update data
        /// </summary>
        public void LoadRaceData()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            int i = 0;
            foreach (RaceData ipd in prefabsdatadata.races.Values)
            {
                //  Debug.LogWarning("LoadItemPrefabData "+ipd.templateId+" "+ipd.name);
                props.Add("iId"+i, ipd.id);
                props.Add("iDate"+i, ipd.date);
                i++;
            }
            props.Add("c", i);
            AtavismClient.Instance.NetworkHelper.GetPrefabs(props, "Race");
        //      Debug.LogError("Race_PREFAB_DATA "+Time.time);
        }
        
        /// <summary>
        /// Function sending list saved Items to serwer to get update data
        /// </summary>
        public void LoadItemPrefabData()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            int i = 0;
            foreach (ItemPrefabData ipd in prefabsdatadata.items.Values)
            {
                //  Debug.LogWarning("LoadItemPrefabData "+ipd.templateId+" "+ipd.name);
                props.Add("iId"+i, ipd.templateId);
                props.Add("iDate"+i, ipd.date);
                i++;
            }
            props.Add("c", i);
            AtavismClient.Instance.NetworkHelper.GetPrefabs(props, "Item");
            //  Debug.LogError("ITEM_PREFAB_DATA "+Time.time);
        }

        /// <summary>
        /// Function sending list saved Crafting recipes to serwer to get update data
        /// </summary>
        public void LoadCraftingRecipePrefabData()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            int i = 0;
            foreach (CraftingRecipePrefabData ipd in prefabsdatadata.craftRecipes.Values)
            {
                props.Add("iId" + i, ipd.recipeID);
                props.Add("iDate" + i, ipd.date);
                i++;
            }
            props.Add("c", i);
            AtavismClient.Instance.NetworkHelper.GetPrefabs(props, "CraftingRecipe");
         //   Debug.LogError("CRAFT_RECIPE_PREFAB_DATA " + Time.time);
        }

        /// <summary>
        /// Function sending list saved Crafting recipes to serwer to get update data
        /// </summary>
        public void LoadBuldingObjectsPrefabData()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            int i = 0;
            foreach (BuildObjPrefabData ipd in prefabsdatadata.buildObjects.Values)
            {
                props.Add("iId" + i, ipd.id);
                props.Add("iDate" + i, ipd.date);
                i++;
            }
            props.Add("c", i);
            AtavismClient.Instance.NetworkHelper.GetPrefabs(props, "BuildingObject");
         //   Debug.LogError("BUILD_OBJ_PREFAB_DATA " + Time.time);
        }

        /// <summary>
        /// Function sending list saved currency to serwer to get update data
        /// </summary>

        public void LoadCurrencyPrefabData()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            int i = 0;
            foreach (CurrencyPrefabData ipd in prefabsdatadata.currencies.Values)
            {
                props.Add("iId" + i, ipd.id);
                props.Add("iDate" + i, ipd.date);
                i++;
            }
            props.Add("c", i);
            AtavismClient.Instance.NetworkHelper.GetPrefabs(props, "Currency");
        //    Debug.LogError("CURR_PREFAB_DATA " + Time.time);
        }
        /// <summary>
        /// Function sending list saved Items set to serwer to get update data
        /// </summary>

        public void LoadItemSetPrefabData()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            int i = 0;
            foreach (ItemSetPrefabData ipd in prefabsdatadata.itemSets.Values)
            {
                props.Add("iId" + i, ipd.Setid);
                props.Add("iDate" + i, ipd.date);
                i++;
            }
            props.Add("c", i);
            AtavismClient.Instance.NetworkHelper.GetPrefabs(props, "ItemSet");
        //    Debug.LogError("ISET_PREFAB_DATA " + Time.time);
        }

        /// <summary>
        /// Function sending list saved skills to serwer to get update data
        /// </summary>

        public void LoadSkillsPrefabData()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            int i = 0;
            foreach (SkillPrefabData ipd in prefabsdatadata.skills.Values)
            {
                props.Add("iId" + i, ipd.id);
                props.Add("iDate" + i, ipd.date);
                i++;
            }
            props.Add("c", i);
            AtavismClient.Instance.NetworkHelper.GetPrefabs(props, "Skill");
        //    Debug.LogError("SKILL_PREFAB_DATA " + Time.time);
        }

        /// <summary>
        /// Function sending list saved abilities to serwer to get update data
        /// </summary>

        public void LoadAbilitiesPrefabData()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            int i = 0;
            foreach (AbilityPrefabData ipd in prefabsdatadata.abilities.Values)
            {
                props.Add("iId" + i, ipd.id);
                props.Add("iDate" + i, ipd.date);
                i++;
            }
            props.Add("c", i);
            AtavismClient.Instance.NetworkHelper.GetPrefabs(props, "Ability");
          //  Debug.LogError("ABILITY_PREFAB_DATA " + Time.time);
        }

        /// <summary>
        /// Function sending list saved effects to serwer to get update data
        /// </summary>

        public void LoadEffectsPrefabData()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            int i = 0;
            foreach (EffectsPrefabData ipd in prefabsdatadata.effects.Values)
            {
                props.Add("iId" + i, ipd.id);
                props.Add("iDate" + i, ipd.date);
                i++;
            }
            props.Add("c", i);
            AtavismClient.Instance.NetworkHelper.GetPrefabs(props, "Effect");
         //   Debug.LogError("EFFECT_PREFAB_DATA " + Time.time);
        }

  public void LoadResourceNodePrefabData()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            int i = 0;
            foreach (RecourceNodeProfileData ipd in prefabsdatadata.recourceNode.Values)
            {
                props.Add("iId" + i, ipd.id);
                props.Add("iDate" + i, ipd.date);
                i++;
            }
            props.Add("c", i);
            AtavismClient.Instance.NetworkHelper.GetPrefabs(props, "ResourceNode");
         //   Debug.LogError("RESOURCE_NODE_PREFAB_DATA " + Time.time);
        }


        public RecourceNodeProfileSettingData LoadResourceNodeData(int profileId, int settingId)
        {
            if (profileId < 1 || settingId < 0)
                return null;
            if (prefabsdatadata.recourceNode.ContainsKey(profileId))
            {
                if (prefabsdatadata.recourceNode[profileId].settingList.ContainsKey(settingId))
                {
                    return prefabsdatadata.recourceNode[profileId].settingList[settingId];
                }
            }
            if (!resourceIconGet.ContainsKey(profileId + "|" + settingId))
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("objs", profileId + "|" + settingId + ";");
                //  Debug.LogWarning("!!!!!!!!!!!!    Get  Item Icon for id " + id);
                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(props, "ResourceNodeIcon");
                resourceIconGet.Add(profileId + "|" + settingId, Time.time);
            }
            else
            {
                if (resourceIconGet[profileId + "|" + settingId] + 2f < Time.time)
                {
                    Dictionary<string, object> props = new Dictionary<string, object>();
                    props.Add("objs", profileId + "|" + settingId + ";");
                    //    Debug.LogWarning("!!!!!!!!!!!!    Get  Item Icon for id " + id);
                    AtavismClient.Instance.NetworkHelper.GetIconPrefabs(props, "ResourceNodeIcon");
                    resourceIconGet[profileId + "|" + settingId]= Time.time;
                }
            }
           
            return null;
        }

        public GlobalEventData LoadGlobaEventsData(int eventId)
        {
            if (prefabsdatadata.globalEvents.ContainsKey(eventId))
            {
                return prefabsdatadata.globalEvents[eventId];
            }
            
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("iId" + 0, eventId);
                props.Add("iDate" + 0, 0L);
                props.Add("c", 1);
                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(props, "GlobalEvents");
             //   Debug.LogError("LoadGlobaEventsData " + Time.time);
                return null;
            
        }

        public void LoadGlobaEventsData()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            int i = 0;
            foreach (GlobalEventData ipd in prefabsdatadata.globalEvents.Values)
            {
                props.Add("iId" + i, ipd.id);
                props.Add("iDate" + i, ipd.date);
                i++;
            }
            props.Add("c", i);
            AtavismClient.Instance.NetworkHelper.GetIconPrefabs(props, "GlobalEvents");
          //  Debug.LogError("LoadGlobaEventsData " + Time.time);
        }
        
     
        public static AtavismPrefabManager Instance
        {
            get
            {
                return instance;
            }
        }

        public void SaveResourceNodeIcon(int id, int settingId, Sprite icons, string icons2, Texture2D iconc, string iconc2, long date)
        {
            if (prefabsdatadata.recourceNode.ContainsKey(id))
            {
                prefabsdatadata.recourceNode[id].date = date;
                if (!prefabsdatadata.recourceNode[id].settingList.ContainsKey(settingId))
                {
                    prefabsdatadata.recourceNode[id].settingList[settingId] = new RecourceNodeProfileSettingData();
                    prefabsdatadata.recourceNode[id].settingList[settingId].id = settingId;
                } 
                prefabsdatadata.recourceNode[id].settingList[settingId].selectedIcon = icons;
                prefabsdatadata.recourceNode[id].settingList[settingId].selectedIconData = icons2;
                prefabsdatadata.recourceNode[id].settingList[settingId].cursorIcon = iconc;
                prefabsdatadata.recourceNode[id].settingList[settingId].cursorIconData = iconc2;
            }
            else
            {
                prefabsdatadata.recourceNode[id] = new RecourceNodeProfileData();
                prefabsdatadata.recourceNode[id].id = id;
                prefabsdatadata.recourceNode[id].date = date;
                prefabsdatadata.recourceNode[id].settingList[settingId] = new RecourceNodeProfileSettingData();
                prefabsdatadata.recourceNode[id].settingList[settingId].id = settingId;
                prefabsdatadata.recourceNode[id].settingList[settingId].selectedIcon = icons;
                prefabsdatadata.recourceNode[id].settingList[settingId].selectedIconData = icons2;
                prefabsdatadata.recourceNode[id].settingList[settingId].cursorIcon = iconc;
                prefabsdatadata.recourceNode[id].settingList[settingId].cursorIconData = iconc2;
            }
        }

        public void SaveGlobalEventIcon(int id, Sprite sprite, string icon2, long date)
        {
            if (prefabsdatadata.globalEvents.ContainsKey(id))
            {
                prefabsdatadata.globalEvents[id].iconData = icon2;
                prefabsdatadata.globalEvents[id].icon = sprite;
                prefabsdatadata.globalEvents[id].id = id;
                prefabsdatadata.globalEvents[id].date = date;
            }
            else
            {
                prefabsdatadata.globalEvents[id] = new GlobalEventData();
                prefabsdatadata.globalEvents[id].iconData = icon2;
                prefabsdatadata.globalEvents[id].icon = sprite;
                prefabsdatadata.globalEvents[id].id = id;
                prefabsdatadata.globalEvents[id].date = date;
            }
        }
    }
}