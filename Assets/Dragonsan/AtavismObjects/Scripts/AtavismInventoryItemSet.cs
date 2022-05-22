using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Atavism
{
    [Serializable]
    public class AtavismInventoryItemSetLevel
    {
        public int DamageValue = 0;
        public int DamageValuePercentage = 0;
        public int NumerOfParts = 0;
        public List<string> itemStatName = new List<string>();
        public List<int> itemStatValues = new List<int>();
        public List<int> itemStatValuesPercentage = new List<int>();
    }

    public class AtavismInventoryItemSet //: MonoBehaviour
    {
        public int Setid = 0;
        public string Name = "name";        // The set name
                                          
        public List<int> itemList = new List<int>();
        public List<AtavismInventoryItemSetLevel> levelList = new List<AtavismInventoryItemSetLevel>();

        public AtavismInventoryItemSet Clone()
        {
            AtavismInventoryItemSet clone = new AtavismInventoryItemSet();
            clone.Name = Name;
            clone.itemList = itemList;
            return clone;
        }


       
        public void ClearLevels()
        {
            levelList.Clear();

        }
      
        private static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }
        /// <summary>
        /// Gets or sets the name of the base.
        /// </summary>
        /// <value>The name of the base.</value>
      
    }
}