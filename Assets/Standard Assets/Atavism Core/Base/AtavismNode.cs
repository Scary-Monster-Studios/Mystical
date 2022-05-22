using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Atavism
{

    /// <summary>
    /// An intermediary class that assists between standard Unity Objects and AtavismObjectNodes (which are from the server).
    /// The AtavismNode gets added automatically to all objects from the server and should be manually added
    /// to all objects added locally.
    /// </summary>
    public class AtavismNode : MonoBehaviour
    {

        AtavismObjectNode node;
        Dictionary<string, object> localProps = new Dictionary<string, object>();

        public bool showParam = false;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (showParam)
            {
                
                showParam = false;
                if (node != null)
                {
                    ClientAPI.Write("Properties for: " + name + " (" + Oid + ") with num Properties: " + node.PropertyNames.Count);
                    foreach (string prop in node.PropertyNames)
                        ClientAPI.Write(prop + ": " + node.GetProperty(prop));
                }
            }
        }

        public void AddLocalProperty(string propertyName, object propertyValue)
        {
            localProps[propertyName] = propertyValue;
        }

        public void RemoveLocalProperty(string propertyName)
        {
            localProps.Remove(propertyName);
        }

        public bool PropertyExists(string propertyName)
        {
            if (localProps.ContainsKey(propertyName))
            {
                return true;
            }
            else if (node != null)
            {
                return node.PropertyExists(propertyName);
            }
            else
            {
                return false;
            }
        }

        public object GetProperty(string propertyName)
        {
            if (localProps != null && localProps.ContainsKey(propertyName))
            {
                return localProps[propertyName];
            }
            else if (node != null)
            {
                return node.GetProperty(propertyName);
            }
            else
            {
                return null;
            }
        }

        public bool CheckBooleanProperty(string propName)
        {
            if (node != null)
            {
                return node.CheckBooleanProperty(propName);
            }
            else
            {
                if (PropertyExists(propName))
                {
                    object val = GetProperty(propName);
                    if (val is bool)
                    {
                        return (bool)val;
                    }
                }
                return false;
            }
        }

        public void RegisterObjectPropertyChangeHandler(string propertyName, PropertyChangeEventHandler handler)
        {
            if (node != null)
            {
                node.RegisterPropertyChangeHandler(propertyName, handler);
            }
            else
            {
                //   Debug.LogError("AtavismObjectNode is null");
            }
        }

        public void RemoveObjectPropertyChangeHandler(string propertyName, PropertyChangeEventHandler handler)
        {
            if (node != null)
            {
                node.RemovePropertyChangeHandler(propertyName, handler);
            }
            else
            {
                //   Debug.LogError("AtavismObjectNode is null");
            }
        }

        public void SetMobController(AtavismMobController mobController)
        {
            node.MobController = mobController;
        }

        public void SetNodeData(AtavismObjectNode node)
        {
            this.node = node;
            // Send broadcast to let the components know it can register property handlers etc.
            gameObject.BroadcastMessage("ObjectNodeReady", SendMessageOptions.DontRequireReceiver);
        }

        public void ReplaceGameObject(GameObject replacement)
        {
            node.GameObject = replacement;
        }

        public bool IsPlayer()
        {
            if (node is AtavismPlayer)
                return true;
            else
                return false;
        }

        public long Oid
        {
            get
            {
                if(node!=null)
                    return node.Oid;
                return 0L;
            }
        }

        public Vector3 Position
        {
            get
            {
                if (node != null)
                {
                    return node.Position;
                }
                else
                {
                    return gameObject.transform.position;
                }
            }
        }

        public void despawn()
        {
        }

        public GameObject GameObject
        {
            get
            {
                return gameObject;
            }
        }
    }
}