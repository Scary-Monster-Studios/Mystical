using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Atavism
{

    public class UGUIVipSlot : MonoBehaviour
    {
        public TextMeshProUGUI name;
        public TextMeshProUGUI vipA;
        public TextMeshProUGUI vipB;

        public void UpdateDetaile(string Name, string vipABonus, string vipBBonus)
        {
            if (name)
            {
                name.text = Name;
            }
            if (vipA)
            {
                vipA.text = vipABonus;
            }
            if (vipB)
            {
                vipB.text = vipBBonus;
            }

        }

    }
}