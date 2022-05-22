using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Atavism
{
    // This is the base class for Atavism Plugins
    public class AtavismPlugin : ScriptableObject
    {
        public string pluginName;
        public Texture icon;// = new Texture2D(1, 1);
        public Texture iconOver;// = new Texture2D(1, 1);
        public Texture iconSelected;// = new Texture2D(1, 1);

        public List<AtavismFunction> pluginFunctions = new List<AtavismFunction>();

        // Use this for initialization
        public AtavismPlugin()
        {
            pluginName = "Base Atavism Plugin";
            pluginFunctions.Clear();

        }

        // Update is called once per frame
        void Update()
        {

        }

        // Register a plugin function 
        public void RegisterFunction(AtavismFunction func, string category)
        {
            // The function class name is PluginName+FunctionName
            //string className = pluginName + function;
            //AtavismFunction temp = (AtavismFunction) System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(className); // AtavismFunction.CreateInstance(className);
            //EditorUtility.
            func.functionCategory = category;
            pluginFunctions.Add(func);
        }
        public void ClearFunctions()
        {
            pluginFunctions.Clear();
        }

        // List all the plugin functions
        public AtavismFunction[] ListFunctions()
        {
            AtavismFunction[] list = new AtavismFunction[pluginFunctions.Count];

            for (int i = 0; i < pluginFunctions.Count; i++)
            {
                list[i] = pluginFunctions[i];
            }
            return list;
        }

    }
}