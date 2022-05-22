using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Timers;
using UnityEngine.SceneManagement;
namespace Atavism
{

    /// <summary>
    ///		Delegate for object node events
    /// </summary>
    public delegate void ObjectEventHandler(object sender, AtavismObjectNode objNode);

    public delegate void PlayerInitializedHandler(object sender, EventArgs args);

    /// <summary>
    /// Event argument class for global object property change events
    /// </summary>
    public class ObjectPropertyChangeEventArgs : EventArgs
    {
        protected long oid;
        protected string propName;

        public ObjectPropertyChangeEventArgs(long oid, string propName)
        {
            this.propName = propName;
            this.oid = oid;
        }

        public long Oid
        {
            get
            {
                return oid;
            }
        }

        public string PropName
        {
            get
            {
                return propName;
            }
        }
    }

    /// <summary>
    /// Delegate global for object property change events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void ObjectPropertyChangeEventHandler(object sender, ObjectPropertyChangeEventArgs args);

    ///<summary>
    ///    A callback delegate for loading state changes.  
    ///<param name="msg">An ExtensionMessage instance. The msg has
    ///properties that may be meaningful to some handlers.
    ///</param>
    ///<param name="starting">True if the msg announces that we've
    ///started the loading process; false if the loading process is
    ///ending
    ///</param>
    ///</summary>
    public delegate void LoadingStateChangeHandler(ExtensionMessage msg, bool starting);

    public class AtavismWorldManager : IWorldManager
    {
        public class ObjectStub
        {
            public Vector3 scale;
            public bool followTerrain;
            public string name;
            //public GameObject gameObject;
            public Vector3 position;
            public Quaternion rotation;
            public ObjectNodeType objectType;
            public Vector3 direction;
            public long lastInterp;
        }

        #region Fields
        //protected StaticGeometryHelper staticGeometryHelper;

        protected float CollisionRange = 50 * AtavismClient.OneMeter;
        protected float DirUpdateInterval = 0.100f; // time in ticks between direction updates - 0.050f;
        protected float OrientUpdateInterval = 0.100f; // time in ticks between orientation updates - 0.050f;
                                                       //protected float PlayerUpdateTimer = 5.0f; // time in ticks between sending player updates
        protected float MaxLoadTime = 10.000f; // maximum number of milliseconds to wait before aborting the load of a mesh
        protected int BasePerceptionRadius = 200000; // 200m - this will be multiplied by the players view distance and the objects perception radius
        protected int mobLoadFrameSkip = 2;//Z 10
        private int frameCount = 0;

        bool playerInitialized = false;
        //bool terrainInitialized = false;
        bool playerStubInitialized = false;
        bool loadCompleted = false;
        long nextLocalOid = int.MaxValue;
        bool dirLocOrientSupported = true;

        // The network helper object
        AtavismNetworkHelper networkHelper = null;
        // The player object
        long playerId = 0;
        AtavismPlayer player = null;
        // The input controller
        AtavismInputController activeInputController;
        // The target object
        long targetId = -1;
        AtavismMobNode target = null;
        Vector3 lastCheckedPosition = new Vector3(0, 0, 0);
        //bool shuttingDown = false;
        bool nowLoading = false;

        private bool sendPlayerUpdateLocation = true;
        // Timer for updating the server with the player's position and other information
        float lastUpdateSent = 0;
        float movingTimeBetweenUpdates = 2.5f;
        float staticTimeBetweenUpdates = 5f; // Send updates every 2.5 seconds (was 5)

        // Resync distance limits
        float movingMaxSyncDistance = 4;
        float staticMaxSyncDistance = 1;

        private float lastsendha=0;

        // Complete objects
        Dictionary<long, AtavismObjectNode> nodeDictionary;
        Dictionary<long, float> startSpawnTime = new Dictionary<long, float>();
        Dictionary<long, float> startDespawnTime = new Dictionary<long, float>();


        // Stub object (got NewObject, but no ModelInfo)
        Dictionary<long, ObjectStub> objectStubs;

        List<ModelInfoMessage> modelsToLoad = new List<ModelInfoMessage>();

        // keep track of whether fog has been set or not
      //  protected Animation fogAnim = null;

        public event ObjectEventHandler ObjectAdded;
        public event ObjectEventHandler ObjectRemoved;
        public event PlayerInitializedHandler PlayerInitializedEvent;
        public event LoadingStateChangeHandler OnLoadingStateChange;

        protected bool logTerrainConfig = false;

        // Handlers for global object property change events
        Dictionary<string, List<ObjectPropertyChangeEventHandler>> objectPropertyChangeHandlers = new Dictionary<string, List<ObjectPropertyChangeEventHandler>>();

        //VoiceManager voiceMgr = null;
        int requestedWorld = -1;
        bool isAdmin = false;
        bool isDeveloper = false;

        public bool sceneLoaded = true;

        #endregion

        public AtavismWorldManager()
        {
            nodeDictionary = new Dictionary<long, AtavismObjectNode>();
            objectStubs = new Dictionary<long, ObjectStub>();

            
            MessageDispatcher.Instance.RegisterHandler(WorldMessageType.ModelInfo,
                                                           new WorldMessageHandler(this.HandleModelInfo));
            MessageDispatcher.Instance.RegisterHandler(WorldMessageType.NewObject,
                                                           new WorldMessageHandler(this.HandleNewObject));
            MessageDispatcher.Instance.RegisterHandler(WorldMessageType.FreeObject,
                                                           new WorldMessageHandler(this.HandleFreeObject));
            MessageDispatcher.Instance.RegisterHandler(WorldMessageType.Direction,
                                                           new WorldMessageHandler(this.HandleDirection));
            MessageDispatcher.Instance.RegisterHandler(WorldMessageType.Orientation,
                                                           new WorldMessageHandler(this.HandleOrientation));
            MessageDispatcher.Instance.RegisterHandler(WorldMessageType.DirLocOrient,
                                                           new WorldMessageHandler(this.HandleDirLocOrient));
            MessageDispatcher.Instance.RegisterHandler(WorldMessageType.TerrainConfig,
                                                           new WorldMessageHandler(this.HandleTerrainConfig));
            MessageDispatcher.Instance.RegisterHandler(WorldMessageType.InvokeEffect,
                                                           new WorldMessageHandler(this.HandleInvokeEffect));
            MessageDispatcher.Instance.RegisterHandler(WorldMessageType.MobPath,
                                                           new WorldMessageHandler(this.HandleMobPath));
            MessageDispatcher.Instance.RegisterHandler(WorldMessageType.WorldFile,
                                                           new WorldMessageHandler(this.HandleWorldFileName));
            MessageDispatcher.Instance.RegisterHandler(WorldMessageType.Logout,
                                                           new WorldMessageHandler(this.HandleLogout));
            
            RegisterObjectPropertyChangeHandler("jump", JumpHandler);
            RegisterObjectPropertyChangeHandler("rotating", RotatingHandler);
            RegisterObjectPropertyChangeHandler("movement_speed", PlayerSpeedHandler);
            RegisterObjectPropertyChangeHandler("follow_terrain", FollowTerrainHandler);
            RegisterObjectPropertyChangeHandler("facing", MobFacingHandler);
        }
        // Update is called once per frame
        public void LateUpdate()
        {
            // UnityEngine.Debug.LogError("AWM Update");
            if (!playerInitialized || player == null || !sceneLoaded)
                return;

            lock (nodeDictionary)
            {
                foreach (AtavismObjectNode node in nodeDictionary.Values)
                {
                    if (node is AtavismMobNode && !(node is AtavismPlayer))
                    {
                        ((AtavismMobNode)node).LateTick(Time.deltaTime);
                    }
                }
            }


        }
            // Update is called once per frame
            public void Update()
        {
           // UnityEngine.Debug.LogError("AWM Update");
            if (!playerInitialized || player == null || !sceneLoaded)
                return;
            float now = UnityEngine.Time.time;
            bool sendDir = (player.dirUpdate.dirty && (player.lastDirSent + DirUpdateInterval < now));
            bool sendOrient = (player.orientUpdate.dirty && (player.lastOrientSent + OrientUpdateInterval < now));
            // If the server supports the DirLocOrientMessage, use that because it minimizes network traffic
            if (lastsendha + 10 < now)
            {
                SendHaMessage(now);
                lastsendha = now;
            }

                if (AtavismLogger.logLevel <= LogLevel.Info)
                AtavismLogger.LogInfoMessage("AWM Player " + player + " dirLocOrientSupported=" + dirLocOrientSupported + " sendDir=" + sendDir + " sendOrient=" + sendOrient);
            if (dirLocOrientSupported)
            {
                if (sendDir || sendOrient)
                {
                    SendDirLocOrientMessage(player, now);
                }
            }
            else
            {
                if (sendDir)
                {
                    //directionMeter.Enter();
                    SendDirectionMessage(player, now);
                    //directionMeter.Exit();
                }
                if (sendOrient)
                {
                    //orientationMeter.Enter();
                    SendOrientationMessage(player, now);
                    //orientationMeter.Exit();
                }
            }

            // Do we need to send a update?
            if (sendPlayerUpdateLocation)
            {
                if (player.Direction == Vector3.zero)
                {
                    if (player.lastUpdateMovmentSent > (Time.realtimeSinceStartup - 1f) && (player.lastUpdateSent + 0.2f) < Time.realtimeSinceStartup)
                    {
                        SendDirLocOrientMessage(player, now);
                        player.lastUpdateSent = Time.realtimeSinceStartup;
                    }

                    if ((player.lastUpdateSent + staticTimeBetweenUpdates) < Time.realtimeSinceStartup)
                    {
                        SendDirLocOrientMessage(player, now);
                        player.lastUpdateSent = Time.realtimeSinceStartup;
                    }
                }
                else
                {
                    if ((player.lastUpdateSent + movingTimeBetweenUpdates) < Time.realtimeSinceStartup)
                    {
                        SendDirLocOrientMessage(player, now);
                        player.lastUpdateSent = Time.realtimeSinceStartup;
                        player.lastUpdateMovmentSent = Time.realtimeSinceStartup;
                    }
                }
            }

            // Do we need to send a heartbeat?
            if ((lastUpdateSent + staticTimeBetweenUpdates) < Time.realtimeSinceStartup)
            {
                SendHeartBeatMessage();
                lastUpdateSent = Time.realtimeSinceStartup;
            }

            // Load in any objects added to the models to add list
            //if (sceneLoaded && (frameCount % mobLoadFrameSkip == 0) && modelsToLoad.Count > 0)
            if (sceneLoaded && (frameCount % AtavismClient.Instance.mobLoadFrameSkip == 0) && modelsToLoad.Count > 0)
            {
                
                ModelInfoMessage modelInfo = modelsToLoad[0];
                if (AtavismLogger.logLevel <= LogLevel.Debug)
                    AtavismLogger.LogDebugMessage("Update Spawn Model " + modelInfo.Oid+" SLoaded="+ sceneLoaded+" Active="+SceneManager.GetActiveScene().name);

               AddObject(modelInfo, false);
                modelsToLoad.RemoveAt(0);
            }

            lock (nodeDictionary)
            {
                foreach (AtavismObjectNode node in nodeDictionary.Values)
                {
                    if (node is AtavismMobNode && !(node is AtavismPlayer))
                    {
                        ((AtavismMobNode)node).Tick(Time.deltaTime);
                    }
                }
            }
            frameCount++;
        }

        #region Message handlers

        public void HandleModelInfo(BaseWorldMessage message)
        {
            ModelInfoMessage modelInfo = (ModelInfoMessage)message;
            if (AtavismLogger.logLevel <= LogLevel.Debug)
                AtavismLogger.LogDebugMessage("HandleModelInfo " + message + " " + message.MessageType + " " + modelInfo.ModelInfo + " id=" + modelInfo.Oid+" "+ modelInfo.ForceInstantLoad);
            MeshInfo meshInfo = modelInfo.ModelInfo[0];
            AddObject(modelInfo/*modelInfo.Oid, meshInfo.MeshFile, meshInfo.DisplayID*/, true);
        }

        public void HandleNewObject(BaseWorldMessage message)
        {
            AtavismLogger.LogDebugMessage("Got HandleNewObject message");
            NewObjectMessage newObj = (NewObjectMessage)message;
            if (AtavismLogger.logLevel <= LogLevel.Debug)
                AtavismLogger.LogDebugMessage("HandleNewObject " + message + " " + message.MessageType+" "+ newObj.Name+" id="+ newObj.ObjectId);
         
            if (newObj.Name.Contains("_ign_") && AtavismClient.Instance.IgnoreProps)
            {
                return;
            }

            AddObjectStub(newObj.ObjectId, newObj.Name, newObj.Location,
                              newObj.Direction, newObj.LastInterp,
                              newObj.Orientation, newObj.ScaleFactor,
                              newObj.ObjectType, newObj.FollowTerrain);
        }

        public void HandleFreeObject(BaseWorldMessage message)
        {
             FreeObjectMessage freeObj = (FreeObjectMessage)message;
            if (AtavismLogger.logLevel <= LogLevel.Debug)
                AtavismLogger.LogDebugMessage("HandleFreeObject " + message + " " + message.MessageType+" "+ freeObj.ObjectId);
            RemoveObject(freeObj.ObjectId);
        }

        public void HandleDirection(BaseWorldMessage message)
        {
            DirectionMessage dirMessage = (DirectionMessage)message;
            long now = AtavismWorldManager.CurrentTime;
            SetDirLoc(dirMessage.Oid, dirMessage.Timestamp, dirMessage.Direction, dirMessage.Location);
        }

        public void HandleOrientation(BaseWorldMessage message)
        {
            OrientationMessage orientMessage = (OrientationMessage)message;
            SetOrientation(orientMessage.Oid, orientMessage.Orientation);
        }

        public bool sceneInLoading = false;
        DirLocOrientMessage dirMessageDelay;
        public void HandleDirLocOrient(BaseMessage message)
        {
            DirLocOrientMessage dirMessage = (DirLocOrientMessage)message;
            long now = AtavismWorldManager.CurrentTime;
            if (dirMessage.Oid == playerId)
            {
                if (!sceneInLoading)
                {
                    AtavismObjectNode node = GetObjectNode(playerId);
                    CharacterController cc = null;
                    if (node != null)
                    {
                        if (node.MobController.Mount != null)
                        {
                           cc = node.MobController.Mount.GetComponent<CharacterController>();
                        }
                        else
                        {
                            cc = node.GameObject.GetComponent<CharacterController>();
                        }

                    }
                    if (cc != null)
                        cc.enabled = false;
                    Vector3 currentPosition = player.Position;

                    SetDirLoc(dirMessage.Oid, dirMessage.Timestamp, dirMessage.Direction, dirMessage.Location);
                    SetOrientation(dirMessage.Oid, dirMessage.Orientation);
                    if (cc != null)
                        cc.enabled = true;

                    if (AtavismLogger.logLevel <= LogLevel.Debug)
                        AtavismLogger.LogDebugMessage("Got dirlocorient for player with distance: " + Vector3.Distance(dirMessage.Location, currentPosition) + " and position: " + dirMessage.Location);
                    if (Vector3.Distance(dirMessage.Location, currentPosition) > 15)
                    {
                  //   Debug.LogError("Teleport 1 "+Vector3.Distance(dirMessage.Location, currentPosition));
                     /*   string[] args = new string[2];

                        args[0] = "" + Vector3.Distance(dirMessage.Location, currentPosition);
                        args[1] = dirMessage.Location.x + "," + dirMessage.Location.y + "," + dirMessage.Location.z;
                        AtavismEventSystem.DispatchEvent("PLAYER_TELEPORTED", args);*/
                    }
                }
                else
                {
                    dirMessageDelay = (DirLocOrientMessage)message;
                    AtavismClient.Instance.StartDelay();
                }
            }
            else
            {
                if (AtavismLogger.logLevel <= LogLevel.Debug)
                    AtavismLogger.LogDebugMessage("Got dir: " + dirMessage.Direction + " for mob: " + dirMessage.Oid + " with timestamp: " + dirMessage.Timestamp);
                AtavismObjectNode node = GetObjectNode(dirMessage.Oid);
                CharacterController cc = null;
                if (node != null)
                {
                    if (node.MobController!=null && node.MobController.Mount != null)
                    {
                        cc = node.MobController.Mount.GetComponent<CharacterController>();
                    }
                    else
                    {
                       if(node.GameObject != null)
                            cc = node.GameObject.GetComponent<CharacterController>();
                    }
                    if (Vector3.Distance(dirMessage.Location, node.Position) > 10)
                    {
                        if (cc != null)
                            cc.enabled = false;
                    }
                }
                SetDirLoc(dirMessage.Oid, dirMessage.Timestamp, dirMessage.Direction, dirMessage.Location);
                if (cc != null)
                    cc.enabled = true;
                SetOrientation(dirMessage.Oid, dirMessage.Orientation); 
            }

        }

        bool delayStart = false;

        public void DelayPlayerMove()
        {
            if (dirMessageDelay != null)
            {
                Vector3 currentPosition = player.Position;
                SetDirLoc(playerId, dirMessageDelay.Timestamp, dirMessageDelay.Direction, dirMessageDelay.Location);
                SetOrientation(dirMessageDelay.Oid, dirMessageDelay.Orientation);
                if (Vector3.Distance(dirMessageDelay.Location, currentPosition) > 15)
                {
                    AtavismLogger.LogDebugMessage("Teleport 2 "+Vector3.Distance(dirMessageDelay.Location, currentPosition));
                   /* string[] args = new string[2];
                    args[0] = "" + Vector3.Distance(dirMessageDelay.Location, currentPosition);
                    args[1] = dirMessageDelay.Location.x + "," + dirMessageDelay.Location.y + "," + dirMessageDelay.Location.z;
                    AtavismEventSystem.DispatchEvent("PLAYER_TELEPORTED", args);*/
                }
            }
        }

        public void HandleTerrainConfig(BaseWorldMessage message)
        {
            AtavismLogger.LogDebugMessage("In handleterrainconfig");

            TerrainConfigMessage terrainConfigMessage = (TerrainConfigMessage)message;
            //Monitor.Enter (sceneManager);
            try
            {
                string s = terrainConfigMessage.ConfigString;

                // raise an event for scripts that want to mess with the world on startup
                //ClientAPI.TriggerWorldInitialized = true;
            }
            finally
            {
                //Monitor.Exit (sceneManager);
            }
            //log.InfoFormat ("Left handleterrainconfig");
        }

        public void HandleInvokeEffect(BaseWorldMessage message)
        {
            InvokeEffectMessage effectMessage = (InvokeEffectMessage)message;

            // log the effect and args
            if (AtavismLogger.logLevel <= LogLevel.Info)
                AtavismLogger.LogInfoMessage("HandleInvokeEffect starting effect " + effectMessage.EffectName + " with instanceID " + message.Oid + ", args:");

            try
            {
                CoordinatedEffectSystem.ExecuteCoordinatedEffect(effectMessage.EffectName, effectMessage.Args);
            }
            catch (Exception e)
            {
                AtavismLogger.LogError("HandleInvokeEffect starting effect " + effectMessage.EffectName + " with instanceID " + message.Oid + ", Got Exeption " + e.Message);
            }
        }

        public void HandleMobPath(BaseWorldMessage message)
        {
            AtavismLogger.LogInfoMessage("HandleMobPath ");
            
            try
            {
                MobPathMessage pathMsg = (MobPathMessage)message;
                if (AtavismLogger.logLevel <= LogLevel.Debug)
                    Debug.LogError("Got MobPath");
                 
                List<Vector3> pathPoints = pathMsg.PathPoints;
                long mobOid = pathMsg.Oid;
                AtavismMobNode mobNode = (AtavismMobNode)GetObjectNode(mobOid);
                if (AtavismLogger.logLevel <= LogLevel.Warning)
                    AtavismLogger.LogInfoMessage("HandleMobPath  for oid " + mobOid + ", name " + mobNode.Name + " pathPoints=" + pathPoints.Count+" path="+ string.Join(";", pathPoints.ToArray())+" Source="+pathMsg.Source+" Arrived="+pathMsg.Arrived);
                if (ClientAPI.Instance.mobOidDebug > 0L && mobOid == ClientAPI.Instance.mobOidDebug)
                {
                    UnityEngine.Debug.LogError("HandleMobPath  for oid " + mobOid + ", name " + mobNode.Name + " startTime="+pathMsg.StartTime+" interpKind="+pathMsg.InterpKind+" speed="+pathMsg.Speed+" terrainString="+pathMsg.TerrainString+" pathPoints=" + 
                                               pathPoints.Count + " path=" + string.Join(";", pathPoints.ToArray()) + " arrived=" + pathMsg.Arrived+"  source="+pathMsg.Source);
                         
                }

               if (mobNode == null)
                {
                   // AtavismLogger.LogError("Received MobPathMessage for oid " + mobOid + ", but that object is not a MobNode!");
                    // If there are no points, this message cancels any previous path interpolator for the object
                }
                else if (pathPoints.Count == 0)
                {
                    //mobNode.Interpolator = null;
                    if (AtavismLogger.logLevel <= LogLevel.Info)
                    AtavismLogger.LogInfoMessage("HandleMobPath  for oid " + mobOid + ", name " + mobNode.Name + " Position=" + mobNode.Position);

                    mobNode.LastDirection = Vector3.zero;
                    // Check if the model isn't loaded yet - if so, make sure the loc and dir are updated
                    if (GetModelInfo(mobOid) != null)
                    {
                        ObjectStub objStub = GetObjectStub(mobOid);
                        objStub.direction = Vector3.zero;
                    }
                    if (AtavismLogger.logLevel <= LogLevel.Info)
                        AtavismLogger.LogInfoMessage("Removed path interpolator for oid " + mobOid + ", name " + mobNode.Name + " create pathliner");
                    mobNode.Interpolator = (AtavismPathInterpolator)new PathLinear(mobOid, mobNode, pathMsg.StartTime, pathMsg.Speed, pathMsg.TerrainString, pathPoints);
                }
                else if (pathPoints.Count == 1)
                {
                    if (AtavismLogger.logLevel <= LogLevel.Warning)
                        AtavismLogger.LogInfoMessage("MobPath count = 1");
                    if (AtavismLogger.logLevel <= LogLevel.Info)
                        AtavismLogger.LogInfoMessage("Removed path interpolator for oid " + mobOid + ", name " + mobNode.Name + " and final loc: " + pathPoints[0]);
                    if (AtavismLogger.logLevel <= LogLevel.Warning)
                        AtavismLogger.LogWarning("Removed path interpolator for oid " + mobOid + ", name " + mobNode.Name + " and final loc: " + pathPoints[0]);
                    mobNode.LastDirection = Vector3.zero;
                    float dist = Vector2.Distance(new Vector2(pathPoints[0].x, pathPoints[0].z), new Vector2(mobNode.Position.x, mobNode.Position.z));
                    if (AtavismLogger.logLevel <= LogLevel.Warning)
                        AtavismLogger.LogWarning("mobNode distance to point: " + dist);
                    if (Vector2.Distance(new Vector2(pathPoints[0].x, pathPoints[0].z), new Vector2(mobNode.Position.x, mobNode.Position.z)) > 2)//ZBDS change from 3 to 1
                    {

                        if (AtavismLogger.logLevel <= LogLevel.Warning)
                            AtavismLogger.LogWarning("Moved mobNode before: " + mobNode.Position+" to: "+ pathPoints[0]);
                        mobNode.Position = new Vector3(pathPoints[0].x, mobNode.Position.y, pathPoints[0].z);
                        if (AtavismLogger.logLevel <= LogLevel.Info)
                            AtavismLogger.LogInfoMessage("Moved mobNode to: " + mobNode.Position);
                        if (AtavismLogger.logLevel <= LogLevel.Warning)
                            AtavismLogger.LogWarning("Moved mobNode to: " + mobNode.Position);
                    }
                    if (pathMsg.Arrived)
                    {
                        return;
                    }
                    // Check if the model isn't loaded yet - if so, make sure the loc and dir are updated
                    if (GetModelInfo(mobOid) != null)
                    {
                        ObjectStub objStub = GetObjectStub(mobOid);
                        objStub.direction = Vector3.zero;
                        objStub.position = new Vector3(pathPoints[0].x, pathPoints[0].y, pathPoints[0].z);
                    }
                    mobNode.Interpolator = (AtavismPathInterpolator)new PathLinear(mobOid, mobNode, pathMsg.StartTime, pathMsg.Speed, pathMsg.TerrainString, pathPoints);
                }
                else
                {
                    if (AtavismLogger.logLevel <= LogLevel.Debug)
                        AtavismLogger.LogInfoMessage("Adding new MobPath " + pathMsg.InterpKind.ToLower());
                    if (AtavismLogger.logLevel <= LogLevel.Debug)
                        AtavismLogger.LogInfoMessage("HandleMobPath  for oid " + mobOid + ", name " + mobNode.Name + " Position=" + mobNode.Position);
                    long now = AtavismWorldManager.CurrentTime;
                    AtavismPathInterpolator pathInterpolator =
                            (pathMsg.InterpKind.ToLower() == "spline") ?
                            (AtavismPathInterpolator)new PathSpline(mobOid, pathMsg.StartTime, pathMsg.Speed, pathMsg.TerrainString, pathPoints) :
                            (AtavismPathInterpolator)new PathLinear(mobOid, mobNode, pathMsg.StartTime, pathMsg.Speed, pathMsg.TerrainString, pathPoints);
                    mobNode.Interpolator = pathInterpolator;
                    
                    if (AtavismLogger.logLevel <= LogLevel.Info)
                        AtavismLogger.LogInfoMessage("Added path interpolator for oid " + mobOid + ", name " + mobNode.Name + ", speed " + pathMsg.Speed + ", point Count " + pathPoints.Count + ", interpolator " + pathInterpolator.ToString() + " with current location: " + mobNode.Position + " at time: " + Time.time);
                }
            }
            catch (Exception e)
            {
                  AtavismLogger.LogError("HandleMobPath: Exception " + e.Message + " " + e); 
            }
        }
        ///<summary>
        ///    A temp indicating if we've received the first loading
        ///    state change to false.
        ///</summary>
        private static int loadingStateCycles = 0;

        ///<summary>
        ///    If we're talking to a 1.1 server, which is the only
        ///    source of the old LoadingStateMessage, only react to
        ///    the first time the client receives loading state =
        ///    false.
        ///</summary>
        private void HandleLoadingState(BaseWorldMessage message)
        {
            LoadingStateMessage stateMessage = message as LoadingStateMessage;
            if (AtavismClient.ServerRelease1_5)
                AtavismLogger.LogInfoMessage("WorldManager.HandleLoadingState: loadingState " + stateMessage.LoadingState + " Ignoring message because talking to a release 1.5 or later server");
            else if (!stateMessage.LoadingState && loadingStateCycles == 0)
            {
                if (AtavismLogger.logLevel <= LogLevel.Info)
                    AtavismLogger.LogInfoMessage("LoadingStateMessage: state = " + stateMessage.LoadingState);
                HandleLoadingStateChange(null, stateMessage.LoadingState);
                loadingStateCycles++;
            }
        }

        private void HandleNewLoadingState(BaseWorldMessage message)
        {
            ExtensionMessage msg = message as ExtensionMessage;
            if (!msg.Properties.ContainsKey("ext_msg_subtype"))
                return;
            string s = msg.Properties["ext_msg_subtype"].ToString();
            if (AtavismLogger.logLevel <= LogLevel.Debug)
                AtavismLogger.LogError("HandleNewLoadingState: ext_msg_subtype=" + s);
            bool startingLoad = s == "mv.SCENE_BEGIN";
            bool endingLoad = s == "mv.SCENE_END";
            if (!startingLoad && !endingLoad)
                return;
            HandleLoadingStateChange(msg, startingLoad);
        }

        ///<summary>
        ///    The default handler for an old or new loading state
        ///    message.
        ///</summary>
        private void HandleLoadingStateChange(ExtensionMessage message, bool startingLoad)
        {
            LoadingStateChangeHandler handler = OnLoadingStateChange;
            nowLoading = startingLoad;
            string s = (startingLoad ? "start" : "stop");
            if (handler != null)
            {
                if (AtavismLogger.logLevel <= LogLevel.Info)
                    AtavismLogger.LogInfoMessage("WorldManager.HandleLoadingStateChange: Running event handler " + handler + " for loading state: " + s + " loading");
                handler(message, startingLoad);
            }
            else
            {
                if (AtavismLogger.logLevel <= LogLevel.Info)
                    AtavismLogger.LogInfoMessage("WorldManager.HandleLoadingStateChange: Running default actions for loading state " + s + " loading");
                // Should either put up a loading screen, or not
                // UpdateRenderTargets, but not both, because stopping
                // updating of render targets will prevent the loading
                // screen from being visible.
                if (startingLoad)
                {
                    // We're starting the load process, so mark the
                    // loadWindow visible first, and whack static
                    // geometry last.
                    //Client.Instance.LoadWindowVisible = startingLoad;
                    //Client.Instance.UpdateRenderTargets = !startingLoad;
                    //staticGeometryHelper.RebuildIfFinishedLoading(message, startingLoad);
                }
                else
                {
                    // We're ending the load process, so mark the
                    // whack static geometry first, and make the
                    // loadWindow invisible last.
                    //staticGeometryHelper.RebuildIfFinishedLoading(message, startingLoad);
                    //Client.Instance.UpdateRenderTargets = !startingLoad;
                    //Client.Instance.LoadWindowVisible = startingLoad;
                }
            }
        }

        public void HandleWorldAccessResponse(BaseWorldMessage message)
        {
            ExtensionMessage msg = message as ExtensionMessage;
            if (!msg.Properties.ContainsKey("ext_msg_subtype"))
                return;
            string s = msg.Properties["ext_msg_subtype"].ToString();
            if (s == "world_access_response")
            {
                requestedWorld = (int)msg.Properties["world"];
                /*if (!client.WorldUpdateNeeded (requestedWorld)) {
                    EnterWorld ();
                }*/
                EnterWorld();
            }
            else if (s == "world_developer_response")
            {
                isDeveloper = (bool)msg.Properties["isDeveloper"];
                isAdmin = (bool)msg.Properties["isAdmin"];
            }
        }

        /// <summary>
        /// Handles the world file message from the server. This clears the old scene and loads in the new 
        /// world files based on the world file name from the message.
        /// </summary>
        /// <param name="message"></param>
        public void HandleWorldFileName(BaseWorldMessage message)
        {
            //Debug.LogError("Handling world file name with current scene: " + Application.loadedLevelName);
            WorldFileMessage worldFileMessage = (WorldFileMessage)message;
            //Monitor.Enter(sceneManager);
            string worldFileName = worldFileMessage.WorldFileName;
            sceneLoaded = false;
            //Application.LoadLevel (worldFileName);

            // Call the Load Level system
            AtavismClient.Instance.LoadLevel(worldFileName);
            // Clear all models that are to be loaded
            lock (nodeDictionary)
            {
                // Clone the world dictionary, so that if an object's 
                // Dispose method tries to remove it from the dictionary, 
                // it won't cause a concurrent modification exception.
                List<AtavismObjectNode> nodes = new List<AtavismObjectNode>(nodeDictionary.Values);
                foreach (AtavismObjectNode node in nodes)
                {
                    try
                    {
                        if (node.Oid == playerId)
                            continue;

                        if (node is AtavismObjectNode)
                            RemoveNode(node as AtavismObjectNode);
                        //else if (node is LightEntity)
                        //	RemoveNode (node as LightEntity);
                    }
                    catch (Exception e)
                    {
                        AtavismLogger.LogError("Exception caught while removing node: " + e);
                        // Just ignore the exception here
                    }
                    //node.Dispose ();
                }
                //nodeDictionary.Clear();
            }

            //TODO: also then remove the objects?
            modelsToLoad.Clear();

        }

        public void HandleLogout(BaseWorldMessage message)
        {
            AtavismLogger.LogDebugMessage("Handling world logout");
            LogoutMessage logoutMessage = (LogoutMessage)message;
            networkHelper.AuthToken = logoutMessage.AuthToken;
            AtavismClient.Instance.ProcessServerLogout(logoutMessage.LogoutToCharacterSelection);
        }
        #endregion Message handlers



        public void EnterWorld()
        {
            ExtensionMessage enterWorldMsg = new ExtensionMessage(new OID(), false);
            enterWorldMsg.Properties.Add("ext_msg_subtype", "ao.ENTER_WORLD");
            enterWorldMsg.Properties.Add("senderOid", player.Oid);
            enterWorldMsg.Properties.Add("world", requestedWorld);
            networkHelper.SendMessage(enterWorldMsg);
        }

        protected GameObject SetGameObject(AtavismObjectNode node, string prefabName, string name, Vector3 position, Quaternion rotation)
        {
            if (node == null)
                return null;
            GameObject nodeObject = null;
            if (AtavismClient.instance.resourceManager == null)
            {
                if (AtavismLogger.logLevel <= LogLevel.Debug)
                    AtavismLogger.LogDebugMessage("Loading prefab: " + prefabName + " for oid: " + node.Oid);
                if (prefabName.Contains(".prefab"))
                {
                    int resourcePathPos = prefabName.IndexOf("Resources/");
                    prefabName = prefabName.Substring(resourcePathPos + 10);
                    prefabName = prefabName.Remove(prefabName.Length - 7);
                    GameObject prefab = (GameObject)Resources.Load(prefabName);
                    if (prefab == null)
                    {
                        AtavismLogger.LogError("Could not find prefab: " + prefabName);

                        prefab = (GameObject)Resources.Load("ExampleCharacter");
                    }
                    nodeObject = UnityEngine.Object.Instantiate(prefab) as GameObject;
                    nodeObject.transform.position = position;
                    nodeObject.transform.rotation = rotation;
                    UnityEngine.Physics.SyncTransforms();
                }
                else if (prefabName != "")
                {
                    GameObject prefab = (GameObject)Resources.Load(prefabName);
                    if (prefab == null)
                    {
                        AtavismLogger.LogError("Could not find | prefab: " + prefabName);
                        prefab = (GameObject)Resources.Load("ExampleCharacter");
                    }
                    nodeObject = UnityEngine.Object.Instantiate(prefab) as GameObject;
                    nodeObject.transform.position = position;
                    nodeObject.transform.rotation = rotation;
                    UnityEngine.Physics.SyncTransforms();
                }
                else if (AtavismClient.Instance.defaultObject != null)
                {
                    nodeObject = (GameObject)UnityEngine.Object.Instantiate(AtavismClient.Instance.defaultObject, position, rotation);
                }
                else
                {
                    nodeObject = new GameObject(name);
                    nodeObject.transform.position = position;

                }
                nodeObject.name = name;// + node.Oid;
                if (AtavismLogger.logLevel <= LogLevel.Debug)
                    AtavismLogger.LogDebugMessage("Created nodeObject: " + nodeObject + " from prefab: " + prefabName);
            }
            else
            {
                if (AtavismLogger.logLevel <= LogLevel.Debug)
                    AtavismLogger.LogDebugMessage("Loading prefab from external resource manager");
                string path = "";
                string fileName = prefabName;
                int splitPos = prefabName.LastIndexOf('/');
                if (splitPos != -1)
                {
                    path = prefabName.Substring(0, splitPos + 1);
                    fileName = prefabName.Substring(splitPos + 1);
                }
                object asset = AtavismClient.Instance.resourceManager.LoadAsset(node, path, fileName);
                if (asset != null)
                {
                    GameObject prefab = (GameObject)asset;
                    nodeObject = (GameObject)UnityEngine.Object.Instantiate(prefab, position, rotation);
                    nodeObject.name = name;// + node.Oid;
                                           //node.GameObject = nodeObject;
                    UnityEngine.Physics.SyncTransforms();
                }
            }
            if (AtavismLogger.logLevel <= LogLevel.Debug)
                AtavismLogger.LogDebugMessage("Returning from Creating GameObject for oid: " + node.Oid);
        //    UnityEngine.Debug.LogError(node.Oid+" "+name + " position=" + position+ " go position="+nodeObject.transform.position);
            return nodeObject;
        }


        // At some point, I would like to expose the scene node tree, so that
        // I can have objects move with other objects.
        private void AddObjectStub(long oid, string name, Vector3 location,
                                       Vector3 direction, long lastInterp,
                                       Quaternion orientation, Vector3 scale,
                                       ObjectNodeType objType, bool followTerrain)
        {
            if (AtavismLogger.logLevel <= LogLevel.Info)
                AtavismLogger.LogInfoMessage("In AddObjectStub - oid: " + oid + "; name: " + name + "; objType: " + objType
                + ", followTerrain: " + followTerrain + ", location: " + location + ", orient: " + orientation + " and direction: " + direction);

            ObjectStub stub = new ObjectStub();
            stub.name = name;
            stub.followTerrain = followTerrain;
            stub.objectType = objType;
            //stub.gameObject = gameObject;
            stub.position = location;
            stub.rotation = orientation;
            stub.direction = direction;
            stub.lastInterp = lastInterp;
            objectStubs[oid] = stub;
            if (oid == playerId)
            {
                playerStubInitialized = true;
            }
            lock (startDespawnTime)
            {
                startSpawnTime[oid] = Time.time;
            }
            if (AtavismLogger.logLevel <= LogLevel.Debug)
                AtavismLogger.LogDebugMessage("AddObjectStub completed for object: " + name);
         //   UnityEngine.Debug.LogError("AddObjectStub completed for object: " + name);
        }

        private AtavismObjectNode NewObjectNode(long oid, ObjectNodeType objectType, string name, bool followTerrain)
        {
            AtavismObjectNode node;

            if (AtavismLogger.logLevel <= LogLevel.Debug)
                AtavismLogger.LogDebugMessage("NewObjectNode oid " + oid + ", objectType " + objectType + ", name " + name);
            switch (objectType)
            {
                case ObjectNodeType.User:
                case ObjectNodeType.Npc:
                    if (player == null && oid == playerId)
                        node = new AtavismPlayer(oid, name, this);
                    else
                        node = new AtavismMobNode(oid, name, this);
                    break;
                case ObjectNodeType.Item:
                    node = new AtavismObjectNode(oid, name, objectType, this);
                    break;
                case ObjectNodeType.Prop:
                    node = new AtavismObjectNode(oid, name, objectType, this);
                    //staticGeometryHelper.NodeAdded (node);
                    break;
                default:
                    node = null;
                    break;
            }
            node.FollowTerrain = followTerrain;

            return node;
        }

        /// <summary>
        /// Adds the specified object to the scene. This function runs twice per object - once to set up the Node
        /// (when createNode is true) then runs again on an Update() cycle to load in the model when Unity is ready.
        /// </summary>
        /// <param name="modelInfo"></param>
        /// <param name="createNode"></param>
        private void AddObject(ModelInfoMessage modelInfo/*long oid, string meshFile, int displayID*/, bool createNode)
        {
            long oid = modelInfo.Oid;
            MeshInfo meshInfo = modelInfo.ModelInfo[0];
            string meshFile = meshInfo.MeshFile;
            int displayID = meshInfo.DisplayID;
            if (AtavismLogger.logLevel <= LogLevel.Debug)
                AtavismLogger.LogDebugMessage("In AddObject - oid: " + oid + "; model: " + meshFile);
            AtavismObjectNode node;

            lock (nodeDictionary)
            {
                if (nodeDictionary.ContainsKey(oid) && createNode)
                {
                    //   AtavismLogger.LogError("Got duplicate AddObject for oid " + oid);
                    if (!(nodeDictionary[oid] is AtavismObjectNode))
                    {
                        AtavismLogger.LogError("AddObject for existing non-object entity");
                        return;
                    }
                    // If GameObject is null destroy parent and delete from array
                    if (nodeDictionary[oid].GameObject == null)
                    {
                        //    AtavismLogger.LogError("Got duplicate AddObject for oid " + oid+" GameObject is null");
                        if (nodeDictionary[oid].Parent != null)
                        {
                            GameObject.Destroy(nodeDictionary[oid].Parent);
                        }
                        nodeDictionary.Remove(oid);
                    }
                    else
                    {
                        //   AtavismLogger.LogError("Got duplicate AddObject for oid " + oid + " GameObject not null");
                        //node = nodeDictionary[oid] as AtavismObjectNode;
                        //SetGameObject (node, meshFile, node.Name, node.Position, node.Orientation);
                        return;
                    }
                }
            }

            ObjectStub objStub = GetObjectStub(oid);
            if (objStub == null)
            {
                //AtavismLogger.LogError("GetObjectStub get null");
                return;
            }
            if (createNode)
            {
                node = NewObjectNode(oid, objStub.objectType, objStub.name, objStub.followTerrain);
            }
            else
            {
                node = GetObjectNode(oid);
            }

            //
            // Add the node to the dictionary first, so that OID lookups in the
            // PlayerInitialized and ObjectAdded event handlers will find it.
            // 
            if (AtavismLogger.logLevel <= LogLevel.Debug)
                AtavismLogger.LogDebugMessage("About to add to node dictionary with oid: " + oid);
           // AtavismLogger.LogError("About to add to node dictionary with oid: " + oid);
            if (createNode)
            {
                lock (nodeDictionary)
                {
                    nodeDictionary[oid] = node;
                }
                /*lock (startDespawnTime)
                {
                    startSpawnTime[oid] = Time.time;
                }*/
            }

            /*bool status = SetGameObject (node, meshFile);
            if (!status)
                return;*/
            if (AtavismLogger.logLevel <= LogLevel.Debug)
                AtavismLogger.LogDebugMessage("About to apply model with node: " + node);
            if (!createNode /*sceneLoaded || */ || oid == playerId)
            {
            //    AtavismLogger.LogError("About to Set GameObject: " + oid);
                GameObject nodeObject = SetGameObject(node, meshFile, objStub.name, objStub.position, objStub.rotation);
                if (nodeObject == null)
                {
                    AtavismLogger.LogError("nodeObject is null break : " + oid);

                    return;
                }

              //  AtavismLogger.LogError("About to Set go: " + oid + " " + nodeObject+" "+nodeObject.hideFlags+" "+ nodeObject.scene.name+" "+nodeObject.name+" "+nodeObject.transform.parent+" "+nodeObject.transform.position+" "+nodeObject.activeInHierarchy);
                // Set the GameObject to the node
                node.GameObject = nodeObject;
                node.createdGo = true;
            }
            else
            {
                // Only creating the node at this point, will load up the model when the system is ready
                modelsToLoad.Add(modelInfo);
                return;
            }
          //  AtavismLogger.LogError("About to Set position: " + oid);
            Vector3 p = objStub.position; // + new Vector3 (0, 15, 0);
            if (AtavismLogger.logLevel <= LogLevel.Debug)
                AtavismLogger.LogDebugMessage("About to set position and direction with position: " + p);
           // UnityEngine.Debug.LogError(node.Oid+" "+ objStub.name+" About to set position and direction with position: " + p);

            //if (objStub.followTerrain && p.y == 0)
            //	p.y = GetHeightAt (p);

            if (objStub.direction.magnitude == 0f || !(node is AtavismMobNode))
            {
                // Pretend this message was created at time 0.  Since the 
                // direction vector is zero, this will not mess up the 
                // interpolation, but will allow any of the server's queued 
                // dirloc messages about this object to be processed
                node.SetDirLoc(0, Vector3.zero, p);
            }
            else
            {
                // We have a direction and a last interp time from the
                // ObjectStub, and this is a mobnode.  So adjust the
                // timestamp, and compute the new position.
                long now = AtavismWorldManager.CurrentTime;
                node.SetDirLoc(now, objStub.direction, p);
            }

            node.SetOrientation(objStub.rotation);

            //
            // Set the player first, so that PlayerInitialized
            //  triggers before ObjectAdded, and so that the Player
            //  property is initialized for both event handlers.
            //
            AtavismLogger.LogDebugMessage("About to check if node is the player");
            if (node is AtavismPlayer && node.Oid == playerId)
            {
                SetPlayer(node);
            }

            if (ObjectAdded != null)
            {
                ObjectAdded(this, node);
            }
            node.SetProperty("displayID", displayID);
           // AtavismLogger.LogError("End: " + oid);
        }

        public void RemoveObject(long oid)
        {
            if (AtavismLogger.logLevel <= LogLevel.Debug)
                AtavismLogger.LogDebugMessage("Got RemoveObject for oid " + oid);
         //  Debug.LogError("Got RemoveObject for oid " + oid);
            //Debug.Assert (oid != playerId);
            if (oid == playerId)
            {
                AtavismLogger.LogError("Attempted to remove the player from the world");
                return;
            }
            lock (modelsToLoad)
            {
                foreach (ModelInfoMessage modelInfo in modelsToLoad)
                {
                    if (modelInfo.Oid == oid)
                    {
                        modelsToLoad.Remove(modelInfo);
                        break;
                    }
                }
            }
            // Clear the entry from the world dictionary
            lock (nodeDictionary)
            {
                if (!nodeDictionary.ContainsKey(oid))
                {
                    //AtavismLogger.LogError("Got RemoveObject for invalid oid " + oid);
                    return;
                }
                // RemoveNode(oid);

                AtavismObjectNode node = nodeDictionary[oid];
                if (node is AtavismObjectNode)
                {
                    AtavismObjectNode objNode = node as AtavismObjectNode;
                  
                    //    Debug.LogError("Despawn Error Oid:" + node.Oid + " Name:" + node.Name + " GO:" + node.GameObject + " Parent:" + node.Parent);
                        if (node.GameObject != null)
                        {
                            if(node.createdGo)
                                if(node.GameObject.activeInHierarchy)
                                    node.GameObject.SendMessage("despawn");
                        }
                        if (node.Parent != null)
                        {
                            if (node.Parent.activeInHierarchy)
                                node.Parent.SendMessage("despawn");
                        }
                  
                }
                lock (startDespawnTime)
                {
                    startDespawnTime[oid] = Time.time;
                }

                AtavismClient.Instance.StartDespawnDelay(oid);
               
            }
            if (AtavismLogger.logLevel <= LogLevel.Debug)
                AtavismLogger.LogDebugMessage("Removed object for oid " + oid);
        }
        public void RemoveObjectNode(long oid)
        {
            if (startSpawnTime.ContainsKey(oid) && startDespawnTime.ContainsKey(oid))
            {
                lock (startDespawnTime)
                {
                    lock (startDespawnTime)
                    {
                        if (startDespawnTime[oid] < startSpawnTime[oid])
                        {
                            if (AtavismLogger.logLevel <= LogLevel.Debug)
                                AtavismLogger.LogDebugMessage("Got RemoveObject for object that was recreated oid " + oid);
                          //  Debug.LogError("Got RemoveObject for object that was recreated oid " + oid + " Spwan="+ startSpawnTime[oid]+" Despwan="+ startDespawnTime[oid]);

                            return;
                        }
                    }
                }
            }
            lock (nodeDictionary)
            {
                if (!nodeDictionary.ContainsKey(oid))
                {
                    //AtavismLogger.LogError("Got RemoveObject for invalid oid " + oid);
                    return;
                }
                RemoveNode(oid);
            }
        }
        /// <summary>
        ///   We already hold the lock on the sceneManager and the nodeDictionary.
        ///   This doesn't remove the node from the dictionary, but it does remove
        ///   it from the scene, and cleans up.
        /// </summary>
        /// <param name="oid"></param>
        private void RemoveNode(AtavismObjectNode node)
        {
            GameObject.Destroy(node.GameObject);
            if (node.Parent != null)
            {
                GameObject.Destroy(node.Parent);
            }
            if (ObjectRemoved != null)
                ObjectRemoved(this, node);
        }


        // Removes a node from the nodeDictionary and also does whatever
        // cleanup needs to be done on that node.
        private void RemoveNode(long oid)
        {
            //Monitor.Enter (sceneManager);
            try
            {
                lock (nodeDictionary)
                {
                    AtavismObjectNode node = nodeDictionary[oid];
                    if (node is AtavismObjectNode)
                    {
                        AtavismObjectNode objNode = node as AtavismObjectNode;

                        if (AtavismLogger.logLevel <= LogLevel.Debug)
                        {
                            AtavismLogger.LogDebugMessage("Removing node " + objNode.Name + ", oid " + oid);
                        }
                     //   Debug.LogError("Removing node " + objNode.Name + ", oid " + oid);
                        RemoveNode(objNode);
                    }
                    nodeDictionary.Remove(oid);
                }
                // If removing the target, set target to null
                if (oid == targetId)
                {
                    TargetId = -1;
                }
            }
            finally
            {
                //Monitor.Exit (sceneManager);
            }
        }


    /*    IEnumerator UpdateTimer()
        {
            WaitForSeconds delay  = new WaitForSeconds(0.04f);
            yield return delay;
            RemoveNode(oid);
        }*/



            // Fetch a list of all objects in the world.
            public List<long> GetObjectOidList()
        {
            List<long> rv = new List<long>();
            lock (nodeDictionary)
            {
                foreach (long val in nodeDictionary.Keys)
                    rv.Add(val);
            }
            return rv;
        }

        protected ObjectStub GetObjectStub(long oid)
        {
            lock (objectStubs)
            {
                if(objectStubs.ContainsKey(oid))
                     return objectStubs[oid];
            //    AtavismLogger.LogError("Not found Object  "+oid);
                return null;
            }
        }

        public AtavismObjectNode GetObjectNode(OID oid)
        {
            return GetObjectNode(oid.ToLong());
        }

        public AtavismObjectNode GetObjectNode(long oid)
        {
            lock (nodeDictionary)
            {
                AtavismObjectNode node;
                if (nodeDictionary.TryGetValue(oid, out node) && node is AtavismObjectNode)
                    return node as AtavismObjectNode;
                return null;
            }
        }

        public AtavismObjectNode GetObjectNode(string name)
        {
            lock (nodeDictionary)
            {
                foreach (AtavismObjectNode node in nodeDictionary.Values)
                    if (node is AtavismObjectNode)
                    {
                        AtavismObjectNode objNode = node as AtavismObjectNode;
                        if (objNode.Name == name)
                            return objNode;
                    }
                return null;
            }
        }

        /// <summary>
        /// Returns a list of all ObjectNode names.  This is used by scripting.
        /// </summary>
        /// <returns></returns>
        public List<string> GetObjectNodeNames()
        {
            List<string> retList = new List<string>();

            lock (nodeDictionary)
            {
                foreach (AtavismObjectNode node in nodeDictionary.Values)
                {
                    AtavismObjectNode objNode = node as AtavismObjectNode;
                    if (objNode != null)
                    {
                        retList.Add(objNode.Name);
                    }
                }
            }

            return retList;
        }

        private ModelInfoMessage GetModelInfo(long oid)
        {
            foreach (ModelInfoMessage message in modelsToLoad)
            {
                if (message.Oid == oid)
                {
                    return message;
                }
            }
            return null;
        }

        /// <summary>
        ///   Sets up the data needed for interpolated movement of an object.
        ///   In this case, it is the direction, location, and timestamp when 
        ///   that location was the current location.
        /// </summary>
        /// <param name="oid">object id of the object for which we are setting the dir and loc</param>
        /// <param name="timestamp">time that the message was created (in client time)</param>
        /// <param name="dir">direction of motion of the object</param>
        /// <param name="loc">initial location of the object (at the time the message was created)</param>
        private void SetDirLoc(long oid, long timestamp, Vector3 dir, Vector3 loc)
        {
            AtavismObjectNode node = GetObjectNode(oid);
            if (node != null && node == player)
            {
                //CheckWorldObjectProximities(false);
                node.SetDirLoc(timestamp, dir, loc);
                if (AtavismLogger.logLevel <= LogLevel.Debug)
                    AtavismLogger.LogDebugMessage("SetDirLoc: " + loc);
                //  Debug.LogError(node.Name + " " + node.GameObject.transform.position+" funkcja wyzej loc=" +loc+" dir="+dir+" dl="+dir.magnitude);
            }
            else if (node != null)
            {
                node.SetDirLoc(timestamp, dir, loc);
                node.SetProperty("loc_t", Time.time);
                node.SetProperty("player_d", Vector3.Distance(loc, player.Position));

                // Insert code here (or just above) to check if the loc difference is small
            }
            else
            {
                if (AtavismLogger.logLevel <= LogLevel.Warning)

                    AtavismLogger.LogWarning("No node match for: " + oid);
            }

        }

        private void SetOrientation(long oid, Quaternion orient)
        {
            AtavismObjectNode node = GetObjectNode(oid);

            if (node != null && node == player)
            {
                //log.InfoFormat("Server set player orientation to {0} instead of {1}",
                //			   orient, player.Orientation);
                
            }
            if (node != null)
                node.SetOrientation(orient);
            else
            {
                if (AtavismLogger.logLevel <= LogLevel.Warning)

                    AtavismLogger.LogWarning("No node match for: " + oid);
            }
        }

        public List<AtavismMobNode> GetMobNodes()
        {
            List<AtavismMobNode> rv = new List<AtavismMobNode>();
            lock (nodeDictionary)
            {
                foreach (AtavismObjectNode node in nodeDictionary.Values)
                    if (node is AtavismMobNode)
                        rv.Add((AtavismMobNode)node);
                return rv;
            }
        }

        private void OnPlayerInitialized()
        {
            PlayerInitializedHandler handler = PlayerInitializedEvent;
            if (handler != null)
            {
                handler(null, null);
            }
        }

        private void SetPlayer(AtavismObjectNode node)
        {
            if (AtavismLogger.logLevel <= LogLevel.Info)
                AtavismLogger.LogInfoMessage("Setting player to node: " + node);
            if (node == null && player != null && player.GameObject != null)
            {
                GameObject.Destroy(player.GameObject);
            }
            player = (AtavismPlayer)node;
            if (player != null)
            {
                //player.Disposed += new ObjectNodeDisposed (PlayerDisposed);
                //UpdateVisibleObjects (player.Position);
                if (player.createdGo)
                {
                    player.GameObject.layer = 1 << AtavismClient.Instance.playerLayer;
                    player.GameObject.tag = AtavismClient.Instance.playerTag;
                }
            }
            OnPlayerInitialized();
        }

        void PlayerDisposed(AtavismObjectNode objNode)
        {
            if ((player != null) || (objNode == player))
            {
                SetPlayer(null);
            }
        }

        // The timer event
        public void SendPlayerData(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Monitor.Enter (this);
            try
            {
                if (player == null || networkHelper == null)
                    return;
                long now = AtavismWorldManager.CurrentTime;
                if (dirLocOrientSupported)
                    SendDirLocOrientMessage(player, now);
                else
                {
                    SendDirectionMessage(player, now);
                    SendOrientationMessage(player, now);
                }
            }
            finally
            {
                //Monitor.Exit (this);
            }
        }

        protected void SendOrientationMessage(AtavismPlayer node, float now)
        {
            if (node.orientUpdate.dirty)
            {
                Quaternion orient = node.orientUpdate.orientation;
                node.orientUpdate.dirty = false;
                networkHelper.SendOrientationMessage(orient);
            }
            else
            {
                // normal periodic send
                Quaternion orient = node.Orientation;
                networkHelper.SendOrientationMessage(orient);
            }
            node.lastOrientSent = now;
        }

        private void SendHaMessage(float now)
        {
            DateTime date1 = new DateTime(1970, 1, 1);
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - date1.Ticks);
            //  Console.WriteLine(Convert.ToInt32(ts.TotalSeconds));
            long timestamp = (long)Convert.ToUInt64(ts.TotalMilliseconds);
            networkHelper.SendHaMessage(timestamp);
         //   AtavismLogger.LogDebugMessage("Sending Ha update");
        }


        protected void SendDirectionMessage(AtavismPlayer node, float now)
        {
            if (node.dirUpdate.dirty)
            {
                float timestamp = node.dirUpdate.timestamp;
                Vector3 dir = node.dirUpdate.direction;
                Vector3 pos = node.dirUpdate.position;
                networkHelper.SendDirectionMessage((long)timestamp, dir, pos);
                node.dirUpdate.dirty = false;
                if (AtavismLogger.logLevel <= LogLevel.Debug)
                    AtavismLogger.LogDebugMessage("Sending dir update: " + dir);
            }
            else
            {
                float timestamp = node.dirUpdate.timestamp;
                Vector3 dir = node.Direction;
                Vector3 pos = node.Position;
                networkHelper.SendDirectionMessage((long)timestamp, dir, pos);
                if (AtavismLogger.logLevel <= LogLevel.Debug)
                    AtavismLogger.LogDebugMessage("Sending timed dir update: " + dir);
            }
            node.lastDirSent = now;
        }

        protected void SendDirLocOrientMessage(AtavismPlayer node, float now)
        {
            if (node.dirUpdate.dirty || node.orientUpdate.dirty)
            {
                //float timestamp = node.dirUpdate.timestamp;
               // long timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;//node.dirUpdate.timestamp;
                DateTime date1 = new DateTime(1970, 1, 1);
                TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - date1.Ticks);
                //  Console.WriteLine(Convert.ToInt32(ts.TotalSeconds));
                long timestamp = (long)Convert.ToUInt64(ts.TotalMilliseconds);
                Vector3 dir = node.dirUpdate.direction;
                Vector3 pos = node.dirUpdate.position;
                Quaternion orient = node.orientUpdate.orientation;
                networkHelper.SendDirLocOrientMessage((long)timestamp, dir, pos, orient, node.updateCount);
                node.dirUpdate.dirty = false;
                node.orientUpdate.dirty = false;
                //  AtavismLogger.LogDebugMessage("Sending dirty loc update: " + pos + " dir: " + dir + " ts:" + timestamp + " lts:" + (long)timestamp);
                if (AtavismLogger.logLevel <= LogLevel.Debug)
                    AtavismLogger.LogDebugMessage("Sending "+node.Name+" dirty loc update: " + pos + " dir: " + dir + " ts:" + timestamp + " lts:" + (long)timestamp);
            }
            else
            {
                // normal periodic send
                DateTime date1 = new DateTime(1970, 1, 1);
                TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - date1.Ticks);
                //  Console.WriteLine(Convert.ToInt32(ts.TotalSeconds));
                long timestamp = (long)Convert.ToUInt64(ts.TotalMilliseconds);
                Vector3 dir = node.Direction;
                Vector3 pos = node.Position;
                Quaternion orient = node.Orientation;
                networkHelper.SendDirLocOrientMessage((long)timestamp, dir, pos, orient, node.updateCount);
            }
            node.lastDirSent = now;
            node.lastOrientSent = now;
            node.updateCount++;
            //CheckWorldObjectProximities(false);
        }

        protected void SendHeartBeatMessage()
        {
            AtavismLogger.LogDebugMessage("SendHeartBeatMessage");
            Dictionary<string, object> props = new Dictionary<string, object>();
            ExtensionMessage message = new ExtensionMessage(playerId, false);
            message.Properties["ext_msg_subtype"] = "ao.heartbeat";
            AtavismClient.Instance.NetworkHelper.SendMessage(message);
        }

        /// <summary>
        ///   Update the time offset.
        /// </summary>
        /// <param name="timestamp">timestamp received from the server</param>
        public void AdjustTimestamp(long timestamp)
        {
            networkHelper.AdjustTimestamp(timestamp);
        }

        public void RegisterObjectPropertyChangeHandler(string propName, ObjectPropertyChangeEventHandler handler)
        {
            if (!objectPropertyChangeHandlers.ContainsKey(propName))
            {
                objectPropertyChangeHandlers[propName] = new List<ObjectPropertyChangeEventHandler>();
            }

            List<ObjectPropertyChangeEventHandler> handlers = objectPropertyChangeHandlers[propName];

            handlers.Add(handler);
            //objectPropertyChangeHandlers [propName]
        }

        public void RemoveObjectPropertyChangeHandler(string propName, ObjectPropertyChangeEventHandler handler)
        {
            List<ObjectPropertyChangeEventHandler> handlers = objectPropertyChangeHandlers[propName];

            handlers.Remove(handler);

            // remove the list if there are no more handlers for this property
            if (handlers.Count == 0)
            {
                objectPropertyChangeHandlers.Remove(propName);
            }
        }

        /// <summary>
        /// Invoke object property change events
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="objectNode"></param>
        public void OnObjectPropertyChange(string propName, AtavismObjectNode objectNode)
        {
            if (objectPropertyChangeHandlers.ContainsKey(propName))
            {
                foreach (ObjectPropertyChangeEventHandler handler in objectPropertyChangeHandlers[propName])
                {
                    //Logger.LogDebugMessage("Got handler for prop: " + propName);
                    handler(objectNode, new ObjectPropertyChangeEventArgs(objectNode.Oid, propName));
                }
            }
        }

        public void InputControllerActivated(AtavismInputController inputController)
        {
            activeInputController = inputController;
            if (player != null)
            {
                activeInputController.Target = player.GetControllingTransform();
            }
        }

        /// <summary>
        /// Called by the Physics System when the player has told their character to jump. Sends a message
        /// out to the server so all players nearby will see them jump.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="objectOid"></param>
        /// <param name="isJumping"></param>
        public void SendJumpStarted()
        {
            networkHelper.SendTargettedCommand(player.Oid, "/jump true");
        }

        public void SendFallStarted()
        {
            ExtensionMessage message = new ExtensionMessage(player.Oid, false);
            message.Properties["fallingState"] = "start";
            message.Properties["ext_msg_subtype"] = "ao.FALLING_EVENT";
            AtavismClient.Instance.NetworkHelper.SendMessage(message);
        }

        public void SendFallEnded()
        {
            ExtensionMessage message = new ExtensionMessage(player.Oid, false);
            message.Properties["fallingState"] = "end";
            message.Properties["ext_msg_subtype"] = "ao.FALLING_EVENT";
            AtavismClient.Instance.NetworkHelper.SendMessage(message);
        }

        /// <summary>
        /// Called by the Physics System when the player has told their character to jump. Sends a message
        /// out to the server so all players nearby will see them jump.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="objectOid"></param>
        /// <param name="isJumping"></param>
        public void SendPlayerRotationDiration(float rotationDirection)
        {
            networkHelper.SendTargettedCommand(player.Oid, "/rotating " + rotationDirection);
        }

        /// <summary>
        /// Called by the Physics System when a mobs (or players) falling state has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="objectNode"></param>
        /// <param name="isFalling"></param>
        protected void FallingHandler(object sender, object objectNode, bool isFalling)
        {
            AtavismObjectNode node = (AtavismObjectNode)objectNode;
            node.SetProperty("falling", isFalling);
        }

        /// <summary>
        /// Runs when another players/mob jump state has changed. If true, then we need to tell the physics
        /// system to make this character jump on our screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void JumpHandler(object sender, ObjectPropertyChangeEventArgs args)
        {
            if (AtavismLogger.logLevel <= LogLevel.Debug)
                AtavismLogger.LogDebugMessage("Got jump handler with oid: " + args.Oid);
            long oid = args.Oid;
            // Ignore messages about the clients characters
            if (oid == player.Oid)
                return;
            AtavismObjectNode objNode = GetObjectNode(oid);
            
            if (objNode != null && (bool)objNode.GetProperty(args.PropName) == true && objNode is AtavismMobNode)
            {
                AtavismMobNode mobNode = (AtavismMobNode)objNode;
                if (mobNode != null && mobNode.MobController != null)
                {
                    mobNode.MobController.LastJumpButtonTime = Time.time;
                }
                //Logger.LogDebugMessage ("Starting jump for mob: " + mobNode.Name);
            }
        }

        protected void RotatingHandler(object sender, ObjectPropertyChangeEventArgs args)
        {
            if (player == null)
                return;
            //AtavismLogger.LogDebugMessage("Got rotating handler with oid: " + args.Oid);
            long oid = args.Oid;
            // Ignore messages about the clients characters
            if (oid == player.Oid)
                return;
            AtavismObjectNode objNode = GetObjectNode(oid);
            if (objNode is AtavismMobNode)
            {
                AtavismMobNode mobNode = (AtavismMobNode)objNode;
                if (mobNode != null && mobNode.MobController != null)
                {
                    mobNode.MobController.RotatingDirection = (float)objNode.GetProperty(args.PropName);
                }//Logger.LogDebugMessage ("Starting jump for mob: " + mobNode.Name);
            }
        }

        /// <summary>
        /// Runs when the movement speed property is changed for a mob/player. If it is the player who is running
        /// this client then we need to set their movement speed in the inputHandler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void PlayerSpeedHandler(object sender, ObjectPropertyChangeEventArgs args)
        {
            //Logger.LogError("Got Player Speed");
            long oid = args.Oid;
            if (!oid.Equals(player.Oid))
                return;
            AtavismObjectNode objNode = GetObjectNode(oid);
            int movement_speed = (int)objNode.GetProperty(args.PropName);
            objNode.MobController.RunSpeed = movement_speed;
            if (movement_speed == 0)
            {
                player.Direction = Vector3.zero;
                player.LastDirection = Vector3.zero;
                player.dirUpdate.dirty = true;
                SendDirLocOrientMessage(player, AtavismWorldManager.CurrentTime);
            }
        }

        /// <summary>
        /// Runs when the follow terrain property is changed for a mob/player.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void FollowTerrainHandler(object sender, ObjectPropertyChangeEventArgs args)
        {
            long oid = args.Oid;
            AtavismObjectNode objNode = GetObjectNode(oid);
            bool followTerrain = (bool)objNode.GetProperty(args.PropName);
            if (objNode.MobController != null)
                objNode.MobController.FollowTerrain = followTerrain;
            else
            {
       
                    AtavismLogger.LogWarning("MobController is null");
            }
        }

      
        protected void MobFacingHandler(object sender, ObjectPropertyChangeEventArgs args)
        {
            long oid = args.Oid;
            AtavismMobNode mobNode = (AtavismMobNode)GetObjectNode(oid);
            if (mobNode != null)
            {
                OID facing = (OID)mobNode.GetProperty(args.PropName);
                if (facing == null)
                {

                    if (mobNode.MobController!=null)
                        mobNode.MobController.Target = -1;
                }
                else
                {
                    if (mobNode.MobController != null)
                        mobNode.MobController.Target = facing.ToLong();
                }
            }
            //UnityEngine.Debug.Log("FACING: set mob to face: " + mobNode.MobController.Target);
        }

     public void ClearWorld()
        {
            // Remove all objects from the scene
            //Monitor.Enter (sceneManager);
            try
            {
                lock (nodeDictionary)
                {
                    // Clone the world dictionary, so that if an object's 
                    // Dispose method tries to remove it from the dictionary, 
                    // it won't cause a concurrent modification exception.
                    List<AtavismObjectNode> nodes = new List<AtavismObjectNode>(nodeDictionary.Values);
                    foreach (AtavismObjectNode node in nodes)
                    {
                        try
                        {
                            if (node is AtavismObjectNode)
                                RemoveNode(node as AtavismObjectNode);
                            //else if (node is LightEntity)
                            //	RemoveNode (node as LightEntity);
                        }
                        catch (Exception e)
                        {
                            AtavismLogger.LogError("Exception caught while removing node: " + e);
                            // Just ignore the exception here
                        }
                        //node.Dispose ();
                    }
                    nodeDictionary.Clear();
                }

                modelsToLoad.Clear();
          SetPlayer(null);
                playerId = 0;
                playerInitialized = false;
                //terrainInitialized = false;
                playerStubInitialized = false;
                loadCompleted = false;
              
            }
            finally
            {
                //Monitor.Exit (sceneManager);
            }
        }

        public void Disconnected()
        {
            networkHelper.Disconnect();
            ClearWorld();
            if (player != null)
            {
                GameObject.Destroy(player.GameObject);
                PlayerDisposed(player);
                playerId = 0;
                playerInitialized = false;
            }
            if (AtavismClient.Instance != null)
                AtavismClient.Instance.LoadLevel(AtavismClient.Instance.LoginScene);
            else
                SceneManager.LoadScene(0);
            Debug.Log("Loading Login Scene");
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("DISCONNECTED", args);
        }

        public void RequestShutdown(string message)
        {
            //if (client != null)
            //    client.RequestShutdown(message);
        }

        #region Properties

        public long PlayerId
        {
            get
            {
                //Monitor.Enter(this);
                long rv = playerId;
                //Monitor.Exit(this);
                return rv;
            }
            set
            {
                //Monitor.Enter(this);
                playerId = value;
                playerInitialized = true;
                //SetPlayer(null);
                //Monitor.Exit(this);
            }
        }

        public AtavismPlayer Player
        {
            get
            {
                //Monitor.Enter (this);
                AtavismPlayer rv = player;
                //Monitor.Exit (this);
                return rv;
            }
        }

        public bool PlayerInitialized
        {
            get
            {
                //Monitor.Enter(this);
                bool rv = playerInitialized;
                //Monitor.Exit(this);
                return rv;
            }
        }

        public bool PlayerStubInitialized
        {
            get
            {
                return playerStubInitialized;
            }
        }

        public AtavismInputController ActiveInputController
        {
            get
            {
                return activeInputController;
            }
        }

        public long TargetId
        {
            get
            {
                //Monitor.Enter(this);
                long rv = targetId;
                //Monitor.Exit(this);
                return rv;
            }
            set
            {
                //Monitor.Enter(this);
                if (targetId != -1 && GetObjectNode(targetId) != null)
                {
                    if (GetObjectNode(targetId).GameObject!=null)
                        GetObjectNode(targetId).GameObject.SendMessage("StopTarget");
                }
                targetId = value;
                string[] args = new string[1];
                if (targetId != -1)
                {
                    if (nodeDictionary.ContainsKey(targetId))
                    {
                        target = (AtavismMobNode)nodeDictionary[targetId];
                        args[0] = targetId.ToString();
                    }
                    else
                    {
                        target = null;
                        args[0] = "";
                    }
                }
                else
                {
                    target = null;
                    args[0] = "";
                }
                AtavismEventSystem.DispatchEvent("PLAYER_TARGET_CHANGED", args);
                if (GetObjectNode(targetId) != null)
                    if (GetObjectNode(targetId).GameObject != null)
                        GetObjectNode(targetId).GameObject.SendMessage("StartTarget");
                //Monitor.Exit(this);
            }
        }

        public AtavismMobNode Target
        {
            get
            {
                //Monitor.Enter (this);
                AtavismMobNode rv = target;
                //Monitor.Exit (this);
                return rv;
            }
        }

        public bool LoadCompleted
        {
            get
            {
                return loadCompleted;
            }
        }

        public AtavismNetworkHelper NetworkHelper
        {
            set
            {
                networkHelper = value;
            }
            get
            {
                return networkHelper;
            }
        }

        public static long CurrentTime
        {

            get
            {
                return AtavismTimeTool.CurrentTime;
            }
        }

        // Kludge: This is referenced by interface IWorldManager, and
        // interfaces can't refer to static properties, so define this
        // non-static property
        public long CurrentTimeValue
        {
            get
            {
                return AtavismWorldManager.CurrentTime;
            }
        }

        public string ServerCapabilities
        {
            set
            {
                string[] capabilities = value.Split(',');
                // For now, we're only interested in DirLocEvent
                foreach (string s in capabilities)
                {
                    string ts = s.Trim();
                    if (ts == "DirLocEvent")
                        dirLocOrientSupported = true;
                }
            }
        }

        public int RequestedWorld
        {
            get
            {
                return requestedWorld;
            }
        }

        public float StaticTimeBetweenUpdates
        {
            get
            {
                return staticTimeBetweenUpdates;
            }
            set
            {
                staticTimeBetweenUpdates = value;
            }
        }

        public float MovingTimeBetweenUpdates
        {
            get
            {
                return movingTimeBetweenUpdates;
            }
            set
            {
                movingTimeBetweenUpdates = value;
            }
        }

        public bool SendPlayerUpdateLocation
        {
            get
            {
                return sendPlayerUpdateLocation;
            }
            set
            {
                sendPlayerUpdateLocation = value;
            }
        }
        public float MovingMaxSyncDistance
        {
            get
            {
                return movingMaxSyncDistance;
            }
            set
            {
                movingMaxSyncDistance = value;
            }
        }

        public float StaticMaxSyncDistance
        {
            get
            {
                return staticMaxSyncDistance;
            }
            set
            {
                staticMaxSyncDistance = value;
            }
        }

        #endregion
    }
}