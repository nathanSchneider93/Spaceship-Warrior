using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    /// Forces an int or float to be equal or greater than a value.
    /// </summary>
    [PublicAPI]
    public sealed class NotLessThanAttribute : PropertyAttributeBase
    {
        public readonly float Value;

        public NotLessThanAttribute(float value, params string[] callbacks)
            : base(callbacks)
        {
            Value = value;
        }

        public NotLessThanAttribute(float value, bool delayed, params string[] callbacks)
            : base(delayed, callbacks)
        {
            Value = value;
        }

        public NotLessThanAttribute(float value, ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks)
        {
            Value = value;
        }

        public NotLessThanAttribute(float value, ReadWriteMode editMode, ReadWriteMode playMode, bool delayed, params string[] callbacks)
            : base(editMode, playMode, delayed, callbacks)
        {
            Value = value;
        }
    }
}
