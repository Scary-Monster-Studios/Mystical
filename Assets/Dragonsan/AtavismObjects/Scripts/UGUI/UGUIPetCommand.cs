using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Atavism
{

    public class UGUIPetCommand : MonoBehaviour
    {

        //  CanvasGroup cg;
        bool showing = false;

        void Start()
        {
            NetworkAPI.RegisterExtensionMessageHandler("ao.pet_stats", petStatHeandler);
            ClientAPI.GetPlayerObject().RegisterPropertyChangeHandler("hasPet", activeHandler);
            //   cg = GetComponent<CanvasGroup>();
            Hide();
        }

        public Slider healthBar;
        [SerializeField] Text passiveButtonText;
        [SerializeField] TextMeshProUGUI passiveButtonTextTMP;
        [SerializeField] Text defensiveButtonText;
        [SerializeField] TextMeshProUGUI defensiveButtonTextTMP;
        [SerializeField] Text aggressiveButtonText;
        [SerializeField] TextMeshProUGUI aggressiveButtonTextTMP;
        [SerializeField] Text stayButtonText;
        [SerializeField] TextMeshProUGUI stayButtonTextTMP;
        [SerializeField] Text followButtonText;
        [SerializeField] TextMeshProUGUI followButtonTextTMP;
        [SerializeField] Text attackButtonText;
        [SerializeField] TextMeshProUGUI attackButtonTextTMP;
        [SerializeField] Color selectedTextColor = Color.green;
        [SerializeField] Color defaultTextColor = Color.white;
        int health = 0;
        int health_max = 0;
        private OID activePetOid;
        private AtavismObjectNode node;



        private void petStatHeandler(Dictionary<string, object> props)
        {
            AtavismLogger.LogInfoMessage("Got petStatHeandler");

            if (props.ContainsKey(AtavismCombat.Instance.HealthStat))
            {
                health = (int) props[AtavismCombat.Instance.HealthStat];
                AtavismLogger.LogInfoMessage("Got petStatHeandler health=" + health);
            }

            if (props.ContainsKey(AtavismCombat.Instance.HealthMaxStat))
            {
                health_max = (int) props[AtavismCombat.Instance.HealthMaxStat];
                AtavismLogger.LogInfoMessage("Got petStatHeandler health_max=" + health_max);
            }

            if (health > health_max)
            {
                health_max = health;
                AtavismLogger.LogInfoMessage("Got petStatHeandler set  health_max = health");
            }

          //  Debug.LogError("Pet 1 health=" + health + " health_max=" + health_max);
            try
            {
                if (activePetOid != null)
                {
                    object hm = ClientAPI.GetObjectProperty(activePetOid.ToLong(), AtavismCombat.Instance.HealthMaxStat);
                    if (hm != null)
                        health_max = (int) hm;
                    hm = ClientAPI.GetObjectProperty(activePetOid.ToLong(), AtavismCombat.Instance.HealthStat);
                    if (hm != null)
                        health = (int) hm;
                }
            }
            catch (Exception e)
            {
                AtavismLogger.LogError("Pet petStatHeandler Exception " + e.Message+"\n\n"+e.StackTrace);
            }

          //  Debug.LogError("Pet 2 health=" + health + " health_max=" + health_max);

            if (healthBar != null && health_max > 0)
                healthBar.value = (float) health / (float) health_max;
            AtavismLogger.LogInfoMessage("Got petStatHeandler End");
        }

        /// <summary>
        /// Function switching show / hide
        /// </summary>
        public void Toggle()
        {
            if (showing)
                Hide();
            else
                Show();
        }

        /// <summary>
        /// Function showing window of pet actions
        /// </summary>
        public void Show()
        {
            showing = true;
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
            AtavismUIUtility.BringToFront(this.gameObject);
            if (healthBar)
                healthBar.value = 1;

            if (passiveButtonText)
                passiveButtonText.color = defaultTextColor;
            if (passiveButtonTextTMP)
                passiveButtonTextTMP.color = defaultTextColor;
            if (defensiveButtonText)
                defensiveButtonText.color = selectedTextColor;
            if (defensiveButtonTextTMP)
                defensiveButtonTextTMP.color = selectedTextColor;
            if (aggressiveButtonText)
                aggressiveButtonText.color = defaultTextColor;
            if (aggressiveButtonTextTMP)
                aggressiveButtonTextTMP.color = defaultTextColor;
            if (stayButtonText)
                stayButtonText.color = defaultTextColor;
            if (stayButtonTextTMP)
                stayButtonTextTMP.color = defaultTextColor;
            if (followButtonText)
                followButtonText.color = selectedTextColor;
            if (followButtonTextTMP)
                followButtonTextTMP.color = selectedTextColor;
            if (attackButtonText)
                attackButtonText.color = defaultTextColor;
            if (attackButtonTextTMP)
                attackButtonTextTMP.color = defaultTextColor;

            try
            {
                object hm = ClientAPI.GetObjectProperty(activePetOid.ToLong(), AtavismCombat.Instance.HealthMaxStat);
                if (hm != null)
                    health_max = (int) hm;
                hm = ClientAPI.GetObjectProperty(activePetOid.ToLong(), AtavismCombat.Instance.HealthStat);
                if (hm != null)
                    health = (int) hm;
              //  Debug.LogError("Pet health=" + health + " health_max=" + health_max);
            }
            catch (Exception e)
            {
                AtavismLogger.LogError("Pet show Exception " + e.Message+"\n\n"+e.StackTrace);
            }
        }

        public void Hide()
        {
            showing = false;
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            if (activePetOid != null)
                if (ClientAPI.GetObjectNode(activePetOid.ToLong()) != null)
                {
                    ClientAPI.GetObjectNode(activePetOid.ToLong()).RemovePropertyChangeHandler(AtavismCombat.Instance.HealthMaxStat, petHealthHandler);
                    ClientAPI.GetObjectNode(activePetOid.ToLong()).RemovePropertyChangeHandler(AtavismCombat.Instance.HealthStat, petHealthHandler);
                }

            if (healthBar)
                healthBar.value = 1;
            activePetOid = null;
            node = null;
        }

        public void activeHandler(object sender, PropertyChangeEventArgs args)
        {
            try
            {
                AtavismLogger.LogInfoMessage("Pet activeHandler");
                bool activePet = (bool) ClientAPI.GetPlayerObject().GetProperty("hasPet");
                activePetOid = (OID) ClientAPI.GetPlayerObject().GetProperty("aP");
                AtavismLogger.LogInfoMessage("Pet activeHandler " + DateTime.Now + " activePet=" + activePet);
                //ClientAPI.GetObjectNode(activePetOid.ToLong()).RegisterPropertyChangeHandler("health-max", petHealthHandler);
                //ClientAPI.GetObjectNode(activePetOid.ToLong()).RegisterPropertyChangeHandler("health", petHealthHandler);
                if (!activePet)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
            }
            catch (Exception e)
            {
                AtavismLogger.LogError("Pet activeHandler Exception " + e.Message+"\n\n"+e.StackTrace);
            }
        }

        public void petHealthHandler(object sender, PropertyChangeEventArgs args)
        {
            if (args.PropertyName.Equals(AtavismCombat.Instance.HealthStat))
            {
                object hm = ClientAPI.GetObjectProperty(activePetOid.ToLong(), AtavismCombat.Instance.HealthStat);
                if (hm != null)
                    health = (int) hm;
                AtavismLogger.LogInfoMessage("Got petStatHeandler health=" + health);
            }

            if (args.PropertyName.Equals(AtavismCombat.Instance.HealthMaxStat))
            {
                object hm = ClientAPI.GetObjectProperty(activePetOid.ToLong(), AtavismCombat.Instance.HealthMaxStat);
                if (hm != null)
                    health_max = (int) hm;

                AtavismLogger.LogInfoMessage("Got petStatHeandler health_max=" + health_max);
            }

            if (health > health_max)
            {
                health_max = health;
                AtavismLogger.LogInfoMessage("Got petStatHeandler set  health_max = health");
            }

           // Debug.LogError("Pet health=" + health + " health_max=" + health_max);
            if (healthBar != null && health_max > 0)
                healthBar.value = (float) health / (float) health_max;
        }



        public void PassiveCommand()
        {

            NetworkAPI.SendTargetedCommand(ClientAPI.GetTargetOid(), "/petCommand passive");
            if (passiveButtonText)
                passiveButtonText.color = selectedTextColor;
            if (passiveButtonTextTMP)
                passiveButtonTextTMP.color = selectedTextColor;
            if (defensiveButtonText)
                defensiveButtonText.color = defaultTextColor;
            if (defensiveButtonTextTMP)
                defensiveButtonTextTMP.color = defaultTextColor;
            if (aggressiveButtonText)
                aggressiveButtonText.color = defaultTextColor;
            if (aggressiveButtonTextTMP)
                aggressiveButtonTextTMP.color = defaultTextColor;

        }

        public void DefensiveCommand()
        {
            NetworkAPI.SendTargetedCommand(ClientAPI.GetTargetOid(), "/petCommand defensive");
            if (passiveButtonText)
                passiveButtonText.color = defaultTextColor;
            if (passiveButtonTextTMP)
                passiveButtonTextTMP.color = defaultTextColor;
            if (defensiveButtonText)
                defensiveButtonText.color = selectedTextColor;
            if (defensiveButtonTextTMP)
                defensiveButtonTextTMP.color = selectedTextColor;
            if (aggressiveButtonText)
                aggressiveButtonText.color = defaultTextColor;
            if (aggressiveButtonTextTMP)
                aggressiveButtonTextTMP.color = defaultTextColor;

        }

        public void AggressiveCommand()
        {
            NetworkAPI.SendTargetedCommand(ClientAPI.GetTargetOid(), "/petCommand aggressive");
            if (passiveButtonText)
                passiveButtonText.color = defaultTextColor;
            if (passiveButtonTextTMP)
                passiveButtonTextTMP.color = defaultTextColor;
            if (defensiveButtonText)
                defensiveButtonText.color = defaultTextColor;
            if (defensiveButtonTextTMP)
                defensiveButtonTextTMP.color = defaultTextColor;
            if (aggressiveButtonText)
                aggressiveButtonText.color = selectedTextColor;
            if (aggressiveButtonTextTMP)
                aggressiveButtonTextTMP.color = selectedTextColor;

        }

        public void StayCommand()
        {
            NetworkAPI.SendTargetedCommand(ClientAPI.GetTargetOid(), "/petCommand stay");

            if (stayButtonText)
                stayButtonText.color = selectedTextColor;
            if (stayButtonTextTMP)
                stayButtonTextTMP.color = selectedTextColor;
            if (followButtonText)
                followButtonText.color = defaultTextColor;
            if (followButtonTextTMP)
                followButtonTextTMP.color = defaultTextColor;

        }

        public void FollowCommand()
        {
            NetworkAPI.SendTargetedCommand(ClientAPI.GetTargetOid(), "/petCommand follow");

            if (stayButtonText)
                stayButtonText.color = defaultTextColor;
            if (stayButtonTextTMP)
                stayButtonTextTMP.color = defaultTextColor;
            if (followButtonText)
                followButtonText.color = selectedTextColor;
            if (followButtonTextTMP)
                followButtonTextTMP.color = selectedTextColor;

        }

        public void AttackCommand()
        {
            NetworkAPI.SendTargetedCommand(ClientAPI.GetTargetOid(), "/petCommand attack");

        }

        public void DespawnCommand()
        {
            NetworkAPI.SendTargetedCommand(ClientAPI.GetTargetOid(), "/petCommand despawn");
        }

        public void SelectPet()
        {
            if (activePetOid != null)
                ClientAPI.SetTarget(activePetOid.ToLong());
        }

    }
}