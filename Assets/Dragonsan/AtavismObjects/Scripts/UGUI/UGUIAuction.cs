using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Atavism
{
    [Serializable]
    public enum searchType
    {
        Type,
        SubType,
        Slot,
    }
    [Serializable]
    public class menuTree
    {
        //[SerializeField] int m_ID;
        public string name;
        public string value;
        public searchType type;
        public List<menuTree> submenu = new List<menuTree>();
    }

    public class UGUIAuction : MonoBehaviour
    {
        public GameObject AuctionPrefab;
        private bool showing = false;

        [AtavismSeparator("Auctions")]
        public GameObject buyPanel;
        public List<UGUIAuctionSlot> auctionsList = new List<UGUIAuctionSlot>();
        bool auctionlist = true;
        public Transform buySellgrid;
        bool sortCount = false;
        bool sortName = true;
        bool sortPrice = false;
        string searchRace = "Any";
        string searchClass = "Any";
        string searchText = "";
        string searchCat = "";
        string searchCatType = "";
        Dictionary<string, object> searchCatDic = new Dictionary<string, object>();
        public TMP_InputField searchInput;
        public TMP_InputField searchMinLevelInput;
        public TMP_InputField searchMaxLevelInput;
        public TMP_Dropdown searchClassDropdown;
        public TMP_Dropdown searchRaceDropdown;
        [Tooltip("List of Names Races Defined in Atavism. First must be Any")]
        [SerializeField] List<string> raceKeys = new List<string>();
        [Tooltip("List of Names Class Defined in Atavism. First must be Any")]
        [SerializeField] List<string> classKeys = new List<string>();
        public List<Toggle> qualityList;
        string qualitylevels = "";
        List<int> qualitylevelsList = new List<int>();

        int minLevel = 1;
        int maxLevel = 100;
        public Button itemCountButton;
        public Button itemNameButton;
        public Button itemPriceButton;
        public bool showPriceSellItem = false;
        public GameObject searchPanel;
        public GameObject searchPanelMenu;
        public RectTransform buySellPanelToResize;
        [Tooltip("X: Left; Y: Right; Z:Top; W:Bottom")]
        public Quaternion buyParamSize = new Quaternion();
        [Tooltip("X: Left; Y: Right; Z:Top; W:Bottom")]
        public Quaternion sellParamSize = new Quaternion();
        public string countSortText = "Count";
        public string nameSortText = "Name";
        public string priceSortText = "Price";
        public TextMeshProUGUI countSortButtonText;
        public TextMeshProUGUI nameSortButtonText;
        public TextMeshProUGUI priceSortButtonText;
        bool sortAsc = true;
        public TextMeshProUGUI errorText;

        [AtavismSeparator("Inventory")]
        public GameObject inventoryPanel;
        public List<UGUIAuctionSlot> inventoryList = new List<UGUIAuctionSlot>();
        public Transform inventoryGrid;
        //bool inventorylist = true;
        bool showSell = false;

        [AtavismSeparator("Sell/Buy")]
        public UGUIInventoryAuctionSlot slotPrefab;
        // public GridLayoutGroup inventoryGrid;
        //public GameObject invantoryPanel;
        //  public List<UGUIInventoryAuctionSlot> slots = new List<UGUIInventoryAuctionSlot>();
        //  public List<UGUIAuctionSlot> itemsList = new List<UGUIAuctionSlot>();
        public GameObject sellPanel;
        public UGUIItemDisplay sellslot;
        public TextMeshProUGUI sellItemName;
        public List<TMP_InputField> sellCurrencyInput;
        public List<Image> sellCurrencyInputImage;
        public List<UGUICurrency> sellSumaryCurrencies;
        public TMP_InputField sellItemsCount;
        public Slider sellItemsCountSlider;
        public TextMeshProUGUI totalCount;
        public TextMeshProUGUI confirmButtonText;
        public List<UGUICurrency> commissionSumaryCurrencies;
        public TextMeshProUGUI commissionText;

        public AuctionCountSlot auctionCountPrefab;
        public Transform sellCountListGrid;
        public Transform buyCountListGrid;
        public List<AuctionCountSlot> sellCountList = new List<AuctionCountSlot>();
        public List<AuctionCountSlot> buyCountList = new List<AuctionCountSlot>();
        Dictionary<string, long> sellprice = new Dictionary<string, long>();
        public TextMeshProUGUI confirmBottunText;
        public string sellButtonText = "Sell Instant";
        public string listSellButtonText = "List Sell";
        public string buyButtonText = "Buy Instant";
        public string orderButtonText = "Place Order";
        bool sellInstant = false;
        bool buyInstant = false;
        int totalAuctionCountItems = 100;
        [AtavismSeparator("Transactions")]
        public GameObject transactionsPanel;
        public List<UGUICurrency> seledCurrencies;
        public Transform transactionsGrid;
        public List<UGUIAuctionSlot> ownAuctionsList = new List<UGUIAuctionSlot>();
        bool buying = true; bool selling = false; bool bought = false; bool sold = false; bool expired = false;
        bool showTransactions = false;
        public TextMeshProUGUI buyingButton;
        public TextMeshProUGUI sellingButton;
        public TextMeshProUGUI boughtButton;
        public TextMeshProUGUI soldButton;
        public TextMeshProUGUI expiredButton;



        [AtavismSeparator("Menu Settings")]
        public Color defaultTopMenuColor = Color.white;
        public Color selectedTopMenuColor = Color.green;
        public Color defaultTreeMenuColor = Color.white;
        public Color selectedTeeMenuColor = Color.green;
        public Color defaultSortColor = Color.white;
        public Color selectedSortMenuColor = Color.green;
        public bool disableTopMenuImage = true;
        public Button buyMenuButton;
        public TextMeshProUGUI buyMenuButtonText;
        public Button sellMenuButton;
        public TextMeshProUGUI sellMenuButtonText;
        public Button transactionsMenuButton;
        public TextMeshProUGUI transactionsMenuButtonText;
        [SerializeField] GameObject menuPrefab;
        public List<menuTree> menuFilter = new List<menuTree>();
        [SerializeField] Transform menuGrid;
        [SerializeField] List<UGUIMenuSlot> menuObjects = new List<UGUIMenuSlot>();
        public Dictionary<int, menuTree> testowy = new Dictionary<int, menuTree>();
        int menuObjectNum = 0;
       
        /// <summary>
        /// 
        /// </summary>
        public void Toggle()
        {
            if (showing)
                Hide();
            else
                Show();
        }
        /// <summary>
        /// 
        /// </summary>
        void Show()
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
            AtavismUIUtility.BringToFront(this.gameObject);
            AtavismAuction.Instance.GetAuctionList();
            showing = true;
            ShowAuctionList();
        }
        /// <summary>
        /// 
        /// </summary>
        void OnlyShow()
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
            AtavismUIUtility.BringToFront(this.gameObject);
            showing = true;
            ShowAuctionList();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            GetComponent<CanvasGroup>().interactable = false;

            showing = false;
            AtavismAuction.Instance.Auctions.Clear();
            AtavismAuction.Instance.OwnAuctions.Clear();
            AtavismAuction.Instance.AuctionsForGroupOrder.Clear();
            AtavismAuction.Instance.AuctionsForGroupSell.Clear();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eData"></param>
        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "AUCTION_OPEN")
            {
                OnlyShow();
            }
            else if (eData.eventType == "AUCTION_LIST_UPDATE")
                {
                    //  Debug.LogError("OnEvent AUCTION_LIST_UPDATE");
                    if (auctionlist)
                {
                    foreach (UGUIAuctionSlot uguias in auctionsList)
                    {
                        if (uguias != null)
                            uguias.gameObject.SetActive(false);
                    }
                    Dictionary<string, Auction> auctions = AtavismAuction.Instance.Auctions;
                    int i = 1;
                    //   Debug.LogError(auctions.Keys);
                    foreach (string aucid in auctions.Keys)
                    {
                        //      Debug.LogError("i:" + i + " aucid:" + aucid + " auctions count:" + auctions.Count);
                        if (i > auctionsList.Count)
                        {
                            GameObject go = (GameObject)Instantiate(AuctionPrefab, buySellgrid);
                            auctionsList.Add(go.GetComponent<UGUIAuctionSlot>());
                        }
                        if (!auctionsList[i - 1].gameObject.activeSelf)
                            auctionsList[i - 1].gameObject.SetActive(true);
                        auctionsList[i - 1].SetDetale(selectAuction, auctions[aucid]);
                        i++;
                    }
                    if (i >= AtavismAuction.Instance.AuctionsLimit)
                    {
                        if (errorText != null)
#if AT_I2LOC_PRESET
                        errorText.text = I2.Loc.LocalizationManager.GetTranslation("List of displayed auctions has been limited, please use filters to narrow your search");
#else
                            errorText.text = "List of displayed auctions has been limited, please use filters to narrow your search";
#endif
                    }
                    else
                    {
                        errorText.text = "";
                    }

                }

                //  Debug.LogError("OnEvent AUCTION_LIST_UPDATE End");


            }
            else if (eData.eventType == "INVENTORY_UPDATE")
            {
                if (showSell)
                    ShowSellList();
            }
            else if (eData.eventType == "AUCTION_LIST_FOR_GROUP_UPDATE")
            {
                //  Debug.LogError("OnEvent AUCTION_LIST_FOR_GROUP_UPDATE");

                //   ShowAuctionList();
                foreach (AuctionCountSlot uguias in sellCountList)
                {
                    if (uguias != null)
                        uguias.gameObject.SetActive(false);
                }
                foreach (AuctionCountSlot uguias in buyCountList)
                {
                    if (uguias != null)
                        uguias.gameObject.SetActive(false);
                }
                Dictionary<long, AuctionCountPrice> auctionsForGroupOrder = AtavismAuction.Instance.AuctionsForGroupOrder;
                Dictionary<long, AuctionCountPrice> auctionsForGroupSell = AtavismAuction.Instance.AuctionsForGroupSell;
                int i = 1;
                //   Debug.LogError(auctions.Keys);
                foreach (long aucid in auctionsForGroupOrder.Keys)
                {
                    //    Debug.LogError(aucid + " " + auctionsForGroupOrder.Count);
                    if (i > buyCountList.Count)
                    {
                        GameObject go = (GameObject)Instantiate(auctionCountPrefab.gameObject, buyCountListGrid);
                        buyCountList.Add(go.GetComponent<AuctionCountSlot>());
                    }
                    if (!buyCountList[i - 1].gameObject.activeSelf)
                        buyCountList[i - 1].gameObject.SetActive(true);
                    buyCountList[i - 1].SetDetale(auctionsForGroupOrder[aucid]);
                    if (i == 1)
                    {
                        if (showSell)
                        {
                            List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(auctionsForGroupOrder[aucid].currency, auctionsForGroupOrder[aucid].price,true);
                            //      Debug.LogError("currencyDisplayList " + currencyDisplayList.Count);
                            for (int ii = 0; ii < sellCurrencyInput.Count; ii++)
                            {
                                if (ii < currencyDisplayList.Count)
                                {
                                    sellCurrencyInput[sellCurrencyInput.Count - ii - 1].gameObject.SetActive(true);
                                    sellCurrencyInput[sellCurrencyInput.Count - ii - 1].text = currencyDisplayList[currencyDisplayList.Count - ii - 1].amount.ToString();
                                    sellCurrencyInputImage[sellCurrencyInput.Count - ii - 1].sprite = currencyDisplayList[currencyDisplayList.Count - ii - 1].icon;
                                    sellCurrencyInputImage[sellCurrencyInput.Count - ii - 1].enabled = true;
                                }
                                else
                                {
                                    sellCurrencyInput[sellCurrencyInput.Count - ii - 1].gameObject.SetActive(false);
                                    sellCurrencyInput[sellCurrencyInput.Count - ii - 1].text = "0";
                                    sellCurrencyInputImage[sellCurrencyInput.Count - ii - 1].enabled = false;

                                }
                            }
                            changeSellPrice();
                        }
                    }
                    i++;
                }
                int j = 1;
                foreach (long aucid in auctionsForGroupSell.Keys)
                {
                   // Debug.LogError(aucid + " auctionsForGroupSell=" + auctionsForGroupSell.Count+" j="+j+ " sellCountList="+ sellCountList.Count);
                    while (j > sellCountList.Count)
                    {

                        GameObject go = (GameObject)Instantiate(auctionCountPrefab.gameObject, sellCountListGrid);
                        sellCountList.Add(go.GetComponent<AuctionCountSlot>());
                    }
                    if (!sellCountList[j - 1].gameObject.activeSelf)
                        sellCountList[j - 1].gameObject.SetActive(true);
                //    Debug.LogError(aucid + " | auctionsForGroupSell=" + auctionsForGroupSell.Count + " j=" + j + " sellCountList=" + sellCountList.Count);

                    sellCountList[j - 1].SetDetale(auctionsForGroupSell[aucid]);
                    if (j == 1)
                    {
                        if (auctionlist)
                        {
                            List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(auctionsForGroupSell[aucid].currency, auctionsForGroupSell[aucid].price,true);
                            //     Debug.LogError("currencyDisplayList " + currencyDisplayList.Count);
                            for (int ii = 0; ii < sellCurrencyInput.Count; ii++)
                            {
                                if (ii < currencyDisplayList.Count)
                                {
                                    //  sellCurrencyInput[i].gameObject.SetActive(true);
                                    sellCurrencyInput[sellCurrencyInput.Count - ii - 1].gameObject.SetActive(true);
                                    sellCurrencyInput[sellCurrencyInput.Count - ii - 1].text = currencyDisplayList[currencyDisplayList.Count - ii - 1].amount.ToString();
                                    sellCurrencyInputImage[sellCurrencyInput.Count - ii - 1].sprite = currencyDisplayList[currencyDisplayList.Count - ii - 1].icon;
                                    sellCurrencyInputImage[sellCurrencyInput.Count - ii - 1].enabled = true;
                                }
                                else
                                {
                                    sellCurrencyInput[sellCurrencyInput.Count - ii - 1].gameObject.SetActive(false);
                                    sellCurrencyInput[sellCurrencyInput.Count - ii - 1].text = "0";
                                    sellCurrencyInputImage[sellCurrencyInput.Count - ii - 1].enabled = false;
                                }

                            }
                        }
                        changeSellPrice();

                    }
                    j++;
                }
                totalAuctionCountItems = 0;
                foreach (long aucid in auctionsForGroupSell.Keys)
                {
                    totalAuctionCountItems += auctionsForGroupSell[aucid].count;
                }
                changeSellPrice();
                //    Debug.LogError("AUCTION_LIST_FOR_GROUP_UPDATE end");

            }
            else if (eData.eventType == "AUCTION_OWN_LIST_UPDATE")
            {
                //   Debug.LogError("OnEvent AUCTION_OWN_LIST_UPDATE");

                foreach (UGUIAuctionSlot uguias in ownAuctionsList)
                {
                    if (uguias != null)
                        uguias.gameObject.SetActive(false);
                }

                Dictionary<int, Auction> auctionsForGroupOrder = AtavismAuction.Instance.OwnAuctions;
                int i = 1;
                //    Debug.LogError("auctionsForGroupOrder "+auctionsForGroupOrder.Count);
                foreach (int aucid in auctionsForGroupOrder.Keys)
                {
                    //      Debug.LogError(aucid + " " + auctionsForGroupOrder.Count);
                    if (i > ownAuctionsList.Count)
                    {
                        GameObject go = (GameObject)Instantiate(AuctionPrefab, transactionsGrid);
                        ownAuctionsList.Add(go.GetComponent<UGUIAuctionSlot>());
                    }
                    if (!ownAuctionsList[i - 1].gameObject.activeSelf)
                        ownAuctionsList[i - 1].gameObject.SetActive(true);
                    if (selling || buying)
                    {
                        ownAuctionsList[i - 1].SetDetale(cancelAuction, auctionsForGroupOrder[aucid]);
                    }
                    else
                    {
                        ownAuctionsList[i - 1].SetDetale(null, auctionsForGroupOrder[aucid]);
                    }

                    i++;
                }
                //      Debug.LogError("AUCTION_OWN_LIST_UPDATE end");
            }
        }

        private void cancelAuction(Auction auction)
        {
#if AT_I2LOC_PRESET
        UGUIConfirmationPanel.Instance.ShowConfirmationBox(I2.Loc.LocalizationManager.GetTranslation("Do you really want to cancel the auctions") + " " + I2.Loc.LocalizationManager.GetTranslation("Items/"+auction.item.BaseName) + " ?", auction, cancelAuction);
#else
            UGUIConfirmationPanel.Instance.ShowConfirmationBox("Do you really want to cancel the auctions " + auction.item.BaseName + "?", auction, cancelAuction);
#endif
        }

        void cancelAuction(object auction, bool accepted)
        {
            if (accepted)
                AtavismAuction.Instance.CancelAuction((Auction)auction, selling, buying);

        }


        /// <summary>
        /// 
        /// </summary>
        public void ShowAuctionList()
        {
            auctionlist = true;
            showSell = false;
            showTransactions = false;
            // string qualitylevels = "";
            qualitylevels = "";
            int k = 0;
            qualitylevelsList.Clear();
            foreach (Toggle t in qualityList)
            {
                k++;
                if (t != null)
                {
                    if (t.isOn)
                    {
                        qualitylevels += k;
                        qualitylevelsList.Add(k);
                    }
                }
                if (k < qualityList.Count)
                    qualitylevels += ";";
            }
            List<object> list = new List<object>();
            foreach (int elm in qualitylevelsList)
                list.Add(elm);
            AtavismAuction.Instance.SearchAuction(sortCount, sortName, sortPrice, qualitylevels, searchRace, searchClass, minLevel, maxLevel, searchCatType, searchCat, searchText, list, sortAsc, searchCatDic);

            if (buyMenuButton != null)

            {
                Color c = buyMenuButton.targetGraphic.color;
                c.a = 1f;
                buyMenuButton.targetGraphic.color = c;
            }
            if (sellMenuButton != null)
            {
                Color c = sellMenuButton.targetGraphic.color;
                c.a = 0f;
                sellMenuButton.targetGraphic.color = c;
            }
            if (transactionsMenuButton != null)
            {
                Color c = transactionsMenuButton.targetGraphic.color;
                c.a = 0f;
                transactionsMenuButton.targetGraphic.color = c;
            }
            if (buyMenuButtonText != null)
                buyMenuButtonText.color = selectedTopMenuColor;
            if (sellMenuButtonText != null)
                sellMenuButtonText.color = defaultTopMenuColor;
            if (transactionsMenuButtonText != null)
                transactionsMenuButtonText.color = defaultTopMenuColor;

            if (!buyPanel.activeSelf)
                buyPanel.SetActive(true);
            if (!searchPanel.activeSelf)
                searchPanel.SetActive(true);
            if (searchPanelMenu != null)
                if (!searchPanelMenu.activeSelf)
                    searchPanelMenu.SetActive(true);
            if (inventoryPanel.activeSelf)
                inventoryPanel.SetActive(false);
            if (sellPanel.activeSelf)
                sellPanel.SetActive(false);



            itemCountButton.interactable = true;
            itemNameButton.interactable = true;
            itemPriceButton.interactable = true;
            priceSortButtonText.enabled = true;

            if (transactionsPanel.activeSelf)
                transactionsPanel.SetActive(false);
            foreach (UGUIAuctionSlot uguias in auctionsList)
            {
                if (uguias != null)
                    uguias.gameObject.SetActive(false);
            }
            Dictionary<string, Auction> auctions = AtavismAuction.Instance.Auctions;
            int i = 1;
            //    Debug.LogError(auctions.Keys.Count);
            foreach (string aucid in auctions.Keys)
            {
                //    Debug.LogError(aucid + " " + auctions.Count);
                if (i > auctionsList.Count)
                {
                    GameObject go = (GameObject)Instantiate(AuctionPrefab, buySellgrid);
                    auctionsList.Add(go.GetComponent<UGUIAuctionSlot>());
                }
                if (!auctionsList[i - 1].gameObject.activeSelf)
                    auctionsList[i - 1].gameObject.SetActive(true);
                auctionsList[i - 1].SetDetale(selectAuction, auctions[aucid]);
                i++;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void ShowSellList()
        {
            auctionlist = false;
            showSell = true;
            showTransactions = false;
            if (buyMenuButton != null)
            {
                Color c = buyMenuButton.targetGraphic.color;
                c.a = 0f;
                buyMenuButton.targetGraphic.color = c;
            }
            if (sellMenuButton != null)
            {
                Color c = sellMenuButton.targetGraphic.color;
                c.a = 1f;
                sellMenuButton.targetGraphic.color = c;
            }
            if (transactionsMenuButton != null)
            {
                Color c = transactionsMenuButton.targetGraphic.color;
                c.a = 0f;
                transactionsMenuButton.targetGraphic.color = c;
            }
            if (buyMenuButtonText != null)
                buyMenuButtonText.color = defaultTopMenuColor;
            if (sellMenuButtonText != null)
                sellMenuButtonText.color = selectedTopMenuColor;
            if (transactionsMenuButtonText != null)
                transactionsMenuButtonText.color = defaultTopMenuColor;
            if (sellPanel.activeSelf)
                sellPanel.SetActive(false);

            if (buyPanel.activeSelf)
                buyPanel.SetActive(false);
            if (searchPanel.activeSelf)
                searchPanel.SetActive(false);
            if (searchPanelMenu != null)
                if (searchPanelMenu.activeSelf)
                    searchPanelMenu.SetActive(false);
            itemCountButton.interactable = false;
            itemNameButton.interactable = false;
            itemPriceButton.interactable = false;
            priceSortButtonText.text = priceSortText;
            countSortButtonText.text = countSortText;
            nameSortButtonText.text = nameSortText;
            if (!showPriceSellItem)
                priceSortButtonText.enabled = false;
            if (transactionsPanel.activeSelf)
                transactionsPanel.SetActive(false);
            if (!inventoryPanel.activeSelf)
                inventoryPanel.SetActive(true);




            foreach (UGUIAuctionSlot uguias in inventoryList)
            {
                if (uguias != null)
                    uguias.gameObject.SetActive(false);
            }
            Dictionary<int, Bag> bags = Inventory.Instance.Bags;
            // Dictionary<int, AtavismInventoryItem> aii = new Dictionary<int, AtavismInventoryItem>();

            int it = 0;
            List<string> itemGroups = new List<string>();
            for (int i = 0; i < bags.Count; i++)
            {
                for (int k = 0; k < bags[i].numSlots; k++)
                {
                    if (bags[i].items.ContainsKey(k))
                    {
                        if (Inventory.Instance.GetCurrencyGroup(bags[i].items[k].CurrencyType) == Inventory.Instance.GetCurrencyGroup(AtavismAuction.Instance.GetCurrencyType))
                        if (!bags[i].items[k].isBound && bags[i].items[k].auctionHouse)
                        {

                            String itemGroup = bags[i].items[k].templateId.ToString();
                            if (bags[i].items[k].enchantLeval > 0)
                                itemGroup += "_E" + bags[i].items[k].enchantLeval;
                            if (bags[i].items[k].SocketSlotsOid.Count > 0)
                            {
                                List<long> socketItems = new List<long>();
                                //       HashMap<Integer, SocketInfo> itemSockets = (HashMap<Integer, SocketInfo>)Item.getProperty("sockets");
                                //    ArrayList<Long> socketItems = new ArrayList<Long>();
                                foreach (String sType in bags[i].items[k].SocketSlotsOid.Keys)
                                {
                                    foreach (int sId in bags[i].items[k].SocketSlotsOid[sType].Keys)
                                    {
                                        //  if (itemSockets.get(sId).GetItemOid() != null)
                                        //   {
                                        socketItems.Add(bags[i].items[k].SocketSlotsOid[sType][sId]);
                                        //     }
                                    }
                                }
                                socketItems.Sort();


                                //  Collections.sort(socketItems);
                                foreach (long l in socketItems)
                                {
                                    itemGroup += "_S" + l;
                                }
                                /*   ItemsCount.put(ite, Item.getStackSize());
                                   if (groupedItems.containsKey(unicItem))
                                   {
                                       groupedItems.get(unicItem).add(ite);
                                   }
                                   else
                                   {
                                       ArrayList<OID> list = new ArrayList<OID>();
                                       list.add(ite);
                                       groupedItems.put(unicItem, list);
                                   }
                                   */
                            }


                            if (!itemGroups.Contains(itemGroup) || bags[i].items[k].StackLimit == 1)
                            {
                                itemGroups.Add(itemGroup);
                                if (inventoryList.Count <= it)
                                {
                                    GameObject go = (GameObject)Instantiate(AuctionPrefab, inventoryGrid);
                                    inventoryList.Add(go.GetComponent<UGUIAuctionSlot>());
                                }
                                if (!inventoryList[it].gameObject.activeSelf)
                                    inventoryList[it].gameObject.SetActive(true);
                                Auction auction = new Auction();
                                if (bags[i].items[k].StackLimit > 1)
                                    auction.count = Inventory.Instance.GetCountOfItem(bags[i].items[k].templateId);
                                auction.item = bags[i].items[k];
                                     int currencyId = 0;
                                    long currencyAmount = 0;
                                    Inventory.Instance.ConvertCurrencyToBaseCurrency(bags[i].items[k].currencyType, bags[i].items[k].cost, out  currencyId,out currencyAmount);
                                    //   auction.currency = bags[i].items[k].currencyType;
                                    auction.buyout = currencyAmount;
                                    auction.currency = currencyId;
                                    // AtavismAuction.Instance.GetCurrencyType;

                                if (bags[i].items[k].StackLimit > 1)
                                    auction.groupId = itemGroup;
                                inventoryList[it].SetDetale(selectItem, auction, true);
                                //    Debug.LogWarning("Auction item for sell bn:" + bags[i].items[k].BaseName + " el:" + bags[i].items[k].enchantLeval+" c:"+ bags[i].items[k].Count);
                                it++;
                            }
                        }

                    }

                }

            }

        }





        /// <summary>
        /// 
        /// </summary>
        public void ShowTransactions()
        {
            auctionlist = false;
            showSell = false;
            showTransactions = true;
            if (buyMenuButton != null)
            {
                Color c = buyMenuButton.targetGraphic.color;
                c.a = 0f;
                buyMenuButton.targetGraphic.color = c;
            }
            if (sellMenuButton != null)
            {
                Color c = sellMenuButton.targetGraphic.color;
                c.a = 0f;
                sellMenuButton.targetGraphic.color = c;
            }
            if (transactionsMenuButton != null)
            {
                Color c = transactionsMenuButton.targetGraphic.color;
                c.a = 1f;
                transactionsMenuButton.targetGraphic.color = c;
            }
            if (buyMenuButtonText != null)
                buyMenuButtonText.color = defaultTopMenuColor;
            if (sellMenuButtonText != null)
                sellMenuButtonText.color = defaultTopMenuColor;
            if (transactionsMenuButtonText != null)
                transactionsMenuButtonText.color = selectedTopMenuColor;
            if (buyPanel.activeSelf)
                buyPanel.SetActive(false);
            if (!transactionsPanel.activeSelf)
                transactionsPanel.SetActive(true);
            if (inventoryPanel.activeSelf)
                inventoryPanel.SetActive(false);
            ShowBuying();
        }
        /// <summary>
        /// 
        /// </summary>

        public void SortCount()
        {
            sortCount = true;
            sortName = false;
            sortPrice = false;
            sortAsc = !sortAsc;
            if (countSortButtonText != null)
            {
                if (sortAsc)
#if AT_I2LOC_PRESET
            countSortButtonText.text = I2.Loc.LocalizationManager.GetTranslation(countSortText)+ "<sprite=0>";
#else
                    countSortButtonText.text = countSortText + "<sprite=0>";
#endif
                else
#if AT_I2LOC_PRESET
            countSortButtonText.text = I2.Loc.LocalizationManager.GetTranslation(countSortText)+ "<sprite=1>";
#else
                    countSortButtonText.text = countSortText + "<sprite=1>";
#endif
            }
            if (priceSortButtonText != null)
#if AT_I2LOC_PRESET
            priceSortButtonText.text = I2.Loc.LocalizationManager.GetTranslation(priceSortText);
#else
                priceSortButtonText.text = priceSortText;
#endif
            if (nameSortButtonText != null)
#if AT_I2LOC_PRESET
            nameSortButtonText.text = I2.Loc.LocalizationManager.GetTranslation(nameSortText);
#else
                nameSortButtonText.text = nameSortText;
#endif
            if (auctionlist)
            {
                List<object> list = new List<object>();
                foreach (int elm in qualitylevelsList)
                    list.Add(elm);
                AtavismAuction.Instance.SearchAuction(sortCount, sortName, sortPrice, qualitylevels, searchRace, searchClass, minLevel, maxLevel, searchCatType, searchCat, searchText, list, sortAsc, searchCatDic);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        public void SortItemName()
        {
            sortName = true;
            sortCount = false;
            sortPrice = false;
            sortAsc = !sortAsc;
            if (nameSortButtonText != null)
            {
                if (sortAsc)
#if AT_I2LOC_PRESET
            nameSortButtonText.text = I2.Loc.LocalizationManager.GetTranslation(nameSortText)+ "<sprite=0>";
#else
                    nameSortButtonText.text = nameSortText + "<sprite=0>";
#endif
                else
#if AT_I2LOC_PRESET
            nameSortButtonText.text = I2.Loc.LocalizationManager.GetTranslation(nameSortText)+ "<sprite=1>";
#else
                    nameSortButtonText.text = nameSortText + "<sprite=1>";
#endif
            }
            if (priceSortButtonText != null)
#if AT_I2LOC_PRESET
            priceSortButtonText.text = I2.Loc.LocalizationManager.GetTranslation(priceSortText);
#else
                priceSortButtonText.text = priceSortText;
#endif
            if (countSortButtonText != null)
#if AT_I2LOC_PRESET
            countSortButtonText.text = I2.Loc.LocalizationManager.GetTranslation(countSortText);
#else
                countSortButtonText.text = countSortText;
#endif
            if (auctionlist)
            {
                List<object> list = new List<object>();
                foreach (int elm in qualitylevelsList)
                    list.Add(elm);
                AtavismAuction.Instance.SearchAuction(sortCount, sortName, sortPrice, qualitylevels, searchRace, searchClass, minLevel, maxLevel, searchCatType, searchCat, searchText, list, sortAsc, searchCatDic);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void SortPrice()
        {
            sortCount = false;
            sortName = false;
            sortPrice = true;
            sortAsc = !sortAsc;
            if (priceSortButtonText != null)
            {
                if (sortAsc)
#if AT_I2LOC_PRESET
            priceSortButtonText.text = I2.Loc.LocalizationManager.GetTranslation(priceSortText)+ "<sprite=0>";
#else
                    priceSortButtonText.text = priceSortText + "<sprite=0>";
#endif
                else
#if AT_I2LOC_PRESET
            priceSortButtonText.text = I2.Loc.LocalizationManager.GetTranslation(priceSortText)+ "<sprite=1>";
#else
                    priceSortButtonText.text = priceSortText + "<sprite=1>";
#endif
            }
            if (countSortButtonText != null)
#if AT_I2LOC_PRESET
            countSortButtonText.text = I2.Loc.LocalizationManager.GetTranslation(countSortText);
#else
                countSortButtonText.text = countSortText;
#endif
            if (nameSortButtonText != null)
#if AT_I2LOC_PRESET
            nameSortButtonText.text = I2.Loc.LocalizationManager.GetTranslation(nameSortText);
#else
                nameSortButtonText.text = nameSortText;
#endif

            if (auctionlist)
            {
                List<object> list = new List<object>();
                foreach (int elm in qualitylevelsList)
                    list.Add(elm);
                AtavismAuction.Instance.SearchAuction(sortCount, sortName, sortPrice, qualitylevels, searchRace, searchClass, minLevel, maxLevel, searchCatType, searchCat, searchText, list, sortAsc, searchCatDic);

            }
            else if (showSell)
            {

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void ShowBuying()
        {
            buying = true;
            selling = false;
            bought = false;
            sold = false;
            expired = false;

            AtavismAuction.Instance.GetOwnAuctionList(buying, selling, bought, sold, expired);
            buyingButton.color = selectedTeeMenuColor;
            sellingButton.color = defaultTreeMenuColor;
            boughtButton.color = defaultTreeMenuColor;
            soldButton.color = defaultTreeMenuColor;
            expiredButton.color = defaultTreeMenuColor;


        }
        /// <summary>
        /// 
        /// </summary>
        public void ShowSelling()
        {
            buying = false;
            selling = true;
            bought = false;
            sold = false;
            expired = false;

            AtavismAuction.Instance.GetOwnAuctionList(buying, selling, bought, sold, expired);
            buyingButton.color = defaultTreeMenuColor;
            sellingButton.color = selectedTeeMenuColor;
            boughtButton.color = defaultTreeMenuColor;
            soldButton.color = defaultTreeMenuColor;
            expiredButton.color = defaultTreeMenuColor;

        }
        /// <summary>
        /// 
        /// </summary>
        public void ShowBought()
        {
            buying = false;
            selling = false;
            bought = true;
            sold = false;
            expired = false;

            AtavismAuction.Instance.GetOwnAuctionList(buying, selling, bought, sold, expired);
            buyingButton.color = defaultTreeMenuColor;
            sellingButton.color = defaultTreeMenuColor;
            boughtButton.color = selectedTeeMenuColor;
            soldButton.color = defaultTreeMenuColor;
            expiredButton.color = defaultTreeMenuColor;

        }
        /// <summary>
        /// 
        /// </summary>
        public void ShowSold()
        {
            buying = false;
            selling = false;
            bought = false;
            sold = true;
            expired = false;

            AtavismAuction.Instance.GetOwnAuctionList(buying, selling, bought, sold, expired);
            buyingButton.color = defaultTreeMenuColor;
            sellingButton.color = defaultTreeMenuColor;
            boughtButton.color = defaultTreeMenuColor;
            soldButton.color = selectedTeeMenuColor;
            expiredButton.color = defaultTreeMenuColor;
        }
        /// <summary>
        /// 
        /// </summary>
        public void ShowExpired()
        {
            buying = false;
            selling = false;
            bought = false;
            sold = false;
            expired = true;

            AtavismAuction.Instance.GetOwnAuctionList(buying, selling, bought, sold, expired);
            buyingButton.color = defaultTreeMenuColor;
            sellingButton.color = defaultTreeMenuColor;
            boughtButton.color = defaultTreeMenuColor;
            soldButton.color = defaultTreeMenuColor;
            expiredButton.color = selectedTeeMenuColor;

        }

        /// <summary>
        /// 
        /// </summary>
        public void searchAuction()
        {
            searchText = searchInput.text;
            List<object> list = new List<object>();
            foreach (int elm in qualitylevelsList)
                list.Add(elm);
            AtavismAuction.Instance.SearchAuction(sortCount, sortName, sortPrice, qualitylevels, searchRace, searchClass, minLevel, maxLevel, searchCatType, searchCat, searchText, list, sortAsc, searchCatDic);
        }

        /// <summary>
        /// 
        /// </summary>
        public void TakeAll()
        {
            AtavismAuction.Instance.TakeReward(buying, selling, bought, sold, expired);
        }

        /// <summary>
        ///  
        /// </summary>
        public void searchClassChange()
        {
            // Debug.LogError("searchClassChange " + searchClassDropdown.value + " " + searchClassDropdown.options[searchClassDropdown.value].text);
            searchClass = classKeys[searchClassDropdown.value];
            List<object> list = new List<object>();
            foreach (int elm in qualitylevelsList)
                list.Add(elm);
            AtavismAuction.Instance.SearchAuction(sortCount, sortName, sortPrice, qualitylevels, searchRace, searchClass, minLevel, maxLevel, searchCatType, searchCat, searchText, list, sortAsc, searchCatDic);
        }

        /// <summary>
        /// 
        /// </summary>
        public void searchRaceChange()
        {
            // Debug.LogError("searchClassChange " + searchRaceDropdown.value + " " + searchRaceDropdown.options[searchRaceDropdown.value].text);
            searchRace = raceKeys[searchRaceDropdown.value];
            List<object> list = new List<object>();
            foreach (int elm in qualitylevelsList)
                list.Add(elm);
            AtavismAuction.Instance.SearchAuction(sortCount, sortName, sortPrice, qualitylevels, searchRace, searchClass, minLevel, maxLevel, searchCatType, searchCat, searchText, list, sortAsc, searchCatDic);
        }

        /// <summary>
        /// 
        /// </summary>
        public void searchQuality()
        {
            qualitylevels = "";
            int k = 0;
            qualitylevelsList.Clear();
            foreach (Toggle t in qualityList)
            {
                k++;
                if (t != null)
                {
                    if (t.isOn)
                    {
                        qualitylevels += k;
                        qualitylevelsList.Add(k);
                    }
                }
                if (k < qualityList.Count)
                    qualitylevels += ";";
            }
            List<object> list = new List<object>();
            foreach (int elm in qualitylevelsList)
                list.Add(elm);
            AtavismAuction.Instance.SearchAuction(sortCount, sortName, sortPrice, qualitylevels, searchRace, searchClass, minLevel, maxLevel, searchCatType, searchCat, searchText, list, sortAsc, searchCatDic);
        }

        /// <summary>
        /// 
        /// </summary>
        public void searchMinLevel()
        {
            if (searchMinLevelInput.text == "" || searchMinLevelInput.text == " ")
                searchMinLevelInput.text = "1";
            minLevel = int.Parse(searchMinLevelInput.text);
            if (maxLevel < minLevel)
                searchMinLevelInput.text = maxLevel.ToString();
            List<object> list = new List<object>();
            foreach (int elm in qualitylevelsList)
                list.Add(elm);
            AtavismAuction.Instance.SearchAuction(sortCount, sortName, sortPrice, qualitylevels, searchRace, searchClass, minLevel, maxLevel, searchCatType, searchCat, searchText, list, sortAsc, searchCatDic);
        }

        /// <summary>
        /// 
        /// </summary>
        public void searchMaxLevel()
        {
            if (searchMaxLevelInput.text == "" || searchMaxLevelInput.text == " ")
                searchMaxLevelInput.text = "999";
            maxLevel = int.Parse(searchMaxLevelInput.text);
            if (maxLevel < minLevel)
                searchMaxLevelInput.text = minLevel.ToString();
            List<object> list = new List<object>();
            foreach (int elm in qualitylevelsList)
                list.Add(elm);
            AtavismAuction.Instance.SearchAuction(sortCount, sortName, sortPrice, qualitylevels, searchRace, searchClass, minLevel, maxLevel, searchCatType, searchCat, searchText, list, sortAsc, searchCatDic);
        }

        /// <summary>
        /// 
        /// </summary>
        public void closeSell()
        {
            if (sellPanel != null)
                sellPanel.SetActive(false);
            itemtosell = null;
            auctionGroupId = "";
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClickSell()
        {
            Dictionary<string, object> currencies = new Dictionary<string, object>();
            foreach (string currencyID in sellprice.Keys)
            {
                currencies.Add(currencyID, sellprice[currencyID]);
            }
            if (showSell)
            {
                AtavismAuction.Instance.CreateAuction(itemtosell, currencies, (int)sellItemsCountSlider.value, auctionGroupId);
            }
            else
            {
                if (buyInstant)
                    AtavismAuction.Instance.BuyAuction(auctionGroupId, currencies, (int)sellItemsCountSlider.value);
                else
                    AtavismAuction.Instance.OrderAuction(auctionGroupId, currencies, (int)sellItemsCountSlider.value);
            }
            closeSell();
        }

        /// <summary>
        /// 
        /// </summary>
        public void CancelSell()
        {
            closeSell();
            itemtosell = null;
            auctionGroupId = "";
        }

        /// <summary>
        /// Updates the currency amount for the first "main" currency
        /// </summary>
        /// <param name="currencyAmount">Currency amount.</param>
        public void SetCurrency1(string currencyAmount)
        {
            if (currencyAmount == "" || currencyAmount == " " || string.IsNullOrEmpty(currencyAmount)) //currencyAmount = "0";
                sellCurrencyInput[0].text = "0";
            //    sellprice[Inventory.Instance.GetMainCurrency(0).id.ToString()] = int.Parse(currencyAmount);
            changeSellPrice();
        }

        /// <summary>
        /// Updates the currency amount for the first "main" currency
        /// </summary>
        /// <param name="currencyAmount">Currency amount.</param>
        public void SetCurrency2(string currencyAmount)
        {
            if (currencyAmount == "" || currencyAmount == " " || string.IsNullOrEmpty(currencyAmount))
                sellCurrencyInput[1].text = "0";
            //   sellprice[Inventory.Instance.GetMainCurrency(1).id.ToString()] = int.Parse(currencyAmount);
            changeSellPrice();
        }

        /// <summary>
        /// Updates the currency amount for the first "main" currency
        /// </summary>
        /// <param name="currencyAmount">Currency amount.</param>
        public void SetCurrency3(string currencyAmount)
        {
            if (currencyAmount == "" || currencyAmount == " " || string.IsNullOrEmpty(currencyAmount))
                sellCurrencyInput[2].text = "0";
            //     sellprice[Inventory.Instance.GetMainCurrency(2).id.ToString()] = int.Parse(currencyAmount);
            changeSellPrice();
        }

        /// <summary>
        ///  function search parent currency and return it
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lc"></param>
        /// <returns></returns>
        Currency GetChildCurrency(int id, List<Currency> lc)
        {
            foreach (Currency c in lc)
            {
                if (c.convertsTo.Equals(id))
                    return c;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        void changeSellPrice()
        {
            if (sellCurrencyInput.Count > 0 && sellCurrencyInput[0].text == "" || sellCurrencyInput[0].text == " " || string.IsNullOrEmpty(sellCurrencyInput[0].text))
                sellCurrencyInput[0].text = "0";
            if (sellCurrencyInput.Count > 1 &&sellCurrencyInput[1].text == "" || sellCurrencyInput[1].text == " " || string.IsNullOrEmpty(sellCurrencyInput[1].text))
                sellCurrencyInput[1].text = "0";
            if (sellCurrencyInput.Count > 2 && sellCurrencyInput[2].text == "" || sellCurrencyInput[2].text == " " || string.IsNullOrEmpty(sellCurrencyInput[2].text))
                sellCurrencyInput[2].text = "0";
            if (sellCurrencyInput.Count > 2 && sellCurrencyInput[0].text == "0" && sellCurrencyInput[1].text == "0" && sellCurrencyInput[2].text == "0")
                sellCurrencyInput[sellCurrencyInput.Count-1].text = "1";


            int curgrup = Inventory.Instance.GetCurrencyGroup(AtavismAuction.Instance.GetCurrencyType);
          //  Inventory.Instance.GetCurrenciesInGroup(curgrup);
         //   sellprice[Inventory.Instance.GetCurrenciesInGroup(curgrup)[0].id.ToString()] = int.Parse(sellCurrencyInput[0].text);
          //  Debug.LogError(Inventory.Instance.GetCurrenciesInGroup(curgrup)[0].id.ToString() + " " + sellCurrencyInput[0].text);
            int countInput = 0;
            foreach (TMP_InputField tmpif in sellCurrencyInput)
            {
                if (tmpif.gameObject.activeSelf)
                {
                    if (Inventory.Instance.GetCurrenciesInGroup(curgrup).Count > countInput)
                    {
                        if (Inventory.Instance.GetCurrenciesInGroup(curgrup)[countInput].max < long.Parse(tmpif.text))
                        {
                            tmpif.text = Inventory.Instance.GetCurrenciesInGroup(curgrup)[countInput].max.ToString();
                        }
                        else if (long.Parse(tmpif.text)<0)
                        {
                            tmpif.text = "0";
                        }
                        sellprice[Inventory.Instance.GetCurrenciesInGroup(curgrup)[countInput].id.ToString()] = long.Parse(tmpif.text);
                        countInput++;
                    }
                }
            }
           
         /*   if (Inventory.Instance.GetCurrenciesInGroup(curgrup).Count > 1)
            {
                Debug.LogError("1 -"+Inventory.Instance.GetCurrenciesInGroup(curgrup)[1].id.ToString() + " " + sellCurrencyInput[1].text);
                                sellprice[Inventory.Instance.GetCurrenciesInGroup(curgrup)[1].id.ToString()] = int.Parse(sellCurrencyInput[1].text);
            }
            if (Inventory.Instance.GetCurrenciesInGroup(curgrup).Count > 2)
            {
                Debug.LogError("2 -"+Inventory.Instance.GetCurrenciesInGroup(curgrup)[2].id.ToString() + " " + sellCurrencyInput[2].text);

                sellprice[Inventory.Instance.GetCurrenciesInGroup(curgrup)[2].id.ToString()] = int.Parse(sellCurrencyInput[2].text);
            }
            */
            int currencyType = -1;
            long currencyAmount = 0;
            foreach (string currencyID in sellprice.Keys)
            {
                Currency c = Inventory.Instance.GetCurrency(int.Parse(currencyID));
                long amount = sellprice[currencyID];
            //    Debug.LogError("amount=" + amount + " currencyID=" +currencyID);
                int cid = c.id;
                List<Currency> lc = Inventory.Instance.GetCurrenciesInGroup(c.group);
                Currency cur = c;
                while (cur != null)
                {
                    cur = GetChildCurrency(cid, lc);
                    if (cur != null)
                    {
                        amount *= cur.conversionAmountReq;
                        cid = cur.id;
                    }
                }
                currencyAmount += amount;
                currencyType = cid;
            }
         //   Debug.LogError("currencyAmount=" + currencyAmount+" "+ sellprice.Keys.Count);
            if (String.IsNullOrEmpty(sellItemsCount.text) || sellItemsCount.text == "0")
                sellItemsCount.text = "1";
            int number = int.Parse(sellItemsCount.text);
            //   Debug.LogError("changeSellPrice: currencyAmount:" + currencyAmount + " number:" + number);
            Dictionary<long, AuctionCountPrice> auctionsForGroupOrder = AtavismAuction.Instance.AuctionsForGroupOrder;
            Dictionary<long, AuctionCountPrice> auctionsForGroupSell = AtavismAuction.Instance.AuctionsForGroupSell;
            sellInstant = false;
            buyInstant = false;
            long pricetotal = 0;
            if (showSell)
            {
                int ii = 1;
                bool setPrice = false;
                foreach (long aucid in auctionsForGroupOrder.Keys)
                {
                    if (auctionsForGroupOrder[aucid].price == currencyAmount)
                    {
                        if (auctionsForGroupOrder[aucid].count > number)
                        {
                            if (buyCountList.Count >= ii)
                            {
                                buyCountList[ii - 1].setPartial();
                                //    pricetotal += (auctionsForGroupOrder[aucid].count - number) * auctionsForGroupOrder[aucid].price;
                            }
                            else
                            {
                                //     Debug.LogError("changeSellPrice: buyCountList.Count:" + buyCountList.Count + " ii-1:" + (ii - 1));
                                //     numberslected += auctionsForGroupOrder[aucid].count;
                            }
                        }
                        else if (auctionsForGroupOrder[aucid].count == number)
                        {
                            if (buyCountList.Count >= ii)
                            {
                                //      pricetotal += auctionsForGroupOrder[aucid].count * auctionsForGroupOrder[aucid].price;
                                buyCountList[ii - 1].setFull();
                            }
                            else
                            {
                                //   Debug.LogError("changeSellPrice: buyCountList.Count:" + buyCountList.Count + " ii-1:" + (ii - 1));
                                //  numberslected += auctionsForGroupOrder[aucid].count;
                            }
                        }
                        else if (auctionsForGroupOrder[aucid].count < number)
                        {
                            if (buyCountList.Count >= ii)
                            {
                                //     pricetotal += auctionsForGroupOrder[aucid].count * auctionsForGroupOrder[aucid].price;
                                buyCountList[ii - 1].setFull();
                            }
                            else
                            {
                                //    Debug.LogError("changeSellPrice: buyCountList.Count:" + buyCountList.Count + " ii-1:" + (ii - 1));
                            }
                            sellItemsCount.text = auctionsForGroupOrder[aucid].count.ToString();
                            sellItemsCountSlider.maxValue = auctionsForGroupOrder[aucid].count;
                            sellItemsCountSlider.value = auctionsForGroupOrder[aucid].count;
                            totalCount.text = sellItemsCount.text + "/" + auctionsForGroupOrder[aucid].count;
                        }
                        totalCount.text = sellItemsCount.text + "/" + auctionsForGroupOrder[aucid].count;
                        sellInstant = true;
                    }
                    else
                    {
                        if (!setPrice && auctionsForGroupOrder[aucid].price > currencyAmount)
                        {
                            setPrice = true;

                          //  int curId = 0;
                         //   long curAmount = 0;
                          //  Inventory.Instance.ConvertCurrencyToBaseCurrency(itemtosell.currencyType, auctionsForGroupOrder[aucid].price,out curId,out curAmount);
                            currencyAmount = auctionsForGroupOrder[aucid].price;



                            List<CurrencyDisplay> currencyDisplayList2 = Inventory.Instance.GenerateCurrencyListFromAmount(itemtosell.currencyType, auctionsForGroupOrder[aucid].price,true);
                            for (int ij = 0; ij < sellCurrencyInput.Count; ij++)
                            {
                                if (ij < currencyDisplayList2.Count)
                                {
                                    sellCurrencyInput[sellCurrencyInput.Count - ij - 1].gameObject.SetActive(true);
                                    sellCurrencyInput[sellCurrencyInput.Count - ij - 1].text = currencyDisplayList2[currencyDisplayList2.Count - ij - 1].amount.ToString();
                                    sellCurrencyInputImage[sellCurrencyInput.Count - ij - 1].sprite = currencyDisplayList2[currencyDisplayList2.Count - ij - 1].icon;
                                    sellCurrencyInputImage[sellCurrencyInput.Count - ij - 1].enabled = true;
                                }
                                else
                                {
                                    //    Debug.LogError("currency i:" + ij + " sellCurrencyInput.Count:" + sellCurrencyInput.Count);
                                    sellCurrencyInput[sellCurrencyInput.Count - ij - 1].gameObject.SetActive(false);
                                    sellCurrencyInput[sellCurrencyInput.Count - ij - 1].text = "0";
                                    sellCurrencyInputImage[sellCurrencyInput.Count - ij - 1].enabled = false;
                                }
                            }
                            changeSellPrice();
                        }
                        if (buyCountList.Count >= ii)
                        {
                            buyCountList[ii - 1].Reset();
                        }
                        else
                        {
                            //    Debug.LogError("changeSellPrice: buyCountList.Count:" + buyCountList.Count + " ii-1:" + (ii - 1));
                        }
                    }
                    ii++;
                }
                ii = 1;
                int numberslected = 0;
                // if (!sellInstant)
                foreach (long aucid in auctionsForGroupSell.Keys)
                {
                    if (auctionsForGroupSell[aucid].price == currencyAmount)
                    {
                        if (auctionsForGroupSell[aucid].count > number - numberslected)
                        {
                            if (sellCountList.Count >= ii)
                            {
                                sellCountList[ii - 1].setPartial();
                            }
                            else
                            {
                                //    Debug.LogError("changeSellPrice: buyCountList.Count:" + sellCountList.Count + " ii-1:" + (ii - 1));
                            }
                            numberslected += auctionsForGroupSell[aucid].count;
                        }
                        else if (auctionsForGroupSell[aucid].count <= number - numberslected)
                        {
                            if (sellCountList.Count >= ii)
                            {
                                sellCountList[ii - 1].setFull();
                            }
                            else
                            {
                                //     Debug.LogError("changeSellPrice: buyCountList.Count:" + sellCountList.Count + " ii-1:" + (ii - 1));
                            }
                            numberslected += auctionsForGroupSell[aucid].count;
                        }
                        else if (number - numberslected <= 0)
                        {
                            if (sellCountList.Count >= ii)
                            {
                                sellCountList[ii - 1].Reset();
                            }
                            else
                            {
                                // Debug.LogError("changeSellPrice: buyCountList.Count:" + sellCountList.Count + " ii-1:" + (ii - 1));
                            }
                        }

                    }
                    else
                    {
                        if (sellCountList.Count >= ii)
                        {
                            sellCountList[ii - 1].Reset();
                        }
                        else
                        {
                            //       Debug.LogError("changeSellPrice: buyCountList.Count:" + sellCountList.Count + " ii-1:" + (ii - 1));
                        }
                    }
                    ii++;
                }

                if (!sellInstant)
                {
                    int value = Inventory.Instance.GetCountOfItem(itemtosell.templateId);
                    if (value > itemtosell.StackLimit)
                        value = itemtosell.StackLimit;
                    if (value < int.Parse(sellItemsCount.text))
                        sellItemsCount.text = itemtosell.StackLimit.ToString();
                    sellItemsCountSlider.maxValue = value;
                    totalCount.text = sellItemsCount.text + "/" + value;
                }

                if (setPrice)
                    changeSellPrice();
            }
            //    Debug.LogError("Total ptzrd auction list" + pricetotal);

            if (auctionlist)
            {
                // bool isin = false;
                int ii = 1;
                int numberslected = 0;
                bool setPrice = false;
                if(auctionsForGroupSell!=null && auctionsForGroupSell.Count>0)
                foreach (long aucid in auctionsForGroupSell.Keys)
                {
                    if (auctionsForGroupSell[aucid].price <= currencyAmount)
                    {
                        if (auctionsForGroupSell[aucid].count > number - numberslected && number - numberslected > 0)
                        {
                            if (sellCountList.Count >= ii)
                            {
                                sellCountList[ii - 1].setPartial();
                                //    Debug.LogError("changeSellPrice: auctionsForGroupSell.Count:" + auctionsForGroupSell[aucid].count + " number:" + number+ " numberslected:"+ numberslected+" price:"+ auctionsForGroupSell[aucid].price);
                                //      Debug.LogError("Total " + pricetotal+" add "+ (( (number - numberslected)) * auctionsForGroupSell[aucid].price));
                                pricetotal += ((number - numberslected)) * auctionsForGroupSell[aucid].price;
                            }
                            else
                            {
                                //  Debug.LogError("changeSellPrice: buyCountList.Count:" + sellCountList.Count + " ii-1:" + (ii - 1));
                            }
                            numberslected += auctionsForGroupSell[aucid].count;
                        }
                        else if (auctionsForGroupSell[aucid].count <= number - numberslected)
                        {
                            if (sellCountList.Count >= ii)
                            {
                                sellCountList[ii - 1].setFull();
                                //        Debug.LogError("changeSellPrice: auctionsForGroupSell.Count:" + auctionsForGroupSell[aucid].count + " number:" + number + " numberslected:" + numberslected + " price:" + auctionsForGroupSell[aucid].price);
                                //        Debug.LogError("Total " + pricetotal+" add:"+((auctionsForGroupSell[aucid].count) * auctionsForGroupSell[aucid].price));
                                pricetotal += (auctionsForGroupSell[aucid].count) * auctionsForGroupSell[aucid].price;
                            }
                            else
                            {
                                //   Debug.LogError("changeSellPrice: buyCountList.Count:" + sellCountList.Count + " ii-1:" + (ii - 1));
                            }
                            numberslected += auctionsForGroupSell[aucid].count;
                        }
                        else if (number - numberslected <= 0)
                        {
                            if (sellCountList.Count >= ii)
                            {
                                sellCountList[ii - 1].Reset();
                                //  pricetotal += (auctionsForGroupOrder[aucid].count - number) * auctionsForGroupOrder[aucid].price;

                            }
                            else
                            {
                                //       Debug.LogError("changeSellPrice: buyCountList.Count:" + sellCountList.Count + " ii-1:" + (ii - 1));
                            }
                        }
                        buyInstant = true;

                        //Debug.LogError("changeSellPrice: currencyAmount:" + currencyAmount + " number:" + number+ " aucid:"+ aucid+" price:"+ auctionsForGroupSell[aucid].price+" count: "+ auctionsForGroupSell[aucid].count+ " numberslected:"+ numberslected);

                    }
                    else
                    {
                        //    Debug.LogError("changeSellPrice: currencyAmount:" + currencyAmount + " number:" + number + " aucid:" + aucid + " price:" + auctionsForGroupSell[aucid].price + " count: " + auctionsForGroupSell[aucid].count + " numberslected:" + numberslected+ " setPrice:"+ setPrice);
                        if (!setPrice && buyInstant && number - numberslected > 0)
                        {
                            setPrice = true;
                            List<CurrencyDisplay> currencyDisplayList2 = Inventory.Instance.GenerateCurrencyListFromAmount(itemtosell.currencyType, auctionsForGroupSell[aucid].price,true);
                            for (int ij = 0; ij < sellCurrencyInput.Count; ij++)
                            {
                                if (ij < currencyDisplayList2.Count)
                                {
                                    //  sellCurrencyInput[i].gameObject.SetActive(true);
                                    sellCurrencyInput[sellCurrencyInput.Count - ij - 1].gameObject.SetActive(true);
                                    sellCurrencyInput[sellCurrencyInput.Count - ij - 1].text = currencyDisplayList2[currencyDisplayList2.Count - ij - 1].amount.ToString();
                                    sellCurrencyInputImage[sellCurrencyInput.Count - ij - 1].sprite = currencyDisplayList2[currencyDisplayList2.Count - ij - 1].icon;
                                    sellCurrencyInputImage[sellCurrencyInput.Count - ij - 1].enabled = true;
                                }
                                else
                                {
                                    //   Debug.LogError("currency i:" + ij + " sellCurrencyInput.Count:" + sellCurrencyInput.Count);
                                    sellCurrencyInput[sellCurrencyInput.Count - ij - 1].gameObject.SetActive(false);
                                    sellCurrencyInput[sellCurrencyInput.Count - ij - 1].text = "0";
                                    sellCurrencyInputImage[sellCurrencyInput.Count - ij - 1].enabled = false;
                                }
                            }

                        }

                        if (sellCountList.Count >= ii)
                        {
                            sellCountList[ii - 1].Reset();
                        }
                        else
                        {
                            //       Debug.LogError("changeSellPrice: buyCountList.Count:" + sellCountList.Count + " ii-1:" + (ii - 1));
                        }
                    }
                    ii++;
                }
                ii = 1;
                //     if (!buyInstant)
                if (auctionsForGroupOrder != null && auctionsForGroupOrder.Count > 0)
                    foreach (long aucid in auctionsForGroupOrder.Keys)
                {
                    if (auctionsForGroupOrder[aucid].price == currencyAmount)
                    {
                        if (auctionsForGroupOrder[aucid].count > number - numberslected)
                        {
                            if (buyCountList.Count >= ii)
                            {
                                buyCountList[ii - 1].setPartial();
                            }
                            else
                            {
                                //    Debug.LogError("changeSellPrice: buyCountList.Count:" + buyCountList.Count + " ii-1:" + (ii - 1));
                            }
                            numberslected += auctionsForGroupOrder[aucid].count;
                        }
                        else if (auctionsForGroupOrder[aucid].count <= number - numberslected)
                        {
                            if (buyCountList.Count >= ii)
                            {
                                buyCountList[ii - 1].setFull();
                            }
                            else
                            {
                                //   Debug.LogError("changeSellPrice: buyCountList.Count:" + buyCountList.Count + " ii-1:" + (ii - 1));
                            }
                            numberslected += auctionsForGroupOrder[aucid].count;
                        }
                        else if (number - numberslected <= 0)
                        {
                            if (buyCountList.Count >= ii)
                            {
                                buyCountList[ii - 1].Reset();
                            }
                            else
                            {
                                //       Debug.LogError("changeSellPrice: buyCountList.Count:" + buyCountList.Count + " ii-1:" + (ii - 1));
                            }
                        }
                    }
                    else
                    {
                        if (buyCountList.Count >= ii)
                        {
                            buyCountList[ii - 1].Reset();
                        }
                        else
                        {
                            //       Debug.LogError("changeSellPrice: buyCountList.Count:" + buyCountList.Count + " ii-1:" + (ii - 1));
                        }
                    }
                    ii++;
                }
            }

            //   currencyAmount *= int.Parse(sellItemsCount.text);
            // Debug.LogError("Total " + pricetotal);
            if (auctionlist)
            {
                if (pricetotal > 0)
                {
                    currencyAmount = pricetotal;
                }
                else
                {
                    currencyAmount *= int.Parse(sellItemsCount.text);
                }

                //    if (auctionlist)
                //     {
                if (selectedAuction.countSell > 0 && totalAuctionCountItems < itemtosell.StackLimit && buyInstant)
                {
                    sellItemsCountSlider.maxValue = totalAuctionCountItems;
                    totalCount.text = sellItemsCount.text + "/" + totalAuctionCountItems;
                }
                else
                {
                    sellItemsCountSlider.maxValue = itemtosell.StackLimit;
                    totalCount.text = sellItemsCount.text + "/" + itemtosell.StackLimit;
                }
                //   }

            }
            else
            {
                currencyAmount *= int.Parse(sellItemsCount.text);
            }
            long costSell = 0;
            if (showSell)
            {
                if (!commissionText.gameObject.activeSelf)
                    commissionText.gameObject.SetActive(true);

              /*  int basecost = 0;
                int basecostCur = AtavismAuction.Instance.GetCurrencyType;
                Inventory.Instance.ConvertCurrencyToBaseCurrency(itemtosell.cost, itemtosell.currencyType,out basecost,out basecostCur);
                */
                costSell = AtavismAuction.Instance.CalcCost(currencyAmount);
             //   Debug.LogError("Auction  ca=" + currencyAmount + " cost=" + costSell);
                List<CurrencyDisplay> currencyDisplayList1 = Inventory.Instance.GenerateCurrencyListFromAmount(AtavismAuction.Instance.GetCurrencyType/* itemtosell.currencyType*/, costSell);

                for (int i = 0; i < commissionSumaryCurrencies.Count; i++)
                {
                    if (i < currencyDisplayList1.Count)
                    {
                        commissionSumaryCurrencies[i].gameObject.SetActive(true);
                        commissionSumaryCurrencies[i].SetCurrencyDisplayData(currencyDisplayList1[i]);
                    }
                    else
                    {
                        commissionSumaryCurrencies[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                if (commissionText.gameObject.activeSelf)
                    commissionText.gameObject.SetActive(false);
                for (int i = 0; i < commissionSumaryCurrencies.Count; i++)
                {
                    commissionSumaryCurrencies[i].gameObject.SetActive(false);
                }
            }
            //  Debug.LogError(" showSell:" + showSell + " sellInstant:" + sellInstant + " auctionlist:" + auctionlist + " buyInstant:" + buyInstant+ " currencyAmount:"+ currencyAmount);
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


            if (showSell)
            {
                if (sellInstant)
                {
#if AT_I2LOC_PRESET
           if (confirmButtonText != null) confirmButtonText.text = I2.Loc.LocalizationManager.GetTranslation(sellButtonText);
#else
                    if (confirmButtonText != null)
                        confirmButtonText.text = sellButtonText;
#endif
                }
                else
                {
#if AT_I2LOC_PRESET
           if (confirmButtonText != null) confirmButtonText.text = I2.Loc.LocalizationManager.GetTranslation(listSellButtonText);
#else
                    if (confirmButtonText != null)
                        confirmButtonText.text = listSellButtonText;
#endif
                }
            }
            if (auctionlist)
            {
                if (buyInstant)
                {
#if AT_I2LOC_PRESET
           if (confirmButtonText != null) confirmButtonText.text = I2.Loc.LocalizationManager.GetTranslation(buyButtonText);
#else
                    if (confirmButtonText != null)
                        confirmButtonText.text = buyButtonText;
#endif
                }
                else
                {
#if AT_I2LOC_PRESET
           if (confirmButtonText != null) confirmButtonText.text = I2.Loc.LocalizationManager.GetTranslation(orderButtonText);
#else
                    if (confirmButtonText != null)
                        confirmButtonText.text = orderButtonText;
#endif
                }
            }



        }

        AtavismInventoryItem itemtosell;
        Auction selectedAuction;
        string auctionGroupId = "";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="auction"></param>
        private void selectItem(Auction auction)
        {
            //  Debug.LogError("selectItem id" + auction.item.ItemId);
            itemtosell = auction.item;
            selectedAuction = auction;
            if (auction.groupId.Length > 0)
                auctionGroupId = auction.groupId;
#if AT_I2LOC_PRESET
           if (confirmButtonText != null) confirmButtonText.text = I2.Loc.LocalizationManager.GetTranslation(sellButtonText);
#else
            if (confirmButtonText != null)
                confirmButtonText.text = sellButtonText;
#endif
            if (sellPanel != null)
            {
                sellPanel.SetActive(true);
                sellPanel.transform.position = new Vector3((Screen.width / 2), (Screen.height / 2), 0);
                AtavismUIUtility.BringToFront(sellPanel);

            }
            foreach (AuctionCountSlot uguias in sellCountList)
            {
                if (uguias != null)
                    uguias.gameObject.SetActive(false);
            }
            foreach (AuctionCountSlot uguias in buyCountList)
            {
                if (uguias != null)
                    uguias.gameObject.SetActive(false);
            }
            if (sellItemName != null)
            {
                if (itemtosell.enchantLeval > 0)
#if AT_I2LOC_PRESET
                 sellItemName.text = "+" + itemtosell.enchantLeval + " " + I2.Loc.LocalizationManager.GetTranslation("Items/" + itemtosell.BaseName);
#else
                    sellItemName.text = "+" + itemtosell.enchantLeval + " " + itemtosell.BaseName;
#endif
                else
#if AT_I2LOC_PRESET
                 sellItemName.text = I2.Loc.LocalizationManager.GetTranslation("Items/" + itemtosell.BaseName);
#else
                    sellItemName.text = itemtosell.BaseName;
#endif
            }
            int value = Inventory.Instance.GetCountOfItem(itemtosell.templateId);
            //int stack = itemtosell.StackLimit;
            //  Debug.LogError(" Item Sell val:" + value + " ss:" + itemtosell.StackLimit + " st:" + itemtosell.Count + " cost:" + itemtosell.cost + " cur:" + itemtosell.currencyType);
            if (value > itemtosell.StackLimit)
                value = itemtosell.StackLimit;
            // if (value<)


            sellItemsCount.text = value.ToString();
            sellItemsCountSlider.maxValue = value;
            sellItemsCountSlider.value = value;
            totalCount.text = value + "/" + value;
            sellslot.SetItemData(itemtosell, null);
            if (!commissionText.gameObject.activeSelf)
                commissionText.gameObject.SetActive(true);

            long basecost = 0;
            int basecostCur = AtavismAuction.Instance.GetCurrencyType;
            Inventory.Instance.ConvertCurrencyToBaseCurrency(itemtosell.currencyType, itemtosell.cost, out basecostCur ,out basecost);

            long costSell = AtavismAuction.Instance.CalcCost(basecost * value);
            //   Debug.LogError("CalcCost: c:"+ itemtosell.cost+" count:"+ value+" sum:"+ (itemtosell.cost * value)+" costSell:"+ costSell);


            List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(AtavismAuction.Instance.GetCurrencyType/*basecostCur*/, costSell,true);
            for (int i = 0; i < commissionSumaryCurrencies.Count; i++)
            {
                if (i < currencyDisplayList.Count)
                {
                    commissionSumaryCurrencies[i].gameObject.SetActive(true);
                    commissionSumaryCurrencies[i].SetCurrencyDisplayData(currencyDisplayList[i]);
                }
                else
                {
                    commissionSumaryCurrencies[i].gameObject.SetActive(false);
                }
            }
            currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(AtavismAuction.Instance.GetCurrencyType/*itemtosell.currencyType*/, basecost, true);
            //  Debug.LogError("currencyDisplayList " + currencyDisplayList.Count);
            for (int i = 0; i < sellCurrencyInput.Count; i++)
            {
                if (i < currencyDisplayList.Count)
                {
                    //  sellCurrencyInput[i].gameObject.SetActive(true);
                    sellCurrencyInput[sellCurrencyInput.Count - i - 1].gameObject.SetActive(true);
                    sellCurrencyInput[sellCurrencyInput.Count - i - 1].text = currencyDisplayList[currencyDisplayList.Count - i - 1].amount.ToString();
                    sellCurrencyInputImage[sellCurrencyInput.Count - i - 1].sprite = currencyDisplayList[currencyDisplayList.Count - i - 1].icon;
                    sellCurrencyInputImage[sellCurrencyInput.Count - i - 1].enabled = true;
                }
                else
                {
                    sellCurrencyInput[sellCurrencyInput.Count - i - 1].gameObject.SetActive(false);
                    sellCurrencyInput[sellCurrencyInput.Count - i - 1].text = "0";
                    sellCurrencyInputImage[sellCurrencyInput.Count - i - 1].enabled = false;
                }

            }

            changeSellPrice();



            if (auctionlist)
                AtavismAuction.Instance.GetAuctionsForGroup(auctionGroupId, 0L);
            else
                AtavismAuction.Instance.GetAuctionsForGroup("", itemtosell.ItemId.ToLong());

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="auction"></param>
        private void selectAuction(Auction auction)
        {
            auctionGroupId = auction.groupId;
            selectedAuction = auction;
            //  Debug.LogError("selectItem id" + auction.item.ItemId);
            itemtosell = auction.item;
#if AT_I2LOC_PRESET
           if (confirmButtonText != null) confirmButtonText.text = I2.Loc.LocalizationManager.GetTranslation(buyButtonText);
#else
            if (confirmButtonText != null)
                confirmButtonText.text = buyButtonText;
#endif
            if (sellPanel != null)
            {

                sellPanel.SetActive(true);
                sellPanel.transform.position = new Vector3((Screen.width / 2), (Screen.height / 2), 0);
                AtavismUIUtility.BringToFront(sellPanel);

            }
            foreach (AuctionCountSlot uguias in sellCountList)
            {
                if (uguias != null)
                    uguias.gameObject.SetActive(false);
            }
            foreach (AuctionCountSlot uguias in buyCountList)
            {
                if (uguias != null)
                    uguias.gameObject.SetActive(false);
            }
            if (sellItemName != null)
            {
                if (itemtosell.enchantLeval > 0)
#if AT_I2LOC_PRESET
                sellItemName.text = "+" + itemtosell.enchantLeval + " " + I2.Loc.LocalizationManager.GetTranslation("Items/" + itemtosell.BaseName);
#else
                    sellItemName.text = "+" + itemtosell.enchantLeval + " " + itemtosell.BaseName;
#endif
                else
#if AT_I2LOC_PRESET
                 sellItemName.text = I2.Loc.LocalizationManager.GetTranslation("Items/" + itemtosell.BaseName);
#else
                    sellItemName.text = itemtosell.BaseName;
#endif
            }
            int value = 1;
            //int stack = itemtosell.StackLimit;
            //   Debug.LogError(" Item Sell val:" + itemtosell.Count + " ss:" + itemtosell.StackLimit + " st:" + itemtosell.Count+ " cost:"+itemtosell.cost+" cur:"+ itemtosell.currencyType);
            if (value > itemtosell.StackLimit)
                value = itemtosell.StackLimit;
            // if (value<)

            List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(itemtosell.currencyType, itemtosell.cost,true);
            for (int i = 0; i < sellCurrencyInput.Count; i++)
            {
                if (i < currencyDisplayList.Count)
                {
                    //  sellCurrencyInput[i].gameObject.SetActive(true);
                    sellCurrencyInput[sellCurrencyInput.Count - i - 1].gameObject.SetActive(true);
                    sellCurrencyInput[sellCurrencyInput.Count - i - 1].text = currencyDisplayList[currencyDisplayList.Count - i - 1].amount.ToString();
                    sellCurrencyInputImage[sellCurrencyInput.Count - i - 1].sprite = currencyDisplayList[currencyDisplayList.Count - i - 1].icon;
                    sellCurrencyInputImage[sellCurrencyInput.Count - i - 1].enabled = true;
                }
                else
                {
                    //      Debug.LogError("currency i:" + i + " sellCurrencyInput.Count:" + sellCurrencyInput.Count);
                    sellCurrencyInput[sellCurrencyInput.Count - i - 1].gameObject.SetActive(false);
                    sellCurrencyInput[sellCurrencyInput.Count - i - 1].text = "0";
                    sellCurrencyInputImage[sellCurrencyInput.Count - i - 1].enabled = false;
                }
            }

            sellItemsCount.text = value.ToString();
            // Debug.LogWarning("selectedAuction.countSell:" + selectedAuction.countSell + " itemtosell.StackLimit:" + itemtosell.StackLimit);
            if (selectedAuction.countSell > 0 && totalAuctionCountItems < itemtosell.StackLimit)
            {
                sellItemsCountSlider.maxValue = totalAuctionCountItems;
                totalCount.text = value + "/" + totalAuctionCountItems;
            }
            else
            {
                sellItemsCountSlider.maxValue = itemtosell.StackLimit;
                totalCount.text = value + "/" + itemtosell.StackLimit;
            }
            sellItemsCountSlider.value = value;
            sellslot.SetItemData(itemtosell, null);

            changeSellPrice();

            if (auctionlist)
                AtavismAuction.Instance.GetAuctionsForGroup(auctionGroupId, 0L);
            else
                AtavismAuction.Instance.GetAuctionsForGroup("", itemtosell.ItemId.ToLong());

        }
        /// <summary>
        /// 
        /// </summary>
        public void changeQuantitySlider()
        {
            //  int value = Inventory.Instance.GetCountOfItem(itemtosell.templateId);
            //  if (value > itemtosell.StackLimit) value = itemtosell.StackLimit;
            //  Debug.LogWarning("selectedAuction.countSell:" + selectedAuction.countSell + " itemtosell.StackLimit:" + itemtosell.StackLimit+ " auctionlist:"+ auctionlist+ " buyInstant:"+ buyInstant+ " showSell:"+ showSell+ " sellInstant:"+ sellInstant);

            if (auctionlist)
            {
                if (buyInstant)
                {


                    if (selectedAuction.countSell > 0 && totalAuctionCountItems < itemtosell.StackLimit)
                    {
                        sellItemsCountSlider.maxValue = totalAuctionCountItems;
                        totalCount.text = sellItemsCountSlider.value + "/" + totalAuctionCountItems;
                    }
                    else
                    {
                        sellItemsCountSlider.maxValue = itemtosell.StackLimit;
                        totalCount.text = sellItemsCountSlider.value + "/" + itemtosell.StackLimit;
                    }
                    //  int value = Inventory.Instance.GetCountOfItem(itemtosell.templateId);
                    //    if (value > itemtosell.StackLimit) value = itemtosell.StackLimit;
                    // asdasd
                    //   totalCount.text = sellItemsCountSlider.value + "/" + itemtosell.StackLimit;

                }
            }
            if (showSell)
            {
                if (sellInstant)
                {
                    //   totalCount.text = sellItemsCountSlider.value + "/" + value;
                }
                else
                {
                    int value = Inventory.Instance.GetCountOfItem(itemtosell.templateId);
                    if (value > itemtosell.StackLimit)
                        value = itemtosell.StackLimit;
                    totalCount.text = sellItemsCountSlider.value + "/" + value;
                }
            }
            sellItemsCount.text = sellItemsCountSlider.value.ToString();
            changeSellPrice();
        }
        /// <summary>
        /// 
        /// </summary>
        public void changeQuantity()
        {

            if (String.IsNullOrEmpty(sellItemsCount.text) || sellItemsCount.text == "0" || sellItemsCount.text == "" || sellItemsCount.text.Length == 0)
                sellItemsCount.text = "1";

            if (auctionlist)
            {
                totalCount.text = sellItemsCount.text + "/" + itemtosell.StackLimit;
                if (selectedAuction.countSell > 0 && selectedAuction.countSell < itemtosell.StackLimit)
                {
                    sellItemsCountSlider.maxValue = selectedAuction.countSell;
                    totalCount.text = sellItemsCount.text + "/" + selectedAuction.countSell;
                }
                else
                {
                    sellItemsCountSlider.maxValue = itemtosell.StackLimit;
                    totalCount.text = sellItemsCount.text + "/" + itemtosell.StackLimit;
                }   //  Dictionary<int, AuctionCountPrice> auctionsForGroupSell = AtavismAuction.Instance.AuctionsForGroupSell;

            }
            if (showSell)
            {
                if (sellInstant)
                {
                    //   totalCount.text = sellItemsCountSlider.value + "/" + value;
                }
                else
                {
                    int value = Inventory.Instance.GetCountOfItem(itemtosell.templateId);
                    if (value > itemtosell.StackLimit)
                        value = itemtosell.StackLimit;
                    if (value < int.Parse(sellItemsCount.text))
                        sellItemsCount.text = itemtosell.StackLimit.ToString();

                    totalCount.text = sellItemsCount.text + "/" + value;
                }
            }
            sellItemsCountSlider.value = int.Parse(sellItemsCount.text);
            changeSellPrice();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="j"></param>
        /// <param name="nested"></param>
        /// <param name="parentSocket"></param>
        private void GenerateMenu(List<menuTree> menu, int j, string nested, UGUIMenuSlot parentSocket)
        {
            for (int iii = 0; iii < menu.Count; iii++)
            {
                menuTree m = menu[iii];
                string nested1 = nested;
                if (nested.Length > 0)
                    nested1 += ";" + menuObjectNum;
                else
                    nested1 = menuObjectNum.ToString();
                if (menuObjects.Count > menuObjectNum)
                {
                    menuObjects[menuObjectNum].Setup(m.name, j, m.type.ToString(), m.value, Search, nested1, menuObjectNum, m.submenu.Count > 0, parentSocket);
                }
                else
                {
                    GameObject go = Instantiate(menuPrefab, menuGrid);
                    UGUIMenuSlot ms = go.GetComponent<UGUIMenuSlot>();
                    ms.Setup(m.name, j, m.type.ToString(), m.value, Search, nested1, menuObjectNum, m.submenu.Count > 0, parentSocket);
                    menuObjects.Add(ms);
                }
                //            Debug.LogWarning("generateMenu i:" + menuObjectNum + " j:" + j + " n:" + m.name + " nested:" + nested);
                menuObjectNum++;
                if (m.submenu.Count > 0)
                {
                    GenerateMenu(m.submenu, j + 1, nested1, menuObjects[menuObjectNum - 1]);// menuObjects[menuObjectNum]
                }
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="value"></param>
        /// <param name="nested"></param>
        void Search(string category, string value, string nested)
        {
            searchCatType = category;
            searchCat = value;
            searchCatDic.Clear();
            string[] nes = nested.Split(';');
            //  Debug.LogError("UGUI Auction Search " + value + " " + nested+" "+nes);
            //searchCatDic
            foreach (string s in nes)
            {
                int id = int.Parse(s);
                foreach (UGUIMenuSlot ms in menuObjects)
                {
                    if (id.Equals(ms.id))
                    {
                        searchCatDic.Add(ms.category, ms.value);
                        break;
                    }
                }

            }
            foreach (UGUIMenuSlot ms in menuObjects)
            {
                ms.SetupMark(false);
            }

            foreach (UGUIMenuSlot ms in menuObjects)
            {
                if (ms.getNested.Length > 1)
                {
                    string[] msNes = ms.getNested.Split(';');
                    bool showMenu = false;
                    if (msNes.Length == 1)
                    {
                        showMenu = true;
                    }
                    else
                        for (int ii = 0; ii < msNes.Length; ii++)
                        {
                            //                        Debug.LogWarning(nested + " | " + ms.getNested + " | ii:" + ii + " |0|" + nes.Length + " " + msNes.Length );
                            if (nes.Length - 2 > ii)
                            {
                                if (nes[ii].Equals(msNes[ii]))
                                {
                                    //                                Debug.LogWarning(nested +" | "+ ms.getNested+" ii:" + ii + "|1-1|" + nes.Length + " " + msNes.Length + " show");
                                    showMenu = true;
                                }
                                else
                                {
                                    //                                Debug.LogWarning(nested + " | " + ms.getNested + " ii:" + ii + "|1|"+ nes.Length+" "+msNes.Length + " break");
                                    showMenu = false;
                                    break;
                                }
                            }
                            else if (nes.Length - 2 == ii)
                            {
                                if (nes[ii].Equals(msNes[ii]))
                                {
                                    //                                Debug.LogWarning(nested + " | " + ms.getNested + " ii:" + ii + "|2-1|" + nes.Length + " " + msNes.Length + " show");
                                    showMenu = true;
                                }
                                else if (showMenu && nes[ii - 1].Equals(msNes[ii - 1]))
                                {
                                    //                                Debug.LogWarning(nested + " | " + ms.getNested + " ii:" + ii + "|2-2|" + nes.Length + " " + msNes.Length + " show");
                                    showMenu = true;
                                }
                                else
                                {
                                    //                                Debug.LogWarning(nested+ " | " + ms.getNested + " ii:" +ii + "|2|" + nes.Length + " " + msNes.Length + " break");
                                    showMenu = false;
                                    break;
                                }
                                //if (showMenu)

                            }
                            else if (nes.Length - 1 == ii)
                            {
                                if (nes[ii].Equals(msNes[ii]))
                                {
                                    //                                Debug.LogWarning(nested + " | " + ms.getNested + " ii:" + ii + "|3-1|" + nes.Length + " " + msNes.Length+" show");
                                    showMenu = true;
                                }
                                else if (showMenu && nes[ii - 1].Equals(msNes[ii - 1]))
                                {
                                    //                                Debug.LogWarning(nested + " | " + ms.getNested + " ii:" + ii + "|3-2|" + nes.Length + " " + msNes.Length + " show");
                                    showMenu = true;
                                }
                                else
                                {
                                    //                                Debug.LogWarning(nested + " | " + ms.getNested + " ii:" + ii + "|3|" + nes.Length + " " + msNes.Length + " break");
                                    showMenu = false;
                                    break;
                                }
                            }
                            else if (nes.Length == ii)
                            {
                                if (showMenu && nes[ii - 1].Equals(msNes[ii - 1]))
                                {
                                    //                                Debug.LogWarning(nested + " | " + ms.getNested + " ii:" + ii + "|5|" + nes.Length + " " + msNes.Length + " show");
                                    showMenu = true;
                                }
                                else
                                {
                                    //                                Debug.LogWarning(nested + " | " + ms.getNested + " ii:" + ii + "|5|" + nes.Length + " " + msNes.Length + " break");
                                    showMenu = false;
                                    break;
                                }
                            }
                            else
                            {
                                //                            Debug.LogWarning(nested + " | " + ms.getNested + " ii:" + ii+ "|4|" + nes.Length + " " + msNes.Length + " break");
                                showMenu = false;
                                break;
                            }
                        }
                    if (showMenu)
                    {
                        if (!ms.gameObject.activeSelf)
                        {
                            ms.gameObject.SetActive(true);
                        }
                        ms.SetupMark(false);
                        ms.Show();
                    }
                    else
                    {
                        if (ms.gameObject.activeSelf)
                        {
                            ms.gameObject.SetActive(false);
                        }
                        ms.SetupMark(false);
                    }
                }
                if (ms.nested.Equals(nested))
                {
                    ms.Selected(true);
                }
                else
                {
                    ms.Selected(false);
                }
            }
            List<object> list = new List<object>();
            foreach (int elm in qualitylevelsList)
                list.Add(elm);
            AtavismAuction.Instance.SearchAuction(sortCount, sortName, sortPrice, qualitylevels, searchRace, searchClass, minLevel, maxLevel, searchCatType, searchCat, searchText, list, sortAsc, searchCatDic);

        }
        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("AUCTION_OPEN", this);
            AtavismEventSystem.RegisterEvent("AUCTION_LIST_UPDATE", this);
            AtavismEventSystem.RegisterEvent("INVENTORY_UPDATE", this);
            AtavismEventSystem.RegisterEvent("AUCTION_LIST_FOR_GROUP_UPDATE", this);
            AtavismEventSystem.RegisterEvent("AUCTION_OWN_LIST_UPDATE", this);
            //  int i = 0;
            GenerateMenu(menuFilter, 0, "", null);
            foreach (UGUIMenuSlot ms in menuObjects)
            {
                if (ms != null)
                {
                    if (ms.getNested.Split(';').Length > 1)
                    {
                        if (ms.gameObject.activeSelf)
                            ms.gameObject.SetActive(false);
                    }
                }
                else
                {
                    Debug.LogWarning(" UGUIMenuSlot is null " + ms);
                }
            }
            Hide();
            if (searchMinLevelInput != null)
                searchMinLevelInput.text = "1";
            if (searchMaxLevelInput != null)
                searchMaxLevelInput.text = "999";

        }
        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("AUCTION_OPEN", this);
            AtavismEventSystem.UnregisterEvent("AUCTION_LIST_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("INVENTORY_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("AUCTION_LIST_FOR_GROUP_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("AUCTION_OWN_LIST_UPDATE", this);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}