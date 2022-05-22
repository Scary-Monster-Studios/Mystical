using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Atavism
{

    public class AtavismProgressBar : MonoBehaviour
    {
        public Image fillImage;

        public TextMeshProUGUI fillText;
        public TextMeshProUGUI timeText;
        [SerializeField] float height = 2f;
        [SerializeField] float renderDistance = 50f;
        private float value = 1;
        private float speed = 1f;
        private float valueMax = 1;
        private float lastBuildTimeUpdate = 0;
       private Coroutine coroutine = null;
        private Coroutine timerCoroutine = null;
        private ClaimObject cObject;

        private long currentTime = 0L;

        private long totalTime = 0L;
        // Start is called before the first frame update
        void Start()
        {
          
           // transform.localPosition = transform.localPosition + new Vector3(0f, height, 0f);
            coroutine = StartCoroutine(UpdateBar());
           // Debug.LogError("!!!!!!!!!!!!!!!!!! cObject="+cObject);
        }

        public void setClaimObject(ClaimObject co)
        {
            cObject = co;
            transform.position = cObject.transform.position + cObject.positionProgressBar;
        }
        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        public void updateProgress()
        {
            if (timerCoroutine == null)
            {
              //  timerCoroutine = StartCoroutine(UpdateTimer());
                //StopCoroutine(timerCoroutine);
            }

            UpdateProgressBar();
        }

        void UpdateProgressBar()
        {
            // Debug.LogError("!!!!!!!!!!!!!!!!!! UpdateProgressBar cObject=" + cObject);

            if (cObject != null)
            {
                value = cObject.currentTime / 1000f;
                valueMax = cObject.totalTime / 1000f;

                lastBuildTimeUpdate = cObject.lastBuildTimeUpdate;
                speed = cObject.timeSpeed;
            }

            if (valueMax == 0 || value == valueMax)
            {
                return;
            }
       //  Debug.LogError("$$$$$$$$$$$$$$$$            current=" + value + " max=" + valueMax + " time=" + Time.time + " lastBuildTimeUpdate=" + lastBuildTimeUpdate + " speed=" + speed);
            if (fillImage != null)
            {
                float f =   (Time.time - lastBuildTimeUpdate) * speed / speed;
                fillImage.fillAmount = (value + (float.IsInfinity(f)?0:f))/ valueMax;
            }

            if (fillText != null)
                fillText.text = value + " / " + valueMax;
            if (timeText != null)
            {
                // Debug.LogError("$$$$$$$$$$$$$$$$            current=" + value + " max=" + valueMax + " time=" + Time.time + " lastBuildTimeUpdate=" + lastBuildTimeUpdate + " speed=" + speed);
                float time = ((valueMax - value) - (Time.time - lastBuildTimeUpdate) * speed) / speed;

                int hour = 0;
                int minute = 0;
                int secound = 0;
                if (time > 3600L)
                    hour = (int) (time / 3600F);
                if (time - hour * 3600 > 60)
                    minute = (int) (time - hour * 3600) / 60;
                secound = (int) (time - hour * 3600 - minute * 60);
                float rest = time - (hour * 3600 - minute * 60) - secound;
                //   Debug.LogError( "$$$$$$$$$$$$$$$$            hour="+hour+" minute="+minute+" secound="+secound);

                string outTime = "";
                if (hour > 0)
                {
                    outTime += hour + ":";
                    if (minute > 0)
                    {
                        if (minute < 10)
                            outTime += "0" + minute + ":";
                        else
                            outTime += minute + ":";
                    }
                    else
                    {
                        outTime += "00:";
                    }

                    if (secound > 0)
                    {
                        if (secound < 10)
                            outTime += "0" + secound;
                        else
                            outTime += secound + "";
                    }
                    else
                    {
                        outTime += "00";
                    }
                }
                else if (minute > 0)
                {
                    outTime += minute + ":";
                    if (secound > 0)
                    {
                        if (secound < 10)
                            outTime += "0" + secound + "";
                        else
                            outTime += secound + "";
                    }
                    else
                    {
                        outTime += "00";
                    }
                }
                else if (secound > 0)
                {
                    outTime += secound;

                }

                if (speed == 0f)
                    outTime = "Infinity";
                timeText.text = outTime;
            }

        }

        IEnumerator UpdateBar()
        {
            WaitForSeconds delay = new WaitForSeconds(0.1f);
            WaitForSeconds delay2 = new WaitForSeconds(0.1f);
            while (Camera.main == null)
            {
                yield return delay2;
            }

            while (Camera.main != null)
            {
                transform.rotation = Camera.main.transform.rotation;
                UpdateProgressBar();
                float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
                if (distance < renderDistance)
                {
                    transform.rotation = Camera.main.transform.rotation;
                    if (value == valueMax)
                    {
                        if (GetComponent<Canvas>() != null)
                            transform.GetComponent<Canvas>().enabled = false;
                    }
                    else
                    {
                        if (GetComponent<Canvas>() != null)
                            transform.GetComponent<Canvas>().enabled = true;
                    }
                }
                else
                {
                    if (GetComponent<Canvas>() != null)
                        transform.GetComponent<Canvas>().enabled = false;
                }
                yield return delay;
            }
            yield return delay;

        }
        
        
      /*  IEnumerator UpdateTimer()
        {
            Debug.LogError("!!!!!!!!!!!!!!!!!! UpdateTimer cObject="+cObject);
            WaitForSeconds delay = new WaitForSeconds(1f);
            value++;
            UpdateProgressBar();
            yield return delay;

        }*/
    }
}