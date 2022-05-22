using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using TMPro;
//using VolumetricFogAndMist;

namespace Atavism
{

    public class UIGraphicSettings : MonoBehaviour
    {
        [AtavismSeparator("Resolution")]
        // public Dropdown monitorSelect;
        [SerializeField] TMP_Dropdown resolutionSelect;
        [SerializeField] Toggle fullscreenToggle;
        [SerializeField] Toggle fpsToggle;
        [AtavismSeparator("Quality")]
        [SerializeField] TMP_Dropdown qualitySelect;
        [SerializeField] TMP_Dropdown shadowsDropdown;
        [SerializeField] TMP_Dropdown shadowResolutionDropdown;
        [SerializeField] TMP_Dropdown shadowDistanceDropdown;
        [SerializeField] Toggle verticalSyncToggle;
        [SerializeField] TMP_Dropdown lodBiasDropdown;
        [SerializeField] TMP_Dropdown particleRaycastBudgetDropdown;
        [SerializeField] TMP_Dropdown masterTextureLimitDropdown;
        [SerializeField] Toggle softParticlesToggle;
        [AtavismSeparator("Effects")]
        [SerializeField] TMP_Dropdown antialiasingDropdown;
        [SerializeField] Toggle ambiertOcclusionsToggle;
        [SerializeField] Toggle screenSpaceToggle;
        [SerializeField] Toggle volumetricFogToggle;
        [SerializeField] Toggle volumetricCameraToggle;
        [SerializeField] Toggle depthOfFieldToggle;
        [SerializeField] Toggle bloomToggle;
        [SerializeField] Toggle vignetteToggle;
        [SerializeField] Toggle chromaticAberrationToggle;
        [SerializeField] Toggle motionBlurToggle;
        [SerializeField] Toggle autoExposureToggle;
        [SerializeField] Toggle colorGradingToggle;
        [SerializeField] Toggle ditheringToggle;
        [SerializeField] Toggle screenSpaceReflectionsToggle;
        [AtavismSeparator("Terrain")]
        [SerializeField] Slider grassSlider;
        [AtavismSeparator("Wather")]
        // public Dropdown warterQualitySelect;
        // public Toggle ReflectionsToggle;
        //  public Dropdown waterResolutionSelect;
        //  public Dropdown waterRenderingSelect;
        //   private Display[] displays;
        private Resolution[] resolutionsAll;
        private List<Resolution> resolutions = new List<Resolution>();
        private bool fulscr;
        private float startTimer = 0f;

        void OnEnable()
        {
            startTimer = Time.time + 0.5f;
            updateResolutions();
            updParam();
        }
        void updateResolutions()
        {
            List<TMP_Dropdown.OptionData> Options2 = new List<TMP_Dropdown.OptionData>();

            resolutionsAll = Screen.resolutions;
          //  Debug.LogError("Grpthic updParam resolutionsAll=" + resolutionsAll + " " + resolutionsAll.Length);
            resolutionSelect.ClearOptions();
            resolutions.Clear();
            var m = 0;
            int k = 0;
            List<string> resol = new List<string>();
            foreach (Resolution res in resolutionsAll)
            {
                if (!resol.Contains(res.width + "x" + res.height))
                {
                    resol.Add(res.width + "x" + res.height);
                    resolutions.Add(res);
                    Options2.Add(new TMP_Dropdown.OptionData(res.width + "x" + res.height));
                    if (Screen.width == res.width && Screen.height == res.height)
                    {
                        k = m;
                    }
                    m++;
                }
            }
            resol.Clear();
            resolutionSelect.AddOptions(Options2);
            resolutionSelect.value = k;
        }
        public void updParam()
        {
            startTimer = Time.time + 0.5f;
            fpsToggle.isOn = AtavismSettings.Instance.GetVideoSettings().fps;
            /*     displays = Display.displays;
              //   monitorSelect.options.Clear();
                 int i = 0;
                 foreach (Display disp in displays)
                 {
                     //   Debug.Log(disp);
                     monitorSelect.options.Add(new Dropdown.OptionData("Display " + i++));
                     //monitorSelect.SelectOption();
                 }
         #if UNITY_EDITOR
                 monitorSelect.value = 0;
         #else
                 monitorSelect.value = 0;
         #endif
         */
            fullscreenToggle.isOn = Screen.fullScreen;
            List<TMP_Dropdown.OptionData> Options2 = new List<TMP_Dropdown.OptionData>();

            resolutionsAll = Screen.resolutions;
           // Debug.LogError("Grpthic updParam resolutionsAll="+ resolutionsAll+" "+ resolutionsAll.Length);
            //resolutionSelect.ClearOptions();
         //   resolutions.Clear();
            var m = 0;
            int k = 0;
            List<string> resol = new List<string>();
            foreach (Resolution res in resolutionsAll)
            {
                if (!resol.Contains(res.width + "x" + res.height))
                {
                    resol.Add(res.width + "x" + res.height);
                   // resolutions.Add(res);
                //    Options2.Add(new TMP_Dropdown.OptionData(res.width + "x" + res.height));
                    if (Screen.width == res.width && Screen.height == res.height)
                    {
                        k = m;
                    }
                    m++;
                }
            }
            resol.Clear();
           // resolutionSelect.AddOptions(Options2);
            resolutionSelect.value = k;
            Options2.Clear();
            qualitySelect.ClearOptions();
            for (int ii = 0; ii < QualitySettings.names.Length; ii++)
            {
#if AT_I2LOC_PRESET
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation(QualitySettings.names[ii])));
#else
                Options2.Add(new TMP_Dropdown.OptionData(QualitySettings.names[ii]));
#endif
            }
#if AT_I2LOC_PRESET
       Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("Custom")));
#else
            Options2.Add(new TMP_Dropdown.OptionData("Custom"));
#endif
            qualitySelect.AddOptions(Options2);
            if (AtavismSettings.Instance.GetVideoSettings().customSettings)
                qualitySelect.value = QualitySettings.names.Length;
            else
                qualitySelect.value = QualitySettings.GetQualityLevel();
            //  List<string> Options = new List<string>();

            if (!AtavismSettings.Instance.GetVideoSettings().customSettings)
            {
                switch (QualitySettings.shadows)
                {
                    case ShadowQuality.Disable:
                        AtavismSettings.Instance.GetVideoSettings().shadows = 0;
                        break;
                    case ShadowQuality.HardOnly:
                        AtavismSettings.Instance.GetVideoSettings().shadows = 1;
                        break;
                    case ShadowQuality.All:
                        AtavismSettings.Instance.GetVideoSettings().shadows = 2;
                        break;
                }

                AtavismSettings.Instance.GetVideoSettings().shadowDistance = (int)QualitySettings.shadowDistance;
                switch (QualitySettings.shadowResolution)
                {
                    case ShadowResolution.Low:
                        AtavismSettings.Instance.GetVideoSettings().shadowResolution = 0;
                        break;
                    case ShadowResolution.Medium:
                        AtavismSettings.Instance.GetVideoSettings().shadowResolution = 1;
                        break;
                    case ShadowResolution.High:
                        AtavismSettings.Instance.GetVideoSettings().shadowResolution = 2;
                        break;
                    case ShadowResolution.VeryHigh:
                        AtavismSettings.Instance.GetVideoSettings().shadowResolution = 3;
                        break;
                }
                AtavismSettings.Instance.GetVideoSettings().verticalSync = QualitySettings.vSyncCount;
                if (QualitySettings.lodBias == 2f)
                    AtavismSettings.Instance.GetVideoSettings().lodBias = 5;
                if (QualitySettings.lodBias == 1.5f)
                    AtavismSettings.Instance.GetVideoSettings().lodBias = 4;
                if (QualitySettings.lodBias == 1f)
                    AtavismSettings.Instance.GetVideoSettings().lodBias = 3;
                if (QualitySettings.lodBias == 0.7f)
                    AtavismSettings.Instance.GetVideoSettings().lodBias = 2;
                if (QualitySettings.lodBias == 0.4f)
                    AtavismSettings.Instance.GetVideoSettings().lodBias = 1;
                if (QualitySettings.lodBias == 0.3f)
                    AtavismSettings.Instance.GetVideoSettings().lodBias = 0;


                switch (QualitySettings.particleRaycastBudget)
                {
                    case 4096:
                        AtavismSettings.Instance.GetVideoSettings().particleRaycastBudget = 5;
                        break;
                    case 1024:
                        AtavismSettings.Instance.GetVideoSettings().particleRaycastBudget = 4;
                        break;
                    case 256:
                        AtavismSettings.Instance.GetVideoSettings().particleRaycastBudget = 3;
                        break;
                    case 64:
                        AtavismSettings.Instance.GetVideoSettings().particleRaycastBudget = 2;
                        break;
                    case 16:
                        AtavismSettings.Instance.GetVideoSettings().particleRaycastBudget = 1;
                        break;
                    case 4:
                        AtavismSettings.Instance.GetVideoSettings().particleRaycastBudget = 0;
                        break;
                }

                AtavismSettings.Instance.GetVideoSettings().masterTextureLimit = 3 - QualitySettings.masterTextureLimit;
                AtavismSettings.Instance.GetVideoSettings().softParticles = QualitySettings.softParticles;
            }


            if (shadowsDropdown != null)
            {
                Options2.Clear();
                shadowsDropdown.ClearOptions();
#if AT_I2LOC_PRESET
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("Disable")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("HardOnly")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("All")));
#else
                Options2.Add(new TMP_Dropdown.OptionData("Disable"));
                Options2.Add(new TMP_Dropdown.OptionData("HardOnly"));
                Options2.Add(new TMP_Dropdown.OptionData("All"));
#endif
                shadowsDropdown.AddOptions(Options2);
                shadowsDropdown.value = AtavismSettings.Instance.GetVideoSettings().shadows;
            }
            if (shadowDistanceDropdown != null)
            {
                Options2.Clear();
                shadowDistanceDropdown.ClearOptions();
#if AT_I2LOC_PRESET
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("VeryLow")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("Low")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("Medium")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("High")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("VeryHigh")));
#else
                Options2.Add(new TMP_Dropdown.OptionData("VeryLow"));
                Options2.Add(new TMP_Dropdown.OptionData("Low"));
                Options2.Add(new TMP_Dropdown.OptionData("Medium"));
                Options2.Add(new TMP_Dropdown.OptionData("High"));
                Options2.Add(new TMP_Dropdown.OptionData("VeryHigh"));
#endif
                shadowDistanceDropdown.AddOptions(Options2);
                shadowDistanceDropdown.value = AtavismSettings.Instance.GetVideoSettings().shadowDistance;
            }

            if (shadowResolutionDropdown != null)
            {
                Options2.Clear();
                shadowResolutionDropdown.ClearOptions();
#if AT_I2LOC_PRESET
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("Low")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("Medium")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("High")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("VeryHigh")));
#else
                Options2.Add(new TMP_Dropdown.OptionData("Low"));
                Options2.Add(new TMP_Dropdown.OptionData("Medium"));
                Options2.Add(new TMP_Dropdown.OptionData("High"));
                Options2.Add(new TMP_Dropdown.OptionData("VeryHigh"));
#endif
                shadowResolutionDropdown.AddOptions(Options2);
                shadowResolutionDropdown.value = AtavismSettings.Instance.GetVideoSettings().shadowResolution;
            }
            verticalSyncToggle.isOn = AtavismSettings.Instance.GetVideoSettings().verticalSync == 1 ? true : false;

            if (lodBiasDropdown != null)
            {
                Options2.Clear();
                lodBiasDropdown.ClearOptions();
#if AT_I2LOC_PRESET
           Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("VeryLow")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("Low")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("Medium")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("High")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("VeryHigh")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("UltraHigh")));
#else
                Options2.Add(new TMP_Dropdown.OptionData("VeryLow"));
                Options2.Add(new TMP_Dropdown.OptionData("Low"));
                Options2.Add(new TMP_Dropdown.OptionData("Medium"));
                Options2.Add(new TMP_Dropdown.OptionData("High"));
                Options2.Add(new TMP_Dropdown.OptionData("VeryHigh"));
                Options2.Add(new TMP_Dropdown.OptionData("UltraHigh"));
#endif
                lodBiasDropdown.AddOptions(Options2);
                lodBiasDropdown.value = AtavismSettings.Instance.GetVideoSettings().lodBias;
            }
            if (particleRaycastBudgetDropdown != null)
            {
                Options2.Clear();
                particleRaycastBudgetDropdown.ClearOptions();
#if AT_I2LOC_PRESET
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("VeryLow")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("Low")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("Medium")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("High")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("VeryHigh")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("UltraHigh")));
#else
                Options2.Add(new TMP_Dropdown.OptionData("VeryLow"));
                Options2.Add(new TMP_Dropdown.OptionData("Low"));
                Options2.Add(new TMP_Dropdown.OptionData("Medium"));
                Options2.Add(new TMP_Dropdown.OptionData("High"));
                Options2.Add(new TMP_Dropdown.OptionData("VeryHigh"));
                Options2.Add(new TMP_Dropdown.OptionData("UltraHigh"));

#endif
                particleRaycastBudgetDropdown.AddOptions(Options2);
                particleRaycastBudgetDropdown.value = AtavismSettings.Instance.GetVideoSettings().particleRaycastBudget;
            }


            if (masterTextureLimitDropdown != null)
            {
                Options2.Clear();
                masterTextureLimitDropdown.ClearOptions();
#if AT_I2LOC_PRESET
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("Low")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("Medium")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("High")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("VeryHigh")));
#else
                Options2.Add(new TMP_Dropdown.OptionData("Low"));
                Options2.Add(new TMP_Dropdown.OptionData("Medium"));
                Options2.Add(new TMP_Dropdown.OptionData("High"));
                Options2.Add(new TMP_Dropdown.OptionData("VeryHigh"));
#endif
                masterTextureLimitDropdown.AddOptions(Options2);
                masterTextureLimitDropdown.value = AtavismSettings.Instance.GetVideoSettings().masterTextureLimit;
            }

            if (shadowDistanceDropdown != null)
            {
                Options2.Clear();
                antialiasingDropdown.ClearOptions();
#if AT_I2LOC_PRESET
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("None")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("Fast Approximate")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("Subpixel Morphological")));
            Options2.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation("Temporal")));
#else
                Options2.Add(new TMP_Dropdown.OptionData("None"));
                Options2.Add(new TMP_Dropdown.OptionData("Fast Approximate"));
                Options2.Add(new TMP_Dropdown.OptionData("Subpixel Morphological"));
                Options2.Add(new TMP_Dropdown.OptionData("Temporal"));

#endif
                antialiasingDropdown.AddOptions(Options2);
                antialiasingDropdown.value = AtavismSettings.Instance.GetVideoSettings().antialiasing;
            }

            if (ambiertOcclusionsToggle)
                ambiertOcclusionsToggle.isOn = AtavismSettings.Instance.GetVideoSettings().ambientOcclusion;
            if (softParticlesToggle)
                softParticlesToggle.isOn = AtavismSettings.Instance.GetVideoSettings().softParticles;
            // antialiasingToggle.isOn = AtavismSettings.Instance.GetVideoSettings().antialiasing;
            if (depthOfFieldToggle)
                depthOfFieldToggle.isOn = AtavismSettings.Instance.GetVideoSettings().depthOfField;
            if (vignetteToggle)
                vignetteToggle.isOn = AtavismSettings.Instance.GetVideoSettings().vignette;
            if (bloomToggle)
                bloomToggle.isOn = AtavismSettings.Instance.GetVideoSettings().bloom;
            if (chromaticAberrationToggle)
                chromaticAberrationToggle.isOn = AtavismSettings.Instance.GetVideoSettings().chromaticAberration;
            if (motionBlurToggle)
                motionBlurToggle.isOn = AtavismSettings.Instance.GetVideoSettings().motionBlur;
            if (autoExposureToggle)
                autoExposureToggle.isOn = AtavismSettings.Instance.GetVideoSettings().autoExposure;
            if (colorGradingToggle)
                colorGradingToggle.isOn = AtavismSettings.Instance.GetVideoSettings().colorGrading;
            if (ditheringToggle)
                ditheringToggle.isOn = AtavismSettings.Instance.GetVideoSettings().dithering;
            if (screenSpaceReflectionsToggle)
                screenSpaceReflectionsToggle.isOn = AtavismSettings.Instance.GetVideoSettings().screenSpaceReflections;
            // amplifyOcclusionEffectToggle.isOn = AtavismSettings.Instance.GetVideoSettings().amplifyOcclusionEffect;
            if (screenSpaceToggle)
                screenSpaceToggle.isOn = AtavismSettings.Instance.GetVideoSettings().seScreenSpaceShadows;
            if (volumetricCameraToggle)
                volumetricCameraToggle.isOn = AtavismSettings.Instance.GetVideoSettings().hxVolumetricCamera;
            if (volumetricFogToggle)
                volumetricFogToggle.isOn = AtavismSettings.Instance.GetVideoSettings().volumetricFog;


        }

        public void ChangeAntialiasing()
        {
            /*switch(antialiasingSelect.value)
             {
                 case 0:
                     QualitySettings.antiAliasing = 0;
                     break;
                 case 1:
                     QualitySettings.antiAliasing = 2;
                     break;
                 case 2:
                     QualitySettings.antiAliasing = 4;
                     break;
                 case 3:
                     QualitySettings.antiAliasing = 8;
                     break;
             }*/
        }

        public void ChangeFps()
        {
            if (startTimer > Time.time)
                return;
            AtavismSettings.Instance.GetVideoSettings().fps = fpsToggle.isOn;

        }
        /*   public void ChangeMonitor() {
               if (startTimer > Time.time) return;
               displays[monitorSelect.value].Activate();
           }*/
        float setQualityTime = 0;

        public void ChangeQuality()
        {
            if (startTimer > Time.time)
                return;
            if (qualitySelect.value == QualitySettings.names.Length)
            {
                AtavismSettings.Instance.GetVideoSettings().customSettings = true;
            }
            else
            {
                AtavismSettings.Instance.GetVideoSettings().customSettings = false;
                AtavismSettings.Instance.GetVideoSettings().quality = qualitySelect.value;
                QualitySettings.SetQualityLevel(qualitySelect.value, true);
                //Apply default values
                AtavismQualitySetingsDefault q = AtavismSettings.Instance.GetDefaultQuality(qualitySelect.value);
                QualitySettings.pixelLightCount = q.pixelLightCount;
                QualitySettings.masterTextureLimit = q.masterTextureLimit;
                QualitySettings.anisotropicFiltering = q.anisotropicFiltering;
                QualitySettings.softParticles = q.softParticles;
                QualitySettings.realtimeReflectionProbes = q.realtimeReflectionProbes;
                QualitySettings.billboardsFaceCameraPosition = q.billboardsFaceCameraPosition;
                QualitySettings.shadows = q.shadows;
                QualitySettings.shadowResolution = q.shadowResolution;
                QualitySettings.shadowDistance = q.shadowDistance;
                QualitySettings.shadowCascades = q.shadowCascades;
                QualitySettings.skinWeights = q.blendWeights;
                QualitySettings.vSyncCount = q.verticalSync;
                QualitySettings.lodBias = q.lodBias;
                QualitySettings.particleRaycastBudget = q.particleRaycastBudget;

                setQualityTime = Time.time + .1f;
                switch (QualitySettings.shadows)
                {
                    case ShadowQuality.Disable:
                        AtavismSettings.Instance.GetVideoSettings().shadows = 0;
                        break;
                    case ShadowQuality.HardOnly:
                        AtavismSettings.Instance.GetVideoSettings().shadows = 1;
                        break;
                    case ShadowQuality.All:
                        AtavismSettings.Instance.GetVideoSettings().shadows = 2;
                        break;
                }

                AtavismSettings.Instance.GetVideoSettings().shadowDistance = (int)QualitySettings.shadowDistance;
                switch (QualitySettings.shadowResolution)
                {
                    case ShadowResolution.Low:
                        AtavismSettings.Instance.GetVideoSettings().shadowResolution = 0;
                        break;
                    case ShadowResolution.Medium:
                        AtavismSettings.Instance.GetVideoSettings().shadowResolution = 1;
                        break;
                    case ShadowResolution.High:
                        AtavismSettings.Instance.GetVideoSettings().shadowResolution = 2;
                        break;
                    case ShadowResolution.VeryHigh:
                        AtavismSettings.Instance.GetVideoSettings().shadowResolution = 3;
                        break;
                }
                AtavismSettings.Instance.GetVideoSettings().verticalSync = QualitySettings.vSyncCount;
                if (QualitySettings.lodBias == 2f)
                    AtavismSettings.Instance.GetVideoSettings().lodBias = 5;
                if (QualitySettings.lodBias == 1.5f)
                    AtavismSettings.Instance.GetVideoSettings().lodBias = 4;
                if (QualitySettings.lodBias == 1f)
                    AtavismSettings.Instance.GetVideoSettings().lodBias = 3;
                if (QualitySettings.lodBias == 0.7f)
                    AtavismSettings.Instance.GetVideoSettings().lodBias = 2;
                if (QualitySettings.lodBias == 0.4f)
                    AtavismSettings.Instance.GetVideoSettings().lodBias = 1;
                if (QualitySettings.lodBias == 0.3f)
                    AtavismSettings.Instance.GetVideoSettings().lodBias = 0;


                switch (QualitySettings.particleRaycastBudget)
                {
                    case 4096:
                        AtavismSettings.Instance.GetVideoSettings().particleRaycastBudget = 5;
                        break;
                    case 1024:
                        AtavismSettings.Instance.GetVideoSettings().particleRaycastBudget = 4;
                        break;
                    case 256:
                        AtavismSettings.Instance.GetVideoSettings().particleRaycastBudget = 3;
                        break;
                    case 64:
                        AtavismSettings.Instance.GetVideoSettings().particleRaycastBudget = 2;
                        break;
                    case 16:
                        AtavismSettings.Instance.GetVideoSettings().particleRaycastBudget = 1;
                        break;
                    case 4:
                        AtavismSettings.Instance.GetVideoSettings().particleRaycastBudget = 0;
                        break;
                }

                AtavismSettings.Instance.GetVideoSettings().masterTextureLimit = 3 - QualitySettings.masterTextureLimit;
                AtavismSettings.Instance.GetVideoSettings().softParticles = QualitySettings.softParticles;
                updParam();
            }
        }
        public void ChangeShadows()
        {
            if (startTimer > Time.time)
                return;

            // AtavismSettings.Instance.GetVideoSettings().customSettings = true;
            if (setQualityTime < Time.time)
            {
                if (qualitySelect != null)
                    qualitySelect.value = QualitySettings.names.Length;

                AtavismSettings.Instance.GetVideoSettings().shadows = (int)shadowsDropdown.value;
                switch ((int)shadowsDropdown.value)
                {
                    case 0:
                        QualitySettings.shadows = ShadowQuality.Disable;
                        break;
                    case 1:
                        QualitySettings.shadows = ShadowQuality.HardOnly;
                        break;
                    case 2:
                        QualitySettings.shadows = ShadowQuality.All;
                        break;
                }
            }
          //  updParam();
        }
        public void ChangeShadowsDist()
        {
            if (startTimer > Time.time)
                return;
            if (setQualityTime < Time.time)
            {
                qualitySelect.value = QualitySettings.names.Length;
                AtavismSettings.Instance.GetVideoSettings().shadowDistance = (int)shadowDistanceDropdown.value;
                switch ((int)shadowDistanceDropdown.value)
                {
                    case 0:
                        QualitySettings.shadowDistance = 50;
                        break;
                    case 1:
                        QualitySettings.shadowDistance = 100;
                        break;
                    case 2:
                        QualitySettings.shadowDistance = 150;
                        break;
                    case 3:
                        QualitySettings.shadowDistance = 300;
                        break;
                    case 4:
                        QualitySettings.shadowDistance = 500;
                        break;
                }
            }
           // updParam();
        }
        public void ChangeShadowsRez()
        {
            if (startTimer > Time.time)
                return;
            if (setQualityTime < Time.time)
            {
                qualitySelect.value = QualitySettings.names.Length;
                AtavismSettings.Instance.GetVideoSettings().shadowResolution = (int)shadowResolutionDropdown.value;
                switch ((int)shadowResolutionDropdown.value)
                {
                    case 0:
                        QualitySettings.shadowResolution = ShadowResolution.Low;
                        break;
                    case 1:
                        QualitySettings.shadowResolution = ShadowResolution.Medium;
                        break;
                    case 2:
                        QualitySettings.shadowResolution = ShadowResolution.High;
                        break;
                    case 3:
                        QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                        break;
                }
            }
          //  updParam();
        }

        public void ChangeSync()
        {
            if (startTimer > Time.time)
                return;
            if (setQualityTime < Time.time)
            {
                qualitySelect.value = QualitySettings.names.Length;
                AtavismSettings.Instance.GetVideoSettings().verticalSync = verticalSyncToggle.isOn ? 1 : 0;
                QualitySettings.vSyncCount = verticalSyncToggle.isOn ? 1 : 0;
            }
           // updParam();
        }

        public void ChangeLodbias()
        {
            if (startTimer > Time.time)
                return;
            if (setQualityTime < Time.time)
            {
                qualitySelect.value = QualitySettings.names.Length;
                AtavismSettings.Instance.GetVideoSettings().lodBias = (int)lodBiasDropdown.value;
                switch ((int)lodBiasDropdown.value)
                {
                    case 0:
                        QualitySettings.lodBias = 0.3f;
                        break;
                    case 1:
                        QualitySettings.lodBias = 0.4f;
                        break;
                    case 2:
                        QualitySettings.lodBias = 0.7f;
                        break;
                    case 3:
                        QualitySettings.lodBias = 1f;
                        break;
                    case 4:
                        QualitySettings.lodBias = 1.5f;
                        break;
                    case 5:
                        QualitySettings.lodBias = 2f;
                        break;
                }
            }
          //  updParam();
        }
        public void ChangeBudget()
        {
            if (startTimer > Time.time)
                return;
            if (setQualityTime < Time.time)
            {
                qualitySelect.value = QualitySettings.names.Length;
                AtavismSettings.Instance.GetVideoSettings().particleRaycastBudget = (int)particleRaycastBudgetDropdown.value;
                switch ((int)particleRaycastBudgetDropdown.value)
                {
                    case 0:
                        QualitySettings.particleRaycastBudget = 4;
                        break;
                    case 1:
                        QualitySettings.particleRaycastBudget = 16;
                        break;
                    case 2:
                        QualitySettings.particleRaycastBudget = 64;
                        break;
                    case 3:
                        QualitySettings.particleRaycastBudget = 256;
                        break;
                    case 4:
                        QualitySettings.particleRaycastBudget = 1024;
                        break;
                    case 5:
                        QualitySettings.particleRaycastBudget = 4096;
                        break;
                }
            }
         //   updParam();
        }

        public void ChangeTexture()
        {
            if (startTimer > Time.time)
                return;
            if (setQualityTime < Time.time)
            {
                qualitySelect.value = QualitySettings.names.Length;
                AtavismSettings.Instance.GetVideoSettings().masterTextureLimit = (int)masterTextureLimitDropdown.value;
                QualitySettings.masterTextureLimit = 3 - (int)masterTextureLimitDropdown.value;
            }
          //  updParam();
        }
        public void ChangeSoftParticels()
        {
            if (startTimer > Time.time)
                return;
            if (setQualityTime < Time.time)
            {
                qualitySelect.value = QualitySettings.names.Length;
                AtavismSettings.Instance.GetVideoSettings().softParticles = softParticlesToggle.isOn;
                QualitySettings.softParticles = softParticlesToggle.isOn;
            }
           // updParam();
        }

        public void ChangeEffects()
        {
            if (startTimer > Time.time)
                return;
            if (depthOfFieldToggle)
                AtavismSettings.Instance.GetVideoSettings().depthOfField = depthOfFieldToggle.isOn;
            if (bloomToggle)
                AtavismSettings.Instance.GetVideoSettings().bloom = bloomToggle.isOn;
            if (vignetteToggle)
                AtavismSettings.Instance.GetVideoSettings().vignette = vignetteToggle.isOn;
            //    AtavismSettings.Instance.GetVideoSettings().amplifyOcclusionEffect = amplifyOcclusionEffectToggle.isOn;
            if (screenSpaceToggle)
                AtavismSettings.Instance.GetVideoSettings().seScreenSpaceShadows = screenSpaceToggle.isOn;
            if (volumetricFogToggle)
                AtavismSettings.Instance.GetVideoSettings().volumetricFog = volumetricFogToggle.isOn;
            if (volumetricCameraToggle)
                AtavismSettings.Instance.GetVideoSettings().hxVolumetricCamera = volumetricCameraToggle.isOn;
            if (motionBlurToggle)
                AtavismSettings.Instance.GetVideoSettings().motionBlur = motionBlurToggle.isOn;
            if (chromaticAberrationToggle)
                AtavismSettings.Instance.GetVideoSettings().chromaticAberration = chromaticAberrationToggle.isOn;

            if (ambiertOcclusionsToggle)
                AtavismSettings.Instance.GetVideoSettings().ambientOcclusion = ambiertOcclusionsToggle.isOn;
            if (softParticlesToggle)
                AtavismSettings.Instance.GetVideoSettings().softParticles = softParticlesToggle.isOn;
            if (autoExposureToggle)
                AtavismSettings.Instance.GetVideoSettings().autoExposure = autoExposureToggle.isOn;
            if (colorGradingToggle)
                AtavismSettings.Instance.GetVideoSettings().colorGrading = colorGradingToggle.isOn;
            if (ditheringToggle)
                AtavismSettings.Instance.GetVideoSettings().dithering = ditheringToggle.isOn;
            if (screenSpaceReflectionsToggle)
                AtavismSettings.Instance.GetVideoSettings().screenSpaceReflections = screenSpaceReflectionsToggle.isOn;

            if (antialiasingDropdown)
                AtavismSettings.Instance.GetVideoSettings().antialiasing = antialiasingDropdown.value;
            AtavismSettings.Instance.ApplyCamEffect();

        }


        public void ChangeResolution()
        {
            if (startTimer > Time.time)
                return;
            if (resolutions != null && resolutions.Count >= resolutionSelect.value)
            {
                Screen.SetResolution(resolutions[resolutionSelect.value].width, resolutions[resolutionSelect.value].height, fullscreenToggle.isOn);
            }
          //  updParam();
        }

        public void ChangeRenderingSampling()
        {
            if (startTimer > Time.time)
                return;

        }

        // Use this for initialization
        void Start()
        {
            updateResolutions();
            updParam();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}