using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using EasyBuildSystem.Features.Scripts.Core.Base.Socket.Enums;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Core.Base.Socket.Data
{
    [System.Serializable]
    public class Offset
    {
        #region Fields

        public OffsetRefererType Referer;

        public PieceBehaviour Piece;

        public string Category;

        public Vector3 Position;

        public Vector3 Rotation;

        public Vector3 Scale = Vector3.one;

        #endregion Fields

        #region Methods

        public Offset(PieceBehaviour piece)
        {
            Piece = piece;
        }

        #endregion Methods
    }
}