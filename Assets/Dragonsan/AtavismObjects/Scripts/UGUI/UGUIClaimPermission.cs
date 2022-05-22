using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUIClaimPermission : MonoBehaviour
    {

        public Text permissionText;
        public TextMeshProUGUI TMPPermissionText;
        ClaimPermission permission;

        // Use this for initialization
        void Start()
        {

        }

        public void SetPermissionDetails(ClaimPermission permission)
        {
            string[] levels = new string[] { "Interaction", "Add Objects", "Edit Objects", "Add Users", "Manage Users" };
            this.permission = permission;
            if (permissionText != null)
                this.permissionText.text = permission.playerName + " (" + levels[permission.permissionLevel - 1] + ")";
            if (TMPPermissionText != null)
                this.TMPPermissionText.text = permission.playerName + " (" + levels[permission.permissionLevel - 1] + ")";
        }

        public void RemoveClaimPermission()
        {
            WorldBuilder.Instance.RemovePermission(permission.playerName);
        }

    }
}