using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Atavism
{
    public class UGUIAnnouncementSpecialText : MonoBehaviour
    {

        public float displayTime = 3f;
        float stopDisplay;
        bool showing = false;
        string lastmessage = "";
        // Use this for initialization
        void Start()
        {
            Hide();
            AtavismEventSystem.RegisterEvent("ANNOUNCEMENT_SPECIAL", this);
            NetworkAPI.RegisterExtensionMessageHandler("announcement_special", HandleAnnouncementMessage);
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("ANNOUNCEMENT_SPECIAL", this);
            NetworkAPI.RemoveExtensionMessageHandler("announcement_special", HandleAnnouncementMessage);
        }

        // Update is called once per frame
        void Update()
        {
            if (showing && Time.time > stopDisplay)
            {
                Hide();
            }

        }

        void Show(string message)
        {
            if (string.IsNullOrEmpty(message) || message.Equals(lastmessage))
                return;
            lastmessage = message;
            GetComponent<TextMeshProUGUI>().text = message;
            GetComponent<CanvasGroup>().alpha = 1f;
            stopDisplay = Time.time + displayTime;
            showing = true;
        }

        public void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            showing = false;
        }

        public void HandleAnnouncementMessage(Dictionary<string, object> props)
        {
            Show((string)props["AnnouncementText"]);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "ANNOUNCEMENT_SPECIAL")
            {
                Show(eData.eventArgs[0]);
            }
        }
    }
}