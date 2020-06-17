using Unity.Entities;

[GenerateAuthoringComponent]
public struct CurrentHealth : IComponentData
{
    public int Value;
}
