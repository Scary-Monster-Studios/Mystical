using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUIDialogueOption : MonoBehaviour
    {

        public Image icon;
        public Image itemIcon;
        public Image currencyIcon;
        public Text optionText;
        public TextMeshProUGUI TMPOptionText;

        public Sprite newQuestSprite;
        public Sprite progressQuestSprite;
        public Sprite dialogueSprite;
        public Sprite merchantSprite;
        public Sprite bankSprite;
        public Sprite repairSprite;
        public Sprite abilitySprite;
        public Sprite auctionSprite;
        public Sprite mailSprite;
        public Sprite gearModificationSprite;
        public Sprite guildWarehouseSprite;

        NpcInteractionEntry interaction;

        // Use this for initialization
        void Start()
        {

        }

        public void SetNpcInteraction(NpcInteractionEntry interaction)
        {
            this.interaction = interaction;
#if AT_I2LOC_PRESET
      //  Debug.LogError(interaction.interactionTitle + " ||||| " + interaction.interactionTitle.IndexOf("(Complete)"));
        if (interaction.interactionTitle.IndexOf("(Repeatable)") > 0)
        {
           // Debug.LogError(interaction.interactionTitle + " ||||| " + interaction.interactionTitle.IndexOf("(Complete)") + " ||||| " + interaction.interactionTitle.Substring(0, interaction.interactionTitle.IndexOf("(Complete)") - 1));

            if (optionText != null) this.optionText.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + interaction.interactionTitle.Substring(0, interaction.interactionTitle.IndexOf("(Repeatable)") - 1)) + " (" + I2.Loc.LocalizationManager.GetTranslation("Repeatable") + ")";
            if (TMPOptionText != null) this.TMPOptionText.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + interaction.interactionTitle.Substring(0, interaction.interactionTitle.IndexOf("(Repeatable)") - 1)) + " (" + I2.Loc.LocalizationManager.GetTranslation("Repeatable") + ")";
        }
        else if(interaction.interactionTitle.IndexOf("(Complete)") > 0)
        {
         //   Debug.LogError(interaction.interactionTitle + " ||||| " + interaction.interactionTitle.IndexOf("(Complete)")+" ||||| "+ interaction.interactionTitle.Substring(0, interaction.interactionTitle.IndexOf("(Complete)") - 1));

            if (optionText != null) this.optionText.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + interaction.interactionTitle.Substring(0, interaction.interactionTitle.IndexOf("(Complete)") - 1)) + " (" + I2.Loc.LocalizationManager.GetTranslation("Complete") + ")";
            if (TMPOptionText != null) this.TMPOptionText.text = I2.Loc.LocalizationManager.GetTranslation("Quests/" + interaction.interactionTitle.Substring(0, interaction.interactionTitle.IndexOf("(Complete)") - 1)) + " (" + I2.Loc.LocalizationManager.GetTranslation("Complete") + ")";
        }
        else
        {

            if (optionText != null) this.optionText.text = string.IsNullOrEmpty(I2.Loc.LocalizationManager.GetTranslation("Quests/" + interaction.interactionTitle)) ? interaction.interactionTitle : I2.Loc.LocalizationManager.GetTranslation("Quests/" + interaction.interactionTitle);
            if (TMPOptionText != null) this.TMPOptionText.text = string.IsNullOrEmpty(I2.Loc.LocalizationManager.GetTranslation("Quests/" + interaction.interactionTitle)) ? interaction.interactionTitle : I2.Loc.LocalizationManager.GetTranslation("Quests/" + interaction.interactionTitle);
        }
#else
            if (optionText != null)
                this.optionText.text = interaction.interactionTitle;
            if (TMPOptionText != null)
                this.TMPOptionText.text = interaction.interactionTitle;
#endif

            if (interaction.interactionType == "offered_quest" || interaction.interactionType == "Quest")
            {
                if (newQuestSprite != null)
                {
                    icon.enabled = true;
                    icon.sprite = newQuestSprite;
                }
                else
                {
                    icon.enabled = false;
                }
            }
            else if (interaction.interactionType == "progress_quest" || interaction.interactionType == "QuestPregerss")
            {
                if (progressQuestSprite != null)
                {
                    icon.enabled = true;
                    icon.sprite = progressQuestSprite;
                }
                else
                {
                    icon.enabled = false;
                }
            }
            else if (interaction.interactionType == "dialogue" || interaction.interactionType == "Ability")
            {
                if (dialogueSprite != null)
                {
                    icon.enabled = true;
                    icon.sprite = dialogueSprite;
                }
                else
                {
                    icon.enabled = false;
                }
            }
            else if (interaction.interactionType == "merchant" || interaction.interactionType == "Merchant")
            {
                if (merchantSprite != null)
                {
                    icon.enabled = true;
                    icon.sprite = merchantSprite;
                }
                else
                {
                    icon.enabled = false;
                }
            }
            else if (interaction.interactionType == "Bank")
            {
                if (bankSprite != null)
                {
                    icon.enabled = true;
                    icon.sprite = bankSprite;
                }
                else
                {
                    icon.enabled = false;
                }
            }
            else if (interaction.interactionType == "Repair")
            {
                if (repairSprite != null)
                {
                    icon.enabled = true;
                    icon.sprite = repairSprite;
                }
                else
                {
                    icon.enabled = false;
                }
            }
            else if (interaction.interactionType == "Ability")
            {
                if (abilitySprite != null)
                {
                    icon.enabled = true;
                    icon.sprite = abilitySprite;
                }
                else
                {
                    icon.enabled = false;
                }
            }
            else if (interaction.interactionType == "Auction")
            {
                if (auctionSprite != null)
                {
                    icon.enabled = true;
                    icon.sprite = auctionSprite;
                }
                else
                {
                    icon.enabled = false;
                }
            }
            else if (interaction.interactionType == "Mail")
            {
                if (mailSprite != null)
                {
                    icon.enabled = true;
                    icon.sprite = mailSprite;
                }
                else
                {
                    icon.enabled = false;
                }
            }
            else if (interaction.interactionType == "GearModification")
            {
                if (gearModificationSprite != null)
                {
                    icon.enabled = true;
                    icon.sprite = gearModificationSprite;
                }
                else
                {
                    icon.enabled = false;
                }
            }
            else if (interaction.interactionType == "GuildWarehouse")
            {
                if (guildWarehouseSprite != null)
                {
                    icon.enabled = true;
                    icon.sprite = guildWarehouseSprite;
                }
                else
                {
                    icon.enabled = false;
                }
            }
            else
            {
                icon.enabled = false;
            }
            if (currencyIcon != null)
                if (interaction.currency > 0 && interaction.currencyAmmount > 0)
                {
                    currencyIcon.enabled = true;
                    string curr = Inventory.Instance.GetCostString(interaction.currency, interaction.currencyAmmount);
                    UGUIMiniTooltipEvent mte = currencyIcon.transform.GetComponent<UGUIMiniTooltipEvent>();
                    if (mte != null)
                        mte.dectName = curr;
                }
                else
                {
                    currencyIcon.enabled = false;
                }
            if (itemIcon != null)
                if (interaction.itemId > 0 )
                {
                    itemIcon.enabled = true;
                    UGUIItemDisplay uid = itemIcon.transform.GetComponent<UGUIItemDisplay>();
                    if (uid != null)
                    {
                        AtavismInventoryItem aii = AtavismPrefabManager.Instance.LoadItem(interaction.itemId);
                        if (aii != null)
                            uid.SetItemData(aii, null);
                    }
                }
                else
                {
                    itemIcon.enabled = false;
                }


        }

        public void ActionClicked()
        {
            interaction.StartInteraction();
        }
    }
}