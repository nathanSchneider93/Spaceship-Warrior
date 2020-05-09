using UnityEngine;

namespace SpaceshipWarrior.PlayerModule
{
    public class PlayerCannonProjectile : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private int _damage;
        [SerializeField] private float _speed;

        public void Initialize(Vector3 direction)
        {
            _rigidbody.velocity = direction * _speed;
        }

        private void OnCollisionEnter(Collision other)
        {
            var damageable = other.gameObject.GetComponent<IDamageable>();
            damageable?.TakeDamage(_damage);
        }
    }
}
