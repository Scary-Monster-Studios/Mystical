using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System;
using HNGamers.Atavism;
using UnityEngine.Analytics;
using UnityEngine.XR;
using static HNGamers.Atavism.ModularCustomizationManager;
#if WeaponTrails
using XftWeapon;
#endif

namespace Atavism
{
    
    public enum AttachmentSocket
    {
        Root,
        LeftFoot,
        RightFoot,
        Pelvis,
        LeftHip,
        RightHip,
        LeftKnee,
        RightKnee,
        LeftWrist,
        RightWrist,
        MainHand,
        MainHand2,
        OffHand,
        OffHand2,
        MainHandRest,
        MainHandRest2,
        OffHandRest,
        OffHandRest2,
        Shield,
        Shield2,
        ShieldRest,
        ShieldRest2,
        Chest,
        Back,
        LeftShoulder,
        RightShoulder,
        Head,
        HeadCovering,
        Neck,
        Mouth,
        LeftEye,
        RightEye,
        Overhead,
        MainWeapon,
        SecondaryWeapon,
        LeftElbow,
        RightElbow,
        None
    }
    
    
   /* public enum AttachmentSocket
    {
        Root,
        LeftFoot,
        RightFoot,
        Pelvis,
        LeftHip,
        RightHip,
        MainHand,
        MainHand2,
        OffHand,
        OffHand2,
        MainHandRest,
        MainHandRest2,
        OffHandRest,
        OffHandRest2,
        Shield,
        Shield2,
        ShieldRest,
        ShieldRest2,
        Chest,
        Back,
        LeftShoulder,
        RightShoulder,
        Head,
        Neck,
        Mouth,
        LeftEye,
        RightEye,
        Overhead,
        MainWeapon,
        SecondaryWeapon,
        None
    }*/
   
   [RequireComponent(typeof(Animator))]
    public class ActiveEquipmentDisplay
    {
        public EquipmentDisplay equipDisplay;
        public GameObject attachedObject;
        public string slot; 
        public string socket;
        public Material baseMaterial;

        public ActiveEquipmentDisplay()
        {
        }

        public ActiveEquipmentDisplay(EquipmentDisplay equipDisplay, GameObject attachedObject, string  socket, string slot)
        {
            this.equipDisplay = equipDisplay;
            this.attachedObject = attachedObject;
            this.socket = socket;
            this.slot = slot;
        }

        public ActiveEquipmentDisplay(EquipmentDisplay equipDisplay, GameObject attachedObject, Material baseMaterial, string slot)
        {
            this.equipDisplay = equipDisplay;
            this.attachedObject = attachedObject;
            this.baseMaterial = baseMaterial;
            this.slot = slot;
        }
    }

    public class AtavismMobAppearance : MonoBehaviour
    {

        // Icon
        public Sprite portraitIcon;
        public string mobprefabName; 

        // Sockets for attaching weapons (and particles)
        [Obsolete] public Transform mainHand;
        [Obsolete] public Transform mainHand2;
        [Obsolete] public Transform offHand;
        [Obsolete] public Transform offHand2;
        [Obsolete] public Transform mainHandRest;
        [Obsolete] public Transform mainHandRest2;
        [Obsolete] public Transform offHandRest;
        [Obsolete] public Transform offHandRest2;
        [Obsolete] public Transform shield;
        [Obsolete] public Transform shield2;
        [Obsolete] public Transform shieldRest;
        [Obsolete] public Transform shieldRest2;
        [Obsolete] public Transform head;
        [Obsolete] public Transform leftShoulderSocket;
        [Obsolete] public Transform rightShoulderSocket;

        // Sockets for particles
        [Obsolete] public Transform rootSocket;
        [Obsolete] public Transform leftFootSocket;
        [Obsolete] public Transform rightFootSocket;
        [Obsolete] public Transform pelvisSocket;
        [Obsolete] public Transform leftHipSocket;
        [Obsolete] public Transform rightHipSocket;
        [Obsolete] public Transform chestSocket;
        [Obsolete] public Transform backSocket;
        [Obsolete] public Transform neckSocket;
        [Obsolete] public Transform mouthSocket;
        [Obsolete] public Transform leftEyeSocket;
        [Obsolete] public Transform rightEyeSocket;
        [Obsolete] public Transform overheadSocket;

        public List<string> slots = new List<string>();

        public List<Transform> sockets = new List<Transform>();
        public List<Transform> restsockets = new List<Transform>();
         protected Dictionary<string, List<ActiveEquipmentDisplay>> activeEquipDisplays = new Dictionary<string, List<ActiveEquipmentDisplay>>();
        protected bool inCombat = false;
        //  string weaponType = "";
        public float combatCloseDelay = 0.5f;
        float toRestTime = 0;
        bool toRest = false;
        float hideTime = 0;
        bool hidedWeapon = false;
        bool dead = false;
        public  int gender = -1;
        public int race = -1;
        public int aspect = -1;
        public string displayVal = "";
        public bool unsheatheAllWeaponsNotDefaultRestOnEnterInCombat = true;
        public AtavismNode node;
        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        protected void Update()
        {
            if (toRestTime < Time.time && toRest && toRestTime != 0)
            {
                toRest = false;
                if (inCombat == false)
                    SetWeaponsAttachmentSlot();
            }
            if (hideTime < Time.time && hideTime != 0 && hidedWeapon)
            {
                hidedWeapon = false;
                ShowWeapon();
            }
        }

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

        
        public Transform GetRestSocketTransform(string slot)
        {
            int slotId = slots.IndexOf(slot);
            if (slotId >= 0)
            {
                if (restsockets[slotId] != null)
                    return restsockets[slotId];
                
            }

            return transform;
        }
        public void SetRestSocketTransform(string slot, Transform trans)
        {
            if (slot.Length == 0)
            {
                Debug.LogError("Slot name cant be empty");
                return;
            }

            int slotId = slots.IndexOf(slot);
            if (slotId >= 0)
            {
                restsockets[slotId] = trans;
            }
            else
            {
                slots.Add(slot);
                restsockets.Add(transform);
                sockets.Add(null);
            }

        }
        public Transform GetRestSocketTransformOrNull(string slot)
        {
            int slotId = slots.IndexOf(slot);
            //  Debug.LogError("GetSocketTransform: "+slotId+" "+slots.Contains(slot));
            if (slotId >=0)
            {
                if (restsockets[slotId] != null)
                    return restsockets[slotId];
            }

            return null;
        }
        public Transform GetSocketTransform(string slot)
        {
            int slotId = slots.IndexOf(slot);
          //  Debug.LogError("GetSocketTransform: "+slotId+" "+slots.Contains(slot));
            if (slotId >=0)
            {
                if (sockets[slotId] != null)
                    return sockets[slotId];
            }

            return transform;
        }
        public Transform GetSocketTransformOrNull(string slot)
        {
            int slotId = slots.IndexOf(slot);
            //  Debug.LogError("GetSocketTransform: "+slotId+" "+slots.Contains(slot));
            if (slotId >=0)
            {
                if (sockets[slotId] != null)
                    return sockets[slotId];
            }

            return null;
        }
        public void SetSocketTransform(string slot, Transform trans)
        {
            if (slot.Length == 0)
            {
                Debug.LogError("Slot name cant be empty");
                return;
            }

            int slotId = slots.IndexOf(slot);
            if (slotId >= 0)
            {
                sockets[slotId] = trans;
            }
            else
            {
                slots.Add(slot);
                sockets.Add(transform);
                restsockets.Add(null);
            }

        }
        protected virtual void OnDestroy()
        {
            if (GetComponent<AtavismNode>())
            {
                foreach (string slot in Inventory.Instance.itemSlots.Keys)
                {
                    GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler(slot+"DisplayID", EquipPropertyHandler);
                }
                GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler("combatstate", HandleCombatState);
                GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler("model", ModelHandler);
                GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler("weaponType", HandleWeaponType);
                GetComponent<AtavismNode>().RemoveObjectPropertyChangeHandler("deadstate", HandleDeadState);
            }
        }

        protected void ObjectNodeReady()
        {
            GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler("model", ModelHandler);

            foreach (string slot in Inventory.Instance.itemSlots.Keys)
            {
                GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler(slot+"DisplayID", EquipPropertyHandler);
                if (GetComponent<AtavismNode>().PropertyExists(slot+"DisplayID"))
                {
                    string displayID = (string)GetComponent<AtavismNode>().GetProperty(slot+"DisplayID");
                    UpdateEquipDisplay(slot+"DisplayID", displayID);
                }
            }

            GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler("combatstate", HandleCombatState);
            if (GetComponent<AtavismNode>().PropertyExists("combatstate"))
            {
                inCombat = (bool)GetComponent<AtavismNode>().GetProperty("combatstate");
                SetWeaponsAttachmentSlot();
            }
            AtavismLogger.LogInfoMessage("Registered display properties for: " + name);


            GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler("weaponType", HandleWeaponType);
            if (GetComponent<AtavismNode>().PropertyExists("weaponType"))
            {
                //   weaponType = (string)GetComponent<AtavismNode>().GetProperty("weaponType");
            }
            GetComponent<AtavismNode>().RegisterObjectPropertyChangeHandler("deadstate", HandleDeadState);
            if (GetComponent<AtavismNode>().PropertyExists("deadstate"))
            {
                dead = (bool)GetComponent<AtavismNode>().GetProperty("deadstate");
            }
        }

        public void HandleWeaponType(object sender, PropertyChangeEventArgs args)
        {
            //   weaponType = (string)GetComponent<AtavismNode>().GetProperty("weaponType");
        }

        public void ModelHandler(object sender, PropertyChangeEventArgs args)
        {
            AtavismLogger.LogDebugMessage("Got model");
            UpdateModel((string)GetComponent<AtavismNode>().GetProperty(args.PropertyName));
            ReApplyEquipDisplay();
        }

       
        
        public void UpdateModel(string prefabName)
        {
            if (prefabName.Contains(".prefab"))
            {
                int resourcePathPos = prefabName.IndexOf("Resources/");
                prefabName = prefabName.Substring(resourcePathPos + 10);
                prefabName = prefabName.Remove(prefabName.Length - 7);
            }

            GameObject prefab = (GameObject)Resources.Load(prefabName);
            GameObject newCharacter = (GameObject)UnityEngine.Object.Instantiate(prefab, transform.position, transform.rotation);
            newCharacter.name = name;
            var node = GetComponent<AtavismNode>();

            node.ReplaceGameObject(newCharacter);

            // ******************************************
           
            ModularCustomizationManager m_ModularCustomizationManager = null;
            m_ModularCustomizationManager = newCharacter.GetComponent<ModularCustomizationManager>();

            if (m_ModularCustomizationManager)
            {

                var oidItem = node.Oid;


                if (node.PropertyExists(m_ModularCustomizationManager.EyeMaterialPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.EyeMaterialPropertyName);
                    m_ModularCustomizationManager.UpdateEyeMaterial((int)prefabitem);
                }
                if (node.PropertyExists(m_ModularCustomizationManager.HairMaterialPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.HairMaterialPropertyName);
                    m_ModularCustomizationManager.UpdateHairMaterial((int)prefabitem);
                }
                if (node.PropertyExists(m_ModularCustomizationManager.SkinMaterialPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.SkinMaterialPropertyName);
                    m_ModularCustomizationManager.UpdateSkinMaterial((int)prefabitem);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.MouthMaterialPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.MouthMaterialPropertyName);
                    m_ModularCustomizationManager.UpdateMouthMaterial((int)prefabitem);
                }


                if (node.PropertyExists(m_ModularCustomizationManager.bodyColorPropertyName))
                {
                    var coloritem = node.GetProperty(m_ModularCustomizationManager.bodyColorPropertyName).ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                    m_ModularCustomizationManager.UpdateBodyColor(color32);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.scarColorPropertyName))
                {
                    var coloritem = node.GetProperty(m_ModularCustomizationManager.scarColorPropertyName).ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                    m_ModularCustomizationManager.UpdateBodyScarColor(color32);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.mouthColorPropertyName))
                {
                    var coloritem = node.GetProperty(m_ModularCustomizationManager.mouthColorPropertyName).ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                    m_ModularCustomizationManager.UpdateMouthColor(color32);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.hairColorPropertyName))
                {
                    var coloritem = node.GetProperty(m_ModularCustomizationManager.hairColorPropertyName).ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                    m_ModularCustomizationManager.UpdateHairColor(color32);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.beardColorPropertyName))
                {
                    var coloritem = node.GetProperty(m_ModularCustomizationManager.beardColorPropertyName).ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                    m_ModularCustomizationManager.UpdateBeardColor(color32);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.eyeBrowColorPropertyName))
                {
                    var coloritem = node.GetProperty(m_ModularCustomizationManager.eyeBrowColorPropertyName).ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                    m_ModularCustomizationManager.UpdateEyebrowColor(color32);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.stubbleColorPropertyName))
                {
                    var coloritem = node.GetProperty(m_ModularCustomizationManager.stubbleColorPropertyName).ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                    m_ModularCustomizationManager.UpdateStubbleColor(color32);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.bodyArtColorPropertyName))
                {
                    var coloritem = node.GetProperty(m_ModularCustomizationManager.bodyArtColorPropertyName).ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                    m_ModularCustomizationManager.UpdateBodyArtColor(color32);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.eyeColorPropertyName))
                {
                    var coloritem = node.GetProperty(m_ModularCustomizationManager.eyeColorPropertyName).ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                    m_ModularCustomizationManager.UpdateEyeColor(color32);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.helmetColorPropertyName))
                {
                    var colorProperties = node.GetProperty(m_ModularCustomizationManager.helmetColorPropertyName).ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        m_ModularCustomizationManager.UpdateShaderColor(color32, colorslot, BodyType.Helmet);
                    }
                }


                if (node.PropertyExists(m_ModularCustomizationManager.torsoColorPropertyName))
                {
                    var colorProperties = node.GetProperty(m_ModularCustomizationManager.torsoColorPropertyName).ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        m_ModularCustomizationManager.UpdateShaderColor(color32, colorslot, BodyType.Torso);
                    }
                }

                if (node.PropertyExists(m_ModularCustomizationManager.upperArmsColorPropertyName))
                {
                    var colorProperties = node.GetProperty(m_ModularCustomizationManager.upperArmsColorPropertyName).ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        m_ModularCustomizationManager.UpdateShaderColor(color32, colorslot, BodyType.Upperarms);
                    }
                }

                if (node.PropertyExists(m_ModularCustomizationManager.lowerArmsColorPropertyName))
                {
                    var colorProperties = node.GetProperty(m_ModularCustomizationManager.lowerArmsColorPropertyName).ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        m_ModularCustomizationManager.UpdateShaderColor(color32, colorslot, BodyType.LowerArms);
                    }
                }

                if (node.PropertyExists(m_ModularCustomizationManager.hipsColorPropertyName))
                {
                    var colorProperties = node.GetProperty(m_ModularCustomizationManager.hipsColorPropertyName).ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        m_ModularCustomizationManager.UpdateShaderColor(color32, colorslot, BodyType.Hips);
                    }
                }

                if (node.PropertyExists(m_ModularCustomizationManager.lowerLegsColorPropertyName))
                {
                    var colorProperties = node.GetProperty(m_ModularCustomizationManager.lowerLegsColorPropertyName).ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        m_ModularCustomizationManager.UpdateShaderColor(color32, colorslot, BodyType.LowerLegs);
                    }
                }


                if (node.PropertyExists(m_ModularCustomizationManager.feetColorPropertyName))
                {
                    var colorProperties = node.GetProperty(m_ModularCustomizationManager.feetColorPropertyName).ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        m_ModularCustomizationManager.UpdateShaderColor(color32, colorslot, BodyType.Feet);
                    }
                }

                if (node.PropertyExists(m_ModularCustomizationManager.handsColorPropertyName))
                {
                    var colorProperties = node.GetProperty(m_ModularCustomizationManager.handsColorPropertyName).ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        m_ModularCustomizationManager.UpdateShaderColor(color32, colorslot, BodyType.Hands);
                    }
                }

                if (node.PropertyExists(m_ModularCustomizationManager.headPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.headPropertyName);
                    m_ModularCustomizationManager.UpdateBodyModel((string)prefabitem, BodyType.Head);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.faceTexPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.faceTexPropertyName);
                    m_ModularCustomizationManager.UpdateBodyModel((string)prefabitem, BodyType.Face);
                }


                if (node.PropertyExists(m_ModularCustomizationManager.handsPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.handsPropertyName);
                    m_ModularCustomizationManager.UpdateBodyModel((string)prefabitem, BodyType.Hands);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.lowerArmsPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.lowerArmsPropertyName);
                    m_ModularCustomizationManager.UpdateBodyModel((string)prefabitem, BodyType.LowerArms);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.upperArmsPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.upperArmsPropertyName);
                    m_ModularCustomizationManager.UpdateBodyModel((string)prefabitem, BodyType.Upperarms);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.torsoPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.torsoPropertyName);
                    m_ModularCustomizationManager.UpdateBodyModel((string)prefabitem, BodyType.Torso);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.hipsPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.hipsPropertyName);
                    m_ModularCustomizationManager.UpdateBodyModel((string)prefabitem, BodyType.Hips);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.lowerLegsPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.lowerLegsPropertyName);
                    m_ModularCustomizationManager.UpdateBodyModel((string)prefabitem, BodyType.LowerLegs);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.feetPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.feetPropertyName);
                    m_ModularCustomizationManager.UpdateBodyModel((string)prefabitem, BodyType.Feet);
                }
                /*
                if (node.PropertyExists(m_ModularCustomizationManager.hairPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.hairPropertyName);
                    m_ModularCustomizationManager.UpdateHairModel((int)prefabitem);
                }



                if (node.PropertyExists(m_ModularCustomizationManager.beardPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.beardPropertyName);
                    m_ModularCustomizationManager.UpdateBeardModel((int)prefabitem);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.eyebrowPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.eyebrowPropertyName);
                    m_ModularCustomizationManager.UpdateEyebrowModel((int)prefabitem);
                }*/

                if (node.PropertyExists(m_ModularCustomizationManager.mouthPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.mouthPropertyName);
                    m_ModularCustomizationManager.UpdateMouthModel((int)prefabitem);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.eyesPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.eyesPropertyName);
                    m_ModularCustomizationManager.UpdateEyeModel((int)prefabitem);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.tuskPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.tuskPropertyName);
                    m_ModularCustomizationManager.UpdateTuskModel((int)prefabitem);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.earsPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.earsPropertyName);
                    m_ModularCustomizationManager.UpdateEarModel((int)prefabitem);
                }
#if IPBRInt
                if (node.PropertyExists(m_ModularCustomizationManager.blendshapePresetValue)
                    && (!m_ModularCustomizationManager.enableSavingInfinityPBRBlendshapes))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.blendshapePresetValue);
                    m_ModularCustomizationManager.UpdateBlendShapePresets((int)prefabitem);
                }

                if (node.PropertyExists(m_ModularCustomizationManager.infinityBlendShapes))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.infinityBlendShapes);
                    m_ModularCustomizationManager.UpdateBlendShapes((string)prefabitem);
                }
#endif
                if (node.PropertyExists(m_ModularCustomizationManager.faithPropertyName))
                {
                    var prefabitem = node.GetProperty(m_ModularCustomizationManager.faithPropertyName);
                    m_ModularCustomizationManager.SetFaith((string)prefabitem);
                }

            }

            // ******************************************
            
            
            // Check if the player should be hidden as they are in spirit world
            if (!node.Oid.Equals(ClientAPI.GetPlayerOid()))
            if (node.PropertyExists("state"))
            {
                string state = (string)node.GetProperty("state");
                if (state == "spirit")
                    newCharacter.SetActive(false);
            }
        }

        public void HandleCombatState(object sender, PropertyChangeEventArgs args)
        {
            inCombat = (bool)GetComponent<AtavismNode>().GetProperty(args.PropertyName);
            // Reset weapon attached weapons based on combat state
            if (!inCombat)
            {
                toRestTime = Time.time + combatCloseDelay;
                toRest = true;
                return;
            }
            if(unsheatheAllWeaponsNotDefaultRestOnEnterInCombat)
                SetWeaponsAttachmentSlot();
        }

        public void SetWeaponsAttachmentSlot()
        {
            SetWeaponsAttachmentSlot(false);
        }

        public void SetWeaponsAttachmentSlot(bool force)
        {
            foreach (List<ActiveEquipmentDisplay> activeDisplays in activeEquipDisplays.Values)
            {
                foreach (ActiveEquipmentDisplay activeDisplay in activeDisplays)
                {
                    if (activeDisplay.equipDisplay.equipDisplayType != EquipDisplayType.AttachedObject)
                        continue;
                    GameObject weapon = activeDisplay.attachedObject;
                    if (weapon == null)
                        continue;
#if WeaponOffset
                    WeaponOffset offsetToApply = null;

                    WeaponOffset slotBaseOffset = Resources.Load<WeaponOffset>("Offsets/" + activeDisplay.socket.ToString() + "_" + "Base" + "_" + mobprefabName);
                    WeaponOffset modelOffset = null;
                    if (activeDisplay.equipDisplay.weaponOffset)
                    {
                         modelOffset = activeDisplay.equipDisplay.weaponOffset;
                    }

                    if (!modelOffset)
                    {
                        modelOffset = Resources.Load<WeaponOffset>("Offsets/" + activeDisplay.socket.ToString() + "_" + activeDisplay.equipDisplay.model.name + "_" + mobprefabName);
                    }

                    if (slotBaseOffset && !modelOffset)
                    {
                        offsetToApply = slotBaseOffset;
                    }
                    else if (modelOffset)
                    {
                        offsetToApply = modelOffset;
                    }

#endif
#if WeaponIKOffset
                    WeaponIKOffset weaponIKOffsettoApply = null;
                    WeaponIKOffset modelIKOffset = null;
                    if (activeDisplay.equipDisplay.weaponIKOffset)
                    {
                        modelIKOffset = activeDisplay.equipDisplay.weaponIKOffset;
                    }

                    if (!modelIKOffset)
                    {
                        modelIKOffset = Resources.Load<WeaponIKOffset>("IKOffsets/" + activeDisplay.socket.ToString() + "_" + activeDisplay.equipDisplay.model.name + "_" + mobprefabName);
                    }
                    if (modelIKOffset)
                    {
                        weaponIKOffsettoApply = modelIKOffset;
                    }

#endif

                    //TODO: Handle items that can be both primary and off hand
                    if ((inCombat && !activeDisplay.equipDisplay.defaultRestSlot )|| !activeDisplay.equipDisplay.useRestSlot)
                    {
                        if (activeDisplay.socket == activeDisplay.slot && !force)
                        {
                            toRestTime = 0;
                            toRest = false;
                            continue;
                        }
                        // Set the weapons socket to the main socket for the display
                        activeDisplay.socket = activeDisplay.slot;
                       
                        weapon.transform.parent = GetSocketTransform(activeDisplay.socket);
                        AtavismNode node = GetComponent<AtavismNode>();
                        if (node)
                        {
                            gender = node.PropertyExists("genderId") ? (int) node.GetProperty("genderId") : -1;
                            race = node.PropertyExists("race") ? (int) node.GetProperty("race") : -1;
                            aspect = node.PropertyExists("aspect") ? (int) node.GetProperty("aspect") : -1;
                        }

                        var genderdef = activeDisplay.equipDisplay.GetGanderDisplayDef(race, aspect, gender);
                        if (genderdef != null)
                        {
                            var slotDef = genderdef.getSlotDef(activeDisplay.slot);
                            if (slotDef != null)
                            {
                                weapon.transform.localPosition = slotDef.position;
                                weapon.transform.localScale = slotDef.scale;
                                weapon.transform.localRotation = Quaternion.Euler(slotDef.rotation);
                            }
                        }
                      
                        #if WeaponOffset
                        if (offsetToApply)
                        {
                            weapon.transform.localPosition = offsetToApply.ModelPos;
                            weapon.transform.localRotation = Quaternion.Euler(offsetToApply.ModelRot);
                            weapon.transform.localScale = offsetToApply.ModelScale;
                        }
#endif
#if WeaponIKOffset
                        if (weaponIKOffsettoApply)
                        {
                            activeDisplay.equipDisplay.weaponIKOffset = weaponIKOffsettoApply;

                            GameObject leftTarget = new GameObject("LeftHandedTarget");
                            GameObject righttarget = new GameObject("RightHandedTarget");
                            AtavismWeaponIKTarget righthandIKTarget = null;
                            AtavismWeaponIKTarget lefthandIKTarget = null;

                            if (weaponIKOffsettoApply.enableLeftHand)
                            {

                                lefthandIKTarget = leftTarget.AddComponent<AtavismWeaponIKTarget>();
                                lefthandIKTarget.atavismWeaponIKTargetType = AtavismWeaponIKTarget.AtavismWeaponIKTargetType.LeftHand;
                                leftTarget.transform.SetParent(weapon.transform);
                                leftTarget.transform.position = weapon.transform.position;
                            }

                            if (weaponIKOffsettoApply.enableRightHand)
                            {
                                righthandIKTarget = righttarget.AddComponent<AtavismWeaponIKTarget>();
                                righthandIKTarget.atavismWeaponIKTargetType = AtavismWeaponIKTarget.AtavismWeaponIKTargetType.RightHand;

                                righttarget.transform.SetParent(weapon.transform);
                                righttarget.transform.position = weapon.transform.position;
                            }


                            if (weaponIKOffsettoApply.lookAt || !weaponIKOffsettoApply.rightEnabledlookAt || !weaponIKOffsettoApply.leftEnabledlookAt)
                            {
                                Transform atavismWeaponLookAtTarget = null;


                                if (weaponIKOffsettoApply.lookAtTarget)
                                {
                                    atavismWeaponLookAtTarget = weaponIKOffsettoApply.lookAtTarget;
                                }
                                else
                                {
                                    GameObject lookAtTarget = new GameObject("LookAtTarget");
                                    lookAtTarget.transform.SetParent(weapon.transform);
                                    lookAtTarget.transform.localPosition = new Vector3(0, 0, 0);
                                    lookAtTarget.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                                    lookAtTarget.transform.localRotation = Quaternion.Euler(weaponIKOffsettoApply.lookAtRotations);
                                    lookAtTarget.transform.localPosition = weaponIKOffsettoApply.lookAtPositions;
                                    atavismWeaponLookAtTarget = lookAtTarget.transform;

                                    if (lefthandIKTarget && !righthandIKTarget)
                                    {
                                        lefthandIKTarget.lookAtPoint = lookAtTarget.transform;
                                        lefthandIKTarget.enableLookAt = true;
                                    }
                                    else if (righthandIKTarget)
                                    {
                                        righthandIKTarget.lookAtPoint = lookAtTarget.transform;
                                        righthandIKTarget.enableLookAt = true;
                                    }
                                }
                            }
                        }
#endif

                    }
                    else if (force || activeDisplay.socket == activeDisplay.slot)
                    {
                        // Set the weapons socket to the rest socket for the display
                        activeDisplay.socket = "rest_"+activeDisplay.slot;
                      
                        weapon.transform.parent = GetRestSocketTransform(activeDisplay.slot);
                        weapon.transform.localPosition = Vector3.zero;
                        weapon.transform.localRotation = new Quaternion(0, 0, 0, 0);
                        weapon.transform.localScale = Vector3.one;
                        //correct position an rotation
                        AtavismNode node = GetComponent<AtavismNode>();
                        if (node)
                        {
                            gender = node.PropertyExists("genderId") ? (int) node.GetProperty("genderId") : -1;
                            race = node.PropertyExists("race") ? (int) node.GetProperty("race") : -1;
                            aspect = node.PropertyExists("aspect") ? (int) node.GetProperty("aspect") : -1;
                        }
                        var genderdef = activeDisplay.equipDisplay.GetGanderDisplayDef(race, aspect, gender);
                        if (genderdef != null)
                        {
                            var slotDef = genderdef.getSlotDef(activeDisplay.slot);
                            if (slotDef != null)
                            {
                                weapon.transform.localPosition = slotDef.restPosition;
                                weapon.transform.localScale = slotDef.restScale;
                                weapon.transform.localRotation = Quaternion.Euler(slotDef.restRotation);
                            }
                        }
                        // weapon.transform.Rotate(activeDisplay.equipDisplay.restRotation.x, activeDisplay.equipDisplay.restRotation.y, activeDisplay.equipDisplay.restRotation.z, Space.Self);
#if WeaponTrails
                        //Disable Trails 
                            MeleeWeaponTrail[] mwts = weapon.GetComponentsInChildren<MeleeWeaponTrail>();
                            foreach (MeleeWeaponTrail mwt in mwts)
                            {
                                if (mwt != null)
                                    mwt.gameObject.SetActive(false);
                            }
#endif
                    }
                }
            }
        }
        /// <summary>
        /// Function move equiped weapon to main slot
        /// </summary>
        /// <param name="time"></param>
        public void GetWeapon(float time)
        {
            foreach (List<ActiveEquipmentDisplay> activeDisplays in activeEquipDisplays.Values)
            {
                foreach (ActiveEquipmentDisplay activeDisplay in activeDisplays)
                {
                    toRestTime = Time.time + time;
                    toRest = true;
                    if (activeDisplay.socket == activeDisplay.slot )
                        continue;
                    // Set the weapons socket to the main socket for the display
                    activeDisplay.socket = activeDisplay.slot;
                    GameObject weapon = activeDisplay.attachedObject;
                    weapon.transform.parent = GetSocketTransform(activeDisplay.socket);
                    AtavismNode node = GetComponent<AtavismNode>();
                  
                    int gender = node.PropertyExists("genderId") ? (int)node.GetProperty("genderId"):-1;
                    int race = node.PropertyExists("race") ? (int)node.GetProperty("race"):-1;
                    int aspect = node.PropertyExists("aspect") ? (int)node.GetProperty("aspect"):-1;
                    
                    var genderdef = activeDisplay.equipDisplay.GetGanderDisplayDef(race, aspect, gender);
                    if (genderdef != null)
                    {
                        var slotDef = genderdef.getSlotDef(activeDisplay.slot);
                        if (slotDef != null)
                        {
                            weapon.transform.localPosition = slotDef.position;
                            weapon.transform.localScale = slotDef.scale;
                            weapon.transform.localRotation = Quaternion.Euler(slotDef.rotation);
                        }
                    }
                    /*weapon.transform.localPosition = Vector3.zero;
                    weapon.transform.localRotation = new Quaternion(0, 0, 0, 0);
                    weapon.transform.localScale = Vector3.one;*/
                }
            }
        }

        /// <summary>
        /// Function move equiped weapon to main slot
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="time"></param>
        public void GetWeapon(string slot, float time)
        {
            foreach (List<ActiveEquipmentDisplay> activeDisplays in activeEquipDisplays.Values)
            {
                foreach (ActiveEquipmentDisplay activeDisplay in activeDisplays)
                {
                  //  Debug.LogError("||Slot "+activeDisplay.slot);

                    if(!activeDisplay.slot.Equals(slot))
                        continue;
                  //  Debug.LogError("||Slot | "+activeDisplay.slot);
                    toRestTime = Time.time + time;
                    toRest = true;
                    
                    if (activeDisplay.socket == activeDisplay.slot)
                        continue;
                   // Debug.LogError("||Slot || "+activeDisplay.slot);
                    // Set the weapons socket to the main socket for the display
                    activeDisplay.socket = activeDisplay.slot;
                    GameObject weapon = activeDisplay.attachedObject;
                    weapon.transform.parent = GetSocketTransform(activeDisplay.socket);
                    AtavismNode node = GetComponent<AtavismNode>();
                    if (node)
                    {
                         gender = node.PropertyExists("genderId") ? (int) node.GetProperty("genderId") : -1;
                         race = node.PropertyExists("race") ? (int) node.GetProperty("race") : -1;
                         aspect = node.PropertyExists("aspect") ? (int) node.GetProperty("aspect") : -1;
                    }

                    var genderdef = activeDisplay.equipDisplay.GetGanderDisplayDef(race, aspect, gender);
                    var slotDef = genderdef.getSlotDef(activeDisplay.slot);
                    weapon.transform.localPosition = slotDef.position;
                    weapon.transform.localScale = slotDef.scale;
                    weapon.transform.localRotation = Quaternion.Euler(slotDef.rotation);
                  /*  weapon.transform.localPosition = Vector3.zero;
                    weapon.transform.localRotation = new Quaternion(0, 0, 0, 0);
                    weapon.transform.localScale = Vector3.one;*/
                }
            }
        }
        
        /// <summary>
        /// Function move equiped weapon to main slot
        /// </summary>
        /// <param name="slot"></param>
      
        public void WeaponToRest(string slot, float time)
        {
            foreach (List<ActiveEquipmentDisplay> activeDisplays in activeEquipDisplays.Values)
            {
                foreach (ActiveEquipmentDisplay activeDisplay in activeDisplays)
                {
                   // Debug.LogError("Slot "+activeDisplay.slot);
                    if (!activeDisplay.slot.Equals(slot))
                    {
                        
                        continue;
                    }
                  //  Debug.LogError("Slot | "+activeDisplay.slot);
                    if (activeDisplay.socket == "rest_"+activeDisplay.slot)
                        continue;
                  //  Debug.LogError("Slot || "+activeDisplay.slot);
                    toRestTime = Time.time + time;
                    toRest = true;
                    // Set the weapons socket to the main socket for the display
                    activeDisplay.socket = "rest_"+activeDisplay.slot;
                    GameObject weapon = activeDisplay.attachedObject;
                    weapon.transform.parent = GetRestSocketTransform(activeDisplay.slot);
                    AtavismNode node = GetComponent<AtavismNode>();
                    if (node)
                    {
                        gender = node.PropertyExists("genderId") ? (int) node.GetProperty("genderId") : -1;
                        race = node.PropertyExists("race") ? (int) node.GetProperty("race") : -1;
                        aspect = node.PropertyExists("aspect") ? (int) node.GetProperty("aspect") : -1;
                    }

                    var genderdef = activeDisplay.equipDisplay.GetGanderDisplayDef(race, aspect, gender);
                    var slotDef = genderdef.getSlotDef(activeDisplay.slot);
                    weapon.transform.localPosition = slotDef.restPosition;
                    weapon.transform.localScale = slotDef.restScale;
                    weapon.transform.localRotation = Quaternion.Euler(slotDef.restRotation);
                  /*  weapon.transform.localPosition = Vector3.zero;
                    weapon.transform.localRotation = new Quaternion(0, 0, 0, 0);
                    weapon.transform.localScale = Vector3.one;*/
                }
            }
        }
        
        public void WeaponToRest()
        {
            foreach (List<ActiveEquipmentDisplay> activeDisplays in activeEquipDisplays.Values)
            {
                foreach (ActiveEquipmentDisplay activeDisplay in activeDisplays)
                {
                  //  Debug.LogError("Slot "+activeDisplay.slot);

                    var sl = Inventory.Instance.itemSlots[activeDisplay.slot];
                    
                  //  Debug.LogError("Slot "+activeDisplay.slot+" Type="+string.Join("|",sl.type));
                    if(!sl.type.Contains("Weapon"))
                        continue;
                   // Debug.LogError("Slot | "+activeDisplay.slot);

                    if (activeDisplay.socket == "rest_"+activeDisplay.slot)
                        continue;
                 //   Debug.LogError("Slot || "+activeDisplay.slot);

                    // Set the weapons socket to the main socket for the display
                    activeDisplay.socket = "rest_"+activeDisplay.slot;
                    GameObject weapon = activeDisplay.attachedObject;
                    weapon.transform.parent = GetRestSocketTransform(activeDisplay.slot);
                    AtavismNode node = GetComponent<AtavismNode>();
                    if (node)
                    {
                        gender = node.PropertyExists("genderId") ? (int) node.GetProperty("genderId") : -1;
                        race = node.PropertyExists("race") ? (int) node.GetProperty("race") : -1;
                        aspect = node.PropertyExists("aspect") ? (int) node.GetProperty("aspect") : -1;
                    }

                    var genderdef = activeDisplay.equipDisplay.GetGanderDisplayDef(race, aspect, gender);
                    var slotDef = genderdef.getSlotDef(activeDisplay.slot);
                    weapon.transform.localPosition = slotDef.restPosition;
                    weapon.transform.localScale = slotDef.restScale;
                    weapon.transform.localRotation = Quaternion.Euler(slotDef.restRotation);
                   /* weapon.transform.localPosition = Vector3.zero;
                    weapon.transform.localRotation = new Quaternion(0, 0, 0, 0);
                    weapon.transform.localScale = Vector3.one;*/
                }
            }
        }

        /// <summary>
        /// Function turn off object of equiped weapon and dieable trails
        /// </summary>
        /// <param name="t"></param>
        public void HideWeapon(float t)
        {
            foreach (List<ActiveEquipmentDisplay> activeDisplays in activeEquipDisplays.Values)
            {
                foreach (ActiveEquipmentDisplay activeDisplay in activeDisplays)
                {
                    GameObject weapon = activeDisplay.attachedObject;
                    var sl = Inventory.Instance.itemSlots[activeDisplay.slot];
                    if(!sl.type.Contains("Weapon"))
                        continue;
#if WeaponTrails
                    //Disable trails
                    if (weapon != null){
                          XWeaponTrail trail = weapon.GetComponentInChildren<XWeaponTrail>();
                          if (trail != null) trail.Deactivate();
                    }
#endif
                        if (weapon != null)
                            weapon.SetActive(false);

                        hideTime = Time.time + t;
                        hidedWeapon = true;
                }
            }
        }

        /// <summary>
        /// Function turn off object of equiped weapon and dieable trails
        /// </summary>
        /// <param name="t"></param>
        public void HideWeapon(string slot, float t)
        {
            foreach (List<ActiveEquipmentDisplay> activeDisplays in activeEquipDisplays.Values)
            {
                foreach (ActiveEquipmentDisplay activeDisplay in activeDisplays)
                {
                    var sl = Inventory.Instance.itemSlots[activeDisplay.slot];
                    if(!sl.type.Contains("Weapon"))
                        continue;
                    GameObject weapon = activeDisplay.attachedObject;
#if WeaponTrails
                    //Disable trails
                          XWeaponTrail trail = weapon.GetComponentInChildren<XWeaponTrail>();
                          if (trail != null) trail.Deactivate();
#endif
                    weapon.SetActive(false);
                    hideTime = Time.time + t;
                    hidedWeapon = true;
                }
            }
        }

        /// <summary>
        /// Function return model of equiped weapon
        /// </summary>
        /// <returns></returns>
        public GameObject GetWeaponObjectModel()
        {
            foreach (List<ActiveEquipmentDisplay> activeDisplays in activeEquipDisplays.Values)
            foreach (ActiveEquipmentDisplay activeDisplay in activeDisplays)
            {
                var sl = Inventory.Instance.itemSlots[activeDisplay.slot];
                if(!sl.type.Contains("Weapon"))
                    continue;
                return activeDisplay.equipDisplay.model;
            }

            return null;
        }

        /// <summary>
        /// Function return object of equiped weapon
        /// </summary>
        /// <returns></returns>
        public GameObject GetWeaponObject()
        {
            foreach (List<ActiveEquipmentDisplay> activeDisplays in activeEquipDisplays.Values)
            foreach (ActiveEquipmentDisplay activeDisplay in activeDisplays)
            {
                var sl = Inventory.Instance.itemSlots[activeDisplay.slot];
                if(!sl.type.Contains("Weapon"))
                    continue;
                return activeDisplay.attachedObject;
            }

            return null;
        }

        /// <summary>
        /// Function return name of model equiped weapon 
        /// </summary>
        /// <returns></returns>
        public string GetWeaponName()
        {
            foreach (List<ActiveEquipmentDisplay> activeDisplays in activeEquipDisplays.Values)
            foreach (ActiveEquipmentDisplay activeDisplay in activeDisplays)
            {
                var sl = Inventory.Instance.itemSlots[activeDisplay.slot];
                if (!sl.type.Contains("Weapon"))
                    continue;
                return activeDisplay.equipDisplay.model.name;
            }

            return null;
        }

        /// <summary>
        /// Function turn on trails on equiped weapon
        /// </summary>
        public void ShowTrail()
        {
            foreach (List<ActiveEquipmentDisplay> activeDisplays in activeEquipDisplays.Values)
                foreach (ActiveEquipmentDisplay activeDisplay in activeDisplays)
                {
                      GameObject weapon = activeDisplay.attachedObject;
#if WeaponTrails
                       XWeaponTrail trail = weapon.GetComponentInChildren<XWeaponTrail>(true);
                      if (trail != null) trail.Activate();
#endif
                }
        }

        /// <summary>
        /// Function trun off trails on equiped weapon
        /// </summary>
        public void HideTrail()
        {
            foreach (List<ActiveEquipmentDisplay> activeDisplays in activeEquipDisplays.Values)
                foreach (ActiveEquipmentDisplay activeDisplay in activeDisplays)
                {  
                    GameObject weapon = activeDisplay.attachedObject;
#if WeaponTrails
                        XWeaponTrail trail = weapon.GetComponentInChildren<XWeaponTrail>();
                        if (trail != null) trail.Deactivate();
#endif
                }
        }

        /// <summary>
        /// Function turn on object of equiped weapon
        /// </summary>
        public void ShowWeapon()
        {
            foreach (List<ActiveEquipmentDisplay> activeDisplays in activeEquipDisplays.Values)
            {
                foreach (ActiveEquipmentDisplay activeDisplay in activeDisplays)
                {
                    var sl = Inventory.Instance.itemSlots[activeDisplay.slot];
                    if (!sl.type.Contains("Weapon"))
                        continue;
                    activeDisplay.attachedObject.SetActive(true);
                }
            }
        }

        public void EquipPropertyHandler(object sender, PropertyChangeEventArgs args)
        {
            string displayID = (string)GetComponent<AtavismNode>().GetProperty(args.PropertyName);
            if (AtavismSettings.Instance.CharacterAvatar != null)
            {
                if (GetComponent<AtavismNode>() != null)
                    if (GetComponent<AtavismNode>().Oid == ClientAPI.GetPlayerOid())
                    {
                        AtavismNode node = GetComponent<AtavismNode>();
                        AtavismSettings.Instance.CharacterAvatar.GetComponent<AtavismMobAppearance>().gender = node.PropertyExists("genderId") ? (int)node.GetProperty("genderId") : -1;
                        AtavismSettings.Instance.CharacterAvatar.GetComponent<AtavismMobAppearance>().race = node.PropertyExists("race") ? (int)node.GetProperty("race") : -1;
                        AtavismSettings.Instance.CharacterAvatar.GetComponent<AtavismMobAppearance>().aspect = node.PropertyExists("aspect") ? (int)node.GetProperty("aspect") : -1;
                        string slot = args.PropertyName.Remove(args.PropertyName.IndexOf("DisplayID"));
                        AtavismSettings.Instance.CharacterAvatar.GetComponent<AtavismMobAppearance>().displayVal = (string) node.GetProperty(slot + "DisplayVAL");
                        AtavismSettings.Instance.CharacterAvatar.GetComponent<AtavismMobAppearance>().UpdateEquipDisplay(args.PropertyName, displayID);
                    }
            }


            if (AtavismSettings.Instance.OtherCharacterAvatar != null)
            {
              //  Debug.LogError("EquipPropertyHandler: Other "+GetComponent<AtavismNode>().Oid+" "+UGUIOtherCharacterPanel.Instance.Id);
                if (GetComponent<AtavismNode>() != null && UGUIOtherCharacterPanel.Instance != null)
                {
                   // Debug.LogError("EquipPropertyHandler: Other |");
                   if (GetComponent<AtavismNode>().Oid == UGUIOtherCharacterPanel.Instance.Id)
                   {
                       //   Debug.LogError("EquipPropertyHandler: Other ||");
                       AtavismNode node = GetComponent<AtavismNode>();
                       AtavismSettings.Instance.OtherCharacterAvatar.GetComponent<AtavismMobAppearance>().gender = node.PropertyExists("genderId") ? (int) node.GetProperty("genderId") : -1;
                       AtavismSettings.Instance.OtherCharacterAvatar.GetComponent<AtavismMobAppearance>().race = node.PropertyExists("race") ? (int) node.GetProperty("race") : -1;
                       AtavismSettings.Instance.OtherCharacterAvatar.GetComponent<AtavismMobAppearance>().aspect = node.PropertyExists("aspect") ? (int) node.GetProperty("aspect") : -1;
                       string slot = args.PropertyName.Remove(args.PropertyName.IndexOf("DisplayID"));
                       AtavismSettings.Instance.OtherCharacterAvatar.GetComponent<AtavismMobAppearance>().displayVal = (string) node.GetProperty(slot + "DisplayVAL");
                       AtavismSettings.Instance.OtherCharacterAvatar.GetComponent<AtavismMobAppearance>().UpdateEquipDisplay(args.PropertyName, displayID);
                   }
                }
            }
            
            UpdateEquipDisplay(args.PropertyName, displayID);
        }

        public void UpdateEquipDisplay(string propName, string displayID)
        {
            ModularCustomizationManager _modularCustomizationManager = GetComponent<ModularCustomizationManager>();
            string slot = propName.Remove(propName.IndexOf("DisplayID"));
           // Debug.LogError("UpdateEquipDisplay: propName="+propName+" slot="+slot);
            if (activeEquipDisplays.ContainsKey(propName))
            {
                foreach (ActiveEquipmentDisplay activeDisplay in activeEquipDisplays[propName])
                {
                    if (activeDisplay.equipDisplay.equipDisplayType == EquipDisplayType.AttachedObject)
                    {
                        RemoveAttachedObject(activeDisplay,slot);
                    }
                    else if (activeDisplay.equipDisplay.equipDisplayType == EquipDisplayType.ActivatedModel)
                    {
                        DeactivateModel(activeDisplay);
                    }
                    else if (activeDisplay.equipDisplay.equipDisplayType == EquipDisplayType.ModularArmor)
                    {
                        DeactivateModularArmor(activeDisplay, slot);
                    }
                    else if (activeDisplay.equipDisplay.equipDisplayType == EquipDisplayType.BaseTextureSwap)
                    {
                        ResetBaseTexture(activeDisplay);
                    }
                }
                activeEquipDisplays.Remove(propName);
            }
            if (displayID != null && displayID != "")
            {
                List<EquipmentDisplay> displays = Inventory.Instance.LoadEquipmentDisplay(displayID);
                if (displays == null || displays.Count == 0)
                    return;
                List<ActiveEquipmentDisplay> activeDisplays = new List<ActiveEquipmentDisplay>();
                foreach (EquipmentDisplay display in displays)
                {
                    if (display.equipDisplayType == EquipDisplayType.AttachedObject)
                    {
                      /*  if (display.model == null)
                        {
                            Debug.LogError("AttachObject model is null propName="+ propName+ " displayID=" + displayID);
                        }
                        else
                        {*/
                            activeDisplays.Add(AttachObject(display, slot));
                       // }
                    }
                    else if (display.equipDisplayType == EquipDisplayType.ActivatedModel)
                    {
                        activeDisplays.Add(ActivateModel(display, slot));
                    }
                    // This is for the Modular Drop Down in Equipment Display
                    else if (display.equipDisplayType == EquipDisplayType.ModularArmor)
                    {
                        activeDisplays.Add(ActivateModularArmor(display, slot));
                    }
                    else if (display.equipDisplayType == EquipDisplayType.BaseTextureSwap)
                    {
                        activeDisplays.Add(SwapBaseModelTexture(display, slot));
                    }
                }
                activeEquipDisplays.Add(propName, activeDisplays);
                SetWeaponsAttachmentSlot();
            }
        }

        protected ActiveEquipmentDisplay AttachObject(EquipmentDisplay equipDisplay, string slot)
        {
            AtavismNode node = GetComponent<AtavismNode>();
            if (this.node != null &&  this.node.PropertyExists("race"))
            {
            //    Debug.LogError(equipDisplay+" slot="+slot+" race="+this.node.PropertyExists("race")+" aspect="+this.node.PropertyExists("aspect")+" gender="+this.node.PropertyExists("genderId"), gameObject);

                gender = this.node.PropertyExists("genderId") ? (int) this.node.GetProperty("genderId") : -1;
                race = this.node.PropertyExists("race") ? (int) this.node.GetProperty("race") : -1;
                aspect = this.node.PropertyExists("aspect") ? (int) this.node.GetProperty("aspect") : -1;
            }
            if (node != null &&  node.PropertyExists("race"))
            {
              //  Debug.LogError(equipDisplay+" slot="+slot+" race="+node.PropertyExists("race")+" aspect="+node.PropertyExists("aspect")+" gender="+node.PropertyExists("genderId"), gameObject);

                gender = node.PropertyExists("genderId") ? (int) node.GetProperty("genderId") : -1;
                race = node.PropertyExists("race") ? (int) node.GetProperty("race") : -1;
                aspect = node.PropertyExists("aspect") ? (int) node.GetProperty("aspect") : -1;
            }
           

          //  Debug.LogError(equipDisplay+" slot="+slot+" race="+race+" aspect="+aspect+" gender="+gender, gameObject);
            var genderdef = equipDisplay.GetGanderDisplayDef(race, aspect, gender);

#if WeaponOffset
            WeaponOffset offsetToApply = equipDisplay.weaponOffset;            
            if (!offsetToApply)
            {
                WeaponOffset slotBaseOffset = Resources.Load<WeaponOffset>("Offsets/" + slot + "_" + "Base" + "_" + mobprefabName);
                WeaponOffset modelOffset = null;

                if (!modelOffset && equipDisplay.model)
                {
                    modelOffset = Resources.Load<WeaponOffset>("Offsets/" + slot + "_" + equipDisplay.model.name + "_" + mobprefabName);
                }

                if (slotBaseOffset && !modelOffset)
                {
                    offsetToApply = slotBaseOffset;
                }
                else if (modelOffset)
                {
                    offsetToApply = modelOffset;
                }
            }
#endif
            if (equipDisplay.gearType == GearType.AttachedObjectArmor)
            {
                ModularCustomizationManager _modularCustomizationManager = GetComponent<ModularCustomizationManager>();
                if (_modularCustomizationManager)
                {
                    handleHairDisables(equipDisplay, _modularCustomizationManager, equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot);
                    handleShoulderDisables(equipDisplay, _modularCustomizationManager,equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot);
                }

                GameObject armor = null;
                if (genderdef != null&& genderdef.model!=null)
                {
                    armor = (GameObject) Instantiate(genderdef.model, GetSocketTransform(equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot).position, GetSocketTransform(equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot).rotation);
                }

                if (armor == null && equipDisplay.model != null)
                    armor = (GameObject) Instantiate(equipDisplay.model, GetSocketTransform(equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot).position, GetSocketTransform(equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot).rotation);
                if (armor != null)
                {

                    if (equipDisplay.material != null)
                    {
                        if (armor.GetComponent<Renderer>() != null)
                            armor.GetComponent<Renderer>().material = equipDisplay.material;
                    }

                    if (armor.GetComponent<AtavismItemVFX>() != null)
                    {
                        armor.GetComponent<AtavismItemVFX>().DisplayVFX(slot, this.node != null ? this.node : node, displayVal);
                    }

                    armor.SendMessage("SetRootGameObject", gameObject, SendMessageOptions.DontRequireReceiver);

                    armor.transform.parent = GetSocketTransform(equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot);
                    if (genderdef != null)
                    {
                        var slotDef = genderdef.getSlotDef(equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot);
                        if (slotDef != null)
                        {
                            armor.transform.localScale = slotDef.scale;
                            armor.transform.localPosition = slotDef.position;
                            armor.transform.localRotation = Quaternion.Euler(slotDef.rotation);
                        }
                        else
                        {
                            Debug.LogError("AttachObject: no slot");
                        }
                    }
                    else
                    {
                        Debug.LogError("AttachObject: no gender");
                    }
#if WeaponOffset
                    if (offsetToApply)
                    {
                        equipDisplay.weaponOffset = offsetToApply;
                        armor.transform.localPosition = offsetToApply.ModelPos;
                        armor.transform.localRotation = Quaternion.Euler(offsetToApply.ModelRot);
                        armor.transform.localScale = offsetToApply.ModelScale;
                    }
#endif
                }

                return new ActiveEquipmentDisplay(equipDisplay, armor, equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot,equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot :  slot);

            }
            else if (equipDisplay.gearType == GearType.AttachedObjectWeapon)
            {
//Debug.LogError("AttachObject: "+slot);
                if (equipDisplay.defaultRestSlot)
                {
                    GameObject weapon = null;
                    if (genderdef != null && genderdef.model!=null)
                    {
                        weapon = (GameObject) Instantiate(genderdef.model, GetRestSocketTransform(equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot).position, GetRestSocketTransform(equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot).rotation);
                    }

                    if (weapon == null && equipDisplay.model != null)
                        weapon = (GameObject) Instantiate(equipDisplay.model, GetRestSocketTransform(equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot).position, GetRestSocketTransform(equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot).rotation);
                    if (weapon != null)
                    {

                        if (equipDisplay.material != null)
                        {
                            if (weapon.GetComponent<Renderer>() != null)
                                weapon.GetComponent<Renderer>().material = equipDisplay.material;
                        }

                        if (weapon.GetComponent<AtavismItemVFX>() != null)
                        {
                            weapon.GetComponent<AtavismItemVFX>().DisplayVFX(slot, this.node != null ? this.node : node, displayVal);
                        }

                        weapon.SendMessage("SetRootGameObject", gameObject, SendMessageOptions.DontRequireReceiver);

                        weapon.transform.parent = GetRestSocketTransform(equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot);
                        if (genderdef != null)
                        {
                            var slotDef = genderdef.getSlotDef(equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot);
                            if (slotDef != null)
                            {

                                weapon.transform.localScale = slotDef.restScale;
                                weapon.transform.localPosition = slotDef.restPosition;
                                weapon.transform.localRotation = Quaternion.Euler(slotDef.restRotation);
                            }
                            else
                            {
                                Debug.LogError("AttachObject: no slot");
                            }
                        }
                        else
                        {
                            Debug.LogError("AttachObject: no gender");
                        }
#if WeaponOffset
                if (offsetToApply)
                {
                    equipDisplay.weaponOffset = offsetToApply;

                    weapon.transform.localPosition = offsetToApply.ModelPos;
                    weapon.transform.localRotation = Quaternion.Euler(offsetToApply.ModelRot);
                    weapon.transform.localScale = offsetToApply.ModelScale;
                }

#endif
                    }

                    return new ActiveEquipmentDisplay(equipDisplay, weapon, equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot,equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot :  slot);
                }
                else
                {
                    GameObject weapon = null;
                    if (genderdef != null && genderdef.model!=null)
                    {
                        weapon = (GameObject) Instantiate(genderdef.model, GetSocketTransform(equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot).position, GetSocketTransform(equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot).rotation);
                    }
                    if (weapon == null && equipDisplay.model != null)
                        weapon = (GameObject) Instantiate(equipDisplay.model, GetSocketTransform(equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot).position, GetSocketTransform(equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot).rotation);
                    if (weapon != null)
                    {
                        if (equipDisplay.material != null)
                        {
                            if (weapon.GetComponent<Renderer>() != null)
                                weapon.GetComponent<Renderer>().material = equipDisplay.material;
                        }

                        if (weapon.GetComponent<AtavismItemVFX>() != null)
                        {
                            weapon.GetComponent<AtavismItemVFX>().DisplayVFX(slot, this.node != null ? this.node : node, displayVal);
                        }

                        weapon.SendMessage("SetRootGameObject", gameObject, SendMessageOptions.DontRequireReceiver);

                        weapon.transform.parent = GetSocketTransform(equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot);
                        if (genderdef != null)
                        {
                            var slotDef = genderdef.getSlotDef(equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot);
                            if (slotDef != null)
                            {
                                //  Debug.LogError("Pos "+weapon.transform.localPosition+" rot "+weapon.transform.localRotation);
                                //  Debug.LogError("Set Pos "+slotDef.position+" rot "+slotDef.rotation);

                                weapon.transform.localScale = slotDef.scale;
                                weapon.transform.localPosition = slotDef.position;
                                weapon.transform.localRotation = Quaternion.Euler(slotDef.rotation);
                                //  Debug.LogError("End Pos "+weapon.transform.localPosition+" rot "+weapon.transform.localRotation);

                            }
                            else
                            {
                                Debug.LogError("AttachObject: no slot");
                            }
                        }
                        else
                        {
                            Debug.LogError("AttachObject: no gender");
                        }
#if WeaponOffset
                if (offsetToApply)
                {
                    equipDisplay.weaponOffset = offsetToApply;

                    weapon.transform.localPosition = offsetToApply.ModelPos;
                    weapon.transform.localRotation = Quaternion.Euler(offsetToApply.ModelRot);
                    weapon.transform.localScale = offsetToApply.ModelScale;
                }

#endif
                    }

                    return new ActiveEquipmentDisplay(equipDisplay, weapon, equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot,equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot :  slot);
                }
            }

            return null;
        }

       protected ActiveEquipmentDisplay ActivateModel(EquipmentDisplay equipDisplay, string slot)
        {
            if (string.IsNullOrEmpty(equipDisplay.modelName) && equipDisplay.model != null)
                equipDisplay.modelName = equipDisplay.model.name;
            Transform newModel = FindDeepChild(equipDisplay.modelName);
            ActiveEquipmentDisplay activeDisplay = new ActiveEquipmentDisplay(equipDisplay, null, "", slot);
            
            if (newModel != null)
            {
                newModel.gameObject.SetActive(true);
                if (equipDisplay.material != null)
                {
                    activeDisplay.baseMaterial = newModel.GetComponent<Renderer>().material;
                    newModel.GetComponent<Renderer>().material = equipDisplay.material;
                }
                if (newModel.GetComponent<AtavismItemVFX>() != null)
                {
                    newModel.GetComponent<AtavismItemVFX>().DisplayVFX(slot, GetComponent<AtavismNode>(),displayVal);
                }
            }
            return activeDisplay;
        }
       
       private void handleHairEnables(ActiveEquipmentDisplay activeDisplay, ModularCustomizationManager _modularCustomizationManager)
        {
            if (activeDisplay.equipDisplay.headDisables == HeadDisables.Hair && _modularCustomizationManager.ActiveHair)
            {
                _modularCustomizationManager.ActiveHair.SetActive(true);
            }

            if (activeDisplay.equipDisplay.headDisables == HeadDisables.Eyebrows && _modularCustomizationManager.ActiveEyebrow)
            {
                _modularCustomizationManager.ActiveEyebrow.SetActive(true);
            }
            if (activeDisplay.equipDisplay.headDisables == HeadDisables.Beard && _modularCustomizationManager.ActiveBeard)
            {
                _modularCustomizationManager.ActiveBeard.SetActive(true);
            }
            if (activeDisplay.equipDisplay.headDisables == HeadDisables.HairEyebrows)
            {
                if (_modularCustomizationManager.ActiveHair)
                {
                    _modularCustomizationManager.ActiveHair.SetActive(true);
                }
                if (_modularCustomizationManager.ActiveEyebrow)
                {
                    _modularCustomizationManager.ActiveEyebrow.SetActive(true);
                }
            }
            if (activeDisplay.equipDisplay.headDisables == HeadDisables.HairBeard)
            {
                if (_modularCustomizationManager.ActiveHair)
                {
                    _modularCustomizationManager.ActiveHair.SetActive(true);
                }
                if (_modularCustomizationManager.ActiveBeard)
                {
                    _modularCustomizationManager.ActiveBeard.SetActive(true);
                }
            }
            if (activeDisplay.equipDisplay.headDisables == HeadDisables.BeardEyebrows)
            {
                if (_modularCustomizationManager.ActiveBeard)
                {
                    _modularCustomizationManager.ActiveBeard.SetActive(true);
                }
                if (_modularCustomizationManager.ActiveEyebrow)
                {
                    _modularCustomizationManager.ActiveEyebrow.SetActive(true);
                }
            }
            if (activeDisplay.equipDisplay.headDisables == HeadDisables.HairEyebrowsBeard)
            {
                if (_modularCustomizationManager.ActiveHair)
                {
                    _modularCustomizationManager.ActiveHair.SetActive(true);
                }
                if (_modularCustomizationManager.ActiveEyebrow)
                {
                    _modularCustomizationManager.ActiveEyebrow.SetActive(true);
                }
                if (_modularCustomizationManager.ActiveBeard)
                {
                    _modularCustomizationManager.ActiveBeard.SetActive(true);
                }
            }
        }

        private void handleHairDisables(EquipmentDisplay equipDisplay, ModularCustomizationManager _modularCustomizationManager, string slot)
        {
          //  Debug.LogError("handleHairDisables "+equipDisplay.headDisables);
            if (equipDisplay.headDisables == HeadDisables.Hair)
            {
                if (_modularCustomizationManager.ActiveHair)
                {
                    
                        _modularCustomizationManager.ActiveHair.SetActive(false);
                    
                }
                else
                {
                   // Debug.LogError("handleHairDisables dont have hair");
                }
            }
            else
            {
               // Debug.LogError("handleHairDisables no hair");
            }
            if (equipDisplay.headDisables == HeadDisables.Eyebrows)
            {
                if (_modularCustomizationManager.ActiveEyebrow)
                {
                    
                        _modularCustomizationManager.ActiveEyebrow.SetActive(false);
                    
                }
            }
            if (equipDisplay.headDisables == HeadDisables.Beard)
            {
                if (_modularCustomizationManager.ActiveBeard)
                {
                    _modularCustomizationManager.ActiveBeard.SetActive(false);
                }
            }
            if (equipDisplay.headDisables == HeadDisables.HairEyebrows)
            {
                if (_modularCustomizationManager.ActiveHair)
                {
                    _modularCustomizationManager.ActiveHair.SetActive(false);
                }
                if (_modularCustomizationManager.ActiveEyebrow)
                {
                    _modularCustomizationManager.ActiveEyebrow.SetActive(false);
                }
            }
            if (equipDisplay.headDisables == HeadDisables.HairBeard)
            {
                if (_modularCustomizationManager.ActiveHair)
                {
                    _modularCustomizationManager.ActiveHair.SetActive(false);
                }
                if (_modularCustomizationManager.ActiveBeard)
                {
                    _modularCustomizationManager.ActiveBeard.SetActive(false);
                }
            }
            if (equipDisplay.headDisables == HeadDisables.BeardEyebrows)
            {
                if (_modularCustomizationManager.ActiveBeard)
                {
                    _modularCustomizationManager.ActiveBeard.SetActive(false);
                }
                if (_modularCustomizationManager.ActiveEyebrow)
                {
                    _modularCustomizationManager.ActiveEyebrow.SetActive(false);
                }
            }
            if (equipDisplay.headDisables == HeadDisables.HairEyebrowsBeard)
            {
                if (_modularCustomizationManager.ActiveHair)
                {
                    _modularCustomizationManager.ActiveHair.SetActive(false);
                }
                if (_modularCustomizationManager.ActiveEyebrow)
                {
                    _modularCustomizationManager.ActiveEyebrow.SetActive(false);
                }
                if (_modularCustomizationManager.ActiveBeard)
                {
                    _modularCustomizationManager.ActiveBeard.SetActive(false);
                }
            }
        }
        
        private void handleShoulderEnables(ActiveEquipmentDisplay activeDisplay, ModularCustomizationManager _modularCustomizationManager)
        {
            if (ModularCustomizationSettings.Instance == null)
                return;
            if(ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.LeftShoulder) == activeDisplay.slot||ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.RightShoulder) == activeDisplay.slot)
            //if (activeDisplay.equipDisplay.socket == AttachmentSocket.LeftShoulder || activeDisplay.equipDisplay.socket == AttachmentSocket.RightShoulder)
            {
                // if ((activeDisplay.equipDisplay.shoulderDisables == ShoulderDisables.UpperArms || activeDisplay.equipDisplay.shoulderDisables == ShoulderDisables.UpperAndLowerArms == true) && _modularCustomizationManager.ActiveHands)
                if (activeDisplay.equipDisplay.shoulderDisables == ShoulderDisables.UpperArms || activeDisplay.equipDisplay.shoulderDisables == ShoulderDisables.UpperAndLowerArms == true)
                {
                   // _modularCustomizationManager.ActiveUpperArms.SetActive(true);
                    foreach (var go in _modularCustomizationManager.ActiveUpperArms)
                    {
                        go.SetActive(false);     
                    }
                    if (activeDisplay.equipDisplay.shoulderDisables == ShoulderDisables.UpperAndLowerArms)
                    {
                       // _modularCustomizationManager.ActiveLowerArms.SetActive(true);
                        foreach (var go in _modularCustomizationManager.ActiveLowerArms)
                        {
                            go.SetActive(false);     
                        }
                    }
                }
            }
            if(ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.LeftElbow) == activeDisplay.slot||ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.RightElbow) == activeDisplay.slot)
//            if (activeDisplay.equipDisplay.socket == AttachmentSocket.LeftElbow || activeDisplay.equipDisplay.socket == AttachmentSocket.RightElbow)
            {

                if (activeDisplay.equipDisplay.shoulderDisables == ShoulderDisables.UpperAndLowerArms)
                {
                   // _modularCustomizationManager.ActiveLowerArms.SetActive(true);
                    foreach (var go in _modularCustomizationManager.ActiveLowerArms)
                    {
                        go.SetActive(false);     
                    }
                }

            }
        }


        private void handleShoulderDisables(EquipmentDisplay equipDisplay, ModularCustomizationManager _modularCustomizationManager, string slot)
        {  
            if (ModularCustomizationSettings.Instance == null)
                return;
            if(ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.LeftShoulder) == slot||ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.RightShoulder) == slot)
          //  if (equipDisplay.socket == AttachmentSocket.LeftShoulder || equipDisplay.socket == AttachmentSocket.RightShoulder)
            {
                if (equipDisplay.shoulderDisables == ShoulderDisables.UpperArms || equipDisplay.shoulderDisables == ShoulderDisables.UpperAndLowerArms == true)
                {
                    if (_modularCustomizationManager.ActiveUpperArms!=null)
                    {
                        foreach (var go in _modularCustomizationManager.ActiveUpperArms)
                        {
                            go.SetActive(false);     
                        }
                    }
                    if (equipDisplay.shoulderDisables == ShoulderDisables.UpperAndLowerArms)
                    {
                        if (_modularCustomizationManager.ActiveLowerArms!=null)
                        {
                            foreach (var go in _modularCustomizationManager.ActiveLowerArms)
                            {
                                go.SetActive(false);     
                            }
                        }
                    }
                }
            }
            if(ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.LeftElbow) == slot||ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.RightElbow) == slot)
          //  if (equipDisplay.socket == AttachmentSocket.LeftElbow || equipDisplay.socket == AttachmentSocket.RightElbow)
            {
                if (equipDisplay.shoulderDisables == ShoulderDisables.LowerArms)
                {
                    if (_modularCustomizationManager.ActiveLowerArms!=null)
                    {
                        foreach (var go in _modularCustomizationManager.ActiveLowerArms)
                        {
                            go.SetActive(false);     
                        }
                    }
                }
            }
        }




        //  Turns on Modular Armor
        protected ActiveEquipmentDisplay ActivateModularArmor(EquipmentDisplay equipDisplay, string slot)
        {
           // Debug.LogError("ActivateModularArmor "+slot);
            ActiveEquipmentDisplay activeDisplay = new ActiveEquipmentDisplay(equipDisplay, null, equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot,equipDisplay.isOverrideSlot() ? equipDisplay.overrideSlot : slot);
         //   string currentRace = CharacterSelectionCreationManager.Instance.raceName;
      
         AtavismNode node = GetComponent<AtavismNode>();
         if (node)
         {
              gender = node.PropertyExists("genderId") ? (int) node.GetProperty("genderId") : -1;
              race = node.PropertyExists("race") ? (int) node.GetProperty("race") : -1;
              aspect = node.PropertyExists("aspect") ? (int) node.GetProperty("aspect") : -1;
         }
         
         Transform newModel = FindDeepChild(equipDisplay.modelName);
         if (newModel)
         {
             newModel.gameObject.SetActive(true);
             if (newModel.GetComponent<AtavismItemVFX>() != null)
             {
                 newModel.GetComponent<AtavismItemVFX>().DisplayVFX(slot, this.node != null ? this.node : node, displayVal);
             }
         }
         var genderdef = activeDisplay.equipDisplay.GetGanderDisplayDef(race, aspect, gender);
         var racedef = activeDisplay.equipDisplay.GetRaceDisplayDef(race);
         var classdef = activeDisplay.equipDisplay.GetClassDisplayDef(race, aspect);
        // if(genderdef.pa)
            ModularCustomizationManager _modularCustomizationManager = GetComponent<ModularCustomizationManager>();
            
            if (ModularCustomizationSettings.Instance != null && _modularCustomizationManager)
            {
                bool isGender = false;
                bool isClass = false;
                bool isRace = false;

                if (racedef != null)
                {
                    if( racedef.disableGearParts.Count > 0 || racedef.disableGearPartsName.Count>0 || racedef.GearParts.Count>0|| racedef.GearPartsName.Count>0)
                    {
                        isRace = true;
                    }
                }
                if (classdef != null)
                {
                    if( classdef.disableGearParts.Count > 0 || classdef.disableGearPartsName.Count>0 || classdef.GearParts.Count>0|| classdef.GearPartsName.Count>0)
                    {
                        isClass = true;
                    }
                }
                if (genderdef != null)
                {
                    if( genderdef.disableGearParts.Count > 0 || genderdef.disableGearPartsName.Count>0 || genderdef.GearParts.Count>0|| genderdef.GearPartsName.Count>0)
                    {
                        isGender = true;
                    }
                }

                if (!isRace && !isClass && !isGender && newModel == null)
                {
                    return activeDisplay;
                }
                
#if ModularNHance
                if (_modularCustomizationManager.nHanceModelSwapping)
                {
                    if (racedef != null)
                    {
                        foreach (GameObject gear in racedef.disableGearParts)
                        {
                            Transform armor = FindDeepChild(gear.name);
                            if (armor)
                            {
                                Destroy(armor.gameObject);
                            }
                        }
                        foreach (GameObject gear in racedef.GearParts)
                        {
                            _modularCustomizationManager.enableNHance(gear);
                        }
                    }

                    if (classdef != null)
                    {
                        foreach (GameObject gear in classdef.disableGearParts)
                        {
                            Transform armor = FindDeepChild(gear.name);
                            if (armor)
                            {
                                Destroy(armor.gameObject);
                            }
                        }
                        
                        foreach (GameObject gear in classdef.GearParts)
                        {
                            _modularCustomizationManager.enableNHance(gear);
                        }
                    }

                    if (genderdef != null)
                    {
                        foreach (GameObject gear in genderdef.disableGearParts)
                        {
                            Transform armor = FindDeepChild(gear.name);
                            if (armor)
                            {
                                Destroy(armor.gameObject);
                            }
                        }

                        foreach (GameObject gear in genderdef.GearParts)
                        {
                            _modularCustomizationManager.enableNHance(gear);
                        }
                    }

                  
             

                    /*    if (_modularCustomizationManager.gender == characterGender.Female)
                        {
                            foreach (GameObject item in equipDisplay.FemaleGearParts)
                            {
                                _modularCustomizationManager.enableNHance(item);
                            }
                        }
                        else
                        {
                            foreach (GameObject item in equipDisplay.GearParts)
                            {
                                _modularCustomizationManager.enableNHance(item);
                            }
                        }*/
                }
                else
                {
#endif
                /* if (equipDisplay.socket == AttachmentSocket.None)
                 {
                 }*/
                if (ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.MainHand) == activeDisplay.slot ||
                    ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.MainHand2) == activeDisplay.slot ||
                    ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.OffHand) == activeDisplay.slot ||
                    ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.OffHand2) == activeDisplay.slot)
                    //if (equipDisplay.socket == AttachmentSocket.MainHand2 || equipDisplay.socket == AttachmentSocket.MainHand || equipDisplay.socket == AttachmentSocket.OffHand || equipDisplay.socket == AttachmentSocket.OffHand2)
                {

                    if (_modularCustomizationManager.ActiveHands!=null)
                    {
                        foreach (var go in _modularCustomizationManager.ActiveHands)
                        {
                            go.SetActive(false);     
                        }
                    }

                }

                if (ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.Chest) == activeDisplay.slot)

                    // if (equipDisplay.socket == AttachmentSocket.Chest)
                {
                    if (_modularCustomizationManager.ActiveTorso!=null)
                    {
                      //  Debug.LogError("Torso "+_modularCustomizationManager.ActiveTorso.Count);
                        foreach (var go in _modularCustomizationManager.ActiveTorso)
                        {
                         //   Debug.LogError("Torso "+go.name+" "+go.activeSelf+" set to false");
                            go.SetActive(false); 
                            
                        }
                    }
                }

                if (ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.LeftShoulder) == activeDisplay.slot || ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.RightShoulder) == activeDisplay.slot)
                    // if (equipDisplay.socket == AttachmentSocket.LeftShoulder || equipDisplay.socket == AttachmentSocket.RightShoulder)
                {
                    if (_modularCustomizationManager.ActiveUpperArms!=null)
                    {
                        foreach (var go in _modularCustomizationManager.ActiveUpperArms)
                        {
                            go.SetActive(false);     
                        };
                    }
                }


                if (ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.LeftElbow) == activeDisplay.slot || ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.RightElbow) == activeDisplay.slot)
                    // if (equipDisplay.socket == AttachmentSocket.LeftElbow || equipDisplay.socket == AttachmentSocket.RightElbow)
                {
                    if (_modularCustomizationManager.ActiveLowerArms!=null)
                    {
                        foreach (var go in _modularCustomizationManager.ActiveLowerArms)
                        {
                            go.SetActive(false);     
                        }
                    }
                }

                if (ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.Head) == activeDisplay.slot)
                    //if (equipDisplay.socket == AttachmentSocket.Head)
                {
                    if (_modularCustomizationManager.ActiveHead!=null)
                    {
                        foreach (var go in _modularCustomizationManager.ActiveHead)
                        {
                            go.SetActive(false);     
                        }
                    }
                }

                if (ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.HeadCovering) == activeDisplay.slot)
                    //if (equipDisplay.socket == AttachmentSocket.HeadCoverings)
                {
                    if (_modularCustomizationManager.ActiveHeadCovering!=null)
                    {
                        foreach (var go in _modularCustomizationManager.ActiveHeadCovering)
                        {
                            go.SetActive(false);     
                        }
                    }
                }

                if (ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.Pelvis) == activeDisplay.slot)
                    //  if (equipDisplay.socket == AttachmentSocket.Pelvis)
                {
                    if (_modularCustomizationManager.ActiveHips!=null)
                    {
                        foreach (var go in _modularCustomizationManager.ActiveHips)
                        {
                            go.SetActive(false);     
                        }
                    }
                }

                if (ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.LeftKnee) == activeDisplay.slot || ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.RightKnee) == activeDisplay.slot)
                    // if (equipDisplay.socket == AttachmentSocket.LeftKnee || equipDisplay.socket == AttachmentSocket.RightKnee)
                {
                    if (_modularCustomizationManager.ActiveLowerLegs!=null)
                    {
                        foreach (var go in _modularCustomizationManager.ActiveLowerLegs)
                        {
                            go.SetActive(false);     
                        }
                    }
                }

                if (ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.LeftFoot) == activeDisplay.slot || ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.RightFoot) == activeDisplay.slot)
                    //  if (equipDisplay.socket == AttachmentSocket.LeftFoot || equipDisplay.socket == AttachmentSocket.RightFoot)
                {
                    if (_modularCustomizationManager.ActiveFeet!=null)
                    {
                        foreach (var go in _modularCustomizationManager.ActiveFeet)
                        {
                            go.SetActive(false);     
                        }
                    }
                }

                handleShoulderDisables(equipDisplay, _modularCustomizationManager, activeDisplay.slot);
                handleHairDisables(equipDisplay, _modularCustomizationManager, activeDisplay.slot);
                if (racedef != null)
                {
                    //disable
                    foreach (GameObject gear in racedef.disableGearParts)
                    {
                        if (gear)
                        {
                            Transform armor = FindDeepChild(gear.name);
                            if (armor)
                            {
                                armor.gameObject.SetActive(false);
                               
                            }
                        }
                    }

                    foreach (string gear in racedef.disableGearPartsName)
                    {
                        Transform armor = FindDeepChild(gear);
                        if (armor)
                        {
                            armor.gameObject.SetActive(false);
                            
                        }
                    }
                    //enable
                    foreach (GameObject gear in racedef.GearParts)
                    {
                        if (gear)
                        {
                            Transform armor = FindDeepChild(gear.name);
                            if (armor)
                            {
                                armor.gameObject.SetActive(true);
                                if (armor.GetComponent<AtavismItemVFX>() != null)
                                {
                                    armor.GetComponent<AtavismItemVFX>().DisplayVFX(slot, this.node != null ? this.node : node, displayVal);
                                }
                            }
                        }
                    }

                    foreach (string gear in racedef.GearPartsName)
                    {
                        Transform armor = FindDeepChild(gear);
                        if (armor)
                        {
                            armor.gameObject.SetActive(true);
                            if (armor.GetComponent<AtavismItemVFX>() != null)
                            {
                                armor.GetComponent<AtavismItemVFX>().DisplayVFX(slot, this.node != null ? this.node : node, displayVal);
                            }
                        }
                    }
                }

                if (classdef != null)
                {
                    //Disable
                    foreach (GameObject gear in classdef.disableGearParts)
                    {
                        if (gear)
                        {
                            Transform armor = FindDeepChild(gear.name);
                            if (armor)
                            {
                                armor.gameObject.SetActive(false);
                              
                            }
                        }
                    }

                    foreach (string gear in classdef.disableGearPartsName)
                    {
                        Transform armor = FindDeepChild(gear);
                        if (armor)
                        {
                            armor.gameObject.SetActive(false);
                           
                        }
                    }
                    //Enable
                    foreach (GameObject gear in classdef.GearParts)
                    {
                        if (gear)
                        {
                            Transform armor = FindDeepChild(gear.name);
                            if (armor)
                            {
                                armor.gameObject.SetActive(true);
                                if (armor.GetComponent<AtavismItemVFX>() != null)
                                {
                                    armor.GetComponent<AtavismItemVFX>().DisplayVFX(slot, this.node != null ? this.node : node, displayVal);
                                }
                            }
                        }
                    }

                    foreach (string gear in classdef.GearPartsName)
                    {
                        Transform armor = FindDeepChild(gear);
                        if (armor)
                        {
                            armor.gameObject.SetActive(true);
                            if (armor.GetComponent<AtavismItemVFX>() != null)
                            {
                                armor.GetComponent<AtavismItemVFX>().DisplayVFX(slot, this.node != null ? this.node : node, displayVal);
                            }
                        }
                    }
                }

                if (genderdef != null)
                {
                    //Disable
                    foreach (GameObject gear in genderdef.disableGearParts)
                    {
                        if (gear)
                        {
                            Transform armor = FindDeepChild(gear.name);
                            if (armor)
                            {
                                armor.gameObject.SetActive(false);
                            }
                        }
                    }

                    foreach (string gear in genderdef.disableGearPartsName)
                    {
                        Transform armor = FindDeepChild(gear);
                        if (armor)
                        {
                          //  Debug.LogError(armor.gameObject + " "+armor.gameObject.activeSelf+" set to false");
                            armor.gameObject.SetActive(false);
                          //  Debug.LogError(armor.gameObject + " "+armor.gameObject.activeSelf+" set2 to false");
                        }
                    }

                    //Enable
                    foreach (GameObject gear in genderdef.GearParts)
                    {
                        if (gear)
                        {
                            Transform armor = FindDeepChild(gear.name);
                            if (armor)
                            {
                                armor.gameObject.SetActive(true);
                                if (armor.GetComponent<AtavismItemVFX>() != null)
                                {
                                    armor.GetComponent<AtavismItemVFX>().DisplayVFX(slot, this.node != null ? this.node : node, displayVal);
                                }
                            }
                        }
                    }

                    foreach (string gear in genderdef.GearPartsName)
                    {
                        Transform armor = FindDeepChild(gear);
                        if (armor)
                        {
                           // Debug.LogError(armor.gameObject + " set to true");
                            armor.gameObject.SetActive(true);
                            if (armor.GetComponent<AtavismItemVFX>() != null)
                            {
                                armor.GetComponent<AtavismItemVFX>().DisplayVFX(slot, this.node != null ? this.node : node, displayVal);
                            }
                        }
                    }
                }


#if ModularNHance
                }
#endif
            }

            return activeDisplay;
        }

        
        protected ActiveEquipmentDisplay SwapBaseModelTexture(EquipmentDisplay equipDisplay, string slot)
        {
            if (string.IsNullOrEmpty(equipDisplay.modelName) && equipDisplay.model != null)
                equipDisplay.modelName = equipDisplay.model.name;
            Transform model = FindDeepChild(equipDisplay.modelName);
            if (model)
            {

                // Store the base material first
                ActiveEquipmentDisplay activeDisplay = new ActiveEquipmentDisplay(equipDisplay, null, model.GetComponent<Renderer>().material, slot);
                model.GetComponent<Renderer>().material = equipDisplay.material;
                return activeDisplay;
            }

            return null;
        }

        protected void RemoveAttachedObject(ActiveEquipmentDisplay activeDisplay, string slot)
        {
            Destroy(activeDisplay.attachedObject);
            ModularCustomizationManager _modularCustomizationManager = GetComponent<ModularCustomizationManager>();
            if (_modularCustomizationManager)
            {
                handleHairEnables(activeDisplay, _modularCustomizationManager);
                handleShoulderEnables(activeDisplay, _modularCustomizationManager);
            }

        }

        protected void DeactivateModel(ActiveEquipmentDisplay activeDisplay)
        {
            if (string.IsNullOrEmpty(activeDisplay.equipDisplay.modelName) && activeDisplay.equipDisplay.model != null)
                activeDisplay.equipDisplay.modelName = activeDisplay.equipDisplay.model.name;
            Transform model = FindDeepChild(activeDisplay.equipDisplay.modelName);
            if (model != null)
            {
                if (activeDisplay.baseMaterial != null)
                {
                    model.GetComponent<Renderer>().material = activeDisplay.baseMaterial;
                }
                model.gameObject.SetActive(false);
            }
        }
        //  Turns off modular armor
        protected void DeactivateModularArmor(ActiveEquipmentDisplay activeDisplay, string slot)
        {
          //  Debug.LogError("DeactivateModularArmor "+slot);
            // string currentRace = CharacterSelectionCreationManager.Instance.raceName;
            AtavismNode node = GetComponent<AtavismNode>();
            if (node)
            {
                gender = node.PropertyExists("genderId") ? (int) node.GetProperty("genderId") : -1;
                race = node.PropertyExists("race") ? (int) node.GetProperty("race") : -1;
                aspect = node.PropertyExists("aspect") ? (int) node.GetProperty("aspect") : -1;
            }

            Transform newModel = FindDeepChild(activeDisplay.equipDisplay.modelName);
            if (newModel)
            {
                newModel.gameObject.SetActive(false);
            }

            var genderdef = activeDisplay.equipDisplay.GetGanderDisplayDef(race, aspect, gender);
            var racedef = activeDisplay.equipDisplay.GetRaceDisplayDef(race);
            var classdef = activeDisplay.equipDisplay.GetClassDisplayDef(race, aspect);
            ModularCustomizationManager _modularCustomizationManager = GetComponent<ModularCustomizationManager>();
            if (_modularCustomizationManager)
            {
#if ModularNHance
                if (_modularCustomizationManager.nHanceModelSwapping)
                {
                    
                    foreach (GameObject gear in racedef.GearParts)
                    {
                        Transform armor = FindDeepChild(gear.name);
                        if (armor)
                        {
                            Destroy(armor.gameObject);
                        }
                    }
                    foreach (GameObject gear in classdef.GearParts)
                    {
                        Transform armor = FindDeepChild(gear.name);
                        if (armor)
                        {
                            Destroy(armor.gameObject);
                        }
                    }
                    foreach (GameObject gear in genderdef.GearParts)
                    {
                        Transform armor = FindDeepChild(gear.name);
                        if (armor)
                        {
                            Destroy(armor.gameObject);
                        }
                    }

                    
                    /*
                    if (_modularCustomizationManager.gender == characterGender.Female)
                    {

                            foreach (GameObject gear in activeDisplay.equipDisplay.FemaleGearParts)
                            {
                                Transform armor = FindDeepChild(gear.name);
                                if (armor)
                                {
                                    Destroy(armor.gameObject);
                                }
                            }
                        
                    }
                    else
                    {
      
                            foreach (GameObject gear in activeDisplay.equipDisplay.GearParts)
                            {
                                Transform armor = FindDeepChild(gear.name);
                                Destroy(armor.gameObject);
                            }
                        
                    }*/
                }
                else
                {
#endif
                if (racedef != null)
                {
                    foreach (GameObject gear in racedef.GearParts)
                    {
                        if (gear)
                        {
                            Transform armor = FindDeepChild(gear.name);
                            if (armor)
                            {
                                armor.gameObject.SetActive(false);
                            }
                        }
                    }

                    foreach (string gear in racedef.GearPartsName)
                    {
                        Transform armor = FindDeepChild(gear);
                        if (armor)
                        {
                            armor.gameObject.SetActive(false);
                        }
                    }
                    
                    foreach (GameObject gear in racedef.disableGearParts)
                    {
                        if (gear)
                        {
                            Transform armor = FindDeepChild(gear.name);
                            if (armor)
                            {
                                armor.gameObject.SetActive(true);
                            }
                        }
                    }

                    foreach (string gear in racedef.disableGearPartsName)
                    {
                        Transform armor = FindDeepChild(gear);
                        if (armor)
                        {
                            armor.gameObject.SetActive(true);
                        }
                    }
                }

                if (classdef != null)
                {
                    foreach (GameObject gear in classdef.GearParts)
                    {
                        if (gear)
                        {
                            Transform armor = FindDeepChild(gear.name);
                            if (armor)
                            {
                                armor.gameObject.SetActive(false);
                            }
                        }
                    }

                    foreach (string gear in classdef.GearPartsName)
                    {
                        Transform armor = FindDeepChild(gear);
                        if (armor)
                        {
                            armor.gameObject.SetActive(false);
                        }
                    }
                    foreach (GameObject gear in classdef.disableGearParts)
                    {
                        if (gear)
                        {
                            Transform armor = FindDeepChild(gear.name);
                            if (armor)
                            {
                                armor.gameObject.SetActive(true);
                            }
                        }
                    }

                    foreach (string gear in classdef.disableGearPartsName)
                    {
                        Transform armor = FindDeepChild(gear);
                        if (armor)
                        {
                            armor.gameObject.SetActive(true);
                        }
                    }
                }

                if (genderdef != null)
                {
                    foreach (GameObject gear in genderdef.GearParts)
                    {
                        if (gear)
                        {
                            Transform armor = FindDeepChild(gear.name);
                            if (armor)
                            {
                                armor.gameObject.SetActive(false);
                            }
                        }
                    }

                    foreach (string gear in genderdef.GearPartsName)
                    {
                        Transform armor = FindDeepChild(gear);
                        if (armor)
                        {
                           // Debug.LogError(armor.gameObject + " srt to false");
                            armor.gameObject.SetActive(false);
                        }
                    }
                    foreach (GameObject gear in genderdef.disableGearParts)
                    {
                        if (gear)
                        {
                            Transform armor = FindDeepChild(gear.name);
                            if (armor)
                            {
                                
                                armor.gameObject.SetActive(true);
                            }
                        }
                    }

                    foreach (string gear in genderdef.disableGearPartsName)
                    {
                        Transform armor = FindDeepChild(gear);
                        if (armor)
                        {
                          //  Debug.LogError(armor.gameObject + " srt to true");
                            armor.gameObject.SetActive(true);
                        }
                    }
                }

                /* if (activeDisplay.equipDisplay.socket == AttachmentSocket.None)
                 {
                 }*/
                if (ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.MainHand) == activeDisplay.slot ||
                    ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.MainHand2) == activeDisplay.slot ||
                    ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.OffHand) == activeDisplay.slot ||
                    ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.OffHand2) == activeDisplay.slot)
                    if (_modularCustomizationManager.ActiveHands !=null)
                    {
                        foreach (var go in _modularCustomizationManager.ActiveHands)
                        {
                            go.SetActive(true);     
                        }
                    }


                if (ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.Chest) == activeDisplay.slot && _modularCustomizationManager.ActiveTorso !=null)
                {
                    foreach (var go in _modularCustomizationManager.ActiveTorso)
                    {
                       // Debug.LogError("Torso "+go.name+" "+go.activeSelf+" set to true");
                        go.SetActive(true);     
                    }
                }

                if ((ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.LeftShoulder) == activeDisplay.slot || ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.RightShoulder) == activeDisplay.slot) && _modularCustomizationManager.ActiveUpperArms !=null)
                {
                    if (_modularCustomizationManager.ActiveUpperArms !=null)
                    {
                        foreach (var go in _modularCustomizationManager.ActiveUpperArms)
                        {
                            go.SetActive(true);     
                        }
                    }
                }

                if ((ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.LeftElbow) == activeDisplay.slot || ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.RightElbow) == activeDisplay.slot) && _modularCustomizationManager.ActiveLowerArms !=null)
                {
                    if (_modularCustomizationManager.ActiveLowerArms !=null)
                    {
                        foreach (var go in _modularCustomizationManager.ActiveLowerArms)
                        {
                            go.SetActive(true);     
                        }
                    }
                }

                if (ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.Head) == activeDisplay.slot && _modularCustomizationManager.ActiveHead !=null)
                {
                    foreach (var go in _modularCustomizationManager.ActiveHead)
                    {
                        go.SetActive(true);     
                    }
                }

                if (ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.HeadCovering) == activeDisplay.slot && _modularCustomizationManager.ActiveHeadCovering !=null)
                {
                    foreach (var go in _modularCustomizationManager.ActiveHeadCovering)
                    {
                        go.SetActive(true);     
                    }
                }

                if (ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.Pelvis) == activeDisplay.slot && _modularCustomizationManager.ActiveHips !=null)
                {
                    foreach (var go in _modularCustomizationManager.ActiveHips)
                    {
                        go.SetActive(true);     
                    }
                }

                if ((ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.LeftKnee) == activeDisplay.slot || ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.RightKnee) == activeDisplay.slot) && _modularCustomizationManager.ActiveLowerLegs.Count>0)
                {
                    foreach (var go in _modularCustomizationManager.ActiveLowerLegs)
                    {
                        go.SetActive(true);     
                    }
                }
                else if ((ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.LeftKnee) == activeDisplay.slot || ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.RightKnee) == activeDisplay.slot) && _modularCustomizationManager.ActiveFeet.Count>0 &&
                         _modularCustomizationManager.ActiveLowerLegs.Count==0)
                {
                    foreach (var go in _modularCustomizationManager.ActiveFeet)
                    {
                        go.SetActive(true);     
                    }
                }

                if ((ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.LeftFoot) == activeDisplay.slot || ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.RightFoot) == activeDisplay.slot) && _modularCustomizationManager.ActiveFeet.Count>0)
                {
                    foreach (var go in _modularCustomizationManager.ActiveFeet)
                    {
                        go.SetActive(true);     
                    }
                }
                else if ((ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.LeftFoot) == activeDisplay.slot || ModularCustomizationSettings.Instance.getSlot(AttachmentSocket.RightFoot) == activeDisplay.slot) && _modularCustomizationManager.ActiveLowerLegs.Count>0 &&
                         _modularCustomizationManager.ActiveFeet.Count==0)
                {
                    foreach (var go in _modularCustomizationManager.ActiveLowerLegs)
                    {
                        go.SetActive(true);     
                    }
                }

                handleShoulderEnables(activeDisplay, _modularCustomizationManager);
                handleHairEnables(activeDisplay, _modularCustomizationManager);
#if ModularNHance
                }
#endif
            }
        }


        protected void ResetBaseTexture(ActiveEquipmentDisplay activeDisplay)
        {
            if (string.IsNullOrEmpty(activeDisplay.equipDisplay.modelName) && activeDisplay.equipDisplay.model != null)
                activeDisplay.equipDisplay.modelName = activeDisplay.equipDisplay.model.name;
            Transform model = FindDeepChild(activeDisplay.equipDisplay.modelName);
            model.GetComponent<Renderer>().material = activeDisplay.baseMaterial;
        }


        public void HandleDeadState(object sender, PropertyChangeEventArgs args)
        {
            dead = (bool)GetComponent<AtavismNode>().GetProperty(args.PropertyName);
        }

        public void ResetAttachObject()
        {
            foreach (List<ActiveEquipmentDisplay> activeDisplays in activeEquipDisplays.Values)
            foreach (ActiveEquipmentDisplay activeDisplay in activeDisplays)
            {
                if (activeDisplay.attachedObject != null)
                {
                  //  Debug.LogError("ResetAttachObject "+activeDisplay.attachedObject.name);
                    DestroyImmediate(activeDisplay.attachedObject);
                }
            }
        }

        public void ReApplyEquipDisplay()
        {
            foreach (string slot in Inventory.Instance.itemSlots.Keys)
            {
                if (gameObject.GetComponent<AtavismNode>()!=null)
                {
                    string displayID = (string) GetComponent<AtavismNode>().GetProperty(slot + "DisplayID");
                    if (AtavismSettings.Instance.CharacterAvatar != null)
                    {
                        if (GetComponent<AtavismNode>() != null)
                            if (GetComponent<AtavismNode>().Oid == ClientAPI.GetPlayerOid())
                            {
                                AtavismNode node = GetComponent<AtavismNode>();
                                // AtavismSettings.Instance.CharacterAvatar.GetComponent<AtavismMobAppearance>().node = GetComponent<AtavismNode>();
                                AtavismSettings.Instance.CharacterAvatar.GetComponent<AtavismMobAppearance>().gender = node.PropertyExists("genderId") ? (int) node.GetProperty("genderId") : -1;
                                AtavismSettings.Instance.CharacterAvatar.GetComponent<AtavismMobAppearance>().race = node.PropertyExists("race") ? (int) node.GetProperty("race") : -1;
                                AtavismSettings.Instance.CharacterAvatar.GetComponent<AtavismMobAppearance>().aspect = node.PropertyExists("aspect") ? (int) node.GetProperty("aspect") : -1;
                                AtavismSettings.Instance.CharacterAvatar.GetComponent<AtavismMobAppearance>().displayVal = (string) GetComponent<AtavismNode>().GetProperty(slot + "DisplayVAL");
                                AtavismSettings.Instance.CharacterAvatar.GetComponent<AtavismMobAppearance>().UpdateEquipDisplay(slot + "DisplayID", displayID);
                            }
                    }


                    if (AtavismSettings.Instance.OtherCharacterAvatar != null)
                    {
                        //   Debug.LogError("Other "+GetComponent<AtavismNode>().Oid+" "+UGUIOtherCharacterPanel.Instance.Id);
                        if (GetComponent<AtavismNode>() != null && UGUIOtherCharacterPanel.Instance != null)
                        {
                            //    Debug.LogError("Other |");
                            if (GetComponent<AtavismNode>().Oid == UGUIOtherCharacterPanel.Instance.Id)
                            {
                                //   Debug.LogError("Other ||");
                                AtavismNode node = GetComponent<AtavismNode>();
                                // AtavismSettings.Instance.OtherCharacterAvatar.GetComponent<AtavismMobAppearance>().node = GetComponent<AtavismNode>();
                                AtavismSettings.Instance.OtherCharacterAvatar.GetComponent<AtavismMobAppearance>().gender = node.PropertyExists("genderId") ? (int) node.GetProperty("genderId") : -1;
                                AtavismSettings.Instance.OtherCharacterAvatar.GetComponent<AtavismMobAppearance>().race = node.PropertyExists("race") ? (int) node.GetProperty("race") : -1;
                                AtavismSettings.Instance.OtherCharacterAvatar.GetComponent<AtavismMobAppearance>().aspect = node.PropertyExists("aspect") ? (int) node.GetProperty("aspect") : -1;
                                AtavismSettings.Instance.OtherCharacterAvatar.GetComponent<AtavismMobAppearance>().displayVal = (string) GetComponent<AtavismNode>().GetProperty(slot + "DisplayVAL");
                                AtavismSettings.Instance.OtherCharacterAvatar.GetComponent<AtavismMobAppearance>().UpdateEquipDisplay(slot + "DisplayID", displayID);
                            }
                        }
                    }

                    UpdateEquipDisplay(slot + "DisplayID", displayID);
                }
            }
        }



        public void SetupSockets(AtavismMobSockets ams)
        {

            this.slots = ams.slots;

            this.sockets = ams.sockets;
            this.restsockets = ams.restsockets;
        
            
          
            SetWeaponsAttachmentSlot(true);
        }

    }
}