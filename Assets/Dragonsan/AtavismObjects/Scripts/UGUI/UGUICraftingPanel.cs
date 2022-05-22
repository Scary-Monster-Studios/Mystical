using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using TMPro;

namespace Atavism
{
    public class UGUICraftingPanel : MonoBehaviour, IPointerDownHandler
    {

        public UGUIPanelTitleBar titleBar;
        public List<UGUICraftingSlot> craftingSlots;
        public List<UGUIItemDisplay> resultSlots;
        public Button craftButton;
        bool showing = false;
        [SerializeField] GameObject panel;
        [AtavismSeparator("Craft Book")]
        [SerializeField] bool newCraft = false;
        [SerializeField] GameObject craftPrefab;
        [SerializeField] Transform recipeGrid;
        [SerializeField] List<UGUICraftRecipeSlot> recipeSlots = new List<UGUICraftRecipeSlot>();
        List<int> recipies = new List<int>();
        [SerializeField] List<UGUICraftingSlot> requiredItemSlots;
        [SerializeField] List<UGUICraftingSlot> craftingSlots1;
        [SerializeField] List<UGUICraftingSlot> craftingSlots2;
        [SerializeField] List<UGUICraftingSlot> craftingSlots3;
        [SerializeField] List<UGUICraftingSlot> craftingSlots4;
        [SerializeField] TMP_InputField inputCount;
        [SerializeField] TextMeshProUGUI stationName;
        [SerializeField] TextMeshProUGUI skillName;
        [SerializeField] TextMeshProUGUI skillExp;
        [SerializeField] Slider skillExpSlider;
        [SerializeField] Toggle availableToggle;
        [SerializeField] Toggle onlyBackpackToggle;
        // [SerializeField] List<Station> stations = new List<Station>();
        [SerializeField] TMP_Dropdown skillList;
        [SerializeField] List<GameObject> hideObjects;
        AtavismCraftingRecipe recipeSelected;
        int selectSkill = 0;

        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("CRAFTING_GRID_UPDATE", this);
            AtavismEventSystem.RegisterEvent("CRAFTING_RECIPE_UPDATE", this);
            AtavismEventSystem.RegisterEvent("CRAFTING_START", this);
            AtavismEventSystem.RegisterEvent("CLOSE_CRAFTING_STATION", this);
            AtavismEventSystem.RegisterEvent("INVENTORY_UPDATE", this);
            AtavismEventSystem.RegisterEvent("SKILL_UPDATE", this);
            AtavismEventSystem.RegisterEvent("SKILL_ICON_UPDATE", this);
            if (inputCount != null)
                inputCount.text = 1 + "";
            if (ClientAPI.GetPlayerObject() != null)
            {
                if (ClientAPI.GetPlayerObject().GameObject != null)
                {
                    if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>() != null)
                    {
                        ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler("recipes", HandleRecipe);
                    }
                    else
                    {
                        Debug.LogError("UGUIDeathPopup: AtavismNode is null");
                    }
                }
                else
                {
                    Debug.LogError("UGUIDeathPopup: GameObject is null");
                }
            }
            else
            {
                Debug.LogError("UGUIDeathPopup: PlayerObject is null");
            }
            if (ClientAPI.GetPlayerObject() != null)
            {
                if (ClientAPI.GetPlayerObject().PropertyExists("recipes"))
                {
                    recipies.Clear();
                    LinkedList<object> recipeList = (LinkedList<object>)ClientAPI.GetPlayerObject().GetProperty("recipes");
                    foreach (string s in recipeList)
                    {
                        recipies.Add(int.Parse(s));
                    }
                }
            }
            if (titleBar != null)
                titleBar.SetOnPanelClose(Hide);
            Hide();
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("CRAFTING_GRID_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("CRAFTING_RECIPE_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("CRAFTING_START", this);
            AtavismEventSystem.UnregisterEvent("CLOSE_CRAFTING_STATION", this);
            AtavismEventSystem.UnregisterEvent("INVENTORY_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("SKILL_UPDATE", this);
            AtavismEventSystem.UnregisterEvent("SKILL_ICON_UPDATE", this);
            if (ClientAPI.GetPlayerObject() != null && ClientAPI.GetPlayerObject().GameObject != null && ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>() != null)
            {
                ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler("recipes", HandleRecipe);
            }
        }

        private void HandleRecipe(object sender, PropertyChangeEventArgs args)
        {
            if (ClientAPI.GetPlayerObject() != null)
            {
                if (ClientAPI.GetPlayerObject().PropertyExists("recipes"))
                {
                    recipies.Clear();
                    LinkedList<object> recipeList = (LinkedList<object>)ClientAPI.GetPlayerObject().GetProperty("recipes");
                    foreach (string s in recipeList)
                    {
                        recipies.Add(int.Parse(s));
                    }
                }
            }
            updateRecipeList();
        }

        public void Toggle()
        {

            if (showing)
                Hide();
            else
                Show();

        }

        public void Show()
        {
            if (panel != null)
                panel.SetActive(true);
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            showing = true;

            if (titleBar != null)
                titleBar.SetPanelTitle(Crafting.Instance.StationType.ToString());

            UpdateCraftingGrid();
            if (!newCraft)
                AtavismCursor.Instance.SetUGUIActivatableClickedOverride(PlaceCraftingItem);
            AtavismUIUtility.BringToFront(gameObject);
            if (newCraft)
            {
                updateRecipeList();
            }
        }

        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            showing = false;

            // Set all referenced items back to non referenced
            for (int i = 0; i < craftingSlots.Count; i++)
            {
                if (craftingSlots[i] != null)
                    craftingSlots[i].ResetSlot();
            }
            recipeSelected = null;
            HideObjects();
            Crafting.Instance.ClearGrid();
            Crafting.Instance.StationType = "";
            Crafting.Instance.Station = null;

            if (AtavismCursor.Instance != null)
                AtavismCursor.Instance.ClearUGUIActivatableClickedOverride(PlaceCraftingItem);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "CRAFTING_GRID_UPDATE")
            {
                // Update 
                UpdateCraftingGrid();
            }
            else if (eData.eventType == "CRAFTING_START")
            {
                if (!showing)
                {
                    Show();
                }
            }
            else if (eData.eventType == "CLOSE_CRAFTING_STATION")
            {
                Hide();
            }
            else if (eData.eventType == "CRAFTING_RECIPE_UPDATE" || eData.eventType == "INVENTORY_UPDATE" || eData.eventType == "SKILL_UPDATE"|| eData.eventType == "SKILL_ICON_UPDATE")
            {
                if (showing && newCraft)
                {
                    updateRecipeList();
                    if (recipeSelected != null)
                        selectRecipe(recipeSelected);
                }
            }

        }

        public void CountDecrease()
        {
            if (recipeSelected == null)
                return;
            string count = inputCount.text;
            if (count == "" || count == " ")
                count = "1";
            int _count = int.Parse(count);
            _count -= 1;
            if (_count < 1)
                _count = 1;
            inputCount.text = _count.ToString();
        }

        public void CountDecreaseMax()
        {
            inputCount.text = "1";
        }

        public void CountIncrease()
        {
            if (recipeSelected == null)
                return;
            string count = inputCount.text;
            if (count == "" || count == " ")
                count = "1";
            int _count = int.Parse(count);
            _count += 1;
            int number = 100000;
            int _countCraft = 0;
            for (int i = 0; i < recipeSelected.itemsReq.Count; i++)
            {
                int num = Inventory.Instance.GetCountOfItem(recipeSelected.itemsReq[i]);
                _countCraft = num / recipeSelected.itemsReqCounts[i];
                if (number > _countCraft)
                    number = _countCraft;
            }
            if (_count > number)
                _count = number;
            inputCount.text = _count.ToString();
        }

        public void CountIncreaseMax()
        {
            if (recipeSelected == null)
                return;
            int number = 100000;
            int _countCraft = 0;
            for (int i = 0; i < recipeSelected.itemsReq.Count; i++)
            {
                int num = Inventory.Instance.GetCountOfItem(recipeSelected.itemsReq[i]);
                _countCraft = num / recipeSelected.itemsReqCounts[i];
                if (number > _countCraft)
                    number = _countCraft;
            }
            inputCount.text = number.ToString();
        }

        public void ShowAvailable()
        {
            updateRecipeList();
        }

        public void ChangeSkill()
        {
            selectSkill = skillList.value;
            updateRecipeList();
        }

        void HideObjects()
        {
            for (int i = 0; i < requiredItemSlots.Count; i++)
            {
                if (requiredItemSlots[i].gameObject.activeSelf)
                    requiredItemSlots[i].gameObject.SetActive(false);
                requiredItemSlots[i].UpdateCraftingBookSlotData(null);
            }
            for (int i = 0; i < craftingSlots1.Count; i++)
            {
                if (craftingSlots1[i].gameObject.activeSelf)
                    craftingSlots1[i].gameObject.SetActive(false);
                craftingSlots1[i].UpdateCraftingBookSlotData(null);
            }
            for (int i = 0; i < craftingSlots2.Count; i++)
            {
                if (craftingSlots2[i].gameObject.activeSelf)
                    craftingSlots2[i].gameObject.SetActive(false);
                craftingSlots2[i].UpdateCraftingBookSlotData(null);
            }
            for (int i = 0; i < craftingSlots3.Count; i++)
            {
                if (craftingSlots3[i].gameObject.activeSelf)
                    craftingSlots3[i].gameObject.SetActive(false);
                craftingSlots3[i].UpdateCraftingBookSlotData(null);
            }
            for (int i = 0; i < craftingSlots4.Count; i++)
            {
                if (craftingSlots4[i].gameObject.activeSelf)
                    craftingSlots4[i].gameObject.SetActive(false);
                craftingSlots4[i].UpdateCraftingBookSlotData(null);
            }
            foreach (GameObject go in hideObjects)
            {
                if (go != null)
                    if (go.activeSelf)
                        go.SetActive(false);
            }
        }


        void updateRecipeList()
        {
            if (ClientAPI.GetPlayerObject().PropertyExists("recipes"))
            {
                recipies.Clear();
                LinkedList<object> recipeList = (LinkedList<object>)ClientAPI.GetPlayerObject().GetProperty("recipes");
                foreach (string s in recipeList)
                {
                    recipies.Add(int.Parse(s));
//                    Debug.Log("Know Recipe "+s);
                }
            }

            if (stationName != null)
            {
#if AT_I2LOC_PRESET
                stationName.text = I2.Loc.LocalizationManager.GetTranslation(Crafting.Instance.StationType);
#else
                stationName.text = Crafting.Instance.StationType;
#endif
            }

            if (skillName != null)
            {
#if AT_I2LOC_PRESET
                skillName.text = I2.Loc.LocalizationManager.GetTranslation(Crafting.Instance.StationType);
#else
                skillName.text = Crafting.Instance.StationType;
#endif
            }

            List<int> skills = Crafting.Instance.GetShowAllSkills ?Crafting.Instance.GetShowAllKnownSkills? Skills.Instance.GetAllKnownCraftSkillsID() : Skills.Instance.GetAllCraftSkillsID() : Inventory.Instance.GetCraftingRecipeMatch(recipies, Crafting.Instance.StationType);
            //            Debug.LogError("Crafting skils list "+skills+" "+skills.Count);
            if (skillList == null)
                return;

            if (skillList.options != null && skillList.options.Count > 0)
                skillList.options.Clear();
            if(skills.Count>0)
            foreach (int sid in skills)
            {
                Skill skill = Skills.Instance.GetSkillByID(sid);
                if (skill != null)
                {
#if AT_I2LOC_PRESET
                skillList.options.Add(new TMP_Dropdown.OptionData(I2.Loc.LocalizationManager.GetTranslation(skill.skillname)));
#else
                    skillList.options.Add(new TMP_Dropdown.OptionData(skill.skillname));
#endif
                }
            }



            if (skills.Count > selectSkill)
                skillList.value = selectSkill;
            else
                skillList.value = 0;
            if (skillList.options.Count > skillList.value)
                skillList.captionText.text = skillList.options[skillList.value].text;
            else
                skillList.captionText.text = "";
            if (skillList.options.Count == 0)
            {
                foreach (UGUICraftRecipeSlot uguias in recipeSlots)
                {
                    if (uguias != null)
                        uguias.gameObject.SetActive(false);
                }
                if (skillExpSlider != null)
                {
                    skillExpSlider.enabled = false;
                }
                if (skillExp != null)
                {
                    skillExp.text = "";
                }
                return;
            }


            Skill _skill = Skills.Instance.GetSkillByID(skills[skillList.value]);
            if (Skills.Instance.PlayerSkills.ContainsKey(skills[skillList.value]))
            {
                Skill _skillPly = Skills.Instance.PlayerSkills[skills[skillList.value]];
                if (skillExpSlider != null)
                {
                    if (!skillExpSlider.isActiveAndEnabled)
                        skillExpSlider.enabled = true;
                    if (_skillPly.expMax == 0 && _skillPly.exp == 0)
                    {
                        skillExpSlider.maxValue = _skillPly.MaximumLevel;
                        skillExpSlider.value = _skillPly.CurrentLevel;
                    }
                    else
                    {
                        skillExpSlider.maxValue = _skillPly.expMax;
                        skillExpSlider.value = _skillPly.exp;
                    }
                }
                if (skillExp != null)
                {
                    if (_skillPly.expMax == 0 && _skillPly.exp == 0)
                    {
                        skillExp.text = _skillPly.CurrentLevel + "/" + _skillPly.MaximumLevel;
                    }
                    else
                    {
                        skillExp.text = _skillPly.exp + "/" + _skillPly.expMax;
                    }
                }

                //  Debug.LogError("updateRecipeList " + _skillPly);
                //    Debug.LogError("updateRecipeList " + _skillPly.exp + " " + _skillPly.expMax);
            }
            else
            {
                if (skillExpSlider != null)
                {
                    skillExpSlider.maxValue = 1;
                    skillExpSlider.value = 0;
                    skillExpSlider.enabled = false;
                }
                if (skillExp != null)
                {
                    skillExp.text = "Not learned";
                }
            }
            foreach (UGUICraftRecipeSlot uguias in recipeSlots)
            {
                if (uguias != null)
                    uguias.gameObject.SetActive(false);
            }
            int i = 1;
            //   Debug.LogError(recipies.Count);
            foreach (AtavismCraftingRecipe recipe in Inventory.Instance.GetCraftingRecipeMatch(recipies, _skill.id))
            {
                //   Debug.LogError("i:" + i + " recipe:" + recipe + "  recipe ID:" + recipe.recipeID);
                if (i > recipeSlots.Count)
                {
                    GameObject go = (GameObject)Instantiate(craftPrefab, recipeGrid);
                    recipeSlots.Add(go.GetComponent<UGUICraftRecipeSlot>());
                }
                if (!recipeSlots[i - 1].gameObject.activeSelf)
                    recipeSlots[i - 1].gameObject.SetActive(true);
                recipeSlots[i - 1].SetDetale(recipe, selectRecipe, availableToggle, onlyBackpackToggle, recipeSelected);
                i++;
            }
            if (recipeSelected != null)
            {
                int number = 100000;
                int _countCraft = 0;
                for (int iii = 0; iii < recipeSelected.itemsReq.Count; iii++)
                {
                    int num = Inventory.Instance.GetCountOfItem(recipeSelected.itemsReq[iii]);
                    _countCraft = num / recipeSelected.itemsReqCounts[iii];
                    if (number > _countCraft)
                        number = _countCraft;
                }

                if (number > 0)
                {
                    if (Crafting.Instance.StationType != "" && recipeSelected != null && (recipeSelected.stationReq.Equals("Any") || recipeSelected.stationReq.Equals(Crafting.Instance.StationType)))
                    {
                        craftButton.interactable = true;
                    }
                    else if (recipeSelected != null && recipeSelected.stationReq.Equals("none"))
                    {
                        craftButton.interactable = true;
                    }
                    else
                    {
                        craftButton.interactable = false;
                    }
                }
                else
                {
                    craftButton.interactable = false;
                }
            }
            else
            {
                craftButton.interactable = false;

            }

        }






        private void selectRecipe(AtavismCraftingRecipe recipe)
        {
            recipeSelected = recipe;
         //   Debug.LogError(" recipe:" + recipe + "  recipe ID:" + recipe.recipeID);
         //   Debug.LogError(" itemsReq:" + recipe.itemsReq.Count);

            foreach (GameObject go in hideObjects)
            {
                if (go != null)
                    if (!go.activeSelf)
                        go.SetActive(true);
            }


            for (int i = 0; i < requiredItemSlots.Count; i++)
            {
                if (i < recipe.itemsReq.Count && Inventory.Instance.GetItemByTemplateID(recipe.itemsReq[i]) != null)
                {
                    CraftingComponent cc = new CraftingComponent();
                    cc.item = Inventory.Instance.GetItemByTemplateID(recipe.itemsReq[i]);
                    cc.count = recipe.itemsReqCounts[i];
                    if (!requiredItemSlots[i].gameObject.activeSelf)
                        requiredItemSlots[i].gameObject.SetActive(true);
                    requiredItemSlots[i].UpdateCraftingBookSlotData(cc);
                }
                else
                {
                    if (requiredItemSlots[i].gameObject.activeSelf)
                        requiredItemSlots[i].gameObject.SetActive(false);
                    requiredItemSlots[i].UpdateCraftingBookSlotData(null);
                }
            }


            for (int i = 0; i < craftingSlots1.Count; i++)
            {
                if (i < recipe.createsItems.Count && Inventory.Instance.GetItemByTemplateID(recipe.createsItems[i]) != null)
                {
                    CraftingComponent cc = new CraftingComponent();
                    cc.item = Inventory.Instance.GetItemByTemplateID(recipe.createsItems[i]);
                    cc.count = recipe.createsItemsCounts[i];
                    if (!craftingSlots1[i].gameObject.activeSelf)
                        craftingSlots1[i].gameObject.SetActive(true);
                    craftingSlots1[i].UpdateCraftingBookSlotData(cc);
                }
                else
                {
                    if (craftingSlots1[i].gameObject.activeSelf)
                        craftingSlots1[i].gameObject.SetActive(false);
                    craftingSlots1[i].UpdateCraftingBookSlotData(null);
                }
            }
            for (int i = 0; i < craftingSlots2.Count; i++)
            {
                if (i < recipe.createsItems2.Count && Inventory.Instance.GetItemByTemplateID(recipe.createsItems2[i]) != null)
                {
                    CraftingComponent cc = new CraftingComponent();
                    cc.item = Inventory.Instance.GetItemByTemplateID(recipe.createsItems2[i]);
                    cc.count = recipe.createsItemsCounts2[i];
                    if (!craftingSlots2[i].gameObject.activeSelf)
                        craftingSlots2[i].gameObject.SetActive(true);
                    craftingSlots2[i].UpdateCraftingBookSlotData(cc);
                }
                else
                {
                    if (craftingSlots2[i].gameObject.activeSelf)
                        craftingSlots2[i].gameObject.SetActive(false);
                    craftingSlots2[i].UpdateCraftingBookSlotData(null);
                }
            }
            for (int i = 0; i < craftingSlots3.Count; i++)
            {
                if (i < recipe.createsItems3.Count && Inventory.Instance.GetItemByTemplateID(recipe.createsItems3[i]) != null)
                {
                    CraftingComponent cc = new CraftingComponent();
                    cc.item = Inventory.Instance.GetItemByTemplateID(recipe.createsItems3[i]);
                    cc.count = recipe.createsItemsCounts3[i];
                    if (!craftingSlots3[i].gameObject.activeSelf)
                        craftingSlots3[i].gameObject.SetActive(true);
                    craftingSlots3[i].UpdateCraftingBookSlotData(cc);
                }
                else
                {
                    if (craftingSlots3[i].gameObject.activeSelf)
                        craftingSlots3[i].gameObject.SetActive(false);
                    craftingSlots3[i].UpdateCraftingBookSlotData(null);
                }
            }
            for (int i = 0; i < craftingSlots4.Count; i++)
            {
                if (i < recipe.createsItems4.Count && Inventory.Instance.GetItemByTemplateID(recipe.createsItems4[i]) != null)
                {
                    CraftingComponent cc = new CraftingComponent();
                    cc.item = Inventory.Instance.GetItemByTemplateID(recipe.createsItems4[i]);
                    cc.count = recipe.createsItemsCounts4[i];
                    if (!craftingSlots4[i].gameObject.activeSelf)
                        craftingSlots4[i].gameObject.SetActive(true);
                    craftingSlots4[i].UpdateCraftingBookSlotData(cc);
                }
                else
                {
                    if (craftingSlots4[i].gameObject.activeSelf)
                        craftingSlots4[i].gameObject.SetActive(false);
                    craftingSlots4[i].UpdateCraftingBookSlotData(null);
                }
            }
            int number = 100000;
            int _countCraft = 0;
            for (int iii = 0; iii < recipeSelected.itemsReq.Count; iii++)
            {
                int num = Inventory.Instance.GetCountOfItem(recipeSelected.itemsReq[iii]);
                _countCraft = num / recipeSelected.itemsReqCounts[iii];
                if (number > _countCraft)
                    number = _countCraft;
            }
            inputCount.text = number.ToString();
            if (number > 0)
            {
                if (Crafting.Instance.StationType != "")
                    craftButton.interactable = true;
                else
                    craftButton.interactable = false;
            }
            else
                craftButton.interactable = false;
            updateRecipeList();
        }

        void UpdateCraftingGrid()
        {
            if (newCraft)
                return;
            for (int i = 0; i < craftingSlots.Count; i++)
            {
                if (i < Crafting.Instance.GridItems.Count && Crafting.Instance.GridItems[i].item != null)
                {
                    craftingSlots[i].UpdateCraftingSlotData(Crafting.Instance.GridItems[i]);
                }
                else
                {
                    craftingSlots[i].UpdateCraftingSlotData(null);
                }
            }

            for (int i = 0; i < resultSlots.Count; i++)
            {
                if (i < Crafting.Instance.ResultItems.Count)
                {
                    resultSlots[i].gameObject.SetActive(true);
                    resultSlots[i].SetItemData(Crafting.Instance.ResultItems[i], null);
                }
                else
                {
                    resultSlots[i].gameObject.SetActive(false);
                }
            }

            if (Crafting.Instance.ResultItems.Count > 0)
            {
                craftButton.interactable = true;
            }
            else
            {

                craftButton.interactable = false;
            }
        }

        public void DoCraft()
        {
            if (newCraft)
            {

                string count = inputCount.text;
                if (count.Length == 0)
                    count = "1";
                if (recipeSelected == null)
                    return;
                Crafting.Instance.CraftItemBook(recipeSelected.recipeID, int.Parse(count));
            }
            else
            {
                Crafting.Instance.CraftItem();
            }
        }

        public void PlaceCraftingItem(UGUIAtavismActivatable activatable)
        {
            if (activatable.Link != null)
                return;
            for (int i = 0; i < craftingSlots.Count; i++)
            {
                if (i < Crafting.Instance.GridItems.Count && Crafting.Instance.GridItems[i].item == null)
                {

                    craftingSlots[i].SetActivatable(activatable);
                    return;
                }
            }
        }
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            // Focus the window
            AtavismUIUtility.BringToFront(this.gameObject);
        }
    }
}