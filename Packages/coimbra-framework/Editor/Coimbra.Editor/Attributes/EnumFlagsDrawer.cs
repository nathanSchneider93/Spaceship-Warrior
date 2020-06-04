using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public sealed class EnumFlagsDrawer : PropertyAttributeDrawer<EnumFlagsAttribute>
    {
        private static readonly Dictionary<System.Type, int> Offset = new Dictionary<System.Type, int>();

        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorGUI.LabelField(position, label.text, "Use EnumMask with flags Enum.");

                return;
            }

            var enumValue = property.GetValue<System.Enum>();
            System.Type enumType = enumValue.GetType();

            if (enumType.IsDefined(typeof(System.FlagsAttribute), false) == false)
            {
                EditorGUI.LabelField(position, label.text, "Use EnumMask with flags Enum.");

                return;
            }

            using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
            {
                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    System.Enum value = EditorGUI.EnumFlagsField(position, propertyScope.content, enumValue);

                    if (changeCheckScope.changed)
                    {
                        int valueIndex = System.Convert.ToInt32(value);

                        if (valueIndex < -1)
                        {
                            if (Offset.TryGetValue(enumType, out int offset) == false)
                            {
                                offset = System.Enum.GetValues(enumType).Cast<int>().Max() * 2;
                                Offset.Add(enumType, offset);
                            }

                            valueIndex += offset;
                        }

                        property.intValue = valueIndex;
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
