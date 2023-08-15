using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class RandomAuthoring : MonoBehaviour
{
}

public class RandomAuthoringBacker : Baker<RandomAuthoring>
{
    public override void Bake(RandomAuthoring authoring)
    {
        Entity entity = this.GetEntity(authoring, TransformUsageFlags.None);

        AddComponent(entity, new RandomComponent
        {
            random = new Unity.Mathematics.Random(1)
        });

    }
}