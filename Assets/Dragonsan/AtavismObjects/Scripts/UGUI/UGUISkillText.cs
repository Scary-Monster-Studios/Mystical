using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{
    public class UGUISkillText : MonoBehaviour
    {

        public Text text;
        [SerializeField] TextMeshProUGUI TMPText;
        public int skillID;
        public bool showMax = false;

        // Use this for initialization
        void Start()
        {
            if (text == null)
                text = GetComponent<Text>();
            if (TMPText == null)
                TMPText = GetComponent<TextMeshProUGUI>();

            AtavismEventSystem.RegisterEvent("SKILL_UPDATE", this);
            AtavismEventSystem.RegisterEvent("SKILL_ICON_UPDATE", this);

            UpdateSkillText();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("SKILL_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("SKILL_ICON_UPDATE", this);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "SKILL_UPDATE"||eData.eventType == "SKILL_ICON_UPDATE")
            {
                // Update 
                UpdateSkillText();
            }
        }

        void UpdateSkillText()
        {
            if (showMax && Skills.Instance.PlayerSkills.ContainsKey(skillID))
            {
                if (text != null)
                    text.text = "" + Skills.Instance.GetPlayerSkillLevel(skillID) + "/" + Skills.Instance.PlayerSkills[skillID].MaximumLevel;
                if (TMPText != null)
                    TMPText.text = "" + Skills.Instance.GetPlayerSkillLevel(skillID) + "/" + Skills.Instance.PlayerSkills[skillID].MaximumLevel;
            }
            else
            {
                if (text != null)
                    text.text = "" + Skills.Instance.GetPlayerSkillLevel(skillID);
                if (TMPText != null)
                    TMPText.text = "" + Skills.Instance.GetPlayerSkillLevel(skillID);
            }

        }
    }
}