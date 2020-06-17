using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public sealed partial class CollisionEventSystem
{
    [BurstCompile]
    private struct ProcessJob : ICollisionEventsJob
    {
        [ReadOnly] public PhysicsWorld PhysicsWorld;
        [ReadOnly] public ComponentDataFromEntity<CollisionEventListener> CollisionEventListenerGroup;

        public BufferFromEntity<CollisionEventBufferElement> CollisionEventBufferGroup;

        public void Execute(CollisionEvent collisionEvent)
        {
            CollisionEvent.Details collisionEventDetails = default;

            var calculateDetailsForA = false;
            var calculateDetailsForB = false;

            if (CollisionEventListenerGroup.Exists(collisionEvent.Entities.EntityA))
            {
                calculateDetailsForA = CollisionEventListenerGroup[collisionEvent.Entities.EntityA].CalculateDetails;
            }

            if (CollisionEventListenerGroup.Exists(collisionEvent.Entities.EntityB))
            {
                calculateDetailsForB = CollisionEventListenerGroup[collisionEvent.Entities.EntityB].CalculateDetails;
            }

            if (calculateDetailsForA || calculateDetailsForB)
            {
                collisionEventDetails = collisionEvent.CalculateDetails(ref PhysicsWorld);
            }

            if (CollisionEventBufferGroup.Exists(collisionEvent.Entities.EntityA))
            {
                ProcessEntity(collisionEvent.Entities.EntityA, collisionEvent.Entities.EntityB, collisionEvent.Normal, calculateDetailsForA, collisionEventDetails);
            }

            if (CollisionEventBufferGroup.Exists(collisionEvent.Entities.EntityB))
            {
                ProcessEntity(collisionEvent.Entities.EntityB, collisionEvent.Entities.EntityA, collisionEvent.Normal, calculateDetailsForB, collisionEventDetails);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ProcessEntity(Entity entity, Entity otherEntity, float3 normal, bool calculateDetails, CollisionEvent.Details collisionEventDetails)
        {
            DynamicBuffer<CollisionEventBufferElement> collisionEventBuffer = CollisionEventBufferGroup[entity];

            for (var i = 0; i < collisionEventBuffer.Length; i++)
            {
                CollisionEventBufferElement collisionEventBufferElement = collisionEventBuffer[i];

                if (collisionEventBufferElement.Entity != otherEntity)
                {
                    continue;
                }

                collisionEventBufferElement.HasCollisionDetails = calculateDetails;
                collisionEventBufferElement.IsColliding = true;
                collisionEventBufferElement.State = collisionEventBufferElement.State == PhysicsEventState.Exit ? PhysicsEventState.Enter : PhysicsEventState.Stay;
                collisionEventBufferElement.Normal = normal;
                collisionEventBufferElement.CollisionDetails.AverageContactPointPosition = collisionEventDetails.AverageContactPointPosition;
                collisionEventBufferElement.CollisionDetails.EstimatedImpulse = collisionEventDetails.EstimatedImpulse;
                collisionEventBuffer[i] = collisionEventBufferElement;

                return;
            }

            collisionEventBuffer.Add(new CollisionEventBufferElement
            {
                HasCollisionDetails = calculateDetails,
                IsColliding = true,
                State = PhysicsEventState.Enter,
                Normal = normal,
                Entity = otherEntity,
                CollisionDetails = new CollisionDetails
                {
                    EstimatedImpulse = collisionEventDetails.EstimatedImpulse,
                    AverageContactPointPosition = collisionEventDetails.AverageContactPointPosition
                }
            });
        }
    }
}
