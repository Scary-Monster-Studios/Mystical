using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
namespace Atavism
{

    enum ChatWindowView
    {
        Chat,
        EventLog,
        CombatLog,
        Group,
        Guild,
        Whisper

    }


    public static class ColorTypeConverter
    {
        public static string ToRGBHex(Color c)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
        }

        private static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }
    }

    public class UGUIChatController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        static UGUIChatController instance;
        [SerializeField] GameObject chatTextPrefab;
        [SerializeField] RectTransform chatTextParent;
        public ScrollRect scrollView;
        //public Text textWindow;
        public TextMeshProUGUI textWindow;
        //  public InputField input;
        public TMP_InputField input;

        [AtavismSeparator("Text Colors Settings")]
        //Public Chat Colors
        public Color chatColor = Color.white;
        public Color eventColor = Color.yellow;
        public Color combatColor = Color.red;
        public Color announcementColor = Color.green;
        public Color whisperColor = Color.cyan;
        public Color groupColor = Color.magenta;
        public Color guildColor = Color.blue;
        public Color globalColor = new Color(1f, 0.5f, 0f);
        public Color instanceColor = new Color(.5f, 0f, 1f);
        public Color currentChannelCommandColor = Color.white;

        [AtavismSeparator("Menu Settings")]
        public Button chatButton;
        public Button combatButton;
        public Button eventButton;
        public Button guildButton;
        public Button groupButton;
        public TextMeshProUGUI chatButtonText;
        public TextMeshProUGUI combatButtonText;
        public TextMeshProUGUI eventButtonText;
        public TextMeshProUGUI guildButtonText;
        public TextMeshProUGUI groupButtonText;
        public Color defaultColorActive = Color.white;
        public Color chatColorActive = Color.green;
        public Color combatColorActive = Color.magenta;
        public Color eventColorActive = Color.cyan;
        public Color guildColorActive = Color.blue;
        public Color groupColorActive = Color.cyan;

        static ChatWindowView windowView = ChatWindowView.Chat;
        static string currentChannelCommand = "/say";
        static string placeholderText = "Say:";
        static List<string> chatMessages = new List<string>();
        static List<string> eventLog = new List<string>();
        static List<string> combatLog = new List<string>();
        static List<string> guildLog = new List<string>();
        static List<string> whisperLog = new List<string>();
        static List<string> groupLog = new List<string>();
        public List<TextMeshProUGUI> chatWindows = new List<TextMeshProUGUI>();

        private bool isHoveringObject;
        //   private int m_selectedWord = -1;
        private int m_selectedLink = -1;
        //   private int m_lastIndex = -1;
        private TextMeshProUGUI m_selectedWindow = null;
        private TextMeshProUGUI m_TextMeshPro;
        private Canvas m_Canvas;
        private Camera m_Camera;
        [SerializeField]
        int chatLimitRow = 1000;
        [SerializeField]
        int chatLimitWindow = 3;
        [Range(1,18000)]
        [SerializeField]
        int TextLengthPerTextObject = 18000;

        int textLines = 1;
        [SerializeField] bool reloginClearHistory = false;

        void Start()
        {
            chatTextParent.pivot = new Vector2(0, 0);
            if (instance != null)
            {
                GameObject.DestroyImmediate(gameObject);
                return;
            }
            instance = this;
            m_Canvas = gameObject.GetComponentInParent<Canvas>();
            if (m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                m_Camera = null;
            else
                m_Camera = m_Canvas.worldCamera;
            scrollView.verticalScrollbar.value = 0f;

            AtavismEventSystem.RegisterEvent("CHAT_MSG_SERVER", this);
            AtavismEventSystem.RegisterEvent("CHAT_MSG_SAY", this);
            AtavismEventSystem.RegisterEvent("CHAT_MSG_SYSTEM", this);
            AtavismEventSystem.RegisterEvent("ADMIN_MESSAGE", this);

            NetworkAPI.RegisterExtensionMessageHandler("announcement", HandleAnnouncementMessage);
            AtavismEventSystem.RegisterEvent("INVENTORY_EVENT", this);
            AtavismEventSystem.RegisterEvent("COMBAT_EVENT", this);
            AtavismEventSystem.RegisterEvent("UPDATE_LANGUAGE", this);

            // Load in existing channel text
            if (windowView == ChatWindowView.Chat)
            {
                ViewChat();
            }
            else if (windowView == ChatWindowView.CombatLog)
            {
                ViewCombatLog();
            }
            else if (windowView == ChatWindowView.EventLog)
            {
                ViewEventLog();
            }
            else if (windowView == ChatWindowView.Group)
            {
                ViewGroupLog();
            }
            else if (windowView == ChatWindowView.Guild)
            {
                ViewGuild();
            }
            SceneManager.sceneLoaded += LevelWasLoaded;

        }

        void LevelWasLoaded(Scene newScene, LoadSceneMode mode)
        {
            if(reloginClearHistory)
            if (newScene.name.Equals("Login") || newScene.name.Equals(ClientAPI.Instance.characterSceneName))
            {
                
                chatMessages.Clear();
                eventLog.Clear();
                combatLog.Clear();
                guildLog.Clear();
                whisperLog.Clear();
                groupLog.Clear();
            }

        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("CHAT_MSG_SERVER", this);
            AtavismEventSystem.UnregisterEvent("CHAT_MSG_SAY", this);
            AtavismEventSystem.UnregisterEvent("CHAT_MSG_SYSTEM", this);
            AtavismEventSystem.UnregisterEvent("ADMIN_MESSAGE", this);

            NetworkAPI.RemoveExtensionMessageHandler("announcement", HandleAnnouncementMessage);
            AtavismEventSystem.UnregisterEvent("INVENTORY_EVENT", this);
            AtavismEventSystem.UnregisterEvent("COMBAT_EVENT", this);
            AtavismEventSystem.UnregisterEvent("UPDATE_LANGUAGE", this);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (!ClientAPI.UIHasFocus())
                {
                    // If no other inputs are selected, give focus to this input when enter is pressed.
                    EventSystem.current.SetSelectedGameObject(input.gameObject);
                }
                else if (EventSystem.current.currentSelectedGameObject == input.gameObject)
                {
                    // Clear the input from selection so keys work again for movement
                    if (input != null)
                        OnSubmit(input.text);
                    //     if (TMPinput != null)
                    //        OnSubmit(TMPinput.text);
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }

            //	if (input != null && input.placeholder.GetComponent<Text>() != null)
            //		input.placeholder.GetComponent<Text>().text = placeholderText;
            if (input != null && input.placeholder.GetComponent<TextMeshProUGUI>() != null)
                input.placeholder.GetComponent<TextMeshProUGUI>().text = placeholderText;
        }
        public void Submit()
        {
            if (input != null)
                OnSubmit(input.text);
            EventSystem.current.SetSelectedGameObject(null);
        }

        public void OnSubmit(string message)
        {
            string text = message; //NGUIText.StripSymbols(mInput.value);

            if (!string.IsNullOrEmpty(text))
            {



                if (text.ToLower().Equals("/test"))
                {
                    Dictionary<string, object> props = new Dictionary<string, object>();
                    props.Add("coordEffect", "Emote_wave"); // Put name of Coord Effect Prefab to play here
                    props.Add("hasTarget", false);
                    NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.PLAY_COORD_EFFECT", props);
                }
                // check if it has a / at the front and if it is a new channel option
                if (text.ToLower().Equals("/navmeshtest"))
                {
                    Dictionary<string, object> props = new Dictionary<string, object>();
                    props.Add("add", 1);
                    //  props.Add("state", MoveToNextState());
                    NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.ADD_NM_OBJECT", props);
                }
               
                if (text.ToLower().Equals("/help") || text.ToLower().Equals("/help ") || text.ToLower().Equals("/h") || text.ToLower().Equals("/h "))
                {
                    ChatHelp();
                    input.text = "";
                    return;
                }
                else if (text.StartsWith("/clearChat"))
                {
                    switch (windowView)
                    {
                        case ChatWindowView.Chat:
                            chatMessages.Clear();
                            break;
                        case ChatWindowView.EventLog:
                            eventLog.Clear();
                            break;
                        case ChatWindowView.CombatLog:
                            combatLog.Clear();
                            break;
                        case ChatWindowView.Guild:
                            guildLog.Clear();
                            break;
                        case ChatWindowView.Whisper:
                            whisperLog.Clear();
                            break;
                        case ChatWindowView.Group:
                            groupLog.Clear();
                            break;
                }
                    foreach (Transform child in chatTextParent)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                    chatWindows.Clear();
                    var go = Instantiate(chatTextPrefab, chatTextParent);
                    textWindow = go.GetComponent<TextMeshProUGUI>();
                    chatWindows.Add(go.GetComponent<TextMeshProUGUI>());
                }
                else if (text.StartsWith("/say ") || text.StartsWith("/s "))
                {

                    currentChannelCommand = "/say";
#if AT_I2LOC_PRESET
                placeholderText = I2.Loc.LocalizationManager.GetTranslation("Say") + ":";
#else
                    placeholderText = "Say:";
#endif
                    currentChannelCommandColor = chatColor;
                }
                else if (text.StartsWith("/group ") || text.StartsWith("/p "))
                {
                    currentChannelCommand = "/group";
#if AT_I2LOC_PRESET
                placeholderText = I2.Loc.LocalizationManager.GetTranslation("Group") + ":";
#else
                    placeholderText = "Group:";
#endif
                    currentChannelCommandColor = chatColor;
                }
                else if (text.StartsWith("/whisper ") || text.StartsWith("/w "))
                {
                    string[] splitMessage = message.Split(' ');
                    if (splitMessage[1].StartsWith("\""))
                    {
                        splitMessage = message.Split('"');
                        currentChannelCommand = "/whisper \"" + splitMessage[1]+"\"";
                    }
                    else
                    {
                        currentChannelCommand = "/whisper " + splitMessage[1];
                    }



#if AT_I2LOC_PRESET
                placeholderText = I2.Loc.LocalizationManager.GetTranslation("Whisper") + " " + splitMessage[1] + ":";
#else
                    placeholderText = "Whisper " + splitMessage[1] + ":";
#endif
                    //var len = currentChannelCommand.Length;
                    //currentChannelCommandColor = whisperColor;
                    //text = currentChannelCommand + " " + GetLine(text.Substring(len, text.Length - len), whisperColor);

                }
                else if (text.StartsWith("/guild ") || text.StartsWith("/g "))
                {
                    currentChannelCommand = "/guild";
#if AT_I2LOC_PRESET
                placeholderText = I2.Loc.LocalizationManager.GetTranslation("Guild") + ":";
#else
                    placeholderText = "Guild:";
#endif
                    currentChannelCommandColor = chatColor;
                }
                else if (!text.StartsWith("/"))
                {
                    text = currentChannelCommand + " " + text;// GetLine(text, currentChannelCommandColor);
                }

                // Intercept global chat channels here
                if (text.StartsWith("/1 "))
                {
                    currentChannelCommand = "/1";
#if AT_I2LOC_PRESET
                placeholderText = I2.Loc.LocalizationManager.GetTranslation("Instance General") + ":";
#else
                    placeholderText = "Instance General:";
#endif
                    currentChannelCommandColor = chatColor;
                    Dictionary<string, object> props = new Dictionary<string, object>();
                    props.Add("channel", 10); // Channels 1-5 are already used, lets try 10+
                    message = text.Substring(text.IndexOf(' '));
                    props.Add("message", message);
                    NetworkAPI.SendExtensionMessage(0, false, "ao.GLOBAL_CHAT", props);

                    input.text = "";
                    return;
                }
                else if (text.StartsWith("/2 "))
                {
                    currentChannelCommand = "/2";
#if AT_I2LOC_PRESET
                placeholderText = I2.Loc.LocalizationManager.GetTranslation("Global") + ":";
#else
                    placeholderText = "Global:";
#endif
                    currentChannelCommandColor = chatColor;
                    Dictionary<string, object> props = new Dictionary<string, object>();
                    props.Add("channel", -1); // Use -1 for global
                    message = text.Substring(text.IndexOf(' '));
                    props.Add("message", message);
                    NetworkAPI.SendExtensionMessage(0, false, "ao.GLOBAL_CHAT", props);

                    input.text = "";
                    return;
                }
                else if (text.StartsWith("/admininfo "))
                {
                    currentChannelCommand = "/admininfo";
#if AT_I2LOC_PRESET
                placeholderText = I2.Loc.LocalizationManager.GetTranslation("Admin") + ":";
#else
                    placeholderText = "Admin:";
#endif
                    currentChannelCommandColor = chatColor;
                    Dictionary<string, object> props = new Dictionary<string, object>();
                    props.Add("channel", -2); // Use -1 for global
                    message = text.Substring(text.IndexOf(' '));
                    props.Add("message", message);
                    NetworkAPI.SendExtensionMessage(0, false, "ao.GLOBAL_CHAT", props);
                    input.text = "";
                    return;
                }


                // Send to the Atavism Command class
                AtavismCommand.HandleCommand(text);
                input.text = "";
                //mInput.isSelected = false;
            }
        }

        // This method to be called when remote chat message is received

        void AddGuildMessage(string message)
        {
            bool scrollToBottom = false;
            int proc = textLines - 1;
            if (proc < 1)
                proc = 1;
            if (textLines < 15)
                scrollView.verticalScrollbar.value = 0f;
            if (scrollView.verticalScrollbar.value <= 2f / proc)
                scrollToBottom = true;

            var line = GetLine(message, guildColor);
            if (guildLog.Count > chatLimitRow)
            {
                while (guildLog.Count > chatLimitRow)
                    guildLog.RemoveAt(0);
            }
            if (line.Contains("<link=item#"))
            {
                string s = line.Substring(line.IndexOf("<link=item#") + 11);
                s = s.Substring(0, s.IndexOf(">"));
                int id = Int32.Parse(s);
                Inventory.Instance.GetItemByTemplateID(id);
            }
            if (line.Contains("<link=ability#"))
            {
                string s = line.Substring(line.IndexOf("<link=ability#") + 14);
                s = s.Substring(0, s.IndexOf(">"));
                int id = Int32.Parse(s);
                Abilities.Instance.GetAbility(id);
            }
            if (line.Contains("<size="))
            {
                while (line.Contains("<size="))
                {
                    string fs = line.Substring(0, line.IndexOf("<size"));
                    string s = line.Substring(line.IndexOf("<size"));
                    s = s.Substring(s.IndexOf(">") + 1);
                    line = fs + s;
                }
            }
            guildLog.Add(line);
            if (windowView == ChatWindowView.Guild)
            {
                if (textWindow != null && combatLog.Count > chatLimitRow)
                {

                    if (chatWindows.Count == 1)
                    {
                        string _t = textWindow.text;
                        int count = _t.Split('\n').Length - 1;
                        for (int i = 0; i < count - chatLimitRow; i++)
                        {
                            _t = _t.Remove(0, _t.IndexOf("\n") + 1);
                        }
                        textWindow.text = _t;
                    }
                    else if (chatWindows.Count > 1)
                    {
                        int count = 0;
                        foreach (TextMeshProUGUI t in chatWindows)
                        {
                            count += t.text.Split('\n').Length - 1;
                        }
                        int todelete = count - chatLimitRow;
                        if (todelete > 0)
                        {
                            foreach (TextMeshProUGUI t in chatWindows)
                            {
                                string _t = t.text;
                                int maxcount = _t.Split('\n').Length - 1;
                                while (todelete > 0 && (((_t.Split('\n').Length - 1) > 0) || ((_t.Split('\n').Length - 1) == 0 && _t.Length > 0)))
                                {
                                    int idx = _t.IndexOf("\n");
                                    if (idx > 0)
                                        _t = _t.Remove(0, idx + 1);
                                    else
                                        _t = "";
                                    todelete--;
                                }
                                t.text = _t;
                            }
                        }
                    }
                }
                string chattext = "";
                if (textWindow == null || textWindow.text.Length > TextLengthPerTextObject)
                {

                    if (chatWindows.Count > chatLimitWindow - 1)
                    {
                        while (chatWindows.Count > chatLimitWindow - 1)
                        {
                            DestroyImmediate(chatWindows[0].gameObject);
                            chatWindows.RemoveAt(0);
                        }
                    }
                    var go = Instantiate(chatTextPrefab, chatTextParent);
                    textWindow = go.GetComponent<TextMeshProUGUI>();
                    go.name = go.name + chatTextParent.transform.childCount;
                    chatWindows.Add(go.GetComponent<TextMeshProUGUI>());
                }
                else
                {
                    chattext = textWindow.text;
                }
                if (chattext.Length > 0)
                    chattext += "\n";
                textWindow.text = chattext + line;


                textLines = guildLog.Count;
                if (scrollToBottom)
                {
                    scrollView.verticalScrollbar.value = 0f;
                }
            }
        }


        void AddGroupMessage(string message)
        {
            bool scrollToBottom = false;
            int proc = textLines - 1;
            if (proc < 1)
                proc = 1;
            if (textLines < 15)
                scrollView.verticalScrollbar.value = 0f;
            if (scrollView.verticalScrollbar.value <= 2f / proc)
                scrollToBottom = true;
            if (groupLog.Count > chatLimitRow)
            {
                while (groupLog.Count > chatLimitRow)
                    groupLog.RemoveAt(0);
            }
            var line = GetLine(message, groupColor);
            if (line.Contains("<link=item#"))
            {
                string s = line.Substring(line.IndexOf("<link=item#") + 11);
                s = s.Substring(0, s.IndexOf(">"));
                int id = Int32.Parse(s);
                Inventory.Instance.GetItemByTemplateID(id);
            }
            if (line.Contains("<link=ability#"))
            {
                string s = line.Substring(line.IndexOf("<link=ability#") + 14);
                s = s.Substring(0, s.IndexOf(">"));
                int id = Int32.Parse(s);
                Abilities.Instance.GetAbility(id);
            }
            if (line.Contains("<size="))
            {
                while (line.Contains("<size="))
                {
                    string fs = line.Substring(0, line.IndexOf("<size"));
                    string s = line.Substring(line.IndexOf("<size"));
                    s = s.Substring(s.IndexOf(">") + 1);
                    line = fs + s;
                }
            }

            groupLog.Add(line);
            if (windowView == ChatWindowView.Group)
            {
                if (textWindow != null && combatLog.Count > chatLimitRow)
                {

                    if (chatWindows.Count == 1)
                    {
                        string _t = textWindow.text;
                        int count = _t.Split('\n').Length - 1;
                        for (int i = 0; i < count - chatLimitRow; i++)
                        {
                            _t = _t.Remove(0, _t.IndexOf("\n") + 1);
                        }
                        textWindow.text = _t;
                    }
                    else if (chatWindows.Count > 1)
                    {
                        int count = 0;
                        foreach (TextMeshProUGUI t in chatWindows)
                        {
                            count += t.text.Split('\n').Length - 1;
                        }
                        int todelete = count - chatLimitRow;
                        if (todelete > 0)
                        {
                            foreach (TextMeshProUGUI t in chatWindows)
                            {
                                string _t = t.text;
                                int maxcount = _t.Split('\n').Length - 1;
                                while (todelete > 0 && (((_t.Split('\n').Length - 1) > 0) || ((_t.Split('\n').Length - 1) == 0 && _t.Length > 0)))
                                {
                                    int idx = _t.IndexOf("\n");
                                    if (idx > 0)
                                        _t = _t.Remove(0, idx + 1);
                                    else
                                        _t = "";
                                    todelete--;
                                }
                                t.text = _t;
                            }
                        }
                    }
                }
                string chattext = "";
                if (textWindow == null || textWindow.text.Length > TextLengthPerTextObject)
                {

                    if (chatWindows.Count > chatLimitWindow - 1)
                    {
                        while (chatWindows.Count > chatLimitWindow - 1)
                        {
                            DestroyImmediate(chatWindows[0].gameObject);
                            chatWindows.RemoveAt(0);
                        }
                    }
                    var go = Instantiate(chatTextPrefab, chatTextParent);
                    textWindow = go.GetComponent<TextMeshProUGUI>();
                    go.name = go.name + chatTextParent.transform.childCount;
                    chatWindows.Add(go.GetComponent<TextMeshProUGUI>());
                }
                else
                {
                    chattext = textWindow.text;
                }
                if (chattext.Length > 0)
                    chattext += "\n";
                textWindow.text = chattext + line;
                textLines = groupLog.Count;
                if (scrollToBottom)
                {
                    scrollView.verticalScrollbar.value = 0f;
                }
            }
        }


        void AddChatMessage(string message)
        {
            bool scrollToBottom = false;
            if (scrollView.verticalScrollbar.value == 0f)
                scrollToBottom = true;
            if (chatMessages.Count > chatLimitRow)
            {
                while (chatMessages.Count > chatLimitRow)
                    chatMessages.RemoveAt(0);
            }
            var line = GetLine(message, chatColor);
            if (line.Contains("<link=item#"))
            {
                string s = line.Substring(line.IndexOf("<link=item#") + 11);
                s = s.Substring(0, s.IndexOf(">"));
                int id = Int32.Parse(s);
                Inventory.Instance.GetItemByTemplateID(id);
            }
            if (line.Contains("<link=ability#"))
            {
                string s = line.Substring(line.IndexOf("<link=ability#") + 14);
                s = s.Substring(0, s.IndexOf(">"));
                int id = Int32.Parse(s);
                Abilities.Instance.GetAbility(id);
            }
            if (line.Contains("<size="))
            {
                while (line.Contains("<size="))
                {
                    string fs = line.Substring(0, line.IndexOf("<size"));
                    string s = line.Substring(line.IndexOf("<size"));
                    s = s.Substring(s.IndexOf(">") + 1);
                    line = fs + s;
                }
            }

            chatMessages.Add(line);
            if (windowView == ChatWindowView.Chat)
            {
                if (textWindow != null && combatLog.Count > chatLimitRow)
                {

                    if (chatWindows.Count == 1)
                    {
                        string _t = textWindow.text;
                        int count = _t.Split('\n').Length - 1;
                        for (int i = 0; i < count - chatLimitRow; i++)
                        {
                            _t = _t.Remove(0, _t.IndexOf("\n") + 1);
                        }
                        textWindow.text = _t;
                    }
                    else if (chatWindows.Count > 1)
                    {
                        int count = 0;
                        foreach (TextMeshProUGUI t in chatWindows)
                        {
                            count += t.text.Split('\n').Length - 1;
                        }
                        int todelete = count - chatLimitRow;
                        if (todelete > 0)
                        {
                            foreach (TextMeshProUGUI t in chatWindows)
                            {
                                string _t = t.text;
                                int maxcount = _t.Split('\n').Length - 1;
                                while (todelete > 0 && (((_t.Split('\n').Length - 1) > 0) || ((_t.Split('\n').Length - 1) == 0 && _t.Length > 0)))
                                {
                                    int idx = _t.IndexOf("\n");
                                    if (idx > 0)
                                        _t = _t.Remove(0, idx + 1);
                                    else
                                        _t = "";
                                    todelete--;
                                }
                                t.text = _t;
                            }
                        }
                    }
                }
                string chattext = "";
                if (textWindow == null || textWindow.text.Length > TextLengthPerTextObject)
                {

                    if (chatWindows.Count > chatLimitWindow - 1)
                    {
                        while (chatWindows.Count > chatLimitWindow - 1)
                        {
                            DestroyImmediate(chatWindows[0].gameObject);
                            chatWindows.RemoveAt(0);
                        }
                    }
                    var go = Instantiate(chatTextPrefab, chatTextParent);
                    textWindow = go.GetComponent<TextMeshProUGUI>();
                    go.name = go.name + chatTextParent.transform.childCount;
                    chatWindows.Add(go.GetComponent<TextMeshProUGUI>());
                }
                else
                {
                    chattext = textWindow.text;
                }
                if (chattext.Length > 0)
                    chattext += "\n";
                textWindow.text = chattext + line;
                textWindow.ForceMeshUpdate();
                textLines = chatMessages.Count;
                if (scrollToBottom)
                    scrollView.velocity = new Vector2(0, 1000f);
            }
        }

        // This method to be called when remote chat message is received
        void AddCombatMessage(string message)
        {
            //     Debug.LogError("CHAT:  message:" + message);
            bool scrollToBottom = false;
            int proc = textLines - 1;
            if (proc < 1)
                proc = 1;
            if (textLines < 15)
                scrollView.verticalScrollbar.value = 0f;
            if (scrollView.verticalScrollbar.value <= 2f / proc)
                scrollToBottom = true;
            if (combatLog.Count > chatLimitRow)
            {
                while (combatLog.Count > chatLimitRow)
                    combatLog.RemoveAt(0);
            }
            var line = GetLine(message, combatColor);
            combatLog.Add(line);

            if (windowView == ChatWindowView.CombatLog)
            {

                if (textWindow != null && combatLog.Count > chatLimitRow)
                {

                    if (chatWindows.Count == 1)
                    {
                        string _t = textWindow.text;
                        int count = _t.Split('\n').Length - 1;
                        for (int i = 0; i < count - chatLimitRow; i++)
                        {
                            _t = _t.Remove(0, _t.IndexOf("\n") + 1);
                        }
                        textWindow.text = _t;
                    }
                    else if (chatWindows.Count > 1)
                    {
                        int count = 0;
                        foreach (TextMeshProUGUI t in chatWindows)
                        {
                            count += t.text.Split('\n').Length - 1;
                        }
                        int todelete = count - chatLimitRow;
                        if (todelete > 0)
                        {
                            foreach (TextMeshProUGUI t in chatWindows)
                            {
                                string _t = t.text;
                                int maxcount = _t.Split('\n').Length - 1;
                                while (todelete > 0 && (((_t.Split('\n').Length - 1) > 0) || ((_t.Split('\n').Length - 1) == 0 && _t.Length > 0)))
                                {
                                    int idx = _t.IndexOf("\n");
                                    if (idx > 0)
                                        _t = _t.Remove(0, idx + 1);
                                    else
                                        _t = "";
                                    todelete--;
                                }
                                t.text = _t;
                            }
                        }
                    }
                }
                string chattext = "";
                if (textWindow == null || textWindow.text.Length > TextLengthPerTextObject)
                {

                    if (chatWindows.Count > chatLimitWindow - 1)
                    {
                        while (chatWindows.Count > chatLimitWindow - 1)
                        {
                            DestroyImmediate(chatWindows[0].gameObject);
                            chatWindows.RemoveAt(0);
                        }
                    }
                    var go = Instantiate(chatTextPrefab, chatTextParent);
                    textWindow = go.GetComponent<TextMeshProUGUI>();
                    go.name = go.name + chatTextParent.transform.childCount;
                    chatWindows.Add(go.GetComponent<TextMeshProUGUI>());
                }
                else
                {
                    chattext = textWindow.text;
                }
                if (chattext.Length > 0)
                    chattext += "\n";
                textWindow.text = chattext + line;
                textLines = combatLog.Count;
                if (scrollToBottom)
                {
                    scrollView.velocity = new Vector2(0, 1000f);
                    scrollView.verticalScrollbar.value = 0f;
                }
                if (scrollToBottom)
                {
                    scrollView.velocity = new Vector2(0, 1000f);
                }
            }
        }

        public void HandleAnnouncementMessage(Dictionary<string, object> props)
        {
            bool scrollToBottom = false;
            if (scrollView.verticalScrollbar.value == 0f)
                scrollToBottom = true;
            int proc = textLines - 1;
            if (proc < 1)
                proc = 1;
            if (textLines < 15)
                scrollView.verticalScrollbar.value = 0f;
            if (scrollView.verticalScrollbar.value <= 2f / proc)
                scrollToBottom = true;
            if (eventLog.Count > chatLimitRow)
            {
                while (eventLog.Count > chatLimitRow)
                    eventLog.RemoveAt(0);
            }
            string message = "(" + GetTime() + ")" + (string)props["AnnouncementText"];
            var line = GetLine(message, eventColor);
            if (line.Contains("<link=item#"))
            {
                string s = line.Substring(line.IndexOf("<link=item#") + 11);
                s = s.Substring(0, s.IndexOf(">"));
                int id = Int32.Parse(s);
                Inventory.Instance.GetItemByTemplateID(id);
            }
            if (line.Contains("<link=ability#"))
            {
                string s = line.Substring(line.IndexOf("<link=ability#") + 14);
                s = s.Substring(0, s.IndexOf(">"));
                int id = Int32.Parse(s);
                Abilities.Instance.GetAbility(id);
            }
            if (line.Contains("<size="))
            {
                while (line.Contains("<size="))
                {
                    string fs = line.Substring(0, line.IndexOf("<size"));
                    string s = line.Substring(line.IndexOf("<size"));
                    s = s.Substring(s.IndexOf(">") + 1);
                    line = fs + s;
                }
            }

            eventLog.Add(message);
            if (windowView == ChatWindowView.EventLog)
            {
                if (textWindow != null && combatLog.Count > chatLimitRow)
                {

                    if (chatWindows.Count == 1)
                    {
                        string _t = textWindow.text;
                        int count = _t.Split('\n').Length - 1;
                        for (int i = 0; i < count - chatLimitRow; i++)
                        {
                            _t = _t.Remove(0, _t.IndexOf("\n") + 1);
                        }
                        textWindow.text = _t;
                    }
                    else if (chatWindows.Count > 1)
                    {
                        int count = 0;
                        foreach (TextMeshProUGUI t in chatWindows)
                        {
                            count += t.text.Split('\n').Length - 1;
                        }
                        int todelete = count - chatLimitRow;
                        if (todelete > 0)
                        {
                            foreach (TextMeshProUGUI t in chatWindows)
                            {
                                string _t = t.text;
                                int maxcount = _t.Split('\n').Length - 1;
                                while (todelete > 0 && (((_t.Split('\n').Length - 1) > 0) || ((_t.Split('\n').Length - 1) == 0 && _t.Length > 0)))
                                {
                                    int idx = _t.IndexOf("\n");
                                    if (idx > 0)
                                        _t = _t.Remove(0, idx + 1);
                                    else
                                        _t = "";
                                    todelete--;
                                }
                                t.text = _t;
                            }
                        }
                    }
                }
                string chattext = "";
                if (textWindow == null || textWindow.text.Length > TextLengthPerTextObject)
                {

                    if (chatWindows.Count > chatLimitWindow - 1)
                    {
                        while (chatWindows.Count > chatLimitWindow - 1)
                        {
                            DestroyImmediate(chatWindows[0].gameObject);
                            chatWindows.RemoveAt(0);
                        }
                    }
                    var go = Instantiate(chatTextPrefab, chatTextParent);
                    textWindow = go.GetComponent<TextMeshProUGUI>();
                    go.name = go.name + chatTextParent.transform.childCount;
                    chatWindows.Add(go.GetComponent<TextMeshProUGUI>());
                }
                else
                {
                    chattext = textWindow.text;
                }
                if (chattext.Length > 0)
                    chattext += "\n";
                textWindow.text = chattext + line;
                textLines = eventLog.Count;
                if (scrollToBottom)
                {
                    scrollView.velocity = new Vector2(0, 1000f);
                }
            }
        }

        void ProcessInventoryEvent(string[] eventArgs)
        {
            if (eventArgs[0] == "ItemHarvested")
            {
                //    string[] args = new string[1];
                AtavismInventoryItem item = Inventory.Instance.GetItemByTemplateID(int.Parse(eventArgs[1]));
                if (eventLog.Count > chatLimitRow)
                    eventLog.RemoveAt(0);
#if AT_I2LOC_PRESET
            string message = GetLine("(" + GetTime() + ")" + I2.Loc.LocalizationManager.GetTranslation("Received") + " " + I2.Loc.LocalizationManager.GetTranslation("Items/" + item.name) + " x" + eventArgs[2],announcementColor);
#else
                string message = GetLine("(" + GetTime() + ")" + "Received " + item.name + " x" + eventArgs[2], announcementColor);
#endif
                eventLog.Add(message);
                if (windowView == ChatWindowView.EventLog)
                    textWindow.text = "\n" + message;
                textWindow.color = announcementColor;
            }
            else if (eventArgs[0] == "ItemLooted")
            {
                //    string[] args = new string[1];
                AtavismInventoryItem item = Inventory.Instance.GetItemByTemplateID(int.Parse(eventArgs[1]));
                if (eventLog.Count > chatLimitRow)
                    eventLog.RemoveAt(0);
#if AT_I2LOC_PRESET
            string line = GetLine(I2.Loc.LocalizationManager.GetTranslation("Received") + " " + I2.Loc.LocalizationManager.GetTranslation("Items/" + item.name) + " x" + eventArgs[2], announcementColor);
#else
                string line = GetLine("Received " + item.name + " x" + eventArgs[2], announcementColor);
#endif
                eventLog.Add(line);
                if (windowView == ChatWindowView.EventLog)
                    textWindow.text = "\n" + line;
            }
        }

        void ProcessCombatEvent(string[] eventArgs)
        {
            string casterName = GetObjectName(eventArgs[1]);
            string targetName = GetObjectName(eventArgs[2]);
#if AT_I2LOC_PRESET
        if (I2.Loc.LocalizationManager.GetTranslation("Mobs/" + casterName) != "") casterName = I2.Loc.LocalizationManager.GetTranslation("Mobs/" + casterName);
        if (I2.Loc.LocalizationManager.GetTranslation("Mobs/" + targetName) != "") targetName = I2.Loc.LocalizationManager.GetTranslation("Mobs/" + targetName);
#endif
            string message = "(" + GetTime() + ") ";
            if (eventArgs[0] == "CombatDamage")
            {
#if AT_I2LOC_PRESET
            message = casterName + " " + I2.Loc.LocalizationManager.GetTranslation("hit") + " " + targetName + " " + I2.Loc.LocalizationManager.GetTranslation("for") + " " + eventArgs[3] + " " + I2.Loc.LocalizationManager.GetTranslation(eventArgs[8])+ " " + I2.Loc.LocalizationManager.GetTranslation("damage") + ".";
#else
                message = casterName + " hit " + targetName + " for " + eventArgs[3] +" "+eventArgs[8]+ " damage.";
#endif
            }
          /*  else if (eventArgs[0] == "CombatMagicalDamage")
            {
#if AT_I2LOC_PRESET
            message = casterName + " " + I2.Loc.LocalizationManager.GetTranslation("hit") + " " + targetName + " " + I2.Loc.LocalizationManager.GetTranslation("for") + " " + eventArgs[3] + " " + I2.Loc.LocalizationManager.GetTranslation("magical damage") + ".";
#else
                message = casterName + " hit " + targetName + " for " + eventArgs[3] + " magical damage.";
#endif
            }*/
            else if (eventArgs[0] == "CombatDamageCritical")
            {
#if AT_I2LOC_PRESET
            message = casterName + " " + I2.Loc.LocalizationManager.GetTranslation("hit") + " " + targetName + " " + I2.Loc.LocalizationManager.GetTranslation("for") + " " + eventArgs[3] + " " + I2.Loc.LocalizationManager.GetTranslation(eventArgs[8])+ " " + I2.Loc.LocalizationManager.GetTranslation("critical damage") + ".";
#else
                message = casterName + " hit " + targetName + " for " + eventArgs[3] + " "+eventArgs[8]+" critical damage.";
#endif
            }
           /* else if (eventArgs[0] == "CombatMagicalCritical")
            {
#if AT_I2LOC_PRESET
            message = casterName + " " + I2.Loc.LocalizationManager.GetTranslation("hit") + " " + targetName + " " + I2.Loc.LocalizationManager.GetTranslation("for") + " " + eventArgs[3] + " " + I2.Loc.LocalizationManager.GetTranslation("magical damage") + ".";
#else
                message = casterName + " hit " + targetName + " for " + eventArgs[3] + " critical magical damage.";
#endif
            }*/
            else if (eventArgs[0] == "CombatBuffGained")
            {
#if AT_I2LOC_PRESET
            message = casterName + " " + I2.Loc.LocalizationManager.GetTranslation("gained") + " " + eventArgs[3] + ".";
#else
                message = targetName + " gained " + eventArgs[3] + ".";
#endif
            }
            else if (eventArgs[0] == "CombatBuffLost")
            {
                //   string effectName = "effect";
#if AT_I2LOC_PRESET
            message = casterName + " " + I2.Loc.LocalizationManager.GetTranslation("lost") + " " + eventArgs[3] + ".";
#else
                message = targetName + " lost " + eventArgs[3] + ".";
#endif
            }
            else if (eventArgs[0] == "CombatHeal")
            {
                //    string effectName = "effect";
                if (eventArgs[7] == AtavismCombat.Instance.HealthStat)
                {

#if AT_I2LOC_PRESET
                    message = casterName + " " + I2.Loc.LocalizationManager.GetTranslation("healed by") + " " + eventArgs[3] + ".";
#else
                    message = casterName + " healed by " + eventArgs[3] + ".";
#endif
                } else
                {

#if AT_I2LOC_PRESET
                    message = casterName + " " + I2.Loc.LocalizationManager.GetTranslation("restore")+ " " + I2.Loc.LocalizationManager.GetTranslation(eventArgs[7])+ " " + I2.Loc.LocalizationManager.GetTranslation("by") + " " + eventArgs[3] + ".";
#else
                    message = casterName + " restore " + eventArgs[7]+" by " + eventArgs[3] + ".";
#endif
                }

            }else if (eventArgs[0] == "CombatHealCritical")
            {
                //    string effectName = "effect";
                if (eventArgs[7] == AtavismCombat.Instance.HealthStat)
                {

#if AT_I2LOC_PRESET
                    message = casterName + " " + I2.Loc.LocalizationManager.GetTranslation("healed by") + " " + eventArgs[3] + ".";
#else
                    message = casterName + " healed by " + eventArgs[3] + ".";
#endif
                } else
                {

#if AT_I2LOC_PRESET
                    message = casterName + " " + I2.Loc.LocalizationManager.GetTranslation("restore")+ " " + I2.Loc.LocalizationManager.GetTranslation(eventArgs[7])+ " " + I2.Loc.LocalizationManager.GetTranslation("by") + " " + eventArgs[3] + ".";
#else
                    message = casterName + " restore " + eventArgs[7]+" by " + eventArgs[3] + ".";
#endif
                }

            }
            else if (eventArgs[0] == "CombatExpGained")
            {
                //    string effectName = "effect";
#if AT_I2LOC_PRESET
            message = I2.Loc.LocalizationManager.GetTranslation("You have received") + " " + eventArgs[3] +" "+ I2.Loc.LocalizationManager.GetTranslation("experience points") + ".";
#else
                message = "You have received " + eventArgs[3] + " experience points.";
#endif
            }
            else if (eventArgs[0] == "CombatMissed")
            {
#if AT_I2LOC_PRESET
              
                message =casterName+" "+ I2.Loc.LocalizationManager.GetTranslation("Missed");
#else
                message = casterName + " has Missed";
#endif
            }
            else if (eventArgs[0] == "CombatDodged")
            {
#if AT_I2LOC_PRESET
            
                message = targetName+" "+I2.Loc.LocalizationManager.GetTranslation("have Dodged");
#else
                message = targetName + " have Dodged";
#endif
            }
            else if (eventArgs[0] == "CombatBlocked")
            {
#if AT_I2LOC_PRESET
            
                message = targetName+" "+I2.Loc.LocalizationManager.GetTranslation("have Blocked");

#else
                message = targetName + " have Blocked";
#endif
            }
            else if (eventArgs[0] == "CombatParried")
            {
#if AT_I2LOC_PRESET
            
                message = targetName+" "+I2.Loc.LocalizationManager.GetTranslation("have Parried");

#else
                message = targetName + " have Parried";
#endif
            }
            else if (eventArgs[0] == "CombatEvaded")
            {
#if AT_I2LOC_PRESET
           
                message =targetName+" "+ I2.Loc.LocalizationManager.GetTranslation("have Evaded");
#else
                message = targetName + " have Evaded";
#endif
            }
            else if (eventArgs[0] == "CombatImmune")
            {
#if AT_I2LOC_PRESET
            
                message = targetName+" "+I2.Loc.LocalizationManager.GetTranslation("is Immune");
#else
                message = targetName + " is Immune";
#endif
            }
            else
            {
                Debug.Log("no message type " + eventArgs[0]);
            }

            AddCombatMessage(message);
        }

        string GetObjectName(string oid)
        {
            if(oid.Length > 0)
            if (OID.fromString(oid).ToLong() == ClientAPI.GetPlayerOid())
            {
#if AT_I2LOC_PRESET
            return I2.Loc.LocalizationManager.GetTranslation("You");
#else
                return "You";
#endif
            }
            else
            {
                AtavismObjectNode caster = ClientAPI.GetObjectNode(OID.fromString(oid).ToLong());
                if (caster != null)
                    return caster.Name;
            }
            return "";
        }

        public void OnEvent(AtavismEventData eData)
        {
            //   Debug.LogError(eData.eventType + " >" + eData.eventArgs[0] + "<");
            if (eData.eventType == "CHAT_MSG_SERVER")
            {
                AddChatMessage("(" + GetTime() + ") " + eData.eventArgs[0]);
            }
            else if (eData.eventType == "CHAT_MSG_SAY")
            {
                AtavismLogger.LogDebugMessage("Got chat say event with numargs: " + eData.eventArgs.Length);
                HandleChatMessage(eData.eventArgs); //Process chat message events
            }
            else if (eData.eventType == "CHAT_MSG_SYSTEM")
            {
                AtavismLogger.LogDebugMessage("Got system event with numargs: " + eData.eventArgs.Length);
#if AT_I2LOC_PRESET
            AddChatMessage(GetLine("(" + GetTime() + ") [" + I2.Loc.LocalizationManager.GetTranslation("Event") + "]: " + eData.eventArgs[0], announcementColor)); //System Event
#else
                AddChatMessage(GetLine("(" + GetTime() + ")[Event]: " + eData.eventArgs[0], announcementColor)); //System Event
#endif
            }
            else if (eData.eventType == "ADMIN_MESSAGE")
            {
                AtavismLogger.LogDebugMessage("Got system event with numargs: " + eData.eventArgs.Length);
#if AT_I2LOC_PRESET
            AddChatMessage(GetLine("(" + GetTime() + ") [" + I2.Loc.LocalizationManager.GetTranslation("Admin") + "]: " + eData.eventArgs[0], eventColor)); //System Event
            AddGroupMessage(GetLine("(" + GetTime() + ") [" + I2.Loc.LocalizationManager.GetTranslation("Admin") + "]: " + eData.eventArgs[0], eventColor)); //System Event
            AddGuildMessage(GetLine("(" + GetTime() + ") [" + I2.Loc.LocalizationManager.GetTranslation("Admin") + "]: " + eData.eventArgs[0], eventColor)); //System Event
#else
                AddChatMessage(GetLine("(" + GetTime() + ") [Admin]: " + eData.eventArgs[0], eventColor)); //System Event
                AddGroupMessage(GetLine("(" + GetTime() + ") [Admin]: " + eData.eventArgs[0], eventColor)); //System Event
                AddGuildMessage(GetLine("(" + GetTime() + ") [Admin]: " + eData.eventArgs[0], eventColor)); //System Event
#endif

            }
            else if (eData.eventType == "INVENTORY_EVENT")
            {
                AtavismLogger.LogDebugMessage("Got inventory event with numargs: " + eData.eventArgs.Length);
                // Process the message
                ProcessInventoryEvent(eData.eventArgs); //ProcessInventoryEvent
            }
            else if (eData.eventType == "COMBAT_EVENT")
            {
                AtavismLogger.LogDebugMessage("Got combat event with numargs: " + eData.eventArgs.Length);
                // Process the message
                ProcessCombatEvent(eData.eventArgs); //ProcessCombatEvent
            }
            else if (eData.eventType == "UPDATE_LANGUAGE")
            {
#if AT_I2LOC_PRESET
            switch (currentChannelCommand)
            {
                case "/say":
                    placeholderText = I2.Loc.LocalizationManager.GetTranslation("Say") + ":";
                    break;
                case "/group":
                    placeholderText = I2.Loc.LocalizationManager.GetTranslation("Group") + ":";
                    break;
                case "/guild":
                    placeholderText = I2.Loc.LocalizationManager.GetTranslation("Guild") + ":";
                    break;

                case "/1":
                    placeholderText = I2.Loc.LocalizationManager.GetTranslation("Instance General") + ":";
                    break;
                case "/2":
                    placeholderText = I2.Loc.LocalizationManager.GetTranslation("Global") + ":";
                    break;
                case "/admininfo":
                    placeholderText = I2.Loc.LocalizationManager.GetTranslation("Admin") + ":";
                    break;
                default:
                    if (currentChannelCommand.IndexOf("/whisper") > -1)
                    {
                        string[] splitMessage = currentChannelCommand.Split(' ');
                        placeholderText = I2.Loc.LocalizationManager.GetTranslation("Whisper") + " " + splitMessage[1] + ":";
                    }
                    break;
            }
#endif
            }

        }

        void HandleChatMessage(string[] args)
        {
            //Debug.Log("oid of chat sender: " + args[3]);
            if (args[2] == "4")
            {
#if AT_I2LOC_PRESET
            AddChatMessage(GetLine("(" + GetTime() + ") [" + I2.Loc.LocalizationManager.GetTranslation("Group") + "]: " + args[0], groupColor)); //Group Message
            AddGroupMessage(GetLine("(" + GetTime() + ") [" + I2.Loc.LocalizationManager.GetTranslation("Group") + "]: " + args[0], groupColor)); //Group Message
#else
                AddChatMessage(GetLine("(" + GetTime() + ") [Group] " + args[1] + ": " + args[0], groupColor)); //Group Message
                AddGroupMessage(GetLine("(" + GetTime() + ") [Group] " + args[1] + ": " + args[0], groupColor)); //Group Message
#endif
            }
            else if (args[2] == "5")
            {
#if AT_I2LOC_PRESET
            AddChatMessage(GetLine("(" + GetTime() + ") [" + I2.Loc.LocalizationManager.GetTranslation("Guild") + "] " + args[1] + ": " + args[0], guildColor)); //Guild Message
            AddGuildMessage(GetLine("(" + GetTime() + ") [" + I2.Loc.LocalizationManager.GetTranslation("Guild") + "] " + args[1] + ": " + args[0], guildColor)); //Guild Message
#else
                AddChatMessage(GetLine("(" + GetTime() + ") [Guild] " + args[1] + ": " + args[0], guildColor)); //Guild Message
                AddGuildMessage(GetLine("(" + GetTime() + ") [Guild] " + args[1] + ": " + args[0], guildColor)); //Guild Message
#endif
            }
            else if (args[2] == "1")
            {
                AddChatMessage(GetLine("(" + GetTime() + ") <link=user#" + args[1] + ">[" + args[1] + "]</link>: " + args[0], chatColor)); //Chat Message
            }
            else if (args[2] == "6")
            {
                AddChatMessage(GetLine("(" + GetTime() + ") <link=user#" + args[1] + ">[" + args[1] + "]</link>: " + args[0], whisperColor)); //Whisper Message
            }
            else if (int.Parse(args[2]) >= 10)
            {
                AddChatMessage(GetLine("(" + GetTime() + ") <link=user#" + args[1] + ">[" + args[1] + "]</link>:" + args[0], instanceColor)); //Some Other Message
            }
            else if (int.Parse(args[2]) == -1)
            {
                AddChatMessage(GetLine("(" + GetTime() + ") <link=user#" + args[1] + ">[" + args[1] + "]</link>:" + args[0], globalColor)); //Global chat Message
            }
        }

        public void StartWhisper(string targetName)
        { //TODO This is the whisper function I was talking about as well
            EventSystem.current.SetSelectedGameObject(input.gameObject);
            input.Select();
            input.ActivateInputField();

            string[] splitMessage = targetName.Split(' ');
            if (splitMessage.Length > 1)
            {
                currentChannelCommand = "/whisper \"" + targetName+"\"";
            }
            else
            {
                currentChannelCommand = "/whisper " + targetName;
            }
           
            placeholderText = "Whisper " + targetName + ":";
        }

        public void ViewChat()
        {
            chatButtonText.color = chatColorActive;
            combatButtonText.color = defaultColorActive;
            eventButtonText.color = defaultColorActive;
            guildButtonText.color = defaultColorActive;
            groupButtonText.color = defaultColorActive;
            if (chatButton != null)
            {
                Color c = chatButton.targetGraphic.color;
                c.a = 1f;
                chatButton.targetGraphic.color = c;
            }
            if (combatButton != null)
            {
                Color c = combatButton.targetGraphic.color;
                c.a = 0f;
                combatButton.targetGraphic.color = c;
            }
            if (eventButton != null)
            {
                Color c = eventButton.targetGraphic.color;
                c.a = 0f;
                eventButton.targetGraphic.color = c;
            }
            if (guildButton != null)
            {
                Color c = guildButton.targetGraphic.color;
                c.a = 0f;
                guildButton.targetGraphic.color = c;
            }
            if (groupButton != null)
            {
                Color c = groupButton.targetGraphic.color;
                c.a = 0f;
                groupButton.targetGraphic.color = c;
            }

            windowView = ChatWindowView.Chat;
            foreach (Transform child in chatTextParent)
            {
                GameObject.Destroy(child.gameObject);
            }
            chatWindows.Clear();
            var go = Instantiate(chatTextPrefab, chatTextParent);
            textWindow = go.GetComponent<TextMeshProUGUI>();
            chatWindows.Add(go.GetComponent<TextMeshProUGUI>());
            foreach (string message in chatMessages)
            {
                if (textWindow.text.Length > TextLengthPerTextObject)
                {
                    go = Instantiate(chatTextPrefab, chatTextParent);
                    go.name = go.name + chatTextParent.transform.childCount;
                    textWindow = go.GetComponent<TextMeshProUGUI>();
                    chatWindows.Add(go.GetComponent<TextMeshProUGUI>());
                }
                textWindow.text += "\n" + message;
            }
            currentChannelCommand = "/say";
#if AT_I2LOC_PRESET
        placeholderText = I2.Loc.LocalizationManager.GetTranslation("Say") + ":";
#else
            placeholderText = "Say:";
#endif
            currentChannelCommandColor = chatColor;
            scrollView.verticalScrollbar.value = 0f;
        }

        public void ViewGuild()
        {
            chatButtonText.color = defaultColorActive;
            combatButtonText.color = defaultColorActive;
            eventButtonText.color = defaultColorActive;
            guildButtonText.color = guildColorActive;
            groupButtonText.color = defaultColorActive;
            if (chatButton != null)
            {
                Color c = chatButton.targetGraphic.color;
                c.a = 0f;
                chatButton.targetGraphic.color = c;
            }
            if (combatButton != null)
            {
                Color c = combatButton.targetGraphic.color;
                c.a = 0f;
                combatButton.targetGraphic.color = c;
            }
            if (eventButton != null)
            {
                Color c = eventButton.targetGraphic.color;
                c.a = 0f;
                eventButton.targetGraphic.color = c;
            }
            if (guildButton != null)
            {
                Color c = guildButton.targetGraphic.color;
                c.a = 1f;
                guildButton.targetGraphic.color = c;
            }
            if (groupButton != null)
            {
                Color c = groupButton.targetGraphic.color;
                c.a = 0f;
                groupButton.targetGraphic.color = c;
            }
            windowView = ChatWindowView.Guild;
            foreach (Transform child in chatTextParent)
            {
                GameObject.Destroy(child.gameObject);
            }
            chatWindows.Clear();
            var go = Instantiate(chatTextPrefab, chatTextParent);
            textWindow = go.GetComponent<TextMeshProUGUI>();
            chatWindows.Add(go.GetComponent<TextMeshProUGUI>());
            foreach (string message in guildLog)
            {
                if (textWindow.text.Length > TextLengthPerTextObject)
                {
                    go = Instantiate(chatTextPrefab, chatTextParent);
                    go.name = go.name + chatTextParent.transform.childCount;
                    textWindow = go.GetComponent<TextMeshProUGUI>();
                    chatWindows.Add(go.GetComponent<TextMeshProUGUI>());
                }
                textWindow.text += "\n" + message;
            }
            currentChannelCommand = "/guild";
#if AT_I2LOC_PRESET
       placeholderText = I2.Loc.LocalizationManager.GetTranslation("Guild") + ":";
#else
            placeholderText = "Guild:";
#endif
            currentChannelCommandColor = chatColor;
            scrollView.verticalScrollbar.value = 0f;
        }

        public void ViewEventLog()
        {
            chatButtonText.color = defaultColorActive;
            combatButtonText.color = defaultColorActive;
            eventButtonText.color = eventColorActive;
            guildButtonText.color = defaultColorActive;
            groupButtonText.color = defaultColorActive;
            if (chatButton != null)
            {
                Color c = chatButton.targetGraphic.color;
                c.a = 0f;
                chatButton.targetGraphic.color = c;
            }
            if (combatButton != null)
            {
                Color c = combatButton.targetGraphic.color;
                c.a = 0f;
                combatButton.targetGraphic.color = c;
            }
            if (eventButton != null)
            {
                Color c = eventButton.targetGraphic.color;
                c.a = 1f;
                eventButton.targetGraphic.color = c;
            }
            if (guildButton != null)
            {
                Color c = guildButton.targetGraphic.color;
                c.a = 0f;
                guildButton.targetGraphic.color = c;
            }
            if (groupButton != null)
            {
                Color c = groupButton.targetGraphic.color;
                c.a = 0f;
                groupButton.targetGraphic.color = c;
            }
            windowView = ChatWindowView.EventLog;
            foreach (Transform child in chatTextParent)
            {
                GameObject.Destroy(child.gameObject);
            }
            chatWindows.Clear();
            var go = Instantiate(chatTextPrefab, chatTextParent);
            textWindow = go.GetComponent<TextMeshProUGUI>();
            chatWindows.Add(go.GetComponent<TextMeshProUGUI>());
            foreach (string message in eventLog)
            {
                if (textWindow.text.Length > TextLengthPerTextObject)
                {
                    go = Instantiate(chatTextPrefab, chatTextParent);
                    go.name = go.name + chatTextParent.transform.childCount;
                    textWindow = go.GetComponent<TextMeshProUGUI>();
                    chatWindows.Add(go.GetComponent<TextMeshProUGUI>());
                }
                textWindow.text += "\n" + message;
            }
            scrollView.verticalScrollbar.value = 0f;
        }

        public void ViewCombatLog()
        {
            chatButtonText.color = defaultColorActive;
            combatButtonText.color = combatColorActive;
            eventButtonText.color = defaultColorActive;
            guildButtonText.color = defaultColorActive;
            groupButtonText.color = defaultColorActive;
            if (chatButton != null)
            {
                Color c = chatButton.targetGraphic.color;
                c.a = 0f;
                chatButton.targetGraphic.color = c;
            }
            if (combatButton != null)
            {
                Color c = combatButton.targetGraphic.color;
                c.a = 1f;
                combatButton.targetGraphic.color = c;
            }
            if (eventButton != null)
            {
                Color c = eventButton.targetGraphic.color;
                c.a = 0f;
                eventButton.targetGraphic.color = c;
            }
            if (guildButton != null)
            {
                Color c = guildButton.targetGraphic.color;
                c.a = 0f;
                guildButton.targetGraphic.color = c;
            }
            if (groupButton != null)
            {
                Color c = groupButton.targetGraphic.color;
                c.a = 0f;
                groupButton.targetGraphic.color = c;
            }
            windowView = ChatWindowView.CombatLog;
            foreach (Transform child in chatTextParent)
            {
                GameObject.Destroy(child.gameObject);
            }
            chatWindows.Clear();
            var go = Instantiate(chatTextPrefab, chatTextParent);
            textWindow = go.GetComponent<TextMeshProUGUI>();
            chatWindows.Add(go.GetComponent<TextMeshProUGUI>());
            foreach (string message in combatLog)
            {
                if (textWindow.text.Length > TextLengthPerTextObject)
                {
                    go = Instantiate(chatTextPrefab, chatTextParent);
                    go.name = go.name + chatTextParent.transform.childCount;
                    textWindow = go.GetComponent<TextMeshProUGUI>();
                    chatWindows.Add(go.GetComponent<TextMeshProUGUI>());
                }
                textWindow.text += "\n" + message;
            }
            scrollView.verticalScrollbar.value = 0f;
        }

        public void ViewGroupLog()
        {
            chatButtonText.color = defaultColorActive;
            combatButtonText.color = defaultColorActive;
            eventButtonText.color = defaultColorActive;
            guildButtonText.color = defaultColorActive;
            groupButtonText.color = groupColorActive;
            if (chatButton != null)
            {
                Color c = chatButton.targetGraphic.color;
                c.a = 0f;
                chatButton.targetGraphic.color = c;
            }
            if (combatButton != null)
            {
                Color c = combatButton.targetGraphic.color;
                c.a = 0f;
                combatButton.targetGraphic.color = c;
            }
            if (eventButton != null)
            {
                Color c = eventButton.targetGraphic.color;
                c.a = 0f;
                eventButton.targetGraphic.color = c;
            }
            if (guildButton != null)
            {
                Color c = guildButton.targetGraphic.color;
                c.a = 0f;
                guildButton.targetGraphic.color = c;
            }
            if (groupButton != null)
            {
                Color c = groupButton.targetGraphic.color;
                c.a = 1f;
                groupButton.targetGraphic.color = c;
            }
            windowView = ChatWindowView.Group;
            foreach (Transform child in chatTextParent)
            {
                GameObject.Destroy(child.gameObject);
            }
            chatWindows.Clear();
            var go = Instantiate(chatTextPrefab, chatTextParent);
            textWindow = go.GetComponent<TextMeshProUGUI>();
            chatWindows.Add(go.GetComponent<TextMeshProUGUI>());
            foreach (string message in groupLog)
            {
                if (textWindow.text.Length > TextLengthPerTextObject)
                {
                    go = Instantiate(chatTextPrefab, chatTextParent);
                    go.name = go.name + chatTextParent.transform.childCount;
                    textWindow = go.GetComponent<TextMeshProUGUI>();
                    chatWindows.Add(go.GetComponent<TextMeshProUGUI>());
                }
                textWindow.text += "\n" + message;
            }
            currentChannelCommand = "/group";
#if AT_I2LOC_PRESET
        placeholderText = I2.Loc.LocalizationManager.GetTranslation("Group") + ":";
#else
            placeholderText = "Group:";
#endif
            currentChannelCommandColor = chatColor;
            scrollView.verticalScrollbar.value = 0f;
        }



        string GetLine(string message, Color color)
        {
            return "<color=" + ColorTypeConverter.ToRGBHex(color) + ">" + message + "</color>";
        }

        public void ChatHelp()
        {
#if AT_I2LOC_PRESET
        string[] helps = { I2.Loc.LocalizationManager.GetTranslation("Chat commands:"),
            "/help "+I2.Loc.LocalizationManager.GetTranslation("or")+" /h" + I2.Loc.LocalizationManager.GetTranslation("HelpHelp"),
            "/say "+I2.Loc.LocalizationManager.GetTranslation("or")+" /s " + I2.Loc.LocalizationManager.GetTranslation("HelpSay"),
            "/group "+I2.Loc.LocalizationManager.GetTranslation("or")+" /p " + I2.Loc.LocalizationManager.GetTranslation("HelpGroup"),
            "/guild "+I2.Loc.LocalizationManager.GetTranslation("or")+" /g " + I2.Loc.LocalizationManager.GetTranslation("HelpGuild"),
            "/whisper "+I2.Loc.LocalizationManager.GetTranslation("or")+" /w " + I2.Loc.LocalizationManager.GetTranslation("HelpWhisper"),
            "/1 "+I2.Loc.LocalizationManager.GetTranslation("HelpInstanceGeneral"),
            "/2 "+I2.Loc.LocalizationManager.GetTranslation("HelpGlobal"),
        };
#else
            string[] helps = { "Chat commands:",
            "/help or /h This help",
            "/say or /s Send message localy",
            "/group or /p Send message to Group",
            "/guild or /g Send message to Guild",
            "/whisper or /w \"Player Name\" Send private mesage",
            "/1 Send message to whole instance",
            "/2 Send message Globaly",
        };

#endif
            foreach (string message in helps)
            {
                chatMessages.Add(GetLine(message, chatColor));
                if (windowView == ChatWindowView.Chat)
                {
                    textWindow.text += "\n" + GetLine(message, chatColor);
                }
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            isHoveringObject = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHoveringObject = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isHoveringObject)
            {
                //     bool linkseleced = false;
                foreach (Transform child in chatTextParent)
                {
                    TextMeshProUGUI _textWindow = child.GetComponent<TextMeshProUGUI>();
                    // Check if mouse intersects with any links.
                    int linkIndex = TMP_TextUtilities.FindIntersectingLink(_textWindow, Input.mousePosition, m_Camera);
                    // Clear previous link selection if one existed.
                    if ((linkIndex == -1 && m_selectedLink != -1 && m_selectedWindow != null && (m_selectedWindow == _textWindow)) || (linkIndex != m_selectedLink && (m_selectedWindow == _textWindow)))
                    {
                        UGUITooltip.Instance.Hide();
                        m_selectedLink = -1;
                        m_selectedWindow = null;
                    }

                    // Handle new Link selection.
                    if (linkIndex != -1 && linkIndex != m_selectedLink && (m_selectedWindow == null || m_selectedWindow == _textWindow))
                    {
                        m_selectedLink = linkIndex;
                        m_selectedWindow = _textWindow;
                        TMP_LinkInfo linkInfo = _textWindow.textInfo.linkInfo[linkIndex];
                        Vector3 worldPointInRectangle = Vector3.zero;
                        RectTransformUtility.ScreenPointToWorldPointInRectangle(_textWindow.rectTransform, Input.mousePosition, m_Camera, out worldPointInRectangle);
                        string link = linkInfo.GetLinkID();
                        char[] dd = { '#' };
                        string[] pLink = link.Split(dd);
                        switch (pLink[0])
                        {
                            case "user":
                                break;
                        }
                    }
                }
            }
        }



        void LateUpdate()
        {
            if (isHoveringObject)
            {
                foreach (Transform child in chatTextParent)
                {
                    TextMeshProUGUI _textWindow = child.GetComponent<TextMeshProUGUI>();
                    // Check if mouse intersects with any links.
                    int linkIndex = TMP_TextUtilities.FindIntersectingLink(_textWindow, Input.mousePosition, m_Camera);
                    if ((linkIndex == -1 && m_selectedLink != -1 && m_selectedWindow != null && (m_selectedWindow == _textWindow)) || (linkIndex != m_selectedLink && (m_selectedWindow == _textWindow)))
                    {
                        UGUITooltip.Instance.Hide();
                        m_selectedLink = -1;
                        m_selectedWindow = null;
                    }

                    // Handle new Link selection.
                    if (linkIndex != -1 && linkIndex != m_selectedLink && (m_selectedWindow == null || m_selectedWindow == _textWindow))
                    {
                        m_selectedLink = linkIndex;
                        m_selectedWindow = _textWindow;
                        TMP_LinkInfo linkInfo = _textWindow.textInfo.linkInfo[linkIndex];
                        Vector3 worldPointInRectangle = Vector3.zero;
                        RectTransformUtility.ScreenPointToWorldPointInRectangle(_textWindow.rectTransform, Input.mousePosition, m_Camera, out worldPointInRectangle);
                        string link = linkInfo.GetLinkID();
                        char[] dd = { '#' };
                        string[] pLink = link.Split(dd);
                        switch (pLink[0])
                        {
                            case "ability":
                                AtavismAbility ability = Abilities.Instance.GetAbility(Int32.Parse(pLink[1]));
                                ability.ShowTooltip(_textWindow.gameObject);
                                break;
                            case "item":
                                AtavismInventoryItem item = Inventory.Instance.GetItemByTemplateID(Int32.Parse(pLink[1]));
                                item.ShowTooltip(_textWindow.gameObject);
                                break;
                            case "user":
                                break;
                        }
                    }
                }
            }
        }
        private string GetTime()
        {
            string hour = DateTime.Now.Hour.ToString(); //= TimeManager.Instance.Hour.ToString();
            if (DateTime.Now.Hour < 10)
            {
                hour = "0" + hour;
            }
            string minute = DateTime.Now.Minute.ToString();//= TimeManager.Instance.Minute.ToString();
            if (DateTime.Now.Minute < 10)
            {
                minute = "0" + minute;
            }
            return hour + ":" + minute;
        }

        public static UGUIChatController Instance
        {
            get
            {
                return instance;
            }
        }

    }
}