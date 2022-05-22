using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{

    public class UGUIBag : MonoBehaviour
    {

        public int bagNum;
        public UGUIInventorySlot slotPrefab;
        public GridLayoutGroup inventoryGrid;
        public float verticalPadding = 50;
        List<UGUIInventorySlot> slots = new List<UGUIInventorySlot>();
        bool open = false;

        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("INVENTORY_UPDATE", this);
            UpdateInventory();
        }

        void OnEnable()
        {
            UpdateInventory();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("INVENTORY_UPDATE", this);
        }

        void Update()
        {
            // Recalculate size every frame (not efficient, but not many other options)
            RectTransform rect = GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, inventoryGrid.GetComponent<RectTransform>().sizeDelta.y + verticalPadding);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "INVENTORY_UPDATE")
            {
                // Update 
                UpdateInventory();
            }
        }

        public void UpdateInventory()
        {
            Bag bag = Inventory.Instance.Bags[bagNum];

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
                    UGUIInventorySlot slot = (UGUIInventorySlot)Instantiate(slotPrefab);
                    slot.transform.SetParent(inventoryGrid.transform);
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

        public void Toggle()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        public bool Open
        {
            get
            {
                return open;
            }
        }
    }
}