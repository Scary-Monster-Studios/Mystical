using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atavism
{
   public class DebugDynamidNavMesh : MonoBehaviour
   {
       public GameObject box;
       public GameObject sphere;
       public GameObject capsule;

       public List<GameObject> objects = new List<GameObject>();
        // Start is called before the first frame update
        void Start()
        {
            NetworkAPI.RegisterExtensionMessageHandler("DNM_colliders", HandleNMMessage);
        }

        private void OnDestroy()
        {
            NetworkAPI.RemoveExtensionMessageHandler("DNM_colliders", HandleNMMessage);
        }

        private void HandleNMMessage(Dictionary<string, object> props)
        {
            bool clear = (bool)props["clear"];
            Debug.LogError("HandleNMMessage clear="+clear);
            if (clear && objects.Count > 0)
            {
                foreach (var obj in objects)
                {
                    Destroy(obj);
                }
                objects.Clear();
            }
            int num = (int)props["num"];
            for (int i = 0; i < num; i++)
            {
                int col = (int)props["o"+i+"num"];
                Vector3 pos = Vector3.zero;
                Vector3 he1 = Vector3.zero;
                Vector3 he2 = Vector3.zero;
                Vector3 he3 = Vector3.zero;

                for (int j = 0; j < col; j++)
                {
                    if (props.ContainsKey("t" + i + "_" + j))
                    {
                        string t = (string) props["t" + i + "_" + j];
                        Debug.LogError("t=" + t);
                        switch (t)
                        {
                            case "Box":
                                float x = (float) props["p" + i + "_" + j + "X"];
                                float y = (float) props["p" + i + "_" + j + "Y"];
                                float z = (float) props["p" + i + "_" + j + "Z"];
                                pos = new Vector3(x, y, z);
                                float hex1 = (float) props["p" + i + "_" + j + "X0"];
                                float hey1 = (float) props["p" + i + "_" + j + "Y0"];
                                float hez1 = (float) props["p" + i + "_" + j + "Z0"];
                                he1 = new Vector3(hex1, hey1, hez1);
                                float hex2 = (float) props["p" + i + "_" + j + "X1"];
                                float hey2 = (float) props["p" + i + "_" + j + "Y1"];
                                float hez2 = (float) props["p" + i + "_" + j + "Z1"];
                                he2 = new Vector3(hex2, hey2, hez2);
                                float hex3 = (float) props["p" + i + "_" + j + "X2"];
                                float hey3 = (float) props["p" + i + "_" + j + "Y2"];
                                float hez3 = (float) props["p" + i + "_" + j + "Z2"];
                                he3 = new Vector3(hex3, hey3, hez3);
                                Quaternion q = Quaternion.Euler(he1);
                                if (box != null)
                                {
                                    GameObject go = Instantiate(box, pos, q);
                                    go.transform.localScale = new Vector3(he2.magnitude * 2, he1.magnitude * 2, he3.magnitude * 2);
                                    objects.Add(go);
                                }

                                break;
                            case "Sphere":
                                float sx = (float) props["p" + i + "_" + j + "X"];
                                float sy = (float) props["p" + i + "_" + j + "Y"];
                                float sz = (float) props["p" + i + "_" + j + "Z"];
                                pos = new Vector3(sx, sy, sz);
                                float sr = (float) props["r" + i + "_" + j];
                                if (sphere != null)
                                {
                                    GameObject go = Instantiate(sphere, pos, Quaternion.identity);
                                    go.transform.localScale = new Vector3(sr, sr, sr);
                                    objects.Add(go);
                                }

                                break;
                            case "Capsule":
                                float cx1 = (float) props["p" + i + "_" + j + "X0"];
                                float cy1 = (float) props["p" + i + "_" + j + "Y0"];
                                float cz1 = (float) props["p" + i + "_" + j + "Z0"];
                                he1 = new Vector3(cx1, cy1, cz1);
                                float cx2 = (float) props["p" + i + "_" + j + "X1"];
                                float cy2 = (float) props["p" + i + "_" + j + "Y1"];
                                float cz2 = (float) props["p" + i + "_" + j + "Z1"];
                                he2 = new Vector3(cx2, cy2, cz2);
                                Vector3 v = he1 - he2;
                                float h = v.magnitude;
                                Vector3 v2 = v / 2;
                                pos = he1 + v2;
                                Quaternion cq = Quaternion.Euler(v);

                                float cr = (float) props["r" + i + "_" + j];
                                if (capsule != null)
                                {
                                    GameObject go = Instantiate(capsule, pos, cq);
                                    var c = go.GetComponent<CapsuleCollider>();
                                    if (c != null)
                                    {
                                        c.radius = cr;
                                        c.height = h;
                                    }

                                    //go.transform.localScale = new Vector3(he2.magnitude * 2, he1.magnitude * 2, he3.magnitude * 2);
                                    objects.Add(go);
                                }

                                break;
                        }
                    }
                    else
                    {
                        Debug.LogError("missing "+"t" + i + "_" + j);
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}