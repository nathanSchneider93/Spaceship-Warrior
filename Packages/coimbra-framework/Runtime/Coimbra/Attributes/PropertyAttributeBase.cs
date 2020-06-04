using System.ComponentModel;
using UnityEngine;

namespace Coimbra
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class PropertyAttributeBase : PropertyAttribute
    {
        public readonly bool Delayed;
        public readonly ReadWriteMode EditMode;
        public readonly ReadWriteMode PlayMode;
        public readonly string[] Callbacks;

        protected PropertyAttributeBase(params string[] callbacks)
            : this(ReadWriteMode.Write, ReadWriteMode.Write, false, callbacks) { }

        protected PropertyAttributeBase(bool delayed, params string[] callbacks)
            : this(ReadWriteMode.Write, ReadWriteMode.Write, delayed, callbacks) { }

        protected PropertyAttributeBase(ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : this(editMode, playMode, false, callbacks) { }

        protected PropertyAttributeBase(ReadWriteMode editMode, ReadWriteMode playMode, bool delayed, params string[] callbacks)
        {
            Delayed = delayed;
            EditMode = editMode;
            PlayMode = playMode;
            Callbacks = callbacks;
        }
    }
}
