using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Atavism
{
    [CustomEditor(typeof(ArenaObject))]
    public class ArenaObjectEditor : Editor
    {

       // private bool currencyLoaded = false;
     //   private bool itemsLoaded = false;
       // private bool tasksLoaded = false;
      //  private bool instancesLoaded = false;
      //  string[] interactionTypes;
        bool help = false;

        public override void OnInspectorGUI()
        {
            ArenaObject obj = target as ArenaObject;
         //   var indentOffset = EditorGUI.indentLevel * 5f;
            var lineRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(lineRect.x, lineRect.y, EditorGUIUtility.currentViewWidth - 60f, lineRect.height);
            var fieldRect = new Rect(labelRect.xMax, lineRect.y, lineRect.width - labelRect.width - 60f, lineRect.height);
            var buttonRect = new Rect(fieldRect.xMax, lineRect.y, 60f, lineRect.height);
            GUIContent content = new GUIContent("Help");
            content.tooltip = "Click to show or hide help informations";
            if (GUI.Button(buttonRect, content, EditorStyles.miniButton))
                help = !help;
            GUIStyle topStyle = new GUIStyle(GUI.skin.box);
            topStyle.normal.textColor = Color.white;
            topStyle.fontStyle = FontStyle.Bold;
            topStyle.alignment = TextAnchor.UpperLeft;
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.textColor = Color.cyan;
            boxStyle.fontStyle = FontStyle.Bold;
            boxStyle.alignment = TextAnchor.UpperLeft;
            GUILayout.BeginVertical("Atavism Arena Object Configuration", topStyle);
            GUILayout.Space(20);
            //EditorGUILayout.LabelField("ID: " + obj.id);

            content = new GUIContent("Move Distance Y");
            content.tooltip = "Move Distance Y";
            obj.moveDistanceY = EditorGUILayout.FloatField(content, obj.moveDistanceY);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Move Time");
            content.tooltip = "Move time";
            obj.movetime = EditorGUILayout.FloatField(content, obj.movetime);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Move Time Step");
            content.tooltip = "Move Time Step";
            obj.timeStep = EditorGUILayout.FloatField(content, obj.timeStep);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);



          
            GUILayout.Space(2);
            GUILayout.BeginVertical("Sound Configuration", boxStyle);
            GUILayout.Space(20);
            content = new GUIContent("Sound Clip");
            content.tooltip = "List of Sound Clip";

            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.soundClip == null)
                obj.soundClip = new System.Collections.Generic.List<AudioClip>();
            for (int i = 0; i < obj.soundClip.Count; i++)
            {
                obj.soundClip[i] = (AudioClip)EditorGUILayout.ObjectField(obj.soundClip[i], typeof(AudioClip), false);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.soundClip.Add(null);
            }
            if (GUILayout.Button("Remove"))
            {
                if (obj.soundClip.Count > 0)
                    obj.soundClip.RemoveAt(obj.soundClip.Count - 1);
            }
            EditorGUILayout.EndHorizontal();



#if AT_MASTERAUDIO_PRESET
           content = new GUIContent("MasterAudio Sound Name");
            content.tooltip = "Put Name Master Audio Sound";
            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.soundNames == null)
                obj.soundNames = new System.Collections.Generic.List<string>();
            for (int i = 0; i < obj.soundNames.Count; i++)
            {
                obj.soundNames[i] = EditorGUILayout.TextField(obj.soundNames[i]);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.soundNames.Add(null);
            }
            if (GUILayout.Button("Remove"))
            {
              if (obj.soundNames.Count > 0)
                obj.soundNames.RemoveAt(obj.soundNames.Count - 1);
            }
            EditorGUILayout.EndHorizontal();

         
            if( obj.soundNames.Count > 0 || obj.soundClip.Count > 0 ){
#else
            if (obj.soundClip.Count > 0)
            {
#endif

                content = new GUIContent("Sound Clip 3D");
                content.tooltip = "Sound Clip 3D";
                obj.soundClip3D = EditorGUILayout.Toggle(content, obj.soundClip3D);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                content = new GUIContent("Sound Volume");
                content.tooltip = "Put sound Volume";
                obj.soundVolume = EditorGUILayout.FloatField(content, obj.soundVolume);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }
            GUILayout.EndVertical();
            GUILayout.EndVertical();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(obj);
                EditorUtility.SetDirty(target);
            }
        }
       
    }
}