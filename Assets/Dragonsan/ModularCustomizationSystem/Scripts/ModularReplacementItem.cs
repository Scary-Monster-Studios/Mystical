using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ReplacementItems
{
    None,
    Head,
    Torso,
    UpperArms,
    LowerArms,
    Hips,
    LowerLegs,
    Feet,
    Hands  
}

[CreateAssetMenu(fileName = "Data", menuName = "HNGamers/ModularReplacementItem", order = 800)]
public class ModularReplacementItem : ScriptableObject
{
    public string ReplacementName;

    public List<ReplacementItems> replacementItems;

    public GameObject fullModelReplacement;
    
    public string HeadModelReplacement;

    public string TorsoModelReplacement;

    public string UpperArmsModelReplacement;

    public string LowerArmsModelReplacement;

    public string HipsModelReplacement;

    public string LowerLegsModelReplacement;

    public string FeetModelReplacement;

    public string HandsModelReplacement;

}
