using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Atavism
{

    public class UGUIRepairSlot : UGUIDraggableSlot
    {
        UGUIRepairWindow repairWindow;
        bool mouseEntered = false;
        //   float cooldownExpiration = -1;

        // Use this for initialization
        void Start()
        {
            slotBehaviour = DraggableBehaviour.Temporary;
        }

        /*void OnDisable()
        {
            if (uguiActivatable != null)
                Discarded();
        }*/

        public void UpdateRepairSlotData(AtavismInventoryItem component)
        {
            if (component == null)
            {
                if (uguiActivatable != null)
                {
                    DestroyImmediate(uguiActivatable.gameObject);
                    if (backLink != null)
                    {
                        backLink.SetLink(null);
                    }
                }
            }
            else
            {
                if (uguiActivatable == null)
                {
                    uguiActivatable = (UGUIAtavismActivatable)Instantiate(Inventory.Instance.uguiAtavismItemPrefab);
                    uguiActivatable.transform.parent = transform;
                    uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                }
                uguiActivatable.SetActivatable(component, ActivatableType.Item, this);
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

            // Reject any references or non item slots
            if (droppedActivatable.Source.SlotBehaviour == DraggableBehaviour.Reference || droppedActivatable.Link != null
                || droppedActivatable.ActivatableType != ActivatableType.Item)
            {
                return;
            }
            else
            {
                AtavismInventoryItem droppedItem = (AtavismInventoryItem)droppedActivatable.ActivatableObject;
                if (droppedItem.MaxDurability == 0 || droppedItem.Durability == droppedItem.MaxDurability || !droppedItem.repairable)
                {
                    // dispatch a ui event to tell the rest of the system
                    string[] args = new string[1];
                    args[0] = "That Item cannot be repaired";
                    AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
                    droppedActivatable.PreventDiscard();
                    return;
                }
            }
            SetActivatable(droppedActivatable);
        }

        public void SetActivatable(UGUIAtavismActivatable droppedActivatable)
        {

            if (uguiActivatable != null && uguiActivatable != droppedActivatable)
            {
                // Delete existing child
                DestroyImmediate(uguiActivatable.gameObject);
                if (backLink != null)
                {
                    backLink.SetLink(null);
                }
            }
            else if (uguiActivatable == droppedActivatable)
            {
                droppedOnSelf = true;
            }

            // If the source was a temporary slot, clear it
            if (droppedActivatable.Source.SlotBehaviour == DraggableBehaviour.Temporary)
            {
                droppedActivatable.Source.ClearChildSlot();
                uguiActivatable = droppedActivatable;

                uguiActivatable.transform.parent = transform;
                uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                backLink = droppedActivatable.Source.BackLink;
            }
            else
            {
                // Create a duplicate
                uguiActivatable = Instantiate(droppedActivatable);
                uguiActivatable.transform.parent = transform;
                uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                uguiActivatable.GetComponent<CanvasGroup>().blocksRaycasts = true;
                uguiActivatable.SetActivatable(droppedActivatable.ActivatableObject, ActivatableType.Item, this);

                // Set the back link
                backLink = droppedActivatable;
                droppedActivatable.SetLink(uguiActivatable);
            }

            droppedActivatable.SetDropTarget(this);

            //repairWindow.RepairListUpdated(this);
        }

        public override void ClearChildSlot()
        {
            uguiActivatable = null;
            //Crafting.Instance.SetGridItem(slotNum, null);
        }

        public override void Discarded()
        {
            if (droppedOnSelf)
            {
                droppedOnSelf = false;
                return;
            }
            if(uguiActivatable!=null)
            DestroyImmediate(uguiActivatable.gameObject);
            if (backLink != null)
            {
                backLink.SetLink(null);
            }
            backLink = null;
            ClearChildSlot();
        }

        public override void Activate()
        {
            // Do nothing
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
                if (mouseEntered && uguiActivatable != null)
                {
                    uguiActivatable.ShowTooltip(gameObject);
                }
                else
                {
                    HideTooltip();
                }
            }
        }
    }
}