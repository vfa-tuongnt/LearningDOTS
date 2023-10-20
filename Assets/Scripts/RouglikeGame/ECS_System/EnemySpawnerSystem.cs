using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Jobs;

public class EnemySpawnerSystem : MonoBehaviour
{
    public float _enemyNumber = 200;
    public GameObject enemyPref;
}

public struct EnemySpawnerComponent : IComponentData
{
    public Entity _enemyPrefab;
    public float _enemyNumber;
}

public class EnemySpawnerBaker : Baker<EnemySpawnerSystem>
{
    public override void Bake(EnemySpawnerSystem authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.None);

        AddComponent(entity, new EnemySpawnerComponent
        {
            _enemyNumber = authoring._enemyNumber,
            _enemyPrefab = GetEntity(authoring.enemyPref, TransformUsageFlags.None)
        });
    }
}

public partial class EnemySpawnerSystemEntity : SystemBase
{
    public RefRW<RandomComponent> randomComponent;
    public RefRW<EnemySpawnerComponent> enemySpawnerComponent;
    float3 _playerPosition = new float3(0, 0, 0);
    FlowField _flowField;
    private bool _isSetup;

    protected override void OnUpdate()
    {
        if(RightPad.Instance != null && _isSetup == false)
        {
            _isSetup = true;
            RightPad.OnClick += Spawn5Enemy;
        }
    }

    private void Spawn5Enemy()
    {
        _flowField = GridController.Instance.curFlowField;
        if(!SystemAPI.TryGetSingletonRW<EnemySpawnerComponent>(out enemySpawnerComponent))
        {
            return;
        }

        BeginInitializationEntityCommandBufferSystem beginInitializationECBSystem = World.GetOrCreateSystemManaged<BeginInitializationEntityCommandBufferSystem>();

        EntityCommandBuffer.ParallelWriter entityParallelBuffer = beginInitializationECBSystem.CreateCommandBuffer().AsParallelWriter();
        for (int i = 0; i < 5; i++)
        {
            float3 newPosition = _flowField.GetRandomCellPosition();
            newPosition.y = 0;

            JobHandle SpawnJob = new SpawnEnemyJob
            {
                beginBuffer = entityParallelBuffer,
                enemyPrefab = enemySpawnerComponent.ValueRW._enemyPrefab,
                position = newPosition
            }.Schedule(this.Dependency);
            beginInitializationECBSystem.AddJobHandleForProducer(SpawnJob);
            SpawnJob.Complete();
        }
    }

    [BurstCompile]
    public struct SpawnEnemyJob : IJob
    {
        public EntityCommandBuffer.ParallelWriter beginBuffer;
        public Entity enemyPrefab;
        public float3 position;

        public void Execute()
        {
            Entity enemy =  beginBuffer.Instantiate(1, enemyPrefab);

            beginBuffer.SetComponent(1, enemy, new LocalTransform{
                Position = position,
                Scale = 1,
                Rotation = quaternion.identity
            });
        }
    }
}
