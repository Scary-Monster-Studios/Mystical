using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Atavism
{

    public class UGUIAchievement : MonoBehaviour
    {
        public UGUIPanelTitleBar titleBar;
        public UGUIAchievementSlot prefab;
        public Transform grid;
        public List<UGUIAchievementSlot> slots = new List<UGUIAchievementSlot>();
        bool showing = false;
        CanvasGroup _canvasGroup;
        // Start is called before the first frame update
        void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (titleBar != null)
                titleBar.SetOnPanelClose(Hide);
            AtavismEventSystem.RegisterEvent("ACHIEVEMENT_UPDATE", this);
            for (int i = 0; i < 100; i++)
            {
                UGUIAchievementSlot go = Instantiate(prefab, grid);
                slots.Add(go);
                go.gameObject.SetActive(false);
            }
            AtavismAchievements.Instance.GetAchievementStatus();
            if (ClientAPI.GetObjectNode(ClientAPI.GetPlayerOid()) != null)
            {
                ClientAPI.GetObjectNode(ClientAPI.GetPlayerOid()).RegisterPropertyChangeHandler("title", titleHandler);
            }
                Hide();
        }
        private void OnDestroy()
        {
            if (ClientAPI.GetObjectNode(ClientAPI.GetPlayerOid()) != null)
            {
                ClientAPI.GetObjectNode(ClientAPI.GetPlayerOid()).RemovePropertyChangeHandler("title", titleHandler);
            }
            AtavismEventSystem.UnregisterEvent("ACHIEVEMENT_UPDATE", this);
        }

        public void titleHandler(object sender, PropertyChangeEventArgs args)
        {
            UpdateDetails();
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "ACHIEVEMENT_UPDATE")
            {
                UpdateDetails();
            }
        }

        void UpdateDetails()
        {
            int i = 0;
            foreach (AtavismAchievement aa in AtavismAchievements.Instance.achivments)
            {
                if (slots.Count < i)
                {
                    UGUIAchievementSlot go = Instantiate(prefab, grid);
                    slots.Add(go);
                    go.gameObject.SetActive(false);
                }
                slots[i].UpdateInfo(aa);
                if (!slots[i].gameObject.activeSelf)
                    slots[i].gameObject.SetActive(true);
                i++;
            }
            for (int ii = i; ii < slots.Count; ii++)
                if(slots[ii].gameObject.activeSelf)
                slots[ii].gameObject.SetActive(false);
        }

        public void Show()
        {
            AtavismUIUtility.BringToFront(gameObject);
            AtavismSocial.Instance.SendGetFriends();
            UpdateDetails();
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.interactable = true;
            }
            showing = true;
        }

        public void Hide()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.blocksRaycasts = false;
            }
            showing = false;
        }

        public void Toggle()
        {
            if (showing)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
      
     
    }
}