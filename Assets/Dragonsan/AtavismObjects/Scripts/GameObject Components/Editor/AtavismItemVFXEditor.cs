using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Atavism
{
     [CustomEditor(typeof(AtavismItemVFX))]
    public class AtavismItemVFXEditor : Editor
    {

        private bool itemsLoaded = false;
        bool help = false;


        public override void OnInspectorGUI()
        {
          //  var indentOffset = EditorGUI.indentLevel * 5f;
            var lineRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(lineRect.x, lineRect.y, EditorGUIUtility.currentViewWidth - 60f, lineRect.height);
            var fieldRect = new Rect(labelRect.xMax, lineRect.y, lineRect.width - labelRect.width - 60f, lineRect.height);
            var buttonRect = new Rect(fieldRect.xMax, lineRect.y, 60f, lineRect.height);
            AtavismItemVFX obj = target as AtavismItemVFX;

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
            GUILayout.BeginVertical("Atavism VFX Configuration", topStyle);
            GUILayout.Space(25);
            
            
            GUILayout.BeginVertical("Enchant", topStyle);
            GUILayout.Space(20);
            for (int j = 0; j < obj.enchantsObject.Count; j++)
            { 
                GUI.backgroundColor = Color.red;
                GUILayout.BeginVertical("Level "+(j+1), topStyle);
                GUILayout.Space(20);
                GUI.backgroundColor = Color.white;
                content = new GUIContent("Effect Object");
                content.tooltip = "Assign the Object that will be enabled";
                obj.enchantsObject[j] = (GameObject)EditorGUILayout.ObjectField(content, obj.enchantsObject[j], typeof(GameObject), true);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);                
                
                
                content = new GUIContent("Effect Prefab");
                content.tooltip = "Assign Prefab Object that will be instantiated";
                obj.enchantsPrefab[j] = (GameObject)EditorGUILayout.ObjectField(content, obj.enchantsPrefab[j], typeof(GameObject), true);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                
                content = new GUIContent("Disable Object");
                content.tooltip = "Assign Prefab Object that will be disabled";
                obj.enchantsDisableObject[j] = (GameObject)EditorGUILayout.ObjectField(content, obj.enchantsDisableObject[j], typeof(GameObject), true);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                GUILayout.EndVertical();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.enchantsObject.Add(null);
                obj.enchantsDisableObject.Add(null);
                obj.enchantsPrefab.Add(null);
            }

            if (GUILayout.Button("Remove Last"))
            {
                obj.enchantsObject.RemoveAt(obj.enchantsObject.Count - 1);
                obj.enchantsDisableObject.RemoveAt(obj.enchantsDisableObject.Count - 1);
                obj.enchantsPrefab.RemoveAt(obj.enchantsPrefab.Count - 1);
            }
            EditorGUILayout.EndHorizontal();
            
            
            GUILayout.EndVertical();
            
            GUILayout.BeginVertical("Sockets", topStyle);
            GUILayout.Space(20);
            if (!itemsLoaded)
            {
                ServerCharacter.LoadItemOptions(true);
                itemsLoaded = true;
            }
           
            for (int j = 0; j < obj.socketEffects.Count; j++)
            {
                GUI.backgroundColor = Color.red;
                GUILayout.BeginVertical("", topStyle);
                GUI.backgroundColor = Color.white;
                content = new GUIContent("Items");
                content.tooltip = "items";
                EditorGUILayout.LabelField(content);
                if (obj.socketEffects[j].items == null)
                    obj.socketEffects[j].items = new System.Collections.Generic.List<int>();
                if (obj.socketEffects[j].items.Count == 0)
                {
                    obj.socketEffects[j].items.Add(-1);
                    obj.socketEffects[j].search.Add("");
                }
                for (int i = 0; i < obj.socketEffects[j].items.Count; i++)
                {
                    GUI.backgroundColor = Color.green;
                    GUILayout.BeginVertical("", topStyle);
                    GUI.backgroundColor = Color.white;
                    content = new GUIContent("Item #" + i + " id:");
                    content.tooltip = "Item ";
                    obj.socketEffects[j].items[i] = (int) EditorGUILayout.IntField(content, obj.socketEffects[j].items[i]);
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                    content = new GUIContent("Item #" + i + " Search:");
                    content.tooltip = "Search Item by name";

                    if (obj.socketEffects[j].search.Count <= i)
                    {
                        obj.socketEffects[j].search.Add("");
                    }

                    obj.socketEffects[j].search[i] = EditorGUILayout.TextField(content, obj.socketEffects[j].search[i]);
                    content = new GUIContent("Item #" + i + " :");
                    content.tooltip = "Item ";

                    var search = obj.socketEffects[j].search[i];
                    obj.socketEffects[j].items[i] = ServerCharacter.GetFilteredListSelector(content, ref search, obj.socketEffects[j].items[i], ServerCharacter.GuiItemsList, ServerCharacter.itemIds);
                    //   obj.deedItemIDs[i] = (int)EditorGUILayout.IntPopup(content, obj.deedItemIDs[i], ServerCharacter.GuiItemsList, ServerCharacter.itemIds);
                    obj.socketEffects[j].search[i] = search;
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                    GUILayout.EndVertical();
                    GUILayout.Space(2);
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add item"))
                {
                    obj.socketEffects[j].items.Add(-1);
                    obj.socketEffects[j].search.Add("");
                }

                if (GUILayout.Button("Remove item"))
                {
                    obj.socketEffects[j].items.RemoveAt(obj.socketEffects[j].items.Count - 1);
                    obj.socketEffects[j].search.RemoveAt(obj.socketEffects[j].search.Count - 1);
                }
                EditorGUILayout.EndHorizontal();
                

                content = new GUIContent("Effect Object");
                content.tooltip = "Assign the Object that will be enabled";
                obj.socketEffects[j].effectObject = (GameObject)EditorGUILayout.ObjectField(content, obj.socketEffects[j].effectObject, typeof(GameObject), true);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);                
                
                
                content = new GUIContent("Effect Prefab");
                content.tooltip = "Assign Prefab Object that will be instantiated";
                obj.socketEffects[j].effectPrefab = (GameObject)EditorGUILayout.ObjectField(content, obj.socketEffects[j].effectPrefab, typeof(GameObject), true);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                
                content = new GUIContent("Disable  Object");
                content.tooltip = "Assign the Object that will be disabled";
                obj.socketEffects[j].disableObject = (GameObject)EditorGUILayout.ObjectField(content, obj.socketEffects[j].disableObject, typeof(GameObject), true);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);                
                GUILayout.EndVertical();
                GUILayout.Space(2);
            }
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.socketEffects.Add(new ItemSocketEffect());
            }

            if (GUILayout.Button("Remove Last"))
            {
                obj.socketEffects.RemoveAt(obj.socketEffects.Count - 1);
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
    }
}