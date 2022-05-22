using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Atavism
{

    public delegate void RecipeResponse(AtavismCraftingRecipe auction);



        public class UGUICraftRecipeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
        {
            public Image Icon;
            public Image Selected;
            public TextMeshProUGUI Name;
            public TextMeshProUGUI Level;
            public TextMeshProUGUI Status;
            public TextMeshProUGUI Count;
            AtavismInventoryItem item;
            RecipeResponse response;
            AtavismCraftingRecipe recipe;
            bool mouseEntered = false;
          
            public void SetDetale(AtavismCraftingRecipe recipe, RecipeResponse response, Toggle avaiable,  Toggle backpack, AtavismCraftingRecipe selectedRecipe)
            {

                int number = 10000;
                this.response = response;
                this.recipe = recipe;
                if (Icon != null)
                    Icon.sprite = recipe.icon;
                if (Selected != null)
                    if (selectedRecipe != null)
                    {
                        if (selectedRecipe.recipeID == recipe.recipeID)
                            Selected.enabled = true;
                        else
                            Selected.enabled = false;
                    }
                    else
                    {
                        Selected.enabled = false;

                    }
                for (int i = 0; i < recipe.itemsReq.Count; i++)
                {
                    int num = Inventory.Instance.GetCountOfItem(recipe.itemsReq[i]);
                    int _count = num / recipe.itemsReqCounts[i];
                    if (number > _count)
                        number = _count;
                }

                if (backpack != null && backpack.isOn && recipe.stationReq != "none")
                {
                    if (gameObject.activeSelf)
                        gameObject.SetActive(false);
                }

                if (avaiable.isOn && (number < 1 || Skills.Instance.GetPlayerSkillLevel(recipe.skillID) < recipe.skillLevelReq))
                {
                    if (gameObject.activeSelf)
                        gameObject.SetActive(false);
                }
                if (Count != null)
                    Count.text = number.ToString();
                if (Level != null)
                    Level.text = recipe.skillLevelReq.ToString();
#if AT_I2LOC_PRESET
                if (Name != null) Name.text = I2.Loc.LocalizationManager.GetTranslation(recipe.recipeName)+" ("+number+")" ;
#else
                if (Name != null)
                    Name.text = recipe.recipeName + " (" + number + ")";
#endif
                string msg = "";
            //    Debug.LogError(recipe.recipeName+" >"+recipe.stationReq+"<  >"+Crafting.Instance.StationType+"< "+Crafting.Instance.StationType.Length+" "+recipe.stationReq.Equals(Crafting.Instance.StationType)+" "+(Crafting.Instance.StationType.Length > 0 && !recipe.stationReq.Equals("Any"))+" "+(Crafting.Instance.StationType.Length ==0 && (recipe.stationReq.Equals("Any")||!recipe.stationReq.Equals("none")))+" "+recipe.stationReq.Equals("none"));
                if (Skills.Instance.GetPlayerSkillLevel(recipe.skillID) < recipe.skillLevelReq)
                {
#if AT_I2LOC_PRESET
                    msg = I2.Loc.LocalizationManager.GetTranslation("Low Skill") ;
#else
                    msg = "Low Skill";
#endif
                }
                else if (number < 1)
                {
#if AT_I2LOC_PRESET
                    msg = I2.Loc.LocalizationManager.GetTranslation("Low Resource");
#else
                    msg = "Low Resources";
#endif
                }
                else if(!recipe.stationReq.Equals(Crafting.Instance.StationType) && ((Crafting.Instance.StationType.Length > 0 && !recipe.stationReq.Equals("Any"))||(Crafting.Instance.StationType.Length ==0 && (recipe.stationReq.Equals("Any")||!recipe.stationReq.Equals("none"))))&& !recipe.stationReq.Equals("none"))
                {
#if AT_I2LOC_PRESET
                    msg = I2.Loc.LocalizationManager.GetTranslation("Wrong Station");
#else
                    msg = "Wrong Station";
#endif
                }

                if (Status != null)
                    Status.text = msg;

            }
            public void Click()
            {
                if (response != null)
                    response(recipe);

            }
            public void OnPointerEnter(PointerEventData eventData)
            {

                MouseEntered = true;
            }

            public void OnPointerExit(PointerEventData eventData)
            {

                MouseEntered = false;
            }
            void HideTooltip()
            {
                UGUITooltip.Instance.Hide();

            }

            public bool MouseEntered
            {
                get
                {
                    return mouseEntered;
                }
                set
                {
                    mouseEntered = value;
                    if (mouseEntered && recipe != null)
                    {
                        recipe.ShowTooltip(gameObject);
                        //   cor = StartCoroutine(CheckOver());
                    }
                    else
                    {
                        HideTooltip();
                    }
                }
            }
        }
    }
