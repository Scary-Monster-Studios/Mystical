using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Atavism
{
    public class BuildObjectItemEntry
    {
        public BuildObjectItemEntry(int itemId, int count)
        {
            this.itemId = itemId;
            this.count = count;
        }

        public int itemId;
        public int count = 1;
        public string itemSearch = "";
    }

    public class BuildObjectStage : DataStructure
    {

        public BuildObjectStage() : this(-1, "", 0, -1)
        {

        }

        public BuildObjectStage(int entryID, string gameObject, int buildTimeReq, int nextStage)
        {
            this.id = entryID;
            this.gameObject = gameObject;
            this.buildTimeReq = buildTimeReq;
            this.nextStage = nextStage;

            fields = new Dictionary<string, string>() {
            {"gameObject", "string"},
            {"buildTimeReq", "float"},
            {"nextStage", "int"},
            {"itemReq1", "int"},
            {"itemReq1Count", "int"},
            {"itemReq2", "int"},
            {"itemReq2Count", "int"},
            {"itemReq3", "int"},
            {"itemReq3Count", "int"},
            {"itemReq4", "int"},
            {"itemReq4Count", "int"},
            {"itemReq5", "int"},
            {"itemReq5Count", "int"},
            {"itemReq6", "int"},
            {"itemReq6Count", "int"}
        };
        }

        public string gameObject = "";
        public float buildTimeReq = 0f;
        public int nextStage = -1;
        public int maxEntries = 6;
        public List<BuildObjectItemEntry> entries = new List<BuildObjectItemEntry>();

        public BuildObjectStage Clone()
        {
            return (BuildObjectStage)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "gameObject":
                    return gameObject;
                case "buildTimeReq":
                    return buildTimeReq.ToString();
                case "nextStage":
                    return nextStage.ToString();
                case "itemReq1":
                    return entries[0].itemId.ToString();
                case "itemReq1Count":
                    return entries[0].count.ToString();
                case "itemReq2":
                    return entries[1].itemId.ToString();
                case "itemReq2Count":
                    return entries[1].count.ToString();
                case "itemReq3":
                    return entries[2].itemId.ToString();
                case "itemReq3Count":
                    return entries[2].count.ToString();
                case "itemReq4":
                    return entries[3].itemId.ToString();
                case "itemReq4Count":
                    return entries[3].count.ToString();
                case "itemReq5":
                    return entries[4].itemId.ToString();
                case "itemReq5Count":
                    return entries[4].count.ToString();
                case "itemReq6":
                    return entries[5].itemId.ToString();
                case "itemReq6Count":
                    return entries[5].count.ToString();

            }
            return "";
        }
    }

    public class BuildObjectData : DataStructure
    {
                                            // General Parameters
        public string icon = "";

        public int category = 0;
        public int skill = -1;
        public int skillLevelReq = 0;
        public string weaponReq = "";
        public float distanceReq = 2f;
        public bool buildTaskReqPlayer = true;
        public int validClaimType = 1;
        public int firstStageID = 0;
        public bool availableFromItemOnly = false;
        public string interactionType = "";
        public int interactionID = -1;
        public string interactionData1 = "";
        public List<BuildObjectStage> stages = new List<BuildObjectStage>();
        public List<int> stagesToBeDeleted = new List<int>();

        public BuildObjectData()
        {
            // Database fields
            fields = new Dictionary<string, string>() {
        {"name", "string"},
        {"icon", "string"},
        {"category", "int"},
        {"skill", "int"},
        {"skillLevelReq", "int"},
        {"weaponReq", "string"},
        {"distanceReq", "float"},
        {"buildTaskReqPlayer", "bool"},
        {"validClaimType", "int"},
        {"firstStageID", "int"},
        {"availableFromItemOnly", "bool"},
        {"interactionType", "string"},
        {"interactionID", "int"},
        {"interactionData1", "string"},
                {"icon2","string" },
    };
        }

        public BuildObjectData Clone()
        {
            return (BuildObjectData)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "name":
                    return Name;
                case "icon":
                    return icon;
                case "category":
                    return category.ToString();
                case "skill":
                    return skill.ToString();
                case "skillLevelReq":
                    return skillLevelReq.ToString();
                case "weaponReq":
                    return weaponReq;
                case "distanceReq":
                    return distanceReq.ToString();
                case "buildTaskReqPlayer":
                    return buildTaskReqPlayer.ToString();
                case "validClaimType":
                    return validClaimType.ToString();
                case "firstStageID":
                    return firstStageID.ToString();
                case "availableFromItemOnly":
                    return availableFromItemOnly.ToString();
                case "interactionType":
                    return interactionType;
                case "interactionID":
                    return interactionID.ToString();
                case "interactionData1":
                    return interactionData1;
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