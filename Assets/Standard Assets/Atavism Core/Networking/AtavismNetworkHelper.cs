using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Threading;
namespace Atavism
{
    public delegate void ExtensionMessageHandler(Dictionary<string, object> props);

    public class LoginSettings
    {
        public string tcpServer;
        public ushort tcpPort = 9005;
        public string rdpServer;
        public ushort rdpPort = 9010;
        public string username;
        public string password;
        public string loginUrl;
        public string worldId;
        public bool createAccount = false;
        public string emailAddress;
        public Dictionary<string, object> props;
        public OID characterId = null;

        public override string ToString()
        {
            return string.Format("{0}:{1} {2}/*****", tcpServer, tcpPort, username);
        }
    }

    public class CharacterEntry : Dictionary<string, object>
    {
        public CharacterEntry(Dictionary<string, object> other)
            : base(other)
        {
        }
        public OID CharacterId
        {
            get
            {
                return (OID)this["characterId"];
            }
        }
        public string Hostname
        {
            get
            {
                return this.ContainsKey("hostname") ? (string)this["hostname"] : null;
            }
        }
        public int Port
        {
            get
            {
                return this.ContainsKey("port") ? (int)this["port"] : 0;
            }
        }
        public bool Status
        {
            get
            {
                if (!this.ContainsKey("status"))
                    return true;
                return (bool)this["status"];
            }
        }
    }

    public class WorldServerEntry : Dictionary<string, object>
    {
        public WorldServerEntry(Dictionary<string, object> other)
            : base(other)
        {
        }

        protected bool standalone = false;

        public WorldServerEntry(string worldName, string hostname, int port)
        {
            //ClientAPI.Write ("Creating WorldServerEntry");
            AtavismLogger.LogInfoMessage("Creating WorldServerEntry: " + worldName + " with hostname: " + hostname+":"+port);
            this.Name = worldName;
            this.Hostname = hostname;
            this.Port = port;
        }

        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }
        public string Hostname
        {
            get
            {
                return this.ContainsKey("hostname") ? (string)this["hostname"] : null;
            }
            set
            {
                this["hostname"] = value;
            }
        }
        public int Port
        {
            get
            {
                return this.ContainsKey("port") ? (int)this["port"] : 0;
            }
            set
            {
                this["port"] = value;
            }
        }
        public bool Status
        {
            get
            {
                if (!this.ContainsKey("status"))
                    return true;
                return (bool)this["status"];
            }
        }
        public bool Standalone
        {
            get
            {
                return standalone;
            }
            set
            {
                standalone = value;
            }
        }
    }

    public enum NetworkHelperStatus
    {
        Success,
        LoginFailure,
        NoAccessFailure,
        BannedFailure,
        SubscriptionExpiredFailure,
        WorldConnectFailure,
        MasterConnectFailure,
        WorldTcpConnectFailure,
        MasterTcpConnectFailure,
        WorldResolveSuccess,
        WorldResolveFailure,
        UsernameUsed,
        EmailUsed,
        Standalone,
        UnsupportedClientVersion,
        Unknown,
        WorldEnterFailure,
        ServerMaintanance,

    }

    public enum LoginStatus
    {
        InvalidPassword = 0,
        Success = 1,
        AlreadyLoggedIn = 2,
        ServerError = 3,
        NoAccess = 4,
        Banned = 5,
        UsernameUsed = 6,
        EmailUsed = 7,
        DatabaseError = 8,
        SubscriptionExpired = 9,
        ServerMaintanance = 10
    }

    public delegate byte[] AsyncDelegate(ref IPEndPoint remoteEP);

    // The RdpMessageHandler and TcpWorldMessageHandler both implement this
    // interface
    public interface IMessageHandler : IDisposable
    {
        void SendMessage(AtavismOutgoingMessage outMessage);
        void BeginListen();

        long PacketsSentCounter
        {
            get;
        }
        long PacketsReceivedCounter
        {
            get;
        }
        long BytesSentCounter
        {
            get;
        }
        long BytesReceivedCounter
        {
            get;
        }
    }

    public abstract class RdpMessageHandler : IMessageHandler, IDisposable
    {
        // Create a logger for use in this class
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(RdpMessageHandler));

        private RdpClient rdp;
        private AsyncCallback pfnCallBack;
        private AsyncDelegate rdpDelegate;
        private AtavismRdpConnection rdpConn;

        public RdpMessageHandler(IPEndPoint remote, int localPort, int millisecondsTimeout)
        {
            rdp = new RdpClient(localPort);
            try
            {
                rdpConn = rdp.Connect(remote, millisecondsTimeout);
            }
            catch (Exception)
            {
                if (rdp != null)
                    rdp.Dispose();
                throw;
            }
        }

        public RdpMessageHandler(IPEndPoint remote, int millisecondsTimeout)
        {
            System.Random r = new System.Random();
            // Make up to 20 attempts (guessing a random port)
            for (int i = 0; i < 20; ++i)
            {
                int localPort = 5000 + (r.Next() % 1000);
                try
                {
                    rdp = new RdpClient(localPort);
                    //log.InfoFormat("Setting up RdpClient with localPort = {0}", localPort);
                }
                catch (Exception)
                {
                    continue;
                }
                if (rdp != null)
                    break;
            }
            if (rdp == null)
            {
                //log.Warn("Unable to establish local port for RDP");
                throw new Exception("Unable to establish local port for RDP");
            }
            try
            {
                rdpConn = rdp.Connect(remote, millisecondsTimeout);
            }
            catch (Exception)
            {
                if (rdp != null)
                    rdp.Dispose();
                throw;
            }
        }

        #region low level packet callback stuff

        /// <summary>
        ///   Wait loop for the reliable UDP data
        /// </summary>
        private void WaitForRDPData()
        {
            // now start to listen for any data...
            rdpDelegate = new AsyncDelegate(rdpConn.Receive);
            IPEndPoint remoteIpEndPoint = null;
            IAsyncResult asyncResult =
                rdpDelegate.BeginInvoke(ref remoteIpEndPoint, this.OnRDPDataReceived, null);
        }

        /// <summary>
        ///   Callback for the reliable UDP data
        /// </summary>
        /// <param name="asyn"></param>
        private void OnRDPDataReceived(IAsyncResult asyn)
        {
            IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] buf = rdpDelegate.EndInvoke(ref remoteIpEndPoint, asyn);
            // Check to see if we got disposed.  If we did, don't bother 
            // handling this message, and don't wait for more.
            if (rdpConn == null)
                return;
            try
            {
                HandleMessage(buf, remoteIpEndPoint);
            }
            catch (Exception e)
            {
                //LogUtil.ExceptionLog.ErrorFormat("Exception: {0}", e);
                throw e;
            }
            WaitForRDPData();
        }

        #endregion // low level packet stuff

        public void BeginListen()
        {
            pfnCallBack = new AsyncCallback(OnRDPDataReceived);
            WaitForRDPData();
        }
        public void SendMessage(AtavismOutgoingMessage outMessage)
        {
            if (AtavismNetworkHelper.LogMessageContents)
            {
                byte[] buf = outMessage.GetBytes();
                //log.DebugFormat("RdpMessageHandler.Message: length {0}, packet {1}", 
                //    buf.Length, NetworkHelper.ByteArrayToHexString(buf, 0, buf.Length));
            }
            outMessage.Send(rdpConn);
        }

        public void Dispose()
        {
            if (rdpConn != null)
            {
                try
                {
                    if (!rdpConn.IsClosed)
                    {
                        rdpConn.Close();
                    }
                    else
                    {
                       // UnityEngine.Debug.LogError("rdpMaster rdpConn IsClosed");
                    }
                }
                catch (Exception e)
                {
                    // no throws from dispose
                    UnityEngine.Debug.LogError("rdpMaster rdpConn Exception "+e);

                }
                rdpConn = null;
            }
            else
            {
               // UnityEngine.Debug.LogError("rdpMaster rdpConn is null");
            }
            if (rdp != null)
            {

                rdp.Dispose();
            }
            else
            {
             //   UnityEngine.Debug.LogError("rdpMaster rdp is null");

            }
        }

        /// <summary>
        ///   Method to deal with incoming messages
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="endpoint"></param>
        protected abstract void HandleMessage(byte[] buf, IPEndPoint endpoint);

        public long PacketsSentCounter
        {
            get
            {
                if (rdpConn == null)
                    return 0;
                return rdpConn.PacketsSentCounter;
            }
        }
        public long PacketsReceivedCounter
        {
            get
            {
                if (rdpConn == null)
                    return 0;
                return rdpConn.PacketsReceivedCounter;
            }
        }
        public long BytesSentCounter
        {
            get
            {
                if (rdpConn == null)
                    return 0;
                return rdpConn.BytesSentCounter;
            }
        }
        public long BytesReceivedCounter
        {
            get
            {
                if (rdpConn == null)
                    return 0;
                return rdpConn.BytesReceivedCounter;
            }
        }
    }

    public class RdpWorldMessageHandler : RdpMessageHandler
    {
        // Create a logger for use in this class
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(RdpWorldMessageHandler));

        // const int LocalRdpPort = 6010;
        MessageDispatcher dispatcher = null;
        const int ConnectTimeoutMillis = 30000;//ZBDS default 10000


        public RdpWorldMessageHandler(IPEndPoint remote, MessageDispatcher dispatcher) :
            base(remote, ConnectTimeoutMillis)
        {
            this.dispatcher = dispatcher;
        }

        public RdpWorldMessageHandler(IPEndPoint remote) :
            base(remote, ConnectTimeoutMillis)
        {
        }

        protected override void HandleMessage(byte[] buffer, IPEndPoint remoteIpEndPoint)
        {
            if (AtavismNetworkHelper.LogMessageContents)
                if (buffer.Length == 0)
                    // probably a nul packet
                    return;
            if (buffer.Length < 8)
            {
                return;
            }
            AtavismIncomingMessage inMessage =
                new AtavismIncomingMessage(buffer, 0, buffer.Length, remoteIpEndPoint);
            BaseWorldMessage message = WorldMessageFactory.ReadMessage(inMessage);
            if (message == null)
            {
                //log.Warn("Failed to read message from factory");
                return;
            }
            //UnityEngine.Debug.Log("World message type: " + message.MessageType + "; Oid: " + message.Oid);
            if (dispatcher != null)
                dispatcher.QueueMessage(message);
            else
                MessageDispatcher.Instance.QueueMessage(message);
        }
    }

    public class TcpWorldMessageHandler : IMessageHandler
    {
        // Create a logger for use in this class

        private TcpClient tcpClient;
        MessageDispatcher dispatcher = null;
        IPEndPoint remoteEndPoint;
        private int receiveBufferSize = 32 * 1024;
        // The count of valid bytes in the buffer.  To get the offset
        // where we add bytes, add this to receiveBufferOffset
        private int bytesInReceiveBuffer = 0;
        // The offset at which we will _read_ the next bytes
        private int receiveBufferOffset = 0;
        // private int receiveBufferStartOfMessage = 0; (unused)
        private byte[] receiveBuffer;
        private byte[] currentMsgBuf;
        private int currentMsgOffset = 0;

        private int packetsSentCounter = 0;
        private int packetsReceivedCounter = 0;
        private int bytesSentCounter = 0;
        private int bytesReceivedCounter = 0;

        public TcpWorldMessageHandler(IPEndPoint remote, MessageDispatcher dispatcher)
        {
            remoteEndPoint = remote;
            Init();
            // This constructor arbitrarily assigns the local port number.
            tcpClient = new TcpClient(AddressFamily.InterNetworkV6);
            try
            {
                tcpClient.Connect(remote.Address, remote.Port);
            }
            catch (Exception e)
            {
                AtavismLogger.LogError(e.Message + "; stack trace\n" + e.StackTrace);
                tcpClient.Close();
                return;
            }
            tcpClient.Client.Blocking = false;
            tcpClient.Client.NoDelay = true;
        }

        private void Init()
        {
            receiveBuffer = new byte[receiveBufferSize];
        }

        public void SendMessage(AtavismOutgoingMessage outMessage)
        {
            if (tcpClient == null || tcpClient.Client == null)
                return;
            packetsSentCounter++;
            byte[] msgBytes = outMessage.GetBytes();
            int length = msgBytes.Length;
            byte[] msgBytesWithLength = new byte[length + 4];
            int cnt = 0;
            for (int i = 3; i >= 0; i--)
                msgBytesWithLength[cnt++] = (byte)((length >> (8 * i)) & 0xff);
            Array.Copy(msgBytes, 0, msgBytesWithLength, 4, length);
            try
            {
                tcpClient.Client.Send(msgBytesWithLength);
            }
            catch (Exception e)
            {
                AtavismLogger.LogError("In TcpWorldMessageHandler.SendMessage, exception: " + e.Message + "; stack trace\n" + e.StackTrace);
                if (tcpClient != null)
                    tcpClient.Client = null;
                return;
            }
            bytesSentCounter += length + 4;
        }

        protected void HandleMessage(byte[] buf, IPEndPoint endpoint)
        {
            if (AtavismNetworkHelper.LogMessageContents)
                if (buf.Length == 0)
                    // probably a nul packet
                    return;
            if (buf.Length < 8)
            {
                return;
            }
            packetsReceivedCounter++;
            AtavismIncomingMessage inMessage = new AtavismIncomingMessage(buf, 0, buf.Length, remoteEndPoint);
            BaseWorldMessage message = WorldMessageFactory.ReadMessage(inMessage);
            if (message == null)
            {
                return;
            }
            if (dispatcher != null)
                dispatcher.QueueMessage(message);
            else
                MessageDispatcher.Instance.QueueMessage(message);
        }

        public void BeginListen()
        {
            WaitForTcpData();
        }

        #region low level packet callback stuff

        private int GetMessageLength(int offset)
        {
            int length = 0;
            for (int i = 3; i >= 0; i--)
                length = length | (receiveBuffer[offset++] << (8 * i));
            return length;
        }

        private void WaitForTcpData()
        {
            // Begin receiving the data from the remote device.
            try
            {
                if (tcpClient == null || tcpClient.Client == null)
                    return;
                tcpClient.Client.BeginReceive(receiveBuffer, bytesInReceiveBuffer,
                    receiveBufferSize - bytesInReceiveBuffer, 0,
                    new AsyncCallback(OnTCPDataReceived), null);
            }
            catch (Exception e)
            {
                //LogUtil.ExceptionLog.ErrorFormat("In TcpWorldMessageHandler.WaitForTcpData, exception: {0}", e);
                if (tcpClient != null)
                    tcpClient.Client = null;
                AtavismNetworkHelper.Instance.RequestShutdown("TCP Connection closed by the server.  Exception was " + e.Message);
                return;
            }
        }

        private void AdvanceReceiveBuffer(int amount)
        {
            bytesInReceiveBuffer -= amount;
            receiveBufferOffset += amount;
        }

        /// <summary>
        ///   Callback for the TCP data
        /// </summary>
        /// <param name="asyn"></param>
        private void OnTCPDataReceived(IAsyncResult asyn)
        {
            int bytesReceived;
            try
            {
                try
                {
                    if (tcpClient == null || tcpClient.Client == null)
                        return;
                    bytesReceived = tcpClient.Client.EndReceive(asyn);
                }
                catch (Exception e)
                {
                    //LogUtil.ExceptionLog.ErrorFormat("In TcpWorldMessageHandler.OnTCPDataReceived, exception: {0}", e);
                    if (tcpClient != null)
                        tcpClient.Client = null;
                    AtavismNetworkHelper.Instance.RequestShutdown("TCP Connection closed by the server.  Exception was " + e.Message);
                    return;
                }
                bytesReceivedCounter += bytesReceived;
                bytesInReceiveBuffer += bytesReceived;
                // If we are still building a message from the last read
                if (currentMsgBuf != null)
                {
                    int bufLeft = currentMsgBuf.Length - currentMsgOffset;
                    int amountToCopy = Math.Min(bytesInReceiveBuffer, bufLeft);
                    Array.Copy(receiveBuffer, receiveBufferOffset, currentMsgBuf, currentMsgOffset, amountToCopy);
                    AdvanceReceiveBuffer(amountToCopy);
                    currentMsgOffset += amountToCopy;
                    if (amountToCopy == bufLeft)
                    {
                        // We have a complete message
                        HandleMessage(currentMsgBuf, remoteEndPoint);
                        currentMsgBuf = null;
                    }
                }
                while (bytesInReceiveBuffer >= 4)
                {
                    // Get the message length
                    int length = GetMessageLength(receiveBufferOffset);
                    AdvanceReceiveBuffer(4);
                    byte[] buf = new byte[length];
                    if (bytesInReceiveBuffer >= length)
                    {
                        Array.Copy(receiveBuffer, receiveBufferOffset, buf, 0, length);
                        HandleMessage(buf, remoteEndPoint);
                        AdvanceReceiveBuffer(length);
                    }
                    else
                    {
                        // We didn't get all the bytes required for this message
                        currentMsgBuf = buf;
                        currentMsgOffset = bytesInReceiveBuffer;
                        Array.Copy(receiveBuffer, receiveBufferOffset, buf, 0, bytesInReceiveBuffer);
                        AdvanceReceiveBuffer(bytesInReceiveBuffer);
                    }
                }
                if (bytesInReceiveBuffer == 0)
                    receiveBufferOffset = 0;
                else
                {
                    /*if (bytesInReceiveBuffer < 0 || bytesInReceiveBuffer > 3)
                        log.ErrorFormat("TcpWorldMessageHandler.OnTCPDataReceived: Bytes left illegal!  left {0} bytesReceived {1} receiveBufferOffset {2} currentMsgBuf {3}",
                            bytesInReceiveBuffer, bytesInReceiveBuffer, receiveBufferOffset, currentMsgBuf);
                    else {*/
                    Array.Copy(receiveBuffer, receiveBufferOffset, receiveBuffer, 0, bytesInReceiveBuffer);
                    receiveBufferOffset = 0;
                    //}
                }
            }
            catch (Exception e)
            {
                //log.Error("TcpWorldMessageHandler.OnTcpDataReceived: Exception " + e.Message + "; stack trace\n" + e.StackTrace);
            }
            WaitForTcpData();
        }

        #endregion // low level packet stuff

        public void Dispose()
        {
            if (tcpClient != null)
            {
                try
                {
                    tcpClient.Close();
                }
                catch (Exception)
                {
                    // no throws from dispose
                }
                tcpClient = null;
            }
        }

        public long PacketsSentCounter
        {
            get
            {
                if (tcpClient == null)
                    return 0;
                return packetsSentCounter;
            }
        }
        public long PacketsReceivedCounter
        {
            get
            {
                if (tcpClient == null)
                    return 0;
                return packetsReceivedCounter;
            }
        }
        public long BytesSentCounter
        {
            get
            {
                if (tcpClient == null)
                    return 0;
                return bytesSentCounter;
            }
        }
        public long BytesReceivedCounter
        {
            get
            {
                if (tcpClient == null)
                    return 0;
                return bytesReceivedCounter;
            }
        }
    }

    public class RdpMasterMessageHandler : RdpMessageHandler
    {
        // Create a logger for use in this class
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(RdpMasterMessageHandler));
        // const int LocalRdpPort = 6005;
        Dictionary<string, WorldServerEntry> worldServerMap;
        const int ConnectTimeoutMillis = 30000;//ZBDS default 10000


        public RdpMasterMessageHandler(IPEndPoint remote, Dictionary<string, WorldServerEntry> worldServerMap) :
                base(remote, ConnectTimeoutMillis)
        {
            this.worldServerMap = worldServerMap;
        }

        protected override void HandleMessage(byte[] buffer, IPEndPoint remoteIpEndPoint)
        {
            if (buffer.Length == 0)
                // probably a nul packet
                return;
            if (buffer.Length < 8)
            {
                //log.ErrorFormat("Invalid message length: {0}", buffer.Length);
                return;
            }
            AtavismIncomingMessage inMessage = new AtavismIncomingMessage(buffer, 0, buffer.Length, remoteIpEndPoint);
            BaseMasterMessage message = MasterMessageFactory.ReadMessage(inMessage);
            if (message == null)
                return;
            switch (message.MessageType)
            {
                case MasterMessageType.ResolveResponse:
                    {
                        ResolveResponseMessage rv = (ResolveResponseMessage)message;
                        if (rv.Status)
                        {
                            WorldServerEntry entry =
                                new WorldServerEntry(rv.WorldName, rv.Hostname, rv.Port);
                            Monitor.Enter(worldServerMap);
                            try
                            {
                                worldServerMap[rv.WorldName] = entry;
                                AtavismClient.Instance.WorldId = rv.WorldName;
                                Monitor.PulseAll(worldServerMap);
                            }
                            finally
                            {
                                Monitor.Exit(worldServerMap);
                            }
                        }
                        else
                        {
                            //log.ErrorFormat("Failed to resolve world name: {0}", rv.WorldName);
                        }
                    }
                    break;
                default:
                    //log.WarnFormat("Unknown message type from master server: {0}", message.MessageType);
                    break;
            }
        }
    }

    public class AtavismNetworkHelper
    {

        private static AtavismNetworkHelper instance = null;

        // Create a logger for use in this class
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(NetworkHelper));
        //private static readonly log4net.ILog log_status = log4net.LogManager.GetLogger("Network Information");

        /// <summary>
        ///   timeOffset = Client time (as a long) - Server time (as a ushort)
        /// </summary>
        private long timeOffset;

        /// <summary>
        ///   Token submitted to the login server for authentication
        /// </summary>
		byte[] authToken;
        /// <summary>
        ///   Token that should be submitted to the world server for authorization
        /// </summary>
        byte[] worldToken;

        string worldFilesUrl = "";
        long account;

        private IWorldManager worldManager;
        // private Client client;

        private Dictionary<string, WorldServerEntry> worldServerMap;
        private List<CharacterEntry> characterEntries;
        private int numCharacterSlots;
        private string loginPluginHost;

        string proxyPluginHost;
        int proxyPluginPort;

        IMessageHandler messageHandlerWorld;
        RdpMessageHandler rdpMaster;

        string status = "";

        /// <summary>
        ///   Should this client use TCP to connect to the proxy?
        /// </summary>
        private bool useTCP = false;

        protected TcpClient tcpWorldConnection = null;
        protected TcpClient tcpPrefabConnection = null;

        /// <summary>
        ///   Set this to true to log the contents of all messages.
        ///   The log must be at the debug level to see the messages.
        /// </summary>
        private static bool logMessageContents = false;

        public static string ByteArrayToHexString(byte[] inBuf, int startingByte, int length)
        {
            byte ch = 0x00;
            string[] pseudo = {"0", "1", "2",
                               "3", "4", "5", "6", "7", "8",
                               "9", "A", "B", "C", "D", "E",
                               "F"};
            StringBuilder outBuf = new StringBuilder(length * 2);
            StringBuilder charBuf = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                byte b = inBuf[i + startingByte];
                ch = (byte)(b & 0xF0);  // Strip off high nibble
                ch = (byte)(ch >> 4);                         // shift the bits down
                ch = (byte)(ch & 0x0F);                       // must do this if high order bit is on!
                outBuf.Append(pseudo[(int)ch]);                // convert the nibble to a String Character
                ch = (byte)(b & 0x0F);  // Strip off low nibble 
                outBuf.Append(pseudo[(int)ch]);              // convert the nibble to a String Character
                if (b >= 32 && b <= 126)
                    charBuf.Append((char)b);
                else
                    charBuf.Append("*");
            }
            return outBuf + " == " + charBuf;
        }

        #region Send methods

        /// <summary>
        ///   General purpose send message method that can send any type of message
        ///   This used to be private, but it is now public, so that plugins can
        ///   use this method.
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(BaseWorldMessage message)
        {
            //UnityEngine.Debug.Log("Got Send message");
            message.Oid = worldManager.PlayerId;
            //UnityEngine.Debug.Log("Added player id to the message - the messageHandlerWorld is: " + messageHandlerWorld);
            if (messageHandlerWorld != null && AtavismClient.Instance.WorldManager.sceneLoaded)
            {
                messageHandlerWorld.SendMessage(message.CreateMessage());
              //  UnityEngine.Debug.Log("Sending message: " + message.ToString() + " " + DateTime.Now.ToString("HH:mm:ss.ffffff"));
            }
            else
            {
                AtavismLogger.LogWarning("No message handler found to send message");
            }
        }

        public void SendHaMessage(long time)
        {
            HaMessage message = new HaMessage();
            message.Timestamp = time;
           
            SendMessage(message);
        }

        public void SendOrientationMessage(Quaternion orientation)
        {
            OrientationMessage message = new OrientationMessage();
            message.Orientation = orientation;

            SendMessage(message);
        }

        public void SendDirectionMessage(long timestamp, Vector3 dir, Vector3 pos)
        {
            DirectionMessage message = new DirectionMessage();
            message.Timestamp = GetServerTimestamp(timestamp);
            message.Direction = dir;
            message.Location = pos;

            SendMessage(message);
        }

        public void SendDirLocOrientMessage(long timestamp, Vector3 dir, Vector3 pos, Quaternion orientation, long updId)
        {
            DirLocOrientMessage message = new DirLocOrientMessage();
            message.Timestamp = GetServerTimestamp(timestamp);
            message.Direction = dir;
            message.Location = pos;
            message.Orientation = orientation;
            message.UpdateId = updId;
          //  UnityEngine.Debug.Log("Sending DirLoc with dir: " + dir + " and Direction: " + message.Direction);
            SendMessage(message);
        }

        public void SendCommMessage(string text)
        {
            CommMessage message = new CommMessage();
            message.ChannelId = (int)CommChannel.Say;
            message.Message = text;

            SendMessage(message);
        }

        public void SendAcquireMessage(long objectId)
        {
            AcquireMessage message = new AcquireMessage();
            message.ObjectId = objectId;
            SendMessage(message);
        }

        public void SendEquipMessage(long objectId, string slotName)
        {
            EquipMessage message = new EquipMessage();
            message.ObjectId = objectId;
            message.SlotName = slotName;
            SendMessage(message);
        }

        public void SendAttackMessage(long objectId, string attackType, bool attackStatus)
        {
            AutoAttackMessage message = new AutoAttackMessage();
            message.ObjectId = objectId;
            message.AttackType = attackType;
            message.AttackStatus = attackStatus;
            SendMessage(message);
        }

        /*public void SendLogoutMessage() {
			LogoutMessage message = new LogoutMessage();
			SendMessage(message);
		}*/

        public void SendSceneLoadedMessage()
        {
            SceneLoadedMessage message = new SceneLoadedMessage();
            SendMessage(message);
        }

        public void SendTargettedCommand(long objectId, string text)
        {
            CommandMessage message = new CommandMessage();
            message.ObjectId = objectId;
            message.Command = text;

            SendMessage(message);
        }

        public void SendQuestInfoRequestMessage(long objectId)
        {
            QuestInfoRequestMessage message = new QuestInfoRequestMessage();
            message.ObjectId = objectId;

            SendMessage(message);
        }

        public void SendQuestResponseMessage(long objectId, long questId, bool accepted)
        {
            QuestResponseMessage message = new QuestResponseMessage();
            message.ObjectId = objectId;
            message.QuestId = questId;
            message.Accepted = accepted;

            SendMessage(message);
        }

        public void SendQuestConcludeRequestMessage(long objectId)
        {
            QuestConcludeRequestMessage message = new QuestConcludeRequestMessage();
            message.ObjectId = objectId;

            SendMessage(message);
        }

        public void SendTradeOffer(long playerId, long partnerId, List<long> itemIds, bool accepted, bool cancelled)
        {
            TradeOfferRequestMessage message = new TradeOfferRequestMessage();
            message.Oid = playerId;
            message.ObjectId = partnerId;
            message.Offer = itemIds;
            message.Accepted = accepted;
            message.Cancelled = cancelled;

            SendMessage(message);
        }

        public void SendActivateItemMessage(OID itemId, long objectId)
        {
            ActivateItemMessage message = new ActivateItemMessage();
            message.ItemId = itemId;
            message.ObjectId = objectId;

            SendMessage(message);
        }

        #endregion // send methods

        public void RequestShutdown(string message)
        {
            worldManager.RequestShutdown(message);
        }

        /// <summary>
        ///   This call requires the tcp world connection to be established
        /// </summary>
        /// <param name="characterProperties"></param>
        /// <returns></returns>
        public CharacterEntry CreateCharacter(Dictionary<string, object> characterProperties)
        {
            AtavismLogger.LogInfoMessage("Creating character with num props: " + characterProperties.Count);
            // Make sure we have a connection
            System.Diagnostics.Debug.Assert(tcpWorldConnection != null);

            // Send the character creation message
            //Logger.LogInfoMessage("Sending create character message");
            WorldCharacterCreateMessage createMessage = new WorldCharacterCreateMessage();
            createMessage.Properties = characterProperties;
            AtavismOutgoingMessage outMessage = createMessage.CreateMessage();
            outMessage.Send(tcpWorldConnection);
            AtavismLogger.LogInfoMessage("Sent create character message");

            // Read a message off of our tcp stream
            AtavismIncomingMessage inMessage = new AtavismIncomingMessage(tcpWorldConnection.GetStream());
            BaseWorldTcpMessage response = WorldTcpMessageFactory.ReadMessage(inMessage);
            WorldCharacterCreateResponseMessage createResponse = response as WorldCharacterCreateResponseMessage;
            if (createResponse == null)
                throw new Exception("Invalid response to world tcp server character create");

            AtavismLogger.LogInfoMessage("Got response: " + createResponse.Properties.ToString());
            CharacterEntry newCharacter = new CharacterEntry(createResponse.Properties);
            if (!newCharacter.Status)
                return newCharacter;
            // Update our list of character entries
            characterEntries.RemoveAll(delegate (CharacterEntry entry)
            {
                return entry.CharacterId == newCharacter.CharacterId;
            });
            characterEntries.Add(newCharacter);

            return newCharacter;
        }

        /// <summary>
        ///   This call requires the tcp world connection to be established
        /// </summary>
        /// <param name="characterProperties"></param>
        /// <returns></returns>
        public void DeleteCharacter(Dictionary<string, object> characterProperties)
        {
            // Make sure we have a connection
            System.Diagnostics.Debug.Assert(tcpWorldConnection != null);

            // Send the character deletion message
            WorldCharacterDeleteMessage deleteMessage = new WorldCharacterDeleteMessage();
            deleteMessage.Properties = characterProperties;
            AtavismOutgoingMessage outMessage = deleteMessage.CreateMessage();
            outMessage.Send(tcpWorldConnection);

            // Read a message off of our tcp stream
            AtavismIncomingMessage inMessage = new AtavismIncomingMessage(tcpWorldConnection.GetStream());
            BaseWorldTcpMessage response = WorldTcpMessageFactory.ReadMessage(inMessage);
            WorldCharacterDeleteResponseMessage deleteResponse = response as WorldCharacterDeleteResponseMessage;
            if (deleteResponse == null)
                throw new Exception("Invalid response to world tcp server character deletion");

            OID characterId = (OID)deleteResponse.Properties["characterId"];
            // Update our list of character entries
            characterEntries.RemoveAll(delegate (CharacterEntry entry)
            {
                if (entry.CharacterId.compareTo(characterId) == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        /// <summary>
        ///   Sends a WorldServerListRequestMessage to the Login Server to get the list of 
        ///   servers available to connect to.
        /// </summary>
        /// <param name="clientVersion"></param>
        /// <returns></returns>
        public NetworkHelperStatus GetServerList(string clientVersion)
        {
            AtavismLogger.LogInfoMessage("Creating get server list message");
            WorldServerListRequestMessage characterRequest = new WorldServerListRequestMessage();
            characterRequest.Version = clientVersion;
            characterRequest.AuthToken = authToken;
            AtavismOutgoingMessage outMessage = characterRequest.CreateMessage();
            outMessage.Send(tcpWorldConnection);
            AtavismLogger.LogInfoMessage("Wrote message to tcp world server");

            // Read a message off of our tcp stream
            AtavismIncomingMessage inMessage = new AtavismIncomingMessage(tcpWorldConnection.GetStream());
            BaseWorldTcpMessage response = WorldTcpMessageFactory.ReadMessage(inMessage);
            WorldServerListResponseMessage serverResponse = response as WorldServerListResponseMessage;
            if (serverResponse == null)
                throw new Exception("Invalid response to server list request");

            AtavismLogger.LogInfoMessage("Read message from tcp world server");


            // Store the list of server entries, so it can be accessed later
            worldServerMap.Clear();
            foreach (WorldServerEntry worldEntry in serverResponse.ServerEntries)
            {
                worldServerMap[worldEntry.Name] = worldEntry;
            }

            NetworkHelperStatus rv;
            if (serverResponse.Error == string.Empty)
            {
                rv = NetworkHelperStatus.Success;
            }
            else if (serverResponse.Error == "Unsupported client version")
            {
                // custom logic to handle this case
                rv = NetworkHelperStatus.UnsupportedClientVersion;
            }
            else
            {
                // An unknown error
                //log.Warn("Bad login status response: " + characterResponse.Error);
                throw new Exception(serverResponse.Error);
            }
            return rv;
        }

        /// <summary>
        ///   Sends the heartbeat message to the Login server to verify the connection is still alive.
        /// </summary>
        /// <param name="clientVersion"></param>
        /// <returns></returns>
        public NetworkHelperStatus LoginServerHeartbeat(string clientVersion)
        {
            if (tcpWorldConnection == null || tcpWorldConnection.Connected == null)
            {
                AtavismLogger.LogInfoMessage("LoginServerHeartbeat "+ tcpWorldConnection+" "+(tcpWorldConnection != null? tcpWorldConnection.Connected+"":" 1null"));
                //  AtavismClient.Instance.Disconnected();
                return NetworkHelperStatus.WorldTcpConnectFailure;
            }
            if (!tcpWorldConnection.Connected)
            {
                AtavismLogger.LogInfoMessage("LoginServerHeartbeat  tcpWorldConnection  not connected ");

               // AtavismClient.Instance.Disconnected();
                return NetworkHelperStatus.WorldTcpConnectFailure;
            }
            AtavismLogger.LogInfoMessage("Creating server heartbeat message");
            WorldServerLoginHeartbeatMessage characterRequest = new WorldServerLoginHeartbeatMessage();
            characterRequest.Version = clientVersion;
            characterRequest.AuthToken = authToken;
            AtavismOutgoingMessage outMessage = characterRequest.CreateMessage();
            outMessage.Send(tcpWorldConnection);
            AtavismLogger.LogInfoMessage("Wrote message to tcp world server");

            // Read a message off of our tcp stream
            if (!tcpWorldConnection.Connected)
            {
                AtavismLogger.LogInfoMessage("LoginServerHeartbeat  tcpWorldConnection  not connected 2");
              //  AtavismClient.Instance.Disconnected();
                return NetworkHelperStatus.WorldTcpConnectFailure;
            }
            AtavismIncomingMessage inMessage = new AtavismIncomingMessage(tcpWorldConnection.GetStream());
            BaseWorldTcpMessage response = WorldTcpMessageFactory.ReadMessage(inMessage);
            AtavismLogger.LogInfoMessage("Got response: " + response);
            WorldServerLoginHeartbeatResponseMessage serverResponse = response as WorldServerLoginHeartbeatResponseMessage;
            if (serverResponse == null)
                throw new Exception("Invalid response to heartbeat message");

            AtavismLogger.LogInfoMessage("Read message from tcp world server");

           
            return NetworkHelperStatus.Success;
        }

        /// <summary>
        /// Return a time adjusted with respect to the WorldManager's
        /// current time
        /// </summary>
        public long AdjustTimestamp(long timestamp)
        {
            return AdjustTimestamp(timestamp, worldManager.CurrentTimeValue);
        }

        /// <summary>
        ///   Get the time in our local time system (instead of server time), 
        ///   and update the time offset if neccessary.
        /// </summary>
        /// <param name="timestamp">timestamp from the server</param>
        /// <param name="now">client's current time</param>
        /// <returns>timestamp in client time</returns>
        public long AdjustTimestamp(long timestamp, long now)
        {
            // Adjust this so that this is the adjusted time of the message
            long clientTimestamp = timeOffset + timestamp;
            // If the time is off by more than 30 seconds, update the offset.
            long newOffset = now - timestamp;
            if (Math.Abs(clientTimestamp - now) > 30 * 1000)
            {
                //log.InfoFormat("Increasing networkHelper.TimeOffset from {0} to {1}",
                //		       timeOffset, newOffset);
                timeOffset = newOffset;
                clientTimestamp = now;
            }
            else if (clientTimestamp > now)
            {
                //log.InfoFormat("Decreasing networkHelper.TimeOffset from {0} to {1}",
                //		       timeOffset, newOffset);
                timeOffset = newOffset;
                clientTimestamp = now;
            }

            return clientTimestamp;
        }

        /// <summary>
        ///   Convert the timestamp in client time to a timestamp in server time.
        /// </summary>
        /// <param name="clientTimestamp">timestamp in client time</param>
        /// <returns>timestamp in the server time</returns>
        public long GetServerTimestamp(long clientTimestamp)
        {
            return (long)(clientTimestamp /*- timeOffset*/);
        }

        private TcpClient Connect(string hostname, int port)
        {
            // This constructor arbitrarily assigns the local port number.
            TcpClient tcpClient = new TcpClient(AddressFamily.InterNetworkV6);
            try
            {
                IPAddress addr = AtavismNetworkHelper.GetIPv4Address(hostname);
                if (addr == null)
                {
                    AtavismLogger.LogWarning("No valid IPv4 address for " + hostname);
                    return null;
                }
               
                tcpClient.Connect(addr, port);
                AtavismLogger.LogInfoMessage("Connected to login server");
                try
                {
                    // Wait up to ten seconds for a read failure.
                    tcpClient.ReceiveTimeout = 30000;//ZBDS default 10000
                }
                catch (InvalidOperationException e)
                {
                    // I guess this doesn't support the receive timeout
                    AtavismLogger.LogWarning("Tcp connection does not support receive timeout");
                }
                loginPluginHost = hostname;
                return tcpClient;
            }
            catch (Exception e)
            {
                AtavismLogger.LogWarning("Failed to connect to login server at " + hostname + ":" + port + " - " + e);
                tcpClient.Close();
                return null;
            }
        }

        public WorldServerEntry GetWorldEntry(string worldId)
        {
            //Monitor.Enter(worldServerMap);
            try
            {
                return worldServerMap[worldId];
            }
            finally
            {
                //Monitor.Exit(worldServerMap);
            }
        }

        public void SetWorldEntry(string worldId, WorldServerEntry entry)
        {
            //Monitor.Enter(worldServerMap);
            try
            {
                worldServerMap[worldId] = entry;
            }
            finally
            {
                //Monitor.Exit(worldServerMap);
            }
        }

        public bool HasWorldEntry(string worldId)
        {
            Monitor.Enter(worldServerMap);
            try
            {
                return worldServerMap.ContainsKey(worldId);
            }
            finally
            {
                Monitor.Exit(worldServerMap);
            }
        }

        public Dictionary<string, WorldServerEntry> WorldServerMap
        {
            get
            {
                return worldServerMap;
            }
        }

        #region login messages
        /// <summary>
        ///   Login to the Master server using tcp
        /// </summary>
        /// <param name="loginSettings"></param>
        /// <returns></returns>
        public NetworkHelperStatus CreateAccountMaster(LoginSettings loginSettings)
        {
            //log_status.Info("Connecting to master tcp server");
            //ClientAPI.Write ("Connecting to master tcp server");
            return TcpMasterConnect(loginSettings);
        }

        /// <summary>
        ///   Login to the Master server using tcp
        /// </summary>
        /// <param name="loginSettings"></param>
        /// <returns></returns>
        public NetworkHelperStatus LoginMaster(LoginSettings loginSettings)
        {
            status = "Connecting to Authentication Server...";
            //log_status.Info("Connecting to master tcp server");
            //ClientAPI.Write ("Connecting to master tcp server");
            return TcpMasterConnect(loginSettings);
        }


        /// <summary>
        ///   Connect to the Master server using rdp, and resolve the world id
        /// </summary>
        /// <param name="loginSettings"></param>
        /// <returns></returns>
        public NetworkHelperStatus ResolveWorld(LoginSettings loginSettings)
        {
            this.status = "Resolving World Server...";
            NetworkHelperStatus status = NetworkHelperStatus.WorldResolveFailure;
            //log_status.Info("Connecting to master rdp server");
            AtavismLogger.LogInfoMessage("Connecting " + loginSettings+" "+ loginSettings.rdpServer+":"+ loginSettings.rdpPort);
            for (int attempts = 0; attempts < 2; ++attempts)
            {
                status = RdpMasterConnect(loginSettings.rdpServer, loginSettings.rdpPort);
                if (status == NetworkHelperStatus.Success)
                    break;
            }
            AtavismLogger.LogInfoMessage("Connecting ResolveWorld status:" + status);
            if (status != NetworkHelperStatus.Success)
                return status;
            return ResolveWorld(loginSettings.worldId, loginSettings.username);
        }

        protected NetworkHelperStatus ResolveWorld(string worldId, string userName)
        {
            NetworkHelperStatus status;
            // Close any existing world connections
            RdpWorldDisconnect();

            AtavismLogger.LogInfoMessage("Sending world resolve message with username: " + userName);
            status = RdpResolveWorld(worldId, userName);
            //log_status.InfoFormat("World resolve message status: {0}", status);
            if (status != NetworkHelperStatus.Success)
                return status;
            WorldServerEntry entry = GetWorldEntry(/*worldId*/AtavismClient.Instance.WorldId);
            //log_status.InfoFormat("WorldServerEntry: {0}", entry);
            return NetworkHelperStatus.WorldResolveSuccess;
        }

        /// <summary>
        ///   Connect to the login server, so we can do character 
        ///   selection there.
        /// </summary>
        /// <param name="worldId"></param>
        /// <param name="charEntries"></param>
        /// <returns></returns>
        public NetworkHelperStatus ConnectToLogin(string worldId, string clientVersion)
        {
            WorldServerEntry entry = GetWorldEntry(worldId);
            if (entry.Standalone)
                return NetworkHelperStatus.Standalone;

            // Connect to the world using tcp, and get the list of characters
            AtavismLogger.LogInfoMessage("Connecting to world login manager at " + entry.Hostname + ":" + entry.Port);
            return TcpWorldConnect(entry.Hostname, entry.Port, clientVersion);
        }

     

        public NetworkHelperStatus ConnectToWorld(OID characterId, string hostname, int port, string clientVersion)
        {
            NetworkHelperStatus status = NetworkHelperStatus.Unknown;

            // close my connection to the tcp world server
            TcpWorldDisconnect();

            for (int attempts = 0; attempts < 2; ++attempts)
            {
                AtavismLogger.LogInfoMessage("Connecting to world proxy at " + hostname + ":" + port);
                status = RdpWorldConnect(hostname, port);
                if (status == NetworkHelperStatus.Success)
                    break;
            }
            if (status != NetworkHelperStatus.Success)
                return status;

            AtavismLogger.LogInfoMessage("Logging into world server as " + characterId);
            status = RdpLogin(characterId, clientVersion);

            return status;
        }

        public void DisconnectFromMaster()
        {
            RdpMasterDisconnect();
            RdpWorldDisconnect();
        }

        public void DisconnectFromLogin()
        {
            TcpWorldDisconnect();
        }

        private void TcpWorldDisconnect()
        {
            // Close the connection to the tcp world server
            if (tcpWorldConnection != null)
            {
                //log_status.InfoFormat("Disconnecting from tcp world server");
                tcpWorldConnection.Close();
                tcpWorldConnection = null;
            }
            else
            {
                AtavismLogger.LogDebugMessage("tcpWorldConnection is null");
            }
        }
      
        public void Disconnect()
        {
            System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
            AtavismLogger.LogDebugMessage("Got Disconnect() from " + t.ToString());
            UnityEngine.Debug.LogWarning("Got Disconnect() from " + t.ToString());
            RdpWorldDisconnect();
            TcpWorldDisconnect();
            RdpMasterDisconnect();
           
        }
     /*   private void RdpConnectionDisconect()
        {
            if(rdp)
        }*/

        /// <summary>
        ///   Connect to the master server via tcp, send our username and 
        ///   password, and get an authentication token.  Finally, disconnect.
        /// </summary>
        /// <param name="loginSettings"></param>
        /// <param name="idToken"></param>
        /// <returns></returns>
		private NetworkHelperStatus TcpMasterConnect(LoginSettings loginSettings)
        {
            // clear my auth token
            authToken = null;
            //int maxAttempts = 1;
            //int attempts = 0;
            while (true)
            {
                // Use the world server address
                TcpClient tcpClient = Connect(loginSettings.tcpServer, loginSettings.tcpPort);
                //TcpClient tcpClient = Connect(GetWorldEntry(loginSettings.worldId).Hostname, loginSettings.tcpPort);
                if (tcpClient == null)
                    return NetworkHelperStatus.MasterTcpConnectFailure;
                MasterTcpLoginMessage loginMessage = new MasterTcpLoginMessage();
                loginMessage.MagicCookie = 0xffffffff;
                loginMessage.Version = 1;

                loginMessage.Username = AtavismEncryption.EncryptString(loginSettings.username);
               // UnityEngine.Debug.LogError("user=" + loginSettings.username);
               // UnityEngine.Debug.LogError("user=" + loginMessage.Username);
                //loginMessage.Username = loginSettings.username;
                loginMessage.Password = AtavismEncryption.EncryptString(loginSettings.password);
                //loginMessage.Password = loginSettings.password;
                loginMessage.CreateAccount = loginSettings.createAccount;
                if (loginSettings.createAccount)
                    loginMessage.EmailAddress = AtavismEncryption.EncryptString(loginSettings.emailAddress);
                loginMessage.Props = loginSettings.props;

                AtavismOutgoingMessage outMessage = loginMessage.CreateMessage();
                outMessage.Send(tcpClient);

                AtavismIncomingMessage inMessage = new AtavismIncomingMessage(tcpClient.GetStream());
                /*MasterTcpLoginChallengeMessage challengeMessage = new MasterTcpLoginChallengeMessage();
                challengeMessage.ParseMasterTcpMessage(inMessage);
                log_status.Info("AuthProtocolVersion: " + challengeMessage.AuthProtocolVersion + "ChallengeLength: " + challengeMessage.ChallengeLength
                    + "Challenge: " + challengeMessage.Challenge);
                MasterTcpLoginChallengeResponseMessage challengeResponseMessage = new MasterTcpLoginChallengeResponseMessage();
                challengeResponseMessage.Response = challengeMessage.Challenge;

                OutgoingMessage outMessage2 = challengeMessage.CreateMessage();
                outMessage2.Send(tcpClient);*/
                LoginStatus result = LoginStatus.ServerError;
                try
                {
                    if (loginSettings.createAccount)
                    {
                        MasterTcpCreateAccountResponseMessage responseMessage = new MasterTcpCreateAccountResponseMessage();
                        responseMessage.ParseMasterTcpMessage(inMessage);
                        result = responseMessage.LoginStatus;
                        AtavismLogger.LogInfoMessage("Create Account Result: " + result);
                    }
                    else
                    {
                        MasterTcpLoginResponseMessage responseMessage = new MasterTcpLoginResponseMessage();
                        responseMessage.ParseMasterTcpMessage(inMessage);
                        result = responseMessage.LoginStatus;
                        authToken = responseMessage.AuthToken;
                        AtavismLogger.LogInfoMessage("Auth token: " + authToken);
                    }
                }
                catch (EndOfStreamException e)
                {
                    //log.Info("Login Result: Got EndOfStreamException");
                    //attempts++;
                    //if (attempts < maxAttempts)
                    //    continue;
                }


                tcpClient.Close();
                //log_status.InfoFormat("Login Result: {0}, {1}", authToken, result);
                if (result == LoginStatus.Success)
                {
                    return NetworkHelperStatus.Success;
                }
                else if (result == LoginStatus.AlreadyLoggedIn)
                {
                    Logout(loginSettings);
                    continue;
                }
                else if (result == LoginStatus.ServerError)
                {
                    return NetworkHelperStatus.MasterTcpConnectFailure;
                }
                else if (result == LoginStatus.InvalidPassword)
                {
                    return NetworkHelperStatus.LoginFailure;
                }
                else if (result == LoginStatus.NoAccess)
                {
                    return NetworkHelperStatus.NoAccessFailure;
                }
                else if (result == LoginStatus.Banned)
                {
                    return NetworkHelperStatus.BannedFailure;
                }
                else if (result == LoginStatus.UsernameUsed)
                {
                    return NetworkHelperStatus.UsernameUsed;
                }
                else if (result == LoginStatus.EmailUsed)
                {
                    return NetworkHelperStatus.EmailUsed;
                }
                else if (result == LoginStatus.DatabaseError)
                {
                    return NetworkHelperStatus.Unknown;
                }
                else if (result == LoginStatus.SubscriptionExpired)
                {
                    return NetworkHelperStatus.SubscriptionExpiredFailure;
                }
                else if (result == LoginStatus.ServerMaintanance)
                {
                    return NetworkHelperStatus.ServerMaintanance;
                }
                else
                {
                    AtavismLogger.LogWarning("Invalid status: " + result);
                    return NetworkHelperStatus.MasterTcpConnectFailure;
                }
            }
        }

        /// <summary>
        ///   Connect to the world server and get a list of available characters.
        ///   This leaves the connection open, in case they want to create
        ///   new characters or get more information about existing characters.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <param name="idToken"></param>
        /// <returns></returns>
        private NetworkHelperStatus TcpWorldConnect(string hostname, int port, string clientVersion)
        {
            System.Diagnostics.Debug.Assert(tcpWorldConnection == null);
            AtavismLogger.LogInfoMessage("Connecting to tcp world server at " + hostname + ":" + port);
            tcpWorldConnection = Connect(hostname, port);
            if (tcpWorldConnection == null)
            {
                AtavismLogger.LogWarning("Unable to connect to tcp world server");
                return NetworkHelperStatus.WorldTcpConnectFailure;
            }
            AtavismLogger.LogInfoMessage("Connected to tcp world server");
            return GetCharacterEntries(clientVersion);
        }



        public void GetCharacters(string clientVersion)
        {
            if (tcpWorldConnection == null)
            {
                AtavismLogger.LogWarning("Not connected to tcp world server");
            }
            else
            {
                AtavismLogger.LogInfoMessage("GetCharacters");
                GetCharacterEntries(clientVersion);
            }
        }

        /// <summary>
        ///   Helper method to deal with the fact that some servers were not 
        ///   forward compatible.
        /// </summary>
        /// <param name="clientVersion"></param>
        /// <returns></returns>
        private NetworkHelperStatus GetCharacterEntries(string clientVersion)
        {
            AtavismLogger.LogInfoMessage("Creating get character entries message");
            WorldCharacterRequestMessage characterRequest = new WorldCharacterRequestMessage();
            characterRequest.Version = clientVersion;
            characterRequest.AuthToken = authToken;
            AtavismOutgoingMessage outMessage = characterRequest.CreateMessage();
            outMessage.Send(tcpWorldConnection);
            AtavismLogger.LogInfoMessage("Wrote message to tcp world server");

            // Read a message off of our tcp stream
            AtavismIncomingMessage inMessage = new AtavismIncomingMessage(tcpWorldConnection.GetStream());
            BaseWorldTcpMessage response = WorldTcpMessageFactory.ReadMessage(inMessage);
            WorldCharacterResponseMessage characterResponse = response as WorldCharacterResponseMessage;
            if (characterResponse == null)
                throw new Exception("Invalid response to world login");

            AtavismLogger.LogInfoMessage("Read message from tcp world server");

            // TODO: We need to be using world token (or auth token) when we 
            // talk to the world proxy server, since these are our only 
            // mechanisms for authentication.
            worldToken = characterResponse.WorldToken;
            worldFilesUrl = characterResponse.WorldFilesUrl;
            account = characterResponse.Account;

            // Store the list of character entries, so it can be accessed later
            characterEntries = new List<CharacterEntry>(characterResponse.CharacterEntries);
            numCharacterSlots = characterResponse.CharacterSlots;

            NetworkHelperStatus rv;
            // TODO: This is ugly.  I want to be able to grab character info 
            if (characterResponse.Error == string.Empty)
            {
                rv = NetworkHelperStatus.Success;
            }
            else if (characterResponse.Error == "Unsupported client version")
            {
                // custom logic to handle this case
                rv = NetworkHelperStatus.UnsupportedClientVersion;
            }
            else
            {
                // An unknown error
                //log.Warn("Bad login status response: " + characterResponse.Error);
                throw new Exception(characterResponse.Error);
            }
            return rv;
        }

        /// <summary>
        ///   Helper method to deal with the fact that some servers were not 
        ///   forward compatible.
        /// </summary>
        /// <param name="clientVersion"></param>
        /// <returns></returns>
        public NetworkHelperStatus GetProxyToken(OID characterId)
        {
            if (!tcpWorldConnection.Connected)
            {
                return NetworkHelperStatus.MasterTcpConnectFailure;
            }
            WorldCharacterSelectRequestMessage selectRequest = new WorldCharacterSelectRequestMessage();
            selectRequest.Properties.Add("characterId", characterId);
            AtavismOutgoingMessage outMessage = selectRequest.CreateMessage();
            outMessage.Send(tcpWorldConnection);
            //AtavismLogger.LogInfoMessage("Wrote message to tcp world server");

            // Read a message off of our tcp stream
            AtavismIncomingMessage inMessage = new AtavismIncomingMessage(tcpWorldConnection.GetStream());
            BaseWorldTcpMessage response = WorldTcpMessageFactory.ReadMessage(inMessage);

            WorldCharacterSelectResponseMessage selectResponse = response as WorldCharacterSelectResponseMessage;
            if (selectResponse == null)
            {
                //throw new Exception("Invalid response to world login");
                return NetworkHelperStatus.MasterTcpConnectFailure;
            }
            NetworkHelperStatus rv;
            if (selectResponse.Properties.ContainsKey("errorMessage"))
            {
                string[] args = new string[1];
                args[0] = (string)selectResponse.Properties["errorMessage"];
                AtavismEventSystem.DispatchEvent("World_Error", args);
                rv = NetworkHelperStatus.WorldEnterFailure;
            }
            else
            {
                //log.InfoFormat("Read select response message from tcp world server");

                worldToken = (byte[])selectResponse.Properties["token"];
                proxyPluginHost = (string)selectResponse.Properties["proxyHostname"];
                proxyPluginPort = (int)selectResponse.Properties["proxyPort"];

               
                // TODO: This is ugly.
                if (!selectResponse.Properties.ContainsKey("errorMsg") ||
                    (string)selectResponse.Properties["errorMsg"] == string.Empty)
                {
                    rv = NetworkHelperStatus.Success;
                }
                else if ((string)selectResponse.Properties["errorMsg"] == "Unsupported client version")
                {
                    // custom logic to handle this case
                    rv = NetworkHelperStatus.UnsupportedClientVersion;
                }
                else
                {
                    // An unknown error
                    //log.Warn("Bad character select response response: " + selectResponse.Properties["errorMsg"]);
                    throw new Exception((string)selectResponse.Properties["errorMsg"]);
                }
            }
            return rv;
        }

        private NetworkHelperStatus RdpResolveWorld(string worldName, string userName)
        {
            ResolveMessage resolve = new ResolveMessage();
            resolve.WorldName = worldName;
            resolve.UserName = userName;
            rdpMaster.SendMessage(resolve.CreateMessage());
            int sleepCount = 0;
            while (sleepCount < 10)
            {
                Monitor.Enter(worldServerMap);
                try
                {
                    if (worldServerMap.ContainsKey(/*worldName*/ AtavismClient.Instance.WorldId))
                    {
                        //log_status.InfoFormat("Found entry for world '{0}'", worldName);
                        return NetworkHelperStatus.Success;
                    }
                }
                finally
                {
                    Monitor.Exit(worldServerMap);
                }
                Thread.Sleep(100);
                sleepCount++;
            }
            //log.ErrorFormat("Unable to resolve world: {0}", worldName);
            return NetworkHelperStatus.WorldResolveFailure;
        }

        /// <summary>
        ///   This is basically a noop now - In the future, I should
        ///   send a message on the RDP channel to the master server.
        /// </summary>
        /// <param name="loginSettings"></param>
        /// <returns></returns>
        private NetworkHelperStatus Logout(LoginSettings loginSettings)
        {
            return NetworkHelperStatus.MasterTcpConnectFailure;
        }

        /// <summary>
        ///   Look up the hostname in DNS and return the first IPv4 address
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        private static IPAddress GetIPv4Address(string hostname)
        {
            IPAddress addr = null;
            if (IPAddress.TryParse(hostname, out addr))
            {
                //log.InfoFormat("IP Address for {0}: '{1}'", hostname, addr);
                return addr;
            }

            IPHostEntry IPHost = Dns.GetHostEntry(hostname);
            IPAddress[] addrs = IPHost.AddressList;
            foreach (IPAddress entry in addrs)
            {
                if (entry.AddressFamily == AddressFamily.InterNetwork)
                {
                    addr = entry;
                    break;
                }
            }
            //log.InfoFormat("IP Address for {0}: '{1}'", hostname, addr);
            return addr;
        }

        private NetworkHelperStatus RdpMasterConnect(string hostname, int port)
        {
            try
            {
                //log_status.InfoFormat("Connecting to rdp world server at {0}:{1}", hostname, port);
                //ClientAPI.Write("Connecting to rdp world server at " + hostname + ":" + port);
                IPAddress addr = AtavismNetworkHelper.GetIPv4Address(hostname);
                if (addr == null)
                {
                    //log.WarnFormat("No valid IPv4 address for {0}", hostname);
                    return NetworkHelperStatus.MasterConnectFailure;
                }
                AtavismLogger.LogInfoMessage("connecting to rdp master server: " + addr+":"+port);

                IPEndPoint endpoint = new IPEndPoint(addr, port);
                rdpMaster = new RdpMasterMessageHandler(endpoint, worldServerMap);
                rdpMaster.BeginListen();
            }
            catch (Exception e)
            {
                AtavismLogger.LogError("Exception connecting to rdp master server: " + e);
                RdpMasterDisconnect();
                //log.ErrorFormat("Failed to connect to rdp master server at {0}:{1}", hostname, port);
                return NetworkHelperStatus.MasterConnectFailure;
            }
            return NetworkHelperStatus.Success;
        }

        private void RdpMasterDisconnect()
        {
            if (rdpMaster != null)
            {
                //log_status.Info("Disconnecting from rdp master server");
                rdpMaster.Dispose();
                rdpMaster = null;
            }
            else
            {
                AtavismLogger.LogDebugMessage("rdpMaster is null");
            }
        }

        private void RdpWorldDisconnect()
        {
            if (messageHandlerWorld != null)
            {
                //log_status.Info("Disconnecting from rdp world server");
                messageHandlerWorld.Dispose();
                messageHandlerWorld = null;
            }
            else
            {
                AtavismLogger.LogDebugMessage("messageHandlerWorld is null");
            }
        }

        private NetworkHelperStatus RdpWorldConnect(string hostname, int port)
        {
            try
            {
                AtavismLogger.LogInfoMessage("Connecting to rdp world server at " + hostname + ":" + port);
                IPAddress addr = AtavismNetworkHelper.GetIPv4Address(hostname);
                if (addr == null)
                {
                    AtavismLogger.LogWarning("No valid IPv4 address for " + hostname);
                    return NetworkHelperStatus.WorldConnectFailure;
                }
                IPEndPoint endpoint = new IPEndPoint(addr, port);
                MessageDispatcher dispatcher = new DefragmentingMessageDispatcher();
                if (this.UseTCP)
                    messageHandlerWorld = new TcpWorldMessageHandler(endpoint, dispatcher);
                else
                    messageHandlerWorld = new RdpWorldMessageHandler(endpoint, dispatcher);
                messageHandlerWorld.BeginListen();
            }
            catch (Exception e)
            {
                AtavismLogger.LogWarning("Exception connecting to rdp world server " + hostname + ":" + port + ":" + e);
                RdpWorldDisconnect();
                AtavismLogger.LogError("Failed to connect to rdp world server at " + hostname + ":" + port);
                return NetworkHelperStatus.WorldConnectFailure;
            }
            return NetworkHelperStatus.Success;
        }

        private NetworkHelperStatus RdpLogin(OID characterId, string clientVersion)
        {
            AuthorizedLoginMessage message = new AuthorizedLoginMessage();
            message.CharacterId = characterId;
            message.ClientVersion = clientVersion;
            message.WorldToken = worldToken;

            SendMessage(message);
            return NetworkHelperStatus.Success;
        }

        #endregion

        public AtavismNetworkHelper(IWorldManager worldManager)
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                AtavismLogger.LogError("A Network helper already exists");
                //log.ErrorFormat("NetworkHelper constructor: NetworkHelper is a singleton, but the instance already exists");
            }
            //this.client = client;
            this.worldManager = worldManager;
            worldManager.NetworkHelper = this;
            worldServerMap = new Dictionary<string, WorldServerEntry>();
            characterEntries = new List<CharacterEntry>();

           
        }

        private NetworkHelperStatus TcpPrefabConnect(string hostname, int port)
        {
            System.Diagnostics.Debug.Assert(tcpPrefabConnection == null);
            AtavismLogger.LogInfoMessage("Connecting to tcp world server at " + hostname + ":" + port);
            tcpPrefabConnection = Connect(hostname, port);
            if (tcpPrefabConnection == null)
            {
                AtavismLogger.LogWarning("Unable to connect to tcp world server");
                return NetworkHelperStatus.WorldTcpConnectFailure;
            }
            AtavismLogger.LogInfoMessage("Connected to tcp world server");
            return NetworkHelperStatus.Success;
                //GetCharacterEntries(clientVersion);
        }

        public void TcpPrefabDisconnect()
        {
            // Close the connection to the tcp world server
            if (tcpPrefabConnection != null)
            {
                AtavismLogger.LogDebugMessage("Close connection to Prefab server");
                tcpPrefabConnection.Close();
                tcpPrefabConnection = null;
            }
            else
            {
                AtavismLogger.LogDebugMessage("tcpWorldConnection is null");
            }
            prefabrun = false;
            foreach (Thread t in clientReceiveThreads)
            {
                t.Abort();
            }
        }

        //   Dictionary<string, List<Dictionary<string, object>>> prefabQueue = new Dictionary<string, List<Dictionary<string, object>>>();
        Queue<AtavismOutgoingMessage> prefabMessagesQueue = new Queue<AtavismOutgoingMessage>();
        Queue<BaseWorldTcpMessage> prefabMessagesResponseQueue = new Queue<BaseWorldTcpMessage>();
        long prefabMessagesQueued = 0;
        public virtual void QueueMessage(AtavismOutgoingMessage message)
        {
            //  AtavismLogger.LogError("QueueMessage message: " + message+" "+ messagesQueued);

            Monitor.Enter(prefabMessagesQueue);
            try
            {
                prefabMessagesQueue.Enqueue(message);
                prefabMessagesQueued++;
            }
            finally
            {
                Monitor.Exit(prefabMessagesQueue);
            }
        }


        public void StartClientPrefab(string prefabServer, int prefabServerPort)
        {
          //  AtavismLogger.LogError("Connect to Prefab server");
            //  if (prefabrun)
            //    return;
            this.preSrv = prefabServer;
            this.preSrvP = prefabServerPort;
            Thread t = new Thread(new ThreadStart(SendPrefabMessage));
            t.IsBackground = true;
            t.Name = "PrefabManager";
            t.Start();
            clientReceiveThreads.Add(t);
        }

        private List<Thread> clientReceiveThreads = new List<Thread>();
        bool prefabrun = true;
        private TcpClient socketConnection;
        private Thread clientReceiveThread;
        protected string preSrv = "";
        protected int preSrvP = 5555;
       
        private void ListenForData()
        {
            try
            {
                socketConnection = Connect(preSrv, preSrvP);
                //new TcpClient("10.0.0.98", 8052); //172.30.40.19
              //  Byte[] bytes = new Byte[1024];
                while (true)
                {
                    // Get a stream object for reading                
                    using (NetworkStream stream = socketConnection.GetStream())
                    {
                     //   AtavismLogger.LogError("Listen For Prefab Data got message");
                        AtavismIncomingMessage inMessage = new AtavismIncomingMessage(stream);
                        BaseWorldTcpMessage response = PrefabTcpMessageFactory.ReadMessage(inMessage);
                        Monitor.Enter(prefabMessagesResponseQueue);
                        try
                        {
                            prefabMessagesResponseQueue.Enqueue(response);
                        }
                        finally
                        {
                            Monitor.Exit(prefabMessagesResponseQueue);
                        }
                    }
                }
            }
            catch (SocketException socketException)
            {
                AtavismLogger.LogError("Socket exception: " + socketException);
            }
        }

        public void SendPrefabMsg()
        {
            if (socketConnection == null)
            {
                return;
            }

            if (prefabMessagesQueue.Count == 0)
                return;
            try
            {
               // AtavismLogger.LogError("About Wrote message to prefab server Queue count= "+ prefabMessagesQueue.Count);
                // Get a stream object for writing. 
                if (socketConnection.Connected)
                {
                   // socketConnection.
                    NetworkStream stream = socketConnection.GetStream();
                    if (stream.CanWrite)
                    {
                        AtavismOutgoingMessage msg;
                        Monitor.Enter(prefabMessagesQueue);
                        try
                        {

                            msg = prefabMessagesQueue.Dequeue();

                        }
                        finally
                        {
                            Monitor.Exit(prefabMessagesQueue);
                        }
                        AtavismLogger.LogError("About Wrote message to prefab server ");

                        msg.Send(socketConnection);
                    }
                }
            }
            catch (SocketException socketException)
            {
                AtavismLogger.LogError("Socket exception: " + socketException);
            }
        }


        private void SendPrefabMessage()
        {
        
            while (prefabrun)
            {
                if (prefabMessagesQueue.Count > 0)
                {
                    if (tcpPrefabConnection == null || !tcpPrefabConnection.Connected )
                    {
                        NetworkHelperStatus res = TcpPrefabConnect(preSrv, preSrvP);
                        if (!res.Equals(NetworkHelperStatus.Success))
                        {
                            AtavismLogger.LogWarning("Cant Connect to Prefab Server");
                        }
                    }
                    else
                    {
                     //   UnityEngine.Debug.LogError("Get message from Queue " + prefabMessagesQueue.Count + " | " + Thread.CurrentThread.Name);

                        AtavismOutgoingMessage msg;
                        BaseWorldTcpMessage response=null;
                        Monitor.Enter(prefabMessagesQueue);
                        try
                        {
                            msg = prefabMessagesQueue.Peek();

                        }
                        finally
                        {
                            Monitor.Exit(prefabMessagesQueue);
                        }
                        //    AtavismLogger.LogError("About Wrote message to prefab server ");

                        try
                        {
                            msg.Send(tcpPrefabConnection);

                            AtavismLogger.LogInfoMessage("Wrote message to prefab server");
                            //   AtavismLogger.LogError("Wrote message to prefab server");

                            AtavismIncomingMessage inMessage = new AtavismIncomingMessage(tcpPrefabConnection.GetStream());
                            response = PrefabTcpMessageFactory.ReadMessage(inMessage);

                            AtavismLogger.LogInfoMessage("Read message from prefab server");
                            //   AtavismLogger.LogError("Read message from prefab server ");
                        }
                        catch (Exception e)
                        {
                            UnityEngine.Debug.LogError("Read message from prefab server got Exception " + e + " " + (e != null ? e.Message : ""));
                            UnityEngine.Debug.LogError("Read message from prefab server got Connected ? " + (tcpPrefabConnection != null ? tcpPrefabConnection.Connected.ToString() : "bd"));
                            tcpPrefabConnection.Close();
                        }

                        if (response != null)
                        {
                            Monitor.Enter(prefabMessagesQueue);
                            try
                            {
                                 prefabMessagesQueue.Dequeue();
                            }
                            finally
                            {
                                Monitor.Exit(prefabMessagesQueue);
                            }
                            Monitor.Enter(prefabMessagesResponseQueue);
                            try
                            {
                                prefabMessagesResponseQueue.Enqueue(response);
                            }
                            finally
                            {
                                Monitor.Exit(prefabMessagesResponseQueue);
                            }
                        }
                      
                    }
                }
                else
                {
                    Thread.Sleep(500);
                }
                
            }


        }

        public void handlePrefabMessages()
        {
            BaseWorldTcpMessage msg;
            Monitor.Enter(prefabMessagesResponseQueue);
            try
            {
                if (prefabMessagesResponseQueue.Count == 0)
                    return;
                msg = prefabMessagesResponseQueue.Dequeue();
            }
            finally
            {
                Monitor.Exit(prefabMessagesResponseQueue);
            }
            if (msg != null)
            {
                if (msg is PrefabResponseMessage)
                {
                    PrefabResponseMessage msgResponse = msg as PrefabResponseMessage;
                    handlePrefabMessage(msgResponse.TypeMsg, msgResponse.Properties);

                }
                else if (msg is IconPrefabResponseMessage)
                {
                    IconPrefabResponseMessage msgResponse = msg as IconPrefabResponseMessage;
                    handlePrefabMessage(msgResponse.TypeMsg, msgResponse.Properties);
                }
            }
        }

        private void handlePrefabMessage(string msgtype, Dictionary<string, object> properties)
        {
           // AtavismLogger.LogError("handlePrefabMessage "+ msgtype);
            try
            {
                if (prefabHandlers.ContainsKey(msgtype))
                {
                    List<ExtensionMessageHandler> handlers = prefabHandlers[msgtype];
                    foreach (ExtensionMessageHandler handler in handlers)
                        handler(properties);
                }
                else
                {
                    AtavismLogger.LogDebugMessage("handlePrefabMessage no handle for "+msgtype);
                }
            }
            catch (Exception e)
            {
                AtavismLogger.LogError("Exception " + e);

            }
        }


        public NetworkHelperStatus GetIconPrefabs(Dictionary<string, object> properties, string TypeMsg)
        {

            IconPrefabRequestMessage Request = new IconPrefabRequestMessage();
            Request.Properties = properties;
            Request.TypeMsg = TypeMsg;
            // Request.AuthToken = authToken;
            AtavismOutgoingMessage outMessage = Request.CreateMessage();
            Monitor.Enter(prefabMessagesQueue);
            try
            {
                prefabMessagesQueue.Enqueue(outMessage);
            }
            finally
            {
                Monitor.Exit(prefabMessagesQueue);
            }
            return NetworkHelperStatus.Success;
        }

        public NetworkHelperStatus GetPrefabs(Dictionary<string, object> properties,string TypeMsg)
        {
            PrefabRequestMessage Request = new PrefabRequestMessage();
            Request.Properties = properties;
            Request.TypeMsg = TypeMsg;
            // Request.AuthToken = authToken;
            AtavismOutgoingMessage outMessage = Request.CreateMessage();
            Monitor.Enter(prefabMessagesQueue);
            try
            {
                prefabMessagesQueue.Enqueue(outMessage);
            }
            finally
            {
                Monitor.Exit(prefabMessagesQueue);
            }
            return NetworkHelperStatus.Success;
        }

        Dictionary<string, List<ExtensionMessageHandler>> prefabHandlers = new Dictionary<string, List<ExtensionMessageHandler>>();
        public void RegisterPrefabMessageHandler(string extensionType, ExtensionMessageHandler handler)
        {
            List<ExtensionMessageHandler> handlers;
            if (prefabHandlers.ContainsKey(extensionType))
                handlers = prefabHandlers[extensionType];
            else
                handlers = new List<ExtensionMessageHandler>();
            handlers.Add(handler);
            prefabHandlers[extensionType] = handlers;
            if (AtavismLogger.logLevel <= LogLevel.Debug)
            {
                string ss = "";
                foreach (string s in prefabHandlers.Keys)
                {
                    ss += s + "; ";
                }
                AtavismLogger.LogError("RegisterPrefabMessageHandler prefabHandlers keys = " + ss);
            }
        }

        public void RemovePrefabMessageHandler(string extensionType, ExtensionMessageHandler handler)
        {
        //    AtavismLogger.LogError("RemovePrefabMessageHandler prefabHandlers extensionType = " + extensionType);
            if (prefabHandlers.ContainsKey(extensionType))
            {
                List<ExtensionMessageHandler> handlers = prefabHandlers[extensionType];
                handlers.Remove(handler);
                if (handlers.Count == 0)
                    prefabHandlers.Remove(extensionType);
            }
            if (AtavismLogger.logLevel <= LogLevel.Debug)
            {
                string ss = "";
                foreach (string s in prefabHandlers.Keys)
                {
                    ss += s + "; ";
                }
                AtavismLogger.LogError("RemovePrefabMessageHandler prefabHandlers keys = " + ss);
            }
        }

        public void Init()
        {
        }

        public void Dispose()
        {
            Disconnect();
        }

        public static AtavismNetworkHelper Instance
        {
            get
            {
                return instance;
            }
        }

       

        public long TimeOffset
        {
            get
            {
                return timeOffset;
            }
            set
            {
                timeOffset = value;
            }
        }

        public long PacketsSentCounter
        {
            get
            {
                if (messageHandlerWorld == null)
                    return 0;
                return messageHandlerWorld.PacketsSentCounter;
            }
        }
        public long PacketsReceivedCounter
        {
            get
            {
                if (messageHandlerWorld == null)
                    return 0;
                return messageHandlerWorld.PacketsReceivedCounter;
            }
        }
        public long BytesSentCounter
        {
            get
            {
                if (messageHandlerWorld == null)
                    return 0;
                return messageHandlerWorld.BytesSentCounter;
            }
        }
        public long BytesReceivedCounter
        {
            get
            {
                if (messageHandlerWorld == null)
                    return 0;
                return messageHandlerWorld.BytesReceivedCounter;
            }
        }
        public byte[] AuthToken
        {
            get
            {
                return authToken;
            }
            set
            {
                authToken = value;
            }
        }
        public List<CharacterEntry> CharacterEntries
        {
            get
            {
                return characterEntries;
            }
        }
        public int NumCharacterSlots
        {
            get
            {
                return numCharacterSlots;
            }
        }
        public string WorldFilesUrl
        {
            get
            {
                return worldFilesUrl;
            }
        }
        public long Account
        {
            get
            {
                return account;
            }
        }
        public bool UseTCP
        {
            get
            {
                return useTCP;
            }
            set
            {
                useTCP = value;
            }
        }
        public string LoginPluginHost
        {
            get
            {
                return loginPluginHost;
            }
        }
        public string ProxyPluginHost
        {
            get
            {
                return proxyPluginHost;
            }
        }
        public int ProxyPluginPort
        {
            get
            {
                return proxyPluginPort;
            }
        }

        public static bool LogMessageContents
        {
            get
            {
                return logMessageContents;
            }
            set
            {
                logMessageContents = value;
            }
        }

        public string Status
        {
            get
            {
                return status;
            }
        }
    }
}