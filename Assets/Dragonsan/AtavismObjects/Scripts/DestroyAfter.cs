using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Atavism
{
    public class DestroyAfter : MonoBehaviour
    {
        // Start is called before the first frame update
        public float DestroyAfterSeconds = 60f;
        private void Start()
        {
            Destroy(gameObject, DestroyAfterSeconds);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}