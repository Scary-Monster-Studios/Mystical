using EasyBuildSystem.Features.Scripts.Core.Base.Storage.Data;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Core.Scriptables.Blueprint
{
    public class BlueprintTemplate : ScriptableObject
    {
        #region Fields

        public PieceData Model = new PieceData();
        public string Data;

        #endregion Fields
    }
}