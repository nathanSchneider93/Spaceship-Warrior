using Unity.Entities;

[GenerateAuthoringComponent]
public struct BarrelReference : IComponentData
{
    public Entity Value;
}