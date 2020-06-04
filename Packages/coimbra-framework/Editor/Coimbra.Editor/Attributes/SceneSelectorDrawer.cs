using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    [CustomPropertyDrawer(typeof(SceneSelectorAttribute))]
    public sealed class SceneSelectorDrawer : PropertyAttributeDrawer<SceneSelectorAttribute>
    {
        private static readonly List<string> Options = new List<string>();

        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                {
                    Options.Clear();

                    EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

                    using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
                    {
                        for (int i = 0; i < scenes.Length; i++)
                        {
                            if (Attribute.ExcludeDisabled && scenes[i].enabled == false)
                            {
                                continue;
                            }

                            Options.Add($"{Options.Count}: {Path.GetFileNameWithoutExtension(scenes[i].path)}");
                        }

                        if (Options.Count > 0)
                        {
                            property.SetValues((int value) =>
                            {
                                if (value < 0 || value >= Options.Count)
                                {
                                    return 0;
                                }

                                return value;
                            });
                        }
                        else
                        {
                            EditorGUI.LabelField(position, label.text, "No Scene added to build settings.");

                            break;
                        }

                        using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                        {
                            int value = EditorGUI.Popup(position, propertyScope.content.text, property.hasMultipleDifferentValues ? -1 : property.intValue, Options.ToArray());

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
                    Options.Clear();

                    EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

                    using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
                    {
                        for (int i = 0; i < scenes.Length; i++)
                        {
                            if (Attribute.ExcludeDisabled && scenes[i].enabled == false)
                            {
                                continue;
                            }

                            Options.Add(Path.GetFileNameWithoutExtension(scenes[i].path));
                        }

                        if (Options.Count > 0)
                        {
                            property.SetValues((string value) =>
                            {
                                if (string.IsNullOrEmpty(value) || Options.Contains(value) == false)
                                {
                                    return Options[0];
                                }

                                return value;
                            });
                        }
                        else
                        {
                            EditorGUI.LabelField(position, label.text, "No Scene added to build settings.");

                            break;
                        }

                        using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                        {
                            int value = EditorGUI.Popup(position, propertyScope.content.text, property.hasMultipleDifferentValues ? -1 : Options.IndexOf(property.stringValue), Options.ToArray());

                            if (changeCheckScope.changed)
                            {
                                property.stringValue = Options[value];
                            }
                        }
                    }

                    break;
                }

                default:
                {
                    EditorGUI.LabelField(position, label.text, "Use SceneSelector with int or string.");

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
