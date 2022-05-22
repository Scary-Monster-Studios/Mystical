using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;


namespace Atavism
{
    public class CurrencyConversionEntry : DataStructure
    {
        public CurrencyConversionEntry() : this(-1, -1, 0)
        {
        }

        public CurrencyConversionEntry(int currencyID, int currencyTo, int amount)
        {
            this.currencyID = currencyID;
            this.currencyToID = currencyTo;
            this.amount = amount;

            fields = new Dictionary<string, string>() {
            {"currencyID", "int"},
            {"currencyToID", "int"},
            {"amount", "int"},
            {"autoConverts", "bool"},
        };
        }

        public int currencyID;
        public int currencyToID;
        public int amount = 1;
        public bool autoConverts = true;

        public CurrencyConversionEntry Clone()
        {
            return (CurrencyConversionEntry)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            if (fieldKey == "id")
            {
                return id.ToString();
            }
            else if (fieldKey == "currencyID")
            {
                return currencyID.ToString();
            }
            else if (fieldKey == "currencyToID")
            {
                return currencyToID.ToString();
            }
            else if (fieldKey == "amount")
            {
                return amount.ToString();
            }
            else if (fieldKey == "autoConverts")
            {
                return autoConverts.ToString();
            }
            return "";
        }
    }

    public class CurrencyData : DataStructure
    {
                                             // General Parameters
          public int category = 1;            //leave this as 1
        public string icon = "";            // The item icon
        public string description = "";     // A description of the currency (optional)
        public long maximum = 999999;        // The maximum amount of the currency a player can have
        public int currencyGroup = 1;
        public int currencyPosition = 0;
        public string[] positionOptions = { "1", "2", "3" };
        public bool external = false;       // Can be modified outside the game (such as being purchased from an online store

        public List<CurrencyConversionEntry> currencyConversion = new List<CurrencyConversionEntry>();

        public List<int> conversionsToBeDeleted = new List<int>();

        public CurrencyData()
        {
            Name = "name";
            // Database fields
            fields = new Dictionary<string, string>() {
            {"name", "string"},
            {"category", "int"},
            {"icon", "string"},
            {"description", "string"},
            {"maximum", "long"},
            {"currencyGroup", "int"},
            {"currencyPosition", "int"},
            {"external", "bool"},
              {"icon2", "string"},
        };
        }

        public CurrencyData Clone()
        {
            return (CurrencyData)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "id":
                    return id.ToString();
                case "name":
                    return Name;
                case "category":
                    return category.ToString();
                case "icon":
                    return icon;
                case "description":
                    return description;
                case "maximum":
                    return maximum.ToString();
                case "currencyGroup":
                    return currencyGroup.ToString();
                case "currencyPosition":
                    return currencyPosition.ToString();
                case "external":
                    return external.ToString();
                case "icon2":
                    Sprite sicon = (Sprite)AssetDatabase.LoadAssetAtPath(icon, typeof(Sprite));
                    if (System.IO.File.Exists(icon))
                    {
                        byte[] fileData = System.IO.File.ReadAllBytes(icon);

                        Texture2D tex = new Texture2D(2, 2);
                        int width = 0;
                        int height = 0;
                        if (icon.EndsWith(".BMP") || icon.EndsWith(".bmp"))
                        {
                            B83.Image.BMP.BMPLoader bmpLoader = new B83.Image.BMP.BMPLoader();
                            B83.Image.BMP.BMPImage bmpImg = bmpLoader.LoadBMP(fileData);
                            tex = bmpImg.ToTexture2D();
                        }
                        else
                        {
                            tex.LoadImage(fileData);
                        }
                        byte[] b = tex.EncodeToPNG();
                        if (tex.width > sicon.texture.width && tex.height > sicon.texture.height)
                        {
                            Texture2D result = new Texture2D(sicon.texture.width, sicon.texture.height, tex.format, true);
                            Color[] rpixels = tex.GetPixels(0);
                            Color[] rezpixel = new Color[(sicon.texture.width * sicon.texture.height)];
                            float incX = ((float)1 / tex.width) * ((float)tex.width / sicon.texture.width);
                            float incY = ((float)1 / tex.height) * ((float)tex.height / sicon.texture.height);
                            Debug.LogError("TestImage: rpixels=" + rpixels.Length + " incX=" + incX + " incY=" + incY);
                            for (int px = 0; px < rezpixel.Length; px++)
                            {
                                //   Debug.LogError("TestImage: px=" + px + " X=" + (incX * ((float)px % sicon.texture.width) + " Y=" + (incY * (Mathf.Floor(px / sicon.texture.width))) ));
                                rezpixel[px] = tex.GetPixelBilinear(incX * ((float)px % sicon.texture.width),
                                                  incY * (Mathf.Floor(px / sicon.texture.width)));
                            }
                            // Debug.LogError("TestImage: rpixels=" + rpixels.Length + " incX=" + incX + " incY=" + incY);

                            result.SetPixels(rezpixel, 0);

                            result.Apply();


                            b = result.EncodeToPNG();
                        }
                        return System.Convert.ToBase64String(b);
                    }
                    return "";
            }
            return "";
        }
    }
}