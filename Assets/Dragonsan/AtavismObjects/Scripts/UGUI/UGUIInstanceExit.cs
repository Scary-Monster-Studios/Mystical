using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Atavism
{

    public class UGUIInstanceExit : MonoBehaviour
    {

        public void LeaveInstance()
        {
            if (AtavismSettings.Instance.ArenaInstances.Contains(SceneManager.GetActiveScene().name))
            {
#if AT_I2LOC_PRESET
            UGUIConfirmationPanel.Instance.ShowConfirmationBox(I2.Loc.LocalizationManager.GetTranslation("Are you sure you want exit arena") + " " + "?", null, SendLeaveArena);
#else
                UGUIConfirmationPanel.Instance.ShowConfirmationBox("Are you sure you want exit arena ?", null, SendLeaveArena);
#endif
            }

            if (AtavismSettings.Instance.DungeonInstances.Contains(SceneManager.GetActiveScene().name))
            {
#if AT_I2LOC_PRESET
            UGUIConfirmationPanel.Instance.ShowConfirmationBox(I2.Loc.LocalizationManager.GetTranslation("Are you sure you want exit instance") + " " + "?", null, SendLeaveInstance);
#else
                UGUIConfirmationPanel.Instance.ShowConfirmationBox("Are you sure you want exit instance ?", null, SendLeaveInstance);
#endif
            }
        }

        void SendLeaveArena(object item, bool accepted)
        {
            if (accepted)
            {
                //    Debug.LogError("Leave Arena");
                Dictionary<string, object> props = new Dictionary<string, object>();
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "arena.leaveArena", props);
            }
        }

        void SendLeaveInstance(object item, bool accepted)
        {
            if (accepted)
            {
                //    Debug.LogError("Leave Instance");
                Dictionary<string, object> props = new Dictionary<string, object>();
                NetworkAPI.SendExtensionMessage(ClientAPI.GetPlayerOid(), false, "ao.leaveInstance", props);
            }
        }

    }
}