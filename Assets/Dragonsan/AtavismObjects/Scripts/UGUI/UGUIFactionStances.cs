using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atavism
{
    public class UGUIFactionStances : MonoBehaviour
    {

        [SerializeField] List<UGUIFactionStancesSlot> factions = new List<UGUIFactionStancesSlot>();
        List<string> dKeys = new List<string>();
        List<string> dKeysRegistered = new List<string>();
        [SerializeField] UGUIFactionStancesSlot prefab;
        AtavismObjectNode node;
        private bool showing = false;

        // Use this for initialization
        void Start()
        {
            if (ClientAPI.GetPlayerObject() != null)
            {
                node = ClientAPI.GetObjectNode(ClientAPI.GetPlayerOid());
                foreach (string key in ClientAPI.GetPlayerObject().Properties.Keys)
                {
                    if (key.Contains("Reputation"))
                    {
                        if (!key.EndsWith("_t"))
                            if (!dKeys.Contains(key))
                                dKeys.Add(key);
                        //    Debug.LogError(key);
                    }

                }
                if (node != null)
                {
                    foreach (string s in dKeys)
                    {
                        node.RegisterPropertyChangeHandler(s, reputationHandler);
                        dKeysRegistered.Add(s);
                    }
                }

                UpdateDisplay();
            }
            Hide();
        }

        private void reputationHandler(object sender, PropertyChangeEventArgs args)
        {

            UpdateDisplay();
        }

        // Update is called once per frame
        void UpdateDisplay()
        {
            int i = 0;
            foreach (string s in dKeys)
            {
                if (i >= factions.Count)
                    factions.Add(Instantiate(prefab, transform));
                factions[i].gameObject.SetActive(true);
                string faction = (string)ClientAPI.GetPlayerObject().GetProperty(s);
                string[] fac = faction.Split(new Char[] { ' ' });
                if (fac.Length == 3)
                {
                    factions[i].UpdateDisplay(fac[1], int.Parse(fac[fac.Length - 1]));
                }
                else
                {
                    string facName = "";
                    for (int j = 1; j < fac.Length - 2; j++)
                    {
                        facName += fac[j];
                        if (j < fac.Length - 3)
                            facName += " ";
                    }
                    factions[i].UpdateDisplay(facName, int.Parse(fac[fac.Length - 1]));
                }
                i++;
            }
            for (int j = i; j < factions.Count; j++)
                factions[i].gameObject.SetActive(false);

        }
        private void OnDestroy()
        {
            if (node != null)
            {
                foreach (string s in dKeysRegistered)
                {
                    node.RemovePropertyChangeHandler(s, reputationHandler);
                }
                dKeysRegistered.Clear();
            }
        }


        public void Show()
        {
            AtavismUIUtility.BringToFront(gameObject);
            UpdateDisplay();
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
            showing = true;

        }

        public void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            showing = false;
        }



        public void Toggle()
        {
            if (showing)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

    }
}