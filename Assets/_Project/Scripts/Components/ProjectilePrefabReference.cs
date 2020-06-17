using Unity.Entities;

[GenerateAuthoringComponent]
public struct ProjectilePrefabReference : IComponentData
{
    public Entity Value;
}
