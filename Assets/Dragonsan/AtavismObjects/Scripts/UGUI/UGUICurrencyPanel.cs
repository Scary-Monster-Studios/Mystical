using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{

    public class UGUICurrencyPanel : MonoBehaviour
    {

        public List<UGUICurrency> currencies;
        public int currencyGroupOverride = -1;
        [SerializeField] UGUICurrency greenCoin;

        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("CURRENCY_UPDATE", this);
            AtavismEventSystem.RegisterEvent("CURRENCY_ICON_UPDATE", this);
            UpdateCurrencies();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("CURRENCY_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("CURRENCY_ICON_UPDATE", this);
        }

        void OnEnable()
        {
            UpdateCurrencies();
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (!enabled)
                return;

            if (eData.eventType == "CURRENCY_UPDATE"||eData.eventType == "CURRENCY_ICON_UPDATE")
            {
                UpdateCurrencies();
            }
        }

        void UpdateCurrencies()
        {
            List<Currency> mainCurrencies;
            if (currencyGroupOverride > 0)
            {
                mainCurrencies = Inventory.Instance.GetCurrenciesInGroup(currencyGroupOverride);
            }
            else
            {
                mainCurrencies = Inventory.Instance.GetMainCurrencies();
            }

            for (int i = 0; i < currencies.Count; i++)
            {
                if (i < mainCurrencies.Count)
                {
                    currencies[i].gameObject.SetActive(true);
                    currencies[i].UpdateCurrency(mainCurrencies[mainCurrencies.Count - i - 1]);
                }
                else
                {
                    currencies[i].gameObject.SetActive(false);
                }
            }
            if (greenCoin != null)
                greenCoin.UpdateCurrency(Inventory.Instance.GetCurrency(5));
        }
    }
}