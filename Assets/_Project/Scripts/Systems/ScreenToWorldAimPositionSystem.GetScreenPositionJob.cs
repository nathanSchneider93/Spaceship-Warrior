using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public sealed partial class ScreenToWorldAimPositionSystem
{
    [BurstCompile]
    private struct GetScreenPositionJob : IJobChunk
    {
        [ReadOnly] public float3 CameraPosition;
        [ReadOnly] public ArchetypeChunkComponentType<Translation> TranslationType;
        [ReadOnly] public ArchetypeChunkComponentType<AimScreenPosition> AimScreenPositionType;

        public NativeArray<float3> Result;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Translation> translationArray = chunk.GetNativeArray(TranslationType);
            NativeArray<AimScreenPosition> aimScreenPositionArray = chunk.GetNativeArray(AimScreenPositionType);

            for (var i = 0; i < chunk.Count; i++)
            {
                Translation translation = translationArray[i];
                AimScreenPosition aimScreenPosition = aimScreenPositionArray[i];

                Result[i] = new float3(aimScreenPosition.Value, CameraPosition.y - translation.Value.y);
            }
        }
    }
}
