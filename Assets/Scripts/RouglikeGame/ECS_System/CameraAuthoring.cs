using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class CameraAuthoring : MonoBehaviour
{
    public Vector3 offset;
}
public struct CameraComponent : IComponentData
{
    public float3 offset;
}
public class CameraComponentBaker : Baker<CameraAuthoring>
{
    public override void Bake(CameraAuthoring authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity, new CameraComponent
        {
            offset = authoring.offset
        });
    }
}
