using UnityEngine;

namespace SpaceshipWarrior.EnemyModule
{
    public class EnemyController : MonoBehaviour
    {
        public delegate Vector3 TargetPositionHandler();

        private TargetPositionHandler _getTargetPosition;

        public void Initialize(TargetPositionHandler getTargetPosition)
        {
            _getTargetPosition = getTargetPosition;
        }
    }
}
