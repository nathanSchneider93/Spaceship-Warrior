using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    [CustomPropertyDrawer(typeof(ReorderableAttribute))]
    public sealed class ReorderableDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReorderableList reorderableList = ReorderableList.GetReorderableList(property, attribute as ReorderableAttribute);

            if (reorderableList != null)
            {
                reorderableList.DrawGUI(EditorGUI.IndentedRect(position), label);
            }
            else
            {
                GUI.Label(position, $"{label.text} must be a SerializableList", EditorStyles.label);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ReorderableList.GetReorderableList(property, attribute as ReorderableAttribute)?.TotalHeight ?? EditorGUIUtility.singleLineHeight;
        }
    }
}
