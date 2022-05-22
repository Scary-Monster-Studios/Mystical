using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Atavism
{
    public class UGUISendColor : MonoBehaviour
    {

        public GameObject receiver;
        public string functionName;

        // Use this for initialization
        void Start()
        {

        }

        public void SendButtonColor()
        {
            Color color = GetComponent<Button>().colors.normalColor;
            receiver.SendMessage(functionName, color, SendMessageOptions.DontRequireReceiver);
        }
    }
}