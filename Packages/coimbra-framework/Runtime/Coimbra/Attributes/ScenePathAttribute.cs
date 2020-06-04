using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    /// Transforms a string field into a scene path popup.
    /// </summary>
    [PublicAPI]
    public sealed class ScenePathAttribute : PropertyAttributeBase
    {
        public readonly bool ExcludeDisabled;

        public ScenePathAttribute(params string[] callbacks)
            : base(callbacks)
        {
            ExcludeDisabled = false;
        }

        public ScenePathAttribute(bool excludeDisabled, params string[] callbacks)
            : base(callbacks)
        {
            ExcludeDisabled = excludeDisabled;
        }

        public ScenePathAttribute(ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks)
        {
            ExcludeDisabled = false;
        }

        public ScenePathAttribute(bool excludeDisabled, ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks)
        {
            ExcludeDisabled = excludeDisabled;
        }
    }
}
