using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{

    public enum DraggableBehaviour
    {
        Standard, // Can be dragged from and receive drops (i.e Inventory Bags)
        SourceOnly, // Can only be dragged from, will not receive any (i.e Ability Window)
        Reference, // Can receive from all, can only be dragged into other reference slots (i.e Action Bar)
        Temporary // Can receive from all (except reference) and can only be dragged into other temporary slots (i.e Mail attachments, crafting)
    }

    public class UGUIDraggableSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {

        protected DraggableBehaviour slotBehaviour = DraggableBehaviour.Standard;
        protected UGUIAtavismActivatable uguiActivatable;
        public bool allowOverwrite = true;
        public int slotNum;
        public bool ammo = false;
        protected bool droppedOnSelf = false; // Has this slot just received a drop that was from itself
        protected UGUIAtavismActivatable backLink; // Links back to the original
        protected Coroutine cor = null;

        // Use this for initialization
        void Start()
        {
        }

        void OnDisable()
        {
            ResetSlot();
        }

        public void ResetSlot()
        {
            if (backLink != null)
            {
                backLink.SetLink(null);
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
        }

        public virtual void ClearChildSlot()
        {
        }
        public virtual void ClearChildSlot(bool send)
        {
        }

        /// <summary>
        /// Called when a player drags the item from the slot and drops it onto nothing
        /// </summary>
        public virtual void Discarded()
        {
        }

        public virtual void Activate()
        {
        }

        protected virtual void ShowTooltip()
        {
        }

        void HideTooltip()
        {
            UGUITooltip.Instance.Hide();
            if (cor != null)
                StopCoroutine(cor);
        }

        public DraggableBehaviour SlotBehaviour
        {
            get
            {
                return slotBehaviour;
            }
            set
            {
                slotBehaviour = value;
            }
        }

        public UGUIAtavismActivatable UguiActivatable
        {
            get
            {
                return uguiActivatable;
            }
            set
            {
                uguiActivatable = value;
            }
        }

        public UGUIAtavismActivatable BackLink
        {
            get
            {
                return backLink;
            }
            set
            {
                backLink = value;
            }
        }

        protected IEnumerator CheckOver()
        {
            bool show = true;
            WaitForSeconds delay = new WaitForSeconds(1.0f);
            while (show)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    PointerEventData pointerData = new PointerEventData(EventSystem.current)
                    {
                        pointerId = -1,
                    };
                    pointerData.position = Input.mousePosition;
                    List<RaycastResult> results = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(pointerData, results);
                    show = false;
                    if (results.Count > 0)
                    {
                        foreach (RaycastResult rr in results)
                        {
                            if (rr.gameObject != null)
                            {
                                UGUIDraggableSlot dds = rr.gameObject.GetComponent<UGUIDraggableSlot>();
                                if (dds != null)
                                {
                                    if (dds == this)
                                    {
                                        AtavismLogger.LogDebugMessage("UGUIDraggableSlot CheckOver dds is this");
                                        show = true;
                                    }
                                    else
                                        AtavismLogger.LogDebugMessage("GUIDraggableSlot CheckOver dds not this");
                                }
                                else
                                    AtavismLogger.LogDebugMessage("UGUIDraggableSlot CheckOver dds null");
                            }
                            else
                                AtavismLogger.LogDebugMessage("UGUIDraggableSlot CheckOver rr.gameObject null");
                        }
                    }
                    else
                        AtavismLogger.LogDebugMessage("UGUIDraggableSlot CheckOver results.Count = 0");
                }
                yield return delay;
            }
            HideTooltip();
        }

    }
}