using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{
    public class UGUIResourceLootListEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        //public Text itemNameText;
        public Text itemNameText;
        public TextMeshProUGUI itemNameTextTMP;
        public Image itemIcon;
        public Text countText;
        public TextMeshProUGUI countTextTMP;
        ResourceItem resource;
        public Image itemQuality;

        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("ITEM_ICON_UPDATE", this);

        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("ITEM_ICON_UPDATE", this);
        
        }
        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "ITEM_ICON_UPDATE")
            {
                if (this.resource != null && this.resource.item != null)
                {
                    if (AtavismPrefabManager.Instance.GetItemIconByID(resource.item.templateId) != null)
                    {
                        this.itemIcon.sprite = AtavismPrefabManager.Instance.GetItemIconByID(resource.item.templateId);
                    }
                    else
                        this.itemIcon.sprite = AtavismSettings.Instance.defaultItemIcon;
                }
            }
           
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseEntered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseEntered = false;
        }

        public void LootEntryClicked()
        {
            Crafting.Instance.LootResource(resource.item);
        }

        public void SetResourceLootEntryDetails(ResourceItem resourceItem)
        {
            this.resource = resourceItem;
            if (itemNameText != null)
            {
#if AT_I2LOC_PRESET
                this.itemNameText.text = I2.Loc.LocalizationManager.GetTranslation("Items/" + resource.item.name);
#else
                this.itemNameText.text = resource.item.name;
#endif
                this.itemNameText.color = AtavismSettings.Instance.ItemQualityColor(resource.item.quality);
            }
            if (itemNameTextTMP != null)
            {
#if AT_I2LOC_PRESET
                this.itemNameTextTMP.text = I2.Loc.LocalizationManager.GetTranslation("Items/" + resource.item.name);
#else
                this.itemNameTextTMP.text = resource.item.name;
#endif
                this.itemNameTextTMP.color = AtavismSettings.Instance.ItemQualityColor(resource.item.quality);
            }
            if (itemQuality)
                itemQuality.color = AtavismSettings.Instance.ItemQualityColor(resource.item.quality);
            if (resource.item.icon != null)
                this.itemIcon.sprite = resource.item.icon;
            else
                this.itemIcon.sprite = AtavismSettings.Instance.defaultItemIcon;

          //  this.itemIcon.sprite = resource.item.icon;
            if (countText != null)
            {
                if (resource.count > 1)
                {
                    if (countText != null)
                        this.countText.text = resource.count.ToString();
                }
                else
                {
                    if (countText != null)
                        this.countText.text = "";
                }
            }
            if (countTextTMP != null)
            {
                if (resource.count > 1)
                {
                    if (countTextTMP != null)
                        this.countTextTMP.text = resource.count.ToString();
                }
                else
                {
                    if (countTextTMP != null)
                        this.countTextTMP.text = "";
                }
            }
        }

        void HideTooltip()
        {
            UGUITooltip.Instance.Hide();
        }

        public bool MouseEntered
        {
            set
            {
                if (value)
                {
                    resource.item.ShowTooltip(gameObject);
                }
                else
                {
                    HideTooltip();
                }
            }
        }
    }
}