using UnityEditor;
using UnityEngine;

namespace Atavism
{
    [CustomPropertyDrawer(typeof(AtavismSeparatorAttribute))]
    public class AtavismSeparatorDrawer : DecoratorDrawer
    {
        AtavismSeparatorAttribute atavismSeparatorAttribute { get { return ((AtavismSeparatorAttribute)attribute); } }


        public override void OnGUI(Rect _position)
        {
            if (atavismSeparatorAttribute.title == "")
            {
                _position.height = 1;
                _position.y += 19;
                GUI.Box(_position, "");
            }
            else
            {
                Vector2 textSize = GUI.skin.label.CalcSize(new GUIContent(atavismSeparatorAttribute.title));
                textSize = textSize * 2;
                _position.y += 19;
                GUI.Box(new Rect(_position.xMin - 10, _position.yMin - 8.0f, _position.width + 15, 20), atavismSeparatorAttribute.title, EditorStyles.toolbarButton);

            }
        }

        public override float GetHeight()
        {
            return 41.0f;
        }
    }
}