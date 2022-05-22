using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Atavism
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UGUIRegions : MonoBehaviour
    {
        float stopDisplay;
        bool showing = false;
        string actRegion = "";
        void Start()
        {
            Hide();
            AtavismEventSystem.RegisterEvent("REGION_MESSAGE", this);


        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("REGION_MESSAGE", this);


        }
        void Update()
        {
            if (showing && Time.time > stopDisplay)
            {
                Hide();
            }
        }

        void Show(string message)
        {
#if AT_I2LOC_PRESET
        message = string.IsNullOrEmpty(I2.Loc.LocalizationManager.GetTranslation("Regions/" + message)) ? message : I2.Loc.LocalizationManager.GetTranslation("Regions/" + message);
#endif
            if (GetComponent<TextMeshProUGUI>() != null)
                GetComponent<TextMeshProUGUI>().text = message;
            if (GetComponent<Text>() != null)
                GetComponent<Text>().text = message;
            GetComponent<CanvasGroup>().alpha = 1f;
            stopDisplay = Time.time + 4f;
            showing = true;
        }

        public void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            showing = false;
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
        }
    }
}