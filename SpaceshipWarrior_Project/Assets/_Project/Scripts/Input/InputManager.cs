using UnityEngine;

namespace SpaceshipWarrior.InputModule
{
    public class InputManager : MonoBehaviour
    {
        public delegate void KeyPressedHandler();

        public event KeyPressedHandler OnShootKeyPressed;

        [SerializeField] private ArduinoInputController _arduinoController;
        [SerializeField] private KeyboardInputController _keyboardController;
        [SerializeField] private KeyCode _switchInputModeKey;

        private BaseInputController CurrentController { get; set; }

        public void Initialize()
        {
            CurrentController = _keyboardController;
        }

        public void OnUpdate()
        {
            if (CurrentController.GetShootKeyDown())
            {
                OnShootKeyPressed?.Invoke();
            }

            if (Input.GetKeyDown(_switchInputModeKey))
            {
                SwitchCurrentController();
            }
        }

        private void SwitchCurrentController()
        {
            if (CurrentController == _arduinoController)
            {
                CurrentController = _keyboardController;
            }
            else
            {
                CurrentController = _arduinoController;
            }
        }
    }
}
