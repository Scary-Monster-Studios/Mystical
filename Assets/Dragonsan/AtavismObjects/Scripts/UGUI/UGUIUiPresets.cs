using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atavism
{
    public class UGUIUiPresets : MonoBehaviour
    {
        [SerializeField] RectTransform[] UIGameObjects;
        [SerializeField] Vector3 UIScale = new Vector3(.6f, .6f, .6f);
        int screenHeight;
        int screenWidth;
        [SerializeField] RectTransform[] UIGameObjectsWidthResize;
        [SerializeField] RectTransform[] UIMiniMapGameObjects;

        [SerializeField] int UiWidthDeltaSize;
        [SerializeField] bool scaleCanvas = false;
        //  bool minimapFullScreen = false;
        // Use this for initialization
        void Start()
        {
            if (Application.isPlaying)
                UpdateUiSacle();
        }

        // Update is called once per frame
        void Update()
        {
            if (!Screen.height.Equals(screenHeight) || !Screen.width.Equals(screenWidth))
                if (Application.isPlaying)
                {
                    UpdateUiSacle();
                }
        }
        void UpdateUiSacle()
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;

            if ((Screen.width < 1360) || (Screen.height < 720))
            {
                if (scaleCanvas)
                {
                    CanvasScaler sc = GetComponent<CanvasScaler>();
                    if (sc != null)
                    {
                        sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                        sc.referenceResolution = new Vector2(1366, 768);
                    }
                }
                else
                {
                    if (UIGameObjects.Length > 0)
                        for (int i = 0; i < UIGameObjects.Length; i++)
                        {
                            UIGameObjects[i].localScale = UIScale;
                        }
                    if (UIGameObjectsWidthResize.Length > 0)
                        for (int i = 0; i < UIGameObjectsWidthResize.Length; i++)
                        {
                            Vector2 sd = UIGameObjectsWidthResize[i].sizeDelta;
                            sd.x -= UiWidthDeltaSize;
                            UIGameObjectsWidthResize[i].sizeDelta = sd;
                        }
                }
            }
            else
            {
                if (scaleCanvas)
                {
                    CanvasScaler sc = GetComponent<CanvasScaler>();
                    if (sc != null)
                    {
                        sc.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
                    }
                }
                else
                {
                    if (UIGameObjects.Length > 0)
                        for (int i = 0; i < UIGameObjects.Length; i++)
                        {
                            UIGameObjects[i].localScale = Vector3.one;
                        }
                    if (UIGameObjectsWidthResize.Length > 0)
                        for (int i = 0; i < UIGameObjectsWidthResize.Length; i++)
                        {
                            Vector2 sd = UIGameObjectsWidthResize[i].sizeDelta;
                            sd.x += UiWidthDeltaSize;
                            UIGameObjectsWidthResize[i].sizeDelta = sd;
                        }
                }
            }
        }


    }
}