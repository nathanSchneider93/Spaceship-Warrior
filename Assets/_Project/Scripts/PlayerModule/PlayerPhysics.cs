using UnityEngine;

namespace SpaceshipWarrior.PlayerModule
{
    public class PlayerPhysics : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private Rigidbody _rigidbody;

        private Transform _transform;

        public Vector3 Position => _transform.position;
        public Vector3 EulerAngles => _transform.rotation.eulerAngles;

        public void Initialize()
        {
            _transform = transform;
        }

        public void OnUpdate()
        {
            _transform.position += _transform.forward * (_speed * Time.deltaTime);
        }

        public void LookAtPoint(Vector3 value)
        {
            Vector3 lookDirection = (value - Position).normalized;
            _transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }

        public void AddEulerAnglesDelta(Vector3 value)
        {
            Vector3 eulerAngles = EulerAngles + value;
            _transform.rotation = Quaternion.Euler(eulerAngles);
        }
    }
}
