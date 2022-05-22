using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atavism
{
    public class AoeAbilityObject : MonoBehaviour
    {
        public GameObject posivive;
        public GameObject nagative;

        public void SetPositive()
        {
            if (posivive != null)
                posivive.SetActive(true);
            if (nagative != null)
                nagative.SetActive(false);
        }
        public void SetNegative()
        {
            if (posivive != null)
                posivive.SetActive(false);
            if (nagative != null)
                nagative.SetActive(true);
        }
    }
}