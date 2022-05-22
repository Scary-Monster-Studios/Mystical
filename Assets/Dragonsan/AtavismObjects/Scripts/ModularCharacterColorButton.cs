using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModularCharacterColorButton : MonoBehaviour
{
    public ModularCharacterColor panel;
	
    public void ColorClicked()
    {
        Color buttonColor = gameObject.GetComponent<Image>().color;
        panel.AdjustColor(buttonColor);
    }
}
