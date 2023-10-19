using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Core;
using Unity.Collections;

public partial class PlayerUpdateSystem : SystemBase
{
    private float attackCooldown = 0.75f;
    private float attackRange = 5f;
    private float attackDuration = 1.25f;
    private bool isAttacking;
    private float nextAttackTime;
    private float nextAttackEndTime;

    protected override void OnStartRunning()
    {
        nextAttackTime = (float) SystemAPI.Time.ElapsedTime + attackCooldown;
    }

    protected override void OnUpdate()
    {
        // Loop all the enemy to get the direction to the closest one
        LoopAllEnemies();
    }


    private void LoopAllEnemies()
    {
        float closestDistance = 100f;
        float3 dir = new Vector3();

        EntityCommandBuffer endBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.Unmanaged);
        if(SystemAPI.TryGetSingletonRW<PlayerComponent>(out RefRW<PlayerComponent> playerComponent))
        {
            foreach(ECS_Player_Transform_Aspect playerTransformAspect in SystemAPI.Query<ECS_Player_Transform_Aspect>())
            {
                foreach((ECS_Enemy_Transform_Aspect enemyTransformAspect, RefRW<EnemyAnimateComponent> enemyAnimation) in SystemAPI.Query<ECS_Enemy_Transform_Aspect, RefRW<EnemyAnimateComponent>>())
                {
                    if(enemyAnimation.ValueRW.isDead == true) continue;

                    float distance = math.distance(enemyTransformAspect._localTransform.ValueRW.Position, playerTransformAspect._localTransform.ValueRW.Position);
                    float3 enemyDir = math.normalize(enemyTransformAspect._localTransform.ValueRW.Position - playerTransformAspect._localTransform.ValueRW.Position);
                    if(distance < closestDistance) 
                    {
                        closestDistance = distance;
                        dir = enemyDir;
                        playerComponent.ValueRW.faceDirection = enemyDir;
                    }
                }
                playerComponent.ValueRW.position = playerTransformAspect._localTransform.ValueRW.Position;
                playerTransformAspect.FaceToDirection(dir);
            }
        }
    }

    private bool IsCloseSameDir(float3 dir1, float3 dir2)
    {
        // float checkRange = 0.5f;
        float angle1 = math.atan2(dir1.x, dir1.z);
        float angle2 = math.atan2(dir2.x, dir2.z);
        float angleDeg1 = math.degrees(angle1);
        float angleDeg2 = math.degrees(angle2);

        if(angleDeg1- angleDeg2 <= 10)
        {
            return true;
        }
        return false;
    }
}