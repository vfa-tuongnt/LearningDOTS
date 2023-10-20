using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Vector2 movement;
    private Vector2 inputPosition;
    [SerializeField] private float speed;

    // Update is called once per frame
    void Update()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(PlayerMovementComponent)); // Get all component in the world
        NativeArray<Entity> playerMovementComponentEntities = entityQuery.ToEntityArray(Allocator.Temp);

        if(playerMovementComponentEntities.Count() == 0)
        {
            // Check if any exist in current world
            playerMovementComponentEntities.Dispose();
            return;
        }  

        Entity playerMovementEntity = playerMovementComponentEntities[0];
        entityManager.SetComponentData(playerMovementEntity, new PlayerMovementComponent
        {
            movement = this.movement, 
            speed = speed
        });
        playerMovementComponentEntities.Dispose();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        inputPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentPosition = eventData.position;

        if(currentPosition != inputPosition)
        {
            // Move
            movement = (currentPosition - inputPosition).normalized;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        movement = Vector2.zero;
    }
}
