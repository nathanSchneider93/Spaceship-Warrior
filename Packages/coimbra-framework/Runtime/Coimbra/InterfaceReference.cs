using JetBrains.Annotations;
using System;
using UnityEngine;

namespace Coimbra
{
    [PublicAPI, Serializable]
    public abstract class InterfaceReference<T> : InterfaceReferenceBase where T : class
    {
        private T m_Value;

        public T Value => m_Value ?? (m_Value = GetValue());
        public bool HasValue => Value != null;

        public static implicit operator T(InterfaceReference<T> reference)
        {
            return reference.Value;
        }

        private T GetValue()
        {
            switch (Object)
            {
                case T value:
                    return value;

                case GameObject gameObject:
                    return gameObject.GetComponent<T>();

                default:
                    return null;
            }
        }
    }
}
