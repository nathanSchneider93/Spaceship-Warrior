using UnityEngine;

namespace SpaceshipWarrior.InputModule
{
    [CreateAssetMenu(menuName = "Create " + nameof(KeyboardInputController))]
    public sealed class KeyboardInputController : BaseInputController
    {
        [SerializeField] private KeyCode _shootKey;

        public override void Initialize()
        {
        }

        public override bool GetShootKeyDown()
        {
            return Input.GetKeyDown(_shootKey);
        }

        public override int GetHorizontalAxis()
        {
            return (int)Input.GetAxisRaw("Horizontal");
        }
    }
}
