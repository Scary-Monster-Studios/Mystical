using UnityEngine;
using System.Collections;

namespace Atavism
{
    public class IntegrationPlugin : AtavismPlugin
    {

        // Use this for initialization
        public IntegrationPlugin()
        {
            pluginName = "Integration";
            //string serverCategory = "AT_button_category_integration";
            /*icon = (Texture)Resources.Load (serverCategory, typeof(Texture));
            iconOver = (Texture)Resources.Load (serverCategory + "_over", typeof(Texture));
            iconSelected = (Texture)Resources.Load (serverCategory + "_selected", typeof(Texture));
            icon.LoadImage(System.IO.File.ReadAllBytes("Assets\\AtavismUnity\\Editor\\Resources\\" + serverCategory + ".png"));
            iconOver.LoadImage(System.IO.File.ReadAllBytes("Assets\\AtavismUnity\\Editor\\Resources\\" + serverCategory + "_over.png"));
            iconSelected.LoadImage(System.IO.File.ReadAllBytes("Assets\\AtavismUnity\\Editor\\Resources\\" + serverCategory + "_selected.png"));*/
        }

        void Awake()
        {
            string serverCategory = "AT_button_category_integration";
            icon = (Texture)Resources.Load(serverCategory, typeof(Texture));
            iconOver = (Texture)Resources.Load(serverCategory + "_over", typeof(Texture));
            iconSelected = (Texture)Resources.Load(serverCategory + "_selected", typeof(Texture));
        }


        // Update is called once per frame
        void Update()
        {

        }
    }
}