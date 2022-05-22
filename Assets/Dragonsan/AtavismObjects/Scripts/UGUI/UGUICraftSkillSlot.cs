using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Atavism
{
    public class UGUICraftSkillSlot : MonoBehaviour
    {
        [SerializeField] Text skillName;
        [SerializeField] TextMeshProUGUI TMPSkillName;
        [SerializeField] Text skillLevel;
        [SerializeField] TextMeshProUGUI TMPSkillLevel;
        [SerializeField] Image skillFill;
    //    [SerializeField] int perLevel = 1000;


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void UpdateDisplay(Skill skill)
        {
            //  int level = (int)skill.CurrentLevel / perLevel;
            if (skill.expMax == 0 && skill.exp == 0)
            {
                if (skillName != null)
                {
#if AT_I2LOC_PRESET
                    skillName.text = I2.Loc.LocalizationManager.GetTranslation(skill.skillname) ;
#else
                    skillName.text = skill.skillname ;
#endif
                }
                if (TMPSkillName != null)
                {
#if AT_I2LOC_PRESET
                    TMPSkillName.text = I2.Loc.LocalizationManager.GetTranslation(skill.skillname) ;
#else
                    TMPSkillName.text = skill.skillname ;
#endif
                }
            }
            else
            {
                if (skillName != null)
                {
#if AT_I2LOC_PRESET
                    skillName.text = I2.Loc.LocalizationManager.GetTranslation(skill.skillname) +" "+ (skill.CurrentLevel);
#else
                    skillName.text = skill.skillname + " "+ (skill.CurrentLevel);
#endif
                }
                if (TMPSkillName != null)
                {
#if AT_I2LOC_PRESET
                    TMPSkillName.text = I2.Loc.LocalizationManager.GetTranslation(skill.skillname) + " " + (skill.CurrentLevel);
#else
                    TMPSkillName.text = skill.skillname + " " +(skill.CurrentLevel);
#endif
                }
            }
            /*         if (skillLevel != null)
                         skillLevel.text = (skill.CurrentLevel - level * perLevel) + "/" + perLevel;
                     if (TMPSkillLevel != null)
                         TMPSkillLevel.text = (skill.CurrentLevel - level * perLevel) + "/" + perLevel;
                     if (skillFill != null)
                         skillFill.fillAmount = (float)((float)(skill.CurrentLevel - level * (float)perLevel) / (float)perLevel);
         */


            if (skill.expMax == 0 && skill.exp == 0)
            {
                if (skillLevel != null)
                    skillLevel.text = skill.CurrentLevel + "/" + skill.MaximumLevel;
                if (TMPSkillLevel != null)
                    TMPSkillLevel.text = skill.CurrentLevel + "/" + skill.MaximumLevel;
                if (skillFill != null)
                    skillFill.fillAmount = (float)(((float)skill.CurrentLevel) / (float)skill.MaximumLevel);
            }
            else
            {
                if (skillLevel != null)
                    skillLevel.text = skill.exp + "/" + skill.expMax;
                if (TMPSkillLevel != null)
                    TMPSkillLevel.text = skill.exp + "/" + skill.expMax;
                if (skillFill != null)
                    skillFill.fillAmount = (float)(((float)skill.exp) / (float)skill.expMax);
            }
        }
    }
}