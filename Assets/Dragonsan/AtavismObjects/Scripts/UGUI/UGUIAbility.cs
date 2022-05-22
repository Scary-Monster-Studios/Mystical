using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUIAbility : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public int slotNum;
        public Text Name;
        public TextMeshProUGUI TMPName;
        public Text description;
        public TextMeshProUGUI TMPDescription;
        public Text levelReq;
        public TextMeshProUGUI TMPLevelReq;
        public UGUIAbilitySlot abilitySlot;
        AtavismAbility ability;
        bool mouseEntered = false;

        // Use this for initialization
        void Start()
        {
        }

        /*public void OnPointerClick(PointerEventData data)
        {
            switch(data.button)
            {
            case PointerEventData.InputButton.Left:
                PickUpOrPlaceItem();
                break;
            case PointerEventData.InputButton.Right:
                Activate();
                break;
            case PointerEventData.InputButton.Middle:
                break;
            }
        }*/

        public void UpdateAbilityData(AtavismAbility ability)
        {
            this.ability = ability;
            abilitySlot.UpdateAbilityData(ability);
            if (ability == null)
            {
                if (Name != null)
                    Name.text = "";
                if (TMPName != null)
                    TMPName.text = "";
                if (description != null)
                    description.text = "";
                if (TMPDescription != null)
                    TMPDescription.text = "";
                return;
            }
#if AT_I2LOC_PRESET
      if(description!=null) description.text = I2.Loc.LocalizationManager.GetTranslation("Ability/" + ability.tooltip);
      if(TMPDescription!=null) TMPDescription.text = I2.Loc.LocalizationManager.GetTranslation("Ability/" + ability.tooltip);
      if(Name!=null)   Name.text = I2.Loc.LocalizationManager.GetTranslation("Ability/" + ability.name);
     if(TMPName!=null)   TMPName.text = I2.Loc.LocalizationManager.GetTranslation("Ability/" + ability.name);
#else
            if (Name != null)
                Name.text = ability.name;
            if (TMPName != null)
                TMPName.text = ability.name;
            if (description != null)
                description.text = ability.tooltip;
            if (TMPDescription != null)
                TMPDescription.text = ability.tooltip;

#endif
            if (levelReq != null)
            {
                levelReq.text = "";
                Skill skill = Skills.Instance.GetSkillOfAbility(ability.id);
                if (skill != null)
                {
                    for (int i = 0; i < skill.abilities.Count; i++)
                    {
                        if (skill.abilities[i] == ability.id)
                        {
                            levelReq.text = skill.abilityLevelReqs[i].ToString();
                            break;
                        }
                    }
                }
            }
            if (TMPLevelReq != null)
            {
                TMPLevelReq.text = "";
                Skill skill = Skills.Instance.GetSkillOfAbility(ability.id);
                if (skill != null)
                {
                    for (int i = 0; i < skill.abilities.Count; i++)
                    {
                        if (skill.abilities[i] == ability.id)
                        {
                            TMPLevelReq.text = skill.abilityLevelReqs[i].ToString();
                            break;
                        }
                    }
                }
            }

            // If the player doesn't know this ability, disable this panel
            if (!Abilities.Instance.PlayerAbilities.Contains(ability))
            {
                GetComponent<Image>().color = Color.gray;
            }
            else
            {
                GetComponent<Image>().color = Color.white;
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
        public bool MouseEntered
        {
            get
            {
                return mouseEntered;
            }
            set
            {
                mouseEntered = value;
                if (mouseEntered && ability != null)
                {
                    ability.ShowTooltip(gameObject);
                }
                else
                {
                    HideTooltip();
                }
            }
        }
        void HideTooltip()
        {
            UGUITooltip.Instance.Hide();
        }

    }
}