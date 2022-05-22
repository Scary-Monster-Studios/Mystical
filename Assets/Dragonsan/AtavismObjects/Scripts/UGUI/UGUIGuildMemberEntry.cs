using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUIGuildMemberEntry : MonoBehaviour, IPointerClickHandler
    {

        public Text nameText;
        public TextMeshProUGUI TMPNameText;
        public Text rankText;
        public TextMeshProUGUI TMPRankText;
        public Text levelText;
        public TextMeshProUGUI TMPLevelText;
        public Text statusText;
        public TextMeshProUGUI TMPStatusText;
        public Color onlineColor = Color.green;
        public Color offlineColor = Color.red;
        public Color awayColor = Color.gray;
        AtavismGuildMember guildMember;
        UGUIGuildPanel guildPanel;

        // Use this for initialization
        void Start()
        {

        }

        public void SetGuildMemberDetails(AtavismGuildMember guildMember, UGUIGuildPanel guildPanel)
        {
            this.guildMember = guildMember;
            this.guildPanel = guildPanel;
            if (nameText != null)
                nameText.text = guildMember.name;
            if (TMPNameText != null)
                TMPNameText.text = guildMember.name;
            string rankName = "";//AtavismGuild.Instance.Ranks.Count > 0 ? AtavismGuild.Instance.Ranks[guildMember.rank].rankName : "";
            foreach (var r in AtavismGuild.Instance.Ranks)
            {
                if (r.rankLevel == guildMember.rank)
                {
                    rankName = r.rankName;
                }
            }

            if (rankText != null)
                rankText.text = rankName;
            if (TMPRankText != null)
                TMPRankText.text = rankName;
            if (levelText != null)
                levelText.text = guildMember.level.ToString();
            if (TMPLevelText != null)
                TMPLevelText.text = guildMember.level.ToString();
#if AT_I2LOC_PRESET
       if (statusText != null)  statusText.text = guildMember.status > 0 ? (guildMember.status > 1 ? I2.Loc.LocalizationManager.GetTranslation("Away") : I2.Loc.LocalizationManager.GetTranslation("Online")) : I2.Loc.LocalizationManager.GetTranslation("Offline");
       if (TMPStatusText != null)  TMPStatusText.text = guildMember.status > 0 ? (guildMember.status > 1 ? I2.Loc.LocalizationManager.GetTranslation("Away") : I2.Loc.LocalizationManager.GetTranslation("Online")) : I2.Loc.LocalizationManager.GetTranslation("Offline");
#else
            if (statusText != null)
                statusText.text = guildMember.status > 0 ? (guildMember.status > 1 ? "Away" : "Online") : "Offline";
            if (TMPStatusText != null)
                TMPStatusText.text = guildMember.status > 0 ? (guildMember.status > 1 ? "Away" : "Online") : "Offline";
#endif
            if (guildMember.status > 0)
            {
                if (guildMember.status > 1)
                {
                    if (TMPStatusText != null)
                        TMPStatusText.color = awayColor;
                    if (statusText != null)
                        statusText.color = awayColor;
                }
                else
                {
                    if (TMPStatusText != null)
                        TMPStatusText.color = onlineColor;
                    if (statusText != null)
                        statusText.color = onlineColor;

                }
            }
            else
            {
                if (TMPStatusText != null)
                    TMPStatusText.color = offlineColor;
                ;
                if (statusText != null)
                    statusText.color = offlineColor;
                ;

            }

        }

        public void GuildMemberClicked()
        {
            AtavismGuild.Instance.SelectedMember = guildMember;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                guildPanel.ShowMemberPopup(this, guildMember);
                GetComponent<Button>().Select();
            }
            else
            {
                guildPanel.HideMemberPopup();
            }
        }
    }
}