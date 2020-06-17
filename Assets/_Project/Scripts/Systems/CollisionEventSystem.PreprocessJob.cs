using Unity.Burst;
using Unity.Entities;

public sealed partial class CollisionEventSystem
{
    [BurstCompile]
    private struct PreprocessJob : IJobChunk
    {
        public ArchetypeChunkBufferType<CollisionEventBufferElement> CollisionEventBufferType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            BufferAccessor<CollisionEventBufferElement> collisionEventBufferAccessor = chunk.GetBufferAccessor(CollisionEventBufferType);

            for (var i = 0; i < chunk.Count; i++)
            {
                DynamicBuffer<CollisionEventBufferElement> collisionEventBuffer = collisionEventBufferAccessor[i];

                for (var j = 0; j < collisionEventBuffer.Length; j++)
                {
                    CollisionEventBufferElement collisionEventBufferElement = collisionEventBuffer[j];
                    collisionEventBufferElement.IsColliding = false;
                    collisionEventBuffer[j] = collisionEventBufferElement;
                }
            }
        }
    }
}
