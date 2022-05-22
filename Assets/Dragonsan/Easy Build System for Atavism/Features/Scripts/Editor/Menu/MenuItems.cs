using EasyBuildSystem.Features.Scripts.Core.Scriptables.Blueprint;
using EasyBuildSystem.Features.Scripts.Core.Scriptables.Collection;
using EasyBuildSystem.Features.Scripts.Editor.Window;
using EasyBuildSystem.Features.Scripts.Extensions;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Editor.Menu
{
    public class MenuItems : ScriptableObject
    {
        #region Methods

        [MenuItem(@"Tools/Easy Build System/Editor %#e", priority = -800)]
        public static void EditorWindow()
        {
            MainEditor.Init();
        }

        [MenuItem(@"Tools/Easy Build System/Check Missing Layer(s)...", priority = -700)]
        public static void EditorCheckLayer()
        {
            MainEditor.CheckMissingLayers();
        }
        
        [MenuItem(@"Tools/Easy Build System/Components/Scriptable Object(s)/Create New Piece Collection...", priority = -500)]
        public static void EditorCreatePieceCollection()
        {
            ScriptableObjectExtension.CreateAsset<PieceCollection>("New Piece Collection...");
        }

        [MenuItem(@"Tools/Easy Build System/Components/Scriptable Object(s)/Create New Blueprint Template...", priority = -500)]
        public static void EditorCreateBlueprintData()
        {
            ScriptableObjectExtension.CreateAsset<BlueprintTemplate>("New Blueprint Template...");
        }

        [MenuItem(@"Tools/Easy Build System/Components/Code Template(s)/Create New Addon Template...", isValidateFunction: false, priority = -500)]
        public static void EditorCreateAddonScript()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Assets/Easy Build System/Features/Scripts/Core/Templates/AddonTemplate.txt", "NewAddonTemplate.cs");
        }

        [MenuItem(@"Tools/Easy Build System/Components/Code Template(s)/Create New Condition Template...", isValidateFunction: false, priority = -500)]
        public static void EditorCreateConditionScript()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Assets/Easy Build System/Features/Scripts/Core/Templates/ConditionTemplate.txt", "NewConditionTemplate.cs");
        }

        [MenuItem(@"Tools/Easy Build System/Components/Create New Area Behaviour...", priority = -200)]
        public static void EditorCreateArea()
        {
            MainEditor.CreateNewArea();
        }

        [MenuItem(@"Tools/Easy Build System/Components/Create New Piece Behaviour...", priority = -200)]
        public static void EditorCreatePiece()
        {
            MainEditor.CreateNewPiece();
        }

        [MenuItem(@"Tools/Easy Build System/Components/Create New Socket Behaviour...", priority = -200)]
        public static void EditorCreateSocket()
        {
            MainEditor.CreateNewSocket();
        }

        private static readonly MethodInfo CreateScriptMethod = typeof(ProjectWindowUtil).GetMethod("CreateScriptAsset", BindingFlags.Static | BindingFlags.NonPublic);
        public static void CreateScriptAsset(string templatePath, string destName)
        {
            CreateScriptMethod.Invoke(null, new object[] { templatePath, destName });
        }

        [MenuItem(@"Tools/Easy Build System/About Us...")]
        public static void EditorLinkWeb()
        {
            Application.OpenURL("https://connect.unity.com/u/ads-cryptoz");
        }

        #endregion Methods
    }
}