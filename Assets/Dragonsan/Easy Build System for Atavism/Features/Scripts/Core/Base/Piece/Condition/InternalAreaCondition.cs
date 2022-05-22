using Atavism;
using EasyBuildSystem.Features.Scripts.Core.Base.Area;
using EasyBuildSystem.Features.Scripts.Core.Base.Condition;
using EasyBuildSystem.Features.Scripts.Core.Base.Condition.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Core.Base.Piece.Condition
{
    [Condition("Internal Area Condition", "Check and denies the actions, if this piece entering in a Area Behaviour component.\n" +
    "You can find more information about of conditions in the online documentation.", ConditionTarget.PieceBehaviour)]
    public class InternalAreaCondition : ConditionBehaviour
    {
        #region Fields

        public bool RequireAreaForPlacement;
        public bool RequireAreaForDestruction;
        public bool RequireAreaForEdition;

        #endregion Fields

        #region Methods

        public override bool CheckForPlacement()
        {
            AreaBehaviour NearestArea = BuildManager.Instance.GetNearestArea(transform.position);
       // Debug.LogError("CheckForPlacement NearestArea="+NearestArea+" RequireAreaForPlacement="+RequireAreaForPlacement+" Piece="+Piece);
            if (NearestArea != null)
            {
                if (NearestArea.AllowAllPlacement)
                {
                  //  Debug.LogError("CheckForPlacement AllowAllPlacement");
                    Bounds currentReticleBounds = new Bounds(Piece.transform.position, Vector3.zero);
                    currentReticleBounds.Encapsulate(Piece.MeshBounds);
                    if (WorldBuilder.Instance.ActiveClaim.IsObjectFullyInsideClaim(Piece.MeshBoundsToWorld))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (NearestArea.CheckAllowedPlacement(Piece))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (RequireAreaForPlacement)
                {
                    return false;
                }
            }

            return true;
        }

        public override bool CheckForDestruction()
        {
            AreaBehaviour NearestArea = BuildManager.Instance.GetNearestArea(transform.position);
          //  Debug.LogError("CheckForDestruction NearestArea="+NearestArea);

            if (NearestArea != null)
            {
                if (NearestArea.AllowAllDestruction)
                {
                    return true;
                }
                else
                {
                    if (NearestArea.CheckAllowedDestruction(Piece))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (RequireAreaForDestruction)
                {
                    return false;
                }
            }

            return true;
        }

        public override bool CheckForEdition()
        {
            AreaBehaviour NearestArea = BuildManager.Instance.GetNearestArea(transform.position);
         //   Debug.LogError("CheckForEdition NearestArea="+NearestArea);

            if (NearestArea != null)
            {
                if (NearestArea.AllowAllEdition)
                {
                    return true;
                }
                else
                {
                    if (NearestArea.CheckAllowedEdition(Piece))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (RequireAreaForEdition)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}