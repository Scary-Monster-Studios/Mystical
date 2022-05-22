using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Atavism
{
    public class UGUIClaimListEntry : MonoBehaviour
    {
        public Button buttonPay;
        public TextMeshProUGUI TMPButtonText;
        public TextMeshProUGUI TMPName;
        public TextMeshProUGUI TMPTaxPayedTime;
        public TextMeshProUGUI TMPTaxInfo;
        public UGUIMiniTooltipEvent tooltip;
        private float time = 0;
        private int id = -1;
        private long taxAmount ;
        private long taxInterval;
        private long taxPeriodPay;
        private int taxCurrency;
        // Update is called once per frame
        public void UpdateDisplay(string name, int claimId, float time,long taxAmount,int taxCurrency,long taxInterval,long taxPeriodPay)
        {
         //   Debug.LogError("UGUIClaimListEntry.UpdateDisplay: name="+name+" claimId="+claimId+" time="+time+" taxAmount="+taxAmount+" taxCurrency="+taxCurrency+" taxInterval="+taxInterval+" taxPeriodPay="+taxPeriodPay);
            id = claimId;
            this.time = time;
            this.taxAmount = taxAmount;
            this.taxCurrency = taxCurrency;
            this.taxInterval = taxInterval;
            this.taxPeriodPay = taxPeriodPay;
            if (TMPName != null)
                TMPName.text = name;
            if (tooltip != null)
                tooltip.dectName = name;
         
            long days = 0;
            long hour = 0;
            if (taxInterval > 24)
            {
                days = (long) (taxInterval / 24F);
                hour =  (taxInterval - (days * 24));
            }
            else
            {
                hour = taxInterval;
            }
            string cost = Inventory.Instance.GetCostString(taxCurrency, taxAmount);
            
            if (taxAmount > 0)
            {
                
                long _days = 0;
                long _hour = 0;
                if (taxPeriodPay > 24)
                {
                    _days = (long) (taxPeriodPay / 24F);
                    _hour =  (taxPeriodPay - (_days * 24));
                }
                else
                {
                    _hour = taxPeriodPay;
                }
#if AT_I2LOC_PRESET
                TMPTaxInfo.text = cost + " "+I2.Loc.LocalizationManager.GetTranslation("per")+" " + (days > 0 ? days > 1 ? days + " "+I2.Loc.LocalizationManager.GetTranslation("days")+" " : days + " "+
                        I2.Loc.LocalizationManager.GetTranslation("day")+" " : "") + (hour > 0 ? hour + " "+I2.Loc.LocalizationManager.GetTranslation("hour") : "")+
                                  ". "+I2.Loc.LocalizationManager.GetTranslation("Can be paid")+" "+ (_days > 0 ? _days > 1 ? _days + " days " : _days + " day " : "") + (_hour > 0 ? _hour + " hour" : "")+" "+
                                 I2.Loc.LocalizationManager.GetTranslation("before tax expire");
#else
                TMPTaxInfo.text = cost + " per " + (days > 0 ? days > 1 ? days + " days " : days + " day " : "") + (hour > 0 ? hour + " hour" : "")+
                                  ". Can be paid "+ (_days > 0 ? _days > 1 ? _days + " days " : _days + " day " : "") + (_hour > 0 ? _hour + " hour" : "")+" before tax expires";

#endif
            }
            else
            {
#if AT_I2LOC_PRESET
                TMPTaxInfo.text = I2.Loc.LocalizationManager.GetTranslation("No Tax");
#else
                TMPTaxInfo.text = "No tax";

#endif
            }

            if (taxAmount <=0)
            {
                if (buttonPay != null)
                    buttonPay.gameObject.SetActive(false);
            }
            else
            {
                if (buttonPay != null)
                    buttonPay.gameObject.SetActive(true);
            }
            StopAllCoroutines();
            StartCoroutine(UpdateTimer());
        }

        IEnumerator UpdateTimer()
        {
          //  Debug.LogError("!!!!!!!!!!!!!!!!!! UpdateTimer "+time);
            WaitForSeconds delay = new WaitForSeconds(1f);
            while (true)
            {
                if (time > Time.time)
                {
                    float _time = time - Time.time;
                    int days = 0;
                    int hour = 0;
                    int minute = 0;
                    int secound = 0;
                    if (_time > 24 * 3600)
                    {
                        days = (int) (_time / (24F * 3600F));
                    }

                    if (_time - days * 24 * 3600 > 3600)
                        hour = (int) ((_time - days * 24 * 3600) / 3600F);
                    if (_time - days * 24 * 3600 - hour * 3600 > 60)
                        minute = (int) (_time - days * 24 * 3600 - hour * 3600) / 60;
                    secound = (int) (_time - days * 24 * 3600 - hour * 3600 - minute * 60);

#if AT_I2LOC_PRESET
                     TMPTaxPayedTime.text = (days > 0 ? days > 1 ? days + " "+I2.Loc.LocalizationManager.GetTranslation("days")+" " : days + " "+
                        I2.Loc.LocalizationManager.GetTranslation("day")+" " : "") + (hour > 0 ? hour + " "+I2.Loc.LocalizationManager.GetTranslation("hour") : "") + (minute > 0 ? minute + " "+I2.Loc.LocalizationManager.GetTranslation("minute") : "");
#else
                    TMPTaxPayedTime.text = (days > 0 ? days > 1 ? days + " days " : days + " day " : "") + (hour > 0 ? hour + " h " : "") + (minute > 0 ? minute + " m" : "");

#endif
                    if (time - Time.time > taxPeriodPay * 3600F)
                    {
                        if (buttonPay)
                            buttonPay.interactable = false;
                        if (TMPButtonText)
                        {
                            float t = time - Time.time - taxPeriodPay * 3600f;

                            days = 0;
                            hour = 0;
                            minute = 0;
                            secound = 0;
                            if (_time > 24 * 3600)
                            {
                                days = (int) (t / (24F * 3600F));
                            }

                            if (t - days * 24 * 3600 > 3600)
                                hour = (int) ((t - days * 24 * 3600) / 3600F);
                            if (t - days * 24 * 3600 - hour * 3600 > 60)
                                minute = (int) (t - days * 24 * 3600 - hour * 3600) / 60;
                            secound = (int) (t - days * 24 * 3600 - hour * 3600 - minute * 60);

                            TMPButtonText.text = "Pay" + (days > 0 ? "(" + days + "days)" : hour > 0 ? "(" + hour + "h)" : minute > 0 ? "(" + minute + "m)" : secound > 0 ? "(" + secound + "s)" : "") + "";
                        }
                    }
                    else
                    {
                        if (buttonPay != null)
                        {
                            buttonPay.interactable = true;
                        }

                        if (TMPButtonText)
                        {
#if AT_I2LOC_PRESET
                     TMPButtonText.text = I2.Loc.LocalizationManager.GetTranslation("PAY");
#else
                            TMPButtonText.text = "PAY";
#endif
                        }
                    }
                }
                else
                {
                    if (TMPTaxPayedTime != null)
                    {
                        TMPTaxPayedTime.text = "";
                    }
                }

                yield return delay;
            }
        }
        public void Click()
        {
            WorldBuilder.Instance.SendPayTaxForClaim(id, false);
        }

    }
}

