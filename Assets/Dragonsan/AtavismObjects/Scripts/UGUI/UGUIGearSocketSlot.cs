using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Atavism
{

    public delegate void ItemResponse(AtavismInventoryItem item);

    public class UGUIGearSocketSlot : UGUIDraggableSlot, IPointerClickHandler
    {

        Button button;
        AtavismAction action;
        bool mouseEntered = false;
        void Start()
        {
            slotBehaviour = DraggableBehaviour.Temporary;
        }

        public void UpdateAttachmentData(AtavismAction action)
        {
            if (itemResponse != null)
                itemResponse(null);
            Debug.LogWarning("UpdateAttachmentData");

            this.action = action;
            if (action == null || action.actionObject == null)
            {
                if (uguiActivatable != null)
                {
                    Debug.LogWarning("UpdateAttachmentData dest/roy");
                    if (uguiActivatable != null)
                        DestroyImmediate(uguiActivatable.gameObject);
                }
                Debug.LogWarning("UpdateAttachmentDatav 2");

            }
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.LogWarning("UGUIGearSocketSlot OnPointerClick " + eventData);

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

                uguiActivatable.transform.SetParent(transform, false);
                uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                backLink = droppedActivatable.Source.BackLink;
            }
            else
            {
                AtavismInventoryItem item = (AtavismInventoryItem)droppedActivatable.ActivatableObject;
                if (item != null)
                {
                    if ((item.sockettype.Length > 0 && SocketMode == 0) || (item.itemEffectTypes.Contains("Sockets") && SocketMode == 1))
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

                        droppedActivatable.SetDropTarget(this);
                        if (itemResponse != null)
                            itemResponse(item);
                        else
                            Debug.LogWarning("itemResponse is null");

                    }
                    else if ((item.itemType.Contains("Armor") || item.itemType.Contains("Weapon")) && SocketMode == 2 && item.EnchantId > 0)
                    {
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

                        droppedActivatable.SetDropTarget(this);
                        if (itemResponse != null)
                            itemResponse(item);
                        else
                            Debug.LogWarning("itemResponse is null");
                    }
                    else
                    {
                        //    droppedActivatable.Source.ClearChildSlot();
                        droppedActivatable.PreventDiscard();
                        /*   droppedActivatable.PreventDiscard();
                           droppedActivatable.transform.SetParent(transform, false);
                           droppedActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                           backLink = droppedActivatable.Source.BackLink;*/
                        //   Debug.LogError("Wrong Item");
                        string[] args = new string[1];
#if AT_I2LOC_PRESET
                    args[0] = I2.Loc.LocalizationManager.GetTranslation("Wrong Item");
#else
                        args[0] = "Wrong Item";
#endif
                        AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
                    }


                }

            }


        }
        ItemResponse itemResponse;
        int SocketMode = 0;
        public void SetSocket(ItemResponse itemResponse, int SocketMode)
        {
            this.itemResponse = itemResponse;
            this.SocketMode = SocketMode;
        }

        public override void ClearChildSlot()
        {
            uguiActivatable = null;
            //   Mailing.Instance.SetMailItem(slotNum, null);
        }

        public override void Discarded()
        {
            // Debug.LogError("Discarded UGUIGearSocketSlot " + name);

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
            if (itemResponse != null)
                itemResponse(null);
            ClearChildSlot();
        }

        public override void Activate()
        {
            if (uguiActivatable != null)
                Discarded();
            // Debug.LogError("UGUIGearSocketSlot Activate");
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