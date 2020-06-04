using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    /// Transforms a string field into a tag popup.
    /// </summary>
    [PublicAPI]
    public sealed class TagSelectorAttribute : PropertyAttributeBase
    {
        public TagSelectorAttribute(params string[] callbacks)
            : base(callbacks) { }

        public TagSelectorAttribute(ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks) { }
    }
}
