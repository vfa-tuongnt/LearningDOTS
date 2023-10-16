using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class GridController : MonoBehaviour
{
    private static GridController _instance;
    public static GridController Instance => _instance;

    public Vector2Int gridSize;
    public float cellRadius = 0.5f;
    public FlowField curFlowField;
    public GridDebug gridDebug;

    private Vector3 prePos;
    private EntityManager entityManager;
    private float delayTimer;

    private void InitializedFlowField()
    {
        curFlowField = new FlowField(cellRadius, gridSize);
        curFlowField.CreateGrid();
        gridDebug.SetFlowField(curFlowField);
    }

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        InitializedFlowField();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
    
    void Update()
    {
        UpdateTargetPos();
        UpdateUnitDirection();
    }

    public void SetTarget(Vector3 position)
    {
        curFlowField.CreateGrid();

        Cell desCell = curFlowField.GetCellFromWorldPos(position);
        prePos = desCell.worldPos;
        curFlowField.CreateCostField(); // Create cost for each node
        curFlowField.CreateIntegrationField(desCell); // Create cost to go to desCell
        curFlowField.CreateFlowField(); // find direction to go to des cell
        gridDebug.DrawFlowField();
    }

    public void UpdateTargetPos()
    {
        delayTimer += Time.deltaTime;
        if(entityManager == null) return;

        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(TargetPositionComponent));
        NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);

        Entity target = entityArray[0];
        TargetPositionComponent targetPositionComponent = entityManager.GetComponentData<TargetPositionComponent>(target);

        Cell desCell = curFlowField.GetCellFromWorldPos(targetPositionComponent.targetPosition);
        if (desCell.worldPos != prePos || prePos == null)
        {
            if(delayTimer >= 0.2f)
            {
                SetTarget(targetPositionComponent.targetPosition);
                delayTimer = 0;
            }
        }
    }

    public void UpdateUnitDirection()
    {
        if(entityManager == null)
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery unitQuery = entityManager.CreateEntityQuery(typeof(UnitPositionComponent));
        NativeArray<Entity> units = unitQuery.ToEntityArray(Allocator.Temp);
        foreach(Entity unit in units)
        {
            float3 unitPosition = entityManager.GetComponentData<UnitPositionComponent>(unit).position;
            Cell unitCell = curFlowField.GetCellFromWorldPos(unitPosition);
            Vector2Int moveDir = unitCell.bestDirection.Vector;
            entityManager.SetComponentData(unit, new UnitPositionComponent
            {
                position = unitPosition,
                direction = moveDir
            });
        }
    }
}