using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace Atavism
{
    public class SetFocus : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
            if (GetComponent<InputField>() != null)
            {
                GetComponent<InputField>().Select();
                GetComponent<InputField>().ActivateInputField();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnEnable()
        {
            if (EventSystem.current == null)
                return;
            EventSystem.current.SetSelectedGameObject(gameObject);
            if (GetComponent<InputField>() != null)
            {
                GetComponent<InputField>().Select();
                GetComponent<InputField>().ActivateInputField();
            }
        }
    }
}