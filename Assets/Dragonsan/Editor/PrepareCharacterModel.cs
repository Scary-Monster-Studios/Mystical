using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Atavism;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class PrepareCharacterModel : EditorWindow
{
    private  Component[] rgoc = null;
    private  bool[] rgocb = null;
    private  GameObject rgo = null;
    private static GameObject[] gos = null;

    //  private string[] modeList = new string[] {"Player", "Mob" };
    // private int mode = 0;
    private int modeController = 0;
    private string[] modeControllerList = new string[] {"Mecanim", "Legacy"};
    private bool assetbundles = false;
    private bool assetbundlesAddressable = false;
    private string[] actionList = new string[] {"Only Add Missing ", "Override Params"};
    private bool action = true;
    private bool mobName = false;
    private bool mobApp = false;
    private bool charCtrl = false;
    private bool mobCtrl = false;
    private bool assetbun = false;
    private bool assetaddr = false;


    [MenuItem("Window/Atavism/Prepare Character Model", false, 10)]
    public static void ShowWindow()
    {
        PrepareCharacterModel ew = (PrepareCharacterModel)  EditorWindow.GetWindow(typeof(PrepareCharacterModel));
        ew.minSize = new Vector2(100, 100);
        getSelected();
    }

    [MenuItem("GameObject/Atavism/Prepare Character Model", false, 10)]
    public static void GoStrat()
    {
        ShowWindow();
    }

    [MenuItem("Assets/Atavism/Prepare Character Model", false, 10)]
    public static void AssetStart()

    {
        ShowWindow();
    }

    static void getSelected()
    {
        gos = Selection.gameObjects;
    }


    void analize()
    {
        mobName = false;
        mobApp = false;
        charCtrl = false;
        mobCtrl = false;
        assetbun = false;
        if (rgoc != null)
        {
            for (int i = 0; i < rgoc.Length; i++)
            {
                if (rgoc[i].GetType() == typeof(AtavismMobName))
                {
                    rgocb[i] = true;
                    mobName = true;
                }

                if (rgoc[i].GetType() == typeof(AtavismMobAppearance))
                {
                    rgocb[i] = true;
                    mobApp = true;
                }

                if (rgoc[i].GetType() == typeof(CharacterController))
                {
                    rgocb[i] = true;
                    charCtrl = true;
                }

                if (modeController == 0)
                {
                    if (rgoc[i].GetType() == typeof(AtavismMecanimMobController3D))
                    {
                        rgocb[i] = true;
                        mobCtrl = true;
                    }

                    if (rgoc[i].GetType() == typeof(AtavismLegacyAnimationMobController3D))
                    {
                        rgocb[i] = false;
                    }
                }
                else
                {
                    if (rgoc[i].GetType() == typeof(AtavismLegacyAnimationMobController3D))
                    {
                        rgocb[i] = true;
                        mobCtrl = true;
                    }

                    if (rgoc[i].GetType() == typeof(AtavismMecanimMobController3D))
                    {
                        rgocb[i] = false;
                    }
                }


                if (assetbundlesAddressable)
                {
                    if (rgoc[i].GetType() == typeof(AtavismGetModel))
                    {
                        rgocb[i] = false;
                    }

                    if (rgoc[i].GetType() == typeof(GetAddressableModel))
                    {
                        rgocb[i] = true;
                        assetaddr = true;
                    }
                }

                if (assetbundles)
                {
                    if (rgoc[i].GetType() == typeof(AtavismGetModel))
                    {
                        rgocb[i] = false;
                    }

                    if (rgoc[i].GetType() == typeof(GetAddressableModel))
                    {
                        rgocb[i] = true;
                        assetbun = true;
                    }
                }
            }
        }

    }


    public void OnGUI()
    {

        var modeController1 = EditorGUILayout.Popup("Animation Controller Type", modeController, modeControllerList);
        if (modeController != modeController1)
        {
            modeController = modeController1;
            analize();
        }

        var assetbundles1 = GUILayout.Toggle(assetbundles, "Asset Bundles");
        if (assetbundles != assetbundles1)
        {
            assetbundles = assetbundles1;
            assetbundlesAddressable = false;
            analize();
        }

        var assetbundlesAddressable1 = GUILayout.Toggle(assetbundlesAddressable, "Addressable");
        if (assetbundlesAddressable != assetbundlesAddressable1)
        {
            assetbundlesAddressable = assetbundlesAddressable1;
            assetbundles = false;

            analize();
        }

    //    action = EditorGUILayout.Popup("Component Action", action, actionList);

     action = GUILayout.Toggle(action, "Override Params From Source");


        GUILayout.Label("");
        var go = EditorGUILayout.ObjectField("Model source", rgo, typeof(GameObject));
        if (rgo != go)
        {
            if (go == null)
            {
                rgoc = null;
                rgocb = new bool[0];
                rgo = null;

            }
            else
            {
                rgo = (GameObject) go;
               rgoc = rgo.GetComponents(typeof(Component));
                rgocb = new bool[rgoc.Length];
            }

            analize();
        }

        GUILayout.Space(5);

        GUILayout.Space(5);


        if (rgoc != null)
        {
            for (int i = 0; i < rgoc.Length; i++)
            {
                if (rgoc[i].GetType() != typeof(Transform))
                {
                    string[] s = rgoc[i].GetType().ToString().Split('.');
                    rgocb[i] = GUILayout.Toggle(rgocb[i], s[s.Length - 1]);
                }
            }

        }

        GUILayout.Space(5);
        if (!mobName)
        {
            GUILayout.Label("Missing on source AtavismMobName ");
        }

        if (!charCtrl)
        {
            GUILayout.Label("Missing on source CharacterController ");
        }

        if (!mobApp)
        {
            GUILayout.Label("Missing on source AtavismMobAppearance ");
        }

        if (!mobCtrl)
        {
            if (modeController == 0)
                GUILayout.Label("Missing on source AtavismMecanimMobController3D ");
            else
                GUILayout.Label("Missing on source AtavismLegacyAnimationMobController3D ");
        }

        if (!assetbun && assetbundles)
        {
            GUILayout.Label("Missing on source AtavismGetModel");
        }

        if (!assetaddr && assetbundlesAddressable)
        {
            GUILayout.Label("Missing on source GetAddressableModel");
        }

        GUILayout.Space(5);
        if (GUILayout.Button("Get selected objects to modify"))
        {
            getSelected();
        }

        if (gos != null)
        {
            for (int i = 0; i < gos.Length; i++)
            {
                gos[i] = (GameObject) EditorGUILayout.ObjectField("", gos[i], typeof(GameObject));
            }
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Prepare"))
        {
            copyComponents();
        }

    }

    private void copyComponents()
    {
        if (gos != null && gos.Length > 0)
        {
            for (int g = 0; g < gos.Length; g++)
            {
                if (rgoc != null && rgoc.Length > 0)
                {
                    for (int i = 0; i < rgoc.Length; i++)
                    {
                        if (rgocb[i])
                        {
                            var c = gos[g].GetComponent(rgoc[i].GetType());
                            if (!action  && c == null)
                            {
                                if (c == null)
                                    c = gos[g].AddComponent(rgoc[i].GetType());
                                GetCopyOf(c, rgoc[i]);
                            }
                            else if (action)
                            {
                                if (c == null)
                                    c = gos[g].AddComponent(rgoc[i].GetType());
                                GetCopyOf(c, rgoc[i]);
                            }
                        }
                    }
                }

                //add missing
                if (!mobName)
                {
                    var o = gos[g].GetComponent<AtavismMobName>();
                    if (o == null)
                    {
                        o = gos[g].AddComponent<AtavismMobName>();
                    }
                }

                if (!charCtrl)
                {
                    var o = gos[g].GetComponent<CharacterController>();
                    if (o == null)
                    {
                        o = gos[g].AddComponent<CharacterController>();
                        o.center = Vector3.one;
                    }
                }

                if (!mobApp)
                {
                    var o = gos[g].GetComponent<AtavismMobAppearance>();
                    if (o == null)
                    {
                        o = gos[g].AddComponent<AtavismMobAppearance>();
                    }
                }

                if (!mobCtrl)
                {
                    if (modeController == 0)
                    {
                        var o = gos[g].GetComponent<AtavismMecanimMobController3D>();
                        if (o == null)
                        {
                            o = gos[g].AddComponent<AtavismMecanimMobController3D>();
                        }
                    }
                    else
                    {
                        var o = gos[g].GetComponent<AtavismLegacyAnimationMobController3D>();
                        if (o == null)
                        {
                            o = gos[g].AddComponent<AtavismLegacyAnimationMobController3D>();
                        }
                    }
                }

                if (!assetbun && assetbundles)
                {
                    if (assetbundlesAddressable)
                    {
                        var o = gos[g].GetComponent<GetAddressableModel>();
                        if (o == null)
                        {
                            o = gos[g].AddComponent<GetAddressableModel>();
                        }
                    }
                    else
                    {
                        var o = gos[g].GetComponent<AtavismGetModel>();
                        if (o == null)
                        {
                            o = gos[g].AddComponent<AtavismGetModel>();
                        }
                    }
                }

            }
        }
    }

    public static T GetCopyOf<T>(T comp, T other) where T : Component
    {
        Type type = comp.GetType();
        Type othersType = other.GetType();
        if (type != othersType)
        {
            return null;
        }

        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default;
        PropertyInfo[] pinfos = type.GetProperties(flags);

        foreach (var pinfo in pinfos)
        {
                if (pinfo.CanWrite)
                {
                 //   if(type == typeof(CharacterController))
                //        Debug.LogError("Property => "+pinfo.Name+" "+pinfo.CanWrite);
                    try
                    {
                        if (pinfo.Name != "name" && pinfo.Name != "hideFlags")
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch
                    {

                    }
                }
        }

        FieldInfo[] finfos = type.GetFields(flags);

        foreach (var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(other));
        }

        return comp as T;
    }
}
