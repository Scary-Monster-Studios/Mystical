using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Atavism
{
    public class UGUIPlayerShopList : MonoBehaviour
    {
        [SerializeField] UGUIShopListEntry prefab;
        [SerializeField] List<UGUIShopListEntry> shops = new List<UGUIShopListEntry>();
        private bool showing = false;
        [SerializeField] bool autoShowHide = false;
        // Start is called before the first frame update
        void Start()
        {
            AtavismEventSystem.RegisterEvent("SHOP_LIST_UPDATE", this);
            if (AtavismPlayerShop.Instance.ShopList.Count == 0)
            {
                Hide();
            }
            else
            {
                if (autoShowHide)
                {
                    Show();
                }
                UpdateDisplay();
            }
        }
        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("SHOP_LIST_UPDATE", this);
        }


        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "SHOP_LIST_UPDATE")
            {

                if (!showing && autoShowHide)
                    Show();

                UpdateDisplay();
            } else if (eData.eventType == "CLOSE_SHOP")
            {
                Hide();
            }
        }

        void UpdateDisplay()
        {
           

            int i = 0;
            foreach (OID oid in AtavismPlayerShop.Instance.ShopList.Keys)
            {
                if (i >= shops.Count)
                    shops.Add(Instantiate(prefab, transform));
                shops[i].gameObject.SetActive(true);
                shops[i].UpdateDisplay(AtavismPlayerShop.Instance.ShopList[oid], oid);
                i++;
            }
            for (int j = i; j < shops.Count; j++)
                shops[i].gameObject.SetActive(false);

            if (AtavismPlayerShop.Instance.ShopList.Count == 0 && autoShowHide)
            {
                Hide();
                return;
            }
        }

        public void Show()
        {
            AtavismUIUtility.BringToFront(gameObject);
            UpdateDisplay();
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
            showing = true;

        }

        public void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            showing = false;

        }
        public void Toggle()
        {
            if (showing)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }
}