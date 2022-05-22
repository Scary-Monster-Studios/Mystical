using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Structure of a Atavism Abilities
/*
/* Table structure for tables related to abilities
/*

CREATE TABLE `abilities` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `abilityType` varchar(64) DEFAULT NULL,
  `skill` int(11) DEFAULT NULL,
  `passive` tinyint(1) DEFAULT NULL,
  `activationCost` int(11) DEFAULT NULL,
  `activationCostType` varchar(32) DEFAULT NULL,
  `activationLength` int(11) DEFAULT NULL,
  `activationAnimation` varchar(32) DEFAULT NULL,
  `activationParticles` varchar(32) DEFAULT NULL,
  `casterEffectRequired` int(11) DEFAULT NULL,
  `casterEffectConsumed` tinyint(1) DEFAULT NULL,
  `targetEffectRequired` int(11) DEFAULT NULL,
  `targetEffectConsumed` tinyint(1) DEFAULT NULL,
  `weaponRequired` varchar(32) DEFAULT NULL,
  `reagentRequired` int(11) NOT NULL DEFAULT '-1',
  `reagentConsumed` tinyint(1) DEFAULT NULL,
  `maxRange` int(11) DEFAULT NULL,
  `minRange` int(11) DEFAULT NULL,
  `aoeRadius` int(11) NOT NULL DEFAULT '0',
  `targetType` varchar(32) DEFAULT NULL,
  `targetState` int(11) DEFAULT NULL,
  `speciesTargetReq` varchar(32) DEFAULT NULL,
  `specificTargetReq` varchar(64) DEFAULT NULL,
  `globalCooldown` tinyint(1) DEFAULT NULL,
  `cooldown1Type` varchar(32) DEFAULT NULL,
  `cooldown1Duration` int(11) DEFAULT NULL,
  `weaponCooldown` tinyint(1) DEFAULT NULL,
  `activationEffect1` int(11) DEFAULT NULL,
  `activationTarget1` varchar(32) DEFAULT NULL,
  `activationEffect2` int(11) DEFAULT NULL,
  `activationTarget2` varchar(32) DEFAULT NULL,
  `activationEffect3` int(11) DEFAULT NULL,
  `activationTarget3` varchar(32) DEFAULT NULL,
  `coordEffect1event` varchar(32) DEFAULT NULL,
  `coordEffect1` varchar(64) DEFAULT NULL,
  `coordEffect2event` varchar(32) DEFAULT NULL,
  `coordEffect2` varchar(64) DEFAULT NULL,
    
The ability system is complicated because each ability can have up to 3 effects (which are a separate table)
 and many of the fields for the ability link back to effects. 
 
 The system allows for adding effect requirements to activate an ability or add bonuses and the like.

The other part is coordinated effects. These are special effect tools that allow things like animations,
 particles and sounds to be played when using an ability. These are defined solely on the client (as they have no effect on the game, it's all cosmetic).

*/
namespace Atavism
{
    public class AbilitiesData : DataStructure
    {
        //public int id = 0;					// Database Index
        // General Parameters
       // public string name = "name";        // The ability template name
        public string icon = "";            // The ability icon
        public string abilityType = "";
        public string[] abilityTypeOptions = new string[] { "CombatMeleeAbility", "MagicalAttackAbility", "EffectAbility", "FriendlyEffectAbility" };
        public int skill = -1;                  // what skill this ability belongs to
        public bool passive = false;                // Does this ability auto apply it's effects as soon as it is learned, or is it an ability that requires activation
        public int activationCost = 0;          // The value of the specified resource required to activate the ability
        public float activationCostPercentage = 0;          // The value of the specified resource required to activate the ability
        public string activationCostType = "";  // What resource this ability requires to activate (either mana or health)
        public float activationLength = 0;      // How long it takes to activate the ability (cast time). The player cannot move while casting
        public string activationAnimation = "";     // The animation to play while casting
        public string activationParticles = "";     // The particle to apply while casting
        public int casterEffectRequired = 0;    // Does the caster require an effect on them to activate the ability (generally set to 0: no, otherwise the id of the effect required)
        public bool casterEffectConsumed = false;   // Is the effect removed from the caster
        public int targetEffectRequired = 0;    // Does the targetrequire an effect on them for the ability to activate (generally set to 0: no, otherwise the id of the effect required)
        public bool targetEffectConsumed;   // Is the effect removed from the target
        public string weaponRequired = "";      // What weapon type is required to activate the ability (None, Sword, Axe, Mace etc);
        public int reagentRequired = 0;         // Is an item required to activate this ability, if so, which one (0: no, otherwise the id of the item)
        public int reagentCount = 1;
        public bool reagentConsumed = false;        // Is the item deleted from the players inventory upon activation
        public int reagent2Required = 0;        // Is an item required to activate this ability, if so, which one (0: no, otherwise the id of the item)
        public int reagent2Count = 1;
        public bool reagent2Consumed = false;       // Is the item deleted from the players inventory upon activation
        public int reagent3Required = 0;        // Is an item required to activate this ability, if so, which one (0: no, otherwise the id of the item)
        public int reagent3Count = 1;
        public bool reagent3Consumed = false;       // Is the item deleted from the players inventory upon activation
        public bool consumeOnActivation = false;
        public bool consumeAllreagent = false;        // Is the item deleted from the players inventory upon activation
        public bool pulseConsumeAllreagent = false;        // Is the item deleted from the players inventory upon activation
        public int ammoUsed = 0;
        public int maxRange = 4;                // How far away the target can be (in metres)
        public int minRange = 0;                // How close the target can be (in metres)
        public int aoeRadius = 1;               // how wide of an area the ability hits (in metres)
        public float aoeAngle = 360f;
        public string aoeType = "None";
        public string aoePrefab = "";
        public string[] aoeTypeOptions = new string[] { "PlayerRadius", "TargetRadius","LocationRadius" };
        public string[] selfAoeTypeOptions = new string[] { "LocationRadius" };
        public string[] aoePredictionOptions = new string[] { "Realtime", "Predicted"};

        public string[] groupTypeOptions = new string[] { "PlayerRadius" };
        public bool reqTarget = true;
        public bool reqFacingTarget = false;
        public bool autoRotateToTarget = false;
        public bool castingInRun = false;
        public int relativePositionReq = 0;
        public string[] relativePositionOptions = new string[] { "None","In Front", "Beside","Behind" };
        public string targetType = "";          // Any: can hit anyone; Enemy: can only be used on hostile targets; Self: can only be used on the caster; Friendly: can be used on a friendly unit; FriendNotSelf: a friendly unit, but not the caster; Group: can only target someone in the casters group; AoE Enemy: multiple enemy targets; AoE Friendly: multiple friendly targets
        public int targetState = 1;             // 0: Dead; 1; // Alive
        public string[] targetStateOptions = new string[] { "Dead", "Alive" };
        public string speciesTargetReq = "";    // One of the mob species (e.g. Humanoid, Beast, Dragon, Elemental, Undead, None)
        public string specificTargetReq = "";   // Is there a specific target (such as a certain mob) that the ability can be used on; // probably best to hide this for now.
        public bool globalCooldown = true;      // Does this ability trigger the global cooldown
        public string cooldown1Type = "";       // The name of the individual cooldown this ability triggers
        public float cooldown1Duration = 0;         // How long the cooldown lasts
        public bool weaponCooldown = false;         // Does this ability trigger the weapon cooldown
        public bool startCooldownsOnActivation = false; // Do the cooldowns get started when the player activates the ability
        public int activationEffect1 = 0;       // The id of the effect this ability applies
        public string activationTarget1 = "";   // Is this effect applied to the caster (true) or the target (false)
        public string[] activationTargetOptions = new string[] { "target", "caster" };
        public int activationEffect2 = 0;       // The id of the second effect this ability applies (optional)
        public string activationTarget2 = "";   // Is this effect applied to the caster (true) or the target (false)
       // public string[] activationTarget2Options = new string[] { "target", "caster" };
        public int activationEffect3 = 0;       // The id of the third effect this ability applies (optional)
        public string activationTarget4 = "";   // Is this effect applied to the caster (true) or the target (false)
        public int activationEffect4 = 0;       // The id of the third effect this ability applies (optional)
        public string activationTarget5 = "";   // Is this effect applied to the caster (true) or the target (false)
        public int activationEffect5 = 0;       // The id of the third effect this ability applies (optional)
        public string activationTarget6 = "";   // Is this effect applied to the caster (true) or the target (false)
        public int activationEffect6 = 0;       // The id of the third effect this ability applies (optional)
        public string activationTarget3 = "";   // Is this effect applied to the caster (true) or the target (false)
                                                // public string[] activationTarget3Options = new string[] { "target", "caster" };
     //   public string aoeCoordEffect = "";        // The name of the coordinated effect this ability activated (optional)
        public string coordEffect1 = "";        // The name of the coordinated effect this ability activated (optional)
        public string coordEffect1Event = "";   // When the coordinated effect is activated, can be: activating, activated, initializing, channelling, interrupted, failed
        public string[] coordEffect1EventOptions = new string[] {"activating", "activated", "channelling", "completed" ,/* "initializing",/* "channelling", */"interrupted", "failed" };
        public string coordEffect2 = "";        // The name of the second coordinated effect this ability activated (optional)
        public string coordEffect2Event = "";   // When the second coordinated effect is activated, can be: activating, activated, initializing, channelling, interrupted, failed
        public string coordEffect3 = "";        // The name of the second coordinated effect this ability activated (optional)
        public string coordEffect3Event = "";   // When the second coordinated effect is activated, can be: activating, activated, initializing, channelling, interrupted, failed
        public string coordEffect4 = "";        // The name of the second coordinated effect this ability activated (optional)
        public string coordEffect4Event = "";   // When the second coordinated effect is activated, can be: activating, activated, initializing, channelling, interrupted, failed
        public string coordEffect5 = "";        // The name of the second coordinated effect this ability activated (optional)
        public string coordEffect5Event = "";   // When the second coordinated effect is activated, can be: activating, activated, initializing, channelling, interrupted, failed
        public string[] coordEffect2EventOptions = new string[] { "activating", "activated", "channelling", "completed", /* "initializing",/* "channelling", */"interrupted", "failed" };
        public string[] coordEffect3EventOptions = new string[] { "activating", "activated", "channelling", "completed", /* "initializing",/* "channelling", */"interrupted", "failed" };
        public string tooltip = "";
        public float chance = 1f;
        public int exp = 0;
        public int channelling_pulse_num = 1;
        public float channelling_pulse_time = 1;
        public float activationDelay = 0f;
        public bool channelling = false;
      //  public int channelling_cost = 0;
        public bool channelling_in_run = false;

        public int pulseCost = 0;          // The value of the specified resource required to activate the ability
        public float pulseCostPercentage = 0;          // The value of the specified resource required to activate the ability
        public string pulseCostType = "";  // What resource this ability requires to activate (either mana or health)
        public int pulseCasterEffectRequired = 0;    // Does the caster require an effect on them to activate the ability (generally set to 0: no, otherwise the id of the effect required)
        public bool pulseCasterEffectConsumed = false;   // Is the effect removed from the caster
        public int pulseTargetEffectRequired = 0;    // Does the targetrequire an effect on them for the ability to activate (generally set to 0: no, otherwise the id of the effect required)
        public bool pulseTargetEffectConsumed;   // Is the effect removed from the target
        public int pulseReagentRequired = 0;         // Is an item required to activate this ability, if so, which one (0: no, otherwise the id of the item)
        public int pulseReagentCount = 1;
        public bool pulseReagentConsumed = false;        // Is the item deleted from the players inventory upon activation
        public int pulseReagent2Required = 0;        // Is an item required to activate this ability, if so, which one (0: no, otherwise the id of the item)
        public int pulseReagent2Count = 1;
        public bool pulseReagent2Consumed = false;       // Is the item deleted from the players inventory upon activation
        public int pulseReagent3Required = 0;        // Is an item required to activate this ability, if so, which one (0: no, otherwise the id of the item)
        public int pulseReagent3Count = 1;
        public bool pulseReagent3Consumed = false;       // Is the item deleted from the players inventory upon activation
        public int pulseAmmoUsed = 0;

        public bool skipChecks = false;
        public bool stealthReduction = false;
        public bool interruptible = false;
        public float interruption_chance = 0f;
        public bool toggle = false;
        public List<int> tags = new List<int>();
        public List<string> tagSearch = new List<string>();
        public int tagToDisable = -1;
        public string tagToDisableSearch = "";
        public int maxCountWithTag = 1;
        public float speed = 1;
        public float chunk_length = 1;
        public int aoePrediction = 0;
        public string[] aoeCountOptions = new string[] {"Unlimited", "First", "Random" };
        public int aoeCountTargets = 5;
        public int aoeCountType = 0;

        public AbilitiesData()
        {
            // Database fields
            fields = new Dictionary<string, string>() {
        {"name", "string"},
        {"icon", "string"},
        {"abilityType", "string"},
        {"skill", "int"},
        {"passive", "bool"},
        {"activationCost", "int"},
        {"activationCostPercentage", "float"},
        {"activationCostType", "string"},
        {"activationLength", "float"},
        {"activationAnimation", "string"},
        {"activationParticles", "string"},
        {"casterEffectRequired", "int"},
        {"casterEffectConsumed", "bool"},
        {"targetEffectRequired", "int"},
        {"targetEffectConsumed", "bool"},
        {"weaponRequired", "string"},
        {"reagentRequired", "int"},
        {"reagentCount", "int"},
        {"reagentConsumed", "bool"},
        {"reagent2Required", "int"},
        {"reagent2Count", "int"},
        {"reagent2Consumed", "bool"},
        {"reagent3Required", "int"},
        {"reagent3Count", "int"},
        {"reagent3Consumed", "bool"},
        {"ammoUsed", "int"},
        {"maxRange", "int"},
        {"minRange", "int"},
        {"aoeRadius", "int"},
        {"aoeAngle", "float"},
        {"aoeType", "string"},
        {"reqTarget", "bool"},
        {"reqFacingTarget", "bool"},
        {"autoRotateToTarget", "bool"},
        {"castingInRun", "bool"},
        {"relativePositionReq", "int"},
        {"targetType", "string"},
        {"targetState", "int"},
        {"speciesTargetReq", "string"},
        {"specificTargetReq", "string"},
        {"globalCooldown", "bool"},
        {"cooldown1Type", "string"},
        {"cooldown1Duration", "float"},
        {"weaponCooldown", "bool"},
        {"startCooldownsOnActivation", "bool"},
        {"activationEffect1", "int"},
        {"activationTarget1", "string"},
        {"activationEffect2", "int"},
        {"activationTarget2", "string"},
        {"activationEffect3", "int"},
        {"activationTarget3", "string"},
        {"activationEffect4", "int"},
        {"activationTarget4", "string"},
        {"activationEffect5", "int"},
        {"activationTarget5", "string"},
        {"activationEffect6", "int"},
        {"activationTarget6", "string"},
        {"coordEffect1event", "string"},
        {"coordEffect1", "string"},
        {"coordEffect2event", "string"},
        {"coordEffect2", "string"},
        {"coordEffect3event", "string"},
        {"coordEffect3", "string"},
        {"coordEffect4event", "string"},
        {"coordEffect4", "string"},
        {"coordEffect5event", "string"},
        {"coordEffect5", "string"},
        {"tooltip", "string"},
        {"chance","float" },
        {"exp","int" },
        {"consumeOnActivation", "bool"},
   //  {"aoeCoordEffect", "string"},
        {"aoePrefab", "string"},
        {"icon2", "string"},
        {"channelling_pulse_time","float" },
        {"channelling_pulse_num","int" },
    //     {"channelling_cost","int" },
        {"channelling","bool" },
        {"activationDelay","float" },
        {"channelling_in_run", "bool"},

        {"pulseCost", "int"},
        {"pulseCostPercentage", "float"},
        {"pulseCostType", "string"},
        {"pulseCasterEffectRequired", "int"},
        {"pulseCasterEffectConsumed", "bool"},
        {"pulseTargetEffectRequired", "int"},
        {"pulseTargetEffectConsumed", "bool"},
        {"pulseReagentRequired", "int"},
        {"pulseReagentCount", "int"},
        {"pulseReagentConsumed", "bool"},
        {"pulseReagent2Required", "int"},
        {"pulseReagent2Count", "int"},
        {"pulseReagent2Consumed", "bool"},
        {"pulseReagent3Required", "int"},
        {"pulseReagent3Count", "int"},
        {"pulseReagent3Consumed", "bool"},
        {"pulseAmmoUsed", "int"},
        {"skipChecks","bool" },
        {"stealth_reduce","bool" },
        {"interruptible","bool" },
        {"interruption_chance","float" },
        {"toggle","bool" },
        {"tags", "string"},
        {"tag_count", "int"},
        {"tag_disable", "int"},
        {"speed","float" },
        {"chunk_length","float" },
          {"prediction", "int"},
                {"aoe_target_count_type", "int" },
                {"aoe_target_count", "int" },
            };
        }

        public AbilitiesData Clone()
        {
            return (AbilitiesData)this.MemberwiseClone();
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
                case "abilityType":
                    return abilityType;
                case "skill":
                    return skill.ToString();
                case "passive":
                    return passive.ToString();
                case "activationCost":
                    return activationCost.ToString();
                case "activationCostType":
                    return activationCostType;
                case "activationLength":
                    return activationLength.ToString();
                case "activationAnimation":
                    return activationAnimation;
                case "activationParticles":
                    return activationParticles;
                case "casterEffectRequired":
                    return casterEffectRequired.ToString();
                case "casterEffectConsumed":
                    return casterEffectConsumed.ToString();
                case "targetEffectRequired":
                    return targetEffectRequired.ToString();
                case "targetEffectConsumed":
                    return targetEffectConsumed.ToString();
                case "weaponRequired":
                    return weaponRequired;
                case "reagentRequired":
                    return reagentRequired.ToString();
                case "reagentCount":
                    return reagentCount.ToString();
                case "reagentConsumed":
                    return reagentConsumed.ToString();
                case "reagent2Required":
                    return reagent2Required.ToString();
                case "reagent2Count":
                    return reagent2Count.ToString();
                case "reagent2Consumed":
                    return reagent2Consumed.ToString();
                case "reagent3Required":
                    return reagent3Required.ToString();
                case "reagent3Count":
                    return reagent3Count.ToString();
                case "reagent3Consumed":
                    return reagent3Consumed.ToString();
                case "ammoUsed":
                    return ammoUsed.ToString();
                case "maxRange":
                    return maxRange.ToString();
                case "minRange":
                    return minRange.ToString();
                case "aoeRadius":
                    return aoeRadius.ToString();
                case "aoeAngle":
                    return aoeAngle.ToString();
                case "aoeType":
                    return aoeType;
                case "reqTarget":
                    return reqTarget.ToString();
                case "reqFacingTarget":
                    return reqFacingTarget.ToString();
                case "autoRotateToTarget":
                    return autoRotateToTarget.ToString();
                case "castingInRun":
                    return castingInRun.ToString();
                case "relativePositionReq":
                    return relativePositionReq.ToString();
                case "targetType":
                    return targetType;
                case "targetState":
                    return targetState.ToString();
                case "speciesTargetReq":
                    return speciesTargetReq;
                case "specificTargetReq":
                    return specificTargetReq;
                case "globalCooldown":
                    return globalCooldown.ToString();
                case "cooldown1Type":
                    return cooldown1Type;
                case "cooldown1Duration":
                    return cooldown1Duration.ToString();
                case "weaponCooldown":
                    return weaponCooldown.ToString();
                case "startCooldownsOnActivation":
                    return startCooldownsOnActivation.ToString();
                case "activationEffect1":
                    return activationEffect1.ToString();
                case "activationTarget1":
                    return activationTarget1;
                case "activationEffect2":
                    return activationEffect2.ToString();
                case "activationTarget2":
                    return activationTarget2;
                case "activationEffect3":
                    return activationEffect3.ToString();
                case "activationTarget3":
                    return activationTarget3.ToString();
                case "activationEffect4":
                    return activationEffect4.ToString();
                case "activationTarget4":
                    return activationTarget4.ToString();
                case "activationEffect5":
                    return activationEffect5.ToString();
                case "activationTarget5":
                    return activationTarget5.ToString();
                case "activationEffect6":
                    return activationEffect6.ToString();
                case "activationTarget6":
                    return activationTarget6.ToString();
                case "coordEffect1event":
                    return coordEffect1Event;
                case "coordEffect1":
                    return coordEffect1;
                case "coordEffect2event":
                    return coordEffect2Event;
                case "coordEffect2":
                    return coordEffect2;
                case "coordEffect3event":
                    return coordEffect3Event;
                case "coordEffect3":
                    return coordEffect3;
                case "coordEffect4event":
                    return coordEffect4Event;
                case "coordEffect4":
                    return coordEffect4;
                case "coordEffect5event":
                    return coordEffect5Event;
                case "coordEffect5":
                    return coordEffect5;
                case "tooltip":
                    return tooltip;
                case "chance":
                    return chance.ToString();
                case "exp":
                    return exp.ToString();
                case "consumeOnActivation":
                    return consumeOnActivation.ToString();
              //  case "aoeCoordEffect":
              //      return aoeCoordEffect;
                case "channelling_pulse_num":
                    return channelling_pulse_num.ToString();
                case "channelling_pulse_time":
                    return channelling_pulse_time.ToString();
                case "channelling":
                    return channelling.ToString();
                //case "channelling_cost":
                 //   return channelling_cost.ToString();
                case "activationDelay":
                    return activationDelay.ToString();
                case "channelling_in_run":
                    return channelling_in_run.ToString();
                case "aoePrefab":
                    return aoePrefab;
                case "pulseCost":
                    return pulseCost.ToString();
                case "pulseCostType":
                    return pulseCostType;
                case "pulseCasterEffectRequired":
                    return pulseCasterEffectRequired.ToString();
                case "pulseCasterEffectConsumed":
                    return pulseCasterEffectConsumed.ToString();
                case "pulseTargetEffectRequired":
                    return pulseTargetEffectRequired.ToString();
                case "pulseTargetEffectConsumed":
                    return pulseTargetEffectConsumed.ToString();
                case "pulseReagentRequired":
                    return pulseReagentRequired.ToString();
                case "pulseReagentCount":
                    return pulseReagentCount.ToString();
                case "pulseReagentConsumed":
                    return pulseReagentConsumed.ToString();
                case "pulseReagent2Required":
                    return pulseReagent2Required.ToString();
                case "pulseReagent2Count":
                    return pulseReagent2Count.ToString();
                case "pulseReagent2Consumed":
                    return pulseReagent2Consumed.ToString();
                case "pulseReagent3Required":
                    return pulseReagent3Required.ToString();
                case "pulseReagent3Count":
                    return pulseReagent3Count.ToString();
                case "pulseReagent3Consumed":
                    return pulseReagent3Consumed.ToString();
                case "pulseAmmoUsed":
                    return pulseAmmoUsed.ToString();
                case "skipChecks":
                    return skipChecks.ToString();
                case "stealth_reduce":
                    return stealthReduction.ToString();
                case "pulseCostPercentage":
                    return pulseCostPercentage.ToString();
                case "activationCostPercentage":
                    return activationCostPercentage.ToString();
                case "interruptible":
                    return interruptible.ToString();
                case "interruption_chance":
                    return interruption_chance.ToString();
                case "toggle":
                    return toggle.ToString();
                case "tags":
                    return string.Join(";", tags.ConvertAll(i => i.ToString()).ToArray());
                case "tag_disable":
                    return tagToDisable.ToString();
                case "tag_count":
                    return maxCountWithTag.ToString();
                case "speed":
                    return speed.ToString();
                case "chunk_length":
                    return chunk_length.ToString();
                case "prediction":
                    return aoePrediction.ToString();
                case "aoe_target_count_type":
                    return aoeCountType.ToString();
                case "aoe_target_count":
                    return aoeCountTargets.ToString();
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