using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using System.Collections.Generic;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Core.Scriptables.Collection
{
    public class PieceCollection : ScriptableObject
    {
        #region Fields

        public List<PieceBehaviour> Pieces = new List<PieceBehaviour>();

        #endregion Fields
    }
}