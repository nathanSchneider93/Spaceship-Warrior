using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    [CustomPropertyDrawer(typeof(ObjectPickerAttribute), true)]
    public sealed class ObjectPickerDrawer : PropertyAttributeDrawer<ObjectPickerAttribute>
    {
        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
                {
                    using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                    {
                        Object value = EditorGUI.ObjectField(position, propertyScope.content, property.objectReferenceValue, fieldInfo.FieldType, Attribute.AllowSceneObjects);

                        if (changeCheckScope.changed)
                        {
                            property.objectReferenceValue = value;
                        }
                    }
                }
            }
            else if (fieldInfo.FieldType.IsSubclassOf(typeof(InterfaceReferenceBase)))
            {
                InterfaceReferenceDrawer.OnGUI(position, property, label, Attribute.AllowSceneObjects);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use ObjectPicker with Object or InterfaceReference.");
            }
        }

        protected override float GetHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
