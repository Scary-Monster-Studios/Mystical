using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Atavism
{

    public class UGUIAbilitySlot : UGUIDraggableSlot
    {

        AtavismAbility ability;
        bool mouseEntered = false;
        public Image icon;
        // Use this for initialization
        void Start()
        {
            slotBehaviour = DraggableBehaviour.SourceOnly;
            if (icon == null)
                icon = GetComponent<Image>();
        }

        public void UpdateAbilityData(AtavismAbility ability)
        {
            this.ability = ability;
            if (ability == null)
            {
                if (uguiActivatable != null)
                {
                    Destroy(uguiActivatable.gameObject);
                }
            }
            else if (Abilities.Instance.PlayerAbilities.Contains(ability))
            {
                if (uguiActivatable == null)
                {
                    if (AtavismSettings.Instance.inventoryItemPrefab != null)
                        uguiActivatable = (UGUIAtavismActivatable)Instantiate(AtavismSettings.Instance.inventoryItemPrefab, transform, false);
                    else
                        uguiActivatable = (UGUIAtavismActivatable)Instantiate(Abilities.Instance.uguiAtavismAbilityPrefab, transform, false);
                    //    uguiActivatable.transform.SetParent(transform, false);
                    uguiActivatable.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                    if (uguiActivatable.GetComponent<RectTransform>().anchorMin == Vector2.zero && uguiActivatable.GetComponent<RectTransform>().anchorMax == Vector2.one)
                        uguiActivatable.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                }
                uguiActivatable.SetActivatable(ability, ActivatableType.Ability, this);
                // Set background Image - HACK to still show ability when it is being dragged
                if (icon != null)
                    icon.sprite = ability.icon;

            }
            else
            {
                if (icon != null)
                {
                    icon.sprite = ability.icon;
                }
            }
        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            MouseEntered = true;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            MouseEntered = false;
        }

        public override void OnDrop(PointerEventData eventData)
        {
            Debug.Log("On Drop");
            // Do nothing
        }

        public override void ClearChildSlot()
        {
            uguiActivatable = null;
        }

        public override void Discarded()
        {
        }


        public override void Activate()
        {
            if (ability == null)
                return;
            ability.Activate();
        }

        protected override void ShowTooltip()
        {
        }

        void HideTooltip()
        {
            UGUITooltip.Instance.Hide();
            if (cor != null)
                StopCoroutine(cor);
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
                if (mouseEntered && uguiActivatable != null)
                {
                    uguiActivatable.ShowTooltip(gameObject);
                    cor = StartCoroutine(CheckOver());
                }
                else
                {
                    HideTooltip();
                }
            }
        }
    }
}