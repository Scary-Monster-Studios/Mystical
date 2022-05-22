using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Atavism
{

    public delegate void AuctionResponse(Auction auction);
    public delegate void InventoryAuctionResponse(AtavismInventoryItem item);

    public class UGUIAuctionSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        [SerializeField] TextMeshProUGUI itemName;
        [SerializeField] TextMeshProUGUI itemQuantity;
        [SerializeField] Image itemIcon;
        [SerializeField] Image itemQuality;
        [SerializeField] List<UGUICurrency> currencyList;
        AuctionResponse actionResponse;
        InventoryAuctionResponse ItemResponse;
        Auction auction;
        bool mouseEntered = false;

        void Start()
        {
            AtavismEventSystem.RegisterEvent("ITEM_ICON_UPDATE", this);
        }

        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("ITEM_ICON_UPDATE", this);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "ITEM_ICON_UPDATE")
            {
                if (auction != null)
                {
                    if (auction.item != null)
                    {
                        auction.item.icon = AtavismPrefabManager.Instance.GetItemIconByID(auction.item.templateId);
                        itemIcon.sprite = auction.item.icon;
                    }
                    if (this.itemIcon != null)
                    {
                        itemIcon.enabled = true;
                        if (auction.item.icon != null)
                            itemIcon.sprite = auction.item.icon;
                        else
                            itemIcon.sprite = AtavismSettings.Instance.defaultItemIcon;
                    }
                }
            }
        }



        public void SetDetale(AuctionResponse actionResponse, Auction auction)
        {
            this.actionResponse = actionResponse;
            ItemResponse = null;
            // AtavismInventoryItem it = Inventory.Instance.GetItemByTemplateID(templateId);
            this.auction = auction;
            if (itemName != null)
            {
                if (auction.item.enchantLeval > 0)
#if AT_I2LOC_PRESET
                 itemName.text = "+" + auction.item.enchantLeval + " " + I2.Loc.LocalizationManager.GetTranslation("Items/" + auction.item.BaseName);
#else
                    itemName.text = "+" + auction.item.enchantLeval + " " + auction.item.BaseName;
#endif
                else
#if AT_I2LOC_PRESET
                 itemName.text = I2.Loc.LocalizationManager.GetTranslation("Items/" + auction.item.BaseName);
#else
                    itemName.text = auction.item.BaseName;
#endif
                if (actionResponse == null)
                {
                    if (auction.mode == 0)
                    {
#if AT_I2LOC_PRESET
                 itemName.text +=  " ( "+I2.Loc.LocalizationManager.GetTranslation("auction")+" )";
#else
                        itemName.text += " ( Auction )";
#endif
                    }
                    else
                    {
#if AT_I2LOC_PRESET
                 itemName.text +=  " ( "+I2.Loc.LocalizationManager.GetTranslation("order")+" )";
#else
                        itemName.text += " ( Order )";
#endif

                    }
                }
                itemName.color = AtavismSettings.Instance.ItemQualityColor(auction.item.quality);

            }
            if (itemQuantity != null)
            {
                if (auction.item.Count > 1)
                    itemQuantity.text = auction.item.Count.ToString();
                else
                    itemQuantity.text = "";
            }
            if (itemIcon != null)
            {
                if (auction.item.icon != null)
                    itemIcon.sprite = auction.item.icon;
                else
                    itemIcon.sprite = AtavismSettings.Instance.defaultItemIcon;
                //itemIcon.sprite = auction.item.icon;
            }
            if (itemQuality != null)
            {
                itemQuality.color = AtavismSettings.Instance.ItemQualityColor(auction.item.quality);
            }
            List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(auction.currency, auction.buyout);
            for (int i = 0; i < currencyList.Count; i++)
            {
                if (i < currencyDisplayList.Count)
                {
                    currencyList[i].gameObject.SetActive(true);
                    currencyList[i].SetCurrencyDisplayData(currencyDisplayList[i]);
                }
                else
                {
                    currencyList[i].gameObject.SetActive(false);
                }
            }
        }
        public void SetDetale(AuctionResponse itemResponse, Auction auction, bool t)
        {
            //  Debug.LogError("Auction SetDetale bn:" + auction.item.BaseName + " el:" + auction.item.enchantLeval + " c:" + auction.item.Count, gameObject);
            // this.ItemResponse = itemResponse;
            this.actionResponse = itemResponse;
            // actionResponse = null;
            // AtavismInventoryItem it = Inventory.Instance.GetItemByTemplateID(templateId);
            this.auction = auction;
            if (itemName != null)
            {
                if (auction.item.enchantLeval > 0)
#if AT_I2LOC_PRESET
                 itemName.text = "+" + auction.item.enchantLeval + " " + I2.Loc.LocalizationManager.GetTranslation("Items/" + auction.item.BaseName);
#else
                    itemName.text = "+" + auction.item.enchantLeval + " " + auction.item.BaseName;
#endif
                else
#if AT_I2LOC_PRESET
                 itemName.text = I2.Loc.LocalizationManager.GetTranslation("Items/" + auction.item.BaseName);
#else
                    itemName.text = auction.item.BaseName;
#endif
                itemName.color = AtavismSettings.Instance.ItemQualityColor(auction.item.quality);

            }
            else
            {
                Debug.LogError("UGUIAuctionSlot.itemName is not assigned", gameObject);
            }
            if (itemQuantity != null)
            {
                if (auction.count > 0)
                    itemQuantity.text = auction.count.ToString();
                else if (auction.item.Count > 1)
                    itemQuantity.text = auction.item.Count.ToString();
                else
                    itemQuantity.text = "";
            }
            else
            {
                Debug.LogError("UGUIAuctionSlot.itemQuantity is not assigned", gameObject);
            }

            if (itemIcon != null)
            {
                if (auction.item.icon != null)
                    itemIcon.sprite = auction.item.icon;
                else
                    itemIcon.sprite = AtavismSettings.Instance.defaultItemIcon;

             //   itemIcon.sprite = auction.item.icon;
            }
            else
            {
                Debug.LogError("UGUIAuctionSlot.itemIcon is not assigned", gameObject);
            }
            if (itemQuality != null)
            {
                itemQuality.color = AtavismSettings.Instance.ItemQualityColor(auction.item.quality);
            }
            else
            {
                Debug.LogError("UGUIAuctionSlot.itemQuality is not assigned", gameObject);
            }
            //  Debug.LogError("Auction SetDetale bn:" + auction.item.BaseName + " el:" + auction.item.enchantLeval + " c:" + auction.item.Count + "   currency", gameObject);

            List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(auction.currency, auction.buyout);
            if (currencyList.Count == 0)
            {
                Debug.LogError("UGUIAuctionSlot.currencyList is not assigned", gameObject);
            }
            for (int i = 0; i < currencyList.Count; i++)
            {
                if (i < currencyDisplayList.Count)
                {
                    currencyList[i].gameObject.SetActive(true);
                    currencyList[i].SetCurrencyDisplayData(currencyDisplayList[i]);
                }
                else
                {
                    currencyList[i].gameObject.SetActive(false);
                }
            }
            //  Debug.LogError("Auction SetDetale bn:" + auction.item.BaseName + " el:" + auction.item.enchantLeval + " c:" + auction.item.Count+"   end", gameObject);

        }
        public void Click()
        {
            if (ItemResponse != null)
                ItemResponse(auction.item);
            if (actionResponse != null)
                actionResponse(auction);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {

            MouseEntered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {

            MouseEntered = false;
        }
        void HideTooltip()
        {
            UGUITooltip.Instance.Hide();

        }

        public bool MouseEntered
        {
            get
            {
                return mouseEntered;
            }
            set
            {
                mouseEntered = value;
                if (mouseEntered && auction != null && auction.item != null)
                {
                    auction.item.ShowTooltip(gameObject);
                    //   cor = StartCoroutine(CheckOver());
                }
                else
                {
                    HideTooltip();
                }
            }
        }
    }
}