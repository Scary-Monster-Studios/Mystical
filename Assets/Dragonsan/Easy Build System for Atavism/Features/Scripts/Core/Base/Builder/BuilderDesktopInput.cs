using EasyBuildSystem.Features.Scripts.Core.Base.Builder.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EasyBuildSystem.Features.Scripts.Core.Base.Builder
{
    [AddComponentMenu("Easy Build System/Features/Builders Behaviour/Inputs/Builder Desktop Input")]
    public class BuilderDesktopInput : MonoBehaviour
    {
        #region Fields

        public bool UseShortcuts = true;
        public bool UIBlocking = false;

        public KeyCode BuilderPlacementModeKey = KeyCode.E;
        public KeyCode BuilderDestructionModeKey = KeyCode.R;
        public KeyCode BuilderEditionModeKey = KeyCode.T;

        public KeyCode BuilderValidateModeKey = KeyCode.Mouse0;
        public KeyCode BuilderCancelModeKey = KeyCode.Mouse1;

        public int SelectedIndex { get; set; }

        #endregion

        #region Methods

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            if (UseShortcuts)
            {
         
                if (Input.GetKeyDown(BuilderCancelModeKey))
                {
                    BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
                }
            }

            if (BuilderBehaviour.Instance.CurrentMode == BuildMode.Placement)
            {
             /*   if (IsPointerOverUIElement())
                {
                    return;
                }

                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }*/

                if (Input.GetKeyDown(BuilderValidateModeKey))
                {
                    BuilderBehaviour.Instance.PlacePrefab();
                }

                float WheelAxis = Input.GetAxis("Mouse ScrollWheel");

                if (WheelAxis > 0)
                {
                    BuilderBehaviour.Instance.RotatePreview(BuilderBehaviour.Instance.SelectedPrefab.RotationAxis);
                }
                else if (WheelAxis < 0)
                {
                    BuilderBehaviour.Instance.RotatePreview(-BuilderBehaviour.Instance.SelectedPrefab.RotationAxis);
                }

                if (Input.GetKeyDown(BuilderCancelModeKey))
                {
                    BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
                }
            }
            else if (BuilderBehaviour.Instance.CurrentMode == BuildMode.Edition)
            {
             /*   if (IsPointerOverUIElement())
                {
                    return;
                }*/


                if (Input.GetKeyDown(BuilderValidateModeKey))
                {
                   // ??
                   // BuilderBehaviour.Instance.EditPrefab();
                }

                if (Input.GetKeyDown(BuilderCancelModeKey))
                {
                    BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
                }
                float WheelAxis = Input.GetAxis("Mouse ScrollWheel");

                if (WheelAxis > 0)
                {
                    if(BuilderBehaviour.Instance.SelectedPrefab!=null)
                        BuilderBehaviour.Instance.RotatePreview(BuilderBehaviour.Instance.SelectedPrefab.RotationAxis);
                }
                else if (WheelAxis < 0)
                {
                    if(BuilderBehaviour.Instance.SelectedPrefab!=null)
                        BuilderBehaviour.Instance.RotatePreview(-BuilderBehaviour.Instance.SelectedPrefab.RotationAxis);
                }
            }
            else if (BuilderBehaviour.Instance.CurrentMode == BuildMode.Destruction)
            {
               /* if (IsPointerOverUIElement())
                {
                    return;
                }
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }*/
                if (Input.GetKeyDown(BuilderValidateModeKey))
                {
                    if (BuilderBehaviour.Instance.CurrentRemovePreview != null)
                    {
                        BuilderBehaviour.Instance.DestroyPrefab();
                    }
                }

                if (Input.GetKeyDown(BuilderCancelModeKey))
                {
                    BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
                }
            }
        }

        private void UpdatePrefabSelection()
        {
            float WheelAxis = Input.GetAxis("Mouse ScrollWheel");

            if (WheelAxis > 0)
            {
                if (SelectedIndex < BuildManager.Instance.Pieces.Count - 1)
                {
                    SelectedIndex++;
                }
                else
                {
                    SelectedIndex = 0;
                }
            }
            else if (WheelAxis < 0)
            {
                if (SelectedIndex > 0)
                {
                    SelectedIndex--;
                }
                else
                {
                    SelectedIndex = BuildManager.Instance.Pieces.Count - 1;
                }
            }

            if (SelectedIndex == -1)
            {
                return;
            }

            if (BuildManager.Instance.Pieces.Count != 0)
            {
                BuilderBehaviour.Instance.SelectPrefab(BuildManager.Instance.Pieces[SelectedIndex]);
            }
        }

        /// <summary>
        /// Check if the cursor is above a UI element or if the ciruclar menu is open.
        /// </summary>
        private bool IsPointerOverUIElement()
        {
            if (!UIBlocking)
            {
                return false;
            }

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                return false;
            }

            if (EventSystem.current == null)
            {
                return false;
            }

            PointerEventData EventData = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };

            List<RaycastResult> Results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(EventData, Results);
            return Results.Count > 0;
        }

        #endregion
    }
}