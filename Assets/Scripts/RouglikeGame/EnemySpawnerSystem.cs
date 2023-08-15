using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;

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

    protected override void OnStartRunning()
    {
        Entities.ForEach((Entity entity, ref LocalTransform localTransform, ref PlayerTag playerTag) =>
        {
            if(SystemAPI.HasComponent<PlayerTag>(entity))
            {
                _playerPosition = localTransform.Position;
            }
        }).WithoutBurst().Run();
    }

    protected override void OnUpdate()
    {

        EntityQuery enemyQuery = EntityManager.CreateEntityQuery(typeof(EnemyTag));
        NativeArray<Entity> enemyArray = enemyQuery.ToEntityArray(Allocator.Temp);
        enemySpawnerComponent = SystemAPI.GetSingletonRW<EnemySpawnerComponent>();
        randomComponent = SystemAPI.GetSingletonRW<RandomComponent>();

        EntityCommandBuffer beginBuffer = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.Unmanaged);
        EntityCommandBuffer endBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.Unmanaged);

        Entity enemyPref = enemySpawnerComponent.ValueRO._enemyPrefab;
        if(enemyArray.Length < enemySpawnerComponent.ValueRO._enemyNumber)
        {
            Entity enemyEntity = beginBuffer.Instantiate(enemyPref);
            float angle = randomComponent.ValueRW.random.NextFloat(0, 360f);
            float radius = 15f;
            float x = math.cos(angle) * radius;
            float z = math.sin(angle) * radius;
            float3 position = _playerPosition + new float3(x, 0, z);
            Debug.Log("Enemy Position: " + position);
            beginBuffer.SetComponent(enemyEntity, new LocalTransform
            {
                Position = position,
                Scale = 1,
                Rotation = quaternion.identity
            });
            beginBuffer.SetComponent(enemyEntity, new EnemyMovementComponent
            {
                speed = 2,
                targetPosition = _playerPosition
            });
        }
    }
}
