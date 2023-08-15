using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct EnemyMovementComponent : IComponentData
{
    public float speed;
    public float3 targetPosition;
}
