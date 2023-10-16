using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell 
{
    public Vector3 worldPos;
    public Vector2Int gridIndex;
    public byte cost;
    public ushort bestCost;
    public GridDirection bestDirection;
 
    public Cell(Vector3 _worldPos, Vector2Int _gridIndex)
    {
        worldPos = _worldPos;
        gridIndex = _gridIndex;
        cost = 1;
        bestCost = ushort.MaxValue;
        bestDirection = GridDirection.None;
    }

    public void Init()
    {
        cost = 1;
        bestCost = ushort.MaxValue;
        bestDirection = GridDirection.None;
    }
 
    public void IncreaseCost(int amount)
    {
        if (cost == byte.MaxValue) { return; }
        if (amount + cost >= 255) { cost = byte.MaxValue; }
        else { cost += (byte)amount; }
    }
}
