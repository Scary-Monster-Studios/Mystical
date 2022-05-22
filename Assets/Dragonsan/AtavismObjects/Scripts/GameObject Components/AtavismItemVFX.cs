using System;
using System.Collections;
using System.Collections.Generic;
using Atavism;
using UnityEngine;

namespace Atavism
{
   [Serializable]
   public class ItemSocketEffect
   {
      public List<int> items = new List<int>();
      public GameObject effectObject;
      public GameObject effectPrefab;
      public GameObject disableObject;
      [NonSerialized] public List<string> search = new List<string>();

   }



   public class AtavismItemVFX : MonoBehaviour
   {
      public List<GameObject> enchantsObject = new List<GameObject>();
      public List<GameObject> enchantsPrefab = new List<GameObject>();
      public List<GameObject> enchantsDisableObject = new List<GameObject>();
      public GameObject disableObject = null;
      public List<ItemSocketEffect> socketEffects = new List<ItemSocketEffect>();

      private string val = "";
      public void DisplayVFX(string slot, AtavismNode node, string val)
      {
         this.val = val;
         StartCoroutine(display(slot, node));
      }

      IEnumerator display(string slot, AtavismNode node)
      {
         yield return new WaitForSeconds(0.1f);
         string slotData = val;
         if (node != null && node.PropertyExists(slot + "DisplayVAL"))
         {
           // Debug.LogError("AtavismItemVFX:  node");
            slotData = (string) node.GetProperty(slot + "DisplayVAL");
         }

       //  Debug.LogError("AtavismItemVFX: DisplayEnchant " + slotData+" "+val, gameObject);
         if (slotData != null && slotData.Length > 0)
         {
            string[] data = slotData.Split(';');
            if (data.Length > 1)
            {
               for (int i = 1; i < data.Length; i++)
               {
                  if (data[i].StartsWith("E"))
                  {
                     int enchantLeval = int.Parse(data[i].Substring(1));
                   //  Debug.LogError("AtavismItemVFX: DisplayEnchant | " + enchantLeval);
                     if (enchantLeval > 0)
                     {
                        if (enchantsObject.Count >= enchantLeval)
                        {
                           if (enchantsObject[enchantLeval - 1] != null)
                           {
                             // Debug.LogError("AtavismItemVFX: DisplayEnchant | " + enchantLeval+" on ");
                             enchantsObject[enchantLeval - 1].SetActive(true);
                             if (enchantsDisableObject[enchantLeval - 1] != null)
                                enchantsDisableObject[enchantLeval - 1].SetActive(false);
                           }
                           else if (enchantsPrefab.Count >= enchantLeval)
                           {
                              if (enchantsPrefab[enchantLeval - 1] != null)
                              {
                                 GameObject go = Instantiate(enchantsPrefab[enchantLeval - 1], transform);
                                 go.transform.localPosition = Vector3.zero;
                                 go.transform.localRotation = Quaternion.identity;
                              }
                              if (enchantsDisableObject[enchantLeval - 1] != null)
                                  enchantsDisableObject[enchantLeval - 1].SetActive(false);
                           }
                        }
                        if (disableObject != null)
                           disableObject.SetActive(false);
                     }
                  }
                  else if (data[i].StartsWith("S"))
                  {
                     string socket = data[i].Substring(1);
                     string[] socketData = socket.Split('|');
                     int socItemID = int.Parse(socketData[2]);

                     foreach (var effect in socketEffects)
                     {
                        if (effect.items.Contains(socItemID))
                        {
                           if (effect.effectObject != null)
                           {
                              effect.effectObject.SetActive(true);
                              if (effect.disableObject != null)
                                 effect.disableObject.SetActive(false);
                           }
                           else if (effect.effectPrefab != null)
                           {
                              GameObject go = Instantiate(effect.effectPrefab, transform);
                              go.transform.localPosition = Vector3.zero;
                              go.transform.localRotation = Quaternion.identity;
                              if (effect.disableObject != null)
                                 effect.disableObject.SetActive(false);
                           }
                        }
                     }
                  }
               }
            }
         }
      }
   }
}