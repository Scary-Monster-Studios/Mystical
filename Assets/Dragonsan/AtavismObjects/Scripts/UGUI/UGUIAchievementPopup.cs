using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Atavism
{

    public class UGUIAchievementPopup : MonoBehaviour
    {
        // public Vector2 minSize = new Vector2(0, 0);
        // public Vector2 maxSize = new Vector2(300, 300);
        public float time = 1f;
        public Sprite background;
        public TextMeshProUGUI message;
        // Start is called before the first frame update
        void Start()
        {
            AtavismEventSystem.RegisterEvent("ACHIEV_UPDATE", this);
            if (background)
            {

             //   background.gameObject.setActive(false);
            }
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("ACHIEV_UPDATE", this);
        }
        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "ACHIEV_UPDATE")
            {

            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}