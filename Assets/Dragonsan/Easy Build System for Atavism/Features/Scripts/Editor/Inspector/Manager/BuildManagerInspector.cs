using EasyBuildSystem.Features.Scripts.Core.Addons.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using EasyBuildSystem.Features.Scripts.Core.Scriptables.Collection;
using EasyBuildSystem.Features.Scripts.Editor.Menu;
using EasyBuildSystem.Features.Scripts.Editor.Window;
using EasyBuildSystem.Features.Scripts.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Editor.Inspector.Manager
{
    [CustomEditor(typeof(BuildManager))]
    public class BuildManagerInspector : UnityEditor.Editor
    {
        #region Fields

        private readonly List<UnityEditor.Editor> PiecePreviews = new List<UnityEditor.Editor>();
        private PieceCollection LoadCollection;
        private bool[] PieceFoldout = new bool[999];

        private BuildManager Target;

        private static bool GeneralFoldout;
        private static bool PiecesFoldout;
        private static bool BlueprintFoldout;
        private static bool AddonsFoldout;
        private static bool UtilitiesFoldout;

        #endregion Fields

        #region Methods

        private void OnEnable()
        {
            Target = (BuildManager)target;
            MainEditor.LoadAddons(Target, AddonTarget.BuildManager);
            PieceFoldout = new bool[999];
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region Build Manager General

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Build Manager - General", EditorStyles.largeLabel);

            GUI.color = Color.white;

            GeneralFoldout = EditorGUILayout.Foldout(GeneralFoldout, "General Settings");

            if (GeneralFoldout)
            {
                if (Target.BuildingSupport == Core.Base.Manager.Enums.SupportType.AnyCollider)
                {
                    EditorGUILayout.HelpBox("Build Surface is defined as 'Any Collider'.\n" +
                        "Recommended to use the 'Terrain Collider' or 'Surface Collider' to have a optimal placement.\n" +
                        "You can find more information about Build Surface in the documentation.", MessageType.Info);
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("BuildingSupport"), new GUIContent("Build Surface :",
                    "Define the type of support to be able to place all the pieces of type (Foundation).\n\n" +
                    "(All) Allow only the placement on all the gameObject(s).\n\n" +
                    "(Terrain) Allow only the placement on Unity Terrain.\n\n" +
                    "(Voxeland) Allow only the placement on Voxeland Terrain.\n\n" +
                    "(Surface) Allow only the placement on the colliders with the component Surface Collider."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("FreeLayers"), new GUIContent("Build Free Layers :", "Define the layer(s) on which the preview will be placed and moved."));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("DefaultPreviewMaterial"), new GUIContent("Build Preview Material :", "Define the layer(s) that will be detected to snap the preview."));
            }

            #endregion

            #region Build Manager Pieces

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Build Manager - Pieces", EditorStyles.largeLabel);

            GUI.color = Color.white;

            PiecesFoldout = EditorGUILayout.Foldout(PiecesFoldout, "Pieces Settings");

            if (PiecesFoldout)
            {
                bool Flag = false;

                if (PieceFoldout == null)
                {
                    PieceFoldout = new bool[Target.Pieces.Count];
                }

                for (int i = 0; i < Target.Pieces.Count; i++)
                {
                    if (Target.Pieces[i] == null)
                    {
                        Flag = true;
                    }
                }

                if (Flag)
                {
                    Target.Pieces = Target.Pieces.Where(s => s != null).Distinct().ToList();
                }

                int Index = 0;

                EditorGUILayout.BeginVertical("box");

                if (Target.Pieces.Count == 0)
                {
                    GUILayout.Label("Pieces list does not contains any piece(s).");
                }
                else
                {
                    foreach (PieceBehaviour Piece in Target.Pieces)
                    {
                        if (Piece == null)
                        {
                            return;
                        }

                        GUILayout.BeginHorizontal();

                        GUILayout.Space(13);

                        EditorGUI.BeginChangeCheck();

                        string Format = string.Format("[Index:{0}] ", Index) + Piece.name;

                        PieceFoldout[Index] = EditorGUILayout.Foldout(PieceFoldout[Index],
                            Format);

                        if (EditorGUI.EndChangeCheck())
                        {
                            if (PieceFoldout[Index] == true)
                            {
                                for (int i = 0; i < PieceFoldout.Length; i++)
                                {
                                    if (i != Index)
                                    {
                                        PieceFoldout[i] = false;
                                    }
                                }
                            }

                            PiecePreviews.Clear();
                        }

                        GUI.color = Color.white;

                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();

                        GUILayout.Space(5);

                        if (PieceFoldout[Index])
                        {
                            GUILayout.BeginHorizontal();

                            GUILayout.BeginVertical("box");

                            if (Piece != null)
                            {
                                GUILayout.BeginHorizontal();

                                UnityEditor.Editor PreviewEditor = null;

                                if (PiecePreviews.Count > Index)
                                {
                                    PreviewEditor = PiecePreviews[Index];
                                }

                                if (PreviewEditor == null)
                                {
                                    PreviewEditor = CreateEditor(Piece.gameObject);

                                    PiecePreviews.Add(PreviewEditor);

                                    PreviewEditor.OnPreviewGUI(GUILayoutUtility.GetRect(128, 128), EditorStyles.textArea);
                                }
                                else
                                {
                                    PreviewEditor.OnPreviewGUI(GUILayoutUtility.GetRect(128, 128), EditorStyles.textArea);
                                }

                                GUILayout.BeginVertical("box");

                                GUI.color = MainEditor.GetEditorColor;

                                if (GUILayout.Button("Select Piece", GUILayout.Width(120)))
                                {
                                    EditorGUIUtility.PingObject(Piece.gameObject);
                                    Selection.activeObject = Piece.gameObject;
                                }

                                if (GUILayout.Button("Increase Index", GUILayout.Width(120)))
                                {
                                    try
                                    {
                                        ListExtension.Move<PieceBehaviour>(Target.Pieces, Target.Pieces.IndexOf(Piece), ListExtension.MoveDirection.Increase);

                                        PieceFoldout[Index] = false;

                                        PieceFoldout[Target.Pieces.IndexOf(Piece)] = true;

                                        Repaint();

                                        EditorUtility.SetDirty(target);
                                    }
                                    catch
                                    {
                                    }

                                    Repaint();

                                    EditorUtility.SetDirty(target);

                                    PiecePreviews.Clear();

                                    break;
                                }

                                if (GUILayout.Button("Decrease Index", GUILayout.Width(120)))
                                {
                                    try
                                    {
                                        int NewIndex = Target.Pieces.IndexOf(Piece);

                                        ListExtension.Move<PieceBehaviour>(Target.Pieces, NewIndex, ListExtension.MoveDirection.Decrease);

                                        PieceFoldout[Index] = false;

                                        PieceFoldout[Target.Pieces.IndexOf(Piece)] = true;

                                        Repaint();

                                        EditorUtility.SetDirty(target);
                                    }
                                    catch
                                    {
                                    }

                                    Repaint();

                                    EditorUtility.SetDirty(target);

                                    PiecePreviews.Clear();

                                    break;
                                }

                                GUI.color = new Color(1f, 0, 0);

                                if (GUILayout.Button("Remove Piece", GUILayout.Width(120)))
                                {
                                    Target.Pieces.Remove(Piece);

                                    Repaint();

                                    EditorUtility.SetDirty(target);

                                    PiecePreviews.Clear();

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

                GUI.Box(DropRect, "You can drag and drop your piece to add it in the list.", EditorStyles.centeredGreyMiniLabel);

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

                            PieceBehaviour DraggedPiece = DraggedObject.GetComponent<PieceBehaviour>();

                            if (DraggedPiece == null)
                            {
                                Debug.LogError("<b>Easy Build System</b> : Only piece can be added to list!");
                                return;
                            }

                            if (Target.Pieces.Find(entry => entry.Id == DraggedPiece.Id) == null)
                            {
                                Target.Pieces.Add(DraggedPiece);
                                EditorUtility.SetDirty(target);
                                Repaint();
                            }
                            else
                            {
                                Debug.LogError("<b>Easy Build System</b> : This piece is already exists in the list.");
                            }
                        }

                        UnityEngine.Event.current.Use();
                    }
                }

                GUILayout.EndVertical();

                GUILayout.BeginVertical("box");

                LoadCollection = (PieceCollection)EditorGUILayout.ObjectField("Pieces Collection :", LoadCollection, typeof(PieceCollection), false);

                GUI.color = MainEditor.GetEditorColor;

                GUI.enabled = LoadCollection != null;

                if (GUILayout.Button("Load Pieces Collection"))
                {
                    Target.Pieces.AddRange(LoadCollection.Pieces);
                    LoadCollection = null;
                    Repaint();
                    EditorUtility.SetDirty(target);
                }

                GUI.enabled = true;

                if (GUILayout.Button("Clear All Piece(s) List"))
                {
                    Target.Pieces.Clear();
                    Repaint();
                    EditorUtility.SetDirty(target);
                }

                GUI.enabled = true;

                GUI.color = Color.white;

                GUILayout.EndVertical();
            }

            #endregion Build Manager Pieces

            #region Build Manager Blueprint

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Build Manager - Blueprint", EditorStyles.largeLabel);

            GUI.color = Color.white;

            BlueprintFoldout = EditorGUILayout.Foldout(BlueprintFoldout, "Blueprint Settings");

            if (BlueprintFoldout)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(13);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Blueprints"), new GUIContent("Blueprint(s) :", ""), true);
                GUILayout.EndHorizontal();

                GUI.color = MainEditor.GetEditorColor;

                GUI.enabled = Target.Blueprints.Length > 0;

                if (GUILayout.Button("Load All Blueprint(s) In Editor"))
                {
                    Target.LoadBlueprintInEditor();
                }

                GUI.enabled = Application.isPlaying;

                if (GUILayout.Button("Load All Blueprint(s) In Runtime Editor"))
                {
                    Target.LoadBlueprintInRuntimeEditor();
                }

                GUI.enabled = true;

                GUI.color = Color.white;
            }

            #endregion

            #region Building Manager Addons

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Build Manager - Addons", EditorStyles.largeLabel);

            GUI.color = Color.white;

            AddonsFoldout = EditorGUILayout.Foldout(AddonsFoldout, "Addons Settings");

            if (AddonsFoldout)
            {
                MainEditor.DrawAddons(Target, AddonTarget.BuildManager);
            }

            #endregion Build Manager Addnns

            #region Build Manager Utilities

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Build Manager - Utilities", EditorStyles.largeLabel);

            GUI.color = Color.white;

            UtilitiesFoldout = EditorGUILayout.Foldout(UtilitiesFoldout, "Utilities Settings");

            if (UtilitiesFoldout)
            {
                GUI.color = MainEditor.GetEditorColor;

                if (GUILayout.Button("Open Editor"))
                {
                    MainEditor.Init();
                }

                if (GUILayout.Button("Quick Start")) { QuickStart.Init(); }

                if (GUILayout.Button("Check Missing Layer(s)")) { MainEditor.CheckMissingLayers(); }

                GUI.color = Color.white;

                #region Build Manager Components

                GUI.color = MainEditor.GetEditorColor;

                GUILayout.Label("Components");

                if (GUILayout.Button("Create New Area Behaviour")) { MenuItems.EditorCreateArea(); }

                if (GUILayout.Button("Create New Piece Behaviour")) { MenuItems.EditorCreatePiece(); }

                if (GUILayout.Button("Create New Socket Behaviour")) { MenuItems.EditorCreateSocket(); }

                #endregion

                #region Build Manager Scriptable Object(s)

                GUILayout.Label("Scriptable Object(s)");

                if (GUILayout.Button("Create New Pieces Collection")) { MenuItems.EditorCreatePieceCollection(); }

                if (GUILayout.Button("Create New Blueprint Template")) { MenuItems.EditorCreateBlueprintData(); }

                #endregion

                #region Build Manager Code Generator

                GUILayout.Label("Code Generator");

                if (GUILayout.Button("Create New Addon Template")) { MenuItems.EditorCreateAddonScript(); }

                if (GUILayout.Button("Create New Condition Template")) { MenuItems.EditorCreateConditionScript(); }

                    #endregion
            }

            #endregion

            serializedObject.ApplyModifiedProperties();

            #endregion
        }
    }
}