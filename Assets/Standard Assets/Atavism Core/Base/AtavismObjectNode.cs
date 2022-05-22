using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Atavism
{


    public class PropertyChangeEventArgs : EventArgs
    {
        protected string propName;

        public PropertyChangeEventArgs(string name)
        {
            propName = name;
        }

        public string PropertyName
        {
            get
            {
                return propName;
            }
        }
    }

    public delegate void ObjectNodeDisposed(AtavismObjectNode objNode);

    public delegate void PositionChangeEventHandler(object sender, EventArgs args);

    public delegate void OrientationChangeEventHandler(object sender, EventArgs args);

    public delegate void PropertyChangeEventHandler(object sender, PropertyChangeEventArgs args);

    public class AtavismObjectNode
    {
        protected long oid = 0;
        protected GameObject gameObject;
        protected AtavismMobController mobController;
        protected AtavismWorldManager worldManager;
        protected ObjectNodeType objType;
        protected GameObject parent;
        protected bool followTerrain;
        protected bool targetable;
        protected long lastLocTimestamp = -1;
        public bool createdGo = false;
        private Dictionary<string, object> properties = new Dictionary<string, object>();
        // Handlers for object property change events
        Dictionary<string, List<PropertyChangeEventHandler>> propertyChangeHandlers = new Dictionary<string, List<PropertyChangeEventHandler>>();

        public event PositionChangeEventHandler PositionChange;
        public event OrientationChangeEventHandler OrientationChange;
        public event PropertyChangeEventHandler PropertyChange;

        public AtavismObjectNode(long oid, string name, ObjectNodeType objType, AtavismWorldManager worldManager)
        {
            this.oid = oid;
            Init(name, objType, worldManager);
        }

        protected virtual void OnPositionChange()
        {
            PositionChangeEventHandler handler = PositionChange;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        protected virtual void OnOrientationChange()
        {
            OrientationChangeEventHandler handler = OrientationChange;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        protected void Init(string name, ObjectNodeType objType, AtavismWorldManager worldManager)
        {
            gameObject = new GameObject(name);
            this.objType = objType;
            this.worldManager = worldManager;
            this.Targetable = false;

            //attachmentPoints = new Dictionary<string, AttachmentPoint> ();
            //soundSources = new List<SoundSource> ();
        }

        /// <summary>
        ///   Sets up the data needed for interpolated movement of an object.
        ///   In this case, it is the direction, location, and timestamp when 
        ///   that location was the current location.
        /// </summary>
        /// <param name="timestamp">time that the message was created (in client time)</param>
        /// <param name="dir">direction of motion of the object</param>
        /// <param name="loc">initial location of the object (at the time the message was created)</param>
        public virtual void SetDirLoc(long timestamp, Vector3 dir, Vector3 pos)
        {
            SetLoc(timestamp, pos);
        }

        protected void SetLoc(long timestamp, Vector3 loc)
        {
            if (timestamp <= lastLocTimestamp)
            {
                Debug.LogError("timestamp <= lastLocTimestamp " + timestamp + " " + lastLocTimestamp + " \n" + UnityEngine.StackTraceUtility.ExtractStackTrace());
                return;
            }
            // timestamp is newer.
            lastLocTimestamp = timestamp;
            // Use the property to set position (which side effects the 
            // sounds, namebar, and bubble chat)
            // This will be applied on the next tick
            //Vector3 newLoc = worldManager.ResolveLocation(this, loc);
            //log.DebugFormat("loc for node {0} = newLoc {1} oldLoc {2}", this.Oid, newLoc, this.Position);
            //Vector3 displacement = newLoc - this.Position;
            //  Debug.LogError("obj " + Name + " loc=" + loc);
            this.Position = loc;

            //  Debug.LogError("obj "+Name+" "+this.Position+" || "+ gameObject.transform.position+" "+gameObject.transform.localPosition);

        }

        public virtual void SetOrientation(Quaternion orient)
        {
            // directly modify the orientation (instead of using property) to avoid marking the orientation dirty. 
            // This will be applied on the next tick
            Vector3 dir = Vector3.forward;
            Vector3 newDir = orient * dir;

            float yaw = orient.eulerAngles.y;
            //    UnityEngine.Debug.Log("Setting orientation to: " + orient + " with yaw: " + yaw + " and new orient: " + Quaternion.AngleAxis (yaw, Vector3.up)+ " newDir=" + newDir);
            // Orientation = Quaternion.AngleAxis(yaw, Vector3.up);
            _orientation = orient;
            turn = true;
            turnTime = 0f;
            //UnityEngine.Debug.Log("mob orientation is now: " + gameObject.transform.rotation);
        }

        Quaternion _orientation = Quaternion.identity;
        bool turn = false;
        float turnTime = 0f;
        /// <summary>
        ///   Update the node
        /// </summary>
        /// <param name="timeSinceLastFrame">time since last frame (in seconds)</param>
        public virtual void Tick(float timeSinceLastFrame)
        {
            //  UnityEngine.Debug.LogError("Tick "+Orientation+" != "+ _orientation);
            if (turn)
            {
                // UnityEngine.Debug.LogError("Setting orientation to: " + _orientation + " actual: " + Orientation+" t="+ turnTime);
//                Orientation = Quaternion.Lerp(Orientation, _orientation, timeSinceLastFrame * AtavismClient.Instance.smoothRotation);
                if(!Orientation.Equals(_orientation))
                    Orientation = Quaternion.LerpUnclamped(Orientation, _orientation, timeSinceLastFrame * AtavismClient.Instance.smoothRotation);
              // UnityEngine.Debug.LogError("Setting || orientation to: " + _orientation + " actual: " + Orientation);
                turnTime += 1 / AtavismClient.Instance.smoothRotation * timeSinceLastFrame;
                if (turnTime > 1f)
                    turn = false;
            }
        }

        public virtual void LateTick(float timeSinceLastFrame)
        {

        }
            /// <summary>
            /// Set multiple properties at once.
            /// We set all the properties first, and then raise the PropertyChange() events
            ///  so that if multiple properties change in one message, they appear atomic
            ///  to the scripts.
            /// </summary>
            /// <param name="newProperties"></param>
            public void UpdateProperties(Dictionary<string, object> newProperties)
            {
                string s = "";
            foreach (string prop in newProperties.Keys)
            {
                // set the property 
                properties[prop] = newProperties[prop];
                properties[prop + "_t"] = Time.time;
                // Special case some properties
                switch (prop)
                {
                    case "follow_terrain":
                        this.FollowTerrain = CheckBooleanProperty(prop);
                        break;
                }

               // s += prop + "=" + newProperties[prop] + " | ";
            }

          //  Debug.LogError("UpdateProperties name="+Name+" Params: "+s);
            foreach (string prop in newProperties.Keys)
            {
                OnPropertyChange(prop);
            }
        }

        public bool CheckBooleanProperty(string propName)
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

        public bool PropertyExists(string propName)
        {
            return properties.ContainsKey(propName);
        }

        public object GetProperty(string propName)
        {
            if (properties.ContainsKey(propName))
                return properties[propName];
            AtavismLogger.LogWarning("Property " + propName);
            return null;

        }

        public void SetProperty(string propName, object value)
        {
            properties[propName] = value;
            OnPropertyChange(propName);
        }

        protected virtual void OnPropertyChange(string propName)
        {
            if (propertyChangeHandlers.ContainsKey(propName))
            {
                foreach (PropertyChangeEventHandler handler in propertyChangeHandlers[propName].ToArray())
                {
                    //      Debug.Log ("Got handler for prop: " + propName + " for: " + oid);
                    if (handler != null && handler.Target != null && handler.Target.ToString() != "null")
                    {
                        try
                        {
                            handler(this, new PropertyChangeEventArgs(propName));
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Exception OnPropertyChange :" + e.Message + "\n\n " + e.StackTrace + "\n\n " + e.InnerException);
                        }
                    }
                    else
                    {
                        //  RemovePropertyChangeHandler(propName, handler);

                        Debug.LogWarning("Got null property handler for property: " + propName
                            + ". Make sure you have OnDestroy() functions that call RemovePropertyHandler in all classes that register property handlers");
                    }
                }
            }


            // invoke any global events on the property change
            worldManager.OnObjectPropertyChange(propName, this);
        }

        public void RegisterPropertyChangeHandler(string propName, PropertyChangeEventHandler handler)
        {
            if (!propertyChangeHandlers.ContainsKey(propName))
            {
                propertyChangeHandlers[propName] = new List<PropertyChangeEventHandler>();
            }

            List<PropertyChangeEventHandler> handlers = propertyChangeHandlers[propName];

            handlers.Add(handler);
        }

        public void RemovePropertyChangeHandler(string propName, PropertyChangeEventHandler handler)
        {
            if (!propertyChangeHandlers.ContainsKey(propName))
                return;
            List<PropertyChangeEventHandler> handlers = propertyChangeHandlers[propName];

            handlers.Remove(handler);

            // remove the list if there are no more handlers for this property
            if (handlers.Count == 0)
            {
                propertyChangeHandlers.Remove(propName);
            }
        }

        public Transform GetControllingTransform()
        {
            if (parent != null)
            {
                return parent.transform;
            }
            else
            {
                return gameObject.transform;
            }
        }

        #region Properties
        public long Oid
        {
            get
            {
                return oid;
            }
        }

        public virtual GameObject GameObject
        {
            get
            {
                return gameObject;
            }
            set
            {
                if (gameObject != null && !gameObject.Equals(value))
                {
                    //UnityEngine.Debug.LogError("Removing GameObject for: " + gameObject.name + " replacing with: " + value.name);
                    UnityEngine.GameObject.Destroy(gameObject);
                    gameObject = null;
                }
                gameObject = value;
                /*if (oid == worldManager.PlayerId) {
                    gameObject.AddComponent (Client.Instance.InputController);
                    UnityEngine.Object.DontDestroyOnLoad (gameObject);
                }*/
                if (gameObject.GetComponent<AtavismNode>() != null)
                    GameObject.DestroyImmediate(gameObject.GetComponent<AtavismNode>());
                gameObject.AddComponent<AtavismNode>();
                gameObject.GetComponent<AtavismNode>().SetNodeData(this);
              //  AtavismLogger.LogError("Set GameObject for: " + oid + "to: " + gameObject.name);
            }
        }

        public virtual GameObject Parent
        {
            get
            {
                return parent;
            }
            set
            {
                if (value != null)
                {
                    gameObject.transform.localPosition = Vector3.zero;
                    gameObject.transform.localRotation = Quaternion.identity;
                }
                else if (parent != null)
                {
                    gameObject.transform.localPosition = parent.transform.position;
                    gameObject.transform.localRotation = parent.transform.rotation;
                }
                parent = value;
            }
        }

        public string Name
        {
            get
            {
                if(gameObject != null)
                    return gameObject.name;
                return "";
            }
        }

        public List<string> PropertyNames
        {
            get
            {
                return new List<string>(properties.Keys);
            }
        }

        public Dictionary<string, object> Properties
        {
            get
            {
                return properties;
            }
        }

        public ObjectNodeType ObjectType
        {
            get
            {
                return objType;
            }
        }

        /// <summary>
        ///   Set and get the object's target position
        /// </summary>
        /// <value></value>
        public virtual Vector3 Position
        {
            get
            {
                if (parent != null)
                {
                    return parent.transform.position;
                }
                else
                {
                    if(gameObject!=null)
                        return gameObject.transform.position;
                    return Vector3.zero;
                }

            }
            set
            {
                if (parent != null)
                {
                    if (value != parent.transform.position)
                    {
                        //   Debug.LogError("Set Position for " + Name + " from " + gameObject.transform.position + " to " + value + "\n" + UnityEngine.StackTraceUtility.ExtractStackTrace());
                        parent.transform.position = value;
                        OnPositionChange();
                    }
                }
                else
                {
                    if (gameObject != null)
                        if (value != gameObject.transform.position)
                        {
                            if (AtavismLogger.logLevel <= LogLevel.Debug)
                                Debug.LogError("Set Position for " + Name + " from " + gameObject.transform.position + " to " + value + "\n" + UnityEngine.StackTraceUtility.ExtractStackTrace());
                            if (gameObject.GetComponent<CharacterController>() != null)
                            {

                                //  CharacterController cc = gameObject.GetComponent<CharacterController>();
                                //  if (!cc.enabled)
                                //     cc.enabled = true;
                                //cc.Move(value - gameObject.transform.position);
                                //  cc.enabled = false;
                                //gameObject.transform.position = value;
                                //  cc.enabled = true;
                                runChangePos = AtavismClient.Instance.StartCoroutine(ChangePos(value));
                                rubChangeToPos = value;
                            }
                            else
                                gameObject.transform.position = value;
                            UnityEngine.Physics.SyncTransforms();
                            OnPositionChange();
                        }
                }
            }
            
        }
        public Coroutine runChangePos =null;
        public Vector3 rubChangeToPos = Vector3.zero;
        IEnumerator ChangePos(Vector3 value)
        {

            if (AtavismLogger.logLevel <= LogLevel.Debug)
                Debug.LogError("ChangePos for " + Name + " from " + gameObject.transform.position + " to " + value );

            if (MobController.Mount != null)
            {
               MobController.Mount.GetComponent<CharacterController>().enabled = false;
            }
            else
            {
               gameObject.GetComponent<CharacterController>().enabled = false;
            }
            yield return new WaitForSeconds(0);
          // gameObject.transform.position = value;
            if (MobController.Mount != null)
            {
                MobController.Mount.transform.position = value;
            }
            else
            {
                gameObject.transform.position = value;
            }
            yield return new WaitForSeconds(0);
            if (MobController.Mount != null)
            {
                MobController.Mount.GetComponent<CharacterController>().enabled = true;
            }
            else
            {
                gameObject.GetComponent<CharacterController>().enabled = true;
            }
            if (AtavismLogger.logLevel <= LogLevel.Debug)
                Debug.LogError("ChangePos for " + Name + " pos " + gameObject.transform.position + " ==? " + value);
            runChangePos = null;
            rubChangeToPos = Vector3.zero;
            UnityEngine.Physics.SyncTransforms();
            //            gameObject.GetComponent<CharacterController>().enabled = true;
        }

        public virtual Quaternion Orientation
        {
            get
            {
                if (parent != null)
                {
                    return parent.transform.rotation;
                }
                else
                {
                    return gameObject.transform.rotation;
                }
            }
            set
            {
             //   Debug.LogError("Orientation "+Name, gameObject);
                if (parent != null)
                {
                    if (parent.transform.rotation != value)
                    {
                        parent.transform.rotation = value;
                        OnOrientationChange();
                    }
                }
                else
                {
                    if (gameObject != null)
                        if (gameObject.transform.rotation != value)
                        {
                            gameObject.transform.rotation = value;
                            OnOrientationChange();
                        }
                }
            }
        }

        public bool Targetable
        {
            get
            {
                return targetable;
            }
            set
            {
                targetable = value;
            }
        }

        public bool FollowTerrain
        {
            get
            {
                return followTerrain;
            }
            set
            {
                followTerrain = value;
            }
        }

        public AtavismMobController MobController
        {
            get
            {
                return mobController;
            }
            set
            {
                mobController = value;
                if (oid == worldManager.PlayerId)
                {
                    mobController.isPlayer = true;
                }
                else
                {
                    mobController.isPlayer = false;
                }
            }
        }
        #endregion

    }
}