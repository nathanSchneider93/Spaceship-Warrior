using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    /// Forces an int or float to be equal or smaller than a value.
    /// </summary>
    [PublicAPI]
    public sealed class NotGreaterThanAttribute : PropertyAttributeBase
    {
        public readonly float Value;

        public NotGreaterThanAttribute(float value, params string[] callbacks)
            : base(callbacks)
        {
            Value = value;
        }

        public NotGreaterThanAttribute(float value, bool delayed, params string[] callbacks)
            : base(delayed, callbacks)
        {
            Value = value;
        }

        public NotGreaterThanAttribute(float value, ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks)
        {
            Value = value;
        }

        public NotGreaterThanAttribute(float value, ReadWriteMode editMode, ReadWriteMode playMode, bool delayed, params string[] callbacks)
            : base(editMode, playMode, delayed, callbacks)
        {
            Value = value;
        }
    }
}
