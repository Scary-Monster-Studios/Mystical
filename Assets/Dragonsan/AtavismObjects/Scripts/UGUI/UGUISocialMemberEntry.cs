using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUISocialMemberEntry : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {

        public Text nameText;
        public TextMeshProUGUI nameTextTMP;
        public Text levelText;
        public TextMeshProUGUI levelTextTMP;
        public Text statusText;
        public TextMeshProUGUI statusTextTMP;
        public Image select;
        public Color onlineColor = Color.green;
        public Color offlineColor = Color.grey;
        AtavismSocialMember socialMember;
        UGUISocialPanel socialPanel;
        bool selected = false;
        // Use this for initialization
        void Start()
        {

        }

        public void SetSocialMemberDetails(AtavismSocialMember socialMember, UGUISocialPanel socialPanel)
        {
            this.socialMember = socialMember;
            this.socialPanel = socialPanel;
            if (nameText)
                nameText.text = socialMember.name;
            if (nameTextTMP)
                nameTextTMP.text = socialMember.name;
            if (levelText)
                levelText.text = socialMember.level.ToString();
            if (levelTextTMP)
                levelTextTMP.text = socialMember.level.ToString();
#if AT_I2LOC_PRESET
        if(statusText)statusText.text = socialMember.status  ? I2.Loc.LocalizationManager.GetTranslation("Online") : I2.Loc.LocalizationManager.GetTranslation("Online");
        if(statusTextTMP)statusTextTMP.text = socialMember.status ? I2.Loc.LocalizationManager.GetTranslation("Online") : I2.Loc.LocalizationManager.GetTranslation("Online");
#else
            if (statusText)
                statusText.text = socialMember.status ? "Online" : "Offline";
            if (statusTextTMP)
                statusTextTMP.text = socialMember.status ? "Online" : "Offline";
#endif
            if (statusTextTMP)
            {
                if (socialMember.status)
                    statusTextTMP.color = onlineColor;
                else
                    statusTextTMP.color = offlineColor;

            }
        }
        public void SetBlokSocialMemberDetails(AtavismSocialMember socialMember, UGUISocialPanel socialPanel)
        {
            this.socialMember = socialMember;
            this.socialPanel = socialPanel;
            if (nameText)
                nameText.text = socialMember.name;
            if (nameTextTMP)
                nameTextTMP.text = socialMember.name;
            if (levelText)
                levelText.text = "";
            if (levelTextTMP)
                levelTextTMP.text = "";
#if AT_I2LOC_PRESET
        if(statusText)statusText.text = "";
        if(statusTextTMP)statusTextTMP.text = "";
#else
            if (statusText)
                statusText.text = "";
            if (statusTextTMP)
                statusTextTMP.text = "";
#endif
        }



        public void SocialMemberClicked()
        {
            AtavismSocial.Instance.SelectedMember = socialMember;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseEntered = true;
            if (!selected)
                if (select)
                    select.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseEntered = false;
            if (!selected)
                if (select)
                    select.enabled = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                socialPanel.ShowMemberPopup(this, socialMember);
                GetComponent<Button>().Select();
            }
            else
            {
                socialPanel.HideMemberPopup();
            }
        }

        public bool MouseEntered
        {
            set
            {
            }
        }
    }
}