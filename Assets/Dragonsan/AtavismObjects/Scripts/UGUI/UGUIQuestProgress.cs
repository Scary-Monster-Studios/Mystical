using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUIQuestProgress : MonoBehaviour
    {

        public UGUIDialoguePanel dialoguePanel;
        public Text questTitle;
        public TextMeshProUGUI TMPQuestTitle;
        public Text questProgress;
        public TextMeshProUGUI TMPQuestProgress;
        int questPos = 1;
        //public Button continueButton;

        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("UPDATE_LANGUAGE", this);
            AtavismEventSystem.RegisterEvent("QUEST_ITEM_UPDATE", this);
        }
        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("UPDATE_LANGUAGE", this);
            AtavismEventSystem.UnregisterEvent("QUEST_ITEM_UPDATE", this);
        }

        public void UpdateQuestProgressDetails()
        {
            UpdateQuestProgressDetails(0);
        }

        public void UpdateQuestProgressDetails(int questPos)
        {
            this.questPos = questPos;
            QuestLogEntry selectedQuest = Quests.Instance.GetQuestProgressInfo(questPos);
#if AT_I2LOC_PRESET
        if (questTitle!=null) questTitle.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + selectedQuest.Title);
        if (TMPQuestTitle!=null) TMPQuestTitle.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + selectedQuest.Title);
        if (questProgress!=null)  questProgress.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + selectedQuest.ProgressText);
       if (TMPQuestProgress!=null)  TMPQuestProgress.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + selectedQuest.ProgressText);
#else
            if (questTitle != null)
                questTitle.text = selectedQuest.Title;
            if (TMPQuestTitle != null)
                TMPQuestTitle.text = selectedQuest.Title;
            if (questProgress != null)
                questProgress.text = selectedQuest.ProgressText;
            if (TMPQuestProgress != null)
                TMPQuestProgress.text = selectedQuest.ProgressText;
#endif
        }

        public void Continue()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.ShowConcludeQuestPanel();
            }
            this.questPos = -1;
        }

        public void Cancel()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.Hide();
            }
            //        if (NpcInteraction.Instance != null)
            //            NpcInteraction.Instance.GetInteractionOptionsForNpc(NpcInteraction.Instance.NpcId.ToLong());
            this.questPos = -1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eData"></param>
        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "UPDATE_LANGUAGE" || eData.eventType == "QUEST_ITEM_UPDATE")
            {
                if (this.questPos != -1)
                    UpdateQuestProgressDetails(this.questPos);
            }
        }
    }
}