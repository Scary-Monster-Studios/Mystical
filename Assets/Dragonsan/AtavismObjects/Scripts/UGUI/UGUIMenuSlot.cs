using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Atavism
{
    public delegate void MenuResponse(string category, string value, string nested);

    public class UGUIMenuSlot : MonoBehaviour
    {
        public TextMeshProUGUI Name;
        public int leftMarginPer = 15;
        // string id = "";
        public string value = "";
        public string category = "";
        string baseName = "";
        public string nested = "";
        public int id = 0;
        bool hasChildren = false;
        MenuResponse response;
        public UGUIMenuSlot slotparent;
        public Color selectedColor = Color.green;
        public Color defaultColor = Color.white;

        // Use this for initialization
        void Start()
        {

        }
        public void Setup(string name, int nest, string category, string value, MenuResponse func, string nested, int id, bool hasChildren, UGUIMenuSlot slotparent)
        {

            //  if (id == 0)
            //       Debug.LogError("UGUIMenuSlot Setup :" +  " hasChildren:" + hasChildren);
            this.id = id;
            this.slotparent = slotparent;
            this.nested = nested;
            this.value = value;
            this.category = category;
            this.baseName = name;
            this.response = func;
            this.hasChildren = hasChildren;
            if (Name != null)
            {
                string mark = "";
                if (hasChildren)
                    mark = "<sprite=0>";
#if AT_I2LOC_PRESET
                 Name.text = mark+I2.Loc.LocalizationManager.GetTranslation(name);
#else
                Name.text = mark + name;
#endif
                Name.margin = new Vector4(leftMarginPer * nest, 0, 0, 0);
            }

        }
        public void SetupMark(bool show)
        {
            //   if (id == 0) Debug.LogError("UGUIMenuSlot show:" + show+ " hasChildren:"+ hasChildren);
            if (hasChildren)
                if (Name != null)
                {
                    string mark = "";
                    if (show)
                        mark = "<sprite=1>";
                    else
                        mark = "<sprite=0>";

#if AT_I2LOC_PRESET
                 Name.text = mark+I2.Loc.LocalizationManager.GetTranslation(baseName);
#else
                    Name.text = mark + baseName;
#endif
                    //  Name.margin = new Vector4(leftMarginPer * nest, 0, 0, 0);
                }
        }
        public void Show()
        {
            if (slotparent != null)
                slotparent.SetupMark(true);
        }
        public void Click()
        {
            if (response != null)
                response(category, value, nested);
        }
        public void Selected(bool v)
        {
            if (Name != null)
                if (v)
                {
                    Name.color = selectedColor;
                }
                else
                {
                    Name.color = defaultColor;
                }
        }
        public string getNested
        {
            get
            {
                return nested;
            }
        }


    }
}