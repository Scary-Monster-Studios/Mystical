using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using System.Globalization;
using System.Threading;

namespace Atavism
{

    public class ClientAPI : MonoBehaviour
    {
        static ClientAPI instance;
        [HideInInspector]
        public GameObject loginController;
        public GameObject defaultObject;
        //public string mobTag;
        public LayerMask playerLayer;
        public string playerTag;
        public GameObject scriptObject;
        public AtavismResourceManager resourceManager;
        public bool ignoreServerPropObjects = true;
        public bool checkAtavismCloudServerStatus = true;
        public bool resyncMobs = true;
        public float movingTimeBetweenUpdates = 2.5f;
        public float staticTimeBetweenUpdates = 5f;
        public float movingMaxSyncDistance = 4;
        public float staticMaxSyncDistance = 1;
        public float maxTimeBetweenMessages = 120;
        public float smoothRotation = 10f;
        public string masterServer = "";
        public ushort masterServerRdpPort = 9010;
        public ushort masterServerTcpPort = 9005;
        public string characterSceneName = "CharacterSelection";
        public string prefabServer = "";
        public ushort prefabServerPort = 5566;
        [HideInInspector]
        public long mobOidDebug = 0L;
        //public string WorldId = "atavism_demo";
        bool webPlayer = false;
        public LogLevel logLevel = LogLevel.Info;

        public static AtavismObjectNode mouseOverTarget;
        public static bool mouseLook = false;
        //  public bool streamer = false;
        public float spawnMobAdditionalY = 0.1f;
        
        public int mobLoadFrameSkip = 10;
        public float despawnDelay = 2f;
        void Awake()
        {
            CultureInfo ci = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                GameObject.DestroyImmediate(gameObject);
                return;
            }
            if (scriptObject == null)
            {
                Debug.LogError("Scripts GameObject is not assign to scriptObject in Login scene",gameObject);
                scriptObject = GameObject.FindGameObjectWithTag("ScriptUpdater");
            }
            AtavismClient client = gameObject.AddComponent<AtavismClient>();
            if (prefabServer == "")
                prefabServer = masterServer;
            client.Initalise(scriptObject, defaultObject, characterSceneName.Trim(), "", webPlayer, logLevel, playerLayer,
                playerTag, maxTimeBetweenMessages,prefabServer.Trim(),prefabServerPort);
            client.DefaultMasterServer = masterServer.Trim();
            client.MasterServer = masterServer.Trim();
            client.MasterServerRdpPort = masterServerRdpPort;
            client.MasterServerTcpPort = masterServerTcpPort;
            client.mobLoadFrameSkip = mobLoadFrameSkip;
            client.despawnDelay = despawnDelay;
            client.smoothRotation = smoothRotation;
            client.DefaultWorldId = "";
            if (resourceManager != null)
            {
                client.resourceManager = resourceManager;
            }
            client.IgnoreProps = ignoreServerPropObjects;
            client.ResyncMobs = resyncMobs;
            client.WorldManager.MovingTimeBetweenUpdates = movingTimeBetweenUpdates;
            client.WorldManager.StaticTimeBetweenUpdates = staticTimeBetweenUpdates;
            client.WorldManager.MovingMaxSyncDistance = movingMaxSyncDistance;
            client.WorldManager.StaticMaxSyncDistance = staticMaxSyncDistance;
#if AT_STREAMER || AT_STREAMER2
        client.streamer = true;
#else
            client.streamer = false;

#endif
            ScriptObject.BroadcastMessage("ClientReady");

         /*   if (checkAtavismCloudServerStatus && masterServer.Contains("atavismaws.com"))
            {
                string serverStatus = client.CheckCloudServer();
                Debug.Log("Server Status: " + serverStatus);
                if (!serverStatus.StartsWith("Running"))
                {
                    loginController.SendMessage("ShowDialogWithButton", serverStatus);
                }
            }*/

            // Read in public key for encrypting messages
            TextAsset bindata = Resources.Load("atavismkey") as TextAsset;
            if (bindata != null)
            {
                AtavismEncryption.ImportPublicKey(bindata.text);
            }
            else
            {
                Debug.Log("No key file found. Data will not be encrypted.");
            }

            /*string filePath = Directory.GetCurrentDirectory() + "/Assets/atavism.pem";
            FileInfo keyFile = new FileInfo (filePath);
            try {
                StreamReader reader = keyFile.OpenText();
                string publicKey = reader.ReadToEnd();
                AtavismEncryption.ImportPublicKey(publicKey);
            } catch (FileNotFoundException e) {
                Debug.Log("No key file found. Data will not be encrypted.");
            }*/
        }

        public static void Write(string message)
        {
            string[] eventArgs = new string[1];
            eventArgs[0] = message;
            AtavismEventSystem.DispatchEvent("CHAT_MSG_SYSTEM", eventArgs);
        }

        public static AtavismPlayer GetPlayerObject()
        {
            return AtavismClient.Instance.WorldManager.Player;
        }

        public static long GetPlayerOid()
        {
            return AtavismClient.Instance.WorldManager.PlayerId;
        }

        public static AtavismMobNode GetTargetObject()
        {
            return AtavismClient.Instance.WorldManager.Target;
        }

        public static long GetTargetOid()
        {
            return AtavismClient.Instance.WorldManager.TargetId;
        }

        public static void SetTarget(long oid)
        {
            WorldManager.TargetId = oid;
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("CLAIM_TARGET_CLEAR", args);

        }

        public static void ClearTarget()
        {
            WorldManager.TargetId = -1;
            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("TARGET_CLEAR", args);

        }

        public static AtavismObjectNode GetObjectNode(long oid)
        {
            if (WorldManager.GetObjectNode(oid) != null)
                return WorldManager.GetObjectNode(oid);
            else
                return null;
        }

        public static object GetObjectProperty(long oid, string propName)
        {
            if (WorldManager.GetObjectNode(oid) != null)
                return WorldManager.GetObjectNode(oid).GetProperty(propName);
            else
                return null;
        }

        public static void InputControllerActivated(AtavismInputController inputController)
        {
            AtavismClient.Instance.WorldManager.InputControllerActivated(inputController);
        }

        public static AtavismInputController GetInputController()
        {
            return AtavismClient.Instance.ActiveInputController;
            //return (AtavismInputController)Client.Instance.WorldManager.Player.GameObject.GetComponent(Client.Instance.InputController);
        }

        public static List<CharacterEntry> GetCharacterEntries()
        {
            return AtavismClient.Instance.CharacterEntries;
        }

        public static void RegisterEventHandler(string eventName, System.EventHandler eventHandler)
        {
            /*if (eventName in _newWorldEvents) {
            _deprecated("1.1", "ClientAPI.RegisterEventHandler('" + eventName + "')", "ClientAPI.World.RegisterEventHandler('" + eventName + "')");
            World.RegisterEventHandler(eventName, eventHandler);
            }
        else*/
            if (eventName == "WorldConnect")
                AtavismAPI.WorldConnect += eventHandler;
            else if (eventName == "WorldDisconnect")
                AtavismAPI.WorldDisconnect += eventHandler;
            else if (eventName == "FrameStarted")
                AtavismAPI.FrameStarted += new FrameStartedHandler(eventHandler);
            else if (eventName == "FrameEnded")
                AtavismAPI.FrameEnded += new FrameEndedHandler(eventHandler);
            else if (eventName == "CameraWaterEvent")
                AtavismAPI.CameraWaterEvent += new WaterEventHandler(eventHandler);
            else if (eventName == "ZoneEvent")
            {
                AtavismAPI.ZoneEvent += new ZoneEventHandler(eventHandler);
            }
            else if (eventName == "PlayerInitialized")
                WorldManager.PlayerInitializedEvent += new PlayerInitializedHandler(eventHandler);
            //else
            //	ClientAPI.LogError ("Invalid event name '%s' passed to ClientAPI.RegisterEventHandler" % str (eventName));
        }

        public static void SetBlockIntercept(int x, int y, int z, int type)
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("voxelandID", 1);
            props.Add("blockx", x);
            props.Add("blocky", y);
            props.Add("blockz", z);
            props.Add("type", type);
            NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.SET_BLOCK", props);
            GetPlayerObject().MobController.PlayAnimation("mining", 1.5f);
        }

        public static bool UIHasFocus()
        {
            if (EventSystem.current.currentSelectedGameObject != null && (EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null
                || EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() != null))
                return true;
            return false;
        }

        public static bool IsPlayerAdmin()
        {
            int adminLevel = (int)ClientAPI.GetObjectProperty(ClientAPI.GetPlayerOid(), "adminLevel");
            if (adminLevel >= 5)
                return true;
            return false;
        }

        public void Quit()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        public static ClientAPI Instance
        {
            get
            {
                return instance;
            }
        }

        public static AtavismWorldManager WorldManager
        {
            get
            {
                return AtavismClient.Instance.WorldManager;
            }
        }

        public static GameObject ScriptObject
        {
            get
            {
                return instance.scriptObject;
            }
        }
    }
}