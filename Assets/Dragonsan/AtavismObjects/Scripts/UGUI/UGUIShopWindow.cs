using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Atavism
{
    
    public class UGUIShopWindow : MonoBehaviour
    {

        public UGUIPanelTitleBar titleBar;
        public List<UGUIMerchantItemEntry> entries;
        public Button prevButton;
        public Button nextButton;
        public Text currentPageText;
        public TextMeshProUGUI TMPCurrentPageText;
        public RectTransform purchaseCountPanel;
        public InputField purchaseCountText;
        public TMP_InputField TMPPurchaseCountText;
        public Button minusButton;
        public Button plusButton;
        //	UGUIMerchantItemSlot selectedPurchaseItem;
        int multiplePurchaseCount = 1;
        int currentPage = 0;
        bool showing = false;
        OID NpcId;
        bool sellMode = true;
        [SerializeField] GameObject panel;
        [SerializeField] Text itemName;
        [SerializeField] TextMeshProUGUI itemNameTMP;
        [SerializeField] UGUICurrency[] sumCost;

        public UGUIInventory inventory;
        [SerializeField] List<SkillTypeButton> tabButtons = new List<SkillTypeButton>();
        [SerializeField] Color buttonMenuSelectedColor = Color.green;
        [SerializeField] Color buttonMenuNormalColor = Color.white;
        [SerializeField] Color buttonMenuSelectedTextColor = Color.black;
        [SerializeField] Color buttonMenuNormalTextColor = Color.black;
        // Use this for initialization
        void Start()
        {
            if (titleBar != null)
                titleBar.SetOnPanelClose(Hide);
            AtavismEventSystem.RegisterEvent("SHOP_UPDATE", this);
            AtavismEventSystem.RegisterEvent("CLOSE_SHOP", this);
            AtavismEventSystem.RegisterEvent("ITEM_ICON_UPDATE", this);
            Hide();
            showBuy();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("SHOP_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("CLOSE_SHOP", this);
            AtavismEventSystem.UnregisterEvent("ITEM_ICON_UPDATE", this);
        }

        public void Show()
        {
            if (inventory != null)
                inventory.Show();
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            showing = true;
            currentPage = 0;
    
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("MERCHANT_UI_OPENED", args);
            if (panel != null)
                panel.SetActive(true);
            AtavismUIUtility.BringToFront(gameObject);
        }

        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            showing = false;
     
            if (purchaseCountPanel != null)
            {
                purchaseCountPanel.gameObject.SetActive(false);
            }
        }

        public void ShowPrevPage()
        {
            if (currentPage > 0)
            {
                currentPage--;
            }
            UpdatePlayerShop();
        }

        public void ShowNextPage()
        {
            currentPage++;
            UpdatePlayerShop();
        }
        public void showBuy()
        {
            sellMode = true;
            foreach (SkillTypeButton stb in tabButtons)
            {
                if (stb.typeId == 1)
                {
                    if (stb.Button)
                        stb.Button.targetGraphic.color = buttonMenuSelectedColor;
                    if (stb.ButtonText)
                        stb.ButtonText.color = buttonMenuSelectedTextColor;
                }
                else
                {
                    if (stb.Button)
                        stb.Button.targetGraphic.color = buttonMenuNormalColor;
                    if (stb.ButtonText)
                        stb.ButtonText.color = buttonMenuNormalTextColor;
                }
            }
            UpdatePlayerShop();
        }

        public void showSell()
        {
            sellMode = false;
            foreach (SkillTypeButton stb in tabButtons)
            {
                if (stb.typeId == 0)
                {
                    if (stb.Button)
                        stb.Button.targetGraphic.color = buttonMenuSelectedColor;
                    if (stb.ButtonText)
                        stb.ButtonText.color = buttonMenuSelectedTextColor;
                }
                else
                {
                    if (stb.Button)
                        stb.Button.targetGraphic.color = buttonMenuNormalColor;
                    if (stb.ButtonText)
                        stb.ButtonText.color = buttonMenuNormalTextColor;
                }
            }
            UpdatePlayerShop();
        }
        void UpdatePlayerShop()
        {
            if (sellMode)
            {

                for (int i = 0; i < entries.Count; i++)
                {
                    if ((currentPage * entries.Count) + i < AtavismPlayerShop.Instance.PlayerShopSellItems.Count)
                    {
                        entries[i].gameObject.SetActive(true);
                        entries[i].UpdateShopItemData(AtavismPlayerShop.Instance.GetSellItem((currentPage * entries.Count) + i), this);
                    }
                    else
                    {
                        entries[i].gameObject.SetActive(false);
                    }
                }
                if (currentPageText != null)
                    if (AtavismPlayerShop.Instance.PlayerShopSellItems.Count > entries.Count)
                        currentPageText.text = (currentPage + 1) + " / " + ((AtavismPlayerShop.Instance.PlayerShopSellItems.Count % entries.Count) > 0 ? (AtavismPlayerShop.Instance.PlayerShopSellItems.Count / entries.Count + 1).ToString() : (AtavismPlayerShop.Instance.PlayerShopSellItems.Count / entries.Count).ToString());
                    else
                        currentPageText.text = "";
                if (TMPCurrentPageText != null)
                    if (AtavismPlayerShop.Instance.PlayerShopSellItems.Count > entries.Count)
                        TMPCurrentPageText.text = (currentPage + 1) + " / " + ((AtavismPlayerShop.Instance.PlayerShopSellItems.Count % entries.Count) > 0 ? (AtavismPlayerShop.Instance.PlayerShopSellItems.Count / entries.Count + 1).ToString() : (AtavismPlayerShop.Instance.PlayerShopSellItems.Count / entries.Count).ToString());
                    else
                        TMPCurrentPageText.text = "1/1";
            }
            else
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    if ((currentPage * entries.Count) + i < AtavismPlayerShop.Instance.PlayerShopBuyItems.Count)
                    {
                        entries[i].gameObject.SetActive(true);
                        entries[i].UpdateShopItemData(AtavismPlayerShop.Instance.GetBuyItem((currentPage * entries.Count) + i), this);
                    }
                    else
                    {
                        entries[i].gameObject.SetActive(false);
                    }
                }
                if (currentPageText != null)
                    if (AtavismPlayerShop.Instance.PlayerShopBuyItems.Count > entries.Count)
                        currentPageText.text = (currentPage + 1) + " / " + ((AtavismPlayerShop.Instance.PlayerShopBuyItems.Count % entries.Count) > 0 ? (AtavismPlayerShop.Instance.PlayerShopBuyItems.Count / entries.Count + 1).ToString() : (AtavismPlayerShop.Instance.PlayerShopBuyItems.Count / entries.Count).ToString());
                    else
                        currentPageText.text = "";
                if (TMPCurrentPageText != null)
                    if (AtavismPlayerShop.Instance.PlayerShopBuyItems.Count > entries.Count)
                        TMPCurrentPageText.text = (currentPage + 1) + " / " + ((AtavismPlayerShop.Instance.PlayerShopBuyItems.Count % entries.Count) > 0 ? (AtavismPlayerShop.Instance.PlayerShopBuyItems.Count / entries.Count + 1).ToString() : (AtavismPlayerShop.Instance.PlayerShopBuyItems.Count / entries.Count).ToString());
                    else
                        TMPCurrentPageText.text = "1/1";
            }
            // Update visibility of prev and next buttons
            if (currentPage > 0)
            {
                if(prevButton)
                prevButton.gameObject.SetActive(true);
            }
            else
            {
                if (prevButton)
                    prevButton.gameObject.SetActive(false);
            }
            if (sellMode)
            {
                if (AtavismPlayerShop.Instance.PlayerShopSellItems.Count > (currentPage + 1) * entries.Count)
                {
                    if (nextButton)
                        nextButton.gameObject.SetActive(true);
                }
                else
                {
                    if (nextButton)
                        nextButton.gameObject.SetActive(false);
                }
            }
            else
            {
                if (AtavismPlayerShop.Instance.PlayerShopBuyItems.Count > (currentPage + 1) * entries.Count)
                {
                    if (nextButton)
                        nextButton.gameObject.SetActive(true);
                }
                else
                {
                    if (nextButton)
                        nextButton.gameObject.SetActive(false);
                }
            }
            if (purchaseCountPanel != null)
            {
                purchaseCountPanel.gameObject.SetActive(false);
            }
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "SHOP_UPDATE")
            {
                if (!showing)
                    Show();
               
                   currentPage = 0;

                if (AtavismPlayerShop.Instance.PlayerShopBuyItems.Count > 0)
                    showSell();
                else
                    showBuy();
                UpdatePlayerShop();
            }
            else if (eData.eventType == "ITEM_ICON_UPDATE")
            {
                if (showing)
                    UpdatePlayerShop();
            }
            else if (eData.eventType == "CLOSE_SHOP")
            {
                Hide();
                
            }
        }
        ShopItem shopItem;

        public bool ShowPurchaseCountPanel(UGUIMerchantItemSlot mItemEntry, ShopItem shopItem)
        {
            this.shopItem = shopItem;
            if (purchaseCountPanel != null)
            {
                purchaseCountPanel.gameObject.SetActive(true);
                multiplePurchaseCount = 1;
                if (purchaseCountText != null)
                {
                    purchaseCountText.text = multiplePurchaseCount.ToString();
                }
                if (TMPPurchaseCountText != null)
                {
                    TMPPurchaseCountText.text = multiplePurchaseCount.ToString();
                }
                if (minusButton != null)
                    minusButton.gameObject.SetActive(false);
                //	selectedPurchaseItem = mItemEntry;
                AtavismInventoryItem aii = Inventory.Instance.GetItemByTemplateID(shopItem.itemID);
                
                if (itemName != null && aii != null)
                {
#if AT_I2LOC_PRESET
                itemName.text = string.IsNullOrEmpty(I2.Loc.LocalizationManager.GetTranslation("Items/" + aii.name)) ? aii.name : I2.Loc.LocalizationManager.GetTranslation("Items/" + aii.name);
#else
                    itemName.text = aii.name;
#endif
                }
                if (itemNameTMP != null && aii != null)
                {
#if AT_I2LOC_PRESET
                itemNameTMP.text = string.IsNullOrEmpty(I2.Loc.LocalizationManager.GetTranslation("Items/" + aii.name)) ? aii.name : I2.Loc.LocalizationManager.GetTranslation("Items/" + aii.name);
#else
                    itemNameTMP.text = aii.name;
#endif
                }

                List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(shopItem.purchaseCurrency, shopItem.cost * multiplePurchaseCount);
                for (int i = 0; i < sumCost.Length; i++)
                {
                    if (i < currencyDisplayList.Count)
                    {
                        sumCost[i].gameObject.SetActive(true);
                        sumCost[i].SetCurrencyDisplayData(currencyDisplayList[currencyDisplayList.Count - i - 1]);
                    }
                    else
                    {
                        sumCost[i].gameObject.SetActive(false);
                    }
                }
                return true;
            }
            return false;
        }

        public void ReduceMultipleCount()
        {
            multiplePurchaseCount--;
            if (multiplePurchaseCount < 2)
            {
                multiplePurchaseCount = 1;
                if (minusButton != null)
                    minusButton.gameObject.SetActive(false);
            }
            if (purchaseCountText != null)
            {
                purchaseCountText.text = multiplePurchaseCount.ToString();
            }
            if (TMPPurchaseCountText != null)
            {
                TMPPurchaseCountText.text = multiplePurchaseCount.ToString();
            }
            List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(shopItem.purchaseCurrency, shopItem.cost * multiplePurchaseCount);
            for (int i = 0; i < sumCost.Length; i++)
            {
                if (i < currencyDisplayList.Count)
                {
                    sumCost[i].gameObject.SetActive(true);
                    sumCost[i].SetCurrencyDisplayData(currencyDisplayList[currencyDisplayList.Count - i - 1]);
                }
                else
                {
                    sumCost[i].gameObject.SetActive(false);
                }
            }

        }

        public void IncreaseMultipleCount()
        {
            multiplePurchaseCount++;

            int cItem = Inventory.Instance.GetCountOfItem(shopItem.itemID);
            if (multiplePurchaseCount > cItem)
            {
                multiplePurchaseCount = cItem;
            }

            if (minusButton != null)
                minusButton.gameObject.SetActive(true);
            if (purchaseCountText != null)
            {
                purchaseCountText.text = multiplePurchaseCount.ToString();
            }
            if (TMPPurchaseCountText != null)
            {
                TMPPurchaseCountText.text = multiplePurchaseCount.ToString();
            }
            List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(shopItem.purchaseCurrency, shopItem.cost * multiplePurchaseCount);
            for (int i = 0; i < sumCost.Length; i++)
            {
                if (i < currencyDisplayList.Count)
                {
                    sumCost[i].gameObject.SetActive(true);
                    sumCost[i].SetCurrencyDisplayData(currencyDisplayList[currencyDisplayList.Count - i - 1]);
                }
                else
                {
                    sumCost[i].gameObject.SetActive(false);
                }
            }

        }

        public void UpdatePurchaseCount()
        {
            if (purchaseCountText != null)
            {
                if (purchaseCountText.text != "")
                    multiplePurchaseCount = int.Parse(purchaseCountText.text);
                else
                    multiplePurchaseCount = 0;
                if (minusButton != null)
                    if (multiplePurchaseCount > 1)
                    {
                        if (!minusButton.gameObject.activeSelf)
                            minusButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        if (minusButton.gameObject.activeSelf)
                            minusButton.gameObject.SetActive(false);
                    }
            }
            if (TMPPurchaseCountText != null)
            {
                if (TMPPurchaseCountText.text != "")
                    multiplePurchaseCount = int.Parse(TMPPurchaseCountText.text);
                else
                    multiplePurchaseCount = 0;
                if (minusButton != null)
                    if (multiplePurchaseCount > 1)
                    {
                        if (!minusButton.gameObject.activeSelf)
                            minusButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        if (minusButton.gameObject.activeSelf)
                            minusButton.gameObject.SetActive(false);
                    }
            }

            List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(shopItem.purchaseCurrency, shopItem.cost * multiplePurchaseCount);
            for (int i = 0; i < sumCost.Length; i++)
            {
                if (i < currencyDisplayList.Count)
                {
                    sumCost[i].gameObject.SetActive(true);
                    sumCost[i].SetCurrencyDisplayData(currencyDisplayList[currencyDisplayList.Count - i - 1]);
                }
                else
                {
                    sumCost[i].gameObject.SetActive(false);
                }
            }
        }

        public void PurchaseMultiple()
        {
            if (purchaseCountText != null)
            {
                //			int count = int.Parse(purchaseCountText.text);
                ShopItem mItem = new ShopItem();
                mItem.itemID = shopItem.itemID;
                mItem.sellCount = int.Parse(purchaseCountText.text);
                PurchaseItemConfirmed(mItem, true);
               shopItem = null;
                if (purchaseCountPanel.gameObject.activeSelf)
                    purchaseCountPanel.gameObject.SetActive(false);
            }
            if (TMPPurchaseCountText != null)
            {
                //			int count = int.Parse(purchaseCountText.text);
                ShopItem mItem = new ShopItem();
                mItem.itemID = shopItem.itemID;
                mItem.sellCount = int.Parse(TMPPurchaseCountText.text);
                PurchaseItemConfirmed(mItem, true);
                shopItem = null;
                if (purchaseCountPanel.gameObject.activeSelf)
                    purchaseCountPanel.gameObject.SetActive(false);
            }
        }

        public void CancelPurchase()
        {
            if (purchaseCountPanel != null)
            {
                if (purchaseCountPanel.gameObject.activeSelf)
                    purchaseCountPanel.gameObject.SetActive(false);
            }
            shopItem = null;
        }

    
        public void PurchaseItemConfirmed(object item, bool response)
        {
         //   Debug.LogError("PurchaseItemConfirmed  ");
            if (!response)
                return;

            ShopItem mItem = (ShopItem)item;
            
       
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("shop", AtavismPlayerShop.Instance.Shop);
            if (mItem.itemOID != null)
            {
                props.Add("ItemId", -1);
                props.Add("ItemOid", mItem.itemOID);
                props.Add("ItemCount",-1);
            }
            else
            {
                props.Add("ItemId", mItem.itemID);
                props.Add("ItemOid", OID.fromLong(0L));
                props.Add("ItemCount", mItem.sellCount);
            }
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.PLAYER_SHOP_BUY", props);
         //   Debug.LogError("PurchaseItemConfirmed  END");
        }
    }
}