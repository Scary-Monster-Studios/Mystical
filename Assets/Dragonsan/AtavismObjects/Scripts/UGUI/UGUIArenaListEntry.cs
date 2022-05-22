using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

namespace Atavism
{
    public class UGUIArenaListEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        public TextMeshProUGUI arenaTitleText;
        //public Toggle SelectedToggle;
        public TextMeshProUGUI arenaLevelText;
        public TextMeshProUGUI arenaTimeText;
        public Image select;
        public Image queue;
        public Image hover;
        [SerializeField] Color normalColorText = Color.black;
        [SerializeField] Color reqFailedColorText = Color.red;

        ArenaEntry entry;
        int arenaPos;
        UGUIArenaList arenaList;
        bool selected = false;

        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("UPDATE_LANGUAGE", this);
        }
        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("UPDATE_LANGUAGE", this);

        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            MouseEntered = true;
            if (hover != null)
                hover.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MouseEntered = false;
            if (hover != null)
                hover.enabled = false;
        }
        /// <summary>
        /// Function 
        /// </summary>
        public void ArenaEntryClicked()
        {
            Arena.Instance.ArenaEntrySelected(arenaPos);
            arenaList.SetArenaDetails();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eData"></param>
        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "UPDATE_LANGUAGE")
            {
                if (this.entry != null)
                    UpdateDisplay();
            }
        }

        public void SetArenaEntryDetails(ArenaEntry entry, int pos, UGUIArenaList arenaList)
        {
            this.entry = entry;
            this.arenaPos = pos;
            this.arenaList = arenaList;
            if (entry == null)
                Debug.LogError("No Arena Entity");
            UpdateDisplay();

        }
        private void UpdateDisplay()
        {
            if (GetComponent<Image>() != null)
            {
                ArenaEntry selectedArena;
                selectedArena = Arena.Instance.GetSelectedArenaEntry();
                if (selectedArena != null && entry != null)
                    if (selectedArena.ArenaId == entry.ArenaId)
                    {
                        select.enabled = true;
                        selected = true;
                    }
                    else
                    {
                        select.enabled = false;
                        selected = false;
                    }
            }
            if (queue != null)
                if (entry.ArenaQueued)
                    queue.enabled = true;
                else
                    queue.enabled = false;
            DateTime arenaStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, entry.StartHour, entry.StartMin, 0);
            DateTime arenaEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, entry.EndHour, entry.EndMin, 0);
            DateTime timeNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, TimeManager.Instance.Hour, TimeManager.Instance.Minute, 0);
#if AT_I2LOC_PRESET
        this.arenaTitleText.text = string.IsNullOrEmpty(I2.Loc.LocalizationManager.GetTranslation("Arena/" + entry.ArenaName)) ? entry.ArenaName/* + " " + (entry.ArenaQueued ? I2.Loc.LocalizationManager.GetTranslation("Arena/Queued") : "") */: I2.Loc.LocalizationManager.GetTranslation("Arena/" + entry.ArenaName /*+ " " + (entry.ArenaQueued ? I2.Loc.LocalizationManager.GetTranslation("Arena/Queued") : "")*/);
#else
            this.arenaTitleText.text = entry.ArenaName;// + " " + (entry.ArenaQueued ? "Queued" : "") ;
#endif

            this.arenaTimeText.text = entry.StartHour + ":" + (entry.StartMin < 10 ? "0" + entry.StartMin.ToString() : entry.StartMin.ToString()) + "-" + entry.EndHour + ":" + (entry.EndMin < 10 ? "0" + entry.EndMin.ToString() : entry.EndMin.ToString());
            this.arenaLevelText.text = "[" + entry.ReqLeval + (entry.MaxLeval - entry.ReqLeval > 0 ? "-" + entry.MaxLeval : "") + "] ";
            if (ClientAPI.GetPlayerObject().PropertyExists("level"))
            {
                if ((int)ClientAPI.GetPlayerObject().GetProperty("level") >= entry.ReqLeval && (int)ClientAPI.GetPlayerObject().GetProperty("level") <= entry.MaxLeval)
                {
                    this.arenaLevelText.color = normalColorText;
                    //  if (DateTime.UtcNow >= arenaStart && DateTime.UtcNow <= arenaEnd)
                    if (entry.StartHour == 0 && entry.StartMin == 0 && entry.EndHour == 0 && entry.EndMin == 0)
                    {
                        this.arenaTimeText.color = normalColorText;
                        this.arenaTitleText.color = normalColorText;
                    }
                    else if (timeNow >= arenaStart && timeNow <= arenaEnd)
                    {
                        this.arenaTimeText.color = normalColorText;
                        this.arenaTitleText.color = normalColorText;
                    }
                    else
                    {
                        this.arenaTimeText.color = reqFailedColorText;
                        //   this.arenaLevelText.color = Color.red;
                        this.arenaTitleText.color = reqFailedColorText;
                    }
                }
                else
                {
                    //   if (DateTime.UtcNow >= arenaStart && DateTime.UtcNow <= arenaEnd)
                    if (entry.StartHour == 0 && entry.StartMin == 0 && entry.EndHour == 0 && entry.EndMin == 0)
                    {
                        this.arenaTimeText.color = normalColorText;
                    }
                    else  if (timeNow >= arenaStart && timeNow <= arenaEnd)
                        this.arenaTimeText.color = normalColorText;
                    else
                        this.arenaTimeText.color = reqFailedColorText;
                    this.arenaTitleText.color = reqFailedColorText;
                    this.arenaLevelText.color = reqFailedColorText;
                }
            }

        }



        void HideTooltip()
        {
            UGUITooltip.Instance.Hide();
        }

        public bool MouseEntered
        {
            set
            {

            }
        }
    }
}