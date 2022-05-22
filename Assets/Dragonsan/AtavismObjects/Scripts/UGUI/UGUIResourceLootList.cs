using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class UGUIResourceLootList : AtList<UGUIResourceLootListEntry>
    {

        public UGUIPanelTitleBar titleBar;
        bool showing = false;
        [SerializeField]
        GameObject panel;

        void Awake()
        {
            Hide();
            AtavismEventSystem.RegisterEvent("RESOURCE_LOOT_UPDATE", this);
            AtavismEventSystem.RegisterEvent("CLOSE_RESOURCE_LOOT_WINDOW", this);
        }

        void Start()
        {
            if (titleBar != null)
                titleBar.SetOnPanelClose(Hide);
            // Delete the old list
            ClearAllCells();

            Refresh();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("RESOURCE_LOOT_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("CLOSE_RESOURCE_LOOT_WINDOW", this);
        }

        void Show()
        {
            if (!showing)
            {
                showing = true;
                GetComponent<CanvasGroup>().alpha = 1f;
                GetComponent<CanvasGroup>().blocksRaycasts = true;
                //	transform.position = Input.mousePosition;
                if (panel != null)
                    panel.SetActive(true);
            }

            // Delete the old list
            ClearAllCells();

            Refresh();
        }

        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            showing = false;
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "RESOURCE_LOOT_UPDATE")
            {
                if (Crafting.Instance.ResourceLoot.Count > 0)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            }
            else if (eData.eventType == "CLOSE_RESOURCE_LOOT_WINDOW")
            {
                Hide();
            }
        }

        public void LootAll()
        {
            //NetworkAPI.SendTargetedCommand(Inventory.Instance.LootTarget.ToLong(), "/lootAll");
        }

        #region implemented abstract members of AtList

        public override int NumberOfCells()
        {
            int numCells = Crafting.Instance.ResourceLoot.Count;
            return numCells;
        }

        public override void UpdateCell(int index, UGUIResourceLootListEntry cell)
        {
            cell.SetResourceLootEntryDetails(Crafting.Instance.ResourceLoot[index]);
        }

        #endregion
    }
}