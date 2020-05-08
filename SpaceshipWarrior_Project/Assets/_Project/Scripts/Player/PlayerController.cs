using UnityEngine;

namespace SpaceshipWarrior.PlayerModule
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerPhysics _physics;
        [SerializeField] private PlayerCannon _cannon;

        public void Initialize()
        {
        }

        public void OnUpdate()
        {
            _physics.OnUpdate();
        }

        public void SetMovementDirection(int value)
        {
            _physics.SetMovementDirection(value);
        }

        public void FireCannon()
        {
            _cannon.Fire();
        }
    }
}
