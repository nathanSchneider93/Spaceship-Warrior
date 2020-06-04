using System.Collections;
using UnityEngine;

namespace SpaceshipWarrior.PlayerModule
{
    public class PlayerCannon : MonoBehaviour
    {
        [SerializeField] private Transform _origin;
        [SerializeField] private PlayerCannonProjectile _projectilePrefab;
        [SerializeField] private int _fireRate;

        private bool _locked;

        public void Fire()
        {
            if (_locked)
            {
                return;
            }

            PlayerCannonProjectile projectile = Instantiate(_projectilePrefab, _origin.position, _origin.rotation);
            projectile.Initialize(_origin.forward);

            StartCoroutine(LockTimerCoroutine());
        }

        private IEnumerator LockTimerCoroutine()
        {
            _locked = true;

            yield return new WaitForSeconds(_fireRate);

            _locked = false;
        }
    }
}
