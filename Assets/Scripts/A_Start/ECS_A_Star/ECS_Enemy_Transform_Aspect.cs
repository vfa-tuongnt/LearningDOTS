using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public readonly partial struct ECS_Enemy_Transform_Aspect : IAspect
{
    public readonly Entity entity;

    public readonly RefRW<EnemyTag> _enemyTag;
    public readonly RefRW<LocalTransform> _localTransform;
}
