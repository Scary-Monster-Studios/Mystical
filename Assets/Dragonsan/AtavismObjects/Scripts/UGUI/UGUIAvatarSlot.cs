using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Atavism
{

    public class UGUIAvatarSlot : MonoBehaviour
    {
        [SerializeField] Image slotOverlay;
        public Image avatarIcon;
        int slotNumber;
        [SerializeField] Button button;
        Color normalColor = Color.white;

        // Use this for initialization
        void Start()
        {
            //  button = GetComponent<Button>();
            normalColor = button.colors.normalColor;

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void SetSlotNumber(int number)
        {
            slotNumber = number;
        }

        public void SelectAvatar()
        {
            UGUIAvatarList.Instance.SelectAvatar(slotNumber);
        }
        public void Selected(int id)
        {
            //      Debug.LogError("Avatar Slot " + id + " | " + slotNumber);
            if (id == slotNumber)
            {
                slotOverlay.enabled = true;
                slotOverlay.color = CharacterSelectionCreationManager.Instance.selectedButomTextColor;
            }
            else
            {
                slotOverlay.enabled = false;
                slotOverlay.color = CharacterSelectionCreationManager.Instance.defaultButomTextColor;
            }
        }
    }
}