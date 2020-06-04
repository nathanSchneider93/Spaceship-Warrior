using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    [CustomPropertyDrawer(typeof(SortingLayerIDAttribute))]
    public sealed class SortingLayerIDDrawer : PropertyAttributeDrawer<SortingLayerIDAttribute>
    {
        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.LabelField(position, label.text, "Use SortingLayerID with int.");

                return;
            }

            MethodInfo method = typeof(EditorGUI).GetMethod("SortingLayerField", BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(Rect), typeof(GUIContent), typeof(SerializedProperty), typeof(GUIStyle), typeof(GUIStyle) }, null);

            if (method != null)
            {
                using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
                {
                    method.Invoke(null, new object[] { position, propertyScope.content, property, EditorStyles.popup, EditorStyles.label });
                }
            }
            else
            {
                Debug.LogError("Method UnityEditor.EditorGUI.SortingLayerField(Rect, GUIContent, SerializedProperty, GUIStyle, GUIStyle) was not found!");
            }
        }

        protected override float GetHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
