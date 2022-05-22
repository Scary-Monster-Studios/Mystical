using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace Atavism
{

    public class UGUIAchievementSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler/*, IPointerClickHandler*/
    {
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Status;
        public Slider progress;
        public Toggle check;

        public Color activeColor = Color.white;
        public Color noActiveColor = Color.gray;
        public Color activeTitleColor = Color.green;
        bool mouseEntered = false;
        AtavismAchievement achievement;
        // Start is called before the first frame update
        public void UpdateInfo(AtavismAchievement achievement)
        {
            this.achievement = achievement;

            if (Name != null)
            {
#if AT_I2LOC_PRESET
                    Name.text = I2.Loc.LocalizationManager.GetTranslation(achievement.name);
#else
                Name.text = achievement.name;
#endif
                if (achievement.active)
                {
                    if (ClientAPI.GetObjectNode(ClientAPI.GetPlayerOid()).PropertyExists("title") && ((string)ClientAPI.GetObjectNode(ClientAPI.GetPlayerOid()).GetProperty("title")).Equals(achievement.name))
                    {
                        Name.color = activeTitleColor;
                    }
                    else
                    {
                        Name.color = activeColor;
                    }
                }
                else
                {
                    Name.color = noActiveColor;
                }
            }
            else
            {
                Debug.LogError("Text componet is not assigned", this);
            }

            if(Status)
                Status.text = achievement.value + "/" + achievement.max;    
            else
                Debug.LogError("Text componet is not assigned", this);

            if (progress)
            {
                progress.minValue = 0;
                progress.maxValue = achievement.max;
                progress.value = achievement.value;
            }

        }
    
        /*    public void OnPointerClick(PointerEventData eventData)
        {
            Debug.LogError("OnPointerClick "+ eventData);
            if (achievement != null && achievement.id > 0)
            {
                Debug.LogError("OnPointerClick "+ achievement.id);
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("id", achievement.id);
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.SET_ACHIEVEMENTS_TITLE", props);
            }
        }*/

        public void Click()
        {
            if (achievement != null && achievement.id > 0)
            {
                // Debug.LogError("OnPointerClick " + achievement.id);
                ClientAPI.GetPlayerObject();
                Dictionary<string, object> props = new Dictionary<string, object>();
                if (ClientAPI.GetObjectNode(ClientAPI.GetPlayerOid()).PropertyExists("title") && ((string)ClientAPI.GetObjectNode(ClientAPI.GetPlayerOid()).GetProperty("title")).Equals(achievement.name))
                    props.Add("id", 0);
                else
                    props.Add("id", achievement.id);
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.SET_ACHIEVEMENTS_TITLE", props);
            }
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
          //  Debug.LogError("ShowTooltip");
            if (achievement == null)
                return;
#if AT_I2LOC_PRESET
            UGUITooltip.Instance.SetTitle(I2.Loc.LocalizationManager.GetTranslation(achievement.name));
#else
            UGUITooltip.Instance.SetTitle(achievement.name);
#endif
            UGUITooltip.Instance.HideType(true);
            UGUITooltip.Instance.HideWeight(true);
            UGUITooltip.Instance.EnableIcon(false);
            if (AtavismVip.Instance.GetMaxPoints > 0)
#if AT_I2LOC_PRESET
               UGUITooltip.Instance.AddAttribute(I2.Loc.LocalizationManager.GetTranslation("Progress"),achievement.value + " / " + achievement.max, true);
#else
                UGUITooltip.Instance.AddAttribute("Progress ", achievement.value + " / " + achievement.max, true);
#endif
            //  UGUITooltip.Instance.AddAttribute(AtavismVip.Instance.GetPoints+" / "+ AtavismVip.Instance.GetMaxPoints , "", true);
            UGUITooltip.Instance.SetDescription(achievement.desc);

            UGUITooltip.Instance.Show(gameObject);
        }
        void HideTooltip()
        {
            UGUITooltip.Instance.Hide();
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