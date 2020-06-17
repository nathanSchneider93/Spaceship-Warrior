using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public sealed class FireEventSystem : SystemBase
{
    [BurstCompile]
    private struct FireJob : IJobChunk
    {
        [ReadOnly] public ArchetypeChunkEntityType EntityType;
        [ReadOnly] public ArchetypeChunkComponentType<BarrelReference> BarrelReferenceType;
        [ReadOnly] public ArchetypeChunkComponentType<ProjectilePrefabReference> ProjectilePrefabReferenceType;
        [ReadOnly] public ComponentDataFromEntity<LocalToWorld> LocalToWorldGroup;

        public EntityCommandBuffer EntityCommandBuffer;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Entity> entityArray = chunk.GetNativeArray(EntityType);
            NativeArray<BarrelReference> barrelReferenceArray = chunk.GetNativeArray(BarrelReferenceType);
            NativeArray<ProjectilePrefabReference> projectilePrefabReferenceArray = chunk.GetNativeArray(ProjectilePrefabReferenceType);

            for (var i = 0; i < chunk.Count; i++)
            {
                Entity entity = entityArray[i];
                BarrelReference barrelReference = barrelReferenceArray[i];
                ProjectilePrefabReference projectilePrefabReference = projectilePrefabReferenceArray[i];

                if (!LocalToWorldGroup.HasComponent(barrelReference.Value))
                {
                    continue;
                }

                LocalToWorld projectileLocalToWorld = LocalToWorldGroup[barrelReference.Value];
                Entity projectileEntity = EntityCommandBuffer.Instantiate(projectilePrefabReference.Value);

                EntityCommandBuffer.SetComponent(projectileEntity, new Translation { Value = projectileLocalToWorld.Position });
                EntityCommandBuffer.SetComponent(projectileEntity, new Rotation { Value = projectileLocalToWorld.Rotation });
                EntityCommandBuffer.SetComponent(projectileEntity, new MovementDirection { Value = projectileLocalToWorld.Forward });
                EntityCommandBuffer.RemoveComponent<FireEventTag>(entity);
            }
        }
    }

    private static readonly ComponentType[] ComponentTypes =
    {
        ComponentType.ReadOnly<BarrelReference>(),
        ComponentType.ReadOnly<ProjectilePrefabReference>(),
        ComponentType.ReadOnly<FireEventTag>()
    };

    private EntityCommandBufferSystem _entityCommandBufferSystem;
    private EntityQuery _entityQuery;

    protected override void OnCreate()
    {
        _entityCommandBufferSystem = World.GetExistingSystem<BeginInitializationEntityCommandBufferSystem>();
        _entityQuery = GetEntityQuery(ComponentTypes);
    }

    protected override void OnUpdate()
    {
        Dependency = new FireJob
        {
            EntityType = GetArchetypeChunkEntityType(),
            BarrelReferenceType = GetArchetypeChunkComponentType<BarrelReference>(true),
            ProjectilePrefabReferenceType = GetArchetypeChunkComponentType<ProjectilePrefabReference>(true),
            LocalToWorldGroup = GetComponentDataFromEntity<LocalToWorld>(true),
            EntityCommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer()
        }.ScheduleSingle(_entityQuery, Dependency);

        _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
