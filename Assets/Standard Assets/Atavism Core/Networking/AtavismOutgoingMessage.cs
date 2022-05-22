using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Diagnostics;
namespace Atavism
{

    public class AtavismOutgoingMessage
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        // Create a logger for use in this class

        private BinaryWriter writer;
        private MemoryStream memStream;

        public AtavismOutgoingMessage()
        {
            memStream = new MemoryStream();
            writer = new BinaryWriter(memStream);
        }

        public void Write(long value)
        {
            writer.Write(IPAddress.HostToNetworkOrder(value));
        }

        public void Write(int value)
        {
            writer.Write(IPAddress.HostToNetworkOrder(value));
        }

        public void Write(uint value)
        {
            writer.Write(IPAddress.HostToNetworkOrder((int)value));
        }

        public void Write(short value)
        {
            writer.Write(IPAddress.HostToNetworkOrder(value));
        }

        public void Write(ushort value)
        {
            writer.Write(IPAddress.HostToNetworkOrder((short)value));
        }

        public void Write(string value)
        {
            Write(Encoding.UTF8.GetBytes(value));
        }

        public void Write(byte value)
        {
            writer.Write(value);
        }

        public void Write(byte[] value)
        {
            Write(value.Length);
            writer.Write(value);
        }

        public void Write(MasterMessageType value)
        {
            Write((int)value);
        }

        public void Write(WorldMessageType value)
        {
            Write((int)value);
        }

        public void Write(WorldTcpMessageType value)
        {
            Write((int)value);
        }

        public void Write(bool value)
        {
            if (value)
                Write(1);
            else
                Write(0);
        }

        public void Write(float value)
        {
            byte[] data = BitConverter.GetBytes(value);
            int val = BitConverter.ToInt32(data, 0);
            Write(val);
        }

        public void Write(double value)
        {
            byte[] data = BitConverter.GetBytes(value);
            long val = BitConverter.ToInt64(data, 0);
            Write(val);
        }

        public void Write(Vector3 value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
        }

        public void Write(IntVector3 value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
        }

        public void Write(Quaternion value)
        {
            Write(value.x);
            Write(value.y);
            Write(value.z);
            Write(value.w);
        }

        public void Write(Color color)
        {
            writer.Write((byte)(color.a * 255));
            writer.Write((byte)(color.b * 255));
            writer.Write((byte)(color.g * 255));
            writer.Write((byte)(color.r * 255));
        }

        public void Write(OID oid)
        {
            Write(oid.ToLong());
            //writer.Write(oid.toLong());
        }

        public void Send(UdpClient udpClient)
        {
            try
            {
                writer.Flush();
                udpClient.Send(memStream.ToArray(), (int)memStream.Length);

            }
            catch (Exception)
            {
                AtavismClient.Instance.Disconnected();
            }
        }

        public void Send(AtavismRdpConnection rdpConn)
        {
            writer.Flush();
            try
            {
                rdpConn.Send(memStream.ToArray());
            }
            catch (RdpFragmentationException e)
            {
                UnityEngine.Debug.LogError("RdpFragmentationException "+e);
                AtavismClient.Instance.Disconnected();
            }

        }

        public void Send(TcpClient tcpClient)
        {
            try
            {
                writer.Flush();
                tcpClient.GetStream().Write(memStream.ToArray(), 0, (int)memStream.Length);
            }
            catch (Exception)
            {
                AtavismClient.Instance.Disconnected();
            }   
            
        }

        public byte[] GetBytes()
        {
            writer.Flush();
            byte[] rv = memStream.ToArray();
            System.Diagnostics.Debug.Assert((int)memStream.Length == rv.Length);
            return rv;
        }
    }
}