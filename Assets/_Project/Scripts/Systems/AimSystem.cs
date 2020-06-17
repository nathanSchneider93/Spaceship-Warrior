using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(TransformSystemGroup))]
public sealed class AimSystem : SystemBase
{
    [BurstCompile]
    private struct AimJob : IJobChunk
    {
        [ReadOnly] public float3 UpDirection;
        [ReadOnly] public ArchetypeChunkComponentType<Translation> TranslationType;
        [ReadOnly] public ArchetypeChunkComponentType<AimWorldPosition> AimWorldPositionType;

        public ArchetypeChunkComponentType<Rotation> RotationType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Translation> translationArray = chunk.GetNativeArray(TranslationType);
            NativeArray<AimWorldPosition> aimWorldPositionArray = chunk.GetNativeArray(AimWorldPositionType);
            NativeArray<Rotation> rotationArray = chunk.GetNativeArray(RotationType);

            for (var i = 0; i < chunk.Count; i++)
            {
                float3 lookDirection = math.normalize(aimWorldPositionArray[i].Value - translationArray[i].Value);
                rotationArray[i] = new Rotation { Value = quaternion.LookRotation(lookDirection, UpDirection) };
            }
        }
    }

    private static readonly float3 Up = math.up();
    private static readonly ComponentType[] ComponentTypes =
    {
        ComponentType.ReadWrite<Rotation>(),
        ComponentType.ReadOnly<Translation>(),
        ComponentType.ReadOnly<AimWorldPosition>(),
        ComponentType.ReadOnly<PlayerTag>()
    };

    private EntityQuery _entityQuery;

    protected override void OnCreate()
    {
        _entityQuery = GetEntityQuery(ComponentTypes);
    }

    protected override void OnUpdate()
    {
        float3 upDirection = Up;

        Dependency = new AimJob
        {
            UpDirection = upDirection,
            TranslationType = GetArchetypeChunkComponentType<Translation>(true),
            AimWorldPositionType = GetArchetypeChunkComponentType<AimWorldPosition>(true),
            RotationType = GetArchetypeChunkComponentType<Rotation>()
        }.ScheduleSingle(_entityQuery, Dependency);
    }
}
