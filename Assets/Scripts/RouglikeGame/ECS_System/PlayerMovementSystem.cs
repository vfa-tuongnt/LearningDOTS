using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class PlayerMovementSystem : SystemBase
{
    protected override void OnCreate()
    {

    }

    protected override void OnUpdate()
    {
        // move
        foreach(ECS_Player_Transform_Aspect player_Transform_Aspect in SystemAPI.Query<ECS_Player_Transform_Aspect>())
        {
            player_Transform_Aspect.Move(SystemAPI.Time.DeltaTime);
        }
    }
}
