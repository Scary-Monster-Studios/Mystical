using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atavism
{

    public class AtavismGlobalEvents : MonoBehaviour
    {
        static AtavismGlobalEvents instance;
        private List<GlobalEvent> list = new List<GlobalEvent>();

        // Start is called before the first frame update
        void Start()
        {
            if (instance != null)
            {
                return;
            }

            instance = this;

            NetworkAPI.RegisterExtensionMessageHandler("GlobalEventList", HandleGlobalEvents);
        }
        void ClientReady()
        {
            AtavismLogger.LogDebugMessage("Global Events ClientReady");
            AtavismClient.Instance.NetworkHelper.RegisterPrefabMessageHandler("GlobalEventIcons", HandleGlobalEventsData);
          }
        private void OnDestroy()
        {
            NetworkAPI.RemoveExtensionMessageHandler("GlobalEventList", HandleGlobalEvents);
            AtavismClient.Instance.NetworkHelper.RemovePrefabMessageHandler("GlobalEventIcons", HandleGlobalEventsData);
        }

        private void HandleGlobalEvents(Dictionary<string, object> props)
        {
          //  Debug.LogError("HandleGlobalEvents");
            list.Clear();
            int num = (int) props["num"];
       //     Debug.LogError("HandleGlobalEvents "+num);

            for (int i = 0; i < num; i++)
            {
                GlobalEvent gv = new GlobalEvent();
                gv.id = (int) props["id" + i];
                gv.name = (string) props["name" + i];
                gv.description = (string) props["desc" + i];
              //  Debug.LogError("HandleGlobalEvents "+gv.id+" "+gv.name+" "+gv.description);
                var ge = AtavismPrefabManager.Instance.LoadGlobaEventsData(gv.id);
                list.Add(gv);
            }
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("GLOABL_EVENTS_UPDATE", args);
         //   Debug.LogError("HandleGlobalEvents END");

        }
        
   public void HandleGlobalEventsData(Dictionary<string, object> props)
        {
        //   Debug.LogError("HandleGlobalEventsData");
            try
            {
                int num = (int)props["num"];
                bool sendAll = (bool)props["all"];
                for (int i = 0; i < num; i++)
                {
                    int id = (int)props["i" + i + "id"];
                    long date = (long)props["i" + i + "date"];
                    string icon = (string)props["i" + i + "icon"];
                    string icon2 = (string)props["i" + i + "icon2"];
                    Texture2D tex = new Texture2D(2, 2);
                    bool wyn = tex.LoadImage(System.Convert.FromBase64String(icon2));
                    Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);

                    AtavismPrefabManager.Instance.SaveGlobalEventIcon(id, sprite, icon2, date);
                }
                
                if (props.ContainsKey("toRemove"))
                {
                    string keys = (string)props["toRemove"];
                    if (keys.Length > 0)
                    {
                        string[] _keys = keys.Split(';');
                        foreach (string k in _keys)
                        {
                            if (k.Length > 0)
                            {
                                AtavismPrefabManager.Instance.DeleteGlobalEvent(int.Parse(k));
                            }
                        }
                    }
                }

                if (sendAll)
                {
                   
                    string[] args = new string[1];
                    AtavismEventSystem.DispatchEvent("GLOABL_EVENTS_ICON", args);
                    AtavismPrefabManager.Instance.reloaded++;
                    if(AtavismLogger.logLevel <= LogLevel.Debug) 
                    Debug.Log("All data received. Running Queued Global Events update message.");
                }
                else
                {
                    AtavismPrefabManager.Instance.LoadGlobaEventsData();
                    string[] args = new string[1];
                    AtavismEventSystem.DispatchEvent("GLOABL_EVENTS_ICON", args);
                    Debug.LogWarning("Not Running Queued Item update message. Not all data sent");
                    
                }
            }
            catch (System.Exception e)
            {
                AtavismLogger.LogError("Exception loading ability prefab data " + e);
            }
         //   Debug.LogError("AbilitiesPrefabHandler End");
        }
     

        public List<GlobalEvent> List
        {
            get
            {
                return list;
             }
        }

        public static AtavismGlobalEvents Instance
        {
            get
            {
                return instance;
            }
        }

    }

    [Serializable]
    public class GlobalEvent
    {
        public string name;
        public string description;
        public int id;
    }
}