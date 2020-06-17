using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private int _maxHealth;
    [SerializeField] private GameObject _gunGameObject;

    public void Convert(Entity entity, EntityManager destinationManager, GameObjectConversionSystem conversionSystem)
    {
        destinationManager.AddComponentData(entity, new IsMovementEnabled { Value = true });
        destinationManager.AddComponentData(entity, new MovementDirection());
        destinationManager.AddComponentData(entity, new MovementSpeed { Value = _movementSpeed });
        destinationManager.AddComponentData(entity, new AimScreenPosition());
        destinationManager.AddComponentData(entity, new AimWorldPosition());
        destinationManager.AddComponentData(entity, new CurrentHealth());
        destinationManager.AddComponentData(entity, new MaxHealth { Value = _maxHealth });
        destinationManager.AddComponentData(entity, new GunReference { Value = conversionSystem.GetPrimaryEntity(_gunGameObject) });
        destinationManager.AddComponentData(entity, new PlayerTag());
    }
}
