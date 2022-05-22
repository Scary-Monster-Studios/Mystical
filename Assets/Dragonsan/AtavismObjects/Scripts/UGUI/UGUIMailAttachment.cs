using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUIMailAttachment : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        public Text countText;
        public TextMeshProUGUI TMPCountText;
        public Image iconImage;
        public Image qualityImage;
        public GameObject stackBox;
        AtavismInventoryItem item;
        int slotNum;
        bool mouseEntered = false;

        // Use this for initialization
        void Start()
        {
            if (iconImage == null)
                if (GetComponent<Button>() != null)
                    iconImage = GetComponent<Button>().image;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseEntered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseEntered = false;
        }

        public void SetMailAttachmentData(AtavismInventoryItem item, int count, int slot)
        {
            this.item = item;
            if (iconImage != null)
            {
                if (item.icon != null)
                    iconImage.sprite = item.icon;
                else
                    iconImage.sprite = AtavismSettings.Instance.defaultItemIcon;
           }
            if (qualityImage != null)
            {
                if (item != null)
                {
                    qualityImage.color = AtavismSettings.Instance.ItemQualityColor(item.quality);
                    qualityImage.enabled = true;
                }
                else
                {
                    qualityImage.enabled = false;
                }
            }

            if (countText != null)
                countText.text = count.ToString();
            if (TMPCountText != null)
                TMPCountText.text = count.ToString();
            if (stackBox != null)
                stackBox.SetActive(count > 0);

            this.slotNum = slot;
        }

        public void TakeMailAttachment()
        {
            Mailing.Instance.TakeMailItem(slotNum);
        }

        void HideTooltip()
        {
            UGUITooltip.Instance.Hide();
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