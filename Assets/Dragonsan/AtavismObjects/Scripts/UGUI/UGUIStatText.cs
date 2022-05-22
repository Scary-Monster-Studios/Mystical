using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{
    public class UGUIStatText : MonoBehaviour
    {

        public Text text;
        public TextMeshProUGUI TMPText;
        public string prop;

        // Use this for initialization
        void Start()
        {
            if (text == null)
                text = GetComponent<Text>();
            if (TMPText == null)
                TMPText = GetComponent<TextMeshProUGUI>();

            if (ClientAPI.GetPlayerObject() != null)
            {
                ClientAPI.GetPlayerObject().RegisterPropertyChangeHandler(prop, PropHandler);
                if (ClientAPI.GetPlayerObject().PropertyExists(prop))
                {
                    int value = (int)ClientAPI.GetPlayerObject().GetProperty(prop);
                    if (text != null)
                        text.text = "" + value;
                    if (TMPText != null)
                        TMPText.text = "" + value;
                }
            }
        }

        void OnDestroy()
        {
            if (ClientAPI.GetPlayerObject() != null)
                ClientAPI.GetPlayerObject().RemovePropertyChangeHandler(prop, PropHandler);
        }

        public void PropHandler(object sender, PropertyChangeEventArgs args)
        {
            int value = (int)ClientAPI.GetPlayerObject().GetProperty(prop);
            if (text != null)
                text.text = "" + value;
            if (TMPText != null)
                TMPText.text = "" + value;
        }
    }
}