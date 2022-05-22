using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class UGUIGameMenu : MonoBehaviour
    {

        bool showing = false;
        [SerializeField] GameObject settings;

        // Use this for initialization
        void Start()
        {
            Hide();

            NetworkAPI.RegisterExtensionMessageHandler("LOGOUT_STARTED", ClaimIDMessage);
        }

        public void ClaimIDMessage(Dictionary<string, object> props)
        {
            int status = (int)props["status"];
            Debug.Log("Got Logout: " + status);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && ClientAPI.GetTargetObject() == null)
            {
                if (settings != null && settings.activeSelf == true)
                {
                    settings.SetActive(false);
                }
                else
                {
                    if (UGUILogoutPanel.Instance.isShow())
                    {
                        UGUILogoutPanel.Instance.Hide();
                        Dictionary<string, object> props = new Dictionary<string, object>();
                        NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.CANCEL_LOGOUT_REQUEST", props);
                    }
                    else
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
            else if (Input.GetKeyDown(KeyCode.Escape) && ClientAPI.GetTargetObject() != null)
            {
                ClientAPI.ClearTarget();
            }
        }

        public void Show()
        {
            GetComponent<CanvasGroup>().alpha = 1.0f;
            GetComponent<CanvasGroup>().interactable = true;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            showing = true;
            if (Camera.main != null)
                Camera.main.gameObject.SendMessage("NoMove", true);
        }

        public void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().interactable = false;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            showing = false;
            if (Camera.main != null)
                Camera.main.gameObject.SendMessage("NoMove", false);
        }

        public void Logout()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.LOGOUT_REQUEST", props);
#if AT_I2LOC_PRESET
        UGUILogoutPanel.Instance.ShowConfirmationBox(I2.Loc.LocalizationManager.GetTranslation("Logging out will occur in"), null, CancelLogout);
#else
            UGUILogoutPanel.Instance.ShowConfirmationBox("Logging out will occur in", null, CancelLogout);
#endif
            Hide();
        }
        public void CancelLogout(object obj, bool accepted)
        {
            if (!accepted)
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.CANCEL_LOGOUT_REQUEST", props);
                Hide();
            }
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}