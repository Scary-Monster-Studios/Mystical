using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUIWorldTimeWAPIDisplay : MonoBehaviour
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

            AtavismEventSystem.RegisterEvent("WORLD_TIME_UPDATE_WAPI", this);
            //	UpdateTimeDisplay();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("WORLD_TIME_UPDATE_WAPI", this);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "WORLD_TIME_UPDATE_WAPI")
            {
                //  Debug.LogError("Got WORLD_TIME_UPDATE_WAPI");
                if (timeText != null)
                    timeText.text = eData.eventArgs[4] + "-" + eData.eventArgs[3] + "-" + eData.eventArgs[2] + " " + eData.eventArgs[1] + ":" + eData.eventArgs[0] + " " + eData.eventArgs[5];
                if (TMPTimeText != null)
                    TMPTimeText.text = eData.eventArgs[4] + "-" + eData.eventArgs[3] + "-" + eData.eventArgs[2] + " " + eData.eventArgs[1] + ":" + eData.eventArgs[0] + " " + eData.eventArgs[5];
            }
        }

        /*void UpdateTimeDisplay() {
            if (timeText != null) {
                // Get time from the time system and set it to the text
                string hour = TimeManager.Instance.Hour.ToString();
                if (TimeManager.Instance.Hour < 10) {
                    hour = "0" + hour;
                }
                string minute = TimeManager.Instance.Minute.ToString();
                if (TimeManager.Instance.Minute < 10) {
                    minute = "0" + minute;
                }
                timeText.text = hour + ":" + minute;
            }
        }*/
    }
}