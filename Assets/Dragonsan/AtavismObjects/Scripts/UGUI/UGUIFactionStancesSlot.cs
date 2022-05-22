using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Atavism
{
    public class UGUIFactionStancesSlot : MonoBehaviour
    {

        [SerializeField] TextMeshProUGUI Name;
        [SerializeField] TextMeshProUGUI Stance;
        [SerializeField] TextMeshProUGUI StanceNext;
        [SerializeField] TextMeshProUGUI StancePrev;
        [SerializeField] Slider slider;

        public void UpdateDisplay(string name, int value)
        {
            if (Name != null)
#if AT_I2LOC_PRESET
            Name.text = I2.Loc.LocalizationManager.GetTranslation(name);
#else
                Name.text = name;
#endif

            if (slider != null)
            {
                slider.value = value;
                slider.targetGraphic.color = Color.red;
                if (value < -1500)
                {
                    slider.minValue = -3000;
                    slider.maxValue = -1500;
                    if (StanceNext != null)
#if AT_I2LOC_PRESET
                    StanceNext.text = I2.Loc.LocalizationManager.GetTranslation("Disliked");
#else
                        StanceNext.text = "Disliked";
#endif
                    if (StancePrev != null)
                        StancePrev.text = "";
                    if (Stance != null)
#if AT_I2LOC_PRESET
                    Stance.text = I2.Loc.LocalizationManager.GetTranslation("Hated");
#else
                        Stance.text = "Hated";
#endif
                }
                else if (value < 0)
                {
                    slider.minValue = -1500;
                    slider.maxValue = 0;

                    if (StanceNext != null)
#if AT_I2LOC_PRESET
                    StanceNext.text = I2.Loc.LocalizationManager.GetTranslation("Neutral");
#else
                        StanceNext.text = "Neutral";
#endif
                    if (StancePrev != null)
#if AT_I2LOC_PRESET
                    StancePrev.text = I2.Loc.LocalizationManager.GetTranslation("Hated");
#else
                        StancePrev.text = "Hated";
#endif
                    if (Stance != null)
#if AT_I2LOC_PRESET
                    Stance.text = I2.Loc.LocalizationManager.GetTranslation("Disliked");
#else
                        Stance.text = "Disliked";
#endif
                }
                else if (value < 500)
                {
                    slider.minValue = 0;
                    slider.maxValue = 500;
                    if (StanceNext != null)
#if AT_I2LOC_PRESET
                    StanceNext.text = I2.Loc.LocalizationManager.GetTranslation("Friendly");
#else
                        StanceNext.text = "Friendly";
#endif
                    if (StancePrev != null)
#if AT_I2LOC_PRESET
                    StancePrev.text = I2.Loc.LocalizationManager.GetTranslation("Disliked");
#else
                        StancePrev.text = "Disliked";
#endif
                    if (Stance != null)
#if AT_I2LOC_PRESET
                    Stance.text = I2.Loc.LocalizationManager.GetTranslation("Neutral");
#else
                        Stance.text = "Neutral";
#endif
                }
                else if (value < 1500)
                {
                    slider.minValue = 500;
                    slider.maxValue = 1500;
                    if (StanceNext != null)
#if AT_I2LOC_PRESET
                    StanceNext.text = I2.Loc.LocalizationManager.GetTranslation("Honoured");
#else
                        StanceNext.text = "Honoured";
#endif
                    if (StancePrev != null)
#if AT_I2LOC_PRESET
                    StancePrev.text = I2.Loc.LocalizationManager.GetTranslation("Neutral");
#else
                        StancePrev.text = "Neutral";
#endif
                    if (Stance != null)
#if AT_I2LOC_PRESET
                    Stance.text = I2.Loc.LocalizationManager.GetTranslation("Friendly");
#else
                        Stance.text = "Friendly";
#endif
                }
                else if (value < 3000)
                {
                    slider.minValue = 1500;
                    slider.maxValue = 3000;
                    if (StanceNext != null)
#if AT_I2LOC_PRESET
                    StanceNext.text = I2.Loc.LocalizationManager.GetTranslation("Exalted");
#else
                        StanceNext.text = "Exalted";
#endif
                    if (StancePrev != null)
#if AT_I2LOC_PRESET
                    StancePrev.text = I2.Loc.LocalizationManager.GetTranslation("Friendly");
#else
                        StancePrev.text = "Friendly";
#endif
                    if (Stance != null)
#if AT_I2LOC_PRESET
                    Stance.text = I2.Loc.LocalizationManager.GetTranslation("Honoured");
#else
                        Stance.text = "Honoured";
#endif
                }
                else
                {
                    slider.minValue = 3000;
                    slider.maxValue = 3100;
                    if (StanceNext != null)
                        StanceNext.text = "";
                    if (StancePrev != null)
#if AT_I2LOC_PRESET
                    StancePrev.text = I2.Loc.LocalizationManager.GetTranslation("Honoured");
#else
                        StancePrev.text = "Honoured";
#endif
                    if (Stance != null)
#if AT_I2LOC_PRESET
                    Stance.text = I2.Loc.LocalizationManager.GetTranslation("Exalted");
#else
                        Stance.text = "Exalted";
#endif
                }
                /*public static final int HatedRep = -3000;
                public static final int DislikedRep = -1500;
                public static final int NeutralRep = 0;
                public static final int FriendlyRep = 500;
                public static final int HonouredRep = 1500;
                public static final int ExaltedRep = 3000;
            */
            }

        }
    }
}