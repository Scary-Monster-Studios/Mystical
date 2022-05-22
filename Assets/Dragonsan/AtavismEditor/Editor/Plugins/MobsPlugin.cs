using UnityEngine;
using System.Collections;

namespace Atavism
{
    public class MobsPlugin : AtavismPlugin
    {
        // Use this for initialization
        public MobsPlugin()
        {
            pluginName = "Mob";
        }

        void Awake()
        {
            string serverCategory = "AT_button_category_mob";
            icon = (Texture2D)Resources.Load(serverCategory, typeof(Texture));
            iconOver = (Texture2D)Resources.Load(serverCategory + "_over", typeof(Texture));
            iconSelected = (Texture2D)Resources.Load(serverCategory + "_selected", typeof(Texture));
        }
    }
}