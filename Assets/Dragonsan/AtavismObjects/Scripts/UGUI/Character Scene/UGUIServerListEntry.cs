using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUIServerListEntry : MonoBehaviour
    {

        public Text serverName;
        public TextMeshProUGUI TMPServerName;
        public Text serverType;
        public TextMeshProUGUI TMPServerType;
        public Text serverPopulation;
        public TextMeshProUGUI TMPServerPopulation;
        WorldServerEntry entry;
        UGUIServerList serverList;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetServerDetails(WorldServerEntry entry, UGUIServerList serverList)
        {
            this.entry = entry;
            if (entry.Name == AtavismClient.Instance.WorldId)
            {
                if (serverName != null)
                    this.serverName.text = entry.Name + "(current)";
                if (TMPServerName != null)
                    this.TMPServerName.text = entry.Name + "(current)";
                GetComponent<Button>().interactable = false;
            }
            else
            {
                string status = (string)entry["status"];
                if (status != "Online")
                {
                    if (serverName != null)
                        this.serverName.text = entry.Name + " (" + status + ")";
                    if (TMPServerName != null)
                        this.TMPServerName.text = entry.Name + " (" + status + ")";
                    GetComponent<Button>().interactable = false;
                }
                else
                {
                    if (serverName != null)
                        this.serverName.text = entry.Name;
                    if (TMPServerName != null)
                        this.TMPServerName.text = entry.Name;
                    GetComponent<Button>().interactable = true;
                }
            }

            if (serverType != null)
                this.serverType.text = "";
            if (TMPServerType != null)
                this.TMPServerType.text = "";
            if (serverPopulation != null)
                this.serverPopulation.text = "";
            if (TMPServerPopulation != null)
                this.TMPServerPopulation.text = "";
            this.serverList = serverList;
        }

        public void ServerSelected()
        {
            serverList.SelectEntry(entry);
            //EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}