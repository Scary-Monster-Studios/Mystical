/// Legal License https://creativecommons.org/licenses/by-nd/4.0/
/// HNGamers
/// Thevisad
/// 2020

using System.Linq;
#if ModularNHance
using NHance.Assets.Scripts;
#endif

namespace HNGamers
{

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using System.IO;
    using global::Atavism;

    namespace Atavism
    {

        public enum characterGender
        {
            Male,
            Female
        }

        [Serializable]
        public class part
        {
            public string Name;
            public List<GameObject> parts = new List<GameObject>();

        }
        [HelpURL("https://docs.google.com/document/d/1JkZ1PEX4mVGOi4ZhWIKUvKV0CTbJWwB7lXF5j-R-hgQ/edit?usp=sharing")]
        public class ModularCustomizationManager : MonoBehaviour
        {

            [SerializeField]
            [HideInInspector]
            public string currentTab;
            public int toolbarTop;
            public int toolbarMiddle;
            public int toolbarBottom;
            public int toolbarSubBottom;

            [Tooltip("Is this a male or female model.")]
            public characterGender gender;

            [Tooltip("The item that is used as the base prefab replacement item.")]
            public GameObject replacementObject;

            public GameObject headReplacementObject;
            public GameObject torsoReplacementObject;
            public GameObject upperArmsReplacementObject;
            public GameObject lowerArmsReplacementObject;
            public GameObject hipsReplacementObject;
            public GameObject lowerLegsReplacementObject;
            public GameObject feetReplacementObject;
            public GameObject handsReplacementObject;

            public bool modularModelSwapping = false;
#if ModularNHance
            public bool nHanceModelSwapping = false;
            public GameObject defaultTorso;
            public GameObject defaultHands;
            public GameObject defaultFeet;
            public GameObject defaultLegs;
#endif
            [Header("Model Material Section")]
            [Tooltip("Your models material, this is a requirement for the models to work right for certain models to change colors. Simply look at the material that is attached to the original prefab.")]
            public Material bodyMat;

            [Tooltip("Your models material, this is a requirement for the models to work right for certain models to change colors. Simply look at the material that is attached to the original prefab.")]
            public Material headMat;

            [Tooltip("Your models material, this is a requirement for the models to work right for certain models to change colors. Simply look at the material that is attached to the original prefab.")]
            public Material hairMat;

            [Tooltip("Your models material, this is a requirement for the models to work right for certain models to change colors. Simply look at the material that is attached to the original prefab.")]
            public Material eyeMat;

            [Tooltip("Your models material, this is a requirement for the models to work right for certain models to change colors. Simply look at the material that is attached to the original prefab.")]
            public Material mouthMat;

            [Tooltip("This is for a model that has a large number of various materials for the skins. Tafi is a model that has this setup, this helps to sync all the materials on the model as needed..")]
            public List<Material> skinMaterialsToSync = new List<Material>();
            [HideInInspector] public List<Material> skinMaterialsSynced = new List<Material>();


            [Tooltip("This is for a model that has a large number of various materials for the skins. Tafi is a model that has this setup, this helps to sync all the materials on the model as needed..")]
            public List<Material> hairMaterialsToSync = new List<Material>();
            [HideInInspector] public List<Material> hairMaterialsSynced = new List<Material>();

            [Header("Optional Material Section")]

            [Tooltip("Your models material, this is a requirement for the models to work right for certain models to change colors. Simply look at the material that is attached to the original prefab.")]
            public List<Material> bodyMatList;

            [Tooltip("Your models material, this is a requirement for the models to work right for certain models to change colors. Simply look at the material that is attached to the original prefab.")]
            public List<Material> headMatList;

            [Tooltip("Your models material, this is a requirement for the models to work right for certain models to change colors. Simply look at the material that is attached to the original prefab.")]
            public List<Material> hairMatList;

            [Tooltip("Your models material, this is a requirement for the models to work right for certain models to change colors. Simply look at the material that is attached to the original prefab.")]
            public List<Material> eyeMatList;

            [Tooltip("Your models material, this is a requirement for the models to work right for certain models to change colors. Simply look at the material that is attached to the original prefab.")]
            public List<Material> mouthMatList;


            [Tooltip("The list of colors your characters skin can be. ")]
            public Color32[] skinColors;
            [Tooltip("The list of colors your characters hair can be. ")]
            public Color32[] hairColors;
            [Tooltip("The list of colors your characters eyes can be. ")]
            public Color32[] eyeColors;
            [Tooltip("The list of colors your characters stubble can be. ")]
            public Color32[] stubbleColors;
            [Tooltip("The list of colors your characters body art can be. ")]
            public Color32[] bodyArtColors;
            [Tooltip("The list of colors your characters scars can be. ")]
            public Color32[] scarColors;


            [Tooltip("Whether the material should support GPU instancing. Note: if you are using the color system this is effectively disabled as any change to the materials disables this.")]
            [HideInInspector] public bool enableInstancing;

            [Header("Model Shader Section")]

            [Tooltip("Your models material, this is a requirement for the models to work right for certain models to change colors.This would be the default shader for this model OR a custom one such as provided for color changing.")]
            public Shader bodyShader;

            [Tooltip("Your models material, this is a requirement for the models to work right for certain models to change colors.This would be the default shader for this model OR a custom one such as provided for color changing.")]
            public Shader headShader;

            [Tooltip("Your models material, this is a requirement for the models to work right for certain models to change colors.This would be the default shader for this model OR a custom one such as provided for color changing.")]
            public Shader hairShader;

            [Tooltip("Your models material, this is a requirement for the models to work right for certain models to change colors. This would be the default shader for this model OR a custom one such as provided for color changing.")]
            public Shader eyeShader;

            [Tooltip("Your models material, this is a requirement for the models to work right for certain models to change colors. This would be the default shader for this model OR a custom one such as provided for color changing.")]
            public Shader mouthShader;

            [Header("Color Options")]
            [Tooltip("Some models require setting the color directly via the color option. Infinity PBR hair is an example of this. ")]
            public bool directSetHairColor = true;
            

            [Tooltip("Some models require setting the color directly via the color option. Meshtint eyes is an example of this.")]
            public bool directSetEyeColor = true;

            [Tooltip("Some models require setting the color directly via the color option. Meshtint eyes is an example of this.")]
            public bool directSetSkinColor = true;

            public string colorParamName = "_Color_Skin";
            
            [Tooltip("Some models use a color 2 option (the girls) for blending, use the eye to directly set this color option.")]
            public bool useBeardColorForColor2;

            [Tooltip("Allow setting individual colors on body parts, enabling this uses more memory.")]
            public bool enableMultipleCharacterColors;

            [Tooltip("Enabling this setting will disable the ability to change colors and will force all materials to be the same, lowering the draw calls for games. Use this if you have no intention of enabling the color system.")]
            public bool completelyDisableInstancedMaterials;

            private Material internalMat;
            private Material internalHandsMat;
            private Material internalLowerArmsMat;
            private Material internalUpperArmsMat;
            private Material internalTorsoMat;
            private Material internalHipsMat;
            private Material internalHelmetMat;
            private Material internalLowerLegsMat;
            private Material internalFeetMat;
            private Material internalHeadMat;
            private Material internalHairMat;
            private Material internalMouthMat;
            private Material internalEyeMat;
            private Material internalBeardMat;
            private Material internalEyebrowMat;


            [Header("Internal/External Model Settings")]


            [Tooltip("This will allow different hair colors for the eyebrow, beard and hair models on some characters.")]
            [SerializeField] public bool allowDifferentHairColors;

            [Tooltip("Are the models inside or outside of the prefab.")]
            [SerializeField] public bool internalHairModels;

            [Tooltip("Are the models inside or outside of the prefab.")]
            [SerializeField] public bool internalBeardModels;

            [Tooltip("Are the models inside or outside of the prefab.")]
            [SerializeField] public bool internalEyebrowModels;

            [Tooltip("Are the models inside or outside of the prefab.")]
            [SerializeField] public bool internalMouthModels;

            [Tooltip("Are the models inside or outside of the prefab.")]
            [SerializeField] public bool internalEarModels;

            [Tooltip("Are the models inside or outside of the prefab.")]
            [SerializeField] public bool internalEyeModels;

            [Tooltip("Are the models inside or outside of the prefab.")]
            [SerializeField] public bool internalTuskModels;

            [Header("Character Model Hair Transforms")]
            [Tooltip("Select the root joint in the skeleton where the model will spawn.")]
            public Transform parentJoint;

            [Tooltip("Select the root joint in the skeleton where the model will spawn.")]
            public Transform beardJoint;

            [Tooltip("Select the root joint in the skeleton where the model will spawn.")]
            public Transform eyeBrowJoint;

            [Tooltip("Select the root joint in the skeleton where the model will spawn.")]
            public Transform earJoint;

            [Tooltip("Select the root joint in the skeleton where the model will spawn.")]
            public Transform mouthJoint;

            [Tooltip("Select the root joint in the skeleton where the model will spawn.")]
            public Transform eyeJoint;

            [Tooltip("Select the root joint in the skeleton, this is usually the hips and is for skeleton swapping. ")]
            public Transform rootBone;

            [Header("Main Modular Character Center")]
            [Tooltip("The list of models that will be used to cycle through. Note this fields name is arbitrary, use the field closest to the body part your trying to swap.")]
            public List<GameObject> headModels;


            [Tooltip("The list of models that will be used to cycle through. Use this one for full body skinned mesh swapping or for swapping characters in the Synty line up.")]
            public List<GameObject> torsoModels;

            [Tooltip("The list of models that will be used to cycle through. Note this fields name is arbitrary, use the field closest to the body part your trying to swap.")]
            public List<GameObject> upperArmModels;

            [Tooltip("The list of models that will be used to cycle through. Note this fields name is arbitrary, use the field closest to the body part your trying to swap.")]
            public List<GameObject> lowerArmModels;

            [Tooltip("The list of models that will be used to cycle through. Note this fields name is arbitrary, use the field closest to the body part your trying to swap.")]
            public List<GameObject> handModels;

            [Tooltip("The list of models that will be used to cycle through. Note this fields name is arbitrary, use the field closest to the body part your trying to swap.")]
            public List<GameObject> hipModels;

            [Tooltip("The list of models that will be used to cycle through. Note this fields name is arbitrary, use the field closest to the body part your trying to swap.")]
            public List<GameObject> lowerLegModels;

            [Tooltip("The list of models that will be used to cycle through. Note this fields name is arbitrary, use the field closest to the body part your trying to swap.")]
            public List<GameObject> feetModels;

#if IPBRInt
            [Header("Infinity PBR Section")]
            [Tooltip("The list of presets that will be used to cycle through for InfinityPBR.")]
            public List<TextAsset> blendshapePresetObjects = new List<TextAsset>();
            public InfinityPBRIntegrationV3 infinityPBRIntegration;
            public bool enableSavingInfinityPBRBlendshapes = false;
#endif


            [Header("Optional Models Center")]
            [Tooltip("The list of all potential models that might be used by this character. This is for the shader to do it's work properly.")]
            public List<GameObject> helmetModels;

            [Tooltip("The list of models that will be used to cycle through.")]
            public List<GameObject> hairModels;

            [Tooltip("The list of models that will be used to cycle through.")]
            public List<GameObject> faceModels;

            [Tooltip("The list of models that will be used to cycle through.")]
            public List<GameObject> beardModels;

            [Tooltip("The list of models that will be used to cycle through.")]
            public List<GameObject> eyebrowModels;

            [Tooltip("The list of models that will be used to cycle through.")]
            public List<GameObject> earModels;

            [Tooltip("The list of models that will be used to cycle through.")]
            public List<GameObject> tuskModels;

            [Tooltip("The list of models that will be used to cycle through.")]
            public List<GameObject> mouthModels;

            [Tooltip("The list of models that will be used to cycle through.")]
            public List<GameObject> eyeModels;


            [Tooltip("The list of all potential models that might be used by this character. This is for the shader to do it's work properly.")]
            public List<ModularReplacementItem> replacementItems;

            [Header("Model Property Names")]
            [Tooltip("Future holder for face texture support.")]
            [HideInInspector] public string faceTexPropertyName = "FaceModel";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string hairPropertyName = "HairModel";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string beardPropertyName = "BeardModel";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string eyebrowPropertyName = "EyebrowModel";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string headPropertyName = "HeadModel";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string handsPropertyName = "HandsModel";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string lowerArmsPropertyName = "LowerArmsModel";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string upperArmsPropertyName = "UpperArmsModel";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string torsoPropertyName = "TorsoModel";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string hipsPropertyName = "HipsModel";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string lowerLegsPropertyName = "LowerLegs";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string feetPropertyName = "FeetModel";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string mouthPropertyName = "MouthModel";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string eyesPropertyName = "EyeModel";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string tuskPropertyName = "TusksModel";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string earsPropertyName = "EarModel";


            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string EyeMaterialPropertyName = "EyeMaterial";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string HairMaterialPropertyName = "HairMaterial";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string SkinMaterialPropertyName = "SkinMaterial";
            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string MouthMaterialPropertyName = "MouthMaterial";            

            [Header("Color Property Names")]
            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string bodyColorPropertyName = "BodyColor";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string scarColorPropertyName = "ScarColor";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string mouthColorPropertyName = "MouthColor";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string hairColorPropertyName = "HairColor";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string stubbleColorPropertyName = "StubbleColor";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string beardColorPropertyName = "BeardColor";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string eyeBrowColorPropertyName = "EyeBrowColor";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string bodyArtColorPropertyName = "BodyArtColor";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string eyeColorPropertyName = "EyeColor";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string helmetColorPropertyName = "HelmetColor";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string torsoColorPropertyName = "TorsoColor";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string upperArmsColorPropertyName = "UpperArmsColor";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string lowerArmsColorPropertyName = "LowerArmsColor";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string hipsColorPropertyName = "HipsColor";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string lowerLegsColorPropertyName = "LowerLegsColor";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string handsColorPropertyName = "HandsColor";

            [Tooltip("The name of the item that helps dentify the values held.")]
            [HideInInspector] public string feetColorPropertyName = "FeetColor";

            [Tooltip("The name of the item that helps identify the values held.")]
            [HideInInspector] public string faithPropertyName = "FaithValue";

            [Tooltip("The name of the item that helps identify the values held.")]
            [HideInInspector] public string infinityBlendShapes = "BlendshapesValue";

            [Tooltip("The name of the item that helps identify the values held.")]
            [HideInInspector] public string blendshapePresetValue = "BlendshapePresetValue";

            [Header("Resource Folder Hair Folder")]
            [HideInInspector] public string hairDirectory = "_Customizer_Male_Hair"; // Resource Folder Hair Folder
            [HideInInspector] public string FhairDirectory = "_Customizer_Female_Hair"; // Resource Folder Hair Folder
            [Header("Resource Folder Beard Folder")]
            [HideInInspector] public string beardDirectory = "_Customizer_Male_Beards";  // Resource Folder Beard Folder
            [HideInInspector] public string fbeardDirectory = "_Customizer_Female_Beards";
            [Header("Resource Folder Eyebrow Folder")]
            [HideInInspector] public string eyebrowDirectory = "_Customizer_Male_Eyebrows"; // Resource Folder eyebrow folder
            [HideInInspector] public string feyebrowDirectory = "_Customizer_Female_Eyebrows";

            [Header("Male Default Parts")]
            [Tooltip("")]
            public List<string> defaultHeadName = new List<string>();
            [Tooltip("")]
            public List<string> defaultFaceName = new List<string>();
            [Tooltip("")]
            public List<string> defaultTorsoName = new List<string>();
            [Tooltip("")]
            public List<string> defaultUpperArmName = new List<string>();
            [Tooltip("")]
            public List<string> defaultLowerArmName = new List<string>();
            [Tooltip("")]
            public List<string> defaultHandName = new List<string>();
            [Tooltip("")]
            public List<string> defaultHipsName = new List<string>();
            [Tooltip("")]
            public List<string> defaultLowerLegName = new List<string>();
            [Tooltip("")]
            public List<string> defaultFeetName = new List<string>();
            [Tooltip("")]
            public string defaultMouthColorName = "";
            [Tooltip("")]
            public string defaultEyeColorName = "";
            [Tooltip("")]
            public string defaultEarName = "";
            [Tooltip("")]
            public string defaultTuskName = "";
            [Tooltip("")]
            public string defaultBeardName = "";
            [Tooltip("")]
            public string defaultHairName = "";
            [Tooltip("")]
            public List<string> defaultCapeName = new List<string>();

            [Header("Female Default Parts")]
            [Tooltip("")]
            public List<string> defaultFemaleHeadName = new List<string>();
            [Tooltip("")]
            public List<string> defaultFemaleFaceName = new List<string>();
            [Tooltip("")]
            public List<string> defaultFemaleTorsoName = new List<string>();
            [Tooltip("")]
            public List<string> defaultFemaleUpperArmName =new List<string>();
            [Tooltip("")]
            public List<string> defaultFemaleLowerArmName = new List<string>();
            [Tooltip("")]
            public List<string> defaultFemaleHandName = new List<string>();
            [Tooltip("")]
            public List<string> defaultFemaleHipsName = new List<string>();
            [Tooltip("")]
            public List<string> defaultFemaleLowerLegName = new List<string>();
            [Tooltip("")]
            public List<string> defaultFemaleFeetName = new List<string>();
            [Tooltip("")]
            public string defaultFemaleMouthName = "";
            [Tooltip("")]
            public string defaultFemaleEyeName = "";
            [Tooltip("")]
            public string defaultFemaleEarName = "";
            [Tooltip("")]
            public string defaultFemaleTuskName = "";
            [Tooltip("")]
            public string defaultFemaleBeardName = "";
            [Tooltip("")]
            public string defaultFemaleHairName = "";
            [Tooltip("")]
            public List<string> defaultFemaleCapeName = new List<string>();

            [Tooltip("")]
            public string defaultFaithName = "";

            [Header("Default Color")]
            [Tooltip("The default color for the selected item.")]
            public Color32 defaultSkinColor = new Color32(255, 204, 174, 255);
            [Tooltip("The default color for the selected item.")]
            public Color32 defaultHairColor = new Color32(79, 65, 45, 255);
            [Tooltip("The default color for the selected item.")]
            public Color32 defaultScarColor = new Color32(237, 175, 151, 255);
            [Tooltip("The default color for the selected item.")]
            public Color32 defaultStubbleColor = new Color32(13, 172, 251, 255);
            [Tooltip("The default color for the selected item.")]
            public Color32 defaultBodyArtColor = new Color32(13, 172, 251, 255);
            [Tooltip("The default color for the selected item.")]
            public Color32 defaultEyeColor = new Color32(58, 148, 193, 255);
            [Tooltip("The default color for the selected item.")]
            public Color32 defaultMouthColor = new Color32(255, 204, 174, 255); //check
            [Tooltip("The default color for the selected item.")]
            public Color32 defaultPrimaryColor = new Color32(62, 107, 158, 255);
            [Tooltip("The default color for the selected item.")]
            public Color32 defaultSecondaryColor = new Color32(209, 164, 76, 255);
            [Tooltip("The default color for the selected item.")]
            public Color32 defaultTertiaryColor = new Color32(95, 84, 71, 255);
            [Tooltip("The default color for the selected item.")]
            public Color32 defaultLeatherPrimaryColor = new Color32(72, 53, 42, 255);
            [Tooltip("The default color for the selected item.")]
            public Color32 defaultLeatherTertiaryColor = new Color32(95, 84, 71, 255);
            [Tooltip("The default color for the selected item.")]
            public Color32 defaultMetalPrimaryColor = new Color32(152, 156, 160, 255);
            [Tooltip("The default color for the selected item.")]
            public Color32 defaultLeatherSecondaryColor = new Color32(95, 84, 71, 255);
            [Tooltip("The default color for the selected item.")]
            public Color32 defaultMetalDarkColor = new Color32(45, 50, 55, 255);
            [Tooltip("The default color for the selected item.")]
            public Color32 defaultMetalSecondaryColor = new Color32(35, 148, 193, 255);

            // normal stuff needs to stay as is 
            int switchHair = -1;
            int switchBeard = -1;
            int switchEyebrow = -1;
            int switchEye = -1;
            int switchMouth = -1;
            int switchTusk = -1;
            int switchEar = -1;

            int eyeMaterialIndex = 0;
            int skinMaterialIndex = 0;
            int hairMaterialIndex = 0;
            int mouthMaterialIndex = 0;

            int handIndex = 0;
            int lowerArmIndex = 0;
            int upperArmIndex = 0;
            int torsoIndex = 0;
            int hipsIndex = 0;
            int lowerLegIndex = 0;
            int feetIndex = 0;
            int switchHead = 0;
            int switchFace = 0;

            int eyeColorIndex = 0;
            int stubbleColorIndex = 0;
            int skinScarColorIndex = 0;
            int skinColorIndex = 0;
            int hairColorIndex = 0;
            int beardColorIndex = 0;
            int bodyArtColorIndex = 0;
            int eyebrowColorIndex = 0;

            int blendshapePresetObjectsIndex = -1;
            int replacementIndex = 0;

            private System.Guid myGUID;
            public enum BodyType
            {
                // Add a new type if needed, do not change existing
                Hands = 0,
                LowerArms = 1,
                Upperarms = 2,
                Torso = 3,
                Hips = 4,
                LowerLegs = 5,
                Head = 6,
                None = 7,
                Helmet = 8,
                Cape = 9,
                Feet = 10,
                Face = 11,
                Skin = 12,
                Eye = 13,
                Hair = 14,
                Mouth =15
            }

            public enum ShaderColorType
            {
                Primary = 0,
                Secondary = 1,
                LeatherPrimary = 2,
                LeatherSecondary = 3,
                MetalPrimary = 4,
                MetalDark = 5,
                MetalSecondary = 6,
                Hair = 7,
                Skin = 8,
                Stubble = 9,
                Scar = 10,
                BodyArt = 11,
                Eyes = 12,
                Mouth = 13,
                Beard = 14,
                LeatherTertiary = 15,
                Tertiary = 16
            }



            // Use this for initialization
            void Start()
            {
                if(ActiveHands == null)
                ActiveHands = new List<GameObject>();
                if(ActiveLowerArms == null)
                ActiveLowerArms = new List<GameObject>();
                if(ActiveUpperArms == null)
                ActiveUpperArms = new List<GameObject>();
                if(ActiveTorso == null)
                ActiveTorso = new List<GameObject>();
                if(ActiveHips == null)
                ActiveHips = new List<GameObject>();
                if(ActiveLowerLegs == null)
                ActiveLowerLegs = new List<GameObject>();
                if(ActiveFeet == null)
                ActiveFeet = new List<GameObject>();
                if(ActiveHead == null)
                ActiveHead = new List<GameObject>();
                if(ActiveTusk == null)
                ActiveTusk = new List<GameObject>();
            }


            void Awake()
            {
                if(ActiveHands == null)
                    ActiveHands = new List<GameObject>();
                if(ActiveLowerArms == null)
                    ActiveLowerArms = new List<GameObject>();
                if(ActiveUpperArms == null)
                    ActiveUpperArms = new List<GameObject>();
                if(ActiveTorso == null)
                    ActiveTorso = new List<GameObject>();
                if(ActiveHips == null)
                    ActiveHips = new List<GameObject>();
                if(ActiveLowerLegs == null)
                    ActiveLowerLegs = new List<GameObject>();
                if(ActiveFeet == null)
                    ActiveFeet = new List<GameObject>();
                if(ActiveHead == null)
                    ActiveHead = new List<GameObject>();
                if(ActiveTusk == null)
                    ActiveTusk = new List<GameObject>();
                
                myGUID = System.Guid.NewGuid();

                if (internalMat == null && (bodyShader != null && bodyMat != null))
                {
                    internalMat = new Material(bodyShader);
                    internalMat.enableInstancing = enableInstancing;
                    internalMat.CopyPropertiesFromMaterial(bodyMat);
                    internalMat.name = "internalMat" + myGUID;
                    internalMat.color = defaultSkinColor;

                }

                if (skinMaterialsToSync.Count > 0 && bodyShader!=null)
                {
                    foreach (Material material in skinMaterialsToSync)
                    {
                        Material internalMatSynced = new Material(bodyShader);
                        internalMatSynced.enableInstancing = enableInstancing;
                        internalMatSynced.CopyPropertiesFromMaterial(material);
                        internalMatSynced.name = material.name + "_internalSynced" + myGUID;
                        internalMatSynced.color = defaultSkinColor;
                        skinMaterialsSynced.Add(internalMatSynced);
                    }
                }

                if (hairMaterialsToSync.Count > 0)
                {
                    foreach (Material material in hairMaterialsToSync)
                    {
                        Material internalMatSynced = new Material(hairShader);
                        internalMatSynced.enableInstancing = enableInstancing;
                        internalMatSynced.CopyPropertiesFromMaterial(hairMat);
                        internalMatSynced.name = material.name + "_internalSynced" + myGUID;
                        internalMatSynced.color = defaultSkinColor;
                        hairMaterialsSynced.Add(internalMatSynced);
                    }
                }

                if (internalHandsMat == null && (bodyShader != null && bodyMat != null) && enableMultipleCharacterColors)
                {
                    internalHandsMat = new Material(bodyShader);
                    internalHandsMat.enableInstancing = enableInstancing;
                    internalHandsMat.CopyPropertiesFromMaterial(bodyMat);
                    internalHandsMat.name = "internalHandsMat" + myGUID;
                    internalHandsMat.color = defaultSkinColor;

                }

                if (internalLowerArmsMat == null && (bodyShader != null && bodyMat != null) && enableMultipleCharacterColors)
                {
                    internalLowerArmsMat = new Material(bodyShader);
                    internalLowerArmsMat.enableInstancing = enableInstancing;
                    internalLowerArmsMat.CopyPropertiesFromMaterial(bodyMat);
                    internalLowerArmsMat.name = "internalLowerArmsMat" + myGUID;
                    internalLowerArmsMat.color = defaultSkinColor;


                }

                if (internalUpperArmsMat == null && (bodyShader != null && bodyMat != null) && enableMultipleCharacterColors)
                {
                    internalUpperArmsMat = new Material(bodyShader);
                    internalUpperArmsMat.enableInstancing = enableInstancing;
                    internalUpperArmsMat.CopyPropertiesFromMaterial(bodyMat);
                    internalUpperArmsMat.name = "internalUpperArmsMat" + myGUID;
                    internalUpperArmsMat.color = defaultSkinColor;

                }

                if (internalTorsoMat == null && (bodyShader != null && bodyMat != null) && enableMultipleCharacterColors)
                {
                    internalTorsoMat = new Material(bodyShader);
                    internalTorsoMat.enableInstancing = enableInstancing;
                    internalTorsoMat.CopyPropertiesFromMaterial(bodyMat);
                    internalTorsoMat.name = "internalTorsoMat" + myGUID;
                    internalTorsoMat.color = defaultSkinColor;

                }

                if (internalHipsMat == null && (bodyShader != null && bodyMat != null) && enableMultipleCharacterColors)
                {
                    internalHipsMat = new Material(bodyShader);
                    internalHipsMat.enableInstancing = enableInstancing;
                    internalHipsMat.CopyPropertiesFromMaterial(bodyMat);
                    internalHipsMat.name = "internalHipsMat" + myGUID;
                    internalHipsMat.color = defaultSkinColor;

                }

                if (internalFeetMat == null && (bodyShader != null && bodyMat != null) && enableMultipleCharacterColors)
                {
                    internalFeetMat = new Material(bodyShader);
                    internalFeetMat.enableInstancing = enableInstancing;
                    internalFeetMat.CopyPropertiesFromMaterial(bodyMat);
                    internalFeetMat.name = "internalFeetMat" + myGUID;
                    internalFeetMat.color = defaultSkinColor;

                }

                if (internalLowerLegsMat == null && (bodyShader != null && bodyMat != null) && enableMultipleCharacterColors)
                {
                    internalLowerLegsMat = new Material(bodyShader);
                    internalLowerLegsMat.enableInstancing = enableInstancing;
                    internalLowerLegsMat.CopyPropertiesFromMaterial(bodyMat);
                    internalLowerLegsMat.name = "internalLowerLegsMat" + myGUID;
                    internalLowerLegsMat.color = defaultSkinColor;

                }

                if (internalHeadMat == null && (headShader != null && headMat != null))
                {
                    internalHeadMat = new Material(headShader);
                    internalHeadMat.enableInstancing = enableInstancing;
                    internalHeadMat.CopyPropertiesFromMaterial(headMat);
                    internalHeadMat.name = "internalHeadMat" + myGUID;
                    internalHeadMat.color = defaultSkinColor;

                }
                else if (internalHeadMat == null && (headShader == null && headMat == null) && bodyShader!=null)
                {
                    internalHeadMat = new Material(bodyShader);
                    internalHeadMat.enableInstancing = enableInstancing;
                    internalHeadMat.CopyPropertiesFromMaterial(bodyMat);
                    internalHeadMat.name = "internalBodyHeadMat" + myGUID;
                    internalHeadMat.color = defaultSkinColor;
                }

                if (internalHelmetMat == null && (headShader != null && headMat != null) && enableMultipleCharacterColors&& bodyShader!=null)
                {
                    internalHelmetMat = new Material(bodyShader);
                    internalHelmetMat.enableInstancing = enableInstancing;
                    internalHelmetMat.CopyPropertiesFromMaterial(bodyMat);
                    internalHelmetMat.name = "internalHelmetMat" + myGUID;
                }


                if (internalHairMat == null && (hairShader != null && (hairMat != null || bodyMat != null)))
                {
                    if (hairShader)
                    {
                        internalHairMat = new Material(hairShader);
                        internalHairMat.enableInstancing = enableInstancing;
                        if (hairMat)
                        {
                            internalHairMat.CopyPropertiesFromMaterial(hairMat);
                        }
                        else
                        {
                            internalHairMat.CopyPropertiesFromMaterial(bodyMat);
                        }
                        internalHairMat.name = "internalHairMat" + myGUID;
                        internalHairMat.color = defaultHairColor;

                    }
                }
                else
                {
                    if (hairShader)
                    {
                        internalHairMat = new Material(hairShader);
                        internalHairMat.enableInstancing = enableInstancing;
                        internalHairMat.CopyPropertiesFromMaterial(hairMat);
                        internalHairMat.name = "internalHairMat" + myGUID;
                        internalHairMat.color = defaultHairColor;

                    }
                    else if( bodyShader!=null)
                    {
                        internalHairMat = new Material(bodyShader);
                        internalHairMat.enableInstancing = enableInstancing;
                        internalHairMat.CopyPropertiesFromMaterial(bodyMat);
                        internalHairMat.name = "internalHairMat" + myGUID;
                        internalHairMat.color = defaultHairColor;


                    }
                }

                if (internalMouthMat == null && (mouthShader != null && mouthMat != null))
                {
                    if (mouthShader)
                    {
                        internalMouthMat = new Material(mouthShader);
                        internalMouthMat.enableInstancing = enableInstancing;
                        if (mouthMat)
                        {
                            internalMouthMat.CopyPropertiesFromMaterial(mouthMat);
                        }
                        internalMouthMat.name = "internalMouthMat" + myGUID;
                        internalMouthMat.color = defaultMouthColor;

                    }
                }
                else
                {
                    internalMouthMat = internalMat;
                }

                if (internalEyeMat == null && (eyeShader != null && eyeMat != null))
                {
                    if (eyeShader)
                    {
                        internalEyeMat = new Material(eyeShader);
                        internalEyeMat.enableInstancing = enableInstancing;
                        if (eyeMat)
                        {
                            internalEyeMat.CopyPropertiesFromMaterial(eyeMat);
                        }
                        internalEyeMat.name = "internalEyeMat" + myGUID;
                        internalEyeMat.color = defaultEyeColor;

                    }
                }
                else
                {
                    internalEyeMat = internalMat;
                }

                if (internalBeardMat == null && allowDifferentHairColors && ((hairShader != null || bodyShader != null) && (hairMat != null || bodyMat != null)))
                {
                    if (hairShader)
                    {
                        internalBeardMat = new Material(hairShader);
                        internalEyebrowMat = new Material(hairShader);
                    }
                    else
                    {
                        internalBeardMat = new Material(bodyShader);
                        internalEyebrowMat = new Material(bodyShader);
                    }
                    if (hairMat)
                    {
                        internalBeardMat.CopyPropertiesFromMaterial(hairMat);
                        internalEyebrowMat.CopyPropertiesFromMaterial(hairMat);
                    }
                    else
                    {
                        internalBeardMat.CopyPropertiesFromMaterial(bodyMat);
                        internalEyebrowMat.CopyPropertiesFromMaterial(bodyMat);
                    }

                    internalBeardMat.enableInstancing = enableInstancing;
                    internalBeardMat.name = "internalBeardMat" + myGUID;

                    internalEyebrowMat.enableInstancing = enableInstancing;
                    internalEyebrowMat.name = "internalEyebrowMat" + myGUID;
                    internalBeardMat.color = defaultHairColor;
                    internalEyebrowMat.color = defaultHairColor;
                }
                else if (internalBeardMat == null && allowDifferentHairColors && ((hairShader == null && bodyShader == null) && (hairMat == null && bodyMat == null)))
                {
                    if (bodyShader != null)
                    {
                        internalBeardMat = new Material(bodyShader);
                        internalEyebrowMat = new Material(bodyShader);
                        internalBeardMat.CopyPropertiesFromMaterial(bodyMat);
                        internalEyebrowMat.CopyPropertiesFromMaterial(bodyMat);
                        internalBeardMat.enableInstancing = enableInstancing;
                        internalBeardMat.name = "internalBeardMat" + myGUID;

                        internalEyebrowMat.enableInstancing = enableInstancing;
                        internalEyebrowMat.name = "internalEyebrowMat" + myGUID;
                        internalBeardMat.color = defaultHairColor;
                        internalEyebrowMat.color = defaultHairColor;
                    }
                }


                ActiveBodyColor = defaultSkinColor;
                ActiveHairColor = defaultHairColor;
                ActiveScarColor = defaultScarColor;
                ActiveStubbleColor = defaultStubbleColor;
                ActiveEyeColor = defaultEyeColor;
                ActiveBodyArtColor = defaultBodyArtColor;
                ActiveEyebrowColor = defaultHairColor;
                ActiveBeardColor = defaultHairColor;
                ActiveMouthColor = defaultMouthColor;

                if (gender == characterGender.Female)
                {
                    if (defaultFemaleHeadName.Count > 0)
                    {
                        foreach (var s in defaultFemaleHeadName)
                        {
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                    ActiveHead.Add(tr.gameObject);        
                                }
                            }
                            
                        }
                       // ActiveHead = FindDeepChild(defaultFemaleHeadName).gameObject;
                    }
                    if (defaultFemaleHandName.Count > 0)
                    {   foreach (var s in defaultFemaleHandName)
                        {
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                    ActiveHands.Add(tr.gameObject);        
                                }
                            }
                            
                        }
                        //ActiveHands = FindDeepChild(defaultFemaleHandName).gameObject;
                    }
                    if (defaultFemaleLowerArmName.Count > 0)
                    {   foreach (var s in defaultFemaleLowerArmName)
                        {
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                    ActiveLowerArms.Add(tr.gameObject);        
                                }
                            }
                            
                        }
                       // ActiveLowerArms = FindDeepChild(defaultFemaleLowerArmName).gameObject;
                    }
                    if (defaultFemaleUpperArmName.Count > 0)
                    {   foreach (var s in defaultFemaleUpperArmName)
                        {
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                    ActiveUpperArms.Add(tr.gameObject);        
                                }
                            }
                            
                        }
                       // ActiveUpperArms = FindDeepChild(defaultFemaleUpperArmName).gameObject;
                    }
                    if (defaultFemaleTorsoName.Count > 0)
                    {   foreach (var s in defaultFemaleTorsoName)
                        {
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                    ActiveTorso.Add(tr.gameObject);        
                                }
                            }
                            
                        }
                      //  ActiveTorso = FindDeepChild(defaultFemaleTorsoName).gameObject;
                    }
                    if (defaultFemaleHipsName.Count > 0)
                    {   foreach (var s in defaultFemaleHipsName)
                        {
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                    ActiveHips.Add(tr.gameObject);        
                                }
                            }
                            
                        }
                      //  ActiveHips = FindDeepChild(defaultFemaleHipsName).gameObject;
                    }
                    if (defaultFemaleLowerLegName.Count > 0)
                    {   foreach (var s in defaultFemaleLowerLegName)
                        {
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                    ActiveLowerLegs.Add(tr.gameObject);        
                                }
                            }
                            
                        }
                       // ActiveLowerLegs = FindDeepChild(defaultFemaleLowerLegName).gameObject;
                    }
                    if (defaultFemaleFeetName.Count > 0)
                    {   foreach (var s in defaultFemaleFeetName)
                        {
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                    ActiveFeet.Add(tr.gameObject);        
                                }
                            }
                            
                        }
                        //ActiveFeet = FindDeepChild(defaultFemaleFeetName).gameObject;
                    }
                    if (defaultFemaleHairName.Length > 0)
                    {  /* foreach (var s in defaultFemaleHairName)
                        {
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                    ActiveHair.Add(tr.gameObject);        
                                }
                            }
                            
                        }*/
                       ActiveHair = FindDeepChild(defaultFemaleHairName).gameObject;
                    }
                    if (defaultFemaleBeardName.Length > 0)
                    {/*   foreach (var s in defaultFemaleBeardName)
                        {
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                    ActiveBeard.Add(tr.gameObject);        
                                }
                            }
                            
                        }*/
                        ActiveBeard = FindDeepChild(defaultFemaleBeardName).gameObject;
                    }
                }
                else
                {
                    if (defaultHeadName.Count > 0)
                    {
                        foreach (var s in defaultHeadName)
                        {
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                    ActiveHead.Add(tr.gameObject);
                                }
                            }

                        }
                    }

                    if (defaultHandName.Count > 0)
                    {
                        foreach (var s in defaultHandName)
                        {
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                    ActiveHands.Add(tr.gameObject);
                                }
                            }

                        }
                        //  ActiveHands = FindDeepChild(defaultHandName).gameObject;
                    }

                    if (defaultLowerArmName.Count > 0)
                    {
                        foreach (var s in defaultLowerArmName)
                        {
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                    ActiveLowerArms.Add(tr.gameObject);
                                }
                            }

                        }
                        //  ActiveLowerArms = FindDeepChild(defaultLowerArmName).gameObject;
                    }

                    if (defaultUpperArmName.Count > 0)
                    {
                        foreach (var s in defaultUpperArmName)
                        {
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                    ActiveUpperArms.Add(tr.gameObject);
                                }
                            }

                        }
                        // ActiveUpperArms = FindDeepChild(defaultUpperArmName).gameObject;
                    }

                    if (defaultTorsoName.Count > 0)
                    {
                        foreach (var s in defaultTorsoName)
                        {
                          //  Debug.LogError("s="+s);
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                  //  Debug.LogError(tr.gameObject);
                                    ActiveTorso.Add(tr.gameObject);
                                }
                                else
                                {
                                    Debug.LogError(" Not Found "+s);
                                }
                            }

                        }
                        // ActiveTorso = FindDeepChild(defaultTorsoName).gameObject;
                    }

                    if (defaultHipsName.Count > 0)
                    {
                        foreach (var s in defaultHipsName)
                        {
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                    ActiveHips.Add(tr.gameObject);
                                }
                            }

                        }
                        //  ActiveHips = FindDeepChild(defaultHipsName).gameObject;
                    }

                    if (defaultLowerLegName.Count > 0)
                    {
                        foreach (var s in defaultLowerLegName)
                        {
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                    ActiveLowerLegs.Add(tr.gameObject);
                                }
                            }

                        }
                        //   ActiveLowerLegs = FindDeepChild(defaultLowerLegName).gameObject;
                    }

                    if (defaultFeetName.Count > 0)
                    {
                        foreach (var s in defaultFeetName)
                        {
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                    ActiveFeet.Add(tr.gameObject);
                                }
                            }

                        }
                        //  ActiveFeet = FindDeepChild(defaultFeetName).gameObject;
                    }

                    if (defaultHairName.Length > 0)
                    {
                        /*foreach (var s in defaultHairColorName)
                        {
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                    ActiveHair.Add(tr.gameObject);
                                }
                            }

                        }*/
                          ActiveHair = FindDeepChild(defaultHairName).gameObject;
                    }

                    if (defaultBeardName.Length > 0)
                    {
                       /* foreach (var s in defaultBeardName)
                        {
                            if (s.Length > 0)
                            {
                                Transform tr = FindDeepChild(s);
                                if (tr != null)
                                {
                                    ActiveBeard.Add(tr.gameObject);
                                }
                            }

                        }*/
                        ActiveBeard = FindDeepChild(defaultBeardName).gameObject;
                    }
                }

#if ModularNHance
                if (defaultLegs)
                {
                    ActiveHips = enableNHance(defaultLegs);
                }

                if (defaultHands)
                {
                    ActiveHands = enableNHance(defaultHands);
                }

                if (defaultFeet)
                {
                    ActiveFeet = enableNHance(defaultFeet);
                }

                if (defaultTorso)
                {
                    ActiveTorso = enableNHance(defaultTorso);
                }
#endif
                if (!completelyDisableInstancedMaterials)
                {
                    ReplaceAllModelShaders();
                }
               
            }

            public List<GameObject> enableNHance(GameObject thisobject)
            {
                nHanceReplacementItem m_nHanceReplacementItem = null;

                if (!m_nHanceReplacementItem)
                {
                    m_nHanceReplacementItem = ScriptableObject.CreateInstance<nHanceReplacementItem>();
                    m_nHanceReplacementItem.modelReplacement = thisobject;
                    m_nHanceReplacementItem.name = thisobject.name;
                    //m_nHanceReplacementItem.modelMaterial = internalMat;
                   // m_nHanceReplacementItem.modelMaterial.color = defaultSkinColor;
                }

                SkinnedMeshRenderer skinnedMeshRenderer = m_nHanceReplacementItem.modelReplacement.GetComponent<SkinnedMeshRenderer>();
                if (!skinnedMeshRenderer)
                {
                    skinnedMeshRenderer = m_nHanceReplacementItem.modelReplacement.GetComponentInChildren<SkinnedMeshRenderer>();

                }

                SkinnedMeshRenderer SkinnedMeshRendererItemInstance = Instantiate(skinnedMeshRenderer, transform);

                // ItemInstance.material = m_nHanceReplacementItem.modelMaterial;
                if (SkinnedMeshRendererItemInstance)
                {
#if ModularNHance
                    nHanceEquipment thisequipment = SkinnedMeshRendererItemInstance.gameObject.GetComponent<nHanceEquipment>();


                    if (!thisequipment)
                    {
                        thisequipment = SkinnedMeshRendererItemInstance.gameObject.AddComponent<nHanceEquipment>();
                        thisequipment.target = this.gameObject;
                    }
                    else
                    {
                        thisequipment.target = this.gameObject;
                    }

                    thisequipment.processBones();
#endif
                }
                SkinnedMeshRendererItemInstance.gameObject.name = m_nHanceReplacementItem.name;
                SkinnedMeshRendererItemInstance.name = m_nHanceReplacementItem.name;
                List<GameObject> list = new List<GameObject>();
                list.Add(SkinnedMeshRendererItemInstance.gameObject);
                return list;
            }

            // Using the ObjectNodeReady function means this will run when the node is all set up and ready to go
            protected void ObjectNodeReady()
            {
                if (GetComponent<AtavismNode>() != null)
                {
                    // Register a property changer for the hair model so when the character loads in the game it will show up
                    GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(hairPropertyName, HandleHairChange);
                    // The property may have already been loaded, so do a check now as well
                    if (GetComponent<AtavismNode>().PropertyExists(hairPropertyName.ToString()))
                    {
                        int hairModel = (int)GetComponent<AtavismNode>().GetProperty(hairPropertyName);
                        UpdateHairModel(hairModel);
                    }
                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(EyeMaterialPropertyName, HandleEyeMaterialChange);


                if (GetComponent<AtavismNode>().PropertyExists(EyeMaterialPropertyName))
                {
                    int beardModel = (int)GetComponent<AtavismNode>().GetProperty(EyeMaterialPropertyName);
                     UpdateEyeMaterial((int)beardModel);
                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(SkinMaterialPropertyName, HandleSkinMaterialChange);


                if (GetComponent<AtavismNode>().PropertyExists(SkinMaterialPropertyName))
                {
                    int beardModel = (int)GetComponent<AtavismNode>().GetProperty(SkinMaterialPropertyName);
                    UpdateSkinMaterial((int)beardModel);
                }

                
                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(MouthMaterialPropertyName, HandleMouthMaterialChange);


                if (GetComponent<AtavismNode>().PropertyExists(MouthMaterialPropertyName))
                {
                    int beardModel = (int)GetComponent<AtavismNode>().GetProperty(MouthMaterialPropertyName);
                    UpdateMouthMaterial(beardModel);
                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(HairMaterialPropertyName, HandleHairMaterialChange);

                if (GetComponent<AtavismNode>().PropertyExists(HairMaterialPropertyName))
                {
                    int beardModel = (int)GetComponent<AtavismNode>().GetProperty(HairMaterialPropertyName);
                    UpdateHairMaterial(beardModel);
                }


                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(beardPropertyName, HandleBeardChange);


                if (GetComponent<AtavismNode>().PropertyExists(beardPropertyName))
                {
                    int beardModel = (int)GetComponent<AtavismNode>().GetProperty(beardPropertyName);
                    UpdateBeardModel(beardModel);
                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(mouthPropertyName, HandleMouthChange);


                if (GetComponent<AtavismNode>().PropertyExists(mouthPropertyName))
                {
                    int currentModel = (int)GetComponent<AtavismNode>().GetProperty(mouthPropertyName);
                    UpdateMouthModel(currentModel);
                }
                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(eyesPropertyName, HandleEyesChange);


                if (GetComponent<AtavismNode>().PropertyExists(eyesPropertyName))
                {
                    int currentModel = (int)GetComponent<AtavismNode>().GetProperty(eyesPropertyName);
                    UpdateEyeModel(currentModel);
                }
                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(earsPropertyName, HandleEarsChange);


                if (GetComponent<AtavismNode>().PropertyExists(earsPropertyName))
                {
                    int currentModel = (int)GetComponent<AtavismNode>().GetProperty(earsPropertyName);
                    UpdateEarModel(currentModel);
                }
                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(tuskPropertyName, HandleTusksChange);


                if (GetComponent<AtavismNode>().PropertyExists(tuskPropertyName))
                {
                    int currentModel = (int)GetComponent<AtavismNode>().GetProperty(tuskPropertyName);
                    UpdateTuskModel(currentModel);
                }


                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(eyebrowPropertyName, HandleEyebrowChange);


                if (GetComponent<AtavismNode>().PropertyExists(eyebrowPropertyName))
                {
                    int eyebrowModel = (int)GetComponent<AtavismNode>().GetProperty(eyebrowPropertyName);
                    UpdateEyebrowModel(eyebrowModel);
                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(headPropertyName, HandleHeadChange);

                if (GetComponent<AtavismNode>().PropertyExists(headPropertyName))
                {
                    string bodyPart = (string)GetComponent<AtavismNode>().GetProperty(headPropertyName);
                    UpdateBodyModel(bodyPart, BodyType.Head);
                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(faceTexPropertyName, HandleFaceChange);

                if (GetComponent<AtavismNode>().PropertyExists(faceTexPropertyName))
                {
                    string bodyPart = (string)GetComponent<AtavismNode>().GetProperty(faceTexPropertyName);
                    UpdateBodyModel(bodyPart, BodyType.Face);
                }


                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(handsPropertyName, HandleHandsChange);

                if (GetComponent<AtavismNode>().PropertyExists(handsPropertyName))
                {
                    string bodyPart = (string)GetComponent<AtavismNode>().GetProperty(handsPropertyName);
                    UpdateBodyModel(bodyPart, BodyType.Hands);
                }


                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(lowerArmsPropertyName, HandleLowerArmsChange);

                if (GetComponent<AtavismNode>().PropertyExists(lowerArmsPropertyName))
                {
                    string bodyPart = (string)GetComponent<AtavismNode>().GetProperty(lowerArmsPropertyName);
                    UpdateBodyModel(bodyPart, BodyType.LowerArms);
                }


                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(upperArmsPropertyName, HandleUpperArmsChange);

                if (GetComponent<AtavismNode>().PropertyExists(upperArmsPropertyName))
                {
                    string bodyPart = (string)GetComponent<AtavismNode>().GetProperty(upperArmsPropertyName);
                    UpdateBodyModel(bodyPart, BodyType.Upperarms);
                }


                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(torsoPropertyName, HandleTorsoChange);

                if (GetComponent<AtavismNode>().PropertyExists(torsoPropertyName))
                {
                    string bodyPart = (string)GetComponent<AtavismNode>().GetProperty(torsoPropertyName);
                    UpdateBodyModel(bodyPart, BodyType.Torso);
                }


                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(hipsPropertyName, HandleHipsChange);

                if (GetComponent<AtavismNode>().PropertyExists(hipsPropertyName))
                {
                    string bodyPart = (string)GetComponent<AtavismNode>().GetProperty(hipsPropertyName);
                    UpdateBodyModel(bodyPart, BodyType.Hips);
                }


                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(lowerLegsPropertyName, HandleLowerLegsChange);

                if (GetComponent<AtavismNode>().PropertyExists(lowerLegsPropertyName))
                {
                    string bodyPart = (string)GetComponent<AtavismNode>().GetProperty(lowerLegsPropertyName);
                    UpdateBodyModel(bodyPart, BodyType.LowerLegs);
                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(feetPropertyName, HandleFeetChange);

                if (GetComponent<AtavismNode>().PropertyExists(feetPropertyName))
                {
                    string bodyPart = (string)GetComponent<AtavismNode>().GetProperty(feetPropertyName);
                    UpdateBodyModel(bodyPart, BodyType.Feet);
                }
#if IPBRInt
                if (!enableSavingInfinityPBRBlendshapes)
                {
                    GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(blendshapePresetValue, HandleBlendShapePresetChange);
                    if (GetComponent<AtavismNode>().PropertyExists(blendshapePresetValue) && (!enableSavingInfinityPBRBlendshapes))
                    {
                        int currentModel = (int)GetComponent<AtavismNode>().GetProperty(blendshapePresetValue);
                        UpdateBlendShapePresets(currentModel);
                    }
                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(infinityBlendShapes, HandleBlendShapesChange);
                if (GetComponent<AtavismNode>().PropertyExists(infinityBlendShapes))
                {
                    string currentModel = (string)GetComponent<AtavismNode>().GetProperty(infinityBlendShapes);
                    UpdateBlendShapes(currentModel);
                }
#endif

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(bodyColorPropertyName, HandleBodyColorChange);

                if (GetComponent<AtavismNode>().PropertyExists(bodyColorPropertyName))
                {
                    var item = GetComponent<AtavismNode>().GetProperty(bodyColorPropertyName);
                    string[] localItem = item.ToString().Split(',');
                    if (item != null && localItem == null)
                    {
                        Color32 color32 = (Color32)item;
                        UpdateBodyColor(color32);
                    }
                    else
                    {
                        Color32 color32 = new Color32(Convert.ToByte(localItem[0]), Convert.ToByte(localItem[1]), Convert.ToByte(localItem[2]), Convert.ToByte(localItem[3]));
                        UpdateBodyColor(color32);
                    }
                }


                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(bodyColorPropertyName, HandleBodyColorChange);

                if (GetComponent<AtavismNode>().PropertyExists(bodyColorPropertyName))
                {
                    var item = GetComponent<AtavismNode>().GetProperty(bodyColorPropertyName);
                    string[] localItem = item.ToString().Split(',');
                    if (item != null && localItem == null)
                    {
                        Color32 color32 = (Color32)item;
                        UpdateBodyColor(color32);
                    }
                    else
                    {
                        Color32 color32 = new Color32(Convert.ToByte(localItem[0]), Convert.ToByte(localItem[1]), Convert.ToByte(localItem[2]), Convert.ToByte(localItem[3]));
                        UpdateBodyColor(color32);
                    }
                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(scarColorPropertyName, HandleScarColorChange);

                if (GetComponent<AtavismNode>().PropertyExists(scarColorPropertyName))
                {
                    var item = GetComponent<AtavismNode>().GetProperty(scarColorPropertyName);
                    string[] localItem = item.ToString().Split(',');
                    if (item != null && localItem == null)
                    {
                        Color32 color32 = (Color32)item;
                        UpdateBodyScarColor(color32);
                    }
                    else
                    {
                        Color32 color32 = new Color32(Convert.ToByte(localItem[0]), Convert.ToByte(localItem[1]), Convert.ToByte(localItem[2]), Convert.ToByte(localItem[3]));
                        UpdateBodyScarColor(color32);
                    }
                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(mouthColorPropertyName, HandleMouthColorChange);

                if (GetComponent<AtavismNode>().PropertyExists(mouthColorPropertyName))
                {
                    var item = GetComponent<AtavismNode>().GetProperty(mouthColorPropertyName);
                    string[] localItem = item.ToString().Split(',');
                    if (item != null && localItem == null)
                    {
                        Color32 color32 = (Color32)item;
                        UpdateMouthColor(color32);
                    }
                    else
                    {
                        Color32 color32 = new Color32(Convert.ToByte(localItem[0]), Convert.ToByte(localItem[1]), Convert.ToByte(localItem[2]), Convert.ToByte(localItem[3]));
                        UpdateMouthColor(color32);
                    }
                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(hairColorPropertyName, HandleHairColorChange);

                if (GetComponent<AtavismNode>().PropertyExists(hairColorPropertyName))
                {
                    var item = GetComponent<AtavismNode>().GetProperty(hairColorPropertyName);
                    string[] localItem = item.ToString().Split(',');
                    if (item != null && localItem == null)
                    {
                        Color32 color32 = (Color32)item;
                        UpdateHairColor(color32);
                    }
                    else
                    {
                        Color32 color32 = new Color32(Convert.ToByte(localItem[0]), Convert.ToByte(localItem[1]), Convert.ToByte(localItem[2]), Convert.ToByte(localItem[3]));
                        UpdateHairColor(color32);
                    }


                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(stubbleColorPropertyName, HandleStubbleColorChange);

                if (GetComponent<AtavismNode>().PropertyExists(stubbleColorPropertyName))
                {
                    var item = GetComponent<AtavismNode>().GetProperty(stubbleColorPropertyName);
                    string[] localItem = item.ToString().Split(',');
                    if (item != null && localItem == null)
                    {
                        Color32 color32 = (Color32)item;
                        UpdateStubbleColor(color32);
                    }
                    else
                    {
                        Color32 color32 = new Color32(Convert.ToByte(localItem[0]), Convert.ToByte(localItem[1]), Convert.ToByte(localItem[2]), Convert.ToByte(localItem[3]));
                        UpdateStubbleColor(color32);
                    }

                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(bodyArtColorPropertyName, HandleBodyArtColorChange);

                if (GetComponent<AtavismNode>().PropertyExists(bodyArtColorPropertyName))
                {
                    var item = GetComponent<AtavismNode>().GetProperty(bodyArtColorPropertyName);
                    string[] localItem = item.ToString().Split(',');
                    if (item != null && localItem == null)
                    {
                        Color32 color32 = (Color32)item;
                        UpdateBodyArtColor(color32);
                    }
                    else
                    {
                        Color32 color32 = new Color32(Convert.ToByte(localItem[0]), Convert.ToByte(localItem[1]), Convert.ToByte(localItem[2]), Convert.ToByte(localItem[3]));
                        UpdateBodyArtColor(color32);
                    }

                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(eyeColorPropertyName, HandleEyeColorChange);

                if (GetComponent<AtavismNode>().PropertyExists(eyeColorPropertyName))
                {
                    var item = GetComponent<AtavismNode>().GetProperty(eyeColorPropertyName);
                    string[] localItem = item.ToString().Split(',');
                    if (item != null && localItem == null)
                    {
                        Color32 color32 = (Color32)item;
                        UpdateEyeColor(color32);
                    }
                    else
                    {
                        Color32 color32 = new Color32(Convert.ToByte(localItem[0]), Convert.ToByte(localItem[1]), Convert.ToByte(localItem[2]), Convert.ToByte(localItem[3]));
                        UpdateEyeColor(color32);
                    }
                }


                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(helmetColorPropertyName, HandleHelmetColorChange);

                if (GetComponent<AtavismNode>().PropertyExists(helmetColorPropertyName))
                {
                    var colorProperties = GetComponent<AtavismNode>().GetProperty(helmetColorPropertyName).ToString().Split('@');
                    var item = GetComponent<AtavismNode>().GetProperty(helmetColorPropertyName);
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');

                        if (item != null && coloritem == null)
                        {
                            Color32 color32 = (Color32)item;
                            UpdateShaderColor(color32, colorslot, BodyType.Helmet);
                        }
                        else
                        {
                            Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                            UpdateShaderColor(color32, colorslot, BodyType.Helmet);
                        }
                    }
                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(torsoColorPropertyName, HandleTorsoColorChange);

                if (GetComponent<AtavismNode>().PropertyExists(torsoColorPropertyName))
                {
                    var colorProperties = GetComponent<AtavismNode>().GetProperty(torsoColorPropertyName).ToString().Split('@');
                    var item = GetComponent<AtavismNode>().GetProperty(torsoColorPropertyName);
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');

                        if (item != null && coloritem == null)
                        {
                            Color32 color32 = (Color32)item;
                            UpdateShaderColor(color32, colorslot, BodyType.Torso);
                        }
                        else
                        {
                            Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                            UpdateShaderColor(color32, colorslot, BodyType.Torso);
                        }
                    }
                }


                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(upperArmsColorPropertyName, HandleUpperArmsColorChange);

                if (GetComponent<AtavismNode>().PropertyExists(upperArmsColorPropertyName))
                {
                    var colorProperties = GetComponent<AtavismNode>().GetProperty(upperArmsColorPropertyName).ToString().Split('@');
                    var item = GetComponent<AtavismNode>().GetProperty(upperArmsColorPropertyName);
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');

                        if (item != null && coloritem == null)
                        {
                            Color32 color32 = (Color32)item;
                            UpdateShaderColor(color32, colorslot, BodyType.Upperarms);
                        }
                        else
                        {
                            Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                            UpdateShaderColor(color32, colorslot, BodyType.Upperarms);
                        }
                    }
                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(lowerArmsColorPropertyName, HandleLowerArmsColorChange);

                if (GetComponent<AtavismNode>().PropertyExists(lowerArmsColorPropertyName))
                {
                    var colorProperties = GetComponent<AtavismNode>().GetProperty(lowerArmsColorPropertyName).ToString().Split('@');
                    var item = GetComponent<AtavismNode>().GetProperty(lowerArmsColorPropertyName);
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');

                        if (item != null && coloritem == null)
                        {
                            Color32 color32 = (Color32)item;
                            UpdateShaderColor(color32, colorslot, BodyType.LowerArms);
                        }
                        else
                        {
                            Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                            UpdateShaderColor(color32, colorslot, BodyType.LowerArms);
                        }
                    }
                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(hipsColorPropertyName, HandleHipsColorChange);

                if (GetComponent<AtavismNode>().PropertyExists(hipsColorPropertyName))
                {
                    var colorProperties = GetComponent<AtavismNode>().GetProperty(hipsColorPropertyName).ToString().Split('@');
                    var item = GetComponent<AtavismNode>().GetProperty(hipsColorPropertyName);
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');

                        if (item != null && coloritem == null)
                        {
                            Color32 color32 = (Color32)item;
                            UpdateShaderColor(color32, colorslot, BodyType.Hips);
                        }
                        else
                        {
                            Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                            UpdateShaderColor(color32, colorslot, BodyType.Hips);
                        }
                    }
                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(handsColorPropertyName, HandleHandsColorChange);

                if (GetComponent<AtavismNode>().PropertyExists(handsColorPropertyName))
                {
                    var colorProperties = GetComponent<AtavismNode>().GetProperty(handsColorPropertyName).ToString().Split('@');
                    var item = GetComponent<AtavismNode>().GetProperty(handsColorPropertyName);
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        if (item != null && coloritem == null)
                        {
                            Color32 color32 = (Color32)item;
                            UpdateShaderColor(color32, colorslot, BodyType.Hands);
                        }
                        else
                        {
                            Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                            UpdateShaderColor(color32, colorslot, BodyType.Hands);
                        }
                    }
                }

                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(lowerLegsColorPropertyName, HandleLowerLegsColorChange);

                if (GetComponent<AtavismNode>().PropertyExists(lowerLegsColorPropertyName))
                {
                    var colorProperties = GetComponent<AtavismNode>().GetProperty(lowerLegsColorPropertyName).ToString().Split('@');
                    var item = GetComponent<AtavismNode>().GetProperty(lowerLegsColorPropertyName);
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');

                        if (item != null && coloritem == null)
                        {
                            Color32 color32 = (Color32)item;
                            UpdateShaderColor(color32, colorslot, BodyType.LowerLegs);
                        }
                        else
                        {
                            Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                            UpdateShaderColor(color32, colorslot, BodyType.LowerLegs);
                        }
                    }
                }


                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(feetColorPropertyName, HandleFeetColorChange);

                if (GetComponent<AtavismNode>().PropertyExists(feetColorPropertyName))
                {
                    var colorProperties = GetComponent<AtavismNode>().GetProperty(feetColorPropertyName).ToString().Split('@');
                    var item = GetComponent<AtavismNode>().GetProperty(feetColorPropertyName);
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');

                        if (item != null && coloritem == null)
                        {
                            Color32 color32 = (Color32)item;
                            UpdateShaderColor(color32, colorslot, BodyType.Feet);
                        }
                        else
                        {
                            Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                            UpdateShaderColor(color32, colorslot, BodyType.Feet);
                        }
                    }
                }

            }
            // It's good practice to remove any property handlers when an object is being destroyed
            void OnDestroy()
            {
                if (GetComponent<AtavismNode>() != null)
                {
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(EyeMaterialPropertyName, HandleEyeMaterialChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(SkinMaterialPropertyName, HandleSkinMaterialChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(MouthMaterialPropertyName, HandleMouthMaterialChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(HairMaterialPropertyName, HandleHairMaterialChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(hairPropertyName.ToString(), HandleHairChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(beardPropertyName.ToString(), HandleBeardChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(mouthPropertyName.ToString(), HandleMouthChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(eyesPropertyName.ToString(), HandleEyesChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(earsPropertyName.ToString(), HandleEarsChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(tuskPropertyName.ToString(), HandleTusksChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(eyebrowPropertyName.ToString(), HandleEyebrowChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(headPropertyName, HandleHeadChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(faceTexPropertyName, HandleFaceChange);

                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(handsPropertyName, HandleHandsChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(lowerArmsPropertyName, HandleLowerArmsChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(upperArmsPropertyName, HandleUpperArmsChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(torsoPropertyName, HandleTorsoChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(hipsPropertyName, HandleHipsChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(lowerLegsPropertyName, HandleLowerLegsChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(feetPropertyName, HandleFeetChange);

                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(bodyColorPropertyName, HandleBodyColorChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(scarColorPropertyName, HandleScarColorChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(hairColorPropertyName, HandleHairColorChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(stubbleColorPropertyName, HandleStubbleColorChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(bodyArtColorPropertyName, HandleBodyArtColorChange);
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(eyeColorPropertyName, HandleEyeColorChange);

                    GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(helmetColorPropertyName, HandleHelmetColorChange);
                    GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(torsoColorPropertyName, HandleTorsoColorChange);
                    GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(upperArmsColorPropertyName, HandleUpperArmsColorChange);
                    GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(lowerArmsColorPropertyName, HandleLowerArmsColorChange);
                    GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(handsColorPropertyName, HandleHandsColorChange);
                    GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(hipsColorPropertyName, HandleHipsColorChange);
                    GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(lowerLegsColorPropertyName, HandleLowerLegsColorChange);
#if IPBRInt
                    if (!enableSavingInfinityPBRBlendshapes)
                    {
                        GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(blendshapePresetValue, HandleBlendShapePresetChange);
                    }
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(infinityBlendShapes, HandleBlendShapesChange);
#endif
                }
            }

            public BodyType ReturnBodyType(string sentBodyType)
            {
                BodyType bodyType = BodyType.None;
                switch (sentBodyType)
                {
                    case "Hips":
                        bodyType = BodyType.Hips;
                        break;
                    case "Torso":
                        bodyType = BodyType.Torso;
                        break;
                    case "Hands":
                        bodyType = BodyType.Hands;
                        break;
                    case "LowerLegs":
                        bodyType = BodyType.LowerLegs;
                        break;
                    case "Upperarms":
                        bodyType = BodyType.Upperarms;
                        break;
                    case "LowerArms":
                        bodyType = BodyType.LowerArms;
                        break;
                    case "Head":
                        bodyType = BodyType.Head;
                        break;
                    case "Helmet":
                        bodyType = BodyType.Head;
                        break;
                    case "Feet":
                        bodyType = BodyType.Feet;
                        break;
                }
                return bodyType;
            }

            //first Hair Model Change button
            public void SwitchHairForward()
            {
                switchHair++;
                if (switchHair == hairModels.Count)
                    switchHair = 0;
                UpdateHairModel(switchHair);
            }

            public void SwitchHairForward(int id)
            {
                switchHair = id;
                if (switchHair > hairModels.Count)
                    switchHair = hairModels.Count - 1;
                if (switchHair < 0)
                    switchHair = 0;
                UpdateHairModel(switchHair);
            }

            //Change to previous hair
            public void SwitchHairBack()
            {
                if (switchHair > 0)
                    switchHair--;
                else
                    switchHair = hairModels.Count - 1;
                UpdateHairModel(switchHair);
            }

            // First beard change button
            public void SwitchBeardForward()
            {
                switchBeard++;
                if (switchBeard == beardModels.Count)
                    switchBeard = 0;
                UpdateBeardModel(switchBeard);
            }
            public void SwitchBeardForward(int id)
            {
                switchBeard = id;
                if (switchBeard > beardModels.Count)
                    switchBeard = beardModels.Count - 1;
                if (switchBeard < 0)
                    switchBeard = 0;
                UpdateBeardModel(switchBeard);
            }

            // Go back to pervious beard selected
            public void SwitchBeardBack()
            {
                if (switchBeard > 0)
                    switchBeard--;
                else
                    switchBeard = beardModels.Count - 1;

                UpdateBeardModel(switchBeard);

            }

            //Eyebrow change button
            public void SwitchEyebrowForward()
            {
                switchEyebrow++;
                if (switchEyebrow == eyebrowModels.Count)
                    switchEyebrow = 0;
                UpdateEyebrowModel(switchEyebrow);
            }
            
            public void SwitchEyebrowForward(int id)
            {
                switchEyebrow = id;
                if (switchEyebrow > eyebrowModels.Count)
                    switchEyebrow = eyebrowModels.Count - 1;
                if (switchEyebrow < 0)
                    switchEyebrow = 0;
                UpdateEyebrowModel(switchEyebrow);
            }

            public void SwitchEyebrowBack()
            {
                if (switchEyebrow > 0)
                    switchEyebrow--;
                else
                    switchEyebrow = eyebrowModels.Count - 1;

                UpdateEyebrowModel(switchEyebrow);
            }

            //Eyebrow change button
            public void SwitchEarsForward()
            {
                switchEar++;
                if (switchEar == earModels.Count)
                    switchEar = 0;
                UpdateEarModel(switchEar);
            }
            public void SwitchEarsForward(int id)
            {
                switchEar = id;
                if (switchEar > earModels.Count)
                    switchEar = earModels.Count - 1;
                if (switchEar < 0)
                    switchEar = 0;
                UpdateEarModel(switchEar);
            }
            public void SwitchEarsBack()
            {
                if (switchEar > 0)
                    switchEar--;
                else
                    switchEar = earModels.Count - 1;

                UpdateEarModel(switchEar);
            }

            //Eyebrow change button
            public void SwitchTusksForward()
            {
                switchTusk++;
                if (switchTusk == tuskModels.Count)
                    switchTusk = 0;
                UpdateTuskModel(switchTusk);
            }
            
            public void SwitchTusksForward(int id)
            {
                switchTusk = id;
                if (switchTusk > tuskModels.Count)
                    switchTusk = tuskModels.Count - 1;
                if (switchTusk < 0)
                    switchTusk = 0;
                UpdateTuskModel(switchTusk);
            }
            
            public void SwitchTusksBack()
            {
                if (switchTusk > 0)
                    switchTusk--;
                else
                    switchTusk = tuskModels.Count - 1;

                UpdateTuskModel(switchTusk);
            }

            //Eyebrow change button
            public void SwitchMouthForward()
            {
                switchMouth++;
                if (switchMouth == mouthModels.Count)
                    switchMouth = 0;
                UpdateMouthModel(switchMouth);
            }
            
            public void SwitchMouthForward(int id)
            {
                switchMouth = id;
                if (switchMouth > mouthModels.Count)
                    switchMouth = mouthModels.Count - 1;
                if (switchMouth < 0)
                    switchMouth = 0;
                UpdateMouthModel(switchMouth);
            }
            
            public void SwitchMouthBack()
            {
                if (switchMouth > 0)
                    switchMouth--;
                else
                    switchMouth = mouthModels.Count - 1;

                UpdateMouthModel(switchMouth);
            }

            //Eyebrow change button
            public void SwitchEyeForward()
            {
                switchEye++;
                if (switchEye == eyeModels.Count)
                    switchEye = 0;
                UpdateEyeModel(switchEye);
            }
            public void SwitchEyeForward(int id)
            {
                switchEye = id;
                if (switchEye > eyeModels.Count)
                    switchEye = eyeModels.Count - 1;
                if (switchEye < 0)
                    switchEye = 0;
                UpdateEyeModel(switchEye);
            }
            public void SwitchEyeBack()
            {
                if (switchEye > 0)
                    switchEye--;
                else
                    switchEye = eyeModels.Count - 1;

                UpdateEyeModel(switchEye);
            }

            public void SkinColorSet(Color32 color32)
            {
                UpdateBodyColor(color32);
            }
            public void BeardColorSet(Color32 color32)
            {
                UpdateBeardColor(color32);
            }

            public void EyebrowColorSet(Color32 color32)
            {
                UpdateEyebrowColor(color32);
            }

            public void MouthColorSet(Color32 color32)
            {
                UpdateMouthColor(color32);
            }

            public void HairColorSet(Color32 color32)
            {
                UpdateHairColor(color32);
            }

            public void StubbleColorSet(Color32 color32)
            {
                UpdateStubbleColor(color32);
            }

            public void BodyArtColorSet(Color32 color32)
            {
                UpdateBodyArtColor(color32);
            }

            public void ScarColorSet(Color32 color32)
            {
                UpdateBodyScarColor(color32);
            }

            public void EyeColorSet(Color32 color32)
            {
                UpdateEyeColor(color32);
            }


            public void PrimaryColorSet(Color32 color32, BodyType bodyType)
            {
                UpdateShaderColor(color32, ShaderColorType.Primary, bodyType);
            }

            public void SecondaryColorSet(Color32 color32, BodyType bodyType)
            {
                UpdateShaderColor(color32, ShaderColorType.Secondary, bodyType);
            }

            public void TertiaryColorSet(Color32 color32, BodyType bodyType)
            {
                UpdateShaderColor(color32, ShaderColorType.Tertiary, bodyType);
            }

            public void MetalPrimaryColorSet(Color32 color32, BodyType bodyType)
            {
                UpdateShaderColor(color32, ShaderColorType.MetalPrimary, bodyType);
            }

            public void MetalSecondaryColorSet(Color32 color32, BodyType bodyType)
            {
                UpdateShaderColor(color32, ShaderColorType.MetalSecondary, bodyType);
            }

            public void MetalDarkColorSet(Color32 color32, BodyType bodyType)
            {
                UpdateShaderColor(color32, ShaderColorType.MetalDark, bodyType);
            }

            public void LeatherPrimaryColorSet(Color32 color32, BodyType bodyType)
            {
                UpdateShaderColor(color32, ShaderColorType.LeatherPrimary, bodyType);
            }

            public void LeatherSecondaryColorSet(Color32 color32, BodyType bodyType)
            {
                UpdateShaderColor(color32, ShaderColorType.LeatherSecondary, bodyType);
            }

            public void LeatherTertiaryColorSet(Color32 color32, BodyType bodyType)
            {
                UpdateShaderColor(color32, ShaderColorType.LeatherTertiary, bodyType);
            }

            public void SwitchFace(bool reverse = false)
            {
                if ((switchFace < 0))
                    switchFace = 0;

                if (faceModels.Count > switchFace)
                    if(faceModels[switchFace])  faceModels[switchFace].SetActive(false);
                if (reverse)
                {
                    switchFace--;
                    if (switchFace < 0)
                        switchFace = faceModels.Count - 1;
                }
                else
                {
                    switchFace++;
                    if (switchFace > faceModels.Count - 1)
                        switchFace = 0;
                }

                if (faceModels.Count > switchFace)
                {
                    if (faceModels[switchFace])
                    {
                        faceModels[switchFace].SetActive(true);
                        UpdateBodyModel(faceModels[switchFace].name, BodyType.Face);
                    }
                }
            }

            public void SwitchFace(int id)
            {
                if ((switchFace < 0))
                    switchFace = 0;

                if (faceModels.Count > switchFace)
                    if(faceModels[switchFace])  faceModels[switchFace].SetActive(false);
                switchFace = id;
                if (switchFace > faceModels.Count - 1)
                    switchFace = faceModels.Count - 1;
                if (switchFace < 0)
                    switchFace = 0;

                if (faceModels.Count > switchFace)
                {
                    if (faceModels[switchFace])
                    {
                        faceModels[switchFace].SetActive(true);
                        UpdateBodyModel(faceModels[switchFace].name, BodyType.Face);
                    }
                }
            }

            public void SwitchHead(bool reverse = false)
            {
                if ((switchHead < 0))
                    switchHead = 0;
                if (headModels.Count > switchHead)
                    if(headModels[switchHead]) headModels[switchHead].SetActive(false);
                if (reverse)
                {
                    switchHead--;
                    if (switchHead < 0)
                        switchHead = headModels.Count - 1;
                }
                else
                {
                    switchHead++;
                    if (switchHead > headModels.Count - 1)
                        switchHead = 0;
                }

                if (headModels.Count > switchHead)
                {
                    if (headModels[switchHead])
                    {
                        headModels[switchHead].SetActive(true);
                        UpdateBodyModel(headModels[switchHead].name, BodyType.Head);
                    }
                }
            }

            public void SwitchHead(int id)
            {
                if ((switchHead < 0))
                    switchHead = 0;

                if (headModels.Count > switchHead)
                    if(headModels[switchHead])  headModels[switchHead].SetActive(false);
                switchHead = id;
                if (switchHead > headModels.Count - 1)
                    switchHead = headModels.Count - 1;
                if (switchHead < 0)
                    switchHead = 0;

                if (headModels.Count > switchHead)
                {
                    if (headModels[switchHead])
                    {
                        headModels[switchHead].SetActive(true);
                        UpdateBodyModel(headModels[switchHead].name, BodyType.Head);
                    }
                }
            }

            public void SwitchHands(bool reverse = false)
            {
                if (handIndex < 0)
                    handIndex = 0;

                if (handModels.Count > handIndex)
                    if(handModels[handIndex]) handModels[handIndex].SetActive(false);
                if (reverse)
                {
                    handIndex--;
                    if (handIndex < 0)
                        handIndex = handModels.Count - 1;
                }
#if ModularNHance
                else if (nHanceModelSwapping)
                {
                    if (handIndex < 0)
                        handIndex = 0;

                    foreach (var go in ActiveHands)
                    {
                        Destroy(go);
                    }

                    if (reverse)
                    {
                        handIndex--;
                        if (handIndex < 0)
                            handIndex = handModels.Count - 1;
                    }
                    else
                    {
                        handIndex++;
                        if (handIndex > handModels.Count - 1)
                            handIndex = 0;
                    }

                    ActiveHands = enableNHance(handModels[handIndex]);

                }
#endif
                else
                {
                    handIndex++;
                    if (handIndex > handModels.Count - 1)
                        handIndex = 0;
                }

                if (handModels.Count > handIndex)
                {
                    if (handModels[handIndex])
                    {
                        handModels[handIndex].SetActive(true);
                        UpdateBodyModel(handModels[handIndex].name, BodyType.Hands);
                    }
                }
            }

            public void SwitchHands(int id)
            {
                if (handIndex < 0)
                    handIndex = 0;

                if (handModels.Count > handIndex)
                    if(handModels[handIndex])
                    handModels[handIndex].SetActive(false);

#if ModularNHance
                 if (nHanceModelSwapping)
                {
                    if (handIndex < 0)
                        handIndex = 0;

                    foreach (var go in ActiveHands)
                    {
                        Destroy(go);
                    }
                   

                   handIndex = id;
                  if (handIndex > handModels.Count - 1)
                      handIndex = handModels.Count - 1;
                  if (handIndex < 0)
                      handIndex = handModels.Count - 1;

                    ActiveHands = enableNHance(handModels[handIndex]);

                }else
#endif
                {
                    handIndex = id;
                    if (handIndex > handModels.Count - 1)
                        handIndex = handModels.Count - 1;
                    if (handIndex < 0)
                        handIndex = handModels.Count - 1;
                }
                if (handModels.Count > handIndex)
                {
                    if (handModels[handIndex])
                    {
                        handModels[handIndex].SetActive(true);
                        UpdateBodyModel(handModels[handIndex].name, BodyType.Hands);
                    }
                }
            }

            public void SwitchLowerArm(bool reverse = false)
            {
                if ((lowerArmIndex < 0))
                    lowerArmIndex = 0;
                if (lowerArmModels.Count > lowerArmIndex)
                    if(lowerArmModels[lowerArmIndex])   lowerArmModels[lowerArmIndex].SetActive(false);
                if (reverse)
                {
                    lowerArmIndex--;
                    if (lowerArmIndex < 0)
                        lowerArmIndex = lowerArmModels.Count - 1;
                }
                else
                {
                    lowerArmIndex++;
                    if (lowerArmIndex > lowerArmModels.Count - 1)
                        lowerArmIndex = 0;
                }

                if (lowerArmModels.Count > lowerArmIndex)
                {
                    if (lowerArmModels[lowerArmIndex])
                    {
                        lowerArmModels[lowerArmIndex].SetActive(true);
                        UpdateBodyModel(lowerArmModels[lowerArmIndex].name, BodyType.LowerArms);
                    }
                }
            }

            public void SwitchLowerArm(int id)
            {
                if ((lowerArmIndex < 0))
                    lowerArmIndex = 0;

                if (lowerArmModels.Count > lowerArmIndex)
                    if(lowerArmModels[lowerArmIndex])   lowerArmModels[lowerArmIndex].SetActive(false);
                lowerArmIndex = id;
                if (lowerArmIndex > lowerArmModels.Count - 1)
                    lowerArmIndex = lowerArmModels.Count - 1;
                if (lowerArmIndex < 0)
                    lowerArmIndex = 0;

                if (lowerArmModels.Count > lowerArmIndex)
                {
                    if (lowerArmModels[lowerArmIndex])
                    {
                        lowerArmModels[lowerArmIndex].SetActive(true);
                        UpdateBodyModel(lowerArmModels[lowerArmIndex].name, BodyType.LowerArms);
                    }
                }
            }

            public void SwitchUpperArm(bool reverse = false)
            {
                if (upperArmIndex < 0)
                    upperArmIndex = 0;

                if (upperArmModels.Count > upperArmIndex)
                    if(upperArmModels[upperArmIndex]) upperArmModels[upperArmIndex].SetActive(false);

                if (reverse)
                {
                    upperArmIndex--;
                    if (upperArmIndex < 0)
                        upperArmIndex = upperArmModels.Count - 1;
                }
                else
                {
                    upperArmIndex++;
                    if (upperArmIndex > upperArmModels.Count - 1)
                        upperArmIndex = 0;
                }

                if (upperArmModels.Count > upperArmIndex)
                {
                    if (upperArmModels[upperArmIndex])
                    {
                        upperArmModels[upperArmIndex].SetActive(true);
                        UpdateBodyModel(upperArmModels[upperArmIndex].name, BodyType.Upperarms);
                    }
                }
            }

            public void SwitchUpperArm(int id)
            {
                if (upperArmIndex < 0)
                    upperArmIndex = 0;
                if (upperArmModels.Count > upperArmIndex)
                    if(upperArmModels[upperArmIndex]) upperArmModels[upperArmIndex].SetActive(false);

                upperArmIndex = id;
                if (upperArmIndex > upperArmModels.Count - 1)
                    upperArmIndex = upperArmModels.Count - 1;
                if (upperArmIndex < 0)
                    upperArmIndex = 0;
                if (upperArmModels.Count > upperArmIndex)
                {
                    if (upperArmModels[upperArmIndex])
                    {
                        upperArmModels[upperArmIndex].SetActive(true);
                        UpdateBodyModel(upperArmModels[upperArmIndex].name, BodyType.Upperarms);
                    }
                }
            
            }

            public void SwitchTorso(bool reverse = false)
            {
                if (modularModelSwapping)
                {
                    if (replacementIndex < 0)
                        replacementIndex = 0;
                    //replacementItems[replacementIndex].SetActive(false);

                    if (reverse)
                    {
                        replacementIndex--;
                        if (replacementIndex < 0)
                            replacementIndex = replacementItems.Count - 1;
                    }
                    else
                    {
                        replacementIndex++;
                        if (replacementIndex > replacementItems.Count - 1)
                            replacementIndex = 0;
                    }

                    //replacementItems[replacementIndex].SetActive(true);
                    UpdateBodyModel(replacementItems[replacementIndex].name, BodyType.Torso);
                }
#if ModularNHance
                else if (nHanceModelSwapping)
                {
                    if (torsoIndex < 0)
                        torsoIndex = 0;
                  
                    foreach (var go in ActiveTorso)
                    {
                        Destroy(go);
                    }
                    
                    if (reverse)
                    {
                        torsoIndex--;
                        if (torsoIndex < 0)
                            torsoIndex = torsoModels.Count - 1;
                    }
                    else
                    {
                        torsoIndex++;
                        if (torsoIndex > torsoModels.Count - 1)
                            torsoIndex = 0;
                    }

                    ActiveTorso = enableNHance(torsoModels[torsoIndex]);

                }
#endif
                else
                {
                    if (torsoIndex < 0)
                        torsoIndex = 0;
                    if (torsoModels.Count > torsoIndex)
                        if(torsoModels[torsoIndex])   torsoModels[torsoIndex].SetActive(false);
                    if (reverse)
                    {
                        torsoIndex--;
                        if (torsoIndex < 0)
                            torsoIndex = torsoModels.Count - 1;
                    }
                    else
                    {
                        torsoIndex++;
                        if (torsoIndex > torsoModels.Count - 1)
                            torsoIndex = 0;
                    }

                    if (torsoModels.Count > torsoIndex)
                    {
                        if (torsoModels[torsoIndex])
                        {
                            torsoModels[torsoIndex].SetActive(true);
                            UpdateBodyModel(torsoModels[torsoIndex].name, BodyType.Torso);
                        }
                    }
                }
            }

            public void SwitchTorso(int id)
            {
                if (modularModelSwapping)
                {
                    if (replacementIndex < 0)
                        replacementIndex = 0;
                    //replacementItems[replacementIndex].SetActive(false);

                    replacementIndex = id;
                    if (replacementIndex > replacementItems.Count - 1)
                        replacementIndex = 0;
                    if (replacementIndex < 0)
                        replacementIndex = replacementItems.Count - 1;

                    //replacementItems[replacementIndex].SetActive(true);
                    UpdateBodyModel(replacementItems[replacementIndex].name, BodyType.Torso);
                }
#if ModularNHance
                else if (nHanceModelSwapping)
                {
                    if (torsoIndex < 0)
                        torsoIndex = 0;

                    foreach (var go in ActiveTorso)
                    {
                        Destroy(go);
                    }

                        torsoIndex = id;
                        if (torsoIndex > torsoModels.Count - 1)
                            torsoIndex = torsoModels.Count - 1;
                        if (torsoIndex < 0)
                            torsoIndex = 0;


                    ActiveTorso = enableNHance(torsoModels[torsoIndex]);

                }
#endif
                else
                {
                    if (torsoIndex < 0)
                        torsoIndex = 0;
                    if (torsoModels.Count > torsoIndex)
                        if(torsoModels[torsoIndex])  torsoModels[torsoIndex].SetActive(false);

                    torsoIndex = id;
                    if (torsoIndex > torsoModels.Count - 1)
                        torsoIndex = torsoModels.Count - 1;
                    if (torsoIndex < 0)
                        torsoIndex = 0;

                    if (torsoModels.Count > torsoIndex)
                    {
                        if (torsoModels[torsoIndex])
                        {
                            torsoModels[torsoIndex].SetActive(true);
                            UpdateBodyModel(torsoModels[torsoIndex].name, BodyType.Torso);
                        }
                    }
                }
            }

            public void SwitchHips(bool reverse = false)
            {
                if ((hipsIndex < 0))
                    hipsIndex = 0;

                if (hipModels.Count > hipsIndex)
                    if(hipModels[hipsIndex])  hipModels[hipsIndex].SetActive(false);
                if (reverse)
                {
                    hipsIndex--;
                    if (hipsIndex < 0)
                        hipsIndex = hipModels.Count - 1;
                }
#if ModularNHance
                else if (nHanceModelSwapping)
                {
                    if (hipsIndex < 0)
                        hipsIndex = 0;

                    foreach (var go in ActiveHips)
                    {
                        Destroy(go);
                    }

                    if (reverse)
                    {
                        hipsIndex--;
                        if (hipsIndex < 0)
                            hipsIndex = hipModels.Count - 1;
                    }
                    else
                    {
                        hipsIndex++;
                        if (hipsIndex > hipModels.Count - 1)
                            hipsIndex = 0;
                    }

                    ActiveHips = enableNHance(hipModels[hipsIndex]);
                }
#endif
                else
                {
                    hipsIndex++;
                    if (hipsIndex > hipModels.Count - 1)
                        hipsIndex = 0;
                }

                if (hipModels.Count > hipsIndex)
                {
                    if (hipModels[hipsIndex])
                    {
                        hipModels[hipsIndex].SetActive(true);
                        UpdateBodyModel(hipModels[hipsIndex].name, BodyType.Hips);
                    }
                }
            }

            public void SwitchHips(int id)
            {
                if ((hipsIndex < 0))
                    hipsIndex = 0;

                if (hipModels.Count > hipsIndex)
                    if(hipModels[hipsIndex])  hipModels[hipsIndex].SetActive(false);

#if ModularNHance
                 if (nHanceModelSwapping)
                {
                    if (hipsIndex < 0)
                        hipsIndex = 0;

                    foreach (var go in ActiveHips)
                    {
                        Destroy(go);
                    }

                    hipsIndex = id;
                    if (hipsIndex > hipModels.Count - 1)
                        hipsIndex = hipModels.Count - 1;
                    if (hipsIndex < 0)
                        hipsIndex = 0;

                    ActiveHips = enableNHance(hipModels[hipsIndex]);
                }else
#endif

                {
                    hipsIndex = id;
                    if (hipsIndex > hipModels.Count - 1)
                        hipsIndex = hipModels.Count - 1;
                    if (hipsIndex < 0)
                        hipsIndex = 0;
                }

                if (hipModels.Count > hipsIndex)
                {
                    if (hipModels[hipsIndex])
                    {
                        hipModels[hipsIndex].SetActive(true);
                        UpdateBodyModel(hipModels[hipsIndex].name, BodyType.Hips);
                    }
                }
            }


#if IPBRInt
            public void SwitchBlendshapePreset(bool reverse = false)
            {
                if ((blendshapePresetObjectsIndex < 0))
                    blendshapePresetObjectsIndex = 0;
                if (reverse)
                {
                    blendshapePresetObjectsIndex--;
                    if (blendshapePresetObjectsIndex < 0)
                        blendshapePresetObjectsIndex = blendshapePresetObjects.Count - 1;
                }
                else
                {
                    blendshapePresetObjectsIndex++;
                    if (blendshapePresetObjectsIndex > blendshapePresetObjects.Count - 1)
                        blendshapePresetObjectsIndex = 0;
                }

                UpdateBlendShapePresets(blendshapePresetObjectsIndex);
            }
#endif

            public void SwitchLowerLegs(bool reverse = false)
            {
                if (lowerLegIndex < 0)
                    lowerLegIndex = 0;
                if (lowerLegModels.Count > lowerLegIndex)
                    if(lowerLegModels[lowerLegIndex]) lowerLegModels[lowerLegIndex].SetActive(false);
#if ModularNHance
                if (nHanceModelSwapping)
                {
                    if (lowerLegIndex < 0)
                        lowerLegIndex = 0;

                    foreach (var go in ActiveLowerLegs)
                    {
                        Destroy(go);
                    }

                    if (reverse)
                    {
                        lowerLegIndex--;
                        if (lowerLegIndex < 0)
                            lowerLegIndex = lowerLegModels.Count - 1;
                    }
                    else
                    {
                        lowerLegIndex++;
                        if (lowerLegIndex > lowerLegModels.Count - 1)
                            lowerLegIndex = 0;
                    }

                    ActiveLowerLegs = enableNHance(lowerLegModels[hipsIndex]);
                }
                else
#endif
                if (reverse)
                {
                    lowerLegIndex--;
                    if (lowerLegIndex < 0)
                        lowerLegIndex = lowerLegModels.Count - 1;
                }
                else
                {
                    lowerLegIndex++;
                    if (lowerLegIndex > lowerLegModels.Count - 1)
                        lowerLegIndex = 0;
                }

                if (lowerLegModels.Count > lowerLegIndex)
                {
                    if (lowerLegModels[lowerLegIndex])
                    {
                        lowerLegModels[lowerLegIndex].SetActive(true);
                        UpdateBodyModel(lowerLegModels[lowerLegIndex].name, BodyType.LowerLegs);
                    }
                }
            }

            public void SwitchLowerLegs(int id)
            {

                if (lowerLegIndex < 0)
                    lowerLegIndex = 0;

                if (lowerLegModels.Count > lowerLegIndex)
                    if(lowerLegModels[lowerLegIndex]) lowerLegModels[lowerLegIndex].SetActive(false);
#if ModularNHance
                if (nHanceModelSwapping)
                {
                    if (lowerLegIndex < 0)
                        lowerLegIndex = 0;

                    foreach (var go in ActiveLowerLegs)
                    {
                        Destroy(go);
                    }

                     lowerLegIndex = id;
                    if (lowerLegIndex < 0)
                        lowerLegIndex = 0;
                    if (lowerLegIndex > lowerLegModels.Count - 1)
                        lowerLegIndex = lowerLegModels.Count - 1;

                    ActiveLowerLegs = enableNHance(lowerLegModels[hipsIndex]);
                }
                else
#endif
                {
                    lowerLegIndex = id;
                    if (lowerLegIndex < 0)
                        lowerLegIndex = 0;
                    if (lowerLegIndex > lowerLegModels.Count - 1)
                        lowerLegIndex = lowerLegModels.Count - 1;
                }

                if (lowerLegModels.Count > lowerLegIndex)
                {
                    if (lowerLegModels[lowerLegIndex])
                    {
                        lowerLegModels[lowerLegIndex].SetActive(true);
                        UpdateBodyModel(lowerLegModels[lowerLegIndex].name, BodyType.LowerLegs);
                    }
                }
            }

            public void SwitchFeet(bool reverse = false)
            {
                if (feetIndex < 0)
                    feetIndex = 0;
                if (feetModels.Count > feetIndex)
                    if(feetModels[feetIndex]) feetModels[feetIndex].SetActive(false);
                if (reverse)
                {
                    feetIndex--;
                    if (feetIndex < 0)
                        feetIndex = feetModels.Count - 1;
                }
#if ModularNHance
                else if (nHanceModelSwapping)
                {
                    if (feetIndex < 0)
                        feetIndex = 0;

                    foreach (var go in ActiveFeet)
                    {
                        Destroy(go);
                    }

                    if (reverse)
                    {
                        feetIndex--;
                        if (feetIndex < 0)
                            feetIndex = feetModels.Count - 1;
                    }
                    else
                    {
                        feetIndex++;
                        if (feetIndex > feetModels.Count - 1)
                            feetIndex = 0;
                    }

                    ActiveFeet = enableNHance(feetModels[feetIndex]);

                }
#endif
                else
                {
                    feetIndex++;
                    if (feetIndex > feetModels.Count - 1)
                        feetIndex = 0;
                }

                if (feetModels.Count > feetIndex)
                {
                    if (feetModels[feetIndex])
                    {
                        feetModels[feetIndex].SetActive(true);
                        UpdateBodyModel(feetModels[feetIndex].name, BodyType.Feet);
                    }
                }
            }

            public void SwitchFeet(int id)
            {
                if (feetIndex < 0)
                    feetIndex = 0;

                if (feetModels.Count > feetIndex)
                    if(feetModels[feetIndex]) feetModels[feetIndex].SetActive(false);

#if ModularNHance
                else if (nHanceModelSwapping)
                {
                    if (feetIndex < 0)
                        feetIndex = 0;

                    foreach (var go in ActiveFeet)
                    {
                        Destroy(go);
                    }

                    feetIndex = id;
                    if (feetIndex > feetModels.Count - 1)
                        feetIndex = 0;
                    if (feetIndex < 0)
                        feetIndex = feetModels.Count - 1;

                    ActiveFeet = enableNHance(feetModels[feetIndex]);

                }else

#endif
                {
                    feetIndex = id;
                    if (feetIndex > feetModels.Count - 1)
                        feetIndex = 0;
                    if (feetIndex < 0)
                        feetIndex = feetModels.Count - 1;
                }

                if (feetModels.Count > feetIndex)
                {
                    if (feetModels[feetIndex])
                    {
                        feetModels[feetIndex].SetActive(true);
                        UpdateBodyModel(feetModels[feetIndex].name, BodyType.Feet);
                    }
                }
            }

            void ReplaceModelShaders(Material maerialToChangeFrom, Material maerialToSwitchTo, Shader shaderToUse)
            {
                Transform[] rootTransform = GetComponentsInChildren<Transform>(true);

                // cycle through all child objects of the parent object
                for (int i = 0; i < rootTransform.Length; i++)
                {
                    // get child gameobject index i
                    GameObject go = rootTransform[i].gameObject;

                    SkinnedMeshRenderer skinnedMeshRenderer = go.GetComponent<SkinnedMeshRenderer>();
                    MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
                    Material[] materials = null;

                    if (skinnedMeshRenderer)
                    {
                        materials = skinnedMeshRenderer.materials;
                    }
                    else if (meshRenderer)
                    {
                        materials = meshRenderer.materials;
                    }

                    if (skinnedMeshRenderer || meshRenderer)
                    {
                        int materialCount = -1;

                        foreach (var item in materials)
                        {
                            materialCount++;
                            if (item.name.Contains(maerialToChangeFrom.name))
                            {
                                materials[materialCount] = maerialToSwitchTo;
                            }
                            if (skinnedMeshRenderer)
                            {
                                skinnedMeshRenderer.materials = materials;
                            }
                            else
                            {
                                meshRenderer.materials = materials;
                            }
                        }
                    }
                }
            }

            public void SwitchMaterialWorker(Material maerialToChangeFrom, Material maerialToSwitchTo, Shader shaderToUse, string materialName, BodyType thisBodyType)
            {
                Material tempMaterial = new Material(shaderToUse);
                tempMaterial.enableInstancing = enableInstancing;
                tempMaterial.CopyPropertiesFromMaterial(maerialToSwitchTo);
                tempMaterial.name = materialName + myGUID;
                if (thisBodyType == BodyType.Hair)
                {
                    internalHairMat = tempMaterial;
                    internalBeardMat = tempMaterial;
                    internalEyebrowMat = tempMaterial;
                    ActiveHairColor = tempMaterial.color;
                }
                else if (thisBodyType == BodyType.Skin)
                {
                    internalMat = tempMaterial;
                    ActiveBodyColor = tempMaterial.color;
                }
                else if (thisBodyType == BodyType.Eye)
                {
                    internalEyeMat = tempMaterial;
                    ActiveEyeColor = tempMaterial.color;
                }
                else if (thisBodyType == BodyType.Mouth)
                {
                    internalMouthMat = tempMaterial;
                    ActiveMouthColor = tempMaterial.color;
                }
                ReplaceModelShaders(maerialToChangeFrom, tempMaterial, shaderToUse);
            }

            public void SwitchMaterial(BodyType thistype, bool reverse = false)
            {
                switch (thistype)
                {
                    case BodyType.Hair:
                        if (hairMaterialIndex < 0)
                            hairMaterialIndex = 0;
                        if (reverse)
                        {
                            hairMaterialIndex--;
                            if (hairMaterialIndex < 0)
                                hairMaterialIndex = hairMatList.Count - 1;
                        }
                        else
                        {
                            hairMaterialIndex++;
                            if (hairMaterialIndex > hairMatList.Count - 1)
                                hairMaterialIndex = 0;
                        }
                        ActiveHairMaterialId = hairMaterialIndex;
                        SwitchMaterialWorker(internalBeardMat, hairMatList[hairMaterialIndex], hairShader, "internalBeardMat", thistype);
                        SwitchMaterialWorker(internalEyebrowMat, hairMatList[hairMaterialIndex], hairShader, "internalEyebrowMat", thistype);
                        SwitchMaterialWorker(internalHairMat, hairMatList[hairMaterialIndex], hairShader, "internalHairMat", thistype);

                        break;

                    case BodyType.Eye:
                        if (eyeMaterialIndex < 0)
                            eyeMaterialIndex = 0;

                        if (reverse)
                        {
                            eyeMaterialIndex--;
                            if (eyeMaterialIndex < 0)
                                eyeMaterialIndex = eyeMatList.Count - 1;
                        }
                        else
                        {
                            eyeMaterialIndex++;
                            if (eyeMaterialIndex > eyeMatList.Count - 1)
                                eyeMaterialIndex = 0;
                        }
                        ActiveEyeMaterialId = eyeMaterialIndex;
                        SwitchMaterialWorker(internalEyeMat, eyeMatList[eyeMaterialIndex], eyeShader, "internalEyeMat", thistype);

                        break;

                    case BodyType.Skin:
                        if (skinMaterialIndex < 0)
                            skinMaterialIndex = 0;
                        if (reverse)
                        {
                            skinMaterialIndex--;
                            if (skinMaterialIndex < 0)
                                skinMaterialIndex = bodyMatList.Count - 1;
                        }
                        else
                        {
                            skinMaterialIndex++;
                            if (skinMaterialIndex > bodyMatList.Count - 1)
                                skinMaterialIndex = 0;
                        }
                        ActiveSkinMaterialId = skinMaterialIndex;
                        SwitchMaterialWorker(internalMat, bodyMatList[skinMaterialIndex], bodyShader, "internalMat", thistype);
                        break;


                    case BodyType.Mouth:
                        if (mouthMaterialIndex < 0)
                            mouthMaterialIndex = 0;
                        if (reverse)
                        {
                            mouthMaterialIndex--;
                            if (mouthMaterialIndex < 0)
                                mouthMaterialIndex = bodyMatList.Count - 1;
                        }
                        else
                        {
                            mouthMaterialIndex++;
                            if (mouthMaterialIndex > bodyMatList.Count - 1)
                                mouthMaterialIndex = 0;
                        }
                        ActiveMouthMaterialId = mouthMaterialIndex;
                        SwitchMaterialWorker(internalMat, bodyMatList[mouthMaterialIndex], bodyShader, "internalMouthMat", thistype);
                        break;
                }
            }

            public void SwitchMaterial(BodyType thistype, int id)
            {
                switch (thistype)
                {
                    case BodyType.Hair:
                        hairMaterialIndex = id;
                        if (hairMaterialIndex > hairMatList.Count - 1)
                            hairMaterialIndex = hairMatList.Count - 1;
                        if (hairMaterialIndex < 0)
                            hairMaterialIndex = 0;
                        ActiveHairMaterialId = hairMaterialIndex;
                        SwitchMaterialWorker(internalBeardMat, hairMatList[hairMaterialIndex], hairShader, "internalBeardMat", thistype);
                        SwitchMaterialWorker(internalEyebrowMat, hairMatList[hairMaterialIndex], hairShader, "internalEyebrowMat", thistype);
                        SwitchMaterialWorker(internalHairMat, hairMatList[hairMaterialIndex], hairShader, "internalHairMat", thistype);

                        break;

                    case BodyType.Eye:
                        eyeMaterialIndex = id;
                        if (eyeMaterialIndex > eyeMatList.Count - 1)
                            eyeMaterialIndex = 0;
                        if (eyeMaterialIndex < 0)
                            eyeMaterialIndex = eyeMatList.Count - 1;
                        ActiveEyeMaterialId = eyeMaterialIndex;
                        SwitchMaterialWorker(internalEyeMat, eyeMatList[eyeMaterialIndex], eyeShader, "internalEyeMat", thistype);

                        break;

                    case BodyType.Skin:
                        skinMaterialIndex = id;
                        if (skinMaterialIndex > bodyMatList.Count - 1)
                            skinMaterialIndex = bodyMatList.Count - 1;
                        if (skinMaterialIndex < 0)
                            skinMaterialIndex = 0;
                        ActiveSkinMaterialId = skinMaterialIndex;
                        SwitchMaterialWorker(internalMat, bodyMatList[skinMaterialIndex], bodyShader, "internalMat", thistype);
                        break;


                    case BodyType.Mouth:
                        mouthMaterialIndex = id;
                        if (mouthMaterialIndex > bodyMatList.Count - 1)
                            mouthMaterialIndex = bodyMatList.Count - 1;
                        if (mouthMaterialIndex < 0)
                            mouthMaterialIndex = 0;
                        ActiveMouthMaterialId = mouthMaterialIndex;
                        SwitchMaterialWorker(internalMat, bodyMatList[mouthMaterialIndex], bodyShader, "internalMouthMat", thistype);
                        break;
                }
            }

            // This will run when the game gets a new property - it will get the property value then run the UpdateModel function.
            public void HandleHairChange(object sender, PropertyChangeEventArgs args)
            {
                int hairModel = (int)GetComponent<AtavismNode>().GetProperty(hairPropertyName);
                UpdateHairModel(switchHair);
            }

            public void HandleEyeMaterialChange(object sender, PropertyChangeEventArgs args)
            {
                int ModelValue = (int)GetComponent<AtavismNode>().GetProperty(EyeMaterialPropertyName);
                if (eyeMatList.Count > 0)
                {
                    SwitchMaterialWorker(internalEyeMat, eyeMatList[ModelValue], hairShader, "internalEyeMat", BodyType.Eye);
                }
            }
            public void HandleSkinMaterialChange(object sender, PropertyChangeEventArgs args)
            {
                int ModelValue = (int)GetComponent<AtavismNode>().GetProperty(SkinMaterialPropertyName);
                if (bodyMatList.Count > 0)
                {
                    SwitchMaterialWorker(internalMat, bodyMatList[ModelValue], bodyShader, "internalMat", BodyType.Skin);
                }                
            }

            public void HandleMouthMaterialChange(object sender, PropertyChangeEventArgs args)
            {
                int ModelValue = (int)GetComponent<AtavismNode>().GetProperty(MouthMaterialPropertyName);
                if (mouthMatList.Count > 0)
                {
                    SwitchMaterialWorker(mouthMat, mouthMatList[ModelValue], mouthShader, "internalMouthMat", BodyType.Mouth);
                }
            }

            
            public void HandleHairMaterialChange(object sender, PropertyChangeEventArgs args)
            {
                int ModelValue = (int)GetComponent<AtavismNode>().GetProperty(HairMaterialPropertyName);
                if (hairMatList.Count > 0)
                {
                    SwitchMaterialWorker(internalHairMat, hairMatList[ModelValue], hairShader, "internalHairMat", BodyType.Hair);
                }

            }

            public void HandleBeardChange(object sender, PropertyChangeEventArgs args)
            {
                int beardModel = (int)GetComponent<AtavismNode>().GetProperty(beardPropertyName);
                UpdateBeardModel(switchBeard);
            }
            public void HandleEyebrowChange(object sender, PropertyChangeEventArgs args)
            {
                int eyeBrowModel = (int)GetComponent<AtavismNode>().GetProperty(eyebrowPropertyName);
                UpdateEyebrowModel(switchEyebrow);
            }

            public void HandleHeadChange(object sender, PropertyChangeEventArgs args)
            {
                string bodyPart = (string)GetComponent<AtavismNode>().GetProperty(headPropertyName);
                UpdateBodyModel(bodyPart, BodyType.Head);
            }

            public void HandleFaceChange(object sender, PropertyChangeEventArgs args)
            {
                string bodyPart = (string)GetComponent<AtavismNode>().GetProperty(faceTexPropertyName);
                UpdateBodyModel(bodyPart, BodyType.Face);
            }

            public void HandleHandsChange(object sender, PropertyChangeEventArgs args)
            {
                string bodyPart = (string)GetComponent<AtavismNode>().GetProperty(handsPropertyName);
                UpdateBodyModel(bodyPart, BodyType.Hands);
            }
            public void HandleLowerArmsChange(object sender, PropertyChangeEventArgs args)
            {
                string bodyPart = (string)GetComponent<AtavismNode>().GetProperty(lowerArmsPropertyName);
                UpdateBodyModel(bodyPart, BodyType.LowerArms);
            }
            public void HandleUpperArmsChange(object sender, PropertyChangeEventArgs args)
            {
                string bodyPart = (string)GetComponent<AtavismNode>().GetProperty(upperArmsPropertyName);
                UpdateBodyModel(bodyPart, BodyType.Upperarms);
            }
            public void HandleTorsoChange(object sender, PropertyChangeEventArgs args)
            {
                string bodyPart = (string)GetComponent<AtavismNode>().GetProperty(torsoPropertyName);
                UpdateBodyModel(bodyPart, BodyType.Torso);
            }
            public void HandleHipsChange(object sender, PropertyChangeEventArgs args)
            {
                string bodyPart = (string)GetComponent<AtavismNode>().GetProperty(hipsPropertyName);
                UpdateBodyModel(bodyPart, BodyType.Hips);
            }
            public void HandleLowerLegsChange(object sender, PropertyChangeEventArgs args)
            {
                string bodyPart = (string)GetComponent<AtavismNode>().GetProperty(lowerLegsPropertyName);
                UpdateBodyModel(bodyPart, BodyType.LowerLegs);
            }

            public void HandleFeetChange(object sender, PropertyChangeEventArgs args)
            {
                string bodyPart = (string)GetComponent<AtavismNode>().GetProperty(feetPropertyName);
                UpdateBodyModel(bodyPart, BodyType.Feet);
            }

            public void HandleMouthChange(object sender, PropertyChangeEventArgs args)
            {
                int bodyPart = (int)GetComponent<AtavismNode>().GetProperty(mouthPropertyName);
                UpdateMouthModel(bodyPart);
            }
            public void HandleEyesChange(object sender, PropertyChangeEventArgs args)
            {
                int bodyPart = (int)GetComponent<AtavismNode>().GetProperty(eyesPropertyName);
                UpdateEyeModel(bodyPart);
            }
            public void HandleEarsChange(object sender, PropertyChangeEventArgs args)
            {
                int bodyPart = (int)GetComponent<AtavismNode>().GetProperty(earsPropertyName);
                UpdateEarModel(bodyPart);
            }
#if IPBRInt
            public void HandleBlendShapesChange(object sender, PropertyChangeEventArgs args)
            {
                string bodyPart = (string)GetComponent<AtavismNode>().GetProperty(infinityBlendShapes);
                UpdateBlendShapes(bodyPart);
            }

            public void HandleBlendShapePresetChange(object sender, PropertyChangeEventArgs args)
            {
                int bodyPart = (int)GetComponent<AtavismNode>().GetProperty(blendshapePresetValue);
                UpdateBlendShapePresets(bodyPart);
            }
#endif
            public void HandleTusksChange(object sender, PropertyChangeEventArgs args)
            {
                int bodyPart = (int)GetComponent<AtavismNode>().GetProperty(tuskPropertyName);
                UpdateTuskModel(bodyPart);
            }

            public void HandleBodyColorChange(object sender, PropertyChangeEventArgs args)
            {
                var item = GetComponent<AtavismNode>().GetProperty(bodyColorPropertyName);
                string[] localItem = item.ToString().Split(',');
                if (item != null && localItem == null)
                {

                    Color32 color32 = (Color32)item;
                    UpdateBodyColor(color32);
                }
                else
                {
                    Color32 color32 = new Color32(Convert.ToByte(localItem[0]), Convert.ToByte(localItem[1]), Convert.ToByte(localItem[2]), Convert.ToByte(localItem[3]));
                    UpdateBodyColor(color32);
                }
            }

            public void HandleBodyArtColorChange(object sender, PropertyChangeEventArgs args)
            {
                var item = GetComponent<AtavismNode>().GetProperty(bodyArtColorPropertyName);
                string[] localItem = item.ToString().Split(',');
                if (item != null && localItem == null)
                {
                    Color32 color32 = (Color32)item;
                    UpdateBodyArtColor(color32);
                }
                else
                {
                    Color32 color32 = new Color32(Convert.ToByte(localItem[0]), Convert.ToByte(localItem[1]), Convert.ToByte(localItem[2]), Convert.ToByte(localItem[3]));
                    UpdateBodyArtColor(color32);
                }
            }

            public void HandleStubbleColorChange(object sender, PropertyChangeEventArgs args)
            {
                var item = GetComponent<AtavismNode>().GetProperty(stubbleColorPropertyName);
                string[] localItem = item.ToString().Split(',');
                if (item != null && localItem == null)
                {
                    Color32 color32 = (Color32)item;
                    UpdateStubbleColor(color32);
                }
                else
                {
                    Color32 color32 = new Color32(Convert.ToByte(localItem[0]), Convert.ToByte(localItem[1]), Convert.ToByte(localItem[2]), Convert.ToByte(localItem[3]));
                    UpdateStubbleColor(color32);
                }
            }
            public void HandleScarColorChange(object sender, PropertyChangeEventArgs args)
            {
                var item = GetComponent<AtavismNode>().GetProperty(scarColorPropertyName);
                string[] localItem = item.ToString().Split(',');
                if (item != null && localItem == null)
                {
                    Color32 color32 = (Color32)item;
                    UpdateBodyScarColor(color32);
                }
                else
                {
                    Color32 color32 = new Color32(Convert.ToByte(localItem[0]), Convert.ToByte(localItem[1]), Convert.ToByte(localItem[2]), Convert.ToByte(localItem[3]));
                    UpdateBodyScarColor(color32);
                }
            }
            public void HandleMouthColorChange(object sender, PropertyChangeEventArgs args)
            {
                var item = GetComponent<AtavismNode>().GetProperty(mouthColorPropertyName);
                string[] localItem = item.ToString().Split(',');
                if (item != null && localItem == null)
                {
                    Color32 color32 = (Color32)item;
                    UpdateMouthColor(color32);
                }
                else
                {
                    Color32 color32 = new Color32(Convert.ToByte(localItem[0]), Convert.ToByte(localItem[1]), Convert.ToByte(localItem[2]), Convert.ToByte(localItem[3]));
                    UpdateMouthColor(color32);
                }
            }


            public void HandleHairColorChange(object sender, PropertyChangeEventArgs args)
            {
                var item = GetComponent<AtavismNode>().GetProperty(hairColorPropertyName);
                string[] localItem = item.ToString().Split(',');
                if (item != null && localItem == null)
                {
                    Color32 color32 = (Color32)item;
                    UpdateHairColor(color32);
                }
                else
                {
                    Color32 color32 = new Color32(Convert.ToByte(localItem[0]), Convert.ToByte(localItem[1]), Convert.ToByte(localItem[2]), Convert.ToByte(localItem[3]));
                    UpdateHairColor(color32);
                }
            }
            public void HandleEyeColorChange(object sender, PropertyChangeEventArgs args)
            {
                var item = GetComponent<AtavismNode>().GetProperty(eyeColorPropertyName);
                string[] localItem = item.ToString().Split(',');
                if (item != null && localItem == null)
                {
                    Color32 color32 = (Color32)item;
                    UpdateEyeColor(color32);
                }
                else
                {
                    Color32 color32 = new Color32(Convert.ToByte(localItem[0]), Convert.ToByte(localItem[1]), Convert.ToByte(localItem[2]), Convert.ToByte(localItem[3]));
                    UpdateEyeColor(color32);
                }
            }

            public void HandleHelmetColorChange(object sender, PropertyChangeEventArgs args)
            {
                var item = GetComponent<AtavismNode>().GetProperty(helmetColorPropertyName);

                var colorProperties = GetComponent<AtavismNode>().GetProperty(helmetColorPropertyName).ToString().Split('@');
                foreach (var colorProperty in colorProperties)
                {
                    var colorPropertyItem = colorProperty.Split(':');
                    var colorslot = colorPropertyItem[0];
                    var coloritem = colorPropertyItem[1].Split(',');
                    if (item != null && coloritem == null)
                    {

                        Color32 color32 = (Color32)item;
                        UpdateShaderColor(color32, colorslot, BodyType.Helmet);
                    }
                    else
                    {
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        UpdateShaderColor(color32, colorslot, BodyType.Helmet);
                    }
                }
            }

            public void HandleTorsoColorChange(object sender, PropertyChangeEventArgs args)
            {
                var item = GetComponent<AtavismNode>().GetProperty(helmetColorPropertyName);

                var colorProperties = GetComponent<AtavismNode>().GetProperty(helmetColorPropertyName).ToString().Split('@');
                foreach (var colorProperty in colorProperties)
                {
                    var colorPropertyItem = colorProperty.Split(':');
                    var colorslot = colorPropertyItem[0];
                    var coloritem = colorPropertyItem[1].Split(',');
                    if (item != null && coloritem == null)
                    {

                        Color32 color32 = (Color32)item;
                        UpdateShaderColor(color32, colorslot, BodyType.Torso);
                    }
                    else
                    {
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        UpdateShaderColor(color32, colorslot, BodyType.Torso);
                    }
                }
            }

            public void HandleUpperArmsColorChange(object sender, PropertyChangeEventArgs args)
            {
                var item = GetComponent<AtavismNode>().GetProperty(upperArmsColorPropertyName);

                var colorProperties = GetComponent<AtavismNode>().GetProperty(upperArmsColorPropertyName).ToString().Split('@');
                foreach (var colorProperty in colorProperties)
                {
                    var colorPropertyItem = colorProperty.Split(':');
                    var colorslot = colorPropertyItem[0];
                    var coloritem = colorPropertyItem[1].Split(',');
                    if (item != null && coloritem == null)
                    {

                        Color32 color32 = (Color32)item;
                        UpdateShaderColor(color32, colorslot, BodyType.Upperarms);
                    }
                    else
                    {
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        UpdateShaderColor(color32, colorslot, BodyType.Upperarms);
                    }
                }
            }
            public void HandleLowerArmsColorChange(object sender, PropertyChangeEventArgs args)
            {
                var item = GetComponent<AtavismNode>().GetProperty(lowerArmsColorPropertyName);

                var colorProperties = GetComponent<AtavismNode>().GetProperty(lowerArmsColorPropertyName).ToString().Split('@');
                foreach (var colorProperty in colorProperties)
                {
                    var colorPropertyItem = colorProperty.Split(':');
                    var colorslot = colorPropertyItem[0];
                    var coloritem = colorPropertyItem[1].Split(',');
                    if (item != null && coloritem == null)
                    {

                        Color32 color32 = (Color32)item;
                        UpdateShaderColor(color32, colorslot, BodyType.Helmet);
                    }
                    else
                    {
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        UpdateShaderColor(color32, colorslot, BodyType.Helmet);
                    }
                }
            }
            public void HandleHandsColorChange(object sender, PropertyChangeEventArgs args)
            {
                var item = GetComponent<AtavismNode>().GetProperty(handsColorPropertyName);

                var colorProperties = GetComponent<AtavismNode>().GetProperty(handsColorPropertyName).ToString().Split('@');
                foreach (var colorProperty in colorProperties)
                {
                    var colorPropertyItem = colorProperty.Split(':');
                    var colorslot = colorPropertyItem[0];
                    var coloritem = colorPropertyItem[1].Split(',');
                    if (item != null && coloritem == null)
                    {

                        Color32 color32 = (Color32)item;
                        UpdateShaderColor(color32, colorslot, BodyType.Hands);
                    }
                    else
                    {
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        UpdateShaderColor(color32, colorslot, BodyType.Hands);
                    }
                }
            }

            public void HandleHipsColorChange(object sender, PropertyChangeEventArgs args)
            {
                var item = GetComponent<AtavismNode>().GetProperty(hipsColorPropertyName);

                var colorProperties = GetComponent<AtavismNode>().GetProperty(hipsColorPropertyName).ToString().Split('@');
                foreach (var colorProperty in colorProperties)
                {
                    var colorPropertyItem = colorProperty.Split(':');
                    var colorslot = colorPropertyItem[0];
                    var coloritem = colorPropertyItem[1].Split(',');
                    if (item != null && coloritem == null)
                    {

                        Color32 color32 = (Color32)item;
                        UpdateShaderColor(color32, colorslot, BodyType.Hips);
                    }
                    else
                    {
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        UpdateShaderColor(color32, colorslot, BodyType.Hips);
                    }
                }
            }
            public void HandleLowerLegsColorChange(object sender, PropertyChangeEventArgs args)
            {
                var item = GetComponent<AtavismNode>().GetProperty(lowerLegsColorPropertyName);

                var colorProperties = GetComponent<AtavismNode>().GetProperty(lowerLegsColorPropertyName).ToString().Split('@');
                foreach (var colorProperty in colorProperties)
                {
                    var colorPropertyItem = colorProperty.Split(':');
                    var colorslot = colorPropertyItem[0];
                    var coloritem = colorPropertyItem[1].Split(',');
                    if (item != null && coloritem == null)
                    {

                        Color32 color32 = (Color32)item;
                        UpdateShaderColor(color32, colorslot, BodyType.LowerLegs);
                    }
                    else
                    {
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        UpdateShaderColor(color32, colorslot, BodyType.LowerLegs);
                    }
                }
            }


            public void HandleFeetColorChange(object sender, PropertyChangeEventArgs args)
            {
                var item = GetComponent<AtavismNode>().GetProperty(feetColorPropertyName);

                var colorProperties = GetComponent<AtavismNode>().GetProperty(feetColorPropertyName).ToString().Split('@');
                foreach (var colorProperty in colorProperties)
                {
                    var colorPropertyItem = colorProperty.Split(':');
                    var colorslot = colorPropertyItem[0];
                    var coloritem = colorPropertyItem[1].Split(',');
                    if (item != null && coloritem == null)
                    {

                        Color32 color32 = (Color32)item;
                        UpdateShaderColor(color32, colorslot, BodyType.Feet);
                    }
                    else
                    {
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        UpdateShaderColor(color32, colorslot, BodyType.Feet);
                    }
                }
            }


            // called from the BuildLists method
            void ReplaceAllModelShaders()
            {
                Transform[] rootTransform = GetComponentsInChildren<Transform>(true);

                // cycle through all child objects of the parent object
                for (int i = 0; i < rootTransform.Length; i++)
                {
                    // get child gameobject index i
                    GameObject go = rootTransform[i].gameObject;
                    bool torso = false;
                    BodyType bodyType = BodyType.None;
                    if (torsoModels.Contains(go))
                    {
                        bodyType = BodyType.Torso;
                    }
                    if (handModels.Contains(go))
                    {
                        bodyType = BodyType.Hands;
                    }
                    if (lowerArmModels.Contains(go))
                    {
                        bodyType = BodyType.LowerArms;
                    }
                    if (upperArmModels.Contains(go))
                    {
                        bodyType = BodyType.Upperarms;
                    }
                    if (hipModels.Contains(go))
                    {
                        bodyType = BodyType.Hips;
                    }
                    if (lowerLegModels.Contains(go))
                    {
                        bodyType = BodyType.LowerLegs;
                    }
                    if (feetModels.Contains(go))
                    {
                        bodyType = BodyType.Feet;
                    }
                    if (headModels.Contains(go))
                    {
                        bodyType = BodyType.Head;
                    }
                    if (helmetModels.Contains(go))
                    {
                        bodyType = BodyType.Helmet;
                    }

                    SkinnedMeshRenderer skinnedMeshRenderer = go.GetComponent<SkinnedMeshRenderer>();
                    MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
                    Material[] materials = null;

                    if (skinnedMeshRenderer)
                    {

                        materials = skinnedMeshRenderer.materials;

                    }
                    else if (meshRenderer)
                    {
                        materials = meshRenderer.materials;
                    }

                    if (skinnedMeshRenderer || meshRenderer)
                    {
                        int materialCount = -1;

                        foreach (var item in materials)
                        {
                            materialCount++;
                            if (bodyMat)
                            {
                                if (item.name.Contains(bodyMat.name))
                                {
                                    switch (bodyType)
                                    {
                                        case BodyType.Torso:
                                            if (internalTorsoMat != null)
                                                materials[materialCount] = internalTorsoMat;
                                            else
                                                materials[materialCount] = internalMat;
                                            break;
                                        case BodyType.Hands:
                                            if (internalHandsMat != null)
                                                materials[materialCount] = internalHandsMat;
                                            else
                                                materials[materialCount] = internalMat;
                                            break;
                                        case BodyType.Upperarms:
                                            if (internalUpperArmsMat != null)
                                                materials[materialCount] = internalUpperArmsMat;
                                            else
                                                materials[materialCount] = internalMat;
                                            break;
                                        case BodyType.LowerArms:
                                            if (internalLowerArmsMat != null)
                                                materials[materialCount] = internalLowerArmsMat;
                                            else
                                                materials[materialCount] = internalMat;
                                            break;
                                        case BodyType.Hips:
                                            if (internalHipsMat != null)
                                                materials[materialCount] = internalHipsMat;
                                            else
                                                materials[materialCount] = internalMat;
                                            break;
                                        case BodyType.LowerLegs:
                                            if (internalLowerLegsMat != null)
                                                materials[materialCount] = internalLowerLegsMat;
                                            else
                                                materials[materialCount] = internalMat;
                                            break;

                                        case BodyType.Feet:
                                            if (internalLowerLegsMat != null)
                                                materials[materialCount] = internalFeetMat;
                                            else
                                                materials[materialCount] = internalMat;
                                            break;

                                        case BodyType.Helmet:
                                            if (internalHelmetMat != null)
                                                materials[materialCount] = internalHelmetMat;
                                            else
                                                materials[materialCount] = internalMat;
                                            break;
                                        case BodyType.Head:
                                            if (internalHeadMat != null)
                                                materials[materialCount] = internalHeadMat;
                                            else
                                                materials[materialCount] = internalMat;
                                            break;

                                        case BodyType.None:
                                            if (internalMat != null)
                                                materials[materialCount] = internalMat;
                                            else
                                                materials[materialCount] = bodyMat;
                                            break;

                                    }

                                    skinnedMeshRenderer.materials = materials;
                                }
                            }

                            if (headMat)
                            {
                                if (item.name.Contains(headMat.name))
                                {
                                    switch (bodyType)
                                    {
                                        case BodyType.Helmet:
                                            if (internalHeadMat != null)
                                                materials[materialCount] = internalHeadMat;
                                            else
                                                materials[materialCount] = internalMat;
                                            break;
                                        case BodyType.Head:
                                            if (internalHeadMat != null)
                                                materials[materialCount] = internalHeadMat;
                                            else
                                                materials[materialCount] = internalMat;
                                            break;
                                        case BodyType.None:
                                            if (internalMat != null)
                                                materials[materialCount] = internalHeadMat;
                                            else
                                                materials[materialCount] = internalMat;
                                            break;
                                    }

                                    skinnedMeshRenderer.materials = materials;
                                }
                            }


                            if (eyeMat)
                            {
                                if (item.name.Contains(eyeMat.name))
                                {
                                    if (internalEyeMat != null)
                                        materials[materialCount] = internalEyeMat;
                                    else
                                        materials[materialCount] = eyeMat;

                                    skinnedMeshRenderer.materials = materials;
                                }
                            }

                            if (hairMat && internalHairModels)
                            {
                                if (item.name.Contains(hairMat.name))
                                {
                                    if (internalHairMat != null)
                                        materials[materialCount] = internalHairMat;
                                    else
                                        materials[materialCount] = hairMat;

                                    skinnedMeshRenderer.materials = materials;
                                }
                            }

                            for (int si = 0; si < skinMaterialsToSync.Count; si++)
                            {
                                if (skinMaterialsToSync[si] != null)
                                {
                                    if (item.name.Contains(skinMaterialsToSync[si].name))
                                    {
                                        materials[materialCount] = skinMaterialsSynced[si];

                                        skinnedMeshRenderer.materials = materials;
                                        
                                    }
                                }
                            }
                        }
                    }
                }
            }


            public string ReturnMaterialString(BodyType bodyType)
            {
                Material thisMaterial = ReturnMaterial(bodyType);
                string colorString = null;

                if (!thisMaterial) { return null; }

                if (thisMaterial.HasProperty("_Color_Primary"))
                {
                    Color32 _Color_Primary = thisMaterial.GetColor("_Color_Primary");
                    colorString = "_Color_Primary:" + String.Format("{0},{1},{2},{3}", _Color_Primary.r, _Color_Primary.g, _Color_Primary.b, _Color_Primary.a);
                }

                if (thisMaterial.HasProperty("_Color_Secondary"))
                {
                    Color32 _Color_Secondary = thisMaterial.GetColor("_Color_Secondary");
                    colorString += "@_Color_Secondary:" + String.Format("{0},{1},{2},{3}", _Color_Secondary.r, _Color_Secondary.g, _Color_Secondary.b, _Color_Secondary.a);
                }

                if (thisMaterial.HasProperty("_Color_Tertiary"))
                {
                    Color32 _Color_Tertiary = thisMaterial.GetColor("_Color_Tertiary");
                    colorString += "@_Color_Tertiary:" + String.Format("{0},{1},{2},{3}", _Color_Tertiary.r, _Color_Tertiary.g, _Color_Tertiary.b, _Color_Tertiary.a);
                }

                if (thisMaterial.HasProperty("_Color_Leather_Primary"))
                {
                    Color32 _Color_Leather_Primary = thisMaterial.GetColor("_Color_Leather_Primary");
                    colorString += "@_Color_Leather_Primary:" + String.Format("{0},{1},{2},{3}", _Color_Leather_Primary.r, _Color_Leather_Primary.g, _Color_Leather_Primary.b, _Color_Leather_Primary.a);
                }

                if (thisMaterial.HasProperty("_Color_Leather_Secondary"))
                {
                    Color32 _Color_Leather_Secondary = thisMaterial.GetColor("_Color_Leather_Secondary");
                    colorString += "@_Color_Leather_Secondary:" + String.Format("{0},{1},{2},{3}", _Color_Leather_Secondary.r, _Color_Leather_Secondary.g, _Color_Leather_Secondary.b, _Color_Leather_Secondary.a);
                }

                if (thisMaterial.HasProperty("_Color_Leather_Tertiary"))
                {
                    Color32 _Color_Leather_Tertiary = thisMaterial.GetColor("_Color_Leather_Tertiary");
                    colorString += "@_Color_Leather_Tertiary:" + String.Format("{0},{1},{2},{3}", _Color_Leather_Tertiary.r, _Color_Leather_Tertiary.g, _Color_Leather_Tertiary.b, _Color_Leather_Tertiary.a);
                }

                if (thisMaterial.HasProperty("_Color_Metal_Primary"))
                {
                    Color32 _Color_Metal_Primary = thisMaterial.GetColor("_Color_Metal_Primary");
                    colorString += "@_Color_Metal_Primary:" + String.Format("{0},{1},{2},{3}", _Color_Metal_Primary.r, _Color_Metal_Primary.g, _Color_Metal_Primary.b, _Color_Metal_Primary.a);
                }

                if (thisMaterial.HasProperty("_Color_Metal_Secondary"))
                {
                    Color32 _Color_Metal_Secondary = thisMaterial.GetColor("_Color_Metal_Secondary");
                    colorString += "@_Color_Metal_Secondary:" + String.Format("{0},{1},{2},{3}", _Color_Metal_Secondary.r, _Color_Metal_Secondary.g, _Color_Metal_Secondary.b, _Color_Metal_Secondary.a);
                }

                if (thisMaterial.HasProperty("_Color_Metal_Dark"))
                {
                    Color32 _Color_Metal_Dark = thisMaterial.GetColor("_Color_Metal_Dark");
                    colorString += "@_Color_Metal_Dark:" + String.Format("{0},{1},{2},{3}", _Color_Metal_Dark.r, _Color_Metal_Dark.g, _Color_Metal_Dark.b, _Color_Metal_Dark.a);
                }

                return colorString;
            }

            private Material ReturnMaterial(BodyType bodyType)
            {
                Material material = null;
                Material thisHandMaterial = null;
                Material thisTorsoMaterial = null;
                Material thisUpperArmMaterial = null;
                Material thisLowerArmMaterial = null;
                Material thisHipsMaterial = null;
                Material thisLowerLegMaterial = null;
                Material thisFeetMaterial = null;

                Material thisHeadMaterial = null;
                Material thisHelmetMaterial = null;
                if (enableMultipleCharacterColors)
                {
                    thisHandMaterial = GetComponent<ModularCustomizationManager>().internalHandsMat;
                    thisTorsoMaterial = GetComponent<ModularCustomizationManager>().internalTorsoMat;
                    thisUpperArmMaterial = GetComponent<ModularCustomizationManager>().internalUpperArmsMat;
                    thisLowerArmMaterial = GetComponent<ModularCustomizationManager>().internalLowerArmsMat;
                    thisHipsMaterial = GetComponent<ModularCustomizationManager>().internalHipsMat;
                    thisLowerLegMaterial = GetComponent<ModularCustomizationManager>().internalLowerLegsMat;
                    thisHeadMaterial = GetComponent<ModularCustomizationManager>().internalHeadMat;
                    thisHelmetMaterial = GetComponent<ModularCustomizationManager>().internalHelmetMat;
                    thisFeetMaterial = GetComponent<ModularCustomizationManager>().internalFeetMat;
                }
                else
                {
                    thisHandMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                    thisTorsoMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                    thisUpperArmMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                    thisLowerArmMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                    thisHipsMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                    thisFeetMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                    thisLowerLegMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                    thisHelmetMaterial = GetComponent<ModularCustomizationManager>().internalMat;

                    if (GetComponent<ModularCustomizationManager>().internalHeadMat)
                    {
                        thisHeadMaterial = GetComponent<ModularCustomizationManager>().internalHeadMat;
                    }
                    else
                    {
                        thisHeadMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                    }

                }
                switch (bodyType)
                {
                    case BodyType.Hands:
                        material = thisHandMaterial;
                        break;
                    case BodyType.Torso:
                        material = thisTorsoMaterial;
                        break;
                    case BodyType.Upperarms:
                        material = thisUpperArmMaterial;
                        break;
                    case BodyType.LowerArms:
                        material = thisLowerArmMaterial;
                        break;
                    case BodyType.Hips:
                        material = thisHipsMaterial;
                        break;
                    case BodyType.LowerLegs:
                        material = thisLowerLegMaterial;
                        break;
                    case BodyType.Feet:
                        material = thisFeetMaterial;
                        break;
                    case BodyType.Head:
                        material = thisHeadMaterial;
                        break;
                    case BodyType.Helmet:
                        material = thisHelmetMaterial;
                        break;
                }

                return material;
            }

            public void UpdateShaderColor(Color32 color, string colorToChange, BodyType bodyType)
            {
                Material thisMaterial = ReturnMaterial(bodyType);
                if (thisMaterial)
                {
                    thisMaterial.SetColor(colorToChange, color);
                }
            }

            public void UpdateShaderColor(Color32 color, ShaderColorType colorType, BodyType bodyType)
            {
                string colorSwitch = null;
                Material thisMaterial = null;
                Material thisHeadMaterial = null;

                Material thisHandMaterial = GetComponent<ModularCustomizationManager>().internalHandsMat;
                Material thisTorsoMaterial = GetComponent<ModularCustomizationManager>().internalTorsoMat;
                Material thisUpperArmMaterial = GetComponent<ModularCustomizationManager>().internalUpperArmsMat;
                Material thisLowerArmMaterial = GetComponent<ModularCustomizationManager>().internalLowerArmsMat;
                Material thisHipsMaterial = GetComponent<ModularCustomizationManager>().internalHipsMat;
                Material thisLowerLegMaterial = GetComponent<ModularCustomizationManager>().internalLowerLegsMat;
                Material thisHelmetMaterial = GetComponent<ModularCustomizationManager>().internalHelmetMat;

                switch (bodyType)
                {
                    case BodyType.Hands:
                        ActiveHandsColor = ReturnMaterialString(bodyType);
                        break;
                    case BodyType.Head:
                        ActiveHeadColor = ReturnMaterialString(bodyType);
                        break;
                    case BodyType.Helmet:
                        ActiveHelmetColor = ReturnMaterialString(bodyType);
                        break;
                    case BodyType.Hips:
                        ActiveHipsColor = ReturnMaterialString(bodyType);
                        break;
                    case BodyType.LowerArms:
                        ActiveLowerArmsColor = ReturnMaterialString(bodyType);
                        break;
                    case BodyType.LowerLegs:
                        ActiveLowerLegsColor = ReturnMaterialString(bodyType);
                        break;
                    case BodyType.Feet:
                        ActiveLowerLegsColor = ReturnMaterialString(bodyType);
                        break;
                    case BodyType.Torso:
                        ActiveTorsoColor = ReturnMaterialString(bodyType);
                        break;
                    case BodyType.Upperarms:
                        ActiveUpperArmsColor = ReturnMaterialString(bodyType);
                        break;
                    case BodyType.None:
                        break;
                }


                switch (colorType)
                {
                    case ShaderColorType.Eyes:
                        if (internalEyeMat)
                        {
                            thisMaterial = GetComponent<ModularCustomizationManager>().internalEyeMat;
                        }
                        else
                        {
                            thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                        }
                        if (thisMaterial)
                        {
                            ActiveEyeColor = color;
                            if (directSetEyeColor)
                            {
                                thisMaterial.color = color;
                            }
                            else
                            {
                                colorSwitch = "_Color_Eyes";
                            }
                        }
                        break;
                    case ShaderColorType.Hair:
                        if (allowDifferentHairColors)
                        {
                            thisMaterial = GetComponent<ModularCustomizationManager>().internalEyebrowMat;
                        }
                        else
                        {
                            if (internalHairMat)
                            {
                                thisMaterial = GetComponent<ModularCustomizationManager>().internalHairMat;
                            }
                            else
                            {
                                thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                            }
                        }

                        ActiveEyebrowColor = color;
                        if (directSetHairColor)
                        {

                            thisMaterial.color = color;
                        }
                        else
                        {
                            colorSwitch = "_Color_Hair";
                        }

                        break;


                    case ShaderColorType.LeatherPrimary:
                        if (bodyType != BodyType.None)
                        {
                            thisMaterial = ReturnMaterial(bodyType);
                        }
                        else
                        {
                            thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                        }
                        ActiveLeatherPrimaryColor = color;
                        colorSwitch = "_Color_Leather_primary";
                        break;
                    case ShaderColorType.LeatherSecondary:
                        if (bodyType != BodyType.None)
                        {
                            thisMaterial = ReturnMaterial(bodyType);

                        }
                        else
                        {
                            thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                        }
                        ActiveLeatherSecondaryColor = color;
                        colorSwitch = "_Color_Leather_Secondary";
                        break;
                    case ShaderColorType.LeatherTertiary:
                        if (bodyType != BodyType.None)
                        {
                            thisMaterial = ReturnMaterial(bodyType);

                        }
                        else
                        {
                            thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                        }
                        ActiveLeatherTertiaryColor = color;
                        colorSwitch = "_Color_Leather_Tertiary";
                        break;
                    case ShaderColorType.MetalDark:
                        if (bodyType != BodyType.None)
                        {
                            thisMaterial = ReturnMaterial(bodyType);
                        }
                        else
                        {
                            thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                        }
                        ActiveMetalDarkColor = color;
                        colorSwitch = "_Color_Metal_Dark";
                        break;
                    case ShaderColorType.MetalPrimary:
                        if (bodyType != BodyType.None)
                        {
                            thisMaterial = ReturnMaterial(bodyType);
                        }
                        else
                        {
                            thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                        }
                        ActiveMetalPrimaryColor = color;
                        colorSwitch = "_Color_Metal_Primary";
                        break;
                    case ShaderColorType.MetalSecondary:
                        if (bodyType != BodyType.None)
                        {
                            thisMaterial = ReturnMaterial(bodyType);
                        }
                        else
                        {
                            thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                        }
                        ActiveMetalSecondaryColor = color;
                        colorSwitch = "_Color_Metal_Secondary";
                        break;
                    case ShaderColorType.Primary:
                        if (bodyType != BodyType.None)
                        {
                            thisMaterial = ReturnMaterial(bodyType);
                        }
                        else
                        {
                            thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                        }
                        ActivePrimaryColor = color;
                        colorSwitch = "_Color_Primary";
                        break;
                    case ShaderColorType.Secondary:
                        if (bodyType != BodyType.None)
                        {
                            thisMaterial = ReturnMaterial(bodyType);
                        }
                        else
                        {
                            thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                        }
                        ActiveSecondaryColor = color;
                        colorSwitch = "_Color_Secondary";
                        break;
                    case ShaderColorType.Tertiary:
                        if (bodyType != BodyType.None)
                        {
                            thisMaterial = ReturnMaterial(bodyType);
                        }
                        else
                        {
                            thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                        }
                        ActiveTertiaryColor = color;
                        colorSwitch = "_Color_Tertiary";
                        break;
                    case ShaderColorType.BodyArt:
                        thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                        ActiveBodyArtColor = color;
                        colorSwitch = "_Color_BodyArt";
                        break;
                    case ShaderColorType.Scar:
                        if (internalMat)
                        {
                            thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                            ActiveScarColor = color;
                        }
                        colorSwitch = "_Color_Scar";
                        break;
                    case ShaderColorType.Skin:
                        colorSwitch = "_Color_Skin";
                        ActiveBodyColor = color;
                        thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                        thisHeadMaterial = GetComponent<ModularCustomizationManager>().internalHeadMat;
                        break;
                    case ShaderColorType.Stubble:
                        colorSwitch = "_Color_Stubble";
                        thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                        ActiveStubbleColor = color;
                        break;
                    case ShaderColorType.Mouth:
                        if (internalMouthMat)
                        {
                            thisMaterial = GetComponent<ModularCustomizationManager>().internalMouthMat;
                            ActiveMouthColor = color;
                            if (thisMaterial)
                            {
                                colorSwitch = "_Color_Mouth";
                            }
                        }
                        break;
                    case ShaderColorType.Beard:
                        if (allowDifferentHairColors)
                        {
                            if (internalBeardMat && !useBeardColorForColor2)
                            {
                                thisMaterial = GetComponent<ModularCustomizationManager>().internalBeardMat;
                            }
                            else if (useBeardColorForColor2)
                            {
                                thisMaterial = GetComponent<ModularCustomizationManager>().internalHairMat;
                            }
                        }
                        else
                        {
                            if (internalHairMat)
                            {
                                thisMaterial = GetComponent<ModularCustomizationManager>().internalHairMat;
                            }
                            else
                            {
                                thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                            }
                        }
                        ActiveBeardColor = color;
                        if (thisMaterial && useBeardColorForColor2)
                        {
                            thisMaterial.SetColor("_Color2", color);
                        }
                        if (thisMaterial && !useBeardColorForColor2)
                        {
                            if (directSetHairColor)
                            {

                                thisMaterial.color = color;
                            }
                            else
                            {
                                colorSwitch = "_Color_Hair";
                            }
                        }
                        break;
                }

                if (thisMaterial)
                {
                    if (directSetSkinColor)
                    {
                        thisMaterial.color = color;

                    }
                    else
                    {
                        thisMaterial.SetColor(colorSwitch, color);
                    }
                }
                if (thisHeadMaterial)
                {
                    if (directSetSkinColor)
                    {
                        thisHeadMaterial.color = color;
                    }
                    else
                    {
                        thisHeadMaterial.SetColor(colorSwitch, color);
                    }
                }
            }

            public void SkinColorUP()
            {
                skinColorIndex++;
                if (skinColorIndex > skinColors.Length - 1)
                    skinColorIndex = 0;
                UpdateBodyColor(skinColors[skinColorIndex]);

            }
            
            public void SkinColorUP(int id)
            {
                skinColorIndex = id;
                if (skinColorIndex > skinColors.Length - 1)
                    skinColorIndex = skinColors.Length - 1;
                if (skinColorIndex < 0)
                    skinColorIndex = 0;
                UpdateBodyColor(skinColors[skinColorIndex]);

            }

            public void SyncHairColorUP()
            {
                hairColorIndex++;
                if (hairColorIndex > hairColors.Length - 1)
                    hairColorIndex = 0;
                UpdateHairColor(hairColors[hairColorIndex]);
                UpdateBeardColor(hairColors[hairColorIndex]);
                UpdateEyebrowColor(hairColors[hairColorIndex]);
            }
            
            public void SyncHairColorUP(int id)
            {
                hairColorIndex = id;
                if (hairColorIndex > hairColors.Length - 1)
                    hairColorIndex = hairColors.Length - 1;
                if (hairColorIndex < 0)
                    hairColorIndex = 0;
                UpdateHairColor(hairColors[hairColorIndex]);
                UpdateBeardColor(hairColors[hairColorIndex]);
                UpdateEyebrowColor(hairColors[hairColorIndex]);
            }

            public void HairColorUP()
            {
                hairColorIndex++;
                if (hairColorIndex > hairColors.Length - 1)
                    hairColorIndex = 0;
                UpdateHairColor(hairColors[hairColorIndex]);
            }

            public void HairColorUP(int id)
            {
                hairColorIndex = id;
                if (hairColorIndex > hairColors.Length - 1)
                    hairColorIndex = hairColors.Length - 1;
                if (hairColorIndex < 0)
                    hairColorIndex = 0;
                UpdateHairColor(hairColors[hairColorIndex]);
            }

            public void BeardColorUP()
            {
                beardColorIndex++;
                if (beardColorIndex > hairColors.Length - 1)
                    beardColorIndex = 0;
                UpdateBeardColor(hairColors[beardColorIndex]);
            }

            public void BeardColorUP(int id)
            {
                beardColorIndex = id;
                if (beardColorIndex > hairColors.Length - 1)
                    beardColorIndex = hairColors.Length - 1;
                if (beardColorIndex < 0)
                    beardColorIndex = 0;
                UpdateBeardColor(hairColors[beardColorIndex]);
            }

            public void EyebrowColorUP()
            {
                eyebrowColorIndex++;
                if (eyebrowColorIndex > hairColors.Length - 1)
                    eyebrowColorIndex = 0;
                UpdateEyebrowColor(hairColors[eyebrowColorIndex]);
            }

            public void EyebrowColorUP(int id)
            {
                eyebrowColorIndex = id;
                if (eyebrowColorIndex > hairColors.Length - 1)
                    eyebrowColorIndex = hairColors.Length - 1;
                if (eyebrowColorIndex < 0)
                    eyebrowColorIndex = 0;
                UpdateEyebrowColor(hairColors[eyebrowColorIndex]);
            }

            public void EyeColorUP()
            {
                eyeColorIndex++;
                if (eyeColorIndex > eyeColors.Length - 1)
                    eyeColorIndex = 0;
                UpdateEyeColor(eyeColors[eyeColorIndex]);

            }

            public void EyeColorUP(int id)
            {
                eyeColorIndex = id;
                if (eyeColorIndex > eyeColors.Length - 1)
                    eyeColorIndex = eyeColors.Length - 1;
                if (eyeColorIndex < 0)
                    eyeColorIndex = 0;
                UpdateEyeColor(eyeColors[eyeColorIndex]);

            }

            public void StubbleColorUP()
            {
                stubbleColorIndex++;
                if (stubbleColorIndex > stubbleColors.Length - 1)
                    stubbleColorIndex = 0;
                UpdateStubbleColor(stubbleColors[stubbleColorIndex]);

            }

            public void StubbleColorUP(int id)
            {
                stubbleColorIndex = id;
                if (stubbleColorIndex > stubbleColors.Length - 1)
                    stubbleColorIndex = stubbleColors.Length - 1;
                if (stubbleColorIndex < 0)
                    stubbleColorIndex = 0;
                UpdateStubbleColor(stubbleColors[stubbleColorIndex]);

            }

            public void SkinScarColorUP()
            {
                skinScarColorIndex++;
                if (skinScarColorIndex == scarColors.Length - 1)
                    skinScarColorIndex = 0;
                UpdateBodyScarColor(skinColors[skinScarColorIndex]);

            }

            public void SkinScarColorUP(int id)
            {
                skinScarColorIndex = id;
                if (skinScarColorIndex > scarColors.Length - 1)
                    skinScarColorIndex = scarColors.Length - 1;
                if (skinScarColorIndex < 0)
                    skinScarColorIndex = 0;
                UpdateBodyScarColor(skinColors[skinScarColorIndex]);

            }

            public void BodyArtColorUP()
            {
                bodyArtColorIndex++;
                if (bodyArtColorIndex > bodyArtColors.Length - 1)
                    bodyArtColorIndex = 0;
                UpdateBodyArtColor(bodyArtColors[bodyArtColorIndex]);
            }

            public void BodyArtColorUP(int id)
            {
                bodyArtColorIndex = id;
                if (bodyArtColorIndex > bodyArtColors.Length - 1)
                    bodyArtColorIndex = bodyArtColors.Length - 1;
                if (bodyArtColorIndex < 0)
                    bodyArtColorIndex = 0;
                UpdateBodyArtColor(bodyArtColors[bodyArtColorIndex]);
            }


            public void UpdateBodyColor(Color32 color)
            {
                Material thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                Material thisHeadMaterial = GetComponent<ModularCustomizationManager>().internalHeadMat;

                Material thisHandMaterial = GetComponent<ModularCustomizationManager>().internalHandsMat;
                Material thisTorsoMaterial = GetComponent<ModularCustomizationManager>().internalTorsoMat;
                Material thisUpperArmMaterial = GetComponent<ModularCustomizationManager>().internalUpperArmsMat;
                Material thisLowerArmMaterial = GetComponent<ModularCustomizationManager>().internalLowerArmsMat;
                Material thisHipsMaterial = GetComponent<ModularCustomizationManager>().internalHipsMat;
                Material thisLowerLegMaterial = GetComponent<ModularCustomizationManager>().internalLowerLegsMat;
                Material thisFeetMat = GetComponent<ModularCustomizationManager>().internalFeetMat;

                Material thisHelmetMaterial = GetComponent<ModularCustomizationManager>().internalHelmetMat;


                ActiveBodyColor = color;
                if (thisMaterial)
                {
                    if (directSetSkinColor)
                    {
                        thisMaterial.color = color;

                    }
                    else
                    {
                        thisMaterial.SetColor("_Color_Skin", color);
                    }

                    if (completelyDisableInstancedMaterials)
                    {
                        if (skinMaterialsToSync.Count > 0)
                        {
                            foreach (Material skinItem in skinMaterialsToSync)
                            {
                                skinItem.color = color;
                            }
                        }
                    }
                    else
                    {
                        if (skinMaterialsSynced.Count > 0)
                        {
                            foreach (Material skinItem in skinMaterialsSynced)
                            {
                                skinItem.color = color;
                            }
                        }
                    }
                }



                if (thisHeadMaterial)
                {
                    if (directSetSkinColor)
                    {
                        thisHeadMaterial.color = color;
                    }
                    else
                    {
                        thisHeadMaterial.SetColor("_Color_Skin", color);
                    }
                }

                if (thisHandMaterial)
                {
                    if (directSetSkinColor)
                    {
                        thisHandMaterial.color = color;

                    }
                    else
                    {
                        thisHandMaterial.SetColor("_Color_Skin", color);
                    }
                }
                if (thisTorsoMaterial)
                {
                    if (directSetSkinColor)
                    {
                        thisTorsoMaterial.color = color;

                    }
                    else
                    {
                        thisTorsoMaterial.SetColor("_Color_Skin", color);
                    }
                }
                if (thisUpperArmMaterial)
                {
                    if (directSetSkinColor)
                    {
                        thisUpperArmMaterial.color = color;

                    }
                    else
                    {
                        thisUpperArmMaterial.SetColor("_Color_Skin", color);
                    }
                }
                if (thisLowerArmMaterial)
                {
                    if (directSetSkinColor)
                    {
                        thisLowerArmMaterial.color = color;

                    }
                    else
                    {
                        thisLowerArmMaterial.SetColor("_Color_Skin", color);
                    }
                }
                if (thisHipsMaterial)
                {
                    if (directSetSkinColor)
                    {
                        thisHipsMaterial.color = color;

                    }
                    else
                    {
                        thisHipsMaterial.SetColor("_Color_Skin", color);
                    }
                }
                if (thisLowerLegMaterial)
                {
                    if (directSetSkinColor)
                    {
                        thisLowerLegMaterial.color = color;

                    }
                    else
                    {
                        thisLowerLegMaterial.SetColor("_Color_Skin", color);
                    }
                }

                if (thisFeetMat)
                {
                    if (directSetSkinColor)
                    {
                        thisFeetMat.color = color;

                    }
                    else
                    {
                        thisFeetMat.SetColor("_Color_Skin", color);
                    }
                }

                if (thisHelmetMaterial)
                {
                    if (directSetSkinColor)
                    {
                        thisHelmetMaterial.color = color;
                    }
                    else
                    {
                        thisHelmetMaterial.SetColor("_Color_Skin", color);
                    }
                }


            }

            public void UpdateStubbleColor(Color32 color)
            {

                Material thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                Material thisHeadMaterial = GetComponent<ModularCustomizationManager>().internalHeadMat;

                ActiveStubbleColor = color;
                if (thisMaterial)
                {
                    thisMaterial.SetColor("_Color_Stubble", color);
                }
                if (thisHeadMaterial)
                {
                    thisHeadMaterial.SetColor("_Color_Stubble", color);
                }
            }

            public void UpdateBodyArtColor(Color32 color)
            {
                Material thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                Material thisHeadMaterial = GetComponent<ModularCustomizationManager>().internalHeadMat;

                ActiveBodyArtColor = color;
                if (thisMaterial)
                {
                    thisMaterial.SetColor("_Color_BodyArt", color);
                }
                if (thisHeadMaterial)
                {
                    thisHeadMaterial.SetColor("_Color_BodyArt", color);
                }
            }

            public void UpdateEyebrowColor(Color32 color)
            {
                Material thisMaterial = null;
                Material thisHeadMaterial = GetComponent<ModularCustomizationManager>().internalHeadMat;

                if (allowDifferentHairColors)
                {
                    thisMaterial = GetComponent<ModularCustomizationManager>().internalEyebrowMat;
                }
                else
                {
                    if (internalHairMat)
                    {
                        thisMaterial = GetComponent<ModularCustomizationManager>().internalHairMat;
                    }
                    else
                    {
                        thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                    }
                }
                if (thisMaterial)
                {
                    ActiveEyebrowColor = color;
                    if (directSetHairColor)
                    {
                        thisMaterial.color = color;
                    }
                    else
                    {
                        if (thisHeadMaterial)
                        {
                            thisHeadMaterial.SetColor("_Color_BodyArt", color);
                        }
                        thisMaterial.SetColor("_Color_Hair", color);
                    }

                }
            }

            public void UpdateBeardColor(Color32 color)
            {
                Material thisMaterial = null;
                Material thisHeadMaterial = GetComponent<ModularCustomizationManager>().internalHeadMat;

                if (allowDifferentHairColors)
                {
                    if (internalBeardMat && !useBeardColorForColor2)
                    {
                        thisMaterial = GetComponent<ModularCustomizationManager>().internalBeardMat;
                    }
                    else if (useBeardColorForColor2)
                    {
                        thisMaterial = GetComponent<ModularCustomizationManager>().internalHairMat;
                    }
                }
                else
                {
                    if (internalHairMat)
                    {
                        thisMaterial = GetComponent<ModularCustomizationManager>().internalHairMat;
                    }
                    else
                    {
                        thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                    }
                }
                ActiveBeardColor = color;
                if (thisMaterial && useBeardColorForColor2)
                {
                    thisMaterial.SetColor("_Color2", color);
                    if (thisHeadMaterial)
                    {
                        thisHeadMaterial.SetColor("_Color2", color);
                    }
                }
                if (thisMaterial && !useBeardColorForColor2)
                {
                    if (directSetHairColor)
                    {

                        thisMaterial.color = color;
                    }
                    else
                    {
                        if (thisHeadMaterial)
                        {
                            thisHeadMaterial.SetColor("_Color_Hair", color);
                        }
                        thisMaterial.SetColor("_Color_Hair", color);
                    }
                }

            }

            public void UpdateMouthColor(Color32 color)
            {
                if (internalMouthMat)
                {
                    Material thisMaterial = GetComponent<ModularCustomizationManager>().internalMouthMat;
                    Material thisHeadMaterial = GetComponent<ModularCustomizationManager>().internalHeadMat;

                    ActiveMouthColor = color;
                    if (thisMaterial)
                    {
                        thisMaterial.SetColor("_Color_Mouth", color);
                    }
                    if (thisHeadMaterial)
                    {
                        thisHeadMaterial.SetColor("_Color_Mouth", color);
                    }
                }
            }

            public void UpdateBodyScarColor(Color32 color)
            {
                if (internalMat)
                {
                    Material thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                    Material thisHeadMaterial = GetComponent<ModularCustomizationManager>().internalHeadMat;

                    ActiveScarColor = color;
                    if (thisMaterial)
                    {
                        thisMaterial.SetColor("_Color_Scar", color);
                    }
                    if (thisHeadMaterial)
                    {
                        thisHeadMaterial.SetColor("_Color_Scar", color);
                    }
                }
            }

            public void UpdateEyeColor(Color32 color)
            {
                Material thisMaterial = null;
                Material thisHeadMaterial = GetComponent<ModularCustomizationManager>().internalHeadMat;

                if (internalEyeMat)
                {
                    thisMaterial = GetComponent<ModularCustomizationManager>().internalEyeMat;
                }
                else
                {
                    thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                }
                if (thisMaterial)
                {
                    ActiveEyeColor = color;
                    if (directSetEyeColor)
                    {
                        thisMaterial.color = color;
                    }
                    else
                    {
                        if (thisHeadMaterial)
                        {
                            thisHeadMaterial.SetColor("_Color_Eyes", color);
                        }
                        thisMaterial.SetColor("_Color_Eyes", color);
                    }
                }
            }

            public void UpdateHairColor(Color32 color)
            {
                Material thisMaterial = null;
                if (internalHairMat)
                {
                    thisMaterial = GetComponent<ModularCustomizationManager>().internalHairMat;
                }
                else
                {
                    thisMaterial = GetComponent<ModularCustomizationManager>().internalMat;
                }

                if (thisMaterial)
                {

                    ActiveHairColor = color;
                    if (directSetHairColor)
                    {
                        thisMaterial.color = color;
                    }
                    else
                    {
                        thisMaterial.SetColor("_Color_Hair", color);
                    }
                }
            }

            //to update eyebrow models
            public void UpdateEyebrowModel(int hairItem)
            {
                if (hairItem == -1) return;
                if (internalEyebrowModels)
                {
                    if (ActiveEyebrow != null)
                    {
                        ActiveEyebrow.SetActive(false);
                    }
                    GameObject prefab = eyebrowModels[hairItem];
                    if (prefab == null)
                        return;
                    ActiveEyebrowId = hairItem;
                    ActiveEyebrow = prefab;
                    ActiveEyebrow.SetActive(true);
                }
                else
                {
                    if (ActiveEyebrow != null)
                    {
                        Destroy(ActiveEyebrow);
                    }
                    GameObject prefab = eyebrowModels[hairItem];
                    if (prefab == null)
                        return;
                    ActiveEyebrowId = hairItem;
                    ActiveEyebrow = GameObject.Instantiate(prefab);
                    ActiveEyebrow.name = prefab.name;
                    ActiveEyebrow.transform.parent = eyeBrowJoint;
                    ActiveEyebrow.transform.localScale = transform.localScale;
                    ActiveEyebrow.transform.localPosition = Vector3.zero;
                    ActiveEyebrow.transform.localEulerAngles = Vector3.zero;
                    ActiveEyebrow.transform.localScale = Vector3.one;
                }
                MeshRenderer meshRenderer = ActiveEyebrow.GetComponent<MeshRenderer>();
                SkinnedMeshRenderer skinnedMeshRenderer = ActiveEyebrow.GetComponent<SkinnedMeshRenderer>();

                if (!meshRenderer)
                {
                    meshRenderer = ActiveEyebrow.GetComponentInChildren<MeshRenderer>();
                }

                if (!skinnedMeshRenderer)
                {
                    skinnedMeshRenderer = ActiveEyebrow.GetComponentInChildren<SkinnedMeshRenderer>();
                }

                Material material = null;
                Material[] materials = null;
                if(!completelyDisableInstancedMaterials)
                if (skinnedMeshRenderer)
                {
                    material = skinnedMeshRenderer.material;
                    materials = skinnedMeshRenderer.materials;
                    if (materials.Length > 0)
                    {
                        if (allowDifferentHairColors)
                        {
                            materials[0] = internalBeardMat;
                        }
                        else
                        {
                            materials[0] = internalHairMat;
                        }
                        skinnedMeshRenderer.materials = materials;
                    }
                }
                else if (meshRenderer)
                {
                    material = meshRenderer.material;
                    materials = meshRenderer.materials;
                    if (materials.Length > 0)
                    {
                        if (allowDifferentHairColors)
                        {
                            materials[0] = internalEyebrowMat;
                        }
                        else
                        {
                            materials[0] = internalHairMat;
                        }
                        meshRenderer.materials = materials;
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="itemNumber"></param>
            public void UpdateEyeModel(int itemNumber)
            {
                if (itemNumber == -1) return;
                if (internalEyeModels)
                {
                    if (ActiveEye != null)
                    {
                        ActiveEye.SetActive(false);
                    }
                    GameObject prefab = eyeModels[itemNumber];
                    if (prefab == null)
                        return;
                    ActiveEyeId = itemNumber;
                    ActiveEye = prefab;
                    ActiveEye.SetActive(true);
                }
                else
                {
                    if (ActiveEye != null)
                    {
                        Destroy(ActiveEye);
                    }
                    GameObject prefab = eyeModels[itemNumber];
                    if (prefab == null)
                        return;
                    ActiveEyeId = itemNumber;
                    ActiveEye = GameObject.Instantiate(prefab);
                    ActiveEye.name = prefab.name;
                    ActiveEye.transform.parent = eyeJoint;
                    ActiveEye.transform.localScale = transform.localScale;
                    ActiveEye.transform.localPosition = Vector3.zero;
                    ActiveEye.transform.localEulerAngles = Vector3.zero;
                    ActiveEye.transform.localScale = Vector3.one;
                }
                MeshRenderer meshRenderer = ActiveEye.GetComponent<MeshRenderer>();
                SkinnedMeshRenderer skinnedMeshRenderer = ActiveEye.GetComponent<SkinnedMeshRenderer>();

                if (!meshRenderer)
                {
                    meshRenderer = ActiveEye.GetComponentInChildren<MeshRenderer>();
                }

                if (!skinnedMeshRenderer)
                {
                    skinnedMeshRenderer = ActiveEye.GetComponentInChildren<SkinnedMeshRenderer>();
                }

                Material material = null;
                Material[] materials = null;
                if(!completelyDisableInstancedMaterials)
                if (skinnedMeshRenderer)
                {
                    material = skinnedMeshRenderer.material;
                    materials = skinnedMeshRenderer.materials;
                    if (materials.Length > 0)
                    {
                        if (materials.Length > 0)
                        {
                            materials[0] = internalEyeMat;
                            meshRenderer.materials = materials;
                        }
                        skinnedMeshRenderer.materials = materials;
                    }
                }
                else if (meshRenderer)
                {
                    material = meshRenderer.material;
                    materials = meshRenderer.materials;
                    if (materials.Length > 0)
                    {
                        materials[0] = internalEyeMat;
                        meshRenderer.materials = materials;
                    }
                }
            }

            //to update eyebrow models
            public void UpdateTuskModel(int itemNumber)
            {
                if (itemNumber == -1) return;
                if (internalTuskModels)
                {
                    if (ActiveTusk != null)
                    {
                        foreach (var go in ActiveTusk)
                        {
                            go.SetActive(false);
                        }
                        
                    }
                    GameObject prefab = tuskModels[itemNumber];
                    if (prefab == null)
                        return;
                    ActiveTuskId = itemNumber;
                    ActiveTusk.Clear();
                        ActiveTusk.Add(prefab);
                        prefab.SetActive(true);
                }
                else
                {
                    if (ActiveTusk != null)
                    {
                        foreach (var g in ActiveTusk)
                        {
                            Destroy(g);
                        }
                        ActiveTusk.Clear();
                    }
                    GameObject prefab = tuskModels[itemNumber];
                    if (prefab == null)
                        return;
                    ActiveTuskId = itemNumber;
                    GameObject go = GameObject.Instantiate(prefab);
                    go.name = prefab.name;
                    go.transform.parent = mouthJoint;
                    go.transform.localScale = transform.localScale;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localEulerAngles = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                    ActiveTusk.Add(go);
                }

                foreach (var go in ActiveTusk)
                {
                    MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
                    SkinnedMeshRenderer skinnedMeshRenderer = go.GetComponent<SkinnedMeshRenderer>();

                    if (!meshRenderer)
                    {
                        meshRenderer = go.GetComponentInChildren<MeshRenderer>();
                    }

                    if (!skinnedMeshRenderer)
                    {
                        skinnedMeshRenderer = go.GetComponentInChildren<SkinnedMeshRenderer>();
                    }

                    Material material = null;
                    Material[] materials = null;
                    if (!completelyDisableInstancedMaterials)
                        if (skinnedMeshRenderer)
                        {
                            material = skinnedMeshRenderer.material;
                            materials = skinnedMeshRenderer.materials;
                            if (materials.Length > 0)
                            {
                                if (materials.Length > 0)
                                {
                                    materials[0] = internalMat;
                                    meshRenderer.materials = materials;
                                }

                                skinnedMeshRenderer.materials = materials;
                            }
                        }
                        else if (meshRenderer)
                        {
                            material = meshRenderer.material;
                            materials = meshRenderer.materials;
                            if (materials.Length > 0)
                            {
                                materials[0] = internalMat;
                                meshRenderer.materials = materials;
                            }
                        }
                }
            }

#if IPBRInt
            public void UpdateBlendShapes(string stringAsset)
            {
                TextAsset thisAsset = new TextAsset(stringAsset);
                if (infinityPBRIntegration)
                {
                    infinityPBRIntegration.blendShapesManager.LoadRangeFile(thisAsset);
                    ActiveBlendshapes = stringAsset;
                }
            }

            public void UpdateBlendShapePresets(int itemNumber)
            {
                if (blendshapePresetObjects.Count > 0)
                {
                    infinityPBRIntegration.blendShapesManager.LoadRangeFile(blendshapePresetObjects[itemNumber]);
                    ActiveBlendshapePreset = itemNumber;
                }
            }
#endif

            //to update eyebrow models
            public void UpdateEarModel(int itemNumber)
            {
                if (itemNumber == -1) return;
                if (internalEarModels)
                {
                    if (ActiveEar != null)
                    {
                        ActiveEar.SetActive(false);
                    }
                    GameObject prefab = earModels[itemNumber];
                    if (prefab == null)
                        return;
                    ActiveEarId = itemNumber;
                    ActiveEar = prefab;
                    ActiveEar.SetActive(true);
                }
                else
                {
                    if (ActiveEar != null)
                    {
                        Destroy(ActiveEar);
                    }
                    GameObject prefab = earModels[itemNumber];
                    if (prefab == null)
                        return;
                    ActiveEarId = itemNumber;
                    ActiveEar = GameObject.Instantiate(prefab);
                    ActiveEar.name = prefab.name;
                    ActiveEar.transform.parent = earJoint;
                    ActiveEar.transform.localScale = transform.localScale;
                    ActiveEar.transform.localPosition = Vector3.zero;
                    ActiveEar.transform.localEulerAngles = Vector3.zero;
                    ActiveEar.transform.localScale = Vector3.one;
                }
                MeshRenderer meshRenderer = ActiveEar.GetComponent<MeshRenderer>();
                SkinnedMeshRenderer skinnedMeshRenderer = ActiveEar.GetComponent<SkinnedMeshRenderer>();

                if (!meshRenderer)
                {
                    meshRenderer = ActiveEar.GetComponentInChildren<MeshRenderer>();
                }

                if (!skinnedMeshRenderer)
                {
                    skinnedMeshRenderer = ActiveEar.GetComponentInChildren<SkinnedMeshRenderer>();
                }

                Material material = null;
                Material[] materials = null;
                if(!completelyDisableInstancedMaterials)
                if (skinnedMeshRenderer)
                {
                    material = skinnedMeshRenderer.material;
                    materials = skinnedMeshRenderer.materials;
                    if (materials.Length > 0)
                    {
                        if (materials.Length > 0)
                        {
                            materials[0] = internalMat;
                            meshRenderer.materials = materials;
                        }
                        skinnedMeshRenderer.materials = materials;
                    }
                }
                else if (meshRenderer)
                {
                    material = meshRenderer.material;
                    materials = meshRenderer.materials;
                    if (materials.Length > 0)
                    {
                        materials[0] = internalMat;
                        meshRenderer.materials = materials;
                    }
                }
            }

            //to update eyebrow models
            public void UpdateMouthModel(int itemNumber)
            {
                if (itemNumber == -1) return;
                if (internalMouthModels)
                {
                    if (ActiveMouth != null)
                    {
                        ActiveMouth.SetActive(false);
                    }
                    GameObject prefab = mouthModels[itemNumber];
                    if (prefab == null)
                        return;
                    ActiveMouthId = itemNumber;
                    ActiveMouth = prefab;
                    ActiveMouth.SetActive(true);
                }
                else
                {
                    if (ActiveMouth != null)
                    {
                        Destroy(ActiveMouth);
                    }
                    GameObject prefab = mouthModels[itemNumber];
                    if (prefab == null)
                        return;
                    ActiveMouthId = itemNumber;
                    ActiveMouth = GameObject.Instantiate(prefab);
                    ActiveMouth.name = prefab.name;
                    ActiveMouth.transform.parent = mouthJoint;
                    ActiveMouth.transform.localScale = transform.localScale;
                    ActiveMouth.transform.localPosition = Vector3.zero;
                    ActiveMouth.transform.localEulerAngles = Vector3.zero;
                    ActiveMouth.transform.localScale = Vector3.one;
                }
                MeshRenderer meshRenderer = ActiveMouth.GetComponent<MeshRenderer>();
                SkinnedMeshRenderer skinnedMeshRenderer = ActiveMouth.GetComponent<SkinnedMeshRenderer>();

                if (!meshRenderer)
                {
                    meshRenderer = ActiveMouth.GetComponentInChildren<MeshRenderer>();
                }

                if (!skinnedMeshRenderer)
                {
                    skinnedMeshRenderer = ActiveMouth.GetComponentInChildren<SkinnedMeshRenderer>();
                }

                Material material = null;
                Material[] materials = null;
                if(!completelyDisableInstancedMaterials)
                if (skinnedMeshRenderer)
                {
                    material = skinnedMeshRenderer.material;
                    materials = skinnedMeshRenderer.materials;
                    if (materials.Length > 0)
                    {
                        if (materials.Length > 0)
                        {
                            materials[0] = internalMouthMat;
                            meshRenderer.materials = materials;
                        }
                        skinnedMeshRenderer.materials = materials;
                    }
                }
                else if (meshRenderer)
                {
                    material = meshRenderer.material;
                    materials = meshRenderer.materials;
                    if (materials.Length > 0)
                    {
                        materials[0] = internalMouthMat;
                        meshRenderer.materials = materials;
                    }
                }
            }

            public void UpdateEyeMaterial(int ModelValue)
            {
                if (eyeMatList.Count > 0)
                {
                    SwitchMaterialWorker(internalEyeMat, eyeMatList[ModelValue], eyeShader, "internalEyeMat", BodyType.Eye);
                    ActiveEyeMaterialId = ModelValue;
                }
            }

            public void UpdateMouthMaterial(int ModelValue)
            {
                if (mouthMatList.Count > 0)
                {
                    SwitchMaterialWorker(internalMouthMat, mouthMatList[ModelValue], mouthShader, "internalMouthMat", BodyType.Mouth);
                    ActiveMouthMaterialId = ModelValue;
                }
            }

            public void UpdateSkinMaterial(int ModelValue)
            {
                if (bodyMatList.Count > 0)
                {
                    SwitchMaterialWorker(internalMat, bodyMatList[ModelValue], bodyShader, "internalMat", BodyType.Skin);
                    ActiveSkinMaterialId = ModelValue;
                }

            }
             public void UpdateHairMaterial(int ModelValue)
            {
                    if (hairMatList.Count > 0)
                    {
                        SwitchMaterialWorker(internalHairMat, hairMatList[ModelValue], hairShader, "internalHairMat", BodyType.Hair);
                        ActiveHairMaterialId = ModelValue;
                    }
            }

            // update Beard Models
            public void UpdateBeardModel(int hairItem)
            {
                if (hairItem == -1) return;
                if (internalBeardModels)
                {
                    if (ActiveBeard != null)
                    {
                        ActiveBeard.SetActive(false);
                    }
                    GameObject newbeardPrefab = beardModels[hairItem];
                    if (newbeardPrefab == null)
                        return;
                    ActiveBeardId = hairItem;
                    ActiveBeard = newbeardPrefab;
                    ActiveBeard.SetActive(true);
                }
                else
                {

                    if (ActiveBeard != null)
                    {
                        Destroy(ActiveBeard);
                    }

                    GameObject beardPrefab = beardModels[hairItem];
                    if (beardPrefab == null)
                        return;
                    ActiveBeardId = hairItem;
                    ActiveBeard = (GameObject)Instantiate(beardPrefab, beardJoint.position, beardJoint.rotation);
                    ActiveBeard.name = beardPrefab.name; // To get rid of the Clone that gets added to the end of the name
                    ActiveBeard.transform.parent = beardJoint;
                    ActiveBeard.transform.localPosition = beardPrefab.transform.localPosition;
                    ActiveBeard.transform.localScale = beardPrefab.transform.localScale;
                    if(!ActiveBeard.activeSelf)
                        ActiveBeard.SetActive(true);

                }
                MeshRenderer meshRenderer = ActiveBeard.GetComponent<MeshRenderer>();
                SkinnedMeshRenderer skinnedMeshRenderer = ActiveBeard.GetComponent<SkinnedMeshRenderer>();

                if (!meshRenderer)
                {
                    meshRenderer = ActiveBeard.GetComponentInChildren<MeshRenderer>();
                }

                if (!skinnedMeshRenderer)
                {
                    skinnedMeshRenderer = ActiveBeard.GetComponentInChildren<SkinnedMeshRenderer>();
                }

                Material material = null;
                Material[] materials = null;
                if(!completelyDisableInstancedMaterials)
                if (skinnedMeshRenderer)
                {
                    material = skinnedMeshRenderer.material;
                    materials = skinnedMeshRenderer.materials;
                    if (materials.Length > 0)
                    {
                        if (allowDifferentHairColors)
                        {
                            materials[0] = internalBeardMat;
                        }
                        else
                        {
                            materials[0] = internalHairMat;
                        }
                        skinnedMeshRenderer.materials = materials;
                    }
                }
                else if (meshRenderer)
                {
                    material = meshRenderer.material;
                    materials = meshRenderer.materials;
                    if (materials.Length > 0)
                    {
                        if (allowDifferentHairColors)
                        {
                            materials[0] = internalBeardMat;
                        }
                        else
                        {
                            materials[0] = internalHairMat;
                        }
                        meshRenderer.materials = materials;
                    }
                }
            }

            // to Update Hair Model
            public void UpdateHairModel(int hairItem)
            {
                if (hairItem == -1) return;
               
                if (internalHairModels)
                {

                    if (ActiveHair != null)
                    {
                        ActiveHair.SetActive(false);
                    }
                    GameObject prefab = hairModels[hairItem];
                    if (prefab == null)
                        return;

                    ActiveHairId = hairItem;
                    ActiveHair = prefab;
                    ActiveHair.SetActive(true);
                }
                else
                {
                    if (ActiveHair != null)
                    {
                        Destroy(ActiveHair);
                    }

                    GameObject prefab = hairModels[hairItem];
                    if (prefab == null)
                        return;

                    ActiveHairId = hairItem;
                    ActiveHair = (GameObject) Instantiate(prefab, parentJoint.position, parentJoint.rotation);
                    ActiveHair.name = prefab.name;
                    ActiveHair.transform.parent = parentJoint;
                    ActiveHair.transform.localPosition = prefab.transform.localPosition;
                    ActiveHair.transform.localScale = prefab.transform.localScale;
                    if(!ActiveHair.activeSelf)
                        ActiveHair.SetActive(true);

                }

                MeshRenderer meshRenderer = ActiveHair.GetComponent<MeshRenderer>();
                SkinnedMeshRenderer skinnedMeshRenderer = ActiveHair.GetComponent<SkinnedMeshRenderer>();

                if (!meshRenderer)
                {
                    meshRenderer = ActiveHair.GetComponentInChildren<MeshRenderer>();
                }

                if (!skinnedMeshRenderer)
                {
                    skinnedMeshRenderer = ActiveHair.GetComponentInChildren<SkinnedMeshRenderer>();
                }
                Material material = null;
                Material[] materials = null;
                if(!completelyDisableInstancedMaterials)
                if (skinnedMeshRenderer)
                {
                    material = skinnedMeshRenderer.material;
                    materials = skinnedMeshRenderer.materials;
                    if (materials.Length > 0)
                    {
                        if (materials.Length > 0)
                        {
                            materials[0] = internalHairMat;
                        }
                        skinnedMeshRenderer.materials = materials;
                    }
                }
                else if (meshRenderer)
                {
                    material = meshRenderer.material;
                    materials = meshRenderer.materials;
                    if (materials != null)
                    {
                        if (materials.Length > 0)
                        {
                            materials[0] = internalHairMat;
                        }
                        meshRenderer.materials = materials;
                    }
                }
            }


            public void nHanceIt(GameObject targetBody)
            {
                if (ActiveTorso == null || targetBody == null)
                {
                    return;
                }

                ActiveTorso.Clear();
                ActiveTorso.Add(targetBody);
            }


            public void ChangeBody(GameObject targetBody)
            {
                if (ActiveTorso == null || targetBody == null)
                {
                    return;
                }
               // if (!DefaultTorso)
               // {
                    DefaultTorso = ActiveTorso;
               // }
                if (replacementObject)
                {
                    if (!replacementObject.activeInHierarchy)
                    {
                        replacementObject.SetActive(true);
                    }

                    foreach (var g in DefaultTorso)
                    {
                       // Debug.LogError(g+" "+g.activeSelf+" Set false");
                        g.SetActive(false);
                    }
                  
                    ChangeRenderer(replacementObject, targetBody);

                }
                else
                {
                    foreach (var go in DefaultTorso)
                    {
                        ChangeRenderer(go, targetBody);
                    }
                }
//Debug.LogError("Torso Clear");
                ActiveTorso.Clear();
                ActiveTorso.Add(targetBody);
            }

            public void ChangeBody(ModularReplacementItem targetBody)
            {
                if (ActiveTorso == null || targetBody == null)
                {
                    return;
                }

                if (headReplacementObject != null)
                {
                   // if (!DefaultHead)
                   // {
                        DefaultHead = ActiveHead;
                        foreach (var go in DefaultHead)
                        {
                            go.SetActive(false);
                        }
                    //}
                    if (!headReplacementObject.activeInHierarchy)
                    {
                        headReplacementObject.SetActive(true);
                    }
                }
                if (torsoReplacementObject != null)
                {
                  //  if (!DefaultTorso)
                   // {
                        DefaultTorso = ActiveTorso;
                        foreach (var go in DefaultTorso)
                        {
                            go.SetActive(false);
                        }
                  //  }

                    if (!torsoReplacementObject.activeInHierarchy)
                    {
                        torsoReplacementObject.SetActive(true);
                    }
                }
                if (upperArmsReplacementObject != null)
                {
                  //  if (!DefaultUpperArms)
                  //  {
                        DefaultUpperArms = ActiveUpperArms;
                        foreach (var go in DefaultUpperArms)
                        {
                            go.SetActive(false);
                        }
                  //  }
                    if (!upperArmsReplacementObject.activeInHierarchy)
                    {
                        upperArmsReplacementObject.SetActive(true);
                    }
                }
                if (lowerArmsReplacementObject != null)
                {
                  //  if (!DefaultLowerArms)
                 //   {
                        DefaultLowerArms = ActiveLowerArms;
                        foreach (var go in DefaultLowerArms)
                        {
                            go.SetActive(false);
                        }
                  //  }
                    if (!lowerArmsReplacementObject.activeInHierarchy)
                    {
                        lowerArmsReplacementObject.SetActive(true);
                    }
                }
                if (hipsReplacementObject != null)
                {
                 //   if (!DefaultHips)
                 //   {
                        DefaultHips = ActiveHips;
                        foreach (var go in DefaultHips)
                        {
                            go.SetActive(false);
                        }
                 //   }
                    if (!hipsReplacementObject.activeInHierarchy)
                    {
                        hipsReplacementObject.SetActive(true);
                    }
                }
                if (lowerLegsReplacementObject != null)
                {
                   // if (!DefaultLowerLegs)
                  //  {
                        DefaultLowerLegs = ActiveLowerLegs;
                        foreach (var go in DefaultLowerLegs)
                        {
                            go.SetActive(false);
                        }
                 //   }
                    if (!lowerLegsReplacementObject.activeInHierarchy)
                    {
                        lowerLegsReplacementObject.SetActive(true);
                    }
                }
                if (feetReplacementObject != null)
                {
                  //  if (!DefaultFeet)
                  //  {
                        DefaultFeet = ActiveFeet;
                        foreach (var go in DefaultFeet)
                        {
                            go.SetActive(false);
                        }
                   // }
                    if (!feetReplacementObject.activeInHierarchy)
                    {
                        feetReplacementObject.SetActive(true);
                    }
                }
                if (handsReplacementObject != null)
                {
                 //   if (!DefaultHands)
                  //  {
                        DefaultHands = ActiveHands;
                        foreach (var go in DefaultHands)
                        {
                            go.SetActive(false);
                        }
                   // }
                    if (!handsReplacementObject.activeInHierarchy)
                    {
                        handsReplacementObject.SetActive(true);
                    }
                }


                foreach (ReplacementItems thisItem in targetBody.replacementItems)
                {
                    if (thisItem.Equals(ReplacementItems.Head))
                    {
                        Transform thisobject = FindDeepChild(targetBody.fullModelReplacement.transform, targetBody.HeadModelReplacement);
                        if (thisobject)
                        {
                            GameObject thisGameObject = thisobject.gameObject;
                            ChangeRenderer(headReplacementObject, thisGameObject);

                            foreach (var go in ActiveHead)
                            {
                                go.SetActive(false);
                            }
                            ActiveHead.Clear();
                            ActiveHead.Add(thisGameObject);

                        }
                    }
                    if (thisItem.Equals(ReplacementItems.Torso))
                    {
                        Transform thisobject = FindDeepChild(targetBody.fullModelReplacement.transform, targetBody.TorsoModelReplacement);
                        if (thisobject)
                        {
                            GameObject thisGameObject = thisobject.gameObject;
                            ChangeRenderer(torsoReplacementObject, thisGameObject);

                            foreach (var go in ActiveTorso)
                            {
                              //  Debug.LogError(go+" "+go.activeSelf+" set to false");
                                go.SetActive(false);
                            }
                            ActiveTorso.Clear();
                            ActiveTorso.Add(thisGameObject);

                        }
                    }
                    if (thisItem.Equals(ReplacementItems.UpperArms))
                    {
                        Transform thisobject = FindDeepChild(targetBody.fullModelReplacement.transform, targetBody.UpperArmsModelReplacement);
                        if (thisobject)
                        {
                            GameObject thisGameObject = thisobject.gameObject;
                            ChangeRenderer(upperArmsReplacementObject, thisGameObject);

                            
                            foreach (var go in ActiveUpperArms)
                            {
                                go.SetActive(false);
                            }
                            ActiveUpperArms.Clear();
                            ActiveUpperArms.Add(thisGameObject);

                        }
                    }
                    if (thisItem.Equals(ReplacementItems.LowerArms))
                    {
                        Transform thisobject = FindDeepChild(targetBody.fullModelReplacement.transform, targetBody.LowerArmsModelReplacement);
                        if (thisobject)
                        {
                            GameObject thisGameObject = thisobject.gameObject;
                            ChangeRenderer(lowerArmsReplacementObject, thisGameObject);

                            foreach (var go in ActiveLowerArms)
                            {
                                go.SetActive(false);
                            }
                            ActiveLowerArms.Clear();
                            ActiveLowerArms.Add(thisGameObject);

                        }
                    }
                    if (thisItem.Equals(ReplacementItems.Hips))
                    {
                        Transform thisobject = FindDeepChild(targetBody.fullModelReplacement.transform, targetBody.HipsModelReplacement);
                        if (thisobject)
                        {
                            GameObject thisGameObject = thisobject.gameObject;
                            ChangeRenderer(hipsReplacementObject, thisGameObject);

                            foreach (var go in ActiveHips)
                            {
                                go.SetActive(false);
                            }
                            ActiveHips.Clear();
                            ActiveHips.Add(thisGameObject);

                        }
                    }
                    if (thisItem.Equals(ReplacementItems.LowerLegs))
                    {
                        Transform thisobject = FindDeepChild(targetBody.fullModelReplacement.transform, targetBody.LowerLegsModelReplacement);
                        if (thisobject)
                        {
                            GameObject thisGameObject = thisobject.gameObject;
                            ChangeRenderer(lowerLegsReplacementObject, thisGameObject);

                            foreach (var go in ActiveLowerLegs)
                            {
                                go.SetActive(false);
                            }
                            ActiveLowerLegs.Clear();
                            ActiveLowerLegs.Add(thisGameObject);
                       
                        }
                    }
                    if (thisItem.Equals(ReplacementItems.Feet))
                    {
                        Transform thisobject = FindDeepChild(targetBody.fullModelReplacement.transform, targetBody.FeetModelReplacement);
                        if (thisobject)
                        {
                            GameObject thisGameObject = thisobject.gameObject;
                            ChangeRenderer(feetReplacementObject, thisGameObject);
                            foreach (var go in ActiveFeet)
                            {
                                go.SetActive(false);
                            }
                            ActiveFeet.Clear();
                            ActiveFeet.Add(thisGameObject);
                        }
                    }
                    if (thisItem.Equals(ReplacementItems.Hands))
                    {
                        Transform thisobject = FindDeepChild(targetBody.fullModelReplacement.transform, targetBody.HandsModelReplacement);
                        if (thisobject)
                        {
                            GameObject thisGameObject = thisobject.gameObject;
                            ChangeRenderer(handsReplacementObject, thisGameObject);

                            foreach (var go in ActiveHands)
                            {
                                go.SetActive(false);
                            }
                            ActiveHands.Clear();
                            ActiveHands.Add(thisGameObject);
                        }
                    }
                }
            }


            private void ChangeRenderer(GameObject current, GameObject target)
            {
                SkinnedMeshRenderer currentSkin = current.GetComponent<SkinnedMeshRenderer>();
                if (currentSkin == null)
                {
                    currentSkin = current.GetComponentInChildren<SkinnedMeshRenderer>();
                }

                SkinnedMeshRenderer targetSkin = target.GetComponent<SkinnedMeshRenderer>();
                if (targetSkin == null)
                {
                    targetSkin = target.GetComponentInChildren<SkinnedMeshRenderer>();
                }

                if (currentSkin == null || targetSkin == null)
                {
                    return;
                }

                SwapSkinnedMeshRenderer(currentSkin, targetSkin);
                currentSkin.sharedMesh = targetSkin.sharedMesh;
                currentSkin.sharedMaterials = targetSkin.sharedMaterials;

            }


            private void SwapSkinnedMeshRenderer(SkinnedMeshRenderer currentSkin, SkinnedMeshRenderer targetSkin)
            {
                Transform[] newMeshBones = new Transform[targetSkin.bones.Length];

                Dictionary<Transform, string> missingBones = new Dictionary<Transform, string>();

                for (int i = 0; i < targetSkin.bones.Length; i++)
                {

                    GameObject bone = targetSkin.bones[i].gameObject;
                    newMeshBones[i] = FindDeepChild(bone.name);
                    if (newMeshBones[i])
                    {
                        if (newMeshBones[i].name.Contains("Ribbon"))
                        {
                            newMeshBones[i].localPosition = bone.transform.localPosition;
                            newMeshBones[i].localRotation = bone.transform.localRotation;
                            newMeshBones[i].localScale = bone.transform.localScale;
                        }
                    }
                }
                currentSkin.bones = newMeshBones;
            }


            public void UpdateBodyModel(string prefabName, BodyType bodyType)
            {
               string[] prefabs =  prefabName.Split('|');
               // List<string> presabs = 
                List<Transform> originalprefabItem = new List<Transform>();
                List<Transform> prefabItem = new List<Transform>();
                List<GameObject> gameitem = new List<GameObject>();
                List<GameObject> originalPrefab = new List<GameObject>();
                foreach (var p in prefabs)
                {
                    Transform t = FindDeepChild(p);
                    if (t)
                        prefabItem.Add(t);
                }
                //prefabItem = FindDeepChild(prefabName);
                List<string> defaultPrefabName = new List<string>();

                if (prefabItem.Count == 0 && bodyType == BodyType.Torso)
                {

                    if (modularModelSwapping)
                    {
                        foreach (var p in prefabs)
                        {
                            ModularReplacementItem ReplacementItem = replacementItems.Find(x => x.ReplacementName == p);
                            if (ReplacementItem)
                            {
                                ChangeBody(ReplacementItem);
                            }
                        }
                    }
                    else
                    {
                        foreach (var p in prefabs)
                        {
                            GameObject TorsoItem = torsoModels.Find(x => x.name == p);
                            if (TorsoItem)
                            {
#if ModularNHance
                            if (nHanceModelSwapping)
                            {
                                nHanceIt(TorsoItem);
                            }
                            else
#endif
                                ChangeBody(TorsoItem);
                            }
                        }
                    }
                }
                else
                {
                    if (replacementObject)
                    {
                        if (replacementObject.activeInHierarchy)
                        {
                            replacementObject.SetActive(false);
                            //DefaultTorso.SetActive(true);

                        }
                    }
                }

                if (bodyType.Equals(BodyType.Head))
                {
                    if (ActiveHead != null)
                    {
                        foreach (var g in ActiveHead)
                        {
                            originalprefabItem.Add(g.transform);
                        }
                      //  originalprefabItem = ActiveHead.transform;
                    }
                    else
                    {
                        if (gender == characterGender.Female)
                        {
                            foreach (var s in defaultFemaleHeadName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                 originalprefabItem.Add(t);
                            }
                           
                        }
                        else
                        {
                            foreach (var s in defaultHeadName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                    originalprefabItem.Add(t);
                            }
                        }
                    }

                    if (originalprefabItem.Count>0)
                    {
                        foreach (var s in originalprefabItem)
                        {
                            defaultPrefabName.Add(s.name);
                        }
                        //defaultPrefabName = originalprefabItem.name;
                    }
                }
                else if (bodyType.Equals(BodyType.Helmet))
                {
                    if (ActiveHeadCovering != null)
                    {
                        foreach (var g in ActiveHead)
                        {
                            originalprefabItem.Add(g.transform);
                        }
                        //  originalprefabItem = ActiveHeadCovering.transform;
                    }

                    if (originalprefabItem.Count>0)
                    {
                        foreach (var s in originalprefabItem)
                        {
                            defaultPrefabName.Add(s.name);
                        }
                        //defaultPrefabName = originalprefabItem.name;
                    }
                }
                else if (bodyType.Equals(BodyType.Face))
                {
                    if (ActiveFace != null)
                    {
                        foreach (var g in ActiveFace)
                        {
                            originalprefabItem.Add(g.transform);
                        }
                        //  originalprefabItem = ActiveFace.transform;
                    }
                    else
                    {
                        if (gender == characterGender.Female)
                        {
                         //   originalprefabItem = FindDeepChild(defaultFemaleFaceName);
                            foreach (var s in defaultFemaleFaceName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                    originalprefabItem.Add(t);
                            }
                        }
                        else
                        {
                           // originalprefabItem = FindDeepChild(defaultFaceName);
                            foreach (var s in defaultFaceName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                    originalprefabItem.Add(t);
                            }
                        }
                    }

                    if (originalprefabItem.Count>0)
                    {
                        foreach (var s in originalprefabItem)
                        {
                            defaultPrefabName.Add(s.name);
                        }
                        //defaultPrefabName = originalprefabItem.name;
                    }
                }
                else if (bodyType.Equals(BodyType.Hands))
                {
                    if (ActiveHands != null)
                    {
                        foreach (var g in ActiveHands)
                        {
                            originalprefabItem.Add(g.transform);
                        }
                        //  originalprefabItem = ActiveHands.transform;
                    }
                    else
                    {
                        if (gender == characterGender.Female)
                        {
                           // originalprefabItem = FindDeepChild(defaultFemaleHandName);
                            foreach (var s in defaultFemaleHandName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                    originalprefabItem.Add(t);
                            }
                        }
                        else
                        {
                         //   originalprefabItem = FindDeepChild(defaultHandName);
                            foreach (var s in defaultHandName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                    originalprefabItem.Add(t);
                            }
                        }
                    }

                    if (originalprefabItem.Count>0)
                    {
                        foreach (var s in originalprefabItem)
                        {
                            defaultPrefabName.Add(s.name);
                        }
                        //defaultPrefabName = originalprefabItem.name;
                    }
                }
                else if (bodyType.Equals(BodyType.Hips))
                {
                    if (ActiveHips != null)
                    {
                        foreach (var g in ActiveHips)
                        {
                            originalprefabItem.Add(g.transform);
                        }
                        //  originalprefabItem = ActiveHips.transform;
                    }
                    else
                    {
                        if (gender == characterGender.Female)
                        {
                          //  originalprefabItem = FindDeepChild(defaultFemaleHipsName);
                            foreach (var s in defaultFemaleHipsName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                    originalprefabItem.Add(t);
                            }
                        }
                        else
                        {
                           // originalprefabItem = FindDeepChild(defaultHipsName);
                            foreach (var s in defaultHipsName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                    originalprefabItem.Add(t);
                            }
                        }
                    }

                    if (originalprefabItem.Count>0)
                    {
                        foreach (var s in originalprefabItem)
                        {
                            defaultPrefabName.Add(s.name);
                        }
                        //defaultPrefabName = originalprefabItem.name;
                    }
                }
                else if (bodyType.Equals(BodyType.LowerArms))
                {
                    if (ActiveLowerArms != null)
                    {
                        foreach (var g in ActiveLowerArms)
                        {
                            originalprefabItem.Add(g.transform);
                        }
                        //  originalprefabItem = ActiveLowerArms.transform;
                    }
                    else
                    {
                        if (gender == characterGender.Female)
                        {
                           // originalprefabItem = FindDeepChild(defaultFemaleLowerArmName);
                            foreach (var s in defaultFemaleLowerArmName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                    originalprefabItem.Add(t);
                            }
                        }
                        else
                        {
                           // originalprefabItem = FindDeepChild(defaultLowerArmName);
                            foreach (var s in defaultLowerArmName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                    originalprefabItem.Add(t);
                            }
                        }
                    }

                    if (originalprefabItem.Count>0)
                    {
                        foreach (var s in originalprefabItem)
                        {
                            defaultPrefabName.Add(s.name);
                        }
                        //defaultPrefabName = originalprefabItem.name;
                    }
                }
                else if (bodyType.Equals(BodyType.LowerLegs))
                {
                    if (ActiveLowerLegs != null)
                    {
                        foreach (var g in ActiveLowerLegs)
                        {
                            originalprefabItem.Add(g.transform);
                        }
                        //  originalprefabItem = ActiveLowerLegs.transform;
                    }
                    else
                    {
                        if (gender == characterGender.Female)
                        {
                           // originalprefabItem = FindDeepChild(defaultFemaleLowerLegName);
                            foreach (var s in defaultFemaleLowerLegName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                    originalprefabItem.Add(t);
                            }
                        }
                        else
                        {
                          //  originalprefabItem = FindDeepChild(defaultLowerLegName);
                            foreach (var s in defaultLowerLegName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                    originalprefabItem.Add(t);
                            }
                        }
                    }

                    if (originalprefabItem.Count>0)
                    {
                        foreach (var s in originalprefabItem)
                        {
                            defaultPrefabName.Add(s.name);
                        }
                        //defaultPrefabName = originalprefabItem.name;
                    }
                }

                if (bodyType.Equals(BodyType.Feet))
                {
                    if (ActiveFeet != null)
                    {
                        foreach (var g in ActiveFeet)
                        {
                            originalprefabItem.Add(g.transform);
                        }
                        //  originalprefabItem = ActiveFeet.transform;
                    }
                    else
                    {
                        if (gender == characterGender.Female)
                        {
                          //  originalprefabItem = FindDeepChild(defaultFemaleFeetName);
                            foreach (var s in defaultFemaleFeetName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                    originalprefabItem.Add(t);
                            }
                        }
                        else
                        {
                            //originalprefabItem = FindDeepChild(defaultFeetName);
                            foreach (var s in defaultFeetName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                    originalprefabItem.Add(t);
                            }
                        }
                    }

                    if (originalprefabItem.Count>0)
                    {
                        foreach (var s in originalprefabItem)
                        {
                            defaultPrefabName.Add(s.name);
                        }
                        //defaultPrefabName = originalprefabItem.name;
                    }
                }
                else if (bodyType.Equals(BodyType.Torso))
                {
                  //  Debug.LogError("body type Torso");
                    if (ActiveTorso != null)
                    {
                        foreach (var g in ActiveTorso)
                        {
                            originalprefabItem.Add(g.transform);
                        }
                        //  originalprefabItem = ActiveTorso.transform;
                    }
                    else
                    {
                        if (gender == characterGender.Female)
                        {
                          //  originalprefabItem = FindDeepChild(defaultFemaleTorsoName);
                            foreach (var s in defaultFemaleTorsoName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                    originalprefabItem.Add(t);
                            }
                        }
                        else
                        {
                           // originalprefabItem = FindDeepChild(defaultTorsoName);
                            foreach (var s in defaultTorsoName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                    originalprefabItem.Add(t);
                            }
                        }
                    }

                    if (originalprefabItem.Count>0)
                    {
                        foreach (var s in originalprefabItem)
                        {
                            defaultPrefabName.Add(s.name);
                        }
                        //defaultPrefabName = originalprefabItem.name;
                    }
                    //DefaultTorso.SetActive(true);

                }
                else if (bodyType.Equals(BodyType.Upperarms))
                {
                    if (ActiveUpperArms != null)
                    {
                        foreach (var g in ActiveUpperArms)
                        {
                            originalprefabItem.Add(g.transform);
                        }
                        //  originalprefabItem = ActiveUpperArms.transform;
                    }
                    else
                    {
                        if (gender == characterGender.Female)
                        {
                           // originalprefabItem = FindDeepChild(defaultFemaleUpperArmName);
                            foreach (var s in defaultFemaleUpperArmName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                    originalprefabItem.Add(t);
                            }
                        }
                        else
                        {
                          //  originalprefabItem = FindDeepChild(defaultUpperArmName);
                            foreach (var s in defaultUpperArmName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                    originalprefabItem.Add(t);
                            }
                        }
                    }

                    if (originalprefabItem.Count>0)
                    {
                        foreach (var s in originalprefabItem)
                        {
                            defaultPrefabName.Add(s.name);
                        }
                        //defaultPrefabName = originalprefabItem.name;
                    }
                }
                else if (bodyType.Equals(BodyType.Cape))
                {
                    if (gender == characterGender.Female)
                    {
                        if (defaultFemaleCapeName.Count == 0)
                        {
                            foreach (var p in prefabs)
                            {
                                defaultFemaleCapeName.Add(p);
                            }
                        }
                    }
                    else
                    {
                        if (defaultCapeName.Count == 0)
                        {
                            foreach (var p in prefabs)
                            {
                                defaultCapeName.Add(p);
                            }
                        }
                    }

                    if (ActiveCape != null)
                    {
                        foreach (var g in ActiveCape)
                        {
                            originalprefabItem.Add(g.transform);
                        }
                        //  originalprefabItem = ActiveCape.transform;
                    }
                    else
                    {
                        if (gender == characterGender.Female)
                        {
                           // originalprefabItem = FindDeepChild(defaultFemaleCapeName);
                            foreach (var s in defaultFemaleCapeName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                    originalprefabItem.Add(t);
                            }
                        }
                        else
                        {
                           // originalprefabItem = FindDeepChild(defaultCapeName);
                            foreach (var s in defaultCapeName)
                            {
                                Transform t = FindDeepChild(s);
                                if(t)
                                    originalprefabItem.Add(t);
                            }
                        }
                    }

                    if (originalprefabItem.Count>0)
                    {
                        foreach (var s in originalprefabItem)
                        {
                            defaultPrefabName.Add(s.name);
                        }
                        //defaultPrefabName = originalprefabItem.name;
                    }
                }

                if (originalprefabItem.Count>0)
                {
                    foreach (var t in originalprefabItem)
                    {
                        originalPrefab.Add(t.gameObject);
                    }
                    //originalPrefab = originalprefabItem.gameObject;
                }

                if (prefabItem.Count>0)
                {
                    foreach (var t in prefabItem)
                    {
                        gameitem.Add(t.gameObject);
                    }
                   
                }


                if (!defaultPrefabName.Except(prefabs).Any())
                {
                    if (originalprefabItem.Count > 0 && gameitem.Count > 0)
                    {
                        foreach (var go in originalPrefab)
                        {
                        //    Debug.LogError(go+" "+go.activeSelf+" Set false");
                            if( go.gameObject.activeSelf)
                                go.SetActive(false);
                        }
                       // originalPrefab.SetActive(false);
                       foreach (var go in originalPrefab)
                       {
                        //   Debug.LogError(go+" "+go.activeSelf+" Set true");
                           if( !go.gameObject.activeSelf)
                               go.SetActive(true);
                           foreach (Transform item in go.transform)
                           {
                               if (!item.gameObject.activeSelf)
                               {
                              //     Debug.LogError( item.gameObject+" "+go.activeSelf+" Set true");
                                   if( !item.gameObject.activeSelf)
                                    item.gameObject.SetActive(true);
                               }
                           }
                       }

                       if (gameitem.Count>0)
                            if (bodyType.Equals(BodyType.Hands))
                            {
                                ActiveHands.Clear();
                                
                                ActiveHands.AddRange(gameitem);
                            }
                            else if (bodyType.Equals(BodyType.LowerArms))
                            {
                                
                                ActiveLowerArms.Clear();
                                ActiveLowerArms.AddRange(gameitem);
                            }
                            else if (bodyType.Equals(BodyType.Upperarms))
                            {
                                ActiveUpperArms.Clear();
                                ActiveUpperArms.AddRange( gameitem);
                            }
                            else if (bodyType.Equals(BodyType.Torso))
                            {
                                ActiveTorso.Clear();
                                ActiveTorso.AddRange(gameitem);
                            }
                            else if (bodyType.Equals(BodyType.Hips))
                            {
                                ActiveHips.Clear();
                                ActiveHips.AddRange(gameitem);
                            }
                            else if (bodyType.Equals(BodyType.LowerLegs))
                            {
                                ActiveLowerLegs.Clear();
                                ActiveLowerLegs.AddRange(gameitem);
                            }
                            else if (bodyType.Equals(BodyType.Head))
                            {
                                ActiveHead.Clear();
                                ActiveHead.AddRange(gameitem);
                            }
                            // else if (bodyType.Equals(BodyType.Helmet))
                            //     ActiveHeadCovering = gameitem;
                            else if (bodyType.Equals(BodyType.Feet))
                            {
                                ActiveFeet.Clear();
                                ActiveFeet.AddRange(gameitem);
                            }
                        // else if (bodyType.Equals(BodyType.Cape))
                        //ActiveCape = gameitem;

                    }
                    else if (gameitem.Count>0)
                    {
                        foreach (var go in gameitem)
                        {
                          //  Debug.LogError(go.name+" " +go.activeSelf+" Set true");
                            go.SetActive(true);
                        }
                       
                    }
                }

                var ama = GetComponent<AtavismMobAppearance>();
                if(ama)
                    ama.ReApplyEquipDisplay();
            }


            //Breadth-first search
            public Transform FindDeepChild(string aName)
            {
                Queue<Transform> queue = new Queue<Transform>();
                queue.Enqueue(transform);
                while (queue.Count > 0)
                {
                    var c = queue.Dequeue();
                    if (c.name == aName)
                        return c;
                    foreach (Transform t in c)
                        queue.Enqueue(t);
                }
                return null;
            }


            public static Transform FindDeepChild(Transform aParent, string aName)
            {
                foreach (Transform child in aParent)
                {
                    if (child.name == aName)
                        return child;
                    var result = FindDeepChild(child, aName);
                    if (result != null)
                        return result;
                }
                return null;
            }

            public void SetFaith(string value)
            {
                ActiveFaith = value;
            }

#if IPBRInt
            public void SetBlendShapes()
            {
                string theseShapes = infinityPBRIntegration.CreateBlendshapeDataV3();
                UpdateBlendShapes(theseShapes);
            }

            public void SetBlendShapePreset(int value)
            {
                UpdateBlendShapePresets(value);
            }
#endif



            // Update models Do not touch
            [HideInInspector] public int ActiveHairMaterialId { get; private set; }
            [HideInInspector] public int ActiveSkinMaterialId { get; private set; }
            [HideInInspector] public int ActiveEyeMaterialId { get; private set; }
            [HideInInspector] public int ActiveMouthMaterialId { get; private set; }
            

            [HideInInspector] public string ActiveFaith { get; private set; }
            [HideInInspector] public int ActiveHairId { get; private set; }
            [HideInInspector] public int ActiveBeardId { get; private set; }
            [HideInInspector] public int ActiveEyebrowId { get; private set; }
            [HideInInspector] public int ActiveMouthId { get; private set; }
            [HideInInspector] public int ActiveTuskId { get; private set; }
            [HideInInspector] public int ActiveEarId { get; private set; }
            [HideInInspector] public int ActiveEyeId { get; private set; }
            [HideInInspector] public GameObject ActiveHair { get; private set; }
            [HideInInspector] public GameObject ActiveBeard { get; private set; }
            [HideInInspector] public GameObject ActiveEyebrow { get; private set; }
            [HideInInspector] public GameObject ActiveEye { get; private set; }
            [HideInInspector] public GameObject ActiveEar { get; private set; }
            [HideInInspector] public GameObject ActiveMouth { get; private set; }
            [HideInInspector] public List<GameObject> ActiveTusk { get; private set; }
            [HideInInspector] public List<GameObject> ActiveHead { get; private set; }

            [HideInInspector] public List<GameObject> ActiveHeadCovering { get; private set; }
            [HideInInspector] public List<GameObject> ActiveHands { get; private set; }
            [HideInInspector] public List<GameObject> ActiveLowerArms { get; private set; }
            [HideInInspector] public List<GameObject> ActiveUpperArms { get; private set; }
            [HideInInspector] public List<GameObject> ActiveTorso { get; private set; }
            [HideInInspector] public List<GameObject> ActiveHips { get; private set; }
            [HideInInspector] public List<GameObject> ActiveLowerLegs { get; private set; }
            [HideInInspector] public List<GameObject> ActiveFeet { get; private set; }
            [HideInInspector] public List<GameObject> ActiveCape { get; private set; }

            [HideInInspector] public List<GameObject> ActiveFace { get; private set; }

            [HideInInspector] public int ActiveBlendshapePreset { get; private set; }
            [HideInInspector] public string ActiveBlendshapes { get; private set; }
            [HideInInspector] public Color32 ActiveBodyColor { get; private set; }
            [HideInInspector] public Color32 ActiveMouthColor { get; private set; }
            [HideInInspector] public Color32 ActiveHairColor { get; private set; }
            [HideInInspector] public Color32 ActiveBeardColor { get; private set; }
            [HideInInspector] public Color32 ActiveEyebrowColor { get; private set; }
            [HideInInspector] public Color32 ActiveScarColor { get; private set; }
            [HideInInspector] public Color32 ActiveStubbleColor { get; private set; }
            [HideInInspector] public Color32 ActiveBodyArtColor { get; private set; }
            [HideInInspector] public Color32 ActiveEyeColor { get; private set; }
            [HideInInspector] public Color32 ActiveLeatherSecondaryColor { get; private set; }
            [HideInInspector] public Color32 ActiveLeatherPrimaryColor { get; private set; }
            [HideInInspector] public Color32 ActiveLeatherTertiaryColor { get; private set; }
            [HideInInspector] public Color32 ActiveMetalDarkColor { get; private set; }
            [HideInInspector] public Color32 ActiveMetalPrimaryColor { get; private set; }
            [HideInInspector] public Color32 ActiveMetalSecondaryColor { get; private set; }
            [HideInInspector] public Color32 ActivePrimaryColor { get; private set; }
            [HideInInspector] public Color32 ActiveSecondaryColor { get; private set; }
            [HideInInspector] public Color32 ActiveTertiaryColor { get; private set; }
            [HideInInspector] public string ActiveHelmetColor { get; private set; }
            [HideInInspector] public string ActiveTorsoColor { get; private set; }
            [HideInInspector] public string ActiveHeadColor { get; private set; }
            [HideInInspector] public string ActiveUpperArmsColor { get; private set; }
            [HideInInspector] public string ActiveLowerArmsColor { get; private set; }
            [HideInInspector] public string ActiveHipsColor { get; private set; }
            [HideInInspector] public string ActiveLowerLegsColor { get; private set; }
            [HideInInspector] public string ActiveHandsColor { get; private set; }
            [HideInInspector] public string ActiveFeetColor { get; private set; }

            [HideInInspector] public List<GameObject> DefaultHead { get; private set; }
            [HideInInspector] public List<GameObject> DefaultHands { get; private set; }
            [HideInInspector] public List<GameObject> DefaultLowerArms { get; private set; }
            [HideInInspector] public List<GameObject> DefaultUpperArms { get; private set; }
            [HideInInspector] public List<GameObject> DefaultTorso { get; private set; }
            [HideInInspector] public List<GameObject> DefaultHips { get; private set; }
            [HideInInspector] public List<GameObject> DefaultLowerLegs { get; private set; }
            [HideInInspector] public List<GameObject> DefaultFeet { get; private set; }


        }
    }
}