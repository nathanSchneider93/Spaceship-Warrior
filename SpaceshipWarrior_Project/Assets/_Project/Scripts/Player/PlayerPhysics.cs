using UnityEngine;

namespace SpaceshipWarrior.PlayerModule
{
    public class PlayerPhysics : MonoBehaviour
    {
        [SerializeField] private float _speed;

        private int _movementDirection;

        public void OnUpdate()
        {
            transform.Translate(Vector3.right * _movementDirection * _speed * Time.deltaTime);
        }

        public void SetMovementDirection(int value)
        {
            _movementDirection = value;
        }
    }
}
