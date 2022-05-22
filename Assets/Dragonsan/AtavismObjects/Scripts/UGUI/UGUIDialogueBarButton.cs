using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{
    public delegate void DialogueButtonFunction();

    public class UGUIDialogueBarButton : MonoBehaviour
    {

        public Text buttonText;
        public TextMeshProUGUI TMPButtonText;
        DialogueButtonFunction buttonFunction;

        // Use this for initialization
        void Start()
        {

        }

        public void SetButtonFunction(DialogueButtonFunction function, string text)
        {
            this.buttonFunction = function;
            if (buttonText != null)
                this.buttonText.text = text;
            if (TMPButtonText != null)
                this.TMPButtonText.text = text;
        }

        public void DialogueButtonClicked()
        {
            if (buttonFunction != null)
                buttonFunction();
        }
    }
}