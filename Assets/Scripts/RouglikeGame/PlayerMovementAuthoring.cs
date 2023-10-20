using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class PlayerMovementAuthoring : MonoBehaviour
{
}

public struct PlayerMovementComponent : IComponentData
{
    public float2 movement;
    public float speed;
}

public class PlayerMovementComponentBaker : Baker<PlayerMovementAuthoring>
{
    public override void Bake(PlayerMovementAuthoring authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity, new PlayerMovementComponent());
    }
}