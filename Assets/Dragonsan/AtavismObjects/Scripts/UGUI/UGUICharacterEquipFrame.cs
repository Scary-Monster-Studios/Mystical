using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{

    public class UGUICharacterEquipFrame : MonoBehaviour
    {

        public UGUIPanelTitleBar titleBar;
        public List<UGUICharacterEquipSlot> slots;
        public UGUIItemDisplay ammoSlot;
        public UGUICharacterEquipSlot ammoSlot2;


        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("EQUIPPED_UPDATE", this);
            AtavismEventSystem.RegisterEvent("ITEM_ICON_UPDATE", this);
            UpdateEquipSlots();

            if (titleBar != null)
            {
                titleBar.SetPanelTitle(ClientAPI.GetPlayerObject().Name);
            }
        }

        void OnEnable()
        {
            UpdateEquipSlots();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("EQUIPPED_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("ITEM_ICON_UPDATE", this);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "EQUIPPED_UPDATE" || eData.eventType == "ITEM_ICON_UPDATE")
            {
                // Update 
                UpdateEquipSlots();
            }
        }

        public void UpdateEquipSlots()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i] != null)
                {
                    AtavismInventoryItem item = GetItemInSlot(slots[i].slotName);
                    slots[i].UpdateEquipItemData(item);
                }
            }

            if (ammoSlot2 != null)
                ammoSlot2.UpdateEquipItemData(Inventory.Instance.EquippedAmmo);
            if (Inventory.Instance.EquippedAmmo != null)
            {
                //	ammoSlot.gameObject.SetActive(true);
                if (ammoSlot2 != null)
                    ammoSlot2.UpdateEquipItemData(Inventory.Instance.EquippedAmmo);
                //ammoSlot.SetItemData(Inventory.Instance.EquippedAmmo, Inventory.Instance.UnequipAmmo);
                //    if (ammoSlot.countText != null)
                //		ammoSlot.countText.text = Inventory.Instance.GetCountOfItem(Inventory.Instance.EquippedAmmo.templateId).ToString();
            }
            else
            {
                //	ammoSlot.gameObject.SetActive(false);
            }
        }

        AtavismInventoryItem GetItemInSlot(string slotName)
        {
            foreach (AtavismInventoryItem item in Inventory.Instance.EquippedItems.Values)
            {
                string orgSlotName = Inventory.Instance.GetItemByTemplateID(item.TemplateId).slot;
               
                    if (Inventory.Instance.itemGroupSlots.ContainsKey(slotName))
                    {
                       // Debug.LogError(" found Group "+slotName+" GS:"+Inventory.Instance.itemGroupSlots[slotName]);
                       if (Inventory.Instance.itemGroupSlots[orgSlotName].all)
                       {
                           foreach (var s in Inventory.Instance.itemGroupSlots[orgSlotName].slots)
                           {
                               if (s.name.ToLower() == slotName.ToLower())
                               {
                                   return item;
                               }
                           }
                       }
                        foreach (var s in Inventory.Instance.itemGroupSlots[slotName].slots)
                        {
                           // Debug.LogError(" Group Slot "+s.name+" | "+slotName);

                            if (s.name.ToLower() == item.slot.ToLower())
                            {// Debug.LogError(" add items "+item.itemId);
                                return item;
                            }
                        }

                      /*  if (item.slot.ToLower() == slotName.ToLower())
                        { //Debug.LogError(" add items "+item.itemId);
                            items.Add(item);
                        }*/
                    }
                 /*   else
                    {
                        if (item.slot.ToLower() == slotName.ToLower())
                        {
                            //Debug.LogError(" add items "+item.itemId);
                            items.Add(item);
                        }
                    }*/
                
                  
             /*   if (item.slot == slotName)
                {
                    return item;
                }
                else if (item.slot == "Two Hand" && slotName == "Main Hand")
                {
                    return item;
                }
                else if (item.slot == "PrimaryRing" && slotName == "Main Ring")
                {
                    return item;
                }
                else if (item.slot == "SecondaryRing" && slotName == "Off Ring")
                {
                    return item;
                }
                else if (item.slot == "PrimaryWeapon" && slotName == "Main Hand")
                {
                    return item;
                }
                else if (item.slot == "SecondaryWeapon" && slotName == "Off Hand")
                {
                    return item;
                }
                else if (item.slot == "PrimaryEarring" && slotName == "Main Earring")
                {
                    return item;
                }
                else if (item.slot == "SecondaryEarring" && slotName == "Off Earring")
                {
                    return item;
                }*/

            }
            return null;
        }

        public void Toggle()
        {
            gameObject.SetActive(!gameObject.activeSelf);
            if (gameObject.activeSelf)
                AtavismUIUtility.BringToFront(transform.parent.gameObject);
        }
    }
}