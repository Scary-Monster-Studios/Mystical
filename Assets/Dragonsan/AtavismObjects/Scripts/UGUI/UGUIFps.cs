using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Atavism
{

    public class UGUIFps : MonoBehaviour
    {

        [SerializeField] float updateInterval = 0.5F;
        [SerializeField] Text fpsTextOutput;
        [SerializeField] TextMeshProUGUI TMPFpsTextOutput;
        private float timeleft;
        private float accum = 0;
        private int frames = 0;
        // Use this for initialization
        void Start()
        {
            timeleft = updateInterval;
            if (fpsTextOutput != null)
                fpsTextOutput.material = new Material(fpsTextOutput.material);
            if (TMPFpsTextOutput != null)
                TMPFpsTextOutput.material = new Material(TMPFpsTextOutput.material);
        }

        // Update is called once per frame
        void Update()
        {
            if (AtavismSettings.Instance != null && AtavismSettings.Instance.GetVideoSettings().fps)
            {
                timeleft -= Time.deltaTime;
                accum += Time.timeScale / Time.deltaTime;
                ++frames;
                if (timeleft <= 0.0)
                {
                    float fps = accum / frames;
                    string format = System.String.Format("{0:F2} FPS", fps);
                    if (fpsTextOutput != null)
                        fpsTextOutput.text = format;
                    if (TMPFpsTextOutput != null)
                        TMPFpsTextOutput.text = format;
                    if (fps < 30)
                    {
                        if (fpsTextOutput != null)
                            fpsTextOutput.material.color = Color.yellow;
                        if (TMPFpsTextOutput != null)
                            TMPFpsTextOutput.color = Color.yellow;
                    }
                    else if (fps < 10)
                    {
                        if (fpsTextOutput != null)
                            fpsTextOutput.material.color = Color.red;
                        if (TMPFpsTextOutput != null)
                            TMPFpsTextOutput.color = Color.red;
                    }
                    else
                    {
                        if (fpsTextOutput != null)
                            fpsTextOutput.material.color = Color.green;
                        if (TMPFpsTextOutput != null)
                            TMPFpsTextOutput.color = Color.green;
                    }
                    timeleft = updateInterval;
                    accum = 0.0F;
                    frames = 0;
                }
            }
            else
            {
                if (fpsTextOutput != null)
                    fpsTextOutput.text = "";
                if (TMPFpsTextOutput != null)
                    TMPFpsTextOutput.text = "";
            }
        }

    }
}