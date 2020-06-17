using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(PlayerInputSystem))]
public sealed partial class ScreenToWorldAimPositionSystem : SystemBase
{
    private static readonly ComponentType[] ComponentTypes =
    {
        ComponentType.ReadWrite<AimWorldPosition>(),
        ComponentType.ReadOnly<AimScreenPosition>(),
        ComponentType.ReadOnly<Translation>()
    };

    private CameraSystem _cameraSystem;
    private EntityQuery _entityQuery;

    protected override void OnCreate()
    {
        _cameraSystem = World.GetExistingSystem<CameraSystem>();
        _entityQuery = GetEntityQuery(ComponentTypes);
    }

    protected override void OnUpdate()
    {
        var positions = new NativeArray<float3>(_entityQuery.CalculateEntityCount(), Allocator.TempJob);

        Dependency.Complete();

        var getScreenPositionJob = new GetScreenPositionJob
        {
            CameraPosition = _cameraSystem.GetCameraPosition(),
            TranslationType = GetArchetypeChunkComponentType<Translation>(true),
            AimScreenPositionType = GetArchetypeChunkComponentType<AimScreenPosition>(true),
            Result = positions
        };

        getScreenPositionJob.Run(_entityQuery);
        positions = getScreenPositionJob.Result;

        for (var i = 0; i < positions.Length; i++)
        {
            positions[i] = _cameraSystem.ScreenToWorldPoint(positions[i]);
        }

        Dependency = new SetWorldPositionJob
        {
            Positions = positions,
            AimWorldPositionType = GetArchetypeChunkComponentType<AimWorldPosition>()
        }.Schedule(_entityQuery, Dependency);
    }
}
