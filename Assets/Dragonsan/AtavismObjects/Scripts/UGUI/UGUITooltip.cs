using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Atavism
{

    public class AttributeInfo
    {
        public string value;
        public string text;
        public Color textColour = UGUITooltip.Instance.defaultTextColour;
        public bool singleColumnRow;
        public RectOffset margin;
        public bool separator = false;
        public bool title = false;
        public bool socket = false;
        public bool resource = false;
        public Sprite socketIcon;


    }

    public class UGUITooltip : MonoBehaviour
    {

        static UGUITooltip instance;

        public Text title;
        public TextMeshProUGUI TMPTitle;
        public Text type;
        public TextMeshProUGUI TMPType;
        public Text weight;
        public TextMeshProUGUI TMPWeight;
        public Text description;
        public TextMeshProUGUI TMPDescription;
        public GameObject contentPanel;
        public Color attributeTextColour = Color.white;
        public GameObject attributeRow;
        public GameObject resourceRow;
        public GameObject attributeTitle;
        public GameObject socketRow;
        public GameObject separator;
        public int greaterSpriteId = 0;
        public int lowerSpriteId = 1;
        public int equalSpriteId = 2;
        public int newSpriteId = 3;
        //	public List<Color> itemGradeColors; 
        public Color itemTypeColour = Color.white;
        public Color itemStatColour = Color.green;
        public Color abilityRangeColour = Color.white;
        public Color abilityCostColour = Color.white;
        public Color abilityCastTimeColour = Color.white;
        public Color defaultTextColour = Color.black;
        public Color itemStatLowerColour = Color.red;
        //public Color itemStatHigherColour = Color.green;
        public Color itemSectionTitleColour = Color.yellow;
        public Color itemSetColour = Color.yellow;
        public Color itemInactiveSetColour = Color.gray;

        public Image overlayIcon;
        public Image itemIcon;
        public GameObject iconPanel;
        public Image anchorTR;
        public Image anchorTL;
        public Image anchorBR;
        public Image anchorBL;

        CanvasGroup canvasGroup = null;
        [SerializeField] Color defaultTitleColor = Color.white;
        List<AttributeInfo> attributes = new List<AttributeInfo>();
        List<GameObject> attributesRows = new List<GameObject>();
        GameObject target;
        bool showing = false;

        [AtavismSeparatorAttribute("Additional Tooltip 1")]
        [SerializeField] Transform additionalTooltip;
        [SerializeField] Text additionalTooltipTitle;
        [SerializeField] TextMeshProUGUI TMPAdditionalTooltipTitle;
        [SerializeField] Text additionalTooltipType;
        [SerializeField] TextMeshProUGUI TMPAdditionalTooltipType;
        [SerializeField] Text additionalTooltipWeight;
        [SerializeField] TextMeshProUGUI TMPAdditionalTooltipWeight;
        [SerializeField] Text additionalTooltipDescription;
        [SerializeField] TextMeshProUGUI TMPAdditionalTooltipDescription;
        [SerializeField] Image additionalTooltipOverlayIcon;
        [SerializeField] Image additionalTooltipItemIcon;
        [SerializeField] GameObject additionalTooltipItemIconPanel;
        [SerializeField] GameObject additionalTooltipContentPanel;
        List<AttributeInfo> additionalAttributes = new List<AttributeInfo>();
        List<GameObject> additionalAttributesRows = new List<GameObject>();
        [AtavismSeparatorAttribute("Additional Tooltip 2")]
        [SerializeField] Transform additionalTooltip2;
        [SerializeField] Text additionalTooltipTitle2;
        [SerializeField] TextMeshProUGUI TMPAdditionalTooltipTitle2;
        [SerializeField] Text additionalTooltipType2;
        [SerializeField] TextMeshProUGUI TMPAdditionalTooltipType2;
        [SerializeField] Text additionalTooltipWeight2;
        [SerializeField] TextMeshProUGUI TMPAdditionalTooltipWeight2;
        [SerializeField] Text additionalTooltipDescription2;
        [SerializeField] TextMeshProUGUI TMPAdditionalTooltipDescription2;
        [SerializeField] Image additionalTooltipOverlayIcon2;
        [SerializeField] Image additionalTooltipItemIcon2;
        [SerializeField] GameObject additionalTooltipItemIconPanel2;
        [SerializeField] GameObject additionalTooltipContentPanel2;
        List<AttributeInfo> additionalAttributes2 = new List<AttributeInfo>();
        List<GameObject> additionalAttributesRows2 = new List<GameObject>();

        /// <summary>
        /// 
        /// </summary>
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
            //	defaultTitleColor = title.color;
            Hide();
            additionalTooltip.gameObject.SetActive(false);
        }
        /// <summary>
        /// 
        /// </summary>
        void OnDestroy()
        {
            instance = null;
        }
        /// <summary>
        /// 
        /// </summary>
        void Update()
        {
            if (!showing)
                return;

            if (this.target == null || !this.target.activeInHierarchy || target.GetComponent<CanvasRenderer>().GetAlpha() == 0)
            {
                Hide();
                return;
            }

          
            // Save the position where the tooltip should appear
            Vector3 tooltipPosition = Input.mousePosition;
            // Add a button width/height to the tooltip position
            tooltipPosition += new Vector3(5, 5, 0);
            // Check if the position will be out of the screen
            if (anchorBL != null)
                anchorBL.enabled = false;
            if (anchorTL != null)
                anchorTL.enabled = false;
            if (anchorBR != null)
                anchorBR.enabled = false;
            if (anchorTR != null)
                anchorTR.enabled = false;
            bool left = false;
            bool top = false;

          
            if (tooltipPosition.x + (GetComponent<RectTransform>().sizeDelta.x + 5) * GetComponent<RectTransform>().lossyScale.x > Screen.width)
            {
                if (tooltipPosition.x - (GetComponent<RectTransform>().sizeDelta.x + 5) * GetComponent<RectTransform>().lossyScale.x >= 0)
                    tooltipPosition.x -= (GetComponent<RectTransform>().sizeDelta.x + 5) * GetComponent<RectTransform>().lossyScale.x;
                else
                    tooltipPosition.x = 1;

                left = true;
            }
            else
                tooltipPosition.x += 5;

            if (tooltipPosition.y + (GetComponent<RectTransform>().sizeDelta.y + 5) * GetComponent<RectTransform>().lossyScale.y > Screen.height)
            {
                if (tooltipPosition.y - (GetComponent<RectTransform>().sizeDelta.y + 5) * GetComponent<RectTransform>().lossyScale.y >= 0)
                    tooltipPosition.y -= (GetComponent<RectTransform>().sizeDelta.y + 5) * GetComponent<RectTransform>().lossyScale.y;
                else
                    tooltipPosition.y = 0;
                top = true;
            }
            else
                tooltipPosition.y += 5;

            transform.position = tooltipPosition;
            if (!top && left)
                if (anchorBR != null)
                    anchorBR.enabled = true;
            if (top && !left)
                if (anchorTL != null)
                    anchorTL.enabled = true;
            if (!top && !left)
                if (anchorBL != null)
                    anchorBL.enabled = true;
            if (top && left)
                if (anchorTR != null)
                    anchorTR.enabled = true;
            if (additionalTooltip.gameObject.activeSelf)
                if (left)
                {
                    additionalTooltip.SetSiblingIndex(0);
                    additionalTooltip2.SetSiblingIndex(1);
                }
                else
                {
                    additionalTooltip2.SetSiblingIndex(2);
                    additionalTooltip.SetSiblingIndex(1);
                }
        }

        /// <summary>
        /// Set Title 
        /// </summary>
        /// <param name="titleText"></param>
        public void SetTitle(string titleText)
        {
            if (title != null)
            {
                title.text = titleText;
                title.color = defaultTitleColor;
            }
            if (TMPTitle != null)
            {
                TMPTitle.text = titleText;
                TMPTitle.color = defaultTitleColor;
            }
        }

        /// <summary>
        ///  Set title color
        /// </summary>
        /// <param name="color"></param>
        public void SetTitleColour(Color color)
        {
            if (title != null)
                title.color = color;
            if (TMPTitle != null)
                TMPTitle.color = color;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeText"></param>
        public void SetType(string typeText)
        {
            if (type != null)
                type.text = typeText;
            if (TMPType != null)
                TMPType.text = typeText;
        }

        public void HideType(bool b)
        {
            if (type != null)
                type.gameObject.SetActive(!b);
            if (TMPType != null)
                TMPType.gameObject.SetActive(!b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        public void SetTypeColour(Color color)
        {
            if (type != null)
                type.color = color;
            if (TMPType != null)
                TMPType.color = color;
        }

        /// <summary>
        /// Set weight
        /// </summary>
        /// <param name="weightText"></param>
        public void SetWeight(string weightText)
        {
            if (weight != null)
            {
                weight.text = weightText;
            }
            if (TMPWeight != null)
            {
                TMPWeight.text = weightText;
            }
        }

        public void HideWeight(bool b)
        {
            if (weight != null)
                weight.gameObject.SetActive(!b);
            if (TMPWeight != null)
                TMPWeight.gameObject.SetActive(!b);
        }
        /// <summary>
        /// Set description
        /// </summary>
        /// <param name="descriptionText"></param>
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

        /// <summary>
        /// Set icon
        /// </summary>
        /// <param name="icon"></param>
        public void SetIcon(Sprite icon)
        {
            if (icon != null)
            {
                EnableIcon(true);
                itemIcon.sprite = icon;
            }
            else
            {
                EnableIcon(false);
            }
        }

        /// <summary>
        /// Function enable/disable icon panel
        /// </summary>
        /// <param name="i"></param>
        public void EnableIcon(bool i)
        {
            if (iconPanel != null)
                iconPanel.SetActive(i);
        }

        /// <summary>
        /// Function to set quality color icon overlay and title
        /// </summary>
        /// <param name="quality"></param>
        public void SetQuality(int quality)
        {
            overlayIcon.color = AtavismSettings.Instance.ItemQualityColor(quality);
            if (title != null)
                title.color = AtavismSettings.Instance.ItemQualityColor(quality);
            if (TMPTitle != null)
                TMPTitle.color = AtavismSettings.Instance.ItemQualityColor(quality);
        }

        /// <summary>
        /// Function to set quality color icon overlay and title
        /// </summary>
        /// <param name="quality"></param>
        public void SetQualityColor(Color quality)        {
            overlayIcon.color = quality;
            if (title != null)
                title.color = quality;
            if (TMPTitle != null)
                TMPTitle.color = quality;
        }

        /// <summary>
        /// Set Attribute Row of tooltip
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        public void AddAttributeTitle(string text)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.text = text;
            info.title = true;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.attributes.Add(info);
        }
        /// <summary>
        /// Set Attribute Row of tooltip
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        public void AddAttributeTitle(string text, Color colour)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.text = text;
            info.textColour = colour;
            info.title = true;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.attributes.Add(info);
        }


        /// <summary>
        /// Set Attribute Row of tooltip
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        public void AddAttributeSeperator()
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.separator = true;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.attributes.Add(info);
        }


        /// <summary>
        /// Set Attribute Row of tooltip
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        public void AddAttributeSocket(string text, Sprite socketIcon, bool singleColumn)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.text = text;
            info.singleColumnRow = singleColumn;
            info.socket = true;
            info.socketIcon = socketIcon;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.attributes.Add(info);
        }
        /// <summary>
        /// Set Attribute Row of tooltip
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        public void AddAttributeResource(string text, string value, Sprite socketIcon, bool singleColumn)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.text = text;
            info.value = value;
            info.singleColumnRow = singleColumn;
            info.resource = true;
            info.socketIcon = socketIcon;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.attributes.Add(info);
        }

        /// <summary>
        /// Set Attribute Row of tooltip
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        public void AddAttribute(string text, string value, bool singleColumn)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.value = value;
            info.text = text;
            info.singleColumnRow = singleColumn;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.attributes.Add(info);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        /// <param name="colour"></param>
        public void AddAttribute(string text, string value, bool singleColumn, Color colour)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            //string colourText = string.Format("#{0:X2}{1:X2}{2:X2}ff", ToByte(colour.r), ToByte(colour.g), ToByte(colour.b));
            info.value = /*"<color=" + colourText + ">" + */value;
            info.text = text /*+ "</color>"*/;
            info.textColour = colour;
            info.singleColumnRow = singleColumn;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.attributes.Add(info);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void Show(GameObject target)
        {
            AtavismUIUtility.BringToFront(gameObject);
            canvasGroup.alpha = 1f;

            // Cleanup any attributes left, if any at all
            Cleanup();

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

            // Prepare the attributes
            if (attributes.Count > 0 && attributeRow != null)
            {
                bool isLeft = true;
                UGUITooltipAttributeRow lastRow = null;
                UGUITooltipSocketRow lastSocketRow = null;
                UGUITooltipResourceRow lastResourceRow = null;
                int lastDepth = 0;
                if (title != null)
                    lastDepth = title.depth;
                if (TMPTitle != null)
                    lastDepth = TMPTitle.depth;

                // Loop the attributes
                foreach (AttributeInfo info in attributes)
                {

                    if (info.separator)
                    {
                        GameObject obj = (GameObject)Instantiate(this.separator, contentPanel.transform, false);
                        // Fix position and scale
                        obj.transform.localScale = Vector3.one;
                        obj.transform.localPosition = Vector3.zero;
                        obj.transform.localRotation = Quaternion.identity;

                        // Increase the depth
                        lastDepth = lastDepth + 1;
                        isLeft = true;
                        attributesRows.Add(obj);
                    }
                    else if (info.title)
                    {
                        GameObject obj = (GameObject)Instantiate(this.attributeTitle, contentPanel.transform, false);
                        // Fix position and scale
                        obj.transform.localScale = Vector3.one;
                        obj.transform.localPosition = Vector3.zero;
                        obj.transform.localRotation = Quaternion.identity;
                        TextMeshProUGUI t = obj.GetComponent<TextMeshProUGUI>();
                        if (t != null)
                        {
                            t.text = info.text;
                            t.color = info.textColour;
                        }

                        // Increase the depth
                        lastDepth = lastDepth + 1;
                        isLeft = true;
                        attributesRows.Add(obj);
                    }
                    else
                    {

                        // Force left column in case it's a single column row
                        if (info.singleColumnRow)
                            isLeft = true;
                        if (isLeft)
                        {
                            // Instantiate a prefab

                            GameObject obj;
                            if (info.socket)
                            {
                                obj = (GameObject)Instantiate(this.socketRow, contentPanel.transform, false);
                            }
                            else if (info.resource)
                            {
                                obj = (GameObject)Instantiate(this.resourceRow, contentPanel.transform, false);
                            }
                            else
                            {
                                obj = (GameObject)Instantiate(this.attributeRow, contentPanel.transform, false);
                            }
                            // Apply parent
                            //	obj.transform.SetParent(transform, false);

                            // Fix position and scale
                            obj.transform.localScale = Vector3.one;
                            obj.transform.localPosition = Vector3.zero;
                            obj.transform.localRotation = Quaternion.identity;

                            // Increase the depth
                            lastDepth = lastDepth + 1;

                            // Get the attribute row script referrence
                            lastRow = obj.GetComponent<UGUITooltipAttributeRow>();
                            lastSocketRow = obj.GetComponent<UGUITooltipSocketRow>();
                            lastResourceRow = obj.GetComponent<UGUITooltipResourceRow>();


                            // Make some changes if it's a single column row
                            if (info.singleColumnRow)
                            {
                                // Destroy the right column
                                /*   if (lastRow.rightText != null)
                                       Destroy(lastRow.rightText.gameObject);
                                   if (lastRow.rightTextValue != null)
                                       Destroy(lastRow.rightTextValue.gameObject);
                                   if (lastRow.TMPRightText != null)
                                       Destroy(lastRow.TMPRightText.gameObject);
                                   if (lastRow.TMPRightTextValue != null)
                                       Destroy(lastRow.TMPRightTextValue.gameObject);*/
                            }

                            // Add it to the instanced objects list
                            attributesRows.Add(obj);
                        }

                        // Check if we have a row object to work with
                        if (lastRow != null)
                        {
                            Text text = null;
                            Text textValue = null;
                            TextMeshProUGUI TMPtext = null;
                            TextMeshProUGUI TMPtextValue = null;
                            if (isLeft)
                            {
                                if (lastRow.leftText != null)
                                    text = lastRow.leftText;
                                if (lastRow.leftTextValue != null)
                                    textValue = lastRow.leftTextValue;
                                if (lastRow.TMPLeftText != null)
                                    TMPtext = lastRow.TMPLeftText;
                                if (lastRow.TMPLeftTextValue != null)
                                    TMPtextValue = lastRow.TMPLeftTextValue;
                            }
                            else
                            {
                                if (lastRow.rightText != null)
                                    text = lastRow.rightText;
                                if (lastRow.rightTextValue != null)
                                    textValue = lastRow.rightTextValue;
                                if (lastRow.TMPRightText != null)
                                    TMPtext = lastRow.TMPRightText;
                                if (lastRow.TMPRightTextValue != null)
                                    TMPtextValue = lastRow.TMPRightTextValue;
                            }

                            // Check if we have the label
                            if (text != null)
                            {
                                if (textValue == null)
                                {
                                    // Set the label text
                                    text.text = info.value + info.text;
                                }
                                else
                                {
                                    text.text = info.text;
                                    textValue.text = info.value;
                                }
                                // Flip is left
                                if (!info.singleColumnRow)
                                    isLeft = !isLeft;
                            }
                            if (TMPtext != null)
                            {
                                if (TMPtextValue == null)
                                {
                                    // Set the label text
                                    TMPtext.text = info.value + info.text;
                                    TMPtext.color = info.textColour;
                                }
                                else
                                {
                                    TMPtext.text = info.text;
                                    TMPtextValue.text = info.value;
                                    TMPtext.color = info.textColour;
                                    TMPtextValue.color = info.textColour;
                                }
                                // Flip is left
                                if (!info.singleColumnRow)
                                    isLeft = !isLeft;
                            }
                        }
                        //Sockets
                        // Check if we have a row object to work with
                        if (lastSocketRow != null)
                        {
                            TextMeshProUGUI textRow = null;
                            Image iconRow = null;
                            if (isLeft)
                            {
                                if (lastSocketRow.TMPLeftText != null)
                                    textRow = lastSocketRow.TMPLeftText;
                                if (lastSocketRow.leftIcon != null)
                                    iconRow = lastSocketRow.leftIcon;
                            }
                            else
                            {
                                if (lastSocketRow.TMPRightText != null)
                                    textRow = lastSocketRow.TMPRightText;
                                if (lastSocketRow.rightIcon != null)
                                    iconRow = lastSocketRow.rightIcon;
                            }

                            // Check if we have the label
                            if (textRow != null)
                            {
                                // Set the label text
                                textRow.text = info.text;
                                textRow.color = info.textColour;
                                iconRow.enabled = true;
                                if (info.socketIcon != null)
                                    iconRow.sprite = info.socketIcon;
                                //  if (info.socketIcon == null)
                                //      iconRow.enabled = false;
                                // Flip is left
                                if (!info.singleColumnRow)
                                    isLeft = !isLeft;
                            }
                            else
                                Debug.LogWarning("Tooltip Socket row text is null isLeft:" + isLeft);


                        }
                        //Resource
                        // Check if we have a row object to work with
                        if (lastResourceRow != null)
                        {
                            TextMeshProUGUI textRow = null;
                            TextMeshProUGUI valueRow = null;
                            Image iconRow = null;
                            if (isLeft)
                            {
                                if (lastResourceRow.TMPLeftText != null)
                                    textRow = lastResourceRow.TMPLeftText;
                                if (lastResourceRow.TMPLeftTextValue != null)
                                    valueRow = lastResourceRow.TMPLeftTextValue;
                                if (lastResourceRow.leftIcon != null)
                                    iconRow = lastResourceRow.leftIcon;
                            }
                            else
                            {
                                if (lastResourceRow.TMPRightText != null)
                                    textRow = lastResourceRow.TMPRightText;
                                if (lastResourceRow.TMPRightTextValue != null)
                                    valueRow = lastResourceRow.TMPRightTextValue;
                                if (lastResourceRow.rightIcon != null)
                                    iconRow = lastResourceRow.rightIcon;
                            }

                            // Check if we have the label
                            if (textRow != null)
                            {
                                // Set the label text
                                textRow.text = info.text;
                                    textRow.color = info.textColour;
                                if (valueRow != null)
                                {
                                    valueRow.color = info.textColour;
                                    valueRow.text = info.value;
                                }
                                else
                                    Debug.LogWarning("Tooltip Resource row value text is null isLeft:" + isLeft);

                                iconRow.enabled = true;
                                if (info.socketIcon != null)
                                    iconRow.sprite = info.socketIcon;
                                //  if (info.socketIcon == null)
                                //      iconRow.enabled = false;
                                // Flip is left
                                if (!info.singleColumnRow)
                                    isLeft = !isLeft;
                            }
                            else
                                Debug.LogWarning("Tooltip Resource row text is null isLeft:" + isLeft);

                        }

                    }
                }

                // Clear the attributes list, we no longer need it
                attributes.Clear();
            }
            if (description != null)
                description.transform.SetAsLastSibling();
            if (TMPDescription != null)
                TMPDescription.transform.SetAsLastSibling();

            showing = true;
            this.target = target;
        }

        /// <summary>
        /// Function hide tooltip
        /// </summary>
        public void Hide()
        {
            canvasGroup.alpha = 0f;
            showing = false;
            additionalTooltip.gameObject.SetActive(false);
            additionalTooltip2.gameObject.SetActive(false);

        }

        /// <summary>
        /// 
        /// </summary>
        private void Cleanup()
        {
            //Destroy the attributes
            foreach (GameObject obj in attributesRows)
            {
                if (obj != null)
                    DestroyImmediate(obj);
            }

            // Clear the list
            attributesRows.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        public static UGUITooltip Instance
        {
            get
            {
                return instance;
            }

        }
        #region Additional Tooltip Functions

        public void SetAdditionalTitle(string titleText)
        {
            if (additionalTooltipTitle != null)
            {
                additionalTooltipTitle.text = titleText;
                additionalTooltipTitle.color = defaultTitleColor;
            }
            if (TMPAdditionalTooltipTitle != null)
            {
                TMPAdditionalTooltipTitle.text = titleText;
                TMPAdditionalTooltipTitle.color = defaultTitleColor;
            }
            HideAdditionalTitle(false);
        }

        public void HideAdditionalTitle(bool b)
        {
            if (additionalTooltipTitle != null)
                additionalTooltipTitle.gameObject.SetActive(!b);
            if (TMPAdditionalTooltipTitle != null)
                TMPAdditionalTooltipTitle.gameObject.SetActive(!b);

        }

        public void SetAdditionalTitleColour(Color color)
        {
            if (additionalTooltipTitle != null)
                additionalTooltipTitle.color = color;
            if (TMPAdditionalTooltipTitle != null)
                TMPAdditionalTooltipTitle.color = color;
        }

        public void SetAdditionalType(string typeText)
        {
            if (additionalTooltipType != null)
                additionalTooltipType.text = typeText;
            if (TMPAdditionalTooltipType != null)
                TMPAdditionalTooltipType.text = typeText;
            HideAdditionalType(false);
        }

        public void HideAdditionalType(bool b)
        {
            if (additionalTooltipType != null)
                additionalTooltipType.gameObject.SetActive(!b);
            if (TMPAdditionalTooltipType != null)
                TMPAdditionalTooltipType.gameObject.SetActive(!b);
        }

        public void SetAdditionalTypeColour(Color color)
        {
            if (additionalTooltipType != null)
                additionalTooltipType.color = color;
            if (TMPAdditionalTooltipType != null)
                TMPAdditionalTooltipType.color = color;
        }

        public void SetAdditionalWeight(string weightText)
        {
            if (additionalTooltipWeight != null)
            {
                additionalTooltipWeight.text = weightText;
            }
            if (TMPAdditionalTooltipWeight != null)
            {
                TMPAdditionalTooltipWeight.text = weightText;
            }
            HideAdditionalWeight(false);

        }

        public void HideAdditionalWeight(bool b)
        {
            if (additionalTooltipWeight != null)
                additionalTooltipWeight.gameObject.SetActive(!b);
            if (TMPAdditionalTooltipWeight != null)
                TMPAdditionalTooltipWeight.gameObject.SetActive(!b);
        }

        public void SetAdditionalDescription(string descriptionText)
        {
            if (additionalTooltipDescription != null)
            {
                additionalTooltipDescription.text = descriptionText;
                if (string.IsNullOrEmpty(descriptionText))
                {
                    additionalTooltipDescription.enabled = false;
                }
                else
                {
                    additionalTooltipDescription.enabled = true;
                }
            }
            if (TMPAdditionalTooltipDescription != null)
            {
                TMPAdditionalTooltipDescription.text = descriptionText;
                if (string.IsNullOrEmpty(descriptionText))
                {
                    TMPAdditionalTooltipDescription.enabled = false;
                }
                else
                {
                    TMPAdditionalTooltipDescription.enabled = true;
                }
            }
        }

        public void SetAdditionalIcon(Sprite icon)
        {
            if (icon != null)
            {
                EnableAdditionalIcon(true);
                additionalTooltipItemIcon.sprite = icon;
            }
            else
            {
                EnableAdditionalIcon(false);
            }
        }

        public void EnableAdditionalIcon(bool i)
        {
            additionalTooltipItemIconPanel.SetActive(i);
        }

        public void SetAdditionalQuality(int quality)
        {
            additionalTooltipOverlayIcon.color = AtavismSettings.Instance.ItemQualityColor(quality);
            if (additionalTooltipTitle != null)
                additionalTooltipTitle.color = AtavismSettings.Instance.ItemQualityColor(quality);
            if (TMPAdditionalTooltipTitle != null)
                TMPAdditionalTooltipTitle.color = AtavismSettings.Instance.ItemQualityColor(quality);
        }

        /// <summary>
        /// Set Attribute Row of tooltip
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        public void AddAdditionalAttributeTitle(string text)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.text = text;
            info.title = true;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.additionalAttributes.Add(info);
        }
        /// <summary>
        /// Set Attribute Row of tooltip
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        public void AddAdditionalAttributeTitle(string text, Color colour)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.text = text;
            info.textColour = colour;
            info.title = true;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.additionalAttributes.Add(info);
        }


        /// <summary>
        /// Set Attribute Row of tooltip
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        public void AddAdditionalAttributeSeperator()
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.separator = true;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.additionalAttributes.Add(info);
        }


        /// <summary>
        /// Set Attribute Row of tooltip
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        public void AddAdditionalAttributeSocket(string text, Sprite socketIcon, bool singleColumn)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.text = text;
            info.singleColumnRow = singleColumn;
            info.socket = true;
            info.socketIcon = socketIcon;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.additionalAttributes.Add(info);
        }
        /// <summary>
        /// Set Attribute Row of tooltip
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        public void AddAdditionalAttributeResource(string text, string value, Sprite socketIcon, bool singleColumn)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.text = text;
            info.value = value;
            info.singleColumnRow = singleColumn;
            info.resource = true;
            info.socketIcon = socketIcon;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.additionalAttributes.Add(info);
        }
        /// <summary>
        /// Set Attribute Row of tooltip
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        public void AddAdditionalAttribute(string text, string value, bool singleColumn)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.value = value;
            info.text = text;
            info.singleColumnRow = singleColumn;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.additionalAttributes.Add(info);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        /// <param name="colour"></param>
        public void AddAdditionalAttribute(string text, string value, bool singleColumn, Color colour)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            //  string colourText = string.Format("#{0:X2}{1:X2}{2:X2}ff", ToByte(colour.r), ToByte(colour.g), ToByte(colour.b));
            info.value = value;
            info.text = text;
            info.textColour = colour;
            info.singleColumnRow = singleColumn;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.additionalAttributes.Add(info);
        }

        /*

        public void AddAdditionalAttribute(string value, string text, bool singleColumn)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.value = value;
            info.text = text;
            info.singleColumnRow = singleColumn;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.additionalAttributes.Add(info);
        }

        public void AddAdditionalAttribute(string value, string text, bool singleColumn, Color colour)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            string colourText = string.Format("#{0:X2}{1:X2}{2:X2}ff", ToByte(colour.r), ToByte(colour.g), ToByte(colour.b));
            info.value = "<color=" + colourText + ">" + value;
            info.text = text + "</color>";
            info.singleColumnRow = singleColumn;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.additionalAttributes.Add(info);
        }*/

        public void ShowAdditionalTooltip()
        {
            if (!additionalTooltip.gameObject.activeSelf)
                additionalTooltip.gameObject.SetActive(true);
            // Cleanup any attributes left, if any at all
            CleanupAdditional();
            // Prepare the attributes
            if (additionalAttributes.Count > 0 && attributeRow != null)
            {
                bool isLeft = true;
                UGUITooltipAttributeRow lastRow = null;
                UGUITooltipSocketRow lastSocketRow = null;
                UGUITooltipResourceRow lastResourceRow = null;
                int lastDepth = 0;
                if (additionalTooltipTitle != null)
                    lastDepth = additionalTooltipTitle.depth;
                if (TMPAdditionalTooltipTitle != null)
                    lastDepth = TMPAdditionalTooltipTitle.depth;
                // Loop the attributes
                foreach (AttributeInfo info in additionalAttributes)
                {


                    if (info.separator)
                    {
                        GameObject obj = (GameObject)Instantiate(this.separator, additionalTooltipContentPanel.transform, false);
                        // Fix position and scale
                        obj.transform.localScale = Vector3.one;
                        obj.transform.localPosition = Vector3.zero;
                        obj.transform.localRotation = Quaternion.identity;

                        // Increase the depth
                        lastDepth = lastDepth + 1;
                        isLeft = true;
                        additionalAttributesRows.Add(obj);
                    }
                    else if (info.title)
                    {
                        GameObject obj = (GameObject)Instantiate(this.attributeTitle, additionalTooltipContentPanel.transform, false);
                        // Fix position and scale
                        obj.transform.localScale = Vector3.one;
                        obj.transform.localPosition = Vector3.zero;
                        obj.transform.localRotation = Quaternion.identity;
                        TextMeshProUGUI t = obj.GetComponent<TextMeshProUGUI>();
                        if (t != null)
                        {
                            t.text = info.text;
                            t.color = info.textColour;
                        }
                        // Increase the depth
                        lastDepth = lastDepth + 1;
                        isLeft = true;
                        additionalAttributesRows.Add(obj);
                    }
                    else
                    {

                        // Force left column in case it's a single column row
                        if (info.singleColumnRow)
                            isLeft = true;
                        if (isLeft)
                        {
                            // Instantiate a prefab
                            GameObject obj;
                            if (info.socket)
                            {
                                obj = (GameObject)Instantiate(this.socketRow, additionalTooltipContentPanel.transform, false);
                                //    Debug.LogWarning("Tooltip Instantiate socket isLeft:" + isLeft);

                            }
                            else if (info.resource)
                            {
                                obj = (GameObject)Instantiate(this.resourceRow, additionalTooltipContentPanel.transform, false);
                                //    Debug.LogWarning("Tooltip Instantiate resource isLeft:" + isLeft);
                            }
                            else
                            {
                                obj = (GameObject)Instantiate(this.attributeRow, additionalTooltipContentPanel.transform, false);
                                //     Debug.LogWarning("Tooltip Instantiate else isLeft:" + isLeft);
                            }
                            // Fix position and scale
                            obj.transform.localScale = Vector3.one;
                            obj.transform.localPosition = Vector3.zero;
                            obj.transform.localRotation = Quaternion.identity;

                            // Increase the depth
                            lastDepth = lastDepth + 1;

                            // Get the attribute row script referrence
                            lastRow = obj.GetComponent<UGUITooltipAttributeRow>();
                            lastSocketRow = obj.GetComponent<UGUITooltipSocketRow>();
                            lastResourceRow = obj.GetComponent<UGUITooltipResourceRow>();

                            // Make some changes if it's a single column row
                            if (info.singleColumnRow)
                            {
                                // Destroy the right column
                                //			Destroy(lastRow.rightText.gameObject);
                            }

                            // Add it to the instanced objects list
                            additionalAttributesRows.Add(obj);
                        }
                        // Check if we have a row object to work with
                        if (lastRow != null)
                        {
                            Text text = null;
                            Text textValue = null;
                            TextMeshProUGUI TMPtext = null;
                            TextMeshProUGUI TMPtextValue = null;
                            if (isLeft)
                            {
                                if (lastRow.leftText != null)
                                    text = lastRow.leftText;
                                if (lastRow.leftTextValue != null)
                                    textValue = lastRow.leftTextValue;
                                if (lastRow.TMPLeftText != null)
                                    TMPtext = lastRow.TMPLeftText;
                                if (lastRow.TMPLeftTextValue != null)
                                    TMPtextValue = lastRow.TMPLeftTextValue;
                            }
                            else
                            {
                                if (lastRow.rightText != null)
                                    text = lastRow.rightText;
                                if (lastRow.rightTextValue != null)
                                    textValue = lastRow.rightTextValue;
                                if (lastRow.TMPRightText != null)
                                    TMPtext = lastRow.TMPRightText;
                                if (lastRow.TMPRightTextValue != null)
                                    TMPtextValue = lastRow.TMPRightTextValue;
                            }

                            // Check if we have the label
                            if (text != null)
                            {
                                if (textValue == null)
                                {
                                    // Set the label text
                                    text.text = info.value + info.text;
                                    text.color = info.textColour;

                                }
                                else
                                {
                                    text.text = info.text;
                                    textValue.text = info.value;
                                    text.color = info.textColour;
                                    textValue.color = info.textColour;
                                }
                                // Flip is left
                                if (!info.singleColumnRow)
                                    isLeft = !isLeft;
                            }
                            if (TMPtext != null)
                            {
                                if (TMPtextValue == null)
                                {
                                    // Set the label text
                                    TMPtext.text = info.value + info.text;
                                    TMPtext.color = info.textColour;
                                }
                                else
                                {
                                    TMPtext.text = info.text;
                                    TMPtextValue.text = info.value;
                                    TMPtext.color = info.textColour;
                                    TMPtextValue.color = info.textColour;
                                }
                                // Flip is left
                                if (!info.singleColumnRow)
                                    isLeft = !isLeft;
                            }
                        }
                        //Sockets
                        // Check if we have a row object to work with
                        if (lastSocketRow != null)
                        {
                            //   Debug.LogWarning("Tooltip lastSocketRow is not null isLeft:" + isLeft);
                            TextMeshProUGUI textRow = null;
                            Image iconRow = null;
                            if (isLeft)
                            {
                                if (lastSocketRow.TMPLeftText != null)
                                    textRow = lastSocketRow.TMPLeftText;
                                if (lastSocketRow.leftIcon != null)
                                    iconRow = lastSocketRow.leftIcon;
                            }
                            else
                            {
                                if (lastSocketRow.TMPRightText != null)
                                    textRow = lastSocketRow.TMPRightText;
                                if (lastSocketRow.rightIcon != null)
                                    iconRow = lastSocketRow.rightIcon;
                            }

                            // Check if we have the label
                            if (textRow != null)
                            {
                                // Set the label text
                                textRow.text = info.text;
                                textRow.color = info.textColour;
                                iconRow.enabled = true;
                                if (info.socketIcon != null)
                                    iconRow.sprite = info.socketIcon;
                                //  if (info.socketIcon == null)
                                //      iconRow.enabled = false;
                                // Flip is left
                                if (!info.singleColumnRow)
                                    isLeft = !isLeft;
                            }
                            else
                                Debug.LogWarning("Tooltip Socket row text is null isLeft:" + isLeft);

                        }
                        //Resource
                        // Check if we have a row object to work with
                        if (lastResourceRow != null)
                        {
                            TextMeshProUGUI textRow = null;
                            TextMeshProUGUI valueRow = null;
                            Image iconRow = null;
                            if (isLeft)
                            {
                                if (lastResourceRow.TMPLeftText != null)
                                    textRow = lastResourceRow.TMPLeftText;
                                if (lastResourceRow.TMPLeftTextValue != null)
                                    valueRow = lastResourceRow.TMPLeftTextValue;
                                if (lastResourceRow.leftIcon != null)
                                    iconRow = lastResourceRow.leftIcon;
                            }
                            else
                            {
                                if (lastResourceRow.TMPRightText != null)
                                    textRow = lastResourceRow.TMPRightText;
                                if (lastResourceRow.TMPRightTextValue != null)
                                    valueRow = lastResourceRow.TMPRightTextValue;
                                if (lastResourceRow.rightIcon != null)
                                    iconRow = lastResourceRow.rightIcon;
                            }

                            // Check if we have the label
                            if (textRow != null)
                            {
                                // Set the label text
                                textRow.text = info.text;
                                textRow.color = info.textColour;
                                if (valueRow != null)
                                {
                                    valueRow.text = info.value;
                                    valueRow.color = info.textColour;
                                }
                                else
                                    Debug.LogWarning("Tooltip Resource row value text is null isLeft:" + isLeft);

                                iconRow.enabled = true;
                                if (info.socketIcon != null)
                                    iconRow.sprite = info.socketIcon;
                                //  if (info.socketIcon == null)
                                //      iconRow.enabled = false;
                                // Flip is left
                                if (!info.singleColumnRow)
                                    isLeft = !isLeft;
                            }
                            else
                                Debug.LogWarning("Tooltip Resource row text is null isLeft:" + isLeft);

                        }

                    }


                }
                // Clear the attributes list, we no longer need it
                additionalAttributes.Clear();
            }
            if (additionalTooltipDescription != null)
                additionalTooltipDescription.transform.SetAsLastSibling();
            if (TMPAdditionalTooltipDescription != null)
                TMPAdditionalTooltipDescription.transform.SetAsLastSibling();

            //   AtavimUIUtility.BringToFront(this.gameObject);

            //   showing = true;
            // this.target = target;
        }
        private void CleanupAdditional()
        {
            //Destroy the attributes
            foreach (GameObject obj in additionalAttributesRows)
            {
                if (obj != null)
                    DestroyImmediate(obj);
            }

            // Clear the list
            additionalAttributesRows.Clear();
        }
        #endregion Additional Tooltip Functions

        #region Additional 2 Tooltip Functions

        public void SetAdditionalTitle2(string titleText)
        {
            if (additionalTooltipTitle2 != null)
            {
                additionalTooltipTitle2.text = titleText;
                additionalTooltipTitle2.color = defaultTitleColor;
            }
            if (TMPAdditionalTooltipTitle2 != null)
            {
                TMPAdditionalTooltipTitle2.text = titleText;
                TMPAdditionalTooltipTitle2.color = defaultTitleColor;
            }
            HideAdditionalTitle2(false);
        }

        public void HideAdditionalTitle2(bool b)
        {
            if (additionalTooltipTitle2 != null)
                additionalTooltipTitle2.gameObject.SetActive(!b);
            if (TMPAdditionalTooltipTitle2 != null)
                TMPAdditionalTooltipTitle2.gameObject.SetActive(!b);

        }

        public void SetAdditionalTitleColour2(Color color)
        {
            if (additionalTooltipTitle2 != null)
                additionalTooltipTitle2.color = color;
            if (TMPAdditionalTooltipTitle2 != null)
                TMPAdditionalTooltipTitle2.color = color;
        }

        public void SetAdditionalType2(string typeText)
        {
            if (additionalTooltipType2 != null)
                additionalTooltipType2.text = typeText;
            if (TMPAdditionalTooltipType2 != null)
                TMPAdditionalTooltipType2.text = typeText;
            HideAdditionalType2(false);
        }

        public void HideAdditionalType2(bool b)
        {
            if (additionalTooltipType2 != null)
                additionalTooltipType2.gameObject.SetActive(!b);
            if (TMPAdditionalTooltipType2 != null)
                TMPAdditionalTooltipType2.gameObject.SetActive(!b);
        }

        public void SetAdditionalTypeColour2(Color color)
        {
            if (additionalTooltipType2 != null)
                additionalTooltipType2.color = color;
            if (TMPAdditionalTooltipType2 != null)
                TMPAdditionalTooltipType2.color = color;
        }

        public void SetAdditionalWeight2(string weightText)
        {
            if (additionalTooltipWeight2 != null)
            {
                additionalTooltipWeight2.text = weightText;
            }
            if (TMPAdditionalTooltipWeight2 != null)
            {
                TMPAdditionalTooltipWeight2.text = weightText;
            }
            HideAdditionalWeight2(false);

        }

        public void HideAdditionalWeight2(bool b)
        {
            if (additionalTooltipWeight2 != null)
                additionalTooltipWeight2.gameObject.SetActive(!b);
            if (TMPAdditionalTooltipWeight2 != null)
                TMPAdditionalTooltipWeight2.gameObject.SetActive(!b);
        }

        public void SetAdditionalDescription2(string descriptionText)
        {
            if (additionalTooltipDescription2 != null)
            {
                additionalTooltipDescription2.text = descriptionText;
                if (string.IsNullOrEmpty(descriptionText))
                {
                    additionalTooltipDescription2.enabled = false;
                }
                else
                {
                    additionalTooltipDescription2.enabled = true;
                }
            }
            if (TMPAdditionalTooltipDescription2 != null)
            {
                TMPAdditionalTooltipDescription2.text = descriptionText;
                if (string.IsNullOrEmpty(descriptionText))
                {
                    TMPAdditionalTooltipDescription2.enabled = false;
                }
                else
                {
                    TMPAdditionalTooltipDescription2.enabled = true;
                }
            }
        }

        public void SetAdditionalIcon2(Sprite icon)
        {
            if (icon != null)
            {
                EnableAdditionalIcon2(true);
                additionalTooltipItemIcon2.sprite = icon;
            }
            else
            {
                EnableAdditionalIcon2(false);
            }
        }

        public void EnableAdditionalIcon2(bool i)
        {
            additionalTooltipItemIconPanel2.SetActive(i);
        }

        public void SetAdditionalQuality2(int quality)
        {
            additionalTooltipOverlayIcon2.color = AtavismSettings.Instance.ItemQualityColor(quality);
            if (additionalTooltipTitle2 != null)
                additionalTooltipTitle2.color = AtavismSettings.Instance.ItemQualityColor(quality);
            if (TMPAdditionalTooltipTitle2 != null)
                TMPAdditionalTooltipTitle2.color = AtavismSettings.Instance.ItemQualityColor(quality);
        }

        /// <summary>
        /// Set Attribute Row of tooltip
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        public void AddAdditionalAttributeTitle2(string text)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.text = text;
            info.title = true;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.additionalAttributes2.Add(info);
        }
        /// <summary>
        /// Set Attribute Row of tooltip
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        public void AddAdditionalAttributeTitle2(string text, Color colour)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.text = text;
            info.textColour = colour;
            info.title = true;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.additionalAttributes2.Add(info);
        }


        /// <summary>
        /// Set Attribute Row of tooltip
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        public void AddAdditionalAttributeSeperator2()
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.separator = true;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.additionalAttributes2.Add(info);
        }


        /// <summary>
        /// Set Attribute Row of tooltip
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        public void AddAdditionalAttributeSocket2(string text, Sprite socketIcon, bool singleColumn)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.text = text;
            info.singleColumnRow = singleColumn;
            info.socket = true;
            info.socketIcon = socketIcon;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.additionalAttributes2.Add(info);
        }
        /// <summary>
        /// Set Attribute Row of tooltip
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        public void AddAdditionalAttributeResource2(string text, string value, Sprite socketIcon, bool singleColumn)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.text = text;
            info.value = value;
            info.singleColumnRow = singleColumn;
            info.resource = true;
            info.socketIcon = socketIcon;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.additionalAttributes2.Add(info);
        }
        /// <summary>
        /// Set Attribute Row of tooltip
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        public void AddAdditionalAttribute2(string text, string value, bool singleColumn)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.value = value;
            info.text = text;
            info.singleColumnRow = singleColumn;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.additionalAttributes2.Add(info);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <param name="singleColumn"></param>
        /// <param name="colour"></param>
        public void AddAdditionalAttribute2(string text, string value, bool singleColumn, Color colour)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            //    string colourText = string.Format("#{0:X2}{1:X2}{2:X2}ff", ToByte(colour.r), ToByte(colour.g), ToByte(colour.b));
            info.value = value;
            info.text = text;
            info.textColour = colour;
            info.singleColumnRow = singleColumn;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.additionalAttributes2.Add(info);
        }

        /*

        public void AddAdditionalAttribute(string value, string text, bool singleColumn)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            info.value = value;
            info.text = text;
            info.singleColumnRow = singleColumn;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.additionalAttributes.Add(info);
        }

        public void AddAdditionalAttribute(string value, string text, bool singleColumn, Color colour)
        {
            // Create new attribute info
            AttributeInfo info = new AttributeInfo();
            string colourText = string.Format("#{0:X2}{1:X2}{2:X2}ff", ToByte(colour.r), ToByte(colour.g), ToByte(colour.b));
            info.value = "<color=" + colourText + ">" + value;
            info.text = text + "</color>";
            info.singleColumnRow = singleColumn;
            info.margin = new RectOffset();

            // Add it to the attribute list
            instance.additionalAttributes.Add(info);
        }*/

        public void ShowAdditionalTooltip2()
        {
            if (!additionalTooltip2.gameObject.activeSelf)
                additionalTooltip2.gameObject.SetActive(true);
            // Cleanup any attributes left, if any at all
            CleanupAdditional2();
            // Prepare the attributes
            if (additionalAttributes2.Count > 0 && attributeRow != null)
            {
                bool isLeft = true;
                UGUITooltipAttributeRow lastRow = null;
                UGUITooltipSocketRow lastSocketRow = null;
                UGUITooltipResourceRow lastResourceRow = null;
                int lastDepth = 0;
                if (additionalTooltipTitle2 != null)
                    lastDepth = additionalTooltipTitle2.depth;
                if (TMPAdditionalTooltipTitle2 != null)
                    lastDepth = TMPAdditionalTooltipTitle2.depth;
                // Loop the attributes
                foreach (AttributeInfo info in additionalAttributes2)
                {


                    if (info.separator)
                    {
                        GameObject obj = (GameObject)Instantiate(this.separator, additionalTooltipContentPanel2.transform, false);
                        // Fix position and scale
                        obj.transform.localScale = Vector3.one;
                        obj.transform.localPosition = Vector3.zero;
                        obj.transform.localRotation = Quaternion.identity;

                        // Increase the depth
                        lastDepth = lastDepth + 1;
                        isLeft = true;
                        additionalAttributesRows2.Add(obj);
                    }
                    else if (info.title)
                    {
                        GameObject obj = (GameObject)Instantiate(this.attributeTitle, additionalTooltipContentPanel2.transform, false);
                        // Fix position and scale
                        obj.transform.localScale = Vector3.one;
                        obj.transform.localPosition = Vector3.zero;
                        obj.transform.localRotation = Quaternion.identity;
                        TextMeshProUGUI t = obj.GetComponent<TextMeshProUGUI>();
                        if (t != null)
                        {
                            t.text = info.text;
                            t.color = info.textColour;
                        }

                        // Increase the depth
                        lastDepth = lastDepth + 1;
                        isLeft = true;
                        additionalAttributesRows2.Add(obj);
                    }
                    else
                    {

                        // Force left column in case it's a single column row
                        if (info.singleColumnRow) 
                        isLeft = true;
                        if (isLeft)
                        {
                            GameObject obj;
                            // Instantiate a prefab
                            if (info.socket)
                            {
                                obj = (GameObject)Instantiate(this.socketRow, additionalTooltipContentPanel2.transform, false);
                            }
                            else if (info.resource)
                            {
                                obj = (GameObject)Instantiate(this.resourceRow, additionalTooltipContentPanel2.transform, false);
                            }
                            else
                            {
                                obj = (GameObject)Instantiate(this.attributeRow, additionalTooltipContentPanel2.transform, false);
                            }
                            // Fix position and scale
                            obj.transform.localScale = Vector3.one;
                            obj.transform.localPosition = Vector3.zero;
                            obj.transform.localRotation = Quaternion.identity;

                            // Increase the depth
                            lastDepth = lastDepth + 1;

                            // Get the attribute row script referrence
                            lastRow = obj.GetComponent<UGUITooltipAttributeRow>();
                            lastSocketRow = obj.GetComponent<UGUITooltipSocketRow>();
                            lastResourceRow = obj.GetComponent<UGUITooltipResourceRow>();

                            // Make some changes if it's a single column row
                            if (info.singleColumnRow)
                            {
                                // Destroy the right column
                                //			Destroy(lastRow.rightText.gameObject);
                            }

                            // Add it to the instanced objects list
                            additionalAttributesRows2.Add(obj);
                        }
                        // Check if we have a row object to work with
                        if (lastRow != null)
                        {
                            Text text = null;
                            Text textValue = null;
                            TextMeshProUGUI TMPtext = null;
                            TextMeshProUGUI TMPtextValue = null;
                            if (isLeft)
                            {
                                if (lastRow.leftText != null)
                                    text = lastRow.leftText;
                                if (lastRow.leftTextValue != null)
                                    textValue = lastRow.leftTextValue;
                                if (lastRow.TMPLeftText != null)
                                    TMPtext = lastRow.TMPLeftText;
                                if (lastRow.TMPLeftTextValue != null)
                                    TMPtextValue = lastRow.TMPLeftTextValue;
                            }
                            else
                            {
                                if (lastRow.rightText != null)
                                    text = lastRow.rightText;
                                if (lastRow.rightTextValue != null)
                                    textValue = lastRow.rightTextValue;
                                if (lastRow.TMPRightText != null)
                                    TMPtext = lastRow.TMPRightText;
                                if (lastRow.TMPRightTextValue != null)
                                    TMPtextValue = lastRow.TMPRightTextValue;
                            }

                            // Check if we have the label
                            if (text != null)
                            {
                                if (textValue == null)
                                {
                                    // Set the label text
                                    text.text = info.value + info.text;
                                    text.color = info.textColour;

                                }
                                else
                                {
                                    text.text = info.text;
                                    textValue.text = info.value;
                                    text.color = info.textColour;
                                    textValue.color = info.textColour;
                                }
                                // Flip is left
                                if (!info.singleColumnRow)
                                    isLeft = !isLeft;
                            }
                            if (TMPtext != null)
                            {
                                if (TMPtextValue == null)
                                {
                                    // Set the label text
                                    TMPtext.text = info.value + info.text;
                                    TMPtext.color = info.textColour;
                                }
                                else
                                {
                                    TMPtext.text = info.text;
                                    TMPtextValue.text = info.value;
                                    TMPtext.color = info.textColour;
                                    TMPtextValue.color = info.textColour;
                                }
                                // Flip is left
                                if (!info.singleColumnRow)
                                    isLeft = !isLeft;
                            }
                        }
                        //Sockets
                        // Check if we have a row object to work with
                        if (lastSocketRow != null)
                        {
                            TextMeshProUGUI textRow = null;
                            Image iconRow = null;
                            if (isLeft)
                            {
                                if (lastSocketRow.TMPLeftText != null)
                                    textRow = lastSocketRow.TMPLeftText;
                                if (lastSocketRow.leftIcon != null)
                                    iconRow = lastSocketRow.leftIcon;
                            }
                            else
                            {
                                if (lastSocketRow.TMPRightText != null)
                                    textRow = lastSocketRow.TMPRightText;
                                if (lastSocketRow.rightIcon != null)
                                    iconRow = lastSocketRow.rightIcon;
                            }

                            // Check if we have the label
                            if (textRow != null)
                            {
                                // Set the label text
                                textRow.text = info.text;
                                textRow.color = info.textColour;
                                iconRow.enabled = true;
                                if (info.socketIcon != null)
                                    iconRow.sprite = info.socketIcon;
                                //  if (info.socketIcon == null)
                                //      iconRow.enabled = false;
                                // Flip is left
                                if (!info.singleColumnRow)
                                    isLeft = !isLeft;
                            }

                        }
                        //Resource
                        // Check if we have a row object to work with
                        if (lastResourceRow != null)
                        {
                            TextMeshProUGUI textRow = null;
                            TextMeshProUGUI valueRow = null;
                            Image iconRow = null;
                            if (isLeft)
                            {
                                if (lastResourceRow.TMPLeftText != null)
                                    textRow = lastResourceRow.TMPLeftText;
                                if (lastResourceRow.TMPLeftTextValue != null)
                                    valueRow = lastResourceRow.TMPLeftTextValue;
                                if (lastResourceRow.leftIcon != null)
                                    iconRow = lastResourceRow.leftIcon;
                            }
                            else
                            {
                                if (lastResourceRow.TMPRightText != null)
                                    textRow = lastResourceRow.TMPRightText;
                                if (lastResourceRow.TMPRightTextValue != null)
                                    valueRow = lastResourceRow.TMPRightTextValue;
                                if (lastResourceRow.rightIcon != null)
                                    iconRow = lastResourceRow.rightIcon;
                            }

                            // Check if we have the label
                            if (textRow != null)
                            {
                                // Set the label text
                                textRow.text = info.text;
                                if (valueRow != null)
                                    valueRow.text = info.value;
                                else
                                    Debug.LogWarning("Tooltip Resource row value text is null isLeft:" + isLeft);

                                iconRow.enabled = true;
                                if (info.socketIcon != null)
                                    iconRow.sprite = info.socketIcon;
                                //  if (info.socketIcon == null)
                                //      iconRow.enabled = false;
                                // Flip is left
                                if (!info.singleColumnRow)
                                    isLeft = !isLeft;
                            }
                            else
                                Debug.LogWarning("Tooltip Resource row text is null isLeft:" + isLeft);

                        }



                    }
                }
                // Clear the attributes list, we no longer need it
                additionalAttributes2.Clear();
            }
            if (additionalTooltipDescription2 != null)
                additionalTooltipDescription2.transform.SetAsLastSibling();
            if (TMPAdditionalTooltipDescription2 != null)
                TMPAdditionalTooltipDescription2.transform.SetAsLastSibling();

            //   AtavimUIUtility.BringToFront(this.gameObject);

            //   showing = true;
            // this.target = target;
        }
        private void CleanupAdditional2()
        {
            //Destroy the attributes
            foreach (GameObject obj in additionalAttributesRows2)
            {
                if (obj != null)
                    DestroyImmediate(obj);
            }

            // Clear the list
            additionalAttributesRows2.Clear();
        }
        #endregion Additional 2 Tooltip Functions

    }
}