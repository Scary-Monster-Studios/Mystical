using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
//using System.Drawing.Image;
using B83.Image.BMP;

namespace Atavism
{

    public class AtavismSaveIcons : EditorWindow
    {
        [MenuItem("Window/Atavism/Atavism Icon Saver")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(AtavismSaveIcons));
        }
        int size = 64;
        bool overide = false;
        public void OnGUI()
        {
           overide = EditorGUILayout.Toggle("Overide Icon size",overide);
            if(overide)
            size = EditorGUILayout.IntField("New Icon Size", size);

            if (GUILayout.Button("Save Icons to database"))
            {
                updateIcons();
            }
        }

        void updateIcons()
        {
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            // string query2 = "SELECT action FROM server";
            string[] tables = new string[] { "skills", "currencies", "item_templates", "abilities", "effects", "build_object_template", "crafting_recipes" };
            // If there is a row, clear it.
            foreach (string table in tables)
            {
                string query = "SELECT id, icon FROM " + table + " WHERE isactive = 1";

                if (rows != null)
                    rows.Clear();
                // Load data
                rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
                // Read all the data
                if ((rows != null) && (rows.Count > 0))
                {
                    foreach (Dictionary<string, string> data in rows)
                    {
                        int id = int.Parse(data["id"]);
                        string icon = data["icon"];
                        string icon2 = "";

                        if(overide)
                            icon2 = getIcon2(icon);
                        else
                            icon2 = getIcon(icon);
                        Debug.Log("In " + table + " Icon " + icon + " form id " + id + " Length="+icon2.Length);

                        if (icon2.Length > 250000)
                        {
                            Debug.LogError("In " + table + " Icon "+ icon + " form id " + id + " is not optimized and will consume some of your server memory. Consider optimizing its size by reducing its resolution.");
                        }else if (icon2.Length > 50000)
                        {
                            Debug.LogWarning("In " + table + " Icon " + icon + " form id " + id + " is not optimized and will consume some of your server memory. Consider optimizing its size by reducing its resolution.");
                        }

                        string queryU = "UPDATE " + table + " SET icon2=?icon2, updatetimestamp=?updatetimestamp WHERE id=?id";

                        // Setup the register data		
                        List<Register> update = new List<Register>();

                        update.Add(new Register("id", "?id", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                        update.Add(new Register("icon2", "?icon2", MySqlDbType.String, icon2, Register.TypesOfField.String));
                        update.Add(new Register("updatetimestamp", "?updatetimestamp", MySqlDbType.String, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Register.TypesOfField.String));

                        DatabasePack.Update(DatabasePack.contentDatabasePrefix, queryU, update);
                    }
                }
                Debug.Log("Done for " + table);
            }
            
              string query1 = "SELECT id, icon FROM character_create_gender WHERE isactive = 1";

                if (rows != null)
                    rows.Clear();
                // Load data
                rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query1);
                // Read all the data
                if ((rows != null) && (rows.Count > 0))
                {
                    foreach (Dictionary<string, string> data in rows)
                    {
                        int id = int.Parse(data["id"]);
                        string icon = data["icon"];
                        string icon2 = "";

                        if(overide)
                            icon2 = getIcon2(icon);
                        else
                            icon2 = getIcon(icon);
                        Debug.Log("In character_create_gender Icon " + icon + " form id " + id + " Length="+icon2.Length);

                        if (icon2.Length > 250000)
                        {
                            Debug.LogError("In  character_create_gender Icon "+ icon + " form id " + id + " is not optimized and will consume some of your server memory. Consider optimizing its size by reducing its resolution. ");
                        }else if (icon2.Length > 50000)
                        {
                            Debug.LogWarning("In character_create_gender Icon " + icon + " form id " + id + " is not optimized and will consume some of your server memory. Consider optimizing its size by reducing its resolution.");
                        }

                        string queryU = "UPDATE character_create_gender SET icon2=?icon2, updatetimestamp=?updatetimestamp WHERE id=?id";

                        // Setup the register data		
                        List<Register> update = new List<Register>();

                        update.Add(new Register("id", "?id", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                        update.Add(new Register("icon2", "?icon2", MySqlDbType.String, icon2, Register.TypesOfField.String));
                        update.Add(new Register("updatetimestamp", "?updatetimestamp", MySqlDbType.String, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Register.TypesOfField.String));

                        DatabasePack.Update(DatabasePack.contentDatabasePrefix, queryU, update);
                    }
                }
            
              query1 = "SELECT id, race_icon, class_icon FROM character_create_template WHERE isactive = 1";

                if (rows != null)
                    rows.Clear();
                // Load data
                rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query1);
                // Read all the data
                if ((rows != null) && (rows.Count > 0))
                {
                    foreach (Dictionary<string, string> data in rows)
                    {
                        int id = int.Parse(data["id"]);
                        string icon = data["race_icon"];
                        string icon2 = "";

                        if(overide)
                            icon2 = getIcon2(icon);
                        else
                            icon2 = getIcon(icon);
                        Debug.Log("In character_create_gender Icon " + icon + " form id " + id + " Length="+icon2.Length);

                        if (icon2.Length > 250000)
                        {
                            Debug.LogError("In  character_create_gender Icon "+ icon + " form id " + id + " is not optimized and will consume some of your server memory. Consider optimizing its size by reducing its resolution.");
                        }else if (icon2.Length > 50000)
                        {
                            Debug.LogWarning("In character_create_gender Icon " + icon + " form id " + id + " is not optimized and will consume some of your server memory. Consider optimizing its size by reducing its resolution.");
                        }

                        string queryU = "UPDATE character_create_template SET race_icon2=?race_icon2, updatetimestamp=?updatetimestamp WHERE id=?id";

                        // Setup the register data		
                        List<Register> update = new List<Register>();

                        update.Add(new Register("id", "?id", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                        update.Add(new Register("race_icon2", "?race_icon2", MySqlDbType.String, icon2, Register.TypesOfField.String));
                        update.Add(new Register("updatetimestamp", "?updatetimestamp", MySqlDbType.String, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Register.TypesOfField.String));

                        DatabasePack.Update(DatabasePack.contentDatabasePrefix, queryU, update);
                        
                        icon = data["class_icon"];
                         icon2 = "";

                        if(overide)
                            icon2 = getIcon2(icon);
                        else
                            icon2 = getIcon(icon);
                        Debug.Log("In character_create_gender Icon " + icon + " form id " + id + " Length="+icon2.Length);

                        if (icon2.Length > 250000)
                        {
                            Debug.LogError("In  character_create_gender Icon "+ icon + " form id " + id + " is not optimized and will consume some of your server memory. Consider optimizing its size by reducing its resolution.");
                        }else if (icon2.Length > 50000)
                        {
                            Debug.LogWarning("In character_create_gender Icon " + icon + " form id " + id + " is not optimized and will consume some of your server memory. Consider optimizing its size by reducing its resolution.");
                        }

                         queryU = "UPDATE character_create_template SET class_icon2=?class_icon2, updatetimestamp=?updatetimestamp WHERE id=?id";

                        // Setup the register data		
                        update = new List<Register>();

                        update.Add(new Register("id", "?id", MySqlDbType.Int32, id.ToString(), Register.TypesOfField.Int));
                        update.Add(new Register("class_icon2", "?class_icon2", MySqlDbType.String, icon2, Register.TypesOfField.String));
                        update.Add(new Register("updatetimestamp", "?updatetimestamp", MySqlDbType.String, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Register.TypesOfField.String));

                        DatabasePack.Update(DatabasePack.contentDatabasePrefix, queryU, update);
                        
                        
                        
                    }
                }
            
            
            
            
            
            Debug.Log("Done");
        }

        string getIcon(string icon)
        {
            Debug.Log("getIcon");
            Sprite sicon = (Sprite)AssetDatabase.LoadAssetAtPath(icon, typeof(Sprite));
            if (System.IO.File.Exists(icon))
            {
                byte[] fileData = System.IO.File.ReadAllBytes(icon);

                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
                byte[] b = tex.EncodeToPNG();
                if (tex.width > sicon.texture.width && tex.height > sicon.texture.height)
                {
                    Texture2D result = new Texture2D(sicon.texture.width, sicon.texture.height, tex.format, true);
                    Color[] rpixels = tex.GetPixels(0);
                    Color[] rezpixel = new Color[(sicon.texture.width * sicon.texture.height)];
                    float incX = ((float)1 / tex.width) * ((float)tex.width / sicon.texture.width);
                    float incY = ((float)1 / tex.height) * ((float)tex.height / sicon.texture.height);
                    // Debug.LogError("TestImage: rpixels=" + rpixels.Length + " incX=" + incX + " incY=" + incY);
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
        string getIcon2(string icon)
        {
            Debug.Log("getIcon2");
            //  Sprite sicon = (Sprite)AssetDatabase.LoadAssetAtPath(icon, typeof(Sprite));
            if (System.IO.File.Exists(icon))
            {
                byte[] fileData = System.IO.File.ReadAllBytes(icon);
                Texture2D tex = new Texture2D(2, 2);
                int width = 0;
                int height = 0;
                if (icon.EndsWith(".BMP") || icon.EndsWith(".bmp"))
                {
                    BMPLoader bmpLoader = new BMPLoader();
                    BMPImage bmpImg = bmpLoader.LoadBMP(fileData);
                    tex = bmpImg.ToTexture2D();
                }
                else
                {
                    tex.LoadImage(fileData);
                }
                byte[] b = tex.EncodeToPNG();
                if (tex.width == tex.height)
                {
                    if (tex.width > size || tex.height > size)
                    {
                        Texture2D result = new Texture2D(size, size, tex.format, true);
                        Color[] rpixels = tex.GetPixels(0);
                        Color[] rezpixel = new Color[(size * size)];
                        float incX = ((float)1 / tex.width) * ((float)tex.width / size);
                        float incY = ((float)1 / tex.height) * ((float)tex.height / size);
                        // Debug.LogError("TestImage: rpixels=" + rpixels.Length + " incX=" + incX + " incY=" + incY);
                        for (int px = 0; px < rezpixel.Length; px++)
                        {
                            //   Debug.LogError("TestImage: px=" + px + " X=" + (incX * ((float)px % sicon.texture.width) + " Y=" + (incY * (Mathf.Floor(px / sicon.texture.width))) ));
                            rezpixel[px] = tex.GetPixelBilinear(incX * ((float)px % size),
                                              incY * (Mathf.Floor(px / size)));
                        }
                        // Debug.LogError("TestImage: rpixels=" + rpixels.Length + " incX=" + incX + " incY=" + incY);

                        result.SetPixels(rezpixel, 0);

                        result.Apply();


                        b = result.EncodeToPNG();
                    }
                    return System.Convert.ToBase64String(b);
                }
                else
                {
                    if (tex.width > size || tex.height > size)
                    {
                        if (tex.width > tex.height)
                        {
                            width = size;
                            height = (int)Mathf.Floor(tex.height / (float)(tex.width / size));
                        }
                        else
                        {
                            height = size;
                            width = (int)Mathf.Floor(tex.width / (float)(tex.height / size));
                        }
                     //   Debug.Log("Size h=" + height + " w=" + width + " | th=" + tex.height + " tw=" + tex.width + " | s=" + size);
                        Texture2D result = new Texture2D(width, height, tex.format, true);
                        Color[] rpixels = tex.GetPixels(0);
                        Color[] rezpixel = new Color[(width * height)];
                        float incX = ((float)1 / tex.width) * ((float)tex.width / width);
                        float incY = ((float)1 / tex.height) * ((float)tex.height / height);
                        // Debug.LogError("TestImage: rpixels=" + rpixels.Length + " incX=" + incX + " incY=" + incY);
                        for (int px = 0; px < rezpixel.Length; px++)
                        {
                            //   Debug.LogError("TestImage: px=" + px + " X=" + (incX * ((float)px % sicon.texture.width) + " Y=" + (incY * (Mathf.Floor(px / sicon.texture.width))) ));
                            rezpixel[px] = tex.GetPixelBilinear(incX * ((float)px % width),
                                              incY * (Mathf.Floor(px / width)));
                        }
                        // Debug.LogError("TestImage: rpixels=" + rpixels.Length + " incX=" + incX + " incY=" + incY);
                        result.SetPixels(rezpixel, 0);
                        result.Apply();
                        b = result.EncodeToPNG();
                    }
                    return System.Convert.ToBase64String(b);
                }
            }
            return "";
        }

    }
}