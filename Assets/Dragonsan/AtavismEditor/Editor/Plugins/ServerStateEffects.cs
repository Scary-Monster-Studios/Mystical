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
    public class ServerStateEffects : ServerEffectType
    {

        public new string effectType = "State";
        public new string[] effectTypeOptions = new string[] { "StateEffect" };
        public string[] stateOptions = new string[] { "~ none ~" };

        // Use this for initialization
        public ServerStateEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
            stateOptions = ServerOptionChoices.LoadAtavismChoiceOptions("State", false);
        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            editingDisplay.stringValue1 = ImagePack.DrawSelector(pos, Lang.GetTranslate("State")+":", editingDisplay.stringValue1, stateOptions);
            pos.y += ImagePack.fieldHeight;
            showSkillModFields = false;
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