using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{

    public class UGUIActionBar : MonoBehaviour
    {

        public int barNum = 0;
        public bool mainActionBar = true;
        public List<UGUIActionBarSlot> slots;
        private bool presed = false;

        // Use this for initialization
        void Start()
        {
            //Actions.Instance.AddActionBar(gameObject, barNum);

            AtavismEventSystem.RegisterEvent("ACTION_UPDATE", this);
            AtavismEventSystem.RegisterEvent("COOLDOWN_UPDATE", this);
            AtavismEventSystem.RegisterEvent("INVENTORY_UPDATE", this);
            AtavismEventSystem.RegisterEvent("ABILITY_UPDATE", this);
            UpdateActions();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("ACTION_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("COOLDOWN_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("INVENTORY_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("ABILITY_UPDATE", this);
        }


        void Update()
        {
            if (Input.GetKeyDown(AtavismSettings.Instance.GetKeySettings().sprint) && !ClientAPI.UIHasFocus())
            {
               // Debug.LogError("sprint Down");
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("state", 1);
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "combat.SPRINT", props);
            }
            else if(Input.GetKeyUp(AtavismSettings.Instance.GetKeySettings().sprint) && !ClientAPI.UIHasFocus())
            {
              //  Debug.LogError("sprint Up");
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("state", 0);
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "combat.SPRINT", props);
            }
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "ACTION_UPDATE") 
            {
                UpdateActions();
            }
            else if (eData.eventType == "ABILITY_UPDATE")
            {
                UpdateActions();
            }
            else if (eData.eventType == "COOLDOWN_UPDATE")
            {
                UpdateActions();
            }
            else if (eData.eventType == "INVENTORY_UPDATE")
            {
                UpdateActions();
            }
        }

        public void UpdateActions()
        {
            List<List<AtavismAction>> actionBars = Actions.Instance.PlayerActions;
            if (actionBars.Count == 0)
            {
                return;
            }
            if (mainActionBar)
            {
                barNum = Actions.Instance.MainActionBar;
            }

            List<AtavismAction> actionBar = new List<AtavismAction>();
            if (actionBars.Count > barNum)
            {
                actionBar = actionBars[barNum];
            }
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].isActiveAndEnabled)
                    if (actionBar.Count > i)
                    {
                        slots[i].UpdateActionData(actionBar[i], barNum);
                    }
                    else
                    {
                        slots[i].UpdateActionData(null, barNum);

                    }
            }
        }

        public void Toggle()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}