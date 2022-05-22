using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Atavism
{
    public class AuctionCountSlot : MonoBehaviour
    {
        public List<UGUICurrency> currencies = new List<UGUICurrency>();
        public TextMeshProUGUI totalCount;
        public Image background;
        public Image checkedfull;
        public Image partialChecked;
        public Color bgNormalColor = new Color(1, 1, 1, 0);
        public Color bgSelectedColor = new Color(1, 1, 1, 0.1f);
        AuctionCountPrice obj;
        public void SetDetale(AuctionCountPrice v)
        {
            obj = v;
            if (totalCount != null)
                totalCount.text = v.count.ToString();

            //  Debug.LogError("AuctionCountSlot: price:" + v.price + "curr:" + v.currency + " count:" + v.count);

            List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(v.currency, v.price);
            for (int i = 0; i < currencies.Count; i++)
            {
                if (i < currencyDisplayList.Count)
                {
                    currencies[i].gameObject.SetActive(true);
                    currencies[i].SetCurrencyDisplayData(currencyDisplayList[i]);
                }
                else
                {
                    currencies[i].gameObject.SetActive(false);
                }
            }
            if (background != null)
            {
                background.color = bgNormalColor;
            }
            if (checkedfull != null)
                checkedfull.enabled = false;
            if (partialChecked != null)
                partialChecked.enabled = false;
        }

        public void setFull()
        {
            //  Debug.LogError("AuctionCountSlot: setFull" );
            if (background != null)
            {
                background.color = bgSelectedColor;
            }
            if (checkedfull != null)
                checkedfull.enabled = true;
            if (partialChecked != null)
                partialChecked.enabled = false;
        }

        public void setPartial()
        {
            //  Debug.LogError("AuctionCountSlot: setPartial");
            if (background != null)
            {
                background.color = bgSelectedColor;
            }
            if (checkedfull != null)
                checkedfull.enabled = false;
            if (partialChecked != null)
                partialChecked.enabled = true;

        }
        public void Reset()
        {
            if (background != null)
                background.color = bgNormalColor;
            if (checkedfull != null)
                checkedfull.enabled = false;
            if (partialChecked != null)
                partialChecked.enabled = false;
        }

    }
}