﻿using System;
using System.IO.Ports;
using UnityEngine;

namespace SpaceshipWarrior.InputModule
{
    public class InputManager : MonoBehaviour
    {
        public delegate void KeyPressedHandler();

        public delegate void LookPointChangedHandler(Vector3 value);

        public delegate void VerticalRotationDeltaChangedHandler(float value);

        public event KeyPressedHandler OnFireKeyPressed;
        public event LookPointChangedHandler OnLookPointChanged;
        public event VerticalRotationDeltaChangedHandler OnVerticalRotationDeltaChanged;

        [Header("Arduino")]
        [SerializeField] private string _arduinoPortName = "\\\\.\\" + "COM4";
        [SerializeField] private int _arduinoBaudRate = 9600;

        [Header("Keyboard and Mouse")]
        [SerializeField] private KeyCode _fireKey;

        private Action _onUpdate;
        private SerialPort _serialPort;

        public void Initialize()
        {
            var foundPort = false;
            string[] availablePorts = SerialPort.GetPortNames();

            foreach (string portName in availablePorts)
            {
                if (!string.Equals(portName, _arduinoPortName))
                {
                    continue;
                }

                foundPort = true;

                break;
            }

            if (!foundPort)
            {
                _onUpdate = OnUpdateKeyboardAndMouse;

                return;
            }

            _serialPort = new SerialPort(_arduinoPortName, _arduinoBaudRate) { ReadTimeout = 2000 };
            _serialPort.Open();
            _onUpdate = OnUpdateArduino;
        }

        public void OnUpdate()
        {
            _onUpdate?.Invoke();
        }

        private void OnUpdateKeyboardAndMouse()
        {
            OnLookPointChanged?.Invoke(Input.mousePosition);

            if (Input.GetKeyDown(_fireKey))
            {
                OnFireKeyPressed?.Invoke();
            }
        }

        private void OnUpdateArduino()
        {
            var data = "Null received";

            if (_serialPort.IsOpen)
            {
                data = _serialPort.ReadLine();
            }

            if (string.IsNullOrWhiteSpace(data))
            {
                return;
            }

            const float tolerance = 0.025f;
            const char splitter = ';';

            string[] rawData = data.Split(splitter);
            float verticalAngle = int.Parse(rawData[4]);

            if (Mathf.Abs(verticalAngle) < tolerance)
            {
                verticalAngle = 0f;
            }

            OnVerticalRotationDeltaChanged?.Invoke(-verticalAngle);
        }
    }
}
