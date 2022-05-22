using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using EasyBuildSystem.Features.Scripts.Core.Base.Builder;
using EasyBuildSystem.Features.Scripts.Core.Base.Builder.Enums;
using TMPro;

namespace Atavism
{

    public class UGUIWorldBuilder : AtList<UGUIClaimPermission>
    {

        static UGUIWorldBuilder instance;

        public UGUIPanelTitleBar titleBar;
        public Text claimName;
        public TextMeshProUGUI TMPClaimName;
        public TextMeshProUGUI TMPClaimType;
        public Text size;
        public TextMeshProUGUI TMPSize;
        public Text status;
        public TextMeshProUGUI TMPStatus;
        public Button buyButton;
        public Button sellButton;
        public Button createButton;
        public Button deleteButton;
        public Button permissionsButton;
     
        public UGUIBuildObjectsList buildObjectList;
        public TMP_Dropdown buildingCategory;
        public RectTransform editObjectPanel;
        public Text objectName;
        public TextMeshProUGUI TMPObjectName;
        public Text editMode;
        public TextMeshProUGUI TMPEditMode;
        public RectTransform constructObjectPanel;
        public Text constructObjectName;
        public TextMeshProUGUI TMPConstructObjectName;
        public Text objectStatus;
        public TextMeshProUGUI TMPObjectStatus;
        public List<UGUIBuildMaterialSlot> requiredItems;
        public RectTransform createClaimPanel;
        public Text claimSizeText;
        public TextMeshProUGUI TMPClaimSizeText;
        public TMP_Dropdown newClaimType;
        public UGUICurrencyInputPanel currencyInputPanel;
        public RectTransform sellClaimPanel;
        public Toggle sellClaimForSaleToggle;
        public UGUICurrencyInputPanel sellClaimCurrencyPanel;
        
        public TMP_Dropdown taxCurrency;
        public TMP_InputField taxAmount;
        public TMP_InputField taxInterval;
        public TMP_InputField timeWindowToPayTax;
        public TMP_InputField timeWindowToSellClaim;
        
        public RectTransform permissionsPanel;
        public Text permissionLevel;
        public TextMeshProUGUI TMPPermissionLevel;
        public InputField permissionPlayerName;
        public TMP_InputField TMPPermissionPlayerName;
        public KeyCode toggleKey;
        bool showing = false;
       
        public Button upgradeButton;
        public RectTransform upgradePanel;
        public TextMeshProUGUI TMPReqItemTitle;
        public List<UGUIBuildMaterialSlot> UpgradeRequiredItems;
        public List<UGUICurrency>  upgradeCost;
        public TextMeshProUGUI TMPUpgradeSize;
        //Taxs
        public Button taxPayButton;
        public TextMeshProUGUI taxStatusTitle;
        public TextMeshProUGUI taxStatusText;
        public TextMeshProUGUI taxInfoTitle;
        public TextMeshProUGUI taxInfoText;
        //Limits
        public RectTransform limitsPanel;
        public RectTransform limitsGrid;
        public UGUILimitDisplay limitsPrefab;
        public Button limitsButton;
        public List<UGUILimitDisplay> limitsList;
        private bool showLimits = false;

        //public KeyCode toggleKey;
        string newClaimName;
        int newClaimSize = 10;
        bool playerOwned = true;
        bool forSale = true;
        int currencyID = 0;
        long cost = 0;
        string playerPermissionName;
        int permissionLevelGiven = 0;
        string[] levels = new string[] { "Interaction", "Add Objects", "Edit Objects", "Add Users", "Manage Users" };

        // Use this for initialization
        void Start()
        {
            if (instance != null)
            {
                GameObject.DestroyImmediate(gameObject);
                return;
            }
            instance = this;

            if (titleBar != null)
                titleBar.SetOnPanelClose(Hide);
            Hide();
            SetClaimSize(newClaimSize);
            AtavismEventSystem.RegisterEvent("CLAIM_CHANGED", this);
            AtavismEventSystem.RegisterEvent("CLAIM_OBJECT_SELECTED", this);
            AtavismEventSystem.RegisterEvent("CLAIM_OBJECT_UPDATED", this);
            AtavismEventSystem.RegisterEvent("CLAIM_UPGRADE_SHOW", this);
         //   AtavismEventSystem.RegisterEvent("CLAIM_TAX_SHOW", this);
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("CLAIM_CHANGED", this);
            AtavismEventSystem.UnregisterEvent("CLAIM_OBJECT_SELECTED", this);
            AtavismEventSystem.UnregisterEvent("CLAIM_OBJECT_UPDATED", this);
            AtavismEventSystem.UnregisterEvent("CLAIM_UPGRADE_SHOW", this);
        //    AtavismEventSystem.UnregisterEvent("CLAIM_TAX_SHOW", this);
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey) && !ClientAPI.UIHasFocus())
            {
                if (showing)
                    Hide();
                else
                    Show();
            }
        }

        public void Toggle()
        {
            if (showing)
                Hide();
            else
                Show();
        }

        /*void OnEnable() {
            UpdateClaimDetails();
            WorldBuilder.Instance.ShowClaims = true;
        }*/

        /*void OnDisable() {
            WorldBuilder.Instance.ShowClaims = false;
            HidePanels();
        }*/

        public void Show()
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            showing = true;
            HidePanels();
            UpdateClaimDetails();
            WorldBuilder.Instance.ShowClaims = true;

            WorldBuilder.Instance.BuildingState = WorldBuildingState.SelectItem;
            BuilderBehaviour.Instance.ChangeMode(BuildMode.Edition);
          
            AtavismCursor.Instance.SetUGUIActivatableClickedOverride(WorldBuilder.Instance.StartPlaceClaimObject);
            AtavismUIUtility.BringToFront(gameObject);
        }

        public void Hide()
        {
         //   Debug.LogError("Hide");
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            showing = false;

            WorldBuilder.Instance.ShowClaims = false;
            HidePanels();
            WorldBuilder.Instance.BuildingState = WorldBuildingState.None;
            WorldBuilder.Instance.SelectedObject = null;
            if(BuilderBehaviour.Instance!=null)
                BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
            if (AtavismCursor.Instance != null)
                AtavismCursor.Instance.ClearUGUIActivatableClickedOverride(WorldBuilder.Instance.StartPlaceClaimObject);
            
        }

        public void OnEvent(AtavismEventData eData)
        {
        //    Debug.LogError("UGUIWorldBuilder OnEvent "+eData.eventType);
            if (eData.eventType == "CLAIM_UPGRADE_SHOW")
            {
                HidePanels();
           //     Debug.LogError("ClaimUpgradeMessage ");
                int claimId = int.Parse(eData.eventArgs[0]);
                int currency = int.Parse(eData.eventArgs[1]);
                long cost = long.Parse(eData.eventArgs[2]);

                if (upgradePanel != null)
                    upgradePanel.gameObject.SetActive(true);

                List<int> items = new List<int>();
                if (eData.eventArgs[3].Length > 0)
                {
                    string[] itemsList = eData.eventArgs[3].Split(',');
                    foreach (string item in itemsList)
                    {
                        if(item.Length>0)
                            items.Add(int.Parse(item));
                    }
                }
             //   Debug.LogError("ClaimUpgradeMessage "+items);
             if (UpgradeRequiredItems != null)
             {
               //  Debug.LogError("ClaimUpgradeMessage UpgradeRequiredItems count " + UpgradeRequiredItems.Count + " item count " + items.Count);
                 if (items.Count == 0)
                 {
                     if (TMPReqItemTitle)
                         TMPReqItemTitle.gameObject.SetActive(false);
                 }
                 else
                 {
                     if (TMPReqItemTitle)
                         if (!TMPReqItemTitle.gameObject.activeSelf)
                             TMPReqItemTitle.gameObject.SetActive(true);
                 }

                 int i = 0;
                 foreach (var tempId in items)
                 {
                     AtavismInventoryItem aii = AtavismPrefabManager.Instance.LoadItem(tempId);
                     if (UpgradeRequiredItems.Count > i)
                     {
                         if (!UpgradeRequiredItems[i].gameObject.activeSelf)
                             UpgradeRequiredItems[i].gameObject.SetActive(true);
                         UpgradeRequiredItems[i].UpdateBuildingSlotData(aii, 0);
                     }

                     i++;
                 }

              //   Debug.LogError("ClaimUpgradeMessage i=" + i);
                 for (int j = i; j < UpgradeRequiredItems.Count; j++)
                 {
                     UpgradeRequiredItems[j].UpdateBuildingSlotData(null, 0);
                     if (UpgradeRequiredItems[j].gameObject.activeSelf)
                         UpgradeRequiredItems[j].gameObject.SetActive(false);
                 }
             }

             if (upgradeCost != null)
                {
                    List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(currency, cost);
                    for (int i = 0; i < upgradeCost.Count; i++)
                    {
                        if (i < currencyDisplayList.Count)
                        {
                            upgradeCost[i].gameObject.SetActive(true);
                            upgradeCost[i].SetCurrencyDisplayData(currencyDisplayList[i]);
                        }
                        else
                        {
                            upgradeCost[i].gameObject.SetActive(false);
                        }
                    }
                }

                if (TMPUpgradeSize != null)
                {
                    TMPUpgradeSize.text = eData.eventArgs[4] + "x" + eData.eventArgs[5] + "x" + eData.eventArgs[6];
                }
            }
            else if (eData.eventType == "CLAIM_CHANGED")
            {
                if (WorldBuilder.Instance.ActiveClaim == null)
                {
                    Hide();
                }
                else
                {
                    if (showing)
                    {
                       /* if (showLimits)
                        {
                            if (limitsPanel)
                            {
                                limitsPanel.gameObject.SetActive(true);
                                updateDisplayLimits();
                            }
                        }*/

                        UpdateClaimDetails();
                        WorldBuilder.Instance.BuildingState = WorldBuildingState.EditItem;
                        if (BuilderBehaviour.Instance.CurrentMode == BuildMode.None)
                            BuilderBehaviour.Instance.ChangeMode(BuildMode.Edition);

                    }
                }
            }
            else if (eData.eventType == "CLAIM_OBJECT_SELECTED")
            {
                if (WorldBuilder.Instance.SelectedObject == null)
                {
                    HidePanels();
                    return;
                }
                for (int i = 0; i < requiredItems.Count; i++)
                {
                    requiredItems[i].Discarded();
                }
                AtavismBuildObjectTemplate template = WorldBuilder.Instance.GetBuildObjectTemplate(WorldBuilder.Instance.SelectedObject.TemplateID);
                if (template.upgradeItemsReq.Count == 0 || WorldBuilder.Instance.SelectedObject.FinalStage)
                {
                    ShowEditObject();
                }
                else
                {
                    ShowConstructObject();
                }
            }
            else if (eData.eventType == "CLAIM_OBJECT_UPDATED")
            {
            //   Debug.LogError("############### CLAIM_OBJECT_UPDATED "+WorldBuilder.Instance.SelectedObject);
                if (WorldBuilder.Instance.SelectedObject == null)
                {
                    HidePanels();
                    if (WorldBuilder.Instance.ActiveClaim == null || !WorldBuilder.Instance.ActiveClaim.playerOwned)
                        return;
                    buildObjectList.gameObject.SetActive(true);
                    ChangeCategory();
                    if (showLimits)
                    {
                        if (limitsPanel)
                        {
                            limitsPanel.gameObject.SetActive(true);
                            updateDisplayLimits();
                        }  }

                    if (buildingCategory!=null)
                    {
                        buildingCategory.options.Clear();
                        buildingCategory.options.Add(new TMP_Dropdown.OptionData()
                        {
                            text = "All"
                        });

                        if (WorldBuilder.Instance.ActiveClaim.limits.Count > 0)
                        {
                            foreach (var key in WorldBuilder.Instance.ActiveClaim.limits.Keys)
                            {
                                string n = WorldBuilder.Instance.GetBuildingCategory(key);
                                buildingCategory.options.Add(new TMP_Dropdown.OptionData()
                                {
                                    text = n
                                });
                            }
                        }
                        else
                        {

                            foreach (var ct in WorldBuilder.Instance.BuildingCategory.Keys)
                            {
                                // Debug.LogError("ShowObjectList "+WorldBuilder.Instance.BuildingCategory[ct].name);
                                buildingCategory.options.Add(new TMP_Dropdown.OptionData()
                                {
                                    text = WorldBuilder.Instance.BuildingCategory[ct].name
                                });
                            }
                        }

                        buildingCategory.SetValueWithoutNotify(0); 
                    }
                    return;
                }
              // Debug.LogError("############### CLAIM_OBJECT_UPDATED ItemReqs count "+WorldBuilder.Instance.SelectedObject.ItemReqs.Count+" FinalStage "+WorldBuilder.Instance.SelectedObject.FinalStage);
                if (WorldBuilder.Instance.SelectedObject.ItemReqs.Count == 0 || WorldBuilder.Instance.SelectedObject.FinalStage)
                {
                    for (int i = 0; i < requiredItems.Count; i++)
                    {
                        requiredItems[i].Discarded();
                    }
                    ShowEditObject();
                }
                else
                {
//                    Debug.LogError("############### ShowConstructObject");
                    ShowConstructObject();
                }
            }
        }

        IEnumerator UpdateTimer()
        {
          //  Debug.LogError("!!!!!!!!!!!!!!!!!! UpdateTimer " + WorldBuilder.Instance.ActiveClaim.taxTime);
            WaitForSeconds delay = new WaitForSeconds(1f);
            while (true)
            {
                if (WorldBuilder.Instance.ActiveClaim !=null && WorldBuilder.Instance.ActiveClaim.taxTime > Time.time)
                {
                    float _time = WorldBuilder.Instance.ActiveClaim.taxTime - Time.time;
                    int days = 0;
                    int hour = 0;
                    int minute = 0;
                    int secound = 0;
                    if (_time > 24 * 3600)
                    {
                        days = (int) (_time / (24F * 3600F));
                    }

                    if (_time - days * 24 * 3600 > 3600)
                        hour = (int) ((_time - days * 24 * 3600) / 3600F);
                    if (_time - days * 24 * 3600 - hour * 3600 > 60)
                        minute = (int) (_time - days * 24 * 3600 - hour * 3600) / 60;
                    //secound = (int) (_time - days * 24 * 3600 - hour * 3600 - minute * 60);
                    //   Debug.LogError( "$$$$$$$$$$$$$$$$            hour="+hour+" minute="+minute+" secound="+secound);
                    string outTime = "";
                    if (days > 0)
                    {
#if AT_I2LOC_PRESET
                    outTime += days + " " + I2.Loc.LocalizationManager.GetTranslation("days");
#else
                        outTime += days + " " + "days";
#endif
                    }

                    if (hour > 0)
                    {
                        if (minute > 9)
                            outTime += " " + hour + ":";
                        else
                            outTime += " 0" + hour + ":";
                        if (minute > 0)
                        {
                            if (minute < 10)
                                outTime += "0" + minute;
                            else
                                outTime += minute;
                        }
                        else
                        {
                            outTime += "00";
                        }
                    }
                    else if (minute > 0)
                    {
                        if (minute > 9)
                            outTime += " " + minute;
                        else
                            outTime += " 0" + minute;
                    }

                    //  outTime = (days > 0 ? days > 1 ? days + " days " : days + " day " : "") + (hour > 0 ? hour + "h " : "")+ (minute > 0 ? minute + "m" : "");
                    if (taxStatusText != null)
                        taxStatusText.text = outTime;

                }
                yield return delay;
            }
        }
        

        public void UpdateClaimDetails()
        {
            if(taxStatusText!=null)
                taxStatusText.gameObject.SetActive(false);
            if(taxStatusTitle!=null)
                taxStatusTitle.gameObject.SetActive(false);
            if (limitsPanel!=null && limitsPanel.gameObject.activeSelf)
                if (WorldBuilder.Instance.ActiveClaim != null)
                {
                    int index = 0;
                    if (limitsGrid != null)
                    {
                        foreach (var key in WorldBuilder.Instance.ActiveClaim.limits.Keys)
                        {
                            string c = (WorldBuilder.Instance.ActiveClaim.limitsCount.ContainsKey(key) ? WorldBuilder.Instance.ActiveClaim.limitsCount[key] + "" : "0") + "/" + WorldBuilder.Instance.ActiveClaim.limits[key];
                            string n = WorldBuilder.Instance.GetBuildingCategory(key);
                            if (limitsList.Count <= index)
                            {
                                UGUILimitDisplay go = (UGUILimitDisplay)Instantiate(limitsPrefab, limitsGrid);
                                limitsList.Add(go);
                            }
                            if(!limitsList[index].gameObject.activeSelf)
                                limitsList[index].gameObject.SetActive(true);
                            limitsList[index].Display(c,n);
                            index++;
                        }

                        for (int i = index; i < limitsList.Count; i++)
                        {
                            if(limitsList[index].gameObject.activeSelf)
                                limitsList[i].gameObject.SetActive(false);
                        }
                    }

                }
            if (permissionsPanel.gameObject.activeSelf && WorldBuilder.Instance.ActiveClaim != null)
            {
                PermissionsUpdated();

                if (claimName != null)
                    claimName.text = WorldBuilder.Instance.ActiveClaim.name;
                if (TMPClaimName != null)
                                    TMPClaimName.text = WorldBuilder.Instance.ActiveClaim.name;
                if (TMPClaimType != null)
                {
                    foreach (var ct in WorldBuilder.Instance.ClaimTypes.Values)
                    {
                        if (ct.id == WorldBuilder.Instance.ActiveClaim.claimType)
                        {
                            TMPClaimType.text = ct.name;
                        }
                    }
                }

                if (size != null)
                    size.text = WorldBuilder.Instance.ActiveClaim.sizeX + "x" +WorldBuilder.Instance.ActiveClaim.sizeY + "x" + WorldBuilder.Instance.ActiveClaim.sizeZ;
                if (TMPSize != null)
                    TMPSize.text = WorldBuilder.Instance.ActiveClaim.sizeX + "x" + WorldBuilder.Instance.ActiveClaim.sizeY + "x"+ WorldBuilder.Instance.ActiveClaim.sizeZ;
              
                
             //   Debug.LogWarning("UpdateClaimDetails: "+WorldBuilder.Instance.ActiveClaim.taxTime);
                if (WorldBuilder.Instance.ActiveClaim != null && WorldBuilder.Instance.ActiveClaim.taxTime > 0)
                {
                   /* if (WorldBuilder.Instance.ActiveClaim.permissionlevel > 0)
                    {*/
                        if (taxStatusText != null)
                            taxStatusText.gameObject.SetActive(true);
                        if (taxStatusTitle != null)
                            taxStatusTitle.gameObject.SetActive(true);
                        if (taxPayButton != null)
                            taxPayButton.gameObject.SetActive(true);
                        StopAllCoroutines();
                        StartCoroutine(UpdateTimer());
                   /* }
                    else
                    {
                        StopAllCoroutines();
                    }*/
                }
                else
                {
                    if(taxStatusText!=null)
                        taxStatusText.gameObject.SetActive(false);
                    if(taxStatusTitle!=null)
                        taxStatusTitle.gameObject.SetActive(false);
                    if(taxPayButton!=null)
                        taxPayButton.gameObject.SetActive(false);
                    
                }
                
                if (WorldBuilder.Instance.ActiveClaim.forSale)
                {
#if AT_I2LOC_PRESET
             if (status != null)     status.text = I2.Loc.LocalizationManager.GetTranslation("For Sale");
            if (TMPStatus != null)     TMPStatus.text = I2.Loc.LocalizationManager.GetTranslation("For Sale");
#else
                    if (status != null)
                        status.text = "For Sale";
                    if (TMPStatus != null)
                        TMPStatus.text = "For Sale";
#endif
                    if (!WorldBuilder.Instance.ActiveClaim.playerOwned)
                        buyButton.gameObject.SetActive(true);
                   
                }
                else
                {
#if AT_I2LOC_PRESET
              if (status != null)    status.text = I2.Loc.LocalizationManager.GetTranslation("Owned");
              if (TMPStatus != null)    TMPStatus.text = I2.Loc.LocalizationManager.GetTranslation("Owned");
#else
                    if (status != null)
                        status.text = "Owned";
                    if (TMPStatus != null)
                        TMPStatus.text = "Owned";
#endif
                    
                    
                  
                }

            
                
                
                return;
            }
            else
            {
                HidePanels();
                buyButton.gameObject.SetActive(false);
                sellButton.gameObject.SetActive(false);
                if(taxPayButton!=null)
                    taxPayButton.gameObject.SetActive(false);
                if(upgradeButton!=null)
                    upgradeButton.gameObject.SetActive(false);
                deleteButton.gameObject.SetActive(false);
                createButton.gameObject.SetActive(false);
                permissionsButton.gameObject.SetActive(false);
                if (WorldBuilder.Instance.ActiveClaim == null)
                {
                    if (claimName != null)
                        claimName.text = "-";
                    if (TMPClaimName != null)
                        TMPClaimName.text = "-";
                    if (size != null)
                        size.text = "-";
                    if (TMPSize != null)
                        TMPSize.text = "-";
                    if (status != null)
                        status.text = "-";
                    if (TMPStatus != null)
                        TMPStatus.text = "-";
                    if (taxInfoText != null)
                        taxInfoText.gameObject.SetActive(false);
                    if (taxInfoTitle != null)
                        taxInfoTitle.gameObject.SetActive(false);
                    if (TMPClaimType != null)
                    {
                        TMPClaimType.text = "-";
                    }
                    if (ClientAPI.IsPlayerAdmin())
                    {
                        createButton.gameObject.SetActive(true);
                    }
                    return;
                }
            }

            if (claimName != null)
                claimName.text = WorldBuilder.Instance.ActiveClaim.name;
            if (TMPClaimName != null)
                TMPClaimName.text = WorldBuilder.Instance.ActiveClaim.name;
            if (TMPClaimType != null)
            {
                foreach (var ct in WorldBuilder.Instance.ClaimTypes.Values)
                {
                    if (ct.id == WorldBuilder.Instance.ActiveClaim.claimType)
                    {
                        TMPClaimType.text = ct.name;
                    }
                }
            }
            if (size != null)
                size.text = WorldBuilder.Instance.ActiveClaim.sizeX + "x" + WorldBuilder.Instance.ActiveClaim.sizeY + "x" + WorldBuilder.Instance.ActiveClaim.sizeZ;
            if (TMPSize != null)
                TMPSize.text = WorldBuilder.Instance.ActiveClaim.sizeX + "x" + WorldBuilder.Instance.ActiveClaim.sizeY + "x" + WorldBuilder.Instance.ActiveClaim.sizeZ;
           
            if (WorldBuilder.Instance.ActiveClaim != null && WorldBuilder.Instance.ActiveClaim.taxTime > 0)
            {
                if (WorldBuilder.Instance.ActiveClaim.permissionlevel > 0||WorldBuilder.Instance.ActiveClaim.forSale)
                {
                    if (taxStatusText != null)
                        taxStatusText.gameObject.SetActive(true);
                    if (taxStatusTitle != null)
                        taxStatusTitle.gameObject.SetActive(true);
                    if (WorldBuilder.Instance.ActiveClaim.permissionlevel > 0)
                    {
                        if (taxPayButton != null)
                            taxPayButton.gameObject.SetActive(true);
                    }

                    StopAllCoroutines();
                    StartCoroutine(UpdateTimer());
                }
            }
            else
            {
                if(taxStatusText!=null)
                    taxStatusText.gameObject.SetActive(false);
                if(taxStatusTitle!=null)
                    taxStatusTitle.gameObject.SetActive(false);
                if(taxPayButton!=null)
                    taxPayButton.gameObject.SetActive(false);
                    
            }
            
            
            if (taxInfoTitle != null)
            {
                taxInfoTitle.gameObject.SetActive(true);
            }
                
            if (taxInfoText != null)
            {
                taxInfoText.gameObject.SetActive(true);
                long time = (long) WorldBuilder.Instance.ActiveClaim.taxInterval;
                if (WorldBuilder.Instance.ActiveClaim.taxAmount > 0)
                {
                long days = 0;
                long hour = 0;
                if (time > 24)
                {
                    days = (long) (time / 24F);
                    hour =  (time - (days * 24));
                }
                else
                {
                    hour = time;
                }
                string cost = Inventory.Instance.GetCostString(WorldBuilder.Instance.ActiveClaim.taxCurrency, WorldBuilder.Instance.ActiveClaim.taxAmount);
                
                
                long _days = 0;
                long _hour = 0;
                if (WorldBuilder.Instance.ActiveClaim.taxPeriodPay > 24)
                {
                    _days = (long) (WorldBuilder.Instance.ActiveClaim.taxPeriodPay / 24F);
                    _hour =  (WorldBuilder.Instance.ActiveClaim.taxPeriodPay - (_days * 24));
                }
                else
                {
                    _hour = WorldBuilder.Instance.ActiveClaim.taxPeriodPay;
                }
                
#if AT_I2LOC_PRESET
                     taxInfoText.text = cost + " "+I2.Loc.LocalizationManager.GetTranslation("per")+" " + (days > 0 ? days > 1 ? days + " "+I2.Loc.LocalizationManager.GetTranslation("days")+" " : days + " "+
                        I2.Loc.LocalizationManager.GetTranslation("day")+" " : "") + (hour > 0 ? hour + " "+I2.Loc.LocalizationManager.GetTranslation("hour") : "")+
                                  ". "+I2.Loc.LocalizationManager.GetTranslation("Can be paid")+" "+ (_days > 0 ? _days > 1 ? _days + " days " : _days + " day " : "") + (_hour > 0 ? _hour + " hour" : "")+" "+
                                 I2.Loc.LocalizationManager.GetTranslation("before tax expire");
#else
                taxInfoText.text = cost + " per " + (days > 0 ? days > 1 ? days + " days " : days + " day " : "") + (hour > 0 ? hour + " hour" : "")+
                                   ". Can be paid "+ (_days > 0 ? _days > 1 ? _days + " days " : _days + " day " : "") + (_hour > 0 ? _hour + " hour" : "")+" before tax expires";;
#endif
                }
                else
                {
#if AT_I2LOC_PRESET
                taxInfoText.text = I2.Loc.LocalizationManager.GetTranslation("No Tax");
#else
                    taxInfoText.text = "No tax";
#endif
                }
            }

            
            if (WorldBuilder.Instance.ActiveClaim.playerOwned)
            {
                buildObjectList.gameObject.SetActive(true);
                ChangeCategory();
                if (showLimits)
                {
                    if (limitsPanel)
                    {
                        limitsPanel.gameObject.SetActive(true);
                        updateDisplayLimits();
                    }      }

                sellButton.gameObject.SetActive(true);
                deleteButton.gameObject.SetActive(true);
                permissionsButton.gameObject.SetActive(true);
                if(upgradeButton!=null)
                    upgradeButton.gameObject.SetActive(true);
                if(taxPayButton!=null)
                    taxPayButton.gameObject.SetActive(true);
                if (buildingCategory!=null)
                {
                    buildingCategory.options.Clear();
                    buildingCategory.options.Add(new TMP_Dropdown.OptionData()
                    {
                        text = "All"
                    });
                    if (WorldBuilder.Instance.ActiveClaim.limits.Count > 0)
                    {
                        foreach (var key in WorldBuilder.Instance.ActiveClaim.limits.Keys)
                        {
                            string n = WorldBuilder.Instance.GetBuildingCategory(key);
                            buildingCategory.options.Add(new TMP_Dropdown.OptionData()
                            {
                                text = n
                            });
                        }
                    }
                    else
                    {

                        foreach (var ct in WorldBuilder.Instance.BuildingCategory.Keys)
                        {
                            // Debug.LogError("ShowObjectList "+WorldBuilder.Instance.BuildingCategory[ct].name);
                            buildingCategory.options.Add(new TMP_Dropdown.OptionData()
                            {
                                text = WorldBuilder.Instance.BuildingCategory[ct].name
                            });
                        }
                    }
                  
                    buildingCategory.SetValueWithoutNotify(0); 

                }

            }
            else if (WorldBuilder.Instance.ActiveClaim.permissionlevel > 0)
            {
                buildObjectList.gameObject.SetActive(true);
                ChangeCategory();
                if (showLimits)
                {
                    if (limitsPanel)
                    {
                        limitsPanel.gameObject.SetActive(true);
                        updateDisplayLimits();
                    }
                }
                if (buildingCategory!=null)
                {
                    buildingCategory.options.Clear();
                    buildingCategory.options.Add(new TMP_Dropdown.OptionData()
                    {
                        text = "All"
                    });
                    if (WorldBuilder.Instance.ActiveClaim.limits.Count > 0)
                    {
                        foreach (var key in WorldBuilder.Instance.ActiveClaim.limits.Keys)
                        {
                            string n = WorldBuilder.Instance.GetBuildingCategory(key);
                            buildingCategory.options.Add(new TMP_Dropdown.OptionData()
                            {
                                text = n
                            });
                        }
                    }
                    else
                    {

                        foreach (var ct in WorldBuilder.Instance.BuildingCategory.Keys)
                        {
                            // Debug.LogError("ShowObjectList "+WorldBuilder.Instance.BuildingCategory[ct].name);
                            buildingCategory.options.Add(new TMP_Dropdown.OptionData()
                            {
                                text = WorldBuilder.Instance.BuildingCategory[ct].name
                            });
                        }
                    }
                    buildingCategory.SetValueWithoutNotify(0); 

                }
                
                
                
            }
            else
            {
                buildObjectList.gameObject.SetActive(false);
            }

            if (WorldBuilder.Instance.ActiveClaim.forSale)
            {
#if AT_I2LOC_PRESET
             if (status != null) status.text = I2.Loc.LocalizationManager.GetTranslation("For Sale");
             if (TMPStatus != null) TMPStatus.text = I2.Loc.LocalizationManager.GetTranslation("For Sale");
#else
                if (status != null)
                    status.text = "For Sale";
                if (TMPStatus != null)
                    TMPStatus.text = "For Sale";
#endif
                if (!WorldBuilder.Instance.ActiveClaim.playerOwned)
                    buyButton.gameObject.SetActive(true);
            }
            else
            {
#if AT_I2LOC_PRESET
           if (status != null)   status.text = I2.Loc.LocalizationManager.GetTranslation("Owned");
           if (TMPStatus != null)   TMPStatus.text = I2.Loc.LocalizationManager.GetTranslation("Owned");
#else
                if (status != null)
                    status.text = "Owned";
                if (TMPStatus != null)
                    TMPStatus.text = "Owned";
#endif
            }
        }

        void HidePanels()
        {
            buildObjectList.gameObject.SetActive(false);
            editObjectPanel.gameObject.SetActive(false);
            createClaimPanel.gameObject.SetActive(false);
            sellClaimPanel.gameObject.SetActive(false);
            permissionsPanel.gameObject.SetActive(false);
            if (constructObjectPanel != null)
                constructObjectPanel.gameObject.SetActive(false);
            if (upgradePanel != null)
                upgradePanel.gameObject.SetActive(false);
            if(limitsPanel!=null)
                limitsPanel.gameObject.SetActive(false);
        }

        public void ShowLimits()
        {
            if (limitsPanel != null)
            {
                if (limitsPanel.gameObject.activeSelf)
                {
                    limitsPanel.gameObject.SetActive(false);
                    showLimits = false;
                }
                else
                {
                    limitsPanel.gameObject.SetActive(true);
                    showLimits = true;
                }

                updateDisplayLimits();
            }
        }

        void updateDisplayLimits()
        {
            if (limitsPanel.gameObject.activeSelf)
                if (WorldBuilder.Instance.ActiveClaim != null)
                {
                    int index = 0;
                    if (limitsGrid != null)
                    {
                        foreach (var key in WorldBuilder.Instance.ActiveClaim.limits.Keys)
                        {
                            string c = (WorldBuilder.Instance.ActiveClaim.limitsCount.ContainsKey(key) ? WorldBuilder.Instance.ActiveClaim.limitsCount[key] + "" : "0") + "/" + WorldBuilder.Instance.ActiveClaim.limits[key];
                            string n = WorldBuilder.Instance.GetBuildingCategory(key);
                            if (limitsList.Count <= index)
                            {
                                UGUILimitDisplay go = (UGUILimitDisplay) Instantiate(limitsPrefab, limitsGrid);
                                limitsList.Add(go);
                            }

                            if (!limitsList[index].gameObject.activeSelf)
                                limitsList[index].gameObject.SetActive(true);
                            limitsList[index].Display(c, n);
                            index++;
                        }

                        for (int i = index; i < limitsList.Count; i++)
                        {
                            if (limitsList[index].gameObject.activeSelf)
                                limitsList[i].gameObject.SetActive(false);
                        }
                    }

                }
        }

        public void DeleteClaim()
        {
#if AT_I2LOC_PRESET
       string message = I2.Loc.LocalizationManager.GetTranslation("Are you sure you want to delete your claim") + ": " + WorldBuilder.Instance.ActiveClaim.name;
#else
            string message = "Are you sure you want to delete your claim: " + WorldBuilder.Instance.ActiveClaim.name;
#endif
            UGUIConfirmationPanel.Instance.ShowConfirmationBox(message, null, ConfirmedDeleteClaim);
        }

        public void ConfirmedDeleteClaim(object obj, bool accepted)
        {
            if (accepted)
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("claimID", WorldBuilder.Instance.ActiveClaim.id);
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.DELETE_CLAIM", props);
                AtavismCursor.Instance.ClearUGUIActivatableClickedOverride(WorldBuilder.Instance.StartPlaceClaimObject);
            }
        }

        public void BuyClaim()
        {
            string costString = Inventory.Instance.GetCostString(WorldBuilder.Instance.ActiveClaim.currency, WorldBuilder.Instance.ActiveClaim.cost);
            string costitem = "";
            if (WorldBuilder.Instance.ActiveClaim.purchaseItemReq > 0)
            {
               AtavismInventoryItem aii = Inventory.Instance.GetItemByTemplateID(WorldBuilder.Instance.ActiveClaim.purchaseItemReq);
                costitem = " and " + aii.BaseName;
            }
#if AT_I2LOC_PRESET
         if (WorldBuilder.Instance.ActiveClaim.purchaseItemReq > 0)
            {
               AtavismInventoryItem aii = Inventory.Instance.GetItemByTemplateID(WorldBuilder.Instance.ActiveClaim.purchaseItemReq);
                costitem = " "+I2.Loc.LocalizationManager.GetTranslation("and")+ " " +  I2.Loc.LocalizationManager.GetTranslation("Items/"+aii.BaseName);
            }  
            string message = I2.Loc.LocalizationManager.GetTranslation("Are you sure you want to buy claim") + ": " + WorldBuilder.Instance.ActiveClaim.name
    + " " + I2.Loc.LocalizationManager.GetTranslation("for") + " " + costString + costitem+ "?";
           
#else
            string message = "Are you sure you want to buy claim: " + WorldBuilder.Instance.ActiveClaim.name
                + " for " + costString + costitem+"?";
#endif
            UGUIConfirmationPanel.Instance.ShowConfirmationBox(message, null, ConfirmedBuyClaim);
        }

        public void ConfirmedBuyClaim(object obj, bool accepted)
        {
            if (accepted)
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("claimID", WorldBuilder.Instance.ActiveClaim.id);
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.PURCHASE_CLAIM", props);
                AtavismCursor.Instance.ClearUGUIActivatableClickedOverride(WorldBuilder.Instance.StartPlaceClaimObject);
            }
        }

        #region Create Claim

        public void ShowCreateClaimPanel()
        {
            HidePanels();
            createButton.gameObject.SetActive(false);
            createClaimPanel.gameObject.SetActive(true);
            if (newClaimType)
            {
                newClaimType.options.Clear();
                foreach (var ct in WorldBuilder.Instance.ClaimTypes.Keys)
                {
                    
                    newClaimType.options.Add(new TMP_Dropdown.OptionData()
                    {
                        text = WorldBuilder.Instance.ClaimTypes[ct].name
                    });
                }
                newClaimType.SetValueWithoutNotify(0);
            }

            if (taxCurrency)
            {
                taxCurrency.options.Clear();
                foreach (var cur in Inventory.Instance.Currencies.Values)
                {
                    taxCurrency.options.Add(new TMP_Dropdown.OptionData()
                    {
                        text = cur.name
                    });
                }
                taxCurrency.SetValueWithoutNotify(0);
            }

         
            
            currencyInputPanel.SetCurrencies(Inventory.Instance.GetMainCurrencies());
        }

        public void SetClaimName(string name)
        {
            newClaimName = name;
        }

        public void SetClaimSize(float size)
        {
            newClaimSize = (int)size;
            if (claimSizeText != null)
                claimSizeText.text = newClaimSize.ToString();
            if (TMPClaimSizeText != null)
                TMPClaimSizeText.text = newClaimSize.ToString();
        }

        public void SetPlayerOwned(bool owned)
        {
            playerOwned = owned;
        }

        public void SetForSale(bool forSale)
        {
            this.forSale = forSale;
        }

        public void CreateClaim()
        {
            currencyInputPanel.GetCurrencyAmount(out currencyID, out cost);
            int ct = newClaimType.value;
            string ctn = newClaimType.options[newClaimType.value].text;
            int tCurrency = -1;
            if (taxCurrency)
            {
                int index = 0;
                foreach (var cur in Inventory.Instance.Currencies.Values)
                {
                 //   Debug.LogError("Create Claim cur "+cur.name+" "+cur.id);
                    if (index == taxCurrency.value)
                    {
                        tCurrency = cur.id;
                      //  Debug.LogError("Create Claim set cur "+cur.name+" "+cur.id);
                    }
                    index++;
                }
            }

            long tAmount = 0L;
            if (taxAmount)
            {
                if(taxAmount.text.Length>0)
                    tAmount = long.Parse(taxAmount.text);
            }

            long tInterval = 0L;
            if (taxInterval)
            {
                if (taxInterval.text.Length > 0)
                    tInterval = long.Parse(taxInterval.text);
            }

            long tTimeWindowToPayTax = 0L;
            if (timeWindowToPayTax)
            {
                if (timeWindowToPayTax.text.Length > 0)
                    tTimeWindowToPayTax = long.Parse(timeWindowToPayTax.text);
            }

            long tTimeWindowToSellClaim = 0L;
            if (timeWindowToSellClaim)
            {
                if (timeWindowToSellClaim.text.Length > 0)
                    tTimeWindowToSellClaim = long.Parse(timeWindowToSellClaim.text);
            }

            WorldBuilder.Instance.CreateClaim(newClaimName, newClaimSize, playerOwned, forSale, currencyID, cost, ct, ctn, tCurrency, tAmount, tInterval, tTimeWindowToPayTax, tTimeWindowToSellClaim);
            UpdateClaimDetails();
            //	ShowObjectList();
        }

        #endregion Create Claim

        #region Edit Object
        public void ShowObjectList()
        {
            HidePanels();
            buildObjectList.gameObject.SetActive(true);
            ChangeCategory();
            if (showLimits)
            {
                if (limitsPanel)
                {
                    limitsPanel.gameObject.SetActive(true);
                    updateDisplayLimits();
                }
            }
         //   Debug.LogError("ShowObjectList");
            if (buildingCategory!=null)
            {
                buildingCategory.options.Clear();
                buildingCategory.options.Add(new TMP_Dropdown.OptionData()
                {
                    text = "All"
                });
                if (WorldBuilder.Instance.ActiveClaim.limits.Count > 0)
                {
                    foreach (var key in WorldBuilder.Instance.ActiveClaim.limits.Keys)
                    {
                        string n = WorldBuilder.Instance.GetBuildingCategory(key);
                        buildingCategory.options.Add(new TMP_Dropdown.OptionData()
                        {
                            text = n
                        });
                    }
                }
                else
                {

                    foreach (var ct in WorldBuilder.Instance.BuildingCategory.Keys)
                    {
                        // Debug.LogError("ShowObjectList "+WorldBuilder.Instance.BuildingCategory[ct].name);
                        buildingCategory.options.Add(new TMP_Dropdown.OptionData()
                        {
                            text = WorldBuilder.Instance.BuildingCategory[ct].name
                        });
                    }
                }
                buildingCategory.SetValueWithoutNotify(0); 

            }

        }

        public void ChangeCategory()
        {
            if (buildObjectList != null && buildingCategory != null)
            {
                if (buildingCategory.value == 0)
                {
                        buildObjectList.changeCategory(-2);
                }
                else
                {
                    if (WorldBuilder.Instance.ActiveClaim.limits.Count > 0)
                    {
                        int index = 1;
                        foreach (var key in WorldBuilder.Instance.ActiveClaim.limits.Keys)
                        {
                            if (index == buildingCategory.value)
                            {
                                buildObjectList.changeCategory(key);
                            }
                            index++;
                        }
                    }
                    else
                    {
                        buildObjectList.changeCategory(WorldBuilder.Instance.BuildingCategory[buildingCategory.value - 1].id);
                    }
                }
            }
        }

        public void ShowEditObjectPanel()
        {
            HidePanels();
            editObjectPanel.gameObject.SetActive(true);
        }

        public void StartSelectObject()
        {
            WorldBuilder.Instance.BuildingState = WorldBuildingState.SelectItem;
            AtavismCursor.Instance.ClearUGUIActivatableClickedOverride(WorldBuilder.Instance.StartPlaceClaimObject);
           // WorldBuilderInterface.Instance.StartSelectObject();
            BuilderBehaviour.Instance.ChangeMode(BuildMode.Edition);
            
        }

        void ShowEditObject()
        {
          //  Debug.LogError("!!!!!!!!!!!!!!!!!   ShowEditObject");
            AtavismCursor.Instance.SetUGUIActivatableClickedOverride(WorldBuilder.Instance.StartPlaceClaimObject);
            HidePanels();
            editObjectPanel.gameObject.SetActive(true);

            if (objectName != null)
                objectName.text = WorldBuilder.Instance.SelectedObject.TemplateName;
            if (TMPObjectName != null)
                TMPObjectName.text = WorldBuilder.Instance.SelectedObject.TemplateName;
            if (WorldBuilderInterface.Instance.MouseWheelBuildMode == MouseWheelBuildMode.MoveVertical)
            {
#if AT_I2LOC_PRESET
           if (editMode != null)  editMode.text = I2.Loc.LocalizationManager.GetTranslation("Vertical");
           if (TMPEditMode != null)  TMPEditMode.text = I2.Loc.LocalizationManager.GetTranslation("Vertical");
#else
                if (editMode != null)
                    editMode.text = "Vertical";
                if (TMPEditMode != null)
                    TMPEditMode.text = "Vertical";
#endif
            }
            else
            {
#if AT_I2LOC_PRESET
           if (editMode != null)  editMode.text = I2.Loc.LocalizationManager.GetTranslation("Rotate");
           if (TMPEditMode != null)  TMPEditMode.text = I2.Loc.LocalizationManager.GetTranslation("Rotate");
#else
                if (editMode != null)
                    editMode.text = "Rotate";
                if (TMPEditMode != null)
                    TMPEditMode.text = "Rotate";
#endif
            }
        }

        public void StartMoveItem()
        {
            if (WorldBuilder.Instance.SelectedObject == null)
            {
                Debug.LogError("WorldBuilder: SelectedObject is null");
                return;
            }
            if (BuilderBehaviour.Instance.CurrentMode == BuildMode.Placement)
            {
                return;
            }

            if (!WorldBuilder.Instance.SelectedObject.canBeMoved)
            {
                string[] args = new string[1];
#if AT_I2LOC_PRESET
            args[0] = WorldBuilder.Instance.SelectedObject.TemplateName + " " + I2.Loc.LocalizationManager.GetTranslation("cannot be moved.");
#else
                args[0] = WorldBuilder.Instance.SelectedObject.TemplateName + " cannot be moved.";
#endif
                AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
                return;
            }
            if (WorldBuilder.Instance.BuildingState != WorldBuildingState.MoveItem)
            {
                WorldBuilder.Instance.BuildingState = WorldBuildingState.MoveItem;
                WorldBuilder.Instance.SelectedObject.ObjectPlaced = false;

                WorldBuilderInterface.Instance.SetCurrentReticle(WorldBuilder.Instance.SelectedObject.gameObject);
            }
        }

        public void ChangeEditMode()
        {
            if (WorldBuilderInterface.Instance.MouseWheelBuildMode == MouseWheelBuildMode.MoveVertical)
            {
                WorldBuilderInterface.Instance.MouseWheelBuildMode = MouseWheelBuildMode.Rotate;
#if AT_I2LOC_PRESET
             if (editMode != null)  editMode.text = I2.Loc.LocalizationManager.GetTranslation("Rotate");
             if (TMPEditMode != null)  TMPEditMode.text = I2.Loc.LocalizationManager.GetTranslation("Rotate");
#else
                if (editMode != null)
                    editMode.text = "Rotate";
                if (TMPEditMode != null)
                    TMPEditMode.text = "Rotate";
#endif
            }
            else
            {
                WorldBuilderInterface.Instance.MouseWheelBuildMode = MouseWheelBuildMode.MoveVertical;
#if AT_I2LOC_PRESET
             if (editMode != null)  editMode.text = I2.Loc.LocalizationManager.GetTranslation("Vertical");
             if (TMPEditMode != null)  TMPEditMode.text = I2.Loc.LocalizationManager.GetTranslation("Vertical");
#else
                if (editMode != null)
                    editMode.text = "Vertical";
                if (TMPEditMode != null)
                    TMPEditMode.text = "Vertical";
#endif
            }
        }

        public void RemoveItem()
        {
            WorldBuilder.Instance.PickupClaimObject();
            HidePanels();
            WorldBuilder.Instance.BuildingState = WorldBuildingState.SelectItem;
            BuilderBehaviour.Instance.ChangeMode(BuildMode.Edition);
            buildObjectList.gameObject.SetActive(true);
            ChangeCategory();
        }

        public void SaveObjectChanges()
        {
            HidePanels();
            WorldBuilder.Instance.BuildingState = WorldBuildingState.SelectItem;
            BuilderBehaviour.Instance.ChangeMode(BuildMode.Edition);
            buildObjectList.gameObject.SetActive(true);
            ChangeCategory();
            WorldBuilder.Instance.SelectedObject.ObjectPlaced = false;

        }

        #endregion Edit Object

        #region Construct Building
        void ShowConstructObject()
        {
            Debug.LogError("ShowConstructObject");
            HidePanels();
            constructObjectPanel.gameObject.SetActive(true);

            if (constructObjectName != null)
                constructObjectName.text = WorldBuilder.Instance.SelectedObject.TemplateName;
            if (TMPConstructObjectName != null)
                TMPConstructObjectName.text = WorldBuilder.Instance.SelectedObject.TemplateName;
            if (WorldBuilder.Instance.SelectedObject.Complete)
            {
                if (WorldBuilder.Instance.SelectedObject.Health < WorldBuilder.Instance.SelectedObject.MaxHealth)
                {
#if AT_I2LOC_PRESET
                string statusText = I2.Loc.LocalizationManager.GetTranslation("Damaged") + ": " + WorldBuilder.Instance.SelectedObject.Health + "/" + WorldBuilder.Instance.SelectedObject.MaxHealth;
#else
                    string statusText = "Damaged: " + WorldBuilder.Instance.SelectedObject.Health + "/" + WorldBuilder.Instance.SelectedObject.MaxHealth;
#endif
                    if (objectStatus != null)
                        objectStatus.text = statusText;
                    if (TMPObjectStatus != null)
                        TMPObjectStatus.text = statusText;
                }
                else
                {
#if AT_I2LOC_PRESET
                if (objectStatus != null) objectStatus.text = I2.Loc.LocalizationManager.GetTranslation("Complete");
                if (TMPObjectStatus != null) TMPObjectStatus.text = I2.Loc.LocalizationManager.GetTranslation("Complete");
#else
                    if (objectStatus != null)
                        objectStatus.text = "Complete";
                    if (TMPObjectStatus != null)
                        TMPObjectStatus.text = "Complete";
#endif
                }
            }
            else
            {
#if AT_I2LOC_PRESET
            string statusText = I2.Loc.LocalizationManager.GetTranslation("In Construction") + ": " + WorldBuilder.Instance.SelectedObject.Health + "/" + WorldBuilder.Instance.SelectedObject.MaxHealth;
#else
                string statusText = "In Construction: " + WorldBuilder.Instance.SelectedObject.Health + "/" + WorldBuilder.Instance.SelectedObject.MaxHealth;
#endif
                if (objectStatus != null)
                    objectStatus.text = statusText;
                if (TMPObjectStatus != null)
                    TMPObjectStatus.text = statusText;
            }

            for (int i = 0; i < requiredItems.Count; i++)
            {
                requiredItems[i].gameObject.SetActive(false);
            }

            int itemNum = 0;
            foreach (int itemID in WorldBuilder.Instance.SelectedObject.ItemReqs.Keys)
            {

                requiredItems[itemNum].gameObject.SetActive(true);
                AtavismInventoryItem item = Inventory.Instance.GetItemByTemplateID(itemID);
                
                requiredItems[itemNum].UpdateBuildingSlotData(item, WorldBuilder.Instance.SelectedObject.ItemReqs[itemID]);
                itemNum++;
            }
        }

        public void BuildClicked()
        {
            WorldBuilder.Instance.ImproveBuildObject();
        }

        #endregion Construct Building

        #region Sell Claim
        public void ShowSellClaimPanel()
        {
            HidePanels();
            sellClaimPanel.gameObject.SetActive(true);
            sellClaimCurrencyPanel.SetCurrencies(Inventory.Instance.GetMainCurrencies());
            sellClaimCurrencyPanel.SetCurrencyAmounts(WorldBuilder.Instance.ActiveClaim.currency, WorldBuilder.Instance.ActiveClaim.cost);
            sellClaimForSaleToggle.isOn = WorldBuilder.Instance.ActiveClaim.forSale;
        }

        public void SaveSellClaimSettings()
        {
            WorldBuilder.Instance.ActiveClaim.forSale = sellClaimForSaleToggle.isOn;
            sellClaimCurrencyPanel.GetCurrencyAmount(out currencyID, out cost);
            WorldBuilder.Instance.ActiveClaim.currency = currencyID;
            WorldBuilder.Instance.ActiveClaim.cost = cost;
            WorldBuilder.Instance.SendEditClaim();
            ShowObjectList();
        }
        

        #endregion Sell Claim

        #region Permissions
        public void ShowPermissionsPanel()
        {
            HidePanels();
            permissionsPanel.gameObject.SetActive(true);
            playerPermissionName = "";
            permissionLevelGiven = 0;
#if AT_I2LOC_PRESET
       if (permissionLevel!=null)  permissionLevel.text = I2.Loc.LocalizationManager.GetTranslation(levels[permissionLevelGiven].ToString());
       if (TMPPermissionLevel!=null)  TMPPermissionLevel.text = I2.Loc.LocalizationManager.GetTranslation(levels[permissionLevelGiven].ToString());
#else
            if (permissionLevel != null)
                permissionLevel.text = levels[permissionLevelGiven].ToString();
            if (TMPPermissionLevel != null)
                TMPPermissionLevel.text = levels[permissionLevelGiven].ToString();
#endif

            // Delete the old list
            ClearAllCells();

            Refresh();
        }

        void PermissionsUpdated()
        {
            // Delete the old list
            ClearAllCells();

            Refresh();
        }

        public void SetPermissionPlayerName(string name)
        {
            if (permissionPlayerName)
                playerPermissionName = permissionPlayerName.text;
            else if (TMPPermissionPlayerName)
                playerPermissionName = TMPPermissionPlayerName.text;
            else
                playerPermissionName = name;
        }

        public void ChangePermissionLevel()
        {
            permissionLevelGiven++;
            if (permissionLevelGiven >= levels.Length)
            {
                permissionLevelGiven = 0;
            }
#if AT_I2LOC_PRESET
        if (permissionLevel != null)  permissionLevel.text = I2.Loc.LocalizationManager.GetTranslation(levels[permissionLevelGiven].ToString());
        if (TMPPermissionLevel != null)  TMPPermissionLevel.text = I2.Loc.LocalizationManager.GetTranslation(levels[permissionLevelGiven].ToString());
#else
            if (permissionLevel != null)
                permissionLevel.text = levels[permissionLevelGiven].ToString();
            if (TMPPermissionLevel != null)
                TMPPermissionLevel.text = levels[permissionLevelGiven].ToString();
#endif
        }

        public void AddPermission()
        {
            // Add 1 to permission level because I'm an idiot and set it to index 1 on the server
            WorldBuilder.Instance.AddPermission(playerPermissionName, permissionLevelGiven + 1);
            if (permissionPlayerName)
                 permissionPlayerName.text="";
            else if (TMPPermissionPlayerName)
                 TMPPermissionPlayerName.text="";
        }


        public void ShowUpgradeClaim()
        {
            
            WorldBuilder.Instance.SendGetUpgradeClaim();
        }
        
        public void SendUpgradeClaim()
        {
            
            WorldBuilder.Instance.SendUpgradeClaim();
            UpdateClaimDetails();
        }

        public void ClickPayTax()
        {
            if(WorldBuilder.Instance.ActiveClaim!=null)
                WorldBuilder.Instance.SendPayTaxForClaim(WorldBuilder.Instance.ActiveClaim.id, false);
        }
        
        #region implemented abstract members of AtList

        public override int NumberOfCells()
        {
            int numCells = WorldBuilder.Instance.ActiveClaim.permissions.Count;
            return numCells;
        }

        public override void UpdateCell(int index, UGUIClaimPermission cell)
        {
            cell.SetPermissionDetails(WorldBuilder.Instance.ActiveClaim.permissions[index]);
        }

        #endregion

        #endregion Permissions

        public static UGUIWorldBuilder Instance
        {
            get
            {
                return instance;
            }
        }

        public bool Showing
        {
            get
            {
                return showing;
            }
        }
    }
}