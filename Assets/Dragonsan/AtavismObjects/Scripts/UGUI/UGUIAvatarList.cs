using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

namespace Atavism
{

    public class UGUIAvatarList : MonoBehaviour
    {
        protected static UGUIAvatarList instance;
        [SerializeField] GridLayoutGroup slotGrid;
        [SerializeField] UGUIAvatarSlot avatarPrefab;
        List<UGUIAvatarSlot> slots = new List<UGUIAvatarSlot>();
        int selected = 0;
        public Sprite[] icons = null;

        // Use this for initialization
        void Start()
        {
            if (instance != null)
            {
                GameObject.DestroyImmediate(gameObject);
                return;
            }
            instance = this;
         //   gameObject.SetActive(false);
        }

        internal void SelectAvatar(int slotNumber)
        {
            this.selected = slotNumber;
            foreach (UGUIAvatarSlot slot in slots)
            {
                slot.Selected(selected);
            }
            CharacterSelectionCreationManager.Instance.AvatarSelected();
        }

        public void PreparSlots(string Race, string Gender, string Class)
        {
            this.selected = 0;
            slots.Clear();
            foreach (Transform child in slotGrid.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            icons = AtavismSettings.Instance.Avatars(Race,Gender,Class);
            /*if (gender == "Male")
                icons = AtavismSettings.Instance.meleAvatars;
            if (gender == "Female")
                icons = AtavismSettings.Instance.femaleAvatars;*/
            if (icons != null)
                for (int k = 0; k < icons.Length; k++)
                {
                    UGUIAvatarSlot slot = (UGUIAvatarSlot)Instantiate(avatarPrefab, slotGrid.transform, false);
                    slot.transform.localScale = Vector3.one;
                    slot.avatarIcon.sprite = icons[k];
                    slot.SetSlotNumber(k);
                    slot.Selected(selected);
                    slots.Add(slot);
                }
            if (slots.Count > 0)
            {
                System.Random rand = new System.Random();
                SelectAvatar(rand.Next(slots.Count - 1));
            }
        }

        public int Selected()
        {
            return this.selected;
        }

        public static UGUIAvatarList Instance
        {
            get
            {
                return instance;
            }
        }
    }
}