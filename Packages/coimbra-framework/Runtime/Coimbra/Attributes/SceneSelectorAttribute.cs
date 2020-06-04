using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    /// Transforms an int or string field into a scene popup.
    /// </summary>
    [PublicAPI]
    public sealed class SceneSelectorAttribute : PropertyAttributeBase
    {
        public readonly bool ExcludeDisabled;

        public SceneSelectorAttribute(params string[] callbacks)
            : base(callbacks)
        {
            ExcludeDisabled = false;
        }

        public SceneSelectorAttribute(bool excludeDisabled, params string[] callbacks)
            : base(callbacks)
        {
            ExcludeDisabled = excludeDisabled;
        }

        public SceneSelectorAttribute(ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks)
        {
            ExcludeDisabled = false;
        }

        public SceneSelectorAttribute(bool excludeDisabled, ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks)
        {
            ExcludeDisabled = excludeDisabled;
        }
    }
}
