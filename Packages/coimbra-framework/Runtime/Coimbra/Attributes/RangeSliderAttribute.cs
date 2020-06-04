using JetBrains.Annotations;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Creates a min and max slider for a <see cref="Vector2"/> or <see cref="Vector2Int"/> or <see cref="FloatRange"/> or <see cref="IntRange"/> field.
    /// </summary>
    [PublicAPI]
    public class RangeSliderAttribute : PropertyAttributeBase
    {
        public readonly float Max;
        public readonly float Min;

        public RangeSliderAttribute(float a, float b, params string[] callbacks)
            : base(callbacks)
        {
            Min = Mathf.Min(a, b);
            Max = Mathf.Max(a, b);
        }

        public RangeSliderAttribute(float a, float b, bool delayed, params string[] callbacks)
            : base(delayed, callbacks)
        {
            Min = Mathf.Min(a, b);
            Max = Mathf.Max(a, b);
        }

        public RangeSliderAttribute(float a, float b, ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks)
        {
            Min = Mathf.Min(a, b);
            Max = Mathf.Max(a, b);
        }

        public RangeSliderAttribute(float a, float b, ReadWriteMode editMode, ReadWriteMode playMode, bool delayed, params string[] callbacks)
            : base(editMode, playMode, delayed, callbacks)
        {
            Min = Mathf.Min(a, b);
            Max = Mathf.Max(a, b);
        }
    }
}
