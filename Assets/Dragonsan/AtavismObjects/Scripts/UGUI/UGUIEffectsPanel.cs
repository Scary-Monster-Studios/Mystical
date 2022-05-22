using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{

    public class UGUIEffectsPanel : MonoBehaviour
    {

        public List<UGUIEffect> effectButtons;
        [SerializeField] bool passiveEffect = false;

        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("EFFECT_UPDATE", this);
            AtavismEventSystem.RegisterEvent("EFFECT_ICON_UPDATE", this);
            UpdateEffectButtons();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("EFFECT_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("EFFECT_ICON_UPDATE", this);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "EFFECT_UPDATE" || eData.eventType == "EFFECT_ICON_UPDATE")
            {
                // Update 
                UpdateEffectButtons();
            }
        }

        public void UpdateEffectButtons()
        {
            for (int i = 0; i < effectButtons.Count; i++)
            {
                if (i < Abilities.Instance.PlayerEffects.Count)
                {
                    if (!passiveEffect && Abilities.Instance.PlayerEffects[i].Active == false)
                    {
                        effectButtons[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        effectButtons[i].gameObject.SetActive(true);
                        effectButtons[i].SetEffect(Abilities.Instance.PlayerEffects[i], i);
                    }
                }
                else
                {
                    effectButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }
}