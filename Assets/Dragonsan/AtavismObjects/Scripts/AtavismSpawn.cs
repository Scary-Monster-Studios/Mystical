using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Atavism
{

    public class AtavismSpawn : MonoBehaviour
    {
        public GameObject prefab;


        // Start is called before the first frame update
        void Start()
        {
            if (prefab != null)
            {
                if (!SceneManager.GetActiveScene().name.Equals("Login") && !SceneManager.GetActiveScene().name.Equals(ClientAPI.Instance.characterSceneName))
                {
                    if(GetComponent<AtavismNode>()!=null)
                        if(!GetComponent<AtavismNode>().Oid.Equals(ClientAPI.GetPlayerOid()))
                            UnityEngine.Object.Instantiate(prefab, transform);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}