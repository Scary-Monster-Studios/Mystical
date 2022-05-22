using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Atavism
{

    public class UGUIQuestLogPrev : MonoBehaviour
    {
        [SerializeField] Text questTitle;
        [SerializeField] TextMeshProUGUI TMPQuestTitle;
        [SerializeField] Text questObjective;
        [SerializeField] TextMeshProUGUI TMPQuestObjective;
        [SerializeField] List<Text> objectiveTexts;
        [SerializeField] List<TextMeshProUGUI> TMPObjectiveTexts;
        QuestLogEntry quest;


        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("QUEST_LOG_UPDATE", this);
            AtavismEventSystem.RegisterEvent("UPDATE_LANGUAGE", this);
        }

        void Awake()
        {


            //   Hide();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("QUEST_LOG_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("UPDATE_LANGUAGE", this);
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void SetQuestPrev(QuestLogEntry selectedQuest)
        {
            //QuestLogEntry selectedQuest = Quests.Instance.GetSelectedListQuestLog[ii];
            this.quest = selectedQuest;
            if (this.quest != null)
                UpdateQuestPrev();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eData"></param>

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "QUEST_LOG_UPDATE" || eData.eventType == "UPDATE_LANGUAGE")
            {
                // Delete the old list
                if (this.quest != null)
                    UpdateQuestPrev();
            }
        }


        /// <summary>
        /// Funkca aktualizuja wyświetlanie podgladu questa
        /// </summary>

        private void UpdateQuestPrev()
        {
            if (questTitle != null)
                if (quest.Complete)
                {
#if AT_I2LOC_PRESET

                questTitle.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + quest.Title) + " (" + I2.Loc.LocalizationManager.GetTranslation("Complete") + ")";
#else
                    questTitle.text = quest.Title + " (Complete)";
#endif
                }
                else
                {
#if AT_I2LOC_PRESET
                questTitle.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + quest.Title);
#else
                    questTitle.text = quest.Title;
#endif
                }
            if (TMPQuestTitle != null)
                if (quest.Complete)
                {
#if AT_I2LOC_PRESET

                TMPQuestTitle.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + quest.Title) + " (" + I2.Loc.LocalizationManager.GetTranslation("Complete") + ")";
#else
                    TMPQuestTitle.text = quest.Title + " (Complete)";
#endif
                }
                else
                {
#if AT_I2LOC_PRESET
                TMPQuestTitle.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + quest.Title);
#else
                    TMPQuestTitle.text = quest.Title;
#endif
                }



            if (questObjective != null)
#if AT_I2LOC_PRESET
            questObjective.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + quest.Objective);
#else
                questObjective.text = quest.Objective;
#endif
            if (TMPQuestObjective != null)
#if AT_I2LOC_PRESET
            TMPQuestObjective.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + quest.Objective);
#else
                TMPQuestObjective.text = quest.Objective;
#endif

            for (int i = 0; i < objectiveTexts.Count; i++)
            {
                if (i < quest.gradeInfo[0].objectives.Count)
                {
                    objectiveTexts[i].gameObject.SetActive(true);
                    string objectives = quest.gradeInfo[0].objectives[i];
#if AT_I2LOC_PRESET
                if (objectives != null && objectives != "" && objectives != ": 0/0")
                {
                    string nameOjective = "";
                    if (objectives.IndexOf(" slain:") != -1)
                    {
                        string objectivesNames = I2.Loc.LocalizationManager.GetTranslation("Quests/" + objectives.Remove(objectives.IndexOf(" slain:")));
                        //    string objectivesValues = data.Remove(0, data.LastIndexOf(':') < 0 ? 0 : data.LastIndexOf(':')+1);
                        nameOjective = I2.Loc.LocalizationManager.GetTranslation("slain") + " " + objectivesNames;
                    }
                    else if (objectives.IndexOf(" collect:") != -1)
                    {
                        string objectivesNames = I2.Loc.LocalizationManager.GetTranslation("Quests/" + objectives.Remove(objectives.IndexOf(" collect:")));
                        //    string objectivesValues = data.Remove(0, data.LastIndexOf(':') < 0 ? 0 : data.LastIndexOf(':')+1);
                        nameOjective = I2.Loc.LocalizationManager.GetTranslation("collect") + " " + objectivesNames;
                    }
                    else if (objectives.IndexOf(":") != -1)
                    {
                        nameOjective = I2.Loc.LocalizationManager.GetTranslation("Quests/" + objectives.Remove(objectives.LastIndexOf(':')));
                    }
                        string valueObjective = objectives.Remove(0, objectives.LastIndexOf(':') < 0 ? 0 : objectives.LastIndexOf(':'));
                    objectiveTexts[i].text = nameOjective + " " + valueObjective;
                }
                else objectiveTexts[i].text = "";

#else
                    objectiveTexts[i].text = objectives;
#endif
                }
                else
                {
                    objectiveTexts[i].gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < TMPObjectiveTexts.Count; i++)
            {
                if (i < quest.gradeInfo[0].objectives.Count)
                {
                    TMPObjectiveTexts[i].gameObject.SetActive(true);
                    string objectives = quest.gradeInfo[0].objectives[i];
#if AT_I2LOC_PRESET
                if (objectives != null && objectives != "" && objectives != ": 0/0")
                {
                    string nameOjective = "";
                    if (objectives.IndexOf(" slain:") != -1)
                    {
                        string objectivesNames = I2.Loc.LocalizationManager.GetTranslation("Quests/" + objectives.Remove(objectives.IndexOf(" slain:")));
                        //    string objectivesValues = data.Remove(0, data.LastIndexOf(':') < 0 ? 0 : data.LastIndexOf(':')+1);
                        nameOjective = I2.Loc.LocalizationManager.GetTranslation("slain") + " " + objectivesNames;
                    }
                    else if (objectives.IndexOf(" collect:") != -1)
                    {
                        string objectivesNames = I2.Loc.LocalizationManager.GetTranslation("Quests/" + objectives.Remove(objectives.IndexOf(" collect:")));
                        //    string objectivesValues = data.Remove(0, data.LastIndexOf(':') < 0 ? 0 : data.LastIndexOf(':')+1);
                        nameOjective = I2.Loc.LocalizationManager.GetTranslation("collect") + " " + objectivesNames;
                    }
                    else if (objectives.IndexOf(":") != -1)
                    {
                        nameOjective = I2.Loc.LocalizationManager.GetTranslation("Quests/" + objectives.Remove(objectives.LastIndexOf(':')));
                    }
                        string valueObjective = objectives.Remove(0, objectives.LastIndexOf(':') < 0 ? 0 : objectives.LastIndexOf(':'));
                    TMPObjectiveTexts[i].text = nameOjective + " " + valueObjective;
                }
                else TMPObjectiveTexts[i].text = "";

#else
                    TMPObjectiveTexts[i].text = objectives;
#endif
                }
                else
                {
                    TMPObjectiveTexts[i].gameObject.SetActive(false);
                }
            }




        }


        //   List<GameObject> qObjects = new List<GameObject>();
        //  List<GameObject> qNpc = new List<GameObject>();
        public void ClickTitle()
        {
            Quests.Instance.ClickedQuest(quest);

        }


    }
}