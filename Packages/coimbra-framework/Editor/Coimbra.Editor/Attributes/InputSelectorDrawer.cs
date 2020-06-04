using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    [CustomPropertyDrawer(typeof(InputSelectorAttribute))]
    public sealed class InputSelectorDrawer : PropertyAttributeDrawer<InputSelectorAttribute>
    {
        private static readonly List<string> Options = new List<string>();

        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use InputSelector with string.");

                return;
            }

            Options.Clear();

            foreach (SerializedProperty inputProperty in new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]).FindProperty("m_Axes"))
            {
                string name = inputProperty.FindPropertyRelative("m_Name").stringValue;

                if (Options.Contains(name) == false)
                {
                    Options.Add(name);
                }
            }

            if (Options.Count == 0)
            {
                EditorGUI.LabelField(position, label.text, "No Input added to input manager.");

                return;
            }

            string SetValue(string value)
            {
                if (string.IsNullOrEmpty(value) || Options.Contains(value) == false)
                {
                    return Options[0];
                }

                return value;
            }

            property.SetValues<string>(SetValue);

            using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
            {
                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    int value = EditorGUI.Popup(position, propertyScope.content.text, property.hasMultipleDifferentValues ? -1 : Options.IndexOf(property.stringValue), Options.ToArray());

                    if (changeCheckScope.changed)
                    {
                        property.stringValue = Options[value];
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
