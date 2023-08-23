using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using GPUECSAnimationBaker.Engine.AnimatorSystem;

public partial class EnemyAnimationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var enemyAnimationComponents = GetComponentLookup<EnemyAnimateComponent>();

        foreach ((GpuEcsAnimatorAspect gpuEcsAnimatorAspect, EnemyTag enemy) in SystemAPI.Query<GpuEcsAnimatorAspect, EnemyTag>())
        {
            EnemyAnimateComponent enemyAnimate = enemyAnimationComponents[enemy.parent];
            RefRW<EnemyTag> parentEnemyTag = SystemAPI.GetComponentRW<EnemyTag>(enemy.parent);
            if (enemyAnimate.isDead)
            {
                parentEnemyTag.ValueRW.animationTimer += SystemAPI.Time.DeltaTime;
                if (parentEnemyTag.ValueRW.animationTimer >= parentEnemyTag.ValueRW.animationDelay)
                {
                    int animationId = (int)enemyAnimate.animationID;
                    gpuEcsAnimatorAspect.RunAnimation(animationId);
                }
            }
        }
    }
}
