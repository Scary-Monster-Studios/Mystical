using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Atavism
{

    public class UGUITradeOffer : MonoBehaviour
    {

        public UGUIItemDisplay itemDisplay;
        public Text nameLabel;
        public TextMeshProUGUI TMPNameLabel;

        public void UpdateTradeOfferData(AtavismInventoryItem item)
        {
            if (item == null)
            {
                if (nameLabel != null)
                    nameLabel.text = "";
                if (TMPNameLabel != null)
                    TMPNameLabel.text = "";
                itemDisplay.Reset();
            }
            else
            {
                if (nameLabel != null)
                    nameLabel.text = item.name;
                if (TMPNameLabel != null)
                    TMPNameLabel.text = item.name;
                itemDisplay.SetItemData(item, null);
            }
        }
    }
}