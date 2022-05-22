using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Atavism
{

    public class UGUIQuestList : AtList<UGUIQuestListEntry>
    {

        public UGUIPanelTitleBar titleBar;
        public RectTransform questDetailsPanel;
        public Text questTitle;
        public TextMeshProUGUI TMPQuestTitle;
        public Text questObjective;
        public TextMeshProUGUI TMPQuestObjective;
        public List<Text> objectiveTexts;
        public List<TextMeshProUGUI> TMPObjectiveTexts;
        public Text questDescription;
        public TextMeshProUGUI TMPQuestDescription;
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
        public Button abandonButton;
        public KeyCode toggleKey;
        bool showing = false;
        public Button localizeButton;
        public Scrollbar questListScroll;
        public Scrollbar questDetailScroll;
        bool history = false;
        [AtavismSeparator("Menu Settings")]
        public bool hideNormaleMenuImage = true;
        [SerializeField] Button activeButton;
        [SerializeField] Button historicalButton;
        [SerializeField] TextMeshProUGUI activeButtonText;
        [SerializeField] TextMeshProUGUI historicalButtonText;
        [SerializeField] Color buttonMenuSelectedColor = Color.green;
        [SerializeField] Color buttonMenuNormalColor = Color.white;
        [SerializeField] Color buttonMenuSelectedTextColor = Color.black;
        [SerializeField] Color buttonMenuNormalTextColor = Color.black;

        void Awake()
        {
            AtavismEventSystem.RegisterEvent("QUEST_LOG_UPDATE", this);
            AtavismEventSystem.RegisterEvent("UPDATE_LANGUAGE", this);
            AtavismEventSystem.RegisterEvent("QUEST_ITEM_UPDATE", this);

            if (titleBar != null)
                titleBar.SetOnPanelClose(Hide);
            Hide();
        }

        void Start()
        {
            // Delete the old list
            ActiveQuests();
            ClearAllCells();
            Refresh();

            //SetQuestDetails();
        }

        void OnEnable()
        {
            // Delete the old list
            ClearAllCells();
            Refresh();

            SetQuestDetails();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("QUEST_LOG_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("UPDATE_LANGUAGE", this);
            AtavismEventSystem.UnregisterEvent("QUEST_ITEM_UPDATE", this);
        }

        public void Show()
        {
            showing = true;
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            // Delete the old list
            ClearAllCells();

            Refresh();
            AtavismUIUtility.BringToFront(this.gameObject);
        }

        public void Hide()
        {
            // gameObject.SetActive(false);
            showing = false;
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            QuestExitClicked();
        }
        public void QuestExitClicked()
        {
            Quests.Instance.QuestLogEntrySelected(-1);
            Quests.Instance.QuestHistoryLogEntrySelected(-1);
            questDetailsPanel.GetComponent<CanvasGroup>().alpha = 0f;
            questDetailsPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        void Update()
        {
            //if (Input.GetKeyDown(toggleKey) && !ClientAPI.UIHasFocus()) {
            if (Input.GetKeyDown(AtavismSettings.Instance.GetKeySettings().quest) && !ClientAPI.UIHasFocus())
            {
                if (showing)
                    Hide();
                else
                    Show();
            }
        }

        public void Toggle()
        {
            if (showing)
                Hide();
            else
                Show();
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "QUEST_LOG_UPDATE" || eData.eventType == "UPDATE_LANGUAGE" || eData.eventType == "QUEST_ITEM_UPDATE")
            {
                // Delete the old list
                ClearAllCells();
                Refresh();

                QuestLogEntry selectedQuest = Quests.Instance.GetSelectedQuestLogEntry();
                if (selectedQuest == null)
                {
                    if (questDetailsPanel.GetComponent<CanvasGroup>() != null)
                    {
                        questDetailsPanel.GetComponent<CanvasGroup>().alpha = 0f;
                        questDetailsPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    }
                    else
                    {
                        questDetailsPanel.gameObject.SetActive(false);
                    }
                    abandonButton.interactable = false;
                    return;
                }
                else
                {
                    SetQuestDetails();
                }
            }
        }

        public void SetQuestDetails()
        {
            ClearAllCells();
            Refresh();
            if (questDetailScroll != null)
                questDetailScroll.value = 1;
            QuestLogEntry selectedQuest;
            if (history)
                selectedQuest = Quests.Instance.GetSelectedQuestHistoryLogEntry();
            else
                selectedQuest = Quests.Instance.GetSelectedQuestLogEntry();

            if (selectedQuest == null)
            {
                if (questDetailsPanel.GetComponent<CanvasGroup>() != null)
                {
                    questDetailsPanel.GetComponent<CanvasGroup>().alpha = 0f;
                    questDetailsPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
                }
                else
                {
                    questDetailsPanel.gameObject.SetActive(false);
                }
                abandonButton.interactable = false;
                return;
            }
            abandonButton.interactable = true;
            if (history)
            {
                abandonButton.gameObject.SetActive(false);
                if (localizeButton != null)
                    localizeButton.gameObject.SetActive(false);
            }
            else
            {
                abandonButton.gameObject.SetActive(true);
                if (localizeButton != null)
                    localizeButton.gameObject.SetActive(true);
            }

            if (questDetailsPanel.GetComponent<CanvasGroup>() != null)
            {
                questDetailsPanel.GetComponent<CanvasGroup>().alpha = 1f;
                questDetailsPanel.GetComponent<CanvasGroup>().interactable = true;
                questDetailsPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            else
            {
                questDetailsPanel.gameObject.SetActive(true);
            }
#if AT_I2LOC_PRESET
        if (questTitle != null)  questTitle.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + selectedQuest.Title);
        if (TMPQuestTitle != null)  TMPQuestTitle.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + selectedQuest.Title);
        if (questObjective!=null) questObjective.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + selectedQuest.Objective);
        if (TMPQuestObjective!=null) TMPQuestObjective.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + selectedQuest.Objective);
#else
            if (questTitle != null)
                questTitle.text = selectedQuest.Title;
            if (TMPQuestTitle != null)
                TMPQuestTitle.text = selectedQuest.Title;

            if (questObjective != null)
                questObjective.text = selectedQuest.Objective;
            if (TMPQuestObjective != null)
                TMPQuestObjective.text = selectedQuest.Objective;
#endif
            for (int i = 0; i < objectiveTexts.Count; i++)
            {

                if (selectedQuest != null && selectedQuest.gradeInfo != null && selectedQuest.gradeInfo.Count > 0 && selectedQuest.gradeInfo[0].objectives != null && i < selectedQuest.gradeInfo[0].objectives.Count)
                {
                    objectiveTexts[i].gameObject.SetActive(true);
#if AT_I2LOC_PRESET
                string objectives = selectedQuest.gradeInfo[0].objectives[i];
                if (objectives != null && objectives != "" && objectives != ": 0/0") {
                    string nameOjective = "";
                    if (objectives.IndexOf(" slain:") != -1) {
                        string objectivesNames = I2.Loc.LocalizationManager.GetTranslation("Quests/" + objectives.Remove(objectives.IndexOf(" slain:")));
                        nameOjective = I2.Loc.LocalizationManager.GetTranslation("slain") + " " + objectivesNames;
                    } else if (objectives.IndexOf(" collect:") != -1) {
                        string objectivesNames = I2.Loc.LocalizationManager.GetTranslation("Quests/" + objectives.Remove(objectives.IndexOf(" collect:")));
                        nameOjective = I2.Loc.LocalizationManager.GetTranslation("collect") + " " + objectivesNames;
                    } else if (objectives.IndexOf(":") != -1) {
                        nameOjective = I2.Loc.LocalizationManager.GetTranslation("Quests/" + objectives.Remove(objectives.LastIndexOf(':')));
                    }
                    string valueObjective = objectives.Remove(0, objectives.LastIndexOf(':') < 0 ? 0 : objectives.LastIndexOf(':'));
                    if (history) valueObjective = "";
                    objectiveTexts[i].text = nameOjective + " " + valueObjective;
                }
                else objectiveTexts[i].text = "";
#else
                    objectiveTexts[i].text = selectedQuest.gradeInfo[0].objectives[i];
#endif
                }
                else
                {
                    objectiveTexts[i].gameObject.SetActive(false);
                }
            }
            for (int i = 0; i < TMPObjectiveTexts.Count; i++)
            {

                if (selectedQuest != null && selectedQuest.gradeInfo != null && selectedQuest.gradeInfo.Count > 0 && selectedQuest.gradeInfo[0].objectives != null && i < selectedQuest.gradeInfo[0].objectives.Count)
                {
                    TMPObjectiveTexts[i].gameObject.SetActive(true);
#if AT_I2LOC_PRESET
                string objectives = selectedQuest.gradeInfo[0].objectives[i];
                if (objectives != null && objectives != "" && objectives != ": 0/0") {
                    string nameOjective = "";
                    if (objectives.IndexOf(" slain:") != -1) {
                        string objectivesNames = I2.Loc.LocalizationManager.GetTranslation("Quests/" + objectives.Remove(objectives.IndexOf(" slain:")));
                        nameOjective = I2.Loc.LocalizationManager.GetTranslation("slain") + " " + objectivesNames;
                    } else if (objectives.IndexOf(" collect:") != -1) {
                        string objectivesNames = I2.Loc.LocalizationManager.GetTranslation("Quests/" + objectives.Remove(objectives.IndexOf(" collect:")));
                        nameOjective = I2.Loc.LocalizationManager.GetTranslation("collect") + " " + objectivesNames;
                    } else if (objectives.IndexOf(":") != -1) {
                        nameOjective = I2.Loc.LocalizationManager.GetTranslation("Quests/" + objectives.Remove(objectives.LastIndexOf(':')));
                    }
                    string valueObjective = objectives.Remove(0, objectives.LastIndexOf(':') < 0 ? 0 : objectives.LastIndexOf(':'));
                    if (history) valueObjective = "";
                    TMPObjectiveTexts[i].text = nameOjective + " " + valueObjective;
                }
                else TMPObjectiveTexts[i].text = "";
#else
                    TMPObjectiveTexts[i].text = selectedQuest.gradeInfo[0].objectives[i];
#endif
                }
                else
                {
                    TMPObjectiveTexts[i].gameObject.SetActive(false);
                }
            }
#if AT_I2LOC_PRESET
       if (questDescription != null) questDescription.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + selectedQuest.Description);
       if (TMPQuestDescription != null) TMPQuestDescription.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + selectedQuest.Description);
#else
            if (questDescription != null)
                questDescription.text = selectedQuest.Description;
            if (TMPQuestDescription != null)
                TMPQuestDescription.text = selectedQuest.Description;
#endif
            int i0 = 0;
            if (!history && selectedQuest != null && selectedQuest.gradeInfo != null && selectedQuest.gradeInfo.Count > 0 && selectedQuest.gradeInfo[0].expReward != null && selectedQuest.gradeInfo[0].expReward > 0)
            {
                itemRewards[i0].gameObject.SetActive(true);
#if AT_I2LOC_PRESET
             if (itemRewards[i0].itemName!=null)   itemRewards[i0].itemName.text = selectedQuest.gradeInfo[0].expReward.ToString() + " " + I2.Loc.LocalizationManager.GetTranslation("EXP");
             if (itemRewards[i0].TMPItemName!=null)   itemRewards[i0].TMPItemName.text = selectedQuest.gradeInfo[0].expReward.ToString() + " " + I2.Loc.LocalizationManager.GetTranslation("EXP");
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
            else
            {
                AtavismLogger.LogWarning("No Exp reward");
            }

            // Item Rewards
            for (int i = i0; i < itemRewards.Count; i++)
            {
                if (!history && selectedQuest != null && selectedQuest.gradeInfo != null && i - i0 < selectedQuest.gradeInfo[0].rewardItems.Count)
                {
                    itemRewards[i].gameObject.SetActive(true);
                    itemRewards[i].SetItemData(selectedQuest.gradeInfo[0].rewardItems[i - i0].item, null);
                }
                else
                {
                    itemRewards[i].gameObject.SetActive(false);
                }
            }
            if (rewardPanel != null)
            {
                if ((selectedQuest != null && selectedQuest.gradeInfo != null && selectedQuest.gradeInfo.Count > 0 && selectedQuest.gradeInfo[0].rewardItems.Count == 0 && selectedQuest.gradeInfo[0].expReward == 0) || history)
                    rewardPanel.gameObject.SetActive(false);
                else
                    rewardPanel.gameObject.SetActive(true);
            }
            if (rewardTitle != null)
            {
                if ((selectedQuest != null && selectedQuest.gradeInfo != null && selectedQuest.gradeInfo.Count > 0 && selectedQuest.gradeInfo[0].rewardItems.Count == 0 && selectedQuest.gradeInfo[0].currencies.Count == 0 )|| history)
                    rewardTitle.gameObject.SetActive(false);
                else
                    rewardTitle.gameObject.SetActive(true);
            }
            if (TMPRewardTitle != null)
            {
                if ((selectedQuest != null && selectedQuest.gradeInfo != null && selectedQuest.gradeInfo.Count > 0 && selectedQuest.gradeInfo[0].rewardItems.Count == 0 && selectedQuest.gradeInfo[0].currencies.Count == 0) || history)
                    TMPRewardTitle.gameObject.SetActive(false);
                else
                    TMPRewardTitle.gameObject.SetActive(true);
            }

            // Item Choose Rewards
            for (int i = 0; i < chooseRewards.Count; i++)
            {
                if (!history && selectedQuest != null && selectedQuest.gradeInfo != null && selectedQuest.gradeInfo.Count > 0 && i < selectedQuest.gradeInfo[0].RewardItemsToChoose.Count)
                {
                    chooseRewards[i].gameObject.SetActive(true);
                    chooseRewards[i].SetItemData(selectedQuest.gradeInfo[0].RewardItemsToChoose[i].item, null);
                }
                else
                {
                    chooseRewards[i].gameObject.SetActive(false);
                }
            }
            if (choosePanel != null)
            {
                if (selectedQuest != null && selectedQuest.gradeInfo != null && selectedQuest.gradeInfo.Count > 0 && selectedQuest.gradeInfo[0].RewardItemsToChoose.Count == 0 || history)
                    choosePanel.gameObject.SetActive(false);
                else
                    choosePanel.gameObject.SetActive(true);
            }
            if (chooseTitle != null)
            {
                if (selectedQuest != null && selectedQuest.gradeInfo != null && selectedQuest.gradeInfo.Count > 0 && selectedQuest.gradeInfo[0].RewardItemsToChoose.Count == 0 || history)
                    chooseTitle.gameObject.SetActive(false);
                else
                    chooseTitle.gameObject.SetActive(true);
            }
            if (TMPChooseTitle != null)
            {
                if (selectedQuest != null && selectedQuest.gradeInfo != null && selectedQuest.gradeInfo.Count > 0 && selectedQuest.gradeInfo[0].RewardItemsToChoose.Count == 0 || history)
                    TMPChooseTitle.gameObject.SetActive(false);
                else
                    TMPChooseTitle.gameObject.SetActive(true);
            }

            // Currency Rewards
            if (!history && selectedQuest != null && selectedQuest.gradeInfo != null && selectedQuest.gradeInfo.Count > 0 && selectedQuest.gradeInfo[0].currencies != null && selectedQuest.gradeInfo[0].currencies.Count > 0)
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
            if (!history && selectedQuest != null && selectedQuest.gradeInfo != null && selectedQuest.gradeInfo.Count > 0 && selectedQuest.gradeInfo[0].currencies != null && selectedQuest.gradeInfo[0].currencies.Count > 1)
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

            if (selectedQuest != null && selectedQuest.gradeInfo != null && selectedQuest.gradeInfo.Count > 0 && selectedQuest.gradeInfo[0].rewardRep != null && selectedQuest.gradeInfo[0].rewardRep.Count > 0)
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

        public void LocalizeTargets()
        {
            Quests.Instance.ClickedQuest(Quests.Instance.GetSelectedQuestLogEntry());
        }

        public void HistoryQuests()
        {
            if (historicalButton)
            {
                historicalButton.targetGraphic.color = buttonMenuSelectedColor;
            }
            if (historicalButtonText)
            {
                historicalButtonText.color = buttonMenuSelectedTextColor;
            }
            if (activeButton)
            {
                activeButton.targetGraphic.color = buttonMenuNormalColor;
            }
            if (activeButtonText)
            {
                activeButtonText.color = buttonMenuNormalTextColor;
            }
            Quests.Instance.QuestLogEntrySelected(-1);
            history = true;
            ClearAllCells();
            Refresh();
            questDetailsPanel.GetComponent<CanvasGroup>().alpha = 0f;
            questDetailsPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        public void ActiveQuests()
        {
            history = false;
            if (historicalButton)
            {
                historicalButton.targetGraphic.color = buttonMenuNormalColor;
            }
            if (historicalButtonText)
            {
                historicalButtonText.color = buttonMenuNormalTextColor;
            }
            if (activeButton)
            {
                activeButton.targetGraphic.color = buttonMenuSelectedColor;
            }
            if (activeButtonText)
            {
                activeButtonText.color = buttonMenuSelectedTextColor;
            }
            Quests.Instance.QuestHistoryLogEntrySelected(-1);
            ClearAllCells();
            Refresh();
            questDetailsPanel.GetComponent<CanvasGroup>().alpha = 0f;
            questDetailsPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        public bool History
        {
            get
            {
                return history;
            }
        }

        public void AbandonQuest()
        {
#if AT_I2LOC_PRESET
        if (questTitle != null)  UGUIConfirmationPanel.Instance.ShowConfirmationBox(I2.Loc.LocalizationManager.GetTranslation("Are you sure you want abandon") + " " + I2.Loc.LocalizationManager.GetTranslation("Quests/" + questTitle.text) + "?", null, AbandonQuest);
        if (TMPQuestTitle != null)  UGUIConfirmationPanel.Instance.ShowConfirmationBox(I2.Loc.LocalizationManager.GetTranslation("Are you sure you want abandon") + " " + I2.Loc.LocalizationManager.GetTranslation("Quests/" + TMPQuestTitle.text) + "?", null, AbandonQuest);
#else
            if (questTitle != null)
                UGUIConfirmationPanel.Instance.ShowConfirmationBox("Are you sure you want abandon " + questTitle.text + "?", null, AbandonQuest);
            if (TMPQuestTitle != null)
                UGUIConfirmationPanel.Instance.ShowConfirmationBox("Are you sure you want abandon " + TMPQuestTitle.text + "?", null, AbandonQuest);
#endif

        }

        public void AbandonQuest(object item, bool accepted)
        {
            if (accepted)
            {
                Quests.Instance.AbandonQuest();
                questDetailsPanel.GetComponent<CanvasGroup>().alpha = 0f;
                questDetailsPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
                ClearAllCells();
                Refresh();
            }
        }


#region implemented abstract members of AtList

        public override int NumberOfCells()
        {
            int numCells;
            if (history)
                numCells = Quests.Instance.QuestHistoryLogEntries.Count;
            else
                numCells = Quests.Instance.QuestLogEntries.Count;
            return numCells;
        }

        public override void UpdateCell(int index, UGUIQuestListEntry cell)
        {
            if (history)
                cell.SetQuestEntryDetails(Quests.Instance.QuestHistoryLogEntries[index], index, this);
            else
                cell.SetQuestEntryDetails(Quests.Instance.QuestLogEntries[index], index, this);
        }

#endregion
    }
}