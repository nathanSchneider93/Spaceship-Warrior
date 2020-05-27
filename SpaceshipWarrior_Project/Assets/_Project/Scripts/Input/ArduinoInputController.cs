using System.IO.Ports;
using UnityEngine;

namespace SpaceshipWarrior.InputModule
{
    [CreateAssetMenu(menuName = "Create " + nameof(ArduinoInputController))]
    public sealed class ArduinoInputController : BaseInputController
    {
        [SerializeField] private int _baudRate;
        [SerializeField] private string _portName;

        private SerialPort _port;

        public override void Initialize()
        {
            _port = new SerialPort(_portName, _baudRate) { ReadTimeout = 1 };
            _port.Open();
        }

        public override bool GetShootKeyDown()
        {
            return false;
        }
    }
}
