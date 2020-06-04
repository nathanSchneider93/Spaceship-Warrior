using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    /// Transforms an enum field with <see cref="System.FlagsAttribute"/> into a flags popup.
    /// </summary>
    [PublicAPI]
    public sealed class EnumFlagsAttribute : PropertyAttributeBase
    {
        public EnumFlagsAttribute(params string[] callbacks)
            : base(callbacks) { }

        public EnumFlagsAttribute(ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks) { }
    }
}
