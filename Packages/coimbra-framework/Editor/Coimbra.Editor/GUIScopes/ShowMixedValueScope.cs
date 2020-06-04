using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    public sealed class ShowMixedValueScope : GUI.Scope
    {
        private readonly bool ShowMixedValue;

        public ShowMixedValueScope()
        {
            ShowMixedValue = EditorGUI.showMixedValue;
        }

        public ShowMixedValueScope(bool showMixedValue)
        {
            ShowMixedValue = EditorGUI.showMixedValue;
            EditorGUI.showMixedValue = showMixedValue;
        }

        protected override void CloseScope()
        {
            EditorGUI.showMixedValue = ShowMixedValue;
        }
    }
}
