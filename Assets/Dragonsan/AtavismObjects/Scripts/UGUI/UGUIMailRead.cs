using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Atavism
{

    public class UGUIMailRead : MonoBehaviour
    {

        public Text nameText;
        public TextMeshProUGUI TMPNameText;
        public Text subjectText;
        public TextMeshProUGUI TMPSubjectText;
        public Text messageText;
        public TextMeshProUGUI TMPMessageText;
        public List<UGUIMailAttachment> itemSlots;
        public List<UGUICurrency> currencyDisplays;
        public Button takeCurrencyButton;
        public UGUIMailCompose composePanel;
        public UGUIMailList mailList;
        public UGUIMailBox mailBox;
        public UGUIPanelTitleBar titleBar;
        MailEntry mailBeingRead;
        [SerializeField] GameObject panel;
        [SerializeField] bool hideOnShowMailList = true;
        [SerializeField] bool hideMailListOnStartRead = true;

        // Use this for initialization
        void Start()
        {
            if (titleBar != null)
                titleBar.SetOnPanelClose(Hide);
            AtavismEventSystem.RegisterEvent("MAIL_UPDATE", this);
            AtavismEventSystem.RegisterEvent("CLOSE_MAIL_WINDOW", this);
            AtavismEventSystem.RegisterEvent("CLOSE_READ_MAIL_WINDOW", this);
            AtavismEventSystem.RegisterEvent("MAIL_SELECTED", this);
            Hide();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("MAIL_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("CLOSE_MAIL_WINDOW", this);
            AtavismEventSystem.UnregisterEvent("CLOSE_READ_MAIL_WINDOW", this);
            AtavismEventSystem.UnregisterEvent("MAIL_SELECTED", this);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (!enabled)
                return;

            if (eData.eventType == "MAIL_UPDATE")
            {
                UpdateAttachmentsDisplay();
            }
            else if (eData.eventType == "CLOSE_MAIL_WINDOW")
            {
                //gameObject.SetActive(false);
                Hide();
            }
            else if (eData.eventType == "CLOSE_READ_MAIL_WINDOW")
            {
                //gameObject.SetActive(false);
                if (hideOnShowMailList)
                    Hide();
            }
            else if (eData.eventType == "MAIL_SELECTED")
            {
                StartReadingMail();
            }
        }

        public void StartReadingMail()
        {
            mailBeingRead = Mailing.Instance.SelectedMail;
            if (mailBeingRead != null)
            {
                Mailing.Instance.SetMailRead(mailBeingRead);
                //gameObject.SetActive(true);
                Show();
                if (nameText != null)
                    nameText.text = mailBeingRead.senderName;
                if (TMPNameText != null)
                    TMPNameText.text = mailBeingRead.senderName;
                if (subjectText != null)
                    subjectText.text = mailBeingRead.subject;
                if (TMPSubjectText != null)
                    TMPSubjectText.text = mailBeingRead.subject;
                if (messageText != null)
                    messageText.text = mailBeingRead.message;
                if (TMPMessageText != null)
                    TMPMessageText.text = mailBeingRead.message;
                UpdateAttachmentsDisplay();
                AtavismUIUtility.BringToFront(gameObject);
                if (hideMailListOnStartRead)
                    if (mailList != null)
                        mailList.Hide();
            }
            else
            {
                Hide();
            }
        }

        void UpdateAttachmentsDisplay()
        {
            mailBeingRead = Mailing.Instance.SelectedMail;
            if (mailBeingRead == null)
                return;
            // Items
            bool isAttachment = false;
            for (int i = 0; i < itemSlots.Count; i++)
            {
                if (mailBeingRead.items.Count > i && mailBeingRead.items[i].item != null)
                {
                    itemSlots[i].gameObject.SetActive(true);
                    itemSlots[i].SetMailAttachmentData(mailBeingRead.items[i].item, mailBeingRead.items[i].count, i);
                    isAttachment = true;

                }
                else
                {
                    itemSlots[i].gameObject.SetActive(false);
                }
            }
            // Currency
            Currency c = mailBeingRead.GetMainCurrency();
            List<CurrencyDisplay> currencyDisplayList = new List<CurrencyDisplay>();
            if (c != null && mailBeingRead.currencies[c] > 0)
            {
                currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(c.id, mailBeingRead.currencies[c]);
                takeCurrencyButton.gameObject.SetActive(true);
            }
            else
            {

                if (isAttachment)
                    takeCurrencyButton.gameObject.SetActive(true);
                else
                    takeCurrencyButton.gameObject.SetActive(false);
            }

            for (int i = 0; i < currencyDisplays.Count; i++)
            {
                if (i < currencyDisplayList.Count)
                {
                    currencyDisplays[i].gameObject.SetActive(true);
                    currencyDisplays[i].SetCurrencyDisplayData(currencyDisplayList[i]);
                }
                else
                {
                    currencyDisplays[i].gameObject.SetActive(false);
                }
            }
        }

        public void TakeCurrency()
        {
            Mailing.Instance.TakeMailCurrency(mailBeingRead);
            for (int i = 0; i < itemSlots.Count; i++)
            {
                if (itemSlots[i].gameObject.activeSelf)
                    Mailing.Instance.TakeMailItem(i);
            }
        }
        public void DeleteMail()
        {
#if AT_I2LOC_PRESET
       if (subjectText != null)   UGUIConfirmationPanel.Instance.ShowConfirmationBox(I2.Loc.LocalizationManager.GetTranslation("DeleteMailPopup") + " " + subjectText.text + "?", null, DeleteMail);
       if (TMPSubjectText != null)   UGUIConfirmationPanel.Instance.ShowConfirmationBox(I2.Loc.LocalizationManager.GetTranslation("DeleteMailPopup") + " " + TMPSubjectText.text + "?", null, DeleteMail);
#else
            if (subjectText != null)
                UGUIConfirmationPanel.Instance.ShowConfirmationBox("Delete Mail " + subjectText.text + "?", null, DeleteMail);
            if (TMPSubjectText != null)
                UGUIConfirmationPanel.Instance.ShowConfirmationBox("Delete Mail " + TMPSubjectText.text + "?", null, DeleteMail);
#endif
        }

        public void DeleteMail(object item, bool accepted)
        {
            if (accepted)
            {
                Mailing.Instance.DeleteMail(mailBeingRead);
                Hide();
                mailBox.ShowList();
            }
        }



        public void Reply()
        {
            if (composePanel == null)
                return;

            //	gameObject.SetActive(false);
            //composePanel.gameObject.SetActive(true);
            Hide();
            mailBox.ShowCompose();
            composePanel.StartReplyMail(mailBeingRead.senderName, mailBeingRead.subject);
        }

        public void Return()
        {
            //mailList.Show();
            mailBox.ShowList();
            Hide();
        }

        public void Close()
        {
            Hide();
            Mailing.Instance.SelectedMail = null;
            //        gameObject.SetActive(false);
        }
        public void Show()
        {
            this.GetComponent<CanvasGroup>().alpha = 1f;
            this.GetComponent<CanvasGroup>().blocksRaycasts = true;
            this.GetComponent<CanvasGroup>().interactable = true;
            if (panel != null)
                panel.SetActive(true);
            AtavismUIUtility.BringToFront(transform.parent.gameObject);

        }
        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);
            this.GetComponent<CanvasGroup>().alpha = 0f;
            this.GetComponent<CanvasGroup>().blocksRaycasts = false;
            //   this.GetComponent<CanvasGroup>().interactable = false;
        }

    }
}