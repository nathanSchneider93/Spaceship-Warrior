using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(LateSimulationSystemGroup)), UpdateBefore(typeof(DamageEventSystem))]
public sealed class ProjectileCollisionSystem : SystemBase
{
    private struct CollisionJob : IJobChunk
    {
        [ReadOnly] public ArchetypeChunkEntityType EntityType;
        [ReadOnly] public ArchetypeChunkBufferType<CollisionEventBufferElement> CollisionEventBufferType;
        [ReadOnly] public ComponentDataFromEntity<CurrentHealth> CurrentHealthGroup;
        [ReadOnly] public ComponentDataFromEntity<Damage> DamageGroup;

        public ComponentDataFromEntity<DamageEvent> DamageEventGroup;
        public EntityCommandBuffer EntityCommandBuffer;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Entity> entityArray = chunk.GetNativeArray(EntityType);
            BufferAccessor<CollisionEventBufferElement> collisionEventBufferAccessor = chunk.GetBufferAccessor(CollisionEventBufferType);

            for (var i = 0; i < chunk.Count; i++)
            {
                Entity entityA = entityArray[i];
                DynamicBuffer<CollisionEventBufferElement> collisionEventBuffer = collisionEventBufferAccessor[i];

                for (var j = 0; j < collisionEventBuffer.Length; j++)
                {
                    CollisionEventBufferElement collisionEvent = collisionEventBuffer[j];

                    if (collisionEvent.State != PhysicsEventState.Enter)
                    {
                        continue;
                    }

                    Entity entityB = collisionEvent.Entity;

                    bool currentHealthExistsInA = CurrentHealthGroup.HasComponent(entityA);
                    bool currentHealthExistsInB = CurrentHealthGroup.HasComponent(entityB);
                    bool damageExistsInA = DamageGroup.HasComponent(entityA);
                    bool damageExistsInB = DamageGroup.HasComponent(entityB);

                    if (damageExistsInA && currentHealthExistsInB)
                    {
                        AddOrUpdateDamageEvent(entityB, DamageGroup[entityA]);
                    }

                    if (damageExistsInB && currentHealthExistsInA)
                    {
                        AddOrUpdateDamageEvent(entityA, DamageGroup[entityB]);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddOrUpdateDamageEvent(Entity entity, Damage damage)
        {
            if (DamageEventGroup.HasComponent(entity))
            {
                DamageEvent damageEvent = DamageEventGroup[entity];
                damageEvent.Value += damage.Value;
            }
            else
            {
                EntityCommandBuffer.AddComponent(entity, new DamageEvent
                {
                    Value = damage.Value
                });
            }
        }
    }

    private static readonly EntityQueryDesc ComponentTypes = new EntityQueryDesc
    {
        All = new [] { ComponentType.ReadOnly<CollisionEventBufferElement>() },
        Any = new[] { ComponentType.ReadOnly<CurrentHealth>(), ComponentType.ReadOnly<Damage>() }
    };

    private EntityCommandBufferSystem _entityCommandBufferSystem;
    private EntityQuery _entityQuery;

    protected override void OnCreate()
    {
        _entityQuery = GetEntityQuery(ComponentTypes);
        _entityCommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        Dependency = new CollisionJob
        {
            EntityType = GetArchetypeChunkEntityType(),
            CollisionEventBufferType = GetArchetypeChunkBufferType<CollisionEventBufferElement>(true),
            CurrentHealthGroup = GetComponentDataFromEntity<CurrentHealth>(true),
            DamageGroup = GetComponentDataFromEntity<Damage>(true),
            DamageEventGroup = GetComponentDataFromEntity<DamageEvent>(),
            EntityCommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer()
        }.ScheduleSingle(_entityQuery, Dependency);

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
