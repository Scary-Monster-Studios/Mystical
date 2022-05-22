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
    public class ServerDispelEffect : ServerEffectType
    {

        public new string effectType = "Dispel";
        public new string[] effectTypeOptions = new string[] { "DispelEffect" };
        public string[] dispelTypeOptions = new string[] { "All", /*"Buffs", "Debuffs",*/"By Tags", "MountEffect", "MorphEffect" };

        public int[] instanceIds = new int[] { -1 };
        public string[] instanceOptions = new string[] { "~ none ~" };

        public int[] tagIds = new int[] { -1 };
        public string[] tagOptions = new string[] { "~ none ~" };
        List<int> tags = new List<int>();
        List<string> tagSearch = new List<string>();

        // Use this for initialization
        public ServerDispelEffect()
        {
        }

        public override void LoadOptions(EffectsData editingDisplay, bool newItem)
        {
            ServerOptionChoices.LoadAtavismChoiceOptions("Effects Tags", false, out tagIds, out tagOptions);

        }

        // Edit or Create
        public override Rect DrawEditor(Rect pos, bool newItem, EffectsData editingDisplay, out bool showTimeFields, out bool showSkillModFields)
        {
            editingDisplay.stringValue1 = ImagePack.DrawSelector(pos, Lang.GetTranslate("Dispel Type")+":", editingDisplay.stringValue1, dispelTypeOptions);
            pos.y += ImagePack.fieldHeight;
            if (editingDisplay.stringValue1.Equals("By Tags"))
            {
              //  pos.width /= 2;
                ImagePack.DrawLabel(pos.x, pos.y, Lang.GetTranslate("Dispel Tags"));
                //   pos.y += ImagePack.fieldHeight;
                tags.Clear();

               if (editingDisplay.stringValue2.Length > 0)
                {
                    string[] splitted = editingDisplay.stringValue2.Split(';');
                    for (int i = 0; i < splitted.Length; i++)
                    {
                        tags.Add(int.Parse(splitted[i]));
                        if(tagSearch.Count<=i)
                            tagSearch.Add("");

                    }
                }

                for (int j = 0; j < tags.Count; j++)
                {
                    pos.y += ImagePack.fieldHeight;

                    int selectedTag = GetOptionPosition(tags[j], tagIds);
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

                editingDisplay.stringValue2 = string.Join(";", tags.ConvertAll(i => i.ToString()).ToArray());
             //   pos.width *= 2;
                pos.y += ImagePack.fieldHeight;

            }
            editingDisplay.intValue1 = ImagePack.DrawField(pos, Lang.GetTranslate("Amount to Remove")+":", editingDisplay.intValue1);
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