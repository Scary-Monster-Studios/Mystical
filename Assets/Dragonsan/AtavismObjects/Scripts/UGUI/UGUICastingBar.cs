using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{
    public class UGUICastingBar : MonoBehaviour
    {

        static UGUICastingBar instance;

        public Image icon;
        public GameObject iconGameObject;
        public Text castName;
        public TextMeshProUGUI TMPCastName;
        public Text castTime;
        public TextMeshProUGUI TMPCastTime;
        public Image castFill;
        float startTime;
        float endTime = -1;

        // Use this for initialization
        void Start()
        {
            if (instance != null)
            {
                GameObject.DestroyImmediate(gameObject);
                return;
            }
            instance = this;

            Hide();
            AtavismEventSystem.RegisterEvent("CASTING_STARTED", this);
            AtavismEventSystem.RegisterEvent("CASTING_CANCELLED", this);
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("CASTING_STARTED", this);
            AtavismEventSystem.UnregisterEvent("CASTING_CANCELLED", this);
        }

        // Update is called once per frame
        void Update()
        {
            if (endTime != -1 && endTime > Time.time)
            {
                float total = endTime - startTime;
                float currentTime = endTime - Time.time;
                if (GetComponent<Slider>() != null)
                    GetComponent<Slider>().value = 1 - ((float)currentTime / (float)total);
                if (castFill != null)
                    castFill.fillAmount = /*1 - */((float)currentTime / (float)total);
                if (TMPCastTime != null)
                    TMPCastTime.text = string.Format("{0:0.0}", currentTime) + "s";
            }
            else
            {
                Hide();
            }
        }

        void Show()
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().ignoreParentGroups = true;
        }

        public void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            GetComponent<CanvasGroup>().ignoreParentGroups = false;
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "CASTING_STARTED")
            {
                if (eData.eventArgs.Length > 1 && OID.fromString(eData.eventArgs[1]).ToLong() != ClientAPI.GetPlayerOid())
                    return;
                Show();
                startTime = Time.time;
                endTime = Time.time + float.Parse(eData.eventArgs[0]);
                int abiltyId = 0;
                if(eData.eventArgs.Length>2)
                    abiltyId = int.Parse(eData.eventArgs[2]);
                if (iconGameObject != null)
                {
                    if (abiltyId > 0)
                    {
                        iconGameObject.SetActive(true);
                    }
                    else
                    {
                        iconGameObject.SetActive(false);
                    }
                }
                if (icon != null)
                {
                    if (abiltyId > 0)
                    {
                        AtavismAbility aa = Abilities.Instance.GetAbility(abiltyId);
                        icon.sprite = aa.icon;
                        icon.enabled = true;
                    }
                    else
                    {
                        icon.enabled = false;
                    }
                }

            }
            else if (eData.eventType == "CASTING_CANCELLED")
            {
                if (eData.eventArgs.Length > 1 && OID.fromString(eData.eventArgs[1]).ToLong() != ClientAPI.GetPlayerOid())
                    return;
                Hide();
                endTime = -1;
            }
        }

        public static UGUICastingBar Instance
        {
            get
            {
                return instance;
            }
        }
    }
}