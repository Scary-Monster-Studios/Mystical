using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUICharacterSelectSlot : MonoBehaviour
    {

        public Text Name;
        public TextMeshProUGUI TMPName;
        public Text classText;
        public TextMeshProUGUI TMPClass;
        public Text level;
        public TextMeshProUGUI TMPLevel;
        public Text race;
        public TextMeshProUGUI TMPRace;
        public Image portrait;
        public Button button;
        public PortraitType portraitType;
        public Image selectedIcon;
        Sprite normalSprite;
        Color normalColor;
        CharacterEntry character;
        bool selected = false;
        [SerializeField]
        Color selectedColorName = Color.green;
        [SerializeField]
        Color normalColorName = Color.white;
        // Use this for initialization
        void Awake()
        {
            //normalSprite = button.image.sprite;
            normalColor = button.colors.normalColor;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SelectCharacter()
        {
            CharacterSelectionCreationManager.Instance.CharacterSelected(character);
        }

        public void OnMouseEnter()
        {
            if (!selected)
            {
                button.image.color = button.colors.highlightedColor;
            }
        }

        public void OnMouseExit()
        {
            if (!selected)
            {
                button.image.color = normalColor;
            }
        }

        public void SetCharacter(CharacterEntry character)
        {
         /*   string paramsList = "[ ";
            foreach (string ss in character.Keys)
            {
                if (paramsList.Length > 2)
                    paramsList += " ; ";
                paramsList += ss + "=" + character[ss];
            }
            Debug.LogWarning(paramsList + " ]");*/
            this.character = character;
            if (Name != null)
                Name.text = (string)character["characterName"];
            if (TMPName != null)
                TMPName.text = (string)character["characterName"];
            if (classText != null)
                classText.text = (string)character["aspect"];
            if (TMPClass != null)
                TMPClass.text = (string)character["aspect"];
            if (race != null)
                race.text = (string)character["race"];
            if (TMPRace != null)
                TMPRace.text = (string)character["race"];
            if (level != null)
            {
                if (character.ContainsKey("level"))
                {
                    level.text = "Level " + (int)character["level"];
                }
                else
                {
                    level.text = "Level 1";
                }
            }
            if (TMPLevel != null)
            {
                if (character.ContainsKey("level"))
                {
                    TMPLevel.text = "Level " + (int)character["level"];
                }
                else
                {
                    TMPLevel.text = "Level 1";
                }
            }

            string gender = (string)character["gender"];
            int raceId = (int)character["raceId"];
            int aspectId = (int)character["aspectId"];
            int genderId = -1;
            if (character.ContainsKey("genderId"))
            {
                genderId = (int) character["genderId"];
            }
            else
            {
                foreach(var gen in AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList.Values)
                {
                    if (gen.name.Equals(gender))
                    {
                        genderId = gen.id;
                    }
                }
            }

            //Sprite portraitSprite = PortraitManager.Instance.GetCharacterSelectionPortrait(gender, (string)character["race"], (string)character["aspect"], portraitType);
            /*  if (PortraitManager.Instance.portraitType == PortraitType.Custom)
              {
                  Sprite[] icons = null;
                  if (gender == "Male") icons = AtavismSettings.Instance.meleAvatars;
                  if (gender == "Female") icons = AtavismSettings.Instance.femaleAvatars;
                  if (icons != null){
                      foreach(Sprite s in icons)
                      {
                          if (s.name.Equals((string)character["portrait"])) {
                              portrait.sprite = s;
                              break;
                          }
                      }
                  }
              }*/
            Sprite portraitSprite = character.ContainsKey("portrait") ?
              PortraitManager.Instance.LoadPortrait((string)character["portrait"]) : character.ContainsKey("custom:portrait") ? PortraitManager.Instance.LoadPortrait((string)character["custom:portrait"]) :
              PortraitManager.Instance.GetCharacterSelectionPortrait(genderId, raceId, aspectId, portraitType);
            if (portrait != null && portraitSprite != null)
            {
                portrait.sprite = portraitSprite;
            }

        }

        public void CharacterSelected(CharacterEntry selectedChar)
        {
            if (selectedChar.Equals(character))
            {
                //button.image.sprite = button.spriteState.pressedSprite;
                //button.image.color = button.colors.pressedColor;
                selected = true;
                if (selectedIcon != null)
                    selectedIcon.gameObject.SetActive(true);
                if (Name != null)
                    Name.color = selectedColorName;
                if (TMPName != null)
                    TMPName.color = selectedColorName;
            }
            else
            {
                //button.image.sprite = normalSprite;
                //			button.image.color = normalColor;
                selected = false;
                if (selectedIcon != null)
                    selectedIcon.gameObject.SetActive(false);
                if (Name != null)
                    Name.color = normalColorName;
                if (TMPName != null)
                    TMPName.color = normalColorName;

            }
        }
    }
}