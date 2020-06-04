using UnityEngine;

namespace SpaceshipWarrior.PlayerModule
{
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerPhysics _physics;
        [SerializeField] private PlayerCannon _cannon;

        public void Initialize()
        {
            _physics.Initialize();
        }

        public void OnUpdate()
        {
            _physics.OnUpdate();
        }

        public void FireCannon()
        {
            _cannon.Fire();
        }

        public void LookAtScreenPoint(Vector3 value)
        {
            _physics.LookAtPoint(CalculateWorldPoint(value));
        }

        private Vector3 CalculateWorldPoint(Vector3 mousePosition)
        {
            Vector3 cameraPosition = CameraSystem.GetCameraPosition();
            mousePosition.z = cameraPosition.y - _physics.Position.y;

            return CameraSystem.ScreenToWorldPoint(mousePosition);
        }
    }
}
