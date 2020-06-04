using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    /// Transforms an int field into an sorting layer popup.
    /// </summary>
    [PublicAPI]
    public sealed class SortingLayerIDAttribute : PropertyAttributeBase
    {
        public SortingLayerIDAttribute(params string[] callbacks)
            : base(callbacks) { }

        public SortingLayerIDAttribute(ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks) { }
    }
}
