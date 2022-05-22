using UnityEngine;

namespace Atavism
{

    public class UGUICurrencyDisplay : MonoBehaviour
    {
        [SerializeField] UGUICurrency currency;
        [SerializeField] int currencyId;

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

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "CURRENCY_UPDATE"||eData.eventType == "CURRENCY_ICON_UPDATE")
            {
                UpdateCurrencies();
            }
        }

        void UpdateCurrencies()
        {
            if (currency != null)
                currency.UpdateCurrency(Inventory.Instance.GetCurrency(currencyId));
        }

    }

}