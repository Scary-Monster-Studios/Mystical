using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atavism
{
    public class UGUIShopClose : MonoBehaviour
    {
        OID shop = null;
        bool isshop = false;
        // Start is called before the first frame update
        void Start()
        {
            ClientAPI.GetPlayerObject().RegisterPropertyChangeHandler("playerShop", shopHandle);
            if (ClientAPI.GetPlayerObject().PropertyExists("playerShop"))
            {
                isshop = (bool)ClientAPI.GetPlayerObject().GetProperty("playerShop");
                if (!isshop)
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void shopHandle(object sender, PropertyChangeEventArgs args)
        {

            isshop = (bool)ClientAPI.GetPlayerObject().GetProperty("playerShop");
            if (isshop)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if(ClientAPI.GetPlayerObject()!=null)
                ClientAPI.GetPlayerObject().RemovePropertyChangeHandler("playerShop", shopHandle);
        }
        public void Click()
        {
            if (isshop)
            {
                if (ClientAPI.GetPlayerObject().PropertyExists("plyShopId"))
                {
                    shop = (OID)ClientAPI.GetPlayerObject().GetProperty("plyShopId");

                }
                UGUIConfirmationPanel.Instance.ShowConfirmationBox(AtavismPlayerShop.Instance.CloseShopConfirmMessage, shop, AtavismPlayerShop.Instance.CloseShopConfirmed);
            }
        }
    }
}