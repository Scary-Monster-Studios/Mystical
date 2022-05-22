using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

namespace Atavism
{

    public delegate void AdminChooseEntryClicked(int id);

    public class UGUIAdminPanel : AtList<UGUIAdminChooseEntry>
    {

        enum ChooseType
        {
            Currency,
            Item,
            Skill,
            Weather
        }

        static UGUIAdminPanel instance;

        public UGUIPanelTitleBar titleBar;
        public Text gmStatusText;
        public TextMeshProUGUI TMPGmStatusText;
        public Text instanceText;
        public TextMeshProUGUI TMPInstanceText;
        public Text positionText;
        public TextMeshProUGUI TMPPositionText;

        [AtavismSeparator("Teleport Commands")]
        public RectTransform teleportPanel;
        public InputField teleportToPlayerField;
        public TMP_InputField TMPTeleportToPlayerField;
        public InputField summonPlayerField;
        public TMP_InputField TMPSummonPlayerField;
        public InputField changeInstanceField;
        public TMP_InputField TMPChangeInstanceField;
        public InputField gotoXField;
        public TMP_InputField TMPGotoXField;
        public InputField gotoYField;
        public TMP_InputField TMPGotoYField;
        public InputField gotoZField;
        public TMP_InputField TMPGotoZField;
        public Dropdown instanceLocs;
        public TMP_Dropdown TMPInstanceLocs;
        [AtavismSeparator("Gain Commands")]
        public RectTransform gainCommandsPanel;
        public InputField expField;
        public TMP_InputField TMPExpField;
        public Button chooseItemButton;
        public InputField itemCountField;
        public TMP_InputField TMPItemCountField;
        public Button chooseCurrencyButton;
        public InputField currencyCountField;
        public TMP_InputField TMPCurrencyCountField;
        public Button chooseSkillButton;
        public InputField skillCountField;
        public TMP_InputField TMPSkillCountField;
        public RectTransform choosePanel;
        public TextMeshProUGUI TMPItemIconCountText;
        public TextMeshProUGUI TMPSkillIconCountText;
        public TextMeshProUGUI TMPCurrencyIconCountText;
        public InputField iconLoadPartCountField;
        public TMP_InputField TMPIconLoadPartCountField;
        public Button itemIconLoadButton;
        public Button skillIconLoadButton;
        public Button currencyIconLoadButton;

      
        [AtavismSeparator("Weather Commands")]
        public RectTransform weatherCommandsPanel;
        public TMP_InputField TMPYearField;
        public TMP_InputField TMPMonthField;
        public TMP_InputField TMPDayField;
        public TMP_InputField TMPHourField;
        public TMP_InputField TMPMinuteField;
        public Button setTimeButton;
        public Button chooseWeatherButton;
        [AtavismSeparator("Choose Window")]
        public Text chooseTitle;
        public TextMeshProUGUI TMPChooseTitle;
        public InputField filterInputField;
        public TMP_InputField TMPFilterInputField;
        [AtavismSeparator("Server Commands")]
        public RectTransform serverCommandsPanel;
        public TMP_InputField TMPServerMessageField;
        public TMP_InputField TMPServerCountdowanField;
        public TMP_InputField TMPServerScheduleField;
        public TMP_Dropdown TMPServerCommandProfile;
        public Toggle TMPServerRestartToggle;
       
        
        public Button serverReloadButton;
        public RectTransform serverReloadProgressPanel;
        public Slider serverReloadProgressSlider;



        bool gmActive = false;
        ChooseType chooseType = ChooseType.Currency;
        int currencyID = -1;
        int itemID = -1;
        int skillID = -1;
        int profileId = -1;

        bool showing = false;
        string actLoc = "";

        private bool autoItemIconGet = false;
        private bool autoSkillIconGet = false;
        private bool autoCurrencyIconGet = false;
        // Use this for initialization
        void Start()
        {
            if (instance != null)
            {
                GameObject.DestroyImmediate(gameObject);
                return;
            }

            instance = this;
            AtavismEventSystem.RegisterEvent("ITEM_ICON_UPDATE", this);
            AtavismEventSystem.RegisterEvent("CURRENCY_ICON_UPDATE", this);
            AtavismEventSystem.RegisterEvent("SKILL_ICON_UPDATE", this);
            AtavismEventSystem.RegisterEvent("SETTINGS", this);

            if (titleBar != null)
                titleBar.SetOnPanelClose(Hide);
            Hide();
            if (choosePanel != null)
                choosePanel.gameObject.SetActive(false);
            if (ClientAPI.GetPlayerObject() != null)
            {
                ClientAPI.GetPlayerObject().RegisterPropertyChangeHandler("gm", GMHandler);
                if (ClientAPI.GetPlayerObject().PropertyExists("gm"))
                {
                    gmActive = (bool) ClientAPI.GetPlayerObject().GetProperty("gm");
                }
            }

            if (gmActive)
            {
                if (gmStatusText != null)
                    gmStatusText.text = "Active";
                if (TMPGmStatusText != null)
                    TMPGmStatusText.text = "Active";
            }
            else
            {
                if (gmStatusText != null)
                    gmStatusText.text = "Inactive";
                if (TMPGmStatusText != null)
                    TMPGmStatusText.text = "Inactive";
            }

            if (serverReloadButton)
                if (AtavismPrefabManager.Instance.PrefabReloading)
                {
                    serverReloadButton.interactable = true;
                }
                else
                {
                    serverReloadButton.interactable = false;
                }
            if(serverReloadProgressPanel)
                serverReloadProgressPanel.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("ITEM_ICON_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("CURRENCY_ICON_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("SKILL_ICON_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("SETTINGS", this);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "ITEM_ICON_UPDATE")
            {
                if (TMPItemIconCountText)
                {
                    TMPItemIconCountText.text = AtavismPrefabManager.Instance.GetCountLoadedItemIcons() + " / " + AtavismPrefabManager.Instance.GetCountItems();
                }

                if (autoItemIconGet)
                {
                    if (AtavismPrefabManager.Instance.GetCountItems() - AtavismPrefabManager.Instance.GetCountLoadedItemIcons() > 0)
                    {
                        int count = iconLoadPartCountField != null ? iconLoadPartCountField.text.Length > 0 ? int.Parse(iconLoadPartCountField.text) : -1 : -1;
                        if (count == -1)
                            count = TMPIconLoadPartCountField != null ? TMPIconLoadPartCountField.text.Length > 0 ? int.Parse(TMPIconLoadPartCountField.text) : -1 : -1;

                        if (count < 1)
                        {
                            AtavismPrefabManager.Instance.LoadItemIcons();
                        }
                        else
                        {
                            AtavismPrefabManager.Instance.LoadItemIcons(count);
                        }
                    }
                    else
                    {
                        autoItemIconGet = !autoItemIconGet;
                       
                        if (itemIconLoadButton)
                        {
                            if (itemIconLoadButton.GetComponentInChildren<Text>() != null)
                                itemIconLoadButton.GetComponentInChildren<Text>().text = "LOAD";
                            if (itemIconLoadButton.GetComponentInChildren<TextMeshProUGUI>() != null)
                                itemIconLoadButton.GetComponentInChildren<TextMeshProUGUI>().text =  "LOAD";
                        }
                    }
                }

                if (chooseType == ChooseType.Item)
                {
                    ClearAllCells();
                    Refresh();
                }
            }else
            if (eData.eventType == "Skill_ICON_UPDATE")
            {
                if (TMPSkillIconCountText)
                {
                    TMPSkillIconCountText.text = AtavismPrefabManager.Instance.GetCountLoadedSkillIcons() + " / " + AtavismPrefabManager.Instance.GetCountSkills();
                }

                if (autoSkillIconGet)
                {
                    if (AtavismPrefabManager.Instance.GetCountSkills() - AtavismPrefabManager.Instance.GetCountLoadedSkillIcons() > 0)
                    {
                        int count = iconLoadPartCountField != null ? iconLoadPartCountField.text.Length > 0 ? int.Parse(iconLoadPartCountField.text) : -1 : -1;
                        if (count == -1)
                            count = TMPIconLoadPartCountField != null ? TMPIconLoadPartCountField.text.Length > 0 ? int.Parse(TMPIconLoadPartCountField.text) : -1 : -1;

                        if (count < 1)
                        {
                            AtavismPrefabManager.Instance.LoadSkillIcons();
                        }
                        else
                        {
                            AtavismPrefabManager.Instance.LoadSkillIcons(count);
                        }
                    }
                    else
                    {
                        autoSkillIconGet = !autoSkillIconGet;
                       
                        if (skillIconLoadButton)
                        {
                            if (skillIconLoadButton.GetComponentInChildren<Text>() != null)
                                skillIconLoadButton.GetComponentInChildren<Text>().text = "LOAD";
                            if (skillIconLoadButton.GetComponentInChildren<TextMeshProUGUI>() != null)
                                skillIconLoadButton.GetComponentInChildren<TextMeshProUGUI>().text =  "LOAD";
                        }
                    }
                }

                if (chooseType == ChooseType.Skill)
                {
                    ClearAllCells();
                    Refresh();
                }
            }else
            if (eData.eventType == "CURRENCY_ICON_UPDATE")
            {
                if (TMPCurrencyIconCountText)
                {
                    TMPCurrencyIconCountText.text = AtavismPrefabManager.Instance.GetCountLoadedCurrencyIcons() + " / " + AtavismPrefabManager.Instance.GetCountCurrencies();
                }

                if (autoCurrencyIconGet)
                {
                    if (AtavismPrefabManager.Instance.GetCountCurrencies() - AtavismPrefabManager.Instance.GetCountLoadedCurrencyIcons() > 0)
                    {
                        int count = iconLoadPartCountField != null ? iconLoadPartCountField.text.Length > 0 ? int.Parse(iconLoadPartCountField.text) : -1 : -1;
                        if (count == -1)
                            count = TMPIconLoadPartCountField != null ? TMPIconLoadPartCountField.text.Length > 0 ? int.Parse(TMPIconLoadPartCountField.text) : -1 : -1;

                        if (count < 1)
                        {
                            AtavismPrefabManager.Instance.LoadCurrencyIcons();
                        }
                        else
                        {
                            AtavismPrefabManager.Instance.LoadCurrencyIcons(count);
                        }
                    }
                    else
                    {
                        autoCurrencyIconGet = !autoCurrencyIconGet;
                       
                        if (currencyIconLoadButton)
                        {
                            if (currencyIconLoadButton.GetComponentInChildren<Text>() != null)
                                currencyIconLoadButton.GetComponentInChildren<Text>().text = "LOAD";
                            if (currencyIconLoadButton.GetComponentInChildren<TextMeshProUGUI>() != null)
                                currencyIconLoadButton.GetComponentInChildren<TextMeshProUGUI>().text =  "LOAD";
                        }
                    }
                }

                if (chooseType == ChooseType.Currency)
                {
                    ClearAllCells();
                    Refresh();
                }
            }
            else if (eData.eventType == "SETTINGS")
            {
              //  Debug.LogError("SETTINGS");
                if (serverReloadButton)
                    if (AtavismPrefabManager.Instance.PrefabReloading)
                    {
                        serverReloadButton.interactable = true;
                    }
                    else
                    {
                        serverReloadButton.interactable = false;
                    }
            }
            
        }

        public void Toggle()
        {
            if (showing)
                Hide();
            else
                Show();
        }

        public void Show()
        {
            int adminLevel = (int) ClientAPI.GetObjectProperty(ClientAPI.GetPlayerOid(), "adminLevel");
            if (adminLevel >= 3 )
            {
                GetComponent<CanvasGroup>().alpha = 1f;
                GetComponent<CanvasGroup>().blocksRaycasts = true;
                showing = true;
                gameObject.SetActive(true);
                AtavismUIUtility.BringToFront(gameObject);
                if (serverReloadButton)
                    if (AtavismPrefabManager.Instance.PrefabReloading)
                    {
                        serverReloadButton.interactable = true;
                    }
                    else
                    {
                        serverReloadButton.interactable = false;
                    }
                if(serverReloadProgressPanel)
                    serverReloadProgressPanel.gameObject.SetActive(false);
            }
            else
            {
                Hide();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            if(serverReloadProgressPanel)
                serverReloadProgressPanel.gameObject.SetActive(true);
            showing = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (showing)
            {
                //			instanceText.text = Application.loadedLevelName;
                if (instanceText != null)
                    instanceText.text = SceneManager.GetActiveScene().name;
                if (TMPInstanceText != null)
                    TMPInstanceText.text = SceneManager.GetActiveScene().name;
                if (ClientAPI.GetPlayerObject() != null && ClientAPI.GetPlayerObject().Position != null)
                {
                    if (positionText != null)
                        positionText.text = ClientAPI.GetPlayerObject().Position.x.ToString("n2") + "," + ClientAPI.GetPlayerObject().Position.y.ToString("n2") + "," + ClientAPI.GetPlayerObject().Position.z.ToString("n2");
                    if (TMPPositionText != null)
                        TMPPositionText.text = ClientAPI.GetPlayerObject().Position.x.ToString("n2") + "," + ClientAPI.GetPlayerObject().Position.y.ToString("n2") + "," + ClientAPI.GetPlayerObject().Position.z.ToString("n2");
                }
                if (!actLoc.Equals(SceneManager.GetActiveScene().name))
                {
                    actLoc = SceneManager.GetActiveScene().name;
                    UpdateLocList();
                }
            }
        }

        public void GMHandler(object sender, PropertyChangeEventArgs args)
        {
            gmActive = (bool)ClientAPI.GetPlayerObject().GetProperty("gm");
            if (gmActive)
            {
                if (gmStatusText != null)
                    gmStatusText.text = "Active";
                if (TMPGmStatusText != null)
                    TMPGmStatusText.text = "Active";
            }
            else
            {
                if (gmStatusText != null)
                    gmStatusText.text = "Inactive";
                if (TMPGmStatusText != null)
                    TMPGmStatusText.text = "Inactive";
            }
        }

        public void ToggleGMMode()
        {
            if (gmActive)
            {
                NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/gm 0");
            }
            else
            {
                NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/gm 1");
            }
        }

        public void ShowTeleportOptions()
        {
            if(teleportPanel)  teleportPanel.gameObject.SetActive(true);
            if(gainCommandsPanel)  gainCommandsPanel.gameObject.SetActive(false);
            if(weatherCommandsPanel)weatherCommandsPanel.gameObject.SetActive(false);
            if(serverCommandsPanel)serverCommandsPanel.gameObject.SetActive(false);

        }

        public void ShowGainCommands()
        {
            if(teleportPanel)  teleportPanel.gameObject.SetActive(false);
            if(gainCommandsPanel)  gainCommandsPanel.gameObject.SetActive(true);
            if(weatherCommandsPanel)weatherCommandsPanel.gameObject.SetActive(false);
            if(serverCommandsPanel)serverCommandsPanel.gameObject.SetActive(false);

            if (TMPItemIconCountText)
            {
                TMPItemIconCountText.text = AtavismPrefabManager.Instance.GetCountLoadedItemIcons() + " / " + AtavismPrefabManager.Instance.GetCountItems();
            }
            if (TMPSkillIconCountText)
            {
                TMPSkillIconCountText.text = AtavismPrefabManager.Instance.GetCountLoadedSkillIcons() + " / " + AtavismPrefabManager.Instance.GetCountSkills();
            }
            if (TMPCurrencyIconCountText)
            {
                TMPCurrencyIconCountText.text = AtavismPrefabManager.Instance.GetCountLoadedCurrencyIcons() + " / " + AtavismPrefabManager.Instance.GetCountCurrencies();
            }

            
            
        }
        public void ShowWeatherCommands()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            NetworkAPI.SendExtensionMessage(0, false, "weather.GET_WEATHER_PROFILE", props);
            if(teleportPanel) teleportPanel.gameObject.SetActive(false);
            if(gainCommandsPanel) gainCommandsPanel.gameObject.SetActive(false);
            if(weatherCommandsPanel)weatherCommandsPanel.gameObject.SetActive(true);
            if(serverCommandsPanel)serverCommandsPanel.gameObject.SetActive(false);

        }
        public void ShowServerCommands()
        { 
            int adminLevel = (int) ClientAPI.GetObjectProperty(ClientAPI.GetPlayerOid(), "adminLevel");
            if (adminLevel >= 5)
            {
                if (teleportPanel) teleportPanel.gameObject.SetActive(false);
                if (gainCommandsPanel) gainCommandsPanel.gameObject.SetActive(false);
                if (weatherCommandsPanel) weatherCommandsPanel.gameObject.SetActive(false);
                if (serverCommandsPanel) serverCommandsPanel.gameObject.SetActive(true);
                UpdateServerCommandProfileList();
            }
        }

        public void ShowSpawner()
        {
            //	Camera.main.GetComponentInChildren<MobCreator>().ToggleBuildingModeEnabled();
            UGUIMobCreator.Instance.ToggleBuildingModeEnabled();

        }

        
        void UpdateServerCommandProfileList()
        {
            if (TMPServerCommandProfile != null)
            {
                List<DsAdminRestart> _profiles = AtavismSettings.Instance.GetAdminRestarts;

                List<TMP_Dropdown.OptionData> _options = new List<TMP_Dropdown.OptionData>();
                TMPServerCommandProfile.ClearOptions();
                _options.Add(new TMP_Dropdown.OptionData("None"));
                foreach (DsAdminRestart l in _profiles)
                {
                    _options.Add(new TMP_Dropdown.OptionData(l.Name));
                }

                TMPServerCommandProfile.AddOptions(_options);
            }
        }

        public void SetCommandServerProfile(int id)
        {
           // Debug.LogError("SetCommandServerProfile: id:"+id);
            List<DsAdminRestart> _profiles = AtavismSettings.Instance.GetAdminRestarts;
            if (id > 0 )
            {
            //    Debug.LogError("SetCommandServerProfile: selected Profile " + _profiles[id - 1].Name);
                if (TMPServerMessageField != null)
                    TMPServerMessageField.text = _profiles[id - 1].Message;
                if (TMPServerScheduleField != null)
                    TMPServerScheduleField.text = _profiles[id - 1].Schedule;
                if (TMPServerCountdowanField != null)
                    TMPServerCountdowanField.text = _profiles[id - 1].CountdownTime.ToString();
            }
          //  else Debug.LogError("SetCommandServerProfile: selected Profile -> no profile");
            
        }
        
        public void SendShutdown()
        {
            int adminLevel = (int) ClientAPI.GetObjectProperty(ClientAPI.GetPlayerOid(), "adminLevel");
            if (adminLevel >= 5)
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("message", TMPServerMessageField.text);
                props.Add("time", TMPServerCountdowanField.text.Length > 0 ? int.Parse(TMPServerCountdowanField.text) : 10);
                props.Add("schedule", TMPServerScheduleField.text);
                props.Add("restart", TMPServerRestartToggle != null ? TMPServerRestartToggle.isOn : true);
                NetworkAPI.SendExtensionMessage(0, false, "server.Shutdown", props);
               
            }

        }


        IEnumerator checkReload()
        {
         //   Debug.LogError("checkReload "+AtavismPrefabManager.Instance.reloaded+" < "+AtavismPrefabManager.Instance.ToReload);
            while (AtavismPrefabManager.Instance.reloaded < AtavismPrefabManager.Instance.ToReload)
            {

                if (serverReloadProgressSlider != null)
                {
                    serverReloadProgressSlider.maxValue = AtavismPrefabManager.Instance.ToReload;
                    serverReloadProgressSlider.value = AtavismPrefabManager.Instance.reloaded;
                }

                yield return new WaitForSeconds(0.1f);

                if (serverReloadProgressSlider != null)
                {
                    serverReloadProgressSlider.maxValue = AtavismPrefabManager.Instance.ToReload;
                    serverReloadProgressSlider.value = AtavismPrefabManager.Instance.reloaded;
                }
          //      Debug.LogError("checkReload| "+AtavismPrefabManager.Instance.reloaded+" < "+AtavismPrefabManager.Instance.ToReload);

            }
            if (serverReloadProgressPanel)
                serverReloadProgressPanel.gameObject.SetActive(false);
            string[] args = new string[1];
            args[0] = "Definitions reloaded";
            AtavismEventSystem.DispatchEvent("ANNOUNCEMENT", args);
        }


        public void SendReload()
        {
            int adminLevel = (int) ClientAPI.GetObjectProperty(ClientAPI.GetPlayerOid(), "adminLevel");
            if (adminLevel >= 5)
            {
                if (AtavismPrefabManager.Instance.PrefabReloading)
                {
                    Dictionary<string, object> props = new Dictionary<string, object>();
                    NetworkAPI.SendExtensionMessage(0, false, "server.Reload", props);
                    AtavismPrefabManager.Instance.reloaded = 0;
                    //AtavismPrefabManager.Instance.refabReloading = true;
                    if (serverReloadProgressPanel)
                        serverReloadProgressPanel.gameObject.SetActive(true);
                    StartCoroutine(checkReload());
                }
            }
        }

        
        public void TeleportToPlayer()
        {
            if (teleportToPlayerField != null)
                NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/gotoplayer " + teleportToPlayerField.text);
            if (TMPTeleportToPlayerField != null)
                NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/gotoplayer " + TMPTeleportToPlayerField.text);
        }

        public void SummonPlayer()
        {
            if (summonPlayerField != null)
                NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/summon " + summonPlayerField.text);
            if (TMPSummonPlayerField != null)
                NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/summon " + TMPSummonPlayerField.text);
        }

        public void ChangeInstance()
        {
            if (changeInstanceField != null)
                NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/ci " + changeInstanceField.text);
            if (TMPChangeInstanceField != null)
                NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/ci " + TMPChangeInstanceField.text);
        }

        public void GotoPosition()
        {
            if (gotoXField != null && gotoYField != null && gotoZField != null)
                NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/gotopos " + gotoXField.text + " " + gotoYField.text + " " + gotoZField.text);
            if (TMPGotoXField != null && TMPGotoYField != null && TMPGotoZField != null)
                NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/gotopos " + TMPGotoXField.text + " " + TMPGotoYField.text + " " + TMPGotoZField.text);
        }

        public void GetExperience()
        {
            int expAmount = 0;
            if (expField != null)
                expAmount = int.Parse(expField.text);
            if (TMPExpField != null)
                expAmount = int.Parse(TMPExpField.text);
            NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/getExp " + expAmount);
        }

        public void ChooseWeatherProfile()
        {
            if (chooseType != ChooseType.Weather)
            {
                if (TMPFilterInputField)
                    TMPFilterInputField.text = "";
                if (filterInputField)
                    filterInputField.text = "";
            }

            if (choosePanel)
                choosePanel.gameObject.SetActive(true);
            chooseType = ChooseType.Weather;

            if (chooseTitle != null)
                chooseTitle.text = "Weather Profile";
            if (TMPChooseTitle != null)
                TMPChooseTitle.text = "Weather Profile";
            // Delete the old list
            ClearAllCells();
            Refresh();
        }

        public void ChooseCurrency()
        {
            if (chooseType != ChooseType.Currency)
            {
                if (TMPFilterInputField)
                    TMPFilterInputField.text = "";
                if (filterInputField)
                    filterInputField.text = "";
                
            }
            chooseType = ChooseType.Currency;
            choosePanel.gameObject.SetActive(true);
            if (chooseTitle != null)
                chooseTitle.text = "Choose Currency";
            if (TMPChooseTitle != null)
                TMPChooseTitle.text = "Choose Currency";
            // Delete the old list
            ClearAllCells();
            Refresh();
        }

        public void ChooseItem()
        {
            if (chooseType != ChooseType.Item)
            {
                if (TMPFilterInputField)
                    TMPFilterInputField.text = "";
                if (filterInputField)
                    filterInputField.text = "";
                
            }
            chooseType = ChooseType.Item;
            choosePanel.gameObject.SetActive(true);
            if (chooseTitle != null)
                chooseTitle.text = "Choose Item";
            if (TMPChooseTitle != null)
                TMPChooseTitle.text = "Choose Item";
            // Delete the old list
            ClearAllCells();
            Refresh();
        }

        public void ChooseSkill()
        {
            if (chooseType != ChooseType.Skill)
            {
                if (TMPFilterInputField)
                    TMPFilterInputField.text = "";
                if (filterInputField)
                    filterInputField.text = "";
                
            }
            chooseType = ChooseType.Skill;
            choosePanel.gameObject.SetActive(true);
            if (chooseTitle != null)
                chooseTitle.text = "Choose Skill";
            if (TMPChooseTitle != null)
                TMPChooseTitle.text = "Choose Skill";
            // Delete the old list
            ClearAllCells();
            Refresh();
        }

        public void UpdateChooseFilter(string filterText)
        {
            // Delete the old list
            ClearAllCells();
            Refresh();
        }

        public void CurrencySelected(int id)
        {
            currencyID = id;
            choosePanel.gameObject.SetActive(false);
            if (chooseCurrencyButton.GetComponentInChildren<Text>() != null)
                chooseCurrencyButton.GetComponentInChildren<Text>().text = currencyID.ToString();
            if (chooseCurrencyButton.GetComponentInChildren<TextMeshProUGUI>() != null)
                chooseCurrencyButton.GetComponentInChildren<TextMeshProUGUI>().text = currencyID.ToString();
        }

        public void ItemSelected(int id)
        {
            itemID = id;
            choosePanel.gameObject.SetActive(false);
            if (chooseItemButton.GetComponentInChildren<Text>() != null)
                chooseItemButton.GetComponentInChildren<Text>().text = itemID.ToString();
            if (chooseItemButton.GetComponentInChildren<TextMeshProUGUI>() != null)
                chooseItemButton.GetComponentInChildren<TextMeshProUGUI>().text = itemID.ToString();
        }

        public void SkillSelected(int id)
        {
            skillID = id;
            choosePanel.gameObject.SetActive(false);
            if (chooseSkillButton.GetComponentInChildren<Text>() != null)
                chooseSkillButton.GetComponentInChildren<Text>().text = skillID.ToString();
            if (chooseSkillButton.GetComponentInChildren<TextMeshProUGUI>() != null)
                chooseSkillButton.GetComponentInChildren<TextMeshProUGUI>().text = skillID.ToString();
        }

        public void WeatherProfileSelected(int id)
        {
            profileId = id;
            choosePanel.gameObject.SetActive(false);
            if (chooseWeatherButton.GetComponentInChildren<Text>() != null)
                chooseWeatherButton.GetComponentInChildren<Text>().text = AtavismWeatherManager.Instance.Profiles[profileId].name;
            if (chooseWeatherButton.GetComponentInChildren<TextMeshProUGUI>() != null)
                chooseWeatherButton.GetComponentInChildren<TextMeshProUGUI>().text = AtavismWeatherManager.Instance.Profiles[profileId].name;
        }

        public void ItemIconUpdate(int id)
        {
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("objs", id+";");
            AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "ItemIcon");
        }

        public void SkillIconUpdate(int id)
        {
          
        }

        public void GetItemIcons()
        {
            if (AtavismPrefabManager.Instance.GetCountItems() - AtavismPrefabManager.Instance.GetCountLoadedItemIcons() == 0)
                return;
            autoItemIconGet = !autoItemIconGet;
            if (autoItemIconGet)
            {
                int count = iconLoadPartCountField != null ? iconLoadPartCountField.text.Length > 0 ? int.Parse(iconLoadPartCountField.text) : -1 : -1;
                if(count==-1)
                    count = TMPIconLoadPartCountField != null ? TMPIconLoadPartCountField.text.Length > 0 ? int.Parse(TMPIconLoadPartCountField.text) : -1 : -1;

                if (count < 1)
                {
                    AtavismPrefabManager.Instance.LoadItemIcons();
                }
                else
                {
                    AtavismPrefabManager.Instance.LoadItemIcons(count);
                }

                if (itemIconLoadButton)
                {
                    if (itemIconLoadButton.GetComponentInChildren<Text>() != null)
                        itemIconLoadButton.GetComponentInChildren<Text>().text = "STOP";
                    if (itemIconLoadButton.GetComponentInChildren<TextMeshProUGUI>() != null)
                        itemIconLoadButton.GetComponentInChildren<TextMeshProUGUI>().text = "STOP";
                }
            }
            else
            {
                if (itemIconLoadButton)
                {
                    if (itemIconLoadButton.GetComponentInChildren<Text>() != null)
                        itemIconLoadButton.GetComponentInChildren<Text>().text = "LOAD";
                    if (itemIconLoadButton.GetComponentInChildren<TextMeshProUGUI>() != null)
                        itemIconLoadButton.GetComponentInChildren<TextMeshProUGUI>().text =  "LOAD";
                }
            }
            
        }
        public void GetSkillIcons()
        {
            if (AtavismPrefabManager.Instance.GetCountSkills() - AtavismPrefabManager.Instance.GetCountLoadedSkillIcons() == 0)
                return;
            autoSkillIconGet = !autoSkillIconGet;
            if (autoSkillIconGet)
            {
                int count = iconLoadPartCountField != null ? iconLoadPartCountField.text.Length > 0 ? int.Parse(iconLoadPartCountField.text) : -1 : -1;
                if(count==-1)
                    count = TMPIconLoadPartCountField != null ? TMPIconLoadPartCountField.text.Length > 0 ? int.Parse(TMPIconLoadPartCountField.text) : -1 : -1;

                if (count < 1)
                {
                    AtavismPrefabManager.Instance.LoadSkillIcons();
                }
                else
                {
                    AtavismPrefabManager.Instance.LoadSkillIcons(count);
                }

                if (skillIconLoadButton)
                {
                    if (skillIconLoadButton.GetComponentInChildren<Text>() != null)
                        skillIconLoadButton.GetComponentInChildren<Text>().text = "STOP";
                    if (skillIconLoadButton.GetComponentInChildren<TextMeshProUGUI>() != null)
                        skillIconLoadButton.GetComponentInChildren<TextMeshProUGUI>().text = "STOP";
                }
            }
            else
            {
                if (skillIconLoadButton)
                {
                    if (skillIconLoadButton.GetComponentInChildren<Text>() != null)
                        skillIconLoadButton.GetComponentInChildren<Text>().text = "LOAD";
                    if (skillIconLoadButton.GetComponentInChildren<TextMeshProUGUI>() != null)
                        skillIconLoadButton.GetComponentInChildren<TextMeshProUGUI>().text =  "LOAD";
                }
            }
            
        }
        public void GetCurrencytIcons()
        {
            if (AtavismPrefabManager.Instance.GetCountCurrencies() - AtavismPrefabManager.Instance.GetCountLoadedCurrencyIcons() == 0)
                return;
            autoCurrencyIconGet = !autoCurrencyIconGet;
            if (autoCurrencyIconGet)
            {
                int count = iconLoadPartCountField != null ? iconLoadPartCountField.text.Length > 0 ? int.Parse(iconLoadPartCountField.text) : -1 : -1;
                if(count==-1)
                    count = TMPIconLoadPartCountField != null ? TMPIconLoadPartCountField.text.Length > 0 ? int.Parse(TMPIconLoadPartCountField.text) : -1 : -1;

                if (count < 1)
                {
                    AtavismPrefabManager.Instance.LoadCurrencyIcons();
                }
                else
                {
                    AtavismPrefabManager.Instance.LoadCurrencyIcons(count);
                }

                if (currencyIconLoadButton)
                {
                    if (currencyIconLoadButton.GetComponentInChildren<Text>() != null)
                        currencyIconLoadButton.GetComponentInChildren<Text>().text = "STOP";
                    if (currencyIconLoadButton.GetComponentInChildren<TextMeshProUGUI>() != null)
                        currencyIconLoadButton.GetComponentInChildren<TextMeshProUGUI>().text = "STOP";
                }
            }
            else
            {
                if (currencyIconLoadButton)
                {
                    if (currencyIconLoadButton.GetComponentInChildren<Text>() != null)
                        currencyIconLoadButton.GetComponentInChildren<Text>().text = "LOAD";
                    if (currencyIconLoadButton.GetComponentInChildren<TextMeshProUGUI>() != null)
                        currencyIconLoadButton.GetComponentInChildren<TextMeshProUGUI>().text =  "LOAD";
                }
            }
            
        }

        
        public void GenerateItem()
        {
            int count = 0;
            if (itemCountField != null)
                count = int.Parse(itemCountField.text);
            if (TMPItemCountField != null)
                count = int.Parse(TMPItemCountField.text);
            NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/gi " + itemID + " " + count);
        }

        public void GetCurrency()
        {
            int count = 0;
            if (currencyCountField != null)
                count = int.Parse(currencyCountField.text);
            if (TMPCurrencyCountField != null)
                count = int.Parse(TMPCurrencyCountField.text);
            NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/getCurrency " + currencyID + " " + count);
        }

        public void GainSkill()
        {
            int count = 0;
            if (skillCountField != null)
                count = int.Parse(skillCountField.text);
            if (TMPSkillCountField != null)
                count = int.Parse(TMPSkillCountField.text);
            NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/getSkillCurrent " + skillID + " " + count);
        }

        void UpdateLocList()
        {
            DsAdminPanelSettings _loc = AtavismSettings.Instance.GetAdminLocations();
            if (_loc != null)
            {
                if (instanceLocs != null)
                {
                    List<Dropdown.OptionData> _options = new List<Dropdown.OptionData>();
                    instanceLocs.ClearOptions();
                    foreach (DsAdminLocation l in _loc.Loc)
                    {
                        _options.Add(new Dropdown.OptionData(l.Name));
                    }
                    instanceLocs.AddOptions(_options);
                }
                if (TMPInstanceLocs != null)
                {
                    List<TMP_Dropdown.OptionData> _options = new List<TMP_Dropdown.OptionData>();
                    TMPInstanceLocs.ClearOptions();
                    foreach (DsAdminLocation l in _loc.Loc)
                    {
                        _options.Add(new TMP_Dropdown.OptionData(l.Name));
                    }
                    TMPInstanceLocs.AddOptions(_options);
                }
            }
            else
            {
                Debug.LogWarning("No Admin Settings for instace " + actLoc);
            }
        }

      

        public void GoToInstanceLoc()
        {
            if (instanceLocs != null)
            {
                DsAdminPanelSettings _loc = AtavismSettings.Instance.GetAdminLocations();
                if (_loc != null)
                {
                    Vector3 l = _loc.Loc[instanceLocs.value].Loc;
                    NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/gotopos " + l.x + " " + l.y + " " + l.z);
                }
                else
                {
                    Debug.LogWarning("No Admin Settings for instace " + actLoc);
                }
            }
            if (TMPInstanceLocs != null)
            {
                DsAdminPanelSettings _loc = AtavismSettings.Instance.GetAdminLocations();
                if (_loc != null)
                {
                    Vector3 l = _loc.Loc[TMPInstanceLocs.value].Loc;
                    NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/gotopos " + l.x + " " + l.y + " " + l.z);
                }
                else
                {
                    Debug.LogWarning("No Admin Settings for instace " + actLoc);
                }
            }
        }

        public void SetWorldTime()
        {
            int year = 0;
            int month = 0;
            int day = 0;
            int hour = 0;
            int minute = 0;
            if (TMPYearField != null)
                year = int.Parse(TMPYearField.text);
            if (TMPMonthField != null)
                month = int.Parse(TMPMonthField.text);
            if (TMPDayField != null)
                day = int.Parse(TMPDayField.text);
            if (TMPHourField != null)
                hour = int.Parse(TMPHourField.text);
            if (TMPMinuteField != null)
                minute = int.Parse(TMPMinuteField.text);

            NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/setWorldTime " + year + " " + month + " " + day + " " + hour + " " + minute);
        }
        
        public void GetWorldTime()
        {

            if (TMPYearField != null)
                TMPYearField.text = AtavismWeatherManager.Instance.Year.ToString();
            if (TMPMonthField != null)
                TMPMonthField.text = AtavismWeatherManager.Instance.Month.ToString();
            if (TMPDayField != null)
                TMPDayField.text = AtavismWeatherManager.Instance.Day.ToString();
            if (TMPHourField != null)
                TMPHourField.text = AtavismWeatherManager.Instance.Hour.ToString();
            if (TMPMinuteField != null)
                TMPMinuteField.text = AtavismWeatherManager.Instance.Minute.ToString();

        }

        
        public void SetWeatherProfile()
        {
            NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/setWeatherProfile " + profileId );
        }

        #region implemented abstract members of AtList

        public override int NumberOfCells()
        {
            int count = 0;
            if (chooseType == ChooseType.Currency)
            {
                foreach (Currency currency in Inventory.Instance.Currencies.Values)
                {
                    if (filterInputField != null)
                        if (currency.name.ToLower().Contains(filterInputField.text.ToLower()))
                            count++;
                    if (TMPFilterInputField != null)
                        if (currency.name.ToLower().Contains(TMPFilterInputField.text.ToLower()))
                            count++;
                }
            }
            else if (chooseType == ChooseType.Item)
            {
                /*  foreach (AtavismInventoryItem item in Inventory.Instance.Items.Values)
                  {
                      if (filterInputField != null)
                          if (item.name.ToLower().Contains(filterInputField.text.ToLower()))
                              count++;
                      if (TMPFilterInputField != null)
                          if (item.name.ToLower().Contains(TMPFilterInputField.text.ToLower()))
                              count++;
                  }*/
                foreach (ItemPrefabData item in AtavismPrefabManager.Instance.GetItemPrefabData())
                {
                    if (filterInputField != null)
                        if (item.name.ToLower().Contains(filterInputField.text.ToLower()))
                            count++;
                    if (TMPFilterInputField != null)
                        if (item.name.ToLower().Contains(TMPFilterInputField.text.ToLower()))
                            count++;
                }

            }
            else if (chooseType == ChooseType.Skill)
            {
                foreach (Skill skill in Skills.Instance.SkillsList.Values)
                {
                    if (filterInputField != null)
                        if (skill.skillname.ToLower().Contains(filterInputField.text.ToLower()))
                            count++;
                    if (TMPFilterInputField != null)
                        if (skill.skillname.ToLower().Contains(TMPFilterInputField.text.ToLower()))
                            count++;
                }
            } else if (chooseType == ChooseType.Weather)
            {
                foreach (WeatherProfile profile in AtavismWeatherManager.Instance.Profiles.Values)
                {
                    if (filterInputField != null)
                        if (profile.name.ToLower().Contains(filterInputField.text.ToLower()))
                            count++;
                    if (TMPFilterInputField != null)
                        if (profile.name.ToLower().Contains(TMPFilterInputField.text.ToLower()))
                            count++;
                }
            }
            return count;
        }

        public override void UpdateCell(int index, UGUIAdminChooseEntry cell)
        {
            if (chooseType == ChooseType.Currency)
            {
                List<Currency> activeCurrencies = new List<Currency>();
                foreach (Currency currency in Inventory.Instance.Currencies.Values)
                {
                    if (filterInputField != null)
                        if (currency.name.ToLower().Contains(filterInputField.text.ToLower()))
                            activeCurrencies.Add(currency);
                    if (TMPFilterInputField != null)
                        if (currency.name.ToLower().Contains(TMPFilterInputField.text.ToLower()))
                            activeCurrencies.Add(currency);
                }
                Currency c = activeCurrencies[index];
                cell.SetEntryDetails(CurrencySelected, c.id, c.name, c.icon, SkillIconUpdate);
            }
            else if (chooseType == ChooseType.Item)
            {
              /*  List<AtavismInventoryItem> activeItems = new List<AtavismInventoryItem>();
                foreach (AtavismInventoryItem item in Inventory.Instance.Items.Values)
                {
                    if (filterInputField != null)
                        if (item.name.ToLower().Contains(filterInputField.text.ToLower()))
                            activeItems.Add(item);
                    if (TMPFilterInputField != null)
                        if (item.name.ToLower().Contains(TMPFilterInputField.text.ToLower()))
                            activeItems.Add(item);
                }
                AtavismInventoryItem selectedItem = activeItems[index];
                cell.SetEntryDetails(ItemSelected, selectedItem.templateId, selectedItem.name, selectedItem.icon);
                */
                List<ItemPrefabData> activeItems = new List<ItemPrefabData>();
                foreach (ItemPrefabData item in AtavismPrefabManager.Instance.GetItemPrefabData())
                {
                    if (filterInputField != null)
                        if (item.name.ToLower().Contains(filterInputField.text.ToLower()))
                            activeItems.Add(item);
                    if (TMPFilterInputField != null)
                        if (item.name.ToLower().Contains(TMPFilterInputField.text.ToLower()))
                            activeItems.Add(item);
                }
                ItemPrefabData selectedItem = activeItems[index];
                cell.SetEntryDetails(ItemSelected, selectedItem.templateId, selectedItem.name, selectedItem.icon, ItemIconUpdate);

            }
            else if (chooseType == ChooseType.Skill)
            {
                List<Skill> activeSkills = new List<Skill>();
                foreach (Skill skill in Skills.Instance.SkillsList.Values)
                {
                    if (filterInputField != null)
                        if (skill.skillname.ToLower().Contains(filterInputField.text.ToLower()))
                            activeSkills.Add(skill);
                    if (TMPFilterInputField != null)
                        if (skill.skillname.ToLower().Contains(TMPFilterInputField.text.ToLower()))
                            activeSkills.Add(skill);
                }
                Skill selectedSkill = activeSkills[index];
                cell.SetEntryDetails(SkillSelected, selectedSkill.id, selectedSkill.skillname, selectedSkill.icon, SkillIconUpdate);
            }
            else if (chooseType == ChooseType.Weather)
            {
                List<WeatherProfile> activeProfiles = new List<WeatherProfile>();
                foreach (WeatherProfile profile in AtavismWeatherManager.Instance.Profiles.Values)
                {
                    if (filterInputField != null)
                        if (profile.name.ToLower().Contains(filterInputField.text.ToLower()))
                            activeProfiles.Add(profile);
                    if (TMPFilterInputField != null)
                        if (profile.name.ToLower().Contains(TMPFilterInputField.text.ToLower()))
                            activeProfiles.Add(profile);
                }
                WeatherProfile selectedProfile = activeProfiles[index];
                cell.SetEntryDetails(WeatherProfileSelected, selectedProfile.id, selectedProfile.name, null, SkillIconUpdate);
                
             
            }
        }

        #endregion

        public static UGUIAdminPanel Instance
        {
            get
            {
                return instance;
            }
        }
    }
}