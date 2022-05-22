using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public delegate void ConfirmationResponse(object confirmObject, bool accepted);

    public class AtavismUIManager : MonoBehaviour
    {

        static AtavismUIManager instance;

        public GameObject confirmationUI;

        // Use this for initialization
        void Start()
        {
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ShowConfirmationBox(string message, object confirmObject, ConfirmationResponse responseMethod)
        {
            confirmationUI.SendMessage("SetMessage", message);
            if (confirmObject == null)
                confirmObject = new object();
            confirmationUI.SendMessage("SetObject", confirmObject);
            //confirmationUI.SendMessage("SetResponseTarget", responseTarget);
            confirmationUI.SendMessage("SetConfirmationResponse", responseMethod);
            confirmationUI.SendMessage("Show");
        }

        public static AtavismUIManager Instance
        {
            get
            {
                return instance;
            }
        }
    }
}