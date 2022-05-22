using UnityEngine;
using System.Collections;
using System;

namespace Atavism
{
    [Serializable]
    public class ResourceDrop //: MonoBehaviour
    {

       // public AtavismInventoryItem item;
        public int itemId =-1;
        public int min;
        public int max;
        public float chance = 100f;
        public string search = "";
        /* // Use this for initialization
         void Start()
         {

         }

         // Update is called once per frame
         void Update()
         {

         }*/
    }
}