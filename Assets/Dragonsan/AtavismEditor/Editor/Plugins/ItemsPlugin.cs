using UnityEngine;
using System.Collections;

namespace Atavism
{
    public class ItemsPlugin : AtavismPlugin
    {
        // Use this for initialization
        public ItemsPlugin()
        {
            pluginName = "Item";
        }

        void Awake()
        {
            string serverCategory = "AT_button_category_items";
            icon = (Texture)Resources.Load(serverCategory, typeof(Texture));
            iconOver = (Texture)Resources.Load(serverCategory + "_over", typeof(Texture));
            iconSelected = (Texture)Resources.Load(serverCategory + "_selected", typeof(Texture));
        }

    }
}