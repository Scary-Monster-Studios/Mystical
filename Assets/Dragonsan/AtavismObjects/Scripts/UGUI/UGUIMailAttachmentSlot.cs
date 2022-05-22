using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Atavism
{

    public class UGUIMailAttachmentSlot : UGUIDraggableSlot
    {

        Button button;
        public Text cooldownLabel;
        AtavismAction action;
        bool mouseEntered = false;
        //	float cooldownExpiration = -1;

        // Use this for initialization
        void Start()
        {
            slotBehaviour = DraggableBehaviour.Temporary;
        }

        public void UpdateAttachmentData(AtavismAction action)
        {
            this.action = action;
            if (action == null || action.actionObject == null)
            {
                if (uguiActivatable != null)
                {
                    DestroyImmediate(uguiActivatable.gameObject);
                }
            }
            else
            {
                if (uguiActivatable == null)
                {
                    if (action.actionType == ActionType.Ability)
                    {
                        if (AtavismSettings.Instance.abilitySlotPrefab != null)
                            uguiActivatable = (UGUIAtavismActivatable)Instantiate(AtavismSettings.Instance.abilitySlotPrefab, transform, false);
                        else
                            uguiActivatable = (UGUIAtavismActivatable)Instantiate(Abilities.Instance.uguiAtavismAbilityPrefab, transform, false);
                    }
                    else
                    {
                        if (AtavismSettings.Instance.inventoryItemPrefab != null)
                            uguiActivatable = (UGUIAtavismActivatable)Instantiate(AtavismSettings.Instance.inventoryItemPrefab, transform, false);
                        else
                            uguiActivatable = (UGUIAtavismActivatable)Instantiate(Inventory.Instance.uguiAtavismItemPrefab, transform, false);
                    }
                    //			uguiActivatable.transform.SetParent(transform, false);
                    uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                    if (uguiActivatable.GetComponent<RectTransform>().anchorMin == Vector2.zero && uguiActivatable.GetComponent<RectTransform>().anchorMax == Vector2.one)
                        uguiActivatable.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                }
                uguiActivatable.SetActivatable(action.actionObject, ActivatableType.Item, this);
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

                uguiActivatable.transform.SetParent(transform, false);
                uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                backLink = droppedActivatable.Source.BackLink;
            }
            else
            {
                // Create a duplicate
                uguiActivatable = Instantiate(droppedActivatable);
                uguiActivatable.transform.SetParent(transform, false);
                uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                if (uguiActivatable.GetComponent<RectTransform>().anchorMin == Vector2.zero && uguiActivatable.GetComponent<RectTransform>().anchorMax == Vector2.one)
                    uguiActivatable.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                uguiActivatable.GetComponent<CanvasGroup>().blocksRaycasts = true;
                uguiActivatable.SetActivatable(droppedActivatable.ActivatableObject, ActivatableType.Item, this);

                // Set the back link
                backLink = droppedActivatable;
                droppedActivatable.SetLink(uguiActivatable);
            }

            droppedActivatable.SetDropTarget(this);

            Mailing.Instance.SetMailItem(slotNum, (AtavismInventoryItem)droppedActivatable.ActivatableObject);
        }

        public override void ClearChildSlot()
        {
            uguiActivatable = null;
            Mailing.Instance.SetMailItem(slotNum, null);
        }

        public override void Discarded()
        {
            if (droppedOnSelf)
            {
                droppedOnSelf = false;
                return;
            }
            if (uguiActivatable != null)
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
                //if (mouseEntered && action != null && action.actionObject != null) {
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