using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Coimbra
{
    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public sealed class TagSelectorDrawer : PropertyAttributeDrawer<TagSelectorAttribute>
    {
        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use TagSelector with string.");

                return;
            }

            string[] tags = InternalEditorUtility.tags;

            string SetValue(string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return tags[0];
                }

                for (int i = 0; i < tags.Length; i++)
                {
                    if (value == tags[i])
                    {
                        return value;
                    }
                }

                return tags[0];
            }

            property.SetValues<string>(SetValue);

            using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
            {
                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                     string value = EditorGUI.TagField(position, propertyScope.content, property.hasMultipleDifferentValues ? "-" : property.stringValue);

                    if (changeCheckScope.changed)
                    {
                        property.stringValue = value;
                    }
                }
            }
        }

        protected override float GetHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
