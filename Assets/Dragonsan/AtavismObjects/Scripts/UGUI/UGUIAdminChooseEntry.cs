using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Atavism
{

    public class UGUIAdminChooseEntry : MonoBehaviour
    {

        public Text text;
        public TextMeshProUGUI TMPText;
        public Image icon;
        AdminChooseEntryClicked entryClicked;
        AdminChooseEntryClicked entryIconUpdate;
        int id;
        Sprite image;

        public void SetEntryDetails(AdminChooseEntryClicked entryClicked, int id, string name, Sprite image, AdminChooseEntryClicked entryIconUpdate)
        {
            this.entryClicked = entryClicked;
            this.entryIconUpdate = entryIconUpdate;
            this.id = id;
            if (text != null)
                text.text = name;
            if (TMPText != null)
                TMPText.text = name;
            this.image = image;
            if (image != null)
                icon.sprite = image;
            else
                icon.sprite = AtavismSettings.Instance.defaultItemIcon;

          //  icon.sprite = image;
        }
     
        void OnBecameVisible()
        {
            Debug.LogError("AdminEntry " + id);
            entryIconUpdate(id);
        }

        public void EntryClicked()
        {
            entryClicked(id);
        }
    }
}