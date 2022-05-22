using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Atavism
{
    [Serializable]
    public class AtavismClaimUpgrade
    {
        public Vector3 size = Vector3.one;
        public Vector3 position = Vector3.zero;
        public int currency = -1;
        public long currencyAmount = 0L;
        public Color color;
        public List<int> deedItemIDs= new List<int>();
        public List<string> deedItemSearch = new List<string>();
        public int object_limit_profile = -1;
        
        public int taxCurrency = -1;
        public long taxCurrencyAmount = 0L;
        public long taxInterval = 0L;
        public long taxMaxTimePay = 0L;
        public long taxMinTimeSell = 0L;


    }
    //[RequireComponent(BoxCollider)]
    public class AtavismClaimRegion : MonoBehaviour
    {
        public int id;
      //  public ClaimType claimType;
        public int claimType = -1;
        public Vector3 size = Vector3.one;
        public int purchaseCurrency = 1;
        public long cost = 1;
        public List<int> deedItemIDs;
        public List<string> deedItemSearch = new List<string>();
        public List<AtavismClaimUpgrade> upgrades = new List<AtavismClaimUpgrade>();
        public int object_limit_profile = -1;
        public int taxCurrency = -1;
        public long taxCurrencyAmount = 0L;
        public long taxInterval = 0L;
        public long taxMaxTimePay = 0L;
        public long taxMinTimeSell = 0L;

        private void OnDrawGizmosSelected()
        {
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, size);
            
            for (int i = 0; i < upgrades.Count; i++)
            {
                if (upgrades[i].color == new Color(0,0,0,0))
                {
                    float R = Random.Range(0, 226/255f);
                    float G = Random.Range(0, 226/255f);
                    float B = Random.Range(0, 226/255f);
                    upgrades[i].color = new Color(R, G, B,1F);
                }
                Gizmos.color = upgrades[i].color;
                Gizmos.DrawWireCube(transform.TransformPoint(upgrades[i].position), upgrades[i].size);
           }
        }
    }
}