using System;
using System.Collections;
using System.Collections.Generic;
using Atavism;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModularCharacterSlider : MonoBehaviour
{
    public TextMeshProUGUI label;
    public Slider slider;
    public string dna;

    public void Assign()
    {
        //Debug.LogError("ModularCharacterSlider: " + dna);
        CharacterSelectionCreationManager.Instance.RegisterModularCharacterSlider(this);
    }

   
}
