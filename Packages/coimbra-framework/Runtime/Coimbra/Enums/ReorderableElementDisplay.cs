using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    /// Control how the elements of a reorderable list should be displayed.
    /// </summary>
    [PublicAPI]
    public enum ReorderableElementDisplay
    {
        /// <summary>
        /// Automatically detect.
        /// </summary>
        Auto,
        /// <summary>
        /// Handled by a custom editor or custom property drawer.
        /// </summary>
        Custom,
        /// <summary>
        /// All elements are expandable.
        /// </summary>
        Expandable,
        /// <summary>
        /// All elements are non-expandable.
        /// </summary>
        NonExpandable
    }
}
