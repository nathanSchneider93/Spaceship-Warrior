using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public sealed class ButtonDrawer : PropertyAttributeDrawer<ButtonAttribute>
    {
        private static readonly string[] Callbacks = new string[1];

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Boolean)
            {
                EditorGUI.LabelField(position, label.text, "Use Button with bool.");

                return;
            }

            if (IsVisible)
            {
                using (new EditorGUI.DisabledScope(IsReadOnly))
                {
                    if (ContainsOptions(ButtonOptions.Stretch))
                    {
                        position = EditorGUI.IndentedRect(position);
                    }
                    else if (ContainsOptions(ButtonOptions.FillFieldsArea))
                    {
                        position.width -= EditorGUIUtility.labelWidth;
                        position.x += EditorGUIUtility.labelWidth;
                    }
                    else if (ContainsOptions(ButtonOptions.FillLabelsArea))
                    {
                        position.width = EditorGUIUtility.labelWidth;

                        float xMax = position.xMax;
                        position = EditorGUI.IndentedRect(position);
                        position.xMax = xMax;
                    }

                    if (ContainsOptions(ButtonOptions.OnePerCallback) == false || Attribute?.Callbacks == null || Attribute.Callbacks.Length <= 1)
                    {
                        if (GUI.Button(position, label, EditorStyles.miniButton))
                        {
                            InvokeCallbacks(property, Attribute.Callbacks);
                        }
                    }
                    else
                    {
                        int last = Attribute.Callbacks.Length - 1;
                        float origin = position.x;
                        float width = position.width / Attribute.Callbacks.Length;
                        position.width = width;
                        Callbacks[0] = Attribute.Callbacks[0];

                        if (GUI.Button(position, Callbacks[0], EditorStyles.miniButtonLeft))
                        {
                            InvokeCallbacks(property, Callbacks);
                        }

                        position.x = origin + width * last;
                        Callbacks[0] = Attribute.Callbacks[last];

                        if (GUI.Button(position, Callbacks[0], EditorStyles.miniButtonRight))
                        {
                            InvokeCallbacks(property, Callbacks);
                        }

                        for (var i = 1; i < last; i++)
                        {
                            position.x = origin + width * i;
                            Callbacks[0] = Attribute.Callbacks[i];

                            if (GUI.Button(position, Callbacks[0], EditorStyles.miniButtonMid))
                            {
                                InvokeCallbacks(property, Callbacks);
                            }
                        }
                    }
                }
            }
        }

        protected override float GetHeight(SerializedProperty property, GUIContent label)
        {
            return EditorStyles.miniButton.CalcSize(label).y;
        }

        private bool ContainsOptions(ButtonOptions options)
        {
            return (Attribute.Options & options) == options;
        }
    }
}
