using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{

    public enum PortraitType
    {
        Prefab,
        Class, // This will get saved to the player
        Custom
    }

    public class PortraitManager : MonoBehaviour
    {

        static PortraitManager instance;
        public PortraitType portraitType;

        void Start()
        {
            if (instance != null)
            {
                return;
            }
            instance = this;
        }

        /// <summary>
        /// Gets the portrait for Character Selection Slots. Can only be used in the Character Selection Scene.
        /// </summary>
        /// <returns>The character selection portrait.</returns>
        /// <param name="gender">Gender Id.</param>
        /// <param name="raceName">Race Id.</param>
        /// <param name="className">Class Id.</param>
        /// <param name="portraitType">Portrait type.</param>
        public Sprite GetCharacterSelectionPortrait(int genderId, int raceId, int classId, PortraitType portraitType)
        {
            if (portraitType == PortraitType.Prefab)
            {
                return GetRacePortrait(genderId, raceId,classId);
            }
            else if (portraitType == PortraitType.Class)
            {
                return GetClassPortrait(genderId,raceId, classId);
            }
            else if (portraitType == PortraitType.Custom)
            {
                //TODO: Add your code here?
                return null;
            }
            return null;
        }

       
        Sprite GetRacePortrait(int genderId, int raceId, int classId)
        {
            if (AtavismPrefabManager.Instance.GetRaceData().ContainsKey(raceId))
            {
                return AtavismPrefabManager.Instance.GetRaceData()[raceId].icon;
            }

            return null;
        }
       
        Sprite GetClassPortrait(int genderId, int raceId, int classId)
        {
            if (AtavismPrefabManager.Instance.GetRaceData().ContainsKey(raceId))
            {
                if (AtavismPrefabManager.Instance.GetRaceData()[raceId].classList.ContainsKey(classId))
                {
                    return AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[classId].icon;
                }
            }
                
           
            return null;
        }

        /// <summary>
        /// Loads the portrait for a player/mob in-game.
        /// </summary>
        /// <returns>The portrait.</returns>
        /// <param name="node">Node.</param>
        public Sprite LoadPortrait(AtavismNode node)
        {
            if (node == null)
                return null;
            if (portraitType == PortraitType.Class && node.PropertyExists("portrait"))
            {
                // Class portraits will currently be saved on the player
                string portraitName = (string)node.GetProperty("portrait");
                Sprite portraitSprite = Resources.Load<Sprite>("Portraits/" + portraitName);
                return portraitSprite;
            }
            else if (portraitType == PortraitType.Prefab || portraitType == PortraitType.Class)
            {
                // If the target doesn't have a portrait property, fall back to prefab
                if (node.GameObject != null && node.GameObject.GetComponent<AtavismMobAppearance>() != null)
                    return node.GameObject.GetComponent<AtavismMobAppearance>().portraitIcon;
            }
            else if (portraitType == PortraitType.Custom)
            {
                // TODO: Add your code here?
                if (node.PropertyExists("portrait"))
                {
                    string portraitName = (string)node.GetProperty("portrait");
                    Sprite portraitSprite = Resources.Load<Sprite>("Portraits/" + portraitName);
                    return portraitSprite;
                }
                if (node.GameObject != null && node.GameObject.GetComponent<AtavismMobAppearance>() != null)
                    return node.GameObject.GetComponent<AtavismMobAppearance>().portraitIcon;

            }
            return null;
        }

        public Sprite LoadPortrait(string portraitName)
        {
            Sprite portraitSprite = null;
         /*   Sprite[] icons = AtavismSettings.Instance.meleAvatars;
            foreach (Sprite s in icons)
            {
                if (s != null)
                    if (s.name.Equals(portraitName))
                        portraitSprite = s;
            }
            if (portraitSprite == null)
            {
                icons = AtavismSettings.Instance.femaleAvatars;
                foreach (Sprite s in icons)
                {
                    if (s != null)
                        if (s.name.Equals(portraitName))
                            portraitSprite = s;
                }

            }
            if (portraitSprite == null)
            */
                portraitSprite = AtavismSettings.Instance.Avatar(portraitName);
            




            if (portraitSprite == null)
                portraitSprite = Resources.Load<Sprite>("Portraits/" + portraitName);
            return portraitSprite;
        }


        public static PortraitManager Instance
        {
            get
            {
                return instance;
            }
        }
    }

}