using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Atavism
{

    public class UGUICharacterEquipSlot : UGUIDraggableSlot
    {

        public string slotName;
        Button button;
        //public Text countLabel;
        AtavismInventoryItem item;
        bool mouseEntered = false;

        void Start()
        {
            slotBehaviour = DraggableBehaviour.Standard;
        }

        /// <summary>
        /// Creates a UGUIAtavismActivatable object to put in this slot if the item is not null.
        /// </summary>
        /// <param name="item">Item.</param>
        public void UpdateEquipItemData(AtavismInventoryItem item)
        {
            if (item == null)
            {
                if (uguiActivatable != null)
                {
                    DestroyImmediate(uguiActivatable.gameObject);
                    uguiActivatable = null;
                    if (mouseEntered)
                        HideTooltip();
                }
            }
            else 
            {
                if (this.item != null && ((item.ItemId != null && !item.ItemId.Equals(this.item.ItemId)) || item.ItemId == null))
                {
                    if (uguiActivatable != null)
                    {
                        DestroyImmediate(uguiActivatable.gameObject);
                        uguiActivatable = null;
                    }
                }
                if (uguiActivatable == null)
                {
                    if (AtavismSettings.Instance.inventoryItemPrefab != null)
                        uguiActivatable = (UGUIAtavismActivatable)Instantiate(AtavismSettings.Instance.inventoryItemPrefab, transform, false);
                    else
                        uguiActivatable = (UGUIAtavismActivatable)Instantiate(Inventory.Instance.uguiAtavismItemPrefab, transform, false);
                    uguiActivatable.transform.SetParent(transform, false);
                    uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                }
                uguiActivatable.SetActivatable(item, ActivatableType.Item, this);
            }
            this.item = item;
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
            // Apply logic here
            UGUIAtavismActivatable droppedActivatable = eventData.pointerDrag.GetComponent<UGUIAtavismActivatable>();
            if (droppedActivatable != null)
                if (droppedActivatable.Source == this)
                {
                    droppedActivatable.PreventDiscard();
                    return;
                }

            // Reject any references, temporaries or non item slots
            if (droppedActivatable.Source.SlotBehaviour == DraggableBehaviour.Reference ||
                droppedActivatable.Source.SlotBehaviour == DraggableBehaviour.Temporary ||
                droppedActivatable.ActivatableType != ActivatableType.Item)
            {
                return;
            }
            //if (item == null && uguiItem == null) {
            // TODO: perhaps change this to store locally

            //uguiItem.SetDropTarget(this.transform);
            //droppedActivatable.ActivatableObject.Activate();
            if (droppedActivatable.ActivatableObject is AtavismInventoryItem)
            {
                Inventory.Instance.EquipItemInSlot((AtavismInventoryItem)droppedActivatable.ActivatableObject, slotName);
            }
            droppedActivatable.PreventDiscard();
            //}
        }

        public override void ClearChildSlot()
        {
            uguiActivatable = null;
        }

        public override void Activate()
        {
            if (item != null)
                item.Activate();
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
                if (mouseEntered)
                {
                    if (uguiActivatable != null)
                    {
                        uguiActivatable.ShowTooltip(gameObject);
                        cor = StartCoroutine(CheckOver());
                    }
                    else
                    {
                        ShowTooltip();
                    }
                }
                else
                {
                    HideTooltip();
                }
            }
        }
    }
}