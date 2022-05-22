using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUIPortrait : MonoBehaviour, IPointerClickHandler
    {

        public TextMeshProUGUI TMPName;
        public Text Name;
        public Text levelText;
        public TextMeshProUGUI TMPLevelText;
        public Image portrait;
        public RectTransform popupMenu;
        public Image leaderIcon;
        public int premiumCurrency = 4;
        string characterName;
        //	string gender;
        //	int classID = -1;
        string level;
        [SerializeField] UGUICurrency currencyPrem;

        // Use this for initialization
        void Start()
        {
            //	ClientAPI.GetPlayerObject().RegisterPropertyChangeHandler("gender", GenderHandler);
            //	ClientAPI.GetPlayerObject().RegisterPropertyChangeHandler("aspect", ClassHandler);
            ClientAPI.GetPlayerObject().RegisterPropertyChangeHandler("level", LevelHandler);
            if (ClientAPI.GetPlayerObject() != null)
            {
                /*		if (ClientAPI.GetPlayerObject().PropertyExists("gender")) {
                            gender = (string)ClientAPI.GetPlayerObject().GetProperty("gender");
                        }*/
                /*		if (ClientAPI.GetPlayerObject().PropertyExists("aspect")) {
                            classID = (int)ClientAPI.GetPlayerObject().GetProperty("aspect");
                        }*/
                if (ClientAPI.GetPlayerObject().PropertyExists("level"))
                {
                    level = "" + (int)ClientAPI.GetPlayerObject().GetProperty("level");
                }
                UpdatePortrait();
            }
            AtavismEventSystem.RegisterEvent("CURRENCY_UPDATE", this);
            AtavismEventSystem.RegisterEvent("CURRENCY_ICON_UPDATE", this);
            UpdateCurrencies();
            AtavismEventSystem.RegisterEvent("GROUP_UPDATE", this);
            if (leaderIcon != null)
                leaderIcon.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            if (ClientAPI.GetPlayerObject() != null)
            {

                //    ClientAPI.GetPlayerObject().RemovePropertyChangeHandler("gender", GenderHandler);
                //   ClientAPI.GetPlayerObject().RemovePropertyChangeHandler("aspect", ClassHandler);
                ClientAPI.GetPlayerObject().RemovePropertyChangeHandler("level", LevelHandler);
            }
            AtavismEventSystem.UnregisterEvent("GROUP_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("CURRENCY_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("CURRENCY_ICON_UPDATE", this);
        }

        /* public void GenderHandler(object sender, PropertyChangeEventArgs args) {
             gender = (string)ClientAPI.GetPlayerObject().GetProperty("gender");
             UpdatePortrait();
         }*/

        /*	public void ClassHandler(object sender, PropertyChangeEventArgs args) {
                classID = (int)ClientAPI.GetPlayerObject().GetProperty("aspect");
                UpdatePortrait();
            }*/

        public void LevelHandler(object sender, PropertyChangeEventArgs args)
        {
            level = "" + (int)ClientAPI.GetPlayerObject().GetProperty("level");
            UpdatePortrait();
        }

        public void UpdatePortrait()
        {
            if (Name != null)
                Name.text = ClientAPI.GetPlayerObject().Name;
            if (TMPName != null)
                TMPName.text = ClientAPI.GetPlayerObject().Name;
            if (levelText != null)
                levelText.text = level;
            if (TMPLevelText != null)
                TMPLevelText.text = level;
            if (portrait != null)
            {
                Sprite portraitSprite = PortraitManager.Instance.LoadPortrait(ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>());
                if (portraitSprite == null)
                {
                    portraitSprite = ClientAPI.GetPlayerObject().PropertyExists("portrait") ?
                             PortraitManager.Instance.LoadPortrait((string)ClientAPI.GetPlayerObject().GetProperty("portrait")) :
                             ClientAPI.GetPlayerObject().PropertyExists("custom:portrait") ?
                             PortraitManager.Instance.LoadPortrait((string)ClientAPI.GetPlayerObject().GetProperty("custom:portrait")) : null;
                }

                if (portraitSprite != null)
                    portrait.sprite = portraitSprite;
            }

            if (leaderIcon != null)
            {
                if (AtavismGroup.Instance.LeaderOid != null && AtavismGroup.Instance.LeaderOid.ToLong() == ClientAPI.GetPlayerOid())
                {
                    leaderIcon.gameObject.SetActive(true);
                }
                else
                {
                    leaderIcon.gameObject.SetActive(false);
                }
            }

        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "GROUP_UPDATE")
            {
                // Update 
                UpdatePortrait();
            }
            if (eData.eventType == "CURRENCY_UPDATE"||eData.eventType == "CURRENCY_ICON_UPDATE")
            {
                UpdateCurrencies();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                ClientAPI.SetTarget(ClientAPI.GetPlayerOid());
                return;
            }
            if (eventData.button != PointerEventData.InputButton.Right)
                return;

            if (popupMenu.gameObject.activeSelf)
            {
                popupMenu.gameObject.SetActive(false);
                AtavismSettings.Instance.DsContextMenu(null);
                return;
            }

            // Verify the player is in a group
            if (AtavismGroup.Instance.Members == null || AtavismGroup.Instance.Members.Count == 0)
                return;

            // Work out what to put in the popup menu here
            popupMenu.gameObject.SetActive(true);
            AtavismSettings.Instance.DsContextMenu(popupMenu.gameObject);

        }

        public void LeaveGroup()
        {
            AtavismGroup.Instance.LeaveGroup();
            popupMenu.gameObject.SetActive(false);
            AtavismSettings.Instance.DsContextMenu(null);
        }
        void UpdateCurrencies()
        {
            if (currencyPrem != null)
                currencyPrem.UpdateCurrency(Inventory.Instance.GetCurrency(premiumCurrency));
        }

    }
}