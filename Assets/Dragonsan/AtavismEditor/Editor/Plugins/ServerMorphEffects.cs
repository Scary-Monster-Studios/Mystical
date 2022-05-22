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
    public class ServerMorphEffects : ServerEffectType
    {

        public new string effectType = "Morph";
        public new string[] effectTypeOptions = new string[] { "MorphEffect" };
        public string[] morphTypeOptions = new string[] { "Ground", "Swimming", "Flying" };

        // Use this for initialization
        public ServerMorphEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            editingDisplay.stringValue1 = ImagePack.DrawGameObject(pos, Lang.GetTranslate("Model")+": ", editingDisplay.stringValue1, 0.75f);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.intValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Switch to ActionBar")+": ", editingDisplay.intValue1);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.intValue2 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Morph Type"), editingDisplay.intValue2, morphTypeOptions);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.boolValue1 = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Remove existing morphs")+":", editingDisplay.boolValue1);
            pos.y += ImagePack.fieldHeight;
            showTimeFields = true;
            showSkillModFields = false;
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