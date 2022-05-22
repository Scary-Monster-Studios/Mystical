using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{

    public class UGUIRepairWindow : MonoBehaviour
    {

       // public List<UGUICurrency> costCurrencies;
        public List<UGUIRepairSlot> repairSlots;
        bool showing = false;
        CanvasGroup _canvasGroup;

        // Use this for initialization
        void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            AtavismEventSystem.RegisterEvent("REPAIR_COMPLETE", this);
            AtavismEventSystem.RegisterEvent("REPAIR_START", this);
            Hide();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("REPAIR_COMPLETE", this);
            AtavismEventSystem.UnregisterEvent("REPAIR_START", this);
        }

        void OnDisable()
        {
            foreach (UGUIRepairSlot repairSlot in repairSlots)
            {
                repairSlot.UpdateRepairSlotData(null);
            }
        }

        public void OnEvent(AtavismEventData eData)
        {
          //  Debug.LogError("OnEvent " + eData.eventType);
            if (eData.eventType == "REPAIR_COMPLETE")
            {
                // Clear slots 
                foreach (UGUIRepairSlot repairSlot in repairSlots)
                {
                    repairSlot.UpdateRepairSlotData(null);
                }
            }else if (eData.eventType == "REPAIR_START")
            {
                if (AtavismCursor.Instance != null)
                    AtavismCursor.Instance.SetUGUIActivatableClickedOverride(PlaceRepirItem);
                Show();

            }
        }

        public void RepairListUpdated(UGUIRepairSlot slot)
        {
            //TODO: update currency display
        }

        public void Repair()
        {
            List<AtavismInventoryItem> itemsToRepair = new List<AtavismInventoryItem>();
            foreach (UGUIRepairSlot repairSlot in repairSlots)
            {
                if (repairSlot != null && repairSlot.UguiActivatable != null)
                {
                    itemsToRepair.Add((AtavismInventoryItem)repairSlot.UguiActivatable.ActivatableObject);
                }
            }
            NpcInteraction.Instance.RepairItems(itemsToRepair);
         /*   foreach (UGUIRepairSlot repairSlot in repairSlots)
            {
                if (repairSlot != null)
                    repairSlot.Discarded();
            }*/
        }

        public void RepairAll()
        {
            NpcInteraction.Instance.RepairAllItems();
        }


        private void PlaceRepirItem(UGUIAtavismActivatable activatable)
        {
           //   Debug.LogError("PlaceRepirItem " + activatable.Link);

            if (activatable.Link != null)
            {
                       Debug.LogError("PlaceRepirItem " + activatable.Link);
                return;
            }
            AtavismInventoryItem item = (AtavismInventoryItem)activatable.ActivatableObject;
            if (item != null)
            {
               //    Debug.LogError("item.MaxDurability: "+item.MaxDurability);
                if (item.MaxDurability > 0 && item.Durability < item.MaxDurability && item.repairable)
                {
                    foreach (UGUIRepairSlot repairSlot in repairSlots)
                    {
                        if (repairSlot != null && repairSlot.UguiActivatable == null)
                        {
                            repairSlot.SetActivatable(activatable);
                            return;
                        }
                    }
                }
                else
                {
                    activatable.PreventDiscard();

                    //     Debug.LogError("Wrong Item");
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

       
        public void Show()
        {
            AtavismUIUtility.BringToFront(gameObject);
            if (AtavismCursor.Instance != null)
                AtavismCursor.Instance.SetUGUIActivatableClickedOverride(PlaceRepirItem);
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.interactable = true;
            }
            showing = true;
        }

        public void Hide()
        {
            if (AtavismCursor.Instance != null)
                AtavismCursor.Instance.ClearUGUIActivatableClickedOverride(PlaceRepirItem);

            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.blocksRaycasts = false;
            }
            foreach (UGUIRepairSlot repairSlot in repairSlots)
            {
                if (repairSlot != null)
                    repairSlot.Discarded();
            }
            showing = false;
        }

        public void Toggle()
        {
            if (showing)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }


    }
}