using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Atavism
{
    public class UGUIShopListEntry : MonoBehaviour
    {
        public TextMeshProUGUI TMPName;
        OID shop = null;
        public UGUIMiniTooltipEvent tooltip;
       
        // Update is called once per frame
        public void UpdateDisplay(string msg, OID shop)
        {
          //  Debug.LogError("UGUIShopListEntry. UpdateDisplay " + msg + " " + shop);
            this.shop = shop;
            if (TMPName != null)
                TMPName.text = msg;
            if (tooltip != null)
                tooltip.dectName = msg;
            //   Debug.LogError("UGUIShopListEntry. UpdateDisplay " + TMPName.text);
        }

        public void Click()
        {
            UGUIConfirmationPanel.Instance.ShowConfirmationBox(AtavismPlayerShop.Instance.CloseShopConfirmMessage, shop, AtavismPlayerShop.Instance.CloseShopConfirmed);
        }
      
    }
}