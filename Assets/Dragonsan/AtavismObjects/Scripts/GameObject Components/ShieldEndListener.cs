using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atavism
{
    public class ShieldEndListener : MonoBehaviour
    {
        public string shieldName = "";
        public AtavismNode node;
        // Start is called before the first frame update
        void Start()
        {
            node = transform.GetComponentInParent<AtavismNode>();
            if (node != null)
            {
            //    Debug.LogError("SkieldEnd regidter 1");
                node.RegisterObjectPropertyChangeHandler("shield_" + shieldName, ShieldHandler);
            }
        }

        private void OnDestroy()
        {
            if (node != null)
                node.RemoveObjectPropertyChangeHandler("shield_" + shieldName, ShieldHandler);
        }

        // Update is called once per frame
        void Update()
        {
            if (node == null)
            {
                node = transform.GetComponentInParent<AtavismNode>();
           //     Debug.LogError("SkieldEnd shield =" + shieldName + " node=" + node);
                if (node != null)
                {
                 //   Debug.LogError("SkieldEnd regidter");
                     node.RegisterObjectPropertyChangeHandler("shield_" + shieldName, ShieldHandler);
                }
            }
        }

        public void ShieldHandler(object sender, PropertyChangeEventArgs args)
        {
          //  AtavismLogger.LogDebugMessage("Got model");
            bool state = (bool)node.GetProperty("shield_" + shieldName);
         //   Debug.LogError("SkieldEnd shield =" + shieldName + " state=" + state);
            if (!state)
                DestroyImmediate(gameObject);
        }
    }
}