using UnityEngine;
using System.Collections;

namespace Atavism
{
    public class AtavismSceneLoadedEventSender : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            AtavismClient.Instance.SceneReady();
        }
    }
}