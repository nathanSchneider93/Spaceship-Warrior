using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    /// Transforms an int or string field into a layer popup.
    /// </summary>
    [PublicAPI]
    public sealed class LayerSelectorAttribute : PropertyAttributeBase
    {
        public LayerSelectorAttribute(params string[] callbacks)
            : base(callbacks) { }

        public LayerSelectorAttribute(ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks) { }
    }
}
