using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

// Structure of a Atavism Skills
/*
/* Table structure for table `skills`
/*

CREATE TABLE `skills` (
  `id` int(11) NOT NULL,
  `name` varchar(64) NOT NULL,
  `aspect` int(11) DEFAULT NULL,
  `oppositeAspect` int(11) DEFAULT NULL,
  `primaryStat` int(11) NOT NULL,
  `secondaryStat` int(11) NOT NULL,
  `thirdStat` int(11) NOT NULL,
  `fourthStat` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

*/
namespace Atavism
{
    public class SkillAbilityEntry : DataStructure
    {
        public SkillAbilityEntry() : this(1, -1, -1, true)
        {
        }

        public SkillAbilityEntry(int skillLevelReq, int abilityID) : this(skillLevelReq, abilityID, -1, true)
        {
        }

        public SkillAbilityEntry(int skillLevelReq, int abilityID, int entryID, bool autoLearn)
        {
            this.id = entryID;
            this.skillLevelReq = skillLevelReq;
            this.abilityID = abilityID;
            this.automaticallyLearn = autoLearn;

            fields = new Dictionary<string, string>() {
            {"skillID", "int"},
            {"skillLevelReq", "int"},
            {"abilityID", "int"},
            {"automaticallyLearn", "bool"},
        };
        }

        public int skillID;
        public int skillLevelReq = 1;
        public int abilityID;
        public bool automaticallyLearn = true;
        public string abilitySearch = "";

        public SkillAbilityEntry Clone()
        {
            return (SkillAbilityEntry)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            if (fieldKey == "id")
            {
                return id.ToString();
            }
            else if (fieldKey == "skillID")
            {
                return skillID.ToString();
            }
            else if (fieldKey == "skillLevelReq")
            {
                return skillLevelReq.ToString();
            }
            else if (fieldKey == "abilityID")
            {
                return abilityID.ToString();
            }
            else if (fieldKey == "automaticallyLearn")
            {
                return automaticallyLearn.ToString();
            }
            return "";
        }
    }

    public class SkillsData : DataStructure
    {
           public string icon = "";            // The ability icon

        public int aspect;                  // Id of the aspect this skill belongs to
        public int type = 1;
        public int oppositeAspect;          // Id of the opposite aspect for this skill
        public bool mainAspectOnly = false;
        public bool talent = false;
        public string primaryStat;          // Stat that gets the most gains from this skill
        public string secondaryStat;        // Stat that gets a high amount of gain from this skill
        public string thirdStat;            // Stat that gets a medium amount of gain from this skill
        public string fourthStat;           // Stat that gets a low amount of gain from this skill

        public int maxLevel = 1;
        public bool automaticallyLearn = true;
        public int skillPointCost = 0;
        public int parentSkill = -1;
        public int parentSkillLevelReq = 0;
        public int prereqSkill1 = -1;
        public int prereqSkill1Level = 0;
        public int prereqSkill2 = -1;
        public int prereqSkill2Level = 0;
        public int prereqSkill3 = -1;
        public int prereqSkill3Level = 0;
        public int playerLevelReq = 0;
        public int skillProfile = -1;
        public List<SkillAbilityEntry> skillAbilities = new List<SkillAbilityEntry>();
        public List<int> abilitiesToBeDeleted = new List<int>();

        public SkillsData()
        {
            id = 1;
            // Database fields
            fields = new Dictionary<string, string>() {
            {"name", "string"},
            {"icon", "string"},
            {"aspect", "int"},
           {"type", "int"},
            {"oppositeAspect", "int"},
            {"mainAspectOnly", "bool"},
            {"primaryStat", "string"},
            {"secondaryStat", "string"},
            {"thirdStat", "string"},
            {"fourthStat", "string"},
            {"maxLevel", "int"},
            {"automaticallyLearn", "bool"},
            {"skillPointCost", "int"},
            {"parentSkill", "int"},
            {"parentSkillLevelReq", "int"},
            {"prereqSkill1", "int"},
            {"prereqSkill1Level", "int"},
            {"prereqSkill2", "int"},
            {"prereqSkill2Level", "int"},
            {"prereqSkill3", "int"},
            {"prereqSkill3Level", "int"},
            {"playerLevelReq", "int"},
            {"skill_profile_id", "int"},
            {"talent", "bool"},
               {"icon2", "string"},
        };

        }

        public SkillsData Clone()
        {
            return (SkillsData)this.MemberwiseClone();
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
                case "aspect":
                    return aspect.ToString();
                case "type":
                    return type.ToString();
                case "oppositeAspect":
                    return oppositeAspect.ToString();
                case "mainAspectOnly":
                    return mainAspectOnly.ToString();
                case "primaryStat":
                    return primaryStat;
                case "secondaryStat":
                    return secondaryStat;
                case "thirdStat":
                    return thirdStat;
                case "fourthStat":
                    return fourthStat;
                case "maxLevel":
                    return maxLevel.ToString();
                case "automaticallyLearn":
                    return automaticallyLearn.ToString();
                case "skillPointCost":
                    return skillPointCost.ToString();
                case "parentSkill":
                    return parentSkill.ToString();
                case "parentSkillLevelReq":
                    return parentSkillLevelReq.ToString();
                case "prereqSkill1":
                    return prereqSkill1.ToString();
                case "prereqSkill1Level":
                    return prereqSkill1Level.ToString();
                case "prereqSkill2":
                    return prereqSkill2.ToString();
                case "prereqSkill2Level":
                    return prereqSkill2Level.ToString();
                case "prereqSkill3":
                    return prereqSkill3.ToString();
                case "prereqSkill3Level":
                    return prereqSkill3Level.ToString();
                case "playerLevelReq":
                    return playerLevelReq.ToString();
                case "skill_profile_id":
                    return skillProfile.ToString();
                case "talent":
                    return talent.ToString();
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