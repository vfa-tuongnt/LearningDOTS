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

    protected override void OnUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            enemySpawnerComponent = SystemAPI.GetSingletonRW<EnemySpawnerComponent>();

            EntityCommandBuffer beginBuffer = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(World.Unmanaged);
            for (int i = 0; i < 5; i++)
            {
                Entity enemy = beginBuffer.Instantiate(enemySpawnerComponent.ValueRW._enemyPrefab);

                float3 newPosition = GridController.Instance.curFlowField.GetRandomCellPosition();
                newPosition.y = 0;

                beginBuffer.SetComponent(enemy, new LocalTransform{
                    Position = newPosition,
                    Scale = 1,
                    Rotation = quaternion.identity
                });
            }
        }
    }
}
