using UnityEngine;
using UnityEditor;
using System.Collections;
namespace Atavism
{
    public class ServerPlugin : AtavismPlugin
    {

        // Use this for initialization
        public ServerPlugin()
        {
            pluginName = "Server";
           
        }

        void Awake()
        {
            string serverCategory = "AT_button_category_server";
            icon = (Texture2D)Resources.Load(serverCategory, typeof(Texture));
            iconOver = (Texture2D)Resources.Load(serverCategory + "_over", typeof(Texture));
            iconSelected = (Texture2D)Resources.Load(serverCategory + "_selected", typeof(Texture));
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnEnable()
        {
            ClearFunctions(); 
            RegisterFunction(ScriptableObject.CreateInstance<ServerDataBase>(), "Server");
            RegisterFunction(ScriptableObject.CreateInstance<ServerMobSpawner>(), "Server");
            RegisterFunction(ScriptableObject.CreateInstance<ServerResourceNodes>(), "Server");
            RegisterFunction(ScriptableObject.CreateInstance<ServerInstanceObjects>(), "Server");
            RegisterFunction(ScriptableObject.CreateInstance<ServerBuildingColliders>(), "Server");
            RegisterFunction(ScriptableObject.CreateInstance<ServerIntegrations>(), "Integration");
        }

    }
}