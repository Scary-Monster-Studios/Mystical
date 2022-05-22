using EasyBuildSystem.Features.Scripts.Core.Base.Area.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using System.Collections.Generic;
using Atavism;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Core.Base.Area
{
    [AddComponentMenu("Easy Build System/Features/Buildings Behaviour/Area Behaviour")]
    public class AreaBehaviour : MonoBehaviour
    {
        #region Fields

        public AreaShape Shape;
        public float Radius = 5f;
        public Bounds Bounds;

        public bool AllowAllPlacement = true;
        public List<PieceBehaviour> AllowPlacementSpecificPieces = new List<PieceBehaviour>();
        public bool AllowAllDestruction = true;
        public List<PieceBehaviour> AllowDestructionSpecificPieces = new List<PieceBehaviour>();
        public bool AllowAllEdition = true;
        public List<PieceBehaviour> AllowEditionSpecificPieces = new List<PieceBehaviour>();

        #endregion Fields

        #region Methods

        private void Start()
        {
            BuildManager.Instance.AddArea(this);
        }

        private void OnDestroy()
        {
            BuildManager.Instance.RemoveArea(this);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;

            if (Shape == AreaShape.Bounds)
            {
                Gizmos.DrawWireCube(transform.TransformPoint(Bounds.center), Bounds.extents);
            }
            else if (Shape == AreaShape.Sphere)
            {
                Gizmos.DrawWireSphere(transform.position, Radius);
            }
        }


        /// <summary>
        /// This method allows to check if the piece exists in the AllowPlacementSpecificPieces list.
        /// </summary>
        public bool CheckAllowedPlacement(PieceBehaviour piece)
        {
            Debug.LogWarning("CheckAllowedPlacement AllowPlacementSpecificPieces=" + AllowPlacementSpecificPieces.Count);

            if (AllowPlacementSpecificPieces.Count == 0) return false;

            bool allow = AllowPlacementSpecificPieces.Find(entry => entry.Id == piece.Id);
            Debug.LogWarning("CheckAllowedPlacement allow=" + allow);
            if (allow)
            {
                Bounds currentReticleBounds = new Bounds(piece.transform.position, Vector3.zero);
                currentReticleBounds.Encapsulate(piece.MeshBounds);
                Debug.LogWarning("CheckAllowedPlacement currentReticleBounds=" + currentReticleBounds + " " + WorldBuilder.Instance.ActiveClaim.bounds);
                if (!WorldBuilder.Instance.ActiveClaim.IsObjectFullyInsideClaim(currentReticleBounds))
                {
                    Debug.LogError("Placement failed due to object not fully inside claim");
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method allows to check if the piece exists in the AllowDestructionSpecificPieces list.
        /// </summary>
        public bool CheckAllowedDestruction(PieceBehaviour piece)
        {
            if (AllowDestructionSpecificPieces.Count == 0) return false;

            return AllowDestructionSpecificPieces.Find(entry => entry.Id == piece.Id);
        }

        /// <summary>
        /// This method allows to check if the piece exists in the AllowEditionSpecificPieces list.
        /// </summary>
        public bool CheckAllowedEdition(PieceBehaviour piece)
        {
            if (AllowEditionSpecificPieces.Count == 0) return false;

            return AllowEditionSpecificPieces.Find(entry => entry.Id == piece.Id);
        }

        #endregion Methods
    }
}