using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Atavism
{

    public class UGUICharacterRaceSlot : MonoBehaviour
    {

        public Button button;
        Sprite normalSprite;
        Color normalColor;
     //   public AtavismRaceData raceData;
        bool selected = false;
        public Image iconImage;
        public Image SelectedImage;
        public GameObject rootGameObject;
        int raceId =-1;
        // Use this for initialization
        void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();
             normalColor = button.colors.normalColor;
        }

        // Update is called once per frame
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

        public void SelectRace()
        {
           // CharacterSelectionCreationManager.Instance.SetCharacterRace(raceData);
            CharacterSelectionCreationManager.Instance.SetCharacterRace(raceId);
            
        }

        public void SetRace(int raceId)
        {
            this.raceId = raceId;
            if (AtavismPrefabManager.Instance.GetRaceData().ContainsKey(raceId))
            {
               
                if (button != null)
                    button.image.sprite = AtavismPrefabManager.Instance.GetRaceData()[raceId].icon;
            }
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

        public void RaceSelected(int selectedRace)
        {
            if (selectedRace == raceId)
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