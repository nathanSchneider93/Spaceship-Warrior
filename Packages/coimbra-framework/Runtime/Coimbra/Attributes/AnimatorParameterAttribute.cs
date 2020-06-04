using JetBrains.Annotations;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Transforms an int or string field into a popup to choose the desired animator controller parameter name.
    /// </summary>
    [PublicAPI]
    public sealed class AnimatorParameterAttribute : PropertyAttributeBase
    {
        public readonly string AnimatorField;
        public readonly AnimatorControllerParameterType ParameterType;

        public AnimatorParameterAttribute(AnimatorControllerParameterType parameterType, params string[] callbacks)
            : base(callbacks)
        {
            AnimatorField = null;
            ParameterType = parameterType;
        }

        public AnimatorParameterAttribute(AnimatorControllerParameterType parameterType, ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks)
        {
            AnimatorField = null;
            ParameterType = parameterType;
        }

        /// <param name="animatorField">The target animator field. If none, it searches for the first serialized <see cref="Animator"/> field available.</param>
        public AnimatorParameterAttribute(string animatorField, AnimatorControllerParameterType parameterType, params string[] callbacks)
            : base(callbacks)
        {
            AnimatorField = animatorField;
            ParameterType = parameterType;
        }

        /// <param name="animatorField">The target animator field. If none, it searches for the first serialized <see cref="Animator"/> field available.</param>
        public AnimatorParameterAttribute(string animatorField, AnimatorControllerParameterType parameterType, ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks)
        {
            AnimatorField = animatorField;
            ParameterType = parameterType;
        }
    }
}
