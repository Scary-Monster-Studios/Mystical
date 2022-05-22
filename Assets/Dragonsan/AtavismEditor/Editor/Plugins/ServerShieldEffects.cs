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
    public class ServerShieldEffects : ServerEffectType
    {

        public new string effectType = "Shield";
        public new string[] effectTypeOptions = new string[] { "ShieldEffect" };

        public string[] statOptions = new string[] { "~ none ~" };
        Char delimiter = ';';
        List<int> tags = new List<int>();
        List<int> tags2 = new List<int>();
        List<int> tags3 = new List<int>();
        List<int> tags4 = new List<int>();
        List<int> tags5 = new List<int>();
        List<string> tagSearch = new List<string>();
        List<string> tagSearch2 = new List<string>();
        List<string> tagSearch3 = new List<string>();
        List<string> tagSearch4 = new List<string>();
        List<string> tagSearch5 = new List<string>();
        public int[] tagIds = new int[] { -1 };
        public string[] tagOptions = new string[] { "~ none ~" };

        // Use this for initialization
        public ServerShieldEffects()
        {
            tagSearch.Clear();
            tagSearch2.Clear();
            tagSearch3.Clear();
            tagSearch4.Clear();
            tagSearch5.Clear();
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
            ServerOptionChoices.LoadAtavismChoiceOptions("Effects Tags", false, out tagIds, out tagOptions);

        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            /* editingDisplay.boolValue2 = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Effect Stays after Logout")+":", editingDisplay.boolValue2);
             pos.y += ImagePack.fieldHeight;
             editingDisplay.intValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Stealth")+":", editingDisplay.intValue1);*/

            tags.Clear();
            if (editingDisplay.stringValue1.Length > 0)
            {
                string[] splitted = editingDisplay.stringValue1.Split(delimiter);
                for (int i = 0; i < splitted.Length; i++)
                {
                    tags.Add(int.Parse(splitted[i]));
                }
            }
            tags2.Clear();
            if (editingDisplay.stringValue2.Length > 0)
            {
                string[] splitted = editingDisplay.stringValue2.Split(delimiter);
                for (int i = 0; i < splitted.Length; i++)
                {
                    tags2.Add(int.Parse(splitted[i]));
                }
            }
            tags3.Clear();
            if (editingDisplay.stringValue3.Length > 0)
            {
                string[] splitted = editingDisplay.stringValue3.Split(delimiter);
                for (int i = 0; i < splitted.Length; i++)
                {
                    tags3.Add(int.Parse(splitted[i]));
                }
            }
            tags4.Clear();
            if (editingDisplay.stringValue4.Length > 0)
            {
                string[] splitted = editingDisplay.stringValue4.Split(delimiter);
                for (int i = 0; i < splitted.Length; i++)
                {
                    tags4.Add(int.Parse(splitted[i]));
                }
            }
           /* tags5.Clear();
            if (editingDisplay.stringValue5.Length > 0)
            {
                string[] splitted = editingDisplay.stringValue5.Split(delimiter);
                for (int i = 0; i < splitted.Length; i++)
                {
                    tags5.Add(int.Parse(splitted[i]));
                }
            }*/
            editingDisplay.intValue5 = ImagePack.DrawField(pos, Lang.GetTranslate("Shield Value"), editingDisplay.intValue5);
            pos.y += ImagePack.fieldHeight;
            editingDisplay.floatValue5 = ImagePack.DrawField(pos, Lang.GetTranslate("Hit Count"), (int)editingDisplay.floatValue5);
            pos.y += ImagePack.fieldHeight+1.5f;

            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Setting 1"));
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Tags"));
            for (int j = 0; j < tags.Count; j++)
            {
                pos.y += ImagePack.fieldHeight;
                int selectedTag = GetOptionPosition(tags[j], tagIds);
                if(tagSearch.Count<=j)
                    tagSearch.Add("");
                string search = tagSearch[j];
                selectedTag = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Tag ") + (j + 1) + ":", ref search, selectedTag, tagOptions);
                tagSearch[j] = search;
                tags[j] = tagIds[selectedTag];
                pos.x += pos.width;
                pos.y += ImagePack.fieldHeight;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Tag")))
                {
                    tags.RemoveAt(j);
                    tagSearch.RemoveAt(j);
                }
                pos.x -= pos.width;
            }
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Tag")))
            {
                tags.Add(0);
                tagSearch.Add("");
            }
            editingDisplay.stringValue1 = string.Join(";", tags.ConvertAll(i => i.ToString()).ToArray());
            pos.y += ImagePack.fieldHeight;
            if (!editingDisplay.boolValue1)
                editingDisplay.intValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Absorb Value"), editingDisplay.intValue1);
            else
                editingDisplay.intValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Reflect Value"), editingDisplay.intValue1);
            pos.x += pos.width;
            if(!editingDisplay.boolValue1)
                editingDisplay.floatValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Absorb %"), editingDisplay.floatValue1);
            else
                editingDisplay.floatValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Reflect %"), editingDisplay.floatValue1);
            pos.y += ImagePack.fieldHeight;
            pos.x -= pos.width;
            editingDisplay.boolValue1 = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Reflect ") + "?", editingDisplay.boolValue1);
            pos.y += ImagePack.fieldHeight;

            ////////////////////////////////////////////
            pos.y += ImagePack.fieldHeight;

            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Setting 2"));
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Tags"));
            for (int j = 0; j < tags2.Count; j++)
            {
                pos.y += ImagePack.fieldHeight;
                int selectedTag = GetOptionPosition(tags2[j], tagIds);
                if (tagSearch2.Count <= j)
                    tagSearch2.Add("");
                string search = tagSearch2[j];
                selectedTag = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Tag ") + (j + 1) + ":", ref search, selectedTag, tagOptions);
                tagSearch2[j] = search;
                tags2[j] = tagIds[selectedTag];
                pos.x += pos.width;
                pos.y += ImagePack.fieldHeight;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Tag")))
                {
                    tags2.RemoveAt(j);
                    tagSearch2.RemoveAt(j);
                }
                pos.x -= pos.width;
            }
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Tag")))
            {
                tags2.Add(0);
                tagSearch2.Add("");
            }
            editingDisplay.stringValue2 = string.Join(";", tags2.ConvertAll(i => i.ToString()).ToArray());
            pos.y += ImagePack.fieldHeight;
            if (!editingDisplay.boolValue2)
                editingDisplay.intValue2 = ImagePack.DrawField(pos, Lang.GetTranslate("Absorb Value"), editingDisplay.intValue2);
            else
                editingDisplay.intValue2 = ImagePack.DrawField(pos, Lang.GetTranslate("Reflect Value"), editingDisplay.intValue2);
            pos.x += pos.width;
            if (!editingDisplay.boolValue2)
                editingDisplay.floatValue2 = ImagePack.DrawField(pos, Lang.GetTranslate("Absorb %"), editingDisplay.floatValue2);
            else
                editingDisplay.floatValue2 = ImagePack.DrawField(pos, Lang.GetTranslate("Reflect %"), editingDisplay.floatValue2);
            pos.y += ImagePack.fieldHeight;
            pos.x -= pos.width;
            editingDisplay.boolValue2 = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Reflect ") + "?", editingDisplay.boolValue2);
            pos.y += ImagePack.fieldHeight;

            ////////////////////////////////////////////
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Setting 3"));
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Tags"));
            for (int j = 0; j < tags3.Count; j++)
            {
                pos.y += ImagePack.fieldHeight;
                int selectedTag = GetOptionPosition(tags3[j], tagIds);
                if (tagSearch3.Count <= j)
                    tagSearch3.Add("");
                string search = tagSearch3[j];
                selectedTag = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Tag ") + (j + 1) + ":", ref search, selectedTag, tagOptions);
                tagSearch3[j] = search;
                tags3[j] = tagIds[selectedTag];
                pos.x += pos.width;
                pos.y += ImagePack.fieldHeight;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Tag")))
                {
                    tags3.RemoveAt(j);
                    tagSearch3.RemoveAt(j);
                }
                pos.x -= pos.width;
            }
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Tag")))
            {
                tags3.Add(0);
                tagSearch3.Add("");
            }
            editingDisplay.stringValue3 = string.Join(";", tags3.ConvertAll(i => i.ToString()).ToArray());
            pos.y += ImagePack.fieldHeight;
            if (!editingDisplay.boolValue3)
                editingDisplay.intValue3 = ImagePack.DrawField(pos, Lang.GetTranslate("Absorb Value"), editingDisplay.intValue3);
            else
                editingDisplay.intValue3 = ImagePack.DrawField(pos, Lang.GetTranslate("Reflect Value"), editingDisplay.intValue3);
            pos.x += pos.width;
            if (!editingDisplay.boolValue3)
                editingDisplay.floatValue3 = ImagePack.DrawField(pos, Lang.GetTranslate("Absorb %"), editingDisplay.floatValue3);
            else
                editingDisplay.floatValue3 = ImagePack.DrawField(pos, Lang.GetTranslate("Reflect %"), editingDisplay.floatValue3);
            pos.y += ImagePack.fieldHeight;
            pos.x -= pos.width;
            editingDisplay.boolValue3 = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Reflect ") + "?", editingDisplay.boolValue3);
            pos.y += ImagePack.fieldHeight;

            ////////////////////////////////////////////
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Setting 4"));
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Tags"));
            for (int j = 0; j < tags4.Count; j++)
            {
                pos.y += ImagePack.fieldHeight;
                int selectedTag = GetOptionPosition(tags4[j], tagIds);
                if (tagSearch4.Count <= j)
                    tagSearch4.Add("");
                string search = tagSearch4[j];
                selectedTag = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Tag ") + (j + 1) + ":", ref search, selectedTag, tagOptions);
                tagSearch4[j] = search;
                tags4[j] = tagIds[selectedTag];
                pos.x += pos.width;
                pos.y += ImagePack.fieldHeight;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Tag")))
                {
                    tags4.RemoveAt(j);
                    tagSearch4.RemoveAt(j);
                }
                pos.x -= pos.width;
            }
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Tag")))
            {
                tags4.Add(0);
                tagSearch4.Add("");
            }
            editingDisplay.stringValue4 = string.Join(";", tags4.ConvertAll(i => i.ToString()).ToArray());
            pos.y += ImagePack.fieldHeight;
            if (!editingDisplay.boolValue4)
                editingDisplay.intValue4 = ImagePack.DrawField(pos, Lang.GetTranslate("Absorb Value"), editingDisplay.intValue4);
            else
                editingDisplay.intValue4 = ImagePack.DrawField(pos, Lang.GetTranslate("Reflect Value"), editingDisplay.intValue4);
            pos.x += pos.width;
            if (!editingDisplay.boolValue4)
                editingDisplay.floatValue4 = ImagePack.DrawField(pos, Lang.GetTranslate("Absorb %"), editingDisplay.floatValue4);
            else
                editingDisplay.floatValue4 = ImagePack.DrawField(pos, Lang.GetTranslate("Reflect %"), editingDisplay.floatValue4);
            pos.y += ImagePack.fieldHeight;
            pos.x -= pos.width;
            editingDisplay.boolValue4 = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Reflect ") + "?", editingDisplay.boolValue4);
            pos.y += ImagePack.fieldHeight;

            ////////////////////////////////////////////
          /*  pos.y += ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Setting 5"));
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Tags"));
            for (int j = 0; j < tags5.Count; j++)
            {
                pos.y += ImagePack.fieldHeight;
                int selectedTag = GetOptionPosition(tags5[j], tagIds);
                if (tagSearch5.Count <= j)
                    tagSearch5.Add("");
                string search = tagSearch5[j];
                selectedTag = ImagePack.DrawDynamicFilteredListSelector(pos, Lang.GetTranslate("Tag ") + (j + 1) + ":", ref search, selectedTag, tagOptions);
                tagSearch5[j] = search;
                tags5[j] = tagIds[selectedTag];
                pos.x += pos.width;
                pos.y += ImagePack.fieldHeight;
                if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Delete Tag")))
                {
                    tags5.RemoveAt(j);
                    tagSearch5.RemoveAt(j);
                }
                pos.x -= pos.width;
            }
            pos.y += ImagePack.fieldHeight;
            if (ImagePack.DrawButton(pos.x, pos.y, Lang.GetTranslate("Add Tag")))
            {
                tags5.Add(0);
                tagSearch5.Add("");
            }
            editingDisplay.stringValue5 = string.Join(";", tags5.ConvertAll(i => i.ToString()).ToArray());
            pos.y += ImagePack.fieldHeight;
            if (!editingDisplay.boolValue5)
                editingDisplay.intValue5 = ImagePack.DrawField(pos, Lang.GetTranslate("Absorb Value"), editingDisplay.intValue5);
            else
                editingDisplay.intValue5 = ImagePack.DrawField(pos, Lang.GetTranslate("Reflect Value"), editingDisplay.intValue5);
            pos.x += pos.width;
            if (!editingDisplay.boolValue5)
                editingDisplay.floatValue5 = ImagePack.DrawField(pos, Lang.GetTranslate("Absorb %"), editingDisplay.floatValue5);
            else
                editingDisplay.floatValue5 = ImagePack.DrawField(pos, Lang.GetTranslate("Reflect %"), editingDisplay.floatValue5);
            pos.y += ImagePack.fieldHeight;
            pos.x -= pos.width;
            editingDisplay.boolValue5 = ImagePack.DrawToggleBox(pos, Lang.GetTranslate("Reflect ") + "?", editingDisplay.boolValue5);
            pos.y += ImagePack.fieldHeight;

            ////////////////////////////////////////////
            */





            pos.y += ImagePack.fieldHeight+5;
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