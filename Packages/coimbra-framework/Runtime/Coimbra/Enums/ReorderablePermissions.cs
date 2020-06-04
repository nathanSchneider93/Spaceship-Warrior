using JetBrains.Annotations;
using System;

namespace Coimbra
{
    /// <summary>
    /// Control the permissions of a reorderable list.
    /// </summary>
    [PublicAPI, Flags]
    public enum ReorderablePermissions
    {
        Nothing = 0,
        Everything = ~0,
        /// <summary>
        /// The list is visible.
        /// </summary>
        Read = 1,
        /// <summary>
        /// Can add new elements.
        /// </summary>
        Add = 1 << 1,
        /// <summary>
        /// Can duplicate the elements through the context menu.
        /// </summary>
        Duplicate = 1 << 2,
        /// <summary>
        /// Can remove elements through a button.
        /// </summary>
        RemoveByButton = 1 << 3,
        /// <summary>
        /// Can remove elements through the context menu.
        /// </summary>
        RemoveByContext = 1 << 4,
        /// <summary>
        /// Can drag the elements to reorder them.
        /// </summary>
        Drag = 1 << 5,
        /// <summary>
        /// Can edit the elements.
        /// </summary>
        EditElements = 1 << 6,
        /// <summary>
        /// Can collapse the entire list.
        /// </summary>
        Collapse = 1 << 7,
        /// <summary>
        /// Can select multiple elements at same time.
        /// </summary>
        MultiSelect = 1 << 8,
        /// <summary>
        /// Can see and collapse the entire list.
        /// </summary>
        ReadOnly = Read | Collapse
    }
}
