using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    /// Forces an int or float to always be negative.
    /// </summary>
    [PublicAPI]
    public sealed class NegativeAttribute : PropertyAttributeBase
    {
        public NegativeAttribute(params string[] callbacks)
            : base(callbacks) { }

        public NegativeAttribute(bool delayed, params string[] callbacks)
            : base(delayed, callbacks) { }

        public NegativeAttribute(ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks) { }

        public NegativeAttribute(ReadWriteMode editMode, ReadWriteMode playMode, bool delayed, params string[] callbacks)
            : base(editMode, playMode, delayed, callbacks) { }
    }
}
