using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atavism
{


    public class UGUILoadingPrefabData : MonoBehaviour
    {
        [SerializeField] private RectTransform background;
        [SerializeField] private Slider progressBar;
        
        void Start()
        {
            if(background && background.gameObject.activeSelf)
                background.gameObject.SetActive(false);
            AtavismEventSystem.RegisterEvent("LOADING_PREFAB_UPDATE", this);
            AtavismEventSystem.RegisterEvent("LOADING_PREFAB_SHOW", this);
            AtavismEventSystem.RegisterEvent("LOADING_PREFAB_HIDE", this);

        }

        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("LOADING_PREFAB_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("LOADING_PREFAB_SHOW", this);
            AtavismEventSystem.UnregisterEvent("LOADING_PREFAB_HIDE", this);

        }

        public void OnEvent(AtavismEventData eData)
        {
         //   Debug.LogError("AtavismPrefabManager: " + eData.eventType);
            if (eData.eventType == "LOADING_PREFAB_UPDATE")
            {
                int value = int.Parse(eData.eventArgs[0]);
                int max = int.Parse(eData.eventArgs[1]);
                if (progressBar)
                {
                    progressBar.maxValue = max;
                    progressBar.value = value;
                }
            }
            else if (eData.eventType == "LOADING_PREFAB_SHOW")
            {
                if (background && !background.gameObject.activeSelf)
                    background.gameObject.SetActive(true);
            }
            else if (eData.eventType == "LOADING_PREFAB_HIDE")
            {
                if (background && background.gameObject.activeSelf)
                    background.gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}