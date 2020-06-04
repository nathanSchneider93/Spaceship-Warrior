using System;
using System.ComponentModel;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra
{
    [Serializable, EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class InterfaceReferenceBase
    {
#pragma warning disable 0649
        [SerializeField] private Object m_Object;
#pragma warning restore 0649

        public Object Object => m_Object;
    }
}
