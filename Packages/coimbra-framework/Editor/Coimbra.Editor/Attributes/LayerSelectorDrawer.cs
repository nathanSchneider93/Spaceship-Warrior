using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Coimbra
{
    [CustomPropertyDrawer(typeof(LayerSelectorAttribute))]
    public sealed class LayerSelectorDrawer : PropertyAttributeDrawer<LayerSelectorAttribute>
    {
        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                {
                    string[] layers = InternalEditorUtility.layers;

                    int SetValue(int value)
                    {
                        if (string.IsNullOrEmpty(LayerMask.LayerToName(value)))
                        {
                            return LayerMask.NameToLayer(layers[0]);
                        }

                        for (int i = 0; i < layers.Length; i++)
                        {
                            if (LayerMask.LayerToName(value) == layers[i])
                            {
                                return value;
                            }
                        }

                        return LayerMask.NameToLayer(layers[0]);
                    }

                    property.SetValues<int>(SetValue);

                    using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
                    {
                        using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                        {
                            int value = EditorGUI.LayerField(position, propertyScope.content, property.intValue);

                            if (changeCheckScope.changed)
                            {
                                property.intValue = value;
                            }
                        }
                    }

                    break;
                }

                case SerializedPropertyType.String:
                {
                    string[] layers = InternalEditorUtility.layers;

                    string SetValue(string value)
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            return layers[0];
                        }

                        for (int i = 0; i < layers.Length; i++)
                        {
                            if (value == layers[i])
                            {
                                return value;
                            }
                        }

                        return layers[0];
                    }

                    property.SetValues<string>(SetValue);

                    using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
                    {
                        using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                        {
                            int value = EditorGUI.LayerField(position, propertyScope.content, LayerMask.NameToLayer(property.stringValue));

                            if (changeCheckScope.changed)
                            {
                                property.stringValue = LayerMask.LayerToName(value);
                            }
                        }
                    }

                    break;
                }

                default:
                {
                    EditorGUI.LabelField(position, label.text, "Use LayerSelector with int or string.");

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
