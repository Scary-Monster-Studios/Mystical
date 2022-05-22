using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Atavism
{

    public enum CoordinatedEffectTarget
    {
        Caster,
        Target
    }

    public abstract class CoordinatedEffect : MonoBehaviour
    {

        public CoordinatedEffectTarget target;
        public float activationDelay = 0f;
        protected float activationTime = 0f;
        public float duration = 1.5f;
        public bool destroyWhenFinished = true;
        protected Dictionary<string, object> props;
        protected OID casterOid;
        protected OID targetOid;

        public abstract void Execute(Dictionary<string, object> props);
    }

    public class CoordinatedEffectSystem : MonoBehaviour
    {

        // Dictionary of effects
        static Dictionary<string, CoordinatedEffect> effectRegistry =
                new Dictionary<string, CoordinatedEffect>();

        public static void RegisterCoordinatedEffect(string effectName, CoordinatedEffect co)
        {
            effectRegistry[effectName] = co;
            AtavismLogger.LogInfoMessage("Registered coordinated effect: " + effectName);
        }

        public static void UnregisterCoordinatedEffect(string effectName)
        {
            if (!effectRegistry.ContainsKey(effectName))
                return;
            effectRegistry.Remove(effectName);
        }

        public static void ExecuteCoordinatedEffect(string effectName, Dictionary<string, object> props)
        {
            AtavismLogger.LogDebugMessage("Executing effect with name: " + effectName);
            if (AtavismClient.Instance.resourceManager == null)
            {
                // Load prefab resource
                if (effectName.Contains(".prefab"))
                {
                    int resourcePathPos = effectName.IndexOf("Resources/");
                    effectName = effectName.Substring(resourcePathPos + 10);
                    effectName = effectName.Remove(effectName.Length - 7);
                }
                else
                {
                    effectName = "Content/CoordinatedEffects/" + effectName;
                }

                AtavismLogger.LogDebugMessage("Executing effect with filename: " + effectName);
                GameObject coordPrefab = (GameObject)Resources.Load(effectName);
                if (coordPrefab == null)
                    return;
                AtavismLogger.LogDebugMessage("Got coord prefab: " + coordPrefab.name);
                GameObject coordObject = (GameObject)UnityEngine.Object.Instantiate(coordPrefab, Vector3.zero, Quaternion.identity);
                AtavismLogger.LogDebugMessage("About to execute matching coord effect " + effectName + " " + coordObject);
                coordObject.SendMessage("Execute", props);
            }
            else
            {
                AtavismLogger.LogDebugMessage("Loading coordinated effect from external resource manager");
                string path = "";
                string fileName = effectName;
                int splitPos = effectName.LastIndexOf('/');
                //Debug.Log("Split pos: " + splitPos);
                if (splitPos != -1)
                {
                    path = effectName.Substring(0, splitPos + 1);
                    fileName = effectName.Substring(splitPos + 1);
                }
                object asset = AtavismClient.Instance.resourceManager.LoadAsset(props, path, fileName);
                if (asset != null)
                {
                    GameObject coordObject = (GameObject)UnityEngine.Object.Instantiate((GameObject)asset, Vector3.zero, Quaternion.identity);
                    AtavismLogger.LogDebugMessage("About to execute matching coord effect");
                    coordObject.SendMessage("Execute", props);
                }
            }

            AtavismLogger.LogDebugMessage("ExecuteCoordinatedEffect end " + effectName);


           
        }
    }
}