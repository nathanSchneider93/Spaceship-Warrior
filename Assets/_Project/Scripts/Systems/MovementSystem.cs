using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

[UpdateBefore(typeof(AimSystem))]
public sealed class MovementSystem : SystemBase
{
    [BurstCompile]
    private struct MovementJob : IJobChunk
    {
        [ReadOnly] public ArchetypeChunkComponentType<IsMovementEnabled> IsMovementEnabledType;
        [ReadOnly] public ArchetypeChunkComponentType<MovementDirection> MovementDirectionType;
        [ReadOnly] public ArchetypeChunkComponentType<MovementSpeed> MovementSpeedType;

        public ArchetypeChunkComponentType<PhysicsVelocity> PhysicsVelocityType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<IsMovementEnabled> isMovementEnabledArray = chunk.GetNativeArray(IsMovementEnabledType);
            NativeArray<MovementDirection> movementDirectionArray = chunk.GetNativeArray(MovementDirectionType);
            NativeArray<MovementSpeed> movementSpeedArray = chunk.GetNativeArray(MovementSpeedType);
            NativeArray<PhysicsVelocity> physicsVelocityArray = chunk.GetNativeArray(PhysicsVelocityType);

            for (var i = 0; i < chunk.Count; i++)
            {
                if (!isMovementEnabledArray[i].Value)
                {
                    continue;
                }

                PhysicsVelocity physicsVelocity = physicsVelocityArray[i];
                physicsVelocity.Linear = movementDirectionArray[i].Value * movementSpeedArray[i].Value;
                physicsVelocityArray[i] = physicsVelocity;
            }
        }
    }

    private static readonly ComponentType[] ComponentTypes =
    {
        ComponentType.ReadWrite<PhysicsVelocity>(),
        ComponentType.ReadOnly<IsMovementEnabled>(),
        ComponentType.ReadOnly<MovementDirection>(),
        ComponentType.ReadOnly<MovementSpeed>()
    };

    private EntityQuery _entityQuery;

    protected override void OnCreate()
    {
        _entityQuery = GetEntityQuery(ComponentTypes);
    }

    protected override void OnUpdate()
    {
        Dependency = new MovementJob
        {
            IsMovementEnabledType = GetArchetypeChunkComponentType<IsMovementEnabled>(true),
            MovementDirectionType = GetArchetypeChunkComponentType<MovementDirection>(true),
            MovementSpeedType = GetArchetypeChunkComponentType<MovementSpeed>(true),
            PhysicsVelocityType = GetArchetypeChunkComponentType<PhysicsVelocity>()
        }.ScheduleSingle(_entityQuery, Dependency);
    }
}
