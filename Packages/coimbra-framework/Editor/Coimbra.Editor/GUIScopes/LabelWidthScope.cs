using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    public sealed class LabelWidthScope : GUI.Scope
    {
        private readonly float Increment;

        public LabelWidthScope(float increment)
        {
            Increment = increment;
            EditorGUIUtility.labelWidth += Increment;
        }

        protected override void CloseScope()
        {
            EditorGUIUtility.labelWidth -= Increment;
        }
    }
}
