using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class TargetAuthoring : MonoBehaviour
{
}

public struct TargetPositionComponent : IComponentData
{
    public float3 targetPosition;
}

public class TargetPositionBaker : Baker<TargetAuthoring>
{
    public override void Bake(TargetAuthoring authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.None);
        AddComponent(entity, new TargetPositionComponent
        {
            targetPosition = authoring.transform.position
        });
    }
}