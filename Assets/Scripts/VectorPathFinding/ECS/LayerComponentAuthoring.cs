using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;

public class LayerComponentAuthoring : MonoBehaviour
{
    
}

public struct LayerComponent : IComponentData
{
    public int layerMask;
}

public class LayerComponentBaker : Baker<LayerComponentAuthoring>
{
    public override void Bake(LayerComponentAuthoring authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.None);
        AddComponent(entity, new LayerComponent
        {
            layerMask = authoring.gameObject.layer
        });
    }
}