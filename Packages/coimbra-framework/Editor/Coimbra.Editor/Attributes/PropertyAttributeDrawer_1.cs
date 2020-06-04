using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    public class PropertyAttributeDrawer<T> : PropertyDrawer where T : PropertyAttributeBase
    {
        private const BindingFlags CallbacksBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

        protected readonly FloatRangeDrawer FloatRangeDrawer = new FloatRangeDrawer();
        protected readonly IntRangeDrawer IntRangeDrawer = new IntRangeDrawer();
        protected readonly InterfaceReferenceDrawer InterfaceReferenceDrawer = new InterfaceReferenceDrawer();

        private readonly List<object> Targets = new List<object>();

        private bool _invokeCallbacks;

        private T m_Attribute;

        public T Attribute { get => m_Attribute ?? (m_Attribute = attribute as T); set => m_Attribute = value; }

        protected bool IsReadOnly => (EditorApplication.isPlaying ? Attribute.PlayMode : Attribute.EditMode) == ReadWriteMode.Read;
        protected bool IsVisible => (EditorApplication.isPlaying ? Attribute.PlayMode : Attribute.EditMode) != ReadWriteMode.Hide;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_invokeCallbacks)
            {
                _invokeCallbacks = false;
                InvokeCallbacks(property, Attribute.Callbacks);
            }
            
            if (IsVisible)
            {
                using (new EditorGUI.DisabledScope(IsReadOnly))
                {
                    using (var scope = new EditorGUI.ChangeCheckScope())
                    {
                        DrawGUI(position, property, label);

                        if (scope.changed)
                        {
                            _invokeCallbacks = true;
                        }
                    }
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return IsVisible ? GetHeight(property, label) : 0;
        }

        public void OnGUI(Rect position, SerializedProperty property, GUIContent label, T attributeOverride)
        {
            T attributeBackup = Attribute;
            Attribute = attributeOverride;
            OnGUI(position, property, label);
            Attribute = attributeBackup;
        }

        protected virtual void DrawGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawDefaultGUI(position, property, label);
        }

        protected virtual float GetHeight(SerializedProperty property, GUIContent label)
        {
            return GetDefaultHeight(property, label);
        }

        protected void DrawDefaultGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.type == typeof(FloatRange).Name)
            {
                FloatRangeDrawer.OnGUI(position, property, label, Attribute.Delayed);
            }
            else if (property.type == typeof(IntRange).Name)
            {
                IntRangeDrawer.OnGUI(position, property, label, Attribute.Delayed);
            }
            else if (Attribute.Delayed)
            {
                switch (property.propertyType)
                {
                    case SerializedPropertyType.Float:
                    {
                        EditorGUI.DelayedFloatField(position, property, label);

                        break;
                    }

                    case SerializedPropertyType.Integer:
                    {
                        EditorGUI.DelayedIntField(position, property, label);

                        break;
                    }

                    case SerializedPropertyType.String:
                    {
                        EditorGUI.DelayedTextField(position, property, label);

                        break;
                    }

                    default:
                    {
                        EditorGUI.LabelField(position, label.text, "Use Delayed with float, int, or string.");

                        break;
                    }
                }
            }
            else if (fieldInfo.FieldType.IsSubclassOf(typeof(InterfaceReferenceBase)))
            {
                InterfaceReferenceDrawer.OnGUI(position, property, label);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        protected void InvokeCallbacks(SerializedProperty property, string[] callbacks)
        {
            if (callbacks == null || callbacks.Length == 0)
            {
                return;
            }

            Targets.Clear();
            property.GetParentObjects(Targets);

            for (var i = 0; i < callbacks.Length; i++)
            {
                MethodInfo method = property.GetParentObject().GetType().GetMethod(callbacks[i], CallbacksBindingFlags);

                if (method == null || method.GetParameters().Length > 0)
                {
                    continue;
                }

                for (var j = 0; j < Targets.Count; j++)
                {
                    method.Invoke(Targets[j], null);
                }
            }
        }

        protected float GetDefaultHeight(SerializedProperty property, GUIContent label)
        {
            if (property.type == typeof(FloatRange).Name)
            {
                return FloatRangeDrawer.GetPropertyHeight(property, label);
            }

            if (property.type == typeof(IntRange).Name)
            {
                return IntRangeDrawer.GetPropertyHeight(property, label);
            }

            if (fieldInfo.FieldType.IsSubclassOf(typeof(InterfaceReferenceBase)))
            {
                return InterfaceReferenceDrawer.GetPropertyHeight(property, label);
            }

            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
