using EasyBuildSystem.Features.Scripts.Core.Addons.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Area;
using EasyBuildSystem.Features.Scripts.Core.Base.Area.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Condition;
using EasyBuildSystem.Features.Scripts.Core.Base.Condition.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Condition.Helper;
using EasyBuildSystem.Features.Scripts.Editor.Window;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Editor.Inspector.Area
{
    [CustomEditor(typeof(AreaBehaviour))]
    public class AreaBehaviourInspector : UnityEditor.Editor
    {
        #region Fields

        public static Rect OffsetEditingWindow = new Rect(50, 50, 300, 280);

        private AreaBehaviour Target;

        private List<ConditionAttribute> Conditions = new List<ConditionAttribute>();

        private static bool GeneralFoldout;
        private static bool ConditionsFoldout;
        private static bool AddonsFoldout;

        #endregion Fields

        #region Methods

        private void OnEnable()
        {
            Target = (AreaBehaviour)target;

            Conditions = ConditionHelper.GetConditionsByTarget(ConditionTarget.AreaBehaviour);

            foreach (ConditionBehaviour Condition in Target.GetComponentsInChildren<ConditionBehaviour>())
            {
                if (Condition != null)
                {
                    Condition.hideFlags = HideFlags.HideInInspector;
                }
            }

            MainEditor.LoadAddons(Target, AddonTarget.AreaBehaviour);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region Area Behaviour General

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Area Behaviour - General", EditorStyles.largeLabel);

            GUI.color = Color.white;

            GeneralFoldout = EditorGUILayout.Foldout(GeneralFoldout, "General Settings");

            if (GeneralFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Shape"), new GUIContent("Area Shape Type :", "Define the shape of area."));

                if (Target.Shape == AreaShape.Bounds)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Bounds"), new GUIContent("Area Shape Size :", "Define the bounds of area."));
                }
                else
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Radius"), new GUIContent("Area Shape Radius :", "Define the radius of area."));
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("AllowAllPlacement"), new GUIContent("Area Allow All Placement :", "Allow all the placement."));

                if (!Target.AllowAllPlacement)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("AllowPlacementSpecificPieces"), new GUIContent("Area Allow Specific Piece(s) In Placement :", "Allow specific piece(s) to allow the placement."), true);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("AllowAllDestruction"), new GUIContent("Area Allow All Destruction :", "Allow all the destruction."));

                if (!Target.AllowAllDestruction)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("AllowDestructionSpecificPieces"), new GUIContent("Area Allow Specific Piece(s) In Destruction :", "Allow specific piece(s) to allow the destruction."), true);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("AllowAllEdition"), new GUIContent("Area Allow All Edition :", "Allow all the edition."));

                if (!Target.AllowAllEdition)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("AllowEditionSpecificPieces"), new GUIContent("Area Allow Specific Piece(s) In Edition :", "Allow specific piece(s) to allow the edition."), true);
            }

            #endregion Area Behaviour General

            #region Area Behaviour Conditions

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Area Behaviour - Conditions", EditorStyles.largeLabel);

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
                    GUILayout.Space(12);
                    GUILayout.BeginVertical();
                    if (Target.gameObject.GetComponent(Condition.Behaviour) != null)
                    {
                        UnityEditor.Editor ConditionEditor = CreateEditor(Target.gameObject.GetComponent(Condition.Behaviour));
                        ConditionEditor.DrawDefaultInspector();
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                }

                EditorGUILayout.EndVertical();
            }

            #endregion

            #region Area Behaviour Addons

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Area Behaviour - Addons", EditorStyles.largeLabel);

            GUI.color = Color.white;

            AddonsFoldout = EditorGUILayout.Foldout(AddonsFoldout, "Addons Settings");

            if (AddonsFoldout)
            {
                MainEditor.DrawAddons(Target, AddonTarget.AreaBehaviour);
            }

            #endregion Area Behaviour Addons

            serializedObject.ApplyModifiedProperties();
        }

        #endregion Methods
    }
}