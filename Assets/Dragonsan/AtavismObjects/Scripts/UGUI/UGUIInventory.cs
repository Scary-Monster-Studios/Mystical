using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{

    public class UGUIInventory : MonoBehaviour, IPointerDownHandler
    {
        public UGUIPanelTitleBar titleBar;
        //  public List<UGUIBagSlot> bagButtons;
        ///   public List<UGUIBag> bagPanels;
        //    public bool fixedPositionbags = true;
        //   public BagOverflowExpandDirection overflowDirection = BagOverflowExpandDirection.Left;
        public UGUIInventorySlot slotPrefab;
        public UGUIBagSlot bagPrefab;
        public GridLayoutGroup bagGrid;
        public GridLayoutGroup inventoryGrid;
        public GameObject bagsPanel;
        [SerializeField] List<UGUIInventorySlot> slots = new List<UGUIInventorySlot>();
        [SerializeField] List<UGUIBagSlot> bagSlots = new List<UGUIBagSlot>();
        bool showing = false;

        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("INVENTORY_UPDATE", this);
            Dictionary<int, Bag> bags = Inventory.Instance.Bags;
            ProcessBagInventoryChange(bags);
            if (titleBar != null)
                titleBar.SetOnPanelClose(Hide);
        }
        void Awake()
        {
            Hide();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("INVENTORY_UPDATE", this);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(AtavismSettings.Instance.GetKeySettings().inventory) && !ClientAPI.UIHasFocus())
                Toggle();
        }
        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "INVENTORY_UPDATE")
            {
                // Update 
                Dictionary<int, Bag> bags = Inventory.Instance.Bags;//ClientAPI.ScriptObject.GetComponent<Inventory>().Bags;
                                                                    //   Debug.LogError("INVENTORY_UPDATE",gameObject);
                ProcessBagInventoryChange(bags);
            }
        }
        void ProcessBagInventoryChange(Dictionary<int, Bag> bags)
        {
            int numSlots = inventoryGrid.transform.childCount;
            int numBags = bagGrid.transform.childCount;
            int numBagSlots = 0;
            int numBagHaveSlots = 0;
            for (int iii = 0; iii < bags.Count; iii++)
            {
                numBagSlots += bags[iii].numSlots;
                // if (bags[ii].isActive)
                numBagHaveSlots++;
            }
            if (numBags != numBagHaveSlots || bagSlots.Count!= numBagHaveSlots)
            {
                bagSlots.Clear();
                foreach (Transform child in bagGrid.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
                for (int i = 0; i < bags.Count; i++)
                {
                    //   if (bags[i].isActive)
                    //   {
                    UGUIBagSlot bag = (UGUIBagSlot)Instantiate(bagPrefab, bagGrid.transform, false);
                    bag.transform.localScale = Vector3.one;
                    //    bag.transform.SetParent(bagGrid.transform,false);
                    //bag.bagNum = i;
                    bag.slotNum = i;
                    bagSlots.Add(bag);
                    //    }
                }
            }
            //   if (numBagSlots != numSlots) {
            //    slots.Clear();
            foreach (UGUIInventorySlot uis in slots)
            {
                if (uis != null)
                {
                    uis.ResetSlot();
                    if (uis.gameObject.activeSelf)
                        uis.gameObject.SetActive(false);
                }
            }
            /*  foreach (Transform child in inventoryGrid.transform)
              {
                  GameObject.Destroy(child.gameObject);
              }*/
            int ii = 0;
            for (int i = 0; i < bags.Count; i++)
            {
                for (int k = 0; k < bags[i].numSlots; k++)
                {
                    if (ii >= slots.Count)
                    {
                        UGUIInventorySlot slot = (UGUIInventorySlot)Instantiate(slotPrefab, inventoryGrid.transform, false);
                        slots.Add(slot);
                    }
                    if (!slots[ii].gameObject.activeSelf)
                        slots[ii].gameObject.SetActive(true);
                    slots[ii].transform.localScale = Vector3.one;
                    //  slot.transform.SetParent(inventoryGrid.transform,false);
                    slots[ii].bagNum = i;
                    slots[ii].slotNum = k;
                    ii++;
                }
            }

            //  }
            int it = 0;
            for (int i = 0; i < bags.Count; i++)
            {
                for (int k = 0; k < bags[i].numSlots; k++)
                {
                    if (bags[i].items.ContainsKey(k))
                    {
                        slots[it + k].UpdateInventoryItemData(bags[i].items[k]);

                    }
                    else
                    {
                        slots[it + k].UpdateInventoryItemData(null);
                    }
                }
                it += bags[i].numSlots;
                if (bags.ContainsKey(i) && bags[i].isActive)
                {
                    bagSlots[i].UpdateBagData(bags[i], null/*bagPanels[i]*/);

                }
                else
                {
                    bagSlots[i].UpdateBagData(null, null/*bagPanels[i]*/);
                }
            }
        }
        public void Toggle()
        {

            if (showing)
                Hide();
            else
                Show();

        }
        public void Show()
        {
            gameObject.SetActive(true);

            showing = true;
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
            AtavismUIUtility.BringToFront(this.gameObject);

            ProcessBagInventoryChange(Inventory.Instance.Bags);
        }

        public void Hide()
        {
            //   gameObject.SetActive(false);
            showing = false;
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            // Focus the window
            AtavismUIUtility.BringToFront(this.gameObject);
        }
    }
}