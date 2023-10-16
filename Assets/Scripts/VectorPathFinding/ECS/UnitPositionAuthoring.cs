using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor.PackageManager;
using UnityEngine;

public class UnitPositionAuthoring : MonoBehaviour
{

}

public struct UnitPositionComponent : IComponentData
{
    public float3 position;
    public Vector2Int direction;
}

public class UnitPositionBaker : Baker<UnitPositionAuthoring>
{
    public override void Bake(UnitPositionAuthoring authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity, new UnitPositionComponent
        {
            position = authoring.transform.position,
            direction = new Vector2Int()
        });
    }
}