using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUIMailListEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        public Text senderText;
        public TextMeshProUGUI TMPSenderText;
        public Text subjectText;
        public TextMeshProUGUI TMPSubjectText;
        public Image itemIcon;
        public Image selectedImage;
        MailEntry entry;
        bool selected = false;

        // Use this for initialization
        void Start()
        {

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseEntered = true;
            if (!selected)
            {
                /*  Color col = GetComponent<Image>().color;
                  col.a = 0.8f;
                  GetComponent<Image>().color = col;*/
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseEntered = false;
            if (!selected)
            {
                /*  Color col = GetComponent<Image>().color;
                  col.a = 0f;
                  GetComponent<Image>().color = col;*/
            }
        }

        public void MailEntryClicked()
        {
            Mailing.Instance.SelectedMail = entry;
        }

        public void SetMailEntryDetails(MailEntry entry)
        {
            this.entry = entry;
            if (senderText != null)
                this.senderText.text = entry.senderName;
            if (TMPSenderText != null)
                this.TMPSenderText.text = entry.senderName;
            if (entry.subject == "")
            {
#if AT_I2LOC_PRESET
             if (subjectText != null) this.subjectText.text = I2.Loc.LocalizationManager.GetTranslation("No topic");
             if (TMPSubjectText != null) this.TMPSubjectText.text = I2.Loc.LocalizationManager.GetTranslation("No topic");
#else
                if (subjectText != null)
                    this.subjectText.text = "No topic";
                if (TMPSubjectText != null)
                    this.TMPSubjectText.text = "No topic";
#endif
            }
            else
            {
                if (subjectText != null)
                    this.subjectText.text = entry.subject;
                if (TMPSubjectText != null)
                    this.TMPSubjectText.text = entry.subject;
            }
            //this.itemIcon.sprite = entry.;
            if (entry == Mailing.Instance.SelectedMail)
            {
                selected = true;
                if (selectedImage != null)
                    selectedImage.enabled = true;
                /*  Color col = GetComponent<Image>().color;
                   col.a = 1f;
                   GetComponent<Image>().color = col;*/
            }
            else
            {
                selected = false;
                if (selectedImage != null)
                    selectedImage.enabled = false;
                /*  Color col = GetComponent<Image>().color;
                   col.a = 0f;
                   GetComponent<Image>().color = col;*/
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
            }
        }
    }
}