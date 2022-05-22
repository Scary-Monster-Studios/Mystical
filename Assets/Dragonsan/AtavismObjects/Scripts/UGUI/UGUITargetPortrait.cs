using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using System.Linq;

namespace Atavism
{
    [Serializable]
    public class TargetMobTyp
    {
        public int Mobtype = 0;
        public string mark = "";
        public Sprite additionalImage;
      //  public Sprite glowLightImage;
    }

    public class UGUITargetPortrait : MonoBehaviour, IPointerClickHandler
    {

        public Text name;
        public TextMeshProUGUI TMPName;
        public Text levelText;
        public TextMeshProUGUI TMPDescriptionText;
        public TextMeshProUGUI TMPSpeciesText;
        public TextMeshProUGUI TMPLevelText;
        public Image portrait;
        public Image agroImage;
        public bool showPopupOnRightClick = true;
        [Header("Popup settings")]
        public Button menuPopupButton;
        public RectTransform popupMenu;
        public Button WhisperButton;
        public Button InviteGroupButton;
        public Button TradeButton;
        public Button DuelButton;
        public Button InfoButton;
        public Button petDespawnButton;
        public Button friendButton;
        public Button guildButton;
        public Button buildHelpButton;
        public Button buildRepairButton;

        public UGUIChatController chatController;
        public List<UGUIEffect> effectButtons;
        string characterName;
        string gender;
        //int classID = -1;
        int level = 1;
        Sprite defaultPortrait;
        [Header("Mob Quality Settings")]
        public Image mobTypeImage;
        public bool mobTypeAddStars;
        public List<TargetMobTyp> mobtypeDefinition = new List<TargetMobTyp>();

        [Header("Mob stance Settings")]
        public Image LightImage;
        public Sprite friendlyImage;
        public Sprite neutralImage;
        public Sprite enemyImage;

        public Color friendlyNameColour = Color.green;
        public Color neutralNameColour = Color.yellow;
        public Color enemyNameColour = Color.red;
        List<AtavismEffect> targetEffects;
        AtavismObjectNode node;
        long targetOID = 0;
        public bool activeEffect = true;

        // Use this for initialization
        void Start()
        {
            targetEffects = new List<AtavismEffect>();
            if (portrait != null)
                defaultPortrait = portrait.sprite;
            Hide();
            AtavismEventSystem.RegisterEvent("PLAYER_TARGET_CHANGED", this);
            AtavismEventSystem.RegisterEvent("OBJECT_TARGET_CHANGED", this);
            AtavismEventSystem.RegisterEvent("EFFECT_ICON_UPDATE", this);

        }

        void Awake()
        {
            Hide();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("PLAYER_TARGET_CHANGED", this);
            AtavismEventSystem.UnregisterEvent("OBJECT_TARGET_CHANGED", this);
            AtavismEventSystem.UnregisterEvent("EFFECT_ICON_UPDATE", this);
        }

        void Show()
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().interactable = true;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().ignoreParentGroups = true;
        }

        void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "OBJECT_TARGET_CHANGED")
            {
                // int id = int.Parse(eData.eventArgs[0]);                
                //  int claimId = int.Parse(eData.eventArgs[1]);                
                if(WorldBuilder.Instance.SelectedClaimObject!=null){
                    int id = WorldBuilder.Instance.SelectedClaimObject.ID;                
                    int claimId = WorldBuilder.Instance.SelectedClaimObject.ClaimID;    
                    
                    Show();
                    UpdatePortraitClaimObject(id,claimId);
                }
                else if(ClientAPI.GetTargetObject() == null)
                {
                    targetEffects.Clear();
                    UpdateEffects();
                    Hide();
                    popupMenu.gameObject.SetActive(false);
                    AtavismSettings.Instance.DsContextMenu(null);
                }
              

            }else if (eData.eventType == "PLAYER_TARGET_CHANGED")
            {
                if (ClientAPI.GetTargetObject() != null)
                {
                    Show();
                    UpdatePortrait();
                }
                else
                {
                    targetEffects.Clear();
                    UpdateEffects();
                    Hide();
                }
                popupMenu.gameObject.SetActive(false);
                AtavismSettings.Instance.DsContextMenu(null);

            }
            else if (eData.eventType == "EFFECT_ICON_UPDATE")
            {
                CheckEffects();
            }
        }
        public void UpdatePortraitClaimObject(int objId, int claimId)
        {
//            Debug.LogError("claimObject selected  claimId=" +claimId+" objId="+objId);
            if (petDespawnButton != null)
                petDespawnButton.gameObject.SetActive(false);
            if (WhisperButton != null)
                WhisperButton.gameObject.SetActive(false);
            if (InviteGroupButton != null)
                InviteGroupButton.gameObject.SetActive(false);
            if (TradeButton != null)
                TradeButton.gameObject.SetActive(false);
            if (DuelButton != null)
                DuelButton.gameObject.SetActive(false);
            if (InfoButton != null)
                InfoButton.gameObject.SetActive(false);
            if (friendButton != null)
                friendButton.gameObject.SetActive(false);
            if (guildButton != null)
                guildButton.gameObject.SetActive(false);
            if (buildHelpButton != null)
                buildHelpButton.gameObject.SetActive(false);
            if (buildRepairButton != null)
                buildRepairButton.gameObject.SetActive(false);
            
            if (menuPopupButton != null)
            {
                menuPopupButton.gameObject.SetActive(false);
            }
            if (levelText != null)
            {
                levelText.text = "";
            }

            if (TMPLevelText != null)
            {
                TMPLevelText.text = "";
            }

            if (LightImage != null)
            {
                LightImage.enabled = false;
            }
            if (mobTypeImage != null)
            {
                mobTypeImage.enabled = false;
            }
            ClaimObject co = WorldBuilder.Instance.SelectedClaimObject;
            if (co != null)
            {
                if (!co.Solo)
                {
                    if (co.totalTime > 0 )
                    {
                        if (!WorldBuilder.Instance.GetClaim(co.ClaimID).playerOwned || co.timeSpeed == 0F) ;
                        {
                            if (buildHelpButton != null)
                                buildHelpButton.gameObject.SetActive(true);
                            if (menuPopupButton != null)
                            {
                                menuPopupButton.gameObject.SetActive(true);
                            }
                        }
                    }
                }

                string mName = co.TemplateName;
                if (name != null)
                {
                    name.text = mName;

                }

                if (TMPName != null)
                {
                    TMPName.text = mName;
                }
                if (portrait != null)
                {
                    //portrait.enabled = false;
                    AtavismBuildObjectTemplate template = WorldBuilder.Instance.GetBuildObjectTemplate(co.TemplateID);
                    portrait.sprite = template.icon;
                }

                
                if (TMPDescriptionText != null)
                {
                
                    TMPDescriptionText.text = co.Status;
                }
                if (TMPSpeciesText != null)
                {
               
                    TMPSpeciesText.text = "";
                }
            }

            // Show/Hide menuPopupButton depending on whether it is an NPC or not
           
        }

        public void UpdatePortrait()
        {
            if (ClientAPI.GetTargetObject() == null)
            {
                Hide();
                return;
            }
         /*   if (name != null)
                name.text = ClientAPI.GetTargetObject().Name;
            if (TMPName != null)
                TMPName.text = ClientAPI.GetTargetObject().Name;*/
            string mark = "";
            if (targetOID > 0 && targetOID != ClientAPI.GetTargetOid())
            {
                node = ClientAPI.GetObjectNode(targetOID);
                if (node != null)
                {
                    node.RemovePropertyChangeHandler("effects", EffectsPropertyHandler);
                    node.RemovePropertyChangeHandler("level", LevelPropertyHandler);
                    node.RemovePropertyChangeHandler("reaction", ReactionPropertyHandler);
                    node.RemovePropertyChangeHandler("aggressive", ReactionPropertyHandler);
                    //             Debug.LogError("Props unRegister Target");
                }
                node = null;
            }
            if (node == null)
            {
                node = ClientAPI.GetTargetObject();
                if (node != null)
                {
                    targetOID = node.Oid;
                    node.RegisterPropertyChangeHandler("effects", EffectsPropertyHandler);
                    node.RegisterPropertyChangeHandler("level", LevelPropertyHandler);
                    node.RegisterPropertyChangeHandler("reaction", ReactionPropertyHandler);
                    node.RegisterPropertyChangeHandler("aggressive", ReactionPropertyHandler);
                    // OID petOowner = null;
                    //   if (node.PropertyExists("petOwner")) petOowner = (OID)node.GetProperty("petOwner");


                    if (petDespawnButton != null)
                        if (node.PropertyExists("pet") && node.PropertyExists("petOwner") &&
                            ((OID)node.GetProperty("petOwner")).Equals(OID.fromLong(ClientAPI.GetPlayerOid())))
                        {
                            if (petDespawnButton != null)
                                petDespawnButton.gameObject.SetActive(true);
                            if (WhisperButton != null)
                                WhisperButton.gameObject.SetActive(false);
                            if (InviteGroupButton != null)
                                InviteGroupButton.gameObject.SetActive(false);
                            if (TradeButton != null)
                                TradeButton.gameObject.SetActive(false);
                            if (DuelButton != null)
                                DuelButton.gameObject.SetActive(false);
                            if (InfoButton != null)
                                InfoButton.gameObject.SetActive(false);
                            if (friendButton != null)
                                friendButton.gameObject.SetActive(false);
                            if (guildButton != null)
                                guildButton.gameObject.SetActive(false);
                            if (buildHelpButton != null)
                                buildHelpButton.gameObject.SetActive(false);
                            if (buildRepairButton != null)
                                buildRepairButton.gameObject.SetActive(false);
                        }
                        else if (!node.PropertyExists("pet"))
                        {
                            if (petDespawnButton != null)
                                petDespawnButton.gameObject.SetActive(false);
                            if (WhisperButton != null)
                                WhisperButton.gameObject.SetActive(true);
                            if (InviteGroupButton != null)
                                InviteGroupButton.gameObject.SetActive(true);
                            if (TradeButton != null)
                                TradeButton.gameObject.SetActive(true);
                            if (DuelButton != null)
                                DuelButton.gameObject.SetActive(true);
                            if (InfoButton != null)
                                InfoButton.gameObject.SetActive(true);
                            if (friendButton != null)
                                friendButton.gameObject.SetActive(true);
                            if (buildHelpButton != null)
                                buildHelpButton.gameObject.SetActive(false);
                            if (buildRepairButton != null)
                                buildRepairButton.gameObject.SetActive(false);


                            AtavismGuildMember member = AtavismGuild.Instance.GetGuildMemberByOid(OID.fromLong(ClientAPI.GetPlayerOid()));
                            if (member != null)
                            {
                                if (AtavismGuild.Instance.Ranks.Count > member.rank)
                                {
                                    AtavismGuildRank rank = AtavismGuild.Instance.Ranks[member.rank];
                                    if (rank.permissions.Contains(AtavismGuildRankPermission.invite))
                                    {
                                        if (guildButton != null)
                                            guildButton.gameObject.SetActive(true);
                                    }
                                    else
                                    {
                                        if (guildButton != null)
                                            guildButton.gameObject.SetActive(false);
                                    }
                                }
                                else
                                {
                                    if (guildButton != null)
                                        guildButton.gameObject.SetActive(false);
                                }
                            }
                            else
                            {
                                if (guildButton != null)
                                    guildButton.gameObject.SetActive(false);
                            }
                        }

                    CheckEffects();
                }
            }

            if (levelText != null)
            {
                if (ClientAPI.GetTargetObject().PropertyExists("level"))
                    level = (int)ClientAPI.GetTargetObject().GetProperty("level");
                levelText.text = level.ToString();
            }
            if (TMPLevelText != null)
            {
                if (ClientAPI.GetTargetObject().PropertyExists("level"))
                    level = (int)ClientAPI.GetTargetObject().GetProperty("level");
                TMPLevelText.text = level.ToString();
            }
            if (ClientAPI.GetTargetObject().PropertyExists("mobType"))
            {
                int mobType = (int)ClientAPI.GetTargetObject().GetProperty("mobType");
            

                foreach (TargetMobTyp tmt in mobtypeDefinition)
                {
                    if (tmt.Mobtype == mobType)
                    {
                        mark = tmt.mark;
                        if (mobTypeImage != null)
                            mobTypeImage.sprite = tmt.additionalImage;
                    }
                }

                if (mobTypeImage != null)
                {
                      //  mobTypeImage.sprite = mobTypeImageDef[mobType];
                        if (mobTypeImage.sprite != null)
                        {
                            mobTypeImage.enabled = true;
                        }
                        else
                        {
                            mobTypeImage.enabled = false;
                        }
                 
                }
            }
            else
            {
                if (mobTypeImage != null)
                {
                    mobTypeImage.enabled = false;
                }
            }
            if (!mobTypeAddStars)
                mark = "";
            Color col = Color.white;
            int targetType = 0;
            if (ClientAPI.GetTargetObject().PropertyExists("reaction"))
            {
                targetType = (int)ClientAPI.GetTargetObject().GetProperty("reaction");
                if (ClientAPI.GetTargetObject().PropertyExists("aggressive"))
                {
                    if ((bool)ClientAPI.GetTargetObject().GetProperty("aggressive"))
                    {
                        targetType = -1;
                    }
                }
                if (targetType < 0 && agroImage != null)
                    agroImage.enabled = true;
                else if (targetType > 0 && agroImage != null)
                    agroImage.enabled = false;

                if (targetType < 0 && LightImage != null)
                {
                    // if (Lights.Count > 2)
                    LightImage.sprite = enemyImage;
                        //Lights[2];
                    LightImage.enabled = true;
                    col = enemyNameColour;
                }
                else if (targetType > 0 && LightImage != null)
                {
                    ///if (Lights.Count > 0)
                    LightImage.sprite = friendlyImage;
                    //Lights[0];
                    LightImage.enabled = true;
                    col = friendlyNameColour;
                }
                else if (targetType == 0 && LightImage != null)
                {
                    //if (Lights.Count > 1)
                    LightImage.sprite = neutralImage;
                    //Lights[1];
                    LightImage.enabled = true;
                    col = neutralNameColour;
                }
            }
            else
            {
                if (LightImage != null)
                {
                    LightImage.enabled = false;
                }
            }
            string mName = "";
                 if (ClientAPI.GetTargetObject().PropertyExists("displayName"))
            {
                mName = (string)ClientAPI.GetTargetObject().GetProperty("displayName");
            }
            else
            {
                mName = ClientAPI.GetTargetObject().Name;
            }
            if (TMPDescriptionText != null)
            {
                if (ClientAPI.GetTargetObject().PropertyExists("subTitle"))
                    TMPDescriptionText.text = (string)ClientAPI.GetTargetObject().GetProperty("subTitle");
                else
                    TMPDescriptionText.text = "";
            }
            if (TMPSpeciesText != null)
            {
                if (ClientAPI.GetTargetObject().PropertyExists("species"))
                    TMPSpeciesText.text = (string)ClientAPI.GetTargetObject().GetProperty("species");
                else
                    TMPSpeciesText.text = "";
            }
#if AT_I2LOC_PRESET
        if (TMPDescriptionText != null){
            if( ClientAPI.GetTargetObject().PropertyExists("subTitle")&& ((string)ClientAPI.GetTargetObject().GetProperty("subTitle")).Length>0)
                TMPDescriptionText.text = I2.Loc.LocalizationManager.GetTranslation("Mobs/" +(string)ClientAPI.GetTargetObject().GetProperty("subTitle"));
            else
                TMPDescriptionText.text = "";
        }
        if (TMPSpeciesText != null){
            if( ClientAPI.GetTargetObject().PropertyExists("species"))
                TMPSpeciesText.text = I2.Loc.LocalizationManager.GetTranslation((string)ClientAPI.GetTargetObject().GetProperty("species"));
            else
                TMPSpeciesText.text = "";
        }
        if (I2.Loc.LocalizationManager.GetTranslation("Mobs/" + mName) != "" && I2.Loc.LocalizationManager.GetTranslation("Mobs/" + mName) != null) mName = I2.Loc.LocalizationManager.GetTranslation("Mobs/" + mName);
#endif
            if (name != null)
            {
                name.text = mark + " " + mName + " " + mark;
                name.color = col;
            }
            if (TMPName != null)
            {
                TMPName.text = mark + " <" + ToRGBHex(col) + ">" + mName + "</color > " + mark;
            }

            // Try get other properties
            /*if (ClientAPI.GetTargetObject().PropertyExists("health")) {
                health = (int)ClientAPI.GetTargetObject().GetProperty("health");
            }
            if (ClientAPI.GetTargetObject().PropertyExists("health-max")) {
                maxHealth = (int)ClientAPI.GetTargetObject().GetProperty("health-max");
            }*/

            if (portrait != null)
            {
                if (ClientAPI.GetTargetObject() != null && ClientAPI.GetTargetObject().GameObject != null)
                {
                    Sprite portraitSprite = PortraitManager.Instance.LoadPortrait(ClientAPI.GetTargetObject().GameObject.GetComponent<AtavismNode>());
                    if (portraitSprite == null)
                    {
                        if (ClientAPI.GetTargetObject() != null)
                            portraitSprite = ClientAPI.GetTargetObject().PropertyExists("portrait") ?
                                     PortraitManager.Instance.LoadPortrait((string)ClientAPI.GetTargetObject().GetProperty("portrait")) :
                                     ClientAPI.GetTargetObject().PropertyExists("custom:portrait") ?
                                     PortraitManager.Instance.LoadPortrait((string)ClientAPI.GetTargetObject().GetProperty("custom:portrait")) : null;
                    }
                    portrait.enabled = true;
                    if (portraitSprite != null)
                        portrait.sprite = portraitSprite;
                    else
                        portrait.sprite = defaultPortrait;
                }
            }

            // Show/Hide menuPopupButton depending on whether it is an NPC or not
            if (menuPopupButton != null)
            {
                if (ClientAPI.GetTargetObject().CheckBooleanProperty("combat.userflag"))
                {
                    menuPopupButton.gameObject.SetActive(true);
                }
                else
                {
                    menuPopupButton.gameObject.SetActive(false);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right)
            {

                return;
            }
            if (showPopupOnRightClick)
                TogglePopupMenu();
        }

        public void TogglePopupMenu()
        {
            if (popupMenu.gameObject.activeSelf)
            {
                popupMenu.gameObject.SetActive(false);
                AtavismSettings.Instance.DsContextMenu(null);
                return;
            }

            if (node != null && !node.PropertyExists("pet"))
            {
                if (petDespawnButton != null)
                    petDespawnButton.gameObject.SetActive(false);
                if (WhisperButton != null)
                    WhisperButton.gameObject.SetActive(true);
                if (InviteGroupButton != null)
                    InviteGroupButton.gameObject.SetActive(true);
                if (TradeButton != null)
                    TradeButton.gameObject.SetActive(true);
                if (DuelButton != null)
                    DuelButton.gameObject.SetActive(true);
                if (InfoButton != null)
                    InfoButton.gameObject.SetActive(true);
                if (friendButton != null)
                    friendButton.gameObject.SetActive(true);
                if (buildRepairButton != null)
                    buildRepairButton.gameObject.SetActive(false);
                if (buildRepairButton != null)
                    buildRepairButton.gameObject.SetActive(false);
                
                AtavismGuildMember member = AtavismGuild.Instance.GetGuildMemberByOid(OID.fromLong(ClientAPI.GetPlayerOid()));
                if (member != null)
                {
                    if (AtavismGuild.Instance.Ranks.Count > member.rank)
                    {
                        AtavismGuildRank rank = AtavismGuild.Instance.Ranks[member.rank];
                        if (rank.permissions.Contains(AtavismGuildRankPermission.invite))
                        {
                            if (guildButton != null)
                                guildButton.gameObject.SetActive(true);
                        }
                        else
                        {
                            if (guildButton != null)
                                guildButton.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        if (guildButton != null)
                            guildButton.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (guildButton != null)
                        guildButton.gameObject.SetActive(false);
                }
            }

            if (node!=null && node.PropertyExists("pet") && node.PropertyExists("petOwner") &&
                            ((OID)node.GetProperty("petOwner")).Equals(OID.fromLong(ClientAPI.GetPlayerOid())))
            {

                if (petDespawnButton != null)
                    petDespawnButton.gameObject.SetActive(true);
                popupMenu.gameObject.SetActive(true);
                AtavismSettings.Instance.DsContextMenu(popupMenu.gameObject);
            }
            else if(WorldBuilder.Instance.SelectedClaimObject==null)
            {
                if (buildHelpButton != null)
                    buildHelpButton.gameObject.SetActive(false);
                if (buildRepairButton != null)
                    buildRepairButton.gameObject.SetActive(false);
                if (petDespawnButton != null)
                    petDespawnButton.gameObject.SetActive(false);
                if (WhisperButton != null)
                    WhisperButton.gameObject.SetActive(true);
                if (InviteGroupButton != null)
                    InviteGroupButton.gameObject.SetActive(true);
                if (TradeButton != null)
                    TradeButton.gameObject.SetActive(true);
                if (DuelButton != null)
                    DuelButton.gameObject.SetActive(true);
                if (InfoButton != null)
                    InfoButton.gameObject.SetActive(true);
                if (friendButton != null)
                    friendButton.gameObject.SetActive(true);
                AtavismGuildMember member = AtavismGuild.Instance.GetGuildMemberByOid(OID.fromLong(ClientAPI.GetPlayerOid()));
                if (member != null)
                {
                    if (AtavismGuild.Instance.Ranks.Count > member.rank)
                    {
                        AtavismGuildRank rank = AtavismGuild.Instance.Ranks[member.rank];
                        if (rank.permissions.Contains(AtavismGuildRankPermission.invite))
                        {
                            if (guildButton != null)
                                guildButton.gameObject.SetActive(true);
                        }
                        else
                        {
                            if (guildButton != null)
                                guildButton.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        if (guildButton != null)
                            guildButton.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (guildButton != null)
                        guildButton.gameObject.SetActive(false);
                }
            }
             ClaimObject co = WorldBuilder.Instance.SelectedClaimObject;
         //   Debug.LogError("Menu " + co);
            if (co != null)
            {
                if (petDespawnButton != null)
                    petDespawnButton.gameObject.SetActive(false);
                if (WhisperButton != null)
                    WhisperButton.gameObject.SetActive(false);
                if (InviteGroupButton != null)
                    InviteGroupButton.gameObject.SetActive(false);
                if (TradeButton != null)
                    TradeButton.gameObject.SetActive(false);
                if (DuelButton != null)
                    DuelButton.gameObject.SetActive(false);
                if (InfoButton != null)
                    InfoButton.gameObject.SetActive(false);
                if (friendButton != null)
                    friendButton.gameObject.SetActive(false);
                if (guildButton != null)
                    guildButton.gameObject.SetActive(false);
                if (buildHelpButton != null)
                    buildHelpButton.gameObject.SetActive(false);
                if (buildRepairButton != null)
                    buildRepairButton.gameObject.SetActive(false);
               // Debug.LogError("Menu "+co.Solo+" "+co.Health+" "+co.MaxHealth+" "+co.Repairable);
                if (!co.Solo)
                {
                    
                    if (co.totalTime > 0 || co.currentTime < co.totalTime)
                    {
                        if (!WorldBuilder.Instance.GetClaim(co.ClaimID).playerOwned)
                        {
                            if (buildHelpButton != null)
                                buildHelpButton.gameObject.SetActive(true);
                            popupMenu.gameObject.SetActive(true);
                            AtavismSettings.Instance.DsContextMenu(popupMenu.gameObject);
                        }
                    }
                }

                if (co.Health < co.MaxHealth && co.Repairable)
                {
                  /*  if (WorldBuilder.Instance.GetClaim(co.ClaimID).permissionlevel < 1 && !WorldBuilder.Instance.GetClaim(co.ClaimID).playerOwned)
                    {
                        return;
                    }*/
                    if (buildRepairButton != null)
                        buildRepairButton.gameObject.SetActive(true);
                    popupMenu.gameObject.SetActive(true);
                    AtavismSettings.Instance.DsContextMenu(popupMenu.gameObject);
                }
            }

            if (ClientAPI.GetTargetObject() == null)
                return;

            // Verify the target is a player and is friendly
            if (!ClientAPI.GetTargetObject().CheckBooleanProperty("combat.userflag"))
                return;

            if (!ClientAPI.GetTargetObject().PropertyExists("targetType"))
                return;
            int targetType = (int)ClientAPI.GetTargetObject().GetProperty("targetType");
            if (targetType <= 0)
                return;

            // Work out what to put in the popup menu here
            popupMenu.gameObject.SetActive(true);
            AtavismSettings.Instance.DsContextMenu(popupMenu.gameObject);

        }

        public void MenuButtonClicked(int buttonPos)
        {
        }

        public void TradeOptionClicked()
        {

            if (ClientAPI.GetPlayerObject() != null && ClientAPI.GetObjectNode(ClientAPI.GetTargetOid()) != null)
                if (Vector3.Distance(ClientAPI.GetPlayerObject().Position, ClientAPI.GetObjectNode(ClientAPI.GetTargetOid()).Position) > AtavismTrade.Instance.InteractionDistance)
                {
                    string[] args = new string[1];
#if AT_I2LOC_PRESET
                args[0] = I2.Loc.LocalizationManager.GetTranslation("Target is too far away");
#else
                    args[0] = "Target is too far away";
#endif
                    AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
                }
                else
                {
                    NetworkAPI.SendTargetedCommand(ClientAPI.GetTargetOid(), "/trade");
                }
            popupMenu.gameObject.SetActive(false);
            AtavismSettings.Instance.DsContextMenu(null);
        }

        public void WhisperOptionClicked()
        {
            if (chatController != null)
            {
                chatController.StartWhisper(ClientAPI.GetTargetObject().Name);
            }
            UGUIChatController.Instance.StartWhisper(ClientAPI.GetTargetObject().Name);
            popupMenu.gameObject.SetActive(false);
            AtavismSettings.Instance.DsContextMenu(null);
        }

        public void InviteOptionClicked()
        {
            AtavismGroup.Instance.SendInviteRequestMessage(OID.fromLong(ClientAPI.GetTargetOid()));
            popupMenu.gameObject.SetActive(false);
            AtavismSettings.Instance.DsContextMenu(null);
        }

        public void DuelOptionClicked()
        {
            NetworkAPI.SendTargetedCommand(ClientAPI.GetTargetOid(), "/duel");
            popupMenu.gameObject.SetActive(false);
            AtavismSettings.Instance.DsContextMenu(null);
        }
        public void InfoOptionClicked()
        {
            UGUIOtherCharacterPanel.Instance.UpdateCharacetrData(ClientAPI.GetTargetOid());
            popupMenu.gameObject.SetActive(false);
            AtavismSettings.Instance.DsContextMenu(null);
        }

        public void DespawnCommand()
        {
            NetworkAPI.SendTargetedCommand(ClientAPI.GetTargetOid(), "/petCommand despawn");

            popupMenu.gameObject.SetActive(false);
            AtavismSettings.Instance.DsContextMenu(null);
        }

        public void FriendCommand()
        {
            AtavismSocial.Instance.SendInvitation(OID.fromLong(ClientAPI.GetTargetOid()), null);
            popupMenu.gameObject.SetActive(false);
            AtavismSettings.Instance.DsContextMenu(null);
        }
        /// <summary>
        /// Send Invite to guild command 
        /// </summary>
        public void GuildCommand()
        {
            AtavismGuild.Instance.SendGuildCommand("invite", OID.fromLong(ClientAPI.GetTargetOid()), null);
            popupMenu.gameObject.SetActive(false);
            AtavismSettings.Instance.DsContextMenu(null);
        }
        /// <summary>
        /// Send Building Help command 
        /// </summary>
        public void BuildHelpCommand()
        {
         //   Debug.LogError("BuildHelpCommand");
           /* if (WorldBuilder.Instance.SelectedClaimObject == null)
                return;*/
            int id = WorldBuilder.Instance.SelectedClaimObject.ID;                
            int claimID = WorldBuilder.Instance.SelectedClaimObject.ClaimID; 
         //   Debug.LogError("BuildHelpCommand objId="+id+" claimId="+claimID);

            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("action", "useHelp");
            props.Add("claimID", claimID);
            props.Add("objectID", id);
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.EDIT_CLAIM_OBJECT", props);  
            popupMenu.gameObject.SetActive(false);
            AtavismSettings.Instance.DsContextMenu(null);
          //  Debug.LogError("BuildHelpCommand End");

        }
/// <summary>
/// Send Building Repiar command
/// </summary>
        public void BuildRepairCommand()
        {
           /* if (WorldBuilder.Instance.SelectedClaimObject == null)
                return;*/
            int id = WorldBuilder.Instance.SelectedClaimObject.ID;                
            int claimID = WorldBuilder.Instance.SelectedClaimObject.ClaimID; 
            if (WorldBuilder.Instance.GetClaim(claimID).permissionlevel < 1 && !WorldBuilder.Instance.GetClaim(claimID).playerOwned)
            {
                Debug.LogError("BuildHelpCommand no permition");
                return;
            }

            if (!WorldBuilder.Instance.GetClaim(claimID).claimObjects[id].Repairable)
            {
                Debug.LogError("BuildHelpCommand no Repairable");
                return;
            }

            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("action", "useRepair");
            props.Add("claimID", claimID);
            props.Add("objectID", id);
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.EDIT_CLAIM_OBJECT", props);  
            popupMenu.gameObject.SetActive(false);
            AtavismSettings.Instance.DsContextMenu(null);
        }


        static string ToRGBHex(Color c)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
        }

        private static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        public void EffectsPropertyHandler(object sender, PropertyChangeEventArgs args)
        {
            CheckEffects();
        }
        public void LevelPropertyHandler(object sender, PropertyChangeEventArgs args)
        {
            UpdateLeval();
        }
        public void ReactionPropertyHandler(object sender, PropertyChangeEventArgs args)
        {
            UpdatePortrait();
        }

        private void UpdateLeval()
        {
            if (levelText != null)
            {
                if (ClientAPI.GetTargetObject().PropertyExists("level"))
                    level = (int)ClientAPI.GetTargetObject().GetProperty("level");
                levelText.text = level.ToString();
            }
            if (TMPLevelText != null)
            {
                if (ClientAPI.GetTargetObject().PropertyExists("level"))
                    level = (int)ClientAPI.GetTargetObject().GetProperty("level");
                TMPLevelText.text = level.ToString();
            }
        }

        private void CheckEffects()
        {
           // AtavismEffect[] aEffects = tempCombatDataStorage.GetComponents<AtavismEffect>();
            if (ClientAPI.GetTargetObject() == null)
                return;
            LinkedList<object> effects_prop = new LinkedList<object>();
            float effects_prop_time = 0f;
            if (ClientAPI.GetTargetObject().PropertyExists("effects"))
            {
                effects_prop = (LinkedList<object>)ClientAPI.GetTargetObject().GetProperty("effects");
                if (ClientAPI.GetTargetObject().PropertyExists("effects_t"))
                    effects_prop_time = (float)ClientAPI.GetTargetObject().GetProperty("effects_t");
            }
            if (levelText != null)
            {
                if (ClientAPI.GetTargetObject().PropertyExists("level"))
                    level = (int)ClientAPI.GetTargetObject().GetProperty("level");
                levelText.text = level.ToString();
            }
            if (TMPLevelText != null)
            {
                if (ClientAPI.GetTargetObject().PropertyExists("level"))
                    level = (int)ClientAPI.GetTargetObject().GetProperty("level");
                TMPLevelText.text = level.ToString();
            }
            AtavismLogger.LogDebugMessage("Got target effects property change: " + effects_prop);

            targetEffects.Clear();
            //   int pos = 0;
            List<int> iconLack = new List<int>();

            foreach (string effectsProp in effects_prop)
            {

                string[] effectData = effectsProp.Split(',');
                int effectID = int.Parse(effectData[0]);
                //     long endTime = long.Parse(effectData[3]);
                //long serverTime = ClientAPI.ScriptObject.GetComponent<TimeManager>().ServerTime;
                //long timeTillEnd = endTime - serverTime;
                long duration = long.Parse(effectData[4]);
                //if (timeTillEnd < duration)
                //	duration = timeTillEnd;
                bool active = bool.Parse(effectData[5]);
                float secondsLeft = (float)duration / 1000f;
                long length = long.Parse(effectData[6]);

                //ClientAPI.Write("Effect will last for: " + secondsLeft + " seconds, starting at time: " + Time.realtimeSinceStartup);
                AtavismEffect effect = null;
             /*   foreach (AtavismEffect aEffect in aEffects)
                {
                    if (aEffect.id.Equals(effectID))
                    {
                        effect = aEffect;
                        break;
                    }
                }*/
                if (effect == null)
                    if (Abilities.Instance.GetEffect(effectID) != null)
                        effect = Abilities.Instance.GetEffect(effectID).Clone();
                if (effect == null)
                {
                    UnityEngine.Debug.LogWarning("Effect " + effectID + " does not exist");
                    continue;
                }
                effect.StackSize = int.Parse(effectData[1]);
                effect.isBuff = bool.Parse(effectData[2]);
                effect.Expiration = Time.time + secondsLeft - (Time.time - effects_prop_time);
                effect.Active = active;
                effect.Length = (float)length / 1000f;
                effect.startTime = long.Parse(effectData[9]);

                targetEffects.Add(effect);
                if (effect.icon == null)
                {
                    iconLack.Add(effect.id);
                }
            }
            targetEffects = targetEffects.OrderBy(x => x.startTime).ToList();

            if (iconLack.Count > 0)
            {
                string s = "";
                foreach (int id in iconLack)
                {
                    s += id + ";";
                }

                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("objs", s);
                AtavismClient.Instance.NetworkHelper.GetIconPrefabs(ps, "EffectIcon");
                //NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.GET_EFFECT_ICON", ps);
            }

            UpdateEffects();
        }
        private void UpdateEffects()
        {
            for (int i = 0; i < effectButtons.Count; i++)
            {
                if (i < targetEffects.Count)
                {
                    if (activeEffect && targetEffects[i].Active == false)
                    {
                        effectButtons[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        effectButtons[i].gameObject.SetActive(true);
                        effectButtons[i].SetEffect(targetEffects[i], i);
                    }
                }
                else
                {
                    effectButtons[i].gameObject.SetActive(false);
                }
            }
        }

        private void Update()
        {
            if (ClientAPI.GetTargetObject() == null)
            {
                node = ClientAPI.GetObjectNode(targetOID);
                if (node != null)
                {
                    node.RemovePropertyChangeHandler("effects", EffectsPropertyHandler);
                    node.RemovePropertyChangeHandler("level", LevelPropertyHandler);
                    node.RemovePropertyChangeHandler("reaction", ReactionPropertyHandler);
                    node.RemovePropertyChangeHandler("aggressive", ReactionPropertyHandler);
                    node = null;
                    targetOID = 0;
                    targetEffects.Clear();
                    UpdateEffects();
                }
            }
            else
            {
                UpdatePortrait();
            }
        }
    }

}