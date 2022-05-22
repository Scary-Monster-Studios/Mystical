using UnityEngine;
//using UnityEngine.UI.Tweens;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if AT_MASTERAUDIO_PRESET
using DarkTonic.MasterAudio;
#endif

namespace Atavism
{

    [RequireComponent(typeof(CanvasGroup))]
    public class UGUIMailBox : MonoBehaviour
    {
        public UGUIMailList mailList;
        public UGUIMailCompose mailCompose;
        [SerializeField] Text headerText;
        bool showing = false;
        // Use this for initialization

        void Awake()
        {
            if (mailList == null)
                mailList = GetComponentInChildren<UGUIMailList>();
            if (mailCompose == null)
                mailCompose = GetComponentInChildren<UGUIMailCompose>();
            AtavismEventSystem.RegisterEvent("MAILBOX_OPEN", this);
            this.Hide();
        }
        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("MAILBOX_OPEN", this);

        }
        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "MAILBOX_OPEN")
            {
                Mailing.Instance.RequestMailList();
                Show();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(AtavismSettings.Instance.GetKeySettings().mail) && !ClientAPI.UIHasFocus())
            {
                if (showing)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
            }
        }
        public void Show()
        {
            //     gameObject.SetActive(true);
            showing = true;
            this.GetComponent<CanvasGroup>().alpha = 1f;
            this.GetComponent<CanvasGroup>().blocksRaycasts = true;
            AtavismUIUtility.BringToFront(this.gameObject);
            this.ShowList();
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("MAILBOX_OPENED", args);
        }
        public void Hide()
        {
            //     gameObject.SetActive(false);
            showing = false;
            this.GetComponent<CanvasGroup>().alpha = 0f;
            this.GetComponent<CanvasGroup>().blocksRaycasts = false;
            //       this.m_CanvasGroup.interactable = false;
            this.mailCompose.Close();
        }
        public void Toggle()
        {
            if (showing)
                Hide();
            else
                Show();
        }
        public void ShowCompose()
        {
            this.mailList.Hide();
            this.mailCompose.StartNewMail();
#if AT_I2LOC_PRESET
        if (this.headerText != null) this.headerText.text = I2.Loc.LocalizationManager.GetTranslation("New Message").ToUpper();
#else
            if (this.headerText != null)
                this.headerText.text = ("New Message").ToUpper();
#endif
            this.GetComponent<CanvasGroup>().alpha = 1f;
            this.GetComponent<CanvasGroup>().blocksRaycasts = true;
            //   this.mailList.gameObject.SetActive(false);
            this.mailCompose.gameObject.SetActive(true);
        }
        public void ShowList()
        {
            //   this.mailList.gameObject.SetActive(true);
            this.mailCompose.gameObject.SetActive(false);
            this.mailList.Show();
#if AT_I2LOC_PRESET
        if (this.headerText != null) this.headerText.text = I2.Loc.LocalizationManager.GetTranslation("MailBox").ToUpper();
#else
            if (this.headerText != null)
                this.headerText.text = ("MailBox").ToUpper();
#endif

        }



    }
}