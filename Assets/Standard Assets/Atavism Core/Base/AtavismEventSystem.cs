using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

namespace Atavism
{

    public struct AtavismEventData
    {
        public string eventType;
        public string[] eventArgs;
    }

    public class AtavismEventSystem
    {

        // Dictionary of event names and subscribers
        public static Dictionary<string, List<Component>> eventRegistry =
                new Dictionary<string, List<Component>>();

        public static void RegisterEvent(string eventName, Component go)
        {
            Monitor.Enter(eventRegistry);
            try
            {

                if (!eventRegistry.ContainsKey(eventName))
                    eventRegistry[eventName] = new List<Component>();
                if (!eventRegistry[eventName].Contains(go))
                    eventRegistry[eventName].Add(go);
            }
            finally
            {
                Monitor.Exit(eventRegistry);
            }
        }

        public static void UnregisterEvent(string eventName, Component frame)
        {
            Monitor.Enter(eventRegistry);
            try
            {
                if (!eventRegistry.ContainsKey(eventName))
                    return;
                if (eventRegistry[eventName].Contains(frame))
                    eventRegistry[eventName].Remove(frame);
            }
            finally
            {
                Monitor.Exit(eventRegistry);
            }
        }

        public static void DispatchEvent(string eventName, string[] eventArgs)
        {
            if (!eventRegistry.ContainsKey(eventName))
            {
                return;
            }
            Monitor.Enter(eventRegistry);
            try
            {
                List<Component> sub = eventRegistry[eventName];
                // Create an EventData object to pass to the subscribers
                List<Component> subscribers = new List<Component>(sub);
               AtavismEventData eData = new AtavismEventData();
                eData.eventType = eventName;
                eData.eventArgs = eventArgs;
                // subscribers.GetEnumerator().
                foreach (Component go in subscribers)
                {
                    try
                    {
                        go.SendMessage("OnEvent", eData, SendMessageOptions.DontRequireReceiver);
                    }
                    catch (Exception ex)
                    {
                       // Debug.LogError("Exception in event handler: " + ex + " with arg: " + eventArgs[0]);
                        AtavismLogger.LogWarning("Exception in event handler: " + ex + " with arg: " + eventArgs[0]);
                    }
                }
            }
            finally
            {
                Monitor.Exit(eventRegistry);
            }
        }
    }
}