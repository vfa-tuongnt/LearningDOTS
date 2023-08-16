using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.AI;

public class EnemyMovementAuthoring : MonoBehaviour
{
    public float speed;
}

public class EnemyMovementBaker : Baker<EnemyMovementAuthoring>
{
    public override void Bake(EnemyMovementAuthoring authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity, new EnemyMovementComponent
        {
            speed = authoring.speed
        });
    }
}