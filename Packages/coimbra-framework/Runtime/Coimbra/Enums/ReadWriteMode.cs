using JetBrains.Annotations;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Used to control the read and write permissions of the serialized field.
    /// </summary>
    [PublicAPI]
    public enum ReadWriteMode
    {
        /// <summary>
        /// Hide the field in the inspector, like <see cref="HideInInspector"/>.
        /// </summary>
        Hide,
        /// <summary>
        /// Show the field in a read-only state, like <see cref="ReadOnlyAttribute"/>.
        /// </summary>
        Read,
        /// <summary>
        /// Show the field as usual.
        /// </summary>
        Write
    }
}
