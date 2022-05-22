using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Atavism
{
    [CustomEditor(typeof(ResourceNode))]
    [CanEditMultipleObjects]
    public class AtavismResourceNodeEditor : Editor
    {

        bool help = false;
        private bool profilesLoaded = false;
        string search = "";

        SerializedProperty idProp;
        SerializedProperty profileIdProp;
        SerializedProperty highlightColourProp;
        private SerializedProperty isLODChildProp;
        private SerializedProperty highlightProp;
        private SerializedProperty highlightParamProp;

        void OnEnable()
        {
            // Setup the SerializedProperties.
            idProp = serializedObject.FindProperty("id");
            profileIdProp = serializedObject.FindProperty("profileId");
            highlightColourProp = serializedObject.FindProperty("highlightColour");
            highlightProp = serializedObject.FindProperty("highlight");
            highlightParamProp = serializedObject.FindProperty("highlightParam");
            isLODChildProp = serializedObject.FindProperty("isLODChild");
        }

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();
            var lineRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(lineRect.x, lineRect.y, EditorGUIUtility.currentViewWidth - 60f, lineRect.height);
            var fieldRect = new Rect(labelRect.xMax, lineRect.y, lineRect.width - labelRect.width - 60f, lineRect.height);
            var buttonRect = new Rect(fieldRect.xMax, lineRect.y, 60f, lineRect.height);

            ResourceNode obj = (ResourceNode) target; // as ResourceNode;
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
            GUILayout.BeginVertical("Atavism Resource Node Configuration", topStyle);
            GUILayout.Space(20);
            //ID
            var lineResetRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var buttonResetRect = new Rect(fieldRect.xMax, lineResetRect.y, 60f, lineResetRect.height);
            GUIContent reset = new GUIContent("Reset ID");
            reset.tooltip = "Click to reset ID of Resource Node";
            if (GUI.Button(buttonResetRect, reset, EditorStyles.miniButton))
            {
                if (idProp.intValue > 0)
                {
                    int instanceID = ServerInstances.GetInstanceID(obj.gameObject.scene.name);
                    ResourceNode[] ResourceNodes = FindObjectsOfType<ResourceNode>();
                    bool found = false;
                    foreach (ResourceNode rn in ResourceNodes)
                    {
                        if (!rn.Equals(obj) && rn.id.Equals(idProp.intValue))
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        DatabasePack.con = DatabasePack.Connect(DatabasePack.contentDatabasePrefix);
                        string query = "delete from resource_node_template where instance = " + instanceID + " and id = " + idProp.intValue;
                        DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, query);
                     //   query = "delete from resource_drop where  resource_template = " + obj.id;
                      //  DatabasePack.ExecuteNonQuery(DatabasePack.contentDatabasePrefix, query);
                    }
                }

                idProp.intValue = -1;
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(idProp, content);
            EditorGUI.EndDisabledGroup();
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            if (!profilesLoaded)
            {
                ServerCharacter.LoadResourceNodeProfileOptions(true);
                profilesLoaded = true;
            }

            content = new GUIContent("Search Profile:");
            content.tooltip = "Search Profile ";
            search = EditorGUILayout.TextField(content, search);
            content = new GUIContent("Profile:");
            content.tooltip = "Profile";

            profileIdProp.intValue = ServerCharacter.GetFilteredListSelector(content, ref search, profileIdProp.intValue, ServerCharacter.RNProfileList, ServerCharacter.RNProfileIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Highlight");
            content.tooltip = "Highlight";
            EditorGUILayout.PropertyField(highlightProp, content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            content = new GUIContent("Highlight Param");
            content.tooltip = "Highlight Param on the material";
            EditorGUILayout.PropertyField(highlightParamProp, content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (highlightProp.boolValue)
            {

                content = new GUIContent("Highlight Color");
                content.tooltip = "Select Highlight Color";
                EditorGUILayout.PropertyField(highlightColourProp, content);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }

            content = new GUIContent("Is LOD Child:");
            content.tooltip = "Is LOD Child";
            EditorGUILayout.PropertyField(isLODChildProp, content);

            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            
            
            content = new GUIContent("Sub Profile Game Objects:");
            content.tooltip = "Sub Profile Game Objects";
                EditorGUILayout.LabelField(content);
            if (obj.subProfileGameObjects == null)
                obj.subProfileGameObjects = new System.Collections.Generic.List<GameObject>();
            for (int i = 0; i < obj.subProfileGameObjects.Count; i++)
            {
                GUI.backgroundColor = Color.green;
                GUILayout.BeginVertical("", topStyle);
                GUI.backgroundColor = Color.white;
                content = new GUIContent( i + ":");
                content.tooltip = "Game Object ";
                obj.subProfileGameObjects[i] = (GameObject) EditorGUILayout.ObjectField(content,obj.subProfileGameObjects[i], typeof(GameObject));
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

              
                GUILayout.EndVertical();
                GUILayout.Space(2);
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.subProfileGameObjects.Add(null);
            }
            if (GUILayout.Button("Remove"))
            {
                obj.subProfileGameObjects.RemoveAt(obj.subProfileGameObjects.Count - 1);
            }
            EditorGUILayout.EndHorizontal();
            
            
            GUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }
}