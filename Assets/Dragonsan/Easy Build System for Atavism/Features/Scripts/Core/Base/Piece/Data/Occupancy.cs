namespace EasyBuildSystem.Features.Scripts.Core.Base.Piece.Data
{
    [System.Serializable]
    public class Occupancy
    {
        #region Fields

        public PieceBehaviour Piece;

        #endregion Fields

        #region Methods

        public Occupancy(PieceBehaviour piece)
        {
            Piece = piece;
        }

        #endregion Methods
    }
}