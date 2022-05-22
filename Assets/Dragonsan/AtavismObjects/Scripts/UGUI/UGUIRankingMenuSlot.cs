using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Atavism
{
    public class UGUIRankingMenuSlot : MonoBehaviour, IPointerClickHandler
    {
        public TextMeshProUGUI menuName;
        public Button button;
        public Color currentRankingColor = Color.green;
        public Color defaultRankingColor = Color.white;
        int id = -1;
        string desc = "";
        bool mouseEntered = false;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateInfo(int id, string name, string desc)
        {
            this.id = id;
            if (menuName)
                menuName.text = name;
            this.desc = desc;
        }
        public void checkSelected(int id)
        {
            if (menuName)
            {
                if (this.id.Equals(id))
                {
                    menuName.color = currentRankingColor;
                }
                else
                {
                    menuName.color = defaultRankingColor;
                }
            }
        } 
        public void Select()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("id", id);
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.GET_RANKING", props);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Select();
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseEntered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseEntered = false;
        }
        void ShowTooltip()
        {

            UGUIMiniTooltip.Instance.SetDescription(desc);
            UGUIMiniTooltip.Instance.Show(gameObject);
        }
        void HideTooltip()
        {
            UGUIMiniTooltip.Instance.Hide();
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
                if (mouseEntered)
                {
                    ShowTooltip();
                }
                else
                {
                    HideTooltip();
                }
            }
        }
    }
}