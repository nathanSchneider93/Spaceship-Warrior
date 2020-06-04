using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine.Assertions;

namespace Coimbra
{
    /// <summary>
    /// Extensions to make easier to use the <see cref="PropertyPathInfo"/>.
    /// </summary>
    [PublicAPI]
    public static class PropertyPathInfoExtensions
    {
        private const BindingFlags PropertyPathInfoFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        /// <summary>
        /// Set the property value.
        /// </summary>
        public static void SetValue<T>(this SerializedProperty property, T value, int multiEditTargetIndex = 0)
        {
            property.GetPropertyPathInfo().SetValue(property.serializedObject.targetObjects[multiEditTargetIndex], value);
        }

        /// <summary>
        /// Set the property value.
        /// </summary>
        public static void SetValue<T>(this SerializedProperty property, Func<T, T> onSetValue, int multiEditTargetIndex = 0)
        {
            if (onSetValue == null)
            {
                return;
            }

            property.GetPropertyPathInfo().SetValue(property.serializedObject.targetObjects[multiEditTargetIndex], onSetValue);
        }

        /// <summary>
        /// Set the property value.
        /// </summary>
        public static void SetValue<T>(this SerializedProperty property, PropertyPathInfo info, T value, int multiEditTargetIndex = 0)
        {
            info.SetValue(property.serializedObject.targetObjects[multiEditTargetIndex], value);
        }

        /// <summary>
        /// Set the property value.
        /// </summary>
        public static void SetValue<T>(this SerializedProperty property, PropertyPathInfo info, Func<T, T> onSetValue, int multiEditTargetIndex = 0)
        {
            if (onSetValue == null)
            {
                return;
            }

            info.SetValue(property.serializedObject.targetObjects[multiEditTargetIndex], onSetValue);
        }

        /// <summary>
        /// Set all properties values.
        /// </summary>
        public static void SetValues<T>(this SerializedProperty property, T value)
        {
            GetPropertyPathInfo(property).SetValues(property.serializedObject.targetObjects, value);
        }

        /// <summary>
        /// Set all properties values.
        /// </summary>
        public static void SetValues<T>(this SerializedProperty property, Func<T, T> onSetValue)
        {
            if (onSetValue == null)
            {
                return;
            }

            GetPropertyPathInfo(property).SetValues(property.serializedObject.targetObjects, onSetValue);
        }

        /// <summary>
        /// Set all properties values.
        /// </summary>
        public static void SetValues<T>(this SerializedProperty property, PropertyPathInfo info, T value)
        {
            info.SetValues(property.serializedObject.targetObjects, value);
        }

        /// <summary>
        /// Set all properties values.
        /// </summary>
        public static void SetValues<T>(this SerializedProperty property, PropertyPathInfo info, Func<T, T> onSetValue)
        {
            if (onSetValue == null)
            {
                return;
            }

            info.SetValues(property.serializedObject.targetObjects, onSetValue);
        }

        /// <summary>
        /// Creates a <see cref="PropertyPathInfo"/> for this property.
        /// </summary>
        public static PropertyPathInfo GetPropertyPathInfo(this SerializedProperty property)
        {
            return GetPropertyPathInfo(property, new List<string>(property.propertyPath.Split('.')));
        }

        /// <summary>
        /// Creates a <see cref="PropertyPathInfo"/> for this property.
        /// </summary>
        public static PropertyPathInfo GetPropertyPathInfo(this SerializedProperty property, List<string> propertyPath)
        {
            return GetPropertyPathInfoRecursively(property.serializedObject.targetObject.GetType(), propertyPath);
        }

        /// <summary>
        /// Get the object that contains the property.
        /// </summary>
        public static T GetParentObject<T>(this SerializedProperty property, int multiEditTargetIndex = 0)
        {
            return property.GetPropertyPathInfo().GetParentObject<T>(property.serializedObject.targetObjects[multiEditTargetIndex]);
        }

        /// <summary>
        /// Get the object that contains the property.
        /// </summary>
        public static T GetParentObject<T>(this SerializedProperty property, PropertyPathInfo info, int multiEditTargetIndex = 0)
        {
            return info.GetParentObject<T>(property.serializedObject.targetObjects[multiEditTargetIndex]);
        }

        /// <summary>
        /// Get the object that contains the property.
        /// </summary>
        public static object GetParentObject(this SerializedProperty property, int multiEditTargetIndex = 0)
        {
            return property.GetPropertyPathInfo().GetParentObject(property.serializedObject.targetObjects[multiEditTargetIndex]);
        }

        /// <summary>
        /// Get the object that contains the property.
        /// </summary>
        public static object GetParentObject(this SerializedProperty property, PropertyPathInfo info, int multiEditTargetIndex = 0)
        {
            return info.GetParentObject(property.serializedObject.targetObjects[multiEditTargetIndex]);
        }

        /// <summary>
        /// Get the property value.
        /// </summary>
        public static object GetValue(this SerializedProperty property, int multiEditTargetIndex = 0)
        {
            return property.GetPropertyPathInfo().GetValue(property.serializedObject.targetObjects[multiEditTargetIndex]);
        }

        /// <summary>
        /// Get the property value.
        /// </summary>
        public static T GetValue<T>(this SerializedProperty property, int multiEditTargetIndex = 0)
        {
            return property.GetPropertyPathInfo().GetValue<T>(property.serializedObject.targetObjects[multiEditTargetIndex]);
        }

        /// <summary>
        /// Get the property value.
        /// </summary>
        public static T GetValue<T>(this SerializedProperty property, PropertyPathInfo info, int multiEditTargetIndex = 0)
        {
            return info.GetValue<T>(property.serializedObject.targetObjects[multiEditTargetIndex]);
        }

        /// <summary>
        /// Get the objects that contains the property.
        /// </summary>
        public static void GetParentObjects(this SerializedProperty property, List<object> append)
        {
            property.GetPropertyPathInfo().GetParentObjects(property.serializedObject.targetObjects, append);
        }

        /// <summary>
        /// Get the objects that contains the property.
        /// </summary>
        public static void GetParentObjects(this SerializedProperty property, PropertyPathInfo info, List<object> append)
        {
            info.GetParentObjects(property.serializedObject.targetObjects, append);
        }

        /// <summary>
        /// Get the objects that contains the property.
        /// </summary>
        public static object[] GetParentObjects(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().GetParentObjects(property.serializedObject.targetObjects);
        }

        /// <summary>
        /// Get the objects that contains the property.
        /// </summary>
        public static object[] GetParentObjects(this SerializedProperty property, PropertyPathInfo info)
        {
            return info.GetParentObjects(property.serializedObject.targetObjects);
        }

        /// <summary>
        /// Get the objects that contains the property.
        /// </summary>
        public static void GetParentObjects<T>(this SerializedProperty property, List<T> append)
        {
            property.GetPropertyPathInfo().GetParentObjects(property.serializedObject.targetObjects, append);
        }

        /// <summary>
        /// Get the objects that contains the property.
        /// </summary>
        public static void GetParentObjects<T>(this SerializedProperty property, PropertyPathInfo info, List<T> append)
        {
            info.GetParentObjects(property.serializedObject.targetObjects, append);
        }

        /// <summary>
        /// Get the objects that contains the property.
        /// </summary>
        public static T[] GetParentObjects<T>(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().GetParentObjects<T>(property.serializedObject.targetObjects);
        }

        /// <summary>
        /// Get the objects that contains the property.
        /// </summary>
        public static T[] GetParentObjects<T>(this SerializedProperty property, PropertyPathInfo info)
        {
            return info.GetParentObjects<T>(property.serializedObject.targetObjects);
        }

        /// <summary>
        /// Get all properties values.
        /// </summary>
        public static void GetValues<T>(this SerializedProperty property, List<T> append)
        {
            property.GetPropertyPathInfo().GetValues(property.serializedObject.targetObjects, append);
        }

        /// <summary>
        /// Get all properties values.
        /// </summary>
        public static void GetValues<T>(this SerializedProperty property, PropertyPathInfo info, List<T> append)
        {
            info.GetValues(property.serializedObject.targetObjects, append);
        }

        /// <summary>
        /// Get all properties values.
        /// </summary>
        public static T[] GetValues<T>(this SerializedProperty property)
        {
            return property.GetPropertyPathInfo().GetValues<T>(property.serializedObject.targetObjects);
        }

        /// <summary>
        /// Get all properties values.
        /// </summary>
        public static T[] GetValues<T>(this SerializedProperty property, PropertyPathInfo info)
        {
            return info.GetValues<T>(property.serializedObject.targetObjects);
        }

        private static PropertyPathInfo GetPropertyPathInfoRecursively(Type rootTargetType, List<string> propertyPath)
        {
            if (propertyPath.Count == 0)
            {
                return null;
            }

            FieldInfo fieldInfo = rootTargetType.GetField(propertyPath[0], PropertyPathInfoFlags);

            while (fieldInfo == null && rootTargetType.BaseType != null)
            {
                rootTargetType = rootTargetType.BaseType;
                fieldInfo = rootTargetType.GetField(propertyPath[0], PropertyPathInfoFlags);
            }

            Assert.IsNotNull(fieldInfo);

            if (propertyPath.Count > 2 && propertyPath[1] == "Array")
            {
                var index = new string(propertyPath[2].Where(char.IsDigit).ToArray());

                if (propertyPath[2].Replace(index, "") == "data[]")
                {
                    propertyPath.RemoveRange(0, 3);

                    PropertyPathInfo nextInfo = GetPropertyPathInfoRecursively(fieldInfo.FieldType.GetCollectionType(), propertyPath);

                    return new PropertyPathInfo(fieldInfo, nextInfo, Convert.ToInt32(index));
                }
            }

            propertyPath.RemoveAt(0);

            return new PropertyPathInfo(fieldInfo, GetPropertyPathInfoRecursively(fieldInfo.FieldType, propertyPath));
        }

        private static Type GetCollectionType(this Type type)
        {
            Type value = type.GetElementType();

            if (value != null)
            {
                return value;
            }

            Type[] arguments = type.GetGenericArguments();

            return arguments.Length > 0 ? arguments[0] : type;
        }
    }
}
