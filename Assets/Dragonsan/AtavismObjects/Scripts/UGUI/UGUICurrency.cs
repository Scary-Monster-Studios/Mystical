using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUICurrency : MonoBehaviour
    {

        public Image image;
        public Text amountText;
        [SerializeField] TextMeshProUGUI TMPAmountText;

        // Use this for initialization
        void Start()
        {

        }

        public void UpdateCurrency(Currency c)
        {
            if (c != null)
            {
                if (image != null)
                    image.sprite = c.icon;
                if (amountText != null)
                    amountText.text = c.Current.ToString();
                if (TMPAmountText != null)
                    TMPAmountText.text = c.Current.ToString();
            }
        }

        public void SetCurrencyDisplayData(CurrencyDisplay display)
        {
            if (amountText != null)
                amountText.text = display.amount.ToString();
            if (TMPAmountText != null)
                TMPAmountText.text = display.amount.ToString();
            if (image != null)
                image.sprite = display.icon;
        }
    }
}