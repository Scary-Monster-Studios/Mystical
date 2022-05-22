using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Atavism
{

    public class UGUISkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {

        Skill skill;
        UGUISkillsWindow skillsWindow;
        int pos;
        public Image icon;
        public Image selectIcon;

        // Use this for initialization
        void Start()
        {

        }

        public void SetSkillData(Skill skill, UGUISkillsWindow skillsWindow, int pos, int select)
        {
            this.skill = skill;
            if (icon == null)
                GetComponent<Image>().sprite = skill.icon;
            this.skillsWindow = skillsWindow;
            this.pos = pos;
            icon.sprite = skill.icon;
            if (selectIcon != null)
            {
                selectIcon.gameObject.SetActive(pos == select);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (skillsWindow != null)
            {
                skillsWindow.SelectSkill(pos);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseEntered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseEntered = false;
        }

        void ShowTooltip()
        {
            if (skill == null)
            {
                HideTooltip();
                return;
            }
#if AT_I2LOC_PRESET
            if (skill.CurrentLevel==0)
                UGUITooltip.Instance.SetTitle(I2.Loc.LocalizationManager.GetTranslation("Ability/" + skill.skillname) );
            else
                UGUITooltip.Instance.SetTitle(I2.Loc.LocalizationManager.GetTranslation("Ability/" + skill.skillname) + " (" + skill.CurrentLevel + ")");
#else
            if (skill.CurrentLevel==0)
                UGUITooltip.Instance.SetTitle(skill.skillname);
            else
                UGUITooltip.Instance.SetTitle(skill.skillname + " (" + skill.CurrentLevel + ")");
#endif
            UGUITooltip.Instance.SetIcon(skill.icon);
            UGUITooltip.Instance.SetType("");
            UGUITooltip.Instance.SetWeight("");
            // UGUITooltip.Instance.HideType(true);
            // UGUITooltip.Instance.HideWeight(true);
            UGUITooltip.Instance.SetDescription("");
            UGUITooltip.Instance.Show(gameObject);
        }

        void HideTooltip()
        {
            UGUITooltip.Instance.Hide();
        }

        public bool MouseEntered
        {
            set
            {
                if (value)
                {
                    ShowTooltip();
                }
                else
                {
                    HideTooltip();
                }
            }
        }
    }
}