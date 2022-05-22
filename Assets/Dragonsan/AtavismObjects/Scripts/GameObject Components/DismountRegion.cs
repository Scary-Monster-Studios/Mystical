using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class DismountRegion : MonoBehaviour
    {

        void Start()
        {
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == ClientAPI.GetPlayerObject().GameObject)
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.DISMOUNT", props);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject == ClientAPI.GetPlayerObject().GameObject)
            {
                // Do something to say the player can now mount?
            }
        }
    }
}