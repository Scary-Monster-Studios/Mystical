using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace Atavism
{
    public class UGUIMiniTooltipEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string dectName = "";
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseEntered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseEntered = false;
        }
        void ShowMiniTooltip()
        {
#if AT_I2LOC_PRESET
        UGUIMiniTooltip.Instance.SetDescription(I2.Loc.LocalizationManager.GetTranslation(dectName));
#else
            UGUIMiniTooltip.Instance.SetDescription(dectName);
#endif
            UGUIMiniTooltip.Instance.Show(gameObject);
        }
        void HideMiniTooltip()
        {
            UGUIMiniTooltip.Instance.Hide();
        }
        public bool MouseEntered
        {
            set
            {
                if (value && dectName != "")
                {
                    ShowMiniTooltip();
                }
                else
                {
                    HideMiniTooltip();
                }
            }
        }
    }
}