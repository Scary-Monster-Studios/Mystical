using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

namespace Atavism
{

    //public delegate void InventoryAuctionResponse(AtavismInventoryItem item);

    public class UGUIInventoryAuctionSlot : UGUIDraggableSlot
    {

        public int bagNum;
        public Text countLabel;
        public TextMeshProUGUI TMPCountLabel;
        AtavismInventoryItem item;
        public Image itemIcon;
        public Image hover;
        bool mouseEntered = false;
        InventoryAuctionResponse response;
        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("ITEM_ICON_UPDATE", this);
            slotBehaviour = DraggableBehaviour.Standard;
        }
        void Update()
        {
            if (this.item == null)
            {
                if (itemIcon != null)
                    itemIcon.enabled = false;
            }
        }

          
        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("ITEM_ICON_UPDATE", this);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "ITEM_ICON_UPDATE")
            {
                if (item != null)
                {
                    item.icon = AtavismPrefabManager.Instance.GetItemIconByID(item.templateId);
                    itemIcon.sprite = item.icon;
                }
                if (this.itemIcon != null)
                {
                    itemIcon.enabled = true;
                    if (item.icon != null)
                        itemIcon.sprite = item.icon;
                    else
                        itemIcon.sprite = AtavismSettings.Instance.defaultItemIcon;
                }
            }
        }


        /// <summary>
        /// Creates a UGUIAtavismActivatable object to put in this slot if the item is not null.
        /// </summary>
        /// <param name="item">Item.</param>
        public void UpdateInventoryItemData(AtavismInventoryItem item/*, InventoryAuctionResponse response*/)
        {
          //  this.response = response;
            this.item = item;
            if (item == null)
            {
                if (uguiActivatable != null)
                {
                    Destroy(uguiActivatable.gameObject);
                    uguiActivatable = null;
                    if (itemIcon != null)
                        itemIcon.enabled = false;
                    if (mouseEntered)
                        mouseEntered = false;

                }
            }
            else
            {
                if (uguiActivatable == null)
                {
                    if (AtavismSettings.Instance.inventoryItemPrefab != null)
                        uguiActivatable = (UGUIAtavismActivatable)Instantiate(AtavismSettings.Instance.inventoryItemPrefab, transform, false);
                    else
                        uguiActivatable = (UGUIAtavismActivatable)Instantiate(Inventory.Instance.uguiAtavismItemPrefab, transform, false);
                    //uguiActivatable.transform.SetParent(transform, false);
                    uguiActivatable.transform.localScale = Vector3.one;
                    if (uguiActivatable.GetComponent<RectTransform>().anchorMin == Vector2.zero && uguiActivatable.GetComponent<RectTransform>().anchorMax == Vector2.one)
                        uguiActivatable.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                    uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                }
                if (itemIcon != null)
                {
                    itemIcon.enabled = true;
                    if (item.icon != null)
                        itemIcon.sprite = item.icon;
                    else
                        itemIcon.sprite = AtavismSettings.Instance.defaultItemIcon;

                  //  itemIcon.sprite = item.icon;
                }
                uguiActivatable.SetActivatable(item, ActivatableType.Item, this);
                if (mouseEntered)
                    uguiActivatable.ShowTooltip(gameObject);
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                UGUIAtavismActivatable dActivatable = eventData.pointerDrag.GetComponent<UGUIAtavismActivatable>();
                if (dActivatable != null)
                    if (dActivatable.Source == this)
                    {
                        if (hover != null)
                        {
                            hover.enabled = true;
                            hover.color = AtavismSettings.Instance.itemDropColorFalse;
                        }
                    }
                    else
                    {
                        if (uguiActivatable != null)
                        {
                            if (uguiActivatable.hover != null)
                            {
                                uguiActivatable.hover.enabled = true;
                                uguiActivatable.hover.color = AtavismSettings.Instance.itemDropColorTrue;
                            }
                        }
                        else
                        {
                            if (hover != null)
                            {
                                hover.enabled = true;
                                hover.color = AtavismSettings.Instance.itemDropColorTrue;
                            }
                        }
                    }
            }
            MouseEntered = true;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (uguiActivatable != null)
            {
                if (uguiActivatable.hover != null)
                    uguiActivatable.hover.enabled = false;
            }
            if (hover != null)
                hover.enabled = false;
            MouseEntered = false;
        }

        public override void OnDrop(PointerEventData eventData)
        {
            UGUIAtavismActivatable droppedActivatable = eventData.pointerDrag.GetComponent<UGUIAtavismActivatable>();
            droppedActivatable.PreventDiscard();
          
        }

        public override void ClearChildSlot()
        {
            uguiActivatable = null;
            if (itemIcon != null)
                itemIcon.enabled = false;
        }

        public override void Discarded()
        {
#if AT_I2LOC_PRESET
        UGUIConfirmationPanel.Instance.ShowConfirmationBox(I2.Loc.LocalizationManager.GetTranslation("DeleteItemPopup") + " " + I2.Loc.LocalizationManager.GetTranslation("Items/" + item.name) + "?", item, Inventory.Instance.DeleteItemStack);
#else
            UGUIConfirmationPanel.Instance.ShowConfirmationBox("Delete " + item.name + "?", item, Inventory.Instance.DeleteItemStack);
#endif
        }

        public override void Activate()
        {
            if (item == null)
                return;
            if (response != null)
                response(item);
            //  Debug.LogError("AuctionSlot Activate");


            /*if (!AtavismCursor.Instance.HandleUGUIActivatableUseOverride(uguiActivatable)) {
                item.Activate();
            }*/
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