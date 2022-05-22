using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{

    public class UGUIBank : MonoBehaviour
    {

        public UGUIPanelTitleBar titleBar;
        public int bagNum;
        public UGUIBankSlot slotPrefab;
        public GridLayoutGroup inventoryGrid;
        public float verticalPadding = 50;
        public KeyCode toggleKey;
        List<UGUIBankSlot> slots = new List<UGUIBankSlot>();
        bool showing = false;

        // Use this for initialization
        void Start()
        {
            if (titleBar != null)
                titleBar.SetOnPanelClose(Hide);
            AtavismEventSystem.RegisterEvent("BANK_UPDATE", this);
            AtavismEventSystem.RegisterEvent("CLOSE_STORAGE_WINDOW", this);
            AtavismEventSystem.RegisterEvent("ITEM_ICON_UPDATE", this);
            Hide();
        }

        public void Show()
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            showing = true;
            AtavismCursor.Instance.SetUGUIActivatableClickedOverride(PlaceBankItem);
            AtavismUIUtility.BringToFront(gameObject);
            // dispatch a ui event to tell the rest of the system
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("CLOSE_NPC_DIALOGUE", args);
        }

        public void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            showing = false;
            if (AtavismCursor.Instance != null)
                AtavismCursor.Instance.ClearUGUIActivatableClickedOverride(PlaceBankItem);
            Inventory.Instance.StorageClosed();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("BANK_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("CLOSE_STORAGE_WINDOW", this);
            AtavismEventSystem.UnregisterEvent("ITEM_ICON_UPDATE", this);
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey) && !ClientAPI.UIHasFocus())
            {
                if (showing)
                    Hide();
                else
                    Inventory.Instance.RequestBankInfo();
            }

            if (showing)
            {
                // Recalculate size every frame (not efficient, but not many other options)
                RectTransform rect = GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, inventoryGrid.GetComponent<RectTransform>().sizeDelta.y + verticalPadding);
            }
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "BANK_UPDATE")
            {
                bool open = bool.Parse(eData.eventArgs[0]);
                if (!showing && open)
                    Show();
                if (showing)
                    UpdateInventory();
            }
            else if (eData.eventType == "CLOSE_STORAGE_WINDOW")
            {
                if (showing)
                    Hide();
            }
            else if (eData.eventType == "ITEM_ICON_UPDATE")
            {
                if (showing)
                    UpdateInventory();
            }
        }

        public void UpdateInventory()
        {
            Bag bag = null;
            if (Inventory.Instance.StorageItems.ContainsKey(bagNum))
                bag = Inventory.Instance.StorageItems[bagNum];
           if (bag == null)
            {
                Hide();
                return;
            }
            Debug.Log("Bank slot count: " + bag.numSlots);

            int numSlots = inventoryGrid.transform.childCount;
            if (bag.numSlots != numSlots)
            {
                slots.Clear();
                foreach (Transform child in inventoryGrid.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }

                for (int i = 0; i < bag.numSlots; i++)
                {
                    UGUIBankSlot slot = (UGUIBankSlot)Instantiate(slotPrefab, inventoryGrid.transform);
                    //	slot.transform.parent = inventoryGrid.transform;
                    slot.bagNum = bagNum;
                    slot.slotNum = i;
                    slots.Add(slot);
                }

                //Canvas.ForceUpdateCanvases();
                //RectTransform rect = GetComponent<RectTransform>();
                //rect.sizeDelta = new Vector2(rect.sizeDelta.x, inventoryGrid.GetComponent<RectTransform>().sizeDelta.y + verticalPadding);
            }

            for (int i = 0; i < bag.numSlots; i++)
            {
                if (bag.items.ContainsKey(i))
                {
                    slots[i].UpdateInventoryItemData(bag.items[i]);
                }
                else
                {
                    slots[i].UpdateInventoryItemData(null);
                }
            }
        }

        public void PlaceBankItem(UGUIAtavismActivatable activatable)
        {
            if (activatable.Link != null)
                return;
            AtavismInventoryItem item = (AtavismInventoryItem)activatable.ActivatableObject;
            Inventory.Instance.PlaceItemInBank(item, item.Count);
        }
    }
}