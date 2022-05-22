using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Atavism
{
    public class AtavismMobSpawnMarkerGenerator : MonoBehaviour
    {

        // public int id = -1;
        public int mobTemplateID = -1;
        public int mobTemplateID2 = -1;
        public int mobTemplateID3 = -1;
        public int mobTemplateID4 = -1;
        public int mobTemplateID5 = -1;
        public string mobTemplateSearch = "";
        public string mobTemplateSearch2 = "";
        public string mobTemplateSearch3 = "";
        public string mobTemplateSearch4 = "";
        public string mobTemplateSearch5 = "";
        public int spawnRadius = 0;
        public int numSpawns = 1;
        public int respawnTime = 60;
        public int respawnTimeMax = 60;
        public int despawnTime = 50;

        public int spawnActiveStartHour = -1;
        public int spawnActiveEndHour = -1;

        public int alternateMobTemplateID = -1;
        public int alternateMobTemplateID2 = -1;
        public int alternateMobTemplateID3 = -1;
        public int alternateMobTemplateID4 = -1;
        public int alternateMobTemplateID5 = -1;
        public string alternateMobTemplateSearch= "";
        public string alternateMobTemplateSearch2= "";
        public string alternateMobTemplateSearch3= "";
        public string alternateMobTemplateSearch4= "";
        public string alternateMobTemplateSearch5= "";

        public float roamRadius = 0;
        public int patrolPath = -1;
        public bool isChest = false;
        public int pickupItemID = -1;

        public int merchantTable = -1;
        
        public string startsQuestsSearch= "";
        public string endsQuestsSearch= "";
        public string startsDialoguesSearch= "";
        public List<int> startsQuests = new List<int>();
        public List<int> endsQuests = new List<int>();
        public List<int> startsDialogues = new List<int>();

        public List<string> otherActions = new List<string>();
        public List<PatrolPoint> patrolPoints = new List<PatrolPoint>();
        public bool travelReverse = false;
        public bool hasCombat = true;
        public bool weaponSheathed = false;
        public List<GameObject> mobs = new List<GameObject>();
#if UNITY_EDITOR
        
        public int spawn_count = 10;
        public float spawn_min_distance = 10f;
        public float spawn_radius = 10f;
        public float spawn_height = 10f;
        public int iterationCount = 100;
        public GameObject mesh ;
        public LayerMask layers = (1 << 0);
        public int lineDrawCount = 18;
        public bool show_aggro = false;
        public bool show_roam = false;
        
        private void OnDrawGizmosSelected()
        {
            Handles.color = Color.blue;
            Gizmos.color = Color.blue;
            var _rot = Quaternion.LookRotation(Vector3.up);
            var center2 = new Vector3(0f, 0, spawn_height);

            Matrix4x4 angleMatrix = Matrix4x4.TRS(transform.position-new Vector3(0,spawn_height/2,0), _rot, Handles.matrix.lossyScale);
            
            using (new Handles.DrawingScope(angleMatrix))
            {
                Handles.DrawWireDisc(Vector3.zero, Vector3.forward, spawn_radius);
                Handles.DrawWireDisc(center2, Vector3.forward, spawn_radius);
                for (int i = 0; i < lineDrawCount; i++)
                {
                    DrawLine(spawn_radius * Mathf.Cos(i * 2 * Mathf.PI / lineDrawCount), spawn_radius * Mathf.Sin(i * 2 * Mathf.PI / lineDrawCount), spawn_height);
                }
            }
        }

        private  void DrawLine(float arg1, float arg2, float forward)
        {
            Gizmos.DrawLine(transform.position+new Vector3(arg1, -forward/2,arg2), transform.position+new Vector3(arg1, forward/2, arg2));
          //  Handles.DrawLine(new Vector3(arg1, arg2, 0f), new Vector3(arg1, arg2, forward));
        }

        public int Spawn(string prefabName, int aggro,int num=1, int numMax=1)
        {
         //   string prefabName = ServerMobs.LoadMobTemplateModel(mobTemplateID);
            if (prefabName.Contains(".prefab"))
            {
                int resourcePathPos = prefabName.IndexOf("Resources/");
                prefabName = prefabName.Substring(resourcePathPos + 10);
                prefabName = prefabName.Remove(prefabName.Length - 7);
            }
            GameObject prefab = (GameObject)Resources.Load(prefabName);
            int c = 0;
            int l = 0;
            int i = 0;
            bool cancel = false;
            do
            {
                i++;
                int cc = 0;
                bool found = false;
                do
                {
                    cc++;
                    Vector3 v = FindPoint(spawn_radius, spawn_min_distance);
                    c++;
                    if (v.Equals(Vector3.negativeInfinity))
                    {
                        // break;
                    }
                    else
                    {
                        l++;
                        RaycastHit HitDown;
                        if (Physics.Raycast(new Vector3(v.x, v.y + spawn_height / 2, v.z), -Vector3.up, out HitDown, spawn_height, layers))
                        {
                            v = HitDown.point + new Vector3(0f, 0.1f, 0f);
                            var r = new System.Random();
                            Quaternion q = Quaternion.Euler(0f, (float) (r.NextDouble() * 360f), 0f);
                            GameObject go = Instantiate(prefab, v, q);
                            go.transform.SetParent(transform);
                            AtavismMobSpawnMarker amsm = go.AddComponent<AtavismMobSpawnMarker>();
                            clone(amsm);
                            mobs.Add(go);
                            found = true;
                        }
                    }
                    
                    if (EditorUtility.DisplayCancelableProgressBar("Ganagate...", "Generating position for mob "+i+"/"+spawn_count+(numMax>1?" in generator "+num+"/"+numMax:""), ((float) cc / (float) iterationCount)))
                    {
                        cancel = true;
                    }
                } while (!cancel && !found && cc < iterationCount);
            } while (!cancel && i < spawn_count);

            EditorUtility.ClearProgressBar();
            return mobs.Count;
            //  Debug.LogError("Generated "+mobs.Count+" / "+c+" "+l);
        }

        private Vector3 FindPoint(float radius, float min_distance)
        {
            var random = new System.Random();
            float radiusSQ = radius * radius;
            float dx, dz;
            float distSQ;
            bool free = true;
            int count = 0;
            Vector3 pos = Vector3.negativeInfinity;
            do
            {
                free = true;
                do
                {
                    dx = random.Next(100) / 100F * (2 * radius) - radius;
                    dz = random.Next(100) / 100F * (2 * radius) - radius;
                    distSQ = dx * dx + dz * dz;
                } while (distSQ > radiusSQ);

                pos = transform.position + new Vector3(dx, 0, dz);
                foreach (var go in mobs)
                {
                    if (go != null)
                    {
                        var d = Vector3.Distance(go.transform.position, pos);
                        if (d < min_distance)
                            free = false;
                    }
                }

                count++;
                if (count > iterationCount)
                {
                    free = true;
                    pos = Vector3.negativeInfinity;
                }
              /*  if (EditorUtility.DisplayCancelableProgressBar("Ganagate...", "Generating mobs try position 1 ", ((float) count / (float) iterationCount)))
                {
                    free = true;
                    pos = Vector3.negativeInfinity;
                }*/
            } while (!free);

            return pos;
        }

        public void Clear()
        {
            foreach (var m in mobs)
            {
                if(m != null)
                    DestroyImmediate(m);
            }
            mobs.Clear();
        }

        void clone(AtavismMobSpawnMarker marker)
        {
        marker.mobTemplateID = mobTemplateID;
        marker.mobTemplateID2 = mobTemplateID2;
        marker.mobTemplateID3 = mobTemplateID3;
        marker.mobTemplateID4 = mobTemplateID4;
        marker.mobTemplateID5 = mobTemplateID5;
    
        marker.respawnTime = respawnTime;
        marker.respawnTimeMax = respawnTimeMax;
        marker.despawnTime = despawnTime;

        marker.spawnActiveStartHour = spawnActiveStartHour;
        marker.spawnActiveEndHour = spawnActiveEndHour;

        marker.alternateMobTemplateID = alternateMobTemplateID;
        marker.alternateMobTemplateID2 = alternateMobTemplateID2;
        marker.alternateMobTemplateID3 = alternateMobTemplateID3;
        marker.alternateMobTemplateID4 = alternateMobTemplateID4;
        marker.alternateMobTemplateID5 = alternateMobTemplateID5;
        marker.roamRadius = roamRadius;
        marker.patrolPath = -1;
        marker.isChest = isChest;
        marker.pickupItemID = pickupItemID;
        marker.merchantTable = merchantTable;
        marker.startsQuests =startsQuests;
        marker.endsQuests = endsQuests;
        marker.startsDialogues = startsDialogues;

        marker.otherActions = otherActions;
        marker.patrolPoints = patrolPoints;
        marker.travelReverse = travelReverse;
        marker.hasCombat = hasCombat;
        marker.weaponSheathed = weaponSheathed;
        
        marker.layers = layers;
        marker.position = marker.gameObject.transform.position;
        }
        
        
#endif

    }
}