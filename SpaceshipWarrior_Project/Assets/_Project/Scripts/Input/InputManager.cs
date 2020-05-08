using UnityEngine;

namespace SpaceshipWarrior.InputModule
{
    public class InputManager : MonoBehaviour
    {
        public delegate void KeyPressedHandler();

        public delegate void AxisChangedHandler(int value);

        public event KeyPressedHandler OnShootKeyPressed;
        public event AxisChangedHandler OnHorizontalAxisChanged;

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
            int horizontalAxis = CurrentController.GetHorizontalAxis();
            OnHorizontalAxisChanged?.Invoke(horizontalAxis);

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
