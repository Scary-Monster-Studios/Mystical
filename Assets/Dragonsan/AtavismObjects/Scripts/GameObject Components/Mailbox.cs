using UnityEngine;
using System.Collections;

namespace Atavism
{
    public class Mailbox : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnMouseDown()
        {
            if (!AtavismCursor.Instance.IsMouseOverUI())
            {
                Mailing.Instance.RequestMailList(transform.position);
            }
        }
    }
}