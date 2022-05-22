using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using Debug = UnityEngine.Debug;

namespace Atavism
{

    public class MessageTypes
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public class OID
    {
        private long data;

        public OID()
        {
            data = 0;
        }

        public static OID fromLong(long l)
        {
            if (l == 0)
            {
                return null;
            }
            OID rv = new OID();
            rv.data = l;
            return rv;
        }

        public long ToLong()
        {
            return data;
        }

        public override string ToString()
        {
            // Left pad the field with '0'
            StringBuilder sb = new StringBuilder();
            string hexString = data.ToString("x");
            for (int i = hexString.Length; i < 16; ++i)
            {
                sb.Append('0');
            }
            sb.Append(hexString);
            return sb.ToString();
        }

        public static OID fromString(string oidString)
        {
            long oidLong = Convert.ToInt64(oidString, 16);
            return OID.fromLong(oidLong);
        }

        public int hashCode()
        {
            return (int)data;
        }

        public int compareTo(OID other)
        {
            if (data < other.data)
            {
                return -1;
            }
            else if (data > other.data)
            {
                return 1;
            }
            return 0;
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to OID
            OID oid = obj as OID;
            if ((System.Object)oid == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (data == oid.data);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static OID parseLong(string str)
        {
            return OID.fromLong(long.Parse(str));
        }

        #region Overloaded operators + CLS compliant method equivalents

        /// <summary>
        ///		User to compare two OID instances for equality.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>true or false</returns>
        public static bool operator ==(OID left, OID right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
            {
                return false;
            }
            return (left.data == right.data);
        }
        /// <summary>
        ///		User to compare two OID instances for inequality.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>true or false</returns>
        public static bool operator !=(OID left, OID right)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(left, right))
            {
                return false;
            }

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
            {
                return true;
            }
            return (left.data != right.data);
        }


        #endregion Overloaded operators + CLS compliant method equivalents
    }

    public class SubmeshInfo
    {
        public string submeshName;
        public string materialName;
        public bool castShadows = true;
        public bool receiveShadows = true;
        public bool show = true;
        public bool isCollidable = true;

        public SubmeshInfo()
        {
        }

        public SubmeshInfo(string submeshName, string materialName)
        {
            this.submeshName = submeshName;
            this.materialName = materialName;
        }

        public string SubmeshName
        {
            get
            {
                return submeshName;
            }
            set
            {
                submeshName = value;
            }
        }
        public string MaterialName
        {
            get
            {
                return materialName;
            }
            set
            {
                materialName = value;
            }
        }
        public bool CastShadows
        {
            get
            {
                return castShadows;
            }
            set
            {
                castShadows = value;
            }
        }
        public bool ReceiveShadows
        {
            get
            {
                return receiveShadows;
            }
            set
            {
                receiveShadows = value;
            }
        }
        public bool Show
        {
            get
            {
                return show;
            }
            set
            {
                show = value;
            }
        }
        public bool IsCollidable
        {
            get
            {
                return isCollidable;
            }
            set
            {
                isCollidable = value;
            }
        }
    }

    // Provides I/O of a hierarchy of common types and list, set and
    // property map containers
    public class EncodedObjectIO
    {

        public enum ValueType
        {
            Null = 0,
            String = 1,
            Long = 2,
            Integer = 3,
            Boolean = 4,
            BooleanFalse = 4,
            BooleanTrue = 5,
            Float = 6,
            Double = 7,
            Point = 8,
            MVVector = 9,
            Quaternion = 10,
            Color = 11,
            Byte = 12,
            Short = 13,
            OID = 14,

            LinkedList = 20,
            List = 21,
            HashSet = 22,
            HashMap = 23,
            ByteArray = 24,
            TreeMap = 25,
        }

        private AtavismIncomingMessage inMessage;
        private AtavismOutgoingMessage outMessage;
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(EncodedObjectIO));
        private static Dictionary<Type, ValueType> classToValueTypeMap = null;

        public EncodedObjectIO(AtavismIncomingMessage inMessage)
        {
            this.inMessage = inMessage;
            if (classToValueTypeMap == null)
                initializeClassToValueTypeMap();
        }

        public EncodedObjectIO(AtavismOutgoingMessage outMessage)
        {
            this.outMessage = outMessage;
            if (classToValueTypeMap == null)
                initializeClassToValueTypeMap();
        }

        static EncodedObjectIO()
        {
            initializeClassToValueTypeMap();
        }

        private static void initializeClassToValueTypeMap()
        {
            string v0 = "";
            long v1 = 3L;
            int v2 = 3;
            bool v3 = true;
            float v4 = 3.0f;
            double v5 = 3.0;
            byte v6 = 1;
            short v7 = 1;
            classToValueTypeMap = new Dictionary<Type, ValueType>();
            classToValueTypeMap[v0.GetType()] = ValueType.String;
            classToValueTypeMap[v1.GetType()] = ValueType.Long;
            classToValueTypeMap[v2.GetType()] = ValueType.Integer;
            classToValueTypeMap[v3.GetType()] = ValueType.Boolean;
            classToValueTypeMap[v4.GetType()] = ValueType.Float;
            classToValueTypeMap[v5.GetType()] = ValueType.Double;
            classToValueTypeMap[v6.GetType()] = ValueType.Byte;
            classToValueTypeMap[v7.GetType()] = ValueType.Short;

            classToValueTypeMap[(new Vector3()).GetType()] = ValueType.Point;
            classToValueTypeMap[(new Vector3()).GetType()] = ValueType.MVVector;
            classToValueTypeMap[(new Quaternion()).GetType()] = ValueType.Quaternion;
            classToValueTypeMap[(new Color()).GetType()] = ValueType.Color;
            classToValueTypeMap[(new OID()).GetType()] = ValueType.OID;
            classToValueTypeMap[(new LinkedList<object>()).GetType()] = ValueType.LinkedList;
            classToValueTypeMap[(new List<object>()).GetType()] = ValueType.List;
            classToValueTypeMap[(new Hashtable()).GetType()] = ValueType.HashSet;
            classToValueTypeMap[(new Dictionary<string, object>()).GetType()] = ValueType.HashMap;
            classToValueTypeMap[(new byte[3]).GetType()] = ValueType.ByteArray;
            classToValueTypeMap[(new SortedDictionary<string, object>()).GetType()] = ValueType.TreeMap;
        }

        public void WriteEncodedObject(object val)
        {
         //   UnityEngine.Debug.LogError("WriteEncodedObject "+ val);
            if (val == null)
                outMessage.Write((byte)ValueType.Null);
            else
            {
                Type c = val.GetType();
                ValueType index;
                if (!classToValueTypeMap.TryGetValue(c, out index))
                    throw new Exception("EncodedObjectIO.WriteEncodedObject: no support for object of class " + val.GetType());
                switch (index)
                {
                    case ValueType.String:
                        outMessage.Write((byte)ValueType.String);
                        outMessage.Write((string)val);
                        break;
                    case ValueType.Byte:
                        outMessage.Write((byte)ValueType.Long);
                        outMessage.Write((byte)val);
                        break;
                    case ValueType.Short:
                        outMessage.Write((byte)ValueType.Short);
                        outMessage.Write((short)val);
                        break;
                    case ValueType.Long:
                        outMessage.Write((byte)ValueType.Long);
                        outMessage.Write((long)val);
                        break;
                    case ValueType.Integer:
                        outMessage.Write((byte)ValueType.Integer);
                        outMessage.Write((int)val);
                        break;
                    case ValueType.Boolean:
                        outMessage.Write((byte)(((bool)val) ? ValueType.BooleanTrue : ValueType.BooleanFalse));
                        break;
                    case ValueType.Float:
                        outMessage.Write((byte)ValueType.Float);
                        outMessage.Write((float)val);
                        break;
                    case ValueType.Double:
                        outMessage.Write((byte)ValueType.Double);
                        outMessage.Write((double)val);
                        break;
                    case ValueType.Point:
                   //     UnityEngine.Debug.LogError("Send Point");
                        outMessage.Write((byte)ValueType.Point);
                        outMessage.Write((Vector3)val);
                        break;
                    case ValueType.MVVector:
                    //    UnityEngine.Debug.LogError("Send AOVector");
                        outMessage.Write((byte)ValueType.MVVector);
                        outMessage.Write((Vector3)val);
                        break;
                    case ValueType.Quaternion:
                        outMessage.Write((byte)ValueType.Quaternion);
                        outMessage.Write((Quaternion)val);
                        break;
                    case ValueType.Color:
                        outMessage.Write((byte)ValueType.Color);
                        outMessage.Write((Color)val);
                        break;
                    case ValueType.OID:
                        outMessage.Write((byte)ValueType.OID);
                        outMessage.Write((OID)val);
                        break;
                    case ValueType.LinkedList:
                        outMessage.Write((byte)ValueType.LinkedList);
                        LinkedList<object> linkedList = (LinkedList<object>)val;
                        outMessage.Write(linkedList.Count);
                        foreach (object obj in linkedList)
                        {
                            // Recurse
                            WriteEncodedObject(obj);
                        }
                        break;
                    case ValueType.List:
                        outMessage.Write((byte)ValueType.List);
                        List<object> list = (List<object>)val;
                        outMessage.Write(list.Count);
                        foreach (object obj in list)
                        {
                            // Recurse
                            WriteEncodedObject(obj);
                        }
                        break;
                    case ValueType.HashSet:
                        outMessage.Write((byte)ValueType.HashSet);
                        Hashtable set = (Hashtable)val;
                        outMessage.Write(set.Count);
                        foreach (object obj in set.Keys)
                        {
                            // Recurse
                            WriteEncodedObject(obj);
                        }
                        break;
                    case ValueType.HashMap:
                        outMessage.Write((byte)ValueType.HashMap);
                        WritePropertyMap((Dictionary<string, object>)val);
                        break;
                    case ValueType.ByteArray:
                        outMessage.Write((byte)ValueType.ByteArray);
                        outMessage.Write((byte[])val);
                        break;
                    case ValueType.TreeMap:
                        outMessage.Write((byte)ValueType.TreeMap);
                        WriteSortedPropertyMap((SortedDictionary<string, object>)val);
                        break;
                    default:
                        //log.Error("EncodedObjectIO.WriteEncodedObject: index " + index + 
                        //    " out of bounds; class " + c.Name);
                        break;
                }
            }
        }

        public System.Object ReadEncodedObject()
        {
            int count;
            byte typecode = inMessage.ReadByte();
            switch ((ValueType)typecode)
            {
                case ValueType.Null:
                    return null;
                case ValueType.String:
                    return inMessage.ReadString();
                case ValueType.Byte:
                    return inMessage.ReadByte();
                case ValueType.Short:
                    return inMessage.ReadInt16();
                case ValueType.Long:
                    return inMessage.ReadInt64();
                case ValueType.Integer:
                    return inMessage.ReadInt32();
                case ValueType.BooleanFalse:
                    return false;
                case ValueType.BooleanTrue:
                    return true;
                case ValueType.Float:
                    return inMessage.ReadSingle();
                case ValueType.Double:
                    return inMessage.ReadDouble();
                case ValueType.Point:
                    return inMessage.ReadIntVector();
                case ValueType.MVVector:
                    return inMessage.ReadVector();
                case ValueType.Quaternion:
                    return inMessage.ReadQuaternion();
                case ValueType.Color:
                    return inMessage.ReadColor();
                case ValueType.OID:
                    return inMessage.ReadOID();
                case ValueType.LinkedList:
                    count = inMessage.ReadInt32();
                    LinkedList<object> linkedList = new LinkedList<object>();
                    for (int i = 0; i < count; i++)
                        linkedList.AddLast(ReadEncodedObject());
                    return linkedList;
                case ValueType.List:
                    count = inMessage.ReadInt32();
                    List<object> list = new List<object>();
                    for (int i = 0; i < count; i++)
                        list.Add(ReadEncodedObject());
                    return list;
                case ValueType.HashSet:
                    count = inMessage.ReadInt32();
                    Hashtable set = new Hashtable();
                    for (int i = 0; i < count; i++)
                    {
                        object value = ReadEncodedObject();
                        set.Add(value, null);
                    }
                    return set;
                case ValueType.HashMap:
                    return ReadPropertyMap();
                case ValueType.ByteArray:
                    return inMessage.ReadBytes();
                case ValueType.TreeMap:
                    return ReadSortedPropertyMap();
                default:
                    //log.Error("EncodedObjectIO.ReadEncodedObject: Illegal value type code " + typecode);
                    return null;
            }
        }

        public static String FormatEncodedObject(object val)
        {
            if (val == null)
                return "null";
            else
            {
                string s = "";
                Type c = val.GetType();
                ValueType index;

                if (classToValueTypeMap.TryGetValue(c, out index))
                {
                    switch (index)
                    {
                        case ValueType.String:
                            return (string)val;
                        case ValueType.Long:
                            return val.ToString();
                        case ValueType.Integer:
                            return val.ToString();
                        case ValueType.Boolean:
                            return val.ToString();
                        case ValueType.Float:
                            return val.ToString();
                        case ValueType.Double:
                            return val.ToString();
                        case ValueType.Point:
                            return val.ToString();
                        case ValueType.MVVector:
                            return val.ToString();
                        case ValueType.Quaternion:
                            return val.ToString();
                        case ValueType.Color:
                            return val.ToString();
                        case ValueType.OID:
                            return val.ToString();
                        case ValueType.HashMap:
                            Dictionary<string, object> map = (Dictionary<string, object>)val;
                            foreach (KeyValuePair<string, object> pair in map)
                            {
                                string key = pair.Key;
                                object value = pair.Value;
                                if (s != "")
                                    s += ",";
                                s += key + "=" + FormatEncodedObject(value);
                                //log.InfoFormat("formatting map s = " + s);
                            }
                            return "map[" + s + "]";
                        case ValueType.LinkedList:
                            LinkedList<object> linkedList = (LinkedList<object>)val;
                            foreach (object obj in linkedList)
                            {
                                // Recurse
                                if (s != "")
                                    s += ",";
                                s += FormatEncodedObject(obj);
                            }
                            return "list[" + s + "]";
                        case ValueType.List:
                            List<object> list = (List<object>)val;
                            foreach (object obj in list)
                            {
                                // Recurse
                                if (s != "")
                                    s += ",";
                                s += FormatEncodedObject(obj);
                            }
                            return "list[" + s + "]";
                        case ValueType.HashSet:
                            Hashtable set = (Hashtable)val;
                            foreach (object obj in set.Keys)
                            {
                                // Recurse
                                if (s != "")
                                    s += ",";
                                s += FormatEncodedObject(obj);
                            }
                            return "set[" + s + "]";
                        default:
                            // Keep the compiler happy
                            return "";
                    }
                }
                else
                {
                    return val.ToString();
                }
            }
        }

        public void WritePropertyMap(Dictionary<string, object> propertyMap)
        {
            outMessage.Write((int)(propertyMap == null ? 0 : propertyMap.Count));
            if (propertyMap != null)
            {
                foreach (KeyValuePair<string, object> pair in propertyMap)
                {
                    string key = pair.Key;
                    object val = pair.Value;
                    outMessage.Write(key);
                    WriteEncodedObject(val);
                }
            }
        }

        public void WriteSortedPropertyMap(SortedDictionary<string, object> propertyMap)
        {
            outMessage.Write((int)(propertyMap == null ? 0 : propertyMap.Count));
            if (propertyMap != null)
            {
                foreach (KeyValuePair<string, object> pair in propertyMap)
                {
                    string key = pair.Key;
                    object val = pair.Value;
                    outMessage.Write(key);
                    WriteEncodedObject(val);
                }
            }
        }

        public Dictionary<string, object> ReadPropertyMap()
        {
            int count = inMessage.ReadInt32();
            Dictionary<string, object> map = new Dictionary<string, object>();
            for (int i = 0; i < count; i++)
            {
                string key = inMessage.ReadString();
                object value = ReadEncodedObject();
                map[key] = value;
            }
            return map;
        }

        public SortedDictionary<string, object> ReadSortedPropertyMap()
        {
            int count = inMessage.ReadInt32();
            SortedDictionary<string, object> map = new SortedDictionary<string, object>();
            for (int i = 0; i < count; i++)
            {
                string key = inMessage.ReadString();
                object value = ReadEncodedObject();
                map[key] = value;
            }
            return map;
        }

    }

    public class PropertyMap
    {
        Dictionary<string, object> properties;

        public PropertyMap()
        {
            properties = new Dictionary<string, object>();
        }

        #region Serialization methods
        public void ParseMessage(AtavismIncomingMessage inMessage)
        {
            EncodedObjectIO io = new EncodedObjectIO(inMessage);
            properties = io.ReadPropertyMap();
        }

        public void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            EncodedObjectIO io = new EncodedObjectIO(outMessage);
            io.WritePropertyMap(properties);
        }

        // This is used to read CharacterResponse,
        // CharacterDeleteResponse, and CharacterCreateResponse
        // messages, but nothing else
        public void OldParseMessage(AtavismIncomingMessage inMessage)
        {
            int numProperties = inMessage.ReadInt32();
            for (int i = 0; i < numProperties; i++)
            {
                string key = inMessage.ReadString();
                string type = inMessage.ReadString();
                string value = inMessage.ReadString();
                switch (type)
                {
                    case "S":
                        properties[key] = value;
                        break;
                    case "B":
                        properties[key] = bool.Parse(value);
                        break;
                    case "I":
                        properties[key] = int.Parse(value);
                        break;
                    case "L":
                        properties[key] = long.Parse(value);
                        break;
                    case "O":
                        //properties[key] = OID.fromString(value);
                        properties[key] = OID.fromLong(long.Parse(value));
                        break;
                    case "F":
                        value = value.Replace('.', ',');//ZBDS to U2018 and .Net 4.6
                        properties[key] = float.Parse(value);
                        break;
                    case "V":
                        properties[key] = ParseVector3(value);
                        break;
                    case "Q":
                        properties[key] = ParseQuaternion(value);
                        break;
                    default:
                        //throw new System.IO.InvalidDataException("unknown property type");
                        throw new Exception("unknown property type");
                }
            }
        }

        public static Vector3 ParseVector3(string vector)
        {
            // the format is "Vector3(x, y, z)"
            if (!vector.StartsWith("Vector3("))
                throw new FormatException();

            string[] vals = vector.Substring(8).TrimEnd(')').Split(',');

            return new Vector3(float.Parse(vals[0].Trim(), CultureInfo.InvariantCulture),
                                float.Parse(vals[1].Trim(), CultureInfo.InvariantCulture),
                                float.Parse(vals[2].Trim(), CultureInfo.InvariantCulture));
        }

        public Quaternion ParseQuaternion(string quat)
        {
            // the format is "Quaternion(w, x, y, z)"
            if (!quat.StartsWith("Quaternion("))
                throw new FormatException();

            string[] values = quat.Substring(11).TrimEnd(')').Split(',');

            return new Quaternion(float.Parse(values[0], CultureInfo.InvariantCulture),
                                  float.Parse(values[1], CultureInfo.InvariantCulture),
                                  float.Parse(values[2], CultureInfo.InvariantCulture),
                                  float.Parse(values[3], CultureInfo.InvariantCulture));

        }

        // This is used for CharacterRequest, CharacterCreateRequest
        // and CharacterDeleteRequest but nothing else.
        public void OldWriteMessage(AtavismOutgoingMessage outMessage)
        {
           // UnityEngine.Debug.LogError("OldWriteMessage");
            List<String> propStrings = new List<string>();
            int propCount = 0;
            foreach (string key in properties.Keys)
            {
                object value = properties[key];
                if (value is string)
                {
                    propStrings.Add(key);
                    propStrings.Add("S");
                    propStrings.Add((string)value);
                    propCount++;
                }
                else if (value is bool)
                {
                    propStrings.Add(key);
                    propStrings.Add("B");
                    propStrings.Add(value.ToString());
                    propCount++;
                }
                else if (value is int)
                {
                    propStrings.Add(key);
                    propStrings.Add("I");
                    propStrings.Add(value.ToString());
                    propCount++;
                }
                else if (value is long)
                {
                    propStrings.Add(key);
                    propStrings.Add("L");
                    propStrings.Add(value.ToString());
                    propCount++;
                }
                else if (value is float)
                {
                    propStrings.Add(key);
                    propStrings.Add("F");
                    propStrings.Add(value.ToString());
                    propCount++;
                }
                else if (value is Vector3)
                {
                    propStrings.Add(key);
                    propStrings.Add("V");
                    propStrings.Add(value.ToString());
                    propCount++;
                }
                else if (value is Quaternion)
                {
                    propStrings.Add(key);
                    propStrings.Add("Q");
                    propStrings.Add(value.ToString());
                    propCount++;
                }
                else if (value is OID)
                {
                    propStrings.Add(key);
                    propStrings.Add("O");
                    //propStrings.Add(value.ToString());
                    OID oid = (OID)value;
                    propStrings.Add((oid.ToLong()).ToString());
                    propCount++;
                }
                else
                {
                    //LogManager.Instance.Write("PropertyMap: unknown type, skipping key: {0}", key);
                }
            }
            outMessage.Write(propCount);
            foreach (string entry in propStrings)
                outMessage.Write(entry);
        }

        #endregion

        #region Get methods
        public int GetIntProperty(string key)
        {
            object value = null;
            if (properties.TryGetValue(key, out value))
            {
                if (value is int)
                    return (int)value;
            }
            //throw new System.IO.InvalidDataException("GetIntProperty property missing or type wrong");
            throw new Exception("GetIntProperty property missing or type wrone");
        }

        public long GetLongProperty(string key)
        {
            object value = null;
            if (properties.TryGetValue(key, out value))
            {
                if (value is long)
                    return (long)value;
            }
            //throw new System.IO.InvalidDataException("GetLongProperty property missing or type wrong");
            throw new Exception("GetLongProperty property missing or type wrong");
        }

        public float GetFloatProperty(string key)
        {
            object value = null;
            if (properties.TryGetValue(key, out value))
            {
                if (value is float)
                    return (float)value;
            }
            //throw new System.IO.InvalidDataException("GetFloatProperty property missing or type wrong");
            throw new Exception("GetFloatProperty property missing or type wrong");
        }

        public string GetStringProperty(string key)
        {
            object value = null;
            if (properties.TryGetValue(key, out value))
            {
                if (value is string)
                    return (string)value;
            }
            //throw new System.IO.InvalidDataException("GetStringProperty property");
            throw new Exception("GetStringProperty property missing or type wrong");
        }

        public Vector3 GetVector3Property(string key)
        {
            object value = null;
            if (properties.TryGetValue(key, out value))
            {
                if (value is Vector3)
                    return (Vector3)value;
            }
            //throw new System.IO.InvalidDataException("GetVector3Property property missing or type wrong");
            throw new Exception("GetVector3Property property missing or type wrong");
        }

        public Quaternion GetQuaternionProperty(string key)
        {
            object value = null;
            if (properties.TryGetValue(key, out value))
            {
                if (value is Quaternion)
                    return (Quaternion)value;
            }
            //throw new System.IO.InvalidDataException("GetQuaternionProperty property missing or type wrong");
            throw new Exception("GetQuaternionProperty property missing or type wrong");
        }

        public object GetObjectProperty(string key)
        {
            object value = null;
            if (properties.TryGetValue(key, out value))
                return value;
            //throw new System.IO.InvalidDataException("GetObjectProperty property missing or type wrong");
            throw new Exception("GetObjectProperty property missing or type wrong");
        }

        #endregion

        public Dictionary<string, object> Properties
        {
            get
            {
                return properties;
            }
        }
    }

    public class OldMeshInfo : MeshInfo
    {
        public override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            meshFile = inMessage.ReadString();
            int submeshCount = inMessage.ReadInt32();
            // submesh count of zero means we leave the submesh list null
            if (submeshCount != 0)
                submeshList = new List<SubmeshInfo>();
            for (int i = 0; i < submeshCount; ++i)
            {
                SubmeshInfo submeshInfo = new SubmeshInfo();
                submeshInfo.submeshName = inMessage.ReadString();
                submeshInfo.materialName = inMessage.ReadString();
                submeshInfo.castShadows = true;
                submeshInfo.receiveShadows = true;
                submeshList.Add(submeshInfo);
                //log.InfoFormat("submesh name: {0}; material: {1}", submeshInfo.SubmeshName, submeshInfo.MaterialName);
            }
        }
    }
    /// <summary>
    ///   Portion of model information associated with a single mesh file.
    /// </summary>
    public class MeshInfo
    {
        //protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MeshInfo));

        protected string meshFile;
        protected List<SubmeshInfo> submeshList;
        protected int displayID = -1;

        public virtual void ParseMessage(AtavismIncomingMessage inMessage)
        {
            meshFile = inMessage.ReadString();
            int submeshCount = inMessage.ReadInt32();
            // submesh count of zero means we leave the submesh list null
            if (submeshCount != 0)
                submeshList = new List<SubmeshInfo>();
            for (int i = 0; i < submeshCount; ++i)
            {
              //  UnityEngine.Debug.LogError("SubmeshInfo "+i);
                SubmeshInfo submeshInfo = new SubmeshInfo();
                submeshInfo.SubmeshName = inMessage.ReadString();
                submeshInfo.MaterialName = inMessage.ReadString();
                submeshInfo.CastShadows = inMessage.ReadBool();
                submeshInfo.ReceiveShadows = inMessage.ReadBool();
                submeshList.Add(submeshInfo);
                //log.InfoFormat("submesh name: {0}; material: {1}; castShadows: {2}", submeshInfo.SubmeshName, submeshInfo.MaterialName, submeshInfo.CastShadows.ToString());
            }
            displayID = inMessage.ReadInt32();
        }

        public void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(meshFile);
            outMessage.Write(submeshList.Count);
            foreach (SubmeshInfo submeshInfo in submeshList)
            {
                outMessage.Write(submeshInfo.SubmeshName);
                outMessage.Write(submeshInfo.MaterialName);
                outMessage.Write(submeshInfo.CastShadows);
                outMessage.Write(submeshInfo.ReceiveShadows);
            }
            outMessage.Write(displayID);
        }

        #region Properties
        public string MeshFile
        {
            get
            {
                return meshFile;
            }
            set
            {
                meshFile = value;
            }
        }
        public List<SubmeshInfo> SubmeshList
        {
            get
            {
                return submeshList;
            }
            set
            {
                submeshList = value;
            }
        }
        public int DisplayID
        {
            get
            {
                return displayID;
            }
            set
            {
                displayID = value;
            }
        }
        #endregion
    }

    public class AnimationEntry
    {
        public string animationName;
        public float animationSpeed;
        public bool loop;

        public string AnimationName
        {
            get
            {
                return animationName;
            }
            set
            {
                animationName = value;
            }
        }

        public float AnimationSpeed
        {
            get
            {
                return animationSpeed;
            }
            set
            {
                animationSpeed = value;
            }
        }

        public bool Loop
        {
            get
            {
                return loop;
            }
            set
            {
                loop = value;
            }
        }
    }

    public class SoundEntry
    {
        public string soundName;
        public float soundSpeed;
        public float soundGain;
        public bool loop;
        public override string ToString()
        {
            return soundName + (loop ? " (looping)" : "");
        }
    }

    public class InventoryUpdateEntry
    {
        public long itemId;
        public int containerId;
        public int slotId;
        public string name;
        public string icon;
        public int count;
    }

    public class ItemEntry
    {
        public string name;
        public string icon;
        public int count;
    }

    public class InvItemInfo
    {
        public long itemId;
        public string name;
        public string icon;
        public int count;
    }

    public class GroupInfoEntry
    {
        public long memberId;
        public string memberName;
    }

    public class AbilityEntry
    {
        public string name;
        public string icon;
        public string category;
    }

    public enum ObjectNodeType
    {
        Prop = 0,
        Npc = 1,
        Item = 2,
        User = 3
    }

    public enum LightNodeType
    {
        Point = 0,
        Directional = 1,
        Spotlight = 2
    }

    public enum MasterTcpMessageType
    {
        Login = 0,
        LoginResponse = 1
    }

    public enum WorldTcpMessageType
    {
        OldCharacterRequest = 1,
        CharacterResponse = 2,
        CharacterDeleteRequest = 3,
        CharacterDeleteResponse = 4,
        CharacterCreateRequest = 5,
        CharacterCreateResponse = 6,
        CharacterRequest = 7,
        CharacterSelectRequest = 8,
        CharacterSelectResponse = 9,
        SecureCharacterRequest = 10,
        ServerListRequest = 11,
        ServerListResponse = 12,
        Heartbeat = 13,
        HeartbeatResponse = 14,
        PrefabRequest = 15,
        PrefabResponse = 16,
        IconPrefabRequest = 17,
        IconPrefabResponse = 18,

    }

    public enum MasterMessageType
    {
        Resolve = 0,
        Chat = 1,
        ResolveResponse = 2
    }

    public enum WorldMessageType
    {
        Login = 1,
        Direction = 2,
        Comm = 3,
        LoginResponse = 4,
        Logout = 5,
        OldTerrainConfig = 6,   // No longer used
        SkyboxMaterial = 7,
        NewObject = 8,
        Orientation = 9,
        FreeObject = 10,
        Acquire = 11,
        AcquireResponse = 12,
        Command = 13,
        Equip = 14,
        EquipResponse = 15,
        Unequip = 16,
        UnequipResponse = 17,
        Attach = 18,
        Detach = 19,
        Combat = 20,
        AutoAttack = 21,
        StatUpdate = 22,
        Damage = 23,
        Drop = 24,
        DropResponse = 25,
        Animation = 26,
        Sound = 27,
        AmbientSound = 28,
        FollowTerrain = 29,
        Portal = 30,
        AmbientLight = 31,
        NewLight = 32,
        TradeStartRequest = 33,
        TradeStart = 34,
        TradeOfferRequest = 35,
        TradeComplete = 36,
        TradeOfferUpdate = 37,
        StateMessage = 38,
        QuestInfoRequest = 39,
        QuestInfoResponse = 40,
        QuestResponse = 41,
        RegionConfig = 42,
        InventoryUpdate = 43,
        QuestLogInfo = 44,
        QuestStateInfo = 45,
        RemoveQuestRequest = 46,
        RemoveQuestResponse = 47,
        GroupInfo = 48,
        QuestConcludeRequest = 49,
        UiTheme = 50,
        LootAll = 51,
        OldModelInfo = 52,
        FragmentMessage = 53,
        RoadInfo = 54,
        Fog = 55,
        AbilityUpdate = 56,
        AbilityInfo = 57,
        OldObjectProperty = 61,
        ObjectProperty = 62,
        AddParticleEffect = 63,
        RemoveParticleEffect = 64,
        ClientParameter = 65,      // No longer used
        TerrainConfig = 66,
        TrackObjectInterpolation = 67,
        TrackLocationInterpolation = 68,
        FreeRoad = 69,
        OldExtension = 70,
        InvokeEffect = 71,
        ActivateItem = 72,
        MobPath = 73,
        AggregatedRDP = 74,
        NewDecal = 75,
        FreeDecal = 76,
        ModelInfo = 77,
        SoundControl = 78,
        DirLocOrient = 79,
        AuthorizedLogin = 80,
        AuthorizedLoginResponse = 81,
        LoadingState = 82,
        Extension = 83,
        WorldFile = 85,
        IslandManifest = 86,
        SceneLoaded = 87,
        Ha =88
    }

    public enum CommChannel
    {
        Say = 1,
        ServerInfo = 2,
        CombatInfo = 3,
        CombatInfoSelf = 4,
        CombatInfoOther = 5
    }

    public delegate void WorldMessageHandler(BaseWorldMessage message);

    public class RequireLoginFilter
    {

        IWorldManager worldManager;
        public RequireLoginFilter(IWorldManager worldManager)
        {
            this.worldManager = worldManager;
        }

        public bool ShouldQueue(BaseWorldMessage message)
        {

            if (!worldManager.PlayerInitialized)
            {
                // The connection isn't yet set up -- we only handle the login response message here
                if (message is AuthorizedLoginResponseMessage)
                {
                    AuthorizedLoginResponseMessage response = (AuthorizedLoginResponseMessage)message;
                    // I need to handle the response in this thread, because I don't want to skip any messages
                    // while I'm waiting for the login response to be handled by another thread.
                    if (response.Success)
                    {
                        worldManager.PlayerId = response.Oid;
                        AtavismLogger.LogDebugMessage("Player initialized -- playerId = " + worldManager.PlayerId);
                    }
                    else
                    {
                        string[] args = new string[1];
                        switch (response.Message)
                        {
                            case "Login Failed: Unsupported client version":
                                args[0] = "Unsupported client version";
                                break;
                            case "Login Failed: Servers at capacity, please try again later.":
                                args[0] = "Servers at capacity, please try again later";
                                break;
                            case "Login Failed: Secure token invalid.":
                                args[0] = "Secure token invalid";
                                break;
                            case "Login Failed: Already connected":
                                args[0] = "Already connected";
                                break;
                        }
                        AtavismEventSystem.DispatchEvent("LOGIN_WORLD_RESPONSE", args);
                        AtavismLogger.LogError("Failure Login to World with message " + response.Message);

                    }
                    // Still queue the login response for handling by other code, so the login message can be
                    // dealt with if needed.
                }
                else
                {
                    //log.WarnFormat("Ignoring message {0}, since player is not set up", message);
                    return false;
                }
            }
            return true;
        }
    }
    /// <summary>
    ///   This class handles aggregate messages, as well as message fragments.
    /// </summary>
	public class DefragmentingMessageDispatcher : MessageDispatcher
    {
        //private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(DefragmentingMessageDispatcher));

        List<BaseWorldMessage> messageQueue = new List<BaseWorldMessage>();

        protected class FragmentInfo
        {
            public int numFragments;
            public Dictionary<int, byte[]> payloads = new Dictionary<int, byte[]>();
        }

        private Dictionary<int, FragmentInfo> fragmentDictionary =
            new Dictionary<int, FragmentInfo>();

        public override void QueueMessage(BaseWorldMessage message)
        {
           // Debug.LogError("QueueMessage "+message.MessageType);
            Monitor.Enter(messageQueue);
            try
            {
                bool addToQueue = true;
                if (message.MessageType == WorldMessageType.FragmentMessage)
                {
                    HandleFragmentMessage(message);
                    FragmentMessage fragMessage = (FragmentMessage)message;
                    if (fragMessage.FragmentNumber != 0)
                        addToQueue = false;
                }
                if (addToQueue)
                    messageQueue.Add(message);
                AssembleFragments();
                while (messageQueue.Count > 0)
                {
                    BaseWorldMessage message2 = messageQueue[0];
                    // we can now handle messages until we get a fragment
                    if (message2.MessageType == WorldMessageType.FragmentMessage)
                        break;
                    if (message2.MessageType == WorldMessageType.AggregatedRDP)
                    {
                        AggregatedRDPMessage aggregateMessage = (AggregatedRDPMessage)message2;
                        foreach (BaseWorldMessage message3 in aggregateMessage.SubMessages)
                        {
                            MessageDispatcher.Instance.QueueMessage(message3);
                        }
                    }
                    else
                    {
                        MessageDispatcher.Instance.QueueMessage(message2);
                    }
                    messageQueue.RemoveAt(0);
                }
            }
            finally
            {
                Monitor.Exit(messageQueue);
            }
        }


        /// <summary>
        ///   Handle fragmented messages
        /// </summary>
        /// <param name="message"></param>
        private void HandleFragmentMessage(BaseWorldMessage message)
        {
            FragmentMessage fragment = (FragmentMessage)message;
            //log.InfoFormat("Got message fragment for {0}: {1}/{2}",
            //		       fragment.MessageNumber, fragment.FragmentNumber, fragment.NumFragments);
            if (!fragmentDictionary.ContainsKey(fragment.MessageNumber))
                fragmentDictionary[fragment.MessageNumber] = new FragmentInfo();
            FragmentInfo fragInfo = fragmentDictionary[fragment.MessageNumber];
            fragInfo.payloads[fragment.FragmentNumber] = fragment.Payload;
            if (fragment.FragmentNumber == 0)
                fragInfo.numFragments = fragment.NumFragments;
        }

        private void AssembleFragments()
        {
            // see if we have any completed messages
            Dictionary<int, BaseWorldMessage> completeMessages =
                new Dictionary<int, BaseWorldMessage>();
            foreach (int messageNumber in fragmentDictionary.Keys)
            {
                FragmentInfo fragInfo = fragmentDictionary[messageNumber];
                if (fragInfo.numFragments > 0 &&
                    fragInfo.payloads.Count == fragInfo.numFragments)
                {
                    int payloadLen = 0;
                    // Reassemble the packets
                    for (int i = 0; i < fragInfo.numFragments; ++i)
                        payloadLen += fragInfo.payloads[i].Length;
                    byte[] fullPayload = new byte[payloadLen];
                    int offset = 0;
                    for (int i = 0; i < fragInfo.numFragments; ++i)
                    {
                        int len = fragInfo.payloads[i].Length;
                        Array.Copy(fragInfo.payloads[i], 0, fullPayload, offset, len);
                        offset += len;
                    }
                    completeMessages[messageNumber] = GetMessage(fullPayload);
                }
            }

            // Find all the fragments that have been completed,
            // replace the stub message with the real message.
            foreach (int messageNumber in completeMessages.Keys)
            {
                fragmentDictionary.Remove(messageNumber);
                for (int i = 0; i < messageQueue.Count; ++i)
                {
                    BaseWorldMessage message = messageQueue[i];
                    if (message.MessageType == WorldMessageType.FragmentMessage)
                    {
                        FragmentMessage fragMessage = (FragmentMessage)message;
                        if (fragMessage.MessageNumber == messageNumber)
                            messageQueue[i] = completeMessages[messageNumber];
                    }
                }
            }
        }


        private BaseWorldMessage GetMessage(byte[] buffer)
        {
            //string tmp = "Buffer ";
            //for (int i = 0; i < buffer.Length; ++i)
            //    tmp += ":" + buffer[i];

            //Logger.Log(0, tmp);

            if (buffer.Length < 8)
            {
                //log.ErrorFormat("Invalid message length: {0}", buffer.Length);
                return null;
            }
            AtavismIncomingMessage inMessage = new AtavismIncomingMessage(buffer, 0, buffer.Length, new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 0));
            BaseWorldMessage message = WorldMessageFactory.ReadMessage(inMessage);
            if (message == null)
            {
                //log.Warn("Failed to read message from factory");
                return null;
            }
            //log.Info("Handled fragmented message");
            return message;
        }
    }

    public class MessageDispatcher
    {
        //private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(MessageDispatcher));

        Dictionary<WorldMessageType, List<WorldMessageHandler>> preMessageHandlers =
            new Dictionary<WorldMessageType, List<WorldMessageHandler>>();
        Dictionary<WorldMessageType, List<WorldMessageHandler>> messageHandlers =
            new Dictionary<WorldMessageType, List<WorldMessageHandler>>();
        Dictionary<WorldMessageType, List<WorldMessageHandler>> postMessageHandlers =
            new Dictionary<WorldMessageType, List<WorldMessageHandler>>();

        static MessageDispatcher dispatcher;
        Queue<BaseWorldMessage> messageQueue = new Queue<BaseWorldMessage>();
        // Return true if we can handle the message
        Predicate<BaseWorldMessage> queueFilter = null;

        // We keep a boxcar average of the messages handled over the
        // last second
        long messagesHandled = 0;
        long averagingStartTicks = 0;
        long lastMessagesHandled = 0;
        int messagesHandledPerSecond = 0;

        long lastBytesReceivedCounter = 0;
        long lastBytesSentCounter = 0;
        int bytesReceivedPerSecond = 0;
        int bytesSentPerSecond = 0;

        long messagesQueued = 0;

        // Variables to control when to disconnect the client if no messages have been received. Values are in seconds
        float lastMessageReceived = 0;
        float maxTimeWithoutMessage = 120;

        public float MaxTimeWithoutMessage
        {
            get
            {
                return maxTimeWithoutMessage;
            }
            set
            {
                maxTimeWithoutMessage = value;
            }
        }

        public void RegisterHandler(WorldMessageType messageType,
                                        WorldMessageHandler messageHandler)
        {
            if (!messageHandlers.ContainsKey(messageType))
                messageHandlers[messageType] = new List<WorldMessageHandler>();
            messageHandlers[messageType].Add(messageHandler);
        }

        public void RegisterPreHandler(WorldMessageType messageType,
                                       WorldMessageHandler messageHandler)
        {
            if (!preMessageHandlers.ContainsKey(messageType))
                preMessageHandlers[messageType] = new List<WorldMessageHandler>();
            preMessageHandlers[messageType].Add(messageHandler);
        }

        public void RegisterPostHandler(WorldMessageType messageType,
                                        WorldMessageHandler messageHandler)
        {
            if (!postMessageHandlers.ContainsKey(messageType))
                postMessageHandlers[messageType] = new List<WorldMessageHandler>();
            postMessageHandlers[messageType].Add(messageHandler);
        }

        public virtual void QueueMessage(BaseWorldMessage message)
        {
          //  AtavismLogger.LogError("QueueMessage message: " + message+" "+ messagesQueued);

            Monitor.Enter(messageQueue);
            try
            {
                messageQueue.Enqueue(message);
                messagesQueued++;
            }
            finally
            {
                Monitor.Exit(messageQueue);
            }
        }

        public void ClearQueue()
        {
            messageQueue.Clear();
        }

        public void SetWorldMessageFilter(Predicate<BaseWorldMessage> queueFilter)
        {
            this.queueFilter = queueFilter;
        }

        private bool HandleMessage(BaseWorldMessage message)
        {
          if (queueFilter != null && !queueFilter(message))
            {
            //    AtavismLogger.LogError("HandleMessage skip ");
                // Skip this message, and pull it from the queue
                return true;
            }
            List<WorldMessageHandler> handlers;
            // Run through the pre-handlers for the message -- these can delay processing
            if (preMessageHandlers.TryGetValue(message.MessageType, out handlers))
            {
                foreach (WorldMessageHandler handler in handlers)
                {
                    handler(message);
                    if (message.DelayHandling)
                    {
                       // AtavismLogger.LogError("HandleMessage DelayHandling ");
                        return false;
                    }
                }
            }

            //UnityEngine.Debug.Log("Handle message called for " + message);
            if (messageHandlers.TryGetValue(message.MessageType, out handlers))
            {
                foreach (WorldMessageHandler handler in handlers)
                {
                    long before = System.Environment.TickCount;
                    //UnityEngine.Debug.Log("Handling message: " + message.MessageType);
                    try
                    {
                        handler(message);
                        if (message.AbortHandling)
                        {
                            return false;
                        }
                    }
                    catch (Exception e)
                    {
                        AtavismLogger.LogError("HandleMessage Exception "+e);
                    }
                }
            }
            else
            {
            }

            if (postMessageHandlers.TryGetValue(message.MessageType, out handlers))
            {
                foreach (WorldMessageHandler handler in handlers)
                {
                    handler(message);
                }
            }

            messagesHandled++;
            return true;
        }

        /// <summary>
        ///   Handle the messages that are in the queue.  This is written to 
        ///   support leaving entries in the queue.
        /// </summary>
        /// <param name="maximum"></param>
        public void HandleMessageQueue(int maximumMessages)
        {
            HandleMessageQueue(maximumMessages, 0);
        }

        /// <summary>
        ///   Handle the messages that are in the queue.  This is written to 
        ///   support leaving entries in the queue.
        /// </summary>
        /// <param name="maximum"></param>
        public void HandleMessageQueue(long maximumTime)
        {
            HandleMessageQueue(0, maximumTime);
        }

        /// <summary>
        ///   Handle messages until we hit the max messages, or the max time.
        /// </summary>
        /// <param name="maximumMessages">the maximum number of messages to handle (or 0 for unlimited)</param>
        /// <param name="maximumTime">the maximum number of milliseconds before we will not start handling any more messages (or 0 for unlimited)</param>
        public void HandleMessageQueue(int maximumMessages, long maximumTime)
        {
            if (AtavismClient.Instance.AtavismState == GameState.World && AtavismClient.Instance.WorldManager.sceneLoaded &&
                this.MessageCount == 0 && (UnityEngine.Time.realtimeSinceStartup - lastMessageReceived) > maxTimeWithoutMessage)
            {
                AtavismLogger.LogError("Disconnected player due to not getting a message in " + (UnityEngine.Time.realtimeSinceStartup - lastMessageReceived) + " seconds");
                AtavismClient.Instance.Disconnected();
            }
            else if (this.MessageCount > 0)
            {
                lastMessageReceived = Time.realtimeSinceStartup;
            }

            int messageCount = this.MessageCount;
            long startTime = AtavismTimeTool.CurrentTime;
            int countHandled = 0;
            for (int i = 0; i < messageCount; ++i)
            {
                if (maximumTime != 0 && (AtavismTimeTool.CurrentTime - startTime) >= maximumTime)
                    break;
                if (maximumMessages != 0 && countHandled >= maximumMessages)
                    break;
               // AtavismLogger.LogError("HandleMessageQueue HandleMessage before get message");

                BaseWorldMessage message;
                Monitor.Enter(messageQueue);
                try
                {
                    if (messageQueue.Count == 0)
                        break;
                    message = messageQueue.Peek();
                }
                finally
                {
                    Monitor.Exit(messageQueue);
                }
                // Sometimes HandleMessage will want us to stop
                if (HandleMessage(message))
                {
                    countHandled++;
                    Monitor.Enter(messageQueue);
                    try
                    {
                        messageQueue.Dequeue();
                    }
                    finally
                    {
                        Monitor.Exit(messageQueue);
                    }
                }
                else
                {
                  //  AtavismLogger.LogError("HandleMessageQueue HandleMessage false");
                    // leave it in the queue, and stop processing
                    break;
                }
            }
            long s = AtavismTimeTool.CurrentTime;
            if (s >= averagingStartTicks + 1000)
            {
                averagingStartTicks = s;
                messagesHandledPerSecond = (int)(messagesHandled - lastMessagesHandled);
                lastMessagesHandled = messagesHandled;
                long totalReceived = AtavismRdpConnection.TotalBytesReceivedCounter;
                long totalSent = AtavismRdpConnection.TotalBytesSentCounter;
                bytesReceivedPerSecond = (int)(totalReceived - lastBytesReceivedCounter);
                bytesSentPerSecond = (int)(totalSent - lastBytesSentCounter);
                lastBytesReceivedCounter = totalReceived;
                lastBytesSentCounter = totalSent;
            }
          //  AtavismLogger.LogError("HandleMessageQueue END countHandled="+ countHandled);
        }

        #region Properties
        public static MessageDispatcher Instance
        {
            get
            {
                if (dispatcher == null)
                    dispatcher = new MessageDispatcher();
                return dispatcher;
            }
        }

        public int MessageCount
        {
            get
            {
                int messageCount;
                Monitor.Enter(messageQueue);
                try
                {
                    messageCount = messageQueue.Count;
                }
                finally
                {
                    Monitor.Exit(messageQueue);
                }
                return messageCount;
            }
        }

        public int MessagesPerSecond
        {
            get
            {
                return messagesHandledPerSecond;
            }
        }

        public int BytesReceivedPerSecond
        {
            get
            {
                return bytesReceivedPerSecond;
            }
        }

        public int BytesSentPerSecond
        {
            get
            {
                return bytesSentPerSecond;
            }
        }

        public Predicate<BaseWorldMessage> QueueFilter
        {
            get
            {
                return queueFilter;
            }
            set
            {
                queueFilter = value;
            }
        }

        #endregion
    }

    public class MasterMessageFactory
    {
        public static BaseMasterMessage ReadMessage(AtavismIncomingMessage inMessage)
        {
            BaseMasterMessage rv;
            MasterMessageType messageType = inMessage.ReadMasterMessageType();
            switch (messageType)
            {
                case MasterMessageType.ResolveResponse:
                    rv = new ResolveResponseMessage();
                    break;
                default:
                    //log.ErrorFormat("Unhandled master message type: {0}", messageType);
                    return null;
            }
            rv.ParseMasterMessage(inMessage);
            return rv;
        }
    }

    public class WorldTcpMessageFactory
    {

        public static BaseWorldTcpMessage ReadMessage(AtavismIncomingMessage inMessage)
        {
            BaseWorldTcpMessage rv;
            byte[] data = inMessage.ReadBytes();
            AtavismIncomingMessage subMsg = new AtavismIncomingMessage(data, inMessage);
            WorldTcpMessageType messageType = subMsg.ReadWorldTcpMessageType();
            switch (messageType)
            {
                case WorldTcpMessageType.CharacterResponse:
                    rv = new WorldCharacterResponseMessage();
                    break;
                case WorldTcpMessageType.CharacterCreateResponse:
                    rv = new WorldCharacterCreateResponseMessage();
                    break;
                case WorldTcpMessageType.CharacterDeleteResponse:
                    rv = new WorldCharacterDeleteResponseMessage();
                    break;
                case WorldTcpMessageType.CharacterSelectResponse:
                    rv = new WorldCharacterSelectResponseMessage();
                    break;
                case WorldTcpMessageType.ServerListResponse:
                    rv = new WorldServerListResponseMessage();
                    break;
                case WorldTcpMessageType.HeartbeatResponse:
                    rv = new WorldServerLoginHeartbeatResponseMessage();
                    break;
                default:
                    //log.ErrorFormat("Unhandled world tcp message type: {0}", messageType);
                    return null;
            }
            rv.ParseWorldTcpMessage(subMsg);
            return rv;
        }
    }

    public class PrefabTcpMessageFactory
    {

        public static BaseWorldTcpMessage ReadMessage(AtavismIncomingMessage inMessage)
        {
            BaseWorldTcpMessage rv;
            byte[] data = inMessage.ReadBytes();
            AtavismIncomingMessage subMsg = new AtavismIncomingMessage(data, inMessage);
            WorldTcpMessageType messageType = subMsg.ReadWorldTcpMessageType();
            switch (messageType)
            {
                case WorldTcpMessageType.PrefabResponse:
                    rv = new PrefabResponseMessage();
                    break;
                case WorldTcpMessageType.IconPrefabResponse:
                    rv = new IconPrefabResponseMessage();
                    break;
               
                default:
                    //log.ErrorFormat("Unhandled world tcp message type: {0}", messageType);
                    return null;
            }
            rv.ParseWorldTcpMessage(subMsg);
            return rv;
        }
    }
    public class WorldMessageFactory
    {

        public static BaseWorldMessage ReadMessage(AtavismIncomingMessage inMessage)
        {
            BaseWorldMessage rv;
            long oid = inMessage.ReadInt64();
            WorldMessageType messageType = inMessage.ReadMessageType();
            if (AtavismLogger.logLevel.Equals(LogLevel.Trace))
                AtavismLogger.LogTraceMessage("messageType=" + messageType);
            switch (messageType)
            {
                case WorldMessageType.LoginResponse:
                    rv = new LoginResponseMessage();
                    break;
                case WorldMessageType.Logout:
                    rv = new LogoutMessage();
                    break;
                case WorldMessageType.Comm:
                    rv = new CommMessage();
                    break;
                case WorldMessageType.Direction:
                    rv = new DirectionMessage();
                    break;
                case WorldMessageType.Orientation:
                    rv = new OrientationMessage();
                    break;
                case WorldMessageType.TerrainConfig:
                    rv = new TerrainConfigMessage();
                    break;
                case WorldMessageType.SkyboxMaterial:
                    rv = new SkyboxMaterialMessage();
                    break;
                case WorldMessageType.NewObject:
                    rv = new NewObjectMessage();
                    break;
                case WorldMessageType.FreeObject:
                    rv = new FreeObjectMessage();
                    break;
                case WorldMessageType.AcquireResponse:
                    rv = new AcquireResponseMessage();
                    break;
                case WorldMessageType.EquipResponse:
                    rv = new EquipResponseMessage();
                    break;
                case WorldMessageType.UnequipResponse:
                    rv = new UnequipResponseMessage();
                    break;
                case WorldMessageType.Attach:
                    rv = new AttachMessage();
                    break;
                case WorldMessageType.Detach:
                    rv = new DetachMessage();
                    break;
                case WorldMessageType.StatUpdate:
                    rv = new StatUpdateMessage();
                    break;
                case WorldMessageType.Damage:
                    rv = new DamageMessage();
                    break;
                case WorldMessageType.Animation:
                    rv = new AnimationMessage();
                    break;
                case WorldMessageType.Sound:
                    rv = new SoundMessage();
                    break;
                case WorldMessageType.AmbientSound:
                    rv = new AmbientSoundMessage();
                    break;
                case WorldMessageType.FollowTerrain:
                    rv = new FollowTerrainMessage();
                    break;
                case WorldMessageType.Portal:
                    rv = new PortalMessage();
                    break;
                case WorldMessageType.AmbientLight:
                    rv = new AmbientLightMessage();
                    break;
                case WorldMessageType.NewLight:
                    rv = new NewLightMessage();
                    break;
                case WorldMessageType.TradeStartRequest:
                    rv = new TradeStartRequestMessage();
                    break;
                case WorldMessageType.TradeStart:
                    rv = new TradeStartMessage();
                    break;
                case WorldMessageType.TradeOfferRequest:
                    rv = new TradeOfferRequestMessage();
                    break;
                case WorldMessageType.TradeComplete:
                    rv = new TradeCompleteMessage();
                    break;
                case WorldMessageType.TradeOfferUpdate:
                    rv = new TradeOfferUpdateMessage();
                    break;
                case WorldMessageType.StateMessage:
                    rv = new StateMessage();
                    break;
                case WorldMessageType.QuestInfoRequest:
                    rv = new QuestInfoRequestMessage();
                    break;
                case WorldMessageType.QuestInfoResponse:
                    rv = new QuestInfoResponseMessage();
                    break;
                case WorldMessageType.QuestResponse:
                    rv = new QuestResponseMessage();
                    break;
                case WorldMessageType.RegionConfig:
                    rv = new RegionConfigMessage();
                    break;
                case WorldMessageType.InventoryUpdate:
                    rv = new InventoryUpdateMessage();
                    break;
                case WorldMessageType.QuestLogInfo:
                    rv = new QuestLogInfoMessage();
                    break;
                case WorldMessageType.QuestStateInfo:
                    rv = new QuestStateInfoMessage();
                    break;
                case WorldMessageType.RemoveQuestRequest:
                    rv = new RemoveQuestRequestMessage();
                    break;
                case WorldMessageType.RemoveQuestResponse:
                    rv = new RemoveQuestResponseMessage();
                    break;
                case WorldMessageType.GroupInfo:
                    rv = new GroupInfoMessage();
                    break;
                case WorldMessageType.UiTheme:
                    rv = new UiThemeMessage();
                    break;
                case WorldMessageType.LootAll:
                    rv = new LootAllMessage();
                    break;
                case WorldMessageType.OldModelInfo:
                    rv = new OldModelInfoMessage();
                    break;
                case WorldMessageType.FragmentMessage:
                    rv = new FragmentMessage();
                    break;
                case WorldMessageType.RoadInfo:
                    rv = new RoadInfoMessage();
                    break;
                case WorldMessageType.Fog:
                    rv = new FogMessage();
                    break;
                case WorldMessageType.AbilityInfo:
                    rv = new AbilityInfoMessage();
                    break;
                case WorldMessageType.AbilityUpdate:
                    rv = new AbilityUpdateMessage();
                    break;
                case WorldMessageType.OldObjectProperty:
                    rv = new OldObjectPropertyMessage();
                    break;
                case WorldMessageType.ObjectProperty:
                    rv = new ObjectPropertyMessage();
                    break;
                case WorldMessageType.AddParticleEffect:
                    rv = new AddParticleEffectMessage();
                    break;
                case WorldMessageType.RemoveParticleEffect:
                    rv = new RemoveParticleEffectMessage();
                    break;
                case WorldMessageType.TrackObjectInterpolation:
                    rv = new TrackObjectInterpolationMessage();
                    break;
                case WorldMessageType.TrackLocationInterpolation:
                    rv = new TrackLocationInterpolationMessage();
                    break;
                case WorldMessageType.OldExtension:
                    rv = new OldExtensionMessage();
                    break;
                case WorldMessageType.Extension:
                    rv = new ExtensionMessage();
                    break;
                case WorldMessageType.InvokeEffect:
                    rv = new InvokeEffectMessage();
                    break;
                case WorldMessageType.MobPath:
                    rv = new MobPathMessage();
                    break;
                case WorldMessageType.AggregatedRDP:
                    rv = new AggregatedRDPMessage();
                    break;
                case WorldMessageType.NewDecal:
                    rv = new NewDecalMessage();
                    break;
                case WorldMessageType.FreeDecal:
                    rv = new FreeDecalMessage();
                    break;
                case WorldMessageType.ModelInfo:
                    rv = new ModelInfoMessage();
                    break;
                case WorldMessageType.SoundControl:
                    rv = new SoundControlMessage();
                    break;
                case WorldMessageType.DirLocOrient:
                    rv = new DirLocOrientMessage();
                    break;
                case WorldMessageType.AuthorizedLogin:
                    rv = new AuthorizedLoginMessage();
                    break;
                case WorldMessageType.AuthorizedLoginResponse:
                    rv = new AuthorizedLoginResponseMessage();
                    break;
                case WorldMessageType.LoadingState:
                    rv = new LoadingStateMessage();
                    break;
                case WorldMessageType.WorldFile:
                    rv = new WorldFileMessage();
                    break;
                case WorldMessageType.IslandManifest:
                    rv = new WorldFileMessage();
                    break;
                default:
                    AtavismLogger.LogError("Unhandled world message type: " + messageType);
                    return null;
            }
            rv.ParseWorldMessage(oid, inMessage);
            return rv;
        }
    }

    public abstract class BaseMessage
    {
        //protected static log4net.ILog log = log4net.LogManager.GetLogger(typeof(BaseMessage));

        #region Methods
        protected virtual void ParseMessage(AtavismIncomingMessage inMessage)
        {
            throw new NotImplementedException();
        }
        protected virtual void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            throw new NotImplementedException();
        }
        public abstract AtavismOutgoingMessage CreateMessage();
        #endregion
    }

    #region Master Tcp Server Messages
    public abstract class BaseMasterTcpMessage : BaseMessage
    {
        protected MasterTcpMessageType messageType;
        public void ParseMasterTcpMessage(AtavismIncomingMessage inMessage)
        {
            ParseMessage(inMessage);
        }
        public override AtavismOutgoingMessage CreateMessage()
        {
            AtavismOutgoingMessage outMessage = new AtavismOutgoingMessage();
            WriteMessage(outMessage);
            return outMessage;
        }

        public MasterTcpMessageType MessageType
        {
            get
            {
                return messageType;
            }
            set
            {
                messageType = value;
            }
        }
    }

    public class MasterTcpLoginMessage : BaseMasterTcpMessage
    {
        string username;
        string password;
        bool createAccount;
        string emailAddress;
        Dictionary<string, object> props;
        uint magicCookie;
        int version;

        public MasterTcpLoginMessage()
        {
            this.MessageType = MasterTcpMessageType.Login;
        }

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(magicCookie);
            outMessage.Write(version);
            outMessage.Write(username);
            outMessage.Write(password);

            // Convert props into a string
            string propString = "";
            foreach (string prop in props.Keys)
            {
                propString += Convert.ToBase64String(Encoding.UTF8.GetBytes(prop)) + "-" + Convert.ToBase64String(Encoding.UTF8.GetBytes(props[prop].ToString())) + "|";
            }
            outMessage.Write(propString);

            // Account Registration Properties
            outMessage.Write(createAccount);
            if (createAccount)
            {
                outMessage.Write(emailAddress);
            }
        }
        #endregion

        #region Properties
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
            }
        }
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
            }
        }
        public bool CreateAccount
        {
            get
            {
                return createAccount;
            }
            set
            {
                createAccount = value;
            }
        }
        public string EmailAddress
        {
            get
            {
                return emailAddress;
            }
            set
            {
                emailAddress = value;
            }
        }
        public Dictionary<string, object> Props
        {
            get
            {
                return props;
            }
            set
            {
                props = value;
            }
        }
        public uint MagicCookie
        {
            get
            {
                return magicCookie;
            }
            set
            {
                magicCookie = value;
            }
        }
        public int Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
        }
        #endregion
    }

    public class MasterTcpLoginChallengeMessage : BaseMasterTcpMessage
    {
        int authProtocolVersion;
        int challengeLength;
        byte[] challenge;

        public MasterTcpLoginChallengeMessage()
        {
            this.MessageType = MasterTcpMessageType.Login;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            authProtocolVersion = inMessage.ReadInt32();
            challengeLength = inMessage.ReadInt32();
            challenge = inMessage.ReadBytes();
        }
        #endregion

        #region Properties
        public int AuthProtocolVersion
        {
            get
            {
                return authProtocolVersion;
            }
            set
            {
                authProtocolVersion = value;
            }
        }
        public int ChallengeLength
        {
            get
            {
                return challengeLength;
            }
            set
            {
                challengeLength = value;
            }
        }
        public byte[] Challenge
        {
            get
            {
                return challenge;
            }
            set
            {
                challenge = value;
            }
        }
        #endregion
    }

    public class MasterTcpLoginChallengeResponseMessage : BaseMasterTcpMessage
    {
        byte[] response;

        public MasterTcpLoginChallengeResponseMessage()
        {
            this.MessageType = MasterTcpMessageType.Login;
        }

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(response);
        }
        #endregion

        #region Properties
        public byte[] Response
        {
            get
            {
                return response;
            }
            set
            {
                response = value;
            }
        }

        #endregion
    }

    public class MasterTcpLoginResponseMessage : BaseMasterTcpMessage
    {
        LoginStatus loginStatus;
        byte[] authToken;

        public MasterTcpLoginResponseMessage()
        {
            this.MessageType = MasterTcpMessageType.LoginResponse;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            loginStatus = (LoginStatus)inMessage.ReadInt32();
            authToken = inMessage.ReadBytes();
        }
        #endregion

        #region Properties
        public LoginStatus LoginStatus
        {
            get
            {
                return loginStatus;
            }
            set
            {
                loginStatus = value;
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
        #endregion
    }

    public class MasterTcpCreateAccountResponseMessage : BaseMasterTcpMessage
    {
        LoginStatus loginStatus;

        public MasterTcpCreateAccountResponseMessage()
        {
            this.MessageType = MasterTcpMessageType.LoginResponse;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            loginStatus = (LoginStatus)inMessage.ReadInt32();
        }
        #endregion

        #region Properties
        public LoginStatus LoginStatus
        {
            get
            {
                return loginStatus;
            }
            set
            {
                loginStatus = value;
            }
        }
        #endregion
    }
    #endregion

    #region World Tcp Server Messages (a.k.a. Login Server)
    public abstract class BaseWorldTcpMessage : BaseMessage
    {
        protected WorldTcpMessageType messageType;
        public void ParseWorldTcpMessage(AtavismIncomingMessage inMessage)
        {
            ParseMessage(inMessage);
        }
        public override AtavismOutgoingMessage CreateMessage()
        {
            AtavismOutgoingMessage outMessage = new AtavismOutgoingMessage();
            outMessage.Write(messageType);
            WriteMessage(outMessage);
            AtavismOutgoingMessage rv = new AtavismOutgoingMessage();
            rv.Write(outMessage.GetBytes());
            return rv;
        }

        public WorldTcpMessageType MessageType
        {
            get
            {
                return messageType;
            }
            set
            {
                messageType = value;
            }
        }
    }

    public class WorldCharacterRequestMessage : BaseWorldTcpMessage
    {
        string version = string.Empty;
        byte[] authToken;

        public WorldCharacterRequestMessage()
        {
            //this.MessageType = WorldTcpMessageType.CharacterRequest;
            this.MessageType = WorldTcpMessageType.SecureCharacterRequest;
        }

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(version);
            outMessage.Write(authToken);
        }
        #endregion

        #region Properties
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
        public string Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
        }
        #endregion
    }

    public class WorldCharacterResponseMessage : BaseWorldTcpMessage
    {
        string version;
        byte[] worldToken;
        string error;
        string worldFilesUrl;
        long account;
        int characterSlots;
        List<PropertyMap> entries = new List<PropertyMap>();

        public WorldCharacterResponseMessage()
        {
            this.MessageType = WorldTcpMessageType.CharacterResponse;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            version = inMessage.ReadString();
            worldToken = inMessage.ReadBytes();
            error = inMessage.ReadString();
            worldFilesUrl = inMessage.ReadString();
            account = inMessage.ReadOID().ToLong();
            characterSlots = inMessage.ReadInt32();
            int entryCount = inMessage.ReadInt32();
            entries = new List<PropertyMap>();
            for (int i = 0; i < entryCount; ++i)
            {
                PropertyMap entry = new PropertyMap();
                entry.OldParseMessage(inMessage);
                entries.Add(entry);
            }
        }

        #endregion

        #region Properties
        public byte[] WorldToken
        {
            get
            {
                return worldToken;
            }
            set
            {
                worldToken = value;
            }
        }
        public string Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
        }
        public string Error
        {
            get
            {
                return error;
            }
            set
            {
                error = value;
            }
        }
        public string WorldFilesUrl
        {
            get
            {
                return worldFilesUrl;
            }
            set
            {
                worldFilesUrl = value;
            }
        }
        public long Account
        {
            get
            {
                return account;
            }
            set
            {
                account = value;
            }
        }
        public int CharacterSlots
        {
            get
            {
                return characterSlots;
            }
            set
            {
                characterSlots = value;
            }
        }
        public List<CharacterEntry> CharacterEntries
        {
            get
            {
                List<CharacterEntry> rv = new List<CharacterEntry>();
                foreach (PropertyMap entry in entries)
                    rv.Add(new CharacterEntry(entry.Properties));
                return rv;
            }
        }
        #endregion
    }

    public class WorldCharacterDeleteMessage : BaseWorldTcpMessage
    {
        PropertyMap propertyMap = new PropertyMap();

        public WorldCharacterDeleteMessage()
        {
            this.MessageType = WorldTcpMessageType.CharacterDeleteRequest;
        }

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            propertyMap.OldWriteMessage(outMessage);
        }
        #endregion

        #region Properties
        public Dictionary<string, object> Properties
        {
            get
            {
                return propertyMap.Properties;
            }
            set
            {
                propertyMap.Properties.Clear();
                foreach (string key in value.Keys)
                    propertyMap.Properties[key] = value[key];
            }
        }
        #endregion
    }


    public class WorldCharacterDeleteResponseMessage : BaseWorldTcpMessage
    {
        PropertyMap propertyMap = new PropertyMap();

        public WorldCharacterDeleteResponseMessage()
        {
            this.MessageType = WorldTcpMessageType.CharacterDeleteResponse;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            propertyMap = new PropertyMap();
            propertyMap.OldParseMessage(inMessage);
        }
        #endregion

        #region Properties
        public Dictionary<string, object> Properties
        {
            get
            {
                return propertyMap.Properties;
            }
        }
        #endregion
    }

    public class WorldCharacterCreateMessage : BaseWorldTcpMessage
    {
        PropertyMap propertyMap = new PropertyMap();

        public WorldCharacterCreateMessage()
        {
            this.MessageType = WorldTcpMessageType.CharacterCreateRequest;
        }

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            propertyMap.OldWriteMessage(outMessage);
        }
        #endregion

        #region Properties
        public Dictionary<string, object> Properties
        {
            get
            {
                return propertyMap.Properties;
            }
            set
            {
                propertyMap.Properties.Clear();
                foreach (string key in value.Keys)
                    propertyMap.Properties[key] = value[key];
            }
        }
        #endregion
    }

    public class WorldCharacterCreateResponseMessage : BaseWorldTcpMessage
    {
        PropertyMap propertyMap = new PropertyMap();

        public WorldCharacterCreateResponseMessage()
        {
            this.MessageType = WorldTcpMessageType.CharacterCreateResponse;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            propertyMap = new PropertyMap();
            propertyMap.OldParseMessage(inMessage);
        }
        #endregion

        #region Properties
        public Dictionary<string, object> Properties
        {
            get
            {
                return propertyMap.Properties;
            }
        }
        #endregion
    }

    public class WorldCharacterSelectRequestMessage : BaseWorldTcpMessage
    {
        PropertyMap propertyMap = new PropertyMap();

        public WorldCharacterSelectRequestMessage()
        {
            this.MessageType = WorldTcpMessageType.CharacterSelectRequest;
        }

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            propertyMap.OldWriteMessage(outMessage);
        }
        #endregion

        #region Properties
        public Dictionary<string, object> Properties
        {
            get
            {
                return propertyMap.Properties;
            }
            set
            {
                propertyMap.Properties.Clear();
                foreach (string key in value.Keys)
                    propertyMap.Properties[key] = value[key];
            }
        }
        #endregion
    }

    public class WorldCharacterSelectResponseMessage : BaseWorldTcpMessage
    {
        PropertyMap propertyMap = new PropertyMap();

        public WorldCharacterSelectResponseMessage()
        {
            this.MessageType = WorldTcpMessageType.CharacterSelectResponse;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            propertyMap = new PropertyMap();
            propertyMap.ParseMessage(inMessage);
        }
        #endregion

        #region Properties
        public Dictionary<string, object> Properties
        {
            get
            {
                return propertyMap.Properties;
            }
            set
            {
                propertyMap.Properties.Clear();
                foreach (string key in value.Keys)
                    propertyMap.Properties[key] = value[key];
            }
        }
        #endregion
    }

    public class WorldServerListRequestMessage : BaseWorldTcpMessage
    {
        string version = string.Empty;
        byte[] authToken;

        public WorldServerListRequestMessage()
        {
            this.MessageType = WorldTcpMessageType.ServerListRequest;
        }

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(version);
            outMessage.Write(authToken);
        }
        #endregion

        #region Properties
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
        public string Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
        }
        #endregion
    }

    public class WorldServerListResponseMessage : BaseWorldTcpMessage
    {
        string version;
        byte[] worldToken;
        string error;
        List<PropertyMap> entries = new List<PropertyMap>();

        public WorldServerListResponseMessage()
        {
            this.MessageType = WorldTcpMessageType.ServerListResponse;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            version = inMessage.ReadString();
            worldToken = inMessage.ReadBytes();
            error = inMessage.ReadString();
            int entryCount = inMessage.ReadInt32();
            entries = new List<PropertyMap>();
            for (int i = 0; i < entryCount; ++i)
            {
                PropertyMap entry = new PropertyMap();
                entry.OldParseMessage(inMessage);
                entries.Add(entry);
            }
        }

        #endregion

        #region Properties
        public byte[] WorldToken
        {
            get
            {
                return worldToken;
            }
            set
            {
                worldToken = value;
            }
        }
        public string Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
        }
        public string Error
        {
            get
            {
                return error;
            }
            set
            {
                error = value;
            }
        }
        public List<WorldServerEntry> ServerEntries
        {
            get
            {
                List<WorldServerEntry> rv = new List<WorldServerEntry>();
                foreach (PropertyMap entry in entries)
                    rv.Add(new WorldServerEntry(entry.Properties));
                return rv;
            }
        }
        #endregion
    }

    public class WorldServerLoginHeartbeatMessage : BaseWorldTcpMessage
    {
        string version = string.Empty;
        byte[] authToken;

        public WorldServerLoginHeartbeatMessage()
        {
            this.MessageType = WorldTcpMessageType.Heartbeat;
        }

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(version);
            outMessage.Write(authToken);
        }
        #endregion

        #region Properties
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
        public string Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
        }
        #endregion
    }

    public class WorldServerLoginHeartbeatResponseMessage : BaseWorldTcpMessage
    {
        string version;

        public WorldServerLoginHeartbeatResponseMessage()
        {
            this.MessageType = WorldTcpMessageType.HeartbeatResponse;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            version = inMessage.ReadString();
        }

        #endregion

        #region Properties
        public string Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
        }
        #endregion
    }

    #endregion

    #region Master Rdp Server Messages

    public abstract class BaseMasterMessage : BaseMessage
    {
        #region Fields

        /// <summary>
        ///   What type of message this is.
        /// </summary>
        protected MasterMessageType messageType;

        #endregion

        #region Methods
        public override AtavismOutgoingMessage CreateMessage()
        {
            AtavismOutgoingMessage outMessage = new AtavismOutgoingMessage();
            outMessage.Write(messageType);
            WriteMessage(outMessage);
            return outMessage;
        }
        public void ParseMasterMessage(AtavismIncomingMessage inMessage)
        {
            ParseMessage(inMessage);
        }
        #endregion

        #region Properties

        public MasterMessageType MessageType
        {
            get
            {
                return messageType;
            }
            set
            {
                messageType = value;
            }
        }

        #endregion
    }

    public class ResolveMessage : BaseMasterMessage
    {
        string worldName;
        string userName;

        public ResolveMessage()
        {
            this.MessageType = MasterMessageType.Resolve;
        }

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(worldName);
            outMessage.Write(userName);
        }
        #endregion

        #region Properties
        public string WorldName
        {
            get
            {
                return worldName;
            }
            set
            {
                worldName = value;
            }
        }
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
            }
        }
        #endregion
    }

    public class ResolveResponseMessage : BaseMasterMessage
    {
        string worldName;
        bool status;
        string hostname;
        int port;
        // Added in later versions
        string patcherUrl;
        string updateUrl;


        public ResolveResponseMessage()
        {
            this.MessageType = MasterMessageType.ResolveResponse;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            worldName = inMessage.ReadString();
            status = inMessage.ReadBool();
            hostname = inMessage.ReadString();
            port = inMessage.ReadInt32();
            //UnityEngine.Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! PatcherUrl ");

            try
            {
                patcherUrl = inMessage.ReadString();
               // UnityEngine.Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! PatcherUrl 1");
                updateUrl = inMessage.ReadString();
              //  UnityEngine.Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! PatcherUrl 2");

            }
            catch (System.IO.EndOfStreamException)
            {
              //  UnityEngine.Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! PatcherUrl exception");
                //log.Warn("Got old style world resolve response");
                // ignore this - it means we got an old style response
            }
        }
        #endregion

        #region Properties
        public string WorldName
        {
            get
            {
                return worldName;
            }
            set
            {
                worldName = value;
            }
        }
        public bool Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }
        public string Hostname
        {
            get
            {
                return hostname;
            }
            set
            {
                hostname = value;
            }
        }
        public int Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
            }
        }
        public string PatcherUrl
        {
            get
            {
                return patcherUrl;
            }
            set
            {
                patcherUrl = value;
            }
        }
        public string UpdateUrl
        {
            get
            {
                return updateUrl;
            }
            set
            {
                updateUrl = value;
            }
        }

        #endregion
    }

    #endregion

    #region World Proxy Rdp Server Messages

    public abstract class BaseWorldMessage : BaseMessage
    {
        #region Fields

        /// <summary>
        ///   Some messages (such as model info) require additional work
        ///   to be done before we can continue.
        /// </summary>
        protected bool delayHandling = false;
        /// <summary>
        ///   Some messages (such as the portal message) cause us to abort 
        ///   processing messages from the queue.
        /// </summary>
        protected bool abortHandling = false;

        /// <summary>
        ///   When the message is received (prior to handling), 
        ///   write the tick count here.  This is only used for incoming 
        ///   messages.
        /// </summary>
        protected long recvTickCount;

        /// <summary>
        ///   Object id to which the message applies.  
        ///   This is the 'subject' of the message.
        /// </summary>
        protected long oid;

        /// <summary>
        ///   What type of message this is.
        /// </summary>
        protected WorldMessageType messageType;

        #endregion

        #region Methods
        public override AtavismOutgoingMessage CreateMessage()
        {
            AtavismOutgoingMessage outMessage = new AtavismOutgoingMessage();
            outMessage.Write(oid);
            outMessage.Write(messageType);
            WriteMessage(outMessage);
            return outMessage;
        }

        public void ParseWorldMessage(long oid, AtavismIncomingMessage inMessage)
        {
            this.recvTickCount = AtavismTimeTool.CurrentTime;
            this.oid = oid;
            ParseMessage(inMessage);
        }
        #endregion

        #region Properties
        public long ReceivedTickCount
        {
            get
            {
                return recvTickCount;
            }
        }

        public long Oid
        {
            get
            {
                return oid;
            }
            set
            {
                oid = value;
            }
        }

        public WorldMessageType MessageType
        {
            get
            {
                return messageType;
            }
            set
            {
                messageType = value;
            }
        }

        public bool DelayHandling
        {
            get
            {
                return delayHandling;
            }
            set
            {
                delayHandling = value;
            }
        }
        public bool AbortHandling
        {
            get
            {
                return abortHandling;
            }
            set
            {
                abortHandling = value;
            }
        }
        #endregion
    }

    public abstract class TimestampedMessage : BaseWorldMessage
    {
        protected long timestamp;

        public TimestampedMessage()
        {
            timestamp = 0;
        }

        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(timestamp);
        }
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            timestamp = inMessage.ReadTimestamp();
        }
        public long Timestamp
        {
            get
            {
                return timestamp;
            }
            set
            {
                timestamp = value;
            }
        }
    }

    public class CommMessage : BaseWorldMessage
    {
        string senderName;
        int channelId;
        string commMessage;

        public CommMessage()
        {
            this.MessageType = WorldMessageType.Comm;
        }

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(senderName);
            outMessage.Write(channelId);
            outMessage.Write(commMessage);
            //Logger.LogDebugMessage("Writing CommMessage: " + commMessage);
        }
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            senderName = inMessage.ReadString();
            channelId = inMessage.ReadInt32();
            commMessage = inMessage.ReadString();
        }
        #endregion

        #region Properties
        public string SenderName
        {
            get
            {
                return senderName;
            }
            set
            {
                senderName = value;
            }
        }
        public int ChannelId
        {
            get
            {
                return channelId;
            }
            set
            {
                channelId = value;
            }
        }
        public string Message
        {
            get
            {
                return commMessage;
            }
            set
            {
                commMessage = value;
            }
        }
        #endregion
    }

    public class DirectionMessage : TimestampedMessage
    {
        Vector3 direction;
        Vector3 location;

        #region Methods
        public DirectionMessage()
        {
            this.MessageType = WorldMessageType.Direction;
        }

        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
            outMessage.Write(direction);
            outMessage.Write(location);
        }
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            direction = inMessage.ReadVector();
            //Logger.LogDebugMessage("Got DirLoc: " + direction);
            location = inMessage.ReadVector();
        }
        #endregion

        #region Properties	
        public Vector3 Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
            }
        }
        public Vector3 Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
            }
        }
        #endregion
    }

    public class DirLocOrientMessage : DirectionMessage
    {
        Quaternion orientation;
        private long updateId = 0; 

        #region Methods
        public DirLocOrientMessage()
        {
            this.MessageType = WorldMessageType.DirLocOrient;
        }

        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
            outMessage.Write(orientation);
            outMessage.Write(updateId);
        }
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            orientation = inMessage.ReadQuaternion();
        }
        #endregion

        #region Properties	

        public Quaternion Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value;
            }
        }
        public long UpdateId
        {
            get
            {
                return updateId;
            }
            set
            {
                updateId = value;
            }
        }
        #endregion
    }

    public class LoginMessage : BaseWorldMessage
    {
        OID characterId;
        string clientVersion;

        public LoginMessage()
        {
            this.MessageType = WorldMessageType.Login;
        }

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(characterId);
            outMessage.Write(clientVersion);
        }

        #endregion

        #region Properties
        public OID CharacterId
        {
            set
            {
                characterId = value;
            }
        }
        public string ClientVersion
        {
            set
            {
                clientVersion = value;
            }
        }
        #endregion
    }

    public class LoginResponseMessage : TimestampedMessage
    {
        bool success;
        string message;

        public LoginResponseMessage()
        {
            this.MessageType = WorldMessageType.LoginResponse;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            //log.InfoFormat("LoginResponseMessage.ParseMessage - Oid: {0}", oid);
            success = inMessage.ReadBool();
            message = inMessage.ReadString();
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
            outMessage.Write(success);
            outMessage.Write(message);
        }

        #endregion

        #region Properties
        public bool Success
        {
            get
            {
                return success;
            }
            set
            {
                success = value;
            }
        }
        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }
        #endregion
    }

    public class LogoutMessage : BaseWorldMessage
    {
        bool success;
        bool logoutToCharacterSelection;
        byte[] authToken;

        public LogoutMessage()
        {
            this.MessageType = WorldMessageType.Logout;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            success = inMessage.ReadBool();
            logoutToCharacterSelection = inMessage.ReadBool();
            if (success && logoutToCharacterSelection)
            {
                int authTokenSize = inMessage.ReadInt32();
                authToken = inMessage.ReadBytes();
            }
        }

        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            // nothing to write 
            // just override so we don't throw not implemented
            outMessage.Write(true);
            outMessage.Write("test");
        }
        #endregion

        #region Properties
        public bool Success
        {
            get
            {
                return success;
            }
        }

        public bool LogoutToCharacterSelection
        {
            get
            {
                return logoutToCharacterSelection;
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
        #endregion
    }

    public class TerrainConfigMessage : BaseWorldMessage
    {
        string configKind;      // either "file" or "xmlstring"
        string configString;
        public TerrainConfigMessage()
        {
            this.MessageType = WorldMessageType.TerrainConfig;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            configKind = inMessage.ReadString();
            configString = inMessage.ReadString();
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(configKind);
            outMessage.Write(configString);
        }
        #endregion

        #region Properties
        public string ConfigString
        {
            get
            {
                return configString;
            }
            set
            {
                configString = value;
            }
        }
        public string ConfigKind
        {
            get
            {
                return configKind;
            }
            set
            {
                configKind = value;
            }
        }
        #endregion
    }

    public class SkyboxMaterialMessage : BaseWorldMessage
    {
        string material;
        public SkyboxMaterialMessage()
        {
            this.MessageType = WorldMessageType.SkyboxMaterial;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            material = inMessage.ReadString();
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(material);
        }
        #endregion

        #region Properties
        public string Material
        {
            get
            {
                return material;
            }
            set
            {
                material = value;
            }
        }
        #endregion
    }

    public class NewObjectMessage : TargetMessage
    {
        string name;
        ObjectNodeType objectType;
        bool followTerrain;

        Vector3 location;
        // Provide default values for direction and lastInterp, in
        // case we're receiving an old-style NewObjectMessage.
        Vector3 direction = Vector3.zero;
        long lastInterp = 0L;
        Quaternion orientation;
        Vector3 scale;

        public NewObjectMessage()
        {
            this.MessageType = WorldMessageType.NewObject;
        }

        #region Methods

        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            name = inMessage.ReadString();
            location = inMessage.ReadVector();
            orientation = inMessage.ReadQuaternion();
            scale = inMessage.ReadVector();
            objectType = (ObjectNodeType)inMessage.ReadInt32();
            followTerrain = inMessage.ReadBool();
           // UnityEngine.Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! NewObjectMessage ");

            try
            {
                direction = inMessage.ReadVector();
         //       UnityEngine.Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! NewObjectMessage 1 "+ direction);
                lastInterp = inMessage.ReadTimestamp();
          //      UnityEngine.Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! NewObjectMessage 2 "+ lastInterp);
            }
            catch (System.IO.EndOfStreamException)
            {
         //       UnityEngine.Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! NewObjectMessage Exception");

                // ignore this - it means we got an old style response
            }
        }


        /// <summary>
        ///   This method is used by the loopback network helper for debugging.
        /// </summary>
        /// <param name="outMessage"></param>
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
            outMessage.Write(name);
            outMessage.Write(location);
            outMessage.Write(orientation);
            outMessage.Write(scale);
            outMessage.Write((int)objectType);
            outMessage.Write(followTerrain);
            outMessage.Write(direction);
            outMessage.Write(lastInterp);
        }


        #endregion

        #region Properties

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public Vector3 Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
            }
        }

        public Vector3 Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
            }
        }

        public long LastInterp
        {
            get
            {
                return lastInterp;
            }
            set
            {
                lastInterp = value;
            }
        }

        public Quaternion Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value;
            }
        }

        public Vector3 ScaleFactor
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }
        }


        public ObjectNodeType ObjectType
        {
            get
            {
                return objectType;
            }
            set
            {
                objectType = value;
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

        #endregion
    }

    public class OrientationMessage : BaseWorldMessage
    {
        Quaternion orientation;

        public OrientationMessage()
        {
            base.MessageType = WorldMessageType.Orientation;
        }

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(orientation);
        }
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            orientation = inMessage.ReadQuaternion();
        }
        #endregion

        #region Properties
        public Quaternion Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value;
            }
        }
        #endregion
    }

    public class HaMessage : TimestampedMessage
    {
      
        public HaMessage()
        {
            base.MessageType = WorldMessageType.Ha;
        }

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
        }
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
        }
        #endregion

        #region Properties

        #endregion
    }

    public class FreeObjectMessage : TargetMessage
    {
        public FreeObjectMessage()
        {
            this.MessageType = WorldMessageType.FreeObject;
        }

        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
        }
    }

    /// <summary>
    ///   A message with a target object
    /// </summary>
    public abstract class TargetMessage : BaseWorldMessage
    {
        protected long objectId;

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(objectId);
        }
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            objectId = inMessage.ReadInt64();
        }
        #endregion

        #region Properties
        public long ObjectId
        {
            get
            {
                return objectId;
            }
            set
            {
                objectId = value;
            }
        }
        #endregion

    }

    public class AcquireMessage : TargetMessage
    {
        public AcquireMessage()
        {
            this.MessageType = WorldMessageType.Acquire;
        }
    }

    public abstract class ItemStatusMessage : TargetMessage
    {
        protected bool status;

        #region Properties
        public bool Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }
        #endregion
    }

    public abstract class ItemEquipStatusMessage : ItemStatusMessage
    {
        protected string slotName;

        #region Properties
        public string SlotName
        {
            get
            {
                return slotName;
            }
            set
            {
                slotName = value;
            }
        }
        #endregion
    }

    public class AcquireResponseMessage : ItemStatusMessage
    {
        public AcquireResponseMessage()
        {
            this.MessageType = WorldMessageType.AcquireResponse;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            status = inMessage.ReadBool();
        }
        #endregion
    }

    public class EquipMessage : TargetMessage
    {
        protected string slotName;

        public EquipMessage()
        {
            this.MessageType = WorldMessageType.Equip;
        }

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
            outMessage.Write(slotName);
        }
        #endregion

        #region Properties
        public string SlotName
        {
            get
            {
                return slotName;
            }
            set
            {
                slotName = value;
            }
        }
        #endregion
    }

    public class EquipResponseMessage : ItemEquipStatusMessage
    {
        public EquipResponseMessage()
        {
            this.MessageType = WorldMessageType.EquipResponse;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            slotName = inMessage.ReadString();
            status = inMessage.ReadBool();
        }
        #endregion
    }

    public class UnequipResponseMessage : EquipResponseMessage
    {
        public UnequipResponseMessage()
        {
            this.MessageType = WorldMessageType.UnequipResponse;
        }
    }

    public class AttachMessage : TargetMessage
    {
        protected string slotName;
        string meshFile;
        List<string> submeshNames;
        List<string> materialNames;


        public AttachMessage()
        {
            this.MessageType = WorldMessageType.Attach;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            slotName = inMessage.ReadString();
            meshFile = inMessage.ReadString();
            int submeshCount = inMessage.ReadInt32();
            if (submeshCount > 0)
            {
                submeshNames = new List<string>();
                materialNames = new List<string>();
            }
            for (int i = 0; i < submeshCount; ++i)
            {
                string submeshName = inMessage.ReadString();
                string material = inMessage.ReadString();
                submeshNames.Add(submeshName);
                // If the material string is empty, replace it with null
                // so that the later code will default the material.
                if (material.Length == 0)
                    material = null;
                materialNames.Add(material);
            }
        }
        #endregion

        #region Properties
        public string SlotName
        {
            get
            {
                return slotName;
            }
            set
            {
                slotName = value;
            }
        }

        public string MeshFile
        {
            get
            {
                return meshFile;
            }
            set
            {
                meshFile = value;
            }
        }

        public List<string> SubmeshNames
        {
            get
            {
                return submeshNames;
            }
            set
            {
                submeshNames = value;
            }
        }

        public List<string> MaterialNames
        {
            get
            {
                return materialNames;
            }
            set
            {
                materialNames = value;
            }
        }
        #endregion
    }

    public class DetachMessage : TargetMessage
    {
        protected string slotName;

        public DetachMessage()
        {
            this.MessageType = WorldMessageType.Detach;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            slotName = inMessage.ReadString();
        }
        #endregion

        #region Properties
        public string SlotName
        {
            get
            {
                return slotName;
            }
            set
            {
                slotName = value;
            }
        }
        #endregion
    }

    public class CombatMessage : TargetMessage
    {
        protected string attackType;
        protected bool attackStatus;

        public CombatMessage()
        {
            this.MessageType = WorldMessageType.Combat;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            attackType = inMessage.ReadString();
            attackStatus = inMessage.ReadBool();
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
            outMessage.Write(attackType);
            outMessage.Write(attackStatus);
        }
        #endregion

        #region Properties
        public string AttackType
        {
            get
            {
                return attackType;
            }
            set
            {
                attackType = value;
            }
        }
        public bool AttackStatus
        {
            get
            {
                return attackStatus;
            }
            set
            {
                attackStatus = value;
            }
        }
        #endregion
    }

    public class AutoAttackMessage : CombatMessage
    {
        public AutoAttackMessage()
        {
            this.MessageType = WorldMessageType.AutoAttack;
        }
    }

    public class StatUpdateMessage : BaseWorldMessage
    {
        Dictionary<string, int> statValues;

        public StatUpdateMessage()
        {
            this.MessageType = WorldMessageType.StatUpdate;
            statValues = new Dictionary<string, int>();
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            statValues.Clear();
            int numElements = inMessage.ReadInt32();
            for (int i = 0; i < numElements; ++i)
            {
                string statName = inMessage.ReadString();
                int statValue = inMessage.ReadInt32();
                statValues[statName] = statValue;
            }
        }
        #endregion

        #region Properties
        public Dictionary<string, int> StatValues
        {
            get
            {
                return statValues;
            }
            set
            {
                statValues = value;
            }
        }
        #endregion
    }

    public class DamageMessage : TargetMessage
    {
        protected string damageType;
        protected int damageAmount;

        public DamageMessage()
        {
            this.MessageType = WorldMessageType.Damage;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            damageType = inMessage.ReadString();
            damageAmount = inMessage.ReadInt32();
        }
        #endregion

        #region Properties
        public string DamageType
        {
            get
            {
                return damageType;
            }
            set
            {
                damageType = value;
            }
        }
        public int DamageAmount
        {
            get
            {
                return damageAmount;
            }
            set
            {
                damageAmount = value;
            }
        }
        #endregion
    }

    public class AnimationMessage : BaseWorldMessage
    {
        bool clear = false;

        List<AnimationEntry> animations = new List<AnimationEntry>();

        public AnimationMessage()
        {
            this.MessageType = WorldMessageType.Animation;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            int numEntries = inMessage.ReadInt32();
            for (int i = 0; i < numEntries; ++i)
            {
                string type = inMessage.ReadString();
                switch (type)
                {
                    case "add":
                        AnimationEntry entry = new AnimationEntry();
                        entry.animationName = inMessage.ReadString();
                        entry.animationSpeed = 1.0f;
                        entry.loop = inMessage.ReadBool();
                        if (animations.Count > 0)
                        {
                            AnimationEntry last = animations[animations.Count - 1];
                            if (last.loop)
                                animations.RemoveAt(animations.Count - 1);
                        }
                        animations.Add(entry);
                        break;
                    case "clear":
                        animations.Clear();
                        clear = true;
                        break;
                    default:
                        //log.WarnFormat("Invalid animation message type: {0}", type);
                        break;
                }
            }
        }
        #endregion

        #region Properties
        public bool Clear
        {
            get
            {
                return clear;
            }
        }
        public List<AnimationEntry> Animations
        {
            get
            {
                return animations;
            }
        }
        #endregion
    }

    public class SoundMessage : BaseWorldMessage
    {
        bool clear = false;

        List<SoundEntry> sounds = new List<SoundEntry>();

        public SoundMessage()
        {
            this.MessageType = WorldMessageType.Sound;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            int numEntries = inMessage.ReadInt32();
            for (int i = 0; i < numEntries; ++i)
            {
                string type = inMessage.ReadString();
                switch (type)
                {
                    case "add":
                        SoundEntry entry = new SoundEntry();
                        entry.soundName = inMessage.ReadString();
                        if (entry.soundName == "idle")
                        {
                            sounds.Clear();
                            clear = true;
                            break;
                        }
                        else if (entry.soundName == "strike")
                        {
                            entry.soundName = "swing.wav";
                        }
                        else if (entry.soundName == "run")
                        {
                            entry.soundName = "gravelwalk.wav";
                        }
                        else if (entry.soundName == "death")
                        {
                            entry.soundName = "ugh.wav";
                        }

                        entry.soundSpeed = 1.0f;
                        entry.soundGain = 1.0f;
                        entry.loop = inMessage.ReadBool();
                        //log.InfoFormat("Playing Sound {0}, looping = {1}",
                        //               entry.soundName, entry.loop);
                        if (sounds.Count > 0)
                        {
                            SoundEntry last = sounds[sounds.Count - 1];
                            if (last.loop)
                                sounds.RemoveAt(sounds.Count - 1);
                        }
                        sounds.Add(entry);
                        break;
                    case "clear":
                        sounds.Clear();
                        clear = true;
                        break;
                    default:
                        //log.ErrorFormat("Invalid sound message type: {0}", type);
                        break;
                }
            }
        }
        #endregion

        #region Properties
        public bool Clear
        {
            get
            {
                return clear;
            }
        }
        public List<SoundEntry> Sounds
        {
            get
            {
                return sounds;
            }
        }
        #endregion
    }

    public class AmbientSoundMessage : BaseWorldMessage
    {
        bool active;
        string sound;

        public AmbientSoundMessage()
        {
            this.MessageType = WorldMessageType.AmbientSound;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            active = inMessage.ReadBool();
            sound = inMessage.ReadString();
        }
        #endregion

        #region Properties
        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
            }
        }
        public string Sound
        {
            get
            {
                return sound;
            }
            set
            {
                sound = value;
            }
        }
        #endregion
    }

    public class FollowTerrainMessage : BaseWorldMessage
    {
        bool followTerrain;

        public FollowTerrainMessage()
        {
            this.MessageType = WorldMessageType.FollowTerrain;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            followTerrain = inMessage.ReadBool();
        }
        #endregion

        #region Properties
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
        #endregion
    }

    public class PortalMessage : BaseWorldMessage
    {
        string worldId;
        OID characterId;

        public PortalMessage()
        {
            this.MessageType = WorldMessageType.Portal;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            worldId = inMessage.ReadString();
            characterId = inMessage.ReadOID();
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(worldId);
            outMessage.Write(characterId);
        }
        #endregion

        #region Properties
        public string WorldId
        {
            get
            {
                return worldId;
            }
            set
            {
                worldId = value;
            }
        }
        public OID CharacterId
        {
            get
            {
                return characterId;
            }
            set
            {
                characterId = value;
            }
        }
        #endregion
    }

    public class AmbientLightMessage : BaseWorldMessage
    {
        Color color;
        public AmbientLightMessage()
        {
            this.MessageType = WorldMessageType.AmbientLight;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            color = inMessage.ReadColor();
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(color);
        }
        #endregion

        #region Properties
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }
        #endregion
    }

    public class NewLightMessage : TargetMessage
    {
        string name;
        Vector3 location;
        Quaternion orientation;
        LightNodeType lightType;
        Color diffuse;
        Color specular;
        float attenuationRange;
        float attenuationConstant;
        float attenuationLinear;
        float attenuationQuadratic;
        float spotlightInnerAngle;
        float spotlightOuterAngle;
        float spotlightFalloff;

        public NewLightMessage()
        {
            this.MessageType = WorldMessageType.NewLight;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            lightType = (LightNodeType)inMessage.ReadInt32();
            name = inMessage.ReadString();
            diffuse = inMessage.ReadColor();
            specular = inMessage.ReadColor();
            attenuationRange = inMessage.ReadSingle();
            attenuationConstant = inMessage.ReadSingle();
            attenuationLinear = inMessage.ReadSingle();
            attenuationQuadratic = inMessage.ReadSingle();
            switch (lightType)
            {
                case LightNodeType.Point:
                    location = inMessage.ReadVector();
                    break;
                case LightNodeType.Directional:
                    orientation = inMessage.ReadQuaternion();
                    break;
                case LightNodeType.Spotlight:
                    location = inMessage.ReadVector();
                    orientation = inMessage.ReadQuaternion();
                    spotlightInnerAngle = inMessage.ReadSingle();
                    spotlightOuterAngle = inMessage.ReadSingle();
                    spotlightFalloff = inMessage.ReadSingle();
                    break;
                default:
                    throw new Exception("Invalid light node type: " + lightType);
            }
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
            outMessage.Write((int)lightType);
            outMessage.Write(name);
            outMessage.Write(diffuse);
            outMessage.Write(specular);
            outMessage.Write(attenuationRange);
            outMessage.Write(attenuationConstant);
            outMessage.Write(attenuationLinear);
            outMessage.Write(attenuationQuadratic);
            switch (lightType)
            {
                case LightNodeType.Point:
                    outMessage.Write(location);
                    break;
                case LightNodeType.Directional:
                    outMessage.Write(orientation);
                    break;
                case LightNodeType.Spotlight:
                    outMessage.Write(location);
                    outMessage.Write(orientation);
                    outMessage.Write(spotlightInnerAngle);
                    outMessage.Write(spotlightOuterAngle);
                    outMessage.Write(spotlightFalloff);
                    break;
                default:
                    throw new Exception("Invalid light node type: " + lightType);
            }
        }
        #endregion

        #region Properties

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public Vector3 Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
            }
        }

        public Quaternion Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value;
            }
        }

        public LightNodeType LightType
        {
            get
            {
                return lightType;
            }
            set
            {
                lightType = value;
            }
        }

        public Color Diffuse
        {
            get
            {
                return diffuse;
            }
            set
            {
                diffuse = value;
            }
        }

        public Color Specular
        {
            get
            {
                return specular;
            }
            set
            {
                specular = value;
            }
        }

        public float AttenuationRange
        {
            get
            {
                return attenuationRange;
            }
            set
            {
                attenuationRange = value;
            }
        }

        public float AttenuationConstant
        {
            get
            {
                return attenuationConstant;
            }
            set
            {
                attenuationConstant = value;
            }
        }

        public float AttenuationLinear
        {
            get
            {
                return attenuationLinear;
            }
            set
            {
                attenuationLinear = value;
            }
        }

        public float AttenuationQuadratic
        {
            get
            {
                return attenuationQuadratic;
            }
            set
            {
                attenuationQuadratic = value;
            }
        }

        public float SpotlightInnerAngle
        {
            get
            {
                return spotlightInnerAngle;
            }
            set
            {
                spotlightInnerAngle = value;
            }
        }

        public float SpotlightOuterAngle
        {
            get
            {
                return spotlightOuterAngle;
            }
            set
            {
                spotlightOuterAngle = value;
            }
        }

        public float SpotlightFalloff
        {
            get
            {
                return spotlightFalloff;
            }
            set
            {
                spotlightFalloff = value;
            }
        }

        #endregion
    }

    public class CommandMessage : TargetMessage
    {
        string command;

        public CommandMessage()
        {
            this.MessageType = WorldMessageType.Command;
        }

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
            outMessage.Write(command);
        }
        #endregion

        #region Properties
        public string Command
        {
            get
            {
                return command;
            }
            set
            {
                command = value;
            }
        }
        #endregion
    }

    public class TradeStartRequestMessage : TargetMessage
    {
        public TradeStartRequestMessage()
        {
            this.MessageType = WorldMessageType.TradeStartRequest;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
        }
        #endregion
    }

    public class TradeStartMessage : TargetMessage
    {
        public TradeStartMessage()
        {
            this.MessageType = WorldMessageType.TradeStart;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
        }
        #endregion
    }

    public class TradeOfferRequestMessage : TargetMessage
    {
        bool accepted;
        bool cancelled;
        List<long> offer;

        public TradeOfferRequestMessage()
        {
            this.MessageType = WorldMessageType.TradeOfferRequest;
            offer = new List<long>();
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            accepted = inMessage.ReadBool();
            cancelled = inMessage.ReadBool();
            int numItems = inMessage.ReadInt32();
            for (int i = 0; i < numItems; ++i)
            {
                offer.Add(inMessage.ReadInt64());
            }
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
            outMessage.Write(accepted);
            outMessage.Write(cancelled);
            outMessage.Write(offer.Count);
            foreach (long itemOid in offer)
            {
                outMessage.Write(itemOid);
            }
        }
        #endregion

        #region Properties
        public bool Accepted
        {
            get
            {
                return accepted;
            }
            set
            {
                accepted = value;
            }
        }
        public bool Cancelled
        {
            get
            {
                return cancelled;
            }
            set
            {
                cancelled = value;
            }
        }
        public List<long> Offer
        {
            get
            {
                return offer;
            }
            set
            {
                offer = value;
            }
        }
        #endregion
    }

    public class TradeCompleteMessage : TargetMessage
    {
        int status;

        public TradeCompleteMessage()
        {
            this.MessageType = WorldMessageType.TradeComplete;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            status = inMessage.ReadInt32();
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
            outMessage.Write(status);
        }
        #endregion

        #region Properties
        public int Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }
        #endregion
    }

    public class TradeOfferUpdateMessage : TargetMessage
    {
        List<InvItemInfo> offer1;
        List<InvItemInfo> offer2;
        bool accepted1;
        bool accepted2;

        public TradeOfferUpdateMessage()
        {
            this.MessageType = WorldMessageType.TradeOfferUpdate;
            offer1 = new List<InvItemInfo>();
            offer2 = new List<InvItemInfo>();
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            int numItems = inMessage.ReadInt32();
            for (int i = 0; i < numItems; ++i)
            {
                InvItemInfo info = new InvItemInfo();
                info.itemId = inMessage.ReadInt64();
                info.name = inMessage.ReadString();
                info.icon = inMessage.ReadString();
                offer1.Add(info);
            }
            accepted1 = inMessage.ReadBool();
            numItems = inMessage.ReadInt32();
            for (int i = 0; i < numItems; ++i)
            {
                InvItemInfo info = new InvItemInfo();
                info.itemId = inMessage.ReadInt64();
                info.name = inMessage.ReadString();
                info.icon = inMessage.ReadString();
                offer2.Add(info);
            }
            accepted2 = inMessage.ReadBool();
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
            outMessage.Write(offer1.Count);
            foreach (InvItemInfo item in offer1)
            {
                outMessage.Write(item.itemId);
                outMessage.Write(item.name);
                outMessage.Write(item.icon);
            }
            outMessage.Write(accepted1);
            foreach (InvItemInfo item in offer2)
            {
                outMessage.Write(item.itemId);
                outMessage.Write(item.name);
                outMessage.Write(item.icon);
            }
            outMessage.Write(accepted2);
        }
        #endregion

        #region Properties
        public List<InvItemInfo> Offer1
        {
            get
            {
                return offer1;
            }
            set
            {
                offer1 = value;
            }
        }
        public List<InvItemInfo> Offer2
        {
            get
            {
                return offer2;
            }
            set
            {
                offer2 = value;
            }
        }
        public bool Accepted1
        {
            get
            {
                return accepted1;
            }
            set
            {
                accepted1 = value;
            }
        }
        public bool Accepted2
        {
            get
            {
                return accepted2;
            }
            set
            {
                accepted2 = value;
            }
        }
        #endregion
    }

    public class StateMessage : BaseWorldMessage
    {
        Dictionary<string, int> states;

        public StateMessage()
        {
            this.MessageType = WorldMessageType.StateMessage;
            states = new Dictionary<string, int>();
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            int stateCount = inMessage.ReadInt32();
            while (stateCount > 0)
            {
                string key = inMessage.ReadString();
                int val = inMessage.ReadInt32();
                states[key] = val;
                stateCount--;
            }
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(states.Count);
            foreach (string key in states.Keys)
            {
                outMessage.Write(key);
                outMessage.Write(states[key]);
            }
        }
        #endregion

        #region Properties
        public Dictionary<string, int> States
        {
            get
            {
                return states;
            }
            set
            {
                states = value;
            }
        }
        #endregion
    }

    public class QuestInfoRequestMessage : TargetMessage
    {
        public QuestInfoRequestMessage()
        {
            this.MessageType = WorldMessageType.QuestInfoRequest;
        }
    }

    public class QuestInfoResponseMessage : TargetMessage
    {
        long questId;
        string title;
        string description;
        string objective;
        List<ItemEntry> rewardItems;

        public QuestInfoResponseMessage()
        {
            this.MessageType = WorldMessageType.QuestInfoResponse;
            rewardItems = new List<ItemEntry>();
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            questId = inMessage.ReadInt64();
            title = inMessage.ReadString();
            description = inMessage.ReadString();
            objective = inMessage.ReadString();
            int rewardCount = inMessage.ReadInt32();
            for (int i = 0; i < rewardCount; ++i)
            {
                ItemEntry entry = new ItemEntry();
                entry.name = inMessage.ReadString();
                entry.icon = inMessage.ReadString();
                entry.count = inMessage.ReadInt32();

                rewardItems.Add(entry);
            }
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
            outMessage.Write(questId);
            outMessage.Write(title);
            outMessage.Write(description);
            outMessage.Write(objective);
            outMessage.Write(rewardItems.Count);
            foreach (ItemEntry reward in rewardItems)
            {
                outMessage.Write(reward.name);
                outMessage.Write(reward.icon);
                outMessage.Write(reward.count);
            }
        }
        #endregion

        #region Properties
        public long QuestId
        {
            get
            {
                return questId;
            }
            set
            {
                questId = value;
            }
        }
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }
        public string Objective
        {
            get
            {
                return objective;
            }
            set
            {
                objective = value;
            }
        }
        public List<ItemEntry> RewardItems
        {
            get
            {
                return rewardItems;
            }
            set
            {
                rewardItems = value;
            }
        }
        #endregion
    }

    public class QuestResponseMessage : TargetMessage
    {
        long questId;
        bool accepted;

        public QuestResponseMessage()
        {
            this.MessageType = WorldMessageType.QuestResponse;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            questId = inMessage.ReadInt64();
            accepted = inMessage.ReadBool();
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
            outMessage.Write(questId);
            outMessage.Write(accepted);
        }
        #endregion

        #region Properties
        public long QuestId
        {
            get
            {
                return questId;
            }
            set
            {
                questId = value;
            }
        }
        public bool Accepted
        {
            get
            {
                return accepted;
            }
            set
            {
                accepted = value;
            }
        }
        #endregion
    }

    public class RegionConfigMessage : BaseWorldMessage
    {
        string configString;
        public RegionConfigMessage()
        {
            this.MessageType = WorldMessageType.RegionConfig;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            configString = inMessage.ReadString();
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(configString);
        }
        #endregion

        #region Properties
        public string ConfigString
        {
            get
            {
                return configString;
            }
            set
            {
                configString = value;
            }
        }
        #endregion
    }

    public class InventoryUpdateMessage : BaseWorldMessage
    {
        List<InventoryUpdateEntry> entries;

        public InventoryUpdateMessage()
        {
            this.MessageType = WorldMessageType.InventoryUpdate;
            entries = new List<InventoryUpdateEntry>();
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            int numEntries = inMessage.ReadInt32();
            for (int i = 0; i < numEntries; ++i)
            {
                InventoryUpdateEntry entry = new InventoryUpdateEntry();
                entry.itemId = inMessage.ReadInt64();
                entry.containerId = inMessage.ReadInt32();
                entry.slotId = inMessage.ReadInt32();
                entry.name = inMessage.ReadString();
                entry.icon = inMessage.ReadString();
                entries.Add(entry);
            }
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(entries.Count);
            foreach (InventoryUpdateEntry entry in entries)
            {
                outMessage.Write(entry.itemId);
                outMessage.Write(entry.containerId);
                outMessage.Write(entry.slotId);
                outMessage.Write(entry.name);
                outMessage.Write(entry.icon);
            }
        }
        #endregion

        #region Properties
        public List<InventoryUpdateEntry> Inventory
        {
            get
            {
                return entries;
            }
        }
        #endregion
    }

    public class QuestLogInfoMessage : BaseWorldMessage
    {
        long questId;
        string title;
        string description;
        string objective;
        List<ItemEntry> rewardItems;

        public QuestLogInfoMessage()
        {
            this.MessageType = WorldMessageType.QuestLogInfo;
            rewardItems = new List<ItemEntry>();
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            questId = inMessage.ReadInt64();
            title = inMessage.ReadString();
            description = inMessage.ReadString();
            objective = inMessage.ReadString();
            int rewardCount = inMessage.ReadInt32();
            for (int i = 0; i < rewardCount; ++i)
            {
                ItemEntry entry = new ItemEntry();
                entry.name = inMessage.ReadString();
                entry.icon = inMessage.ReadString();
                entry.count = inMessage.ReadInt32();

                rewardItems.Add(entry);
            }
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(questId);
            outMessage.Write(title);
            outMessage.Write(description);
            outMessage.Write(objective);
            outMessage.Write(rewardItems.Count);
            foreach (ItemEntry reward in rewardItems)
            {
                outMessage.Write(reward.name);
                outMessage.Write(reward.icon);
                outMessage.Write(reward.count);
            }
        }
        #endregion

        #region Properties
        public long QuestId
        {
            get
            {
                return questId;
            }
            set
            {
                questId = value;
            }
        }
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }
        public string Objective
        {
            get
            {
                return objective;
            }
            set
            {
                objective = value;
            }
        }
        public List<ItemEntry> RewardItems
        {
            get
            {
                return rewardItems;
            }
            set
            {
                rewardItems = value;
            }
        }
        #endregion
    }

    public class QuestStateInfoMessage : BaseWorldMessage
    {
        long questId;
        List<string> objectives;

        public QuestStateInfoMessage()
        {
            this.MessageType = WorldMessageType.QuestStateInfo;
            objectives = new List<string>();
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            questId = inMessage.ReadInt64();
            int numEntries = inMessage.ReadInt32();
            for (int i = 0; i < numEntries; ++i)
                objectives.Add(inMessage.ReadString());
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(questId);
            outMessage.Write(objectives.Count);
            foreach (string objective in objectives)
                outMessage.Write(objective);
        }
        #endregion

        #region Properties
        public long QuestId
        {
            get
            {
                return questId;
            }
            set
            {
                questId = value;
            }
        }
        public List<string> Objectives
        {
            get
            {
                return objectives;
            }
            set
            {
                objectives = value;
            }
        }
        #endregion
    }

    public class RemoveQuestRequestMessage : BaseWorldMessage
    {
        long questId;
        List<string> objectives;

        public RemoveQuestRequestMessage()
        {
            this.MessageType = WorldMessageType.RemoveQuestRequest;
            objectives = new List<string>();
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            questId = inMessage.ReadInt64();
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(questId);
        }
        #endregion

        #region Properties
        public long QuestId
        {
            get
            {
                return questId;
            }
            set
            {
                questId = value;
            }
        }
        #endregion
    }

    public class RemoveQuestResponseMessage : BaseWorldMessage
    {
        long questId;

        public RemoveQuestResponseMessage()
        {
            this.MessageType = WorldMessageType.RemoveQuestResponse;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            questId = inMessage.ReadInt64();
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(questId);
        }
        #endregion

        #region Properties
        public long QuestId
        {
            get
            {
                return questId;
            }
            set
            {
                questId = value;
            }
        }
        #endregion
    }

    public class GroupInfoMessage : BaseWorldMessage
    {
        long groupId;
        long leaderId;
        List<GroupInfoEntry> memberList;

        public GroupInfoMessage()
        {
            this.MessageType = WorldMessageType.GroupInfo;
            memberList = new List<GroupInfoEntry>();
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            groupId = inMessage.ReadInt64();
            leaderId = inMessage.ReadInt64();
            int numEntries = inMessage.ReadInt32();
            for (int i = 0; i < numEntries; ++i)
            {
                GroupInfoEntry entry = new GroupInfoEntry();
                entry.memberId = inMessage.ReadInt64();
                entry.memberName = inMessage.ReadString();
                memberList.Add(entry);
            }
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(groupId);
            outMessage.Write(leaderId);
            outMessage.Write(memberList.Count);
            foreach (GroupInfoEntry entry in memberList)
            {
                outMessage.Write(entry.memberId);
                outMessage.Write(entry.memberName);
            }
        }
        #endregion

        #region Properties
        public long GroupId
        {
            get
            {
                return groupId;
            }
            set
            {
                groupId = value;
            }
        }
        public long LeaderId
        {
            get
            {
                return leaderId;
            }
            set
            {
                leaderId = value;
            }
        }
        public List<GroupInfoEntry> GroupInfoEntries
        {
            get
            {
                return memberList;
            }
        }

        #endregion
    }

    public class QuestConcludeRequestMessage : TargetMessage
    {
        public QuestConcludeRequestMessage()
        {
            this.MessageType = WorldMessageType.QuestConcludeRequest;
        }
    }

    public class UiThemeMessage : BaseWorldMessage
    {
        List<string> uiModules;
        string keyBindingsFile;

        public UiThemeMessage()
        {
            this.MessageType = WorldMessageType.UiTheme;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            uiModules = new List<string>();
            int numModules = inMessage.ReadInt32();
            for (int i = 0; i < numModules; ++i)
                uiModules.Add(inMessage.ReadString());
            try
            {
                // introduced in 1.0
                keyBindingsFile = inMessage.ReadString();
            }
            catch (Exception)
            {
            }
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(uiModules.Count);
            foreach (string uiModule in uiModules)
                outMessage.Write(uiModule);
            if (keyBindingsFile != null)
                outMessage.Write(keyBindingsFile);
        }
        #endregion

        #region Properties
        public List<string> UiModules
        {
            get
            {
                return uiModules;
            }
            set
            {
                uiModules = value;
            }
        }
        public string KeyBindingsFile
        {
            get
            {
                return keyBindingsFile;
            }
            set
            {
                keyBindingsFile = value;
            }
        }
        #endregion
    }

    public class LootAllMessage : BaseWorldMessage
    {
        public LootAllMessage()
        {
            this.MessageType = WorldMessageType.LootAll;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
        }
        #endregion

        #region Properties
        #endregion
    }

    public class OldModelInfoMessage : ModelInfoMessage
    {
        public OldModelInfoMessage()
        {
            this.MessageType = WorldMessageType.OldModelInfo;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            int meshCount = inMessage.ReadInt32();
            for (int i = 0; i < meshCount; ++i)
            {
                OldMeshInfo meshInfo = new OldMeshInfo();
                meshInfo.ParseMessage(inMessage);
                modelInfo.Add(meshInfo);
            }
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(modelInfo.Count);
            foreach (MeshInfo meshInfo in modelInfo)
                meshInfo.WriteMessage(outMessage);
        }
        #endregion
    }

    public class FragmentMessage : BaseWorldMessage
    {
        int numFragments;
        int messageNumber;
        int fragmentNumber;
        byte[] payload;

        public FragmentMessage()
        {
            this.MessageType = WorldMessageType.FragmentMessage;
            numFragments = -1;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            messageNumber = inMessage.ReadInt32();
            fragmentNumber = inMessage.ReadInt32();
            if (fragmentNumber == 0)
                numFragments = inMessage.ReadInt32();
            payload = inMessage.ReadBytes();
        }
        #endregion

        #region Properties

        public int NumFragments
        {
            get
            {
                return numFragments;
            }
        }
        public int MessageNumber
        {
            get
            {
                return messageNumber;
            }
        }
        public int FragmentNumber
        {
            get
            {
                return fragmentNumber;
            }
        }
        public byte[] Payload
        {
            get
            {
                return payload;
            }
        }

        #endregion
    }

    public class RoadInfoMessage : BaseWorldMessage
    {
        string name;
        /// <summary>
        ///   Half the width of the road.  If no road width is specified, 
        ///   we will use the magic value of -1, and not set the road width.
        ///   This means we will end up with the default, which is probably 2.
        /// </summary>
        int halfWidth;
        List<Vector3> points;

        public RoadInfoMessage()
        {
            this.MessageType = WorldMessageType.RoadInfo;
            points = new List<Vector3>();
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            name = inMessage.ReadString();
            points = new List<Vector3>();
            int numPoints = inMessage.ReadInt32();
            for (int i = 0; i < numPoints; ++i)
            {
                Vector3 point = inMessage.ReadVector();
                points.Add(point);
            }
            UnityEngine.Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! RoadInfoMessage ");

            try
            {
                halfWidth = inMessage.ReadInt32();
            }
            catch (System.IO.EndOfStreamException)
            {
                UnityEngine.Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! RoadInfoMessage Exception");

                //log.Warn("Got old style road message");
                // ignore this - it means we got an old style message
                halfWidth = -1; // Use default
            }
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(name);
            outMessage.Write(points.Count);
            foreach (Vector3 point in points)
                outMessage.Write(point);
            outMessage.Write(halfWidth);
        }
        #endregion

        #region Properties
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public int HalfWidth
        {
            get
            {
                return halfWidth;
            }
            set
            {
                halfWidth = value;
            }
        }
        public List<Vector3> Points
        {
            get
            {
                return points;
            }
        }
        #endregion
    }

    public class FogMessage : BaseWorldMessage
    {
        Color fogColor;
        int fogStart;
        int fogEnd;

        public FogMessage()
        {
            this.MessageType = WorldMessageType.Fog;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            fogColor = inMessage.ReadColor();
            fogStart = inMessage.ReadInt32();
            fogEnd = inMessage.ReadInt32();
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(fogColor);
            outMessage.Write(fogStart);
            outMessage.Write(fogEnd);
        }
        #endregion

        #region Properties
        public Color FogColor
        {
            get
            {
                return fogColor;
            }
            set
            {
                fogColor = value;
            }
        }
        public int FogStart
        {
            get
            {
                return fogStart;
            }
            set
            {
                fogStart = value;
            }
        }
        public int FogEnd
        {
            get
            {
                return fogEnd;
            }
            set
            {
                fogEnd = value;
            }
        }
        #endregion
    }

    public class AbilityUpdateMessage : BaseWorldMessage
    {
        List<AbilityEntry> entries;

        public AbilityUpdateMessage()
        {
            this.MessageType = WorldMessageType.AbilityUpdate;
            entries = new List<AbilityEntry>();
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            int numEntries = inMessage.ReadInt32();
            for (int i = 0; i < numEntries; ++i)
            {
                AbilityEntry entry = new AbilityEntry();
                entry.name = inMessage.ReadString();
                entry.icon = inMessage.ReadString();
                entry.category = inMessage.ReadString();
                entries.Add(entry);
            }
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(entries.Count);
            foreach (AbilityEntry entry in entries)
            {
                outMessage.Write(entry.name);
                outMessage.Write(entry.icon);
                outMessage.Write(entry.category);
            }
        }
        #endregion

        #region Properties
        public List<AbilityEntry> Entries
        {
            get
            {
                return entries;
            }
        }
        #endregion
    }

    public class AbilityInfoMessage : BaseWorldMessage
    {
        string name;
        string icon;
        string description;
        List<string> cooldowns;
        Dictionary<string, string> properties;

        public AbilityInfoMessage()
        {
            this.MessageType = WorldMessageType.AbilityInfo;
            cooldowns = new List<string>();
            properties = new Dictionary<string, string>();
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            name = inMessage.ReadString();
            icon = inMessage.ReadString();
            description = inMessage.ReadString();

            int numCooldowns = inMessage.ReadInt32();
            for (int i = 0; i < numCooldowns; i++)
                cooldowns.Add(inMessage.ReadString());

            int numProperties = inMessage.ReadInt32();
            for (int i = 0; i < numProperties; i++)
            {
                string key = inMessage.ReadString();
                string value = inMessage.ReadString();
                properties[key] = value;
            }
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(name);
            outMessage.Write(icon);
            outMessage.Write(description);

            outMessage.Write(cooldowns.Count);
            foreach (string entry in cooldowns)
                outMessage.Write(entry);

            outMessage.Write(properties.Count);
            foreach (string key in properties.Keys)
            {
                outMessage.Write(key);
                outMessage.Write(properties[key]);
            }
        }
        #endregion

        #region Properties
        public List<string> Cooldowns
        {
            get
            {
                return cooldowns;
            }
        }
        public Dictionary<string, string> Properties
        {
            get
            {
                return properties;
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public string Icon
        {
            get
            {
                return icon;
            }
            set
            {
                icon = value;
            }
        }
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }
        #endregion
    }

    public class OldObjectPropertyMessage : BaseWorldMessage
    {
        Dictionary<string, string> properties;

        public OldObjectPropertyMessage()
        {
            this.MessageType = WorldMessageType.OldObjectProperty;
            properties = new Dictionary<string, string>();
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            int numProperties = inMessage.ReadInt32();
            for (int i = 0; i < numProperties; i++)
            {
                string key = inMessage.ReadString();
                string value = inMessage.ReadString();
                properties[key] = value;
            }
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(properties.Count);
            foreach (string key in properties.Keys)
            {
                outMessage.Write(key);
                outMessage.Write(properties[key]);
            }
        }
        #endregion

        #region Properties
        public Dictionary<string, string> Properties
        {
            get
            {
                return properties;
            }
        }
        #endregion
    }

    public class ObjectPropertyMessage : BaseWorldMessage
    {
        protected PropertyMap propertyMap = new PropertyMap();

        public ObjectPropertyMessage()
        {
            this.MessageType = WorldMessageType.ObjectProperty;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            propertyMap = new PropertyMap();
            propertyMap.ParseMessage(inMessage);
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            propertyMap.WriteMessage(outMessage);
        }

        public int GetIntProperty(string key)
        {
            return propertyMap.GetIntProperty(key);
        }

        public long GetLongProperty(string key)
        {
            return propertyMap.GetLongProperty(key);
        }

        public float GetFloatProperty(string key)
        {
            return propertyMap.GetFloatProperty(key);
        }

        public string GetStringProperty(string key)
        {
            return propertyMap.GetStringProperty(key);
        }

        public Vector3 GetVector3Property(string key)
        {
            return propertyMap.GetVector3Property(key);
        }

        public Quaternion GetQuaternionProperty(string key)
        {
            return propertyMap.GetQuaternionProperty(key);
        }

        public System.Object GetObjectProperty(string key)
        {
            return propertyMap.GetObjectProperty(key);
        }

        #endregion

        #region Properties
        public Dictionary<string, object> Properties
        {
            get
            {
                return propertyMap.Properties;
            }
        }
        #endregion
    }

    public class InvokeEffectMessage : BaseWorldMessage
    {
        PropertyMap propertyMap = new PropertyMap();
        string effectName;

        public InvokeEffectMessage()
        {
            this.MessageType = WorldMessageType.InvokeEffect;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            propertyMap = new PropertyMap();
            // read the effect name from the message
            effectName = inMessage.ReadString();
            propertyMap.ParseMessage(inMessage);
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(effectName);
            propertyMap.WriteMessage(outMessage);
        }

        #endregion

        #region Properties
        public Dictionary<string, object> Args
        {
            get
            {
                return propertyMap.Properties;
                ;
            }
        }

        public string EffectName
        {
            get
            {
                return effectName;
            }
            set
            {
                effectName = value;
            }
        }

        #endregion
    }

    public class ActivateItemMessage : TargetMessage
    {
        protected OID itemId;

        public ActivateItemMessage()
        {
            this.MessageType = WorldMessageType.ActivateItem;
        }

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
            outMessage.Write(itemId);
        }
        #endregion

        #region Properties
        public OID ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
            }
        }
        #endregion
    }

    public class AddParticleEffectMessage : TargetMessage
    {
        protected string slotName;
        protected string effectName;
        protected Quaternion orientation;
        protected float velocityMultiplier;
        protected float particleSizeMultiplier;
        protected byte particleBooleans;
        protected Color color;

        // These values are or'ed together to make the particleBooleans
        // LocalSpace should not be used, since this has migrated to the particle
        // script instead.
        public enum Flags { LocalSpace = 1, WorldOrientation = 2, HasColor = 4 };

        public AddParticleEffectMessage()
        {
            this.MessageType = WorldMessageType.AddParticleEffect;
            orientation = Quaternion.identity;
            velocityMultiplier = 1.0f;
            particleSizeMultiplier = 1.0f;
            particleBooleans = 0;
            //color = null; - ANDREW
            color = new Color();
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            slotName = inMessage.ReadString();
            effectName = inMessage.ReadString();
            orientation = inMessage.ReadQuaternion();
            velocityMultiplier = inMessage.ReadSingle();
            particleSizeMultiplier = inMessage.ReadSingle();
            particleBooleans = inMessage.ReadByte();
            if (GetFlag(Flags.HasColor))
                color = inMessage.ReadColor();
        }

        public bool GetFlag(Flags flag)
        {
            return ((particleBooleans & (byte)flag) != 0);
        }

        public void SetFlag(Flags flag, bool value)
        {
            if (value)
                particleBooleans |= (byte)flag;
            else
            {
                byte tmp = (byte)flag;
                byte not = (byte)(~tmp);
                particleBooleans &= not;
            }
        }

        #endregion

        #region Properties
        public string SlotName
        {
            get
            {
                return slotName;
            }
            set
            {
                slotName = value;
            }
        }

        public string EffectName
        {
            get
            {
                return effectName;
            }
            set
            {
                effectName = value;
            }
        }

        public Quaternion Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value;
            }
        }

        public float VelocityMultiplier
        {
            get
            {
                return velocityMultiplier;
            }
            set
            {
                velocityMultiplier = value;
            }
        }

        public float ParticleSizeMultiplier
        {
            get
            {
                return particleSizeMultiplier;
            }
            set
            {
                particleSizeMultiplier = value;
            }
        }

        public byte ParticleBooleans
        {
            get
            {
                return particleBooleans;
            }
            set
            {
                particleBooleans = value;
            }
        }

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                // if the color is non-null, set our flag
                SetFlag(Flags.HasColor, color != null);
            }
        }

        #endregion
    }

    public class RemoveParticleEffectMessage : TargetMessage
    {
        public RemoveParticleEffectMessage()
        {
            this.MessageType = WorldMessageType.RemoveParticleEffect;
        }
    }

    public class TrackObjectInterpolationMessage : TargetMessage
    {
        protected long particleOid;
        protected string targetSocket;
        protected int timeToImpact;  // Milliseconds
        protected long timestamp;

        public TrackObjectInterpolationMessage()
        {
            this.MessageType = WorldMessageType.TrackObjectInterpolation;
            targetSocket = "";
            timeToImpact = 0;
            timestamp = 0;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            particleOid = inMessage.ReadInt64();
            timeToImpact = inMessage.ReadInt32();
            timestamp = inMessage.ReadTimestamp();
            targetSocket = inMessage.ReadString();
        }
        #endregion

        #region Properties

        public long ParticleOid
        {
            get
            {
                return particleOid;
            }
            set
            {
                particleOid = value;
            }
        }

        public string TargetSocket
        {
            get
            {
                return targetSocket;
            }
            set
            {
                targetSocket = value;
            }
        }

        public int TimeToImpact
        {
            get
            {
                return timeToImpact;
            }
            set
            {
                timeToImpact = value;
            }
        }

        public long Timestamp
        {
            get
            {
                return timestamp;
            }
            set
            {
                timestamp = value;
            }
        }

        #endregion
    }

    public class TrackLocationInterpolationMessage : BaseWorldMessage
    {
        protected long particleOid;
        protected int timeToImpact;  // Milliseconds
        protected Vector3 location;
        protected long timestamp;

        public TrackLocationInterpolationMessage()
        {
            this.MessageType = WorldMessageType.TrackObjectInterpolation;
            timeToImpact = 0;
            location = new Vector3(0, 0, 0);
            timestamp = 0;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            particleOid = inMessage.ReadInt64();
            timeToImpact = inMessage.ReadInt32();
            timestamp = inMessage.ReadTimestamp();
            location = inMessage.ReadVector();
        }
        #endregion

        #region Properties

        public long ParticleOid
        {
            get
            {
                return particleOid;
            }
            set
            {
                particleOid = value;
            }
        }

        public int TimeToImpact
        {
            get
            {
                return timeToImpact;
            }
            set
            {
                timeToImpact = value;
            }
        }

        public Vector3 Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
            }
        }

        public long Timestamp
        {
            get
            {
                return timestamp;
            }
            set
            {
                timestamp = value;
            }
        }

        #endregion
    }

    public class OldExtensionMessage : ObjectPropertyMessage
    {
        public OldExtensionMessage()
        {
            this.MessageType = WorldMessageType.OldExtension;
        }
    }

    public class ExtensionMessage : ObjectPropertyMessage
    {

        OID targetOid;
        bool clientTargeted;

        public ExtensionMessage()
        {
            this.MessageType = WorldMessageType.Extension;
        }

        public ExtensionMessage(long targetOid, bool clientTargeted)
        {
            this.MessageType = WorldMessageType.Extension;
            if (targetOid == 0)
                this.targetOid = null;
            this.clientTargeted = clientTargeted;
        }

        public ExtensionMessage(OID targetOid, bool clientTargeted)
        {
            this.MessageType = WorldMessageType.Extension;
            this.targetOid = targetOid;
            this.clientTargeted = clientTargeted;
        }

        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            byte b = inMessage.ReadByte();
            if ((b & 1) != 0)
                targetOid = inMessage.ReadOID();
            clientTargeted = (b & 2) != 0;
            propertyMap = new PropertyMap();
            propertyMap.ParseMessage(inMessage);
        }

        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            byte b = (byte)((targetOid != null ? 1 : 0) | (clientTargeted ? 2 : 0));
            outMessage.Write(b);
            if (targetOid != null)
                outMessage.Write(targetOid);
            propertyMap.WriteMessage(outMessage);
        }

        public OID TargetOid
        {
            get
            {
                return targetOid;
            }
            set
            {
                targetOid = value;
            }
        }

        public bool ClientTargeted
        {
            get
            {
                return clientTargeted;
            }
            set
            {
                clientTargeted = value;
            }
        }
    }

    public class MobPathMessage : BaseWorldMessage
    {
        protected long startTime;
        protected string interpKind;
        protected float speed;
        protected string terrainString;
        protected List<Vector3> pathPoints;
        protected bool arrived;
        protected string source;

        public MobPathMessage()
        {
            this.MessageType = WorldMessageType.MobPath;
            startTime = 0;
            interpKind = "linear";
            speed = 0f;
            terrainString = "";
            pathPoints = null;
            arrived = false;
            source = "";
        }

        #region Methods

        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            startTime = inMessage.ReadTimestamp();
            interpKind = inMessage.ReadString();
            speed = inMessage.ReadSingle();
            terrainString = inMessage.ReadString();
            int count = inMessage.ReadInt32();
            pathPoints = new List<Vector3>();
            for (int i = 0; i < count; i++)
            {
                Vector3 v3 = inMessage.ReadVector();
             //   UnityEngine.Debug.LogWarning("MobPathMessage: !!!!!!!!!!!!!!!!  i="+i+" Vector3 " + v3);
                pathPoints.Add(v3);
           
            }

            arrived = inMessage.ReadBool();
            source = inMessage.ReadString();
            //  UnityEngine.Debug.LogWarning("MobPathMessage: !!!!!!!!!!!!!!!! path  " + string.Join(";", pathPoints.ToArray()));

        }

        #endregion

        #region Properties

        public long StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                startTime = value;
            }
        }

        public string InterpKind
        {
            get
            {
                return interpKind;
            }
            set
            {
                interpKind = value;
            }
        }

        public float Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
            }
        }

        public string TerrainString
        {
            get
            {
                return terrainString;
            }
            set
            {
                terrainString = value;
            }
        }

        public List<Vector3> PathPoints
        {
            get
            {
                return pathPoints;
            }
            set
            {
                pathPoints = value;
            }
        }

        public bool Arrived
        {
            get
            {
                return arrived;
            }
            set
            {
                arrived = value;
            }
        }
        public string Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
            }
        }
        #endregion
    }

    public class AggregatedRDPMessage : BaseWorldMessage
    {
        protected List<BaseWorldMessage> subMessages = new List<BaseWorldMessage>();

        public AggregatedRDPMessage()
        {
            this.MessageType = WorldMessageType.AggregatedRDP;
        }

        #region Methods

        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            int subMessageCount = inMessage.ReadInt32();
            for (int i = 0; i < subMessageCount; i++)
            {
                byte[] subMessageBytes = inMessage.ReadBytes();
                AtavismIncomingMessage subMessage = new AtavismIncomingMessage(subMessageBytes, inMessage);
                BaseWorldMessage msg = WorldMessageFactory.ReadMessage(subMessage);
                if (msg != null)
                    subMessages.Add(msg);
                //else
                //log.WarnFormat("AggregatedRDP.ParseMessage got null when reading message from {0} bytes", subMessageBytes.Length);
            }
        }

        #endregion

        #region Properties

        public List<BaseWorldMessage> SubMessages
        {
            get
            {
                return subMessages;
            }
        }

        #endregion
    }

    public class NewDecalMessage : BaseWorldMessage
    {
        protected string imageName;
        protected long positionX;
        protected long positionZ;
        protected float sizeX;
        protected float sizeZ;
        protected float rotation;
        protected int priority;
        protected long expireTime;

        public NewDecalMessage()
        {
            this.MessageType = WorldMessageType.NewDecal;
            imageName = string.Empty;
            positionX = 0;
            positionZ = 0;
            sizeX = 0;
            sizeZ = 0;
            rotation = 0;
            priority = 0;
            expireTime = 0;
        }

        #region Methods

        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            imageName = inMessage.ReadString();
            positionX = inMessage.ReadInt32();
            positionZ = inMessage.ReadInt32();
            sizeX = inMessage.ReadSingle();
            sizeZ = inMessage.ReadSingle();
            rotation = inMessage.ReadSingle();
            priority = inMessage.ReadInt32();
            expireTime = inMessage.ReadInt64();
        }

        #endregion

        #region Properties

        public string ImageName
        {
            get
            {
                return imageName;
            }
            set
            {
                imageName = value;
            }
        }

        public long PositionX
        {
            get
            {
                return positionX;
            }
            set
            {
                positionX = value;
            }
        }

        public long PositionZ
        {
            get
            {
                return positionZ;
            }
            set
            {
                positionZ = value;
            }
        }

        public float SizeX
        {
            get
            {
                return sizeX;
            }
            set
            {
                sizeX = value;
            }
        }

        public float SizeZ
        {
            get
            {
                return sizeZ;
            }
            set
            {
                sizeZ = value;
            }
        }

        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        public int Priority
        {
            get
            {
                return priority;
            }
            set
            {
                priority = value;
            }
        }

        public long ExpireTime
        {
            get
            {
                return expireTime;
            }
            set
            {
                expireTime = value;
            }
        }

        #endregion
    }

    public class FreeDecalMessage : BaseWorldMessage
    {
        public FreeDecalMessage()
        {
            this.MessageType = WorldMessageType.FreeDecal;
        }

        // Need to define this method in order to avoid having
        // BaseWorldMessage throw an error
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
        }
    }

    public class ModelInfoMessage : BaseWorldMessage
    {
        protected List<MeshInfo> modelInfo;
        protected bool forceInstantLoad;

        public ModelInfoMessage()
        {
            this.MessageType = WorldMessageType.ModelInfo;
            modelInfo = new List<MeshInfo>();
            forceInstantLoad = false;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            modelInfo = new List<MeshInfo>();
            int meshCount = inMessage.ReadInt32();
            for (int i = 0; i < meshCount; ++i)
            {
                MeshInfo meshInfo = new MeshInfo();
                meshInfo.ParseMessage(inMessage);
                modelInfo.Add(meshInfo);
            }
            forceInstantLoad = inMessage.ReadBool();
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(modelInfo.Count);
            foreach (MeshInfo meshInfo in modelInfo)
                meshInfo.WriteMessage(outMessage);
            outMessage.Write(forceInstantLoad);
        }
        #endregion

        #region Properties
        public List<MeshInfo> ModelInfo
        {
            get
            {
                return modelInfo;
            }
        }
        public bool ForceInstantLoad
        {
            get
            {
                return forceInstantLoad;
            }
        }
        #endregion
    }

    public class SoundControlMessage : BaseWorldMessage
    {
        Dictionary<string, PropertyMap> newSoundEntries = new Dictionary<string, PropertyMap>();
        List<string> freeSoundEntries = new List<string>();
        bool clearSounds = false;

        public SoundControlMessage()
        {
            this.MessageType = WorldMessageType.SoundControl;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            newSoundEntries = new Dictionary<string, PropertyMap>();
            freeSoundEntries = new List<string>();
            clearSounds = false;

            int numEntries = inMessage.ReadInt32();
            for (int i = 0; i < numEntries; ++i)
            {
                string msgType = inMessage.ReadString();
                switch (msgType)
                {
                    case "on":
                        {
                            string soundName = inMessage.ReadString();
                            PropertyMap propertyMap = new PropertyMap();
                            propertyMap.ParseMessage(inMessage);
                            newSoundEntries[soundName] = propertyMap;
                        }
                        break;
                    case "off":
                        freeSoundEntries.Add(inMessage.ReadString());
                        break;
                    case "clear":
                        clearSounds = true;
                        break;
                }
            }
        }

        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            int numEntries = newSoundEntries.Count + freeSoundEntries.Count;
            if (clearSounds)
                numEntries++;
            outMessage.Write(numEntries);
            foreach (KeyValuePair<string, PropertyMap> kvp in newSoundEntries)
            {
                outMessage.Write("on");
                outMessage.Write(kvp.Key);
                kvp.Value.WriteMessage(outMessage);
            }
            foreach (string soundName in freeSoundEntries)
            {
                outMessage.Write("off");
                outMessage.Write(soundName);
            }
            if (clearSounds)
                outMessage.Write("clear");
        }


        #endregion

        #region Properties
        public Dictionary<string, PropertyMap> NewSoundEntries
        {
            get
            {
                return newSoundEntries;
            }
        }
        public List<string> FreeSoundEntries
        {
            get
            {
                return freeSoundEntries;
            }
        }
        public bool ClearSounds
        {
            get
            {
                return clearSounds;
            }
        }
        #endregion
    }

    public class AuthorizedLoginMessage : LoginMessage
    {
        byte[] worldToken;

        public AuthorizedLoginMessage()
        {
            this.MessageType = WorldMessageType.AuthorizedLogin;
        }

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
            outMessage.Write(worldToken);
        }

        #endregion

        #region Properties
        public byte[] WorldToken
        {
            set
            {
                worldToken = value;
            }
        }
        #endregion
    }

    public class AuthorizedLoginResponseMessage : LoginResponseMessage
    {
        string version;

        public AuthorizedLoginResponseMessage()
        {
            this.MessageType = WorldMessageType.AuthorizedLoginResponse;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            base.ParseMessage(inMessage);
            version = inMessage.ReadString();
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            base.WriteMessage(outMessage);
            outMessage.Write(version);
        }

        #endregion

        #region Properties
        public string Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
        }
        #endregion
    }

    public class LoadingStateMessage : BaseWorldMessage
    {
        private bool loadingState = false;

        public LoadingStateMessage()
        {
            this.MessageType = WorldMessageType.LoadingState;
        }

        #region Methods

        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            loadingState = inMessage.ReadBool();
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(loadingState);
        }

        #endregion

        #region Properties
        public bool LoadingState
        {
            get
            {
                return loadingState;
            }
            set
            {
                loadingState = value;
            }
        }
        #endregion
    }

    public class WorldFileMessage : BaseWorldMessage
    {
        string worldFileName;
        Vector3 loc;
        bool hasLoc;
        public WorldFileMessage()
        {
            this.MessageType = WorldMessageType.WorldFile;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            //UnityEngine.Debug.LogError("Got world file message");
            worldFileName = inMessage.ReadString();
            //UnityEngine.Debug.LogError("World file name: " + worldFileName);
            hasLoc = inMessage.ReadBool();
            //UnityEngine.Debug.LogError("World file message has loc: " + hasLoc);
            if (hasLoc)
                loc = inMessage.ReadVector();
            //UnityEngine.Debug.LogError("World file message loc: " + loc);
            //UnityEngine.Debug.LogError("Finished Parsing world file message");
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(worldFileName);
        }
        #endregion

        #region Properties
        public string WorldFileName
        {
            get
            {
                return worldFileName;
            }
            set
            {
                worldFileName = value;
            }
        }
        public Vector3 Location
        {
            get
            {
                return loc;
            }
            set
            {
                loc = value;
            }
        }
        public bool HasLoc
        {
            get
            {
                return hasLoc;
            }
            set
            {
                hasLoc = value;
            }
        }
        #endregion
    }

    public class IslandManifestMessage : BaseWorldMessage
    {
        string worldName;
        string worldFilesDirectoryUrl;
        int worldFileCount;
        Dictionary<string, string> worldFiles;
        public IslandManifestMessage()
        {
            this.MessageType = WorldMessageType.IslandManifest;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            worldName = inMessage.ReadString();
            worldFilesDirectoryUrl = inMessage.ReadString();
            worldFileCount = inMessage.ReadInt32();
            worldFiles = new Dictionary<string, string>();
            for (int i = 0; i < worldFileCount; i++)
            {
                string fileName = inMessage.ReadString();
                string fileHash = inMessage.ReadString();
                worldFiles.Add(fileName, fileHash);
            }

        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(worldName);
        }
        #endregion

        #region Properties
        public string WorldName
        {
            get
            {
                return worldName;
            }
            set
            {
                worldName = value;
            }
        }
        public string WorldFilesDirectoryUrl
        {
            get
            {
                return worldFilesDirectoryUrl;
            }
            set
            {
                worldFilesDirectoryUrl = value;
            }
        }
        public int WorldFileCount
        {
            get
            {
                return worldFileCount;
            }
            set
            {
                worldFileCount = value;
            }
        }
        public Dictionary<string, string> WorldFiles
        {
            get
            {
                return worldFiles;
            }
            set
            {
                worldFiles = value;
            }
        }
        #endregion
    }

    public class SceneLoadedMessage : BaseWorldMessage
    {
        public SceneLoadedMessage()
        {
            this.MessageType = WorldMessageType.SceneLoaded;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
        }
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write("sometext");
        }
        #endregion

        #region Properties
        #endregion
    }




    public class PrefabRequestMessage : BaseWorldTcpMessage
    {
        string typeMsg;
        PropertyMap propertyMap = new PropertyMap();

        public PrefabRequestMessage()
        {
            this.MessageType = WorldTcpMessageType.PrefabRequest;
        }

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(typeMsg);
            propertyMap.OldWriteMessage(outMessage);
        }
        #endregion

        #region Properties
        public string TypeMsg
        {
            get
            {
                return typeMsg;
            }
            set
            {
                typeMsg = value;
            }
        }
        public Dictionary<string, object> Properties
        {
            get
            {
                return propertyMap.Properties;
            }
            set
            {
                propertyMap.Properties.Clear();
                foreach (string key in value.Keys)
                    propertyMap.Properties[key] = value[key];
            }
        }
        #endregion
    }



    public class PrefabResponseMessage : BaseWorldTcpMessage
    {
        string typeMsg;
        PropertyMap propertyMap = new PropertyMap();

        public PrefabResponseMessage()
        {
            this.MessageType = WorldTcpMessageType.PrefabResponse;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            typeMsg = inMessage.ReadString();
            propertyMap = new PropertyMap();
            propertyMap.OldParseMessage(inMessage);
        }
        #endregion

        #region Properties
        public string TypeMsg
        {
            get
            {
                return typeMsg;
            }
            set
            {
                typeMsg = value;
            }
        }
        public Dictionary<string, object> Properties
        {
            get
            {
                return propertyMap.Properties;
            }
        }
        #endregion
    }

    public class IconPrefabRequestMessage : BaseWorldTcpMessage
    {
        string typeMsg;
        PropertyMap propertyMap = new PropertyMap();

        public IconPrefabRequestMessage()
        {
            this.MessageType = WorldTcpMessageType.IconPrefabRequest;
        }

        #region Methods
        protected override void WriteMessage(AtavismOutgoingMessage outMessage)
        {
            outMessage.Write(typeMsg);
            propertyMap.OldWriteMessage(outMessage);
        }
        #endregion

        #region Properties
        public string TypeMsg
        {
            get
            {
                return typeMsg;
            }
            set
            {
                typeMsg = value;
            }
        }
        public Dictionary<string, object> Properties
        {
            get
            {
                return propertyMap.Properties;
            }
            set
            {
                propertyMap.Properties.Clear();
                foreach (string key in value.Keys)
                    propertyMap.Properties[key] = value[key];
            }
        }
        #endregion
    }



    public class IconPrefabResponseMessage : BaseWorldTcpMessage
    {
        string typeMsg;
        PropertyMap propertyMap = new PropertyMap();

        public IconPrefabResponseMessage()
        {
            this.MessageType = WorldTcpMessageType.IconPrefabResponse;
        }

        #region Methods
        protected override void ParseMessage(AtavismIncomingMessage inMessage)
        {
            typeMsg = inMessage.ReadString();
            propertyMap = new PropertyMap();
            propertyMap.OldParseMessage(inMessage);
        }
        #endregion

        #region Properties
        public string TypeMsg
        {
            get
            {
                return typeMsg;
            }
            set
            {
                typeMsg = value;
            }
        }
        public Dictionary<string, object> Properties
        {
            get
            {
                return propertyMap.Properties;
            }
        }
        #endregion
    }

    #endregion
}