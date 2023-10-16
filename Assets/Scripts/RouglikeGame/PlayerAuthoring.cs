using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class PlayerAuthoring : MonoBehaviour
{

}

public struct PlayerComponent : IComponentData
{
    public float3 faceDirection;
    public float3 position;
}

public class PlayerComponentBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.None);

        AddComponent(entity, new PlayerComponent());
    }
}