using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Atavism
{

    public class UGUIMerchantFrame : MonoBehaviour
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

        [SerializeField] GameObject panel;
        [SerializeField] Text itemName;
        [SerializeField] TextMeshProUGUI itemNameTMP;
        [SerializeField] UGUICurrency[] sumCost;

        public UGUIInventory inventory;

        // Use this for initialization
        void Start()
        {
            if (titleBar != null)
                titleBar.SetOnPanelClose(Hide);
            AtavismEventSystem.RegisterEvent("MERCHANT_UPDATE", this);
            AtavismEventSystem.RegisterEvent("CLOSE_NPC_DIALOGUE", this);
            AtavismEventSystem.RegisterEvent("ITEM_ICON_UPDATE", this);
            Hide();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("MERCHANT_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("CLOSE_NPC_DIALOGUE", this);
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
            AtavismCursor.Instance.SetUGUIActivatableClickedOverride(SellItemToMerchant);

            if (titleBar != null)
                titleBar.SetPanelTitle(ClientAPI.GetObjectNode(NpcInteraction.Instance.NpcId.ToLong()).Name);

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
            if (AtavismCursor.Instance != null)
                AtavismCursor.Instance.ClearUGUIActivatableClickedOverride(SellItemToMerchant);

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
            UpdateMerchantFrame();
        }

        public void ShowNextPage()
        {
            currentPage++;
            UpdateMerchantFrame();
        }

        void UpdateMerchantFrame()
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if ((currentPage * entries.Count) + i < NpcInteraction.Instance.MerchantItems.Count)
                {
                    entries[i].gameObject.SetActive(true);
                    entries[i].UpdateMerchantItemData(NpcInteraction.Instance.GetMerchantItem((currentPage * entries.Count) + i), this);
                }
                else
                {
                    entries[i].gameObject.SetActive(false);
                }
            }
            if (currentPageText != null)
                if (NpcInteraction.Instance.MerchantItems.Count > entries.Count)
                    currentPageText.text = (currentPage + 1) + " / " + ((NpcInteraction.Instance.MerchantItems.Count % entries.Count) > 0 ? (NpcInteraction.Instance.MerchantItems.Count / entries.Count + 1).ToString() : (NpcInteraction.Instance.MerchantItems.Count / entries.Count).ToString());
                else
                    currentPageText.text = "";
            if (TMPCurrentPageText != null)
                if (NpcInteraction.Instance.MerchantItems.Count > entries.Count)
                    TMPCurrentPageText.text = (currentPage + 1) + " / " + ((NpcInteraction.Instance.MerchantItems.Count % entries.Count) > 0 ? (NpcInteraction.Instance.MerchantItems.Count / entries.Count + 1).ToString() : (NpcInteraction.Instance.MerchantItems.Count / entries.Count).ToString());
                else
                    TMPCurrentPageText.text = "";

            // Update visibility of prev and next buttons
            if (currentPage > 0)
            {
                prevButton.gameObject.SetActive(true);
            }
            else
            {
                prevButton.gameObject.SetActive(false);
            }

            if (NpcInteraction.Instance.MerchantItems.Count > (currentPage + 1) * entries.Count)
            {
                nextButton.gameObject.SetActive(true);
            }
            else
            {
                nextButton.gameObject.SetActive(false);
            }

            if (purchaseCountPanel != null)
            {
                purchaseCountPanel.gameObject.SetActive(false);
            }
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "MERCHANT_UPDATE")
            {
                if (!showing)
                    Show();
                if (eData.eventArgs[0] != null)
                {
                    if (NpcId != OID.fromString(eData.eventArgs[0]))
                    {
                        NpcId = OID.fromString(eData.eventArgs[0]);
                        currentPage = 0;
                    }
                }
                UpdateMerchantFrame();
            }
            else if (eData.eventType == "ITEM_ICON_UPDATE")
            {
                if (showing)
                    UpdateMerchantFrame();
            }
            else if (eData.eventType == "CLOSE_NPC_DIALOGUE")
            {
                Hide();
                NpcId = null;
            }
        }
        MerchantItem merchantItem;

        public bool ShowPurchaseCountPanel(UGUIMerchantItemSlot mItemEntry, MerchantItem merchantItem)
        {
            this.merchantItem = merchantItem;
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
                AtavismInventoryItem aii = Inventory.Instance.GetItemByTemplateID(merchantItem.itemID);
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

                List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(merchantItem.purchaseCurrency, merchantItem.cost * multiplePurchaseCount);
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
            List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(merchantItem.purchaseCurrency, merchantItem.cost * multiplePurchaseCount);
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
            List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(merchantItem.purchaseCurrency, merchantItem.cost * multiplePurchaseCount);
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

            List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(merchantItem.purchaseCurrency, merchantItem.cost * multiplePurchaseCount);
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
                MerchantItem mItem = new MerchantItem();
                mItem.itemID = merchantItem.itemID;
                mItem.count = int.Parse(purchaseCountText.text);
                NpcInteraction.Instance.PurchaseItemConfirmed(mItem, true);
                merchantItem = null;
                if (purchaseCountPanel.gameObject.activeSelf)
                    purchaseCountPanel.gameObject.SetActive(false);
            }
            if (TMPPurchaseCountText != null)
            {
                //			int count = int.Parse(purchaseCountText.text);
                MerchantItem mItem = new MerchantItem();
                mItem.itemID = merchantItem.itemID;
                mItem.count = int.Parse(TMPPurchaseCountText.text);
                NpcInteraction.Instance.PurchaseItemConfirmed(mItem, true);
                merchantItem = null;
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
            merchantItem = null;
        }

        public void SellItemToMerchant(UGUIAtavismActivatable activatable)
        {
            NpcInteraction.Instance.SellItemToMerchant((AtavismInventoryItem)activatable.ActivatableObject);
        }
    }
}