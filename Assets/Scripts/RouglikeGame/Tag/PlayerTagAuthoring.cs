using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class PlayerTagAuthoring : MonoBehaviour
{
}

public struct PlayerTag : IComponentData
{

}

public class PlayerTagBaker : Baker<PlayerTagAuthoring>
{
    public override void Bake(PlayerTagAuthoring authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.None);
        AddComponent(entity, new PlayerTag());
    }
}
