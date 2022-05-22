using UnityEngine;
using System.Collections;

namespace Atavism
{
    public class ClaimObjectChild : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnMouseOver()
        {
            if (transform.parent != null)
                transform.parent.SendMessageUpwards("OnMouseOver");
        }

        void OnMouseExit()
        {
            if (transform.parent != null)
                transform.parent.SendMessageUpwards("OnMouseExit");
        }

        void OnMouseDown()
        {
            if (transform.parent != null)
                transform.parent.SendMessageUpwards("OnMouseDown");
        }
    }
}