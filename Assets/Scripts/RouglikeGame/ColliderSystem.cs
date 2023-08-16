using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Burst;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSimulationGroup))]
public partial struct ColliderSystem : ISystem
{

    [BurstCompile]
    public partial struct TestCollider : ICollisionEventsJob
    {
        public void Execute(CollisionEvent collisionEvent)
        {
            Debug.Log($"Collision between entities { collisionEvent.EntityA.Index } and { collisionEvent.EntityB.Index }");
        }
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // state.Dependency = new TestCollider
        // {
        // }.Schedule()
    }
}

