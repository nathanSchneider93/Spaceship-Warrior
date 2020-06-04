using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    /// Customize when the field have it's values visible and editable in inspector.
    /// </summary>
    [PublicAPI]
    public sealed class ReadWriteAttribute : PropertyAttributeBase
    {
        public ReadWriteAttribute(ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks) { }

        public ReadWriteAttribute(ReadWriteMode editMode, ReadWriteMode playMode, bool delayed, params string[] callbacks)
            : base(editMode, playMode, delayed, callbacks) { }
    }
}
