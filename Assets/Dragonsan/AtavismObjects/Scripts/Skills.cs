using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class Skills : MonoBehaviour
    {
        static Skills instance;

        int currentSkillPoints;
        int totalSkillPoints;
        int currentTalentPoints;
        int totalTalentPoints;
        int skillPointCost;
        Dictionary<int, Skill> playerSkills = new Dictionary<int, Skill>();
        Dictionary<int, Skill> skills = new Dictionary<int, Skill>();
        bool skilldataloaded = false;

        void Start()
        {
            if (instance != null)
            {
                return;
            }
            instance = this;

            // Register for skills message
            NetworkAPI.RegisterExtensionMessageHandler("skills", HandleSkillUpdate);
            NetworkAPI.RegisterExtensionMessageHandler("SkillsPrefabData", HandleSkillsPrefabData);
            NetworkAPI.RegisterExtensionMessageHandler("SkillIcon", HandleSkillIcon);
            AtavismEventSystem.RegisterEvent("LOAD_PREFAB", this);

        }

        void ClientReady()
        {
            AtavismLogger.LogDebugMessage("Skill ClientReady");
            AtavismClient.Instance.NetworkHelper.RegisterPrefabMessageHandler("Skill", HandleSkillsPrefabData);
            AtavismClient.Instance.NetworkHelper.RegisterPrefabMessageHandler("SkillIcon", HandleSkillIcon);
        }

            private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("LOAD_PREFAB", this);

        }
        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "LOAD_PREFAB")
            {
                skills.Clear();
                List<Skill> list = AtavismPrefabManager.Instance.LoadAllSkill();
                foreach (Skill c in list)
                {
                    skills.Add(c.id, c);
                }
            }
        }


        public void ResetSkills(bool talents)
        {
            //NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/skillIncrease " + skillID);
        }

        public void IncreaseSkill(int skillID)
        {
            NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/skillIncrease " + skillID);
        }

        public void DecreaseSkill(int skillID)
        {
            NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/skillDecrease " + skillID);
        }

        public void PurchaseSkillPoint()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "combat.PURCHASE_SKILL_POINT", props);
        }


        public void HandleSkillIcon(Dictionary<string, object> props)
        {
          //  Debug.LogError("HandleSkillIcon " + Time.time);
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

                    AtavismPrefabManager.Instance.SaveSkillIcon(id, sprite, icon2);
                    if (skills.ContainsKey(id))
                    {
                        skills[id].icon = sprite;
                    }
                }
                string[] args = new string[1];
                AtavismEventSystem.DispatchEvent("SKILL_UPDATE", args);
            }
            catch (System.Exception e)
            {
                AtavismLogger.LogError("Exception loading skill icon prefab data " + e);
            }
         //   Debug.LogError("HandleSkillIcon End");
        }


        public void HandleSkillsPrefabData(Dictionary<string, object> props)
        {
          //  Debug.LogError("HandleSkillsPrefabData " + Time.time);
            try
            {
                int num = (int)props["num"];
                bool sendAll = (bool)props["all"];
                for (int i = 0; i < num; i++)
                {
                  //  Debug.LogError("HandleSkillsPrefabData " + i);
                    SkillPrefabData cpd = new SkillPrefabData();
                    cpd.id = (int)props["i" + i + "id"];
                    cpd.skillname = (string)props["i" + i + "name"];
                    cpd.mainAspect = (int)props["i" + i + "mAsp"];
                  //  cpd.type = (int)props["i" + i + "type"];
                    cpd.oppositeAspect = (int)props["i" + i + "opose"];
                    cpd.mainAspectOnly = (bool)props["i" + i + "mAspO"];
                    cpd.parentSkill = (int)props["i" + i + "pskill"];
                    cpd.parentSkillLevelReq = (int)props["i" + i + "pskilllevreq"];
                    cpd.playerLevelReq = (int)props["i" + i + "plylevreq"];
                    cpd.pcost = (int)props["i" + i + "pcost"];
                    cpd.talent = (bool)props["i" + i + "talent"];
                    cpd.type = (int)props["i" + i + "type"];

                    string abi =  (string)props["i" + i + "abilites"];
                    string abiLev = (string)props["i" + i + "abilitesLev"];
                    string[] abi2 = abi.Split(';');
                    string[] abiLev2 = abiLev.Split(';');
                 //   Debug.LogError("HandleSkillsPrefabData "+abi+" | "+abiLev);
                    foreach (string s in abi2)
                    {
                        if(s.Length>0)
                            cpd.abilities.Add(int.Parse(s));
                    }
                    foreach (string s in abiLev2)
                    {
                        if (s.Length > 0)
                            cpd.abilityLevelReqs.Add(int.Parse(s));
                    }
                    cpd.date = (long)props["i" + i + "date"];
                    AtavismPrefabManager.Instance.SaveSkill(cpd);
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
                                AtavismPrefabManager.Instance.DeleteSkill(int.Parse(k));
                            }
                        }
                    }
                }
                if (sendAll)
                {
                    skilldataloaded = true;
                    skills.Clear();
                    List<Skill> list = AtavismPrefabManager.Instance.LoadAllSkill();
                    foreach (Skill c in list)
                    {
                        skills.Add(c.id, c);
                    }
                    foreach (Dictionary<string, object> qProps in skillMsgQueue)
                    {
                        if (AtavismLogger.logLevel <= LogLevel.Debug)
                            Debug.LogError("Running Queued Skill update message");
                        HandleSkillUpdate(qProps);
                    }
                    AtavismPrefabManager.Instance.reloaded++;

                    if(AtavismLogger.logLevel <= LogLevel.Debug) 
                    Debug.Log("All data received. Running Queued Skills update message.");
                }
                else
                {
                    AtavismPrefabManager.Instance.LoadSkillsPrefabData();
                    Debug.LogWarning("Not Running Queued Item update message. Not all data sent");
                }
            }
            catch (System.Exception e)
            {
                AtavismLogger.LogError("Exception loading skill prefab data " + e);
            }
        //    Debug.LogError("HandleSkillsPrefabData End");
        }


        List<Dictionary<string, object>> skillMsgQueue = new List<Dictionary<string, object>>();


        public void HandleSkillUpdate(Dictionary<string, object> props)
        {
          //  Debug.LogError("HandleSkillUpdate skilldataloaded="+skilldataloaded);
            if (!skilldataloaded)
            {
                AtavismPrefabManager.Instance.LoadSkillsPrefabData();
                skillMsgQueue.Add(props);
                Debug.LogWarning("HandleSkillUpdate skilldata not loaded add queue");
                return;
            }
            
             /*       string keys = " [ ";
                    foreach (var it in props.Keys)
                    {
                        keys += " ; " + it + " => " + props[it];
                    }
                    Debug.LogWarning("HandleSkillUpdate: keys:" + keys);
            */
            /*foreach (Skill skill in playerSkills.Values)
            {
                Destroy(skill);
            }*/
           // Debug.LogError("HandleSkillUpdate |");
            playerSkills.Clear();
            currentSkillPoints = (int)props["skillPoints"];
            totalSkillPoints = (int)props["totalSkillPoints"];
            currentTalentPoints = (int)props["talentPoints"];
            totalTalentPoints = (int)props["totalTalentPoints"];
            skillPointCost = (int)props["skillPointCost"];
            int numSkills = (int)props["numSkills"];
          //  Debug.LogError("HandleSkillUpdate ||");
            if (AtavismLogger.logLevel <= LogLevel.Debug)
                AtavismLogger.LogDebugMessage("Got skill update with numSkills: " + numSkills);
            for (int i = 0; i < numSkills; i++)
            {
                //Skill skill = gameObject.AddComponent<Skill> ();
                int skillID = (int)props["skill" + i + "ID"];

                if (!skills.ContainsKey(skillID))
                {
                    UnityEngine.Debug.LogWarning("Skill " + skillID + " does not exist");
                    continue;
                }
                Skill skill = GetSkillByID(skillID).Clone();
                skill.CurrentPoints = (int)props["skill" + i + "Current"];
                skill.CurrentLevel = (int)props["skill" + i + "Level"];
                skill.MaximumLevel = (int)props["skill" + i + "Max"];
                skill.exp = (int)props["skill" + i + "Xp"];
                skill.expMax = (int)props["skill" + i + "XpMax"];
               // Debug.LogError("skillID:" + skillID + " Xp:" + props["skill" + i + "Xp"] + " XpMax:" + props["skill" + i + "XpMax"]+ " CurrentLevel:"+ skill.CurrentLevel+ "  MaximumLevel:" + skill.MaximumLevel+ " CurrentPoints:"+ skill.CurrentPoints);
                playerSkills.Add(skillID, skill);
            }
          //  Debug.LogError("HandleSkillUpdate |||");
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("SKILL_ICON_UPDATE", args);
            if (AtavismLogger.logLevel <= LogLevel.Debug)
                Debug.LogWarning("HandleSkillUpdate end");
        }
        public List<Skill> GetAllKnownCraftSkills()
        {
            List<Skill> _list = new List<Skill>();

            foreach (Skill s in playerSkills.Values)
            {
                if(s.type==0)
                    _list.Add(s);
            }
            return _list;
        }

        public List<int> GetAllKnownCraftSkillsID()
        {
            List<int> _list = new List<int>();
//            Debug.LogError("Ply skills " + playerSkills.Count+" skills="+skills.Count);
            foreach (Skill s in playerSkills.Values)
            {
//                Debug.LogError("Ply skill n=" + s.skillname+" t="+s.type);
                if (s.type == 0)
                    _list.Add(s.id);
            }

            return _list;
        }
        /**
         * Get all crafting type skill ids  
         */
        public List<int> GetAllCraftSkillsID()
        {
            List<int> _list = new List<int>();
            foreach (Skill s in skills.Values)
            {
                if (s.type == 0)
                    _list.Add(s.id);
            }
            return _list;
        }
        public Skill GetSkillByID(int id)
        {
            if (!skills.ContainsKey(id))
            {
                Skill it = AtavismPrefabManager.Instance.LoadSkill(id);
                if (it != null)
                {
                    skills.Add(id, it);
                }
            }
            if (skills.ContainsKey(id))
            {
                return skills[id];
            }
            return null;
        }

        public int GetPlayerSkillLevel(int skillID)
        {
            if (playerSkills.ContainsKey(skillID))
            {
                return playerSkills[skillID].CurrentLevel;
            }
            return 0;
        }
        public int GetPlayerSkillLevel(string name)
        {
            foreach (Skill s in playerSkills.Values)
            {
                if (s.skillname.Equals(name))
                    return s.CurrentLevel;
            }
            return 0;
        }

        public Skill GetSkillOfAbility(int abilityID)
        {
            foreach (Skill skill in skills.Values)
            {
                if (skill.abilities.Contains(abilityID))
                    return skill;
            }
            return null;
        }

        #region Properties
        public static Skills Instance
        {
            get
            {
                return instance;
            }
        }

        public Dictionary<int, Skill> SkillsList
        {
            get
            {
                return skills;
            }
        }

        public Dictionary<int, Skill> PlayerSkills
        {
            get
            {
                return playerSkills;
            }
        }

        public int CurrentSkillPoints
        {
            get
            {
                return currentSkillPoints;
            }
        }

        public int TotalSkillPoints
        {
            get
            {
                return totalSkillPoints;
            }
        }
        public int CurrentTalentPoints
        {
            get
            {
                return currentTalentPoints;
            }
        }

        public int TotalTalentPoints
        {
            get
            {
                return totalTalentPoints;
            }
        }

        public int SkillPointCost
        {
            get
            {
                return skillPointCost;
            }
        }

        #endregion Properties
    }
}