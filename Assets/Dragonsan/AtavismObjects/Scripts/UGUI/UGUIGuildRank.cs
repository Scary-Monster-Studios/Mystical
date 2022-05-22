using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{
    public class UGUIGuildRank : MonoBehaviour
    {
        public TextMeshProUGUI rankTextTMP;
        public Text rankText;
        public InputField input;
        public TMP_InputField inputTMP;
        public Button deleteButton;

        int rankId = -1;
        public void UpdateRankName()
        {
            if (input != null && input.text.Length > 0)
                if (rankId >= 0)
                    AtavismGuild.Instance.SendGuildCommand("editRank", null, rankId + ";rename;" + input.text);
                else
                    AtavismGuild.Instance.SendGuildCommand("addRank", null, input.text);
            if (inputTMP != null && inputTMP.text.Length > 0)
                if (rankId >= 0)
                    AtavismGuild.Instance.SendGuildCommand("editRank", null, rankId + ";rename;" + inputTMP.text);
                else
                    AtavismGuild.Instance.SendGuildCommand("addRank", null, inputTMP.text);

        }
        public void DeleteRank()
        {
            AtavismGuild.Instance.SendGuildCommand("delRank", null, rankId.ToString());
        }
        public int setRankId
        {
            set
            {
                this.rankId = value;
            }
        }
    }
}