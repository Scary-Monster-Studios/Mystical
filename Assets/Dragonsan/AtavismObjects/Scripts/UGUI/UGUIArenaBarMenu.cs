using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Atavism
{
    public class UGUIArenaBarMenu : MonoBehaviour
    {

        //   [SerializeField] List<Button> areny = new List<Button>();
        public List<int> queued = new List<int>();

        void Start()
        {
            NetworkAPI.RegisterExtensionMessageHandler("Arena_Queued", HandleArenaQueued);
        }
        void OnDestroy()
        {
            NetworkAPI.RemoveExtensionMessageHandler("Arena_Queued", HandleArenaQueued);
        }

        public void HandleArenaQueued(Dictionary<string, object> props)
        {
            int numArena = (int)props["numArenasQueued"];
            queued.Clear();
            for (int i = 0; i < numArena; i++)
            {
                int q = (int)props["arenaTemp" + i];
                if (!queued.Contains(q))
                    queued.Add(q);
            }
            UpdateButons();
        }
        void UpdateButons()
        {

        }
        // Update is called once per frame
        void Update()
        {

        }
        public void ClickArena(int i)
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            props.Add("arenaType", 1);
            props.Add("arenaTemp", i);
            if (!queued.Contains(i))
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "arena.joinQueue", props);
            else
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "arena.leaveQueue", props);

            ClientAPI.Write("Sent arena message");
        }





    }
}