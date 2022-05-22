using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{

    public enum BagOverflowExpandDirection
    {
        Left,
        Right
    }

    public class UGUIBagBar : MonoBehaviour
    {

        public List<UGUIBagSlot> bagButtons;
        public List<UGUIBag> bagPanels;
        public bool fixedPositionbags = true;
        public BagOverflowExpandDirection overflowDirection = BagOverflowExpandDirection.Left;

        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("INVENTORY_UPDATE", this);
            Dictionary<int, Bag> bags = Inventory.Instance.Bags;
            ProcessBagInventoryChange(bags);
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("INVENTORY_UPDATE", this);
        }

        // Update is called once per frame
        void Update()
        {
            if (fixedPositionbags)
                return;

            // Position the bags above the bar
            float offsetX = 0;
            Vector2 offSet = new Vector2(0, GetComponent<RectTransform>().sizeDelta.y);
            foreach (UGUIBag bag in bagPanels)
            {
                if (!bag.gameObject.activeSelf)
                    continue;
                RectTransform rect = bag.GetComponent<RectTransform>();
                rect.pivot = GetComponent<RectTransform>().pivot;
                rect.anchoredPosition = offSet;
                if (rect.anchoredPosition.y + rect.sizeDelta.y > Screen.height)
                {
                    if (overflowDirection == BagOverflowExpandDirection.Left)
                    {
                        offSet = new Vector2(offsetX - rect.sizeDelta.x, GetComponent<RectTransform>().sizeDelta.y);
                    }
                    else
                    {
                        offSet = new Vector2(offsetX + rect.sizeDelta.x, GetComponent<RectTransform>().sizeDelta.y);
                    }

                    rect.anchoredPosition = offSet;
                }
                offSet = new Vector2(offsetX, offSet.y + rect.sizeDelta.y);
            }
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "INVENTORY_UPDATE")
            {
                // Update 
                Dictionary<int, Bag> bags = Inventory.Instance.Bags;
                ProcessBagInventoryChange(bags);
            }
        }

        void ProcessBagInventoryChange(Dictionary<int, Bag> bags)
        {
            for (int i = 0; i < bagButtons.Count; i++)
            {
                if (bags.ContainsKey(i) && bags[i].isActive)
                {
                    //bagButtons[i].gameObject.SetActive(true);
                    // Set icon
                    bagButtons[i].UpdateBagData(bags[i], bagPanels[i]);
                }
                else
                {
                    //bagButtons[i].gameObject.SetActive(false);
                    bagButtons[i].UpdateBagData(null, bagPanels[i]);
                    bagPanels[i].gameObject.SetActive(false);
                }
            }
        }
    }
}