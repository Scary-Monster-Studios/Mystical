using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Atavism
{

    public class UGUILootList : AtList<UGUILootListEntry>
    {

        public UGUIPanelTitleBar titleBar;
        [SerializeField]
        GameObject panel;

        void Awake()
        {
            Hide();
            AtavismEventSystem.RegisterEvent("LOOT_UPDATE", this);
            AtavismEventSystem.RegisterEvent("CLOSE_LOOT_WINDOW", this);
            AtavismEventSystem.RegisterEvent("ITEM_ICON_UPDATE", this);
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
            AtavismEventSystem.UnregisterEvent("LOOT_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("CLOSE_LOOT_WINDOW", this);
            AtavismEventSystem.UnregisterEvent("ITEM_ICON_UPDATE", this);
        }

        void Show()
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            //	transform.position = Input.mousePosition;
            // transform.localPosition = new Vector3((Screen.width / 2) , (Screen.height / 2) , 0);

            // Delete the old list
            ClearAllCells();

            Refresh();
            if (panel != null)
                panel.SetActive(true);

        }

        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            Inventory.Instance.Loot.Clear();
            Inventory.Instance.LootCurr.Clear();
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "LOOT_UPDATE" || eData.eventType == "ITEM_ICON_UPDATE")
            {
                if (Inventory.Instance.Loot.Count > 0 || Inventory.Instance.LootCurr.Count > 0)
                {
                    Show();
                }
                else
                {
                    GetComponent<CanvasGroup>().alpha = 0f;
                    GetComponent<CanvasGroup>().blocksRaycasts = false;
                }
            }
            else if (eData.eventType == "CLOSE_LOOT_WINDOW")
            {
                Hide();
            }
        }

        public void LootAll()
        {
            NetworkAPI.SendTargetedCommand(Inventory.Instance.LootTarget.ToLong(), "/lootAll");
        }

        #region implemented abstract members of AtList

        public override int NumberOfCells()
        {
            int numCells = Inventory.Instance.Loot.Count+ Inventory.Instance.LootCurr.Count;
            return numCells;
        }

        public override void UpdateCell(int index, UGUILootListEntry cell)
        {
            if(index < Inventory.Instance.Loot.Count)
                cell.SetLootEntryDetails(Inventory.Instance.Loot[index]);
            if (index >= Inventory.Instance.Loot.Count)
                cell.SetLootEntryDetails(Inventory.Instance.LootCurr.ElementAt(index - Inventory.Instance.Loot.Count).Key, Inventory.Instance.LootCurr.ElementAt(index - Inventory.Instance.Loot.Count).Value);
        }

        #endregion
    }
}