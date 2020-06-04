using JetBrains.Annotations;
using UnityEngine;

namespace Coimbra
{
    /// <summary>
    /// Use it with <see cref="SerializableList{T}"/> to show a reorderable list in the inspector.
    /// </summary>
    [PublicAPI]
    public sealed class ReorderableAttribute : PropertyAttribute
    {
        public readonly string ElementName;
        public readonly ReorderableElementDisplay ElementDisplay;
        public readonly ReorderablePermissions EditModePermissions;
        public readonly ReorderablePermissions PlayModePermissions;

        public ReorderableAttribute(ReorderableElementDisplay elementDisplay = ReorderableElementDisplay.Auto)
            : this(null, ReorderablePermissions.Everything, ReorderablePermissions.Everything, elementDisplay) { }

        public ReorderableAttribute(ReorderablePermissions permissions, ReorderableElementDisplay elementDisplay = ReorderableElementDisplay.Auto)
            : this(null, permissions, permissions, elementDisplay) { }

        public ReorderableAttribute(ReorderablePermissions editModePermissions, ReorderablePermissions playModePermissions, ReorderableElementDisplay elementDisplay = ReorderableElementDisplay.Auto)
            : this(null, editModePermissions, playModePermissions, elementDisplay) { }

        /// <param name="elementName">
        /// Change the element display name. If the element is an <see cref="UnityEngine.Object"/> you can use the value "name" to display the object's name.
        /// <para>
        /// To change it based on a method of the inspected object, use "MethodName" where the method signature is "string MethodName(IList, int)".
        /// </para>
        /// <para>
        /// To change it based on a SerializedField on the element, use the "$" as the first character. E.g. "$elementFieldName", "$elementFieldName.nested"
        /// </para>
        /// <para>
        /// To change it to a constant string followed by the element index, use "@" as the first character. E.g. "@MyElementName"
        /// </para>
        /// </param>
        public ReorderableAttribute(string elementName, ReorderableElementDisplay elementDisplay = ReorderableElementDisplay.Auto)
            : this(elementName, ReorderablePermissions.Everything, ReorderablePermissions.Everything, elementDisplay) { }

        /// <param name="elementName">
        /// Change the element display name. If the element is an <see cref="UnityEngine.Object"/> you can use the value "name" to display the object's name.
        /// <para>
        /// To change it based on a method of the inspected object, use "MethodName" where the method signature is "string MethodName(IList, int)".
        /// </para>
        /// <para>
        /// To change it based on a SerializedField on the element, use the "$" as the first character. E.g. "$elementFieldName", "$elementFieldName.nested"
        /// </para>
        /// <para>
        /// To change it to a constant string followed by the element index, use "@" as the first character. E.g. "@MyElementName"
        /// </para>
        /// </param>
        public ReorderableAttribute(string elementName, ReorderablePermissions permissions, ReorderableElementDisplay elementDisplay = ReorderableElementDisplay.Auto)
            : this(elementName, permissions, permissions, elementDisplay) { }

        /// <param name="elementName">
        /// Change the element display name. If the element is an <see cref="UnityEngine.Object"/> you can use the value "name" to display the object's name.
        /// <para>
        /// To change it based on a method of the inspected object, use "MethodName" where the method signature is "string MethodName(IList, int)".
        /// </para>
        /// <para>
        /// To change it based on a SerializedField on the element, use the "$" as the first character. E.g. "$elementFieldName", "$elementFieldName.nested"
        /// </para>
        /// <para>
        /// To change it to a constant string followed by the element index, use "@" as the first character. E.g. "@MyElementName"
        /// </para>
        /// </param>
        public ReorderableAttribute(string elementName, ReorderablePermissions editModePermissions, ReorderablePermissions playModePermissions, ReorderableElementDisplay elementDisplay = ReorderableElementDisplay.Auto)
        {
            ElementName = elementName;
            EditModePermissions = editModePermissions;
            PlayModePermissions = playModePermissions;
            ElementDisplay = elementDisplay;
        }
    }
}
