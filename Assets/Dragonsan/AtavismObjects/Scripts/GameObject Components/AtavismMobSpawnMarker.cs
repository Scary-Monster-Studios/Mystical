using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Atavism
{
    public class AtavismMobSpawnMarker : MonoBehaviour
    {

        public int id = -1;
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
      //  public List<GameObject> mobs = new List<GameObject>();
        public float aggro_radius = 17f;

    //    public float spawn_radius = 10f;
        public float spawn_height = 10f;
      //  public int spawn_count = 10;
     //   public float spawn_min_distance = 10f;
        public Vector3 position = Vector3.zero;
#if UNITY_EDITOR
     //   public int iterationCount = 5000;
       // public GameObject mesh ;
        public LayerMask layers = (1 << 0);
        public int lineDrawCount = 180;
        public bool show_aggro = false;
        public bool show_roam = false;
        private void OnDrawGizmosSelected()
        {
            
            Gizmos.color = Color.green;
            Handles.color = Color.green;
            var p_rot = Quaternion.LookRotation(Vector3.up);
            var pcenter = new Vector3(0f, 0, spawn_height);

            Matrix4x4 pangleMatrix = Matrix4x4.TRS(transform.position - new Vector3(0, spawn_height / 2, 0), p_rot, Handles.matrix.lossyScale);

            using (new Handles.DrawingScope(pangleMatrix))
            {
                Handles.DrawWireDisc(Vector3.zero, Vector3.forward, 0.1f);
                Handles.DrawWireDisc(pcenter, Vector3.forward, 0.1f);
                for (int i = 0; i < 18; i++)
                {
                    DrawLine(0.1f * Mathf.Cos(i * 2 * Mathf.PI / 18), 0.1f * Mathf.Sin(i * 2 * Mathf.PI / 18), spawn_height);
                }
            }
            
            if (show_roam)
            {
                Handles.color = Color.cyan;
                Gizmos.color = Color.cyan;
                var _rot = Quaternion.LookRotation(Vector3.up);
                var center = new Vector3(0f, 0, spawn_height);

                Matrix4x4 angleMatrix = Matrix4x4.TRS(transform.position - new Vector3(0, spawn_height / 2, 0), _rot, Handles.matrix.lossyScale);

                using (new Handles.DrawingScope(angleMatrix))
                {
                    Handles.DrawWireDisc(Vector3.zero, Vector3.forward, roamRadius);
                    Handles.DrawWireDisc(center, Vector3.forward, roamRadius);
                    for (int i = 0; i < lineDrawCount; i++)
                    {
                        DrawLine(roamRadius * Mathf.Cos(i * 2 * Mathf.PI / lineDrawCount), roamRadius * Mathf.Sin(i * 2 * Mathf.PI / lineDrawCount), spawn_height);
                    }
                }
            }

            if (show_aggro)
            {
                Handles.color = Color.magenta;
                Gizmos.color = Color.magenta;
                var a_rot = Quaternion.LookRotation(Vector3.up);
                var acenter = new Vector3(0f, 0, spawn_height);

                Matrix4x4 angleMatrix = Matrix4x4.TRS(transform.position - new Vector3(0, spawn_height / 2, 0), a_rot, Handles.matrix.lossyScale);

                using (new Handles.DrawingScope(angleMatrix))
                {
                    Handles.DrawWireDisc(Vector3.zero, Vector3.forward, aggro_radius);
                    Handles.DrawWireDisc(acenter, Vector3.forward, aggro_radius);
                    for (int i = 0; i < lineDrawCount; i++)
                    {
                        DrawLine(aggro_radius * Mathf.Cos(i * 2 * Mathf.PI / lineDrawCount), aggro_radius * Mathf.Sin(i * 2 * Mathf.PI / lineDrawCount), spawn_height);
                    }
                }
            }
        }

        private  void DrawLine(float arg1, float arg2, float forward)
        {
            Gizmos.DrawLine(transform.position+new Vector3(arg1, -forward/2,arg2), transform.position+new Vector3(arg1, forward/2, arg2));
          //  Handles.DrawLine(new Vector3(arg1, arg2, 0f), new Vector3(arg1, arg2, forward));
        }

      
        
#endif

    }
}