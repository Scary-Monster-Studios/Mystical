using System.Collections;
using System.Collections.Generic;
using Atavism;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModularCharacterColor : MonoBehaviour
{
    public TextMeshProUGUI label;
    public Canvas pickerCanvas;
    public ColorPicker picker;
    public List<Button> colors = new List<Button>(); 
        
    public string dna;

    public void Assign()
    {
        //Debug.LogError("ModularCharacterColor: "+dna);
        CharacterSelectionCreationManager.Instance.RegisterModularCharacterPicker(this);
    }

    public void AdjustColor(Color buttonColor)
    {
        CharacterSelectionCreationManager.Instance.ModularCharacterSetColor(dna, buttonColor);
    }

    public void PickerClicked()
    {
        if (pickerCanvas)
        {
            pickerCanvas.enabled = !pickerCanvas.enabled;
        }
    }
}
