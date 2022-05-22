using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Atavism
{
    public class UGUIAdminMessage : MonoBehaviour
    {
        float stopDisplay;
        bool showing = false;
        void Start()
        {
            Hide();
            AtavismEventSystem.RegisterEvent("ADMIN_MESSAGE", this);
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("ADMIN_MESSAGE", this);
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
            if (GetComponent<TextMeshProUGUI>() != null)
                GetComponent<TextMeshProUGUI>().text = message;
            if (GetComponent<Text>() != null)
                GetComponent<Text>().text = message;
            GetComponent<CanvasGroup>().alpha = 1f;
            stopDisplay = Time.time + 4f;
            showing = true;
        }

        void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            showing = false;
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "ADMIN_MESSAGE")
            {
                Show(eData.eventArgs[0]);
            }
        }
    }
}