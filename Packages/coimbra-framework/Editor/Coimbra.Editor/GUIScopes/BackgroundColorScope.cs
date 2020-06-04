using UnityEngine;

namespace Coimbra
{
    public sealed class BackgroundColorScope : GUI.Scope
    {
        private readonly Color BackgroundColor;

        public BackgroundColorScope()
        {
            BackgroundColor = GUI.backgroundColor;
        }

        public BackgroundColorScope(Color backgroundColor)
        {
            BackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundColor;
        }

        protected override void CloseScope()
        {
            GUI.backgroundColor = BackgroundColor;
        }
    }
}
