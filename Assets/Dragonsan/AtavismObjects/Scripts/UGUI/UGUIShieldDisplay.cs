using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Atavism
{

    public class UGUIShieldDisplay : MonoBehaviour
    {
        public GameObject amountObject;
        public Image imageAmountFill;
        public Slider sliderAmountFill;
        public Text amountText;
        public TextMeshProUGUI TMPAmountText;
        public GameObject hitCountObject;
        public Image imageHitCountFill;
        public Slider sliderHitCountFill;
        public Text hitCountText;
        public TextMeshProUGUI TMPHitCountText;
        [SerializeField] string shieldName;
        //[SerializeField] bool showAmount = true;
        public CanvasGroup canvasGroup;
        // Start is called before the first frame update
        void Start()
        {
            NetworkAPI.RegisterExtensionMessageHandler("shieldUpdate", HandleShieldListUpdate);
            if (canvasGroup)
                canvasGroup.alpha = 0;
            if (amountObject)
                amountObject.SetActive(false);
            if (hitCountObject)
                hitCountObject.SetActive(false);


            if (sliderAmountFill != null)
                sliderAmountFill.value = 0;
            if (imageAmountFill != null)
                imageAmountFill.fillAmount = 0;
            if (sliderHitCountFill != null)
                sliderHitCountFill.value = 0;
            if (imageHitCountFill != null)
                imageHitCountFill.fillAmount = 0;
        }

        private void HandleShieldListUpdate(Dictionary<string, object> props)
        {
            if (props["name"].Equals(shieldName))
            {
                int sMax = (int)(props["sMax"]);
                int sCur = (int)(props["sCur"]);
                int cMax = (int)(props["cMax"]);
                int cCur = (int)(props["cCur"]);
                Debug.LogWarning("Shield " + shieldName + " sMax=" + sMax + " sCur=" + sCur + " cMax=" + cMax + " cCur=" + cCur);

                if (sliderAmountFill != null && sMax > 0)
                    sliderAmountFill.value = 1 - ((float)sCur / (float)sMax);
                if (imageAmountFill != null && sMax > 0)
                    imageAmountFill.fillAmount = /*1 - */((float)sCur / (float)sMax);
                if (amountText && sMax > 0)
                    amountText.text = sCur + " / " + sMax;
                if (TMPAmountText && sMax > 0)
                    TMPAmountText.text = sCur + " / " + sMax;

                if (sliderHitCountFill != null && cMax > 0)
                    sliderHitCountFill.value = 1 - ((float)cCur / (float)cMax);
                if (imageHitCountFill != null && cMax > 0)
                    imageHitCountFill.fillAmount = /*1 - */((float)cCur / (float)cMax);
                if (hitCountText && cMax > 0)
                    hitCountText.text = cCur + " / " + cMax;
                if (TMPHitCountText && cMax > 0)
                    TMPHitCountText.text = cCur + " / " + cMax;
                if (amountObject)
                {
                    if ((sCur <= 0 && sMax > 0)||sMax==-1)
                        amountObject.SetActive(false);
                    else
                        amountObject.SetActive(true);
                }
                if (hitCountObject)
                {
                    if ((cCur <= 0))
                        hitCountObject.SetActive(false);
                    else
                        hitCountObject.SetActive(true);
                }

                if (canvasGroup)
                {
                    if ((sCur == 0 && sMax > 0) || (cCur == 0 && cMax > 0))
                        canvasGroup.alpha = 0;
                    else
                        canvasGroup.alpha = 1;

                }
            }
        }

        private void OnDestroy()
        {
            NetworkAPI.RemoveExtensionMessageHandler("shieldUpdate", HandleShieldListUpdate);
        }
    
    }
}