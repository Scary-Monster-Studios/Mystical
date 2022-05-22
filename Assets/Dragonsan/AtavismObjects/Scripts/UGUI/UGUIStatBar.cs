using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;

namespace Atavism
{
    public class UGUIStatBar : MonoBehaviour
    {

        public string prop;
        public string propMax;
        public bool forTarget = false;
        public Text label;
        public TextMeshProUGUI TMPLabel;
        public Image barImage;
        public Image barImageDelay;
        public Slider sliderBarImage;
        public Slider sliderBarImageDelay;
        [Range(0,1)]
        public float delaySpeed = 0.01f;
        int value = 1;
        int valueMax = 1;
        private bool mouseEntered;

        // Use this for initialization
        void Start()
        {
            if (forTarget)
            {
                AtavismEventSystem.RegisterEvent("PROPERTY_" + prop, this);
                AtavismEventSystem.RegisterEvent("PROPERTY_" + propMax, this);
                AtavismEventSystem.RegisterEvent("PLAYER_TARGET_CHANGED", this);
                AtavismEventSystem.RegisterEvent("OBJECT_TARGET_CHANGED", this);

                if (ClientAPI.GetTargetObject() != null)
                {
                    value = (int)ClientAPI.GetTargetObject().GetProperty(prop);
                    valueMax = (int)ClientAPI.GetTargetObject().GetProperty(propMax);
                    UpdateProgressBar();
                }
            }
            else
            {
                if (ClientAPI.GetPlayerObject() != null)
                {
                    ClientAPI.GetPlayerObject().RegisterPropertyChangeHandler(prop, PropHandler);
                    ClientAPI.GetPlayerObject().RegisterPropertyChangeHandler(propMax, PropMaxHandler);

                    if (ClientAPI.GetPlayerObject().PropertyExists(prop))
                    {
                        value = (int)ClientAPI.GetPlayerObject().GetProperty(prop);
                    }
                    if (ClientAPI.GetPlayerObject().PropertyExists(propMax))
                    {
                        valueMax = (int)ClientAPI.GetPlayerObject().GetProperty(propMax);
                    }
                    UpdateProgressBar();
                }
            }
        }

        void OnDestroy()
        {
            if (forTarget)
            {
                AtavismEventSystem.UnregisterEvent("PROPERTY_" + prop, this);
                AtavismEventSystem.UnregisterEvent("PROPERTY_" + propMax, this);
                AtavismEventSystem.UnregisterEvent("PLAYER_TARGET_CHANGED", this);
                AtavismEventSystem.UnregisterEvent("OBJECT_TARGET_CHANGED", this);
            }
            else
            {
                if (ClientAPI.GetPlayerObject() != null)
                {
                    ClientAPI.GetPlayerObject().RemovePropertyChangeHandler(prop, PropHandler);
                    ClientAPI.GetPlayerObject().RemovePropertyChangeHandler(propMax, PropMaxHandler);
                }
            }
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "PROPERTY_" + prop)
            {
                if (eData.eventArgs[0] == "target")
                {
                    value = (int)ClientAPI.GetTargetObject().GetProperty(prop);
                }
                //Debug.Log("Got health property: " + eData.eventArgs.Length + " with unit: " + eData.eventArgs[0]);
            }
            else if (eData.eventType == "PROPERTY_" + propMax)
            {
                if (eData.eventArgs[0] == "target")
                {
                    valueMax = (int)ClientAPI.GetTargetObject().GetProperty(propMax);
                }
            }
            else if (eData.eventType == "PLAYER_TARGET_CHANGED")
            {
                if (ClientAPI.GetTargetObject() != null)
                {
                    value = (int)ClientAPI.GetTargetObject().GetProperty(prop);
                    valueMax = (int)ClientAPI.GetTargetObject().GetProperty(propMax);
                    UpdateProgressBar();
                }
                else
                {
                    value = 100;
                    valueMax = 100;
                    UpdateProgressBar();
                }
            }else if (eData.eventType == "OBJECT_TARGET_CHANGED")
            {
              //  int id = int.Parse(eData.eventArgs[0]);                
               // int claimId = int.Parse(eData.eventArgs[1]);           
             //   Debug.Log("UGUI StatBar val=" + value + " max=" + valueMax);
                if(WorldBuilder.Instance.SelectedClaimObject!=null){
                    value = WorldBuilder.Instance.SelectedClaimObject.Health;
                    valueMax = WorldBuilder.Instance.SelectedClaimObject.MaxHealth;
                   // Debug.Log("UGUI StatBar val=" + value + " max=" + valueMax);
                    UpdateProgressBar();
                }
                else
                {
                    value = 100;
                    valueMax = 100;
                    UpdateProgressBar();
                }
            }
            UpdateProgressBar();
        }

        public void PropHandler(object sender, PropertyChangeEventArgs args)
        {
            value = (int)ClientAPI.GetPlayerObject().GetProperty(prop);
            UpdateProgressBar();
        }

        public void PropMaxHandler(object sender, PropertyChangeEventArgs args)
        {
            valueMax = (int)ClientAPI.GetPlayerObject().GetProperty(propMax);
            UpdateProgressBar();
        }

        void UpdateProgressBar()
        {
            if (sliderBarImage != null)
                sliderBarImage.value = (float)value / (float)valueMax;
            else if (GetComponent<Slider>() != null)
                GetComponent<Slider>().value = (float)value / (float)valueMax;
            if (barImage != null)
                barImage.fillAmount = (float)value / (float)valueMax;
            else  if (GetComponent<Image>() != null)
                GetComponent<Image>().fillAmount = (float)value / (float)valueMax;
            if(barImage != null)
                barImage.fillAmount = (float)value / (float)valueMax;
            if (label != null)
            {
                label.text = value + " / " + valueMax;
            }
            if (TMPLabel != null)
                TMPLabel.text = value + " / " + valueMax;

        }

        private void Update()
        {
            if (barImage != null && barImageDelay != null)
            {
              //  Debug.LogError("" + (barImageDelay.fillAmount - barImage.fillAmount) + " " + delaySpeed);

                if (barImage.fillAmount != barImageDelay.fillAmount)
                {
                    if (barImage.fillAmount < barImageDelay.fillAmount)
                    {
                      //  Debug.LogError(""+(barImageDelay.fillAmount - barImage.fillAmount)+" "+ delaySpeed);
                        if (barImageDelay.fillAmount - barImage.fillAmount < delaySpeed)
                            barImageDelay.fillAmount = barImage.fillAmount;
                        else
                            barImageDelay.fillAmount -= delaySpeed;
                    }
                    else
                    {
                        if (barImage.fillAmount - barImageDelay.fillAmount < delaySpeed)
                            barImageDelay.fillAmount = barImage.fillAmount;
                        else
                            barImageDelay.fillAmount += delaySpeed;

                    }
                }
            }
            if (sliderBarImage != null && sliderBarImageDelay != null)
            {
                if (sliderBarImage.value != sliderBarImageDelay.value)
                {
                    if (sliderBarImage.value < sliderBarImageDelay.value)
                    {
                        if (sliderBarImageDelay.value - sliderBarImage.value < delaySpeed)
                            sliderBarImageDelay.value = sliderBarImage.value;
                        else
                            sliderBarImageDelay.value -= delaySpeed;
                    }
                    else
                    {
                        if (sliderBarImage.value - sliderBarImageDelay.value < delaySpeed)
                            sliderBarImageDelay.value = sliderBarImage.value;
                        else
                            sliderBarImageDelay.value += delaySpeed;

                    }
                }
            }
        }

    }

}