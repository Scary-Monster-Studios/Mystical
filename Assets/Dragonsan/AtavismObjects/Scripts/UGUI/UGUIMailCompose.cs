using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Atavism
{

    public class UGUIMailCompose : MonoBehaviour
    {
        public UGUIMailBox mailBox;
        public InputField toText;
        public InputField subjectText;
        public InputField messageText;
        public TMP_InputField TMPToText;
        public TMP_InputField TMPSubjectText;
        public TMP_InputField TMPMessageText;
        //    public InputField toText;
        //public InputField subjectText;
        //public InputField messageText;
        public List<UGUIMailAttachmentSlot> attachmentSlots;
        public List<InputField> currencyFields;
        public List<TMP_InputField> TMPCurrencyFields;
        public List<Image> currencyIcons;
        [SerializeField] Toggle codToggle;
        [SerializeField] Toggle moneyToggle;

        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("CLOSE_MAIL_WINDOW", this);
            AtavismEventSystem.RegisterEvent("MAIL_SENT", this);
            if (mailBox == null)
                mailBox = GetComponentInParent<UGUIMailBox>();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("CLOSE_MAIL_WINDOW", this);
            AtavismEventSystem.UnregisterEvent("MAIL_SENT", this);
        }

        void OnEnable()
        {
            StartNewMail();
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "CLOSE_MAIL_WINDOW")
            {
                //	gameObject.SetActive(false);
                this.mailBox.Hide();
            }
            else if (eData.eventType == "MAIL_SENT")
            {
                //	gameObject.SetActive(false);
                this.mailBox.ShowList();
            }
        }

        public void StartNewMail()
        {
            Mailing.Instance.StartComposingMail();
            if (toText != null)
                toText.text = "";
            if (subjectText != null)
                subjectText.text = "";
            if (messageText != null)
                messageText.text = "";

            if (TMPToText != null)
                TMPToText.text = "";
            if (TMPSubjectText != null)
                TMPSubjectText.text = "";
            if (TMPMessageText != null)
                TMPMessageText.text = "";

            for (int i = 0; i < currencyFields.Count; i++)
            {
                if (i < Mailing.Instance.MailBeingComposed.currencies.Count)
                {
                    currencyFields[i].gameObject.SetActive(true);
                    currencyFields[i].text = "0";
                    currencyIcons[i].gameObject.SetActive(true);
                    currencyIcons[i].sprite = Inventory.Instance.GetMainCurrency(i).icon;
                }
                else
                {
                    currencyFields[i].gameObject.SetActive(false);
                    currencyIcons[i].gameObject.SetActive(false);
                }
            }
            for (int i = 0; i < TMPCurrencyFields.Count; i++)
            {
                if (i < Mailing.Instance.MailBeingComposed.currencies.Count)
                {
                    TMPCurrencyFields[i].gameObject.SetActive(true);
                    TMPCurrencyFields[i].text = "0";
                    currencyIcons[i].gameObject.SetActive(true);
                    currencyIcons[i].sprite = Inventory.Instance.GetMainCurrency(i).icon;
                }
                else
                {
                    TMPCurrencyFields[i].gameObject.SetActive(false);
                    currencyIcons[i].gameObject.SetActive(false);
                }
            }

            foreach (UGUIMailAttachmentSlot attachmentSlot in attachmentSlots)
            {
                attachmentSlot.Discarded();
            }

            if (moneyToggle != null)
                if (moneyToggle.isOn != !Mailing.Instance.MailBeingComposed.cashOnDelivery)
                    moneyToggle.isOn = !Mailing.Instance.MailBeingComposed.cashOnDelivery;
            if (codToggle != null)
                if (codToggle.isOn != Mailing.Instance.MailBeingComposed.cashOnDelivery)
                    codToggle.isOn = Mailing.Instance.MailBeingComposed.cashOnDelivery;

        }

        public void StartReplyMail(string to, string subject)
        {
            StartNewMail();
            if (toText != null)
                toText.text = to;
            if (subjectText != null)
                subjectText.text = "Re: " + subject;
            if (TMPToText != null)
                TMPToText.text = to;
            if (TMPSubjectText != null)
                TMPSubjectText.text = "Re: " + subject;
        }

        public void SetMailTo(string to)
        {
            Mailing.Instance.MailBeingComposed.senderName = to;
        }

        public void SetSubject(string subject)
        {
            Mailing.Instance.MailBeingComposed.subject = subject;
        }

        public void SetMessage(string message)
        {
            Mailing.Instance.MailBeingComposed.message = message;
        }

        /// <summary>
        /// Updates the currency amount for the first "main" currency
        /// </summary>
        /// <param name="currencyAmount">Currency amount.</param>
        public void SetCurrency1(string currencyAmount)
        {
            int amount = int.Parse(currencyAmount);
            if (amount < 0)
                amount = 0;
            Mailing.Instance.SetMailCurrencyAmount(0, amount);
            if (currencyFields.Count > 0 && currencyFields[0] != null)
                currencyFields[0].text = amount.ToString();


            checkCurrency();
        }

        /// <summary>
        /// Updates the currency amount for the first "main" currency
        /// </summary>
        /// <param name="currencyAmount">Currency amount.</param>
        public void SetCurrency2(string currencyAmount)
        {
            int amount = int.Parse(currencyAmount);
            if (amount < 0)
                amount = 0;
            Mailing.Instance.SetMailCurrencyAmount(1, amount);
            if (currencyFields.Count > 1 && currencyFields[1] != null)
                currencyFields[1].text = amount.ToString();
            checkCurrency();
        }

        /// <summary>
        /// Updates the currency amount for the first "main" currency
        /// </summary>
        /// <param name="currencyAmount">Currency amount.</param>
        public void SetCurrency3(string currencyAmount)
        {
            int amount = int.Parse(currencyAmount);
            if (amount < 0)
                amount = 0;
            Mailing.Instance.SetMailCurrencyAmount(2, amount);
            if (currencyFields.Count > 2 && currencyFields[2] != null)
                currencyFields[2].text = amount.ToString();
            checkCurrency();
        }

        public void SetSendMoney(bool sendMoney)
        {
            Mailing.Instance.MailBeingComposed.cashOnDelivery = !sendMoney;
            if (codToggle != null)
                if (codToggle.isOn != !sendMoney)
                    codToggle.isOn = !sendMoney;

        }

        public void SetCOD(bool COD)
        {
            Mailing.Instance.MailBeingComposed.cashOnDelivery = COD;
            if (moneyToggle != null)
                if (moneyToggle.isOn != !COD)
                    moneyToggle.isOn = !COD;
        }

        public void Send()
        {
            if (toText != null)
                Mailing.Instance.MailBeingComposed.senderName = toText.text;
            if (messageText != null)
                Mailing.Instance.MailBeingComposed.message = messageText.text;
            if (subjectText != null)
                Mailing.Instance.MailBeingComposed.subject = subjectText.text;

            if (TMPToText != null)
                Mailing.Instance.MailBeingComposed.senderName = TMPToText.text;
            if (TMPMessageText != null)
                Mailing.Instance.MailBeingComposed.message = TMPMessageText.text;
            if (TMPSubjectText != null)
                Mailing.Instance.MailBeingComposed.subject = TMPSubjectText.text;

            Mailing.Instance.SendMail();
            // Clear all attachment slots
            foreach (UGUIMailAttachmentSlot attachmentSlot in attachmentSlots)
            {
                if (attachmentSlot.UguiActivatable != null)
                    attachmentSlot.Discarded();
            }
            //gameObject.SetActive(false);
            mailBox.ShowList();
        }

        public void Cancel()
        {
            mailBox.ShowList();
            foreach (UGUIMailAttachmentSlot attachmentSlot in attachmentSlots)
            {
                if (attachmentSlot.UguiActivatable != null)
                    attachmentSlot.Discarded();
            }
            gameObject.SetActive(false);
        }

        public void Close()
        {
            foreach (UGUIMailAttachmentSlot attachmentSlot in attachmentSlots)
            {
                if (attachmentSlot.UguiActivatable != null)
                    attachmentSlot.Discarded();
            }
            gameObject.SetActive(false);
        }

        bool checkCurrency()
        {
            List<Vector2> currencies = new List<Vector2>();
            foreach (Currency currency in Mailing.Instance.MailBeingComposed.currencies.Keys)
            {
                currencies.Add(new Vector2(currency.id, Mailing.Instance.MailBeingComposed.currencies[currency]));
            }

            if (Inventory.Instance.DoesPlayerHaveEnoughCurrency(currencies))
            {
                Debug.Log("Player does have enough currency");
                return true;
            }
            Debug.Log("Player does not have enough currency");
            return false;
        }
    }
}