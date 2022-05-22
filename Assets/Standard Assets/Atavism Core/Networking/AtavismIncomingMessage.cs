using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
namespace Atavism
{

    public class AtavismIncomingMessage
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private BinaryReader reader;
        private IPEndPoint remoteIpEndPoint;

        private void Init(byte[] buf, IPEndPoint remoteEP)
        {
            Init(buf, 0, buf.Length, remoteEP);
        }
        private void Init(byte[] buf, int offset, int len, IPEndPoint remoteEP)
        {
            remoteIpEndPoint = remoteEP;
            MemoryStream memStream = new MemoryStream();
            memStream.Write(buf, offset, len);
            memStream.Flush();
            memStream.Seek(0, System.IO.SeekOrigin.Begin);
            reader = new BinaryReader(memStream);
        }

        public AtavismIncomingMessage(byte[] buf, IPEndPoint remoteEP)
        {
            Init(buf, remoteEP);
        }

        public AtavismIncomingMessage(byte[] buf, AtavismIncomingMessage source)
        {
            Init(buf, source.remoteIpEndPoint);
        }

        public AtavismIncomingMessage(byte[] buf, int offset, int len, IPEndPoint remoteEP)
        {
            Init(buf, offset, len, remoteEP);
        }

        public AtavismIncomingMessage(Stream readStream)
        {
            reader = new BinaryReader(readStream);
        }

        public WorldMessageType ReadMessageType()
        {
            return (WorldMessageType)ReadInt32();
        }

        public MasterMessageType ReadMasterMessageType()
        {
            return (MasterMessageType)ReadInt32();
        }

        public WorldTcpMessageType ReadWorldTcpMessageType()
        {
            return (WorldTcpMessageType)ReadInt32();
        }

        public long ReadTimestamp()
        {
            long timestamp = ReadInt64();
            long updatedTimestamp = AtavismNetworkHelper.Instance.AdjustTimestamp(timestamp);
            return updatedTimestamp;
        }

        public long ReadInt64()
        {
            return IPAddress.NetworkToHostOrder(reader.ReadInt64());
        }

        public int ReadInt32()
        {
            return IPAddress.NetworkToHostOrder(reader.ReadInt32());
        }

        public uint ReadUInt32()
        {
            return (uint)IPAddress.NetworkToHostOrder(reader.ReadInt32());
        }

        public short ReadInt16()
        {
            return IPAddress.NetworkToHostOrder(reader.ReadInt16());
        }

        public ushort ReadUInt16()
        {
            return (ushort)IPAddress.NetworkToHostOrder(reader.ReadInt16());
        }

        public float ReadSingle()
        {
            int val = ReadInt32();
            byte[] data = BitConverter.GetBytes(val);
            return BitConverter.ToSingle(data, 0);
        }

        public double ReadDouble()
        {
            long val = ReadInt64();
            byte[] data = BitConverter.GetBytes(val);
            return BitConverter.ToDouble(data, 0);
        }

        public bool ReadBool()
        {
            return ReadInt32() == 1;
        }

        public byte ReadByte()
        {
            return reader.ReadByte();
        }

        public bool ReadUShort()
        {
            return ReadInt32() == 1;
        }

        public Vector3 ReadVector()
        {
            float x = ReadSingle();
            float y = ReadSingle();
            float z = ReadSingle();
            return new Vector3(x, y, z);
        }

        public IntVector3 ReadIntVector()
        {
            int x = ReadInt32();
            int y = ReadInt32();
            int z = ReadInt32();
            return new IntVector3(x, y, z);
        }

        public Quaternion ReadQuaternion()
        {
            float x = ReadSingle();
            float y = ReadSingle();
            float z = ReadSingle();
            float w = ReadSingle();
            return new Quaternion(x, y, z, w);
        }

        public string ReadString()
        {
            return Encoding.UTF8.GetString(ReadBytes());
        }

        public byte[] ReadBytes()
        {
            int len = ReadInt32();
            return reader.ReadBytes(len);
        }

        public UnityEngine.Color ReadColor()
        {
            Color color = new Color();
            color.a = (float)reader.ReadByte() / 255;
            color.b = (float)reader.ReadByte() / 255;
            color.g = (float)reader.ReadByte() / 255;
            color.r = (float)reader.ReadByte() / 255;
            return color;
        }

        public OID ReadOID()
        {
            long oid = ReadInt64();
            return OID.fromLong(oid);
        }
    }
}