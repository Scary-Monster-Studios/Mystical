using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Atavism
{
    [CustomEditor(typeof(PatrolPathMarker))]
    public class PatrolPathMarkerEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            PatrolPathMarker marker = target as PatrolPathMarker;

            EditorGUILayout.LabelField("ID: " + marker.id);
            marker.startingPoint = EditorGUILayout.Toggle("Starting Point", marker.startingPoint);
            if (marker.startingPoint)
            {
                marker.travelReverse = EditorGUILayout.Toggle("Travel Reverse at end", marker.travelReverse);
            }

            marker.lingerTime = EditorGUILayout.FloatField("Linger Time", marker.lingerTime);
            marker.nextPoint = (GameObject)EditorGUILayout.ObjectField("Next Point", marker.nextPoint, typeof(GameObject), true);
            if (marker.nextPoint != null && marker.nextPoint.GetComponent<PatrolPathMarker>() == null)
                marker.nextPoint = null;

            if (marker.startingPoint)
            {
                if (GUILayout.Button("Save"))
                {
                    ServerPatrolPaths.SavePatrolPath(marker);
                }
            }

            if (GUILayout.Button("Delete"))
            {
                if (marker.id > 0)
                    ServerPatrolPaths.DeleteMarker(marker);
                DestroyImmediate(marker.gameObject);
            }

            EditorUtility.SetDirty(target);
        }
    }
}