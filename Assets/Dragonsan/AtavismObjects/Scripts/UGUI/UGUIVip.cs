using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Atavism
{
    public class UGUIVip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        public Sprite[] backgrounds;
        public Image background;
        public TextMeshProUGUI level;
        public bool showZeroLevel;
        bool mouseEntered = false;
        bool started = false;
        // Start is called before the first frame update
        void Start()
        {
            AtavismEventSystem.RegisterEvent("VIP_UPDATE", this);
            AtavismVip.Instance.GetVipStatus();
            UpdateDisplay();
        }
        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("VIP_UPDATE", this);
        }
        // Update is called once per frame
        void Update()
        {
            UpdateDisplay();
        }

        void UpdateDisplay()
        {
            int lev = AtavismVip.Instance.GetLevel;
            if (level)
            {
                if (started)
                {
                    if (!level.text.Equals(lev.ToString()))
                    {
                        string[] event_args = new string[1];
                        AtavismEventSystem.DispatchEvent("VIP_LEVELUP", event_args);
                    }
                }
                level.text = lev.ToString();
                if (background)
                {
                    if (backgrounds.Length > lev)
                        background.sprite = backgrounds[lev];
                    if (lev == 0 && !showZeroLevel)
                    {
                        level.enabled = false;
                        background.enabled = false;
                    }
                    else
                    {
                        level.enabled = true;
                        background.enabled = true;
                    }
                }
                started = true;
            }
            if (AtavismVip.Instance.GetTime != 0)
            {

                float timeLeft = AtavismVip.Instance.GetTimeElapsed - Time.time;
                if (background)
                {
                    if (timeLeft > 0)
                        background.color = Color.white;
                    else
                        background.color = Color.gray;

                }
            }
        }
        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "VIP_UPDATE")
            {
                UpdateDisplay();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseEntered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseEntered = false;
        }
        void ShowTooltip()
        {
#if AT_I2LOC_PRESET
            UGUITooltip.Instance.SetTitle(I2.Loc.LocalizationManager.GetTranslation(AtavismVip.Instance.GetName));
#else
            UGUITooltip.Instance.SetTitle(AtavismVip.Instance.GetName);
#endif
            UGUITooltip.Instance.HideType(true);
            UGUITooltip.Instance.EnableIcon(false);
            if(AtavismVip.Instance.GetMaxPoints > 0)
#if AT_I2LOC_PRESET
               UGUITooltip.Instance.AddAttribute(I2.Loc.LocalizationManager.GetTranslation("Points"),AtavismVip.Instance.GetPoints + " / " + AtavismVip.Instance.GetMaxPoints, true);
#else
                UGUITooltip.Instance.AddAttribute("Points ",AtavismVip.Instance.GetPoints + " / " + AtavismVip.Instance.GetMaxPoints, true);
#endif
            foreach (Bonus b in AtavismVip.Instance.GetBonuses)
            {
                string bonusName = b.name;
#if AT_I2LOC_PRESET
                bonusName = I2.Loc.LocalizationManager.GetTranslation(bonusName);
#endif
                if (b.value != 0)
                    UGUITooltip.Instance.AddAttribute(bonusName, b.value.ToString(), false);
                if (b.percentage != 0)
                    UGUITooltip.Instance.AddAttribute(bonusName, b.percentage + " %", false);
            }
          //  UGUITooltip.Instance.AddAttribute(AtavismVip.Instance.GetPoints+" / "+ AtavismVip.Instance.GetMaxPoints , "", true);
            if (AtavismVip.Instance.GetTime != 0){

                float timeLeft = AtavismVip.Instance.GetTimeElapsed - Time.time;
                int days = 0;
                int hours = 0;
                int minutes = 0;
                if (timeLeft > 86400)
                {
                    days = (int)timeLeft / 86400;
                    timeLeft -= days * 86400;
                }
                if (timeLeft > 3600)
                {
                    hours = (int)timeLeft / 3600;
                    timeLeft -= hours * 3600;
                }
                if (timeLeft > 60)
                {
                    minutes = (int)timeLeft / 60;
                    timeLeft = minutes * 60;
                }
                if (days > 0)
                {
#if AT_I2LOC_PRESET
                       UGUITooltip.Instance.AddAttribute(I2.Loc.LocalizationManager.GetTranslation("Expires in :") + days + " "+I2.Loc.LocalizationManager.GetTranslation("days"), "", true);
#else
                    UGUITooltip.Instance.AddAttribute("Expires in :" + days + " days", "", true);
#endif
                }
                else if (hours > 0)
                {
#if AT_I2LOC_PRESET
                       UGUITooltip.Instance.AddAttribute(I2.Loc.LocalizationManager.GetTranslation("Expires in :") + hours + " "+I2.Loc.LocalizationManager.GetTranslation("hours"), "", true);
#else
                    UGUITooltip.Instance.AddAttribute("Expires in :" + hours + " hour", "", true);
#endif
                }
                else if (minutes > 0 && timeLeft > 0)
                {
#if AT_I2LOC_PRESET
                       UGUITooltip.Instance.AddAttribute(I2.Loc.LocalizationManager.GetTranslation("Expires in :") + minutes + " "+I2.Loc.LocalizationManager.GetTranslation("minutes"), "", true);
#else
                    UGUITooltip.Instance.AddAttribute("Expires in :" + minutes + " minutes", "", true);
#endif
                }
                else
                {
#if AT_I2LOC_PRESET
                       UGUITooltip.Instance.AddAttribute(I2.Loc.LocalizationManager.GetTranslation("Expired"), "", true);
#else
                    UGUITooltip.Instance.AddAttribute("Expired", "", true,Color.red);
#endif

                }
            }
#if AT_I2LOC_PRESET
               UGUITooltip.Instance.SetDescription(I2.Loc.LocalizationManager.GetTranslation(AtavismVip.Instance.GetDescription));
#else
            UGUITooltip.Instance.SetDescription( AtavismVip.Instance.GetDescription );
#endif

            UGUITooltip.Instance.Show(gameObject);
        }
        void HideTooltip()
        {
            UGUITooltip.Instance.Hide();
        }
        public bool MouseEntered
        {
            get
            {
                return mouseEntered;
            }
            set
            {
                mouseEntered = value;
                if (mouseEntered )
                {
                    ShowTooltip();
                }
                else
                {
                    HideTooltip();
                }
            }
        }

    }
}