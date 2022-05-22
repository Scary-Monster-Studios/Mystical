using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Atavism
{
    public class Region : MonoBehaviour
    {

        [SerializeField]
        string regionName = "";

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<AtavismNode>() != null && other.gameObject.layer == 2)
            {
                string[] args = new string[1];
                args[0] = regionName;
                AtavismEventSystem.DispatchEvent("REGION_MESSAGE", args);
            }
        }

    }
}