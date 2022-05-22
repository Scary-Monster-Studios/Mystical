using EasyBuildSystem.Features.Scripts.Core.Base.Event;
using EasyBuildSystem.Features.Scripts.Editor.Window;
using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Editor.Inspector.Event
{
    [CustomEditor(typeof(BuildEvent))]
    public class BuildEventInspector : UnityEditor.Editor
    {
        #region Fields

        private static bool BuilderFoldout;
        private static bool PieceFoldout;
        private static bool SocketFoldout;
        private static bool GroupFoldout;
        private static bool StorageFoldout;

        private static bool GeneralFoldout;

        #endregion Fields

        #region Methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region Build Event General

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Build Event - General", EditorStyles.largeLabel);

            GUI.color = Color.white;

            GeneralFoldout = EditorGUILayout.Foldout(GeneralFoldout, "General Settings");

            if (GeneralFoldout)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(13);
                BuilderFoldout = EditorGUILayout.Foldout(BuilderFoldout, "Builder Behaviour Events");
                GUILayout.EndHorizontal();

                if (BuilderFoldout)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnChangedBuildMode"), new GUIContent("OnChangedBuildMode", "Define the origin offset."), true);
                }

                GUILayout.BeginHorizontal();
                GUILayout.Space(13);
                PieceFoldout = EditorGUILayout.Foldout(PieceFoldout, "Piece Behaviour Events");
                GUILayout.EndHorizontal();

                if (PieceFoldout)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPieceInstantiated"), new GUIContent("OnPieceInstantiated", "Define the origin offset."), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPieceDestroyed"), new GUIContent("OnPieceDestroyed", "Define the origin offset."), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPieceChangedState"), new GUIContent("OnPieceChangedState", "Define the origin offset."), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPieceChangedApperance"), new GUIContent("OnPieceChangedApperance", "Define the origin offset."), true);
                }

                GUILayout.BeginHorizontal();
                GUILayout.Space(13);
                SocketFoldout = EditorGUILayout.Foldout(SocketFoldout, "Socket Behaviour Events");
                GUILayout.EndHorizontal();

                if (SocketFoldout)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnChangedSocketState"), new GUIContent("OnChangedSocketState", "Define the origin offset."), true);
                }

                GUILayout.BeginHorizontal();
                GUILayout.Space(13);
                GroupFoldout = EditorGUILayout.Foldout(GroupFoldout, "Group Behaviour Events");
                GUILayout.EndHorizontal();

                if (GroupFoldout)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnGroupInstantiated"), new GUIContent("OnGroupInstantiated", "Define the origin offset."), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnGroupUpdated"), new GUIContent("OnGroupUpdated", "Define the origin offset."), true);
                }

                GUILayout.BeginHorizontal();
                GUILayout.Space(13);
                StorageFoldout = EditorGUILayout.Foldout(StorageFoldout, "Build Storage Events");
                GUILayout.EndHorizontal();

                if (StorageFoldout)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnStorageSaving"), new GUIContent("OnStorageSaving", "Define the origin offset."), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnStorageLoading"), new GUIContent("OnStorageLoading", "Define the origin offset."), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnStorageLoadingResult"), new GUIContent("OnStorageLoadingResult", "Define the origin offset."), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnStorageSavingResult"), new GUIContent("OnStorageSavingResult", "Define the origin offset."), true);
                }
            }

            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        #endregion Fields
    }
}