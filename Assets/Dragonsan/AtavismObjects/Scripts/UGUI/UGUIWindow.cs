using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Atavism
{

    public class UGUIWindow : MonoBehaviour, IPointerDownHandler
    {
        public bool resetPositionOnStartup = false;
        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("WINDOWS_RESET", this);
            Vector3 winPos = AtavismSettings.Instance.GetWindowPosition(name);
            if (resetPositionOnStartup)
            {
                if (winPos == Vector3.zero)
                {
                    transform.localPosition = new Vector3((Screen.width / 2) + (GetComponent<RectTransform>().sizeDelta.x / 4) * GetComponent<RectTransform>().localScale.x, (Screen.height / 2) + GetComponent<RectTransform>().sizeDelta.y / 4 * GetComponent<RectTransform>().localScale.y, 0);
                }
                else
                {
                    transform.localPosition = winPos;
                }
            }
        }


        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("WINDOWS_RESET", this);

        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "WINDOWS_RESET")
            {
                // Delete the old list
                Vector3 winPos = AtavismSettings.Instance.GetWindowPosition(name);
                if (winPos == Vector3.zero)
                    transform.localPosition = new Vector3((Screen.width / 2) - (GetComponent<RectTransform>().sizeDelta.x / 4) * GetComponent<RectTransform>().localScale.x, (Screen.height / 2) - GetComponent<RectTransform>().sizeDelta.y / 4 * GetComponent<RectTransform>().localScale.y, 0);
                else
                    transform.localPosition = winPos;
            }
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            // Focus the window
            AtavismUIUtility.BringToFront(this.gameObject);
        }
    }
}