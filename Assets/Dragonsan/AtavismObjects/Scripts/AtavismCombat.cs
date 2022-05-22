using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Atavism
{
    public class AtavismCombat : MonoBehaviour
    {
        [SerializeField]  string healthStat = "health";
        [SerializeField]  string healthMaxStat = "health-max";
      /*  [SerializeField]  string manaStat = "mana";
        [SerializeField]  string manaMaxStat = "mana-max";*/
       // [SerializeField] static string healthStat = "health";
        [SerializeField] string CriticAnimTrigerProperty = "Critic";
        [SerializeField] string EvadedAnimTrigerProperty = "Evaded";
        [SerializeField] string DodgedAnimTrigerProperty = "Dodged";
        [SerializeField] string BlockedAnimTrigerProperty = "Blocked"; 
        [SerializeField] string ParriedAnimTrigerProperty = "Parried"; 
        // Use this for initialization
        private static AtavismCombat instance;
        void Start()
        {
            if (instance != null)
            {
                return;
            }
            instance = this;
           
        }

        void ClientReady()
        {
            NetworkAPI.RegisterExtensionMessageHandler("combat_event", HandleCombatEvent);
            NetworkAPI.RegisterExtensionMessageHandler("combat_text", HandleCombatText);
            NetworkAPI.RegisterExtensionMessageHandler("Duel_Challenge", HandleDuelChallenge);
            NetworkAPI.RegisterExtensionMessageHandler("Duel_Challenge_End", HandleDuelChallengeEnd);

            AtavismClient.Instance.WorldManager.RegisterObjectPropertyChangeHandler("state", HandleState);
        }

        private void HandleCombatText(Dictionary<string, object> props)
        {
           
        }

        private void OnDestroy()
        {
            AtavismClient.Instance.WorldManager.RemoveObjectPropertyChangeHandler("state", HandleState);

        }
        public void HandleCombatEvent(Dictionary<string, object> props)
        {
            string eventType = (string)props["event"];
            OID caster = (OID)props["caster"];
            OID target = (OID)props["target"];
            int abilityID = (int)props["abilityID"];
            int effectID = (int)props["effectID"];
            string value1 = "" + (int)props["value1"];
            string value2 = "" + (int)props["value2"];
            string value3 = (string)props["value3"];
            string value4 = (string)props["value4"];
           // Debug.LogWarning("Got Combat Event " + eventType);
            //Debug.LogError("HandleCombatEvent " + caster + " | " + target + " | " + abilityID + " | " + effectID + " | " + value1 + " | " + value2 + " | " + value3 + " | " + value4 + " | " + eventType);
            //Automatical select attacer
            try
            {
                if (target!=null && target.ToLong() == ClientAPI.GetPlayerOid() && target != caster &&
                    (ClientAPI.GetTargetObject() == null || (ClientAPI.GetTargetObject() != null && ClientAPI.GetTargetObject().PropertyExists(healthStat) && (int)ClientAPI.GetTargetObject().GetProperty(healthStat) == 0)))
                {
                    ClientAPI.SetTarget(caster.ToLong());

                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Exception e="+e);
            }
           // ClientAPI.Write("Got Combat Event: " + eventType);
            //   int messageType = 2;
      
            if (eventType == "CombatDamage")
            {
                //		messageType = 1;
            }
           /* else if (eventType == "CombatMagicalDamage")
            {

            }*/
            else if (eventType == "CombatDamageCritical")
            {
                if(target!=null)
                     ClientAPI.GetObjectNode(target.ToLong()).MobController.PlayAnimationTrigger(CriticAnimTrigerProperty);

                //		messageType = 1;
            }
           /* else if (eventType == "CombatMagicalCritical")
            {
                if(target!=null)
                ClientAPI.GetObjectNode(target.ToLong()).MobController.PlayAnimationTrigger(CriticAnimTrigerProperty);
                //		messageType = 1;
            }*/
            else if (eventType == "CombatMissed")
            {
                if(target!=null)
                ClientAPI.GetObjectNode(target.ToLong()).MobController.PlayAnimationTrigger(EvadedAnimTrigerProperty);

#if AT_I2LOC_PRESET
                        if (target.ToLong() == ClientAPI.GetPlayerOid())
                value1 = I2.Loc.LocalizationManager.GetTranslation("MissedSelf");
            else
                value1 = I2.Loc.LocalizationManager.GetTranslation("Missed");
#else
                value1 = "Missed";
#endif
            }
            else if (eventType == "CombatDodged")
            {
                if(target!=null)
                ClientAPI.GetObjectNode(target.ToLong()).MobController.PlayAnimationTrigger(DodgedAnimTrigerProperty);

#if AT_I2LOC_PRESET
              if (target.ToLong() == ClientAPI.GetPlayerOid())
                value1 = I2.Loc.LocalizationManager.GetTranslation("DodgedSelf"); 
            else
                value1 = I2.Loc.LocalizationManager.GetTranslation("Dodged");
#else
                value1 = "Dodged";
#endif
            }
            else if (eventType == "CombatBlocked")
            {
                if(target!=null)
                ClientAPI.GetObjectNode(target.ToLong()).MobController.PlayAnimationTrigger(BlockedAnimTrigerProperty);
#if AT_I2LOC_PRESET
             if (target.ToLong() == ClientAPI.GetPlayerOid())
                value1 = I2.Loc.LocalizationManager.GetTranslation("BlockedSelf");
            else
                value1 = I2.Loc.LocalizationManager.GetTranslation("Blocked");

#else
                value1 = "Blocked";
#endif
            }
            else if (eventType == "CombatParried")
            {
                if(target!=null)
                ClientAPI.GetObjectNode(target.ToLong()).MobController.PlayAnimationTrigger(ParriedAnimTrigerProperty);
                #if AT_I2LOC_PRESET
            if (target.ToLong() == ClientAPI.GetPlayerOid())
                value1 = I2.Loc.LocalizationManager.GetTranslation("ParriedSelf");  
            else
                value1 = I2.Loc.LocalizationManager.GetTranslation("Parried");

#else
                    value1 = "Parried";
#endif
            }
            else if (eventType == "CombatEvaded")
            {
                if(target!=null)
                ClientAPI.GetObjectNode(target.ToLong()).MobController.PlayAnimationTrigger(EvadedAnimTrigerProperty);

#if AT_I2LOC_PRESET
              if (target.ToLong() == ClientAPI.GetPlayerOid())
                value1 = I2.Loc.LocalizationManager.GetTranslation("EvadedSelf");  
            else
                value1 = I2.Loc.LocalizationManager.GetTranslation("Evaded");
#else
                value1 = "Evaded";
#endif
            }
            else if (eventType == "CombatImmune")
            {
#if AT_I2LOC_PRESET
               if (target.ToLong() == ClientAPI.GetPlayerOid())
                value1 = I2.Loc.LocalizationManager.GetTranslation("ImmuneSelf"); 
            else
                value1 = I2.Loc.LocalizationManager.GetTranslation("Immune");
#else
                value1 = "Immune";
#endif
            }
            else if (eventType == "CombatBuffGained" || eventType == "CombatDebuffGained")
            {
                AtavismEffect e = Abilities.Instance.GetEffect(effectID);
                if (e != null)
                {
#if AT_I2LOC_PRESET
                value1 = I2.Loc.LocalizationManager.GetTranslation("Ability/"+e.name);
#else
                    value1 = e.name;
#endif
                }
                else
                {
                    value1 = "";
                }
            }
            else if (eventType == "CombatBuffLost" || eventType == "CombatDebuffLost")
            {
                AtavismEffect e = Abilities.Instance.GetEffect(effectID);
                if (e != null)
                {
#if AT_I2LOC_PRESET
           		value1 = I2.Loc.LocalizationManager.GetTranslation("Effects/" + e.name);
#else
                    value1 = e.name;
#endif
                }
                else
                {
                    value1 = "";
                }
            }
            else if (eventType == "CastingStarted")
            {
                if (int.Parse(value1) > 0)
                {
                    string[] csArgs = new string[3];
                    csArgs[0] = (int.Parse(value1)/1000f)+"";
                    csArgs[1] = caster.ToString();
                    csArgs[2] = abilityID.ToString();

                    AtavismEventSystem.DispatchEvent("CASTING_STARTED", csArgs);
                }
                return;
            }
            else if (eventType == "CastingCancelled")
            {
               //    Debug.LogError("CastingCancelled 1");
                string[] ccArgs = new string[2];
                ccArgs[0] = abilityID.ToString();
                ccArgs[1] = caster.ToString();
                AtavismEventSystem.DispatchEvent("CASTING_CANCELLED", ccArgs);
                //   Debug.LogError("CastingCancelled 2");
                return;
            }
            // dispatch a ui event to tell the rest of the system
            try
            {
                string[] args = new string[9];
                args[0] = eventType;
                args[1] = caster.ToString();
                args[2] = target!=null?target.ToString():"";
                args[3] = value1;
                args[4] = value2;
                args[5] = abilityID.ToString();
                args[6] = effectID.ToString();
                args[7] = value3;
                args[8] = value4;
                AtavismEventSystem.DispatchEvent("COMBAT_EVENT", args);
            }
            catch (System.Exception e )
            {

                Debug.LogError("COMBAT_EVENT Exception:" + e);
            }

         

            //ClientAPI.GetObjectNode(target.ToLong()).GameObject.GetComponent<MobController3D>().GotDamageMessage(messageType, value1);
        }

        public void HandleDuelChallenge(Dictionary<string, object> props)
        {
            string challenger = (string)props["challenger"];
#if AT_I2LOC_PRESET
        UGUIConfirmationPanel.Instance.ShowConfirmationBox(challenger + " " + I2.Loc.LocalizationManager.GetTranslation("has challenged you to a Duel. Do you accept the challenge?"), null, DuelChallengeResponse, 30f);
#else
            UGUIConfirmationPanel.Instance.ShowConfirmationBox(challenger + " has challenged you to a Duel. Do you accept the challenge?", null, DuelChallengeResponse);
#endif
        }

        public void DuelChallengeResponse(object item, bool accepted)
        {
            if (accepted)
            {
                NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/duelAccept");
            }
            else
            {
                NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/duelDecline");
            }
        }

        public void HandleDuelChallengeEnd(Dictionary<string, object> props)
        {
            UGUIConfirmationPanel.Instance.Hide();
        }

        public void HandleState(object sender, ObjectPropertyChangeEventArgs args)
        {
            if (args.Oid == ClientAPI.GetPlayerOid())
                return;

            string state = (string)ClientAPI.GetObjectProperty(args.Oid, args.PropName);
            if (state == "spirit")
            {
                if(ClientAPI.GetObjectNode(args.Oid)!=null && ClientAPI.GetObjectNode(args.Oid).GameObject != null)
                    ClientAPI.GetObjectNode(args.Oid).GameObject.SetActive(false);
            }
            else
            {
                if (ClientAPI.GetObjectNode(args.Oid) != null && ClientAPI.GetObjectNode(args.Oid).GameObject != null)
                    ClientAPI.GetObjectNode(args.Oid).GameObject.SetActive(true);
            }
        }

        public static AtavismCombat Instance
        {
            get
            {
                return instance;
            }
        }
        public  string HealthStat
        {
            get
            {
                return healthStat;
            }
        }

        public  string HealthMaxStat
        {
            get
            {
                return healthMaxStat;
            }
        }    
    /*    public  string ManaStat
        {
            get
            {
                return manaStat;
            }
        }

        public  string ManaMaxStat
        {
            get
            {
                return manaMaxStat;
            }
        }*/
    }
}