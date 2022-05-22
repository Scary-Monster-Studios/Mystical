using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EasyBuildSystem.Features.Scripts.Core.Base.Builder;
using EasyBuildSystem.Features.Scripts.Core.Base.Builder.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using TMPro;

namespace Atavism
{

    public class UGUIBuildObject : MonoBehaviour
    {

        public Button button;
        public Text nameText;
        public TextMeshProUGUI TMPNameText;
        public Text requirementsText;
        public TextMeshProUGUI TMPRequirementsText;
        public TextMeshProUGUI TMPCategoryText;
        AtavismBuildObjectTemplate template;

        // Use this for initialization
        void Start()
        {

        }

        public void UpdateBuildObjectInfo(AtavismBuildObjectTemplate template)
        {
            this.template = template;
            button.GetComponent<Image>().sprite = template.icon;
            if (nameText != null)
                nameText.text = template.buildObjectName;
            if (TMPNameText != null)
                TMPNameText.text = template.buildObjectName;
            if (TMPCategoryText != null)
            {
                TMPCategoryText.text = WorldBuilder.Instance.GetBuildingCategory(template.category);
            }
            // template.buildTimeReq;
            int days = 0;
            int hour = 0;
            int minute = 0;
            int secound = 0;
            if (template.buildTimeReq > 24 * 3600L)
                days = (int) (template.buildTimeReq / (3600F * 24));
            if (template.buildTimeReq > 3600L)
                hour = (int) ((template.buildTimeReq - days * 24 * 3600) / 3600F);
            if (template.buildTimeReq - hour * 3600 > 60)
                minute = (int) (template.buildTimeReq - days * 24 * 3600 - hour * 3600) / 60;
            secound = (int) (template.buildTimeReq - days * 24 * 3600 - hour * 3600 - minute * 60);
        //    float rest = template.buildTimeReq - days * 24 * 3600 - hour * 3600 - minute * 60 - secound;
        
            string outTime = "";

            if (days > 0)
            {
#if AT_I2LOC_PRESET
                if (days > 1)
                {
                    outTime += days + " "+I2.Loc.LocalizationManager.GetTranslation("days")+" ";
                }
                else
                {
                    outTime += days+ " "+I2.Loc.LocalizationManager.GetTranslation("day")+" ";
                }
#else
                if (days > 1)
                {
                    outTime += days + " days ";
                }
                else
                {
                    outTime += days+" day ";
                }
#endif
            }
                
            if (hour > 0)
            {
                outTime += hour + " h ";
            }

            if (minute > 0)
            {
                outTime += minute + " m ";
            }

            if (secound > 0)
            {
                outTime += secound+" s ";
            }
            string requirements = "";
#if AT_I2LOC_PRESET
            requirements += I2.Loc.LocalizationManager.GetTranslation("Build Time")+": "+outTime+"\n";
            requirements += I2.Loc.LocalizationManager.GetTranslation("Requires")+": ";

#else
            requirements += "Build Time: " + outTime + "\n";
            requirements += "Requires: ";
#endif

            for (int i = 0; i < template.itemsReq.Count; i++)
            {

                AtavismInventoryItem item = Inventory.Instance.GetItemByTemplateID(template.itemsReq[i]);
                if (item != null)
                    requirements += "  " + item.name + " x" + template.itemsReqCount[i] + ";  ";
            }

            if (template.skill > 0)
            {
                Skill skill = Skills.Instance.GetSkillByID(template.skill);
                if (skill != null)
                {
                    if (skill.CurrentLevel >= template.skillLevelReq)
                    {
#if AT_I2LOC_PRESET
                        requirements += I2.Loc.LocalizationManager.GetTranslation("Skill")+" " + I2.Loc.LocalizationManager.GetTranslation(skill.skillname)+" " + I2.Loc.LocalizationManager.GetTranslation("level")+" " + template.skillLevelReq;
#else
                        requirements += "Skill " + skill.skillname + " level " + template.skillLevelReq;
#endif
                    }
                    else
                    {
#if AT_I2LOC_PRESET
                       requirements += "<color=#ff0000ff>"+I2.Loc.LocalizationManager.GetTranslation("Skill")+" " + I2.Loc.LocalizationManager.GetTranslation(skill.skillname) + " "+I2.Loc.LocalizationManager.GetTranslation("level")+" " + template.skillLevelReq + "</color>";
#else
                        requirements += "<color=#ff0000ff>Skill " + skill.skillname + " level " + template.skillLevelReq + "</color>";
#endif
                    }
                }
                else
                {
                    Debug.LogError("Building Object Skill " + template.skill + " can't be found");
                }
            }

            if (requirementsText != null)
                requirementsText.text = requirements;
            if (TMPRequirementsText != null)
                TMPRequirementsText.text = requirements;
        }

        public void BuildObjectClicked()
        {
            WorldBuilder.Instance.BuildingState = WorldBuildingState.SelectItem;
            
            if (BuildManager.Instance.Pieces.Count != 0)
            {
                if (BuilderBehaviour.Instance != null)
                {
                    BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
                    BuilderBehaviour.Instance.ChangeMode(BuildMode.Placement);
                    bool f = false;
                    foreach (var piece in BuildManager.Instance.Pieces)
                    {
                        if (piece != null)
                        {
                            AtavismLogger.LogDebugMessage("BuildObjectClicked piece"+piece.Id+" " + template.id + " piece.BuildObjDefId=" + piece.BuildObjDefId);
                            if (piece.BuildObjDefId == template.id)
                            {
                                AtavismLogger.LogDebugMessage("BuildObjectClicked found " + template.id + " piece.BuildObjDefId=" + piece.BuildObjDefId+" "+piece.name);

                                BuilderBehaviour.Instance.SelectPrefab(piece);
                                f = true;
                            }
                        }
                    }

                    AtavismLogger.LogDebugMessage("BuildObjectClicked " + template.id + " found=" + f);
                    BuilderBehaviour.Instance.ChangeMode(BuildMode.Placement);
                }
                else
                {
                    WorldBuilder.Instance.StartPlaceBuildObject(template.id);
                }
            }
           //     WorldBuilder.Instance.StartPlaceBuildObject(template.id);
           
        }
    }
}