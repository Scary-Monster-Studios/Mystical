using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using EasyBuildSystem.Features.Scripts.Editor.Window;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace EasyBuildSystem.Addons.CircularMenu.Scripts.Editor
{
    [CustomEditor(typeof(CircularMenu))]
    public class CircularMenuInspector : UnityEditor.Editor
    {
        #region Fields

        private CircularMenu Target;

        private string CategoryName;

        private bool GeneralFoldout;
        private bool InputsFoldout;
        private bool CategoriesFoldout; 
        private bool AnimatorFoldout;
        private bool PostProcessFoldout;

        #endregion Fields

        #region Methods

        private void OnEnable()
        {
            Target = (CircularMenu)target;
         }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region Circular Menu General

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Circular Menu - General", EditorStyles.largeLabel);

            GUI.color = Color.white;

            GeneralFoldout = EditorGUILayout.Foldout(GeneralFoldout, "General Settings");

            if (GeneralFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Selection"), new GUIContent("Selection :", ""));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("SelectionIcon"), new GUIContent("Selection Icon :", ""));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SelectionText"), new GUIContent("Selection Name :", ""));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SelectionDescription"), new GUIContent("Selection Description :", ""));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("ButtonNormalColor"), new GUIContent("Selection Icon Normal Color :", ""));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ButtonHoverColor"), new GUIContent("Selection Icon Hover Color :", ""));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ButtonPressedColor"), new GUIContent("Selection Icon Pressed Color :", ""));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("PreviousIcon"), new GUIContent("Previous Icon :", ""));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("NextIcon"), new GUIContent("Next Icon :", ""));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ReturnIcon"), new GUIContent("Return Icon :", ""));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("CircularButton"), new GUIContent("Button Prefab :", ""));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ButtonSpacing"), new GUIContent("Button Spacing :", ""));
            }

            #endregion Circular Menu General

            #region Circular Menu Input

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Circular Menu - Inputs", EditorStyles.largeLabel);

            GUI.color = Color.white;

            InputsFoldout = EditorGUILayout.Foldout(InputsFoldout, "Inputs Settings");

            if (InputsFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OpenCircularKey"), new GUIContent("Input Active Key :", ""));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Controller"), new GUIContent("Input Controller Type :", ""));

                if (Target.Controller == CircularMenu.ControllerType.Gamepad)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("GamepadInputOpenName"), new GUIContent("Gamepad Input Open :", ""));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("GamepadInputAxisX"), new GUIContent("Gamepad Input Axis X :", ""));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("GamepadInputAxisY"), new GUIContent("Gamepad Input Axis Y :", ""));
                }
            }

            #endregion Circular Menu Input

            #region Circular Menu Categories

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Circular Menu - Categories", EditorStyles.largeLabel);

            GUI.color = Color.white;

            CategoriesFoldout = EditorGUILayout.Foldout(CategoriesFoldout, "Categories Settings");

            if (CategoriesFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("DefaultCategoryIndex"), new GUIContent("Default Category Index :", ""));

                GUILayout.BeginVertical("box");

                for (int i = 0; i < Target.Categories.Count; i++)
                {
                    GUILayout.BeginVertical("box");

                    Target.Categories[i].Name = EditorGUILayout.TextField("Category Name :", Target.Categories[i].Name);
                    Target.Categories[i].Content = (GameObject)EditorGUILayout.ObjectField("Category Content :", Target.Categories[i].Content, typeof(GameObject), true);

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(13);
                    SerializedProperty ButtonsProperty = serializedObject.FindProperty("Categories.Array.data[" + i + "].Buttons");

                    EditorGUILayout.PropertyField(ButtonsProperty, true);
                    GUILayout.EndHorizontal();

                    GUI.color = MainEditor.GetEditorColor;

                    if (GUILayout.Button("Pieces Collection To Buttons"))
                    {
                        if (BuildManager.Instance != null)
                        {
                            if (BuildManager.Instance.Pieces != null)
                            {
                                for (int x = 0; x < BuildManager.Instance.Pieces.Count; x++)
                                {
                                    Target.Categories[i].Buttons.Add(new CircularButtonData() {
                                        Icon = BuildManager.Instance.Pieces[x].Icon, Order = x, Text = BuildManager.Instance.Pieces[x].Name, 
                                        Description = BuildManager.Instance.Pieces[x].Description, Action = new UnityEvent()
                                    });
                                }

                                EditorUtility.SetDirty(target);
                            }
                        }
                    }

                    GUI.color = new Color(1f, 0f, 0f);

                    if (GUILayout.Button("Remove Category"))
                    {
                        if (Target.transform.Find(Target.Categories[i].Name) != null)
                            DestroyImmediate(Target.transform.Find(Target.Categories[i].Name).gameObject);

                        Target.Categories.RemoveAt(i);
                    }
                    GUI.color = Color.white;

                    GUILayout.EndVertical();
                }

                GUILayout.EndVertical();

                GUILayout.BeginVertical("box");

                CategoryName = EditorGUILayout.TextField("Category Name :", CategoryName);

                GUI.color = MainEditor.GetEditorColor;

                if (GUILayout.Button("Add New Category"))
                {
                    GameObject NewContent = new GameObject(CategoryName);
                    NewContent.transform.SetParent(Target.transform, false);
                    NewContent.transform.localPosition = Vector3.zero;
                    Target.Categories.Add(new CircularMenu.UICustomCategory() { Name = CategoryName, Content = NewContent });
                    CategoryName = string.Empty;
                }

                GUI.color = Color.white;

                GUILayout.EndVertical();
            }

            #endregion Circular Menu Categories

            #region Circular Menu Animator

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Circular Menu - Animator", EditorStyles.largeLabel);

            GUI.color = Color.white;

            AnimatorFoldout = EditorGUILayout.Foldout(AnimatorFoldout, "Animator Settings");

            if (AnimatorFoldout)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Animator"), new GUIContent("Circular Animator :", ""));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowStateName"), new GUIContent("Circular Show State Name :", ""));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("HideStateName"), new GUIContent("Circular Hide State Name :", ""));
            }

            #endregion Circular Menu Animator
    
            serializedObject.ApplyModifiedProperties();
        }

        #endregion Methods
    }
}