using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{

    public class UGUIBuildObjectsList : AtList<UGUIBuildObject>
    {
        private int category = -2;

        void Start()
        {
            // Delete the old list
            ClearAllCells();

            Refresh();
        }

        public void changeCategory(int category)
        {
            this.category = category;
            ClearAllCells();
            Refresh();
        }

        #region implemented abstract members of AtList

        public override int NumberOfCells()
        {
            int numCells = WorldBuilder.Instance.GetBuildObjectsOfCategory(category, true).Count;
            return numCells;
        }

        public override void UpdateCell(int index, UGUIBuildObject cell)
        {
            List<AtavismBuildObjectTemplate> templates = WorldBuilder.Instance.GetBuildObjectsOfCategory(category, true);
            cell.UpdateBuildObjectInfo(templates[index]);
        }

        #endregion
    }
}