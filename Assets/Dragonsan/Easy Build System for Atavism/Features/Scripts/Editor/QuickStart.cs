using EasyBuildSystem.Features.Scripts.Core.Base.Builder;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using EasyBuildSystem.Features.Scripts.Core.Scriptables.Collection;
using EasyBuildSystem.Features.Scripts.Editor.Window;
using System;
using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Editor
{
    public class QuickStart : MonoBehaviour
    {
        #region Fields

        public const string MANAGER_PREFAB_NAME = "Easy Build System - Build Manager";

        public static bool IsForIntegration;

        #endregion Fields

        #region Methods

        [MenuItem("Tools/Easy Build System/Quick Start...", priority = -700)]
        public static void Init()
        {
            try
            {
                if (!IsForIntegration)
                {
                    if (!EditorUtility.DisplayDialog("Easy Build System - Quick Start",
                        "The Quick Start function allows you to use the default placement system with the default settings.\n\n" +
                        "You will find more information about this function in the documentation.\n\n" +
                        "Check if your current scene has camera with the tag MainCamera.\n\n" +
                        "Do you want run the Quick Start?", "Yes", "No"))
                    {
                        return;
                    }
                }

                if (Camera.main != null)
                {
                    if (FindObjectOfType<BuilderBehaviour>() == null)
                    {
                        Camera.main.gameObject.AddComponent<BuilderBehaviour>();
                    }
                    else
                    {
                        Debug.LogError("<b>Easy Build System</b> : The component <b>BuilderBehaviour</b> already exists.");
                        return;
                    }
                }

                MainEditor.CheckMissingLayers();

                GameObject ResourceManager = (GameObject)Resources.Load(QuickStart.MANAGER_PREFAB_NAME);

                if (ResourceManager == null)
                {
                    Debug.LogError("<b>[Easy Build System]</b> : Cannot load the Build Manager from the resources folder!");
                    return;
                }

                BuildManager Manager = Instantiate(ResourceManager).GetComponent<BuildManager>();

                Manager.name = QuickStart.MANAGER_PREFAB_NAME;

                PieceCollection Collection = Resources.Load<PieceCollection>("Default Modular Pieces Collection");

                if (Collection == null)
                {
                    Debug.Log("<b>[Easy Build System]</b> : The default collection <b>Default Modular Pieces Collection</b> has been not found in resources folder!");
                    return;
                }

                Manager.Pieces.Clear();
                Manager.Pieces.AddRange(Collection.Pieces);

                Debug.Log("<b>Easy Build System</b> : <b>Default Modular Pieces Collection</b> added to Build Manager.");

                if (!IsForIntegration)
                {
                    Debug.Log("<b>Easy Build System</b> : You can now use the system with the default settings!");
                }

                IsForIntegration = false;
            }
            catch (Exception ex)
            {
                Debug.LogError("<b>Easy Build System</b> : " + ex.Message);
            }
        }

        #endregion Methods
    }
}