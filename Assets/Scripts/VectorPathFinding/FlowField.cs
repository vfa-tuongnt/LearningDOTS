using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;

public class FlowField : IDisposable
{
    public Cell[,] Grid { get; private set; }
    public Vector2Int GridSize { get; private set; }
    public float CellRadius { get; private set; }
    public Cell destinationCell;
 
    private float cellDiameter;
 
    public FlowField(float _cellRadius, Vector2Int _gridSize)
    {
        CellRadius = _cellRadius;
        cellDiameter = CellRadius * 2f;
        GridSize = _gridSize;
    }
 
    public void CreateGrid()
    {
        if(Grid == null)
        {
            Grid = new Cell[GridSize.x, GridSize.y];
    
            for (int x = 0; x < GridSize.x; x++)
            {
                for (int y = 0; y < GridSize.y; y++)
                {
                    Vector3 worldPos = new(cellDiameter * x + CellRadius, 0, cellDiameter * y + CellRadius);
                    Grid[x, y] = new Cell(worldPos, new Vector2Int(x, y));
                }
            }
        }
        else
        {
            for (int x = 0; x < GridSize.x; x++)
            {
                for (int y = 0; y < GridSize.y; y++)
                {
                    Grid[x, y].Init();
                }
            }
        }
    }
 
    public void CreateCostField()
    {
        Vector3 cellHalfExtents = Vector3.one * CellRadius;
        int terrainMask = LayerMask.GetMask("Wall", "Mud");
        foreach (Cell curCell in Grid)
        {
            Collider[] obstacles = Physics.OverlapBox(curCell.worldPos, cellHalfExtents, Quaternion.identity, terrainMask);
            bool hasIncreasedCost = false;
            foreach (Collider col in obstacles)
            {
                if (col.gameObject.layer == 6 || col.gameObject.layer == 8) // Walls or Enemy
                {
                    curCell.IncreaseCost(255);
                    continue;
                }
                else if (!hasIncreasedCost && col.gameObject.layer == 7) // Mud
                {
                    curCell.IncreaseCost(3);
                    hasIncreasedCost = true;
                }
            }
        }
    }
 
    public void CreateIntegrationField(Cell _destinationCell)
    {
        destinationCell = _destinationCell;

        destinationCell.cost = 0;
        destinationCell.bestCost = 0;
 
        Queue<Cell> cellsToCheck = new Queue<Cell>();
 
        cellsToCheck.Enqueue(destinationCell);
 
        while(cellsToCheck.Count > 0)
        {
            Cell curCell = cellsToCheck.Dequeue();
            List<Cell> curNeighbors = GetNeighborCells(curCell.gridIndex, GridDirection.CardinalDirections);
            foreach (Cell curNeighbor in curNeighbors)
            {
                if (curNeighbor.cost == byte.MaxValue) { continue; }
                if (curNeighbor.cost + curCell.bestCost < curNeighbor.bestCost)
                {
                    curNeighbor.bestCost = (ushort)(curNeighbor.cost + curCell.bestCost);
                    cellsToCheck.Enqueue(curNeighbor);
                }
            }
        }
    }
 
    public void CreateFlowField()
    {
        foreach(Cell curCell in Grid)
        {
            List<Cell> curNeighbors = GetNeighborCells(curCell.gridIndex, GridDirection.AllDirections);
 
            int bestCost = curCell.bestCost;
 
            foreach(Cell curNeighbor in curNeighbors)
            {
                if(curNeighbor.bestCost < bestCost)
                {
                    bestCost = curNeighbor.bestCost;
                    curCell.bestDirection = GridDirection.GetDirectionFromV2I(curNeighbor.gridIndex - curCell.gridIndex);
                }
            }
        }
    }
 
    private List<Cell> GetNeighborCells(Vector2Int nodeIndex, List<GridDirection> directions)
    {
        List<Cell> neighborCells = new();
 
        foreach (Vector2Int curDirection in directions)
        {
            Cell newNeighbor = GetCellAtRelativePos(nodeIndex, curDirection);
            if (newNeighbor != null)
            {
                neighborCells.Add(newNeighbor);
            }
        }
        return neighborCells;
    }
 
    private Cell GetCellAtRelativePos(Vector2Int originalPos, Vector2Int relativePos)
    {
        Vector2Int finalPos = originalPos + relativePos;
 
        if (finalPos.x < 0 || finalPos.x >= GridSize.x || finalPos.y < 0 || finalPos.y >= GridSize.y)
        {
            return null;
        }
 
        else { return Grid[finalPos.x, finalPos.y]; }
    }
 
    public Cell GetCellFromWorldPos(Vector3 worldPos)
    {
        float percentX = worldPos.x / (GridSize.x * cellDiameter);
        float percentY = worldPos.z / (GridSize.y * cellDiameter);
 
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
 
        int x = Mathf.Clamp(Mathf.FloorToInt((GridSize.x) * percentX), 0, GridSize.x - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt((GridSize.y) * percentY), 0, GridSize.y - 1);
        return Grid[x, y];
    }

    public Vector3 GetRandomCellPosition()
    {
        int randX = UnityEngine.Random.Range(0, GridSize.x);
        int randY = UnityEngine.Random.Range(0, GridSize.y);
        Cell randCell = Grid[randX, randY];
        while(randCell.bestCost == ushort.MaxValue)
        {
            randX = UnityEngine.Random.Range(0, GridSize.x);
            randY = UnityEngine.Random.Range(0, GridSize.y);
            randCell = Grid[randX, randY];
        }
        return randCell.worldPos;
    }

    public void Dispose()
    {
        Grid = null;
        destinationCell = null;
    }
}
