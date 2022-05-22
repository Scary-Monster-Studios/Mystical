using UnityEngine;
using System.Collections;

namespace Atavism
{
    public class CombatPlugin : AtavismPlugin
    {

        // Use this for initialization
        public CombatPlugin()
        {
            pluginName = "Combat";
            //string serverCategory = "AT_button_category_combat";
            /*icon = (Texture)Resources.Load (serverCategory, typeof(Texture));
            iconOver = (Texture)Resources.Load (serverCategory + "_over", typeof(Texture));
            iconSelected = (Texture)Resources.Load (serverCategory + "_selected", typeof(Texture));
            icon.LoadImage(System.IO.File.ReadAllBytes("Assets\\AtavismUnity\\Editor\\Resources\\" + serverCategory + ".png"));
            iconOver.LoadImage(System.IO.File.ReadAllBytes("Assets\\AtavismUnity\\Editor\\Resources\\" + serverCategory + "_over.png"));
            iconSelected.LoadImage(System.IO.File.ReadAllBytes("Assets\\AtavismUnity\\Editor\\Resources\\" + serverCategory + "_selected.png"));*/
        }

        void Awake()
        {
            string serverCategory = "AT_button_category_combat";
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