using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public partial class FlowFieldECS : SystemBase
{
    GridController gridController;
    float3 playerPosition;

    protected override void OnUpdate()
    {
        Entities.ForEach((ref TargetPositionComponent target, ref LocalTransform localTransform) =>
        {
            // Debug.Log("ECS: FlowFieldECS");
            target.targetPosition = localTransform.Position;
        }).ScheduleParallel();
    }
}
