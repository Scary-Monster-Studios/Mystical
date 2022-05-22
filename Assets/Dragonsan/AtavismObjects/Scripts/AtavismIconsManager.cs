using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atavism
{
    [Serializable]
    public class IconSettings
    {
        public string name = "";
        public Sprite sprite;
    }
    public class AtavismIconsManager : ScriptableObject
    {
        [Header("Atavism Icons Manager")]
        [SerializeField]
        private List<IconSettings> List;
        public Sprite defaultSptite;
        public Sprite GetIcon(string path)
        {
            foreach (IconSettings info in List)
            {
                if (info.name == path)
                {
                    return info.sprite;
                }
            }

            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("iconPath", path);
            NetworkAPI.SendExtensionMessage(0, false, "rep.GET_ICON", props);
            Debug.Log("Not found  icon with this path: " + path);
            return defaultSptite;
        }
    }
}