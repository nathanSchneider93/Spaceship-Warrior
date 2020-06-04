using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    public sealed class HierarchyModeScope : GUI.Scope
    {
        private readonly bool HierarchyMode;

        public HierarchyModeScope()
        {
            HierarchyMode = EditorGUIUtility.hierarchyMode;
        }

        public HierarchyModeScope(bool hierarchyMode)
        {
            HierarchyMode = EditorGUIUtility.hierarchyMode;
            EditorGUIUtility.hierarchyMode = hierarchyMode;
        }

        protected override void CloseScope()
        {
            EditorGUIUtility.hierarchyMode = HierarchyMode;
        }
    }
}
