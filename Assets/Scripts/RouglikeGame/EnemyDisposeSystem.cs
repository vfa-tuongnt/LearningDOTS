using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Physics;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;

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
                    EndInitializationEntityCommandBufferSystem endInitializationECBSystem = World.GetOrCreateSystemManaged<EndInitializationEntityCommandBufferSystem>();

                    EntityCommandBuffer.ParallelWriter entityParallelBuffer = endInitializationECBSystem.CreateCommandBuffer().AsParallelWriter();

                    JobHandle DisposeJob = new DisposeEnemyJob
                    {
                        enemy = enemyTag.ValueRW.parent,
                        parallelWriter = entityParallelBuffer
                    }.Schedule(this.Dependency);
                    endInitializationECBSystem.AddJobHandleForProducer(DisposeJob);
                    DisposeJob.Complete();
                }
            }
        }
    }

    [BurstCompile]
    public struct DisposeEnemyJob : IJob
    {
        public Entity enemy;
        public EntityCommandBuffer.ParallelWriter parallelWriter;

        public void Execute()
        {
            parallelWriter.DestroyEntity(2, enemy);
        }
    }
}
