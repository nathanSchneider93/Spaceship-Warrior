using JetBrains.Annotations;
using System;
using UnityEngine.Events;

namespace Coimbra
{
    /// <summary>
    /// Control the permissions of an <see cref="UnityEvent"/>.
    /// </summary>
    [PublicAPI, Flags]
    public enum UnityEventPermissions
    {
        Nothing = 0,
        Everything = ~0,
        /// <summary>
        /// The event list is visible.
        /// </summary>
        Read = 1,
        /// <summary>
        /// Can drag the elements to reorder them.
        /// </summary>
        Drag = (1 << 5) | Read,
        /// <summary>
        /// Can collapse the event list.
        /// </summary>
        Collapse = (1 << 7) | Read,
    }
}
