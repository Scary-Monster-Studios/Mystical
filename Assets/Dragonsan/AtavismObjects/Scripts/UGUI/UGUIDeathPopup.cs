using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Atavism
{
    public class UGUIDeathPopup : MonoBehaviour
    {

        bool dead = false;
        string state = "";

        // Use this for initialization
        void Start()
        {
            Hide();
            if (ClientAPI.GetPlayerObject() != null)
            {
                if (ClientAPI.GetPlayerObject().GameObject != null)
                {
                    if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>() != null)
                    {
                        ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler("deadstate", HandleDeadState);
                        ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler("state", HandleState);
                    }
                    else
                    {
                        Debug.LogError("UGUIDeathPopup: AtavismNode is null");
                    }
                }
                else
                {
                    Debug.LogError("UGUIDeathPopup: GameObject is null");
                }
            }
            else
            {
                Debug.LogError("UGUIDeathPopup: PlayerObject is null");
            }
            // The player may have changed scenes, but their stats were not sent back down, so let's take a look
            if (ClientAPI.GetPlayerObject() != null)
            {
                if (ClientAPI.GetPlayerObject().PropertyExists("deadstate"))
                {
                    dead = (bool)ClientAPI.GetPlayerObject().GetProperty("deadstate");
                }
                if (ClientAPI.GetPlayerObject().PropertyExists("state"))
                {
                    state = (string)ClientAPI.GetPlayerObject().GetProperty("state");
                }
            }

            UpdateShowState();
        }
        private void OnDestroy()
        {
            if (ClientAPI.GetPlayerObject() != null && ClientAPI.GetPlayerObject().GameObject != null && ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>() != null)
            {
                ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler("deadstate", HandleDeadState);
                ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler("state", HandleState);
            }
        }

        void Show()
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            AtavismUIUtility.BringToFront(this.gameObject);
            transform.position = new Vector3((Screen.width / 2), (Screen.height / 2), 0);

        }

        public void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        public void HandleDeadState(object sender, PropertyChangeEventArgs args)
        {
            //  Debug.LogError("UGUIDeathPopup: HandleDeadState");
            dead = (bool)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty("deadstate");
            UpdateShowState();
        }

        public void HandleState(object sender, PropertyChangeEventArgs args)
        {
            state = (string)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty("state");
            UpdateShowState();
        }

        public void UpdateShowState()
        {
            //  Debug.LogError("UGUIDeathPopup: UpdateShowState "+dead+" "+state);
            if (dead && state != "spirit")
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        public void ReleaseClicked()
        {
            NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/release");
        }

        public void ReleaseToSpiritClicked()
        {
            NetworkAPI.SendTargetedCommand(ClientAPI.GetPlayerOid(), "/releaseToSpirit");
        }
    }
}