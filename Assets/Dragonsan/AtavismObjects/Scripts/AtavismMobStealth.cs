using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Atavism
{
    public class AtavismMobStealth : MonoBehaviour
    {

        [SerializeField] AtavismNode node;
        [Range(0, 1)]
        [SerializeField] float stealthAlpha = 0.6F;
        [SerializeField] string stealthParam = "stealth";
        [SerializeField] int stealth = 0;
        [SerializeField] Renderer[] renderers;

        [SerializeField] bool useSpawnAlphaColor = true;
        [Range(0, 1)]
        [SerializeField] float spawnAlpha = 1F;
        [SerializeField] string shaderSpawnParam = "";
        [SerializeField] float shaderSpawnParamValue = 1f;
        [SerializeField] float spawnTime = 2f;
        [SerializeField] int spawnTickNum = 60;
        [SerializeField] GameObject spawnCoordEffect;

        [SerializeField] bool useDespawnAlphaColor = true;
        [Range(0, 1)]
        [SerializeField] float despawnAlpha = 0F;
        [SerializeField] string shaderDespawnParam = "";
        [SerializeField] float shaderDespawnParamValue = 0f;
        [SerializeField] float despawnTime = 2f;
        [SerializeField] int despawnTickNum = 60;
        [SerializeField] GameObject despawnCoordEffect;
        bool started = false;
        bool _despawn = false;
        float destAlfa = -1;
        float destParamValue = -1;

        // Start is called before the first frame update
        void Start()
        {
            node = GetComponent<AtavismNode>();
            if (node != null)
            {
                node.RegisterObjectPropertyChangeHandler(stealthParam, StealthHandler);
            }
            if (renderers == null)
            {
                renderers = GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                {
                    if (r != null)
                        for (int i = 0; i < r.materials.Length; i++)
                        {
                            if (r.materials[i] != null)
                                r.materials[i] = Instantiate<Material>(r.materials[i]);
                        }
                }
            }
            if (!SceneManager.GetActiveScene().name.Equals(ClientAPI.Instance.characterSceneName))
            {
                foreach (Renderer r in renderers)
                {
                    if (useSpawnAlphaColor)
                    {
                        if (r != null)
                            for (int i = 0; i < r.materials.Length; i++)
                            {
                                if (r.materials[i] != null)
                                    if (r.materials[i].HasProperty("_Color"))
                                    {
                                        Color c = r.materials[i].color;
                                        c.a = 0f;
                                        r.materials[i].color = c;
                                    }
                            }
                    }
                }
            }
        }

        public void StealthHandler(object sender, PropertyChangeEventArgs args)
        {
            if (_despawn)
                return;
            if (node != null)
            {
                stealth = (int)node.GetProperty(stealthParam);
                if (stealth > 0)
                {
                    destAlfa = stealthAlpha;
                    foreach (Renderer r in renderers)
                    {
                        if (r != null)
                            for (int i = 0; i < r.materials.Length; i++)
                            {
                                if (r.materials[i] != null)
                                    if (r.materials[i].HasProperty("_Color"))
                                    {
                                        Color c = r.materials[i].color;
                                        c.a = stealthAlpha;
                                        r.materials[i].color = c;
                                    }
                            }
                    }
                }
                else
                {
                    foreach (Renderer r in renderers)
                    {
                        if (r != null)
                            for (int i = 0; i < r.materials.Length; i++)
                            {
                                if (r.materials[i] != null)
                                    if (r.materials[i].HasProperty("_Color"))
                                    {
                                        Color c = r.materials[i].color;
                                        c.a = 1f;
                                        r.materials[i].color = c;
                                    }
                            }
                    }
                }
            }
        }

        public void despawn()
        {
            destAlfa = despawnAlpha;
            destParamValue = shaderDespawnParamValue;
            _despawn = true;
            StartCoroutine(DelayObjDespawn());
            if (despawnCoordEffect != null)
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("gameObject", gameObject);
                CoordinatedEffectSystem.ExecuteCoordinatedEffect(despawnCoordEffect.name, props);
            }
        }

        protected virtual void ObjectNodeReady()
        {
            destAlfa = spawnAlpha;
            destParamValue = shaderSpawnParamValue;

            if (node != null)
            {
                if (node.PropertyExists(stealthParam))
                {
                    int stealth = (int)node.GetProperty(stealthParam);
                    if (stealth > 0)
                    {
                        destAlfa = stealthAlpha;
                    }
                }
            }
            if (renderers == null || renderers.Length == 0)
            {
                renderers = GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                    if (r != null)
                        for (int i = 0; i < r.materials.Length; i++)
                        {
                            if (r.materials[i] != null)
                                r.materials[i] = Instantiate<Material>(r.materials[i]);
                        }
            }
            if (spawnCoordEffect != null)
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("gameObject", gameObject);
                CoordinatedEffectSystem.ExecuteCoordinatedEffect(spawnCoordEffect.name, props);
            }
            StartCoroutine(DelayObjSpawn());
        }

        IEnumerator DelayObjSpawn()
        {
            WaitForSeconds delay = new WaitForSeconds(spawnTime / spawnTickNum);
            int count = spawnTickNum - 1;
            while (count > 0)
            {
                foreach (Renderer r in renderers)
                {
                    if (useSpawnAlphaColor)
                    {
                        if (r != null)
                            for (int i = 0; i < r.materials.Length; i++)
                            {
                                if (r.materials[i] != null)
                                    if (r.materials[i].HasProperty("_Color"))
                                    {
                                        Color c = r.materials[i].color;
                                        float diffAlfia = (destAlfa - c.a) / count;
                                        c.a += diffAlfia;
                                        r.materials[i].color = c;
                                    }
                            }

                    }
                    else
                    {
                        if (r != null)
                            for (int i = 0; i < r.materials.Length; i++)
                            {
                                if (r.materials[i] != null)
                                    if (shaderSpawnParam != null && shaderSpawnParam.Length > 0 && r.materials[i].HasProperty(shaderSpawnParam))
                                    {
                                        //Debug.LogError("DelayObjDespawn: " + count + " "  + " Renderer=" + r+ " destParamValue=" + destParamValue + " cval="+ r.materials[i].GetFloat(shaderSpawnParam) + " val=" + ((destParamValue - r.materials[i].GetFloat(shaderSpawnParam)) / count) + " "+gameObject, gameObject);
                                        r.materials[i].SetFloat(shaderSpawnParam, r.materials[i].GetFloat(shaderSpawnParam) + (destParamValue - r.materials[i].GetFloat(shaderSpawnParam)) / count);
                                    }
                            }
                    }
                }
                yield return delay;
                count--;
            }
            started = true;
        }

        IEnumerator DelayObjDespawn()
        {
            WaitForSeconds delay = new WaitForSeconds(despawnTime / despawnTickNum);
            int count = despawnTickNum - 1;
            while (count > 0)
            {
                foreach (Renderer r in renderers)
                {
                    if (useDespawnAlphaColor)
                    {
                        if (r != null)
                            for (int i = 0; i < r.materials.Length; i++)
                            {
                                if (r.materials[i] != null)
                                    if (r.materials[i].HasProperty("_Color"))
                                    {
                                        Color c = r.materials[i].color;
                                        float diffAlfia = (destAlfa - c.a) / count;
                                        c.a += diffAlfia;
                                        r.materials[i].color = c;
                                    }
                            }
                    }
                    else
                    {
                        if (r != null)
                            for (int i = 0; i < r.materials.Length; i++)
                            {
                                if (r.materials[i] != null)
                                    if (shaderDespawnParam != null && shaderDespawnParam.Length > 0 && r.materials[i].HasProperty(shaderDespawnParam))
                                    {
                                        //Debug.LogError("DelayObjDespawn: " + count + " "  + " Renderer=" + r+ " shaderParam="+ shaderParam+" cval="+ r.materials[i].GetFloat(shaderParam) + " val=" + r.materials[i].GetFloat(shaderParam)+" "+gameObject, gameObject);
                                        r.materials[i].SetFloat(shaderDespawnParam, r.materials[i].GetFloat(shaderDespawnParam) + (destParamValue - r.materials[i].GetFloat(shaderDespawnParam)) / count);
                                    }
                            }
                    }
                }
                yield return delay;
                count--;
            }
            started = true;
        }


        // Update is called once per frame
        void Update()
        {
            if (node == null)
                node = GetComponent<AtavismNode>();

            if (renderers == null || renderers.Length == 0)
            {
                renderers = GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                {
                    if (r != null)
                        for (int i = 0; i < r.materials.Length; i++)
                        {
                            if (r.materials[i] != null)
                                r.materials[i] = Instantiate<Material>(r.materials[i]);
                        }
                }
            }

            if (_despawn)
                return;

            if (started)
            {
                if (renderers != null && node != null)
                {
                    if (node.PropertyExists(stealthParam))
                    {
                        if (stealth != (int)node.GetProperty(stealthParam))
                        {
                            stealth = (int)node.GetProperty(stealthParam);

                            if (stealth > 0)
                            {
                                foreach (Renderer r in renderers)
                                {
                                    if (r != null)
                                        for (int i = 0; i < r.materials.Length; i++)
                                        {
                                            if (r.materials[i] != null)
                                                if (r.materials[i].HasProperty("_Color"))
                                                {
                                                    Color c = r.materials[i].color;
                                                    c.a = stealthAlpha;
                                                    r.materials[i].color = c;
                                                }
                                        }

                                }
                            }
                            else
                            {
                                foreach (Renderer r in renderers)
                                {
                                    if (r != null)
                                        for (int i = 0; i < r.materials.Length; i++)
                                        {
                                            if (r.materials[i] != null)
                                                if (r.materials[i].HasProperty("_Color"))
                                                {
                                                    Color c = r.materials[i].color;
                                                    c.a = 1f;
                                                    r.materials[i].color = c;
                                                }
                                        }

                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning(stealthParam + " is not defined", gameObject);
                    }
                }
                else
                    Debug.LogError("Node and renderers is null", gameObject);
            }
            else
            {
                if (node != null)
                    if (node.PropertyExists(stealthParam))
                    {
                        stealth = (int)node.GetProperty(stealthParam);
                        if (stealth > 0)
                        {
                            destAlfa = stealthAlpha;
                        }
                        else
                        {
                            destAlfa = 1f;
                        }
                    }
            }
        }
    }
}