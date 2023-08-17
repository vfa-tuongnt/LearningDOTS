using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class ECS_Start_Authoring : MonoBehaviour
{
}

public struct ECS_Start_Component : IComponentData
{
}

public class ECS_Start_Baker : Baker<ECS_Start_Authoring>
{
    public override void Bake(ECS_Start_Authoring authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.None);

        AddComponent(entity, new ECS_Start_Component());
    }
}