using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Atavism
{

    /// <summary>
    /// Allows players to upgrade their Build Objects by dropping their items in the rect this is added to
    /// </summary>
    public class UGUIEditObjectDropPanel : MonoBehaviour, IDropHandler
    {

        // Use this for initialization
        void Start()
        {

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