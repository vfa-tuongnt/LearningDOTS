using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;


public partial class EnemyDisposeSystem : SystemBase
{
    protected override void OnUpdate()
    {
        List<Entity> deadEnemy = new List<Entity>();

        foreach((RefRW<EnemyTag> enemyTag, RefRW<EnemyAnimateComponent> enemyAnimateComponent) in SystemAPI.Query<RefRW<EnemyTag>, RefRW<EnemyAnimateComponent>>()) 
        {
            if (enemyAnimateComponent.ValueRW.isDead == true)
            {
                enemyTag.ValueRW.deadTimer += SystemAPI.Time.DeltaTime;
                if (enemyTag.ValueRW.deadTimer >= enemyTag.ValueRW.deadDelay)
                {
                    BeginInitializationEntityCommandBufferSystem beginInitSystem = World.GetOrCreateSystemManaged<BeginInitializationEntityCommandBufferSystem>();

                    EntityCommandBuffer.ParallelWriter entityParallelBuffer = beginInitSystem.CreateCommandBuffer().AsParallelWriter();

                    this.Dependency = new DisposeEnemyJob
                    {
                        enemy = enemyTag.ValueRW.parent,
                        parallelWriter = entityParallelBuffer
                    }.ScheduleParallel(this.Dependency);
                    this.Dependency.Complete();
                }
            }
        }
    }

    [BurstCompile]
    public partial struct DisposeEnemyJob : IJobEntity
    {
        public Entity enemy;
        public EntityCommandBuffer.ParallelWriter parallelWriter;

        public void Execute()
        {
            parallelWriter.DestroyEntity(2, enemy);
        }
    }
}
