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
        float2 movement = new()
        {
            x = Input.GetAxisRaw("Horizontal"),
            y = Input.GetAxisRaw("Vertical")
        };

        if (movement.x != 0 || movement.y != 0)
        {
            // move
            Entities.ForEach((ECS_Player_Transform_Aspect player_Transform_Aspect) =>
            {
                player_Transform_Aspect.Move(movement, 5, SystemAPI.Time.DeltaTime);
            }).Run();
        }
    }
}
