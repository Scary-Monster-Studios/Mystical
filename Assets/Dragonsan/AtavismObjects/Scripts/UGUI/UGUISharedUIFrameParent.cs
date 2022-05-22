using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class UGUISharedUIFrameParent : MonoBehaviour
    {

        public UGUIPanelTitleBar titleBar;
        public List<UGUISharedParentButton> childFrameButtons;
        public int currentChild = 0;
        bool showing;

        // Use this for initialization
        void Start()
        {
            if (titleBar != null)
                titleBar.SetOnPanelClose(Hide);

            Hide();
        }

        // Update is called once per frame
        void Update()
        {
            if (!showing)
            {
                // Check if any children are on
                foreach (UGUISharedParentButton childButton in childFrameButtons)
                {
                    if (childButton.targetFrame.activeSelf)
                    {
                        Show();
                    }
                }
            }
            else
            {
                // Check if any other slots are now active
                bool anyActive = false;
                for (int i = 0; i < childFrameButtons.Count; i++)
                {
                    if (childFrameButtons[i].targetFrame.activeSelf)
                    {
                        anyActive = true;
                        if (i != currentChild)
                        {
                            childFrameButtons[currentChild].targetFrame.SetActive(false);
                            currentChild = i;
                        }
                    }
                }

                if (!anyActive)
                    Hide();
            }
        }

        void Show()
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            showing = true;
        }

        public void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            foreach (UGUISharedParentButton childButton in childFrameButtons)
            {
                childButton.targetFrame.SetActive(false);
            }
            showing = false;
        }
    }
}