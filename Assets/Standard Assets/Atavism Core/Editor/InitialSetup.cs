using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using System.IO;
using UnityEditor.SceneManagement;

namespace Atavism
{
    [InitializeOnLoad]
    class InitialSetup
    {
        static InitialSetup()
        {
            if (!Directory.Exists(Path.GetFullPath("Assets/..") + "/Assets/TextMesh Pro"))
                InstallTMPEsenc();

            if (!File.Exists(Path.GetFullPath("Assets/..") + "/InitialSetup.txt"))
            {
                EditorApplication.update += Update;

            }
        }

        [MenuItem("Window/Atavism/Atavism Unity Setup")]
        public static void SetupAtavismUnity()
        {
            if (EditorUtility.DisplayDialog("Atavism Setup", "Are you sure you want to process Atavism Setup for Unity?\n" +
                                                             "We will set your project settings as following:\n" +
                                                             "- Add necessary layers\n" +
                                                             "- Add scenes to the build settings\n" +
                                                             "- Add TextMesh Pro with installed Essentials\n" +
                                                             "- Set Player settings(Color space to Linear, and.NET to 4.x)", "Setup", "Do Not Setup"))
            {
                Setup();
            }
        }

        static void Update()
        {
            PlayerPrefs.SetInt("AtavismSetup", 1);
            PlayerPrefs.Save();
            EditorApplication.update -= Update;
            TextWriter tw = new StreamWriter(Path.GetFullPath("Assets/..") + "/InitialSetup.txt", true);
            tw.Close();
            if (EditorUtility.DisplayDialog("Atavism Setup", "Are you sure you want to process Atavism Setup for Unity?\n" +
                                                             "We will set your project settings as following:\n" +
                                                             "- Add necessary layers\n" +
                                                             "- Add scenes to the build settings\n" +
                                                             "- Add TextMesh Pro with installed Essentials\n" +
                                                             "- Set Player settings(Color space to Linear, and.NET to 4.x)", "Setup", "Do Not Setup"))
            {
                Setup();
            }
        }

        static void InstallTMPEsenc()
        {
            AssetDatabase.ImportPackage("Packages/com.unity.textmeshpro/Package Resources/TMP Essential Resources.unitypackage", false);
            PlayerPrefs.SetInt("AtavismSetupTMP", 0);
            PlayerPrefs.Save();
            //   EditorSceneManager.OpenScene("Assets/Dragonsan/Scenes/Login.unity"); 

        }

        static void Setup()
        {
            string MainFolder = "Assets/Dragonsan/Scenes/";
            string SceneType = ".unity";
            string[] ScenesList = new string[] { "Login", "CharacterSelection", "MainWorld", "Arena1v1", "Arena2v2", "Deathmatch 1v1", "Deathmatch 2v2", "SinglePlayerPrivate", "GuildPrivate", };
            int ii = 0;
            int notexist = 0;
            for (ii = 0; ii < ScenesList.Length; ii++)
            {
                if (!File.Exists(MainFolder + ScenesList[ii] + SceneType))
                {
                    notexist++;
                }
            }
            // EditorBuildSettingsScene[] original = EditorBuildSettings.scenes;
            EditorBuildSettingsScene[] newSettings = new EditorBuildSettingsScene[/*original.Length + */ScenesList.Length - notexist];
            //  System.Array.Copy(original, newSettings, original.Length);
            int i = 0;
            int index = 0;/* original.Length;*/
            for (i = 0; i < ScenesList.Length; i++)
            {
                if (File.Exists(MainFolder + ScenesList[i] + SceneType))
                {
                    EditorBuildSettingsScene sceneToAdd = new EditorBuildSettingsScene(MainFolder + ScenesList[i] + SceneType, true);
                    newSettings[index] = sceneToAdd;
                    index++;
                }
            }
            EditorBuildSettings.scenes = newSettings;
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layerProps = tagManager.FindProperty("layers");
            var layerProp8 = layerProps.GetArrayElementAtIndex(8);
            layerProp8.stringValue = "Targetable";
            var layerProp9 = layerProps.GetArrayElementAtIndex(9);
            layerProp9.stringValue = "MiniMap";
            var layerProp10 = layerProps.GetArrayElementAtIndex(10);
            layerProp10.stringValue = "AtavismText";
            var layerProp11 = layerProps.GetArrayElementAtIndex(11);
            layerProp11.stringValue = "Socket";
            tagManager.ApplyModifiedProperties();
            // Client.Remove("TextMesh Pro");
            if (Directory.Exists(Path.GetFullPath("Assets/..") + "/Assets/TextMesh Pro"))
                Directory.Delete(Path.GetFullPath("Assets/..") + "/Assets/TextMesh Pro", true);
#if UNITY_2018_2
            // Client.Add("com.unity.textmeshpro@1.2.4");
            AssetDatabase.ImportPackage("Assets/Dragonsan/AtavismObjects/External/SystemThreading.unitypackage", false);
#endif
#if UNITY_2018_3
            AssetDatabase.ImportPackage("Assets/Dragonsan/AtavismObjects/External/SystemThreading.unitypackage", false);
#endif
#if UNITY_2018_2_OR_NEWER
            //   Client.Add("com.unity.textmeshpro");
#endif
            PlayerPrefs.SetInt("AtavismSetupTMP", 1);
            PlayerPrefs.Save();
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.graphicsJobs = true;
            if (PlayerSettings.scriptingRuntimeVersion != ScriptingRuntimeVersion.Latest)
                PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest;
            
            if (!Directory.Exists(Application.streamingAssetsPath))
                Directory.CreateDirectory(Application.streamingAssetsPath);
            BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            
            //    AssetDatabase.ImportPackage("Assets/Dragonsan/AtavismEditor/Editor/Interfaces/system35.unitypackage", false);
            if (Directory.Exists(Path.GetFullPath("Assets/..") + "/Assets/TextMesh Pro"))
                Directory.Delete(Path.GetFullPath("Assets/..") + "/Assets/TextMesh Pro", true);

            Request = Client.List();    // List packages installed for the Project
            EditorApplication.update += Progress;
            //PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_2_0_Subset);
         //  PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_4_6);

        }
        static void Progress()
        {
            if (Request.IsCompleted)
            {
                if (Request.Status == StatusCode.Success)
                    foreach (var package in Request.Result)
                    {
                        if (package.name.Equals("com.unity.textmeshpro"))
                        {
                              Debug.Log("Package name: " + package.name + " " + package.version + " " + package.versions.latestCompatible + " " + package.versions.latest + " " + package.versions.verified);
                            if (!package.version.Equals(package.versions.verified))
                            {
                                Request2 = Client.Add("com.unity.textmeshpro");
                               // 

                                EditorApplication.update += Progress2;
                            }
                            else
                            {
                                AssetDatabase.Refresh();
                                AssetDatabase.ImportPackage("Packages/com.unity.textmeshpro/Package Resources/TMP Essential Resources.unitypackage", false);
                                Request3 = Client.Add("com.unity.addressables");
                                EditorApplication.update += Progress3;
                            }
                        }  
                       /* if (package.name.Equals("com.unity.addressables"))
                        {
                            Debug.Log("Package name: " + package.name + " " + package.version + " " + package.versions.latestCompatible + " " + package.versions.latest + " " + package.versions.verified);
                            if (!package.version.Equals(package.versions.verified))
                            {
                                Request3 = Client.Add("com.unity.addressables");
                                // 

                                EditorApplication.update += Progress3;
                            }
                            else
                            {
                                AssetDatabase.Refresh();
                                EditorApplication.update += Progress3;
                            }
                        }*/
                    }
                EditorApplication.update -= Progress;
              
            }
        }



        static void Progress2()
        {
            if (Request2.IsCompleted)
            {
                EditorApplication.update += Progress3;
                EditorApplication.update -= Progress2;
                AssetDatabase.Refresh();
                Request3 = Client.Add("com.unity.addressables");

            }
        }

        static void Progress3()
        {
            if (Request3.IsCompleted)
            {
                if (Request3.Status == StatusCode.Success)
                {
                   string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                   if (!symbols.Contains("AT_ADDRESSABLES"))
                   {
                       symbols += ";" + "AT_ADDRESSABLES";
                       PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
                   }
                    EditorUtility.DisplayDialog("Atavism Setup", "Atavism setup was successful", "OK", "");

                }
                else if (Request.Status >= StatusCode.Failure)
                    Debug.Log(Request.Error.message);

                EditorApplication.update -= Progress3;
            }
        }
        static ListRequest Request;
        static AddRequest Request2;
        static AddRequest Request3;
    }
}
