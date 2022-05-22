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
    // Abstract base class for the effect type classes
    public abstract class ServerEffectType
    {

        protected string effectType;
        protected string[] effectTypeOptions;

        // Use this for initialization
        public ServerEffectType()
        {
        }

        public abstract void LoadOptions(EffectsData editingDisplay, bool newItem);

        // Edit or Create
        public abstract Rect DrawEditor(Rect box, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields);

        public abstract string EffectType { get; }

        public abstract string[] EffectTypeOptions { get; }

        public int GetOptionPosition(int id, int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                if (ids[i] == id)
                    return i;
            }
            return 0;
        }
    }
}