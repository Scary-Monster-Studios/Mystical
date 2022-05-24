using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
public class RichChatMessage : PoolObject, IPointerClickHandler {

    public DateTime timeStamp;
    public string senderID;
    public string message;
    public string messageType;
    public Text richTextbox;
    private List<GameObject> elements;
    
    public RichChatMessage Set(string senderID, string messageType, string message)
    {
        this.timeStamp = DateTime.Now;
        this.senderID = senderID;
        this.message = message;
        this.messageType = messageType;
        this.richTextbox.text = message;
        return this;
    }

    public void AddPrefixIcons(List<Image> icons)
    {
        if (elements == null)
            elements = new List<GameObject>();
        for(int i = 0; i < elements.Count; i++)
        {
            elements[i].transform.SetParent(null);
            elements[i].gameObject.SetActive(false);
            //DestroyImmediate(elements[i]);
        }
        elements.Clear();
        for(int i = 0; i < icons.Count; i++)
        {
            icons[i].transform.SetParent(this.transform);
            icons[i].transform.SetSiblingIndex(this.transform.childCount - 2);
            icons[i].gameObject.SetActive(true);
            elements.Add(icons[i].gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Debug.Log("Left click");
        else if (eventData.button == PointerEventData.InputButton.Middle)
            Debug.Log("Middle click");
        else if (eventData.button == PointerEventData.InputButton.Right)
            Debug.Log("Right click");        
    }

    public override void MakeAvailable()
    {
        base.MakeAvailable();
        /*
        //this.richTextbox.text = "";
        if (elements == null)
            return;

        for(int i = 0; i < elements.Count; i++)
        {
            Destroy(elements[i]);
        }

        elements.Clear();
        */
    }
}
