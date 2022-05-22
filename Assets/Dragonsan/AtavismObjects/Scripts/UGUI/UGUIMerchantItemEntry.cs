using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Atavism
{

    public class UGUIMerchantItemEntry : MonoBehaviour
    {

        //public Text name;
        public Text name;
        public TextMeshProUGUI nameTMP;
        [SerializeField] Text taught;
        [SerializeField] TextMeshProUGUI taughtTMP;
        public UGUIMerchantItemSlot itemSlot;
        public List<UGUICurrency> currencyDisplays;
        //MerchantItem merchantItem;

        // Use this for initialization
        void Start()
        {
        }

        public void UpdateMerchantItemData(MerchantItem merchantItem, UGUIMerchantFrame merchantFrame)
        {
            //	this.merchantItem = merchantItem;
            itemSlot.UpdateMerchantItemData(merchantItem, merchantFrame);

            AtavismInventoryItem aii = Inventory.Instance.GetItemByTemplateID(merchantItem.itemID);
#if AT_I2LOC_PRESET
        if (name!=null)name.text = I2.Loc.LocalizationManager.GetTranslation("Items/" + aii.name);
        if (nameTMP!=null)nameTMP.text = I2.Loc.LocalizationManager.GetTranslation("Items/" + aii.name);
#else
            if (name != null)
                name.text = aii.name;
            if (nameTMP != null)
                nameTMP.text = aii.name;
#endif
            List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(merchantItem.purchaseCurrency, merchantItem.cost);
            for (int i = 0; i < currencyDisplays.Count; i++)
            {
                if (i < currencyDisplayList.Count)
                {
                    currencyDisplays[i].gameObject.SetActive(true);
                    currencyDisplays[i].SetCurrencyDisplayData(currencyDisplayList[i]);
                }
                else
                {
                    currencyDisplays[i].gameObject.SetActive(false);
                }
            }
            if (aii.GetEffectPositionsOfTypes("UseAbility").Count > 0)
            {
                if (aii.name.IndexOf("TeachAbility") > -1)
                {
                    int abilityID = int.Parse(aii.itemEffectNames[aii.GetEffectPositionsOfTypes("UseAbility")[0]]);
                    //  AtavismAbility aa = Abilities.Instance.GetAbility(abilityID);
                    AtavismAbility paa = Abilities.Instance.GetPlayerAbility(abilityID);
                    if (paa != null)
                    {
                        if (taught != null)
                        {
                            if (!taught.gameObject.activeSelf)
                                taught.gameObject.SetActive(true);
                            return;
                        }
                        if (taughtTMP != null)
                        {
                            if (!taughtTMP.gameObject.activeSelf)
                                taughtTMP.gameObject.SetActive(true);
                            return;
                        }
                    }
                }
            }
            if (taught != null)
                if (taught.gameObject.activeSelf)
                    taught.gameObject.SetActive(false);
            if (taughtTMP != null)
                if (taughtTMP.gameObject.activeSelf)
                    taughtTMP.gameObject.SetActive(false);
        }
        public void UpdateShopItemData(ShopItem merchantItem, UGUIShopWindow merchantFrame)
        {
            //	this.merchantItem = merchantItem;
            itemSlot.UpdateShopItemData(merchantItem, merchantFrame);

            AtavismInventoryItem aii = merchantItem.item;
#if AT_I2LOC_PRESET
        if (name!=null)name.text = I2.Loc.LocalizationManager.GetTranslation("Items/" + aii.name);
        if (nameTMP!=null)nameTMP.text = I2.Loc.LocalizationManager.GetTranslation("Items/" + aii.name);
#else
            if (name != null)
                name.text = aii.name;
            if (nameTMP != null)
                nameTMP.text = aii.name;
#endif
            List<CurrencyDisplay> currencyDisplayList = Inventory.Instance.GenerateCurrencyListFromAmount(merchantItem.purchaseCurrency, merchantItem.cost);
            for (int i = 0; i < currencyDisplays.Count; i++)
            {
                if (i < currencyDisplayList.Count)
                {
                    currencyDisplays[i].gameObject.SetActive(true);
                    currencyDisplays[i].SetCurrencyDisplayData(currencyDisplayList[i]);
                }
                else
                {
                    currencyDisplays[i].gameObject.SetActive(false);
                }
            }
            if (aii.GetEffectPositionsOfTypes("UseAbility").Count > 0)
            {
              /*  if (aii.name.IndexOf("TeachAbility") > -1)
                {*/
                    int abilityID = int.Parse(aii.itemEffectNames[aii.GetEffectPositionsOfTypes("UseAbility")[0]]);
                    //  AtavismAbility aa = Abilities.Instance.GetAbility(abilityID);
                    AtavismAbility paa = Abilities.Instance.GetPlayerAbility(abilityID);
                    if (paa != null)
                    {
                        if (taught != null)
                        {
                            if (!taught.gameObject.activeSelf)
                                taught.gameObject.SetActive(true);
                          //  return;
                        }
                        if (taughtTMP != null)
                        {
                            if (!taughtTMP.gameObject.activeSelf)
                                taughtTMP.gameObject.SetActive(true);
                         //   return;
                        }
                    }
               // }
            }else if (aii.GetEffectPositionsOfTypes("Blueprint").Count > 0)
            {
                if (CheckRecipe(int.Parse(aii.itemEffectValues[aii.GetEffectPositionsOfTypes("Blueprint")[0]])))
                {
                    if (taught != null)
                    {
                        if (!taught.gameObject.activeSelf)
                            taught.gameObject.SetActive(true);
                    }
                    if (taughtTMP != null)
                    {
                        if (!taughtTMP.gameObject.activeSelf)
                            taughtTMP.gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                if (taught != null)
                    if (taught.gameObject.activeSelf)
                        taught.gameObject.SetActive(false);
                if (taughtTMP != null)
                    if (taughtTMP.gameObject.activeSelf)
                        taughtTMP.gameObject.SetActive(false);
            }
        }
        
        bool CheckRecipe(int recipeId)
        {
            if (ClientAPI.GetPlayerObject() != null && ClientAPI.GetPlayerObject().PropertyExists("recipes"))
            {
                LinkedList<object> recipes_prop = (LinkedList<object>)ClientAPI.GetPlayerObject().GetProperty("recipes");
                foreach (string recipeString in recipes_prop)
                {
                    int recipeID = int.Parse(recipeString);
                    if (recipeID == recipeId)
                        return true;
                }
            }
            return false;
        }
    }
}