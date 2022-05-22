using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Atavism
{

    public class UGUISocialPanel : MonoBehaviour, IPointerClickHandler
    {

        public UGUIPanelTitleBar titleBar;
        //	public Text guildMotd;
        //	public RectTransform createPopup;
        //	public InputField guildNameField;
        //   public TMP_InputField TMPguildNameField;
        [SerializeField]
        UGUISocialMemberEntry prefab;
        public KeyCode toggleKey;
        [SerializeField] Button whisperButton;
        [SerializeField] Button groupButton;
        [SerializeField] Button guildButton;
        [SerializeField] Button privetInstanceButton;
        public RectTransform memberPopup;
        bool showing = false;
        AtavismSocialMember selectedMember = null;
        CanvasGroup _canvasGroup;
        float interactionDelay;
        [AtavismSeparator("Menu Settings")]
        public Color defaultTopMenuColor = Color.white;
        public Color selectedTopMenuColor = Color.green;
        public Color defaultTopMenuImageColor = Color.white;
        public Color selectedTopMenuImageColor = Color.green;
        // public bool disableTopMenuImage = true;
        public Button friensMenuButton;
        public TextMeshProUGUI friendsMenuButtonText;
        public Button blockMenuButton;
        public TextMeshProUGUI blockMenuButtonText;
        [AtavismSeparator("Friends List")]
        [SerializeField] GameObject fiendsPanel;
        [SerializeField] List<UGUISocialMemberEntry> friendList = new List<UGUISocialMemberEntry>();
        public RectTransform invitePopup;
        public RectTransform friendListGrid;
        public InputField inviteNameField;
        public TMP_InputField TMPInviteNameField;
        bool _showFriendList = false;

        [AtavismSeparator("Block List")]
        [SerializeField] List<UGUISocialMemberEntry> blockList = new List<UGUISocialMemberEntry>();
        [SerializeField] GameObject blockListPanel;
        public RectTransform blockAddPopup;
        public RectTransform blockListGrid;
        public TMP_InputField TMPBlockListNameField;
        //public RectTransform blockListMemberPopup;
        bool _showBlockList = false;

        // Use this for initialization
        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (titleBar != null)
                titleBar.SetOnPanelClose(Hide);
            Hide();
            toggleKey = AtavismSettings.Instance.GetKeySettings().social;

            AtavismEventSystem.RegisterEvent("SOCIAL_UPDATE", this);

        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("SOCIAL_UPDATE", this);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(toggleKey) && !ClientAPI.UIHasFocus())
            {
                Toggle();
            }
        }

        public void Show()
        {
            AtavismUIUtility.BringToFront(gameObject);
            AtavismSocial.Instance.SendGetFriends();
            //     ClearAllCells();
            UpdateSocialDetails();
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.interactable = true;
            }
            showing = true;
            //      invitePopup.gameObject.GetComponent<UIWindow>().Hide();
            invitePopup.gameObject.SetActive(false);
            blockAddPopup.gameObject.SetActive(false);
            memberPopup.gameObject.SetActive(false);
            showFriensList();

        }

        public void Hide()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.blocksRaycasts = false;
            }
            showing = false;
            if (invitePopup.gameObject.activeSelf)
                invitePopup.gameObject.SetActive(false);

        }

        public void showFriensList()
        {
            if (fiendsPanel != null)
                if (!fiendsPanel.activeSelf)
                    fiendsPanel.SetActive(true);
            if (blockListPanel != null)
                if (blockListPanel.activeSelf)
                    blockListPanel.SetActive(false);
            _showFriendList = true;
            _showBlockList = false;
            if (blockAddPopup.gameObject.activeSelf)
                blockAddPopup.gameObject.SetActive(false);
            if (invitePopup.gameObject.activeSelf)
                invitePopup.gameObject.SetActive(false);

            if (friensMenuButton != null)
            {
                friensMenuButton.targetGraphic.color = selectedTopMenuImageColor;
            }
            if (blockMenuButton != null)
            {
                blockMenuButton.targetGraphic.color = defaultTopMenuImageColor;
            }

            if (friendsMenuButtonText != null)
                friendsMenuButtonText.color = selectedTopMenuColor;
            if (blockMenuButtonText != null)
                blockMenuButtonText.color = defaultTopMenuColor;

        }

        public void showBlockList()
        {
            if (fiendsPanel != null)
                if (fiendsPanel.activeSelf)
                    fiendsPanel.SetActive(false);
            if (blockListPanel != null)
                if (!blockListPanel.activeSelf)
                    blockListPanel.SetActive(true);
            _showFriendList = false;
            _showBlockList = true;
            if (blockAddPopup.gameObject.activeSelf)
                blockAddPopup.gameObject.SetActive(false);
            if (invitePopup.gameObject.activeSelf)
                invitePopup.gameObject.SetActive(false);
            if (friensMenuButton != null)
            {
                friensMenuButton.targetGraphic.color = defaultTopMenuImageColor;
            }
            if (blockMenuButton != null)
            {
                blockMenuButton.targetGraphic.color = selectedTopMenuImageColor;
            }

            if (friendsMenuButtonText != null)
                friendsMenuButtonText.color = defaultTopMenuColor;
            if (blockMenuButtonText != null)
                blockMenuButtonText.color = selectedTopMenuColor;

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

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "SOCIAL_UPDATE")
            {
                //if (showing) {
                UpdateSocialDetails();
                //   }
            }
        }

        public void UpdateSocialDetails()
        {
            // int memberRank = -1;
            List<AtavismSocialMember> members = AtavismSocial.Instance.Friends;
            foreach (UGUISocialMemberEntry member in friendList)
            {
                if (member != null)
                    member.gameObject.SetActive(false);
            }

            int i = 1;
            foreach (AtavismSocialMember member in members)
            {
                //  Debug.LogError(aucid + " " + auctionsForGroupOrder.Count);
                if (i > friendList.Count)
                {
                    GameObject go = (GameObject)Instantiate(prefab.gameObject, friendListGrid);
                    friendList.Add(go.GetComponent<UGUISocialMemberEntry>());
                }
                if (!friendList[i - 1].gameObject.activeSelf)
                    friendList[i - 1].gameObject.SetActive(true);
                friendList[i - 1].SetSocialMemberDetails(member, this);

                i++;
            }

            members = AtavismSocial.Instance.Banneds;
            foreach (UGUISocialMemberEntry member in blockList)
            {
                if (member != null)
                    member.gameObject.SetActive(false);
            }

            i = 1;
            foreach (AtavismSocialMember member in members)
            {
                //  Debug.LogError(aucid + " " + auctionsForGroupOrder.Count);
                if (i > blockList.Count)
                {
                    GameObject go = (GameObject)Instantiate(prefab.gameObject, blockListGrid);
                    blockList.Add(go.GetComponent<UGUISocialMemberEntry>());
                }
                if (!blockList[i - 1].gameObject.activeSelf)
                    blockList[i - 1].gameObject.SetActive(true);
                blockList[i - 1].SetBlokSocialMemberDetails(member, this);

                i++;
            }

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            HideMemberPopup();
        }

        #region BlockList
        public void HideAddBlockPopup()
        {
            blockAddPopup.gameObject.SetActive(false);
        }

        public void AddBlockListMenuClicked()
        {
            HideMemberPopup();
            if (ClientAPI.GetTargetOid() > 0 && ClientAPI.GetTargetObject().CheckBooleanProperty("combat.userflag"))
            {
                AtavismSocial.Instance.SendAddBlock(OID.fromLong(ClientAPI.GetTargetOid()), null);
                return;
            }
            else
            {
                AtavismUIUtility.BringToFront(blockAddPopup.gameObject);
                blockAddPopup.gameObject.SetActive(true);
                blockAddPopup.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

                if (TMPBlockListNameField != null)
                {
                    TMPBlockListNameField.text = "";
                    EventSystem.current.SetSelectedGameObject(TMPBlockListNameField.gameObject, null);
                }
            }
        }
        public void AddBlockListMemberClicked()
        {
            HideMemberPopup();
            AtavismUIUtility.BringToFront(invitePopup.gameObject);
            if (ClientAPI.GetTargetOid() > 0 && ClientAPI.GetTargetObject().CheckBooleanProperty("combat.userflag"))
            {
                AtavismSocial.Instance.SendAddBlock(OID.fromLong(ClientAPI.GetTargetOid()), null);
                return;
            }
            else
            {
                blockAddPopup.gameObject.SetActive(true);

                if (TMPBlockListNameField != null)
                {
                    TMPBlockListNameField.text = "";
                    EventSystem.current.SetSelectedGameObject(TMPBlockListNameField.gameObject, null);
                }
            }
        }
        public void AddBlockListMemberByNameClicked()
        {

            if (TMPBlockListNameField != null)
                if (TMPBlockListNameField.text != "")
                {
                    AtavismSocial.Instance.SendAddBlock(null, TMPBlockListNameField.text);
                    blockAddPopup.gameObject.SetActive(false);
                }
        }

        #endregion BlockList
        #region FriendList
        public void HideInvitePopup()
        {
            invitePopup.gameObject.SetActive(false);
        }

        public void AddMemberClicked()
        {
            HideMemberPopup();
            AtavismUIUtility.BringToFront(invitePopup.gameObject);
            if (ClientAPI.GetTargetOid() > 0 && ClientAPI.GetTargetObject().CheckBooleanProperty("combat.userflag"))
            {
                AtavismSocial.Instance.SendInvitation(OID.fromLong(ClientAPI.GetTargetOid()), null);
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
                AtavismSocial.Instance.SendInvitation(OID.fromLong(ClientAPI.GetTargetOid()), null);
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
                    AtavismSocial.Instance.SendInvitation(null, inviteNameField.text);
                    invitePopup.gameObject.SetActive(false);
                }
            if (TMPInviteNameField != null)
                if (TMPInviteNameField.text != "")
                {
                    AtavismSocial.Instance.SendInvitation(null, TMPInviteNameField.text);
                    invitePopup.gameObject.SetActive(false);
                }
        }
        #endregion FriendList


        #region Member Popup
        public void ShowMemberPopup(UGUISocialMemberEntry selectedMemberEntry, AtavismSocialMember member)
        {
            selectedMember = member;
            memberPopup.gameObject.SetActive(true);
            Vector3 popupPosition = Input.mousePosition;
            // Add a button width/height to the tooltip position
            //RectTransform memberEntryTransform = selectedMemberEntry.GetComponent<RectTransform>();
            popupPosition += new Vector3(memberPopup.sizeDelta.x / 2, -memberPopup.sizeDelta.y / 2, 0);
            memberPopup.position = popupPosition; //new Vector2(popupPosition.x, memberEntryTransform.anchoredPosition.y);
            if (_showFriendList)
            {
                if (AtavismGuild.Instance.GuildName == null || AtavismGuild.Instance.GuildName == "")
                {
                    if (guildButton != null)
                        guildButton.gameObject.SetActive(false);
                }
                else
                {
                    AtavismGuildMember gMember = AtavismGuild.Instance.GetGuildMemberByOid(OID.fromLong(ClientAPI.GetPlayerOid()));
                    if (gMember != null)
                    {
                        AtavismGuildRank rank = AtavismGuild.Instance.Ranks[gMember.rank];
                        if (rank.permissions.Contains(AtavismGuildRankPermission.invite))
                        {
                            if (guildButton != null)
                                guildButton.gameObject.SetActive(true);
                        }
                    }

                }
                if (groupButton != null)
                    groupButton.gameObject.SetActive(true);
                if (whisperButton != null)
                    whisperButton.gameObject.SetActive(true);
                if(privetInstanceButton)
                    privetInstanceButton.gameObject.SetActive(true);

            }
            else
            {
                if (guildButton != null)
                    guildButton.gameObject.SetActive(false);
                if (groupButton != null)
                    groupButton.gameObject.SetActive(false);
                if (whisperButton != null)
                    whisperButton.gameObject.SetActive(false);
                if(privetInstanceButton)
                    privetInstanceButton.gameObject.SetActive(false);

            }
        }

        public void HideMemberPopup()
        {
            memberPopup.gameObject.SetActive(false);
        }

        public void InvitePrivateInstane()
        {
           AtavismSocial.Instance.SendInvitationToPtivate(selectedMember.oid);
           memberPopup.gameObject.SetActive(false);
           AtavismSettings.Instance.DsContextMenu(null);
        }
       
        public void WhisperOptionClicked()
        {

            UGUIChatController.Instance.StartWhisper(AtavismSocial.Instance.GetSocialMemberByOid(selectedMember.oid).name);
            memberPopup.gameObject.SetActive(false);
            AtavismSettings.Instance.DsContextMenu(null);
        }

        public void InviteGroupOptionClicked()
        {
            AtavismGroup.Instance.SendInviteRequestMessage(selectedMember.oid);
            memberPopup.gameObject.SetActive(false);
            AtavismSettings.Instance.DsContextMenu(null);
        }

        public void InviteGuildOptionClicked()
        {
            AtavismGuild.Instance.SendGuildCommand("invite", null, AtavismSocial.Instance.GetSocialMemberByOid(selectedMember.oid).name);
            memberPopup.gameObject.SetActive(false);
            AtavismSettings.Instance.DsContextMenu(null);
        }

        public void DeleteOptionClicked()
        {
            //    Debug.LogError("DeleteOptionClicked");
            if (_showFriendList)
            {
                AtavismSocial.Instance.SendDelFriend(selectedMember.oid);
            }
            else
            {
                AtavismSocial.Instance.SendDelBlock(selectedMember.oid);
            }
            memberPopup.gameObject.SetActive(false);
            AtavismSettings.Instance.DsContextMenu(null);
        }



        #endregion Member Popup


    }
}