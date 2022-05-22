using UnityEngine;
using System.Collections;

namespace Atavism
{

    public class ObjectChildMouseDetector : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            if (GetComponent<CraftingStation>() != null)
            {
                enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnMouseOver()
        {
            if (!enabled)
                return;
            if (transform.parent != null)
                transform.parent.SendMessageUpwards("OnMouseOver");
        }

        void OnMouseExit()
        {
            if (!enabled)
                return;
            if (transform.parent != null)
                transform.parent.SendMessageUpwards("OnMouseExit");
        }

        void OnMouseDown()
        {
            if (!enabled)
                return;
            if (transform.parent != null)
                transform.parent.SendMessageUpwards("OnMouseDown");
        }
    }
}