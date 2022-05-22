using UnityEngine;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
namespace Atavism
{
    // Handles the Effects Configuration
    public class ServerStealthEffects : ServerEffectType
    {

        public new string effectType = "Stealth";
        public new string[] effectTypeOptions = new string[] { "StealthEffect" };

        public string[] statOptions = new string[] { "~ none ~" };

        // Use this for initialization
        public ServerStealthEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
           
        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            editingDisplay.boolValue2 = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Effect Stays after Logout")+":", editingDisplay.boolValue2);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.intValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Stealth")+":", editingDisplay.intValue1);
            pos.y += ImagePack.fieldHeight;
            showSkillModFields = true;
            showTimeFields = true;
            return pos;
        }

      
        public override string EffectType
        {
            get
            {
                return effectType;
            }
        }

        public override string[] EffectTypeOptions
        {
            get
            {
                return effectTypeOptions;
            }
        }
    }
}