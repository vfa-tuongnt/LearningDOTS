using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Extension;

public readonly partial struct ECS_Player_Transform_Aspect : IAspect
{
    public readonly Entity entity;

    public readonly RefRW<PlayerTag> _playerTag;
    public readonly RefRW<LocalTransform> _localTransform;
    public readonly RefRW<PlayerMovementComponent> _playerMovementComponent;

    public void Move(float deltaTime)
    {
        float speed = _playerMovementComponent.ValueRW.speed;
        float2 movement = _playerMovementComponent.ValueRW.movement;
        _localTransform.ValueRW.Position += movement.ToFloat3XZ() * speed * deltaTime;
    }

    public void FaceToTargetPos(float3 target)
    {
        float3 dir = math.normalize(target - _localTransform.ValueRW.Position);
        float angle = math.atan2(dir.x, dir.z);
        // float angleDeg = math.degrees(angle);

        Quaternion qua = quaternion.Euler(0, angle, 0);

        _localTransform.ValueRW.Rotation = qua;
    }

    public void FaceToDirection(float3 dir)
    {
        float angle = math.atan2(dir.x, dir.z);
        // float angleDeg = math.degrees(angle);

        Quaternion qua = quaternion.Euler(0, angle, 0);

        _localTransform.ValueRW.Rotation = qua;
    }
}
