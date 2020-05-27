using UnityEngine;

namespace SpaceshipWarrior.EnemyModule
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private EnemyController _enemyPrefab;

        private EnemyController.TargetPositionHandler _getEnemyTargetPosition;

        public void Initialize(EnemyController.TargetPositionHandler getEnemyTargetPosition)
        {
            _getEnemyTargetPosition = getEnemyTargetPosition;
        }

        public void OnUpdate()
        {
        }
    }
}
