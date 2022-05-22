using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUIInventorySlot : UGUIDraggableSlot
    {

        public int bagNum;
        public Text countLabel;
        public TextMeshProUGUI TMPCountLabel;
        AtavismInventoryItem item;
        public Image itemIcon;
        public Image hover;
        bool mouseEntered = false;

        // Use this for initialization
        void Start()
        {
            slotBehaviour = DraggableBehaviour.Standard;
            AtavismEventSystem.RegisterEvent("ITEM_RELOAD", this);
        }
        
        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("ITEM_RELOAD", this);
        }

        public void OnEvent(AtavismEventData eData)
        {
            AtavismLogger.LogDebugMessage("UGUIInventorySlot " + eData.eventType );

            if (eData.eventType == "ITEM_RELOAD")
            {
                if (item != null)
                {
                    item = AtavismPrefabManager.Instance.LoadItem(item);
                    UpdateInventoryItemData(item);
                }
            }
        }

        void Update()
        {
            if (this.item == null)
            {
                if (itemIcon != null)
                    itemIcon.enabled = false;
            }
        }

        /// <summary>
        /// Creates a UGUIAtavismActivatable object to put in this slot if the item is not null.
        /// </summary>
        /// <param name="item">Item.</param>
        public void UpdateInventoryItemData(AtavismInventoryItem item)
        {
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
                //   Debug.LogError("UpdateInventoryItemData: item nit null uguiActivatable:"+ uguiActivatable);
                if (uguiActivatable == null)
                {
                    if (AtavismSettings.Instance.inventoryItemPrefab != null)
                    {
                        //    Debug.LogError("UpdateInventoryItemData: item nit null AtavismSettings");

                        uguiActivatable = (UGUIAtavismActivatable)Instantiate(AtavismSettings.Instance.inventoryItemPrefab, transform, false);
                    }
                    else
                    {
                        //    Debug.LogError("UpdateInventoryItemData: item nit null Inentory");

                        uguiActivatable = (UGUIAtavismActivatable)Instantiate(Inventory.Instance.uguiAtavismItemPrefab, transform, false);
                    }
                    //uguiActivatable.transform.SetParent(transform, false);
                    uguiActivatable.transform.localScale = Vector3.one;
                    if (uguiActivatable.GetComponent<RectTransform>().anchorMin == Vector2.zero && uguiActivatable.GetComponent<RectTransform>().anchorMax == Vector2.one)
                    {
                        //   Debug.LogError("UpdateInventoryItemData: item not null 0 0",gameObject);

                        uguiActivatable.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                    }
                    else
                    {
                        //   Debug.LogError("UpdateInventoryItemData: item not null 0 0 not",gameObject);
                        //  uguiActivatable.GetComponent<RectTransform>().sizeDelta = uguiActivatable.defaultSlotSize;
                    }
                    uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                }
                if (itemIcon != null)
                {
                    itemIcon.enabled = true;
                    if (item.icon != null)
                        itemIcon.sprite = item.icon;
                    else
                        itemIcon.sprite = AtavismSettings.Instance.defaultItemIcon;
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

            // Don't allow reference or temporary slots, or non Item/bag slots
            if (droppedActivatable != null)
                if (droppedActivatable.Source.SlotBehaviour == DraggableBehaviour.Reference ||
                droppedActivatable.Source.SlotBehaviour == DraggableBehaviour.Temporary || droppedActivatable.Link != null ||
                (droppedActivatable.ActivatableType != ActivatableType.Item && droppedActivatable.ActivatableType != ActivatableType.Bag))
                {
                    return;
                }
            if (droppedActivatable != null)
                if (item == null && uguiActivatable == null)
                {
                    // If this was a drag from a reference, do nothing
                    if (droppedActivatable.Source.SlotBehaviour == DraggableBehaviour.Reference)
                    {
                        return;
                    }
                    else if (droppedActivatable.ActivatableType == ActivatableType.Bag)
                    {
                        // If it is a bag, send the place bag message
                        if (droppedActivatable.Source.slotNum == bagNum)
                        {
                            if (hover != null)
                                hover.enabled = false;
                            droppedActivatable.PreventDiscard();
                            return;
                        }

                        Inventory.Instance.PlaceBagAsItem(droppedActivatable.Source.slotNum, bagNum, slotNum);

                        if (itemIcon != null)
                        {
                            itemIcon.enabled = true;
                            itemIcon.sprite = droppedActivatable.icoImage.sprite;
                        }
                        if (hover != null)
                            hover.enabled = false;
                        return;
                    }
                    else if (droppedActivatable.Source is UGUIMerchantItemSlot)
                    {
                        droppedActivatable.Source.Activate();
                        return;
                    }
                    if (droppedActivatable.Source.ammo)
                    {
                        droppedActivatable.PreventDiscard();
                        return;
                    }
                    this.uguiActivatable = droppedActivatable;
                    uguiActivatable.SetDropTarget(this);
                    AtavismInventoryItem newItem = (AtavismInventoryItem)uguiActivatable.ActivatableObject;

                    Inventory.Instance.PlaceItemInBag(bagNum, slotNum, newItem, newItem.Count);
                    if (itemIcon != null)
                    {
                        itemIcon.enabled = true;
                        if (newItem.icon != null)
                            itemIcon.sprite = newItem.icon;
                        else
                            itemIcon.sprite = AtavismSettings.Instance.defaultItemIcon;
                    //    itemIcon.sprite = newItem.icon;
                    }
                    if (hover != null)
                        hover.enabled = false;

                }
                else
                {
                    if (droppedActivatable.Source == this)
                    {
                        droppedActivatable.PreventDiscard();
                        return;
                    }
                    // Check if the source is the same type of item
                    if (item != null && droppedActivatable.ActivatableType == ActivatableType.Item)
                    {
                        AtavismInventoryItem newItem = (AtavismInventoryItem)droppedActivatable.ActivatableObject;
                        if (item.templateId == newItem.templateId)
                        {
                            Inventory.Instance.PlaceItemInBag(bagNum, slotNum, newItem, newItem.Count);
                            droppedActivatable.PreventDiscard();
                            if (hover != null)
                                hover.enabled = false;
                        }
                        else
                        {
                            // Send move item with swap
                            Inventory.Instance.PlaceItemInBag(bagNum, slotNum, newItem, newItem.Count, true);
                            if (itemIcon != null)
                            {
                                itemIcon.enabled = true;
                                if (newItem.icon != null)
                                    itemIcon.sprite = newItem.icon;
                                else
                                    itemIcon.sprite = AtavismSettings.Instance.defaultItemIcon;
                           //     itemIcon.sprite = newItem.icon;
                            }
                            UGUIInventorySlot dA = (UGUIInventorySlot)droppedActivatable.Source;
                            if (dA.itemIcon)
                            {
                                dA.itemIcon.enabled = true;
                                dA.itemIcon.sprite = item.icon;
                            }
                            if (hover != null)
                                hover.enabled = false;

                            droppedActivatable.PreventDiscard();

                        }
                    }
                }
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
            if (!AtavismCursor.Instance.HandleUGUIActivatableUseOverride(uguiActivatable))
            {
                item.Activate();
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