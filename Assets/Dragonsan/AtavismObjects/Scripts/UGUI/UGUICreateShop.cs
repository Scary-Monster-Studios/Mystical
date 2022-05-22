using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Atavism
{
    public class ShopItem
    {
        public int itemID;
        public OID itemOID;
        public AtavismInventoryItem item;
        public int count;
        public int sellCount;
        public long cost;
        public int purchaseCurrency;
        public int pos=-1;
    }

    public class UGUICreateShop : MonoBehaviour
    {
        public UGUIPanelTitleBar titleBar;
        public List<UGUICreateShopEntity> buyEntries;
        public List<UGUICreateShopEntity> sellEntries;
        public List<Image> priceCurrencyIcons;
        public List<TMP_InputField> TMPPriceCurrencyInput;
        public TMP_InputField priceQuantity;
        public Slider priceQuantitySlider;
        public List<UGUICurrency> sellSumaryCurrencies;
        public RectTransform buyPanel;
        public RectTransform sellPanel;
        public RectTransform pricePanel;
        public TextMeshProUGUI priceItemName;
        public UGUIItemDisplay priceItemDisplay;
        public List<RectTransform> quantityObjects;

        protected List< ShopItem> sellItems = new List< ShopItem>();
        protected List<ShopItem> buyItems = new List<ShopItem>();
        bool buyMode = false;
        int slots = 10;
        bool showing = false;

        public TMP_InputField messageInput;
        [SerializeField] GameObject panel;
        [SerializeField] List<SkillTypeButton> tabButtons = new List<SkillTypeButton>();
        [SerializeField] Color buttonMenuSelectedColor = Color.green;
        [SerializeField] Color buttonMenuNormalColor = Color.white;
        [SerializeField] Color buttonMenuSelectedTextColor = Color.black;
        [SerializeField] Color buttonMenuNormalTextColor = Color.black;
        // Use this for initialization
        void Start()
        {
            if (titleBar != null)
                titleBar.SetOnPanelClose(CancelShop);
            Hide();
            NetworkAPI.RegisterExtensionMessageHandler("start_player_shop", _HandleCreateShop);
            for (int i = 0; i < buyEntries.Count; i++)
            {
                if (buyEntries[i] != null)
                {

                    buyEntries[i].AssignShop(this, i);
                    buyEntries[i].updateDisplay(null);
                }
            }
            for (int i = 0; i < buyEntries.Count; i++)
            {
                if (sellEntries[i] != null)
                {
                    sellEntries[i].AssignShop(this, i);
                    sellEntries[i].updateDisplay(null);
                }
            }

            if (pricePanel)
                pricePanel.gameObject.SetActive(false);
            AtavismEventSystem.RegisterEvent("ITEM_ICON_UPDATE", this);
        }

        private void _HandleCreateShop(Dictionary<string, object> props)
        {
            slots =(int) props["slots"];
            buyItems.Clear();
            sellItems.Clear();
            for (int i = 0; i < buyEntries.Count; i++)
            {
                buyEntries[i].updateDisplay(null);
                buyEntries[i].ResetSlot();
            }
            for (int i = 0; i < sellEntries.Count; i++)
            {
                sellEntries[i].updateDisplay(null);
                sellEntries[i].ResetSlot();
            }
            Show();
          
        }

        void OnDestroy()
        {
            NetworkAPI.RemoveExtensionMessageHandler("start_player_shop", _HandleCreateShop);
            AtavismEventSystem.UnregisterEvent("ITEM_ICON_UPDATE", this);
        }

        public void showBuy()
        {
            buyPanel.gameObject.SetActive(true);
            sellPanel.gameObject.SetActive(false);
            buyMode = true;
            foreach (SkillTypeButton stb in tabButtons)
            {
                if (stb.typeId == 1)
                {
                    if (stb.Button)
                        stb.Button.targetGraphic.color = buttonMenuSelectedColor;
                    if (stb.ButtonText)
                        stb.ButtonText.color = buttonMenuSelectedTextColor;
                }
                else
                {
                    if (stb.Button)
                        stb.Button.targetGraphic.color = buttonMenuNormalColor;
                    if (stb.ButtonText)
                        stb.ButtonText.color = buttonMenuNormalTextColor;
                }
            }
            int c = 0;
            for (int i = 0; i < buyEntries.Count; i++)
            {
                if (buyEntries[i] != null)
                {
                    if (c < slots - sellItems.Count)
                        buyEntries[i].gameObject.SetActive(true);
                    else
                        buyEntries[i].gameObject.SetActive(false);
                    c++;
                }
            }
            c = 0;
            for (int i = 0; i < sellEntries.Count; i++)
            {
                if (c < slots - buyItems.Count)
                    sellEntries[i].gameObject.SetActive(true);
                else
                    sellEntries[i].gameObject.SetActive(false);
                c++;
            }
        }

        public void showSell()
        {
            sellPanel.gameObject.SetActive(true);
            buyPanel.gameObject.SetActive(false);
            buyMode = false;
            foreach (SkillTypeButton stb in tabButtons)
            {
                if (stb.typeId == 0)
                {
                    if (stb.Button)
                        stb.Button.targetGraphic.color = buttonMenuSelectedColor;
                    if (stb.ButtonText)
                        stb.ButtonText.color = buttonMenuSelectedTextColor;
                }
                else
                {
                    if (stb.Button)
                        stb.Button.targetGraphic.color = buttonMenuNormalColor;
                    if (stb.ButtonText)
                        stb.ButtonText.color = buttonMenuNormalTextColor;
                }
            }
            int c = 0;
            for (int i = 0; i < buyEntries.Count; i++)
            {
                if (buyEntries[i] != null)
                {
                    if (c < slots - sellItems.Count)
                        buyEntries[i].gameObject.SetActive(true);
                    else
                        buyEntries[i].gameObject.SetActive(false);
                    c++;
                }
            }
            c = 0;
            for (int i = 0; i < sellEntries.Count; i++)
            {
                if (c < slots - buyItems.Count)
                    sellEntries[i].gameObject.SetActive(true);
                else
                    sellEntries[i].gameObject.SetActive(false);
                c++;
            }
        }

        public void Show()
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            showing = true;
            showSell();
       

                //    AtavismTrade.Instance.NewTradeStarted();
                AtavismUIUtility.BringToFront(gameObject);
            // Handle currency
            int c = 0;
              for (int i = 0; i < buyEntries.Count; i++)
              {
                if (buyEntries[i] != null)
                {
                    if( c < slots- sellItems.Count)
                        buyEntries[i].gameObject.SetActive(true);
                    else
                        buyEntries[i].gameObject.SetActive(false);
                    c++;
                }
              }
            c = 0;
            for (int i = 0; i < sellEntries.Count; i++)
              {
                if (c < slots - buyItems.Count)
                    sellEntries[i].gameObject.SetActive(true);
                else
                    sellEntries[i].gameObject.SetActive(false);
                c++;
            }
            ResetParams();
            if (panel != null)
                panel.SetActive(true);
        }

        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            showing = false;

            // Set all referenced items back to non referenced
            /* for (int i = 0; i < buyEntries.Count; i++)
             {
                 myOfferEntries[i].ResetSlot();
             }*/
        }

        void UpdateWindow()
        {
           /* for (int i = 0; i < buyEntries.Count; i++)
            {
                  buyEntries[i].updateDisplay(null);
            }
            for (int i = 0; i < sellEntries.Count; i++)
            {
                sellEntries[i].updateDisplay(null);
            }*/

            // Handle currency
            /*  for (int i = 0; i < myCurrencyIcons.Count; i++)
              {
                  if (i < Inventory.Instance.GetMainCurrencies().Count)
                  {
                      myCurrencyIcons[i].sprite = Inventory.Instance.GetMainCurrency(i).icon;
                  }
              }
              */


            // If accepted set the colour of the panels

        }

        void ResetParams()
        {
            if (messageInput)
            {
                messageInput.text = "";
            }

            foreach (TMP_InputField tmpif in TMPPriceCurrencyInput)
            {
                if (tmpif != null)
                {
                    tmpif.text = "0";
                }
            }
       }



        public void OnEvent(AtavismEventData eData)
        {
            
            if (eData.eventType == "ITEM_ICON_UPDATE")
            {
                UpdateWindow();
            }
           
        }

        /// <summary>
        /// Updates the currency amount for the first "main" currency
        /// </summary>
        /// <param name="currencyAmount">Currency amount.</param>
        public void SetCurrency1(string currencyAmount)
        {
            if (currencyAmount == "")
                currencyAmount = "0";
        //    AtavismTrade.Instance.SetCurrencyAmount(Inventory.Instance.GetMainCurrency(0).id, int.Parse(currencyAmount));
        }

        /// <summary>
        /// Updates the currency amount for the first "main" currency
        /// </summary>
        /// <param name="currencyAmount">Currency amount.</param>
        public void SetCurrency2(string currencyAmount)
        {
            if (currencyAmount == "")
                currencyAmount = "0";
         //   AtavismTrade.Instance.SetCurrencyAmount(Inventory.Instance.GetMainCurrency(1).id, int.Parse(currencyAmount));
        }

        /// <summary>
        /// Updates the currency amount for the first "main" currency
        /// </summary>
        /// <param name="currencyAmount">Currency amount.</param>
        public void SetCurrency3(string currencyAmount)
        {
            if (currencyAmount == "")
                currencyAmount = "0";
          //  AtavismTrade.Instance.SetCurrencyAmount(Inventory.Instance.GetMainCurrency(2).id, int.Parse(currencyAmount));
        }

        bool checkCurrency()
        {

            List<Vector2> currencies = new List<Vector2>();
            foreach (string currencyID in AtavismTrade.Instance.MyCurrencyOffers.Keys)
            {
                currencies.Add(new Vector2(int.Parse(currencyID), AtavismTrade.Instance.MyCurrencyOffers[currencyID]));
            }

            if (Inventory.Instance.DoesPlayerHaveEnoughCurrency(currencies))
            {
                Debug.Log("Player does have enough currency");
                return true;
            }
            Debug.Log("Player does not have enough currency");
            return false;
        }

        public void StartShop()
        {
            // AtavismTrade.Instance.AcceptTrade();
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("message", messageInput.text);
            int c = 0;
          //  Debug.LogError("Shop StartShop sellItems=" + sellItems.Count + " buyMode=" + buyMode);
            foreach (ShopItem si in sellItems)
            {
                props.Add("sitemOid" + c, si.itemOID);
               // props.Add("itemId" + c, si.itemID);
               // props.Add("itemCount" + c, si.count);
                props.Add("sitemCurr" + c, si.purchaseCurrency);
                props.Add("sitemCost" + c, si.cost);
                c++;
            }
            props.Add("sellNum", c);
         //   Debug.LogError("Shop StartShop buyItems=" + buyItems.Count + " buyMode=" + buyMode);

            c = 0;
            foreach (ShopItem si in buyItems)
            {
                //props.Add("itemOid" + c, si.itemOID);
                props.Add("bitemId" + c, si.itemID);
                props.Add("bitemCount" + c, si.count);
                props.Add("bitemCurr" + c, si.purchaseCurrency);
                props.Add("bitemCost" + c, si.cost);
                c++;
            }
            props.Add("buyNum", c);

            if (sellItems.Count == 0 && buyItems.Count == 0)
            {
                string[] args = new string[1];
                args[0] = "Shop cant be empty";
                AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
                return;
            }


            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.START_SHOP", props);
            Hide();
            for (int i = 0; i < buyEntries.Count; i++)
            {
                buyEntries[i].updateDisplay(null);
            }
            for (int i = 0; i < sellEntries.Count; i++)
            {
                sellEntries[i].updateDisplay(null);
            }

        }

        public void CancelShop()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.CANCEL_SHOP", props);
            Hide();
        }

        public void DropItem(AtavismInventoryItem item, int pos)
        {
           // Debug.LogError("Shop drop "+item+ " buyMode="+ buyMode);
            selectPos = pos;
            if (item != null)
            {
               // Debug.LogError("Shop drop " + item.ItemId);
               // if (item.StackLimit > 1)
               // {
                    if (buyMode)
                    {
                        ShopItem mi = new ShopItem();
                        int currId = -1;
                        long currAmm = -1;
                        mi.cost = currAmm;
                        mi.purchaseCurrency = currId;
                        mi.itemID = item.templateId;
                        mi.item = item;
                        mi.count = item.Count;
                        mi.pos = selectPos;
                    // sellItems.Add(item.ItemId, mi);
                    selectItem = mi;
                        foreach (RectTransform rc in quantityObjects)
                        {
                            if (rc != null)
                                rc.gameObject.SetActive(true);
                        }
                    if (priceQuantity)
                    {
                        priceQuantity.text = item.StackLimit.ToString();
                    }
                    if (priceQuantitySlider)
                    {
                       // Debug.LogError("SHop Drop " + item.BaseName + " " + item.StackLimit);
                        priceQuantitySlider.maxValue = item.StackLimit;
                        priceQuantitySlider.value = item.StackLimit;
                    }
                    if (pricePanel)
                            pricePanel.gameObject.SetActive(true);
                        if (priceItemDisplay)
                        {
                            priceItemDisplay.SetItemData(item, null);
                        if (priceItemDisplay.TMPCountText != null)
                            priceItemDisplay.TMPCountText.text = "";
                        }
                        if (priceItemName)
                        {
                            priceItemName.text = item.BaseName;
                        }
                        //                        buyItems.Add(item.ItemId, mi);
                    }
                    else
                    {
                        ShopItem mi = new ShopItem();
                        int currId = -1;
                        long currAmm = -1;
                       // List<Vector2> c = new List<Vector2>();
                       // Inventory.Instance.ConvertCurrenciesToBaseCurrency(c, out currId, out currAmm);
                        mi.cost = currAmm;
                        mi.purchaseCurrency = currId;
                        mi.itemID = item.templateId;
                        mi.item = item;
                        mi.itemOID = item.ItemId;
                        mi.count = item.Count;
                        mi.pos = selectPos;
                       // sellItems.Add(item.ItemId, mi);
                        selectItem = mi;
                        foreach (RectTransform rc in quantityObjects)
                        {
                            if (rc != null)
                                rc.gameObject.SetActive(false);
                        }
                        if (pricePanel)
                            pricePanel.gameObject.SetActive(true);
                        if (priceItemDisplay)
                        {
                            priceItemDisplay.SetItemData(item, null);
                        }
                        if (priceItemName)
                        {
                            priceItemName.text = item.BaseName;
                        }
                    }
                checkPrice();
               // }
            }
        }

        public void checkPrice()
        {
            int curgrup = Inventory.Instance.mainCurrencyGroup;
            int countInput = 0;

            foreach (TMP_InputField tmpif in TMPPriceCurrencyInput)
            {
                if (tmpif.gameObject.activeSelf)
                {
                    // Debug.LogError("Value >" + tmpif.text + "<");
                    if (tmpif.text.Length == 0)
                        tmpif.text = "0";
                    if (Inventory.Instance.GetCurrenciesInGroup(curgrup).Count > countInput)
                    {
                        if (Inventory.Instance.GetCurrenciesInGroup(curgrup)[countInput].max < long.Parse(tmpif.text))
                        {
                            tmpif.text = Inventory.Instance.GetCurrenciesInGroup(curgrup)[countInput].max.ToString();
                        }
                        else if (long.Parse(tmpif.text) < 0)
                        {
                            tmpif.text = "0";
                        }
                        sellprice[Inventory.Instance.GetCurrenciesInGroup(curgrup)[countInput].id] = long.Parse(tmpif.text);
                        countInput++;
                    }
                }
            }
            if (priceQuantity)
            {
                if (priceQuantity.text.Length == 0)
                    priceQuantity.text = "1";
                int currencyType = -1;
                long currencyAmount = -1;
                Inventory.Instance.ConvertCurrenciesToBaseCurrency(sellprice, out currencyType, out currencyAmount);
                currencyAmount = currencyAmount * int.Parse(priceQuantity.text);
                List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(currencyType, currencyAmount);
                for (int i = 0; i < sellSumaryCurrencies.Count; i++)
                {
                    if (i < currencyDisplayList.Count)
                    {
                        sellSumaryCurrencies[i].gameObject.SetActive(true);
                        sellSumaryCurrencies[i].SetCurrencyDisplayData(currencyDisplayList[i]);
                    }
                    else
                    {
                        sellSumaryCurrencies[i].gameObject.SetActive(false);
                    }
                }
            }
        }


        public void changeQuantity()
        {
            if (priceQuantity && priceQuantitySlider)
            {
                int v = int.Parse(priceQuantity.text);
                if (v < 1)
                {
                    v = 1;
                    priceQuantity.text = v.ToString();
                }
                if (selectItem != null)
                {
                    if (v > selectItem.item.StackLimit)
                    {
                        v = selectItem.item.StackLimit;
                        priceQuantity.text = v.ToString();
                    }
                }

                priceQuantitySlider.value = int.Parse(priceQuantity.text);
            }
            checkPrice();
        }
        public void changeQuantitySlider()
        {
            if (priceQuantity && priceQuantitySlider)
                priceQuantity.text = priceQuantitySlider.value.ToString();
            checkPrice();
        }

        public void cancelPrice()
        {
            if (pricePanel != null)
                pricePanel.gameObject.SetActive(false);
            if (buyMode)
            {
                buyEntries[selectPos].updateDisplay(null);
                buyEntries[selectPos].ResetSlot();
            }
            else
            {
                sellEntries[selectPos].updateDisplay(null);
                sellEntries[selectPos].ResetSlot();
            }
            selectItem = null;
            // UpdateWindow();
        }
        public void PriceSave()
        {
            int curgrup = Inventory.Instance.mainCurrencyGroup;
            int countInput = 0;
            foreach (TMP_InputField tmpif in TMPPriceCurrencyInput)
            {
                if (tmpif.gameObject.activeSelf)
                {
                   // Debug.LogError("Value >" + tmpif.text + "<");
                    if (tmpif.text.Length == 0)
                        tmpif.text = "0";
                    if (Inventory.Instance.GetCurrenciesInGroup(curgrup).Count > countInput)
                    {
                        if (Inventory.Instance.GetCurrenciesInGroup(curgrup)[countInput].max < long.Parse(tmpif.text))
                        {
                            tmpif.text = Inventory.Instance.GetCurrenciesInGroup(curgrup)[countInput].max.ToString();
                        }
                        else if (long.Parse(tmpif.text) < 0)
                        {
                            tmpif.text = "0";
                        }
                        sellprice[Inventory.Instance.GetCurrenciesInGroup(curgrup)[countInput].id] = long.Parse(tmpif.text);
                        countInput++;
                    }
                }
            }
            if (buyMode)
            {
              //  Debug.LogError("selectItem >" + selectItem + " buyMode="+ buyMode);
                if (selectItem != null)
                {
                    int currId = -1;
                    long currAmm = -1;
                    Inventory.Instance.ConvertCurrenciesToBaseCurrency(sellprice, out currId, out currAmm);
                    if (currAmm == 0)
                    {
                        string[] args = new string[1];
#if AT_I2LOC_PRESET
                args[0] = I2.Loc.LocalizationManager.GetTranslation("Price can't be zero");
#else
                        args[0] = "Price can't be zero";
#endif
                        AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
                        
                        return;
                    }
                    selectItem.cost = currAmm;
                    selectItem.purchaseCurrency = currId;
                    if (priceQuantity)
                    {
                        if (priceQuantity)
                            if (priceQuantity.text.Length == 0)
                                priceQuantity.text = "1";
                        selectItem.count = int.Parse(priceQuantity.text);
                    }
                    buyEntries[selectPos].updateDisplay(selectItem);
                 /*   if (buyEntries[selectPos])
                        Debug.LogError("sellEntries[selectPos]");
                    if (buyEntries[selectPos].itemSlot)
                        Debug.LogError("sellEntries[selectPos].itemSlot");
                    if (buyEntries[selectPos].itemSlot.UguiActivatable)
                        Debug.LogError("sellEntries[selectPos].itemSlot.UguiActivatable");*/
                    if (buyEntries[selectPos].itemSlot.UguiActivatable.TMPCountText != null)
                        buyEntries[selectPos].itemSlot.UguiActivatable.TMPCountText.text = selectItem.count.ToString();
                    buyItems.Add(selectItem);
                  //  Debug.LogError("added  buyItems=" + buyItems.Count + " buyMode=" + buyMode);

                }
            }
            else
            {
                if (selectItem != null)
                {
                    int currId = -1;
                    long currAmm = -1;
                    Inventory.Instance.ConvertCurrenciesToBaseCurrency(sellprice, out currId, out currAmm);
                    if (currAmm == 0)
                    {
                        string[] args = new string[1];
#if AT_I2LOC_PRESET
                args[0] = I2.Loc.LocalizationManager.GetTranslation("Price can't be zero");
#else
                        args[0] = "Price can't be zero";
#endif
                        AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);

                        return;
                    }
                    selectItem.cost = currAmm;
                    selectItem.purchaseCurrency = currId;
                  //  if(!sellItems.ContainsKey(selectItem.itemOID))

                    sellItems.Add(selectItem);
                    sellEntries[selectPos].updateDisplay(selectItem);
                    if (sellEntries[selectPos].itemSlot.UguiActivatable.TMPCountText != null)
                        sellEntries[selectPos].itemSlot.UguiActivatable.TMPCountText.text = selectItem.item.Count.ToString();

                    //   Debug.LogError("added  sellItems=" + sellItems.Count + " buyMode=" + buyMode);

                }
            }
            if (pricePanel != null)
                pricePanel.gameObject.SetActive(false);
            selectItem = null;
            foreach (TMP_InputField tmpif in TMPPriceCurrencyInput)
            {
                if (tmpif != null)
                {
                    tmpif.text = "0";
                }
            }
            //  UpdateWindow();
        }

        Dictionary<int, long> sellprice = new Dictionary<int, long>();
        ShopItem selectItem = null;
        int selectPos=-1;
        public void ClearSlot(int pos, AtavismInventoryItem item)
        {
            if (item == null)
                return;
            //Debug.LogError("ClearSlot "+ buyItems.Count+" "+sellItems.Count);
            ShopItem select = null;
            if (buyMode)
            {
                foreach (ShopItem si in buyItems)
                {
                  //  Debug.LogError("ClearSlot buy pos=" + si.pos + " " + pos);
                    if (si.pos == pos)
                    {
                        select = si;
                        break;
                    }
                }
             //   Debug.LogError("ClearSlot BUY select=" + select);
                if (select != null)
                    buyItems.Remove(select);
                if (pos >= 0 && buyEntries.Count > pos)
                {
                   // buyEntries[pos].ResetSlot();
                    buyEntries[pos].updateDisplay(null);
                }
            }
            else
            {
                foreach (ShopItem si in sellItems)
                {
               //     Debug.LogError("ClearSlot sell pos=" + si.pos + " " + pos);
                    if (si.pos == pos)
                    {
                        select = si;
                        break;
                    }
                }
              //  Debug.LogError("ClearSlot SELL select=" + select);
                if (select != null)
                    sellItems.Remove(select);
                if (pos >= 0 && sellEntries.Count > pos)
                {
                   // sellEntries[pos].ResetSlot();
                    sellEntries[pos].updateDisplay(null);
                }
            }
           // Debug.LogError("ClearSlot End " + buyItems.Count + " " + sellItems.Count);

        }
    }
}