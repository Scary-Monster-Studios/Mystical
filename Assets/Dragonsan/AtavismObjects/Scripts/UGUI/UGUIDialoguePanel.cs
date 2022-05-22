using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

namespace Atavism
{

    public class UGUIDialoguePanel : MonoBehaviour, IPointerDownHandler
    {

        public UGUIPanelTitleBar titleBar;
        public RectTransform dialogueContentPanel;
        public Text dialogueText;
        public TextMeshProUGUI TMPDialogueText;
        public TextMeshProUGUI windowTitle;
        public List<UGUIDialogueOption> interactionOptions;
        public List<UGUIDialogueBarButton> bottomBarButtons;
        public UGUIQuestOffer questOfferPanel;
        public UGUIQuestProgress questProgressPanel;
        public UGUIQuestConclude questConcludePanel;

        // Use this for initialization
        void Start()
        {
            if (titleBar != null)
                titleBar.SetOnPanelClose(Hide);

            Hide();
            // Register for 
            AtavismEventSystem.RegisterEvent("NPC_INTERACTIONS_UPDATE", this);
            AtavismEventSystem.RegisterEvent("DIALOGUE_UPDATE", this);
            AtavismEventSystem.RegisterEvent("QUEST_OFFERED_UPDATE", this);
            AtavismEventSystem.RegisterEvent("QUEST_PROGRESS_UPDATE", this);
            AtavismEventSystem.RegisterEvent("CLOSE_NPC_DIALOGUE", this);
            AtavismEventSystem.RegisterEvent("MERCHANT_UI_OPENED", this);
            AtavismEventSystem.RegisterEvent("UPDATE_LANGUAGE", this);
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("NPC_INTERACTIONS_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("DIALOGUE_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("QUEST_OFFERED_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("QUEST_PROGRESS_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("CLOSE_NPC_DIALOGUE", this);
            AtavismEventSystem.UnregisterEvent("MERCHANT_UI_OPENED", this);
            AtavismEventSystem.UnregisterEvent("UPDATE_LANGUAGE", this);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            // Focus the window
            AtavismUIUtility.BringToFront(this.gameObject);
        }
        public void Show()
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            string mName = "";
            if (NpcInteraction.Instance.NpcId != null && ClientAPI.GetObjectNode(NpcInteraction.Instance.NpcId.ToLong()) != null)
            {
               // mName = ClientAPI.GetObjectNode(NpcInteraction.Instance.NpcId.ToLong()).Name;
                mName = (string)ClientAPI.GetObjectNode(NpcInteraction.Instance.NpcId.ToLong()).GetProperty("displayName");
#if AT_I2LOC_PRESET
        if (!string.IsNullOrEmpty(I2.Loc.LocalizationManager.GetTranslation("Mobs/" + mName))) mName = I2.Loc.LocalizationManager.GetTranslation("Mobs/" + mName);
#endif
                //        if (NpcInteraction.Instance.NpcId != null && ClientAPI.GetObjectNode(NpcInteraction.Instance.NpcId.ToLong()) != null) {
                if (windowTitle != null)
                    windowTitle.text = mName.ToUpper();
                if (titleBar != null)
                    titleBar.SetPanelTitle(mName);
            }
            else
            {
                if (titleBar != null)
                    titleBar.SetPanelTitle("");
                if (windowTitle != null)
                    windowTitle.text = "";
            }
            AtavismUIUtility.BringToFront(this.gameObject);
        }

        public void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;

            dialogueContentPanel.gameObject.SetActive(false);
            questOfferPanel.gameObject.SetActive(false);
            questProgressPanel.gameObject.SetActive(false);
            questConcludePanel.gameObject.SetActive(false);
        }

        public void OnEvent(AtavismEventData eData)
        {
            dialogueContentPanel.gameObject.SetActive(false);
            questOfferPanel.gameObject.SetActive(false);
            questProgressPanel.gameObject.SetActive(false);
            questConcludePanel.gameObject.SetActive(false);
            if (eData.eventType == "NPC_INTERACTIONS_UPDATE")
            {
                dialogueContentPanel.gameObject.SetActive(true);
                ShowOptions();
            }
            else if (eData.eventType == "DIALOGUE_UPDATE")
            {
                dialogueContentPanel.gameObject.SetActive(true);
                ShowChat();
            }
            else if (eData.eventType == "QUEST_OFFERED_UPDATE")
            {
                Show();
                dialogueContentPanel.gameObject.SetActive(false);
                questOfferPanel.gameObject.SetActive(true);
                questOfferPanel.UpdateQuestOfferDetails();
                HideButtonBars();
                if (bottomBarButtons.Count > 0)
                {
                    bottomBarButtons[0].gameObject.SetActive(true);
#if AT_I2LOC_PRESET
                bottomBarButtons[0].SetButtonFunction(questOfferPanel.AcceptQuest, I2.Loc.LocalizationManager.GetTranslation("Accept"));
#else
                    bottomBarButtons[0].SetButtonFunction(questOfferPanel.AcceptQuest, "Accept");
#endif
                    bottomBarButtons[0].GetComponent<Button>().interactable = true;
                }
                if (bottomBarButtons.Count > 1)
                {
                    bottomBarButtons[1].gameObject.SetActive(true);
#if AT_I2LOC_PRESET
                bottomBarButtons[1].SetButtonFunction(questOfferPanel.DeclineQuest, I2.Loc.LocalizationManager.GetTranslation("Decline"));
#else
                    bottomBarButtons[1].SetButtonFunction(questOfferPanel.DeclineQuest, "Decline");
#endif
                    bottomBarButtons[1].GetComponent<Button>().interactable = true;
                }
            }
            else if (eData.eventType == "QUEST_PROGRESS_UPDATE")
            {
                Show();
                dialogueContentPanel.gameObject.SetActive(false);
                questProgressPanel.gameObject.SetActive(true);
                questProgressPanel.UpdateQuestProgressDetails();
                HideButtonBars();
                if (bottomBarButtons.Count > 0)
                {
                    bottomBarButtons[0].gameObject.SetActive(true);
#if AT_I2LOC_PRESET
                bottomBarButtons[0].SetButtonFunction(questProgressPanel.Continue, I2.Loc.LocalizationManager.GetTranslation("Continue"));
#else
                    bottomBarButtons[0].SetButtonFunction(questProgressPanel.Continue, "Continue");
#endif
                    if (Quests.Instance.GetQuestProgressInfo(0).Complete)
                    {
                        bottomBarButtons[0].GetComponent<Button>().interactable = true;
                    }
                    else
                    {
                        bottomBarButtons[0].GetComponent<Button>().interactable = false;
                    }
                }
                if (bottomBarButtons.Count > 1)
                {
                    bottomBarButtons[1].gameObject.SetActive(true);
#if AT_I2LOC_PRESET
                bottomBarButtons[1].SetButtonFunction(questProgressPanel.Cancel, I2.Loc.LocalizationManager.GetTranslation("Cancel"));
#else
                    bottomBarButtons[1].SetButtonFunction(questProgressPanel.Cancel, "Cancel");
#endif
                    bottomBarButtons[1].GetComponent<Button>().interactable = true;
                }
            }
            else if (eData.eventType == "CLOSE_NPC_DIALOGUE")
            {
                Hide();
            }
            else if (eData.eventType == "MERCHANT_UI_OPENED")
            {
                Hide();
            }
            else if (eData.eventType == "UPDATE_LANGUAGE")
            {
                if (NpcInteraction.Instance.NpcId != null && ClientAPI.GetObjectNode(NpcInteraction.Instance.NpcId.ToLong()) != null)
                {
                    string mName = ClientAPI.GetObjectNode(NpcInteraction.Instance.NpcId.ToLong()).Name;
#if AT_I2LOC_PRESET
                if (I2.Loc.LocalizationManager.GetTranslation("Mobs/" + mName) != "") mName = I2.Loc.LocalizationManager.GetTranslation("Mobs/" + mName);
#endif
                    if (windowTitle != null)
                        windowTitle.text = mName.ToUpper();
                }
                else
                {
                    if (windowTitle != null)
                        windowTitle.text = "";
                }

            }
        }

        public void ShowQuestList()
        {
            questProgressPanel.gameObject.SetActive(false);
            questConcludePanel.gameObject.SetActive(false);
            questOfferPanel.gameObject.SetActive(false);
            dialogueContentPanel.gameObject.SetActive(true);
            if (NpcInteraction.Instance.InteractionOptions.Count == 0)
            {
                Hide();
                return;
            }
            ShowOptions();

        }

        void ShowOptions()
        {
            Show();

            Dialogue d = NpcInteraction.Instance.Dialogue;
            if (d != null)
            {
                string dialText = d.text;
#if AT_I2LOC_PRESET
                dialText =  I2.Loc.LocalizationManager.GetTranslation("Quests/" +dialText);
#endif
                if (dialogueText != null)
                    dialogueText.text =dialText;
                if (TMPDialogueText != null)
                    TMPDialogueText.text = dialText;
            }
            else
            {
                if (dialogueText != null)
                    dialogueText.text = "";
                if (TMPDialogueText != null)
                    TMPDialogueText.text = "";
            }
            for (int i = 0; i < interactionOptions.Count; i++)
            {
                if (i < NpcInteraction.Instance.InteractionOptions.Count)
                {
                    interactionOptions[i].gameObject.SetActive(true);
                    interactionOptions[i].SetNpcInteraction(NpcInteraction.Instance.InteractionOptions[i]);
                }
                else
                {
                    interactionOptions[i].gameObject.SetActive(false);
                }
            }

            // Only show one bottom bar, close
            HideButtonBars();
            if (bottomBarButtons.Count > 0)
            {
                bottomBarButtons[0].gameObject.SetActive(true);
#if AT_I2LOC_PRESET
            bottomBarButtons[0].SetButtonFunction(Hide, I2.Loc.LocalizationManager.GetTranslation("Close"));
#else
                bottomBarButtons[0].SetButtonFunction(Hide, "Close");
#endif
            }
        }

        void ShowChat()
        {
            Show();

            Dialogue d = NpcInteraction.Instance.Dialogue;
            if (d != null)
            {
              //  Debug.LogError("Dialogue show "+d+" "+d.audioClip);
                if (d.audioClip.Length > 0)
                {
                    AtavismNpcAudioManager.Instance.PlayAudio(d.audioClip, ClientAPI.GetObjectNode(NpcInteraction.Instance.NpcId.ToLong()).GameObject, NpcInteraction.Instance.NpcId.ToLong());
                }
                string dialText = d.text;
#if AT_I2LOC_PRESET
                dialText =  I2.Loc.LocalizationManager.GetTranslation("Quests/" +dialText);
#endif
                if (dialogueText != null)
                    dialogueText.text =dialText;
                if (TMPDialogueText != null)
                    TMPDialogueText.text = dialText;
            }

            for (int i = 0; i < interactionOptions.Count; i++)
            {
                if (i < NpcInteraction.Instance.Dialogue.actions.Count)
                {
                    interactionOptions[i].gameObject.SetActive(true);
                    interactionOptions[i].SetNpcInteraction(NpcInteraction.Instance.Dialogue.actions[i]);
                }
                else
                {
                    interactionOptions[i].gameObject.SetActive(false);
                }
            }

            // Only show one bottom bar, close
            HideButtonBars();
            if (bottomBarButtons.Count > 0)
            {
                bottomBarButtons[0].gameObject.SetActive(true);
#if AT_I2LOC_PRESET
            bottomBarButtons[0].SetButtonFunction(Hide, I2.Loc.LocalizationManager.GetTranslation("Close"));
#else
                bottomBarButtons[0].SetButtonFunction(Hide, "Close");
#endif
            }
        }

        public void ShowConcludeQuestPanel()
        {
            questConcludePanel.gameObject.SetActive(true);
            questConcludePanel.UpdateQuestConcludeDetails();
            questProgressPanel.gameObject.SetActive(false);

            HideButtonBars();
            if (bottomBarButtons.Count > 0)
            {
                bottomBarButtons[0].gameObject.SetActive(true);
#if AT_I2LOC_PRESET
            bottomBarButtons[0].SetButtonFunction(questConcludePanel.CompleteQuest, I2.Loc.LocalizationManager.GetTranslation("Complete"));
#else
                bottomBarButtons[0].SetButtonFunction(questConcludePanel.CompleteQuest, "Complete");
#endif
            }
            if (bottomBarButtons.Count > 1)
            {
                bottomBarButtons[1].gameObject.SetActive(true);
#if AT_I2LOC_PRESET
            bottomBarButtons[1].SetButtonFunction(questProgressPanel.Cancel, I2.Loc.LocalizationManager.GetTranslation("Cancel"));
#else
                bottomBarButtons[1].SetButtonFunction(questProgressPanel.Cancel, "Cancel");
#endif
            }
        }

        void HideButtonBars()
        {
            for (int i = 0; i < bottomBarButtons.Count; i++)
            {
                bottomBarButtons[i].gameObject.SetActive(false);
            }
        }
    }
}