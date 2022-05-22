using EasyBuildSystem.Features.Scripts.Core;
using EasyBuildSystem.Features.Scripts.Core.Addons.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Condition;
using EasyBuildSystem.Features.Scripts.Core.Base.Condition.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Condition.Helper;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using EasyBuildSystem.Features.Scripts.Core.Base.Socket;
using EasyBuildSystem.Features.Scripts.Core.Base.Socket.Data;
using EasyBuildSystem.Features.Scripts.Editor.Helper;
using EasyBuildSystem.Features.Scripts.Editor.Window;
using EasyBuildSystem.Features.Scripts.Extensions;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Editor.Inspector.Piece
{
    [CustomEditor(typeof(PieceBehaviour))]
    [CanEditMultipleObjects]
    public class PieceBehaviourInspector : UnityEditor.Editor
    {
        #region Fields

        public static Rect BoundsEditingWindowRect = new Rect(50, 50, 300, 145);

        private PieceBehaviour Target;

        private List<ConditionAttribute> Conditions = new List<ConditionAttribute>();

        private GameObject CurrentPreview;
        private int PreviewIndex;

        private static bool[] AppearancesFolds = new bool[999];
        private readonly List<UnityEditor.Editor> AppearancePreviews = new List<UnityEditor.Editor>();
        private readonly List<UnityEditor.Editor> CachedEditors = new List<UnityEditor.Editor>();
        private List<GameObject> Previews = new List<GameObject>();

        private static bool GeneralFoldout;
        private static bool PreviewFoldout;
        private static bool BoundsFoldout;
        private static bool ConditionFoldout;
        private static bool AppearancesFoldout;
        private static bool AddonsFoldout;
        private static bool DebuggingFoldout;
        private static bool UtilitiesFoldout;

        private static bool ShowPreviewEditor;

        #endregion Fields

        #region Methods

        private void OnEnable()
        {
            Target = (PieceBehaviour)target;

            Conditions = ConditionHelper.GetConditionsByTarget(ConditionTarget.PieceBehaviour);

            foreach (ConditionBehaviour Condition in Target.GetComponentsInChildren<ConditionBehaviour>())
            {
                if (Condition != null)
                {
                    Condition.hideFlags = HideFlags.HideInInspector;
                }
            }

            MainEditor.LoadAddons(Target, AddonTarget.PieceBehaviour);
        }

        private void OnDisable()
        {
            ShowPreviewEditor = false;
            ClearPreviews();
        }

        private void OnSceneGUI()
        {
            if (SceneView.lastActiveSceneView.camera == null)
            {
                return;
            }

            if (ShowPreviewEditor)
            {
                if (CurrentPreview == null)
                {
                    int Index = 0;

                    foreach (SocketBehaviour ChildSocket in Target.transform.GetComponentsInChildren<SocketBehaviour>())
                    {
                        if (ChildSocket != null)
                        {
                            if (ChildSocket.PartOffsets.Count > PreviewIndex)
                            {
                                Offset OffsetPiece = ChildSocket.PartOffsets[PreviewIndex];

                                CurrentPreview = Instantiate(OffsetPiece.Piece.gameObject);

                                foreach (SocketBehaviour Socket in CurrentPreview.GetComponentsInChildren<SocketBehaviour>())
                                {
                                    DestroyImmediate(Socket);
                                }

                                CurrentPreview.transform.position = ChildSocket.transform.TransformPoint(OffsetPiece.Position);

                                CurrentPreview.transform.rotation = ChildSocket.transform.rotation * Quaternion.Euler(OffsetPiece.Rotation);

                                CurrentPreview.transform.localScale = OffsetPiece.Scale != Vector3.one ? OffsetPiece.Scale : ChildSocket.transform.parent.localScale;

                                CurrentPreview.hideFlags = HideFlags.HideInHierarchy;

                                Material PreviewMaterial = new Material(Shader.Find(Constants.TRANSPARENT_SHADER_NAME))
                                {
                                    color = new Color(0, 1f, 1f, 0.5f)
                                };

                                CurrentPreview.ChangeAllMaterialsInChildren(CurrentPreview.GetComponentsInChildren<MeshRenderer>(), PreviewMaterial);

                                Previews.Add(CurrentPreview);

                                Index++;
                            }
                        }
                    }

                    SceneHelper.Focus(target);
                }
            }

            if (Target.UseGroundUpper)
            {
                Handles.color = Color.green;
                Handles.DrawLine(Target.transform.position, Target.transform.position + Vector3.down * Target.GroundUpperHeight);
            }
        }

        private void ClearPreviews()
        {
            foreach (GameObject Preview in Previews)
            {
                DestroyImmediate(Preview);
            }

            Previews.Clear();
            Previews = new List<GameObject>();

            for (int i = 0; i < CachedEditors.Count; i++)
            {
                CachedEditors[i].ResetTarget();
                DestroyImmediate(CachedEditors[i]);
            }

            CachedEditors.Clear();
        }
        UnityEditor.Editor ConditionEditor;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region Part Behaviour General

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Piece Behaviour - General", EditorStyles.largeLabel);

            GUI.color = Color.white;

            GeneralFoldout = EditorGUILayout.Foldout(GeneralFoldout, "General Settings");

            if (GeneralFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Id"), new GUIContent("Piece Id :", "Define id of this piece."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("Name"), new GUIContent("Piece Name :", "Define the name of this piece."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("Description"), new GUIContent("Piece Description :", "Define the description of this piece."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("Category"), new GUIContent("Piece Category :", "Define the category of this piece."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("Icon"), new GUIContent("Piece Icon :", "Define the icon of this piece."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("IsEditable"), new GUIContent("Piece Editable :", "Define if this piece can be edited via the edition mode."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("IsDestructible"), new GUIContent("Piece Destructible :", "Define if this piece can be destroyed via the destruction mode."));
            }

            #endregion Part Behaviour Settings

            #region Part Behaviour Preview

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Piece Behaviour - Preview", EditorStyles.largeLabel);

            GUI.color = Color.white;

            PreviewFoldout = EditorGUILayout.Foldout(PreviewFoldout, "Preview Settings");

            if (PreviewFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("FreePlacement"), new GUIContent("Preview Free Movement :", "Define if the piece can be placed freely."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("RequireSocket"), new GUIContent("Preview Require Sockets :", "Define if the piece require a socket."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("IgnoreSocket"), new GUIContent("Preview Ignore Sockets :", "Define if the piece ignore the socket(s)."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("UseGroundUpper"), new GUIContent("Preview Ground Upper :", "This allows to to raise from ground the preview on the Y axis."));

                if (serializedObject.FindProperty("UseGroundUpper").boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("GroundUpperHeight"), new GUIContent("Preview Ground Upper Height :", "Define the maximum limit not to exceed on the Y axis."));
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("RotateOnSockets"), new GUIContent("Preview Can Rotate On Socket :", "Define if the preview can be rotated on socket."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("RotateAccordingSlope"), new GUIContent("Preview Rotate According Slope :", "This allows to rotate the preview according to collider slope."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("RotationAxis"), new GUIContent("Preview Rotate Axis :", "Define on what axis the preview will be rotated."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("PreviewOffset"), new GUIContent("Preview Position Offset :", "Define the preview offset position."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("PreviewUseColorLerpTime"), new GUIContent("Preview Color Lerp Time :", "This allows to lerp the preview color when the placement is possible or no."));

                if (serializedObject.FindProperty("PreviewUseColorLerpTime").boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("PreviewColorLerpTime"), new GUIContent("Part Preview Color Lerp Time :", "Define the transition speed on the preview color."));
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("UseDefaultPreviewMaterial"), new GUIContent("Preview Custom Material :", "This allows to use a custom material for the preview."));

                if (serializedObject.FindProperty("UseDefaultPreviewMaterial").boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("CustomPreviewMaterial"), new GUIContent("Preview Custom Material :", "Define here the custom material."));
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("PreviewAllowedColor"), new GUIContent("Preview Allowed Color :", "This allows to show the allowed color when the preview can be placed."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("PreviewDeniedColor"), new GUIContent("Preview Denied Color :", "This allows to show the denied color when the preview can't be placed."));

                GUILayout.BeginHorizontal();
                GUILayout.Space(13);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("PreviewDisableObjects"), new GUIContent("Disable Object(s) In Preview State :", "This allows to disable some object(s) when the piece is in preview state."), true);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(13);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("PreviewDisableBehaviours"), new GUIContent("Disable Mono Behaviour(s) In Preview State :", "This allows to disable some monobehaviour(s) when the piece is in preview/queue/remove/edit state."), true);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(13);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("PreviewDisableColliders"), new GUIContent("Disable Collider(s) In Preview State :", "This allows to disable some collider(s) when the piece is in preview state."), true);
                GUILayout.EndHorizontal();
            }

            #endregion

            #region Part Behaviour Bounds

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Piece Behaviour - Bounds", EditorStyles.largeLabel);

            GUI.color = Color.white;

            BoundsFoldout = EditorGUILayout.Foldout(BoundsFoldout, "Bounds Settings");

            if (BoundsFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("MeshBounds"), new GUIContent("Piece Mesh Bounds :", "Define id of this piece."));

                GUI.color = MainEditor.GetEditorColor;

                if (GUILayout.Button("Generate Smart Bounds"))
                {
                    Target.MeshBounds = Target.gameObject.GetChildsBounds();
                }
            }

            GUI.color = Color.white;

            #endregion Part Behaviour Bounds

            #region Part Behaviour Conditions

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Piece Behaviour - Conditions", EditorStyles.largeLabel);

            GUI.color = Color.white;

            ConditionFoldout = EditorGUILayout.Foldout(ConditionFoldout, "Conditions Settings");

            if (ConditionFoldout)
            {
                EditorGUILayout.BeginVertical();

                if (Conditions.Count == 0)
                {
                    GUI.color = Color.black / 4f;
                    GUILayout.BeginVertical("helpBox");
                    GUI.color = Color.white;
                    GUILayout.Label("No conditions was found for this component.");
                    GUILayout.EndVertical();
                }

                foreach (ConditionAttribute Condition in Conditions)
                {
                    GUI.color = Color.black / 4f;
                    GUILayout.BeginVertical("helpBox");
                    GUI.color = Color.white;

                    GUILayout.BeginHorizontal();

                    GUILayout.Label(Condition.Name, EditorStyles.largeLabel);

                    GUILayout.FlexibleSpace();

                    if (Target.gameObject.GetComponent(Condition.Behaviour) != null)
                    {
                        GUI.color = MainEditor.GetEditorColor;

                        if (GUILayout.Button("Copy Settings", GUILayout.Width(120), GUILayout.Height(17)))
                        {
                            UnityEditorInternal.ComponentUtility.CopyComponent(Target.GetComponent(Condition.Behaviour));
                        }

                        if (GUILayout.Button("Paste Settings", GUILayout.Width(120), GUILayout.Height(17)))
                        {
                            UnityEditorInternal.ComponentUtility.PasteComponentValues(Target.GetComponent(Condition.Behaviour));
                            serializedObject.ApplyModifiedProperties();
                        }

                        GUI.color = Color.white;

                        GUI.color = new Color(1f, 0f, 0f);
                        if (GUILayout.Button("Disable Condition", GUILayout.Width(120), GUILayout.Height(17)))
                        {
                            try
                            {
                                DestroyImmediate(Target.gameObject.GetComponent(Condition.Behaviour), true);
                                break;
                            }
                            catch { }
                        }
                        GUI.color = Color.white;
                    }
                    else
                    {
                        GUI.color = new Color(0f, 1f, 0f);
                        if (GUILayout.Button("Enable Condition", GUILayout.Width(120), GUILayout.Height(17)))
                        {
                            if (Target.gameObject.GetComponent(Condition.Behaviour) != null)
                            {
                                return;
                            }

                            Component Com = Target.gameObject.AddComponent(Condition.Behaviour);
                            Com.hideFlags = HideFlags.HideInInspector;
                        }
                        GUI.color = Color.white;
                    }

                    GUILayout.EndHorizontal();

                    GUI.color = Color.white;

                    GUILayout.Label(Condition.Description, EditorStyles.wordWrappedMiniLabel);

                    GUILayout.BeginHorizontal(); 
                    if (Target.gameObject.GetComponent(Condition.Behaviour) != null)
                    {
                        GUILayout.BeginVertical();

                        ConditionEditor = CreateEditor(Target.gameObject.GetComponent(Condition.Behaviour));
                        ConditionEditor.OnInspectorGUI();

                        GUILayout.EndVertical();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                }

                EditorGUILayout.EndVertical();

            }

#endregion

            #region Part Behaviour Appearances

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Piece Behaviour - Appearances", EditorStyles.largeLabel);

            GUI.color = Color.white;

            AppearancesFoldout = EditorGUILayout.Foldout(AppearancesFoldout, "Appearances Settings");

            if (AppearancesFoldout)
            {
                bool Flag = false;
                
                if (AppearancesFolds == null)
                {
                    AppearancesFolds = new bool[serializedObject.FindProperty("Appearances").arraySize];
                }

                for (int i = 0; i < serializedObject.FindProperty("Appearances").arraySize; i++)
                {
                    if (Target.Appearances[i] == null)
                    {
                        Flag = true;
                    }
                }

                if (Flag)
                {
                    Target.Appearances.Clear();
                }

                int Index = 0;

                EditorGUILayout.BeginVertical("box");

                if (serializedObject.FindProperty("Appearances").arraySize == 0)
                {
                    GUILayout.Label("Appearances list does not contains any transform child(s).");
                }
                else
                {
                    foreach (GameObject Appearance in Target.Appearances)
                    {
                        if (Appearance == null)
                        {
                            return;
                        }

                        GUILayout.BeginHorizontal();

                        GUILayout.Space(13);

                        EditorGUI.BeginChangeCheck();

                        string Format = string.Format("[Index:{0}] ", Index) + Appearance.name;

                        AppearancesFolds[Index] = EditorGUILayout.Foldout(AppearancesFolds[Index], Format);

                        if (EditorGUI.EndChangeCheck())
                        {
                            if (AppearancesFolds[Index] == true)
                            {
                                for (int i = 0; i < AppearancesFolds.Length; i++)
                                {
                                    if (i != Index)
                                    {
                                        AppearancesFolds[i] = false;
                                    }
                                }

                                for (int x = 0; x < Target.Appearances.Count; x++)
                                {
                                    if (x == Index)
                                    {
                                        Target.Appearances[x].SetActive(true);
                                    }
                                    else
                                    {
                                        Target.Appearances[x].SetActive(false);
                                    }
                                }
                            }
                            else
                            {
                                for (int x = 0; x < Target.Appearances.Count; x++)
                                {
                                    if (x == Target.AppearanceIndex)
                                    {
                                        Target.Appearances[x].SetActive(true);
                                    }
                                    else
                                    {
                                        Target.Appearances[x].SetActive(false);
                                    }
                                }
                            }

                            SceneHelper.Focus(Appearance, false);

                            AppearancePreviews.Clear();
                        }

                        GUI.color = MainEditor.GetEditorColor;

                        if (Target.AppearanceIndex == Index)
                        {
                            GUI.enabled = false;
                        }

                        if (GUILayout.Button("Define As Default Appearance", GUILayout.Width(190)))
                        {
                            for (int i = 0; i < AppearancesFolds.Length; i++)
                            {
                                AppearancesFolds[i] = false;
                            }

                            Target.ChangeAppearance(Index);

                            for (int x = 0; x < Target.Appearances.Count; x++)
                            {
                                if (x == Target.AppearanceIndex)
                                {
                                    Target.Appearances[x].SetActive(true);
                                }
                                else
                                {
                                    Target.Appearances[x].SetActive(false);
                                }
                            }

                            Repaint();

                            EditorUtility.SetDirty(target);
                        }

                        GUI.enabled = true;

                        GUI.color = Color.white;

                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();

                        GUILayout.Space(5);

                        if (AppearancesFolds[Index])
                        {
                            GUILayout.BeginHorizontal();

                            GUILayout.BeginVertical("box");

                            if (Appearance != null)
                            {
                                GUILayout.BeginHorizontal();

                                UnityEditor.Editor PreviewEditor = null;

                                if (AppearancePreviews.Count > Index)
                                {
                                    PreviewEditor = AppearancePreviews[Index];
                                }

                                if (PreviewEditor == null)
                                {
                                    PreviewEditor = CreateEditor(Appearance);

                                    AppearancePreviews.Add(PreviewEditor);

                                    PreviewEditor.OnPreviewGUI(GUILayoutUtility.GetRect(128, 128), EditorStyles.textArea);

                                    CachedEditors.Add(PreviewEditor);
                                }
                                else
                                {
                                    PreviewEditor.OnPreviewGUI(GUILayoutUtility.GetRect(128, 128), EditorStyles.textArea);
                                }

                                GUILayout.BeginVertical("box");

                                GUI.color = MainEditor.GetEditorColor;

                                if (GUILayout.Button("Select Child", GUILayout.Width(120)))
                                {

                                }

                                if (GUILayout.Button("Increase Index", GUILayout.Width(120)))
                                {
                                    try
                                    {
                                        ListExtension.Move<GameObject>(Target.Appearances, Target.Appearances.IndexOf(Appearance), ListExtension.MoveDirection.Increase);

                                        AppearancesFolds[Index] = false;

                                        AppearancesFolds[Target.Appearances.IndexOf(Appearance)] = true;

                                        Repaint();

                                        EditorUtility.SetDirty(target);
                                    }
                                    catch
                                    {
                                    }

                                    Repaint();

                                    EditorUtility.SetDirty(target);

                                    AppearancePreviews.Clear();

                                    break;
                                }

                                if (GUILayout.Button("Decrease Index", GUILayout.Width(120)))
                                {
                                    try
                                    {
                                        int NewIndex = Target.Appearances.IndexOf(Appearance);

                                        ListExtension.Move<GameObject>(Target.Appearances, NewIndex, ListExtension.MoveDirection.Decrease);

                                        AppearancesFolds[Index] = false;

                                        AppearancesFolds[Target.Appearances.IndexOf(Appearance)] = true;

                                        Repaint();

                                        EditorUtility.SetDirty(target);
                                    }
                                    catch
                                    {
                                    }

                                    Repaint();

                                    EditorUtility.SetDirty(target);

                                    AppearancePreviews.Clear();

                                    break;
                                }

                                GUI.color = new Color(1f, 0, 0);

                                if (GUILayout.Button("Remove Child", GUILayout.Width(120)))
                                {
                                    Target.Appearances.Remove(Appearance);

                                    Repaint();

                                    EditorUtility.SetDirty(target);

                                    AppearancePreviews.Clear();

                                    break;
                                }

                                GUI.color = Color.white;

                                GUILayout.FlexibleSpace();

                                GUILayout.EndVertical();

                                GUILayout.EndHorizontal();
                            }

                            GUILayout.FlexibleSpace();

                            GUILayout.EndVertical();

                            GUILayout.EndHorizontal();
                        }

                        GUILayout.EndHorizontal();

                        Index++;
                    }
                }

                EditorGUILayout.EndVertical();

                GUILayout.BeginVertical("box");

                Rect DropRect = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true));

                GUI.Box(DropRect, "You can drag and drop your transform child to add it in the list.", EditorStyles.centeredGreyMiniLabel);

                if (DropRect.Contains(UnityEngine.Event.current.mousePosition))
                {
                    if (UnityEngine.Event.current.type == EventType.DragUpdated)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        UnityEngine.Event.current.Use();
                    }
                    else if (UnityEngine.Event.current.type == EventType.DragPerform)
                    {
                        for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                        {
                            GameObject DraggedObject = DragAndDrop.objectReferences[i] as GameObject;

                            if (DraggedObject == null)
                            {
                                Debug.LogError("<b>Easy Build System</b> : Cannot add empty child!");
                                return;
                            }

                            if (!DraggedObject.transform.IsChildOf(Target.transform))
                            {
                                Debug.LogError("<b>Easy Build System</b> : This child does not exist in this transform!");
                                return;
                            }

                            if (Target.Appearances.Contains(DraggedObject) == false)
                            {
                                Target.Appearances.Add(DraggedObject);

                                for (int x = 0; x < Target.Appearances.Count; x++)
                                {
                                    if (x == Target.AppearanceIndex)
                                    {
                                        Target.Appearances[x].SetActive(true);
                                    }
                                    else
                                    {
                                        Target.Appearances[x].SetActive(false);
                                    }
                                }

                                EditorUtility.SetDirty(target);

                                Repaint();
                            }
                            else
                            {
                                Debug.LogError("<b>Easy Build System</b> : This child already exists in the list!");
                            }
                        }
                        UnityEngine.Event.current.Use();
                    }
                }

                GUILayout.EndVertical();
            }

#endregion

#region Part Behaviour Addons

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Piece Behaviour - Addons", EditorStyles.largeLabel);

            GUI.color = Color.white;

            AddonsFoldout = EditorGUILayout.Foldout(AddonsFoldout, "Addons Settings");

            if (AddonsFoldout)
            {
                GUILayout.BeginVertical();

                MainEditor.DrawAddons(Target, AddonTarget.PieceBehaviour);

                GUILayout.EndVertical();
            }

#endregion

#region Piece Behaviour Debugging

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Piece Behaviour - Debugging", EditorStyles.largeLabel);

            GUI.color = Color.white;

            DebuggingFoldout = EditorGUILayout.Foldout(DebuggingFoldout, "Debugging Settings");

            if (DebuggingFoldout)
            {
                EditorGUILayout.HelpBox("Please note that the debugging state here are set only dunring the runtime!\n" +
                    "You can also pass inspector in debug state to have more information about this component.", MessageType.Info);
                GUI.enabled = false;
                EditorGUILayout.Toggle("Piece Has Group :", (Target.GetComponentInParent<Core.Base.Group.GroupBehaviour>() != null));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("CurrentState"), new GUIContent("Piece Current State :", "Current state of the piece."));
                GUILayout.BeginHorizontal();
                GUILayout.Space(13);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("LinkedPieces"), new GUIContent("Piece Linked Pieces :", "All the linked pieces for the collapsing physics logic."), true);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Space(13);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Sockets"), new GUIContent("Piece Socket(s) :", "All the sockets that the piece contains"), true);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Space(13);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Colliders"), new GUIContent("Piece Colliders :", "All collider(s) of the piece."), true);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Space(13);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Renderers"), new GUIContent("Piece Renderers :", "All renderer(s) of the piece."), true);
                GUILayout.EndHorizontal();
                GUI.enabled = true;
            }

            GUI.color = Color.white;

#endregion Piece Debugging Settings

#region Part Behaviour Utilities

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Piece Behaviour - Utilities", EditorStyles.largeLabel);

            GUI.color = Color.white;

            UtilitiesFoldout = EditorGUILayout.Foldout(UtilitiesFoldout, "Utilities Settings");

            if (UtilitiesFoldout)
            {
                GUI.color = MainEditor.GetEditorColor;

                bool EmptyOffset = false;

                for (int i = 0; i < Target.transform.GetComponentsInChildren<SocketBehaviour>().Length; i++)
                {
                    if (!EmptyOffset)
                    {
                        EmptyOffset = Target.transform.GetComponentsInChildren<SocketBehaviour>()[i].PartOffsets.Count > 0;
                    }
                }

                GUI.enabled = (Target.transform.GetComponentsInChildren<SocketBehaviour>().Length != 0) && EmptyOffset;

                if (!ShowPreviewEditor)
                {
                    if (GUILayout.Button("Preview Offset Part(s)"))
                    {
                        ShowPreviewEditor = true;
                    }
                }
                else
                {
                    GUI.enabled = true;

                    GUILayout.BeginHorizontal();

                    EditorGUI.BeginChangeCheck();

                    if (GUILayout.Button("<", GUILayout.Width(130)))
                    {
                        if (PreviewIndex != 0)
                        {
                            PreviewIndex--;
                        }
                    }

                    GUILayout.FlexibleSpace();

                    GUI.color = Color.white;

                    try
                    {
                        if (Previews != null)
                        {
                            if (Previews[0].GetComponent<PieceBehaviour>() != null)
                            {
                                GUILayout.Label("Offset Part : " + Previews[0].GetComponent<PieceBehaviour>().Name);
                            }
                        }
                    }
                    catch { }

                    GUI.color = MainEditor.GetEditorColor;

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button(">", GUILayout.Width(130)))
                    {
                        if (PreviewIndex < Target.transform.GetComponentsInChildren<SocketBehaviour>()[0].PartOffsets.Count - 1)
                        {
                            PreviewIndex++;
                        }
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        ClearPreviews();
                    }

                    GUILayout.EndHorizontal();

                    GUI.color = new Color(1f, 1f, 0f);

                    if (GUILayout.Button("Cancel Preview"))
                    {
                        ShowPreviewEditor = false;

                        ClearPreviews();
                    }
                }

                GUI.enabled = true;

                GUI.color = MainEditor.GetEditorColor;

                if (GUILayout.Button("Create Socket Point"))
                {
                    MainEditor.CreateNewSocket();
                }
            }

#endregion

            serializedObject.ApplyModifiedProperties();
        }

#endregion Methods
    }
}