using UnityEngine;

namespace SpaceshipWarrior.PlayerModule
{
    public class PlayerCannon : MonoBehaviour
    {
        [SerializeField] private Transform _origin;
        [SerializeField] private PlayerCannonProjectile _projectile;

        public void Fire()
        {
            PlayerCannonProjectile projectile = Instantiate(_projectile, _origin.position, _origin.rotation);
            projectile.Initialize(_origin.forward);
        }
    }
}
