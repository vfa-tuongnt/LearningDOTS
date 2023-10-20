using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public partial class CameraMovementSystem : SystemBase
{

    protected override void OnCreate()
    {
    }

    protected override void OnUpdate()
    {
        if (SystemAPI.TryGetSingletonRW<PlayerComponent>(out RefRW<PlayerComponent> playerComponent))
        {
            float3 position = playerComponent.ValueRW.position;
            GameObject camera = Camera.main.gameObject;
            if (camera == null) return;
            if(SystemAPI.TryGetSingleton<CameraComponent>(out CameraComponent cameraComponent))
                camera.transform.position = position + cameraComponent.offset;
        }
    }
}
