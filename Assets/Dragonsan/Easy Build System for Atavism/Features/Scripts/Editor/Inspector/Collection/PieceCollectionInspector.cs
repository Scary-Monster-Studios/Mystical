using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using EasyBuildSystem.Features.Scripts.Core.Scriptables.Collection;
using EasyBuildSystem.Features.Scripts.Editor.Window;
using EasyBuildSystem.Features.Scripts.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Editor.Inspector.Collection
{
    [CustomEditor(typeof(PieceCollection))]
    public class PieceCollectionInspector : UnityEditor.Editor
    {
        #region Fields

        private PieceCollection Target;
        private PieceCollection LoadCollection;
        private bool[] PieceFoldout = new bool[999];
        private readonly List<UnityEditor.Editor> PiecePreviews = new List<UnityEditor.Editor>();

        #endregion Fields

        #region Methods

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region Parts Collection Settings

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Pieces Collection - General", EditorStyles.largeLabel);

            GUI.color = Color.white;

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
                Target.Pieces.Clear();
            }

            int Index = 0;

            GUILayout.BeginHorizontal();

            GUI.color = MainEditor.GetEditorColor;

            if (GUILayout.Button("Sort By Name"))
            {
                Target.Pieces = Target.Pieces.OrderBy(e => e.Name).ToList();
            }

            if (GUILayout.Button("Sort By Id"))
            {
                Target.Pieces = Target.Pieces.OrderBy(e => e.Id).ToList();
            }

            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical("box");

            if (Target.Pieces.Count == 0)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label("Pieces list does not contains any piece(s).");

                GUILayout.EndHorizontal();
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

                            if (GUILayout.Button("Remove", GUILayout.Width(120)))
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

            #endregion Parts Collection Settings

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            Target = (PieceCollection)target;
            PieceFoldout = new bool[999];
        }

        #endregion Methods
    }
}