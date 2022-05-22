using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    [System.Obsolete("Class Obsotete Use InteractiveObject instead InstancePortal", true)]
    public class InstancePortal : MonoBehaviour
    {
        public enum Trigger
        {
            Collide,
            Click
        }

        public Trigger trigger;
        public int instanceID=0;
        public string markerName;
        public Vector3 loc;

        float activeTime;

        void Start()
        {
            activeTime = Time.time;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == ClientAPI.GetPlayerObject().GameObject)
            {
                EnterInstance();
            }
        }

        void OnMouseDown()
        {
        }

        private void OnMouseUp()
        {
            if (trigger == Trigger.Click)
            {
                EnterInstance();
            }
        }

        void EnterInstance()
        {
            if (Time.time > activeTime)
            {
                long targetOid = ClientAPI.GetPlayerObject().Oid;

                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("instanceID", instanceID);
                if (markerName == null)
                    markerName = "";
                props.Add("marker", markerName);
                props.Add("loc", loc);
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.CHANGE_INSTANCE", props);
                activeTime = Time.time + 2f;
            }
        }
    }
}