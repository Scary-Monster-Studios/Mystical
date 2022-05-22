using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Atavism
{

    public class UGUIQuestConclude : MonoBehaviour
    {

        public UGUIDialoguePanel dialoguePanel;
        public Text questTitle;
        public TextMeshProUGUI TMPQuestTitle;
        public Text questCompletionText;
        public TextMeshProUGUI TMPQuestCompletionText;
        public Text rewardTitle;
        public TextMeshProUGUI TMPRewardTitle;
        public RectTransform rewardPanel;
        public List<UGUIItemDisplay> itemRewards;
        public Text chooseTitle;
        public TextMeshProUGUI TMPChooseTitle;
        public RectTransform choosePanel;
        public List<UGUIItemDisplay> chooseRewards;
        public List<UGUICurrency> currency1;
        public List<UGUICurrency> currency2;
        public RectTransform reputationTitle;
        public RectTransform reputation1Panel;
        public TextMeshProUGUI reputation1Name;
        public TextMeshProUGUI reputation1Value;
        public RectTransform reputation2Panel;
        public TextMeshProUGUI reputation2Name;
        public TextMeshProUGUI reputation2Value;
        int questPos = -1;

        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("UPDATE_LANGUAGE", this);
        }
        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("UPDATE_LANGUAGE", this);
        }

        public void UpdateQuestConcludeDetails()
        {
            UpdateQuestConcludeDetails(0);
        }

        public void UpdateQuestConcludeDetails(int questPos)
        {
            this.questPos = questPos;
            QuestLogEntry selectedQuest = Quests.Instance.GetQuestProgressInfo(questPos);
#if AT_I2LOC_PRESET
        if (questTitle != null) questTitle.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + selectedQuest.Title);
        if (TMPQuestTitle != null) TMPQuestTitle.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + selectedQuest.Title);
       if (questCompletionText != null)  questCompletionText.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + selectedQuest.gradeInfo[0].completionText);
       if (TMPQuestCompletionText != null)  TMPQuestCompletionText.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + selectedQuest.gradeInfo[0].completionText);
#else
            if (questTitle != null)
                questTitle.text = selectedQuest.Title;
            if (TMPQuestTitle != null)
                TMPQuestTitle.text = selectedQuest.Title;
            if (questCompletionText != null)
                questCompletionText.text = selectedQuest.gradeInfo[0].completionText;
            if (TMPQuestCompletionText != null)
                TMPQuestCompletionText.text = selectedQuest.gradeInfo[0].completionText;
#endif
            int i0 = 0;
            if (selectedQuest.gradeInfo[0].expReward > 0)
            {
                itemRewards[i0].gameObject.SetActive(true);
#if AT_I2LOC_PRESET
          if (itemRewards[i0].itemName!=null)
              itemRewards[i0].itemName.text = selectedQuest.gradeInfo[0].expReward.ToString() + " " + I2.Loc.LocalizationManager.GetTranslation("EXP");
          if (itemRewards[i0].TMPItemName!=null)
              itemRewards[i0].TMPItemName.text = selectedQuest.gradeInfo[0].expReward.ToString() + " " + I2.Loc.LocalizationManager.GetTranslation("EXP");
#else
                if (itemRewards[i0].itemName != null)
                    itemRewards[i0].itemName.text = selectedQuest.gradeInfo[0].expReward.ToString() + " EXP";
                if (itemRewards[i0].TMPItemName != null)
                    itemRewards[i0].TMPItemName.text = selectedQuest.gradeInfo[0].expReward.ToString() + " EXP";
#endif
                itemRewards[i0].itemIcon.sprite = AtavismSettings.Instance.expIcon;
                if (itemRewards[i0].countText != null)
                    itemRewards[i0].countText.text = selectedQuest.gradeInfo[0].expReward.ToString();
                if (itemRewards[i0].TMPCountText != null)
                    itemRewards[i0].TMPCountText.text = selectedQuest.gradeInfo[0].expReward.ToString();
                i0++;
            }

            // Item Rewards
            for (int i = 0; i < itemRewards.Count - i0; i++)
            {
                if (i < selectedQuest.gradeInfo[0].rewardItems.Count)
                {
                    itemRewards[i0 + i].gameObject.SetActive(true);
                    itemRewards[i0 + i].SetItemData(selectedQuest.gradeInfo[0].rewardItems[i].item, null);
                }
                else
                {
                    itemRewards[i0 + i].gameObject.SetActive(false);
                }
            }
            if (rewardPanel != null)
            {
                if (selectedQuest.gradeInfo[0].rewardItems.Count == 0 && selectedQuest.gradeInfo[0].expReward == 0)
                    rewardPanel.gameObject.SetActive(false);
                else
                    rewardPanel.gameObject.SetActive(true);
            }
            if (rewardTitle != null)
            {
                if (selectedQuest.gradeInfo[0].rewardItems.Count == 0 && selectedQuest.gradeInfo[0].currencies.Count == 0)
                    rewardTitle.gameObject.SetActive(false);
                else
                    rewardTitle.gameObject.SetActive(true);
            }
            if (TMPRewardTitle != null)
            {
                if (selectedQuest.gradeInfo[0].rewardItems.Count == 0 && selectedQuest.gradeInfo[0].currencies.Count == 0)
                    TMPRewardTitle.gameObject.SetActive(false);
                else
                    TMPRewardTitle.gameObject.SetActive(true);
            }

            // Item Choose Rewards
            for (int i = 0; i < chooseRewards.Count; i++)
            {
                if (i < selectedQuest.gradeInfo[0].RewardItemsToChoose.Count)
                {
                    chooseRewards[i].gameObject.SetActive(true);
                    chooseRewards[i].SetItemData(selectedQuest.gradeInfo[0].RewardItemsToChoose[i].item, ItemChosen);
                    chooseRewards[i].Selected(false);
                }
                else
                {
                    chooseRewards[i].gameObject.SetActive(false);
                    chooseRewards[i].Selected(false);
                }
            }
            if (choosePanel != null)
            {
                if (selectedQuest.gradeInfo[0].RewardItemsToChoose.Count == 0)
                    choosePanel.gameObject.SetActive(false);
                else
                    choosePanel.gameObject.SetActive(true);
            }
            if (chooseTitle != null)
            {
                if (selectedQuest.gradeInfo[0].RewardItemsToChoose.Count == 0)
                    chooseTitle.gameObject.SetActive(false);
                else
                    chooseTitle.gameObject.SetActive(true);
            }
            if (TMPChooseTitle != null)
            {
                if (selectedQuest.gradeInfo[0].RewardItemsToChoose.Count == 0)
                    TMPChooseTitle.gameObject.SetActive(false);
                else
                    TMPChooseTitle.gameObject.SetActive(true);
            }

            // Currency Rewards
            if (selectedQuest.gradeInfo[0].currencies.Count > 0)
            {
                List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(selectedQuest.gradeInfo[0].currencies[0].id,
                                                                                                              selectedQuest.gradeInfo[0].currencies[0].count);
                for (int i = 0; i < currency1.Count; i++)
                {
                    if (i < currencyDisplayList.Count)
                    {
                        currency1[i].gameObject.SetActive(true);
                        currency1[i].SetCurrencyDisplayData(currencyDisplayList[i]);
                    }
                    else
                    {
                        currency1[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = 0; i < currency1.Count; i++)
                {
                    currency1[i].gameObject.SetActive(false);
                }
            }
            if (selectedQuest.gradeInfo[0].currencies.Count > 1)
            {
                List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(selectedQuest.gradeInfo[0].currencies[1].id,
                                                                                                              selectedQuest.gradeInfo[0].currencies[1].count);
                for (int i = 0; i < currency2.Count; i++)
                {
                    if (i < currencyDisplayList.Count)
                    {
                        currency2[i].gameObject.SetActive(true);
                        currency2[i].SetCurrencyDisplayData(currencyDisplayList[i]);
                    }
                    else
                    {
                        currency2[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = 0; i < currency2.Count; i++)
                {
                    currency2[i].gameObject.SetActive(false);
                }
            }

            if (selectedQuest != null && selectedQuest.gradeInfo != null && selectedQuest.gradeInfo[0].rewardRep != null && selectedQuest.gradeInfo[0].rewardRep.Count > 0)
            {
                if (reputationTitle != null)
                    reputationTitle.gameObject.SetActive(true);
                if (reputation1Panel != null)
                    reputation1Panel.gameObject.SetActive(true);
                if (reputation1Name != null)
                {
                    reputation1Name.gameObject.SetActive(true);
#if AT_I2LOC_PRESET
                    reputation1Name.text = I2.Loc.LocalizationManager.GetTranslation(selectedQuest.gradeInfo[0].rewardRep[0].name);
#else
                    reputation1Name.text = selectedQuest.gradeInfo[0].rewardRep[0].name;
#endif
                }
                if (reputation1Value != null)
                {
                    reputation1Value.gameObject.SetActive(true);
                    reputation1Value.text = selectedQuest.gradeInfo[0].rewardRep[0].count.ToString();
                    if (selectedQuest.gradeInfo[0].rewardRep[0].count > 0)
                        reputation1Value.color = Color.green;
                    else
                        reputation1Value.color = Color.red;
                }
                if (selectedQuest.gradeInfo[0].rewardRep.Count > 1)
                {
                    if (reputation2Panel != null)
                        reputation2Panel.gameObject.SetActive(true);
                    if (reputation2Name != null)
                    {
                        reputation2Name.gameObject.SetActive(true);
#if AT_I2LOC_PRESET
                        reputation2Name.text =  I2.Loc.LocalizationManager.GetTranslation(selectedQuest.gradeInfo[0].rewardRep[1].name);
#else
                        reputation2Name.text = selectedQuest.gradeInfo[0].rewardRep[1].name;
#endif
                    }
                    if (reputation2Value != null)
                    {
                        reputation2Value.gameObject.SetActive(true);
                        reputation2Value.text = selectedQuest.gradeInfo[0].rewardRep[1].count.ToString();
                        if (selectedQuest.gradeInfo[0].rewardRep[1].count > 0)
                            reputation2Value.color = Color.green;
                        else
                            reputation2Value.color = Color.red;
                    }
                }
                else
                {
                    if (reputation2Panel != null)
                        reputation2Panel.gameObject.SetActive(false);
                    if (reputation2Name != null)
                        reputation2Name.gameObject.SetActive(false);
                    if (reputation2Value != null)
                        reputation2Value.gameObject.SetActive(false);
                }
            }
            else
            {
                if (reputationTitle != null)
                    reputationTitle.gameObject.SetActive(false);
                if (reputation1Panel != null)
                    reputation1Panel.gameObject.SetActive(false);
                if (reputation1Name != null)
                    reputation1Name.gameObject.SetActive(false);
                if (reputation1Value != null)
                    reputation1Value.gameObject.SetActive(false);
                if (reputation2Panel != null)
                    reputation2Panel.gameObject.SetActive(false);
                if (reputation2Name != null)
                    reputation2Name.gameObject.SetActive(false);
                if (reputation2Value != null)
                    reputation2Value.gameObject.SetActive(false);
            }



        }

        public void ItemChosen(AtavismInventoryItem item)
        {
            QuestLogEntry quest = Quests.Instance.GetQuestProgressInfo(0);
            quest.itemChosen = item.templateId;
            for (int i = 0; i < chooseRewards.Count; i++)
            {
                if (i < quest.gradeInfo[0].RewardItemsToChoose.Count)
                {
                    if (quest.gradeInfo[0].RewardItemsToChoose[i].item == item)
                        chooseRewards[i].Selected(true);
                    else
                        chooseRewards[i].Selected(false);
                }
            }
        }

        public void CompleteQuest()
        {
            if (!Quests.Instance.CompleteQuest())
            {
                // dispatch a ui event to tell the rest of the system
                string[] args = new string[1];
#if AT_I2LOC_PRESET
            args[0] = I2.Loc.LocalizationManager.GetTranslation("You must select an item reward before completing this Quest.");
#else
                args[0] = "You must select an item reward before completing this Quest.";
#endif
                AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
                return;
            }

            if (dialoguePanel != null)
            {
                dialoguePanel.Hide();
            }
            this.questPos = -1;
        }

        public void Cancel()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.Hide();
            }
            this.questPos = -1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eData"></param>
        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "UPDATE_LANGUAGE")
            {
                if (this.questPos != -1)
                    UpdateQuestConcludeDetails(this.questPos);
            }
        }

    }
}