using UnityEngine;
using System.Collections;
namespace Atavism
{

    public abstract class AtavismStreamer : MonoBehaviour
    {
        public abstract int GetTilesToLoad();
        public abstract int GetTilesLoaded();
        public abstract float GetLoadingProgress();

    }
}