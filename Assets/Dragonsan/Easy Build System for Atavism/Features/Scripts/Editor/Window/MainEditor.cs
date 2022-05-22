using System;
using System.Collections.Generic;
using UnityEditor;
#if UNITY_2018_3
using UnityEditor.Experimental.SceneManagement;
#endif
using UnityEditorInternal;
using UnityEngine;
using EasyBuildSystem.Features.Scripts.Editor.Helper;
using EasyBuildSystem.Features.Scripts.Core.Addons;
using EasyBuildSystem.Features.Scripts.Core.Addons.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Condition;
using EasyBuildSystem.Features.Scripts.Core;
using EasyBuildSystem.Features.Scripts.Core.Base.Socket;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using EasyBuildSystem.Features.Scripts.Core.Base.Area;
using EasyBuildSystem.Features.Scripts.Core.Base.Condition.Helper;
using EasyBuildSystem.Features.Scripts.Core.Addons.Helper;
using System.IO;

namespace EasyBuildSystem.Features.Scripts.Editor.Window
{
    [InitializeOnLoad]
    public class Initializer
    {
        #region Methods

        static Initializer()
        {
            MainEditor.CheckMissingLayers();
        }

        #endregion
    }

    public class MainEditor : EditorWindow
    {
        #region Fields

        public static List<ConditionAttribute> Conditions = new List<ConditionAttribute>();
        public static List<AddonAttribute> Addons = new List<AddonAttribute>();

#if UNITY_2019_4_OR_NEWER
        public static Color GetEditorColor = new Color(0f, 1f, 1f);
#else
        public static Color GetEditorColor = new Color(0f, 2f, 2f);
#endif

        public static Action LastAction;
        private static MainEditor Window;

        private static int NavigationIndex;

        private static List<BuildTargetGroup> Targets;

        private Vector2 AddonsScrollPosition;

        private Vector2 IntegrationScrollPosition;
        private static readonly List<string> Integrations = new List<string>();

        private bool ShowEditor;

        #endregion

        #region Methods

        private void OnEnable()
        {
            Conditions = ConditionHelper.GetConditions();
            Addons = AddonHelper.GetAddons();

            Targets = new List<BuildTargetGroup>
            {
                BuildTargetGroup.iOS,
                BuildTargetGroup.WebGL,
                BuildTargetGroup.Standalone,
                BuildTargetGroup.Android
            };

            NavigationIndex = EditorPrefs.GetInt("p_lastIndex");
            ShowEditor = EditorPrefs.GetBool("HideOnStart");

            CheckMissingLayers();
        }

        private void OnGUI()
        {
            try
            {
                if (Window == null)
                {
                    Close();
                    Init();
                    return;
                }

                #region Menu

                GUILayout.Space(5);

                GUILayout.BeginHorizontal();

                if (NavigationIndex == 0)
                {
                    GUI.color = GetEditorColor;
                }
                else
                {
                    GUI.color = Color.white / 1.5f;
                }

                if (GUILayout.Button("Welcome", GUILayout.Height(25)))
                {
                    NavigationIndex = 0;
                    EditorPrefs.SetInt("p_lastIndex", 0);
                }

                if (NavigationIndex == 1)
                {
                    GUI.color = GetEditorColor;
                }
                else
                {
                    GUI.color = Color.white / 1.5f;
                }

                if (GUILayout.Button("Conditions", GUILayout.Height(25)))
                {
                    NavigationIndex = 1;
                    EditorPrefs.SetInt("p_lastIndex", 1);
                }

                if (NavigationIndex == 2)
                {
                    GUI.color = GetEditorColor;
                }
                else
                {
                    GUI.color = Color.white / 1.5f;
                }

                if (GUILayout.Button("Add-Ons", GUILayout.Height(25)))
                {
                    NavigationIndex = 2;
                    EditorPrefs.SetInt("p_lastIndex", 2);
                }

                if (NavigationIndex == 3)
                {
                    GUI.color = GetEditorColor;
                }
                else
                {
                    GUI.color = Color.white / 1.5f;
                }

                if (GUILayout.Button("Integrations", GUILayout.Height(25)))
                {
                    NavigationIndex = 3;
                    EditorPrefs.SetInt("p_lastIndex", 3);
                }

                GUI.color = Color.white;

                GUILayout.EndHorizontal();

                #endregion Menu

                #region Navigations

                if (NavigationIndex == 0)
                {
                    GUILayout.BeginVertical();

                    GUI.color = GetEditorColor;

                    GUILayout.Label("Getting Started", EditorStyles.largeLabel);

                    GUI.color = Color.white;

                    GUILayout.Label("Thank you for purchasing Easy Build System!\n" +
                        "It is recommended that read the documentation, if you are using the system for the first time.\n" +
                        "You can check the example scenes, this will give you an idea of what can be done with the system.\n" +
                        "If you only want to use the features, you can follow the getting started steps in the documentation.");

                    GUI.color = GetEditorColor;

                    if (GUILayout.Button("Documentation & Tutorials")) { Application.OpenURL("https://app.archbee.io/public/GMe-mTqOKtUwWMJXi_bct/welcome--Jxp3"); }

                    GUILayout.Label("Old & Beta Releases", EditorStyles.largeLabel);

                    GUI.color = Color.white;

                    GUILayout.Label("You can download old releases as well beta by sending a download request with your invoice number.\n" +
                        "Note that the support/documentation is not available on the old versions of the system and some integrations are deprecated.");

                    GUI.color = GetEditorColor;

                    if (GUILayout.Button("Send Download Request")) { Application.OpenURL("https://eu.jotform.com/form/202960719544359/"); }

                    GUILayout.Label("Feedback", EditorStyles.largeLabel);

                    GUI.color = Color.white;

                    GUILayout.Label("Support the developement by let us know what you think of asset by writing a review on Asset Store.\n" +
                        "All requests, adding new features/integrations will be considered and will be maybe added in a next release.");

                    GUI.color = GetEditorColor;

                    if (GUILayout.Button("Write Review"))
                    {
                        Application.OpenURL("https://assetstore.unity.com/packages/templates/systems/easy-build-system-modular-building-system-45394#reviews");
                    }

                    GUI.color = GetEditorColor;

                    GUILayout.Label("Contact Support", EditorStyles.largeLabel);

                    GUI.color = Color.white;

                    GUILayout.Label("If you've any questions or issues with asset contacting us, any help or download requests require invoice number.");

                    GUI.color = GetEditorColor;

                    if (GUILayout.Button("Contact Support"))
                    {
                        Application.OpenURL("https://form.jotform.com/202960719544359");
                    }

                    GUI.color = Color.white;

                    GUILayout.EndVertical();
                }

                if (NavigationIndex == 1)
                {
                    #region Conditions

                    GUILayout.BeginVertical();

                    GUI.color = GetEditorColor;

                    GUILayout.Label("Conditions :", EditorStyles.largeLabel);

                    GUI.color = Color.white;

                    GUILayout.Label("List of all the conditions available in the asset folder (Easy Build System/).\n" +
                        "Each condition can be enabled/disabled on the respective component called Target.");

                    AddonsScrollPosition = GUILayout.BeginScrollView(AddonsScrollPosition);

                    if (Addons.Count == 0)
                    {
                        GUILayout.Label("No conditions are available for this component.", EditorStyles.miniLabel);
                    }
                    else
                    {
                        foreach (ConditionAttribute Condition in Conditions)
                        {
                            GUILayout.BeginVertical("box");

                            GUI.color = new Color(1.5f, 1.5f, 1.5f);

                            GUILayout.BeginHorizontal();

                            GUILayout.BeginVertical();

                            GUILayout.Space(3);

                            GUI.color = new Color(1f, 1f, 1f);

                            GUI.color = GetEditorColor;
                            GUILayout.Label(Condition.Name, EditorStyles.largeLabel);
                            GUI.color = Color.white;

                            GUILayout.Label("Target : " + Condition.Target.ToString(), EditorStyles.miniLabel);

                            GUILayout.BeginVertical();
                            GUILayout.Label("Description : " + Condition.Description, EditorStyles.miniLabel);
                            GUILayout.EndVertical();

                            GUILayout.EndVertical();

                            GUILayout.FlexibleSpace();

                            GUILayout.EndHorizontal();

                            GUI.color = Color.white;

                            GUILayout.EndVertical();
                        }
                    }

                    GUILayout.EndScrollView();

                    GUILayout.EndVertical();

                    #endregion
                }

                if (NavigationIndex == 2)
                {
                    #region Add-Ons

                    GUILayout.BeginVertical();

                    GUI.color = GetEditorColor;

                    GUILayout.Label("Addons :", EditorStyles.largeLabel);

                    GUI.color = Color.white;

                    GUILayout.Label("List of all the add-ons available in the asset folder (Easy Build System/).\n" +
                        "Each add-on can be enabled/disabled on the respective component called Target.");

                    AddonsScrollPosition = GUILayout.BeginScrollView(AddonsScrollPosition);

                    if (Addons.Count == 0)
                    {
                        GUILayout.Label("No addons was found for this component.", EditorStyles.miniLabel);
                    }
                    else
                    {
                        foreach (AddonAttribute Addon in Addons)
                        {
                            GUILayout.BeginVertical("box");

                            GUI.color = new Color(1.5f, 1.5f, 1.5f);

                            GUILayout.BeginHorizontal();

                            GUILayout.BeginVertical();

                            GUILayout.Space(3);

                            GUI.color = new Color(1f, 1f, 1f);

                            GUI.color = GetEditorColor;
                            GUILayout.Label(Addon.Name, EditorStyles.largeLabel);
                            GUI.color = Color.white;

                            GUILayout.Label("Target : " + Addon.Target.ToString(), EditorStyles.miniLabel);

                            GUILayout.EndVertical();

                            GUILayout.FlexibleSpace();

                            GUILayout.EndHorizontal();

                            GUI.color = Color.white;

                            GUILayout.EndVertical();
                        }
                    }

                    GUILayout.EndScrollView();

                    GUILayout.EndVertical();

                    #endregion Add-Ons
                }

                if (NavigationIndex == 3)
                {
                    #region Integrations

                    GUILayout.BeginVertical();

                    GUI.color = GetEditorColor;

                    GUILayout.Label("Integrations :", EditorStyles.largeLabel);

                    GUI.color = Color.white;

                    GUILayout.Label("List of all the integrations available in the folder (Easy Build System/Features/Integrations).\n" +
                        "Some integration may not work with the new versions, let us know so that we update them!");

                    IntegrationScrollPosition = GUILayout.BeginScrollView(IntegrationScrollPosition);

                    AddIntegration("Game Creator by Catsoft Studios", "/content/89443", "GAMECREATOR",
                        "This integration allows integrating Game Creator with guilde link.", "Game Creator Integration", null, null, "Require: Unity 2019.4f or higher");

                    AddIntegration("Photon Network V2 by Exit Games", "/content/119922", "EXITGAMESV2",
                        "This integration allows integrating Photon Network V2 with guilde link.", "Photon Network V2 Integration", null, null, "Require: Unity 2019.1f or higher");

                    AddIntegration("Mirror Network by Vis2k", "/content/129321", "MIRRORNETWORK",
                        "This integration allows integrating Mirror Network with guilde link.", "Mirror Integration", null, null, "Require: Unity 2019.1f or higher");

                    AddIntegration("Voxeland by Denis Pahunov", "/content/9180", "VOXELAND",
                        "This integration allows integrating Voxeland with guilde link.", "", null, null);

                    AddIntegration("Rewired by Guavaman Enterprises", "/content/21676", "REWIRED",
                        "This integration allows integrating Rewired with guilde link.", "Rewired Integration", null, null);

                    AddIntegration("Third Person Controller by Opsive", "/content/27438", "OPSIVE",
                        "This integration allows integrating Opsive Third Person Controller with guilde link.", "Opsive Third Person Controller Integration", null, null);

                    AddIntegration("Third Person Controller by Invector", "/content/126347", "INVECTOR",
                        "This integration allows integrating Invector Third Person Controller with guilde link.", "Invector Third Person Controller Integration", null, null);

                    AddIntegration("uSurvival by Vis2k", "/content/95015", "USURVIVALVIS2K",
                        "This integration allows integrating uSurvival with guilde link.", "uSurvival Integration", null, null, "Require: Unity 2019.4f or higher");

                    AddIntegration("uRPG by Vis2k", "/content/95016", "URPGVIS2K",
                        "This integration allows integrating uRPG with guilde link.", "uRPG Integration", null, null, "Require: Unity 2019.1f or higher");

                    AddIntegration("Inventory Pro by Devdog", "https://github.com/devdogio/Inventory-Pro", "DEVDOG",
                        "This integration allows integrating Inventory Pro with guilde link.", "Inventory Pro Integration", null, null, "");

                    GUILayout.EndScrollView();

                    GUILayout.FlexibleSpace();

                    GUILayout.EndVertical();

                    #endregion Integrations
                }

                #endregion Navigations

                GUILayout.FlexibleSpace();

                GUILayout.BeginHorizontal();

                GUI.color = GetEditorColor;

                GUILayout.FlexibleSpace();

                GUILayout.Label("Release : 5.2.5");

                GUI.color = Color.white;

                GUILayout.EndHorizontal();

                GUILayout.Space(5);
            }
            catch { }
        }

        public static void Init()
        {
            Window = CreateInstance<MainEditor>();
            Window.titleContent = new GUIContent("Easy Build System - Editor");
            Window.minSize = new Vector2(750, 375);
            Window.maxSize = new Vector2(750, 700);
            Window.ShowUtility();
        }

        private void AddIntegration(string name, string link, string defName, string description, string packageName, Action onEnable, Action onDisable, string requiredVersion = "")
        {
            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();

            GUI.color = GetEditorColor;
            GUILayout.Label(name, EditorStyles.largeLabel);
            GUI.color = Color.white;

            if (requiredVersion != null)
            {
                GUILayout.BeginVertical();
                GUILayout.Space(5);
                GUI.color = Color.yellow;
                GUILayout.Label(requiredVersion, EditorStyles.miniLabel);
                GUI.color = Color.white;
                GUILayout.EndVertical();
            }

            GUILayout.FlexibleSpace();

            if (!IsIntegrationEnabled(defName))
            {
                GUI.color = new Color(0f, 1f, 0f);

                if (GUILayout.Button("Enable Integration", GUILayout.Width(130)))
                {
                    if (packageName == string.Empty)
                    {
                        EnableIntegration(defName, onEnable);
                    }
                    else
                    {
                        if (GetRelativePath(packageName) != string.Empty)
                        {
                            EnableIntegration(defName, onEnable);
                            AssetDatabase.ImportPackage(GetRelativePath(packageName), true);
                        }
                        else
                        {
                            Debug.LogWarning("<b>Easy Build System</b> : Integration <b>(" + name + ")</b> has been not found !");
                        }
                    }
                }
            }
            else
            {
                GUI.color = new Color(1f, 0f, 0f);

                if (GUILayout.Button("Disable Integration", GUILayout.Width(130)))
                {
                    DisableIntegration(defName, onDisable);
                }
            }

            GUI.color = GetEditorColor;
            if (GUILayout.Button("Asset Store Page", GUILayout.Width(120)) && link != string.Empty)
            {
                if (link.Contains("http"))
                    Application.OpenURL(link);
                else
                    AssetStore.Open(link);
            }
            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            GUI.color = Color.white;

            GUILayout.Label(description, EditorStyles.miniLabel);

            GUILayout.EndVertical();
        }

        private string GetRelativePath(string packageName)
        {
            string[] allPaths = Directory.GetFiles(Application.dataPath, "*", SearchOption.AllDirectories);

            for (int i = 0; i < allPaths.Length; i++)
                if (allPaths[i].Contains(packageName))
                    return allPaths[i];

            return string.Empty;
        }

        private static bool IsIntegrationEnabled(string name)
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Contains(name);
        }

        public static void DisableIntegration(string name, Action onDisable)
        {
            if (IsIntegrationEnabled(name) == false)
            {
                return;
            }

            if (onDisable != null)
            {
                onDisable.Invoke();
            }

            foreach (BuildTargetGroup Target in Targets)
            {
                string Symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(Target);

                string[] SplitArray = Symbols.Split(';');

                List<string> Array = new List<string>(SplitArray);

                Array.Remove(name);

                if (Target != BuildTargetGroup.Unknown)
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(Target, string.Join(";", Array.ToArray()));
                }
            }

            Debug.Log("<b>Easy Build System</b> : Integration <b>(" + name + ")</b> has been disabled !");
        }

        public static void EnableIntegration(string name, Action onEnable)
        {
            if (IsIntegrationEnabled(name))
            {
                return;
            }

            Targets = new List<BuildTargetGroup>
            {
                BuildTargetGroup.iOS,

                BuildTargetGroup.WebGL,

                BuildTargetGroup.Standalone,

                BuildTargetGroup.Android
            };

            foreach (BuildTargetGroup Target in Targets)
            {
                string Symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(Target);

                string[] SplitArray = Symbols.Split(';');

                List<string> Array = new List<string>(SplitArray)
                {
                    name
                };

                if (Target != BuildTargetGroup.Unknown)
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(Target, string.Join(";", Array.ToArray()));
                }
            }

            if (onEnable != null)
            {
                Integrations.Add(onEnable.Method.Name);

                onEnable.Invoke();
            }

            Debug.Log("<b>Easy Build System</b> : Integration <b>(" + name + ")</b> has been enabled !");
        }

        public static void LoadAddons(MonoBehaviour target, AddonTarget addOnTarget)
        {
            Addons = AddonHelper.GetAddonsByTarget(addOnTarget);

            foreach (AddonBehaviour Addon in target.GetComponentsInChildren<AddonBehaviour>())
            {
                if (Addon != null)
                {
                    Addon.hideFlags = HideFlags.HideInInspector;
                }
            }
        }

        public static void DrawAddons(MonoBehaviour target, AddonTarget addOnTarget)
        {
            if (Addons.Count == 0)
            {
                GUILayout.BeginVertical("helpBox");
                GUILayout.Label("No addons was found for this component.");
                GUILayout.EndVertical();
            }
            else
            {
                foreach (AddonAttribute Addon in Addons)
                {
                    if (Addon.Target == addOnTarget)
                    {
                        GUI.color = Color.black / 4f;
                        GUILayout.BeginVertical("helpBox");
                        GUI.color = Color.white;

                        GUILayout.BeginHorizontal();

                        GUILayout.Label(Addon.Name, EditorStyles.largeLabel);

                        GUILayout.FlexibleSpace();

                        if (target.gameObject.GetComponent(Addon.Behaviour) == null)
                        {
                            GUI.color = new Color(0f, 1f, 0f);

                            if (GUILayout.Button("Enable Add-On", GUILayout.Width(120), GUILayout.Height(17)))
                            {
                                if (target.gameObject.GetComponent(Addon.Behaviour) != null)
                                {
                                    return;
                                }

                                Component Target = target.gameObject.AddComponent(Addon.Behaviour);
                                Target.hideFlags = HideFlags.HideInInspector;
                            }
                        }
                        else
                        {
                            GUI.color = MainEditor.GetEditorColor;

                            if (GUILayout.Button("Copy Settings", GUILayout.Width(120), GUILayout.Height(17)))
                            {
                                UnityEditorInternal.ComponentUtility.CopyComponent(target.GetComponent(Addon.Behaviour));
                            }

                            if (GUILayout.Button("Paste Settings", GUILayout.Width(120), GUILayout.Height(17)))
                            {
                                UnityEditorInternal.ComponentUtility.PasteComponentValues(target.GetComponent(Addon.Behaviour));
                            }

                            GUI.color = new Color(1f, 0f, 0f);

                            if (GUILayout.Button("Disable Add-On", GUILayout.Width(120), GUILayout.Height(17)))
                            {
                                try
                                {
                                    DestroyImmediate(target.gameObject.GetComponent(Addon.Behaviour), true);
                                    break;
                                }
                                catch { }
                            }
                        }

                        GUILayout.EndHorizontal();

                        GUI.color = Color.white;
                        GUILayout.Space(5);
                        GUILayout.BeginHorizontal();
                        if (target.gameObject.GetComponent(Addon.Behaviour) != null)
                        {
                            GUILayout.Space(13f);
                            GUILayout.BeginVertical();
                            UnityEditor.Editor AddonEditor = UnityEditor.Editor.CreateEditor(target.gameObject.GetComponent(Addon.Behaviour));
                            AddonEditor.OnInspectorGUI();
                            GUILayout.EndVertical();
                            GUILayout.Space(13f);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                    }
                }
            }
        }

        public static void CheckMissingLayers()
        {
            LoadLayers();
        }

        public static bool LoadLayers()
        {
            string[] LayerNames = new string[] { Constants.LAYER_SOCKET };

            bool Result = false;

            SerializedObject Manager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            SerializedProperty SerializedLayers = Manager.FindProperty("layers");

            foreach (string LayerName in LayerNames)
            {
                bool IsFound = false;

                for (int i = 0; i <= 31; i++)
                {
                    SerializedProperty SerializedProperty = SerializedLayers.GetArrayElementAtIndex(i);

                    if (SerializedProperty != null && LayerName.Equals(SerializedProperty.stringValue))
                    {
                        IsFound = true;

                        break;
                    }
                }

                if (!IsFound)
                {
                    Result = true;

                    SerializedProperty SerializedSlot = null;

                    for (int i = 8; i <= 31; i++)
                    {
                        SerializedProperty sp = SerializedLayers.GetArrayElementAtIndex(i);

                        if (sp != null && string.IsNullOrEmpty(sp.stringValue))
                        {
                            SerializedSlot = sp;

                            break;
                        }
                    }

                    if (SerializedSlot != null)
                    {
                        SerializedSlot.stringValue = LayerName;
                    }
                    else
                    {
                        Debug.LogError("<b>Easy Build System</b> : Could not find an open Layer Slot for : " + LayerName);
                    }
                }
            }

            Manager.ApplyModifiedProperties();

            return Result;
        }

        public static void CreateNewPiece()
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<BuildManager>() == null)
            {
                if (Selection.activeGameObject.GetComponentInParent<PieceBehaviour>() == null)
                {
                    string LocalPath = EditorUtility.SaveFilePanel("Define saving path for piece...", "", Selection.activeGameObject.name + ".prefab", "prefab");

                    if (LocalPath == string.Empty)
                    {
                        return;
                    }

                    try
                    {
                        LocalPath = LocalPath.Substring(LocalPath.LastIndexOf("Assets"));
                    }
                    catch { return; }

                    if (LocalPath != string.Empty)
                    {
                        GameObject Parent = new GameObject(LocalPath);

                        Selection.activeGameObject.transform.SetParent(Parent.transform, false);

                        Selection.activeGameObject.transform.position = new Vector3(0, Selection.activeGameObject.transform.position.y, 0);

                        PieceBehaviour Temp = Parent.AddComponent<PieceBehaviour>();

                        Temp.Name = Selection.activeGameObject.name;
                        Temp.gameObject.name = Temp.Name;

#if UNITY_2018_3 || UNITY_2019
                        UnityEngine.Object AssetPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(Temp.gameObject, LocalPath, InteractionMode.UserAction);

                        EditorGUIUtility.PingObject(AssetPrefab);

                        Parent.name = AssetPrefab.name;
#else

#if UNITY_2019_4_OR_NEWER
                        LocalPath = AssetDatabase.GenerateUniqueAssetPath(LocalPath);
                        GameObject AssetPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(Temp.gameObject, LocalPath, InteractionMode.UserAction);
                        EditorGUIUtility.PingObject(AssetPrefab);
#else
                        UnityEngine.Object AssetPrefab = PrefabUtility.CreateEmptyPrefab(LocalPath);

                        GameObject Asset = PrefabUtility.ReplacePrefab(Parent, AssetPrefab, ReplacePrefabOptions.ConnectToPrefab);
                        
                        EditorGUIUtility.PingObject(Asset);
#endif

                        AssetDatabase.Refresh();
#endif

                        SceneHelper.Focus(Parent);
                    }
                }
                else
                {
                    Debug.LogError("<b>Easy Build System</b> : This selected object has already a Piece Behaviour component.");
                }
            }
            else
            {
                if (!EditorUtility.DisplayDialog("Easy Build System - Information", "You've not selected object.\nDo you want create a empty Piece Behaviour ?", "Yes", "No"))
                {
                    return;
                }

                GameObject Parent = new GameObject("New Piece");

                string LocalPath = EditorUtility.SaveFilePanel("Save Path to Part (" + Parent.name + ")", "", Parent.name + ".prefab", "prefab");

                if (LocalPath == string.Empty)
                {
                    DestroyImmediate(Parent);
                    return;
                }

                try
                {
                    LocalPath = LocalPath.Substring(LocalPath.LastIndexOf("Assets"));
                }
                catch { return; }

                if (LocalPath != string.Empty)
                {
                    Parent.AddComponent<PieceBehaviour>();
#if UNITY_2019_4_OR_NEWER
                    LocalPath = AssetDatabase.GenerateUniqueAssetPath(LocalPath);
                    GameObject AssetPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(Parent, LocalPath, InteractionMode.UserAction);
                    EditorGUIUtility.PingObject(AssetPrefab);
#elif UNITY_2018_3
                    GameObject Asset = PrefabUtility.SaveAsPrefabAssetAndConnect(Parent, LocalPath, InteractionMode.UserAction);

                    EditorGUIUtility.PingObject(Asset);
#elif UNITY_2018
                    UnityEngine.Object AssetPrefab = PrefabUtility.CreateEmptyPrefab(LocalPath);

                    GameObject Asset = PrefabUtility.ReplacePrefab(Parent, AssetPrefab, ReplacePrefabOptions.ConnectToPrefab);

                    AssetDatabase.Refresh();

                    EditorGUIUtility.PingObject(Asset);
#endif
                    SceneHelper.Focus(Parent);
                }
            }
        }

        public static void CreateNewSocket()
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<BuildManager>() == null)
            {
                GameObject Child = new GameObject("New Socket");

                Child.transform.SetParent(Selection.activeGameObject.transform, false);

                Child.AddComponent<SocketBehaviour>();

                SceneHelper.Focus(Child);
            }
            else
            {
                GameObject Parent = new GameObject("New Socket");

                Parent.AddComponent<SocketBehaviour>();

                SceneHelper.Focus(Parent);
            }
        }

        public static void CreateNewArea()
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<BuildManager>() == null)
            {
                GameObject Child = new GameObject("New Area");
                Child.transform.SetParent(Selection.activeGameObject.transform, false);
                Child.AddComponent<AreaBehaviour>();
                SceneHelper.Focus(Child);
            }
            else
            {
                GameObject Parent = new GameObject("New Area");
                Parent.AddComponent<AreaBehaviour>();
                SceneHelper.Focus(Parent);
            }
        }

        #endregion
    }
}