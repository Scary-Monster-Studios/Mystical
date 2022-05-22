using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Atavism
{
    [CustomEditor(typeof(Inventory))]
    public class InventoryEditor : Editor
    {
        bool help = false;
        string[] currencyGroup;
        int[] currencyGroupIds;

        public override void OnInspectorGUI()
        {
            Inventory obj = target as Inventory;
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
            GUILayout.BeginVertical("Inventory Configuration", topStyle);
            GUILayout.Space(20);
          
            content = new GUIContent("Sell factor");
            content.tooltip = "Sell factor";
            obj.sellFactor = EditorGUILayout.FloatField(content, obj.sellFactor);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            if (currencyGroup == null)
            {
                ServerOptionChoices.LoadAtavismChoiceOptions("Currency Group", false, out currencyGroupIds, out currencyGroup);
            }
            if (currencyGroup.Length == 0)
            {
                EditorGUILayout.LabelField("!! Currency Group is not loaded check database configuration in Old Atavism Editor !!");
                GUILayout.EndVertical();
                return;
            }

            content = new GUIContent("Main Currency Group");
            content.tooltip = "Select main currency group";
            int selected = GetOptionPosition(obj.mainCurrencyGroup, currencyGroupIds);
            selected = EditorGUILayout.Popup(content, selected, currencyGroup);
            obj.mainCurrencyGroup = currencyGroupIds[selected];
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


            GUILayout.Space(2);

            GUILayout.EndVertical();
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