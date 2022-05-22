using System;
using System.Collections;
using System.Collections.Generic;
using HNGamers.Atavism;
using UnityEngine;
using UnityEngine.UI;
using static HNGamers.Atavism.ModularCustomizationManager;

namespace Atavism
{

    public class UGUICharacterPanel : MonoBehaviour
    {
        bool showing = false;
        //KeyCode key = KeyCode.C;
        void Awake()
        {
            Hide();
        }
        void Update()
        {
            if (Input.GetKeyDown(AtavismSettings.Instance.GetKeySettings().character) && !ClientAPI.UIHasFocus())
                Toggle();
        }
        public void Toggle()
        {
            if (showing)
                Hide();
            else
                Show();
        }
        public void Show()
        {

            showing = true;
            GetComponent<CanvasGroup>().alpha = 1f;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
            AtavismUIUtility.BringToFront(gameObject);
             if (AtavismSettings.Instance.GetCharacterPanelSpawn() != null)
            {
                if(AtavismSettings.Instance.CharacterAvatar!=null)
                    DestroyImmediate(AtavismSettings.Instance.CharacterAvatar);
                ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismMobAppearance>().ResetAttachObject();
            /*  
              AtavismSettings.Instance.CharacterAvatar = Instantiate(ClientAPI.GetPlayerObject().GameObject);

              DestroyImmediate(AtavismSettings.Instance.CharacterAvatar.GetComponent <MobController3D>());
              DestroyImmediate(AtavismSettings.Instance.CharacterAvatar.GetComponent <AtavismNode>());
*/
            
            string prefabName = (string) ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty("model");
            if (prefabName.Contains(".prefab"))
            {
                int resourcePathPos = prefabName.IndexOf("Resources/");
                prefabName = prefabName.Substring(resourcePathPos + 10);
                prefabName = prefabName.Remove(prefabName.Length - 7);
            }
            GameObject prefab = (GameObject)Resources.Load(prefabName);
            AtavismSettings.Instance.CharacterAvatar = (GameObject) Instantiate(prefab, AtavismSettings.Instance.GetCharacterPanelSpawn().position, AtavismSettings.Instance.GetCharacterPanelSpawn().rotation);
            if(ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists("umaData"))
            {	
                Dictionary<string, object> umaDictionary = (Dictionary<string, object>)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty("umaData");
                //   AtavismSettings.Instance.CharacterAvatar.GetComponent<AtavismNode>().AddLocalProperty("umaData",umaDictionary);
                var node = AtavismSettings.Instance.CharacterAvatar.GetComponent<AtavismNode>();
                if (node == null)
                {
                    node = AtavismSettings.Instance.CharacterAvatar.AddComponent<AtavismNode>();
                }
                node.AddLocalProperty("umaData",umaDictionary);
                node.AddLocalProperty("genderId",(int)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty("genderId"));
                node.AddLocalProperty("race",(int)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty("race"));
                node.AddLocalProperty("aspect",(int)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty("aspect"));
                AtavismSettings.Instance.CharacterAvatar.SendMessage("GrabRecipe", SendMessageOptions.DontRequireReceiver);
            }
            var mcm = AtavismSettings.Instance.CharacterAvatar.GetComponent<ModularCustomizationManager>();
            if ( mcm != null)
            {
               
                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.EyeMaterialPropertyName))
                {
                    mcm.UpdateEyeMaterial((int)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.EyeMaterialPropertyName));
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.HairMaterialPropertyName))
                {
                    mcm.UpdateHairMaterial((int)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.HairMaterialPropertyName));
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.SkinMaterialPropertyName))
                {
                    mcm.UpdateSkinMaterial((int)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.SkinMaterialPropertyName));
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.MouthMaterialPropertyName))
                {
                    mcm.UpdateMouthMaterial((int)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.MouthMaterialPropertyName));
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.bodyColorPropertyName))
                {
                    var item = ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.bodyColorPropertyName).ToString().Split(',');
                    Color32 test = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateBodyColor(test);
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.scarColorPropertyName))
                {
                    var item = ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.scarColorPropertyName).ToString().Split(',');
                    Color32 test = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateBodyScarColor(test);
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.hairColorPropertyName))
                {
                    var item = ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.hairColorPropertyName).ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateHairColor(color32);
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.mouthColorPropertyName))
                {
                    var item = ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.mouthColorPropertyName).ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateMouthColor(color32);
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.beardColorPropertyName))
                {
                    var item = ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.beardColorPropertyName).ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateBeardColor(color32);
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.eyeBrowColorPropertyName))
                {
                    var item = ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.eyeBrowColorPropertyName).ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateEyebrowColor(color32);
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.stubbleColorPropertyName))
                {
                    var item = ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.stubbleColorPropertyName).ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateStubbleColor(color32);
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.bodyArtColorPropertyName))
                {
                    var item = ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.bodyArtColorPropertyName).ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateBodyArtColor(color32);
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.eyeColorPropertyName))
                {
                    var item = ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.eyeColorPropertyName).ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateEyeColor(color32);
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.helmetColorPropertyName))
                {
                    var colorProperties = ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.helmetColorPropertyName).ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        mcm.UpdateShaderColor(color32, colorslot, ModularCustomizationManager.BodyType.Head);
                    }
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.torsoColorPropertyName))
                {
                    var colorProperties = ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.torsoColorPropertyName).ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        mcm.UpdateShaderColor(color32, colorslot, BodyType.Torso);
                    }
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.upperArmsColorPropertyName))
                {
                    var colorProperties = ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.upperArmsColorPropertyName).ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        mcm.UpdateShaderColor(color32, colorslot, BodyType.Upperarms);
                    }
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.lowerArmsColorPropertyName))
                {
                    var colorProperties = ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.lowerArmsColorPropertyName).ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        mcm.UpdateShaderColor(color32, colorslot, BodyType.LowerArms);
                    }
                }
                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.hipsColorPropertyName))
                {
                    var colorProperties = ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.hipsColorPropertyName).ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        mcm.UpdateShaderColor(color32, colorslot, BodyType.Hips);
                    }
                }
                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.lowerLegsColorPropertyName))
                {
                    var colorProperties = ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.lowerLegsColorPropertyName).ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        mcm.UpdateShaderColor(color32, colorslot, BodyType.LowerLegs);
                    }
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.feetColorPropertyName))
                {
                    var colorProperties = ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.feetColorPropertyName).ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        mcm.UpdateShaderColor(color32, colorslot, BodyType.Feet);
                    }
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.handsColorPropertyName))
                {
                    var colorProperties = ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.handsColorPropertyName).ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        mcm.UpdateShaderColor(color32, colorslot, BodyType.Hands);
                    }
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.hairPropertyName))
                {
                    mcm.UpdateHairModel((int)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.hairPropertyName));
                }



                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.beardPropertyName))
                {
                    mcm.UpdateBeardModel((int)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.beardPropertyName));
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.eyebrowPropertyName))
                {

                    mcm.UpdateEyebrowModel((int)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.eyebrowPropertyName));
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.headPropertyName))
                {
                    mcm.UpdateBodyModel((string)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.headPropertyName), BodyType.Head);
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.faceTexPropertyName))
                {
                    mcm.UpdateBodyModel((string)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.faceTexPropertyName), BodyType.Face);
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.handsPropertyName))
                {
                    mcm.UpdateBodyModel((string)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.handsPropertyName), BodyType.Hands);
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.lowerArmsPropertyName))
                {
                    mcm.UpdateBodyModel((string)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.lowerArmsPropertyName), BodyType.LowerArms);
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.upperArmsPropertyName))
                {
                    mcm.UpdateBodyModel((string)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.upperArmsPropertyName), BodyType.Upperarms);
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.torsoPropertyName))
                {
                    mcm.UpdateBodyModel((string)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.torsoPropertyName), BodyType.Torso);
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.hipsPropertyName))
                {
                    mcm.UpdateBodyModel((string)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.hipsPropertyName), BodyType.Hips);
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.lowerLegsPropertyName))
                {
                    mcm.UpdateBodyModel((string)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.lowerLegsPropertyName), BodyType.LowerLegs);
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.feetPropertyName))
                {
                    mcm.UpdateBodyModel((string)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.feetPropertyName), BodyType.Feet);
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.earsPropertyName))
                {
                    mcm.UpdateEarModel((int)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.earsPropertyName));
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.eyesPropertyName))
                {
                    mcm.UpdateEyeModel((int)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.eyesPropertyName));
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.tuskPropertyName))
                {
                    mcm.UpdateTuskModel((int)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.tuskPropertyName));
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.mouthPropertyName))
                {
                    mcm.UpdateMouthModel((int)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.mouthPropertyName));
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.faithPropertyName))
                {
                    mcm.SetFaith((string)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.faithPropertyName));
                }
#if IPBRInt
                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.blendshapePresetValue) && (!mcm.enableSavingInfinityPBRBlendshapes))
                {
                    mcm.UpdateBlendShapePresets((int)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.blendshapePresetValue));
                }

                if (ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().PropertyExists(mcm.infinityBlendShapes))
                {
                    mcm.UpdateBlendShapes((string)ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismNode>().GetProperty(mcm.infinityBlendShapes));
                }
#endif

            }
                
            
            
            
            DestroyImmediate(AtavismSettings.Instance.CharacterAvatar.GetComponent <MobController3D>());
            
            
              ClientAPI.GetPlayerObject().GameObject.GetComponent<AtavismMobAppearance>().ReApplyEquipDisplay();
         //     AtavismSettings.Instance.CharacterAvatar.GetComponent<AtavismMobAppearance>().ReApplyEquipDisplay();
    //AtavismSettings.Instance.OtherCharacterAvatar.layer = 24;
                AtavismSettings.Instance.CharacterAvatar.transform.position = AtavismSettings.Instance.GetCharacterPanelSpawn().position;
                AtavismSettings.Instance.CharacterAvatar.transform.rotation = AtavismSettings.Instance.GetCharacterPanelSpawn().rotation;
                AtavismSettings.Instance.GetCharacterPanelCamera().enabled = true;
            }
            //    gameObject.SetActive(true);
        }

        public void Hide()
        {
            //    gameObject.SetActive(false);
            showing = false;
            GetComponent<CanvasGroup>().alpha = 0f;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            if (AtavismSettings.Instance.GetCharacterPanelCamera() != null)
                AtavismSettings.Instance.GetCharacterPanelCamera().enabled = false;

        }


    }
}