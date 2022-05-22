using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atavism
{
    public class MobWeapon : MonoBehaviour
    {
        [SerializeField] Renderer[] weapons;
        protected float hideTime = 0f;
        protected bool hidedWeapon = false;
        void ShowWeapon(bool v = true)
        {
            foreach (Renderer r in weapons)
            {
                if (r != null)
                {
                    r.enabled = v;
                }
            }
        }

        public void HideWeapon(float t)
        {
            ShowWeapon(false);
            hideTime = Time.time + t;
            hidedWeapon = true;
        }

        protected void Update()
        {
            if (hideTime < Time.time && hideTime != 0 && hidedWeapon)
            {
                hidedWeapon = false;
                ShowWeapon(true);
            }
        }

    }
}