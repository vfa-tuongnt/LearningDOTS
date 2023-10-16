using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public partial class CameraMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        RefRW<PlayerComponent> playerComponent = SystemAPI.GetSingletonRW<PlayerComponent>();
        float3 position = playerComponent.ValueRW.position;
        Entities.ForEach((ref CameraComponent cameraComponent) =>
        {
            Camera.main.transform.position = position + cameraComponent.offset;
        }).Run();
    }
}
