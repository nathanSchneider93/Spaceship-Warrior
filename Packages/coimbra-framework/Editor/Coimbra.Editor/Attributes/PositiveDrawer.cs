using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    [CustomPropertyDrawer(typeof(PositiveAttribute))]
    public sealed class PositiveDrawer : PropertyAttributeDrawer<PositiveAttribute>
    {
        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                {
                    property.SetValues((int value) => Mathf.Abs(value));

                    using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
                    {
                        using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                        {
                            int value = EditorGUI.IntField(position, propertyScope.content, property.intValue);

                            if (changeCheckScope.changed)
                            {
                                property.intValue = Mathf.Abs(value);
                            }
                        }
                    }

                    break;
                }

                case SerializedPropertyType.Float:
                {
                    property.SetValues((float value) => Mathf.Abs(value));

                    using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
                    {
                        using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                        {
                            float value = EditorGUI.FloatField(position, propertyScope.content, property.floatValue);

                            if (changeCheckScope.changed)
                            {
                                property.floatValue = Mathf.Abs(value);
                            }
                        }
                    }

                    break;
                }

                default:
                {
                    EditorGUI.LabelField(position, label.text, "Use Positive with int or float.");

                    break;
                }
            }
        }

        protected override float GetHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
