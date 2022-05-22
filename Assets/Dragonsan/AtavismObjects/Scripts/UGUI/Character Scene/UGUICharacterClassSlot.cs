using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Atavism
{

    public class UGUICharacterClassSlot : MonoBehaviour
    {

        public Button button;
        Sprite normalSprite;
        Color normalColor;
      //  public AtavismClassData classData;
        bool selected = false;
        public Image iconImage;
        public Image SelectedImage;
        public GameObject rootGameObject;
        private int classId = -1;

        // Use this for initialization
        void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();
         /*   if (iconImage != null && iconImage.sprite != classData.femaleClassIcon)
                iconImage.sprite = classData.maleClassIcon;

            button.image.sprite = classData.maleClassIcon;*/
            //normalSprite = button.image.sprite;
            normalColor = button.colors.normalColor;
        }

        void Update()
        {
            if (selected)
            {
                if (SelectedImage != null)
                    SelectedImage.color = CharacterSelectionCreationManager.Instance.selectedButomTextColor;
                else if (button != null)
                    button.image.color = button.colors.pressedColor;
            }
            else
            {
                if (SelectedImage != null)
                    SelectedImage.color = CharacterSelectionCreationManager.Instance.defaultButomTextColor;
                else if (button != null)
                    button.image.color = normalColor;
            }
        }
        public void SetClass(int raceId,int classId)
        {
            this.classId = classId;
            if (AtavismPrefabManager.Instance.GetRaceData().ContainsKey(raceId))
            {
               
                if (button != null)
                    button.image.sprite = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[classId].icon;
            }
        }

     

        public void SelectClass()
        {
            CharacterSelectionCreationManager.Instance.SetCharacterClass(classId);
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
        public void ClassSelected(int selectedClass)
        {
            if (selectedClass == classId)
            {
                if (SelectedImage != null)
                    SelectedImage.color = CharacterSelectionCreationManager.Instance.selectedButomTextColor;
                else if (button != null)
                    button.image.color = button.colors.pressedColor;
                selected = true;
            }
            else
            {
                if (SelectedImage != null)
                    SelectedImage.color = CharacterSelectionCreationManager.Instance.defaultButomTextColor;
                else if (button != null)
                    button.image.color = normalColor;
                selected = false;
            }
        }

       
    }
}