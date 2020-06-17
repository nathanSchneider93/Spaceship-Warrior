using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public sealed partial class ScreenToWorldAimPositionSystem
{
    [BurstCompile]
    private struct SetWorldPositionJob : IJobChunk
    {
        [ReadOnly, DeallocateOnJobCompletion] public NativeArray<float3> Positions;

        public ArchetypeChunkComponentType<AimWorldPosition> AimWorldPositionType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<AimWorldPosition> aimWorldPositionArray = chunk.GetNativeArray(AimWorldPositionType);

            for (var i = 0; i < chunk.Count; i++)
            {
                AimWorldPosition aimWorldPosition = aimWorldPositionArray[i];
                aimWorldPosition.Value = Positions[i];
                aimWorldPositionArray[i] = aimWorldPosition;
            }
        }
    }
}
