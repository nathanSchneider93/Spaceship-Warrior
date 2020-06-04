using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    /// Use this to customize how the object picker should behave.
    /// </summary>
    [PublicAPI]
    public sealed class ObjectPickerAttribute : PropertyAttributeBase
    {
        public readonly bool AllowSceneObjects;

        public ObjectPickerAttribute(bool allowSceneObjects, params string[] callbacks)
            : base(callbacks)
        {
            AllowSceneObjects = allowSceneObjects;
        }

        public ObjectPickerAttribute(bool allowSceneObjects, ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks)
        {
            AllowSceneObjects = allowSceneObjects;
        }
    }
}
