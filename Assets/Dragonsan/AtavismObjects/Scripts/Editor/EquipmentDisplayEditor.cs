using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if WeaponOffset
using HNGamers.Atavism;
#endif

namespace Atavism
{
    [CustomEditor(typeof(EquipmentDisplay))]
    public class EquipmentDisplayEditor : Editor
    {
        bool help = false;
        public int[] classIds = new int[] {-1};
        public string[] classOptions = new string[] {"~ none ~"};

        public int[] raceIds = new int[] {-1};
        public string[] raceOptions = new string[] {"~ none ~"};
        public int[] genderIds = new int[] {-1};
        public string[] genderOptions = new string[] {"~ none ~"};
        GUIContent[] slots;
        GUIContent[] slots2;

        private bool loaded = false;

        public override void OnInspectorGUI()
        {
            EquipmentDisplay obj = target as EquipmentDisplay;
            //   var indentOffset = EditorGUI.indentLevel * 5f;
            var lineRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(lineRect.x, lineRect.y, EditorGUIUtility.currentViewWidth - 60f, lineRect.height);
            var fieldRect = new Rect(labelRect.xMax, lineRect.y, lineRect.width - labelRect.width - 60f, lineRect.height);
            var buttonRect = new Rect(fieldRect.xMax, lineRect.y, 60f, lineRect.height);
            GUIContent content = new GUIContent("Help");
            content.tooltip = "Click to show or hide help informations";
            if (GUI.Button(buttonRect, content, EditorStyles.miniButton))
                help = !help;

            GUIStyle topStyle = new GUIStyle(GUI.skin.box);
            topStyle.normal.textColor = Color.white;
            topStyle.fontStyle = FontStyle.Bold;
            topStyle.alignment = TextAnchor.UpperLeft;
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.textColor = Color.cyan;
            boxStyle.fontStyle = FontStyle.Bold;
            boxStyle.alignment = TextAnchor.UpperLeft;
            GUILayout.BeginVertical("Atavism Equipment Display Configuration", topStyle);
            GUILayout.Space(20);
            //EditorGUILayout.LabelField("ID: " + obj.id);
            if (!loaded)
            {
                ServerOptionChoices.LoadAtavismChoiceOptions("Race", false, out raceIds, out raceOptions, true);
                ServerOptionChoices.LoadAtavismChoiceOptions("Class", false, out classIds, out classOptions, true);
                ServerOptionChoices.LoadAtavismChoiceOptions("Gender", false, out genderIds, out genderOptions, true);
                loaded = true;
            }

            if (slots == null)
                slots = ServerItems.LoadSlotsOptions();
            if (slots2 == null)
                slots2 = ServerItems.LoadSlotsOptions(true,true);

            content = new GUIContent("Equip Display Type");
            content.tooltip = "Select equip display type";
            obj.equipDisplayType = (EquipDisplayType) EditorGUILayout.EnumPopup(content, obj.equipDisplayType);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.equipDisplayType == EquipDisplayType.AttachedObject)
            {
                content = new GUIContent("Model");
                content.tooltip = "Select model to attach";
                obj.model = (GameObject) EditorGUILayout.ObjectField(content, obj.model, typeof(GameObject), false);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
#if WeaponOffset
                content = new GUIContent("WeaponOffset");
                content.tooltip = "Select WeaponOffset";
                obj.weaponOffset = (WeaponOffset) EditorGUILayout.ObjectField(content, obj.weaponOffset, typeof(WeaponOffset), false);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
#endif
#if WeaponIKOffset
                content = new GUIContent("WeaponIKOffset");
                content.tooltip = "Select WeaponIKOffset for this item";
                obj.weaponIKOffset = (WeaponIKOffset)EditorGUILayout.ObjectField(content, obj.weaponIKOffset, typeof(WeaponIKOffset), false);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
#endif

            }

            if (string.IsNullOrEmpty(obj.modelName) && obj.model != null)
            {
                obj.modelName = obj.model.name;
            }

            if (obj.equipDisplayType == EquipDisplayType.ActivatedModel || obj.equipDisplayType == EquipDisplayType.BaseTextureSwap)
            {
                content = new GUIContent("Model Name");
                content.tooltip = "Set Model Name to Activate";
                obj.modelName = EditorGUILayout.TextField(content, obj.modelName);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }

            if (obj.equipDisplayType == EquipDisplayType.ModularArmor)
            {
                content = new GUIContent("Model Name");
                content.tooltip = "Set Model Name to Activate";
                obj.modelName = EditorGUILayout.TextField(content, obj.modelName);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }

            if (obj.equipDisplayType == EquipDisplayType.BaseTextureSwap)
            {

                content = new GUIContent("Material");
                content.tooltip = "Select material to change";
                obj.material = (Material) EditorGUILayout.ObjectField(content, obj.material, typeof(Material), false);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }

         /*   if (obj.equipDisplayType == EquipDisplayType.ModularTextureSwap)
            {
                content = new GUIContent("Material To Replace");
                content.tooltip = "Select material to change";
                obj.MaterialToReplace = (Material) EditorGUILayout.ObjectField(content, obj.MaterialToReplace, typeof(Material), false);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


                content = new GUIContent("Replacement Material");
                content.tooltip = "Select replacement material";
                obj.ReplacementMaterial = (Material) EditorGUILayout.ObjectField(content, obj.ReplacementMaterial, typeof(Material), false);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);



                content = new GUIContent("Shader To Replace");
                content.tooltip = "Select shader to change";
                obj.ShaderToReplace = (Shader) EditorGUILayout.ObjectField(content, obj.ShaderToReplace, typeof(Shader), false);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


                content = new GUIContent("Replacement Shader");
                content.tooltip = "Select replacement shader";
                obj.ReplacementShader = (Shader) EditorGUILayout.ObjectField(content, obj.ReplacementShader, typeof(Shader), false);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }*/

            if (obj.equipDisplayType == EquipDisplayType.AttachedObject /*|| obj.equipDisplayType == EquipDisplayType.ModularArmor*/)
            {
                content = new GUIContent("What type of attached gear.");
                content.tooltip = "gearType";
                obj.gearType = (GearType) EditorGUILayout.EnumPopup(content, obj.gearType);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }

            if (obj.equipDisplayType == EquipDisplayType.AttachedObject)
            {
                content = new GUIContent("Use Rest Slot");
                content.tooltip = "Use Rest Slot";
                obj.useRestSlot = EditorGUILayout.Toggle(content, obj.useRestSlot);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                content = new GUIContent("Default Rest Slot");
                content.tooltip = "Sefault Rest Slot";
                obj.defaultRestSlot = EditorGUILayout.Toggle(content, obj.defaultRestSlot);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                
                
            }

            if (obj.equipDisplayType == EquipDisplayType.AttachedObject || obj.equipDisplayType == EquipDisplayType.ModularArmor)
            {
                content = new GUIContent("Override Slot");
                content.tooltip = "Select slot in which particle object will appear. Slots can be defined for particular model in the AtavismMobAppearance component on the character prefab";
                int si = 0;
                int sj = 0;
                foreach (GUIContent c in slots2)
                {
                    if (c.text.Equals(obj.overrideSlot))
                        si = sj;
                    sj++;
                }

                si = EditorGUILayout.Popup(content, si, slots2);
                obj.overrideSlot = slots2[si].text;
                //  obj.slot = (AttachmentSocket)EditorGUILayout.EnumPopup(content, obj.slot);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                
                
                GUILayout.BeginVertical("Race Definitions", boxStyle);
                GUILayout.Space(20);

                for (int r = 0; r < obj.raceIds.Count; r++)
                {

                    GUI.backgroundColor = Color.red;
                    GUILayout.BeginVertical("", topStyle);
                    GUI.backgroundColor = Color.white; //  DamageType dt = obj.damageTypeColor[key];
                    content = new GUIContent("Race");
                    content.tooltip = "Select Race";
                    int selected = GetOptionPosition(obj.raceIds[r], raceIds);
                    selected = EditorGUILayout.Popup(content, selected, raceOptions);
                    obj.raceIds[r] = raceIds[selected];
                    obj.raceDef[r].name = raceOptions[selected];
                    obj.raceDef[r].raceId = raceIds[selected];
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                    if (obj.equipDisplayType == EquipDisplayType.ModularArmor)
                    {
                        GUI.backgroundColor = Color.magenta;
                        GUILayout.BeginVertical("Modular Gear Part To Enable For Race", topStyle);
                        GUI.backgroundColor = Color.white;
                        GUILayout.Space(20);

                        content = new GUIContent("");
                        content.tooltip = "Select model to Add";
                        //var list = serializedObject.("GearParts");
                        for (int gp = 0; gp < obj.raceDef[r].GearParts.Count; gp++)
                        {
                            obj.raceDef[r].GearParts[gp] = (GameObject) EditorGUILayout.ObjectField(content, obj.raceDef[r].GearParts[gp], typeof(GameObject), true);
                            if (help)
                                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                            content = new GUIContent("");
                            content.tooltip = "Select model name to Add";
                            obj.raceDef[r].GearPartsName[gp] = EditorGUILayout.TextField(content, obj.raceDef[r].GearPartsName[gp]);
                            if (help)
                                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                            GUILayout.Space(2);
                        }

                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Add Gear Part"))
                        {
                            obj.raceDef[r].GearParts.Add(null);
                            obj.raceDef[r].GearPartsName.Add(null);
                        }

                        if (GUILayout.Button("Remove Gear Part"))
                        {
                            if (obj.raceDef[r].GearParts.Count > 0)
                                obj.raceDef[r].GearParts.RemoveAt(obj.raceDef[r].GearParts.Count - 1);
                            if (obj.raceDef[r].GearPartsName.Count > 0)
                                obj.raceDef[r].GearPartsName.RemoveAt(obj.raceDef[r].GearPartsName.Count - 1);
                        }
                      

                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                        GUILayout.Space(10);
                         GUI.backgroundColor = Color.gray;
                        GUILayout.BeginVertical("Modular Gear Part To Disable For Race", topStyle);
                        GUI.backgroundColor = Color.white;
                        GUILayout.Space(20);

                        content = new GUIContent("");
                        content.tooltip = "Select model to disable";
                        //var list = serializedObject.("GearParts");
                        for (int gp = 0; gp < obj.raceDef[r].disableGearParts.Count; gp++)
                        {
                            obj.raceDef[r].disableGearParts[gp] = (GameObject) EditorGUILayout.ObjectField(content, obj.raceDef[r].disableGearParts[gp], typeof(GameObject), true);
                            if (help)
                                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                            content = new GUIContent("");
                            content.tooltip = "Select model name to disable";
                            obj.raceDef[r].disableGearPartsName[gp] = EditorGUILayout.TextField(content, obj.raceDef[r].disableGearPartsName[gp]);
                            if (help)
                                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                            GUILayout.Space(2);
                        }

                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Add Gear Part"))
                        {
                            obj.raceDef[r].disableGearParts.Add(null);
                            obj.raceDef[r].disableGearPartsName.Add(null);
                        }

                        if (GUILayout.Button("Remove Gear Part"))
                        {
                            if (obj.raceDef[r].disableGearParts.Count > 0)
                                obj.raceDef[r].disableGearParts.RemoveAt(obj.raceDef[r].disableGearParts.Count - 1);
                            if (obj.raceDef[r].disableGearPartsName.Count > 0)
                                obj.raceDef[r].disableGearPartsName.RemoveAt(obj.raceDef[r].disableGearPartsName.Count - 1);
                        }
                      

                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                    }

                    GUILayout.Space(3);
                    for (int a = 0; a < obj.raceDef[r].aspectIds.Count; a++)
                    {
                        GUI.backgroundColor = Color.green;
                        GUILayout.BeginVertical("", topStyle);
                        GUI.backgroundColor = Color.white;


                        content = new GUIContent("Class");
                        content.tooltip = "Select Class";
                        selected = GetOptionPosition(obj.raceDef[r].aspectIds[a], classIds);
                        selected = EditorGUILayout.Popup(content, selected, classOptions);
                        obj.raceDef[r].aspectIds[a] = classIds[selected];
                        obj.raceDef[r].aspectDef[a].name = classOptions[selected];
                        obj.raceDef[r].aspectDef[a].aspectId = classIds[selected];
                        if (help)
                            EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                        if (obj.equipDisplayType == EquipDisplayType.ModularArmor)
                        {
                            GUI.backgroundColor = Color.magenta;
                            GUILayout.BeginVertical("Modular Gear Part For Class", topStyle);
                            GUI.backgroundColor = Color.white;
                            GUILayout.Space(20);

                            content = new GUIContent("");
                            content.tooltip = "Select model to Add";
                            //var list = serializedObject.("GearParts");
                            for (int gp = 0; gp < obj.raceDef[r].aspectDef[a].GearParts.Count; gp++)
                            {
                                obj.raceDef[r].aspectDef[a].GearParts[gp] = (GameObject) EditorGUILayout.ObjectField(content, obj.raceDef[r].aspectDef[a].GearParts[gp], typeof(GameObject), true);
                                if (help)
                                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                                content = new GUIContent("");
                                content.tooltip = "Select model name to Add";
                                obj.raceDef[r].aspectDef[a].GearPartsName[gp] = EditorGUILayout.TextField(content, obj.raceDef[r].aspectDef[a].GearPartsName[gp]);
                                if (help)
                                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                            }

                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("Add Gear Part"))
                            {
                                obj.raceDef[r].aspectDef[a].GearParts.Add(null);
                                obj.raceDef[r].aspectDef[a].GearPartsName.Add("");
                            }

                            if (GUILayout.Button("Remove Gear Part"))
                            {
                                if (obj.raceDef[r].aspectDef[a].GearParts.Count > 0)
                                    obj.raceDef[r].aspectDef[a].GearParts.RemoveAt(obj.raceDef[r].aspectDef[a].GearParts.Count - 1);
                                if (obj.raceDef[r].aspectDef[a].GearPartsName.Count > 0)
                                    obj.raceDef[r].aspectDef[a].GearPartsName.RemoveAt(obj.raceDef[r].aspectDef[a].GearPartsName.Count - 1);
                            }

                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();
                            GUILayout.Space(10);
                              GUI.backgroundColor = Color.gray;
                            GUILayout.BeginVertical("Modular Gear Part To Disable For Class", topStyle);
                            GUI.backgroundColor = Color.white;
                            GUILayout.Space(20);

                            content = new GUIContent("");
                            content.tooltip = "Select model to disable";
                            //var list = serializedObject.("GearParts");
                            for (int gp = 0; gp < obj.raceDef[r].aspectDef[a].disableGearParts.Count; gp++)
                            {
                                obj.raceDef[r].aspectDef[a].disableGearParts[gp] = (GameObject) EditorGUILayout.ObjectField(content, obj.raceDef[r].aspectDef[a].disableGearParts[gp], typeof(GameObject), true);
                                if (help)
                                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                                content = new GUIContent("");
                                content.tooltip = "Select model name to disable";
                                obj.raceDef[r].aspectDef[a].disableGearPartsName[gp] = EditorGUILayout.TextField(content, obj.raceDef[r].aspectDef[a].disableGearPartsName[gp]);
                                if (help)
                                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                            }

                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("Add Gear Part"))
                            {
                                obj.raceDef[r].aspectDef[a].disableGearParts.Add(null);
                                obj.raceDef[r].aspectDef[a].disableGearPartsName.Add("");
                            }

                            if (GUILayout.Button("Remove Gear Part"))
                            {
                                if (obj.raceDef[r].aspectDef[a].disableGearParts.Count > 0)
                                    obj.raceDef[r].aspectDef[a].disableGearParts.RemoveAt(obj.raceDef[r].aspectDef[a].disableGearParts.Count - 1);
                                if (obj.raceDef[r].aspectDef[a].disableGearPartsName.Count > 0)
                                    obj.raceDef[r].aspectDef[a].disableGearPartsName.RemoveAt(obj.raceDef[r].aspectDef[a].disableGearPartsName.Count - 1);
                            }

                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();
                        }

                        GUILayout.Space(3);
                        for (int g = 0; g < obj.raceDef[r].aspectDef[a].genderIds.Count; g++)
                        {
                            GUI.backgroundColor = Color.blue;
                            GUILayout.BeginVertical("", topStyle);
                            GUI.backgroundColor = Color.white;


                            content = new GUIContent("Gender");
                            content.tooltip = "Select Gender";
                            selected = GetOptionPosition(obj.raceDef[r].aspectDef[a].genderIds[g], genderIds);
                            selected = EditorGUILayout.Popup(content, selected, genderOptions);
                            obj.raceDef[r].aspectDef[a].genderIds[g] = genderIds[selected];
                            obj.raceDef[r].aspectDef[a].genderDef[g].name = genderOptions[selected];
                            obj.raceDef[r].aspectDef[a].genderDef[g].ganderId = genderIds[selected];
                            if (help)
                                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                            if (obj.equipDisplayType == EquipDisplayType.AttachedObject)
                            {
                                content = new GUIContent("Model");
                                content.tooltip = "Select model to attach";
                                obj.raceDef[r].aspectDef[a].genderDef[g].model = (GameObject) EditorGUILayout.ObjectField(content, obj.raceDef[r].aspectDef[a].genderDef[g].model, typeof(GameObject), false);
                                if (help)
                                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                            }

                            if (obj.equipDisplayType == EquipDisplayType.ModularArmor)
                            {
                                GUI.backgroundColor = Color.magenta;
                                GUILayout.BeginVertical("Modular Gear Part For Gender", topStyle);
                                GUI.backgroundColor = Color.white;
                                GUILayout.Space(20);

                             
                                //var list = serializedObject.("GearParts");
                                for (int gp = 0; gp < obj.raceDef[r].aspectDef[a].genderDef[g].GearParts.Count; gp++)
                                {
                                    content = new GUIContent("");
                                    content.tooltip = "Select model to Add";
                                    obj.raceDef[r].aspectDef[a].genderDef[g].GearParts[gp] = (GameObject) EditorGUILayout.ObjectField(content, obj.raceDef[r].aspectDef[a].genderDef[g].GearParts[gp], typeof(GameObject), true);
                                    if (help)
                                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                                    content = new GUIContent("");
                                    content.tooltip = "Select model name to Add";
                                    obj.raceDef[r].aspectDef[a].genderDef[g].GearPartsName[gp] = EditorGUILayout.TextField(content, obj.raceDef[r].aspectDef[a].genderDef[g].GearPartsName[gp]);
                                    if (help)
                                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                                }

                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button("Add Gear Part"))
                                {
                                    obj.raceDef[r].aspectDef[a].genderDef[g].GearParts.Add(null);
                                    obj.raceDef[r].aspectDef[a].genderDef[g].GearPartsName.Add("");
                                }

                                if (GUILayout.Button("Remove Gear Part"))
                                {
                                    if (obj.raceDef[r].aspectDef[a].genderDef[g].GearParts.Count > 0)
                                        obj.raceDef[r].aspectDef[a].genderDef[g].GearParts.RemoveAt(obj.raceDef[r].aspectDef[a].genderDef[g].GearParts.Count - 1);
                                    if (obj.raceDef[r].aspectDef[a].genderDef[g].GearPartsName.Count > 0)
                                        obj.raceDef[r].aspectDef[a].genderDef[g].GearPartsName.RemoveAt(obj.raceDef[r].aspectDef[a].genderDef[g].GearPartsName.Count - 1);
                                }

                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.EndVertical();
                                GUILayout.Space(10);
                                 GUI.backgroundColor = Color.gray;
                                GUILayout.BeginVertical("Modular Gear Part To Disable For Gender", topStyle);
                                GUI.backgroundColor = Color.white;
                                GUILayout.Space(20);

                             
                                //var list = serializedObject.("GearParts");
                                for (int gp = 0; gp < obj.raceDef[r].aspectDef[a].genderDef[g].disableGearParts.Count; gp++)
                                {
                                    content = new GUIContent("");
                                    content.tooltip = "Select model to Add";
                                    obj.raceDef[r].aspectDef[a].genderDef[g].disableGearParts[gp] = (GameObject) EditorGUILayout.ObjectField(content, obj.raceDef[r].aspectDef[a].genderDef[g].disableGearParts[gp], typeof(GameObject), true);
                                    if (help)
                                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                                    content = new GUIContent("");
                                    content.tooltip = "Select model name to Add";
                                    obj.raceDef[r].aspectDef[a].genderDef[g].disableGearPartsName[gp] = EditorGUILayout.TextField(content, obj.raceDef[r].aspectDef[a].genderDef[g].disableGearPartsName[gp]);
                                    if (help)
                                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                                }

                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button("Add Gear Part"))
                                {
                                    obj.raceDef[r].aspectDef[a].genderDef[g].disableGearParts.Add(null);
                                    obj.raceDef[r].aspectDef[a].genderDef[g].disableGearPartsName.Add("");
                                }

                                if (GUILayout.Button("Remove Gear Part"))
                                {
                                    if (obj.raceDef[r].aspectDef[a].genderDef[g].disableGearParts.Count > 0)
                                        obj.raceDef[r].aspectDef[a].genderDef[g].disableGearParts.RemoveAt(obj.raceDef[r].aspectDef[a].genderDef[g].disableGearParts.Count - 1);
                                    if (obj.raceDef[r].aspectDef[a].genderDef[g].disableGearPartsName.Count > 0)
                                        obj.raceDef[r].aspectDef[a].genderDef[g].disableGearPartsName.RemoveAt(obj.raceDef[r].aspectDef[a].genderDef[g].disableGearPartsName.Count - 1);
                                }

                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.EndVertical();
                            }

                            GUILayout.Space(6);
                            if (obj.equipDisplayType == EquipDisplayType.AttachedObject)
                            {
                                for (int s = 0; s < obj.raceDef[r].aspectDef[a].genderDef[g].slot.Count; s++)
                                {
                                    GUI.backgroundColor = Color.red;
                                    GUILayout.BeginVertical("", topStyle);
                                    GUI.backgroundColor = Color.white;


                                    content = new GUIContent("Slot");
                                    content.tooltip = "Select slot in which particle object will appear. Slots can be defined for particular model in the AtavismMobAppearance component on the character prefab";
                                    int ii = 0;
                                    int j = 0;
                                    foreach (GUIContent c in slots)
                                    {
                                        if (c.text.Equals(obj.raceDef[r].aspectDef[a].genderDef[g].slot[s]))
                                            ii = j;
                                        j++;
                                    }

                                    ii = EditorGUILayout.Popup(content, ii, slots);
                                    obj.raceDef[r].aspectDef[a].genderDef[g].slot[s] = slots[ii].text;
                                    //  obj.slot = (AttachmentSocket)EditorGUILayout.EnumPopup(content, obj.slot);
                                    if (help)
                                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                                    /*  content = new GUIContent("Slot");
                                      content.tooltip = "Set Slot";
                                      obj.raceDef[r].aspectDef[a].genderDef[g].slot[s] = EditorGUILayout.TextField(content, obj.raceDef[r].aspectDef[a].genderDef[g].slot[s]);
                                      if (help)
                                          EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            */
                                    content = new GUIContent("Slot Position");
                                    content.tooltip = "Set Slot Position";
                                    obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].position = EditorGUILayout.Vector3Field(content, obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].position);
                                    if (help)
                                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                                    content = new GUIContent("Slot Rotation");
                                    content.tooltip = "Set Slot Rotation";
                                    obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].rotation = EditorGUILayout.Vector3Field(content, obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].rotation);
                                    if (help)
                                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                                    content = new GUIContent("Slot Scale");
                                    content.tooltip = "Set Slot Scale";
                                    obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].scale = EditorGUILayout.Vector3Field(content, obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].scale);
                                    if (help)
                                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                                    GUILayout.Space(3);
                                    content = new GUIContent("Rest Slot Position");
                                    content.tooltip = "Set Rest Slot Position";
                                    obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].restPosition = EditorGUILayout.Vector3Field(content, obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].restPosition);
                                    if (help)
                                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                                    content = new GUIContent("Rest Slot Rotation");
                                    content.tooltip = "Set Rest Slot Rotation";
                                    obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].restRotation = EditorGUILayout.Vector3Field(content, obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].restRotation);
                                    if (help)
                                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                                    content = new GUIContent("Rest Slot Scale");
                                    content.tooltip = "Set Rest Slot Scale";
                                    obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].restScale = EditorGUILayout.Vector3Field(content, obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].restScale);
                                    if (help)
                                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                                    //  bool showCopyTooSlot = false;
                                    EditorGUILayout.BeginHorizontal();
                                    if (GUILayout.Button("Copy Slot To"))
                                    {
                                        obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].showCopyToSlot = !obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].showCopyToSlot;
                                    }

                                    if (GUILayout.Button("Remove Slot"))
                                    {
                                        if (obj.raceDef[r].aspectDef[a].genderDef[g].slotDef.Count > 0)
                                            obj.raceDef[r].aspectDef[a].genderDef[g].slotDef.RemoveAt(s);
                                        // .RemoveAt(obj.raceDef[r].aspectDef[a].genderDef[g].slotDef.Count - 1);
                                        if (obj.raceDef[r].aspectDef[a].genderDef[g].slot.Count > 0)
                                            obj.raceDef[r].aspectDef[a].genderDef[g].slot.RemoveAt(s); //obj.raceDef[r].aspectDef[a].genderDef[g].slot.Count - 1);
                                    }

                                    EditorGUILayout.EndHorizontal();
                                    if(obj.raceDef[r].aspectDef[a].genderDef[g].slotDef.ElementAtOrDefault(s)!=null)
                                    if (obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].showCopyToSlot)
                                    {
                                        GUI.backgroundColor = Color.yellow;
                                        GUILayout.BeginVertical("Copy Slot To", topStyle);
                                        GUI.backgroundColor = Color.white;
                                        GUILayout.Space(15);
                                        content = new GUIContent("Race");
                                        content.tooltip = "Select Race";
                                        int selectedRace = GetOptionPosition(obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].rI, raceIds);
                                        selectedRace = EditorGUILayout.Popup(content, selectedRace, raceOptions);
                                        obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].rI = raceIds[selectedRace];
                                        int rI = raceIds[selectedRace];

                                        content = new GUIContent("Class");
                                        content.tooltip = "Select Class";
                                        int selectedClass = GetOptionPosition(obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].cI, classIds);
                                        selectedClass = EditorGUILayout.Popup(content, selectedClass, classOptions);
                                        obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].cI = classIds[selectedClass];
                                        int cI = classIds[selectedClass];
                                        content = new GUIContent("Gender");
                                        content.tooltip = "Select Gender";
                                        int selectedGender = GetOptionPosition(obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].gI, genderIds);
                                        selectedGender = EditorGUILayout.Popup(content, selectedGender, genderOptions);
                                        obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].gI = genderIds[selectedGender];
                                        int gI = genderIds[selectedGender];
                                        ii = 0;
                                        j = 0;
                                        foreach (GUIContent c in slots)
                                        {
                                            if (c.text.Equals(obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].sN))
                                                ii = j;
                                            j++;
                                        }

                                        ii = EditorGUILayout.Popup(content, ii, slots);
                                        obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].sN = slots[ii].text;
                                        string sN = slots[ii].text;
                                        if (GUILayout.Button("Copy Slot"))
                                        {
                                            int rindex = obj.raceIds.IndexOf(rI);
                                            if (rindex == -1)
                                            {
                                                obj.raceIds.Add(rI);
                                                obj.raceDef.Add(new RaceDef());
                                                rindex = obj.raceIds.IndexOf(rI);
                                                obj.raceDef[rindex].raceId = rI;
                                            }

                                            if (rindex > -1)
                                            {
                                                int aindex = obj.raceDef[rindex].aspectIds.IndexOf(cI);
                                                if (aindex == -1)
                                                {
                                                    obj.raceDef[rindex].aspectIds.Add(cI);
                                                    obj.raceDef[rindex].aspectDef.Add(new ClassDef());
                                                    aindex = obj.raceDef[rindex].aspectIds.IndexOf(cI);
                                                    obj.raceDef[rindex].aspectDef[aindex].aspectId = cI;
                                                }

                                                if (aindex > -1)
                                                {
                                                    int gindex = obj.raceDef[rindex].aspectDef[aindex].genderIds.IndexOf(gI);
                                                    if (gindex == -1)
                                                    {
                                                        obj.raceDef[rindex].aspectDef[aindex].genderIds.Add(gI);
                                                        obj.raceDef[rindex].aspectDef[aindex].genderDef.Add(new GenderDef());
                                                        gindex = obj.raceDef[rindex].aspectDef[aindex].genderIds.IndexOf(gI);
                                                        obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].ganderId = gI;
                                                    }

                                                    if (gindex > -1)
                                                    {
                                                        obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].slot.Add(sN);
                                                        obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].slotDef.Add(new SlotMod());
                                                        obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).position = obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].position;
                                                        obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).rotation = obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].rotation;
                                                        obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).scale = obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].scale;
                                                        obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).restPosition = obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].restPosition;
                                                        obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).restRotation = obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].restRotation;
                                                        obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).restScale = obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].restScale;
                                                    }
                                                    else
                                                    {
                                                        if (gI > 0)
                                                        {
                                                            obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].slot.Add(sN);
                                                            obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].slotDef.Add(new SlotMod());
                                                            obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).position = obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].position;
                                                            obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).rotation = obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].rotation;
                                                            obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).scale = obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].scale;
                                                            obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).restPosition = obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].restPosition;
                                                            obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).restRotation = obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].restRotation;
                                                            obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).restScale = obj.raceDef[r].aspectDef[a].genderDef[g].slotDef[s].restScale;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        GUILayout.Space(5);
                                        GUILayout.EndVertical();
                                        GUILayout.Space(5);
                                        
                                    }


                                    GUILayout.Space(5);
                                    GUILayout.EndVertical();
                                    GUILayout.Space(5);
                                }

                                EditorGUILayout.BeginHorizontal();
                                if (GUILayout.Button("Add Slot"))
                                {
                                    obj.raceDef[r].aspectDef[a].genderDef[g].slot.Add("");
                                    obj.raceDef[r].aspectDef[a].genderDef[g].slotDef.Add(new SlotMod());
                                }

                                EditorGUILayout.EndHorizontal();
                            }

                           
                            GUILayout.Space(10);


                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("Copy Gender To"))
                            {
                                obj.raceDef[r].aspectDef[a].genderDef[g].showCopyTo = !obj.raceDef[r].aspectDef[a].genderDef[g].showCopyTo;
                            }

                            if (GUILayout.Button("Remove Gender"))
                            {
                                if (obj.raceDef[r].aspectDef[a].genderIds.Count > 0)
                                    obj.raceDef[r].aspectDef[a].genderIds.RemoveAt(g);
                                if (obj.raceDef[r].aspectDef[a].genderDef.Count > 0)
                                    obj.raceDef[r].aspectDef[a].genderDef.RemoveAt(g);
                            }

                            EditorGUILayout.EndHorizontal();
                            if(obj.raceDef[r].aspectDef[a].genderDef.ElementAtOrDefault(g)!=null)
                            if (obj.raceDef[r].aspectDef[a].genderDef[g].showCopyTo)
                            {
                                GUI.backgroundColor = Color.yellow;
                                GUILayout.BeginVertical("Copy Gender To", topStyle);
                                GUI.backgroundColor = Color.white;
                                GUILayout.Space(15);
                                content = new GUIContent("Race");
                                content.tooltip = "Select Race";
                                int selectedRace = GetOptionPosition(obj.raceDef[r].aspectDef[a].genderDef[g].rI, raceIds);
                                selectedRace = EditorGUILayout.Popup(content, selectedRace, raceOptions);
                                obj.raceDef[r].aspectDef[a].genderDef[g].rI = raceIds[selectedRace];
                                int rI = raceIds[selectedRace];

                                content = new GUIContent("Class");
                                content.tooltip = "Select Class";
                                int selectedClass = GetOptionPosition(obj.raceDef[r].aspectDef[a].genderDef[g].cI, classIds);
                                selectedClass = EditorGUILayout.Popup(content, selectedClass, classOptions);
                                obj.raceDef[r].aspectDef[a].genderDef[g].cI = classIds[selectedClass];
                                int cI = classIds[selectedClass];
                                content = new GUIContent("Gender");
                                content.tooltip = "Select Gender";
                                int selectedGender = GetOptionPosition(obj.raceDef[r].aspectDef[a].genderDef[g].gI, genderIds);
                                selectedGender = EditorGUILayout.Popup(content, selectedGender, genderOptions);
                                obj.raceDef[r].aspectDef[a].genderDef[g].gI = genderIds[selectedGender];
                                int gI = genderIds[selectedGender];

                                if (GUILayout.Button("Copy Gender"))
                                {
                                    int rindex = obj.raceIds.IndexOf(rI);
                                    if (rindex == -1)
                                    {
                                        obj.raceIds.Add(rI);
                                        obj.raceDef.Add(new RaceDef());
                                        rindex = obj.raceIds.IndexOf(rI);
                                        obj.raceDef[rindex].raceId = rI;
                                    }

                                    if (rindex > -1)
                                    {
                                        int aindex = obj.raceDef[rindex].aspectIds.IndexOf(cI);
                                        if (aindex == -1)
                                        {
                                            obj.raceDef[rindex].aspectIds.Add(cI);
                                            obj.raceDef[rindex].aspectDef.Add(new ClassDef());
                                            aindex = obj.raceDef[rindex].aspectIds.IndexOf(cI);
                                            obj.raceDef[rindex].aspectDef[aindex].aspectId = cI;
                                        }

                                        if (aindex > -1)
                                        {
                                            int gindex = obj.raceDef[rindex].aspectDef[aindex].genderIds.IndexOf(gI);
                                            if (gindex == -1)
                                            {
                                                obj.raceDef[rindex].aspectDef[aindex].genderIds.Add(gI);
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef.Add(new GenderDef());
                                                gindex = obj.raceDef[rindex].aspectDef[aindex].genderIds.IndexOf(gI);
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].ganderId = gI;
                                            }

                                            obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].model = obj.raceDef[r].aspectDef[a].genderDef[g].model;
                                            // obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].slot.AddRange(obj.raceDef[r].aspectDef[a].genderDef[g].slot);
                                            foreach (var sN in obj.raceDef[r].aspectDef[a].genderDef[g].slot)
                                            {
                                                if (!obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].slot.Contains(sN))
                                                {
                                                    obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].slot.Add(sN);
                                                    obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].slotDef.Add(new SlotMod());
                                                }

                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).position = obj.raceDef[r].aspectDef[a].genderDef[g].getSlotDef(sN).position;
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).rotation = obj.raceDef[r].aspectDef[a].genderDef[g].getSlotDef(sN).rotation;
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).scale = obj.raceDef[r].aspectDef[a].genderDef[g].getSlotDef(sN).scale;
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).restPosition = obj.raceDef[r].aspectDef[a].genderDef[g].getSlotDef(sN).restPosition;
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).restRotation = obj.raceDef[r].aspectDef[a].genderDef[g].getSlotDef(sN).restRotation;
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).restScale = obj.raceDef[r].aspectDef[a].genderDef[g].getSlotDef(sN).restScale;
                                            }

                                            obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].GearParts.AddRange(obj.raceDef[r].aspectDef[a].genderDef[g].GearParts);
                                            obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].GearPartsName.AddRange(obj.raceDef[r].aspectDef[a].genderDef[g].GearPartsName);
                                        }
                                    }
                                }

                                GUILayout.Space(5);
                                GUILayout.EndVertical();
                            }

                            GUILayout.Space(5);
                            GUILayout.EndVertical();
                        }
                        GUILayout.Space(5);
                      //  GUILayout.EndVertical();
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Add Gender"))
                        {
                            obj.raceDef[r].aspectDef[a].genderIds.Add(-1);
                            obj.raceDef[r].aspectDef[a].genderDef.Add(new GenderDef());
                        }

                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Copy Class To"))
                        {
                            obj.raceDef[r].aspectDef[a].showCopyTo = !obj.raceDef[r].aspectDef[a].showCopyTo;
                        }
                        if (GUILayout.Button("Remove Class"))
                        {
                            if (obj.raceDef[r].aspectIds.Count > 0)
                                obj.raceDef[r].aspectIds.RemoveAt(a);
                            if (obj.raceDef[r].aspectDef.Count > 0)
                                obj.raceDef[r].aspectDef.RemoveAt(a);
                        }
                        EditorGUILayout.EndHorizontal();
                        if(obj.raceDef[r].aspectDef.ElementAtOrDefault(a)!=null)

                        if (obj.raceDef[r].aspectDef[a].showCopyTo)
                        {
                            GUI.backgroundColor = Color.yellow;
                            GUILayout.BeginVertical("Copy Class To", topStyle);
                            GUI.backgroundColor = Color.white;
                            GUILayout.Space(15);
                            content = new GUIContent("Race");
                            content.tooltip = "Select Race";
                            int selectedRace = GetOptionPosition(obj.raceDef[r].aspectDef[a].rI, raceIds);
                            selectedRace = EditorGUILayout.Popup(content, selectedRace, raceOptions);
                            obj.raceDef[r].aspectDef[a].rI = raceIds[selectedRace];
                            int rI = raceIds[selectedRace];

                            content = new GUIContent("Class");
                            content.tooltip = "Select Class";
                            int selectedClass = GetOptionPosition(obj.raceDef[r].aspectDef[a].cI, classIds);
                            selectedClass = EditorGUILayout.Popup(content, selectedClass, classOptions);
                            obj.raceDef[r].aspectDef[a].cI = classIds[selectedClass];
                            int cI = classIds[selectedClass];


                            if (GUILayout.Button("Copy Class"))
                            {
                                int rindex = obj.raceIds.IndexOf(rI);
                                if (rindex == -1)
                                {
                                    obj.raceIds.Add(rI);
                                    obj.raceDef.Add(new RaceDef());
                                    rindex = obj.raceIds.IndexOf(rI);
                                    obj.raceDef[rindex].raceId = rI;
                                }

                                if (rindex > -1)
                                {
                                    int aindex = obj.raceDef[rindex].aspectIds.IndexOf(cI);
                                    if (aindex == -1)
                                    {
                                        obj.raceDef[rindex].aspectIds.Add(cI);
                                        obj.raceDef[rindex].aspectDef.Add(new ClassDef());
                                        aindex = obj.raceDef[rindex].aspectIds.IndexOf(cI);
                                        obj.raceDef[rindex].aspectDef[aindex].aspectId = cI;
                                    }

                                    if (aindex > -1)
                                    {
                                        foreach (var gd in obj.raceDef[r].aspectDef[a].genderDef)
                                        {
                                            int gindex = obj.raceDef[rindex].aspectDef[aindex].genderIds.IndexOf(gd.ganderId);
                                            if (gindex == -1)
                                            {
                                                obj.raceDef[rindex].aspectDef[aindex].genderIds.Add(gd.ganderId);
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef.Add(new GenderDef());
                                                gindex = obj.raceDef[rindex].aspectDef[aindex].genderIds.IndexOf(gd.ganderId);
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].ganderId = gd.ganderId;
                                            }

                                            obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].model = gd.model;
                                            foreach (var sN in gd.slot)
                                            {
                                                if (!obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].slot.Contains(sN))
                                                {
                                                    obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].slot.Add(sN);
                                                    obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].slotDef.Add(new SlotMod());
                                                }

                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).position = gd.getSlotDef(sN).position;
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).rotation = gd.getSlotDef(sN).rotation;
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).scale = gd.getSlotDef(sN).scale;
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).restPosition = gd.getSlotDef(sN).restPosition;
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).restRotation = gd.getSlotDef(sN).restRotation;
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).restScale = gd.getSlotDef(sN).restScale;
                                            }

                                            obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].GearParts.AddRange(gd.GearParts);
                                            obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].GearPartsName.AddRange(gd.GearPartsName);
                                        }

                                        obj.raceDef[rindex].aspectDef[aindex].GearParts.AddRange(obj.raceDef[r].aspectDef[a].GearParts);
                                        obj.raceDef[rindex].aspectDef[aindex].GearPartsName.AddRange(obj.raceDef[r].aspectDef[a].GearPartsName);
                                    }
                                }
                            }

                            GUILayout.Space(10);
                            GUILayout.EndVertical();
                        }

                        GUILayout.Space(5);
                        GUILayout.EndVertical();
                    }
                   
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Class"))
                    {
                        obj.raceDef[r].aspectIds.Add(-1);
                        obj.raceDef[r].aspectDef.Add(new ClassDef());
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Copy Race To"))
                    {
                        obj.raceDef[r].showCopyTo = !obj.raceDef[r].showCopyTo;
                    }
                    if (GUILayout.Button("Remove Race"))
                    {
                        if (obj.raceIds.Count > 0)
                            obj.raceIds.RemoveAt(r);
                        if (obj.raceDef.Count > 0)
                            obj.raceDef.RemoveAt(r);
                    }
                    EditorGUILayout.EndHorizontal();
                    if(obj.raceDef.ElementAtOrDefault(r)!=null)
                    if (obj.raceDef[r].showCopyTo)
                    {
                        GUI.backgroundColor = Color.yellow;
                        GUILayout.BeginVertical("Copy Race To", topStyle);
                        GUI.backgroundColor = Color.white;
                        GUILayout.Space(15);
                        content = new GUIContent("Race");
                        content.tooltip = "Select Race";
                        int selectedRace = GetOptionPosition(obj.raceDef[r].rI, raceIds);
                        selectedRace = EditorGUILayout.Popup(content, selectedRace, raceOptions);
                        obj.raceDef[r].rI = raceIds[selectedRace];
                        int rI = raceIds[selectedRace];

                        if (GUILayout.Button("Copy Race"))
                        {
                            int rindex = obj.raceIds.IndexOf(rI);
                            if (rindex == -1)
                            {
                                obj.raceIds.Add(rI);
                                obj.raceDef.Add(new RaceDef());
                                rindex = obj.raceIds.IndexOf(rI);
                                obj.raceDef[rindex].raceId = rI;
                            }

                            foreach (var cd in obj.raceDef[r].aspectDef)
                            {
                                if (rindex > -1)
                                {
                                    int aindex = obj.raceDef[rindex].aspectIds.IndexOf(cd.aspectId);
                                    if (aindex == -1)
                                    {
                                        obj.raceDef[rindex].aspectIds.Add(cd.aspectId);
                                        obj.raceDef[rindex].aspectDef.Add(new ClassDef());
                                        aindex = obj.raceDef[rindex].aspectIds.IndexOf(cd.aspectId);
                                        obj.raceDef[rindex].aspectDef[aindex].aspectId = cd.aspectId;
                                    }

                                    if (aindex > -1)
                                    {
                                        foreach (var gd in cd.genderDef)
                                        {
                                            int gindex = obj.raceDef[rindex].aspectDef[aindex].genderIds.IndexOf(gd.ganderId);
                                            if (gindex == -1)
                                            {
                                                obj.raceDef[rindex].aspectDef[aindex].genderIds.Add(gd.ganderId);
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef.Add(new GenderDef());
                                                gindex = obj.raceDef[rindex].aspectDef[aindex].genderIds.IndexOf(gd.ganderId);
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].ganderId = gd.ganderId;
                                            }

                                            obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].model = gd.model;
                                            foreach (var sN in gd.slot)
                                            {
                                                if (!obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].slot.Contains(sN))
                                                {
                                                    obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].slot.Add(sN);
                                                    obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].slotDef.Add(new SlotMod());
                                                }

                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).position = gd.getSlotDef(sN).position;
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).rotation = gd.getSlotDef(sN).rotation;
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).scale = gd.getSlotDef(sN).scale;
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).restPosition = gd.getSlotDef(sN).restPosition;
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).restRotation = gd.getSlotDef(sN).restRotation;
                                                obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].getSlotDef(sN).restScale = gd.getSlotDef(sN).restScale;
                                            }

                                            obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].GearParts.AddRange(gd.GearParts);
                                            obj.raceDef[rindex].aspectDef[aindex].genderDef[gindex].GearPartsName.AddRange(gd.GearPartsName);
                                        }

                                        obj.raceDef[rindex].aspectDef[aindex].GearParts.AddRange(cd.GearParts);
                                        obj.raceDef[rindex].aspectDef[aindex].GearPartsName.AddRange(cd.GearPartsName);
                                    }

                                    obj.raceDef[rindex].GearParts.AddRange(obj.raceDef[r].GearParts);
                                    obj.raceDef[rindex].GearPartsName.AddRange(obj.raceDef[r].GearPartsName);
                                }
                            }
                        }

                        GUILayout.Space(10);
                        GUILayout.EndVertical();
                    }
                    GUILayout.Space(5);
                    GUILayout.EndVertical();
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add Race"))
                {
                    obj.raceIds.Add(-1);
                    obj.raceDef.Add(new RaceDef());
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }


            GUILayout.Space(5);


            if (obj.equipDisplayType == EquipDisplayType.ModularArmor || (obj.equipDisplayType == EquipDisplayType.AttachedObject && obj.gearType == GearType.AttachedObjectArmor))
            {
                content = new GUIContent("Head items to Disable");
                content.tooltip = "Select socket to Attach";
                obj.headDisables = (HeadDisables) EditorGUILayout.EnumPopup(content, obj.headDisables);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                content = new GUIContent("Shoulder items to Disable");
                content.tooltip = "Select socket to Attach";
                obj.shoulderDisables = (ShoulderDisables) EditorGUILayout.EnumPopup(content, obj.shoulderDisables);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }

            if (obj.equipDisplayType == EquipDisplayType.ModularArmor)
            {
                /*    content = new GUIContent("Male Modular Gear Parts");
                    content.tooltip = "Select model to Add";
                    var list = serializedObject.FindProperty("GearParts");
                    EditorGUILayout.PropertyField(list, content, true);
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
    */
                /*      content = new GUIContent("Female Modular Gear Part");
                      content.tooltip = "Select model to Add";
                      var femalelist = serializedObject.FindProperty("FemaleGearParts");
                      EditorGUILayout.PropertyField(femalelist, content, true);
      
                      if (help)
                          EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                      */
                /*  content = new GUIContent("RaceDisplayItem");
                  content.tooltip = "Select model to Add";
                  var raceList = serializedObject.FindProperty("RaceDisplayItem");
                  EditorGUILayout.PropertyField(raceList, content, true);
                  if (help)
                      EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
  
  */

#if ModularNHance
                content = new GUIContent("Enable nHance Modular Equipment");
                content.tooltip = "This for a skinned meshed rendering character system by nHance.";
                //var enableEquipment = serializedObject.FindProperty("enableEquipment");
                obj.enableEquipment = EditorGUILayout.Toggle(content, obj.enableEquipment);

               // obj.enableEquipment = EditorGUILayout.PropertyField(enableEquipment, content, true);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
#endif
            }



            GUILayout.Space(2);

            GUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(obj);
                EditorUtility.SetDirty(target);
            }
        }



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