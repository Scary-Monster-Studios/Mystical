using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUILootListEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        //	public Text itemNameText;
        public Text itemNameText;
        public TextMeshProUGUI itemNameTextTMP;
        public Image itemIcon;
        public Text countText;
        public TextMeshProUGUI countTextTMP;
        public Image itemQuality;
        AtavismInventoryItem item;
        Currency curr;
        // Use this for initialization
        void Start()
        {

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
            if(item!=null)
                NetworkAPI.SendTargetedCommand(Inventory.Instance.LootTarget.ToLong(), "/lootItem " + item.ItemId);

            if (curr != null)
                NetworkAPI.SendTargetedCommand(Inventory.Instance.LootTarget.ToLong(), "/lootItem " + curr.id);
        }

        public void SetLootEntryDetails(AtavismInventoryItem item)
        {
            this.item = item;
            this.curr = null;
#if AT_I2LOC_PRESET
      if (itemNameText != null)  this.itemNameText.text = I2.Loc.LocalizationManager.GetTranslation("Items/" + item.name);
      if (itemNameTextTMP != null)  this.itemNameTextTMP.text = I2.Loc.LocalizationManager.GetTranslation("Items/" + item.name);
#else
            if (itemNameText != null)
                this.itemNameText.text = item.name;
            if (itemNameTextTMP != null)
                this.itemNameTextTMP.text = item.name;
#endif
            if (item.icon != null)
                this.itemIcon.sprite = item.icon;
            else
                this.itemIcon.sprite = AtavismSettings.Instance.defaultItemIcon;

           // this.itemIcon.sprite = item.icon;
            if (countText != null)
            {
                if (item.Count > 1)
                {
                    this.countText.text = item.Count.ToString();
                }
                else
                {
                    this.countText.text = "";
                }
            }
            if (countTextTMP != null)
            {
                if (item.Count > 1)
                {
                    this.countTextTMP.text = item.Count.ToString();
                }
                else
                {
                    this.countTextTMP.text = "";
                }
            }

            if (itemQuality != null)
            {
                this.itemQuality.color = AtavismSettings.Instance.ItemQualityColor(item.Quality);
            }

        }

        public void SetLootEntryDetails(int currencyId, int count)
        {
            this.item = null;
            this.curr = Inventory.Instance.GetCurrency(currencyId);
#if AT_I2LOC_PRESET
      if (itemNameText != null)  this.itemNameText.text = I2.Loc.LocalizationManager.GetTranslation("Items/" + curr.name);
      if (itemNameTextTMP != null)  this.itemNameTextTMP.text = I2.Loc.LocalizationManager.GetTranslation("Items/" + curr.name);
#else
            if (itemNameText != null)
                this.itemNameText.text = curr.name;
            if (itemNameTextTMP != null)
                this.itemNameTextTMP.text = curr.name;
#endif
            if (curr.icon != null)
                this.itemIcon.sprite = curr.icon;
            else
                this.itemIcon.sprite = AtavismSettings.Instance.defaultItemIcon;

            // this.itemIcon.sprite = item.icon;
            if (countText != null)
            {
                if (count > 1)
                {
                    this.countText.text = count.ToString();
                }
                else
                {
                    this.countText.text = "";
                }
            }
            if (countTextTMP != null)
            {
                if (count > 1)
                {
                    this.countTextTMP.text = count.ToString();
                }
                else
                {
                    this.countTextTMP.text = "";
                }
            }

            if (itemQuality != null)
            {
                this.itemQuality.color = AtavismSettings.Instance.ItemQualityColor(1);
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
                    if(item!=null)
                        item.ShowTooltip(gameObject);

                }
                else
                {
                    HideTooltip();
                }
            }
        }
    }
}