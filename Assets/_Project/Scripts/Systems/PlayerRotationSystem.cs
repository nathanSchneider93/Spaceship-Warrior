using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SpaceshipWarrior
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class PlayerRotationSystem : SystemBase
    {
        [BurstCompile]
        [RequireComponentTag(typeof(PlayerTag))]
        private struct UpdateJob : IJobChunk
        {
            [ReadOnly] public ArchetypeChunkComponentType<RotationDelta> RotationDeltaType;

            public ArchetypeChunkComponentType<Rotation> RotationType;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                UnityEngine.Debug.Log($"Running {nameof(UpdateJob)} from {nameof(PlayerRotationSystem)}.");

                NativeArray<RotationDelta> rotationDeltaArray = chunk.GetNativeArray(RotationDeltaType);
                NativeArray<Rotation> rotationArray = chunk.GetNativeArray(RotationType);

                for (var i = 0; i < chunk.Count; i++)
                {
                    float radians = math.radians(rotationDeltaArray[i].Value);
                    quaternion current = rotationArray[i].Value;
                    quaternion delta = quaternion.AxisAngle(math.up(), radians);

                    rotationArray[i] = new Rotation { Value = math.mul(current, delta) };
                }
            }
        }

        private static readonly ComponentType[] ComponentTypes =
        {
            ComponentType.ReadWrite<Rotation>(),
            ComponentType.ReadOnly<RotationDelta>()
        };

        private EntityQuery _entityQuery;

        protected override void OnCreate()
        {
            _entityQuery = GetEntityQuery(ComponentTypes);
        }

        protected override void OnUpdate()
        {
            Dependency = new UpdateJob
            {
                RotationDeltaType = GetArchetypeChunkComponentType<RotationDelta>(true),
                RotationType = GetArchetypeChunkComponentType<Rotation>()
            }.ScheduleSingle(_entityQuery, Dependency);
        }
    }
}
