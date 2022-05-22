using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class UGUIAmmoFrame : MonoBehaviour
    {

        public List<GameObject> ammoSprites;
        public Sprite fullSprite;
        public Sprite emptySprite;
        string prop = "ammoCount";
        string propMax = "ammoCapacity";
        int count = 0;
        int capacity = 0;

        // Use this for initialization
        void Start()
        {
            ClientAPI.GetPlayerObject().RegisterPropertyChangeHandler(prop, PropHandler);
            ClientAPI.GetPlayerObject().RegisterPropertyChangeHandler(propMax, PropMaxHandler);
            if (ClientAPI.GetPlayerObject() != null)
            {
                if (ClientAPI.GetPlayerObject().PropertyExists(prop))
                {
                    count = (int)ClientAPI.GetPlayerObject().GetProperty(prop);
                }
                if (ClientAPI.GetPlayerObject().PropertyExists(propMax))
                {
                    capacity = (int)ClientAPI.GetPlayerObject().GetProperty(propMax);
                }
                UpdateAmmoCount();
            }
        }

        void OnDestroy()
        {
            ClientAPI.GetPlayerObject().RemovePropertyChangeHandler(prop, PropHandler);
            ClientAPI.GetPlayerObject().RemovePropertyChangeHandler(propMax, PropMaxHandler);
        }

        public void PropHandler(object sender, PropertyChangeEventArgs args)
        {
            count = (int)ClientAPI.GetPlayerObject().GetProperty(prop);
            UpdateAmmoCount();
        }

        public void PropMaxHandler(object sender, PropertyChangeEventArgs args)
        {
            capacity = (int)ClientAPI.GetPlayerObject().GetProperty(propMax);
            UpdateAmmoCount();
        }

        void UpdateAmmoCount()
        {
            for (int i = 0; i < ammoSprites.Count; i++)
            {
                if (i < capacity)
                {
                    ammoSprites[i].SetActive(true);
                    if (i < count)
                    {
                        ammoSprites[i].GetComponent<Image>().sprite = fullSprite;
                    }
                    else
                    {
                        ammoSprites[i].GetComponent<Image>().sprite = emptySprite;
                    }
                }
                else
                {
                    ammoSprites[i].SetActive(false);
                }
            }
        }
    }
}