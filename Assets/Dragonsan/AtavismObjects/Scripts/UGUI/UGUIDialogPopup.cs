using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

namespace Atavism
{
    public class UGUIDialogPopup : MonoBehaviour
    {

        public Text textElement;
        public TextMeshProUGUI TMPTextElement;
        public Button okButton;
        public Button yesButton;
        public Button noButton;

        // Use this for initialization
        void Start()
        {

        }

        public void ShowDialogPopup(string text, bool showButton)
        {
            if (textElement != null)
                textElement.text = text;
            if (TMPTextElement != null)
                TMPTextElement.text = text;
            if (okButton != null)
                okButton.gameObject.SetActive(showButton);
            if (yesButton != null)
                yesButton.gameObject.SetActive(false);
            if (noButton != null)
                noButton.gameObject.SetActive(false);
            if (showButton)
            {
                EventSystem.current.SetSelectedGameObject(okButton.gameObject);
            }
        }

        public void ShowDialogOptionPopup(string text)
        {
            if (textElement != null)
                textElement.text = text;
            if (TMPTextElement != null)
                TMPTextElement.text = text;
            if (okButton != null)
                okButton.gameObject.SetActive(false);
            if (yesButton != null)
                yesButton.gameObject.SetActive(true);
            if (noButton != null)
                noButton.gameObject.SetActive(true);
        }
    }
}