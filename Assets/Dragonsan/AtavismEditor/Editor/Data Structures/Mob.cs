using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class MobStat : DataStructure
    {
        // General Parameters
        public int mobTemplate;
        public string stat = "";
        public int value = 0;

        public MobStat()
        {
            // Database fields
            fields = new Dictionary<string, string>() {
            {"mobTemplate", "int"},
            {"stat", "string"},
            {"value", "int"}
        };
        }

        public MobStat Clone()
        {
            return (MobStat)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "mobTemplate":
                    return mobTemplate.ToString();
                case "stat":
                    return stat.ToString();
                case "value":
                    return value.ToString();
            }
            return "";
        }
    }

    public class Mob : DataStructure
    {
        public string displayName = "";
        // General Parameters
        public int category = 0;            // Always set to 0 at the current time
        public string subTitle = "";        // Tag that goes under a mobs name (i.e Weapon Vendor)
        public int mobType = 0;         // 0 = normal, -1 = untargetable/non attackable object (such as a chest), 1 = boss
        public string species = "~ none ~";     // Humanoid, Beast, Undead, Elemental, Dragon, Unknown
        public string subspecies = "";
        public int faction = 0;         // The faction the mob belongs to, this will link to the faction table
                                        // Appearance Parameters
                                        //public List<GameObject> displays;		// Name of the first prefab used for display
        public string display1 = "";        // Name of the first prefab used for display
        public string display2 = "";        // Name of the second prefab used for display
        public string display3 = "";        // Name of the third prefab used for display
        public string display4 = "";        // Name of the fourth prefab used for display
        public float scale = 1.0f;          // How big the mobs model is scaled
        public int hitBox = 1;              // How far away the mob can be hit (generally set to 1)
        public int baseAnimationState = 1;  // 1 = standing, 2 = swimming, 3 = flying
        public string[] baseAnimationStateOptions = new string[] { "standing", "swimming", "flying" };
        public float speedWalk = 3f;        // How fast the mob walks (should be about 3)
        public float speedRun = 7f;         // How fast the mob runs (generally occurs in combat, should be around 7)
        public int primaryWeapon = 0;       // The weapon the mob has in their main hand (this links to the inventory table)
        public int secondaryWeapon = 0;     // The weapon (or shield) the mob has in the off hand (this links to the inventory table) (optional)
                                            // Combat Parameters
        public bool attackable = true;      // Is the mob attackable?
        public int minLevel = 1;            // The minimum level of the mob
        public int maxLevel = 1;            // The maximum level of the mob
        public int minDamage = 1;           // The minimum base damage the mob deals
        public int maxDamage = 1;           // The maximum base damage the mob deals
        public string damageType = "~ none ~";      // Can be either Slash, Crush or Pierce
        public int autoAttack = 0;
        public float attackDistance = 0;
        public int ability0 = -1;
        public string abilityStatReq0 = "";
        public int abilityStatPercent0 = 100;
        public int ability1 = -1;
        public string abilityStatReq1 = "";
        public int abilityStatPercent1 = 100;
        public int ability2 = -1;
        public string abilityStatReq2 = "";
        public int abilityStatPercent2 = 100;

        public float attackSpeed = 1.7f;        // How long the mob waits between attacks (in seconds)
        public string questCategory = "";
        public string specialUse = "";

        public int skinningLootTable = -1;
        public int skinningLevelReq = 0;
        public int skinningLevelMax = 0;
        public int skinningSkillId = -1;
        public int skinningSkillExp = 0;
        public string skinningWeaponReq = "";
        public float skinningHarvestTime = 2f;
        public int aggro_range = 17;
        public int linked_aggro_range = 0;
        public bool linked_aggro_send = false;
        public bool linked_aggro_get = false;
        public int chasing_distance = 60;

        public int exp = 100;
        public int addexplev = 10;

        public List<MobStat> stats = new List<MobStat>();
        public List<MobStat> statsRestore = new List<MobStat>();
        public List<int> mobStatToBeDeleted = new List<int>();

        public Mob()
        {
            id = -1;
            Name = "name";
            // Database fields
            fields = new Dictionary<string, string>() {
        {"category", "int"},
        {"name", "string"},
        {"displayName", "string"},
        {"subTitle", "string"},
        {"mobType", "int"},
        {"display1", "string"},
        {"display2", "string"},
        {"display3", "string"},
        {"display4", "string"},
        {"scale", "float"},
        {"hitbox", "int"},
        {"baseAnimationState", "int"},
        {"faction", "int"},
        {"attackable", "bool"},
        {"minLevel", "int"},
        {"maxLevel", "int"},
        {"species", "string"},
        {"subSpecies", "string"},
        {"questCategory", "string"},
        {"specialUse", "string"},
        {"speed_walk", "float"},
        {"speed_run", "float"},
        {"minDmg", "int"},
        {"maxDmg", "int"},
        {"attackSpeed", "float"},
        {"dmgType", "string"},
        {"primaryWeapon",  "int"},
        {"secondaryWeapon",  "int"},
        {"autoAttack",  "int"},
        {"attackDistance",  "float"},
        {"ability0",  "int"},
        {"abilityStatReq0",  "string"},
        {"abilityStatPercent0",  "int"},
        {"ability1",  "int"},
        {"abilityStatReq1",  "string"},
        {"abilityStatPercent1",  "int"},
        {"ability2",  "int"},
        {"abilityStatReq2",  "string"},
        {"abilityStatPercent2",  "int"},
        {"skinningLootTable",  "int"},
        {"skinningLevelReq",  "int"},
        {"skinningLevelMax",  "int"},
        {"skinningSkillId",  "int"},
        {"skinningSkillExp",  "int"},
        {"skinningWeaponReq",  "string"},
        {"skinningHarvestTime",  "float"},
        {"exp",  "int"},
        {"addExplev",  "int"},
        {"aggro_radius",  "int"},
        {"chasing_distance",  "int"},
        {"link_aggro_range",  "int"},
        {"send_link_aggro", "bool"},
        {"get_link_aggro", "bool"},
       };

        }

        public Mob Clone()
        {
            return (Mob)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "id":
                    return id.ToString();
                case "category":
                    return category.ToString();
                case "name":
                    return Name;
                case "displayName":
                    return displayName;
                case "subTitle":
                    return subTitle;
                case "mobType":
                    return mobType.ToString();
                case "species":
                    return species;
                case "subSpecies":
                    return subspecies;
                case "faction":
                    return faction.ToString();
                case "display1":
                    return display1;
                case "display2":
                    return display2;
                case "display3":
                    return display3;
                case "display4":
                    return display4;
                case "scale":
                    return scale.ToString();
                case "hitbox":
                    return hitBox.ToString();
                case "baseAnimationState":
                    return baseAnimationState.ToString();
                case "speed_walk":
                    return speedWalk.ToString();
                case "speed_run":
                    return speedRun.ToString();
                case "primaryWeapon":
                    return primaryWeapon.ToString();
                case "secondaryWeapon":
                    return secondaryWeapon.ToString();
                case "attackable":
                    return attackable.ToString();
                case "minLevel":
                    return minLevel.ToString();
                case "maxLevel":
                    return maxLevel.ToString();
                case "minDmg":
                    return minDamage.ToString();
                case "maxDmg":
                    return maxDamage.ToString();
                case "dmgType":
                    return damageType;
                case "attackSpeed":
                    return attackSpeed.ToString();
                case "autoAttack":
                    return autoAttack.ToString();
                case "attackDistance":
                    return attackDistance.ToString();
                case "ability0":
                    return ability0.ToString();
                case "abilityStatReq0":
                    return abilityStatReq0;
                case "abilityStatPercent0":
                    return abilityStatPercent0.ToString();
                case "ability1":
                    return ability1.ToString();
                case "abilityStatReq1":
                    return abilityStatReq1;
                case "abilityStatPercent1":
                    return abilityStatPercent1.ToString();
                case "ability2":
                    return ability2.ToString();
                case "abilityStatReq2":
                    return abilityStatReq2;
                case "abilityStatPercent2":
                    return abilityStatPercent2.ToString();
                case "skinningLootTable":
                    return skinningLootTable.ToString();
                case "skinningLevelReq":
                    return skinningLevelReq.ToString();
                case "skinningLevelMax":
                    return skinningLevelMax.ToString();
                case "skinningSkillId":
                    return skinningSkillId.ToString();
                case "skinningSkillExp":
                    return skinningSkillExp.ToString();
                case "skinningWeaponReq":
                    if (skinningWeaponReq.Equals("~ none ~"))
                        return "";
                    return skinningWeaponReq;
                case "skinningHarvestTime":
                    return skinningHarvestTime.ToString();
                case "addExplev":
                    return addexplev.ToString();
                case "exp":
                    return exp.ToString();
                case "aggro_radius":
                    return aggro_range.ToString();
                case "chasing_distance":
                    return chasing_distance.ToString();
                case "link_aggro_range":
                    return linked_aggro_range.ToString();
                case "get_link_aggro":
                    return linked_aggro_get.ToString();
                case "send_link_aggro":
                    return linked_aggro_send.ToString();
            }
            return "";
        }
    }
}