using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class NoBuildClaimRegion : MonoBehaviour
    {

        void Start()
        {
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == ClientAPI.GetPlayerObject().GameObject)
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("noBuild", 1);
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.NO_BUILD_CLAIM_TRIGGER", props);
            }
        }

        void OnTriggerExit(Collider other)
        {
            long oid = other.gameObject.GetComponent<AtavismNode>().Oid;
            if (other.gameObject == ClientAPI.GetPlayerObject().GameObject)
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("noBuild", 0);
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "voxel.NO_BUILD_CLAIM_TRIGGER", props);
            }
        }
    }
}