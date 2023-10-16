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
using UnityEngine.Rendering;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))] // Make sure that the running order of systems is correct
public partial class SwordCastSystem : SystemBase
{
    private float attackCooldown = 0.12f;
    private float attackRange = 1f;
    private float attackPrepare = 0f;
    private bool isAttacking;
    private float nextAttackTime;
    private float nextAttackEndTime;

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
        nextAttackTime += attackCooldown + attackPrepare; // prepare for attack and after attack
        isAttacking = true;

        RefRW<PlayerComponent> playerComponent = SystemAPI.GetSingletonRW<PlayerComponent>();
        float3 playerFaceDir = playerComponent.ValueRW.faceDirection;

        //TODO: Create a sphere collision 
        float3 direction = playerFaceDir;
        float3 origin = playerComponent.ValueRW.position;
        float radius = attackRange;
        float maxDistance = 2f;
        CollisionFilter filter = new CollisionFilter()
        {
            BelongsTo = 1u << 8,
            CollidesWith = 1u << 8, 
        };
        CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        
        // Cast and check sphere collision 
        NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);

        bool isHit = collisionWorld.SphereCastAll(origin, radius, direction, maxDistance, ref hits, filter);


        EntityCommandBuffer endCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.Unmanaged);
        RefRW<RandomComponent> randomComponent = SystemAPI.GetSingletonRW<RandomComponent>();
        Unity.Mathematics.Random random = randomComponent.ValueRW.random;
        float _animationDelay = randomComponent.ValueRW.random.NextFloat(.1f, .5f);
        float _deadDelay = _animationDelay + 0.3f;

        if(isHit)
        {
            foreach(var hit in hits)
            {
                RefRW<EnemyAnimateComponent> enemy = SystemAPI.GetComponentRW<EnemyAnimateComponent>(hit.Entity); 
                enemy.ValueRW.isDead = true; 
                enemy.ValueRW.animationID = AnimationIdsGhoulZombie.Death; 
                EntityManager.RemoveComponent<PhysicsCollider>(hit.Entity);

                RefRW<EnemyTag> enemyTag = SystemAPI.GetComponentRW<EnemyTag>(hit.Entity);
                enemyTag.ValueRW.animationDelay = _animationDelay;
                enemyTag.ValueRW.deadDelay = _deadDelay;
            }
            hits.Dispose();
        } 
    }
}
