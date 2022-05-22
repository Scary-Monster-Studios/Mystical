using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Atavism
{
    public class UGUICreateShopEntity : MonoBehaviour
    {
        public TextMeshProUGUI TMPName;
        public UGUIShopItemSlot itemSlot;
        public List<UGUICurrency> currencyDisplays;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void AssignShop(UGUICreateShop acs, int pos)
        {
            if (itemSlot != null)
                itemSlot.AssignShop(acs,pos);
        }

        public void ResetSlot()
        {
            if (TMPName != null)
                TMPName.text = "";
            itemSlot.Discarded();
            for (int i = 0; i < currencyDisplays.Count; i++)
            {
                    currencyDisplays[i].gameObject.SetActive(false);
            }
        }

        public void updateDisplay(ShopItem merchantItem)
        {
            if (merchantItem == null)
            {
                if (TMPName != null)
                    TMPName.text = "";
                itemSlot.ResetSlot();
                for (int i = 0; i < currencyDisplays.Count; i++)
                    currencyDisplays[i].gameObject.SetActive(false);
                return;
            }
            AtavismInventoryItem aii = Inventory.Instance.GetItemByTemplateID(merchantItem.itemID);
            itemSlot.UpdateSlotData(aii);
#if AT_I2LOC_PRESET
        if (TMPName!=null)TMPName.text = I2.Loc.LocalizationManager.GetTranslation("Items/" + aii.name);
#else

            if (TMPName != null)
                TMPName.text = aii.name;
#endif
            List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(merchantItem.purchaseCurrency, merchantItem.cost);
            for (int i = 0; i < currencyDisplays.Count; i++)
            {
                if (i < currencyDisplayList.Count)
                {
                    currencyDisplays[i].gameObject.SetActive(true);
                    currencyDisplays[i].SetCurrencyDisplayData(currencyDisplayList[i]);
                }
                else
                {
                    currencyDisplays[i].gameObject.SetActive(false);
                }
            }
        }
    }
}