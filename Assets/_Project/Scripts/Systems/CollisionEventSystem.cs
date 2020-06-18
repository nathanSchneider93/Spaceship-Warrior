using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[DisableAutoCreation]
[UpdateAfter(typeof(ExportPhysicsWorld))]
[UpdateBefore(typeof(EndFramePhysicsSystem))]
public sealed partial class CollisionEventSystem : SystemBase
{
    private static readonly ComponentType[] ComponentTypes =
    {
        ComponentType.ReadOnly<PhysicsCollider>(),
        ComponentType.ReadOnly<CollisionEventListener>(),
        ComponentType.ReadWrite<CollisionEventBufferElement>()
    };

    private BuildPhysicsWorld _buildPhysicsWorldSystem;
    private StepPhysicsWorld _stepPhysicsWorldSystem;
    private EndFramePhysicsSystem _endFramePhysicsSystem;
    private EntityQuery _entityQuery;

    protected override void OnCreate()
    {
        _buildPhysicsWorldSystem = World.GetExistingSystem<BuildPhysicsWorld>();
        _stepPhysicsWorldSystem = World.GetExistingSystem<StepPhysicsWorld>();
        _endFramePhysicsSystem = World.GetExistingSystem<EndFramePhysicsSystem>();
        _entityQuery = GetEntityQuery(ComponentTypes);
    }

    protected override void OnUpdate()
    {
        ArchetypeChunkBufferType<CollisionEventBufferElement> collisionEventBufferType = GetArchetypeChunkBufferType<CollisionEventBufferElement>();

        Dependency = new PreprocessJob
        {
            CollisionEventBufferType = collisionEventBufferType
        }.ScheduleParallel(_entityQuery, Dependency);

        Dependency = new ProcessJob
        {
            PhysicsWorld = _buildPhysicsWorldSystem.PhysicsWorld,
            CollisionEventListenerGroup = GetComponentDataFromEntity<CollisionEventListener>(true),
            CollisionEventBufferGroup = GetBufferFromEntity<CollisionEventBufferElement>()
        }.Schedule(_stepPhysicsWorldSystem.Simulation, ref _buildPhysicsWorldSystem.PhysicsWorld, Dependency);

        _endFramePhysicsSystem.HandlesToWaitFor.Add(Dependency);

        Dependency = new PostprocessJob
        {
            CollisionEventBufferType = collisionEventBufferType,
        }.ScheduleParallel(_entityQuery, Dependency);
    }
}
