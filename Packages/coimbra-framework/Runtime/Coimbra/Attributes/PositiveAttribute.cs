using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    /// Forces an int or float to always be positive.
    /// </summary>
    [PublicAPI]
    public sealed class PositiveAttribute : PropertyAttributeBase
    {
        public PositiveAttribute(params string[] callbacks)
            : base(callbacks) { }

        public PositiveAttribute(bool delayed, params string[] callbacks)
            : base(delayed, callbacks) { }

        public PositiveAttribute(ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks) { }

        public PositiveAttribute(ReadWriteMode editMode, ReadWriteMode playMode, bool delayed, params string[] callbacks)
            : base(editMode, playMode, delayed, callbacks) { }
    }
}
