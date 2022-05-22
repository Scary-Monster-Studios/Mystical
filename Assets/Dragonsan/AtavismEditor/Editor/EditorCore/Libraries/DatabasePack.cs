using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
//using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml;

namespace Atavism
{
    public class Register
    {
        public enum TypesOfField
        {
            Int,
            Float,
            Bool,
            String
        };

        public string fieldName;
        public string fieldReference;
        public MySqlDbType mysqlType;
        public string fieldValue;
        public TypesOfField fieldType;

        public Register(string name, string reference, MySqlDbType dbType, string data, TypesOfField type)
        {
            fieldName = name;
            fieldReference = reference;
            mysqlType = dbType;
            fieldValue = data;
            fieldType = type;
        }
    }

    // Static Class to Acces the Database from other Plugins
    public static class DatabasePack
    {

        public static string contentDatabasePrefix = "";
        public static string adminDatabasePrefix = "_admin";
        public static string masterDatabasePrefix = "_master";
        public static string atavismDatabasePrefix = "_atavism";

        public static string loginDatabasePrefix = "login";
        public static string loginTable = "user";
        private static string loginDatabaseHost = "";
        private static string loginDatabaseUser = "";           // Database User Name
        private static string loginDatabasePassword = "";       // Database Password
        private static string loginDatabaseName = "";           // Database Name

        public static MySqlConnection con = null;   // Connection reference
        public static string connectionResult = "";     // Connection results
        private static MySqlDataReader data = null;

     //   private static Dictionary<string, string> fields = new Dictionary<string, string>();

        private static string databaseHost;         // Database Host Address
        private static string databasePort;         // Database Host Port
        private static string databaseUser;         // Database User Name
        private static string databasePassword;     // Database Password
        private static string databaseName;         // Database Name

        private static string currentPrefix = "none";
        /*private static List<string> contentConnections;
        private static string currentContentConnection = "";
        private static List<string> adminConnections;
        private static string currentAdminConnection = "";*/

        public static bool useUtf8mb4 = false;
        private static bool testlock = false;

        public enum TypesOfField
        {
            Int,
            Float,
            Bool,
            String
        };

        public static string GetProjectName()
        {
            string[] s = Application.dataPath.Split('/');
            string projectName = s[s.Length - 2];
            //Debug.Log("project = " + projectName);
            return projectName;
        }

        public static void LoadConnections()
        {
            //databaseHost = EditorPrefs.GetString("databaseHost" + prefix);
        }




        public static MySqlConnection Connect(string prefix)
        {
            return Connect(prefix, 15, false);
        }



        // Connect to the Database
        public static MySqlConnection Connect(string prefix, int timeout, bool force)
        {

            // If there is no connection available
            if (prefix != currentPrefix)
            {
                con = null;
                currentPrefix = prefix;
            }

            if (con == null || force)
            {
                if (prefix == loginDatabasePrefix)
                {
                    databaseHost = loginDatabaseHost;
                    databaseName = loginDatabaseName;
                    databaseUser = loginDatabaseUser;
                    databasePassword = loginDatabasePassword;
                }
                else
                {
                    ReadConfig();
                    databaseHost = EditorPrefs.GetString("databaseHost" + prefix + GetProjectName());
                    databasePort = EditorPrefs.GetString("databasePort" + prefix + GetProjectName());
                    databaseName = EditorPrefs.GetString("databaseName" + prefix + GetProjectName());
                    databaseUser = EditorPrefs.GetString("databaseUser" + prefix + GetProjectName());
                    databasePassword = EditorPrefs.GetString("databasePassword" + prefix + GetProjectName());
                }
                if (string.IsNullOrEmpty(databaseHost))
                {
                    Debug.LogError("Database Host is Empty");
                    SaveLog("Database Host is Empty");

                    return null;
                }
                if (string.IsNullOrEmpty(databaseUser))
                {
                    Debug.LogError("Database Username is Empty");
                    SaveLog("Database Username is Empty");

                    return null;
                }
                if (string.IsNullOrEmpty(databasePassword))
                {
                    Debug.LogError("Database Password is Empty");
                    SaveLog("Database Password is Empty");
                    return null;
                }

                string conString;
                conString = "Server=" + databaseHost;
                if (databasePort != "")
                    conString += ";Port=" + databasePort;
                conString += ";Database=" + databaseName;
                conString += ";User ID=" + databaseUser + ";";
                conString += "Password=\"" + databasePassword + '"';
                if (useUtf8mb4)
                    conString += ";CharSet=utf8mb4";
                else
                    conString += ";CharSet=utf8";
                // conString += ";Pooling=true";
                conString += ";Pooling=true;Connect Timeout="+ timeout+ ";Default Command Timeout="+timeout;
               // conString += ";insecureAuth=true";
               conString += ";SslMode=None";
                //string constr = "Server=" + databaseHost + ";Database=" + databaseName + ";User ID=" + databaseUser + ";Password=" + databasePassword + ";Pooling=true";
                // Create a new connection
              //  UnityEngine.Debug.Log("Connecting to server with constring: " + conString);
                con = new MySqlConnection(conString);
            }
            return con;
        }

        public static bool TestConnection(string prefix)
        {
            return TestConnection(prefix, false);
        }

        // Test the data base connection
        public static bool TestConnection(string prefix, bool force)
        {
            //  Debug.Log("Database test connection force=" + force+ " testlock=" + testlock);
            float startTime = Time.time;
            if (!force && testlock)
            {
                connectionResult = "Testing....";
                return false;
            }
            if (force)
            {
                testlock = false;
            }
            // Set connection as in test
            connectionResult = "Testing...";

            // Load database setup parameters and setup the connection element
            string conString = LoadParameters(prefix);
            if (string.IsNullOrEmpty(conString))
            {
                connectionResult = "Failed:  fill credentials";
                return false;
            }

            //  Debug.Log("Database test connection time:" + roznica);
            // Try to connect
            try
            {
                con = new MySqlConnection(conString);
                currentPrefix = prefix;
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT 1", con))
                {
                    cmd.ExecuteScalar();
                }
            }
            catch (TargetInvocationException e)
            {
                connectionResult = "Failed: " + e.Message;
              //  float stopTime2 = Time.time;
              //  float roznica2 = stopTime2 - startTime;
                Debug.LogError("Database test connection Failed: " + e.Message);
                SaveLog("Database test connection Failed: " + e.Message);
                testlock = true;
                return false;
            }
            catch (TimeoutException e)
            {
                connectionResult = "Failed: " + e.Message;
             //   float stopTime2 = Time.time;
             //   float roznica2 = stopTime2 - startTime;
                Debug.LogError("Database test connection Failed: " + e.Message);
                SaveLog("Database test connection Failed: " + e.Message);
                testlock = true;
                return false;
            }
            catch (IOException e)
            {
                connectionResult = "Failed: " + e.Message;
              //  float stopTime2 = Time.time;
              //  float roznica2 = stopTime2 - startTime;
                Debug.LogError("Database test connection Failed: " + e.Message);
                SaveLog("Database test connection Failed: " + e.Message);
                testlock = true;
                return false;
            }
            catch (System.Net.Sockets.SocketException e)
            {
                connectionResult = "Failed: " + e.Message;
              //  float stopTime2 = Time.time;
              //  float roznica2 = stopTime2 - startTime;
                Debug.LogError("Database test connection Failed: " + e.Message);
                SaveLog("Database test connection Failed: " + e.Message);
                testlock = true;
                return false;
            }
            catch (MySqlException e)
            {
                connectionResult = "Failed: " + e.Message;
               // float stopTime2 = Time.time;
               // float roznica2 = stopTime2 - startTime;
                Debug.LogError("Database test connection Failed: " + e.Message);
                SaveLog("Database test connection Failed: " + e.Message);
                testlock = true;
                return false;
            }
            catch (Exception e)
            {
                connectionResult = "Failed: " + e.Message;
                //float stopTime2 = Time.time;
                //float roznica2 = stopTime2 - startTime;
                Debug.LogError("Database test connection Failed: " + e.Message);
                SaveLog("Database test connection Failed: " + e.Message);
                testlock = true;
                return false;
            }
            float stopTime = Time.time;
            float diffTime = stopTime - startTime;
            //
            if (diffTime > 15f)
            {
                Debug.Log("Database test connection time:" + diffTime);
                SaveLog("Database test connection time: " + diffTime);
                testlock = true;
            }
            if (con.State.ToString() == "Open" && !testlock)
            {
                connectionResult = "Success";
                con.Close();
                return true;
            }
            else
            {
                connectionResult = "Failed";
                con.Close();
                return false;
            }

        }
         public static void ReadConfig()
      {
          if (File.Exists(Path.GetFullPath("Assets/..") + "/atavismeditorprofile.xml"))
          {
              string text = System.IO.File.ReadAllText(Path.GetFullPath("Assets/..") + "/atavismeditorprofile.xml");
              text =  text.Replace("&", "&amp;");
              text = "<atavism>" + text + "</atavism>";
              XmlDocument doc = new XmlDocument();
              doc.LoadXml(text);
              XmlNodeList cnl = doc.GetElementsByTagName("atavism");
              string str = null;
              cnl[0].ChildNodes.Item(0).InnerText.Trim();
             for (int i = 0; i < cnl[0].ChildNodes.Count; i++)
             {
                 str += cnl[0].ChildNodes.Item(i).Name+ " > "+cnl[0].ChildNodes.Item(i).InnerText+ " | ";
                 if (cnl[0].ChildNodes.Item(i).Name == "databases")
                 {
                     XmlNodeList dbdata = cnl[0].ChildNodes.Item(i).ChildNodes;
                     string type = "";
                     string dbname = "";
                     string dbuser = "";
                     string dbpass = "";
                     string dbaddr = "";
                     string dbport = "";
                     for (int j = 0; j < dbdata.Count; j++)
                     {
                         switch (@dbdata.Item(j).Name)
                         {
                             case "type":
                                 type = dbdata.Item(j).InnerText;
                                 break;
                             case "host":
                                 dbaddr = dbdata.Item(j).InnerText;
                                 break;
                             case "port":
                                 dbport = dbdata.Item(j).InnerText;
                                 break;
                             case "user":
                                 dbuser = dbdata.Item(j).InnerText;
                                 break;
                             case "password":
                                 dbpass = dbdata.Item(j).InnerText;
                                 break;
                             case "database":
                                 dbname = dbdata.Item(j).InnerText;
                                 break;
                         }
                     }
                     string prefix = DatabasePack.contentDatabasePrefix;
                     switch (type)
                     {
                         case "admin":
                             prefix = DatabasePack.adminDatabasePrefix;
                             break;
                         case "atavism":
                             prefix = DatabasePack.atavismDatabasePrefix;
                             break;
                         case "master":
                             prefix = DatabasePack.masterDatabasePrefix;
                             break;
                         case "world_content":
                             prefix = DatabasePack.contentDatabasePrefix;
                             break;
                     }
                     EditorPrefs.SetString("databaseName" + prefix + DatabasePack.GetProjectName(), dbname);
                     EditorPrefs.SetString("databaseUser" + prefix + DatabasePack.GetProjectName(), dbuser);
                     EditorPrefs.SetString("databaseHost" + prefix + DatabasePack.GetProjectName(), dbaddr);
                     EditorPrefs.SetString("databasePort" + prefix + DatabasePack.GetProjectName(), dbport);
                     EditorPrefs.SetString("databasePassword" + prefix + DatabasePack.GetProjectName(), dbpass);

                     
                 }
             }

          }

      }

        // Load Database Information
        public static string LoadParameters(string prefix)
        {
            databaseHost = EditorPrefs.GetString("databaseHost" + prefix + GetProjectName());
            databasePort = EditorPrefs.GetString("databasePort" + prefix + GetProjectName());
            databaseName = EditorPrefs.GetString("databaseName" + prefix + GetProjectName());
            databaseUser = EditorPrefs.GetString("databaseUser" + prefix + GetProjectName());
            databasePassword = EditorPrefs.GetString("databasePassword" + prefix + GetProjectName());
            if (string.IsNullOrEmpty(databaseHost))
            {
                // Debug.LogError("Database Host is Empty");
                return "";
            }
            if (string.IsNullOrEmpty(databaseUser))
            {
                // Debug.LogError("Database Username is Empty");
                return "";
            }
            if (string.IsNullOrEmpty(databasePassword))
            {
                //  Debug.LogError("Database Password is Empty");
                return "";
            }

            string conString;
            conString = "Server=" + databaseHost;
            if (databasePort != "")
                conString += ";Port=" + databasePort;
            conString += ";Database=" + databaseName;
            conString += ";User ID=" + databaseUser;
            conString += ";Password=\"" + databasePassword + "\"";
            if (useUtf8mb4)
                conString += ";CharSet=utf8mb4";
            else
                conString += ";CharSet=utf8";
            conString += ";Pooling=true;Connect Timeout=15";

            return conString;
        }

        // Load Database Data
        public static List<Dictionary<string, string>> LoadData(string prefix, string query)
        {
            
            SaveLog(query);
            // Create a new list of fields
            List<Dictionary<string, string>> fieldRows = new List<Dictionary<string, string>>();

            // Connect to the database
            con = Connect(prefix);
            if (con == null)
            {
                return fieldRows;
            }
            // Try to get the data
            try
            {
                // Open the connection
                if (con.State.ToString() != "Open")
                    con.Open();

                // Use the connections to fetch data
                using (con)
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        // Execute the query
                        data = cmd.ExecuteReader();
                        // If there are collumns
                        if (data.HasRows)
                        {
                            int fieldsCount = data.FieldCount;
                            while (data.Read())
                            {
                                Dictionary<string, string> field = new Dictionary<string, string>();
                                for (int i = 0; i < fieldsCount; i++)
                                {
                                    string fieldName = data.GetName(i).ToString();
                                    string fieldValue = data[i].ToString();
                                    //Debug.Log("Name:" + fieldName + "=[" +  fieldValue + "]");
                                    field.Add(fieldName, fieldValue);
                                }
                                fieldRows.Add(field);
                            }
                        }
                        data.Dispose();
                    }
                }
                return fieldRows;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                SaveLog("Database LoadData: " + ex.ToString()+"\n prefix= "+prefix+"\n query= "+query);
                return null;
            }
            finally
            {
            }
        }

        // Update existing entries in the table based on the iddemo_table
        public static void Update(string prefix, string query, List<Register> registers)
        {

            string logs = "\nParams:";
            foreach (Register fieldUpdate in registers)
            {
                logs += "\n" + fieldUpdate.fieldName + ": " + fieldUpdate.fieldValue;
            }
            SaveLog(query+logs);

            // Connect to the database
            con = Connect(prefix);

            // Try to get the data
            try
            {
                // Open the connection
                if (con.State.ToString() != "Open")
                {
                    con.Open();
                }

                // Use the connections to fetch data
                using (con)
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        foreach (Register fieldUpdate in registers)
                        {
                            MySqlParameter oParam = cmd.Parameters.Add(fieldUpdate.fieldReference, fieldUpdate.mysqlType);
                            //Debug.Log("Name:"+fieldUpdate.fieldName+"R:"+fieldUpdate.fieldReference+"R:"+fieldUpdate.mysqlType+"V"+fieldUpdate.fieldValue);
                            if (fieldUpdate.fieldValue != null && fieldUpdate.fieldValue != "")
                            {
                                switch (fieldUpdate.fieldType)
                                {
                                    case Register.TypesOfField.Bool:
                                        if (bool.Parse(fieldUpdate.fieldValue))
                                            oParam.Value = 1;
                                        else
                                            oParam.Value = 0;
                                        break;
                                    case Register.TypesOfField.Float:
                                        oParam.Value = float.Parse(fieldUpdate.fieldValue);
                                        break;
                                    case Register.TypesOfField.Int:
                                        oParam.Value = int.Parse(fieldUpdate.fieldValue);
                                        break;
                                    case Register.TypesOfField.String:
                                        oParam.Value = fieldUpdate.fieldValue;
                                        break;
                                }
                            }
                            else
                            {
                                switch (fieldUpdate.fieldType)
                                {
                                    case Register.TypesOfField.Bool:
                                        oParam.Value = 0;
                                        break;
                                    case Register.TypesOfField.Float:
                                        oParam.Value = float.Parse("0");
                                        break;
                                    case Register.TypesOfField.Int:
                                        oParam.Value = int.Parse("0");
                                        break;
                                    case Register.TypesOfField.String:
                                        oParam.Value = "";
                                        break;
                                }
                            }
                        }
                        cmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                SaveLog("Database Update: " + ex.ToString());
            }
            finally
            {
            }
        }

        public static int Insert(string prefix, string query, List<Register> registers)
        {
            return Insert(prefix, query, registers, true);
        }

        // Insert a new entry existing entries in the table based on the iddemo_table
        public static int Insert(string prefix, string query, List<Register> registers, bool returnID)
        {
            string logs = "\nParams:";
            foreach (Register fieldUpdate in registers)
            {
                logs += "\n" + fieldUpdate.fieldName + ": " + fieldUpdate.fieldValue;
            }
            SaveLog(query + logs);
            // Connect to the database
            con = Connect(prefix);

            // Try to get the data
            try
            {
                // Open the connection
                if (con.State.ToString() != "Open")
                    con.Open();

                // Use the connections to fetch data
                using (con)
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        foreach (Register fieldUpdate in registers)
                        {
                            MySqlParameter oParam = cmd.Parameters.Add(fieldUpdate.fieldReference, fieldUpdate.mysqlType);
                            //Debug.Log("Name:"+fieldUpdate.fieldName+"R:"+fieldUpdate.fieldReference+"R:"+fieldUpdate.mysqlType+"V"+fieldUpdate.fieldValue);
                            if (fieldUpdate.fieldValue != null && fieldUpdate.fieldValue != "")
                            {
                                switch (fieldUpdate.fieldType)
                                {
                                    case Register.TypesOfField.Bool:
                                        oParam.Value = bool.Parse(fieldUpdate.fieldValue);
                                        break;
                                    case Register.TypesOfField.Float:
                                        oParam.Value = float.Parse(fieldUpdate.fieldValue);
                                        break;
                                    case Register.TypesOfField.Int:
                                        oParam.Value = int.Parse(fieldUpdate.fieldValue);
                                        break;
                                    case Register.TypesOfField.String:
                                        oParam.Value = fieldUpdate.fieldValue;
                                        break;
                                }
                            }
                            else
                            {
                                switch (fieldUpdate.fieldType)
                                {
                                    case Register.TypesOfField.Bool:
                                        oParam.Value = 0;
                                        break;
                                    case Register.TypesOfField.Float:
                                        oParam.Value = float.Parse("0");
                                        break;
                                    case Register.TypesOfField.Int:
                                        oParam.Value = int.Parse("0");
                                        break;
                                    case Register.TypesOfField.String:
                                        oParam.Value = "";
                                        break;
                                }
                            }
                        }
                        int result = cmd.ExecuteNonQuery();
                        if (result == -1 || !returnID)
                            return result;
                        else
                            return (int)cmd.LastInsertedId;
                    }
                }

            }
            catch (MySqlException e)
            {
                Debug.LogError(e.ToString());
                SaveLog("Database Insert: " + e.ToString());
                return -1;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                SaveLog("Database Insert: " + ex.ToString());
                return -1;
            }
            finally
            {
            }
        }

        public static int ExecuteNonQuery(string prefix, string query)
        {
            return ExecuteNonQuery(prefix, query, false);
        }

        public static int ExecuteNonQuery(string prefix, string query, int timeout)
        {
            return ExecuteNonQuery(prefix, query, false,timeout,true);
        }

        public static int ExecuteNonQuery(string prefix, string query, bool returnLastInsertedID)
        {
            return ExecuteNonQuery(prefix, query, returnLastInsertedID, 15,false);
        }

        // Insert a new entry existing entries in the table based on the iddemo_table
        public static int ExecuteNonQuery(string prefix, string query, bool returnLastInsertedID,int timeout,bool force)
        {
            SaveLog(query);

            // Connect to the database
            con = Connect(prefix,timeout,force);

            // Try to get the data
            try
            {
                // Open the connection
                if (con.State.ToString() != "Open")
                    con.Open();

                // Use the connections to fetch data
                using (con)
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                      //  cmd.CommandTimeout = timeout;
                        int result = cmd.ExecuteNonQuery();
                        if (returnLastInsertedID)
                        {
                            return (int)cmd.LastInsertedId;
                        }
                        else
                        {
                            return result;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                SaveLog("Database ExecuteNonQuery: " + ex.ToString());
                return -1;
            }
            finally
            {
            }
        }

        public static void Delete(string prefix, string table, List<Register> registers)
        {
            string query = "DELETE FROM " + table + " WHERE ";
            int i = 0;
            string logs = "\nParams:";
            foreach (Register field in registers)
            {
                query += field.fieldName + "=" + field.fieldReference;
                logs += "\n" + field.fieldName + ": " + field.fieldValue;
                if (i < registers.Count-1)
                    query += " AND ";
                i++;
            }
          
          
            SaveLog(query + logs);
          
            // Connect to the database
            con = Connect(prefix);

            // Try to get the data
            try
            {
                // Open the connection
                if (con.State.ToString() != "Open")
                    con.Open();

                // Use the connections to fetch data
                using (con)
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        foreach (Register regId in registers)
                        {
                            MySqlParameter oParam = cmd.Parameters.Add(regId.fieldReference, regId.mysqlType);
                            //Debug.Log("Name:"+fieldUpdate.fieldName+"R:"+fieldUpdate.fieldReference+"R:"+fieldUpdate.mysqlType+"V"+fieldUpdate.fieldValue);
                            if (regId.fieldValue != null && regId.fieldValue != "")
                            {
                                switch (regId.fieldType)
                                {
                                    case Register.TypesOfField.Bool:
                                        oParam.Value = bool.Parse(regId.fieldValue);
                                        break;
                                    case Register.TypesOfField.Float:
                                        oParam.Value = float.Parse(regId.fieldValue);
                                        break;
                                    case Register.TypesOfField.Int:
                                        oParam.Value = int.Parse(regId.fieldValue);
                                        break;
                                    case Register.TypesOfField.String:
                                        oParam.Value = regId.fieldValue;
                                        break;
                                }
                            }
                            else
                            {
                                switch (regId.fieldType)
                                {
                                    case Register.TypesOfField.Bool:
                                        oParam.Value = 0;
                                        break;
                                    case Register.TypesOfField.Float:
                                        oParam.Value = float.Parse("0");
                                        break;
                                    case Register.TypesOfField.Int:
                                        oParam.Value = int.Parse("0");
                                        break;
                                    case Register.TypesOfField.String:
                                        oParam.Value = "";
                                        break;
                                }
                            }






                        }
                        cmd.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                SaveLog("Database Delete: " + ex.ToString());
            }
            finally
            {
            }
        }


        public static void Delete(string prefix, string table, Register regId)
        {
            string query = "DELETE FROM " + table + " WHERE " + regId.fieldName + "=" + regId.fieldReference;
            SaveLog(query);

            // Connect to the database
            con = Connect(prefix);

            // Try to get the data
            try
            {
                // Open the connection
                if (con.State.ToString() != "Open")
                    con.Open();

                // Use the connections to fetch data
                using (con)
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        MySqlParameter oParam = cmd.Parameters.Add(regId.fieldReference, regId.mysqlType);
                        oParam.Value = int.Parse(regId.fieldValue);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                SaveLog("Database Delete: " + ex.ToString());
            }
            finally
            {
            }
        }


        // Delete when there is no ID as primary key
        public static void Delete(string prefix, string table, Register regId, bool noID)
        {
            string query = "DELETE FROM " + table + " WHERE " + regId.fieldName + "=" + regId.fieldReference;
            SaveLog(query);
            // Connect to the database
            con = Connect(prefix);

            // Try to get the data
            try
            {
                // Open the connection
                if (con.State.ToString() != "Open")
                    con.Open();

                // Use the connections to fetch data
                using (con)
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        MySqlParameter oParam = cmd.Parameters.Add(regId.fieldReference, regId.mysqlType);
                        oParam.Value = regId.fieldValue;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                SaveLog("Database Delete: " + ex.ToString());
            }
            finally
            {
            }
        }

       public static void SaveLog(string s)
        {
            try
            {
                long length = 0;
                if (File.Exists("AtavismEditor.log"))
                {
                    length = new System.IO.FileInfo("AtavismEditor.log").Length;
                    int i = 1;
                    if (length > 1000000)
                    {
                        bool done = false;
                        while (!done)
                        {
                            if (!File.Exists("AtavismEditor" + i + ".log"))
                            {
                                System.IO.File.Move("AtavismEditor.log", "AtavismEditor" + i + ".log");
                                done = true;
                            }
                            i++;
                        }
                    }
                }
                string tmp = string.Format("DATE: {0} : {1}", System.DateTime.Now, s);
                TextWriter tw = new StreamWriter("AtavismEditor.log", true);
                tw.WriteLine(tmp);
                tw.Close();
            }
            catch (Exception e)
            {
                Debug.LogError("Save Log Exception " + e);
            }
        }
    }
}