using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class EnemyMovementSystem : SystemBase
{
    float3 _playerPosition;

    protected override void OnStartRunning()
    {
        Entities.ForEach((Entity entity, ref LocalTransform localTransform) =>
        {
            if(SystemAPI.HasComponent<PlayerTag>(entity))
            {
                _playerPosition = localTransform.Position;
            }
        }).WithoutBurst().Run();
    }

    protected override void OnUpdate()
    {
        foreach((MovementTransformAndComponent movementTransformAndComponent, RefRW<EnemyAnimateComponent> enemyAnimateComponent) in SystemAPI.Query<MovementTransformAndComponent, RefRW<EnemyAnimateComponent>>())
        {
            if(enemyAnimateComponent.ValueRW.isDead == true)
            {
                continue;
            } 
            movementTransformAndComponent.Move(SystemAPI.Time.DeltaTime);
            movementTransformAndComponent.CheckReachTarget();
        }
    }
}
