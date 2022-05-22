using System.Collections.Generic;

namespace EasyBuildSystem.Features.Scripts.Extensions
{
    public static class ListExtension
    {
        #region Enums

        public enum MoveDirection
        {
            Increase,
            Decrease
        }

        #endregion

        #region Methods

        public static void Move<T>(this IList<T> list, int iIndexToMove, MoveDirection direction)
        {
            if (direction == MoveDirection.Increase)
            {
                T old = list[iIndexToMove - 1];
                list[iIndexToMove - 1] = list[iIndexToMove];
                list[iIndexToMove] = old;
            }
            else
            {
                T old = list[iIndexToMove + 1];
                list[iIndexToMove + 1] = list[iIndexToMove];
                list[iIndexToMove] = old;
            }
        }

        #endregion
    }
}