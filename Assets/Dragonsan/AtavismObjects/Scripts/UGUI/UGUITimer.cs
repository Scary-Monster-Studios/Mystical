using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;

namespace Atavism
{
    public class UGUITimer : MonoBehaviour
    {
        [SerializeField] Slider progress;
        float stopDisplay;
        bool showing = false;
        [SerializeField] float countdown = 0f;
        [SerializeField] float total = 0f;
        TextMeshProUGUI countdownText;
        // Use this for initialization
        void Start()
        {
            Hide();
            AtavismEventSystem.RegisterEvent("TimerStart", this);
            AtavismEventSystem.RegisterEvent("TimerStop", this);
            countdownText = GetComponent<TextMeshProUGUI>();
            NetworkAPI.RegisterExtensionMessageHandler("arena_ready", HandleArenaReady);
            NetworkAPI.RegisterExtensionMessageHandler("arena_setup", HandleArenaSetup);
            NetworkAPI.RegisterExtensionMessageHandler("arena_countdown", HandleArenaCouldown);
            NetworkAPI.RegisterExtensionMessageHandler("arena_started", HandleArenaStart);
            NetworkAPI.RegisterExtensionMessageHandler("arena_end", HandleArenaEnd);
            NetworkAPI.RegisterExtensionMessageHandler("arena_abilities", HandleArenaAbilities);

        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("TimerStart", this);
            AtavismEventSystem.UnregisterEvent("TimerStop", this);
            NetworkAPI.RemoveExtensionMessageHandler("arena_ready", HandleArenaReady);
            NetworkAPI.RemoveExtensionMessageHandler("arena_setup", HandleArenaSetup);
            NetworkAPI.RemoveExtensionMessageHandler("arena_countdown", HandleArenaCouldown);
            NetworkAPI.RemoveExtensionMessageHandler("arena_started", HandleArenaStart);
            NetworkAPI.RemoveExtensionMessageHandler("arena_end", HandleArenaEnd);
            NetworkAPI.RemoveExtensionMessageHandler("arena_abilities", HandleArenaAbilities);
        }

        private void HandleArenaAbilities(Dictionary<string, object> props)
        {
          //  throw new NotImplementedException();
        }

        private void HandleArenaEnd(Dictionary<string, object> props)
        {
            Hide();
        }

        private void HandleArenaStart(Dictionary<string, object> props)
        {
            int len = (int)props["timeLeft"];
            total = len / 1000f;
            countdown = Time.time + total;
            Show();
        }

        private void HandleArenaCouldown(Dictionary<string, object> props)
        {
            int len = (int)props["setupLength"];
            total = len / 1000f;
            countdown = Time.time + total;
            Show();
        }

        private void HandleArenaSetup(Dictionary<string, object> props)
        {
        }

        private void HandleArenaReady(Dictionary<string, object> props)
        {
         /*   int len = (int)props["setupLength"];
            total = len / 1000f;
            countdown = Time.time + total;*/
            Show();
        }

        // Update is called once per frame
        void Update()
        {
            if (showing && Time.time > countdown)
            {
                Hide();
            }
            if (countdownText != null)
                if (Time.time < countdown)
                {
                    float sec = countdown - Time.time;
                    int min = 0;
                    if (sec > 60)
                    {
                        min = (int)sec / 60;
                    }

                    countdownText.text = (min > 0 ? min + ":" : "") + ((((int)sec - min * 60) < 10 && min > 0) ? "0" + ((int)sec - min * 60) : "" + ((int)sec - min * 60));
                    if (progress != null)
                    {
                        progress.value = sec / total;
                        progress.gameObject.SetActive(true);
                    }
                }
                else
                {
                    countdownText.text = "";
                    if (progress != null)
                        progress.gameObject.SetActive(false);
                }
        }

        void Hide()
        {

            GetComponent<CanvasGroup>().alpha = 0f;
            showing = false;
        }
        void Show()
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            showing = true;
        }


        public void OnEvent(AtavismEventData eData)
        {
            //    Debug.LogError("Timer ");
            if (eData.eventType == "TimerStart")
            {
                //       Debug.LogError("TimerStart " + eData.eventArgs[0]);
                total = float.Parse(eData.eventArgs[0]);
                countdown = Time.time + total;
                Show();
            }
            else if (eData.eventType == "TimerStop")
            {
                countdown = 0f;
                //   Debug.LogError("TimerStop " );
                Hide();
            }
        }
    }
}