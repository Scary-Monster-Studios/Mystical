using EasyBuildSystem.Features.Scripts.Core.Base.Condition;
using EasyBuildSystem.Features.Scripts.Core.Base.Condition.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using EasyBuildSystem.Features.Scripts.Extensions;
using System.Linq;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Core.Base.Piece.Condition
{
    [Condition("Internal Collision Condition", "Check and denies the actions, if the collider of this piece entering in collision with others collider.\n" +
    "You can find more information about of conditions in the online documentation.", ConditionTarget.PieceBehaviour)]
    public class InternalColliderCondition : ConditionBehaviour
    {
        #region Fields

        public LayerMask CollisionLayers;
        [Range(0, 0.99f)]
        public float CollisionClippingTolerance = .5f;
        public bool RequireSupport;
        public bool IgnoreWhenSnap;

        #endregion Fields

        #region Methods

        public override bool CheckForPlacement()
        {
            bool AllowPlacement = false;

            Collider[] Colliders = PhysicExtension.GetNeighborsTypeByBox<Collider>(Piece.MeshBoundsToWorld.center,
                   Piece.MeshBoundsToWorld.extents, transform.rotation, BuildManager.Instance.FreeLayers).Where(x => !x.isTrigger).ToArray();

            for (int i = 0; i < Colliders.Length; i++)
            {
                if (Colliders[i] != null)
                {
                    if (RequireSupport && BuildManager.Instance.IsSupport(Colliders[i]))
                    {
                        AllowPlacement = true;
                    }
                }
            }

            Colliders = PhysicExtension.GetNeighborsTypeByBox<Collider>(Piece.MeshBoundsToWorld.center,
                Piece.MeshBoundsToWorld.extents * CollisionClippingTolerance, transform.rotation, BuildManager.Instance.FreeLayers).Where(x => !x.isTrigger).ToArray();
          //  Debug.LogError("InternalColliderCondition: AllowPlacement="+AllowPlacement+" RequireSupport="+RequireSupport+" Colliders="+Colliders);

            for (int i = 0; i < Colliders.Length; i++)
            {
                if (Colliders[i] != null)
                {
                    if (RequireSupport && !IgnoreWhenSnap && !BuildManager.Instance.IsSupport(Colliders[i]))
                    {
                        return false;
                    }

                    if (!RequireSupport && !BuildManager.Instance.IsSupport(Colliders[i]))
                    {
                        return false;
                    }
                }
            }
//Debug.LogError("InternalColliderCondition: AllowPlacement="+AllowPlacement+" RequireSupport="+RequireSupport);
            return AllowPlacement || !RequireSupport;
        }

        #endregion
    }
}