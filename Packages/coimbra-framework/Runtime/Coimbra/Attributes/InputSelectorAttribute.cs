using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    /// Transforms a string field into a input popup.
    /// </summary>
    [PublicAPI]
    public sealed class InputSelectorAttribute : PropertyAttributeBase
    {
        public InputSelectorAttribute(params string[] callbacks)
            : base(callbacks) { }

        public InputSelectorAttribute(ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks) { }
    }
}
