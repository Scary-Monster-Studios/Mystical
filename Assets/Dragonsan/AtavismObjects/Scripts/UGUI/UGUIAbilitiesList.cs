using UnityEngine;
using System.Collections;

namespace Atavism
{

    public class UGUIAbilitiesList : AtList<UGUIAbility>
    {

        Skill selectedSkill;

        // Use this for initialization
        void Start()
        {

        }

        public void UpdateAbilities(Skill selectedSkill)
        {
            this.selectedSkill = selectedSkill;
            // Delete the old list
            ClearAllCells();

            Refresh();
        }

        #region implemented abstract members of AtList

        public override int NumberOfCells()
        {
            if (selectedSkill != null)
            {
                return selectedSkill.abilities.Count;
            }
            else
            {
                return 0;
            }
        }

        public override void UpdateCell(int index, UGUIAbility cell)
        {
            AtavismAbility ab = Abilities.Instance.GetAbility(selectedSkill.abilities[index]);
            cell.UpdateAbilityData(ab);
        }

        #endregion
    }
}