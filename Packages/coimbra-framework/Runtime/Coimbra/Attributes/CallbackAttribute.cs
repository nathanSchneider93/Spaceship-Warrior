using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    /// Register one or more methods do be called when the field is changed.
    /// </summary>
    [PublicAPI]
    public sealed class CallbackAttribute : PropertyAttributeBase
    {
        public CallbackAttribute(params string[] callbacks)
            : base(callbacks) { }

        public CallbackAttribute(bool delayed, params string[] callbacks)
            : base(delayed, callbacks) { }
    }
}
