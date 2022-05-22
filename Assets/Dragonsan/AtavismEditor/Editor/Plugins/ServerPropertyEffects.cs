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
    public class ServerPropertyEffects : ServerEffectType
    {

        public new string effectType = "Property";
        public new string[] effectTypeOptions = new string[] { "PropertyEffect" };
        public string[] propertyTypeOptions = new string[] { "String", "Integer", "Stat" };

        // Use this for initialization
        public ServerPropertyEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            editingDisplay.stringValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Property")+":", editingDisplay.stringValue1);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.stringValue2 = ImagePack.DrawField(pos, Lang.GetTranslate("Property Value")+":", editingDisplay.stringValue2);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.stringValue3 = ImagePack.DrawField(pos, Lang.GetTranslate("Property Default")+":", editingDisplay.stringValue3);
            pos.y += ImagePack.fieldHeight;
            showTimeFields = true;
            showSkillModFields = true;
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