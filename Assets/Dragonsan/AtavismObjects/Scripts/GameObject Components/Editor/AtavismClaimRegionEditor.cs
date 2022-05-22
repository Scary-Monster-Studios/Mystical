using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    [CustomEditor(typeof(AtavismClaimRegion))]
    public class AtavismClaimRegionEditor : Editor
    {

        private bool currencyLoaded = false;
        private bool itemsLoaded = false;
        private bool profileLoaded = false;
      //  private bool instancesLoaded = false;
      string[] optionsClaimType;
      int[] optionsClaimTypeIds;
        string[] interactionTypes;
        bool help = false;

        private string profileSearch = "";
        private List<string> upgprofileSearch = new List<string>();

        public override void OnInspectorGUI()
        {
            AtavismClaimRegion obj = target as AtavismClaimRegion;
          //  var indentOffset = EditorGUI.indentLevel * 5f;
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
            GUILayout.BeginVertical("Atavism Claim Region Configuration", topStyle);
            GUILayout.Space(20);
            if (optionsClaimType == null)
            {
                ServerOptionChoices.LoadAtavismChoiceOptions("Claim Type", false, out optionsClaimTypeIds, out optionsClaimType);
            }
            if (optionsClaimType.Length == 0)
            {
                EditorGUILayout.LabelField("!! Claim Type is not loaded check database configuration in Old Atavism Editor !!");
                GUILayout.EndVertical();
                return;
            }
            //EditorGUILayout.LabelField("ID: " + obj.id);
            content = new GUIContent("ID");
            content.tooltip = "Unique Id of Graveyard";
            var lineResetRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var buttonResetRect = new Rect(fieldRect.xMax, lineResetRect.y, 60f, lineResetRect.height);
            GUIContent reset = new GUIContent("Reset ID");
            reset.tooltip = "Click to reset ID";
            if (GUI.Button(buttonResetRect, reset, EditorStyles.miniButton))
                obj.id = -1;
            EditorGUI.BeginDisabledGroup(true);
            obj.id = EditorGUILayout.IntField("ID:", obj.id);
            EditorGUI.EndDisabledGroup();
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


            // Read in Interaction options from Database and display a drop down list
            if (interactionTypes == null)
            {
                interactionTypes = new string[] { "~ none ~" };
                interactionTypes = ServerOptionChoices.LoadAtavismChoiceOptions("Interaction Type", false);
            }
            content = new GUIContent("Claim Type");
            content.tooltip = "Type of Claim";
            int selected = GetOptionPosition(obj.claimType, optionsClaimTypeIds);
            selected = EditorGUILayout.Popup(content, selected, optionsClaimType);
            obj.claimType = optionsClaimTypeIds[selected];
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (!profileLoaded)
            {
              ServerInstanceObjects.LoadClaimObjectLimitProfile();
              profileLoaded = true;
            }
            content = new GUIContent("Profile Search:");
            content.tooltip = "Search Profile by name";

          
            profileSearch = EditorGUILayout.TextField(content, profileSearch);
            
            content = new GUIContent("Object Limit Profile:");
            content.tooltip = "Object Limit Profile";
            obj.object_limit_profile = ServerCharacter.GetFilteredListSelector(content, ref profileSearch, obj.object_limit_profile, ServerInstanceObjects.claimProfileOptions, ServerInstanceObjects.claimProfileIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Size :");
            content.tooltip = "Claim Size ";
            obj.size = EditorGUILayout.Vector3Field(content, obj.size);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (!currencyLoaded)
            {

                ServerCurrency.LoadCurrencyOptions(true);
                currencyLoaded = true;
            }
            if (!itemsLoaded)
            {
                ServerCharacter.LoadItemOptions(true);
                itemsLoaded = true;
            }
            content = new GUIContent("Purchase Currency:");
            content.tooltip = "Purchase Currency:";
            obj.purchaseCurrency = EditorGUILayout.IntPopup(content, obj.purchaseCurrency, ServerCurrency.GuiCurrencyOptions, ServerCurrency.currencyIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            content = new GUIContent("Purchase Cost:");
            content.tooltip = "Cost:";
            obj.cost = EditorGUILayout.LongField(content, obj.cost);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Deed Items");
            content.tooltip = "Deed items";

            
            EditorGUILayout.LabelField(content);
            if (obj.deedItemIDs == null)
                obj.deedItemIDs = new System.Collections.Generic.List<int>();
            for (int i = 0; i < obj.deedItemIDs.Count; i++)
            {
                GUI.backgroundColor = Color.green;
                GUILayout.BeginVertical("", topStyle);
                GUI.backgroundColor = Color.white;
                content = new GUIContent("Deed Item #" + i + " id:");
                content.tooltip = "Deed Item ";
                obj.deedItemIDs[i] = (int) EditorGUILayout.IntField(content, obj.deedItemIDs[i]);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                content = new GUIContent("Deed Item #" + i + " Search:");
                content.tooltip = "Search Item by name";

                if (obj.deedItemSearch.Count <= i)
                {
                    obj.deedItemSearch.Add("");
                }

                obj.deedItemSearch[i] = EditorGUILayout.TextField(content, obj.deedItemSearch[i]);
                content = new GUIContent("Deed Item #" + i + " :");
                content.tooltip = "Deed Item ";

                var search = obj.deedItemSearch[i];
                obj.deedItemIDs[i] = ServerCharacter.GetFilteredListSelector(content, ref search, obj.deedItemIDs[i], ServerCharacter.GuiItemsList, ServerCharacter.itemIds);
                //   obj.deedItemIDs[i] = (int)EditorGUILayout.IntPopup(content, obj.deedItemIDs[i], ServerCharacter.GuiItemsList, ServerCharacter.itemIds);
                obj.deedItemSearch[i] = search;
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                GUILayout.EndVertical();
                GUILayout.Space(2);
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.deedItemIDs.Add(-1);
                obj.deedItemSearch.Add("");
            }
            if (GUILayout.Button("Remove"))
            {
                obj.deedItemIDs.RemoveAt(obj.deedItemIDs.Count - 1);
                obj.deedItemSearch.RemoveAt(obj.deedItemSearch.Count - 1);
            }
            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = Color.red;
            GUILayout.BeginVertical("", topStyle);
            GUI.backgroundColor = Color.white;
            
            content = new GUIContent("Tax");
            content.tooltip = "Tax settings";
            EditorGUILayout.LabelField(content);

            content = new GUIContent("Currency:");
            content.tooltip = "Tax Currency:";
            obj.taxCurrency = EditorGUILayout.IntPopup(content, obj.taxCurrency, ServerCurrency.GuiCurrencyOptions, ServerCurrency.currencyIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            content = new GUIContent("Cost:");
            content.tooltip = "Tax Cost:";
            obj.taxCurrencyAmount = EditorGUILayout.LongField(content, obj.taxCurrencyAmount);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            
            content = new GUIContent("Interval Time (H):");
            content.tooltip = "Tax interval:";
            obj.taxInterval = EditorGUILayout.LongField(content, obj.taxInterval);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            
            content = new GUIContent("Max Time Payment (H):");
            content.tooltip = "The maximum time before the end of paid time in which the next interval can be paid ";
            obj.taxMaxTimePay = EditorGUILayout.LongField(content, obj.taxMaxTimePay);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            
            content = new GUIContent("Min Time to Sell (H):");
            content.tooltip = "The minimum time before the end of the paid time in which the claim can be buyed ";
            obj.taxMinTimeSell = EditorGUILayout.LongField(content, obj.taxMinTimeSell);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


            GUILayout.EndVertical();
            GUILayout.Space(2);
            
            
            
         //  GUILayout.Space(20);
            GUILayout.BeginVertical("Claim Upgrades", topStyle);
            GUILayout.Space(20);
            
            for (int i = 0; i < obj.upgrades.Count; i++)
            {
                GUI.backgroundColor = Color.blue;
                GUILayout.BeginVertical("", topStyle);
                GUI.backgroundColor = Color.white;
               
                content = new GUIContent("Object Limit Profile:");
                content.tooltip = "Object Limit Profile";
                if (upgprofileSearch.Count <= i)
                {
                    upgprofileSearch.Add("");
                }
                upgprofileSearch[i] = EditorGUILayout.TextField(content, upgprofileSearch[i]);

                string search = upgprofileSearch[i];
                obj.upgrades[i].object_limit_profile= ServerCharacter.GetFilteredListSelector(content, ref search, obj.upgrades[i].object_limit_profile, ServerInstanceObjects.claimProfileOptions, ServerInstanceObjects.claimProfileIds);
                //   obj.deedItemIDs[i] = (int)EditorGUILayout.IntPopup(content, obj.deedItemIDs[i], ServerCharacter.GuiItemsList, ServerCharacter.itemIds);
                upgprofileSearch[i] = search;
               
               // obj.upgrades[i].object_limit_profile = EditorGUILayout.IntPopup(content, obj.upgrades[i].object_limit_profile, ServerInstanceObjects.claimProfileOptions, ServerInstanceObjects.claimProfileIds);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                
                if (obj.upgrades.Count == 1 || i == 0)
                {

                    if (obj.upgrades[i].size.x <= obj.transform.lossyScale.x * obj.size.x)
                    {
                        obj.upgrades[i].size.x = obj.transform.lossyScale.x * obj.size.x;
                        obj.upgrades[i].position.x = 0;
                    }

                    if (obj.upgrades[i].size.y <= obj.transform.lossyScale.y * obj.size.y)
                    {
                        obj.upgrades[i].size.y = obj.transform.lossyScale.y * obj.size.y;
                        obj.upgrades[i].position.y = 0;
                    }

                    if (obj.upgrades[i].size.z <= obj.transform.lossyScale.z * obj.size.z)
                    {
                        obj.upgrades[i].size.z = obj.transform.lossyScale.z * obj.size.z;
                        obj.upgrades[i].position.z = 0;
                    }

                }
                else
                {
                    if (obj.upgrades[i].size.x <= obj.upgrades[i - 1].size.x)
                    {
                        obj.upgrades[i].size.x = obj.upgrades[i - 1].size.x;
                        obj.upgrades[i].position.x = obj.upgrades[i - 1].position.x;
                    }

                    if (obj.upgrades[i].size.y <= obj.upgrades[i - 1].size.y)
                    {
                        obj.upgrades[i].size.y = obj.upgrades[i - 1].size.y;
                        obj.upgrades[i].position.y = obj.upgrades[i - 1].position.y;
                    }

                    if (obj.upgrades[i].size.z <= obj.upgrades[i - 1].size.z)
                    {
                        obj.upgrades[i].size.z = obj.upgrades[i - 1].size.z;
                        obj.upgrades[i].position.z = obj.upgrades[i - 1].position.z;
                    }

                }

                content = new GUIContent("New Size :");
                content.tooltip = " ";
                obj.upgrades[i].size = EditorGUILayout.Vector3Field(content, obj.upgrades[i].size);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Snap +X"))
                {
                    if (obj.upgrades.Count == 1 || i == 0)
                    {
                        if (obj.upgrades[i].size.x > obj.size.x)
                        {
                            obj.upgrades[i].position.x = (obj.upgrades[i].size.x - obj.size.x) / 2f;
                        }
                        else
                            obj.upgrades[i].position.x = 0;
                    }
                    else
                    {
                        if (obj.upgrades[i].size.x > obj.upgrades[i - 1].size.x)
                            obj.upgrades[i].position.x = (obj.upgrades[i].size.x - obj.upgrades[i - 1].size.x) / 2 + obj.upgrades[i - 1].position.x;
                        else
                            obj.upgrades[i].position.x = obj.upgrades[i - 1].position.x;
                    }
                }

                if (GUILayout.Button("Snap center X"))
                {
                    if (obj.upgrades.Count == 1 || i == 0)
                    {
                        obj.upgrades[i].position.x = 0;

                    }
                    else
                    {
                        obj.upgrades[i].position.x = obj.upgrades[i - 1].position.x;
                    }
                }

                if (GUILayout.Button("Snap -X"))
                {
                    if (obj.upgrades.Count == 1 || i == 0)
                    {

                        if (obj.upgrades[i].size.x > obj.size.x)
                        {
                            obj.upgrades[i].position.x = -(obj.upgrades[i].size.x - obj.size.x) / 2f;
                        }
                        else
                            obj.upgrades[i].position.x = 0;
                    }
                    else
                    {
                        if (obj.upgrades[i].size.x > obj.upgrades[i - 1].size.x)
                            obj.upgrades[i].position.x = -(obj.upgrades[i].size.x - obj.upgrades[i - 1].size.x) / 2 + obj.upgrades[i - 1].position.x;
                        else
                            obj.upgrades[i].position.x = obj.upgrades[i - 1].position.x;
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Snap +Y"))
                {
                    if (obj.upgrades.Count == 1 || i == 0)
                    {
                        if (obj.upgrades[i].size.y > obj.transform.lossyScale.y * obj.size.y)
                        {
                            obj.upgrades[i].position.y = (obj.upgrades[i].size.y - obj.size.y) / 2f;
                        }
                        else
                            obj.upgrades[i].position.y = 0;
                    }
                    else
                    {
                        if (obj.upgrades[i].size.y > obj.upgrades[i - 1].size.y)
                            obj.upgrades[i].position.y = (obj.upgrades[i].size.y - obj.upgrades[i - 1].size.y) / 2 + obj.upgrades[i - 1].position.y;
                        else
                            obj.upgrades[i].position.y = obj.upgrades[i - 1].position.y;
                    }
                }

                if (GUILayout.Button("Snap center Y"))
                {
                    if (obj.upgrades.Count == 1 || i == 0)
                    {
                        obj.upgrades[i].position.y = 0;
                    }
                    else
                    {
                        obj.upgrades[i].position.y = obj.upgrades[i - 1].position.y;
                    }
                }

                if (GUILayout.Button("Snap -Y"))
                {
                    if (obj.upgrades.Count == 1 || i == 0)
                    {
                        if (obj.upgrades[i].size.y > obj.size.y)
                        {
                            obj.upgrades[i].position.y = -(obj.upgrades[i].size.y - obj.size.y) / 2f;
                        }
                        else
                            obj.upgrades[i].position.y = 0;
                    }
                    else
                    {
                        if (obj.upgrades[i].size.y > obj.upgrades[i - 1].size.y)
                            obj.upgrades[i].position.y = -(obj.upgrades[i].size.y - obj.upgrades[i - 1].size.y) / 2 + obj.upgrades[i - 1].position.y;
                        else
                            obj.upgrades[i].position.y = obj.upgrades[i - 1].position.y;
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Snap +Z"))
                {
                    if (obj.upgrades.Count == 1 || i == 0)
                    {
                        if (obj.upgrades[i].size.z > obj.size.z)
                        {
                            obj.upgrades[i].position.z = (obj.upgrades[i].size.z - obj.size.z) / 2f;
                        }
                        else
                            obj.upgrades[i].position.z = 0;
                    }
                    else
                    {
                        if (obj.upgrades[i].size.z > obj.upgrades[i - 1].size.z)
                            obj.upgrades[i].position.z = (obj.upgrades[i].size.z - obj.upgrades[i - 1].size.z) / 2 + obj.upgrades[i - 1].position.z;
                        else
                            obj.upgrades[i].position.z = obj.upgrades[i - 1].position.z;
                    }
                }

                if (GUILayout.Button("Snap center Z"))
                {
                    if (obj.upgrades.Count == 1 || i == 0)
                    {
                        obj.upgrades[i].position.z = 0;
                    }
                    else
                    {
                        obj.upgrades[i].position.z = obj.upgrades[i - 1].position.z;
                    }
                }

                if (GUILayout.Button("Snap -Z"))
                {
                    if (obj.upgrades.Count == 1 || i == 0)
                    {
                        if (obj.upgrades[i].size.z > obj.size.z)
                        {
                            obj.upgrades[i].position.z = -(obj.upgrades[i].size.z - obj.size.z) / 2f;
                        }
                        else
                            obj.upgrades[i].position.z = 0;
                    }
                    else
                    {
                        if (obj.upgrades[i].size.z > obj.upgrades[i - 1].size.z)
                            obj.upgrades[i].position.z = -(obj.upgrades[i].size.z - obj.upgrades[i - 1].size.z) / 2 + obj.upgrades[i - 1].position.z;
                        else
                            obj.upgrades[i].position.z = obj.upgrades[i - 1].position.z;
                    }
                }

                EditorGUILayout.EndHorizontal();
                content = new GUIContent("Position :");
                content.tooltip = " ";
                obj.upgrades[i].position = EditorGUILayout.Vector3Field(content, obj.upgrades[i].position);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                content = new GUIContent("Currency:");
                content.tooltip = "Upgrade Currency:";
                obj.upgrades[i].currency = EditorGUILayout.IntPopup(content, obj.upgrades[i].currency, ServerCurrency.GuiCurrencyOptions, ServerCurrency.currencyIds);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                content = new GUIContent("Upgrade Cost:");
                content.tooltip = "Upgrade Cost:";
                obj.upgrades[i].currencyAmount = EditorGUILayout.LongField(content, obj.upgrades[i].currencyAmount);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                content = new GUIContent("Deed Items");
                content.tooltip = "Deed items";

                EditorGUILayout.LabelField(content);
                if (obj.upgrades[i].deedItemIDs == null)
                    obj.upgrades[i].deedItemIDs = new System.Collections.Generic.List<int>();
                for (int k = 0; k < obj.upgrades[i].deedItemIDs.Count; k++)
                {
                    GUI.backgroundColor = Color.green;
                    GUILayout.BeginVertical("", topStyle);
                    GUI.backgroundColor = Color.white;
                    content = new GUIContent("Deed Item #" + k + " id:");
                    content.tooltip = "Deed Item ";
                    obj.upgrades[i].deedItemIDs[k] = (int)EditorGUILayout.IntField(content, obj.upgrades[i].deedItemIDs[k]);
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                    content = new GUIContent("Deed Item #" + i + " Search:");
                    content.tooltip = "Search Item by name";

                    if (obj.upgrades[i].deedItemSearch.Count <= i)
                    {
                        obj.upgrades[i].deedItemSearch.Add("");
                    }

                    obj.upgrades[i].deedItemSearch[k] = EditorGUILayout.TextField(content, obj.upgrades[i].deedItemSearch[k]);
                    content = new GUIContent("Deed Item #" + i + " :");
                    content.tooltip = "Deed Item ";

                     search = obj.upgrades[i].deedItemSearch[k];
                    obj.upgrades[i].deedItemIDs[k] = ServerCharacter.GetFilteredListSelector(content, ref search, obj.upgrades[i].deedItemIDs[k], ServerCharacter.GuiItemsList, ServerCharacter.itemIds);
                    //   obj.deedItemIDs[i] = (int)EditorGUILayout.IntPopup(content, obj.deedItemIDs[i], ServerCharacter.GuiItemsList, ServerCharacter.itemIds);
                    obj.upgrades[i].deedItemSearch[k] = search;
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                    GUILayout.EndVertical();
                    GUILayout.Space(2);
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add"))
                {
                    obj.upgrades[i].deedItemIDs.Add(-1);
                    obj.upgrades[i].deedItemSearch.Add("");
                }
                if (GUILayout.Button("Remove"))
                {
                    obj.upgrades[i].deedItemIDs.RemoveAt(obj.upgrades[i].deedItemIDs.Count - 1);
                    obj.upgrades[i].deedItemSearch.RemoveAt(obj.upgrades[i].deedItemSearch.Count - 1);
                    
                }
                EditorGUILayout.EndHorizontal();
                
                
                GUI.backgroundColor = Color.red;
                GUILayout.BeginVertical("", topStyle);
                GUI.backgroundColor = Color.white;

                content = new GUIContent("Tax");
                content.tooltip = "Tax settings";
                EditorGUILayout.LabelField(content);

                content = new GUIContent("Currency:");
                content.tooltip = "Tax Currency:";
                obj.upgrades[i].taxCurrency = EditorGUILayout.IntPopup(content, obj.upgrades[i].taxCurrency, ServerCurrency.GuiCurrencyOptions, ServerCurrency.currencyIds);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                content = new GUIContent("Cost:");
                content.tooltip = "Tax Cost:";
                obj.upgrades[i].taxCurrencyAmount = EditorGUILayout.LongField(content, obj.upgrades[i].taxCurrencyAmount);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            
                content = new GUIContent("Interval Time (H):");
                content.tooltip = "Tax interval:";
                obj.upgrades[i].taxInterval = EditorGUILayout.LongField(content, obj.upgrades[i].taxInterval);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            
                content = new GUIContent("Max Time Payment (H):");
                content.tooltip = "The maximum time before the end of paid time in which the next interval can be paid ";
                obj.upgrades[i].taxMaxTimePay = EditorGUILayout.LongField(content, obj.upgrades[i].taxMaxTimePay);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            
                content = new GUIContent("Min Time to Sell (H):");
                content.tooltip = "The minimum time before the end of the paid time in which the claim can be buyed ";
                obj.upgrades[i].taxMinTimeSell = EditorGUILayout.LongField(content, obj.upgrades[i].taxMinTimeSell);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


                GUILayout.EndVertical();
                GUILayout.Space(2);

                content = new GUIContent("Gizmo Color:");
                content.tooltip = "Gizmo Color";
                obj.upgrades[i].color = EditorGUILayout.ColorField(content, obj.upgrades[i].color);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                GUILayout.EndVertical();
                GUILayout.Space(5);

            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Claim Upgrade"))
            {
                obj.upgrades.Add(new AtavismClaimUpgrade());
                upgprofileSearch.Add("");
            }

            if (obj.upgrades.Count > 0)
            {
                if (GUILayout.Button("Remove Last"))
                {
                    obj.upgrades.RemoveAt(obj.upgrades.Count - 1);
                    upgprofileSearch.RemoveAt(upgprofileSearch.Count - 1);
                }
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndVertical();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(obj);
                EditorUtility.SetDirty(target);
            }
        }

        private int GetPositionOfInteraction(string interactionType)
        {
            for (int i = 0; i < interactionTypes.Length; i++)
            {
                if (interactionTypes[i] == interactionType)
                    return i;
            }
            return 0;
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