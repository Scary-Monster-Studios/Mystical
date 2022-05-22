using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUIBagSlot : UGUIDraggableSlot
    {

        UGUIBag bagPanel;
        Button button;
        public Text cooldownLabel;
        public TextMeshProUGUI TMPCooldownLabel;
        Bag bag;
        bool mouseEntered = false;
        public KeyCode activateKey;
        //	float cooldownExpiration = -1;
        public Image itemIcon;
        public Material materialGray;
        public Image hover;

        // Use this for initialization
        void Start()
        {
            slotBehaviour = DraggableBehaviour.Standard;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(activateKey) && !ClientAPI.UIHasFocus())
            {
                Activate();
            }
            if (this.bag == null)
            {
                if (itemIcon != null)
                    itemIcon.enabled = false;
            }

        }

        public void UpdateBagData(Bag bag, UGUIBag bagPanel)
        {
            this.bag = bag;
            this.bagPanel = bagPanel;
            if (bag == null)
            {
                if (uguiActivatable != null)
                {
                    Destroy(uguiActivatable.gameObject);
                    if (itemIcon != null)
                        itemIcon.enabled = false;
                }
            }
            else
            {
                if (bag.itemTemplate == null)
                {
                    // Do nothing, hard coded first bag?
                    if (itemIcon != null)
                    {
                        itemIcon.enabled = true;
                        itemIcon.sprite = AtavismSettings.Instance.defaultBagIcon;
                    }

                }
                else if (uguiActivatable == null)
                {
                    if (itemIcon != null)
                    {
                        itemIcon.enabled = true;
                        if (bag.itemTemplate.icon != null)
                            itemIcon.sprite = bag.itemTemplate.icon;
                        else
                            itemIcon.sprite = AtavismSettings.Instance.defaultItemIcon;
                       // itemIcon.sprite = bag.itemTemplate.icon;
                        itemIcon.material = materialGray;
                    }
                    if (AtavismSettings.Instance.inventoryItemPrefab != null)
                        uguiActivatable = (UGUIAtavismActivatable)Instantiate(AtavismSettings.Instance.inventoryItemPrefab, transform, false);
                    else
                        uguiActivatable = Instantiate(Inventory.Instance.uguiAtavismItemPrefab, transform, false);
                    //uguiActivatable.transform.SetParent(transform, false);
                    uguiActivatable.transform.localScale = Vector3.one;
                    uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                    if (uguiActivatable.GetComponent<RectTransform>().anchorMin == Vector2.zero && uguiActivatable.GetComponent<RectTransform>().anchorMax == Vector2.one)
                        uguiActivatable.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                    uguiActivatable.SetActivatable(bag.itemTemplate, ActivatableType.Bag, this);
                }
                else
                {
                    uguiActivatable.SetActivatable(bag.itemTemplate, ActivatableType.Bag, this);
                }
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            MouseEntered = true;
            if (eventData.pointerDrag != null)
            {
                UGUIAtavismActivatable dActivatable = eventData.pointerDrag.GetComponent<UGUIAtavismActivatable>();
                if (dActivatable != null)
                    if (dActivatable.Source == this)
                    {
                        if (hover != null)
                        {
                            hover.enabled = true;
                            hover.color = AtavismSettings.Instance.itemDropColorFalse;
                        }
                    }
                    else
                    {
                        if (uguiActivatable != null)
                        {
                            if (uguiActivatable.hover != null)
                            {
                                uguiActivatable.hover.enabled = true;
                                uguiActivatable.hover.color = AtavismSettings.Instance.itemDropColorTrue;
                            }
                        }
                        else
                        {
                            if (hover != null)
                            {
                                hover.enabled = true;
                                hover.color = AtavismSettings.Instance.itemDropColorTrue;
                            }
                        }
                    }
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (uguiActivatable != null)
            {
                if (uguiActivatable.hover != null)
                    uguiActivatable.hover.enabled = false;
            }
            if (hover != null)
                hover.enabled = false;
            MouseEntered = false;
        }

        public override void OnDrop(PointerEventData eventData)
        {
            if (!allowOverwrite)
                return;

            UGUIAtavismActivatable droppedActivatable = eventData.pointerDrag.GetComponent<UGUIAtavismActivatable>();
            if (droppedActivatable == null)
                return;
            // Reject any references, temporaries or non item/bag slots
            if (droppedActivatable.Source.SlotBehaviour == DraggableBehaviour.Reference ||
                droppedActivatable.Source.SlotBehaviour == DraggableBehaviour.Temporary || droppedActivatable.Link != null ||
                (droppedActivatable.ActivatableType != ActivatableType.Item && droppedActivatable.ActivatableType != ActivatableType.Bag))
            {
                return;
            }

            if (droppedActivatable.ActivatableType == ActivatableType.Item)
            {
                Inventory.Instance.PlaceItemAsBag((AtavismInventoryItem)droppedActivatable.ActivatableObject, slotNum);
            }
            else if (droppedActivatable.ActivatableType == ActivatableType.Bag)
            {
                Inventory.Instance.MoveBag(droppedActivatable.Source.slotNum, slotNum);
                if (hover != null)
                    hover.enabled = false;
            }
            droppedActivatable.PreventDiscard();
        }

        public override void ClearChildSlot()
        {
            uguiActivatable = null;
            bag = null;
        }

        public void OnClick()
        {
            Activate();
        }

        public override void Activate()
        {
            if (bag == null)
                return;
            bagPanel.gameObject.SetActive(!bagPanel.gameObject.activeSelf);
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