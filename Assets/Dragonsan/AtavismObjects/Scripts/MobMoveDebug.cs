using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atavism
{

    public class MobMoveDebug : MonoBehaviour
    {
        public GameObject positionPrefab;
        public float positionMarkerDestroyDealy = 25f;
        public GameObject dstPositionPrefab;
        public float dstPositionMarkerDestroyDealy = 40f;
        public GameObject plyPositionPrefab;
        public GameObject plyPositionPrefab2;
        public GameObject pathPositionPrefab;

        // Queue<GameObject> pull = new Queue<GameObject>();
        //List<GameObject> list = new List<GameObject>();
        // Start is called before the first frame update
        void Start()
        {
            NetworkAPI.RegisterExtensionMessageHandler("mobposition", HandleMobPositionMessage);
            NetworkAPI.RegisterExtensionMessageHandler("mobdstposition", HandleMobDstPositionMessage);
            NetworkAPI.RegisterExtensionMessageHandler("debugMobMoveTarget", HandleDebugMobMoveTargetMessage);
            NetworkAPI.RegisterExtensionMessageHandler("pathPoints", HandleDebugMobMovePathMessage);
        }

        private void HandleMobPositionMessage(Dictionary<string, object> props)
        {

            float x = (float)props["x"];
            float y = (float)props["y"];
            float z = (float)props["z"];
            OID mob = (OID)props["mob"];
            if (positionPrefab != null)
            {
                GameObject go = Instantiate(positionPrefab, new Vector3(x, y, z), Quaternion.identity);
                //  go.hideFlags = HideFlags.HideAndDontSave;
                Destroy(go, positionMarkerDestroyDealy);
            }
            
        }

        private void HandleDebugMobMovePathMessage(Dictionary<string, object> props)
        {
            int num = (int) props["numPoints"];
            Debug.LogError("HandleDebugMobMovePathMessage points=" + num);
            if (pathPositionPrefab != null)
            {
                for (int i = 0; i < num; i++)
                {
                    float x = (float) props["p" + i + "X"];
                    float y = (float) props["p" + i + "Y"];
                    float z = (float) props["p" + i + "Z"];

                    GameObject go = Instantiate(pathPositionPrefab, new Vector3(x, y, z), Quaternion.identity);
                    //  go.hideFlags = HideFlags.HideAndDontSave;
                    Destroy(go, positionMarkerDestroyDealy);
                }
            }

        }

        private void HandleMobDstPositionMessage(Dictionary<string, object> props)
        {

            float x = (float)props["x"];
            float y = (float)props["y"];
            float z = (float)props["z"];
            OID mob = (OID)props["mob"];
            if (dstPositionPrefab != null)
            {
                GameObject go = Instantiate(dstPositionPrefab, new Vector3(x, y, z), Quaternion.identity);
                //  go.hideFlags = HideFlags.HideAndDontSave;
                Destroy(go, dstPositionMarkerDestroyDealy);
            }
        }
        private void HandleDebugMobMoveTargetMessage(Dictionary<string, object> props)
        {

            float x = (float)props["x"];
            float y = (float)props["y"];
            float z = (float)props["z"];
            float dx = (float)props["dx"];
            float dy = (float)props["dy"];
            float dz = (float)props["dz"];
            Vector3 vp = new Vector3(x, y, z);
            Vector3 vd = vp +new Vector3(dx, dy, dz);

            OID mob = (OID)props["mob"];
            if (plyPositionPrefab != null)
            {
                GameObject go = Instantiate(plyPositionPrefab, vp, Quaternion.identity);
                  go.transform.LookAt(vd);
                Destroy(go, dstPositionMarkerDestroyDealy);
                if (plyPositionPrefab2 != null)
                {

                    go = Instantiate(plyPositionPrefab2, vd, Quaternion.identity);
                    //  go.hideFlags = HideFlags.HideAndDontSave;
                    Destroy(go, dstPositionMarkerDestroyDealy);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}