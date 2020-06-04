using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    [CustomPropertyDrawer(typeof(RangeSliderAttribute))]
    public sealed class RangeSliderDrawer : PropertyAttributeDrawer<RangeSliderAttribute>
    {
        private enum Type
        {
            Vector2,
            Vector2Int,
            FloatRange,
            IntRange
        }

        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Type type;

            switch (property.propertyType)
            {
                case SerializedPropertyType.Vector2:
                {
                    type = Type.Vector2;

                    break;
                }

                case SerializedPropertyType.Vector2Int:
                {
                    type = Type.Vector2Int;

                    break;
                }

                default:
                {
                    if (property.type == typeof(FloatRange).Name)
                    {
                        type = Type.FloatRange;
                    }
                    else if (property.type == typeof(IntRange).Name)
                    {
                        type = Type.IntRange;
                    }
                    else
                    {
                        EditorGUI.LabelField(position, label.text, "Use RangeSlider with Vector2 or Vector2Int or FloatRange or IntRange.");

                        return;
                    }

                    break;
                }
            }

            using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
            {
                position.height = EditorGUIUtility.singleLineHeight;

                Rect labelPosition = position;
                labelPosition.width = EditorGUIUtility.labelWidth;

                EditorGUI.LabelField(labelPosition, propertyScope.content);

                using (new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel))
                {
                    position.x += labelPosition.width;
                    position.width -= labelPosition.width;

                    switch (type)
                    {
                        case Type.Vector2:
                        {
                            Vector2 SetValue(Vector2 value)
                            {
                                float min = Mathf.Max(Mathf.Min(value.x, value.y), Attribute.Min);
                                float max = Mathf.Min(Mathf.Max(value.x, value.y), Attribute.Max);

                                return new Vector2(min, max);
                            }

                            property.SetValues<Vector2>(SetValue);

                            SerializedProperty minProperty = property.FindPropertyRelative("x");
                            SerializedProperty maxProperty = property.FindPropertyRelative("y");
                            OnGUI(position, minProperty, maxProperty, Attribute.Min, Attribute.Max);

                            break;
                        }

                        case Type.Vector2Int:
                        {
                            int minLimit = Mathf.FloorToInt(Attribute.Min);
                            int maxLimit = Mathf.FloorToInt(Attribute.Max);

                            Vector2Int SetValue(Vector2Int value)
                            {
                                int min = Mathf.Max(Mathf.Min(value.x, value.y), minLimit);
                                int max = Mathf.Min(Mathf.Max(value.x, value.y), maxLimit);

                                return new Vector2Int(min, max);
                            }

                            property.SetValues<Vector2Int>(SetValue);

                            SerializedProperty minProperty = property.FindPropertyRelative("x");
                            SerializedProperty maxProperty = property.FindPropertyRelative("y");
                            OnGUI(position, minProperty, maxProperty, minLimit, maxLimit);

                            break;
                        }

                        case Type.FloatRange:
                        {
                            FloatRange SetValue(FloatRange value)
                            {
                                float min = Mathf.Max(value.Min, Attribute.Min);
                                float max = Mathf.Min(value.Max, Attribute.Max);

                                return new FloatRange(min, max);
                            }

                            property.SetValues<FloatRange>(SetValue);

                            SerializedProperty minProperty = property.FindPropertyRelative("m_Min");
                            SerializedProperty maxProperty = property.FindPropertyRelative("m_Max");
                            OnGUI(position, minProperty, maxProperty, Attribute.Min, Attribute.Max);

                            break;
                        }

                        case Type.IntRange:
                        {
                            int minLimit = Mathf.FloorToInt(Attribute.Min);
                            int maxLimit = Mathf.FloorToInt(Attribute.Max);

                            IntRange SetValue(IntRange value)
                            {
                                int min = Mathf.Max(value.Min, minLimit);
                                int max = Mathf.Min(value.Max, maxLimit);

                                return new IntRange(min, max);
                            }

                            property.SetValues<IntRange>(SetValue);

                            SerializedProperty minProperty = property.FindPropertyRelative("m_Min");
                            SerializedProperty maxProperty = property.FindPropertyRelative("m_Max");
                            OnGUI(position, minProperty, maxProperty, minLimit, maxLimit);

                            break;
                        }
                    }
                }
            }
        }

        protected override float GetHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        private void OnGUI(Rect position, SerializedProperty minProperty, SerializedProperty maxProperty, int minLimit, int maxLimit)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            const float spacing = 4;
            const float fieldWidth = 50;
            float totalWidth = position.width;

            using (new EditorGUI.PropertyScope(position, GUIContent.none, minProperty))
            {
                position.width = fieldWidth;

                using (var minCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    int value = Attribute.Delayed
                                    ? EditorGUI.DelayedIntField(position, minProperty.intValue)
                                    : EditorGUI.IntField(position, minProperty.intValue);

                    if (minCheckScope.changed)
                    {
                        minProperty.intValue = Mathf.Clamp(value, minLimit, maxLimit);
                        maxProperty.intValue = Mathf.Max(maxProperty.intValue, minProperty.intValue);
                    }
                }
            }

            using (new ShowMixedValueScope(minProperty.hasMultipleDifferentValues || maxProperty.hasMultipleDifferentValues))
            {
                position.x += position.width + spacing;
                position.width = totalWidth - fieldWidth * 2 - spacing * 2;

                using (var sliderCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    float min = minProperty.hasMultipleDifferentValues ? minLimit : minProperty.intValue;
                    float max = maxProperty.hasMultipleDifferentValues ? maxLimit : maxProperty.intValue;

                    EditorGUI.MinMaxSlider(position, ref min, ref max, minLimit, maxLimit);

                    if (sliderCheckScope.changed)
                    {
                        minProperty.intValue = Mathf.RoundToInt(min);
                        maxProperty.intValue = Mathf.RoundToInt(max);
                    }
                }
            }

            using (new EditorGUI.PropertyScope(position, GUIContent.none, maxProperty))
            {
                position.x += position.width + spacing;
                position.width = fieldWidth;

                using (var maxCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    int value = Attribute.Delayed
                                    ? EditorGUI.DelayedIntField(position, maxProperty.intValue)
                                    : EditorGUI.IntField(position, maxProperty.intValue);

                    if (maxCheckScope.changed)
                    {
                        maxProperty.intValue = Mathf.Clamp(value, minProperty.intValue, maxLimit);
                    }
                }
            }
        }

        private void OnGUI(Rect position, SerializedProperty minProperty, SerializedProperty maxProperty, float minLimit, float maxLimit)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            const float spacing = 4;
            const float fieldWidth = 50;
            float totalWidth = position.width;

            using (new EditorGUI.PropertyScope(position, GUIContent.none, minProperty))
            {
                position.width = fieldWidth;

                using (var minCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = Attribute.Delayed
                                      ? EditorGUI.DelayedFloatField(position, minProperty.floatValue)
                                      : EditorGUI.FloatField(position, minProperty.floatValue);

                    if (minCheckScope.changed)
                    {
                        minProperty.floatValue = Mathf.Clamp(value, minLimit, maxLimit);
                        maxProperty.floatValue = Mathf.Max(maxProperty.floatValue, minProperty.floatValue);
                    }
                }
            }

            using (new ShowMixedValueScope(minProperty.hasMultipleDifferentValues || maxProperty.hasMultipleDifferentValues))
            {
                position.x += position.width + spacing;
                position.width = totalWidth - fieldWidth * 2 - spacing * 2;

                using (var sliderCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    float min = minProperty.hasMultipleDifferentValues ? minLimit : minProperty.floatValue;
                    float max = maxProperty.hasMultipleDifferentValues ? maxLimit : maxProperty.floatValue;

                    EditorGUI.MinMaxSlider(position, ref min, ref max, minLimit, maxLimit);

                    if (sliderCheckScope.changed)
                    {
                        minProperty.floatValue = Mathf.Round(min * 100) / 100;
                        maxProperty.floatValue = Mathf.Round(max * 100) / 100;
                    }
                }
            }

            using (new EditorGUI.PropertyScope(position, GUIContent.none, maxProperty))
            {
                position.x += position.width + spacing;
                position.width = fieldWidth;

                using (var maxCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    float value = Attribute.Delayed
                                      ? EditorGUI.DelayedFloatField(position, maxProperty.floatValue)
                                      : EditorGUI.FloatField(position, maxProperty.floatValue);

                    if (maxCheckScope.changed)
                    {
                        maxProperty.floatValue = Mathf.Clamp(value, minProperty.floatValue, maxLimit);
                    }
                }
            }
        }
    }
}
