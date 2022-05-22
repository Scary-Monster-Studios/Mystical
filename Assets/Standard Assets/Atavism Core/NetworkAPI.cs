using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.SceneManagement;

namespace Atavism
{

  
    public class NetworkAPI : MonoBehaviour
    {

        static NetworkAPI instance;

        static Dictionary<string, List<ExtensionMessageHandler>> extensionHandlers = new Dictionary<string, List<ExtensionMessageHandler>>();
        static float time = 0;
        static bool checkHB = false;
        static float shtime = 0;
        // Use this for initialization
        void Start()
        {
            if (instance != null)
            {
                return;
            }
            instance = this;

            MessageDispatcher.Instance.RegisterHandler(WorldMessageType.Extension, HandleExtensionMessage);
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name.Equals("Login") || arg0.name.Equals(ClientAPI.Instance.characterSceneName))
            {
                time = 0;
                checkHB = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            //if (Time.time - time >)
            if (AtavismClient.Instance.WorldManager.sceneInLoading)
            {
                time = Time.time;
            }
            if (checkHB && !AtavismClient.Instance.WorldManager.sceneInLoading)
            {
                if ((Time.time - time) > 20)
                {
                    //SceneManager.LoadScene("Login");
                    time = Time.time;
                    AtavismClient.Instance.LoadLevel("Login");
                }
              //   Debug.LogError("Hartbeat time=" + (Time.time - time));
             //   shtime += Time.deltaTime;
             //   Debug.LogError("Hartbeat shtime=" + shtime+" dt"+ Time.deltaTime+" DtE"+ (Time.time - time));

            }
        }


        #region Methods to send messages
        public static CharacterEntry CreateCharacter(Dictionary<string, object> attrs)
        {
            return AtavismClient.Instance.NetworkHelper.CreateCharacter(attrs);
        }

        public static void DeleteCharacter(Dictionary<string, object> attrs)
        {
            AtavismClient.Instance.NetworkHelper.DeleteCharacter(attrs);
        }

        public static void SendMessage(BaseWorldMessage message)
        {
            AtavismClient.Instance.NetworkHelper.SendMessage(message);
        }

        public static void SendQuestResponseMessage(long objectId, long questId, bool accepted)
        {
            QuestResponseMessage message = new QuestResponseMessage();
            message.ObjectId = objectId;
            message.QuestId = questId;
            message.Accepted = accepted;
            AtavismClient.Instance.NetworkHelper.SendMessage(message);
        }

        public static void SendCommMessage(string text)
        {
            CommMessage message = new CommMessage();
            message.ChannelId = 1; // CommChannel.Say
            message.Message = text;
            message.SenderName = ClientAPI.GetPlayerObject().Name;
            AtavismClient.Instance.NetworkHelper.SendMessage(message);
        }

        public static void SendAcquireMessage(long objectId)
        {
            AcquireMessage message = new AcquireMessage();
            message.ObjectId = objectId;
            AtavismClient.Instance.NetworkHelper.SendMessage(message);
        }

        public static void SendEquipMessage(long objectId, string slotName)
        {
            EquipMessage message = new EquipMessage();
            message.ObjectId = objectId;
            message.SlotName = slotName;
            AtavismClient.Instance.NetworkHelper.SendMessage(message);
        }

        public static void SendAttackMessage(long objectId, string attackType, bool attackStatus)
        {
            AutoAttackMessage message = new AutoAttackMessage();
            message.ObjectId = objectId;
            message.AttackType = attackType;
            message.AttackStatus = attackStatus;
            AtavismClient.Instance.NetworkHelper.SendMessage(message);
        }

        public static void SendLogoutMessage()
        {
            LogoutMessage message = new LogoutMessage();
            AtavismClient.Instance.NetworkHelper.SendMessage(message);
        }

        public static void SendTargetedCommand(long objectId, string text)
        {
            CommandMessage message = new CommandMessage();
            message.ObjectId = objectId;
            message.Command = text;
            AtavismClient.Instance.NetworkHelper.SendMessage(message);
        }

        public static void SendQuestInfoRequestMessage(long objectId)
        {
            QuestInfoRequestMessage message = new QuestInfoRequestMessage();
            message.ObjectId = objectId;
            AtavismClient.Instance.NetworkHelper.SendMessage(message);
        }

        public static void SendQuestConcludeRequestMessage(long objectId)
        {
            QuestConcludeRequestMessage message = new QuestConcludeRequestMessage();
            message.ObjectId = objectId;
            AtavismClient.Instance.NetworkHelper.SendMessage(message);
        }

        public static void SendTradeOffer(long partnerId, List<long> itemIds, bool accepted, bool cancelled)
        {
            TradeOfferRequestMessage message = new TradeOfferRequestMessage();
            message.Oid = ClientAPI.GetPlayerObject().Oid;
            message.ObjectId = partnerId;
            message.Accepted = accepted;
            message.Cancelled = cancelled;
            message.Offer = itemIds;
            AtavismClient.Instance.NetworkHelper.SendMessage(message);
        }

        public static void SendActivateItemMessage(OID itemId, long objectId)
        {
            ActivateItemMessage message = new ActivateItemMessage();
            message.ItemId = itemId;
            message.ObjectId = objectId;
            AtavismClient.Instance.NetworkHelper.SendMessage(message);
        }

        public static void SendExtensionMessage(long targetOid, bool clientTargeted, string extensionType, Dictionary<string, object> properties)
        {
          //  Debug.LogError("SendExtensionMessage targetOid="+targetOid + " clientTargeted=" + clientTargeted + " extensionType=" + extensionType + " properties=" + properties);
            ExtensionMessage message = new ExtensionMessage(targetOid, clientTargeted);
            foreach (string key in properties.Keys)
            {
                message.Properties[key] = properties[key];
            }
            message.Properties["ext_msg_subtype"] = extensionType;
            AtavismClient.Instance.NetworkHelper.SendMessage(message);
        }

        #endregion Methods to send messages

        public static void RegisterExtensionMessageHandler(string extensionType, ExtensionMessageHandler handler)
        {
            List<ExtensionMessageHandler> handlers;
            if (extensionHandlers.ContainsKey(extensionType))
                handlers = extensionHandlers[extensionType];
            else
                handlers = new List<ExtensionMessageHandler>();
            handlers.Add(handler);
            extensionHandlers[extensionType] = handlers;
        }

        public static void RemoveExtensionMessageHandler(string extensionType, ExtensionMessageHandler handler)
        {
            if (extensionHandlers.ContainsKey(extensionType))
            {
                List<ExtensionMessageHandler> handlers = extensionHandlers[extensionType];
                handlers.Remove(handler);
                if (handlers.Count == 0)
                    extensionHandlers.Remove(extensionType);
            }
        }

        public static void HandleExtensionMessage(BaseMessage message)
        {
            if (message == null || !(message is ExtensionMessage))
                return;
            ExtensionMessage extMessage = (ExtensionMessage)message;
            string extensionType = null;
            if (extMessage.Properties.ContainsKey("ext_msg_subtype"))
            {
                extensionType = (string)extMessage.Properties["ext_msg_subtype"];
            }
            else if (extMessage.Properties.ContainsKey("ext_msg_type"))
            {
                AtavismLogger.LogWarning("Extension message with 'ext_msg_type' is deprecated. Please use 'ext_msg_subtype'");
                extensionType = (string)extMessage.Properties["ext_msg_type"];
            }
            else
            {
                AtavismLogger.LogWarning("Received extension message without a subtype");
                return;
            }
            AtavismLogger.LogDebugMessage("Got extension message with type: " + extensionType);
            CultureInfo ci = CultureInfo.InvariantCulture;
            DateTime date1 = DateTime.Now;
          // Debug.LogError(date1.ToString("hh:mm:ss.fff", ci)+" Got extension message with type: " + extensionType);
          if (extensionType.Equals("a.SCENE_BEGIN"))
          {
              //      Debug.LogError(date1.ToString("hh:mm:ss.fff", ci)+" Got HeartBeat message with type: " + extensionType);
              AtavismClient.Instance.WorldManager.SendPlayerUpdateLocation = false;
              if (extensionHandlers.ContainsKey(extensionType))
              {
                  extMessage.Properties["ext_msg_subject_oid"] = extMessage.Oid;
                  extMessage.Properties["ext_msg_target_oid"] = extMessage.TargetOid;
                  extMessage.Properties["ext_msg_client_targeted"] = extMessage.ClientTargeted;
                  List<ExtensionMessageHandler> handlers = extensionHandlers[extensionType];
                  AtavismLogger.LogDebugMessage("Got " + handlers.Count + " handlers for extension message");
                  foreach (ExtensionMessageHandler handler in handlers)
                      handler(extMessage.Properties);
                  
              }
          }else if (extensionType.Equals("a.SCENE_END"))
          {
              //      Debug.LogError(date1.ToString("hh:mm:ss.fff", ci)+" Got HeartBeat message with type: " + extensionType);
              AtavismClient.Instance.WorldManager.SendPlayerUpdateLocation = true;
              if (extensionHandlers.ContainsKey(extensionType))
              {
                  extMessage.Properties["ext_msg_subject_oid"] = extMessage.Oid;
                  extMessage.Properties["ext_msg_target_oid"] = extMessage.TargetOid;
                  extMessage.Properties["ext_msg_client_targeted"] = extMessage.ClientTargeted;
                  List<ExtensionMessageHandler> handlers = extensionHandlers[extensionType];
                  AtavismLogger.LogDebugMessage("Got " + handlers.Count + " handlers for extension message");
                  foreach (ExtensionMessageHandler handler in handlers)
                      handler(extMessage.Properties);
              }

          }else if (extensionType.Equals("ao.heartbeat"))
            {
          //      Debug.LogError(date1.ToString("hh:mm:ss.fff", ci)+" Got HeartBeat message with type: " + extensionType);
                time = Time.time;
                checkHB = true;
                shtime = 0;
            }
            else if (extensionHandlers.ContainsKey(extensionType))
            {
                extMessage.Properties["ext_msg_subject_oid"] = extMessage.Oid;
                extMessage.Properties["ext_msg_target_oid"] = extMessage.TargetOid;
                extMessage.Properties["ext_msg_client_targeted"] = extMessage.ClientTargeted;
                AtavismLogger.LogDebugMessage("ExtensionMessage extensionType=" + extensionType + " handler");
                List<ExtensionMessageHandler> handlers = extensionHandlers[extensionType];
                AtavismLogger.LogDebugMessage("Got " + handlers.Count + " handlers for extension message");
                foreach (ExtensionMessageHandler handler in handlers)
                    handler(extMessage.Properties);
            }
            else
            {
                if(AtavismLogger.logLevel <= LogLevel.Warning) 
                    Debug.LogWarning("ExtensionMessage extensionType=" + extensionType + " does not have handler");
            }
        }
    }
}