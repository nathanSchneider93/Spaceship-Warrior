using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace Coimbra
{
    /// <summary>
    /// Upgrades the <see cref="UnityEvent"/> to be reorderable and collapsible.
    /// </summary>
    [PublicAPI]
    public sealed class UnityEventAttribute : PropertyAttribute
    {
        public readonly UnityEventPermissions EditModePermissions;
        public readonly UnityEventPermissions PlayModePermissions;

        public UnityEventAttribute(UnityEventPermissions permissions = UnityEventPermissions.Everything)
            : this(permissions, permissions) { }

        public UnityEventAttribute(UnityEventPermissions editModePermissions, UnityEventPermissions playModePermissions)
        {
            EditModePermissions = editModePermissions;
            PlayModePermissions = playModePermissions;
        }
    }
}
