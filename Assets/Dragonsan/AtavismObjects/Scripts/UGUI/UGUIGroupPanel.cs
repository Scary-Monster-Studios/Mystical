using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Atavism
{

    public class UGUIGroupPanel : MonoBehaviour
    {
        [Header("Dice Panel")]
        [SerializeField] GameObject dicePanel;
        [SerializeField] Image timeRemaning;
        [SerializeField] string timeRemaningPrefix = "Remaining Time";
        [SerializeField] TextMeshProUGUI TMPTimeRemaning;
        [SerializeField] UGUIItemDisplay item;
        [Header("Loot Settings Panel")]
        [SerializeField] Button settingsButton;
        [SerializeField] GameObject panelSettings;
        [SerializeField] Toggle lootFreeForAll;
        [SerializeField] Toggle lootRoundRobin;
        [SerializeField] Toggle lootLeader;
        [SerializeField] Toggle gradeNormal;
        [SerializeField] Toggle gradeDice;
        [SerializeField] TMP_Dropdown gradeDiceMin;
        [SerializeField] Button confirmButton;
        [Header("Group Members List")]
        public List<UGUIGroupMember> memberPanels;

        // Coroutine cor;
        bool showing = false;
        float Expiration = 0;
        float Length = 0;
        bool started = false;
        Coroutine cr;
        // Use this for initialization
        void Start()
        {
            Hide();
            AtavismEventSystem.RegisterEvent("GROUP_UPDATE", this);
            AtavismEventSystem.RegisterEvent("GROUP_DICE", this);
            AtavismEventSystem.RegisterEvent("GROUP_UPDATE_SETTINGS", this);
            UpdateGroupMembers();
           
            //  cor = StartCoroutine(CheckMemeters());
            if (dicePanel && dicePanel.activeSelf)
            {
                dicePanel.SetActive(false);
            }
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("GROUP_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("GROUP_DICE", this);
            StopAllCoroutines();

        }

        public void Show()
        {
            if (!showing)
            {
                GetComponent<CanvasGroup>().alpha = 1f;
                GetComponent<CanvasGroup>().blocksRaycasts = true;
                GetComponent<CanvasGroup>().interactable = true;
                GetComponent<CanvasGroup>().ignoreParentGroups = true;
            }
            if (settingsButton)
            {
                if(!settingsButton.gameObject.activeSelf)
                    settingsButton.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            showing = false;
            started = false;
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            if (panelSettings)
            {
                panelSettings.SetActive(false);
            }
            if (settingsButton)
            {
                if (settingsButton.gameObject.activeSelf)
                    settingsButton.gameObject.SetActive(false);
            }

            //      GetComponent<CanvasGroup>().interactable = false;
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "GROUP_UPDATE")
            {
                if (AtavismGroup.Instance.LeaderOid != null)
                {
                    if (AtavismGroup.Instance.LeaderOid.Equals(OID.fromLong(ClientAPI.GetPlayerOid())))
                    {
                        if (!started)
                        {
                            if (lootFreeForAll)
                                lootFreeForAll.isOn = AtavismGroup.Instance.GetRoll == 0;
                            if (lootRoundRobin)
                                lootRoundRobin.isOn = AtavismGroup.Instance.GetRoll == 1;
                            if (lootLeader)
                                lootLeader.isOn = AtavismGroup.Instance.GetRoll == 2;
                            if (gradeNormal)
                                gradeNormal.isOn = AtavismGroup.Instance.GetDice == 0;
                            if (gradeDice)
                                gradeDice.isOn = AtavismGroup.Instance.GetDice == 1;
                            if (gradeDiceMin)
                                gradeDiceMin.value = AtavismGroup.Instance.GetGrade;
                            started = true;
                        }

                        if (confirmButton)
                            confirmButton.interactable = true;
                        if (gradeDiceMin)
                            gradeDiceMin.interactable = true;
                        if (lootFreeForAll)
                            lootFreeForAll.interactable = true;
                        if (lootRoundRobin)
                            lootRoundRobin.interactable = true;
                        if (lootLeader)
                            lootLeader.interactable = true;
                        if (gradeNormal)
                            gradeNormal.interactable = true;
                        if (gradeDice)
                            gradeDice.interactable = true;
                        if (gradeDiceMin)
                            gradeDiceMin.interactable = true;
                    }
                    else
                    {
                        if (lootFreeForAll)
                            lootFreeForAll.isOn = AtavismGroup.Instance.GetRoll == 0;
                        if (lootRoundRobin)
                            lootRoundRobin.isOn = AtavismGroup.Instance.GetRoll == 1;
                        if (lootLeader)
                            lootLeader.isOn = AtavismGroup.Instance.GetRoll == 2;
                        if (gradeNormal)
                            gradeNormal.isOn = AtavismGroup.Instance.GetDice == 0;
                        if (gradeDice)
                            gradeDice.isOn = AtavismGroup.Instance.GetDice == 1;
                        if (gradeDiceMin)
                            gradeDiceMin.value = AtavismGroup.Instance.GetGrade;

                        if (confirmButton)
                            confirmButton.interactable = false;
                        if (gradeDiceMin)
                            gradeDiceMin.interactable = false;
                        if (lootFreeForAll)
                            lootFreeForAll.interactable = false;
                        if (lootRoundRobin)
                            lootRoundRobin.interactable = false;
                        if (lootLeader)
                            lootLeader.interactable = false;
                        if (gradeNormal)
                            gradeNormal.interactable = false;
                        if (gradeDice)
                            gradeDice.interactable = false;
                        if (gradeDiceMin)
                            gradeDiceMin.interactable = false;
                    }
                }
               
                // Update 
                UpdateGroupMembers();
                for (int i = 0; i < memberPanels.Count; i++)
                {
                    if (i < AtavismGroup.Instance.Members.Count)
                    {
                        memberPanels[i].CheckEffects();
                    }
                }

            }
            else if (eData.eventType == "GROUP_DICE")
            {
                if (dicePanel && !dicePanel.activeSelf)
                {
                    dicePanel.SetActive(true);
                    AtavismUIUtility.BringToFront(dicePanel);
                    Length = int.Parse(eData.eventArgs[0]);
                    Expiration = Time.time + Length;
                    int diceitem = int.Parse(eData.eventArgs[1]);
                    if (item)
                    {
                        AtavismInventoryItem _item = Inventory.Instance.GetItemByTemplateID(diceitem);
                        item.SetItemData(_item, null);
                    }
                    cr = StartCoroutine(UpdateTimer());
                }
            }
        }

        public void UpdateGroupMembers()
        {
            if (AtavismGroup.Instance.Members.Count == 0)
            {
                for (int i = 0; i < memberPanels.Count; i++)
                {
                    memberPanels[i].deactive();
                    memberPanels[i].gameObject.SetActive(false);
                }
                Hide();
                return;
            }
            else
            {
                Show();
            }

            for (int i = 0; i < memberPanels.Count; i++)
            {
                if (i < AtavismGroup.Instance.Members.Count)
                {
                    memberPanels[i].gameObject.SetActive(true);
                    memberPanels[i].UpdateGroupMember(AtavismGroup.Instance.Members[i]);
                }
                else
                {
                    memberPanels[i].deactive();
                    memberPanels[i].gameObject.SetActive(false);
                }
            }
        }
        IEnumerator CheckMemeters()
        {
            WaitForSeconds delay = new WaitForSeconds(1f);
            while (true)
            {
                UpdateGroupMembers();
                yield return delay;
            }
        }

        public void ShowSettings()
        {
            panelSettings.SetActive(true);

            if (AtavismGroup.Instance.LeaderOid != null)
            {
                if (AtavismGroup.Instance.LeaderOid.Equals(OID.fromLong(ClientAPI.GetPlayerOid())))
                {
                    if (confirmButton)
                        confirmButton.interactable = true;
                    if (gradeDiceMin)
                        gradeDiceMin.interactable = true;
                    if (lootFreeForAll)
                        lootFreeForAll.interactable = true;
                    if (lootRoundRobin)
                        lootRoundRobin.interactable = true;
                    if (lootLeader)
                        lootLeader.interactable = true;
                    if (gradeNormal)
                        gradeNormal.interactable = true;
                    if (gradeDice)
                        gradeDice.interactable = true;
                    if (gradeDiceMin)
                        gradeDiceMin.interactable = true;
                }
                else
                {
                    if (confirmButton)
                        confirmButton.interactable = false;
                    if (gradeDiceMin)
                        gradeDiceMin.interactable = false;
                    if (lootFreeForAll)
                        lootFreeForAll.interactable = false;
                    if (lootRoundRobin)
                        lootRoundRobin.interactable = false;
                    if (lootLeader)
                        lootLeader.interactable = false;
                    if (gradeNormal)
                        gradeNormal.interactable = false;
                    if (gradeDice)
                        gradeDice.interactable = false;
                    if (gradeDiceMin)
                        gradeDiceMin.interactable = false;
                }
            }
        }
        public void HideSettings()
        {
            panelSettings.SetActive(false);

        }
        public void Confirm()
        {
            bool ffa = false, rr = false, leader = false, norm = false, dice = false;
            int grade = 0;
            if (lootFreeForAll)
                ffa = lootFreeForAll.isOn;
            if (lootRoundRobin)
                rr = lootRoundRobin.isOn;
            if (lootLeader)
                leader = lootLeader.isOn;
            if (gradeNormal)
                norm = gradeNormal.isOn;
            if (gradeDice)
                dice = gradeDice.isOn;
            if (gradeDiceMin)
                grade = gradeDiceMin.value;
         //   Debug.LogError("Confirm " + gradeDiceMin.value);
            AtavismGroup.Instance.SetLootGroup(ffa, rr, leader, norm, dice, grade);
            HideSettings();
        }
        public void Pass()
        {
            AtavismGroup.Instance.Pass();
            StopCoroutine(cr);
            if (dicePanel && dicePanel.activeSelf)
            {
                dicePanel.SetActive(false);
            }
        }

        public void Roll()
        {
            AtavismGroup.Instance.Roll();
            StopCoroutine(cr);
            if (dicePanel && dicePanel.activeSelf)
            {
                dicePanel.SetActive(false);
            }
        }
        IEnumerator UpdateTimer()
        {
            //  corRuning = true;
            while (Expiration > Time.time)
            {
                float timeLeft = Expiration - Time.time;
                if (timeLeft > 60)
                {
                    int minutes = (int)timeLeft / 60;
                    if (TMPTimeRemaning != null)
                    {
#if AT_I2LOC_PRESET
            TMPTimeRemaning.text =  I2.Loc.LocalizationManager.GetTranslation(timeRemaningPrefix)+" " + (int)minutes + "m";
#else
                        TMPTimeRemaning.text = timeRemaningPrefix +" "+ (int)minutes + "m";
#endif
                    }
                }
                else
                {
                    if (TMPTimeRemaning != null)
                    {
#if AT_I2LOC_PRESET
            TMPTimeRemaning.text = I2.Loc.LocalizationManager.GetTranslation(timeRemaningPrefix)+" " + (int)timeLeft + "s";
#else
                        TMPTimeRemaning.text = timeRemaningPrefix +" " + (int)timeLeft + "s";
#endif
                    }
                }
                if (timeRemaning != null)
                    timeRemaning.fillAmount = timeLeft / Length;
                yield return new WaitForSeconds(0.04f);
            }
          
            if (timeRemaning != null)
                timeRemaning.fillAmount = 1f;
            if (dicePanel && dicePanel.activeSelf)
            {
                dicePanel.SetActive(false);
            }
        }




    }
}