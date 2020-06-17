using Unity.Entities;
using Unity.Mathematics;

public struct CollisionEventBufferElement : IBufferElementData
{
    public bool HasCollisionDetails;
    public bool IsColliding;
    public PhysicsEventState State;
    public float3 Normal;
    public Entity Entity;
    public CollisionDetails CollisionDetails;
}
