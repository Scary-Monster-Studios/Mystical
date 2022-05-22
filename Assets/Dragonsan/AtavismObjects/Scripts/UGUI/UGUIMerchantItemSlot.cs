using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Atavism
{

    public class UGUIMerchantItemSlot : UGUIDraggableSlot
    {

        UGUIMerchantFrame merchantFrame;
        UGUIShopWindow playerShop;
        MerchantItem merchantItem;
        ShopItem shopItem;

        bool mouseEntered = false;
        Color defaultColour = Color.white;
        public Image itemIcon;

        // Use this for initialization
        void Start()
        {
            slotBehaviour = DraggableBehaviour.SourceOnly;
        }

        public void UpdateMerchantItemData(MerchantItem merchantItem, UGUIMerchantFrame merchantFrame)
        {
            this.merchantItem = merchantItem;
            this.merchantFrame = merchantFrame;
            this.playerShop = null;
            if (merchantItem == null)
            {
                if (uguiActivatable != null)
                {
                    Destroy(uguiActivatable.gameObject);
                }
            }
            else
            {
                AtavismInventoryItem item = Inventory.Instance.GetItemByTemplateID(merchantItem.itemID);
                if (uguiActivatable == null)
                {
                    if (AtavismSettings.Instance.inventoryItemPrefab != null)
                        uguiActivatable = (UGUIAtavismActivatable)Instantiate(AtavismSettings.Instance.inventoryItemPrefab, transform, false);
                    else
                        uguiActivatable = (UGUIAtavismActivatable)Instantiate(Inventory.Instance.uguiAtavismItemPrefab, transform, false);
                    //	uguiActivatable.transform.SetParent(transform, false);
                    uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                    if (uguiActivatable.GetComponent<RectTransform>().anchorMin == Vector2.zero && uguiActivatable.GetComponent<RectTransform>().anchorMax == Vector2.one)
                        uguiActivatable.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                    uguiActivatable.SetActivatable(item, ActivatableType.Item, this, false);
                    defaultColour = uguiActivatable.GetComponent<Image>().color;
                }

                uguiActivatable.SetActivatable(item, ActivatableType.Item, this, false);
                defaultColour = AtavismSettings.Instance.ItemQualityColor(item.quality);
                if (merchantItem.count > 0)
                {
                    if (uguiActivatable.countText != null)
                        uguiActivatable.countText.text = merchantItem.count.ToString();
                    if (uguiActivatable.TMPCountText != null)
                        uguiActivatable.TMPCountText.text = merchantItem.count.ToString();
                }
                else
                {
                    if (uguiActivatable.countText != null)
                        uguiActivatable.countText.text = "";
                    if (uguiActivatable.TMPCountText != null)
                        uguiActivatable.TMPCountText.text = "";
                }
                if (merchantItem.count == 0)
                {
                    uguiActivatable.GetComponent<Image>().color = Color.gray;
                }
                else
                {
                    uguiActivatable.GetComponent<Image>().color = defaultColour;
                }

                // Set background Image - HACK to still show item when it is being dragged
                if (GetComponent<Image>() != null)
                {
                    GetComponent<Image>().color = AtavismSettings.Instance.ItemQualityColor(item.quality);
                }
                if (itemIcon != null)
                {
                    if (item.icon != null)
                        itemIcon.sprite = item.icon;
                    else
                        itemIcon.sprite = AtavismSettings.Instance.defaultItemIcon;
                }
            }
        }

        public void UpdateShopItemData(ShopItem shopItem, UGUIShopWindow playerShop)
        {
            this.shopItem = shopItem;
            this.playerShop = playerShop;
            this.merchantFrame = null;
            if (shopItem == null)
            {
                if (uguiActivatable != null)
                {
                    Destroy(uguiActivatable.gameObject);
                }
            }
            else
            {
                AtavismInventoryItem item = shopItem.item;
                    //Inventory.Instance.GetItemByTemplateID(shopItem.itemID);
                 if (item.icon == null)
                      item.icon = Inventory.Instance.GetItemByTemplateID(item.templateId).icon;
                if (uguiActivatable == null)
                {
                    if (AtavismSettings.Instance.inventoryItemPrefab != null)
                        uguiActivatable = (UGUIAtavismActivatable)Instantiate(AtavismSettings.Instance.inventoryItemPrefab, transform, false);
                    else
                        uguiActivatable = (UGUIAtavismActivatable)Instantiate(Inventory.Instance.uguiAtavismItemPrefab, transform, false);
                    //	uguiActivatable.transform.SetParent(transform, false);
                    uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                    if (uguiActivatable.GetComponent<RectTransform>().anchorMin == Vector2.zero && uguiActivatable.GetComponent<RectTransform>().anchorMax == Vector2.one)
                        uguiActivatable.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                    uguiActivatable.SetActivatable(item, ActivatableType.Item, this, false);
                    defaultColour = uguiActivatable.GetComponent<Image>().color;
                }

                uguiActivatable.SetActivatable(item, ActivatableType.Item, this, false);
                defaultColour = AtavismSettings.Instance.ItemQualityColor(item.quality);
                if (shopItem.count > 0)
                {
                    if (uguiActivatable.countText != null)
                        uguiActivatable.countText.text = shopItem.count.ToString();
                    if (uguiActivatable.TMPCountText != null)
                        uguiActivatable.TMPCountText.text = shopItem.count.ToString();
                }
                else
                {
                    if (uguiActivatable.countText != null)
                        uguiActivatable.countText.text = "";
                    if (uguiActivatable.TMPCountText != null)
                        uguiActivatable.TMPCountText.text = "";
                }
                if (shopItem.count == 0)
                {
                    uguiActivatable.GetComponent<Image>().color = Color.gray;
                }
                else
                {
                    uguiActivatable.GetComponent<Image>().color = defaultColour;
                }

                // Set background Image - HACK to still show item when it is being dragged
                if (GetComponent<Image>() != null)
                {
                    GetComponent<Image>().color = AtavismSettings.Instance.ItemQualityColor(item.quality);
                }
                if (itemIcon != null)
                {
                    if (item.icon != null)
                        itemIcon.sprite = Inventory.Instance.GetItemByTemplateID(item.templateId).icon;
                    else
                        itemIcon.sprite = AtavismSettings.Instance.defaultItemIcon;
                }
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            MouseEntered = true;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            MouseEntered = false;
        }

        public override void OnDrop(PointerEventData eventData)
        {
            // Do nothing
        }

        public override void ClearChildSlot()
        {
            if (uguiActivatable != null)
                Destroy(uguiActivatable.gameObject);
            uguiActivatable = null;
        }

        public override void Discarded()
        {

        }

        public override void Activate()
        {
            if (shopItem != null)
            {
                string costString = "";
                if (shopItem.itemOID == null)
                {
                    int cItem = Inventory.Instance.GetCountOfItem(shopItem.itemID);
                    if (cItem == 0)
                    {
                        return;
                    }
                }
            }

            //if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                if (playerShop != null && shopItem.itemOID==null && playerShop.ShowPurchaseCountPanel(this, shopItem))
                    return;

                if (merchantFrame!=null && merchantFrame.ShowPurchaseCountPanel(this, merchantItem))
                    return;
            }
            StartPurchase(1);
        }

        public void StartPurchase(int count)
        {
            string confirmationString = "";
            if (merchantItem != null)
            {

                MerchantItem mItem = new MerchantItem();
                mItem.itemID = merchantItem.itemID;
                mItem.count = count;
                AtavismInventoryItem aiItem = Inventory.Instance.GetItemByTemplateID(merchantItem.itemID);
                string costString = Inventory.Instance.GetCostString(merchantItem.purchaseCurrency, merchantItem.cost * mItem.count);
                
                if (mItem.count == 1)
#if AT_I2LOC_PRESET
        confirmationString = I2.Loc.LocalizationManager.GetTranslation("Purchase1Item") + " " + I2.Loc.LocalizationManager.GetTranslation("Items/" + aiItem.name) + " " + I2.Loc.LocalizationManager.GetTranslation("for") + " " + costString + "?";
#else
                    confirmationString = "Purchase " + aiItem.name + " for " + costString + "?";
#endif
                else
#if AT_I2LOC_PRESET
	    confirmationString = I2.Loc.LocalizationManager.GetTranslation("PurchaseXItems") + " " + I2.Loc.LocalizationManager.GetTranslation("Items/" + aiItem.name) + " (x" + mItem.count + ") " + I2.Loc.LocalizationManager.GetTranslation("for") + " " + costString + "?";
#else
                    confirmationString = "Purchase " + aiItem.name + " (x" + mItem.count + ") for " + costString + "?";
#endif
                UGUIConfirmationPanel.Instance.ShowConfirmationBox(confirmationString, mItem, NpcInteraction.Instance.PurchaseItemConfirmed);
            }




            if (shopItem != null)
            {
                string costString = "";
                if (shopItem.itemOID == null)
                {
                    int cItem = Inventory.Instance.GetCountOfItem(shopItem.itemID);
                    if (cItem == 0)
                    {

                        return;
                    }
                    shopItem.sellCount = count;
                    costString = Inventory.Instance.GetCostString(shopItem.purchaseCurrency, shopItem.cost * shopItem.sellCount);

                    if (shopItem.sellCount == 1)
                    {
#if AT_I2LOC_PRESET
        confirmationString = I2.Loc.LocalizationManager.GetTranslation("Sell") + " " + I2.Loc.LocalizationManager.GetTranslation("Items/" + shopItem.item.name) + " " + I2.Loc.LocalizationManager.GetTranslation("for") + " " + costString + "?";
#else
                        confirmationString = "Sell " + shopItem.item.name + " for " + costString + "?";
#endif
                    }
                    else
                    {

#if AT_I2LOC_PRESET
	    confirmationString = I2.Loc.LocalizationManager.GetTranslation("Sell") + " " + I2.Loc.LocalizationManager.GetTranslation("Items/" + shopItem.item.name) + " (x" + shopItem.sellCount + ") " + I2.Loc.LocalizationManager.GetTranslation("for") + " " + costString + "?";
#else
                        confirmationString = "Sell " + shopItem.item.name + " (x" + shopItem.sellCount + ") for " + costString + "?";
#endif
                    }


                }
                else
                {
                    costString = Inventory.Instance.GetCostString(shopItem.purchaseCurrency, shopItem.cost);

                    if (shopItem.count == 1)
                    {

#if AT_I2LOC_PRESET
        confirmationString = I2.Loc.LocalizationManager.GetTranslation("Purchase1Item") + " " + I2.Loc.LocalizationManager.GetTranslation("Items/" + shopItem.item.name) + " " + I2.Loc.LocalizationManager.GetTranslation("for") + " " + costString + "?";
#else
                        confirmationString = "Purchase " + shopItem.item.name + " for " + costString + "?";
#endif
                    }
                    else
                    {

#if AT_I2LOC_PRESET
	    confirmationString = I2.Loc.LocalizationManager.GetTranslation("PurchaseXItems") + " " + I2.Loc.LocalizationManager.GetTranslation("Items/" + shopItem.item.name) + " (x" + shopItem.count + ") " + I2.Loc.LocalizationManager.GetTranslation("for") + " " + costString + "?";
#else
                        confirmationString = "Purchase " + shopItem.item.name + " (x" + shopItem.count + ") for " + costString + "?";
#endif
                    }
                }
                UGUIConfirmationPanel.Instance.ShowConfirmationBox(confirmationString, shopItem, playerShop.PurchaseItemConfirmed);
            }
        }

        protected override void ShowTooltip()
        {
        }

        void HideTooltip()
        {
            UGUITooltip.Instance.Hide();
            if (cor != null)
                StopCoroutine(cor);
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
                if (mouseEntered && uguiActivatable != null)
                {
                    uguiActivatable.ShowTooltip(gameObject);
                    cor = StartCoroutine(CheckOver());
                }
                else
                {
                    HideTooltip();
                }
            }
        }
    }
}