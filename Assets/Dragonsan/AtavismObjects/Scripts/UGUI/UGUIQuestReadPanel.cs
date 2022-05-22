using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Atavism
{
    public class UGUIQuestReadPanel : MonoBehaviour, IPointerDownHandler
    {
        public GameObject questLogPanel;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            // Focus the window
            AtavismUIUtility.BringToFront(questLogPanel);
        }
    }
}