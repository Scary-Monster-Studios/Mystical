using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{

    public class InteractiveObjectsManager : MonoBehaviour
    {

        static InteractiveObjectsManager instance;

        Dictionary<int, InteractiveObject> interactiveObjects = new Dictionary<int, InteractiveObject>();
        Dictionary<int, bool> interactiveObjectsQueueSatus = new Dictionary<int, bool>();
        Dictionary<int, string> interactiveObjectsQueueState = new Dictionary<int, string>();


        // Use this for initialization
        void Start()
        {
            if (instance != null)
            {
                return;
            }
            instance = this;

            NetworkAPI.RegisterExtensionMessageHandler("interactive_object_state", HandleInteractiveObjectStateMessage);
            NetworkAPI.RegisterExtensionMessageHandler("start_interactive_task", HandleStarInteractionTask);
            NetworkAPI.RegisterExtensionMessageHandler("interactive_task_interrupted", HandleInterruptTask);
        }

        public void RegisterInteractiveObject(InteractiveObject iObj)
        {
            interactiveObjects[iObj.id] = iObj;
            if (interactiveObjectsQueueSatus.ContainsKey(iObj.id))
            {
                interactiveObjects[iObj.id].Active = interactiveObjectsQueueSatus[iObj.id];
                interactiveObjects[iObj.id].ResetHighlight();
                if (interactiveObjects[iObj.id].isLODChild)
                {
                    interactiveObjects[iObj.id].transform.parent.gameObject.SetActive(interactiveObjectsQueueSatus[iObj.id]);
                }
                else
                {
                    interactiveObjects[iObj.id].gameObject.SetActive(interactiveObjectsQueueSatus[iObj.id]);
                }

                if (interactiveObjectsQueueSatus[iObj.id])
                {
                    interactiveObjects[iObj.id].StateUpdated(interactiveObjectsQueueState[iObj.id]);
                }
                interactiveObjectsQueueSatus.Remove(iObj.id);
                interactiveObjectsQueueState.Remove(iObj.id);
            }
        }

        public void RemoveInteractiveObject(int id)
        {
            interactiveObjects.Remove(id);
        }

        public void HandleStarInteractionTask(Dictionary<string, object> props)
        {
           // ClientAPI.Write("Starting Interactive task with length: " + (float)props["length"]);
            float length = (float)props["length"];
            int  id = (int)props["intObjId"];
            string[] csArgs = new string[2];
            csArgs[0] = length.ToString();
            csArgs[1] = OID.fromLong(ClientAPI.GetPlayerOid()).ToString();
            AtavismEventSystem.DispatchEvent("CASTING_STARTED", csArgs);

          /*  Dictionary<string, object> props2 = new Dictionary<string, object>();
            props2.Add("gameObject", interactiveObjects[id].gameObject);
            CoordinatedEffectSystem.ExecuteCoordinatedEffect(interactiveObjects[id].activateCoordEffects[0].name, props2);*/
            //interactiveObjects[id].activateCoordEffects


        }
        public void HandleInterruptTask(Dictionary<string, object> props)
        {
            string[] args = new string[2];
            args[0] = "";
            args[1] = OID.fromLong(ClientAPI.GetPlayerOid()).ToString();
            AtavismEventSystem.DispatchEvent("CASTING_CANCELLED", args);

            ClientAPI.GetPlayerObject().MobController.PlayAnimation("", 0);
        }

        void HandleInteractiveObjectStateMessage(Dictionary<string, object> props)
        {
            int nodeID = (int)props["nodeID"];
            bool active = (bool)props["active"];
            string state = (string)props["state"];
            if (interactiveObjects.ContainsKey(nodeID))
            {
                interactiveObjects[nodeID].Active = active;
                interactiveObjects[nodeID].ResetHighlight();

                if (interactiveObjects[nodeID].isLODChild)
                {
                    interactiveObjects[nodeID].transform.parent.gameObject.SetActive(active);
                }
                else
                {
                    interactiveObjects[nodeID].gameObject.SetActive(active);
                }

                if (active)
                {
                    interactiveObjects[nodeID].StateUpdated(state);
                }
            }
            else
            {
                if (interactiveObjectsQueueSatus.ContainsKey(nodeID))
                    interactiveObjectsQueueSatus[nodeID] = active;
                else
                    interactiveObjectsQueueSatus.Add(nodeID, active);
                if (interactiveObjectsQueueState.ContainsKey(nodeID))
                    interactiveObjectsQueueState[nodeID] = state;
                else
                    interactiveObjectsQueueState.Add(nodeID, state);
            }
        }

        public InteractiveObject getInteractiveObject(int id)
        {
            if (interactiveObjects.ContainsKey(id))
            {
                return interactiveObjects[id];
            }

            return null;
        }
        
        public static InteractiveObjectsManager Instance
        {
            get
            {
                return instance;
            }
        }
    }
}