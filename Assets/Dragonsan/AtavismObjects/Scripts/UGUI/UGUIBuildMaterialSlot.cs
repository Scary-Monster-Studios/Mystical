using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUIBuildMaterialSlot : UGUIDraggableSlot
    {

        Button button;
        public Text countText;
        public TextMeshProUGUI TMPCountText;
        public UGUIItemDisplay itemDisplay;
        AtavismInventoryItem component;
        int count = 0;
        bool mouseEntered = false;
        //float cooldownExpiration = -1;
        bool activeItem = false;

        // Use this for initialization
        void Start()
        {
            slotBehaviour = DraggableBehaviour.Temporary;
            AtavismEventSystem.RegisterEvent("INVENTORY_UPDATE", this);

        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("INVENTORY_UPDATE", this);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "INVENTORY_UPDATE")
            {
                UpdateBuildingSlotData(component, count);
            }
        }

        public void UpdateBuildingSlotData(AtavismInventoryItem component, int count)
        {
            Debug.Log("Updating building slot data "+component+" "+count);
                 
            this.component = component;
            this.count = count;
            if (component == null)
            {
                itemDisplay.gameObject.SetActive(false);
                if (countText != null)  countText.text = "";
                if (TMPCountText != null) TMPCountText.text = "";
            }
            else
            {
                itemDisplay.gameObject.SetActive(true);
                itemDisplay.SetItemData(component, null);
                if (countText != null)
                {
                    if(WorldBuilder.Instance.showInConstructMaterialsFromBackpack)
                        countText.text = Inventory.Instance.GetCountOfItem(component.templateId)+" / "+count;
                    else
                    if (count > 0)
                    {
                        countText.text = count.ToString();
                    }
                    else
                    {
                        countText.text = "";
                    }
                }
                if (TMPCountText != null)
                {
                    if (WorldBuilder.Instance.showInConstructMaterialsFromBackpack)
                        TMPCountText.text =   Inventory.Instance.GetCountOfItem(component.templateId)+" / "+ count;
                    else
                    if (count > 0)
                    {
                        TMPCountText.text = count.ToString();
                    }
                    else
                    {
                        TMPCountText.text = "";
                    }
                }
                activeItem = false;
                UpdateDisplay();
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
            if (!WorldBuilder.Instance.itemsForUpgradeMustBeInserted)
                return;
            UGUIAtavismActivatable droppedActivatable = eventData.pointerDrag.GetComponent<UGUIAtavismActivatable>();

            // Reject any references or non item slots
            if (droppedActivatable.Source.SlotBehaviour == DraggableBehaviour.Reference || droppedActivatable.Link != null
                || droppedActivatable.ActivatableType != ActivatableType.Item)
            {
                return;
            }

            SetActivatable(droppedActivatable);
        }

        public void SetActivatable(UGUIAtavismActivatable newActivatable)
        {
            AtavismLogger.LogInfoMessage("Setting activatable");
          
            /*if (uguiActivatable != null && uguiActivatable != newActivatable) {
                // Delete existing child
                DestroyImmediate(uguiActivatable.gameObject);
                if (backLink != null) {
                    backLink.SetLink(null);
                }
            } else if (uguiActivatable == newActivatable) {
                droppedOnSelf = true;
            }*/

            // If the source was a temporary slot, clear it
            if (newActivatable.Source.SlotBehaviour == DraggableBehaviour.Temporary)
            {
                newActivatable.Source.ClearChildSlot();
                uguiActivatable = newActivatable;

                uguiActivatable.transform.parent = transform;
                uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                backLink = newActivatable.Source.BackLink;
            }
            else
            {
                // Does the item placed match the given item?
                AtavismInventoryItem newItem = (AtavismInventoryItem)newActivatable.ActivatableObject;
                if (newItem.templateId != component.templateId)
                {
                    newActivatable.PreventDiscard();
                    return;
                }
                // Create a duplicate
                uguiActivatable = Instantiate(newActivatable);
                uguiActivatable.transform.parent = transform;
                uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                uguiActivatable.GetComponent<CanvasGroup>().blocksRaycasts = true;
                uguiActivatable.SetActivatable(newActivatable.ActivatableObject, ActivatableType.Item, this);

                // Set the back link
                backLink = newActivatable;
                newActivatable.SetLink(uguiActivatable);
            }

            newActivatable.SetDropTarget(this);

            WorldBuilder.Instance.AddItemPlacedForUpgrade((AtavismInventoryItem)newActivatable.ActivatableObject);
            activeItem = true;
            UpdateDisplay();
        }

        public override void ClearChildSlot()
        {
         //   Debug.Log("Clearing child slot");
            if (uguiActivatable != null)
                WorldBuilder.Instance.RemoveItemPlacedForUpgrade((AtavismInventoryItem)uguiActivatable.ActivatableObject);
            uguiActivatable = null;
        }

        public override void Discarded()
        {
            if (droppedOnSelf)
            {
                droppedOnSelf = false;
                return;
            }
            if (uguiActivatable != null)
            {
                DestroyImmediate(uguiActivatable.gameObject);
            }
            if (backLink != null)
            {
                backLink.SetLink(null);
            }
            backLink = null;
            ClearChildSlot();
        }

        public override void Activate()
        {
            // Unlink item?
            Discarded();
        }

        void HideTooltip()
        {
            UGUITooltip.Instance.Hide();
        }

        void UpdateDisplay()
        {
            if (activeItem)
            {
                itemDisplay.GetComponent<Image>().color = Color.white;
            }
            else
            {
                itemDisplay.GetComponent<Image>().color = Color.gray;
            }
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
                if (mouseEntered && component != null && uguiActivatable != null)
                {
                    uguiActivatable.ShowTooltip(gameObject);
                }
                else if (mouseEntered && component != null && uguiActivatable == null)
                {
                    component.ShowTooltip(gameObject);
                }
                else
                {
                    HideTooltip();
                }
            }
        }
    }
}