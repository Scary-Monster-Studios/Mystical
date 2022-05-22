using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using B83.Image.BMP;

// Structure of a Atavism Item
/*
/* Table structure for table `itemtemplates`
/*

CREATE TABLE `itemtemplates` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `icon` varchar(64) DEFAULT NULL,
  `category` varchar(64) DEFAULT NULL,
  `subcategory` varchar(64) DEFAULT NULL,
  `itemType` varchar(64) DEFAULT NULL,
  `subType` varchar(64) DEFAULT NULL,
  `slot` varchar(64) DEFAULT NULL,
  `display` int(11) DEFAULT NULL,
  `itemQuality` tinyint(11) DEFAULT NULL,
  `binding` tinyint(11) DEFAULT NULL,
  `isUnique` tinyint(1) DEFAULT NULL,
  `stackLimit` int(11) DEFAULT NULL,
  `duration` int(11) DEFAULT NULL,
  `purchaseCurrency` tinyint(11) DEFAULT NULL,
  `purchaseCost` int(11) DEFAULT NULL,
  `sellable` tinyint(1) DEFAULT '1',
  `levelReq` int(11) DEFAULT NULL,
  `aspectReq` varchar(64) DEFAULT NULL,
  `raceReq` varchar(64) DEFAULT NULL,
  `damage` int(11) NOT NULL DEFAULT '0',
  `damageMax` int(11) NOT NULL DEFAULT '0',
  `damageType` varchar(32) DEFAULT NULL,
  `delay` int(11) NOT NULL DEFAULT '0',
  `useAbility` int(11) NOT NULL DEFAULT '-1',
  `clickEffect` varchar(64) DEFAULT NULL,
  `toolTip` varchar(255) DEFAULT NULL,
  `triggerEvent` varchar(32) DEFAULT NULL,
  `triggerAction1Type` varchar(32) DEFAULT NULL,
  `triggerAction1Data` varchar(32) DEFAULT NULL,

*/
namespace Atavism
{
    public class ItemTemplateOptionEntry : DataStructure
    {
        public ItemTemplateOptionEntry() : this(-1, -1, "")
        {
        }

        public ItemTemplateOptionEntry(int itemID, int editor_option_type_id, string editor_option_choice_type_id)
        {
            this.itemID = itemID;
            this.editor_option_type_id = editor_option_type_id;
            this.editor_option_choice_type_id = editor_option_choice_type_id;

            fields = new Dictionary<string, string>() {
            {"item_id", "int"},
            {"editor_option_type_id", "int"},
            {"editor_option_choice_type_id", "string"},
            {"required_value", "int"},
        };
        }

        public int itemID;
        public int editor_option_type_id;
        public string editor_option_choice_type_id = "";
        public int required_value = 1;

        public ItemTemplateOptionEntry Clone()
        {
            return (ItemTemplateOptionEntry)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            if (fieldKey == "id")
            {
                return id.ToString();
            }
            else if (fieldKey == "item_id")
            {
                return itemID.ToString();
            }
            else if (fieldKey == "editor_option_type_id")
            {
                return editor_option_type_id.ToString();
            }
            else if (fieldKey == "editor_option_choice_type_id")
            {
                return editor_option_choice_type_id;
            }
            else if (fieldKey == "required_value")
            {
                return required_value.ToString();
            }
            return "";
        }
    }

    public class ItemEffectEntry
    {
        public ItemEffectEntry(string itemEffectType, string itemEffectName, string itemEffectValue)
        {
            this.itemEffectType = itemEffectType;
            this.itemEffectName = itemEffectName;
            this.itemEffectValue = itemEffectValue;
        }

        public string itemEffectType;
        public string itemEffectName;
        public string itemEffectValue;
    }

    public class ItemData : DataStructure
    {
                                            // General Parameters
        public string icon = "";            // The item icon
        public string category = "0";       // Always set to 0 at the current time
        public string subcategory = "0";    // Always set to 0 at the current time
        public string itemType = "";    // Can be either Weapon, Armor, Consumable, Material, Junk
        public int itemQuality = 1;         // Ranges from 1-6 with the names: Poor, Common, Uncommon, Rare, Epic, Legendary
        public int skillExp = 1;      
                                            //public string[] itemQualityOptions = new string[] {"Poor", "Common", "Uncommon", "Rare", "Epic", "Legendary"};
        public int binding = 0;             // 0=No binding, 1=Binds on Equip, 2=Binds on Pickup
        public string[] bindingOptions = new string[] { "No binding", "Equip", "Pickup" };
        public bool isUnique = false;       // If true, the user can only have 1 of the item
        public int stackLimit = 1;          // How many of the item can be put into 1 stack which only takes up 1 inventory slot
        public int purchaseCurrency = 0;    // What currency is required to buy this item, use 0 at the moment
        public long purchaseCost = 0;        // How much of the currency it costs to buy the item
        public bool sellable = true;        // Can the item be sold to a vendor
        public string toolTip = "";     // Some text about the item (usually for fun)
        public bool auctionHouse = false; //Can be put on auction
                                          // Fields common to weapons and armor
        public string subType = "";     // Sword, Axe, Mace, Staff, Bow, Gun
        public string slot = "";            // Weapon: Main Hand, Off Hand, Two Hand - Armor: Head, Shoulder, Chest, Legs, Hands, Feet, Waist, Back 
        public string[] slotWeaponOptions = new string[] { "Main Hand", "Off Hand", "Two Hand", "Any Hand", "Slot3" };
        public string[] slotArmorOptions = new string[] { "Head", "Shoulder", "Chest", "Off Hand", "Legs", "Hands", "Feet", "Waist", "Back", "Neck", "Shirt", "Ring", "Main Ring", "Off Ring", "Earring", "Main Earring", "Off Earring","Fashion","Slot20" ,"Slot3"};
        public string display = "";         // The id of the equipment display to use (this will need further explaining)

        public int passive_ability = -1;
        // Fields only for weapons
        public int damage = 0;          // How much damage the weapon does
        public int damageMax = 0;          // How much damage the weapon does
        public string damageType = "";      // The type of damage done.
        public float delay = 1.5f;              // How long between attacks when using this weapon in seconds
        public int enchant_profile_id = -1;
        public int maxEffectEntries = 32;
        public int gear_score = 0;

        public string socketType = "";
        public int weight = 0;
        public int durability = 0;
        public int autoattack = -1;
        public int ammotype = 0;
        public bool parry = false;
        public bool oadelete = false;
        public int deathLossChance = 0;

        public List<ItemEffectEntry> effects = new List<ItemEffectEntry>();

        public List<ItemTemplateOptionEntry> itemTemplateOptions = new List<ItemTemplateOptionEntry>();

        public List<int> itemOptionsToBeDeleted = new List<int>();
        public int setId = -1;                  // id of Item Set Profile

        public ItemData()
        {
            Name = "name";
            //  itemQualityOptions = ServerOptionChoices.LoadAtavismChoiceOptions("Item Quality", false);
            // Database fields
            fields = new Dictionary<string, string>() {
        {"name", "string"},
        {"icon", "string"},
        {"category", "string"},
        {"subcategory", "string"},
        {"itemType", "string"},
        {"subType", "string"},
        {"slot", "string"},
        {"display", "string"},
        {"itemQuality", "int"},
        {"binding", "int"},
        {"isUnique", "bool"},
        {"stackLimit", "int"},
        {"purchaseCurrency", "int"},
        {"purchaseCost", "int"},
        {"sellable", "bool"},
        {"damage", "int"},
        {"damageMax", "int"},
        {"damageType", "string"},
        {"delay", "float"},
        {"toolTip", "string"},
        {"effect1type", "string"},
        {"effect1name", "string"},
        {"effect1value", "string"},
        {"effect2type", "string"},
        {"effect2name", "string"},
        {"effect2value", "string"},
        {"effect3type", "string"},
        {"effect3name", "string"},
        {"effect3value", "string"},
        {"effect4type", "string"},
        {"effect4name", "string"},
        {"effect4value", "string"},
        {"effect5type", "string"},
        {"effect5name", "string"},
        {"effect5value", "string"},
        {"effect6type", "string"},
        {"effect6name", "string"},
        {"effect6value", "string"},
        {"effect7type", "string"},
        {"effect7name", "string"},
        {"effect7value", "string"},
        {"effect8type", "string"},
        {"effect8name", "string"},
        {"effect8value", "string"},
        {"effect9type", "string"},
        {"effect9name", "string"},
        {"effect9value", "string"},
        {"effect10type", "string"},
        {"effect10name", "string"},
        {"effect10value", "string"},
        {"effect11type", "string"},
        {"effect11name", "string"},
        {"effect11value", "string"},
        {"effect12type", "string"},
        {"effect12name", "string"},
        {"effect12value", "string"},
        {"effect13type", "string"},
        {"effect13name", "string"},
        {"effect13value", "string"},
        {"effect14type", "string"},
        {"effect14name", "string"},
        {"effect14value", "string"},
        {"effect15type", "string"},
        {"effect15name", "string"},
        {"effect15value", "string"},
        {"effect16type", "string"},
        {"effect16name", "string"},
        {"effect16value", "string"},
        {"effect17type", "string"},
        {"effect17name", "string"},
        {"effect17value", "string"},
        {"effect18type", "string"},
        {"effect18name", "string"},
        {"effect18value", "string"},
        {"effect19type", "string"},
        {"effect19name", "string"},
        {"effect19value", "string"},
        {"effect20type", "string"},
        {"effect20name", "string"},
        {"effect20value", "string"},
        {"effect21type", "string"},
        {"effect21name", "string"},
        {"effect21value", "string"},
        {"effect22type", "string"},
        {"effect22name", "string"},
        {"effect22value", "string"},
        {"effect23type", "string"},
        {"effect23name", "string"},
        {"effect23value", "string"},
        {"effect24type", "string"},
        {"effect24name", "string"},
        {"effect24value", "string"},
        {"effect25type", "string"},
        {"effect25name", "string"},
        {"effect25value", "string"},
        {"effect26type", "string"},
        {"effect26name", "string"},
        {"effect26value", "string"},
        {"effect27type", "string"},
        {"effect27name", "string"},
        {"effect27value", "string"},
        {"effect28type", "string"},
        {"effect28name", "string"},
        {"effect28value", "string"},
        {"effect29type", "string"},
        {"effect29name", "string"},
        {"effect29value", "string"},
        {"effect30type", "string"},
        {"effect30name", "string"},
        {"effect30value", "string"},
        {"effect31type", "string"},
        {"effect31name", "string"},
        {"effect31value", "string"},
        {"effect32type", "string"},
        {"effect32name", "string"},
        {"effect32value", "string"},
        {"enchant_profile_id","int" },
        {"skillExp","int" },
        {"auctionHouse", "bool"},
        {"gear_score","int" },
        {"icon2", "string"},

        { "weight","int" },
        {"durability","int" },
        {"autoattack","int" },
        {"ammotype","int" },
        {"death_loss","int" },
        {"parry","bool" },
       {"oadelete","bool" },
        {"socket_type","string" },
                {"passive_ability","int" },


};
        }

        public ItemData Clone()
        {
            return (ItemData)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "id":
                    return id.ToString();
                case "name":
                    return Name;
                case "icon":
                    return icon;
                case "category":
                    return category;
                case "subcategory":
                    return subcategory;
                case "itemType":
                    return itemType;
                case "subType":
                    return subType;
                case "slot":
                    return slot;
                case "display":
                    return display;
                case "itemQuality":
                    return itemQuality.ToString();
                case "binding":
                    return binding.ToString();
                case "isUnique":
                    return isUnique.ToString();
                case "stackLimit":
                    return stackLimit.ToString();
                case "purchaseCurrency":
                    return purchaseCurrency.ToString();
                case "purchaseCost":
                    return purchaseCost.ToString();
                case "sellable":
                    return sellable.ToString();
                case "damage":
                    return damage.ToString();
                case "damageMax":
                    return damageMax.ToString();
                case "damageType":
                    return damageType;
                case "delay":
                    return delay.ToString();
                case "toolTip":
                    return toolTip;
                case "effect1type":
                    return getEffectData(0, "type");
                case "effect1name":
                    return getEffectData(0, "name");
                case "effect1value":
                    return getEffectData(0, "value");
                case "effect2type":
                    return getEffectData(1, "type");
                case "effect2name":
                    return getEffectData(1, "name");
                case "effect2value":
                    return getEffectData(1, "value");
                case "effect3type":
                    return getEffectData(2, "type");
                case "effect3name":
                    return getEffectData(2, "name");
                case "effect3value":
                    return getEffectData(2, "value");
                case "effect4type":
                    return getEffectData(3, "type");
                case "effect4name":
                    return getEffectData(3, "name");
                case "effect4value":
                    return getEffectData(3, "value");
                case "effect5type":
                    return getEffectData(4, "type");
                case "effect5name":
                    return getEffectData(4, "name");
                case "effect5value":
                    return getEffectData(4, "value");
                case "effect6type":
                    return getEffectData(5, "type");
                case "effect6name":
                    return getEffectData(5, "name");
                case "effect6value":
                    return getEffectData(5, "value");
                case "effect7type":
                    return getEffectData(6, "type");
                case "effect7name":
                    return getEffectData(6, "name");
                case "effect7value":
                    return getEffectData(6, "value");
                case "effect8type":
                    return getEffectData(7, "type");
                case "effect8name":
                    return getEffectData(7, "name");
                case "effect8value":
                    return getEffectData(7, "value");
                case "effect9type":
                    return getEffectData(8, "type");
                case "effect9name":
                    return getEffectData(8, "name");
                case "effect9value":
                    return getEffectData(8, "value");
                case "effect10type":
                    return getEffectData(9, "type");
                case "effect10name":
                    return getEffectData(9, "name");
                case "effect10value":
                    return getEffectData(9, "value");
                case "effect11type":
                    return getEffectData(10, "type");
                case "effect11name":
                    return getEffectData(10, "name");
                case "effect11value":
                    return getEffectData(10, "value");
                case "effect12type":
                    return getEffectData(11, "type");
                case "effect12name":
                    return getEffectData(11, "name");
                case "effect12value":
                    return getEffectData(11, "value");

                case "effect13type":
                    return getEffectData(12, "type");
                case "effect13name":
                    return getEffectData(12, "name");
                case "effect13value":
                    return getEffectData(12, "value");
                case "effect14type":
                    return getEffectData(13, "type");
                case "effect14name":
                    return getEffectData(13, "name");
                case "effect14value":
                    return getEffectData(13, "value");
                case "effect15type":
                    return getEffectData(14, "type");
                case "effect15name":
                    return getEffectData(14, "name");
                case "effect15value":
                    return getEffectData(14, "value");
                case "effect16type":
                    return getEffectData(15, "type");
                case "effect16name":
                    return getEffectData(15, "name");
                case "effect16value":
                    return getEffectData(15, "value");
                case "effect17type":
                    return getEffectData(16, "type");
                case "effect17name":
                    return getEffectData(16, "name");
                case "effect17value":
                    return getEffectData(16, "value");
                case "effect18type":
                    return getEffectData(17, "type");
                case "effect18name":
                    return getEffectData(17, "name");
                case "effect18value":
                    return getEffectData(17, "value");
                case "effect19type":
                    return getEffectData(18, "type");
                case "effect19name":
                    return getEffectData(18, "name");
                case "effect19value":
                    return getEffectData(18, "value");
                case "effect20type":
                    return getEffectData(19, "type");
                case "effect20name":
                    return getEffectData(19, "name");
                case "effect20value":
                    return getEffectData(19, "value");
                case "effect21type":
                    return getEffectData(20, "type");
                case "effect21name":
                    return getEffectData(20, "name");
                case "effect21value":
                    return getEffectData(20, "value");
                case "effect22type":
                    return getEffectData(21, "type");
                case "effect22name":
                    return getEffectData(21, "name");
                case "effect22value":
                    return getEffectData(21, "value");
                case "effect23type":
                    return getEffectData(22, "type");
                case "effect23name":
                    return getEffectData(22, "name");
                case "effect23value":
                    return getEffectData(22, "value");
                case "effect24type":
                    return getEffectData(23, "type");
                case "effect24name":
                    return getEffectData(23, "name");
                case "effect24value":
                    return getEffectData(23, "value");
                case "effect25type":
                    return getEffectData(24, "type");
                case "effect25name":
                    return getEffectData(24, "name");
                case "effect25value":
                    return getEffectData(24, "value");
                case "effect26type":
                    return getEffectData(25, "type");
                case "effect26name":
                    return getEffectData(25, "name");
                case "effect26value":
                    return getEffectData(25, "value");
                case "effect27type":
                    return getEffectData(26, "type");
                case "effect27name":
                    return getEffectData(26, "name");
                case "effect27value":
                    return getEffectData(26, "value");
                case "effect28type":
                    return getEffectData(27, "type");
                case "effect28name":
                    return getEffectData(27, "name");
                case "effect28value":
                    return getEffectData(27, "value");
                case "effect29type":
                    return getEffectData(28, "type");
                case "effect29name":
                    return getEffectData(28, "name");
                case "effect29value":
                    return getEffectData(28, "value");
                case "effect30type":
                    return getEffectData(29, "type");
                case "effect30name":
                    return getEffectData(29, "name");
                case "effect30value":
                    return getEffectData(29, "value");
                case "effect31type":
                    return getEffectData(30, "type");
                case "effect31name":
                    return getEffectData(30, "name");
                case "effect31value":
                    return getEffectData(30, "value");
                case "effect32type":
                    return getEffectData(31, "type");
                case "effect32name":
                    return getEffectData(31, "name");
                case "effect32value":
                    return getEffectData(31, "value");
                case "enchant_profile_id":
                    return enchant_profile_id.ToString();
                case "skillExp":
                    return skillExp.ToString();
                case "auctionHouse":
                    return auctionHouse.ToString();
                case "gear_score":
                    return gear_score.ToString();
                case "weight":
                    return weight.ToString();
                case "durability":
                    return durability.ToString();
                case "autoattack":
                    return autoattack.ToString();
                case "ammotype":
                    return ammotype.ToString();
                case "death_loss":
                    return deathLossChance.ToString();
                case "socket_type":
                    return socketType;
                case "parry":
                    return parry.ToString();
                case "oadelete":
                    return oadelete.ToString();
                case "passive_ability":
                    return passive_ability.ToString();
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

        string getEffectData(int effectNum, string field)
        {
            if (effects.Count > effectNum)
            {
                if (field == "type")
                {
                    return effects[effectNum].itemEffectType;
                }
                else if (field == "name")
                {
                    return effects[effectNum].itemEffectName;
                }
                else if (field == "value")
                {
                    return effects[effectNum].itemEffectValue;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

    }
}