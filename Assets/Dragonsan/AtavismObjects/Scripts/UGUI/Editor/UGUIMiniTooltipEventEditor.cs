using UnityEditor;
using UnityEngine;

namespace Atavism
{
    [CustomEditor(typeof(UGUIMiniTooltipEvent))]
    public class UGUIMiniTooltipEventEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            UGUIMiniTooltipEvent obj = target as UGUIMiniTooltipEvent;
            GUIContent content = new GUIContent("Description");
            content.tooltip = "Description";
            obj.dectName = EditorGUILayout.TextField(content, obj.dectName);
        }

    }
}