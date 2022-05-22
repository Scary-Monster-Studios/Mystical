using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace Atavism
{

    public class UGUIGearModification : MonoBehaviour
    {
        [AtavismSeparator("Menu")]
        [SerializeField] TextMeshProUGUI socketingText;
        [SerializeField] Image socketingImage;
        [SerializeField] TextMeshProUGUI resetSocketsText;
        [SerializeField] Image resetSocketsImage;
        [SerializeField] TextMeshProUGUI enchantingText;
        [SerializeField] Image enchantingImage;
        [SerializeField] Color selectedColor = Color.green;
        [SerializeField] Color defauiltColor = Color.white;
        [SerializeField] Color selectedImageColor = Color.green;
        [SerializeField] Color defauiltImageColor = Color.white;

        [AtavismSeparator("Panels")]
        [SerializeField] GameObject socketPanel;
        [SerializeField] GameObject resetSocketPanel;
        [SerializeField] GameObject enchantPanel;
        [AtavismSeparator("Socketing")]
        [SerializeField] TextMeshProUGUI itemToSocketName;
        [SerializeField] UGUIGearSocketSlot itemSocketingSlot;
        [SerializeField] TextMeshProUGUI itemToPutInSocketName;
        [SerializeField] UGUIGearSocketSlot itemSlot;
        [SerializeField] TextMeshProUGUI itemToPutInSocketDectiption;
        [SerializeField] List<UGUICurrency> currency1;
        [SerializeField] Slider progressSlider;
        [SerializeField] Button embedButton;
        [AtavismSeparator("Reset Sockets")]
        [SerializeField] TextMeshProUGUI itemToRecetSocketName;
        [SerializeField] UGUIGearSocketSlot itemResetSocketSlot;
        [SerializeField] TextMeshProUGUI itemResetSocketDectiption;
        [SerializeField] List<UGUICurrency> currency2;
        [SerializeField] Slider resetProgressSlider;
        [SerializeField] Button resetButton;
        [AtavismSeparator("Enchanting")]
        [SerializeField] TextMeshProUGUI itemToEnchantName;
        [SerializeField] UGUIGearSocketSlot enchantSlot;
        [SerializeField] TextMeshProUGUI enchantDectiption;
        [SerializeField] Slider enchantProgressSlider;
        [SerializeField] Button enchantButton;
        [SerializeField] List<UGUICurrency> currency3;

        AtavismInventoryItem socketingItem;
        AtavismInventoryItem socketItem;
        AtavismInventoryItem enchantItem;
        float startTime;
        float endTime = -1;
        private bool showing = false;
        bool socketing = false;
        // Use this for initialization
        void Start()
        {
            NetworkAPI.RegisterExtensionMessageHandler("SocketingMsg", HandleSocketingMessage);
            NetworkAPI.RegisterExtensionMessageHandler("EnchantingMsg", HandleEnchantingMessage);
            NetworkAPI.RegisterExtensionMessageHandler("SocketResetMsg", HandleSocketResetMessage);
            AtavismEventSystem.RegisterEvent("GEAR_MODIFICATION_OPEN", this);
            itemSocketingSlot.SetSocket(SetSocketingItem, 1);
            itemSlot.SetSocket(SetSocketItem, 0);
            itemResetSocketSlot.SetSocket(SetResetItem, 1);
            enchantSlot.SetSocket(SetEnchantItem, 2);
            if (itemToPutInSocketName != null)
                itemToPutInSocketName.text = "";
            if (itemToSocketName != null)
                itemToSocketName.text = "";
            if (itemToRecetSocketName != null)
                itemToRecetSocketName.text = "";
            if (itemToEnchantName != null)
                itemToEnchantName.text = "";
            if (progressSlider != null)
                progressSlider.gameObject.SetActive(false);
            if (resetProgressSlider != null)
                resetProgressSlider.gameObject.SetActive(false);
            if (enchantProgressSlider != null)
                enchantProgressSlider.gameObject.SetActive(false);
            for (int i = 0; i < currency1.Count; i++)
            {
                currency1[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < currency2.Count; i++)
            {
                currency2[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < currency3.Count; i++)
            {
                currency3[i].gameObject.SetActive(false);
            }

            if (itemToPutInSocketDectiption != null)
                itemToPutInSocketDectiption.text = "";
            if (itemResetSocketDectiption != null)
                itemResetSocketDectiption.text = "";
            if (enchantDectiption != null)
                enchantDectiption.text = "";
            Hide();
            socketing = false;
            ShowSocketing();
        }



        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("GEAR_MODIFICATION_OPEN", this);
            NetworkAPI.RemoveExtensionMessageHandler("SocketingMsg", HandleSocketingMessage);
            NetworkAPI.RemoveExtensionMessageHandler("EnchantingMsg", HandleEnchantingMessage);
            NetworkAPI.RemoveExtensionMessageHandler("SocketResetMsg", HandleSocketResetMessage);

        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "GEAR_MODIFICATION_OPEN")
            {
                Mailing.Instance.RequestMailList();
                Show();
            }
        }


        private void SetSocketItem(AtavismInventoryItem item)
        {
            socketItem = item;

            if (itemToPutInSocketName != null)
                itemToPutInSocketName.text = item != null ? item.name : "";
            Inventory.Instance.SocketingCost(socketItem, socketingItem);
            if (socketItem == null || socketingItem == null)
            {
                for (int i = 0; i < currency1.Count; i++)
                {
                    currency1[i].gameObject.SetActive(false);
                }

            }
        }

        private void SetSocketingItem(AtavismInventoryItem item)
        {
            socketingItem = item;

            if (itemToSocketName != null)
                itemToSocketName.text = item != null ? item.name : "";
            Inventory.Instance.SocketingCost(socketItem, socketingItem);
            if (socketItem == null || socketingItem == null)
            {
                for (int i = 0; i < currency1.Count; i++)
                {
                    currency1[i].gameObject.SetActive(false);
                }

            }
        }

        private void SetEnchantItem(AtavismInventoryItem item)
        {
            enchantItem = item;
            if (itemToPutInSocketName != null)
                itemToPutInSocketName.text = item != null ? item.name : "";
            Inventory.Instance.EnchantCost(enchantItem);
            if (enchantItem == null)
            {

                for (int i = 0; i < currency3.Count; i++)
                {
                    currency3[i].gameObject.SetActive(false);
                }
                if (enchantDectiption != null)
                    enchantDectiption.text = "";
            }
        }
        private void SetResetItem(AtavismInventoryItem item)
        {
            socketItem = item;
            if (itemToPutInSocketName != null)
                itemToPutInSocketName.text = item != null ? item.name : "";
            Inventory.Instance.ResetSocketsCost(socketItem);
            if (socketItem == null)
            {

                for (int i = 0; i < currency2.Count; i++)
                {
                    currency2[i].gameObject.SetActive(false);
                }

            }
        }

        public void ClickEmbed()
        {
            if (socketItem != null && socketingItem != null && !socketing)
            {
                Inventory.Instance.EmbedInTheSlot(socketItem, socketingItem);
            }
        }
        public void ClickResetSockets()
        {
            if (socketItem != null && !socketing)
            {
                Inventory.Instance.ResetSloctsSlot(socketItem);
            }
        }

        public void ClickEnchant()
        {
            if (enchantItem != null && !socketing)
            {
                Inventory.Instance.EnchantItem(enchantItem);
            }
        }

        public void ShowSocketing()
        {
            if (socketPanel != null)
                socketPanel.SetActive(true);
            if (resetSocketPanel != null)
                resetSocketPanel.SetActive(false);
            if (enchantPanel != null)
                enchantPanel.SetActive(false);
            if (socketingText != null)
                socketingText.color = selectedColor;
            if (enchantingText != null)
                enchantingText.color = defauiltColor;
            if (resetSocketsText != null)
                resetSocketsText.color = defauiltColor;
            if (socketingImage != null)
                socketingImage.color = selectedImageColor;
            if (enchantingImage != null)
                enchantingImage.color = defauiltImageColor;
            if (resetSocketsImage != null)
                resetSocketsImage.color = defauiltImageColor;
            itemSlot.Discarded();
            itemSocketingSlot.Discarded();
            enchantSlot.Discarded();
            itemResetSocketSlot.Discarded();
        }

        public void ShowEnchanting()
        {
            if (socketPanel != null)
                socketPanel.SetActive(false);
            if (resetSocketPanel != null)
                resetSocketPanel.SetActive(false);
            if (enchantPanel != null)
                enchantPanel.SetActive(true);
            if (socketingText != null)
                socketingText.color = defauiltColor;
            if (enchantingText != null)
                enchantingText.color = selectedColor;
            if (resetSocketsText != null)
                resetSocketsText.color = defauiltColor;
            if (socketingImage != null)
                socketingImage.color = defauiltImageColor;
            if (enchantingImage != null)
                enchantingImage.color = selectedImageColor;
            if (resetSocketsImage != null)
                resetSocketsImage.color = defauiltImageColor;
            itemSlot.Discarded();
            itemSocketingSlot.Discarded();
            enchantSlot.Discarded();
            itemResetSocketSlot.Discarded();
        }

        public void ShowResetSockets()
        {
            if (socketPanel != null)
                socketPanel.SetActive(false);
            if (resetSocketPanel != null)
                resetSocketPanel.SetActive(true);
            if (enchantPanel != null)
                enchantPanel.SetActive(false);
            if (socketingText != null)
                socketingText.color = defauiltColor;
            if (enchantingText != null)
                enchantingText.color = defauiltColor;
            if (resetSocketsText != null)
                resetSocketsText.color = selectedColor;
            if (socketingImage != null)
                socketingImage.color = defauiltImageColor;
            if (enchantingImage != null)
                enchantingImage.color = defauiltImageColor;
            if (resetSocketsImage != null)
                resetSocketsImage.color = selectedImageColor;
            itemSlot.Discarded();
            itemSocketingSlot.Discarded();
            enchantSlot.Discarded();
            itemResetSocketSlot.Discarded();
        }


        void Show()
        {
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
            AtavismUIUtility.BringToFront(this.gameObject);
            if (AtavismCursor.Instance != null)
                AtavismCursor.Instance.SetUGUIActivatableClickedOverride(PlaceSocketingItem);
            ShowSocketing();
            showing = true;
        }

        public void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            GetComponent<CanvasGroup>().interactable = false;
            itemSlot.Discarded();
            itemSocketingSlot.Discarded();
            enchantSlot.Discarded();
            itemResetSocketSlot.Discarded();
            if (itemToPutInSocketName != null)
                itemToPutInSocketName.text = "";
            if (itemToSocketName != null)
                itemToSocketName.text = "";
            if (itemToRecetSocketName != null)
                itemToRecetSocketName.text = "";
            if (itemToEnchantName != null)
                itemToEnchantName.text = "";
            if (progressSlider != null)
                progressSlider.gameObject.SetActive(false);
            if (resetProgressSlider != null)
                resetProgressSlider.gameObject.SetActive(false);
            if (enchantProgressSlider != null)
                enchantProgressSlider.gameObject.SetActive(false);
            for (int i = 0; i < currency1.Count; i++)
            {
                currency1[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < currency2.Count; i++)
            {
                currency2[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < currency3.Count; i++)
            {
                currency3[i].gameObject.SetActive(false);
            }

            if (itemToPutInSocketDectiption != null)
                itemToPutInSocketDectiption.text = "";
            if (itemResetSocketDectiption != null)
                itemResetSocketDectiption.text = "";
            if (enchantDectiption != null)
                enchantDectiption.text = "";
            if (AtavismCursor.Instance != null)
                AtavismCursor.Instance.ClearUGUIActivatableClickedOverride(PlaceSocketingItem);
            showing = false;
        }

        private void PlaceSocketingItem(UGUIAtavismActivatable activatable)
        {
            //  Debug.LogError("PlaceSocketingItem " + activatable.Link);

            if (activatable.Link != null)
            {
                //       Debug.LogError("PlaceSocketingItem " + activatable.Link);
                return;
            }
            AtavismInventoryItem item = (AtavismInventoryItem)activatable.ActivatableObject;
            if (item != null)
            {
                if (socketPanel.activeSelf)
                {
                    if (item.sockettype.Length > 0)
                    {
                        itemSlot.SetActivatable(activatable);
                    }
                    else if (item.itemEffectTypes.Contains("Sockets"))
                    {
                        itemSocketingSlot.SetActivatable(activatable);
                    }
                }
                else if (resetSocketPanel.activeSelf)
                {
                    itemResetSocketSlot.SetActivatable(activatable);
                }
                else if (enchantPanel.activeSelf)
                {
                    //   Debug.LogError("item.EnchantId: "+item.EnchantId);
                    if (item.EnchantId > 0)
                    {
                        enchantSlot.SetActivatable(activatable);
                    }
                    else
                    {
                        activatable.PreventDiscard();

                        //     Debug.LogError("Wrong Item");
                        string[] args = new string[1];
#if AT_I2LOC_PRESET
                    args[0] = I2.Loc.LocalizationManager.GetTranslation("Wrong Item");
#else
                        args[0] = "Wrong Item";
#endif
                        AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
                    }
                }
            }
        }
        //  enchantSlot.Discarded();
        public void Toggle()
        {
            if (showing)
                Hide();
            else
                Show();
        }

        void HandleSocketingMessage(Dictionary<string, object> props)
        {
            string msgType = (string)props["PluginMessageType"];
            switch (msgType)
            {

                case "SocketingStarted":
                    {
                        Debug.LogWarning("SocketingStarted");

                        float creationTime = (float)props["creationTime"];
                        Debug.LogWarning("SocketingStarted creationTime:" + creationTime);

                        if (creationTime > 0)
                        {
                            progressSlider.gameObject.SetActive(true);
                            startTime = Time.time;
                            endTime = Time.time + creationTime;
                        }
                        socketing = true;
                        break;
                    }
                case "SocketingCompleted":
                    {
                        Debug.LogWarning("SocketingCompleted");
                        itemSlot.Discarded();// UpdateAttachmentData(null);
                        itemSocketingSlot.Discarded();
                        socketing = false;
                        break;
                    }
                case "SocketingFailed":
                    {
                        Debug.LogWarning("SocketingFailed");
                        itemSlot.Discarded();// UpdateAttachmentData(null);
                        itemSocketingSlot.Discarded();
                        string[] args = new string[1];
                        args[0] = (string)props["ErrorMsg"];
                        AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
                        socketing = false;
                        break;
                    }
                case "SocketingInterrupted":
                    {
                        Debug.LogWarning("Socketing was interrupted");
                        // dispatch a ui event to tell the rest of the system
                        ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismMobController>().PlayAnimation("", 0);
                        socketing = false;
                        if (progressSlider != null)
                            progressSlider.gameObject.SetActive(false);
                        break;
                    }
                case "SocketingUpdate":
                    {
                        Debug.LogWarning("SocketingUpdate");
                        long creationCost = (long)props["creationCost"];
                        int creationCurrency = (int)props["creationCurrency"];
                        Debug.LogWarning("SocketingUpdate creationCost:" + creationCost + " creationCurrency:" + creationCurrency);
                        List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(creationCurrency, creationCost);
                        for (int i = 0; i < currency1.Count; i++)
                        {
                            if (i < currencyDisplayList.Count)
                            {
                                currency1[i].gameObject.SetActive(true);
                                currency1[i].SetCurrencyDisplayData(currencyDisplayList[i]);
                            }
                            else
                            {
                                currency1[i].gameObject.SetActive(false);
                            }
                        }
                        break;
                    }
            }
            AtavismLogger.LogDebugMessage("Got A Socketing Message!");
        }
        void HandleSocketResetMessage(Dictionary<string, object> props)
        {
            string msgType = (string)props["PluginMessageType"];
            switch (msgType)
            {

                case "SocketResetStarted":
                    {
                        Debug.LogWarning("SocketResetStarted");

                        float creationTime = (float)props["creationTime"];
                        Debug.LogWarning("SocketResetStarted creationTime:" + creationTime);

                        if (creationTime > 0)
                        {
                            if (resetProgressSlider != null)
                                resetProgressSlider.gameObject.SetActive(true);
                            startTime = Time.time;
                            endTime = Time.time + creationTime;
                        }
                        socketing = true;
                        break;
                    }
                case "SocketResetCompleted":
                    {
                        Debug.LogWarning("SocketResetCompleted");
                        itemResetSocketSlot.Discarded();// UpdateAttachmentData(null);
                        socketing = false;
                        break;
                    }
                case "SocketResetFailed":
                    {
                        Debug.LogWarning("SocketingFailed");
                        itemResetSocketSlot.Discarded();// UpdateAttachmentData(null);
                        string[] args = new string[1];
                        args[0] = (string)props["ErrorMsg"];
                        AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
                        socketing = false;
                        break;
                    }
                case "SocketResetInterrupted":
                    {
                        Debug.LogWarning("Socket Reset was interrupted");
                        // dispatch a ui event to tell the rest of the system
                        ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismMobController>().PlayAnimation("", 0);
                        socketing = false;
                        if (resetProgressSlider != null)
                            resetProgressSlider.gameObject.SetActive(false);
                        break;
                    }
                case "SocketResetUpdate":
                    {
                        Debug.LogWarning("SocketResetUpdate");
                        long creationCost = (long)props["creationCost"];
                        int creationCurrency = (int)props["creationCurrency"];
                        Debug.LogWarning("SocketResetUpdate creationCost:" + creationCost + " creationCurrency:" + creationCurrency);
                        List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(creationCurrency, creationCost);
                        for (int i = 0; i < currency2.Count; i++)
                        {
                            if (i < currencyDisplayList.Count)
                            {
                                currency2[i].gameObject.SetActive(true);
                                currency2[i].SetCurrencyDisplayData(currencyDisplayList[i]);
                            }
                            else
                            {
                                currency2[i].gameObject.SetActive(false);
                            }
                        }
                        break;
                    }
            }
            AtavismLogger.LogDebugMessage("Got A Socketing Message!");
        }

        void HandleEnchantingMessage(Dictionary<string, object> props)
        {
            string msgType = (string)props["PluginMessageType"];
            switch (msgType)
            {
                case "EnchantingStarted":
                    {
                        if (AtavismLogger.logLevel <= LogLevel.Debug)  Debug.LogWarning("EnchantingStarted");

                        float creationTime = (float)props["creationTime"];
                        if (AtavismLogger.logLevel <= LogLevel.Debug)  Debug.LogWarning("EnchantingStarted creationTime:" + creationTime);

                        if (creationTime > 0)
                        {
                            if (enchantProgressSlider != null)
                                enchantProgressSlider.gameObject.SetActive(true);
                            startTime = Time.time;
                            endTime = Time.time + creationTime;
                        }
                        socketing = true;
                        break;
                    }
                case "EnchantingCompleted":
                    {
                        if (AtavismLogger.logLevel <= LogLevel.Debug)  Debug.LogWarning("EnchantingCompleted");
                        enchantSlot.Discarded();// UpdateAttachmentData(null);
                        socketing = false;
                        break;
                    }
                case "EnchantingFailed":
                    {
                        if (AtavismLogger.logLevel <= LogLevel.Debug)   Debug.LogWarning("EnchantingFailed");
                        enchantSlot.Discarded();// UpdateAttachmentData(null);
                        string[] args = new string[1];
                        args[0] = (string)props["ErrorMsg"];
                        AtavismEventSystem.DispatchEvent("ERROR_MESSAGE", args);
                        socketing = false;
                        break;
                    }
                case "EnchantingInterrupted":
                    {
                        if (AtavismLogger.logLevel <= LogLevel.Debug)  Debug.LogWarning("Enchanting was interrupted");
                        // dispatch a ui event to tell the rest of the system
                        ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismMobController>().PlayAnimation("", 0);
                        socketing = false;
                        if (enchantProgressSlider != null)
                            enchantProgressSlider.gameObject.SetActive(false);
                        break;
                    }
                case "EnchantingUpdate":
                    {
                        if (AtavismLogger.logLevel <= LogLevel.Debug)
                        {
                            string keys = " [ ";

                            foreach (var it in props.Keys)
                            {
                                keys += " ; " + it + " => " + props[it];
                            }
                            Debug.LogWarning("EnchantingUpdate: keys:" + keys);
                            Debug.LogWarning("EnchantingUpdate");
                        }

                        long creationCost = (long)props["creationCost"];
                        int creationCurrency = (int)props["creationCurrency"];
                        if (AtavismLogger.logLevel <= LogLevel.Debug)
                            Debug.LogWarning("EnchantingUpdate creationCost:" + creationCost + " creationCurrency:" + creationCurrency);
                        List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(creationCurrency, creationCost);
                        for (int i = 0; i < currency3.Count; i++)
                        {
                            if (i < currencyDisplayList.Count)
                            {
                                currency3[i].gameObject.SetActive(true);
                                currency3[i].SetCurrencyDisplayData(currencyDisplayList[i]);
                            }
                            else
                            {
                                currency3[i].gameObject.SetActive(false);
                            }
                        }
                        string detale = "";
                        int statCount = (int)props["statNumber"];
#if AT_I2LOC_PRESET
                    detale += I2.Loc.LocalizationManager.GetTranslation("Enchant to level") + " " + props["nextLevel"] + "\n";
#else
                        detale += "Enchant to level " + props["nextLevel"] + "\n";
#endif
                        for (int i = 0; i < statCount; i++)
                        {
                            if (!props["stat" + i + "name"].Equals("dmg-base") && !props["stat" + i + "name"].Equals("dmg-max"))
                            {
#if AT_I2LOC_PRESET
                            detale += I2.Loc.LocalizationManager.GetTranslation((string)props["stat" + i + "name"]) + " " + props["stat" + i + "value"] + "\n";
#else
                                detale += props["stat" + i + "name"] + " " + props["stat" + i + "value"] + "\n";
#endif
                            }
                            else if (props["stat" + i + "name"].Equals("dmg-base"))
                            {
#if AT_I2LOC_PRESET
                            detale += I2.Loc.LocalizationManager.GetTranslation("Damage") + " " + props["stat" + i + "value"] + "\n";
#else
                                detale += "Damage " + props["stat" + i + "value"] + "\n";
#endif

                            }
                        }

                        if (enchantDectiption != null)
                            enchantDectiption.text = detale;

                        break;
                    }
            }
            AtavismLogger.LogDebugMessage("Got A Enchanting Message!");
        }


        void Update()
        {
            if (endTime != -1 && endTime > Time.time)
            {
                float total = endTime - startTime;
                float currentTime = endTime - Time.time;
                if (progressSlider != null)
                    progressSlider.value = 1 - ((float)currentTime / (float)total);
                if (resetProgressSlider != null)
                    resetProgressSlider.value = 1 - ((float)currentTime / (float)total);
                if (enchantProgressSlider != null)
                    enchantProgressSlider.value = 1 - ((float)currentTime / (float)total);
            }
            else
            {
                if (progressSlider != null)
                    progressSlider.gameObject.SetActive(false);
                if (resetProgressSlider != null)
                    resetProgressSlider.gameObject.SetActive(false);
                if (enchantProgressSlider != null)
                    enchantProgressSlider.gameObject.SetActive(false);
            }
        }


    }
}