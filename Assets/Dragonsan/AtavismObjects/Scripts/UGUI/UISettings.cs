using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

namespace Atavism
{

    public class UISettings : MonoBehaviour
    {
        [SerializeField] Text buttonSoundsText;
        [SerializeField] TextMeshProUGUI TMPButtonSoundsText;
        [SerializeField] Image buttonSoundsImage;
        [SerializeField] Text buttonGraphicsText;
        [SerializeField] TextMeshProUGUI TMPButtonGraphicsText;
        [SerializeField] Image buttonGraphicsImage;
        [SerializeField] Text buttonGeneralText;
        [SerializeField] TextMeshProUGUI TMPButtonGeneralText;
        [SerializeField] Image buttonGeneralImage;
        [SerializeField] Text buttonControllText;
        [SerializeField] TextMeshProUGUI TMPButtonControllText;
        [SerializeField] Image buttonControllImage;
        [SerializeField] GameObject tabSounds;
        [SerializeField] GameObject tabGraphics;
        [SerializeField] GameObject tabGeneral;
        [SerializeField] GameObject tabControll;
        [SerializeField] Color selectedColor = new Color(0f, 1f, 0f, 1f);
        [SerializeField] Color normalColor = new Color(1f, 1f, 1f, 1f);
        [SerializeField] Color selectedTextColor = new Color(0f, 1f, 0f, 1f);
        [SerializeField] Color normalTextColor = new Color(1f, 1f, 1f, 1f);


        // Use this for initialization
        void Start()
        {
            ClickGeneral();
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void ClickSound()
        {
            if (tabControll != null)
                tabControll.SetActive(false);
            if (tabGeneral != null)
                tabGeneral.SetActive(false);
            if (tabGraphics != null)
                tabGraphics.SetActive(false);
            if (tabSounds != null)
                tabSounds.SetActive(true);
            if (buttonControllText != null)
                buttonControllText.color = normalTextColor;
            if (TMPButtonControllText != null)
                TMPButtonControllText.color = normalTextColor;
            if (buttonControllImage != null)
                buttonControllImage.color = normalColor;
            if (buttonGeneralText != null)
                buttonGeneralText.color = normalTextColor;
            if (TMPButtonGeneralText != null)
                TMPButtonGeneralText.color = normalTextColor;
            if (buttonGeneralImage != null)
                buttonGeneralImage.color = normalColor;
            if (buttonGraphicsText != null)
                buttonGraphicsText.color = normalTextColor;
            if (TMPButtonGraphicsText != null)
                TMPButtonGraphicsText.color = normalTextColor;
            if (buttonGraphicsImage != null)
                buttonGraphicsImage.color = normalColor;
            if (buttonSoundsText != null)
            {
                buttonSoundsText.color = selectedTextColor;
            }
            if (TMPButtonSoundsText != null)
            {
                TMPButtonSoundsText.color = selectedTextColor;
            }
            if (buttonSoundsImage != null)
                buttonSoundsImage.color = selectedColor;
        }
        public void ClickGeneral()
        {
            if (tabControll != null)
                tabControll.SetActive(false);
            if (tabGeneral != null)
            {
                tabGeneral.SetActive(true);
                tabGeneral.GetComponent<UIGeneralSettings>().updParam();
            }
            if (tabGraphics != null)
                tabGraphics.SetActive(false);
            if (tabSounds != null)
                tabSounds.SetActive(false);
            if (buttonControllText != null)
                buttonControllText.color = normalTextColor;
            if (TMPButtonControllText != null)
                TMPButtonControllText.color = normalTextColor;
            if (buttonControllImage != null)
                buttonControllImage.color = normalColor;
            if (buttonGeneralText != null)
                buttonGeneralText.color = selectedTextColor;
            if (TMPButtonGeneralText != null)
                TMPButtonGeneralText.color = selectedTextColor;
            if (buttonGeneralImage != null)
                buttonGeneralImage.color = selectedColor;
            if (buttonGraphicsText != null)
                buttonGraphicsText.color = normalTextColor;
            if (TMPButtonGraphicsText != null)
                TMPButtonGraphicsText.color = normalTextColor;
            if (buttonGraphicsImage != null)
                buttonGraphicsImage.color = normalColor;
            if (buttonSoundsText != null)
            {
                buttonSoundsText.color = normalTextColor;
            }
            if (TMPButtonSoundsText != null)
            {
                TMPButtonSoundsText.color = normalTextColor;
            }
            if (buttonSoundsImage != null)
                buttonSoundsImage.color = normalColor;
        }
        public void ClickGraphic()
        {
            if (tabControll != null)
                tabControll.SetActive(false);
            if (tabGeneral != null)
                tabGeneral.SetActive(false);
            if (tabGraphics != null)
                tabGraphics.SetActive(true);
            if (tabSounds != null)
                tabSounds.SetActive(false);
            if (buttonControllText != null)
                buttonControllText.color = normalTextColor;
            if (TMPButtonControllText != null)
                TMPButtonControllText.color = normalTextColor;
            if (buttonControllImage != null)
                buttonControllImage.color = normalColor;
            if (buttonGeneralText != null)
                buttonGeneralText.color = normalTextColor;
            if (TMPButtonGeneralText != null)
                TMPButtonGeneralText.color = normalTextColor;
            if (buttonGeneralImage != null)
                buttonGeneralImage.color = normalColor;
            if (buttonGraphicsText != null)
                buttonGraphicsText.color = selectedTextColor;
            if (TMPButtonGraphicsText != null)
                TMPButtonGraphicsText.color = selectedTextColor;
            if (buttonGraphicsImage != null)
                buttonGraphicsImage.color = selectedColor;
            if (buttonSoundsText != null)
            {
                buttonSoundsText.color = normalTextColor;
            }
            if (TMPButtonSoundsText != null)
            {
                TMPButtonSoundsText.color = normalTextColor;
            }
            if (buttonSoundsImage != null)
                buttonSoundsImage.color = normalColor;
        }
        public void ClickControll()
        {
            if (tabControll != null)
                tabControll.SetActive(true);
            if (tabGeneral != null)
                tabGeneral.SetActive(false);
            if (tabGraphics != null)
                tabGraphics.SetActive(false);
            if (tabSounds != null)
                tabSounds.SetActive(false);
            if (buttonControllText != null)
                buttonControllText.color = selectedTextColor;
            if (TMPButtonControllText != null)
                TMPButtonControllText.color = selectedTextColor;
            if (buttonControllImage != null)
                buttonControllImage.color = selectedColor;
            if (buttonGeneralText != null)
                buttonGeneralText.color = normalTextColor;
            if (TMPButtonGeneralText != null)
                TMPButtonGeneralText.color = normalTextColor;
            if (buttonGeneralImage != null)
                buttonGeneralImage.color = normalColor;
            if (buttonGraphicsText != null)
                buttonGraphicsText.color = normalTextColor;
            if (TMPButtonGraphicsText != null)
                TMPButtonGraphicsText.color = normalTextColor;
            if (buttonGraphicsImage != null)
                buttonGraphicsImage.color = normalColor;
            if (buttonSoundsText != null)
            {
                buttonSoundsText.color = normalTextColor;
            }
            if (TMPButtonSoundsText != null)
            {
                TMPButtonSoundsText.color = normalTextColor;
            }
            if (buttonSoundsImage != null)
                buttonSoundsImage.color = normalColor;
        }
    }
}