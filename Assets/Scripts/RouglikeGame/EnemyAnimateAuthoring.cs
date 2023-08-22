using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using GpuEcsAnimationBaker.Engine.Data;
using GPUECSAnimationBaker.Engine.AnimatorSystem;

public class EnemyAnimateAuthoring : MonoBehaviour
{
    public AnimationIdsGhoulZombie initAnimationState;
    public bool isDead;
}

public struct EnemyAnimateComponent : IComponentData
{
    public AnimationIdsGhoulZombie animationID;
    public bool isDead;
}

public class EnemyAnimateBaker : Baker<EnemyAnimateAuthoring>
{
    public override void Bake(EnemyAnimateAuthoring authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.None);

        AddComponent(entity, new EnemyAnimateComponent
        {
            animationID = authoring.initAnimationState
        });
    }
}
