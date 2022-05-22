using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{

    public class UGUIServerList : AtList<UGUIServerListEntry>
    {

        public Button connectButton;
        List<WorldServerEntry> serverEntries = new List<WorldServerEntry>();
        WorldServerEntry selectedEntry = null;

        void Start()
        {
            connectButton.interactable = false;
            AtavismClient.Instance.GetGameServerList();

            // Delete the old list
            ClearAllCells();

            Refresh();
        }

        void OnEnable()
        {
            // Delete the old list
            ClearAllCells();

            Refresh();
        }

        public void SelectEntry(WorldServerEntry selectedEntry)
        {
            this.selectedEntry = selectedEntry;
            connectButton.interactable = true;
        }

        public void ConnectToSelectedServer()
        {
            if (AtavismClient.Instance.ConnectToGameServer(selectedEntry.Name))
            {
                CharacterSelectionCreationManager.Instance.StartCharacterSelection();
                gameObject.SetActive(false);
                connectButton.interactable = false;
            }
        }

        #region implemented abstract members of AtList

        public override int NumberOfCells()
        {
            serverEntries.Clear();
            foreach (WorldServerEntry entry in AtavismClient.Instance.WorldServerMap.Values)
            {
                serverEntries.Add(entry);
            }
            return serverEntries.Count;
        }

        public override void UpdateCell(int index, UGUIServerListEntry cell)
        {
            cell.SetServerDetails(serverEntries[index], this);
        }

        #endregion
    }
}