using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atavism
{
    public class UGUIGlobalEventList : AtList<UGUIGlobalEventEntry>
    {
      //  [SerializeField] UGUIGlobalEventEntry prefab;
      //  [SerializeField] List<UGUIGlobalEventEntry> events = new List<UGUIGlobalEventEntry>();

        // Start is called before the first frame update
        void Start()
        {
            AtavismEventSystem.RegisterEvent("GLOABL_EVENTS_UPDATE", this);
            ClearAllCells();

            Refresh();
        }

        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("GLOABL_EVENTS_UPDATE", this);
        }
        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "GLOABL_EVENTS_UPDATE")
            {
                ClearAllCells();

                Refresh();
            }
        }

        public override int NumberOfCells()
        {
            return AtavismGlobalEvents.Instance.List.Count;

        }

        public override void UpdateCell(int index, UGUIGlobalEventEntry cell)
        {
            cell.UpdateDisplay(AtavismGlobalEvents.Instance.List[index]);
        }
    }
}