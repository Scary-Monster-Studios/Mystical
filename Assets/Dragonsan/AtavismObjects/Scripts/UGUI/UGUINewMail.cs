using System.Collections;
using System.Collections.Generic;
#if AT_MASTERAUDIO_PRESET
using DarkTonic.MasterAudio;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Atavism
{

    public class UGUINewMail : MonoBehaviour
    {
        private Image image;
        public string audioName = "";

        void Start()
        {
            AtavismEventSystem.RegisterEvent("NO_NEW_MAIL", this);
            AtavismEventSystem.RegisterEvent("NEW_MAIL", this);
            AtavismEventSystem.RegisterEvent("MAILBOX_OPENED", this);
            if (image == null)
                image = GetComponent<Image>();
            Hide();
        }

        void Awake()
        {
            Hide();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("NO_NEW_MAIL", this);
            AtavismEventSystem.UnregisterEvent("NEW_MAIL", this);
            AtavismEventSystem.UnregisterEvent("MAILBOX_OPENED", this);
        }

        void Show()
        {
            if (image)
            {
                image.enabled = true;
            }
#if AT_MASTERAUDIO_PRESET
            MasterAudio.PlaySoundAndForget(audioName);
#endif
        }

        void Hide()
        {
            if (image)
            {
                image.enabled = false;
            }
        }

        public void OnEvent(AtavismEventData eData)
        {
          //  Debug.LogError(eData.eventType);
            if (eData.eventType == "NEW_MAIL")
            {
                Show();
            }
            else if (eData.eventType == "MAILBOX_OPENED")
            {
                Hide();
            }
            else if (eData.eventType == "NO_NEW_MAIL")
            {
                Hide();
            }
        }

        public void Click()
        {
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("MAILBOX_OPEN", args);
        }
    }
}