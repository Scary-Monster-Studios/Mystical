using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Atavism
{

    public class UGUICurrencyInputPanel : MonoBehaviour
    {

        public List<InputField> inputFields;
        public List<TMP_InputField> TMPInputFields;
        public List<Image> icons;
        public bool allowMoreThanPlayersCurrency = false;
        List<int> currencyIDs = new List<int>();
        List<int> currencyAmounts = new List<int>();

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Sets the currencies icons. Call this function first
        /// </summary>
        /// <param name="currencyList">Currency list.</param>
        public void SetCurrencies(List<Currency> currencyList)
        {
            currencyIDs = new List<int>();
            currencyAmounts = new List<int>();
            for (int i = 0; i < currencyList.Count; i++)
            {
                currencyIDs.Add(currencyList[i].id);
                currencyAmounts.Add(0);
                if (i < icons.Count)
                {
                    icons[i].sprite = currencyList[i].icon;
                }
            }
            ClearCurrencyAmounts();
        }

        /// <summary>
        /// Sets the currency amounts if the input should be filled before the player types anything in.
        /// Should be called after SetCurrencies.
        /// </summary>
        /// <param name="currencyID">Currency I.</param>
        /// <param name="amount">Amount.</param>
        public void SetCurrencyAmounts(int currencyID, long amount)
        {
            foreach (InputField inputField in inputFields)
            {
                if (inputField != null)
                    inputField.text = "0";
            }
            foreach (TMP_InputField inputField in TMPInputFields)
            {
                if (inputField != null)
                    inputField.text = "0";
            }
            List<Vector2> convertedCurrencies = Inventory.Instance.GetConvertedCurrencyValues(currencyID, amount);
            foreach (Vector2 currency in convertedCurrencies)
            {
                for (int i = 0; i < currencyIDs.Count; i++)
                {
                    if (currencyIDs[i] == (int)currency.x)
                    {
                        currencyAmounts[i] = (int)currency.y;
                        if (inputFields.Count > i)
                            inputFields[i].text = currencyAmounts[i].ToString();
                        if (TMPInputFields.Count > i)
                            TMPInputFields[i].text = currencyAmounts[i].ToString();
                    }
                }
            }
        }

        public void ClearCurrencyAmounts()
        {
            foreach (InputField inputField in inputFields)
            {
                if (inputField != null)
                    inputField.text = "0";
            }
            foreach (TMP_InputField inputField in TMPInputFields)
            {
                if (inputField != null)
                    inputField.text = "0";
            }

            for (int i = 0; i < currencyAmounts.Count; i++)
            {
                currencyAmounts[i] = 0;
            }
        }

        public void SetCurrency1Amount(string amount)
        {
            currencyAmounts[0] = int.Parse(amount);
        }

        public void SetCurrency2Amount(string amount)
        {
            currencyAmounts[1] = int.Parse(amount);
        }

        public void SetCurrency3Amount(string amount)
        {
            currencyAmounts[2] = int.Parse(amount);
        }

        void CheckPlayerHasCurrency()
        {

        }

        public void GetCurrencyAmount(out int currencyID, out long currencyAmount)
        {
            //currencyID = 1;
            //currencyAmount = 1;
            List<Vector2> currencies = new List<Vector2>();
            for (int i = 0; i < currencyIDs.Count; i++)
            {
                currencies.Add(new Vector2(currencyIDs[i], currencyAmounts[i]));
            }
            Inventory.Instance.ConvertCurrenciesToBaseCurrency(currencies, out currencyID, out currencyAmount);
        }
    }
}