using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// Reflection information for a given <see cref="UnityEditor.SerializedProperty.propertyPath"/>.
    /// To build one use <see cref="PropertyPathInfoExtensions.GetPropertyPathInfo(UnityEditor.SerializedProperty)"/>.
    /// </summary>
    public sealed class PropertyPathInfo
    {
        private readonly int? Index;
        private readonly FieldInfo FieldInfo;
        private readonly PropertyPathInfo Next;

        private string _propertyPath;

        internal PropertyPathInfo(FieldInfo fieldInfo, PropertyPathInfo next, int? index = null)
        {
            _propertyPath = null;
            FieldInfo = fieldInfo;
            Next = next;
            Index = index;
        }

        public override string ToString()
        {
            InitializePropertyPath();

            return _propertyPath;
        }

        /// <summary>
        /// Get the object that contains that field.
        /// </summary>
        public T GetParentObject<T>(Object target)
        {
            object value = GetParentObject(target);

            return value != null ? (T)value : default;
        }

        /// <summary>
        /// Get the object that contains that field.
        /// </summary>
        public object GetParentObject(Object target)
        {
            object value = target;

            for (PropertyPathInfo current = this; current.Next != null; current = current.Next)
            {
                if (current.Index.HasValue == false)
                {
                    value = current.FieldInfo.GetValue(value);
                }
                else
                {
                    IEnumerator enumerator = ((IEnumerable)current.FieldInfo.GetValue(value)).GetEnumerator();

                    for (int i = 0; enumerator.MoveNext(); i++)
                    {
                        if (current.Index == i)
                        {
                            value = enumerator.Current;

                            break;
                        }
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Get the object that contains that field for all targets.
        /// </summary>
        public T[] GetParentObjects<T>(Object[] targets)
        {
            var values = new T[targets.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                values[i] = GetParentObject<T>(targets[i]);
            }

            return values;
        }

        /// <summary>
        /// Get the object that contains that field for all targets.
        /// </summary>
        public object[] GetParentObjects(Object[] targets)
        {
            var values = new object[targets.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                values[i] = GetParentObject(targets[i]);
            }

            return values;
        }

        /// <summary>
        /// Get the object that contains that field for all targets.
        /// </summary>
        /// <param name="append">List to append the objects.</param>
        public void GetParentObjects<T>(Object[] targets, List<T> append)
        {
            int capacity = append.Count + targets.Length;

            if (append.Capacity < capacity)
            {
                append.Capacity = capacity;
            }

            for (int i = 0; i < targets.Length; i++)
            {
                append.Add(GetParentObject<T>(targets[i]));
            }
        }

        /// <summary>
        /// Get the object that contains that field for all targets.
        /// </summary>
        /// <param name="append">List to append the objects.</param>
        public void GetParentObjects(Object[] targets, List<object> append)
        {
            int capacity = append.Count + targets.Length;

            if (append.Capacity < capacity)
            {
                append.Capacity = capacity;
            }

            for (int i = 0; i < targets.Length; i++)
            {
                append.Add(GetParentObject(targets[i]));
            }
        }

        /// <summary>
        /// Get the field value.
        /// </summary>
        public T GetValue<T>(Object target)
        {
            object value = GetValue(target);

            return value != null ? (T)value : default;
        }

        /// <summary>
        /// Get the field value.
        /// </summary>
        public object GetValue(Object target)
        {
            return GetValueInternal(target);
        }

        /// <summary>
        /// Get the field value for all targets.
        /// </summary>
        public T[] GetValues<T>(Object[] targets)
        {
            var values = new T[targets.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                values[i] = GetValue<T>(targets[i]);
            }

            return values;
        }

        /// <summary>
        /// Get the field value for all targets.
        /// </summary>
        /// <param name="append">List to append the values.</param>
        public void GetValues<T>(Object[] targets, List<T> append)
        {
            int capacity = append.Count + targets.Length;

            if (append.Capacity < capacity)
            {
                append.Capacity = capacity;
            }

            for (int i = 0; i < targets.Length; i++)
            {
                append.Add(GetValue<T>(targets[i]));
            }
        }

        /// <summary>
        /// Set the field value.
        /// </summary>
        public void SetValue<T>(Object target, T value)
        {
            SetValueInternal(target, value);
        }

        /// <summary>
        /// Set the field value.
        /// </summary>
        /// <param name="onSetValue">Sends the current value and expected the new value in return.</param>
        public void SetValue<T>(Object target, Func<T, T> onSetValue)
        {
            var oldValue = GetValue<T>(target);
            T newValue = onSetValue.Invoke(oldValue);
            SetValue(target, newValue);
        }

        /// <summary>
        /// Set the field value for all targets.
        /// </summary>
        public void SetValues<T>(Object[] targets, T value)
        {
            foreach (Object target in targets)
            {
                SetValue(target, value);
            }
        }

        /// <summary>
        /// Set the field value for all targets.
        /// </summary>
        /// <param name="onSetValue">Sends the current value and expected the new value in return.</param>
        public void SetValues<T>(Object[] targets, Func<T, T> onSetValue)
        {
            foreach (Object target in targets)
            {
                SetValue(target, onSetValue);
            }
        }

        private object GetValueInternal(object target)
        {
            object value = null;

            if (Index.HasValue == false)
            {
                value = FieldInfo.GetValue(target);
            }
            else
            {
                IEnumerator enumerable = ((IEnumerable)FieldInfo.GetValue(target)).GetEnumerator();

                for (int i = 0; enumerable.MoveNext(); i++)
                {
                    if (Index == i)
                    {
                        value = enumerable.Current;

                        break;
                    }
                }
            }

            return Next != null ? Next.GetValueInternal(value) : value;
        }

        private void InitializePropertyPath()
        {
            if (_propertyPath != null)
            {
                return;
            }

            var builder = new StringBuilder();
            PropertyPathInfo current = this;

            do
            {
                builder.Append($"{current.FieldInfo.Name}");

                if (current.Index.HasValue)
                {
                    builder.Append($"[{current.Index}]");
                }

                current = current.Next;

                if (current == null)
                {
                    break;
                }

                builder.Append(".");
            } while (true);

            _propertyPath = builder.ToString();
        }

        private void SetValueInternal<T>(object obj, T value)
        {
            object target = obj;
            PropertyPathInfo current = this;

            while (current.Next != null)
            {
                if (current.Index.HasValue == false)
                {
                    target = current.FieldInfo.GetValue(target);
                }
                else
                {
                    IEnumerator enumerator = ((IEnumerable)current.FieldInfo.GetValue(target)).GetEnumerator();

                    for (int i = 0; enumerator.MoveNext(); i++)
                    {
                        if (current.Index == i)
                        {
                            target = enumerator.Current;

                            break;
                        }
                    }
                }

                current = current.Next;
            }

            if (current.Index.HasValue == false)
            {
                current.FieldInfo.SetValue(target, value);
            }
            else
            {
                var array = current.FieldInfo.GetValue(target) as T[];

                if (array == null)
                {
                    return;
                }

                if (array.Length > current.Index)
                {
                    array[current.Index.Value] = value;
                }
                else
                {
                    var temp = new List<T>(array);
                    temp.Add(value);
                    array = temp.ToArray();
                }

                current.FieldInfo.SetValue(target, array);
            }
        }
    }
}
