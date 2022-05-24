using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RichChatBox : MonoBehaviour {

    [SerializeField] ContentSizeFitter contentSizeFitter;
    [SerializeField] Transform messageParentPanel;
    [SerializeField] RichChatMessage prefabRichChatMessage;
    [SerializeField] int maxLineCount = 200;    
    [SerializeField] char prefixSeperator = ':';
    [SerializeField] char iconSeperator = '/';
    [SerializeField] List<PoolObject> prefixIcons;

    private int index = 0;
    private ObjectPool messagePool;
    private RichChatMessage message;
    private List<PoolObject> messages;
    private Dictionary<string, ObjectPool> iconDict;

    void Awake()
    {
        if (messagePool == null)
        {
            prefabRichChatMessage.gameObject.hideFlags = HideFlags.HideInHierarchy;
            messagePool = new ObjectPool(prefabRichChatMessage, maxLineCount, maxLineCount, this.transform);
            messages = new List<PoolObject>();
            messages = messagePool.RetrieveAll();
            for (int i = 0; i < messages.Count; i++)//
            {
                messages[i].gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
        }

        if (iconDict == null)
        {
            iconDict = new Dictionary<string, ObjectPool>();
            for (int i = 0; i < prefixIcons.Count; i++)
            {
                iconDict.Add(prefixIcons[i].gameObject.name, new ObjectPool(prefixIcons[i]));
            }
        }
    }

    public void ClearAll()
    {
        for(int i = 0; i < messages.Count; i++)
        {
            messages[i].MakeAvailable();
        }
    }

    public void AddMessage(string senderId, string messageType, string messageText)
    {
        if (messagePool == null)
            Awake();

        message = messagePool.Recycle() as RichChatMessage;
        if (message != null)
        {
            //Set the text that "clean" will work on.
            message.message = messageText;
            
            //Remove and replace key phrases
            SetPrefixIcons(message);

            //Setup the message with updated text.
            message.Set(senderId, messageType, message.message);
            message.gameObject.SetActive(true);
            message.transform.SetParent(messageParentPanel);
            message.transform.SetSiblingIndex(messageParentPanel.childCount - 1);
        }
    }

    public string GetPrefix(string text)
    {
        var index = text.IndexOf(prefixSeperator);
        if (index < 0 || index >= text.Length)
            return ":";
        return text.Substring(0, index) + prefixSeperator;
    }

    public string RemovePrefix(string text)
    {
        var index = text.IndexOf(prefixSeperator);
        if (index < 0 || index >= text.Length)
            return text;
        return text.Substring(index + 1, text.Length - (index + 1));
    }

    void SetPrefixIcons(RichChatMessage message)
    {
        var text = message.message;
        var index = text.IndexOf(prefixSeperator);
        var prefix = text.Substring(0, index);
        message.message = text.Substring(index + 1, text.Length - (index + 1));

        if (prefix.Length <= 0)
            return;

        var split = prefix.Split(iconSeperator);
        List<Image> icons = new List<Image>();
        for(int i = 0; i < split.Length; i++)
        {
            var inst = GetIcon(split[i]);
            if(inst != null)
            {
                icons.Add(inst);
            }
        }

        message.AddPrefixIcons(icons);
    }

    public Image GetIcon(string name)
    {
        if (iconDict != null)
        {
            if (iconDict.ContainsKey(name))
                return iconDict[name].Retrieve().GetComponent<Image>();
            else
                return null;
        }

        for (int i = 0; i < prefixIcons.Count; i++)
        {
            if (prefixIcons[i].gameObject.name == name)
                return prefixIcons[i].GetComponent<Image>();
        }

        return null;
    }
}
