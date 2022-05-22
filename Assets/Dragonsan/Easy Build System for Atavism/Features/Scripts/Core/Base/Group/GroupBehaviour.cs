using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using EasyBuildSystem.Features.Scripts.Core.Base.Storage.Data;
using System.Collections.Generic;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Core.Base.Group
{
    public class GroupBehaviour : MonoBehaviour
    {
        #region Methods

        private void Update()
        {
            if (transform.childCount == 0)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// This method allows to get a model which contains all the base piece data.
        /// </summary>
        public PieceData GetModel()
        {
            PieceData Model = new PieceData();

            PieceBehaviour[] Pieces = GetComponentsInChildren<PieceBehaviour>();

            Model.Pieces = new List<PieceData.SerializedPiece>();

            for (int i = 0; i < Pieces.Length; i++)
            {
                Model.Pieces.Add(new PieceData.SerializedPiece()
                {
                    Id = Pieces[i].Id,
                    Name = Pieces[i].Name,
                    AppearanceIndex = Pieces[i].AppearanceIndex,
                    Position = PieceData.ParseToSerializedVector3(Pieces[i].transform.position),
                    Rotation = PieceData.ParseToSerializedVector3(Pieces[i].transform.eulerAngles),
                    Scale = PieceData.ParseToSerializedVector3(Pieces[i].transform.localScale),
                    Properties = Pieces[i].ExtraProperties
                });
            }

            return Model;
        }

        #endregion Methods
    }
}