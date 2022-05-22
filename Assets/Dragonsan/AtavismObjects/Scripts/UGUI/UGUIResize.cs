using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Atavism
{
    public class UGUIResize : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {

        public Vector2 minSize = new Vector2(100, 100);
        public Vector2 maxSize = new Vector2(400, 400);
        public RectTransform panelToResize;
        // private RectTransform panelRectTransform;
        private Vector2 originalLocalPointerPosition;
        private Vector2 originalSizeDelta;
        [SerializeField]
        bool resizeUp = false;

        public void OnPointerDown(PointerEventData data)
        {
            originalSizeDelta = panelToResize.sizeDelta;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(panelToResize, data.position, data.pressEventCamera, out originalLocalPointerPosition);
        }
        public void OnBeginDrag(PointerEventData data)
        {
        }
        public void OnDrag(PointerEventData data)
        {
            if (panelToResize == null)
                return;
            Vector2 localPointerPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(panelToResize, data.position, data.pressEventCamera, out localPointerPosition);
            Vector3 offsetToOriginal = resizeUp ? originalLocalPointerPosition - localPointerPosition : localPointerPosition - originalLocalPointerPosition;

            Vector2 sizeDelta = originalSizeDelta + new Vector2(-offsetToOriginal.x, -offsetToOriginal.y);
            sizeDelta = new Vector2(
              Mathf.Clamp(sizeDelta.x, minSize.x, maxSize.x),
              Mathf.Clamp(sizeDelta.y, minSize.y, maxSize.y)
          );

            panelToResize.sizeDelta = sizeDelta;
        }
        public void OnEndDrag(PointerEventData data)
        {
        }
    }

}