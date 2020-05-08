using SpaceshipWarrior.InputModule;
using SpaceshipWarrior.PlayerModule;
using UnityEngine;

namespace SpaceshipWarrior
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private PlayerController _playerController;

        private void Awake()
        {
            _inputManager.Initialize();
            _inputManager.OnHorizontalAxisChanged += _playerController.SetMovementDirection;
            _inputManager.OnShootKeyPressed += _playerController.FireCannon;

            _playerController.Initialize();
        }

        private void Update()
        {
            _inputManager.OnUpdate();
            _playerController.OnUpdate();
        }
    }
}
