using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public enum CraftingStationType
    {
        Anvil,
        Smelter,
        Pot,
        Cauldron,
        Oven,
        Loom,
        Sewing,
        Tannery,
        Masonry,
        Alchemy,
        Desk,
        Sawmill,
        None
    }

    public class CraftingStation : MonoBehaviour
    {

        //public CraftingStationType stationType;
        public string stationType;
        //public GameObject target;
      //  public Sprite icon;
        public Texture2D cursorIcon;
        public CoordinatedEffect coordEffect;
        bool mouseOver = false;

        // Use this for initialization
        void Start()
        {
            gameObject.AddComponent<AtavismNode>();
            GetComponent<AtavismNode>().AddLocalProperty("craftingStation", stationType);
            GetComponent<AtavismNode>().AddLocalProperty("targetable", false);
        }

        // Update is called once per frame
        void Update()
        {
            if (mouseOver)
            {
                if (AtavismCursor.Instance.IsMouseOverUI())
                {
                    AtavismCursor.Instance.ClearMouseOverObject(GetComponent<AtavismNode>());
                }
                else
                {
                    AtavismCursor.Instance.SetMouseOverObject(GetComponent<AtavismNode>(), cursorIcon, 4);
                    if (Input.GetMouseButtonDown(1))
                    {
                        // this object was clicked - do something
                        ActivateCraftingStation();
                    }
                }
            }
        }

        void OnMouseOver()
        {
            AtavismCursor.Instance.SetMouseOverObject(GetComponent<AtavismNode>(), cursorIcon, 4);
            mouseOver = true;
        }

        void OnMouseExit()
        {
            AtavismCursor.Instance.ClearMouseOverObject(GetComponent<AtavismNode>());
            mouseOver = false;
        }

        void OnMouseDown()
        {
            // this object was clicked - do something
            if (!AtavismCursor.Instance.IsMouseOverUI())
            {
                ActivateCraftingStation();
            }
        }

        public void ActivateCraftingStation()
        {
            /*if (target == null) 
                target = gameObject;
            target.SetActive(!target.activeSelf);*/
            Crafting.Instance.StationType = stationType;
            Crafting.Instance.Station = gameObject;
            if (coordEffect != null)
                Crafting.Instance.CoordEffect = coordEffect.name;

            string[] args = new string[1];
            AtavismEventSystem.DispatchEvent("CRAFTING_START", args);

            // Set name and icon of UI
            //target.transform.GetChild(0).FindChild("Name").GetComponent<UILabel>().text = stationType.ToString();
            //target.transform.GetChild(0).FindChild("BagIcon").GetComponent<UISprite>().spriteName = icon.name;
        }

        public void CloseStation()
        {
            //target.SetActive(false);
        }
    }
}