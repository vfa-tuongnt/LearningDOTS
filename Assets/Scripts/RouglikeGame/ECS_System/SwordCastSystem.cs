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
    private float attackCooldown = 1.5f;
    private float attackRange = 3f;
    private float attackPrepare = .5f;
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

    private unsafe void Attack()
    {
        nextAttackTime = (float)SystemAPI.Time.ElapsedTime;
        // nextAttackEndTime = (float)SystemAPI.Time.ElapsedTime + attackDuration;
        nextAttackTime += attackCooldown + attackPrepare; // prepare for attack and after attack
        isAttacking = true;

        RefRW<PlayerComponent> playerComponent = SystemAPI.GetSingletonRW<PlayerComponent>();
        float3 playerFaceDir = playerComponent.ValueRW.faceDirection;

        //TODO: Create a sphere collision 
        float3 origin = new float3(1, 0, 1) * playerFaceDir;
        float radius = attackRange;
        float3 direction = playerFaceDir;
        float maxDistance = 10f;
        var filter = new CollisionFilter()
        {
            BelongsTo = 1u << 6,
            CollidesWith = 1u << 6, // all 1s, so all layers, collide with everything
            GroupIndex = 0
        };
        CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        
        // Cast and check sphere collision 
        NativeList<ColliderCastHit> hits = new NativeList<ColliderCastHit>(Allocator.Temp);

        bool isHit = collisionWorld.SphereCastAll(playerFaceDir, radius, direction, maxDistance, ref hits, filter);

        if(isHit == true)
        {
            foreach(var hit in hits)
            {
                RefRW<EnemyAnimateComponent> enemy = SystemAPI.GetComponentRW<EnemyAnimateComponent>(hit.Entity);
                enemy.ValueRW.isDead = true;
                enemy.ValueRW.animationID = AnimationIdsGhoulZombie.Death;
            }
        } 

        hits.Dispose();
    }
}
