using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Mathematics;
using Unity.Collections;
using GPUECSAnimationBaker.Engine.AnimatorSystem;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))] // Make sure that the running order of systems is correct
public partial class SwordCastSystem : SystemBase
{
    private float attackCooldown = 0.12f;
    private float attackRange = .5f;
    private float attackPrepare = 0f;
    private bool isAttacking;
    private float nextAttackTime;
    private float nextAttackEndTime;
    RefRW<RandomComponent> randomComponent;

    protected override void OnStartRunning()
    {
        nextAttackTime = (float)SystemAPI.Time.ElapsedTime + attackCooldown;
    }

    protected override void OnUpdate()
    {
        if((float) SystemAPI.Time.ElapsedTime >= nextAttackTime)
        {
            Attack();
        }
    }

    private void Attack()
    {
        nextAttackTime = (float)SystemAPI.Time.ElapsedTime;
        // nextAttackEndTime = (float)SystemAPI.Time.ElapsedTime + attackDuration;
        nextAttackTime += attackCooldown + attackPrepare; // prepare for attack and after attack
        isAttacking = true;

        RefRW<PlayerComponent> playerComponent = SystemAPI.GetSingletonRW<PlayerComponent>();
        float3 playerFaceDir = playerComponent.ValueRW.faceDirection;

        //TODO: Create a sphere collision 
        float3 origin = new float3(1.5f, 0, 1.5f) * playerFaceDir;
        float radius = attackRange;
        float3 direction = playerFaceDir;
        float maxDistance = 10f;
        var filter = new CollisionFilter()
        {
            BelongsTo = 1u << 6,
            CollidesWith = 1u << 6, 
            GroupIndex = 0
        };
        CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        
        // Cast and check sphere collision 
        NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);

        bool isHit = collisionWorld.SphereCastAll(playerFaceDir, radius, direction, maxDistance, ref hits, filter);

        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.Unmanaged);

        randomComponent = SystemAPI.GetSingletonRW<RandomComponent>();
        if(isHit == true)
        {
            foreach(var hit in hits)
            {
                RefRW<EnemyAnimateComponent> enemy = SystemAPI.GetComponentRW<EnemyAnimateComponent>(hit.Entity);
                enemy.ValueRW.isDead = true;
                enemy.ValueRW.animationID = AnimationIdsGhoulZombie.Death;
                EntityManager.RemoveComponent<PhysicsCollider>(hit.Entity);

                Entity enemyEntity = hit.Entity;

                // float animationDelay = randomComponent.ValueRW.random.NextFloat(1, 2.5f);
                // float deadTimer = randomComponent.ValueRW.random.NextFloat(0, 0.5f);

                // entityCommandBuffer.SetComponent(enemyEntity, new EnemyTag
                // {
                //     parent = hit.Entity,
                //     animationDelay = animationDelay,
                //     deadTimer = deadTimer,
                //     isBeginDeadAnim = false,
                // });
            }
        } 

        hits.Dispose();
    }
}
