//using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;

namespace Atavism
{

    public class WorldEntry
    {
        public string worldId;
        public string worldName;
        public bool isDefault;

        public WorldEntry(string id, string name, bool def)
        {
            worldId = id;
            worldName = name;
            isDefault = def;
        }
    }

    /// <summary>
    ///   This class is made available to the browser control, and provides 
    ///   the interface for the browser to choose a world, login, and display
    ///   any status information required.
    /// </summary>
    public class AtavismLoginHelper
    {

        LoginSettings loginSettings;
        AtavismNetworkHelper networkHelper;
        string username = string.Empty;
        string password = string.Empty;
        string statusText = string.Empty;
        string errorText = string.Empty;
        string worldId = string.Empty;
        bool rememberUsername = false;
        bool fullScan = false;
        string preferredWorldId = string.Empty;
        string logFile = null;
        bool patchMedia = true;
        List<WorldEntry> worldEntries = new List<WorldEntry>();

        NetworkHelperStatus rv;

        public AtavismLoginHelper(LoginSettings loginSettings, AtavismNetworkHelper networkHelper, bool patchMedia)
        {
            AtavismLogger.LogInfoMessage("Login settings: " + loginSettings);
            //ClientAPI.Write ("Login settings: " + loginSettings);
            this.loginSettings = loginSettings;
            this.networkHelper = networkHelper;
            if (loginSettings.worldId != string.Empty)
                preferredWorldId = loginSettings.worldId;
            this.patchMedia = patchMedia;
        }

        public void ReloadSettings()
        {
            //ParseConfig ("../Config/login_settings.xml");
        }

        public bool CreateAccountMaster()
        {

            AtavismLogger.LogDebugMessage("RegisterButton_Click");

            StatusMessage = "Creating account ...";
            NetworkHelperStatus rv = CreateAccountRequest();
            string[] args = new string[1];
            args[0] = ErrorMessage;
            AtavismEventSystem.DispatchEvent("REGISTER_RESPONSE", args);
            return true;
        }

        protected NetworkHelperStatus CreateAccountRequest()
        {
            NetworkHelperStatus rv = networkHelper.CreateAccountMaster(loginSettings);
            AtavismLogger.LogInfoMessage("Register return: " + rv);
            switch (rv)
            {
                case NetworkHelperStatus.Success:
                    ErrorMessage = "Success";
                    break;
                case NetworkHelperStatus.UsernameUsed:
                    StatusMessage = "";
                    ErrorMessage = "UsernameUsed";
                    break;
                case NetworkHelperStatus.EmailUsed:
                    StatusMessage = "";
                    ErrorMessage = "EmailUsed";
                    break;
                case NetworkHelperStatus.Unknown:
                    StatusMessage = "";
                    ErrorMessage = "Unknown";
                    break;
                case NetworkHelperStatus.MasterTcpConnectFailure:
                    StatusMessage = "";
                    ErrorMessage = "MasterTcpConnectFailure";
                    break;
                case NetworkHelperStatus.NoAccessFailure:
                    StatusMessage = "";
                    ErrorMessage = "NoAccessFailure";
                    break;
                default:
                    StatusMessage = "";
                    ErrorMessage = "Unable to Create Account";
                    break;
            }
            return rv;
        }

        public bool LoginMaster(string worldId, ref int step)
        {
           string[] args = new string[1];
            if (step == 0)
            {
                StatusMessage = "Logging In ...";

                args[0] = StatusMessage;
                AtavismEventSystem.DispatchEvent("LOGIN_RESPONSE", args);
                LoginMaster();
                if (rv != NetworkHelperStatus.Success)
                {
                    AtavismLogger.LogInfoMessage("Got error: " + rv);
                    if (rv != NetworkHelperStatus.MasterTcpConnectFailure)
                    {
                        args = new string[1];
                        args[0] = ErrorMessage;
                        AtavismEventSystem.DispatchEvent("LOGIN_RESPONSE", args);
                    }
                    return false;
                }
            }
            AtavismLogger.LogInfoMessage("Moving to step 2");
            step = 2;
            StatusMessage = "Resolving World ...";
            args[0] = StatusMessage;
            AtavismEventSystem.DispatchEvent("LOGIN_RESPONSE", args);
            NetworkHelperStatus status = ResolveWorld();
            AtavismLogger.LogInfoMessage("Completed Resolve World with status: " + status);
            if (status != NetworkHelperStatus.WorldResolveSuccess)
            {
                if (status != NetworkHelperStatus.MasterTcpConnectFailure)
                {
                    args = new string[1];
                    args[0] = ErrorMessage;
                    AtavismEventSystem.DispatchEvent("LOGIN_RESPONSE", args);
                }
                return false;
            }
            return true;
        }

        protected void LoginMaster()
        {
            loginSettings.createAccount = false;
            rv = networkHelper.LoginMaster(loginSettings);
            AtavismLogger.LogInfoMessage("Login return: " + rv);
            switch (rv)
            {
                case NetworkHelperStatus.Success:
                    break;
                case NetworkHelperStatus.LoginFailure:
                    StatusMessage = "";
                    ErrorMessage = "LoginFailure";
                    break;
                case NetworkHelperStatus.NoAccessFailure:
                    StatusMessage = "";
                    ErrorMessage = "NoAccessFailure";
                    break;
                case NetworkHelperStatus.BannedFailure:
                    StatusMessage = "";
                    ErrorMessage = "BannedFailure";
                    break;
                case NetworkHelperStatus.SubscriptionExpiredFailure:
                    StatusMessage = "";
                    ErrorMessage = "SubscriptionExpiredFailure";
                    break;
                case NetworkHelperStatus.ServerMaintanance:
                    StatusMessage = "";
                    ErrorMessage = "ServerMaintanance";
                    break;
                case NetworkHelperStatus.MasterTcpConnectFailure:
                    //StatusMessage = "";
                    ErrorMessage = "";
                    break;
                default:
                    StatusMessage = "";
                    ErrorMessage = "Unable to login";
                    break;
            }
        }

        protected NetworkHelperStatus ResolveWorld()
        {
            // We may already have the world entry.. if so, skip the resolve
            if (networkHelper.HasWorldEntry(loginSettings.worldId))
            {
                //parentForm.DialogResult = DialogResult.OK;
                //parentForm.Close ();
                return NetworkHelperStatus.WorldResolveSuccess;
            }
            NetworkHelperStatus rv = networkHelper.ResolveWorld(loginSettings);
            switch (rv)
            {
                case NetworkHelperStatus.WorldResolveSuccess:
                    //parentForm.DialogResult = DialogResult.OK;
                    AtavismLogger.LogInfoMessage("Success: " + rv);
                    StatusMessage = "Success";
                    //parentForm.Close ();
                    break;
                case NetworkHelperStatus.WorldResolveFailure:
                    StatusMessage = "";
                    ErrorMessage = "Cannot find World Server";
                    break;
                case NetworkHelperStatus.MasterConnectFailure:
                    StatusMessage = "";
                    ErrorMessage = "Unable to connect to Master Server";
                    break;
                default:
                    StatusMessage = "";
                    ErrorMessage = "Unable to connect to World Server";
                    break;
            }
            return rv;
        }

        /// <summary>
        ///   Get the number of worlds that are locally defined (either from 
        ///   something akin to a quicklist, or passed on the command line).
        /// </summary>
        /// <returns></returns>
        public int GetWorldCount()
        {
            return worldEntries.Count;
        }
        /// <summary>
        ///   Get the world entry at a given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public WorldEntry GetWorldEntry(int index)
        {
            return worldEntries[index];
        }

        /// <summary>
        ///   This is used so that the client can pass in a preferred world
        ///   with a flag like --world_id and we can select the associated option
        ///   from the script in the login page.
        /// </summary>
        /// <returns>the preferred world id if it has been set, or the empty string if it has not</returns>
        public string GetPreferredWorld()
        {
            return preferredWorldId;
        }

        public bool NeedUpdate()
        {
            if (!patchMedia)
                return false;
            //return base.NeedUpdate ();
            return true;
        }

        public bool SetWorld(string worldId)
        {
            string savedWorldId = loginSettings.worldId;
            //ClientAPI.Write("Set world to: " + savedWorldId);
            if (!networkHelper.HasWorldEntry(loginSettings.worldId))
            {
                NetworkHelperStatus status = networkHelper.ResolveWorld(loginSettings);
                //for (int i = 0; i < 10; i++)
                //{
                AtavismLogger.LogDebugMessage("SetWorld status: " + status);
                //ClientAPI.Write("SetWorld status: " + status);
                if (status != NetworkHelperStatus.WorldResolveSuccess)
                {
                    // revert the loginSettings
                    loginSettings.worldId = savedWorldId;
                    //HandleConnectionFailed (this);
                    return false;
                }
                //}
            }
            WorldServerEntry entry = networkHelper.GetWorldEntry(worldId);
            return true;
        }

       
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

        public string StatusMessage
        {
            get
            {
                return statusText;
            }
            set
            {
                statusText = value;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return errorText;
            }
            set
            {
                errorText = value;
            }
        }

        public bool FullScan
        {
            get
            {
                return fullScan;
            }
            set
            {
                fullScan = value;
            }
        }

        public bool RememberUsername
        {
            get
            {
                return rememberUsername;
            }
            set
            {
                rememberUsername = value;
            }
        }

        public bool PatchMedia
        {
            get
            {
                return patchMedia;
            }
            set
            {
                patchMedia = value;
            }
        }

        public string WorldName
        {
            set
            {
                // If the player has changed world lets change the updating system.
                if (worldId != value)
                {
                    worldId = value;
                    loginSettings.worldId = worldId;
                    SetWorld(worldId);
                }
            }
        }

        public bool Reconnect
        {
            set
            {
                loginSettings.worldId = worldId;
                SetWorld(worldId);
            }
        }

        public bool LoginReady
        {
            get
            {
                //return loginReady;
                if (!patchMedia)
                    return true;
                return false;
            }
        }
    }

}