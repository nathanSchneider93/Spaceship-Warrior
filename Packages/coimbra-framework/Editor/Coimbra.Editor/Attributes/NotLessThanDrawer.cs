using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    [CustomPropertyDrawer(typeof(NotLessThanAttribute))]
    public sealed class NotLessThanDrawer : PropertyAttributeDrawer<NotLessThanAttribute>
    {
        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                {
                    int minValue = Mathf.FloorToInt(Attribute.Value);

                    int SetValue(int value)
                    {
                        return Mathf.Max(value, minValue);
                    }

                    property.SetValues<int>(SetValue);

                    using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
                    {
                        using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                        {
                            int value = EditorGUI.IntField(position, propertyScope.content, property.intValue);

                            if (changeCheckScope.changed)
                            {
                                property.intValue = Mathf.Max(value, minValue);
                            }
                        }
                    }

                    break;
                }

                case SerializedPropertyType.Float:
                {
                    float SetValue(float value)
                    {
                        return Mathf.Max(value, Attribute.Value);
                    }

                    property.SetValues<float>(SetValue);

                    using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
                    {
                        using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                        {
                            float value = EditorGUI.FloatField(position, propertyScope.content, property.floatValue);

                            if (changeCheckScope.changed)
                            {
                                property.floatValue = Mathf.Max(value, Attribute.Value);
                            }
                        }
                    }

                    break;
                }

                default:
                {
                    EditorGUI.LabelField(position, label.text, "Use Min with int or float.");

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
