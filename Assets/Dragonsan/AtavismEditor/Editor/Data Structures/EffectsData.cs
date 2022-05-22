using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using B83.Image.BMP;
using System.IO;

// Structure of an Atavism Effect
/*
/* Table structure for tables related to effects
/*

CREATE TABLE `effects` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `displayName` varchar(64) DEFAULT NULL,
  `icon` varchar(64) DEFAULT NULL,
  `effectType` varchar(64) DEFAULT NULL,
  `effectFamily` int(11) DEFAULT NULL,
  `isBuff` tinyint(1) NOT NULL DEFAULT '0',
  `skillType` int(11) DEFAULT NULL,
  `passive` tinyint(1) DEFAULT NULL,
  `stackLimit` int(11) DEFAULT NULL,
  `allowMultiple` tinyint(1) DEFAULT NULL,
  `duration` int(11) DEFAULT NULL,
  `numPulses` int(11) DEFAULT NULL,
  `tooltip` varchar(255) DEFAULT NULL,
  `bonusEffectReq` int(11) DEFAULT NULL,
  `bonusEffectReqConsumed` tinyint(1) DEFAULT NULL,
  `bonusEffect` int(11) NOT NULL DEFAULT '-1',
  `pulseParticle` varchar(32) DEFAULT NULL,
    
*/
namespace Atavism
{
    public class EffectsData : DataStructure
    {
                                            // General Parameters
     
        //public string displayName = "";
        public string icon = "";
        public string effectType = "";
        public string effectMainType = "";
        // public int effectFamily = 0;
        public bool isBuff = false;
        public int skillType = 0;
        public float skillLevelMod = 0;
        public bool passive = false;
        public int stackLimit = 1;
        public bool stackTime = false;
        public bool allowMultiple = false;
        public float duration = 0;
        public int pulseCount = 1;
        public string tooltip = "";
        public int bonusEffectReq = 0;
        public bool bonusEffectReqConsumed = false;
        public int bonusEffect = 0;
        public bool removeBonusWhenEffectRemoved = false;
        public string pulseCoordEffect = "";

        public int intValue1 = 0;
        public int intValue2 = 0;
        public int intValue3 = 0;
        public int intValue4 = 0;
        public int intValue5 = 0;

        public float floatValue1 = 0f;
        public float floatValue2 = 0f;
        public float floatValue3 = 0f;
        public float floatValue4 = 0f;
        public float floatValue5 = 0f;

        public string stringValue1 = "";
        public string stringValue2 = "";
        public string stringValue3 = "";
        public string stringValue4 = "";
        public string stringValue5 = "";

        public bool boolValue1 = false;
        public bool boolValue2 = false;
        public bool boolValue3 = false;
        public bool boolValue4 = false;
        public bool boolValue5 = false;

        public string stringValueSearch1 = "";
        public string stringValueSearch2 = "";
        public string stringValueSearch3 = "";
        public string stringValueSearch4 = "";
        public string stringValueSearch5 = "";

        public bool interruptionAll = false;
        public float interruption_chance = 0f;
        public float interruption_chance_max = 0f;


        public List<ServerEffectType> effectTypes = new List<ServerEffectType>();
        public ServerEffectType effectClass = null;

        // Used locally to determine if a prefab should be created
        public bool createPrefab = false;

        public List<int> tags = new List<int>();
        public List<string> tagSearch = new List<string>();

        public EffectsData()
        {
            Name = "name";
            // Database fields
            fields = new Dictionary<string, string>() {
        {"name", "string"},
		//{"displayName", "string"},
		{"icon", "string"},
        {"effectMainType", "string"},
        {"effectType", "string"},
		//{"effectFamily", "int"},
		{"isBuff", "bool"},
        {"skillType", "int"},
        {"skillLevelMod", "float"},
        {"passive", "bool"},
        {"stackLimit", "int"},
        {"allowMultiple", "bool"},
        {"duration", "float"},
        {"pulseCount", "int"},
        {"tooltip", "string"},
        {"bonusEffectReq", "int"},
        {"bonusEffectReqConsumed", "bool"},
        {"bonusEffect", "int"},
        {"removeBonusWhenEffectRemoved", "bool"},
        {"pulseCoordEffect", "string"},
        {"intValue1", "int"},
        {"intValue2", "int"},
        {"intValue3", "int"},
        {"intValue4", "int"},
        {"intValue5", "int"},
        {"floatValue1", "float"},
        {"floatValue2", "float"},
        {"floatValue3", "float"},
        {"floatValue4", "float"},
        {"floatValue5", "float"},
        {"stringValue1", "string"},
        {"stringValue2", "string"},
        {"stringValue3", "string"},
        {"stringValue4", "string"},
        {"stringValue5", "string"},
        {"boolValue1", "bool"},
        {"boolValue2", "bool"},
        {"boolValue3", "bool"},
        {"boolValue4", "bool"},
        {"boolValue5", "bool"},
        {"icon2","string" },
        {"group_tags","string" },
        {"interruption_all", "bool"},
        {"interruption_chance", "float"},
        {"interruption_chance_max", "float"},
                {"stackTime","bool" },
        };

            // Add Effect type classes here
            effectTypes.Add(new ServerDamageEffects());
            effectTypes.Add(new ServerRestoreEffects());
            effectTypes.Add(new ServerReviveEffects());
            effectTypes.Add(new ServerStatEffects());
            effectTypes.Add(new ServerStunEffects());
            effectTypes.Add(new ServerSleepEffects());
            effectTypes.Add(new ServerImmuneEffects());
            effectTypes.Add(new ServerMorphEffects());
            //effectTypes.Add(new ServerPropertyEffects());
            effectTypes.Add(new ServerDispelEffect());
            effectTypes.Add(new ServerTeleportEffects());
            effectTypes.Add(new ServerMountEffects());
            effectTypes.Add(new ServerBuildObjectEffects());
            effectTypes.Add(new ServerTeachAbilityEffects());
            effectTypes.Add(new ServerTeachSkillEffects());
            effectTypes.Add(new ServerTaskEffects());
            effectTypes.Add(new ServerStateEffects());
            effectTypes.Add(new ServerThreatEffects());
            effectTypes.Add(new ServerCreateItemEffects());
            effectTypes.Add(new ServerCreateItemFromLootEffects());
            effectTypes.Add(new ServerSpawnEffect());
            effectTypes.Add(new ServerSetRespawnLocationEffects());
            effectTypes.Add(new ServerVipEffects());
            effectTypes.Add(new ServerBonusesEffects());
            effectTypes.Add(new ServerTrapEffects());
            effectTypes.Add(new ServerStealthEffects());
            effectTypes.Add(new ServerShieldEffects());
            effectTypes.Add(new ServerTriggerEffects());
        }

        public EffectsData Clone()
        {
            return (EffectsData)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "id":
                    return id.ToString();
                case "name":
                    return Name;
                //case "displayName":
                //	return displayName;
                case "icon":
                    return icon;
                case "effectMainType":
                    return effectMainType;
                case "effectType":
                    return effectType;
                //case "effectFamily":
                //	return effectFamily.ToString();
                case "isBuff":
                    return isBuff.ToString();
                case "skillType":
                    return skillType.ToString();
                case "skillLevelMod":
                    return skillLevelMod.ToString();
                case "passive":
                    return passive.ToString();
                case "stackLimit":
                    return stackLimit.ToString();
                case "stackTime":
                    return stackTime.ToString();
                case "allowMultiple":
                    return allowMultiple.ToString();
                case "duration":
                    return duration.ToString();
                case "pulseCount":
                    return pulseCount.ToString();
                case "tooltip":
                    return tooltip.ToString();
                case "bonusEffectReq":
                    return bonusEffectReq.ToString();
                case "bonusEffectReqConsumed":
                    return bonusEffectReqConsumed.ToString();
                case "bonusEffect":
                    return bonusEffect.ToString();
                case "removeBonusWhenEffectRemoved":
                    return removeBonusWhenEffectRemoved.ToString();
                case "pulseCoordEffect":
                    return pulseCoordEffect;
                case "intValue1":
                    return intValue1.ToString();
                case "intValue2":
                    return intValue2.ToString();
                case "intValue3":
                    return intValue3.ToString();
                case "intValue4":
                    return intValue4.ToString();
                case "intValue5":
                    return intValue5.ToString();
                case "floatValue1":
                    return floatValue1.ToString();
                case "floatValue2":
                    return floatValue2.ToString();
                case "floatValue3":
                    return floatValue3.ToString();
                case "floatValue4":
                    return floatValue4.ToString();
                case "floatValue5":
                    return floatValue5.ToString();
                case "stringValue1":
                    return stringValue1;
                case "stringValue2":
                    return stringValue2;
                case "stringValue3":
                    return stringValue3;
                case "stringValue4":
                    return stringValue4;
                case "stringValue5":
                    return stringValue5;
                case "boolValue1":
                    return boolValue1.ToString();
                case "boolValue2":
                    return boolValue2.ToString();
                case "boolValue3":
                    return boolValue3.ToString();
                case "boolValue4":
                    return boolValue4.ToString();
                case "boolValue5":
                    return boolValue5.ToString();
                case "group_tags":
                    return string.Join(";", tags.ConvertAll(i => i.ToString()).ToArray());
                case "interruption_all":
                    return interruptionAll.ToString();
                case "interruption_chance":
                    return interruption_chance.ToString();
                case "interruption_chance_max":
                    return interruption_chance_max.ToString();
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