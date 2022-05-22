using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atavism
{
    public class AtavismMobSockets : MonoBehaviour
    {

        public List<string> slots = new List<string>();

        public List<Transform> sockets = new List<Transform>();
        public List<Transform> restsockets = new List<Transform>();

        // Sockets for attaching weapons (and particles)
        public Transform mainHand;
        public Transform mainHand2;
        public Transform offHand;
        public Transform offHand2;
        public Transform mainHandRest;
        public Transform mainHandRest2;
        public Transform offHandRest;
        public Transform offHandRest2;
        public Transform shield;
        public Transform shield2;
        public Transform shieldRest;
        public Transform shieldRest2;
        public Transform head;
        public Transform leftShoulderSocket;
        public Transform rightShoulderSocket;

        // Sockets for particles
        public Transform rootSocket;
        public Transform leftFootSocket;
        public Transform rightFootSocket;
        public Transform pelvisSocket;
        public Transform leftHipSocket;
        public Transform rightHipSocket;
        public Transform chestSocket;
        public Transform backSocket;
        public Transform neckSocket;
        public Transform mouthSocket;
        public Transform leftEyeSocket;
        public Transform rightEyeSocket;
        public Transform overheadSocket;
        
        public Transform GetSocketTransform(string slot)
        {
            int slotId = slots.IndexOf(slot);
            if (slotId >= 0)
            {
                if (sockets[slotId] != null)
                    return sockets[slotId];
            }

            return transform;
        }

        public void SetSocketTransform(string slot, Transform trans)
        {            
            if (slot.Length == 0)
            {
                Debug.LogError("Slot name cant be empty");
                return;
            }

            int slotId = slots.IndexOf(slot);
            if (slotId >= 0)
            {
                sockets[slotId] = trans;
            }
            else
            {
                slots.Add(slot);
                sockets.Add(transform);
                restsockets.Add(null);
            }
        }

        public Transform GetRestSocketTransform(string slot)
        {
            int slotId = slots.IndexOf(slot);
            if (slotId >= 0)
            {
                if (restsockets[slotId] != null)
                    return restsockets[slotId];
            }

            return transform;
        }

        public void SetRestSocketTransform(string slot, Transform trans)
        {            if (slot.Length == 0)
            {
                Debug.LogError("Slot name cant be empty");
                return;
            }

            int slotId = slots.IndexOf(slot);
            if (slotId >= 0)
            {
                restsockets[slotId] = trans;
            }
            else
            {
                slots.Add(slot);
                restsockets.Add(transform);
                sockets.Add(null);
            }

        }
    }
}