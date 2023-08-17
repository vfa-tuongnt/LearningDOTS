using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class ECS_Start_Transform_Aspect : IAspect
{
    public readonly RefRW<LocalTransform> localTransform;
}
