using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Atavism
{

    public class UGUIGuildPanel : AtList<UGUIGuildMemberEntry>, IPointerClickHandler
    {

        public UGUIPanelTitleBar titleBar;
        public Text guildMotd;
        public TextMeshProUGUI TMPGuildMotd;
        public RectTransform createPopup;
        public InputField guildNameField;
        public TMP_InputField TMPGuildNameField;

        public RectTransform invitePopup;
        public InputField inviteNameField;
        public TMP_InputField TMPInviteNameField;
        public RectTransform memberPopup;
        public RectTransform rankPopup;
        public Button settingsDropDown;
        public RectTransform settingsContainer;
        public List<Button> settingsButtons;
        public RectTransform rankPermissionsPanel;
        public Button rankDropDown;
        public RectTransform rankContainer;
        public List<Button> rankButtons;
        public Toggle guildChatListen;
        public Toggle guildChatSpeak;
        public Toggle inviteMember;
        public Toggle removeMember;
        public Toggle promote;
        public Toggle demote;
        public Toggle setMOTD;
        public Toggle editPublicNote;
        public Toggle claimAdd;
        public Toggle claimEdit;
        public Toggle claimAction;
        public Toggle levelUp;
        public Toggle warehouseAdd;
        public Toggle warehouseGet;

        public RectTransform guildRanksPanel;
        public List<UGUIGuildRank> guildRanks;
        public KeyCode toggleKey;
        public Dropdown settingsDropdown;
        public TMP_Dropdown TMPSettingsDropdown;
        public Dropdown rankModDropdown;
        public TMP_Dropdown TMPRankModDropdown;
        [SerializeField] Button disbandButton;
        [SerializeField] Button leaveButton;
        [SerializeField] Button addRankButton;
        public RectTransform guildSettingsPanel;
        [SerializeField] RectTransform editMOTD;
        [SerializeField] TMP_InputField editMOTDField;
        [SerializeField] Button editMOTDButton;

        public TextMeshProUGUI TMPLevel;
        public Slider levelProgressSlider;
        public RectTransform resourcesWindow;
        public List<UGUIItemDisplay> resourceRequired;
        public List<UGUIItemDisplay> resourceCollected;
        public UGUIItemDisplay resourceCountItemDisplay;
        public Transform resourceCountPanel;
        public TMP_InputField TMPCountText;
        public TextMeshProUGUI TMPItemName;
        // public Button minusButton;
        AtavismInventoryItem resourceItem;
        public Button levelUpButton;
        int resourceItemCount = 1;

        public TextMeshProUGUI memberNumText;
        bool showing = false;
        AtavismGuildMember selectedMember = null;
        bool settingsDropDownOpen = false;
        bool rankDropDownOpen = false;
        int selectedRank = 0;
        [SerializeField] GameObject panel;
        float interactionDelay;
        bool create=false;
        // Use this for initialization
        void Awake()
        {
            if (titleBar != null)
                titleBar.SetOnPanelClose(Hide);
            Hide();
            toggleKey = AtavismSettings.Instance.GetKeySettings().guild;

            AtavismEventSystem.RegisterEvent("GUILD_UPDATE", this);
            AtavismEventSystem.RegisterEvent("GUILD_RES_UPDATE", this);

            for (int i = 0; i < rankButtons.Count; i++)
            {
                rankButtons[i].gameObject.SetActive(true);
            }
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("GUILD_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("GUILD_RES_UPDATE", this);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(toggleKey) && !ClientAPI.UIHasFocus())
            {
                Toggle();
            }

            if (rankDropDown != null && rankPopup.gameObject.activeSelf)
            {
                Vector3 dropdownScale = settingsContainer.localScale;
                dropdownScale.y = Mathf.Lerp(dropdownScale.y, settingsDropDownOpen ? 1 : 0, Time.deltaTime * 12);
                settingsContainer.localScale = dropdownScale;

                dropdownScale = rankContainer.localScale;
                dropdownScale.y = Mathf.Lerp(dropdownScale.y, rankDropDownOpen ? 1 : 0, Time.deltaTime * 12);
                rankContainer.localScale = dropdownScale;
            }
        }

        public void Show()
        {
            AtavismUIUtility.BringToFront(gameObject);

            if (AtavismGuild.Instance.GuildName == null || AtavismGuild.Instance.GuildName == "")
            {
                createPopup.gameObject.SetActive(true);
                AtavismUIUtility.BringToFront(createPopup.gameObject);
                createPopup.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
                createPopup.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
                createPopup.gameObject.GetComponent<CanvasGroup>().interactable = true;
                if (titleBar != null)
                    titleBar.titleText.text = "";
                if (guildMotd != null)
                    guildMotd.text = "";
                if (TMPGuildMotd != null)
                    TMPGuildMotd.text = "";
                ClearAllCells();
                showing = false;
                create = true;
            }
            else
            {
                UpdateGuildDetails();
                GetComponent<CanvasGroup>().alpha = 1f;
                GetComponent<CanvasGroup>().blocksRaycasts = true;
                GetComponent<CanvasGroup>().interactable = true;

                //      invitePopup.gameObject.GetComponent<UIWindow>().Hide();
                invitePopup.gameObject.SetActive(false);
                memberPopup.gameObject.SetActive(false);
                if (!showing)
                {
                    rankPopup.gameObject.SetActive(false);
                    guildRanksPanel.gameObject.SetActive(false);
                    rankPermissionsPanel.gameObject.SetActive(false);
                    guildSettingsPanel.gameObject.SetActive(false);
                }
                rankDropDownOpen = false;
                showing = true;
                create = false;
            }
            if (panel != null)
                panel.SetActive(true);
            //   AtavismUIUtility.BringToFront(gameObject);
        }

        public void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            showing = false;
            if (createPopup != null)
            {
                createPopup.gameObject.SetActive(false);
                createPopup.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
                createPopup.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
            if (rankPopup != null)
                rankPopup.gameObject.SetActive(false);
            if (guildRanksPanel != null)
                guildRanksPanel.gameObject.SetActive(false);
            if (rankPermissionsPanel != null)
                rankPermissionsPanel.gameObject.SetActive(false);
            if (guildSettingsPanel != null)
                guildSettingsPanel.gameObject.SetActive(false);
            if (invitePopup.gameObject.activeSelf)
                invitePopup.gameObject.SetActive(false);
            if (panel != null)
                panel.SetActive(false);
           /* if (resourcesWindow != null)
                resourcesWindow.gameObject.SetActive(false);
            if (resourceCountPanel != null)
                resourceCountPanel.gameObject.SetActive(false);*/
            HideEditMOTDPopup();
            CloseResources();
        }

        public void HideCreatePanel()
        {
            if (createPopup != null)
            {
                createPopup.gameObject.SetActive(false);
                createPopup.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
                createPopup.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }


        public void Toggle()
        {
            if (showing )
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        public void OnEvent(AtavismEventData eData)
        {
           // Debug.LogError("OnEvent " + eData.eventType);
            if (eData.eventType == "GUILD_UPDATE")
            {
                if (showing || create)
                {
                    UpdateGuildDetails();

                    Show();
                }
            }
            else if (eData.eventType == "GUILD_RES_UPDATE")
            {
                UpdateShowResources();
            }

        }

        public void UpdateGuildDetails()
        {
            int memberRank = -1;
            AtavismGuildMember member = AtavismGuild.Instance.GetGuildMemberByOid(OID.fromLong(ClientAPI.GetPlayerOid()));
            if (member != null)
                memberRank = member.rank;

            createPopup.gameObject.SetActive(false);
            if (AtavismGuild.Instance.GuildName == null || AtavismGuild.Instance.GuildName == "")
            {
                Hide();
                return;
            }
            if (titleBar != null)
                titleBar.titleText.text = AtavismGuild.Instance.GuildName;
            if (guildMotd != null)
                guildMotd.text = AtavismGuild.Instance.Motd;
            if (TMPGuildMotd != null)
                TMPGuildMotd.text = AtavismGuild.Instance.Motd;

            if (TMPLevel != null)
                TMPLevel.text = AtavismGuild.Instance.Level;

            if (levelProgressSlider != null)
            {
                int req = 0;
                foreach (int v in AtavismGuild.Instance.RequiredItems.Values)
                    req += v;
                int count = 0;
                foreach (int v in AtavismGuild.Instance.Items.Values)
                    count += v;

                levelProgressSlider.maxValue = req;
                levelProgressSlider.minValue = 0;
                levelProgressSlider.value = count;
            }


            // Delete the old list
            ClearAllCells();

            Refresh();


            if (memberNumText != null)
            {
                if (AtavismGuild.Instance.MemberNum > 0)
                    memberNumText.text = AtavismGuild.Instance.Members.Count + "/" + AtavismGuild.Instance.MemberNum;
                else
                    memberNumText.text = AtavismGuild.Instance.Members.Count.ToString();
            }

            if (rankModDropdown != null)
                rankModDropdown.ClearOptions();
            if (TMPRankModDropdown != null)
                TMPRankModDropdown.ClearOptions();
            // Reset ranks
            for (int i = 0; i < rankButtons.Count; i++)
            {
                if (i >= AtavismGuild.Instance.Ranks.Count)
                {
                    if (rankButtons[i] != null)
                        rankButtons[i].gameObject.SetActive(false);
                }
                else
                {
                    if (rankButtons[i] != null)
                        rankButtons[i].gameObject.SetActive(true);
                    if (rankButtons[i] != null)
                        rankButtons[i].GetComponentInChildren<Text>().text = AtavismGuild.Instance.Ranks[i].rankName;
                    if (rankModDropdown != null)
                        rankModDropdown.options.Add(new Dropdown.OptionData(AtavismGuild.Instance.Ranks[i].rankName));
                    if (TMPRankModDropdown != null)
                        TMPRankModDropdown.options.Add(new TMP_Dropdown.OptionData(AtavismGuild.Instance.Ranks[i].rankName));
                }
            }
            if (rankModDropdown != null)
            {
                for (int i = 0; i < AtavismGuild.Instance.Ranks.Count; i++)
                {
                    rankModDropdown.options.Add(new Dropdown.OptionData(AtavismGuild.Instance.Ranks[i].rankName));
                }
                if (rankModDropdown.options.Count > selectedRank)
                    rankModDropdown.captionText.text = AtavismGuild.Instance.Ranks[selectedRank].rankName;
                else if (rankModDropdown.options.Count > 0)
                    rankModDropdown.captionText.text = AtavismGuild.Instance.Ranks[0].rankName;

                if (rankModDropdown.options.Count > selectedRank)
                    rankModDropdown.value = selectedRank;
                else
                    rankModDropdown.value = 0;

            }
            if (TMPRankModDropdown != null)
            {
                for (int i = 0; i < AtavismGuild.Instance.Ranks.Count; i++)
                {
                    TMPRankModDropdown.options.Add(new TMP_Dropdown.OptionData(AtavismGuild.Instance.Ranks[i].rankName));
                }
                if (TMPRankModDropdown.options.Count > selectedRank)
                    TMPRankModDropdown.captionText.text = AtavismGuild.Instance.Ranks[selectedRank].rankName;
                else if (TMPRankModDropdown.options.Count > 0)
                    TMPRankModDropdown.captionText.text = AtavismGuild.Instance.Ranks[0].rankName;

                if (TMPRankModDropdown.options.Count > selectedRank)
                    TMPRankModDropdown.value = selectedRank;
                else
                    TMPRankModDropdown.value = 0;

            }

            if (memberRank == 0 && selectedRank != 0)
            {
                disbandButton.interactable = true;
                if (leaveButton != null)
                    leaveButton.interactable = false;
                addRankButton.interactable = true;
                levelUp.interactable = true;
                warehouseAdd.interactable = true;
                warehouseGet.interactable = true;
                guildChatListen.interactable = true;
                guildChatSpeak.interactable = true;
                inviteMember.interactable = true;
                removeMember.interactable = true;
                promote.interactable = true;
                demote.interactable = true;
                setMOTD.interactable = true;
                editPublicNote.interactable = true;
                claimAdd.interactable = true;
                claimEdit.interactable = true;
                claimAction.interactable = true;
                editMOTDButton.interactable = true;

            }
            else
            {
                disbandButton.interactable = false;
                if (leaveButton != null)
                    leaveButton.interactable = true;
                addRankButton.interactable = false;
                levelUp.interactable = false;
                warehouseAdd.interactable = false;
                warehouseGet.interactable = false;
                guildChatListen.interactable = false;
                guildChatSpeak.interactable = false;
                inviteMember.interactable = false;
                removeMember.interactable = false;
                promote.interactable = false;
                demote.interactable = false;
                setMOTD.interactable = false;
                editPublicNote.interactable = false;
                claimAdd.interactable = false;
                claimEdit.interactable = false;
                claimAction.interactable = false;
                editMOTDButton.interactable = false;

            }
            if (memberRank == 0)
            {
                disbandButton.interactable = true;
                if (leaveButton != null)
                    leaveButton.interactable = false;
            }
            AtavismGuildRank rank = AtavismGuild.Instance.Ranks[memberRank];

            if (rank.permissions.Contains(AtavismGuildRankPermission.setmotd))
            {
                editMOTDButton.interactable = true;
            }
            for (int i = 0; i < guildRanks.Count; i++)
            {
                if (i >= AtavismGuild.Instance.Ranks.Count)
                {
                    guildRanks[i].gameObject.SetActive(false);
                }
                else
                {
                    guildRanks[i].gameObject.SetActive(true);
                    if (i == 0)
                    {

                    }
                    if (guildRanks[i].input != null)
                        guildRanks[i].input.text = AtavismGuild.Instance.Ranks[i].rankName;
                    if (guildRanks[i].inputTMP != null)
                        guildRanks[i].inputTMP.text = AtavismGuild.Instance.Ranks[i].rankName;
                    guildRanks[i].setRankId = AtavismGuild.Instance.Ranks[i].rankLevel;
#if AT_I2LOC_PRESET
                 if (guildRanks[i].rankText != null) guildRanks[i].rankText.text = I2.Loc.LocalizationManager.GetTranslation("Rank") + " " + (i + 1) + ":";
                 if (guildRanks[i].rankTextTMP != null) guildRanks[i].rankTextTMP.text = I2.Loc.LocalizationManager.GetTranslation("Rank") + " " + (i + 1) + ":";
#else
                    if (guildRanks[i].rankText != null)
                        guildRanks[i].rankText.text = "Rank " + (i + 1) + ":";
                    if (guildRanks[i].rankTextTMP != null)
                        guildRanks[i].rankTextTMP.text = "Rank " + (i + 1) + ":";
#endif
                    if (memberRank == 0 && i > AtavismGuild.Instance.Ranks.Count - 2 )
                                        {
                                            if (guildRanks[i].deleteButton != null)
                                                guildRanks[i].deleteButton.interactable = true;
                                            if (guildRanks[i].input != null)
                                                guildRanks[i].input.interactable = true;
                                            if (guildRanks[i].inputTMP != null)
                                                guildRanks[i].inputTMP.interactable = true;
                                        }
                                        else if (memberRank == 0 && i > 0 )
                                                                {
                                                                    if (guildRanks[i].deleteButton != null)
                                                                        guildRanks[i].deleteButton.interactable = false;
                                                                    if (guildRanks[i].input != null)
                                                                        guildRanks[i].input.interactable = true;
                                                                    if (guildRanks[i].inputTMP != null)
                                                                        guildRanks[i].inputTMP.interactable = true;
                                                                }
                                                                else
                    {
                        if (guildRanks[i].deleteButton != null)
                            guildRanks[i].deleteButton.interactable = false;
                        if (guildRanks[i].input != null)
                            guildRanks[i].input.interactable = false;
                        if (guildRanks[i].inputTMP != null)
                            guildRanks[i].inputTMP.interactable = false;
                    }
                
                }
            }
            if (guildRanks.Count > AtavismGuild.Instance.Ranks.Count)
                addRankButton.interactable = true;
            else
                addRankButton.interactable = false;

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            HideMemberPopup();
        }

        public void CreateGuildClicked()
        {
            if (guildNameField != null)
                if (guildNameField.text != "")
                {
                    AtavismGuild.Instance.CreateGuild(guildNameField.text);
                    createPopup.gameObject.SetActive(false);
                    guildNameField.text = "";
                }
                else
                {

                }
            if (TMPGuildNameField != null)
                if (TMPGuildNameField.text != "")
                {
                    AtavismGuild.Instance.CreateGuild(TMPGuildNameField.text);
                    createPopup.gameObject.SetActive(false);
                    TMPGuildNameField.text = "";
                }
        }

        public void AddMemberClicked()
        {
            HideMemberPopup();
            AtavismUIUtility.BringToFront(invitePopup.gameObject);
            if (ClientAPI.GetTargetOid() > 0 && ClientAPI.GetTargetObject().CheckBooleanProperty("combat.userflag"))
            {
                AtavismGuild.Instance.SendGuildCommand("invite", OID.fromLong(ClientAPI.GetTargetOid()), null);
                return;
            }
            else
            {
                invitePopup.gameObject.SetActive(true);
                if (inviteNameField != null)
                {
                    inviteNameField.text = "";
                    EventSystem.current.SetSelectedGameObject(inviteNameField.gameObject, null);
                }
                if (TMPInviteNameField != null)
                {
                    TMPInviteNameField.text = "";
                    EventSystem.current.SetSelectedGameObject(TMPInviteNameField.gameObject, null);
                }
            }
        }

        public void AddMemberMenuClicked()
        {
            HideMemberPopup();
            if (ClientAPI.GetTargetOid() > 0 && ClientAPI.GetTargetObject().CheckBooleanProperty("combat.userflag"))
            {
                AtavismGuild.Instance.SendGuildCommand("invite", OID.fromLong(ClientAPI.GetTargetOid()), null);
                return;
            }
            else
            {
                AtavismUIUtility.BringToFront(invitePopup.gameObject);
                invitePopup.gameObject.SetActive(true);
                invitePopup.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                if (inviteNameField != null)
                {
                    inviteNameField.text = "";
                    EventSystem.current.SetSelectedGameObject(inviteNameField.gameObject, null);
                }
                if (TMPInviteNameField != null)
                {
                    TMPInviteNameField.text = "";
                    EventSystem.current.SetSelectedGameObject(TMPInviteNameField.gameObject, null);
                }
            }
        }

        public void AddMemberByNameClicked()
        {
            if (inviteNameField != null)
                if (inviteNameField.text != "")
                {
                    AtavismGuild.Instance.SendGuildCommand("invite", null, inviteNameField.text);
                    invitePopup.gameObject.SetActive(false);
                }
            if (TMPInviteNameField != null)
                if (TMPInviteNameField.text != "")
                {
                    AtavismGuild.Instance.SendGuildCommand("invite", null, TMPInviteNameField.text);
                    invitePopup.gameObject.SetActive(false);
                }
        }

        public void ShowResources()
        {
            if (resourcesWindow != null)
            {
                resourcesWindow.gameObject.SetActive(true);
            }
            if (AtavismCursor.Instance != null)
                AtavismCursor.Instance.SetUGUIActivatableClickedOverride(PlaceItem);
            UpdateShowResources();

        }

        public void CloseResources()
        {
            if (resourcesWindow != null)
            {
                resourcesWindow.gameObject.SetActive(false);
            }
            if (resourceCountPanel != null)
                resourceCountPanel.gameObject.SetActive(false);
            if (AtavismCursor.Instance != null)
                AtavismCursor.Instance.ClearUGUIActivatableClickedOverride(PlaceItem);
        }


        public bool ShowResourceCountPanel(AtavismInventoryItem resourceItem)
        {
            this.resourceItem = resourceItem;
            if (resourceCountItemDisplay != null)
                resourceCountItemDisplay.SetItemData(resourceItem, null);
            if (resourceCountPanel != null)
            {
                resourceCountPanel.gameObject.SetActive(true);
                resourceItemCount = 1;
                if (TMPCountText != null)
                {
                    TMPCountText.text = resourceItemCount.ToString();
                }
                if (TMPItemName != null && resourceItem != null)
                {
#if AT_I2LOC_PRESET
                    TMPItemName.text = string.IsNullOrEmpty(I2.Loc.LocalizationManager.GetTranslation("Items/" + resourceItem.name)) ? resourceItem.name : I2.Loc.LocalizationManager.GetTranslation("Items/" + resourceItem.name);
#else
                    TMPItemName.text = resourceItem.name;
#endif
                }
                return true;
            }
            return false;
        }

        public void ReduceMultipleCount()
        {
            resourceItemCount--;
            if (resourceItemCount < 2)
            {
                resourceItemCount = 1;
            }
            if (TMPCountText != null)
            {
                TMPCountText.text = resourceItemCount.ToString();
            }
        }

        public void IncreaseMultipleCount()
        {
            resourceItemCount++;
            int count = Inventory.Instance.GetCountOfItem(resourceItem.templateId);
            if (resourceItemCount > count)
                resourceItemCount = count;
            int reqCount = AtavismGuild.Instance.RequiredItems[resourceItem.templateId];
            int itemCount = 0;
            if (AtavismGuild.Instance.Items.ContainsKey(resourceItem.templateId))
                itemCount = AtavismGuild.Instance.Items[resourceItem.templateId];
            if (resourceItemCount > reqCount - itemCount)
                resourceItemCount = reqCount - itemCount;

            if (TMPCountText != null)
            {
                TMPCountText.text = resourceItemCount.ToString();
            }

        }

        public void UpdateResourceCount()
        {
            if (TMPCountText != null)
            {
                if (TMPCountText.text != "")
                    resourceItemCount = int.Parse(TMPCountText.text);
                else
                    resourceItemCount = 0;
            }
        }


        public void UpdateShowResources()
        {
            if (resourceRequired != null)
            {
                int req = 0;
                foreach (int key in AtavismGuild.Instance.RequiredItems.Keys)
                {
                    //   Debug.LogError("Guild Req item " + key + " " + AtavismGuild.Instance.RequiredItems[key]);
                    if (key > 0)
                    {
                        AtavismInventoryItem item = AtavismPrefabManager.Instance.LoadItem(key);
                        item.Count = AtavismGuild.Instance.RequiredItems[key];
                        if (resourceRequired.Count > req)
                        {
                            if (resourceRequired[req] != null)
                            {
                                resourceRequired[req].gameObject.SetActive(true);
                                resourceRequired[req].SetItemData(item, null);
                            }
                        }
                        req++;
                    }
                }
                if (resourceRequired.Count > req)
                {
                    for (int i = req; i < resourceRequired.Count; i++)
                    {
                        //  Debug.LogError("Guild Req item reset slot " + i );
                        if (resourceRequired[i] != null)
                        {
                            resourceRequired[i].Reset();
                            resourceRequired[i].gameObject.SetActive(false);
                        }
                    }
                }
            }

            if (resourceCollected != null)
            {
                int req = 0;
                foreach (int key in AtavismGuild.Instance.Items.Keys)
                {
                    //  Debug.LogError("Guild item " + key + " " + AtavismGuild.Instance.Items[key]);
                    if (key > 0)
                    {
                        AtavismInventoryItem item = AtavismPrefabManager.Instance.LoadItem(key);
                        item.Count = AtavismGuild.Instance.Items[key];
                        if (resourceCollected.Count > req)
                        {
                            if (resourceCollected[req] != null)
                            {
                                resourceCollected[req].gameObject.SetActive(true);
                                resourceCollected[req].SetItemData(item, null);
                            }
                        }
                        req++;
                    }
                }
                if (resourceCollected.Count > req)
                {
                    for (int i = req; i < resourceCollected.Count; i++)
                    {
                        if (resourceCollected[i] != null)
                        {
                            resourceCollected[i].Reset();
                            resourceCollected[i].gameObject.SetActive(false);
                        }
                    }
                }
            }
            if (levelUpButton != null)
            {
                bool allresources = false;
                foreach (int key in AtavismGuild.Instance.RequiredItems.Keys)
                {
                    if (AtavismGuild.Instance.Items.ContainsKey(key))
                    {
                        if (AtavismGuild.Instance.RequiredItems[key] == AtavismGuild.Instance.Items[key])
                        {
                            allresources = true;
                        }
                        else
                        {
                            allresources = false;
                            break;
                        }
                    }
                    else
                    {
                        allresources = false;
                        break;
                    }
                }
                if (allresources)
                {
                    int memberRank = -1;
                    AtavismGuildMember member = AtavismGuild.Instance.GetGuildMemberByOid(OID.fromLong(ClientAPI.GetPlayerOid()));
                    if (member != null)
                        memberRank = member.rank;
                    AtavismGuildRank rank = AtavismGuild.Instance.Ranks[memberRank];

                    if (rank.permissions.Contains(AtavismGuildRankPermission.levelUp))
                        levelUpButton.interactable = true;
                    else
                        levelUpButton.interactable = false;
                }
                else
                {
                    levelUpButton.interactable = false;
                }
            }
            if (TMPLevel != null)
                TMPLevel.text = AtavismGuild.Instance.Level;
            if (levelProgressSlider != null)
            {
                int req = 0;
                foreach (int v in AtavismGuild.Instance.RequiredItems.Values)
                    req += v;
                int count = 0;
                foreach (int v in AtavismGuild.Instance.Items.Values)
                    count += v;

                levelProgressSlider.maxValue = req;
                levelProgressSlider.minValue = 0;
                levelProgressSlider.value = count;
            }
        }



        public override int NumberOfCells()
        {
            int numCells = AtavismGuild.Instance.Members.Count;
            return numCells;
        }

        public override void UpdateCell(int index, UGUIGuildMemberEntry cell)
        {
            cell.SetGuildMemberDetails(AtavismGuild.Instance.Members[index], this);
        }

        #region Member Popup
        public void ShowMemberPopup(UGUIGuildMemberEntry selectedMemberEntry, AtavismGuildMember member)
        {
            selectedMember = member;
            memberPopup.gameObject.SetActive(true);
            Vector3 popupPosition = Input.mousePosition;
            // Add a button width/height to the tooltip position
            //RectTransform memberEntryTransform = selectedMemberEntry.GetComponent<RectTransform>();
            popupPosition += new Vector3(memberPopup.sizeDelta.x / 2, -memberPopup.sizeDelta.y / 2, 0);
            memberPopup.position = popupPosition; //new Vector2(popupPosition.x, memberEntryTransform.anchoredPosition.y);
        }

        public void HideMemberPopup()
        {
            memberPopup.gameObject.SetActive(false);
        }
        public void HideInvitePopup()
        {
            invitePopup.gameObject.SetActive(false);
        }

        public void WhisperMemberClicked()
        {
        }

        public void PromoteMemberClicked()
        {
            if (selectedMember.rank == 1)
            {
#if AT_I2LOC_PRESET
       UGUIConfirmationPanel.Instance.ShowConfirmationBox(I2.Loc.LocalizationManager.GetTranslation("Are you sure you want promote") +" "+selectedMember.name+" "+ I2.Loc.LocalizationManager.GetTranslation("to Guild Master") +"?", null, masterGuild);
#else
                UGUIConfirmationPanel.Instance.ShowConfirmationBox("Are you sure you want promote " + selectedMember.name + " to Guild Master?", null, masterGuild);
#endif
            }
            else
            {
                AtavismGuild.Instance.SendGuildCommand("promote", selectedMember.oid, null);
                HideMemberPopup();
            }
        }

        private void masterGuild(object confirmObject, bool accepted)
        {
            if (accepted)
            {
                AtavismGuild.Instance.SendGuildCommand("promote", selectedMember.oid, null);
                HideMemberPopup();
            }
        }


        public void DemoteMemberClicked()
        {
            AtavismGuild.Instance.SendGuildCommand("demote", selectedMember.oid, null);
            HideMemberPopup();
        }

        public void KickMemberClicked()
        {

#if AT_I2LOC_PRESET
       UGUIConfirmationPanel.Instance.ShowConfirmationBox(I2.Loc.LocalizationManager.GetTranslation("Are you sure you want kick") +" "+selectedMember.name+" "+ I2.Loc.LocalizationManager.GetTranslation("from Guild") +"?", null, kickMember);
#else
            UGUIConfirmationPanel.Instance.ShowConfirmationBox("Are you sure you want kick " + selectedMember.name + " from Guild ?", null, kickMember);
#endif
            HideMemberPopup();
        }

        private void kickMember(object confirmObject, bool accepted)
        {
            if (accepted)
            {
                AtavismGuild.Instance.SendGuildCommand("kick", selectedMember.oid, null);

            }
        }


        public void DisbandClicked()
        {
#if AT_I2LOC_PRESET
       UGUIConfirmationPanel.Instance.ShowConfirmationBox(I2.Loc.LocalizationManager.GetTranslation("Are you sure you want disband guild") + "?", null, disbandGuild);
#else
            UGUIConfirmationPanel.Instance.ShowConfirmationBox("Are you sure you want disband guild?", null, disbandGuild);
#endif
        }

        private void disbandGuild(object confirmObject, bool accepted)
        {
            if (accepted)
            {
                AtavismGuild.Instance.SendGuildCommand("disband", null, null);
                Hide();
            }
        }
        public void LeaveClicked()
        {
#if AT_I2LOC_PRESET
       UGUIConfirmationPanel.Instance.ShowConfirmationBox(I2.Loc.LocalizationManager.GetTranslation("Are you sure you want leave guild") + "?", null, leaveGuild);
#else
            UGUIConfirmationPanel.Instance.ShowConfirmationBox("Are you sure you want leave guild?", null, leaveGuild);
#endif
        }

        private void leaveGuild(object confirmObject, bool accepted)
        {
            if (accepted)
            {
                AtavismGuild.Instance.SendGuildCommand("quit", null, null);
                Hide();
            }
        }
        #endregion Member Popup



        public void SendResource()
        {
            ////    if (accepted)
            // {
            if (resourceItem != null)
                AtavismGuild.Instance.SendGuildResource(resourceItem.templateId, resourceItemCount);
            if (resourceCountPanel != null)
                resourceCountPanel.gameObject.SetActive(false);
            // }
        }

        public void CancelResource()
        {
            resourceItem = null;
            resourceItemCount = 0;
            if (resourceCountPanel != null)
                resourceCountPanel.gameObject.SetActive(false);

        }

        private void PlaceItem(UGUIAtavismActivatable activatable)
        {
            //  Debug.LogError("PlaceSocketingItem " + activatable.Link);

            if (activatable.Link != null)
            {
                return;
            }
            AtavismInventoryItem item = (AtavismInventoryItem)activatable.ActivatableObject;
            if (item != null)
            {
                if (AtavismGuild.Instance.RequiredItems.ContainsKey(item.templateId))
                {
                    int reqCount = AtavismGuild.Instance.RequiredItems[item.templateId];
                    int itemCount = 0;
                    if (AtavismGuild.Instance.Items.ContainsKey(item.templateId))
                        itemCount = AtavismGuild.Instance.Items[item.templateId];
                    if (reqCount - itemCount > 0)
                    {
                        ShowResourceCountPanel(item);
                    }
                    else
                    {
                        activatable.PreventDiscard();
                        //     Debug.LogError("Wrong Item");
                        string[] args = new string[1];
#if AT_I2LOC_PRESET
                    args[0] = I2.Loc.LocalizationManager.GetTranslation("Items/"+item.name)+" "+I2.Loc.LocalizationManager.GetTranslation("is no longer required");
#else
                        args[0] = item.name+" is no longer required";
#endif
                        AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
                    }

                }
                else
                {
                    activatable.PreventDiscard();

                    //     Debug.LogError("Wrong Item");
                    string[] args = new string[1];
#if AT_I2LOC_PRESET
                    args[0] = I2.Loc.LocalizationManager.GetTranslation("Wrong Item");
#else
                    args[0] = "Wrong Item";
#endif
                    AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
                }

            }
        }



        #region Rank Popup
        public void ToggleRankPopup()
        {
            if (rankPopup.gameObject.activeSelf)
            {
                rankPopup.gameObject.SetActive(false);
            }
            else
            {
                rankPopup.gameObject.SetActive(true);
                if (settingsDropdown)
                    settingsDropdown.value = 2;
                if (TMPSettingsDropdown)
                    TMPSettingsDropdown.value = 2;
                if (rankDropDown)
                    rankDropDown.gameObject.GetComponentInChildren<Text>().text = AtavismGuild.Instance.Ranks[selectedRank].rankName;
                UpdateRankDisplay();
            }
        }

        void UpdateRankDisplay()
        {
            interactionDelay = Time.time + 0.5f;
            int memberRank = -1;
            AtavismGuildMember member = AtavismGuild.Instance.GetGuildMemberByOid(OID.fromLong(ClientAPI.GetPlayerOid()));
            if (member != null)
                memberRank = member.rank;

            levelUp.isOn = false;
            warehouseAdd.isOn = false;
            warehouseGet.isOn = false;
            guildChatListen.isOn = false;
            guildChatSpeak.isOn = false;
            inviteMember.isOn = false;
            removeMember.isOn = false;
            promote.isOn = false;
            demote.isOn = false;
            setMOTD.isOn = false;
            editPublicNote.isOn = false;
            claimAdd.isOn = false;
            claimEdit.isOn = false;
            claimAction.isOn = false;
            claimAction.interactable = false;

            AtavismGuildRank rank = AtavismGuild.Instance.Ranks[selectedRank];
         /*   string r = "";
            foreach (AtavismGuildRankPermission a in rank.permissions)
            {
                r += a + " | ";
            }

            Debug.LogError("GUILD rank perm="+r);*/
            if (rank.permissions.Contains(AtavismGuildRankPermission.chat))
            {
                guildChatListen.isOn = true;
            }
            if (rank.permissions.Contains(AtavismGuildRankPermission.chat))
            {
                guildChatSpeak.isOn = true;
            }
            if (rank.permissions.Contains(AtavismGuildRankPermission.invite))
            {
                inviteMember.isOn = true;
            }
            if (rank.permissions.Contains(AtavismGuildRankPermission.kick))
            {
                removeMember.isOn = true;
            }
            if (rank.permissions.Contains(AtavismGuildRankPermission.promote))
            {
                promote.isOn = true;
            }
            if (rank.permissions.Contains(AtavismGuildRankPermission.demote))
            {
                demote.isOn = true;
            }
            if (rank.permissions.Contains(AtavismGuildRankPermission.setmotd))
            {
                setMOTD.isOn = true;
            }
            if (rank.permissions.Contains(AtavismGuildRankPermission.editPublic))
            {
                editPublicNote.isOn = true;
            }
            if (rank.permissions.Contains(AtavismGuildRankPermission.claimAdd))
            {
                claimAdd.isOn = true;
                claimAction.isOn = true;
            }
            if (rank.permissions.Contains(AtavismGuildRankPermission.claimEdit))
            {
                claimEdit.isOn = true;
                claimAdd.isOn = true;
                claimAction.isOn = true;
            }
            if (rank.permissions.Contains(AtavismGuildRankPermission.claimAction))
            {
                claimAction.isOn = true;
            }
            if (rank.permissions.Contains(AtavismGuildRankPermission.levelUp))
            {
                levelUp.isOn = true;
            }
            if (rank.permissions.Contains(AtavismGuildRankPermission.whAdd))
            {
                warehouseAdd.isOn = true;
            }
            if (rank.permissions.Contains(AtavismGuildRankPermission.whGet))
            {
                warehouseGet.isOn = true;
            }
            if (claimAdd.isOn || claimEdit.isOn)
                claimAction.isOn = true;
            else
                claimAction.isOn = false;
            if (memberRank == 0 && selectedRank != 0)
            {
                levelUp.interactable = true;
                warehouseAdd.interactable = true;
                warehouseGet.interactable = true;
                guildChatListen.interactable = true;
                guildChatSpeak.interactable = true;
                inviteMember.interactable = true;
                removeMember.interactable = true;
                promote.interactable = true;
                demote.interactable = true;
                setMOTD.interactable = true;
                editPublicNote.interactable = true;
                claimAdd.interactable = true;
                claimEdit.interactable = true;
                claimAction.interactable = true;
            }
            else
            {
                levelUp.interactable = false;
                warehouseAdd.interactable = false;
                warehouseGet.interactable = false;
                guildChatListen.interactable = false;
                guildChatSpeak.interactable = false;
                inviteMember.interactable = false;
                removeMember.interactable = false;
                promote.interactable = false;
                demote.interactable = false;
                setMOTD.interactable = false;
                editPublicNote.interactable = false;
                claimAdd.interactable = false;
                claimEdit.interactable = false;
                claimAction.interactable = false;
            }
        }

        public void SettingsDropdownClicked()
        {
            settingsDropDownOpen = !settingsDropDownOpen;
        }

        public void ShowRankSetings()
        {
            if (settingsDropdown != null)
            {
                if (settingsDropdown.value == 0)
                {
                    guildRanksPanel.gameObject.SetActive(true);
                    rankPermissionsPanel.gameObject.SetActive(false);
                }
                else if (settingsDropdown.value == 1)
                {
                    guildRanksPanel.gameObject.SetActive(false);
                    rankPermissionsPanel.gameObject.SetActive(true);
                }
                else
                {
                    guildRanksPanel.gameObject.SetActive(false);
                    rankPermissionsPanel.gameObject.SetActive(false);
                }
            }
            if (TMPSettingsDropdown != null)
            {
                if (TMPSettingsDropdown.value == 0)
                {
                    guildRanksPanel.gameObject.SetActive(true);
                    rankPermissionsPanel.gameObject.SetActive(false);
                }
                else if (TMPSettingsDropdown.value == 1)
                {
                    guildRanksPanel.gameObject.SetActive(false);
                    rankPermissionsPanel.gameObject.SetActive(true);
                }
                else
                {
                    guildRanksPanel.gameObject.SetActive(false);
                    rankPermissionsPanel.gameObject.SetActive(false);
                }
            }
        }

        public void ShowGuildRanks(Text buttonText)
        {
            settingsDropDown.gameObject.GetComponentInChildren<Text>().text = buttonText.text;
            settingsDropDownOpen = false;
            guildRanksPanel.gameObject.SetActive(true);
            rankPermissionsPanel.gameObject.SetActive(false);
            guildSettingsPanel.gameObject.SetActive(false);
        }

        public void ShowRankPermissions(Text buttonText)
        {
            settingsDropDown.gameObject.GetComponentInChildren<Text>().text = buttonText.text;
            settingsDropDownOpen = false;
            guildRanksPanel.gameObject.SetActive(false);
            rankPermissionsPanel.gameObject.SetActive(true);
            guildSettingsPanel.gameObject.SetActive(false);
        }

        public void ShowSettingsOptions(Text buttonText)
        {
            settingsDropDown.gameObject.GetComponentInChildren<Text>().text = buttonText.text;
            settingsDropDownOpen = false;
            guildRanksPanel.gameObject.SetActive(false);
            rankPermissionsPanel.gameObject.SetActive(false);
            guildSettingsPanel.gameObject.SetActive(true);
        }
        public void RankDropdownClicked()
        {
            rankDropDownOpen = !rankDropDownOpen;
            if (rankModDropdown)
            {
                selectedRank = rankModDropdown.value;
                UpdateRankDisplay();
            }
            if (TMPRankModDropdown)
            {
                selectedRank = TMPRankModDropdown.value;
                UpdateRankDisplay();
            }
        }

        public void RankDropdownButtonClicked(GameObject buttonClicked)
        {
            for (int i = 0; i < rankButtons.Count; i++)
            {
                if (buttonClicked.name == rankButtons[i].name)
                {
                    selectedRank = i;
                    break;
                }
            }
            if (rankDropDown)
            {
                rankDropDown.gameObject.GetComponentInChildren<Text>().text = AtavismGuild.Instance.Ranks[selectedRank].rankName;
                rankDropDownOpen = false;
            }
            UpdateRankDisplay();
        }

        public void LevelUpClick()
        {

            AtavismGuild.Instance.SendGuildCommand("levelUp", null, "");
        }

        public void ToggleLevelUp(bool toggled)
        {
            if (Time.time > interactionDelay)
                AtavismGuild.Instance.SendGuildCommand("editRank", null, AtavismGuild.Instance.Ranks[selectedRank].rankLevel + ";levelUp;" + (levelUp.isOn ? "1" : "0"));
        }

        public void ToggleWarehouseAdd(bool toggled)
        {
            if (Time.time > interactionDelay)
                AtavismGuild.Instance.SendGuildCommand("editRank", null, AtavismGuild.Instance.Ranks[selectedRank].rankLevel + ";whAdd;" + (warehouseAdd.isOn ? "1" : "0"));
        }

        public void ToggleWarehouseGet(bool toggled)
        {
            if (Time.time > interactionDelay)
                AtavismGuild.Instance.SendGuildCommand("editRank", null, AtavismGuild.Instance.Ranks[selectedRank].rankLevel + ";whGet;" + (warehouseGet.isOn ? "1" : "0"));
        }

        public void ToggleGuildChatListen(bool toggled)
        {
            if (Time.time > interactionDelay)
                AtavismGuild.Instance.SendGuildCommand("editRank", null, AtavismGuild.Instance.Ranks[selectedRank].rankLevel + ";chat;" + (guildChatListen.isOn ? "1" : "0"));
        }

        public void ToggleGuildChatSpeak(bool toggled)
        {
            if (Time.time > interactionDelay)
                AtavismGuild.Instance.SendGuildCommand("editRank", null, AtavismGuild.Instance.Ranks[selectedRank].rankLevel + ";chat;" + (guildChatSpeak.isOn ? "1" : "0"));
        }

        public void ToggleGuildInvite(bool toggled)
        {
            if (Time.time > interactionDelay)
                AtavismGuild.Instance.SendGuildCommand("editRank", null, AtavismGuild.Instance.Ranks[selectedRank].rankLevel + ";invite;" + (inviteMember.isOn ? "1" : "0"));
        }

        public void ToggleGuildRemove(bool toggled)
        {
            if (Time.time > interactionDelay)
                AtavismGuild.Instance.SendGuildCommand("editRank", null, AtavismGuild.Instance.Ranks[selectedRank].rankLevel + ";kick;" + (removeMember.isOn ? "1" : "0"));
        }

        public void ToggleGuildPromote(bool toggled)
        {
            if (Time.time > interactionDelay)
                AtavismGuild.Instance.SendGuildCommand("editRank", null, AtavismGuild.Instance.Ranks[selectedRank].rankLevel + ";promote;" + (promote.isOn ? "1" : "0"));
        }

        public void ToggleGuildDemote(bool toggled)
        {
            if (Time.time > interactionDelay)
                AtavismGuild.Instance.SendGuildCommand("editRank", null, AtavismGuild.Instance.Ranks[selectedRank].rankLevel + ";demote;" + (demote.isOn ? "1" : "0"));
        }

        public void ToggleGuildSetMotd(bool toggled)
        {
            if (Time.time > interactionDelay)
                AtavismGuild.Instance.SendGuildCommand("editRank", null, AtavismGuild.Instance.Ranks[selectedRank].rankLevel + ";setmotd;" + (setMOTD.isOn ? "1" : "0"));
        }

        public void ToggleGuildEditPublicNote(bool toggled)
        {
            if (Time.time > interactionDelay)
                AtavismGuild.Instance.SendGuildCommand("editRank", null, AtavismGuild.Instance.Ranks[selectedRank].rankLevel + ";editpublic;" + (editPublicNote.isOn ? "1" : "0"));
        }

        public void ToggleGuildEditClaim(bool toggled)
        {
            if (Time.time < interactionDelay)
                return;
         /*   if (claimEdit.isOn)
            {
                claimAdd.isOn = true;
                claimAction.isOn = true;
            }*/
            if (Time.time > interactionDelay)
                AtavismGuild.Instance.SendGuildCommand("editRank", null, AtavismGuild.Instance.Ranks[selectedRank].rankLevel + ";claimEdit;" + (claimEdit.isOn ? "1" : "0"));
        }
        public void ToggleGuildAddClaim(bool toggled)
        {
            if (Time.time < interactionDelay)
                return;
         /*   if (claimAdd.isOn)
            {
                claimAction.isOn = true;
            }
            if (claimEdit.isOn)
                claimAdd.isOn = true;*/
            if (Time.time > interactionDelay)
                AtavismGuild.Instance.SendGuildCommand("editRank", null, AtavismGuild.Instance.Ranks[selectedRank].rankLevel + ";claimAdd;" + (claimAdd.isOn ? "1" : "0"));
        }
        public void ToggleGuildActionClaim(bool toggled)
        {
            if (claimAdd.isOn || claimEdit.isOn)
                claimAction.isOn = true;
            if (Time.time > interactionDelay)
                AtavismGuild.Instance.SendGuildCommand("editRank", null, AtavismGuild.Instance.Ranks[selectedRank].rankLevel + ";claimAction;" + (claimAction.isOn ? "1" : "0"));
        }

        public void AddRankClick()
        {
            if (guildRanks.Count > AtavismGuild.Instance.Ranks.Count)
                AtavismGuild.Instance.SendGuildCommand("addRank", null, "NewRank");
        }
        public void EditMOTDClick()
        {
            if (editMOTDField != null)
                AtavismGuild.Instance.SendGuildCommand("setmotd", null, editMOTDField.text);
            HideEditMOTDPopup();

        }
        public void editMOTDMenuClicked()
        {
            HideMemberPopup();
            if (editMOTD != null)
            {
                editMOTD.gameObject.SetActive(true);
                if (editMOTD.gameObject.GetComponent<CanvasGroup>() != null)
                {
                    editMOTD.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
                    editMOTD.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    editMOTD.gameObject.GetComponent<CanvasGroup>().interactable = true;
                }
                editMOTD.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                editMOTDField.text = AtavismGuild.Instance.Motd;
                AtavismUIUtility.BringToFront(editMOTD.gameObject);
                EventSystem.current.SetSelectedGameObject(editMOTDField.gameObject, null);
            }
        }
        public void HideEditMOTDPopup()
        {
            if (editMOTD != null)
            {
                editMOTD.gameObject.SetActive(false);
                if (editMOTD.gameObject.GetComponent<CanvasGroup>() != null)
                {
                    editMOTD.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
                    editMOTD.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    editMOTD.gameObject.GetComponent<CanvasGroup>().interactable = false;
                }
            }
        }

        #endregion Rank Popup
    }
}