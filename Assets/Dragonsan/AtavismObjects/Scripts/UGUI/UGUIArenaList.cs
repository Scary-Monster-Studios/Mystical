using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Atavism
{
    public class UGUIArenaList : AtList<UGUIArenaListEntry>
    {

        public UGUIPanelTitleBar titleBar;
        [SerializeField] Button joinGroupButton;
        [SerializeField] Button joinSoloButton;
        [SerializeField] Button leaveButton;
        [SerializeField] TextMeshProUGUI instanceName;
        [SerializeField] TextMeshProUGUI instanceArenaType;
        [SerializeField] TextMeshProUGUI instanceTime;
        [SerializeField] TextMeshProUGUI instanceLevel;
        [SerializeField] TextMeshProUGUI instanceDesc;
        public Color normalColorText = Color.black;
        public Color FailedColorText = Color.red;
        ArenaEntry selectedArena;
        public Scrollbar arenaListScroll;
        bool showing = false;
        // int selected = -1;
        void Awake()
        {
            AtavismEventSystem.RegisterEvent("UPDATE_LANGUAGE", this);
            AtavismEventSystem.RegisterEvent("ARENA_LIST_UPDATE", this);
            if (titleBar != null)
                titleBar.SetOnPanelClose(Hide);
            Hide();
        }

        void Start()
        {
            ClearAllCells();
            Refresh();
        }

        void OnEnable()
        {
            ClearAllCells();
            Refresh();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("ARENA_LIST_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("UPDATE_LANGUAGE", this);
        }

        public void Show()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("playerOid", ClientAPI.GetPlayerOid());
            props.Add("cat", 1);
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "arena.getList", props);
            showing = true;
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            AtavismUIUtility.BringToFront(gameObject);

            // Delete the old list
            ClearAllCells();

            Refresh();
            if (arenaListScroll != null)
                arenaListScroll.value = 1;
            //  gameObject.SetActive(true);
        }

        public void Hide()
        {
            // gameObject.SetActive(false);
            showing = false;
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            Arena.Instance.ArenaEntrySelected(0);
        }

        void Update()
        {
            if (Input.GetKeyDown(AtavismSettings.Instance.GetKeySettings().arena) && !ClientAPI.UIHasFocus())
            {
                if (showing)
                    Hide();
                else
                    Show();
            }
        }

        public void Toggle()
        {
            if (showing)
                Hide();
            else
                Show();
        }

        /// <summary>
        /// Function run after get message from atavism evant system
        /// </summary>
        /// <param name="eData"></param>
        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "ARENA_LIST_UPDATE" || eData.eventType == "UPDATE_LANGUAGE")
            {
                // Delete the old list
                ClearAllCells();
                Refresh();
                SetArenaDetails();
            }
        }


        public void SetArenaDetails()
        {
            ClearAllCells();
            Refresh();
            int level = 1;
            if (ClientAPI.GetPlayerObject() != null)
                if (ClientAPI.GetPlayerObject().PropertyExists("level"))
                {
                    level = (int)ClientAPI.GetPlayerObject().GetProperty("level");
                }
            // ArenaEntry selectedArena;
            selectedArena = Arena.Instance.GetSelectedArenaEntry();
            if (selectedArena == null)
                return;
            DateTime arenaStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, selectedArena.StartHour, selectedArena.StartMin, 0);
            DateTime arenaEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, selectedArena.EndHour, selectedArena.EndMin, 0);
            DateTime timeNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, TimeManager.Instance.Hour, TimeManager.Instance.Minute, 0);
            Color color = normalColorText;
           //   Debug.LogError(selectedArena.ArenaName +" " + selectedArena.StartHour + "     |     " + selectedArena.StartMin + "    |    " + selectedArena.EndHour+"    |    "+ selectedArena.EndMin);
            // if (DateTime.UtcNow > arenaStart && DateTime.UtcNow < arenaEnd)
            if (selectedArena.StartHour == 0 && selectedArena.StartMin == 0 && selectedArena.EndHour == 0 && selectedArena.EndMin == 0)
            {
                color = normalColorText;
            }
            else if (timeNow > arenaStart && timeNow < arenaEnd)
            {
                color = normalColorText;
            }
            else
            {
                color = FailedColorText;
            }

            if (selectedArena.ArenaQueued)
            {
                joinGroupButton.gameObject.SetActive(false);
                joinSoloButton.gameObject.SetActive(false);
                leaveButton.gameObject.SetActive(true);
            }
            else
            {
                if (selectedArena.StartHour == 0 && selectedArena.StartMin == 0 && selectedArena.EndHour == 0 && selectedArena.EndMin == 0 && (level <= selectedArena.MaxLeval && level >= selectedArena.ReqLeval))
                {
                    if (selectedArena.teamSize[0] > 1 && AtavismGroup.Instance.Members.Count > 0)
                        joinGroupButton.gameObject.SetActive(true);
                    else
                        joinGroupButton.gameObject.SetActive(false);
                    joinSoloButton.gameObject.SetActive(true);
                }
                else if ((timeNow > arenaStart && timeNow < arenaEnd) && (level <= selectedArena.MaxLeval && level >= selectedArena.ReqLeval))
                {
                    if (selectedArena.teamSize[0] > 1 && AtavismGroup.Instance.Members.Count > 0)
                        joinGroupButton.gameObject.SetActive(true);
                    else
                        joinGroupButton.gameObject.SetActive(false);
                    joinSoloButton.gameObject.SetActive(true);
                }
                else
                {
                    joinGroupButton.gameObject.SetActive(false);
                    joinSoloButton.gameObject.SetActive(false);
                }
                leaveButton.gameObject.SetActive(false);

            }
#if AT_I2LOC_PRESET
        this.instanceName.text = I2.Loc.LocalizationManager.GetTranslation("Arena/InstanceName")+": "+( string.IsNullOrEmpty(I2.Loc.LocalizationManager.GetTranslation("Arena/" + selectedArena.ArenaName)) ? selectedArena.ArenaName : I2.Loc.LocalizationManager.GetTranslation("Arena/" + selectedArena.ArenaName));
        this.instanceArenaType.text = I2.Loc.LocalizationManager.GetTranslation("Arena/InstanceType")+": " + (selectedArena.ArenaType==1? string.IsNullOrEmpty(I2.Loc.LocalizationManager.GetTranslation("Arena/DEATHMATCH_ARENA")) ? "Deathmatch" : I2.Loc.LocalizationManager.GetTranslation("Arena/DEATHMATCH_ARENA"): string.IsNullOrEmpty(I2.Loc.LocalizationManager.GetTranslation("Arena/CTF_ARENA")) ? "Capture the flag" : I2.Loc.LocalizationManager.GetTranslation("Arena/CTF_ARENA"));
        this.instanceTime.text = (string.IsNullOrEmpty(I2.Loc.LocalizationManager.GetTranslation("Arena/available")) ? "Available:" : I2.Loc.LocalizationManager.GetTranslation("Arena/available")+":")+ "<color=" + ColorTypeConverter.ToRGBHex(color) + ">"+" " +selectedArena.StartHour + ":" + (selectedArena.StartMin < 10 ? "0" + selectedArena.StartMin.ToString() : selectedArena.StartMin.ToString()) + " - " + selectedArena.EndHour + ":" + (selectedArena.EndMin < 10 ? "0" + selectedArena.EndMin.ToString() : selectedArena.EndMin.ToString()) + "</color>";
        this.instanceDesc.text = string.IsNullOrEmpty(I2.Loc.LocalizationManager.GetTranslation("Arena/" + selectedArena.Description)) ? selectedArena.Description : I2.Loc.LocalizationManager.GetTranslation("Arena/" + selectedArena.Description);
        this.instanceLevel.text = "";
#else
            this.instanceName.text = "Instance Name: " + selectedArena.ArenaName;
            this.instanceArenaType.text = "Instance Type: " + (selectedArena.ArenaType == 1 ? "Deathmatch" : "Capture the flag");
            this.instanceTime.text = "Available: " + "<color=" + ColorTypeConverter.ToRGBHex(color) + ">" + " " + selectedArena.StartHour + ":" + (selectedArena.StartMin < 10 ? "0" + selectedArena.StartMin.ToString() : selectedArena.StartMin.ToString()) + " - " + selectedArena.EndHour + ":" + (selectedArena.EndMin < 10 ? "0" + selectedArena.EndMin.ToString() : selectedArena.EndMin.ToString()) + "</color>";
            this.instanceDesc.text = selectedArena.Description;
            this.instanceLevel.text = "";
#endif

        }
        /*
        public void ActiveQuests()
        {
            ClearAllCells();
            Refresh();
         }
         */
        /// <summary>
        /// Function send message to server for leave from queue and refresh list
        /// </summary>
        /// <param name="item"></param>
        /// <param name="accepted"></param>
        void LeaveQueue(object item, bool accepted)
        {
            if (accepted)
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                props.Add("arenaType", selectedArena.ArenaType);
                props.Add("arenaTemp", selectedArena.ArenaId);
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "arena.leaveQueue", props);
                ClearAllCells();
                Refresh();
            }
        }

        public void JoinQueueSolo()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("arenaType", selectedArena.ArenaType);
            props.Add("arenaTemp", selectedArena.ArenaId);
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "arena.joinQueue", props);
        }
        public void JoinQueueGroup()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("arenaType", selectedArena.ArenaType);
            props.Add("arenaTemp", selectedArena.ArenaId);
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "arena.groupJoinQueue", props);
        }
        /// <summary>
        /// from click Function show confirmation leave queue
        /// </summary>
        public void LeaveQueue()
        {
#if AT_I2LOC_PRESET
        UGUIConfirmationPanel.Instance.ShowConfirmationBox(I2.Loc.LocalizationManager.GetTranslation("Are you sure you want abandon queue") + " " + I2.Loc.LocalizationManager.GetTranslation("Arena/" + selectedArena.ArenaName) + "?", null, LeaveQueue);
#else
            UGUIConfirmationPanel.Instance.ShowConfirmationBox("Are you sure you want abandon queue " + selectedArena.ArenaName + "?", null, LeaveQueue);
#endif

        }


        #region implemented abstract members of AtList

        public override int NumberOfCells()
        {
            int numCells;
            numCells = Arena.Instance.ArenaEntries.Count;
            return numCells;
        }

        public override void UpdateCell(int index, UGUIArenaListEntry cell)
        {
            cell.SetArenaEntryDetails(Arena.Instance.ArenaEntries[index], index, this);
        }

        #endregion
    }
}