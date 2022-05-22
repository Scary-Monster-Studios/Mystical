using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atavism
{


    public class UGUICharacterGenderSlot : MonoBehaviour
    {
        public Button button;
        public Image iconImage;
        public Image SelectedImage;
        public GameObject rootGameObject;
        bool selected = false;
        private int genderId = -1;
        Sprite normalSprite;
        Color normalColor;

        void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();
            normalColor = button.colors.normalColor;
        }

        public void SetGender(int raceId,int classId, int genderId)
        {
         //   Debug.LogError("SetGender: Rece "+raceId+" Class "+ classId+" Gender "+genderId);
            this.genderId = genderId;
            if (AtavismPrefabManager.Instance.GetRaceData().ContainsKey(raceId))
            {
               
                if (button != null)
                    button.image.sprite = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[classId].genderList[genderId].icon;
            }
        }
        
        public void SelectGender()
        {
            // CharacterSelectionCreationManager.Instance.SetCharacterClass(classData);
            CharacterSelectionCreationManager.Instance.SetCharacterGender(genderId);
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

        public void GenderSelected(int selectedGender)
        {
            if (selectedGender == genderId)
            {
                //button.image.sprite = button.spriteState.pressedSprite;
                if (SelectedImage != null)
                    SelectedImage.color = CharacterSelectionCreationManager.Instance.selectedButomTextColor;
                else if (button != null)
                    button.image.color = button.colors.pressedColor;
                selected = true;
            }
            else
            {
                //button.image.sprite = normalSprite;
                if (SelectedImage != null)
                    SelectedImage.color = CharacterSelectionCreationManager.Instance.defaultButomTextColor;
                else if (button != null)
                    button.image.color = normalColor;
                selected = false;
            }
        }
    }
}