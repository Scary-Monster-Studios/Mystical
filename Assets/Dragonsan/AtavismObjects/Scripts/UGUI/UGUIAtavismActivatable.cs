using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

namespace Atavism
{

    public enum ActivatableType
    {
        Item,
        Ability,
        Bag,
        Action,
        Other
    }

    public class UGUIAtavismActivatable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {

        public Image icoImage;
        public Image cooldownImage;
        public Text countText;
        public TextMeshProUGUI TMPCountText;
        public Material matGray;
        public Image borderColorImage;
        public Image hover;
        public GameObject toggleOn;
        [SerializeField] Image overlay;
        [SerializeField] TextMeshProUGUI cooldownText;
        Coroutine cor;
        //[SerializeField]
        public Vector2 defaultSlotSize = new Vector2(48f, 48f);
        public bool sizeFromParent = true;
        protected bool beingDragged = false;
        protected UGUIDraggableSlot releaseTarget = null;
        protected UGUIDraggableSlot source = null;
        protected Activatable activatableObject;
        protected ActivatableType activatableType;
        protected bool preventDiscard = false;
        protected UGUIAtavismActivatable link;
        protected bool cooldownRun = false;

        void Start()
        {
            if (cooldownImage != null)
                AtavismEventSystem.RegisterEvent("COOLDOWN_UPDATE", this);
            ClientAPI.GetPlayerObject().RegisterPropertyChangeHandler("level", LevelHandler);
            if (this.icoImage == null)
                this.icoImage = GetComponent<Image>();
            AtavismEventSystem.RegisterEvent("ATOGGLE_UPDATE", this);
            if (toggleOn != null)
            {
                toggleOn.gameObject.SetActive(false);
            }
            AtavismEventSystem.RegisterEvent("ITEM_RELOAD", this);
            AtavismEventSystem.RegisterEvent("ABILITY_UPDATE", this);
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("COOLDOWN_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("ATOGGLE_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("ITEM_RELOAD", this);
            AtavismEventSystem.UnregisterEvent("ABILITY_UPDATE", this);

            if (ClientAPI.GetPlayerObject() != null)
                ClientAPI.GetPlayerObject().RemovePropertyChangeHandler("level", LevelHandler);
        }

        void OnEnable()
        {
            RunCooldownUpdate();
        }

        void OnDisable()
        {
            //	StopCoroutine(UpdateCooldown());
            if (corutRuning)
                StopCoroutine(cor);

        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "COOLDOWN_UPDATE")
            {
                // Update 
                RunCooldownUpdate();
            }
            else if (eData.eventType == "ATOGGLE_UPDATE")
            {
                RunToggleUpdate();
            }
            else if (eData.eventType == "ITEM_RELOAD")
            {
               // Debug.LogError("UGUIAtavismActivatable ITEM_RELOAD");
                if (activatableObject is AtavismInventoryItem)
                {
                    AtavismInventoryItem item = (AtavismInventoryItem) activatableObject;
                    if (item != null)
                        activatableObject = AtavismPrefabManager.Instance.LoadItem(item);
                }
            }
            else if (eData.eventType == "ABILITY_UPDATE")
            {
             //   Debug.LogError("UGUIAtavismActivatable ABILITY_UPDATE");
                if (activatableObject is AtavismAbility)
                {
                    AtavismAbility ability = (AtavismAbility) activatableObject;
                    if (ability != null)
                        activatableObject = AtavismPrefabManager.Instance.LoadAbility(ability);
                }
            }
        }

        void RunToggleUpdate()
        {
            if (activatableObject is AtavismAbility)
            {
                AtavismAbility ability = (AtavismAbility)activatableObject;
                if (ability.toggle)
                {
                    bool toggle = Abilities.Instance.isToggleActive(ability.id);
                    if (toggleOn != null)
                    {
                        if(toggle)
                            toggleOn.gameObject.SetActive(true);
                        else
                            toggleOn.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (toggleOn != null)
                    {
                        toggleOn.gameObject.SetActive(false);
                    }
                }
            }
        }

        void RunCooldownUpdate()
        {
            // Check if this is on cooldown
            if (cooldownImage == null)
                return;

            cooldownImage.fillAmount = 0;

            if (activatableObject == null)
                return;

            if (activatableObject.GetLongestActiveCooldown() == null)
                return;
            if (!gameObject.activeInHierarchy)
                return;
            if (corutRuning)
            {
                StopCoroutine(cor);
                cor = StartCoroutine(UpdateCooldown());
            }
            else
                cor = StartCoroutine(UpdateCooldown());
        }
        bool corutRuning = false;


        IEnumerator UpdateCooldown()
        {
            if (!cooldownImage || activatableObject == null)
                yield break;
            corutRuning = true;
            //   WaitForSeconds delay = new WaitForSeconds(0.3f);
            Cooldown c = activatableObject.GetLongestActiveCooldown();

            while (c != null && Time.time < c.expiration)
            {
                cooldownRun = true;
                float total = c.length;
                //            float total = c.expiration - (c.expiration-c.length);
                float currentTime = c.expiration - Time.time;
                cooldownImage.fillAmount = ((float)currentTime / (float)total);
                if (cooldownText != null)
                    cooldownText.text = Math.Round((decimal)currentTime, 0) > 0 ? Math.Round((decimal)currentTime, 0).ToString() : "";
                yield return new WaitForEndOfFrame();
            }
            cooldownRun = false;
            if (cooldownText != null)
                cooldownText.text = "";
            cooldownImage.fillAmount = 0;
            corutRuning = false;

            yield return null;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (link != null)
                return;
            beingDragged = true;
            if (activatableObject is AtavismAbility)
            {
                AtavismAbility ability = (AtavismAbility)activatableObject;
                if (ability.passive)
                    return;
            }
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            transform.SetParent(GameObject.Find("Canvas").transform);
            if (GetComponent<RectTransform>().anchorMin == new Vector2(0.5f, 0.5f) && GetComponent<RectTransform>().anchorMax == new Vector2(0.5f, 0.5f))
            {
                GetComponent<RectTransform>().sizeDelta = defaultSlotSize;
                //    Debug.LogError("set sizeDelta :" + defaultSlotSize);
            }
            else
                GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            AtavismCursor.Instance.UguiIconBeingDragged = true;
            UGUITooltip.Instance.Hide();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (link != null)
                return;
            if (activatableObject is AtavismAbility)
            {
                AtavismAbility ability = (AtavismAbility)activatableObject;
                if (ability.passive)
                    return;
            }
            this.transform.position = eventData.position;
        }

        /// <summary>
        /// Raises the end drag event. This is called after the OnDrop is run for the slot.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (link != null && !beingDragged)
                return;
            //Debug.LogError("Got drag end");
            beingDragged = false;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            AtavismCursor.Instance.UguiIconBeingDragged = false;
            //this.transform.parent = source.transform;

            /*if (releaseTarget == null) {
                GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                source.Discarded();
                return;
            }*/

            // If the drop target is a reference slot and the source isn't then return a copy back to the source
            if (releaseTarget.SlotBehaviour == DraggableBehaviour.Reference && releaseTarget != source)
            {
                if (source.SlotBehaviour != DraggableBehaviour.Reference)
                    this.transform.SetParent(source.transform, false);
                GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

                if (source.SlotBehaviour != DraggableBehaviour.Reference)
                    if (GetComponent<RectTransform>().anchorMin == new Vector2(0.5f, 0.5f) && GetComponent<RectTransform>().anchorMax == new Vector2(0.5f, 0.5f))
                    {
                        GetComponent<RectTransform>().sizeDelta = defaultSlotSize;
                        if (sizeFromParent)
                            GetComponent<RectTransform>().sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
                        //    Debug.LogError("set sizeDelta :" + defaultSlotSize);
                    }
                    else
                        GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                GetComponent<RectTransform>().localScale = Vector3.one;

                // this.releaseTarget = source;
                return;
            }

            // If the drop target is a temporarty slot and the source isn't then return a copy back to the source
            if (releaseTarget.SlotBehaviour == DraggableBehaviour.Temporary && releaseTarget != source)
            {
                if (source.SlotBehaviour != DraggableBehaviour.Temporary)
                {
                    this.transform.SetParent(source.transform, false);
                }
                else
                {
                    source = releaseTarget;
                }
                GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                GetComponent<RectTransform>().localScale = Vector3.one;
                if (GetComponent<RectTransform>().anchorMin == new Vector2(0.5f, 0.5f) && GetComponent<RectTransform>().anchorMax == new Vector2(0.5f, 0.5f))
                {
                    GetComponent<RectTransform>().sizeDelta = defaultSlotSize;
                    if (sizeFromParent)
                        GetComponent<RectTransform>().sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
                    //    Debug.LogError("set sizeDelta :" + defaultSlotSize);
                }
                else
                    GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                this.releaseTarget = source;

                return;
            }

            this.transform.SetParent(releaseTarget.transform);
            if (releaseTarget != source)
            {
                source.ClearChildSlot();
                GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                if (GetComponent<RectTransform>().anchorMin == new Vector2(0.5f, 0.5f) && GetComponent<RectTransform>().anchorMax == new Vector2(0.5f, 0.5f))
                {
                    GetComponent<RectTransform>().sizeDelta = defaultSlotSize;
                    if (sizeFromParent)
                        GetComponent<RectTransform>().sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
                    //     Debug.LogError("set sizeDelta :" + defaultSlotSize);
                }
                else
                    GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                GetComponent<RectTransform>().localScale = Vector3.one;
            }
            else
            {
                if (GetComponent<RectTransform>().anchorMin == new Vector2(0.5f, 0.5f) && GetComponent<RectTransform>().anchorMax == new Vector2(0.5f, 0.5f))
                {
                    GetComponent<RectTransform>().sizeDelta = defaultSlotSize;
                    if (sizeFromParent)
                        GetComponent<RectTransform>().sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
                    //   Debug.LogError("set sizeDelta :" + defaultSlotSize);
                }
                else
                    GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                GetComponent<RectTransform>().localScale = Vector3.one;
                if (preventDiscard)
                {
                    preventDiscard = false;
                    return;
                }
                source.Discarded();
            }
        }

        /*public virtual void OnClick() {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                if (activatableObject is AtavismInventoryItem && source.SlotBehaviour == DraggableBehaviour.Standard) {
                    AtavismInventoryItem item = (AtavismInventoryItem)activatableObject;
                    Inventory.Instance.CreateSplitStack(item, item.Count / 2);
                }
            }
            source.Activate();
        }*/

        public void OnPointerClick(PointerEventData eventData)
        {
            //   if (!cooldownRun) {

            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                if (activatableObject is AtavismInventoryItem && source.SlotBehaviour == DraggableBehaviour.Standard)
                {
                    AtavismInventoryItem item = (AtavismInventoryItem)activatableObject;
                    if (eventData.button == PointerEventData.InputButton.Right)
                    {
                        Inventory.Instance.CreateSplitStack(item, 1);
                    }
                    else
                    {
                        Inventory.Instance.CreateSplitStack(item, item.Count / 2);
                    }
                    return;
                }
            }
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (activatableObject is AtavismInventoryItem)
                {
                    AtavismInventoryItem item = (AtavismInventoryItem)activatableObject;

#if AT_I2LOC_PRESET
                UGUIChatController.Instance.input.text += "<link=item#" + item.TemplateId + "><" + ColorTypeConverter.ToRGBHex(AtavismSettings.Instance.ItemQualityColor(item.quality)) + ">" + I2.Loc.LocalizationManager.GetTranslation("Items/" + item.name) + "</color></link>";
#else
                    UGUIChatController.Instance.input.text += "<link=item#" + item.TemplateId + "><" + ColorTypeConverter.ToRGBHex(AtavismSettings.Instance.ItemQualityColor(item.quality)) + ">" + item.name + "</color></link>";

#endif
                    EventSystem.current.SetSelectedGameObject(UGUIChatController.Instance.input.gameObject);
                }
                else if (activatableObject is AtavismAbility)
                {
                    AtavismAbility ability = (AtavismAbility)activatableObject;
#if AT_I2LOC_PRESET
               UGUIChatController.Instance.input.text += "<link=ability#" + ability.id + "><" + ColorTypeConverter.ToRGBHex(Color.white) + ">" + I2.Loc.LocalizationManager.GetTranslation("Ability/" + ability.name) + "</color></link>";
#else
                    UGUIChatController.Instance.input.text += "<link=ability#" + ability.id + "><" + ColorTypeConverter.ToRGBHex(Color.white) + ">" + ability.name + "</color></link>";
#endif
                    EventSystem.current.SetSelectedGameObject(UGUIChatController.Instance.input.gameObject);
                }
                return;
            }

            source.Activate();
            //   }
        }

        public void LevelHandler(object sender, PropertyChangeEventArgs args)
        {
            if (overlay != null && activatableObject is AtavismInventoryItem)
            {
                AtavismInventoryItem item = (AtavismInventoryItem)activatableObject;
                if (item.ReqLeval > (int)ClientAPI.GetPlayerObject().GetProperty("level"))
                    overlay.enabled = true;
                else
                    overlay.enabled = false;
            }
        }


        public void SetActivatable(Activatable obj, ActivatableType activatableType, UGUIDraggableSlot parent)
        {
            SetActivatable(obj, activatableType, parent, true);
        }

        public void SetActivatable(Activatable obj, ActivatableType activatableType, UGUIDraggableSlot parent, bool showCooldown)
        {
            if (beingDragged)
                return;
            if (this.activatableObject != obj)
            {
                if (cor != null)
                {
                    StopCoroutine(cor);
                    if (cooldownText != null)
                        cooldownText.text = "";
                }
            }

            this.activatableObject = obj;
            this.activatableType = activatableType;
            this.source = parent;
            this.releaseTarget = parent;
            if (this.icoImage == null)
                this.icoImage = GetComponent<Image>();
            if (this.icoImage != null)
            {
                this.icoImage.enabled = true;
                if(obj.icon!=null)
                    this.icoImage.sprite = obj.icon;
                else
                    this.icoImage.sprite = AtavismSettings.Instance.defaultItemIcon;
            }
            //this.GetComponent<Image>().sprite = obj.icon;
            if (hover != null)
                hover.enabled = false;
            if (overlay != null && obj is AtavismInventoryItem)
            {
                AtavismInventoryItem item = (AtavismInventoryItem)obj;
                //int l = ClientAPI.GetPlayerObject.
                if (item.ReqLeval > (int)ClientAPI.GetPlayerObject().GetProperty("level"))
                    overlay.enabled = true;
                else
                    overlay.enabled = false;
            }

            if (this.countText != null && obj is AtavismInventoryItem)
            {
                AtavismInventoryItem item = (AtavismInventoryItem)obj;
                int count = item.Count;
                if (parent is UGUIActionBarSlot)
                {
                    count = Inventory.Instance.GetCountOfItem(item.templateId);
                }
                if (count > 1)
                {
                    countText.text = count.ToString();
                }
                else
                {
                    countText.text = "";
                }
                if (borderColorImage != null)
                {
                    borderColorImage.color = AtavismSettings.Instance.ItemQualityColor(item.quality);
                }
            }


            if (this.TMPCountText != null && obj is AtavismInventoryItem)
            {
                AtavismInventoryItem item = (AtavismInventoryItem)obj;
                if (item.Count > 1)
                {
                    TMPCountText.text = item.Count.ToString();
                }
                else
                {
                    TMPCountText.text = "";
                }
                if (borderColorImage != null)
                {
                    borderColorImage.color = AtavismSettings.Instance.ItemQualityColor(item.quality);
                }
            }


            if (showCooldown)
            {
                RunCooldownUpdate();
            }
            else if (cooldownImage != null)
            {
                cooldownImage.fillAmount = 0;
                //StopCoroutine("UpdateCooldown");
            }
            RunCooldownUpdate();
            if (GetComponent<RectTransform>().anchorMin == new Vector2(0.5f, 0.5f) && GetComponent<RectTransform>().anchorMax == new Vector2(0.5f, 0.5f))
            {
                if (sizeFromParent)
                {
                    //                Debug.LogError("SetActivatable: set  sizeDelta"+ GetComponent<RectTransform>().sizeDelta+" "+ transform.parent.GetComponent<RectTransform>().sizeDelta);
                    if (transform.parent.GetComponent<RectTransform>().sizeDelta == Vector2.zero)
                    {
                        GetComponent<RectTransform>().sizeDelta = defaultSlotSize;
                    }
                    else
                    {
                        GetComponent<RectTransform>().sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
                    }
                }
            }
        }

        /// <summary>
        /// Tells the system to not run the Discard() function
        /// </summary>
        public void PreventDiscard()
        {
            this.preventDiscard = true;
        }

        public void SetDropTarget(UGUIDraggableSlot target)
        {
            releaseTarget = target;
        }

        public void SetLink(UGUIAtavismActivatable link)
        {
            this.link = link;
            if (link != null)
            {
                this.GetComponent<Button>().interactable = false;
                if (matGray != null)
                    icoImage.material = matGray;
            }
            else
            {
                this.GetComponent<Button>().interactable = true;
                if (matGray != null)
                    icoImage.material = null;
            }
        }

        public void ShowTooltip(GameObject target)
        {
            if (activatableObject is AtavismAbility)
            {
                AtavismAbility ability = (AtavismAbility)activatableObject;
                ability.ShowTooltip(target);
            }
            else if (activatableObject is AtavismInventoryItem)
            {
                AtavismInventoryItem item = (AtavismInventoryItem)activatableObject;
                item.ShowTooltip(target);
            }
        }

        public Activatable ActivatableObject
        {
            get
            {
                return activatableObject;
            }
        }

        public ActivatableType ActivatableType
        {
            get
            {
                return activatableType;
            }
            set
            {
                activatableType = value;
            }
        }

        public UGUIDraggableSlot Source
        {
            get
            {
                return source;
            }
        }

        public UGUIAtavismActivatable Link
        {
            get
            {
                return link;
            }
        }
    }
}