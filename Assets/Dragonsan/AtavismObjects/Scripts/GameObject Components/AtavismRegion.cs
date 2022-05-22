using UnityEngine;
using System.Collections;

namespace Atavism
{
    public enum AtavismRegionType
    {
        Water,
        Dismount,
        Teleport,
        ApplyEffect,
        StartQuest,
        CompleteTask,
        PvP,
        Sanctuary,
    }

    public class AtavismRegion : MonoBehaviour
    {

        public int id;
        public AtavismRegionType regionType;
        public int actionID = -1;
        public string actionData1;
        public string actionData2;
        public string actionData3;

     /*   void OnTriggerEnter(Collider other)
        {
            if (regionType == AtavismRegionType.Water)
            {
                if (other.GetComponent<AtavismNode>() != null)
                {
                    long oid = other.GetComponent<AtavismNode>().Oid;
                    if (oid > 0)
                    {
                        ClientAPI.GetObjectNode(oid).MobController.WaterRegionEntered(id);
                    }
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (regionType == AtavismRegionType.Water)
            {
                if (other.GetComponent<AtavismNode>() != null)
                {
                    long oid = other.GetComponent<AtavismNode>().Oid;
                    if (oid > 0)
                    {
                        ClientAPI.GetObjectNode(oid).MobController.WaterRegionLeft(id);
                    }
                }
            }
        }
        */
    }
}