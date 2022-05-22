using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Atavism
{

    public class UGUIActionBarSlot : UGUIDraggableSlot
    {

        Button button;
        AtavismAction action;
        bool mouseEntered = false;
        public KeyCode activateKey;
        //	float cooldownExpiration = -1;
        int barNum = 0;

        // Use this for initialization
        void Start()
        {
            slotBehaviour = DraggableBehaviour.Reference;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(activateKey) && !ClientAPI.UIHasFocus() && Actions.Instance.MainActionBar == barNum)
            {
                Activate();
            }
        }

        public void UpdateActionData(AtavismAction action, int barNum)
        {
            this.action = action;
            this.barNum = barNum;
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
                        if (AtavismSettings.Instance.actionBarPrefab != null)
                            uguiActivatable = (UGUIAtavismActivatable)Instantiate(AtavismSettings.Instance.actionBarPrefab, transform, false);
                        else
                            uguiActivatable = (UGUIAtavismActivatable)Instantiate(Abilities.Instance.uguiAtavismAbilityPrefab, transform, false);
                    }
                    else
                    {
                        if (AtavismSettings.Instance.actionBarPrefab != null)
                            uguiActivatable = (UGUIAtavismActivatable)Instantiate(AtavismSettings.Instance.actionBarPrefab, transform, false);
                        else
                            uguiActivatable = (UGUIAtavismActivatable)Instantiate(Inventory.Instance.uguiAtavismItemPrefab);
                    }
                    //	uguiActivatable.transform.SetParent(transform, false);
                    uguiActivatable.transform.localScale = Vector3.one;
                    uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                }
                else
                {
                    //TODO: something to update the count text?
                }
                uguiActivatable.SetActivatable(action.actionObject, ActivatableType.Action, this);
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
                return;

            if (droppedActivatable.ActivatableObject is AtavismAbility)
            {
                AtavismAbility ability = (AtavismAbility)droppedActivatable.ActivatableObject;
                if (ability.passive)
                    return;
            }

            // Reject any temporaries or bag slots
            if (droppedActivatable.Source.SlotBehaviour == DraggableBehaviour.Temporary || droppedActivatable.Link != null
                || droppedActivatable.ActivatableType == ActivatableType.Bag)
            {
                return;
            }

            if (uguiActivatable != null && uguiActivatable != droppedActivatable)
            {
                // Delete existing child
                DestroyImmediate(uguiActivatable.gameObject);
            }
            else if (uguiActivatable == droppedActivatable)
            {
                droppedOnSelf = true;
            }
            if (droppedActivatable.Source == this)
            {
                droppedActivatable.PreventDiscard();
                return;
            }

            // If the source was a reference slot, clear it
            bool fromOtherSlot = false;
            int sourceBar = 0;
            int sourceSlot = 0;
            if (droppedActivatable.Source.SlotBehaviour == DraggableBehaviour.Reference)
            {
                fromOtherSlot = true;
                sourceSlot = droppedActivatable.Source.slotNum;
                droppedActivatable.Source.UguiActivatable = null;
                UGUIActionBarSlot sourceBarSlot = (UGUIActionBarSlot)droppedActivatable.Source;
                sourceBar = sourceBarSlot.barNum;
                //droppedActivatable.Source.action = null;
                //droppedActivatable.Source.ClearChildSlot();
                if (uguiActivatable != null && uguiActivatable != droppedActivatable)
                {
                    sourceBarSlot.uguiActivatable = uguiActivatable;
                    sourceBarSlot.uguiActivatable.transform.SetParent(sourceBarSlot.transform, false);
                    sourceBarSlot.uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                    Actions.Instance.SetAction(sourceBarSlot.barNum, sourceBarSlot.slotNum, sourceBarSlot.uguiActivatable.ActivatableObject, fromOtherSlot, barNum, slotNum);
                }

                uguiActivatable = droppedActivatable;

                uguiActivatable.transform.SetParent(transform, false);
                uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            }

            droppedActivatable.SetDropTarget(this);
            Actions.Instance.SetAction(barNum, slotNum, droppedActivatable.ActivatableObject, fromOtherSlot, sourceBar, sourceSlot);
        }

        public override void ClearChildSlot()
        {
           // Debug.LogError("ActionSlot Clear");
            uguiActivatable = null;
            action = null;
            Actions.Instance.SetAction(barNum, slotNum, null, false, 0, 0);
        }

        public override void Discarded()
        {
           // Debug.LogError("ActionSlot Discarded");
            if (droppedOnSelf)
            {
                droppedOnSelf = false;
                return;
            }
            DestroyImmediate(uguiActivatable.gameObject);
            ClearChildSlot();
        }

        public override void Activate()
        {
            if (action != null)
                if (action.actionObject is AtavismInventoryItem)
                {
                    AtavismInventoryItem item = (AtavismInventoryItem)action.actionObject;
                    if (item.ItemId == null)
                    {
                        AtavismInventoryItem matchingItem = Inventory.Instance.GetInventoryItem(item.templateId);
                        if (matchingItem == null)
                            return;
                        action.actionObject = matchingItem;
                    }
                }
            if (action != null)
                action.Activate();
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
                if (mouseEntered && action != null && action.actionObject != null)
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