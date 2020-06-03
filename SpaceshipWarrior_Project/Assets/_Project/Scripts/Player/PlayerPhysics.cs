using UnityEngine;

namespace SpaceshipWarrior.PlayerModule
{
    public class PlayerPhysics : MonoBehaviour
    {
        [SerializeField] private float _speed;

        public Vector3 Position => transform.position;

        public void OnUpdate()
        {
        }

        public void LookAtPoint(Vector3 value)
        {
            Vector3 lookDirection = (value - Position).normalized;
            transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }
    }
}
