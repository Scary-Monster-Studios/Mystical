using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Atavism
{

    public class bl_MiniMapTexture : MonoBehaviour, IPointerClickHandler
    {
        private RectTransform Area;
        private bl_MiniMap MiniMap;

        /// <summary>
        /// 
        /// </summary>
        void Awake()
        {
            Area = GetComponent<RectTransform>();
            MiniMap = AtavismSettings.Instance.MiniMap;
                //transform.root.GetComponentInChildren<bl_MiniMap>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(MiniMap==null)
                MiniMap = AtavismSettings.Instance.MiniMap;
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(Area, eventData.pressPosition, eventData.pressEventCamera, out localPoint))
            {
                localPoint.x = (localPoint.x / Area.rect.width);
                localPoint.y = (localPoint.y / Area.rect.height);

                Vector2 absolutePosition = new Vector3(localPoint.x * 2, localPoint.y * 2);

                Vector2 ms = MiniMap.MMCamera.pixelRect.size * 0.5f;
                Vector3 RelativeScreen = new Vector2(absolutePosition.x * ms.x + ms.x, absolutePosition.y * ms.y + ms.y);
                SpawnPointer(RelativeScreen);
            }
        }

        void SpawnPointer(Vector2 position)
        {
            Ray ray = MiniMap.MMCamera.ScreenPointToRay(position);
            RaycastHit raycast;
            if (Physics.Raycast(ray, out raycast))
            {
                MiniMap.SetPointMark(raycast.point);
            }
        }
    }
}