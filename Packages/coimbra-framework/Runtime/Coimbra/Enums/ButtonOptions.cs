using JetBrains.Annotations;
using System;

namespace Coimbra
{
    /// <summary>
    /// Use with <see cref="ButtonAttribute"/> to define its appearance.
    /// </summary>
    [PublicAPI, Flags]
    public enum ButtonOptions
    {
        Default = Stretch,
        FillLabelsArea = 1,
        FillFieldsArea = 1 << 1,
        OnePerCallback = (1 << 2) | Stretch,
        Stretch = FillFieldsArea | FillFieldsArea,
    }
}
