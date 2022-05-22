using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class AtavismRaceData : MonoBehaviour
    {

        public string raceName;
        public Sprite raceIcon;
        public GameObject femaleCharacterPrefab;
        public GameObject maleCharacterPrefab;
        public string description;
        public List<AtavismClassData> availableClasses;
    }
}