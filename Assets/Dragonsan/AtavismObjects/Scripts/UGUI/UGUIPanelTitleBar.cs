using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;

namespace Atavism
{
    public delegate void OnPanelClose();

    public class UGUIPanelTitleBar : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {

        public Text titleText;
        public TextMeshProUGUI TMPTitleText;
        public bool draggable = false;
        OnPanelClose closeFunction = null;
        public Transform moveDraggedObject;

        // Use this for initialization
        void Start()
        {
            if (moveDraggedObject == null)
                moveDraggedObject = this.transform.parent;

        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            AtavismUIUtility.BringToFront(moveDraggedObject.gameObject);
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (draggable)
            {
                Vector3 addPosition = Vector3.zero;
                if (transform.parent != moveDraggedObject)
                    addPosition = transform.parent.localPosition;

                //            moveDraggedObject.position = new Vector3(eventData.position.x, eventData.position.y, 0) -  this.transform.position;
                moveDraggedObject.position = new Vector3(eventData.position.x, eventData.position.y, 0) - (addPosition + this.transform.localPosition);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {

        }
        public void SetPanelTitle(string text)
        {
            if (titleText != null)
                titleText.text = text;
            if (TMPTitleText != null)
                TMPTitleText.text = text;
        }

        public void SetOnPanelClose(OnPanelClose closeFunction)
        {
            this.closeFunction = closeFunction;
        }

        public void Close()
        {
            if (closeFunction != null)
            {
                closeFunction();
            }
            else
            {
                transform.parent.gameObject.SetActive(false);
            }
        }
    }
}