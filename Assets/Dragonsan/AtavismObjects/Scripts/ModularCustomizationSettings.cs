using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HNGamers.Atavism;
namespace Atavism
{
    [Serializable]
    public enum ModularSockets
    {             
        Hands,
        Shoulder,
        Elbow,
        Torso,
        Head,
        HeadCovering,
        Hips,
        Knee,
        Feet
    }
    [Serializable]
    public class ModularSlotSetting
    {
        public AttachmentSocket socket;
        public string slot ="";
    }
   public class ModularCustomizationSettings : MonoBehaviour
   {
       private static ModularCustomizationSettings instance;

      // public List<ModularSockets> sockets = new List<ModularSockets>();
       public List<ModularSlotSetting> settings = new List<ModularSlotSetting>();
        // Start is called before the first frame update
        void Start()
        {
            if (instance != null)
            {
                return;
            }

            instance = this;

        }

        public static ModularCustomizationSettings Instance
        {
            get { return instance; }
        }

        public string getSlot(AttachmentSocket socket)
        {
            foreach (var s in settings)
            {
                if(s.socket == socket)
                    return s.slot;
            }
            return "";
        }
        public AttachmentSocket getSocket(string  slot)
        {
            foreach (var s in settings)
            {
                if(s.slot == slot)
                 return s.socket;
            }
            return AttachmentSocket.None;
        }
    }
}