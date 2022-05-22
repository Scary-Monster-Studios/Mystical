using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Globalization;
using System.Threading;
using System.Linq;

namespace Atavism
{

    public class Cooldown
    {
        public string name = "";
        public float length = 0;
        public float expiration;
    }
    [Serializable]
    public class AoeMark
    {
        public int aoeAbilityId = -1;
        public GameObject aoeMaskPrefab;
    }
    [Serializable]
    public class StatThreshold
    {
        public int threshold = 1;
        public int numPerPoint = 1;
    }

    public class Abilities : MonoBehaviour
    {

        static Abilities instance;

        public UGUIAtavismActivatable uguiAtavismAbilityPrefab;
        List<AtavismAbility> playerAbilities = new List<AtavismAbility>();
        Dictionary<int, AtavismAbility> abilities = new Dictionary<int, AtavismAbility>();
        Dictionary<int, AtavismEffect> effects = new Dictionary<int, AtavismEffect>();
        List<AtavismEffect> playerEffects = new List<AtavismEffect>();
        List<Cooldown> cooldowns = new List<Cooldown>();
        List<int> activeToggle = new List<int>();
        bool effectsLoaded = false;
        bool abilityLoaded = false;
        [SerializeField] string rangeStat = "";
        [SerializeField] List<StatThreshold> rangeThresholds = new List<StatThreshold>();
        [SerializeField] string costStat = "";
        [SerializeField] List<StatThreshold> costThresholds = new List<StatThreshold>();
        [SerializeField] string castTimeStat = "";
        [SerializeField] List<StatThreshold> castTimeThresholds = new List<StatThreshold>();
        float rangeMod = 0f;
        void Start()
        {
            if (instance != null)
            {
                return;
            }
            CultureInfo ci = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            instance = this;
            AtavismEventSystem.RegisterEvent("LOAD_PREFAB", this);

        }
        void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if ((scene.name == "Login" || scene.name == ClientAPI.Instance.characterSceneName) )
            {
                cooldowns.Clear();
            }
        }

        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("LOAD_PREFAB", this);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "LOAD_PREFAB")
            {
                abilities.Clear();
                List<AtavismAbility> list = AtavismPrefabManager.Instance.LoadAllAbilities();
                foreach (AtavismAbility c in list)
                {
                    abilities.Add(c.id, c);
                }
            }
        }

        public void StartAbility(int id ,TargetType type)
        {
           // Debug.LogError("StartAbility " + id + " " + ClientAPI.GetTargetObject() + " " + type);
            if (ClientAPI.GetTargetObject() != null && type != TargetType.Self)
                StartCoroutine(ActivateWait(id));
                //NetworkAPI.SendTargetedCommand(ClientAPI.GetTargetOid(), "/ability " + id);
            else
                StartCoroutine(ActivateWaitSelf(id));
            ///NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/ability " + id);

        }

        // <summary>
        /// Coroutine to stop player for start cast
        /// </summary>
        /// <returns></returns>
        IEnumerator ActivateWaitSelf(int id)
        {
            ClientAPI.GetPlayerObject().GameObject.SendMessage("NoMove", 0.05f);
            //Debug.LogError("ActivateWaitSelf " + id + " " + ClientAPI.GetTargetObject() );
            yield return new WaitForSeconds(0.01f);
            int coId = -1;
            int cId = -1;
            if (WorldBuilder.Instance.SelectedClaimObject != null)
            {
                coId = WorldBuilder.Instance.SelectedClaimObject.ID;
                cId = WorldBuilder.Instance.SelectedClaimObject.ClaimID;
            }
            NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/ability " + id+" "+cId+" "+coId);
        }

        IEnumerator ActivateWait(int id)
        {
            ClientAPI.GetPlayerObject().GameObject.SendMessage("NoMove", 0.05f);
           // Debug.LogError("ActivateWait " + id + " " + ClientAPI.GetTargetObject() );
            yield return new WaitForSeconds(0.01f);
            int coId = -1;
            int cId = -1;
            if (WorldBuilder.Instance.SelectedClaimObject != null)
            {
                coId = WorldBuilder.Instance.SelectedClaimObject.ID;
                cId = WorldBuilder.Instance.SelectedClaimObject.ClaimID;
            }
            NetworkAPI.SendTargetedCommand(ClientAPI.GetTargetOid(), "/ability " + id+" "+cId+" "+coId);
        
        }

        public void StartAoeAbility(int id, int minRadius, int maxRadius, Dictionary<string,string> param )
        {
            if(param.ContainsKey("aoeRadius"))
                this.aoeRadius = int.Parse(param["aoeRadius"]);

            this.minRadius = minRadius;
            this.maxRadius = maxRadius;
            if (aoeObject != null)
            {
                Destroy(aoeObject);
            }
            if (id < 1)
            {
            //    Debug.LogError("");
                return;
            }
            AbilityPrefabData apd = AtavismPrefabManager.Instance.GetAbilityPrefab(id);
            if (apd == null)
            {
                return;
            }
            string model = apd.aoePrefab;
            if (model.Contains(".prefab"))
            {
                int resourcePathPos = model.IndexOf("Resources/");
                model = model.Substring(resourcePathPos + 10);
                model = model.Remove(model.Length - 7);
            }
            // Debug.LogError("HandleTrap targetNode=" + targetNode);
            GameObject prefab = (GameObject)Resources.Load(model);
            //   Debug.LogError("HandleTrap prefab=" + prefab);
            if (prefab == null)
            {
                AtavismLogger.LogError("Could not find prefab: "+apd.aoePrefab);
                prefab = (GameObject)Resources.Load("AoeAbilityMarkerPrefab");
            }

            if (prefab != null)
                aoeObject = Instantiate(prefab);
            else
            {
                Debug.LogError("Prefab for Aoe Localization Ability is not assign");
            }
            if (aoeObject != null)
                aoeObject.SetActive(true);
            abilityAoeId = id;
          //  Debug.LogError("rangeStat is "+ rangeStat);
            if (rangeStat.Length > 0)
            {
               // Debug.LogError("rangeStat is " + rangeStat+" name > 0");
                if (ClientAPI.GetPlayerObject() != null)
                {
                    if (ClientAPI.GetPlayerObject().PropertyExists(rangeStat))
                    {
                        int pointsCalculated = 0;
                        int range = (int)ClientAPI.GetPlayerObject().GetProperty(rangeStat);
                        int rangeCalculated = 0;
                        if (rangeThresholds.Count > 0)
                        {
                         //   Debug.LogWarning("Treshold count > 0");
                            for (int i = 0; i < rangeThresholds.Count; i++)
                            {
                                if (range <= rangeThresholds[i].threshold)
                                {
                                    if (range - pointsCalculated < 0)
                                        break;
                                    rangeCalculated += (int)Mathf.Round((range - pointsCalculated) / rangeThresholds[i].numPerPoint);
                                    pointsCalculated += range - pointsCalculated;
                                }
                                else
                                {
                                    rangeCalculated += (int)Mathf.Round((rangeThresholds[i].threshold - pointsCalculated) / rangeThresholds[i].numPerPoint);
                                    pointsCalculated += rangeThresholds[i].threshold - pointsCalculated;
                                }
                            }
                            if (pointsCalculated < range)
                            {
                                rangeCalculated += (int)Mathf.Round((range - pointsCalculated) / rangeThresholds[rangeThresholds.Count - 1].numPerPoint);
                            }
                        }
                        else
                        {
                      //      Debug.LogWarning("Treshold count ==0");
                            pointsCalculated = range;
                        }
                        rangeMod = rangeCalculated / 100f;
                    //    Debug.LogError("rangeStat is " + rangeStat + " value="+ range+ " rangeMod="+ rangeMod+ " pointsCalculated="+ pointsCalculated+ " rangeCalculated "+ rangeCalculated);
                    }
                }
            }
        }

        private int abilityAoeId =- 1;
        private int aoeRadius = 1;
        private int minRadius = 1;
        private int maxRadius = 1;
        public LayerMask layerMask;
        public LayerMask ignoreLayers;
        private GameObject aoeObject;


        private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
        {
            // Convert the object's layer to a bitfield for comparison
            int objLayerMask = (1 << obj.layer);
            if ((layerMask.value & objLayerMask) > 0)  // Extra round brackets required!
                return true;
            else
                return false;
        }
        /// <summary>
        /// get is runnig Aoe Ability
        /// 
        /// </summary>
        public bool isAbilityAoe
        {
            get
            {
                return abilityAoeId > 0;
            }
        }
        
        private void LateUpdate()
        {
            if (abilityAoeId > 0)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 1000, ~ignoreLayers))
                {
                    if (hit.collider.tag != "Player" && hit.collider.tag != "Enemy" && IsInLayerMask(hit.collider.gameObject, layerMask))
                    {

                        if (aoeObject != null)
                        {
                            aoeObject.transform.position = hit.point;
                        }

                        if (ClientAPI.GetPlayerObject() != null && ClientAPI.GetPlayerObject().GameObject != null)
                        {
                            float distance = Vector3.Distance(hit.point, ClientAPI.GetPlayerObject().GameObject.transform.position);
                        //   Debug.LogError("distance " + distance + " minRadius=" + minRadius + " " + (minRadius - (minRadius * rangeMod))
                        //        + " maxRadius "+ maxRadius+" "+ (maxRadius + (maxRadius * rangeMod))+ " rangeMod "+ rangeMod);
                            if (distance > minRadius - (minRadius * rangeMod) && distance < maxRadius+ (maxRadius * rangeMod))
                            {
                                aoeObject.GetComponent<AoeAbilityObject>().SetPositive();
                            //    Debug.LogError("Positive");
                            }
                            else
                            {
                            //    Debug.LogError("Negative");
                                aoeObject.GetComponent<AoeAbilityObject>().SetNegative();
                            }
                        }


                        if (Input.GetMouseButtonDown(0))
                        {
                            NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/abilityloc " + abilityAoeId + " " + hit.point.x + " " + hit.point.y + " " + hit.point.z);
                            abilityAoeId = -1;
                            if (aoeObject != null)
                                aoeObject.SetActive(false);
                        }
                        //hit.point;

                    }
                    else
                    {

                    }
                
                }
                else
                {
                    
                }
                if (Input.GetMouseButtonDown(1))
                {
                    abilityAoeId = -1;
                    if (aoeObject != null)
                        aoeObject.SetActive(false);
                }
            }
        }


        void ClientReady()
        {
            AtavismLogger.LogDebugMessage("Ability ClientReady");
            AtavismClient.Instance.NetworkHelper.RegisterPrefabMessageHandler("Ability", AbilitiesPrefabHandler);
            AtavismClient.Instance.NetworkHelper.RegisterPrefabMessageHandler("AbilityIcon", HandleAbilityIcon);
            AtavismClient.Instance.NetworkHelper.RegisterPrefabMessageHandler("Effect", EffectsPrefabHandler);
            AtavismClient.Instance.NetworkHelper.RegisterPrefabMessageHandler("EffectIcon", HandleEffectIcon);

          // Register for abilities property
            ClientAPI.WorldManager.RegisterObjectPropertyChangeHandler("abilities", AbilitiesPropertyHandler);
            ClientAPI.WorldManager.RegisterObjectPropertyChangeHandler("effects", EffectsPropertyHandler);

            NetworkAPI.RegisterExtensionMessageHandler("statRange", HandleStatRange);
            NetworkAPI.RegisterExtensionMessageHandler("statCost", HandleStatCost);
            NetworkAPI.RegisterExtensionMessageHandler("statCastTime", HandleStatCastTime);
            NetworkAPI.RegisterExtensionMessageHandler("Trap", HandleTrap);

            NetworkAPI.RegisterExtensionMessageHandler("cooldown", HandleCooldown);
            NetworkAPI.RegisterExtensionMessageHandler("activeToggle", HandleToggle);


        }

        public void HandleStatRange(Dictionary<string, object> props)
        {
            AtavismLogger.LogDebugMessage("HandleStatRange " + Time.time);
            string stat = (string)props["stat"];
            rangeStat = stat;
            int num = (int)props["num"];
            AtavismLogger.LogDebugMessage("HandleStatRange stat= " + stat+" num= "+num);
            rangeThresholds.Clear();
            for (int i = 0; i < num; i++)
            {
                int t = (int)(int)props["def" + i + "T"];
                int p = (int)(int)props["def" + i + "P"];
                StatThreshold st = new StatThreshold();
                st.threshold = t;
                st.numPerPoint = p;
                rangeThresholds.Add(st);
            }
            AtavismLogger.LogDebugMessage("HandleStatRange End " + Time.time);

        }

        public void HandleStatCost(Dictionary<string, object> props)
        {
            AtavismLogger.LogDebugMessage("HandleStatCost Start" );
            string stat = (string)props["stat"];
            costStat = stat;
            int num = (int)props["num"];
            AtavismLogger.LogDebugMessage("HandleStatCost stat= " + stat+" num= "+num);
            costThresholds.Clear();
            for (int i = 0; i < num; i++)
            {
                int t = (int)(int)props["def" + i + "T"];
                int p = (int)(int)props["def" + i + "P"];
                StatThreshold st = new StatThreshold();
                st.threshold = t;
                st.numPerPoint = p;
                costThresholds.Add(st);
            }
            AtavismLogger.LogDebugMessage("HandleStatCost End " + Time.time);

        }

        public void HandleStatCastTime(Dictionary<string, object> props)
        {
            AtavismLogger.LogDebugMessage("HandleStatRange " + Time.time);
            string stat = (string)props["stat"];
            castTimeStat = stat;
            int num = (int)props["num"];
            AtavismLogger.LogDebugMessage("HandleStatRange stat= " + stat+" num= "+num);
           castTimeThresholds.Clear();
            for (int i = 0; i < num; i++)
            {
                int t = (int)(int)props["def" + i + "T"];
                int p = (int)(int)props["def" + i + "P"];
                StatThreshold st = new StatThreshold();
                st.threshold = t;
                st.numPerPoint = p;
                castTimeThresholds.Add(st);
            }
            AtavismLogger.LogDebugMessage("HandleStatRange End " + Time.time);

        }

        public void HandleTrap(Dictionary<string, object> props)
        {
             //  Debug.LogError("HandleTrap " + Time.time);
    
            StartCoroutine(MakeTrap(props));
         //   Debug.LogError("HandleTrap End");
        }

        IEnumerator MakeTrap(Dictionary<string, object> _props)
        {
            WaitForSeconds delay = new WaitForSeconds(0.5f);
            bool created = false;
            while (!created)
            {
                yield return delay;
                //   Debug.LogError("HandleTrap " + Time.time);
                try
                {
                    // int num = (int)props["num"];
                    //bool sendAll = (bool)props["all"];
                    string TName = (string)_props["name"];
                    OID trapOid = (OID)_props["oid"];
                    Vector3 pos = (Vector3)_props["point"];
                  //  Debug.LogError("HandleTrap " + TName + " " + trapOid);
                    AtavismObjectNode targetNode = AtavismClient.Instance.WorldManager.GetObjectNode(trapOid);
                    if (targetNode != null && targetNode.createdGo)
                    {
                        string model = (string)_props["model"];
                        if (model.Contains(".prefab"))
                        {
                            int resourcePathPos = model.IndexOf("Resources/");
                            model = model.Substring(resourcePathPos + 10);
                            model = model.Remove(model.Length - 7);
                        }
                            // Debug.LogError("HandleTrap targetNode=" + targetNode);
                        GameObject prefab = (GameObject)Resources.Load(model);
                        //   Debug.LogError("HandleTrap prefab=" + prefab);
                        if (prefab == null)
                        {
                            AtavismLogger.LogError("Could not find prefab: Trap model "+ model);
                            prefab = (GameObject)Resources.Load("ExampleCharacter");
                        }
                        GameObject go = (GameObject)UnityEngine.Object.Instantiate(prefab, pos, Quaternion.identity);
                        go.transform.parent = targetNode.GameObject.transform;
                        // go.transform.position = Vector3.zero;
                        created = true;
                    }
                }
                catch (System.Exception e)
                {
                    AtavismLogger.LogError("Exception loading HandleTrap data " + e);
                }
            }
         //   Debug.LogError("HandleTrap End");
        }

        public AtavismAbility GetAbility(int abilityID)
        {
            // First check if the player has a copy of this ability
            AtavismAbility ability = GetPlayerAbility(abilityID);
            if (ability == null)
            {
                // Player does not have this ability - lets use the template
                if (!abilities.ContainsKey(abilityID))
                {
                    AtavismAbility it = AtavismPrefabManager.Instance.LoadAbility(abilityID);
                    if (it != null)
                    {
                        abilities.Add(abilityID, it);
                    }
                }
                if (abilities.ContainsKey(abilityID))
                    return abilities[abilityID].Clone();
            }
            return ability;
        }

        public AtavismAbility GetPlayerAbility(int abilityID)
        {
            AtavismAbility ability = null;
            foreach (AtavismAbility ab in playerAbilities)
            {
                if (ab.id == abilityID)
                {
                    return ab;
                }
            }
            return ability;
        }

        public AtavismEffect GetEffect(int effectID)
        {
            // Player does not have this ability - lets use the template
            if (!effects.ContainsKey(effectID))
            {
                AtavismEffect it = AtavismPrefabManager.Instance.LoadEffect(effectID);
                if (it != null)
                {
                    effects.Add(effectID, it);
                }
            }
            if (effects.ContainsKey(effectID))
                return effects[effectID];
            return null;
        }

        public AtavismEffect GetPlayerEffect(int effectID)
        {
            foreach (AtavismEffect ab in playerEffects)
            {
                if (ab.id == effectID)
                {
                    return ab;
                }
            }
            return null;
        }

        public void RemoveBuff(AtavismEffect effect, int pos)
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("effectID", effect.id);
            props.Add("pos", pos);
            props.Add("id", effect.stateid);
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.REMOVE_BUFF", props);
        }

        public List<AtavismEffect> GetTargetEffects()
        {
            List<AtavismEffect> targetEffects = new List<AtavismEffect>();
            LinkedList<object> effects_prop = (LinkedList<object>)ClientAPI.GetTargetObject().GetProperty("effects");
            AtavismLogger.LogDebugMessage("Got target effects property change: " + effects_prop);
            //	int pos = 0;
            foreach (string effectsProp in effects_prop)
            {
                string[] effectData = effectsProp.Split(',');
                int effectID = int.Parse(effectData[0]);
                long timeUntilEnd = long.Parse(effectData[3]);
                bool active = bool.Parse(effectData[4]);
                long duration = long.Parse(effectData[5]);
                AtavismLogger.LogInfoMessage("Got effect " + effectID + " active? " + active);

                float secondsLeft = (float)timeUntilEnd / 1000f;
                if (!effects.ContainsKey(effectID))
                {
                    GetEffect(effectID);
                }
                if (!effects.ContainsKey(effectID))
                {
                    AtavismLogger.LogWarning("Effect " + effectID + " does not exist");
                    continue;
                }
                AtavismEffect effect = effects[effectID].Clone();
                effect.Active = active;
                effect.Expiration = Time.time + secondsLeft;
                effect.Length = duration / 1000f;
                targetEffects.Add(effect);
            }
            return targetEffects;
        }

        /// <summary>
        /// Currently not used by the default atavism system.
        /// </summary>
        /// <returns>The cooldown expiration.</returns>
        /// <param name="cooldownName">Cooldown name.</param>
        /// <param name="globalCooldown">If set to <c>true</c> global cooldown.</param>
        public float GetCooldownExpiration(string cooldownName, bool globalCooldown)
        {
            foreach (Cooldown cooldown in cooldowns)
            {
                if (cooldown.name == cooldownName)
                {
                    return cooldown.expiration;
                }
            }
            if (globalCooldown)
            {
                foreach (Cooldown cooldown in cooldowns)
                {
                    if (cooldown.name == "GLOBAL")
                    {
                        return cooldown.expiration;
                    }
                }
            }
            return -1;
        }

        public Cooldown GetCooldown(string cooldownName, bool globalCooldown)
        {
            Cooldown c = null;
            foreach (Cooldown cooldown in cooldowns)
            {
                if (cooldown.name == cooldownName)
                {
                    c = cooldown;
                    break;
                }
            }
            if (globalCooldown)
            {
                foreach (Cooldown cooldown in cooldowns)
                {
                    if (cooldown.name == "GLOBAL" && (c == null || c.expiration < cooldown.expiration))
                    {
                        c = cooldown;
                        break;
                    }
                }
            }
            return c;
        }

        public void AbilitiesPropertyHandler(object sender, ObjectPropertyChangeEventArgs args)
        {
            if (!abilityLoaded)
                return;
            if (args.Oid != ClientAPI.GetPlayerOid())
                return;
            checkPlayerAbility();
        }
        void checkPlayerAbility()
        {
            if (ClientAPI.GetPlayerObject() == null)
                return;
            List<int> iconLack = new List<int>();
            List<object> abilities_prop = (List<object>)ClientAPI.GetPlayerObject().GetProperty("abilities");
            AtavismLogger.LogDebugMessage("Got player abilities property change: " + abilities_prop);
            playerAbilities.Clear();
            //int pos = 0;
            foreach (int abilityNum in abilities_prop)
            {
                if (!abilities.ContainsKey(abilityNum))
                {
                    AtavismLogger.LogWarning("Ability " + abilityNum + " does not exist");
                    continue;
                }
                AtavismAbility ability = abilities[abilityNum].Clone();
                playerAbilities.Add(ability);
                if (ability.icon == null)
                {
                    iconLack.Add(ability.id);
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
                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "AbilityIcon");

               // NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.GET_ABILITY_ICON", ps);
            }
            // dispatch a ui event to tell the rest of the system
            string[] event_args = new string[1];
            AtavismEventSystem.DispatchEvent("ABILITY_UPDATE", event_args);
        }

        public void HandleAbilityIcon(Dictionary<string, object> props)
        {
          //  Debug.LogError("HandleAbilityIcon " + Time.time);
            try
            {
                int num = (int)props["num"];
                bool sendAll = (bool)props["all"];
                for (int i = 0; i < num; i++)
                {
                    int id = (int)props["i" + i + "id"];
                    string icon = (string)props["i" + i + "icon"];
                    string icon2 = (string)props["i" + i + "icon2"];
                    Texture2D tex = new Texture2D(2, 2);
                    bool wyn = tex.LoadImage(System.Convert.FromBase64String(icon2));
                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                   // Debug.LogWarning("HandleAbilityIcon for id=" + id);
                    AtavismPrefabManager.Instance.SaveAbilityIcon(id, sprite, icon2);
                    if (abilities.ContainsKey(id))
                    {
                        abilities[id].icon = sprite;
                    }
                    foreach (AtavismAbility aa in playerAbilities)
                    {
                        if (aa.id == id)
                            aa.icon = sprite;
                    }
                }
                string[] args = new string[1];
                AtavismEventSystem.DispatchEvent("ABILITY_UPDATE", args);
            }
            catch (System.Exception e)
            {
                AtavismLogger.LogError("Exception loading ability icon prefab data " + e);
            }
         //   Debug.LogError("HandleAbilityIcon End");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="props"></param>
        public void AbilitiesPrefabHandler(Dictionary<string, object> props)
        {
            //   Debug.LogError("AbilitiesPrefabHandler " + Time.time);
            /*    string keys = " [ ";
                 foreach (var it in props.Keys)
                 {
                     keys += " ; " + it+" => "+props[it];
                 }
                 Debug.LogWarning("AbilitiesPrefabHandler: keys:" + keys);*/
        //    Debug.LogError("Abilty . " + (float.Parse("5.0")) );
        //    Debug.LogError("Ability , " + (float.Parse("5,0")));
            try
            {
                int num = (int)props["num"];
                bool sendAll = (bool)props["all"];
                for (int i = 0; i < num; i++)
                {
                 //   Debug.LogError("AbilitiesPrefabHandler " + i); 
                    AbilityPrefabData cpd = new AbilityPrefabData();
                    cpd.id = (int)props["i" + i + "id"];
                    cpd.name = (string)props["i" + i + "name"];
                    cpd.tooltip = (string)props["i" + i + "tooltip"];
                    cpd.cost = (int)props["i" + i + "cost"];
                    cpd.costPercentage = ((int)props["i" + i + "costP"])/1000f;
                    cpd.costProperty = (string)props["i" + i + "costProp"];
                    cpd.pulseCost = (int)props["i" + i + "pcost"];
                    cpd.pulseCostPercentage = ((int)props["i" + i + "pcostP"])/1000f;
                    cpd.pulseCostProperty = (string)props["i" + i + "pcostProp"];
                    cpd.globalcd = (bool)props["i" + i + "globancd"];
                    cpd.weaponcd = (bool)props["i" + i + "weaponcd"];
                    cpd.cooldownType = (string)props["i" + i + "cooldT"];
                //    Debug.LogError("Ability  " + props["i" + i + "cooldL"] +" " +props["i" + i + "cooldL"].GetType());
                    cpd.cooldownLength = ((int)props["i" + i + "cooldL"])/1000f;
                
                   cpd.weaponReq = (string)props["i" + i + "reqWeap"];
                    cpd.reagentReq = (int)props["i" + i + "reqReag"];
                    cpd.distance = (int)props["i" + i + "dist"];
                    cpd.castingInRun = (bool)props["i" + i + "castInRun"];
                    string type = (string)props["i" + i + "targetType"];
                   // Debug.Log(cpd.name + " type " + type);
                    if (type == "AoE Enemy")
                        cpd.targetType = TargetType.AoE_Enemy;
                    else if (type == "AoE Friendly")
                        cpd.targetType = TargetType.AoE_Friendly;
                    else if (type == "Group")
                        cpd.targetType = TargetType.Group;
                    else if (type == "Self Location")
                        cpd.targetType = TargetType.Self_Location;
                    else if (type == "Enemy")
                        cpd.targetType = TargetType.Enemy;
                    else if (type == "Self")
                        cpd.targetType = TargetType.Self;
                    else if (type == "Friendly")
                        cpd.targetType = TargetType.Friendly;
                    else if (type.Contains("none"))
                        cpd.targetType = TargetType.All;

                   
                    cpd.castTime = ((int)props["i" + i + "castTime"])/1000f;
                    cpd.passive = (bool)props["i" + i + "passive"];
                    cpd.toggle = (bool)props["i" + i + "toggle"];
                    cpd.aoeType = (string)props["i" + i + "aoeType"];
                    cpd.aoePrefab = (string)props["i" + i + "aoePrefab"];
                    cpd.aoeRadius = (int)props["i" + i + "aoeRadius"];
                    cpd.minRange = (int)props["i" + i + "minRange"];
                    cpd.maxRange = (int)props["i" + i + "maxRange"];
                    cpd.date = (long)props["i" + i + "date"];
                 //   Debug.LogError("Abilities "+cpd.name+" castTime:"+cpd.castTime+" cooldown:"+cpd.cooldownLength+" | "+ props["i" + i + "castTime"]+" "+ props["i" + i + "cooldL"]);
                    AtavismPrefabManager.Instance.SaveAbility(cpd);
                }
                if (props.ContainsKey("toRemove"))
                {
                    string keys = (string)props["toRemove"];
                    if (keys.Length > 0)
                    {
                        string[] _keys = keys.Split(';');
                        foreach (string k in _keys)
                        {
                            if (k.Length > 0)
                            {
                                AtavismPrefabManager.Instance.DeleteAbility(int.Parse(k));
                            }
                        }
                    }
                }

                if (sendAll)
                {
                    abilityLoaded = true;

                    abilities.Clear();
                    List<AtavismAbility> abi = AtavismPrefabManager.Instance.LoadAllAbilities();
                    foreach (AtavismAbility c in abi)
                    {
                        abilities.Add(c.id, c);
                    }

                    checkPlayerAbility();
                    string[] args = new string[1];
                    AtavismEventSystem.DispatchEvent("ABILITY_UPDATE", args);
                    AtavismPrefabManager.Instance.reloaded++;

                    if(AtavismLogger.logLevel <= LogLevel.Debug) 
                    Debug.Log("All data received. Running Queued ability update message.");
                }
                else
                {
                    AtavismPrefabManager.Instance.LoadAbilitiesPrefabData();
                    Debug.LogWarning("Not Running Queued Item update message. Not all data sent");
                }
            }
            catch (System.Exception e)
            {
                AtavismLogger.LogError("Exception loading ability prefab data " + e);
            }
         //   Debug.LogError("AbilitiesPrefabHandler End");
        }

        public void HandleEffectIcon(Dictionary<string, object> props)
        {
         //   Debug.LogError("HandleEffectIcon " + Time.time);
            try
            {
                int num = (int)props["num"];
                bool sendAll = (bool)props["all"];
                for (int i = 0; i < num; i++)
                {
                    int id = (int)props["i" + i + "id"];
                    string icon = (string)props["i" + i + "icon"];
                    string icon2 = (string)props["i" + i + "icon2"];
                    Texture2D tex = new Texture2D(2, 2);
                    bool wyn = tex.LoadImage(System.Convert.FromBase64String(icon2));
                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

                    AtavismPrefabManager.Instance.SaveEffectsIcon(id, sprite, icon2);
                    if (effects.ContainsKey(id))
                    {
                        effects[id].icon = sprite;
                    }
                    foreach (AtavismEffect ae in playerEffects)
                    {
                        if (ae.id == id)
                        {
                            ae.icon = sprite;
                        }
                    }
                }
                string[] event_args = new string[1];
                AtavismEventSystem.DispatchEvent("EFFECT_ICON_UPDATE", event_args);
                //  CheckPlayerEffects();
            }
            catch (System.Exception e)
            {
                AtavismLogger.LogError("Exception loading effect icon prefab data " + e);
            }
          //  Debug.LogError("HandleEffectIcon End");
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="props"></param>
        public void EffectsPrefabHandler(Dictionary<string, object> props)
        {
          //  Debug.LogError("EffectsPrefabHandler " + Time.time);
            try
            {
                int num = (int)props["num"];
                bool sendAll = (bool)props["all"];
                for (int i = 0; i < num; i++)
                {
                    EffectsPrefabData cpd = new EffectsPrefabData();
                    cpd.id = (int)props["i" + i + "id"];
                    cpd.name = (string)props["i" + i + "name"];
                    cpd.tooltip = (string)props["i" + i + "tooltip"];
                    cpd.isBuff = (bool)props["i" + i + "buff"];
                    cpd.stackLimit = (int)props["i" + i + "stackL"];
                    cpd.stackTime = (bool)props["i" + i + "stackT"];
                    cpd.allowMultiple = (bool)props["i" + i + "aM"];
                    cpd.date = (long)props["i" + i + "date"];
                    AtavismPrefabManager.Instance.SaveEffects(cpd);
                    //   Debug.LogError("EffectsPrefabHandler " + cpd.id);

                }
                if (props.ContainsKey("toRemove"))
                {
                    string keys = (string)props["toRemove"];
                    if (keys.Length > 0)
                    {
                        string[] _keys = keys.Split(';');
                        foreach (string k in _keys)
                        {
                            if (k.Length > 0)
                            {
                                AtavismPrefabManager.Instance.DeleteEffect(int.Parse(k));
                            }
                        }
                    }
                }

                if (sendAll)
                {
                    effectsLoaded = true;
                    CheckPlayerEffects();
                    AtavismPrefabManager.Instance.reloaded++;
                    if(AtavismLogger.logLevel <= LogLevel.Debug) 

                    Debug.Log("All data received. Running Queued Effects update message.");
                }
                else
                {
                    AtavismPrefabManager.Instance.LoadEffectsPrefabData();
                    Debug.LogWarning("Not Running Queued Item update message. Not all data sent");
                }
            }
            catch (System.Exception e)
            {
                AtavismLogger.LogError("Exception loading effects prefab data " + e);
            }
         //   Debug.LogError("EffectsPrefabHandler End");
        }


        public void EffectsPropertyHandler(object sender, ObjectPropertyChangeEventArgs args)
        {
            AtavismLogger.LogWarning("EffectsPropertyHandler Start");

            if (!effectsLoaded)
                return;
            if (args.Oid != ClientAPI.GetPlayerOid())
                return;
            CheckPlayerEffects();
            AtavismLogger.LogWarning("EffectsPropertyHandler End");
        }
        void CheckPlayerEffects()
        {
            AtavismLogger.LogWarning("CheckPlayerEffects Start");
            if (ClientAPI.GetPlayerObject() == null)
                return;
            //ClientAPI.Write("Got effect update at time: " + Time.realtimeSinceStartup);
            LinkedList<object> effects_prop = (LinkedList<object>)ClientAPI.GetPlayerObject().GetProperty("effects");
            AtavismLogger.LogDebugMessage("Got player effects property change: " + effects_prop);
            playerEffects.Clear();
            List<int> iconLack = new List<int>();
            foreach (string effectsProp in effects_prop)
            {
                AtavismLogger.LogWarning("Effect: " + effectsProp);
                string[] effectData = effectsProp.Split(',');
                int effectID = int.Parse(effectData[0]);
                long timeUntilEnd = long.Parse(effectData[4]);
                bool active = bool.Parse(effectData[5]);
                long duration = long.Parse(effectData[6]);

                AtavismLogger.LogInfoMessage("Got effect " + effectID + " active? " + active);
                //if (timeTillEnd < duration)
                //	duration = timeTillEnd;

                float secondsLeft = (float)timeUntilEnd / 1000f;
                if (!effects.ContainsKey(effectID))
                {
                    GetEffect(effectID);
                }
                if (!effects.ContainsKey(effectID))
                {
                    AtavismLogger.LogWarning("Effect " + effectID + " does not exist");
                    continue;
                }
                AtavismLogger.LogWarning("Effect " + effectID + " get data secondsLeft="+ secondsLeft);
                AtavismEffect effect = effects[effectID].Clone();
                effect.StackSize = int.Parse(effectData[1]);
                effect.isBuff = bool.Parse(effectData[2]);
                effect.Active = active;
                effect.Expiration = Time.time + secondsLeft;
                effect.Length = (float)duration / 1000f;
                effect.startTime = long.Parse(effectData[9]);
                effect.stateid = long.Parse(effectData[10]);
                playerEffects.Add(effect);
                if (effect.icon == null)
                {
                    iconLack.Add(effect.id);
                }
            }
            playerEffects = playerEffects.OrderBy(x => x.startTime).ToList();

            if (iconLack.Count > 0)
            {
                string s = "";
                foreach (int id in iconLack)
                {
                    s += id + ";";
                }

                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("objs", s);
                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "EffectIcon");
                //NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.GET_EFFECT_ICON", ps);
            }
            // dispatch a ui event to tell the rest of the system
            string[] event_args = new string[1];
            AtavismEventSystem.DispatchEvent("EFFECT_UPDATE", event_args);
            AtavismLogger.LogWarning("CheckPlayerEffects End");

        }

        public void HandleCooldown(Dictionary<string, object> props)
        {
            Cooldown old = GetCooldown((string)props["CdType"], false);
            if (old != null)
                cooldowns.Remove(old);
            Cooldown cooldown = new Cooldown();
            cooldown.name = (string)props["CdType"];
            cooldown.length = (long)props["CdLength"] / 1000f;
            float expire = (long)props["CdStart"] / 1000f;
            cooldown.expiration = Time.time + expire;
            cooldowns.Add(cooldown);
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("COOLDOWN_UPDATE", args);
            //ClientAPI.Write("Got cooldown: " + cooldown.name + " with length: " + cooldown.length);
        }
        public void HandleToggle(Dictionary<string, object> props)
        {
            activeToggle.Clear();
            int num = (int)props["num"];
            for (int i = 0; i < num; i++)
            {
                activeToggle.Add((int)props["a" + i ]);
            }
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("ATOGGLE_UPDATE", args);
            //ClientAPI.Write("Got cooldown: " + cooldown.name + " with length: " + cooldown.length);
        }
        #region Properties
        public static Abilities Instance
        {
            get
            {
                return instance;
            }
        }

        public List<AtavismAbility> PlayerAbilities
        {
            get
            {
                return playerAbilities;
            }
        }

        public List<AtavismEffect> PlayerEffects
        {
            get
            {
                return playerEffects;
            }
        }

        public bool AbilityLoaded
        {
            get
            {
                return abilityLoaded;
            }
        }

        public bool isToggleActive(int id)
        {
            return activeToggle.Contains(id);
        }

        #endregion Properties
    }
}