using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if WeaponOffset
using HNGamers.Atavism;
#endif
namespace Atavism
{
    [Serializable]
    public class RaceDisplay
    {
        public string racedata;
        public List<GameObject> GearParts = new List<GameObject>(); // Equipment Parts options.    
        public List<GameObject> FemaleGearParts = new List<GameObject>(); // Equipment Parts options.  
    }
    public enum ModularMaterialSwap
    {             
        internalMat,
        internalHandsMat,
        internalLowerArmsMat,
        internalUpperArmsMat,
        internalTorsoMat,
        internalHipsMat,
        internalHelmetMat,
        internalLowerLegsMat,
        internalFeetMat,
        internalHeadMat,
        internalHairMat,
        internalMouthMat,
        internalEyeMat,
        internalBeardMat,
        internalEyebrowMat
    }

    //Checks for wether its a Weapon or Armor, for other checks in MobAppearence
    public enum GearType
    {
        AttachedObjectArmor,
        AttachedObjectWeapon
    }
    public enum HeadDisables
    {
        None,
        //Head, //Head works as string, rather than GameObject.  On list to fix.
        Hair,
        Eyebrows,
        Beard,
        HairEyebrows,
        HairBeard,
        BeardEyebrows,
        HairEyebrowsBeard
    }
    public enum ShoulderDisables
    {
        None,
        UpperArms,
        UpperAndLowerArms,
        LowerArms
    }

    
    public enum EquipDisplayType
    {
        AttachedObject,
        ActivatedModel,
        BaseTextureSwap,
        ModularArmor,
      //  ModularTextureSwap

    }
    
    [Serializable]
    public class SlotMod
    {
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public Vector3 scale = Vector3.one;
        public Vector3 restPosition = Vector3.zero;
        public Vector3 restRotation = Vector3.zero;
        public Vector3 restScale = Vector3.one;
        [NonSerialized]
        public bool showCopyToSlot = false;
        [NonSerialized]
        public int  rI=-1,cI=-1,gI=-1;
        [NonSerialized]
        public string sN="";
    }

    [Serializable]
    public class RaceDef
    {
        public int raceId=-1;
        public string name;
        public List<GameObject> GearParts = new List<GameObject>(); // Equipment Parts options.    
        public List<string> GearPartsName = new List<string>(); // Equipment Parts options.    
        public List<GameObject> disableGearParts = new List<GameObject>(); // Equipment Parts options.    
        public List<string> disableGearPartsName = new List<string>(); // Equipment Parts options.    
        public List<int> aspectIds = new List<int>();
        public List<ClassDef> aspectDef = new List<ClassDef>();
        [NonSerialized] public bool showCopyTo = false;
        [NonSerialized] public int rI = -1;
    }
    [Serializable]
    public class ClassDef
    {
        public int aspectId = -1;
        public string name;
        public List<GameObject> GearParts = new List<GameObject>(); // Equipment Parts options.    
        public List<string> GearPartsName = new List<string>(); // Equipment Parts options.    
        public List<GameObject> disableGearParts = new List<GameObject>(); // Equipment Parts options.    
        public List<string> disableGearPartsName = new List<string>(); // Equipment Parts options.    
        public List<int> genderIds = new List<int>();
        public List<GenderDef> genderDef = new List<GenderDef>();
        [NonSerialized] public bool showCopyTo = false;
        [NonSerialized] public int rI = -1, cI = -1;
    }

    [Serializable]
    public class GenderDef
    {
        public int ganderId = -1;
        public string name ;
        public GameObject model;
        public List<GameObject> GearParts = new List<GameObject>(); // Equipment Parts options.    
        public List<string> GearPartsName = new List<string>(); // Equipment Parts options.    
        public List<GameObject> disableGearParts = new List<GameObject>(); // Equipment Parts options.    
        public List<string> disableGearPartsName = new List<string>(); // Equipment Parts options.    
        public List<string> slot = new List<string>();
        public List<SlotMod> slotDef = new List<SlotMod>();
        [NonSerialized] public bool showCopyTo = false;
        [NonSerialized] public int rI = -1, cI = -1, gI = -1;
        public SlotMod getSlotDef(string slotName)
        {
            int sindex = slot.IndexOf(slotName); 
            if (sindex > -1)
            {
                return slotDef[sindex];
            }
          //  Debug.LogError("getSlotDef: not found ");

            return new SlotMod();
        }
    }

    public class EquipmentDisplay : MonoBehaviour
    {

        int id;
        public EquipDisplayType equipDisplayType;
        public string modelName;
        public GameObject model;
        public Material material;
       // public AttachmentSocket socket = AttachmentSocket.None;
       // public AttachmentSocket restSocket = AttachmentSocket.None;
       public List<int> raceIds = new List<int>();
       public List<RaceDef> raceDef = new List<RaceDef>();
       
       public string overrideSlot = "";

       public bool useRestSlot = false;
       public bool defaultRestSlot = false;

       [Obsolete] public Vector3 restPosition = Vector3.zero;
       [Obsolete] public Vector3 restRotation = Vector3.zero;
      
      public GearType gearType;
#if WeaponOffset
        public WeaponOffset weaponOffset; // Provide socketed or individual item offsets per character model 
#endif
#if WeaponIKOffset
        public WeaponIKOffset weaponIKOffset; // Provide socketed or individual item offsets per character model   
#endif

#if ModularNHance
        public bool enableEquipment;
#endif
      //  [Header("Attachment Armor Parts (Model Prefabs)")]
      [Obsolete] public GameObject maleModel; //Male Version of the item for Attachments
      [Obsolete] public GameObject femaleModel; //Female Version of the item for Attachments
        [Header("Head Disables (Hair, Eyebrows, Beards)")]
        public HeadDisables headDisables;
        [Header("Shoulder Disables (Upper and/or Lower Arms)")]
        public ShoulderDisables shoulderDisables;

       // [Header("Modular Armor Parts (Model Prefabs)")]
       [Obsolete]  public List<GameObject> GearParts = new List<GameObject>(); // Equipment Parts options.    
       [Obsolete] public List<GameObject> FemaleGearParts = new List<GameObject>(); // Equipment Parts options.  
       [Obsolete]  public List<RaceDisplay> RaceDisplayItem = new List<RaceDisplay>(); // Equipment Parts options.  
        public Shader ShaderToReplace;
        public Shader ReplacementShader;
        public Material MaterialToReplace;
        public Material ReplacementMaterial;

        public RaceDef GetRaceDisplayDef(int race)
        {
            int rindex = raceIds.IndexOf(race);
            if (rindex > -1)
            {
                return raceDef[rindex];
            }
            //Get any race Definition
            rindex = raceIds.IndexOf(0);
            if (rindex > -1)
            {
                return raceDef[rindex];
            }
            
            return null;
        }
        public ClassDef GetClassDisplayDef(int race, int aspect)
        {
            var raceDef = GetRaceDisplayDef(race);
            if (raceDef != null)
            {
                int aindex = raceDef.aspectIds.IndexOf(aspect);
                if (aindex > -1)
                {
                    return raceDef.aspectDef[aindex];
                }
                //Get any Class Definition
                aindex = raceDef.aspectIds.IndexOf(0);
                if (aindex > -1)
                {
                    return raceDef.aspectDef[aindex];
                }
            }
            return null;
        }

        public GenderDef GetGanderDisplayDef(int race, int aspect, int gender)
        {
            var aspectDef = GetClassDisplayDef(race, aspect);
            if (aspectDef != null)
            {
                int gindex = aspectDef.genderIds.IndexOf(gender);
                if (gindex > -1)
                {
                    return aspectDef.genderDef[gindex];
                }
                //Get any Gender Definition
                gindex = aspectDef.genderIds.IndexOf(0);
                if (gindex > -1)
                {
                    return aspectDef.genderDef[gindex];
                }
            }

            return null;
        }

        public List<GenderDef> GetGanderFromAllRaceClassDef()
        {
            List<GenderDef> list = new List<GenderDef>();
            foreach (var r in raceDef)
            {
                foreach (var a in r.aspectDef)
                {
                    foreach (var g in a.genderDef)
                    {
                        if(g!=null)
                            list.Add(g);
                    }
                }
            }

            return list;
        }
        public GenderDef GetGanderOrCreateDef(int race, int aspect, int gender)
        {
            int rindex = raceIds.IndexOf(race);
            if (rindex > -1)
            {
                int aindex = raceDef[rindex].aspectIds.IndexOf(aspect);
                if (aindex > -1)
                {
                    int gindex = raceDef[rindex].aspectDef[aindex].genderIds.IndexOf(gender);
                    if (gindex > -1)
                    {
                        return raceDef[rindex].aspectDef[aindex].genderDef[gindex];
                    }
                    else
                    {
                        if (gender > 0)
                        {
                            raceDef[rindex].aspectDef[aindex].genderIds.Add(gender);
                            raceDef[rindex].aspectDef[aindex].genderDef.Add(new GenderDef());
                            gindex = raceDef[rindex].aspectDef[aindex].genderIds.IndexOf(gender);
                            raceDef[rindex].aspectDef[aindex].genderDef[gindex].ganderId = gender;
                            return raceDef[rindex].aspectDef[aindex].genderDef[gindex];
                        }
                    }
                }
                else
                {
                    if (aspect > 0 && gender > 0)
                    {
                        raceDef[rindex].aspectIds.Add(aspect);
                        raceDef[rindex].aspectDef.Add(new ClassDef());
                        aindex = raceDef[rindex].aspectIds.IndexOf(aspect);
                        raceDef[rindex].aspectDef[aindex].aspectId = aspect;
                        
                        raceDef[rindex].aspectDef[aindex].genderIds.Add(gender);
                        raceDef[rindex].aspectDef[aindex].genderDef.Add(new GenderDef());
                        int gindex = raceDef[rindex].aspectDef[aindex].genderIds.IndexOf(gender);
                        raceDef[rindex].aspectDef[aindex].genderDef[gindex].ganderId = gender;
                        return raceDef[rindex].aspectDef[aindex].genderDef[gindex];
                    }
                }
            }
            else
            {
                if (race > 0 && aspect > 0 && gender > 0)
                {
                    raceIds.Add(race);
                    raceDef.Add(new RaceDef());
                    rindex = raceIds.IndexOf(race);
                    raceDef[rindex].raceId = race;
                    
                    raceDef[rindex].aspectIds.Add(aspect);
                    raceDef[rindex].aspectDef.Add(new ClassDef());
                    int aindex = raceDef[rindex].aspectIds.IndexOf(aspect);
                    raceDef[rindex].aspectDef[aindex].aspectId = aspect;

                    raceDef[rindex].aspectDef[aindex].genderIds.Add(gender);
                    raceDef[rindex].aspectDef[aindex].genderDef.Add(new GenderDef());
                    int gindex = raceDef[rindex].aspectDef[aindex].genderIds.IndexOf(gender);
                    raceDef[rindex].aspectDef[aindex].genderDef[gindex].ganderId = gender;
                    return raceDef[rindex].aspectDef[aindex].genderDef[gindex];
                }
            }
            
            return null;
        }

        public bool isOverrideSlot()
        {
            if (overrideSlot.Equals("") || overrideSlot.Equals("None"))
                return false;
            return true;
        }
    }
}