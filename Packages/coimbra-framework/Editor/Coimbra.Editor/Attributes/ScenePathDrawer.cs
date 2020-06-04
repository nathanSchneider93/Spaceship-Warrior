using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    [CustomPropertyDrawer(typeof(ScenePathAttribute))]
    public sealed class ScenePathDrawer : PropertyAttributeDrawer<ScenePathAttribute>
    {
        private static readonly List<string> Options = new List<string>();

        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

                if (scenes.Length == 0)
                {
                    EditorGUI.LabelField(position, label.text, "No Scene added to build settings.");

                    return;
                }

                using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
                {
                    Options.Clear();

                    string prefix = scenes[0].path.Replace(Path.GetFileName(scenes[0].path), "");

                    for (int i = 0; i < scenes.Length; i++)
                    {
                        if (Attribute.ExcludeDisabled && scenes[i].enabled == false)
                        {
                            continue;
                        }

                        Options.Add(scenes[i].path.Remove(scenes[i].path.Length - 6));

                        while (prefix != "" && Options[Options.Count - 1].StartsWith(prefix) == false)
                        {
                            prefix = prefix.Remove(prefix.LastIndexOf("/"));

                            int startIndex = prefix.LastIndexOf("/") + 1;

                            prefix = prefix.Remove(startIndex, prefix.Length - startIndex);
                        }
                    }

                    property.SetValues((string value) =>
                    {
                        if (string.IsNullOrEmpty(value) || Options.Contains(value) == false)
                        {
                            return Options[0];
                        }

                        return value;
                    });

                    var options = new string[Options.Count];

                    for (int i = 0; i < Options.Count; i++)
                    {
                        options[i] = Options[i].Remove(0, prefix.Length);
                    }

                    using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                    {
                        int value = EditorGUI.Popup(position, propertyScope.content.text, property.hasMultipleDifferentValues ? -1 : Options.IndexOf(property.stringValue), options);

                        if (changeCheckScope.changed)
                        {
                            property.stringValue = Options[value];
                        }
                    }
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use ScenePath with string.");
            }
        }

        protected override float GetHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
