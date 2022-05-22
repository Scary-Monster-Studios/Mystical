using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Atavism
{

    public class UGUIControls : MonoBehaviour
    {
        [AtavismSeparator("Key Settings")]
        [SerializeField] Text strafeLeftText;
        [SerializeField] TextMeshProUGUI TMPStrafeLeftText;
        [SerializeField] Text strafeRightText;
        [SerializeField] TextMeshProUGUI TMPStrafeRightText;
        [SerializeField] Text moveForwardText;
        [SerializeField] TextMeshProUGUI TMPMoveForwardText;
        [SerializeField] Text altMoveForwardText;
        [SerializeField] TextMeshProUGUI TMPAltMoveForwardText;
        [SerializeField] Text moveBackwardText;
        [SerializeField] TextMeshProUGUI TMPMoveBackwardText;
        [SerializeField] Text altMoveBackwardText;
        [SerializeField] TextMeshProUGUI TMPAltMoveBackwardText;
        [SerializeField] Text turnLeftText;
        [SerializeField] TextMeshProUGUI TMPTurnLeftText;
        [SerializeField] Text altTurnLeftText;
        [SerializeField] TextMeshProUGUI TMPAltTurnLeftText;
        [SerializeField] Text turnRightText;
        [SerializeField] TextMeshProUGUI TMPTurnRightText;
        [SerializeField] Text altTurnRightText;
        [SerializeField] TextMeshProUGUI TMPAltTurnRightText;
        [SerializeField] Text autoRunText;
        [SerializeField] TextMeshProUGUI TMPAutoRunText;
        [SerializeField] Text walkRunText;
        [SerializeField] TextMeshProUGUI TMPWalkRunText;
        [SerializeField] Text jumpText;
        [SerializeField] TextMeshProUGUI TMPJumpText;
        [SerializeField] Text showWeaponText;
        [SerializeField] TextMeshProUGUI TMPShowWeaponText;
        [SerializeField] Text inventoryText;
        [SerializeField] TextMeshProUGUI TMPInventoryText;
        [SerializeField] Text characterText;
        [SerializeField] TextMeshProUGUI TMPCharacterText;
        [SerializeField] Text mailText;
        [SerializeField] TextMeshProUGUI TMPMailText;
        [SerializeField] Text guildText;
        [SerializeField] TextMeshProUGUI TMPGuildText;
        [SerializeField] Text questText;
        [SerializeField] TextMeshProUGUI TMPQuestText;
        [SerializeField] Text skillsText;
        [SerializeField] TextMeshProUGUI TMPSkillsText;
        [SerializeField] Text mapText;
        [SerializeField] TextMeshProUGUI TMPMapText;
        [SerializeField] Text arenaText;
        [SerializeField] TextMeshProUGUI TMPArenaText;
        [SerializeField] Text socialText;
        [SerializeField] TextMeshProUGUI TMPSocialText;
        [SerializeField] Text sprintText;
        [SerializeField] TextMeshProUGUI TMPSprintText;
        [SerializeField] Text lootText;
        [SerializeField] TextMeshProUGUI TMPLootText;
        [SerializeField] GameObject changeInfoPanel;

        string currentKey = "";
        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("KEY_UPDATE_VIEW", this);
        }
        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("KEY_UPDATE_VIEW", this);

        }
        // Update is called once per frame
        void Update()
        {

        }
        void OnEnable()
        {
            UpdateViewKeys();
        }
        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "KEY_UPDATE_VIEW")
            {
                UpdateViewKeys();
            }
        }

        public void UpdateViewKeys()
        {
            if (AtavismSettings.Instance != null && AtavismSettings.Instance.GetKeySettings() != null)
            {
#if AT_I2LOC_PRESET
            if(strafeLeftText!=null)
                strafeLeftText.text = AtavismSettings.Instance.GetKeySettings().strafeLeft.ToString().Length>2?I2.Loc.LocalizationManager.GetTranslation( AtavismSettings.Instance.GetKeySettings().strafeLeft.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().strafeLeft.ToString().ToUpper();
            if(TMPStrafeLeftText!=null)
                TMPStrafeLeftText.text = AtavismSettings.Instance.GetKeySettings().strafeLeft.ToString().Length>2?I2.Loc.LocalizationManager.GetTranslation( AtavismSettings.Instance.GetKeySettings().strafeLeft.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().strafeLeft.ToString().ToUpper();
            if (strafeRightText != null)
                strafeRightText.text = AtavismSettings.Instance.GetKeySettings().strafeRight.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().strafeRight.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().strafeRight.ToString().ToUpper();
            if (TMPStrafeRightText != null)
                TMPStrafeRightText.text = AtavismSettings.Instance.GetKeySettings().strafeRight.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().strafeRight.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().strafeRight.ToString().ToUpper();
            if (moveForwardText != null)
                moveForwardText.text = AtavismSettings.Instance.GetKeySettings().moveForward.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().moveForward.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().moveForward.ToString().ToUpper();
            if (TMPMoveForwardText != null)
                TMPMoveForwardText.text = AtavismSettings.Instance.GetKeySettings().moveForward.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().moveForward.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().moveForward.ToString().ToUpper();
            if (altMoveForwardText != null)
                altMoveForwardText.text = AtavismSettings.Instance.GetKeySettings().altMoveForward.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().altMoveForward.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().altMoveForward.ToString().ToUpper();
            if (TMPAltMoveForwardText != null)
                TMPAltMoveForwardText.text = AtavismSettings.Instance.GetKeySettings().altMoveForward.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().altMoveForward.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().altMoveForward.ToString().ToUpper();
            if (moveBackwardText != null)
                moveBackwardText.text = AtavismSettings.Instance.GetKeySettings().moveBackward.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().moveBackward.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().moveBackward.ToString().ToUpper();
            if (TMPMoveBackwardText != null)
                TMPMoveBackwardText.text = AtavismSettings.Instance.GetKeySettings().moveBackward.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().moveBackward.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().moveBackward.ToString().ToUpper();
            if (altMoveBackwardText != null)
                altMoveBackwardText.text = AtavismSettings.Instance.GetKeySettings().altMoveBackward.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().altMoveBackward.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().altMoveBackward.ToString().ToUpper();
            if (TMPAltMoveBackwardText != null)
                TMPAltMoveBackwardText.text = AtavismSettings.Instance.GetKeySettings().altMoveBackward.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().altMoveBackward.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().altMoveBackward.ToString().ToUpper();
            if (turnLeftText != null)
                turnLeftText.text = AtavismSettings.Instance.GetKeySettings().turnLeft.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().turnLeft.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().turnLeft.ToString().ToUpper();
            if (TMPTurnLeftText != null)
                TMPTurnLeftText.text = AtavismSettings.Instance.GetKeySettings().turnLeft.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().turnLeft.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().turnLeft.ToString().ToUpper();
            if (altTurnLeftText != null)
                altTurnLeftText.text = AtavismSettings.Instance.GetKeySettings().altTurnLeft.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().altTurnLeft.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().altTurnLeft.ToString().ToUpper();
            if (TMPAltTurnLeftText != null)
                TMPAltTurnLeftText.text = AtavismSettings.Instance.GetKeySettings().altTurnLeft.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().altTurnLeft.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().altTurnLeft.ToString().ToUpper();
            if (turnRightText != null)
                turnRightText.text = AtavismSettings.Instance.GetKeySettings().turnRight.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().turnRight.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().turnRight.ToString().ToUpper();
            if (TMPTurnRightText != null)
                TMPTurnRightText.text = AtavismSettings.Instance.GetKeySettings().turnRight.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().turnRight.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().turnRight.ToString().ToUpper();
            if (altTurnRightText != null)
                altTurnRightText.text = AtavismSettings.Instance.GetKeySettings().altTurnRight.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().altTurnRight.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().altTurnRight.ToString().ToUpper();
            if (TMPAltTurnRightText != null)
                TMPAltTurnRightText.text = AtavismSettings.Instance.GetKeySettings().altTurnRight.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().altTurnRight.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().altTurnRight.ToString().ToUpper();
            if (autoRunText != null)
                autoRunText.text = AtavismSettings.Instance.GetKeySettings().autoRun.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().autoRun.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().autoRun.ToString().ToUpper();
            if (TMPAutoRunText != null)
                TMPAutoRunText.text = AtavismSettings.Instance.GetKeySettings().autoRun.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().autoRun.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().autoRun.ToString().ToUpper();
            if (walkRunText != null)
                walkRunText.text = AtavismSettings.Instance.GetKeySettings().walkRun.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().walkRun.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().walkRun.ToString().ToUpper();
            if (TMPWalkRunText != null)
                TMPWalkRunText.text = AtavismSettings.Instance.GetKeySettings().walkRun.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().walkRun.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().walkRun.ToString().ToUpper();
            if (jumpText != null)
                jumpText.text = AtavismSettings.Instance.GetKeySettings().jump.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().jump.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().jump.ToString().ToUpper();
            if (TMPJumpText != null)
                TMPJumpText.text = AtavismSettings.Instance.GetKeySettings().jump.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().jump.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().jump.ToString().ToUpper();
            if (showWeaponText != null)
                showWeaponText.text = AtavismSettings.Instance.GetKeySettings().showWeapon.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().showWeapon.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().showWeapon.ToString().ToUpper();
            if (TMPShowWeaponText != null)
                TMPShowWeaponText.text = AtavismSettings.Instance.GetKeySettings().showWeapon.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().showWeapon.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().showWeapon.ToString().ToUpper();
            if (inventoryText != null)
                inventoryText.text = AtavismSettings.Instance.GetKeySettings().inventory.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().inventory.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().inventory.ToString().ToUpper();
            if (TMPInventoryText != null)
                TMPInventoryText.text = AtavismSettings.Instance.GetKeySettings().inventory.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().inventory.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().inventory.ToString().ToUpper();
            if (characterText != null)
                characterText.text = AtavismSettings.Instance.GetKeySettings().character.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().character.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().character.ToString().ToUpper();
            if (TMPCharacterText != null)
                TMPCharacterText.text = AtavismSettings.Instance.GetKeySettings().character.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().character.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().character.ToString().ToUpper();
            if (mailText != null)
                mailText.text = AtavismSettings.Instance.GetKeySettings().mail.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().mail.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().mail.ToString().ToUpper();
            if (TMPMailText != null)
                TMPMailText.text = AtavismSettings.Instance.GetKeySettings().mail.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().mail.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().mail.ToString().ToUpper();
            if (guildText != null)
                guildText.text = AtavismSettings.Instance.GetKeySettings().guild.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().guild.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().guild.ToString().ToUpper();
            if (TMPGuildText != null)
                TMPGuildText.text = AtavismSettings.Instance.GetKeySettings().guild.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().guild.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().guild.ToString().ToUpper();
            if (questText != null)
                questText.text = AtavismSettings.Instance.GetKeySettings().quest.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().quest.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().quest.ToString().ToUpper();
            if (TMPQuestText != null)
                TMPQuestText.text = AtavismSettings.Instance.GetKeySettings().quest.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().quest.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().quest.ToString().ToUpper();
            if (skillsText != null)
                skillsText.text = AtavismSettings.Instance.GetKeySettings().skills.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().skills.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().skills.ToString().ToUpper();
            if (TMPSkillsText != null)
                TMPSkillsText.text = AtavismSettings.Instance.GetKeySettings().skills.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().skills.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().skills.ToString().ToUpper();
            if (mapText != null)
                mapText.text = AtavismSettings.Instance.GetKeySettings().map.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().map.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().map.ToString().ToUpper();
            if (TMPMapText != null)
                TMPMapText.text = AtavismSettings.Instance.GetKeySettings().map.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().map.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().map.ToString().ToUpper();
           if (arenaText != null)
                arenaText.text = AtavismSettings.Instance.GetKeySettings().arena.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().arena.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().arena.ToString().ToUpper();
           if (TMPArenaText != null)
                TMPArenaText.text = AtavismSettings.Instance.GetKeySettings().arena.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().arena.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().arena.ToString().ToUpper();
           if (socialText != null)
                arenaText.text = AtavismSettings.Instance.GetKeySettings().social.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().social.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().social.ToString().ToUpper();
           if (TMPSocialText != null)
                TMPSocialText.text = AtavismSettings.Instance.GetKeySettings().social.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().social.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().social.ToString().ToUpper();
           if (sprintText != null)
                sprintText.text = AtavismSettings.Instance.GetKeySettings().sprint.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().sprint.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().sprint.ToString().ToUpper();
           if (TMPSprintText != null)
                TMPSprintText.text = AtavismSettings.Instance.GetKeySettings().sprint.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().sprint.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().sprint.ToString().ToUpper();
          if (lootText != null)
                lootText.text = AtavismSettings.Instance.GetKeySettings().loot.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().loot.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().loot.ToString().ToUpper();
           if (TMPLootText != null)
                TMPLootText.text = AtavismSettings.Instance.GetKeySettings().loot.ToString().Length > 2 ? I2.Loc.LocalizationManager.GetTranslation(AtavismSettings.Instance.GetKeySettings().loot.ToString()).ToUpper() : AtavismSettings.Instance.GetKeySettings().loot.ToString().ToUpper();

#else
                if (strafeLeftText != null)
                    strafeLeftText.text = AtavismSettings.Instance.GetKeySettings().strafeLeft.ToString().ToUpper();
                if (TMPStrafeLeftText != null)
                    TMPStrafeLeftText.text = AtavismSettings.Instance.GetKeySettings().strafeLeft.ToString().ToUpper();
                if (strafeRightText != null)
                    strafeRightText.text = AtavismSettings.Instance.GetKeySettings().strafeRight.ToString().ToUpper();
                if (TMPStrafeRightText != null)
                    TMPStrafeRightText.text = AtavismSettings.Instance.GetKeySettings().strafeRight.ToString().ToUpper();
                if (moveForwardText != null)
                    moveForwardText.text = AtavismSettings.Instance.GetKeySettings().moveForward.ToString().ToUpper();
                if (TMPMoveForwardText != null)
                    TMPMoveForwardText.text = AtavismSettings.Instance.GetKeySettings().moveForward.ToString().ToUpper();
                if (altMoveForwardText != null)
                    altMoveForwardText.text = AtavismSettings.Instance.GetKeySettings().altMoveForward.ToString().ToUpper();
                if (TMPAltMoveForwardText != null)
                    TMPAltMoveForwardText.text = AtavismSettings.Instance.GetKeySettings().altMoveForward.ToString().ToUpper();
                if (moveBackwardText != null)
                    moveBackwardText.text = AtavismSettings.Instance.GetKeySettings().moveBackward.ToString().ToUpper();
                if (TMPMoveBackwardText != null)
                    TMPMoveBackwardText.text = AtavismSettings.Instance.GetKeySettings().moveBackward.ToString().ToUpper();
                if (altMoveBackwardText != null)
                    altMoveBackwardText.text = AtavismSettings.Instance.GetKeySettings().altMoveBackward.ToString().ToUpper();
                if (TMPAltMoveBackwardText != null)
                    TMPAltMoveBackwardText.text = AtavismSettings.Instance.GetKeySettings().altMoveBackward.ToString().ToUpper();
                if (turnLeftText != null)
                    turnLeftText.text = AtavismSettings.Instance.GetKeySettings().turnLeft.ToString().ToUpper();
                if (TMPTurnLeftText != null)
                    TMPTurnLeftText.text = AtavismSettings.Instance.GetKeySettings().turnLeft.ToString().ToUpper();
                if (altTurnLeftText != null)
                    altTurnLeftText.text = AtavismSettings.Instance.GetKeySettings().altTurnLeft.ToString().ToUpper();
                if (TMPAltTurnLeftText != null)
                    TMPAltTurnLeftText.text = AtavismSettings.Instance.GetKeySettings().altTurnLeft.ToString().ToUpper();
                if (turnRightText != null)
                    turnRightText.text = AtavismSettings.Instance.GetKeySettings().turnRight.ToString().ToUpper();
                if (TMPTurnRightText != null)
                    TMPTurnRightText.text = AtavismSettings.Instance.GetKeySettings().turnRight.ToString().ToUpper();
                if (altTurnRightText != null)
                    altTurnRightText.text = AtavismSettings.Instance.GetKeySettings().altTurnRight.ToString().ToUpper();
                if (TMPAltTurnRightText != null)
                    TMPAltTurnRightText.text = AtavismSettings.Instance.GetKeySettings().altTurnRight.ToString().ToUpper();
                if (autoRunText != null)
                    autoRunText.text = AtavismSettings.Instance.GetKeySettings().autoRun.ToString().ToUpper();
                if (TMPAutoRunText != null)
                    TMPAutoRunText.text = AtavismSettings.Instance.GetKeySettings().autoRun.ToString().ToUpper();
                if (walkRunText != null)
                    walkRunText.text = AtavismSettings.Instance.GetKeySettings().walkRun.ToString().ToUpper();
                if (TMPWalkRunText != null)
                    TMPWalkRunText.text = AtavismSettings.Instance.GetKeySettings().walkRun.ToString().ToUpper();
                if (jumpText != null)
                    jumpText.text = AtavismSettings.Instance.GetKeySettings().jump.ToString().ToUpper();
                if (TMPJumpText != null)
                    TMPJumpText.text = AtavismSettings.Instance.GetKeySettings().jump.ToString().ToUpper();
                if (showWeaponText != null)
                    showWeaponText.text = AtavismSettings.Instance.GetKeySettings().showWeapon.ToString().ToUpper();
                if (TMPShowWeaponText != null)
                    TMPShowWeaponText.text = AtavismSettings.Instance.GetKeySettings().showWeapon.ToString().ToUpper();
                if (inventoryText != null)
                    inventoryText.text = AtavismSettings.Instance.GetKeySettings().inventory.ToString().ToUpper();
                if (TMPInventoryText != null)
                    TMPInventoryText.text = AtavismSettings.Instance.GetKeySettings().inventory.ToString().ToUpper();
                if (characterText != null)
                    characterText.text = AtavismSettings.Instance.GetKeySettings().character.ToString().ToUpper();
                if (TMPCharacterText != null)
                    TMPCharacterText.text = AtavismSettings.Instance.GetKeySettings().character.ToString().ToUpper();
                if (mailText != null)
                    mailText.text = AtavismSettings.Instance.GetKeySettings().mail.ToString().ToUpper();
                if (TMPMailText != null)
                    TMPMailText.text = AtavismSettings.Instance.GetKeySettings().mail.ToString().ToUpper();
                if (guildText != null)
                    guildText.text = AtavismSettings.Instance.GetKeySettings().guild.ToString().ToUpper();
                if (TMPGuildText != null)
                    TMPGuildText.text = AtavismSettings.Instance.GetKeySettings().guild.ToString().ToUpper();
                if (questText != null)
                    questText.text = AtavismSettings.Instance.GetKeySettings().quest.ToString().ToUpper();
                if (TMPQuestText != null)
                    TMPQuestText.text = AtavismSettings.Instance.GetKeySettings().quest.ToString().ToUpper();
                if (skillsText != null)
                    skillsText.text = AtavismSettings.Instance.GetKeySettings().skills.ToString().ToUpper();
                if (TMPSkillsText != null)
                    TMPSkillsText.text = AtavismSettings.Instance.GetKeySettings().skills.ToString().ToUpper();
                if (mapText != null)
                    mapText.text = AtavismSettings.Instance.GetKeySettings().map.ToString().ToUpper();
                if (TMPMapText != null)
                    TMPMapText.text = AtavismSettings.Instance.GetKeySettings().map.ToString().ToUpper();
                if (arenaText != null)
                    arenaText.text = AtavismSettings.Instance.GetKeySettings().arena.ToString().ToUpper();
                if (TMPArenaText != null)
                    TMPArenaText.text = AtavismSettings.Instance.GetKeySettings().arena.ToString().ToUpper();
                if (socialText != null)
                    socialText.text = AtavismSettings.Instance.GetKeySettings().social.ToString().ToUpper();
                if (TMPSocialText != null)
                    TMPSocialText.text = AtavismSettings.Instance.GetKeySettings().social.ToString().ToUpper();
                if (sprintText != null)
                    sprintText.text = AtavismSettings.Instance.GetKeySettings().sprint.ToString().ToUpper();
                if (TMPSprintText != null)
                    TMPSprintText.text = AtavismSettings.Instance.GetKeySettings().sprint.ToString().ToUpper();
                if (lootText != null)
                    lootText.text = AtavismSettings.Instance.GetKeySettings().loot.ToString().ToUpper();
                if (TMPLootText != null)
                    TMPLootText.text = AtavismSettings.Instance.GetKeySettings().loot.ToString().ToUpper();

#endif

            }
        }
        private void OnGUI()
        {
            if (!string.IsNullOrEmpty(currentKey) )
            {

                Event e = Event.current;
                if (e.isKey)
                {
                    switch (currentKey)
                    {
                        case "strafeLeft":
                            AtavismSettings.Instance.GetKeySettings().strafeLeft = e.keyCode;
                            break;
                        case "strafeRight":
                            AtavismSettings.Instance.GetKeySettings().strafeRight = e.keyCode;
                            break;
                        case "moveForward":
                            AtavismSettings.Instance.GetKeySettings().moveForward = e.keyCode;
                            break;
                        case "altMoveForward":
                            AtavismSettings.Instance.GetKeySettings().altMoveForward = e.keyCode;
                            break;
                        case "moveBackward":
                            AtavismSettings.Instance.GetKeySettings().moveBackward = e.keyCode;
                            break;
                        case "altMoveBackward":
                            AtavismSettings.Instance.GetKeySettings().altMoveBackward = e.keyCode;
                            break;
                        case "turnLeft":
                            AtavismSettings.Instance.GetKeySettings().turnLeft = e.keyCode;
                            break;
                        case "altTurnLeft":
                            AtavismSettings.Instance.GetKeySettings().altTurnLeft = e.keyCode;
                            break;
                        case "turnRight":
                            AtavismSettings.Instance.GetKeySettings().turnRight = e.keyCode;
                            break;
                        case "altTurnRight":
                            AtavismSettings.Instance.GetKeySettings().altTurnRight = e.keyCode;
                            break;
                        case "autoRun":
                            AtavismSettings.Instance.GetKeySettings().autoRun = e.keyCode;
                            break;
                        case "walkRun":
                            AtavismSettings.Instance.GetKeySettings().walkRun = e.keyCode;
                            break;
                        case "jump":
                            AtavismSettings.Instance.GetKeySettings().jump = e.keyCode;
                            break;
                        case "showWeapon":
                            AtavismSettings.Instance.GetKeySettings().showWeapon = e.keyCode;
                            break;
                        case "inventory":
                            AtavismSettings.Instance.GetKeySettings().inventory = e.keyCode;
                            break;
                        case "character":
                            AtavismSettings.Instance.GetKeySettings().character = e.keyCode;
                            break;
                        case "mail":
                            AtavismSettings.Instance.GetKeySettings().mail = e.keyCode;
                            break;
                        case "guild":
                            AtavismSettings.Instance.GetKeySettings().guild = e.keyCode;
                            break;
                        case "quest":
                            AtavismSettings.Instance.GetKeySettings().quest = e.keyCode;
                            break;
                        case "skills":
                            AtavismSettings.Instance.GetKeySettings().skills = e.keyCode;
                            break;
                        case "map":
                            AtavismSettings.Instance.GetKeySettings().map = e.keyCode;
                            break;
                        case "arena":
                            AtavismSettings.Instance.GetKeySettings().arena = e.keyCode;
                            break;
                        case "social":
                            AtavismSettings.Instance.GetKeySettings().social = e.keyCode;
                            break;
                        case "sprint":
                            AtavismSettings.Instance.GetKeySettings().sprint = e.keyCode;
                            break;
                        case "loot":
                            AtavismSettings.Instance.GetKeySettings().loot = e.keyCode;
                            break;
                        default:
                            Debug.LogError("UGUIControls parametr "+ currentKey +" not defined");
                            break;
                      /*  case "":
                            AtavismSettings.Instance.GetKeySettings().  = e.keyCode;
                            break;*/
                    }
                    currentKey = "";
                    UpdateViewKeys();
                    if (changeInfoPanel)
                        changeInfoPanel.SetActive(false);

                }
            }
        }

        public void ChangeKey(string s)
        {
            currentKey = s;
            if (changeInfoPanel)
                changeInfoPanel.SetActive(true);
        }
    }
}