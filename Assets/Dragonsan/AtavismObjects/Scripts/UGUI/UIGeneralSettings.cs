using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atavism
{

    public class UIGeneralSettings : MonoBehaviour
    {
        [SerializeField]
        Toggle freeCamera;
        [SerializeField]
        Image polishFlag;
        [SerializeField]
        Image englishFlag;
        [SerializeField] Slider sensitivityMouse;
        [SerializeField] Slider sensitivityWheelMouse;
        [SerializeField]
        Toggle showTitle;

        void OnEnable()
        {
        }
        public void updParam()
        {
            if(freeCamera)
                freeCamera.isOn = AtavismSettings.Instance.GetGeneralSettings().freeCamera;
           if(showTitle) showTitle.isOn = AtavismSettings.Instance.GetGeneralSettings().showTitle;
            updateFlags();
            if (sensitivityMouse != null)
                sensitivityMouse.value = AtavismSettings.Instance.GetGeneralSettings().sensitivityMouse;
            if (sensitivityWheelMouse != null)
                sensitivityWheelMouse.value = AtavismSettings.Instance.GetGeneralSettings().sensitivityWheelMouse;
            //      I2.Loc.LocalizationManager.CurrentLanguage
        }
        // Use this for initialization
        public void ChangeFreeCamera()
        {
            AtavismSettings.Instance.GetGeneralSettings().freeCamera = freeCamera.isOn;
        }

        public void ChangeShowTitle()
        {
            AtavismSettings.Instance.GetGeneralSettings().showTitle = showTitle.isOn;
        }


        public void SetLanguage(string _lang)
        {
#if AT_I2LOC_PRESET
        if (I2.Loc.LocalizationManager.HasLanguage(_lang)) {
            I2.Loc.LocalizationManager.CurrentLanguage = _lang;
            AtavismSettings.Instance.GetGeneralSettings().language = _lang;
        }
        string[] args = new string[1];
        AtavismEventSystem.DispatchEvent("UPDATE_LANGUAGE", args);
        AtavismSettings.Instance.GetGeneralSettings().language = I2.Loc.LocalizationManager.CurrentLanguage;
        updateFlags();
#endif
        }
        // Update is called once per frame
        void updateFlags()
        {
#if AT_I2LOC_PRESET

        if (I2.Loc.LocalizationManager.CurrentLanguage == "Polish") { if (polishFlag!=null) polishFlag.enabled = true; } else { if (polishFlag != null) polishFlag.enabled = false; }
        if (I2.Loc.LocalizationManager.CurrentLanguage == "English") { if (englishFlag != null) englishFlag.enabled = true; } else { if (englishFlag != null) englishFlag.enabled = false; }
#endif
        }
        public void ResetWindows()
        {
            AtavismSettings.Instance.ResetWindows();

        }
        public void SetSensitivityMouse(float v)
        {
            AtavismSettings.Instance.GetGeneralSettings().sensitivityMouse = v;
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("MOUSE_SENSITIVE", args);
        }
        public void SetSensitivityWheelMouse(float v)
        {
            AtavismSettings.Instance.GetGeneralSettings().sensitivityWheelMouse = v;
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("MOUSE_SENSITIVE", args);
        }
    }
}