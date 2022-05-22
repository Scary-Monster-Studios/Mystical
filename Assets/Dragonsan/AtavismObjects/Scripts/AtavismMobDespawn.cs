using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Atavism
{
    public class AtavismMobDespawn : MonoBehaviour
    {
        [SerializeField] Renderer[] renderers;
        [SerializeField] int num = 100;  
        // Start is called before the first frame update
        void Start()
        {
            if (renderers == null)
            {
                renderers = GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                    r.material = Instantiate<Material>(r.material);
            }
            foreach (Renderer r in renderers)
            {
                Color c = r.material.color;
                c.a = 0f;
                r.material.color = c;
            }
        }
        public void despawn()
        {
           // float despawnTime = ClientAPI.Instance.despawnDelay * 0.9f;
            float maxAlfa = 1;
            foreach (Renderer r in renderers)
            {
                Color c = r.material.color;
                if (maxAlfa > c.a)
                    maxAlfa = c.a;
            }
            StartCoroutine(DelayObjDespawn(maxAlfa));
        }
        protected virtual void ObjectNodeReady()
        {
            StartCoroutine(DelayObjDespawn(-1));
        }

        IEnumerator DelayObjDespawn(float alfa)
        {
            WaitForSeconds delay = new WaitForSeconds(AtavismClient.Instance.despawnDelay * 0.9f / num);
            int count = num;
            float diffAlfia = alfa / num;
            while (count > 1)
            {
                foreach (Renderer r in renderers)
                {
                    Color c = r.material.color;
                    c.a -= diffAlfia;
                    r.material.color = c;
                }
                yield return delay;
            }
        }
        private void Update()
        {
            if (renderers == null)
            {
                renderers = GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                    r.material = Instantiate<Material>(r.material);
            }
        }
    }
}