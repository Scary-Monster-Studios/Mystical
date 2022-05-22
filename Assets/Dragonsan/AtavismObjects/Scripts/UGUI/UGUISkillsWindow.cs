using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;

namespace Atavism
{
    [Serializable]
    public class SkillTypeButton
    {
        public int typeId = 0;
        public Button Button;
        public TextMeshProUGUI ButtonText;

    }
    public class UGUISkillsWindow : AtList<UGUISkillButton>
    {
        [SerializeField] bool talents = false;
        public UGUIPanelTitleBar titleBar;
        public bool autoFillAbilityList = true;
        public bool onlyShowClassSkills = false;
        public bool onlyShowKnownSkills = false;
        public bool showMaxLevel = true;
        public Image skillIcon;
        public Text skillName;
        public TextMeshProUGUI TMPSkillName;
        public Text skillLevel;
        public TextMeshProUGUI TMPSkillLevel;
        public TextMeshProUGUI TMPSkillExp;
        public Slider TMPSkillExpSlider;
        public Image SkillExpFill;
        public Button increaseButton;
        public Text costText;
        public TextMeshProUGUI TMPCostText;
        public Text pointsText;
        public TextMeshProUGUI TMPPointsText;
        public List<UGUISkillButton> skillButtons;
        public UGUIAbilitiesList abilitiesList;
        List<Skill> activeSkills = new List<Skill>();
        List<Skill> activeCrftSkills = new List<Skill>();
        int selectedSkill = 0;
        public GameObject tabSpells;
        public GameObject tabCraftSpells;
        //  public Text tabSpellsText;
        //  public Text tabCraftSpellsText;
        //public GameObject tabGatheringSpells;
        public UGUICraftSkillSlot[] craftSlots;
        bool showing = false;
       // [SerializeField] bool skillNoAbilityToCraft = false;
        [SerializeField] List<SkillTypeButton> skillTypeButtons = new List<SkillTypeButton>();
        [Obsolete]
        [SerializeField] Button SkillButton;
        [Obsolete]
        [SerializeField] Button CraftSkillButton;
        [Obsolete]
        [SerializeField] Button GatherSkillButton;
        [Obsolete]
        [SerializeField] TextMeshProUGUI SkillButtonText;
        [Obsolete]
        [SerializeField] TextMeshProUGUI CraftSkillButtonText;
        [Obsolete]
        [SerializeField] TextMeshProUGUI GatherSkillButtonText;
        [SerializeField] Color buttonMenuSelectedColor = Color.green;
        [SerializeField] Color buttonMenuNormalColor = Color.white;
        [SerializeField] Color buttonMenuSelectedTextColor = Color.black;
        [SerializeField] Color buttonMenuNormalTextColor = Color.black;

        int type = -1;
        bool withAbilities = true;
        [SerializeField] int defaultSkillType = 1;
        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("ABILITY_UPDATE", this);
            AtavismEventSystem.RegisterEvent("SKILL_UPDATE", this);
            AtavismEventSystem.RegisterEvent("SKILL_ICON_UPDATE", this);
            UpdateSkills();
            Hide();
            if (titleBar != null)
            {
                titleBar.SetPanelTitle(ClientAPI.GetPlayerObject().Name);
            }
        }

        void OnEnable()
        {
            UpdateSkills();
        }

        void OnDisable()
        {
            // Delete the old list
            ClearAllCells();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("ABILITY_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("SKILL_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("SKILL_ICON_UPDATE", this);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "ABILITY_UPDATE" || eData.eventType == "SKILL_UPDATE"|| eData.eventType == "SKILL_ICON_UPDATE")
            {
                // Update 
                UpdateSkills();
            }
        }

        public void SelectSkill(int skillPos)
        {
            selectedSkill = skillPos;
            UpdateSkills();
        }

        public void IncreaseSkill()
        {
            Skills.Instance.IncreaseSkill(activeSkills[selectedSkill].id);
        }

        public void ResetSkill()
        {
            Skills.Instance.ResetSkills(talents);
        }

        public void UpdateSkills()
        {
            if (ClientAPI.GetPlayerObject() != null)
                if (ClientAPI.GetPlayerObject().PropertyExists("aspect"))
                {
                    int classID = (int)ClientAPI.GetPlayerObject().GetProperty("aspect");
                    // Get list of skills
                    activeSkills.Clear();
                    activeCrftSkills.Clear();
                    if (onlyShowKnownSkills)
                    {
                        foreach (Skill skill in Skills.Instance.PlayerSkills.Values)
                        {
                            if ((!onlyShowClassSkills && !skill.mainAspectOnly) || skill.mainAspect == classID)
                            {
                               // Debug.LogError("UGUISkillsWindow UpdateSkills skills=" +skill.skillname +  " type=" + skill.type );

                                if (skill.type == type || type==-1)
                                {
                                    if (withAbilities)
                                    {

                                        if (talents && skill.talent)
                                            activeSkills.Add(skill);
                                        else if (!talents && !skill.talent)
                                            activeSkills.Add(skill);
                                    }
                                    else
                                    {
                                     //   Debug.LogError("UGUISkillsWindow UpdateSkills skills=" + skill.skillname + " type=" + skill.type+ " add activeSkills");
                                        activeCrftSkills.Add(skill);
                                    }
                                }

                               /*     if (skill.abilities.Count > 0 || !skillNoAbilityToCraft)
                                {
                                    if (talents && skill.talent)
                                        activeSkills.Add(skill);
                                    else if (!talents && !skill.talent)
                                        activeSkills.Add(skill);
                                }
                                else
                                    activeCrftSkills.Add(skill);
                                    */
                            }
                        }
                    }
                    else
                    {
                        foreach (Skill skill in Skills.Instance.SkillsList.Values)
                        {
                            if ((!onlyShowClassSkills && !skill.mainAspectOnly) || skill.mainAspect == classID)
                            {
                                if (skill.type == type || type == -1)
                                {
                                    if (withAbilities)
                                    {
                                        if (talents && skill.talent)
                                            activeSkills.Add(skill);
                                        else if (!talents && !skill.talent)
                                            activeSkills.Add(skill);
                                    }
                                    else
                                    {
                                    //    Debug.LogError("UGUISkillsWindow UpdateSkills skills=" + skill.skillname + " type=" + skill.type + " add activeSkills");
                                        activeCrftSkills.Add(skill);
                                    }
                                }
                            }
                        }
                    }

                //    Debug.LogError("UGUISkillsWindow UpdateSkills skills=" + activeSkills.Count + " craft=" + activeCrftSkills.Count + " type=" + type + " " + withAbilities);

                    /*for (int i = 0; i < skillButtons.Count; i++) {
                        if (i < activeSkills.Count) {
                            skillButtons[i].gameObject.SetActive(true);
                            skillButtons[i].SetSkillData(activeSkills[i]);
                        } else {
                            skillButtons[i].gameObject.SetActive(false);
                        }
                    }*/
                  /*  if (activeSkills.Count == 0)
                    {
                        if (skillIcon != null)
                        {
                            skillIcon.enabled = false;
                        }
                        if (skillName != null)
                            skillName.text = "";
                        if (TMPSkillName != null)
                            TMPSkillName.text = "";
                        if (skillLevel != null)
                            skillLevel.text = "-";
                        // Delete the old list
                        ClearAllCells();
                        Refresh();

                        return;
                    }*/
                    // Update skill info
                    if (skillIcon != null)
                    {
                        if (activeSkills.Count == 0)
                        {
                            skillIcon.enabled = false;
                        }
                        else
                        {
                            skillIcon.enabled = true;
                            skillIcon.sprite = activeSkills[selectedSkill].icon;
                        }
                    }
                    if (skillName != null)
                    {
                        if (activeSkills.Count == 0)
                            skillName.text = "";
                        else
                            skillName.text = activeSkills[selectedSkill].skillname;
                    }
                    if (TMPSkillName != null)
                    {
                        if (activeSkills.Count == 0)
                            TMPSkillName.text = "";
                        else
                            TMPSkillName.text = activeSkills[selectedSkill].skillname;
                    }
                    if (skillLevel != null)
                    {
                        
                            
                        if (activeSkills.Count > selectedSkill && Skills.Instance.PlayerSkills.ContainsKey(activeSkills[selectedSkill].id))
                        {
                            Skill playerSkill = Skills.Instance.PlayerSkills[activeSkills[selectedSkill].id];
                            if (showMaxLevel)
                            {
                                skillLevel.text = playerSkill.CurrentLevel + "/" + playerSkill.MaximumLevel;
                            }
                            else
                            {
                                skillLevel.text = playerSkill.CurrentLevel.ToString();
                            }
                        }
                        else
                        {
                            skillLevel.text = "-";
                        }
                    }
                    if (TMPSkillLevel != null)
                    {
                        if (activeSkills.Count > selectedSkill && Skills.Instance.PlayerSkills.ContainsKey(activeSkills[selectedSkill].id))
                        {
                            Skill playerSkill = Skills.Instance.PlayerSkills[activeSkills[selectedSkill].id];
                            if (showMaxLevel)
                            {
                                TMPSkillLevel.text = playerSkill.CurrentLevel + "/" + playerSkill.MaximumLevel;
                            }
                            else
                            {
                                TMPSkillLevel.text = playerSkill.CurrentLevel.ToString();
                            }
                        }
                        else
                        {
                            TMPSkillLevel.text = "-";
                        }
                    }

                    if (activeSkills.Count > selectedSkill && Skills.Instance.PlayerSkills.ContainsKey(activeSkills[selectedSkill].id))
                    {
                       // Skill _skill = Skills.Instance.GetSkillByID(activeSkills[selectedSkill].id);
                        Skill _skillPly = Skills.Instance.PlayerSkills[activeSkills[selectedSkill].id];
                        if (TMPSkillExpSlider != null)
                        {
                            if (!TMPSkillExpSlider.isActiveAndEnabled)
                                TMPSkillExpSlider.enabled = true;
                            if (_skillPly.expMax == 0 && _skillPly.exp == 0)
                            {
                                TMPSkillExpSlider.maxValue = _skillPly.MaximumLevel;
                                TMPSkillExpSlider.value = _skillPly.CurrentLevel;
                            }
                            else
                            {
                                TMPSkillExpSlider.maxValue = _skillPly.expMax;
                                TMPSkillExpSlider.value = _skillPly.exp;
                            }
                        }
                        if (SkillExpFill != null)
                        {
                            SkillExpFill.fillAmount = (float)(((float)_skillPly.exp) / (float)_skillPly.expMax);
                        }

                        if (TMPSkillExp != null)
                        {
                            if (_skillPly.expMax == 0 && _skillPly.exp == 0)
                            {
                                TMPSkillExp.text = _skillPly.CurrentLevel + "/" + _skillPly.MaximumLevel;
                            }
                            else
                            {
                                TMPSkillExp.text = _skillPly.exp + "/" + _skillPly.expMax;
                            }
                        }

                        //  Debug.LogError("updateRecipeList " + _skillPly);
                        //    Debug.LogError("updateRecipeList " + _skillPly.exp + " " + _skillPly.expMax);
                    }
                    else
                    {
                        if (TMPSkillExpSlider != null)
                        {
                            TMPSkillExpSlider.enabled = false;
                        }
                        if (TMPSkillExp != null)
                        {
                            TMPSkillExp.text = "";
                        }
                    }


                    if (costText != null)
                    {
                        if (activeSkills.Count > selectedSkill)
                        {
                            if (Skills.Instance.PlayerSkills.ContainsKey(activeSkills[selectedSkill].id))
                            {
                                Skill playerSkill = Skills.Instance.PlayerSkills[activeSkills[selectedSkill].id];
                                int cost = playerSkill.pcost;
                                if (playerSkill.mainAspect == classID)
                                    cost = playerSkill.pcost;
                                if (playerSkill.oppositeAspect == classID)
                                    cost = playerSkill.pcost * 2;
                                costText.text = cost.ToString();
                            }
                            else
                            {
                                Skill _skill = Skills.Instance.SkillsList[activeSkills[selectedSkill].id];
                                int cost = _skill.pcost;
                                if (_skill.mainAspect == classID)
                                    cost = _skill.pcost;
                                if (_skill.oppositeAspect == classID)
                                    cost = _skill.pcost * 2;
                                costText.text = cost.ToString();
                            }
                        }
                        else
                        {
                            costText.text = "";
                        }
                    }
                    if (TMPCostText != null)
                    {
                        if (activeSkills.Count > selectedSkill)
                        {

                            if (Skills.Instance.PlayerSkills.ContainsKey(activeSkills[selectedSkill].id))
                            {
                                Skill playerSkill = Skills.Instance.PlayerSkills[activeSkills[selectedSkill].id];
                                int cost = playerSkill.pcost;
                                if (playerSkill.mainAspect == classID)
                                    cost = playerSkill.pcost;
                                if (playerSkill.oppositeAspect == classID)
                                    cost = playerSkill.pcost * 2;
                                TMPCostText.text = cost.ToString();
                            }
                            else
                            {
                                Skill _skill = Skills.Instance.SkillsList[activeSkills[selectedSkill].id];
                                int cost = _skill.pcost;
                                if (_skill.mainAspect == classID)
                                    cost = _skill.pcost;
                                if (_skill.oppositeAspect == classID)
                                    cost = _skill.pcost * 2;
                                TMPCostText.text = cost.ToString();

                            }
                        }
                        else
                        {
                            TMPCostText.text = "";
                        }
                    }
                    if (pointsText != null)
                    {
                        if (talents)
                            pointsText.text = Skills.Instance.CurrentTalentPoints.ToString();
                        else
                            pointsText.text = Skills.Instance.CurrentSkillPoints.ToString();
                    }
                    if (TMPPointsText != null)
                    {
                        if (talents)
                            TMPPointsText.text = Skills.Instance.CurrentTalentPoints.ToString();
                        else
                            TMPPointsText.text = Skills.Instance.CurrentSkillPoints.ToString();
                    }
                  //  Debug.LogError("Skill "+craftSlots.Length+" "+ activeCrftSkills.Count);
                    if (craftSlots.Length > 0)
                        for (int i = 0; i < craftSlots.Length; i++)
                        {
                            if (activeCrftSkills.Count > i)
                            {
                                craftSlots[i].gameObject.SetActive(true);
                                craftSlots[i].UpdateDisplay(activeCrftSkills[i]);
                            }
                            else
                            {
                                craftSlots[i].gameObject.SetActive(false);
                            }
                        }

                    if (autoFillAbilityList)
                    {
                        // Delete the old list
                        ClearAllCells();
                        Refresh();
                    }
                    if (activeSkills.Count > 0)
                    {
                        abilitiesList.UpdateAbilities(activeSkills[selectedSkill]);
                    }
                    /*List<Ability> abilities = ClientAPI.ScriptObject.GetComponent<Abilities>().PlayerAbilities;
                    for (int i = 0; i < slots.Count; i++) {
                        if (abilities.Count > i) {
                            slots[i].UpdateAbilityData(abilities[i]);
                        } else {
                            slots[i].UpdateAbilityData(null);
                        }
                    }*/
                }
        }

        void Update()
        {
            if (Input.GetKeyDown(AtavismSettings.Instance.GetKeySettings().skills) && !ClientAPI.UIHasFocus())
            {
                if (showing)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
            }
        }
        public void Show()
        {
            showing = true;
            if (GetComponent<CanvasGroup>() != null)
                this.GetComponent<CanvasGroup>().alpha = 1f;
            if (GetComponent<CanvasGroup>() != null)
                this.GetComponent<CanvasGroup>().blocksRaycasts = true;
            this.UpdateSkills();
            //   gameObject.SetActive(true);
            AtavismUIUtility.BringToFront(gameObject);
            ShowSkillsAbility(defaultSkillType);
        }
        public void Hide()
        {
            //   gameObject.SetActive(false);
            showing = false;

            if (GetComponent<CanvasGroup>() != null)
                GetComponent<CanvasGroup>().alpha = 0f;
            if (GetComponent<CanvasGroup>() != null)
                GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        public void ShowSkillsAbility(int type)
        {
            this.type = type;
            withAbilities = true;
            if (tabSpells != null)
                tabSpells.SetActive(true);
            if (tabCraftSpells != null)
                tabCraftSpells.SetActive(false);

            foreach (SkillTypeButton stb in skillTypeButtons)
            {
                if (stb.typeId == type)
                {
                    if (stb.Button)
                        stb.Button.targetGraphic.color = buttonMenuSelectedColor;
                    if (stb.ButtonText)
                        stb.ButtonText.color = buttonMenuSelectedTextColor;
                }
                else
                {
                    if(stb.Button)
                        stb.Button.targetGraphic.color = buttonMenuNormalColor;
                    if (stb.ButtonText)
                        stb.ButtonText.color = buttonMenuNormalTextColor;
                }
            }

          /*  if (SkillButton)
                SkillButton.targetGraphic.color = buttonMenuSelectedColor;
            if (SkillButtonText)
                SkillButtonText.color = buttonMenuSelectedTextColor;
            if (CraftSkillButton)
                CraftSkillButton.targetGraphic.color = buttonMenuNormalColor;
            if (CraftSkillButtonText)
                CraftSkillButtonText.color = buttonMenuNormalTextColor;*/
            UpdateSkills();
        }

        public void ShowSkills(int type)
        {
            this.type = type;
            withAbilities = false;

            foreach (SkillTypeButton stb in skillTypeButtons)
            {
                if (stb.typeId == type)
                {
                    if (stb.Button)
                        stb.Button.targetGraphic.color = buttonMenuSelectedColor;
                    if (stb.ButtonText)
                        stb.ButtonText.color = buttonMenuSelectedTextColor;
                }
                else
                {
                    if (stb.Button)
                        stb.Button.targetGraphic.color = buttonMenuNormalColor;
                    if (stb.ButtonText)
                        stb.ButtonText.color = buttonMenuNormalTextColor;
                }
            }

            if (tabSpells != null)
                tabSpells.SetActive(false);
            if (tabCraftSpells != null)
                tabCraftSpells.SetActive(true);

         /*   if (SkillButton)
                SkillButton.targetGraphic.color = buttonMenuNormalColor;
            if (SkillButtonText)
                SkillButtonText.color = buttonMenuNormalTextColor;
            if (CraftSkillButton)
                CraftSkillButton.targetGraphic.color = buttonMenuSelectedColor;
            if (CraftSkillButtonText)
                CraftSkillButtonText.color = buttonMenuSelectedTextColor;*/
            UpdateSkills();
        }

      /*  public void ShowGatherSkills()
        {
            if (tabSpells != null)
                tabSpells.SetActive(false);
            if (tabCraftSpells != null)
                tabCraftSpells.SetActive(true);
            if (SkillButton)
                SkillButton.targetGraphic.color = buttonMenuNormalColor;
            if (SkillButtonText)
                SkillButtonText.color = buttonMenuNormalTextColor;
            if (CraftSkillButton)
                CraftSkillButton.targetGraphic.color = buttonMenuSelectedColor;
            if (CraftSkillButtonText)
                CraftSkillButtonText.color = buttonMenuSelectedTextColor;
        }
        */


        public void Toggle()
        {
            if (showing)
                Hide();
            else
                Show();
        }

        #region implemented abstract members of AtList

        public override int NumberOfCells()
        {
            int numCells = activeSkills.Count;
            return numCells;
        }

        public override void UpdateCell(int index, UGUISkillButton cell)
        {
            Skill skill = activeSkills[index];
            cell.SetSkillData(skill, this, index, selectedSkill);
        }

        #endregion
    }
}