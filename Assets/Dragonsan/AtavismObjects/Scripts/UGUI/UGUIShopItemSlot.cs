using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUIShopItemSlot : UGUIDraggableSlot
    {

        Button button;
        public Text nameLabel;
        public TextMeshProUGUI TMPNameLabel;
        AtavismInventoryItem item;
        bool mouseEntered = false;
        UGUICreateShop acs;
        int pos = -1;
        // Use this for initialization
        void Start()
        {
            slotBehaviour = DraggableBehaviour.Temporary;
        }
        public void AssignShop(UGUICreateShop acs,int pos)
        {
            this.acs = acs;
            this.pos = pos;
        }

        public void UpdateSlotData(AtavismInventoryItem item)
        {
            this.item = item;
            if (item == null)
            {
                if (uguiActivatable != null)
                {
                    DestroyImmediate(uguiActivatable.gameObject);
                }
                if (nameLabel != null)
                    nameLabel.text = "";
                if (TMPNameLabel != null)
                    TMPNameLabel.text = "";
            }
            else
            {
                if (uguiActivatable == null)
                {
                    if (AtavismSettings.Instance.inventoryItemPrefab != null)
                        uguiActivatable = (UGUIAtavismActivatable)Instantiate(AtavismSettings.Instance.inventoryItemPrefab, transform, false);
                    else
                        uguiActivatable = (UGUIAtavismActivatable)Instantiate(Inventory.Instance.uguiAtavismItemPrefab);
                    //uguiActivatable.transform.SetParent(transform, false);
                    uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                    if (uguiActivatable.GetComponent<RectTransform>().anchorMin == Vector2.zero && uguiActivatable.GetComponent<RectTransform>().anchorMax == Vector2.one)
                        uguiActivatable.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                }
                uguiActivatable.SetActivatable(item, ActivatableType.Item, this);
#if AT_I2LOC_PRESET
            if (nameLabel != null)  nameLabel.text = I2.Loc.LocalizationManager.GetTranslation("Items/" + item.name); ;
            if (TMPNameLabel != null)  TMPNameLabel.text = I2.Loc.LocalizationManager.GetTranslation("Items/" + item.name); ;
#else
                if (nameLabel != null)
                    nameLabel.text = item.name;
                if (TMPNameLabel != null)
                    TMPNameLabel.text = item.name;
#endif
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
            AtavismInventoryItem item = (AtavismInventoryItem)droppedActivatable.ActivatableObject;
            if (item != null)
            {
                if (item.isBound )
                {
                    string[] args = new string[1];
#if AT_I2LOC_PRESET
                args[0] = I2.Loc.LocalizationManager.GetTranslation("You can't trade soulbound item");
#else
                    args[0] = "You can't trade soulbound item";
#endif
                    AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
                    droppedActivatable.PreventDiscard();
                    return;
                } 
                else if ( !item.sellable || !item.auctionHouse)
                {
                    string[] args = new string[1];
#if AT_I2LOC_PRESET
                args[0] = I2.Loc.LocalizationManager.GetTranslation("You can't trade item");
#else
                    args[0] = "You can't trade item";
#endif
                    AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
                    droppedActivatable.PreventDiscard();
                    return;
                }
            }
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
                droppedActivatable.Source.ClearChildSlot(false);
                uguiActivatable = droppedActivatable;

                uguiActivatable.transform.SetParent(transform, false);
                uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                if (uguiActivatable.GetComponent<RectTransform>().anchorMin == Vector2.zero && uguiActivatable.GetComponent<RectTransform>().anchorMax == Vector2.one)
                    uguiActivatable.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
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
            this.item = item;
            acs.DropItem((AtavismInventoryItem)droppedActivatable.ActivatableObject,pos);
        }

        public override void ClearChildSlot(bool send)
        {
         //   Debug.LogError("ClearChildSlot");
            this.item = null;
         /*   if (item == null)
                return;*/
            uguiActivatable = null;
           
        }

        public override void Discarded()
        {
          //  Debug.LogError("Discarded");
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
            acs.ClearSlot(pos, item);
            ClearChildSlot(true);
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
                if (mouseEntered && item != null )
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