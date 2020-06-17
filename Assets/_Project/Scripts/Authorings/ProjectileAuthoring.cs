using Unity.Entities;
using UnityEngine;

public class ProjectileAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _damage;

    public void Convert(Entity entity, EntityManager destinationManager, GameObjectConversionSystem conversionSystem)
    {
        destinationManager.AddComponentData(entity, new IsMovementEnabled { Value = true });
        destinationManager.AddComponentData(entity, new MovementDirection());
        destinationManager.AddComponentData(entity, new MovementSpeed { Value = _movementSpeed });
        destinationManager.AddComponentData(entity, new CurrentHealth());
        destinationManager.AddComponentData(entity, new MaxHealth { Value = _maxHealth });
        destinationManager.AddComponentData(entity, new Damage { Value = _damage });
        destinationManager.AddComponentData(entity, new CollisionEventListener());
        destinationManager.AddBuffer<CollisionEventBufferElement>(entity);
    }
}
