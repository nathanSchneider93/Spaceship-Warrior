using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    [CustomPropertyDrawer(typeof(InterfaceReferenceBase), true)]
    public sealed class InterfaceReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            OnGUI(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public void OnGUI(Rect position, SerializedProperty property, GUIContent label, bool allowSceneObjects)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty objectProperty = property.FindPropertyRelative("m_Object");
            System.Type interfaceType = property.GetValue().GetType().GetProperty("Value").PropertyType;
            string tooltip = $"{(string.IsNullOrEmpty(label.tooltip) ? "" : $"{label.tooltip}{System.Environment.NewLine}")}*{interfaceType}";
            var objectLabel = new GUIContent($"{label.text}*", label.image, tooltip);

            using (var propertyScope = new EditorGUI.PropertyScope(position, objectLabel, objectProperty))
            {
                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    Object value = EditorGUI.ObjectField(position, propertyScope.content, objectProperty.objectReferenceValue, typeof(Object), allowSceneObjects);

                    if (changeCheckScope.changed)
                    {
                        if (value != null && value.GetType().IsSubclassOf(interfaceType) == false)
                        {
                            value = value is GameObject gameObject ? gameObject.GetComponent(interfaceType) : null;
                        }

                        objectProperty.objectReferenceValue = value;
                    }
                }
            }
        }
    }
}
