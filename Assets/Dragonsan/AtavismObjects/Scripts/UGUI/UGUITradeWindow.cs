using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Atavism
{

    public class UGUITradeWindow : MonoBehaviour
    {

        public UGUIPanelTitleBar titleBar;
        public Text myName;
        public TextMeshProUGUI TMPMyName;
        public Text partnerName;
        public TextMeshProUGUI TMPPartnerName;
        public List<UGUITradeSlot> myOfferEntries;
        public List<Image> myCurrencyIcons;
        public List<InputField> myCurrencyInput;
        public List<TMP_InputField> TMPMyCurrencyInput;
        public List<UGUITradeOffer> partnerOfferEntries;
        public List<UGUICurrency> partnerOfferedCurrencies;
        public Text myStatus;
        public TextMeshProUGUI TMPMyStatus;
        public Text partnerStatus;
        public TextMeshProUGUI TMPPartnerStatus;
        bool showing = false;
        [SerializeField]
        GameObject panel;

        // Use this for initialization
        void Start()
        {
            if (titleBar != null)
                titleBar.SetOnPanelClose(CancelTrade);
            Hide();
            AtavismEventSystem.RegisterEvent("TRADE_START", this);
            AtavismEventSystem.RegisterEvent("TRADE_OFFER_UPDATE", this);
            AtavismEventSystem.RegisterEvent("TRADE_COMPLETE", this);
            AtavismEventSystem.RegisterEvent("ITEM_ICON_UPDATE", this);
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("TRADE_START", this);
            AtavismEventSystem.UnregisterEvent("TRADE_OFFER_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("TRADE_COMPLETE", this);
            AtavismEventSystem.UnregisterEvent("ITEM_ICON_UPDATE", this);
        }

        public void Show()
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            showing = true;

            //if (titleBar != null)
            //	titleBar.SetPanelTitle(ClientAPI.GetObjectNode(AtavismTrade.Instance.TradePartnerOid.ToLong()).Name);
            if (myName != null)
                myName.text = ClientAPI.GetPlayerObject().Name;
            if (TMPMyName != null)
                TMPMyName.text = ClientAPI.GetPlayerObject().Name;
            if (partnerName != null)
                partnerName.text = ClientAPI.GetObjectNode(AtavismTrade.Instance.TradePartnerOid.ToLong()).Name;

               if (TMPPartnerName != null)
                TMPPartnerName.text = ClientAPI.GetObjectNode(AtavismTrade.Instance.TradePartnerOid.ToLong()).Name;

            AtavismTrade.Instance.NewTradeStarted();
            AtavismUIUtility.BringToFront(gameObject);
            // Handle currency
            for (int i = 0; i < myCurrencyInput.Count; i++)
            {
                if (myCurrencyInput[i] != null)
                    myCurrencyInput[i].text = "";
            }
            for (int i = 0; i < TMPMyCurrencyInput.Count; i++)
            {
                if (TMPMyCurrencyInput[i] != null)
                    TMPMyCurrencyInput[i].text = "";
            }
            if (panel != null)
                panel.SetActive(true);
        }

        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            showing = false;

            // Set all referenced items back to non referenced
            for (int i = 0; i < myOfferEntries.Count; i++)
            {
                myOfferEntries[i].ResetSlot();
            }
        }

        void UpdateTradeWindow()
        {
            for (int i = 0; i < myOfferEntries.Count; i++)
            {
                myOfferEntries[i].UpdateTradeSlotData(AtavismTrade.Instance.MyOffers[i]);
            }
            for (int i = 0; i < partnerOfferEntries.Count; i++)
            {
                partnerOfferEntries[i].UpdateTradeOfferData(AtavismTrade.Instance.TheirOffers[i]);
            }

            // Handle currency
            for (int i = 0; i < myCurrencyIcons.Count; i++)
            {
                if (i < Inventory.Instance.GetMainCurrencies().Count)
                {
                    myCurrencyIcons[i].sprite = Inventory.Instance.GetMainCurrency(i).icon;
                }
            }

            for (int i = 0; i < partnerOfferedCurrencies.Count; i++)
            {
                if (i < AtavismTrade.Instance.TheirCurrencyOffers.Count)
                {
                    partnerOfferedCurrencies[i].gameObject.SetActive(true);
                    partnerOfferedCurrencies[i].SetCurrencyDisplayData(AtavismTrade.Instance.TheirCurrencyOffers[i]);
                }
                else
                {
                    partnerOfferedCurrencies[i].gameObject.SetActive(false);
                }
            }

            // If accepted set the colour of the panels
            if (AtavismTrade.Instance.AcceptedByMe)
            {
#if AT_I2LOC_PRESET
           if (myStatus!=null)  myStatus.text = I2.Loc.LocalizationManager.GetTranslation("Accepted");
           if (TMPMyStatus!=null)  TMPMyStatus.text = I2.Loc.LocalizationManager.GetTranslation("Accepted");
#else
                if (myStatus != null)
                    myStatus.text = "Accepted";
                if (TMPMyStatus != null)
                    TMPMyStatus.text = "Accepted";
#endif
            }
            else
            {
#if AT_I2LOC_PRESET
          if (myStatus!=null)   myStatus.text = I2.Loc.LocalizationManager.GetTranslation("Pending...");
          if (TMPMyStatus!=null)   TMPMyStatus.text = I2.Loc.LocalizationManager.GetTranslation("Pending...");
#else
                if (myStatus != null)
                    myStatus.text = "Pending...";
                if (TMPMyStatus != null)
                    TMPMyStatus.text = "Pending...";
#endif
            }

            if (AtavismTrade.Instance.AcceptedByPartner)
            {
#if AT_I2LOC_PRESET
           if (partnerStatus!=null)  partnerStatus.text = I2.Loc.LocalizationManager.GetTranslation("Accepted");
           if (TMPPartnerStatus!=null)  TMPPartnerStatus.text = I2.Loc.LocalizationManager.GetTranslation("Accepted");
#else
                if (partnerStatus != null)
                    partnerStatus.text = "Accepted";
                if (TMPPartnerStatus != null)
                    TMPPartnerStatus.text = "Accepted";
#endif
            }
            else
            {
#if AT_I2LOC_PRESET
          if (partnerStatus!=null)   partnerStatus.text = I2.Loc.LocalizationManager.GetTranslation("Pending...");
          if (TMPPartnerStatus!=null)   TMPPartnerStatus.text = I2.Loc.LocalizationManager.GetTranslation("Pending...");
#else
                if (partnerStatus != null)
                    partnerStatus.text = "Pending...";
                if (TMPPartnerStatus != null)
                    TMPPartnerStatus.text = "Pending...";
#endif
            }
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "TRADE_START")
            {
                if (!showing)
                    Show();
                UpdateTradeWindow();
            }
            else if (eData.eventType == "TRADE_OFFER_UPDATE")
            {
                UpdateTradeWindow();
            }
            else if (eData.eventType == "ITEM_ICON_UPDATE")
            {
                UpdateTradeWindow();
            }
            else if (eData.eventType == "TRADE_COMPLETE")
            {
                Hide();
            }
        }

        /// <summary>
        /// Updates the currency amount for the first "main" currency
        /// </summary>
        /// <param name="currencyAmount">Currency amount.</param>
        public void SetCurrency1(string currencyAmount)
        {
            if (currencyAmount == "")
                currencyAmount = "0";
            AtavismTrade.Instance.SetCurrencyAmount(Inventory.Instance.GetMainCurrency(0).id, int.Parse(currencyAmount));
        }

        /// <summary>
        /// Updates the currency amount for the first "main" currency
        /// </summary>
        /// <param name="currencyAmount">Currency amount.</param>
        public void SetCurrency2(string currencyAmount)
        {
            if (currencyAmount == "")
                currencyAmount = "0";
            AtavismTrade.Instance.SetCurrencyAmount(Inventory.Instance.GetMainCurrency(1).id, int.Parse(currencyAmount));
        }

        /// <summary>
        /// Updates the currency amount for the first "main" currency
        /// </summary>
        /// <param name="currencyAmount">Currency amount.</param>
        public void SetCurrency3(string currencyAmount)
        {
            if (currencyAmount == "")
                currencyAmount = "0";
            AtavismTrade.Instance.SetCurrencyAmount(Inventory.Instance.GetMainCurrency(2).id, int.Parse(currencyAmount));
        }

        bool checkCurrency()
        {
            List<Vector2> currencies = new List<Vector2>();
            foreach (string currencyID in AtavismTrade.Instance.MyCurrencyOffers.Keys)
            {
                currencies.Add(new Vector2(int.Parse(currencyID), AtavismTrade.Instance.MyCurrencyOffers[currencyID]));
            }

            if (Inventory.Instance.DoesPlayerHaveEnoughCurrency(currencies))
            {
                Debug.Log("Player does have enough currency");
                return true;
            }
            Debug.Log("Player does not have enough currency");
            return false;
        }

        public void AcceptTrade()
        {
            AtavismTrade.Instance.AcceptTrade();
        }

        public void CancelTrade()
        {
            AtavismTrade.Instance.CancelTrade();
            Hide();
        }
    }
}