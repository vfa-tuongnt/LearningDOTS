using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Physics;
using Unity.Entities;
using Unity.Physics.Systems;

public partial class EnemyDisposeSystem : SystemBase
{
    protected override void OnUpdate()
    {
        foreach((RefRW<EnemyTag> enemyTag, RefRW<EnemyAnimateComponent> enemyAnimateComponent) in SystemAPI.Query<RefRW<EnemyTag>, RefRW<EnemyAnimateComponent>>()) 
        {
            if (enemyAnimateComponent.ValueRW.isDead == true)
            {
                enemyTag.ValueRW.deadTimer += SystemAPI.Time.DeltaTime;
                if (enemyTag.ValueRW.deadTimer >= enemyTag.ValueRW.deadDelay)
                {
                    DestroyEnemy(enemyTag.ValueRW.parent);
                }
            }
        }
    }

    void DestroyEnemy(Entity entity)
    {
        EntityCommandBuffer endCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.Unmanaged);

        endCommandBuffer.DestroyEntity(entity);
    }
}
