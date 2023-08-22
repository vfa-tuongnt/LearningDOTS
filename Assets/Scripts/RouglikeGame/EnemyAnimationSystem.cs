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

        Entities.ForEach((GpuEcsAnimatorAspect gpuEcsAnimatorAspect, EnemyTag enemy) =>
        {
            EnemyAnimateComponent enemyAnimate = enemyAnimationComponents[enemy.parent];
            // Debug.Log("AnimationID: " + (int)enemyAnimate.animationID);
            int animationId = (int)enemyAnimate.animationID;
            gpuEcsAnimatorAspect.RunAnimation(animationId);
        }).WithoutBurst().Run();
    }
}
