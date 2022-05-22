using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class PvPRegion : MonoBehaviour
    {

        void Start()
        {
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == ClientAPI.GetPlayerObject().GameObject)
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("pvpState", true);
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "faction.UPDATE_PVP_STATE", props);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject == ClientAPI.GetPlayerObject().GameObject)
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("pvpState", false);
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "faction.UPDATE_PVP_STATE", props);
            }
        }
    }
}