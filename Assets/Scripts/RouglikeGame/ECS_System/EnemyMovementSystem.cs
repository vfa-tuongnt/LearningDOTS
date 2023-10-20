using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class EnemyMovementSystem : SystemBase
{
    float3 _playerPosition;
    bool _isRunnable = false;

    protected override void OnStartRunning()
    {
    }

    protected override void OnUpdate()
    {
        if (_isRunnable == false)
        {
            if(SystemAPI.TryGetSingleton<PlayerComponent>(out PlayerComponent playerComponent))
            {
                _playerPosition = playerComponent.position;
                _isRunnable = true;
            }
            return;
        }

        Debug.Log("Move Enemy");
        foreach((MovementTransformAndComponent movementTransformAndComponent, RefRW<EnemyAnimateComponent> enemyAnimateComponent) in SystemAPI.Query<MovementTransformAndComponent, RefRW<EnemyAnimateComponent>>())
        {
            if(enemyAnimateComponent.ValueRW.isDead == true)
            {
                movementTransformAndComponent.Stop();
                continue;
            } 
            movementTransformAndComponent.Move(SystemAPI.Time.DeltaTime);
            // movementTransformAndComponent.CheckReachTarget();
        }
    }
}
