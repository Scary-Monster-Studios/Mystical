using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{

    public class UGUIMailList : AtList<UGUIMailListEntry>
    {

        public UGUIMailBox mailBox;
        public UGUIPanelTitleBar titleBar;
        public UGUIMailRead mailReadPanel;
        public KeyCode toggleKey;
        bool showing = false;
        [SerializeField] bool hideMailListOnStartRead = true;

        void Awake()
        {
            Hide();
            AtavismEventSystem.RegisterEvent("SHOW_MAIL", this);
            AtavismEventSystem.RegisterEvent("MAIL_UPDATE", this);
            AtavismEventSystem.RegisterEvent("MAIL_SENT", this);
            AtavismEventSystem.RegisterEvent("CLOSE_MAIL_WINDOW", this);
            AtavismEventSystem.RegisterEvent("MAIL_SELECTED", this);
        }

        void Start()
        {
            if (titleBar != null)
                titleBar.SetOnPanelClose(this.Hide);
            if (mailBox == null)
                mailBox = GetComponentInParent<UGUIMailBox>();
            // Delete the old list
            ClearAllCells();

            Refresh();

            Mailing.Instance.RequestMailList();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("SHOW_MAIL", this);
            AtavismEventSystem.UnregisterEvent("MAIL_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("MAIL_SENT", this);
            AtavismEventSystem.UnregisterEvent("CLOSE_MAIL_WINDOW", this);
            AtavismEventSystem.UnregisterEvent("MAIL_SELECTED", this);
        }

        void OnEnable()
        {
            // Delete the old list
            if (titleBar != null)
                titleBar.SetOnPanelClose(this.Hide);
            ClearAllCells();

            Refresh();
        }

        public void Show()
        {
            showing = true;
            Mailing.Instance.RequestMailList();
            //  gameObject.SetActive(true);
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            // Delete the old list
            ClearAllCells();

            Refresh();
        }

        public void Hide()
        {
            showing = false;
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            //   gameObject.SetActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey) && !ClientAPI.UIHasFocus())
            {
                if (showing)
                {
                    Hide();
                }
                else
                {
                    Mailing.Instance.RequestMailList();
                    Show();
                }
            }
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "SHOW_MAIL")
            {
                mailBox.Show();
                string[] args = new string[1];
                AtavismEventSystem.DispatchEvent("CLOSE_READ_MAIL_WINDOW", args);
            }
            else if (eData.eventType == "MAIL_UPDATE")
            {
                if (!showing)
                    return;
                // Delete the old list
                ClearAllCells();
                Refresh();
            }
            else if (eData.eventType == "CLOSE_MAIL_WINDOW")
            {
                mailBox.Hide();
            }
            else if (eData.eventType == "MAIL_SELECTED")
            {
                if (hideMailListOnStartRead)
                    Hide();
                mailReadPanel.StartReadingMail();
                ClearAllCells();
                Refresh();
            }
        }

        #region implemented abstract members of AtList

        public override int NumberOfCells()
        {
            int numCells = Mailing.Instance.MailList.Count;
            return numCells;
        }

        public override void UpdateCell(int index, UGUIMailListEntry cell)
        {
            cell.SetMailEntryDetails(Mailing.Instance.GetMailEntry(index));
        }

        #endregion
    }
}