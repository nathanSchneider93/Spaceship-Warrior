using System.IO.Ports;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace SpaceshipWarrior
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class PlayerInputSystem : SystemBase
    {
        [BurstCompile]
        [RequireComponentTag(typeof(PlayerTag))]
        private struct UpdateJob : IJobChunk
        {
            [ReadOnly] public float VerticalRotationDelta;

            public ArchetypeChunkComponentType<RotationDelta> RotationDeltaType;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                UnityEngine.Debug.Log($"Running {nameof(UpdateJob)} from {nameof(PlayerInputSystem)}.");

                NativeArray<RotationDelta> rotationDeltaArray = chunk.GetNativeArray(RotationDeltaType);

                for (var i = 0; i < chunk.Count; i++)
                {
                    RotationDelta rotationDelta = rotationDeltaArray[i];
                    rotationDelta.Value = VerticalRotationDelta;
                    rotationDeltaArray[i] = rotationDelta;
                }
            }
        }

        private static readonly ComponentType[] ComponentTypes = { ComponentType.ReadWrite<RotationDelta>() };

        private const int DefaultBaudRate = 9600;
        private const float VerticalAngleFactor = 45f;
        private const float NormalizerFactor = 1.0f / 32768.0f;
        private const string DefaultPortName = "COM4";

        private EntityQuery _entityQuery;
        private SerialPort _serialPort;

        protected override void OnCreate()
        {
            _entityQuery = GetEntityQuery(ComponentTypes);
            _serialPort = new SerialPort(DefaultPortName, DefaultBaudRate);
        }

        protected override void OnStartRunning()
        {
            _serialPort.Open();
        }

        protected override void OnUpdate()
        {
            Dependency = new UpdateJob
            {
                VerticalRotationDelta = GetVerticalRotationDelta(),
                RotationDeltaType = GetArchetypeChunkComponentType<RotationDelta>()
            }.ScheduleSingle(_entityQuery, Dependency);
        }

        protected override void OnStopRunning()
        {
            _serialPort.Close();
        }

        private float GetVerticalRotationDelta()
        {
            if (_serialPort.IsOpen == false)
            {
                return 0f;
            }

            string data = _serialPort.ReadLine();

            if (string.IsNullOrWhiteSpace(data))
            {
                return 0f;
            }

            const float tolerance = 0.025f;
            const char splitter = ';';

            string[] rawData = data.Split(splitter);
            float gyroscopeVerticalAngle = int.Parse(rawData[4]) * NormalizerFactor;

            if (math.abs(gyroscopeVerticalAngle) < tolerance)
            {
                gyroscopeVerticalAngle = 0f;
            }

            return -gyroscopeVerticalAngle * VerticalAngleFactor;
        }
    }
}
