using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{
    public class UGUIWorldTimeDisplay : MonoBehaviour
    {

        public Text timeText;
        public TextMeshProUGUI TMPTimeText;

        // Use this for initialization
        void Start()
        {
            if (timeText == null)
            {
                timeText = GetComponent<Text>();
            }
            if (TMPTimeText == null)
            {
                TMPTimeText = GetComponent<TextMeshProUGUI>();
            }

            AtavismEventSystem.RegisterEvent("WORLD_TIME_UPDATE", this);
            UpdateTimeDisplay();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("WORLD_TIME_UPDATE", this);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "WORLD_TIME_UPDATE")
            {
                UpdateTimeDisplay();
            }
        }

        void UpdateTimeDisplay()
        {
            if (timeText != null)
            {
                // Get time from the time system and set it to the text
                string hour = TimeManager.Instance.Hour.ToString();
                if (TimeManager.Instance.Hour < 10)
                {
                    hour = "0" + hour;
                }
                string minute = TimeManager.Instance.Minute.ToString();
                if (TimeManager.Instance.Minute < 10)
                {
                    minute = "0" + minute;
                }
                timeText.text = hour + ":" + minute;
            }
            if (TMPTimeText != null)
            {
                // Get time from the time system and set it to the text
                string hour = TimeManager.Instance.Hour.ToString();
                if (TimeManager.Instance.Hour < 10)
                {
                    hour = "0" + hour;
                }
                string minute = TimeManager.Instance.Minute.ToString();
                if (TimeManager.Instance.Minute < 10)
                {
                    minute = "0" + minute;
                }
                TMPTimeText.text = hour + ":" + minute;
            }
        }
    }
}