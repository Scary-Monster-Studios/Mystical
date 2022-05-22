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
    public class ServerVipEffects : ServerEffectType
    {

        public new string effectType = "Vip";
        public new string[] effectTypeOptions = new string[] { "VipEffect" };

        // Use this for initialization
        public ServerVipEffects()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
          //  pos.y += ImagePack.fieldHeight*2;
            editingDisplay.intValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Extend Time (minutes)") + ":", editingDisplay.intValue1);
            // pos.x += pos.width;
            pos.y += ImagePack.fieldHeight;
            editingDisplay.intValue2 = ImagePack.DrawField(pos, Lang.GetTranslate("Points") + ":", editingDisplay.intValue2);
           // pos.x -= pos.width;
            pos.y += ImagePack.fieldHeight;
            showSkillModFields = true;
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