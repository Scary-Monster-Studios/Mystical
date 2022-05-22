using EasyBuildSystem.Features.Scripts.Core.Addons.Enums;
using EasyBuildSystem.Features.Scripts.Core.Addons.Helper;
using EasyBuildSystem.Features.Scripts.Core.Base.Group;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using EasyBuildSystem.Features.Scripts.Editor.Window;
using EasyBuildSystem.Features.Scripts.Extensions;
using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Editor.Inspector.Group
{
    [CustomEditor(typeof(GroupBehaviour))]
    public class GroupBehaviourInspector : UnityEditor.Editor
    {
        #region Fields

        public static Rect OffsetEditingWindow = new Rect(50, 50, 300, 280);

        private GroupBehaviour Target;

        private static bool GeneralFoldout;
        private static bool AddonsFoldout;

        #endregion Fields

        #region Methods

        private void OnEnable()
        {
            Target = (GroupBehaviour)target;

            MainEditor.Addons = AddonHelper.GetAddonsByTarget(AddonTarget.GroupBehaviour);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Group Behaviour Settings", EditorStyles.largeLabel);

            #region Group Behaviour Settings

            GUI.color = Color.white;

            GeneralFoldout = EditorGUILayout.Foldout(GeneralFoldout, "General Settings");

            if (GeneralFoldout)
            {
                GUI.color = MainEditor.GetEditorColor;

                if (GUILayout.Button("Save As Blueprint Template"))
                {
                    if (Target.transform.GetComponentsInChildren<PieceBehaviour>().Length > 0)
                    {
                        Core.Scriptables.Blueprint.BlueprintTemplate Data = ScriptableObjectExtension.CreateAsset<Core.Scriptables.Blueprint.BlueprintTemplate>("New Empty Blueprint");
                        Data.name = Target.name;
                        Data.Model = Target.GetModel();
                        Data.Data = Target.GetModel().ToJson();
                    }
                }
            }

            GUI.color = Color.white;

            #endregion Group Behaviour Settings

            GUI.color = MainEditor.GetEditorColor;

            GUILayout.Label("Group Add-On(s) Settings", EditorStyles.largeLabel);

            #region Group Add-Ons Settings

            GUI.color = Color.white;

            AddonsFoldout = EditorGUILayout.Foldout(AddonsFoldout, "Addons Settings");

            if (AddonsFoldout)
            {
                MainEditor.DrawAddons(Target, AddonTarget.GroupBehaviour);
            }

            #endregion Group Add-Ons Settings

            serializedObject.ApplyModifiedProperties();
        }

        #endregion Methods
    }
}