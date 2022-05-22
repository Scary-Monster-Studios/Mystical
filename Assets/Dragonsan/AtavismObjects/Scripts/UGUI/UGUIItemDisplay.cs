using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{

    public delegate void OnItemClicked(AtavismInventoryItem item);

    /// <summary>
    /// Handles the display of an item in UGUI, such as setting the texture and the count label. 
    /// This script can only be used on a button.
    /// </summary>
    public class UGUIItemDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        public Text countText;
        public TextMeshProUGUI TMPCountText;
        public Image itemIcon;
        OnItemClicked itemClickedFunction;
        AtavismInventoryItem item;
        int slotNum;
        bool mouseEntered = false;
        public Text itemName;
        public TextMeshProUGUI TMPItemName;
        public Image itemQuality;
        [SerializeField] bool resetDisableQuality = false;
        [SerializeField] Color selectedColor = Color.yellow;


        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("ITEM_ICON_UPDATE", this);
        }

        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("ITEM_ICON_UPDATE", this);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "ITEM_ICON_UPDATE")
            {
                if (item != null)
                {
                    item.icon = AtavismPrefabManager.Instance.GetItemIconByID(item.templateId);
                    itemIcon.sprite = item.icon;

                    if (this.itemIcon != null)
                    {
                        itemIcon.enabled = true;
                        if (item.icon != null)
                            itemIcon.sprite = item.icon;
                        else
                            itemIcon.sprite = AtavismSettings.Instance.defaultItemIcon;
                    }

                    if (GetComponent<Button>() != null && item != null && this.itemIcon == null)
                    {
                        if (item.icon != null)
                            GetComponent<Button>().image.sprite = item.icon;
                        else
                            GetComponent<Button>().image.sprite = AtavismSettings.Instance.defaultItemIcon;
                    }
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

        public void ItemClicked()
        {
            if (itemClickedFunction != null)
                itemClickedFunction(item);
        }
        public void Reset()
        {
            this.item = null;
            if (this.itemIcon != null)
            {
                this.itemIcon.enabled = false;
                //    this.itemIcon.sprite = null;
            }
            // }
            if (countText != null)
            {
                countText.text = "";
            }
            if (TMPCountText != null)
            {
                TMPCountText.text = "";
            }
            if (itemName != null)
            {
                itemName.text = "";
            }
            if (TMPItemName != null)
            {
                TMPItemName.text = "";
            }
            this.itemClickedFunction = null;
            if (itemQuality != null)
            {
                this.itemQuality.color = Color.white;
                if (resetDisableQuality)
                    this.itemQuality.enabled = false;
            }
            if (GetComponent<Image>() != null)
                GetComponent<Image>().color = Color.white;

        }

        public void SetItemData(AtavismInventoryItem item, OnItemClicked itemClickedFunction)
        {
            this.item = item;
            if (item == null)
            {
                //   if (this.itemIcon != null)
                //       this.itemIcon.enabled = false;
                if (GetComponent<Image>() != null)
                {
                    GetComponent<Image>().color = Color.white;
                }
                if (itemQuality != null)
                {
                    itemQuality.color = Color.white;
                    if (resetDisableQuality)
                        this.itemQuality.enabled = false;
                }
            }
            if (GetComponent<Button>() != null && item != null && this.itemIcon == null)
            {
                if (item.icon != null)
                    GetComponent<Button>().image.sprite = item.icon;
                else
                    GetComponent<Button>().image.sprite = AtavismSettings.Instance.defaultItemIcon;
             //   GetComponent<Button>().image.sprite = item.icon;
            }
            if (this.itemIcon != null)
            {
                if (item != null)
                {
                    if (item.icon != null)
                        this.itemIcon.sprite = item.icon;
                    else
                        this.itemIcon.sprite = AtavismSettings.Instance.defaultItemIcon;
                    //   this.itemIcon.sprite = item.icon;
                    this.itemIcon.enabled = true;
                }
                else
                {
                    this.itemIcon.enabled = false;
                }
            }
            else
            {
           //     Debug.LogError("Item Icon is null");
            }

            if (GetComponent<Image>() != null)
            {
                if (item != null)
                {
                    GetComponent<Image>().color = AtavismSettings.Instance.ItemQualityColor(item.quality);
                }
            }

            if (countText != null)
            {
                if (item != null && item.Count > 1)
                    countText.text = item.Count.ToString();
                else
                    countText.text = "";
            }
            if (TMPCountText != null)
            {
                if (item != null && item.Count > 1)
                    TMPCountText.text = item.Count.ToString();
                else
                    TMPCountText.text = "";
            }

            if (itemName != null && item != null)
            {
#if AT_I2LOC_PRESET
            itemName.text = I2.Loc.LocalizationManager.GetTranslation("Items/" + item.name);
#else
                itemName.text = item.name;
#endif
            }
            if (TMPItemName != null && item != null)
            {
#if AT_I2LOC_PRESET
            TMPItemName.text = I2.Loc.LocalizationManager.GetTranslation("Items/" + item.name);
#else
                TMPItemName.text = item.name;
#endif
            }
            if (itemQuality != null && item != null)
            {
                this.itemQuality.color = AtavismSettings.Instance.ItemQualityColor(item.Quality);
                this.itemQuality.enabled = true;

            }
            this.itemClickedFunction = itemClickedFunction;
        }

        void HideTooltip()
        {
            UGUITooltip.Instance.Hide();
        }

        public void Selected(bool select)
        {
            if (select)
            { if (GetComponent<Image>() != null)
                GetComponent<Image>().color = selectedColor;
            }
            else
            { if (GetComponent<Image>() != null)
                GetComponent<Image>().color = Color.white;
            }
        }

        public bool MouseEntered
        {
            get
            {
                return mouseEntered;
            }
            set
            {
                mouseEntered = value;
                if (mouseEntered && item != null)
                {
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