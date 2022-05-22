using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Atavism
{
    public class UGUILimitDisplay : MonoBehaviour
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI valueText;

        public void Display(string value, string name)
        {
            if (valueText)
            {
                valueText.text = value;
            }

            if (nameText)
            {
#if AT_I2LOC_PRESET
                nameText.text = I2.Loc.LocalizationManager.GetTranslation(name);
#else
                nameText.text = name;
#endif
               
            }
        }
    }
}