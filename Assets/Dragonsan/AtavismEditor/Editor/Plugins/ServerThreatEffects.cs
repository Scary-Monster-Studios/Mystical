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
    public class ServerThreatEffects : ServerEffectType
    {

        public new string effectType = "Threat";
        public new string[] effectTypeOptions = new string[] { "ThreatEffect" };

        // Use this for initialization
        public ServerThreatEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            editingDisplay.intValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Threat Amount")+":", editingDisplay.intValue1);
            pos.y += ImagePack.fieldHeight;

            showSkillModFields = false;
            showTimeFields = false;
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