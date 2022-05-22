using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

// Structure of a Loot Table
/*
/* Table structure for tables
/*
CREATE TABLE `loot_tables` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `item1` int(11) NOT NULL DEFAULT '-1',
  `item1count` int(11) DEFAULT NULL,
  `item1chance` int(11) DEFAULT NULL,
  `item2` int(11) NOT NULL DEFAULT '-1',
  `item2count` int(11) DEFAULT NULL,
  `item2chance` int(11) DEFAULT NULL,
  `item3` int(11) NOT NULL DEFAULT '-1',
  `item3count` int(11) DEFAULT NULL,
  `item3chance` int(11) DEFAULT NULL,
  `item4` int(11) NOT NULL DEFAULT '-1',
  `item4count` int(11) DEFAULT NULL,
  `item4chance` int(11) DEFAULT NULL,
  `item5` int(11) NOT NULL DEFAULT '-1',
  `item5count` int(11) DEFAULT NULL,
  `item5chance` int(11) DEFAULT NULL,
  `item6` int(11) NOT NULL DEFAULT '-1',
  `item6count` int(11) DEFAULT NULL,
  `item6chance` int(11) DEFAULT NULL,
  `item7` int(11) NOT NULL DEFAULT '-1',
  `item7count` int(11) DEFAULT NULL,
  `item7chance` int(11) DEFAULT NULL,
  `item8` int(11) NOT NULL DEFAULT '-1',
  `item8count` int(11) DEFAULT NULL,
  `item8chance` int(11) DEFAULT NULL,
  `item9` int(11) NOT NULL DEFAULT '-1',
  `item9count` int(11) DEFAULT NULL,
  `item9chance` int(11) DEFAULT NULL,
  `item10` int(11) NOT NULL DEFAULT '-1',
  `item10count` int(11) DEFAULT NULL,
  `item10chance` int(11) DEFAULT NULL,
  `category` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`),
  KEY `item1` (`item1`)
)
*/
namespace Atavism
{
    public class RecipeComponentEntry
    {
        public RecipeComponentEntry(int itemId, int count)
        {
            this.itemId = itemId;
            this.count = count;
        }

        public int itemId;
        public int count = 1;
    }

    public class CraftingRecipe : DataStructure
    {
                                            // General Parameters
        public string icon = "";
        public int resultItemID = -1;
        public int resultItemCount = 1;
        public int resultItem2ID = -1;
        public int resultItem2Count = 1;
        public int resultItem3ID = -1;
        public int resultItem3Count = 1;
        public int resultItem4ID = -1;
        public int resultItem4Count = 1;
        public float chance = 100;
        public int resultItem5ID = -1;
        public int resultItem5Count = 1;
        public int resultItem6ID = -1;
        public int resultItem6Count = 1;
        public int resultItem7ID = -1;
        public int resultItem7Count = 1;
        public int resultItem8ID = -1;
        public int resultItem8Count = 1;
        public float chance2 = 50;

        public int resultItem9ID = -1;
        public int resultItem9Count = 1;
        public int resultItem10ID = -1;
        public int resultItem10Count = 1;
        public int resultItem11ID = -1;
        public int resultItem11Count = 1;
        public int resultItem12ID = -1;
        public int resultItem12Count = 1;
        public float chance3 = 30;

        public int resultItem13ID = -1;
        public int resultItem13Count = 1;
        public int resultItem14ID = -1;
        public int resultItem14Count = 1;
        public int resultItem15ID = -1;
        public int resultItem15Count = 1;
        public int resultItem16ID = -1;
        public int resultItem16Count = 1;
        public float chance4 = 10;

        public int skillID = -1;
        public int skillLevelReq = 1;
        public string stationReq = "";
        public int recipeItemID = -1;
        public int creationTime = 0;
        public bool layoutReq = true;
        public bool qualityChangeable = false;
        public bool allowDyes = true;
        public bool allowEssences = false;
        public int maxEntries = 16; // 4 rows of 4
        public int crafting_xp = 10;

        public List<RecipeComponentEntry> entries = new List<RecipeComponentEntry>();

        public CraftingRecipe()
        {
            // Database fields
            fields = new Dictionary<string, string>() {
        {"name", "string"},
        {"icon", "string"},
        {"resultItemID", "int"},
        {"resultItemCount", "int"},
        {"resultItem2ID", "int"},
        {"resultItem2Count", "int"},
        {"resultItem3ID", "int"},
        {"resultItem3Count", "int"},
        {"resultItem4ID", "int"},
        {"resultItem4Count", "int"},
        {"chance", "float"},
        {"resultItem5ID", "int"},
        {"resultItem5Count", "int"},
        {"resultItem6ID", "int"},
        {"resultItem6Count", "int"},
        {"resultItem7ID", "int"},
        {"resultItem7Count", "int"},
        {"resultItem8ID", "int"},
        {"resultItem8Count", "int"},
        {"chance2", "float"},
        { "resultItem9ID", "int"},
        {"resultItem9Count", "int"},
        {"resultItem10ID", "int"},
        {"resultItem10Count", "int"},
        {"resultItem11ID", "int"},
        {"resultItem11Count", "int"},
        {"resultItem12ID", "int"},
        {"resultItem12Count", "int"},
        {"chance3", "float"},
        {"resultItem13ID", "int"},
        {"resultItem13Count", "int"},
        {"resultItem14ID", "int"},
        {"resultItem14Count", "int"},
        {"resultItem15ID", "int"},
        {"resultItem15Count", "int"},
        {"resultItem16ID", "int"},
        {"resultItem16Count", "int"},
        {"chance4", "float"},
        {"skillID", "int"},
        {"skillLevelReq", "int"},
        {"stationReq", "string"},
        {"recipeItemID", "int"},
        {"creationTime", "int"},
        {"layoutReq", "bool"},
        {"qualityChangeable", "bool"},
        {"allowDyes", "bool"},
        {"allowEssences", "bool"},
        {"component1", "int"},
        {"component1count", "int"},
        {"component2", "int"},
        {"component2count", "int"},
        {"component3", "int"},
        {"component3count", "int"},
        {"component4", "int"},
        {"component4count", "int"},
        {"component5", "int"},
        {"component5count", "int"},
        {"component6", "int"},
        {"component6count", "int"},
        {"component7", "int"},
        {"component7count", "int"},
        {"component8", "int"},
        {"component8count", "int"},
        {"component9", "int"},
        {"component9count", "int"},
        {"component10", "int"},
        {"component10count", "int"},
        {"component11", "int"},
        {"component11count", "int"},
        {"component12", "int"},
        {"component12count", "int"},
        {"component13", "int"},
        {"component13count", "int"},
        {"component14", "int"},
        {"component14count", "int"},
        {"component15", "int"},
        {"component15count", "int"},
        {"component16", "int"},
        {"component16count", "int"},
        {"crafting_xp", "int" },
                {"icon2","string" },
    };
        }

        public CraftingRecipe Clone()
        {
            return (CraftingRecipe)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "name":
                    return Name;
                case "icon":
                    return icon;
                case "resultItemID":
                    return resultItemID.ToString();
                case "resultItemCount":
                    return resultItemCount.ToString();
                case "resultItem2ID":
                    return resultItem2ID.ToString();
                case "resultItem2Count":
                    return resultItem2Count.ToString();
                case "resultItem3ID":
                    return resultItem3ID.ToString();
                case "resultItem3Count":
                    return resultItem3Count.ToString();
                case "resultItem4ID":
                    return resultItem4ID.ToString();
                case "resultItem4Count":
                    return resultItem4Count.ToString();
                case "chance":
                    return chance.ToString();

                case "resultItem5ID":
                    return resultItem5ID.ToString();
                case "resultItem5Count":
                    return resultItem5Count.ToString();
                case "resultItem6ID":
                    return resultItem6ID.ToString();
                case "resultItem6Count":
                    return resultItem6Count.ToString();
                case "resultItem7ID":
                    return resultItem7ID.ToString();
                case "resultItem7Count":
                    return resultItem7Count.ToString();
                case "resultItem8ID":
                    return resultItem8ID.ToString();
                case "resultItem8Count":
                    return resultItem8Count.ToString();
                case "chance2":
                    return chance2.ToString();

                case "resultItem9ID":
                    return resultItem9ID.ToString();
                case "resultItem9Count":
                    return resultItem9Count.ToString();
                case "resultItem10ID":
                    return resultItem10ID.ToString();
                case "resultItem10Count":
                    return resultItem10Count.ToString();
                case "resultItem11ID":
                    return resultItem11ID.ToString();
                case "resultItem11Count":
                    return resultItem11Count.ToString();
                case "resultItem12ID":
                    return resultItem12ID.ToString();
                case "resultItem12Count":
                    return resultItem12Count.ToString();
                case "chance3":
                    return chance3.ToString();

                case "resultItem13ID":
                    return resultItem13ID.ToString();
                case "resultItem13Count":
                    return resultItem13Count.ToString();
                case "resultItem14ID":
                    return resultItem14ID.ToString();
                case "resultItem14Count":
                    return resultItem14Count.ToString();
                case "resultItem15ID":
                    return resultItem15ID.ToString();
                case "resultItem15Count":
                    return resultItem15Count.ToString();
                case "resultItem16ID":
                    return resultItem16ID.ToString();
                case "resultItem16Count":
                    return resultItem16Count.ToString();
                case "chance4":
                    return chance4.ToString();

                case "skillID":
                    return skillID.ToString();
                case "skillLevelReq":
                    return skillLevelReq.ToString();
                case "stationReq":
                    return stationReq;
                case "recipeItemID":
                    return recipeItemID.ToString();
                case "creationTime":
                    return creationTime.ToString();
                case "layoutReq":
                    return layoutReq.ToString();
                case "qualityChangeable":
                    return qualityChangeable.ToString();
                case "allowDyes":
                    return allowDyes.ToString();
                case "allowEssences":
                    return allowEssences.ToString();
                case "component1":
                    return entries[0].itemId.ToString();
                case "component1count":
                    return entries[0].count.ToString();
                case "component2":
                    return entries[1].itemId.ToString();
                case "component2count":
                    return entries[1].count.ToString();
                case "component3":
                    return entries[2].itemId.ToString();
                case "component3count":
                    return entries[2].count.ToString();
                case "component4":
                    return entries[3].itemId.ToString();
                case "component4count":
                    return entries[3].count.ToString();
                case "component5":
                    return entries[4].itemId.ToString();
                case "component5count":
                    return entries[4].count.ToString();
                case "component6":
                    return entries[5].itemId.ToString();
                case "component6count":
                    return entries[5].count.ToString();
                case "component7":
                    return entries[6].itemId.ToString();
                case "component7count":
                    return entries[6].count.ToString();
                case "component8":
                    return entries[7].itemId.ToString();
                case "component8count":
                    return entries[7].count.ToString();
                case "component9":
                    return entries[8].itemId.ToString();
                case "component9count":
                    return entries[8].count.ToString();
                case "component10":
                    return entries[9].itemId.ToString();
                case "component10count":
                    return entries[9].count.ToString();
                case "component11":
                    return entries[10].itemId.ToString();
                case "component11count":
                    return entries[10].count.ToString();
                case "component12":
                    return entries[11].itemId.ToString();
                case "component12count":
                    return entries[11].count.ToString();
                case "component13":
                    return entries[12].itemId.ToString();
                case "component13count":
                    return entries[12].count.ToString();
                case "component14":
                    return entries[13].itemId.ToString();
                case "component14count":
                    return entries[13].count.ToString();
                case "component15":
                    return entries[14].itemId.ToString();
                case "component15count":
                    return entries[14].count.ToString();
                case "component16":
                    return entries[15].itemId.ToString();
                case "component16count":
                    return entries[15].count.ToString();
                case "crafting_xp":
                    return crafting_xp.ToString();
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