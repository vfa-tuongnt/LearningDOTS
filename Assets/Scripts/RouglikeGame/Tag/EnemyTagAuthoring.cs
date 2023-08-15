using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class EnemyTagAuthoring : MonoBehaviour
{
}

public struct EnemyTag : IComponentData
{
}

public class EnemyTagBaker : Baker<EnemyTagAuthoring>
{
    public override void Bake(EnemyTagAuthoring authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.None);

        AddComponent(entity, new EnemyTag());
    }
}