using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class EnemyTagAuthoring : MonoBehaviour
{
    public GameObject parent;
}

public struct EnemyTag : IComponentData
{
    public Entity parent;
    public float timer;
    public float deadTimer;
    public float animationTimer;
    public float animationDelay;
}

public class EnemyTagBaker : Baker<EnemyTagAuthoring>
{
    public override void Bake(EnemyTagAuthoring authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.None);

        AddComponent(entity, new EnemyTag
        {
            parent = GetEntity(authoring.parent, TransformUsageFlags.None),
            timer = 0
        });
    }
}