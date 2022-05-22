using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atavism
{

    public class UGUIRanking : MonoBehaviour
    {

        public UGUIPanelTitleBar titleBar;
        public UGUIRankingSlot prefab;
        public Transform grid;
        public List<UGUIRankingSlot> slots = new List<UGUIRankingSlot>();
        public List<UGUIRankingMenuSlot> menus = new List<UGUIRankingMenuSlot>();
        public UGUIRankingMenuSlot menuPrefab;
        public Transform menuGrid;
        bool showing = false;
        CanvasGroup _canvasGroup;
        void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (titleBar != null)
                titleBar.SetOnPanelClose(Hide);
            // AtavismEventSystem.RegisterEvent("ACHIEVEMENT_UPDATE", this);
            for (int i = 1; i <= 100; i++)
            {
                UGUIRankingSlot go = Instantiate(prefab, grid);
                slots.Add(go);
                go.gameObject.SetActive(false);
            }
            NetworkAPI.RegisterExtensionMessageHandler("ao.RANKING_UPDATE", handleRankingUpdate);
            NetworkAPI.RegisterExtensionMessageHandler("ao.RANKING_LIST", handleRankingListUpdate);
            Dictionary<string, object> props = new Dictionary<string, object>();
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.GET_RANKING_LIST", props);
            //SelectRanking(1);
            Hide();
         
        }

        private void OnDestroy()
        {
            NetworkAPI.RemoveExtensionMessageHandler("ao.RANKING_UPDATE", handleRankingUpdate);
            NetworkAPI.RemoveExtensionMessageHandler("ao.RANKING_LIST", handleRankingListUpdate);
        }

        private void handleRankingListUpdate(Dictionary<string, object> props)
        {

            int num = (int)props["num"];
            if (num > 0)
            {
                int a = 0;
                for (int i = 0; i < num; i++)
                {
                    string name = (string)props["name" + i];
                    string desc = (string)props["desc" + i];
                    int id = (int)props["id" + i];
                 //   Debug.LogError("handleRankingListUpdate id=" + id + " name=" + name + " desc=" + desc+" menucount="+menus.Count);
                    if (menus.Count <= i)
                    {
                      //  Debug.LogError("handleRankingListUpdate Instantiate "+i);
                        UGUIRankingMenuSlot go = Instantiate(menuPrefab, menuGrid);
                        menus.Add(go);
                        go.gameObject.SetActive(false);
                    }
               //     Debug.LogError("handleRankingListUpdate count=" + menus.Count);
#if AT_I2LOC_PRESET
             name = I2.Loc.LocalizationManager.GetTranslation(name);
#endif
#if AT_I2LOC_PRESET
             desc = I2.Loc.LocalizationManager.GetTranslation(desc);
#endif
                    menus[i].UpdateInfo(id, name, desc);
                    if (!menus[i].gameObject.activeSelf)
                        menus[i].gameObject.SetActive(true);
                    a++;
                }
                if (menus.Count > a)
                    for (int ii = a; ii < slots.Count; ii++)
                    if (menus[ii].gameObject.activeSelf)
                        menus[ii].gameObject.SetActive(false);
            }
            else
            {
                for (int ii = 0; ii < slots.Count; ii++)
                    if (menus.Count > 0)
                        if (menus[ii].gameObject.activeSelf)
                            menus[ii].gameObject.SetActive(false);
            }
            if (menus.Count > 0)
                menus[0].Select();
          //  Debug.LogError("handleRankingListUpdate EDN");
        }


        private void handleRankingUpdate(Dictionary<string, object> props)
        {

            int id = (int)props["id"];
            int num = (int)props["num"];
            if (num > 0)
            {
                int a = 0;
                for (int i = 0; i < num; i++)
                {
                    string name = (string)props["name" + i];
                    int value = (int)props["value" + i];
                    int pos = (int)props["pos" + i];
                    if (slots.Count <= i)
                    {
                        UGUIRankingSlot go = Instantiate(prefab, grid);
                        slots.Add(go);
                        go.gameObject.SetActive(false);
                    }
                    slots[i].UpdateInfo(pos, name, value);
                    if (!slots[i].gameObject.activeSelf)
                        slots[i].gameObject.SetActive(true);
                    a++;
                }
                if(slots.Count>a)
                for (int ii = a; ii < slots.Count; ii++)
                    if (slots[ii].gameObject.activeSelf)
                        slots[ii].gameObject.SetActive(false);
            }
            else
            {
                for (int ii = 0; ii < slots.Count; ii++)
                    if (slots[ii].gameObject.activeSelf)
                        slots[ii].gameObject.SetActive(false);
            }
            foreach (UGUIRankingMenuSlot sl in menus)
            {
                sl.checkSelected(id);
            }
        }  
        

        void UpdateDetails()
        {
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

        public void SelectRanking(int id)
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("id", id);
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.GET_RANKING", props);
        }
    }
}