using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

namespace Atavism
{
    public class UGUICraftingSlot : UGUIDraggableSlot
    {

        Button button;
        public Text cooldownLabel;
        CraftingComponent component;
        bool mouseEntered = false;
        public Image ico;
        public Image quality;
        public TextMeshProUGUI count;

        //float cooldownExpiration = -1;

        // Use this for initialization
        void Start()
        {
            slotBehaviour = DraggableBehaviour.Temporary;
        }

        public void UpdateCraftingSlotData(CraftingComponent component)
        {
            this.component = component;
            if (component == null)
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
                    if (AtavismSettings.Instance.inventoryItemPrefab != null)
                        uguiActivatable = (UGUIAtavismActivatable)Instantiate(AtavismSettings.Instance.inventoryItemPrefab, transform, false);
                    else
                        uguiActivatable = (UGUIAtavismActivatable)Instantiate(Inventory.Instance.uguiAtavismItemPrefab, transform, false);
                    //uguiActivatable.transform.SetParent(transform, false);
                    uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                    if (uguiActivatable.GetComponent<RectTransform>().anchorMin == Vector2.zero && uguiActivatable.GetComponent<RectTransform>().anchorMax == Vector2.one)
                        uguiActivatable.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                }
                uguiActivatable.SetActivatable(component.item, ActivatableType.Item, this);
            }
        }
        public void UpdateCraftingBookSlotData(CraftingComponent component)
        {
            this.component = component;
            if (component == null)
            {
                if (ico != null)
                    if (ico.isActiveAndEnabled)
                        ico.enabled = false;
                if (quality != null)
                    if (quality.isActiveAndEnabled)
                        ico.enabled = false;
                if (count != null)
                    count.text = "";
            }
            else
            {
                if (ico != null)
                {
                    if (!ico.isActiveAndEnabled)
                        ico.enabled = true;
                    ico.sprite = component.item.icon;
                }
                if (count != null)
                {
                    if (component.count > 1)
                        count.text = component.count.ToString();
                    else
                        count.text = "";
                }
                if (quality != null)
                {
                    if (!quality.isActiveAndEnabled)
                        quality.enabled = true;
                    quality.color = AtavismSettings.Instance.ItemQualityColor(component.item.quality);
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
            UGUIAtavismActivatable droppedActivatable = eventData.pointerDrag.GetComponent<UGUIAtavismActivatable>();
            if (droppedActivatable == null)
            {
                return;
            }
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

            if (uguiActivatable != null && uguiActivatable != newActivatable)
            {
                // Delete existing child
                DestroyImmediate(uguiActivatable.gameObject);
                if (backLink != null)
                {
                    backLink.SetLink(null);
                }
            }
            else if (uguiActivatable == newActivatable)
            {
                droppedOnSelf = true;
            }

            // If the source was a temporary slot, clear it
            if (newActivatable.Source.SlotBehaviour == DraggableBehaviour.Temporary)
            {
                newActivatable.Source.ClearChildSlot(false);
                uguiActivatable = newActivatable;

                uguiActivatable.transform.SetParent(transform, false);
                uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                if (uguiActivatable.GetComponent<RectTransform>().anchorMin == Vector2.zero && uguiActivatable.GetComponent<RectTransform>().anchorMax == Vector2.one)
                    uguiActivatable.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                backLink = newActivatable.Source.BackLink;
            }
            else
            {
                // Create a duplicate
                uguiActivatable = Instantiate(newActivatable);
                uguiActivatable.transform.SetParent(transform, false);
                uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                if (uguiActivatable.GetComponent<RectTransform>().anchorMin == Vector2.zero && uguiActivatable.GetComponent<RectTransform>().anchorMax == Vector2.one)
                    uguiActivatable.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                uguiActivatable.GetComponent<CanvasGroup>().blocksRaycasts = true;
                uguiActivatable.SetActivatable(newActivatable.ActivatableObject, ActivatableType.Item, this);

                // Set the back link
                backLink = newActivatable;
                newActivatable.SetLink(uguiActivatable);
            }

            newActivatable.SetDropTarget(this);
            Crafting.Instance.SetGridItem(slotNum, (AtavismInventoryItem)newActivatable.ActivatableObject, true);
        }

        public override void ClearChildSlot(bool send)
        {
            uguiActivatable = null;
            Crafting.Instance.SetGridItem(slotNum, null, send);
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
            ClearChildSlot(true);
        }

        public override void Activate()
        {
            // Unlink item?
            Discarded();
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
                if (mouseEntered && component != null && component.item != null)
                {
                    if (uguiActivatable != null)
                        uguiActivatable.ShowTooltip(gameObject);
                    else
                        component.item.ShowTooltip(gameObject);
                    //  cor = StartCoroutine(CheckOver());
                }
                else
                {
                    HideTooltip();
                }
            }
        }
    }
}