using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Atavism
{
    public class UGUIMiniTooltip : MonoBehaviour
    {

        static UGUIMiniTooltip instance;

        public Text description;
        public TextMeshProUGUI TMPDescription;
        CanvasGroup canvasGroup = null;
        GameObject target;
        bool showing = false;

        void Awake()
        {
            if (instance != null)
            {
                GameObject.DestroyImmediate(gameObject);
                return;
            }
            instance = this;
        }

        // Use this for initialization
        void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            Hide();
        }

        void OnDestroy()
        {
            instance = null;
        }

        void Update()
        {
            if (!showing)
                return;

            if (this.target == null || !this.target.activeInHierarchy || target.GetComponent<CanvasRenderer>().GetAlpha() == 0)
            {
                Hide();
                return;
            }

            Vector3 tooltipPosition = Input.mousePosition;


           bool left = false;
            bool top = false;
            if (tooltipPosition.x + GetComponent<RectTransform>().sizeDelta.x * GetComponent<RectTransform>().lossyScale.x + 50 > Screen.width)
            {
                tooltipPosition.x -= (GetComponent<RectTransform>().sizeDelta.x * GetComponent<RectTransform>().lossyScale.x + 5);
                left = true;
            }
            else
                tooltipPosition.x += 5;

            if (tooltipPosition.y + (GetComponent<RectTransform>().sizeDelta.y * GetComponent<RectTransform>().lossyScale.y + 5) > Screen.height)
            {
                tooltipPosition.y -= (GetComponent<RectTransform>().sizeDelta.y * GetComponent<RectTransform>().lossyScale.y + 5);
                top = true;
            }
            else
                tooltipPosition.y += 5;
            if (!left && top)
                tooltipPosition.x += 30;

            transform.position = tooltipPosition;
        }


        public void SetDescription(string descriptionText)
        {
            if (description != null)
            {
                description.text = descriptionText;
                if (string.IsNullOrEmpty(descriptionText))
                {
                    description.enabled = false;
                }
                else
                {
                    description.enabled = true;
                }
            }
            if (TMPDescription != null)
            {
                TMPDescription.text = descriptionText;
                if (string.IsNullOrEmpty(descriptionText))
                {
                    TMPDescription.enabled = false;
                }
                else
                {
                    TMPDescription.enabled = true;
                }
            }
        }

        private static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        public void Show(GameObject target)
        {
            canvasGroup.alpha = 1f;

            // Save the position where the tooltip should appear
            Vector3 tooltipPosition = Input.mousePosition;
            // Add a button width/height to the tooltip position
            tooltipPosition += new Vector3(5, 5, 0);
            // Check if the position will be out of the screen
            if (tooltipPosition.x + GetComponent<RectTransform>().sizeDelta.x > Screen.width)
            {
                tooltipPosition.x = Screen.width - GetComponent<RectTransform>().sizeDelta.x;
            }
            if (tooltipPosition.y + GetComponent<RectTransform>().sizeDelta.y > Screen.height)
            {
                tooltipPosition.y = Screen.height - GetComponent<RectTransform>().sizeDelta.y;
            }
            transform.position = tooltipPosition;

            if (description != null)
                description.transform.SetAsLastSibling();
            if (TMPDescription != null)
                TMPDescription.transform.SetAsLastSibling();

            AtavismUIUtility.BringToFront(this.gameObject);

            showing = true;
            this.target = target;
        }

        public void Hide()
        {
            canvasGroup.alpha = 0f;
            showing = false;
        }

        public static UGUIMiniTooltip Instance
        {
            get
            {
                return instance;
            }

        }
    }
}