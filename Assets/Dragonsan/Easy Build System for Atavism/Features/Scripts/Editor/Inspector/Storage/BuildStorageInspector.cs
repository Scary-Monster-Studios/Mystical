using EasyBuildSystem.Features.Scripts.Core.Addons.Enums;
using EasyBuildSystem.Features.Scripts.Core.Addons.Helper;
using EasyBuildSystem.Features.Scripts.Core.Base.Storage;
using EasyBuildSystem.Features.Scripts.Editor.Window;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Editor.Inspector.Storage
{
    [CustomEditor(typeof(BuildStorage))]
    public class BuildStorageInspector : UnityEditor.Editor
    {
        #region Fields

        private BuildStorage Target;
        private string LoadPath;

        private static bool GeneralFoldout;
        private static bool AddonsFoldout;

        #endregion Fields

        #region Methods

        private void OnEnable()
        {
            Target = (BuildStorage)target;
            MainEditor.Addons = AddonHelper.GetAddonsByTarget(AddonTarget.StorageBehaviour);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region Build Storage General

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Build Storage - General", EditorStyles.largeLabel);

            GUI.color = Color.white;

            GeneralFoldout = EditorGUILayout.Foldout(GeneralFoldout, "General Settings");

            if (GeneralFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("StorageType"), new GUIContent("Storage Type :", "This allows to save/load for Desktop or Android."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("AutoSave"), new GUIContent("Storage Auto Save :", "This allows to enable auto save."));

                if (serializedObject.FindProperty("AutoSave").boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("AutoSaveInterval"), new GUIContent("Storage Auto Save Interval (ms) :", "Define the auto save interval."));
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("LoadAndWaitEndFrame"), new GUIContent("Storage Load And Wait End Of Frame :",
                    "This allows to WaitEndOfFrame to instantiate the next piece, useful to avoid the spikes at start of the scene (recommended: for headless server)."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("SavePrefabs"), new GUIContent("Storage Save All Pieces :", "This allows to save all the prefabs after have exited the scene."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("LoadPrefabs"), new GUIContent("Storage Load All Pieces :", "This allows to save all the prefabs at startup of the scene."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("AutoDefineInDataPath"), new GUIContent("Storage Auto Define In Data Patch :",
                    "This allows to define a patch on start in dataPath."));

                if (!serializedObject.FindProperty("AutoDefineInDataPath").boolValue)
                {
                    if (serializedObject.FindProperty("StorageType").enumValueIndex == 0)
                    {
                        EditorGUI.BeginChangeCheck();

                        EditorGUILayout.HelpBox("Define here the complete path with the name & extension.\n" +
                            @"Example for Windows : C:\Users\My Dekstop\Desktop\MyFile.dat", MessageType.Info);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("StorageOutputFile"), new GUIContent("Storage Output Path :", "Output path to save and load the file."));

                        EditorGUI.EndChangeCheck();

                        if (GUI.changed)
                        {
                            EditorUtility.SetDirty(target);
                        }
                    }
                }

                GUI.color = MainEditor.GetEditorColor;

                if (GUILayout.Button("Define Path"))
                {
                    string SaveLoadPath = EditorUtility.SaveFolderPanel("Easy Build System - Define Path", "", "");

                    if (SaveLoadPath.Length != 0)
                    {
                        Target.StorageOutputFile = SaveLoadPath + "/MyFile.dat";
                    }
                }

                if (GUILayout.Button("Load File In Editor Scene"))
                {
                    if (EditorUtility.DisplayDialog("Easy Build System - Load File", "Your scene will be saved to avoid the loss data in case of crash.", "Load", "Cancel"))
                    {
                        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

                        LoadPath = EditorUtility.OpenFilePanel("Load Storage File :", "", "*.*");

                        if (LoadPath != string.Empty)
                        {
                            Target.LoadInEditor(LoadPath);
                        }
                    }
                }
            }

            #endregion Build Storage General

            #region Build Add-Ons Settings

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Build Storage - Addons", EditorStyles.largeLabel);

            GUI.color = Color.white;

            AddonsFoldout = EditorGUILayout.Foldout(AddonsFoldout, "Addons Settings");

            if (AddonsFoldout)
            {
                MainEditor.DrawAddons(Target, AddonTarget.StorageBehaviour);
            }

            #endregion Socket Add-Ons Settings

            serializedObject.ApplyModifiedProperties();
        }

        #endregion Methods
    }
}