using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class ClaimInteractiveObjectChild : MonoBehaviour
    {

        public List<GameObject> coordEffects;

        // Use this for initialization
        void Start()
        {

        }

        void OnMouseDown()
        {
            if (AtavismCursor.Instance.IsMouseOverUI())
                return;

            Debug.Log("Door clicked");
            GetComponentInParent<ClaimObject>().sendNextState(coordEffects, gameObject);
        }

    }
}