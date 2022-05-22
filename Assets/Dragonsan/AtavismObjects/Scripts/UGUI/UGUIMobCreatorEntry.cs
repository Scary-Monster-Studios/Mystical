using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace Atavism
{
    public class UGUIMobCreatorEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] TextMeshProUGUI Name;
        int id = -1;
        UGUIMobCreator mobCreator;
        bool mobtemplate = false;
        bool startQuest = false;
        bool endQuest = false;
        bool merchant = false;
        bool patrolPath = false;
        bool dialog = false;
        int pos = -1;
        public void OnPointerEnter(PointerEventData eventData)
        {

        }

        public void OnPointerExit(PointerEventData eventData)
        {

        }

        public void SetEntryDetails(string name, int id, bool mob, bool startQuest, bool endQuest, bool merchant, bool dialog, bool patrolPath, int pos, UGUIMobCreator mobCreator)
        {
            this.Name.text = id + ". " + name;
            this.id = id;
            this.mobCreator = mobCreator;
            this.mobtemplate = mob;
            this.startQuest = startQuest;
            this.endQuest = endQuest;
            this.pos = pos;
            this.merchant = merchant;
            this.dialog = dialog;
            this.patrolPath = patrolPath;
            //     UpdateDisplay();

        }
        public void EntryClicked()
        {
            if (mobtemplate)
                mobCreator.SelectTemplate(id);
            if (startQuest)
                mobCreator.StartQuestClicked(id);
            if (endQuest)
                mobCreator.EndQuestClicked(id);
            if (merchant)
                mobCreator.MerchandTableClicked(id);
            if (dialog)
                mobCreator.DialoguesClicked(id);
            if (patrolPath)
                mobCreator.PatrolPathClicked(id);
        }

    }
}