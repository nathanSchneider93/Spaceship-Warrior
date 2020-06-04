using UnityEditor;

namespace Coimbra
{
    [CustomPropertyDrawer(typeof(PropertyAttributeBase), true)]
    public sealed class PropertyAttributeDrawer : PropertyAttributeDrawer<PropertyAttributeBase> { }
}
