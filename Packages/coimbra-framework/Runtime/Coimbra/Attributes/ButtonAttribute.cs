using JetBrains.Annotations;

namespace Coimbra
{
    /// <summary>
    /// Transforms a bool field into a button to call one or more methods from inspector.
    /// </summary>
    [PublicAPI]
    public sealed class ButtonAttribute : PropertyAttributeBase
    {
        public readonly ButtonOptions Options;

        public ButtonAttribute(params string[] callbacks)
            : base(callbacks)
        {
            Options = ButtonOptions.Default;
        }

        public ButtonAttribute(ButtonOptions options, params string[] callbacks)
            : base(callbacks)
        {
            Options = options;
        }

        public ButtonAttribute(ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks)
        {
            Options = ButtonOptions.Default;
        }

        public ButtonAttribute(ButtonOptions options, ReadWriteMode editMode, ReadWriteMode playMode, params string[] callbacks)
            : base(editMode, playMode, callbacks)
        {
            Options = options;
        }
    }
}
