using UnityEngine;

namespace Atavism
{
    public class Startup : MonoBehaviour
    {

        public static Startup instance;

        // Use this for initialization
        void Start()
        {
            if (instance != null)
            {
                GameObject.DestroyImmediate(gameObject);
                return;
            }
            instance = this;
            //Set the scripts object to not be destroyed
            DontDestroyOnLoad(gameObject);
        }
    }
}