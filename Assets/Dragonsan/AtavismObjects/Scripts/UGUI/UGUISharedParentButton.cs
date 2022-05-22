using UnityEngine;
using System.Collections;

namespace Atavism
{
    public class UGUISharedParentButton : MonoBehaviour
    {

        public GameObject targetFrame;
        public KeyCode toggleKey;

        // Use this for initialization
        void Start()
        {

        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey) && !ClientAPI.UIHasFocus())
            {
                if (targetFrame.activeSelf)
                {
                    targetFrame.SetActive(false);
                }
                else
                {
                    targetFrame.SetActive(true);
                }
            }
        }

        public void ShowFrame()
        {
            targetFrame.SetActive(true);
        }
    }
}