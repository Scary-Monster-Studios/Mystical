using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Atavism
{

    public abstract class AtavismResourceManager : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
        }

        public abstract object LoadAsset(AtavismObjectNode node, string path, string fileName);

        public abstract object LoadAsset(Dictionary<string, object> props, string path, string fileName);
    }
}