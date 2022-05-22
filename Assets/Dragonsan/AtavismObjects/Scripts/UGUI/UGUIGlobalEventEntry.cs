using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Atavism
{


    public class UGUIGlobalEventEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private bool mouseEntered = false;

        public Image icon;
        private string name = "";
        private string description = "";
        private int id = -1;

        private void Start()
        {
            AtavismEventSystem.RegisterEvent("GLOABL_EVENTS_ICON", this);
        }

        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("GLOABL_EVENTS_ICON", this);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "GLOABL_EVENTS_ICON")
            {
                loadIcon();
            }
        }
        public void UpdateDisplay(GlobalEvent ge)
        {
            this.name = ge.name;
            this.description = ge.description;
            this.id = ge.id;
            loadIcon();
        }
       
      void loadIcon()
        {
            if (id > 0)
            {
                var ge = AtavismPrefabManager.Instance.LoadGlobaEventsData(id);
                if (ge != null)
                {
                    if (icon != null)
                    {
                        icon.sprite = ge.icon;
                    }
                }
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

        void HideTooltip()
        {
            UGUITooltip.Instance.Hide();

        }
        void ShowTooltip()
        {
#if AT_I2LOC_PRESET
        UGUITooltip.Instance.SetTitle(I2.Loc.LocalizationManager.GetTranslation(name));
        UGUITooltip.Instance.SetDescription(I2.Loc.LocalizationManager.GetTranslation(description));
#else
            UGUITooltip.Instance.SetTitle(name);
            UGUITooltip.Instance.SetDescription(description);
#endif
            //   UGUITooltip.Instance.SetQuality(1);
            UGUITooltip.Instance.SetQualityColor(AtavismSettings.Instance.effectQualityColor);

            UGUITooltip.Instance.SetType("");
            UGUITooltip.Instance.SetWeight("");
            if(icon!=null)
                UGUITooltip.Instance.SetIcon(icon.sprite);
            UGUITooltip.Instance.Show(gameObject);
        }
        
        public bool MouseEntered
        {
            get { return mouseEntered; }
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