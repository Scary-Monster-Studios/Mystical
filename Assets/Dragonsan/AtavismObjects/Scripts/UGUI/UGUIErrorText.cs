using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Atavism
{
    public class UGUIErrorText : MonoBehaviour
    {

        float stopDisplay;
        bool showing = false;
        TextMeshProUGUI TMPTextField;
        Text textField;
        // Use this for initialization
        void Start()
        {
            Hide();
            AtavismEventSystem.RegisterEvent("ERROR_MESSAGE", this);

            NetworkAPI.RegisterExtensionMessageHandler("error_message", HandleErrorMessage);
            NetworkAPI.RegisterExtensionMessageHandler("ability_error", HandleAbilityErrorMessage);
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("ERROR_MESSAGE", this);

            NetworkAPI.RemoveExtensionMessageHandler("error_message", HandleErrorMessage);
            NetworkAPI.RemoveExtensionMessageHandler("ability_error", HandleAbilityErrorMessage);
        }

        // Update is called once per frame
        void Update()
        {
            if (showing && Time.time > stopDisplay)
            {
                Hide();
            }
        }

        void Show(string message)
        {
#if AT_I2LOC_PRESET
        message = string.IsNullOrEmpty(I2.Loc.LocalizationManager.GetTranslation(message)) ? message : I2.Loc.LocalizationManager.GetTranslation(message);
#endif
            if (GetComponent<Text>() != null)
                GetComponent<Text>().text = message;
            if (GetComponent<TextMeshProUGUI>() != null)
                GetComponent<TextMeshProUGUI>().text = message;
            if (TMPTextField != null)
                TMPTextField.text = message;
            if (textField != null)
                textField.text = message;
            GetComponent<CanvasGroup>().alpha = 1f;
            stopDisplay = Time.time + 3;
            showing = true;
        }

        public void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            showing = false;
        }

        public void HandleErrorMessage(Dictionary<string, object> props)
        {
            string errorMessage = (string)props["ErrorText"];

            if (errorMessage == "NotEnoughCurrency")
            {
#if AT_I2LOC_PRESET
            errorMessage = I2.Loc.LocalizationManager.GetTranslation("NotEnoughCurrency");
#else
                errorMessage = "You do not have enough currency to perform that action";
#endif
            }
            else if (errorMessage == "cooldownNoEnd")
            {
#if AT_I2LOC_PRESET
             errorMessage =  I2.Loc.LocalizationManager.GetTranslation("cooldownNoEnd");
#else
                errorMessage = "Cooldown has not finished yet";
#endif
            }
            else if (errorMessage == "SocialPlayerOffline")
            {
#if AT_I2LOC_PRESET
             errorMessage =  I2.Loc.LocalizationManager.GetTranslation("Can not add Friend because is offline");
#else
                errorMessage = "Can not add Friend because is offline";
#endif
            }
            else if (errorMessage == "InstanceRequiresGuild")
            {
#if AT_I2LOC_PRESET
            errorMessage = I2.Loc.LocalizationManager.GetTranslation("You must be in a Guild to enter this Instance");
#else
                errorMessage = "You must be in a Guild to enter this Instance";
#endif
            }

#if AT_I2LOC_PRESET
        Show(I2.Loc.LocalizationManager.GetTranslation(errorMessage));
#else
            Show(errorMessage);
#endif
        }

        public void HandleAbilityErrorMessage(Dictionary<string, object> props)
        {
            string errorMessage = "";

            int messageType = (int)props["ErrorText"];
#if AT_I2LOC_PRESET
        		if (messageType == 1) {
			errorMessage = I2.Loc.LocalizationManager.GetTranslation("Invalid target");
		} else if (messageType == 2) {
			errorMessage = I2.Loc.LocalizationManager.GetTranslation("Target is too far away");
		} else if ( messageType == 3) {
			errorMessage = I2.Loc.LocalizationManager.GetTranslation("Target is too close");
		} else if ( messageType == 4) {
			errorMessage = I2.Loc.LocalizationManager.GetTranslation("You cannot perform that action yet");
		} else if ( messageType == 5) {
             string data = (string)props["data"];
			errorMessage = I2.Loc.LocalizationManager.GetTranslation("Not enough")+" "+I2.Loc.LocalizationManager.GetTranslation(data);
			
		} else if ( messageType == 6) {
			errorMessage = I2.Loc.LocalizationManager.GetTranslation("You do not have the required reagent");
		} else if ( messageType == 7) {
			errorMessage = I2.Loc.LocalizationManager.GetTranslation("You do not have the required tool");
		} else if ( messageType == 8) {
			errorMessage = I2.Loc.LocalizationManager.GetTranslation("You do not have the required ammo equipped");
		} else if ( messageType == 9) {
			errorMessage = I2.Loc.LocalizationManager.GetTranslation("You are not in the correct stance");
		} else if ( messageType == 10) {
			errorMessage = I2.Loc.LocalizationManager.GetTranslation("You do not have the required weapon equipped");
		} else if ( messageType == 11) {
			errorMessage = I2.Loc.LocalizationManager.GetTranslation("You do not have a shield equipped");
		} else if ( messageType == 12) {
			errorMessage = I2.Loc.LocalizationManager.GetTranslation("Not Enough Vigor");
		} else if ( messageType == 13) {
			errorMessage = I2.Loc.LocalizationManager.GetTranslation("You do not have the required effect");
		} else if ( messageType == 14) {
			errorMessage = I2.Loc.LocalizationManager.GetTranslation("You have no target");
		} else if ( messageType == 15) {
			errorMessage = I2.Loc.LocalizationManager.GetTranslation("You do not have the required weapon type equipped");
		} else if ( messageType == 16) {
			errorMessage = I2.Loc.LocalizationManager.GetTranslation("You cannot activate a passive ability");
		} else if ( messageType == 17) {
			errorMessage = I2.Loc.LocalizationManager.GetTranslation("Interrupted");
		} else if ( messageType == 18) {
			errorMessage = I2.Loc.LocalizationManager.GetTranslation("You cannot do that while you are dead");
		}else if ( messageType == 19) {
			errorMessage = I2.Loc.LocalizationManager.GetTranslation("You must be facing your target to use that ability");
		}
        

#else
            if (messageType == 1)
            {
                errorMessage = "Invalid target";
            }
            else if (messageType == 2)
            {
                errorMessage = "Target is too far away";
            }
            else if (messageType == 3)
            {
                errorMessage = "Target is too close";
            }
            else if (messageType == 4)
            {
                errorMessage = "You cannot perform that action yet";
            }
            else if (messageType == 5)
            {
                string data = (string)props["data"];
                errorMessage = "Not enough "+data;
               
            }
            else if (messageType == 6)
            {
                errorMessage = "You do not have the required reagent";
            }
            else if (messageType == 7)
            {
                errorMessage = "You do not have the required tool";
            }
            else if (messageType == 8)
            {
                errorMessage = "You do not have the required ammo equipped";
            }
            else if (messageType == 9)
            {
                errorMessage = "You are not in the correct stance";
            }
            else if (messageType == 10)
            {
                errorMessage = "You do not have the required weapon equipped";
            }
            else if (messageType == 11)
            {
                errorMessage = "You do not have a shield equipped";
            }
            else if (messageType == 12)
            {
                errorMessage = "Not Enough Vigor";
            }
            else if (messageType == 13)
            {
                errorMessage = "You do not have the required effect";
            }
            else if (messageType == 14)
            {
                errorMessage = "You have no target";
            }
            else if (messageType == 15)
            {
                errorMessage = "You do not have the required weapon type equipped";
            }
            else if (messageType == 16)
            {
                errorMessage = "You cannot activate a passive ability";
            }
            else if (messageType == 17)
            {
                errorMessage = "Interrupted";
            }
            else if (messageType == 18)
            {
                errorMessage = "You cannot do that while you are dead";
            }
            else if (messageType == 19)
            {
                errorMessage = "You must be facing your target to use that ability";
            }
#endif
            Show(errorMessage);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "ERROR_MESSAGE")
            {
                Show(eData.eventArgs[0]);
            }
        }
    }
}