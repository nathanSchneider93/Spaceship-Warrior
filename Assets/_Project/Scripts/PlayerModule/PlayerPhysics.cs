using UnityEngine;

namespace SpaceshipWarrior.PlayerModule
{
    public class PlayerPhysics : MonoBehaviour
    {
        [SerializeField] private float _speed;

        private Transform _transform;

        public Vector3 Position => _transform.position;

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
            //aqui muda se o jogador estiver apenas com mouse e teclado
            //Vector3 lookDirection = (value - Position).normalized;
            //transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }
    }
}
