using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Atavism
{
    public class UGUIRegionMinimap : MonoBehaviour
    {
        //   float stopDisplay;
        //  bool showing = false;
        string actRegion = "";
        void Start()
        {
            AtavismEventSystem.RegisterEvent("REGION_MESSAGE", this);
            AtavismEventSystem.RegisterEvent("UPDATE_LANGUAGE", this);
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("REGION_MESSAGE", this);
            AtavismEventSystem.UnregisterEvent("UPDATE_LANGUAGE", this);
        }

        void Show(string message)
        {
#if AT_I2LOC_PRESET
        message = string.IsNullOrEmpty(I2.Loc.LocalizationManager.GetTranslation("Regions/" + message)) ? message : I2.Loc.LocalizationManager.GetTranslation("Regions/" + message);
#endif
            GetComponent<TextMeshProUGUI>().text = message;
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "REGION_MESSAGE")
            {
                if (!actRegion.Equals(eData.eventArgs[0]))
                {
                    actRegion = eData.eventArgs[0];
                    Show(eData.eventArgs[0]);
                }
            }
            else if (eData.eventType == "UPDATE_LANGUAGE")
            {
                Show(actRegion);
            }
        }
    }
}