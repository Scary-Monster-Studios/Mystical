using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Atavism
{
      [CustomEditor(typeof(ResourceDrop))]
    public class ResourceDropEditor : Editor
    {

     //   private bool effectsLoaded = false;
      //  private bool questsLoaded = false;
      //  private bool tasksLoaded = false;
      //  private bool instancesLoaded = false;
      //  string[] interactionTypes;
        bool help = false;
        private bool itemsLoaded = false;
        private string search = "";
        //  int type = 0;
        public override void OnInspectorGUI()
        {
            //    var indentOffset = EditorGUI.indentLevel * 5f;
        /*    var lineRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(lineRect.x, lineRect.y, EditorGUIUtility.currentViewWidth - 60f, lineRect.height);
            var fieldRect = new Rect(labelRect.xMax, lineRect.y, lineRect.width - labelRect.width - 60f, lineRect.height);
            var buttonRect = new Rect(fieldRect.xMax, lineRect.y, 60f, lineRect.height);
            ResourceDrop obj = target as ResourceDrop;

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
            GUILayout.BeginVertical("Atavism Resource Drop Configuration", topStyle);
            GUILayout.Space(20);
            if (!itemsLoaded)
            {
                ServerCharacter.LoadItemOptions(true);
                itemsLoaded = true;
            }

            content = new GUIContent("Item");
            content.tooltip = "Select Item";
            content = new GUIContent("Drop Item id:");
            content.tooltip = "Drop Item ";
            obj.itemId = (int)EditorGUILayout.IntField(content, obj.itemId);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            content = new GUIContent("Search:");
            content.tooltip = "Search Item ";
            search = EditorGUILayout.TextField(content, search);
            content = new GUIContent("Drop Item:");
            content.tooltip = "Deed Item ";

            obj.itemId = ServerCharacter.GetFilteredListSelector(content, ref search, obj.itemId, ServerCharacter.GuiItemsList, ServerCharacter.itemIds);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


            content = new GUIContent("Min");
            content.tooltip = "Min";
            obj.min = EditorGUILayout.IntField(content, obj.min);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Max");
            content.tooltip = "Max";
            obj.max = EditorGUILayout.IntField(content, obj.max);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


            content = new GUIContent("Chance");
            content.tooltip = "Drop Chance";
            obj.chance = EditorGUILayout.FloatField(content, obj.chance);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            GUILayout.EndVertical();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(obj);
                EditorUtility.SetDirty(target);
            }*/
        }
    }
}