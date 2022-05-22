//using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Atavism
{

    public class AtavismGameWorld : IGameWorld
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        static AtavismGameWorld instance;

        protected AtavismClient client;
        protected AtavismWorldManager worldManager;

        /// <summary>
        ///   Simple constructor that takes a reference to the game client, 
        ///   and sets up the basic game state.
        ///   In this case, that state contains information about inventory,
        ///   quests, group membership, and the colors to use for chat.
        /// </summary>
        /// <param name="client">the client class</param>
        public AtavismGameWorld(AtavismClient client)
        {
            this.client = client;

            // Set up the global instance
            instance = this;
        }

        /// <summary>
        ///   Sets up the default cursor.
        /// </summary>
        public void Initialize()
        {
            // Set up the default cursor
            //UiSystem.SetCursor(3, "Interface\\Cursor\\Point");
        }

        /// <summary>
        ///   Clean up our game specific information
        /// </summary>
		public void Dispose()
        {
            this.WorldManager = null;
        }

    #region Message handlers
        /// <summary>
        ///   Special handling for DirectionMessage objects.
        ///   This updates the collision volumes if needed.
        /// </summary>
        /// <param name="message">the message object from the server</param>
        public void HandleDirection(BaseWorldMessage message)
        {
            DirectionMessage dirMessage = message as DirectionMessage;
            
        }

        public void HandleObjectProperty(BaseWorldMessage message)
        {
            ObjectPropertyMessage propMessage = (ObjectPropertyMessage)message;
            // Debugging
            if (AtavismLogger.logLevel == LogLevel.Debug)
            {
                foreach (string key in propMessage.Properties.Keys)
                {
                    object val = propMessage.Properties[key];
                    if (message.Oid.Equals(WorldManager.PlayerId))
                        AtavismLogger.LogDebugMessage("HandleObjectProperty for OID " + message.Oid + ", setting prop " + key + " = " + val);
                }
            }
            HandleObjectPropertyHelper(message.Oid, propMessage.Properties);
        }

        private void HandleObjectPropertyHelper(long oid, Dictionary<string, object> props)
        {
            if (props.Count <= 0)
                return;
            AtavismObjectNode node = worldManager.GetObjectNode(oid);
            if (node == null)
            {
                if (!objectPropertyQueue.ContainsKey(oid))
                    objectPropertyQueue.Add(oid,new Queue<Dictionary<string, object>>());
                objectPropertyQueue[oid].Enqueue(props);
                AtavismLogger.LogWarning("Got stat update message for nonexistent object: " + oid);
                return;
            }
            AtavismLogger.LogWarning("Got stat update message for object: " + oid + " queue size "+ (objectPropertyQueue.ContainsKey(oid)? objectPropertyQueue[oid].Count.ToString():" none"));
            if (objectPropertyQueue.ContainsKey(oid))
            {
                while (objectPropertyQueue[oid].Count > 0)
                {
                    node.UpdateProperties(objectPropertyQueue[oid].Dequeue());
                }
            }
            node.UpdateProperties(props);
        }
        Dictionary<long, Queue<Dictionary<string, object>>> objectPropertyQueue = new Dictionary<long, Queue<Dictionary<string, object>>>();

        #endregion

        /// <summary>
        ///   Register our message handlers with the message callback system
        ///   so that we can have custom logic associated with incoming 
        ///   messages.
        /// </summary>
        public void SetupMessageHandlers()
        {
            // Register for notification about object movement, so we can update the collision data
            MessageDispatcher.Instance.RegisterHandler(WorldMessageType.Direction,
                                                       new WorldMessageHandler(this.HandleDirection));

            // Replacement for the StatUpdate and StateMessage handlers
            MessageDispatcher.Instance.RegisterHandler(WorldMessageType.ObjectProperty,
                                                       new WorldMessageHandler(this.HandleObjectProperty));
        }

        /// <summary>
        ///   This method is called when the player enters the world.
        ///   Generally this only happens when the player logs in.
        ///   This injects 'PLAYER_ENTERING_WORLD' and 'UNIT_NAME_UPDATE' 
        ///   UI events so that any interested widgets can update their data.
        /// </summary>
        public void PlayerEnteringWorld()
        {
           
        }

        /// <summary>
        ///   Writes a message using the specified channel.
        /// </summary>
        /// <param name="message">the message to write</param>
        public void Write(string message)
        {
           
        }

        /// <summary>
        ///   Get or set the WorldManager object that keeps track of objects in the world.
        /// </summary>
        public AtavismWorldManager WorldManager
        {
            get
            {
                return worldManager;
            }
            set
            {
                worldManager = value;
              
            }
        }

        /// <summary>
        ///   Get the singleton
        /// </summary>
        public static AtavismGameWorld Instance
        {
            get
            {
                return instance;
            }
        }
    }
}