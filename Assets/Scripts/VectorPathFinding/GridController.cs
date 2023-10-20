using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
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
        curFlowField?.Dispose();
        curFlowField = new FlowField(cellRadius, gridSize);
        curFlowField.CreateGrid();
        gridDebug.SetFlowField(curFlowField);
    }

    void Awake()
    {
        _instance = this;
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        InitializedFlowField();
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

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(TargetPositionComponent));
        NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
        if (entityArray.Count() > 0)
        {
            Entity target = entityArray[0];
            TargetPositionComponent targetPositionComponent = entityManager.GetComponentData<TargetPositionComponent>(target);

            Cell desCell = curFlowField.GetCellFromWorldPos(targetPositionComponent.targetPosition);
            if (desCell.worldPos != prePos || prePos == null)
            {
                if (delayTimer >= 0.2f)
                {
                    SetTarget(targetPositionComponent.targetPosition);
                    delayTimer = 0;
                }
            }
        }
        entityArray.Dispose();
        entityQuery.Dispose();
    }

    public void UpdateUnitDirection()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery unitQuery = entityManager.CreateEntityQuery(typeof(UnitPositionComponent));
        NativeArray<Entity> units = unitQuery.ToEntityArray(Allocator.Temp);
        if (units.Count() > 0)
        {
            foreach (Entity unit in units)
            {
                LocalTransform unitLocalTransform = entityManager.GetComponentData<LocalTransform>(unit);
                float3 unitPosition = unitLocalTransform.Position;

                UnitPositionComponent unitPositionComponent = entityManager.GetComponentData<UnitPositionComponent>(unit);
                Cell unitCell = curFlowField.GetCellFromWorldPos(unitPosition);
                Vector2Int moveDir = unitCell.bestDirection.Vector;
                unitPositionComponent.direction = moveDir;

                Debug.Log("GridController Mono enemyPosition: " + unitLocalTransform.Position);
                entityManager.SetComponentData(unit, unitPositionComponent);
            }
        }
        units.Dispose();
        unitQuery.Dispose();
    }
}