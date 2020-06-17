using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct MovementDirection : IComponentData
{
    public float3 Value;
}
