using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Atavism
{
    public class UGUILogoutPanel : MonoBehaviour
    {

        static UGUILogoutPanel instance;

        public Text logoutTimerText;
        public TextMeshProUGUI TMPLogoutTimerText;
        float logoutTime = -1;
        bool showing = false;
        public Text confirmationText;
        public TextMeshProUGUI TMPConfirmationText;
        object confirmationObject;
        ConfirmationResponse confirmationResponse;
        // float startTime;
        float endTime = -1;

        // Use this for initialization
        void Start()
        {
            if (instance != null)
            {
                GameObject.DestroyImmediate(gameObject);
                return;
            }
            instance = this;
            Hide();
            NetworkAPI.RegisterExtensionMessageHandler("logout_timer", HandleLogoutTimer);
        }

        void OnDestroy()
        {
            NetworkAPI.RemoveExtensionMessageHandler("logout_timer", HandleLogoutTimer);
        }

        // Update is called once per frame
        void Update()
        {
            if (showing)
            {
                int timeUntilLogout = (int)(logoutTime - Time.realtimeSinceStartup);
                if (TMPLogoutTimerText != null)
                    TMPLogoutTimerText.text = timeUntilLogout.ToString() + "s";
                if (logoutTimerText != null)
                    logoutTimerText.text = timeUntilLogout.ToString() + "s";
            }
            if (endTime != -1 && endTime > Time.time)
            {
                //  float total = endTime - startTime;
                float currentTime = endTime - Time.time;
                if (logoutTimerText != null)
                    logoutTimerText.text = (int)(currentTime) + "s";
                if (TMPLogoutTimerText != null)
                    TMPLogoutTimerText.text = (int)(currentTime) + "s";

            }
            else
            {
                if (showing)
                    Hide();
            }

            if (ClientAPI.GetPlayerObject() != null)
            {
                if (ClientAPI.GetPlayerObject().PropertyExists("combatstate"))
                    if ((bool)ClientAPI.GetPlayerObject().GetProperty("combatstate"))
                    {
                        if (showing)
                            Hide();
                    }
            }
        }

        public void Show()
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            showing = true;
            AtavismUIUtility.BringToFront(this.gameObject);
            transform.position = new Vector3((Screen.width / 2), (Screen.height / 2), 0);
        }

        public void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            showing = false;
        }

        public void CancelLogout()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.CANCEL_LOGOUT_REQUEST", props);
            if (confirmationResponse != null)
                confirmationResponse(confirmationObject, false);
            Hide();
        }

        public void HandleLogoutTimer(Dictionary<string, object> props)
        {
            int timer = (int)props["timer"];
            logoutTime = Time.realtimeSinceStartup + timer;
            Show();
        }
        public static UGUILogoutPanel Instance
        {
            get
            {
                return instance;
            }
        }
        public bool isShow()
        {
            return (GetComponent<CanvasGroup>().alpha == 1f);
        }
        public void ShowConfirmationBox(string message, object confirmObject, ConfirmationResponse responseMethod)
        {
            Show();
            //  startTime = Time.time;
            endTime = Time.time + 10f;
            if (confirmationText != null)
                confirmationText.text = message;
            if (TMPConfirmationText != null)
                TMPConfirmationText.text = message;
            this.confirmationObject = confirmObject;
            this.confirmationResponse = responseMethod;
        }
    }
}