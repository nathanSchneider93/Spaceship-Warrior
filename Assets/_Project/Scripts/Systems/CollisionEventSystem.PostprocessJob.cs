using Unity.Burst;
using Unity.Entities;

public sealed partial class CollisionEventSystem
{
    [BurstCompile]
    private struct PostprocessJob : IJobChunk
    {
        public ArchetypeChunkBufferType<CollisionEventBufferElement> CollisionEventBufferType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            BufferAccessor<CollisionEventBufferElement> collisionEventBufferAccessor = chunk.GetBufferAccessor(CollisionEventBufferType);

            for (var i = 0; i < chunk.Count; i++)
            {
                DynamicBuffer<CollisionEventBufferElement> collisionEventBuffer = collisionEventBufferAccessor[i];

                for (int j = collisionEventBuffer.Length - 1; j >= 0; j--)
                {
                    CollisionEventBufferElement collisionEventBufferElement = collisionEventBuffer[j];

                    if (collisionEventBufferElement.IsColliding)
                    {
                        continue;
                    }

                    if (collisionEventBufferElement.State == PhysicsEventState.Exit)
                    {
                        collisionEventBuffer.RemoveAt(j);
                    }
                    else
                    {
                        collisionEventBufferElement.State = PhysicsEventState.Exit;
                        collisionEventBuffer[j] = collisionEventBufferElement;
                    }
                }
            }
        }
    }
}
