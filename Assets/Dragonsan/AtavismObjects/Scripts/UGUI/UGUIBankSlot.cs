using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Atavism
{

    public class UGUIBankSlot : UGUIDraggableSlot
    {

        public int bagNum;
        public Text countLabel;
        AtavismInventoryItem item;
        bool mouseEntered = false;
        public Image itemIcon;
        public Image hover;

        // Use this for initialization
        void Start()
        {
            slotBehaviour = DraggableBehaviour.Standard;
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
                }
                if (itemIcon != null)
                    itemIcon.enabled = false;
                if (mouseEntered)
                    HideTooltip();
            }
            else
            {
                if (uguiActivatable == null)
                {
                    if (itemIcon != null)
                    {
                        itemIcon.enabled = true;
                        if (item.icon != null)
                            itemIcon.sprite = item.icon;
                        else
                            itemIcon.sprite = AtavismSettings.Instance.defaultItemIcon;
//itemIcon.sprite = item.icon;
                    }
                    if (AtavismSettings.Instance.inventoryItemPrefab != null)
                        uguiActivatable = (UGUIAtavismActivatable)Instantiate(AtavismSettings.Instance.inventoryItemPrefab, transform, false);
                    else
                        uguiActivatable = (UGUIAtavismActivatable)Instantiate(Inventory.Instance.uguiAtavismItemPrefab, transform, false);
                    //uguiActivatable.transform.SetParent(transform, false);
                    uguiActivatable.transform.localScale = Vector3.one;
                    uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                    if (uguiActivatable.GetComponent<RectTransform>().anchorMin == Vector2.zero && uguiActivatable.GetComponent<RectTransform>().anchorMax == Vector2.one)
                        uguiActivatable.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                }
                uguiActivatable.SetActivatable(item, ActivatableType.Item, this);
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
            UGUIAtavismActivatable droppedActivatable = eventData.pointerDrag.GetComponent<UGUIAtavismActivatable>();

            // Don't allow reference or temporary slots, or non Item slots
            if (droppedActivatable.Source.SlotBehaviour == DraggableBehaviour.Reference ||
                droppedActivatable.Source.SlotBehaviour == DraggableBehaviour.Temporary || droppedActivatable.Link != null ||
                (droppedActivatable.ActivatableType != ActivatableType.Item))
            {
                return;
            }

            if (item == null && uguiActivatable == null)
            {
                // If this was a drag from a reference, do nothing
                if (droppedActivatable.Source.SlotBehaviour == DraggableBehaviour.Reference)
                {
                    return;
                }
                this.uguiActivatable = droppedActivatable;
                uguiActivatable.SetDropTarget(this);
                AtavismInventoryItem newItem = (AtavismInventoryItem)uguiActivatable.ActivatableObject;
                Inventory.Instance.PlaceItemInBank(bagNum, slotNum, newItem, newItem.Count, false);
                if (itemIcon != null)
                {
                    itemIcon.enabled = true;
                    if (newItem.icon != null)
                        itemIcon.sprite = newItem.icon;
                    else
                        itemIcon.sprite = AtavismSettings.Instance.defaultItemIcon;
                 //   itemIcon.sprite = newItem.icon;
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
                        Inventory.Instance.PlaceItemInBank(bagNum, slotNum, newItem, newItem.Count, false);
                        droppedActivatable.PreventDiscard();
                        if (hover != null)
                            hover.enabled = false;
                    }
                    else
                    {
                        // Send move item with swap
                        Inventory.Instance.PlaceItemInBank(bagNum, slotNum, newItem, newItem.Count, true);
                     /*   if (itemIcon != null)
                        {
                            itemIcon.enabled = true;
                            itemIcon.sprite = newItem.icon;
                        }*/
                      //  Debug.LogError(" " + droppedActivatable.Source);
                        // Debug.LogError(" " + typeof(droppedActivatable.Source));
                        if (droppedActivatable.Source.GetType().Equals(typeof(UGUIBagSlot)))
                        {
                          /*  UGUIBagSlot dA = (UGUIBagSlot)droppedActivatable.Source;
                            if (dA.itemIcon)
                            {
                                dA.itemIcon.enabled = true;
                                dA.itemIcon.sprite = item.icon;
                            }*/
                        }
                        else if (droppedActivatable.Source.GetType().Equals(typeof(UGUIInventorySlot)))
                        {
                       /*     UGUIInventorySlot dA = (UGUIInventorySlot)droppedActivatable.Source;
                            if (dA.itemIcon)
                            {
                                dA.itemIcon.enabled = true;
                                dA.itemIcon.sprite = item.icon;
                            }*/
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
            //if (!AtavismCursor.Instance.HandleUGUIActivatableUseOverride(uguiActivatable)) {
            Inventory.Instance.RetrieveItemFromBank((AtavismInventoryItem)uguiActivatable.ActivatableObject);
            //}
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