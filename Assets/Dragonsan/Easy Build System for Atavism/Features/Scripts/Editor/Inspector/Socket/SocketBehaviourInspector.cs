using EasyBuildSystem.Features.Scripts.Core;
using EasyBuildSystem.Features.Scripts.Core.Addons.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Condition;
using EasyBuildSystem.Features.Scripts.Core.Base.Condition.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Condition.Helper;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using EasyBuildSystem.Features.Scripts.Core.Base.Socket;
using EasyBuildSystem.Features.Scripts.Core.Base.Socket.Data;
using EasyBuildSystem.Features.Scripts.Core.Base.Socket.Enums;
using EasyBuildSystem.Features.Scripts.Editor.Helper;
using EasyBuildSystem.Features.Scripts.Editor.Window;
using EasyBuildSystem.Features.Scripts.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Editor.Inspector.Socket
{
    [CustomEditor(typeof(SocketBehaviour))]
    [CanEditMultipleObjects]
    public class SocketBehaviourInspector : UnityEditor.Editor
    {
        #region Enums

        public enum EditorHandleType
        {
            None,
            Position,
            Rotation,
            Scale
        }

        #endregion Enums

        #region Fields

        private SocketBehaviour Target;
        private Offset CurrentOffset;
        private GameObject PreviewPiece;

        private List<ConditionAttribute> Conditions = new List<ConditionAttribute>();

        private static bool GeneralFoldout;
        private static bool OffsetsFoldout;
        private static bool ConditionsFoldout;
        private static bool AddonsFoldout;
        private static bool DebuggingFoldout;
        private static bool UtilitiesFoldout;

        #endregion Fields

        #region Methods

        private void OnEnable()
        {
            Target = (SocketBehaviour)target;

            Conditions = ConditionHelper.GetConditionsByTarget(ConditionTarget.SocketBehaviour);

            foreach (ConditionBehaviour Condition in Target.GetComponentsInChildren<ConditionBehaviour>())
            {
                if (Condition != null)
                {
                    Condition.hideFlags = HideFlags.HideInInspector;
                }
            }

            MainEditor.LoadAddons(Target, AddonTarget.SocketBehaviour);
        }

        private void OnDisable()
        {
            ClearPreview();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region Socket Behaviour General

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Socket Behaviour - General", EditorStyles.largeLabel);

            GUI.color = Color.white;

            GeneralFoldout = EditorGUILayout.Foldout(GeneralFoldout, "General Settings");

            if (GeneralFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Type"), new GUIContent("Socket Type :", "Define the socket type."));

                if ((SocketType)serializedObject.FindProperty("Type").enumValueIndex == SocketType.Socket)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Radius"), new GUIContent("Socket Radius :", "Define the socket radius point.\nYou can decrease the socket radius to improve the precision during the detection."));
                }
                else
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("AttachmentBounds"), new GUIContent("Socket Attachment Bounds :", "Define the attachment bounds."));
                }
            }

            #endregion Socket Behaviour Settings

            #region Socket Behaviour Offsets

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Socket Behaviour - Offsets", EditorStyles.largeLabel);

            GUI.color = Color.white;

            OffsetsFoldout = EditorGUILayout.Foldout(OffsetsFoldout, "Offsets Settings");

            if (OffsetsFoldout)
            {
                if (serializedObject.FindProperty("PartOffsets").arraySize == 0)
                {
                    EditorGUILayout.BeginVertical("box");
                    GUILayout.Label("Offset list does not contains any transform child(s).");
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    int Index = 0;

                    foreach (Offset Offset in Target.PartOffsets.ToList())
                    {
                        if (Offset == null || serializedObject.FindProperty("PartOffsets").GetArrayElementAtIndex(Index).FindPropertyRelative("Referer") == null)
                        {
                            return;
                        }

                        if (CurrentOffset == Offset)
                            GUI.color = Color.yellow;
                        else
                            GUI.color = Color.white;

                        GUILayout.BeginVertical("box");

                        GUI.color = Color.white;

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("PartOffsets").GetArrayElementAtIndex(Index).FindPropertyRelative("Referer"), new GUIContent("Offset Referer :", "Define the type of referer for this offset\nThis allow to allow only specific piece or all the pieces of the same category."));
                        
                        if (serializedObject.FindProperty("PartOffsets").GetArrayElementAtIndex(Index).FindPropertyRelative("Referer").enumValueIndex == (int)OffsetRefererType.ByPiece)
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("PartOffsets").GetArrayElementAtIndex(Index).FindPropertyRelative("Piece"), new GUIContent("Offset Piece :", "Define the specific piece which can be snap on this socket."));
                        else
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("PartOffsets").GetArrayElementAtIndex(Index).FindPropertyRelative("Category"), new GUIContent("Offset Category :", "Define the category of piece which can be snap on this."));

                        EditorGUILayout.PropertyField(serializedObject.FindProperty("PartOffsets").GetArrayElementAtIndex(Index).FindPropertyRelative("Position"), new GUIContent("Offset Position :", "Define the type of this piece."));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("PartOffsets").GetArrayElementAtIndex(Index).FindPropertyRelative("Rotation"), new GUIContent("Offset Rotation :", "This allows to set the rotation of piece that will snapped on this socket."));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("PartOffsets").GetArrayElementAtIndex(Index).FindPropertyRelative("Scale"), new GUIContent("Offset Scale :", "Define the specific scale of the piece that will be snapped on this socket."));

                        GUI.color = Color.white;

                        if (PreviewPiece != null && (Offset.Referer == OffsetRefererType.ByPiece ? int.Parse(PreviewPiece.name) == Offset.Piece.Id : int.Parse(PreviewPiece.name) == BuildManager.Instance.GetPieceByCategory(Offset.Category).Id))
                        {
                            GUI.color = new Color(1f, 1f, 0f);

                            if (GUILayout.Button("Cancel Preview"))
                            {
                                ClearPreview();
                            }
                        }
                        else
                        {
                            GUI.color = MainEditor.GetEditorColor;

                            if (GUILayout.Button("Show Preview"))
                            {
                                ClearPreview();
                                CurrentOffset = Offset;
                                CreatePreview(Offset);
                            }
                        }

                        GUI.color = Color.white;

                        GUI.color = new Color(1f, 0, 0);

                        if (GUILayout.Button("Remove Offset"))
                        {
                            Target.PartOffsets.Remove(Offset);

                            ClearPreview();

                            return;
                        }

                        GUI.color = Color.white;

                        GUILayout.EndVertical();

                        Index++;
                    }
                }

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

                            if (!PrefabUtility.IsPartOfPrefabAsset(DraggedObject))
                            {
                                Debug.LogError("<b>Easy Build System</b> : This piece is a instanced prefab, please drag and drop the original prefab.");
                                return;
                            }

                            if (Target.PartOffsets.Find(entry => entry.Piece.Id == DraggedPiece.Id) == null)
                            {
                                ClearPreview();
                                Offset Offset = new Offset(DraggedPiece);
                                Target.PartOffsets.Insert(Target.PartOffsets.Count, Offset);
                                Target.PartOffsets = Target.PartOffsets.OrderBy(x => i).ToList();
                                CurrentOffset = Offset;
                                CreatePreview(Offset);
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
            }

            #endregion Socket Behaviour Offsets

            #region Socket Behaviour Conditions

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Socket Behaviour - Conditions", EditorStyles.largeLabel);

            GUI.color = Color.white;

            ConditionsFoldout = EditorGUILayout.Foldout(ConditionsFoldout, "Conditions Settings");

            if (ConditionsFoldout)
            {
                EditorGUILayout.BeginVertical();

                if (Conditions.Count == 0)
                {
                    GUILayout.BeginVertical("box");
                    GUILayout.Label("No conditions was found for this component.", EditorStyles.miniLabel);
                    GUILayout.EndVertical();
                }

                foreach (ConditionAttribute Condition in Conditions)
                {
                    GUILayout.BeginVertical("box");

                    GUILayout.BeginHorizontal();

                    GUILayout.Label(Condition.Name, EditorStyles.largeLabel);

                    GUILayout.FlexibleSpace();

                    if (Target.gameObject.GetComponent(Condition.Behaviour) != null)
                    {
                        GUI.color = MainEditor.GetEditorColor;

                        if (GUILayout.Button("Copy Settings", GUILayout.Width(120)))
                        {
                            UnityEditorInternal.ComponentUtility.CopyComponent(Target);
                        }

                        if (GUILayout.Button("Paste Settings", GUILayout.Width(120)))
                        {
                            UnityEditorInternal.ComponentUtility.PasteComponentValues(Target);
                        }

                        GUI.color = Color.white;

                        GUI.color = new Color(1f, 0f, 0f);

                        if (GUILayout.Button("Disable Condition", GUILayout.Width(120)))
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
                        GUI.color = new Color(0f, 1.5f, 0f);
                        if (GUILayout.Button("Enable Condition", GUILayout.Width(120)))
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
                    GUILayout.BeginVertical();
                    if (Target.gameObject.GetComponent(Condition.Behaviour) != null)
                    {
                        UnityEditor.Editor ConditionEditor = CreateEditor(Target.gameObject.GetComponent(Condition.Behaviour));
                        ConditionEditor.OnInspectorGUI();
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                }

                EditorGUILayout.EndVertical();
            }

            #endregion

            #region Socket Behaviour Add-Ons

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Socket Behaviour - Addons", EditorStyles.largeLabel);

            GUI.color = Color.white;

            AddonsFoldout = EditorGUILayout.Foldout(AddonsFoldout, "Addons Settings");

            if (AddonsFoldout)
            {
                MainEditor.DrawAddons(Target, AddonTarget.SocketBehaviour);
            }

            #endregion Socket Behaviour Add-Ons

            #region Socket Behaviour Debugging

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Socket Behaviour - Debugging", EditorStyles.largeLabel);

            GUI.color = Color.white;

            DebuggingFoldout = EditorGUILayout.Foldout(DebuggingFoldout, "Debugging Settings");

            if (DebuggingFoldout)
            {
                EditorGUILayout.HelpBox("Please note that the debugging state here are set only dunring the runtime!\n" +
                    "You can also pass inspector in debug state to have more information about this component.", MessageType.Info);
                GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("IsDisabled"), new GUIContent("Socket Is Disabled :", "Socket is disabled?, when disable all the raycast will ignore this."));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ParentPiece"), new GUIContent("Socket Parent Piece :", "Parent piece if the socket has a parent with the Piece Behaviour component."));
                GUILayout.BeginHorizontal();
                GUILayout.Space(13);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("BusySpaces"), new GUIContent("Socket Busy Spaces :", "Find here all the busy spaces of the socket, if a piece is here it's that you cannot place another same piece."), true);
                GUILayout.EndHorizontal();
                GUI.enabled = true;
            }

            GUI.color = Color.white;

            #endregion Socket Debugging Settings

            #region Socket Behaviour Utilities

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Socket Behaviour - Utilities", EditorStyles.largeLabel);

            GUI.color = Color.white;

            UtilitiesFoldout = EditorGUILayout.Foldout(UtilitiesFoldout, "Utilities Settings");
           
            GUI.color = MainEditor.GetEditorColor;

            if (UtilitiesFoldout)
            {
                if (GUILayout.Button("Duplicate this socket"))
                {
                    GameObject Duplicate = Instantiate(Target.gameObject, Target.transform.parent);

                    Duplicate.name = Target.transform.name;

                    Selection.activeGameObject = Duplicate;
                }

                if (GUILayout.Button("Duplicate this socket and inverse position"))
                {
                    GameObject Duplicate = Instantiate(Target.gameObject, Target.transform.parent);

                    Duplicate.name = Target.transform.name;

                    Vector3 Inverse = new Vector3(-Target.transform.localPosition.x, Target.transform.localPosition.y, -Target.transform.localPosition.z);

                    Duplicate.transform.localPosition = Inverse;

                    Selection.activeGameObject = Duplicate;
                }

                if (GUILayout.Button("Destroy this socket"))
                {
                    DestroyImmediate(Target.gameObject);
                }
            }

            GUI.color = Color.white;

            #endregion Socket Utilities Settings

            serializedObject.ApplyModifiedProperties();

            if (CurrentOffset != null)
            {
                if (PreviewPiece != null)
                {
                    PreviewPiece.transform.position = Target.transform.TransformPoint(CurrentOffset.Position);
                    PreviewPiece.transform.rotation = Target.transform.rotation * Quaternion.Euler(CurrentOffset.Rotation);

                    if (CurrentOffset.Scale != Vector3.one)
                    {
                        PreviewPiece.transform.localScale = CurrentOffset.Scale;
                    }
                    else
                        PreviewPiece.transform.localScale = Target.transform.parent != null ? Target.transform.parent.localScale : Target.transform.localScale;
                }
            }
        }

        private void CreatePreview(Offset offsetPiece)
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (PreviewPiece == null)
            {
                PieceBehaviour PieceInstance = offsetPiece.Referer == OffsetRefererType.ByPiece ? offsetPiece.Piece : BuildManager.Instance.GetPieceByCategory(offsetPiece.Category);

                if (PieceInstance == null) return;

                PreviewPiece = Instantiate(PieceInstance.gameObject, Target.transform);

                PreviewPiece.transform.position = Target.transform.TransformPoint(offsetPiece.Position);
                PreviewPiece.transform.rotation = Target.transform.rotation * Quaternion.Euler(offsetPiece.Rotation);

                if (offsetPiece.Scale != Vector3.one)
                {
                    PreviewPiece.transform.localScale = offsetPiece.Scale;
                }
                else
                    PreviewPiece.transform.localScale = Target.transform.parent != null ? Target.transform.parent.localScale : Target.transform.localScale;

                PreviewPiece.name = PieceInstance.Id.ToString();

                DestroyImmediate(PreviewPiece.GetComponent<PieceBehaviour>());

                foreach (SocketBehaviour Socket in PreviewPiece.GetComponentsInChildren<SocketBehaviour>())
                {
                    DestroyImmediate(Socket);
                }

                Material PreviewMaterial = new Material(Shader.Find(Constants.TRANSPARENT_SHADER_NAME))
                {
                    color = new Color(0, 1f, 1f, 0.5f)
                };

                PreviewPiece.ChangeAllMaterialsInChildren(PreviewPiece.GetComponentsInChildren<MeshRenderer>(), PreviewMaterial);

                SceneView.FrameLastActiveSceneView();
            }
        }

        private void ClearPreview()
        {
            if (PreviewPiece != null)
            {
                DestroyImmediate(PreviewPiece);
                PreviewPiece = null;
                CurrentOffset = null;
            }
        }

        #endregion Methods
    }
}