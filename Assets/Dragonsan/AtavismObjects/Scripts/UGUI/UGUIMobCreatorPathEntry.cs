using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Atavism
{
    public class UGUIMobCreatorPathEntry : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI Name;
        PatrolPoint pp;
        UGUIMobCreator mobCreator;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void SetEntryDetails(string name, PatrolPoint pp, UGUIMobCreator mobCreator)
        {
            this.Name.text = name;
            this.mobCreator = mobCreator;
            this.pp = pp;

        }
        public void EntryClicked()
        {
            mobCreator.PatrolPathDeletePointClicked(pp);
        }
    }
}