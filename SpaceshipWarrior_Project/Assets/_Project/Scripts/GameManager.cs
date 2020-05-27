using SpaceshipWarrior.InputModule;
using SpaceshipWarrior.PlayerModule;
using SpaceshipWarrior.EnemyModule;
using UnityEngine;

namespace SpaceshipWarrior
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private EnemyManager _enemyManager;

        private void Awake()
        {
            _inputManager.Initialize();
            _inputManager.OnShootKeyPressed += _playerController.FireCannon;

            _playerController.Initialize();
        }

        private void Update()
        {
            _inputManager.OnUpdate();
            _playerController.OnUpdate();
            _enemyManager.OnUpdate();
        }
    }
}
