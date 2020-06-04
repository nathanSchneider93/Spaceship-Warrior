using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    /// Makes a field to be read-only in the inspector.
    /// </summary>
    [PublicAPI]
    public sealed class ReadOnlyAttribute : PropertyAttributeBase
    {
        public ReadOnlyAttribute()
            : base(ReadWriteMode.Read, ReadWriteMode.Read) { }
    }
}
