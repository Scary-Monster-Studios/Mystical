using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Atavism
{

    public class UGUIRankingSlot : MonoBehaviour
    {
        public TextMeshProUGUI playerNameText;
        public TextMeshProUGUI valueFieldText;
        // Start is called before the first frame update
        public void UpdateInfo(int id, string playerName, int value)
        {
            if (playerNameText != null)
            {
                playerNameText.text = id+". "+playerName;
            }
            else
            {
                Debug.LogError("Text componet is not assigned", this);
            }
            if (valueFieldText != null)
            {
                valueFieldText.text = value.ToString();
            }
            else
            {
                Debug.LogError("Text componet is not assigned", this);
            }
        }
    }
}