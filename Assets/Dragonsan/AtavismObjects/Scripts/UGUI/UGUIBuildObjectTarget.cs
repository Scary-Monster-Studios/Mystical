using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUIBuildObjectTarget : MonoBehaviour, IDropHandler
    {

        public Text nameText;
        public TextMeshProUGUI TMPNameText;
        public Image portrait;
        public Slider healthBar;
        bool showing = false;

        // Use this for initialization
        void Start()
        {
            Hide();
            AtavismEventSystem.RegisterEvent("CLAIM_OBJECT_SELECTED", this);
            AtavismEventSystem.RegisterEvent("CLAIM_OBJECT_UPDATED", this);
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("CLAIM_OBJECT_SELECTED", this);
            AtavismEventSystem.UnregisterEvent("CLAIM_OBJECT_UPDATED", this);
        }

        void Show()
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            showing = true;
        }

        void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            showing = false;
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "CLAIM_OBJECT_SELECTED")
            {
                if (WorldBuilder.Instance.SelectedObject != null)
                {
                    Show();
                    UpdateClaimObject();
                }
                else
                {
                    Hide();
                }
            }
            else if (eData.eventType == "CLAIM_OBJECT_UPDATED")
            {
                if (WorldBuilder.Instance.SelectedObject != null && showing)
                {
                    UpdateClaimObject();
                }
                else
                {
                    Hide();
                }
            }
        }

        public void UpdateClaimObject()
        {
            ClaimObject cObject = WorldBuilder.Instance.SelectedObject;
            AtavismBuildObjectTemplate template = WorldBuilder.Instance.GetBuildObjectTemplate(cObject.TemplateID);

            if (nameText != null)
                nameText.text = template.buildObjectName;
            if (TMPNameText != null)
                TMPNameText.text = template.buildObjectName;

            if (portrait != null)
            {
                portrait.sprite = template.icon;
            }

            if (healthBar != null)
            {
                healthBar.value = (float)cObject.Health / (float)cObject.MaxHealth;
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            UGUIAtavismActivatable droppedActivatable = eventData.pointerDrag.GetComponent<UGUIAtavismActivatable>();

            // Reject any temporaries or bag slots
            if (droppedActivatable.Source.SlotBehaviour == DraggableBehaviour.Temporary || droppedActivatable.Link != null
                || droppedActivatable.ActivatableType != ActivatableType.Item)
            {
                return;
            }

            WorldBuilder.Instance.ImproveBuildObject(WorldBuilder.Instance.SelectedObject.gameObject,
                (AtavismInventoryItem)droppedActivatable.ActivatableObject, 1);

            droppedActivatable.PreventDiscard();
        }
    }
}