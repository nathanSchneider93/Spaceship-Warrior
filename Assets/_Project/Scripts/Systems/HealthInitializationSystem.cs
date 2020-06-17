using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public sealed class HealthInitializationSystem : SystemBase
{
    [BurstCompile]
    private struct InitializeHealthJob : IJobChunk
    {
        [ReadOnly] public ArchetypeChunkEntityType EntityType;
        [ReadOnly] public ArchetypeChunkComponentType<MaxHealth> MaxHealthType;

        public ArchetypeChunkComponentType<CurrentHealth> CurrentHealthType;
        public EntityCommandBuffer EntityCommandBuffer;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Entity> entityArray = chunk.GetNativeArray(EntityType);
            NativeArray<MaxHealth> maxHealthArray = chunk.GetNativeArray(MaxHealthType);
            NativeArray<CurrentHealth> currentHealthArray = chunk.GetNativeArray(CurrentHealthType);

            for (var i = 0; i < chunk.Count; i++)
            {
                Entity entity = entityArray[i];
                MaxHealth maxHealth = maxHealthArray[i];
                CurrentHealth currentHealth = currentHealthArray[i];

                currentHealth.Value = maxHealth.Value;
                currentHealthArray[i] = currentHealth;

                EntityCommandBuffer.AddComponent<HealthInitializedTag>(entity);
            }
        }
    }

    private static readonly ComponentType[] ComponentTypes =
    {
        ComponentType.ReadWrite<CurrentHealth>(),
        ComponentType.ReadOnly<MaxHealth>(),
        ComponentType.Exclude<HealthInitializedTag>()
    };

    private EntityCommandBufferSystem _entityCommandBufferSystem;
    private EntityQuery _entityQuery;

    protected override void OnCreate()
    {
        _entityCommandBufferSystem = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
        _entityQuery = GetEntityQuery(ComponentTypes);
    }

    protected override void OnUpdate()
    {
        Dependency = new InitializeHealthJob
        {
            EntityType = GetArchetypeChunkEntityType(),
            MaxHealthType = GetArchetypeChunkComponentType<MaxHealth>(true),
            CurrentHealthType = GetArchetypeChunkComponentType<CurrentHealth>(),
            EntityCommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer()
        }.ScheduleSingle(_entityQuery, Dependency);

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
