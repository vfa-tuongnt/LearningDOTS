using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;

public readonly partial struct MovementTransformAndComponent : IAspect
{
    private readonly Entity entity; //Which entity is running this aspect

    public readonly RefRW<LocalTransform> transform;
    public readonly RefRW<EnemyMovementComponent> movementComponent;


    public void Move(float deltaTime)
    {
        if(IsGotToTarget()) return;
        movementComponent.ValueRW.targetPosition.y = transform.ValueRO.Position.y;
        float3 dir = math.normalize(movementComponent.ValueRO.targetPosition - transform.ValueRW.Position);
        FaceToTarget(dir);
        transform.ValueRW.Position += dir * deltaTime * movementComponent.ValueRO.speed;
    }

    public void CheckReachTarget()
    {
        if(IsGotToTarget())
        {
            // Reach Target

        }
    }

    public void FaceToTarget(float3 dir)
    {
        float angle = math.atan2(dir.x, dir.z);
        float angleDeg = math.degrees(angle);

        Quaternion qua = quaternion.Euler(0, angle, 0);

        transform.ValueRW.Rotation = qua;
    }

    public float3 GetRandomPosition(RefRW<RandomComponent> randomComponent)
    {
        return new float3(randomComponent.ValueRW.random.NextFloat(-10, 10), 0, randomComponent.ValueRW.random.NextFloat(-10, 10));
    }

    private bool IsGotToTarget()
    {
        return math.distance(transform.ValueRO.Position, movementComponent.ValueRO.targetPosition) < 1f;
    }
}
