using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateAfter(typeof(DamageEventSystem))]
public class DestroyEventSystem : SystemBase
{
    [BurstCompile]
    private struct DestroyJob : IJobChunk
    {
        [ReadOnly] public ArchetypeChunkEntityType EntityType;

        public EntityCommandBuffer EntityCommandBuffer;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Entity> entityArray = chunk.GetNativeArray(EntityType);

            for (var i = 0; i < chunk.Count; i++)
            {
                EntityCommandBuffer.DestroyEntity(entityArray[i]);
            }
        }
    }

    private static readonly ComponentType[] ComponentTypes = { ComponentType.ReadOnly<DestroyEventTag>() };

    private EntityCommandBufferSystem _entityCommandBufferSystem;
    private EntityQuery _entityQuery;

    protected override void OnCreate()
    {
        _entityCommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
        _entityQuery = GetEntityQuery(ComponentTypes);
    }

    protected override void OnUpdate()
    {
        Dependency = new DestroyJob
        {
            EntityType = GetArchetypeChunkEntityType(),
            EntityCommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer()
        }.ScheduleSingle(_entityQuery, Dependency);

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
