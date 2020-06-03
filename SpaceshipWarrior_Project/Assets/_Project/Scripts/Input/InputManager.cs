using UnityEngine;

namespace SpaceshipWarrior.InputModule
{
    public class InputManager : MonoBehaviour
    {
        public delegate void KeyPressedHandler();

        public delegate void LookDirectionChangedHandler(Vector3 value);

        public event KeyPressedHandler OnShootKeyPressed;
        public event LookDirectionChangedHandler OnLookPointChanged;

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
            Vector3 lookPoint = CurrentController.GetLookPoint();
            OnLookPointChanged?.Invoke(lookPoint);

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
