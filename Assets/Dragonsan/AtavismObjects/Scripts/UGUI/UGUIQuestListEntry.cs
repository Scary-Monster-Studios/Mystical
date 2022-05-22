using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;

namespace Atavism
{

    public class UGUIQuestListEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        //	public Text questTitleText;
        public Text questTitleText;
        public TextMeshProUGUI questTitleTextTMP;
        public Toggle questSelectedToggle;
        public Text questLevelText;
        public TextMeshProUGUI questLevelTextTMP;
        public Image select;
        QuestLogEntry entry;
        int questPos;
        UGUIQuestList questList;
        bool selected = false;

        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("UPDATE_LANGUAGE", this);

        }
        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("UPDATE_LANGUAGE", this);

        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseEntered = true;
            /*   if (!selected)
                   select.enabled = true;*/
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseEntered = false;
            /*  if (!selected)
                  select.enabled = false;*/
        }

        public void QuestEntryClicked()
        {
            if (questList.History)
                Quests.Instance.QuestHistoryLogEntrySelected(questPos);
            else
                Quests.Instance.QuestLogEntrySelected(questPos);
            questList.SetQuestDetails();
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "UPDATE_LANGUAGE")
            {
                if (this.entry != null)
                    UpdateDisplay();
            }
        }


        public void SetQuestEntryDetails(QuestLogEntry entry, int pos, UGUIQuestList questList)
        {
            this.entry = entry;
            this.questPos = pos;
            this.questList = questList;
            UpdateDisplay();

        }

        private void UpdateDisplay()
        {
            if (AtavismSettings.Instance != null && AtavismSettings.Instance.GetQuestListSelected() != null &&   (!AtavismSettings.Instance.GetQuestListSelected().ContainsKey(ClientAPI.GetPlayerOid())))
                AtavismSettings.Instance.GetQuestListSelected().Add(ClientAPI.GetPlayerOid(), new List<long>());

            if (GetComponent<Image>() != null)
            {
                QuestLogEntry selectedQuest;
                if (questList.History)
                    selectedQuest = Quests.Instance.GetSelectedQuestHistoryLogEntry();
                else
                    selectedQuest = Quests.Instance.GetSelectedQuestLogEntry();

                if (selectedQuest != null && entry != null)
                    if (selectedQuest.QuestId == entry.QuestId)
                    {
                        select.enabled = true;
                        selected = true;
                    }
                    else
                    {
                        select.enabled = false;
                        selected = false;
                    }
            }


            if (questSelectedToggle)
            {
                if (questList.History)
                    questSelectedToggle.gameObject.SetActive(false);
                else
                    questSelectedToggle.gameObject.SetActive(true);

                if (AtavismSettings.Instance != null && AtavismSettings.Instance.GetQuestListSelected() != null &&
                   
                    AtavismSettings.Instance.GetQuestListSelected()[ClientAPI.GetPlayerOid()].Contains(entry.QuestId.ToLong()))
                    questSelectedToggle.isOn = true;
                else
                    questSelectedToggle.isOn = false;
            }
            if (entry.Complete)
            {
#if AT_I2LOC_PRESET
           if (questTitleText!=null)
            this.questTitleText.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + entry.Title) + " (" + I2.Loc.LocalizationManager.GetTranslation("Complete") + ")";
           if (questTitleTextTMP!=null)
            this.questTitleTextTMP.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + entry.Title) + " (" + I2.Loc.LocalizationManager.GetTranslation("Complete") + ")";
#else
                if (questTitleText != null)
                    this.questTitleText.text = entry.Title + " (Complete)";
                if (questTitleTextTMP != null)
                    this.questTitleTextTMP.text = entry.Title + " (Complete)";
#endif
            }
            else
            {
#if AT_I2LOC_PRESET
           if (questTitleText!=null)
            this.questTitleText.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + entry.Title);
           if (questTitleTextTMP!=null)
            this.questTitleTextTMP.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + entry.Title);
#else
                if (questTitleText != null)
                    this.questTitleText.text = entry.Title;
                if (questTitleTextTMP != null)
                    this.questTitleTextTMP.text = entry.Title;
#endif
            }

            if (questLevelText != null)
                this.questLevelText.text = "1";
            // this.questSelectedToggle.isOn = true;
        }

        public void ClickQuestListToogle()
        {
            if (AtavismSettings.Instance != null && AtavismSettings.Instance.GetQuestListSelected() != null && (!AtavismSettings.Instance.GetQuestListSelected().ContainsKey(ClientAPI.GetPlayerOid())))
                AtavismSettings.Instance.GetQuestListSelected().Add(ClientAPI.GetPlayerOid(), new List<long>());

            if (questSelectedToggle.isOn)
            {
                if (AtavismSettings.Instance != null && AtavismSettings.Instance.GetQuestListSelected() != null && !AtavismSettings.Instance.GetQuestListSelected()[ClientAPI.GetPlayerOid()].Contains(entry.QuestId.ToLong()))
                {
                    if (AtavismSettings.Instance.GetQuestListSelected()[ClientAPI.GetPlayerOid()].Count < AtavismSettings.Instance.GetQuestPrevLimit)
                        AtavismSettings.Instance.GetQuestListSelected()[ClientAPI.GetPlayerOid()].Add(entry.QuestId.ToLong());
                    else
                    {
                        questSelectedToggle.isOn = false;
                        string[] arg = new string[1];
#if AT_I2LOC_PRESET
                    arg[0] = I2.Loc.LocalizationManager.GetTranslation("QuestPrevLimit") + " " + AtavismSettings.Instance.GetQuestPrevLimit;
#else
                        arg[0] = "Limit selected quests is " + AtavismSettings.Instance.GetQuestPrevLimit;
#endif
                        AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", arg);
                        return;
                    }
                }
            }
            else
            {
                if (AtavismSettings.Instance != null && AtavismSettings.Instance.GetQuestListSelected() != null && AtavismSettings.Instance.GetQuestListSelected()[ClientAPI.GetPlayerOid()].Contains(entry.QuestId.ToLong()))
                    AtavismSettings.Instance.GetQuestListSelected()[ClientAPI.GetPlayerOid()].Remove(entry.QuestId.ToLong());
            }
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("QUEST_LOG_LIST_UPDATE", args);
        }

        void HideTooltip()
        {
            UGUITooltip.Instance.Hide();
        }

        public bool MouseEntered
        {
            set
            {
            }
        }
    }
}