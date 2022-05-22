using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atavism
{

    public class UGUIQuestLogPrevList : MonoBehaviour
    {
        [SerializeField]
        List<UGUIQuestLogPrev> questPrevList;
        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("QUEST_LOG_LIST_UPDATE", this);
            AtavismEventSystem.RegisterEvent("QUEST_LOG_UPDATE", this);
            AtavismEventSystem.RegisterEvent("UPDATE_LANGUAGE", this);
            //Quests.Instance
            Refresh();
        }
        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("QUEST_LOG_LIST_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("QUEST_LOG_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("UPDATE_LANGUAGE", this);
        }
        void Refresh()
        {
            //int numCells = Quests.Instance.GetSelectedListQuestLog().Count;
            int k = 0;
            for (int i = 0; i < questPrevList.Count; i++)
            {
                if (i < Quests.Instance.GetSelectedListQuestLog().Count)
                {
                    questPrevList[i].gameObject.SetActive(true);
                    questPrevList[i].SetQuestPrev(Quests.Instance.GetSelectedListQuestLog()[i]);
                }
                else
                {
                    questPrevList[i].SetQuestPrev(null);
                    questPrevList[i].gameObject.SetActive(false);
                    k++;
                }
            }
            if (k == questPrevList.Count)
            {
                GetComponent<Canvas>().enabled = false;
            }
            else
            {
                GetComponent<Canvas>().enabled = true;
            }
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "QUEST_LOG_UPDATE" || eData.eventType == "QUEST_LOG_LIST_UPDATE" || eData.eventType == "UPDATE_LANGUAGE")
            {
                Refresh();
            }
        }
    }
}